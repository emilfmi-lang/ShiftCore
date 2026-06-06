using ShiftCore.Entity.Common;

namespace ShiftCore.Entity;

public class Worker : BaseEntity
{
    public string FullName { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal BaseSalary { get; set; }
    public int MonthlyNormativeHours { get; set; } = 200;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
}
