using Scalar.AspNetCore;
using ShiftCore.Services;
using ShiftCore.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<JsonStorage>();
builder.Services.AddSingleton<WorkerService>();
builder.Services.AddSingleton<AttendanceService>();
builder.Services.AddSingleton<ExcelExporter>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers();

app.Run();

