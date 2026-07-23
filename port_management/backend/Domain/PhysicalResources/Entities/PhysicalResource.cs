using PortManagement.Domain.PhysicalResources.Enums;
using PortManagement.Domain.PhysicalResources.ValueObjects;

namespace PortManagement.Domain.PhysicalResources.Entities;

public abstract class PhysicalResource
{
    public Guid Id { get; set; }

    public PRCode Code { get; private set; }
    public PRDescription Description { get; private set; }
    public PRArea Area { get; private set; }
    public PRStatus Status { get; private set; }
    public PRSetupTime SetupTime { get; private set; }
    public PROperationalWindow OperationalWindow { get; private set; }

    private readonly List<Guid> _requiredQualificationIds = new();
    public IReadOnlyCollection<Guid> RequiredQualificationIds => _requiredQualificationIds.AsReadOnly();

    protected PhysicalResource() { }

    protected PhysicalResource(
        Guid id,
        PRCode code,
        PRDescription description,
        PRArea area,
        PRSetupTime setupTime,
        PROperationalWindow operationalWindow,
        IEnumerable<Guid>? requiredQualificationIds = null,
        PRStatus status = PRStatus.Active)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID cannot be empty.", nameof(id));
        
        Id = id;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Area = area ?? throw new ArgumentNullException(nameof(area));
        SetupTime = setupTime ?? throw new ArgumentNullException(nameof(setupTime));
        OperationalWindow = operationalWindow ?? throw new ArgumentNullException(nameof(operationalWindow));
        Status = status;

        if (requiredQualificationIds != null)
        {
            foreach (var qid in requiredQualificationIds.Distinct())
                AddQualification(qid);
        }
    }

    protected PhysicalResource(
        PRCode code,
        PRDescription description,
        PRArea area,
        PRSetupTime setupTime,
        PROperationalWindow operationalWindow,
        IEnumerable<Guid>? requiredQualificationIds = null,
        PRStatus status = PRStatus.Active)
    {
        Id = Guid.NewGuid();

        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Area = area ?? throw new ArgumentNullException(nameof(area));

        SetupTime = setupTime ?? throw new ArgumentNullException(nameof(setupTime));
        OperationalWindow = operationalWindow ?? throw new ArgumentNullException(nameof(operationalWindow));
        Status = status;

        if (requiredQualificationIds != null)
            {
                foreach (var qid in requiredQualificationIds.Distinct())
                    AddQualification(qid);
            }
   }

    public void MarkActive() => Status = PRStatus.Active;

    public void MarkInactive() => Status = PRStatus.Inactive;

    public void MarkMaintenance() => Status = PRStatus.Maintenance;

    public void UpdateBaseFields(
        PRDescription? description = null,
        PRArea? area = null,
        PRSetupTime? setupTime = null,
        PROperationalWindow? operationalWindow = null,
        IEnumerable<Guid>? qualificationIds = null,
        PRStatus? status = null)
    {
        if (description != null) Description = description;
        if (area != null) Area = area;
        if (setupTime != null) SetupTime = setupTime;
        if (operationalWindow != null) OperationalWindow = operationalWindow;
        if (status.HasValue) Status = status.Value;

        if (qualificationIds != null)
        {
            _requiredQualificationIds.Clear();
            foreach (var qid in qualificationIds.Distinct())
                AddQualification(qid);
        }
    }

    public void AddQualification(Guid qualificationId)
    {
        if (qualificationId == Guid.Empty)
            throw new ArgumentException("Qualification ID cannot be empty.", nameof(qualificationId));

        if (_requiredQualificationIds.Contains(qualificationId))
            throw new InvalidOperationException("Qualification already linked to this resource.");

        _requiredQualificationIds.Add(qualificationId);
    }

    public void RemoveQualification(Guid qualificationId)
    {
        _requiredQualificationIds.Remove(qualificationId);
    }

    public void ClearQualifications() => _requiredQualificationIds.Clear();

    internal void SetId(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));
        Id = id;
    }
}
