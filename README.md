# Port Operations Management System

A .NET 8 Web API that digitizes port operations and supports the lifecycle of Vessel Visit Notifications (VVNs). The system ensures that port infrastructure, resources, and personnel are properly aligned with vessel scheduling and operational needs.

## Main Actors

These actors interact with the system based on organizational responsibilities:

| Actor | Responsibilities |
|-------|-----------------|
| Port Authority Officer | Maintains master data (vessels, vessel types, docks, storage areas) and approves/rejects VVNs. |
| Shipping Agent Representative | Creates, updates, and submits VVNs including cargo and crew information. |
| Logistics Operator | Manages operational staff and physical resources for port operations. |

## System Features

### Port Authority Capabilities
- Manage vessel types and structural constraints
- Register vessels with IMO checksum validation
- Configure docks with berthing limitations
- Maintain storage areas and capacity constraints
- Review, approve, or reject VVNs with audit history
- Assign temporary docking locations upon approval

### Shipping Agent Representative Capabilities
- Create VVNs in an "in progress" state
- Update draft VVNs with cargo manifest and crew data
- Validate container identifiers with ISO 6346 rules
- Submit completed VVNs for Port Authority approval
- Track VVN status for the entire organization

### Logistics Operator Capabilities
- Register and maintain operational staff members
- Track staff availability and qualifications (descriptive text)
- Register physical resources such as cranes and trucks
- Manage operational readiness, capacity, and status

## Architecture Overview

- .NET 8 Web API
- RESTful application design
- Entity Framework Core for data persistence
- Layered design using Controllers, Services, and Repositories
- DTO-based communication between API and clients
- Swagger UI included for built-in API testing

## How to Run the Program

````
dotnet restore
dotnet build
dotnet run --project ./PortManagement/PortManagement.csproj
````

## How to Run the Tests

````
dotnet restore
dotnet build
dotnet test
````

Needs Update :)
