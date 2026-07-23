:- module(server, [start/1]).

:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_json)).

:- use_module(routes).

% Arranca o servidor HTTP
start(Port) :-
    format("Starting Prolog REST server on port ~w...~n", [Port]),
    http_server(http_dispatch, [port(Port)]).
