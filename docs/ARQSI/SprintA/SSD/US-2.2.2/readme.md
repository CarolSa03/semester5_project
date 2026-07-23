# US2.2.2 – Register and Update Vessel Records

## 1. Context

This feature enables a **Port Authority Officer** to create and maintain a registry of **vessel records**.
It ensures that the system contains accurate and validated information for each vessel, which is essential for them to be referenced in subsequent operational processes, such as visit notifications.

The feature is fundamental for port operations, as it establishes a reliable database of vessels, validating their official identifiers and classifying them according to their type and operator.

### 1.1 List of Issues

- Implementation: #33 , #34 , #35
- Testing: #44

---

## 2. Requirements

**User Story:**
As a **Port Authority Officer**, I want to **register and update vessel records**, so that valid vessels can be referenced in visit notifications.

### Acceptance Criteria

* **AC1:** Each vessel record must include key attributes such as IMO number, vessel name, vessel type and operator/owner.
* **AC2:** The system must validate that the IMO number follows the official format (seven digits with a check digit), otherwise reject it.
* **AC3:** Vessel records must be searchable by IMO number, name, or operator.

### Dependencies / References

* **Authentication & Authorization:** Ensures only authorized Port Authority Officers can manage vessel records.
* **Vessel Type Module:** Vessel records are dependent on a pre-existing list of vessel types for classification.
* **Vessel Visit Notification Module:** A valid vessel record is a prerequisite for creating a visit notification.
* **Data Validation Service:** An external or internal service to validate the format and check digit of the IMO number.
* **Data Persistence:** Vessel records must be stored reliably in the database.

---

## 3. Design

### 3.1 Realization

#### Create Vessel Record

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.2/svg/L5-CreateVessel.svg)

#### Update Vessel Record

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.2/svg/L5-UpdateVessel.svg)

### 3.2 Applied Patterns

* **Controller:** A dedicated controller `VesselController` will handle incoming HTTP requests related to vessel management. It will be responsible for receiving data transfer objects (DTOs), invoking the appropriate application service, and returning a corresponding HTTP response.
* **Domain Model:** The core business logic and rules will be encapsulated within domain entities, with `Vessel` being the primary aggregate root. This entity will contain its attributes and enforce invariants, such as the structural validity of the IMO number.
* **Repository:** The `VesselRepository` will abstract the persistence details. It will provide a collection-like interface for accessing `Vessel` domain objects.

### 3.3 Acceptance Tests

Include the main test cases validating each acceptance criterion.

**Test:** *Verify that the system successfully creates a vessel record with valid data.*
**Related Acceptance Criteria:** AC1
