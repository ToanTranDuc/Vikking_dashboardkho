using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SoTheoDoi;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Libs;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.SoTheoDoi
{
    public partial class frmTheKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<TheKhoEntity> lstDSTheKho;
        //List<TheKhoEntity> lstDSTheKhoFilter;
        List<int> lstRowUpdate = new List<int>();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        string dateFilter = string.Empty;
        string maKhoFilter = string.Empty;
        string maHangFilter = string.Empty;
        DataTable tblDVSX, tblHangHoa, tblKhachHang, tblDonVi;
        KeyDownControlHandler keyDownControlHandler;
        public frmTheKho()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            CreateSearchLookup();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSTheKho();
        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtNapLai, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, XuatExcelTheKho, true, ActionType.ExCel);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, System.Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        private void CreateSearchLookup()
        {
            // DonViSanXuat
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            tblDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            this.repositorySearchLookUpEditDVSX.DataSource = tblDVSX;
            this.repositorySearchLookUpEditDVSX.DisplayMember = "TenDVSX";
            this.repositorySearchLookUpEditDVSX.ValueMember = "MaDVSX";

            // HangHoa
            string urlHangHoa = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonHangHoa = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHangHoa); }).Result;
            tblHangHoa = JsonConvert.DeserializeObject<DataTable>(jsonHangHoa);
            this.repositorySearchLookUpEditMaHang.DataSource = tblHangHoa;
            this.repositorySearchLookUpEditMaHang.DisplayMember = "TenHang";
            this.repositorySearchLookUpEditMaHang.ValueMember = "MaHang";

            // KhachHang
            string urlKhachHang = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonKhachHang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKhachHang); }).Result;
            tblKhachHang = JsonConvert.DeserializeObject<DataTable>(jsonKhachHang);
            this.repositorySearchLookUpEditKhachHang.DataSource = tblKhachHang;
            this.repositorySearchLookUpEditKhachHang.DisplayMember = "TenKH";
            this.repositorySearchLookUpEditKhachHang.ValueMember = "MaKH";

            // DonVi
            string urlDonVi = string.Format("{0}?", URL + "DonVi/GetDonVi");
            string jsonDonVi = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDonVi); }).Result;
            tblDonVi = JsonConvert.DeserializeObject<DataTable>(jsonDonVi);
            this.repositorySearchLookUpEditDVNhomVai.DataSource = tblDonVi;
            this.repositorySearchLookUpEditDVNhomVai.DisplayMember = "TenDV";
            this.repositorySearchLookUpEditDVNhomVai.ValueMember = "MaDV";

            List<string> items = new List<string>
            { "Bộ", "Cái", "Chiếc"};

            // Gán danh sách mục cho ComboBox
            repositoryItemComboBox1.Items.AddRange(items);
            repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        }

        private void LoadDSTheKho()
        {
            barEditItem1.EditValue = null;
            barEditItem2.EditValue = null;
            barEditItem3.EditValue = null;
            dateFilter = string.Empty;
            maKhoFilter = string.Empty;
            maHangFilter = string.Empty;

            string url = string.Format("{0}?", URL + "TheKho/GetDSTheKho");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            lstDSTheKho = JsonConvert.DeserializeObject<List<TheKhoEntity>>(json);
            lstDSTheKho.ForEach(x => x.NgayChungTu = DateTime.Now.Date);
            gridControlTheKho.DataSource = lstDSTheKho;            

            _status = ResourceURL.EventStatus.View;
            MapThongTinKho(lstDSTheKho);
            GridViewUpdateStatus(_status);
            focused(this.bandedGridView1);
        }

        private void MapThongTinKho(List<TheKhoEntity> lstTheKho)
        {
            // Gán dữ liệu cho thông tin
            dateEdit1.EditValue = DateTime.Now;
            bool _isCheckKho = lstTheKho.All(x => (x.MaKho.Equals(lstTheKho[0].MaKho)) && (x.MaHang.Equals(lstTheKho[0].MaHang)));

            if (_isCheckKho)
            {
                if (lstDSTheKho.Count == 0) return;
                // Text kho
                DataRow dataRowDVSX = tblDVSX.AsEnumerable().Where(x => x["MaDVSX"].ToString().Equals(lstTheKho[0].MaKho)).FirstOrDefault();
                if (dataRowDVSX != null && dataRowDVSX["TenDVSX"] != null)
                {
                    label1.Text = string.Format("Kho - Warehouse Address: {0}", dataRowDVSX["TenDVSX"].ToString());
                }

                // Text hàng hóa
                DataRow dataRowHangHoa = tblHangHoa.AsEnumerable().Where(x => x["MaHang"].ToString().Equals(lstTheKho[0].MaHang)).FirstOrDefault();
                if (dataRowHangHoa != null && dataRowHangHoa["TenHang"] != null)
                {
                    textBox1.Text = dataRowHangHoa["TenHang"].ToString();
                }

                // Text Khách hàng
                DataRow dataRowMaKH = tblKhachHang.AsEnumerable().Where(x => x["MaKH"].ToString().Equals(lstTheKho[0].MaKhachHang)).FirstOrDefault();
                if (dataRowMaKH != null && dataRowMaKH["TenKH"] != null)
                {
                    textBox3.Text = dataRowMaKH["TenKH"].ToString();
                }
            }
            else
            {
                label1.Text = string.Format("Kho - Warehouse Address: ");
                textBox1.Text = string.Empty;
                textBox3.Text = string.Empty;
            }
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    //bandedGridView1.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                        Them.Enabled = true;
                    if (_allowEdit)
                        Sua.Enabled = true;
                    if (_allowAdd || _allowEdit)
                        Luu.Enabled = false;
                    break;
                case ResourceURL.EventStatus.Edit:
                    //bandedGridView1.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                        Them.Enabled = false;
                    if (_allowEdit)
                        Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                        Luu.Enabled = true;

                    break;
                case ResourceURL.EventStatus.Add:
                    //bandedGridView1.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                        Them.Enabled = false;
                    if (_allowEdit)
                        Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                        Luu.Enabled = true;
                    break;
            }
        }

        private void gridBangSize_ProcessGridKey(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    if (_allowAdd)
                        ThemDong();
                    break;
                case Keys.F2:
                    if (_allowEdit)
                        SuaDong();
                    break;
                case Keys.F3:
                    if (_allowDelete)
                        XoaDong();
                    break;
                case Keys.F4:
                    if (_allowAdd || _allowEdit)
                        LuuDong();
                    break;
                case Keys.F5:
                    LoadDSTheKho();
                    break;
                case Keys.F6:
                    XuatExcelTheKho();
                    break;
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
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
            }
            if (_allowAdd || _allowEdit)
            {
                Luu.Enabled = true;
            }
            else
            {
                Luu.Enabled = false;
            }

            if (!_allowDelete)
                Xoa.Enabled = false;

        }

        private void bandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);

            // Chỉnh font chữ cho bandHeader
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Chỉnh alignment cho bandHeader
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            e.Info.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void ThemDong()
        {
            TheKhoEntity obj = new TheKhoEntity();
            //_bindingHangHoaEntity.Add(obj);
            lstDSTheKho.Add(obj);

            _rowAdd = bandedGridView1.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            bandedGridView1.FocusedRowHandle = _rowAdd;
        }

        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            focused(this.bandedGridView1);
        }

        private async void XoaDong()
        {
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                TheKhoEntity row = bandedGridView1.GetRow(bandedGridView1.FocusedRowHandle) as TheKhoEntity;
                string url = string.Format("{0}?Parameter={1}", URL + "TheKho/Delete", row.ID);
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                    LoadDSTheKho();
                else XtraMessageBox.Show(result);
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //ThemDong();

            frmThemTheKho frm = new frmThemTheKho(tblDVSX, tblHangHoa, tblKhachHang, tblDonVi);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowInTaskbar = false;
            frm.ShowIcon = false;
            frm.ShowDialog();
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TheKhoEntity itemTheKhoEntity = bandedGridView1.GetFocusedRow() as TheKhoEntity;
            if (itemTheKhoEntity != null)
            {
                List<TheKhoEntity> lstfilterDsTheKho = lstDSTheKho.Where(x => x.MaKho == itemTheKhoEntity.MaKho).ToList();
                frmThemTheKho frm = new frmThemTheKho(false, itemTheKhoEntity, lstfilterDsTheKho, tblDVSX, tblHangHoa, tblKhachHang, tblDonVi);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowInTaskbar = false;
                frm.ShowIcon = false;
                frm.ShowDialog();
            }


            //SuaDong();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void LuuDong()
        {
            this.ActiveControl = this.button1;
            List<TheKhoEntity> _lstUpdate = new List<TheKhoEntity>();

            TheKhoEntity focusedRow = bandedGridView1.GetFocusedRow() as TheKhoEntity;

            if (focusedRow == null)
            {
                XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();

            for (int i = 0; i < lstRowUpdate.Count; i++)
            {
                TheKhoEntity item = bandedGridView1.GetRow(lstRowUpdate[i]) as TheKhoEntity;
                _lstUpdate.Add(item);
            }

            string msResult = "";
            if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
            {
                return;
            }
            string url = string.Format("{0}", URL + "TheKho/Post");
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                LoadDSTheKho();
            }
            else XtraMessageBox.Show(msResult);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            lstRowUpdate.Clear();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void BtNapLai()
        {
            LoadDSTheKho();
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtNapLai();
        }

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            switch (_status)
            {
                case ResourceURL.EventStatus.View:
                    if ((view.FocusedRowHandle >= 0 &&
                        (view.FocusedColumn.Equals(this.colNgayChungTu) ||
                        view.FocusedColumn.Equals(this.colSoChungTu) ||
                        view.FocusedColumn.Equals(this.colMoTa) ||
                        view.FocusedColumn.Equals(this.colDonViTinh))))
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    break;
                // Add
                case ResourceURL.EventStatus.Add:
                    if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd)
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    break;

                // Edit
                case ResourceURL.EventStatus.Edit:
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                    break;
            }
        }

        private void gridViewSoTheKho_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            //focused(sender);
        }

        private void XuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XuatExcelTheKho();
        }

        private void XuatExcelTheKho()
        {
            this.ActiveControl = this.button1;
            List<TheKhoEntity> lstExport = bandedGridView1.DataSource as List<TheKhoEntity>;

            //bool isExport = lstExport.All(x => (x.MaKho.Equals(lstExport[0].MaKho)) && (x.MaHang.Equals(lstExport[0].MaHang)));

            if (lstExport != null && lstExport.Count > 0)
            {
                bool isExport = lstExport.All(x => (x.MaKho.Equals(lstExport[0].MaKho)) && (x.MaHang.Equals(lstExport[0].MaHang)));
                if (isExport)
                {


                    if (string.IsNullOrEmpty(textBox2.Text))
                    {
                        XtraMessageBox.Show("Vui lòng nhập Tờ số!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    //if (string.IsNullOrEmpty(lstExport[0].SoChungTu))
                    //{
                    //    XtraMessageBox.Show("Vui lòng nhập Số chứng từ!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    //    return;
                    //}

                    //if (string.IsNullOrEmpty(lstExport[0].MaDV))
                    //{
                    //    XtraMessageBox.Show("Vui lòng chọn đơn vị!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    //    return;
                    //}

                    SaveFileDialog Sfd = new SaveFileDialog();
                    Sfd.Title = "File To Save";
                    Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
                    Sfd.FileName = string.Format("TheKho{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
                    if (Sfd.ShowDialog() == DialogResult.OK)
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                        string fileName = "TheKho.xlsx";
                        string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                        string TemplateFileName = path;
                        string ExportFileName = Sfd.FileName;
                        Export(TemplateFileName, ExportFileName, lstExport);

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
                else
                {
                    XtraMessageBox.Show("Vui lòng lọc dữ liệu theo Mã DVSX, Mã Hàng để xuất Excel.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
            }
            else
            {
                //DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu xuất excel, vui lòng kiểm tra lại.!", "Thông báo");

                clsCommonBS.ConfirmError("Không có dữ liệu xuất excel, Vui lòng kiểm tra lại.!");
            }


        }

        private void Xoa_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        public void Export(string TemplateFileName, string ExportFileName, List<TheKhoEntity> lstTheKho)
        {
            try
            {

                if (lstTheKho != null)
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
                            excelPackage.Workbook.Properties.Title = "TheKho";

                            // Đường dẫn tới file Excel mẫu
                            string templateFilePath = TemplateFileName;

                            // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                            string resultFilePath = ExportFileName;

                            // Mở file Excel mẫu
                            FileInfo templateFile = new FileInfo(templateFilePath);
                            ExcelPackage templatePackage = new ExcelPackage(templateFile);

                            // Lấy bảng tính trong file Excel mẫu (Sheet1 là tên của bảng tính)
                            ExcelWorksheet ws = templatePackage.Workbook.Worksheets["KH-TT02-BM18"];

                            // Xét dữ liệu cho các field thông tin kho: Kho, Ngày lập thẻ

                            // Ô dữ liệu C3: Kho
                            DataRow dataRowDVSX = tblDVSX.AsEnumerable().Where(x => x["MaDVSX"].ToString().Equals(lstTheKho[0].MaKho)).FirstOrDefault();
                            if (dataRowDVSX != null && dataRowDVSX["TenDVSX"] != null)
                            {
                                string tenKho = dataRowDVSX["TenDVSX"].ToString();
                                ws.Cells[3, 3].Value = string.Format("Kho - Warehouse Address: {0}", tenKho);
                            }

                            // Ô dữ liệu A4: Ngày lập thẻ
                            if (!string.IsNullOrEmpty(dateEdit1.DateTime.ToString()) && !string.IsNullOrEmpty(textBox2.Text))
                            {
                                string ngayLapThe= dateEdit1.DateTime.ToString("dd/MM/yyyy");
                                //ws.Cells[4, 1].Value = string.Format("Ngày lập thẻ - Date: {0}                                                                                               Tờ số: {1}", lstTheKho[0].NgayNhapKho.ToString("dd/MM/yyyy"), lstTheKho[0].ToSo);
                                ws.Cells[4, 1].Value = string.Format("Ngày lập thẻ - Date: {0}                                                                                               Tờ số: {1}", ngayLapThe, textBox2.Text);
                                ws.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            }

                            // Ô dữ liệu A5: Mã hàng
                            DataRow dataRowHangHoa = tblHangHoa.AsEnumerable().Where(x => x["MaHang"].ToString().Equals(lstTheKho[0].MaHang)).FirstOrDefault();
                            if (dataRowHangHoa != null && dataRowHangHoa["TenHang"] != null)
                            {
                                string tenHang = dataRowHangHoa["TenHang"].ToString();
                                ws.Cells[5, 1].Value = string.Format("Tên thiết bị, vật tư, hàng hóa - Equiments, Material, Products: {0}", tenHang);
                            }

                            // Ô dữ liệu A6: Mã hàng
                            DataRow dataRowMaKH = tblKhachHang.AsEnumerable().Where(x => x["MaKH"].ToString().Equals(lstTheKho[0].MaKhachHang)).FirstOrDefault();
                            if (dataRowMaKH != null && dataRowMaKH["TenKH"] != null)
                            {
                                string tenKH = dataRowMaKH["TenKH"].ToString();
                                ws.Cells[6, 1].Value = string.Format("Khách hàng - Customer: {0}", tenKH);
                            }


                            // countRow là hàng bắt đầu thêm dữ liệu của gridview
                            int countRow = 10;
                            for (int i = 0; i < lstTheKho.Count; i++)
                            {
                                //DataRow dataRowDV = tblDonVi.AsEnumerable().Where(x => x["MaDV"].ToString().Equals(lstTheKho[i].MaDV)).FirstOrDefault();
                                //string tenDV = string.Empty;
                                //if (dataRowDV != null && dataRowDV["TenDV"] != null)
                                //{
                                //    tenDV = dataRowDV["TenDV"].ToString();
                                //}

                                ws.Row(countRow).Height = 14;
                                ws.Cells[countRow, 1].Value = lstTheKho[i].NgayChungTu.ToString("dd-MM-yyyy");
                                ws.Cells[countRow, 2].Value = lstTheKho[i].SoChungTu;
                                ws.Cells[countRow, 3].Value = lstTheKho[i].MoTa;
                                ws.Cells[countRow, 5].Value = lstTheKho[i].SLNhap;
                                ws.Cells[countRow, 6].Value = lstTheKho[i].SLXuat;
                                ws.Cells[countRow, 7].Value = lstTheKho[i].SLTon;
                                ws.Cells[countRow, 8].Value = lstTheKho[i].MaDV;
                                //ws.Cells[countRow, 8].Value = lstSoTheoDoi[i].NguoiBamSeal;

                                countRow++;
                            }


                            // 12: Là số cột
                            // 10: Là Row bắt đầu
                            DrawStyleCellExcel(ws, lstTheKho, 12, 10);

                            FileInfo resultFile = new FileInfo(resultFilePath);
                            templatePackage.SaveAs(resultFile);
                        }
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

        private void DrawStyleCellExcel(ExcelWorksheet ws, List<TheKhoEntity> lstTheKho, int countColumn, int startRow)
        {
            //ws.Cells[countRow, countColumn].Value = lstSoTheoDoi[i].NgayThangNam.ToString("dd/MM/yyyy");
            for (int row = startRow; row < lstTheKho.Count + startRow; row++)
            {
                for (int column = 1; column <= countColumn; column++)
                {
                    ws.Cells[row, column].Style.Font.Size = 12;
                    //ws.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[row, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Cells[row, column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, column].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    if (column == countColumn)
                    {
                        ws.Cells[row, column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }
            }
        }

        private void repositoryItemDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;

            if (changing.NewValue != null)
            {
                dateFilter = changing.NewValue.ToString();
                FilterData(dateFilter, maKhoFilter, maHangFilter);
            }
        }

        private void bandedGridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(this.bandedGridView1);
        }

        private void bandedGridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(this.bandedGridView1);
        }

        private void repositoryItemSearchDVSX_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;

            if (changing.NewValue != null)
            {
                maKhoFilter = changing.NewValue.ToString();
                FilterData(dateFilter, maKhoFilter, maHangFilter);
            }
        }

        private void repositoryItemSearchMaHang_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;

            if (changing.NewValue != null)
            {
                maHangFilter = changing.NewValue.ToString();
                FilterData(dateFilter, maKhoFilter, maHangFilter);
            }
            Console.WriteLine("EditValueChanged");
        }

        // Filter data allow date, MaKho, MaHang
        private void FilterData(string _dateFilter, string _maKhoFilter, string _maHangFilter)
        {
            List<TheKhoEntity> lstDsTheKhoFilter = JsonConvert.DeserializeObject<List<TheKhoEntity>>(JsonConvert.SerializeObject(lstDSTheKho));
            if (lstDsTheKhoFilter != null)
            {
                if (!string.IsNullOrEmpty(_dateFilter))
                {
                    lstDsTheKhoFilter = lstDsTheKhoFilter.Where(x => (x.NgayNhapKho.Date <= DateTime.Parse(_dateFilter).Date)).ToList();
                }

                if (!string.IsNullOrEmpty(_maKhoFilter))
                {
                    lstDsTheKhoFilter = lstDsTheKhoFilter.Where(x => (x.MaKho == _maKhoFilter)).ToList();
                }

                if (!string.IsNullOrEmpty(_maHangFilter))
                {
                    lstDsTheKhoFilter = lstDsTheKhoFilter.Where(x => (x.MaHang == _maHangFilter)).ToList();
                }
            }
            if(lstDsTheKhoFilter != null&& lstDsTheKhoFilter.Count > 0)
            {
                MapThongTinKho(lstDsTheKhoFilter);
            }

            gridControlTheKho.DataSource = lstDsTheKhoFilter;
        }

        private void gridViewTheKho_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewTheKho_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
    }
}
