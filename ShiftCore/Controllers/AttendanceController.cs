using Microsoft.AspNetCore.Mvc;
using ShiftCore.Infrastructure;
using ShiftCore.Services;

namespace ShiftCore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController(AttendanceService service,
                                  WorkerService workerService) : ControllerBase
{
    [HttpPost("{workerId}")]
    public IActionResult RegisterAttendance(Guid workerId)
    {
        var result = service.RegisterAttendance(workerId);
        return Ok(result);
    }
    [HttpGet("today")]
    public IActionResult GetTodayAttendance()
    {
        var records = service.GetTodayAttendance();
        return Ok(records);
    }
    [HttpGet("export")]
    public IActionResult Export()
    {
        var workers = workerService.GetAllWorkers();
        var records = service.GetTodayAttendance();
        var exporter = new ExcelExporter();
        var file = exporter.ExportDailyAttendance(workers, records);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Attendance.xlsx");  
    }
}
