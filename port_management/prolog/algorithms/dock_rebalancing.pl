:- module(dock_rebalancing, [rebalance_docks/2]).

:- use_module(library(lists)).
:- use_module(library(pairs)).
:- use_module(iarti_core).
:- use_module(optimal_scheduler, [get_dict_default/4, create_error_output/3]).

% ============================================
% US 4.3.3: DOCK REBALANCING
% ============================================

rebalance_docks(Input, Output) :-
    get_dict_default(requestId, Input, "rebalance-request", ReqId),
    get_dict_default(vessels, Input, [], VesselsJson),
    get_dict_default(docks, Input, [], DocksJson),
    
    ( VesselsJson = [] -> 
        create_error_output(ReqId, "No vessels to rebalance", Output)
    ; DocksJson = [] ->
        create_error_output(ReqId, "No docks available", Output)
    ;
        % 1. Inicializar Estado das Docas (ID, Carga=0, CapacidadeGruas, ListaNavios=[])
        init_docks(DocksJson, DocksState),
        
        % 2. Processar Navios (Calcular Carga e Ordenar por ETA)
        process_vessels(VesselsJson, OrderedVessels),
        
        % 3. Distribuir (Algoritmo Guloso de Balanceamento)
        distribute_vessels(OrderedVessels, DocksState, FinalAllocation),
        
        % 4. Gerar Output
        format_allocation_output(FinalAllocation, AllocationList),
        
        Output = _{
            requestId: ReqId,
            success: true,
            algorithm: "IARTI_Load_Balancing_Heuristic",
            allocations: AllocationList
        }
    ).

% --- 1. Inicializar Docas ---
% Formato Interno: dock(Id, LoadAccumulator, NumCranes, AssignedVesselsList)
init_docks([], []).
init_docks([D|Rest], [dock(Id, 0, Cranes, [])|RestDocks]) :-
    get_dict(id, D, Id),
    get_dict_default(cranes, D, 1, Cranes), % Default 1 grua se não especificado
    init_docks(Rest, RestDocks).

% --- 2. Processar Navios ---
% Calcula carga base e ordena por ETA.
% Formato Interno: vessel(Id, Eta, BaseDuration)
process_vessels(VesselsJson, OrderedVessels) :-
    maplist(extract_vessel_info, VesselsJson, VesselStructs),
    % Ordenar por ETA (índice 2 da estrutura vessel/3) garante FCFS na distribuição
    sort(2, @=<, VesselStructs, OrderedVessels).

extract_vessel_info(VJson, vessel(Id, Eta, Duration)) :-
    get_dict(imo, VJson, Id),
    get_dict_default(eta, VJson, 0, Eta),
    get_dict(cargo, VJson, Cargo),
    % Estimativa: 2.4 min/contentor * 2 (load+unload)
    Duration is round(Cargo * 2.4 * 2).

% --- 3. Distribuir Navios ---
distribute_vessels([], Docks, Docks).
distribute_vessels([V|RestV], Docks, FinalDocks) :-
    % Encontrar doca que ficará com menor carga TOTAL se receber este navio
    find_best_dock(V, Docks, BestDockId),
    
    % Atribuir o navio a essa doca e atualizar a carga acumulada
    update_dock_state(V, Docks, BestDockId, UpdatedDocks),
    
    distribute_vessels(RestV, UpdatedDocks, FinalDocks).

find_best_dock(vessel(_, _, Duration), Docks, BestDockId) :-
    % Cria pares Key-Value onde Key = Carga Futura Prevista
    map_list_to_pairs(calculate_potential_load(Duration), Docks, Pairs),
    % Ordena por menor carga (Greedy strategy)
    keysort(Pairs, [_-BestDock|_]), 
    BestDock = dock(BestDockId, _, _, _).

calculate_potential_load(Duration, dock(Id, CurrentLoad, Cranes, _), Key-dock(Id, CurrentLoad, Cranes, _)) :-
    % A carga adicionada ("peso") depende das gruas disponíveis na doca
    EffectiveDuration is round(Duration / Cranes),
    Key is CurrentLoad + EffectiveDuration.

update_dock_state(vessel(VId, _, Duration), [dock(DId, Load, Cranes, Assigned)|Rest], TargetId, [NewDock|Rest]) :-
    DId == TargetId, !,
    EffectiveDuration is round(Duration / Cranes),
    NewLoad is Load + EffectiveDuration,
    % Adiciona navio à cabeça da lista (será invertido no output)
    NewDock = dock(DId, NewLoad, Cranes, [VId|Assigned]).

update_dock_state(V, [D|Rest], TargetId, [D|RestUpdated]) :-
    update_dock_state(V, Rest, TargetId, RestUpdated).

% --- 4. Formatar Output ---
format_allocation_output([], []).
format_allocation_output([dock(Id, TotalLoad, _, Vessels)|Rest], [Entry|RestOut]) :-
    reverse(Vessels, OrderedVessels), % Recuperar ordem cronológica
    Entry = _{
        dockId: Id,
        totalLoadMinutes: TotalLoad,
        assignedVessels: OrderedVessels
    },
    format_allocation_output(Rest, RestOut).