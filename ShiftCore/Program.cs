using ShiftCore.Infrastructure;
using ShiftCore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<JsonStorage>();
builder.Services.AddSingleton<WorkerService>();
builder.Services.AddSingleton<AttendanceService>();
builder.Services.AddSingleton<ExcelExporter>();

var app = builder.Build();
app.MapControllers();

app.Run();

