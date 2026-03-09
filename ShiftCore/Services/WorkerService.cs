using ShiftCore.Entity;
using System.Text.Json;

namespace ShiftCore.Services;

public class WorkerService
{
    private readonly string _filePath;
    public WorkerService()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "workers.json");
        if(!File.Exists(_filePath))
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
    public Worker AddWorker(string fullName,  string role)
    {
        var workers = ReadWorkers();
        var worker = new Worker
        {
            FullName = fullName,
            Role = role,
        };
        workers.Add(worker);
        SaveWorkers(workers);
        return worker;
    }
}
