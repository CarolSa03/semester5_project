# ASIST – Minimum Business Continuity Objective (MBCO)

This document defines the Minimum Business Continuity Objective (MBCO) for the system and relates it to the identified risks and backup/recovery capabilities. The MBCO establishes measurable recovery targets and service availability expectations for the staging environment.

## 1. Scope

The scope of this MBCO is the **staging environment** of the port management prototype deployed in the DEI infrastructure. It supports:

- Port Authority Officers (approval of Vessel Visit Notifications)
- Shipping Agent Representatives (submission and tracking of Vessel Visit Notifications)
- Logistics Operators (visualization and basic planning support)

## 2. MBCO Definition

We define the MBCO in measurable terms:

- **Maximum acceptable downtime per incident:**  
  - The system must be restored to a minimally functional state within **2 hours** of incident detection. This RTO applies to incidents affecting core application functionality, data access, or deployment infrastructure.

- **Minimum acceptable service capacity during recovery:**
The system shall be restored in phases:
**Phase 1 - Minimal Service (Target: within 120 minutes)**
- At least **read-only access** to core data must be available:
  - Vessel Visit Notifications (viewing, approval history, status tracking)
  - Vessel and dock information (searchable, accessible)
  - Storage area availability and capacity data
  - Resources and staff assignments (reference data)
- User authentication must be operational (Google OAuth via IAM integration)
- GET endpoints return valid data with HTTP 200 responses
- **Write operations are blocked** temporarily to prevent data inconsistency during recovery.

**Phase 2 - Partial Service**
- Critical write operations are restored, prioritized as follows:
  1. New Vessel Visit Notification submissions.
  2. Vessel Visit Notification approvals and status updates.
  3. Cargo manifest uploads and modifications.
- Performance may be degraded.

**Phase 3 - Full Service**
- All read and write operations fully operational
- Scheduled tasks and background processing resume
- Performance returns to normal operating levels
- System health monitoring confirms baseline metrics

## 3. Incident Detection and Response Initiation

The 2-hour RTO countdown begins when:

- **Manual Detection**: An incident is reported by users or team members via Discord/email and acknowledged in team communication channel
- **Automated Detection**: System health check fails or alerting system triggers incident notification
- **Documented Start Time**: Incident timestamp is recorded in team log or ticketing system

## 4. Justification and Alignment with Risks

The 2-hour MBCO is justified by:

- The environment is **non-production** and used for educational purposes, but must remain reasonably available for development and evaluation.
- Incident recovery can leverage:
  - The **automated VM setup script** (`setup-asist-infra.sh`), which installs required dependencies and configures key services.
  - The CI/CD pipeline that **automates deployment** of the application to the DEI VM.
  - The **backup script** (`backup.sh`), which enables restoration of the database and core configuration files.

Given these tools, it is realistic to:

1. Reprovision a new VM or repair the existing one (install base packages).
2. Redeploy the application (docker compose).
3. Restore the most recent database backup.
4. Bring the system to a minimal, read-only state within **2 hours**.

This MBCO directly addresses the following identified risks from the accompanying risk assessment:

- **R1 (Disk Exhaustion)**: RTO/RPO provides time frame for infrastructure repair or provisioning
- **R11 (VM Storage Loss)**: RPO mitigates permanent data loss via daily backups
- **R13 (Long Downtime)**: 2-hour RTO ensures bounded recovery window
- **R16 (Schema Migration Failure)**: Backup/restore capability enables safe rollback

## 4. Stakeholder Proposal and Approval

The MBCO is proposed to project stakeholders as follows:

> “In case of a major incident affecting the staging environment, the team commits to restoring minimal service (read-only access to core data) within 2 hours, using automated deployment and backup/restore procedures.”

This proposal is considered **approved** if no objections or changes are raised during sprint review / evaluation sessions. Any refinements requested by stakeholders (e.g., stricter downtime or higher minimum capacity) will be documented and addressed in subsequent sprints.

## 5. Future Improvements

Future work may include:

- Automated off-VM backups (e.g., rsync to a secondary VM).
- More granular service degradation modes (e.g., partial functionality with strict prioritization of certain roles).
- Monitoring and alerting tools to detect incidents earlier and reduce actual downtime.

