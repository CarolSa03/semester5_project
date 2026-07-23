:- module(genetic_scheduler, [schedule_genetic/2]).

:- use_module(library(lists)).
:- use_module(library(random)). 
:- use_module(library(pairs)).
:- use_module(iarti_core).
:- use_module(optimal_scheduler, [
    convert_to_json_schedule/5,
    validate_inputs/5,
    get_dict_default/4,
    create_error_output/3
]).
% Importar a versão correta (com docas) do multi_crane
:- use_module(multi_crane_scheduler, [
    sequence_temporization_multi/4,
    sum_delays_multi/2,
    convert_multi_to_json/4
]).

:- dynamic ga_param/2.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% ENTRY POINT
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

schedule_genetic(Input, Output) :-
    get_time(Start),

    get_dict_default(requestId, Input, "ga-request", ReqId),
    get_dict_default(vessels, Input, [], Vessels),
    get_dict_default(resources, Input, [], Resources),
    get_dict_default(docks, Input, [], Docks), % 1. Ler Docas
    get_dict_default(staff, Input, [], Staff),
    get_dict_default(storage_areas, Input, [], StorageAreas),
    
    length(Resources, NumRes),
    ( NumRes > 1 -> DefaultMode = "multi" ; DefaultMode = "single" ),
    get_dict_default(mode, Input, DefaultMode, Mode),

    get_dict_default(population, Input, 40, PopSize),
    get_dict_default(generations, Input, 50, NumGen),
    get_dict_default(mutationRate, Input, 0.15, MutRate),
    get_dict_default(crossoverRate, Input, 0.80, CrossRate),

    ( validate_inputs(Vessels, Resources, Staff, StorageAreas, Error) ->
        create_error_output(ReqId, Error, Output)
    ;
        iarti_core:clear_vessels,
        iarti_core:load_vessels_from_json(Vessels), % Carrega vessel_dock/2

        get_max_dock_capacity(Docks, MaxDockCap),
        
        retractall(ga_param(_,_)),
        asserta(ga_param(pop, PopSize)),
        asserta(ga_param(gens, NumGen)),
        asserta(ga_param(mut, MutRate)),
        asserta(ga_param(cross, CrossRate)),
        asserta(ga_param(mode, Mode)),
        asserta(ga_param(maxCranes, MaxDockCap)),
        
        % 2. Guardar Docas na memória para o evaluate_perm usar
        asserta(ga_param(docks, Docks)), 

        % Run GA
        run_ga(BestPerm, BestDelay),

        % Decode final
        ( Mode = "single" ->
            iarti_core:sequence_temporization(BestPerm, Triplets),
            convert_to_json_schedule(Triplets, Resources, Staff, StorageAreas, Schedule)
        ;
            ga_param(maxCranes, _),
            % Passa Docks e Resources para o conversor final
            sequence_temporization_multi(BestPerm, Docks, Resources, Triplets), 
            convert_multi_to_json(Triplets, Docks, Staff, Schedule)
        ),

        get_time(End),
        Time is End - Start,
        length(Vessels, NumVessels),
        length(Resources, NumResources),
        length(Staff, NumStaff),

        Output = _{
            requestId: ReqId,
            success: true,
            schedule: Schedule,
            totalDelay: BestDelay,
            algorithm: "IARTI_Genetic_Algorithm",
            mode: Mode,
            performanceMetrics: _{
                computationTimeSeconds: Time,
                problemSize: _{ vessels: NumVessels, resources: NumResources, staff: NumStaff },
                parameters: _{ population: PopSize, generations: NumGen }
            }
        }
    ).

get_max_dock_capacity([], 1).
get_max_dock_capacity(Docks, Max) :-
    findall(C, (member(D, Docks), get_dict(cranes, D, C)), Caps),
    ( Caps = [] -> Max = 1 ; max_list(Caps, Max) ).


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% GA - CICLO PRINCIPAL
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

run_ga(BestPerm, BestValue) :-
    ga_initial_population(Pop),
    evaluate_population(Pop, PopVal),
    order_population(PopVal, PopOrd),
    ga_param(gens, NG),
    evolve(0, NG, PopOrd, BestPerm, BestValue).

evolve(G, G, [BestInd*BestValue|_], BestInd, BestValue) :- !.

