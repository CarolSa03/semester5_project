using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Domain.Vessel.Enums;

namespace PortManagement.Domain.Vessel.Entities
{
    // Represents a vessel registered in the port system
    public class VesselRecord
    {
        public Guid Id { get; set; }                               // Database primary key
        public Imo Imo { get; private set; }                       // IMO number (unique business identifier)
        public VRName Name { get; private set; }                   // Name of the vessel
        public VesselType VesselType { get; private set; } = null!; // Type of the vessel
        public VROwner Owner { get; private set; }                 // Owner or operator of the vessel
        public VRStatus Status { get; private set; }               // Status of the vessel record
        public DateTime CreatedAt { get; private set; }            // When the vessel record was created
        public DateTime? UpdatedAt { get; private set; }           // When the vessel record was last updated

        protected VesselRecord()
        {
            Imo = default!;
            Name = default!;
            Owner = default!;
        }

        public VesselRecord(
            Imo imo,
            VRName name,
            VesselType vesselType,
            VROwner owner,
            VRStatus status = VRStatus.Active)
        {
            Id = Guid.NewGuid();
            Imo = imo;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            VesselType = vesselType ?? throw new ArgumentNullException(nameof(vesselType));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void MarkActive()
        {
            Status = VRStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkInactive()
        {
            Status = VRStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateFields(
            VRName? name = null,
            VesselType? vesselType = null,
            VROwner? owner = null,
            VRStatus? status = null)
        {
            if (name != null) Name = name;
            if (vesselType != null) VesselType = vesselType;
            if (owner != null) Owner = owner;
            if (status.HasValue) Status = status.Value;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Validate()
        {
            // Value objects already validate themselves upon construction
            // This method ensures entity-level invariants are met
            if (VesselType == null)
                throw new ArgumentException("VesselType is required.");
        }

        internal void SetId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID cannot be empty.", nameof(id));
            Id = id;
        }
    }
}
