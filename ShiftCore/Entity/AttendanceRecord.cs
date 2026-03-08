using ShiftCore.Entity.Common;

namespace ShiftCore.Entity;

public class AttendanceRecord : BaseEntity
{
    public Guid WorkerId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
}
