using Microsoft.AspNetCore.Mvc;
using ShiftCore.Infrastructure;
using ShiftCore.Services;

namespace ShiftCore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkerController(WorkerService service, ExcelExporter excelExporter) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllWorkers()
    {
        var workers = service.GetAllWorkers();
        return Ok(workers);
    }
    [HttpPost]
    public IActionResult Add(string fullName, string role)
    {
        var worker = service.AddWorker(fullName, role);
        return Ok(worker);
    }
        [HttpDelete("{id}")]
        public IActionResult Deactivate(Guid id)
        {
            var result = service.DeactivateWorker(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("export")]
        public IActionResult Export()
        {
            var workers = service.GetAllWorkers();
            var file = excelExporter.ExportWorkers(workers);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Workers.xlsx");
        }
    }