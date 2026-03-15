using Scalar.AspNetCore;
using ShiftCore.Infrastructure;
using ShiftCore.Services;

var builder = WebApplication.CreateBuilder(args);

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
app.UseAuthorization();
app.MapControllers();

app.Run();

