# ASIST – Risk Analysis

This document identifies and quantifies the main risks associated with the developed solution.

We group risks into five categories:

- Technical
- Operational
- Security
- Backup
- Business Continuity (BC)



For each risk we assign **Likelihood** (L/M/H) and **Impact** (L/M/H), propose **Mitigation** strategies, and estimate the **Residual Risk**.

## 1. Technical Risks

| ID | Category  | Risk Description                                                  | Likelihood | Impact | Mitigation                                                                                       | Residual Risk |
|----|-----------|-------------------------------------------------------------------|-----------|--------|--------------------------------------------------------------------------------------------------|--------------|
| R1 | Technical | Docker host runs out of disk space (images, logs, DB grow).      | M         | H      | Log rotation, docker cleanup scripts, separate data volume for DB, monitoring `df -h`.          | Low–Med      |
| R2 | Technical | Misconfiguration of CI/CD breaks deployment.                      | M         | M      | Separate “checks” workflow from deploy, use smoke tests, keep manual fallback deployment steps. | Low          |
| R3 | Technical | Single VM running multiple services becomes overloaded.          | L–M       | M      | Resource monitoring, container resource limits, possibility to move services to another VM.     | Med          |
| R4 | Technical | Incorrect PAM configuration causes SSH lockout.                  | L         | H      | Backup PAM files before changes, test on secondary user/session, document revert procedure.     | Low–Med      |

## 2. Operational Risks

| ID | Category    | Risk Description                                     | Likelihood | Impact | Mitigation                                                                 | Residual Risk |
|----|-------------|------------------------------------------------------|-----------|--------|----------------------------------------------------------------------------|--------------|
| R5 | Operational | GitHub Actions runner VM unavailable during deploy. | M         | M      | Allow manual deploy using rsync/ssh, keep script `setup-dei-vm.sh`.        | Med          |
| R6 | Operational | Human error when editing allowlist or nginx config. | M         | M      | Use `apply-allowlist.sh` with validation + nginx `-t` before reload.       | Low–Med      |
| R7 | Operational | Backups not tested, restore fails when needed.      | M         | H      | Periodic restore test (`restore-example.sh`), documented restore procedure.| Low–Med      |

## 3. Security Risks

| ID | Category | Risk Description                                             | Likelihood | Impact | Mitigation                                                                 | Residual Risk |
|----|----------|--------------------------------------------------------------|-----------|--------|----------------------------------------------------------------------------|--------------|
| R8 | Security | Brute-force SSH attacks from Internet or internal network.  | M         | H      | VPN + DEI network restriction, fail2ban, PAM lockout, MFA via Google Auth. | Low          |
| R9 | Security | Compromise of admin credentials (SSH or IAM).               | L–M       | H      | Strong password policy, MFA, limited SSH group, logging and alerts.        | Med          |
| R10| Security | Exposure of personal data via application misconfigurations.| L–M       | H      | Role-based access control, logs, HTTPS, adherence to GDPR guidelines.      | Med          |

## 4. Backup & Business Continuity Risks

| ID | Category | Risk Description                                | Likelihood | Impact | Mitigation                                                                                 | Residual Risk |
|----|----------|-------------------------------------------------|-----------|--------|--------------------------------------------------------------------------------------------|--------------|
| R11| Backup   | Loss of VM storage (disk failure, corruption).  | L         | H      | Daily DB backup to `/srv/backups -> /home/backups`, periodic off-VM copy (future work).    | Med          |
| R12| Backup   | Backup script fails silently.                   | M         | H      | Log backup runs, email on failure, manual verification, automated restore test occasionally.| Low–Med      |
| R13| BC       | Long downtime after major incident.             | M         | H      | Automated VM setup script, CI/CD deployment, documented MBCO (2 hours) and recovery steps. | Med          |

## 5. Application & Data Risks

| ID  | Category    | Risk Description                                                                 | Likelihood | Impact | Mitigation                                                                                                                                           | Residual Risk |
|-----|-------------|----------------------------------------------------------------------------------|-----------|--------|------------------------------------------------------------------------------------------------------------------------------------------------------|--------------|
| R14 | Application | IAM/OAuth provider (Google) becomes unavailable or returns invalid tokens.      | M         | H      | Implement token validation with graceful error handling, cache user sessions temporarily, document manual fallback procedure, monitor IAM uptime.    | Med          |
| R15 | Application | Authorization bypass allowing privilege escalation (e.g., user accessing admin). | L-M       | H      | Server-side RBAC enforcement on all API endpoints, automated security testing in CI/CD, regular code reviews, audit logging of authorization failures.| Low-Med      |
| R16 | Application | Database schema migration fails or corrupts production data.                     | M         | H      | Test migrations on staging environment first, backup DB before migration, use Entity Framework rollback scripts, version control for schema changes. | Low          |

## 6. GDPR & Compliance Risks

| ID  | Category   | Risk Description                                                                      | Likelihood | Impact | Mitigation                                                                                                                                                      | Residual Risk |
|-----|------------|---------------------------------------------------------------------------------------|-----------|--------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------|
| R19 | Compliance | Personal data (citizen IDs, emails, phone numbers) exposed via logs or error messages.| M         | H      | Sanitize logs to exclude PII, structured logging with severity levels, encrypt sensitive fields in database, implement log access controls, GDPR training.      | Low-Med      |
| R20 | Compliance | Data retention policy not enforced, leading to unnecessary storage of personal data.  | M         | M      | Define retention periods per entity type, automated cleanup jobs for expired data, document retention policy, implement "right to be forgotten" endpoints.      | Med          |
| R21 | Compliance | Lack of encryption for personal data at rest or in transit.                          | L         | H      | HTTPS enforcement (TLS 1.2+), database encryption at rest (Transparent Data Encryption), encrypted backup files, certificate management and renewal automation. | Low          |
| R22 | Compliance | Data breach notification procedure undefined or untested.                            | M         | H      | Document breach response plan, designate Data Protection Officer contact, test notification workflow annually, maintain audit trail of all data access.         | Low-Med      |

## 7. Summary

The implemented controls (CI/CD, VPN/IP restrictions, PAM hardening, fail2ban, and backup scripts) reduce most risks to a **Low–Medium** residual level. The remaining risks are mostly organizational (e.g., failure to monitor logs, not testing restores), which are addressed by documentation, procedures, and sprint reviews.

