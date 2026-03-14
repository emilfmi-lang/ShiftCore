using ShiftCore.Entity.Common;

namespace ShiftCore.Entity;

public class Worker : BaseEntity
{
    public string FullName { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
}
