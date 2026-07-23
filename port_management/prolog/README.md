# 🚢 Port Scheduling & Planning Module - IARTI Backend

**Prolog-based REST API for vessel scheduling optimization**

[![SWI-Prolog](https://img.shields.io/badge/SWI--Prolog-9.x-blue)](https://www.swi-prolog.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Sprint](https://img.shields.io/badge/sprint-B-orange)](docs/)

---

## 📋 **Table of Contents**

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Installation](#installation)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Algorithms](#algorithms)
- [Documentation](#documentation)
- [Testing](#testing)
- [Performance](#performance)

---

## 🎯 **Overview**

This module implements **intelligent scheduling algorithms** for port operations, specifically focused on optimizing vessel loading/unloading sequences to **minimize departure delays**.

Developed as part of the **LEI-SEM5-PI-2025-26** integrative project at ISEP, this backend serves as the **Planning & Scheduling Engine** for the Port Management System.

### **Key Capabilities:**

- ✅ **Optimal Scheduling** (n ≤ 8 vessels) - Guaranteed best solution
- ✅ **Fast Heuristics** (n > 8 vessels) - Good quality in <0.1s
- ✅ **Multi-crane Optimization** - Minimize delays with parallel operations
- ✅ **Resource-aware** - Considers cranes, staff, and storage availability
- ✅ **REST API** - Easy integration with any frontend

---

## 🎨 **Features**

### **1. Multiple Scheduling Algorithms**

| Algorithm | Complexity | Quality | Best Use Case |
|-----------|-----------|---------|---------------|
| **Optimal (Brute-force)** | O(n!) | 100% | n ≤ 8, quality-critical |
| **EDT Heuristic** | O(n²) | 81% avg | n > 8, balanced approach |
| **SPT Heuristic** | O(n log n) | 62% avg | Large ports, speed-critical |
| **MST Heuristic** | O(n log n) | 58% avg | Experimental |
| **Multi-crane** | O(n! × c) | Eliminates delays | n ≤ 4, high-priority vessels |

### **2. Resource Constraint Handling**

- 🏗️ **Cranes:** Capacity, operational windows, setup time
- 👷 **Staff:** Qualifications, availability, shift schedules
- 📦 **Storage:** Capacity limits, dock assignments
- ⚓ **Docks:** Physical constraints, vessel type compatibility

### **3. Real-world Compliance**

- ✅ **US 3.4.2** - Optimal scheduling with feasibility validation
- ✅ **US 3.4.3** - Computational complexity analysis
- ✅ **US 3.4.4** - Alternative heuristics (EDT, SPT, MST)
- ✅ **US 3.4.5** - Multi-crane optimization with recommendations

---

## 🏗️ **Architecture**
```
port_management/
├── prolog/
│   ├── algorithms/
│   │   ├── iarti_core.pl              # Core predicates (sequence_temporization, sum_delays)
│   │   ├── optimal_scheduler.pl       # US 3.4.2 - Brute-force + EDT
│   │   ├── greedy_scheduler.pl        # US 3.4.4 - SPT, MST, SPT_IMPROVED
│   │   └── multi_crane_scheduler.pl   # US 3.4.5 - Multi-crane backtracking
│   ├── server/
│   │   ├── server.pl                  # HTTP server setup
│   │   ├── routes.pl                  # Endpoint routing
│   │   └── server_callbacks.pl        # Request handlers
│   ├── tests/
│   │   ├── datasets.pl                # 10 test scenarios (DS1-DS10)
│   │   └── run_comparison.pl          # Heuristics quality benchmarking
│   └── main.pl                        # Entry point
├── docs/
│   ├── complexity-analysis.md
│   ├── heuristics_comparison.md
│   └── openapi.yaml                   # API specification v2.0.0
└── README.md
```

### **Design Principles:**

- **Modular:** Each algorithm is a separate module with clear interfaces
- **Extensible:** Easy to add new heuristics or constraints
- **Testable:** Comprehensive test suite with reproducible datasets
- **Documented:** OpenAPI spec + technical reports

---

## 🚀 **Installation**

### **Prerequisites**

- **SWI-Prolog 9.x** or higher ([Download](https://www.swi-prolog.org/Download.html))
- **Git** for version control

### **Setup**
```bash
# Clone the repository
git clone https://github.com/your-org/LEI-SEM5-PI-2025-26-3DI-04.git
cd LEI-SEM5-PI-2025-26-3DI-04/port_management/prolog

# Start the server (default port: 5001)
swipl main.pl

# Or specify a custom port
swipl -g "server:start(8080)" main.pl
```

**Expected output:**
```
Starting Prolog REST server on port 5001...
% Started server at http://localhost:5001/
```

---

## 💻 **Usage**

### **Quick Start Example**
```bash
# Health check
curl http://localhost:5001/health

# Schedule 3 vessels (optimal)
curl -X POST http://localhost:5001/schedule \
  -H "Content-Type: application/json" \
  -d '{
    "requestId": "test-001",
    "vessels": [
      {"imo": "IMO1234567", "eta": 480, "etd": 660, "cargo": 100},
      {"imo": "IMO1234568", "eta": 540, "etd": 720, "cargo": 75},
      {"imo": "IMO1234569", "eta": 600, "etd": 780, "cargo": 150}
    ],
    "resources": [
      {"code": "CRANE-01", "capacity": 30, "status": "available"}
    ],
    "staff": [
      {"id": "STAFF-001", "status": "available"}
    ],
    "storage_areas": [
      {"code": "YARD-A", "type": "yard", "capacity": 1000}
    ]
  }'
```

### **Response Example**
```json
{
  "requestId": "test-001",
  "success": true,
  "schedule": [
    {
      "vessel": "IMO1234567",
      "start": "08:00",
      "end": "10:00",
      "crane": "CRANE-01",
      "staff": "STAFF-001",
      "storage": "YARD-A",
      "estimatedDuration": 2.0
    },
    // ... more vessels
  ],
  "totalDelay": 45,
  "algorithm": "IARTI_Optimal_BruteForce",
  "performanceMetrics": {
    "computationTimeSeconds": 0.123,
    "problemSize": {"vessels": 3, "resources": 1, "staff": 1},
    "complexity": "O(n!)"
  }
}
```

---

## 🔌 **API Endpoints**

Full API documentation: **[OpenAPI Spec](docs/openapi.yaml)**

### **1. GET `/health`**

Health check endpoint.

**Response:**
```json
{
  "status": "ok",
  "module": "scheduler-backend-iarti",
  "algorithms": [
    "IARTI_Optimal_BruteForce_EDT",
    "IARTI_Greedy_SPT",
    "IARTI_Greedy_EDT",
    "IARTI_Greedy_MST",
    "IARTI_Multi_Crane_Backtracking"
  ]
}
```

---

### **2. POST `/schedule`**

**Optimal scheduler** - Uses brute-force (n≤8) or EDT heuristic (n>8).

**Request Body:**
- `vessels[]` - Array of vessel objects (imo, eta, etd, cargo)
- `resources[]` - Available cranes
- `staff[]` - Available personnel
- `storage_areas[]` - Storage locations

**Algorithm Selection:**
- **n ≤ 8:** Brute-force exhaustive search (guaranteed optimal)
- **n > 8:** EDT heuristic (81% quality, fast)

**Use Cases:**
- Daily planning (n=10-30 vessels)
- Quality-critical scenarios
- Tight deadline schedules

---

### **3. POST `/schedule/greedy`**

**Fast heuristic scheduler** - Multiple algorithm options.

**Request Body (additional field):**
```json
{
  "heuristic": "SPT",  // Options: SPT, EDT, MST, SPT_IMPROVED
  "vessels": [...],
  // ... same as /schedule
}
```

**Available Heuristics:**

| Heuristic | Description | Complexity | Quality |
|-----------|-------------|-----------|---------|
| **SPT** | Shortest Processing Time First (default) | O(n log n) | 62% |
| **EDT** | Early Departure Time | O(n log n) | 81% |
| **MST** | Minimum Slack Time | O(n log n) | 58% |
| **SPT_IMPROVED** | SPT with availability check | O(n²) | 68% |

**Use Cases:**
- Real-time rescheduling (<0.01s response)
- Large ports (n>50 vessels)
- Testing different strategies

---

### **4. POST `/schedule/multi-crane`**

**Multi-crane optimizer** - Two-phase approach with comparison.

**Behavior:**
1. **Phase 1:** Compute single-crane schedule
2. **Phase 2:** If delay > 0, try multi-crane allocation (n≤4 only)
3. **Output:** Both strategies + comparison + recommendation

**Response (additional fields):**
```json
{
  "singleCraneStrategy": { /* ScheduleResponse */ },
  "multiCraneStrategy": { /* ScheduleResponse with cranes[] */ },
  "comparison": {
    "delayReduction": 90,
    "extraCraneHours": 3.5,
    "recommendation": "use_multi_crane"
  }
}
```

**Recommendation Logic:**
- `use_single_crane` - Multi-crane doesn't help or too costly
- `use_multi_crane` - Significant delay reduction, cost-effective
- `use_multi_crane_if_resources_allow` - Helps but resource-intensive
- `problem_too_large_or_insufficient_cranes` - n>4 or cranes<2

**Use Cases:**
- High-priority vessels
- Emergency rescheduling
- Cost-benefit analysis

---

## 🧠 **Algorithms**

### **Optimal Scheduler (US 3.4.2)**

**Brute-force (n ≤ 8):**
```prolog
obtain_seq_shortest_delay(SeqBetterTriplets, SShortestDelay) :-
    findall(V, vessel(V,_,_,_,_,_), LV),
    permutation(LV, SeqV),              % Try all permutations
    sequence_temporization(SeqV, SeqTriplets),
    sum_delays(SeqTriplets, S),
    compare_shortest_delay(SeqTriplets, S),
    fail.
```

**EDT Heuristic (n > 8):**
```prolog
heuristic_early_departure_time(SeqTripletsH, SDelaysH) :-
    findall((Departure, V), vessel(V, _, Departure, _, _, _), LDV),
    sort(LDV, LDVSorted),               % Sort by departure time
    sequence_temporization(SeqV, SeqTriplets),
    sum_delays(SeqTriplets, SDelaysH).
```

**Key Insight:** EDT prioritizes vessels with early departures, reducing cascading delays.

---

### **Greedy Heuristics (US 3.4.4)**

**SPT (Shortest Processing Time):**
```prolog
heuristic_shortest_processing_time(SeqTripletsH, SDelaysH) :-
    findall((ProcTime, V), (
        vessel(V, _, _, TUnload, TLoad, _),
        ProcTime is TUnload + TLoad
    ), LPV),
    sort(LPV, LPVSorted),               % Sort by processing time
    sequence_temporization(SeqV, SeqTriplets),
    sum_delays(SeqTriplets, SDelaysH).
```

**When SPT Excels:**
- High variance in vessel sizes (10 TEU vs 500 TEU)
- Large vessels have flexible schedules
- Speed is critical (fastest heuristic)

**When SPT Fails:**
- Uniform vessel sizes
- Tight deadlines
- Early arrivals with late processing

---

### **Multi-crane (US 3.4.5)**

**Strategy:**
1. Generate all vessel permutations
2. For each permutation, try 1-MaxCranes per vessel
3. Select allocation that minimizes:
    - **Primary:** Total delay
    - **Secondary:** Multi-crane usage intensity

**Example:**
```
Single-crane: Vessel A (2h) → Vessel B (3h) → Total delay: 120 min
Multi-crane:  Vessel A (1h, 2 cranes) → Vessel B (1.5h, 2 cranes) → Total delay: 30 min
Recommendation: use_multi_crane (saves 90 min, uses 3.5 extra crane-hours)
```

---

## 📚 **Documentation**

### **Technical Reports**

1. **[US 3.4.3 - Computational Complexity Analysis](docs/complexity-analysis.md)**
    - Theoretical analysis (O(n!), O(n²), O(n log n))
    - Empirical benchmarks (n=3 to n=50)
    - Time estimation formulas
    - Bottleneck analysis & optimization opportunities

2. **[US 3.4.4 - Heuristics Comparison Study](docs/heuristics_comparison.md)**
    - 10 diverse test datasets
    - Quality comparison (EDT: 81%, SPT: 62%, MST: 58%)
    - Scenario-based recommendations
    - Scalability validation (n=30 vessels)

3. **[OpenAPI Specification v2.0.0](docs/openapi.yaml)**
    - Complete API reference
    - Request/response schemas
    - Interactive documentation (import to Swagger UI)

### **Academic References**

Based on IARTI (Artificial Intelligence) course materials:
- Pinedo, "Scheduling: Theory, Algorithms, and Systems"
- Jackson (1955), "Scheduling a production line to minimize maximum tardiness"
- IARTI Lecture Slides: Sequencing, Heuristics, Combinatorial Explosion

---

## 🧪 **Testing**

### **Unit Tests**
```bash
# Load test suite
swipl
?- [tests/run_comparison].

# Run all heuristic comparisons (10 datasets)
?- run_all_tests.

# Run specific dataset
?- run_test(ds5).
```

### **Test Datasets**

| Dataset | n | Scenario | Key Characteristic |
|---------|---|----------|-------------------|
| DS1 | 5 | Uniform arrivals | Evenly spaced (baseline) |
| DS2 | 5 | Clustered arrivals | High contention |
| DS3 | 7 | Mixed sizes | 15x cargo variance |
| DS4 | 7 | Tight deadlines | Minimal slack time |
| DS5 | 8 | Large variance | 50x cargo variance (10-500 TEU) |
| DS6 | 8 | Sequential | Zero contention (perfect case) |
| DS7 | 10 | Random | Realistic chaos |
| DS8 | 10 | Peak hours | Morning rush (07:00-10:00) |
| DS9 | 12 | Night shift | Sparse arrivals |
| DS10 | 12 | Full day | Continuous operations |

### **Performance Benchmarks**
```
Optimal (n=8):     0.276s  →  100% quality
EDT (n=10):        0.000s  →   81% quality
SPT (n=30):        0.005s  →   62% quality
Multi-crane (n=4): 1.200s  →  Eliminates delays
```

---

## 📊 **Performance**

### **Scalability**

| Problem Size | Optimal | EDT | SPT | Recommended |
|--------------|---------|-----|-----|-------------|
| n ≤ 8 | ✅ <15s | ✅ <0.1s | ✅ <0.01s | **Optimal** |
| n = 9-20 | ❌ Days | ✅ <2s | ✅ <0.2s | **EDT** |
| n = 21-50 | ❌ Years | ✅ <10s | ✅ <0.5s | **SPT** |
| n > 50 | ❌ | ⚠️ Slow | ✅ <2s | **SPT only** |

### **Quality vs Speed Trade-off**
```
Quality (%)
100 │     ●  Optimal
    │    /│
 90 │   / │
    │  /  │  ●  EDT (81%)
 80 │ /   │ /
    │/    │/
 70 │     ●  SPT (62%)
    │    /
 60 │   /
    │  ●  MST (58%)
    └─────────────────────── Speed (log scale) →
    0.001s  0.01s  0.1s  1s  10s
```
---
## 🔗 **Links**

- **Project Repository:** [GitHub](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DI-04)
- **API Documentation:** [OpenAPI Spec](docs/openapi.yaml)
- **Technical Reports:** [docs/](docs/)
- **SWI-Prolog:** [https://www.swi-prolog.org/](https://www.swi-prolog.org/)

---

## 📞 **Support**

For questions or issues:
1. Check [Documentation](docs/)
2. Review [OpenAPI Spec](docs/openapi.yaml)
3. Open an issue on GitHub
4. Contact the team

---

<div align="center">

**Made with ❤️ and ☕ by Antonio, Carolina, Luís and Tiago @ ISEP**

</div>