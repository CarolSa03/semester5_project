:- module(datasets, [
    load_dataset/1,
    dataset_info/3
]).

:- use_module('../algorithms/iarti_core').

% ============================================
% DATASET METADATA
% ============================================
% dataset_info(ID, NumVessels, Description)

dataset_info(ds1, 5, "Uniform arrivals - balanced schedule").
dataset_info(ds2, 5, "Clustered arrivals - high contention").
dataset_info(ds3, 7, "Mixed processing times - varied cargo").
dataset_info(ds4, 7, "Tight deadlines - time pressure").
dataset_info(ds5, 8, "Large variance - small and large vessels").
dataset_info(ds6, 8, "Sequential arrivals - no overlap").
dataset_info(ds7, 10, "Random distribution - realistic chaos").
dataset_info(ds8, 10, "Peak hours - morning rush").
dataset_info(ds9, 12, "Night shift - sparse arrivals").
dataset_info(ds10, 12, "Full day - continuous operations").

% ============================================
% DATASET 1: Uniform Arrivals (n=5)
% ============================================
% Characteristic: Evenly spaced arrivals every hour
% Expected: EDT should perform well

load_dataset(ds1) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO1000001', 480, 660, 60, 60, 100)),   % 08:00 arr, 11:00 dep, 2h proc
    assertz(iarti_core:vessel('IMO1000002', 540, 720, 45, 45, 75)),    % 09:00 arr, 12:00 dep, 1.5h proc
    assertz(iarti_core:vessel('IMO1000003', 600, 780, 90, 90, 150)),   % 10:00 arr, 13:00 dep, 3h proc
    assertz(iarti_core:vessel('IMO1000004', 660, 840, 30, 30, 50)),    % 11:00 arr, 14:00 dep, 1h proc
    assertz(iarti_core:vessel('IMO1000005', 720, 900, 75, 75, 125)).   % 12:00 arr, 15:00 dep, 2.5h proc

% ============================================
% DATASET 2: Clustered Arrivals (n=5)
% ============================================
% Characteristic: All vessels arrive within 30 minutes
% Expected: Heavy contention, delays inevitable

load_dataset(ds2) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO2000001', 480, 660, 60, 60, 100)),   % 08:00
    assertz(iarti_core:vessel('IMO2000002', 485, 720, 45, 45, 75)),    % 08:05
    assertz(iarti_core:vessel('IMO2000003', 490, 780, 90, 90, 150)),   % 08:10
    assertz(iarti_core:vessel('IMO2000004', 495, 840, 30, 30, 50)),    % 08:15
    assertz(iarti_core:vessel('IMO2000005', 510, 900, 75, 75, 125)).   % 08:30

% ============================================
% DATASET 3: Mixed Processing Times (n=7)
% ============================================
% Characteristic: Wide range of cargo sizes (20-300 TEU)
% Expected: SPT will prioritize small vessels

load_dataset(ds3) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO3000001', 420, 720, 120, 120, 300)),  % Large: 4h proc
    assertz(iarti_core:vessel('IMO3000002', 480, 660, 24, 24, 20)),     % Small: 0.8h proc
    assertz(iarti_core:vessel('IMO3000003', 540, 840, 80, 80, 200)),    % Medium: 2.7h proc
    assertz(iarti_core:vessel('IMO3000004', 600, 900, 36, 36, 30)),     % Small: 1.2h proc
    assertz(iarti_core:vessel('IMO3000005', 660, 960, 100, 100, 250)),  % Large: 3.3h proc
    assertz(iarti_core:vessel('IMO3000006', 720, 1020, 48, 48, 40)),    % Small: 1.6h proc
    assertz(iarti_core:vessel('IMO3000007', 780, 1080, 60, 60, 150)).   % Medium: 2h proc

% ============================================
% DATASET 4: Tight Deadlines (n=7)
% ============================================
% Characteristic: Very small slack times (30-60 min)
% Expected: MST should excel here

