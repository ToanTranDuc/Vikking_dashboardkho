using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    public class KHDongThungLib
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
            tbl.Columns.Add("Store", typeof(string));
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
            tbl.Columns.Add("PCB_Pack", typeof(int));
            tbl.Columns.Add("Pack_Ctn", typeof(int));
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
            tbl.Columns.Add("IsStoreThieu", typeof(int));
            tbl.Columns.Add("Destination", typeof(string));
            tbl.Columns.Add("DeliveryTo", typeof(string));
            tbl.Columns.Add("Terms", typeof(string));
            tbl.Columns.Add("CountryOfOrigin", typeof(string));
            tbl.Columns.Add("StyleName", typeof(string));
            tbl.Columns.Add("DeptNo", typeof(string));
            tbl.Columns.Add("KieuLapPCB", typeof(int));
            tbl.Columns.Add("KieuLap", typeof(int));
            tbl.Columns.Add("Cont", typeof(string));
            tbl.Columns.Add("MaDH_XH", typeof(string));
            tbl.Columns.Add("POID_XH", typeof(string));
            tbl.Columns.Add("NVien", typeof(string));
            return tbl;
        }
        public static DataTable CreateTblCaiDatBarcodeNew()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("BarCodeChan", typeof(string));
            dt.Columns.Add("SoLuongChan", typeof(int));
            dt.Columns.Add("BarCodeLe", typeof(string));
            dt.Columns.Add("SoLuongLe", typeof(int));
            dt.Columns.Add("BarCodeBao", typeof(string));
            dt.Columns.Add("SoLuongBao", typeof(int));
            dt.Columns.Add("BarCodeTheBai", typeof(string));
            dt.Columns.Add("SoLuongTheBai", typeof(int));
            dt.Columns.Add("Ecode", typeof(string));
            dt.Columns.Add("SoLuongEcode", typeof(int));
            dt.Columns.Add("BarCodeXacNhan", typeof(string));
            dt.Columns.Add("SoLuongXacNhan", typeof(int));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            return dt;
        }
        public static DataTable CreateTblPackage()
        {
            var dt = new DataTable("dtData");
            dt.Columns.Add("ColGroup", typeof(string));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("TuThung", typeof(int));
            dt.Columns.Add("DenThung", typeof(int));
            dt.Columns.Add("MaDVSX", typeof(string));
            dt.Columns.Add("TenDVSX", typeof(string));
            dt.Columns.Add("MaLenh", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("DotSX", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("PO", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("PCB_Pack", typeof(int));
            dt.Columns.Add("Pack_Ctn", typeof(int));
            dt.Columns.Add("SLThung", typeof(int));
            dt.Columns.Add("TotalPiece", typeof(int));
            dt.Columns.Add("TrongLuong", typeof(float));
            dt.Columns.Add("KhoiLuong", typeof(float));
            //dt.Columns.Add("KhoiLuongT", typeof(float)); //Thoai them ^^
            dt.Columns.Add("TrongLuongA", typeof(float));
            //dt.Columns.Add("TrongLuongT", typeof(float));//Thoai them ^^
            dt.Columns.Add("KyHieu", typeof(string));
            dt.Columns.Add("ChieuDai", typeof(string));
            dt.Columns.Add("ChieuRong", typeof(string));
            dt.Columns.Add("ChieuCao", typeof(string));
            dt.Columns.Add("IsThungLe", typeof(bool));
            dt.Columns.Add("SttThung", typeof(int));
            dt.Columns.Add("Chon", typeof(bool));
            dt.Columns.Add("IsSave", typeof(bool));
            dt.Columns.Add("KieuLap", typeof(string));
            dt.Columns.Add("KieuLapPCB", typeof(string));
            dt.Columns.Add("Stt_Size", typeof(string));
            dt.Columns.Add("IsStoreThieu", typeof(bool));
            dt.Columns.Add("Barcode", typeof(string));
            return dt;
        }
        public static DataTable GopThung(DataTable data, int flagDeCatLon = -1, bool flagCheckTheoKhu = false)
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

                    //var trongluong1 = Convert.ToDouble(drRowA["TrongLuong"]);
                    //var trongluong2 = Convert.ToDouble(drNew["TrongLuong"]);
                    var khoiLuong1 = drRowA["KhoiLuong"] == DBNull.Value ? 0 : Convert.ToDouble(drRowA["KhoiLuong"]);
                    var khoiLuong2 = drNew["KhoiLuong"] == DBNull.Value ? 0 : Convert.ToDouble(drNew["KhoiLuong"]);
                    var SoLuong1 = Convert.ToInt16(drRowA["SoLuong"]);
                    var SoLuong2 = Convert.ToInt16(drNew["SoLuong"]);
                    var TotalPiece1 = Convert.ToInt16(drRowA["TotalPiece"]);
                    var TotalPiece2 = Convert.ToInt16(drNew["TotalPiece"]);


                    drNew["TuThung"] = drRowA["TuThung"];
                    drNew["DenThung"] = drRowA["DenThung"];
                    drNew["SttThung"] = drRowA["SttThung"];
                    drNew["TrongLuong"] = drRowA["TrongLuong"] = khoiLuong1 + khoiLuong2 + (dr["TrongLuongA"] == DBNull.Value ? 0 : Convert.ToDouble(dr["TrongLuongA"]));
                    drNew["KhoiLuong"] = drRowA["KhoiLuong"] = khoiLuong1 + khoiLuong2;
                    drNew["IsThungLe"] = true;
                    if (drNew["POID"].ToString() == drRowA["POID"].ToString() && drNew["MaDVSX"].ToString() == drRowA["MaDVSX"].ToString() && drNew["MaLenh"].ToString() == drRowA["MaLenh"].ToString() && drNew["DauSizeID"].ToString() == drRowA["DauSizeID"].ToString()
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
                        //drNew["SoLuong"] = dataT.Rows[index - 1]["SoLuong"] = SoLuong1 + SoLuong2;
                        //drNew["TotalPiece"] = dataT.Rows[index - 1]["TotalPiece"] = TotalPiece1 + TotalPiece2;
                    }
                    dataT.Rows.InsertAt(drNew, index);

                    foreach (DataRow drT in dataT.Rows)
                    {
                        //if(drT["SttThung"].ToString() == drRowA["SttThung"].ToString())
                        //{                            
                        //    drT["SoLuong"] = SoLuong1 + SoLuong2;
                        //    drT["TotalPiece"] =  TotalPiece1 + TotalPiece2;
                        //}
                        drT["Chon"] = false;
                    }
                    index += 1;
                }
            }
            var dtTrung = dataT.AsEnumerable().AsEnumerable().Where(x => x["SttThung"].ToString() == drRowA["SttThung"].ToString());
            if (dtTrung.Count() > 1)
            {
                int SoLuong = 0, TotalPiece = 0;
                var dtTrungCV = dtTrung.CopyToDataTable();
                foreach (DataRow dr in dtTrungCV.Rows)
                {
                    foreach (DataColumn dc in dtTrungCV.Columns)
                    {
                        if (!dc.ColumnName.Contains('@')) continue;
                        SoLuong += Convert.ToInt16(dr[dc] == DBNull.Value ? 0 : dr[dc]);
                    }
                }
                foreach (DataRow dr in dtTrung)
                {
                    dr["TotalPiece"] = SoLuong;
                    dr["SoLuong"] = SoLuong;
                }
            }
            //dataT = !flagCheckTheoKhu ? dataT : dataT.AsEnumerable().Where(x => x["MaDVSX"].ToString() == drRowA["MaDVSX"].ToString()).CopyToDataTable();
            // gridView1.SetRowCellValue(0, "ColumnName", 0);
            TinhToanLaiKhiXoa(dataT, flagDeCatLon, flagCheckTheoKhu, drRowA["MaDVSX"].ToString());
            return dataT;
        }
        public static void TinhToanLaiKhiXoa(DataTable tblPivot, int flagDeCatLon = -1, bool flagCheckTheoKhu = false, string maDVSX = "", string TuThungStart = "1")
        {
            // DataTable tblPivot = dgrKHDongThung.DataSource as DataTable;
            int sttThungCur = 0;
            int TuThungCur = TuThungStart == "1" ? 0 : Convert.ToInt32(TuThungStart) - 1;
            int tuThungOld = 0;
            int denThungOld = 0;
            int tuThung = 0, denThung = 0;
            bool flagFirst = false;
            if (flagCheckTheoKhu)
            {
                TuThungCur = Convert.ToInt32(tblPivot.Rows[0]["TuThung"]) - 1;
            }
            foreach (DataRow drKH in tblPivot.Rows)
            {
                if (flagDeCatLon == -1)
                {
                    if (drKH["Stt_Size"].ToString() == "1") continue;
                }
                else if (flagDeCatLon == 1) continue;

                if (drKH["KieuLap"].ToString() == "1")
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
                    if (drKH["MaDVSX"].ToString() != maDVSX && flagCheckTheoKhu) continue;

                    if (Convert.ToInt32(drKH["SttThung"]) == sttThungCur)
                    {
                        drKH["TuThung"] = tuThung;
                        drKH["DenThung"] = denThung;
                        continue;
                    }
                    tuThung = 0; denThung = 0;
                    if (!flagFirst)
                    {
                        if (tblPivot.Rows.Count > 1)
                        {
                            if (tblPivot.Rows[1]["TuThung"].ToString() != TuThungStart)
                                TuThungCur = Convert.ToInt32(drKH["TuThung"]) - 1;
                        }
                    }
                    //if (!flagFirst && drKH["TuThung"] != "1") TuThungCur = Convert.ToInt32(drKH["TuThung"]) - 1;
                    ////else if (!flagFirst && drKH["TuThung"] != "1") TuThungCur = 0;
                    //else if (!flagFirst)
                    //{
                    //    if (tblPivot.Rows.Count > 1)
                    //    {
                    //        if (tblPivot.Rows[1]["TuThung"].ToString() != "1")
                    //            TuThungCur = Convert.ToInt32(drKH["TuThung"]) - 1;
                    //        //else
                    //        //    TuThungCur = Convert.ToInt32(drKH["TuThung"]) - 1;
                    //    }
                    //}
                    //
                    tuThung = TuThungCur + 1;
                    denThung = Convert.ToInt32(drKH["SLThung"]) + TuThungCur;
                    TuThungCur = denThung;
                    sttThungCur = Convert.ToInt32(drKH["SttThung"]);
                    drKH["TuThung"] = tuThung;
                    drKH["DenThung"] = denThung;
                    tuThungOld = tuThung;
                    denThungOld = denThung;
                    flagFirst = true;
                }
            }
            //SaveKHDT(tblPivot);
        }

        public static void TinhToanLaiKhiXoaV2(DataTable tblPivot, int flagDeCatLon = -1, bool flagCheckTheoKhu = false, string maDVSX = "")
        {
            // DataTable tblPivot = dgrKHDongThung.DataSource as DataTable;
            int sttThungCur = 0;
            int TuThungCur = 0;
            int tuThungOld = 0;
            int denThungOld = 0;
            bool flagFirst = false;
            if (flagCheckTheoKhu)
            {
                TuThungCur = Convert.ToInt32(tblPivot.Rows[0]["TuThung"]) - 1;
            }
            foreach (DataRow drKH in tblPivot.Rows)
            {
                if (drKH["SLThung"].ToString() == "0") drKH["SLThung"] = 1;
                if (flagDeCatLon == -1)
                {
                    if (drKH["Stt_Size"].ToString() == "1") continue;
                }
                else if (flagDeCatLon == 1) continue;

                if (drKH["KieuLap"].ToString() == "1")
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
                    if (drKH["MaDVSX"].ToString() != maDVSX && flagCheckTheoKhu) continue;
                    int tuThung = 0, denThung = 0;
                    if (Convert.ToInt32(drKH["SttThung"]) == sttThungCur)
                    {
                        continue;
                    }
                    if (!flagFirst)
                    {
                        if (tblPivot.Rows.Count > 1)
                        {
                            if (tblPivot.Rows[1]["TuThung"].ToString() != "1")
                                TuThungCur = Convert.ToInt32(drKH["TuThung"]) - 1;
                        }
                    }

                    tuThung = TuThungCur + 1;
                    denThung = Convert.ToInt32(drKH["SLThung"]) + TuThungCur;
                    TuThungCur = denThung;
                    sttThungCur = Convert.ToInt32(drKH["SttThung"]);
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
                    TotalPiece = z.Sum(row => Convert.ToInt32(row["ToTalPiece"])),
                    KhoiLuong = z.Sum(row => Convert.ToDouble(row["KhoiLuong"])),
                    //KhoiLuongT = z.Sum(row => Convert.ToDouble(row["KhoiLuongT"])),
                    KhoiLuongT = z.Sum(row => decimal.TryParse(row["KhoiLuongT"].ToString(), out var val) ? val : 0m),

                }).ToList();
                // if (SumSL.Count < 2) continue;
                foreach (var itemA in SumSL)
                {
                    var tempA = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == itemA.SttThung.ToString());
                    foreach (DataRow dr in tempA)
                    {
                        dr["SoLuong"] = itemA.SoLuong;
                        dr["ToTalPiece"] = itemA.TotalPiece;
                        var KhoiLuong = dr["KieuLapPCB"].ToString() != "0" ? Convert.ToDouble(dr["KhoiLuong"]) : itemA.KhoiLuong;
                        if (dr["KieuLapPCB"].ToString() == "4") KhoiLuong = itemA.KhoiLuong;

                        var KhoiLuongT = dr["KieuLapPCB"].ToString() != "0" ? (decimal.TryParse(dr["KhoiLuongT"].ToString(), out var val) ? val : 0m) : itemA.KhoiLuongT;
                        if (dr["KieuLapPCB"].ToString() == "4") KhoiLuongT = itemA.KhoiLuongT;

                        dr["KhoiLuong"] = KhoiLuong;
                        dr["TrongLuong"] = KhoiLuong + Convert.ToDouble(dr["TrongLuongA"]);
                        dr["KhoiLuongT"] = KhoiLuongT;
                        dr["TrongLuongT"] = (double)KhoiLuongT + Convert.ToDouble(dr["TrongLuongA"]);
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
                                        MaDH = row["MaDH"].ToString(),
                                        SttThung = row["SttThung"].ToString(),
                                        MaPKLDisplay = row["MaPKLDisplay"].ToString(),
                                    })
                                    .Where(ageGroup => ageGroup.Count() > 1).ToList();
            foreach (var item in tblSttTrung)
            {
                var SumSL = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == item.Key.SttThung && x["MaPKLDisplay"].ToString() == item.Key.MaPKLDisplay).GroupBy(y => new
                {
                    MaDH = y["MaDH"].ToString(),
                    SttThung = y["SttThung"].ToString(),
                    MaPKLDisplay = y["MaPKLDisplay"].ToString(),
                }).Select(z => new
                {
                    SttThung = z.Key.SttThung,
                    MaPKLDisplay = z.Key.MaPKLDisplay,
                    SoLuong = z.Sum(row => Convert.ToInt32(row["SoLuong"])),
                    TotalPiece = z.Sum(row => Convert.ToInt32(row["ToTalPiece"])),
                    KhoiLuong = z.Sum(row => Convert.ToDouble(row["KhoiLuong"])),
                    KhoiLuongT = z.Sum(row => Convert.ToDouble(row["KhoiLuongT"])),
                }).ToList();
                foreach (var itemA in SumSL)
                {
                    var tempA = tbl.AsEnumerable().Where(x => x["SttThung"].ToString() == itemA.SttThung && x["MaPKLDisplay"].ToString() == itemA.MaPKLDisplay);
                    foreach (DataRow dr in tempA)
                    {
                        dr["SoLuong"] = itemA.SoLuong;
                        dr["ToTalPiece"] = itemA.TotalPiece;
                        var KhoiLuong = dr["KieuLapPCB"].ToString() == "1" ? Convert.ToDouble(dr["KhoiLuong"]) : itemA.KhoiLuong;
                        if (dr["KieuLapPCB"].ToString() == "4") KhoiLuong = itemA.KhoiLuong;

                        var KhoiLuongT = dr["KieuLapPCB"].ToString() == "1" ? Convert.ToDouble(dr["KhoiLuongT"]) : itemA.KhoiLuongT;
                        if (dr["KieuLapPCB"].ToString() == "4") KhoiLuong = itemA.KhoiLuongT;

                        dr["KhoiLuong"] = KhoiLuong;
                        dr["TrongLuong"] = KhoiLuong + Convert.ToDouble(dr["TrongLuongA"]);
                        dr["KhoiLuongT"] = KhoiLuongT;
                        dr["TrongLuongT"] = KhoiLuongT + Convert.ToDouble(dr["TrongLuongA"]);
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
        public static void SumGroup(DataTable dt, CustomSummaryEventArgs e, bool flagPKL = false, bool flagCont = false, bool flagSMS = false)
        {
            object _valueSumaryCaton = 0;
            if (e.Item == null)
            {
                return;
            }
            GridSummaryItem item = e.Item as GridSummaryItem;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                List<string> lstSum = new List<string>() { "SLThung", "STDaQuet", "SLDDThung", "SLDNK", "TotalPiece", "SoLuong", "SLNhapKho", "SLTDaXuat", "SLNhapTK", "SLXuat", "SLChuyen", "SLDaChuyenKho", "SLTXuatHang_CC" };
                List<string> lstExcept = new List<string>() { "TrongLuong", "KhoiLuong", "TrongLuongT", "KhoiLuongT", "CBM" };
                if (lstSum.Contains(item.FieldName))
                {
                    _valueSumaryCaton = SumItemCaton(dt, item.FieldName, true, flagPKL, flagCont, flagSMS);
                    e.TotalValue = _valueSumaryCaton;
                }
                else if (lstExcept.Contains(item.FieldName))
                {
                    _valueSumaryCaton = SumItemCaton(dt, item.FieldName, false, flagPKL, false, flagSMS);
                    if (item.FieldName == "CBM")
                        e.TotalValue = Math.Round(Convert.ToDouble(_valueSumaryCaton), 2);
                    else e.TotalValue = Math.Round(Convert.ToDouble(_valueSumaryCaton), 0);
                }
            }
        }
        private static object SumItemCaton(DataTable dt, string col, bool typeInt, bool flagPKL = false, bool flagCont = false, bool flagSMS = false)
        {
            try
            {
                if (flagCont)
                {

                }
                object SumCaton = 0;
                // var dt = dgrKHDongThung.DataSource as DataTable;
                if (dt != null && dt.Rows.Count > 0)
                {
                    //EnumerableRowCollection<DataRow> query;
                    var query = dt.AsEnumerable().Select(x =>
                       new
                       {
                           MaDH = flagSMS ? "" : x["MaDH"],
                           SttThung = x["SttThung"],
                           MaPKL = flagPKL ? x["MaPKLDisplay"] : "",
                           SLThung = x[col].ToString() == "" ? 0 : x[col],
                           SLThungReal = x["SLThung"].ToString() == "" ? 0 : x["SLThung"],
                           Cont = flagCont ? x["Cont"] : ""
                       }).Distinct().ToList();

                    if (col == "CBM")
                        SumCaton = Math.Round(query.Sum(item => Convert.ToDouble(item.SLThung) * Convert.ToInt16(item.SLThungReal)), 2);
                    else SumCaton = query.Sum(item => typeInt ? Convert.ToInt32(item.SLThung) : Convert.ToDouble(item.SLThung));
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
            List<string> lstMerge = new List<string> { "TrongLuong", "KhoiLuong", "Chon", "KyHieu", "TrongLuongT", "KhoiLuongT", "CBM" };
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
                    if (id1 == id2 && id2 == "7")
                    {

                    }
                    string pkl1 = view.GetRowCellValue(e.RowHandle1, "MaPKLDisplay").ToString();
                    string pkl2 = view.GetRowCellValue(e.RowHandle2, "MaPKLDisplay").ToString();
                    return id1 == id2 && pkl1 == pkl2;
                }
                return id1 == id2;
            }
            List<string> lstMerge = new List<string>() { "SLThung", "TuThung", "DenThung", "TrongLuong", "KhoiLuong","TrongLuongT","KhoiLuongT", "TotalPiece","CBM",
                                                            "SoLuong", "SLNhapKho", "SLTDaXuat", "SLNhapTK","SLChuyen","SLDaChuyenKho", "SLTXuatHang_CC","SLDDThung", "SLDNK", "KyHieu" };
            if (lstMerge.Contains(e.Column.FieldName))
            {
                e.Merge = ShouldMerge(e.Column.FieldName);
            }
        }
        public static int _columns;
        public static void dtXuatEX(DataTable dtPKLXuatHang, ExcelWorksheet worksheet, bool flagFilter = true,
                                   int column1 = 9, DataTable dtCodeSize = null, bool flagCodeSize = false, bool flagArt = false, bool flagMaHang = true,
                                   DataTable dtKhoiLuongSize = null, bool isDongThung = true, bool checkKLSize = false, bool checkpo = false, bool hiddenDVSX = false, bool ShowKgThung = false, bool ShowBarCode = false)
        {
            if (dtPKLXuatHang.Rows.Count == 0) return;
            flagMaHang = true;
            string PathLoGo = "";
            ExcelRange range = worksheet.Cells;
            string phapdanhCty = dtPKLXuatHang.Rows[0]["TenCty"].ToString();
            if (phapdanhCty.ToString() == "CÔNG TY TNHH VIKING VIỆT NAM")
                PathLoGo = getImgPath("texgiang.jpg");
            else PathLoGo = getImgPath("texgiang1.jpg");
            range = worksheet.Cells["A1:B3"]; range.Merge = true;
            Image image = Image.FromFile(PathLoGo);
            OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
            picture.SetPosition(0, 0, 0, 0);
            picture.SetSize(100, 60);
            range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = phapdanhCty; range.Style.Font.Bold = true;
            List<string> columnNamesWithSize = dtPKLXuatHang.Columns.Cast<DataColumn>()
                        .Where(column => column.ColumnName.Contains("@"))
                        .Select(column => column.ColumnName)
                        .Distinct()
                        .ToList();
            int colum = 1;
            int colAdd = 0;
            int staticCol = column1;

            // bool flagArt = dtPKLXuatHang.Rows[0]["ArtSize"].ToString() != "" ? true : false;

            var KieuLapPCB = dtPKLXuatHang.Rows[0]["KieuLapPCB"].ToString();
            if (dtCodeSize != null && flagCodeSize)
            {
                colAdd = 1;
            }
            int columnSize = 7;
            if (!isDongThung && checkKLSize)
            {
                columnSize = 8;
                foreach (var colums in columnNamesWithSize)
                {
                    var splitColum = colums.Split('@');
                    var drTempKLSize = dtKhoiLuongSize.AsEnumerable().Where(x => x["SizeID"].ToString() == splitColum[1].ToString() && x["DauSizeID"].ToString() == dtPKLXuatHang.Rows[0]["DauSizeID"].ToString()).FirstOrDefault();
                    string _khoiLuongSize = drTempKLSize is null ? "" : drTempKLSize["SL_KhoiLuong"].ToString();
                    worksheet.Cells[column1, columnSize].Value = _khoiLuongSize;
                    worksheet.Cells[column1, columnSize].Style.Font.Color.SetColor(Color.FromArgb(27, 109, 201));
                    columnSize++;
                }
                column1++;
            }

            range = worksheet.Cells[column1, colum, column1 + colAdd, colum + 1]; range.Merge = true; range.Value = "Carton Number"; range.Style.Font.Bold = true; colum += 2;
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Style Name"; range.Style.Font.Bold = true; colum++;
            if (KieuLapPCB == "4")
            {
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Đợt"; range.Style.Font.Bold = true; colum++;
            }
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "PO"; range.Style.Font.Bold = true; worksheet.Column(colum).AutoFit(); colum++;


            if (KieuLapPCB == "3")
            {
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Store"; range.Style.Font.Bold = true; colum++;
            }

            if (!hiddenDVSX)
            {
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Supplier"; range.Style.Font.Bold = true; colum++;
            }

            if (flagArt)
            {
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Art";
                range.Style.Font.Bold = true; colum++;
            }
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Size/ \n Inseam"; range.Style.Font.Bold = true; range.Style.WrapText = true; colum++;
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Color"; range.Style.Font.Bold = true; colum++;
            foreach (var colums in columnNamesWithSize)
            {
                var splitColum = colums.Split('@');
                if (colAdd == 1)
                {
                    var dtTempcodeSize = dtCodeSize.AsEnumerable().Where(x => x["SizeID"].ToString() == splitColum[1].ToString() && x["MaMau"].ToString() == dtPKLXuatHang.Rows[0]["ColorID"].ToString()).FirstOrDefault();
                    string codeSize = "";
                    if (dtTempcodeSize != null)
                    {
                        codeSize = dtTempcodeSize["CodeSize"].ToString();
                    }
                    worksheet.Cells[column1 + colAdd, colum].Value = codeSize;
                    worksheet.Cells[column1 + colAdd, colum].Style.Font.Color.SetColor(Color.FromArgb(27, 109, 201));
                }

                worksheet.Cells[column1, colum].Value = splitColum[0].ToString();
                worksheet.Cells[column1, colum].Style.Font.Bold = true;
                colum++;
            }

            if (!flagFilter)
                KHDongThungLib.ProcessSttTrung1(dtPKLXuatHang);
            var uniqueValuesA = dtPKLXuatHang.AsEnumerable().
         Select(x => new
         {
             MaPKL = x["MaPKLDisplay"],
             SttThung = x["SttThung"],
             SLThung = x["SLThung"],
             SoLuong = x["SoLuong"],
             CBM = (x["CBM"] == null || x["CBM"].ToString() == "") ? 0 : (Convert.ToSingle(x["CBM"]) * Convert.ToSingle(x["SLThung"])),
         }).Distinct().ToList();
            var uniqueValues = dtPKLXuatHang.AsEnumerable().
            Select(x => new
            {
                MaPKL = x["MaPKLDisplay"],
                SttThung = x["SttThung"],
                SLThung = x["SLThung"],
                TotalPiece = x["TotalPiece"],
                SoLuong = x["SoLuong"],
                TrongLuongT = x["TrongLuongT"],
                KhoiLuongT = x["KhoiLuongT"],
                TrongLuong = x["TrongLuong"],
                KhoiLuong = x["KhoiLuong"],
                KyHieu = x["KyHieu"],
            }).Distinct().ToList();

            int totalSLThung = uniqueValues.Sum(x => Convert.ToInt32(x.SLThung));
            int totalTotalPiece = uniqueValues.Sum(x => Convert.ToInt32(x.TotalPiece));
            int totalSoLuong = uniqueValues.Sum(x => Convert.ToInt32(x.SoLuong));
            float totalTrongLuongT = uniqueValues.Sum(x => Convert.ToSingle(x.TrongLuongT));
            float roundedTotalTrongLuongT = (float)Math.Round(totalTrongLuongT, 0);
            float totalKhoiLuongT = uniqueValues.Sum(x => Convert.ToSingle(x.KhoiLuongT));
            float roundedTotalKhoiLuongT = (float)Math.Round(totalKhoiLuongT, 0);
            float totalTrongLuong = uniqueValues.Sum(x => Convert.ToSingle(x.TrongLuong));
            float roundedTotalTrongLuong = (float)Math.Round(totalTrongLuong, 0);
            float totalCBM = uniqueValuesA.Sum(x => Convert.ToSingle(x.CBM));
            float roundedCBM = (float)Math.Round(totalCBM, 2);

            float totalKhoiLuong = uniqueValues.Sum(x => Convert.ToSingle(x.KhoiLuong));
            float roundedTotalKhoiLuong = (float)Math.Round(totalKhoiLuong, 0);
            List<string> sumToTalAll;
            if (ShowKgThung) sumToTalAll = new List<string>() { totalSoLuong.ToString(), totalSLThung.ToString(), totalTotalPiece.ToString(), roundedTotalKhoiLuongT.ToString(), roundedTotalTrongLuongT.ToString(), roundedTotalKhoiLuong.ToString(), roundedTotalTrongLuong.ToString(), roundedCBM.ToString() };
            else sumToTalAll = new List<string>() { totalSoLuong.ToString(), totalSLThung.ToString(), totalTotalPiece.ToString(), roundedTotalKhoiLuong.ToString(), roundedTotalTrongLuong.ToString(), roundedCBM.ToString() };
            int totalPCB_Pack = dtPKLXuatHang.AsEnumerable().Sum(x => Convert.ToInt32(x["PCB_Pack"]));
            if (totalPCB_Pack != 0)
            {
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Pcs/ \n Pack"; range.Style.WrapText = true; range.Style.Font.Bold = true; colum++;
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Pack/ \n Ctn"; range.Style.WrapText = true; range.Style.Font.Bold = true; colum++;
            }
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Q'ty/ \n Ctn"; range.Style.WrapText = true; range.Style.Font.Bold = true; colum++;
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = " Total \n Ctn"; range.Style.WrapText = true; range.Style.Font.Bold = true; colum++;
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Total \n Piece"; range.Style.WrapText = true; range.Style.Font.Bold = true; colum++;
            if (ShowKgThung)
            {
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "NW \n (kgs)"; range.Style.WrapText = true; range.Style.Font.Bold = true; colum++;
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "GW \n (kgs)"; range.Style.WrapText = true; range.Style.Font.Bold = true; colum++;
            }

            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Total NW \n (kgs)"; range.Style.WrapText = true; range.Style.Font.Bold = true; worksheet.Column(colum).Width = 10; colum++;
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Total GW \n (kgs)"; range.Style.WrapText = true; range.Style.Font.Bold = true; worksheet.Column(colum).Width = 10; colum++;
            var MaDV = dtPKLXuatHang.Rows[0]["MaDV"].ToString() == "1" ? "cm" : "inch";
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = $"Measurement \n {MaDV}"; range.Style.WrapText = true; worksheet.Column(colum).Width = 15; range.Style.Font.Bold = true; colum++;
            if (ShowBarCode)
            {
                range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "Barcode \n"; range.Style.WrapText = true; worksheet.Column(colum).Width = 15; range.Style.Font.Bold = true; colum++;
            }
            range = worksheet.Cells[column1, colum, column1 + colAdd, colum]; range.Merge = true; range.Value = "CBM \n"; range.Style.WrapText = true; worksheet.Column(colum).Width = 15; range.Style.Font.Bold = true; colum++;

            if (!flagCodeSize) worksheet.Row(column1).Height = 30;
            int row = column1 + 1 + colAdd;
            int index = 0;
            int rowMerge = column1 + 1 + colAdd;
            int indexMerge = 0;
            bool checkCountMerge = false;
            bool isCheck = true;
            List<ListMergePO> listPo = new List<ListMergePO>();
            foreach (DataRow rows in dtPKLXuatHang.Rows)
            {
                colum = 1;
                worksheet.Cells[row, colum].Value = Convert.ToInt32(rows["TuThung"]); colum++;
                worksheet.Cells[row, colum].Value = Convert.ToInt32(rows["DenThung"]); colum++;
                if (flagMaHang)
                {
                    string tempMH = "";
                    tempMH = rows["TenHang"].ToString();
                    worksheet.Cells[row, colum].Value = tempMH; worksheet.Column(colum).AutoFit(); colum++;
                }
                if (KieuLapPCB == "4")
                {
                    worksheet.Cells[row, colum].Value = rows["Dot"].ToString(); colum++;
                }
                worksheet.Cells[row, colum].Value = rows["PO"].ToString(); worksheet.Column(colum).AutoFit(); colum++;
                if (KieuLapPCB == "3")
                {
                    worksheet.Cells[row, colum].Value = rows["Store"].ToString(); colum++;
                }
                if (!hiddenDVSX)
                {
                    worksheet.Cells[row, colum].Value = rows["TenDVSX"].ToString(); colum++;
                }



                if (flagArt)
                {
                    worksheet.Cells[row, colum].Value = rows["ArtSize"].ToString();
                    colum++;
                }
                worksheet.Cells[row, colum].Value = rows["DauSize"].ToString(); colum++;
                worksheet.Cells[row, colum].Value = rows["TenMau"].ToString(); worksheet.Column(colum).AutoFit(); colum++;
                int starColumnSize = 0;
                starColumnSize = colum;
                foreach (var colums in columnNamesWithSize)
                {
                    if (rows[colums.ToString()].ToString() == "")
                    {
                        colum++;
                        continue;
                    }
                    int cellValue = Convert.ToInt32(rows[colums.ToString()]);
                    range = worksheet.Cells[row, colum]; range.Value = cellValue == 0 ? (object)" " : cellValue;
                    range.Style.Font.Bold = false;
                    colum++;
                }
                double khoiLuong = Convert.ToDouble(rows["KhoiLuong"]);
                double roundedKhoiLuong = Math.Round(khoiLuong, 2);
                double trongLuong = Convert.ToDouble(rows["TrongLuong"]);
                //int SLThung = Convert.ToInt32(rows["SLThung"]);
                double TrongLuongT = Convert.ToDouble(rows["TrongLuongT"]);
                double KhoiLuongT = Convert.ToDouble(rows["KhoiLuongT"]);
                if (totalPCB_Pack != 0)
                {
                    range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["PCB_Pack"]); range.Style.Font.Bold = false;/*worksheet.Column(colum).AutoFit();*/ colum++;
                    range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["Pack_Ctn"]); range.Style.Font.Bold = false;/*worksheet.Column(colum).AutoFit();*/ colum++;
                }

                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["SoLuong"]); /*worksheet.Column(colum).AutoFit();*/ colum++;
                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["SLThung"]); /*worksheet.Column(colum).AutoFit();*/ colum++;
                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["TotalPiece"]); /*worksheet.Column(colum).AutoFit();*/ colum++;
                if (ShowKgThung)
                {
                    range = worksheet.Cells[row, colum]; range.Value = trongLuong == 0 ? (object)" " : Math.Round(KhoiLuongT, 2); colum++;
                    range = worksheet.Cells[row, colum]; range.Value = khoiLuong == 0 ? (object)" " : Math.Round(TrongLuongT, 2); colum++;
                }
                range = worksheet.Cells[row, colum]; range.Value = khoiLuong == 0 ? (object)" " : roundedKhoiLuong;
                /*worksheet.Column(colum).AutoFit();*/
                colum++;
                range = worksheet.Cells[row, colum]; range.Value = trongLuong == 0 ? (object)" " : Math.Round(trongLuong, 2);
                /*worksheet.Column(colum).AutoFit();*/
                colum++;
                range = worksheet.Cells[row, colum]; range.Value = rows["KyHieu"].ToString(); /*worksheet.Column(colum).AutoFit();*/ colum++;
                double CBMT = (rows["CBM"] == null || rows["CBM"].ToString() == "") ? 0 : Convert.ToDouble(rows["CBM"]);
                double roundedCBMT = Math.Round(CBMT, 4);
                if (ShowBarCode)
                {
                    range = worksheet.Cells[row, colum]; range.Value = rows["Barcode"].ToString(); /*worksheet.Column(colum).AutoFit();*/ colum++;
                }
                range = worksheet.Cells[row, colum]; range.Value = roundedCBMT.ToString(); /*worksheet.Column(colum).AutoFit();*/ colum++;
                if (!isDongThung && checkKLSize)
                {
                    string currentDauSize = rows["DauSizeID"].ToString();
                    string previousDauSize = (index + 1 < dtPKLXuatHang.Rows.Count) ? dtPKLXuatHang.Rows[index + 1]["DauSizeID"].ToString() : "123091831";
                    columnSize = 8;
                    if (currentDauSize != previousDauSize)
                    {
                        if (previousDauSize == "123091831")
                            continue;
                        isCheck = false;
                        row++;
                        foreach (var colums in columnNamesWithSize)
                        {
                            var splitColum = colums.Split('@');

                            var drTempcodeSize = dtKhoiLuongSize.AsEnumerable().Where(x => x["SizeID"].ToString() == splitColum[1].ToString() && x["DauSizeID"].ToString() == previousDauSize.ToString()).FirstOrDefault();
                            string codeSize = "";
                            if (drTempcodeSize != null)
                                codeSize = drTempcodeSize["SL_KhoiLuong"].ToString();
                            worksheet.Cells[row, columnSize].Value = codeSize;
                            worksheet.Cells[row, columnSize].Style.Font.Color.SetColor(Color.FromArgb(27, 109, 201));

                            columnSize++;
                        }
                    }
                }

                int currentSttThung = Convert.ToInt32(rows["SttThung"]);
                int previousSttThung = (index + 1 < dtPKLXuatHang.Rows.Count) ? Convert.ToInt32(dtPKLXuatHang.Rows[index + 1]["SttThung"]) : 1012836403;
                int tuthung = Convert.ToInt32(rows["TuThung"]);
                int denthung = Convert.ToInt32(rows["DenThung"]);


                string current_maPKL = rows["MaPKLDisplay"].ToString();
                string po = rows["PO"].ToString();
                string color = rows["ColorID"].ToString();
                string dausize = rows["DauSizeID"].ToString();
                string pre_maPKL = (index + 1 < dtPKLXuatHang.Rows.Count) ? dtPKLXuatHang.Rows[index + 1]["MaPKLDisplay"].ToString() : "";
                listPo.Add(new ListMergePO()
                {
                    PO = po,
                    Color = color,
                    DauSize = dausize
                });

                List<string> listitem = new List<string> { "SoLuong", "SLThung", "TotalPiece", "CBM" };

                Dictionary<int, double> keyValue = new Dictionary<int, double>
                {

                    {5, Convert.ToInt32(rows[listitem[0]])},
                    {4, Convert.ToInt32(rows[listitem[1]])},
                    {3, Convert.ToInt32(rows[listitem[2]])},
                    {2, roundedKhoiLuong},
                    {1, Math.Round(trongLuong, 2)}
                };

                if (currentSttThung == previousSttThung && current_maPKL == pre_maPKL)
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
                    for (int i = 0; i < 6; i++)
                    {
                        double value = keyValue.ContainsKey(i) ? keyValue[i] : 0;
                        MergeCellsByRow(worksheet, colum - i - 2, rowMerge, rowMerge + indexMerge, "center", value);
                    }
                    MergeCellsByRow(worksheet, colum - 1, rowMerge, rowMerge + indexMerge, "center", roundedCBMT);
                    MergeCellsByRow(worksheet, 1, rowMerge, rowMerge + indexMerge, "center", tuthung);
                    MergeCellsByRow(worksheet, 2, rowMerge, rowMerge + indexMerge, "center", denthung);
                    int rowPo = 0;
                    //if (listPo.Count > 1)
                    //{
                    //    bool checkColor = listPo.All(x => x.Color == listPo.First().Color);
                    //    string PO = string.Join(", ", listPo.AsEnumerable().Select(r => r.PO.ToString()).Distinct());
                    //    bool checkDauSize = listPo.All(x => x.DauSize == listPo.First().DauSize);
                    //    if (!checkDauSize || !checkColor)
                    //    {
                    //        for (int i = 0; i <= indexMerge; i++)
                    //        {
                    //            worksheet.Cells[rowMerge + i, 4].Value = PO;  // Adjust the operation as needed
                    //        }
                    //        MergeCellsByRowString(worksheet, 4, rowMerge, rowMerge + indexMerge, "center");
                    //    }
                    //    else
                    //    {
                    //        for (int i = 0; i < columnNamesWithSize.Count; i++)
                    //        {
                    //            int sumValueNew = 0;
                    //            for (int j = 0; j <= indexMerge; j++)
                    //            {
                    //                int valueSize = worksheet.Cells[rowMerge + j, starColumnSize + i].Value.ToString() == " " ? 0 : Convert.ToInt32(worksheet.Cells[rowMerge + j, starColumnSize + i].Value);
                    //                sumValueNew += valueSize;
                    //                worksheet.Cells[rowMerge + j, 4].Value = PO;
                    //            }
                    //            for (int j = 0; j <= indexMerge; j++)
                    //            {
                    //                worksheet.Cells[rowMerge + j, starColumnSize + i].Value = sumValueNew == 0 ? (object)" " : sumValueNew;
                    //            }
                    //        }
                    //        for (int i = 0; i < listPo.Count - 1; i++)
                    //        {
                    //            worksheet.DeleteRow(row);
                    //            row--;
                    //        }

                    //        rowPo++;
                    //    }
                    //}
                    //listPo = new List<ListMergePO>();
                    rowMerge = row + rowPo;
                    indexMerge = 0;
                    rowPo = 0;
                }
                index++;
                row++;
            }
            if (checkKLSize)
                row++;

            int indexsumAll = 7;
            if (ShowBarCode && ShowKgThung) indexsumAll = 10;
            else if (ShowBarCode) indexsumAll = 8;
            else if (ShowKgThung) indexsumAll = 9;
            int indexSum = 0;
            foreach (string sumAll in sumToTalAll)
            {
                if (sumToTalAll.Count == indexSum + 1)
                {
                    worksheet.Cells[row, colum - 1].Value = sumAll;
                    worksheet.Cells[row, colum - 1].Style.Font.Bold = true;
                }
                else
                {
                    worksheet.Cells[row, colum - indexsumAll].Value = sumAll;
                    worksheet.Cells[row, colum - indexsumAll].Style.Font.Bold = true;
                }
                indexsumAll--;
                indexSum++;
            }
            var borderData = worksheet.Cells[staticCol, 1, row - 1, colum - 1].Style.Border;
            borderData.Bottom.Style =
                borderData.Top.Style =
                borderData.Left.Style =
                borderData.Right.Style = ExcelBorderStyle.Thin;
            row++;

            colum = 4;
            if (flagArt) colum++;
            if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
            if (flagMaHang) colum++;
            if (hiddenDVSX) colum--;
            range = worksheet.Cells[row, colum, row + colAdd, colum]; range.Merge = true; range.Value = "Style"; range.Style.Font.Bold = true; colum++;
            range = worksheet.Cells[row, colum, row + colAdd, colum]; range.Merge = true; range.Value = checkpo == true ? "PO" : "Size/ Inseam"; worksheet.Column(colum).Width = 12; range.Style.Font.Bold = true; colum++;
            range = worksheet.Cells[row, colum, row + colAdd, colum]; range.Merge = true; range.Value = "Color"; range.Style.Font.Bold = true; colum++;


            foreach (var colums in columnNamesWithSize)
            {
                var splitColum = colums.Split('@');
                worksheet.Cells[row, colum].Value = splitColum[0].ToString();
                worksheet.Cells[row, colum].Style.Font.Bold = true;
                if (colAdd == 1)
                {
                    var dtTempcodeSize = dtCodeSize.AsEnumerable().Where(x => x["SizeID"].ToString() == splitColum[1].ToString()
                                            && x["MaMau"].ToString() == dtPKLXuatHang.Rows[0]["ColorID"].ToString()).FirstOrDefault();
                    string codeSize = "";
                    if (dtTempcodeSize != null)
                    {
                        codeSize = dtTempcodeSize["CodeSize"].ToString();
                    }
                    worksheet.Cells[row + colAdd, colum].Value = codeSize;
                    worksheet.Cells[row + colAdd, colum].Style.Font.Color.SetColor(Color.FromArgb(27, 109, 201));
                }
                colum++;
            }
            range = worksheet.Cells[row, colum, row + colAdd, colum]; range.Merge = true; range.Value = "Total PCS"; range.Style.Font.Bold = true; worksheet.Column(colum).Width = 10;
            range = worksheet.Cells[row, colum + 1, row + colAdd, colum + 1]; range.Merge = true; range.Value = "Carton"; range.Style.Font.Bold = true;
            row = row + colAdd;
            row++;
            colum = 4;
            if (flagArt) colum++;
            if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
            if (flagMaHang) colum++;
            if (hiddenDVSX) colum--;
            rowMerge = row;
            var dauSize = dtPKLXuatHang.AsEnumerable().Select(x => new
            {
                MaPKL = x["MaPKLDisplay"],
                TenHang = x["TenHang"],
                MaHang = x["MaHang"],
                NameDauSize = checkpo == true ? x["PO"] : x["DauSize"],
                DauSize = checkpo == true ? x["POID"] : x["DauSizeID"],
                TenMau = x["TenMau"],
                ColorID = x["ColorID"]
            }).Distinct().ToList();
            var distinctColors = dtPKLXuatHang.AsEnumerable()
                .Select(x => x["TenMau"].ToString())
                .Distinct()
                .ToList();
            var tblSttTrung = dtPKLXuatHang.AsEnumerable()
                  .GroupBy(row1 => new
                  {
                      SttThung = row1["SttThung"].ToString(),
                      MaPKLDisplay = row1["MaPKLDisplay"].ToString(),
                  })
                  .Where(ageGroup => ageGroup.Count() > 1)
                  .Select(group => new
                  {
                      SttThung = group.Key.SttThung,
                      MaPKLDisplay = group.Key.MaPKLDisplay,
                      DauSize = String.Join(",", group.Select(x => checkpo == true ? x["POID"].ToString() : x["DauSizeID"].ToString()).Distinct()),
                      NameDauSize = String.Join(",", group.Select(x => checkpo == true ? x["PO"].ToString() : x["DauSize"].ToString()).Distinct()),
                      TenMau = String.Join(",", group.Select(x => x["TenMau"].ToString()).Distinct()),
                      ColorID = String.Join(",", group.Select(x => x["ColorID"].ToString()).Distinct()),
                      MaHang = String.Join(",", group.Select(x => x["MaHang"].ToString()).Distinct())
                  })
                  .ToList();



            int indexColor = 0;
            int columnColor = 0;
            int rowcolor = row;
            int rowBorder = row;
            List<int> lstRow = new List<int>();
            List<int> lstRowSum = new List<int>();
            foreach (var dausize in dauSize)
            {
                colum = 4;
                lstRowSum.Add(row);
                if (flagArt) colum++;
                if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
                if (flagMaHang) colum++;
                if (hiddenDVSX) colum--;
                worksheet.Cells[row, colum].Value = dausize.TenHang; worksheet.Column(colum).AutoFit(); colum++;
                worksheet.Cells[row, colum].Value = dausize.NameDauSize; colum++;
                worksheet.Cells[row, colum].Value = dausize.TenMau; colum++;
                colum = 7;
                string CheckColorDup = dauSize[indexColor].ColorID.ToString();
                if (flagArt) colum++;
                if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
                if (flagMaHang) colum++;
                if (hiddenDVSX) colum--;
                columnColor = colum;
                int sumPCS = 0;
                int sumcarton = 0;
                foreach (var colums in columnNamesWithSize)
                {
                    int sumTotalSize = dtPKLXuatHang.AsEnumerable()
                        .Where(x => (dausize.MaPKL.ToString() == x["MaPKLDisplay"].ToString() && dausize.MaHang.ToString() == x["MaHang"].ToString() && (checkpo == true ? x["POID"].ToString() : x["DauSizeID"].ToString()) == dausize.DauSize.ToString())
                        && x["ColorID"].ToString() == dausize.ColorID.ToString()
                        && !tblSttTrung.Any(sttTrung =>
                            sttTrung.SttThung == x["SttThung"].ToString() &&
                            sttTrung.MaPKLDisplay == x["MaPKLDisplay"].ToString())
                        )
                        .Sum(y =>
                        {
                            int sizeValue = Convert.ToInt32(y[colums].ToString() == "" ? 0 : y[colums]);
                            int quantity = Convert.ToInt32(y["SLThung"]);
                            int PackCtn = Convert.ToInt32(y["Pack_Ctn"]);
                            return y["PCB_Pack"].ToString() != "0" ? sizeValue * quantity * PackCtn : sizeValue * quantity;
                        });
                    worksheet.Cells[row, colum].Style.Font.Bold = false;
                    worksheet.Cells[row, colum].Value = sumTotalSize == 0 ? (object)" " : sumTotalSize;
                    colum++;
                    sumPCS += sumTotalSize;

                    worksheet.Cells[row, colum].Value = sumPCS;
                    worksheet.Cells[row, colum + 1].Style.Font.Bold = false;
                    worksheet.Cells[row, colum + 1].Value = sumcarton;
                }
                int cartonSum = dtPKLXuatHang.AsEnumerable()
                      .Where(x => (dausize.MaPKL.ToString() == x["MaPKLDisplay"].ToString() && dausize.MaHang.ToString() == x["MaHang"].ToString() && (checkpo == true ? x["POID"].ToString() : x["DauSizeID"].ToString()) == dausize.DauSize.ToString())
                      && x["ColorID"].ToString() == dausize.ColorID.ToString()
                                              && !tblSttTrung.Any(sttTrung =>
                            sttTrung.SttThung == x["SttThung"].ToString() &&
                            sttTrung.MaPKLDisplay == x["MaPKLDisplay"].ToString())
                      )
                        .Sum(y => Convert.ToInt32(y["SLThung"]));
                worksheet.Cells[row, colum + 1].Style.Font.Bold = false;
                worksheet.Cells[row, colum + 1].Value = cartonSum == 0 ? (object)" " : cartonSum;

                sumcarton += cartonSum;
                worksheet.Cells[row, colum + 1].Value = sumcarton;
                string colorNext = dauSize.Count > indexColor + 1 ? dauSize[indexColor + 1].ColorID.ToString() : "avbbcsd";
                if (distinctColors.Count > 1)
                {
                    if (CheckColorDup != colorNext)
                    {
                        row++;
                        range = worksheet.Cells[row, 5, row, columnColor - 1]; range.Value = "Tổng"; range.Style.Font.Bold = true; range.Merge = true;
                        colum = columnColor;
                        for (int i = 0; i < columnNamesWithSize.Count + 2; i++)
                        {
                            worksheet.Cells[row, colum].Formula = "=SUM(" + worksheet.Cells[rowcolor, colum].Address + ":" + worksheet.Cells[row - 1, colum].Address + ")";
                            worksheet.Cells[row, colum].Style.Font.Bold = true;
                            colum++;

                        }
                        lstRow.Add(row);
                        rowcolor = row + 1;
                    }
                }
                if (sumcarton == 0)
                {
                    worksheet.DeleteRow(row);
                    //row--;
                }
                else
                {
                    indexMerge++;
                    row++;
                    indexColor++;
                }
            }
            //SumTongA(worksheet, row, colum, colum + columnNamesWithSize.Count + 1, lstRow, rowBorder);
            foreach (var dausize1 in tblSttTrung)
            {
                colum = 4;
                lstRowSum.Add(row);
                lstRow.Add(row);
                if (flagArt) colum++;
                if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
                if (flagMaHang) colum++;
                if (hiddenDVSX) colum--;
                worksheet.Cells[row, colum].Value = dausize1.MaHang; worksheet.Column(colum).AutoFit(); colum++;
                worksheet.Cells[row, colum].Value = dausize1.NameDauSize; colum++;
                worksheet.Cells[row, colum].Value = dausize1.TenMau; colum++;
                colum = 7;
                if (flagArt) colum++;
                if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
                if (flagMaHang) colum++;
                if (hiddenDVSX) colum--;
                int sumPCS = 0;
                int sumcarton = 0;
                foreach (var colums in columnNamesWithSize)
                {

                    int sumTotalSize = dtPKLXuatHang.AsEnumerable()
                        .Where(x => (dausize1.MaPKLDisplay.ToString() == x["MaPKLDisplay"].ToString()
                        && dausize1.SttThung.ToString() == x["SttThung"].ToString())
                        )
                        .Sum(y =>
                        {
                            int sizeValue = Convert.ToInt32(y[colums].ToString() == "" ? 0 : y[colums]);
                            int quantity = Convert.ToInt32(y["SLThung"]);
                            int PackCtn = Convert.ToInt32(y["Pack_Ctn"]);
                            return y["PCB_Pack"].ToString() != "0" ? sizeValue * quantity * PackCtn : sizeValue * quantity;
                        });
                    worksheet.Cells[row, colum].Style.Font.Bold = false;
                    worksheet.Cells[row, colum].Value = sumTotalSize == 0 ? (object)" " : sumTotalSize;
                    colum++;
                    sumPCS += sumTotalSize;

                    worksheet.Cells[row, colum].Value = sumPCS;
                    worksheet.Cells[row, colum + 1].Style.Font.Bold = false;
                    worksheet.Cells[row, colum + 1].Value = sumcarton;
                }

                worksheet.Cells[row, colum + 1].Value = 1;

                indexMerge++;
                row++;
            }


            colum = 4;
            if (flagArt) colum++;
            if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
            if (flagMaHang) colum++;
            if (hiddenDVSX) colum--;
            //range = worksheet.Cells[row, colum, row, colum + 2]; range.Merge = true; range.Value = "Tổng";
            colum = 7;
            if (flagArt) colum++;
            if (KieuLapPCB == "3" || KieuLapPCB == "4") colum++;
            if (flagMaHang) colum++;
            if (hiddenDVSX) colum--;
            int sumTotal = 0;
            if (distinctColors.Count > 1)
            {
                SumTongASum(worksheet, row, colum, colum + columnNamesWithSize.Count + 1, lstRow, rowBorder);
            }
            else SumTongASum(worksheet, row, colum, colum + columnNamesWithSize.Count + 1, lstRowSum, rowBorder);

            //


            //for (int i = 0; i < columnNamesWithSize.Count + 1; i++)
            //{
            //    worksheet.Cells[row, colum].Formula = "=SUM(" + worksheet.Cells[rowMerge, colum].Address + ":" + worksheet.Cells[rowMerge + indexMerge - 1, colum].Address + ")";
            //    worksheet.Cells[row, colum].Style.Font.Bold = true;
            //    colum++;
            //}
            //worksheet.Cells[row, colum].Value = sumToTalAll[1];
            worksheet.Cells[row, colum].Style.Font.Bold = true;
            var dtTotal = dtPKLXuatHang.AsEnumerable().GroupBy(gr => new
            {
                SttThung = gr["SttThung"].ToString(),
                SLThung = gr["SLThung"].ToString(),
            }).Select(y => new
            {
                SLKH = y.Sum(rowA => Convert.ToInt32(rowA["SLThung"])),
            }).Distinct().ToList();
            colum++;
            row += 2;
            var carton = dtPKLXuatHang.AsEnumerable()
            .Select(x => new { KyHieu = x["KyHieu"].ToString() })
            .Distinct()
            .ToList();
            double slThung = 0;
            if (carton.Count > 1)
            {
                int rowCarton = 0;
                foreach (var item in carton)
                {
                    if (item.KyHieu == "") continue;
                    int sumCarton = uniqueValues.Where(x => x.KyHieu.ToString() == item.KyHieu.ToString()).Sum(x => Convert.ToInt32(x.SLThung));
                    range = worksheet.Cells[row + 3 + rowCarton, 9];
                    range.Value = item.KyHieu;

                    string[] totalSLThung1 = item.KyHieu.ToString().Split('x');
                    if (dtPKLXuatHang.Rows[0]["MaDV"].ToString() == "1")
                    {
                        slThung += (Convert.ToDouble(totalSLThung1[0]) * 0.01) * (Convert.ToDouble(totalSLThung1[1]) * 0.01) * (Convert.ToDouble(totalSLThung1[2]) * 0.01) * sumCarton;

                    }

                    else slThung += (Convert.ToDouble(totalSLThung1[0]) * 0.0254) * (Convert.ToDouble(totalSLThung1[1]) * 0.0254) * (Convert.ToDouble(totalSLThung1[2]) * 0.0254) * sumCarton;
                    range = worksheet.Cells[row + 3 + rowCarton, 10];
                    range.Value = sumCarton;

                    rowCarton++;
                }
            }
            else
            {
                if (dtPKLXuatHang.Rows[0]["KyHieu"].ToString() != "")
                {
                    string[] totalSLThung1 = dtPKLXuatHang.Rows[0]["KyHieu"].ToString().Split('x');
                    if (dtPKLXuatHang.Rows[0]["MaDV"].ToString() == "1")
                        slThung = (Convert.ToDouble(totalSLThung1[0]) * 0.01) * (Convert.ToDouble(totalSLThung1[1]) * 0.01) * (Convert.ToDouble(totalSLThung1[2]) * 0.01) * totalSLThung;
                    else slThung = (Convert.ToDouble(totalSLThung1[0]) * 0.0254) * (Convert.ToDouble(totalSLThung1[1]) * 0.0254) * (Convert.ToDouble(totalSLThung1[2]) * 0.0254) * totalSLThung;
                }

            }
            range = worksheet.Cells[row, 7]; range.Merge = true; range.Value = "Ctns";
            range = worksheet.Cells[row, 6]; range.Merge = true; range.Value = totalSLThung;
            range = worksheet.Cells[row + 1, 7]; range.Merge = true; range.Value = "Pcs";
            range = worksheet.Cells[row + 1, 6]; range.Merge = true; range.Value = totalTotalPiece;
            // range = worksheet.Cells[row + 1, 9]; range.Merge = true; range.Value = "Pcs";
            //range = worksheet.Cells[row + 1, 8]; range.Merge = true; range.Value = totalTotalPiece * 2;
            range = worksheet.Cells[row + 2, 7]; range.Merge = true; range.Value = "Kgs";
            range = worksheet.Cells[row + 2, 6]; range.Merge = true; range.Value = roundedTotalKhoiLuong.ToString();
            range = worksheet.Cells[row + 3, 7]; range.Merge = true; range.Value = "Kgs";
            range = worksheet.Cells[row + 3, 6]; range.Merge = true; range.Value = roundedTotalTrongLuong.ToString();
            range = worksheet.Cells[row + 4, 6]; range.Merge = true; range.Value = carton.Count > 1 ? "" : dtPKLXuatHang.Rows[0]["KyHieu"].ToString();
            //slThung += slThung;// * 0.03;
            range = worksheet.Cells[row + 5, 6]; range.Merge = true; range.Value = Math.Round(slThung, 2);
            range = worksheet.Cells[row + 5, 7]; range.Merge = true; range.Value = "CBM";

            range = worksheet.Cells[row, 4, row, 5]; range.Merge = true; range.Value = "Total boxes "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 1, 4, row + 1, 5]; range.Merge = true; range.Value = "Total Quantity  "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 2, 4, row + 2, 5]; range.Merge = true; range.Value = "Total Net Weight"; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 3, 4, row + 3, 5]; range.Merge = true; range.Value = "Total Gross Weight"; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 4, 4, row + 4, 5]; range.Merge = true; range.Value = "Cnt Measurements "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 5, 4, row + 5, 5]; range.Merge = true; range.Value = "Total CBM "; range.Style.Font.Bold = true;

            if (flagArt)
            {
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
            }
            else
            {
                // worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
            }

            int col = 6;
            if (flagArt) col++;
            if (KieuLapPCB == "3" || KieuLapPCB == "4") col++;
            if (flagMaHang) colum++;
            if (hiddenDVSX) colum--;
            for (int i = 1; i < col + 1; i++)
            {
                worksheet.Cells[row + i - 1, 4, row + i - 1, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet.Cells[row + i - 1, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                worksheet.Cells[row + i - 1, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }
            int columA = 4;
            if (flagArt) columA++;
            if (KieuLapPCB == "3" || KieuLapPCB == "4") columA++;
            if (flagMaHang) columA++;
            if (hiddenDVSX) colum++;
            if (hiddenDVSX) columA--;
            var borderData1 = worksheet.Cells[rowMerge - 1 - colAdd, columA, rowMerge + indexMerge, colum - 2].Style.Border;
            borderData1.Bottom.Style =
                borderData1.Top.Style =
                borderData1.Left.Style =
                borderData1.Right.Style = ExcelBorderStyle.Thin;
            if (worksheet.Column(flagArt ? 7 : 6).Width > 11)
                worksheet.Column(flagArt ? 7 : 6).AutoFit();
            else
                worksheet.Column(flagArt ? 7 : 6).Width = 12;

            worksheet.Column(3).Width = 25;
            worksheet.Column(3).Style.WrapText = true;


        }
        public static void SumTongASum(ExcelWorksheet worksheet, int row, int colBD, int colKT, List<int> lstRow, int rowBorder)
        {
            worksheet.Cells[row, 5, row, colBD - 1].Value = "Tổng"; worksheet.Cells[row, 5, row, colBD - 1].Style.Font.Bold = true; worksheet.Cells[row, 5, row, colBD - 1].Merge = true;

            for (int col = colBD; col <= colKT; col++)
            {
                string colLetter = ExcelCellAddress.GetColumnLetter(col);
                string cellRefs = string.Join(",", lstRow.Select(r => $"{colLetter}{r}").Distinct());

                // Gán công thức SUM
                var cell = worksheet.Cells[row, col];
                cell.Formula = $"=SUM({cellRefs})";
                cell.Style.Font.Bold = true;
            }
            var borderData1 = worksheet.Cells[rowBorder - 1, 5, row, colKT].Style.Border;
            borderData1.Bottom.Style =
                borderData1.Top.Style =
                borderData1.Left.Style =
                borderData1.Right.Style = ExcelBorderStyle.Thin;
        }

        private static void MergeCellsByRowString(ExcelWorksheet worksheet, int colum, int startrow, int endrow, string align)
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
        private static void MergeCellsByRow(ExcelWorksheet worksheet, int colum, int startrow, int endrow, string align, double tuthung)
        {
            worksheet.Cells[startrow, colum, endrow, colum].Merge = true;

            if (tuthung != 0)
            {
                worksheet.Cells[startrow, colum].Value = tuthung;

                for (int row = startrow + 1; row <= endrow; row++)
                {
                    worksheet.Cells[row, colum].Value = null;
                }
            }

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
                    try
                    {
                        int sumTotalSize = tbl.AsEnumerable()
                        .Where(x => x["DauSizeID"].ToString() == dausize.DauSize.ToString() && x["ColorID"].ToString() == dausize.Color.ToString())
                        .Sum(y =>
                        {
                            int sizeValue = Convert.ToInt32(y[sizeColumn].ToString() == "" ? 0 : y[sizeColumn]);
                            int quantity = Convert.ToInt32(y["SLThung"]);
                            int PackCtn = Convert.ToInt32(y["Pack_Ctn"]);
                            return y["PCB_Pack"].ToString() != "0" ? sizeValue * quantity * PackCtn : sizeValue * quantity;
                        });

                        newRow[sizeColumn] = sumTotalSize;
                        tongSize += sumTotalSize;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                newRow["TongSize"] = tongSize;
                summaryDataTable.Rows.Add(newRow);
            }
            return summaryDataTable;
        }
        public static DataTable sumToTalPCS_Scan(DataTable tbl)
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
            summaryDataTable.Columns.Add("SLThung", typeof(int));
            summaryDataTable.Columns.Add("STDaQuet", typeof(int));
            summaryDataTable.Columns.Add("TenMau", typeof(string));

            var tblSttTrung = tbl.AsEnumerable()
                                   .GroupBy(row => new
                                   {
                                       SttThung = row["SttThung"].ToString(),
                                       MaPKLDisplay = row["MaPKLDisplay"].ToString(),
                                   })
                                   .Where(ageGroup => ageGroup.Count() > 1).ToList();
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
                var tblCheck = tbl.AsEnumerable()
                       .Where(x => x["DauSizeID"].ToString() == dausize.DauSize.ToString() && x["ColorID"].ToString() == dausize.Color.ToString()
                                  && !tblSttTrung.Any(oj => oj.Key.SttThung.ToString() == x["SttThung"].ToString()));
                foreach (string sizeColumn in columnNamesWithSize)
                {
                    try
                    {
                        int sumTotalSize = tblCheck.Sum(y =>
                        {
                            int sizeValue = Convert.ToInt32(y[sizeColumn].ToString() == "" ? 0 : y[sizeColumn]);
                            int quantity = Convert.ToInt32(y["SLThung"]);
                            int PackCtn = Convert.ToInt32(y["Pack_Ctn"]);
                            return y["PCB_Pack"].ToString() != "0" ? sizeValue * quantity * PackCtn : sizeValue * quantity;
                        });
                        newRow[sizeColumn] = sumTotalSize;
                        tongSize += sumTotalSize;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                int sumSLThung = tblCheck.Sum(y => Convert.ToInt32(y["SLThung"]));
                int sumSTDaQuet = tblCheck.Sum(y => Convert.ToInt32(y["STDaQuet"]));
                newRow["SLThung"] = sumSLThung;
                newRow["STDaQuet"] = sumSTDaQuet;

                summaryDataTable.Rows.Add(newRow);
            }
            foreach (var item in tblSttTrung)
            {
                var dtTrung = tbl.AsEnumerable().Where(x => x["MaPKLDisplay"].ToString() == item.Key.MaPKLDisplay && x["SttThung"].ToString() == item.Key.SttThung.ToString());


            }

            return summaryDataTable;
        }
        public static void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA, DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repotxtN0, bool IsCheckSumSize = true)
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
                col.FieldName = colName;
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
                    if (bandedGridViewKHDT.GetFocusedRowCellValue("IsDongThung1").ToString() == "")
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
                    else if (bandedGridViewKHDT.GetFocusedRowCellValue("IsXuatHang1").ToString() == "1")
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
        public static DataTable GetQuiCach(RepositoryItemSearchLookUpEdit repoQuiCach, string URL, string _maHang, HttpClientExtension _clientExtension, string _maNoiDen = "")
        {
            string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetQuiCachMH&Para1={_maHang}&Para2={_maNoiDen}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable dtQuiCach = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtQuiCach is null) return new DataTable();
            DataTable tblQuiCach = dtQuiCach.Copy();
            if (tblQuiCach.Columns.Contains("SizeID"))
            {
                tblQuiCach.Columns.Remove("SizeID");
                tblQuiCach.Columns.Remove("DauSizeID");
                tblQuiCach = tblQuiCach.AsEnumerable().Distinct(DataRowComparer.Default).CopyToDataTable();
            }
            repoQuiCach.DataSource = tblQuiCach;
            return dtQuiCach;
        }
        public static DataTable GetAllQuiCach(RepositoryItemSearchLookUpEdit repoQuiCach, string URL, string _maHang, HttpClientExtension _clientExtension)
        {
            string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetAllQuiCach&Para1=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable dtQuiCach = JsonConvert.DeserializeObject<DataTable>(json);
            //if (dtQuiCach is null) return new DataTable();
            //DataTable tblQuiCach = dtQuiCach.Copy();
            //if (tblQuiCach.Columns.Contains("SizeID"))
            //{
            //    tblQuiCach.Columns.Remove("SizeID");
            //    tblQuiCach.Columns.Remove("DauSizeID");
            //    tblQuiCach = tblQuiCach.AsEnumerable().Distinct(DataRowComparer.Default).CopyToDataTable();
            //}
            repoQuiCach.DataSource = dtQuiCach;
            return dtQuiCach;
        }
        public static void InitQuiCach(RepositoryItemSearchLookUpEdit repoQuiCach)
        {
            repoQuiCach.ValueMember = "MaQuiCach";
            repoQuiCach.DisplayMember = "KyHieu";
            repoQuiCach.NullText = "[..Chọn QC..]";
            GridView dvView = repoQuiCach.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaQuiCach", Caption = "Mã QC", Name = "colMaQC", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "GhiChu", Caption = "Khách hàng", Name = "colGhiChu", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenQuiCach", Caption = "Tên QC", Name = "colTenQC", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "KyHieu", Caption = "Kích thước", Name = "colKH", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TLThung", Caption = "Cân nặng", Name = "colCanNag", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "SoLop", Caption = "Số lớp", Name = "colSoLop", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "LoaiThung", Caption = "Loại thùng", Name = "colLoaiThung", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "GhiChu2", Caption = "Ghi chú", Name = "colGhiChu2", Visible = true });
            }
        }
        public static Tuple<string, string> GetQuiCachSize(DataTable dt, string DauSize, string Size)
        {
            string MaQCChan = "", MaQCLe = "";
            DataTable dtQuiCach = new DataTable();
            var dtTemp = dt.AsEnumerable().Where(x => x["DauSizeID"].ToString() == DauSize && x["SizeID"].ToString() == Size);
            if (dtTemp.Count() == 0) return new Tuple<string, string>("", "");
            else if (dtTemp.Count() == 1)
            {
                dtQuiCach = dtTemp.CopyToDataTable();
                MaQCChan = MaQCLe = dtQuiCach.Rows[0]["MaQuiCach"].ToString();
            }
            else
            {
                dtQuiCach = dtTemp.CopyToDataTable();
                MaQCChan = dtQuiCach.Rows[0]["MaQuiCach"].ToString();
                MaQCLe = dtQuiCach.Rows[1]["MaQuiCach"].ToString();
            }
            return new Tuple<string, string>(MaQCChan, MaQCLe);
            //switch (type)
            //{
            //    case 0:
            //       return ;
            //    case 1:
            //        return dtQuiCach.Rows[0]["MaQuiCach"].ToString();
            //    case 2:
            //        return ;
            //    default:
            //        return "";
            //}


        }
        public static void AllowVieworNotPackV2(DataTable tbl, GridBand BandMaHang, GridBand BandDot)
        {
            if (tbl.Rows.Count > 0)
            {
                string KieuLapPCB = tbl.Rows[0]["KieuLapPCB"].ToString();
                if (KieuLapPCB == "4")
                {
                    BandMaHang.Visible = true;
                    BandDot.Visible = true;
                }
                else
                {
                    BandMaHang.Visible = false;
                    BandDot.Visible = false;
                }
            }
        }
        public static void AllowVieworNotPack(DataTable tbl, GridBand BandPCB, GridBand BandPack, GridBand BandStore = null)
        {
            if (tbl.Rows.Count > 0)
            {
                string KieuLapPCB = tbl.Rows[0]["KieuLapPCB"].ToString();
                if (KieuLapPCB == "3")
                {
                    BandStore.Visible = true;
                    BandPCB.Visible = false;
                    BandPack.Visible = false;
                }
                else if (KieuLapPCB == "0" || KieuLapPCB == "4")
                {
                    BandPCB.Visible = false;
                    BandPack.Visible = false;
                    BandStore.Visible = false;
                }
                else
                {
                    BandPCB.Visible = true;
                    BandPack.Visible = true;
                    BandStore.Visible = false;
                }
            }
        }
        public static string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }
        public static DataTable CreateTblCaiDatBarcode()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("BarCodeChan", typeof(string));
            dt.Columns.Add("SoLuongChan", typeof(int));
            dt.Columns.Add("BarCodeLe", typeof(string));
            dt.Columns.Add("SoLuongLe", typeof(int));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            return dt;
        }
        public static DataTable CreateTblPCB()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Chon", typeof(bool));
            dt.Columns.Add("ChonQC", typeof(bool));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("PCB_Pack", typeof(int));
            dt.Columns.Add("Pack_Ctn", typeof(int));
            dt.Columns.Add("PCB", typeof(int));
            dt.Columns.Add("SL_KhoiLuong", typeof(float));
            dt.Columns.Add("SL_TrongLuong", typeof(float));
            dt.Columns.Add("Version", typeof(string));
            dt.Columns.Add("MaNoiDen", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            return dt;
        }
        public static DataTable CreateTblQC()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaQuiCach", typeof(string));
            dt.Columns.Add("TenQuiCach", typeof(string));
            dt.Columns.Add("ChieuDai", typeof(float));
            dt.Columns.Add("ChieuRong", typeof(float));
            dt.Columns.Add("ChieuCao", typeof(float));
            dt.Columns.Add("CanNang", typeof(float));
            dt.Columns.Add("MaDV", typeof(string));
            dt.Columns.Add("SoLop", typeof(int));
            dt.Columns.Add("LoaiThung", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("GhiChu2", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }

        public static DataTable CreateTblGopPO()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("POID_G", typeof(string));
            dt.Columns.Add("PO_G", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("Module", typeof(string));
            return dt;
        }
        public static void HandleBandViewKey(KeyEventArgs e, BandedGridView bandView, string _fieldName)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int RowHandle = bandView.FocusedRowHandle;
                if (RowHandle == bandView.RowCount - 1) RowHandle = -1;
                bandView.FocusedRowHandle = RowHandle + 1; // Set focus to the row
                bandView.FocusedColumn = bandView.Columns[_fieldName]; // Set focus to the column
                bandView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Down)
            {
                bandView.FocusedColumn = bandView.Columns[_fieldName]; // Set focus to the column
                bandView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Up)
            {
                bandView.FocusedColumn = bandView.Columns[_fieldName]; // Set focus to the column
                bandView.ShowEditor();
            }
        }
        public static void HandleGridViewKey(KeyEventArgs e, GridView gridView, string _fieldName)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int RowHandle = gridView.FocusedRowHandle;
                if (RowHandle == gridView.RowCount - 1) RowHandle = -1;
                gridView.FocusedRowHandle = RowHandle + 1; // Set focus to the row
                gridView.FocusedColumn = gridView.Columns[_fieldName]; // Set focus to the column
                gridView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Down)
            {
                gridView.FocusedColumn = gridView.Columns[_fieldName]; // Set focus to the column
                gridView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Up)
            {
                gridView.FocusedColumn = gridView.Columns[_fieldName]; // Set focus to the column
                gridView.ShowEditor();
            }
        }
        public static bool CheckKieuLapKhongTheoThuTu(DataTable dtSource)
        {

            DataRow drOld = null;
            bool flagFirst = true;
            foreach (DataRow dr in dtSource.Rows)
            {
                if (flagFirst)
                {
                    if (dr["TuThung"].ToString() != "1")
                    {
                        return true;
                    }
                    flagFirst = false;
                }

                if (drOld == null)
                {
                    drOld = dr;
                    continue;
                }
                if (drOld["SttThung"].ToString() == dr["SttThung"].ToString()) continue;
                if ((Convert.ToInt16(drOld["DenThung"]) + 1) != Convert.ToInt16(dr["TuThung"]))
                {
                    return true;
                }
                drOld = dr;
            }
            return false;
        }
        public static DataTable LoadNoiDen(string URL, string maHang, HttpClientExtension clientExtension)
        {
            try
            {
                string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetNoiDen&Para1={maHang}");
                string json = Task.Run(async () => { return await clientExtension.GetAsnyc(url); }).Result;
                DataTable dtNoiDen = new DataTable();
                if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]" || json.Trim() == "null")
                {
                    dtNoiDen = new DataTable();
                    dtNoiDen.Columns.Add("MaNoiDen", typeof(string));
                    dtNoiDen.Columns.Add("NoiDen", typeof(string));
                }
                else
                {
                    dtNoiDen = JsonConvert.DeserializeObject<DataTable>(json) ?? new DataTable();
                    if (!dtNoiDen.Columns.Contains("MaNoiDen")) dtNoiDen.Columns.Add("MaNoiDen", typeof(string));
                    if (!dtNoiDen.Columns.Contains("NoiDen")) dtNoiDen.Columns.Add("NoiDen", typeof(string));
                }
                return dtNoiDen;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Lỗi LoadNoiDen KHDTLib: {ex.Message}");
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("MaNoiDen", typeof(string));
                dtEmpty.Columns.Add("NoiDen", typeof(string));
                return dtEmpty;
            }
        }
        public static DataTable CalculatorTB(DataTable tbl)
        {
            try
            {
                if (!tbl.Columns.Contains("CBM"))
                {
                    tbl.Columns.Add("CBM", typeof(string));
                }
                foreach (DataRow row in tbl.Rows)
                {
                    if (row["ChieuRong"] == null || row["ChieuRong"].ToString() == "") continue;
                    double rong = Convert.ToDouble(row["ChieuRong"]);
                    double dai = Convert.ToDouble(row["ChieuDai"]);
                    double cao = Convert.ToDouble(row["ChieuCao"]);

                    double cbm = ((rong * dai * cao) / 1_000_000); // từ cm³ sang m³
                                                                   //cbm += cbm * 0.03;
                    row["CBM"] = cbm;
                }
                //tbl.AsEnumerable().ToList().ForEach(row =>
                //{
                //    if (row["ChieuRong"] == null) return;
                //    if (row["ChieuRong"] != null || row["ChieuRong"].ToString() != "")
                //    {

                //    }

                //});
                return tbl;
            }
            catch (Exception ex)
            {
                return tbl;
            }

        }
        public static string LayChuoiThung(int tuThung, int denThung)
        {
            List<string> danhSach = new List<string>();

            for (int i = tuThung; i <= denThung; i++)
            {
                danhSach.Add(i.ToString());
            }

            return string.Join(",", danhSach);
        }
        public static void CopyPasteFor1Col(dynamic bandedGridView1, int totalRow, string _fieldName)
        {
            bandedGridView1.CloseEditor();
            bandedGridView1.UpdateCurrentRow();
            var columnFocus = bandedGridView1.FocusedColumn.FieldName;

            GridView view = bandedGridView1 as GridView;
            string clipboardData = Clipboard.GetText();
            byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
            string decodedClipboardData = Encoding.UTF8.GetString(bytes);
            string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length == 0) return;
            var rowHandle = bandedGridView1.FocusedRowHandle;
            foreach (var item in data)
            {
                var value = item.Split('\t')[0];
                bandedGridView1.SetRowCellValue(rowHandle, _fieldName, value);
                rowHandle++;
                if (rowHandle > totalRow - 1) return;
            }
        }
        public static void RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }
        public static void CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    //if (gridView.IndicatorWidth < nNewSize)
                    //{
                    //    gridView.IndicatorWidth = nNewSize + 20;
                    //}

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "*";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    public class ListMergePO
    {
        public string PO { get; set; }
        public string Color { get; set; }
        public string DauSize { get; set; }
    }
}
