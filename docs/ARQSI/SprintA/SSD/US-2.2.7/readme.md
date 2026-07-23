# US2.2.7 – Approve or Reject Vessel Visit Notifications

## 1. Context
This feature allows the Port Authority Officer to review pending vessel visit notifications and approve or reject them, ensuring control over dock scheduling and vessel compatibility.

**Related Issues:**
- Implementation: #7
- Tests: #57

---

## 2. Requirements

### User Story
> As a Port Authority Officer, I want to approve or reject vessel visit notifications so that dock scheduling and vessel compatibility remain under the port's control.

### Status Lifecycle
- **InProgress:** Notification is being created or edited.
- **PendingApproval:** Submitted and waiting for port authority approval.
- **Approved:** Approved by port authority (dock assigned).
- **Rejected:** Rejected by port authority (reason required).
- **Withdrawn:** Withdrawn by shipping agent.

### Acceptance Criteria
- **AC1:** Only Port Authority Officers can approve or reject notifications.
- **AC2:** Approval requires assigning a valid dock (DockAssignment) compatible with the vessel type and size.
- **AC3:** Rejection requires a reason (RejectionReason).
- **AC4:** Officer ID and timestamp are recorded for both approval (ApprovedBy, ApprovedAt) and rejection (RejectedBy, RejectedAt).
- **AC5:** Approval notes (ApprovalNotes) can be added.
- **AC6:** All decisions are auditable.
- **AC7:** If rejected, the representative can update and resubmit the notification.

### Dependencies / References
- **Dock Management:** Only valid docks can be assigned, validated against vessel type and dock capacity.
- **Auditing:** All decisions are recorded for future reference.

---

## 3. Design

### 3.1 Realization

#### Approve Vessel Visit Notification

![Sequence Diagram Level 3](Documentation/US-2.2.7/svg/L5-ApproveVesselVisitNotification.svg)

#### Reject Vessel Visit Notification

![Sequence Diagram Level 3](Documentation/US-2.2.7/svg/L5-RejectVesselVisitNotification.svg)

#### Review Vessel Visit Notification

![Sequence Diagram Level 3](Documentation/US-2.2.7/svg/L5-ReviewVesselVisitNotification.svg)

---

### 3.2 Applied Patterns
- **Controller:** `VesselVisitNotificationController` exposes endpoints for approval and rejection, enforcing authorization for Port Authority Officers only.
- **Service Layer:** Handles business logic for approving/rejecting notifications, including dock assignment validation, status transitions, and audit field updates (officer, timestamps, notes).
- **Domain Model:** `VesselVisitNotification` entity manages state, status lifecycle, and validation logic (e.g., dock compatibility with vessel type).
- **DTOs:** Request and response models (`ApproveRequest`, `RejectRequest`, `VesselVisitNotificationDto`) ensure data integrity and validation at the API boundary.
- **Auditing:** All decisions (approval/rejection) are recorded with officer ID, timestamp, and outcome for traceability and compliance.

---

### 3.3 Acceptance Tests
- Approve notification and assign a valid dock.
- Reject notification and provide a reason.
- Verify decision record for auditing (officer, timestamp, outcome).
- Allow update and resubmission after rejection.
- Validate dock assignment against vessel type and dock capacity.

---
