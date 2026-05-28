using ShiftCore.Entity.Common;

namespace ShiftCore.Entity;

public class Admin : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
