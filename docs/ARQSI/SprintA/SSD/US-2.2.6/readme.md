# US2.2.6 – Register and Manage Shipping Agent Representatives

## 1. Context

This feature allows the Port Authority Officer to register, update, and deactivate representatives of a shipping agent organization, ensuring that only authorized individuals can interact with the system on behalf of their organization.

**Related Issues:**
- Implementation: #12
- Tests: #56

---

## 2. Requirements

### User Story

> As a Port Authority Officer, I want to register and manage representatives of a shipping agent organization (create, update, deactivate), so that the right individuals are authorized to interact with the system on behalf of their organization.

### Acceptance Criteria

- **AC1:** Each representative must be associated with exactly one shipping agent organization.
- **AC2:** Required representative details include name, citizen ID, nationality, email, and phone number.
- **AC3:** Representatives can be created, updated, and deactivated, but deactivation must preserve data for audit and historical purposes.

### Dependencies / References

- **Authentication & Authorization:** Only Port Authority Officers can manage representatives.
- **Data Persistence:** Representative and organization data must be stored reliably.
- **Notification System:** Email and phone are used for notifications and authentication.

### Client Clarifications

- **Association:** A representative cannot exist without being linked to a shipping agent organization.
- **Data Preservation:** Deactivation does not delete data; it preserves it for audit and planning.

---

## 3. Design

### 3.1 Realization

#### Register Shipping Agent Representative

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.6/svg/L5-CreateRepresentative.svg)

#### Update/Deactivate Shipping Agent Representative

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.6/svg/L5-UpdateDeactivateRepresentative.svg)


---

### 3.2 Applied Patterns

- **Controller:** `ShippingAgentRepresentativeController` handles HTTP requests for creating, updating, and deactivating representatives.
- **Application Service:** `ShippingAgentRepresentativeService` encapsulates business rules and validation.
- **Domain Model:** Entity `ShippingAgentRepresentative` represents the representative and its attributes.
- **Repository:** `ShippingAgentRepresentativeRepository` abstracts persistence operations (CRUD).
- **Validation:**
    - Attributes: `ShippingAgentRepresentativeDto` ([Required], [Email], [Phone])
    - Business: Application service

---

## 4. Acceptance Tests

- Register a representative with all required fields and association to an organization.
- Update representative details.
- Deactivate a representative and ensure data is preserved.
- Only authorized users can manage representatives.


