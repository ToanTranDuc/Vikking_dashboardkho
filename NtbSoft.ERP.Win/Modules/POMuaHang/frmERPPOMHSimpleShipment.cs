using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
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
    public partial class frmERPPOMHSimpleShipment : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        string _phieuMH = string.Empty;string _POMH = string.Empty;
        string Dot = string.Empty;string STTDot = string.Empty;
        int MaxSTTDot = 1;

        DataRow _rowFocused;
        private DataTable tblSaveShipment = new DataTable();
        private DataTable tblVatTuShipment = new DataTable();

        private DataTable tblDot = new DataTable();
        DataTable tblDonVi = new DataTable();
        private List<string> lstColumnEdit = new List<string>() { "SoLuongThucTe", "SoGhiDauCay", "GhiChu" };
        public frmERPPOMHSimpleShipment(DataRow rowFocused)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _rowFocused = rowFocused;
           
        }

        protected override void OnLoad(EventArgs e)
        {
            loadDonVi();
            SetTTPhieuMua();
            InitSearchLookupDot();   
            LoadMaxDot();            
            LoadVatTuShipment();
        }
        private void SetTTPhieuMua()
        {
            GroupPhieuMua.Text = $"Phiếu mua : {_rowFocused["TenPhieu"]?.ToString()}";
            txtNhaCungCap.EditValue = _rowFocused["TenKH"]?.ToString();
            txtP0Mua.EditValue = _rowFocused["POMua"]?.ToString();
            txtDiaChi.EditValue = _rowFocused["DiaChi"]?.ToString();
            txtEmail.EditValue = _rowFocused["Mail"]?.ToString();


            _phieuMH = _rowFocused["MaPhieuMH"]?.ToString();
            _POMH = _rowFocused["POMua"]?.ToString();
        }
        private DataTable CreateTableSaveVatTuSimpment()
        {
            DataTable tblSaveShipment = new DataTable("tblSave");

            tblSaveShipment.Columns.Add("ID", typeof(long)); // bigint
            tblSaveShipment.Columns.Add("SoLoID", typeof(string));
            tblSaveShipment.Columns.Add("MaNPL", typeof(string));
            tblSaveShipment.Columns.Add("MaVTID", typeof(string));
            tblSaveShipment.Columns.Add("MaMauVT", typeof(string));
            tblSaveShipment.Columns.Add("SoKien", typeof(string));
            tblSaveShipment.Columns.Add("SoLoT", typeof(string));
            tblSaveShipment.Columns.Add("MaHaiQuan", typeof(string));
            tblSaveShipment.Columns.Add("MaKeToan", typeof(string));
            tblSaveShipment.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblSaveShipment.Columns.Add("NW", typeof(decimal));
            tblSaveShipment.Columns.Add("GW", typeof(decimal));
            tblSaveShipment.Columns.Add("BarCode", typeof(string));
            tblSaveShipment.Columns.Add("GhiChu", typeof(string));
            tblSaveShipment.Columns.Add("IsNPL", typeof(bool));
            tblSaveShipment.Columns.Add("KhoVaiID", typeof(string));
            tblSaveShipment.Columns.Add("SoKienParent", typeof(string));
            tblSaveShipment.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblSaveShipment.Columns.Add("DonGia", typeof(decimal));
            tblSaveShipment.Columns.Add("ThanhTien", typeof(decimal));
            tblSaveShipment.Columns.Add("Pallet", typeof(string));
            tblSaveShipment.Columns.Add("MaDVVT", typeof(string));
            tblSaveShipment.Columns.Add("MauVTID", typeof(string));
            tblSaveShipment.Columns.Add("IsNK", typeof(bool));
            tblSaveShipment.Columns.Add("NgayNhapKho", typeof(DateTime));
            tblSaveShipment.Columns.Add("SoPhieu", typeof(string));
            tblSaveShipment.Columns.Add("NguoiTaoNhapKho", typeof(string));
            tblSaveShipment.Columns.Add("NgayTaoNhapKho", typeof(DateTime));
            tblSaveShipment.Columns.Add("NguoiSuaNhapKho", typeof(string));
            tblSaveShipment.Columns.Add("NgaySuaNhapKho", typeof(DateTime));
            tblSaveShipment.Columns.Add("STTPhieu", typeof(int));
            tblSaveShipment.Columns.Add("KienGoc", typeof(string));
            tblSaveShipment.Columns.Add("BarCodeGoc", typeof(string));
            tblSaveShipment.Columns.Add("SoKienHienThi", typeof(string));
            tblSaveShipment.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblSaveShipment.Columns.Add("MaDVCD", typeof(string));
            tblSaveShipment.Columns.Add("TenDVCD", typeof(string));
            tblSaveShipment.Columns.Add("tileNW", typeof(decimal));
            tblSaveShipment.Columns.Add("tileGW", typeof(decimal));
            tblSaveShipment.Columns.Add("TienTe", typeof(string));
            tblSaveShipment.Columns.Add("MaVTGhep", typeof(string));
            tblSaveShipment.Columns.Add("MaNhom", typeof(string));
            tblSaveShipment.Columns.Add("QuyDoiID", typeof(string));
            tblSaveShipment.Columns.Add("POMua", typeof(string));
            tblSaveShipment.Columns.Add("Batch", typeof(string));
            tblSaveShipment.Columns.Add("SLTong", typeof(decimal));
            tblSaveShipment.Columns.Add("STTChonVT", typeof(int));
            tblSaveShipment.Columns.Add("IsKiemKe", typeof(bool));
            tblSaveShipment.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblSaveShipment.Columns.Add("TuoiTonKho", typeof(decimal));
            tblSaveShipment.Columns.Add("STTDot", typeof(string));
            tblSaveShipment.Columns.Add("Dot", typeof(string));
            tblSaveShipment.Columns.Add("POMHID", typeof(string));

            return tblSaveShipment;
        }


        private void CreateShipmentDataTable()
        {
            tblVatTuShipment = new DataTable();
            tblVatTuShipment.Columns.Add("ID", typeof(int));
            tblVatTuShipment.Columns.Add("MaCLVT", typeof(string));
            tblVatTuShipment.Columns.Add("SoLoID", typeof(string));
            tblVatTuShipment.Columns.Add("TenPhieu", typeof(string));
            tblVatTuShipment.Columns.Add("MaVTID", typeof(string));
            tblVatTuShipment.Columns.Add("MauVTID", typeof(string));
            tblVatTuShipment.Columns.Add("KhoVaiID", typeof(string));
            tblVatTuShipment.Columns.Add("MaNPL", typeof(string));   
            tblVatTuShipment.Columns.Add("MaDVVT", typeof(string));
            tblVatTuShipment.Columns.Add("POMua", typeof(string));
            tblVatTuShipment.Columns.Add("DonGia", typeof(decimal));
            tblVatTuShipment.Columns.Add("MaTienTe", typeof(string));   
            tblVatTuShipment.Columns.Add("SLMua", typeof(decimal));  
            tblVatTuShipment.Columns.Add("MaNCC", typeof(string));
            tblVatTuShipment.Columns.Add("TenNhaCC", typeof(string));   
            tblVatTuShipment.Columns.Add("ItemCode", typeof(string));   
            tblVatTuShipment.Columns.Add("MaVTGhep", typeof(string));
            tblVatTuShipment.Columns.Add("MoTa", typeof(string));  
            tblVatTuShipment.Columns.Add("ColorCode", typeof(string));   
            tblVatTuShipment.Columns.Add("MauVT", typeof(string));
            tblVatTuShipment.Columns.Add("KhoVai", typeof(string));
            tblVatTuShipment.Columns.Add("ChungLoaiVatTu", typeof(string));
            tblVatTuShipment.Columns.Add("Batch", typeof(string));
            tblVatTuShipment.Columns.Add("SoLot", typeof(string));  
            tblVatTuShipment.Columns.Add("NW", typeof(decimal));
            tblVatTuShipment.Columns.Add("GW", typeof(decimal));
            tblVatTuShipment.Columns.Add("NgayTaoNhapKho", typeof(DateTime));
            tblVatTuShipment.Columns.Add("NguoiTaoNhapKho", typeof(string));
            tblVatTuShipment.Columns.Add("SoKien", typeof(string));
            tblVatTuShipment.Columns.Add("SoKienHienThi", typeof(string));
            tblVatTuShipment.Columns.Add("SoKienParent", typeof(string));
            tblVatTuShipment.Columns.Add("MaDVCD", typeof(string));
            tblVatTuShipment.Columns.Add("DVTT", typeof(string));  
            tblVatTuShipment.Columns.Add("DVTinhCD", typeof(string));  
            tblVatTuShipment.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblVatTuShipment.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblVatTuShipment.Columns.Add("POMHID", typeof(string));  
            tblVatTuShipment.Columns.Add("Dot", typeof(string));
            tblVatTuShipment.Columns.Add("STTDot", typeof(string));
            tblVatTuShipment.Columns.Add("GhiChu", typeof(string));
            tblVatTuShipment.Columns.Add("LoaiNPL", typeof(string));  
            tblVatTuShipment.Columns.Add("IsNPL", typeof(bool));
            tblVatTuShipment.Columns.Add("TongKien", typeof(string));
            tblVatTuShipment.Columns.Add("SLTong", typeof(string));
            tblVatTuShipment.Columns.Add("TrangThai", typeof(string));
            tblVatTuShipment.Columns.Add("IsUse", typeof(bool));

        }
        private void loadDonVi()
        {
            tblDonVi = new DataTable();
            string url = $"{URL}ERPNhapKhoNPL/GET?Action=GETDVCD&para=NONE";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


            if (json != "[]")
            {
                tblDonVi = JsonConvert.DeserializeObject<DataTable>(json);
            }
            

        }
        private void LoadMaxDot()
        {
            DataTable tbl = new DataTable();
            string url = $"{URL}SimpleShipment/Get?action=GetSTTDotMax&para1={_phieuMH}&para2={STTDot}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tbl = JsonConvert.DeserializeObject<DataTable>(json);


            }

            if(tbl!= null && tbl?.Rows?.Count > 0)
            {
                int.TryParse(tbl?.Rows[0]["MaxDot"]?.ToString(), out MaxSTTDot);
            }
        }
        private void LoadVatTuShipment()
        {
            try
            {
                CreateShipmentDataTable();
                string url = $"{URL}SimpleShipment/Get?action=GetVatTuShiment&para1={_phieuMH}&para2={STTDot}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblVatTuShipment = JsonConvert.DeserializeObject<DataTable>(json);
                  

                }
                if(tblVatTuShipment != null && tblVatTuShipment?.Rows?.Count > 0)
                {
                   

                    grcVatTuShipment.DataSource = SetTblVatTuShipment();


                }
                else
                {
                    grcVatTuShipment.DataSource = tblVatTuShipment;
                } 
                

            }
            catch (Exception ex)
            { }
        }
       
         private DataTable SetTblVatTuShipment()
        {
            // Convert sang DataTable
            DataTable dtResult = new DataTable();

            if (tblVatTuShipment != null && tblVatTuShipment?.Rows?.Count > 0)
            {
                var query = from DataRow row in tblVatTuShipment.Rows
                            select new
                            {
                                MaCLVT = row["MaCLVT"] == DBNull.Value ? "" : row["MaCLVT"].ToString(),
                                SoLoID = row["SoLoID"] == DBNull.Value ? "" : row["SoLoID"].ToString(),
                                TenPhieu = row["TenPhieu"] == DBNull.Value ? "" : row["TenPhieu"].ToString(),
                                MaVTID = row["MaVTID"] == DBNull.Value ? "" : row["MaVTID"].ToString(),
                                MauVTID = row["MauVTID"] == DBNull.Value ? "" : row["MauVTID"].ToString(),
                                KhoVaiID = row["KhoVaiID"] == DBNull.Value ? "" : row["KhoVaiID"].ToString(),
                                MaNPL = row["MaNPL"] == DBNull.Value ? "" : row["MaNPL"].ToString(),
                                MaDVVT = row["MaDVVT"] == DBNull.Value ? "" : row["MaDVVT"].ToString(),
                                POMua = row["POMua"] == DBNull.Value ? "" : row["POMua"].ToString(),
                                DonGia = row["DonGia"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DonGia"]),
                                MaTienTe = row["MaTienTe"] == DBNull.Value ? "" : row["MaTienTe"].ToString(),
                                SLMua = row["SLMua"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SLMua"]),
                                MaNCC = row["MaNCC"] == DBNull.Value ? "" : row["MaNCC"].ToString(),
                                TenNhaCC = row["TenNhaCC"] == DBNull.Value ? "" : row["TenNhaCC"].ToString(),
                                ItemCode = row["ItemCode"] == DBNull.Value ? "" : row["ItemCode"].ToString(),
                                MaVTGhep = row["MaVTGhep"] == DBNull.Value ? "" : row["MaVTGhep"].ToString(),
                                MoTa = row["MoTa"] == DBNull.Value ? "" : row["MoTa"].ToString(),
                                ColorCode = row["ColorCode"] == DBNull.Value ? "" : row["ColorCode"].ToString(),
                                MauVT = row["MauVT"] == DBNull.Value ? "" : row["MauVT"].ToString(),
                                KhoVai = row["KhoVai"] == DBNull.Value ? "" : row["KhoVai"].ToString(),
                                ChungLoaiVatTu = row["ChungLoaiVatTu"] == DBNull.Value ? "" : row["ChungLoaiVatTu"].ToString(),
                                DVTT = row["DVTT"] == DBNull.Value ? "" : row["DVTT"].ToString(),
                                POMHID = row["POMHID"] == DBNull.Value ? "" : row["POMHID"].ToString(),
                                LoaiNPL = row["LoaiNPL"] == DBNull.Value ? "" : row["LoaiNPL"].ToString(),
                                IsNPL = row["IsNPL"] == DBNull.Value ? false : Convert.ToBoolean(row["IsNPL"]),
                                TongKien = row["TongKien"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TongKien"]),
                                SLTong = row["SLTong"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SLTong"])
                            };

        
                dtResult.Columns.Add("MaCLVT");
                dtResult.Columns.Add("SoLoID");
                dtResult.Columns.Add("TenPhieu");
                dtResult.Columns.Add("MaVTID");
                dtResult.Columns.Add("MauVTID");
                dtResult.Columns.Add("KhoVaiID");
                dtResult.Columns.Add("MaNPL");
                dtResult.Columns.Add("MaDVVT");
                dtResult.Columns.Add("POMua");
                dtResult.Columns.Add("DonGia", typeof(decimal));
                dtResult.Columns.Add("MaTienTe");
                dtResult.Columns.Add("SLMua", typeof(decimal));
                dtResult.Columns.Add("MaNCC");
                dtResult.Columns.Add("TenNhaCC");
                dtResult.Columns.Add("ItemCode");
                dtResult.Columns.Add("MaVTGhep");
                dtResult.Columns.Add("MoTa");
                dtResult.Columns.Add("ColorCode");
                dtResult.Columns.Add("MauVT");
                dtResult.Columns.Add("KhoVai");
                dtResult.Columns.Add("ChungLoaiVatTu");
                dtResult.Columns.Add("DVTT");
                dtResult.Columns.Add("POMHID");
                dtResult.Columns.Add("LoaiNPL");
                dtResult.Columns.Add("IsNPL", typeof(bool));
                dtResult.Columns.Add("TongKien", typeof(decimal));
                dtResult.Columns.Add("SLTong", typeof(decimal));

                foreach (var item in query.Distinct())
                {
                    dtResult.Rows.Add(
                        item.MaCLVT, item.SoLoID, item.TenPhieu, item.MaVTID, item.MauVTID,
                        item.KhoVaiID, item.MaNPL, item.MaDVVT, item.POMua, item.DonGia,
                        item.MaTienTe, item.SLMua, item.MaNCC, item.TenNhaCC, item.ItemCode,
                        item.MaVTGhep, item.MoTa, item.ColorCode, item.MauVT, item.KhoVai,
                        item.ChungLoaiVatTu, item.DVTT, item.POMHID, item.LoaiNPL, item.IsNPL,
                        item.TongKien, item.SLTong
                    );
                }

            


            }

            return dtResult;
        }

        private void InitSearchLookupDot()
        {
            try
            {
                
                //string urlNCC = $"{URL}SimpleShipment/Get?action=GetDot&para1={_phieuMH}";
                //string jsonNCC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNCC); }).Result;
                //if (jsonNCC == "[]")
                //{
                //    MaxSTTDot = 1;
                //    STTDot = MaxSTTDot.ToString();
                //    Dot = MaxSTTDot.ToString();

                //    tblDot = new DataTable();
                //    tblDot.Columns.Add("Dot", typeof(string));
                //    tblDot.Columns.Add("STTDot", typeof(string));


                //    DataRow rowDot = tblDot.NewRow();
                //    rowDot["Dot"] = Dot;
                //    rowDot["STTDot"] = STTDot;
                //    tblDot.Rows.Add(rowDot);
                //}
                //else
                //{
                //    tblDot = JsonConvert.DeserializeObject<DataTable>(jsonNCC);                                       
                //}
                //searchLookUpDotShipment.Properties.ValueMember = "STTDot";
                //searchLookUpDotShipment.Properties.DisplayMember = "Dot";
                //searchLookUpDotShipment.Properties.DataSource = tblDot;
                //searchLookUpDotShipment.EditValue = tblDot.Rows[0]["STTDot"];
            }
            catch (Exception ex)
            {

            }
        }
        private void SearchLookUpDotShipment_EditValueChanged(object sender, EventArgs e)
        {
            //var view = searchLookUpDotShipment.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
           /* if (view == null) return;

            DataRow selectedRow = view.GetFocusedDataRow() as DataRow;
            if (selectedRow == null) return;


            Dot = selectedRow["Dot"] == DBNull.Value ? "" : selectedRow["Dot"].ToString();
            STTDot = selectedRow["STTDot"] == DBNull.Value ? ""  : selectedRow["STTDot"]?.ToString();
            LoadVatTuShipment();*/


        }
        private void grvVatTuShipment_DataSourceChanged(object sender, EventArgs e)
        {
           if(tblVatTuShipment != null && tblVatTuShipment?.Rows?.Count > 0)
            {
                DataRow rowVatTuShipment = tblVatTuShipment?.Rows[0];
                LoadKienVatTuShipment(rowVatTuShipment);
            }
            else
            {
                grcSoKienShipment.DataSource = new DataTable();
            }
        }

        private void grvVatTuShipment_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            DataRow rowVatTuFocused = view.GetFocusedDataRow();
            if (rowVatTuFocused == null) return;
            LoadKienVatTuShipment(rowVatTuFocused);
        }
       
        private void LoadKienVatTuShipment(DataRow rowVatTuFocused)
        {
            var Query = tblVatTuShipment.AsEnumerable().Where(x => x["MaCLVT"].Equals(rowVatTuFocused["MaCLVT"]) && x["MaVTID"].Equals(rowVatTuFocused["MaVTID"])
                                                 && x["MauVTID"].Equals(rowVatTuFocused["MauVTID"]) && x["KhoVaiID"].Equals(rowVatTuFocused["KhoVaiID"]) && !string.IsNullOrEmpty(x["SoKien"]?.ToString()));
            if(Query!= null &&  Query.Any())
            {
                grcSoKienShipment.DataSource = Query?.CopyToDataTable();
            }
            else
            {
                grcSoKienShipment.DataSource = new DataTable();
            }

            grcSoKienShipment.RefreshDataSource();

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
            bool isEditable = lstColumnEdit.Contains(e.Column.FieldName);

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
                //if (e.Column == colBatDauBH || e.Column == colKetThucBH || e.Column == colNgayMua)
                //{
                //    if (e.Value == null || e.Value == DBNull.Value || string.IsNullOrWhiteSpace(e.Value?.ToString()))
                //    {
                //        e.DisplayText = "";
                //        return;
                //    }
                //    DateTime tblVatTuShipment = NtbSoft.ERP.Win.Utils.clsForrmatUtils.ConvertDate(e.Value.ToString());
                //    e.DisplayText = tblVatTuShipment == DateTime.MinValue ? "" : tblVatTuShipment.ToString("dd-MM-yyyy");
                //}



            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        private void grvVatTuShipment_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                if (!e.IsGetData) return;

                
               
            }
            catch (Exception ex)
            {

            }

        }
        private void grvVatTuShipment_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if(view.FocusedColumn == colThemKien)
            {

                try
                {

                    //if (string.IsNullOrWhiteSpace(txtLo.Text))
                    //{
                    //    MessageBox.Show("Vui lòng nhập Số Lô.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    txtLo.Focus();
                    //    return;
                    //}

                    List<string> currentList = new List<string>();

                    decimal tileNW = 0m, tileGW = 0m;
                    string madvcd = string.Empty, tendvcd = string.Empty, batch = string.Empty;
                    int sttChonVT = 1;
                    //int sttTrongNhom = 1;
                    //if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                    //{
                    //    sttChonVT = Convert.ToInt32(dr["STTChonVT"]);
                    //}
                    DataRow _dr = grvVatTuShipment.GetFocusedDataRow();
                    if (_dr != null)
                    {
                        tileNW = 0m;
                        tileGW = 0m;
                        madvcd = _dr["MaDVCD"].ToString();
                        tendvcd = _dr["DVTinhCD"].ToString();
                        batch = _dr["Batch"].ToString();
                    }

                    frmERPPOMH_AddKienShipmnet frm = new frmERPPOMH_AddKienShipmnet(_dr, tblVatTuShipment, _rowFocused["TenPhieu"]?.ToString(), true, tileNW, tileGW, madvcd, tendvcd, _phieuMH, batch);
                    frm.StartPosition = FormStartPosition.CenterScreen;

                    frm.ThongSoKienChanged += (list) =>
                    {
                        //tblNhapKho = frmERPNhapKhoNPL_KhaiBao.dt;
                        //string maVTID = dr["MaVTID"].ToString();
                        //string maMauVT = dr["MauVTID"].ToString();
                        //string khoVaiID = dr["KhoVaiID"].ToString();
                        //string MaNhom = dr["MaNhom"].ToString();
                        //string MaVTGhep = dr["MaVTGhep"].ToString();
                        //var filteredRows = tblNhapKho.AsEnumerable()
                        //    .Where(row => row["MaVTID"].ToString() == maVTID &&
                        //                  row["MauVTID"].ToString() == maMauVT &&
                        //                  row["KhoVaiID"].ToString() == khoVaiID &&
                        //                  row["MaNhom"].ToString() == MaNhom &&
                        //                  row["MaVTGhep"].ToString() == MaVTGhep);

                        //foreach (var row in filteredRows)
                        //{
                        //    row["STT"] = sttChonVT;
                        //    row["sttChonVT"] = sttChonVT;
                        //}
                        //if (filteredRows.Any())
                        //{
                        //    // Chuyển đổi trực tiếp sang DataTable mới
                        //    DataTable tblNhapKhoFiltered = filteredRows.CopyToDataTable();
                        //    gCNLNhapKho.DataSource = tblNhapKhoFiltered;
                        //}
                        //else
                        //{

                        //    gCNLNhapKho.DataSource = tblNhapKho.Clone();
                        //}
                    };
                    frm.ShowDialog();



                }
                catch (Exception ex)
                {

                }
            }
        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveShipment();
        }

        private void SaveShipment()
        {
            this.ActiveControl = simpleButton1;
            tblSaveShipment =  CreateTableSaveVatTuSimpment();
            try
            {
                foreach (DataRow sourceRow in tblVatTuShipment.Rows)
                {
                    DataRow newRow = tblSaveShipment.NewRow();

                    if (string.IsNullOrEmpty(sourceRow["SoKien"]?.ToString())) continue;
                    newRow["SoLoID"] =_phieuMH;
                    newRow["MaNPL"] = sourceRow["MaNPL"] ?? DBNull.Value;
                    newRow["MaVTID"] = sourceRow["MaVTID"] ?? DBNull.Value;
                    newRow["MaMauVT"] = sourceRow["ColorCode"] ?? DBNull.Value; // ColorCode -> MaMauVT
                    //newRow["MauVT"] = sourceRow["MauVT"] ?? DBNull.Value;
                    newRow["SoKien"] = sourceRow["SoKien"] ?? DBNull.Value;
                    newRow["SoLot"] = sourceRow["SoLot"] ?? DBNull.Value;
                    newRow["MaKeToan"] = ""; // ItemCode -> MaKeToan
                    newRow["SoGhiDauCay"] = ConvertToDecimal(sourceRow["SoGhiDauCay"]);
                    newRow["NW"] = ConvertToDecimal(sourceRow["NW"]);
                    newRow["GW"] = ConvertToDecimal(sourceRow["GW"]);
                    newRow["GhiChu"] = sourceRow["GhiChu"] ?? DBNull.Value;
                    newRow["IsNPL"] = sourceRow["IsNPL"] != DBNull.Value ? sourceRow["IsNPL"] : false;
                    newRow["KhoVaiID"] = sourceRow["KhoVaiID"] ?? DBNull.Value;
                    newRow["SoKienParent"] = sourceRow["SoKienParent"] ?? DBNull.Value;
                    newRow["SoLuongThucTe"] = ConvertToDecimal(sourceRow["SoLuongThucTe"]);
                    newRow["DonGia"] = ConvertToDecimal(sourceRow["DonGia"]);
                    newRow["MaDVVT"] = sourceRow["MaDVVT"] ?? DBNull.Value;
                    newRow["MauVTID"] = sourceRow["MauVTID"] ?? DBNull.Value;
                    newRow["SoKienHienThi"] = sourceRow["SoKienHienThi"] ?? DBNull.Value;
                    newRow["MaDVCD"] = sourceRow["MaDVCD"] ?? DBNull.Value;
                    newRow["TienTe"] = sourceRow["MaTienTe"] ?? DBNull.Value; // MaTienTe -> TienTe
                    newRow["MaVTGhep"] = sourceRow["MaVTGhep"] ?? DBNull.Value;
                    newRow["POMua"] = sourceRow["POMua"] ?? DBNull.Value;
                    newRow["Batch"] = sourceRow["Batch"] ?? DBNull.Value;
                    newRow["SLTong"] = ConvertToDecimal(sourceRow["SLTong"]); // string -> decimal

                   
                    decimal donGia = ConvertToDecimal(sourceRow["DonGia"]);
                    decimal soLuong = ConvertToDecimal(sourceRow["SoLuongThucTe"]);
                    newRow["ThanhTien"] = donGia * soLuong;

                  
                    newRow["ID"] = sourceRow["ID"];
                    newRow["MaHaiQuan"] = DBNull.Value;
                    newRow["BarCode"] = DBNull.Value;
                    newRow["IsNK"] = false;
                    //newRow["STT"] = 0;
                    newRow["NgayNKDuKien"] = DBNull.Value;
                    newRow["TenDVCD"] = DBNull.Value;
                    newRow["tileNW"] = 0m;
                    newRow["tileGW"] = 0m;
                    newRow["Pallet"] = DBNull.Value;
                    newRow["MaNhom"] = sourceRow["MaCLVT"] ?? DBNull.Value;
                    newRow["STTChonVT"] = DBNull.Value;
                    newRow["IsKiemKe"] = false;
                    newRow["NgayKiemKe"] = DBNull.Value;
                    newRow["TuoiTonKho"] = 0m;
                    newRow["STTDot"] = sourceRow["STTDot"] ?? DBNull.Value;
                    newRow["Dot"] = sourceRow["Dot"] ?? DBNull.Value;
                    newRow["POMHID"] = sourceRow["POMHID"] ?? DBNull.Value;
                    newRow["NgayNhapKho"] = DateTime.Now;
                    newRow["SoPhieu"] = DBNull.Value;
                    newRow["NguoiTaoNhapKho"] = GlobleData.UserName;
                    newRow["NgayTaoNhapKho"] = DateTime.Now;
                    newRow["NguoiSuaNhapKho"] = DBNull.Value;
                    newRow["NgaySuaNhapKho"] = DBNull.Value;
                    newRow["MaDVCD"] = sourceRow["MaDVCD"] ?? DBNull.Value;
                    newRow["TenDVCD"] = sourceRow["DVTinhCD"] ?? DBNull.Value;
                    tblSaveShipment.Rows.Add(newRow);
                }

                string url = string.Format("{0}", URL + "SimpleShipment/Post?action=POST");
                string jsonSave = JsonConvert.SerializeObject(tblSaveShipment);
                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, jsonSave);
                }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadVatTuShipment();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal ConvertToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;
            decimal result;
            return decimal.TryParse(value.ToString(), out result) ? result : 0m;
        }
        private void btnRemove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                int idxFocused = grvVatTuShipment.FocusedRowHandle;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvSoKienShipment.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvSoKienShipment.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    if (dr["ID"].ToString() != "0")
                    {
                        //DataTable tbl = new DataTable();
                        //string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={_maPhieuBG}";
                        //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                        //if (json != "[]")
                        //{
                        //    tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        //    if (tbl?.Rows?.Count > 0)
                        //    {
                        //        if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                        //        {
                        //            XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //            return;
                        //        }
                        //    }

                        //}

                        string url = $"{URL}SimpleShipment/Delete?action=Delete&para1={dr["ID"]}&para2=None";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }


                    LoadVatTuShipment();
                    grvVatTuShipment.FocusedRowHandle = idxFocused;
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bttRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadVatTuShipment();
        }

        private void btnThemDot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void grvSoKienShipment_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            DataRow rowVatTuFocused = grvVatTuShipment.GetDataRow(grvVatTuShipment.FocusedRowHandle);
            if (rowVatTuFocused == null) return;

            DataRow rowKienFocused = view.GetFocusedDataRow();
            if (rowKienFocused == null) return;

            if (!lstColumnEdit.Contains(e.Column.FieldName)) return;

        
            var matchedRows = tblVatTuShipment.AsEnumerable()
                .Where(x =>
                    !x.IsNull("MaCLVT") && x["MaCLVT"].Equals(rowVatTuFocused["MaCLVT"]) &&
                    !x.IsNull("MaVTID") && x["MaVTID"].Equals(rowVatTuFocused["MaVTID"]) &&
                    !x.IsNull("MauVTID") && x["MauVTID"].Equals(rowVatTuFocused["MauVTID"]) &&
                    !x.IsNull("KhoVaiID") && x["KhoVaiID"].Equals(rowVatTuFocused["KhoVaiID"])
                    && x["SoKien"].Equals(rowKienFocused["SoKien"])
                     && x["SoLot"].Equals(rowKienFocused["SoLot"]) 
                    && x["Batch"].Equals(rowKienFocused["Batch"]) && !x.IsNull("STTDot") && x["STTDot"].Equals(rowKienFocused["STTDot"])       
                ).ToList();

            if (matchedRows == null || !matchedRows.Any())
            {
                //XtraMessageBox.Show("Không tìm thấy dòng tương ứng!", "Thông báo",
                //    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            foreach (DataRow row in matchedRows)
            {
                switch (e.Column.FieldName)
                {
                    case "SoLuongThucTe":
                        row["SoLuongThucTe"] = ConvertToDecimal(e.Value);
                        row["SoGhiDauCay"] = ConvertToDecimal(e.Value);
                        break;

                    case "SoGhiDauCay":
                        row["SoGhiDauCay"] = ConvertToDecimal(e.Value);
                        row["SoLuongThucTe"] = ConvertToDecimal(e.Value);
                        break;

                    case "GhiChu":
                        row["GhiChu"] = e.Value ?? DBNull.Value;
                        break;

                    default:
                    
                        if (tblVatTuShipment.Columns.Contains(e.Column.FieldName))
                        {
                            row[e.Column.FieldName] = e.Value ?? DBNull.Value;
                        }
                        break;
                }
            }

          
            grvVatTuShipment.RefreshData();
            view.RefreshData();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
               
                //if (string.IsNullOrWhiteSpace(txtLo.Text))
                //{
                //    MessageBox.Show("Vui lòng nhập Số Lô.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    txtLo.Focus();
                //    return;
                //}

                List<string> currentList = new List<string>();
            
                decimal tileNW = 0m, tileGW = 0m;
                string madvcd = string.Empty, tendvcd = string.Empty, batch = string.Empty;
                int sttChonVT = 1;
                //int sttTrongNhom = 1;
                //if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                //{
                //    sttChonVT = Convert.ToInt32(dr["STTChonVT"]);
                //}
                DataRow _dr = grvVatTuShipment.GetFocusedDataRow();
                if (_dr != null)
                {
                    tileNW = decimal.TryParse(_dr["tileNW"].ToString(), out var val) ? val : 0m;
                    tileGW = decimal.TryParse(_dr["tileGW"].ToString(), out var val2) ? val2 : 0m;
                    madvcd = _dr["MaDVCD"].ToString();
                    tendvcd = _dr["TenDVCD"].ToString();
                    batch = _dr["Batch"].ToString();
                }

                frmERPPOMH_AddKienShipmnet frm = new frmERPPOMH_AddKienShipmnet(_dr, tblVatTuShipment, _rowFocused["TenPhieu"]?.ToString(), true, tileNW, tileGW, madvcd, tendvcd, _phieuMH, batch);
                frm.StartPosition = FormStartPosition.CenterScreen;

                frm.ThongSoKienChanged += (list) =>
                {
                    //tblNhapKho = frmERPNhapKhoNPL_KhaiBao.dt;
                    //string maVTID = dr["MaVTID"].ToString();
                    //string maMauVT = dr["MauVTID"].ToString();
                    //string khoVaiID = dr["KhoVaiID"].ToString();
                    //string MaNhom = dr["MaNhom"].ToString();
                    //string MaVTGhep = dr["MaVTGhep"].ToString();
                    //var filteredRows = tblNhapKho.AsEnumerable()
                    //    .Where(row => row["MaVTID"].ToString() == maVTID &&
                    //                  row["MauVTID"].ToString() == maMauVT &&
                    //                  row["KhoVaiID"].ToString() == khoVaiID &&
                    //                  row["MaNhom"].ToString() == MaNhom &&
                    //                  row["MaVTGhep"].ToString() == MaVTGhep);

                    //foreach (var row in filteredRows)
                    //{
                    //    row["STT"] = sttChonVT;
                    //    row["sttChonVT"] = sttChonVT;
                    //}
                    //if (filteredRows.Any())
                    //{
                    //    // Chuyển đổi trực tiếp sang DataTable mới
                    //    DataTable tblNhapKhoFiltered = filteredRows.CopyToDataTable();
                    //    gCNLNhapKho.DataSource = tblNhapKhoFiltered;
                    //}
                    //else
                    //{

                    //    gCNLNhapKho.DataSource = tblNhapKho.Clone();
                    //}
                };
                frm.ShowDialog();



            }
            catch (Exception ex)
            {

            }
        }

        /*Export Mẫu*/

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Lưu file Excel",
                Filter = "Excel 2010 (*.xlsx)|*.xlsx|Excel 2003 (*.xls)|*.xls",
                FileName = string.Format("Mau-nhap-simple-shipment-{0}", DateTime.Now.ToString("ddMMyyyyHHss"))
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
                if (tblVatTuShipment == null || tblVatTuShipment.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                string templatePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Templates",
                    "TemplateShipment.xlsx");

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
                    tblMMTB_ImportCopy = SetTblVatTuShipment();

                    foreach (DataRow dr in tblMMTB_ImportCopy.Rows)
                    {


                        ws.Cells[row, 1].Value = stt++;
                        ws.Cells[row, 2].Value = SafeStr(dr, "ItemCode");
                        ws.Cells[row, 3].Value = SafeStr(dr, "ChungLoaiVatTu");
                        ws.Cells[row, 4].Value = SafeStr(dr, "MoTa");
                        ws.Cells[row, 5].Value = SafeStr(dr, "MauVT");
                        ws.Cells[row, 6].Value = SafeStr(dr, "KhoVai");
                        ws.Cells[row, 7].Value = SafeStr(dr, "SoKienHienThi");
                        ws.Cells[row, 8].Value ="";
                        ws.Cells[row, 9].Value = "";
                        ws.Cells[row, 10].Value = "";
                        ws.Cells[row, 11].Value = SafeStr(dr, "DVTT");
                        ws.Cells[row, 12].Value = "";


                        ws.Cells[row, 10].Style.Numberformat.Format = "#,##0.##";
                      


                        ws.Cells[row, 1, row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, 1, row, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
               


                        thinBorder(ws.Cells[row, 1, row, 12]);


                        row++;
                    }


                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

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
            DateTime d = NtbSoft.ERP.Win.Utils.clsForrmatUtils.ConvertDate(dr[col].ToString());
            return d != DateTime.MinValue ? d : (DateTime?)null;
        }
        /*Import Đợt mới*/
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadMaxDot();
            frmImportExcelShipment frm = new frmImportExcelShipment(MaxSTTDot,false,_phieuMH);
            frm.ShowDialog();
            if (!string.IsNullOrEmpty(frm.Path) && !string.IsNullOrEmpty(frm.SheetName))
            {
                ReadFileExcel(frm.Path, frm.SheetName, MaxSTTDot);
            }
            else
            {
                XtraMessageBox.Show("Bạn chưa chọn file hoặc sheet!");
            }
        }
        /*Import*/
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmImportExcelShipment frm = new frmImportExcelShipment(MaxSTTDot, true, _phieuMH);
            frm.ShowDialog();
            if (!string.IsNullOrEmpty(frm.Path) && !string.IsNullOrEmpty(frm.SheetName))
            {
                ReadFileExcel(frm.Path, frm.SheetName, frm.sttDot);
            }
            else
            {
                XtraMessageBox.Show("Bạn chưa chọn file hoặc sheet!");
            }
        }


        private void ReadFileExcel(string Path, string SheetName, int sttDot)
        {
            if (sttDot == 0) sttDot = 1;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            int rowStart = 5; 
            try
            {
                using (var package = new ExcelPackage(new FileInfo(Path)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == SheetName);
                    if (worksheet == null)
                    {
                        XtraMessageBox.Show("Không tìm thấy sheet: " + SheetName, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < rowStart)
                    {
                        XtraMessageBox.Show("File Excel không có dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                
                    string soLoID = _phieuMH;

                  

                    for (int row = rowStart; row <= rowCount; row++)
                    {
                        // Đọc các cột từ Excel theo thứ tự cột ảnh mẫu:
                        // A=STT, B=ItemCode, C=ChủngLoại, D=MôTả, E=MàuVT, F=With/Size, G=SốRoll, H=Lot, I=Bath, J=SốLượng, K=ĐơnVị, L=GhiChú
                        string itemCode = worksheet.Cells[row, 2].Text?.Trim();
                        string chungLoai = worksheet.Cells[row, 3].Text?.Trim();
                        string moTa = worksheet.Cells[row, 4].Text?.Trim();
                        string mauVT = worksheet.Cells[row, 5].Text?.Trim();
                        string withSize = worksheet.Cells[row, 6].Text?.Trim();
                        string soRoll = worksheet.Cells[row, 7].Text?.Trim();
                        string lot = worksheet.Cells[row, 8].Text?.Trim();
                        string batch = worksheet.Cells[row, 9].Text?.Trim();
                        string soLuongStr = worksheet.Cells[row, 10].Text?.Trim();
                        string donVi = worksheet.Cells[row, 11].Text?.Trim()?.ToUpper();
                        string ghiChu = worksheet.Cells[row, 12].Text?.Trim();

                        if (string.IsNullOrWhiteSpace(itemCode) && string.IsNullOrWhiteSpace(moTa))
                            continue;

                        string MaDonVi = string.Empty;
                        if(tblDonVi != null && tblDonVi?.Rows?.Count > 0)
                        {
                          var queryDonVi =   tblDonVi.AsEnumerable().FirstOrDefault(x => x["TenDVVT"]?.ToString()?.ToUpper() == donVi);
                            if(queryDonVi!= null)
                            {
                                MaDonVi = queryDonVi["MaDVVT"]?.ToString();
                            }
                        }


                        DataRow[] matchedRows = tblVatTuShipment.Select(
                            string.Format("MoTa='{0}' AND ChungLoaiVatTu='{1}' AND MauVT='{2}' AND KhoVai='{3}'AND SoLoID='{4}'AND SoKien='{5}'AND SoLot='{6}'AND Batch='{7}' AND STTDot = '{8}'",
                                EscapeStr(moTa),
                                EscapeStr(chungLoai),
                                EscapeStr(mauVT),
                                EscapeStr(withSize),
                                _phieuMH,
                                soRoll,
                                lot,batch, sttDot


                            )
                        );

                        // Chuyển đổi đơn vị
                        decimal soLuong = ConvertToDecimal(soLuongStr);
                        decimal soLuongThucTe = soLuong;
                        string soKien = soRoll;

                        if (string.IsNullOrEmpty(soRoll))
                        {
                            XtraMessageBox.Show($"Item Code {itemCode} Chưa nhập số kiện", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (matchedRows.Length > 0)
                        {

                            DataRow sourceRow = matchedRows[0];
                            if (sourceRow["TrangThai"] == "Chưa kiểm")
                            {
                                sourceRow["SoGhiDauCay"] = soLuongThucTe;
                                sourceRow["SoLuongThucTe"] = soLuongThucTe;
                                sourceRow["MaDVCD"] = MaDonVi;
                                sourceRow["GhiChu"] = ghiChu ?? string.Empty;
                            }






                        }
                        else
                        {
                            DataRow[] matchedRowsVatTu = tblVatTuShipment.Select(
                           string.Format("MoTa='{0}' AND ChungLoaiVatTu='{1}' AND MauVT='{2}' AND KhoVai='{3}'AND SoLoID='{4}'",
                               EscapeStr(moTa),
                               EscapeStr(chungLoai),
                               EscapeStr(mauVT),
                               EscapeStr(withSize),
                               _phieuMH

                           )
                       );
                            if (matchedRowsVatTu.Length > 0)
                            {
                                DataRow sourceRowVatTu = matchedRowsVatTu[0];

                                DataRow newRow = tblVatTuShipment.NewRow();
                                newRow["MaCLVT"] = sourceRowVatTu["MaCLVT"];
                                newRow["MaVTID"] = sourceRowVatTu["MaVTID"];
                                newRow["MauVTID"] = sourceRowVatTu["MauVTID"];
                                newRow["KhoVaiID"] = sourceRowVatTu["KhoVaiID"];
                                newRow["STTDot"] = sttDot;
                                newRow["Dot"] = sttDot;
                                newRow["POMHID"] = sourceRowVatTu["POMHID"];
                                newRow["DonGia"] = sourceRowVatTu["DonGia"];
                                newRow["MaDVVT"] = sourceRowVatTu["MaDVVT"];
                                newRow["MaTienTe"] = sourceRowVatTu["MaTienTe"];
                                newRow["MaDVCD"] = MaDonVi;
                                newRow["DVTinhCD"] = donVi;
                                newRow["ItemCode"] = itemCode ?? string.Empty;
                                newRow["ChungLoaiVatTu"] = chungLoai ?? string.Empty;
                                newRow["MoTa"] = moTa ?? string.Empty;
                                newRow["MauVT"] = mauVT ?? string.Empty;
                            
                                newRow["SoKien"] = soKien.ToString();
                                newRow["SoKienParent"] = soKien.ToString();
                                newRow["SoKienHienThi"] = soKien.ToString();
                                newRow["SoLot"] = lot ?? string.Empty;
                                newRow["Batch"] = batch ?? string.Empty;
                                newRow["SoLuongThucTe"] = soLuongThucTe;
                                newRow["MaDVVT"] = sourceRowVatTu["MaDVVT"];
                                newRow["GhiChu"] = ghiChu ?? string.Empty;
                                newRow["SoLoID"] = soLoID;
                                newRow["IsNPL"] = false;
                                newRow["IsUse"] = true;
                                newRow["NW"] = 0m;
                                newRow["GW"] = 0m;
                                newRow["DonGia"] = 0m;
                                newRow["SoGhiDauCay"] = soLuongThucTe;
                                newRow["POMua"] = sourceRowVatTu["POMua"];
                                newRow["MaNPL"] = sourceRowVatTu["MaNPL"];
                                newRow["MaVTGhep"] = sourceRowVatTu["MaVTGhep"];
                                tblVatTuShipment.Rows.Add(newRow);
                            }
                            else
                            {
                                XtraMessageBox.Show($"Item Code không khớp vật tư phiếu mua {itemCode}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                }

              
                SaveShipment();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string EscapeStr(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Replace("'", "''");
        }


        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            string fieldName = view.FocusedColumn?.FieldName;
            int rowHandle = view.FocusedRowHandle;


            if (fieldName == "SoGhiDauCay")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng theo chứng từ không được để trống!";
                    return;
                }
                if (!decimal.TryParse(e.Value.ToString(), out _))
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng theo chứng từ không hợp lệ!";
                    return;
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
            ApplyVatTu(keyword);
        }
       
        
        private void ApplyVatTu(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                grvVatTuShipment.ActiveFilter.Clear();
                return;
            }

            grvVatTuShipment.ActiveFilterString = $@"
            [ChungLoaiVatTu] LIKE '%{keyword}%'
            OR [MoTa] LIKE '%{keyword}%'
            OR [ItemCode] LIKE '%{keyword}%'
            OR [KhoVai] LIKE '%{keyword}%'
            OR [MauVT] LIKE '%{keyword}%'
          
        ";

            if (grvVatTuShipment.RowCount > 0)
            {
                DataRow rowVatTuShipment = grvVatTuShipment.GetDataRow(0);
                LoadKienVatTuShipment(rowVatTuShipment);
            }
            else
            {
                grcSoKienShipment.DataSource = new DataTable();
            }
        }


        private void grvSoKienShipment_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvSoKienShipment_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;
                DataRow rowFocused = view.GetFocusedDataRow();
                if (rowFocused == null) return;

                bool.TryParse(rowFocused["IsUse"]?.ToString(), out bool IsUse);               
                if (IsUse)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    return;
                }
                else if (lstColumnEdit.Contains(view.FocusedColumn.FieldName))
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

        private void grvSoKienShipment_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if(grvSoKienShipment.FocusedColumn == colviewQC)
            {
                GridView view = sender as GridView;
                if (view == null) return;

                DataRow rowVatTuFocused = grvVatTuShipment.GetDataRow(grvVatTuShipment.FocusedRowHandle);
                if (rowVatTuFocused == null) return;

                DataRow rowKienFocused = view.GetFocusedDataRow();
                if (rowKienFocused == null) return;

                bool isNPL = rowVatTuFocused["LoaiNPL"]?.ToString() == "Nguyên Liệu";
                frmERP_POMH_QCShipment frm = new frmERP_POMH_QCShipment(isNPL, rowVatTuFocused, rowKienFocused);
                frm.ShowDialog();

            }
        }
    }
}