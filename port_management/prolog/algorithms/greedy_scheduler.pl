:- module(greedy_scheduler, [schedule_greedy/2]).

:- use_module(library(lists)).
:- use_module(iarti_core).
:- use_module(optimal_scheduler, [
    create_error_output/3,
    get_dict_default/4,
    validate_inputs/5
]).
% REUTILIZAÇÃO TOTAL: Usamos o motor do Multi-Crane para calcular tempos e JSON
:- use_module(multi_crane_scheduler, [
    sequence_temporization_multi/4,
    sum_delays_multi/2,
    convert_multi_to_json/4
]).

% ============================================
% US 3.4.4: GREEDY SCHEDULER (DOCK AWARE)
% ============================================
schedule_greedy(Input, Output) :-
    get_time(StartTime),
    
    get_dict_default(requestId, Input, "greedy-request", RequestId),
    get_dict_default(heuristic, Input, "SPT", HeuristicType),
    get_dict_default(vessels, Input, [], Vessels),
    get_dict_default(resources, Input, [], Resources),
    get_dict_default(docks, Input, [], Docks), % <--- Ler Docas
    get_dict_default(staff, Input, [], Staff),
    get_dict_default(storage_areas, Input, [], StorageAreas),
    
    ( validate_inputs(Vessels, Resources, Staff, StorageAreas, Error) ->
        create_error_output(RequestId, Error, Output)
    ;
        iarti_core:clear_vessels,
        iarti_core:load_vessels_from_json(Vessels),
        
        % 1. Obter Lista de Navios Base
        findall(V, iarti_core:vessel(V,_,_,_,_,_), AllVessels),

        % 2. ORDENAR (Esta é a única responsabilidade do Greedy)
        ( HeuristicType = "SPT" ->
            sort_by_processing_time(AllVessels, SortedVessels),
            AlgoName = "IARTI_Greedy_SPT"
        ; HeuristicType = "MST" ->
            sort_by_slack_time(AllVessels, SortedVessels),
            AlgoName = "IARTI_Greedy_MST"
        ; HeuristicType = "EDT" ->
            sort_by_departure_time(AllVessels, SortedVessels),
            AlgoName = "IARTI_Greedy_EDT"
        ;
            sort_by_processing_time(AllVessels, SortedVessels),
            AlgoName = "IARTI_Greedy_SPT"
        ),
        
        % 3. CALCULAR TEMPOS (Usando o motor Paralelo/Docas)
        % Passamos Resources vazio [] pois a sequence_multi usa Docks para capacidade
        sequence_temporization_multi(SortedVessels, Docks, [], ScheduledTriplets),
        sum_delays_multi(ScheduledTriplets, TotalDelay),
        
        % 4. GERAR JSON (Com suporte a multi-crane e docas)
        convert_multi_to_json(ScheduledTriplets, Docks, Staff, Schedule),
        
        get_time(EndTime),
        ComputationTime is EndTime - StartTime,
        length(Vessels, NumVessels),
        length(Resources, NumResources),
        length(Staff, NumStaff),
        
        Output = _{
            requestId: RequestId,
            success: true,
            schedule: Schedule,
            totalDelay: TotalDelay,
            algorithm: AlgoName,
            heuristicType: HeuristicType,
            performanceMetrics: _{
                computationTimeSeconds: ComputationTime,
                problemSize: _{ vessels: NumVessels, resources: NumResources, staff: NumStaff },
                complexity: "O(n log n) + DockSimulation"
            }
        }
    ).

% ============================================
% HEURISTICS (Sorting Only)
% ============================================

% SPT: Shortest Processing Time
sort_by_processing_time(Vessels, Sorted) :-
    map_list_to_pairs(get_processing_time, Vessels, Pairs),
    keysort(Pairs, SortedPairs),
    pairs_values(SortedPairs, Sorted).

get_processing_time(V, Time) :-
    iarti_core:vessel(V, _, _, TUnload, TLoad, _),
    Time is TUnload + TLoad.

% MST: Minimum Slack Time (Slack = ETD - ETA - Duration)
sort_by_slack_time(Vessels, Sorted) :-
    map_list_to_pairs(get_slack_time, Vessels, Pairs),
    keysort(Pairs, SortedPairs),
    pairs_values(SortedPairs, Sorted).

get_slack_time(V, Slack) :-
    iarti_core:vessel(V, ETA, ETD, TUnload, TLoad, _),
    Slack is ETD - ETA - (TUnload + TLoad).

% EDT: Earliest Departure Time
sort_by_departure_time(Vessels, Sorted) :-
    map_list_to_pairs(get_departure_time, Vessels, Pairs),
    keysort(Pairs, SortedPairs),
    pairs_values(SortedPairs, Sorted).

get_departure_time(V, ETD) :-
    iarti_core:vessel(V, _, ETD, _, _, _).