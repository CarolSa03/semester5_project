# US2.2.8 – Create/Submit Vessel Visit Notification

## 1. Context
Allows the Shipping Agent Representative to create and submit vessel visit notifications, scheduling dock and cargo operations.

**Related Issues:**
- Implementation: #8
- Tests: #58

---

## 2. Requirements

### User Story
> As a Shipping Agent Representative, I want to create/submit a vessel visit notification so that operations can be planned in advance.

### Acceptance Criteria
- **AC1:** Cargo manifest for loading/unloading must be included.
- **AC2:** Container IDs must be validated according to ISO 6346:2022.
- **AC3:** Crew information may be requested for security protocols.
- **AC4:** Notifications can remain "in progress" to be completed later.
- **AC5:** When ready, the agent must change the status to "submitted".

### Dependencies / References
- **ISO 6346 Validation:** Container IDs.
- **Security Protocols:** Crew information when required.

---

## 3. Design

### 3.1 Realization

#### Create Vessel Visit Notification

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/SARGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/SARGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.8/svg/L5-CreateVVN.svg)

#### Update Vessel Visit Notification

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.8/svg/L5-UpdateVVN.svg)

---

### 3.2 Applied Patterns
- **Controller:** `VesselVisitNotificationController` exposes endpoints for creating and submitting notifications. Only Shipping Agent Representatives can access these methods.
- **Service Layer:** `VesselVisitNotificationService` handles business logic, including:
  - Validation of container IDs (ISO 6346:2022 compliance).
  - Validation of required fields (vessel, ETA < ETD, dock assignment, cargo manifests).
  - Enforcing business rules for submission (status must be InProgress, ETA/ETD/date ranges, manifest presence, etc).
  - Security compliance: crew information is required for extended stays or dangerous cargo.
  - State transitions: sets status to PendingApproval on successful submission.
- **Domain Model:** `VesselVisitNotification` entity encapsulates state, validation logic, and submission readiness checks.
- **DTOs:** Data transfer objects ensure API boundary validation and data integrity.
- **Repository:** Abstracts persistence for notifications and related entities.

---

# 4. Acceptance Tests
- Create notification with a valid manifest.
- Reject invalid container IDs.
- Request crew information when required.
- Submit a ready notification.

---
