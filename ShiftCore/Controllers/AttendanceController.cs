using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftCore.Services;

namespace ShiftCore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController(AttendanceService service) : ControllerBase
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
}
