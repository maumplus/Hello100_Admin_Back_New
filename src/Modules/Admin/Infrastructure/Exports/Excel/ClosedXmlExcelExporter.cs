using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;

namespace Hello100Admin.Modules.Admin.Infrastructure.Exports.Excel
{
    public sealed class ClosedXmlExcelExporter : IExcelExporter
    {
        public byte[] Export<T>(IReadOnlyList<T> rows, string sheetName, IReadOnlyList<ExcelColumn<T>> columns)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            for (int c = 0; c < columns.Count; c++)
                ws.Cell(1, c + 1).Value = columns[c].Header;

            for (int r = 0; r < rows.Count; r++)
            {
                var row = rows[r];
                for (int c = 0; c < columns.Count; c++)
                {
                    var cell = ws.Cell(r + 2, c + 1);
                    cell.SetValue(columns[c].Value(row)?.ToString() ?? "");
                }
            }

            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] Export<T>(IReadOnlyList<T> rows,
                                string sheetName,
                                string title,
                                IReadOnlyList<ExcelColumn<T>> columns,
                                int titleFontSize,
                                bool titleBold)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            const int startRow = 2;
            const int startCol = 2;

            var titleRow = startRow;
            var headerRow = startRow + 1;
            var dataStartRow = startRow + 2;

            var lastCol = startCol + columns.Count - 1;

            #region SET TITLE & STYLES
            var titleRange = ws.Range(titleRow, startCol, titleRow, lastCol);
            titleRange.Merge();
            titleRange.FirstCell().SetValue(title);

            titleRange.Style.Font.Bold = titleBold;
            titleRange.Style.Font.FontSize = titleFontSize;
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            titleRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#A6A6A6");
            titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Row(titleRow).Height = 48;
            #endregion

            #region SET HEADER & STYLES
            for (int c = 0; c < columns.Count; c++)
            {
                var cell = ws.Cell(headerRow, startCol + c);
                cell.SetValue(columns[c].Header);

                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
            }

            ws.Row(headerRow).Height = 20;
            ws.SheetView.FreezeRows(headerRow);
            #endregion

            #region SET BODY & STYLES
            for (int r = 0; r < rows.Count; r++)
            {
                var row = rows[r];

                for (int c = 0; c < columns.Count; c++)
                {
                    var col = columns[c];
                    var cell = ws.Cell(dataStartRow + r, startCol + c);

                    var v = this.NormalizeExcelValue(col.Value(row));
                    this.SetCellValue(cell, v);

                    if (!string.IsNullOrWhiteSpace(col.Format))
                        cell.Style.NumberFormat.Format = col.Format;

                    cell.Style.Alignment.Horizontal = col.Align;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }
            }

            var lastRow = dataStartRow + rows.Count - 1;

            if (rows.Count > 0)
            {
                var tableRange = ws.Range(headerRow, startCol, lastRow, lastCol);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }
            else
            {
                var headerRange = ws.Range(headerRow, startCol, headerRow, lastCol);
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            ws.Range(headerRow, startCol, headerRow, lastCol).SetAutoFilter();

            for (int c = 0; c < columns.Count; c++)
            {
                var colIndex = startCol + c;
                ws.Column(colIndex).Width = columns[c].Width;
            }
            #endregion

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private void SetCellValue(IXLCell cell, object? value)
        {
            if (value is null) { cell.SetValue(""); return; }

            switch (value)
            {
                case string s: cell.SetValue(s); break;
                case int i: cell.SetValue(i); break;
                case long l: cell.SetValue(l); break;
                case decimal m: cell.SetValue(m); break;
                case double d: cell.SetValue(d); break;
                case float f: cell.SetValue(f); break;
                case bool b: cell.SetValue(b); break;
                case DateTime dt: cell.SetValue(dt); break;
                default:
                    cell.SetValue(value.ToString() ?? "");
                    break;
            }
        }

        private object NormalizeExcelValue(object? value)
        {
            if (value is null) return "";

            return value switch
            {
                DateOnly d => d.ToString("yyyy-MM-dd"),
                TimeOnly t => t.ToString("HH:mm:ss"),
                Enum e => e.ToString(),
                Guid g => g.ToString(),
                _ => value
            };
        }
    }
}
