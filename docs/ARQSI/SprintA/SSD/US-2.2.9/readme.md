# US2.2.9 – Edit or Withdraw Vessel Visit Notification

## 1. Context

This feature allows the Shipping Agent Representative to change, complete, or withdraw a vessel visit notification while it is still in progress. This supports correcting errors, updating information, or canceling requests before submission for approval.

**Related Issues:**
- Implementation: #9
- Tests: #59

---

## 2. Requirements

### User Story

> As a Shipping Agent Representative, I want to change or complete a Vessel Visit Notification while it is still in progress, so that I can correct errors or withdraw requests if necessary.

### Acceptance Criteria

- **AC1:** The notification can be edited while its status is "in progress".
- **AC2:** The representative can change the status to "submitted / approval pending" when ready.
- **AC3:** The representative can withdraw the notification before it is submitted for approval.
- **AC4:** All changes and withdrawals are tracked with timestamps and user identification for auditing.

### Dependencies / References

- Status management for notifications.
- Audit trail for changes and withdrawals.

---

## 3. Design

### 3.1 Realization

#### Edit/Submitt Vessel Visit Notification

![Sequence Diagram Level 3](Documentation/US-2.2.9/svg/L5-EditOrSubmitVesselVisitNotification.svg)



---

### 3.2 Applied Patterns

- **Controller:** `VesselVisitNotificationController` exposes endpoints for editing, submitting, and withdrawing notifications, accessible only to Shipping Agent Representatives.
- **Service Layer:** Handles business logic for status transitions, validation, and auditing.
- **Domain Model:** Manages state, validation, and withdrawal logic.
- **DTOs:** Ensure data integrity and validation at the API boundary.
- **Repository:** Handles persistence and audit trail.

---

### 3.3 Acceptance Tests

- Edit a notification while in progress.
- Submit a notification for approval.
- Withdraw a notification before submission.
- Verify audit records for all changes and withdrawals.

---
