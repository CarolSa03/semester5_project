:- module(scheduler_dispatcher, [dispatch_scheduling/2]).

:- use_module(optimal_scheduler).
:- use_module(greedy_scheduler).
:- use_module(genetic_scheduler).
:- use_module(multi_crane_scheduler).

% ============================================
% US 4.3.2: AUTOMATIC ALGORITHM SELECTION
% ============================================

dispatch_scheduling(Input, Output) :-
    % 1. Extrair dados essenciais para decisão
    get_dict_default(vessels, Input, [], Vessels),
    length(Vessels, NumVessels),
    
    % 2. Verificar se existe override manual ("strategy")
    ( get_dict(strategy, Input, Strategy), Strategy \= "auto" -> 
        run_strategy(Strategy, Input, Output)
    ; 
        % 3. Seleção Automática
        auto_select_strategy(NumVessels, AutoStrategy),
        % Injetar a estratégia escolhida no output para feedback ao utilizador
        run_strategy(AutoStrategy, Input, TempOutput),
        Output = TempOutput.put(autoSelectedStrategy, AutoStrategy)
    ).

% --- Regras de Decisão (Thresholds) ---
auto_select_strategy(N, "optimal") :- N =< 8, !.
auto_select_strategy(N, "genetic") :- N > 8, N =< 50, !.
auto_select_strategy(_, "greedy").

% --- Dispatcher ---

% 1. Optimal (Brute Force)
run_strategy("optimal", Input, Output) :-
    optimal_scheduler:schedule(Input, Output).

% 2. Genetic Algorithm (US 4.3.1)
run_strategy("genetic", Input, Output) :-
    genetic_scheduler:schedule_genetic(Input, Output).

% 3. Greedy (Heurísticas do Sprint B)
run_strategy("greedy", Input, Output) :-
    % Por defeito usa MST (Minimum Slack Time) se não for especificado
    ( get_dict(heuristic, Input, _) -> 
        greedy_scheduler:schedule_greedy(Input, Output)
    ;
        InputWithHeuristic = Input.put(heuristic, "MST"),
        greedy_scheduler:schedule_greedy(InputWithHeuristic, Output)
    ).

% 4. Multi-Crane Specific (US 3.4.5)
run_strategy("multi_crane", Input, Output) :-
    multi_crane_scheduler:schedule_multi_crane(Input, Output).