using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using DevExpress.XtraLayout;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraLayout.Utils;
using System.Web;
using DevExpress.XtraEditors.Popup;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Win.Modules.DicForm;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{

    public partial class frmSaveTL_KL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maHang = string.Empty, _dausize = string.Empty, _size = string.Empty, _sizeid = string.Empty, _maNoiDen = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        public string styleID { get; set; }
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        public static string _styleID;
        public frmSaveTL_KL()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            dataTbale();
            grvCaiDatTS.OptionsSelection.MultiSelect = true;
            grvCaiDatTS.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }

        protected override void OnLoad(EventArgs e)
        {
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            Load_lookUp();
            grvCaiDatTS.ShownEditor += grvCaiDatTS_ShownEditor;
            if (!string.IsNullOrEmpty(styleID))
            {
                ApplyStyleIDFromTongQuan();
            }
        }

        private void ApplyStyleIDFromTongQuan()
        {
            try
            {
                searchLookUpEdit1.EditValue = styleID;
                _styleID = styleID;
                load_GridView();
                LoadQuiCach();
                Load_NoiDen_TheoMaHang();

                if (searchLookUpEdit2.Properties.DataSource != null)
                {
                    DataTable dtNoiDen = searchLookUpEdit2.Properties.DataSource as DataTable;
                    if (dtNoiDen != null && dtNoiDen.Rows.Count > 0)
                    {
                        searchLookUpEdit2.EditValueChanged -= searchLookUpEdit2_EditValueChanged;
                        searchLookUpEdit2.EditValue = dtNoiDen.Rows[0]["MaNoiDen"];
                        searchLookUpEdit2.EditValueChanged += searchLookUpEdit2_EditValueChanged;
                        load_GridView();
                        LoadQuiCach();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi load dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Clear()
        {
            searchLookUpEdit1.EditValue = null; 
            searchLookUpEdit2.EditValue = null; 
            pcbtxtEdit.EditValue = "0";
            pptxtEdit.EditValue = "0";
            packctntxtEdit.EditValue = "0";
            kltxtEdit.EditValue = "0";
            txtEdtGhiChu.EditValue = "";
            _maHang = string.Empty;
            _dausize = string.Empty;
            _size = string.Empty;
            _sizeid = string.Empty;
            _maNoiDen = string.Empty;
            styleID = null;
            _styleID = null;
            if (dgrTLKL.DataSource is DataTable dtTLKL)
            {
                dtTLKL.Clear();
                dtTLKL.AcceptChanges();
            }
            else
            {
                dgrTLKL.DataSource = null;
            }
            grvCaiDatTS.RefreshData();
            if (grcQuiCach.DataSource is DataTable dtQC)
            {
                dtQC.Clear();
                dtQC.AcceptChanges();
            }
            else
            {
                grcQuiCach.DataSource = null;
            }
        }
        private List<ActionControl> InitActionKeyDown()
        {

            actionControlSave = new ActionControl(Save, true, ActionType.Save, true);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, true);

            lstActionControls = new List<ActionControl> {
                 actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue is null) return;
            pcbtxtEdit.EditValue = "0";
            pptxtEdit.EditValue = "0";
            packctntxtEdit.EditValue = "0";
            kltxtEdit.EditValue = "0";
            txtEdtGhiChu.EditValue = "";
            load_GridView();
            LoadQuiCach();
            Load_NoiDen_TheoMaHang();
        }

        private void Save()
        {
            try
            {
                this.ActiveControl = button1;
                string noidentxt = searchLookUpEdit2.EditValue != null ? searchLookUpEdit2.Text.ToString() : "";
                if(noidentxt == "")
                {
                    XtraMessageBox.Show("Vui lòng chọn nơi đến!",
                   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataTable dtNoiDen = LoadNoiDen(false);
                if (dtNoiDen == null || dtNoiDen.Rows.Count == 0) return;
                var ketQua = dtNoiDen.AsEnumerable()
                      .Where(row => row.Field<string>("NoiDen") == "Chung")
                      .FirstOrDefault();
                string _maNoiDenChung = "";
                if (ketQua != null && noidentxt.ToUpper() == "CHUNG")
                {
                    _maNoiDenChung = ketQua["MaNoiDen"].ToString();
                }
                else
                {
                    _maNoiDenChung = searchLookUpEdit2.EditValue?.ToString() ?? "0";

                }
                string _maNoiDen = searchLookUpEdit2.EditValue?.ToString() ?? "0";

                DataTable dt = dgrTLKL.DataSource as DataTable;

                if (dt == null || dt.Rows.Count == 0) return;

                if (dt.Columns.Contains("checkQuiCach"))
                {
                    dt.Columns.Remove("checkQuiCach");
                }
                if (dt.Columns.Contains("TenQC"))
                {
                    dt.Columns.Remove("TenQC");
                }
                var tblTemp = dt.AsEnumerable().Where(x => Convert.ToBoolean(x["Chon"]) == true);

                dt = tblTemp.CopyToDataTable();
                foreach (DataRow row in dt.Rows)
                {
                    row["MaNoiDen"] = _maNoiDenChung;
                    row["GhiChu"] = txtEdtGhiChu.EditValue.ToString();
                }    
                   
                foreach (DataRow data in dt.Rows)
                {
                    if (data["MaNoiDen"] == "0")
                    {
                        data["MaNoiDen"] = _maNoiDenChung;
                    }
                }
                dt.Columns.Remove("ChonQC1");
                string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Post?action=PostTL_KL");
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, dt); }).Result;
                if (json.ToLower() == "true")
                {
                    //clsWaitForm.ShowSuccessForm(this, 2000);
                    pcbtxtEdit.EditValue = "0";
                    pptxtEdit.EditValue = "0";
                    packctntxtEdit.EditValue = "0";
                    kltxtEdit.EditValue = "0";
                    load_GridView();
                }
                else XtraMessageBox.Show(json);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void load_Page()
        {
            load_GridView();
            LoadQuiCach();
        }
        private void dgrTLKL_ProcessGridKey(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:
                    if (_allowAdd || _allowEdit)
                        SaveAll();
                    break;
                case Keys.F5:
                    load_Page();
                    break;
            }

        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string maHang = searchLookUpEdit1.EditValue != null ? searchLookUpEdit1.Text.ToString() : "";
            if (maHang == "")
            {
                XtraMessageBox.Show("Vui lòng chọn Mã hàng!",
                   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (LuuNoiDenChung())
            {
                Save();
            }
        }
        private DataTable LoadNoiDen(bool checkChange = true, bool checkSave = true)
        {
            string maHang = searchLookUpEdit1.EditValue.ToString();
            string url = URL + $"CaiDatThongSoKHDT/Get?Action=GetNoiDen&Para1={maHang}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            if (json == "[]") return null;
            DataTable dt;

            dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (checkSave == true)
                Load_NoiDen_TheoMaHang(checkChange);
            return dt;
        }
        private bool LuuNoiDenChung()
        {
            DataTable dtNoiDen = LoadNoiDen(false, false);
            if (dtNoiDen != null && dtNoiDen.Rows.Count > 0) return true;
            
            DataTable dtSave = new DataTable();
            //var chungRow = dtNoiDen?.AsEnumerable()
            //    .FirstOrDefault(r =>
            //    r.Field<string>("StyleID") == _styleID &&
            //    r.Field<string>("NoiDen") == "Chung");
            //if (chungRow != null)
            //{
            //    return true;
            //}

            dtSave.Columns.Add("MaHang", typeof(string));
            dtSave.Columns.Add("MaNoiDen", typeof(string));
            dtSave.Columns.Add("NoiDen", typeof(string));
            DataRow dr = dtSave.NewRow();
            dr["MaHang"] = _styleID;
            dr["MaNoiDen"] = "0";
            dr["NoiDen"] = "Chung";
            dtSave.Rows.Add(dr);
            string url = URL + "CaiDatThongSoKHDT/PostNoiDen";
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (json == "True")
            {
                return true;
            }
            return false;
        }

        private void NapLaiDong()
        {
            load_GridView();
            LoadQuiCach();
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();

        }

        private void dgrTLKL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }
        private void Load_lookUp()
        {
            string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetMaHang&Para1=A&Para2={_dausize}&Para3={_size}&Para4={_sizeid}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = tbl;
            searchLookUpEdit1.Properties.DisplayMember = "MaHang";
            searchLookUpEdit1.Properties.ValueMember = "StyleID";
            searchLookUpEdit1.Properties.NullText = "[Chọn giá trị]";
            if (tbl.Rows.Count != 0) searchLookUpEdit1.EditValue = _styleID == "" ? tbl.Rows[0]["StyleID"] : _styleID;
            else
            {
                dgrTLKL.DataSource = null;
                return;
            }
            load_GridView();

        }
        #region Thoai
        private void Load_NoiDen_TheoMaHang(bool checkChange = true, bool isLoad = false)
        {
            if (searchLookUpEdit1.EditValue == null) return;
            string maHang = searchLookUpEdit1.EditValue.ToString();
            string url = URL + $"CaiDatThongSoKHDT/Get?Action=GetNoiDen&Para1={maHang}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            DataTable dt;
            if (string.IsNullOrWhiteSpace(json) || json == "[]")
            {
                dt = new DataTable();
                dt.Columns.Add("MaNoiDen", typeof(string));
                dt.Columns.Add("NoiDen", typeof(string));
                dt.Rows.Add("0", "Chung");
            }
            else
            {
                dt = JsonConvert.DeserializeObject<DataTable>(json);
                //var allRows = dt.Select("MaNoiDen = '0'");
                //foreach (var row in allRows)
                //{
                //    row.Delete();
                //}
                //dt.AcceptChanges();
                //if (dt.Rows.Count == 0)
                //    dt.Rows.Add("0", "Chung");
                var tempRows = dt.Select("MaNoiDen = '0'");
                foreach (var row in tempRows)
                    row.Delete();
                dt.AcceptChanges();
            }
            searchLookUpEdit2.Properties.DataSource = dt;
            searchLookUpEdit2.Properties.DisplayMember = "NoiDen";
            searchLookUpEdit2.Properties.ValueMember = "MaNoiDen";
            searchLookUpEdit2.Properties.NullText = "[Chọn nơi đến]";
            if (dt.Rows.Count == 1)
            {
                if (!checkChange)
                    searchLookUpEdit2.EditValueChanged -= searchLookUpEdit2_EditValueChanged;
                searchLookUpEdit2.EditValue = dt.Rows[0]["MaNoiDen"];
                if (!checkChange)
                    searchLookUpEdit2.EditValueChanged += searchLookUpEdit2_EditValueChanged;
            }
            else if (dt.Rows.Count > 1)
            {
                if (!checkChange)
                    searchLookUpEdit2.EditValueChanged -= searchLookUpEdit2_EditValueChanged;
                searchLookUpEdit2.EditValue = _maNoiDen;
                if (!checkChange)
                    searchLookUpEdit2.EditValueChanged += searchLookUpEdit2_EditValueChanged;
            }
        }
        private void searchLookUpEdit2_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl)?.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
                var ownerEdit = popupForm?.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;

                var layout = popupForm?.Controls
                            .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                            .FirstOrDefault()?
                            .Controls
                            .OfType<LayoutControl>()
                            .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    layout.BeginUpdate();

                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // Thiết lập cột layout: 3 cột (Empty - Clear - Quản lý)
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // Empty space bên trái
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize }); // Clear button
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize }); // Quản lý button

                    // 3 hàng: trên - giữa (button) - dưới
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    layout.Root.AddItem(buttonGroup);

                    // Empty space bên trái
                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // Nút Clear ở cột 1
                    var clearButton = new SimpleButton() { Name = "btnClear", Text = "Clear" };
                    clearButton.Click += clearButton_Click;
                    var layoutItemClear = new LayoutControlItem()
                    {
                        Control = clearButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(80, 30),
                        MaxSize = new Size(80, 30)
                    };
                    layoutItemClear.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemClear.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemClear);

                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Quản lý nơi đến" };
                    khaiBaoButton.Click += btnQLNoiDen_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 2;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    // Empty space phía trên
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    // Empty space phía dưới
                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch { }
        }

        private DevExpress.XtraGrid.GridControl FindParentGridControl(Control control)
        {
            while (control != null && !(control is DevExpress.XtraGrid.GridControl))
                control = control.Parent;
            return control as DevExpress.XtraGrid.GridControl;
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            while (control != null && !(control is DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm))
                control = control.Parent;

            if (control is DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm popupForm)
            {
                var searchLookUpEdit = popupForm.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;
                if (searchLookUpEdit != null)
                {
                    var view = searchLookUpEdit.Properties.View;
                    if (view != null)
                    {
                        view.ClearSelection();
                        view.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
                    }
                    searchLookUpEdit.EditValue = null;

                    var gridControl = FindParentGridControl(searchLookUpEdit);
                    if (gridControl != null)
                    {
                        var gridView = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView
                                       ?? gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (gridView != null)
                        {
                            var column = gridView.FocusedColumn;
                            if (column != null && gridView.IsFilterRow(gridView.FocusedRowHandle))
                            {
                                gridView.SetAutoFilterValue(column, null, DevExpress.XtraGrid.Columns.AutoFilterCondition.Equals);
                                //gridView.CloseEditor();
                            }
                        }
                    }
                }
                searchLookUpEdit.ClosePopup();
            }
        }
        private async void btnQLNoiDen_Click(object sender, EventArgs e)
        {
            string styleId = searchLookUpEdit1.EditValue?.ToString();
            string maHang = searchLookUpEdit1.Text?.ToString();
            if (string.IsNullOrWhiteSpace(maHang))
            {
                XtraMessageBox.Show("Vui lòng chọn Mã hàng trước!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //DataTable dtBefore = LoadNoiDen();
            //string idChungOld = dtBefore?.AsEnumerable()
            //                        .FirstOrDefault(r => r.Field<string>("NoiDen") == "Chung")
            //                        ?.Field<string>("MaNoiDen");
            //frmQCDT_NoiDen frm = new frmQCDT_NoiDen(styleId, maHang);
            //frm.StartPosition = FormStartPosition.CenterParent;
            //frm.ShowDialog(this);
            //Load_NoiDen_TheoMaHang();
            //DataTable dtAfter = LoadNoiDen();

            DataTable dtBefore = LoadNoiDen(false);
            var beforeIds = dtBefore?
                .AsEnumerable()
                .Select(r => r.Field<string>("MaNoiDen"))
                .ToList() ?? new List<string>();
            frmQCDT_NoiDen frm = new frmQCDT_NoiDen(styleId, maHang);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
            Load_NoiDen_TheoMaHang();
            DataTable dtAfter = LoadNoiDen();
            if (dtAfter == null || dtAfter.Rows.Count == 0)
            {
                searchLookUpEdit2.EditValue = null;
                return;
            }
            var newRow = dtAfter.AsEnumerable()
                .FirstOrDefault(r => !beforeIds.Contains(r.Field<string>("MaNoiDen")));
            if (newRow != null)
            {
                searchLookUpEdit2.EditValue = newRow["MaNoiDen"];
            }
            else
            {
                searchLookUpEdit2.EditValue = dtAfter.Rows[0]["MaNoiDen"];
            }

        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            load_GridView();
            LoadQuiCach();
        }


        private void pcbtxtEdit_EditValueChanged(object sender, EventArgs e)
        {
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;

            if (!int.TryParse(pcbtxtEdit.EditValue?.ToString(), out int pcbValue))
                pcbValue = 0;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                if ((bool)tbl.Rows[i]["Chon"])
                    tbl.Rows[i]["PCB"] = pcbValue;
            }
            dgrTLKL.DataSource = tbl;
        }


        private void pptxtEdit_EditValueChanged(object sender, EventArgs e)
        {
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;

            if (!int.TryParse(pptxtEdit.EditValue?.ToString(), out int ppValue))
                ppValue = 0;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                if ((bool)tbl.Rows[i]["Chon"])
                    tbl.Rows[i]["PCB_Pack"] = ppValue;
            }
            dgrTLKL.DataSource = tbl;
        }

        private void packctntxtEdit_EditValueChanged(object sender, EventArgs e)
        {
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;

            if (!int.TryParse(packctntxtEdit.EditValue?.ToString(), out int packctnValue))
                packctnValue = 0;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                if ((bool)tbl.Rows[i]["Chon"])
                    tbl.Rows[i]["Pack_Ctn"] = packctnValue;
            }
            dgrTLKL.DataSource = tbl;
        }

        private void kltxtEdit_EditValueChanged(object sender, EventArgs e)
        {
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;

            if (!int.TryParse(kltxtEdit.EditValue?.ToString(), out int klValue))
                klValue = 0;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                if ((bool)tbl.Rows[i]["Chon"])
                    tbl.Rows[i]["SL_KhoiLuong"] = klValue;
            }
            dgrTLKL.DataSource = tbl;
        }

        private void barCheckLap_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barCheckLap.Checked)
            {
                barCheckLap2.Checked = false;
            }
        }

        private void barCheckLap2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barCheckLap2.Checked)
            {
                barCheckLap.Checked = false;
            }              
        }

        private void grvCaiDatTS_ShownEditor(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            string field = view.FocusedColumn?.FieldName;

            if (field != "PCB" && field != "Pack_Ctn" && field != "PCB_Pack" && field != "SL_KhoiLuong") return;

            var editor = view.ActiveEditor as TextEdit;
            if (editor == null) return;
            editor.KeyPress -= grvCaiDatTS_KeyPress;
            editor.KeyPress += grvCaiDatTS_KeyPress;

        }
        private void grvCaiDatTS_KeyPress(object sender, KeyPressEventArgs e)
        {
            var editor = sender as TextEdit;
            if (editor == null) return;

            GridView view = grvCaiDatTS;
            string field = view.FocusedColumn?.FieldName;
            if (field == null) return;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (!barCheckLap.Checked && !barCheckLap2.Checked) return;

            var drFocus = view.GetFocusedDataRow();
            if (drFocus == null) return;

            var data = dgrTLKL.DataSource as DataTable;
            if (data == null || data.Rows.Count == 0) return;

            string curText = editor.EditValue?.ToString() ?? "";
            string newText = "";
            if (e.KeyChar == '\b')
            {
                if (editor.SelectionLength > 0)
                {
                    int start = editor.SelectionStart;
                    newText = curText.Remove(start, editor.SelectionLength);
                }
                else if (curText.Length > 0 && editor.SelectionLength > 0)
                {
                    newText = curText.Remove(editor.SelectionLength - 1, 1);
                }
                newText = string.IsNullOrEmpty(newText) ? "0" : newText;
            }
            else if (char.IsDigit(e.KeyChar) || e.KeyChar == '.')
            {
                if (editor.SelectionLength > 0)
                {
                    int start = editor.SelectionStart;
                    newText = curText.Remove(start, editor.SelectionLength);
                    newText = newText.Insert(start, e.KeyChar.ToString());
                }
                else
                {
                    int start = editor.SelectionStart;
                    newText = curText.Insert(start, e.KeyChar.ToString());
                }
            }
            else
                return;

            object newValue = null;
            if (field == "SL_KhoiLuong")
            {
                if (double.TryParse(newText, out double dValue))
                    newValue = dValue;
            }
            else
            {
                if (double.TryParse(newText, out double iValue))
                    newValue = iValue;
            }
            if (newValue == null) return;

            if (barCheckLap.Checked)
            {
                string focusedSizeID = drFocus["SizeID"].ToString();
                foreach (DataRow dr in data.Rows)
                {
                    if (dr["SizeID"].ToString() == focusedSizeID)
                    {
                        dr[field] = newValue;
                    }
                }
            }
            else if (barCheckLap2.Checked)
            {
                string focusedDauSizeID = drFocus["DauSizeID"].ToString();
                foreach (DataRow dr in data.Rows)
                {
                    if (dr["DauSizeID"].ToString() == focusedDauSizeID)
                    {
                        dr[field] = newValue;
                    }
                }
            }
            view.RefreshData();

        }

        #endregion
        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                DataTable tbl = dgrTLKL.DataSource as DataTable;
                if (tbl.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn mã hàng có dữ liệu", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }

                if (e.KeyChar.ToString() != null)
                {
                    string text1 = "";
                    string PCB = "";
                    int PCBInt = 0;
                    if (e.KeyChar.ToString() == "\b")
                    {
                        string textSubString = pcbtxtEdit.EditValue.ToString();
                        text1 = textSubString.Remove(textSubString.Length - 1);
                        PCB = text1 == "" ? "0" : text1;
                        PCBInt = Convert.ToInt32(PCB);
                    }
                    else
                    {
                        text1 = pcbtxtEdit.EditValue == null ? "" : pcbtxtEdit.EditValue.ToString();
                        PCB = text1 + e.KeyChar.ToString();
                        PCBInt = Convert.ToInt32(PCB);
                    }


                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        if ((bool)tbl.Rows[i]["Chon"])
                            tbl.Rows[i]["PCB"] = PCBInt;
                    }
                    dgrTLKL.DataSource = tbl;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                DataTable tbl = dgrTLKL.DataSource as DataTable;
                if (tbl.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn mã hàng có dữ liệu", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }

                if (e.KeyChar.ToString() != null)
                {
                    string text1 = "";
                    string PCB = "";
                    int PCBInt = 0;
                    if (e.KeyChar.ToString() == "\b")
                    {
                        string textSubString = pptxtEdit.EditValue.ToString();
                        text1 = textSubString.Remove(textSubString.Length - 1);
                        PCB = text1 == "" ? "0" : text1;
                        PCBInt = Convert.ToInt32(PCB);
                    }
                    else
                    {
                        text1 = pptxtEdit.EditValue == null ? "" : pptxtEdit.EditValue.ToString();
                        PCB = text1 + e.KeyChar.ToString();
                        PCBInt = Convert.ToInt32(PCB);
                    }


                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        tbl.Rows[i]["PCB_Pack"] = PCBInt;
                    }
                    dgrTLKL.DataSource = tbl;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void textEdit3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                DataTable tbl = dgrTLKL.DataSource as DataTable;
                if (tbl.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn mã hàng có dữ liệu", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }

                if (e.KeyChar.ToString() != null)
                {
                    string text1 = "";
                    string PCB = "";
                    int PCBInt = 0;
                    if (e.KeyChar.ToString() == "\b")
                    {
                        string textSubString = packctntxtEdit.EditValue.ToString();
                        text1 = textSubString.Remove(textSubString.Length - 1);
                        PCB = text1 == "" ? "0" : text1;
                        PCBInt = Convert.ToInt32(PCB);
                    }
                    else
                    {
                        text1 = packctntxtEdit.EditValue == null ? "" : packctntxtEdit.EditValue.ToString();
                        PCB = text1 + e.KeyChar.ToString();
                        PCBInt = Convert.ToInt32(PCB);
                    }


                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        tbl.Rows[i]["Pack_Ctn"] = PCBInt;
                    }
                    dgrTLKL.DataSource = tbl;
                }
            }
            catch (Exception ex)
            {

            }

        }
        DataTable saveTable = new DataTable();
        private void dataTbale()
        {
            saveTable.Columns.Add("StyleID", typeof(string));
            saveTable.Columns.Add("MaQC", typeof(string));
            saveTable.Columns.Add("Size", typeof(string));
            saveTable.Columns.Add("SizeID", typeof(string));
            saveTable.Columns.Add("DauSize", typeof(string));
            saveTable.Columns.Add("DauSizeID", typeof(string));
            saveTable.Columns.Add("SapXep", typeof(int));
            saveTable.Columns.Add("From", typeof(int));
            saveTable.Columns.Add("To", typeof(int));
            saveTable.Columns.Add("MaNoiDen", typeof(string));
            saveTable.Columns.Add("NguoiTao", typeof(string));
        }
        private void SaveQC()
        {
            try
            {
                this.ActiveControl = button1;
                string noidentxt = searchLookUpEdit2.EditValue != null ? searchLookUpEdit2.Text.ToString() : "";
                DataTable dtNoiDen = LoadNoiDen(false);
                var ketQua = dtNoiDen.AsEnumerable()
                      .Where(row => row.Field<string>("NoiDen") == "Chung")
                      .FirstOrDefault();
                string _maNoiDenChung = "";
                if (ketQua != null && noidentxt.ToUpper() == "CHUNG")
                {
                    _maNoiDenChung = ketQua["MaNoiDen"].ToString();
                }
                else
                {
                    _maNoiDenChung = searchLookUpEdit2.EditValue?.ToString() ?? "0";

                }
                saveTable.Clear();
                DataTable dt = dgrTLKL.DataSource as DataTable;
                foreach (DataRow row in dt.Rows)
                    row["MaNoiDen"] = _maNoiDenChung;
                DataTable tbl = grcQuiCach.DataSource as DataTable;
                if (tbl is null || tbl.Rows.Count == 0 || dt is null || dt.Rows.Count == 0) return;

                var tblTemp = tbl.AsEnumerable().Where(x => Convert.ToBoolean(x["Chon"]) == true);
                var tblTemp1 = dt.AsEnumerable().Where(x => Convert.ToBoolean(x["ChonQC"]) == true);

                if (tblTemp1.Count() == 0)
                {
                    //MessageBox.Show("Vui lòng chọn Size để cài Qui cách");
                    return;
                }
                dt = tblTemp1.CopyToDataTable();
                _maNoiDen = searchLookUpEdit2.EditValue?.ToString() ?? "0";
                if (!dt.Columns.Contains("TenQC"))
                {
                    dt.Columns.Add("TenQC", typeof(string));
                }
                if (!saveTable.Columns.Contains("MaNoiDen"))
                {
                    saveTable.Columns.Add("MaNoiDen", typeof(string));
                }

                if (tblTemp.Count() > 0)
                {
                    tbl = tblTemp.CopyToDataTable();
                    string tenQuiCachGop = string.Join(",",
                        tbl.AsEnumerable()
                           .Select(x => x["TenQuiCach"] != null ? x["TenQuiCach"].ToString().Trim() : "")
                           .Where(x => !string.IsNullOrEmpty(x))
                           .ToList());
                    DataTable dtGridTS = dgrTLKL.DataSource as DataTable;
                    foreach (DataRow row in dt.Rows)
                    {
                        string keyValue = "";
                        string fieldName = "";
                        if (barCheckLap2.Checked)
                        {
                            keyValue = row["DauSizeID"].ToString();
                            fieldName = "DauSizeID";
                        }
                        else if (barCheckLap.Checked)
                        {
                            keyValue = row["SizeID"].ToString();
                            fieldName = "SizeID";
                        }
                        else
                        {
                            keyValue = row["SizeID"].ToString() + "|" + row["DauSizeID"].ToString();
                            fieldName = "UniqueRow";
                        }
                        //var rowsToUpdate = dtGridTS.AsEnumerable()
                        //    .Where(r => r[fieldName].ToString() == keyValue);
                        var rowsToUpdate = dtGridTS.AsEnumerable();

                        if (barCheckLap2.Checked)
                        {
                            rowsToUpdate = rowsToUpdate.Where(r => r["DauSizeID"].ToString() == row["DauSizeID"].ToString());
                        }
                        else if (barCheckLap.Checked)
                        {
                            rowsToUpdate = rowsToUpdate.Where(r => r["SizeID"].ToString() == row["SizeID"].ToString());
                        }
                        else
                        {
                            // Không nhóm → chỉ cập nhật đúng dòng này
                            rowsToUpdate = rowsToUpdate.Where(r =>
                                r["SizeID"].ToString() == row["SizeID"].ToString() &&
                                r["DauSizeID"].ToString() == row["DauSizeID"].ToString());
                        }
                        foreach (var gridRow in rowsToUpdate)
                        {
                            gridRow["TenQC"] = tenQuiCachGop;
                        }
                        foreach (DataRow item in tbl.Rows)
                        {
                           
                            DataRow newRow = saveTable.NewRow();
                            newRow["StyleID"] = searchLookUpEdit1.EditValue.ToString();
                            newRow["MaQC"] = item["MaQuiCach"].ToString();
                            newRow["Size"] = row["Size"].ToString();
                            newRow["SizeID"] = row["SizeID"].ToString();
                            newRow["DauSize"] = row["DauSize"].ToString();
                            newRow["DauSizeID"] = row["DauSizeID"].ToString();
                            newRow["SapXep"] = Convert.ToInt32(item["SapXep"]);
                            newRow["From"] = Convert.ToInt32(item["From"]);
                            newRow["To"] = Convert.ToInt32(item["To"]);
                            newRow["MaNoiDen"] = _maNoiDenChung;
                            saveTable.Rows.Add(newRow);
                        }
                    }
                    saveTable.AsEnumerable()
                       .Where(row => row.Field<string>("MaNoiDen") == "0")
                       .ToList()
                       .ForEach(row => row["MaNoiDen"] = _maNoiDenChung);
                    string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/PostQC");
                    string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, saveTable); }).Result;
                    if (json.ToLower() == "true")
                    {
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    dr["ChonQC"] = false;
                        //}
                        dgrTLKL.DataSource = dtGridTS;
                        //clsWaitForm.ShowSuccessForm(this, 2000);
                        if (cbxSizeQC != null)
                        {
                            cbxSizeQC.Checked = false;
                        }
                        foreach (DataRow row in dtGridTS.Rows)
                        {
                            if (row.Field<bool?>("ChonQC") == true)
                                row["ChonQC"] = false;
                        }
                        grvCaiDatTS.RefreshData();
                        LoadQuiCach();
                    }
                }
                else
                {
                    DataTable dtGridTS = dgrTLKL.DataSource as DataTable;
                    foreach (DataRow row in dt.Rows)
                    {
                        var gridRowToUpdate = dtGridTS.AsEnumerable()
                        .FirstOrDefault(r => r["SizeID"].ToString() == row["SizeID"].ToString());

                        if (gridRowToUpdate != null)
                        {
                            gridRowToUpdate["TenQC"] = "";
                            gridRowToUpdate["MaQCCheck"] = "";
                        }
                        DataRow newRow = saveTable.NewRow();
                        newRow["StyleID"] = searchLookUpEdit1.EditValue.ToString();
                        newRow["MaQC"] = "";
                        newRow["Size"] = row["Size"].ToString();
                        newRow["SizeID"] = row["SizeID"].ToString();
                        newRow["DauSize"] = row["DauSize"].ToString();
                        newRow["DauSizeID"] = row["DauSizeID"].ToString();
                        newRow["SapXep"] = 0;
                        newRow["From"] = 0;
                        newRow["To"] = 0;
                        newRow["MaNoiDen"] = _maNoiDenChung;
                        saveTable.Rows.Add(newRow);
                    }

                    string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/DeleteQC");
                    string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, saveTable); }).Result;
                    if (json.ToLower() == "true")
                    {
                        dgrTLKL.DataSource = dgrTLKL.DataSource as DataTable;
                        //clsWaitForm.ShowSuccessForm(this, 2000);
                        LoadQuiCach();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void btnSaveQC_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (LuuNoiDenChung())
            {
                SaveQC();
            }
        }

        private void textEdit4_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                DataTable tbl = dgrTLKL.DataSource as DataTable;
                if (tbl.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn mã hàng có dữ liệu", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }

                if (e.KeyChar.ToString() != null)
                {
                    string text1 = "";
                    string PCB = "";
                    double PCBInt = 0;
                    if (e.KeyChar.ToString() == "\b")
                    {
                        string textSubString = kltxtEdit.EditValue.ToString();
                        text1 = textSubString.Remove(textSubString.Length - 1);
                        PCB = text1 == "" ? "0" : text1;
                        PCBInt = Convert.ToInt32(PCB);
                    }
                    else
                    {
                        text1 = kltxtEdit.EditValue == null ? "" : kltxtEdit.EditValue.ToString();
                        PCB = text1 + e.KeyChar.ToString();
                        PCBInt = Convert.ToDouble(PCB);
                    }


                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        tbl.Rows[i]["SL_KhoiLuong"] = PCBInt;
                    }
                    dgrTLKL.DataSource = tbl;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void grvCaiDatTS_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            DataTable tbl = dgrTLKL.DataSource as DataTable;

            if (tbl != null && e.FocusedRowHandle >= 0)
            {
                _dausize = view.GetFocusedRowCellValue(colDauSize)?.ToString();
                _size = view.GetFocusedRowCellValue(colSize)?.ToString();
                _sizeid = view.GetFocusedRowCellValue(ColSizeID)?.ToString();
                LoadQuiCach();
            }
            if (e.FocusedRowHandle < 0) return;
        }
        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            var check = sender as CheckEdit;
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl is null) return;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                tbl.Rows[i]["ChonQC"] = check.Checked;
            }
            dgrTLKL.DataSource = tbl;
        }
        private void cbxQuiCach_CheckedChanged(object sender, EventArgs e)
        {
            var check = sender as CheckEdit;
            DataTable tbl = grcQuiCach.DataSource as DataTable;
            if (tbl is null) return;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                tbl.Rows[i]["Chon"] = check.Checked;
            }             
            grcQuiCach.DataSource = tbl;
        }

        #region quan
        private void grvCaiDatTS_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (grvCaiDatTS.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {
                        if (e.HitInfo.Column.FieldName.Contains("SL_KhoiLuong"))
                        {
                            // Add the "Copy" item to the context menu
                            DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", ItemCopy_Click);
                            e.Menu.Items.Add(menuCopyItem);

                            // Add the "Paste" item to the context menu
                            DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", ItemPaste_Click);
                            e.Menu.Items.Add(menuPasteItem);
                        }
                        //DevExpress.Utils.Menu.DXMenuItem menuCoppyPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", ItemCoppyPaste_Click);
                        //e.Menu.Items.Add(menuCoppyPasteItem);
                        //DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", ItemDelete_Click);
                        //e.Menu.Items.Add(menuDeleteItem);
                    }

                }
            }
        }

        private void ItemCopy_Click(object sender, EventArgs e)
        {
            GridView view = dgrTLKL.MainView as GridView;
            if (view != null)
            {
                view.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
                view.CopyToClipboard();
            }

        }

        private void Coppy()
        {
            GridView view = dgrTLKL.MainView as GridView;
            if (view != null)
            {
                view.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
                view.CopyToClipboard();
            }
        }

        private void Paste()
        {
            GridView view = dgrTLKL.MainView as GridView;
            string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length < 1) return;
            int startRow = view.FocusedRowHandle;
            foreach (string row in data)
            {
                AddRow(row, startRow++);
                if (!view.IsValidRowHandle(startRow)) break;
            }
        }

        private void ItemPaste_Click(object sender, EventArgs e)
        {
            GridView view = dgrTLKL.MainView as GridView;
            string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length < 1) return;
            int startRow = view.FocusedRowHandle;
            foreach (string row in data)
            {
                AddRow(row, startRow++);
                if (!view.IsValidRowHandle(startRow)) break;
            }
        }


        private string ClipboardData
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return "";

                if (iData.GetDataPresent(DataFormats.Text))
                    return (string)iData.GetData(DataFormats.Text);
                return "";
            }
            set
            {
                Clipboard.SetDataObject(value);
            }
        }

        DialogResult StatusMsg = DialogResult.None; int startRow = 0; int endRow = 0; bool copiedFromGrid = false;
        private void AddRow(string data, int rowHandle)
        {
            GridView view = dgrTLKL.MainView as GridView;
            if (data == string.Empty) return;
            string[] rowData = data.Split('\t');
            int column = view.FocusedColumn.VisibleIndex;
            view.SetRowCellValue(rowHandle, view.Columns["SL_KhoiLuong"], rowData[0]);
            //List<GridColumn> allSizeColumns = grvCaiDatTS.Columns
            //   .Where(x => x.FieldName.Contains("SL_KhoiLuong"))
            //   .OrderBy(x => x.VisibleIndex)
            //   .ToList();

            //GridColumn focusedColumn = grvCaiDatTS.FocusedColumn;
            //int startColIndex = allSizeColumns.FindIndex(col => col == focusedColumn);

            //if (startColIndex == -1) startColIndex = 0;

            //List<string> lstSizeCol = allSizeColumns
            //    .Skip(startColIndex)
            //    .Select(col => col.FieldName)
            //    .ToList();
            //StatusMsg = DialogResult.None;
            //int startRow = view.FocusedRowHandle;
            //int endRow = data.Length - 1;
            //rowHandle = startRow;

            //foreach (string row in rowData) 
            //{
            //    if (string.IsNullOrEmpty(row)) continue;
            //    if (!view.IsValidRowHandle(rowHandle)) break;


            //}
        }

        private void grvCaiDatTS_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            if (hitInfo.InRowCell)
            {
                if (hitInfo.Column.RealColumnEdit is DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit)
                {
                    view.FocusedRowHandle = hitInfo.RowHandle;
                    view.FocusedColumn = hitInfo.Column;
                    view.ShowEditor();
                    CheckEdit edit = view.ActiveEditor as CheckEdit;
                    if (edit != null)
                    {
                        edit.Toggle();
                        DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                    }
                }
            }
        }

        private void AutoTickCbxChonQC()
        {
            if (!barCheckLap.Checked || !barCheckLap2.Checked) return;
            GridView view = dgrTLKL.MainView as GridView;
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;
            if (view.FocusedRowHandle < 0) return;
            DataRow focusedRow = view.GetFocusedDataRow();
            if (focusedRow == null) return;
            int countTicked = 0;
            if (barCheckLap.Checked && !barCheckLap2.Checked)
            {
                string focusedSize = focusedRow["SizeID"].ToString();
                foreach(DataRow dr in tbl.Rows)
                {
                    if(dr["SizeID"].ToString() == focusedSize)
                    {
                        dr["ChonQC"] = true;
                        countTicked++;
                    }
                }
            }
            else if (!barCheckLap.Checked && barCheckLap2.Checked)
            {
                string focusedDauSize = focusedRow["DauSizeID"].ToString();
                foreach (DataRow dr in tbl.Rows)
                {
                    if (dr["DauSizeID"].ToString() == focusedDauSize)
                    {
                        dr["ChonQC"] = true;
                        countTicked++;
                    }
                }
            }
            view.BeginUpdate();
            view.LayoutChanged();
            view.EndUpdate();
        }

        private void grvCaiDatTS_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
        }

        private void grvCaiDatTS_CellMerge(object sender, CellMergeEventArgs e)
        {

        }

        private void grvCaiDatTS_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName != "ChonQC") return;
            GridView view = sender as GridView;
            if (view == null) return;
            if (!barCheckLap.Checked && !barCheckLap2.Checked) return;
            view.PostEditor();
            view.UpdateCurrentRow();
            bool isChecked = Convert.ToBoolean(e.Value);
            view.BeginUpdate();
            try
            {
                DataRow changedRow = view.GetDataRow(e.RowHandle);
                if (changedRow == null) return;
                if (barCheckLap.Checked)
                {
                    string sizeID = changedRow["SizeID"]?.ToString();
                    for (int i = 0; i < view.DataRowCount; i++)
                    {
                        DataRow dr = view.GetDataRow(i);
                        if (dr == null) continue;
                        if (dr["SizeID"]?.ToString() == sizeID)
                        {
                            view.SetRowCellValue(i, "ChonQC", isChecked);
                        }
                    }
                }
                else if (barCheckLap2.Checked)
                {
                    string dauSizeID = changedRow["DauSizeID"]?.ToString();
                    for (int i = 0; i < view.DataRowCount; i++)
                    {
                        DataRow dr = view.GetDataRow(i);
                        if (dr == null) continue;
                        if (dr["DauSizeID"]?.ToString() == dauSizeID)
                        {
                            view.SetRowCellValue(i, "ChonQC", isChecked);
                        }
                    }
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }
        private void btnCopyMH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEdit1.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentStyleID = searchLookUpEdit1.EditValue.ToString();
            string currentMaHang = searchLookUpEdit1.Text;

            using (var frm = new frmSaveTL_KL_Copy(currentStyleID, currentMaHang, "", "", "", "0"))
            {
                if (frm.ShowDialog(this) == DialogResult.OK && frm.CopiedDataTable != null)
                {
                    DataTable dtCopied = frm.CopiedDataTable;

                    if (dtCopied != null && dtCopied.Rows.Count > 0)
                    {
                        DataTable dtForGrid = dtCopied.Copy();

                        foreach (DataRow dr in dtForGrid.Rows)
                        {
                            dr["StyleID"] = currentStyleID;
                            dr["MaHang"] = currentMaHang;
                        }

                        dgrTLKL.DataSource = dtForGrid;
                        Load_NoiDen_TheoMaHang(false);

                        grvCaiDatTS.RefreshData();

                        if (!string.IsNullOrEmpty(frm.GhiChuCopied))
                            txtEdtGhiChu.EditValue = frm.GhiChuCopied;

                        SaveCopiedData(dtCopied, dtForGrid, currentStyleID, currentMaHang, frm);
                    }
                }
            }
        }

        private void SaveCopiedData(DataTable dtCopied, DataTable dtForGrid, string currentStyleID, string currentMaHang, frmSaveTL_KL_Copy frm)
        {
            try
            {
                if (dtForGrid == null || dtForGrid.Rows.Count == 0)
                    return;
                if (dtCopied == null)
                {
                    dtCopied = new DataTable();
                }
                Dictionary<string, string> maNoiDenMapping = new Dictionary<string, string>();

                List<string> sourceNoiDenList = new List<string>();

                if (dtCopied != null && dtCopied.Rows.Count > 0 && dtCopied.Columns.Contains("MaNoiDen"))
                {
                    sourceNoiDenList = dtCopied.AsEnumerable()
                        .Select(r => r["MaNoiDen"]?.ToString()?.Trim() ?? "")
                        .Where(m => !string.IsNullOrEmpty(m) && m != "0")
                        .Distinct()
                        .ToList();
                }

                if (sourceNoiDenList.Count > 0)
                {
                    string sourceStyleID = dtCopied.Rows[0]["StyleID"]?.ToString() ?? "";

                    var sourceNames = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(sourceStyleID))
                    {
                        string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetNoiDen&Para1={sourceStyleID}");
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                        if (!string.IsNullOrEmpty(json) && json != "[]")
                        {
                            var dtSource = JsonConvert.DeserializeObject<DataTable>(json);
                            sourceNames = dtSource.AsEnumerable()
                                .ToDictionary(r => r["MaNoiDen"]?.ToString()?.Trim() ?? "",
                                              r => r["NoiDen"]?.ToString()?.Trim() ?? "");
                        }
                    }

                    DataTable dtCurrent = LoadNoiDen(false);
                    if (dtCurrent == null) dtCurrent = new DataTable();
                    var dtNewNoiDen = new DataTable();
                    dtNewNoiDen.Columns.Add("MaHang", typeof(string));
                    dtNewNoiDen.Columns.Add("MaNoiDen", typeof(string));
                    dtNewNoiDen.Columns.Add("NoiDen", typeof(string));

                    foreach (string oldMa in sourceNoiDenList)
                    {
                        string ten = sourceNames.TryGetValue(oldMa, out var n) ? n : oldMa;
                        bool exists = dtCurrent.AsEnumerable().Any(r =>
                            r.Field<string>("StyleID") == currentStyleID &&
                            string.Equals(r.Field<string>("NoiDen")?.Trim(), ten, StringComparison.OrdinalIgnoreCase));

                        if (!exists)
                        {
                            dtNewNoiDen.Rows.Add(currentStyleID, "", ten);
                        }
                    }

                    if (dtNewNoiDen.Rows.Count > 0)
                    {
                        string urlND = string.Format("{0}", URL + $"CaiDatThongSoKHDT/PostNoiDen");
                        string jsonND = Task.Run(async () => { return await _clientExtension.PostAsync(urlND, dtNewNoiDen); }).Result;
                        if (jsonND?.ToLower() != "true") return;
                        dtCurrent = LoadNoiDen(false);
                    }

                    foreach (string oldMa in sourceNoiDenList)
                    {
                        string ten = sourceNames.TryGetValue(oldMa, out var n) ? n : oldMa;
                        var row = dtCurrent.AsEnumerable()
                            .FirstOrDefault(r => r.Field<string>("StyleID") == currentStyleID &&
                                                 string.Equals(r.Field<string>("NoiDen")?.Trim(), ten, StringComparison.OrdinalIgnoreCase));
                        maNoiDenMapping[oldMa] = row?["MaNoiDen"]?.ToString()?.Trim() ?? oldMa;
                    }
                }

                foreach (DataRow row in dtForGrid.Rows)
                {
                    string old = row["MaNoiDen"]?.ToString()?.Trim() ?? "0";
                    if (maNoiDenMapping.TryGetValue(old, out var newMa))
                        row["MaNoiDen"] = newMa;
                }
                if (dtForGrid.Columns.Contains("checkQuiCach"))
                {
                    dtForGrid.Columns.Remove("checkQuiCach");
                }
                if (dtForGrid.Columns.Contains("TenQC"))
                {
                    dtForGrid.Columns.Remove("TenQC");
                }
                string urlTS = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Post?action=PostTL_KL");
                string jsonTS = Task.Run(async () => { return await _clientExtension.PostAsync(urlTS, dtForGrid); }).Result;
                if (jsonTS?.ToLower() != "true") return;

                DataTable dtCopiedQC = frm.CopiedQuiCachTable;
                if (dtCopiedQC != null && dtCopiedQC.Rows.Count > 0)
                {
                    var qcToSave = dtCopiedQC.AsEnumerable()
                        .Where(r => r.Field<bool?>("Chon") == true)
                        .ToList();

                    if (qcToSave.Any())
                    {
                        var dtSaveQC = new DataTable();
                        dtSaveQC.Columns.Add("StyleID", typeof(string));
                        dtSaveQC.Columns.Add("MaQC", typeof(string));
                        dtSaveQC.Columns.Add("Size", typeof(string));
                        dtSaveQC.Columns.Add("SizeID", typeof(string));
                        dtSaveQC.Columns.Add("DauSize", typeof(string));
                        dtSaveQC.Columns.Add("DauSizeID", typeof(string));
                        dtSaveQC.Columns.Add("SapXep", typeof(int));
                        dtSaveQC.Columns.Add("From", typeof(int));
                        dtSaveQC.Columns.Add("To", typeof(int));
                        dtSaveQC.Columns.Add("MaNoiDen", typeof(string));
                        dtSaveQC.Columns.Add("NguoiTao", typeof(string));
                        var uniqueKeys = new HashSet<string>();

                        foreach (DataRow qcRow in qcToSave)
                        {
                            string oldMaNoiDen = qcRow["MaNoiDen"]?.ToString()?.Trim() ?? "0";
                            string newMaNoiDen = maNoiDenMapping.TryGetValue(oldMaNoiDen, out var mapped) ? mapped : oldMaNoiDen;

                            string maQC = qcRow["MaQC"]?.ToString() ?? "";
                            string sizeID = qcRow["SizeID"]?.ToString() ?? "";
                            string dauSizeID = qcRow["DauSizeID"]?.ToString() ?? "";

                            string key = $"{maQC}|{sizeID}|{dauSizeID}|{newMaNoiDen}";

                            if (uniqueKeys.Add(key))
                            {
                                dtSaveQC.Rows.Add(
                                    currentStyleID,
                                    maQC,
                                    qcRow["Size"]?.ToString() ?? "",
                                    sizeID,
                                    qcRow["DauSize"]?.ToString() ?? "",
                                    dauSizeID,
                                    qcRow["SapXep"] != DBNull.Value ? Convert.ToInt32(qcRow["SapXep"]) : 0,
                                    qcRow["From"] != DBNull.Value ? Convert.ToInt32(qcRow["From"]) : 0,
                                    qcRow["To"] != DBNull.Value ? Convert.ToInt32(qcRow["To"]) : 0,
                                    newMaNoiDen,
                                    GlobleData.UserName
                                );
                            }
                        }

                        if (dtSaveQC.Rows.Count > 0)
                        {
                            string urlQC = string.Format("{0}", URL + $"CaiDatThongSoKHDT/PostQC");
                            string jsonQC = Task.Run(async () => { return await _clientExtension.PostAsync(urlQC, dtSaveQC); }).Result;
                            if (jsonQC?.ToLower() != "true") return;
                        }
                    }
                }

                clsWaitForm.ShowSuccessForm(this, 2000);
                load_GridView();
                LoadQuiCach();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu dữ liệu thất bại: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveAll();
        }
        private void SaveAll()
        {
            try
            {
                string maHang = searchLookUpEdit1.EditValue != null ? searchLookUpEdit1.Text.ToString() : "";
                if (string.IsNullOrEmpty(maHang))
                {
                    XtraMessageBox.Show("Vui lòng chọn Mã hàng!",
                       "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string noidentxt = searchLookUpEdit2.EditValue != null ? searchLookUpEdit2.Text.ToString() : "";
                if (string.IsNullOrEmpty(noidentxt))
                {
                    XtraMessageBox.Show("Vui lòng chọn nơi đến!",
                       "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Đảm bảo có nơi đến "Chung"
                if (!LuuNoiDenChung())
                {
                    return;
                }

                bool hasQCData = false;
                bool hasTLKLData = false;
                DataTable dtTLKL = dgrTLKL.DataSource as DataTable;
                if (dtTLKL != null && dtTLKL.Rows.Count > 0)
                {
                    var tblTempQC = dtTLKL.AsEnumerable().Where(x => Convert.ToBoolean(x["ChonQC"]) == true);
                    hasQCData = tblTempQC.Any();
                }
                if (dtTLKL != null && dtTLKL.Rows.Count > 0)
                {
                    var tblTempTLKL = dtTLKL.AsEnumerable().Where(x => Convert.ToBoolean(x["Chon"]) == true);
                    hasTLKLData = tblTempTLKL.Any();
                }

                if (!hasQCData && !hasTLKLData)
                {
                    XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng dữ liệu để lưu!",
                       "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool saveQCSuccess = true;
                bool saveTLKLSuccess = true;
                if (hasQCData)
                {
                    try
                    {
                        this.ActiveControl = button1;
                        DataTable dtNoiDen = LoadNoiDen(false);
                        var ketQua = dtNoiDen.AsEnumerable()
                              .Where(row => row.Field<string>("NoiDen") == "Chung")
                              .FirstOrDefault();
                        string _maNoiDenChung = "";
                        if (ketQua != null && noidentxt.ToUpper() == "CHUNG")
                        {
                            _maNoiDenChung = ketQua["MaNoiDen"].ToString();
                        }
                        else
                        {
                            _maNoiDenChung = searchLookUpEdit2.EditValue?.ToString() ?? "0";
                        }

                        saveTable.Clear();
                        DataTable dt = dgrTLKL.DataSource as DataTable;
                        foreach (DataRow row in dt.Rows)
                            row["MaNoiDen"] = _maNoiDenChung;

                        DataTable tbl = grcQuiCach.DataSource as DataTable;
                        if (tbl != null && tbl.Rows.Count > 0 && dt != null && dt.Rows.Count > 0)
                        {
                            var tblTemp = tbl.AsEnumerable().Where(x => Convert.ToBoolean(x["Chon"]) == true);
                            var tblTemp1 = dt.AsEnumerable().Where(x => Convert.ToBoolean(x["ChonQC"]) == true);

                            if (tblTemp1.Count() == 0)
                            {
                                saveQCSuccess = true;
                            }
                            else
                            {
                                dt = tblTemp1.CopyToDataTable();
                                string _maNoiDen = searchLookUpEdit2.EditValue?.ToString() ?? "0";

                                if (!dt.Columns.Contains("TenQC"))
                                {
                                    dt.Columns.Add("TenQC", typeof(string));
                                }
                                //if (!saveTable.Columns.Contains("MaNoiDen"))
                                //{
                                //    saveTable.Columns.Add("MaNoiDen", typeof(string));
                                //}

                                if (tblTemp.Count() > 0)
                                {
                                    // Có qui cách được chọn -> Lưu qui cách
                                    tbl = tblTemp.CopyToDataTable();
                                    string tenQuiCachGop = string.Join(",",
                                        tbl.AsEnumerable()
                                           .Select(x => x["TenQuiCach"] != null ? x["TenQuiCach"].ToString().Trim() : "")
                                           .Where(x => !string.IsNullOrEmpty(x))
                                           .ToList());

                                    DataTable dtGridTS = dgrTLKL.DataSource as DataTable;

                                    foreach (DataRow row in dt.Rows)
                                    {
                                        var rowsToUpdate = dtGridTS.AsEnumerable();

                                        if (barCheckLap2.Checked)
                                        {
                                            rowsToUpdate = rowsToUpdate.Where(r => r["DauSizeID"].ToString() == row["DauSizeID"].ToString());
                                        }
                                        else if (barCheckLap.Checked)
                                        {
                                            rowsToUpdate = rowsToUpdate.Where(r => r["SizeID"].ToString() == row["SizeID"].ToString());
                                        }
                                        else
                                        {
                                            rowsToUpdate = rowsToUpdate.Where(r =>
                                                r["SizeID"].ToString() == row["SizeID"].ToString() &&
                                                r["DauSizeID"].ToString() == row["DauSizeID"].ToString());
                                        }

                                        foreach (var gridRow in rowsToUpdate)
                                        {
                                            gridRow["TenQC"] = tenQuiCachGop;
                                        }

                                        foreach (DataRow item in tbl.Rows)
                                        {
                                            DataRow newRow = saveTable.NewRow();
                                            newRow["StyleID"] = searchLookUpEdit1.EditValue.ToString();
                                            newRow["MaQC"] = item["MaQuiCach"].ToString();
                                            newRow["Size"] = row["Size"].ToString();
                                            newRow["SizeID"] = row["SizeID"].ToString();
                                            newRow["DauSize"] = row["DauSize"].ToString();
                                            newRow["DauSizeID"] = row["DauSizeID"].ToString();
                                            newRow["SapXep"] = Convert.ToInt32(item["SapXep"]);
                                            newRow["From"] = Convert.ToInt32(item["From"]);
                                            newRow["To"] = Convert.ToInt32(item["To"]);
                                            newRow["MaNoiDen"] = _maNoiDenChung;
                                            newRow["NguoiTao"] = GlobleData.UserName;
                                            saveTable.Rows.Add(newRow);
                                        }
                                    }

                                    saveTable.AsEnumerable()
                                       .Where(row => row.Field<string>("MaNoiDen") == "0")
                                       .ToList()
                                       .ForEach(row => row["MaNoiDen"] = _maNoiDenChung);

                                    string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/PostQC");
                                    string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, saveTable); }).Result;

                                    if (json.ToLower() == "true")
                                    {
                                        dgrTLKL.DataSource = dtGridTS;
                                        if (cbxSizeQC != null)
                                        {
                                            cbxSizeQC.Checked = false;
                                        }
                                        foreach (DataRow row in dtGridTS.Rows)
                                        {
                                            if (row.Field<bool?>("ChonQC") == true)
                                                row["ChonQC"] = false;
                                        }
                                        grvCaiDatTS.RefreshData();
                                        saveQCSuccess = true;
                                    }
                                    else
                                    {
                                        saveQCSuccess = false;
                                    }
                                }
                                else
                                {
                                    DataTable dtGridTS = dgrTLKL.DataSource as DataTable;

                                    foreach (DataRow row in dt.Rows)
                                    {
                                        var gridRowToUpdate = dtGridTS.AsEnumerable()
                                            .FirstOrDefault(r => r["SizeID"].ToString() == row["SizeID"].ToString());

                                        if (gridRowToUpdate != null)
                                        {
                                            gridRowToUpdate["TenQC"] = "";
                                            gridRowToUpdate["MaQCCheck"] = "";
                                        }

                                        DataRow newRow = saveTable.NewRow();
                                        newRow["StyleID"] = searchLookUpEdit1.EditValue.ToString();
                                        newRow["MaQC"] = "";
                                        newRow["Size"] = row["Size"].ToString();
                                        newRow["SizeID"] = row["SizeID"].ToString();
                                        newRow["DauSize"] = row["DauSize"].ToString();
                                        newRow["DauSizeID"] = row["DauSizeID"].ToString();
                                        newRow["SapXep"] = 0;
                                        newRow["From"] = 0;
                                        newRow["To"] = 0;
                                        newRow["MaNoiDen"] = _maNoiDenChung;
                                        newRow["NguoiTao"] = GlobleData.UserName;
                                        saveTable.Rows.Add(newRow);
                                    }

                                    string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/DeleteQC");
                                    string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, saveTable); }).Result;

                                    if (json.ToLower() == "true")
                                    {
                                        dgrTLKL.DataSource = dtGridTS;
                                        saveQCSuccess = true;
                                    }
                                    else
                                    {
                                        saveQCSuccess = false;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        saveQCSuccess = false;
                    }
                }
                if (hasTLKLData && saveQCSuccess)
                {
                    try
                    {
                        this.ActiveControl = button1;
                        DataTable dtNoiDen = LoadNoiDen(false);
                        if (dtNoiDen == null || dtNoiDen.Rows.Count == 0)
                        {
                            saveTLKLSuccess = false;
                        }
                        else
                        {
                            var ketQua = dtNoiDen.AsEnumerable()
                                  .Where(row => row.Field<string>("NoiDen") == "Chung")
                                  .FirstOrDefault();
                            string _maNoiDenChung = "";
                            if (ketQua != null && noidentxt.ToUpper() == "CHUNG")
                            {
                                _maNoiDenChung = ketQua["MaNoiDen"].ToString();
                            }
                            else
                            {
                                _maNoiDenChung = searchLookUpEdit2.EditValue?.ToString() ?? "0";
                            }

                            DataTable dt = dgrTLKL.DataSource as DataTable;
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                if (dt.Columns.Contains("checkQuiCach"))
                                {
                                    dt.Columns.Remove("checkQuiCach");
                                }
                                if (dt.Columns.Contains("TenQC"))
                                {
                                    dt.Columns.Remove("TenQC");
                                }

                                var tblTemp = dt.AsEnumerable().Where(x => Convert.ToBoolean(x["Chon"]) == true);
                                if (tblTemp.Any())
                                {
                                    DataTable dtSave = tblTemp.CopyToDataTable();
                                    foreach (DataRow row in dtSave.Rows)
                                    {
                                        row["MaNoiDen"] = _maNoiDenChung;
                                        row["GhiChu"] = txtEdtGhiChu.EditValue?.ToString() ?? "";
                                    }

                                    foreach (DataRow data in dtSave.Rows)
                                    {
                                        if (data["MaNoiDen"].ToString() == "0")
                                        {
                                            data["MaNoiDen"] = _maNoiDenChung;
                                        }
                                    }

                                    if (dtSave.Columns.Contains("ChonQC1"))
                                    {
                                        dtSave.Columns.Remove("ChonQC1");
                                    }

                                    string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Post?action=PostTL_KL");
                                    string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;

                                    if (json.ToLower() == "true")
                                    {
                                        pcbtxtEdit.EditValue = "0";
                                        pptxtEdit.EditValue = "0";
                                        packctntxtEdit.EditValue = "0";
                                        kltxtEdit.EditValue = "0";
                                        saveTLKLSuccess = true;
                                    }
                                    else
                                    {
                                        saveTLKLSuccess = false;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("Lưu đã xảy ra lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        saveTLKLSuccess = false;
                    }
                }
                if (saveQCSuccess && saveTLKLSuccess)
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    load_GridView();
                    LoadQuiCach();
                }
                else
                {
                    string errorMsg = "Lưu thất bại:\n";
                    if (!saveQCSuccess) errorMsg += "Lưu qui cách thất bại\n";
                    if (!saveTLKLSuccess) errorMsg += "Lưu cài đặt thông số thất bại";
                    XtraMessageBox.Show(errorMsg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    load_GridView();
                    LoadQuiCach();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi: " + ex.Message,
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTQCartonList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDicQCDongThungTongQuan frm = new frmDicQCDongThungTongQuan();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }

        private void grvQuiCach_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "TenQuiCach")
            {
                if (e.Value != null && e.Value is string str && !string.IsNullOrWhiteSpace(str))
                {
                    e.DisplayText = str.Replace("x", "*");
                }
            }
            if (e.Column.FieldName == "KyHieu")
            {
                if (e.Value != null && e.Value is string str && !string.IsNullOrWhiteSpace(str))
                {
                    e.DisplayText = str.Replace("x", "*");
                }
            }
        }

        private void frmSaveTL_KL_Load(object sender, EventArgs e)
        {

        }

        private void grvCaiDatTS_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "TenQC")
            {
                if (e.Value != null && e.Value is string str && !string.IsNullOrWhiteSpace(str))
                {
                    e.DisplayText = str.Replace("x", "*");
                }
            }
        }

        private void grvCaiDatTS_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    if (e.Control)
                    {
                        // Tổ hợp phím Ctrl + C
                        Coppy();
                    }
                    break;
                case Keys.V:
                    if (e.Control)
                    {
                        // Tổ hợp phím Ctrl + V
                        Paste();
                    }
                    break;
            }
        }
        #endregion
        //private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //   
        //}

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            _styleID = searchLookUpEdit1.EditValue.ToString();
        }

        private void cbxCheckOrUnCheck_CheckedChanged(object sender, EventArgs e)
        {
            var check = sender as CheckEdit;
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl.Rows.Count == 0 || tbl == null) return;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                tbl.Rows[i]["Chon"] = check.Checked;
            }
            dgrTLKL.DataSource = tbl;
        }

        private void grvCaiDatTS_RowStyle(object sender, RowStyleEventArgs e)
        {
            //if (Convert.ToInt32(grvCaiDatTS.GetRowCellValue(e.RowHandle, colCheckQuiCach)) == 1)
            //{
            //    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");
            //}
            //else
            //{
            //    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff");
            //}
            if (e.RowHandle == grvCaiDatTS.FocusedRowHandle)
            {
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff99");
            }
            //else
            //{
            //    if (Convert.ToInt32(grvCaiDatTS.GetRowCellValue(e.RowHandle,colCheckQuiCach)) == 1)
            //    {
            //        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");
            //    }
            //    else
            //    {
            //        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#fff");
            //    }
            //}
        }

        private void load_GridView()
        {
            try
            {
                if (searchLookUpEdit1.EditValue is null) return;
                _maHang = searchLookUpEdit1.EditValue.ToString();
                _maNoiDen = searchLookUpEdit2.EditValue?.ToString();
                string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetTLKL&Para1={_maHang}&Para2={_dausize}&Para3={_size}&Para4={_sizeid}&Para5={_maNoiDen}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0) return;
                txtEdtGhiChu.EditValue = tbl.Rows[0]["GhiChu"];
                if (!tbl.Columns.Contains("TenQC"))
                {
                    tbl.Columns.Add("TenQC", typeof(string));
                    foreach (DataRow r in tbl.Rows)
                        r["TenQC"] = "";
                }
                foreach (DataRow row in tbl.Rows)
                {
                    row["MaNoiDen"] = _maNoiDen;
                    row["ChonQC"] = false;
                    //if (cbxSizeQC != null)
                    //{
                    //    cbxSizeQC.Checked = false;
                    //}
                }

                dgrTLKL.DataSource = tbl;
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow dr = tbl.Rows[0];
                    if (grvCaiDatTS.FocusedRowHandle >= 0)
                    {
                        dr = grvCaiDatTS.GetFocusedDataRow();
                        if (dr == null) dr = tbl.Rows[0];
                    }
                }
                var rowFocus = grvCaiDatTS.FocusedRowHandle;
                grvCaiDatTS.FocusedRowHandle = rowFocus;
            }
            catch
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadQuiCach()
        {
            try
            {
                
                _dausize = grvCaiDatTS.GetFocusedRowCellValue(colDauSize)?.ToString();
                _size = grvCaiDatTS.GetFocusedRowCellValue(colSize)?.ToString();
                _sizeid = grvCaiDatTS.GetFocusedRowCellValue(ColSizeID)?.ToString();
                _maHang = searchLookUpEdit1.EditValue.ToString();
                _maNoiDen = searchLookUpEdit2.EditValue?.ToString() ?? "0";

                string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetQuiCach&Para1={_maHang}&Para2={_dausize}&Para3={_size}&Para4={_sizeid}&Para5={_maNoiDen}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);         
                //if (!tbl.Columns.Contains("MaNoiDen"))
                //{
                //    tbl.Columns.Add("MaNoiDen", typeof(string));
                //    foreach (DataRow row in tbl.Rows)
                //    {
                //        row["MaNoiDen"] = _maNoiDen;
                //    }
                //}
                grcQuiCach.DataSource = tbl;
            }
            catch
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void dgvTLKL_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (e.Value != null)
            {
                float result;
                if (!float.TryParse(e.Value.ToString(), out result))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng không nhập kí tự!";
                }
            }
        }
    }
}