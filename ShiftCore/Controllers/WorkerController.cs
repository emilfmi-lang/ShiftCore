using Microsoft.AspNetCore.Mvc;
using ShiftCore.Dtos;
using ShiftCore.Infrastructure;
using ShiftCore.Services;

namespace ShiftCore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkerController(WorkerService service, ExcelExporter excelExporter) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllWorkers()
    {
        var workers = await service.GetAllWorkers();
        return Ok(workers);
    }
    [HttpPost]
    public async Task<IActionResult> Add(CreateWorkerDto dto)
    {
        var worker = await service.AddWorkerAsync(dto);
        return Ok(worker);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await service.DeactivateWorker(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var workers = await service.GetAllWorkers();
        var file = excelExporter.ExportWorkers(workers);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Workers.xlsx");
    }
    [HttpDelete("delete/{workerId}")]
    public async Task<IActionResult> DeleteWorker(Guid workerId)
    {
        await service.DeleteWorkerAsync(workerId);
        return NoContent();
    }
}