load_dataset(ds4) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO4000001', 480, 600, 50, 50, 125)),   % Slack: 20 min
    assertz(iarti_core:vessel('IMO4000002', 540, 690, 60, 60, 150)),   % Slack: 30 min
    assertz(iarti_core:vessel('IMO4000003', 600, 750, 45, 45, 112)),   % Slack: 45 min
    assertz(iarti_core:vessel('IMO4000004', 660, 810, 40, 40, 100)),   % Slack: 50 min
    assertz(iarti_core:vessel('IMO4000005', 720, 870, 55, 55, 138)),   % Slack: 35 min
    assertz(iarti_core:vessel('IMO4000006', 780, 930, 48, 48, 120)),   % Slack: 42 min
    assertz(iarti_core:vessel('IMO4000007', 840, 990, 52, 52, 130)).   % Slack: 38 min

% ============================================
% DATASET 5: Large Variance (n=8)
% ============================================
% Characteristic: Mix of tiny (10 TEU) and huge (500 TEU) vessels
% Expected: SPT prioritizes tiny, may delay huge vessels

load_dataset(ds5) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO5000001', 480, 900, 200, 200, 500)),  % Huge: 6.7h
    assertz(iarti_core:vessel('IMO5000002', 510, 630, 12, 12, 10)),     % Tiny: 0.4h
    assertz(iarti_core:vessel('IMO5000003', 540, 960, 160, 160, 400)),  % Huge: 5.3h
    assertz(iarti_core:vessel('IMO5000004', 570, 690, 18, 18, 15)),     % Tiny: 0.6h
    assertz(iarti_core:vessel('IMO5000005', 600, 1020, 120, 120, 300)), % Large: 4h
    assertz(iarti_core:vessel('IMO5000006', 630, 750, 24, 24, 20)),     % Tiny: 0.8h
    assertz(iarti_core:vessel('IMO5000007', 660, 1080, 80, 80, 200)),   % Medium: 2.7h
    assertz(iarti_core:vessel('IMO5000008', 690, 810, 30, 30, 25)).     % Small: 1h

% ============================================
% DATASET 6: Sequential Arrivals (n=8)
% ============================================
% Characteristic: Each vessel arrives after previous finishes
% Expected: All heuristics should perform equally (no contention)

load_dataset(ds6) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO6000001', 480, 720, 60, 60, 150)),   % 08:00-10:00
    assertz(iarti_core:vessel('IMO6000002', 600, 840, 60, 60, 150)),   % 10:00-12:00
    assertz(iarti_core:vessel('IMO6000003', 720, 960, 60, 60, 150)),   % 12:00-14:00
    assertz(iarti_core:vessel('IMO6000004', 840, 1080, 60, 60, 150)),  % 14:00-16:00
    assertz(iarti_core:vessel('IMO6000005', 960, 1200, 60, 60, 150)),  % 16:00-18:00
    assertz(iarti_core:vessel('IMO6000006', 1080, 1320, 60, 60, 150)), % 18:00-20:00
    assertz(iarti_core:vessel('IMO6000007', 1200, 1440, 60, 60, 150)), % 20:00-22:00
    assertz(iarti_core:vessel('IMO6000008', 1320, 1560, 60, 60, 150)). % 22:00-00:00

% ============================================
% DATASET 7: Random Distribution (n=10)
% ============================================
% Characteristic: Realistic chaos - random arrivals/departures/cargo
% Expected: EDT likely best overall

load_dataset(ds7) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO7000001', 435, 812, 78, 78, 195)),
    assertz(iarti_core:vessel('IMO7000002', 568, 945, 52, 52, 130)),
    assertz(iarti_core:vessel('IMO7000003', 723, 1089, 96, 96, 240)),
    assertz(iarti_core:vessel('IMO7000004', 512, 856, 44, 44, 110)),
    assertz(iarti_core:vessel('IMO7000005', 891, 1234, 68, 68, 170)),
    assertz(iarti_core:vessel('IMO7000006', 645, 978, 84, 84, 210)),
    assertz(iarti_core:vessel('IMO7000007', 789, 1156, 60, 60, 150)),
    assertz(iarti_core:vessel('IMO7000008', 456, 823, 72, 72, 180)),
    assertz(iarti_core:vessel('IMO7000009', 612, 1001, 56, 56, 140)),
    assertz(iarti_core:vessel('IMO7000010', 834, 1178, 88, 88, 220)).

% ============================================
% DATASET 8: Peak Hours - Morning Rush (n=10)
% ============================================
% Characteristic: All arrivals 07:00-10:00 (420-600 min)
% Expected: Severe congestion, high delays

