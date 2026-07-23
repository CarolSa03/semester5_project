# US2.2.11 – Register and Manage Operating Staff Members

## 1. Context

This feature allows the Logistics Operator to register, update, and manage operating staff members. It ensures the system accurately reflects staff availability and that only qualified personnel are assigned to resources during scheduling.

**Related Issues:**
- Implementation: #11
- Tests: #61

---

## 2. Requirements

### User Story

> As a Logistics Operator, I want to register and manage operating staff members (create, update, deactivate), so that the system can accurately reflect staff availability and ensure that only qualified personnel are assigned to resources during scheduling.

### Acceptance Criteria

- **AC1:** Each staff member must have a unique mecanographic number (ID), short name, contact details (email, phone), qualifications, operational window, and current status (e.g., available, unavailable).
- **AC2:** Deactivation/reactivation must not delete staff data but preserve it for audit and historical planning purposes.
- **AC3:** Staff members must be searchable and filterable by id, name, status, and qualifications.

### Dependencies / References

- Authentication and authorization for Logistics Operators.
- Audit trail for staff data changes.
- Search and filter capabilities for staff members.

---

## 3. Design

### 3.1 Realization

#### Create Staff Member

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/LOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/LOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.11/svg/L5-CreateStaffMember.svg)

#### Update Staff Member

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/LOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/LOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.11/svg/L5-UpdateStaffMember.svg)

#### Deactivate/Reactivate Staff Member

![Sequence Diagram Level 3](Documentation/US-2.2.11/svg/L5-ActivateDeactivateStaffMember.svg)

---

### 3.2 Applied Patterns

- **Controller:** `StaffMemberController` exposes endpoints for creating, updating, deactivating, and reactivating staff members, accessible to Logistics Operators.
- **Service Layer:** Handles business logic for staff management, validation, and auditing.
- **Domain Model:** Manages staff state, operational window, and qualifications.
- **DTOs:** Ensure data integrity and validation at the API boundary.
- **Repository:** Handles persistence, search, filter, and audit trail.

---

### 3.3 Acceptance Tests

- Register a staff member with all required fields.
- Update staff member details.
- Deactivate and reactivate a staff member, ensuring data is preserved.
- Search and filter staff by id, name, status, and qualifications.
- Verify audit records for all changes.

---

