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

        if (result.Contains("allowed only after") || result.Contains("already recorded"))
        {
            return BadRequest(new { message = result });
        }

        return Ok(new { message = result }); 
    }
    [HttpGet("today")]
    public IActionResult GetTodayAttendance()
    {
        var records = service.GetTodayAttendance();
        return Ok(records);
    }
    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var workers = await workerService.GetAllWorkers();
        var records =  service.GetTodayAttendance();
        var excelExporter = new ExcelExporter();
        var file = excelExporter.ExportDailyAttendance(workers, records);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Attendance.xlsx");  
    }
}
