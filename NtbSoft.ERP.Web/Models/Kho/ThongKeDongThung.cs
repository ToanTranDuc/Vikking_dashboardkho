using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Models.THIETBI
{
    public class ThongKeDongThung
    {
        public static DataTable ProcessSttTrung(DataTable tbl)
        {
            var sttThungOld = "0";
            int flagChangeStatus = 0;
            var tblSttTrung = tbl.AsEnumerable().GroupBy(row => row["SttThung"].ToString())
                                    .Where(ageGroup => ageGroup.Count() > 1).ToList();
            foreach (var item in tblSttTrung)
            {
                var SumSL = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == item.Key).GroupBy(y => new
                {
                    SttThung = y["SttThung"],
                }).Select(z => new
                {
                    SttThung = z.Key.SttThung,
                    SoLuong = z.Sum(row => Convert.ToInt32(row["SoLuong"])),
                    TotalPiece = z.Sum(row => Convert.ToInt32(row["SoLuong"])),
                }).ToList();
                foreach (var itemA in SumSL)
                {
                    var tempA = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == itemA.SttThung.ToString());
                    foreach (DataRow dr in tempA)
                    {
                        dr["SoLuong"] = itemA.SoLuong;
                        dr["ToTalPiece"] = itemA.TotalPiece;
                    }
                }
                
            }

            return tbl;
        }
        public static DataTable ProcessSttTrung1(DataTable tbl)
        {
            var sttThungOld = "0";
            int flagChangeStatus = 0;
            var tblSttTrung = tbl.AsEnumerable()
                                    .GroupBy(row => new
                                    {
                                        SttThung = row["SttThung"].ToString(),
                                        MaPKLDisplay = row["MaPKLDisplay"].ToString(),
                                    })
                                    .Where(ageGroup => ageGroup.Count() > 1).ToList();
            foreach (var item in tblSttTrung)
            {
                var SumSL = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == item.Key.SttThung && x["MaPKLDisplay"].ToString() == item.Key.MaPKLDisplay).GroupBy(y => new
                {
                    SttThung = y["SttThung"].ToString(),
                    MaPKLDisplay = y["MaPKLDisplay"].ToString(),
                }).Select(z => new
                {
                    SttThung = z.Key.SttThung,
                    MaPKLDisplay = z.Key.MaPKLDisplay,
                    SoLuong = z.Sum(row => Convert.ToInt32(row["SoLuong"])),
                    TotalPiece = z.Sum(row => Convert.ToInt32(row["SoLuong"])),
                    KhoiLuong = z.Sum(row => Convert.ToDouble(row["KhoiLuong"])),
                }).ToList();
                foreach (var itemA in SumSL)
                {
                    var tempA = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == itemA.SttThung && x["MaPKLDisplay"].ToString() == itemA.MaPKLDisplay);
                    foreach (DataRow dr in tempA)
                    {
                        dr["SoLuong"] = itemA.SoLuong;
                        dr["ToTalPiece"] = itemA.TotalPiece;
                        var KhoiLuong = dr["PCB_Pack"].ToString() != "0" ? Convert.ToDouble(dr["KhoiLuong"]) : itemA.KhoiLuong;
                        dr["KhoiLuong"] = KhoiLuong;
                        dr["TrongLuong"] = KhoiLuong + Convert.ToDouble(dr["TrongLuongA"]);
                    }
                }
            }
            return tbl;
        }
        public static DataTable sumToTalPCS(DataTable tbl)
        {
            List<string> columnNamesWithSize = tbl.Columns.Cast<DataColumn>()
                        .Where(column => column.ColumnName.Contains("@"))
                        .Select(column => column.ColumnName)
                        .Distinct()
                        .ToList();
            DataTable summaryDataTable = new DataTable();

            summaryDataTable.Columns.Add("DauSize", typeof(string));
            summaryDataTable.Columns.Add("ColorID", typeof(string));
            summaryDataTable.Columns.Add("TongSize", typeof(int));
            summaryDataTable.Columns.Add("TenMau", typeof(string));

            var dauSize = tbl.AsEnumerable().Select(x => new { DauSize = x["DauSizeID"], Color = x["ColorID"], TenMau = x["TenMau"] }).Distinct().ToList();

            foreach (string sizeColumn in columnNamesWithSize)
            {
                summaryDataTable.Columns.Add(sizeColumn, typeof(int));
            }

            foreach (var dausize in dauSize)
            {
                DataRow newRow = summaryDataTable.NewRow();

                newRow["DauSize"] = dausize.DauSize;
                newRow["ColorID"] = dausize.Color;
                newRow["TenMau"] = dausize.TenMau;
                int tongSize = 0;
                foreach (string sizeColumn in columnNamesWithSize)
                {
                    int sumTotalSize = tbl.AsEnumerable()
                        .Where(x => x["DauSizeID"].ToString() == dausize.DauSize.ToString() && x["ColorID"].ToString() == dausize.Color.ToString())
                        .Sum(y => Convert.ToInt32(y[sizeColumn].ToString() == "" ? 0 : y[sizeColumn]) * Convert.ToInt32(y["SLThung"]));

                    newRow[sizeColumn] = sumTotalSize;
                    tongSize += sumTotalSize;
                }
                newRow["TongSize"] = tongSize;
                summaryDataTable.Rows.Add(newRow);
            }
            return summaryDataTable;
        }
        public static void dtXuatEX(DataTable dtPKLXuatHang, ExcelWorksheet worksheet, bool flagFilter = true)
        {
            ExcelRange range = worksheet.Cells;
            List<string> columnNamesWithSize = dtPKLXuatHang.Columns.Cast<DataColumn>()
                        .Where(column => column.ColumnName.Contains("@"))
                        .Select(column => column.ColumnName)
                        .Distinct()
                        .ToList();
            int colum = 7;
            range = worksheet.Cells["A9:b9"]; range.Merge = true; range.Value = "Carton Number";
            range = worksheet.Cells["c9"]; range.Merge = true; range.Value = "PO";
            range = worksheet.Cells["d9"]; range.Merge = true; range.Value = "Supplier";
            range = worksheet.Cells["e9"]; range.Merge = true; range.Value = "Size/Inseam";
            range = worksheet.Cells["f9"]; range.Merge = true; range.Value = "Color";
            foreach (var colums in columnNamesWithSize)
            {
                string splitColum = colums.Split('@')[0];
                worksheet.Cells[9, colum].Value = splitColum.ToString();
                worksheet.Cells[9, colum].Style.Font.Bold = true;
                worksheet.Column(colum).AutoFit();
                colum++;
            }
            
            var uniqueValues = dtPKLXuatHang.AsEnumerable().
            Select(x => new
            {
                MaPKL = x["MaPKLDisplay"],
                SttThung = x["SttThung"],
                SLThung = x["SLThung"],
                TotalPiece = x["TotalPiece"],
                SoLuong = x["SoLuong"],
                TrongLuong = x["TrongLuong"],
                KhoiLuong = x["KhoiLuong"],
            }).Distinct().ToList();

            int totalSLThung = uniqueValues.Sum(x => Convert.ToInt32(x.SLThung));
            int totalTotalPiece = uniqueValues.Sum(x => Convert.ToInt32(x.TotalPiece));
            int totalSoLuong = uniqueValues.Sum(x => Convert.ToInt32(x.SoLuong));
            float totalTrongLuong = uniqueValues.Sum(x => Convert.ToSingle(x.TrongLuong));
            float roundedTotalTrongLuong = (float)Math.Round(totalTrongLuong, 1);
            float totalKhoiLuong = uniqueValues.Sum(x => Convert.ToSingle(x.KhoiLuong));
            float roundedTotalKhoiLuong = (float)Math.Round(totalKhoiLuong, 1);
            List<string> sumToTalAll = new List<string>() { totalSoLuong.ToString(), totalSLThung.ToString(), totalTotalPiece.ToString(), roundedTotalKhoiLuong.ToString(), roundedTotalTrongLuong.ToString(), };

            range = worksheet.Cells[9, colum]; range.Value = "QTY "; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "Carton "; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "ToTal Pieces"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "NW"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "GW"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "Carton"; worksheet.Column(colum).AutoFit(); colum++;
            int row = 10;
            int index = 0;
            int rowMerge = 10;
            int indexMerge = 0;
            bool checkCountMerge = false;
            foreach (DataRow rows in dtPKLXuatHang.Rows)
            {
                colum = 7;
                worksheet.Cells[row, 1].Value = Convert.ToInt32(rows["TuThung"]);
                worksheet.Cells[row, 2].Value = Convert.ToInt32(rows["DenThung"]);
                worksheet.Cells[row, 3].Value = rows["PO"].ToString();
                worksheet.Cells[row, 4].Value = rows["TenDVSX"].ToString();
                worksheet.Cells[row, 5].Value = rows["DauSize"].ToString();
                worksheet.Cells[row, 6].Value = rows["TenMau"].ToString();
                worksheet.Column(3).AutoFit(); worksheet.Column(4).AutoFit(); worksheet.Column(5).AutoFit(); worksheet.Column(6).AutoFit();
                foreach (var colums in columnNamesWithSize)
                {
                    if (rows[colums.ToString()].ToString() == "")
                    {
                        colum++;
                        continue;
                    }
                    int cellValue = Convert.ToInt32(rows[colums.ToString()]);
                    worksheet.Cells[row, colum].Value = cellValue == 0 ? (object)" " : cellValue;
                    worksheet.Column(colum).AutoFit();
                    colum++;
                }
                double khoiLuong = Convert.ToDouble(rows["KhoiLuong"]);
                double roundedKhoiLuong = Math.Round(khoiLuong, 1);
                double trongLuong = Convert.ToDouble(rows["TrongLuong"]);
                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["SoLuong"]); worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["SLThung"]); worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["TotalPiece"]); worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = khoiLuong == 0 ? (object)" " : roundedKhoiLuong;
                worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = trongLuong == 0 ? (object)" " : Math.Round(trongLuong, 1);
                worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = rows["KyHieu"].ToString(); worksheet.Column(colum).AutoFit(); colum++;
                int currentSttThung = Convert.ToInt32(rows["SttThung"]);
                int previousSttThung = (index + 1 < dtPKLXuatHang.Rows.Count) ? Convert.ToInt32(dtPKLXuatHang.Rows[index + 1]["SttThung"]) : 192836403;
                if (currentSttThung == previousSttThung)
                {
                    indexMerge++;
                    checkCountMerge = true;
                }
                else
                {
                    if (checkCountMerge)
                    {
                        rowMerge = row - indexMerge;
                        checkCountMerge = false;
                    }
                    else
                        rowMerge = row;
                    for (int i = 0; i < 5; i++)
                    {
                        MergeCellsByRow(worksheet, colum - i - 2, rowMerge, rowMerge + indexMerge, "center");
                    }
                    MergeCellsByRow(worksheet, 1, rowMerge, rowMerge + indexMerge, "center");
                    MergeCellsByRow(worksheet, 2, rowMerge, rowMerge + indexMerge, "center");
                    rowMerge = row;
                    indexMerge = 0;
                }

                index++;
                row++;

            }
            int indexsumAll = 6;
            foreach (string sumAll in sumToTalAll)
            {
                worksheet.Cells[row, colum - indexsumAll].Value = sumAll;
                worksheet.Cells[row, colum - indexsumAll].Style.Font.Bold = true;
                indexsumAll--;
            }
            var borderData = worksheet.Cells[9, 1, row - 1, colum - 1].Style.Border;
            borderData.Bottom.Style =
                borderData.Top.Style =
                borderData.Left.Style =
                borderData.Right.Style = ExcelBorderStyle.Thin;
            row++;
            range = worksheet.Cells[row, 5]; range.Merge = true; range.Value = "Size/Inseam";
            range = worksheet.Cells[row, 6]; range.Merge = true; range.Value = "Color";
            colum = 7;
            foreach (var colums in columnNamesWithSize)
            {
                string splitColum = colums.Split('@')[0];
                worksheet.Cells[row, colum].Value = splitColum.ToString();
                worksheet.Cells[row, colum].Style.Font.Bold = true;
                worksheet.Column(colum).AutoFit();
                colum++;
            }
            range = worksheet.Cells[row, colum]; range.Merge = true; range.Value = "Total PCS ";
            row++;

            colum = 7;
            rowMerge = row;
            var dauSize = dtPKLXuatHang.AsEnumerable().Select(x => new { DauSize = x["DauSizeID"], TenMau = x["TenMau"], ColorID = x["ColorID"] }).Distinct().ToList();
            foreach (var dausize in dauSize)
            {
                worksheet.Cells[row, 5].Value = dausize.DauSize;
                worksheet.Cells[row, 6].Value = dausize.TenMau;
                colum = 7;
                int sumPCS = 0;
                foreach (var colums in columnNamesWithSize)
                {
                    int sumTotalSize = dtPKLXuatHang.AsEnumerable()
                    .Where(x => x["DauSizeID"].ToString() == dausize.DauSize.ToString() && x["ColorID"].ToString() == dausize.ColorID.ToString())
                    .Sum(y => Convert.ToInt32(y[colums].ToString() == "" ? 0 : y[colums]) * Convert.ToInt32(y["SLThung"]));
                    worksheet.Cells[row, colum].Value = sumTotalSize == 0 ? (object)" " : sumTotalSize;
                    worksheet.Column(colum).AutoFit();
                    colum++;
                    sumPCS += sumTotalSize;
                    worksheet.Cells[row, colum].Value = sumPCS;

                }
                indexMerge++;
                row++;
            }
            range = worksheet.Cells[row, 5, row, 6]; range.Merge = true; range.Value = "Tổng";
            colum = 7;
            int sumTotal = 0;
            for (int i = 0; i < columnNamesWithSize.Count + 1; i++)
            {
                worksheet.Cells[row, colum].Formula = "=SUM(" + worksheet.Cells[rowMerge, colum].Address + ":" + worksheet.Cells[rowMerge + indexMerge - 1, colum].Address + ")";
                worksheet.Cells[row, colum].Style.Font.Bold = true;
                colum++;
            }
            row += 2;
            range = worksheet.Cells[row, 7]; range.Merge = true; range.Value = "pcs";
            range = worksheet.Cells[row, 6]; range.Merge = true; range.Value = totalSLThung;
            range = worksheet.Cells[row + 1, 7]; range.Merge = true; range.Value = "set";
            range = worksheet.Cells[row + 1, 6]; range.Merge = true; range.Value = totalTotalPiece;
            range = worksheet.Cells[row + 1, 9]; range.Merge = true; range.Value = "pcs";
            range = worksheet.Cells[row + 1, 8]; range.Merge = true; range.Value = totalTotalPiece * 2;
            range = worksheet.Cells[row + 2, 7]; range.Merge = true; range.Value = "kg";
            range = worksheet.Cells[row + 2, 6]; range.Merge = true; range.Value = roundedTotalKhoiLuong.ToString();
            range = worksheet.Cells[row + 3, 7]; range.Merge = true; range.Value = "kg";
            range = worksheet.Cells[row + 3, 6]; range.Merge = true; range.Value = roundedTotalTrongLuong.ToString();
            range = worksheet.Cells[row + 4, 6]; range.Merge = true; range.Value = dtPKLXuatHang.Rows[0]["KyHieu"].ToString();
            range = worksheet.Cells[row + 5, 6]; range.Merge = true; range.Value = Convert.ToInt32(0.6 * 0.4 * 0.42 * totalSLThung);
            range = worksheet.Cells[row + 5, 7]; range.Merge = true; range.Value = "CBM";

            range = worksheet.Cells[row, 4, row, 5]; range.Merge = true; range.Value = "Total boxes "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 1, 4, row + 1, 5]; range.Merge = true; range.Value = "Total quantity  "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 2, 4, row + 2, 5]; range.Merge = true; range.Value = "Total net weight"; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 3, 4, row + 3, 5]; range.Merge = true; range.Value = "Total Gross weight"; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 4, 4, row + 4, 5]; range.Merge = true; range.Value = "Cnt Measurements "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 5, 4, row + 5, 5]; range.Merge = true; range.Value = "Total CBM "; range.Style.Font.Bold = true;
            for (int i = 1; i < 7; i++)
            {
                worksheet.Cells[row + i - 1, 4, row + i - 1, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }
            var borderData1 = worksheet.Cells[rowMerge - 1, 5, rowMerge + indexMerge, colum - 1].Style.Border;
            borderData1.Bottom.Style =
                borderData1.Top.Style =
                borderData1.Left.Style =
                borderData1.Right.Style = ExcelBorderStyle.Thin;

        }
        private static void MergeCellsByRow(ExcelWorksheet worksheet, int colum, int startrow, int endrow, string align)
        {
            worksheet.Cells[startrow, colum, endrow, colum].Merge = true;

            using (var range = worksheet.Cells[startrow, colum, endrow, colum])
            {
                if (align == "center")
                {
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                range.Style.WrapText = true;
            }
        }
        public static string getImgPath(string Img)
        {
            string Paths = Directory.GetCurrentDirectory();
            return $"{Paths}\\Resources\\{Img}";
        }
    }
   
  
    public class CTDHResult
    {
        public DataTable ChiTiet { get; set; }
        public DataTable Total { get; set; }
    }
}