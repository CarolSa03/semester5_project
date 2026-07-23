# US2.2.3 – Register and Update Docks

## 1. Context

This feature enables a **Port Authority Officer** to **register, update, and manage docks** within the port.  
Each dock record represents a physical docking facility, including its name, location, and physical dimensions such as length, depth, and maximum draft.

This functionality ensures that the port’s **docking capacity** is accurately represented in the system and supports efficient vessel allocation and operational planning.

### 1.1 List of Issues

- Implementation: #4
- Testing: #53

---

## 2. Requirements

**User Story:**  
As a **Port Authority Officer**, I want to **register and update docks**, so that the system accurately reflects the **docking capacity** of the port.

### Acceptance Criteria

* **AC1:** A dock record must include a unique identifier, name/number, location, and physical characteristics (length, depth, max draft).
* **AC2:** The officer must specify the **types of vessels allowed** to berth at each dock.
* **AC3:** Docks must be **searchable and filterable** by name, vessel type, and location.
* **AC4:** The **name of each dock must be unique**.
* **AC5:** Only **Port Authority Officers** can create, update, or delete docks.

### Dependencies / References

* **Authentication & Authorization:** Ensures that only authorized users (Port Authority Officers) can perform dock management operations.
* **Vessel Type Module:** The list of allowed vessel types for each dock depends on existing vessel types defined in the system.
* **Data Persistence:** Dock records must be stored reliably in the database, supporting updates and soft deletion.
* **Search Functionality:** Supports filtering by name, location, and vessel type for operational use.

### Client Clarifications

#### 1.
**Q:** When registering a dock, do we need to specify its exact position within the port area or just a descriptive location?  
**A:** The **location is descriptive** (e.g., “North Terminal”), not a geospatial coordinate.

#### 2.
**Q:** What happens if a dock’s physical parameters (depth, draft) change over time?  
**A:** Officers can **update the dock record** to reflect these changes, ensuring data accuracy.

#### 3.
**Q:** Can more than one dock have the same name?  
**A:** **No.** Dock names must be unique within the port.

---

## 3. Design

### 3.1 Realization

#### Create Dock

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericCreateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.3/svg/L5-CreateDock.svg)

#### Update Dock

![Sequence Diagram Level 1](Documentation/Global-Artifacts/level1/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 2](Documentation/Global-Artifacts/level2/Images/PAOGenericUpdateSD.svg)

![Sequence Diagram Level 3](Documentation/US-2.2.3/svg/L5-UpdateDock.svg)

---

### 3.2 Applied Patterns

* **Controller:**  
  A dedicated controller, `DockController`, handles HTTP requests for dock management. It validates the request model, invokes the application service (`DockService`), and returns the corresponding HTTP response (Created, OK, NotFound, etc.).

* **Application Service:**  
  The `DockService` encapsulates all business rules and validation logic for dock management, ensuring data integrity (e.g., `MaxDraft ≤ Depth`, `Name` uniqueness, allowed vessel types exist).

* **Domain Model:**  
  The `Dock` entity represents the physical dock. It includes attributes such as name, location, length, depth, max draft, and allowed vessel types. It also supports soft deletion via the `IsActive` flag.

* **Repository:**  
  The `DockRepository` abstracts data persistence operations (CRUD) for docks. It implements the interface `IDockRepository`, providing methods like `GetAllAsync`, `GetByNameAsync`, and `SearchAsync`.

* **Validation Pattern:**  
  Attribute-based validation is performed in `DockDto` (e.g., `[Required]`, `[Range]`), while deeper domain-level validation occurs in `DockService.ValidateDock()` to ensure consistency across layers.

---

### 3.3 Acceptance Tests


