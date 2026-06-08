using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Entity.SYSTEM;
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
    public partial class frmERP_VatTuTV : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        //tam
        private DataTable thongsoVTTable;
        private DataTable chungloaiTable;
        //tam
        DataTable tblThuVien;
        //Thoại tạo biến hiển thị 
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        public frmERP_VatTuTV()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            btnXacNhan.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            CheckPerminsion();
            if (GlobleData.UserName.ToLower() == "admin")
                btnThem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
        }
        public frmERP_VatTuTV(bool ischeck)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            if (ischeck)
            {
                btnXacNhan.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            }
            if (GlobleData.UserName.ToLower() == "admin")
                btnThem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
        }
        protected override void OnLoad(EventArgs e)
        {
            tblThuVien = new DataTable();
            CreateTableThuVien();
            tenNVHienThi = GetTenNhanVien();// khởi tạo lấy ds tên nv
            loadData();
            this.ActiveControl = button1;
        }
        private DataTable CreateTableThuVien()
        {
            tblThuVien = new DataTable("tblThuVien");
            tblThuVien.Columns.Add("ID", typeof(int));
            tblThuVien.Columns.Add("MaVTID", typeof(string));
            tblThuVien.Columns.Add("MaVT", typeof(string));
            tblThuVien.Columns.Add("ChiTiet", typeof(string));
            tblThuVien.Columns.Add("NguoiTao", typeof(string));
            tblThuVien.Columns.Add("NgayTao", typeof(DateTime));
            tblThuVien.Columns.Add("NguoiSua", typeof(string));
            tblThuVien.Columns.Add("NgaySua", typeof(DateTime));
            tblThuVien.Columns.Add("TuoiTonKho", typeof(decimal));
            tblThuVien.Columns.Add("TonToiThieu", typeof(decimal));
            tblThuVien.Columns.Add("TonToiDa", typeof(decimal));
            tblThuVien.Columns.Add("LeadTime", typeof(string));
            return tblThuVien;
        }
        private void loadData()
        {
            try
            {
                string url = $"{URL}ERPVatTuTV/Get?Action=GET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    tblThuVien.Clear();
                    gC.DataSource = null;
                    return;
                }
                tblThuVien = JsonConvert.DeserializeObject<DataTable>(json);

                if (!tblThuVien.Columns.Contains("IsEdit"))
                    tblThuVien.Columns.Add("IsEdit", typeof(int));
                tblThuVien.AsEnumerable().ToList().ForEach(row => row["IsEdit"] = 0);
                tblThuVien.AcceptChanges(); //Thoai them kiem tra Added/Modified



                gC.DataSource = tblThuVien;
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {

            }

        }

        private void btnThem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataRow dr = tblThuVien.NewRow();
                dr["ID"] = 0;
                dr["MaVTID"] = DBNull.Value;
                dr["NguoiTao"] = GlobleData.UserName;
                dr["NgayTao"] = DateTime.Now;
                dr["TuoiTonKho"] = 0;
                tblThuVien.Rows.InsertAt(dr, 0);
                //tblThuVien.Rows.Add(dr);
                gC.DataSource = tblThuVien;
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
                    TenKH = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MaVT"))),
                    SoDienThoai = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("ChiTiet")))
                })
                .Where(g => g.Count() > 1);


            return duplicates.Any();

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
        public string RemoveVietnameseTone2(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;
            result = Regex.Replace(result, "[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            result = Regex.Replace(result, "[ÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴ]", "A");
            result = Regex.Replace(result, "[èéẹẻẽêềếệểễ]", "e");
            result = Regex.Replace(result, "[ÈÉẸẺẼÊỀẾỆỂỄ]", "E");
            result = Regex.Replace(result, "[ìíịỉĩ]", "i");
            result = Regex.Replace(result, "[ÌÍỊỈĨ]", "I");
            result = Regex.Replace(result, "[òóọỏõôồốộổỗơờớợởỡ]", "o");
            result = Regex.Replace(result, "[ÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠ]", "O");
            result = Regex.Replace(result, "[ùúụủũưừứựửữ]", "u");
            result = Regex.Replace(result, "[ÙÚỤỦŨƯỪỨỰỬỮ]", "U");
            result = Regex.Replace(result, "[ỳýỵỷỹ]", "y");
            result = Regex.Replace(result, "[ỲÝỴỶỸ]", "Y");
            result = Regex.Replace(result, "[đ]", "d");
            result = Regex.Replace(result, "[Đ]", "D");
            return result;

        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                if (tblThuVien == null || tblThuVien.Rows.Count == 0) return;
                SaveVatTu(tblThuVien);

            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
        }

        private void SaveVatTu(DataTable dtSource)
        {
            gV.CloseEditor();
            gV.UpdateCurrentRow();

            var duplicateRows = GetDuplicateRows_MaVT_ChiTiet(dtSource);
            if (duplicateRows.Any())
            {
                gV.BeginUpdate();
                try
                {
                    foreach (var row in duplicateRows) dtSource.Rows.Remove(row);
                }
                finally { gV.EndUpdate(); }
                /*string message = "Các dòng sau bị trùng lặp và sẽ được [BỎ QUA] khi lưu:\n";
                foreach (var row in duplicateRows)
                {
                    int rowIndex = dtSource.Rows.IndexOf(row) + 1;
                    message += $"- Dòng {rowIndex}: {row["MaVT"]}, {row["ChiTiet"]}\n";
                }
                XtraMessageBox.Show(message, "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);*/
            }
            DataTable dtSend = CreateTableThuVien();
            dtSend.Columns.Remove("TonToiThieu");
            dtSend.Columns.Remove("TonToiDa");
            for (int i = 0; i < gV.RowCount; i++)
            {
                DataRow row = gV.GetDataRow(i);
                if (row == null) continue;

                if (row["IsEdit"].ToString() == "0")
                    continue;
                if (string.IsNullOrWhiteSpace(row["MaVT"]?.ToString()) || string.IsNullOrWhiteSpace(row["ChiTiet"]?.ToString()))
                {
                    XtraMessageBox.Show($"Dữ liệu trống ở hàng {i + 1}. Vui lòng điền đủ thông tin 'Itemcode' và 'Mô tả'.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    gV.FocusedRowHandle = i;
                    gV.FocusedColumn = gV.Columns["MaVT"];
                    gV.ShowEditor();
                    return;
                }

                DataRow newR = dtSend.NewRow();

                newR["ID"] = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]);
                newR["MaVTID"] = row["MaVTID"];
                newR["MaVT"] = row["MaVT"]?.ToString() ?? "";
                newR["ChiTiet"] = row["ChiTiet"]?.ToString() ?? "";
                newR["TuoiTonKho"] = row["TuoiTonKho"];
                newR["LeadTime"] = row["LeadTime"];

                newR["NguoiTao"] = GlobleData.UserName;
                newR["NgayTao"] = DateTime.Now;

                newR["NguoiSua"] = GlobleData.UserName;
                newR["NgaySua"] = DateTime.Now;


                dtSend.Rows.Add(newR);
            }
            if (dtSend.Rows.Count == 0)
            {
                //clsWaitForm.ShowSuccessFormCustom(this, 2000, "Không có dữ liệu thay đổi!");
                loadData();
                return;
            }
            clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_VatTuTV");
            string url = $"{URL}ERPVatTuTV/Post?Action=POST";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSend); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                loadData();
            }
            else
            {
                return;
                //XtraMessageBox.Show("Lỗi khi lưu: " + msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<DataRow> GetDuplicateRows_MaVT_ChiTiet(DataTable dt)
        {
            List<DataRow> duplicateRows = new List<DataRow>();

            if (dt == null || dt.Rows.Count == 0)
                return duplicateRows;

            var groups = dt.AsEnumerable()
              .GroupBy(row => new
              {
                  MaVT = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MaVT"))),
                  ChiTiet = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("ChiTiet")))
              })
              .Where(g => g.Count() > 1);

            // Lấy tất cả bản ghi trùng từ bản thứ 2 trở đi (bỏ bản ghi đầu)
            foreach (var group in groups)
            {
                duplicateRows.AddRange(group.Skip(1));
            }
            return duplicateRows;
        }
        public DataRow getMau()
        {
            DataRow dr = gV.GetFocusedDataRow();
            if (dr == null)
            {
                return null;
            }
            return dr;
        }
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gV.GetFocusedDataRow();
                if (dr == null) return;
                string urlGET = $"{URL}ERPVatTuTV/Get?Action=GETDELETE&para={dr["MaVTID"].ToString()}";
                string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                if (jsonGET != "[]")
                {
                    XtraMessageBox.Show("Vật tư này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}ERPVatTuTV/Delete?Action=DELETE&para={dr["ID"]}&para2={GlobleData.UserName}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                        }
                    }
                    tblThuVien.Rows.Remove(dr);
                    gC.DataSource = tblThuVien;
                    this.ActiveControl = button1;

                }
            }
            catch (Exception ex)
            {


            }
        }

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadData();
        }

        private void gV_KeyPress(object sender, KeyPressEventArgs e)
        {
            //GridView gridView = sender as GridView;

            //if (gridView != null && gridView.FocusedColumn != null)
            //{
            //    string columnName = gridView.FocusedColumn.FieldName;
            //    if (columnName == "MaVT" || columnName == "MaVT")
            //    {
            //        if (char.IsControl(e.KeyChar)) return;

            //        // Chỉ quan tâm chữ cái
            //        if (char.IsLetter(e.KeyChar))
            //        {
            //            // Upper ký tự
            //            char upperChar = char.ToUpper(e.KeyChar);

            //            // Loại bỏ dấu
            //            string converted = RemoveVietnameseTone2(upperChar.ToString());

            //            // Nếu ký tự có dấu, replace bằng không dấu
            //            if (upperChar.ToString() != converted)
            //            {
            //                // Thay bằng không dấu và upper luôn
            //                e.KeyChar = converted.ToUpper()[0];
            //            }
            //            else
            //            {
            //                e.KeyChar = upperChar;
            //            }
            //        }
            //    }
            //}
        }

        private void gC_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gV_KeyPress(grid.FocusedView, e);
        }

        public bool KiemTraVaCanhBao(DataTable dt)
        {
            bool canhBao = false;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];

                string mamauvt = row["MaVT"]?.ToString().Trim();
                string mauvt = row["ChiTiet"]?.ToString().Trim();





                if (string.IsNullOrWhiteSpace(mamauvt) || string.IsNullOrWhiteSpace(mauvt))
                {
                    canhBao = true;
                }
            }

            return canhBao;
        }
        #region phân quyền
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;

        private void gV_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }



        private void btnXacNhan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
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
                btnThem1.Enabled = false;
            }
            if (!_allowEdit && !_allowAdd)
            {

                btnLuu.Enabled = false;
                gV.OptionsBehavior.Editable = false;
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
        #region Thoai hiển thị username -> tên
        private void gV_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == gridColNguoiTao || e.Column == gridColNguoiSua)
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
        }
        #endregion
        
        private void gVThuVien_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow row_focus = gV.GetFocusedDataRow() as DataRow;

            if (row_focus != null)
            {
                int.TryParse(row_focus["ID"]?.ToString(), out int ID);
                if (ID > 0)
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
                #region Thoai hiển thị username -> tên
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
                #endregion

            }
        }


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
                if (decimalPart.Length >= 2 && e.KeyChar != '\b') // '\b' là phím Backspace
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
        private void btnNhapTonKho_Click(object sender, EventArgs e)
        {
            decimal _tuoi = 0;
            decimal.TryParse(txtTuoiTonKho.Text.ToString(), out _tuoi);
            GridView gridView = gC.MainView as GridView;




            if (gridView != null)
            {
                // Duyệt qua các dòng đang hiển thị (sau khi filter)
                for (int i = 0; i < gridView.DataRowCount; i++)
                {
                    // Sử dụng SetRowCellValue để kích hoạt sự kiện CellValueChanged
                    gridView.SetRowCellValue(i, "TuoiTonKho", _tuoi);
                }
            }
        }

        private void gV_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = gV.GetFocusedDataRow();
            if (dr == null) return;
            dr["IsEdit"] = 1;
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            string searchText = textEdit1.Text.Trim();

            FilterGridView(gV, searchText);
        }

        private void FilterGridView(DevExpress.XtraGrid.Views.Grid.GridView gridView, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                gridView.ActiveFilterString = "";
                return;
            }

            List<string> conditions = new List<string>();

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView.Columns)
            {
                if (col.Visible)
                {
                    // Escape dấu nháy đơn để tránh lỗi SQL
                    string escapedText = searchText.Replace("'", "''");
                    conditions.Add($"Contains([{col.FieldName}], '{escapedText}')");
                }
            }

            if (conditions.Count > 0)
            {
                gridView.ActiveFilterString = string.Join(" OR ", conditions);
            }
        }

        #region Tam
        private void clBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new frmERP_VatTuMinMax(1))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        clsWaitForm.ShowSuccessForm(this, 3000);
                        loadData();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void vtBtn_Click(object sender, EventArgs e)
        {
            GridView view = gV;
            GridColumn colMaVTID = view.Columns["MaVTID"];
            GridColumn colMaVT = view.Columns["MaVT"];

            string vtid = null;
            string mavt = null;

            int focusedHandle = view.FocusedRowHandle;

            if (focusedHandle >= 0)
            {
                // Data row bình thường
                vtid = view.GetFocusedRowCellValue(colMaVTID)?.ToString();
                mavt = view.GetFocusedRowCellValue(colMaVT)?.ToString();
            }
            else return;

            try
            {
                using (var frm = new frmERP_VatTuMinMax(0, vtid, mavt))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        clsWaitForm.ShowSuccessForm(this, 3000);
                        loadData();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}