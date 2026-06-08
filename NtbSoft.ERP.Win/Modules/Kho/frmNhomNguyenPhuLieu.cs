using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.ThuVien;
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
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmNhomNguyenPhuLieu : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        DataTable tblNhom;
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
        Bitmap dragBitmap = null; // Bitmap để lưu hình ảnh của dòng
        private int draggedRowHandle1 = GridControl.InvalidRowHandle; // Khai báo biến cấp lớp
        bool isDragging = false;
        bool isEditing = false;
        string nhommoi = string.Empty;
        public frmNhomNguyenPhuLieu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            tblNhom = new DataTable();
            GridColumn column = gridNhomNPL.Columns["TenNhom"];
            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            gridControl1.RepositoryItems.Add(textEdit);
            column.ColumnEdit = textEdit;

            // Đăng ký sự kiện KeyPress
            textEdit.KeyPress += gridNhomNPL_KeyPress;
        }
        public frmNhomNguyenPhuLieu(string nhom)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            tblNhom = new DataTable();
            GridColumn column = gridNhomNPL.Columns["TenNhom"];
            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            gridControl1.RepositoryItems.Add(textEdit);
            column.ColumnEdit = textEdit;

            // Đăng ký sự kiện KeyPress
            textEdit.KeyPress += gridNhomNPL_KeyPress;
            this.nhommoi = nhom;
        }
        protected override void OnLoad(EventArgs e)
        {
            gridControl1.AllowDrop = true;
            CreateSearchLookup();
            CreateTableNhom();
            CreateComboBoxLoaiVT();
            LoadNhomNPL(false);
            if (string.IsNullOrEmpty(nhommoi)) return;
            string[] lstnhommoi = nhommoi.ToUpper().Trim().Split('|');
            foreach (string tennhom in lstnhommoi)
            {

                DataRow row = tblNhom.NewRow();
                int maxSort = tblNhom.AsEnumerable()
                         .Where(r => r["Sort"] != DBNull.Value)
                         .Select(r => Convert.ToInt32(r["Sort"]))
                         .DefaultIfEmpty(0)
                         .Max();
                row["ID"] = "0";
                row["LoaiVT"] = "Nguyên liệu";
                row["Sort"] = maxSort + 1;
                row["TenNhom"] = tennhom;
                tblNhom.Rows.InsertAt(row, 0);
                gridNhomNPL.FocusedRowHandle = 0;


                _status = ResourceURL.EventStatus.Add;
                GridViewUpdateStatus(_status);

            }
            Xoa.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.NapLai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            gridNhomNPL.OptionsBehavior.Editable = true;
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                   
                    Them.Enabled = true;
                    Sua.Enabled = true;
                    Xoa.Enabled = true;
                    Luu.Enabled = true;
                    //if (_allowAdd)
                    //{
                    //    Them.Enabled = true;
                    //    actionControlAdd.Enabled = true;
                    //}

                    //if (_allowEdit)
                    //{
                    //    Sua.Enabled = true;
                    //    actionControlEdit.Enabled = true;
                    //}

                    //if (_allowDelete)
                    //{
                    //    Xoa.Enabled = true;
                    //    actionControlDelete.Enabled = true;
                    //}

                    //if (_allowAdd || _allowEdit)
                    //{
                    //    Luu.Enabled = false;
                    //    actionControlSave.Enabled = false;
                    //}
                    break;
                case ResourceURL.EventStatus.Edit:
                    //gridNhomNPL.OptionsBehavior.Editable = true;
                    Them.Enabled = false;
                    Sua.Enabled = false;
                    Xoa.Enabled = false;
                    Luu.Enabled = true;
                    //if (_allowAdd)
                    //{
                    //    Them.Enabled = false;
                    //    actionControlAdd.Enabled = false;
                    //}
                    //if (_allowEdit)
                    //{
                    //    Sua.Enabled = false;
                    //    actionControlEdit.Enabled = false;
                    //}

                    //if (_allowDelete)
                    //{
                    //    Xoa.Enabled = false;
                    //    actionControlDelete.Enabled = false;
                    //}

                    //if (_allowAdd || _allowEdit)
                    //{
                    //    Luu.Enabled = true;
                    //    actionControlSave.Enabled = true;
                    //}
                    break;
                case ResourceURL.EventStatus.Add:
                    //gridNhomNPL.OptionsBehavior.Editable = true;
                    Them.Enabled = false;
                    Sua.Enabled = false;
                    Xoa.Enabled = false;
                    Luu.Enabled = true;
                    //if (_allowAdd)
                    //{
                    //    Them.Enabled = false;
                    //    actionControlAdd.Enabled = false;
                    //}

                    //if (_allowEdit)
                    //{
                    //    Sua.Enabled = false;
                    //    actionControlEdit.Enabled = false;
                    //}
                    //if (_allowDelete)
                    //{
                    //    Xoa.Enabled = false;
                    //    actionControlDelete.Enabled = false;
                    //}
                    //Sua.Enabled = false;
                    //if (_allowAdd || _allowEdit)
                    //{
                    //    Luu.Enabled = true;
                    //    actionControlSave.Enabled = true;
                    //}
                    break;
            }
        }
        private void CreateTableNhom()
        {
            tblNhom = new DataTable("tblNhom");
            tblNhom.Columns.Add("ID", typeof(int));
            tblNhom.Columns.Add("MaNhom", typeof(string));
            tblNhom.Columns.Add("TenNhom", typeof(string));
            tblNhom.Columns.Add("NPL", typeof(bool));
            tblNhom.Columns.Add("LoaiVT", typeof(string));
            tblNhom.Columns.Add("TenTA", typeof(string));
            tblNhom.Columns.Add("Sort", typeof(int));
            tblNhom.Columns.Add("VietTat", typeof(string));
            tblNhom.Columns.Add("MaCLVT", typeof(string));
        }
        private void CreateSearchLookup()
        {
            string url = string.Format("{0}?", URL + "ChungLoaiVatTu/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "ChungLoaiVatTu";
            rCountryEdit.ValueMember = "MaCLVT";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn]";
            rCountryEdit.Popup += RCountryEdit_Popup;
            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaCLVT", Caption = "Mã CLVT", Name = "colMaCLVT", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "ChungLoaiVatTu", Caption = "Chủng loại vật tư", Name = "colChungLoaiVatTu", Visible = true });

            }

            gridColumn1.ColumnEdit = rCountryEdit;
        }
        private void RCountryEdit_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnOK"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnOK", Text = "Khai báo chủng loại vật tư" };
                khaiBaoButton.Click += searchOKPLButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }
        private void searchOKPLButton_Click(object sender, EventArgs e)
        {
            frmChungLoaiVatTu frm = new frmChungLoaiVatTu();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            CreateSearchLookup();
        }
        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
        private void ThemDong()
        {
            
            textTN.Text = "";
            txtNPL.Text = "";
            txtTenTA.Text = "";
            DataRow row = tblNhom.NewRow();
            int maxSort = tblNhom.AsEnumerable()
                     .Where(r => r["Sort"] != DBNull.Value)
                     .Select(r => Convert.ToInt32(r["Sort"]))
                     .DefaultIfEmpty(0)
                     .Max();
            row["ID"] = "0";
            row["LoaiVT"] = "Nguyên liệu";
            row["Sort"] = maxSort + 1;
            //_bindingHangHoaEntity.Add(obj);
            tblNhom.Rows.InsertAt(row, 0);
            gridNhomNPL.FocusedRowHandle = 0;
            // gridControl1.RefreshDataSource();
            //  gridNhomNPL.MakeRowVisible(0, true);

            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);

        }

        private void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
            Them.Enabled = false;
            Sua.Enabled = false;
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                if (tblNhom.Rows.Count == 0 || tblNhom == null) return;
                string dup = CheckDuplicateTenNhom_Loop(tblNhom);
                if (dup != null)
                {
                    XtraMessageBox.Show($"Trùng tên nhóm: {dup} . Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tblNhom.Rows.Count == 0 || tblNhom == null) return;
                string sort = CheckDuplicateSort_Loop(tblNhom);
                if (sort != null)
                {
                    XtraMessageBox.Show($"Trùng sắp xếp: {sort} . Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var rowsWithIssue = tblNhom.AsEnumerable()
                                      .Where(row => row.IsNull("MaCLVT") || row["MaCLVT"].ToString() == "")
                                      .ToList();

                if (rowsWithIssue.Any())
                {
                    MessageBox.Show("Chủng loại vật tư không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tblNhom.Columns.Contains("LoaiVT"))
                {
                    foreach (DataRow row in tblNhom.Rows)
                    {
                        if(string.IsNullOrEmpty(row["TenNhom"].ToString().Replace(" ", "")))
                        {

                            MessageBox.Show("Tên nhóm vật tư không được bỏ trống. Vui lòng điền đầy đủ thông tin!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        //if (string.IsNullOrEmpty(row["VietTat"].ToString().Replace(" ", "")))
                        //{

                        //    MessageBox.Show("Viết tắt không được bỏ trống. Vui lòng điền đầy đủ thông tin!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}
                        string manhom = RemoveVietnameseTone(ReplaceSpecialCharacters(row["TenNhom"].ToString().Replace(" ", "")));
                        row["TenNhom"] = row["TenNhom"].ToString().Trim();
                        row["MaNhom"] = manhom.ToString().ToUpper().Trim();
                        row["NPL"] = row["LoaiVT"].ToString() == "Nguyên liệu" ? true : false;
                    }
                    tblNhom.Columns.Remove("LoaiVT");
                }
                var rowsToDelete = tblNhom.AsEnumerable()
                          .Where(row => string.IsNullOrEmpty(row.Field<string>("TenNhom")))
                          .ToList();

                foreach (var row in rowsToDelete)
                {
                    tblNhom.Rows.Remove(row);
                }
               
                string msResult = "";
                if(!tblNhom.Columns.Contains("VietTat"))
                {
                    tblNhom.Columns.Add("VietTat", typeof(string));
                }    
                string url = URL + "NhomNPL/Post";
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhom); }).Result;
                if (tblNhom.Columns.Contains("VietTat"))
                {
                    tblNhom.Columns.Remove("VietTat");
                }
                if (msResult.ToLower() == "true")
                {
                    LoadNhomNPL(false);
                    //foreach (GridColumn col in gridNhomNPL.Columns)
                    //{
                    //    col.OptionsColumn.AllowEdit = false;
                    //}
                    
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {

                
            }

        }
        private void barButtonItem25_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
            //Luu.Enabled = false;
        }
        private async void XoaDong()
        {
            try
            {
                DataRow dr = gridNhomNPL.GetFocusedDataRow();
                if (dr == null) return;
                string MaNhom = dr["MaNhom"].ToString();
                string urlCheck = $"{URL}NhomNPL/GetCheck?manhom={MaNhom}";
                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                if(jsonCheck != "[]")
                {
                    XtraMessageBox.Show("Loại vật tư này đã được sử dụng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }    


                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gridNhomNPL.FocusedRowHandle >= 0)
                    {
                        int ID = Int32.Parse(gridNhomNPL.GetFocusedRowCellValue(colID)?.ToString());
                        string url = URL + $"NhomNPL/Delete?id={ID}&user={GlobleData.UserName}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                            LoadNhomNPL(false);
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private void barButtonItem24_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void barButtonItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            ReadExcelPL(Sfd.FileName);
        }

        private void ReadExcelPL(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                //lstNhom.Clear();
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;

                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$B4:ZZ500");
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();
                DataTable dtSave = new DataTable();
                dtSave = source.ToDataTable();
                AddList(dtSave);
            }

            catch (Exception ex)
            {

            }

        }

        private void AddList(DataTable dtSave)
        {
            string MaNhom = string.Empty, TenNhom = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}", URL + $"NhomNPL/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                dttb = JsonConvert.DeserializeObject<DataTable>(json);
            }
            if (dttb == null || dttb.Rows.Count == 0)
            {
                dttb = tblNhom.Clone();
            }

            foreach (DataRow row in dtSave.Rows)
            {
                if (string.IsNullOrWhiteSpace(row[0].ToString())) continue;
                string tenNhomMoi = row[0].ToString().Trim().Replace(" ", "").ToUpper();


                bool isDuplicate = dttb.AsEnumerable()
                                       .Any(r => r["TenNhom"].ToString().Trim().Replace(" ", "").ToUpper().Equals(tenNhomMoi, StringComparison.OrdinalIgnoreCase));

                if (!isDuplicate)
                {
                    DataRow dr = dttb.NewRow();
                    dr["ID"] = 0;
                    dr["TenNhom"] = row[0].ToString();
                    dr["TenTA"] = row[1].ToString();
                    dr["NPL"] = row[2].ToString() == "Nguyên liệu";
                    dttb.Rows.Add(dr);
                }
                else
                {
                    XtraMessageBox.Show("Trùng tên nhóm. Vui lòng kiểm tra và thử lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            string msResult = "";
            string url1 = URL + "NhomNPL/Post";
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url1, dttb); }).Result;
            LoadNhomNPL(false);
        }
        //private string ReplaceSpecialCharacters(string input)
        //{
        //    string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'\\|,.<>/?]+";
        //    string replacement = "_";
        //    Regex regex = new Regex(pattern);
        //    return regex.Replace(input.Trim(), replacement);
        //}
        private string ReplaceSpecialCharacters(string input)
        {
            // Thêm ký tự \" vào trong mẫu regex
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Import";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                ReadExcelPL(Sfd.FileName);
            }
        }
        private void barButtonItem26_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }
        private void NapLaiDong()
        {
            //Them.Enabled = true;
            //foreach (GridColumn col in gridNhomNPL.Columns)
            //{
            //    col.OptionsColumn.AllowEdit = false;
            //}
          
            LoadNhomNPL(false);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            gridNhomNPL.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            CreateSearchLookup();
        }

        private void loadThongTinNhom()
        {
            if (tblNhom != null && tblNhom.Rows.Count > 0)
            {
                DataRow dr = gridNhomNPL.GetFocusedDataRow();
                textTN.Text = dr["TenNhom"].ToString();
                txtNPL.Text = dr["LoaiVT"].ToString();
                txtTenTA.Text = dr["TenTA"].ToString();
            }
            else
            {

            }

        }
        private void CreateComboBoxLoaiVT()
        {
            RepositoryItemComboBox repositoryComboBox = new RepositoryItemComboBox();
            repositoryComboBox.Items.Add("Nguyên liệu");
            repositoryComboBox.Items.Add("Phụ liệu");
            gridControl1.RepositoryItems.Add(repositoryComboBox);
            gridNhomNPL.Columns["LoaiVT"].ColumnEdit = repositoryComboBox;
        }

        private void LoadNhomNPL(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}", URL + $"NhomNPL/Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {

                    Xoa.Enabled = false;

                    textTN.Text = "";
                    txtTenTA.Text = "";
                    txtNPL.Text = "";
                    gridControl1.DataSource = tblNhom;
                    return;

                };
                if (!string.IsNullOrEmpty(json))
                {
                    dttb = JsonConvert.DeserializeObject<DataTable>(json);
                    tblNhom = JsonConvert.DeserializeObject<DataTable>(json);
                    //loadThongTinNhom(lstNhom[0].ID);
                }
                if (tblNhom.Rows.Count == 0 || tblNhom == null)
                {
                    return;
                }
                if (!tblNhom.Columns.Contains("LoaiVT"))
                {
                    tblNhom.Columns.Add("LoaiVT", typeof(string));
                }
                foreach (DataRow row in tblNhom.Rows)
                {
                    row["LoaiVT"] = bool.TryParse(row["NPL"].ToString(), out bool isNL) && isNL ? "Nguyên liệu" : "Phụ liệu";
                }
                gridControl1.DataSource = tblNhom == null ? null : tblNhom;
                gridControl1.RefreshDataSource();
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridNhomNPL.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridControl1;
                }
            }
            catch
            {
                //XtraMessageBox.Show("Chưa có dữ liệu xin vui lòng kiểm tra lại hoặc nhập thêm dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void gridNhomNPL_Click(object sender, EventArgs e)
        {
            //var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            //if (view != null)
            //{

            //    int rowHandle = view.FocusedRowHandle;


            //    if (rowHandle >= 0)
            //    {

            //        int idValue = int.Parse(view.GetRowCellValue(rowHandle, "ID").ToString());
            //       // loadThongTinNhom();

            //    }
            //}
        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string mavattu = string.Empty;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (tblNhom is null || tblNhom.Rows.Count == 0) return;
            //mavattu = dttb.AsEnumerable().Where(x => x["MaVatTu"] == mavattu.ToString()).FirstOrDefault()["MaVatTu"].ToString();
            //int Size = 0;
            Sfd.FileName = string.Format("NhomNguyenPhuLieu");
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
                    ExportExcel(Sfd.FileName, tblNhom);
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
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("NhomNguyenLieu");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    range = worksheet.Cells["B1:H1"]; range.Merge = true; range.Value = ""; range.Style.Font.Bold = true;
                    range = worksheet.Cells["B2:H3"]; range.Merge = true; range.Value = "Nhóm Nguyên phụ liệu"; range.Style.Font.Bold = true;
                    dtXuatEXNhom(dtsave, worksheet);
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dtXuatEXNhom(DataTable dtSave, ExcelWorksheet worksheet, bool flagFilter = true)
        {
            ExcelRange range = worksheet.Cells;

            Color headerBlue = ColorTranslator.FromHtml("#4472C4");
            Color whiteText = Color.White;
            Color rowEvenColor = ColorTranslator.FromHtml("#D9E1F2");
            Color rowOddColor = Color.White; // Trắng
            Color borderColor = ColorTranslator.FromHtml("#305496");


            range = worksheet.Cells["C4"];
            range.Value = "Tên nhóm";
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(headerBlue);
            range.Style.Font.Color.SetColor(whiteText);
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderColor);

            range = worksheet.Cells["D4"];
            range.Merge = true;
            range.Value = "Tên tiếng Anh";
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(headerBlue);
            range.Style.Font.Color.SetColor(whiteText);
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderColor);

            range = worksheet.Cells["E4"];
            range.Merge = true;
            range.Value = "Loại vật tư";
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(headerBlue);
            range.Style.Font.Color.SetColor(whiteText);
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderColor);

            string LoGo = KHDongThungLib.getImgPath("DongNai.jpg");
            Image image = Image.FromFile(LoGo);
            OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
            picture.SetPosition(0, 0, 0, 0);
            picture.SetSize(105, 55);

            int row = 5;

            foreach (DataRow rows in dtSave.Rows)
            {
                ExcelRange cellNhom = worksheet.Cells[row, 3];
                ExcelRange cellTenTA = worksheet.Cells[row, 4];
                ExcelRange cellLoaiVT = worksheet.Cells[row, 5];

                cellNhom.Value = rows["TenNhom"].ToString();
                worksheet.Column(3).AutoFit();

                cellTenTA.Value = rows["TenTA"].ToString();
                worksheet.Column(4).AutoFit();

                cellLoaiVT.Value = rows["LoaiVT"].ToString();
                worksheet.Column(5).AutoFit();

                Color rowColor = (row % 2 == 0) ? rowEvenColor : rowOddColor;

                cellNhom.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cellNhom.Style.Fill.BackgroundColor.SetColor(rowColor);
                cellNhom.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                cellTenTA.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cellTenTA.Style.Fill.BackgroundColor.SetColor(rowColor);
                cellTenTA.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                cellLoaiVT.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cellLoaiVT.Style.Fill.BackgroundColor.SetColor(rowColor);
                cellLoaiVT.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                cellNhom.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderColor);
                cellTenTA.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderColor);
                cellLoaiVT.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderColor);

                row++;
            }

            ExcelRange dataRange = worksheet.Cells[$"C4:E{row - 1}"];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Top.Color.SetColor(borderColor);
            dataRange.Style.Border.Left.Color.SetColor(borderColor);
            dataRange.Style.Border.Right.Color.SetColor(borderColor);
            dataRange.Style.Border.Bottom.Color.SetColor(borderColor);
        }
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("NhomNguyenLieu{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateNhomNguyenPhuLieu.xlsx";
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

                        excelPackage.Workbook.Properties.Author = "NTB";

                        excelPackage.Workbook.Properties.Title = "NhomNguyenPhuLieu";

                        string templateFilePath = TemplateFileName;

                        string resultFilePath = ExportFileName;

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
        private void gridNhomNPL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            
            //var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            //if (view != null)
            //{
            //    int rowHandle = view.FocusedRowHandle;
            //    if (rowHandle >= 0)
            //    {
            //        loadThongTinNhom();
            //        Xoa.Enabled = true;
            //    }
            //}
        }

        private void gridNhomNPL_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        private void gridNhomNPL_CustomDrawColumnHeader_1(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void gridControl1_DragDrop(object sender, DragEventArgs e)
        {
            //isDragging = false;
            //// Lấy GridControl từ sender
            //GridControl grid = sender as GridControl;
            //if (grid == null) return;

            //// Lấy View (GridView hoặc CardView, v.v.)
            //GridView view = grid.MainView as GridView;
            //if (view == null) return;

            //// Lấy vị trí chuột khi thả
            //Point clientPoint = grid.PointToClient(new Point(e.X, e.Y));

            //// Lấy thông tin hàng tại vị trí thả
            //GridHitInfo hitInfo = view.CalcHitInfo(clientPoint);

            //// Kiểm tra xem vị trí thả có hợp lệ không
            //if (!hitInfo.InRow || hitInfo.RowHandle < 0)
            //{
            //    // Xóa hiệu ứng khoảng trống

            //    draggedRowHandle1 = GridControl.InvalidRowHandle;
            //    view.LayoutChanged();

            //    dragBitmap?.Dispose(); // Giải phóng bitmap
            //    dragBitmap = null;
            //    return;
            //}

            //// Lấy row handle của hàng được kéo
            //int draggedRowHandle = (int)e.Data.GetData(typeof(int));

            //// Lấy row handle của hàng đích (nơi thả)
            //int targetRowHandle = hitInfo.RowHandle;

            //// Nếu hàng được kéo và hàng đích giống nhau, không làm gì
            //if (draggedRowHandle == targetRowHandle)
            //{
            //    // Xóa hiệu ứng khoảng trống
            //    isDragging = false;
            //    draggedRowHandle1 = GridControl.InvalidRowHandle;
            //    view.LayoutChanged();
            //    dragBitmap?.Dispose(); // Giải phóng bitmap
            //    dragBitmap = null;
            //    return;
            //}

            //// Hoán đổi vị trí giữa hai hàng
            //SwapRows(view, draggedRowHandle, targetRowHandle);


            //dragBitmap?.Dispose(); // Giải phóng bitmap
            //dragBitmap = null;
        }

        private void gridControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //GridControl grid = sender as GridControl;
            //if (grid == null) return;

            //GridView view = grid.MainView as GridView;
            //if (view == null) return;

            //// Lấy thông tin về vị trí chuột
            //GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            //if (hitInfo.InRow || hitInfo.InRowCell)
            //{
            //    int rowHandle = hitInfo.RowHandle;
            //    // Lưu row handle của dòng được kéo
            //    isDragging = true;
            //    draggedRowHandle1 = rowHandle;

            //    // Yêu cầu GridView vẽ lại
            //    view.LayoutChanged();

            //    // Lấy kích thước của dòng
            //    Rectangle rowBounds = GetRowBounds(view, rowHandle);

            //    // Kiểm tra tính hợp lệ của rowBounds
            //    if (rowBounds.IsEmpty || rowBounds.Width <= 0 || rowBounds.Height <= 0)
            //    {
            //        isDragging = false;
            //        return;
            //    }

            //    // Chuyển đổi tọa độ từ GridControl sang screen
            //    Point screenLocation = grid.PointToScreen(new Point(rowBounds.Left, rowBounds.Top));

            //    // Chụp hình ảnh của dòng
            //    try
            //    {
            //        using (Bitmap tempBitmap = new Bitmap(rowBounds.Width, rowBounds.Height))
            //        {
            //            using (Graphics g = Graphics.FromImage(tempBitmap))
            //            {
            //                g.CopyFromScreen(screenLocation, Point.Empty, rowBounds.Size);
            //            }
            //            dragBitmap = new Bitmap(tempBitmap);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Lỗi khi chụp hình ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        dragBitmap?.Dispose();
            //        dragBitmap = null;
            //        return;
            //    }

            //    // Bắt đầu kéo
            //    grid.DoDragDrop(rowHandle, DragDropEffects.Move);
            //}
        }

        private void SwapRows(GridView view, int rowHandle1, int rowHandle2)
        {
            // Tạo một từ điển tạm để lưu trữ dữ liệu của hàng 1
            Dictionary<GridColumn, object> tempRowData = new Dictionary<GridColumn, object>();

            // Lưu dữ liệu của hàng 1 vào biến tạm
            foreach (GridColumn column in view.Columns)
            {
                if (column.FieldName == "Sort") continue;
                tempRowData[column] = view.GetRowCellValue(rowHandle1, column);
            }

            // Sao chép dữ liệu từ hàng 2 sang hàng 1
            foreach (GridColumn column in view.Columns)
            {
                if (column.FieldName == "Sort") continue;
                object value2 = view.GetRowCellValue(rowHandle2, column);
                view.SetRowCellValue(rowHandle1, column, value2);
            }

            // Sao chép dữ liệu từ biến tạm (hàng 1) sang hàng 2
            foreach (GridColumn column in view.Columns)
            {
                if (column.FieldName == "Sort") continue;
                view.SetRowCellValue(rowHandle2, column, tempRowData[column]);
            }
        }

        private Rectangle GetRowBounds(GridView view, int rowHandle)
        {
            // Lấy GridControl từ GridView
            GridControl grid = view.GridControl;
            if (grid == null) return Rectangle.Empty;

            // Lấy thông tin về dòng từ GridViewInfo
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            if (viewInfo == null) return Rectangle.Empty;

            // Sử dụng phương thức GetGridRowInfo để lấy thông tin dòng chính xác
            GridDataRowInfo rowInfo = viewInfo.GetGridRowInfo(rowHandle) as GridDataRowInfo;
            if (rowInfo == null) return Rectangle.Empty;

            // Trả về kích thước và vị trí của dòng
            return rowInfo.Bounds;
        }

        private Cursor CreateCursor(Bitmap bmp, Point hotSpot)
        {
            // Tạo con trỏ chuột từ Bitmap và HotSpot
            IntPtr hIcon = bmp.GetHicon();
            IconInfo iconInfo = new IconInfo();
            GetIconInfo(hIcon, ref iconInfo);

            // Thiết lập HotSpot
            iconInfo.xHotspot = hotSpot.X;
            iconInfo.yHotspot = hotSpot.Y;
            iconInfo.fIcon = false; // Đặt là false để tạo con trỏ chuột (cursor), không phải icon

            // Tạo con trỏ chuột mới
            IntPtr hCursor = CreateIconIndirect(ref iconInfo);

            // Giải phóng tài nguyên
            DestroyIcon(hIcon);
            if (iconInfo.hbmMask != IntPtr.Zero) DeleteObject(iconInfo.hbmMask);
            if (iconInfo.hbmColor != IntPtr.Zero) DeleteObject(iconInfo.hbmColor);

            return new Cursor(hCursor);
        }

        // Cấu trúc IconInfo để lưu thông tin về icon
        private struct IconInfo
        {
            public bool fIcon;      // true nếu là icon, false nếu là cursor
            public int xHotspot;    // Tọa độ X của HotSpot
            public int yHotspot;    // Tọa độ Y của HotSpot
            public IntPtr hbmMask;  // Handle của bitmap mask
            public IntPtr hbmColor; // Handle của bitmap màu
        }

        // Các hàm API cần thiết
        [DllImport("user32.dll")]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private void gridControl1_DragLeave(object sender, EventArgs e)
        {
            //dragBitmap?.Dispose(); // Giải phóng bitmap
            //dragBitmap = null;
        }

        private void gridControl1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            //if (dragBitmap != null)
            //{
            //    // Hiển thị hình ảnh của dòng ngay trên con trỏ chuột
            //    e.UseDefaultCursors = false;
            //    Cursor.Current = CreateCursor(dragBitmap, new Point(10, 10)); // Điều chỉnh vị trí của hình ảnh
            //}
            //else
            //{
            //    e.UseDefaultCursors = true;
            //}
        }

        private void gridControl1_DragEnter(object sender, DragEventArgs e)
        {
            //// Kiểm tra xem dữ liệu được kéo có phải là một số nguyên (row handle) không
            //if (e.Data.GetDataPresent(typeof(int)))
            //{
            //    e.Effect = DragDropEffects.Move;
            //}
            //else
            //{
            //    e.Effect = DragDropEffects.None;
            //}
        }

        private void gridNhomNPL_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null) return;

            //// Kiểm tra xem có đang kéo và có phải dòng được kéo không
            //if (isDragging && e.RowHandle == draggedRowHandle1)
            //{
            //    // Vẽ hình chữ nhật trong suốt hoặc có màu nền nhạt
            //    using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.LightGray))) // Màu nền nhạt với độ trong suốt
            //    {
            //        e.Graphics.FillRectangle(brush, e.Bounds);
            //    }

            //    // Ngăn không cho GridView vẽ lại ô này
            //    e.Handled = true;
            //}
        }

        private void gridControl1_MouseMove(object sender, MouseEventArgs e)
        {
            //GridControl grid = sender as GridControl;
            //if (grid == null) return;

            //GridView view = grid.MainView as GridView;
            //if (view == null) return;

            //// Kiểm tra nếu người dùng đang nhấn chuột trái và di chuyển
            //if (e.Button == MouseButtons.Left)
            //{
            //    // Lấy thông tin về vị trí chuột
            //    GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            //    if (hitInfo.InRow || hitInfo.InRowCell)
            //    {
            //        // Đánh dấu là đang kéo
            //        //isDragging = true;
            //        draggedRowHandle1 = hitInfo.RowHandle;

            //        // Yêu cầu GridView vẽ lại để hiển thị khoảng trống
            //        view.LayoutChanged();

            //        // Bắt đầu kéo
            //        grid.DoDragDrop(hitInfo.RowHandle, DragDropEffects.Move);
            //    }
            //}
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }
        public void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }
        private void gridNhomNPL_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }
        private void gridNhomNPL_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Value == null) return;
            if (e.Column.FieldName == "LoaiVT")
            {
               
                string selectedValue = e.Value.ToString();
                DataRow focusedRow = gridNhomNPL.GetDataRow(gridNhomNPL.FocusedRowHandle);

                if (focusedRow != null)
                {
                    if (selectedValue == "Phụ liệu")
                    {
                        focusedRow["NPL"] = false;  
                        focusedRow["LoaiVT"] = "Phụ liệu";  
                    }
                    else if (selectedValue == "Nguyên liệu")
                    {
                        focusedRow["NPL"] = true;  
                        focusedRow["LoaiVT"] = "Nguyên liệu"; 
                    }
                }
            }

            lstRowUpdate.Add(e.RowHandle);
        }
        public string RemoveVietnameseTone(string text)
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
            string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
            result = result.Replace('đ', 'd');
            return result;
        }

        private void gridNhomNPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
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

        private void gridNhomNPL_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            var menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridNhomNPL.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();
                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {
                        //DevExpress.Utils.Menu.DXMenuItem menuCoppyPasteItem = new DevExpress.Utils.Menu.DXMenuItem("▲ Up", ItemUP_Click);
                        var menuUp = new DevExpress.Utils.Menu.DXMenuItem("▲ Up", ItemUP_Click);
                        //e.Menu.Items.Add(menuCoppyPasteItem);
                        e.Menu.Items.Add(menuUp);
                        //DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("▼ Down", ItemDown_Click);
                        //e.Menu.Items.Add(menuDeleteItem);

                        var menuDown = new DevExpress.Utils.Menu.DXMenuItem("▼ Down", ItemDown_Click);
                        e.Menu.Items.Add(menuDown);
                    }

                }
            }
        }

        private void ItemUP_Click(object sender, EventArgs e)
        {
            //var view = gridNhomNPL;
            //int rowIndex = view.FocusedRowHandle;

            //if (rowIndex <= 0)
            //    return;

            //SwapRows1(rowIndex, rowIndex - 1);

            var view = gridNhomNPL;
            int rowIndex = view.FocusedRowHandle;

            if (rowIndex <= 0)
                return;

            SwapRows1(rowIndex, rowIndex - 1);
            view.FocusedRowHandle = rowIndex - 1;
        }

        public void ItemDown_Click(object sender, EventArgs e)
        {
            //var view = gridNhomNPL;
            //int rowIndex = view.FocusedRowHandle;

            //if (rowIndex < 0 || rowIndex >= view.RowCount - 1)
            //    return;

            //SwapRows1(rowIndex, rowIndex + 1);

            var view = gridNhomNPL;
            int rowIndex = view.FocusedRowHandle;

            if (rowIndex < 0 || rowIndex >= view.RowCount - 1)
                return;

            SwapRows1(rowIndex, rowIndex + 1);
            view.FocusedRowHandle = rowIndex + 1;
        }

        private void SwapRows1(int index1, int index2)
        {
            var view = gridControl1;
            var dataTable = view.DataSource as DataTable;

            if (dataTable == null || index1 < 0 || index2 < 0 ||
                index1 >= dataTable.Rows.Count || index2 >= dataTable.Rows.Count)
                return;

            // Hoán đổi dữ liệu giữa hai dòng
            DataRow row1 = dataTable.Rows[index1];
            DataRow row2 = dataTable.Rows[index2];

            // Tạo bản sao để tạm lưu dữ liệu row1
            DataRow tempRow = dataTable.NewRow();
            tempRow.ItemArray = row1.ItemArray.Clone() as object[];

            // Hoán đổi dữ liệu
            row1.ItemArray = row2.ItemArray;
            row2.ItemArray = tempRow.ItemArray;

            // Cập nhật lại giá trị cột Sort cho toàn bộ bảng
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i]["Sort"] = i + 1; // hoặc i nếu bạn bắt đầu từ 0
            }

            //// Refresh view và focus lại vào dòng đã chọn
            //view.Refresh();
            //view.FocusedRowHandle = index2;

            ManuallyMarkRowAsUpdated(index1);
            ManuallyMarkRowAsUpdated(index2);
            UpdateSort();
        }

        private void ManuallyMarkRowAsUpdated(int rowHandle)
        {
            if (!lstRowUpdate.Contains(rowHandle))
                lstRowUpdate.Add(rowHandle);
        }
        private void UpdateSort()
        {
            try
            {
                this.ActiveControl = this.button1;
                if (tblNhom.Rows.Count == 0 || tblNhom == null) return;
                string dup = CheckDuplicateTenNhom_Loop(tblNhom);
                if (dup != null)
                {
                    XtraMessageBox.Show($"Trùng tên nhóm: {dup} . Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tblNhom.Rows.Count == 0 || tblNhom == null) return;
                string sort = CheckDuplicateSort_Loop(tblNhom);
                if (sort != null)
                {
                    XtraMessageBox.Show($"Trùng sắp xếp: {sort} . Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tblNhom.Columns.Contains("LoaiVT"))
                {
                    foreach (DataRow row in tblNhom.Rows)
                    {
                        string manhom = RemoveVietnameseTone(ReplaceSpecialCharacters(row["TenNhom"].ToString().Replace(" ", "")));
                        row["MaNhom"] = manhom.ToString().ToUpper().Trim();
                        row["NPL"] = row["LoaiVT"].ToString() == "Nguyên liệu" ? true : false;
                    }
                    tblNhom.Columns.Remove("LoaiVT");
                }
                var rowsToDelete = tblNhom.AsEnumerable()
                          .Where(row => string.IsNullOrEmpty(row.Field<string>("TenNhom")))
                          .ToList();

                foreach (var row in rowsToDelete)
                {
                    tblNhom.Rows.Remove(row);
                }
                string msResult = "";

                string url = URL + "NhomNPL/UpdateSort";
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhom); }).Result;

                if (msResult.ToLower() == "true")
                {
                    LoadNhomNPL(false);
                    //foreach (GridColumn col in gridNhomNPL.Columns)
                    //{
                    //    col.OptionsColumn.AllowEdit = false;
                    //}
                    //clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);

            }
            catch (Exception ex)
            {


            }

        }

        private void btnUp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int rowIndex = gridNhomNPL.FocusedRowHandle;
            if (rowIndex > 0)
            {
                SwapRows1(rowIndex, rowIndex - 1);
                gridNhomNPL.FocusedRowHandle = rowIndex - 1;
            }
        }

        private void btnDown_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int rowIndex = gridNhomNPL.FocusedRowHandle;
            if (rowIndex >= 0 && rowIndex < gridNhomNPL.RowCount - 1)
            {
                SwapRows1(rowIndex, rowIndex + 1);
                gridNhomNPL.FocusedRowHandle = rowIndex + 1;
            }
        }

        private string CheckDuplicateTenNhom_Loop(DataTable dt)
        {
            HashSet<string> seenValues = new HashSet<string>();

            foreach (DataRow row in dt.Rows)
            {
                string value = row["TenNhom"]?.ToString();
                if (string.IsNullOrEmpty(value))
                    continue;

                if (!seenValues.Add(value)) 
                {
                    return value;
                }
            }

            return null;
        }
        private string CheckDuplicateSort_Loop(DataTable dt)
        {
            HashSet<string> seenValues = new HashSet<string>();

            foreach (DataRow row in dt.Rows)
            {
                string value = row["Sort"]?.ToString();
                if (string.IsNullOrEmpty(value))
                    continue;

                if (!seenValues.Add(value))
                {
                    return value;
                }
            }

            return null;
        }

        private void gV_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView != null && gridView.FocusedColumn != null)
            {

                Type columnType = gridView.FocusedColumn.ColumnType;
                if (columnType == typeof(string))
                {
                    if (Char.IsLetter(e.KeyChar))

                        e.KeyChar = Char.ToUpper(e.KeyChar);
                }
                
            }
        }

        private void gC_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gV_KeyPress(grid.FocusedView, e);
        }

        private void gridNhomNPL_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "LoaiVT")
            {
                // Lấy giá trị của ô hiện tại trong cột đang xét
                object cellValue = e.CellValue;

                if (cellValue != null)
                {
                    if (cellValue.ToString() == "Nguyên liệu")
                    {
                        //e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#EAF2F8"); 
                        e.Appearance.ForeColor = System.Drawing.ColorTranslator.FromHtml("#2874A6"); 
                    }
                    else
                    {
                        // Màu xanh lá cây nhạt, chữ xanh lá đậm -> Phân biệt rõ ràng
                        //e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#E9F7EF"); 
                        e.Appearance.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1E8449"); 
                    }
                }
            }
        }

        /*Validate*/

        private void gridNhomNPL_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            GridColumn colFocus = view.FocusedColumn;
            string newValue = e.Value?.ToString().Trim();
            if (view.FocusedRowHandle < 0) return;
            if (colFocus == colTenNhom ||  colFocus == colTenTA)//||colFocus == colVietTat)
            {

                if (string.IsNullOrWhiteSpace(newValue))
                {
                    e.Valid = false;
                    e.ErrorText = $"{colFocus.Caption} không được để trống.";
                    return;
                }

                // Kiểm tra trùng giá trị trong cột tương ứng, loại trừ dòng hiện tại
                for (int i = 0; i < view.RowCount; i++)
                {
                    if (i == view.FocusedRowHandle || view.IsGroupRow(i)) continue;

                    string otherValue = view.GetRowCellValue(i, colFocus)?.ToString().Trim();
                    if (newValue.Equals(otherValue, StringComparison.OrdinalIgnoreCase))
                    {
                        e.Valid = false;
                        e.ErrorText = $"{view.FocusedColumn.Caption} đã bị trùng với dòng khác.";
                        break;
                    }
                }
            }
          


        }

        private void gridNhomNPL_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            // Chỉ áp dụng logic cho cột 'LoaiVT'
            if (view.FocusedColumn.FieldName == "LoaiVT")
            {
                // Kiểm tra điều kiện, ví dụ: Nhóm đã được sử dụng
                if (!loadCheckVatTu())
                {
                    XtraMessageBox.Show($"Nhóm đã được sử dụng. Không thể chỉnh sửa loại vật tư!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                }
            }
        }

        private void grv_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();


        }
        private bool loadCheckVatTu()
        {
            DataRow dr = gridNhomNPL.GetFocusedDataRow();
            string MaNhom = dr["MaNhom"].ToString();
            string urlCheck = $"{URL}NhomNPL/GetCheck?manhom={MaNhom}";
            string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
            if (jsonCheck != "[]")
            {
                return false;
            }
            return true;
        }
    }
}