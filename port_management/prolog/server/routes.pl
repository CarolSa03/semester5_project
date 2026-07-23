:- module(routes, []).

:- use_module(library(http/http_dispatch)).
:- use_module(server_callbacks).

% ============================================
% HEALTH CHECK
% ============================================
:- http_handler('/health', server_callbacks:health_check, []).

% ============================================
% US 4.3.2: SMART SCHEDULING (DISPATCHER)
% ============================================
% Este é o endpoint principal. O sistema escolhe automaticamente 
% se usa Ótimo, Genético ou Heurística com base no tamanho do problema.
:- http_handler('/schedule', server_callbacks:handle_schedule, []).

% ============================================
% LEGACY / FORCED STRATEGIES
% ============================================
% Estes endpoints forçam uma estratégia específica, útil para testes
% ou para cumprir requisitos específicos de demonstração.
:- http_handler('/schedule/greedy', server_callbacks:handle_schedule_greedy, []).
:- http_handler('/schedule/multi-crane', server_callbacks:handle_schedule_multi_crane, []).
:- http_handler('/schedule/genetic', server_callbacks:handle_schedule_genetic, []). % Novo para testar a US 4.3.1 isoladamente

% ============================================
% US 4.3.3: DOCK REBALANCING
% ============================================
% Novo endpoint para distribuir navios pelas docas antes do agendamento
:- http_handler('/rebalance', server_callbacks:handle_rebalance, []).