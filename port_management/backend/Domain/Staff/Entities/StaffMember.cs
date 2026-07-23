using PortManagement.Domain.PhysicalResources.ValueObjects;
using PortManagement.Domain.Staff.ValueObjects;

namespace PortManagement.Domain.Staff.Entities;

using PortManagement.Domain.Qualification.Entities;


public class StaffMember
{
    public Guid Id { get; set; }
    
    public SMId StaffMemberId { get; private set; }

    public SMName ShortName { get; private set; }
    
    public Email Email { get; private set; } 
    
    public PhoneNumber PhoneNumber { get; private set; }
    public bool IsAvailable { get; private set; }
    public SMOperationalWindow OperationalWindow { get; private set; }

    private readonly List<Guid> _qualificationIds = new();
    public IReadOnlyCollection<Guid> QualificationIds => _qualificationIds.AsReadOnly();
    
    protected StaffMember() { }
    
    public StaffMember(
        SMId staffMemberId,
        SMName shortName,
        Email email,
        PhoneNumber phoneNumber,
        SMOperationalWindow operationalWindow,
        IEnumerable<Guid>? qualificationIds = null,
        bool isAvailable = true)
    {
        Id = Guid.NewGuid();
        
        StaffMemberId = staffMemberId ?? throw new ArgumentNullException(nameof(staffMemberId));
        ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        OperationalWindow = operationalWindow ?? throw new ArgumentNullException(nameof(operationalWindow));
        IsAvailable = isAvailable;

        if (qualificationIds != null)
        {
            foreach (var qid in qualificationIds.Distinct())
                AddQualification(qid);
        }
    }
    
    public void SetAvailability(bool isAvailable) => IsAvailable = isAvailable;
    
    public void AddQualification(Guid qualificationId)
    {
        if (!_qualificationIds.Contains(qualificationId))
            _qualificationIds.Add(qualificationId);
    }
    
    public void RemoveQualification(Guid qualificationId)
    {
        if (_qualificationIds.Contains(qualificationId))
            _qualificationIds.Remove(qualificationId);
    }
    
    
}

