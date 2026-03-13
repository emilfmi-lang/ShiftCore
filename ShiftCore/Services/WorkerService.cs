using ShiftCore.Entity;
using ShiftCore.Infrastructure;
using System.Text.Json;

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
    public List<Worker> GetAllWorkers()
    {
        return _storage.Read<Worker>(_filePath);
    }
    public Worker AddWorker(string fullName, string role)
    {

        var workers = _storage.Read<Worker>(_filePath);
        if (workers.Any(x => x.FullName.ToLower() == fullName.ToLower()))
            throw new Exception("Worker already exists.");
        var worker = new Worker
        {
            FullName = fullName,
            Role = role,
        };
        workers.Add(worker);
        _storage.Write(_filePath, workers);
        return worker;
    }
    public List<Worker> GetAllActiveWorkers() =>
                        _storage.Read<Worker>(_filePath).Where(x => x.IsActive).ToList();
    public bool DeactivateWorker(Guid id)
    {
        var workers = _storage.Read<Worker>(_filePath);
        var worker = workers.FirstOrDefault(x => x.Id == id);
        if (worker == null)
            return false;
        worker.IsActive = false;
        _storage.Write(_filePath, workers);
        return true;
    }

    public Worker? GetWorkerById(Guid id)
    {
        var workers = _storage.Read<Worker>(_filePath);
        return workers.FirstOrDefault(x => x.Id == id);
    }
    public List<Worker> DateWorker(DateTime startDate , DateTime endDate)
    {
        List<Worker> workers = _storage.Read<Worker>(_filePath);
        return workers.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();
    }
}
