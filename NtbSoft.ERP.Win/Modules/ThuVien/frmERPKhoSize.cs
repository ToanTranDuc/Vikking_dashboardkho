using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERPKhoSize : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblThuVien;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        public frmERPKhoSize()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
          
            CheckPerminsion();
        }
        protected override void OnLoad(EventArgs e)
        {
            tblThuVien = new DataTable();
            CreateTableThuVien();
            loadData();
            CreateSearcLookUpKV();
            this.ActiveControl = button1;
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
                btnThem.Enabled = false;
            }
            if (!_allowEdit && !_allowAdd)
            {

                btnSave.Enabled = false;
                gV.OptionsBehavior.Editable = false;
            }

            if (!_allowDelete)
                btnDelete.Enabled = false;

        }
        private void loadData()
        {
            try
            {
                string url = $"{URL}ERPThuVienVT/GetChungKV?Action=GETKV";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    
                    gC.DataSource = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gC.DataSource = tbl;
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {

            }
        }
        private void CreateTableThuVien()
        {
            tblThuVien = new DataTable("tblThuVien");
            tblThuVien.Columns.Add("ID", typeof(int));
            tblThuVien.Columns.Add("KhoVaiID", typeof(string));
            tblThuVien.Columns.Add("KhoVai", typeof(string));
            tblThuVien.Columns.Add("NPL", typeof(bool));
            tblThuVien.Columns.Add("MaNhom", typeof(string));
            tblThuVien.Columns.Add("GhiChu", typeof(string));

        }
        private void CreateSearcLookUpKV()
        {
            searchLookUpEditNhom.Properties.ValueMember = "MaNhom";
            searchLookUpEditNhom.Properties.DisplayMember = "TenNhom";
            searchLookUpEditLocNhom.Properties.ValueMember = "MaNhom";
            searchLookUpEditLocNhom.Properties.DisplayMember = "TenNhom";

            string url = $"{URL}ERPThuVienVT/GetChungKV?Action=GETNHOM";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditNhom.Properties.DataSource = null;
                searchLookUpEditLocNhom.Properties.DataSource = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditNhom.Properties.DataSource = null;
                searchLookUpEditLocNhom.Properties.DataSource = null;
                return;
            }    
               
            searchLookUpEditNhom.Properties.DataSource = tbl;
            searchLookUpEditLocNhom.Properties.DataSource = tbl;

        }
        private void searchLookUpEdit1View_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn15|| info.Column == gridColumn6)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }


            if (view.IsGroupRow(e.RowHandle))
            {


                int groupIndex = view.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

       

        private void searchLookUpEdit2View_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn16|| info.Column == gridColumn17)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (view.IsGroupRow(e.RowHandle))
            {


                int groupIndex = view.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

      
        private void gV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn4|| info.Column == gridColumn8)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (view.IsGroupRow(e.RowHandle))
            {


                int groupIndex = view.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditNhom.EditValue == null || searchLookUpEditNhom.EditValue.ToString() == "")
                return;
            if (string.IsNullOrWhiteSpace(txtKhoSize.Text.ToString()))
                return;
            DataTable tblGC = gC.DataSource as DataTable;
            DataRow dr = tblGC.NewRow();
            var dataRowSearch = searchLookUpEditNhom.Properties.View.GetFocusedDataRow();
            if (dataRowSearch == null)
                return;
            string khoSizeMoi = txtKhoSize.Text.Trim();
            string maNhom = dataRowSearch["MaNhom"].ToString();
            bool existed = tblGC.AsEnumerable().Any(r =>
                string.Equals(r.Field<string>("MaNhom"), maNhom, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(r.Field<string>("KhoVai"), khoSizeMoi, StringComparison.OrdinalIgnoreCase));
            if (existed)
            {
                MessageBox.Show("Trùng Khổ/Size. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }    
               
         
            dr["MaNhom"] = maNhom;
            dr["TenNhom"] = dataRowSearch["TenNhom"].ToString();
            dr["NPL"] = dataRowSearch["NPL"].ToString();
            dr["LoaiVT"] = dataRowSearch["LoaiVT"].ToString();
            dr["KhoVai"] = khoSizeMoi.ToString();
            dr["GhiChu"] = txtGhiChu.Text.ToString();
            dr["Sort"]= dataRowSearch["Sort"].ToString();
            tblGC.Rows.Add(dr);
            gC.DataSource = tblGC;

            searchLookUpEditNhom.EditValue = null;
            txtKhoSize.Text = "";
            txtGhiChu.Text = "";

        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGC = gC.DataSource as DataTable;
                if (tblGC == null || tblGC.Rows.Count == 0) return;
                bool coCanhBao = checkEmpty(tblGC);
                if (coCanhBao)
                {
                    XtraMessageBox.Show("Dữ liệu trống. Vui lòng kiểm tra lại tên chủng loại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var duplicateRows = GetDuplicateRows(tblGC);
                if (duplicateRows.Any())
                {

                    string message = "Các dòng sau bị trùng:\n";
                    foreach (var row in duplicateRows)
                    {
                        int rowIndex = tblThuVien.Rows.IndexOf(row) + 1;
                        message += $"- Dòng {rowIndex}: {row["TenNhom"]}, {row["KhoVai"]}\n";
                    }
                    XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach(DataRow dr in tblGC.Rows)
                {
                    DataRow nr = tblThuVien.NewRow();
                    nr["ID"] = dr["ID"];
                    nr["KhoVaiID"] = dr["KhoVaiID"];
                    nr["KhoVai"] = dr["KhoVai"];
                    nr["NPL"] = dr["NPL"];
                    nr["MaNhom"] = dr["MaNhom"];
                    nr["GhiChu"] = dr["GhiChu"];
                    tblThuVien.Rows.Add(nr);
                }    
              
                string url = $"{URL}ERPThuVienVT/PostKV?para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblThuVien); }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    loadData();

                }
            }
            catch (Exception ex)
            {


            }
        }
        public bool checkEmpty(DataTable dt)
        {
            bool empty = false;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];

                string maNhom = row["MaNhom"]?.ToString().Trim();
                string khoVai = row["KhoVai"]?.ToString().Trim();


                if (string.IsNullOrWhiteSpace(maNhom) || string.IsNullOrWhiteSpace(khoVai))
                {
                    empty = true;
                }
            }

            return empty;
        }
        private List<DataRow> GetDuplicateRows(DataTable dt)
        {
            List<DataRow> duplicateRows = new List<DataRow>();

            if (dt == null || dt.Rows.Count == 0)
                return duplicateRows;

            var groups = dt.AsEnumerable()
              .GroupBy(row => new
              {
                  MaNhom = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MaNhom"))),
                  KhoVai = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("KhoVai")))
              })
              .Where(g => g.Count() > 1);


            foreach (var group in groups)
            {
                duplicateRows.AddRange(group.Skip(1));
            }
            return duplicateRows;
        }

       

        public string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {

                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'Đ')
                    {
                        stringBuilder.Append('D');
                    }
                    else if (c == 'đ')
                    {
                        stringBuilder.Append('d');
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        

        static string ReplaceSpecialCharacters(string input)
        {
            try
            {
                string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
                string replacement = "_";
                Regex regex = new Regex(pattern);
                return regex.Replace(input, replacement);
            }
            catch (Exception ex)
            {
                return input;

            }

        }
        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gV.GetFocusedDataRow();
                if (dr == null) return;
                //string urlGET = $"{URL}ERPThuVienMau/Get?Action=GETDELETE&para={dr["MauVTID"].ToString()}";
                //string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                //if (jsonGET != "[]")
                //{
                //    XtraMessageBox.Show("Màu này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}
                DataTable tblGC = gC.DataSource as DataTable;
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}ERPThuVienVT/DeleteKV?id={dr["ID"]}&user={GlobleData.UserName}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }
                    tblGC.Rows.Remove(dr);
                    gC.DataSource = tblGC;
                    this.ActiveControl = button1;

                }
            }
            catch (Exception ex)
            {


            }
        }
    }
}