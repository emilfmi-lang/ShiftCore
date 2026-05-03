using ShiftCore.Entity;
using System.Text.Json;

namespace ShiftCore.Services;

public class AttendanceService
{
    private string GetFilePath()
    {
        var today = DateTime.UtcNow.ToString("yyyy_MM_dd");
        return Path.Combine(Directory.GetCurrentDirectory(), "Data", $"attendance_{today}.json");

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
    public string RegisterAttendance(Guid workerId)
    {
        var records = ReadAttendance();
        var todayRecord = records.FirstOrDefault(r => r.WorkerId == workerId &&
                                                 r.Date.Date == DateTime.UtcNow.Date);
        if (todayRecord == null)
        {
            var record = new AttendanceRecord
            {
                WorkerId = workerId,
                Date = DateTime.UtcNow.Date,
                EntryTime = DateTime.UtcNow
            };
            records.Add(record);
            SaveAttendance(records);
            return "Entry recorded";
        }
        if (todayRecord.ExitTime != null)
        {
            return "Exit already recorded";
        }
        var diff = DateTime.UtcNow - todayRecord.EntryTime; 
        if (diff.Value.TotalSeconds < 3)
        {
            return "Exit allowed only after 3 hours";
        }
        todayRecord.ExitTime = DateTime.UtcNow;
        SaveAttendance(records);
        return "Exit recorded";
    }
    public async Task<List<AttendanceRecord>> GetTodayAttendance()
    {
        var records = await Task.Run(ReadAttendance);
        return [.. records.Where(r => r.Date.Date == DateTime.UtcNow.Date)];

    }

}
