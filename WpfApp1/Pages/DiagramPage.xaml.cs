using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace WpfApp1.Pages
{
    public partial class DiagramPage : Page
    {
        private Chart chart;

        public DiagramPage()
        {
            InitializeComponent();

            chart = new Chart();
            chart.ChartAreas.Add(new ChartArea("Main"));
            chart.Legends.Add(new Legend());
            chart.Series.Add(new Series("Расходы на перевозки") { IsValueShownAsLabel = true });

            WindowsFormsHost host = new WindowsFormsHost();
            host.Child = chart;
            ChartGrid.Children.Add(host);

            ComboBoxUser.ItemsSource = Core.DB.Users.ToList();
            ComboBoxChart.ItemsSource = new SeriesChartType[]
            {
                SeriesChartType.Column,
                SeriesChartType.Bar,
                SeriesChartType.Pie,
                SeriesChartType.Doughnut,
                SeriesChartType.Line
            };
            ComboBoxChart.SelectedIndex = 0;
            ComboBoxUser.SelectedIndex = 0;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            Users user = ComboBoxUser.SelectedItem as Users;
            if (chart == null || user == null || ComboBoxChart.SelectedItem == null)
                return;

            Series series = chart.Series[0];
            series.ChartType = (SeriesChartType)ComboBoxChart.SelectedItem;
            series.Points.Clear();

            List<Shipment> shipments = Core.DB.Shipment.ToList().Where(s => s.UserID == user.ID).ToList();
            foreach (CargoType type in Core.DB.CargoType.ToList())
            {
                int sum = shipments.Where(s => s.CargoTypeID == type.ID).Sum(s => s.Price);
                if (sum > 0)
                    series.Points.AddXY(type.Name, sum);
            }
        }

        private void ButtonWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Users> users = Core.DB.Users.ToList();
                List<CargoType> types = Core.DB.CargoType.ToList();
                List<Shipment> shipments = Core.DB.Shipment.ToList();

                Word.Application application = new Word.Application();
                Word.Document document = application.Documents.Add();
                Word.Selection selection = application.Selection;
                selection.Font.Name = "Times New Roman";

                foreach (Users user in users)
                {
                    List<Shipment> userShipments = shipments.Where(s => s.UserID == user.ID).ToList();

                    selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    selection.Font.Size = 20;
                    selection.Font.Bold = 1;
                    selection.Font.Color = Word.WdColor.wdColorBlack;
                    selection.TypeText(user.FIO);
                    selection.TypeParagraph();

                    selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    selection.Font.Size = 12;
                    selection.Font.Bold = 0;

                    Word.Table table = document.Tables.Add(selection.Range, types.Count + 1, 2);
                    table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table.Cell(1, 1).Range.Text = "Тип груза";
                    table.Cell(1, 2).Range.Text = "Сумма расходов";
                    table.Rows[1].Range.Font.Bold = 1;
                    table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < types.Count; i++)
                    {
                        int typeId = types[i].ID;
                        int sum = userShipments.Where(s => s.CargoTypeID == typeId).Sum(s => s.Price);
                        table.Cell(i + 2, 1).Range.Text = types[i].Name;
                        table.Cell(i + 2, 2).Range.Text = sum.ToString("N2") + " руб.";
                    }

                    selection.EndKey(Word.WdUnits.wdStory);
                    selection.TypeParagraph();

                    Shipment maxShipment = userShipments.OrderByDescending(s => s.Price).FirstOrDefault();
                    Shipment minShipment = userShipments.OrderBy(s => s.Price).FirstOrDefault();
                    if (maxShipment != null)
                    {
                        selection.Font.Color = Word.WdColor.wdColorDarkRed;
                        selection.TypeText("Самая дорогая перевозка - " + maxShipment.Name + " за " + maxShipment.Price.ToString("N2") +
                                           " руб. от " + maxShipment.ShipDate.ToString("dd.MM.yyyy"));
                        selection.TypeParagraph();
                        selection.Font.Color = Word.WdColor.wdColorDarkGreen;
                        selection.TypeText("Самая дешёвая перевозка - " + minShipment.Name + " за " + minShipment.Price.ToString("N2") +
                                           " руб. от " + minShipment.ShipDate.ToString("dd.MM.yyyy"));
                        selection.TypeParagraph();
                    }
                    else
                    {
                        selection.Font.Color = Word.WdColor.wdColorBlack;
                        selection.TypeText("Перевозок нет");
                        selection.TypeParagraph();
                    }

                    if (user != users.Last())
                        selection.InsertBreak(Word.WdBreakType.wdPageBreak);
                }

                foreach (Word.Section section in document.Sections)
                {
                    section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                    section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = DateTime.Now.ToString("dd.MM.yyyy");
                }

                string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string name = "Shipments_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string docxPath = Path.Combine(folder, name + ".docx");
                string pdfPath = Path.Combine(folder, name + ".pdf");
                document.SaveAs2(docxPath);
                document.ExportAsFixedFormat(pdfPath, Word.WdExportFormat.wdExportFormatPDF);
                application.Visible = true;

                Helper.Info("Отчёты сохранены на рабочем столе:\n" + docxPath + "\n" + pdfPath);
            }
            catch (Exception ex)
            {
                Helper.Error("Не удалось выполнить экспорт в Word. Проверьте, что Microsoft Word установлен.\n" + ex.Message);
            }
        }

        private static void Put(Excel.Worksheet sheet, int row, int column, object value)
        {
            ((Excel.Range)sheet.Cells[row, column]).Value2 = value;
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Users> users = Core.DB.Users.ToList().OrderBy(u => u.FIO).ToList();
                List<CargoType> types = Core.DB.CargoType.ToList();
                List<Shipment> shipments = Core.DB.Shipment.ToList();

                Excel.Application application = new Excel.Application();
                application.SheetsInNewWorkbook = users.Count;
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);
                int grandTotal = 0;

                for (int i = 0; i < users.Count; i++)
                {
                    Excel.Worksheet sheet = (Excel.Worksheet)workbook.Worksheets[i + 1];
                    string sheetName = users[i].FIO;
                    sheet.Name = sheetName.Length > 31 ? sheetName.Substring(0, 31) : sheetName;

                    int row = 1;
                    Put(sheet, row, 1, "Дата перевозки");
                    Put(sheet, row, 2, "Название груза");
                    Put(sheet, row, 3, "Вес, кг");
                    Put(sheet, row, 4, "Стоимость, руб.");
                    Excel.Range header = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 4]];
                    header.Font.Bold = true;
                    header.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    row++;

                    int userId = users[i].ID;
                    var groups = shipments.Where(s => s.UserID == userId)
                                          .OrderBy(s => s.ShipDate)
                                          .GroupBy(s => s.CargoTypeID)
                                          .OrderBy(g => types.First(t => t.ID == g.Key).Name);

                    foreach (var group in groups)
                    {
                        Excel.Range typeRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 4]];
                        typeRange.Merge();
                        typeRange.Value2 = types.First(t => t.ID == group.Key).Name;
                        typeRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        typeRange.Font.Italic = true;
                        row++;

                        int firstRow = row;
                        foreach (Shipment shipment in group)
                        {
                            Put(sheet, row, 1, shipment.ShipDate.ToString("dd.MM.yyyy"));
                            Put(sheet, row, 2, shipment.Name);
                            Put(sheet, row, 3, shipment.Weight);
                            Put(sheet, row, 4, shipment.Price);
                            row++;
                        }

                        Excel.Range totalRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 3]];
                        totalRange.Merge();
                        totalRange.Value2 = "ИТОГО:";
                        totalRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        totalRange.Font.Bold = true;
                        Excel.Range totalCell = (Excel.Range)sheet.Cells[row, 4];
                        totalCell.Formula = "=SUM(D" + firstRow + ":D" + (row - 1) + ")";
                        totalCell.Font.Bold = true;
                        grandTotal += group.Sum(s => s.Price);
                        row++;
                    }

                    Excel.Range allRange = sheet.Range[sheet.Cells[1, 1], sheet.Cells[row - 1, 4]];
                    allRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Columns.AutoFit();
                }

                Excel.Worksheet summarySheet = (Excel.Worksheet)workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
                summarySheet.Name = "Общий итог";
                Put(summarySheet, 1, 1, "Общий итог:");
                Put(summarySheet, 1, 2, grandTotal);
                Excel.Range summaryRange = summarySheet.Range[summarySheet.Cells[1, 1], summarySheet.Cells[1, 2]];
                summaryRange.Font.Color = Excel.XlRgbColor.rgbRed;
                summaryRange.Font.Bold = true;
                summarySheet.Columns.AutoFit();

                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Shipments_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                workbook.SaveAs(path);
                application.Visible = true;

                Helper.Info("Отчёт сохранён на рабочем столе:\n" + path);
            }
            catch (Exception ex)
            {
                Helper.Error("Не удалось выполнить экспорт в Excel. Проверьте, что Microsoft Excel установлен.\n" + ex.Message);
            }
        }
    }
}
