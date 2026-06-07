using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Microsoft.AspNetCore.Authorization;

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
        var records = service.GetTodayAttendance();
        var excelExporter = new ExcelExporter();
        var file = excelExporter.ExportDailyAttendance(workers, records);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Attendance.xlsx");
    }
    [Authorize]
    [HttpGet("export/salary")]
    public async Task<IActionResult> ExportSalary([FromQuery] int year, [FromQuery] int month)
    {
        var workers = await workerService.GetAllWorkers();
        var monthlyRecords = service.GetMonthlyAttendance(year, month);
        var excelExporter = new ExcelExporter();
        var file = excelExporter.ExportMonthlySalaryReport(workers, monthlyRecords, year, month);
        string fileName = $"Maaş_hesabatı_{year}_{month:D2}.xlsx";
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
