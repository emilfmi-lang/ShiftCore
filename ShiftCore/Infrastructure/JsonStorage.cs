using System.Text.Json;

namespace ShiftCore.Infrastructure;

public class JsonStorage
{
    public List<T> Read<T>(string path)
    {
        if(!File.Exists(path))
        {
            File.WriteAllText(path, "[]");
        }
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }
    public void Write<T>(string path, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
