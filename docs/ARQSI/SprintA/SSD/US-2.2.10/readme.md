# US2.2.10 – View Status of Vessel Visit Notifications

## 1. Context

This feature allows the Shipping Agent Representative to view the status of all their submitted Vessel Visit Notifications, including those in progress, pending, approved (with current dock assignment), or rejected (with reason). It ensures representatives are always informed about the decisions of the Port Authority and the current state of their requests.

**Related Issues:**
- Implementation: #10
- Tests: #60

---

## 2. Requirements

### User Story

> As a Shipping Agent Representative, I want to view the status of all my submitted Vessel Visit Notifications (in progress, pending, approved with current dock assignment, or rejected with reason), so that I am always informed about the decisions of the Port Authority.

### Acceptance Criteria

- **AC1:** The representative can view the status of all their Vessel Visit Notifications.
- **AC2:** The representative can also view notifications submitted by other representatives from the same shipping agent organization.
- **AC3:** Each notification displays its status: in progress, pending, approved (with dock assignment), or rejected (with reason).
- **AC4:** Notifications are searchable and filterable by vessel, status, representative, and time.

### Dependencies / References

- Authentication and authorization for representatives and organizations.
- Search and filter capabilities for notifications.
- Status and audit trail for each notification.

---

## 3. Design

### 3.1 Realization

#### View and Search Vessel Visit Notifications

![Sequence Diagram Level 3](Documentation/US-2.2.10/svg/L5-ViewVVNStatus.svg)

---

### 3.2 Applied Patterns

- **Controller:** `VesselVisitNotificationController` exposes endpoints for listing, searching, and filtering notifications, accessible to Shipping Agent Representatives.
- **Service Layer:** Handles business logic for search, filtering, and access control.
- **Domain Model:** Provides status, dock assignment, and rejection reason as needed.
- **DTOs:** Ensure data integrity and presentational consistency.
- **Repository:** Supports efficient queries and filtering.

---

### 3.3 Acceptance Tests

- View all notifications for the logged-in representative.
- View notifications submitted by other representatives from the same organization.
- Filter notifications by vessel, status, representative, and time.
- Display status, dock assignment (if approved), and rejection reason (if rejected).

---

