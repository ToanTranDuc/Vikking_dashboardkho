using DevExpress.DataAccess.Excel;
using DevExpress.Utils.Menu;
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
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.ResourceForm;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmHangHoa : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        //List<HangHoaEntity> lstHangHoa;
        BindingList<HangHoaEntity> _bindingHangHoaEntity;
        List<int> lstRowUpdate = new List<int>();
        List<HangHoaEntity> lstHangHoaEntity;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0, _rowhand = 0;
        DataTable _dtHangHoaCheckKeoVe;
        List<DataTable> lstThuVienDaDung = new List<DataTable>();
        private string selectedFilePath = "";
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        DataTable tblBangMau, tblBangSize, _tblVersion, _tblDsCongDoan;

        private string IsTruocGiat = "0";
        private int Version = 1, Rap = 1;
        private bool flagInsertNewVer = false;
        private bool flagNewRap = false;
        string URL_QTY = "";
        private int VersionChkList = 1;
        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;
        DataTable _tblCheckListCD, _tblCheckList_ThongSo;
        DataTable tblSize_CheckList = new DataTable();
        DataTable tblTVThongSoCheckList = new DataTable();
        DataTable tblVersionCheckList = new DataTable();

        DataTable _tblThongSoCopy = new DataTable();
        DataTable _tblHinhAnhCopy = new DataTable();
        DataTable _tblThongSoCopyAllInSeam = new DataTable();
        DataRow drFocusPO = null;
        private string mahangCopy = "", _makhCopy = string.Empty;
        private bool _isCopied = false;
        private List<int> _selectedSteps = new List<int>();
        public frmHangHoa()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            URL_QTY = (string)settingsReader.GetValue("HostQty", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstHangHoaEntity = new List<HangHoaEntity>();
            _dtHangHoaCheckKeoVe = new DataTable();
            lstThuVienDaDung = GET_ListTableThuVienDaDung();
            tblBangMau = new DataTable();
            tblBangSize = new DataTable();
            grvHinh.DoubleClick += GrvHinh_DoubleClick;
            grvHinh.PopupMenuShowing += GrvHinh_PopupMenuShowing;
            grvHinh.FocusedRowChanged += GrvHinh_FocusedRowChanged;
            grvHinh.CustomUnboundColumnData += GrvHinh_CustomUnboundColumnData;
            grcMaHangHinh.AllowDrop = true;
            grcMaHangHinh.DragDrop += GrcMaHangHinh_DragDrop;
            grcMaHangHinh.DragEnter += GrcMaHangHinh_DragEnter;
            grcMaHangHinh.DragOver += GrcMaHangHinh_DragOver;

            grvTaiLieu.DoubleClick += GrvTaiLieu_DoubleClick;
            grvTaiLieu.PopupMenuShowing += GrvTaiLieu_PopupMenuShowing;
            grvInSeam.FocusedRowChanged += GrvInSeam_FocusedRowChanged;
            grvInSeam.PopupMenuShowing += GrvInSeam_PopupMenuShowing;
            chbxIsTruocGiat.EditValueChanged += ChbxIsTruocGiat_EditValueChanged;
            cbxVersion.EditValueChanged += CbxVersion_EditValueChanged;
            cbxRap.EditValueChanged += CbxRap_EditValueChanged;
            btnImportExcel.Click += BtnImportExcel_Click;
            btnImportNewVersion.Click += BtnImportNewVersion_Click;
            btnImportNewRap.Click += BtnImportNewRap_Click;
            btnSaveSeason_TSo.Click += BtnSaveSeason_TSo_Click;
            btnSaveThongSo.Click += BtnSaveThongSo_Click;
            bandedGridViewNhapThongSo.PopupMenuShowing += BandedGridViewNhapThongSo_PopupMenuShowing;
            bandedGridViewNhapThongSo.CellValueChanged += BandedGridViewNhapThongSo_CellValueChanged;
            repoAnhKCS.ButtonClick += RepoAnhKCS_ButtonClick;
            repoFilePDF.ButtonClick += RepoFilePDF_ButtonClick;
            btnSave_SeasonHinh.Click += BtnSave_SeasonHinh_Click;

            //grvTaiLieu.DoubleClick += GrvTaiLieu_DoubleClick;
            //grvInSeam.FocusedRowChanged += GrvInSeam_FocusedRowChanged;
            //grvInSeam.PopupMenuShowing += GrvInSeam_PopupMenuShowing;
            //chbxIsTruocGiat.EditValueChanged += ChbxIsTruocGiat_EditValueChanged;
            //cbxVersion.EditValueChanged += CbxVersion_EditValueChanged;
            //cbxRap.EditValueChanged += CbxRap_EditValueChanged;
            //btnImportExcel.Click += BtnImportExcel_Click;
            //btnImportNewVersion.Click += BtnImportNewVersion_Click;
            //btnImportNewRap.Click += BtnImportNewRap_Click;
            //btnSaveSeason_TSo.Click += BtnSaveSeason_TSo_Click;
            //repoAnhKCS.ButtonClick += RepoAnhKCS_ButtonClick;
            //repoFilePDF.ButtonClick += RepoFilePDF_ButtonClick;
            //btnSave_SeasonHinh.Click += BtnSave_SeasonHinh_Click;
            grvMau.CustomUnboundColumnData += GrvMau_CustomUnboundColumnData;
            grcTaiLieu.AllowDrop = true;
            groupControl2.AllowDrop = true;
            grcTaiLieu.DragEnter += GrcTaiLieu_DragEnter;
            grcTaiLieu.DragDrop += GrcTaiLieu_DragDrop;
            grcTaiLieu.DragOver += GrcTaiLieu_DragOver;

            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn PO]";
            SettingTabCheckList();

        }



        private void GrvTaiLieu_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = grvTaiLieu.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            DXMenuItem menudelete = new DXMenuItem();
            menudelete.Caption = "Xóa tài liệu";
            menudelete.Click += Menudelete_Click;
            e.Menu.Items.Add(menudelete);
        }
        private void Menudelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DataRow drFocus = grvTaiLieu.GetFocusedDataRow();
                if (drFocus == null) return;
                drFocus["FilePDF"] = "";
                SaveTaiLieu();
            }
        }
        private void MenudeleteImage_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DataRow drFocus = grvHinh.GetFocusedDataRow();
                if (drFocus == null) return;
                var fieldName = grvHinh.FocusedColumn.FieldName;
                drFocus[fieldName] = "";
                SaveHinh();
            }
        }

        private void GrvHinh_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                //if (e.Column.FieldName == "STT" && e.IsGetData)
                //    e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
                if ((e.Column.FieldName == "UrlAnhKCS" || e.Column.FieldName == "UrlAnhTSo" || e.Column.FieldName == "UrlAnhWaterTest") && e.IsGetData)
                {
                    DataRow dr = grvHinh.GetDataRow(e.ListSourceRowIndex);
                    if (dr == null) return;
                    string url = "http://" + URL_QTY + (e.Column.FieldName == "UrlAnhKCS" ? ("/Images/Styles/" + dr["FileName"].ToString()) :
                                                        e.Column.FieldName == "UrlAnhWaterTest" ? ("/Images_WaterTest/Styles/") + dr["FileName_WaterTest"].ToString() :
                                                        ("/Images_DoTS/Styles/" + dr["FileName_TSo"].ToString()));
                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            using (var wc = new System.Net.WebClient())
                            {
                                byte[] data = wc.DownloadData(url);
                                using (var ms = new MemoryStream(data))
                                    e.Value = Image.FromStream(ms);
                            }
                        }
                        catch
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex) { }

        }

        #region Cài đặt hình
        private void InitMaHang_Hinh()
        {
            repoSearchChungLoai.ValueMember = "MaCL";
            repoSearchChungLoai.DisplayMember = "TenCL";
            GridView dvView = repoSearchChungLoai.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaCL", Caption = "Mã CL", Name = "colChungLoaiID", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenCL", Caption = "Tên chủng loại", Name = "colChungLoai", Visible = true });
            }
            repoSearchChungLoai.EditValueChanged += RepoSearchChungLoai_EditValueChanged;
        }
        private void RepoSearchChungLoai_EditValueChanged(object sender, EventArgs e)
        {
            SaveHinh();
        }
        private void LoadChungLoai()
        {
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/GetChungLoai?action=GetAllChungLoai&para1=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            repoSearchChungLoai.DataSource = dt;
        }
        private void LoadMaHang_Hinh()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            string url = string.Format("{0}", URL + $"QTY_MAHANGHINH/Get?action=Get&para1={entityHangHoa.MaHang}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt is null || dt.Rows.Count == 0)
            {

                var dtTemp = CreateTblMaHangHinh();
                var drNew = dtTemp.NewRow();

                drNew["StyleID"] = entityHangHoa.MaHang;
                drNew["MaHang"] = entityHangHoa.TenHang;
                drNew["Version"] = 1;
                dtTemp.Rows.Add(drNew);
                grcMaHangHinh.DataSource = dtTemp;
            }
            else
            {
                grcMaHangHinh.DataSource = dt;
            }
            LoadSeason_Hinh();
        }
        private async void GrvHinh_DoubleClick(object sender, EventArgs e)
        {
            var fieldName = grvHinh.FocusedColumn.FieldName;
            if (fieldName == "FileName" || fieldName == "FileName_TSo" || fieldName == "FileName_WaterTest")
            {
                DataRow drFocus = grvHinh.GetFocusedDataRow();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFileName = openFileDialog.FileName;
                    var fileSave = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName)) + ".png";
                    var result = await UploadImage(fieldName == "FileName" ? "AnhKCS" : fieldName == "FileName_WaterTest" ? "AnhWaterTest" : "AnhDoTS", sourceFileName, fileSave);
                    if (result)
                    {
                        grvHinh.SetRowCellValue(grvHinh.FocusedRowHandle, fieldName, fileSave);
                        SaveHinh();
                    }

                }
            }


        }
        private void RepoAnhKCS_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var fieldName = grvHinh.FocusedColumn.FieldName;
            var path = grvHinh.GetFocusedRowCellValue(fieldName == "AnhKCS" ? "FileName" : "FileName_TSo")?.ToString();

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    string url = "http://localhost:5480" + (fieldName == "AnhKCS" ? "/Images/Styles/" : "/Images_DoTS/Styles/") + path;
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không mở được ảnh: " + ex.Message);
                }
            }
        }
        private async Task<bool> UploadImage(string action, string sourceFileName, string fileName)
        {
            string Url = "";
            string objUrl = (string)settingsReader.GetValue("HostQty", typeof(String));
            var client = new WebClient();
            //object objUrl = _regedit.get_RegistryKey(QtyUrlKeyName);
            //objUrl = "localhost:5480";
            if (objUrl != null)
                Url = string.Format("http://{0}/api/QC/UploadImageKCS", objUrl);
            var uri = new Uri(Url);
            try
            {
                client.Headers.Add("action", action);
                client.Headers.Add("fileName", fileName);
                var data = System.IO.File.ReadAllBytes(sourceFileName);
                var responseBytes = await client.UploadDataTaskAsync(uri, data);
                string result = Encoding.UTF8.GetString(responseBytes);
                if (result.ToUpper() == "TRUE")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void SaveHinh()
        {
            this.ActiveControl = grcTaiLieu;
            var drFocus = grvHinh.GetFocusedDataRow();
            var dtSave = CreateTblMaHangHinh();
            var drNew = dtSave.NewRow();
            //CopyDataRow(drFocus, drNew);
            drNew["Pid"] = 0;
            drNew["StyleID"] = drFocus["StyleID"];
            drNew["MaHang"] = drFocus["MaHang"];
            drNew["ProductTypeID"] = drFocus["ProductTypeID"];
            drNew["FileName"] = drFocus["FileName"];
            drNew["FileName_TSo"] = drFocus["FileName_TSo"];
            drNew["FileName_WaterTest"] = drFocus["FileName_WaterTest"];
            drNew["Version"] = drFocus["Version"];
            dtSave.Rows.Add(drNew);

            string url = string.Format("{0}", URL + "QTY_MaHangHinh/Post?action=Post");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }

        }
        private DataTable CreateTblMaHangHinh()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Pid", typeof(int));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("ProductTypeID", typeof(string));
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("FileName_TSo", typeof(string));
            dt.Columns.Add("FileName_WaterTest", typeof(string));
            dt.Columns.Add("Url_Pdf", typeof(string));
            dt.Columns.Add("Version", typeof(int));
            dt.Columns.Add("GhiChu", typeof(string));
            return dt;
        }
        public void CopyDataRow(DataRow sourceRow, DataRow destinationRow)
        {
            // Loop through columns and copy data
            foreach (DataColumn column in sourceRow.Table.Columns)
            {
                destinationRow[column.ColumnName] = sourceRow[column.ColumnName];
            }
        }
        private void LoadSeason_Hinh()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drFocus = grvHinh.GetFocusedDataRow();
            if (drFocus is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetMaHang_Season?action=GetSeaSon_HinhV2&maHang={entityHangHoa.MaHang}&sizeTypeID=A&season=A&version={drFocus["Version"].ToString()}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            grcSeason_Hinh.DataSource = dt;
        }
        private void BtnSave_SeasonHinh_Click(object sender, EventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;

            this.ActiveControl = dgrNhapThongSo;
            DataTable tblSave = CreateTblMaHang_Season();
            DataTable dtSource = grcSeason_Hinh.DataSource as DataTable;
            if (dtSource is null || dtSource.Rows.Count == 0) return;
            var drFocus = grvHinh.GetFocusedDataRow();
            if (drFocus is null) return;

            foreach (DataRow dr in dtSource.Rows)
            {
                if (!(bool)dr["Chon"]) continue;
                var drNew = tblSave.NewRow();
                drNew["ID"] = 0;
                drNew["StyleID"] = entityHangHoa.MaHang;
                drNew["MaHang"] = entityHangHoa.TenHang;
                drNew["SizeTypeID"] = "";
                drNew["Season"] = dr["Season"].ToString();
                drNew["VersionDTS"] = "0";
                drNew["VersionMH"] = drFocus["Version"].ToString();
                tblSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/PostMaHang_Season?action=SaveHinh");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
        }
        private void MenuCreateNewVersion_Click(object sender, EventArgs e)
        {
            DataRow drFocus = grvHinh.GetFocusedDataRow();
            if (drFocus == null) return;
            DataTable dtSouce = grcMaHangHinh.DataSource as DataTable;
            var drLast = dtSouce.Rows[dtSouce.Rows.Count - 1];
            var version_H = Convert.ToInt16(drLast["Version"]) + 1;
            var drNew = dtSouce.NewRow();
            drNew["StyleID"] = drLast["StyleID"];
            drNew["MaHang"] = drLast["MaHang"];
            drNew["ProductTypeID"] = drLast["ProductTypeID"];
            drNew["FileName"] = "";
            drNew["FileName_TSo"] = "";
            drNew["Version"] = version_H;
            dtSouce.Rows.Add(drNew);

        }
        private void GrvHinh_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadSeason_Hinh();
        }
        private void GrvHinh_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = grvHinh.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            DXMenuItem menuCreateNewVersion = new DXMenuItem();
            menuCreateNewVersion.Caption = "Tạo version mới";
            menuCreateNewVersion.Click += MenuCreateNewVersion_Click;

            e.Menu.Items.Add(menuCreateNewVersion);
            var fieldName = grvHinh.FocusedColumn.FieldName;
            if (fieldName == "FileName" || fieldName == "FileName_TSo" || fieldName == "FileName_WaterTest")
            {
                DXMenuItem menudelete = new DXMenuItem();
                menudelete.Caption = "Xóa hình ảnh";
                menudelete.Click += MenudeleteImage_Click;
                e.Menu.Items.Add(menudelete);
            }



        }
        private void GrcMaHangHinh_DragOver(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] imageExts = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };
                bool hasImage = files.Any(f =>
                                    imageExts.Contains(Path.GetExtension(f).ToLower())
                                );
                e.Effect = hasImage ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void GrcMaHangHinh_DragEnter(object sender, DragEventArgs e)
        {
            // Kiểm tra xem có phải file đang được kéo không
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string[] imageExts = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };
                bool hasPdf = files.Any(f =>
                                    imageExts.Contains(Path.GetExtension(f).ToLower())
                                );

                if (hasPdf)
                {
                    e.Effect = DragDropEffects.Copy; // Hiển thị icon copy
                }
                else
                {
                    e.Effect = DragDropEffects.None; // Không cho phép drop
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private async void GrcMaHangHinh_DragDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DragDrop triggered");

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Xác định vị trí thả
            Point screenPoint = new Point(e.X, e.Y);
            Point gridPoint = grcMaHangHinh.PointToClient(screenPoint);

            GridHitInfo hitInfo = grvHinh.CalcHitInfo(gridPoint);

            if (!hitInfo.InRowCell || hitInfo.Column == null)
                return;

            int rowHandle = hitInfo.RowHandle;
            string columnName = hitInfo.Column.FieldName;

            System.Diagnostics.Debug.WriteLine($"Drop vào Row: {rowHandle}, Column: {columnName}");
            List<string> lstFieldNameHinh = new List<string>() { "FileName", "FileName_TSo", "FileName_WaterTest" };
            if (!lstFieldNameHinh.Contains(columnName)) return;

            DataRow drFocus = grvHinh.GetDataRow(rowHandle);
            if (drFocus == null) return;

            // Các định dạng ảnh cho phép
            string[] imageExts = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file)?.ToLower();

                if (string.IsNullOrEmpty(ext) || !imageExts.Contains(ext))
                    continue;

                FileInfo fileInfo = new FileInfo(file);
                string sourceFileName = file;

                // Tên file xử lý lại
                string fileName = ReplaceSpecialCharacterssize(
                    Path.GetFileNameWithoutExtension(fileInfo.Name)
                ) + ".png";
                var result = await UploadImage(columnName == "FileName" ? "AnhKCS" : columnName == "FileName_WaterTest" ? "AnhWaterTest" : "AnhDoTS", sourceFileName, fileName);
                if (result)
                {
                    grvHinh.SetRowCellValue(grvHinh.FocusedRowHandle, columnName, fileName);
                    SaveHinh();
                }
                // 👉 Upload hình ảnh (tương tự UploadPDF)
                //var result = await UploadImage(
                //    drFocus["MaTaiLieu"].ToString(),
                //    sourceFileName,
                //    fileName,
                //    ext
                //);

                //if (result)
                //{
                //    // Lưu tên / url hình ảnh vào cell
                //    grvHinh.SetRowCellValue(rowHandle, columnName, fileName + ext);
                //    grvHinh.FocusedRowHandle = rowHandle;

                //    SaveTaiLieu();
                //}

                break; // chỉ nhận 1 file cho 1 cell
            }
        }

        #endregion
        #region Cài đặt tài liệu
        private void LoadTaiLieu()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangTaiLieuKiThuat/Get?action=Get&para1={entityHangHoa.MaHang}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            grcTaiLieu.DataSource = dt;
        }
        private async void GrvTaiLieu_DoubleClick(object sender, EventArgs e)
        {
            var focusRow = grvTaiLieu.FocusedRowHandle;
            if (focusRow < 0) return;
            var fieldName = grvTaiLieu.FocusedColumn.FieldName;
            var drFocus = grvTaiLieu.GetFocusedDataRow();
            if (fieldName == "FilePDF")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF files (*.PDF) | *.PDF";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFileName = openFileDialog.FileName;
                    var fileName = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName));

                    var result = await UploadPDF(drFocus["MaTaiLieu"].ToString(), sourceFileName, fileName);
                    if (result)
                    {
                        grvTaiLieu.SetRowCellValue(grvTaiLieu.FocusedRowHandle, fieldName, fileName + ".pdf");
                        SaveTaiLieu();
                    }
                }
            }
        }
        private async Task<bool> UploadPDF(string TenTaiLieu, string sourceFileName, string fileName)
        {
            string Url = "";
            string objUrl = (string)settingsReader.GetValue("HostQty", typeof(String));
            var client = new WebClient();
            //object objUrl = _regedit.get_RegistryKey(QtyUrlKeyName);
            //objUrl = "localhost:5480";
            if (objUrl != null)
                Url = string.Format("http://{0}/api/QC/UploadFilePDFV2", objUrl);
            var uri = new Uri(Url);
            try
            {
                client.Headers.Add("folderName", TenTaiLieu);
                client.Headers.Add("fileName", (fileName));
                var data = System.IO.File.ReadAllBytes(sourceFileName);
                var responseBytes = await client.UploadDataTaskAsync(uri, data);
                string result = Encoding.UTF8.GetString(responseBytes);
                if (result.ToUpper() == "TRUE")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private string ReplaceSpecialCharacterssize(string input)
        {
            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement).ToUpper();
        }
        private void SaveTaiLieu()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drFocus = grvTaiLieu.GetFocusedDataRow();
            if (drFocus is null) return;


            var dtSave = CreateTblTaiLieu();
            var drSave = dtSave.NewRow();
            drSave["ID"] = 0;
            drSave["MaHang"] = entityHangHoa.MaHang;
            drSave["MaTaiLieu"] = drFocus["MaTaiLieu"];
            drSave["FilePDF"] = drFocus["FilePDF"];
            drSave["Version"] = 1;
            drSave["NVien"] = GlobleData.UserName;
            dtSave.Rows.Add(drSave);

            string url = string.Format("{0}", URL + "QTY_MaHangTaiLieuKiThuat/Post?action=Post");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }

        }
        private DataTable CreateTblTaiLieu()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaTaiLieu", typeof(string));
            dt.Columns.Add("FilePDF", typeof(string));
            dt.Columns.Add("Version", typeof(int));
            dt.Columns.Add("NVien", typeof(string));
            return dt;
        }
        private void RepoFilePDF_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var dr = grvTaiLieu.GetFocusedDataRow();
            if (dr is null) return;
            var tailieu = dr["MaTaiLieu"].ToString();
            var filename = dr["FilePDF"].ToString();
            string objUrl = (string)settingsReader.GetValue("HostQty", typeof(String));
            if (!string.IsNullOrEmpty(filename))
            {
                try
                {
                    string url = $"http://{objUrl}/Content/Pdf/" + tailieu + "/" + filename;
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không mở được ảnh: " + ex.Message);
                }
            }
        }
        private void GrcTaiLieu_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool hasPdf = files.Any(f => Path.GetExtension(f).ToLower() == ".pdf");

                e.Effect = hasPdf ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private async void GrcTaiLieu_DragDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DragDrop triggered");

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Xác định vị trí thả file
                System.Drawing.Point screenPoint = new System.Drawing.Point(e.X, e.Y);
                System.Drawing.Point gridPoint = grcTaiLieu.PointToClient(screenPoint);

                GridHitInfo hitInfo = grvTaiLieu.CalcHitInfo(gridPoint);

                int rowHandle = -1;
                string columnName = "";

                if (hitInfo.InRow)
                {
                    rowHandle = hitInfo.RowHandle;
                    if (hitInfo.Column != null)
                    {
                        columnName = hitInfo.Column.FieldName;
                    }

                    System.Diagnostics.Debug.WriteLine($"Drop vào Row: {rowHandle}, Column: {columnName}");
                }
                var drFocus = grvTaiLieu.GetDataRow(hitInfo.RowHandle);
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file);
                    if (!string.IsNullOrEmpty(ext) && ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        string sourceFileName = file;
                        var fileName = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(fileInfo.Name));
                        ;
                        var result = await UploadPDF(drFocus["MaTaiLieu"].ToString(), sourceFileName, fileName);
                        if (result)
                        {
                            grvTaiLieu.SetRowCellValue(rowHandle, columnName, fileName + ".pdf");
                            grvTaiLieu.FocusedRowHandle = rowHandle;
                            SaveTaiLieu();
                        }
                        break;
                    }
                }
            }

        }
        private void GrcTaiLieu_DragEnter(object sender, DragEventArgs e)
        {
            // Kiểm tra xem có phải file đang được kéo không
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Kiểm tra có file PDF không
                bool hasPdf = files.Any(f => Path.GetExtension(f).ToLower() == ".pdf");

                if (hasPdf)
                {
                    e.Effect = DragDropEffects.Copy; // Hiển thị icon copy
                }
                else
                {
                    e.Effect = DragDropEffects.None; // Không cho phép drop
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        #endregion
        #region Cài dặt thông số đo 
        private void btnDeleteTS_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa thông số này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                DataTable tblDelete = CreateTblSave();
                DataTable tblSource = dgrNhapThongSo.DataSource as DataTable;
                var drCheckDelete = tblSource.Rows[0];
                if (drCheckDelete["CheckDel"].ToString() == "1")
                {
                    MessageBox.Show("Thông số đã phát sinh dữ liệu, không thể xóa!", "Thông báo");
                    return;
                }
                var drDelete = tblDelete.NewRow();
                drDelete["StyleID"] = drCheckDelete["StyleID"];
                drDelete["SizeTypeID"] = drCheckDelete["SizeTypeID"];
                drDelete["Version"] = drCheckDelete["Version"];
                drDelete["Rap"] = drCheckDelete["Rap"];
                tblDelete.Rows.Add(drDelete);
                if (tblDelete.Rows.Count == 0) return;
                string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=DeleteAll");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblDelete); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
                    LoadVersion();
                }
            }
        }
        private void BandedGridViewNhapThongSo_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            DataRow newRow = bandedGridViewNhapThongSo.GetFocusedDataRow();
            if (newRow["MaHang"] == null || newRow["MaHang"].ToString() == "")
            {
                //newRow["MaLenh"] = _drMaHang["MaLenh"].ToString();
                newRow["StyleID"] = entityHangHoa.MaHang;
                newRow["MaHang"] = entityHangHoa.TenHang;
                newRow["SizeTypeID"] = drInSeam["SizeTypeID"].ToString();
                newRow["Version"] = Version;
                newRow["Rap"] = Rap;
                newRow["IsInch"] = 0;
                //newRow["Season"] = _drMaHang["Season"].ToString();
                ((DataTable)dgrNhapThongSo.DataSource).Rows.Add(newRow);
            }
            if (e.Column.FieldName == "CongDoan")
            {
                DataTable tblSource = dgrNhapThongSo.DataSource as DataTable;
                string id_cd = _tblDsCongDoan.AsEnumerable().Where(x => x["Name"].ToString() == e.Value.ToString()).Select(x => x["ID"].ToString()).FirstOrDefault();
                DataRow dr = tblSource.AsEnumerable().Where(x => x["ID_cd"].ToString() == id_cd).FirstOrDefault();
                if (dr == null)
                    newRow["ID_cd"] = id_cd;
                else
                {
                    newRow["ID_cd"] = id_cd;
                    XtraMessageBox.Show("Công đoạn đã tồn tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void BandedGridViewNhapThongSo_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();
            DXMenuItem xoa = new DXMenuItem();
            xoa.Caption = "Xóa";
            xoa.Click += Xoa_Click;
            e.Menu.Items.Add(xoa);
        }
        private void Xoa_Click(object sender, EventArgs e)
        {
            DataTable tblSave = CreateTblSave();
            DataTable tblSource = dgrNhapThongSo.DataSource as DataTable;
            DataRow drForcus = bandedGridViewNhapThongSo.GetFocusedDataRow();
            foreach (DataRow dr in tblSource.Rows)
            {
                if (dr != drForcus) continue;
                for (int i = 12; i < tblSource.Columns.Count; i++)
                {
                    DataRow itemAdd = tblSave.NewRow();
                    itemAdd["MaTS"] = dr["MaTS"].ToString();
                    // itemAdd["MaLenh"] = dr["MaLenh"].ToString();
                    itemAdd["StyleID"] = dr["StyleID"].ToString();
                    itemAdd["MaHang"] = dr["MaHang"].ToString();
                    itemAdd["SizeTypeID"] = dr["SizeTypeID"].ToString();
                    // itemAdd["Season"] = dr["Season"].ToString();
                    itemAdd["Size"] = "SIZE_" + tblSource.Columns[i].ColumnName.ToString();
                    //itemAdd["Size"] = tblSource.Columns[i].ColumnName.ToString();
                    string thongSoChuan = dr[tblSource.Columns[i].ColumnName].ToString();
                    if (thongSoChuan == "" || dr["ID_cd"].ToString() == "") continue;
                    itemAdd["ID_cd"] = Convert.ToInt32(dr["ID_cd"]);
                    itemAdd["TenCongDoan"] = dr["CongDoan"].ToString();
                    itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                    itemAdd["Version"] = Version;
                    itemAdd["Rap"] = Rap;
                    itemAdd["IsInch"] = dr["IsInch"].ToString();
                    tblSave.Rows.Add(itemAdd);
                }
            }
            if (tblSave.Rows.Count == 0) return;
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=Delete");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
                LoadThongSoDo();
            }
        }
        private void GrvInSeam_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();
            DXMenuItem CongdoanIE = new DXMenuItem();
            CongdoanIE.Caption = "Danh sách công đoạn";
            CongdoanIE.Click += CongdoanIE_Click;
            e.Menu.Items.Add(CongdoanIE);
        }
        private void CongdoanIE_Click(object sender, EventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            frmCongDoanIE fr = new frmCongDoanIE(entityHangHoa.MaHang, IsTruocGiat, entityHangHoa.TenHang, Version, Rap.ToString());
            fr.ShowDialog();
            LoadCongDoan_TSo();
        }

        private void BtnImportNewRap_Click(object sender, EventArgs e)
        {
            flagInsertNewVer = false;
            flagNewRap = true;
            Rap = (Convert.ToInt16(cbxRap.SelectedItem) + 1);
            ImportExcel();
        }
        private void BtnImportNewVersion_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn thêm phiên bản kiểm cho mã hàng?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Cancel)
            {
                return;
            }
            flagNewRap = false;
            flagInsertNewVer = true;
            ImportExcel();
        }
        private void BtnImportExcel_Click(object sender, EventArgs e)
        {
            flagNewRap = false;
            flagInsertNewVer = false;
            ImportExcel();
        }
        private void BtnSaveThongSo_Click(object sender, EventArgs e)
        {
            DataTable tblSave = CreateTblSave();
            DataTable tblSource = dgrNhapThongSo.DataSource as DataTable;

            foreach (DataRow dr in tblSource.Rows)
            {
                string mats = DateTime.Now.ToString("dd-MM-yyyy_HHmmssfff");
                for (int i = 13; i < tblSource.Columns.Count; i++)
                {
                    DataRow itemAdd = tblSave.NewRow();
                    itemAdd["MaTS"] = dr["MaTS"].ToString() == "" ? string.Format("{0}", mats) : dr["MaTS"].ToString();
                    //itemAdd["MaLenh"] = dr["MaLenh"].ToString();
                    itemAdd["StyleID"] = dr["StyleID"].ToString();
                    itemAdd["MaHang"] = dr["MaHang"].ToString();
                    itemAdd["SizeTypeID"] = dr["SizeTypeID"].ToString();
                    //itemAdd["Season"] = dr["Season"].ToString();
                    itemAdd["Size"] = ReplaceSpecialCharacterssize(tblSource.Columns[i].ColumnName.ToString());
                    //itemAdd["Size"] =ReplaceSpecialCharacterssize(tblSource.Columns[i].ColumnName.ToString());
                    if (dr["IsInch"].ToString() == "1")
                    {
                        itemAdd["SaiSoAm"] = ConvertPStoFloat(dr["SaiSoAm"].ToString() == "" ? "0" : dr["SaiSoAm"].ToString());
                        itemAdd["SaiSoDuong"] = ConvertPStoFloat(dr["SaiSoDuong"].ToString() == "" ? "0" : dr["SaiSoDuong"].ToString());
                    }
                    else
                    {
                        itemAdd["SaiSoAm"] = Convert.ToDouble(dr["SaiSoAm"].ToString() == "" ? "0" : dr["SaiSoAm"]);
                        itemAdd["SaiSoDuong"] = Convert.ToDouble(dr["SaiSoDuong"].ToString() == "" ? "0" : dr["SaiSoDuong"]);
                    }
                    string thongSoChuan = dr[tblSource.Columns[i].ColumnName].ToString();
                    if (thongSoChuan == "" || dr["ID_cd"].ToString() == "") continue;
                    itemAdd["ID_cd"] = Convert.ToInt32(dr["ID_cd"]);
                    itemAdd["TenCongDoan"] = dr["CongDoan"].ToString();
                    itemAdd["ThongSoChuan"] = dr["IsInch"].ToString() == "1" ? MixedFractionToFloat(thongSoChuan) : Convert.ToDouble(thongSoChuan);
                    //itemAdd["ThongSoChuan"] = Convert.ToDouble(thongSoChuan);
                    itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                    itemAdd["Version"] = dr["Version"].ToString();
                    itemAdd["Rap"] = dr["Rap"].ToString();
                    itemAdd["IsInch"] = dr["IsInch"].ToString();
                    tblSave.Rows.Add(itemAdd);
                }

            }
            if (tblSave.Rows.Count == 0) return;
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=SaveThongSoV2");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
                LoadThongSoDo();
            }

        }
        private void BtnSaveSeason_TSo_Click(object sender, EventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            this.ActiveControl = dgrNhapThongSo;
            DataTable tblSave = CreateTblMaHang_Season();
            DataTable dtSource = grcSeason.DataSource as DataTable;
            if (dtSource is null || dtSource.Rows.Count == 0) return;
            foreach (DataRow dr in dtSource.Rows)
            {
                if (!(bool)dr["Chon"]) continue;
                var drNew = tblSave.NewRow();
                drNew["ID"] = 0;
                drNew["StyleID"] = entityHangHoa.MaHang;
                drNew["MaHang"] = entityHangHoa.TenHang;
                drNew["SizeTypeID"] = drInSeam["SizeTypeID"].ToString();
                drNew["Season"] = dr["Season"].ToString();
                drNew["VersionDTS"] = Version;
                drNew["VersionMH"] = "0";
                tblSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/PostMaHang_Season?action=SaveHinh_DTS");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
        }
        private void ImportExcel()
        {
            try
            {
                var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (entityHangHoa is null)
                {
                    MessageBox.Show("Vui lòng chọn mã hàng!", "Thông báo");
                    return;
                }
                var drInSeam = grvInSeam.GetFocusedDataRow();
                if (drInSeam is null)
                {
                    MessageBox.Show("Vui lòng chọn đầu size!", "Thông báo");
                    return;
                }
                frmImportExcel fr = new frmImportExcel();
                fr.FormClosed += Fr_FormClosed;
                fr.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 2000, this);
            }
        }
        private void Fr_FormClosed(object sender, FormClosedEventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            frmImportExcel form2 = (frmImportExcel)sender;
            DataTable dtSource = form2.dt;
            List<DataRow> lstDataEmpty = dtSource.AsEnumerable().Where(x => x["MaHang"].ToString() == "" || x["TenCongDoan"].ToString() == "").ToList();
            foreach (DataRow item in lstDataEmpty)
            {
                dtSource.Rows.Remove(item);
            }
            if (dtSource == null || dtSource.Rows.Count == 0)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 2000, this);
                return;
            }
            Version = flagInsertNewVer ? Convert.ToInt32(_tblVersion.Rows[0]["Version"]) + 1 : Convert.ToUInt16(cbxVersion.SelectedItem);
            if (flagInsertNewVer) Rap = 1;
            if (entityHangHoa.TenHang.Trim() != dtSource.Rows[0]["MaHang"].ToString().Trim())
            {
                MessageBox.Show("Mã hàng không khớp với mã hàng đang chọn. Vui lòng kiểm tra lại!");
                return;
            }
            foreach (DataRow item in dtSource.Rows)
            {
                item["MaHang"] = entityHangHoa.TenHang;
            }
            SaveCongDoanV2(dtSource);
        }
        private void LoadSeason_TSo()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetMaHang_Season?action=GetSeaSon_DTSV2&maHang={entityHangHoa.MaHang}&sizeTypeID={drInSeam["SizeTypeID"]}&season=A&version={Version}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            grcSeason.DataSource = dt;
        }
        private void LoadInSeam_TSo()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetDH?action=GetDauSize&para1={entityHangHoa.MaHang}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            var focusRowHandle = grvInSeam.FocusedRowHandle;
            grcInSeam.DataSource = dt;
            if (focusRowHandle == 0)
            {
                LoadSize_TSo();
                LoadVersion();

            }
            if (dt.Rows.Count == 0)
            {
                gridBandSize.Children.Clear();
                dgrNhapThongSo.DataSource = null;
            }
        }
        private void GrvInSeam_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadSize_TSo();
            LoadVersion();

        }
        private void LoadSize_TSo()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetDH?action=GetSize&para1={entityHangHoa.MaHang}&para2={drInSeam["SizeTypeID"]}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            BindingSize(dt);
        }
        private void LoadVersion()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetVersion&para={entityHangHoa.MaHang}&para1={drInSeam["SizeTypeID"]}&para2={IsTruocGiat}&para3=1");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _tblVersion = tbl;
            cbxVersion.Properties.Items.Clear();

            foreach (DataRow dr in tbl.Rows)
            {
                cbxVersion.Properties.Items.Add(dr["Version"].ToString());
            }
            if (tbl.Rows.Count == 0)
            {
                cbxVersion.Properties.Items.Add("1");
                cbxVersion.SelectedItem = "1";
            }
            else
            {
                cbxVersion.SelectedItem = tbl.Rows[0]["Version"].ToString();
            }
            Version = Convert.ToInt16(cbxVersion.SelectedItem);
            LoadRap();
            LoadSeason_TSo();
        }
        private void LoadRap()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetRap&para={entityHangHoa.MaHang}&para1={drInSeam["SizeTypeID"]}&para2={IsTruocGiat}&para3={Version}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            cbxRap.Properties.Items.Clear();

            foreach (DataRow dr in tbl.Rows)
            {
                cbxRap.Properties.Items.Add(dr["Rap"].ToString());
            }
            if (tbl.Rows.Count == 0)
            {
                cbxRap.Properties.Items.Add("1");
                cbxRap.SelectedItem = "1";
            }
            else
            {
                cbxRap.SelectedItem = tbl.Rows[0]["Rap"].ToString();
            }
            Rap = Convert.ToInt16(cbxRap.SelectedItem);
            LoadCongDoan_TSo();

        }
        private void LoadCongDoan_TSo()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;

            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetDsCongDoan&para={entityHangHoa.MaHang}&para1={IsTruocGiat}&para2={Version}&para3={Rap}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblDsCongDoan = JsonConvert.DeserializeObject<DataTable>(json);
            List<string> lstCongDoan = _tblDsCongDoan.AsEnumerable().Select(x => x["Name"].ToString()).ToList();
            repositoryCboCongDoan.Items.Clear();
            if (lstCongDoan != null)
                repositoryCboCongDoan.Items.AddRange(lstCongDoan);
            LoadThongSoDo();
        }
        private void LoadThongSoDo(bool flagAllInSeam = false)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            string action = flagAllInSeam ? "GetDsThongSoCopy" : "GetDsThongSo";
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/Get?action={action}&maLenh=A&styleID={entityHangHoa.MaHang}&status={IsTruocGiat}&season={Version}&sizeTypeID={drInSeam["SizeTypeID"]}&rap={Rap}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                var IsInch = Convert.ToInt16(tbl.Rows[0]["IsInch"]);
                if (IsInch == 1)
                {
                    chbxIsInch.EditValue = true;

                }
                else chbxIsInch.EditValue = false;
                ConvertDataTS(tbl);
                if (flagAllInSeam) _tblThongSoCopyAllInSeam = tbl;
                else dgrNhapThongSo.DataSource = tbl;
            }
            else
            {
                tbl = new DataTable();
                foreach (BandedGridColumn col in bandedGridViewNhapThongSo.Columns)
                {
                    if (col.FieldName == "MaLenh" || col.FieldName == "MaHang" || col.FieldName == "Season" || col.FieldName == "ID_cd" ||
                        col.FieldName == "CongDoan" || col.FieldName == "StyleID" || col.FieldName == "MaTS" || col.FieldName == "SizeTypeID")
                        tbl.Columns.Add(col.FieldName, typeof(string));
                    else tbl.Columns.Add(col.FieldName, typeof(float));
                }
                tbl.Rows.Add(tbl.NewRow());
                DataRow firstRow = tbl.Rows[0];
                //firstRow["MaLenh"] = drMaHang["MaLenh"].ToString();
                firstRow["StyleID"] = entityHangHoa.MaHang;
                firstRow["MaHang"] = entityHangHoa.TenHang;
                //firstRow["Season"] = drMaHang["Season"].ToString();
                if (flagAllInSeam) _tblThongSoCopyAllInSeam = tbl;
                else dgrNhapThongSo.DataSource = tbl;
            }

        }
        private void SaveCongDoanV2(DataTable dtSource)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            DataTable tblSave = CreateTblSaveCD();
            var filterTable = (from DataRow dRow in dtSource.Rows
                               select new
                               {
                                   MaHang = entityHangHoa.MaHang,
                                   InSeam = dRow["InSeam"],
                                   NO = dRow["NO"],
                                   TenCongDoan = dRow["TenCongDoan"],
                                   SapXep = dRow["STT"]
                               }).Distinct().ToList();
            foreach (var dr in filterTable)
            {
                //var tempDSMaLenh = tblDsMaHang.Select(string.Format("StyleID ='{0}' and Season = '{1}'", dr["MaHang"], dr["Season"])).FirstOrDefault();
                //if (tempDSMaLenh == null) continue;
                //var maLenh = tempDSMaLenh["MaLenh"];
                DataRow itemAdd = tblSave.NewRow();
                itemAdd["ID"] = 0;
                itemAdd["StyleID"] = entityHangHoa.MaHang;
                //itemAdd["InSeam"] = System.Text.RegularExpressions.Regex.Replace(dr.InSeam.ToString(), "[^0-9a-zA-Z]+", "_");
                itemAdd["NO"] = dr.NO;
                itemAdd["Name"] = dr.TenCongDoan;
                itemAdd["SapXep"] = dr.SapXep;
                itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                itemAdd["Version"] = Version;
                itemAdd["Rap"] = Rap;
                tblSave.Rows.Add(itemAdd);
            }
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/PostCongDoanIE?action=SaveCongDoanV2");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {


                string url_CD = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetDsCongDoan&para={entityHangHoa.MaHang}&para1={IsTruocGiat}&para2={Version}&para3={Rap}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url_CD); }).Result;
                _tblDsCongDoan = JsonConvert.DeserializeObject<DataTable>(json);
                SaveThongSoV2(dtSource);
            }

        }
        private void SaveThongSoV2(DataTable tblSource)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            DataTable tblSave = CreateTblSave();
            foreach (DataRow dr in tblSource.Rows)
            {
                string mats = DateTime.Now.ToString("dd-MM-yyyy_HHmmssfff");
                var tempDSCongDoan = _tblDsCongDoan.AsEnumerable().Where(x => x["StyleID"].ToString().ToUpper() == entityHangHoa.MaHang.ToUpper()
                                                                              && x["Name"].ToString().Trim().ToUpper() == dr["TenCongDoan"].ToString().Trim().ToUpper()
                                                                               && x["NO"].ToString().Trim().ToUpper() == dr["NO"].ToString().Trim().ToUpper()
                                                                              && x["Status"].ToString() == IsTruocGiat.ToString()).FirstOrDefault();
                if (tempDSCongDoan == null) continue;
                var Id_cd = tempDSCongDoan["ID"];
                for (int i = 8; i < tblSource.Columns.Count; i++)
                {
                    DataRow itemAdd = tblSave.NewRow();
                    itemAdd["MaTS"] = string.Format("{0}", mats);
                    itemAdd["StyleID"] = entityHangHoa.MaHang;
                    itemAdd["MaHang"] = entityHangHoa.TenHang;
                    itemAdd["SizeTypeID"] = System.Text.RegularExpressions.Regex.Replace(dr["InSeam"].ToString(), "[^0-9a-zA-Z]+", "_");
                    var checkCumCharacter = tblSource.Columns[i].ColumnName.ToString().Contains('+');
                    itemAdd["Size"] = "SIZE_" + ReplaceSpecialCharacterssize(tblSource.Columns[i].ColumnName.ToString().Trim() + (checkCumCharacter ? "_" : ""));
                    if (Double.TryParse(dr["SaiSoAm"].ToString(), out double SaiSoAm))
                    {
                        itemAdd["SaiSoAm"] = SaiSoAm;
                    }
                    else
                    {
                        itemAdd["SaiSoAm"] = ConvertPStoFloat(dr["SaiSoAm"].ToString() == "" ? "0" : dr["SaiSoAm"].ToString());
                    }

                    if (Double.TryParse(dr["SaiSoDuong"].ToString(), out double SaiSoDuong))
                    {
                        itemAdd["SaiSoDuong"] = SaiSoDuong;
                    }
                    else
                    {
                        itemAdd["SaiSoDuong"] = ConvertPStoFloat(dr["SaiSoDuong"].ToString() == "" ? "0" : dr["SaiSoDuong"].ToString());
                    }

                    string thongSoChuan = "";
                    if (Double.TryParse(dr[tblSource.Columns[i].ColumnName].ToString(), out double TSChuan))
                    {
                        thongSoChuan = TSChuan.ToString();
                    }
                    else
                    {
                        if (dr[tblSource.Columns[i].ColumnName].ToString() == "") thongSoChuan = "0";
                        else thongSoChuan = MixedFractionToFloat(dr[tblSource.Columns[i].ColumnName].ToString()).ToString();
                    }
                    //  string thongSoChuan = (bool)barInch.EditValue ? MixedFractionToFloat(dr[tblSource.Columns[i].ColumnName].ToString()).ToString() : dr[tblSource.Columns[i].ColumnName].ToString();
                    if (thongSoChuan == "" || Id_cd == "") continue;
                    itemAdd["ID_cd"] = Convert.ToInt32(Id_cd);
                    itemAdd["TenCongDoan"] = dr["TenCongDoan"].ToString();
                    itemAdd["ThongSoChuan"] = Convert.ToDouble(thongSoChuan);
                    itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                    itemAdd["Version"] = Version;
                    itemAdd["Rap"] = Rap;
                    itemAdd["IsInch"] = (!tblSource.Columns.Contains("DonVi") && dr["DonVi"].ToString() == "") ? "0" : dr["DonVi"].ToString();
                    tblSave.Rows.Add(itemAdd);
                }
            }
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=SaveThongSoV2");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 2000, this);
                if (flagInsertNewVer)
                    LoadVersion();
                else if (flagNewRap)
                    LoadRap();
                else LoadThongSoDo();
            }
        }
        private void BindingSize(DataTable tbl)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DataTable>(BindingSize), new object[] { tbl });
                return;
            }
            GridBand gridBandSize = bandedGridViewNhapThongSo.Bands.Where(x => x.Name == "gridBandSize").FirstOrDefault();
            gridBandSize.Children.Clear();
            RemoveCol();
            if (tbl != null && tbl.Rows.Count > 0)
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    BandedGridColumn bandedGridColumn = new BandedGridColumn();
                    bandedGridColumn.Caption = dr["Size"].ToString();
                    bandedGridColumn.Name = "bandedGridColumnSize" + dr["SizeID"].ToString();
                    bandedGridColumn.FieldName = dr["SizeID"].ToString();
                    bandedGridColumn.Visible = true;
                    bandedGridColumn.Width = 70;
                    bandedGridColumn.ColumnEdit = repositoryTxtNumber;
                    bandedGridColumn.AppearanceCell.Options.UseTextOptions = true;
                    bandedGridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                    GridBand gridBand = new GridBand();
                    gridBand.AppearanceHeader.Options.UseTextOptions = true;
                    gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridBand.Caption = dr["Size"].ToString();
                    gridBand.Name = "gridBandSize" + dr["SizeID"].ToString();
                    gridBand.VisibleIndex = 0;
                    gridBand.Width = 70;

                    gridBandSize.Children.Add(gridBand);
                    gridBand.Columns.Add(bandedGridColumn);
                }
            }

        }
        public void RemoveCol()
        {
            List<GridColumn> lst = bandedGridViewNhapThongSo.Columns.Where(x => x.Name.Contains("bandedGridColumnSize") == true).ToList();
            if (lst == null || lst.Count == 0) return;
            foreach (GridColumn col in lst)
                bandedGridViewNhapThongSo.Columns.Remove(col);
        }
        private DataTable CreateTblSaveCD()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("StyleID", typeof(string));
            tbl.Columns.Add("InSeam", typeof(string));
            tbl.Columns.Add("NO", typeof(string));
            tbl.Columns.Add("Code", typeof(string));
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("SapXep", typeof(int));
            tbl.Columns.Add("Status", typeof(int));
            tbl.Columns.Add("Version", typeof(int));
            tbl.Columns.Add("Rap", typeof(int));
            return tbl;

        }
        private DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("MaTS", typeof(string));
            //tbl.Columns.Add("MaLenh", typeof(string));
            tbl.Columns.Add("StyleID", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("SizeTypeID", typeof(string));
            //tbl.Columns.Add("Season", typeof(string));
            tbl.Columns.Add("ID_cd", typeof(string));
            tbl.Columns.Add("TenCongDoan", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("ThongSoChuan", typeof(float));
            tbl.Columns.Add("SaiSoAm", typeof(float));
            tbl.Columns.Add("SaiSoDuong", typeof(float));
            tbl.Columns.Add("Status", typeof(int));
            tbl.Columns.Add("Version", typeof(int));
            tbl.Columns.Add("Rap", typeof(int));
            tbl.Columns.Add("IsInch", typeof(int));
            tbl.Columns.Add("POID", typeof(string));
            return tbl;
        }
        private DataTable CreateTblMaHang_Season()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("SizeTypeID", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("VersionMH", typeof(string));
            dt.Columns.Add("VersionDTS", typeof(string));
            return dt;
        }
        private DataTable CreateTblCopy()
        {
            DataTable tblCopy = new DataTable();
            tblCopy.Columns.Add("STT", typeof(int));
            tblCopy.Columns.Add("MaHang", typeof(string));
            tblCopy.Columns.Add("NO", typeof(string));
            tblCopy.Columns.Add("TenCongDoan", typeof(string));
            tblCopy.Columns.Add("InSeam", typeof(string));
            tblCopy.Columns.Add("DonVi", typeof(int));
            tblCopy.Columns.Add("SaiSoAm", typeof(string));
            tblCopy.Columns.Add("SaiSoDuong", typeof(string));
            DataTable tblSource = dgrNhapThongSo.DataSource as DataTable;
            var tblInSeam = grcInSeam.DataSource as DataTable;
            if (tblInSeam.Rows.Count > 1)
            {
                LoadThongSoDo(true);
                tblSource = _tblThongSoCopyAllInSeam;
            }
            foreach (DataRow dr in tblSource.Rows)
            {
                var drCongDoan = _tblDsCongDoan.AsEnumerable().Where(x => x["Name"].ToString().Trim().ToUpper() == dr["CongDoan"].ToString().Trim().ToUpper()).FirstOrDefault();
                if (drCongDoan is null) continue;
                DataRow itemAdd = tblCopy.NewRow();
                itemAdd["MaHang"] = dr["StyleID"].ToString();
                itemAdd["InSeam"] = dr["SizeTypeID"].ToString();
                itemAdd["SaiSoAm"] = dr["SaiSoAm"].ToString();
                itemAdd["SaiSoDuong"] = dr["SaiSoDuong"].ToString();
                itemAdd["TenCongDoan"] = dr["CongDoan"].ToString();
                itemAdd["STT"] = drCongDoan["STT"].ToString();
                itemAdd["NO"] = dr["NO"].ToString();
                itemAdd["DonVi"] = dr["IsInch"].ToString();
                for (int i = 13; i < tblSource.Columns.Count; i++)
                {
                    var colSize = tblSource.Columns[i].ColumnName.ToUpper().Replace("SIZE_", "");
                    if (!tblCopy.Columns.Contains(colSize))
                    {
                        tblCopy.Columns.Add(colSize);
                    }
                    itemAdd[colSize] = dr[tblSource.Columns[i].ColumnName].ToString();

                }
                tblCopy.Rows.Add(itemAdd);

            }
            return tblCopy;
        }
        private void ConvertDataTS(DataTable tbl)
        {
            try
            {
                bool flagA = true;
                DataTable tblA = new DataTable();
                foreach (DataColumn dc in tbl.Columns)
                {
                    tblA.Columns.Add(dc.ColumnName, typeof(string));
                }
                tblA = tbl.Copy();
                DataTable dt = tbl.Copy();
                int startSize = 0;
                foreach (DataRow dr in tbl.Rows)
                {
                    startSize = 0;
                    if (dr["IsInch"].ToString() == "0" || dr["IsInch"].ToString() == "2") continue;
                    foreach (DataColumn dc in tbl.Columns)
                    {
                        try
                        {
                            startSize++;
                            if (startSize <= 7) continue;
                            if (dr[dc] is null || dr[dc].ToString() == "") continue;
                            var x = dr[dc].GetType();
                            var a = ConvertFloattoString(Convert.ToSingle(dr[dc] ?? "0"));
                            dr[dc] = ConvertFloattoString(Convert.ToSingle(dr[dc] ?? "0"));
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private string ConvertFloattoString(float value)
        {
            try
            {
                float floatValue = Convert.ToSingle(value);

                int wholePart = (int)floatValue;
                float fractionalPart = floatValue - wholePart;

                int gcd = GetGCD((int)(fractionalPart * 10000), 10000);

                int numerator = (int)(fractionalPart * 10000) / gcd;
                int denominator = 10000 / gcd;
                if (wholePart == 0)
                {
                    return denominator == 1 ? $"{numerator}" : $"{numerator}/{denominator}";
                }
                else
                {
                    if (denominator == 1)
                        return $"{wholePart}";
                    else
                        return wholePart == 0 ? $"{numerator}/{denominator}" : $"{wholePart} {numerator}/{denominator}";
                }
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        private float ConvertPStoFloat(string _strValue)
        {
            float value = 0;
            var tempSSAm = _strValue.Trim().Split(' ');
            try
            {
                if (tempSSAm.Length == 2) value = Convert.ToSingle(tempSSAm[0].Trim()) + ConverttoFloat(tempSSAm[1].Trim());
                else value = ConverttoFloat(tempSSAm[0]);
                return value;
            }
            catch (Exception ex)
            { return 0; }
        }
        private int GetGCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        private float ConverttoFloat(string fraction)
        {
            string[] parts = fraction.Split('/');

            if (parts.Length == 2)
            {
                int numerator;
                int denominator;

                if (int.TryParse(parts[0], out numerator) && int.TryParse(parts[1], out denominator) && denominator != 0)
                {
                    float decimalValue = (float)numerator / denominator;
                    return decimalValue;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return Convert.ToSingle(fraction);
            }
        }
        private float MixedFractionToFloat(string mixedFraction)
        {
            string[] parts = mixedFraction.Split(' ');

            if (parts.Length == 1)
            {
                // If there is no space, it's just a fraction
                string[] fractionalParts = parts[0].Split('/');

                if (fractionalParts.Length == 2)
                {
                    // If there is a fraction part, convert it to a float
                    float numerator = float.Parse(fractionalParts[0]);
                    float denominator = float.Parse(fractionalParts[1]);
                    return numerator / denominator;
                }
                else
                {
                    return float.Parse(parts[0]);
                }
            }
            else if (parts.Length == 2)
            {
                // If there is a space, split the whole part and the fractional part
                float wholePart = float.Parse(parts[0]);
                string[] fractionalParts = parts[1].Split('/');

                if (fractionalParts.Length == 2)
                {
                    // If there is a fraction part, convert it to a float
                    float numerator = float.Parse(fractionalParts[0]);
                    float denominator = float.Parse(fractionalParts[1]);
                    return wholePart + (numerator / denominator);
                }
            }

            throw new ArgumentException("Invalid mixed fraction format");
        }
        private void CbxVersion_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit ch = sender as ComboBoxEdit;
            Version = Convert.ToInt16(ch.SelectedItem);
            LoadRap();
            LoadSeason_TSo();
        }
        private void CbxRap_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit ch = sender as ComboBoxEdit;
            Rap = Convert.ToInt16(ch.SelectedItem);
            LoadCongDoan_TSo();
        }
        private void ChbxIsTruocGiat_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit ch = sender as CheckEdit;
            IsTruocGiat = ch.Checked == true ? "1" : "0";
            LoadVersion();
        }
        #endregion
        #region Cài dặt thông số đo DB
        private void dgrvPO_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            drFocusPO = dgrvPO.GetFocusedDataRow();
            LoadThongSoDo_DB();
            LoadGhiChu();
        }
        private void btnDeleteTS_DB_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa thông số này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                DataTable tblDelete = CreateTblSave();
                DataTable tblSource = dgrNhapThongSo_DB.DataSource as DataTable;
                var drCheckDelete = tblSource.Rows[0];
                if (drCheckDelete["CheckDel"].ToString() == "1")
                {
                    MessageBox.Show("Thông số đã phát sinh dữ liệu, không thể xóa!", "Thông báo");
                    return;
                }
                var drDelete = tblDelete.NewRow();
                drDelete["StyleID"] = drCheckDelete["StyleID"];
                drDelete["SizeTypeID"] = drCheckDelete["SizeTypeID"];
                drDelete["POID"] = drFocusPO["POID"];
                drDelete["Version"] = drCheckDelete["Version"];
                drDelete["Rap"] = drCheckDelete["Rap"];
                tblDelete.Rows.Add(drDelete);
                if (tblDelete.Rows.Count == 0) return;
                string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=DeleteAll");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblDelete); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
                    LoadVersion_DB();
                }
            }
        }
        private void BandedGridViewNhapThongSo_DB_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam_DB.GetFocusedDataRow();
            if (drInSeam is null) return;
            DataRow newRow = bandedGridViewNhapThongSo_DB.GetFocusedDataRow();
            if (newRow["MaHang"] == null || newRow["MaHang"].ToString() == "")
            {
                //newRow["MaLenh"] = _drMaHang["MaLenh"].ToString();
                newRow["StyleID"] = entityHangHoa.MaHang;
                newRow["MaHang"] = entityHangHoa.TenHang;
                newRow["SizeTypeID"] = drInSeam["SizeTypeID"].ToString();
                newRow["Version"] = Version;
                newRow["Rap"] = Rap;
                newRow["IsInch"] = 0;
                //newRow["Season"] = _drMaHang["Season"].ToString();
                ((DataTable)dgrNhapThongSo_DB.DataSource).Rows.Add(newRow);
            }
            if (e.Column.FieldName == "CongDoan")
            {
                DataTable tblSource = dgrNhapThongSo_DB.DataSource as DataTable;
                string id_cd = _tblDsCongDoan.AsEnumerable().Where(x => x["Name"].ToString() == e.Value.ToString()).Select(x => x["ID"].ToString()).FirstOrDefault();
                DataRow dr = tblSource.AsEnumerable().Where(x => x["ID_cd"].ToString() == id_cd).FirstOrDefault();
                if (dr == null)
                    newRow["ID_cd"] = id_cd;
                else
                {
                    newRow["ID_cd"] = id_cd;
                    XtraMessageBox.Show("Công đoạn đã tồn tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void BandedGridViewNhapThongSo_DB_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();
            DXMenuItem xoa = new DXMenuItem();
            xoa.Caption = "Xóa";
            xoa.Click += XoaDB_Click;
            e.Menu.Items.Add(xoa);
        }
        private void XoaDB_Click(object sender, EventArgs e)
        {
            DataTable tblSave = CreateTblSave();
            DataTable tblSource = dgrNhapThongSo_DB.DataSource as DataTable;
            DataRow drForcus = bandedGridViewNhapThongSo_DB.GetFocusedDataRow();
            foreach (DataRow dr in tblSource.Rows)
            {
                if (dr != drForcus) continue;
                for (int i = 12; i < tblSource.Columns.Count; i++)
                {
                    DataRow itemAdd = tblSave.NewRow();
                    itemAdd["MaTS"] = dr["MaTS"].ToString();
                    // itemAdd["MaLenh"] = dr["MaLenh"].ToString();
                    itemAdd["StyleID"] = dr["StyleID"].ToString();
                    itemAdd["MaHang"] = dr["MaHang"].ToString();
                    itemAdd["POID"] = drFocusPO["POID"].ToString();
                    itemAdd["SizeTypeID"] = dr["SizeTypeID"].ToString();
                    // itemAdd["Season"] = dr["Season"].ToString();
                    itemAdd["Size"] = "SIZE_" + tblSource.Columns[i].ColumnName.ToString();
                    //itemAdd["Size"] = tblSource.Columns[i].ColumnName.ToString();
                    string thongSoChuan = dr[tblSource.Columns[i].ColumnName].ToString();
                    if (thongSoChuan == "" || dr["ID_cd"].ToString() == "") continue;
                    itemAdd["ID_cd"] = Convert.ToInt32(dr["ID_cd"]);
                    itemAdd["TenCongDoan"] = dr["CongDoan"].ToString();
                    itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                    itemAdd["Version"] = Version;
                    itemAdd["Rap"] = Rap;
                    itemAdd["IsInch"] = dr["IsInch"].ToString();
                    tblSave.Rows.Add(itemAdd);
                }
            }
            if (tblSave.Rows.Count == 0) return;
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=Delete");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
                LoadThongSoDo_DB();
            }
        }
        private void GrvInSeam_DB_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();
            DXMenuItem CongdoanIE = new DXMenuItem();
            CongdoanIE.Caption = "Danh sách công đoạn";
            CongdoanIE.Click += CongdoanIE_DB_Click;
            e.Menu.Items.Add(CongdoanIE);
        }
        private void CongdoanIE_DB_Click(object sender, EventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            frmCongDoanIE fr = new frmCongDoanIE(entityHangHoa.MaHang, IsTruocGiat, entityHangHoa.TenHang, Version, Rap.ToString());
            fr.ShowDialog();
            LoadCongDoan_TSo_DB();
        }

        private void BtnImportNewRap_DB_Click(object sender, EventArgs e)
        {
            flagInsertNewVer = false;
            flagNewRap = true;
            Rap = (Convert.ToInt16(cbxRap.SelectedItem) + 1);
            ImportExcel_DB();
        }
        private void BtnImportNewVersion_DB_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn thêm phiên bản kiểm cho mã hàng?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Cancel)
            {
                return;
            }
            flagNewRap = false;
            flagInsertNewVer = true;
            ImportExcel_DB();
        }
        private void BtnImportExcel_DB_Click(object sender, EventArgs e)
        {
            flagNewRap = false;
            flagInsertNewVer = false;
            ImportExcel_DB();
        }
        private void BtnSaveThongSo_DB_Click(object sender, EventArgs e)
        {
            SaveGhiChu();
            DataTable tblSave = CreateTblSave();
            DataTable tblSource = dgrNhapThongSo_DB.DataSource as DataTable;

            foreach (DataRow dr in tblSource.Rows)
            {
                string mats = DateTime.Now.ToString("dd-MM-yyyy_HHmmssfff");
                for (int i = 13; i < tblSource.Columns.Count; i++)
                {
                    DataRow itemAdd = tblSave.NewRow();
                    itemAdd["MaTS"] = dr["MaTS"].ToString() == "" ? string.Format("{0}", mats) : dr["MaTS"].ToString();
                    //itemAdd["MaLenh"] = dr["MaLenh"].ToString();
                    itemAdd["StyleID"] = dr["StyleID"].ToString();
                    itemAdd["MaHang"] = dr["MaHang"].ToString();
                    itemAdd["SizeTypeID"] = dr["SizeTypeID"].ToString();
                    //itemAdd["Season"] = dr["Season"].ToString();
                    itemAdd["Size"] = ReplaceSpecialCharacterssize(tblSource.Columns[i].ColumnName.ToString());
                    //itemAdd["Size"] =ReplaceSpecialCharacterssize(tblSource.Columns[i].ColumnName.ToString());
                    if (dr["IsInch"].ToString() == "1")
                    {
                        itemAdd["SaiSoAm"] = ConvertPStoFloat(dr["SaiSoAm"].ToString() == "" ? "0" : dr["SaiSoAm"].ToString());
                        itemAdd["SaiSoDuong"] = ConvertPStoFloat(dr["SaiSoDuong"].ToString() == "" ? "0" : dr["SaiSoDuong"].ToString());
                    }
                    else
                    {
                        itemAdd["SaiSoAm"] = Convert.ToDouble(dr["SaiSoAm"].ToString() == "" ? "0" : dr["SaiSoAm"]);
                        itemAdd["SaiSoDuong"] = Convert.ToDouble(dr["SaiSoDuong"].ToString() == "" ? "0" : dr["SaiSoDuong"]);
                    }
                    string thongSoChuan = dr[tblSource.Columns[i].ColumnName].ToString();
                    if (thongSoChuan == "" || dr["ID_cd"].ToString() == "") continue;
                    itemAdd["ID_cd"] = Convert.ToInt32(dr["ID_cd"]);
                    itemAdd["TenCongDoan"] = dr["CongDoan"].ToString();
                    itemAdd["ThongSoChuan"] = dr["IsInch"].ToString() == "1" ? MixedFractionToFloat(thongSoChuan) : Convert.ToDouble(thongSoChuan);
                    //itemAdd["ThongSoChuan"] = Convert.ToDouble(thongSoChuan);
                    itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                    itemAdd["Version"] = dr["Version"].ToString();
                    itemAdd["Rap"] = dr["Rap"].ToString();
                    itemAdd["IsInch"] = dr["IsInch"].ToString();
                    itemAdd["POID"] = drFocusPO["POID"].ToString();
                    tblSave.Rows.Add(itemAdd);
                }

            }
            if (tblSave.Rows.Count == 0) return;
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=SaveThongSoV2");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
                LoadThongSoDo_DB();
            }
        }
        private void LoadGhiChu()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam_DB.GetFocusedDataRow();
            if (drInSeam is null) return;
            if (drFocusPO is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetGhiChu?action=Get&para1={entityHangHoa.MaHang}&para2={drFocusPO["POID"]}&para3={drInSeam["SizeTypeID"]}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt != null && dt.Rows.Count > 0)
            {
                txtGhiChu_DB.Text = dt.Rows[0]["Note"].ToString();
            }
            else
            {
                txtGhiChu_DB.Text = "";
            }
        }
        private void SaveGhiChu()
        {
            try
            {
                var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (entityHangHoa is null) return;
                var drInSeam = grvInSeam_DB.GetFocusedDataRow();
                if (drInSeam is null) return;
                DataTable tblSave = CreatetblGhiChu();
                DataRow drNew = tblSave.NewRow();
                drNew["StyleID"] = entityHangHoa.MaHang;
                drNew["POID"] = drFocusPO["POID"];
                drNew["DauSizeID"] = drInSeam["SizeTypeID"];
                drNew["Note"] = txtGhiChu_DB.Text;
                drNew["CreateUser"] = GlobleData.UserName;
                tblSave.Rows.Add(drNew);
                string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/PostGhiChu?action=Post");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            }
            catch (Exception ex)
            {

            }


        }
        private void ImportExcel_DB()
        {
            try
            {
                var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (entityHangHoa is null)
                {
                    MessageBox.Show("Vui lòng chọn mã hàng!", "Thông báo");
                    return;
                }
                var drInSeam = grvInSeam.GetFocusedDataRow();
                if (drInSeam is null)
                {
                    MessageBox.Show("Vui lòng chọn đầu size!", "Thông báo");
                    return;
                }
                frmImportExcel fr = new frmImportExcel();
                fr.FormClosed += Fr_DB_FormClosed;
                fr.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 2000, this);
            }
        }
        private void Fr_DB_FormClosed(object sender, FormClosedEventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            frmImportExcel form2 = (frmImportExcel)sender;
            DataTable dtSource = form2.dt;
            List<DataRow> lstDataEmpty = dtSource.AsEnumerable().Where(x => x["MaHang"].ToString() == "" || x["TenCongDoan"].ToString() == "").ToList();
            foreach (DataRow item in lstDataEmpty)
            {
                dtSource.Rows.Remove(item);
            }
            if (dtSource == null || dtSource.Rows.Count == 0)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 2000, this);
                return;
            }
            Version = flagInsertNewVer ? Convert.ToInt32(_tblVersion.Rows[0]["Version"]) + 1 : Convert.ToUInt16(cbxVersion.SelectedItem);
            if (flagInsertNewVer) Rap = 1;
            if (entityHangHoa.TenHang.Trim() != dtSource.Rows[0]["MaHang"].ToString().Trim())
            {
                MessageBox.Show("Mã hàng không khớp với mã hàng đang chọn. Vui lòng kiểm tra lại!");
                return;
            }
            foreach (DataRow item in dtSource.Rows)
            {
                item["MaHang"] = entityHangHoa.TenHang;
            }
            SaveCongDoanV2_DB(dtSource);
        }
        private void LoadPO_DB()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam_DB.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/Get?action=GetPO_MH&maLenh=A&styleID={entityHangHoa.MaHang}&status={IsTruocGiat}&season={Version}&sizeTypeID={drInSeam["SizeTypeID"]}&rap={Rap}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            int focusPO = dgrvPO.FocusedRowHandle;
            grcPO.DataSource = dt;
            if (focusPO == 0 && dt.Rows.Count > 0)
            {
                drFocusPO = dt.Rows[0];
                LoadThongSoDo_DB();
            }
        }
        private void LoadPO_Set()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam_DB.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/Get?action=GetPO_Set&maLenh=A&styleID={entityHangHoa.MaHang}&status={IsTruocGiat}&season={Version}&sizeTypeID={drInSeam["SizeTypeID"]}&rap={Rap}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_PO.Properties.DataSource = dt;
        }
        private void LoadSeason_TSo_DB()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam_DB.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetMaHang_Season?action=GetSeaSon_DTSV2&maHang={entityHangHoa.MaHang}&sizeTypeID={drInSeam["SizeTypeID"]}&season=A&version={Version}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            grcSeason.DataSource = dt;
        }
        private void LoadInSeam_TSo_DB()
        {
            //var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            //if (entityHangHoa is null) return;
            //string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetDH?action=GetDauSize&para1={entityHangHoa.MaHang}");
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = grcInSeam.DataSource as DataTable;
            var focusRowHandle = grvInSeam_DB.FocusedRowHandle;
            grcInSeam_DB.DataSource = dt;
            if (focusRowHandle == 0)
            {
                LoadSize_TSo_DB();
                LoadVersion_DB();
            }
            if (dt.Rows.Count == 0)
            {
                gridBandSize_DB.Children.Clear();
                dgrNhapThongSo_DB.DataSource = null;
            }
        }
        private void GrvInSeam_DB_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadSize_TSo_DB();
            LoadVersion_DB();
            LoadGhiChu();
        }
        private void LoadSize_TSo_DB()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam_DB.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetDH?action=GetSize&para1={entityHangHoa.MaHang}&para2={drInSeam["SizeTypeID"]}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            BindingSize_DB(dt);
        }
        private void LoadVersion_DB()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetVersion&para={entityHangHoa.MaHang}&para1={drInSeam["SizeTypeID"]}&para2={IsTruocGiat}&para3=1");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _tblVersion = tbl;
            cbxVersion.Properties.Items.Clear();

            foreach (DataRow dr in tbl.Rows)
            {
                cbxVersion.Properties.Items.Add(dr["Version"].ToString());
            }
            if (tbl.Rows.Count == 0)
            {
                cbxVersion.Properties.Items.Add("1");
                cbxVersion.SelectedItem = "1";
            }
            else
            {
                cbxVersion.SelectedItem = tbl.Rows[0]["Version"].ToString();
            }
            Version = Convert.ToInt16(cbxVersion.SelectedItem);
            LoadRap_DB();
        }
        private void LoadRap_DB()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam.GetFocusedDataRow();
            if (drInSeam is null) return;
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetRap&para={entityHangHoa.MaHang}&para1={drInSeam["SizeTypeID"]}&para2={IsTruocGiat}&para3={Version}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            cbxRap.Properties.Items.Clear();

            foreach (DataRow dr in tbl.Rows)
            {
                cbxRap.Properties.Items.Add(dr["Rap"].ToString());
            }
            if (tbl.Rows.Count == 0)
            {
                cbxRap.Properties.Items.Add("1");
                cbxRap.SelectedItem = "1";
            }
            else
            {
                cbxRap.SelectedItem = tbl.Rows[0]["Rap"].ToString();
            }
            Rap = Convert.ToInt16(cbxRap.SelectedItem);
            LoadCongDoan_TSo_DB();

        }
        private void LoadCongDoan_TSo_DB()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;

            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetDsCongDoan&para={entityHangHoa.MaHang}&para1={IsTruocGiat}&para2={Version}&para3={Rap}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblDsCongDoan = JsonConvert.DeserializeObject<DataTable>(json);
            List<string> lstCongDoan = _tblDsCongDoan.AsEnumerable().Select(x => x["Name"].ToString()).ToList();
            repositoryCboCongDoan.Items.Clear();
            if (lstCongDoan != null)
                repositoryCboCongDoan.Items.AddRange(lstCongDoan);
            LoadThongSoDo_DB();
        }
        private void LoadThongSoDo_DB(bool flagAllInSeam = false)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drInSeam = grvInSeam_DB.GetFocusedDataRow();
            if (drInSeam is null) return;
            if (drFocusPO is null) return;
            string action = flagAllInSeam ? "GetDsThongSoCopy" : "GetDsThongSo";
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/Get?action={action}&maLenh=A&styleID={entityHangHoa.MaHang}&status={IsTruocGiat}&season={Version}&sizeTypeID={drInSeam["SizeTypeID"]}&rap={Rap}&poid={drFocusPO["POID"].ToString()}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                var IsInch = Convert.ToInt16(tbl.Rows[0]["IsInch"]);
                if (IsInch == 1)
                {
                    chbxIsInch.EditValue = true;
                }
                else chbxIsInch.EditValue = false;
                ConvertDataTS(tbl);
                if (flagAllInSeam) _tblThongSoCopyAllInSeam = tbl;
                else dgrNhapThongSo_DB.DataSource = tbl;
            }
            else
            {
                tbl = new DataTable();
                foreach (BandedGridColumn col in bandedGridViewNhapThongSo_DB.Columns)
                {
                    if (col.FieldName == "MaLenh" || col.FieldName == "MaHang" || col.FieldName == "Season" || col.FieldName == "ID_cd" ||
                        col.FieldName == "CongDoan" || col.FieldName == "StyleID" || col.FieldName == "MaTS" || col.FieldName == "SizeTypeID")
                        tbl.Columns.Add(col.FieldName, typeof(string));
                    else tbl.Columns.Add(col.FieldName, typeof(float));
                }
                tbl.Rows.Add(tbl.NewRow());
                DataRow firstRow = tbl.Rows[0];
                //firstRow["MaLenh"] = drMaHang["MaLenh"].ToString();
                firstRow["StyleID"] = entityHangHoa.MaHang;
                firstRow["MaHang"] = entityHangHoa.TenHang;
                //firstRow["Season"] = drMaHang["Season"].ToString();
                if (flagAllInSeam) _tblThongSoCopyAllInSeam = tbl;
                else dgrNhapThongSo_DB.DataSource = tbl;
            }
        }
        private void SaveCongDoanV2_DB(DataTable dtSource)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            DataTable tblSave = CreateTblSaveCD();
            var filterTable = (from DataRow dRow in dtSource.Rows
                               select new
                               {
                                   MaHang = entityHangHoa.MaHang,
                                   InSeam = dRow["InSeam"],
                                   NO = dRow["NO"],
                                   TenCongDoan = dRow["TenCongDoan"],
                                   SapXep = dRow["STT"]
                               }).Distinct().ToList();
            foreach (var dr in filterTable)
            {
                //var tempDSMaLenh = tblDsMaHang.Select(string.Format("StyleID ='{0}' and Season = '{1}'", dr["MaHang"], dr["Season"])).FirstOrDefault();
                //if (tempDSMaLenh == null) continue;
                //var maLenh = tempDSMaLenh["MaLenh"];
                DataRow itemAdd = tblSave.NewRow();
                itemAdd["ID"] = 0;
                itemAdd["StyleID"] = entityHangHoa.MaHang;
                //itemAdd["InSeam"] = System.Text.RegularExpressions.Regex.Replace(dr.InSeam.ToString(), "[^0-9a-zA-Z]+", "_");
                itemAdd["NO"] = dr.NO;
                itemAdd["Name"] = dr.TenCongDoan;
                itemAdd["SapXep"] = dr.SapXep;
                itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                itemAdd["Version"] = Version;
                itemAdd["Rap"] = Rap;
                tblSave.Rows.Add(itemAdd);
            }
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/PostCongDoanIE?action=SaveCongDoanV2");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {


                string url_CD = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetDsCongDoan&para={entityHangHoa.MaHang}&para1={IsTruocGiat}&para2={Version}&para3={Rap}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url_CD); }).Result;
                _tblDsCongDoan = JsonConvert.DeserializeObject<DataTable>(json);
                SaveThongSoV2_DB(dtSource);
            }

        }
        private void SaveThongSoV2_DB(DataTable tblSource)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            DataTable tblSave = CreateTblSave();
            foreach (DataRow dr in tblSource.Rows)
            {
                string mats = DateTime.Now.ToString("dd-MM-yyyy_HHmmssfff");
                var tempDSCongDoan = _tblDsCongDoan.AsEnumerable().Where(x => x["StyleID"].ToString().ToUpper() == entityHangHoa.MaHang.ToUpper()
                                                                              && x["Name"].ToString().Trim().ToUpper() == dr["TenCongDoan"].ToString().Trim().ToUpper()
                                                                               && x["NO"].ToString().Trim().ToUpper() == dr["NO"].ToString().Trim().ToUpper()
                                                                              && x["Status"].ToString() == IsTruocGiat.ToString()).FirstOrDefault();
                if (tempDSCongDoan == null) continue;
                var Id_cd = tempDSCongDoan["ID"];
                for (int i = 8; i < tblSource.Columns.Count; i++)
                {
                    DataRow itemAdd = tblSave.NewRow();
                    itemAdd["MaTS"] = string.Format("{0}", mats);
                    itemAdd["StyleID"] = entityHangHoa.MaHang;
                    itemAdd["MaHang"] = entityHangHoa.TenHang;
                    itemAdd["SizeTypeID"] = System.Text.RegularExpressions.Regex.Replace(dr["InSeam"].ToString(), "[^0-9a-zA-Z]+", "_");
                    var checkCumCharacter = tblSource.Columns[i].ColumnName.ToString().Contains('+');
                    itemAdd["Size"] = "SIZE_" + ReplaceSpecialCharacterssize(tblSource.Columns[i].ColumnName.ToString().Trim() + (checkCumCharacter ? "_" : ""));
                    if (Double.TryParse(dr["SaiSoAm"].ToString(), out double SaiSoAm))
                    {
                        itemAdd["SaiSoAm"] = SaiSoAm;
                    }
                    else
                    {
                        itemAdd["SaiSoAm"] = ConvertPStoFloat(dr["SaiSoAm"].ToString() == "" ? "0" : dr["SaiSoAm"].ToString());
                    }

                    if (Double.TryParse(dr["SaiSoDuong"].ToString(), out double SaiSoDuong))
                    {
                        itemAdd["SaiSoDuong"] = SaiSoDuong;
                    }
                    else
                    {
                        itemAdd["SaiSoDuong"] = ConvertPStoFloat(dr["SaiSoDuong"].ToString() == "" ? "0" : dr["SaiSoDuong"].ToString());
                    }

                    string thongSoChuan = "";
                    if (Double.TryParse(dr[tblSource.Columns[i].ColumnName].ToString(), out double TSChuan))
                    {
                        thongSoChuan = TSChuan.ToString();
                    }
                    else
                    {
                        if (dr[tblSource.Columns[i].ColumnName].ToString() == "") thongSoChuan = "0";
                        else thongSoChuan = MixedFractionToFloat(dr[tblSource.Columns[i].ColumnName].ToString()).ToString();
                    }
                    //  string thongSoChuan = (bool)barInch.EditValue ? MixedFractionToFloat(dr[tblSource.Columns[i].ColumnName].ToString()).ToString() : dr[tblSource.Columns[i].ColumnName].ToString();
                    if (thongSoChuan == "" || Id_cd == "") continue;
                    itemAdd["ID_cd"] = Convert.ToInt32(Id_cd);
                    itemAdd["TenCongDoan"] = dr["TenCongDoan"].ToString();
                    itemAdd["ThongSoChuan"] = Convert.ToDouble(thongSoChuan);
                    itemAdd["Status"] = Convert.ToInt16(IsTruocGiat);
                    itemAdd["Version"] = Version;
                    itemAdd["Rap"] = Rap;
                    itemAdd["IsInch"] = (!tblSource.Columns.Contains("DonVi") && dr["DonVi"].ToString() == "") ? "0" : dr["DonVi"].ToString();
                    itemAdd["POID"] = drFocusPO["POID"].ToString();
                    tblSave.Rows.Add(itemAdd);
                }
            }
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/Post?action=SaveThongSoV2");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 2000, this);
                if (flagInsertNewVer)
                    LoadVersion_DB();
                else if (flagNewRap)
                    LoadRap_DB();
                else LoadThongSoDo_DB();
            }
        }

        private void BindingSize_DB(DataTable tbl)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DataTable>(BindingSize_DB), new object[] { tbl });
                return;
            }
            GridBand gridBandSize = bandedGridViewNhapThongSo_DB.Bands.Where(x => x.Name == "gridBandSize_DB").FirstOrDefault();
            gridBandSize.Children.Clear();
            RemoveCol_DB();
            if (tbl != null && tbl.Rows.Count > 0)
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    BandedGridColumn bandedGridColumn = new BandedGridColumn();
                    bandedGridColumn.Caption = dr["Size"].ToString();
                    bandedGridColumn.Name = "bandedGridColumnSize_DB" + dr["SizeID"].ToString();
                    bandedGridColumn.FieldName = dr["SizeID"].ToString();
                    bandedGridColumn.Visible = true;
                    bandedGridColumn.Width = 70;
                    bandedGridColumn.ColumnEdit = repositoryTxtNumber;
                    bandedGridColumn.AppearanceCell.Options.UseTextOptions = true;
                    bandedGridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                    GridBand gridBand = new GridBand();
                    gridBand.AppearanceHeader.Options.UseTextOptions = true;
                    gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridBand.Caption = dr["Size"].ToString();
                    gridBand.Name = "gridBandSize_DB" + dr["SizeID"].ToString();
                    gridBand.VisibleIndex = 0;
                    gridBand.Width = 70;

                    gridBandSize.Children.Add(gridBand);
                    gridBand.Columns.Add(bandedGridColumn);
                }
            }

        }
        public void RemoveCol_DB()
        {
            List<GridColumn> lst = bandedGridViewNhapThongSo_DB.Columns.Where(x => x.Name.Contains("bandedGridColumnSize_DB") == true).ToList();
            if (lst == null || lst.Count == 0) return;
            foreach (GridColumn col in lst)
                bandedGridViewNhapThongSo_DB.Columns.Remove(col);
        }
        private void CbxVersion_DB_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit ch = sender as ComboBoxEdit;
            Version = Convert.ToInt16(ch.SelectedItem);
            LoadRap();
            LoadSeason_TSo();
        }
        private void CbxRap_DB_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit ch = sender as ComboBoxEdit;
            Rap = Convert.ToInt16(ch.SelectedItem);
            LoadCongDoan_TSo();
        }
        private void ChbxIsTruocGiat_DB_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit ch = sender as CheckEdit;
            IsTruocGiat = ch.Checked == true ? "1" : "0";
            LoadVersion();
        }
        private DataTable CreatetblGhiChu()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("Note1", typeof(string));
            dt.Columns.Add("CreateUser", typeof(string));
            return dt;
        }
        #endregion
        #region Cài đặt công đoạn check list inline
        private void SettingTabCheckList()
        {
            RepositoryItemMemoEdit memoEdit = new RepositoryItemMemoEdit();
            memoEdit.WordWrap = true;
            grcCheckList_CD.RepositoryItems.Add(memoEdit);

            var descriptionColumn = grvCheckList_CD.Columns["Description"];
            if (descriptionColumn != null)
            {
                descriptionColumn.ColumnEdit = memoEdit;
                descriptionColumn.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                descriptionColumn.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                //descriptionColumn.Width = 350;
                //descriptionColumn.MinWidth = 200;
            }

            var noteColumn = grvCheckList_CD.Columns["Note"];
            if (noteColumn != null)
            {
                noteColumn.ColumnEdit = memoEdit;
                noteColumn.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                noteColumn.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                //noteColumn.Width = 200;
                //noteColumn.MinWidth = 100;
            }

            grvCheckList_CD.OptionsView.RowAutoHeight = true;
            grvCheckList_CD.LayoutChanged();
            grvCheckList_CD.Invalidate();
            grvCheckList_CD.RefreshData();

            //RepositoryItemTextEdit repoTol = new RepositoryItemTextEdit();
            //repoTol.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            //repoTol.Mask.EditMask = @"\d*\.?\d*"; // Chỉ cho phép số dương (nguyên hoặc thực)
            //repoTol.Mask.UseMaskAsDisplayFormat = false;

            //// Tạo repository cho cột Tol_Negative
            //RepositoryItemTextEdit repoTolNeg = new RepositoryItemTextEdit();
            //repoTolNeg.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            //repoTolNeg.Mask.EditMask = @"\d*\.?\d*"; // Chỉ cho phép số dương (nguyên hoặc thực)
            //repoTolNeg.Mask.UseMaskAsDisplayFormat = false;

            //// Gán repository vào các cột
            //grvCheckList_CD.Columns["Tol"].ColumnEdit = repoTol;
            //grvCheckList_CD.Columns["Tol_Negative"].ColumnEdit = repoTolNeg;

            // Gắn sự kiện
            //grvCheckList_CD.CellValueChanged += grvCheckList_CD_CellValueChanged;
            grvChekListTS.CellValueChanged += grvChekListTS_CellValueChanged;
            this.grvCheckList_CD.FocusedRowChanged += grvCheckList_CD_FocusedRowChanged;
            this.grvCheckList_CD.DataSourceChanged += grvCheckList_CD_DataSourceChanged;
            this.grvCheckList_CD.CustomDrawGroupRow += GridView_CustomDrawGroupRow;
            this.grvCheckList_CD.CustomDrawBandHeader += BandedGridView_CustomDrawBandHeader;
            //grvCheckList_CD.CustomColumnSort += GrvCheckList_CD_CustomColumnSort;

            this.grvChekListTS.CustomDrawBandHeader += BandedGridView_CustomDrawBandHeader;
            grvChekListTS.InvalidValueException += grvNhomCDCheckList_InvalidValueException;
            grvChekListTS.ValidatingEditor += grvThongSoCheckList_ValidatingEditor;

            //grvCheckList_CD.OptionsView.ColumnAutoWidth = false;
            grvChekListTS.OptionsView.ColumnAutoWidth = false;

            //this.splitContainerControl7.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainerControl_Paint);
            //grvCheckList_CD.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
            SetupGroupLevelColors();

            grvCheckList_CD.Columns["TenNhomCongDoan"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            grvCheckList_CD.Columns["Sort"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            grvCheckList_CD.CustomColumnSort += BandedView_CustomColumnSort;

            grvChekListTS.ClearSorting(); // Clear sorting cũ (nếu có)
            grvChekListTS.Columns["CongDoan"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
            grvChekListTS.Columns["CongDoan"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            grvChekListTS.CustomColumnSort += GrvChekListTS_CustomColumnSort;
            this.btnUP.Click += new System.EventHandler(this.btnUP_Click);
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            btnInportSizeDoTS_CheckList.Click += BtnInportSizeDoTS_CheckList_Click;

            grvCheckList_NhomSize.CustomDrawColumnHeader += gridView_CustomDrawColumnHeader;
            grvCheckList_NhomSize.FocusedRowChanged += GrvCheckList_NhomSize_FocusedRowChanged;
            grvCheckList_NhomSize.DataSourceChanged += GrvCheckList_NhomSize_DataSourceChanged;
            grvCheckList_NhomSize.RowStyle += GrvCheckList_NhomSize_RowStyle;


            this.grvCheckList_CD.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.grvCheckList_CD_RowCellStyle);
            this.grvChekListTS.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.grvCheckListTS_RowCellStyle);

            this.btnAddSize_TS.Click += new System.EventHandler(this.btnAddSize_TS_Click);
            this.btnAddCD_CheckList.Click += new System.EventHandler(this.btnAddCD_CheckList_Click);

            this.btnImport_ckList.Click += new System.EventHandler(this.btnImport_ckList_Click);
            this.btnSave_ckList.Click += new System.EventHandler(this.btnSave_ckList_Click);
            this.btnXoaCheckList.Click += new System.EventHandler(this.btnXoaCheckList_Click);
            this.btnRefresh_ckList.Click += btnRefresh_ckList_Click;
            this.btnAddCTCD.Click += new System.EventHandler(this.btnAddCTCD_Click);
            this.btnDeleteCTCD.Click += new System.EventHandler(this.btnDeleteCTCD_Click);
            this.btnDeleteTSCD.Click += new System.EventHandler(this.btnDeleteTSCD_Click);
            btnExportCheckList.Click += BtnExportCheckList_Click;
            btnImportNewVerCKList.Click += BtnImportNewVerCKList_Click;
            searchLookUpEdit_VerSionCheckLIst.EditValueChanged += SearchLookUpEdit_VerSionCheckLIst_EditValueChanged;
            //grvChekListTS.CustomColumnDisplayText += GrvChekListTS_CustomColumnDisplayText;

        }

        private void GrvChekListTS_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            if (e.Column.FieldName.Contains("@"))
            {
                object cellValue = e.Value;

                if (cellValue == null || cellValue == DBNull.Value)
                {
                    e.DisplayText = string.Empty;
                    return;
                }

                string strValue = cellValue.ToString();

                
                double doubleResult;
                if (double.TryParse(strValue, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out doubleResult))
                {
               
                    if (doubleResult == Math.Floor(doubleResult))
                    {
                     
                        e.DisplayText = ((long)doubleResult).ToString();
                    }
                    else
                    {
                  
                        e.DisplayText = doubleResult.ToString("N1",
                            System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
                else
                {
                  
                    e.DisplayText = strValue;
                }
            }
        }

        private void LoadVerCheckList()
        {
            tblVersionCheckList = new DataTable();
            try
            {

                string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
                string url = string.Format("{0}?action={1}&para1={2}", URL + "CheckListCD/Get", "GetVersion", MaHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                LoadSizeCheckList(MaHang);
                btnImport_ckList.Enabled = true;
                tblVersionCheckList = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEdit_VerSionCheckLIst.Properties.DataSource = tblVersionCheckList;
                searchLookUpEdit_VerSionCheckLIst.Properties.ValueMember = "Version";
                searchLookUpEdit_VerSionCheckLIst.Properties.DisplayMember = "Version";
                if (tblVersionCheckList?.Rows?.Count > 0)
                {
                    VersionChkList = tblVersionCheckList.AsEnumerable().Select(row =>
                    { int version; return int.TryParse(row["Version"].ToString(), out version) ? version : 1; }).Max();
                    searchLookUpEdit_VerSionCheckLIst.EditValue = VersionChkList;
                    LoadCheckListCD();
                }
                else
                {
                    grcCheckList_CD.DataSource = null;
                    grcChekListTS.DataSource = null;
                    grcCheckList_NhomSize.DataSource = null;
                    return;
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void SearchLookUpEdit_VerSionCheckLIst_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.SearchLookUpEdit edit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (edit == null) return;


            object selectedValue = edit.EditValue;

            if (selectedValue != null)
            {
                int version;
                if (int.TryParse(selectedValue.ToString(), out version))
                {
                    VersionChkList = version;
                }
                LoadCheckListCD();
            }
        }

        private void BtnImportNewVerCKList_Click(object sender, EventArgs e)
        {
            CreateTable_CheckListCD();
            string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
            if (string.IsNullOrEmpty(MaHang))
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng để Import dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tblSize_CheckList.Rows.Count == 0)
            {
                XtraMessageBox.Show("Vui lòng khai báo thêm size để Import dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmImportExcel frm = new frmImportExcel();
            frm.ShowDialog();
            frm.FormClosed -= Fr_FormClosed;
            int version = 0;
            if (tblVersionCheckList?.Rows?.Count > 0)
            {
                VersionChkList = tblVersionCheckList.AsEnumerable().Select(row =>
                { return int.TryParse(row["Version"].ToString(), out version) ? version : 1; }).Max();
                searchLookUpEdit_VerSionCheckLIst.EditValue = VersionChkList;

            }

            ReadFileExcel(frm.Path, frm.SheetName, MaHang, version + 1);
        }
        private void BtnExportCheckList_Click(object sender, EventArgs e)
        {
            string TenHang = gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("MauCheckListThongSo_{0}_{1}", TenHang, DateTime.Now.ToString("ddMMyyyyHHss"));
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string ExportFileName = Sfd.FileName;
                ExportCheckList(ExportFileName, TenHang);

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
        private void ExportCheckList(string ExportFileName, string TenHang)
        {

            try
            {
                DataTable tblExportTS = grcCheckList_CD.DataSource as DataTable;
                if (tblExportTS?.Rows?.Count == 0)
                {
                    XtraMessageBox.Show($"Chưa có dữ liệu để xuất check list !!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add(TenHang);
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Row(1).Height = 20;
                    worksheet.Row(2).Height = 28;
                    worksheet.Row(3).Height = 28;

                    // --- KHỐI TRÁI (Cột 1-2): Tên công ty & Logo ---
                    using (var range = worksheet.Cells[1, 1, 3, 2])
                    {
                        range.Merge = true;
                        range.Value = "\nVIKING VIETNAM CO., LTD";
                        range.Style.Font.Bold = true;
                        range.Style.Font.Size = 11;
                        range.Style.WrapText = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "LOGOVIKING.png");

                    if (File.Exists(logoPath))
                    {
                        try
                        {
                            using (System.Drawing.Image img = System.Drawing.Image.FromFile(logoPath))
                            {
                                var pic = worksheet.Drawings.AddPicture("LogoCompany", img);
                                int targetImgWidth = 55;
                                int targetImgHeight = 55;
                                pic.SetSize(targetImgWidth, targetImgHeight);
                                int anchorRow = 1;
                                int anchorCol = 1;
                                int rowOffset = 12;
                                int colOffset = 90;
                                pic.SetPosition(anchorRow, rowOffset, anchorCol, colOffset);
                            }
                        }
                        catch { }
                    }


                    worksheet.Cells[1, 3, 1, 6].Merge = true;
                    worksheet.Cells[1, 3].Value = "TIÊU CHUẨN KỸ THUẬT";
                    worksheet.Cells[1, 3].Style.Font.Bold = true;
                    worksheet.Cells[1, 3].Style.Font.Size = 14;

                    worksheet.Cells[2, 3, 2, 6].Merge = true;
                    worksheet.Cells[2, 3].Value = $"STYLE : {TenHang.ToUpper()}";
                    worksheet.Cells[2, 3].Style.Font.Bold = true;

                    worksheet.Cells[3, 3, 3, 6].Merge = true;
                    worksheet.Cells[3, 3].Value = "Lần BH: 00";
                    worksheet.Cells[3, 3].Style.Font.Bold = true;

                    using (var range = worksheet.Cells[1, 3, 3, 6])
                    {
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    //worksheet.Cells[1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    //worksheet.Cells[2].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    using (var range = worksheet.Cells[1, 7, 3, 9])
                    {
                        range.Merge = true;
                        range.Value = "Ký hiệu: BM10/QT07/KT01\nLần sửa đổi: 00\nNgày ban hành: 18/07/2025";
                        range.Style.WrapText = true;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }


                    int currentRow = 5;

                    var groupedData = tblExportTS.AsEnumerable()
                         .OrderBy(row => Convert.ToInt32(row["Sort"]))
                         .GroupBy(row => new
                         {
                             ParentNo = row["ParentNo"].ToString(),
                             TenNhomCongDoan = row["TenNhomCongDoan"].ToString()
                         }).ToList();

                    foreach (var group in groupedData)
                    {
                        worksheet.Cells[currentRow, 1].Value = group.Key.TenNhomCongDoan.ToUpper();
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1, currentRow, 9].Merge = true;
                        currentRow++;

                        string[] headers = { "No.", "Mô tả", "Tsố Chuẩn", "Đơn vị", "Dung sai", "Ghi chú" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var cell = worksheet.Cells[currentRow, i + 1];
                            cell.Value = headers[i];
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(235, 241, 222));
                            cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                           
                        }
                        currentRow++;

                        foreach (var row in group.OrderBy(x => Convert.ToInt32(x["NO"])))
                        {
                            worksheet.Cells[currentRow, 1].Value = row["NO"];
                            worksheet.Cells[currentRow, 2].Value = row["Description"];
                            worksheet.Cells[currentRow, 3].Value = row["Note"];
                            worksheet.Cells[currentRow, 4].Value = row["Donvi"];
                            worksheet.Cells[currentRow, 5].Value = FormatTolerance(row["Tol_Negative"]?.ToString(), row["Tol"]?.ToString());
                            worksheet.Cells[currentRow, 6].Value = row["Note_TS"];
                            worksheet.Cells[currentRow, 6].Style.WrapText = true;
                            worksheet.Cells[currentRow, 3].Style.WrapText = true;

                            for (int col = 1; col <= 6; col++)
                            {
                                worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                worksheet.Cells[currentRow, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                if (col == 2) continue;
                                worksheet.Cells[currentRow, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }

                            worksheet.Cells[currentRow, 2].Style.WrapText = true;
                            GetColorFromMucDo(worksheet, currentRow, row["MucDo"]?.ToString() ?? "");
                            currentRow++;
                        }
                       
                        worksheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        
                        worksheet.Column(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                       
                        worksheet.Column(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                      
                        worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ExportChiTietThongSoCheckList(worksheet, group.Key.ParentNo, currentRow, out int endRow);
                        currentRow = endRow + 1;
                    }

                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 45;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 20;

                    //worksheet.View.FreezePanes(6, 1);

                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatTolerance(string neg, string pos)
        {
            if (!string.IsNullOrEmpty(neg) && string.IsNullOrEmpty(pos)) return $"-{neg}";
            if (string.IsNullOrEmpty(neg) && !string.IsNullOrEmpty(pos)) return $"+{pos}";
            if (!string.IsNullOrEmpty(neg) && !string.IsNullOrEmpty(pos))
            {
                return (neg == pos) ? $"+/-{neg}" : $"-{neg}/+{pos}";
            }
            return "";
        }
        private void GetColorFromMucDo(ExcelWorksheet worksheet, int currentRow, string mucDoValue)
        {
            if (string.IsNullOrEmpty(mucDoValue))
                return;

            try
            {
                string[] colorParts = mucDoValue.Split('@');

                if (colorParts.Length > 0)
                {
                    for (int col = 1; col <= 6; col++)
                    {
                        if (col - 1 >= colorParts.Length) break;

                        string colorHex = colorParts[col - 1].Trim();
                        if (colorHex.StartsWith("#"))
                            colorHex = colorHex.Substring(1);

                        if (colorHex.Length == 6)
                        {
                            int r = Convert.ToInt32(colorHex.Substring(0, 2), 16);
                            int g = Convert.ToInt32(colorHex.Substring(2, 2), 16);
                            int b = Convert.ToInt32(colorHex.Substring(4, 2), 16);

                            worksheet.Cells[currentRow, col].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(r, g, b));
                        }
                        worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing color: {ex.Message}");
            }
        }

        private void ExportChiTietThongSoCheckList(ExcelWorksheet worksheet, string ParentNO, int startRow, out int endRow)
        {
            endRow = startRow + 1;

            try
            {
                if (_tblCheckList_ThongSo?.Rows?.Count == 0)
                {
                    return;
                }

                DataTable tblDetail = _tblCheckList_ThongSo?.Copy();
                if (tblDetail == null || tblDetail.Rows.Count == 0)
                    return;

                var detailData = tblDetail.AsEnumerable()
                    .Where(row => row["ParentNO"].ToString() == ParentNO)
                    .OrderBy(row => Convert.ToInt32(row["sort"]))
                    .GroupBy(row => row["SizeType"].ToString())
                    .ToList();

                if (detailData.Count == 0)
                {
                    return;
                }

                foreach (var sizeTypeGroup in detailData)
                {
                    string sizeType = sizeTypeGroup.Key;

                    var allSizes = sizeTypeGroup
                        .Select(row => row["Size"].ToString())
                        .Distinct()
                        .ToList();

                    // Chia sizes thành các nhóm, mỗi nhóm tối đa 10 size (vì cột bắt đầu từ 3, cột 12 = 10 size)
                    int maxSizesPerGroup = 10; // Từ cột 3 đến cột 12 = 10 cột
                    int totalGroups = (int)Math.Ceiling((double)allSizes.Count / maxSizesPerGroup);

                    for (int groupIndex = 0; groupIndex < totalGroups; groupIndex++)
                    {
                        var sizes = allSizes.Skip(groupIndex * maxSizesPerGroup).Take(maxSizesPerGroup).ToList();

                        // Tạo header row với *
                        worksheet.Cells[endRow, 2].Value = "*";
                        worksheet.Cells[endRow, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[endRow, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                        // Merge các cột sizes
                        int sizeStartCol = 3;
                        foreach (var size in sizes)
                        {
                            worksheet.Cells[endRow, sizeStartCol].Value = size;
                            worksheet.Cells[endRow, sizeStartCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[endRow, sizeStartCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksheet.Cells[endRow, sizeStartCol].Style.Font.Bold = true;
                            sizeStartCol++;
                        }
                        endRow++;

                        if (sizeTypeGroup?.Count() > 1 && sizeType != "0")
                        {
                            // Tạo row NHÓM với SizeType
                            worksheet.Cells[endRow, 2].Value = "NHÓM";
                            worksheet.Cells[endRow, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksheet.Cells[endRow, 2].Style.Font.Bold = true;

                            sizeStartCol = 3;
                            foreach (var size in sizes)
                            {
                                worksheet.Cells[endRow, sizeStartCol].Value = sizeType;
                                worksheet.Cells[endRow, sizeStartCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[endRow, sizeStartCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                worksheet.Cells[endRow, sizeStartCol].Style.Font.Bold = true;
                             


                                sizeStartCol++;
                            }
                            endRow++;
                        }

                        // Lấy các công đoạn unique (cả mã và tên) và sort
                        var congDoans = sizeTypeGroup
                            .GroupBy(row => new
                            {
                                MaCongDoan = row["MaCongDoan_ThongSo"].ToString(),
                                TenCongDoan = row["TenNhomCongDoan"].ToString()
                            })
                            .Select(g => new
                            {
                                MaCongDoan = g.Key.MaCongDoan,
                                TenCongDoan = g.Key.TenCongDoan
                            })
                            .OrderBy(c => c.MaCongDoan)
                            .ToList();

                        foreach (var congDoan in congDoans)
                        {
                            string TenCongDoan_CT = "";
                            if (tblTVThongSoCheckList?.Rows?.Count > 0)
                            {
                                var Query = tblTVThongSoCheckList.AsEnumerable().FirstOrDefault(x => x["MaCongDoan_ThongSo"]?.ToString() == congDoan.MaCongDoan);
                                if (Query != null)
                                {
                                    TenCongDoan_CT = Query["CongDoan"]?.ToString();
                                }
                            }

                            worksheet.Cells[endRow, 2].Value = TenCongDoan_CT;
                            worksheet.Cells[endRow, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksheet.Cells[endRow, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            worksheet.Cells[endRow, 2].Style.Font.Bold = true;

                            sizeStartCol = 3;
                            foreach (var size in sizes)
                            {
                                // Tìm giá trị ThongSo cho size và công đoạn này
                                var dataRow = sizeTypeGroup.FirstOrDefault(row =>
                                    row["Size"].ToString() == size &&
                                    row["MaCongDoan_ThongSo"].ToString() == congDoan.MaCongDoan);

                                if (dataRow != null)
                                {
                                    string thongSo = dataRow["ThongSo"]?.ToString() ?? "";
                                    worksheet.Cells[endRow, sizeStartCol].Value = thongSo;
                                    string colorHex = dataRow["MucDo"]?.ToString();
                                    if (colorHex.StartsWith("#"))
                                        colorHex = colorHex.Substring(1);

                                    if (colorHex.Length == 6)
                                    {
                                        int r = Convert.ToInt32(colorHex.Substring(0, 2), 16);
                                        int g = Convert.ToInt32(colorHex.Substring(2, 2), 16);
                                        int b = Convert.ToInt32(colorHex.Substring(4, 2), 16);

                                        worksheet.Cells[endRow, sizeStartCol].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(r, g, b));
                                    }

                                }

                                worksheet.Cells[endRow, sizeStartCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[endRow, sizeStartCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                                sizeStartCol++;
                            }
                            endRow++;
                        }

                   
                        if (groupIndex < totalGroups - 1)
                        {
                            endRow++;
                        }
                    }

                   
                    endRow++;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ExportChiTietThongSoCheckList: {ex.Message}");
            }
        }

        private void GrvCheckList_NhomSize_RowStyle(object sender, RowStyleEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.LightSkyBlue;   // màu nền
                e.Appearance.ForeColor = Color.Black;          // màu chữ
                e.HighPriority = true;                          // rất quan trọng
            }
        }

        private void GrvChekListTS_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "CongDoan")
            {
                GridView view = sender as GridView;

                // Lấy RepositoryItemSearchLookUpEdit
                var repo = e.Column.ColumnEdit as RepositoryItemSearchLookUpEdit;
                if (repo == null)
                {
                    e.Handled = false;
                    return;
                }

                // Lấy giá trị Value từ 2 rows
                object value1 = e.Value1;
                object value2 = e.Value2;

                // Lấy DisplayText tương ứng với Value
                string displayText1 = GetDisplayText(repo, value1);
                string displayText2 = GetDisplayText(repo, value2);

                // Tách số và text để sort natural order (1, 2, 10 thay vì 1, 10, 2)
                int num1 = ExtractLeadingNumber(displayText1);
                int num2 = ExtractLeadingNumber(displayText2);

                // So sánh theo số trước
                if (num1 != num2)
                {
                    e.Result = num1.CompareTo(num2);
                }
                else
                {
                    // Nếu số giống nhau, so sánh theo text
                    e.Result = string.Compare(displayText1, displayText2, StringComparison.Ordinal);
                }

                e.Handled = true;
            }
        }

        // Hàm helper để lấy DisplayText từ SearchLookUpEdit
        private string GetDisplayText(RepositoryItemSearchLookUpEdit repo, object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;

            try
            {
                // Tìm row trong DataSource của lookup có ValueMember = value
                if (repo.DataSource == null)
                    return value.ToString();

                string valueMember = repo.ValueMember;
                string displayMember = repo.DisplayMember;

                if (string.IsNullOrEmpty(valueMember) || string.IsNullOrEmpty(displayMember))
                    return value.ToString();

                // Tìm trong DataSource
                if (repo.DataSource is DataTable dt)
                {
                    DataRow[] rows = dt.Select($"{valueMember} = '{value}'");
                    if (rows.Length > 0)
                        return rows[0][displayMember]?.ToString() ?? "";
                }
                else if (repo.DataSource is DataView dv)
                {
                    dv.RowFilter = $"{valueMember} = '{value}'";
                    if (dv.Count > 0)
                        return dv[0][displayMember]?.ToString() ?? "";
                    dv.RowFilter = ""; // Reset filter
                }
                else if (repo.DataSource is IEnumerable<DataRow> enumerable)
                {
                    var row = enumerable.FirstOrDefault(r => r[valueMember]?.ToString() == value.ToString());
                    if (row != null)
                        return row[displayMember]?.ToString() ?? "";
                }

                return value.ToString();
            }
            catch
            {
                return value.ToString();
            }
        }

        private void SetupGroupLevelColors()
        {
            groupLevelColorBackground = new Dictionary<int, Color>
    {
        { -1, ColorTranslator.FromHtml("#DDEBFB") },
        { 0, ColorTranslator.FromHtml("#DDEBFB") },
        { 1, ColorTranslator.FromHtml("#FFE2D3") },
        { 2, ColorTranslator.FromHtml("#D7F5E8") }
    };

            groupLevelColors = new Dictionary<int, Color>
    {
        { -1, ColorTranslator.FromHtml("#2A5D9F") },
        { 0, ColorTranslator.FromHtml("#2A5D9F") },
        { 1, ColorTranslator.FromHtml("#A53E25") },
        { 2, ColorTranslator.FromHtml("#2E7D5B") }
    };
        }

        private void CreateTable_CheckListCD()
        {
            _tblCheckListCD = new DataTable("tblCheckListCD");
            _tblCheckListCD.Columns.Add("ID", typeof(int));
            _tblCheckListCD.Columns.Add("ParentID", typeof(int));
            _tblCheckListCD.Columns.Add("ParentNO", typeof(int));
            _tblCheckListCD.Columns.Add("TenNhomCongDoan", typeof(string));
            _tblCheckListCD.Columns.Add("StyleID", typeof(string));
            _tblCheckListCD.Columns.Add("MaHang", typeof(string));
            _tblCheckListCD.Columns.Add("NO", typeof(string));
            _tblCheckListCD.Columns.Add("Description", typeof(string));
            _tblCheckListCD.Columns.Add("Note", typeof(string));
            _tblCheckListCD.Columns.Add("Tol", typeof(string));
            _tblCheckListCD.Columns.Add("Sort", typeof(int));
            _tblCheckListCD.Columns.Add("CreateDate", typeof(DateTime));
            _tblCheckListCD.Columns.Add("NVien", typeof(string));
            _tblCheckListCD.Columns.Add("Version", typeof(int));
            _tblCheckListCD.Columns.Add("Tol_Negative", typeof(string));
            _tblCheckListCD.Columns.Add("Donvi", typeof(string));
            _tblCheckListCD.Columns.Add("Note_TS", typeof(string));
            _tblCheckListCD.Columns.Add("MucDo", typeof(string));


            _tblCheckList_ThongSo = new DataTable("tblCheckListTS");
            _tblCheckList_ThongSo.Columns.Add("ID", typeof(int));
            _tblCheckList_ThongSo.Columns.Add("CongDoan", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("ParentID", typeof(int));
            _tblCheckList_ThongSo.Columns.Add("ParentNO", typeof(int));
            _tblCheckList_ThongSo.Columns.Add("SizeID", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("Size", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("Sort", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("StyleID", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("TenNhomCongDoan", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("DonViTS", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("MaCongDoan_ThongSo", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("ThongSo", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("MucDo", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("SizeTypeID", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("SizeType", typeof(string));
            _tblCheckList_ThongSo.Columns.Add("Version", typeof(int));
        }
        private void InitSearchLookThongSo_CheckList(string MaHang)
        {
            try
            {
                tblTVThongSoCheckList = new DataTable();
                string url = string.Format("{0}?action={1}&para1={2}", URL + "CheckListCD/Get", "GetTVThongSoCheckList", MaHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    tblTVThongSoCheckList = JsonConvert.DeserializeObject<DataTable>(json);
                }
                RepositoryItemSearchLookUpEdit rThongSoEdit = new RepositoryItemSearchLookUpEdit();
                rThongSoEdit.DataSource = tblTVThongSoCheckList;
                rThongSoEdit.DisplayMember = "CongDoan";
                rThongSoEdit.ValueMember = "MaCongDoan_ThongSo";
                rThongSoEdit.ShowClearButton = false;
                rThongSoEdit.NullText = "[Chọn Thông Số]";
                rThongSoEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
                rThongSoEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
                GridView dvView = rThongSoEdit.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "MaCongDoan_ThongSo", Caption = "MaCDTS", Name = "rColMaCDTS", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "CongDoan", Caption = "Thông Số Công Đoạn", Name = "rColCongDoan_ThongSo", Visible = true });

                }
                //repositoryItemTextEdit3


                colCongDoan_ckListTS.ColumnEdit = rThongSoEdit;
            }
            catch (Exception ex)
            {

            }

        }
        private void LoadSizeCheckList(string MaHang)
        {
            try
            {
                tblSize_CheckList = new DataTable();
                string url = string.Format("{0}?action={1}&para1={2}", URL + "CheckListCD/Get", "GetSize", MaHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblSize_CheckList = JsonConvert.DeserializeObject<DataTable>(json);


                }
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadCheckListCD()
        {
            try
            {

                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Lấy Dữ Liệu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
                btnImport_ckList.Enabled = true;
                CreateTable_CheckListCD();

                InitSearchLookThongSo_CheckList(MaHang);
                int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
                string urlGET = $"{URL}CheckListCD/GET?action=CheckUsing&para1={MaHang}&para2={version}";
                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}", URL + "CheckListCD/Get", "GetCheckList", MaHang, version);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                string urlThongSoNhomSize = $"{URL}CheckListCD/GET?action=GetNhomSizeThongSo&para1={MaHang}&para2={version}";
                string jsonThongSoNhomSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlThongSoNhomSize); }).Result;

                DataTable tblCheck = new DataTable();
                string urlTS = string.Format("{0}?action={1}&&para1={2}&&para2={3}", URL + "CheckListCD/Get", "GetThongSo", MaHang, version);
                string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;
                if (jsonTS != "[]")
                {

                    _tblCheckList_ThongSo = JsonConvert.DeserializeObject<DataTable>(jsonTS);

                }
                else
                {
                    PivotCheckListTS(new List<DataRow>());
                }
                if (json == "[]")
                {
                    grcCheckList_CD.DataSource = null;
                    grcChekListTS.DataSource = null;
                    grcCheckList_NhomSize.DataSource = null;
                    return;
                }
                if (jsonThongSoNhomSize == "[]")
                {
                    grcCheckList_NhomSize.DataSource = null;

                }
                else
                {
                    DataTable tblThongSo_NhomSize = JsonConvert.DeserializeObject<DataTable>(jsonThongSoNhomSize);
                    grcCheckList_NhomSize.DataSource = tblThongSo_NhomSize;
                }

                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                if (jsonCheck != "[]")
                {
                    tblCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                    if (tblCheck?.Rows?.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(tblCheck.Rows[0]["Msg"]?.ToString()) && tblCheck.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                        {
                            btnImport_ckList.Enabled = false;
                            btnImport_ckList.AppearanceDisabled.BackColor = Color.LightGray;
                            btnImport_ckList.AppearanceDisabled.ForeColor = Color.DarkGray;
                            btnImport_ckList.Refresh();

                        }
                    }

                }



                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                grcCheckList_CD.DataSource = tbl;
                grcCheckList_CD.RefreshDataSource();
                grvCheckList_CD.ExpandAllGroups();
                grvCheckList_CD.FocusedRowHandle = 0;



            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }
        private void BandedView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "TenNhomCongDoan" || e.Column.FieldName == "Sort")
            {
                BandedGridView view = sender as BandedGridView;

                int rowHandle1 = view.GetRowHandle(e.ListSourceRowIndex1);
                int rowHandle2 = view.GetRowHandle(e.ListSourceRowIndex2);


                string nhom1 = view.GetRowCellValue(rowHandle1, "TenNhomCongDoan")?.ToString() ?? "";
                string nhom2 = view.GetRowCellValue(rowHandle2, "TenNhomCongDoan")?.ToString() ?? "";
                int no1 = Convert.ToInt32(view.GetRowCellValue(rowHandle1, "Sort") ?? 0);
                int no2 = Convert.ToInt32(view.GetRowCellValue(rowHandle2, "Sort") ?? 0);


                int numNhom1 = ExtractLeadingNumber(nhom1);
                int numNhom2 = ExtractLeadingNumber(nhom2);


                if (numNhom1 != numNhom2)
                {
                    e.Result = numNhom1.CompareTo(numNhom2);
                }

                else if (nhom1 != nhom2)
                {
                    e.Result = string.Compare(nhom1, nhom2, StringComparison.Ordinal);
                }

                else
                {
                    e.Result = no1.CompareTo(no2);
                }

                e.Handled = true;
            }
        }
        private int ExtractLeadingNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
                return int.MaxValue;

            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(text, @"^(\d+)");

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return int.MaxValue;
        }
        private void LoadCheckListTS(int ParentNO)
        {
            try
            {
                if (_tblCheckList_ThongSo?.Rows.Count > 0)
                {
                    DataTable tbl = _tblCheckList_ThongSo.Copy();
                    string SizeTypeID = grvCheckList_NhomSize.GetFocusedRowCellValue(colSizeTypeID_CheckList)?.ToString();
                    var query = tbl.AsEnumerable().Where(x => Convert.ToInt32(x["ParentNO"]?.ToString()) == ParentNO && x["SizeTypeID"]?.ToString() == SizeTypeID);
                    if (query.Any())
                    {
                        //colCongDoan_ckListTS.OptionsColumn.AllowEdit = false;
                        //colCongDoan_ckListTS.OptionsColumn.ReadOnly = true;
                        PivotCheckListTS(query);
                    }
                    else
                    {
                        PivotCheckListTS(new List<DataRow>());
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in LoadCheckListTS: {ex.Message}");

            }
        }
        private void ReadFileExcel(string filePath, string sheetName, string maHang, int version)
        {


            string TenHang = gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<string> lstSizeTs = new List<string>();
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                    if (worksheet == null)
                    {
                        Console.WriteLine("File Excel không chứa dữ liệu hoặc không hợp lệ!");
                        return;
                    }
                    worksheet.Protection.AllowSelectLockedCells = true;
                    worksheet.Protection.AllowSelectUnlockedCells = true;

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    int colCount = worksheet.Dimension?.Columns ?? 0;
                    if (rowCount < 1)
                    {
                        Console.WriteLine("File Excel không có dữ liệu!");
                        return;
                    }

                    Regex regex = new Regex(@"(\d+)\.\s*(.+?)\s*(?=,\s*\d+\.|$)");
                    int parentNO = 0;
                    int ParentID = 0;
                    int rowStart = 1;
                    int SortCheckList = 0;
                    int SortSizeCD = 0;
                    string TenCongDoan = string.Empty;
                    List<dynamic> sizeHeaders = null;
                    string No = string.Empty;
                    for (int row = rowStart; row <= rowCount; row++)
                    {
                        var cell = worksheet.Cells[row, 1];
                        var NextCol = worksheet.Cells[row, 2];
                        var nextCell = worksheet.Cells[row + 1, 1];
                        string cellText = cell.Text?.Trim() ?? "";
                        string nextCellText = nextCell.Text?.Trim() ?? "";
                        string NextColText = NextCol.Text?.Trim() ?? "";
                        string sizeStart = worksheet.Cells[row, 3].Text?.Trim() ?? "";

                        if (cellText == "***" || cellText == "****") break;

                        
                        if (!string.IsNullOrEmpty(cellText) && (cellText.Contains("NO") || cellText.Contains("No."))) continue;
                        bool isMatch = Regex.IsMatch(cellText, @"^\d+(\.\d+)*\.\s+.+$");
                      
                        if (string.IsNullOrEmpty(cellText) && string.IsNullOrEmpty(NextColText) && string.IsNullOrEmpty(sizeStart))
                        {
                            SortSizeCD = 0;
                            sizeHeaders = null;
                            continue;
                        }

                        if (NextColText == "*")
                        {

                            sizeHeaders = new List<dynamic>();
                            for (int col = 3; col <= colCount; col++)
                            {
                                string header = trimText(worksheet.Cells[row, col].Text) ?? "";
                                if (!string.IsNullOrEmpty(header))
                                {
                                    SortSizeCD++;
                                    string sizeId = string.Empty;
                                    var Query = tblSize_CheckList.AsEnumerable().FirstOrDefault(x => x["Size"]?.ToString()?.ToUpper() == header.ToUpper());
                                    string SizeType = "0"; string SizeTypeID = "0";
                                    if (worksheet.Cells[row + 1, 2].Text?.ToUpper() == "NHÓM")
                                    {
                                        SizeType = trimText(worksheet.Cells[row + 1, col].Text) ?? "0";
                                        SizeTypeID = ReplaceSpecialCharacterssize(SizeType);
                                    }

                                    if (Query != null)
                                    {
                                        sizeId = Query["SizeID"]?.ToString();
                                    }
                                    else
                                    {
                                        bool checkCumCharacter = header.Contains('+');
                                        sizeId = "SIZE_" + ReplaceSpecialCharacterssize(trimText(header) + (checkCumCharacter ? "_" : ""));
                                        if (!lstSizeTs.Any(x => x == header))
                                        {

                                            lstSizeTs.Add($"•  Nhóm CD :<color=blue>{TenCongDoan}</color>- Size:<b><color=red>{header}</color></b>");
                                            //XtraMessageBox.Show($"Vui lòng khai báo thêm size {header} cho mã hàng {TenHang} ! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                                        }
                                    }

                                    sizeHeaders.Add(new { Size = header, Sort = SortSizeCD, SizeID = sizeId, colSizeIdx = col, SizeTypeID = SizeTypeID, SizeType = SizeType });
                                }

                            }
                            continue;
                        }

                        if (sizeHeaders != null && string.IsNullOrEmpty(cellText) && !string.IsNullOrEmpty(NextColText))
                        {
                            if (NextColText?.ToUpper() == "NHÓM")
                            {
                                continue;
                            }
                            else
                            {
                                string congDoan = trimText(NextColText);
                                for (int i = 0; i < sizeHeaders.Count; i++)
                                {
                                    Int32.TryParse(sizeHeaders[i].colSizeIdx?.ToString(), out int colSizeIdx);
                                    string thongSo = trimText(worksheet.Cells[row, colSizeIdx].Value?.ToString() ?? "");
                                    Color color = GetFontColor(worksheet.Cells[row, colSizeIdx]);
                                    string hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                                    if (!string.IsNullOrEmpty(thongSo))
                                    {
                                        string SizeName = trimText(sizeHeaders[i].Size.ToString());
                                        //bool checkCumCharacter = SizeName.Contains('+');
                                        string sizeValue = SizeName.ToUpper();

                                        DataRow rowCheckListTS = _tblCheckList_ThongSo.NewRow();
                                        //string sizeId = "SIZE_" + ReplaceSpecialCharacterssize(trimText(sizeValue) + (checkCumCharacter ? "_" : ""));
                                        string[] arrTs = thongSo.Split(new[] { '\"' }, 2, StringSplitOptions.None);

                                        string thongSoValue = arrTs[0].Trim();
                                        string donVi = "";

                                        if (arrTs.Length > 1)
                                        {
                                            string after = arrTs[1].Trim();
                                            donVi = string.IsNullOrEmpty(after) ? "INCH" : after;
                                        }

                                        rowCheckListTS["ThongSo"] = thongSoValue;
                                        rowCheckListTS["DonViTS"] = donVi;
                                        rowCheckListTS["ID"] = 0;
                                        rowCheckListTS["CongDoan"] = congDoan;
                                        rowCheckListTS["ParentID"] = ParentID;
                                        rowCheckListTS["ParentNO"] = 0;
                                        rowCheckListTS["Size"] = sizeValue;
                                        rowCheckListTS["SizeID"] = trimText(sizeHeaders[i].SizeID.ToString());
                                        rowCheckListTS["Sort"] = trimText(sizeHeaders[i].Sort.ToString());
                                        rowCheckListTS["StyleID"] = maHang;
                                        rowCheckListTS["TenNhomCongDoan"] = TenCongDoan;
                                        rowCheckListTS["MaCongDoan_ThongSo"] = null;
                                        rowCheckListTS["MucDo"] = hex;
                                        rowCheckListTS["SizeTypeID"] = trimText(sizeHeaders[i].SizeTypeID.ToString());
                                        rowCheckListTS["SizeType"] = trimText(sizeHeaders[i].SizeType.ToString());
                                        rowCheckListTS["Version"] = version;
                                        _tblCheckList_ThongSo.Rows.Add(rowCheckListTS);
                                    }
                                }
                                continue;
                            }


                        }

                        if (cell.Start.Row == row )
                        {

                            MatchCollection matches = regex.Matches(cellText);
                           
                            if (matches.Count > 0 && nextCellText.Contains("No"))
                            {
                                
                                SortCheckList = 0;
                                parentNO = row;
                                ParentID = row;
                                TenCongDoan = trimText(cellText);
                                sizeHeaders = null;
                                continue;
                            }
                            else
                            {
                                if (!isMatch && string.IsNullOrEmpty(TenCongDoan)) continue; 
                                if (string.IsNullOrEmpty(trimText(worksheet.Cells[row, 2].Text))) continue;
                                SortCheckList++;
                                if (!string.IsNullOrEmpty(cellText))
                                {
                                    No = trimText(worksheet.Cells[row, 1].Text);
                                }

                                DataRow rowCheckList = _tblCheckListCD.NewRow();
                                rowCheckList["ID"] = 0;
                                rowCheckList["ParentID"] = ParentID;
                                rowCheckList["ParentNO"] = 0;
                                rowCheckList["TenNhomCongDoan"] = TenCongDoan;
                                rowCheckList["StyleID"] = maHang;
                                rowCheckList["MaHang"] = TenHang;
                                rowCheckList["NO"] = No;
                                rowCheckList["Description"] = trimText(worksheet.Cells[row, 2].Text);
                                rowCheckList["Note"] = trimText(worksheet.Cells[row, 3].Text);
                                rowCheckList["Donvi"] = trimText(worksheet.Cells[row, 4].Text);

                                string rawText = worksheet.Cells[row, 5].Text;
                                string DungSize = trimText(rawText);
                                ParseTolerance(DungSize, out string tol, out string tolNeg);

                                rowCheckList["Tol"] = tol;
                                rowCheckList["Tol_Negative"] = tolNeg;
                                rowCheckList["Sort"] = SortCheckList;
                                rowCheckList["CreateDate"] = DateTime.Now;
                                rowCheckList["NVien"] = GlobleData.UserName;
                                rowCheckList["Version"] = version;
                                rowCheckList["Note_TS"] = trimText(worksheet.Cells[row, 6].Text);
                                var colorsInRow = new List<string>();

                                for (int i = 1; i <= 6; i++)
                                {
                                    Color color = GetFontColor(worksheet.Cells[row, i]);
                                    string hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                                    colorsInRow.Add(hex);
                                    if (i == 5)
                                    {
                                        colorsInRow.Add(hex);
                                    }
                                }

                                string MucDo = string.Join("@", colorsInRow);
                                rowCheckList["MucDo"] = MucDo;
                                _tblCheckListCD.Rows.Add(rowCheckList);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }


                DataSet dsSave = new DataSet();
                dsSave.Tables.Add(_tblCheckListCD);

                if (_tblCheckList_ThongSo.Rows.Count == 0)
                {
                    _tblCheckList_ThongSo.Rows.Add(null, null, null, null, null, null, null, null, null, null);
                }
                dsSave.Tables.Add(_tblCheckList_ThongSo);

                if (lstSizeTs.Count > 0)
                {
                    string htmlMessage = $"<b><color=red> Mã hàng {maHang} chưa có các size trong checklist: </color></b>\n\n" +
                                                        string.Join("\n", lstSizeTs);

                    DialogResult messResult = ShowWarning(htmlMessage);
                }
                string url = string.Format("{0}", URL + "CheckListCD/POST?action=ImportExcel");
                string jsonData = JsonConvert.SerializeObject(dsSave);
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    //LoadCheckListCD();
                    LoadVerCheckList();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc file Excel: {ex.Message}");
            }
        }
        private void ReadFileExcel(string filePath, string sheetName, string maHang, int ParentNO, int version)
        {
            int rowHandle_focused = grvCheckList_CD.FocusedRowHandle;

            string TenHang = gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<string> lstSizeTs = new List<string>();
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                    if (worksheet == null)
                    {
                        Console.WriteLine("File Excel không chứa dữ liệu hoặc không hợp lệ!");
                        return;
                    }
                    worksheet.Protection.AllowSelectLockedCells = true;
                    worksheet.Protection.AllowSelectUnlockedCells = true;

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    int colCount = worksheet.Dimension?.Columns ?? 0;
                    if (rowCount < 1)
                    {
                        Console.WriteLine("File Excel không có dữ liệu!");
                        return;
                    }

                    Regex regex = new Regex(@"(\d+)\.\s*(.+?)\s*(?=,\s*\d+\.|$)");
                    int parentNO = 0;
                    int ParentID = 0;
                    int rowStart = 1;
                    int SortCheckList = 0;
                    int SortSizeCD = 0;
                    string TenCongDoan = string.Empty;
                    List<dynamic> sizeHeaders = null;
                    string No = string.Empty;
                    for (int row = rowStart; row <= rowCount; row++)
                    {
                        var cell = worksheet.Cells[row, 1];
                        var NextCol = worksheet.Cells[row, 1];
                        var nextCell = worksheet.Cells[row + 1, 1];
                        string cellText = cell.Text?.Trim() ?? "";
                        string nextCellText = nextCell.Text?.Trim() ?? "";
                        string NextColText = NextCol.Text?.Trim() ?? "";
                        string sizeStart = worksheet.Cells[row, 2].Text?.Trim() ?? "";

                        if (cellText == "***" || cellText == "****") break;

                        //if (!string.IsNullOrEmpty(cellText) && (cellText.Contains("NO") || cellText.Contains("No."))) continue;

                        if (string.IsNullOrEmpty(cellText) && string.IsNullOrEmpty(NextColText) && string.IsNullOrEmpty(sizeStart))
                        {
                            SortSizeCD = 0;
                            sizeHeaders = null;
                            continue;
                        }

                        if (NextColText == "*")
                        {

                            sizeHeaders = new List<dynamic>();
                            for (int col = 2; col <= colCount; col++)
                            {
                                string header = trimText(worksheet.Cells[row, col].Text) ?? "";
                                if (!string.IsNullOrEmpty(header))
                                {
                                    SortSizeCD++;
                                    string sizeId = string.Empty;
                                    var Query = tblSize_CheckList.AsEnumerable().FirstOrDefault(x => x["Size"]?.ToString()?.ToUpper() == header.ToUpper());
                                    string SizeType = "0"; string SizeTypeID = "0";
                                    if (worksheet.Cells[row + 1, 1].Text?.ToUpper() == "NHÓM")
                                    {
                                        SizeType = trimText(worksheet.Cells[row + 1, col].Text) ?? "0";
                                        SizeTypeID = ReplaceSpecialCharacterssize(SizeType);
                                    }
                                    if (Query != null)
                                    {
                                        sizeId = Query["SizeID"]?.ToString();
                                    }
                                    else
                                    {
                                        bool checkCumCharacter = header.Contains('+');
                                        sizeId = "SIZE_" + ReplaceSpecialCharacterssize(trimText(header) + (checkCumCharacter ? "_" : ""));
                                        if (!lstSizeTs.Any(x => x == header))
                                        {

                                            lstSizeTs.Add($"•  Nhóm CD :<color=blue>{TenCongDoan}</color>- Size:<b><color=red>{header}</color></b>");
                                            //XtraMessageBox.Show($"Vui lòng khai báo thêm size {header} cho mã hàng {TenHang} ! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                                        }
                                    }

                                    sizeHeaders.Add(new { Size = header, Sort = SortSizeCD, SizeID = sizeId, colSizeIdx = col, SizeTypeID = SizeTypeID, SizeType = SizeType });
                                }

                            }
                            continue;
                        }

                        if (sizeHeaders != null && !string.IsNullOrEmpty(NextColText))
                        {
                            if (NextColText?.ToUpper() == "NHÓM")
                            {
                                continue;
                            }
                            else
                            {
                                string congDoan = trimText(NextColText);
                                for (int i = 0; i < sizeHeaders.Count; i++)
                                {
                                    Int32.TryParse(sizeHeaders[i].colSizeIdx?.ToString(), out int colSizeIdx);
                                    string thongSo = trimText(worksheet.Cells[row, colSizeIdx].Value?.ToString() ?? "");
                                    Color color = GetFontColor(worksheet.Cells[row, colSizeIdx]);
                                    string hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                                    if (!string.IsNullOrEmpty(thongSo))
                                    {
                                        string SizeName = trimText(sizeHeaders[i].Size.ToString());
                                        //bool checkCumCharacter = SizeName.Contains('+');
                                        string sizeValue = SizeName.ToUpper();

                                        DataRow rowCheckListTS = _tblCheckList_ThongSo.NewRow();
                                        //string sizeId = "SIZE_" + ReplaceSpecialCharacterssize(trimText(sizeValue) + (checkCumCharacter ? "_" : ""));
                                        string[] arrTs = thongSo.Split(new[] { '\"' }, 2, StringSplitOptions.None);

                                        string thongSoValue = arrTs[0].Trim();
                                        string donVi = "";

                                        if (arrTs.Length > 1)
                                        {
                                            string after = arrTs[1].Trim();
                                            donVi = string.IsNullOrEmpty(after) ? "INCH" : after;
                                        }

                                        rowCheckListTS["ThongSo"] = thongSoValue;
                                        rowCheckListTS["DonViTS"] = donVi;
                                        rowCheckListTS["ID"] = 0;
                                        rowCheckListTS["CongDoan"] = congDoan;
                                        rowCheckListTS["ParentID"] = ParentNO;
                                        rowCheckListTS["ParentNO"] = ParentNO;
                                        rowCheckListTS["Size"] = sizeValue;
                                        rowCheckListTS["SizeID"] = trimText(sizeHeaders[i].SizeID.ToString());
                                        rowCheckListTS["Sort"] = trimText(sizeHeaders[i].Sort.ToString());
                                        rowCheckListTS["StyleID"] = maHang;
                                        rowCheckListTS["TenNhomCongDoan"] = TenCongDoan;
                                        rowCheckListTS["MaCongDoan_ThongSo"] = null;
                                        rowCheckListTS["MucDo"] = hex;
                                        rowCheckListTS["SizeTypeID"] = trimText(sizeHeaders[i].SizeTypeID.ToString());
                                        rowCheckListTS["SizeType"] = trimText(sizeHeaders[i].SizeType.ToString());
                                        rowCheckListTS["Version"] = version;

                                        _tblCheckList_ThongSo.Rows.Add(rowCheckListTS);
                                    }
                                }
                                continue;
                            }


                        }


                    }
                }


                DataSet dsSave = new DataSet();
                if (_tblCheckListCD.Rows.Count == 0)
                {
                    _tblCheckListCD.Rows.Add(0, null, null, null, null, null, null, null, null, null, null, null, null, 0, 0, 0, 0, 0);
                }
                dsSave.Tables.Add(_tblCheckListCD);

                if (_tblCheckList_ThongSo.Rows.Count == 0)
                {
                    _tblCheckList_ThongSo.Rows.Add(null, null, null, null, null, null, null, null, null, null);
                }
                dsSave.Tables.Add(_tblCheckList_ThongSo);

                if (lstSizeTs.Count > 0)
                {
                    string htmlMessage = $"<b><color=red> Mã hàng {maHang} chưa có các size trong checklist: </color></b>\n\n" +
                                                        string.Join("\n", lstSizeTs);

                    DialogResult messResult = ShowWarning(htmlMessage);
                }
                string url = string.Format("{0}", URL + "CheckListCD/POST?action=ImportExcelTS");
                string jsonData = JsonConvert.SerializeObject(dsSave);
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadCheckListCD();
                    grvCheckList_CD.FocusedRowHandle = rowHandle_focused;
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc file Excel: {ex.Message}");
            }
        }
        void ParseTolerance(string input, out string tol, out string tolNeg)
        {
            tol = ""; tolNeg = "";
            if (string.IsNullOrWhiteSpace(input)) return;
            input = input.Trim().TrimStart('\'').TrimEnd('\"');

            // Tách đơn vị và text ra khỏi giá trị số
            string cleanInput = Regex.Replace(input, @"([0-9.,]+)\s*([a-zA-Z]+.*)", "$1").Trim();

            if (Regex.IsMatch(cleanInput, @"^[-+]?[0-9]+(\.[0-9]+)?$"))
            {
                if (cleanInput.StartsWith("+")) tol = cleanInput.Substring(1);
                else if (cleanInput.StartsWith("-")) tolNeg = cleanInput.Substring(1);
                else tol = cleanInput;
                return;
            }

            var sym = Regex.Match(cleanInput, @"^(\+/\-|±|\+\-|\-\+)\s*([^\s]+)$", RegexOptions.IgnoreCase);
            if (sym.Success)
            {
                string v = sym.Groups[2].Value;
                v = Regex.Replace(v, @"[a-zA-Z]+.*$", "").Trim();
                tol = v; tolNeg = v;
                return;
            }

            // Hỗ trợ nhiều cách viết: +x/-y, -x/+y, +x/y, x/-y, -x/y
            var range = Regex.Match(cleanInput, @"^([-+]?\s*[^/]+?)\s*/\s*([-+]?\s*[^/]+)$", RegexOptions.IgnoreCase);
            if (range.Success)
            {
                string first = range.Groups[1].Value.Trim();
                string second = range.Groups[2].Value.Trim();

                // Loại bỏ đơn vị và text
                first = Regex.Replace(first, @"[a-zA-Z]+.*$", "").Trim();
                second = Regex.Replace(second, @"[a-zA-Z]+.*$", "").Trim();

                // Xác định giá trị + và -
                bool firstIsPos = first.StartsWith("+") || (!first.StartsWith("-") && second.StartsWith("-"));
                bool secondIsNeg = second.StartsWith("-");

                string pos = firstIsPos ? first.TrimStart('+') : second.TrimStart('+');
                string neg = secondIsNeg ? second.TrimStart('-') : first.TrimStart('-');

                tol = pos;
                tolNeg = neg;
                return;
            }
        }
        private Color GetFontColor(ExcelRange cell)
        {
            var c = cell.Style.Font.Color;
            if (!string.IsNullOrEmpty(c.Rgb) && c.Rgb.Length == 8)
            {
                string hex = c.Rgb;
                int a = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int r = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                return Color.FromArgb(a, r, g, b);
            }

            if (c.Theme.HasValue)
            {
                int baseRgb = 0x000000;

                switch (c.Theme.Value)
                {

                    case eThemeSchemeColor.Accent1: baseRgb = 0x4F81BD; break;
                    case eThemeSchemeColor.Accent2: baseRgb = 0xC0504D; break;
                    case eThemeSchemeColor.Accent3: baseRgb = 0x9BBB59; break;
                    case eThemeSchemeColor.Accent4: baseRgb = 0x8064A2; break;
                    case eThemeSchemeColor.Accent5: baseRgb = 0x4BACC6; break;
                    case eThemeSchemeColor.Accent6: baseRgb = 0xF79646; break;
                    default: baseRgb = 0x000000; break;
                }

                Color baseColor = Color.FromArgb(
                    (baseRgb >> 16) & 255,
                    (baseRgb >> 8) & 255,
                    baseRgb & 255
                );


                if (c.Tint != null && c.Tint != 0m)
                {
                    return ApplyTint(baseColor, (double)c.Tint);
                }
                return baseColor;
            }


            if (c.Indexed >= 0 && c.Indexed < 64)
            {
                switch (c.Indexed)
                {
                    case 0: return Color.Black;
                    case 1: return Color.White;
                    case 2: return Color.Red;
                    case 3: return Color.Green;
                    case 4: return Color.Blue;
                    case 5: return Color.Yellow;
                    case 10: return Color.FromArgb(0, 97, 0);
                    default: return Color.Black;
                }
            }


            return Color.Black;
        }

        private Color ApplyTint(Color color, double tint)
        {
            double Lum(double v) => tint < 0 ? v * (1 + tint) : v * (1 - tint) + (1 - tint);
            return Color.FromArgb(
                (int)(Lum(color.R / 255.0) * 255),
                (int)(Lum(color.G / 255.0) * 255),
                (int)(Lum(color.B / 255.0) * 255)
            );
        }
        private string trimText(string txt)
        {
            return txt?.TrimEnd().TrimStart() ?? "";
        }

        private void PivotCheckListTS(IEnumerable<DataRow> query)
        {
            // SỬA: Kiểm tra null TRƯỚC khi dùng Any()
            if (query != null && query.Any())
            {

                var groupedData = query.Distinct().GroupBy(x => new
                {
                    CongDoan = x.Field<string>("CongDoan"),
                    Size = x.Field<string>("Size"),
                    SizeID = x.Field<string>("SizeID"),
                    SizeTypeID = x.Field<string>("SizeTypeID")

                })
                .Select(g => new
                {
                    CongDoan = g.Key.CongDoan,
                    Size = g.Key.Size,
                    SizeID = g.Key.SizeID,
                    MaxThongSo = g.Max(x => x.Field<string>("ThongSo"))
                }).Distinct().ToList();

                DataTable tblPivot = query.CopyToDataTable().DefaultView.ToTable(true, "CongDoan", "ParentID", "ParentNO", "StyleID", "TenNhomCongDoan");

                var sizes = tblSize_CheckList.AsEnumerable().OrderBy(x => x["Sort"]).ToList();
                foreach (var item in sizes)
                {
                    tblPivot.Columns.Add($"{item["SizeID"]}@Size@{item["Size"]}", typeof(string));
                }

                foreach (DataRow row in tblPivot.Rows)
                {
                    var maxValues = groupedData.Where(x => x.CongDoan == row["CongDoan"].ToString())
                                              .ToDictionary(x => $"{x.SizeID}@Size@{x.Size}", x => x.MaxThongSo);
                    foreach (var kvp in maxValues)
                    {
                        row[kvp.Key] = kvp.Value;
                    }
                }

                CreateBandGridSize_Detail(tblPivot, grvChekListTS, gbSize_ChkListTS);
                grcChekListTS.DataSource = tblPivot;
                grcChekListTS.RefreshDataSource();

                if (grvChekListTS.Columns["Sort"] != null)
                {
                    grvChekListTS.Columns["Sort"].Visible = false;
                }
            }
            else
            {
                // TRƯỜNG HỢP RỖNG HOẶC NULL → TẠO BẢNG TRỐNG CÓ CỘT
                DataTable tblPivot = new DataTable();

                tblPivot.Columns.Add("CongDoan", typeof(string));
                tblPivot.Columns.Add("ParentID", typeof(int));
                tblPivot.Columns.Add("ParentNO", typeof(string));
                tblPivot.Columns.Add("StyleID", typeof(int));
                tblPivot.Columns.Add("TenNhomCongDoan", typeof(string));

                // Thêm cột Size động (dù không có dữ liệu)
                var sizes = tblSize_CheckList.AsEnumerable().OrderBy(x => x["Sort"]).ToList();
                foreach (var item in sizes)
                {
                    tblPivot.Columns.Add($"{item["SizeID"]}@Size@{item["Size"]}", typeof(string));
                }

                ClearBand(gbSize_ChkListTS, grvChekListTS);
                CreateBandGridSize_Detail(tblPivot, grvChekListTS, gbSize_ChkListTS);
                grcChekListTS.DataSource = tblPivot;
                grcChekListTS.RefreshDataSource();


            }
        }

        private void RemoveColumnSize(BandedGridView BandedGridView_Detail)
        {
            for (int i = 0; i < BandedGridView_Detail.Columns.Count;)
            {
                if (BandedGridView_Detail.Columns[i].FieldName.Contains("@Size@"))
                {
                    BandedGridView_Detail.Columns.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }
        }

        private void ClearBand(GridBand GridBandSize, BandedGridView BandedGridView_Detail)
        {
            GridBandSize.Children.Clear();
            RemoveColumnSize(BandedGridView_Detail);
        }

        private bool CheckExistBand(string size, GridBand GridBandSize)
        {
            GridBand gbCheck = GridBandSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private void grvChekListTS_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            string SizeTypeID = grvCheckList_NhomSize.GetFocusedRowCellValue(colSizeTypeID_CheckList)?.ToString();
            string SizeType = grvCheckList_NhomSize.GetFocusedRowCellValue(colSizeType_CheckList)?.ToString();
            int focusedRowHandle = e.RowHandle;

            DataRow rowCK_CD = null;
            if (view.IsGroupRow(focusedRowHandle))
            {

                int childRowHandle = view.GetChildRowHandle(focusedRowHandle, 0);

                if (childRowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    DataRow row = view.GetDataRow(childRowHandle);
                    if (row != null)
                    {
                        rowCK_CD = row;
                    }
                }
            }
            else if (focusedRowHandle >= 0)
            {

                DataRow row = view.GetFocusedDataRow();
                if (row != null)
                {
                    rowCK_CD = row;
                }
            }

            if (view.FocusedColumn.FieldName.Contains("@Size@"))
            {
                string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
                if (string.IsNullOrEmpty(MaHang))
                {
                    XtraMessageBox.Show("Vui lòng chọn mã hàng để thêm dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (rowCK_CD == null) return;
                int.TryParse(rowCK_CD["ParentID"]?.ToString(), out int ParentID_focused);
                int.TryParse(rowCK_CD["ParentNO"]?.ToString(), out int ParentNO_focused);
                DataRow row_focused = view.GetFocusedDataRow();


                string[] parts = view.FocusedColumn.FieldName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                string sizeID = parts[0];
                string size = parts[1];

                if (string.IsNullOrEmpty(row_focused["CongDoan"]?.ToString()))
                {
                    XtraMessageBox.Show("Vui lòng chọn công đoạn thông số để thêm !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_tblCheckList_ThongSo.Rows.Count > 0)
                {
                    bool isUpdate = false;


                    if (row_focused != null)
                    {

                        foreach (DataRow row in _tblCheckList_ThongSo.Rows)
                        {
                            int.TryParse(row["ParentNO"]?.ToString(), out int ParentNO);
                            int.TryParse(row["ParentID"]?.ToString(), out int ParentID);
                            if (ParentNO_focused == ParentNO && ParentID_focused == ParentID && row["SizeTypeID"]?.ToString() == SizeTypeID &&
                                row["SizeType"]?.ToString() == SizeType &&
                                row["SizeID"]?.ToString() == sizeID && row["Size"]?.ToString() == size && row["CongDoan"].Equals(row_focused["CongDoan"]))
                            {
                                row["ThongSo"] = e.Value;
                                isUpdate = true;

                            }
                        }

                        if (!isUpdate)
                        {
                            DataRow row = _tblCheckList_ThongSo.NewRow();
                            row["ID"] = 0;
                            row["CongDoan"] = row_focused["CongDoan"];
                            row["ParentID"] = ParentID_focused;
                            row["ParentNO"] = ParentNO_focused;
                            row["Size"] = size;
                            row["SizeID"] = sizeID;
                            row["ThongSo"] = e.Value;
                            row["StyleID"] = MaHang;
                            row["SizeType"] = SizeType;
                            row["SizeTypeID"] = SizeTypeID;
                            row["MaCongDoan_ThongSo"] = row_focused["CongDoan"];

                            _tblCheckList_ThongSo.Rows.Add(row);
                        }
                        _tblCheckList_ThongSo.AcceptChanges();
                    }




                }
                else
                {
                    DataRow row = _tblCheckList_ThongSo.NewRow();
                    row["ID"] = 0;
                    row["CongDoan"] = row_focused["CongDoan"];
                    row["ParentID"] = ParentID_focused;
                    row["ParentNO"] = ParentNO_focused;
                    row["Size"] = size;
                    row["SizeID"] = sizeID;
                    row["ThongSo"] = e.Value;
                    row["StyleID"] = MaHang;
                    row["MaCongDoan_ThongSo"] = row_focused["CongDoan"];
                    row["SizeType"] = SizeType;
                    row["SizeTypeID"] = SizeTypeID;
                    _tblCheckList_ThongSo.Rows.Add(row);
                    _tblCheckList_ThongSo.AcceptChanges();
                }

            }

        }


        private int _lastFocusedHandle = GridControl.InvalidRowHandle;
        private DataTable GetDataTableFromGrid()
        {
            object ds = grvCheckList_CD.DataSource;

            if (ds is BindingSource bs)
                ds = bs.DataSource;

            if (ds is DataView dv)
                return dv.Table;

            if (ds is DataTable dt)
                return dt;

            return null;
        }

        private DataRow GetFirstDataRowInGroup(int groupRowHandle)
        {
            if (groupRowHandle < 0 && grvCheckList_CD.IsGroupRow(groupRowHandle))
            {
                int childCount = grvCheckList_CD.GetChildRowCount(groupRowHandle);
                if (childCount > 0)
                {
                    int firstChildHandle = grvCheckList_CD.GetChildRowHandle(groupRowHandle, 0);
                    return grvCheckList_CD.GetDataRow(firstChildHandle);
                }
            }
            else if (groupRowHandle >= 0)
            {
                return grvCheckList_CD.GetDataRow(groupRowHandle);
            }
            return null;
        }
        private int GetMaxNo(DataTable dt, object ParentNO)
        {
            int maxNo = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["NO"] == null || row["NO"] == DBNull.Value) continue;
                if (row["ParentNO"].Equals(ParentNO))
                {
                    if (int.TryParse(row["NO"].ToString(), out int no))
                    {
                        if (no > maxNo) maxNo = no;
                    }
                }

            }
            return maxNo;
        }
        private void btnAddCTCD_Click(object sender, EventArgs e)
        {
            try
            {
                string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
                if (string.IsNullOrEmpty(MaHang))
                {
                    XtraMessageBox.Show("Vui lòng chọn mã hàng để thêm.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
                int focusedHandle = grvCheckList_CD.FocusedRowHandle;
                if (focusedHandle == GridControl.InvalidRowHandle) return;


                DataTable dt = GetDataTableFromGrid();
                if (dt == null)
                {
                    //XtraMessageBox.Show("Không thể lấy dữ liệu từ Grid!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                DataRow srcRow = GetFirstDataRowInGroup(focusedHandle);
                if (srcRow == null)
                {
                    //XtraMessageBox.Show("Không tìm thấy dòng dữ liệu trong nhóm để copy.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                DataRow newRow = dt.NewRow();

                int newNo = 0;
                int sort = 0;
                if (Int32.TryParse(srcRow["NO"]?.ToString(), out int _valueNo))
                {
                    newNo = _valueNo;
                }
                else
                {
                    newNo = 1;
                }

                if (Int32.TryParse(srcRow["Sort"]?.ToString(), out int _valueSort))
                {
                    sort = _valueNo;
                }
                else
                {
                    sort = -1;
                }

                int maxNo = GetMaxNo(dt, srcRow["ParentNO"]);
                //int newNo = maxNo + 1;

                newRow["NO"] = newNo;
                newRow["Sort"] = sort + 1;
                string TenHang = srcRow["MaHang"]?.ToString() ?? gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
                newRow["StyleID"] = MaHang;
                newRow["MaHang"] = TenHang;
                newRow["ParentNO"] = srcRow["ParentNO"];
                newRow["ParentID"] = srcRow["ParentID"];
                newRow["TenNhomCongDoan"] = srcRow["TenNhomCongDoan"];
                newRow["ID"] = 0;
                newRow["MucDo"] = "#000000@#000000@#000000@#000000@#000000@#000000@#000000";
                newRow["Version"] = version;
                newRow["IsUse"] = false;
                string groupValue = srcRow["TenNhomCongDoan"]?.ToString() ?? "";
                int insertIndex = dt.Rows.Count;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string rowGroup = dt.Rows[i]["TenNhomCongDoan"]?.ToString() ?? "";
                    if (string.Equals(rowGroup, groupValue))
                    {
                        insertIndex = i;
                        break;
                    }
                }

                dt.Rows.InsertAt(newRow, insertIndex);



                int newRowHandle = grvCheckList_CD.LocateByValue(0, grvCheckList_CD.Columns["NO"], newNo);

                if (newRowHandle != GridControl.InvalidRowHandle)
                {
                    grvCheckList_CD.FocusedRowHandle = newRowHandle;
                    grvCheckList_CD.MakeRowVisible(newRowHandle);
                    grvCheckList_CD.ShowEditor();

                    // Mở nhóm
                    int groupHandle = grvCheckList_CD.GetParentRowHandle(newRowHandle);
                    if (groupHandle != GridControl.InvalidRowHandle)
                        grvCheckList_CD.ExpandGroupRow(groupHandle);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void CreateBandGridSize_Detail(DataTable dt, BandedGridView BandedGridView_Detail, GridBand GridBandSize)
        {
            ClearBand(GridBandSize, BandedGridView_Detail);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                string[] parts = colName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                var _sizeID = parts[0];
                var _size = parts[1];
                if (!CheckExistBand(_sizeID, GridBandSize)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = $"{_sizeID}@Size@{_size}";
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.ReadOnly = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                itemSize.FieldName = col.FieldName;
                itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                itemSize.DisplayFormat = "{0:n0}";
                itemSize.ShowInGroupColumnFooter = col;
                BandedGridView_Detail.GroupSummary.Add(itemSize);

                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1)
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
                BandedGridView_Detail.Columns.AddRange(new BandedGridColumn[] { col });
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
                GridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }

        private void grvCheckList_CD_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;

            int focusedRowHandle = e.FocusedRowHandle;


            if (view.IsGroupRow(focusedRowHandle))
            {

                int childRowHandle = view.GetChildRowHandle(focusedRowHandle, 0);

                if (childRowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    DataRow row = view.GetDataRow(childRowHandle);
                    if (row != null)
                    {
                        grvCheckList_NhomSize.FocusedRowHandle = 0;
                        ProcessRowData(row);
                    }
                }
            }
            else if (focusedRowHandle >= 0)
            {

                DataRow row = view.GetFocusedDataRow();
                if (row != null)
                {
                    grvCheckList_NhomSize.FocusedRowHandle = 0;
                    ProcessRowData(row);
                }
            }
        }
        private void grvCheckList_CD_DataSourceChanged(object sender, EventArgs e)
        {
            if (_tblCheckListCD?.Rows?.Count > 0)
            {
                DataRow row = grvCheckList_CD.GetDataRow(0);
                if (row == null) return;
                int.TryParse(row["ParentID"]?.ToString(), out int ParentID);
                int.TryParse(row["ParentNO"]?.ToString(), out int ParentNO);
                if (ParentID != -1)
                {
                    string SizeTypeID = grvCheckList_NhomSize.GetFocusedRowCellValue(colSizeTypeID_CheckList)?.ToString();
                    DataTable tblCopy = _tblCheckList_ThongSo.Copy();
                    var Query = tblCopy.AsEnumerable()
                        .Where(x => Convert.ToInt32(x["ParentID"]?.ToString() ?? "-1") == ParentID && x["SizeTypeID"]?.ToString() == SizeTypeID);
                    PivotCheckListTS(Query);
                    grvCheckList_NhomSize.FocusedRowHandle = 0;
                }
                else
                {

                    LoadCheckListTS(ParentNO);
                }
            }
        }
        // Tách logic xử lý row để tránh lặp code
        private void ProcessRowData(DataRow row)
        {


            int.TryParse(row["ParentNO"]?.ToString(), out int ParentNO);
            int.TryParse(row["ParentID"]?.ToString(), out int ParentID);

            if (ParentID != -1)
            {
                string SizeTypeID = grvCheckList_NhomSize.GetFocusedRowCellValue(colSizeTypeID_CheckList)?.ToString();
                DataTable tblCopy = _tblCheckList_ThongSo.Copy();
                var Query = tblCopy.AsEnumerable()
                    .Where(x => Convert.ToInt32(x["ParentID"]?.ToString() ?? "-1") == ParentID && x["SizeTypeID"]?.ToString() == SizeTypeID);
                PivotCheckListTS(Query);
            }
            else
            {
                LoadCheckListTS(ParentNO);
            }
        }

        private void GrvCheckList_NhomSize_DataSourceChanged(object sender, EventArgs e)
        {
            if (_tblCheckListCD?.Rows?.Count > 0)
            {
                DataRow row = grvCheckList_CD.GetFocusedDataRow();
                if (row == null) return;
                int.TryParse(row["ParentID"]?.ToString(), out int ParentID);
                int.TryParse(row["ParentNO"]?.ToString(), out int ParentNO);
                if (ParentID != -1)
                {
                    string SizeTypeID = grvCheckList_NhomSize.GetRowCellValue(0, colSizeTypeID_CheckList)?.ToString();
                    DataTable tblCopy = _tblCheckList_ThongSo.Copy();
                    var Query = tblCopy.AsEnumerable().Where(x => Convert.ToInt32(x["ParentID"]?.ToString()) == ParentID && x["SizeTypeID"]?.ToString() == SizeTypeID);
                    PivotCheckListTS(Query);
                }
                else
                {

                    LoadCheckListTS(ParentNO);
                }
            }
        }

        private void GrvCheckList_NhomSize_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {



            int focusedRowHandle = grvCheckList_CD.FocusedRowHandle;

            if (grvCheckList_CD.IsGroupRow(focusedRowHandle))
            {

                int childRowHandle = grvCheckList_CD.GetChildRowHandle(focusedRowHandle, 0);

                if (childRowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    DataRow row = grvCheckList_CD.GetDataRow(childRowHandle);
                    if (row != null)
                    {
                        ProcessRowData(row);
                    }
                }
            }
            else if (focusedRowHandle >= 0)
            {

                DataRow row = grvCheckList_CD.GetDataRow(focusedRowHandle);
                if (row != null)
                {
                    ProcessRowData(row);
                }
            }
        }
        private void grvThongSoCheckList_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            int rowHandle = view.FocusedRowHandle;
            //if (rowHandle < 0) return;
            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;

            if (col_focused == colCongDoan_ckListTS)
            {
                string newValue = e.Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(newValue) || string.IsNullOrWhiteSpace(newValue))
                {
                    e.Valid = false;
                    e.ErrorText = $"{col_focused.Caption} không được bỏ trống!";
                    return;
                }


                if (view.DataRowCount > 0 &&
                    Enumerable.Range(0, view.DataRowCount)
                        .Where(i => i != rowHandle)
                        .Any(i =>
                        {
                            string existingValue = view.GetRowCellValue(i, col_focused)?.ToString()?.Trim();
                            return !string.IsNullOrEmpty(existingValue) &&
                                   string.Equals(existingValue, newValue, StringComparison.OrdinalIgnoreCase);
                        }))
                {
                    e.Valid = false;
                    e.ErrorText = $"{col_focused.Caption} đã tồn tại! Vui lòng nhập công đoạn thông số khác.";
                    return;
                }


                e.Valid = true;
                e.ErrorText = string.Empty;

            }
        }
        private void grvNhomCDCheckList_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }

        private void GridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == colTenNhomCongDoan_ChkListCD)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void BandedGridView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;

            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);
            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {
                ControlPaint.DrawBorder3D(e.Graphics, rect);
                rect.Inflate(-1, -1);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color backColor = ColorTranslator.FromHtml("#FFD480");
                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }
                }

                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                    e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.None,
                            FormatFlags = StringFormatFlags.LineLimit
                        };
                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }
                }

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
        private void grvCheckList_CD_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.RowHandle >= 0)
            {
                string MucDo = view.GetRowCellValue(e.RowHandle, colMucDo)?.ToString();
                if (string.IsNullOrEmpty(MucDo)) return;
                string[] arrMucDo = MucDo?.Split('@');
                if (arrMucDo.Length > 0)
                {
                    int columnIndex = e.Column.VisibleIndex;
                    if (columnIndex >= 0 && columnIndex < arrMucDo.Length)
                    {
                        string colorStr = arrMucDo[columnIndex].Trim();
                        Color foreColor = ColorTranslator.FromHtml(colorStr);

                        // Gán màu chữ
                        e.Appearance.ForeColor = foreColor;

                        // In đậm chữ (giữ nguyên style khác nếu có)
                        //Font currentFont = e.Appearance.Font;
                        //e.Appearance.Font = new Font(currentFont, FontStyle.Bold);
                    }
                }
            }
        }
        private void grvCheckListTS_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;
                if (e.RowHandle >= 0 && e.Column.FieldName.Contains("@Size@"))
                {
                    string[] parts = e.Column.FieldName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                    string sizeID = parts[0];
                    string size = parts[1];
                    DataRow rowFocused = view.GetDataRow(e.RowHandle) as DataRow;
                    if (rowFocused == null) return;
                    string SizeTypeID = grvCheckList_NhomSize.GetFocusedRowCellValue(colSizeTypeID_CheckList)?.ToString();
                    DataTable tblCopy = _tblCheckList_ThongSo.Copy();

                    var query = _tblCheckList_ThongSo.AsEnumerable().FirstOrDefault(x => x["SizeID"]?.ToString() == sizeID && x["MaCongDoan_ThongSo"]?.ToString() == rowFocused["CongDoan"]?.ToString() && x["SizeTypeID"]?.ToString() == SizeTypeID
                    && (x["ParentNO"]?.ToString() == rowFocused["ParentNO"]?.ToString() || x["ParentID"]?.ToString() == rowFocused["ParentID"]));

                    if (query != null)
                    {
                        string colorStr = query["MucDo"]?.ToString();
                        Color foreColor = ColorTranslator.FromHtml(colorStr);
                        e.Appearance.ForeColor = foreColor;
                        Font currentFont = e.Appearance.Font;
                        e.Appearance.Font = new Font(currentFont, FontStyle.Bold);
                    }

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnUP_Click(object sender, EventArgs e)
        {
            if (grvCheckList_CD == null || grvCheckList_CD.FocusedRowHandle < 0)
                return;

            var view = grvCheckList_CD;
            view.BeginUpdate();
            view.BeginSort();
            try
            {
                var currentRow = view.GetDataRow(view.FocusedRowHandle);
                if (currentRow == null)
                    return;

                if (!long.TryParse(currentRow["ParentNO"]?.ToString(), out long maClvt))
                    return;

                var currentId = currentRow["ID"]?.ToString(); // Chuyển thành string để so sánh
                if (string.IsNullOrEmpty(currentId))
                    return;

                DataTable table = null;
                if (view.DataSource is DataView dataView)
                    table = dataView.Table;
                else if (view.DataSource is DataTable dataTable)
                    table = dataTable;

                if (table == null)
                    return;

                // Lấy tất cả rows trong cùng group và sort theo Sort hiện tại
                var groupRows = table.AsEnumerable()
                    .Where(r => long.TryParse(r["ParentNO"]?.ToString(), out long parentNo) && parentNo == maClvt)
                    .OrderBy(r =>
                    {
                        long.TryParse(r["Sort"]?.ToString(), out long sort);
                        return sort;
                    })
                    .ToList();

                if (groupRows.Count < 2)
                    return;

                // Tìm vị trí hiện tại - so sánh bằng string để tránh lỗi type mismatch
                int currentIndex = groupRows.FindIndex(r => r["ID"]?.ToString() == currentId);

                // Debug: Kiểm tra nếu không tìm thấy
                if (currentIndex < 0)
                {
                    XtraMessageBox.Show($"Không tìm thấy row với ID = {currentId}", "Debug",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentIndex == 0) // Đã ở đầu rồi
                    return;

                // Swap vị trí trong list
                var temp = groupRows[currentIndex];
                groupRows[currentIndex] = groupRows[currentIndex - 1];
                groupRows[currentIndex - 1] = temp;

                // Cập nhật lại Sort cho toàn bộ group theo vị trí mới (1, 2, 3, 4...)
                for (int i = 0; i < groupRows.Count; i++)
                {
                    long newSort = i + 1;
                    var rowId = groupRows[i]["ID"];
                    var rowHandle = view.LocateByValue("ID", rowId);

                    if (rowHandle >= 0)
                        view.SetRowCellValue(rowHandle, "Sort", newSort);
                    else
                        groupRows[i]["Sort"] = newSort;
                }

                // Đảm bảo column Sort được sort
                var sortColumn = view.Columns["Sort"];
                if (sortColumn != null && sortColumn.SortOrder == DevExpress.Data.ColumnSortOrder.None)
                    view.SortInfo.AddRange(new[] {
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo(sortColumn, DevExpress.Data.ColumnSortOrder.Ascending)
            });

                view.UpdateCurrentRow();
                view.RefreshData();

                // Focus lại row đã move
                var newHandle = view.LocateByValue("ID", currentId);
                if (newHandle >= 0)
                {
                    view.FocusedRowHandle = newHandle;
                    view.MakeRowVisible(newHandle);
                    view.ClearSelection();
                    view.SelectRow(newHandle);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi di chuyển lên: {ex.Message}\n\nStackTrace: {ex.StackTrace}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                view.EndSort();
                view.EndUpdate();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (grvCheckList_CD == null || grvCheckList_CD.FocusedRowHandle < 0)
                return;

            var view = grvCheckList_CD;
            view.BeginUpdate();
            view.BeginSort();
            try
            {
                var currentRow = view.GetDataRow(view.FocusedRowHandle);
                if (currentRow == null)
                    return;

                if (!long.TryParse(currentRow["ParentNO"]?.ToString(), out long maClvt))
                    return;

                var currentId = currentRow["ID"]?.ToString(); // Chuyển thành string để so sánh
                if (string.IsNullOrEmpty(currentId))
                    return;

                DataTable table = null;
                if (view.DataSource is DataView dataView)
                    table = dataView.Table;
                else if (view.DataSource is DataTable dataTable)
                    table = dataTable;

                if (table == null)
                    return;

                // Lấy tất cả rows trong cùng group và sort theo Sort hiện tại
                var groupRows = table.AsEnumerable()
                    .Where(r => long.TryParse(r["ParentNO"]?.ToString(), out long parentNo) && parentNo == maClvt)
                    .OrderBy(r =>
                    {
                        long.TryParse(r["Sort"]?.ToString(), out long sort);
                        return sort;
                    })
                    .ToList();

                if (groupRows.Count < 2)
                    return;

                // Tìm vị trí hiện tại - so sánh bằng string để tránh lỗi type mismatch
                int currentIndex = groupRows.FindIndex(r => r["ID"]?.ToString() == currentId);

                // Debug: Kiểm tra nếu không tìm thấy
                if (currentIndex < 0)
                {
                    XtraMessageBox.Show($"Không tìm thấy row với ID = {currentId}", "Debug",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentIndex >= groupRows.Count - 1) // Đã ở cuối rồi
                    return;

                // Swap vị trí trong list
                var temp = groupRows[currentIndex];
                groupRows[currentIndex] = groupRows[currentIndex + 1];
                groupRows[currentIndex + 1] = temp;

                // Cập nhật lại Sort cho toàn bộ group theo vị trí mới (1, 2, 3, 4...)
                for (int i = 0; i < groupRows.Count; i++)
                {
                    long newSort = i + 1;
                    var rowId = groupRows[i]["ID"];
                    var rowHandle = view.LocateByValue("ID", rowId);

                    if (rowHandle >= 0)
                        view.SetRowCellValue(rowHandle, "Sort", newSort);
                    else
                        groupRows[i]["Sort"] = newSort;
                }

                // Đảm bảo column Sort được sort
                var sortColumn = view.Columns["Sort"];
                if (sortColumn != null && sortColumn.SortOrder == DevExpress.Data.ColumnSortOrder.None)
                    view.SortInfo.AddRange(new[] {
                new DevExpress.XtraGrid.Columns.GridColumnSortInfo(sortColumn, DevExpress.Data.ColumnSortOrder.Ascending)
            });

                view.UpdateCurrentRow();
                view.RefreshData();

                // Focus lại row đã move
                var newHandle = view.LocateByValue("ID", currentId);
                if (newHandle >= 0)
                {
                    view.FocusedRowHandle = newHandle;
                    view.MakeRowVisible(newHandle);
                    view.ClearSelection();
                    view.SelectRow(newHandle);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi di chuyển xuống: {ex.Message}\n\nStackTrace: {ex.StackTrace}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                view.EndSort();
                view.EndUpdate();
            }
        }
        private void btnAddSize_TS_Click(object sender, EventArgs e)
        {
            string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
            string TenHang = gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
            DataRow row = grvCheckList_CD.GetFocusedDataRow();
            if (row == null) return;
            int.TryParse(row["ParentNO"]?.ToString(), out int ParentNO);
            if (string.IsNullOrEmpty(MaHang))
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng để thêm !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmAddThongSoCheckList frm = new frmAddThongSoCheckList(MaHang, TenHang);
            frm.ShowDialog();

            LoadCheckListTS(ParentNO);
            InitSearchLookThongSo_CheckList(MaHang);
        }
        private void btnAddCD_CheckList_Click(object sender, EventArgs e)
        {
            string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
            string TenHang = gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
            if (string.IsNullOrEmpty(MaHang))
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng để  thêm.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
            frmAddCongDoanChecklist frm = new frmAddCongDoanChecklist(MaHang, TenHang, version);
            frm.ShowDialog();
            LoadCheckListCD();
        }
        private void btnRefresh_ckList_Click(object sender, EventArgs e)
        {
            //LoadCheckListCD();
            LoadVerCheckList();
        }
        private void btnXoaCheckList_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
            if (string.IsNullOrEmpty(MaHang))
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string TenHang = gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
            int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
            DataTable tbl = new DataTable();

            msg = $"Bạn có muốn xóa dữ liệu mã hàng {TenHang} không?";
            string urlGET = $"{URL}CheckListCD/GET?action=CheckUsing&para1={MaHang}&para2={version}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;

            if (json != "[]")
            {
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl?.Rows?.Count > 0)
                {
                    if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                    {
                        //XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //return;

                        msg = $"Bạn có muốn tiếp tục xóa dữ liệu mã hàng {TenHang} đã có kiểm hay không?";
                    }
                }

            }

            DialogResult messResult = MessageBox.Show(
                msg,
                 "Thông báo",
                 MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question);

            if (messResult != DialogResult.Yes) return;


            string url = $"{URL}CheckListCD/Delete?action=Delete&Para1={MaHang}&Para2={version}&Para3={GlobleData.UserName}";
            string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 1000);
            }

            LoadVerCheckList();
        }
        private void btnImport_ckList_Click(object sender, EventArgs e)
        {

            CreateTable_CheckListCD();
            string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
            if (string.IsNullOrEmpty(MaHang))
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng để Import dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tblSize_CheckList.Rows.Count == 0)
            {
                XtraMessageBox.Show("Vui lòng khai báo thêm size để Import dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmImportExcel frm = new frmImportExcel();
            frm.ShowDialog();
            frm.FormClosed -= Fr_FormClosed;
            int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
            if (version == 0) version = 1;
            ReadFileExcel(frm.Path, frm.SheetName, MaHang, version);


        }

        private void btnSave_ckList_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = searchLookUpEditLocMH;
                int rowHandle_focused = grvCheckList_CD.FocusedRowHandle;
                DataSet dsSave = new DataSet();
                if (_tblCheckListCD == null || _tblCheckList_ThongSo == null || _tblCheckList_ThongSo.Rows.Count == 0)
                {
                    CreateTable_CheckListCD();
                }

                DataTable tblSaveCheckListCD = _tblCheckListCD.Clone();
                tblSaveCheckListCD.TableName = "tblCheckListCD";
                DataTable tblSaveCheckListTS = _tblCheckList_ThongSo.Clone();
                tblSaveCheckListTS.TableName = "tblCheckListTS";

                if (tblSaveCheckListTS.Columns.Contains("IsUse"))
                {
                    tblSaveCheckListTS.Columns.Remove("IsUse");
                }
                int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
                if (version == 0) version = 1;

                //DataTable dtCheckListCD = grcCheckList_CD.DataSource as DataTable;
                //if (dtCheckListCD != null)
                //{
                //    foreach (DataRow row in dtCheckListCD.Rows)
                //    {
                //        DataRow newRow = tblSaveCheckListCD.NewRow();
                //        // Copy fields, handling nulls and ensuring type safety
                //        newRow["ID"] = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0;
                //        newRow["ParentID"] = row["ParentID"] != DBNull.Value ? Convert.ToInt32(row["ParentID"]) : 0;
                //        newRow["ParentNO"] = row["ParentNO"] != DBNull.Value ? Convert.ToInt32(row["ParentNO"]) : 0;
                //        newRow["TenNhomCongDoan"] = row["TenNhomCongDoan"]?.ToString() ?? "";
                //        newRow["StyleID"] = row["StyleID"]?.ToString() ?? "";
                //        newRow["MaHang"] = row["MaHang"]?.ToString() ?? "";
                //        newRow["NO"] = row["NO"]?.ToString() ?? "";
                //        newRow["Description"] = row["Description"]?.ToString() ?? "";
                //        newRow["Note"] = row["Note"]?.ToString() ?? "";
                //        newRow["Tol"] = row["Tol"]?.ToString() ?? "";
                //        newRow["Sort"] = row["Sort"] != DBNull.Value ? Convert.ToInt64(row["Sort"]) : 0;
                //        newRow["CreateDate"] = DateTime.Now;
                //        newRow["NVien"] = GlobleData.UserName;
                //        newRow["Version"] = VersionChkList;
                //        newRow["Tol_Negative"] = row["Tol_Negative"]?.ToString() ?? "";
                //        newRow["Donvi"] = row["Donvi"]?.ToString().ToUpper() ?? "";
                //        newRow["Note_TS"] = row["Note_TS"]?.ToString() ?? "";
                //        newRow["MucDo"] = row["MucDo"]?.ToString() ?? "";
                //        tblSaveCheckListCD.Rows.Add(newRow);
                //    }
                //}
                DataTable dtCheckListCD = grcCheckList_CD.DataSource as DataTable;
                if (dtCheckListCD != null)
                {
                    // Lấy tất cả rows và sort
                    var allRows = dtCheckListCD.AsEnumerable()
                        .Select(r => new
                        {
                            Row = r,
                            ParentNO = r["ParentNO"]?.ToString() ?? "",
                            TenNhom = r["TenNhomCongDoan"]?.ToString() ?? "",
                            SortNum = GetSortValue(r)
                        })
                        .OrderBy(x => ExtractLeadingNumber(x.TenNhom))
                        .ThenBy(x => x.TenNhom)
                        .ThenBy(x => x.SortNum)
                        .ToList();

                    // Dictionary để track Sort position cho mỗi ParentNO
                    Dictionary<string, int> sortCounters = new Dictionary<string, int>();

                    foreach (var item in allRows)
                    {
                        DataRow row = item.Row;
                        string parentNoKey = item.ParentNO;

                        // Tăng counter cho ParentNO này
                        if (!sortCounters.ContainsKey(parentNoKey))
                            sortCounters[parentNoKey] = 0;
                        sortCounters[parentNoKey]++;

                        DataRow newRow = tblSaveCheckListCD.NewRow();

                        // Copy fields với TryParse an toàn
                        newRow["ID"] = int.TryParse(row["ID"]?.ToString(), out int id) ? id : 0;
                        newRow["ParentID"] = int.TryParse(row["ParentID"]?.ToString(), out int parentId) ? parentId : 0;
                        newRow["ParentNO"] = int.TryParse(row["ParentNO"]?.ToString(), out int parentNo) ? parentNo : 0;
                        newRow["TenNhomCongDoan"] = row["TenNhomCongDoan"]?.ToString() ?? "";
                        newRow["StyleID"] = row["StyleID"]?.ToString() ?? "";
                        newRow["MaHang"] = row["MaHang"]?.ToString() ?? "";
                        newRow["NO"] = row["NO"]?.ToString() ?? "";
                        newRow["Description"] = row["Description"]?.ToString() ?? "";
                        newRow["Note"] = row["Note"]?.ToString() ?? "";
                        newRow["Tol"] = row["Tol"]?.ToString() ?? "";

                        // Sort = vị trí trong group
                        newRow["Sort"] = sortCounters[parentNoKey];

                        newRow["CreateDate"] = DateTime.Now;
                        newRow["NVien"] = GlobleData.UserName;
                        newRow["Version"] = version;
                        newRow["Tol_Negative"] = row["Tol_Negative"]?.ToString() ?? "";
                        newRow["Donvi"] = row["Donvi"]?.ToString().ToUpper() ?? "";
                        newRow["Note_TS"] = row["Note_TS"]?.ToString() ?? "";
                        newRow["MucDo"] = row["MucDo"]?.ToString() ?? "";

                        tblSaveCheckListCD.Rows.Add(newRow);
                    }
                }
                DataTable dtCheckListTS = _tblCheckList_ThongSo;
                if (dtCheckListTS != null)
                {
                    foreach (DataRow pivotRow in dtCheckListTS.Rows)
                    {
                        string congDoan = pivotRow["CongDoan"]?.ToString() ?? "";
                        int parentID = pivotRow["ParentID"] != DBNull.Value ? Convert.ToInt32(pivotRow["ParentID"]) : 0;
                        int parentNO = pivotRow["ParentNO"] != DBNull.Value ? Convert.ToInt32(pivotRow["ParentNO"]) : 0;
                        string styleID = pivotRow["StyleID"]?.ToString() ?? "";
                        string tenNhomCongDoan = pivotRow["TenNhomCongDoan"]?.ToString() ?? "";

                        DataRow newRow = tblSaveCheckListTS.NewRow();
                        newRow["ID"] = 0; // Assuming ID is assigned by the server
                        newRow["CongDoan"] = congDoan;
                        newRow["ParentID"] = parentID;
                        newRow["ParentNO"] = parentNO;
                        newRow["Size"] = pivotRow["Size"];
                        newRow["SizeID"] = pivotRow["SizeID"];
                        newRow["ThongSo"] = pivotRow["ThongSo"];
                        newRow["Sort"] = pivotRow["Sort"]; // Adjust if Sort is available in DataSource
                        newRow["StyleID"] = styleID;
                        newRow["TenNhomCongDoan"] = tenNhomCongDoan;
                        newRow["DonViTS"] = pivotRow["DonViTS"];
                        newRow["MaCongDoan_ThongSo"] = pivotRow["MaCongDoan_ThongSo"];
                        newRow["SizeType"] = pivotRow["SizeType"];
                        newRow["SizeTypeID"] = pivotRow["SizeTypeID"];
                        newRow["Version"] = version;
                        tblSaveCheckListTS.Rows.Add(newRow);
                    }
                }

                if (tblSaveCheckListCD?.Rows?.Count == 0 && tblSaveCheckListTS?.Rows?.Count == 0)
                {
                    XtraMessageBox.Show($"Chưa có dữ liệu để lưu !!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                dsSave.Tables.Add(tblSaveCheckListCD);
                if (tblSaveCheckListTS.Rows.Count == 0)
                {
                    tblSaveCheckListTS.Rows.Add(null, null, null, null, null, null, null, null, null, null);
                }
                dsSave.Tables.Add(tblSaveCheckListTS);



                string url = string.Format("{0}", URL + "CheckListCD/POST?action=POST");
                string jsonData = JsonConvert.SerializeObject(dsSave);
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadCheckListCD();
                    grvCheckList_CD.FocusedRowHandle = rowHandle_focused;
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Hàm helper để lấy giá trị Sort
        private long GetSortValue(DataRow row)
        {
            long.TryParse(row["Sort"]?.ToString(), out long sort);
            return sort;
        }

        private void btnDeleteCTCD_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable tblThongSo = grcCheckList_CD.DataSource as DataTable;
                if (tblThongSo == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvCheckList_CD.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvCheckList_CD.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    //Nếu cần kiểm tra dữ liệu dùng lại, bạn bật đoạn này lên
                    //if (dr["IsUse"]?.ToString()?.ToLower() == "true")
                    //{
                    //    XtraMessageBox.Show($"Công đoạn chi tiết {dr["Description"]?.ToString()} này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    continue;
                    //}


                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}CheckListCD/Delete?action=DeleteCDChiTiet&Para1={dr["ID"]}&Para2={GlobleData.UserName}";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }

                    // Xóa dòng trong DataTable
                    tblThongSo.Rows.Remove(dr);
                }

                grcCheckList_CD.DataSource = tblThongSo;
                grcCheckList_CD.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnDeleteTSCD_Click(object sender, EventArgs e)
        {
            try
            {
                string SizeTypeID = grvCheckList_NhomSize.GetFocusedRowCellValue(colSizeTypeID_CheckList)?.ToString();
                string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
                string TenHang = gridViewHangHoa.GetFocusedRowCellValue(colTenHang)?.ToString();
                DataRow row = grvCheckList_CD.GetFocusedDataRow();
                if (row == null) return;
                int.TryParse(row["ParentNO"]?.ToString(), out int ParentNO);
                DataTable tblThongSo = grcChekListTS.DataSource as DataTable;
                if (tblThongSo == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvChekListTS.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvChekListTS.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    //Nếu cần kiểm tra dữ liệu dùng lại, bạn bật đoạn này lên
                    //if (dr["IsUse"]?.ToString()?.ToLower() == "true")
                    //{
                    //    XtraMessageBox.Show($"Công đoạn thông số {dr["CongDoan_ThongSo"]?.ToString()} này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    continue;
                    //}


                    if (!string.IsNullOrEmpty(dr["CongDoan"].ToString()))
                    {
                        string url = $"{URL}CheckListCD/Delete?action=DeleteThongSo&Para1={dr["CongDoan"]}&Para2={ParentNO}&Para3={SizeTypeID}&Para4={GlobleData.UserName}";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }

                    // Xóa dòng trong DataTable
                    tblThongSo.Rows.Remove(dr);
                }
                int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
                string urlTS = string.Format("{0}?action={1}&&para1={2}&&para2={3}", URL + "CheckListCD/Get", "GetThongSo", MaHang, version);
                string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;
                if (jsonTS != "[]")
                {

                    _tblCheckList_ThongSo = JsonConvert.DeserializeObject<DataTable>(jsonTS);

                }
                else
                {
                    PivotCheckListTS(new List<DataRow>());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DialogResult ShowWarning(string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs
            {
                Caption = Resources.Warning,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Text = message,


                Buttons = new[] { DialogResult.OK },

                Icon = System.Drawing.SystemIcons.Warning,
                MessageBeepSound = MessageBeepSound.Warning,
                DefaultButtonIndex = 0
            };

            return XtraMessageBox.Show(args);
        }

        private void BtnInportSizeDoTS_CheckList_Click(object sender, EventArgs e)
        {

            DataRow srcRow = GetFirstDataRowInGroup(grvCheckList_CD.FocusedRowHandle);
            if (srcRow == null)
            {

                return;
            }

            int.TryParse(srcRow["ParentNO"]?.ToString(), out int ParentNO);


            CreateTable_CheckListCD();
            string MaHang = gridViewHangHoa.GetFocusedRowCellValue(colMaHang)?.ToString();
            if (string.IsNullOrEmpty(MaHang))
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng để Import dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tblSize_CheckList.Rows.Count == 0)
            {
                XtraMessageBox.Show("Vui lòng khai báo thêm size để Import dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmImportExcel frm = new frmImportExcel();
            frm.ShowDialog();
            frm.FormClosed -= Fr_FormClosed;
            int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
            ReadFileExcel(frm.Path, frm.SheetName, MaHang, ParentNO, version);
        }

        private void gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Column.AppearanceHeader.GradientMode);
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
        string _stylteIDCopy = string.Empty;
        int _versionCopy = 0;
        private void CopyThongSoCheckList()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            _stylteIDCopy = entityHangHoa.MaHang;
            int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
            _versionCopy = version;

        }
        private void PasteThongSoCheckList()
        {


            try
            {
                var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (entityHangHoa is null) return;
                string stylteIDPaste = entityHangHoa.MaHang;
                int.TryParse(searchLookUpEdit_VerSionCheckLIst?.EditValue?.ToString(), out int version);
                string urlGET = $"{URL}CheckListCD/GET?action=CheckUsing&para1={stylteIDPaste}&para1={version}";
                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}", URL + "CheckListCD/Get", "GetCheckList", _stylteIDCopy, _versionCopy);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


                DataTable tblCheck = new DataTable();
                string urlTS = string.Format("{0}?action={1}&&para1={2}&&para2={3}", URL + "CheckListCD/Get", "GetThongSo", _stylteIDCopy, _versionCopy);
                string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;
                if (jsonTS != "[]")
                {

                    _tblCheckList_ThongSo = JsonConvert.DeserializeObject<DataTable>(jsonTS);

                }


                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                if (jsonCheck != "[]")
                {
                    tblCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                    if (tblCheck?.Rows?.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(tblCheck.Rows[0]["Msg"]?.ToString())
                        && tblCheck.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                        {
                            // Hiện cảnh báo Yes/No
                            DialogResult result = MessageBox.Show(
                                "Check hiện tại đã kiểm bản. Bạn có muốn copy Version mới hay không?",
                                "Xác nhận",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (result == DialogResult.No)
                            {
                                return;
                            }

                        }

                    }

                }
                string urlNewVersion = string.Format("{0}?action={1}&&para1={2}", URL + "CheckListCD/Get", "GetNewVersion", stylteIDPaste);
                string jsonNewVersion = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNewVersion); }).Result;
                DataTable tblNewVersion = JsonConvert.DeserializeObject<DataTable>(jsonNewVersion);
                if (tblNewVersion != null && tblNewVersion?.Rows?.Count > 0)
                {
                    int.TryParse(tblNewVersion?.Rows[0]["Version"]?.ToString(), out int Newversion);
                    version = Newversion;
                }
                else
                {
                    version = 1;
                }
                if (version == 0) version = 1;
                DataTable tblTVThongSoCheckList_Copy = new DataTable();
                string urlDicThongSo = string.Format("{0}?action={1}&para1={2}", URL + "CheckListCD/Get", "GetTVThongSoCheckList", _stylteIDCopy);
                string jsonDicThongSo = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDicThongSo); }).Result;
                if (jsonDicThongSo != "[]")
                {
                    tblTVThongSoCheckList = JsonConvert.DeserializeObject<DataTable>(jsonDicThongSo);
                }
                DataTable tblCopy = JsonConvert.DeserializeObject<DataTable>(json);
                int rowHandle_focused = grvCheckList_CD.FocusedRowHandle;
                DataSet dsSave = new DataSet();
                if (_tblCheckListCD == null || _tblCheckList_ThongSo == null || _tblCheckList_ThongSo.Rows.Count == 0)
                {
                    CreateTable_CheckListCD();
                }

                DataTable tblSaveCheckListCD = _tblCheckListCD.Clone();
                tblSaveCheckListCD.TableName = "tblCheckListCD";
                DataTable tblSaveCheckListTS = _tblCheckList_ThongSo.Clone();
                tblSaveCheckListTS.TableName = "tblCheckListTS";

                if (tblSaveCheckListTS.Columns.Contains("IsUse"))
                {
                    tblSaveCheckListTS.Columns.Remove("IsUse");
                }


                DataTable dtCheckListCD = tblCopy;
                if (dtCheckListCD != null)
                {
                    // Lấy tất cả rows và sort
                    var allRows = dtCheckListCD.AsEnumerable()
                        .Select(r => new
                        {
                            Row = r,
                            ParentNO = r["ParentNO"]?.ToString() ?? "",
                            TenNhom = r["TenNhomCongDoan"]?.ToString() ?? "",
                            SortNum = GetSortValue(r)
                        })
                        .OrderBy(x => ExtractLeadingNumber(x.TenNhom))
                        .ThenBy(x => x.TenNhom)
                        .ThenBy(x => x.SortNum)
                        .ToList();

                    // Dictionary để track Sort position cho mỗi ParentNO
                    Dictionary<string, int> sortCounters = new Dictionary<string, int>();

                    foreach (var item in allRows)
                    {
                        DataRow row = item.Row;
                        string parentNoKey = item.ParentNO;

                        // Tăng counter cho ParentNO này
                        if (!sortCounters.ContainsKey(parentNoKey))
                            sortCounters[parentNoKey] = 0;
                        sortCounters[parentNoKey]++;

                        DataRow newRow = tblSaveCheckListCD.NewRow();

                        // Copy fields với TryParse an toàn
                        newRow["ID"] = int.TryParse(row["ID"]?.ToString(), out int id) ? id : 0;
                        newRow["ParentID"] = int.TryParse(row["ParentID"]?.ToString(), out int parentId) ? parentId : 0;
                        newRow["ParentNO"] = int.TryParse(row["ParentNO"]?.ToString(), out int parentNo) ? parentNo : 0;
                        newRow["TenNhomCongDoan"] = row["TenNhomCongDoan"]?.ToString() ?? "";
                        newRow["StyleID"] = stylteIDPaste;
                        newRow["MaHang"] = row["MaHang"]?.ToString() ?? "";
                        newRow["NO"] = row["NO"]?.ToString() ?? "";
                        newRow["Description"] = row["Description"]?.ToString() ?? "";
                        newRow["Note"] = row["Note"]?.ToString() ?? "";
                        newRow["Tol"] = row["Tol"]?.ToString() ?? "";

                        // Sort = vị trí trong group
                        newRow["Sort"] = sortCounters[parentNoKey];

                        newRow["CreateDate"] = DateTime.Now;
                        newRow["NVien"] = GlobleData.UserName;
                        newRow["Version"] = version;
                        newRow["Tol_Negative"] = row["Tol_Negative"]?.ToString() ?? "";
                        newRow["Donvi"] = row["Donvi"]?.ToString().ToUpper() ?? "";
                        newRow["Note_TS"] = row["Note_TS"]?.ToString() ?? "";
                        newRow["MucDo"] = row["MucDo"]?.ToString() ?? "";

                        tblSaveCheckListCD.Rows.Add(newRow);
                    }
                }
                DataTable dtCheckListTS = _tblCheckList_ThongSo;
                if (dtCheckListTS != null)
                {
                    foreach (DataRow pivotRow in dtCheckListTS.Rows)
                    {
                        string congDoan = pivotRow["CongDoan"]?.ToString() ?? "";
                        int parentID = pivotRow["ParentID"] != DBNull.Value ? Convert.ToInt32(pivotRow["ParentID"]) : 0;
                        int parentNO = pivotRow["ParentNO"] != DBNull.Value ? Convert.ToInt32(pivotRow["ParentNO"]) : 0;
                        string styleID = stylteIDPaste;
                        string tenNhomCongDoan = pivotRow["TenNhomCongDoan"]?.ToString() ?? "";

                        DataRow newRow = tblSaveCheckListTS.NewRow();

                        string TenCongDoan_CT = "";
                        if (tblTVThongSoCheckList?.Rows?.Count > 0)
                        {
                            var Query = tblTVThongSoCheckList.AsEnumerable().FirstOrDefault(x => x["MaCongDoan_ThongSo"]?.ToString() == pivotRow["CongDoan"]?.ToString());
                            if (Query != null)
                            {
                                TenCongDoan_CT = Query["CongDoan"]?.ToString();
                            }
                        }
                        newRow["ID"] = 0; // Assuming ID is assigned by the server
                        newRow["CongDoan"] = TenCongDoan_CT;
                        newRow["ParentID"] = parentID;
                        newRow["ParentNO"] = parentNO;
                        newRow["Size"] = pivotRow["Size"];
                        newRow["SizeID"] = pivotRow["SizeID"];
                        newRow["ThongSo"] = pivotRow["ThongSo"];
                        newRow["Sort"] = pivotRow["Sort"]; // Adjust if Sort is available in DataSource
                        newRow["StyleID"] = styleID;
                        newRow["TenNhomCongDoan"] = tenNhomCongDoan;
                        newRow["DonViTS"] = pivotRow["DonViTS"];
                        newRow["MaCongDoan_ThongSo"] = pivotRow["MaCongDoan_ThongSo"];
                        newRow["SizeType"] = pivotRow["SizeType"];
                        newRow["SizeTypeID"] = pivotRow["SizeTypeID"];
                        newRow["Version"] = version;
                        tblSaveCheckListTS.Rows.Add(newRow);
                    }
                }

                if (tblSaveCheckListCD?.Rows?.Count == 0 && tblSaveCheckListTS?.Rows?.Count == 0)
                {

                    return;
                }


                dsSave.Tables.Add(tblSaveCheckListCD);
                if (tblSaveCheckListTS.Rows.Count == 0)
                {
                    tblSaveCheckListTS.Rows.Add(null, null, null, null, null, null, null, null, null, null);
                }
                dsSave.Tables.Add(tblSaveCheckListTS);



                string urlSave = string.Format("{0}", URL + "CheckListCD/POST?action=ImportExcel");
                string jsonData = JsonConvert.SerializeObject(dsSave);
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, jsonData); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    //LoadCheckListCD();
                    LoadVerCheckList();
                    grvCheckList_CD.FocusedRowHandle = rowHandle_focused;
                }
                else
                {
                    //XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }


        #endregion
        public List<DataTable> GET_ListTableThuVienDaDung()
        {
            string urlGetListDataTable = URL + "GetThuVien/GetThuVienDaDung";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetListDataTable); }).Result;
            List<DataTable> _ListDataTable = JsonConvert.DeserializeObject<List<DataTable>>(jsonLstDataTable);
            return _ListDataTable;
        }

        private void gridHangHoa_ProcessGridKey(object sender, KeyEventArgs e)
        {

        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {

            // CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSHangHoa(false);
            LoadHangHoaCheckKeoVe();
            //CreateSearchLookup();
            loadMauVaSize();
            CreateSearchLookUpEditLocKH();
            // MaHang Hinh//
            InitMaHang_Hinh();
            LoadChungLoai();
            CheckPerminsion();
        }

        private void LoadHangHoaCheckKeoVe()
        {
            string url = string.Format("{0}?", URL + "HangHoa/GetHangHoaCheckKeoVe");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                _dtHangHoaCheckKeoVe = JsonConvert.DeserializeObject<DataTable>(json);
            }
        }

        private void loadMauVaSize()
        {
            string url = string.Format("{0}?", URL + "BangMau/GetBangMau");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json) && json != "[]")
            {
                tblBangMau = JsonConvert.DeserializeObject<DataTable>(json);
            }
            string url2 = string.Format("{0}?", URL + "BangSize/GetBangSize");
            string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;

            if (!string.IsNullOrEmpty(json2) && json2 != "[]")
            {
                tblBangSize = JsonConvert.DeserializeObject<DataTable>(json2);
            }
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
                barButtonItem2.Enabled = true;
                barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barButtonItem3.Enabled = true;
                barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnSave_SeasonHinh.Enabled = false;
                btnSaveThongSo.Enabled = false;
                btnDeleteTS.Enabled = false;

                btnImportNewVersion.Enabled = false;
                btnImportNewRap.Enabled = false;
                btnSaveSeason_TSo.Enabled = false;
                btnImportExcel.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
                barButtonItem2.Enabled = true;
                barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barButtonItem3.Enabled = true;
                barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnSave_SeasonHinh.Enabled = false;
                btnSaveThongSo.Enabled = false;
                btnDeleteTS.Enabled = false;

                btnImportNewVersion.Enabled = false;
                btnImportNewRap.Enabled = false;
                btnSaveSeason_TSo.Enabled = false;
                btnImportExcel.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
            {
                barButtonItem2.Enabled = true;
                barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barButtonItem3.Enabled = true;
                barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnSave_SeasonHinh.Enabled = false;
                btnSaveThongSo.Enabled = false;
                btnDeleteTS.Enabled = false;

                btnImportNewVersion.Enabled = false;
                btnImportNewRap.Enabled = false;
                btnSaveSeason_TSo.Enabled = false;
                btnImportExcel.Enabled = false;
            }

        }
        // Hàm focused dùng để ràng buộc focused vào cell nào và không được focused vào cell nào
        private void focused(object sender)
        {
            //DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            //if (_status == ResourceURL.EventStatus.Add)
            //{
            //    if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd && !(view.FocusedColumn == colMaHang))
            //        view.FocusedColumn.OptionsColumn.AllowEdit = true;
            //    else
            //        view.FocusedColumn.OptionsColumn.AllowEdit = false;
            //}
            //else
            //{
            //    if (_status == ResourceURL.EventStatus.Edit)
            //    {
            //        if (view.FocusedColumn == colMaHang)
            //            view.FocusedColumn.OptionsColumn.AllowEdit = false;
            //        else
            //            view.FocusedColumn.OptionsColumn.AllowEdit = true;
            //    }
            //}
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    // gridViewHangHoa.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    //  gridViewHangHoa.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    //  gridViewHangHoa.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        //private void LoadDSHangHoa()
        //{
        //    string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
        //    if (tbl.Rows.Count == 0 || tbl == null)
        //    {
        //        NtbSoft.ERP.Libs.clsConvert<HangHoaEntity> convert = new Libs.clsConvert<HangHoaEntity>();
        //        DataTable tblnull = convert.ToDataTable(lstHangHoa);
        //        gridHangHoa.DataSource = tblnull;
        //    }
        //    else
        //        gridHangHoa.DataSource = tbl;
        //    gridHangHoa.RefreshDataSource();

        //    _status = ResourceURL.EventStatus.View;
        //    GridViewUpdateStatus(_status);
        //}

        private void LoadDSHangHoa(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstHangHoaEntity = JsonConvert.DeserializeObject<List<HangHoaEntity>>(json);
                }

                gridHangHoa.DataSource = lstHangHoaEntity;
                GridViewUpdateStatus(_status);

                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewHangHoa.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridHangHoa;
                }
                GetMau();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void CreateSearchLookup()
        {
            string url = string.Format("{0}?", URL + "LoaiHangHoa/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            searchLookUpEditLHH.Properties.DataSource = tbl;
            searchLookUpEditLHH.Properties.DisplayMember = "TenLHH";
            searchLookUpEditLHH.Properties.ValueMember = "MaLHH";
            searchLookUpEditLHH.Properties.ShowClearButton = false;
            searchLookUpEditLHH.Properties.NullText = "[Chọn giá trị]";
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenLHH";
            rCountryEdit.ValueMember = "MaLHH";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaLHH", Caption = "Mã LHH", Name = "colMaHang", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenLHH", Caption = "Loại hàng hóa", Name = "colTenHang", Visible = true });

            }
            colMaLHH.ColumnEdit = rCountryEdit;

            string urlKH = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
            DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
            searchLookUpEditKhachHang.Properties.DataSource = tblKH;

            repositoryItemSearchLookUpEditKhachHang.DataSource = tblKH;
            //searchLookUpEditKhachHang.Properties.ValueMember = "MaKH";
            //searchLookUpEditKhachHang.Properties.DisplayMember = "TenKH";
        }
        private void ThemDong()
        {
            HangHoaEntity obj = new HangHoaEntity();
            //_bindingHangHoaEntity.Add(obj);
            lstHangHoaEntity.Add(obj);

            _rowAdd = gridViewHangHoa.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridViewHangHoa.FocusedRowHandle = _rowAdd;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }
        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    lstThuVienDaDung = GET_ListTableThuVienDaDung();

                    HangHoaEntity row = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                    string maHangHoaXoa = row.MaHang;

                    DataTable _dtMaHang = lstThuVienDaDung[0];
                    bool isAcceptDelete = true;
                    bool exists = tblBangMau.Select($"MaHang = '{maHangHoaXoa}'").Length > 0;
                    bool exists2 = tblBangSize.Select($"MaHang = '{maHangHoaXoa}'").Length > 0;
                    if (exists)
                    {
                        string message = string.Format(@"Mã hàng {0} đang được sử dụng trong Bảng Màu, không thể xóa!", maHangHoaXoa);
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (exists2)
                    {
                        string message = string.Format(@"Mã hàng {0} đang được sử dụng trong Bảng Size, không thể xóa!", maHangHoaXoa);
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    for (int i = 0; i < _dtMaHang.Rows.Count; i++)
                    {
                        if (maHangHoaXoa == _dtMaHang.Rows[i]["MaHang"].ToString())
                            isAcceptDelete = false;
                    }
                    if (isAcceptDelete == false)
                    {
                        string message = string.Format(@"Mã hàng {0} đang được sử dụng, không thể xóa!", maHangHoaXoa);
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    string url = string.Format("{0}?Parameter={1}", URL + "HangHoa/DeleteHangHoa", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadDSHangHoa(false);
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewHangHoa.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewHangHoa.FocusedRowHandle;
                }
                List<HangHoaEntity> _lstUpdate = new List<HangHoaEntity>();
                HangHoaEntity row = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }

                bool resultValidateInsert = ValidateDataAddMulti();
                if (!resultValidateInsert)
                {
                    return;
                }

                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    HangHoaEntity item = gridViewHangHoa.GetRow(lstRowUpdate[i]) as HangHoaEntity;
                    if (item == null || (item != null && item.TenHang == null))
                    {
                        continue;
                    }
                    item.MaHang = RemoveOneUnderscore(Regex.Replace(RemoveDiacritics(item.TenHang), @"[^\w\d]+", "_"));
                    if (item != null && !string.IsNullOrEmpty(item.TenHang))
                    {
                        //item.ID = 0;
                        _lstUpdate.Add(item);
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    LoadDSHangHoa(true);
                    return;
                }
                if (_status.ToString() == "Edit")
                {
                    string urlKT = string.Format("{0}?", URL + "HangHoa/GetKiemTraHH");
                    string jsonKT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
                    DataTable _dtKiemTra = JsonConvert.DeserializeObject<DataTable>(jsonKT);
                    if (_dtKiemTra != null || _dtKiemTra.Rows.Count > 0)
                    {
                        foreach (var item in _lstUpdate)
                        {
                            DataRow _check = _dtKiemTra.AsEnumerable().Where(x => Convert.ToInt32(x["ID"]) == Convert.ToInt32(item.ID)).FirstOrDefault();
                            if (_check != null)
                            {
                                XtraMessageBox.Show("Mã hàng " + _check["MaHang"].ToString() + " đã có trên đơn hàng tổng. Không thể sửa tên hàng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                }
                string url = string.Format("{0}?", URL + "HangHoa/PostHangHoa");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
                if (msResult.ToLower() == "true")
                {
                    LoadDSHangHoa(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                lstRowUpdate.Clear();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // Đổi kí tự đặc biệt sang kí tự '_'
        // string doidausize = Regex.Replace(dataRows["DauSize"].ToString(), @"[^\w\d]+", "_");

        // Nếu chuỗi có 2 hoặc nhiều hơn 1 dấu kí tự '_' liên tiếp
        // => Chỉ lấy 1 kí tự '_'
        static string RemoveOneUnderscore(string input)
        {
            return Regex.Replace(input, @"_+", "_");
        }

        // Xóa dấu Tiếng Việt của chuỗi
        static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // Bỏ dấu Tiếng Việt của chuỗi
        // Replace kí tự khoảng trắng thành kí tự "_"
        // Thêm hậu tố "_"+ nextID
        private string ReplaceMaMau(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Sử dụng regex để chuyển đổi dấu thành không dấu
            string pattern = @"\p{IsCombiningDiacriticalMarks}+";
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            string result = Regex.Replace(normalizedString, pattern, string.Empty);
            result = Regex.Replace(result, @"Đ", "D");
            result = result.Replace(" ", "_");

            // Thêm hậu tố "_"+_nextID
            //result +=string.Format("{0}_{1}");
            //return string.Format("{0}_{1}",result,_nextID);
            return result;
        }

        private void NapLaiDong()
        {
            //CreateSearchLookup();
            LoadDSHangHoa(false);
            LoadHangHoaCheckKeoVe();
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            gridViewHangHoa.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void gridViewHangHoa_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            if (e.RowHandle < 0)
                return;



            //lstRowUpdate.Add(e.RowHandle);
            //focused(sender);

            if (e.Column.FieldName == "InTheu")
            {
                int focusedRowHandle = gridViewHangHoa.FocusedRowHandle;
                HangHoaEntity itemFocus = gridViewHangHoa.GetRow(focusedRowHandle) as HangHoaEntity;
                DataTable _dtSave = new DataTable();
                _dtSave.Columns.Add("ID", typeof(int));
                _dtSave.Columns.Add("MaHang", typeof(string));
                _dtSave.Columns.Add("TenHang", typeof(string));
                _dtSave.Columns.Add("MaKH", typeof(string));
                _dtSave.Columns.Add("MaHangCu", typeof(string));
                _dtSave.Columns.Add("InTheu", typeof(bool));
                DataRow row = _dtSave.NewRow();

                row["ID"] = 0;
                row["MaHang"] = itemFocus.MaHang;
                row["TenHang"] = itemFocus.TenHang;
                row["MaKH"] = itemFocus.MaKH;
                row["MaHangCu"] = "";
                row["InTheu"] = e.Value;

                _dtSave.Rows.Add(row);
                string url = string.Format("{0}", URL + $"ERPHangHoa/PostEdit?action=PostInTheu&type=@TypeHangHoa");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    return;
                }
            }
            else
            {
                if (e.Column.FieldName == "InTheuCT")
                {
                    int focusedRowHandle = gridViewHangHoa.FocusedRowHandle;
                    HangHoaEntity itemFocus = gridViewHangHoa.GetRow(focusedRowHandle) as HangHoaEntity;
                    DataTable _dtSave = new DataTable();
                    _dtSave.Columns.Add("ID", typeof(int));
                    _dtSave.Columns.Add("MaHang", typeof(string));
                    _dtSave.Columns.Add("TenHang", typeof(string));
                    _dtSave.Columns.Add("MaKH", typeof(string));
                    _dtSave.Columns.Add("MaHangCu", typeof(string));
                    _dtSave.Columns.Add("InTheu", typeof(bool));
                    DataRow row = _dtSave.NewRow();

                    row["ID"] = 0;
                    row["MaHang"] = itemFocus.MaHang;
                    row["TenHang"] = itemFocus.TenHang;
                    row["MaKH"] = itemFocus.MaKH;
                    row["MaHangCu"] = "";
                    row["InTheu"] = e.Value;

                    _dtSave.Rows.Add(row);
                    string url = string.Format("{0}", URL + $"ERPHangHoa/PostEdit?action=PostInTheuCT&type=@TypeHangHoa");

                    string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        return;
                    }
                }
            }

        }

        //private void gridViewHangHoa_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        //{
        //    DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
        //    if (_status == ResourceURL.EventStatus.Add)
        //    {
        //        if (_rowAdd != -1 && view.FocusedRowHandle < 0 || view.RowCount == 1)
        //            view.FocusedColumn.OptionsColumn.AllowEdit = true;
        //        else
        //            view.FocusedColumn.OptionsColumn.AllowEdit = false;
        //    }
        //    else
        //    {
        //        if (_status == ResourceURL.EventStatus.Edit)
        //        {
        //            if (view.FocusedColumn == colMaHang)
        //                view.FocusedColumn.OptionsColumn.AllowEdit = false;
        //        }
        //    }
        //}

        private void gridViewHangHoa_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
            LoadMaHang_Hinh();

            LoadTaiLieu();
            LoadInSeam_TSo();
            GetMau();
            LoadVerCheckList();
            LoadPO_DB();
            LoadPO_Set();
            LoadInSeam_TSo_DB();
            //LoadCheckListCD();
        }

        private void gridViewHangHoa_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        // Fires when an in-place editor is active
        private void gridControlHangHoa_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            GridView view = grid.FocusedView as GridView;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            if (view.FocusedColumn == colMaHang)
            {
                e.Handled = CheckCharacterID(view, e);
            }
            else if (view.FocusedColumn == colTenHang)
            {
                //e.Handled = CheckCharacterName(view, e);
            }

            Console.WriteLine("KeyPress: " + e.KeyChar);


            if (view.FocusedColumn == colMaHang || view.FocusedColumn == colTenHang || view.FocusedColumn == colNhomHang || view.FocusedColumn == colGhiChu)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }

        }

        private bool CheckCharacterID(GridView view, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Check name
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu không phải là kí tự đặc biệt -> Cho nhập
                // Nếu không phải là dấu câu -> Cho nhập
                if ((!char.IsSymbol(e.KeyChar) && !char.IsPunctuation(e.KeyChar)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckCharacterName(GridView view, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Check name
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu không phải là kí tự đặc biệt -> Cho nhập
                // Nếu không phải là dấu câu -> Cho nhập
                if (char.IsWhiteSpace(e.KeyChar) || (!char.IsSymbol(e.KeyChar) && !char.IsPunctuation(e.KeyChar)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }


        private void gridViewHangHoa_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridViewHangHoa.GetFocusedDataSourceRowIndex() >= 0)
            {
                if (gridViewHangHoa.FocusedColumn == this.colTenHang)
                {
                    if (e.Value.ToString() == "")
                    {
                        e.Valid = false;
                        e.ErrorText = "Thông tin không được để trống.!";
                        Luu.Enabled = false;
                        return;
                    }
                    else if (ContainsVietnamese(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Tên hàng hóa không được chứa dấu tiếng việt.!";
                        Luu.Enabled = false;
                        return;
                    }
                    var focusedRowHandless = gridViewHangHoa.GetFocusedDataSourceRowIndex();
                    var focusedRowss = gridViewHangHoa.GetDataRow(focusedRowHandless);
                    var tempLstsizess = new List<HangHoaEntity>(lstHangHoaEntity);
                    tempLstsizess[focusedRowHandless].TenHang = e.Value.ToString();
                    var duplicateItemsss = tempLstsizess
                        .GroupBy(bm => new { bm.TenHang, bm.MaKH })
                        .Where(group => group.Count() > 1)
                        .Select(group => new { group.Key.TenHang, group.Key.MaKH })
                        .ToList();
                    if (duplicateItemsss.Any())
                    {
                        string duplicates = string.Join(", ", duplicateItemsss.Select(item => $"{item.TenHang}"));
                        e.Valid = false;
                        e.ErrorText = $"Có các mã hàng bị trùng: {duplicates}";
                    }

                }
                else if (gridViewHangHoa.FocusedColumn == this.gridColMakH)
                {
                    if (e.Value.ToString() == "")
                    {
                        e.Valid = false;
                        e.ErrorText = "Thông tin không được để trống.!";
                        Luu.Enabled = false;
                        return;
                    }
                    else if (ContainsVietnamese(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "MaKH không được chứa dấu tiếng việt.!";
                        Luu.Enabled = false;
                        return;
                    }

                }

                //DataRow dr = ((DataTable)gridViewHangHoa.DataSource).AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();
                //DataRow dr = gridViewHangHoa.DataSource as BindingList<HangHoaEntity>();
                //    .AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();

                List<HangHoaEntity> LstHangHoa = gridViewHangHoa.DataSource as List<HangHoaEntity>;
                HangHoaEntity itemHangHoa = gridViewHangHoa.GetFocusedRow() as HangHoaEntity;
                List<HangHoaEntity> lstHangHoaExceptFocused = LstHangHoa.Where(item => item.MaHang != itemHangHoa.MaHang).ToList();

                //
                string result = CheckDuplicateDataUpdate(gridViewHangHoa.FocusedColumn.Name, e.Value.ToString(), gridViewHangHoa.GetFocusedRow() as HangHoaEntity, gridViewHangHoa.GetFocusedDataSourceRowIndex());

                if (!string.IsNullOrEmpty(result))
                {
                    e.Valid = false;
                    e.ErrorText = result;
                    Luu.Enabled = false;
                    return;
                }
                else
                {
                    Luu.Enabled = true;
                }
                //
                //HangHoaEntity hangHoa = lstHangHoaExceptFocused.Where(item => item.MaHang == e.Value.ToString()).FirstOrDefault();

                //if (hangHoa != null)
                //{
                //    e.Valid = false;
                //    e.ErrorText = "Mã hàng hóa đã bị trùng. Vui lòng nhập mã hàng hóa mới.!";
                //}
            }
        }

        private string CheckDuplicateDataUpdate(string colName, string value, HangHoaEntity itemUpdate, int currentIndex)
        {
            List<HangHoaEntity> lstDataSource = gridViewHangHoa.DataSource as List<HangHoaEntity>;

            for (int i = 0; i < lstDataSource.Count; i++)
            {
                // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không Update
                switch (colName)
                {
                    case "colTenHang":
                        if (currentIndex != i && ((itemUpdate?.MaKH ?? "").Equals(lstDataSource[i]?.MaKH ?? "")))
                        {
                            if (value.Equals(lstDataSource[i].TenHang))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                    case "gridColKhachHang":
                        if (currentIndex != i && ((itemUpdate?.TenHang ?? "").Equals(lstDataSource[i]?.TenHang ?? "")))
                        {
                            if (value.Equals(lstDataSource[i].MaKH))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                }
            }
            return "";
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        private void gridViewHangHoa_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridViewHangHoa_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridViewHangHoa_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.RowHandle < 0)
                return;
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }

        private void gridViewHangHoa_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (gridViewHangHoa.FocusedColumn.FieldName == "GhiChu")
            {
                return;
            }
            Console.WriteLine("gridViewHangHoa_ShowingEditor");
            string _focusedMaHang = gridViewHangHoa.GetFocusedRowCellValue("MaHang")?.ToString();
            DataRow _check = _dtHangHoaCheckKeoVe.AsEnumerable().Where(x => x["MaHang"].ToString() == _focusedMaHang).FirstOrDefault();
            if (_check != null && int.Parse(_check["IsKeoVe"].ToString()) == 1)
            {
                e.Cancel = true;
                XtraMessageBox.Show("Mã hàng này đã được kéo về.\nChỉ được thay đổi thông tin ghi chú.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //bool isDongThung = (bool)gridViewCongDoan.GetFocusedRowCellValue(this.gridColumnIsDongThung);
            //if (isDongThung)
            //{
            //    e.Cancel = true;
            //    XtraMessageBox.Show("Lệnh sản xuất này đã được lập kế hoạch.\nKhông thể thay đổi DVSX.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }
        static string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }
        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string _txtTenHang = txtTenHang?.EditValue?.ToString().Trim() ?? "";
            List<string> _lstTenHang = _txtTenHang.ToString().Trim().Split(':').ToList();
            string _khachHang = searchLookUpEditKhachHang?.EditValue?.ToString() ?? "";
            List<string> _lstKhachHang = _khachHang.Split(':').ToList();

            if (string.IsNullOrEmpty(_txtTenHang))
            {
                XtraMessageBox.Show("Tên hàng hóa không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable tblKH = searchLookUpEditKhachHang.Properties.DataSource as DataTable;
            DataRow[] rows = tblKH.Select($"MaKH = '{searchLookUpEditKhachHang.EditValue.ToString()}'");
            string tenKhachHang = rows[0]["TenKH"].ToString();
            List<HangHoaEntity> lstHangHoaEntitys = new List<HangHoaEntity>();
            string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstHangHoaEntitys = JsonConvert.DeserializeObject<List<HangHoaEntity>>(json);
            }
            List<HangHoaEntity> _lstAddHangHoa = new List<HangHoaEntity>();
            int focusedRow = lstHangHoaEntity.Count;
            for (int i = 0; i < _lstTenHang.Count; i++)
            {
                if (!string.IsNullOrEmpty(_lstTenHang[i]))
                {
                    HangHoaEntity hangHoaEntity = new HangHoaEntity(
                        _lstTenHang[i],
                        _khachHang,
                        searchLookUpEditLHH?.EditValue?.ToString() ?? "",
                        txtNhomHang.Text.ToString(),
                        txtGhiChu.Text.ToString(),
                        ReplaceSpecialCharacters(RemoveVietnameseTone(_lstTenHang[i].ToString().Trim())).ToString().ToUpper()
                    );
                    if (lstHangHoaEntitys.Count > 0)
                    {
                        bool existsInList = lstHangHoaEntitys.Any(existing => existing.TenHang == hangHoaEntity.TenHang && existing.MaKH == hangHoaEntity.MaKH);

                        if (existsInList)
                        {
                            XtraMessageBox.Show($"Mã hàng '{hangHoaEntity.TenHang}' đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;
                        }
                    }

                    bool existsInAddList = _lstAddHangHoa.Any(existing =>
                        existing.TenHang == hangHoaEntity.TenHang
                    );

                    if (!existsInAddList)
                    {
                        _lstAddHangHoa.Add(hangHoaEntity);
                        lstRowUpdate.Add(focusedRow);
                    }

                    focusedRow += 1;
                }
            }

            lstHangHoaEntity.AddRange(_lstAddHangHoa);
            gridHangHoa.DataSource = lstHangHoaEntity;
            gridViewHangHoa.FocusedRowHandle = lstHangHoaEntity.Count - lstRowUpdate.Count;
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            txtTenHang.EditValue = "";
            searchLookUpEditKhachHang.EditValue = "";
            searchLookUpEditLHH.EditValue = "";
            txtNhomHang.Text = "";
            txtGhiChu.Text = "";
        }

        private void gridViewHangHoa_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void txtTenHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            string vietnameseCharacters = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđíị";

            if (vietnameseCharacters.Contains(e.KeyChar.ToString().ToLower()))
            {
                // Nếu là ký tự có dấu, không cho phép nhập
                e.Handled = true;
            }
            else
            {
                // Nếu không phải ký tự có dấu, chuyển ký tự thành chữ hoa
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                ReadExcelPL(Sfd.FileName);
            }
        }

        private void ReadExcelPL(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                lstHangHoaEntity.Clear();
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;

                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A1:ZZ500");
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();
                DataTable dtSave = new DataTable();
                dtSave = source.ToDataTable();
                AddList(dtSave);
            }

            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        private void AddList(DataTable dtSave)
        {

            string MaHang = string.Empty, TenHang = string.Empty, NhomHang = string.Empty, GhiChu = string.Empty, MaLHH = string.Empty, MaKH = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstHangHoaEntity = JsonConvert.DeserializeObject<List<HangHoaEntity>>(json);
            }
            string urllhh = string.Format("{0}?", URL + "LoaiHangHoa/Get");
            string jsonlhh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urllhh); }).Result;
            DataTable dthh = JsonConvert.DeserializeObject<DataTable>(jsonlhh);

            string urlkh = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkh); }).Result;
            DataTable dtkh = JsonConvert.DeserializeObject<DataTable>(jsonkh);
            List<string> danhsachtrung = new List<string>();
            List<string> chuakhaibaolhh = new List<string>();
            List<string> chuakhaibaokh = new List<string>();
            foreach (DataRow row in dtSave.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                var rowlhh = dthh.AsEnumerable().FirstOrDefault(r => r["TenLHH"].ToString().Trim() == row[1].ToString().Trim());
                var rowlkh = dtkh.AsEnumerable().FirstOrDefault(r => r["TenKH"].ToString().Trim() == row[3].ToString().Trim());

                if (rowlhh == null)
                {
                    //MessageBox.Show(" Loại hàng hóa:  " + row[1].ToString() + " chưa được khai báo trong thư viện Loại Hàng Hóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    //return;
                    chuakhaibaolhh.Add(row[1].ToString() + "\n");
                }
                if (rowlkh == null)
                {
                    //MessageBox.Show(" Khách hàng:  " + row[3].ToString() + " chưa được khai báo trong thư viện Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    //return;
                    chuakhaibaokh.Add(row[3].ToString());
                }

                if (rowlhh == null || rowlkh == null)
                {
                    continue;
                }
                MaHang = RemoveOneUnderscore(Regex.Replace(RemoveDiacritics(row[0].ToString()), @"[^\w\d]+", "_"));
                TenHang = row[0].ToString();
                MaLHH = rowlhh["MaLHH"].ToString();
                NhomHang = row[2].ToString();
                MaKH = rowlkh["MaKH"].ToString();
                GhiChu = row[4].ToString();

                bool Istrung = lstHangHoaEntity.Any(x => x.MaHang == MaHang);
                if (Istrung)
                {
                    //XtraMessageBox.Show($"Hàng hóa bị trùng: {TenHang}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhsachtrung.Add(TenHang);
                }
                else
                {
                    lstHangHoaEntity.Add(new HangHoaEntity
                    {
                        MaHang = MaHang,
                        TenHang = TenHang,
                        MaLHH = MaLHH,
                        NhomHang = NhomHang,
                        MaKH = MaKH,
                        GhiChu = GhiChu
                    });
                    string msResult = "";
                    string urlpost = string.Format("{0}?", URL + "HangHoa/PostHangHoa");
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlpost, lstHangHoaEntity); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        LoadDSHangHoa(true);
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else XtraMessageBox.Show(msResult);
                    //_status = ResourceURL.EventStatus.View;
                    //GridViewUpdateStatus(_status);
                    lstRowUpdate.Clear();
                }
            }
            // Sau khi kiểm tra tất cả các dòng, hiển thị thông báo nếu có lỗi
            if (chuakhaibaolhh.Any())
            {
                XtraMessageBox.Show($"Các Loại Hàng Hóa chưa khai báo:\n{string.Join("\n", chuakhaibaolhh.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chuakhaibaokh.Any())
            {
                XtraMessageBox.Show($"Các Khách Hàng chưa khai báo:\n{string.Join("\n", chuakhaibaokh.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (danhsachtrung.Any())
            {
                string dstrungmadt = string.Join(", ", danhsachtrung.Distinct());
                XtraMessageBox.Show($"Các mã Khách Hàng bị trùng:{dstrungmadt}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void txtNhomHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void gridViewHangHoa_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }



        private bool ValidateDataAddMulti()
        {
            List<HangHoaEntity> lstDataSource = gridViewHangHoa.DataSource as List<HangHoaEntity>;
            for (int i = 0; i < lstRowUpdate.Count; i++)
            {
                //BangSizeEntity itemSize = lstRowUpdate[i];
                HangHoaEntity itemSize = (gridViewHangHoa.DataSource as List<HangHoaEntity>)[lstRowUpdate[i]];

                if (string.IsNullOrEmpty(itemSize.TenHang))
                {
                    XtraMessageBox.Show("Tên hàng không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
                else if (string.IsNullOrEmpty(itemSize.MaKH))
                {
                    XtraMessageBox.Show("Khách hàng không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
                //
                for (int j = 0; j < lstDataSource.Count; j++)
                {
                    // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không insert
                    if (lstRowUpdate[i] != j && (itemSize.TenHang.Equals(lstDataSource[j].TenHang) && itemSize.MaKH.Equals(lstDataSource[j].MaKH)))
                    {
                        XtraMessageBox.Show("Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return false;
                    }
                }
            }

            return true;
        }



        #region quan
        private void searchLookUpEditLocKH_EditValueChanged(object sender, EventArgs e)
        {
            GetMH();
            //LoadDSHangHoaKH(false);
        }

        private void searchLookUpEditLocMH_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSHangHoaKH(false);
        }

        private void CreateSearchLookUpEditLocKH()
        {
            searchLookUpEditLocKH.Properties.ValueMember = "MaKH";
            searchLookUpEditLocKH.Properties.DisplayMember = "TenKH";

            string url = $"{URL}KhaiBaoAll/Get?action=GetKHBangSize";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            searchLookUpEditLocKH.Properties.DataSource = tbl;

        }



        private void GetMH()
        {
            string khachhang = searchLookUpEditLocKH.EditValue?.ToString() ?? "All";

            searchLookUpEditLocMH.Properties.ValueMember = "MaHang";
            searchLookUpEditLocMH.Properties.DisplayMember = "TenHang";
            string urlmh = $"{URL}KhaiBaoAll/Get?action=GetHangHoaBangMau&para1={khachhang}";
            string jsonmh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmh); }).Result;
            DataTable tblmh = JsonConvert.DeserializeObject<DataTable>(jsonmh);
            if (tblmh == null || tblmh.Rows.Count == 0)
            {
                searchLookUpEditLocMH.Properties.DataSource = null;
                return;
            }
            searchLookUpEditLocMH.Properties.DataSource = tblmh;
            searchLookUpEditLocMH.EditValue = tblmh.Rows[0]["MaHang"];
            LoadDSHangHoaKH(false);
        }
        private void LoadDSHangHoaKH(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string kh = searchLookUpEditLocKH.EditValue.ToString() ?? "All";
                string mh = searchLookUpEditLocMH.EditValue.ToString() ?? "All";
                string url1 = string.Format("{0}?makh={1}&&mahang={2}", URL + "HangHoa/GetHangHoaByKH", kh, mh);
                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                //DataTable tblmh = JsonConvert.DeserializeObject<DataTable>(json);
                if (!string.IsNullOrEmpty(json1))
                {
                    lstHangHoaEntity = JsonConvert.DeserializeObject<List<HangHoaEntity>>(json1);
                }

                gridHangHoa.DataSource = lstHangHoaEntity;
                GridViewUpdateStatus(_status);

                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewHangHoa.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridHangHoa;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        #endregion

        #region Mạnh
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var frm = new frmHangHoaChiTiet())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadDSHangHoa(false);
                    string selectMahang = frm.selectMaHang;
                    this.BeginInvoke(new Action(() =>
                    {
                        for (int i = 0; i < gridViewHangHoa.RowCount; i++)
                        {
                            var row = gridViewHangHoa.GetRow(i) as HangHoaEntity;
                            if (row != null && row.MaHang.ToString() == selectMahang)
                            {
                                gridViewHangHoa.FocusedRowHandle = i;
                                gridViewHangHoa.SelectRow(i);
                                break;
                            }
                        }
                    }));
                }
            }
        }
        private void gridViewHangHoa_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            if (!_allowEdit || !_allowAdd || !_allowDelete) return;
            e.Menu.Items.Clear();
            DXMenuItem EditItem = new DXMenuItem();
            EditItem.Caption = "Sửa";
            EditItem.Click += Edit;
            e.Menu.Items.Add(EditItem);

            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa.CheckMaHang == 0)
            {
                DXMenuItem DeleteItem = new DXMenuItem();
                DeleteItem.Caption = "Delete";
                DeleteItem.Click += DeleteHH;
                e.Menu.Items.Add(DeleteItem);
            }
            DXMenuItem Copy = new DXMenuItem();
            Copy.Caption = "Sao chép";
            //Copy.Image = imageCollection1.Images[2];
            Copy.Click += Copy_Click;

            e.Menu.Items.Add(Copy);

            if (_isCopied == true)
            {
                DXMenuItem Paste = new DXMenuItem();
                Paste.Caption = "Dán";
                //Paste.Image = imageCollection1.Images[3];
                Paste.Click += Paste_Click;
                e.Menu.Items.Add(Paste);
            }


        }
        private void Copy_Click(object sender, EventArgs e)
        {
            _isCopied = true;
            _selectedSteps.Clear();
            using (frmCheckCopyHangHoa frm = new frmCheckCopyHangHoa())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _selectedSteps = frm.SelectedSteps.ToList();
                    foreach (int step in _selectedSteps.OrderBy(x => x))
                    {
                        switch (step)
                        {
                            case 1:
                                CopyThongSo_Chung();
                                break;

                            case 2:
                                CopyHinhAnh_TaiLieu();
                                break;


                            case 3:
                                CopyThongSo();
                                break;

                            case 4:
                                CopyThongSoCheckList();
                                break;
                        }
                    }
                }
            }
            //CopyThongSo();
            //CopyHinhAnh_TaiLieu();
            //CopyThongSo_Chung();
            //CopyThongSoCheckList();
        }
        private void Paste_Click(object sender, EventArgs e)
        {
            _isCopied = false;
            if (_selectedSteps == null || !_selectedSteps.Any())
            {
                MessageBox.Show("Chưa chọn nội dung cần paste!", "Thông báo");
                return;
            }

            XuLyPasteTheoThuTu(_selectedSteps);
        }
        private void XuLyPasteTheoThuTu(List<int> steps)
        {
            foreach (int step in steps.OrderBy(x => x))
            {
                switch (step)
                {
                    case 1:
                        PastThongSo_Chung();
                        break;

                    case 2:
                        PasteHinhAnh_TaiLieu();
                        break;

                    case 3:
                        PasteThongSo();
                        break;

                    case 4:
                        PasteThongSoCheckList();
                        break;
                }
            }
        }

        private void CopyHinhAnh_TaiLieu()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            mahangCopy = entityHangHoa.MaHang;
        }
        private void CopyThongSo()
        {
            _tblThongSoCopy = CreateTblCopy();
        }
        private void PasteThongSo()
        {
            LoadInSeam_TSo();
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            foreach (DataRow dr in _tblThongSoCopy.Rows)
            {
                dr["MaHang"] = entityHangHoa.MaHang;
            }
            SaveCongDoanV2(_tblThongSoCopy);
        }
        private void PasteHinhAnh_TaiLieu()
        {
            DataTable dtSource = grcMaHangHinh.DataSource as DataTable;
            _tblHinhAnhCopy = CreateTblMaHangHinh();
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa is null) return;
            var drNew = _tblHinhAnhCopy.NewRow();
            drNew["Pid"] = 0;
            drNew["StyleID"] = entityHangHoa.MaHang;
            drNew["MaHang"] = entityHangHoa.TenHang;
            drNew["Version"] = 1;
            _tblHinhAnhCopy.Rows.Add(drNew);
            string url = string.Format("{0}", URL + $"QTY_MaHangHinh/PostCopy?action=PostCopy&para1={mahangCopy}");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _tblHinhAnhCopy); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                LoadMaHang_Hinh();
                LoadTaiLieu();
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
        }


        private void DeleteHH(object sender, EventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa == null) return;
            int ID = entityHangHoa.ID;
            string url = string.Format("{0}?Parameter={1}", URL + "HangHoa/DeleteHangHoa", ID);
            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (result.ToLower() == "true")
                LoadDSHangHoa(false);
        }



        private void Edit(object sender, EventArgs e)
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa == null) return;
            string mahang = entityHangHoa.MaHang;
            string tenHang = entityHangHoa.TenHang;
            string khachHang = entityHangHoa.MaKH;
            string ghiChu = entityHangHoa.GhiChu ?? "";
            string maChungLoa = entityHangHoa.MaCL ?? "";
            string MaLHH = entityHangHoa.MaLHH ?? "";
            int trangthai = entityHangHoa.TrangThai;
            bool InTheu = entityHangHoa.InTheu;
            bool InTheuCT = entityHangHoa.InTheuCT;
            bool HutAm = entityHangHoa.HutAm;
            bool DoKim = entityHangHoa.DoKim;
            using (var frm = new frmHangHoaChiTiet(mahang, tenHang, khachHang, maChungLoa, MaLHH, ghiChu, true, trangthai, InTheu, InTheuCT, HutAm, DoKim))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadDSHangHoa(false);
                    string selectMahang = mahang;
                    this.BeginInvoke(new Action(() =>
                    {
                        for (int i = 0; i < gridViewHangHoa.RowCount; i++)
                        {
                            var row = gridViewHangHoa.GetRow(i) as HangHoaEntity;
                            if (row != null && row.MaHang.ToString() == selectMahang)
                            {
                                gridViewHangHoa.FocusedRowHandle = i;
                                gridViewHangHoa.SelectRow(i);
                                break;
                            }
                        }
                    }));
                }
                else
                {
                    GetMau();
                }
            }
        }
        private void GetMau()
        {
            try
            {
                var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (entityHangHoa == null) return;
                string mahang = entityHangHoa.MaHang;
                string khachHang = entityHangHoa.MaKH;
                string url = $"{URL}ERPHangHoa/Get?action=GetMauMH&para={mahang}&para2={khachHang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                GetInSeam();
                GetSize();
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcMau.DataSource = null;
                    return;
                }
                grcMau.DataSource = tbl;

            }
            catch (Exception ex)
            {

            }
        }
        private void GetInSeam()
        {
            try
            {
                var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (entityHangHoa == null) return;
                string mahang = entityHangHoa.MaHang;
                string khachHang = entityHangHoa.MaKH;
                string url = $"{URL}ERPHangHoa/Get?action=GetInSeamMH&para={mahang}&para2={khachHang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcInSeamMH.DataSource = null;
                    return;
                }
                grcInSeamMH.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void CopyThongSo_Chung()
        {
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa == null) return;
            string mahang = entityHangHoa.MaHang;
            string khachHang = entityHangHoa.MaKH;
            _makhCopy = khachHang;
            mahangCopy = mahang;
        }
        private void PastThongSo_Chung()
        {

            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa == null) return;
            string mahang = entityHangHoa.MaHang;
            string khachHang = entityHangHoa.MaKH;

            string url = $"{URL}ERPHangHoa/Get?action=PostCopyMaHang&para={_makhCopy}&para2={mahangCopy}&para3={khachHang}&para4={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                if (tbl.Rows[0]["Result"].ToString() == "2")
                {
                    GetMau();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
        }

        private void GetSize()
        {
            try
            {
                var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
                if (entityHangHoa == null) return;
                string mahang = entityHangHoa.MaHang;
                string khachHang = entityHangHoa.MaKH;
                string url = $"{URL}ERPHangHoa/Get?action=GetSizeMH&para={mahang}&para2={khachHang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcSize.DataSource = null;
                    return;
                }
                grcSize.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void grvMau_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            //if (e.Menu == null) return;
            //if (!_allowEdit || !_allowAdd || !_allowDelete) return;
            //e.Menu.Items.Clear();
            //DXMenuItem DeleteMau = new DXMenuItem();
            //DeleteMau.Caption = "Xóa";
            //DeleteMau.Click += Delete;
            //e.Menu.Items.Add(DeleteMau);
        }


        private void Delete(object sender, EventArgs e)
        {
            DataRow row = grvMau.GetFocusedDataRow();
            if (row == null) return;

            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa == null) return;

            string maHang = entityHangHoa.MaHang;
            string maKH = entityHangHoa.MaKH;
            string maMau = row["MaMau"].ToString();
            string IDDelete = row["ID"].ToString();

            string urlCheck = $"{URL}ERPHangHoa/Get?action=CheckMauUsed&para={maHang}&para2={maKH}&para3={maMau}";
            string jsonCheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlCheck)).Result;

            DataTable dtCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
            if (dtCheck != null && dtCheck.Rows.Count > 0)
            {
                XtraMessageBox.Show("Màu đã được sử dụng, không thể xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = string.Format("{0}", URL + $"ERPHangHoa/Delete?action=DeleteMau&id={IDDelete}");
            var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                GetMau();
            }
        }

        private void gridViewHangHoa_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Band == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Band.AppearanceHeader.GradientMode);
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



        private void gridViewHangHoa_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["TrangThai"]);

                    if (Strflth == "0")
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");
                        e.HighPriority = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void gridView6_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            //if (e.Menu == null) return;
            //if (!_allowEdit || !_allowAdd || !_allowDelete) return;
            //e.Menu.Items.Clear();
            //DXMenuItem DeleteSize = new DXMenuItem();
            //DeleteSize.Caption = "Xóa";
            //DeleteSize.Click += DeleteS;
            //e.Menu.Items.Add(DeleteSize);
        }


        private void DeleteS(object sender, EventArgs e)
        {
            DataRow row = grvSize.GetFocusedDataRow();
            if (row == null) return;
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa == null) return;
            string mahang = entityHangHoa.MaHang;
            string khachHang = entityHangHoa.MaKH;
            string maSize = row["MaSize"].ToString();
            string maTheSize = row["MaTheSize"].ToString();

            string urlCheck = URL + $"ERPHangHoa/Get?action=CheckSizeUsed&para={mahang}&para2={khachHang}&para3={maSize}";
            string jsonCheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlCheck)).Result;

            DataTable dtCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
            if (dtCheck != null && dtCheck.Rows.Count > 0)
            {
                XtraMessageBox.Show("Size đã được sử dụng, không thể xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = string.Format("{0}", URL + $"ERPHangHoa/Delete?action=DeleteSize&id={mahang}&para2={khachHang}&para3={maSize}&para4={maTheSize}");
            var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                GetSize();
            }
        }



        private void grvInSeamMH_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            //if (e.Menu == null) return;
            //if (!_allowEdit || !_allowAdd || !_allowDelete) return;
            //e.Menu.Items.Clear();
            //DXMenuItem DeleteIS = new DXMenuItem();
            //DeleteIS.Caption = "Xóa";
            //DeleteIS.Click += DeleteInItem;
            //e.Menu.Items.Add(DeleteIS);
        }
        private void DeleteInItem(object sender, EventArgs e)
        {
            DataRow row = grvInSeamMH.GetFocusedDataRow();
            if (row == null) return;
            var entityHangHoa = gridViewHangHoa.GetRow(gridViewHangHoa.FocusedRowHandle) as HangHoaEntity;
            if (entityHangHoa == null) return;
            string mahang = entityHangHoa.MaHang;
            string khachHang = entityHangHoa.MaKH;
            string maInSeam = row["MaInSeam"].ToString();
            string maTheInSeam = row["MaTheInSeam"].ToString();
            string url = string.Format("{0}", URL + $"ERPHangHoa/Delete?action=DeleteIS&id={mahang}&para2={khachHang}&para3={maInSeam}&para4={maTheInSeam}");
            var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                GetInSeam();
                GetSize();
            }
        }
        private void GrvMau_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                var urlHost = (string)settingsReader.GetValue("URLV2", typeof(String));
                //if (e.Column.FieldName == "STT" && e.IsGetData)
                //    e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
                if ((e.Column.FieldName == "UrlAnh") && e.IsGetData)
                {
                    DataRow dr = grvMau.GetDataRow(e.ListSourceRowIndex);
                    if (dr == null) return;
                    string url = urlHost + "/Images/BangMau/" + dr["HinhAnh"].ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            using (var wc = new System.Net.WebClient())
                            {
                                byte[] data = wc.DownloadData(url);
                                using (var ms = new MemoryStream(data))
                                    e.Value = Image.FromStream(ms);
                            }
                        }
                        catch
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        public List<string> OpenExcelAndShowSheets()
        {
            var sheetNames = new List<string>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All Files|*.*";
                openFileDialog.Title = "Chọn file Excel";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName; // LƯU ĐƯỜNG DẪN FILE

                    try
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(new FileInfo(selectedFilePath)))
                        {
                            foreach (var worksheet in package.Workbook.Worksheets)
                            {
                                sheetNames.Add(worksheet.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Lỗi khi đọc file Excel:\n{ex.Message}",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return sheetNames;
        }
        private DataTable ReadExcelToDataTable(string filePath, string sheetName)
        {
            DataTable dt = new DataTable();

            // Tạo các cột cho DataTable
            dt.Columns.Add("KhachHang", typeof(string));
            dt.Columns.Add("TenHang", typeof(string));
            dt.Columns.Add("LoaiHangHoa", typeof(string));
            dt.Columns.Add("NhomChungLoai", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));

            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[sheetName];

                    if (worksheet == null)
                    {
                        XtraMessageBox.Show($"Không tìm thấy sheet '{sheetName}'", "Lỗi");
                        return dt;
                    }

                    int row = 2; // Bắt đầu từ row 2

                    while (true)
                    {
                        // Lấy giá trị từ cột A đến D
                        var colA = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? "";
                        var colB = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "";
                        var colC = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? "";
                        var colD = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? "";
                        var colE = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "";
                        // Kiểm tra điều kiện dừng: cột A và D đều rỗng
                        if (string.IsNullOrEmpty(colA) && string.IsNullOrEmpty(colE))
                        {
                            break;
                        }

                        // Thêm dòng vào DataTable
                        DataRow dr = dt.NewRow();
                        dr["KhachHang"] = colA;
                        dr["TenHang"] = colB;
                        dr["LoaiHangHoa"] = colC;
                        dr["NhomChungLoai"] = colD;
                        dr["GhiChu"] = colE;
                        dt.Rows.Add(dr);

                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi đọc dữ liệu:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var sheetNames = OpenExcelAndShowSheets();

            if (sheetNames.Count > 0)
            {
                using (var frm = new frmERP_BangSizeImport(sheetNames))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        string selectedSheet = frm.SelectedSheet;
                        DataTable dt = ReadExcelToDataTable(selectedFilePath, selectedSheet);
                        DataTable dtHangHoa = frmHangHoaChiTiet.CreateSaveHangHoa();

                        string url = $"{URL}ERPHangHoa/Get?action=GetKhachHang";
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                        DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(json);

                        string urlCl = $"{URL}ERPHangHoa/Get?action=GetChungLoai";
                        string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCl); }).Result;
                        DataTable tblCL = JsonConvert.DeserializeObject<DataTable>(jsonCL);

                        string urlMH = $"{URL}ERPHangHoa/Get?action=GetMaHang";
                        string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
                        DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);

                        string urlLHH = $"{URL}ERPHangHoa/Get?action=GetLoaiHH";
                        string jsonLHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLHH); }).Result;
                        DataTable tblLHH = JsonConvert.DeserializeObject<DataTable>(jsonLHH);

                        foreach (DataRow item in dt.Rows)
                        {
                            var existsKH = tblKH.AsEnumerable()
                                .Where(r => r["TenKH"].ToString().ToUpper() == item["KhachHang"].ToString().ToUpper());

                            var rowKH = existsKH.FirstOrDefault();
                            if (rowKH == null)
                            {
                                XtraMessageBox.Show($"Khách hàng: {item["KhachHang"]} không tồn tại trong thư viện khách hàng!",
                                                      "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            var existsCL = tblCL.AsEnumerable()
                                .Where(r => r["TenCL"].ToString().ToUpper() == item["NhomChungLoai"].ToString().ToUpper());
                            var rowCL = existsCL.FirstOrDefault();
                            if (!existsCL.Any())
                            {
                                XtraMessageBox.Show($"Tên chủng loại: {item["NhomChungLoai"]} không tồn tại trong thư viện chủng loại!",
                                                      "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            bool existsmH = tblMH.AsEnumerable()
                                .Any(x =>
                                    x["TenHang"].ToString().ToUpper() == item["TenHang"].ToString().ToUpper() &&
                                    x["MaKH"].ToString().ToUpper() == rowKH["MaKH"].ToString().ToUpper()
                                );

                            if (existsmH)
                            {
                                XtraMessageBox.Show($"Tên hàng: {item["TenHang"]} của khách hàng: {item["KhachHang"]} đã tồn tại trong thư viện !",
                                                      "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            DataRow rowLHH = null;
                            if (tblLHH != null)
                            {
                                var existsLHH = tblLHH.AsEnumerable()
                               .Where(r => r["TenLHH"].ToString().ToUpper() == item["LoaiHangHoa"].ToString().ToUpper());

                                rowLHH = existsLHH.FirstOrDefault();
                            }

                            var drNewSource = dtHangHoa.NewRow();
                            drNewSource["ID"] = 0;
                            drNewSource["MaHang"] = RemoveVietnameseTone(frmHangHoaChiTiet.ReplaceSpecialCharactersPLUS(item["TenHang"].ToString())).ToUpper();
                            drNewSource["TenHang"] = item["TenHang"].ToString().ToUpper();
                            drNewSource["NhomHang"] = "";
                            drNewSource["GhiChu"] = item["GhiChu"].ToString();
                            drNewSource["MaLHH"] = rowLHH == null ? "" : rowLHH["MaLHH"];
                            drNewSource["MaKH"] = rowKH["MaKH"];
                            drNewSource["MaCL"] = rowCL["MaCL"];
                            dtHangHoa.Rows.Add(drNewSource);
                        }

                        string urlSave = string.Format("{0}", URL + $"ERPHangHoa/Post?action=PostMH&type=@TypeHangHoa");
                        var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtHangHoa); }).Result;
                        if (msResult.ToUpper() == "TRUE")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                            LoadDSHangHoa(false);
                            string selectMahang = RemoveVietnameseTone(frmHangHoaChiTiet.ReplaceSpecialCharactersPLUS(dt.Rows[0]["TenHang"].ToString())).ToUpper();
                            this.BeginInvoke(new Action(() =>
                            {
                                for (int i = 0; i < gridViewHangHoa.RowCount; i++)
                                {
                                    var row = gridViewHangHoa.GetRow(i) as HangHoaEntity;
                                    if (row != null && row.MaHang.ToString() == selectMahang)
                                    {
                                        gridViewHangHoa.FocusedRowHandle = i;
                                        gridViewHangHoa.SelectRow(i);
                                        break;
                                    }
                                }
                            }));

                        }
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Không có sheet nào được chọn hoặc file rỗng.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion
    }
}
