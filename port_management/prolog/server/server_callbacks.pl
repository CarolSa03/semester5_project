:- module(server_callbacks, [
    handle_schedule/1,
    handle_schedule_greedy/1,
    handle_schedule_multi_crane/1,
    handle_schedule_genetic/1,
    handle_rebalance/1,
    health_check/1
]).

:- use_module(library(http/http_json)).

% IMPORTANTE: No Sprint C, só falamos com os "Gestores", não com os algoritmos diretamente.
:- use_module('../algorithms/scheduler_dispatcher').
:- use_module('../algorithms/dock_rebalancing').

% ============================================
% HEALTH CHECK (Updated for Sprint C)
% ============================================
health_check(_Request) :-
    reply_json_dict(_{
        status: "ok",
        module: "scheduler-backend-iarti",
        sprint: "C",
        version: "3.0-IARTI-SprintC",
        supportedAlgorithms: [
            "Dispatcher (Auto-Selection)",
            "Genetic Algorithm (OX1)",
            "Load Balancing Heuristic",
            "Legacy (Optimal/Greedy)"
        ]
    }).

% ============================================
% US 4.3.2: HANDLE SCHEDULE (AUTO)
% ============================================
handle_schedule(Request) :-
    read_json_input(Request, Input),
    % O Dispatcher decide qual algoritmo usar
    process_request(scheduler_dispatcher:dispatch_scheduling(Input, Output), Output).

% ============================================
% FORCE GREEDY (US 3.4.4 Legacy)
% ============================================
handle_schedule_greedy(Request) :-
    read_json_input(Request, Input),
    % Injetamos a flag "strategy" para forçar o dispatcher
    InputWithStrategy = Input.put(strategy, "greedy"),
    process_request(scheduler_dispatcher:dispatch_scheduling(InputWithStrategy, Output), Output).

% ============================================
% FORCE MULTI-CRANE (US 3.4.5 Legacy)
% ============================================
handle_schedule_multi_crane(Request) :-
    read_json_input(Request, Input),
    InputWithStrategy = Input.put(strategy, "multi_crane"),
    process_request(scheduler_dispatcher:dispatch_scheduling(InputWithStrategy, Output), Output).

% ============================================
% FORCE GENETIC (US 4.3.1 New)
% ============================================
handle_schedule_genetic(Request) :-
    read_json_input(Request, Input),
    InputWithStrategy = Input.put(strategy, "genetic"),
    process_request(scheduler_dispatcher:dispatch_scheduling(InputWithStrategy, Output), Output).

% ============================================
% US 4.3.3: HANDLE REBALANCE
% ============================================
handle_rebalance(Request) :-
    read_json_input(Request, Input),
    process_request(dock_rebalancing:rebalance_docks(Input, Output), Output).

% ============================================
% HELPER PREDICATES
% ============================================

% Lê o JSON do corpo do pedido HTTP
read_json_input(Request, Input) :-
    catch(
        http_read_json_dict(Request, Input),
        _,
        (reply_json_dict(_{error: "Malformed JSON Input"}, [status(400)]), !, fail)
    ).

% Executa o objetivo (Goal) e devolve o Output, ou apanha erro 500
process_request(Goal, Output) :-
    catch(
        Goal,
        Error,
        (
            format(atom(Msg), '~w', [Error]),
            reply_json_dict(_{error: "Internal Processing Error", detail: Msg}, [status(500)]),
            !,
            fail
        )
    ),
    reply_json_dict(Output).