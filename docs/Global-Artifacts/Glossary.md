# Port Operations Management System — Domain Glossary

This glossary defines the key business terms and entities used within the system and documentation.

## User Roles

| Term | Definition |
|------|------------|
| **Port Authority Officer** | Administrative user responsible for port infrastructure, validations, and approval of Vessel Visit Notifications. |
| **Shipping Agent Organization** | Authorized company that handles vessel operations at the port and submits visit requests. |
| **Shipping Agent Representative** | Individual acting on behalf of an organization to create, modify, and submit Vessel Visit Notifications (VVNs). |
| **Logistics Operator** | Operational user responsible for managing staff members and physical resources at the port. |

## Vessel & Port Structure

| Term | Definition |
|------|------------|
| **Vessel Type** | Classification of vessels including structural layout and operational constraints such as capacity, rows, bays, and tiers. |
| **Vessel** | A maritime ship entering or leaving the port; uniquely identified by its IMO number. |
| **IMO Number** | International Maritime Organization identifier — 7 digits plus a check digit |
| **Dock** | Physical port structure used for vessel berthing. Defines limits such as length, depth, and max draft. |
| **Storage Area** | Designated location for temporary cargo handling (yard or warehouse), with limited capacity and possible dock constraints. |
| **Distance Mapping** | Manually entered distance between a Dock and a Storage Area to support logistics planning and optimization. |

## Vessel Visit Scheduling

| Term | Definition |
|------|------------|
| **Vessel Visit Notification (VVN)** | Formal digital request indicating vessel arrival and berthing intent; includes cargo and crew info. |
| **VVN Status** | Current state of the VVN lifecycle: `"in progress"`, `"submitted"`, `"approved"`, `"rejected"`. |
| **Approval Decision** | Action taken by Port Authority Officer to approve or reject a submitted VVN. Must include timestamp, officer ID, and reason when rejected. |
| **Assigned Dock** | Dock temporarily allocated when a VVN is approved. Can be changed later by port operations. |

## Cargo & Crew

| Term | Definition |
|------|------------|
| **Cargo Manifest** | Data describing the cargo to be unloaded or loaded during a port visit. |
| **Container** | Cargo container referencing ISO 6346 standard (validated format and check digit). |
| **Crew Member** | Person working aboard a vessel. Personal data may be required for security compliance. |

## Workforce & Equipment

| Term | Definition |
|------|------------|
| **Staff Member** | Port worker available for loading/unloading or related operations. Has mecanographic ID and status. |
| **Mecanographic Number** | Operationally unique identifier assigned to a staff member. |
| **Qualification** | Certification or skills required for operating specific equipment or performing operations. |
| **Physical Resource** | Machinery or vehicle used in port operations (e.g. cranes, trucks). |
| **Operational Capacity** | Quantified performance measure of a resource (e.g., throughput). |
| **Setup Time** | Required time before a resource becomes operational. |
| **Availability Status** | Whether a resource or staff member is active, inactive, or under maintenance. |
| **Audit Log / Decision History** | Permanent record of decisions and actions on VVNs for regulatory oversight. |
