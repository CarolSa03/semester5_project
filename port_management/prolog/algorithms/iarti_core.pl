:- module(iarti_core, [
    sequence_temporization/2,
    sum_delays/2,
    load_vessels_from_json/1,
    clear_vessels/0,
    vessel_dock/2
]).

:- use_module(library(lists)).
:- dynamic vessel/6.
:- dynamic vessel_dock/2.

% ============================================
% LOAD VESSELS FROM JSON
% ============================================
load_vessels_from_json(JsonVessels) :-
    retractall(vessel(_,_,_,_,_,_)),
    retractall(vessel_dock(_,_)), 
    maplist(assert_vessel_from_json, JsonVessels).

assert_vessel_from_json(JsonVessel) :-
    get_dict(imo, JsonVessel, Imo),
    get_dict_default(eta, JsonVessel, 0, ETA),
    get_dict_default(etd, JsonVessel, 1440, ETD),
    get_dict_default(cargo, JsonVessel, 100, Cargo),
    
    % --- CORREÇÃO ROBUSTA: Só aceita se for String ("D1", "D2"...) ---
    ( get_dict(preAssignedDock, JsonVessel, DockId), 
      string(DockId), 
      DockId \= "" ->
        assertz(vessel_dock(Imo, DockId))
    ;
        true 
    ),

    TUnload is round((Cargo / 25) * 60),
    TLoad is round((Cargo / 25) * 60),
    
    assertz(vessel(Imo, ETA, ETD, TUnload, TLoad, Cargo)).

get_dict_default(Key, Dict, Default, Value) :-
    ( get_dict(Key, Dict, Val) -> Value = Val ; Value = Default ).

clear_vessels :-
    retractall(vessel(_,_,_,_,_,_)),
    retractall(vessel_dock(_,_)).

% (Mantém o resto das funções sequence_temporization e sum_delays originais em baixo se quiseres, 
% mas para o Genético elas não são usadas, é usado o multi_crane)
sequence_temporization(LV, SeqTriplets) :- sequence_temporization1(0, LV, SeqTriplets).
sequence_temporization1(EndPrevSeq, [V|LV], [(V, TInUnload, TEndLoad)|SeqTriplets]) :-
    vessel(V, TIn, _, TUnload, TLoad, _),
    ( TIn > EndPrevSeq -> TInUnload = TIn ; TInUnload is EndPrevSeq + 1 ),
    TEndLoad is TInUnload + TUnload + TLoad - 1,
    sequence_temporization1(TEndLoad, LV, SeqTriplets).
sequence_temporization1(_, [], []).

sum_delays([], 0).
sum_delays([(V, _, TEndLoad)|LV], S) :-
    vessel(V, _, TDep, _, _, _),
    TPossibleDep is TEndLoad + 1,
    ( TPossibleDep > TDep -> SV is TPossibleDep - TDep ; SV = 0 ),
    sum_delays(LV, SLV),
    S is SV + SLV.