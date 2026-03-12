using ShiftCore.Entity;
using System.Text.Json;

namespace ShiftCore.Services;

public class AttendanceService
{
    private string GetFilePath()
    {
        var today = DateTime.Today.ToString("yyyy_MM_dd");
        return  Path.Combine(Directory.GetCurrentDirectory(), "Data", $"attendance_{today}.json");

    }
    private List<AttendanceRecord> ReadAttendance()
    {
        var filePath = GetFilePath();
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<AttendanceRecord>>(json)
            ?? new List<AttendanceRecord>();
    }
    private void SaveAttendance(List<AttendanceRecord> records)
    {
        var path = GetFilePath();
        var json = JsonSerializer.Serialize(records, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(path, json);
    }
}
