:- module(multi_crane_scheduler, [
    schedule_multi_crane/2,
    sequence_temporization_multi/4,
    sum_delays_multi/2,
    convert_multi_to_json/4
]).

:- use_module(library(lists)).
:- use_module(library(between)).
:- use_module(iarti_core).
:- use_module(optimal_scheduler, [schedule/2, format_time_minutes/2, get_dict_default/4, create_error_output/3, validate_inputs/5]).

% ============================================
% WRAPPER
% ============================================
schedule_multi_crane(Input, Output) :-
    get_time(StartT), get_dict_default(requestId, Input, "req", ReqId),
    ( validate_inputs([],[],[],[],_) -> true ; true ),
    
    optimal_scheduler:schedule(Input, SingleOutput),
    get_dict(totalDelay, SingleOutput, SingleDelay),

    ( SingleDelay = 0 -> Output = SingleOutput ;
        get_dict_default(vessels, Input, [], Vessels),
        get_dict_default(resources, Input, [], Resources),
        get_dict_default(docks, Input, [], Docks), 
        get_dict_default(staff, Input, [], Staff),

        iarti_core:clear_vessels,
        iarti_core:load_vessels_from_json(Vessels), 
        findall(V, iarti_core:vessel(V,_,_,_,_,_), VesselList),
        
        sequence_temporization_multi(VesselList, Docks, Resources, ScheduledTriplets),
        sum_delays_multi(ScheduledTriplets, Delay),
        convert_multi_to_json(ScheduledTriplets, Docks, Staff, Json),
        
        get_time(EndT), CT is EndT - StartT,
        Output = _{ requestId: ReqId, success: true, algorithm: "IARTI_Parallel_Dock_Scheduler", totalDelay: Delay, schedule: Json, metrics: _{ time: CT } }
    ).

% ============================================
% CORE: PARALELISMO POR DOCA
% ============================================

sequence_temporization_multi(Vessels, Docks, Resources, Triplets) :-
    % 1. Criar linhas de tempo independentes para cada doca
    % Ex: [dt("D1", 0), dt("D2", 0), dt("D3", 0)]
    init_dock_timelines(Docks, DockTimelines),
    
    % 2. Processar a sequência
    sequence_parallel_recursive(Vessels, DockTimelines, Docks, Resources, Triplets).

init_dock_timelines([], []).
init_dock_timelines([D|Rest], [dt(Id, 0)|RestTL]) :-
    get_dict(id, D, Id),
    init_dock_timelines(Rest, RestTL).

sequence_parallel_recursive([], _, _, _, []).

sequence_parallel_recursive([V|Rest], CurrentTimelines, Docks, Resources, [Trip|RestTriplets]) :-
    iarti_core:vessel(V, Arrival, ETD, TUnload, TLoad, _),
    
    % A. Descobrir a Doca (com fallback se falhar)
    ( iarti_core:vessel_dock(V, AssignedDockId) -> TargetDockId = AssignedDockId
    ; Docks = [FirstDock|_], get_dict(id, FirstDock, TargetDockId)
    ),

    % B. Obter o tempo livre DESTA doca específica
    ( member(dt(TargetDockId, FreeTime), CurrentTimelines) -> DockFreeTime = FreeTime
    ; DockFreeTime = 0 
    ),

    % C. Inicio = Max(Chegada, TempoLivreDestaDoca)
    % Ignora completamente se as outras docas estão ocupadas!
    Start is max(Arrival, DockFreeTime),
    BaseDuration is TUnload + TLoad,

    % D. Calcular Gruas e Duração
    get_dock_capacity(Docks, TargetDockId, MaxCranes),
    find_optimal_crane_count(Start, BaseDuration, ETD, MaxCranes, UsedCranes, EffectiveDuration),
    
    End is Start + EffectiveDuration,
    
    % E. Atualizar APENAS a linha de tempo desta doca
    update_timeline(CurrentTimelines, TargetDockId, End, NewTimelines),

    Trip = (V, Start, End, UsedCranes, TargetDockId),
    
    sequence_parallel_recursive(Rest, NewTimelines, Docks, Resources, RestTriplets).

% Helpers
update_timeline([dt(Id, _)|Rest], TargetId, NewTime, [dt(Id, NewTime)|Rest]) :-
    Id == TargetId, !.
update_timeline([Head|Rest], TargetId, NewTime, [Head|NewRest]) :-
    update_timeline(Rest, TargetId, NewTime, NewRest).

get_dock_capacity([], _, 1).
get_dock_capacity([D|Rest], TargetId, Cap) :-
    get_dict(id, D, Id),
    ( Id == TargetId -> get_dict(cranes, D, Cap) ; get_dock_capacity(Rest, TargetId, Cap) ).

find_optimal_crane_count(Start, Base, ETD, Max, Count, Dur) :-
    between(1, Max, N),
    (N > 1 -> Eff is N * 0.9, D is ceiling(Base / Eff) ; D = Base),
    E is Start + D,
    (E =< ETD ; N == Max), !, Count = N, Dur = D.

sum_delays_multi([], 0).
sum_delays_multi([(V, _, End, _, _)|Rest], Total) :-
    iarti_core:vessel(V, _, Deadline, _, _, _), D is max(0, End - Deadline),
    sum_delays_multi(Rest, R), Total is D + R.

% ============================================
% JSON CONVERSION
% ============================================
convert_multi_to_json([], _, _, []).
convert_multi_to_json([(V, Start, End, UsedCranes, DockId)|Rest], Docks, Staff, [Entry|RestJson]) :-
    Duration is End - Start,
    optimal_scheduler:format_time_minutes(Start, SS),
    optimal_scheduler:format_time_minutes(End, ES),
    iarti_core:vessel(V, _, ETD, _, _, _), Delay is max(0, End - ETD),

    generate_crane_codes_for_dock(DockId, UsedCranes, AssignedCraneCodes),
    findall(Sid, (member(S, Staff), get_dict(id, S, Sid)), AllStaffIds),
    take_n(UsedCranes, AllStaffIds, AssignedStaffIds),

    Entry = _{
        vesselId: V, startTime: Start, startLabel: SS, endTime: End, endLabel: ES,
        durationMinutes: Duration, delay: Delay,
        assignedCrane: AssignedCraneCodes, assignedStaff: AssignedStaffIds,
        craneCount: UsedCranes, dockId: DockId
    },
    convert_multi_to_json(Rest, Docks, Staff, RestJson).

generate_crane_codes_for_dock("D1", 1, ["C1"]).
generate_crane_codes_for_dock("D1", 2, ["C1", "C2"]).
generate_crane_codes_for_dock("D2", _, ["C3"]).
generate_crane_codes_for_dock("D3", _, ["C4"]).
generate_crane_codes_for_dock(_, N, Codes) :- findall(C, (between(1,N,I), format(atom(C),'G~d',[I])), Codes).

take_n(0,_,[]):-!. take_n(_,[],[]):-!. take_n(N,[H|T],[H|R]):- N>0, N1 is N-1, take_n(N1,T,R).