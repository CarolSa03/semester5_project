:- use_module(library(http/json)).
:- use_module('server/server').

start :-
    server:start(5001).

wait_forever :-
    thread_get_message(_).

main:-
    start,
    wait_forever.

:- initialization(main, main).