using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ThuVien;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPThongSoVatTu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblGridVatTu, tblGridMauVT, tblGridKhoVai, tblKH, tblMH, tblVatTu, tblMauVT, tblKhoVai, tblNhom, tblMHCopy,tblDV;
        bool flagVT = true, flagMau = true, flagKho = true,flagDV=true;//gắn cờ cho việc khai báo cũ or mới, true = mới
        bool flagCopy=false;

        public frmERPThongSoVatTu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            //tblGridVatTu = new DataTable();
            tblKH = new DataTable();
            tblMH = new DataTable();
            tblVatTu = new DataTable();
            tblMauVT = new DataTable();
            tblKhoVai = new DataTable();
            tblNhom = new DataTable();
            tblMHCopy = new DataTable();
            tblDV = new DataTable();
            CreateSearchLookUpKH();
            CreateSearchLookUpKHCopy();
            CreateTableGridVatTu();
            CreateTableGridMauVatTu();
            CreateTableGridKhoVai();
            this.ActiveControl = button1;
        }
        protected override void OnLoad(EventArgs e)
        {
          
            cbBoxVatTu.EditValue = "Khai báo mới";
            cbBoxMau.EditValue = "Khai báo mới";
            cbBoxKho.EditValue = "Khai báo mới";
            cbBoxDV.EditValue = "Khai báo mới";
            CreateSearchLookUpVatTu();
            CreateSearchLookUpMau();
            CreateSearchLookUpKho();
            CreateSearchLookUpNhomNPL();
            CreateSearchLookUpDV();
            this.ActiveControl = button1;
        }
        private void CreateTableGridVatTu()
        {
            tblGridVatTu = new DataTable("tblGridVatTu");

            tblGridVatTu.Columns.Add("ID", typeof(int));
            tblGridVatTu.Columns.Add("MaKH", typeof(string));
            tblGridVatTu.Columns.Add("TenKH", typeof(string));
            tblGridVatTu.Columns.Add("MaHang", typeof(string));
            tblGridVatTu.Columns.Add("MaVTID", typeof(string));
            tblGridVatTu.Columns.Add("MaVT", typeof(string));
            tblGridVatTu.Columns.Add("TenVT", typeof(string));
            tblGridVatTu.Columns.Add("ChiTiet", typeof(string));
         
            tblGridVatTu.Columns.Add("NhomNL", typeof(string));
            tblGridVatTu.Columns.Add("TenNhom", typeof(string));
            tblGridVatTu.Columns.Add("NPL", typeof(bool));
            tblGridVatTu.Columns.Add("GhiChu", typeof(string));
            tblGridVatTu.Columns.Add("MaDVVT", typeof(string));
            tblGridVatTu.Columns.Add("TenDVVT", typeof(string));
        }
        private void CreateTableGridMauVatTu()
        {
            tblGridMauVT = new DataTable("tblGridMauVT");

            tblGridMauVT.Columns.Add("ID", typeof(int));
         
            tblGridMauVT.Columns.Add("MauID", typeof(string));
            tblGridMauVT.Columns.Add("MaMauVT", typeof(string));
            tblGridMauVT.Columns.Add("MauVT", typeof(string));
            tblGridMauVT.Columns.Add("MaKH", typeof(string));
            tblGridMauVT.Columns.Add("MaHang", typeof(string));
        }
        private void CreateTableGridKhoVai()
        {
            tblGridKhoVai = new DataTable("tblGridKhoVai");

            tblGridKhoVai.Columns.Add("ID", typeof(int));
          
            tblGridKhoVai.Columns.Add("KhoVaiID", typeof(string));
            tblGridKhoVai.Columns.Add("KhoVai", typeof(string));
            tblGridKhoVai.Columns.Add("MaKH", typeof(string));
            tblGridKhoVai.Columns.Add("MaHang", typeof(string));
        }
        #region searchLookup
        private void searchLookUpEditMH_Properties_EditValueChanged(object sender, EventArgs e)
        {
            loadThongSoVT(true,true,true);
        }
        private void searchLookUpEditKH_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditKH.EditValue != null)
            {
                CreateSearchLookUpMH(searchLookUpEditKH.EditValue.ToString());
                //CreateSearchLookUpMHCopy(searchLookUpEditKH.EditValue.ToString());
            }

            loadThongSoVT(true,true,true);
        }
        private void cbBoxVatTu_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (cbBoxVatTu.EditValue.ToString() == "Khai báo mới")
            {
                flagVT = true;
                LCItxtMaVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtTenVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtChiTiet.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCIsearchTenVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (flagDV)
                {
                    LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                txtMaVT.Text = "";
                txtTenVT.Text = "";
                txtChiTiet.Text = "";
              
                (searchLookUpEditTenVT.Properties.View as GridView)?.ClearSelection();
                searchLookUpEditTenVT.Text = "";
                splitContainerControl1.SplitterPosition +=30 ;
            }
            else if (cbBoxVatTu.EditValue.ToString() == "Đã khai báo")
            {
                flagVT = false;
                LCItxtMaVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtTenVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtChiTiet.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIsearchTenVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtMaVT.Text = "";
                txtTenVT.Text = "";
                txtChiTiet.Text = "";
                (searchLookUpEditTenVT.Properties.View as GridView)?.ClearSelection();
                searchLookUpEditTenVT.Text = "";
                splitContainerControl1.SplitterPosition -= 30;
                CreateSearchLookUpVatTu();
                
            }
        }

        private void cbBoxMau_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (cbBoxMau.EditValue.ToString() == "Khai báo mới")
            {
                flagMau = true;
                LCItxtMaMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCIsearchMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                txtMaMauVT.Text = "";
                txtMauVT.Text = "";
               
                (searchLookUpEditMauVT.Properties.View as GridView)?.ClearSelection();
                searchLookUpEditMauVT.Text = "";
                splitContainerControl1.SplitterPosition += 22;
            }
            else if (cbBoxMau.EditValue.ToString() == "Đã khai báo")
            {
                flagMau = false;
                LCItxtMaMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIsearchMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtMaMauVT.Text = "";
                txtMauVT.Text = "";
                searchLookUpEditMauVT.EditValue = null;
                (searchLookUpEditMauVT.Properties.View as GridView)?.ClearSelection();
                searchLookUpEditMauVT.Text = "";
                splitContainerControl1.SplitterPosition-= 22;
                CreateSearchLookUpMau();
            }
        }

        private void searchLookUpEditTenVT_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            if (edit != null)
            {
                // Lấy danh sách các hàng được chọn
                List<string> selectedItems = new List<string>();
                foreach (var rowHandle in edit.Properties.View.GetSelectedRows())
                {
                    object value = edit.Properties.View.GetRowCellValue(rowHandle, "MaVT");
                    if (value != null)
                    {
                        selectedItems.Add(value.ToString());
                    }
                }

                // Gán chuỗi kết quả
                e.DisplayText = string.Join("|", selectedItems);
            }
        }

        private void cbBoxKho_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (cbBoxKho.EditValue.ToString() == "Khai báo mới")
            {
                flagKho = true;
                LCItxtKho.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCIsearchKho.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                txtKho.Text = "";
              
                (searchLookUpEditKho.Properties.View as GridView)?.ClearSelection();
                searchLookUpEditKho.Text = "";

            }
            else if (cbBoxKho.EditValue.ToString() == "Đã khai báo")
            {
                flagKho = false;
                LCItxtKho.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIsearchKho.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtKho.Text = "";
                (searchLookUpEditKho.Properties.View as GridView)?.ClearSelection();
                searchLookUpEditKho.Text = "";
                CreateSearchLookUpKho();
            }
        }

        private void CreateSearchLookUpKH()
        {
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            searchLookUpEditKH.Properties.NullText = "Chọn khách hàng";
            string url = string.Format("{0}?", URL + "ERPThongSoVatTu/GetKH");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblKH = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditKH.Properties.DataSource = tblKH;

            searchLookUpEditKH.RefreshEditValue();
            searchLookUpEditKH.Refresh();
        }
        private void CreateSearchLookUpKHCopy()
        {
            searchLookUpEditKHCopy.Properties.ValueMember = "MaKH";
            searchLookUpEditKHCopy.Properties.DisplayMember = "TenKH";
            //searchLookUpEditKHCopy.Properties.NullText = "Chọn khách hàng copy";
            string url = string.Format("{0}?", URL + "ERPThongSoVatTu/GetKH");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblKH = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditKHCopy.Properties.DataSource = tblKH;

            searchLookUpEditKHCopy.RefreshEditValue();
            searchLookUpEditKHCopy.Refresh();
        }
        private void searchLookUpEditMauVT_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            if (edit != null)
            {

                List<string> selectedItems = new List<string>();
                foreach (var rowHandle in edit.Properties.View.GetSelectedRows())
                {
                    object value = edit.Properties.View.GetRowCellValue(rowHandle, "MauVT");
                    if (value != null)
                    {
                        selectedItems.Add(value.ToString());
                    }
                }


                e.DisplayText = string.Join("|", selectedItems);
            }
        }

        private void searchLookUpEditKho_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            if (edit != null)
            {
                List<string> selectedItems = new List<string>();
                foreach (var rowHandle in edit.Properties.View.GetSelectedRows())
                {
                    object value = edit.Properties.View.GetRowCellValue(rowHandle, "KhoVai");
                    if (value != null)
                    {
                        selectedItems.Add(value.ToString());
                    }
                }
                e.DisplayText = string.Join("|", selectedItems);
            }
        }
        private void CreateSearchLookUpMH(string makh)
        {
            if (string.IsNullOrWhiteSpace(makh)) return;
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "MaHang";
            searchLookUpEditMH.Properties.NullText = "Chọn mã hàng";
            string url = string.Format("{0}?makh={1}", URL + "ERPThongSoVatTu/GetMH", makh);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditMH.Properties.DataSource = null;
                return;
            } 
                
              
            tblMH = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditMH.Properties.DataSource = tblMH;
            searchLookUpEditMH.RefreshEditValue();
            searchLookUpEditMH.Refresh();
        }

        private void CreateSearchLookUpMHCopy(string makh)
        {
            if (string.IsNullOrWhiteSpace(makh)) return;
            searchLookUpEditMHCopy.Properties.ValueMember = "MaHang";
            searchLookUpEditMHCopy.Properties.DisplayMember = "MaHang";
            searchLookUpEditMHCopy.Properties.NullText = "Chọn mã hàng";
            string url = string.Format("{0}?makh={1}", URL + "ERPThongSoVatTu/GetMH", makh);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMHCopy = JsonConvert.DeserializeObject<DataTable>(json);
            if(searchLookUpEditKH.EditValue!=null&& searchLookUpEditKH.EditValue.ToString()== searchLookUpEditKHCopy.EditValue.ToString())
            {
                if (searchLookUpEditMH.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Mã hàng trước và thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchLookUpEditKHCopy.EditValue = null;
                    return;
                }

                DataRow rowToDelete = tblMHCopy.AsEnumerable()
                                   .FirstOrDefault(row => row["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString());

                if (rowToDelete != null)
                {
                    tblMHCopy.Rows.Remove(rowToDelete);
                }
            }    
          
            searchLookUpEditMHCopy.Properties.DataSource = tblMHCopy;
            searchLookUpEditMHCopy.RefreshEditValue();
            searchLookUpEditMHCopy.Refresh();
        }
        private void CreateSearchLookUpVatTu()
        {
            string url = string.Format("{0}?", URL + "ERPThongSoVatTu/GetVatTuGoiY");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditTenVT.Properties.DataSource = tblVatTu;
        }
        private void CreateSearchLookUpMau()
        {
            string url = string.Format("{0}?", URL + "ERPThongSoVatTu/GetMauVatTuGoiY");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditMauVT.Properties.DataSource = tblMauVT;
        }
        private void CreateSearchLookUpKho()
        {
            string url = string.Format("{0}?", URL + "ERPThongSoVatTu/GetKhoVaiGoiY");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblKhoVai = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditKho.Properties.DataSource = tblKhoVai;
        }
        private void CreateSearchLookUpNhomNPL()
        {
            string url = string.Format("{0}?", URL + "ERPThongSoVatTu/GetNhomGoiY");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblNhom = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblNhom != null)
            {
                tblNhom.Columns.Add("LoaiVatTu", typeof(string)); 
              
            }
            foreach (DataRow row in tblNhom.Rows)
            {
               if (Convert.ToBoolean(row["NPL"]))
               {
                    row["LoaiVatTu"] = "Nguyên liệu";
               }
                else
                {
                    row["LoaiVatTu"] = "Phụ liệu";
                }
            }
            searchLookUpEditNhomNPL.Properties.ValueMember = "MaNhom";
            searchLookUpEditNhomNPL.Properties.DisplayMember = "TenNhom";
            searchLookUpEditNhomNPL.Properties.NullText = "Chọn nhóm nguyên phụ liệu";
            searchLookUpEditNhomNPL.Properties.DataSource = tblNhom;
            repoSearchNhomNPL.ValueMember = "TenNhom";
            repoSearchNhomNPL.DisplayMember = "TenNhom";
            repoSearchNhomNPL.DataSource = tblNhom;
            repoSearchNhomNPL.PopulateViewColumns();

         
        }
        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            searchLookUpEditTenVT.EditValue = GetSelectedValues(searchLookUpEditTenVT, "MaVT");
            searchLookUpEditTenVT.DoValidate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
           
          
            bool _isLuuVT= LuuVatTu();
            bool _isLuuMau=LuuMau();
            bool _isLuuKhoVai=LuuKhoVai();
            loadThongSoVT(_isLuuVT, _isLuuMau, _isLuuKhoVai);

        }
        private bool LuuVatTu()
        {
            if (tblGridVatTu.Rows.Count == 0 || tblGridVatTu == null)
            {

                return false;
            }
            DataTable tblLuu;
          
            if (!ValidateVatTu(tblGridVatTu, out tblLuu))
            {
                //XtraMessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (tblLuu.Rows.Count == 0 || tblLuu == null)
            {

                return false;
            }
            try
            {
                clsWaitForm.ShowWaitForm(this, 2000);
                string url = string.Format("{0}", URL + "ERPThongSoVatTu/Post");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblLuu); }).Result;
                if (msResult.ToLower() == "true")
                {

                    return true;
                    clsWaitForm.ShowSuccessForm(this, 2000);
                
                }
            }
            catch (Exception ex)
            {
               
            }
            return false;

        }
        private bool LuuMau()
        {
            if (tblGridMauVT.Rows.Count == 0 || tblGridMauVT == null)
            {

                return false;
            }
            DataTable tblLuu;

            if (!ValidateMau(tblGridMauVT, out tblLuu))
            {
                //XtraMessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (tblLuu.Rows.Count == 0 || tblLuu == null)
            {

                return false;
            }
            try
            {

                clsWaitForm.ShowWaitForm(this, 2000);
                string url = string.Format("{0}", URL + "ERPThongSoVatTu/PostMauVatTu");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblLuu); }).Result;
                if (msResult.ToLower() == "true")
                {

                    return true;
                    clsWaitForm.ShowSuccessForm(this, 2000);

                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        private bool LuuKhoVai()
        {
            if (tblGridKhoVai.Rows.Count == 0 || tblGridKhoVai == null)
            {

                return false;
            }
            DataTable tblLuu;

            if (!ValidateKhoVai(tblGridKhoVai, out tblLuu))
            {
                //XtraMessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (tblLuu.Rows.Count == 0 || tblLuu == null)
            {

                return false;
            }

            try
            {
                clsWaitForm.ShowWaitForm(this, 2000);
                string url = string.Format("{0}", URL + "ERPThongSoVatTu/PostKhoVai");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblLuu); }).Result;
                if (msResult.ToLower() == "true")
                {

                    return true;
                    clsWaitForm.ShowSuccessForm(this, 2000);

                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        private bool ValidateVatTu(DataTable tblGrid, out DataTable processedTable)
        {
            bool isValid = true;

            // Kiểm tra thiếu dữ liệu và loại bỏ các hàng thiếu
            for (int i = tblGrid.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = tblGrid.Rows[i];
                if (string.IsNullOrWhiteSpace(row["MaVT"].ToString()) || string.IsNullOrWhiteSpace(row["TenNhom"].ToString()))
                {
                    tblGrid.Rows.Remove(row);
                    isValid = false;
                }
            }

            // Loại bỏ các hàng trùng lặp trong tblGrid
            var distinctRows = tblGrid.AsEnumerable()
        .GroupBy(row => new
        {
            MaVT = row["MaVT"].ToString(),
            TenNhom = row["TenNhom"].ToString()
        })
        .Select(group => group.First()).ToList();

            // Tạo DataTable mới với cấu trúc giống tblGrid
            DataTable newTable = tblGrid.Clone(); // Clone cấu trúc, không sao chép dữ liệu
            foreach (var row in distinctRows)
            {
                newTable.ImportRow(row); // Thêm các hàng vào bảng mới
            }

            // Gán lại tblGrid
            tblGrid.Clear();
            foreach (DataRow row in newTable.Rows)
            {
                tblGrid.ImportRow(row);
            }

            // So sánh với dữ liệu từ API và loại bỏ các hàng trùng
            if (searchLookUpEditKH.EditValue != null && searchLookUpEditMH.EditValue != null)
            {
                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mamh = searchLookUpEditMH.EditValue.ToString();
                string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/Get", _makh, _mamh);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                DataTable tblVatTuFromApi = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblVatTuFromApi != null && tblVatTuFromApi.Rows.Count > 0)
                {
                    // Tìm các giá trị trùng với API
                    var duplicateWithApi = (from rowGrid in tblGrid.AsEnumerable()
                                            join rowApi in tblVatTuFromApi.AsEnumerable()
                                            on new { MaVT = rowGrid["MaVT"].ToString(), TenNhom = rowGrid["TenNhom"].ToString() }
                                            equals new { MaVT = rowApi["MaVT"].ToString(), TenNhom = rowApi["TenNhom"].ToString() }
                                            select new { MaVT = rowGrid["MaVT"].ToString(), TenNhom = rowGrid["TenNhom"].ToString() })
                                            .Distinct().ToList();

                    // Loại bỏ các hàng trùng với API
                    for (int i = tblGrid.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow row = tblGrid.Rows[i];
                        if (duplicateWithApi.Any(d => d.MaVT == row["MaVT"].ToString() && d.TenNhom == row["TenNhom"].ToString()))
                        {
                            tblGrid.Rows.Remove(row);
                            isValid = false;
                        }
                    }
                }
            }

            // Gán tblGrid đã xử lý cho tham số out
            processedTable = tblGrid;
            return isValid;
        }
        private bool ValidateMau(DataTable tblGrid, out DataTable processedTable)
        {
            bool isValid = true;

            // Loại bỏ các hàng trùng lặp trong tblGrid
            var distinctRows = tblGrid.AsEnumerable()
                .GroupBy(row => new
                {
                    MaMauVT = row["MaMauVT"].ToString(),
                    MauVT = row["MauVT"].ToString()
                })
                .Select(group => group.First()).ToList();
            DataTable newTable = tblGrid.Clone();
            foreach (var row in distinctRows)
            {
                newTable.ImportRow(row);
            }
            tblGrid.Clear();
            foreach (DataRow row in newTable.Rows)
            {
                tblGrid.ImportRow(row);
            }


            // So sánh với dữ liệu từ API và loại bỏ các hàng trùng
            if (searchLookUpEditKH.EditValue != null && searchLookUpEditMH.EditValue != null)
            {
                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mamh = searchLookUpEditMH.EditValue.ToString();
                string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/GetMauVatTu", _makh, _mamh);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                DataTable tblMauFromApi = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblMauFromApi != null && tblMauFromApi.Rows.Count > 0)
                {
                    // Tìm các giá trị trùng với API
                    var duplicateWithApi = (from rowGrid in tblGrid.AsEnumerable()
                                            join rowApi in tblMauFromApi.AsEnumerable()
                                            on new { MaMauVT = rowGrid["MaMauVT"].ToString(), MauVT = rowGrid["MauVT"].ToString() }
                                            equals new { MaMauVT = rowApi["MaMauVT"].ToString(), MauVT = rowApi["MauVT"].ToString() }
                                            select new { MaMauVT = rowGrid["MaMauVT"].ToString(), MauVT = rowGrid["MauVT"].ToString() })
                                            .Distinct().ToList();

                    // Loại bỏ các hàng trùng với API
                    for (int i = tblGrid.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow row = tblGrid.Rows[i];
                        if (duplicateWithApi.Any(d => d.MaMauVT == row["MaMauVT"].ToString() && d.MauVT == row["MauVT"].ToString()))
                        {
                            tblGrid.Rows.Remove(row);
                            isValid = false;
                        }
                    }
                }
            }

            // Gán tblGrid đã xử lý cho tham số out
            processedTable = tblGrid;
            return isValid;
        }
        private bool ValidateKhoVai(DataTable tblGrid, out DataTable processedTable)
        {
            bool isValid = true;

            
            var distinctRows = tblGrid.AsEnumerable()
                .GroupBy(row => row["KhoVai"].ToString())
                .Select(group => group.First()).ToList();

            DataTable newTable = tblGrid.Clone();
            foreach (var row in distinctRows)
            {
                newTable.ImportRow(row);
            }
            tblGrid.Clear();
            foreach (DataRow row in newTable.Rows)
            {
                tblGrid.ImportRow(row);
            }
            if (searchLookUpEditKH.EditValue != null && searchLookUpEditMH.EditValue != null)
            {
                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mamh = searchLookUpEditMH.EditValue.ToString();
                string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/GetKhoVai", _makh, _mamh);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                DataTable tblKhoVaiFromApi = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblKhoVaiFromApi != null && tblKhoVaiFromApi.Rows.Count > 0)
                {
                    // Tìm các giá trị trùng với API
                    var duplicateWithApi = (from rowGrid in tblGrid.AsEnumerable()
                                            join rowApi in tblKhoVaiFromApi.AsEnumerable()
                                            on rowGrid["KhoVai"].ToString() equals rowApi["KhoVai"].ToString()
                                            select rowGrid["KhoVai"].ToString()).Distinct().ToList();

                    // Loại bỏ các hàng trùng với API
                    for (int i = tblGrid.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow row = tblGrid.Rows[i];
                        if (duplicateWithApi.Contains(row["KhoVai"].ToString()))
                        {
                            tblGrid.Rows.Remove(row);
                            isValid = false;
                        }
                    }
                }
            }

            // Gán tblGrid đã xử lý cho tham số out
            processedTable = tblGrid;
            return isValid;
        }
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditMH.EditValue == null) return;
            if (searchLookUpEditKH.EditValue == null) return;
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa tất cả không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
               
                string _mamh = searchLookUpEditMH.EditValue.ToString();
                string url = string.Format("{0}?mahang={1}", URL + "ERPThongSoVatTu/DeleteAll", _mamh);
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
               
                string _makh = searchLookUpEditKH.EditValue.ToString();
                string url1 = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/DeleteAllMauVatTu", _makh, _mamh);
                string result1 = Task.Run(async () => { return await _clientExtension.DeletedAsync(url1); }).Result;
                string url2 = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/DeleteAllKhoVai", _makh, _mamh);
                string result2 = Task.Run(async () => { return await _clientExtension.DeletedAsync(url2); }).Result;
                loadThongSoVT(true, true, true);
            }
           
        }

      

        private void checkEditCopyMH_Click(object sender, EventArgs e)
        {

            if(!flagVT)
            {
                cbBoxVatTu.EditValue = "Khai báo mới";
                flagVT = true;
            }   
            if(!flagMau)
            {
                cbBoxMau.EditValue = "Khai báo mới";
                flagMau = true;
            }   
            if(!flagKho)
            {
                cbBoxKho.EditValue = "Khai báo mới";
                flagKho = true;
            }
            if (!flagDV)
            {
                cbBoxDV.EditValue = "Khai báo mới";
                flagDV = true;
            }
            flagCopy = !flagCopy;
            if(flagCopy)
            {
                LCIMHCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCIKHCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCIbtnCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtMaVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtTenVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtChiTiet.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtMaMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtKho.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCINhomNPL.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                splitContainerControl1.SplitterPosition = 156;
                cbBoxVatTu.Properties.ReadOnly = true;
                cbBoxMau.Properties.ReadOnly = true;
                cbBoxKho.Properties.ReadOnly = true;
                cbBoxDV.Properties.ReadOnly = true;
            }
            else
            {
                LCIMHCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIKHCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIbtnCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCItxtMaVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtTenVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtChiTiet.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtMaMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtMauVT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCItxtKho.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCINhomNPL.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if(flagDV)
                {
                    LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
              
               
                splitContainerControl1.SplitterPosition = 290;
                cbBoxVatTu.Properties.ReadOnly = false;
                cbBoxMau.Properties.ReadOnly = false;
                cbBoxKho.Properties.ReadOnly = false;
                cbBoxDV.Properties.ReadOnly = false;
            }
          
        }

        private void searchLookUpEditMHCopy_Properties_EditValueChanged(object sender, EventArgs e)
        {
            //loadMaHangCopy();
        }

        private void searchLookUpEdit3View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            searchLookUpEditMauVT.EditValue = GetSelectedValues(searchLookUpEditMauVT, "MauVT");
            searchLookUpEditMauVT.DoValidate();
        }

        private void gVVatTu_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            loadMaHangCopy();
        }

        private void btnNapLai_Click(object sender, EventArgs e)
        {
            loadThongSoVT(true,true,true);
        }

        private void gVMauVT_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void gVKhoVai_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void searchLookUpEditKHCopy_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if(searchLookUpEditKHCopy.EditValue!=null)
            {
                CreateSearchLookUpMHCopy(searchLookUpEditKHCopy.EditValue.ToString());
            }    
            
        }

        private void searchLookUpEdit2View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            searchLookUpEditKho.EditValue = GetSelectedValues(searchLookUpEditKho, "KhoVai");
            searchLookUpEditKho.DoValidate();
        }

        //hàm gọi từ view của searchLookupmutilselect, để update displaytext ngay lập tức
        private string GetSelectedValues(SearchLookUpEdit search, string _tencot)
        {
            List<string> selectedValues = new List<string>();
            foreach (var rowHandle in search.Properties.View.GetSelectedRows())
            {
                object value = search.Properties.View.GetRowCellValue(rowHandle, _tencot);
                if (value != null)
                {
                    selectedValues.Add(value.ToString());
                }
            }

            return string.Join(", ", selectedValues);
        }
        #endregion
        private void loadThongSoVT(bool _isLuuVT,bool _isLuuMau,bool _isLuuKhoVai)
        {
            if (searchLookUpEditKH.EditValue == null || searchLookUpEditMH.EditValue == null) return;
            string _makh = searchLookUpEditKH.EditValue.ToString();
            string _mamh = searchLookUpEditMH.EditValue.ToString();
            if (string.IsNullOrWhiteSpace(_mamh) || string.IsNullOrWhiteSpace(_makh)) return;
            if (_isLuuVT)
            {
                tblGridVatTu.Clear();
                gCVatTu.DataSource = tblGridVatTu.Clone();
            }
            if (_isLuuMau)
            {
                tblGridMauVT.Clear();
                gCMauVT.DataSource = tblGridMauVT.Clone();
            }
            if (_isLuuKhoVai)
            {
                tblGridKhoVai.Clear();
                gCKhoVai.DataSource = tblGridKhoVai.Clone();
            }

        }
        #region load khong dùng tới
        private void loadVatTu(string _makh, string _mamh)
        {
            //Vật tư
            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/Get", _makh, _mamh);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                tblGridVatTu.Clear();
                gCVatTu.DataSource = tblGridVatTu.Clone();
                return;
            }
            tblGridVatTu = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblGridVatTu.Rows.Count == 0 || tblGridVatTu == null)
            {
                tblGridVatTu.Clear();
                gCVatTu.DataSource = tblGridVatTu.Clone();
                return;
            }
            gCVatTu.DataSource = tblGridVatTu;
        }
        private void loadMau(string _makh, string _mamh)
        {
            //Màu vật tư
            string url1 = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/GetMauVatTu", _makh, _mamh);
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            if (json1 == "[]")
            {
                tblGridMauVT.Clear();
                gCMauVT.DataSource = tblGridMauVT.Clone();
                return;
            }
            tblGridMauVT = JsonConvert.DeserializeObject<DataTable>(json1);
            if (tblGridMauVT == null || tblGridMauVT.Rows.Count == 0)
            {
                tblGridMauVT.Clear();
                gCMauVT.DataSource = tblGridMauVT.Clone();
                return;
            }
            gCMauVT.DataSource = tblGridMauVT;
        }
        private void loadKhoVai(string _makh, string _mamh)
        {
            //Khổ vải
            string url2 = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/GetKhoVai", _makh, _mamh);
            string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
            if (json2 == "[]")
            {
                tblGridKhoVai.Clear();
                gCKhoVai.DataSource = tblGridKhoVai.Clone();
                return;
            }
            tblGridKhoVai = JsonConvert.DeserializeObject<DataTable>(json2);
            if (tblGridKhoVai.Rows.Count == 0 || tblGridKhoVai == null)
            {
                tblGridKhoVai.Clear();
                gCKhoVai.DataSource = tblGridKhoVai.Clone();
                return;
            }
            gCKhoVai.DataSource = tblGridKhoVai;
        }
        #endregion

        private void gVVatTu_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVVatTu.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", ItemXoaDongVatTu_Click);
                        e.Menu.Items.Add(menuCopyItem);
                    }

                }
            }
        }
        private void ItemXoaDongVatTu_Click(object sender, EventArgs e)
        {
            DataRow dr = gVVatTu.GetFocusedDataRow();
            if (dr != null)
            {
                if (dr["id"] != null && int.TryParse(dr["id"].ToString(), out int id))
                {
                    if (id != 0)
                    {
                        string url = string.Format("{0}?id={1}", URL + "ERPThongSoVatTu/Delete", id);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    }
                }
                tblGridVatTu.Rows.Remove(dr);
                gVVatTu.RefreshData();

            }


        }

        private void gVMauVT_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVMauVT.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", ItemXoaDongMau_Click);
                        e.Menu.Items.Add(menuCopyItem);
                    }

                }
            }
        }
        private void ItemXoaDongMau_Click(object sender, EventArgs e)
        {
            DataRow dr = gVMauVT.GetFocusedDataRow();
            if (dr != null)
            {
                if (dr["id"] != null && int.TryParse(dr["id"].ToString(), out int id))
                {
                    if (id != 0)
                    {
                        string url = string.Format("{0}?id={1}", URL + "ERPThongSoVatTu/DeleteMauVatTu", id);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    }
                }
                tblGridMauVT.Rows.Remove(dr);
                gVMauVT.RefreshData();

            }


        }

        private void gVKhoVai_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVKhoVai.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", ItemXoaDongKhoVai_Click);
                        e.Menu.Items.Add(menuCopyItem);
                    }

                }
            }
        }
        private void ItemXoaDongKhoVai_Click(object sender, EventArgs e)
        {
            DataRow dr = gVKhoVai.GetFocusedDataRow();
            if (dr != null)
            {
                if (dr["id"] != null && int.TryParse(dr["id"].ToString(), out int id))
                {
                    if (id != 0)
                    {
                        string url = string.Format("{0}?id={1}", URL + "ERPThongSoVatTu/DeleteKhoVai", id);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    }
                }
                tblGridKhoVai.Rows.Remove(dr);
                gVKhoVai.RefreshData();

            }


        }
     
        private void loadMaHangCopy()
        {
            if (searchLookUpEditKH.EditValue == null || searchLookUpEditMHCopy.EditValue == null|| searchLookUpEditMH.EditValue == null||searchLookUpEditKHCopy.EditValue==null) return;
            string _makh = searchLookUpEditKH.EditValue.ToString();
            string _mamh = searchLookUpEditMH.EditValue.ToString();
            string _mamhcopy = searchLookUpEditMHCopy.EditValue.ToString();
            string _makhcopy = searchLookUpEditKHCopy.EditValue.ToString();
            copyVatTu(_makh, _mamh, _makhcopy, _mamhcopy);
            copyMau(_makh, _mamh, _makhcopy, _mamhcopy);
            copyKhoVai(_makh, _mamh, _makhcopy, _mamhcopy);



        }
        private void copyVatTu(string _makh,string _mamh, string _makhcopy,string _mamhcopy)
        {
            string _tenkh = searchLookUpEditKH.Text;
            if (string.IsNullOrWhiteSpace(_mamh) || string.IsNullOrWhiteSpace(_makh) || string.IsNullOrWhiteSpace(_mamhcopy) || string.IsNullOrWhiteSpace(_makhcopy)) return;
            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/Get", _makhcopy, _mamhcopy);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                //tblGridVatTu.Clear();
                //gCThongSo.DataSource = tblGridVatTu.Clone();
                return;
            }
            DataTable tblCopyVatTu = JsonConvert.DeserializeObject<DataTable>(json);
            tblGridVatTu.Clear();
            if (tblCopyVatTu != null && tblCopyVatTu.Rows.Count > 0)
            {
                foreach (DataRow row in tblCopyVatTu.Rows)
                {
                    row["ID"] = 0;
                    row["MaKH"] = _makh;
                    row["TenKH"] = _tenkh;
                    row["MaHang"] = _mamh;
                    tblGridVatTu.ImportRow(row);
                }

                gCVatTu.DataSource = tblGridVatTu; 
                gCVatTu.RefreshDataSource();
            }
        }

      

        private void copyMau(string _makh, string _mamh, string _makhcopy, string _mamhcopy)
        {
            string _tenkh = searchLookUpEditKH.Text;
            if (string.IsNullOrWhiteSpace(_mamh) || string.IsNullOrWhiteSpace(_makh) || string.IsNullOrWhiteSpace(_mamhcopy) || string.IsNullOrWhiteSpace(_makhcopy)) return;
            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/GetMauVatTu", _makhcopy, _mamhcopy);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                //tblGridVatTu.Clear();
                //gCThongSo.DataSource = tblGridVatTu.Clone();
                return;
            }
            DataTable tblCopyMau = JsonConvert.DeserializeObject<DataTable>(json);
            tblGridMauVT.Clear();
            if (tblCopyMau != null && tblCopyMau.Rows.Count > 0)
            {
                foreach (DataRow row in tblCopyMau.Rows)
                {
                    row["ID"] = 0;
                    row["MaKH"] = _makh;
                    row["MaHang"] = _mamh;
                    tblGridMauVT.ImportRow(row);
                }

                gCMauVT.DataSource = tblGridMauVT; 
                gCMauVT.RefreshDataSource(); 
            }
        }

       

        private void copyKhoVai(string _makh, string _mamh, string _makhcopy, string _mamhcopy)
        {
            string _tenkh = searchLookUpEditKH.Text;
            if (string.IsNullOrWhiteSpace(_mamh) || string.IsNullOrWhiteSpace(_makh) || string.IsNullOrWhiteSpace(_mamhcopy) || string.IsNullOrWhiteSpace(_makhcopy)) return;
            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPThongSoVatTu/GetKhoVai", _makhcopy, _mamhcopy);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                //tblGridVatTu.Clear();
                //gCThongSo.DataSource = tblGridVatTu.Clone();
                return;
            }
            DataTable tblCopyKhoVai = JsonConvert.DeserializeObject<DataTable>(json);
            tblGridKhoVai.Clear();
            if (tblCopyKhoVai != null && tblCopyKhoVai.Rows.Count > 0)
            {
                foreach (DataRow row in tblCopyKhoVai.Rows)
                {
                    row["ID"] = 0;
                    row["MaKH"] = _makh;
          
                    row["MaHang"] = _mamh;
                    tblGridKhoVai.ImportRow(row);
                }

                gCKhoVai.DataSource = tblGridKhoVai; 
                gCKhoVai.RefreshDataSource(); 
            }
        }

      

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null)
            {
                XtraMessageBox.Show(" Vui lòng chọn khách hàng!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if ( searchLookUpEditMH.EditValue == null)
            {
                XtraMessageBox.Show(" Vui lòng chọn mã hàng!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string _makh = string.Empty, _tenkh = string.Empty, _nhomnp = string.Empty, _tennhom = string.Empty;
            bool _npl = false;
            DataRowView selectedRow = searchLookUpEditKH.GetSelectedDataRow() as DataRowView;
            DataRowView selectRowNhom = searchLookUpEditNhomNPL.GetSelectedDataRow() as DataRowView;
            DataRowView selectRowDV= searchLookUpEditDV.GetSelectedDataRow() as DataRowView;
            if (selectedRow != null)
            {
                DataRow row = selectedRow.Row;
                _makh = row["MaKH"].ToString();
                _tenkh = row["TenKH"].ToString();
            }

            if (selectRowNhom != null)
            {
                DataRow row = selectRowNhom.Row;
                _nhomnp = row["MaNhom"].ToString();
                _tennhom = row["TenNhom"].ToString();
                _npl = row["NPL"] != DBNull.Value && Convert.ToBoolean(row["NPL"]);
            }
          
            ThemVatTu(_makh,_nhomnp,_tennhom,_npl);
            ThemMau(_makh);
            ThemKhoVai(_makh);
           

        }

      
        private void ThemVatTu(string _makh, string _nhomnp,string _tennhom,bool _npl)
        {

            string _mavattu = "", _tenvattu = "", _chitiet = "", _mavtid = "",_tendv = "";
            if (flagVT)
            {
                if (string.IsNullOrWhiteSpace(txtMaVT.Text) && string.IsNullOrWhiteSpace(txtChiTiet.Text))
                {
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchLookUpEditTenVT.Text))
                {
                    return;
                }
            }

                if (searchLookUpEditNhomNPL.EditValue == null)
            {
                XtraMessageBox.Show(" Vui lòng chọn nhóm nguyên phụ liệu!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (flagVT)
            {
                if (
                    !ValidateInput(txtMaVT, "Vui lòng điền thông tin mã vật tư!!!") ||
                    !ValidateInput(txtChiTiet, "Vui lòng điền thông tin chi tiết vật tư!!!"))
                    return;
                _mavattu = txtMaVT.Text;
                _tenvattu = txtTenVT.Text;
                _chitiet = txtChiTiet.Text;
                if (flagDV)
                {
                    if (
                        !ValidateInput(txtDV, "Vui lòng điền thông tin đơn vị!!!"))
                        return;
                    _tendv = txtDV.Text;
                }
                else
                {
                    if (!ValidateInput(searchLookUpEditDV, "Vui lòng chọn đơn vị tư!!!"))
                        return;

                    int[] selectedRowHandles2 = gVsearchDV.GetSelectedRows();
                    List<string> tendvList = new List<string>();


                    foreach (int rowHandle in selectedRowHandles2)
                    {
                        string tendv = gVsearchDV.GetRowCellValue(rowHandle, "TenDVVT")?.ToString();
                        tendvList.Add(tendv);
                    }
                    _tendv = string.Join("|", tendvList);

                }
            }
            else
            {
                if (!ValidateInput(searchLookUpEditTenVT, "Vui lòng chọn vật tư!!!"))
                    return;

                int[] selectedRowHandles = gVSearchVatTu.GetSelectedRows();
                List<string> mavtidList = new List<string>();
                List<string> mavtList = new List<string>();
                List<string> tenvtList = new List<string>();
                List<string> chitietList = new List<string>();
                List<string> tendvList = new List<string>();


                foreach (int rowHandle in selectedRowHandles)
                {
                    string mavtid = gVSearchVatTu.GetRowCellValue(rowHandle, "MaVTID")?.ToString();
                    string mavt = gVSearchVatTu.GetRowCellValue(rowHandle, "MaVT")?.ToString();
                    string tenvt = gVSearchVatTu.GetRowCellValue(rowHandle, "TenVT")?.ToString();
                    string chitiet = gVSearchVatTu.GetRowCellValue(rowHandle, "ChiTiet")?.ToString();
                    //string madonvi = gVSearchVatTu.GetRowCellValue(rowHandle, "MaDVVT")?.ToString();
                    string tendonvi = gVSearchVatTu.GetRowCellValue(rowHandle, "TenDVVT")?.ToString();

                    mavtidList.Add(mavtid);

                    mavtList.Add(mavt);

                    tenvtList.Add(tenvt);

                    chitietList.Add(chitiet);

                    tendvList.Add(tendonvi);
                }
                _mavtid = string.Join("|", mavtidList);
                _mavattu = string.Join("|", mavtList);
                _tenvattu = string.Join("|", tenvtList);
                _chitiet = string.Join("|", chitietList);
                _tendv = string.Join("|", tendvList);
            }
          

            string[] arrMaVTid = tachString(_mavtid);
            string[] arrMaVT = tachString(_mavattu);
            string[] arrTenVT = tachString(_tenvattu);
            string[] arrChiTiet = tachString(_chitiet);
            string[] arrDonVi = tachString(_tendv);
            if ( arrMaVT.Length != arrChiTiet.Length || arrMaVT.Length != arrDonVi.Length || arrDonVi.Length != arrChiTiet.Length)
            {
                XtraMessageBox.Show(" Vui lòng điền đủ thông tin vật tư!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            addRowToGridVatTu(arrMaVTid, arrMaVT, arrTenVT, arrChiTiet, _makh, _nhomnp, _tennhom, _npl, arrDonVi);

        }

      

        private void addRowToGridVatTu(string[] arrMaVTid, string[] arrMaVT, string[] arrTenVT, string[] arrChiTiet, string _makh, string _nhomnp, string _tennhom, bool _npl,string[] arrDonVi)
        {
           
            if (arrMaVT.Any(string.IsNullOrWhiteSpace))
            { 
                return;
            }
            for(int i=0; i<arrMaVT.Length;i++)
            {
                DataRow dr = tblGridVatTu.NewRow();
                dr["ID"] = 0;
                dr["MaKH"] = _makh == string.Empty ? "" : _makh;
                
                dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                dr["MaVTID"] = (arrMaVTid.Length == 1 || arrMaVTid[0] == "") ? "0" : arrMaVTid[i];
                dr["MaVT"] = arrMaVT[i];
                dr["MaVT"] = arrMaVT[i];
                dr["TenVT"] = "";
                dr["ChiTiet"] = arrChiTiet[i];
                dr["NhomNL"] = _nhomnp;
                dr["TenNhom"] = _tennhom;
                dr["NPL"] = _npl;
                dr["MaDVVT"] = "";
                dr["TenDVVT"] = arrDonVi[i];
                tblGridVatTu.Rows.Add(dr);
            }
            gCVatTu.DataSource = tblGridVatTu;
           
            txtMaVT.Text = "";
            txtTenVT.Text = "";
            txtChiTiet.Text = "";
            txtDV.Text = "";
            (searchLookUpEditTenVT.Properties.View as GridView)?.ClearSelection();
            (searchLookUpEditDV.Properties.View as GridView)?.ClearSelection();
            searchLookUpEditTenVT.Text = "";
            searchLookUpEditDV.Text = "";
        }

     
        private void ThemMau(string _makh)
        {
            string _mamauvt = "", _mauvt = "", _mauid = "";
            if (flagMau)
            {
                if (string.IsNullOrWhiteSpace(txtMaMauVT.Text) && string.IsNullOrWhiteSpace(txtMauVT.Text))
                {
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchLookUpEditMauVT.Text))
                {
                    return;
                }
            }
            if (flagMau)
            {
                if (!ValidateInput(txtMaMauVT, "Vui lòng điền thông tin màu vật tư!!!") ||
                    !ValidateInput(txtMauVT, "Vui lòng điền thông tin màu vật tư!!!"))
                    return;
                _mamauvt = txtMaMauVT.Text;
                _mauvt = txtMauVT.Text;
            }
            else
            {
                if (!ValidateInput(searchLookUpEditMauVT, "Vui lòng điền thông tin màu vật tư!!!"))
                    return;

                int[] selectedRowHandles = gVSearchMau.GetSelectedRows();
                List<string> mauvtidList = new List<string>();
                List<string> mamauvtList = new List<string>();
                List<string> mauvtList = new List<string>();


                foreach (int rowHandle in selectedRowHandles)
                {
                    string mauvtid = gVSearchMau.GetRowCellValue(rowHandle, "MauID")?.ToString();
                    string mamauvt = gVSearchMau.GetRowCellValue(rowHandle, "MaMauVT")?.ToString();
                    string mauvt = gVSearchMau.GetRowCellValue(rowHandle, "MauVT")?.ToString();


                    if (!string.IsNullOrEmpty(mauvtid)) mauvtidList.Add(mauvtid);
                    if (!string.IsNullOrEmpty(mamauvt)) mamauvtList.Add(mamauvt);
                    if (!string.IsNullOrEmpty(mauvt)) mauvtList.Add(mauvt);

                }
                _mauid = string.Join("|", mauvtidList);
                _mamauvt = string.Join("|", mamauvtList);
                _mauvt = string.Join("|", mauvtList);

            }
            string[] arrMauVTid = tachString(_mauid);
            string[] arrMaMauVT = tachString(_mamauvt);
            string[] arrMauVT = tachString(_mauvt);
            addRowToGridMauVT(arrMauVTid, arrMaMauVT, arrMauVT,_makh);
        }

       

        private void addRowToGridMauVT(string[] arrMauVTid, string[] arrMaMauVT, string[] arrMauVT, string _makh)
        {
            if (arrMaMauVT.Length != arrMauVT.Length)
            {
                XtraMessageBox.Show(" Vui lòng điền đủ thông tin màu vật tư!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (arrMaMauVT.Any(string.IsNullOrWhiteSpace) || arrMauVT.Any(string.IsNullOrWhiteSpace))
            {
              
                return;
            }
            for(int i=0;i< arrMaMauVT.Length;i++)
            {
                DataRow dr = tblGridMauVT.NewRow();
                dr["ID"] = 0;
                dr["MaKH"] = _makh == string.Empty ? "" : _makh;
                dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                dr["MauID"] = (arrMauVTid.Length == 1 || arrMauVTid[0] == "") ? "0" : arrMauVTid[i];
                dr["MaMauVT"] = arrMaMauVT[i];
                dr["MauVT"] = arrMauVT[i];

                tblGridMauVT.Rows.Add(dr);
            }
            gCMauVT.DataSource = tblGridMauVT;
         
            txtMauVT.Text = "";
            txtMaMauVT.Text = "";
      
            (searchLookUpEditMauVT.Properties.View as GridView)?.ClearSelection();
            searchLookUpEditMauVT.Text = "";
        }

        private void searchLookUpEditDV_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            if (edit != null)
            {
               
                List<string> selectedItems = new List<string>();
                foreach (var rowHandle in edit.Properties.View.GetSelectedRows())
                {
                    object value = edit.Properties.View.GetRowCellValue(rowHandle, "TenDVVT");
                    if (value != null)
                    {
                        selectedItems.Add(value.ToString());
                    }
                }

              
                e.DisplayText = string.Join("|", selectedItems);
            }
        }

        private void gVsearchDV_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            searchLookUpEditDV.EditValue = GetSelectedValues(searchLookUpEditDV, "TenDVVT");
            searchLookUpEditDV.DoValidate();
        }

       

        private void comboBoxEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if(flagVT)
            {
            if (cbBoxDV.EditValue.ToString() == "Khai báo mới")
            {
                flagDV = true;
                LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                txtDV.Text = "";
                searchLookUpEditDV.EditValue = null;
            }
            else if (cbBoxDV.EditValue.ToString() == "Đã khai báo")
            {
                flagDV = false;
                LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtDV.Text = "";
                searchLookUpEditDV.EditValue = null;
                CreateSearchLookUpDV();
            }
            }
            else
            {
                LCItxtDV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LCIsearchDonVi.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
        }

        private void ThemKhoVai(string _makh)
        {
            string _khovai = "", _khovaiid = "";
            if (flagKho)
            {
                if (string.IsNullOrWhiteSpace(txtKho.Text))
                {
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchLookUpEditKho.Text))
                {
                    return;
                }
            }
            if (flagKho)
            {
                if (!ValidateInput(txtKho, "Vui lòng điền thông tin khổ vải!!!"))
                    return;
                _khovai = txtKho.Text;
            }
            else
            {
                if (!ValidateInput(searchLookUpEditKho, "Vui lòng điền thông tin khổ vải!!!"))
                    return;
                int[] selectedRowHandles = gVSearchKho.GetSelectedRows();
                List<string> mauvtidList = new List<string>();
                List<string> mamauvtList = new List<string>();



                foreach (int rowHandle in selectedRowHandles)
                {
                    string khovaiid = gVSearchKho.GetRowCellValue(rowHandle, "KhoVaiID")?.ToString();
                    string khovai = gVSearchKho.GetRowCellValue(rowHandle, "KhoVai")?.ToString();



                    if (!string.IsNullOrEmpty(khovaiid)) mauvtidList.Add(khovaiid);
                    if (!string.IsNullOrEmpty(khovai)) mamauvtList.Add(khovai);


                }
                _khovaiid = string.Join("|", mauvtidList);
                _khovai = string.Join("|", mamauvtList);

            }

            string[] arrKhoid = tachString(_khovaiid);
            string[] arrKho = tachString(_khovai);
            addRowToGridKhoVai(arrKhoid, arrKho,_makh);
            txtKho.Text = "";
           
            (searchLookUpEditKho.Properties.View as GridView)?.ClearSelection();
            searchLookUpEditKho.Text = "";
        }
        private void addRowToGridKhoVai(string[] arrKhoid, string[] arrKho, string _makh)
        {
            if ( arrKho.Any(string.IsNullOrWhiteSpace))
            {
                return;
            }
            for(int i=0;i<arrKho.Length;i++)
            {
                DataRow dr = tblGridKhoVai.NewRow();
                dr["ID"] = 0;
                dr["MaKH"] = _makh == string.Empty ? "" : _makh;
                dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();     
                dr["KhoVaiID"] = (arrKhoid.Length == 1 || arrKhoid[0] == "") ? "0" : arrKhoid[i];
                dr["KhoVai"] = arrKho[i];
                tblGridKhoVai.Rows.Add(dr);
            }
            gCKhoVai.DataSource = tblGridKhoVai;
        }
        private bool ValidateInput(Control control, string message)
        {
            if (string.IsNullOrWhiteSpace(control.Text))
            {
                XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        private string[] tachString(string text)
        {
            return text.Trim().Split('|');
        }

        private void CreateSearchLookUpDV()
        {
            searchLookUpEditDV.Properties.ValueMember = "MaDVVT";
            searchLookUpEditDV.Properties.DisplayMember = "TenDVVT";
            searchLookUpEditDV.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            //searchLookUpEditKH.Properties.NullText = "Chọn khách hàng";
            string url = string.Format("{0}?", URL + "ERPThongSoVatTu/GetDV");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblDV = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDV.Properties.DataSource = tblDV;

            searchLookUpEditDV.RefreshEditValue();
            searchLookUpEditDV.Refresh();
          

        }
        private void btnTVKH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmKhachHang frm = new frmKhachHang();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookUpKH();
                CreateSearchLookUpKHCopy();
            };
            frm.ShowDialog();
        }
        private void btnTVMH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmHangHoa frm = new frmHangHoa();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookUpKH();
                CreateSearchLookUpKHCopy();

            };
            frm.ShowDialog();
        }
        private void btnTVNhomNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmNhomNguyenPhuLieu frm = new frmNhomNguyenPhuLieu();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookUpNhomNPL();


            };
            frm.ShowDialog();

        }
        private void btnTVVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmERPVatTu frm = new frmERPVatTu();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookUpVatTu();
               
               
            };
            frm.ShowDialog();
        }
        private void btnTVMVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmERPMaMauVT frm = new frmERPMaMauVT();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookUpMau();
   
            };
            frm.ShowDialog();
        }
        private void btnTVKV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmERPKhoVai frm = new frmERPKhoVai();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookUpKho();
                
            };
            frm.ShowDialog();
        }
        private void btnTVDV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmERPDonViVT frm = new frmERPDonViVT();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookUpDV();
              
            };
            frm.ShowDialog();
        }
        private void repoSearchNhomNPL_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit editor = sender as SearchLookUpEdit;

            if (editor != null)
            {
             
                GridView view = editor.Properties.View;

             
                int rowHandle = view.FocusedRowHandle;

            
                if (rowHandle >= 0)
                {
              
                    DataRow selectedRow = view.GetDataRow(rowHandle);

                    if (selectedRow != null)
                    {
                      
                        string tenNhom = selectedRow["TenNhom"].ToString();
                        string maNhom = selectedRow["MaNhom"].ToString(); 
                        string npl = selectedRow["NPL"].ToString();
                        int focusedRowHandle = gVVatTu.FocusedRowHandle;

                        if (focusedRowHandle >= 0)
                        {
                            // Cập nhật giá trị vào các cột tương ứng
                            gVVatTu.SetRowCellValue(focusedRowHandle, "TenNhom", tenNhom);
                            gVVatTu.SetRowCellValue(focusedRowHandle, "NhomNL", maNhom);
                            gVVatTu.SetRowCellValue(focusedRowHandle, "NPL", npl);
                        }
                    }
                }
            }
            DataTable tbl = gCVatTu.DataSource as DataTable;
        }
    }
   
}
//Vật tư +-40, Màu +-22, còn ẩn hết thì còn 156 