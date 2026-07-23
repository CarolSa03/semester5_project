# 🛡️ Disaster Recovery Plan (DR Plan) - Staging Environment

## 1. Objective and Scope

**Objective:** To ensure that the port management system in the staging environment can be restored to an operational state (minimum service) within the established **Minimum Business Continuity Objective (MBCO)**.

**Scope:** Staging environment hosted on the DEI (ISEP) infrastructure.

**Target MBCO (from Sprint B):**
* **RTO (Recovery Time Objective):** Restoration to a minimally functional state (Phase 1) within **2 hours** of incident detection.
* **Minimum State:** **Read-Only** access to core data (VVNs, Vessels, Docks, Storage Areas, Resources).

**Target RPO (Recovery Point Objective):** **24 hours** (Guaranteed by the daily backup implemented in **US 4.4.6**).

## 2. Roles and Responsibilities

| Role | Main Responsibility |
| :--- | :--- |
| **DR Leader (Project Manager)** | Declaring the disaster, initiating procedures, and managing stakeholder communication. |
| **System Administrator** | Executing technical steps: infrastructure recovery, backup restoration, and application deployment. |
| **Development Team** | Post-restoration validation (smoke tests) and resolving application-specific issues. |

## 3. Disaster Recovery Procedure (DRP)

The *2-hour RTO* countdown begins upon Incident Detection/Declaration. The process leverages existing automation tools (infrastructure scripts, CI/CD pipelines, and backup/restore scripts).

| Step | Procedure | Estimated Duration | MBCO Objective |
| :--- | :--- | :--- | :--- |
| **1. Assessment & Declaration** | Confirm the failure and document the incident start time. | 5 min | |
| **2. Environment Provisioning** | **Recreate/Repair Infrastructure:** Run the automated VM configuration script (`setup-asist-infra.sh` or equivalent). | 30 min | |
| **3. Data Restoration (RPO)** | **Restore Database:** Execute the automated restore script using the most recent backup (`<db_name>_yyyymmdd`). | 45 min | RPO (Data Loss) minimized. |
| **4. Application Deployment** | **Redeploy Backend/Frontend:** Trigger the CI/CD pipeline to deploy modules. **Configure Backend to block write operations (POST/PUT/DELETE)**. | 30 min | |
| **5. Minimum Service Validation** | **Smoke Tests:** Perform data integrity validation (Execution of **US 4.4.12** - Automated Restore Validation). **Enable Read-Only Access (MBCO Phase 1)**. | 10 min | **RTO ≤ 2 hours met.** |
| **6. Critical Write Restoration** | Re-enable critical write operations (VVN submission/approval, manifest uploads). | Variable | **Phase 2 - Partial Service.** |
| **7. Full Recovery** | Re-enable all features. Monitor performance and confirm return to baseline levels. | Variable | **Phase 3 - Full Service.** |

## 4. Validation and Recovery Testing

The DR Plan must be validated to ensure the 2-hour RTO is achievable.

### Test Requirements

| Item | Acceptance Criteria | Source / Basis |
| :--- | :--- | :--- |
| **Simulated Test** | The plan must be validated through **at least one simulated recovery test**. | US 4.4.1 |
| **MBCO Compliance** | Measured recovery time must be ≤ 2 hours. | US 4.4.1 |
| **Integrity Test** | The automated restore test script (**US 4.4.12**) must execute successfully after restoration. | US 4.4.12 |

### Standard Test Scenario
1. Simulate a total data loss on the VM storage volume.
2. Execute DRP steps **1 through 5** and measure the elapsed time.
3. Total time must be ≤ 120 minutes.
4. Logs must confirm that data validation (**US 4.4.12**) was successful.

## 5. Note on MTD (Maximum Tolerable Downtime)

The **2-hour RTO** defined in this MBCO is appropriate for an educational staging environment. To achieve an MTD of **20 minutes** (as required by **US 4.4.2** for critical production environments), investment in High Availability (HA) infrastructure, including real-time database replication and automated failover, would be necessary. This exceeds the scope of the current environment.