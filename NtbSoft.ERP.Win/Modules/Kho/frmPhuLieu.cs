using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmPhuLieu : DevExpress.XtraEditors.XtraForm
    {
        
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        List<PhuLieuEntity> lstPhuLieu;
        List<int> lstRowUpdate = new List<int>();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        DataTable dttb;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;


        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        public frmPhuLieu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
             lstPhuLieu = new List<PhuLieuEntity>();
            _clientExtension = new HttpClientExtension();

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

        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            CreateSearchLookUpNhom();
            CreateSearchLookUpDVT();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadPL(false);
        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);//SystemUser/GetPer/userID=...
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
        private void LoadPL(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                clsWaitForm.ShowWaitForm(this, 1000);
                string url = string.Format("{0}", URL + $"KhoNPL/GetPL");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    dttb = JsonConvert.DeserializeObject<DataTable>(json);
                    lstPhuLieu = JsonConvert.DeserializeObject<List<PhuLieuEntity>>(json);
                    loadThongTinPL(lstPhuLieu[0].ID);
                }
                gridPL.DataSource = lstPhuLieu.Count > 0 ? lstPhuLieu : null;
                gridPL.RefreshDataSource();
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridView1.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridPL;
                }

            }
            catch
            {
                XtraMessageBox.Show("Chưa có dữ liệu xin vui lòng kiểm tra lại hoặc nhập thêm dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void CreateSearchLookUpNhom()
        {
            string url = string.Format("{0}", URL + $"KhoNPL/GetNhomNPL");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit plEdit = new RepositoryItemSearchLookUpEdit();
            plEdit.DataSource = tbl;
            plEdit.DisplayMember = "TenNhom";
            plEdit.ValueMember = "MaNhom";
            plEdit.ShowClearButton = false;
            plEdit.NullText = "[Chọn giá trị]";

            GridView dvView = plEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Tên Nhóm", Name = "colNhom", Visible = true });
            }
            colNhom.ColumnEdit = plEdit;

            textNhom.Properties.DataSource = tbl;
            textNhom.Properties.ValueMember = "MaNhom";
            textNhom.Properties.DisplayMember = "TenNhom";

        }
        private void CreateSearchLookUpDVT()
        {
            string url = string.Format("{0}?", URL + "DonViChungLoai/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit plEdit = new RepositoryItemSearchLookUpEdit();
            plEdit.DataSource = tbl;
            plEdit.DisplayMember = "TenNhom";
            plEdit.ValueMember = "MaNhom";
            plEdit.ShowClearButton = false;
            plEdit.NullText = "[Chọn giá trị]";

            GridView dvView = plEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Tên Nhóm", Name = "colNhom", Visible = true });
            }
            colNhom.ColumnEdit = plEdit;

            textDVT.Properties.DataSource = tbl;
            textDVT.Properties.ValueMember = "MaDVCL";
            textDVT.Properties.DisplayMember = "TenDVCL";
        }
        private void loadThongTinPL(int ID)
        {
            if (dttb != null)
            {
                PhuLieuEntity result = lstPhuLieu.Find(nl => nl.ID == ID);
                textID.Text = ID.ToString();
                textMVT.Text = result.MaVatTu;
                textTVT.Text = result.TenVatTu;
                textTMM.Text = result.TenCodeMau;
                textMM.Text = result.CodeMau;
                textMKT.Text = result.MaKeToan;
                textDVT.Text = result.DonViTinh;
                textNhom.Text = result.Nhom;

                textMVT.Properties.ReadOnly = true;
                textTVT.Properties.ReadOnly = true;
                textTMM.Properties.ReadOnly = true;
                textMM.Properties.ReadOnly = true;
                textMKT.Properties.ReadOnly = true;
                textDVT.Properties.ReadOnly = true;
                textNhom.Properties.ReadOnly = true;
            }
            else
            {

                MessageBox.Show("Chưa có dữ liệu, vui lòng thêm dữ liệu ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void ThemDong()
        {
            textID.Text = "0";
            textMVT.Text = "";
            textTVT.Text = "";
            textTMM.Text = "";
            textMM.Text = "";
            textMKT.Text = "";
            textDVT.Text = "";
            textNhom.Text = "";

            textMVT.Properties.ReadOnly = false;
            textTVT.Properties.ReadOnly = false;
            textTMM.Properties.ReadOnly = false;
            textMM.Properties.ReadOnly = false;
            textMKT.Properties.ReadOnly = false;
            textDVT.Properties.ReadOnly = false;
            textNhom.Properties.ReadOnly = false;
            PhuLieuEntity obj = new PhuLieuEntity();

            //_bindingHangHoaEntity.Add(obj);
            lstPhuLieu.Add(obj);
            _rowAdd = gridView1.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridView1.FocusedRowHandle = _rowAdd;
            gridPL.DataSource = lstPhuLieu;
            gridPL.RefreshDataSource();
        }

        private void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
            Them.Enabled = false;
            Sua.Enabled = false;
            Luu.Enabled = true;
            
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gridView1.FocusedRowHandle>=0){ 
                    int ID = Int32.Parse(gridView1.GetFocusedRowCellValue(colID)?.ToString());
                    string url = URL + $"KhoNPL/DeletePL?ID={ID}";
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                            LoadPL(false);
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void barButtonItem24_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridView1.OptionsBehavior.Editable = false;
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
                    gridView1.OptionsBehavior.Editable = true;
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
                    gridView1.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = true;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        private void SuaDong()
        {
            textMVT.Properties.ReadOnly = false;
            textTVT.Properties.ReadOnly = false;
            textTMM.Properties.ReadOnly = false;
            textMM.Properties.ReadOnly = false;
            textMKT.Properties.ReadOnly = false;
            textDVT.Properties.ReadOnly = false;
            textNhom.Properties.ReadOnly = false;
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            
     
        }
        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {

            if (this.gridView1.FocusedColumn != null)
            {
                e.Valid = true;

            if (this.gridView1.FocusedColumn == colMaVatTu) 
                {
                    List<PhuLieuEntity> lst = this.gridView1.DataSource as List<PhuLieuEntity>;
                    bool IsPL = lst.Any(x => x.MaVatTu == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Mã vật tư không được bỏ trống";
                        e.Valid = false;
                    }
                    else if (IsPL)
                    {

                        e.ErrorText = $"Mã vật tư {e.Value.ToString()} đã bị trùng";
                        e.Valid = false;
                    }
                }
                else if (this.gridView1.FocusedColumn == colMaVatTu)
                {
                    List<PhuLieuEntity> lst = this.gridView1.DataSource as List<PhuLieuEntity>;
                    bool IsPL = lst.Any(x => x.MaVatTu == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Mã vật tư không được bỏ trống";
                        e.Valid = false;
                    }
                }
                else if (this.gridView1.FocusedColumn == colTenVatTu)
                {
                    List<PhuLieuEntity> lst = this.gridView1.DataSource as List<PhuLieuEntity>;
                    bool IsPL = lst.Any(x => x.TenVatTu == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Tên vật tư không được bỏ trống";
                        e.Valid = false;
                    }
                    else if (IsPL)
                    {

                        e.ErrorText = $"Tên vật tư {e.Value.ToString()} đã bị trùng";
                        e.Valid = false;
                    }
                }
                else if (this.gridView1.FocusedColumn == colDonViTinh)
                {
                    List<PhuLieuEntity> lst = this.gridView1.DataSource as List<PhuLieuEntity>;
                    bool IsPL = lst.Any(x => x.DonViTinh == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Đơn vị tính không được bỏ trống";
                        e.Valid = false;
                    }
                }
                else if (this.gridView1.FocusedColumn == colDonViTinh)
                {
                    List<PhuLieuEntity> lst = this.gridView1.DataSource as List<PhuLieuEntity>;
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Đơn vị tính không được bỏ trống";
                        e.Valid = false;
                    }
                }
                else if (this.gridView1.FocusedColumn == colNhom)
                {
                    List<PhuLieuEntity> lst = this.gridView1.DataSource as List<PhuLieuEntity>;
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Nhóm không được bỏ trống";
                        e.Valid = false;
                    }
                }
            }
        }

        private void NapLaiDong()
        {
            LoadPL(false);
        }
        private void barButtonItem26_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void barButtonItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            ReadExcelPL(Sfd.FileName);
        }

        private void barButtonItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                lstPhuLieu.Clear();
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;

                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A4:ZZ500");
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
             string MaHangDonHang = string.Empty, MaVatTu = string.Empty, TenVatTu = string.Empty, CodeMau = string.Empty, TenCodeMau = string.Empty, DonViTinh = string.Empty, Nhom = string.Empty, MaKeToan = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}", URL + $"KhoNPL/GetPL");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


            if (!string.IsNullOrEmpty(json))
            {
                lstPhuLieu = JsonConvert.DeserializeObject<List<PhuLieuEntity>>(json);
            }
            foreach (DataRow row in dtSave.Rows)
            {
                MaVatTu = row[0].ToString();
                TenVatTu = row[1].ToString();
                CodeMau = row[2].ToString();
                TenCodeMau = row[3].ToString();
                MaKeToan = row[4].ToString();
                DonViTinh = row[5].ToString();
                Nhom = row[6].ToString();
                //check trung
                bool IsPL = lstPhuLieu.Any(x => x.MaVatTu == MaVatTu);
                if (IsPL)
                {
                    XtraMessageBox.Show($"Trùng mã vật tư: {MaVatTu}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lstPhuLieu.Add(new PhuLieuEntity
                    {
                        MaVatTu = MaVatTu,
                        TenVatTu = TenVatTu,
                        CodeMau = CodeMau,
                        TenCodeMau = TenCodeMau,
                        MaKeToan = MaKeToan,
                        DonViTinh = DonViTinh,
                        Nhom = Nhom
                    });
                    string msResult = "";
                    string url1 = URL + "KhoNPL/PostPL";
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url1, lstPhuLieu); }).Result;
                    LoadPL(false);
                }
            }
        }
        private async void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                PhuLieuEntity pl = new PhuLieuEntity
                {
                    ID = int.TryParse(textID.Text, out int id) ? id : 0,
                    MaVatTu = textMVT.Text,
                    TenVatTu = textTVT.Text,
                    CodeMau = textMM.Text,
                    TenCodeMau = textTMM.Text,
                    DonViTinh = textDVT.Text,
                    Nhom = textNhom.Text,
                    MaKeToan = textMKT.Text,
                };
                List<PhuLieuEntity> _lstUpdate = new List<PhuLieuEntity> { pl };
                //List<PhuLieuEntity> lstpost = new List<PhuLieuEntity>();
                    //for (int i = 0; i < lstPhuLieu.Count; i++)
                    //{
                    //    PhuLieuEntity item = lstPhuLieu[i] as PhuLieuEntity;
                    //    if (item != null && !string.IsNullOrEmpty(item.MaVatTu) && !string.IsNullOrEmpty(item.TenVatTu) && !string.IsNullOrEmpty(item.CodeMau))
                    //    {
                    //        _lstUpdate.Add(item);

                    //    }
                    //    else
                    //    {
                    //        XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    //        return;
                    //    }
                    //}
                    string msResult = "";
                    if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                    {
                        return;
                    }

                    string url = URL + "KhoNPL/PostPL";
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                //
                if (msResult.ToLower() == "true")
                {
                    LoadPL(false);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                lstRowUpdate.Clear();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            textMVT.Properties.ReadOnly = true;
            textTVT.Properties.ReadOnly = true;
            textTMM.Properties.ReadOnly = true;
            textMM.Properties.ReadOnly = true;
            textMKT.Properties.ReadOnly = true;
            textDVT.Properties.ReadOnly = true;
            textNhom.Properties.ReadOnly = true;

        }

        private void barButtonItem30_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string mavattu = string.Empty;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (dttb is null || dttb.Rows.Count == 0) return;
            //mavattu = dttb.AsEnumerable().Where(x => x["MaVatTu"] == mavattu.ToString()).FirstOrDefault()["MaVatTu"].ToString();
            //int Size = 0;
            Sfd.FileName = string.Format("PhuLieu");
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("Bao Cao");
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
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PhuLieu");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "NAM THANH BINH COMPANY"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "PHU LIEU"; range.Style.Font.Bold = true;
                    PhuLieuLiP.dtXuatEX(dtsave, worksheet);
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        private void textEdit11_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {

                int rowHandle = view.FocusedRowHandle;


                if (rowHandle >= 0)
                {

                    int idValue = int.Parse(view.GetRowCellValue(rowHandle, "ID").ToString());
                    loadThongTinPL(idValue);

                }
            }
        }

        private void gridView1_ColumnChanged(object sender, EventArgs e)
        {

        }

        private void gridView1_RowClick(object sender, RowClickEventArgs e)
        {

        }

        private void barButtonItem25_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
                LuuDong();

        }
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }

        
    }
}