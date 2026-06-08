using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmCanDoiNPLV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private string _nplID;

        private bool isUpdate = false;
        private decimal slTonKho = 0;
        private string maDonHang;
        private bool _suppressEditValueChanged = false;

        List<DataRow> seacrhLookupEdit1CheckedRows;
        List<string> seacrhLookupEdit1PreviousID;
        string searchLookupEdit1Display;

        private int searchLookupEdit2RowHandle;
        private string searchLookupEdit2MaNhom = "";
        private string searchLookupEdit2MaMau = "";
        private string searchLookupEdit2MaSize = "";
        private string maDH;

        private DataTable gridControl1Table;
        private DataTable gridControl2Table;
        private DataTable candoiNPLTongTable;
        private DataTable candoiVTTable;
        private DataTable donhangTable;
        private DataTable donhangdachonTable;
        private DataTable vattuTable;
        private DataTable tonkhoTable;
        private DataTable filterVatTuTable;
        private DataTable phieucandoiTable;
        private DataTable ngayDBTable;

        private DataRow vattuRow;

        private Dictionary<string, string> vattudachon = new Dictionary<string, string>
        {
            {"MaVTID", "" },
            {"MaNhomVT", "" },
            {"MaMauVT", "" },
            {"MaKhoVT", "" }
        };

        List<string> lstFormatFieldName = new List<string> { "SLCanDoiKho", "SLCapPhat", "SLTonKhoSauCanDoi", "SLMuaThem" };
        public frmCanDoiNPLV1(string nplID = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            //string nplID = "key-202511261604413775200";
            this._nplID = nplID;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OverrideFont(this, new Font("Tahoma", 8.25f));
            //getCanDoiNpLTable();
            //getSLTonKho();
            if (_nplID != "") isUpdate = true;
            if (isUpdate)
            {
                layoutControlGroup1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //upBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //downBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                getCanDoiNPLToUpdate();
                getNgayDongBoDK();
                getDonHangToUpdate();
                calcDonHangDaChon();
                getVatTuToUpdate();
                loadCanDoiNPLToUpdate();
                calcCanDoiNPL(false);
                loadNgayDongBoDK();
                loadVatTuDaChon();
                loadGridControl1();
                loadGridControl2();
            }
            else
            {
                layoutControlGroup2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                loadSearchLookupEdit1();
                loadSLTonKho();
            }

            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
        }
        private void OverrideFont(Control parent, Font font)
        {
            parent.Font = font;
            foreach (Control ctrl in parent.Controls)
            {
                OverrideFont(ctrl, font);
            }
        }


        // load to update
        private void loadCanDoiNPLToUpdate()
        {
            if (phieucandoiTable == null || phieucandoiTable.Columns.Count <= 0 || phieucandoiTable.Rows.Count <= 0) return;
            DataRow row = phieucandoiTable.Rows[0];
            slTonKho = XuLyVTUnits.SmartTryParse<decimal>(row["SLTonKhoSauCanDoi"].ToString()) + XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"]);
            slTonKhoText.EditValue = slTonKho;
            textEdit2.Text = vattuRow["ChiTiet"].ToString();

            List<string> displayValue = new List<string>();
            foreach(DataRow loopRow in donhangdachonTable.Rows)
            {
                string dh = loopRow["MaDH"].ToString();
                string hh = loopRow["TenHang"].ToString();
                string sl = loopRow["SoLuong"].ToString();
                string dhml = dh + "|" + hh + "|" + sl;
                displayValue.Add(dhml);
            }
            textEdit1.Text = string.Join(", ", displayValue);
            textEdit1.EditValue = string.Join(", ", displayValue);
        }
        private void getCanDoiNPLToUpdate()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getphieucansuatheovattu";
            request.Parameter = _nplID;
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            phieucandoiTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            //if (!candoiVTTable.Columns.Contains("IsSelected"))
            //candoiVTTable.Columns.Add("IsSelected", typeof(bool));
            if (!phieucandoiTable.Columns.Contains("generatedID"))
                phieucandoiTable.Columns.Add("generatedID");
            //if (!candoiVTTable.Columns.Contains("SLCapPhat"))
            //    candoiVTTable.Columns.Add("SLCapPhat");
            //if (!candoiVTTable.Columns.Contains("SLCanDoiKho"))
            //    candoiVTTable.Columns.Add("SLCanDoiKho");
            //if (!candoiVTTable.Columns.Contains("SLTonKhoSauCanDoi"))
            //    candoiVTTable.Columns.Add("SLTonKhoSauCanDoi");
            //if (!candoiVTTable.Columns.Contains("SLMuaThem"))
            //    candoiVTTable.Columns.Add("SLMuaThem");
            foreach (DataRow row in phieucandoiTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
            if (phieucandoiTable != null && phieucandoiTable.Rows.Count > 0)
            {
                vattuRow = phieucandoiTable.Rows[0];
                maDH = phieucandoiTable.Rows[0]["MaDH"].ToString();
            }
        }
        private void getDonHangToUpdate()
        {
            try
            {
                var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
                request.Action = "getdanhsachdonhang";
                if (phieucandoiTable != null && phieucandoiTable.Rows.Count > 0)
                {
                    foreach (DataRow row in phieucandoiTable.Rows)
                    {
                        DataRow newRow = request.TypeTable.NewRow();
                        newRow["MaDH"] = row["MaDH"];
                        request.TypeTable.Rows.Add(newRow);
                    }
                }

                string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                donhangdachonTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
        private void getVatTuToUpdate()
        {
            
            if (vattuRow == null) return;
            SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            try
            {
                var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
                request.Action = "getvattucancandoi";

                if (phieucandoiTable != null && phieucandoiTable.Rows.Count > 0)
                {
                    foreach (DataRow row in phieucandoiTable.Rows)
                    {
                        DataRow newRow = request.TypeTable.NewRow();
                        newRow["MaDH"] = row["MaDH"];
                        request.TypeTable.Rows.Add(newRow);
                    }
                }

                DataRow reqRow = request.TypeTable.NewRow();
                reqRow["MaVTID"] = vattuRow["MaVTID"].ToString();
                reqRow["MaNhomVT"] = vattuRow["MaNhomVT"].ToString();
                reqRow["MaMauVT"] = vattuRow["MaMauVT"].ToString();
                reqRow["MaKhoVT"] = vattuRow["MaKhoVT"].ToString();

                request.TypeTable.Rows.Add(reqRow);
                string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                candoiVTTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

                if (!candoiVTTable.Columns.Contains("generatedID"))
                    candoiVTTable.Columns.Add("generatedID");
                if (!candoiVTTable.Columns.Contains("SLNhuCau"))
                    candoiVTTable.Columns.Add("SLNhuCau", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("SLCanDoiKho"))
                    candoiVTTable.Columns.Add("SLCanDoiKho", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("SLTonKhoSauCanDoi"))
                    candoiVTTable.Columns.Add("SLTonKhoSauCanDoi", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("SLMuaThem"))
                    candoiVTTable.Columns.Add("SLMuaThem", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("TyLeMuaThem"))
                    candoiVTTable.Columns.Add("TyLeMuaThem", typeof(decimal));
                foreach (DataRow row in candoiVTTable.Rows)
                {
                    row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
                }
                if (candoiVTTable != null && candoiVTTable.Rows.Count > 0)
                {
                    vattuRow = candoiVTTable.Rows[0];
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
            finally
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
        }
        private void calcVatTuToUpdate()
        {

        }
        private void loadNgayDongBoDK()
        {
            if (ngayDBTable == null || ngayDBTable.Columns.Count <= 0 || ngayDBTable.Rows.Count <= 0) return;
            string ngayStr = ngayDBTable.Rows[0]["NgayDKDBo"]?.ToString(); // lấy từ DataTable
            DateTime? ngayValue = null;

            if (!string.IsNullOrWhiteSpace(ngayStr))
            {
                // Parse theo format dd/MM/yyyy
                if (DateTime.TryParseExact(
                    ngayStr,
                    "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    ngayValue = parsedDate;
                }
            }
            ngayDBDKEdit.EditValue = ngayValue;
        }
        private void getNgayDongBoDK()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getngaydongbo";
            request.Parameter = maDH;
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            ngayDBTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
        }


        // đơn hàng
        private void loadSearchLookupEdit1()
        {
            getDonHang();
            DataTable dt = donhangTable;
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit1.Properties.DataSource = dt;
            searchLookUpEdit1.Properties.DisplayMember = "MaDH";
            searchLookUpEdit1.Properties.ValueMember = "MaDH";
            searchLookUpEdit1.Properties.PopulateViewColumns();                             

            GridView view = searchLookUpEdit1.Properties.View;

            view.Columns["MaDH"].Caption = "Đơn hàng";
            view.Columns["TenHang"].Caption = "Tên hàng";
            //view.Columns["MaLenh"].Caption = "Lệnh";
            view.Columns["TenKH"].Caption = "Khách hàng";
            view.Columns["NgayDKVC"].Caption = "Ngày dự kiến VC";
            view.Columns["NgayThucTeVC"].Caption = "Ngày thực tế VC";
            view.Columns["SoLuong"].Caption = "Số lượng";

            //view.Columns["TenLenh"].Visible = false;
            view.Columns["MaKH"].Visible = false;
            view.Columns["MaHang"].Visible = false;
            view.Columns["NgayTao"].Visible = false;
            view.Columns["generatedID"].Visible = false;

            view.OptionsFind.AlwaysVisible = true;
        }
        private void getDonHang()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getdonhangtong";
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            donhangTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!donhangTable.Columns.Contains("generatedID"))
                donhangTable.Columns.Add("generatedID");
            foreach (DataRow row in donhangTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (_suppressEditValueChanged)
                return;
            searchLookUpEdit2.Enabled = false;
            try
            {
                vattuTable = null;
                candoiVTTable = null;
                gridControl1Table = null;
                gridControl1.DataSource = null;
                gridView1.RefreshData();                
                getDonHangDaChon();
                calcDonHangDaChon();
                loadGridControl2();                
                loadSearchLookupEdit2();
                slTonKho = 0;
                loadSLTonKho();
            }
            finally
            {
                searchLookUpEdit2.Enabled = true;
            }

        }
        private void searchLookUpEdit1_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            GridView view = searchLookUpEdit1.Properties.View;
            int[] selectedRows = view.GetSelectedRows();

            List<string> displayValue = new List<string>();
            seacrhLookupEdit1CheckedRows = new List<DataRow>();
            List<string> searchLookupEdit1CurrentID = new List<string>();

            foreach (int handle in selectedRows)
            {
                string dh = view.GetRowCellValue(handle, "MaDH").ToString();
                string hh = view.GetRowCellValue(handle, "TenHang").ToString();
                string sl = view.GetRowCellValue(handle, "SoLuong").ToString();
                string dhml = dh + "|" + hh + "|" + sl;
                displayValue.Add(dhml);

                string genid = view.GetRowCellValue(handle, "generatedID").ToString();
                searchLookupEdit1CurrentID.Add(genid);

                DataRow row = view.GetDataRow(handle);
                if (row != null)
                {
                    seacrhLookupEdit1CheckedRows.Add(row);
                }
            }

            if (seacrhLookupEdit1PreviousID == null) seacrhLookupEdit1PreviousID = new List<string>();
            seacrhLookupEdit1PreviousID.Sort();
            searchLookupEdit1CurrentID.Sort();

            bool isSame = seacrhLookupEdit1PreviousID.SequenceEqual(searchLookupEdit1CurrentID);

            if (!isSame)
            {
                _suppressEditValueChanged = true;
                searchLookUpEdit1.EditValue = string.Join(", ", displayValue);
                _suppressEditValueChanged = false;

                searchLookupEdit1Display = string.Join(", ", displayValue);

                seacrhLookupEdit1PreviousID = new List<string>(searchLookupEdit1CurrentID);
            }
        }
        private void searchLookUpEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = searchLookupEdit1Display;
        }
        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            
        }
        private void searchLookUpEdit1View_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            e.Appearance.ForeColor = Color.Black;
            e.Appearance.BackColor = Color.LightGray;

            e.Painter.DrawObject(e.Info);
            e.Handled = true;
        }
        private void searchLookUpEdit1_QueryPopUp(object sender, CancelEventArgs e)
        {
            SearchLookUpEdit slup = sender as SearchLookUpEdit;
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int desiredWidth = Math.Max(500, (int)(screenWidth * 0.5));
            slup.Properties.PopupFormWidth = desiredWidth;
        }
        private void searchLookUpEdit1View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            
        }



        // vật tư
        private void loadSearchLookupEdit2()
        {
            getVatTu();
            searchLookUpEdit2.Properties.DataSource = null;
            searchLookUpEdit2.Properties.View.Columns.Clear();
            searchLookUpEdit2.EditValue = null;
            if (checkSameSelect()) searchLookUpEdit2.EditValue = vattudachon["MaVTID"];
            else
            {
                
                resetVatTuDaChon();
                vattudachon["MaVTID"] = "";
                vattudachon["MaNhomVT"] = "";
                vattudachon["MaMauVT"] = "";
                vattudachon["MaKhoVT"] = "";
            }
            
            searchLookUpEdit2.Properties.DisplayMember = string.Empty;
            searchLookUpEdit2.Properties.ValueMember = string.Empty;
            //calcVatTu();
            //calcVatTuJoinTonKho();
            DataTable dt = vattuTable;
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0)
            {
                resetVatTuDaChon();
                vattudachon["MaVTID"] = "";
                vattudachon["MaNhomVT"] = "";
                vattudachon["MaMauVT"] = "";
                vattudachon["MaKhoVT"] = "";
                vattuTable = null;
                candoiVTTable = null;
                gridControl1Table = null;
                gridControl1.DataSource = null;
                return;
            }

            if (!dt.Columns.Contains("IsNPLText"))
                dt.Columns.Add("IsNPLText", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";
            }


            searchLookUpEdit2.Properties.DataSource = dt;
            searchLookUpEdit2.Properties.DisplayMember = "ChiTiet";
            searchLookUpEdit2.Properties.ValueMember = "MaVTID";
            searchLookUpEdit2.Properties.PopulateViewColumns();

            GridColumn colSTT = searchLookUpEdit2.Properties.View.Columns.AddField("STT");
            colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            colSTT.Caption = "STT";
            colSTT.OptionsColumn.AllowEdit = false;
            colSTT.Visible = true;
            colSTT.VisibleIndex = 0;
            colSTT.OptionsColumn.FixedWidth = true;
            colSTT.Width = 50;

            GridView view = searchLookUpEdit2.Properties.View;
            view.GroupFormat = "{1}";

            view.Columns["IsNPLText"].GroupIndex = 0;
            view.Columns["ChungLoaiVatTu"].GroupIndex = 1;


            view.Columns["MaVT"].Caption = "Item code";
            view.Columns["ChiTiet"].Caption = "Mô tả";
            view.Columns["MauVT"].Caption = "Màu vật tư";
            view.Columns["KhoVai"].Caption = "Khổ/Size";
            view.Columns["TenDVVT"].Caption = "Đơn vị";
            view.Columns["TonKho"].Caption = "Tồn kho";
            view.Columns["CodeMau"].Caption = "Code màu";
            //view.Columns["ChungLoaiChiTiet"].Caption = "Chủng loại chi tiết";

            view.Columns["MaVTID"].Visible = false;
            view.Columns["MaVTID_Key"].Visible = false;
            view.Columns["MaNhomVT"].Visible = false;
            view.Columns["IsNPL"].Visible = false;
            view.Columns["MaMauVT"].Visible = false;
            view.Columns["MaKhoVT"].Visible = false;
            view.Columns["MaDVVT"].Visible = false;
            view.Columns["TonKhoCu"].Visible = false;
            view.Columns["Sort"].Visible = false;

            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            textEdit.Mask.EditMask = "n2"; // 2 số thập phân
            textEdit.Mask.UseMaskAsDisplayFormat = true;

            view.Columns["TonKho"].ColumnEdit = textEdit;

            view.OptionsFind.AlwaysVisible = true;

            
        }
        private void getVatTu()
        {            
            gridControl1.DataSource = null;
            gridView1.RefreshData();
            try
            {
                var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
                request.Action = "getvattu";
                if (seacrhLookupEdit1CheckedRows != null && seacrhLookupEdit1CheckedRows.Count > 0)
                {
                    foreach (DataRow row in seacrhLookupEdit1CheckedRows)
                    {
                        DataRow newRow = request.TypeTable.NewRow();
                        newRow["MaDH"] = row["MaDH"];
                        request.TypeTable.Rows.Add(newRow);
                    }
                }

                string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }   
        private bool checkSameSelect()
        {
            DataTable dt = vattuTable;
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return false;
            foreach(DataRow row in dt.Rows)
            {
                if (row["MaVTID"].ToString() == vattudachon["MaVTID"] &&
                    row["MaNhomVT"].ToString() == vattudachon["MaNhomVT"] &&
                    row["MaMauVT"].ToString() == vattudachon["MaMauVT"] &&
                    row["MaKhoVT"].ToString() == vattudachon["MaKhoVT"]
                    ) return true;
            }
            return false;
        }
        private void calcVatTu()
        {
            var groupedResult = from row in vattuTable.AsEnumerable()
                                group row by new
                                {
                                    MaVTID = row.Field<string>("MaVTID"),
                                    MaNhomVT = row.Field<string>("MaNhomVT"),
                                    ChungLoaiVatTu = row.Field<string>("ChungLoaiVatTu"),
                                    MaVT = row.Field<string>("MaVT"),
                                    ChiTiet = row.Field<string>("ChiTiet"),
                                    MaMauVT = row.Field<string>("MaMauVT"),
                                    MauVT = row.Field<string>("MauVT"),
                                    MaKhoVT = row.Field<string>("MaKhoVT"),
                                    KhoVai = row.Field<string>("KhoVai"),
                                    TenDVVT = row.Field<string>("TenDVVT"),
                                    TonKho = row.Field<double?>("TonKho") ?? 0,
                                    IsNPL = row.Field<bool>("IsNPL")
                                } into grp
                                select new
                                {
                                    grp.Key.MaVTID,
                                    grp.Key.MaNhomVT,
                                    grp.Key.ChungLoaiVatTu,
                                    grp.Key.MaVT,
                                    grp.Key.ChiTiet,
                                    grp.Key.MaMauVT,
                                    grp.Key.MauVT,
                                    grp.Key.MaKhoVT,
                                    grp.Key.KhoVai,
                                    grp.Key.TenDVVT,
                                    grp.Key.TonKho,
                                    grp.Key.IsNPL
                                    //Count = grp.Count(), // Example: count rows in each group
                                    //Cities = string.Join(", ", grp.Select(r => r.Field<string>("City")).Distinct()) // Example: list unique cities
                                };
            filterVatTuTable = new DataTable();
            filterVatTuTable.Columns.Add("MaVTID");
            filterVatTuTable.Columns.Add("MaNhomVT");
            filterVatTuTable.Columns.Add("ChungLoaiVatTu");
            filterVatTuTable.Columns.Add("MaVT");
            filterVatTuTable.Columns.Add("ChiTiet");
            filterVatTuTable.Columns.Add("MaMauVT");
            filterVatTuTable.Columns.Add("MauVT");
            filterVatTuTable.Columns.Add("MaKhoVT");
            filterVatTuTable.Columns.Add("KhoVai");
            filterVatTuTable.Columns.Add("TenDVVT");
            filterVatTuTable.Columns.Add("TonKho", typeof(double));
            filterVatTuTable.Columns.Add("IsNPL", typeof(bool));
            foreach (var item in groupedResult)
            {
                filterVatTuTable.Rows.Add(
                    item.MaVTID,
                    item.MaNhomVT,
                    item.ChungLoaiVatTu,
                    item.MaVT,
                    item.ChiTiet,
                    item.MaMauVT,
                    item.MauVT,
                    item.MaKhoVT,
                    item.KhoVai,
                    item.TenDVVT,
                    item.TonKho,
                    item.IsNPL
                );
            }
        }
        private void calcVatTuJoinTonKho()
        {
            //var query = from t1 in vattuTable.AsEnumerable()
            //            join t2 in tonkhoTable.AsEnumerable()
            //            on new
            //            {
            //                MaNhomVT = t1.Field<string>("MaNhomVT"),
            //                MaVTID = t1.Field<string>("MaVTID"),
            //                MauVT = t1.Field<string>("MaMauVT"),
            //                KhoVT = t1.Field<string>("MaKhoVT")
            //            }
            //            equals new
            //            {
            //                MaNhomVT = t2.Field<string>("MaNhom"),
            //                MaVTID = t2.Field<string>("MaVTID"),
            //                MauVT = t2.Field<string>("MauVTID"),
            //                KhoVT = t2.Field<string>("KhoVaiID")
            //            }
            //            into leftjoin 
            //            from sub in leftjoin.DefaultIfEmpty()
            //            select new
            //            {
            //                t1All = t1,
            //                TonKho = sub == null ? 0 : sub.Field<double>("TonKho")
            //            };
            //vattuJoinTonkhoTable = vattuTable.Clone();
            //vattuJoinTonkhoTable.Columns.Add("TonKho", typeof(float));
            //foreach (var item in query)
            //{
            //    DataRow newRow = vattuJoinTonkhoTable.NewRow();

            //    // copy toàn bộ cột từ A
            //    foreach (DataColumn col in vattuTable.Columns)
            //    {
            //        newRow[col.ColumnName] = item.t1All[col.ColumnName];
            //    }

            //    // thêm cột từ B
            //    newRow["TonKHo"] = XuLyVTUnits.SmartTryParse<float>(item.TonKho);

            //    vattuJoinTonkhoTable.Rows.Add(newRow);
            //}
        }
        private void View_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            // Chỉ xử lý khi Grid đang compare trên cột ChungLoaiVatTu
            if (e.Column == null || e.Column.FieldName != "ChungLoaiVatTu") return;

            // Lấy giá trị Sort tương ứng cho 2 hàng đang so sánh
            object sortObj1 = view.GetListSourceRowCellValue(e.ListSourceRowIndex1, "Sort");
            object sortObj2 = view.GetListSourceRowCellValue(e.ListSourceRowIndex2, "Sort");

            // Try parse to int if possible (thường hợp Sort là numeric string)
            int s1, s2;
            bool p1 = int.TryParse(sortObj1?.ToString(), out s1);
            bool p2 = int.TryParse(sortObj2?.ToString(), out s2);

            if (p1 && p2)
            {
                e.Result = s1.CompareTo(s2);
            }
            else
            {
                // fallback: so sánh string (Ordinal or CurrentCulture tùy bạn)
                e.Result = string.Compare(
                    sortObj1?.ToString() ?? string.Empty,
                    sortObj2?.ToString() ?? string.Empty,
                    StringComparison.Ordinal);
            }

            e.Handled = true;
        }
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit2.EditValue == null) return;
            // ✅ ĐỔ DỮ LIỆU VÀO DICTIONARY
            vattudachon["MaVTID"] = vattuRow["MaVTID"].ToString();
            vattudachon["MaNhomVT"] = vattuRow["MaNhomVT"].ToString();
            vattudachon["MaMauVT"] = vattuRow["MaMauVT"].ToString();
            vattudachon["MaKhoVT"] = vattuRow["MaKhoVT"].ToString();

            slTonKho = XuLyVTUnits.SmartTryParse<decimal>(vattuRow["TonKho"]);

            SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            try
            {
                resetVatTuDaChon();
                loadVatTuDaChon();
                loadSLTonKho();
                getCanDoiVTTable();
                calcCanDoiNPL(false);
                loadGridControl1();
            }
            finally
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
        }
        private void searchLookUpEdit2_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            GridView view = searchLookUpEdit2.Properties.View;

            int rowHandle = view.GetSelectedRows().FirstOrDefault();
            if (rowHandle < 0)
                return;

            DataRow row = view.GetDataRow(rowHandle);
            if (row == null)
                return;

            // ✅ GIỮ ROW
            vattuRow = row;
        }
        private void searchLookUpEdit2View_EndGrouping(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            view.ExpandAllGroups();
        }
        private void searchLookUpEdit2View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

        }
        private void searchLookUpEdit2View_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            e.Appearance.ForeColor = Color.Black;
            e.Appearance.BackColor = Color.LightGray;

            e.Painter.DrawObject(e.Info);
            e.Handled = true;
        }
        private void searchLookUpEdit2_QueryPopUp(object sender, CancelEventArgs e)
        {
            SearchLookUpEdit slup = sender as SearchLookUpEdit;
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int desiredWidth = Math.Max(500, (int)(screenWidth * 0.7));
            slup.Properties.PopupFormWidth = desiredWidth;
        }
        private void searchLookUpEdit2View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }



        // thông tin vật tư đã chọn
        private void loadVatTuDaChon()
        {
            if (vattuRow == null) return;
            textEdit5.Text = vattuRow["ChungLoaiVatTu"].ToString();
            textEdit3.Text = vattuRow["CodeMau"].ToString() + " - " + vattuRow["MauVT"].ToString();
            textEdit4.Text = vattuRow["KhoVai"].ToString();
            textEdit6.Text = vattuRow["TenDVVT"].ToString();
        }
        private void resetVatTuDaChon()
        {
            textEdit5.Text = "";
            textEdit3.Text = "";
            textEdit4.Text = "";
            textEdit6.Text = "";
        }



        // gridcontrol1 - vật tư cân đối
        private void loadGridControl1()
        {
            
            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView1.Columns["SLMuaThem"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["TyLeMuaThem"].OptionsColumn.AllowEdit = true;
            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            textEdit.Mask.EditMask = "n2"; // 2 số thập phân
            textEdit.Mask.UseMaskAsDisplayFormat = true;
            gridView1.Columns["SLNhuCau"].ColumnEdit = textEdit;
            gridView1.Columns["SLCanDoiKho"].ColumnEdit = textEdit;
            gridView1.Columns["SLMuaThem"].ColumnEdit = textEdit;
            gridView1.Columns["TyLeMuaThem"].ColumnEdit = textEdit;
            gridView1.Columns["SLMuaThemTong"].ColumnEdit = textEdit;
            gridView1.Columns["SLTonKhoSauCanDoi"].ColumnEdit = textEdit;
            
            //gridView1.OptionsView.ShowFooter = true;
            //gridView1.Columns["SLCapPhat"].Summary.Add(DevExpress.Data.SummaryItemType.Sum, "SLCapPhat", "{0:n3}");
            //gridView1.Columns["SLCanDoi"].Summary.Add(DevExpress.Data.SummaryItemType.Sum, "SLCanDoi", "{0:n3}");
            //gridView1.Columns["SLMuaThem"].Summary.Add(DevExpress.Data.SummaryItemType.Sum, "SLMuaThem", "{0:n3}");
            gridView1.ExpandAllGroups();
        }
        private void getCanDoiVTTable()
        {
            if (seacrhLookupEdit1CheckedRows == null || seacrhLookupEdit1CheckedRows.Count <= 0) return;
            if (vattuRow == null) return;
            
            try
            {
                var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
                request.Action = "getdonhangcancandoi";

                if (seacrhLookupEdit1CheckedRows != null && seacrhLookupEdit1CheckedRows.Count > 0)
                {
                    foreach (DataRow row in seacrhLookupEdit1CheckedRows)
                    {
                        DataRow newRow = request.TypeTable.NewRow();
                        newRow["MaDH"] = row["MaDH"];
                        request.TypeTable.Rows.Add(newRow);
                    }
                }

                DataRow reqRow = request.TypeTable.NewRow();
                reqRow["MaVTID"] = vattuRow["MaVTID"].ToString();
                reqRow["MaNhomVT"] = vattuRow["MaNhomVT"].ToString();
                reqRow["MaMauVT"] = vattuRow["MaMauVT"].ToString();
                reqRow["MaKhoVT"] = vattuRow["MaKhoVT"].ToString();

                request.TypeTable.Rows.Add(reqRow);
                string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                candoiVTTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

                if (!candoiVTTable.Columns.Contains("generatedID"))
                    candoiVTTable.Columns.Add("generatedID");
                if (!candoiVTTable.Columns.Contains("SLNhuCau"))
                    candoiVTTable.Columns.Add("SLNhuCau", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("SLCanDoiKho"))
                    candoiVTTable.Columns.Add("SLCanDoiKho", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("SLTonKhoSauCanDoi"))
                    candoiVTTable.Columns.Add("SLTonKhoSauCanDoi", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("SLMuaThem"))
                    candoiVTTable.Columns.Add("SLMuaThem", typeof(decimal));
                if (!candoiVTTable.Columns.Contains("TyLeMuaThem"))
                    candoiVTTable.Columns.Add("TyLeMuaThem", typeof(decimal));
                foreach (DataRow row in candoiVTTable.Rows)
                {
                    row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
                }
            }
            catch (Exception ex)
            {
                
            }
            finally
            {

            }
        }
        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            e.Appearance.ForeColor = Color.Black;
            e.Appearance.BackColor = Color.LightGray;

            e.Painter.DrawObject(e.Info);
            e.Handled = true;
            
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "SLMuaThemTong" && e.IsGetData)
            {
                DataRow row = ((DataRowView)e.Row).Row;
                decimal slMuaThemTong = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString()) *
                    (1 + XuLyVTUnits.SmartTryParse<decimal>(row["TyLeMuaThem"].ToString()) / 100m);
                e.Value = slMuaThemTong;//a * (1 + b / 100m);
            }
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "TyLeMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThemTong")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLCanDoiKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }                
            }
            else if (e.Column.FieldName == "SLMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }

            }
        }



        // gridcontrol2 - danh sách đơn hàng
        private void loadGridControl2()
        {

            gridControl2.DataSource = gridControl2Table;
            foreach (GridColumn col in gridView2.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }

            //RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            //textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            //textEdit.Mask.EditMask = "n2"; // 2 số thập phân
            //textEdit.Mask.UseMaskAsDisplayFormat = true;

            gridView2.ExpandAllGroups();
        }
        private void getDonHangDaChon()
        {
            try
            {
                var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
                request.Action = "getdanhsachdonhang";
                if (seacrhLookupEdit1CheckedRows != null && seacrhLookupEdit1CheckedRows.Count > 0)
                {
                    foreach (DataRow row in seacrhLookupEdit1CheckedRows)
                    {
                        DataRow newRow = request.TypeTable.NewRow();
                        newRow["MaDH"] = row["MaDH"];
                        request.TypeTable.Rows.Add(newRow);
                    }
                }

                string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                donhangdachonTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                
            }
        }
        private void calcDonHangDaChon()
        {
            DataTable dt = donhangdachonTable.Copy();
            gridControl2Table = donhangdachonTable.Clone();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            foreach (DataRow row in dt.Rows)
            {

            }
            gridControl2Table = dt.Copy();
        }


        // sl tồn kho
        private void loadSLTonKho()
        {
            slTonKhoText.EditValue = slTonKho;
        }
        private void getSLTonKho()
        {
            try
            {
                
                var request = XuLyVTRequestGet.createDefault();
                request.Action = "gettonkho";
                request.Parameter1 = "all";
                string urlGetListDataTable = URL + "CanDoiNPL/Get";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                tonkhoTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            }
            catch(Exception ex)
            {
               
            }
            finally
            {
                
            }
            
        }



        // lưu
        private void saveBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!checkCanDoiNPL()) return;
            DialogResult result = XtraMessageBox.Show(
                "Bạn có chắc chắn muốn lưu dữ liệu này không?",
                "Xác nhận lưu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes)
                return;
            try
            {
                if (isUpdate) updateCanDoiNPL();
                else saveCanDoiNPL();
                clsWaitForm.ShowSuccessForm(this, 2000);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void saveCanDoiNPL()
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "createtophieu";

            string key = XuLyVTUnits.generatedTimeKey("group");
            int sttdh = 1;

            foreach (DataRow gridRow in dt.Rows)
            {
                DataRow row = request.TypeTable.NewRow();

                foreach (DataColumn col in request.TypeTable.Columns)
                {
                    if (dt.Columns.Contains(col.ColumnName))
                    {
                        row[col.ColumnName] = gridRow[col.ColumnName];
                    }
                    else
                    {
                        // Nếu muốn set default cho cột không có ở bảng A
                        row[col.ColumnName] = DBNull.Value;
                    }
                }
                row["MaPhieu"] = key;
                row["IsDuyet"] = 0;
                row["NguoiTao"] = GlobleData.UserName;
                row["NgayTao"] = DateTime.Now;
                row["STTDH"] = sttdh;
                sttdh++;
                request.TypeTable.Rows.Add(row);
            }
            string urlGetListDataTable = URL + "CanDoiNPL/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            if (response != "OK")
            {
                throw new ApplicationException(response);
            }
            DateTime? selectedDateTime = ngayDBDKEdit.EditValue as DateTime?;
            if (selectedDateTime.HasValue)
            {
                var update = XuLyVTRequestPost.createDefault("CanDoiNPL");
                update.Action = "updatengaydongbo";
                
                foreach(DataRow gridRow in dt.Rows)
                {
                    DataRow row = update.TypeTable.NewRow();
                    row["MaDH"] = gridRow["MaDH"];
                    update.TypeTable.Rows.Add(row);
                }
                update.Parameter6 = selectedDateTime.Value;
                response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, update); }).Result;
            }
        }
        private void updateCanDoiNPL()
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "updatephieucandoi";
            int sttdh = 1;
            foreach (DataRow gridRow in dt.Rows)
            {
                DataRow row = request.TypeTable.NewRow();

                foreach (DataColumn col in request.TypeTable.Columns)
                {
                    if (dt.Columns.Contains(col.ColumnName))
                    {
                        row[col.ColumnName] = gridRow[col.ColumnName];
                    }
                    else
                    {
                        // Nếu muốn set default cho cột không có ở bảng A
                        row[col.ColumnName] = DBNull.Value;
                    }
                }
                row["NguoiSua"] = GlobleData.UserName;
                row["NgaySua"] = DateTime.Now;
                row["STTDH"] = sttdh;
                sttdh++;
                request.TypeTable.Rows.Add(row);
            }
            string urlGetListDataTable = URL + "CanDoiNPL/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DateTime? selectedDateTime = ngayDBDKEdit.EditValue as DateTime?;
            if (selectedDateTime.HasValue)
            {
                var update = XuLyVTRequestPost.createDefault("CanDoiNPL");
                update.Action = "updatengaydongbo";

                foreach (DataRow gridRow in dt.Rows)
                {
                    DataRow row = update.TypeTable.NewRow();
                    row["MaDH"] = gridRow["MaDH"];
                    update.TypeTable.Rows.Add(row);
                }
                update.Parameter6 = selectedDateTime.Value;
                response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, update); }).Result;
            }
        }
        private bool checkCanDoiNPL()
        {
            if (isUpdate) return true;

            DataTable changes = gridControl1Table.GetChanges();
            
            if (changes != null)
            {
                foreach (DataRow row in changes.Rows)
                {
                    if (row.RowState == DataRowState.Modified)
                    {
                        decimal slTonKho = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho", DataRowVersion.Original].ToString());
                        decimal slCanDoiKho = XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho", DataRowVersion.Original].ToString());
                        decimal slMuaThem = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem", DataRowVersion.Original].ToString());

                        decimal slTonKhoMoi = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho", DataRowVersion.Current].ToString());
                        decimal slCanDoiKhoMoi = XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho", DataRowVersion.Current].ToString());
                        decimal slMuaThemMoi = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem", DataRowVersion.Current].ToString());

                        if (slCanDoiKhoMoi + slTonKhoMoi > slCanDoiKho + slTonKho)
                        {
                            XtraMessageBox.Show("Số lượng cân đối từ tồn kho không hợp lệ!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        // nút up down
        private void upBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle > 0) // không phải dòng đầu tiên
            {
                DataRow row = gridView1.GetDataRow(rowHandle);
                DataRow prevRow = gridView1.GetDataRow(rowHandle - 1);

                // hoán đổi dữ liệu
                object[] temp = row.ItemArray;
                row.ItemArray = prevRow.ItemArray;
                prevRow.ItemArray = temp;

                // focus lại vào dòng vừa di chuyển
                gridView1.FocusedRowHandle = rowHandle - 1;
            }
            slTonKho = XuLyVTUnits.SmartTryParse<decimal>(slTonKhoText.EditValue.ToString());
            calcCanDoiNPL(true);
        }
        private void downBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle < gridView1.RowCount - 1) // không phải dòng cuối
            {
                DataRow row = gridView1.GetDataRow(rowHandle);
                DataRow nextRow = gridView1.GetDataRow(rowHandle + 1);

                // hoán đổi dữ liệu
                object[] temp = row.ItemArray;
                row.ItemArray = nextRow.ItemArray;
                nextRow.ItemArray = temp;

                // focus lại vào dòng vừa di chuyển
                gridView1.FocusedRowHandle = rowHandle + 1;
            }
            slTonKho = XuLyVTUnits.SmartTryParse<decimal>(slTonKhoText.EditValue.ToString());
            calcCanDoiNPL(true);
        }
        private void calcCanDoiNPL(bool updown)
        {
            if (updown) gridControl1Table = gridControl1.DataSource as DataTable;
            else gridControl1Table = candoiVTTable.Copy();
            if (gridControl1Table == null || gridControl1Table.Columns.Count <= 0 || gridControl1Table.Rows.Count <= 0) return;
            if (isUpdate)
            {

            }
            else
            {
                foreach (DataRow row in gridControl1Table.Rows)
                {
                    //if (row["DinhMucHaoHut"] == DBNull.Value || row["DinhMucHaoHut"].ToString() == "") row["DinhMucHaoHut"] = 0;
                    //if (row["DinhMucChung"] == DBNull.Value || row["DinhMucChung"].ToString() == "") row["DinhMucChung"] = 0;
                    //float slCapPhat = XuLyVTUnits.SmartTryParse<float>(row["DinhMucChung"].ToString()) *
                    //    XuLyVTUnits.SmartTryParse<float>(row["SoLuong"].ToString()) *
                    //    (1 + XuLyVTUnits.SmartTryParse<float>(row["DinhMucHaoHut"].ToString()) / 100);
                    decimal slCanDoi = 0;
                    decimal slCapPhat = XuLyVTUnits.SmartTryParse<decimal>(row["SLNhuCau"].ToString());
                    slCapPhat = (decimal)Math.Round(slCapPhat, 2, MidpointRounding.AwayFromZero);
                    decimal slMuaThem = 0;
                    if (slTonKho > 0)
                    {
                        if (slTonKho >= slCapPhat)
                        {
                            slCanDoi = slCapPhat;
                        }
                        else
                        {
                            slCanDoi = slTonKho;
                            slMuaThem = slCapPhat - slCanDoi;
                        }
                        slTonKho = slTonKho - slCanDoi;
                    }
                    else
                    {
                        slMuaThem = slCapPhat;
                    }

                    row["SLNhuCau"] = slCapPhat;
                    row["SLCanDoiKho"] = slCanDoi;
                    row["SLTonKhoSauCanDoi"] = slTonKho;
                    row["SLMuaThem"] = slMuaThem;

                    
                }
            }
        }


        private void gridView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (lstFormatFieldName.Contains(e.Column.FieldName))
            {
                if (e.Value != null && e.Value is float || e.Value is double)
                {
                    float value = Convert.ToSingle(e.Value);

                    if (value == Math.Floor(value))
                    {
                        e.DisplayText = value.ToString("0");
                    }
                    else
                    {
                        e.DisplayText = value.ToString("0.###");
                    }

                }
            }
        }
        private void naplaiBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            calcCanDoiNPL(false);
            loadGridControl1();
        }

        
    }
}