evolve(N, Max, PopVal, BestPerm, BestValue) :-
    PopVal = [Elite1, Elite2 | Rest],
    random_permutation(Rest, ScrambledRest),
    crossover(ScrambledRest, ChildrenC),
    mutation(ChildrenC, ChildrenM),
    evaluate_population(ChildrenM, ChildrenEvaluated),
    append([Elite1, Elite2], ChildrenEvaluated, NewPop),
    order_population(NewPop, NewPopSorted),
    ga_param(pop, PopSize),
    take_n(PopSize, NewPopSorted, NextGen),
    N2 is N + 1,
    evolve(N2, Max, NextGen, BestPerm, BestValue).

take_n(0, _, []) :- !.
take_n(_, [], []) :- !.
take_n(N, [H|T], [H|R]) :- N > 0, N1 is N - 1, take_n(N1, T, R).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% GA - HELPER FUNCTIONS
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

ga_initial_population(Pop) :-
    ga_param(pop, PopSize),
    findall(V, iarti_core:vessel(V,_,_,_,_,_), Vessels),
    generate_population(PopSize, Vessels, Pop).

generate_population(0, _, []) :- !.
generate_population(N, Base, [P|R]) :-
    N > 0, random_permutation(Base, P), N1 is N - 1, generate_population(N1, Base, R).

evaluate_population([], []).
evaluate_population([Ind|R], [Ind*V | R2]) :-
    evaluate_perm(Ind, V), evaluate_population(R, R2).

evaluate_perm(Perm, Value) :-
    ga_param(mode, Mode),
    ( Mode = "single" ->
        iarti_core:sequence_temporization(Perm, Triplets),
        iarti_core:sum_delays(Triplets, Value)
    ;
        % 3. Ler as Docas da memória
        ga_param(docks, Docks), 
        % Usar a versão /4 (passando [] nos resources para poupar tempo no loop)
        sequence_temporization_multi(Perm, Docks, [], Triplets),
        sum_delays_multi(Triplets, Value)
    ).

order_population(PopList, SortedPop) :-
    map_list_to_pairs(PopList, Pairs),
    keysort(Pairs, SortedPairs),
    map_pairs_to_list(SortedPairs, SortedPop).

map_list_to_pairs([], []).
map_list_to_pairs([Ind*Val | T], [Val-Ind | T2]) :- map_list_to_pairs(T, T2).

map_pairs_to_list([], []).
map_pairs_to_list([Val-Ind | T], [Ind*Val | T2]) :- map_pairs_to_list(T, T2).

crossover([], []).
crossover([Ind*_], [Ind]).
crossover([Ind1*_, Ind2*_ | Rest], [NInd1, NInd2 | Rest2]) :-
    ga_param(cross, Pcross),
    random(0.0, 1.0, Pc),
    ( Pc =< Pcross ->
        ox1_crossover(Ind1, Ind2, NInd1),
        ox1_crossover(Ind2, Ind1, NInd2)
    ;
        NInd1 = Ind1, NInd2 = Ind2
    ),
    crossover(Rest, Rest2).

ox1_crossover(P1, P2, Child) :-
    length(P1, Len),
    random_between(1, Len, CP1), random_between(1, Len, CP2),
    Min is min(CP1, CP2), Max is max(CP1, CP2),
    sublist_indices(P1, Min, Max, Sub1),
    subtract(P2, Sub1, Remaining),
    insert_sublist(Remaining, Sub1, Min, Child).

sublist_indices(List, Start, End, Sub) :-
    findall(E, (nth1(I, List, E), I >= Start, I =< End), Sub).

insert_sublist(List, Sub, Index, Result) :-
    I1 is Index - 1,
    length(Prefix, I1), append(Prefix, Suffix, List),
    append(Prefix, Sub, Temp), append(Temp, Suffix, Result).

mutation([], []).
mutation([Ind|R], [New|R2]) :-
    ga_param(mut, Pm),
    random(0.0,1.0,Val),
    ( Val =< Pm -> swap_mutation(Ind, New) ; New = Ind ),
    mutation(R, R2).

swap_mutation(List, New) :-
    length(List, L),
    random_between(1, L, X), random_between(1, L, Y),
    X \= Y, swap_elements(List, X, Y, New), !.
swap_mutation(List, List).

swap_elements(List, I, J, NewList) :-
    nth1(I, List, ElemI), nth1(J, List, ElemJ),
    replace_at(List, I, ElemJ, Temp),
    replace_at(Temp, J, ElemI, NewList).

replace_at([_|T], 1, X, [X|T]) :- !.
replace_at([H|T], I, X, [H|R]) :- I > 1, I1 is I - 1, replace_at(T, I1, X, R).