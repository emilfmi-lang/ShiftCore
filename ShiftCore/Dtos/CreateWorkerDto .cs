using DocumentFormat.OpenXml.Bibliography;

namespace ShiftCore.Dtos;

public class CreateWorkerDto
{
    public string FullName { get; set; } = string.Empty;    
    public string Role { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public int MonthlyNormativeHours { get; set; } 
}
