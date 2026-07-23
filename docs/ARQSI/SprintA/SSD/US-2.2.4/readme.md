# US2.2.4 – Register and Update Storage Areas

## 1. Context

This feature allows the Port Authority Officer to register, update, and manage storage areas in the port. Each area includes type (e.g., yard, warehouse), descriptive location, maximum capacity (TEUs), current occupancy, and relationship with the served docks. It ensures the correct assignment of (un)loading and storage operations, supporting logistics planning.

**Related Issues:**
- Implementation: #5
- Tests: #54

---

## 2. Requirements

### User Story

> As a Port Authority Officer, I want to register and update storage areas so that (un)loading and storage operations can be assigned to the correct locations.

### Acceptance Criteria

- **AC1:** Each area has a unique identifier, type, and descriptive location.
- **AC2:** Maximum capacity (TEUs) and current occupancy must be specified.
- **AC3:** By default, serves the entire port, but can be limited to specific docks.
- **AC4:** Possibility to manually register the distance between the area and the served docks.
- **AC5:** Current occupancy cannot exceed maximum capacity.
- **AC6:** Only Port Authority Officers can create, update, or delete areas.

### Dependencies / References

- **Authentication & Authorization:** Only authorized users can manage areas.
- **Dock Management:** List of docks depends on those existing in the system.
- **Data Persistence:** Records must be reliable and support updates/removal.
- **Validation:** Occupancy ≤ maximum capacity; distances must be non-negative.

### Client Clarifications

- **Location:** Descriptive field (e.g., “North Warehouse”).
- **Dock Association:** An area can serve all or only some docks.
- **Excess Occupancy:** Operation must be rejected.

---

## 3. Design

### 3.1 Realization

#### Create Storage Area

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.4/svg/L5-CreateStorageArea.svg)

#### Update Storage Area

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.4/svg/L5-UpdateStorageArea.svg)

### 3.2 Applied Patterns

- **Controller:** `StorageAreaController` handles HTTP requests, validates models, invokes the service, and returns responses.
- **Application Service:** `StorageAreaService` encapsulates business rules and validation.
- **Domain Model:** Entity `StorageArea` represents the physical area and its attributes.
- **Repository:** `StorageAreaRepository` abstracts persistence operations (CRUD).
- **Validation:**
    - Attributes: `StorageAreaDto` ([Required], [Range])
    - Business: Application service

---

## 4. Acceptance Tests

- Register area with all required fields.
- Update occupancy without exceeding capacity.
- Associate area with specific docks and register distances.
- Reject registration/update if occupancy > capacity.
- Only authorized users can create/edit/delete.
