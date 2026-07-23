# US2.2.1 – Create and Update Vessel Types

## 1. Context

This feature allows a **Port Authority Officer** to define and manage **vessel types**.
By establishing a standardized classification system, this functionality ensures that all vessels are categorized consistently based on their attributes and operational constraints.

This is a foundational feature that supports the accurate registration of vessels and subsequent port operations planning.

### 1.1 List of Issues

- Implementation: #29 , #30 , #31 , #32
- Testing: #43

---

## 2. Requirements

**User Story:**
As a **Port Authority Officer**, I want to **create and update vessel types**, so that vessels can be classified consistently and their operational constraints are properly defined.

### Acceptance Criteria

* **AC1:** Vessel types must include attributes such as name, description, capacity, and operational constraints (e.g.: maximum number of rows, bays, and tiers).
* **AC2:** Vessel types must be available for reference when registering vessel records.
* **AC3:** Vessel types must be searchable and filterable by name and description.
* **AC4:** The name of a vessel type must be unique

### Dependencies / References

* **Authentication & Authorization:** Ensures only authorized Port Authority Officers can manage vessel types.
* **Vessel Module:** The creation of vessel records depends on the existence of these vessel types.
* **Data Persistence:** Vessel type records must be stored reliably in the database.

### Client Clarifications
#### 1.
Q: Good morning,
Regarding this part of the text - "The size, type, and cargo capacity of a vessel strongly influence its operational needs at the port, such as the length of dock required, the number of STS cranes that can be engaged, and the volume of containers to be handled."
We have a doubt: what is the difference between “cargo capacity” and “volume of containers”? Our perception is that they should represent distinct attributes, but we are not sure.
Thank you very much.
A: Both expressions refer to the maximum number of containers (e.g.: 18000) in the adopted unit of measurement (TEU).

#### 2. 
Q: The vessel type defines the dimensions, capacity and cargo disposition of all vessels of that type or is some variation possible within the same type? And which are the max dimensions and cargo disposition for each type?
A: 1. For simplicity, you should consider that no variation is possible.
2. Such dimensions are defined by the user, i.e., the Port Authority Officer.

Some examples of vessel types are mentioned of the system description document.
Moreover, it states that "The type of vessel determines the maximum number of rows, bays, and tiers, and therefore its maximum TEU capacity.".
#### 3.
Q: Can there be more than one vessel type with the same name?
A: No! Names are unique.
---

## 3. Design


### 3.1 Realization

#### Create Vessel Type

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.1/svg/L5-CreateVesselType.svg)

#### Update Vessel Type

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.1/svg/L5-UpdateVesselType.svg)


### 3.2 Applied Patterns

* **Controller:** A dedicated controller, `VesselTypeController`, will handle incoming HTTP requests for managing vessel types. It will be responsible for receiving data transfer objects (DTOs), invoking the appropriate application service, and returning a corresponding HTTP response.
* **Domain Model:** The core business logic and rules will be encapsulated within domain entities, with `VesselType` being the primary aggregate root. This entity will contain its attributes and enforce invariants, such as rules on its operational constraints.
* **Repository:** The `VesselTypeRepository` will abstract the persistence details. It will provide a collection-like interface for accessing `VesselType` domain objects.

### 3.3 Acceptance Tests

    // Test logic to create a vessel type with a name, description, and constraints.
    // Assert that the object is created and persisted successfully.