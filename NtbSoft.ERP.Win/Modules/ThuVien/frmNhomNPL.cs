using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmNhomNPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        DataTable tblNhom = new DataTable();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        public frmNhomNPL()
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
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSNhom(false);
            createComboBoxLoaiVT();
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
        
      

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gVNhom.OptionsBehavior.Editable = false;

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
                    gVNhom.OptionsBehavior.Editable = true;
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
                    gVNhom.OptionsBehavior.Editable = true;
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
        private void LoadDSNhom(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "NhomNPL/Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    tblNhom = CreateTableNhom();
                    gCNhom.DataSource = null;
                    return; 
                }
                tblNhom = JsonConvert.DeserializeObject<DataTable>(json);
                gCNhom.DataSource = tblNhom;

                GridViewUpdateStatus(_status);

                if (isFromSave && FocusedIndex >= 0)
                {
                    gVNhom.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gCNhom;
                }
            }catch(Exception ex)
            {
                
            }
            
        }
        private void createComboBoxLoaiVT()
        {
            RepositoryItemComboBox comboBox = new RepositoryItemComboBox();
            comboBox.Items.Add("Nguyên liệu");
            comboBox.Items.Add("Phụ liệu");
            comboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            comboBox.EditValueChanged += ComboBox_EditValueChanged;
            gCNhom.RepositoryItems.Add(comboBox);

            gVNhom.Columns["LoaiVT"].ColumnEdit = comboBox;
        }
        private void ComboBox_EditValueChanged(object sender, EventArgs e)
        {
            // Lấy giá trị mới được chọn
            ComboBoxEdit editor = sender as ComboBoxEdit;
            if (editor != null)
            {
                string selectedValue = editor.EditValue?.ToString();

                int rowHandle = gVNhom.FocusedRowHandle;

                if (rowHandle >= 0)
                {
                    if (selectedValue == "Nguyên liệu")
                    {
     
                        gVNhom.SetRowCellValue(rowHandle, "NPL", true);
                    }
                    else
                    {
                    
                        gVNhom.SetRowCellValue(rowHandle, "NPL", false);
                    }
                }
            }
        }
        private void ThemDong()
        {
            DataTable tbl = gCNhom.DataSource as DataTable;
            if(tbl==null||tbl.Rows.Count==0)
            {
                tbl = CreateTableNhom();
            }    
            DataRow row = tbl.NewRow();
            row["ID"] = 0;
            int maxSort = tblNhom.AsEnumerable()
                    .Where(r => r["Sort"] != DBNull.Value)
                    .Select(r => Convert.ToInt32(r["Sort"]))
                    .DefaultIfEmpty(0)
                    .Max();
            row["LoaiVT"] = "Nguyên liệu";
            row["NPL"] = true;
            row["Sort"] = maxSort + 1;
            tbl.Rows.Add(row);


            _rowAdd = gVNhom.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gVNhom.FocusedRowHandle = _rowAdd;
        }
        private DataTable CreateTableNhom()
        {
            DataTable dtbcreate = new DataTable("dtbcreate");
            dtbcreate.Columns.Add("ID", typeof(int));
            dtbcreate.Columns.Add("MaNhom", typeof(string));
            dtbcreate.Columns.Add("TenNhom", typeof(string));
            dtbcreate.Columns.Add("NPL", typeof(string));
            dtbcreate.Columns.Add("TenTA", typeof(string));
            dtbcreate.Columns.Add("Sort", typeof(string));
            dtbcreate.Columns.Add("LoaiVT", typeof(string));
         
            return dtbcreate;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            focused(this.gVNhom);
            gridColumn5.OptionsColumn.AllowEdit = false;
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
                    DataRow row = gVNhom.GetFocusedDataRow();
                    if (row == null) return;
                    if(row["ID"].ToString()!="0")
                    {
                        string url = string.Format("{0}?id={1}", URL + "NhomNPL/Delete", row["ID"]);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    }
                    DataTable tbl = gCNhom.DataSource as DataTable;
                    tbl.Rows.Remove(row);
                    gCNhom.DataSource = tbl;
                  
                }
            }catch(Exception ex)
            {
               
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
                this.ActiveControl = button1;
                DataTable tbl = gCNhom.DataSource as DataTable;
                if (!IsTenNhomNotEmpty(tbl))
                {
                    XtraMessageBox.Show("Nhóm không được rỗng.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!checkTrung(tbl))
                {
                    XtraMessageBox.Show("Nhóm bị trùng.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } 
                    

                if(tbl.Columns.Contains("LoaiVT"))
                {
                    tbl.Columns.Remove("LoaiVT");
                }    
                string url = string.Format("{0}?", URL + "NhomNPL/Post");
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;

                if (msResult.ToLower() == "true")
                {
                    LoadDSNhom(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                lstRowUpdate.Clear();
            }
            catch(Exception ex)
            {
               
            }
            
        }

        private bool checkTrung(DataTable tbl)
        {
            HashSet<string> seen = new HashSet<string>();

            foreach (DataRow row in tbl.Rows)
            {
                string rawTenNhom = row["TenNhom"]?.ToString();

                if (!string.IsNullOrWhiteSpace(rawTenNhom))
                {
                  
                    string normalizedTenNhom = rawTenNhom
                        .ToUpperInvariant()
                        .Replace(" ", "")    
                        .Trim();

                    if (seen.Contains(normalizedTenNhom))
                    {
                        return false;
                    }
                    else
                    {
                        seen.Add(normalizedTenNhom);
                    }
                }
            }

            return true;
        }
        public bool IsTenNhomNotEmpty(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                string tenNhom = row["TenNhom"]?.ToString();

                if (string.IsNullOrWhiteSpace(tenNhom))
                {
                  
                    return false;
                }
            }

          
            return true;
        }
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
           
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);

            LoadDSNhom(false);
            gVNhom.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }


        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void gridViewChungLoai_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            //focused(sender);
        }

        private void gridViewChungLoai_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridViewChungLoai_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
           /* DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd && !(view.FocusedColumn == colMaCL))
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == colMaCL)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
           
            }*/
        }

        //private void gridControlChungLoai_EditorKeyPress(object sender, KeyPressEventArgs e)
        private void gridControlChungLoai_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            GridView view = grid.FocusedView as GridView;
        }

        private bool CheckCharacterID(GridView view, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
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

        private void gridViewChungLoai_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gVNhom.GetFocusedDataSourceRowIndex() >= 0)
            {
               
            }
        }
        private bool CheckDupLiCateString(string firstString, string secondString)
        {
            if (firstString == null || (firstString != null && string.IsNullOrEmpty(firstString)))
            {
                return false;
            }
            return firstString.Replace(" ", "").ToUpper().Equals(secondString.Replace(" ", "").ToUpper());
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        private void gridViewChungLoai_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridViewChungLoai_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview 
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gVNhom_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVNhom.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Sắp xếp lên", ItemUp_Click);
                        e.Menu.Items.Add(menuCopyItem);

                        DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Sắp xếp xuống ", ItemDown_Click);
                        e.Menu.Items.Add(menuCopyMauItem);

                        
                       
                    }

                }
            }
        }
        private void ItemUp_Click(object sender, EventArgs e)
        {
            Luu.Enabled = true;
            int focusedRowHandle = gVNhom.FocusedRowHandle;
            if(focusedRowHandle>0)
            {
                DataRow row = gVNhom.GetDataRow(focusedRowHandle);
                DataRow preRow = gVNhom.GetDataRow(focusedRowHandle - 1);
                if (row == null || preRow == null) return;
             
                object currentSort = row["sort"];
                object prevSort = preRow["sort"];

                row["sort"] = prevSort;
                preRow["sort"] = currentSort;
                
                DataTable tbl = gCNhom.DataSource as DataTable;
                DataRow newPreRow = tbl.NewRow();
                newPreRow.ItemArray = preRow.ItemArray.Clone() as object[];
                tbl.Rows.Remove(preRow);
                tbl.Rows.InsertAt(newPreRow, focusedRowHandle);
                gCNhom.DataSource = tbl;
               
            }    
            
        }
        private void ItemDown_Click(object sender, EventArgs e)
        {
            Luu.Enabled = true;
            int focusedRowHandle = gVNhom.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DataRow row = gVNhom.GetDataRow(focusedRowHandle);
                DataRow afRow = gVNhom.GetDataRow(focusedRowHandle + 1);
                if (row == null || afRow == null) return;
                object currentSort = row["sort"];
                object afSort = afRow["sort"];

                row["sort"] = afSort;
                afRow["sort"] = currentSort;
                DataTable tbl = gCNhom.DataSource as DataTable;
                DataRow newRow = tbl.NewRow();
                newRow.ItemArray = row.ItemArray.Clone() as object[];
                tbl.Rows.Remove(row);
                tbl.Rows.InsertAt(newRow, focusedRowHandle+1);
                gCNhom.DataSource = tbl;

            }
        }


        private void gridViewChungLoai_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

    }
}
