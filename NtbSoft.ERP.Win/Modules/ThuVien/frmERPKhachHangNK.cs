using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NtbSoft.ERP.Win.Utils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Entity.SYSTEM;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERPKhachHangNK : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblThuVien;
        public frmERPKhachHangNK()
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
            this.ActiveControl = simpleButton1;
        }

        private void CreateTableThuVien()
        {
            tblThuVien = new DataTable("tblThuVien");
            tblThuVien.Columns.Add("ID", typeof(int));
            tblThuVien.Columns.Add("MaKH", typeof(string));
            tblThuVien.Columns.Add("TenKH", typeof(string));
            tblThuVien.Columns.Add("DiaChi", typeof(string));
            tblThuVien.Columns.Add("SoDienThoai", typeof(string));
            tblThuVien.Columns.Add("NguoiDaiDien", typeof(string));
            tblThuVien.Columns.Add("MaSoThue", typeof(string));
            tblThuVien.Columns.Add("LoaiDT", typeof(bool));

        }
        private void loadData()
        {
            try
            {
                string url = $"{URL}ERPThuVienNK/Get?Action=GETKH";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    tblThuVien.Clear();
                    gCThuVien.DataSource = null;
                    return;
                }
                tblThuVien = JsonConvert.DeserializeObject<DataTable>(json);
                gCThuVien.DataSource = tblThuVien;
                this.ActiveControl = simpleButton1;
            }
            catch (Exception ex)
            {


            }

        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataRow dr = tblThuVien.NewRow();
                dr["ID"] = 0;
                dr["LoaiDT"] = false;
                tblThuVien.Rows.InsertAt(dr, 0);
                gCThuVien.DataSource = tblThuVien;
            }

            catch (Exception ex)
            {


            }

        }
        public bool KiemTraVaCanhBao(DataTable dt)
        {
            bool canhBao = false;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];

                string tenKH = row["TenKH"]?.ToString().Trim();
                string soDienThoai = row["SoDienThoai"]?.ToString().Trim();
                string diaChi = row["DiaChi"]?.ToString().Trim();
                string nguoiDaiDien = row["NguoiDaiDien"]?.ToString().Trim();
                string maSoThue = row["MaSoThue"]?.ToString().Trim();

                bool coTenKH = !string.IsNullOrEmpty(tenKH);
                bool coDuLieuKhac = !string.IsNullOrEmpty(soDienThoai) || !string.IsNullOrEmpty(diaChi) ||
                                    !string.IsNullOrEmpty(nguoiDaiDien) || !string.IsNullOrEmpty(maSoThue);

                if (string.IsNullOrEmpty(tenKH) && !coDuLieuKhac)
                {

                    dt.Rows.RemoveAt(i);
                }
                else if (string.IsNullOrEmpty(tenKH) && coDuLieuKhac)
                {
                    canhBao = true;
                }
            }

            return canhBao;
        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                //if (checkDup(tblThuVien))
                //{
                //    XtraMessageBox.Show("Có khách hàng bị trùng. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                var duplicateRows = GetDuplicateRows(tblThuVien);
                if (duplicateRows.Any())
                {

                    string message = "Các dòng sau bị trùng:\n";
                    foreach (var row in duplicateRows)
                    {
                        int rowIndex = tblThuVien.Rows.IndexOf(row) + 1; // +1 để cho user dễ hiểu (dòng bắt đầu từ 1)
                        message += $"- Dòng {rowIndex}: {row["TenKH"]}, {row["SoDienThoai"]}\n";
                    }
                    XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                bool coCanhBao = KiemTraVaCanhBao(tblThuVien);
                if (coCanhBao)
                {
                    Console.WriteLine("Có dòng bị thiếu thông tin Khách hàng");
                    XtraMessageBox.Show("Có dòng bị thiếu thông tin Khách hàng. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                foreach (DataRow dr in tblThuVien.Rows)
                {
                    if (string.IsNullOrWhiteSpace(dr["MaSoThue"].ToString()))
                    {
                        XtraMessageBox.Show("Có dòng bị thiếu thông tin Mã số thuế. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }


                clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_KhachHangNK");
                string url = $"{URL}ERPThuVienNK/PostKH?Action=POSTKH";
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
        private List<DataRow> GetDuplicateRows(DataTable dt)
        {
            List<DataRow> duplicateRows = new List<DataRow>();

            if (dt == null || dt.Rows.Count == 0)
                return duplicateRows;

            var groups = dt.AsEnumerable()
              .GroupBy(row => new
              {
                  TenKH = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("TenKH"))),
                  SoDienThoai = row["MaSoThue"]?.ToString()
              })
              .Where(g => g.Count() > 1);

            foreach (var group in groups)
            {
                duplicateRows.AddRange(group.Skip(1));
            }
            return duplicateRows;
        }
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gVThuVien.GetFocusedDataRow();
                if (dr == null) return;

                string makh = dr["MaKH"].ToString();
                string urlCheck = $"{URL}ERPThuVienNK/Get?Action=GETCHECKKHNK&para={makh}";
                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                if (jsonCheck != "[]")
                {
                    XtraMessageBox.Show("Khách hàng này đã được sử dụng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}ERPThuVienNK/Delete?Action=DELETEKH&para={dr["ID"]}&para2={GlobleData.UserName}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }
                    tblThuVien.Rows.Remove(dr);
                    gCThuVien.DataSource = tblThuVien;
                    this.ActiveControl = simpleButton1;

                }
            }
            catch (Exception ex)
            {


            }


        }
        private bool checkDup(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return false;


            var duplicates = dt.AsEnumerable()
                .GroupBy(row => new
                {
                    TenKH = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("TenKH"))),
                    SoDienThoai = row.Field<string>("SoDienThoai")
                })
                .Where(g => g.Count() > 1);


            return duplicates.Any();

        }
        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadData();
        }
        private string ReplaceSpecialCharacters(string input)
        {
            try
            {
                if (input == null) return input;
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
        private string RemoveVietnameseTone(string text)
        {
            try
            {
                if (text == null) return text;
                string result = text.ToLower();
                result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
                result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
                result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
                result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
                result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
                result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
                result = Regex.Replace(result, "đ", "d");
                return result.ToUpper();
            }
            catch (Exception ex)
            {

                return text;
            }

        }
        #region phân quyền
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;

        private void gCThuVien_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gVNL_KeyPress(grid.FocusedView, e);
        }

        private void gVThuVien_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView != null && gridView.FocusedColumn != null)
            {
                string columnName = gridView.FocusedColumn.FieldName;
                if (columnName == "TenKH" || columnName == "TenKH")
                {
                    if (Char.IsLetter(e.KeyChar))

                        e.KeyChar = Char.ToUpper(e.KeyChar);
                }
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
                btnThem.Enabled = false;
            }
            if (!_allowEdit && !_allowAdd)
            {

                btnLuu.Enabled = false;

            }

            if (!_allowDelete)
                btnXoa.Enabled = false;

        }

        #endregion
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
        private void gridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            
            DataRow row_focus = gVThuVien.GetFocusedDataRow() as DataRow;

            if (row_focus != null )
            {
                int.TryParse(row_focus["ID"]?.ToString(), out int ID);
                if(ID > 0)
                {
                    string content = clsWriteLogThuVienLib.FormatRow(row_focus);
                    var Query = lstLog.FirstOrDefault(x => x.ID == ID);
                    if (Query != null)
                    {
                        Query.Content = content;
                    }
                    else
                    {
                        lstLog.Add(new LogThuvienEntity
                        {
                            ID = ID,
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
}
