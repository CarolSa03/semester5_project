:- module(run_comparison, [run_all_tests/0]).

:- use_module(datasets).
:- use_module('../algorithms/iarti_core').
:- use_module('../algorithms/optimal_scheduler').
:- use_module('../algorithms/greedy_scheduler').

% ============================================
% RUN ALL COMPARISON TESTS
% ============================================
run_all_tests :-
    write('==============================================='), nl,
    write('  HEURISTICS COMPARISON STUDY - US 3.4.4'), nl,
    write('==============================================='), nl, nl,
    
    findall(DS, datasets:dataset_info(DS, _, _), AllDatasets),
    maplist(test_dataset, AllDatasets),
    
    nl,
    write('==============================================='), nl,
    write('  ALL TESTS COMPLETED'), nl,
    write('==============================================='), nl.

% ============================================
% TEST SINGLE DATASET
% ============================================
test_dataset(DatasetID) :-
    datasets:dataset_info(DatasetID, NumVessels, Description),
    
    format('~n--- DATASET: ~w (n=~w) ---~n', [DatasetID, NumVessels]),
    format('Description: ~w~n~n', [Description]),
    
    % Load dataset
    datasets:load_dataset(DatasetID),
    
    % Test Optimal (only for n <= 8)
    ( NumVessels =< 8 ->
        test_optimal(OptimalDelay, OptimalTime)
    ;
        OptimalDelay = null,
        OptimalTime = null
    ),
    
    % Test EDT
    datasets:load_dataset(DatasetID),
    test_edt(EDTDelay, EDTTime),
    
    % Test SPT
    datasets:load_dataset(DatasetID),
    test_spt(SPTDelay, SPTTime),
    
    % Test MST (if implemented)
    datasets:load_dataset(DatasetID),
    ( catch(test_mst(MSTDelay, MSTTime), _, (MSTDelay = null, MSTTime = null)) ->
        true
    ;
        MSTDelay = null, MSTTime = null
    ),
    
    % Calculate quality percentages
    ( OptimalDelay \= null ->
        % Avoid division by zero - if EDTDelay = 0, quality is 100%
        ( EDTDelay > 0 ->
            EDTQuality is round((OptimalDelay / EDTDelay) * 100)
        ;
            EDTQuality = 100  % Perfect schedule, no delay
        ),
        
        ( SPTDelay > 0 ->
            SPTQuality is round((OptimalDelay / SPTDelay) * 100)
        ;
            SPTQuality = 100
        ),
        
        ( MSTDelay \= null ->
            ( MSTDelay > 0 ->
                MSTQuality is round((OptimalDelay / MSTDelay) * 100)
            ;
                MSTQuality = 100
            )
        ;
            MSTQuality = null
        )
    ;
        EDTQuality = null,
        SPTQuality = null,
        MSTQuality = null
    ),
    
    % Print results
    format('| Algorithm       | Delay (min) | Time (s) | Quality |~n', []),
    format('|-----------------|-------------|----------|---------|~n', []),
    
    ( OptimalDelay \= null ->
        format('| Optimal         | ~w         | ~3f    | 100%    |~n', [OptimalDelay, OptimalTime])
    ;
        true
    ),
    
    format('| EDT             | ~w         | ~3f    | ~w%     |~n', [EDTDelay, EDTTime, EDTQuality]),
    format('| SPT             | ~w         | ~3f    | ~w%     |~n', [SPTDelay, SPTTime, SPTQuality]),
    
    ( MSTDelay \= null ->
        format('| MST             | ~w         | ~3f    | ~w%     |~n', [MSTDelay, MSTTime, MSTQuality])
    ;
        true
    ),
    
    nl.

% ============================================
% TEST INDIVIDUAL ALGORITHMS
% ============================================
test_optimal(Delay, Time) :-
    get_time(Start),
    optimal_scheduler:obtain_seq_shortest_delay(_, Delay),
    get_time(End),
    Time is End - Start.

test_edt(Delay, Time) :-
    get_time(Start),
    findall((Departure, V), iarti_core:vessel(V, _, Departure, _, _, _), LDV),
    sort(LDV, LDVSorted),
    greedy_scheduler:obtain_vessels(LDVSorted, SeqV),
    iarti_core:sequence_temporization(SeqV, SeqTriplets),
    iarti_core:sum_delays(SeqTriplets, Delay),
    get_time(End),
    Time is End - Start.

test_spt(Delay, Time) :-
    get_time(Start),
    greedy_scheduler:heuristic_shortest_processing_time(_, Delay),
    get_time(End),
    Time is End - Start.

test_mst(Delay, Time) :-
    get_time(Start),
    greedy_scheduler:heuristic_minimum_slack_time(_, Delay),
    get_time(End),
    Time is End - Start.

% ============================================
% HELPER: Run specific dataset
% ============================================
run_test(DatasetID) :-
    test_dataset(DatasetID).