namespace ShiftCore.Entity.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
