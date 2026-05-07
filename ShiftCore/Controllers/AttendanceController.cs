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
        var response = service.RegisterAttendance(workerId);
        if (response.IsSuccess)
        {
            return Ok(response);
        } 
        return BadRequest(response);
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
