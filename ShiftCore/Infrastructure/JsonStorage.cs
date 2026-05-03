using System.Text.Json;

namespace ShiftCore.Infrastructure;

public class JsonStorage
{
    public async Task<List<T>> Read<T>(string path)
    {
        if(!File.Exists(path))
        {
            await File.WriteAllTextAsync(path, "[]");
        }
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }
    public async Task Write<T>(string path, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }
}
