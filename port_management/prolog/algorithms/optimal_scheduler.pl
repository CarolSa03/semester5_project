:- module(optimal_scheduler, [
    schedule/2,
    create_error_output/3,
    get_dict_default/4,
    validate_inputs/5,
    format_time_minutes/2
]).

:- use_module(library(lists)).
:- use_module(iarti_core).
% Usar motor Multi-Crane para avaliação real nas docas
:- use_module(multi_crane_scheduler, [
    sequence_temporization_multi/4,
    sum_delays_multi/2,
    convert_multi_to_json/4
]).

:- dynamic shortest_delay/2.

% ============================================
% US 3.4.2: OPTIMAL SCHEDULER (DOCK AWARE)
% ============================================
schedule(Input, Output) :-
    get_time(StartTime),
    get_dict_default(requestId, Input, "optimal-request", RequestId),
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
        length(Vessels, NumVessels),
        length(Resources, NumResources),
        length(Staff, NumStaff),
        
        % Determinar estratégia baseada no tamanho N (Optimal ou Heurística se N for grande demais para teste)
        ( NumVessels =< 8 ->
            Algorithm = "IARTI_Optimal_BruteForce",
            obtain_seq_shortest_delay(Docks, SeqTriplets, TotalDelay)
        ;
            Algorithm = "IARTI_EDT_Heuristic_Fallback",
            % Fallback simples usando a mesma lógica do Greedy (Ordenado por ETD)
            findall(V, iarti_core:vessel(V,_,_,_,_,_), AllVessels),
            sort_by_departure_time(AllVessels, SortedVessels),
            sequence_temporization_multi(SortedVessels, Docks, [], SeqTriplets),
            sum_delays_multi(SeqTriplets, TotalDelay)
        ),
        
        % Gerar JSON final correto
        convert_multi_to_json(SeqTriplets, Docks, Staff, Schedule),
        
        get_time(EndTime),
        ComputationTime is EndTime - StartTime,
        
        Output = _{
            requestId: RequestId,
            success: true,
            schedule: Schedule,
            totalDelay: TotalDelay,
            warnings: [],
            algorithm: Algorithm,
            performanceMetrics: _{
                computationTimeSeconds: ComputationTime,
                problemSize: _{ vessels: NumVessels, resources: NumResources, staff: NumStaff }
            }
        }
    ).

% ============================================
% BRUTE FORCE (With Parallel Dock Eval)
% ============================================
obtain_seq_shortest_delay(Docks, SeqBetterTriplets, SShortestDelay) :-
    retractall(shortest_delay(_,_)),
    (obtain_seq_shortest_delay1(Docks) ; true),
    retract(shortest_delay(SeqBetterTriplets, SShortestDelay)),
    !.

obtain_seq_shortest_delay1(Docks) :-
    asserta(shortest_delay(_, 99999999)),
    findall(V, iarti_core:vessel(V,_,_,_,_,_), LV),
    LV \= [], !,
    permutation(LV, SeqV),
    
    % AQUI MUDOU: Calcula tempos usando as docas reais
    sequence_temporization_multi(SeqV, Docks, [], SeqTriplets),
    sum_delays_multi(SeqTriplets, S),
    
    compare_shortest_delay(SeqTriplets, S),
    fail.

compare_shortest_delay(SeqTriplets, S) :-
    shortest_delay(_, SLower),
    ( S < SLower -> 
        retract(shortest_delay(_,_)), 
        asserta(shortest_delay(SeqTriplets, S)) 
    ; 
        true 
    ).

% Helper de ordenação para fallback
sort_by_departure_time(Vessels, Sorted) :-
    map_list_to_pairs(get_departure_time, Vessels, Pairs),
    keysort(Pairs, SortedPairs),
    pairs_values(SortedPairs, Sorted).

get_departure_time(V, ETD) :- iarti_core:vessel(V, _, ETD, _, _, _).

% ============================================
% UTILS (Mantidos para compatibilidade)
% ============================================
format_time_minutes(TotalMinutes, TimeStr) :-
    IntMinutes is round(TotalMinutes),
    Hours is IntMinutes // 60,
    Minutes is IntMinutes mod 60,
    format(string(TimeStr), "~|~`0t~d~2+:~|~`0t~d~2+", [Hours, Minutes]).

validate_inputs(Vessels, Resources, Staff, StorageAreas, Error) :-
    ( Vessels = [] -> Error = "No vessels" ; Resources = [] -> Error = "No resources" ; Staff = [] -> Error = "No staff" ; StorageAreas = [] -> Error = "No storage" ; fail ).

create_error_output(RequestId, ErrorMsg, Output) :-
    Output = _{ requestId: RequestId, success: false, error: ErrorMsg, schedule: [], totalDelay: 0, warnings: [] }.

get_dict_default(Key, Dict, Default, Value) :-
    ( get_dict(Key, Dict, Val) -> Value = Val ; Value = Default ).