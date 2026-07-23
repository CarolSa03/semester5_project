using PortManagement.Domain.Qualification.ValueObjects;

namespace PortManagement.Domain.Qualification.Entities;

public class Qualification
{
    public Guid Id { get; set; }
    public QCode Code { get; private set; }
    public QDescription Description { get; private set; }
    
    protected Qualification () { }

    public Qualification(string code, string description)
    {
        Id = Guid.NewGuid();
        Code = new QCode(code);
        Description = new QDescription(description);
    }

    public void Update(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description))
            Description = description.Trim();
    }

    public override string ToString() => $"{Code} - {Description}";
}
