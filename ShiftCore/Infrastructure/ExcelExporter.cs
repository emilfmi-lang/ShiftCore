using ClosedXML.Excel;
using ShiftCore.Entity;

namespace ShiftCore.Infrastructure;

public class ExcelExporter
{
    public byte[] ExportDailyAttendance(List<Worker> workers, List<AttendanceRecord> records)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Attendance");

        worksheet.Cell(1, 1).Value = "S/s";
        worksheet.Cell(1, 2).Value = "Ad, soyad";
        worksheet.Cell(1, 3).Value = "Vəzifə";
        worksheet.Cell(1, 4).Value = "Giriş";
        worksheet.Cell(1, 5).Value = "İmza";
        worksheet.Cell(1, 6).Value = "Çıxış";
        worksheet.Cell(1, 7).Value = "İmza";

        int row = 2;
        int index = 1;

        foreach (var worker in workers)
        {
            var record = records.FirstOrDefault(r => r.WorkerId == worker.Id);

            worksheet.Cell(row, 1).Value = index;
            worksheet.Cell(row, 2).Value = worker.FullName;
            worksheet.Cell(row, 3).Value = worker.Role;

            if (record != null)
            {
                worksheet.Cell(row, 4).Value = record.EntryTime?.ToString("HH:mm");
                worksheet.Cell(row, 6).Value = record.ExitTime?.ToString("HH:mm");
            }

            row++;
            index++;
        }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                return stream.ToArray();
            }

            public byte[] ExportWorkers(List<Worker> workers)
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Workers");

                worksheet.Cell(1, 1).Value = "S/s";
                worksheet.Cell(1, 2).Value = "Ad, soyad";
                worksheet.Cell(1, 3).Value = "Vəzifə";
                worksheet.Cell(1, 4).Value = "Status";
                worksheet.Cell(1, 5).Value = "Yaradılma tarixi";

                int row = 2;
                int index = 1;

                foreach (var worker in workers)
                {
                    worksheet.Cell(row, 1).Value = index;
                    worksheet.Cell(row, 2).Value = worker.FullName;
                    worksheet.Cell(row, 3).Value = worker.Role;
                    worksheet.Cell(row, 4).Value = worker.IsActive ? "Aktiv" : "Deaktiv";
                    worksheet.Cell(row, 5).Value = worker.CreatedAt.ToString("dd.MM.yyyy HH:mm");

                    row++;
                    index++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                return stream.ToArray();
            }
        }


