using ShiftCore.Dtos;
using ShiftCore.Entity;
using ShiftCore.Infrastructure;
using ShiftCore.Mapping;

namespace ShiftCore.Services;

public class WorkerService
{
    private readonly string _filePath;
    private readonly JsonStorage _storage;
    public WorkerService(JsonStorage storage)
    {
        _storage = storage;
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "workers.json");
    }
    public async Task<List<Worker>> GetAllWorkers() => await _storage.Read<Worker>(_filePath);
    
    public async Task<Worker> AddWorkerAsync(CreateWorkerDto dto)
    {
        var workers = await _storage.Read<Worker>(_filePath);
        var newWorker = dto.ToEntity();
        workers.Add(newWorker);
        await _storage.Write(_filePath, workers);
        return newWorker;
    }
    public async Task<List<Worker>> GetAllActiveWorkers()
    {
        var workers = await _storage.Read<Worker>(_filePath);
        return workers.Where(x => x.IsActive).ToList(); 
    }
    public async Task<bool> DeactivateWorker(Guid id)
    {
        var workers = await _storage.Read<Worker>(_filePath);
        var worker = workers.FirstOrDefault(x => x.Id == id);
        if (worker == null)
            return false;
        worker.IsActive = false;
        await _storage.Write(_filePath, workers);
        return true;
    }

    public async Task<Worker?> GetWorkerById(Guid id)
    {
        var workers = await _storage.Read<Worker>(_filePath);
        return workers.FirstOrDefault(x => x.Id == id);
    }
    public async Task<List<Worker>> DateWorker(DateTime startDate, DateTime endDate)
    {
        var workers = await  _storage.Read<Worker>(_filePath);
        return workers.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();
    }
    public async Task DeleteWorkerAsync(Guid workerId)
    {
        var workers = await _storage.Read<Worker>(_filePath);
        var workerToDelete = workers.FirstOrDefault(w => w.Id == workerId);
        if (workerToDelete != null)
        {
            workers.Remove(workerToDelete);
            await _storage.Write(_filePath, workers);
        }
    }
}
