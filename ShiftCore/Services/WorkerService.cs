using ShiftCore.Entity;
using System.Text.Json;

namespace ShiftCore.Services;

public class WorkerService
{
    private readonly string _filePath;
    public WorkerService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "workers.json");
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }
    private List<Worker> ReadWorkers()
    {
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Worker>>(json) ?? new List<Worker>();
    }
    private void SaveWorkers(List<Worker> workers)
    {
        var json = JsonSerializer.Serialize(workers, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
    public List<Worker> GetAllWorkers()
    {
        return ReadWorkers();
    }
    public Worker AddWorker(string fullName, string role)
    {

        var workers = ReadWorkers();
        if (workers.Any(x => x.FullName.ToLower() == fullName.ToLower()))
            throw new Exception("Worker already exists.");
        var worker = new Worker
        {
            FullName = fullName,
            Role = role,
        };
        workers.Add(worker);
        SaveWorkers(workers);
        return worker;
    }
    public List<Worker> GetAllActiveWorkers() =>
                        ReadWorkers().Where(x => x.IsActive).ToList();
    public bool DeactivateWorker(Guid id)
    {
        var workers = ReadWorkers();
        var worker = workers.FirstOrDefault(x => x.Id == id);
        if (worker == null)
            return false;
        worker.IsActive = false;
        SaveWorkers(workers);
        return true;
    }

    public Worker? GetWorkerById(Guid id)
    {
        var workers = ReadWorkers();
        return workers.FirstOrDefault(x => x.Id == id);
    }
    public List<Worker> DateWorker(DateTime startDate , DateTime endDate)
    {
        List<Worker> workers = ReadWorkers();
        return workers.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();
    }
}
