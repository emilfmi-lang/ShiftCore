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

                public byte[] ExportMonthlySalaryReport(List<Worker> workers, List<AttendanceRecord> monthlyRecords, int year, int month)
                {
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Salary Report");

                    int daysInMonth = DateTime.DaysInMonth(year, month);

                    // --- 1. BAŞLIQLAR (HEADERS) ---
                    worksheet.Cell(1, 1).Value = "S/s";
                    worksheet.Cell(1, 2).Value = "Ad, Soyad";
                    worksheet.Cell(1, 3).Value = "Vəzifə";
                    worksheet.Cell(1, 4).Value = "Maaş (AZN)";
                    worksheet.Cell(1, 5).Value = "Normativ Saat";

                    // Ayın günlərini (1, 2, 3...) yan-yana başlıq olaraq yazır
                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        worksheet.Cell(1, 5 + i).Value = $"{i}";
                    }

                    worksheet.Cell(1, 6 + daysInMonth).Value = "Cəmi Saat";
                    worksheet.Cell(1, 7 + daysInMonth).Value = "Məbləğ (Yekun)";

                    var headerRange = worksheet.Range(1, 1, 1, 7 + daysInMonth);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // --- 2. İŞÇİLƏR VƏ DATA (DOLDURMA) ---
                    int row = 2;
                    int s_s = 1;

                    foreach (var worker in workers)
                    {
                        worksheet.Cell(row, 1).Value = s_s++;
                        worksheet.Cell(row, 2).Value = worker.FullName;
                        worksheet.Cell(row, 3).Value = worker.Role;
                        worksheet.Cell(row, 4).Value = worker.BaseSalary;
                        worksheet.Cell(row, 5).Value = worker.MonthlyNormativeHours;

                        // Fəhlənin həmin ayki BÜTÜN qeydlərini tapırıq
                        var workerRecords = monthlyRecords.Where(r => r.WorkerId == worker.Id).ToList();

                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            var recordOfTheDay = workerRecords.FirstOrDefault(r => r.Date.Day == day);
                            var cell = worksheet.Cell(row, 5 + day);

                            if (recordOfTheDay != null && recordOfTheDay.TotalWorkedHours.HasValue)
                            {
                                cell.Value = recordOfTheDay.TotalWorkedHours.Value;
                            }
                            else
                            {
                                cell.Value = 0; // İşə gəlmədiyi (yaxud saat fərqi olmadığı) gün
                            }
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        // EXCEL DÜSTURLARI
                        string startCol = worksheet.Cell(row, 6).Address.ColumnLetter;
                        string endCol = worksheet.Cell(row, 5 + daysInMonth).Address.ColumnLetter;

                        // Cəmi İşlənmiş Saat: =SUM(F2:AJ2) (Dinamik olaraq hərfləri hesablayır)
                        var sumCell = worksheet.Cell(row, 6 + daysInMonth);
                        sumCell.FormulaA1 = $"SUM({startCol}{row}:{endCol}{row})";
                        sumCell.Style.Font.Bold = true;

                        // Yekun Məbləğ: =(Cəmi Saat * (Maaş / Normativ Saat))
                        string sumAddress = sumCell.Address.ToString();
                        var totalCell = worksheet.Cell(row, 7 + daysInMonth);
                        totalCell.FormulaA1 = $"IF(E{row}>0, ROUND({sumAddress}*(D{row}/E{row}), 2), 0)";
                        totalCell.Style.Font.Bold = true;
                        totalCell.Style.Font.FontColor = XLColor.Green;

                        row++;
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



