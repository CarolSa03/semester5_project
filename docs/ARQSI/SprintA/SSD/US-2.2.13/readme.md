# US2.2.13 – Register and Manage Qualifications

## 1. Context

This feature allows the Logistics Operator to register and manage qualifications (create, update), ensuring that staff members and resources are consistently associated with the correct skills and certifications required for port operations.

**Related Issues:**
- Implementation: #13
- Tests: #63

---

## 2. Requirements

### User Story

> As a Logistics Operator, I want to register and manage qualifications (create, update), so that staff members and resources can be consistently associated with the correct skills and certifications required for port operations.

### Acceptance Criteria

- **AC1:** Each qualification has a unique code and a descriptive name (e.g., "STS Crane Operator," "Truck Driver").
- **AC2:** Qualifications must be searchable and filterable by code or name.
- **AC3:** A qualification must exist before it can be assigned to staff members or resources.

### Dependencies / References

- Authentication and authorization for Logistics Operators.
- Search and filter capabilities for qualifications.
- Association logic for staff and resources.

---

## 3. Design

### 3.1 Realization

#### Create Qualification

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/LOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/LOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.13/svg/L5-CreateQualification.svg)

#### Update Qualification

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/LOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/LOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.13/svg/L5-UpdateQualification.svg)

#### Filter Qualification

![Sequence Diagram Level 1](Documentation/US-2.2.13/svg/L5-FilterQualificationByid.svg)

---

### 3.2 Applied Patterns

- **Controller:** `QualificationController` exposes endpoints for creating and updating qualifications, accessible to Logistics Operators.
- **Service Layer:** Handles business logic for qualification management, validation, and association.
- **Domain Model:** Manages qualification state and uniqueness.
- **DTOs:** Ensure data integrity and validation at the API boundary.
- **Repository:** Handles persistence, search, filter, and association logic.

---

### 3.3 Acceptance Tests

- Register a qualification with a unique code and descriptive name.
- Update qualification details.
- Search and filter qualifications by code or name.
- Attempt to assign a non-existent qualification to a staff member or resource (should fail).

---

