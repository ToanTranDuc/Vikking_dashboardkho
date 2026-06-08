using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPKhoVai : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<ERPKhoVaiEntity> lstKVNL;
        List<ERPKhoVaiEntity> lstKVPL;
        List<ERPKhoVaiEntity> lstKhovaiKT;
        List<int> lstRowUpdate = new List<int>();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        string _maKH = string.Empty;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        bool isTabNL = true;
      

        public frmERPKhoVai()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstKVNL = new List<ERPKhoVaiEntity>();
            lstKhovaiKT = new List<ERPKhoVaiEntity>();
            lstKVPL = new List<ERPKhoVaiEntity>();
        }


        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();

            //keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            Init();
            LoadDSKhoVai(false);

        }

        private void Init()
        {


        }


        private List<ActionControl> InitActionKeyDown()
        {
            //actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
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
                //Them.Enabled = false;
                layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            gVNL.OptionsBehavior.Editable = true;
            Luu.Enabled = true;
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                

                    if (_allowAdd)
                    {
                        //Them.Enabled = true;
                        //actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        // actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        //actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        //Luu.Enabled = false;
                        //actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                   // gVNL.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        //Them.Enabled = false;
                        //actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        //actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        //actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        //actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    //gVNL.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        //Them.Enabled = false;
                        // actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        //actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        //actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        // actionControlSave.Enabled = true;
                    }

                    break;
            }
        }

        private void LoadDSKhoVai(bool isFromSave)
        {
            try
            {

                _status = ResourceURL.EventStatus.View;


                if (isTabNL)
                {
                    string url = $"{URL}ERPThuVienVT/GetKVNL";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (!string.IsNullOrEmpty(json))
                    {
                        lstKVNL = JsonConvert.DeserializeObject<List<ERPKhoVaiEntity>>(json);

                    }
                    gCNL.DataSource = lstKVNL;
                    GridViewUpdateStatus(_status);
                    if (isFromSave && FocusedIndex >= 0)
                    {
                        gVNL.FocusedRowHandle = FocusedIndex;
                        this.ActiveControl = gCNL;
                    }
                }
                else
                {
                    string url = $"{URL}ERPThuVienVT/GetKVPL";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (!string.IsNullOrEmpty(json))
                    {
                        lstKVPL = JsonConvert.DeserializeObject<List<ERPKhoVaiEntity>>(json);

                    }
                    gCPL.DataSource = lstKVPL;
                    GridViewUpdateStatus(_status);
                    if (isFromSave && FocusedIndex >= 0)
                    {
                        gVPL.FocusedRowHandle = FocusedIndex;
                        this.ActiveControl = gCPL;
                    }

                }

            }
            catch (Exception ex)
            {

            }


        }





        private void gridViewKhoVai_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }



        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            string _txtKho = txtKho?.EditValue?.ToString() ?? "";
            List<string> _lstKho = _txtKho.ToString().Trim().Split(':').ToList();
            List<ERPKhoVaiEntity> _lstAddKhoVai = (isTabNL ? gCNL : gCPL).DataSource as List<ERPKhoVaiEntity>;
            int focusedRow = (isTabNL ? lstKVNL : lstKVPL).Count;
            int khoIndex = -1;
            foreach (var kho in _lstKho)
            {
                khoIndex++;

                if (!string.IsNullOrEmpty(kho))
                {
                    ERPKhoVaiEntity khoEntity = new ERPKhoVaiEntity(kho.ToString().Trim(), true);
                    bool existsInList = (isTabNL ? lstKVNL : lstKVPL).Any(existing =>
                          existing.KhoVai.ToString().Trim().ToUpper() == khoEntity.KhoVai.ToString().Trim().ToUpper()

                    ); ;
                    if (existsInList)
                    {
                        XtraMessageBox.Show($"Khổ vải đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    else
                    {
                        _lstAddKhoVai.Add(khoEntity);
                    }
                }
            }

            (isTabNL ? gCNL : gCPL).DataSource = _lstAddKhoVai;
            (isTabNL ? gVNL : gVPL).RefreshData();
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            txtKho.Text = null;
        }

       

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private async void XoaDong()
        {
            try
            {
                int focusedRowHandle = (isTabNL ? gVNL : gVPL).FocusedRowHandle;
                ERPKhoVaiEntity dr = (isTabNL ? gVNL : gVPL).GetRow(focusedRowHandle) as ERPKhoVaiEntity;
                if (dr == null) return;
                string khovaiid = dr.KhoVaiID.ToString();
                string urlCheck = $"{URL}ERPThuVienVT/GetCheckKV?khovaiid={khovaiid}";
                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                if (jsonCheck != "[]")
                {
                    XtraMessageBox.Show("Khổ/Size này đã được sử dụng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if ((isTabNL ? gVNL : gVPL).FocusedRowHandle >= 0)
                    {
                        int ID = Int32.Parse((isTabNL ? gVNL : gVPL).GetFocusedRowCellValue(colID)?.ToString());
                        string url = URL + $"ERPThuVienVT/DeleteKV?id={ID}&user={GlobleData.UserName}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                            LoadDSKhoVai(false);
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void NapLaiDong()
        {

            LoadDSKhoVai(false);
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }


        private void tab_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            var selectedTabPage = e.Page;

            if (selectedTabPage.Name == "tabNL")
            {
                isTabNL = true;
                LoadDSKhoVai(false);
            }
            else
            {
                isTabNL = false;
                LoadDSKhoVai(false);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string _txtKho = textEdit1?.EditValue?.ToString() ?? "";
            List<string> _lstKho = _txtKho.ToString().Trim().Split(':').ToList();
            List<ERPKhoVaiEntity> _lstAddKhoVai = (isTabNL ? gCNL : gCPL).DataSource as List<ERPKhoVaiEntity>;
            int focusedRow = (isTabNL ? lstKVNL : lstKVPL).Count;
            int khoIndex = -1;
            foreach (var kho in _lstKho)
            {
                khoIndex++;

                if (!string.IsNullOrEmpty(kho))
                {
                    ERPKhoVaiEntity khoEntity = new ERPKhoVaiEntity(kho.ToString().Trim(), false);
                    bool existsInList = (isTabNL ? lstKVNL : lstKVPL).Any(existing =>
                          existing.KhoVai.ToString().Trim().ToUpper() == khoEntity.KhoVai.ToString().Trim().ToUpper()

                    ); ;
                    if (existsInList)
                    {
                        XtraMessageBox.Show($"Khổ/Size đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    else
                    {
                        _lstAddKhoVai.Add(khoEntity);
                    }
                }
            }


            (isTabNL ? gCNL : gCPL).DataSource = _lstAddKhoVai;
            (isTabNL ? gVNL : gVPL).RefreshData();
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            textEdit1.Text = null;
        }

        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;

                if ((_status != ResourceURL.EventStatus.View) && (isTabNL ? gVNL : gVPL).FocusedRowHandle >= 0)
                {
                    FocusedIndex = (isTabNL ? gVNL : gVPL).FocusedRowHandle;
                }

                List<ERPKhoVaiEntity> _lstAll = new List<ERPKhoVaiEntity>();
                _lstAll = (isTabNL ? gCNL : gCPL).DataSource as List<ERPKhoVaiEntity>;
                bool coKhoVaiRong = CheckKhoVaiNullOrEmpty(_lstAll);
                if (coKhoVaiRong)
                {
                    XtraMessageBox.Show("Có Khổ/Size rỗng.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (KiemTraTrungKhoVai(_lstAll))
                {
                    XtraMessageBox.Show("Có Khổ/Size bị trùng.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string msResult = "";
                if (_lstAll == null || (_lstAll != null && _lstAll.Count == 0))
                {
                    LoadDSKhoVai(true);
                    return;
                }


                clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_KhoVai");
                string url = string.Format("{0}?", URL + "ERPThuVienVT/PostKV");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstAll); }).Result;


                if (msResult.ToLower() == "true")
                {
                    LoadDSKhoVai(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
            }
            catch (Exception ex)
            {

            }

        }
        private bool CheckKhoVaiNullOrEmpty(List<ERPKhoVaiEntity> lst)
        {
            if (lst == null || lst.Count == 0)
                return false;

            foreach (var item in lst)
            {
                if (string.IsNullOrWhiteSpace(item.KhoVai))
                {
                    return true;
                }
            }
            return false;
        }



        private bool KiemTraTrungKhoVai(List<ERPKhoVaiEntity> danhSach)
        {
            return danhSach
               .GroupBy(x => (x.KhoVai ?? "").Trim().Replace(" ", "").ToUpper())
                .Any(g => g.Count() > 1);
        }


        private void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                // Lấy dữ liệu từ clipboard
                string clipboardText = Clipboard.GetText();

                // Chỉ can thiệp nếu dữ liệu có nhiều dòng
                if (!string.IsNullOrEmpty(clipboardText) && (clipboardText.Contains("\n") || clipboardText.Contains("\r")))
                {
                    // Lấy đối tượng TextEdit
                    TextEdit editor = sender as TextEdit;

                    // Loại bỏ các ký tự xuống dòng
                    string singleLineText = Regex.Replace(clipboardText, @"\r\n?|\n", " ");

                    // Chèn văn bản đã được xử lý vào vị trí con trỏ hiện tại
                    // Hoặc thay thế văn bản đang được chọn
                    editor.SelectedText = singleLineText;

                    // Đánh dấu rằng phím này đã được xử lý
                    // để ngăn TextEdit thực hiện hành vi dán mặc định của nó
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Ngăn tiếng "bíp" của hệ thống
                }
            }
        }
        private void txtKho_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                // Lấy dữ liệu từ clipboard
                string clipboardText = Clipboard.GetText();

                // Chỉ can thiệp nếu dữ liệu có nhiều dòng
                if (!string.IsNullOrEmpty(clipboardText) && (clipboardText.Contains("\n") || clipboardText.Contains("\r")))
                {
                    // Lấy đối tượng TextEdit
                    TextEdit editor = sender as TextEdit;

                    // Loại bỏ các ký tự xuống dòng
                    string singleLineText = Regex.Replace(clipboardText, @"\r\n?|\n", " ");

                    // Chèn văn bản đã được xử lý vào vị trí con trỏ hiện tại
                    // Hoặc thay thế văn bản đang được chọn
                    editor.SelectedText = singleLineText;

                    // Đánh dấu rằng phím này đã được xử lý
                    // để ngăn TextEdit thực hiện hành vi dán mặc định của nó
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Ngăn tiếng "bíp" của hệ thống
                }
            }
        }
        private void gC_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {

                GridControl grid = sender as GridControl;
                GridView view = grid.FocusedView as GridView;
                if (view != null && view.OptionsBehavior.Editable)
                {
                    string clipboardText = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(clipboardText) && (clipboardText.Contains("\n") || clipboardText.Contains("\r")))
                    {
                        string singleLineText = Regex.Replace(clipboardText, @"\r\n?|\n", " ");
                        view.ShowEditor();
                        if (view.ActiveEditor is DevExpress.XtraEditors.TextEdit editor)
                        {
                            editor.SelectedText = singleLineText;
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        else
                        {
                            view.SetFocusedValue(singleLineText);
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                    }
                }
            }
        }

         /*Write log when modify row*/
        List<LogThuvienEntity> lstLog = new List<LogThuvienEntity>();
        private void gridViewKhoVai_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            lstRowUpdate.Add(e.RowHandle);

            ERPKhoVaiEntity row_focus = (isTabNL ? gVNL : gVPL).GetFocusedRow() as ERPKhoVaiEntity;
            if(row_focus != null && row_focus.ID > 0)
            {
                string content = clsWriteLogThuVienLib.FormatRow(row_focus);
                var Query = lstLog.FirstOrDefault(x => x.ID == row_focus.ID);
                if (Query != null)
                {
                    Query.Content = content;
                }
                else
                {
                    lstLog.Add(new LogThuvienEntity
                    {
                        ID = row_focus.ID,
                        Action = $"Sửa {this.Text}",
                        Module = this.Name,
                        Content = content,
                        UserID = GlobleData.UserName,
                        CreatedDate = DateTime.Now

                    });
                }
            }
            

        }

     

    }
}