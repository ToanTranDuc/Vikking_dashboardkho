using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    public class KHDongThungLibP
    {
        public static bool CheckRole(string MaDVSX)
        {
            if (GlobleData.lstDVSX.Contains(MaDVSX)) return true;
            return false;
        }
        public static DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPKL", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaLenh", typeof(string));
            tbl.Columns.Add("DotSX", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("ColorID", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("NgayLapKH", typeof(DateTime));
            tbl.Columns.Add("ChieuDai", typeof(double));
            tbl.Columns.Add("ChieuRong", typeof(double));
            tbl.Columns.Add("ChieuCao", typeof(double));
            tbl.Columns.Add("TrongLuong", typeof(double));
            tbl.Columns.Add("KhoiLuong", typeof(double));
            tbl.Columns.Add("SoLuongThung", typeof(int));
            tbl.Columns.Add("TuThung", typeof(int));
            tbl.Columns.Add("DenThung", typeof(int));
            tbl.Columns.Add("SttThung", typeof(int));
            tbl.Columns.Add("SoLuongSP", typeof(int));
            tbl.Columns.Add("IsDongThung", typeof(bool));
            tbl.Columns.Add("NgayDongThung", typeof(DateTime));
            tbl.Columns.Add("QRCode", typeof(string));
            tbl.Columns.Add("IsScan", typeof(bool));
            tbl.Columns.Add("IsNhapKho", typeof(bool));
            tbl.Columns.Add("NgayNhapKho", typeof(DateTime));
            tbl.Columns.Add("KyHieu", typeof(string));
            tbl.Columns.Add("Chon", typeof(bool));
            tbl.Columns.Add("SttThung_temp", typeof(int));
            tbl.Columns.Add("SttThung_LapMau", typeof(int));
            tbl.Columns.Add("SttThung_decat", typeof(int));
            tbl.Columns.Add("SttThung_start", typeof(int));
            tbl.Columns.Add("IsThungLe", typeof(int));
            tbl.Columns.Add("Cont", typeof(string));
            tbl.Columns.Add("MaDH_XH", typeof(string));
            tbl.Columns.Add("POID_XH", typeof(string));
            tbl.Columns.Add("NVien", typeof(string));
            return tbl;
        }
        public static DataTable GopThung(DataTable data, int flagDeCatLon = -1)
        {
            bool flagPrevent = true;
            var dataT = data.Copy();
            var dtGopTemp = data.AsEnumerable().Where(x => (bool)x["Chon"] == true);
            if (dtGopTemp.Count() == 0) return new DataTable();
            var dtGop = dtGopTemp.CopyToDataTable();
            var drRow = dtGop.Rows[0];
            var drRowA = data.AsEnumerable().Where(x => (bool)x["Chon"] == true && Convert.ToInt32(x["TuThung"]) == Convert.ToInt32(drRow["TuThung"])).FirstOrDefault();

            var index = data.Rows.IndexOf(drRowA) + 1;
            foreach (DataRow dr in data.Rows)
            {
                if ((bool)dr["Chon"] == true && flagPrevent)
                {
                    flagPrevent = false;
                    continue;
                }
                if ((bool)dr["Chon"] == true && !flagPrevent)
                {
                    
                    var drRemove = dataT.AsEnumerable().Where(x => Convert.ToInt16(x["ID"]) == Convert.ToInt16(dr["ID"])).FirstOrDefault();
                    var drNew = dataT.NewRow();
                    CopyDataRow(drRemove, drNew);
                    if (dr["KieuLap"].ToString() == "1")
                    {
                        if (drNew["ColorID"].ToString() != drRowA["ColorID"].ToString())
                        {
                            MessageBox.Show("Đang lập theo màu. Không thể gọp 2 màu khác nhau");
                            foreach (DataRow drT in dataT.Rows)
                            {
                                drT["Chon"] = false;
                            }
                            return dataT;
                        }
                    }
                    dataT.Rows.Remove(drRemove);
                    
                    var trongluong1 = Convert.ToDouble(drRowA["TrongLuong"]);
                    var trongluong2 = Convert.ToDouble(drNew["TrongLuong"]);
                    var khoiLuong1 = Convert.ToDouble(drRowA["KhoiLuong"]);
                    var khoiLuong2 = Convert.ToDouble(drNew["KhoiLuong"]);
                    var SoLuong1 = Convert.ToInt16(drRowA["SoLuong"]);
                    var SoLuong2 = Convert.ToInt16(drNew["SoLuong"]);
                    var TotalPiece1 = Convert.ToInt16(drRowA["TotalPiece"]);
                    var TotalPiece2 = Convert.ToInt16(drNew["TotalPiece"]);


                    drNew["TuThung"] = drRowA["TuThung"];
                    drNew["DenThung"] = drRowA["DenThung"];
                    drNew["SttThung"] = drRowA["SttThung"];
                    drNew["TrongLuong"] = drRowA["TrongLuong"] = trongluong1 + trongluong2;
                    drNew["KhoiLuong"] = drRowA["KhoiLuong"] = khoiLuong1 + khoiLuong2;
                    drNew["IsThungLe"] = true;
                    if (drNew["MaDVSX"].ToString() == drRowA["MaDVSX"].ToString() && drNew["MaLenh"].ToString() == drRowA["MaLenh"].ToString() && drNew["DauSizeID"].ToString() == drRowA["DauSizeID"].ToString()
                        && drNew["ColorID"].ToString() == drRowA["ColorID"].ToString()
                        )
                    {
                        int sumSL = 0;
                        foreach (DataColumn dc in data.Columns)
                        {
                            string colName = dc.ColumnName;
                            if (!colName.Contains('@')) continue;
                            var SLNew = drNew[colName].ToString() == "" ? 0 : Convert.ToInt32(drNew[colName]);
                            var SLOld = drRowA[colName].ToString() == "" ? 0 : Convert.ToInt32(drRowA[colName]);
                            drRowA[colName] = SLNew + SLOld;
                            sumSL += drRowA[colName].ToString() == "" ? 0 : Convert.ToInt32(drRowA[colName]);
                            drRowA["SoLuong"] = sumSL;
                            if (drRowA["SLThung"].ToString() != "0" || drRowA["SLThung"].ToString() != "")
                                drRowA["TotalPiece"] = sumSL * Convert.ToInt32(drRowA["SLThung"]);
                        }
                        //sumSL += drFocus["Size_" + dr["SizeID"].ToString()].ToString() == "" ? 0 : Convert.ToInt32(drFocus["Size_" + dr["SizeID"].ToString()]);
                        CopyDataRow(drRowA, drNew);
                        index = index - 1;
                        dataT.Rows.RemoveAt(index);

                    }
                    else
                    {
                        dataT.Rows[index - 1]["TrongLuong"] = drNew["TrongLuong"];
                        dataT.Rows[index - 1]["KhoiLuong"] = drNew["KhoiLuong"];
                        drNew["SoLuong"] = dataT.Rows[index - 1]["SoLuong"] = SoLuong1 + SoLuong2;
                        drNew["TotalPiece"] = dataT.Rows[index - 1]["TotalPiece"] = TotalPiece1 + TotalPiece2;
                    }
                    dataT.Rows.InsertAt(drNew, index);
                    foreach (DataRow drT in dataT.Rows)
                    {
                        drT["Chon"] = false;
                    }
                    index += 1;
                }
            }
            // gridView1.SetRowCellValue(0, "ColumnName", 0);
            TinhToanLaiKhiXoa(dataT, flagDeCatLon);
            return dataT;
        }
        public static void TinhToanLaiKhiXoa(DataTable tblPivot, int flagDeCatLon = -1)
        {
            // DataTable tblPivot = dgrKHDongThung.DataSource as DataTable;
            int sttThungCur = 0;
            int tuThungOld = 0;
            int denThungOld = 0;
            bool flagFirst = false;
            foreach (DataRow drKH in tblPivot.Rows)
            {
                if (flagDeCatLon == -1)
                {
                    if (drKH["Stt_Size"].ToString() == "1") continue;
                }
                else if (flagDeCatLon == 1) continue;

                if(drKH["KieuLap"].ToString() == "1")
                {                    
                    if (Convert.ToInt16(drKH["TuThung"]) != 1)
                    {
                        drKH["TuThung"] = denThungOld + 1;
                        drKH["DenThung"] = Convert.ToInt32(drKH["SLThung"]) + denThungOld;                        
                    }               
                    denThungOld = Convert.ToInt32(drKH["DenThung"]);
                    continue;
                }
                else
                {
                    int tuThung = 0, denThung = 0;
                    if (Convert.ToInt16(drKH["TuThung"]) == sttThungCur && flagFirst)
                    {
                        drKH["TuThung"] = tuThungOld;
                        drKH["DenThung"] = denThungOld;
                        continue;
                    }
                    if (!flagFirst) sttThungCur = Convert.ToInt32(drKH["TuThung"]) - 1;
                    tuThung = sttThungCur + 1;
                    denThung = Convert.ToInt32(drKH["SLThung"]) + sttThungCur;
                    sttThungCur = denThung;

                    drKH["TuThung"] = tuThung;
                    drKH["DenThung"] = denThung;
                    tuThungOld = tuThung;
                    denThungOld = denThung;
                    flagFirst = true;
                }
               


            }
            //SaveKHDT(tblPivot);
        }
        public static void CopyDataRow(DataRow sourceRow, DataRow destinationRow)
        {
            // Loop through columns and copy data
            foreach (DataColumn column in sourceRow.Table.Columns)
            {
                destinationRow[column.ColumnName] = sourceRow[column.ColumnName];
            }
        }
        public static void ProcessSttTrung(DataTable tbl)
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
                    TotalPiece = z.Sum(row => Convert.ToInt32(row["TotalPiece"])),
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
        }
        public static void ProcessSttTrung1(DataTable tbl)
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
                }).ToList();
                foreach (var itemA in SumSL)
                {
                    var tempA = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == itemA.SttThung && x["MaPKLDisplay"].ToString() == itemA.MaPKLDisplay);
                    foreach (DataRow dr in tempA)
                    {
                        dr["SoLuong"] = itemA.SoLuong;
                        dr["ToTalPiece"] = itemA.TotalPiece;
                    }
                }
            }
        }
        public static void ProcessChangeNW_GW(DataTable tbl, DataRow dr)
        {
            var tempA = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == dr["SttThung"].ToString());
            foreach (var item in tempA)
            {
                item["TrongLuong"] = dr["TrongLuong"];
                item["KhoiLuong"] = dr["KhoiLuong"];
            }
        }
        public static void SumGroup(DataTable dt, CustomSummaryEventArgs e, bool flagPKL = false)
        {
            object _valueSumaryCaton = 0;
            if (e.Item == null)
            {
                return;
            }
            GridSummaryItem item = e.Item as GridSummaryItem;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                List<string> lstSum = new List<string>() { "SLThung","SLDDThung", "SLDNK", "TotalPiece", "SoLuong", "SLNhapKho", "SLTDaXuat", "SLNhapTK", "SLXuat", "SLChuyen", "SLDaChuyenKho", "SLTXuatHang_CC" };
                List<string> lstExcept = new List<string>() { "TrongLuong", "KhoiLuong" };
                if (lstSum.Contains(item.FieldName))
                {
                    _valueSumaryCaton = SumItemCaton(dt, item.FieldName, true, flagPKL);
                    e.TotalValue = _valueSumaryCaton;
                }
                else if (lstExcept.Contains(item.FieldName))
                {
                    _valueSumaryCaton = SumItemCaton(dt, item.FieldName, false, flagPKL);
                    e.TotalValue = _valueSumaryCaton;
                }
            }
        }
        private static object SumItemCaton(DataTable dt, string col, bool typeInt, bool flagPKL = false)
        {
            try
            {
                object SumCaton = 0;
                // var dt = dgrKHDongThung.DataSource as DataTable;
                if (dt != null && dt.Rows.Count > 0)
                {
                    //EnumerableRowCollection<DataRow> query;
                    var query = dt.AsEnumerable().Select(x =>
                       new
                       {
                           SttThung = x["SttThung"],
                           MaPKL = flagPKL ? x["MaPKLDisplay"] : "",
                           SLThung = x[col].ToString() == "" ? 0 : x[col]
                       }).ToList().Distinct();

                    SumCaton = query.Sum(item => typeInt ? Convert.ToInt32(item.SLThung) : Convert.ToDouble(item.SLThung));
                    return SumCaton;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public static bool MergeAllowChangeValue(BandedGridView bandview, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            List<string> lstMerge = new List<string> { "TrongLuong", "KhoiLuong", "Chon" };
            if (lstMerge.Contains(e.Column.FieldName))
            {
                var preSTTThung = bandview.GetRowCellValue(e.RowHandle - 1, "SttThung");
                var curSTTThung = bandview.GetRowCellValue(e.RowHandle, "SttThung");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                {
                    return true;
                }
            }
            return false;
        }
        public static void Merge(object sender, CellMergeEventArgs e, bool flagPKL = false)
        {
            GridView view = sender as GridView;
            bool ShouldMerge(string fieldName)
            {
                e.Handled = true;
                e.Merge = false;
                if (!object.Equals(e.CellValue1, e.CellValue2))
                    return false;
                string id1 = view.GetRowCellValue(e.RowHandle1, "SttThung").ToString();
                string id2 = view.GetRowCellValue(e.RowHandle2, "SttThung").ToString();
                if (flagPKL)
                {
                    string pkl1 = view.GetRowCellValue(e.RowHandle1, "MaPKLDisplay").ToString();
                    string pkl2 = view.GetRowCellValue(e.RowHandle2, "MaPKLDisplay").ToString();
                    return id1 == id2 && pkl1 == pkl2;
                }

                return id1 == id2;
            }
            List<string> lstMerge = new List<string>() { "SLNhap", "SLThung", "TuThung", "DenThung", "TrongLuong", "KhoiLuong", "TotalPiece",
                                                            "SoLuong", "SLNhapKho", "SLTDaXuat", "SLNhapTK","SLChuyen","SLDaChuyenKho", "SLTXuatHang_CC","SLDDThung", "SLDNK", "SLTCD", };
            if (lstMerge.Contains(e.Column.FieldName))
            {
                e.Merge = ShouldMerge(e.Column.FieldName);
            }
        }
        public static int _columns;
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
            if (!flagFilter)
                KHDongThungLibP.ProcessSttTrung1(dtPKLXuatHang);

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
            int totalPCB_Pack = dtPKLXuatHang.AsEnumerable().Sum(x => Convert.ToInt32(x["PCB_Pack"]));
            if (totalPCB_Pack != 0)
            {
                range = worksheet.Cells[9, colum]; range.Value = "PCB_Pack "; worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[9, colum]; range.Value = "Pack_Ctn "; worksheet.Column(colum).AutoFit(); colum++;
            }
            range = worksheet.Cells[9, colum]; range.Value = "QTY "; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "Carton "; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "ToTal Pieces"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "NW"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "GW"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "Carton"; worksheet.Column(colum).AutoFit(); colum++;
            _columns = colum;
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
                if (totalPCB_Pack != 0)
                {
                    range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["PCB_Pack"]); worksheet.Column(colum).AutoFit(); colum++;
                    range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["Pack_Ctn"]); worksheet.Column(colum).AutoFit(); colum++;
                }

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
                        .Sum(y =>
                        {
                            int sizeValue = Convert.ToInt32(y[colums].ToString() == "" ? 0 : y[colums]);
                            int quantity = Convert.ToInt32(y["SLThung"]);
                            int PackCtn = Convert.ToInt32(y["Pack_Ctn"]);
                            return y["PCB_Pack"].ToString() != "0" ? sizeValue * quantity * PackCtn : sizeValue * quantity;
                        });
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
        public static void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA, DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repotxtN0,bool IsCheckSumSize = true)
        {
            ClearBand(gbSizeA);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
                if (!CheckExistBand(_sizeID, gbSizeA)) continue;

                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = _size + "@" + _sizeID;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = repotxtN0;
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                if (IsCheckSumSize)
                {
                    if (col.FieldName.Contains("@"))
                    {
                        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                        itemSize.FieldName = col.FieldName;
                        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        itemSize.DisplayFormat = "{0:n0}";
                        itemSize.ShowInGroupColumnFooter = col;
                        grvShared.GroupSummary.Add(itemSize);
                    }
                    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrName.Length > 1)
                    {
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    }
                }
                

               

                // Thêm cột vào grid
                grvShared.Columns.AddRange(new BandedGridColumn[] { col });

                // Tạo và thêm GridBand vào grid
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }

        public static void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }

        public static bool CheckExistBand(string size, GridBand gbSizeA)
        {
            GridBand gbCheck = gbSizeA.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            return gbCheck == null;
        }
        public static void EventShowingEditor(BandedGridView bandedGridViewKHDT, CancelEventArgs e, bool flagEvent = false, DataColumnCollection lstCol = null)
        {
            var _maDVSX = bandedGridViewKHDT.GetFocusedRowCellValue("MaDVSX").ToString();              
            if (!KHDongThungLib.CheckRole(_maDVSX))
            {
                e.Cancel = true;
                MessageBox.Show("User không có quyền chỉnh sửa!", "Thông báo");
                return;
            }

            if (bandedGridViewKHDT.FocusedColumn.FieldName == "TenDVSX" || bandedGridViewKHDT.FocusedColumn.FieldName == "DauSize" ||
                bandedGridViewKHDT.FocusedColumn.FieldName == "TenMau"
                )
            {
                if ((bool)bandedGridViewKHDT.GetFocusedRowCellValue("IsSave") == true)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }            
            if (bandedGridViewKHDT.FocusedColumn.FieldName == "Chon")
            {
                if (!(bool)bandedGridViewKHDT.GetFocusedRowCellValue("IsThungLe"))
                {
                    e.Cancel = true;
                    return;
                }               
                else if (flagEvent || (lstCol != null && lstCol.Contains("IsDongThung1")))
                {
                    if(bandedGridViewKHDT.GetFocusedRowCellValue("IsDongThung1").ToString() == "")
                    {
                        e.Cancel = false;
                        return;
                    }
                    if (bandedGridViewKHDT.GetFocusedRowCellValue("IsDongThung1").ToString() == "1")
                    {
                        e.Cancel = true;
                        MessageBox.Show("Thùng đã được đóng không cho phép gọp!", "Thông báo");
                        return;
                    }
                    else if(bandedGridViewKHDT.GetFocusedRowCellValue("IsXuatHang1").ToString() == "1")
                    {
                        e.Cancel = true;
                        MessageBox.Show("Thùng đã được lập kế hoạch không cho phép gọp!", "Thông báo");
                        return;
                    }                   
                }
                else
                {
                    e.Cancel = false;
                    return;
                }


            }
        }
        public static void CustomColumnDisplay(DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DauSize") return;
            if (e.Value != null && (e.Value.ToString() == "0" || e.Value.ToString() == "")) e.DisplayText = "-";
        }

    }
}
