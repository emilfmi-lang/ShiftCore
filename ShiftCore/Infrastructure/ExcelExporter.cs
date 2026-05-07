using ClosedXML.Excel;


namespace ShiftCore.Infrastructure;

public class ExcelExporter
{
    public byte[] ExportDailyAttendance(List<Worker> workers, List<AttendanceRecord> records)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Attendance");

        worksheet.Cell(1, 1).Value = "GÜNDƏLİK DAVAMİYYƏT VƏRƏQİ";
        worksheet.Range(1, 1, 1, 7).Merge(); 
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        worksheet.Cell(2, 5).Value = "GÜN";
        worksheet.Cell(2, 6).Value = "AY";
        worksheet.Cell(2, 7).Value = "İL";

        var today = DateTime.Today;
        worksheet.Cell(3, 5).Value = today.Day;
        worksheet.Cell(3, 6).Value = today.Month;
        worksheet.Cell(3, 7).Value = today.Year;

        worksheet.Cell(4, 1).Value = "F";
        worksheet.Cell(4, 2).Value = "S/s";
        worksheet.Cell(4, 3).Value = "Adı, soyadı, atasının adı";
        worksheet.Cell(4, 4).Value = "Vəzifəsi";
        worksheet.Cell(4, 5).Value = "Giriş";
        worksheet.Cell(4, 6).Value = "İmza";
        worksheet.Cell(4, 7).Value = "Çıxış";
        worksheet.Cell(4, 8).Value = "İmza";

        var headerRange = worksheet.Range(4, 1, 4, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.Yellow; 
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        int row = 5;
        int index = 1;

        foreach (var worker in workers)
        {
            var record = records.FirstOrDefault(r => r.WorkerId == worker.Id);

            worksheet.Cell(row, 1).Value = "";
            worksheet.Cell(row, 2).Value = index;
            worksheet.Cell(row, 3).Value = worker.FullName;
            worksheet.Cell(row, 4).Value = worker.Role;

            worksheet.Range(row, 1, row, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(row, 1, row, 8).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Range(row, 3, row, 4).Style.Fill.BackgroundColor = XLColor.LightYellow;

            if (record != null)
            {
                var entryLocal = record.EntryTime?.AddHours(4);
                var exitLocal = record.ExitTime?.AddHours(4);

                worksheet.Cell(row, 5).Value = entryLocal?.ToString("HH:mm");
                worksheet.Cell(row, 6).Value = entryLocal != null ? "✓" : ""; 
                worksheet.Cell(row, 7).Value = exitLocal?.ToString("HH:mm");
                worksheet.Cell(row, 8).Value = exitLocal != null ? "✓" : "";
            }

            row++;
            index++;
        }

        worksheet.Columns().AdjustToContents();
        worksheet.Column(3).Width = 35; 

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


