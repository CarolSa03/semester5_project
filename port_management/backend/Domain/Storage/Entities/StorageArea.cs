using System.ComponentModel.DataAnnotations.Schema;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Storage.ValueObjects;
using PortManagement.Domain.Storage.Enums;

namespace PortManagement.Domain.Storage.Entities
{
    // Represents a storage area within the port
    public class StorageArea
    {
        public Guid Id { get; set; }                            // Database primary key
        public SAId BusinessId { get; private set; }            // Business identifier (SA-XXX)
        public SAType Type { get; private set; }                // Type of storage (yard, warehouse)
        public SALocation Location { get; private set; }        // Physical location in the port
        public SACapacity MaxCapacity { get; private set; }     // Maximum capacity in TEU
        public SACapacity CurrentCapacity { get; private set; } // Current occupancy in TEU
        public List<Dock>? ServedDocks { get; private set; }    // List of docks served by this storage area
        public SANotes? Notes { get; private set; }             // Additional notes or comments

        [NotMapped]
        public Dictionary<int, double>? DockDistances { get; set; } // Distance mapping (not persisted)

        protected StorageArea()
        {
            BusinessId = default!;
            Location = default!;
            MaxCapacity = default!;
            CurrentCapacity = default!;
        }

        public StorageArea(
            SAId businessId,
            SAType type,
            SALocation location,
            SACapacity maxCapacity,
            SACapacity currentCapacity,
            SANotes? notes = null,
            List<Dock>? servedDocks = null)
        {
            Id = Guid.NewGuid();
            BusinessId = businessId ?? throw new ArgumentNullException(nameof(businessId));
            Type = type;
            Location = location ?? throw new ArgumentNullException(nameof(location));
            MaxCapacity = maxCapacity ?? throw new ArgumentNullException(nameof(maxCapacity));
            CurrentCapacity = currentCapacity ?? throw new ArgumentNullException(nameof(currentCapacity));
            Notes = notes;
            ServedDocks = servedDocks ?? new List<Dock>();

            // Validate capacity constraint
            if (currentCapacity.Value > maxCapacity.Value)
                throw new ArgumentException("Current capacity cannot exceed max capacity");
        }

        public void UpdateCapacity(SACapacity newCurrentCapacity)
        {
            if (newCurrentCapacity == null)
                throw new ArgumentNullException(nameof(newCurrentCapacity));

            if (newCurrentCapacity.Value > MaxCapacity.Value)
                throw new ArgumentException("Current capacity cannot exceed max capacity");

            CurrentCapacity = newCurrentCapacity;
        }

        public void AddCapacity(int teu)
        {
            if (teu < 0)
                throw new ArgumentException("Cannot add negative capacity");

            var newCapacity = CurrentCapacity.Value + teu;
            UpdateCapacity(new SACapacity(newCapacity));
        }

        public void RemoveCapacity(int teu)
        {
            if (teu < 0)
                throw new ArgumentException("Cannot remove negative capacity");

            var newCapacity = CurrentCapacity.Value - teu;
            if (newCapacity < 0)
                throw new ArgumentException("Cannot remove more capacity than available");

            UpdateCapacity(new SACapacity(newCapacity));
        }

        public void UpdateFields(
            SAType? type = null,
            SALocation? location = null,
            SACapacity? maxCapacity = null,
            SANotes? notes = null)
        {
            if (type.HasValue) Type = type.Value;
            if (location != null) Location = location;
            if (notes != null) Notes = notes;

            if (maxCapacity != null)
            {
                if (CurrentCapacity.Value > maxCapacity.Value)
                    throw new ArgumentException("Cannot set max capacity below current capacity");
                MaxCapacity = maxCapacity;
            }
        }

        public void AddServedDock(Dock dock)
        {
            if (dock == null)
                throw new ArgumentNullException(nameof(dock));

            ServedDocks ??= new List<Dock>();

            if (ServedDocks.Any(d => d.Id == dock.Id))
                throw new InvalidOperationException("Dock already served by this storage area");

            ServedDocks.Add(dock);
        }

        public void RemoveServedDock(Guid dockId)
        {
            if (ServedDocks == null) return;

            var dock = ServedDocks.FirstOrDefault(d => d.Id == dockId);
            if (dock != null)
                ServedDocks.Remove(dock);
        }

        public void Validate()
        {
            // Value objects already validate themselves upon construction
            // This method ensures entity-level invariants are met
            if (CurrentCapacity.Value > MaxCapacity.Value)
                throw new ArgumentException("Current capacity cannot exceed max capacity");
        }

        internal void SetId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty.", nameof(id));
            Id = id;
        }
    }
}
