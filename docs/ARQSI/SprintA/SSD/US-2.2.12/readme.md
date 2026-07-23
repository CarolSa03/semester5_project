# US-2.2.12 – Register and Manage Physical Resources

## 1. Context

This feature allows a Logistics Operator to register, update, and deactivate physical resources so that they can be accurately considered during planning and scheduling operations.

### List of Issues
- Documentation: [#64](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DI-04/issues/64)
- Implementation: [#65](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DI-04/issues/65), [#77](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DI-04/issues/77), [#78](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DI-04/issues/78)
- Testing: [#66](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DI-04/issues/66)

---

## 2. Requirements

**User Story:**
As a Logistics Operator, I want to register and manage physical resources (create, update, deactivate), so that they can be accurately considered during planning and scheduling operations.

### Acceptance Criteria
- **AC1:** Each resource must have a unique alpha-numeric code and a description.
- **AC2:** Each resource must store its operational capacity, which varies according to the kind of resource.
- **AC3:** Each resource must store its assigned area, if any.
- **AC4:** Each resource must also include:
    - current availability status (active, inactive, under maintenance);
    - setup time (in minutes), if relevant, before starting operations;
    - (staff) qualification requirements, ensuring only properly certified staff can be scheduled with the resource;
- **AC5:** Resources include cranes (fixed and mobile), trucks, and other equipment directly involved in vessel and yard operations.
- **AC6:** Deactivation/reactivation must not delete resource data but preserve it for audit and historical planning purposes.
- **AC7:** Resources must be searchable and filterable by code, description, kind of resource, and status.

---

### Dependencies / References
- **Authentication & Authorization:** Only authorized Logistics Operators can register and manage physical resources.
- **Data Persistence:** Physical Resources data must be stored reliably in the database.

---

### Client Clarifications
- [Code and Description lengths for Qualification](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=912): Code field should have a max of 15 chars, and Description field should have a min of 2 words, and a max of 150 chars.
- [Code and Description lengths for Physical Resource](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=1303): Code field should have a max of 20 chars, and Description field should have a max of 255 chars.
- [Status of created Physical Resource](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=1048): By default, Physical Resources can be set as available.
- [Duration and Times definitions](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=1051): Duration should be in seconds internally, but should be presented in a clear format. ETA and ETD are moments, so should be timestamps.
- [Updatable fields when managing a Physical Resource](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=1094): All can be updated except the code, which is unique.
- [Search and Filter best approach](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=1131): The best approach is returning partial matches with case insensitive. Although, refining search with operators (Equals, Contains, ...) would be nice.
- [Deactivated Resources](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=1110): Deactivated Resources can be activated and deactivated multiple times. When deactivating, a reason must be provided.
- [Resource Qualification Specific](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=1147): The Qualifications needed should be required by each Resource, instead of each type of Resource.
- [Mobility Cranes Services](https://moodle.isep.ipp.pt/mod/forum/discuss.php?d=710): Mobility Cranes are capable of serving all yards. Trucks have port-wide mobility.

---

## 3. Design

### 3.1 Realization

#### Create Physical Resource

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/LOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/LOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.12/svg/L5-CreatePhysicalResource.svg)

#### Update Physical Resource

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/LOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/LOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.12/svg/L5-UpdateDeactivatePhysicalResource.svg)

---

### 3.2 Applied Patterns
- **Controller:** `PhysicalResourceController` exposes endpoints for creating, updating, deactivating, and reactivating resources, accessible to Logistics Operators.
- **Service Layer:** Handles business logic for resource management, validation, and auditing.
- **Domain Model:** Manages resource state, operational capacity, and qualifications.
- **DTOs:** Ensure data integrity and validation at the API boundary.
- **Repository:** Handles persistence, search, filter, and audit trail.

---

### 3.3 Acceptance Tests
- Register a physical resource with all required fields.
- Update resource details (except code).
- Deactivate and reactivate a resource, ensuring data is preserved and a reason is required for deactivation.
- Search and filter resources by code, description, kind, and status.
- Verify audit records for all changes.

---
