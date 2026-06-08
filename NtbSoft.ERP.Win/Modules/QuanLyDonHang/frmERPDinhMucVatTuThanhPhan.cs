using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmERPDinhMucVatTuThanhPhan : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        DataRow _rowFocused;
        private List<string> lstColumnEdit = new List<string>() { "SoLuong", "HaoHut", "GiaCat", "GiaGiaCong", "MaTienTe", "GhiChu" };
        private List<string> lstColumnFormat = new List<string>() { "SoLuong", "HaoHut", "GiaCat", "GiaGiaCong", "ThanhTien", "TieuHao", "TonKho" };
        private List<string> lstValiDate = new List<string>() { "SoLuong", "HaoHut", "GiaCat", "GiaGiaCong" };
        string MaGop = string.Empty;
        string MaLenhSanXuat = string.Empty;
        string MaLenh = string.Empty;
        DataTable tblDot = new DataTable();
        int MaxSTTDot = 1;
        string TienTeDeault = "VND";
        bool IsDuyet = false;
        bool IsXacNhan = false;


        DataSet dsDinhMucVatTuTP = new DataSet();
        DataTable _tblQuyDoiTT = new DataTable();

        public frmERPDinhMucVatTuThanhPhan(DataRow rowDHFocused)
        {
            InitializeComponent();

            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _rowFocused = rowDHFocused;
            if (rowDHFocused != null)
            {
                this.Text = $"{rowDHFocused["TenKH"]} - {rowDHFocused["MaDH"]} - {rowDHFocused["TenHang"]}";
                txtMaHang.EditValue = rowDHFocused["TenHang"];
            }
            CheckPerminsion();
            LoadQuyDoi();
            FormatTextEditSoTien(txtTongTien.Properties);
            CreateDSDinhMucVatTuTP();
            InitMasterDetail();
            LoadLenhSX();
            SetUpColDonViTienTe();



        }
        private void CheckPerminsion()
        {
            if (GlobleData.UserName?.ToLower() == "admin" || GlobleData.UserName == "QLDH_01")
            {
                btnDuyetVT.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                btnHuyDuyetVT.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                btnDuyetVT.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnHuyDuyetVT.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        private void LoadQuyDoi()
        {

            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=GetQuyDoiTT";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblQuyDoiTT = JsonConvert.DeserializeObject<DataTable>(json);
                if (_tblQuyDoiTT != null && _tblQuyDoiTT?.Rows?.Count > 0)
                {
                    var Query = _tblQuyDoiTT.AsEnumerable().FirstOrDefault(x => x["MaTienTe"]?.ToString()?.ToUpper() == "VND");
                    if (Query != null)
                    {
                        TienTeDeault = Query["TienTeID"]?.ToString();

                    }
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }
        private void CreateDSDinhMucVatTuTP()
        {
            dsDinhMucVatTuTP = new DataSet();
            DataTable dtMaster = new DataTable("DinhMucVatTuTP");
            dtMaster.Columns.AddRange(new DataColumn[]
            {
             new DataColumn("ID", typeof(int)),
            new DataColumn("ID_DMThanhPhan_Parent", typeof(int)),
            new DataColumn("Dot",                   typeof(string)),
            new DataColumn("MaHang",                typeof(string)),
            new DataColumn("MaDH",                  typeof(string)),
            new DataColumn("MaLenh",                typeof(string)),
            new DataColumn("MaLenhSX",              typeof(string)),
            new DataColumn("MaGop",                 typeof(string)),
            new DataColumn("MaCLVTID",              typeof(string)),
            new DataColumn("MaVTID",                typeof(string)),
            new DataColumn("MauVTID",               typeof(string)),
            new DataColumn("KhoVaiID",              typeof(string)),
            new DataColumn("MaDVT",                 typeof(string)),
            new DataColumn("DinhMucTP",             typeof(decimal)),
            new DataColumn("TenCL",                 typeof(string)),
            new DataColumn("MoTa",                  typeof(string)),
            new DataColumn("ItemCode",              typeof(string)),
            new DataColumn("KhoVai",                typeof(string)),
            new DataColumn("MauVT",                 typeof(string)),
            new DataColumn("CodeMau",               typeof(string)),
            new DataColumn("TenDVVT",               typeof(string)),
            new DataColumn("LoaiNPL",               typeof(string)),
            new DataColumn("IsNPL",                 typeof(bool)),
            new DataColumn("SoLuong",               typeof(decimal)),
            new DataColumn("TieuHao",               typeof(decimal)),
            new DataColumn("GiaCat",                typeof(decimal)),
            new DataColumn("GiaGiaCong",            typeof(decimal)),
            new DataColumn("IsDuyet",               typeof(bool)),
            new DataColumn("IsXacNhan",             typeof(bool)),
            new DataColumn("GhiChu",                typeof(string)),
            new DataColumn("MaTienTe",              typeof(string)),
            new DataColumn("TienTeQD",              typeof(string)),
            new DataColumn("HaoHut",                typeof(decimal)),
            new DataColumn("NhuCau",                typeof(decimal)),
            new DataColumn("TonKho",                typeof(decimal)),

                });

            dtMaster.PrimaryKey = new DataColumn[] { dtMaster.Columns["ID_DMThanhPhan_Parent"] };


            DataTable dtDetail = new DataTable("DinhMucVatTuTP_Detail");
            dtDetail.Columns.AddRange(new DataColumn[]
            {

                new DataColumn("ID_DMThanhPhan_Parent", typeof(int)),   // FK → Master
                new DataColumn("Dot",                   typeof(string)),
                new DataColumn("MaHang",                typeof(string)),
                new DataColumn("MaDH",                  typeof(string)),
                new DataColumn("MaLenh",                typeof(string)),
                new DataColumn("MaLenhSX",              typeof(string)),
                new DataColumn("MaGop",                 typeof(string)),
                new DataColumn("MaCLVTID",              typeof(string)),
                new DataColumn("MaVTID",                typeof(string)),
                new DataColumn("MauVTID",               typeof(string)),
                new DataColumn("KhoVaiID",              typeof(string)),
                new DataColumn("MaDVT",                 typeof(string)),
                new DataColumn("DinhMucTP",             typeof(decimal)),
                new DataColumn("TenCL",                 typeof(string)),
                new DataColumn("MoTa",                  typeof(string)),
                new DataColumn("ItemCode",              typeof(string)),
                new DataColumn("KhoVai",                typeof(string)),
                new DataColumn("MauVT",                 typeof(string)),
                new DataColumn("CodeMau",               typeof(string)),
                new DataColumn("TenDVVT",               typeof(string)),
                new DataColumn("LoaiNPL",               typeof(string)),
                new DataColumn("IsNPL",                 typeof(bool)),
                new DataColumn("SoLuong",               typeof(decimal)),
                new DataColumn("TieuHao",               typeof(decimal)),
                new DataColumn("GiaCat",                typeof(decimal)),
                new DataColumn("GiaGiaCong",            typeof(decimal)),
                new DataColumn("MaTienTe",              typeof(string)),
                new DataColumn("GhiChu",                typeof(string)),
                new DataColumn("HaoHut",                typeof(decimal)),
                new DataColumn("NhuCau",                typeof(decimal)),
                new DataColumn("TonKho",                typeof(decimal)),
                    });


            dsDinhMucVatTuTP.Tables.Add(dtMaster);
            dsDinhMucVatTuTP.Tables.Add(dtDetail);


            dsDinhMucVatTuTP.Relations.Add(new DataRelation(
                "Thành phần",
                dsDinhMucVatTuTP.Tables["DinhMucVatTuTP"].Columns["ID_DMThanhPhan_Parent"],
                dsDinhMucVatTuTP.Tables["DinhMucVatTuTP_Detail"].Columns["ID_DMThanhPhan_Parent"]
            ));
        }

        private DataTable CreateTableVatTuTP_Save()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("ID_DMThanhPhan_Parent", typeof(long));
            dt.Columns.Add("MaLenhSX", typeof(string));
            dt.Columns.Add("MaGop", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("MaLenh", typeof(string));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaCLVTID", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaDVT", typeof(string));
            dt.Columns.Add("SoLuong", typeof(double));
            dt.Columns.Add("DMVatTuThanhPhan", typeof(double));
            dt.Columns.Add("TieuHao", typeof(double));
            dt.Columns.Add("GiaCat", typeof(double));
            dt.Columns.Add("GiaGiaCong", typeof(double));
            dt.Columns.Add("MaTienTe", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            dt.Columns.Add("Creater", typeof(string));
            dt.Columns.Add("IsDuyet", typeof(bool));
            dt.Columns.Add("IsXacNhan", typeof(bool));
            dt.Columns.Add("IsParent", typeof(int));
            dt.Columns.Add("TienTeQD", typeof(string));
            dt.Columns.Add("HaoHut", typeof(decimal));
            dt.Columns.Add("NhuCau", typeof(decimal));
            dt.Columns.Add("TonKho", typeof(decimal));
            return dt;
        }

        private void SetUpColDonViTienTe()
        {
            DataTable tbl = new DataTable();
            string url = string.Format("{0}?action={1}", URL + "PhieuBaoGia/Get", "GetTienTe");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }
            RepositoryItemSearchLookUpEdit rItemEdit = new RepositoryItemSearchLookUpEdit();
            rItemEdit.DataSource = tbl;
            rItemEdit.DisplayMember = "MaTienTe";
            rItemEdit.ValueMember = "MaTienTe";
            rItemEdit.ShowClearButton = false;
            rItemEdit.NullText = "[Chọn tiền tệ]";
            rItemEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rItemEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rItemEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TienTeID", Caption = "TienTeID", Name = "rTienTeID", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "MaTienTe", Caption = "Mã tiền tệ", Name = "rMaTienTe", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenTienTe", Caption = "Tiền tệ", Name = "rTenTienTe", Visible = true });
            }
            colTienTe.ColumnEdit = rItemEdit;
            colTienTe_Detail.ColumnEdit = rItemEdit;


            SearchLookupDVTienTe.Properties.DataSource = tbl;
            SearchLookupDVTienTe.Properties.ValueMember = "MaTienTe";
            SearchLookupDVTienTe.Properties.DisplayMember = "MaTienTe";
        }
        private decimal QuyDoiDonGia(string TienTeQuyDoi, decimal DonGia)
        {
            decimal DonGiaQD = 0;
            decimal result = 0;
            decimal DonGiaTpm = 0;
            if (_tblQuyDoiTT != null && _tblQuyDoiTT?.Rows?.Count > 0)
            {
                var QueryTienTeGoc = _tblQuyDoiTT.AsEnumerable().FirstOrDefault(x => x["MaTienTe"]?.ToString() == TienTeQuyDoi);
                if (QueryTienTeGoc != null)
                {
                    decimal.TryParse(QueryTienTeGoc["Gia"]?.ToString(), out DonGiaTpm);
                }

                var Query = _tblQuyDoiTT.AsEnumerable().FirstOrDefault(x => x["MaTienTe"]?.ToString() == SearchLookupDVTienTe.EditValue?.ToString());
                if (Query != null)
                {
                    decimal.TryParse(Query["Gia"]?.ToString(), out DonGiaQD);
                }
                if (DonGiaQD == 0) return 0;
                decimal result_tmp = DonGia * DonGiaTpm;
                result = result_tmp / DonGiaQD;
            }
            return Math.Round(result, 2);
        }
        private void SumThanhTienVT()
        {
            decimal tongChiPhi = 0;
            DataSet dsVatTuTP = grcVattuTP.DataSource as DataSet;
            if (dsVatTuTP != null && dsVatTuTP.Tables["DinhMucVatTuTP_Detail"]?.Rows?.Count > 0)
            {
                foreach (DataRow rowVatTu_detail in dsVatTuTP.Tables["DinhMucVatTuTP_Detail"]?.Rows)
                {
                    decimal GiaCat = 0;
                    decimal GiaGiaCong = 0;
                    decimal SoLuong = 0;
                    decimal DinhMuc = 0;

                    object valGiaCat = rowVatTu_detail["GiaCat"];
                    object valGiaGiaCong = rowVatTu_detail["GiaGiaCong"];
                    object valSoLuong = rowVatTu_detail["TieuHao"];
                    object valDinhMuc = rowVatTu_detail["DinhMucTP"];

                    if (valGiaCat != null && valGiaCat != DBNull.Value)
                        decimal.TryParse(valGiaCat.ToString(), out GiaCat);

                    if (valGiaGiaCong != null && valGiaGiaCong != DBNull.Value)
                        decimal.TryParse(valGiaGiaCong.ToString(), out GiaGiaCong);

                    if (valSoLuong != null && valSoLuong != DBNull.Value)
                        decimal.TryParse(valSoLuong.ToString(), out SoLuong);

                    if (valDinhMuc != null && valDinhMuc != DBNull.Value)
                        decimal.TryParse(valDinhMuc.ToString(), out DinhMuc);
                    tongChiPhi += QuyDoiDonGia(rowVatTu_detail["MaTienTe"]?.ToString() ?? TienTeDeault, ((SoLuong * DinhMuc) * (GiaCat + GiaGiaCong)));
                }

                txtTongTien.EditValue = tongChiPhi;
            }

        }
        private void FormatTextEditSoTien(params RepositoryItemTextEdit[] edits)
        {

            foreach (var edit in edits)
            {
                edit.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                edit.Appearance.Options.UseTextOptions = true;

                edit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                edit.Mask.EditMask = "n2";
                edit.Mask.UseMaskAsDisplayFormat = true;


                edit.Mask.Culture = System.Globalization.CultureInfo.InvariantCulture;
                // hoặc: edit.Mask.Culture = new CultureInfo("en-US");
            }
        }
        private void InitMasterDetail()
        {

            GridView masterView = grcVattuTP.MainView as GridView;
            masterView.OptionsDetail.EnableMasterViewMode = true;
            masterView.OptionsDetail.ShowDetailTabs = true;
            masterView.OptionsDetail.SmartDetailHeight = true;


            grvVattuTP_Detail.OptionsBehavior.AutoPopulateColumns = true;


            if (grcVattuTP.LevelTree.Nodes.Count > 0)
            {
                grcVattuTP.LevelTree.Nodes[0].RelationName = "Thành phần";
                grcVattuTP.LevelTree.Nodes[0].LevelTemplate = grvVattuTP_Detail;
            }
            else
            {

                GridLevelNode detailNode = new GridLevelNode();
                detailNode.RelationName = "Thành phần";
                detailNode.LevelTemplate = grvVattuTP_Detail;
                grcVattuTP.LevelTree.Nodes.Add(detailNode);
            }


            grcVattuTP.DataSource = dsDinhMucVatTuTP;
            grcVattuTP.DataMember = "DinhMucVatTuTP";
        }

        private void LoadLenhSX()
        {
            try
            {
                string url = $"{URL}ERPVatTuThanhPhan/GET?action=GetLenhSX&para1={_rowFocused["MaDH"]}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblLSX = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpMaLenh.Properties.DataSource = tblLSX;
                searchLookUpMaLenh.Properties.ValueMember = "MaLenh";
                searchLookUpMaLenh.Properties.DisplayMember = "MaLenh";

                searchLookUpMaLenh.EditValue = null;

                if (tblLSX != null && tblLSX?.Rows?.Count == 1)
                {
                    searchLookUpMaLenh.EditValue = tblLSX?.Rows[0]["MaLenh"];
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }


        private void LoadDot()
        {
            string urlNCC = $"{URL}ERPVatTuThanhPhan/Get?action=GetDot&para1={searchLookUpMaLenh.EditValue}";
            string jsonNCC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNCC); }).Result;
            if (jsonNCC == "[]")
            {
                MaxSTTDot = 1;

                tblDot = new DataTable();
                tblDot.Columns.Add("Dot", typeof(string));
                tblDot.Columns.Add("STTDot", typeof(string));


                DataRow rowDot = tblDot.NewRow();
                rowDot["Dot"] = MaxSTTDot;
                rowDot["STTDot"] = MaxSTTDot;
                tblDot.Rows.Add(rowDot);
            }
            else
            {
                tblDot = JsonConvert.DeserializeObject<DataTable>(jsonNCC);

            }
            searchLookUpDot.Properties.ValueMember = "STTDot";
            searchLookUpDot.Properties.DisplayMember = "Dot";
            searchLookUpDot.Properties.DataSource = tblDot;

            int MaxDot = tblDot.AsEnumerable()
            .Select(x => x["STTDot"] != DBNull.Value && int.TryParse(x["STTDot"].ToString(), out int v) ? v : 0)
            .DefaultIfEmpty(0)
            .Max();

            searchLookUpDot.EditValue = MaxDot;
        }
        private void searchLookUpMaLenh_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit searchLookUp = sender as SearchLookUpEdit;

            var keyValue = searchLookUp.EditValue;
            if (keyValue == null || keyValue == DBNull.Value) return;


            DataTable dt = searchLookUp.Properties.DataSource as DataTable;
            if (dt == null) return;


            string valueMember = searchLookUp.Properties.ValueMember;
            DataRow[] rows = dt.Select($"{valueMember} = '{keyValue}'");
            if (rows.Length == 0) return;

            DataRow row = rows[0];
            MaGop = row["MaGop"]?.ToString();
            MaLenhSanXuat = row["MaLenhSanXuat"]?.ToString();
            MaLenh = row["MaLenh"]?.ToString();
            txtSLKH.EditValue = row["SoLuong"]?.ToString();
            LoadDot();
        }
        private void searchLookUpDot_EditValueChanged(object sender, EventArgs e)
        {
            LoadThanhPhanVatTu();
        }
        private void LoadThanhPhanVatTu()
        {
            try
            {
                string url = $"{URL}ERPVatTuThanhPhan/GETDS?action=GetDinhMucThanhPhanVT&para1={MaLenh}&para2={searchLookUpDot.EditValue}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                CreateDSDinhMucVatTuTP();
                InitMasterDetail();

                if (json != "[]" && !string.IsNullOrEmpty(json))
                {
                    DataSet dsVatTu = JsonConvert.DeserializeObject<DataSet>(json);

                    if (dsVatTu != null && dsVatTu.Tables.Count >= 2)
                    {

                        DataTable dtMaster = dsDinhMucVatTuTP.Tables["DinhMucVatTuTP"];
                        DataTable dtDetail = dsDinhMucVatTuTP.Tables["DinhMucVatTuTP_Detail"];

                        dtMaster.Clear();
                        dtDetail.Clear();


                        foreach (DataRow row in dsVatTu.Tables[0].Rows)
                        {
                            DataRow newRow = dtMaster.NewRow();
                            foreach (DataColumn col in dtMaster.Columns)
                            {
                                if (dsVatTu.Tables[0].Columns.Contains(col.ColumnName))
                                    newRow[col.ColumnName] = row[col.ColumnName];
                            }
                            dtMaster.Rows.Add(newRow);
                        }

                        foreach (DataRow row in dsVatTu.Tables[1].Rows)
                        {
                            DataRow newRow = dtDetail.NewRow();
                            foreach (DataColumn col in dtDetail.Columns)
                            {
                                if (dsVatTu.Tables[1].Columns.Contains(col.ColumnName))
                                    newRow[col.ColumnName] = row[col.ColumnName];
                            }
                            dtDetail.Rows.Add(newRow);
                        }

                        if (dsVatTu.Tables[0] != null && dsVatTu.Tables[0]?.Rows?.Count > 0)
                        {
                            SearchLookupDVTienTe.EditValue = dsVatTu.Tables[0].Rows[0]["TienTeQD"] ?? TienTeDeault;
                            bool.TryParse(dsVatTu.Tables[0].Rows[0]["IsDuyet"]?.ToString(), out IsDuyet);
                            bool.TryParse(dsVatTu.Tables[0].Rows[0]["IsXacNhan"]?.ToString(), out IsXacNhan);
                        }
                        else
                        {
                            IsDuyet = false;
                            IsXacNhan = false;
                            SearchLookupDVTienTe.EditValue = TienTeDeault;
                        }

                    }
                }

                grcVattuTP.DataSource = null;
                grcVattuTP.DataMember = string.Empty;
                grcVattuTP.DataSource = dsDinhMucVatTuTP;
                grcVattuTP.DataMember = "DinhMucVatTuTP";
                grcVattuTP.RefreshDataSource();
                SumThanhTienVT();

                if (IsDuyet || IsXacNhan)
                {
                    btnChooseVatTu.Enabled = false;
                    btnLuuVT.Enabled = false;
                    btnDelete.Enabled = false;
                }
                else
                {
                    btnChooseVatTu.Enabled = true;
                    btnLuuVT.Enabled = true;
                    btnDelete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi LoadThanhPhanVatTu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private void GrvRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            bool isEditable = lstColumnEdit.Contains(e.Column.FieldName);

            if (isEditable)
            {
                e.Appearance.BackColor = Color.FromArgb(192, 255, 255);



            }
            else if (e.Column.FieldName == "DinhMucTP")
            {
                e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            }

        }
        private void grv_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null || e.Value == DBNull.Value || e.Value.ToString() == "")
                {
                    if (lstValiDate.Contains(e.Column.FieldName)) e.DisplayText = "-";
                    return;
                }

                var view = sender as GridView;

                if (lstColumnFormat.Contains(e.Column.FieldName))
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




            }
            catch (Exception ex)
            {

            }
        }
        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }
        bool indicatorIcon = true;
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize + 20;
                    }

                    e.Info.DisplayText = "*";
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion
        /**/
        private void btnChooseVatTu_Click(object sender, EventArgs e)
        {
            DataSet dsVatTuDM = grcVattuTP.DataSource as DataSet;
            if (dsVatTuDM == null)
            {
                CreateDSDinhMucVatTuTP();
                dsVatTuDM = dsDinhMucVatTuTP;
            }
            frmSelecteDinhMucVatTuTP frm = new frmSelecteDinhMucVatTuTP(dsVatTuDM.Tables["DinhMucVatTuTP"], MaGop, MaLenhSanXuat);
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                foreach (DataRow rowSelected in frm.tblVatTuSelected.Rows)
                {
                    // Kiểm tra trùng ID nếu cần
                    bool isDuplicate = dsVatTuDM.Tables["DinhMucVatTuTP"].AsEnumerable()
                        .Any(r => r["MaVTID"]?.ToString() == rowSelected["MaVTID"]?.ToString()
                               && r["MaCLVTID"]?.ToString() == rowSelected["MaCLVTID"]?.ToString()
                               && r["MaVTID"]?.ToString() == rowSelected["MaVTID"]?.ToString()
                               && r["KhoVaiID"]?.ToString() == rowSelected["KhoVaiID"]?.ToString()

                               );

                    if (isDuplicate) continue; // bỏ qua nếu đã tồn tại

                    DataRow newRow = dsVatTuDM.Tables["DinhMucVatTuTP"].NewRow();


                    decimal.TryParse(rowSelected["TongCong"]?.ToString(), out decimal TongNhuCau);

                    decimal.TryParse(rowSelected["SLTonKho"]?.ToString(), out decimal TonKho);

                    newRow["ID_DMThanhPhan_Parent"] = rowSelected["ID_VatTuThanhPhan"];
                    newRow["MaCLVTID"] = rowSelected["MaNhomVT"];
                    newRow["MaVTID"] = rowSelected["MaVTID"];
                    newRow["MauVTID"] = rowSelected["MaMauVT"];
                    newRow["KhoVaiID"] = rowSelected["MaKhoVT"];
                    newRow["MaDVT"] = rowSelected["MaDVVT"] != DBNull.Value ? rowSelected["MaDVVT"] : (object)DBNull.Value;
                    newRow["DinhMucTP"] = rowSelected["DM_VatTuTP"];
                    newRow["GhiChu"] = "";
                    newRow["TenCL"] = rowSelected["TenNhom"] != DBNull.Value ? rowSelected["TenNhom"] : (object)DBNull.Value;
                    newRow["MoTa"] = rowSelected["TenVT"] != DBNull.Value ? rowSelected["TenVT"] : (object)DBNull.Value;
                    newRow["ItemCode"] = rowSelected["MaVT"] != DBNull.Value ? rowSelected["MaVT"] : (object)DBNull.Value;
                    newRow["KhoVai"] = rowSelected["KhoVai"] != DBNull.Value ? rowSelected["KhoVai"] : (object)DBNull.Value;
                    newRow["MauVT"] = rowSelected["Mau"] != DBNull.Value ? rowSelected["Mau"] : (object)DBNull.Value;
                    newRow["CodeMau"] = rowSelected["CodeMau"] != DBNull.Value ? rowSelected["CodeMau"] : (object)DBNull.Value;
                    newRow["TenDVVT"] = rowSelected["DonVi"] != DBNull.Value ? rowSelected["DonVi"] : (object)DBNull.Value;
                    newRow["LoaiNPL"] = rowSelected["LoaiVTNPL"] != DBNull.Value ? rowSelected["LoaiVTNPL"] : (object)DBNull.Value;
                    newRow["IsNPL"] = rowSelected["IsNPL"] != DBNull.Value ? rowSelected["IsNPL"] : (object)DBNull.Value;
                    newRow["SoLuong"] = (TongNhuCau - TonKho) < 0 ? 0 : (TongNhuCau - TonKho);
                    newRow["TieuHao"] = (TongNhuCau - TonKho) < 0 ? 0 : (TongNhuCau - TonKho);
                    newRow["GiaCat"] = 0.0;
                    newRow["GiaGiaCong"] = 0.0;
                    newRow["MaTienTe"] = TienTeDeault;
                    newRow["HaoHut"] = 0;
                    newRow["NhuCau"] = TongNhuCau;
                    newRow["TonKho"] = TonKho;
                    newRow["IsXacNhan"] = false;
                    newRow["IsDuyet"] = false;
                    dsVatTuDM.Tables["DinhMucVatTuTP"].Rows.Add(newRow);


                }

                foreach (DataRow rowSelected in frm.tblVatTuDM_DetailSelected.Rows)
                {
                    // Kiểm tra trùng ID nếu cần
                    bool isDuplicate = dsVatTuDM.Tables["DinhMucVatTuTP_Detail"].AsEnumerable()
                        .Any(r => r["MaVTID"]?.ToString() == rowSelected["MaVTID"]?.ToString()
                               && r["MaCLVTID"]?.ToString() == rowSelected["MaCLVTID"]?.ToString()
                               && r["MaVTID"]?.ToString() == rowSelected["MaVTID"]?.ToString()
                               && r["KhoVaiID"]?.ToString() == rowSelected["KhoVaiID"]?.ToString()

                               );

                    if (isDuplicate) continue; // bỏ qua nếu đã tồn tại

                    DataRow newRow = dsVatTuDM.Tables["DinhMucVatTuTP_Detail"].NewRow();
                    decimal.TryParse(rowSelected["SLTonKho"]?.ToString(), out decimal TonKho);
                    newRow["ID_DMThanhPhan_Parent"] = rowSelected["ID_VatTu"];
                    newRow["MaCLVTID"] = rowSelected["MaCLVTID"];
                    newRow["MaVTID"] = rowSelected["MaVTID"];
                    newRow["MauVTID"] = rowSelected["MauVTID"];
                    newRow["KhoVaiID"] = rowSelected["KhoVaiID"];
                    newRow["MaDVT"] = rowSelected["MaDVVT"] != DBNull.Value ? rowSelected["MaDVVT"] : (object)DBNull.Value;
                    newRow["DinhMucTP"] = rowSelected["DinhMuc"];
                    newRow["GhiChu"] = "";
                    newRow["TenCL"] = rowSelected["TenCL"] != DBNull.Value ? rowSelected["TenCL"] : (object)DBNull.Value;
                    newRow["MoTa"] = rowSelected["MoTa"] != DBNull.Value ? rowSelected["MoTa"] : (object)DBNull.Value;
                    newRow["ItemCode"] = rowSelected["ItemCode"] != DBNull.Value ? rowSelected["ItemCode"] : (object)DBNull.Value;
                    newRow["KhoVai"] = rowSelected["KhoVai"] != DBNull.Value ? rowSelected["KhoVai"] : (object)DBNull.Value;
                    newRow["MauVT"] = rowSelected["MauVT"] != DBNull.Value ? rowSelected["MauVT"] : (object)DBNull.Value;
                    newRow["CodeMau"] = rowSelected["CodeMau"] != DBNull.Value ? rowSelected["CodeMau"] : (object)DBNull.Value;
                    newRow["TenDVVT"] = rowSelected["TenDVVT"] != DBNull.Value ? rowSelected["TenDVVT"] : (object)DBNull.Value;
                    newRow["LoaiNPL"] = rowSelected["LoaiNPL"] != DBNull.Value ? rowSelected["LoaiNPL"] : (object)DBNull.Value;
                    newRow["IsNPL"] = rowSelected["IsNPL"] != DBNull.Value ? rowSelected["IsNPL"] : (object)DBNull.Value;
                    newRow["SoLuong"] = 0;
                    newRow["TieuHao"] = 0.0;
                    newRow["GiaCat"] = 0.0;
                    newRow["GiaGiaCong"] = 0.0;
                    newRow["MaTienTe"] = TienTeDeault;
                    newRow["TonKho"] = TonKho;
                    dsVatTuDM.Tables["DinhMucVatTuTP_Detail"].Rows.Add(newRow);
                }

                grcVattuTP.DataSource = dsDinhMucVatTuTP;
                grcVattuTP.DataMember = "DinhMucVatTuTP";
                grcVattuTP.RefreshDataSource();



                if (dsDinhMucVatTuTP == null) return;
                if (dsDinhMucVatTuTP.Tables["DinhMucVatTuTP"]?.Rows?.Count > 0)
                {
                    DataTable tblVatTuThanhPhan = dsDinhMucVatTuTP.Tables["DinhMucVatTuTP"];
                    foreach (DataRow rowVatTu in tblVatTuThanhPhan.Rows)
                    {
                        decimal soLuong = Convert.ToDecimal(rowVatTu["SoLuong"]);
                        decimal haoHut = Convert.ToDecimal(rowVatTu["HaoHut"]);
                        decimal DMCha = Convert.ToDecimal(rowVatTu["DinhMucTP"]);
                        decimal tieuHaoCha = soLuong + (soLuong * haoHut) / 100;
                        int parentID = Convert.ToInt32(rowVatTu["ID_DMThanhPhan_Parent"]);
                        UpdateDetailRows(parentID, tieuHaoCha, DMCha);
                    }
                }

                grvVattuTP.ExpandAllGroups();
                grvVattuTP_Detail.ExpandAllGroups();

            }
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataTable tblSaveVatTuThanhPhan = CreateTableVatTuTP_Save();
                DataSet dsSaveVatTuThanhPhan = grcVattuTP.DataSource as DataSet;

                DataTable tblVatTuThanhPhan = dsSaveVatTuThanhPhan.Tables["DinhMucVatTuTP"];
                DataTable tblVatTuThanhPhan_Detail = dsSaveVatTuThanhPhan.Tables["DinhMucVatTuTP_Detail"];


                if (tblVatTuThanhPhan_Detail != null && tblVatTuThanhPhan_Detail.Rows.Count > 0)
                {
                    foreach (DataRow row in tblVatTuThanhPhan_Detail.Rows)
                    {
                        DataRow newRow = tblSaveVatTuThanhPhan.NewRow();

                        //newRow["ID"] = row["ID"] != DBNull.Value ? Convert.ToInt64(row["ID"]) : (object)DBNull.Value;
                        newRow["ID_DMThanhPhan_Parent"] = row["ID_DMThanhPhan_Parent"] != DBNull.Value ? Convert.ToInt64(row["ID_DMThanhPhan_Parent"]) : (object)DBNull.Value;
                        newRow["MaLenhSX"] = MaLenhSanXuat;
                        newRow["MaGop"] = MaGop;
                        newRow["Dot"] = searchLookUpDot.EditValue;
                        newRow["MaLenh"] = MaLenh;
                        newRow["MaDH"] = _rowFocused["MaDH"];
                        newRow["MaHang"] = _rowFocused["MaHang"];
                        newRow["MaCLVTID"] = row["MaCLVTID"] != DBNull.Value ? row["MaCLVTID"].ToString() : (object)DBNull.Value;
                        newRow["MaVTID"] = row["MaVTID"] != DBNull.Value ? row["MaVTID"].ToString() : (object)DBNull.Value;
                        newRow["MauVTID"] = row["MauVTID"] != DBNull.Value ? row["MauVTID"].ToString() : (object)DBNull.Value;
                        newRow["KhoVaiID"] = row["KhoVaiID"] != DBNull.Value ? row["KhoVaiID"].ToString() : (object)DBNull.Value;
                        newRow["MaDVT"] = row["MaDVT"] != DBNull.Value ? row["MaDVT"].ToString() : (object)DBNull.Value;
                        newRow["SoLuong"] = row["SoLuong"] != DBNull.Value ? Convert.ToDouble(row["SoLuong"]) : (object)DBNull.Value;
                        newRow["DMVatTuThanhPhan"] = row["DinhMucTP"] != DBNull.Value ? Convert.ToDouble(row["DinhMucTP"]) : (object)DBNull.Value; // DinhMucTP → DMVatTuThanhPhan
                        newRow["TieuHao"] = row["TieuHao"] != DBNull.Value ? Convert.ToDouble(row["TieuHao"]) : (object)DBNull.Value;
                        newRow["GiaCat"] = row["GiaCat"] != DBNull.Value ? Convert.ToDouble(row["GiaCat"]) : (object)DBNull.Value;
                        newRow["GiaGiaCong"] = row["GiaGiaCong"] != DBNull.Value ? Convert.ToDouble(row["GiaGiaCong"]) : (object)DBNull.Value;
                        newRow["MaTienTe"] = row["MaTienTe"] != DBNull.Value ? row["MaTienTe"].ToString() : (object)DBNull.Value;
                        newRow["GhiChu"] = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : (object)DBNull.Value;
                        newRow["IsParent"] = 0;
                        newRow["CreateDate"] = DateTime.Now;
                        newRow["Creater"] = GlobleData.UserName;
                        newRow["TienTeQD"] = SearchLookupDVTienTe.EditValue;
                        newRow["TonKho"] = row["TonKho"];
                        newRow["NhuCau"] = 0;
                        newRow["HaoHut"] = row["HaoHut"];


                        tblSaveVatTuThanhPhan.Rows.Add(newRow);
                    }
                }


                if (tblVatTuThanhPhan != null && tblVatTuThanhPhan.Rows.Count > 0)
                {
                    foreach (DataRow row in tblVatTuThanhPhan.Rows)
                    {

                        bool isDuplicate = tblSaveVatTuThanhPhan.AsEnumerable()
                            .Any(r => r["MaVTID"]?.ToString() == row["MaVTID"]?.ToString()
                                   && r["MauVTID"]?.ToString() == row["MauVTID"]?.ToString());

                        if (isDuplicate) continue;

                        DataRow newRow = tblSaveVatTuThanhPhan.NewRow();

                        //newRow["ID"] = row["ID_DMThanhPhan_Parent"] != DBNull.Value ? Convert.ToInt64(row["ID_DMThanhPhan_Parent"]) : (object)DBNull.Value; // Master dùng ID_DMThanhPhan_Parent làm ID
                        newRow["ID_DMThanhPhan_Parent"] = row["ID_DMThanhPhan_Parent"] != DBNull.Value ? Convert.ToInt64(row["ID_DMThanhPhan_Parent"]) : (object)DBNull.Value;
                        newRow["MaLenhSX"] = MaLenhSanXuat;
                        newRow["MaGop"] = MaGop;
                        newRow["Dot"] = searchLookUpDot.EditValue;
                        newRow["MaLenh"] = MaLenh;
                        newRow["MaDH"] = _rowFocused["MaDH"];
                        newRow["MaHang"] = _rowFocused["MaHang"];
                        newRow["MaCLVTID"] = row["MaCLVTID"] != DBNull.Value ? row["MaCLVTID"].ToString() : (object)DBNull.Value;
                        newRow["MaVTID"] = row["MaVTID"] != DBNull.Value ? row["MaVTID"].ToString() : (object)DBNull.Value;
                        newRow["MauVTID"] = row["MauVTID"] != DBNull.Value ? row["MauVTID"].ToString() : (object)DBNull.Value;
                        newRow["KhoVaiID"] = row["KhoVaiID"] != DBNull.Value ? row["KhoVaiID"].ToString() : (object)DBNull.Value;
                        newRow["MaDVT"] = row["MaDVT"] != DBNull.Value ? row["MaDVT"].ToString() : (object)DBNull.Value;
                        newRow["SoLuong"] = row["SoLuong"] != DBNull.Value ? Convert.ToDouble(row["SoLuong"]) : (object)DBNull.Value;
                        newRow["DMVatTuThanhPhan"] = row["DinhMucTP"] != DBNull.Value ? Convert.ToDouble(row["DinhMucTP"]) : (object)DBNull.Value; // DinhMucTP → DMVatTuThanhPhan
                        newRow["TieuHao"] = row["TieuHao"] != DBNull.Value ? Convert.ToDouble(row["TieuHao"]) : (object)DBNull.Value;
                        newRow["GiaCat"] = row["GiaCat"] != DBNull.Value ? Convert.ToDouble(row["GiaCat"]) : (object)DBNull.Value;
                        newRow["GiaGiaCong"] = row["GiaGiaCong"] != DBNull.Value ? Convert.ToDouble(row["GiaGiaCong"]) : (object)DBNull.Value;
                        newRow["MaTienTe"] = row["MaTienTe"] != DBNull.Value ? row["MaTienTe"].ToString() : (object)DBNull.Value;
                        newRow["GhiChu"] = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : (object)DBNull.Value;
                        newRow["IsDuyet"] = row["IsDuyet"] != DBNull.Value ? Convert.ToBoolean(row["IsDuyet"]) : (object)DBNull.Value;
                        newRow["IsXacNhan"] = row["IsXacNhan"] != DBNull.Value ? Convert.ToBoolean(row["IsXacNhan"]) : (object)DBNull.Value;
                        newRow["IsParent"] = 1;
                        newRow["CreateDate"] = DateTime.Now;
                        newRow["Creater"] = GlobleData.UserName;
                        newRow["TienTeQD"] = SearchLookupDVTienTe.EditValue;
                        newRow["TonKho"] = row["TonKho"];
                        newRow["NhuCau"] = row["NhuCau"];
                        newRow["HaoHut"] = row["HaoHut"];
                        tblSaveVatTuThanhPhan.Rows.Add(newRow);
                    }
                }


                string url = string.Format("{0}", URL + "ERPVatTuThanhPhan/Post?action=POST_DMThanhPhan");
                string jsonSave = JsonConvert.SerializeObject(tblSaveVatTuThanhPhan);
                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, jsonSave);
                }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadThanhPhanVatTu();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvVattuTP.GetSelectedRows();


                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    if (rowHandle < 0) continue;

                    DataRow drMaster = grvVattuTP.GetDataRow(rowHandle);
                    if (drMaster == null) continue;


                    int parentId = Convert.ToInt32(drMaster["ID_DMThanhPhan_Parent"]);


                    DataTable dtDetail = dsDinhMucVatTuTP.Tables["DinhMucVatTuTP_Detail"];
                    DataRow[] detailRows = dtDetail.Select($"ID_DMThanhPhan_Parent = {parentId}");
                    foreach (DataRow detailRow in detailRows)
                    {
                        detailRow.Delete();
                    }


                    if (drMaster["ID"].ToString() != "0")
                    {
                        string url = $"{URL}ERPVatTuThanhPhan/Delete?action=DeleteDMThanhPhan" +
                                     $"&para1={parentId}" +
                                     $"&para2={searchLookUpMaLenh.EditValue}" +
                                     $"&para3={searchLookUpDot.EditValue}";

                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }


                    grvVattuTP.DeleteRow(rowHandle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;
                if (view.FocusedColumn == null) return;
                if (IsXacNhan || IsDuyet)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
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

        private void grvVattuTP_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvVattuTP_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvVattuTP_Detail_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvVattuTP_Detail_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }
        private void grv_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            var column = view.FocusedColumn;
            var cellValue = view.GetRowCellValue(view.FocusedRowHandle, column);
            if (cellValue != null && cellValue.ToString() == "0")
            {
                view.SetRowCellValue(view.FocusedRowHandle, column, string.Empty);
            }
        }

        private void bttRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadThanhPhanVatTu();
        }

        private void btnThemDot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tblDot?.Rows?.Count == 0 || tblDot == null)
            {
                MaxSTTDot = 1;

                tblDot = new DataTable();
                tblDot.Columns.Add("Dot", typeof(string));
                tblDot.Columns.Add("STTDot", typeof(string));

            }
            else
            {
                MaxSTTDot = MaxSTTDot + 1;
            }


            DataRow rowDot = tblDot.NewRow();
            rowDot["Dot"] = MaxSTTDot;
            rowDot["STTDot"] = MaxSTTDot;
            tblDot.Rows.Add(rowDot);

            searchLookUpDot.Properties.ValueMember = "STTDot";
            searchLookUpDot.Properties.DisplayMember = "Dot";
            searchLookUpDot.Properties.DataSource = tblDot;

            int MaxDot = tblDot.AsEnumerable()
            .Select(x => x["STTDot"] != DBNull.Value && int.TryParse(x["STTDot"].ToString(), out int v) ? v : 0)
            .DefaultIfEmpty(0)
            .Max();

            searchLookUpDot.EditValue = MaxDot;
        }

        private void grv_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            if (e.Column.FieldName == "ThanhTien" && e.IsGetData)
            {
                decimal GiaCat = 0;
                decimal GiaGiaCong = 0;
                decimal SoLuong = 0;
                decimal DinhMuc = 0;


                object valGiaCat = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "GiaCat");
                object valGiaGiaCong = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "GiaGiaCong");
                object valSoLuong = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "TieuHao");
                object valDinhMuc = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "DinhMucTP");

                if (valGiaCat != null && valGiaCat != DBNull.Value)
                    decimal.TryParse(valGiaCat.ToString(), out GiaCat);

                if (valGiaGiaCong != null && valGiaGiaCong != DBNull.Value)
                    decimal.TryParse(valGiaGiaCong.ToString(), out GiaGiaCong);

                if (valSoLuong != null && valSoLuong != DBNull.Value)
                    decimal.TryParse(valSoLuong.ToString(), out SoLuong);

                if (valDinhMuc != null && valDinhMuc != DBNull.Value)
                    decimal.TryParse(valDinhMuc.ToString(), out DinhMuc);

                decimal tongChiPhi = (SoLuong * DinhMuc) * (GiaCat + GiaGiaCong);
                e.Value = tongChiPhi;
                SumThanhTienVT();
            }
        }
        private bool _isUpdating = false;
        private bool _isUpdatingDetail = false;

        private void grvVattuTP_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_isUpdating) return;
            try
            {
                _isUpdating = true;
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;


                decimal.TryParse(view.GetFocusedRowCellValue("SoLuong")?.ToString(), out decimal soLuong);
                decimal haoHut = Convert.ToDecimal(view.GetFocusedRowCellValue("HaoHut"));
                decimal dinhmucCha = Convert.ToDecimal(view.GetFocusedRowCellValue("DinhMucTP"));


                if (e.Column.FieldName == "SoLuong")
                    soLuong = Convert.ToDecimal(e.Value);
                else if (e.Column.FieldName == "HaoHut")
                    haoHut = Convert.ToDecimal(e.Value);
                else
                    return;


                decimal tieuHaoCha = soLuong + (soLuong * haoHut) / 100;
                view.SetFocusedRowCellValue("TieuHao", tieuHaoCha);


                int parentID = Convert.ToInt32(view.GetFocusedRowCellValue("ID_DMThanhPhan_Parent"));


                UpdateDetailRows(parentID, tieuHaoCha, dinhmucCha);
            }
            catch { }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateDetailRows(int parentID, decimal tieuHaoCha, decimal dinhmucCha)
        {
            if (_isUpdatingDetail) return;
            try
            {
                _isUpdatingDetail = true;

                DataTable dtDetail = dsDinhMucVatTuTP.Tables["DinhMucVatTuTP_Detail"];
                if (dtDetail == null) return;


                DataRow[] childRows = dtDetail.Select(
                    $"ID_DMThanhPhan_Parent = {parentID}");

                foreach (DataRow row in childRows)
                {
                    decimal dinhMucCon = row["DinhMucTP"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DinhMucTP"]);
                    decimal haoHutCon = row["HaoHut"] == DBNull.Value ? 0 : Convert.ToDecimal(row["HaoHut"]);

                    decimal soLuongCon = (dinhmucCha == 0) ? 0 : (dinhMucCon * tieuHaoCha) / dinhmucCha;


                    decimal tieuHaoCon = soLuongCon + (soLuongCon * haoHutCon) / 100;

                    row.BeginEdit();
                    row["SoLuong"] = soLuongCon;
                    row["TieuHao"] = tieuHaoCon;
                    row.EndEdit();
                }


                RefreshDetailView(parentID);
            }
            catch { }
            finally
            {
                _isUpdatingDetail = false;
            }
        }


        private void RefreshDetailView(int parentID)
        {
            try
            {
                GridView masterView = grcVattuTP.MainView as GridView;
                if (masterView == null) return;

                for (int i = 0; i < masterView.DataRowCount; i++)
                {

                    GridView detailView = masterView.GetDetailView(i, 0) as GridView;
                    if (detailView == null) continue;


                    int rowParentID = Convert.ToInt32(
                        masterView.GetRowCellValue(i, "ID_DMThanhPhan_Parent"));

                    if (rowParentID == parentID)
                    {
                        detailView.RefreshData();
                        break;
                    }
                }
            }
            catch { }
        }


        private void grvVattuTP_Detail_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_isUpdatingDetail) return;
            try
            {
                _isUpdatingDetail = true;
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;

                if (e.Column.FieldName == "HaoHut")
                {
                    decimal haoHut = Convert.ToDecimal(e.Value);
                    decimal soLuong = Convert.ToDecimal(view.GetFocusedRowCellValue("SoLuong"));
                    decimal tieuHao = soLuong + (soLuong * haoHut) / 100;
                    view.SetFocusedRowCellValue("TieuHao", tieuHao);
                }
                else if (e.Column.FieldName == "SoLuong")
                {
                    decimal soLuong = Convert.ToDecimal(e.Value);
                    decimal haoHut = Convert.ToDecimal(view.GetFocusedRowCellValue("HaoHut"));
                    decimal tieuHao = soLuong + (soLuong * haoHut) / 100;
                    view.SetFocusedRowCellValue("TieuHao", tieuHao);
                }
            }
            catch { }
            finally
            {
                _isUpdatingDetail = false;
            }
        }
        private void gridView_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            string fieldName = view.FocusedColumn?.FieldName;

            if (lstValiDate.Contains(fieldName))
            {
                //if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                //{
                //    e.Valid = false;
                //    e.ErrorText = $"{view.FocusedColumn.Caption} không được để trống!";
                //    return;
                //}

                if (!double.TryParse(e.Value.ToString(), out double dinhMuc))
                {
                    e.Valid = false;
                    e.ErrorText = $"{view.FocusedColumn.Caption} không hợp lệ!";
                    return;
                }


                if (dinhMuc <= 0)
                {
                    e.Valid = false;
                    e.ErrorText = $"{view.FocusedColumn.Caption} phải lớn hơn 0!";
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
        private void btnXacNhan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url = URL + $"ERPVatTuThanhPhan/Update?action=XacNhan&para1={searchLookUpMaLenh.EditValue}&para2={searchLookUpDot.EditValue ?? 1}&para3=1";

                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, null);
                }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadThanhPhanVatTu();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnHuyXacNhan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url = URL + $"ERPVatTuThanhPhan/Update?action=XacNhan&para1={searchLookUpMaLenh.EditValue}&para2={searchLookUpDot.EditValue ?? 1}&para3=0";

                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, null);
                }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadThanhPhanVatTu();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnDuyetVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url = URL + $"ERPVatTuThanhPhan/Update?action=DuyetVT&para1={searchLookUpMaLenh.EditValue}&para2={searchLookUpDot.EditValue ?? 1}&para3=1";

                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, null);
                }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadThanhPhanVatTu();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnHuyDuyetVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url = URL + $"ERPVatTuThanhPhan/Update?action=DuyetVT&para1={searchLookUpMaLenh.EditValue}&para2={searchLookUpDot.EditValue ?? 1}&para3=0";

                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, null);
                }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadThanhPhanVatTu();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SearchLookupDVTienTe_EditValueChanged(object sender, EventArgs e)
        {
            SumThanhTienVT();
        }


    }
}