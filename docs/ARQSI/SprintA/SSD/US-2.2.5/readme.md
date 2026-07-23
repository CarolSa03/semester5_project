# US2.2.5 – Register Shipping Agent Organizations

## 1. Context

This feature allows the Port Authority Officer to register new shipping agent organizations, enabling them to operate within the port’s digital system.

**Related Issues:**
- Implementation: #6
- Tests: #55

---

## 2. Requirements

### User Story

> As a Port Authority Officer, I want to register new shipping agent organizations, so that they can operate within the port’s digital system.

### Acceptance Criteria

- **AC1:** Each organization must have at least an identifier, legal and alternative names, an address, and its tax number.
- **AC2:** Each organization must include at least one representative at the time of registration.
- **AC3:** Representatives must be registered with name, citizen ID, nationality, email, and phone number.
- **AC4:** Email and phone number are used for system notifications, including approval decisions and the authentication process.

### Dependencies / References

- **Authentication & Authorization:** Only Port Authority Officers can register organizations.
- **Data Persistence:** Organization and representative data must be stored reliably.
- **Notification System:** Email and phone are used for notifications and authentication.

### Client Clarifications

- **Minimum Data:** All fields are mandatory for both organizations and representatives at registration.
- **Notifications:** Representatives will receive system notifications via email and phone.

---

## 3. Design

### 3.1 Realization

#### Register Shipping Agent Organization

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.5/svg/L5-CreateShippingAgentOrganization.svg)

### 3.2 Applied Patterns

- **Controller:** `ShippingAgentOrganizationController` handles HTTP requests, validates models, invokes the service, and returns responses.
- **Application Service:** `ShippingAgentOrganizationService` encapsulates business rules and validation.
- **Domain Model:** Entity `ShippingAgentOrganization` represents the organization and its attributes.
- **Repository:** `ShippingAgentOrganizationRepository` abstracts persistence operations (CRUD).
- **Validation:**
    - Attributes: `ShippingAgentOrganizationDto`, `ShippingAgentRepresentativeDto` ([Required], [Email], [Phone])
    - Business: Application service

---

## 4. Acceptance Tests

- Register an organization with all required fields and at least one representative.
- Reject registration if any required field is missing.
- Ensure representatives receive notifications via email and phone.
- Only authorized users can register organizations.
