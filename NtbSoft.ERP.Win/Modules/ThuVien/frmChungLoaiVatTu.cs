using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Kho;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmChungLoaiVatTu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;

        private DataTable tblChungLoai_CT = new DataTable();
        private DataTable tblKhoVai = new DataTable();
        private DataTable tblTheKhoVai = new DataTable();
        List<ERPDonViVTEntity> lstDonViVTEntity;

        string clvt = string.Empty;
        bool indicatorIcon = true;
        public bool Result { get; set; }
        #region Create Table

        private void CreateTableChungLoaiCT()
        {
            tblChungLoai_CT = new DataTable("tblChungLoaiCT");
            tblChungLoai_CT.Columns.Add("ID", typeof(int));
            tblChungLoai_CT.Columns.Add("MaNhom", typeof(string));
            tblChungLoai_CT.Columns.Add("TenNhom", typeof(string));
            tblChungLoai_CT.Columns.Add("NPL", typeof(bool));
            tblChungLoai_CT.Columns.Add("TenTA", typeof(string));
            tblChungLoai_CT.Columns.Add("Sort", typeof(int));
            tblChungLoai_CT.Columns.Add("VietTat", typeof(string));
            tblChungLoai_CT.Columns.Add("MaCLVT", typeof(string));

        }

        private void CreateTableKhoVai()
        {
            tblKhoVai = new DataTable("tblKhoVai");
            tblKhoVai.Columns.Add("ID", typeof(int));
            tblKhoVai.Columns.Add("KhoVaiID", typeof(string));
            tblKhoVai.Columns.Add("KhoVai", typeof(string));
            tblKhoVai.Columns.Add("NPL", typeof(bool));
            tblKhoVai.Columns.Add("MaNhom", typeof(string));
            tblKhoVai.Columns.Add("GhiChu", typeof(string));
            tblKhoVai.Columns.Add("MaDVVT", typeof(string));
            tblKhoVai.Columns.Add("KhoVaiMet", typeof(decimal));
            tblKhoVai.Columns.Add("MaTheKhoVai", typeof(string));
        }
        private void CreateTableTheKhoVai()
        {
            tblTheKhoVai = new DataTable("tblTheKhoVai");
            tblTheKhoVai.Columns.Add("ID", typeof(int));
            tblTheKhoVai.Columns.Add("MaTheKhoVai", typeof(string));
            tblTheKhoVai.Columns.Add("TheKhoVai", typeof(string));
            tblTheKhoVai.Columns.Add("MaCLVT", typeof(string));
            tblTheKhoVai.Columns.Add("UserName", typeof(string));
            tblTheKhoVai.Columns.Add("CreateDate", typeof(DateTime));
           
        }
        #endregion
        #region GridView Chủng Loại

        public frmChungLoaiVatTu()
        {
            InitializeComponent();
            this.KeyPreview = true;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        public frmChungLoaiVatTu(string clvt)
        {
            InitializeComponent();
            this.KeyPreview = true;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this.clvt = clvt;
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            CreateComboBoxLoaiVT();
            CreateTableChungLoaiCT();
            CreateTableTheKhoVai();
            CreateTableKhoVai();
            LoadDS();
            if (!string.IsNullOrEmpty(clvt))
            {
                string[] arrCLVT = clvt.Split('|');
                if(arrCLVT.Length > 0)
                {
                    foreach(string item in arrCLVT)
                    {
                        AddRowCL(item);
                    }
                }
              
            }
            Result = true;
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
                btnAddChungLoai.Enabled = false;
                btnAddKhoSize.Enabled = false;
                btnAddTheKV.Enabled = false;
                btnAdd_CT.Enabled = false;
            }
            if (!_allowEdit)
            {
                btnSaveKV.Enabled = false;
                btnSaveTheSize.Enabled = false;
                btnSave_CL.Enabled = false;
                btnSave_CT.Enabled = false;

                //Sua.Enabled = false;
                //Luu.Enabled = false;
            }
            //else
            //{
            //    Luu.Enabled = false;
            //}
            if (!_allowDelete)
            {
                //Xoa.Enabled = false;
                btnDeleteChungLoai.Enabled = false;
                btnDeleteTheSize.Enabled = false;
                btnDeleteTheKV.Enabled = false;
                btnDelete_CT.Enabled = false;
            }
                

        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    grvChungLoai.OptionsBehavior.Editable = true;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    grvChungLoai.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    grvChungLoai.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        // Them.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                    }

                    break;
            }
        }

        private void LoadDS()
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "ChungLoaiVatTu/Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    grcChungLoai.DataSource = null;
                    grcChungLoaiCT.DataSource = null;
                    grcKhoVai.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                grcChungLoai.DataSource = tbl;
               
            }
            catch (Exception ex)
            {

            }

        }
    
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            LoadDS();
        }

      
        private void Editor_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextEdit editor = sender as TextEdit;
            if (editor == null) return;

            // Lấy GridView chứa editor này
            GridView view = grvChungLoaiCT;

            if (view.FocusedColumn.FieldName != "GhiChu")
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }


        private void focused(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            if (view == null) return;

            if (view.FocusedRowHandle >= 0)
            {
                view.FocusedColumn.OptionsColumn.AllowEdit = true;
                if (view.FocusedColumn == colLoaiVT_CL || view.FocusedColumn == colVietTat)
                {
                    if(bool.TryParse(view.GetFocusedRowCellValue(colIsUse)?.ToString(),out bool IsUse))
                    {
                        if (IsUse)
                        {
                            view.FocusedColumn.OptionsColumn.AllowEdit = false;
                        }
                       
                    }
                   
                }
            }
       
           
        }

        bool IsFormat_KhoSize = false;
        private void gridViewChungLoai_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
        
            if (e.Column.FieldName != "LoaiVT" || e.Value == null)
                return;

            DataRow focusedRow = grvChungLoai.GetFocusedDataRow();
            if (focusedRow == null)
                return;
            bool.TryParse(focusedRow["IsNPL_Goc"]?.ToString(), out bool IsNPL_Goc);
            string selectedValue = e.Value.ToString();
            bool isNguyenLieu = selectedValue == "Nguyên liệu";
           
            focusedRow["IsNPL"] = isNguyenLieu;
            focusedRow["LoaiVT"] = selectedValue;
            if (focusedRow["ID"]?.ToString() != "0" && IsNPL_Goc != isNguyenLieu)
            {
                

                DataTable tblKhoSize = grcKhoVai.DataSource as DataTable;
                if (tblKhoSize?.Rows?.Count > 0)
                {
                    bool isValidFormat = true;

                    foreach (DataRow row in tblKhoSize.Rows)
                    {

                        if (isNguyenLieu)
                        {
                            if (!int.TryParse(row["KhoVai"]?.ToString(), out int _))
                            {
                                isValidFormat = true;
                                break;
                            }
                        }
                        else
                        {
                            if (int.TryParse(row["KhoVai"]?.ToString(), out int _) && row["MaDVVT"]?.ToString() != "DVVT_10")
                            {
                                isValidFormat = true;
                                break;
                            }
                        }

                    }

                    if (isValidFormat)
                    {
                        ShowWarning("Khi thay đổi loại chủng loại vật tư .Vui lòng nhập lại đúng định dạng khổ/size theo loại vật tư đó");
                        grvChungLoai.SetFocusedRowCellValue(colIsNPL, !isNguyenLieu);
                        grvChungLoai.SetFocusedRowCellValue(colLoaiVT_CL, isNguyenLieu ? "Phụ liệu" : "Nguyên liệu");
                        return;
                    }
                }
            }
           
            if (int.TryParse(focusedRow["ID"]?.ToString(), out int id) && id > 0)
            {
                string content = clsWriteLogThuVienLib.FormatRow(focusedRow);
                var existingLog = lstLog.FirstOrDefault(x => x.ID == id);

                if (existingLog != null)
                {
                    existingLog.Content = content;
                }
                else
                {
                    lstLog.Add(new LogThuvienEntity
                    {
                        ID = id,
                        Action = $"Sửa {this.Text}",
                        Module = this.Name,
                        Content = content,
                        UserID = GlobleData.UserName,
                        CreatedDate = DateTime.Now
                    });
                }
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MoveRow(-1);
        }

        private void gridViewChungLoai_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "IsNPL")
            {
                string value = e.CellValue?.ToString();
                if (value == "Nguyên Liệu")
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
                else if (value == "Phụ Liệu")
                {
                    e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MoveRow(1);
        }

    
        private void gridViewChungLoai_RowCellStyle(object sender, RowCellStyleEventArgs e)
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
     
        private void MoveRow(int direction)
        {
            GridView view = grvChungLoai;
            int index = view.FocusedRowHandle;
            if (index < 0) return;

            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= view.RowCount) return;

            DataTable dt = grcChungLoai.DataSource as DataTable;
            if (dt == null) return;

            DataRow currentRow = view.GetDataRow(index);
            DataRow targetRow = view.GetDataRow(newIndex);

            int currentSort = Convert.ToInt32(currentRow["Sort"]);
            int targetSort = Convert.ToInt32(targetRow["Sort"]);
            currentRow["Sort"] = targetSort;
            targetRow["Sort"] = currentSort;

            object[] currentValues = currentRow.ItemArray;
            object[] targetValues = targetRow.ItemArray;

            dt.Rows[index].ItemArray = targetValues;
            dt.Rows[newIndex].ItemArray = currentValues;

            grcChungLoai.DataSource = dt;
            grvChungLoai.FocusedRowHandle = newIndex;
        }
        private void CreateComboBoxLoaiVT()
        {
            RepositoryItemComboBox repositoryComboBox = new RepositoryItemComboBox();
            repositoryComboBox.Items.Add("Nguyên liệu");
            repositoryComboBox.Items.Add("Phụ liệu");
            repositoryComboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
            grcChungLoai.RepositoryItems.Add(repositoryComboBox);
            grvChungLoai.Columns["LoaiVT"].ColumnEdit = repositoryComboBox;
        }
        private void gridViewChungLoai_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if(grvChungLoai.FocusedRowHandle >= 0)
            {
                string MaChungLoai = view.GetFocusedRowCellValue(colMaCL)?.ToString();
                string IsNPL = view.GetFocusedRowCellValue(colIsNPL)?.ToString();
                SetColumnKhoVai(IsNPL);
                LoadChungLoaiCT(MaChungLoai);
                LoadTheKhoVai(MaChungLoai);
            }
            focused(this.grvChungLoai);
        }
        private void grvChungLoai_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(this.grvChungLoai);
        }
    
        private void gridViewChungLoai_DataSourceChanged(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view != null)
            {

                string MaChungLoai = view.GetFocusedRowCellValue(colMaCL)?.ToString();
                LoadChungLoaiCT(MaChungLoai);
                LoadTheKhoVai(MaChungLoai);
                view.FocusedRowHandle = 0;
            }
        }
        private void btnSortUp_CL_Click(object sender, EventArgs e)
        {
            MoveRow(-1);

        }
        private void btnSortDown_CL_Click(object sender, EventArgs e)
        {
            MoveRow(1);

        }

        private void AddRowCL(string ChungLoaiVatTu)
        {
            this.ActiveControl = button1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            DataTable dt = grcChungLoai.DataSource as DataTable;
            //if (dt == null)
            //{
            //    grvChungLoai.AddNewRow();
            //    return;
            //}


            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("MaCLVT", typeof(string));
                dt.Columns.Add("ChungLoaiVatTu", typeof(string));
                dt.Columns.Add("VietTat", typeof(string));
                dt.Columns.Add("IsNPL", typeof(string));
                dt.Columns.Add("Sort", typeof(int));
                dt.Columns.Add("TenKhac", typeof(string));
                dt.Columns.Add("LoaiVT", typeof(string));
                dt.Columns.Add("IsUse", typeof(bool));
            }

            //DataRow newRow = dt.NewRow();
            //dt.Rows.InsertAt(newRow, 0);
            //gridChungLoai.DataSource = dt;
            //gridViewChungLoai.FocusedRowHandle = 0;

            int maxSort = dt.AsEnumerable()
                .Select(r => Convert.ToInt32(r["Sort"]))
                .DefaultIfEmpty(0)
                .Max();
            DataRow newRow = dt.NewRow();
            newRow["Sort"] = maxSort + 1;
            newRow["ChungLoaiVatTu"] = ChungLoaiVatTu;
            newRow["ID"] = 0;
            dt.Rows.InsertAt(newRow, 0);
            grcChungLoai.DataSource = dt;
            grvChungLoai.FocusedRowHandle = 0;
            //if (grvChungLoai.VisibleColumns.Count > 0)
            //{
            //    grvChungLoai.FocusedColumn = grvChungLoai.VisibleColumns[0];
            //    grvChungLoai.ShowEditor();
            //}
        }
        private void btnAddChungLoai_Click(object sender, EventArgs e)
        {
            AddRowCL("");
        }
        private void btnDelete_CL_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvChungLoai.GetSelectedRows();

                // Duyệt ngược để tránh handle bị lệch
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow row = grvChungLoai.GetDataRow(rowHandle);
                    if (row == null) continue;

                    string urlCheck = $"{URL}ChungLoaiVatTu/CheckChungLoaiVT?MaCLVT={row["MaCLVT"]}";
                    string jsonCheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlCheck)).Result;

                    if (jsonCheck != "[]")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                            "Chủng loại vật tư này đã được sử dụng. Không thể xóa!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        continue;
                    }

                    if (row["ID"].ToString() != "0")
                    {
                        string url = $"{URL}ChungLoaiVatTu/Delete?id={row["ID"]}";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                        }
                    }

                    grvChungLoai.DeleteRow(rowHandle);
                }

             
                grcChungLoai.DataSource = GetSortedDataTableFromGrid(grcChungLoai);
                grcChungLoai.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_CL_Click(object sender, EventArgs e)
        {
            try
            {
                grvChungLoai.CloseEditor();
                grvChungLoai.UpdateCurrentRow();
                int Sort = 0;
                this.ActiveControl = button1;
                DataTable tblGrid = grcChungLoai.DataSource as DataTable;
                if (tblGrid == null || tblGrid.Rows.Count == 0)
                {
                    return;
                }
                DataTable sortedTable = tblGrid.AsEnumerable()
                 .OrderBy(x => x["Sort"] == null || string.IsNullOrEmpty(x["Sort"].ToString())
                     ? 0
                     : Convert.ToInt64(x["Sort"]))
                 .CopyToDataTable();
                DataTable _dtSave = new DataTable("tblSave");
                _dtSave.Columns.Add("ID", typeof(int));
                _dtSave.Columns.Add("MaCLVT", typeof(string));
                _dtSave.Columns.Add("ChungLoaiVatTu", typeof(string));
                _dtSave.Columns.Add("VietTat", typeof(string));
                _dtSave.Columns.Add("IsNPL", typeof(string));
                _dtSave.Columns.Add("Sort", typeof(int));
                _dtSave.Columns.Add("TenKhac", typeof(string));
                _dtSave.Columns.Add("UserName", typeof(string));
                // List tạm để lưu tên đã add (kiểm tra trùng ngay khi add)
                List<string> existingNames = new List<string>();

                foreach (DataRow row in sortedTable.Rows)
                {
                    string tenCL = row["ChungLoaiVatTu"].ToString().Trim();
                    string VietTat = row["VietTat"].ToString().Trim();
                    if (string.IsNullOrEmpty(tenCL))
                    {
                        MessageBox.Show("Chủng loại vật tư không được bỏ trống! Vui lòng kiểm tra và nhập đầy đủ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrEmpty(VietTat))
                    {
                        MessageBox.Show("Viết tắt chủng loại vật tư không được bỏ trống! Vui lòng kiểm tra và nhập đầy đủ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (existingNames.Any(name => string.Equals(name, tenCL, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"Tên chủng loại '{tenCL}' đã tồn tại! Vui lòng kiểm tra và sửa lại.", "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    existingNames.Add(tenCL);
                    Sort++;

                    if (!row["IsNPL"].Equals(row["IsNPL_Goc"]) && !string.IsNullOrEmpty(row["IsNPL_Goc"]?.ToString()))
                    {
                        ShowWarning($"Khi thay đổi loại chủng loại {tenCL} .Vui lòng nhập lại đúng định dạng khổ/size theo loại vật tư đó");
                        return;
                    }

                    DataRow dr = _dtSave.NewRow();
                    dr["ID"] = row["ID"];
                    dr["MaCLVT"] = row["MaCLVT"];
                    dr["ChungLoaiVatTu"] = tenCL;
                    dr["VietTat"] = VietTat;
                    dr["IsNPL"] = row["IsNPL"];
                    dr["Sort"] = Sort;
                    dr["TenKhac"] = row["TenKhac"];
                    dr["UserName"] = GlobleData.UserName;
                    _dtSave.Rows.Add(dr);
                }

                if (_dtSave == null || _dtSave.Rows.Count == 0)
                {
                    return;
                }

                var rowsWithIssue = _dtSave.AsEnumerable()
                                       .Where(row => row.IsNull("IsNPL") || row["IsNPL"].ToString() == "")
                                       .ToList();

                if (rowsWithIssue.Any())
                {
                    MessageBox.Show("Nguyên phụ liệu không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ChungLoaiVatTu");

                string url = string.Format("{0}", URL + "ChungLoaiVatTu/Post");
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    _status = ResourceURL.EventStatus.View;
                    GridViewUpdateStatus(_status);
                    LoadDS();
                 
                    if (!string.IsNullOrEmpty(clvt) && _dtSave?.Rows?.Count > 0)
                    {
                        string[] arrCLVT = clvt.Split('|');
                        if (arrCLVT.Length > 0)
                        {
                            foreach (string item in arrCLVT)
                            {
                                bool HasItemCL = _dtSave.AsEnumerable().Any(x => x["ChungLoaiVatTu"]?.ToString() == item);
                                if (!HasItemCL)
                                {
                                    Result = false;
                                    break;
                                }
                            }
                        }

                    }
                    this.DialogResult = DialogResult.OK;
                    //return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void grvChungLoai_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
                GridView view = sender as GridView;
                if (view == null) return;

                int rowHandle = view.FocusedRowHandle;
                if (rowHandle < 0) return;

                GridColumn col_focused = view.FocusedColumn;
                if (col_focused == null) return;

                if (col_focused == colTenChungLoai || col_focused == colVietTat)
                {
                    string newValue = e.Value?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(newValue))
                    {
                        e.Valid = false;
                        e.ErrorText = $"{col_focused.Caption} không được bỏ trống!";
                        return;
                    }

                 
                    DataTable table = view.DataSource as DataTable;
                    if (table != null)
                    {
                        bool exists = table.AsEnumerable()
                            .Where(r =>
                            {
                                int rowIndex = table.Rows.IndexOf(r);
                                return view.GetRowHandle(rowIndex) != rowHandle;
                            })
                            .Any(r =>
                            {
                                string existingValue = r[col_focused.FieldName]?.ToString()?.Trim();
                                return !string.IsNullOrEmpty(existingValue) &&
                                       string.Equals(existingValue, newValue, StringComparison.OrdinalIgnoreCase);
                            });

                        if (exists)
                        {
                            e.Valid = false;
                            e.ErrorText = $"{col_focused.Caption} {newValue} đã tồn tại! Vui lòng nhập chủng loại khác.";
                            return;
                        }
                    }

                    e.Valid = true;
                    e.ErrorText = string.Empty;
            }
        }

        #endregion

        #region Chủng Loại Chi Tiết

        private void LoadChungLoaiCT(string MaChungLoai)
        {
            try
            {
                string url = $"{URL}NhomNPL/GetChung?Action=GET&para={MaChungLoai}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    grcChungLoaiCT.DataSource = null;
                    
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                grcChungLoaiCT.DataSource = tbl;
                grcChungLoaiCT.RefreshDataSource();
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {

            }
        }

  
      
        private void btnSortDown_CT_Click(object sender, EventArgs e)
        {
            int direction = 1;
            GridView view = grvChungLoaiCT;
            int index = view.FocusedRowHandle;
            if (index < 0) return;

            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= view.RowCount) return;

            DataTable dt = grcChungLoaiCT.DataSource as DataTable;
            if (dt == null) return;

            DataRow currentRow = view.GetDataRow(index);
            DataRow targetRow = view.GetDataRow(newIndex);

            int currentSort = Convert.ToInt32(currentRow["Sort"]);
            int targetSort = Convert.ToInt32(targetRow["Sort"]);
            currentRow["Sort"] = targetSort;
            targetRow["Sort"] = currentSort;

            object[] currentValues = currentRow.ItemArray;
            object[] targetValues = targetRow.ItemArray;

            dt.Rows[index].ItemArray = targetValues;
            dt.Rows[newIndex].ItemArray = currentValues;

            grcChungLoaiCT.DataSource = dt;
            grvChungLoaiCT.FocusedRowHandle = newIndex;
        }


        private void btnSortUp_CT_Click(object sender, EventArgs e)
        {
            int direction = -1;
            GridView view = grvChungLoaiCT;
            int index = view.FocusedRowHandle;
            if (index < 0) return;

            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= view.RowCount) return;

            DataTable dt = grcChungLoaiCT.DataSource as DataTable;
            if (dt == null) return;

            DataRow currentRow = view.GetDataRow(index);
            DataRow targetRow = view.GetDataRow(newIndex);

            int currentSort = Convert.ToInt32(currentRow["Sort"]);
            int targetSort = Convert.ToInt32(targetRow["Sort"]);
            currentRow["Sort"] = targetSort;
            targetRow["Sort"] = currentSort;

            object[] currentValues = currentRow.ItemArray;
            object[] targetValues = targetRow.ItemArray;

            dt.Rows[index].ItemArray = targetValues;
            dt.Rows[newIndex].ItemArray = currentValues;

            grcChungLoaiCT.DataSource = dt;
            grvChungLoaiCT.FocusedRowHandle = newIndex;
        }

        private void btnAdd_CT_Click(object sender, EventArgs e)
        {
     
            DataTable tblGC = grcChungLoaiCT.DataSource as DataTable;
            DataRow dr;
            if (tblGC == null)
            {
                tblGC = new DataTable();
                tblGC.Columns.Add("ID", typeof(int));
                tblGC.Columns.Add("MaNhom", typeof(string));
                tblGC.Columns.Add("TenNhom", typeof(string));
                tblGC.Columns.Add("LoaiVT", typeof(string));
                tblGC.Columns.Add("MaCLVT", typeof(string));
                tblGC.Columns.Add("NPL", typeof(string));
                tblGC.Columns.Add("TenCLVT", typeof(string));
                tblGC.Columns.Add("TenTA", typeof(string));
                tblGC.Columns.Add("VietTat", typeof(string));
                tblGC.Columns.Add("Sort", typeof(int));
                tblGC.Columns.Add("SortGroup", typeof(int));
            }

            dr = tblGC.NewRow();
            DataRow rowFocuse_CL = grvChungLoai.GetFocusedDataRow();
            if(rowFocuse_CL== null)
            {
                MessageBox.Show("Vui lòng nhập chủng loại trước mới thêm chi tiết", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        
            string maCLVT = rowFocuse_CL["MaCLVT"].ToString();
            if(string.IsNullOrEmpty(maCLVT))
            {
                MessageBox.Show("Vui lòng nhập chủng loại trước mới thêm chi tiết", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //bool existed = tblGC.AsEnumerable().Any(r =>
            //    string.Equals(r.Field<string>("MaCLVT"), maCLVT, StringComparison.OrdinalIgnoreCase) &&
            //    string.Equals(r.Field<string>("TenNhom"), tenNhomMoi, StringComparison.OrdinalIgnoreCase));
            //if (existed)
            //{
            //    MessageBox.Show("Trùng chủng loại. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}


            int maxSort = tblGC.AsEnumerable()
                .Where(r => r.Field<string>("MaCLVT") == maCLVT && !r.IsNull("Sort"))
                .Select(r => Convert.ToInt32(r["Sort"]))
                .DefaultIfEmpty(0)
                .Max();
            dr["MaCLVT"] = maCLVT;
            dr["TenCLVT"] = "";
            dr["SortGroup"] = 0;
            dr["NPL"] = rowFocuse_CL["IsNPL"];
            dr["LoaiVT"] = rowFocuse_CL["LoaiVT"].ToString();
            dr["TenNhom"] = "";
            dr["VietTat"] = "";
            dr["Sort"] = maxSort + 1;
            tblGC.Rows.Add(dr);
            grcChungLoaiCT.DataSource = tblGC;
            
        }

        private void btnDelete_CT_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGC = grcChungLoaiCT.DataSource as DataTable;
                if (tblGC == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvChungLoaiCT.GetSelectedRows();

                // Duyệt ngược để tránh lỗi handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvChungLoaiCT.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    string urlGET = $"{URL}NhomNPL/GetChung?Action=GETCHECK&para={dr["MaNhom"]}";
                    string jsonGET = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;

                    if (jsonGET != "[]")
                    {
                        XtraMessageBox.Show(
                            "Màu này đang được sử dụng. Không thể xóa!!!",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        continue;
                    }

                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}NhomNPL/Delete?id={dr["ID"]}&user={GlobleData.UserName}";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }
                    grvChungLoaiCT.DeleteRow(rowHandle);
                    
                }
                //string MaChungLoai = grvChungLoai.GetFocusedRowCellValue(colMaCL)?.ToString();
                //LoadChungLoaiCT(MaChungLoai);
                grcChungLoaiCT.DataSource = GetSortedDataTableFromGrid(grcChungLoaiCT);
                grcChungLoaiCT.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataTable GetSortedDataTableFromGrid(GridControl grc)
        {
            DataTable sortedTable = null;
            DataView dataView = grc.DataSource as DataView;
            if (dataView != null)
            {
               
                DataTable tblGC = dataView.Table;

               
                sortedTable = tblGC.Clone();

              
                var sortedRows = tblGC.AsEnumerable()
                    .OrderBy(x => Convert.ToInt64(x["Sort"]?.ToString() ?? "0"));
                int sort = 0;
                foreach (DataRow row in sortedRows)
                {
                    sort++;
                    row["Sort"] = sort;
                    sortedTable.ImportRow(row);
                }
            }
            else
            {
               
                DataTable tblGC = grc.DataSource as DataTable;
                if (tblGC != null)
                {
                    int sort = 0;
                    sortedTable = tblGC.Clone();
                    var sortedRows = tblGC.AsEnumerable()
                        .OrderBy(x => Convert.ToInt64(x["Sort"]?.ToString() ?? "0"));
                    foreach (DataRow row in sortedRows)
                    {
                        sort++;
                        row["Sort"] = sort;
                        sortedTable.ImportRow(row);
                    }
                }
            }
            return sortedTable;
        }

        private void btnSave_CT_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGC = GetSortedDataTableFromGrid(grcChungLoaiCT);

                if (tblGC == null || tblGC.Rows.Count == 0) return;

                if (tblChungLoai_CT != null)
                {
                    tblChungLoai_CT.Clear();
                }
                DataTable sortedTable = tblGC.AsEnumerable()
               .OrderBy(x => x["Sort"] == null || string.IsNullOrEmpty(x["Sort"].ToString())
                   ? 0
                   : Convert.ToInt64(x["Sort"]))
               .CopyToDataTable();
                List<string> existingNames = new List<string>();
                int sort = 0;
                foreach (DataRow dr in sortedTable.Rows)
                {
                    string tenNhom = dr["TenNhom"].ToString().Trim();
                    if (string.IsNullOrEmpty(tenNhom))
                    {
                        MessageBox.Show("Tên chủng loại chi tiết không được bỏ trống! Vui lòng kiểm tra và nhập đầy đủ.",
                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (existingNames.Any(name => string.Equals(name, tenNhom, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"Tên chủng loại chi tiết '{tenNhom}' đã tồn tại! Vui lòng kiểm tra và sửa lại.",
                            "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    existingNames.Add(tenNhom);
                    sort++;
                    DataRow nr = tblChungLoai_CT.NewRow();
                    nr["ID"] = dr["ID"];
                    nr["MaNhom"] = dr["MaNhom"];
                    nr["TenNhom"] = tenNhom;
                    nr["NPL"] = dr["NPL"];
                    nr["TenTA"] = dr["TenTA"];
                    nr["Sort"] = sort;
                    nr["VietTat"] = dr["VietTat"];
                    nr["MaCLVT"] = dr["MaCLVT"];
                    tblChungLoai_CT.Rows.Add(nr);
                }

                string url = $"{URL}NhomNPL/Post?para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblChungLoai_CT); }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    string MaChungLoai = grvChungLoai.GetFocusedRowCellValue(colMaCL)?.ToString();
                    LoadChungLoaiCT(MaChungLoai);
                }
            }
            catch (Exception ex)
            {
                // Nên log exception thay vì bỏ qua
                // Ví dụ: Console.WriteLine(ex.Message);
            }
        }

        private void grvChungLoaiCT_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            int rowHandle = view.FocusedRowHandle;
            if (rowHandle < 0) return;

            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;

            if (col_focused != colChungLoai_CT) return;

            string newValue = e.Value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(newValue))
            {
                e.Valid = false;
                e.ErrorText = "Chủng loại chi tiết không được bỏ trống!";
                return;
            }
       
            DataTable table = view.DataSource as DataTable;
            if (table != null)
            {
                bool exists = table.AsEnumerable()
                    .Where(r =>
                    {
                        int rowIndex = table.Rows.IndexOf(r);
                        return view.GetRowHandle(rowIndex) != rowHandle;
                    })
                    .Any(r =>
                    {
                        string existingValue = r[col_focused.FieldName]?.ToString()?.Trim();
                        return !string.IsNullOrEmpty(existingValue) &&
                               string.Equals(existingValue, newValue, StringComparison.OrdinalIgnoreCase);
                    });

                if (exists)
                {
                    e.Valid = false;
                    e.ErrorText = $"Chủng loại chi tiết {newValue} đã tồn tại! Vui lòng nhập chủng loại chi tiết khác.";
                    return;
                }
            }

            e.Valid = true;
            e.ErrorText = string.Empty;
        }

        #endregion

        #region Thẻ Khổ Size
        private void LoadTheKhoVai(string MaChungLoai)
        {
            try
            {
              
                string url = $"{URL}ERPThuVienVT/GetChungKV?Action=GetTheKV&para={MaChungLoai}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    grcTheKhoVai.DataSource = null;
                    grcKhoVai.DataSource = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                grcTheKhoVai.DataSource = tbl;

                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {
                // Xử lý exception (ví dụ: log hoặc message)
                MessageBox.Show($"Lỗi khi load khổ vải: {ex.Message}");
            }
        }

        private void grvTheKhoVai_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (grvChungLoai.FocusedRowHandle >= 0 && grvTheKhoVai.FocusedRowHandle >=0)
            {
                string MaChungLoai = view.GetFocusedRowCellValue(colMaCL)?.ToString();
                string MaTheKV = view.GetFocusedRowCellValue(colMaTheKV)?.ToString();
                LoadKhoVai(MaChungLoai, MaTheKV);
            }
        }
        private void grvTheKhoVai_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            int rowHandle = view.FocusedRowHandle;
            if (rowHandle < 0) return;

            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;

            if (col_focused == colTheKhoVai)
            {
                string newValue = e.Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(newValue))
                {
                    e.Valid = false;
                    e.ErrorText = $"{col_focused.Caption} không được bỏ trống!";
                    return;
                }

             
                DataTable table = view.DataSource as DataTable;
                if (table != null)
                {
                    bool exists = table.AsEnumerable()
                        .Where(r =>
                        {
                            int rowIndex = table.Rows.IndexOf(r);
                            return view.GetRowHandle(rowIndex) != rowHandle;
                        })
                        .Any(r =>
                        {
                            string existingValue = r[col_focused.FieldName]?.ToString()?.Trim();
                            return !string.IsNullOrEmpty(existingValue) &&
                                   string.Equals(existingValue, newValue, StringComparison.OrdinalIgnoreCase);
                        });

                    if (exists)
                    {
                        e.Valid = false;
                        e.ErrorText = $"{col_focused.Caption} {newValue} đã tồn tại! Vui lòng nhập thẻ khổ vải khác.";
                        return;
                    }
                }

                e.Valid = true;
                e.ErrorText = string.Empty;
            }
        }


        private void grvTheKhoVai_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (grvChungLoai.FocusedRowHandle >= 0)
            {
                string MaChungLoai = view.GetFocusedRowCellValue(colMaCL)?.ToString();
                string MaTheKV = view.GetFocusedRowCellValue(colMaTheKV)?.ToString();
                LoadKhoVai(MaChungLoai, MaTheKV);
            }
        }
        private void btnAddTheKV_Click(object sender, EventArgs e)
        {
            DataTable tblGC = grcTheKhoVai.DataSource as DataTable;
            DataRow rowChungLoai_focuse = grvChungLoai.GetFocusedDataRow();
            if (rowChungLoai_focuse == null)
            {
                MessageBox.Show("Vui lòng nhập chủng loại chi tiết trước mới thêm thẻ khổ size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maCLVT = rowChungLoai_focuse["MaCLVT"].ToString();
            if (string.IsNullOrEmpty(maCLVT))
            {
                MessageBox.Show("Vui lòng nhập chủng loại trước mới thêm khổ size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
            DataRow dr;
            if (tblGC == null)
            {
                tblGC = new DataTable();
                tblGC.Columns.Add("ID", typeof(int));
                tblGC.Columns.Add("MaTheKhoVai", typeof(string));
                tblGC.Columns.Add("TheKhoVai", typeof(string));
                tblGC.Columns.Add("MaCLVT", typeof(string));
                
            }

            dr = tblGC.NewRow();
            dr["ID"] = 0;
            dr["MaCLVT"] = maCLVT;
          

            tblGC.Rows.Add(dr);
            grcTheKhoVai.DataSource = tblGC;
        }
        private void btnSaveKV_Click(object sender, EventArgs e)
        {
            try
            {
                grvTheKhoVai.CloseEditor();
                grvTheKhoVai.UpdateCurrentRow();

                this.ActiveControl = button1;
                DataTable tblGC = grcTheKhoVai.DataSource as DataTable;
                if (tblGC == null || tblGC.Rows.Count == 0) return;

                if (tblTheKhoVai != null)
                {
                    tblTheKhoVai.Clear();
                }

                // List tạm để lưu key ghép đã add (kiểm tra trùng ngay khi add)
                List<string> existingKeys = new List<string>();
              
                foreach (DataRow dr in tblGC.Rows)
                {
                    string TheKhoVai = dr["TheKhoVai"].ToString().Trim();
                  

                    if (string.IsNullOrEmpty(TheKhoVai))
                    {
                        MessageBox.Show("Thẻ Khổ/size không được bỏ trống! Vui lòng kiểm tra và nhập đầy đủ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Dừng ngay, không save
                    }

               

                    string compositeKey = $"{TheKhoVai}";

                    if (existingKeys.Any(key => string.Equals(key, compositeKey, StringComparison.OrdinalIgnoreCase)))
                    {
                    
                        MessageBox.Show($" Thẻ khổ /size {TheKhoVai}  đã tồn tại! Vui lòng kiểm tra và sửa lại.", "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    existingKeys.Add(compositeKey);

                    DataRow nr = tblTheKhoVai.NewRow();
                    nr["ID"] = dr["ID"];
                    nr["MaTheKhoVai"] = dr["MaTheKhoVai"];
                    nr["TheKhoVai"] = TheKhoVai;                 
                    nr["MaCLVT"] = dr["MaCLVT"];                 
                    nr["UserName"] = GlobleData.UserName;
                    nr["CreateDate"] = DateTime.Now;
                    tblTheKhoVai.Rows.Add(nr);
                }

                string url = $"{URL}ERPThuVienVT/PostKV?action=PostTheKV";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblTheKhoVai); }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    string MaChungLoai = grvChungLoai.GetFocusedRowCellValue(colMaCL)?.ToString();                  
                    LoadTheKhoVai(MaChungLoai);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnDeleteTheKV_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGC = grcTheKhoVai.DataSource as DataTable;
                if (tblGC == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvTheKhoVai.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvTheKhoVai.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    //Nếu cần kiểm tra dữ liệu dùng lại, bạn bật đoạn này lên

                    string urlGET = $"{URL}ERPThuVienVT/GetChungKV?Action=GetCheckTheKV&para={dr["MaTheKhoVai"]}";
                    string jsonGET = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;
                    if (jsonGET != "[]")
                    {
                        XtraMessageBox.Show("Thẻ Khổ/size này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }


                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}ERPThuVienVT/DeleteKV?id={dr["MaTheKhoVai"]}&action=DeleteTheKV&UserName={GlobleData.UserName}";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }

                    // Xóa dòng trong DataTable
                    tblGC.Rows.Remove(dr);
                }

                grcTheKhoVai.DataSource = tblGC;
                grcTheKhoVai.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion

        #region Khổ Vải (Size)

        private void SetColumnKhoVai(string IsNPL)
        {
            if (IsNPL?.ToLower() == "true")
            {
                colKhoSize.ColumnEdit = rItemTextEdit_QDMet;
                colQDMet.Visible = true;
                colDonViTinh.Visible = true;
            }
            else if (IsNPL?.ToLower() == "false")
            {
                colKhoSize.ColumnEdit = repositoryItemTextEdit3;
                colQDMet.Visible = false;
                colDonViTinh.Visible = false;
            }
        }
        private void SetColDonVi()
        {
             lstDonViVTEntity = new List<ERPDonViVTEntity>();
            string url = string.Format("{0}?", URL + "ERPThuVienVT/GetDVVT");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstDonViVTEntity = JsonConvert.DeserializeObject<List<ERPDonViVTEntity>>(json);
            }
            RepositoryItemSearchLookUpEdit rDonViEdit = new RepositoryItemSearchLookUpEdit();
            rDonViEdit.DataSource = lstDonViVTEntity;
            rDonViEdit.DisplayMember = "TenDVVT";
            rDonViEdit.ValueMember = "MaDVVT";
            rDonViEdit.ShowClearButton = false;
            rDonViEdit.NullText = "[Chọn ĐV Vật Tư]";
            rDonViEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rDonViEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rDonViEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaDVVT", Caption = "MaDVVT", Name = "rColMaDVVT", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDVVT", Caption = "Đơn Vị Tính", Name = "rColDV", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "Tile", Caption = "QĐ Mét", Name = "rColQDMet", Visible = true });
            }
            //repositoryItemTextEdit3

            rDonViEdit.EditValueChanged += RDonViEdit_EditValueChanged;
            colDonViTinh.ColumnEdit = rDonViEdit;
            //gridDonViVT.DataSource = lstDonViVTEntity.OrderBy(x => x.TenDVVT).ToList();
        }

        private void RDonViEdit_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;

           
            GridView gridView = grcKhoVai.MainView as GridView; // Hoặc cách lấy GridView tương ứng

            int rowHandle = gridView.FocusedRowHandle;
            //if (!gridView.IsValidRowHandle(rowHandle)) return;

          
            var khoVaiValue = gridView.GetRowCellValue(rowHandle, "KhoVai");
            if (khoVaiValue == null || string.IsNullOrEmpty(khoVaiValue.ToString())) return;

         
            if (float.TryParse(khoVaiValue.ToString(), out float khoVai))
            {
                // Lấy object đầy đủ (selected row) từ SearchLookUpEdit
                var selectedEntity = edit.GetSelectedDataRow() as ERPDonViVTEntity;
                if (selectedEntity != null)
                {
                    float tile = selectedEntity.Tile;
                    float QDMet = khoVai * tile;

                
                    gridView.SetRowCellValue(rowHandle, colQDMet, Math.Round(QDMet,4));
                }
            }
        }

        private void btnDonViTinh_Click(object sender, EventArgs e)
        {
            frmERPDonViVT frm = new frmERPDonViVT(true);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            SetColDonVi();
        }
        private void grvKhoVai_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var gridView = sender as GridView;
            if (gridView == null || !gridView.IsValidRowHandle(e.RowHandle)) return;

           
            if (!float.TryParse(e.Value?.ToString(), out float khoVai)) return;

       
            var donViValue = gridView.GetRowCellValue(e.RowHandle, colDonViTinh);
            if (donViValue == null || string.IsNullOrEmpty(donViValue.ToString())) return;

          
            var selectedEntity = lstDonViVTEntity.FirstOrDefault(x => x.MaDVVT == donViValue.ToString());
            if (selectedEntity != null)
            {
                float tile = selectedEntity.Tile;
                float ketQua = khoVai * tile;

                         
                gridView.SetRowCellValue(e.RowHandle, colQDMet, Math.Round(ketQua, 4));
            }
        }
        private void LoadKhoVai(string MaChungLoai,string MaTheKV)
        {
            try
            {
                SetColDonVi();
                string url = $"{URL}ERPThuVienVT/GetChungKV?Action=GETKV&para={MaChungLoai}&para1={MaTheKV}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    grcKhoVai.DataSource = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);  
                // Duyệt tất cả rows để tính KhoVaiMet = Khovai * Tile (nếu Khovai là số)
                //foreach (DataRow row in tbl.Rows)
                //{
                //    var khoVaiValue = row["Khovai"];
                //    if (khoVaiValue == null || string.IsNullOrEmpty(khoVaiValue.ToString()) || !float.TryParse(khoVaiValue.ToString(), out float khoVai))
                //    {
                //        row["KhoVaiMet"] = 0m;
                //        continue;
                //    }

                  
                //    var donViValue = row["MaDVVT"];
                //    if (donViValue == null || string.IsNullOrEmpty(donViValue.ToString()))
                //    {
                //        row["KhoVaiMet"] = 0m;
                //        continue;
                //    }

                //    string donVi = donViValue.ToString();
                 
                //    var selectedEntity = lstDonViVTEntity.FirstOrDefault(x => x.MaDVVT == donVi);
                //    if (selectedEntity != null )
                //    {
                //        float tile = selectedEntity.Tile;
                //        //float ketQua = khoVai * tile;
                //        row["KhoVaiMet"] = Math.Round((khoVai * tile), 4);
                //    }
                //    else
                //    {
                //        row["KhoVaiMet"] = 0m;
                //    }
                //}

                grcKhoVai.DataSource = tbl;
              
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {
                // Xử lý exception (ví dụ: log hoặc message)
                MessageBox.Show($"Lỗi khi load khổ vải: {ex.Message}");
            }
        }
        private void btnAddKhoSize_Click(object sender, EventArgs e)
        {
       
            DataTable tblGC = grcKhoVai.DataSource as DataTable;
            DataRow rowChungLoai_focuse = grvChungLoai.GetFocusedDataRow();
            if (rowChungLoai_focuse == null)
            {
                MessageBox.Show("Vui lòng nhập chủng loạitrước mới thêm thẻ khổ/size và khổ/size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maCLVT = rowChungLoai_focuse["MaCLVT"].ToString();
            if (string.IsNullOrEmpty(maCLVT))
            {
                MessageBox.Show("Vui lòng nhập chủng loại trước mới thêm thẻ khổ/size và khổ/size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRow rowTheKV_focuse = grvTheKhoVai.GetFocusedDataRow();
            if (rowTheKV_focuse == null)
            {
                MessageBox.Show("Vui lòng nhập thẻ khổ/size trước mới thêm khổ size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string MaTheKV = rowTheKV_focuse["MaTheKhoVai"].ToString();
            if (string.IsNullOrEmpty(MaTheKV))
            {
                MessageBox.Show("Vui lòng nhập thẻ khổ/size trước mới thêm khổ size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            DataRow dr;
            if (tblGC == null)
            {
                tblGC = new DataTable("tblKhoVai");
                tblGC.Columns.Add("ID", typeof(int));
                tblGC.Columns.Add("KhoVaiID", typeof(string));
                tblGC.Columns.Add("KhoVai", typeof(string));
                tblGC.Columns.Add("NPL", typeof(bool));
                tblGC.Columns.Add("MaNhom", typeof(string));
                tblGC.Columns.Add("GhiChu", typeof(string));
                tblGC.Columns.Add("MaDVVT", typeof(string));
                tblGC.Columns.Add("KhoVaiMet", typeof(decimal));
                tblGC.Columns.Add("LoaiVT", typeof(string));
                tblGC.Columns.Add("TenNhom", typeof(string));
                tblGC.Columns.Add("MaTheKhoVai", typeof(string));
            }

            dr = tblGC.NewRow();
            dr["ID"] = 0;
            dr["MaNhom"] = maCLVT;
            dr["TenNhom"] = rowChungLoai_focuse["ChungLoaiVatTu"];
            dr["NPL"] = rowChungLoai_focuse["IsNPL"];
            dr["MaTheKhoVai"] = rowTheKV_focuse["MaTheKhoVai"];
            tblGC.Rows.InsertAt(dr,0);
            grcKhoVai.DataSource = tblGC;
            grcKhoVai.RefreshDataSource();


        }
        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }
            if (currentText.Contains("."))
            {
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);
                if (decimalPart.Length >= 4 && e.KeyChar != '\b') // '\b' là phím Backspace
                {
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
            }
        }
        private void rItemTextEdit_QDMet_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void btnDeleteTheSize_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGC = grcKhoVai.DataSource as DataTable;
                if (tblGC == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvKhoVai.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvKhoVai.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    //Nếu cần kiểm tra dữ liệu dùng lại, bạn bật đoạn này lên

                    string urlGET = $"{URL}ERPThuVienVT/GetChungKV?Action=GetCheckKV&para={dr["KhoVaiID"]}";
                    string jsonGET = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;
                    if (jsonGET != "[]")
                    {
                        XtraMessageBox.Show("Khổ vải này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }


                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}ERPThuVienVT/DeleteKV?id={dr["ID"]}&action=Delete&UserName={GlobleData.UserName}";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }

                    // Xóa dòng trong DataTable
                    tblGC.Rows.Remove(dr);
                }

                grcKhoVai.DataSource = tblGC;
                grcKhoVai.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnSaveTheSize_Click(object sender, EventArgs e)
        {
            try
            {
                grvKhoVai.CloseEditor();
                grvKhoVai.UpdateCurrentRow();
                var errors = new List<string>();
                this.ActiveControl = button1;
                DataTable tblGC = grcKhoVai.DataSource as DataTable;
                if (tblGC == null || tblGC.Rows.Count == 0) return;

                if (tblKhoVai != null)
                {
                    tblKhoVai.Clear();
                }

                // List tạm để lưu key ghép đã add (kiểm tra trùng ngay khi add)
                List<string> existingKeys = new List<string>();
                string IsNPL = grvChungLoai.GetFocusedRowCellValue(colIsNPL)?.ToString();
                foreach (DataRow dr in tblGC.Rows)
                {
                    string khoVai = dr["KhoVai"].ToString().Trim();
                    string maDVVT = dr["MaDVVT"].ToString().Trim();

                    if (string.IsNullOrEmpty(khoVai) )
                    {
                        MessageBox.Show("Khổ/size không được bỏ trống! Vui lòng kiểm tra và nhập đầy đủ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Dừng ngay, không save
                    }

                    if (string.IsNullOrEmpty(maDVVT) && IsNPL?.ToLower() == "true")
                    {
                        MessageBox.Show("Đơn vị không được bỏ trống! Vui lòng kiểm tra và nhập đầy đủ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Dừng ngay, không save
                    }

                 
                    string compositeKey = string.Empty;
                    if(IsNPL?.ToLower() == "true")
                    {
                        compositeKey = $"{maDVVT}|{khoVai}";
                    }
                    else
                    {
                        maDVVT = "DVVT_10";
                        compositeKey = khoVai;
                    }

                    if (existingKeys.Any(key => string.Equals(key, compositeKey, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Lấy display text cho đơn vị
                        string donViDisplay = lstDonViVTEntity.FirstOrDefault(x => x.MaDVVT == maDVVT)?.TenDVVT ?? maDVVT;
                        MessageBox.Show($" Khổ /size  {khoVai} {(maDVVT == "DVVT_10" ? "" : donViDisplay)} đã tồn tại! Vui lòng kiểm tra và sửa lại.", "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; 
                    }

                  
                    existingKeys.Add(compositeKey);

                    DataRow nr = tblKhoVai.NewRow();
                    nr["ID"] = dr["ID"];
                    nr["KhoVaiID"] = dr["KhoVaiID"];
                    nr["KhoVai"] = khoVai;
                    nr["NPL"] = dr["NPL"];
                    nr["MaNhom"] = dr["MaNhom"];
                    nr["GhiChu"] = dr["GhiChu"];
                    nr["MaDVVT"] = maDVVT;
                    nr["KhoVaiMet"] = dr["KhoVaiMet"];
                    nr["MaTheKhoVai"] = dr["MaTheKhoVai"];
                    tblKhoVai.Rows.Add(nr);
                }

                DataTable tblGridChungLoaiVT = grcChungLoai.DataSource as DataTable;
            
                if (tblGridChungLoaiVT != null && tblGridChungLoaiVT?.Rows?.Count > 0)
                {
                    var Query = tblGridChungLoaiVT.AsEnumerable().FirstOrDefault(row => !row["IsNPL"].Equals(row["IsNPL_Goc"]) && !string.IsNullOrEmpty(row["IsNPL_Goc"]?.ToString()));
                   
                    if (Query != null)
                    {
                        string tenCL = Query["ChungLoaiVatTu"]?.ToString()?.Trim();
                        ShowWarning($"Khi thay đổi loại chủng loại {tenCL} .Vui lòng nhập lại đúng định dạng khổ/size theo loại vật tư đó");
                        return;
                    }

                    
                }

                string url = $"{URL}ERPThuVienVT/PostKV?action=Post";
                string urlCheck = $"{URL}ERPThuVienVT/Get?action=CheckTrungKV";
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(urlCheck, tblKhoVai); }).Result;
                if (json != "[]")
                {
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tbl?.Rows.Count > 0)
                    {
                        if (tbl.Rows.Count > 0)
                        {
                            foreach (DataRow row in tbl?.Rows)
                            {

                                bool.TryParse(row["Isval"]?.ToString(), out bool Isval);
                                if (Isval)
                                {
                                    //<color=blue>Dòng {row["SoDong"]}</color>
                                    errors.Add($"• {row["Msg"]?.ToString()}.\n");

                                }

                            }
                        }
                    }
                }
                if (errors.Count > 0)
                {
                    string htmlMessage = "<b><color=red> Dữ liệu hiện chưa lưu được vì:</color></b>\n\n" + string.Join("", errors);

                    ShowWarning(htmlMessage);
                    return; // Có lỗi
                }
            
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblKhoVai); }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    string MaChungLoai = grvChungLoai.GetFocusedRowCellValue(colMaCL)?.ToString();
                    string MaTheKV = grvTheKhoVai.GetFocusedRowCellValue(colMaTheKV)?.ToString();
                    LoadKhoVai(MaChungLoai, MaTheKV);
                }
            }
            catch (Exception ex)
            {

            }
        }
        void ShowWarning(string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs();
            args.Caption = Resources.Warning;
            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Text = $"{message}";
            args.Buttons = new DialogResult[] { DialogResult.OK };
            args.Icon = SystemIcons.Warning;
            args.MessageBeepSound = MessageBeepSound.Warning;
            XtraMessageBox.Show(args);
        }
        private void grvKhoVai_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            int rowHandle = view.FocusedRowHandle;
            if (rowHandle < 0) return;

            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;
            string IsNPL = grvChungLoai.GetFocusedRowCellValue(colIsNPL)?.ToString();
            if (col_focused != colDonViTinh && col_focused != colKhoSize) return;
            DataTable tblCheckKhoSize = grcKhoVai.DataSource as DataTable;
            if (tblCheckKhoSize == null && tblCheckKhoSize?.Rows?.Count == 0) return;
            string newCode = e.Value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(newCode) || string.IsNullOrWhiteSpace(newCode))
            {
              
                e.Valid = false;
                e.ErrorText = $"{(col_focused == colDonViTinh ? "Đơn vị" : "Khổ/size")} không được bỏ trống!";
                return;
            }

            string donViCode;
            string khoSizeCode;
            if (col_focused == colDonViTinh)
            {
                donViCode = newCode;  
                khoSizeCode = view.GetRowCellValue(rowHandle, colKhoSize)?.ToString()?.Trim() ?? string.Empty;
            }
            else
            {
                donViCode = view.GetRowCellValue(rowHandle, colDonViTinh)?.ToString()?.Trim() ?? string.Empty;
                khoSizeCode = newCode;
            }

            bool isDuplicate = false;
            string compositeKey = string.Empty;

            if (IsNPL != null && IsNPL.ToString().ToLower() == "true")
            {
                compositeKey = string.Format("{0}|{1}", donViCode, khoSizeCode);

                isDuplicate = tblCheckKhoSize.AsEnumerable()
                    .Any(row =>
                        row["KhoVai"] != null &&
                        row["MaDVVT"] != null &&
                        row["KhoVai"].ToString().Equals(khoSizeCode, StringComparison.OrdinalIgnoreCase) &&
                        row["MaDVVT"].ToString().Equals(donViCode, StringComparison.OrdinalIgnoreCase)
                    );
            }
            else
            {
                compositeKey = khoSizeCode;

                isDuplicate = tblCheckKhoSize.AsEnumerable()
                    .Any(row =>
                        row["KhoVai"] != null &&
                        row["KhoVai"].ToString().Equals(khoSizeCode, StringComparison.OrdinalIgnoreCase)
                    );
            }

            if (isDuplicate)
            {
                // Lấy display text cho thông báo (tách riêng logic cho từng cột)
                string donViDisplay = lstDonViVTEntity.FirstOrDefault(x => x.MaDVVT == donViCode)?.TenDVVT ?? donViCode;
                string khoSizeDisplay = khoSizeCode;  // Giả sử Khổ size dùng code làm display, nếu có list thì tương tự

                e.Valid = false;
                e.ErrorText = $"Khổ/size {khoSizeDisplay}{(donViDisplay == "DVVT_10" ? "" : donViDisplay)} đã tồn tại ở dòng khác! Vui lòng nhập lại.";
                return;
            }

            e.Valid = true;
            e.ErrorText = string.Empty;
        }
        #endregion
        private void gridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();


        }
      
        private void gV_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.RowHandle >= 0 && view?.RowCount > 0)
            {
                e.Appearance.ForeColor = ColorTranslator.FromHtml("#1A0000");
                e.HighPriority = true;
                if (view.FocusedRowHandle == e.RowHandle)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#FFFBE6");
                }


                //e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Bold);

            }

        }

        private void splitContainerControl_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl1.SplitterPosition = (int)(splitContainerControl1.Width * 0.6);

        }

        private void gV_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView != null && gridView.FocusedColumn != null)
            {
                string columnName = gridView.FocusedColumn.FieldName;
                if (columnName == "GhiChu" || columnName == "TenKhac")
                {
                    return;
                }

                if (Char.IsLetter(e.KeyChar))

                    e.KeyChar = Char.ToUpper(e.KeyChar);
            }
        }

        private void gC_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gV_KeyPress(grid.FocusedView, e);
        }

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
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
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
     
        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F1))
            {
                btnAddChungLoai.PerformClick();
                return true;
            }
            if (keyData == (Keys.F2))
            {
                btnAdd_CT.PerformClick();
                return true;
            }
            if (keyData == (Keys.F3))
            {
                btnAddKhoSize.PerformClick();
                return true;
            }
            //if (keyData == (Keys.Control | Keys.Delete))
            //{
            //    //btnClear.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.S))
            //{
            //    //btnLuu.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.A))
            //{
            //    //simpleButton1.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.T))
            //{
            //    //simpleButton2.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.Shift | Keys.E))
            //{
            //    //btnNhapExcel.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.I))
            //{
            //    //btnSelectImage.PerformClick();
            //    return true;
            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #region
        //private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        grvChungLoai.CloseEditor();
        //        grvChungLoai.UpdateCurrentRow();

        //        this.ActiveControl = button1;
        //        DataTable tblGrid = grcChungLoai.DataSource as DataTable;
        //        if (tblGrid == null || tblGrid.Rows.Count == 0)
        //        {
        //            return;
        //        }
        //        DataTable _dtSave = new DataTable("tblSave");
        //        _dtSave.Columns.Add("ID", typeof(int));
        //        _dtSave.Columns.Add("MaCLVT", typeof(string));
        //        _dtSave.Columns.Add("ChungLoaiVatTu", typeof(string));
        //        _dtSave.Columns.Add("VietTat", typeof(string));
        //        _dtSave.Columns.Add("IsNPL", typeof(string));
        //        _dtSave.Columns.Add("Sort", typeof(int));

        //        foreach (DataRow row in tblGrid.Rows)
        //        {
        //            if (row["ChungLoaiVatTu"].ToString() != "")
        //            {
        //                DataRow dr = _dtSave.NewRow();
        //                dr["ID"] = row["ID"];
        //                dr["MaCLVT"] = row["MaCLVT"];
        //                dr["ChungLoaiVatTu"] = row["ChungLoaiVatTu"];
        //                dr["VietTat"] = row["VietTat"];
        //                dr["IsNPL"] = row["IsNPL"];
        //                dr["Sort"] = row["Sort"];
        //                _dtSave.Rows.Add(dr);
        //            }

        //        }
        //        if (_dtSave == null || _dtSave.Rows.Count == 0)
        //        {
        //            return;
        //        }

        //        var rowsWithIssue = _dtSave.AsEnumerable()
        //                               .Where(row => row.IsNull("IsNPL") || row["IsNPL"].ToString() == "")
        //                               .ToList();

        //        if (rowsWithIssue.Any())
        //        {
        //            MessageBox.Show("Nguyên phụ liệu không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }

        //        clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ChungLoaiVatTu");

        //        string url = string.Format("{0}", URL + "ChungLoaiVatTu/Post");
        //        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
        //        if (msResult.ToLower() == "true")
        //        {
        //            clsWaitForm.ShowSuccessForm(this, 2000);
        //            _status = ResourceURL.EventStatus.View;
        //            GridViewUpdateStatus(_status);
        //            LoadDS();
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            DataTable dt = grcChungLoai.DataSource as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("MaCLVT", typeof(string));
                dt.Columns.Add("ChungLoaiVatTu", typeof(string));
                dt.Columns.Add("VietTat", typeof(string));
                dt.Columns.Add("IsNPL", typeof(string));
                dt.Columns.Add("Sort", typeof(int));
                dt.Columns.Add("TenKhac", typeof(string));
                //dt.Columns.Add("LoaiVT", typeof(string));
            }

            //DataRow newRow = dt.NewRow();
            //dt.Rows.InsertAt(newRow, 0);
            //gridChungLoai.DataSource = dt;
            //gridViewChungLoai.FocusedRowHandle = 0;

            int maxSort = dt.AsEnumerable()
                .Select(r => Convert.ToInt32(r["Sort"]))
                .DefaultIfEmpty(0)
                .Max();
            DataRow newRow = dt.NewRow();
            newRow["Sort"] = maxSort + 1;
            dt.Rows.InsertAt(newRow, 0);
            grcChungLoai.DataSource = dt;
            grvChungLoai.FocusedRowHandle = 0;
            if (grvChungLoai.VisibleColumns.Count > 0)
            {
                grvChungLoai.FocusedColumn = grvChungLoai.VisibleColumns[0];
                grvChungLoai.ShowEditor();
            }
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //_status = ResourceURL.EventStatus.Edit;
            //GridViewUpdateStatus(_status);
            //focused(this.grvChungLoai);
        }
        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return string.Join(" ", text.Trim().ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void Luu_ItemClick(object sender, EventArgs e)
        {
            try
            {
                grvChungLoai.CloseEditor();
                grvChungLoai.UpdateCurrentRow();

                this.ActiveControl = button1;
                DataTable tblGrid = grcChungLoai.DataSource as DataTable;
                if (tblGrid == null || tblGrid.Rows.Count == 0)
                {
                    return;
                }
                DataTable _dtSave = new DataTable("tblSave");
                _dtSave.Columns.Add("ID", typeof(int));
                _dtSave.Columns.Add("MaCLVT", typeof(string));
                _dtSave.Columns.Add("ChungLoaiVatTu", typeof(string));
                _dtSave.Columns.Add("VietTat", typeof(string));
                _dtSave.Columns.Add("IsNPL", typeof(string));
                _dtSave.Columns.Add("Sort", typeof(int));

                // List tạm để lưu tên đã add (kiểm tra trùng ngay khi add)
                List<string> existingNames = new List<string>();

                foreach (DataRow row in tblGrid.Rows)
                {
                    string tenCL = row["ChungLoaiVatTu"].ToString().Trim();
                    if (string.IsNullOrEmpty(tenCL))
                    {
                        MessageBox.Show("Chủng loại vật tư không được bỏ trống! Vui lòng kiểm tra và nhập đầy đủ.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    if (existingNames.Any(name => string.Equals(name, tenCL, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"Tên chủng loại '{tenCL}' đã tồn tại! Vui lòng kiểm tra và sửa lại.", "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    existingNames.Add(tenCL);

                    DataRow dr = _dtSave.NewRow();
                    dr["ID"] = row["ID"];
                    dr["MaCLVT"] = row["MaCLVT"];
                    dr["ChungLoaiVatTu"] = tenCL;
                    dr["VietTat"] = row["VietTat"];
                    dr["IsNPL"] = row["IsNPL"];
                    dr["Sort"] = row["Sort"];
                    _dtSave.Rows.Add(dr);
                }

                if (_dtSave == null || _dtSave.Rows.Count == 0)
                {
                    return;
                }

                var rowsWithIssue = _dtSave.AsEnumerable()
                                       .Where(row => row.IsNull("IsNPL") || row["IsNPL"].ToString() == "")
                                       .ToList();

                if (rowsWithIssue.Any())
                {
                    MessageBox.Show("Nguyên phụ liệu không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ChungLoaiVatTu");

                string url = string.Format("{0}", URL + "ChungLoaiVatTu/Post");
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    _status = ResourceURL.EventStatus.View;
                    GridViewUpdateStatus(_status);
                    LoadDS();
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }
        List<LogThuvienEntity> lstLogXoa = new List<LogThuvienEntity>();

      

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow row = grvChungLoai.GetDataRow(grvChungLoai.FocusedRowHandle);
                if (row == null) return;
                string urlCheck = $"{URL}ChungLoaiVatTu/CheckChungLoaiVT?MaCLVT={row["MaCLVT"]?.ToString()}";
                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                if (jsonCheck != "[]")
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chủng loại vật tư này đã được sử dụng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa dòng này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string content = clsWriteLogThuVienLib.FormatRow(row);
                    lstLogXoa.Clear();
                    lstLogXoa.Add(new LogThuvienEntity
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Action = $"Xóa {this.Text}",
                        Module = this.Name,
                        Content = content,
                        UserID = GlobleData.UserName,
                        CreatedDate = DateTime.Now

                    });
                    clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLogXoa, "ChungLoaiVatTu");
                    string url = string.Format("{0}?id={1}", URL + "ChungLoaiVatTu/Delete", row["ID"]);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        LoadDS();
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        /*Write log when modify row*/
        List<LogThuvienEntity> lstLog = new List<LogThuvienEntity>();
        #endregion
        private void grvChungLoai_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {

            object newValue = e.Value;
            DataRow dr = grvChungLoai.GetFocusedDataRow();
            if (dr == null) return;

            int isps = newValue.ToString() == "True" ? 1 : 0;
            string url = $"{URL}KhoiTaoBOMV1/Post1?Action=UpdateIsPS&para1={dr["ID"].ToString()}&para2={isps}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if(msResult=="True")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }    
        }
    }
}
