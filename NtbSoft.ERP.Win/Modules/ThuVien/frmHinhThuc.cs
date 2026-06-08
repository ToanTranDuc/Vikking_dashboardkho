using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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
    public partial class frmHinhThuc : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        //Thoai thêm biến lấy tên nv
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();
        DataTable tbl = new DataTable();
        public frmHinhThuc()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            tenNVHienThi = GetTenNhanVien(); //Thoại khởi tạo lấy ds tên nv
            LoadData();
            //CreateTableThuVien(); //Thoai
        }

        private void LoadData()
        {
            string url = string.Format("{0}?", URL + "HinhThuc/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null) tbl.AcceptChanges();
            if (tbl.Rows.Count > 0 && tbl != null)
                gridControl1.DataSource = tbl;
            else
                gridControl1.DataSource = null;
        }

        private DataTable CreateTableThuVien() //Thoai sửa từ void -> DataTable
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaHT", typeof(string));
            tbl.Columns.Add("TenHT", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            return tbl;
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            var dtSource = gridView1.DataSource as DataTable;
            if (dtSource == null)
            {
                dtSource = this.tbl;
            }
            if (dtSource == null)
            {
                clsWaitForm.ShowSuccessFormCustom(this, 2000, "Không có dữ liệu thay đổi!");
                return;
            }
            SaveHinhThuc(tbl);
            
            /*foreach (DataRow dr in tbl.Rows)
            {
                if (string.IsNullOrWhiteSpace(dr["TenHT"].ToString()))
                {
                    XtraMessageBox.Show("Vui lòng điền thông tin hình thức", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (tbl.Rows.Count > 0 && tbl != null)
            {
                string url = string.Format("{0}?", URL + "HinhThuc/Post");
                string mss = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu Quy đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
                var dtSource = gridView1.DataSource as DataTable;
                SaveHinhThuc(tbl);
                LoadData();
            }*/
        }
        #region Thoai them hàm Save
        private void SaveHinhThuc(DataTable dtSource)
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();

            if (dtSource == null || dtSource.Rows.Count == 0) return;
            DataTable dtSend = CreateTableThuVien();

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                DataRow row = gridView1.GetDataRow(i);
                if (row == null) continue;
                if (row.RowState != DataRowState.Added && row.RowState != DataRowState.Modified)
                    continue;

                if (string.IsNullOrWhiteSpace(row["TenHT"]?.ToString()))
                {
                    XtraMessageBox.Show($"Vui lòng điền thông tin 'Tên Hình Thức' ở hàng {i + 1}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    gridView1.FocusedRowHandle = i;
                    gridView1.FocusedColumn = gridView1.Columns["TenHT"];
                    gridView1.ShowEditor();
                    return;
                }

                DataRow newR = dtSend.NewRow();

                string maHT = "";
                if (string.IsNullOrEmpty(maHT))
                    maHT = ReplaceSpecialCharacterssize(row["TenHT"]?.ToString() ?? "");

                newR["ID"] = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]);
                newR["MaHT"] = maHT;
                newR["TenHT"] = row["TenHT"]?.ToString() ?? "";
                newR["GhiChu"] = row["GhiChu"]?.ToString() ?? "";

                if (row.RowState == DataRowState.Added)
                {
                    newR["NguoiTao"] = GlobleData.UserName;
                    newR["NgayTao"] = DateTime.Now;
                    newR["NguoiSua"] = DBNull.Value;
                    newR["NgaySua"] = DBNull.Value;
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    newR["NguoiTao"] = row["NguoiTao", DataRowVersion.Original];
                    newR["NgayTao"] = row["NgayTao", DataRowVersion.Original];
                    newR["NguoiSua"] = GlobleData.UserName;
                    newR["NgaySua"] = DateTime.Now;
                }

                dtSend.Rows.Add(newR);
            }

            if (dtSend.Rows.Count == 0)
            {
                clsWaitForm.ShowSuccessFormCustom(this, 2000, "Không có dữ liệu thay đổi!");
                return;
            }
            string url = string.Format("{0}?", URL + "HinhThuc/Post");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSend); }).Result;

            if (msResult != null && msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadData();
            }
            else
            {
                XtraMessageBox.Show("Lỗi Lưu Hình Thức", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        private string ReplaceSpecialCharacterssize(string input)
        {
            string pattern = @"[^\w\s\(\)]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            var a = regex.Replace(input, replacement).Replace(" ", "");
            return regex.Replace(input, replacement).Replace(" ", "");
        }
       
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridControl1.DataSource != null)
                {
                    tbl = gridControl1.DataSource as DataTable;
                }
                if(this.tbl == null)
                {
                    this.tbl = CreateTableThuVien();
                    gridControl1.DataSource = this.tbl;
                }
                this.ActiveControl = simpleButton1;
                DataRow dr = tbl.NewRow();
                dr["ID"] = 0;
                dr["MaHT"] = "";
                #region Thoai
                dr["NguoiTao"] = GlobleData.UserName;
                dr["NgayTao"] = DateTime.Now;
                #endregion
                tbl.Rows.InsertAt(dr, 0);
                gridControl1.DataSource = tbl;
            }

            catch (Exception ex)
            {
            }
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                // e.ListSourceRowIndex là vị trí thật trong data source
                if (e.ListSourceRowIndex >= 0)
                    e.DisplayText = (e.ListSourceRowIndex + 1).ToString();
            }
            #region Thoai
            if (e.Column == colNguoiTao || e.Column == colNguoiSua)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
                {
                    string upperUser = username.ToUpper();
                    if (tenNVHienThi.ContainsKey(upperUser))
                    {
                        e.DisplayText = tenNVHienThi[upperUser];
                    }
                }
            }
            #endregion
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

                    DataRow dr = gridView1.GetFocusedDataRow();
                    if (dr == null) return;
                    int id = Convert.ToInt32(dr["ID"].ToString());


                    string url = string.Format("{0}?Parameter={1}", URL + "HinhThuc/Delete", id);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #region Thoai
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || e.RowHandle < 0 || view.IsNewItemRow(e.RowHandle)) return;

                DataRow row = view.GetDataRow(e.RowHandle);
                if (row == null) return;

                if (row["ID"] != DBNull.Value && Convert.ToInt32(row["ID"]) > 0)
                {
                    row["NguoiSua"] = GlobleData.UserName;
                    row["NgaySua"] = DateTime.Now;
                }
                else if (string.IsNullOrEmpty(row["NguoiTao"]?.ToString()))
                {
                    row["NguoiTao"] = GlobleData.UserName;
                    row["NgayTao"] = DateTime.Now;
                }
            }
            catch
            {
                clsWaitForm.ShowErrorForm(this, 1500);
            }
        }
        #endregion
        
        #region Thoai
        private Dictionary<string, string> GetTenNhanVien()
        {
            try
            {
                string url = string.Format("{0}?", URL + "GetTenNV/GetTenNV");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    DataTable dtNhanVien = JsonConvert.DeserializeObject<DataTable>(json);
                    string userNameCol = "UserName";
                    string tenNVCol = "TenNV";
                    return dtNhanVien.AsEnumerable()
                        .Where(row => row[userNameCol] != DBNull.Value && row[userNameCol] != null && !string.IsNullOrWhiteSpace(row.Field<string>(userNameCol))) // Lọc row null/rỗng
                        .GroupBy(row => row.Field<string>(userNameCol).Trim().ToUpper())
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().Field<string>(tenNVCol) ?? g.Key
                        );
                }

            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
            return new Dictionary<string, string>();
        }
        #endregion
    }
}