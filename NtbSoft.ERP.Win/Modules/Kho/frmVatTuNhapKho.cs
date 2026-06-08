using DevExpress.Data;
using DevExpress.DataAccess.Excel;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Properties;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmVatTuNhapKho : DevExpress.XtraEditors.XtraForm
    {
        // 1 pixel = (25,4/300) mm 

        // 50x100 mm

        // padding : 9x3x10x3
        //2x5 phiếu /1A4
        SearchCheckSelection gridCheckMarks_VatTu;

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();

        DataTable tblBOM, tblVT, tblSave, tblTenVT, tblNhapKho, tblVatTuChiTiet, tblKhachHang, tblMaKho, tblViTri;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        private string _mahang = string.Empty, nhomsize = string.Empty, _mavt = string.Empty, _MaVatTu = string.Empty;
        bool indicatorIcon = true;
        int dotnhap = -1;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        string _nguoiTao = string.Empty;
        List<string> _lstBarcode = new List<string>();
        DataTable dtNgayNhap, dtDotNhap, dtNguoiNhap;
        Random _random = new Random();
        public frmVatTuNhapKho()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            _clientExtension = new HttpClientExtension();
            tblBOM = new DataTable();
            tblVT = new DataTable();
            tblSave = new DataTable();
            tblTenVT = new DataTable();
            tblNhapKho = new DataTable();
            tblVatTuChiTiet = new DataTable();
            tblKhachHang = new DataTable();
            tblMaKho = new DataTable();
            tblViTri = new DataTable();
            _lstBarcode = new List<string>();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            dtNgayNhap = new DataTable();
            dtDotNhap = new DataTable();
            dtNguoiNhap = new DataTable();
            //_allowAdd = allowAdd;
            //_allowEdit = allowEdit;
            //_allowDelete = allowDelete;
            //this._nguoiTao = nguoitao;
        }

        private void BtLoad()
        {
            try
            {
                CreateSearchLookup();
                CreateSearchLookupKhachHang();
                CreateSearchLookupMaKho();
                CreateDateEditNgayNhap();
                CreateNewDataTable();
                CreateSearchLookupViTri();
                CreateTableSave();
                CreateRepoSoLuong();
                loadBarcode();
                //creatTable_BOM();
                loadtblVatTuChiTiet();
               
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại1.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        protected override void OnLoad(EventArgs e)
        {
            BtLoad();
            //CheckPerminsion();
            //keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        #region hàm load 

        private void loadLoc()
        {
            DataTable dt = gCNhapKho.DataSource as DataTable;
            if (dt == null) return;
            if (dt.Rows.Count == 0) return;
            List<string> lstDotNhap = dt.AsEnumerable()
           .Select(row => row.Field<string>("DotNhap"))
           .Distinct()
           .ToList();
            List<string>  lstNgayNhap = dt.AsEnumerable()
            .Select(row => row.Field<string>("NgayNhap"))
            .Distinct()
            .ToList();
            List<string>  lstNguoiNhap = dt.AsEnumerable()
            .Select(row => row.Field<string>("NguoiNhap"))
            .Distinct()
            .ToList();
            dtDotNhap.Clear();
            dtNgayNhap.Clear();
            dtNguoiNhap.Clear();
            if (!dtDotNhap.Columns.Contains("DotNhap"))
            {
                dtDotNhap.Columns.Add("DotNhap", typeof(string));
            }

            // Kiểm tra và thêm cột "NgayNhap" nếu chưa tồn tại
            if (!dtNgayNhap.Columns.Contains("NgayNhap"))
            {
                dtNgayNhap.Columns.Add("NgayNhap", typeof(string));
            }

            // Kiểm tra và thêm cột "NguoiNhap" nếu chưa tồn tại
            if (!dtNguoiNhap.Columns.Contains("NguoiNhap"))
            {
                dtNguoiNhap.Columns.Add("NguoiNhap", typeof(string));
            }
            if(lstDotNhap!=null)
            {
                foreach (var dotNhap in lstDotNhap)
                {
                    dtDotNhap.Rows.Add(dotNhap);
                }
            }    
           
            if (lstNgayNhap != null)
            {
                foreach (var ngayNhap in lstNgayNhap)
                {
                    dtNgayNhap.Rows.Add(ConvertDateFormat(ngayNhap));
                }
            }
           
            if (lstNguoiNhap != null)
            {
                foreach (var nguoiNhap in lstNguoiNhap)
                {
                    dtNguoiNhap.Rows.Add(nguoiNhap);
                }
            }
           


            searchLocDot.Properties.DataSource = dtDotNhap;
            searchLocDot.Properties.ValueMember = "DotNhap";
            searchLocDot.Properties.DisplayMember = "DotNhap";

            searchLocNgay.Properties.DataSource = dtNgayNhap;
            searchLocNgay.Properties.ValueMember = "NgayNhap";
            searchLocNgay.Properties.DisplayMember = "NgayNhap";

            searchLocNguoi.Properties.DataSource = dtNguoiNhap;
            searchLocNguoi.Properties.ValueMember = "NguoiNhap";
            searchLocNguoi.Properties.DisplayMember = "NguoiNhap";

        }
        #endregion
        private void CreateRepoSoLuong()
        {
            RepositoryItemTextEdit repo = new RepositoryItemTextEdit();
            repo.KeyPress += RepositoryItemTextEditSoLuong_KeyPress;
            colSoLuongNK.ColumnEdit = repo;
        }
        private void CreateDateEditNgayNhap()
        {
            RepositoryItemDateEdit dateEdit = new RepositoryItemDateEdit();

            // Đặt định dạng ngày
            dateEdit.DisplayFormat.FormatString = "dd-MM-yyyy";
            dateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dateEdit.EditFormat.FormatString = "dd-MM-yyyy";
            dateEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dateEdit.Mask.EditMask = "dd-MM-yyyy";
            dateEdit.Mask.UseMaskAsDisplayFormat = true;
            colNgayNhapNK.ColumnEdit = dateEdit;
        }

        private void loadtblVatTuChiTiet()
        {
            string url = string.Format("{0}?", URL + "KhoVatTu/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblVatTuChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
        }

        private void LoadDataSource()
        {
            gCVatTu.RefreshDataSource();

            gCVatTu.DataSource = tblBOM;
            DataRow row = gVVatTu.GetFocusedDataRow();

            if (row != null)
            {
                setTxtValueVatTu(row);
                loadVatTuNhapKho(row["MaVatTu"].ToString());
            }
            else
            {
                setTxtVatTuNull();
            }
        }
        private void CreateSearchLookup()
        {

            string urlVT = string.Format("{0}?", URL + "BOM/GET_TenVatTu");
            string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
            if(jsonVT!="[]")
            {
                tblTenVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);

            }

            string urlHH = string.Format("{0}?", URL + "BOM/GETMH");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = null;
            if (jsonHH!="[]")
            {
                tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            }    
            
            searchLookUpEdit_MH.Properties.DataSource = tblHH;
            searchLookUpEdit_MH.Properties.ValueMember = "MaHang";
            searchLookUpEdit_MH.Properties.DisplayMember = "TenHang";
        }
        private void CreateSearchLookupViTri()
        {

            string urlViTri = string.Format("{0}?", URL + "VatTuNhapKho/GetViTri");
            string jsonViTri = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlViTri); }).Result;
            if (jsonViTri == "[]") return;
            tblViTri = JsonConvert.DeserializeObject<DataTable>(jsonViTri);


            searchLookUpEditViTri.Properties.DataSource = tblViTri;
            searchLookUpEditViTri.Properties.ValueMember = "MaDisplay";
            searchLookUpEditViTri.Properties.DisplayMember = "Display";


            RepositoryItemSearchLookUpEdit repositorySearchLookupViTri = new RepositoryItemSearchLookUpEdit();
            repositorySearchLookupViTri.DataSource = tblViTri;
            repositorySearchLookupViTri.DisplayMember = "TenO";
            repositorySearchLookupViTri.ValueMember = "MaDisplay";
            repositorySearchLookupViTri.ShowClearButton = false;
            repositorySearchLookupViTri.NullText = "[Chọn giá trị]";

            GridView dvView = repositorySearchLookupViTri.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TenO", Caption = "Tên ô", Name = "colTenOSearch", Visible = true, Width = 50 });
                dvView.Columns.Add(new GridColumn { FieldName = "MaDisplay", Caption = "Mã display", Name = "colMaDisplaySearch", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "Display", Caption = "Vị trí", Name = "colDisplaySearch", Visible = true });

            }
            colMaViTriNK.ColumnEdit = repositorySearchLookupViTri;
        }
        private void CreateSearchLookupMaKho()
        {

            string urlMaKho = string.Format("{0}?", URL + "VatTuNhapKho/GetMaKho");
            string jsonMaKho = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMaKho); }).Result;
            if (jsonMaKho == "[]") return;
            tblMaKho = JsonConvert.DeserializeObject<DataTable>(jsonMaKho);


            searchLookUpEditMaKho.Properties.DataSource = tblMaKho;
            searchLookUpEditMaKho.Properties.ValueMember = "MaKho";
            searchLookUpEditMaKho.Properties.DisplayMember = "TenDVSX";


            RepositoryItemSearchLookUpEdit repositorySearchLookupMakho = new RepositoryItemSearchLookUpEdit();
            repositorySearchLookupMakho.DataSource = tblMaKho;
            repositorySearchLookupMakho.DisplayMember = "TenDVSX";
            repositorySearchLookupMakho.ValueMember = "MaKho";
            repositorySearchLookupMakho.ShowClearButton = false;
            repositorySearchLookupMakho.NullText = "[Chọn giá trị]";
            
            GridView dvView = repositorySearchLookupMakho.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKho", Caption = "Mã kho", Name = "colMaKhoSearch", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDVSX", Caption = "Tên kho", Name = "colTenKhoSearch", Visible = true });

            }
            colMaKhoNK.ColumnEdit = repositorySearchLookupMakho;
        }
        private void CreateSearchLookupKhachHang()
        {
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblKhachHang = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditKH.Properties.DataSource = tbl;
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            RepositoryItemSearchLookUpEdit repositorySearchLookupKhachHang = new RepositoryItemSearchLookUpEdit();
            repositorySearchLookupKhachHang.DataSource = tbl;
            repositorySearchLookupKhachHang.DisplayMember = "TenKH";
            repositorySearchLookupKhachHang.ValueMember = "MaKH";
            repositorySearchLookupKhachHang.ShowClearButton = false;
            repositorySearchLookupKhachHang.NullText = "[Chọn giá trị]";

            GridView dvView = repositorySearchLookupKhachHang.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKHSearch", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Tên khách hàng", Name = "colTenKHSearch", Visible = true });

            }
            colMaDTKHNK.ColumnEdit = repositorySearchLookupKhachHang;
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }

        private void BtSave()
        {
            try
            {

                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                if (tblNhapKho.Rows.Count > 0)
                {
                    tblSave.Clear();
                    DataRow row = gVVatTu.GetFocusedDataRow();
                    if (row == null)
                    {
                        return;
                    }
                    string _maVT = row["MaVatTu"]?.ToString();

                    loadDotNhap(_maVT);
                    //DataTable tblSource = gCNhapKho.DataSource as DataTable;
                    for (int i = 0; i < tblNhapKho.Rows.Count; i++)
                    {
                        if (tblNhapKho.Rows[i]["XacNhan"].ToString() != "True")
                        {
                            continue;
                        }
                        Random random = new Random();
                        string uniqueCode = /*DateTime.UtcNow.Ticks.ToString() +*/"|" + random.Next(0, 99999);


                        DataRow _dr = tblSave.NewRow();
                        if (Convert.ToInt32(tblNhapKho.Rows[i]["ID"]) != 0)
                        {
                            var b = tblNhapKho.Rows[i]["ID"];
                            var a = _dr["ID"];

                            _dr["ID"] = tblNhapKho.Rows[i]["ID"];

                        }
                        else
                        {
                            _dr["ID"] = 0;
                        }

                        _dr["MaVTNhapKho"] = tblNhapKho.Rows[i]["MaVTNhapKho"];
                        _dr["MaDH"] = tblNhapKho.Rows[i]["MaDH"];
                        _dr["Barcode"] = tblNhapKho.Rows[i]["Barcode"];
                        _dr["MaVT"] = tblNhapKho.Rows[i]["MaVT"];
                        _dr["TenVT"] = tblNhapKho.Rows[i]["TenVT"];
                        _dr["MaVTMau"] = tblNhapKho.Rows[i]["MaVTMau"];
                        _dr["SoLuong"] = tblNhapKho.Rows[i]["SoLuong"];
                        _dr["MaMau"] = tblNhapKho.Rows[i]["MaMau"];
                        _dr["TenMau"] = tblNhapKho.Rows[i]["TenMau"];
                        _dr["CodeMau"] = tblNhapKho.Rows[i]["CodeMau"];
                        _dr["DVTinh"] = tblNhapKho.Rows[i]["DVTinh"];
                        _dr["MaKhoSize"] = tblNhapKho.Rows[i]["MaKhoSize"];

                        _dr["KhoSize"] = tblNhapKho.Rows[i]["KhoSize"];
                        _dr["TongKien"] = tblNhapKho.Rows[i]["TongKien"];
                        _dr["Kien"] = tblNhapKho.Rows[i]["Kien"];
                        _dr["MaVTSP"] = tblNhapKho.Rows[i]["MaVTSP"];
                        _dr["MaViTri"] = tblNhapKho.Rows[i]["MaViTri"];
                        _dr["MaKho"] = tblNhapKho.Rows[i]["MaKho"];
                        _dr["MaDTKH"] = tblNhapKho.Rows[i]["MaDTKH"];
                        _dr["Lot"] = tblNhapKho.Rows[i]["Lot"];
                        _dr["AnhMau"] = tblNhapKho.Rows[i]["AnhMau"];
                        _dr["LoangMau"] = tblNhapKho.Rows[i]["LoangMau"];
                        _dr["GhiChu"] = tblNhapKho.Rows[i]["GhiChu"];
                        string ngayNhap = Convert.ToDateTime(tblNhapKho.Rows[i]["NgayNhap"]).ToString("yyyy-MM-dd");
                            _dr["NgayNhap"] = ngayNhap;
                        _dr["NguoiNhap"] = tblNhapKho.Rows[i]["NguoiNhap"];
                        _dr["DotNhap"] = tblNhapKho.Rows[i]["DotNhap"];

                        // Thêm dòng vào DataTable
                        tblSave.Rows.Add(_dr);
                    }
                    if (tblSave.Rows.Count == 0&&tblNhapKho.Rows.Count==0)
                    {
                        XtraMessageBox.Show("Vui lòng xác nhân nhập kho ít nhất một vật tư!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlSaveBOMDH = string.Format("{0}?", URL + "VatTuNhapKho/Post");
                    string mssSaveBOMDH = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveBOMDH, tblSave); }).Result;
                    if (mssSaveBOMDH.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        tblSave.Clear();
                        tblNhapKho.Clear();
                        LoadDataSource();
                    }
                    else
                    {
                        tblSave.Clear();
                        tblNhapKho.Clear();
                        LoadDataSource();
                        return;
                    }
                    //XtraMessageBox.Show("Luu thành công");
                    /* if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                         DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);*/
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!" + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        //searchLookUpEdit_VatTu_EditValueChanged

       
      
        private Control FindSearchLookUpEditInLayout(Control control)
        {
            // Kiểm tra null
            if (control == null)
                return null;

            // Nếu control hiện tại là SearchLookUpEdit, trả về nó
            if (control is DevExpress.XtraEditors.SearchLookUpEdit)
                return control;

            // Nếu control là LayoutControl, lặp qua các item của nó
            if (control is DevExpress.XtraLayout.LayoutControl layoutControl)
            {
                foreach (var item in layoutControl.Items)
                {
                    if (item is DevExpress.XtraLayout.LayoutControlItem layoutItem && layoutItem.Control != null)
                    {
                        // Tìm SearchLookUpEdit trong LayoutControlItem
                        var result = FindSearchLookUpEditInLayout(layoutItem.Control);
                        if (result != null)
                            return result;
                    }
                }
            }

            // Nếu control có ActiveControl, tiếp tục tìm
            if (control is ContainerControl containerControl && containerControl.ActiveControl != null)
            {
                return FindSearchLookUpEditInLayout(containerControl.ActiveControl);
            }

            // Không tìm thấy
            return null;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

            BtSave();

        }

        private void btn_TimKiem_Click(object sender, EventArgs e)
        {
            TimKiem();
        }
        private void TimKiem()
        {
            string urlTK = string.Format("{0}?para2={1}", URL + "VatTuNhapKho/GetTimKiem", _mahang);
            string jsonTK = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTK); }).Result;

            tblBOM = JsonConvert.DeserializeObject<DataTable>(jsonTK);

            if (tblBOM.Rows.Count == 0)
            {
                //creatTable_BOM();
            }

            for (int i = 0; i < tblBOM.Rows.Count; i++)
            {
                if (!_mavt.Contains(tblBOM.Rows[i]["MaVTMau"].ToString()))
                {
                    _mavt = _mavt != "" ? _mavt + ";" + tblBOM.Rows[i]["MaVTMau"].ToString() : tblBOM.Rows[i]["MaVTMau"].ToString();
                }
                if (!_MaVatTu.Contains(tblBOM.Rows[i]["MaVatTu"].ToString()))
                {
                    _MaVatTu = _MaVatTu != "" ? _MaVatTu + ";" + tblBOM.Rows[i]["MaVatTu"].ToString() : tblBOM.Rows[i]["MaVatTu"].ToString();
                }
            }

            LoadDataSource();
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            DELETE();
        }
        private void DELETE()
        {
            DataRow row = gVNhapKho.GetFocusedDataRow();
            if (row != null)
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa vật tư nhập kho này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {


                    string _id = row["ID"].ToString();
                    if (_id == "0")
                    {
                        int focusedRowHandle = gVVatTu.FocusedRowHandle; // Lấy chỉ số dòng đang chọn
                        if (focusedRowHandle >= 0) // Đảm bảo chỉ số hợp lệ
                        {
                            gVNhapKho.DeleteRow(focusedRowHandle); // Xóa dòng
                        }
                    }
                    else
                    {

                        int _ID = int.TryParse(_id, out var result) ? result : 0;

                        string url = string.Format("{0}?id={1}", URL + "VatTuNhapKho/Delete", _ID);
                        string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

                        //TimKiem();
                    }

                    LoadDataSource();
                }
            }
            }

            private void colBtn_Xoa_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DataRow row = gVVatTu.GetFocusedDataRow();

            string _idDeleteRow = row["ID_DM"].ToString();
            DELETE();
        }
   

      /*  private int FindRowIndexByName(DataTable _dtTemp, string name)
        {
            // Sử dụng LINQ để ìm index của hàng
            var rowIndex = _dtTemp.AsEnumerable()
                .Select((row, index) => new { Row = row, Index = index })
                .FirstOrDefault(item => item.Row["MaVtMau"].ToString() == name)?.Index ?? -1;
        
            return rowIndex;
        }*/


      

        private void gVVatTu_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow row = gVVatTu.GetFocusedDataRow();
            if (row == null)
            {
                setTxtNhapKhoNull();
                return;
            }
            string _maVT = row["MaVatTu"]?.ToString();
            setTxtValueVatTu(row);
            loadVatTuNhapKho(_maVT);
            loadDotNhap(_maVT);
        }

        private void gVNhapKho_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVVatTu_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVVatTu_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column == colSTT1)
            {
                if (e.RowHandle < 0)
                {
                    e.DisplayText = "0";
                }
                else
                {
                    string sttValue = Convert.ToString(e.RowHandle + 1);
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);

                }
            }
        }
        #region excel nè
        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            string mavattu = string.Empty;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            DataTable dttb = getVatTuNhapKho();
            if (dttb is null || dttb.Rows.Count == 0) return;
            //mavattu = dttb.AsEnumerable().Where(x => x["MaVatTu"] == mavattu.ToString()).FirstOrDefault()["MaVatTu"].ToString();
            //int Size = 0;
            Sfd.FileName = string.Format("VatTuNhapKho" + DateTime.Now.ToString("dd-MM-yyyy"));
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("VatTuNhapKho");
                    ExportExcel(Sfd.FileName, dttb);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                }
            }
        }
        private void ExportExcel(string path, DataTable dtsave)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("VẬT TƯ NHẬP KHO");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    range = worksheet.Cells["B1:L1"]; range.Merge = true; range.Value = "NAM THANH BINH COMPANY"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["B2:L3"]; range.Merge = true; range.Value = "Vật tư nhập kho"; range.Style.Font.Bold = true;
                    dtXuatExcelVTNhapKho(dtsave, worksheet);
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }
        public void dtXuatExcelVTNhapKho(DataTable dtSave, ExcelWorksheet worksheet, bool flagFilter = true)
        {
            ExcelRange range = worksheet.Cells;
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Thiết lập tiêu đề bảng (thêm "Barcode" trước "Mã vật tư")
            string[] headers = { "STT", "Mã đơn hàng", "Barcode", "Mã vật tư", "Tên vật tư", "Tên màu", "Khổ/Size", "Số lượng", "Tổng kiện",
                     "Kiện", "Mã VTSP", "Vị trí", "Kho", "Khách hàng", "LOT", "Ánh màu", "Loang màu","Đợt nhập", "Ghi chú" };

            for (int i = 0; i < headers.Length; i++)
            {
                range = worksheet.Cells[4, i + 1];
                range.Value = headers[i];
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 5; // Bắt đầu từ dòng 5
            int index = 0;

            // Duyệt qua các hàng dữ liệu trong DataTable
            foreach (DataRow rows in dtSave.Rows)
            {
                var _vitri = (from DataRow rowVT in tblViTri.Rows
                                   where rowVT.Field<string>("MaDisplay") == rows["MaViTri"].ToString()
                                   select rowVT.Field<string>("TenO")).FirstOrDefault();
                var _kho = (from DataRow rowKho in tblMaKho.Rows
                              where rowKho.Field<string>("MaKho") == rows["MaKho"].ToString()
                              select rowKho.Field<string>("TenDVSX")).FirstOrDefault();
                var _khachHang = (from DataRow rowVT in tblKhachHang.Rows
                              where rowVT.Field<string>("MaKH") == rows["MaDTKH"].ToString()
                              select rowVT.Field<string>("TenKH")).FirstOrDefault();
                worksheet.Cells[row, 1].Value = index + 1;
                worksheet.Cells[row, 2].Value = rows["MaDH"].ToString();
                worksheet.Cells[row, 3].Value = rows["Barcode"].ToString();  // Thêm Barcode trước MaVT
                worksheet.Cells[row, 4].Value = rows["MaVT"].ToString();
                worksheet.Cells[row, 5].Value = rows["TenVT"].ToString();
                worksheet.Cells[row, 6].Value = rows["TenMau"].ToString();
                worksheet.Cells[row, 7].Value = rows["KhoSize"].ToString();
                worksheet.Cells[row, 8].Value = rows["SoLuong"].ToString();
                worksheet.Cells[row, 9].Value = rows["TongKien"].ToString();
                worksheet.Cells[row, 10].Value = rows["Kien"].ToString();
                worksheet.Cells[row, 11].Value = rows["MaVTSP"].ToString();
                worksheet.Cells[row, 12].Value = _vitri;
                worksheet.Cells[row, 13].Value = _kho;
                worksheet.Cells[row, 14].Value = _khachHang;
                worksheet.Cells[row, 15].Value = rows["Lot"].ToString();
                worksheet.Cells[row, 16].Value = rows["AnhMau"].ToString();
                worksheet.Cells[row, 17].Value = rows["LoangMau"].ToString();

                worksheet.Cells[row, 18].Value = rows["DotNhap"].ToString();
                    worksheet.Cells[row, 19].Value = rows["GhiChu"].ToString();

                // Tô màu nền cho dòng chẵn/lẻ
                if (index % 2 == 0)
                {
                    worksheet.Cells[row, 1, row, 19].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 19].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                }

                // Căn chỉnh dữ liệu
                worksheet.Cells[row, 1, row, 19].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                index++;
                row++;
            }

            // Thêm border cho toàn bộ bảng
            int totalRows = dtSave.Rows.Count + 4;
            var tableRange = worksheet.Cells[4, 1, totalRows, 19]; // Cập nhật số cột cuối cùng
            tableRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            // Đặt màu cho border
            tableRange.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);

            // Tự động điều chỉnh độ rộng các cột
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void btnXuatExcelMau_Click(object sender, EventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("NhapKhoVatTu{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateVatTuNhapKho.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName);

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Mở File", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(Sfd.FileName))
                            System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                    catch
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Error..");
                    }
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }
        public void Export(string TemplateFileName, string ExportFileName)
        {
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        // đặt tên người tạo file
                        excelPackage.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        excelPackage.Workbook.Properties.Title = "VatTuNhapKho";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);


                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                object misValue = System.Reflection.Missing.Value;
                throw new Exception(ex.Message);
            }


        }

        private void btnNhapExcel_Click(object sender, EventArgs e)
        {
            if (_mahang == string.Empty)
            {
                XtraMessageBox.Show("Vui lòng chọn đơn hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string mavattu = string.Empty;
            DataRow row = gVVatTu.GetFocusedDataRow();
            if (row != null)
            {
                mavattu = row["MaVatTu"].ToString();
            }
            if (mavattu == string.Empty)
            {
                XtraMessageBox.Show("Chưa có vật tư. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataTable _excel = ImportExcel(row, mavattu);
            if (_excel != null)
            {
                frmNhapVatTuNhapKho frm = new frmNhapVatTuNhapKho(_excel, row);
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
            }
            loadVatTuNhapKho(mavattu);
        }

        private DataTable ImportExcel(DataRow row, string mavattu)
        {
            try
            {
                loadDotNhap(mavattu);
                // Hiển thị hộp thoại để chọn file Excel
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";
                openFileDialog1.Title = "Import Excel";
                openFileDialog1.Multiselect = false;
                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult != DialogResult.OK) return null;

                // Lấy đường dẫn file và kiểm tra
                string filePath = openFileDialog1.FileName;
                if (string.IsNullOrEmpty(filePath)) return null;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                string firstSheetName;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    firstSheetName = package.Workbook.Worksheets[0].Name; 
                }
              
                string range = "$A2:ZZ500";    

               
                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = filePath;
            
                var worksheetSettings = new ExcelWorksheetSettings(firstSheetName, range);
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill(); 

                DataTable dt = source.ToDataTable();

                // Tạo DataTable để lưu dữ liệu đã xử lý
                tblSave.Clear(); CreateTableSave();
                DataTable dtSave = tblSave;

                // Duyệt qua từng dòng trong DataTable và kiểm tra dữ liệu
                for (int i = 0; i < dt.Rows.Count; i++) // Không cần bỏ qua dòng đầu tiên vì range đã bắt đầu từ A2
                {
                    bool isValidRow =
                        !string.IsNullOrWhiteSpace(dt.Rows[i][1]?.ToString()) && // MaKeToan
                        !string.IsNullOrWhiteSpace(dt.Rows[i][2]?.ToString()) && // MaKho
                        !string.IsNullOrWhiteSpace(dt.Rows[i][3]?.ToString());  // DVTinh

                    // Nếu dòng không hợp lệ, bỏ qua
                    if (!isValidRow)
                    {
                        continue;
                    }
                    string _mavitri = string.Empty;
                    foreach (DataRow rowvt in tblViTri.Rows)
                    {
                        if (rowvt["TenO"].ToString() == dt.Rows[i][4].ToString())
                        {
                            _mavitri = rowvt["MaDisplay"].ToString();
                            break; // Thoát khỏi vòng lặp khi tìm thấy
                        }
                    }
                    var drnew = dtSave.NewRow();
                    var maKhachHang = (from DataRow rowKH in tblKhachHang.Rows
                                       where rowKH.Field<string>("TenKH") == dt.Rows[i][6].ToString()
                                       select rowKH.Field<string>("MaKH")).FirstOrDefault();
                    drnew["ID"] = 0;
                    drnew["MaVTNhapKho"] = "";
                    drnew["MaDH"] = _mahang;
                    drnew["Barcode"] = CreateValueBarcode(dt.Rows[i][2].ToString(), dt.Rows[i][5].ToString());
                    drnew["MaVT"] = mavattu;
                    drnew["TenVT"] = row["TenVatTu"].ToString();
                    drnew["MaVTMau"] = row["MaVTMau"].ToString();
                    drnew["SoLuong"] = dt.Rows[i][1].ToString() == "" ? 0 : float.Parse(dt.Rows[i][1].ToString());
                    drnew["MaMau"] = "";
                    drnew["TenMau"] = row["Mau"].ToString();
                    drnew["CodeMau"] = "";
                    drnew["MaDVTinh"] = "";
                    drnew["DVTinh"] = row["DonViTinh"];
                    drnew["MaKhoSize"] = "";
                    drnew["KhoSize"] = row["KhoVai"].ToString();
                    drnew["TongKien"] = dt.Rows[i][2].ToString();
                    drnew["Kien"] = dt.Rows[i][3].ToString();
                    drnew["MaVTSP"] = mavattu;
                    drnew["MaViTri"] = _mavitri != null ? _mavitri : dt.Rows[i][4].ToString();
                    drnew["MaKho"] = dt.Rows[i][5].ToString();
                    drnew["MaDTKH"] = maKhachHang != null ? maKhachHang.ToString() : dt.Rows[i][6].ToString();
                    drnew["Lot"] = dt.Rows[i][7].ToString();
                    drnew["AnhMau"] = dt.Rows[i][8].ToString();
                    drnew["LoangMau"] = dt.Rows[i][9].ToString();
                    drnew["GhiChu"] = dt.Rows[i][10].ToString();
                    drnew["NgayNhap"] = DateTime.Now.ToString("yyyy-MM-dd");
                    drnew["NguoiNhap"] = GlobleData.UserName;
                    drnew["DotNhap"] = dotnhap;
                    if (tblVatTuChiTiet.Rows.Count > 0)
                    {
                        foreach (DataRow chiTietRow in tblVatTuChiTiet.Rows)
                        {
                            if (ReplaceSpecialCharactersAndRemoveSpaces(chiTietRow["MaVTMau"].ToString()) == ReplaceSpecialCharactersAndRemoveSpaces(drnew["MaVTMau"].ToString())
                                && ReplaceSpecialCharactersAndRemoveSpaces(chiTietRow["CodeMau"].ToString()) == ReplaceSpecialCharactersAndRemoveSpaces(drnew["CodeMau"].ToString())
                                && ReplaceSpecialCharactersAndRemoveSpaces(chiTietRow["SizeKho"].ToString()) == ReplaceSpecialCharactersAndRemoveSpaces(drnew["KhoSize"].ToString()))
                            {
                                drnew["MaVTMau"] = chiTietRow["MaVtMau"];
                                drnew["MaMau"] = chiTietRow["MaMau"];
                                drnew["TenMau"] = chiTietRow["Mau"];
                                drnew["MaKhoSize"] = chiTietRow["MaSizeKho"];
                                drnew["TenVT"] = chiTietRow["TenVT"];
                                
                                break; // Thoát vòng lặp nếu chỉ cần tìm dòng đầu tiên phù hợp
                            }
                        }
                    }
                    // Thêm dòng vào bảng dtSave
                    dtSave.Rows.Add(drnew);

                }

                // Lưu dữ liệu đã xử lý
                return dtSave;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                XtraMessageBox.Show("Đã xảy ra lỗi khi xử lý file Excel. Vui lòng kiểm tra lại!\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        #endregion
        private void gVNhapKho_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            // Kiểm tra nếu nhấp vào header của cột
            if (hitInfo.InColumn && hitInfo.Column != null && hitInfo.Column.FieldName == "XacNhan")
            {
                if (!tblNhapKho.Columns.Contains("XacNhan"))
                {
                    DataColumn col = new DataColumn("XacNhan", typeof(bool));
                    tblNhapKho.Columns.Add(col);
                }
                foreach (DataRow row in tblNhapKho.Rows)
                {
                    row["XacNhan"] = true;
                }
                gCNhapKho.DataSource = tblNhapKho;//gán datasource cho gỉdcontorl
            }
        }
        private void loadBarcode()
        {
            string urlGet = string.Format("{0}", URL + "VatTuNhapKho/Get");
            string jsonGet = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGet); }).Result;
            if (jsonGet == "[]") return;

            DataTable tblBarcode = JsonConvert.DeserializeObject<DataTable>(jsonGet);
            foreach (DataRow row in tblBarcode.Rows)
            {
                // Lấy giá trị từ cột "Barcode" và thêm vào danh sách
                string barcode = row["Barcode"].ToString();
                _lstBarcode.Add(barcode);
            }
        }
        private void loadVatTuNhapKho(string _maVT)
        {
            string urlGet = string.Format("{0}?para2={1}", URL + "VatTuNhapKho/Get", _maVT);
            string jsonGet = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGet); }).Result;

            tblNhapKho = JsonConvert.DeserializeObject<DataTable>(jsonGet);
            if (tblNhapKho.Rows.Count == 0)
            {
                setTxtNhapKhoNull();
                CreateNewDataTable();
            }

            if (!tblNhapKho.Columns.Contains("XacNhan"))
            {
                DataColumn col = new DataColumn("XacNhan", typeof(bool));
                tblNhapKho.Columns.Add(col);

                // Đánh dấu tất cả các hàng trong cột 'XacNhan' là true
                foreach (DataRow row in tblNhapKho.Rows)
                {
                    row["XacNhan"] = true; // Hoặc điều kiện logic để xác định giá trị
                }
            }

            gCNhapKho.DataSource = tblNhapKho;
            for (int i = 0; i < gVNhapKho.RowCount; i++)
            {
                gVNhapKho.SetRowCellValue(i, colMaDTKHNK, tblNhapKho.Rows[i]["MaDTKH"].ToString());
            }
            loadLoc();
        }

        private void gVNhapKho_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow row = gVNhapKho.GetFocusedDataRow();
            if (row == null) return;

            setTxtValueNhapKho(row);
        }

        private void loadDotNhap(string _maVT)
        {
            string urlDotNhap = string.Format("{0}?para2={1}", URL + "VatTuNhapKho/GetDotNhap", _maVT);
            string jsonDotNhap = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDotNhap); }).Result;

            if (jsonDotNhap != "[]")
            {
                DataTable tblDotNhap = JsonConvert.DeserializeObject<DataTable>(jsonDotNhap);
                dotnhap = int.TryParse(tblDotNhap.Rows[0]["DotNhap"].ToString(), out int dotNhapValue) ? dotNhapValue + 1 : dotnhap;

            }
        }

        private void txtTongKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập
            }
        }
        private void searchLookUpEdit_MH_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MH.EditValue != null)
            {
                _mahang = searchLookUpEdit_MH.EditValue.ToString();
                string urlMau_DH = string.Format("{0}?_mahang={1}", URL + "BOM/GET_Mau", _mahang);
                string jsonMau_DH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau_DH); }).Result;
                if (jsonMau_DH == "[]") return;
                DataTable tblMau_DH = JsonConvert.DeserializeObject<DataTable>(jsonMau_DH);


                /*  searchLookUpEdit_MauDH.Properties.DataSource = tblMau_DH;
                  searchLookUpEdit_MauDH.Properties.ValueMember = "MaMau";
                  searchLookUpEdit_MauDH.Properties.DisplayMember = "TenMau";*/
            }
            TimKiem();
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            string filePathPrintReview = XuatFile();
            if (filePathPrintReview == "null")
            {
                MessageBox.Show("Xảy ra lỗi khi tạo file", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if(filePathPrintReview == "none")
            {
                return;
            }    
            else
            {
                frmPrint_DSVatTu frm = new frmPrint_DSVatTu(filePathPrintReview);
                frm.Show();
            }
        }

        #region in phiếu
        private string XuatFile()
        {
            DataRow row = gVNhapKho.GetFocusedDataRow();
            if (row == null)
            {
                MessageBox.Show("Không có vật tư nhập kho. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "none";
            }
            string filename = "PrintReviewPhieuVatTu" + "-" + DateTime.Now.Millisecond.ToString() + _random.Next(0, 99999);
            string filePathPrintReview = Path.Combine(Application.StartupPath, @"PrintReview\") + filename;
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất phiếu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                op.ShowGridLines = false;
                op.SheetName = string.Format("BaoCao");

                //_listChonThongSo.Clear();
                DataTable tblPrint = createTableforPrint();
                ExportExcelPhieu(filePathPrintReview, tblPrint);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                return filePathPrintReview;
            }
            catch (Exception ex)
            {
                Console.WriteLine("catch_save_file");
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                return "null";
            }

            return null;
        }
        private void ExportExcelPhieu(string path, DataTable tblPrint)
        {
            FileInfo file = new FileInfo(path);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

            string fileName = "";
            string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                //ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                excelWorksheet.View.ShowGridLines = false;

                excelWorksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                excelWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                excelWorksheet.Cells.Style.Font.Name = "Times New Roman";
                excelWorksheet.Cells.Style.Font.Size = 8;
                int count = 0;
                int maASCII = 61;
                int column = -4;
                int rowIndex = 0;
                foreach (DataRow _row in tblPrint.Rows)
                {
                    count += 1;
                    maASCII += 4;
                    column += 4;
                    createExcel(excelWorksheet, rowIndex, _row, maASCII, column);
                    //4 phiếu 1 hàng
                    if (count % 2 == 0)
                    {
                        maASCII = 61;
                        column = -4;
                        rowIndex += 6;
                    }
                }

                excelPackage.Save();
            }
        }
        private void createExcel(ExcelWorksheet excelWorksheet, int rowIndex, DataRow row, int maASCIIA, int column)
        {
            try
            {
                //int column = -4; // column = -4 để mỗi vòng lặp tăng thêm 4, phù hợp cho vòng lặp
                //int maASCIIA = 61; // tạo mã ASCII = 61 để + 4 = 65( mã ASCII của kí tự 'A')
                int row1 = rowIndex + 1;
                int row2 = rowIndex + 2;
                int row3 = rowIndex + 3;
                int row4 = rowIndex + 4;
                int row5 = rowIndex + 5;
                int row6 = rowIndex + 6;
                // 807.0 là chiều cao trang A4
                // 7 là số hàng phiếu in trên một tờ A4( chiều cao 1 tờ A4 là 15 phiếu)
                // 6 là số dòng của mỗi phiếu
                double height = 807.0 / (10 * 6);
                excelWorksheet.Row(row1).Height = height * 0.9 * 2;
                excelWorksheet.Row(row2).Height = height * 0.9 * 2;
                excelWorksheet.Row(row3).Height = height * 0.9 * 2;
                excelWorksheet.Row(row4).Height = height * 0.9 * 2;
                excelWorksheet.Row(row5).Height = height * 0.9 * 2;
                excelWorksheet.Row(row6).Height = height * 0.9 * 2;

                // set kích thước
                // column A, B, C, D,... /4 = 4 phiếu 1 hàng
                excelWorksheet.Column(column + 1).Width = 0.2 * (100.0 / 2);
                excelWorksheet.Column(column + 2).Width = 0.22 * (100.0 / 2);
                excelWorksheet.Column(column + 3).Width = 0.225 * (100.0 / 2);
                excelWorksheet.Column(column + 4).Width = 0.355 * (100.0 / 2);

                // maASCIIA += 4;// Mã Ascii kí tự A
                // Column A
                string cellA1 = (char)maASCIIA + row1.ToString();
                string cellA2 = (char)maASCIIA + row2.ToString();
                string cellA3 = (char)maASCIIA + row3.ToString();
                string cellA4 = (char)maASCIIA + row4.ToString();
                string cellA5 = (char)maASCIIA + row5.ToString();
                string cellA6 = (char)maASCIIA + row6.ToString();
                // Column B
                int maASCIIB = maASCIIA + 1;
                string cellB1 = (char)maASCIIB + row1.ToString();
                string cellB2 = (char)maASCIIB + row2.ToString();
                string cellB3 = (char)maASCIIB + row3.ToString();
                string cellB4 = (char)maASCIIB + row4.ToString();
                string cellB5 = (char)maASCIIB + row5.ToString();
                string cellB6 = (char)maASCIIB + row6.ToString();
                // Column C
                int maASCIIC = maASCIIA + 2;
                string cellC1 = (char)maASCIIC + row1.ToString();
                string cellC2 = (char)maASCIIC + row2.ToString();
                string cellC3 = (char)maASCIIC + row3.ToString();
                string cellC4 = (char)maASCIIC + row4.ToString();
                string cellC5 = (char)maASCIIC + row5.ToString();
                string cellC6 = (char)maASCIIC + row6.ToString();
                if (maASCIIC == 91)
                {
                    cellC1 = ((char)65).ToString() + ((char)65).ToString() + row1.ToString();
                    cellC2 = ((char)65).ToString() + ((char)65).ToString() + row2.ToString();
                    cellC3 = ((char)65).ToString() + ((char)65).ToString() + row3.ToString();
                    cellC4 = ((char)65).ToString() + ((char)65).ToString() + row4.ToString();
                    cellC5 = ((char)65).ToString() + ((char)65).ToString() + row5.ToString();
                    cellC6 = ((char)65).ToString() + ((char)65).ToString() + row6.ToString();
                }

                // Column D
                int maASCIID = maASCIIA + 3;
                string cellD1 = (char)maASCIID + row1.ToString();
                string cellD2 = (char)maASCIID + row2.ToString();
                string cellD3 = (char)maASCIID + row3.ToString();
                string cellD4 = (char)maASCIID + row4.ToString();
                string cellD5 = (char)maASCIID + row5.ToString();
                string cellD6 = (char)maASCIID + row6.ToString();

                if (maASCIID == 92)
                {
                    cellD1 = ((char)65).ToString() + ((char)66).ToString() + row1.ToString();
                    cellD2 = ((char)65).ToString() + ((char)66).ToString() + row2.ToString();
                    cellD3 = ((char)65).ToString() + ((char)66).ToString() + row3.ToString();
                    cellD4 = ((char)65).ToString() + ((char)66).ToString() + row4.ToString();
                    cellD5 = ((char)65).ToString() + ((char)66).ToString() + row5.ToString();
                    cellD6 = ((char)65).ToString() + ((char)66).ToString() + row6.ToString();
                }
                ExcelRange excelRange1 = excelWorksheet.Cells[cellA1];

                excelRange1.Value = row["TenCongTy"];


                excelWorksheet.Cells[string.Concat(cellA1, ":", cellD1)].Merge = true;
                excelWorksheet.Cells[string.Concat(cellA1, ":", cellD1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                excelWorksheet.Cells[string.Concat(cellA1, ":", cellD1)].Style.WrapText = true;

                // row
                //excelWorksheet.Row(1).Height = 10;
                // style cell A1
                excelWorksheet.Cells[string.Concat(cellA1, ":", cellD1)].Style.Font.Size = 16;
                excelWorksheet.Cells[string.Concat(cellA1, ":", cellD1)].Style.Font.Bold = true;
                //excelWorksheet.Cells[cellA1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellA1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellA1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellA1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // style cell B1
                excelWorksheet.Cells[cellB1].Style.Font.Size = 10;
                excelWorksheet.Cells[cellB1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                excelWorksheet.Cells[cellB1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellB1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // style cell C1
                excelWorksheet.Cells[cellC1].Style.Font.Size = 10;
                excelWorksheet.Cells[cellC1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                excelWorksheet.Cells[cellC1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellC1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // style cell D1
                //excelWorksheet.Cells[cellD1].Style.Font.Bold = true;
                excelWorksheet.Cells[cellD1].Style.Font.Size = 10;
                excelWorksheet.Cells[cellD1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                excelWorksheet.Cells[cellD1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellD1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellD1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // hàng 2

                ExcelRange excelRange2 = excelWorksheet.Cells[cellA2];

                excelRange2.Value = row["MaVT"];
                string _ngaynhap = row["NgayNhap"].ToString();
                if (_ngaynhap.Length > 10)
                {
                    _ngaynhap = _ngaynhap.Substring(0, 10);
                }

                excelWorksheet.Cells[string.Concat(cellA2, ":", cellC2)].Merge = true;
                excelWorksheet.Cells[string.Concat(cellA2, ":", cellC2)].Style.Font.Size = 12;
                excelWorksheet.Cells[string.Concat(cellA2, ":", cellC2)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[string.Concat(cellA2, ":", cellC2)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                excelWorksheet.Cells[string.Concat(cellA2, ":", cellC2)].Style.WrapText = true;
                excelWorksheet.Cells[cellD2].Value =   _ngaynhap;
                //excelWorksheet.Cells[cellD2].Style.WrapText = true;
                // style cell A2
                excelWorksheet.Cells[string.Concat(cellA2, ":", cellC2)].Style.Font.Size = 14;
                //excelWorksheet.Cells[cellA2].Style.Font.Bold = true;
                excelWorksheet.Cells[cellA2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellA2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                excelWorksheet.Cells[cellA2].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                // style cell D2
                //excelWorksheet.Cells[cellD2].Style.Font.Bold = true;
                excelWorksheet.Cells[cellD2].Style.Font.Size = 10;
                excelWorksheet.Cells[cellD2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                excelWorksheet.Cells[cellD2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                excelWorksheet.Cells[cellD2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                // hàng 3
                ExcelRange excelRange3 = excelWorksheet.Cells[cellA3];

                excelWorksheet.Cells[string.Concat(cellA3, ":", cellC3)].Merge = true;
                excelWorksheet.Cells[string.Concat(cellA3, ":", cellC3)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[string.Concat(cellA3, ":", cellC3)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                excelWorksheet.Cells[string.Concat(cellA3, ":", cellC3)].Style.WrapText = true;
                excelWorksheet.Cells[cellA3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                excelRange3.Value = row["TenVT"];
                excelRange3.Style.Font.Size = 12;
                excelWorksheet.Cells[cellD3].Value = "";
                // style cell A3 -> D3
                excelWorksheet.Cells[cellA3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellD3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //hàng 4
                // hàng 4(A4)
                excelWorksheet.Cells[cellA4].Value = "Kiện: " + row["Kien"];
                excelWorksheet.Cells[cellA4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellA4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                excelWorksheet.Cells[cellA4].Style.Font.Size = 10;
                excelWorksheet.Cells[cellA4].Style.WrapText = true;
                // hàng 4(B4)
                excelWorksheet.Cells[cellB4].Value = "Lót: " + row["Lot"];
                excelWorksheet.Cells[cellB4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellB4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                excelWorksheet.Cells[cellB4].Style.Font.Size = 10;
                excelWorksheet.Cells[cellB4].Style.WrapText = true;
                // hàng 4(C4) 
                excelWorksheet.Cells[cellC4].Value = "Ghi chú: " + row["GhiChu"];

                excelWorksheet.Cells[cellC4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellC4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                excelWorksheet.Cells[cellC4].Style.Font.Size = 10;
                //excelWorksheet.Cells[cellC4].Style.WrapText = true;
                // hàng 4(D4)

                excelWorksheet.Cells[cellD4].Value = "";
                excelWorksheet.Cells[cellD4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellD4].Style.Font.Size = 10;

                //border bìa phải
                excelWorksheet.Cells[cellD4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // hàng 5(A5->B5)
                ExcelRange excelRangeA5 = excelWorksheet.Cells[cellA5];
                //excelWorksheet.Cells["A5:B5"].Merge = true;
                excelRangeA5.Value = "Số lượng: " + row["SoLuong"];
                // style cell A5
                excelWorksheet.Cells[string.Concat(cellA5, ":", cellB5)].Merge = true;
                //excelWorksheet.Cells[cellA5].Style.Font.Bold = true;
                excelWorksheet.Cells[string.Concat(cellA5, ":", cellB5)].Style.Font.Size = 10;
                excelWorksheet.Cells[cellA5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellA5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                excelWorksheet.Cells[cellA5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //excelWorksheet.Cells[cellA5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //Hàng C5
                excelWorksheet.Cells[cellC5].Value = row["DVTinh"];
                excelWorksheet.Cells[cellC5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellC5].Style.Font.Size = 10;
                // hàng 5(D5)
                excelWorksheet.Cells[cellD5].Value = "";
                excelWorksheet.Cells[cellD5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellD5].Style.Font.Size = 10;
                excelWorksheet.Cells[cellD5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //hàng 6 A6 đến C6
                ExcelRange excelRangeA6 = excelWorksheet.Cells[cellA6];
                //excelWorksheet.Cells["A5:B5"].Merge = true;
                excelRangeA6.Value = "Barcode: " + row["Barcode"];
                // style cell A6
                excelWorksheet.Cells[string.Concat(cellA6, ":", cellC6)].Merge = true;
                //excelWorksheet.Cells[cellA6].Style.Font.Bold = true;
                excelWorksheet.Cells[string.Concat(cellA6, ":", cellC6)].Style.Font.Size = 11;
                excelWorksheet.Cells[cellA6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellA6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                excelWorksheet.Cells[cellA6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellA6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellB6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellC6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //Hàng 6 D6
                excelWorksheet.Cells[cellD6].Value = "";
                excelWorksheet.Cells[cellD6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                excelWorksheet.Cells[cellD6].Style.Font.Size = 10;
                excelWorksheet.Cells[cellD6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[cellD6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                //Gán ảnh Barcode
                ExcelRange excelRangeD3 = excelWorksheet.Cells[cellD3];
                excelWorksheet.Cells[string.Concat(cellD3, ":", cellD6)].Merge = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap barcodeBitmap = CreateBarcodeBitmap(row["Barcode"].ToString()))
                    {
                        barcodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0; // Reset lại vị trí của MemoryStream

                        using (var image = System.Drawing.Image.FromStream(ms))
                        {
                            //string fileSave = SaveImage((Image)image);
                            var picture = excelWorksheet.Drawings.AddPicture($"Image_{Guid.NewGuid()}",/* new FileInfo(fileSave)*/Image.FromStream(ms));
                            int columnIndex = column + 3; // Tính chỉ mục cột cho `D`
                            int rowIndexForD3 = row3 - 1; // `row3` là chỉ số dòng đã tính trước đó

                            // Đặt hình ảnh tại góc trên bên trái của ô `cellD3`
                            picture.SetPosition(rowIndexForD3, 0, columnIndex, 0);
                            Console.WriteLine($"rowIndexForD3: {rowIndexForD3}, columnIndex: {columnIndex}");

                            picture.SetSize(120, 120);
                        }
                    }
                }

                // style cell D5
                ExcelRange excelRangeD5 = excelWorksheet.Cells[cellD5];

                //excelRangeD5.Style.Font.Bold = true;
                excelRangeD5.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                excelRangeD5.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                excelRangeD5.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

       /* public string SaveImage(Image image)
        {

            string filePathPrintReview = Path.Combine(Application.StartupPath, @"PrintImPreview\sourcePath.jpg");
            image = Image.FromFile(filePathPrintReview);

           

            // Lưu ảnh vào địa chỉ cho trước
            image.Save(filePathPrintReview, System.Drawing.Imaging.ImageFormat.Jpeg);

            // Giải phóng tài nguyên
            image.Dispose();

            return filePathPrintReview;
        }*/
        private DataTable createTableforPrint()
        {
            DataTable newTable = new DataTable();
            newTable.Columns.Add("TenCongTy", typeof(string));
            newTable.Columns.Add("MaVT", typeof(string));
            newTable.Columns.Add("TenVT", typeof(string));
            newTable.Columns.Add("Kien", typeof(string));
            newTable.Columns.Add("Lot", typeof(string));
            newTable.Columns.Add("Ghichu", typeof(string));
            newTable.Columns.Add("SoLuong", typeof(float));
            newTable.Columns.Add("DVTinh", typeof(string));
            newTable.Columns.Add("Barcode", typeof(string));
            newTable.Columns.Add("NgayNhap", typeof(string));


            // Duyệt qua từng hàng trong tblNhapKho để chuyển dữ liệu sang newTable
            foreach (DataRow row in tblNhapKho.Rows)
            {
                DataRow newRow = newTable.NewRow();

                // Giả sử tblNhapKho có các cột tương ứng, bạn cần điều chỉnh tên cột chính xác theo tblNhapKho
                newRow["TenCongTy"] = "TEX-GIANG BRANCH JSC";
                newRow["MaVT"] = row["MaVT"];
                newRow["TenVT"] = row["TenVT"];
                newRow["Kien"] = row["Kien"];
                newRow["Lot"] = row["Lot"];
                newRow["Ghichu"] = row["Ghichu"];
                newRow["SoLuong"] = row["SoLuong"];
                newRow["DVTinh"] = row["DVTinh"];
                newRow["Barcode"] = row["Barcode"];
                newRow["NgayNhap"] = row["NgayNhap"];

                // Thêm hàng mới vào DataTable
                newTable.Rows.Add(newRow);
            }

            return newTable;
        }
        #endregion
        private void txtSL_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void btn_Them_Click(object sender, EventArgs e)
        {
            try
            {

                int count = int.TryParse(txtTongKien.Text, out int temp) ? temp : 0;
                if (count == 0) count = 1;
                ThemDongHangLoat(count);
                int lastRowHandle = gVNhapKho.RowCount - 1;
                if (lastRowHandle >= 0)
                {
                    gVNhapKho.FocusedRowHandle = lastRowHandle;
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!" + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ThemDongHangLoat(int count)
        {
            DataRow _rowFocus = gVNhapKho.GetFocusedDataRow();

            for (int i = 0; i < count; i++)
            {
                DataRow row = gVVatTu.GetFocusedDataRow();
                if (row == null) return;
                DataRow newRow = tblNhapKho.NewRow();

                string _maVT = row["MaVatTu"]?.ToString();

                loadDotNhap(_maVT);
                string _makho = "", _maKH = "", _vitri = "";
                if (searchLookUpEditMaKho.EditValue != null)

                {
                    _makho = searchLookUpEditMaKho.EditValue.ToString();
                }
                if (searchLookUpEditKH.EditValue != null)
                {
                    _maKH = searchLookUpEditKH.EditValue.ToString();
                }
                if (searchLookUpEditViTri.EditValue != null)
                {
                    _vitri = searchLookUpEditViTri.EditValue.ToString();
                }
                newRow["ID"] = 0;
                newRow["MaVTNhapKho"] = row["MaVatTu"] + DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                newRow["MaDH"] = _mahang;
                newRow["Barcode"] = CreateValueBarcode(_makho, txtTongKien.Text);
                newRow["MaVT"] = row["MaVatTu"];//cái này là mã phhair tự tìm
                newRow["TenVT"] = row["TenVatTu"];
                newRow["MaVTMau"] = row["MaVTMau"];
                newRow["SoLuong"] = txtSL.Text;
                newRow["MaMau"] = "";
                newRow["TenMau"] = row["Mau"];
                newRow["MaDVTinh"] = "";
                newRow["DVTinh"] = row["DonViTinh"];
                newRow["MaKhoSize"] = "";
                newRow["KhoSize"] = row["KhoVai"];
                newRow["TongKien"] = txtTongKien.Text;
                newRow["Kien"] = txtKien.Text + i;
                newRow["MaVTSP"] = row["MaVatTu"];
                newRow["MaViTri"] = _vitri;
                newRow["MaKho"] = _makho != "" ? _makho : "KH00";
                newRow["MaDTKH"] = _maKH;
                newRow["Lot"] = txtLot.Text;
                newRow["AnhMau"] = txtAnhMau.Text;
                newRow["LoangMau"] = txtLoangMau.Text; // Nhập vào loang màu
                newRow["GhiChu"] = txtGhiChu.Text; // Nhập vào ghi chú
                newRow["NgayNhap"] = dateNgayNhap.EditValue != null ? dateNgayNhap.EditValue.ToString() : DateTime.Now.ToString("dd-MM-yyyy"); // Tự tạo ngày nhập
                newRow["NguoiNhap"] = GlobleData.UserName; // Tự tạo người nhập từ môi trường hệ thống
                newRow["DotNhap"] = dotnhap;
                if (tblVatTuChiTiet.Rows.Count > 0)
                {
                    foreach (DataRow chiTietRow in tblVatTuChiTiet.Rows)
                    {
                        if (chiTietRow["MaVtMau"].ToString() == row["MaVTMau"].ToString())
                        {
                            //newRow["MaVT"] = chiTietRow["CodeVT"];
                            newRow["MaMau"] = chiTietRow["MaMau"];
                            newRow["MaKhoSize"] = chiTietRow["MaSizeKho"];

                            break; // Thoát vòng lặp nếu chỉ cần tìm dòng đầu tiên phù hợp
                        }
                    }
                }
                /* if(_rowFocus!=null)
                 {
                     newRow["SoLuong"] = _rowFocus["SoLuong"];

                     newRow["MaViTri"] = _rowFocus["MaViTri"];         
                     newRow["MaDTKH"] = _rowFocus["MaDTKH"];
                     newRow["Lot"] = _rowFocus["Lot"];
                     newRow["AnhMau"] = _rowFocus["AnhMau"];
                     newRow["LoangMau"] = _rowFocus["LoangMau"];
                     newRow["GhiChu"] = _rowFocus["GhiChu"];
                 }    */
                // Thêm dòng mới vào DataTable
                tblNhapKho.Rows.Add(newRow);

                // Làm mới và cập nhật lại nguồn dữ liệu cho GridView
                gCNhapKho.DataSource = tblNhapKho;
                gCNhapKho.RefreshDataSource();
            }
        }

       
        
      
        private void BtNapLai()
        {
            CreateSearchLookupKhachHang();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void Naplai_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            BtNapLai();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            LocDuLieu();
        }
        #region hàm phụ
        private void RepositoryItemTextEditSoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private DataTable getVatTuNhapKho()
        {
            DataRow row = gVVatTu.GetFocusedDataRow();
            if (row == null)
            {
                return null;
            }
            string _maVT = row["MaVatTu"]?.ToString();
            string urlGet = string.Format("{0}?para2={1}", URL + "VatTuNhapKho/Get", _maVT);
            string jsonGet = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGet); }).Result;
            if (jsonGet == "[]") return null;
            DataTable tblXuat = JsonConvert.DeserializeObject<DataTable>(jsonGet);

            return tblXuat;
        }
        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;


            string currentText = textEdit.Text;

            // Kiểm tra nếu ký tự không phải là số và cũng không phải dấu chấm
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }

            // Kiểm tra nếu dấu chấm đã tồn tại
            if (currentText.Contains("."))
            {
                // Chỉ cho phép nhập 2 chữ số ở phần thập phân
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);

                // Nếu phần thập phân đã có 2 số thì ngăn không cho nhập thêm
                if (decimalPart.Length >= 2 && e.KeyChar != '\b') // '\b' là phím Backspace
                {
                    e.Handled = true;
                    return;
                }
            }

            // Kiểm tra nếu là dấu chấm và đã tồn tại trong chuỗi
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true; // Ngăn không cho nhập thêm dấu chấm
            }
        }
        private void CreateNewDataTable()
        {
            tblNhapKho = new DataTable("tblNhapKho");

            tblNhapKho.Columns.Add("ID", typeof(int));
            tblNhapKho.Columns.Add("MaVTNhapKho", typeof(string));
            tblNhapKho.Columns.Add("MaDH", typeof(string));
            tblNhapKho.Columns.Add("Barcode", typeof(string));
            tblNhapKho.Columns.Add("MaVT", typeof(string));
            tblNhapKho.Columns.Add("TenVT", typeof(string));
            tblNhapKho.Columns.Add("MaVTMau", typeof(string));
            tblNhapKho.Columns.Add("SoLuong", typeof(float));
            tblNhapKho.Columns.Add("MaMau", typeof(string));
            tblNhapKho.Columns.Add("TenMau", typeof(string));
            tblNhapKho.Columns.Add("CodeMau", typeof(string));
            tblNhapKho.Columns.Add("MaDVTinh", typeof(string));
            tblNhapKho.Columns.Add("DVTinh", typeof(string));
            tblNhapKho.Columns.Add("MaKhoSize", typeof(string));
            tblNhapKho.Columns.Add("KhoSize", typeof(string));
            tblNhapKho.Columns.Add("TongKien", typeof(string));
            tblNhapKho.Columns.Add("Kien", typeof(string));
            tblNhapKho.Columns.Add("MaVTSP", typeof(string));
            tblNhapKho.Columns.Add("MaViTri", typeof(string));
            tblNhapKho.Columns.Add("MaKho", typeof(string));
            tblNhapKho.Columns.Add("MaDTKH", typeof(string));
            tblNhapKho.Columns.Add("Lot", typeof(string));
            tblNhapKho.Columns.Add("AnhMau", typeof(string));
            tblNhapKho.Columns.Add("LoangMau", typeof(string));
            tblNhapKho.Columns.Add("GhiChu", typeof(string));
            tblNhapKho.Columns.Add("NgayNhap", typeof(string));
            tblNhapKho.Columns.Add("NguoiNhap", typeof(string));
            tblNhapKho.Columns.Add("DotNhap", typeof(string));

        }

        private void CreateTableSave()
        {
            tblSave = new DataTable("tblSave");

            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("MaVTNhapKho", typeof(string));
            tblSave.Columns.Add("MaDH", typeof(string));
            tblSave.Columns.Add("Barcode", typeof(string));
            tblSave.Columns.Add("MaVT", typeof(string));
            tblSave.Columns.Add("TenVT", typeof(string));
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("SoLuong", typeof(float));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("TenMau", typeof(string));
            tblSave.Columns.Add("CodeMau", typeof(string));
            tblSave.Columns.Add("MaDVTinh", typeof(string));
            tblSave.Columns.Add("DVTinh", typeof(string));
            tblSave.Columns.Add("MaKhoSize", typeof(string));
            tblSave.Columns.Add("KhoSize", typeof(string));
            tblSave.Columns.Add("TongKien", typeof(string));
            tblSave.Columns.Add("Kien", typeof(string));
            tblSave.Columns.Add("MaVTSP", typeof(string));
            tblSave.Columns.Add("MaViTri", typeof(string));
            tblSave.Columns.Add("MaKho", typeof(string));
            tblSave.Columns.Add("MaDTKH", typeof(string));
            tblSave.Columns.Add("Lot", typeof(string));
            tblSave.Columns.Add("AnhMau", typeof(string));
            tblSave.Columns.Add("LoangMau", typeof(string));
            tblSave.Columns.Add("GhiChu", typeof(string));
            tblSave.Columns.Add("NgayNhap", typeof(string));
            tblSave.Columns.Add("NguoiNhap", typeof(string));
            tblSave.Columns.Add("DotNhap", typeof(string));

        }
        private string ReplaceSpecialCharactersAndRemoveSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            input = Regex.Replace(input, @"\s+", "");
            string result = Regex.Replace(input, @"[^a-zA-Z0-9]", "_");
            return result;
        }
        private void setTxtVatTuNull()
        {
            txtMaVT.Text = "";
            txtTenVT.Text = "";
            txtTenMau.Text = "";
            txtKhoSize.Text = "";
            txtDVTinh.Text = "";

        }
        private void setTxtNhapKhoNull()
        {
            txtMaVTSP.Text = "";
           
            searchLookUpEditKH.EditValue = "";
            txtTongKien.Text = "";
            searchLookUpEditViTri.EditValue = "";
            searchLookUpEditMaKho.EditValue = "";

            txtKien.Text = "";
            txtSL.Text = "";
            txtLot.Text = "";
            txtAnhMau.Text = "";
            txtLoangMau.Text = "";
            txtDotNhap.Text = "";
            txtMaVach.Text = "";
            picBarcode.Image = null;
        }

        private void setTxtValueVatTu(DataRow row)
        {
            txtMaVT.Text = row["MaVatTu"].ToString();
            txtTenVT.Text = row["TenVatTu"].ToString();
            txtTenMau.Text = row["Mau"].ToString();
            txtKhoSize.Text = row["KhoVai"].ToString();
            txtDVTinh.Text = row["DonViTinh"].ToString();
        }
        private void setTxtValueNhapKho(DataRow row)
        {
            txtMaVT.Text = row["MaVT"].ToString();
            txtTenVT.Text = row["TenVT"].ToString();
            txtMaVTSP.Text = row["MaVTSP"].ToString();
            //txtTenMau.Text = row["TenMau"].ToString();
            //txtDVTinh.Text = row["DVTinh"].ToString();
            //txtKhoSize.Text = row["KhoSize"].ToString();
            searchLookUpEditKH.EditValue = row["MaDTKH"].ToString();

            txtTongKien.Text = row["TongKien"].ToString();
            searchLookUpEditViTri.EditValue = row["MaViTri"].ToString();
            searchLookUpEditMaKho.EditValue = row["MaKho"].ToString();
            txtMaKho.Text = row["MaKho"].ToString();
            txtKien.Text = row["Kien"].ToString();
            txtSL.Text = row["SoLuong"].ToString();
            txtLot.Text = row["Lot"].ToString();
            txtAnhMau.Text = row["AnhMau"].ToString();
            txtLoangMau.Text = row["LoangMau"].ToString();
            txtDotNhap.Text = row["DotNhap"].ToString();

            txtMaVach.Text = row["BarCode"].ToString();
            DateTime parsedDate;
            if (DateTime.TryParseExact(row["NgayNhap"].ToString(), "yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out parsedDate))
            {
                // Gán giá trị DateTime đã chuyển đổi vào DateEdit
                dateNgayNhap.EditValue = parsedDate;
            }
            //gán barcode

            // Gán hình ảnh cho PictureBox
            picBarcode.Image = CreateBarcodeBitmap(row["BarCode"].ToString());



        }
        private string CreateValueBarcode(string _makho, string _tongkien)
        {

            string _mavach;
            if (_lstBarcode.Count == 0)
            {
                _mavach = DateTime.Now.ToString("ddMMyy") + "0" + _tongkien + _makho +
                          _random.Next(0, 9) + _random.Next(0, 9) + _random.Next(0, 9) +
                          _random.Next(0, 9) + _random.Next(0, 9);
                _lstBarcode.Add(_mavach);
                return _mavach;
            }
            do
            {
                _mavach = DateTime.Now.ToString("ddMMyy") + "0" + _tongkien + _makho +
                          _random.Next(0, 9) + _random.Next(0, 9) + _random.Next(0, 9) +
                          _random.Next(0, 9) + _random.Next(0, 9);
            } while (_lstBarcode.Contains(_mavach));
            _lstBarcode.Add(_mavach);
            return _mavach;
        }
        private Bitmap CreateBarcodeBitmap(string barcode)
        {
            string barcodeValue = barcode;
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions { Width = 70, Height = 70, Margin = 0, PureBarcode = true }
            };
            Bitmap barcodeBitmap = barcodeWriter.Write(barcodeValue);
            return barcodeBitmap;
        }
        private void LocDuLieu()
        {
            // Lấy tiêu chí lọc từ giao diện
            string dotNhap = searchLocDot.EditValue?.ToString();
            string ngayNhap = searchLocNgay.EditValue?.ToString();
            string nguoiNhap = searchLocNguoi.EditValue?.ToString();
            // Tạo điều kiện lọc
            string filterCondition = "";

            if (!string.IsNullOrEmpty(dotNhap))
            {
                filterCondition += $"[DotNhap] = '{dotNhap}'";
            }

            if (!string.IsNullOrEmpty(ngayNhap))
            {
                if (!string.IsNullOrEmpty(filterCondition)) filterCondition += " AND ";
                filterCondition += $"[NgayNhap] = '{ConvertDateFormatReverse(ngayNhap)}'";
            }

            if (!string.IsNullOrEmpty(nguoiNhap))
            {
                if (!string.IsNullOrEmpty(filterCondition)) filterCondition += " AND ";
                filterCondition += $"[NguoiNhap] LIKE '%{nguoiNhap}%'";
            }

            if (string.IsNullOrEmpty(filterCondition))
            {
                (gCNhapKho.DataSource as DataTable).DefaultView.RowFilter = string.Empty;
            }
            else
            {
                (gCNhapKho.DataSource as DataTable).DefaultView.RowFilter = filterCondition;
            }
        }
        private string ConvertDateFormat(string dateString)
        {
            
            string pattern = @"^\d{4}-\d{2}-\d{2}$";

          
            if (Regex.IsMatch(dateString, pattern))
            {
               
                string[] dateParts = dateString.Split('-');

               
                return $"{dateParts[2]}-{dateParts[1]}-{dateParts[0]}";
            }
            else
            {
               
                return dateString;
            }
        }
        private string ConvertDateFormatReverse(string dateString)
        {
            // Biểu thức chính quy để kiểm tra định dạng dd-MM-yyyy
            string pattern = @"^\d{2}-\d{2}-\d{4}$";

            // Kiểm tra xem chuỗi có khớp với định dạng không
            if (Regex.IsMatch(dateString, pattern))
            {
                // Tách chuỗi thành các phần tử ngày, tháng, năm
                string[] dateParts = dateString.Split('-');

                // Sắp xếp lại và ghép thành chuỗi mới với định dạng yyyy-MM-dd
                return $"{dateParts[2]}-{dateParts[1]}-{dateParts[0]}";
            }
            else
            {
                // Trả về chuỗi gốc nếu không đúng định dạng
                return dateString;
            }
        }
        #endregion


    }

}