load_dataset(ds8) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO8000001', 420, 720, 60, 60, 150)),
    assertz(iarti_core:vessel('IMO8000002', 435, 750, 65, 65, 162)),
    assertz(iarti_core:vessel('IMO8000003', 450, 780, 70, 70, 175)),
    assertz(iarti_core:vessel('IMO8000004', 465, 810, 55, 55, 138)),
    assertz(iarti_core:vessel('IMO8000005', 480, 840, 75, 75, 188)),
    assertz(iarti_core:vessel('IMO8000006', 495, 870, 50, 50, 125)),
    assertz(iarti_core:vessel('IMO8000007', 510, 900, 80, 80, 200)),
    assertz(iarti_core:vessel('IMO8000008', 525, 930, 60, 60, 150)),
    assertz(iarti_core:vessel('IMO8000009', 540, 960, 70, 70, 175)),
    assertz(iarti_core:vessel('IMO8000010', 555, 990, 65, 65, 162)).

% ============================================
% DATASET 9: Night Shift - Sparse (n=12)
% ============================================
% Characteristic: Arrivals spread across 22:00-06:00 (1320-360+1440)
% Expected: Low utilization, minimal delays

load_dataset(ds9) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO9000001', 1320, 1560, 48, 48, 120)),  % 22:00
    assertz(iarti_core:vessel('IMO9000002', 1380, 1620, 54, 54, 135)),  % 23:00
    assertz(iarti_core:vessel('IMO9000003', 1440, 1680, 60, 60, 150)),  % 00:00 (next day)
    assertz(iarti_core:vessel('IMO9000004', 1500, 1740, 42, 42, 105)),  % 01:00
    assertz(iarti_core:vessel('IMO9000005', 1560, 1800, 50, 50, 125)),  % 02:00
    assertz(iarti_core:vessel('IMO9000006', 1620, 1860, 46, 46, 115)),  % 03:00
    assertz(iarti_core:vessel('IMO9000007', 1680, 1920, 52, 52, 130)),  % 04:00
    assertz(iarti_core:vessel('IMO9000008', 1740, 1980, 48, 48, 120)),  % 05:00
    assertz(iarti_core:vessel('IMO9000009', 1800, 2040, 56, 56, 140)),  % 06:00
    assertz(iarti_core:vessel('IMO9000010', 1320, 1560, 44, 44, 110)),  % 22:00
    assertz(iarti_core:vessel('IMO9000011', 1440, 1680, 58, 58, 145)),  % 00:00
    assertz(iarti_core:vessel('IMO9000012', 1620, 1860, 50, 50, 125)).  % 03:00

% ============================================
% DATASET 10: Full Day Operations (n=12)
% ============================================
% Characteristic: Continuous arrivals 06:00-22:00, varied cargo
% Expected: Realistic full-day scenario

load_dataset(ds10) :-
    iarti_core:clear_vessels,
    assertz(iarti_core:vessel('IMO1000001', 360, 660, 65, 65, 162)),   % 06:00
    assertz(iarti_core:vessel('IMO1000002', 480, 780, 72, 72, 180)),   % 08:00
    assertz(iarti_core:vessel('IMO1000003', 540, 840, 58, 58, 145)),   % 09:00
    assertz(iarti_core:vessel('IMO1000004', 600, 900, 80, 80, 200)),   % 10:00
    assertz(iarti_core:vessel('IMO1000005', 720, 1020, 68, 68, 170)),  % 12:00
    assertz(iarti_core:vessel('IMO1000006', 780, 1080, 54, 54, 135)),  % 13:00
    assertz(iarti_core:vessel('IMO1000007', 840, 1140, 76, 76, 190)),  % 14:00
    assertz(iarti_core:vessel('IMO1000008', 960, 1260, 62, 62, 155)),  % 16:00
    assertz(iarti_core:vessel('IMO1000009', 1020, 1320, 70, 70, 175)), % 17:00
    assertz(iarti_core:vessel('IMO1000010', 1140, 1440, 64, 64, 160)), % 19:00
    assertz(iarti_core:vessel('IMO1000011', 1200, 1500, 58, 58, 145)), % 20:00
    assertz(iarti_core:vessel('IMO1000012', 1320, 1620, 66, 66, 165)). % 22:00