# Business Impact Analysis (BIA) & Risk Management Update

---

## 1. Introduction

This document presents the Business Impact Analysis (BIA) for the final solution of the Port Logistics Management System. It identifies critical components, quantifies the impact of their failure on port operations, and defines continuity requirements (MTD, RTO, RPO). It also updates the Risk Register established in Sprint B, reclassifying existing risks and identifying new threats introduced by Sprint C features (OEM, Genetic Algorithms, 3D Visualization).

---

## 2. Business Impact Analysis (BIA)

The analysis identifies operational and financial impacts of downtime for each critical module. It validates the infrastructure requirement for a Maximum Tolerable Downtime (MTD) of 20 minutes for execution-critical components.

### 2.1 Criticality Matrix

| Critical Component | Description & Dependency | Impact of Failure (Qualitative) | MTD (Max Downtime) | RTO (Target) | RPO (Data Loss) | Criticality Level |
|---|---|---:|---:|---:|---:|---:|
| OEM Module (Operations & Execution) | Records real-time events (Gate-in, Load/Unload, Service Completion). Dependent on Database. | CRITICAL. Operations become "blind". Inability to track cargo handover creates safety risks and blocks billing triggers. Immediate truck/crane congestion. | 20 min | 15 min | < 5 min | Tier 1 |
| Core Database & API | Central persistence for all modules (Vessels, Containers, Users). | CATASTROPHIC. Total system paralysis. No authentication, no data retrieval. Legal risk regarding manifest data availability. | 20 min | 15 min | ~0 (Sync Rep) | Tier 1 |
| Planning Engine (Genetic Algos) | Generates stowage plans and resource schedules. | HIGH. Inability to plan future vessels. Operations on current vessels can continue manually for a short period. | 4 Hours | 2 Hours | 24 Hours | Tier 2 |
| 3D Visualization (WebGL) | Visual monitoring of port state. Dependent on Read-Only APIs. | LOW/MEDIUM. Loss of situational awareness. Operations can persist using tabular data views (SPA lists) as a fallback. | 24 Hours | 8 Hours | N/A | Tier 3 |

### 2.2 Impact Categories

- Operational: High congestion in the container yard; crane operations halted due to missing move orders.
- Financial: Contractual penalties (Demurrage charges) for delayed vessels; loss of throughput revenue.
- Legal/Compliance: Failure to provide Cargo Manifest data to Customs (US 2.2.8) or personal data breach (GDPR) in crew logs.
- Reputational: Loss of trust from Shipping Lines, leading to redirection of routes to competing ports.

---

## 3. Risk Management Update

Review of the risks identified in Sprint B (file `Risk_analysis.md`) and addition of new risks from Sprint C.

### 3.1 Review of Sprint B Risks (Status Update)

| ID | Risk Description | Original Level | Mitigation Implemented (Sprint B/C) | Current Status |
|---:|---|---:|---|---|
| R1 | Docker Host Disk Space Exhaustion | High | Log rotation configured; separate volumes for DB. Note: Monitoring required due to the high volume of OEM logs. | Residual (Medium) |
| R8 | SSH Brute-Force Attack | High | VPN restriction, IP Allowlisting, Key-based Auth (US 4.4.10). | Mitigated (Low) |
| R14 | IAM Provider (Google) Failure | High | Token caching implementation. Constraint: Still a Single Point of Failure for login. | Residual (High) |
| R19 | PII Exposure in Logs (GDPR) | High | Structured logging with sanitization filters. | Residual (Low) |
| R3 | Service Overload (Single VM) | Medium | Resource limits (Docker). Upgrade: Load balancing proposed in US 4.4.5. | Mitigated (Low) |

### 3.2 New Risks Identified in Sprint C

| ID | Risk Name | Context | Impact | Prob. | Mitigation Strategy |
|---:|---|---|---:|---:|---|
| R_NEW_01 | Algorithm Non-Convergence | The Genetic Algorithm (US 4.3.1) may fail to find a valid schedule within the time limit for complex datasets. | High (Planning Delay) | Med | Implement a timeout & fallback mechanism to switch to a heuristic algorithm (Greedy) if GA > 5 mins. |
| R_NEW_02 | Plan vs. Execution Drift | OEM records deviations (e.g., wrong slot) that are not fed back to the Planner, causing invalid future plans. | High (Data Corruption) | Low | Implement strong consistency checks in the OEM API before confirming tasks. |
| R_NEW_03 | Client-Side DoS (WebGL) | Rendering thousands of containers (US 4.2.2) crashes the browser on low-end hardware. | Low (Usability) | High | Implement Level of Detail (LOD) and pagination for 3D objects; provide a 2D list fallback. |
| R_NEW_04 | Sensitive Crew Data Leak | OEM records crew details (US 2.2.8) which might be exposed in plain text via API responses or logs. | Critical (GDPR Fine) | Med | Enforce field-level encryption for PII in the DB; mask crew names in general logs. |

### 3.3 Risk Matrix (Probability vs. Impact)
The following matrix visualizes the prioritization of risks (residuals from Sprint B and new threats from Sprint C), highlighting which areas require immediate attention.

|Probability ↓ \ Impact →   |  Low |  Medium |  High | Critical  |
|---|---|---|---|---|
|  High | R_NEW_03 (WebGL Crash)  |   |   |   |
|  Medium |   | R1 (Disk Space - Residual)  |  R_NEW_01 (Algo Non-Convergence) | R_NEW_04 (Crew Data Leak)  |
|  Low |  R3 (Service Overload)<br/>R8 (SSH Brute-Force)<br/>R19 (PII Logs) |   |  R_NEW_02 (Execution Drift) | R14 (IAM Provider Failure)  |


---

## 4. Continuity Strategy Recommendations

Based on the BIA (Tier 1 components requiring MTD < 20 min) and the updated Risk Register:

- High Availability (Clustering): Implement a Load Balancer (Nginx/HAProxy) distributing traffic to redundant backend instances (US 4.4.5) to mitigate R3 and ensure RTO < 15 min.
- Database Replication: Deploy master-slave replication to minimize RPO and support the high throughput of the OEM module.
- Automated Failover: Scripts to detect service health and switch traffic/DB roles automatically, mitigating the "Human Availability" bottleneck.

---

## Conclusion

The transition from "Planning" (Sprint B) to "Execution" (Sprint C) has increased the system's criticality. The proposed mitigations and infrastructure upgrades are essential to support 24/7 port operations.
