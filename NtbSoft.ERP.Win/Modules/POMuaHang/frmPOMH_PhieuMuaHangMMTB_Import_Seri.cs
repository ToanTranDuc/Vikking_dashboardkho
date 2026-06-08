using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMH;
using NtbSoft.ERP.Utils;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmPOMH_PhieuMuaHangMMTB_Import_Seri : DevExpress.XtraEditors.XtraForm
    {
   
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string _maPhieuMH = string.Empty;
        string _maPhieuYC = string.Empty;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private DataTable tblChungLoaiMMTB = new DataTable();
        private DataTable tblVatTuMH = new DataTable();
        private DataTable tblSeriVatTu = new DataTable();
        private DataTable _tblMMTB_Import = new DataTable();
        private List<ERP_POMH_PhieuMuaHang_MMTB_SoSeriEntity>  lstSaveMMTB_Seri = new  List<ERP_POMH_PhieuMuaHang_MMTB_SoSeriEntity>();
        DataRow _focusedRow;

        int _defaultChuKiBaoTri = 90;
        int _defaultKhauHao = 60;

        private List<string> lstColumnReadOnly = new List<string>() {"DonGia", "DonVi", "TenCL", "TenVatTu", "NgayKetThucBH" };
        public frmPOMH_PhieuMuaHangMMTB_Import_Seri(string PhieuMH,string MaPhieuYC, DataRow rowFocused)
        {
         
            InitializeComponent();          
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _focusedRow = rowFocused;
            _maPhieuMH = PhieuMH;
            _maPhieuYC = MaPhieuYC;
            LoadChungLoaiMMTB();
            LoadVatTuThietBiMuaHang();
            LoadMayMocTB_Seri();
         

            
        }

        private  DataTable CreateTable_PhieuMuaHang_MMTB_SoSeri()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("PhieuMH", typeof(string));
            dt.Columns.Add("PhieuYC", typeof(string));
            dt.Columns.Add("POMH", typeof(string));
            dt.Columns.Add("MaCL", typeof(string));
            dt.Columns.Add("TenCL", typeof(string));
            dt.Columns.Add("CodeVatTuMua", typeof(string));
            dt.Columns.Add("SoSeri", typeof(string));
            dt.Columns.Add("NgayBatDauBH", typeof(DateTime));
            //dt.Columns.Add("NgayBatDauBH", typeof(DateTime));
            dt.Columns.Add("LoaiPhieu", typeof(int));
            dt.Columns.Add("TenVatTu", typeof(string));
            dt.Columns.Add("MauMa", typeof(string));
            dt.Columns.Add("XuatXu", typeof(string));
            dt.Columns.Add("HangSX", typeof(string));
            dt.Columns.Add("NamSX", typeof(string));
            dt.Columns.Add("ChuKiBaoTri", typeof(string));
            dt.Columns.Add("MaNhaCC", typeof(string));
            dt.Columns.Add("DonGia", typeof(double));
            dt.Columns.Add("SoLuong", typeof(double));
            dt.Columns.Add("DonVi", typeof(string));
            dt.Columns.Add("KhauHao", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NgayXacNhan", typeof(DateTime));
            dt.Columns.Add("NguoiXacNhan", typeof(string));
            dt.Columns.Add("IsGet", typeof(bool));
            dt.Columns.Add("IsThietBi", typeof(bool));
            dt.Columns.Add("NgayMua", typeof(DateTime));
            dt.Columns.Add("ThoiHanBHText", typeof(string));
            dt.Columns.Add("ThoiHanBH", typeof(string));
            dt.Columns.Add("DVThoiHanBH", typeof(string));
            dt.Columns.Add("ChungLoaiMM", typeof(string));
            return dt;
        }

        private void LoadChungLoaiMMTB()
        {
            tblChungLoaiMMTB = new DataTable();
            try
            {
                string url = $"{URL}ERP_POMH_PhieuMuaHangMMTB_Import_Seri/GET?action=GetChungLoaiMMTB&para1=NONE&para2=NONE";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
               
                if (json != "[]")
                {
                    tblChungLoaiMMTB = JsonConvert.DeserializeObject<DataTable>(json);
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void LoadMayMocTB_Seri()
        {
            try
            {
                _tblMMTB_Import = CreateTable_PhieuMuaHang_MMTB_SoSeri();
                string url = $"{URL}ERP_POMH_PhieuMuaHangMMTB_Import_Seri/GET?action=GetMayMocTB_Seri&para1={_maPhieuMH}&para2={_maPhieuYC}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
          
                if (json != "[]")
                {
                    _tblMMTB_Import = JsonConvert.DeserializeObject<DataTable>(json);
                }
                else
                {
                    setTblImport_MMTB();
                }


                gridControl1.DataSource = _tblMMTB_Import;
                gridControl1.RefreshDataSource();
                gridView1.ExpandAllGroups();
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadVatTuThietBiMuaHang()
        {
            tblVatTuMH = new DataTable();
            try
            {
                string url = $"{URL}ERP_POMH_PhieuMuaHangMMTB_Import_Seri/GET?action=GetTTPhieuMH_MMTB&para1={_maPhieuMH}&para2={_maPhieuYC}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
               
                if (json != "[]")
                {
                    tblVatTuMH = JsonConvert.DeserializeObject<DataTable>(json);
                    _maPhieuYC = tblVatTuMH?.Rows[0]["PhieuYC"]?.ToString();
                }
                SetLayoutTTPhieuMua(_focusedRow);
            }
            catch (Exception ex)
            {

            }
        }
        private void setTblImport_MMTB()
        {
            try
            {
                int IDNew = 1;
              
               
                if (tblVatTuMH != null && tblVatTuMH?.Rows?.Count > 0)
                {
                  
                    foreach (DataRow rowVatTu in tblVatTuMH.Rows)
                    {
                        int.TryParse(rowVatTu["SoLuong"]?.ToString(), out int SLMua);
                                                                  
                        if (rowVatTu["IsThietBi"]?.ToString()?.ToLower() == "true")
                        {
                            for (int i = 0; i < SLMua; i++)
                            {
                                DataRow newRow = _tblMMTB_Import.NewRow();
                                newRow["ID"] = IDNew;
                                newRow["PhieuMH"] = _maPhieuMH;
                                newRow["PhieuYC"] = _maPhieuYC;
                                newRow["POMH"] = rowVatTu["POMua"];
                                newRow["MaCL"] = rowVatTu["MaCL"];
                                newRow["TenCL"] = rowVatTu["TenCL"];
                                newRow["CodeVatTuMua"] = rowVatTu["CodeVatTuMua"];
                                newRow["SoSeri"] = string.Empty;
                                //newRow["NgayKetThucBH"] = DateTime.Now;
                                newRow["NgayBatDauBH"] = DateTime.Now;
                                newRow["LoaiPhieu"] = rowVatTu["LoaiPhieu"];
                                newRow["TenVatTu"] = rowVatTu["TenVatTu"];
                                newRow["MauMa"] = rowVatTu["MauMa"];
                                newRow["XuatXu"] = rowVatTu["XuatXu"];
                                newRow["HangSX"] = rowVatTu["HangSX"];
                                newRow["NamSX"] = string.Empty;
                                newRow["ChuKiBaoTri"] = _defaultChuKiBaoTri;
                                newRow["MaNhaCC"] = rowVatTu["MaNCC"];
                                newRow["DonGia"] = rowVatTu["DonGia"];
                                newRow["SoLuong"] = rowVatTu["SoLuong"];
                                newRow["DonVi"] = rowVatTu["DonVi"];
                                newRow["KhauHao"] =_defaultKhauHao;
                                newRow["GhiChu"] = "";
                                newRow["NgayXacNhan"] =  NtbSoft.ERP.Win.Utils.clsForrmatUtils.ConvertDate(rowVatTu["NgayXacNhan"]);
                                newRow["NguoiXacNhan"] = rowVatTu["NguoiXacNhan"];
                                newRow["IsGet"] = false;
                                newRow["IsThietBi"] = rowVatTu["IsThietBi"];
                                newRow["NgayMua"] = DateTime.Now;
                                newRow["ThoiHanBHText"] = rowVatTu["ThoiHanBHText"];
                                newRow["ThoiHanBH"] = rowVatTu["ThoiHanBH"];
                                newRow["DVThoiHanBH"] = rowVatTu["DVThoiHanBH"];
                                newRow["ChungLoaiMM"] = rowVatTu["ChungLoaiMM"];
                                _tblMMTB_Import.Rows.Add(newRow);
                            }

                            
                        }
                        else
                        {
                            DataRow newRow = _tblMMTB_Import.NewRow();
                            newRow["ID"] = IDNew;
                            newRow["PhieuMH"] = _maPhieuMH;
                            newRow["PhieuYC"] = _maPhieuYC;
                            newRow["POMH"] = rowVatTu["POMua"];
                            newRow["MaCL"] = rowVatTu["MaCL"];
                            newRow["TenCL"] = rowVatTu["TenCL"];
                            newRow["CodeVatTuMua"] = rowVatTu["CodeVatTuMua"];
                            newRow["SoSeri"] = string.Empty;
                            //newRow["NgayKetThucBH"] = DBNull.Value;
                            newRow["NgayBatDauBH"] = DBNull.Value;
                            newRow["LoaiPhieu"] = rowVatTu["LoaiPhieu"];
                            newRow["TenVatTu"] = rowVatTu["TenVatTu"];
                            newRow["MauMa"] = string.Empty;
                            newRow["XuatXu"] = string.Empty;
                            newRow["HangSX"] = string.Empty;
                            newRow["NamSX"] = string.Empty;
                            newRow["ChuKiBaoTri"] = 0;
                            newRow["MaNhaCC"] = rowVatTu["MaNCC"];
                            newRow["DonGia"] = rowVatTu["DonGia"];
                            newRow["SoLuong"] = rowVatTu["SoLuong"];
                            newRow["DonVi"] = rowVatTu["DonVi"];
                            newRow["KhauHao"] = 0;
                            newRow["GhiChu"] = "";
                            newRow["NgayXacNhan"] =  NtbSoft.ERP.Win.Utils.clsForrmatUtils.ConvertDate(rowVatTu["NgayXacNhan"]);
                            newRow["NguoiXacNhan"] = rowVatTu["NguoiXacNhan"];
                            newRow["IsGet"] = false;
                            newRow["IsThietBi"] = rowVatTu["IsThietBi"];
                            newRow["NgayMua"] = DateTime.Now;
                            newRow["ThoiHanBHText"] = rowVatTu["ThoiHanBHText"];
                            newRow["ThoiHanBH"] = rowVatTu["ThoiHanBH"];
                            newRow["DVThoiHanBH"] = rowVatTu["DVThoiHanBH"];
                            newRow["ChungLoaiMM"] = rowVatTu["ChungLoaiMM"];
                            _tblMMTB_Import.Rows.Add(newRow);                          
                        }
                        IDNew++;
                    }
                }
           
            }
            catch(Exception ex)
            {

            }
           


        }
        private void SetLayoutTTPhieuMua(DataRow row)
        {
            txtNhaCC.EditValue = row["TenKH"];
            txtPhieuYV.EditValue = _maPhieuYC;
            txtPOMua.EditValue = row["POMua"];
            layoutControlGroup3.Text = $"Phiếu Mua : {row["TenPhieu"]}";
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {

        }

        private void btnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Chọn file Import Seri MMTB",
                Filter = "Excel (*.xlsx)|*.xlsx"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            if (_tblMMTB_Import == null || _tblMMTB_Import.Rows.Count == 0)
            {
                XtraMessageBox.Show("Chưa có dữ liệu trong bảng MMTB Import.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Import Excel");
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang xử lý...");

            var unmatched = new List<string>();
            int updated = 0;

            try
            {

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var pkg = new ExcelPackage(new FileInfo(ofd.FileName)))
                {
                    var ws = pkg.Workbook.Worksheets[0];
                    if (ws?.Dimension == null) throw new Exception("File Excel rỗng hoặc không đọc được.");

                    for (int row = 5; row <= ws.Dimension.End.Row; row++)
                    {
                        string exChungLoai = CellStr(ws, row, 2);
                        string exTenVatTu = CellStr(ws, row, 3);
                        string exXuatXu = CellStr(ws, row, 4);
                        string exHangSX = CellStr(ws, row, 5);

                        if (string.IsNullOrWhiteSpace(exChungLoai) && string.IsNullOrWhiteSpace(exTenVatTu))
                            continue;

                   
                        DataRow matchedRow = null;
                        foreach (DataRow dr in _tblMMTB_Import.Rows)
                        {
                            if (Eq(SafeStr(dr, "TenCL"), exChungLoai) &&
                                Eq(SafeStr(dr, "TenVatTu"), exTenVatTu) &&
                                
                                string.IsNullOrEmpty(dr["SoSeri"]?.ToString()) 
                                && dr["IsGet"]?.ToString()?.ToLower() == "false"
                                )
                            {
                                matchedRow = dr; break;
                            }
                        }

                        if (matchedRow == null)
                        {
                            unmatched.Add(string.Format("  Dòng {0}: [{1}] | [{2}] | [{3}] | [{4}]",
                                row, exChungLoai, exTenVatTu, exXuatXu, exHangSX));
                            continue;
                        }

                 
                        SetCell(matchedRow, "NamSX", CellStr(ws, row, 7));
                        SetCell(matchedRow, "HangSX", CellStr(ws, row, 5));
                        SetCell(matchedRow, "XuatXu", CellStr(ws, row, 4));
                        SetCell(matchedRow, "MauMa", CellStr(ws, row, 6));
                        SetCell(matchedRow, "ChuKiBaoTri", CellStr(ws, row, 13));
                        SetCell(matchedRow, "KhauHao", CellStr(ws, row, 14));
                        SetCell(matchedRow, "GhiChu", CellStr(ws, row, 15));
                        SetCell(matchedRow, "SoSeri", CellStr(ws, row, 11));
                      
                        var ngayBDBH = CellDate(ws, row, 12);
                        if (ngayBDBH.HasValue) matchedRow["NgayBatDauBH"] = ngayBDBH.Value;
                        var NgayMua = CellDate(ws, row, 10);
                        if (NgayMua.HasValue) matchedRow["NgayMua"] = NgayMua.Value;
                        updated++;
                    }
                }

             
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                
                SaveSeri();
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show("Lỗi import:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       

        private static string CellStr(ExcelWorksheet ws, int row, int col)
        {
            var v = ws.Cells[row, col].Value;
            return v == null ? string.Empty : v.ToString().Trim();
        }

        private static DateTime? CellDate(ExcelWorksheet ws, int row, int col)
        {
            var v = ws.Cells[row, col].Value;
            if (v == null) return null;
            if (v is DateTime) return (DateTime)v;
            if (v is double) { try { return DateTime.FromOADate((double)v); } catch { return null; } }
            DateTime d;
            return DateTime.TryParse(v.ToString(), out d) ? d : (DateTime?)null;
        }

        private static bool Eq(string a, string b) =>
            string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);


        private static void SetCell(DataRow dr, string col, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && dr.Table.Columns.Contains(col))
                dr[col] = value;
        }

        private void btnExportMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Lưu file Excel",
                Filter = "Excel 2010 (*.xlsx)|*.xlsx|Excel 2003 (*.xls)|*.xls",
                FileName = string.Format("Mau-Import-Seri-MMTB-{0}", DateTime.Now.ToString("ddMMyyyyHHss"))
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");

            try
            {
                Export(sfd.FileName);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }

            if (XtraMessageBox.Show("Mở file vừa xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (File.Exists(sfd.FileName))
                        System.Diagnostics.Process.Start("explorer.exe", sfd.FileName);
                }
                catch
                {
                    XtraMessageBox.Show("Không thể mở file.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

      
        private void Export(string exportFileName)
        {
            try
            {
                if (_tblMMTB_Import == null || _tblMMTB_Import.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

               
                string templatePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Templates",
                    "TemplateSeriMMTB.xlsx");

                if (!File.Exists(templatePath))
                {
                    XtraMessageBox.Show(
                        string.Format("Không tìm thấy template:\n{0}", templatePath),
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

               
                File.Copy(templatePath, exportFileName, overwrite: true);

          
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var pkg = new ExcelPackage(new FileInfo(exportFileName)))
                {
                    var ws = pkg.Workbook.Worksheets[0];   

                  
                    var thinBorder = new Action<ExcelRange>(range =>
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    });

                  
                    int row = 5;
                    int stt = 1;
                    DataTable tblMMTB_ImportCopy = new DataTable();
                    var Query = _tblMMTB_Import?.AsEnumerable().Where(x => x["IsGet"]?.ToString()?.ToLower() == "false")?.ToList();
                    if (Query.Any())
                    {
                        tblMMTB_ImportCopy = Query.CopyToDataTable();
                    }

                    foreach (DataRow dr in tblMMTB_ImportCopy.Rows)
                    {
                       

                        ws.Cells[row, 1].Value = stt++;
                        ws.Cells[row, 2].Value = SafeStr(dr, "TenCL");
                        ws.Cells[row, 3].Value = SafeStr(dr, "TenVatTu");
                        ws.Cells[row, 4].Value = SafeStr(dr, "XuatXu");
                        ws.Cells[row, 5].Value = SafeStr(dr, "HangSX");
                        ws.Cells[row, 6].Value = SafeStr(dr, "MauMa");
                        ws.Cells[row, 7].Value = "";
                        ws.Cells[row, 8].Value = SafeDouble(dr, "DonGia");
                        ws.Cells[row, 9].Value = SafeStr(dr, "DonVi");
                        ws.Cells[row, 10].Value = "";
                        ws.Cells[row, 11].Value = "";
                        ws.Cells[row, 12].Value = SafeDate(dr, "NgayBatDauBH");
                        ws.Cells[row, 13].Value = SafeStr(dr, "ChuKiBaoTri");
                        ws.Cells[row, 14].Value = SafeStr(dr, "KhauHao");
                        ws.Cells[row, 15].Value = SafeStr(dr, "GhiChu");

                     
                        ws.Cells[row, 8].Style.Numberformat.Format = "#,##0.##";
                        ws.Cells[row, 10].Style.Numberformat.Format = "dd-MM-yyyy";
                        ws.Cells[row, 12].Style.Numberformat.Format = "dd-MM-yyyy";

                       
                        ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                 
                        thinBorder(ws.Cells[row, 1, row, 15]);


                        row++;
                    }

                   
                    // ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    pkg.Save();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    string.Format("Lỗi xuất file:\n{0}", ex.Message),
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    
        private static string SafeStr(DataRow dr, string col)
        {
            return dr.Table.Columns.Contains(col) && dr[col] != DBNull.Value
                ? dr[col].ToString()
                : string.Empty;
        }

        private static double? SafeDouble(DataRow dr, string col)
        {
            if (!dr.Table.Columns.Contains(col) || dr[col] == DBNull.Value) return null;
            double v;
            return double.TryParse(dr[col].ToString(), out v) ? v : (double?)null;
        }

        private static DateTime? SafeDate(DataRow dr, string col)
        {
            if (!dr.Table.Columns.Contains(col) || dr[col] == DBNull.Value) return null;
            DateTime d =  NtbSoft.ERP.Win.Utils.clsForrmatUtils.ConvertDate(dr[col].ToString());
            return d != DateTime.MinValue ? d : (DateTime?)null;
        }
      

        /*btn Save*/
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveSeri();
        }

        private bool ValidateAllRows()
        {
            try
            {


            } catch(Exception e)
            {

            }
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null) return true;

            var seriList = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                // Kiểm tra NgayBatDauBH nếu IsThietBi == true
                bool isTB = dr["IsThietBi"] != DBNull.Value && Convert.ToBoolean(dr["IsThietBi"]);
                // Kiểm tra NgayMua không trống
                if (dr["NgayMua"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["NgayMua"]?.ToString()))
                {
                    XtraMessageBox.Show($"Dòng {i + 1}: Ngày mua không được để trống!",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    gridView1.FocusedRowHandle = gridView1.GetRowHandle(i);
                    gridView1.FocusedColumn = gridView1.Columns["NgayMua"];
                    return false;
                }

                // Kiểm tra SoSeri trùng trong grid
                string seri = dr["SoSeri"]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(seri))
                {
                    if (seriList.Contains(seri.ToLower()))
                    {
                        XtraMessageBox.Show($"Dòng {i + 1}: Số seri '{seri}' bị trùng trong danh sách!",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        gridView1.FocusedRowHandle = gridView1.GetRowHandle(i);
                        gridView1.FocusedColumn = gridView1.Columns["SoSeri"];
                        return false;
                    }
                    seriList.Add(seri.ToLower());

                    // Kiểm tra SoSeri trùng trên server
                    string urlCheck = $"{URL}ERP_POMH_PhieuMuaHangMMTB_Import_Seri/GET?action=CheckSoSeri&para1={seri}&para2={_maPhieuMH}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                    if (json != "[]")
                    {
                        XtraMessageBox.Show($"Dòng {i + 1}: Số seri '{seri}' đã tồn tại trong hệ thống!",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        gridView1.FocusedRowHandle = gridView1.GetRowHandle(i);
                        gridView1.FocusedColumn = gridView1.Columns["SoSeri"];
                        return false;
                    }
                }

              
                if (isTB)
                {
                    if (string.IsNullOrEmpty(seri))
                    {
                        XtraMessageBox.Show($"Dòng {i + 1}: Vui nhập số Seri cho thiết bị!",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (dr["NgayBatDauBH"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["NgayBatDauBH"]?.ToString()))
                    {
                        XtraMessageBox.Show($"Dòng {i + 1}: Ngày bắt đầu bảo hành không được trống khi là thiết bị!",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        gridView1.FocusedRowHandle = gridView1.GetRowHandle(i);
                        gridView1.FocusedColumn = gridView1.Columns["NgayBatDauBH"];
                        return false;
                    }
                }
            }

            return true;
        }

        private void SaveSeri()
        {
            try
            {
                this.ActiveControl = simpleButton1;

                // Validate toàn bộ grid trước khi save
                if (!ValidateAllRows()) return;

                lstSaveMMTB_Seri = new List<ERP_POMH_PhieuMuaHang_MMTB_SoSeriEntity>();
                DataTable tblSave = gridControl1.DataSource as DataTable;
                if (_tblMMTB_Import != null && _tblMMTB_Import?.Rows?.Count == 0) return;

                lstSaveMMTB_Seri = _tblMMTB_Import.AsEnumerable().Select(dr =>
                {
                    DateTime? ngayKetThuc = null;
                    DateTime? ngayBatDau = SafeDate(dr, "NgayBatDauBH");
                    string donVi = SafeStr(dr, "DVThoiHanBH")?.Trim().ToLower();
                    int soTG = 0;
                    int.TryParse(SafeStr(dr, "ThoiHanBH"), out soTG);

                    if (ngayBatDau.HasValue && soTG > 0 && !string.IsNullOrEmpty(donVi))
                    {
                        if (donVi == "tháng")
                            ngayKetThuc = ngayBatDau.Value.AddMonths(soTG);
                        else if (donVi == "năm")
                            ngayKetThuc = ngayBatDau.Value.AddYears(soTG);
                        else
                            ngayKetThuc = ngayBatDau.Value.AddDays(soTG);
                    }

                    return new ERP_POMH_PhieuMuaHang_MMTB_SoSeriEntity
                    {
                        ID = dr["ID"] != DBNull.Value ? (long?)Convert.ToInt64(dr["ID"]) : null,
                        PhieuMH = _maPhieuMH,
                        PhieuYC = _maPhieuYC,
                        POMH = SafeStr(dr, "POMH"),
                        MaCL = SafeStr(dr, "MaCL"),
                        CodeVatTuMua = SafeStr(dr, "CodeVatTuMua"),
                        SoSeri = SafeStr(dr, "SoSeri"),
                        NgayBatDauBH = ngayBatDau,
                        NgayKetThucBH = ngayKetThuc,
                        LoaiPhieu = dr["LoaiPhieu"] != DBNull.Value ? (int?)Convert.ToInt32(dr["LoaiPhieu"]) : null,
                        TenVatTu = SafeStr(dr, "TenVatTu"),
                        MauMa = SafeStr(dr, "MauMa"),
                        XuatXu = SafeStr(dr, "XuatXu"),
                        HangSX = SafeStr(dr, "HangSX"),
                        NamSX = SafeStr(dr, "NamSX"),
                        ChuKiBaoTri = SafeStr(dr, "ChuKiBaoTri"),
                        MaNhaCC = SafeStr(dr, "MaNhaCC"),
                        DonGia = SafeDouble(dr, "DonGia").HasValue ? (float?)Convert.ToSingle(SafeDouble(dr, "DonGia").Value) : null,
                        SoLuong = SafeDouble(dr, "SoLuong").HasValue ? (float?)Convert.ToSingle(SafeDouble(dr, "SoLuong").Value) : null,
                        DonVi = SafeStr(dr, "DonVi"),
                        KhauHao = SafeStr(dr, "KhauHao"),
                        GhiChu = SafeStr(dr, "GhiChu"),
                        NgayXacNhan = SafeDate(dr, "NgayXacNhan"),
                        NguoiXacNhan = SafeStr(dr, "NguoiXacNhan"),
                        IsGet = dr["IsGet"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["IsGet"]) : null,
                        IsThietBi = dr["IsThietBi"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["IsThietBi"]) : null,
                        NgayMua = SafeDate(dr, "NgayMua"),
                    };
                }).ToList();

                string url = string.Format("{0}", URL + "ERP_POMH_PhieuMuaHangMMTB_Import_Seri/Post?action=POST");
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSaveMMTB_Seri); }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception ex)
            {

            }
            
        }
        /*btnRefresh*/
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadVatTuThietBiMuaHang();
            LoadMayMocTB_Seri();        
            
        }

        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
            try
            {


                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                DataRow rowFocused = gridView1.GetFocusedDataRow() as DataRow;
                if (rowFocused == null) return;
                if (view == null) return;
                bool.TryParse(rowFocused["IsGet"]?.ToString(), out bool IsGet);

                if (!lstColumnReadOnly.Contains(view.FocusedColumn.FieldName) && !IsGet)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }              
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            string fieldName = view.FocusedColumn?.FieldName;
            int rowHandle = view.FocusedRowHandle;

            
            if (fieldName == "NgayMua")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Ngày mua không được để trống!";
                    return;
                }
                if (!DateTime.TryParse(e.Value.ToString(), out _))
                {
                    e.Valid = false;
                    e.ErrorText = "Ngày mua không hợp lệ!";
                    return;
                }
            }

          
            if (fieldName == "SoSeri")
            {
                var isTBVal = view.GetRowCellValue(rowHandle, "IsThietBi");
                bool isTB = isTBVal != null && isTBVal != DBNull.Value && Convert.ToBoolean(isTBVal);
                string newSeri = e.Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(newSeri) && !isTB) return;
                if (string.IsNullOrWhiteSpace(newSeri) && isTB)
                {
                    e.Valid = false;
                    e.ErrorText = $"Số seri thiết bị không bỏ trống";
                    return;
                }
                    DataTable dt = gridControl1.DataSource as DataTable;
                if (dt == null) return;

                int currentListIndex = view.GetDataSourceRowIndex(rowHandle);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i == currentListIndex) continue; // bỏ qua dòng hiện tại

                    string existingSeri = dt.Rows[i]["SoSeri"]?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(existingSeri) &&
                        string.Equals(existingSeri, newSeri, StringComparison.OrdinalIgnoreCase))
                    {
                        e.Valid = false;
                        e.ErrorText = $"Số seri '{newSeri}' đã tồn tại ở dòng {i + 1}, không được trùng!";
                        return;
                    }
                }
                string url = $"{URL}ERP_POMH_PhieuMuaHangMMTB_Import_Seri/GET?action=CheckSoSeri&para1={newSeri}&para2={_maPhieuMH}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json != "[]")
                {
                    e.Valid = false;
                    e.ErrorText = $"Số seri '{newSeri}' đã có , không được trùng!";
                    return;
                }

            }

        
            if (fieldName == "NgayBatDauBH")
            {
                var isTBVal = view.GetRowCellValue(rowHandle, "IsThietBi");
                bool isTB = isTBVal != null && isTBVal != DBNull.Value && Convert.ToBoolean(isTBVal);

                if (isTB)
                {
                    if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Ngày bắt đầu bảo hành không được để trống khi là thiết bị!";
                        return;
                    }
                    if (!DateTime.TryParse(e.Value.ToString(), out _))
                    {
                        e.Valid = false;
                        e.ErrorText = "Ngày bắt đầu bảo hành không hợp lệ!";
                        return;
                    }
                }
            }
        }

        private void gridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }

        private void searchVatTu_TextChanged(object sender, EventArgs e)
        {
            string keyword = searchVatTu.Text.Trim();
            ApplyFilterMMTB(keyword);
        }
        
        private void ApplyFilterMMTB(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                gridView1.ActiveFilter.Clear();
                return;
            }

            gridView1.ActiveFilterString = $@"
            [TenVatTu] LIKE '%{keyword}%'
            OR [MauMa] LIKE '%{keyword}%'
            OR [XuatXu] LIKE '%{keyword}%'
            OR [HangSX] LIKE '%{keyword}%'
            OR [NamSX] LIKE '%{keyword}%'
            OR [TenCL] LIKE '%{keyword}%'
            OR [SoSeri] LIKE '%{keyword}%'
           
        ";
        }

        #region Styte grid
        private void griview_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }

        }
        private void griview_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;

                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;

                int groupLevel = view.GetRowLevel(e.RowHandle);

                GridColumn groupColumn = info.Column;


                info.GroupText = string.Format("{0}", info.GroupValueText);

                if (view.IsGroupRow(e.RowHandle))
                {

                    Color textColor = Color.Black;

                    switch (groupLevel)

                    {

                        case 0: textColor = Color.MediumBlue; break;

                        case 1: textColor = Color.Maroon; break;

                    }

                    e.Appearance.ForeColor = textColor;

                    e.DefaultDraw();

                    e.Handled = true;

                }

            }
            catch (Exception ex)
            {

            }
        }

        private void bgvVatTuMMTB_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            bool isEditable = !lstColumnReadOnly.Contains(e.Column.FieldName);

            if (isEditable)
            {
                e.Appearance.BackColor = Color.FromArgb(192, 255, 255);

            }

        }
        private void grv_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null || e.Value == DBNull.Value || e.Value.ToString() == "")
                {
                    if (e.Column.FieldName == "DonGia") e.DisplayText = "-";
                    return;
                }

                var view = sender as GridView;
                
                if (e.Column.FieldName == "DonGia")
                {

                    if (e.Value == null || e.Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(e.Value?.ToString()) ||
                        !decimal.TryParse(e.Value.ToString(), out decimal value) ||
                        value == 0m)
                    {
                        e.DisplayText = "-";
                        return;
                    }


                    var nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    nfi.NumberGroupSeparator = ",";

                    int decimals = (decimal.GetBits(value)[3] >> 16) & 0x000000FF;
                    decimals = decimals == 0 ? 0 : Math.Min(decimals, 4);

                    string format = decimals == 0 ? "N0" : $"N{decimals}";

                    e.DisplayText = value.ToString(format, nfi);
                    return;
                }
                if (e.Column == colBatDauBH || e.Column == colKetThucBH || e.Column == colNgayMua)
                {
                    if (e.Value == null || e.Value == DBNull.Value || string.IsNullOrWhiteSpace(e.Value?.ToString()))
                    {
                        e.DisplayText = "";
                        return;
                    }
                    DateTime dt =  NtbSoft.ERP.Win.Utils.clsForrmatUtils.ConvertDate(e.Value.ToString());
                    e.DisplayText = dt == DateTime.MinValue ? "" : dt.ToString("dd-MM-yyyy");
                }

               

            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                if (!e.IsGetData) return;

                // Lấy giá trị từ các cột của dòng hiện tại theo rowHandle
                var thoiHanBHVal = view.GetRowCellValue(e.ListSourceRowIndex, "ThoiHanBH");
                var dvThoiHanBHVal = view.GetRowCellValue(e.ListSourceRowIndex, "DVThoiHanBH");
                var ngayBatDauVal = view.GetRowCellValue(e.ListSourceRowIndex, "NgayBatDauBH");

                if (thoiHanBHVal == null || dvThoiHanBHVal == null || ngayBatDauVal == null)
                    return;

                if (!int.TryParse(thoiHanBHVal.ToString(), out int soTGHieuLuc) || soTGHieuLuc <= 0)
                    return;

                if (!DateTime.TryParse(ngayBatDauVal.ToString(), out DateTime ngayBatDau))
                    return;

                string donVi = dvThoiHanBHVal.ToString().Trim().ToLower();

                DateTime ngayKetThuc;

                if (donVi == "tháng")
                {
                    ngayKetThuc = ngayBatDau.AddMonths(soTGHieuLuc);
                }
                else if (donVi == "năm")
                {
                    ngayKetThuc = ngayBatDau.AddYears(soTGHieuLuc);
                }
                else // ngày
                {
                    ngayKetThuc = ngayBatDau.AddDays(soTGHieuLuc);
                }

                e.Value = ngayKetThuc;
            }
            catch(Exception ex)
            {

            }
          
        }
    }
}