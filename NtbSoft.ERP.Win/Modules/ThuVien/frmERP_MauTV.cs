using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_MauTV : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        string Host = string.Empty; bool indicatorIcon = true;
        private HttpClientExtension _clientExtension;
        DataTable tblThuVien = new DataTable();
        DataTable tblTheMau = new DataTable();
        DataTable tblSave = new DataTable();
        List<dynamic> imageList = new List<dynamic>();
        List<int> indicesToRemove = new List<int>();
        //private Dictionary<int, object> imageCache = new Dictionary<int, object>();
        int TagMau_focused = 0;
        //Thoại tạo biến hiển thị username -> Tên
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        //private 
        public frmERP_MauTV()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            Host = (string)settingsReader.GetValue("HostDH", typeof(String));
            _clientExtension = new HttpClientExtension();
            btnXacNhan.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            CheckPerminsion();
        }
        public frmERP_MauTV(bool ischeck)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            Host = (string)settingsReader.GetValue("Host", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            if (ischeck)
            {
                btnXacNhan.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            }
        }
        protected override void OnLoad(EventArgs e)
        {
            SetupUI();
            tblThuVien = new DataTable();
            tenNVHienThi = GetTenNhanVien();
            CreateTableTheMau();
            CreateTableThuVien();
            LoadTheMau();

            //loadData();
            grvTheMau.OptionsSelection.MultiSelect = true;
            grvTheMau.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            gV.OptionsSelection.MultiSelect = true;
            gV.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            //this.grvTheMau.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.grvTheMau_PopupMenuShowing);
        }
        private void SetupUI()
        {
            // Tạo PictureEdit repository
            var riPicture = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            riPicture.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            riPicture.ShowMenu = true;
            riPicture.CustomHeight = 50;
            riPicture.NullText = " ";

            colImage.ColumnEdit = riPicture;
            colImage.MinWidth = 50;
            colImage.Width = 50;
            bandImg.Width = 50;
            bandImg.MinWidth = 50;

            bandHex.MinWidth = 50;
            bandHex.Width = 50;
            colHexCompile.Width = 50;
            colHexCompile.MinWidth = 50;

            colImage.UnboundType = DevExpress.Data.UnboundColumnType.Object;

            gV.OptionsBehavior.EditorShowMode = EditorShowMode.Click; // Tránh auto-edit
            //gV.RowHeight = 40;
        }
        private void CreateTableTheMau()
        {
            tblTheMau = new DataTable("tblTheMau");
            tblTheMau.Columns.Add("TheMau", typeof(string));
            tblTheMau.Columns.Add("MaTheMau", typeof(string));
            tblTheMau.Columns.Add("IsUse", typeof(string));
            tblTheMau.Columns.Add("IsEdit", typeof(string));
        }
        private void CreateTableThuVien()
        {
            tblThuVien = new DataTable("tblThuVien");
            tblThuVien.Columns.Add("ID", typeof(int));
            tblThuVien.Columns.Add("MauVTID", typeof(string));
            tblThuVien.Columns.Add("MaMauVT", typeof(string));
            tblThuVien.Columns.Add("MauVT", typeof(string));
            tblThuVien.Columns.Add("TenKhac", typeof(string));
            tblThuVien.Columns.Add("CorelDraw", typeof(string));
            tblThuVien.Columns.Add("Hex", typeof(string));
            tblThuVien.Columns.Add("CMYK", typeof(string));
            tblThuVien.Columns.Add("GhiChu", typeof(string));
            
            tblThuVien.Columns.Add("UserName", typeof(string));
            tblThuVien.Columns.Add("MaTheMau", typeof(string));
            tblThuVien.Columns.Add("TheMau", typeof(string));
            tblThuVien.Columns.Add("Image", typeof(string));
            tblThuVien.Columns.Add("IsUse", typeof(string));
            tblThuVien.Columns.Add("IsEdit", typeof(string));
            tblThuVien.Columns.Add("NguoiTao", typeof(string));
            tblThuVien.Columns.Add("NgayTao", typeof(DateTime));
            tblThuVien.Columns.Add("NguoiSua", typeof(string));
            tblThuVien.Columns.Add("NgaySua", typeof(DateTime));
        }
        private void LoadTheMau()
        {
            try
            {
                tblTheMau.Clear();
                tblThuVien.Clear();
                string url = $"{URL}ERPThuVienMau/Get?Action=GetTagColor";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {

                    grcTheMau.DataSource = null;

                    gC.DataSource = null;

                    return;
                }
                tblTheMau = JsonConvert.DeserializeObject<DataTable>(json);
                //Thoai
                if (tblTheMau != null)
                {
                    tblTheMau.AcceptChanges(); // Để theo dõi RowState (Added, Modified)
                }
                grcTheMau.DataSource = tblTheMau;

            }
            catch (Exception ex)
            {
            }

        }
        private void loadData(string MaTheMau)
        {
            try
            {
                string url = $"{URL}ERPThuVienMau/Get?Action=GET&para={MaTheMau}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    tblThuVien.Clear();
                    gC.DataSource = null;
                    grcTheMau.RefreshDataSource();
                    return;
                }

                tblThuVien = JsonConvert.DeserializeObject<DataTable>(json);
                //Thoai
                if (tblThuVien != null)
                {
                    tblThuVien.AcceptChanges(); // Để theo dõi RowState (Added, Modified)
                }
                gC.DataSource = tblThuVien;
                this.ActiveControl = button1;
                grvTheMau.FocusedRowHandle = TagMau_focused;
            }
            catch (Exception ex){}
        }

        private List<DataRow> GetDuplicateRows(DataTable dt)
        {
            List<DataRow> duplicateRows = new List<DataRow>();

            if (dt == null || dt.Rows.Count == 0)
                return duplicateRows;

            var groups = dt.AsEnumerable()
              .GroupBy(row => new
              {
                  MaMauVT = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MaMauVT"))),
                  MauVT = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MauVT")))
              })
              .Where(g => g.Count() > 1);


            foreach (var group in groups)
            {
                duplicateRows.AddRange(group.Skip(1));
            }
            return duplicateRows;
        }
        private bool checkDup(DataTable dt)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                    return false;


                var duplicates = dt.AsEnumerable()
                    .GroupBy(row => new
                    {
                        TenKH = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MaMauVT"))),
                        SoDienThoai = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MauVT")))
                    })
                    .Where(g => g.Count() > 1);


                return duplicates.Any();
            }
            catch (Exception ex)
            {
                return true;
            }
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
            DeleteMauVT();
        }

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadTheMau();
        }

        private void gV_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView != null && gridView.FocusedColumn != null)
            {
                string columnName = gridView.FocusedColumn.FieldName;
                if (columnName == "MaMauVT" || columnName == "MauVT")
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

        public bool KiemTraVaCanhBao(DataTable dt)
        {
            bool canhBao = false;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];

                string mamauvt = row["MaMauVT"]?.ToString().Trim();
                string mauvt = row["MauVT"]?.ToString().Trim();





                if (string.IsNullOrWhiteSpace(mamauvt) || string.IsNullOrWhiteSpace(mauvt))
                {
                    canhBao = true;
                }
            }

            return canhBao;
        }

        #region phân quyền
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;


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

        private void grvTheMau_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {
                string MaTheMau = view.GetRowCellValue(view.FocusedRowHandle, colMaTheMau)?.ToString();
                bool IsAddNewRow = tblTheMau.AsEnumerable().Any(x => x["MaTheMau"]?.ToString() == "NONE" && x["MaTheMau"]?.ToString() != MaTheMau);
                if (string.IsNullOrEmpty(MaTheMau))
                    return;

                //if (IsAddNewRow)
                //{
                //    ShowWarning("Vui lòng lưu chi tiết màu vật tư");
                //    return;
                //}

                TagMau_focused = e.FocusedRowHandle;
                ClearImageCache();


                try
                {
                    if (MaTheMau != "NONE")
                    {
                        loadData(MaTheMau);
                    }
                    else
                    {
                        tblThuVien.Clear();
                        gC.DataSource = tblThuVien;
                        gV.UpdateCurrentRow();
                    }

                }
                catch (Exception ex)
                {

                    view.FocusedRowHandle = TagMau_focused - 1; // Hoặc giá trị phù hợp
                }
                finally
                {

                }
            }
        }

        private void grvTheMau_DataSourceChanged(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view != null)
            {

                string MaTheMau = view.GetRowCellValue(0, colMaTheMau)?.ToString();
                loadData(MaTheMau);
                view.FocusedRowHandle = 0;
            }
        }

        private void gV_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // ---- CỘT HEX ----
            if (e.Column == colHexCompile)
            {
                try
                {
                    var hex = e.CellValue?.ToString();
                    if (string.IsNullOrEmpty(hex))
                        return;

                    Color color = ColorTranslator.FromHtml(hex);
                    using (SolidBrush backBrush = new SolidBrush(e.Appearance.BackColor))
                        e.Graphics.FillRectangle(backBrush, e.Bounds);

                    int padding = 2;
                    int colorHeight = 50;
                    Rectangle colorRect = new Rectangle(
                        e.Bounds.X + padding,
                        e.Bounds.Y + padding,
                        e.Bounds.Width - 2 * padding,
                        Math.Min(colorHeight, e.Bounds.Height - 2 * padding)
                    );

                    using (SolidBrush brush = new SolidBrush(color))
                        e.Graphics.FillRectangle(brush, colorRect);

                    e.Graphics.DrawRectangle(Pens.DarkGray, colorRect);

                    e.Handled = true;
                }
                catch
                {

                }
            }
        }


        // Fields (giữ nguyên)
        private Dictionary<int, Image> imageCache = new Dictionary<int, Image>();
        private HashSet<int> loadingRows = new HashSet<int>();
        private SemaphoreSlim loadSem = new SemaphoreSlim(Environment.ProcessorCount);

        private void gV_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column != colImage) return;

            int rowIndex = e.ListSourceRowIndex;

            if (e.IsGetData)
            {
                if (imageCache.TryGetValue(rowIndex, out Image img))
                {
                    e.Value = img;
                    return;
                }

                string url = tblThuVien.Rows[rowIndex]["Image"]?.ToString();
                if (string.IsNullOrEmpty(url))
                {
                    e.Value = null;
                    return;
                }

                e.Value = null;

                if (!loadingRows.Contains(rowIndex))
                {
                    loadingRows.Add(rowIndex);
                    Task.Run(() => LoadImageAsync(rowIndex, url));
                }
            }
        }

        private async Task LoadImageAsync(int rowIndex, string url)
        {
            await loadSem.WaitAsync();
            try
            {
                if (rowIndex < 0 || rowIndex >= tblThuVien.Rows.Count) return;

                Image img = await LoadImageFromUrlAsync(url);
                if (img == null) return;

                this.BeginInvoke(new MethodInvoker(delegate
                {
                    imageCache[rowIndex] = img;

                    int rowHandle = gV.GetRowHandle(rowIndex);
                    if (gV.IsValidRowHandle(rowHandle))
                    {
                        gV.RefreshRow(rowHandle);
                        gV.RefreshRowCell(rowHandle, colImage);  // Force repaint image ngay
                    }
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Load error: " + ex.Message);
            }
            finally
            {
                loadSem.Release();
                loadingRows.Remove(rowIndex);
            }
        }

        private async Task<Image> LoadImageFromUrlAsync(string url)
        {
            try
            {
                using (HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
                {
                    string fullUrl = new Uri(new Uri(Host), url).ToString();
                    byte[] data = await http.GetByteArrayAsync(fullUrl);

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        Image origin = Image.FromStream(ms, true);
                        Bitmap resized = new Bitmap(50, 50);
                        using (Graphics g = Graphics.FromImage(resized))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.DrawImage(origin, new Rectangle(0, 0, 50, 50), 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel);
                        }
                        origin.Dispose();
                        return resized;
                    }
                }
            }
            catch
            {
                return null;
            }
        }


        /*   private void btnThem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
                {
                    try
                    {
                        this.ActiveControl = button1;
                        object MaTheMau = grvTheMau.GetRowCellValue(grvTheMau.FocusedRowHandle, colMaTheMau);
                        string TheMau = grvTheMau.GetRowCellValue(grvTheMau.FocusedRowHandle, colTheMau)?.ToString();
                        if (string.IsNullOrEmpty(TheMau))
                        {
                            XtraMessageBox.Show("Vui lòng nhập thẻ màu trước rôi mới thêm chi tiết!", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                            return;
                        }

                        DataRow dr = tblThuVien.NewRow();
                        dr["ID"] = 0;
                        dr["MauVTID"] = DBNull.Value;
                        dr["MaTheMau"] = string.IsNullOrEmpty(MaTheMau?.ToString()) ? "NONE" : MaTheMau?.ToString();
                        dr["TheMau"] = TheMau;
                        dr["IsEdit"] = true;
                        dr["UserName"] = GlobleData.UserName;
                        tblThuVien.Rows.InsertAt(dr, 0);


                        gC.DataSource = tblThuVien;
                    }

                    catch (Exception ex)
                    {


                    }
                }*/
        private void btnThem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            string TheMau = grvTheMau.GetRowCellValue(grvTheMau.FocusedRowHandle, colTheMau)?.ToString();

            if (string.IsNullOrEmpty(TheMau))
            {
                XtraMessageBox.Show("Vui lòng nhập thẻ màu!", "Cảnh Báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddNewMau(TheMau);
        }

        private void AddNewMau(string TheMau)
        {

            object MaTheMau = grvTheMau.GetRowCellValue(grvTheMau.FocusedRowHandle, colMaTheMau);

            DataRow dr = tblThuVien.NewRow();
            dr["ID"] = 0;
            dr["MauVTID"] = DBNull.Value;
            dr["MaTheMau"] = MaTheMau?.ToString() ?? "NONE";
            dr["TheMau"] = TheMau;
            dr["IsEdit"] = true;
            dr["UserName"] = GlobleData.UserName;
            dr["NguoiTao"] = GlobleData.UserName;
            //dr["NgayTao"] = DateTime.Now;
            dr["NguoiSua"] = DBNull.Value;
           // dr["NgaySua"] = DBNull.Value;


            tblThuVien.Rows.InsertAt(dr, 0);
            gC.DataSource = tblThuVien;
            var newImageCache = new Dictionary<int, Image>();
            foreach (var kvp in imageCache)
            {
                int newKey = kvp.Key + 1;  // Shift +1 vì insert tại 0
                newImageCache[newKey] = kvp.Value;
            }
            imageCache.Clear();
            imageCache = newImageCache;  // Hoặc gán trực tiếp nếu C# 4.5 hỗ trợ

            var newLoadingRows = new HashSet<int>();
            foreach (int oldKey in loadingRows)
            {
                int newKey = oldKey + 1;
                newLoadingRows.Add(newKey);
            }
            loadingRows.Clear();
            loadingRows = newLoadingRows;

            gV.BeginUpdate();
            gV.RefreshData();
            gV.EndUpdate();
        }

        private void gV_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            DevExpress.XtraGrid.Columns.GridColumn focused_col = gV.FocusedColumn;
            //if(gV.FocusedColumn == colDelete)
            //{
            //    try
            //    {

            //        DataRow dr = gV.GetFocusedDataRow();
            //        if (dr == null) return;
            //        string urlGET = $"{URL}ERPThuVienMau/Get?Action=GETDELETE&para={dr["MauVTID"].ToString()}";
            //        string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
            //        if (jsonGET != "[]")
            //        {
            //            XtraMessageBox.Show("Màu này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }

            //        DialogResult messResult = MessageBox.Show($"Bạn có muốn xóa Màu: {dr["MauVT"]} - Code Màu : {dr["MaMauVT"]} này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (messResult == DialogResult.Yes)
            //        {

            //            if (dr["ID"].ToString() != "0")
            //            {
            //                string url = $"{URL}ERPThuVienMau/Delete?Action=DELETE&para={dr["ID"]}&para2={GlobleData.UserName}";
            //                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            //                if (result.ToLower() == "true")
            //                {
            //                    clsWaitForm.ShowSuccessForm(this, 1000);
            //                }
            //            }
            //            tblThuVien.Rows.Remove(dr);
            //            gC.DataSource = tblThuVien;
            //            this.ActiveControl = button1;

            //        }
            //    }
            //    catch (Exception ex)
            //    {


            //    }
            //}
            //else
            if (focused_col == colHexCompile || focused_col == colHex || focused_col == colCMYK)
            {
                string HexColorFocused = gV.GetRowCellValue(e.RowHandle, colHex)?.ToString();
                frmSelectColor frm = new frmSelectColor(HexColorFocused);
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    gV.SetRowCellValue(e.RowHandle, colHexCompile, frm.Hex);
                    gV.SetRowCellValue(e.RowHandle, colHex, frm.Hex);
                    gV.SetRowCellValue(e.RowHandle, colCMYK, frm.CMYK);
                    gV.SetRowCellValue(e.RowHandle, colIsEdit, true);
                    gV.UpdateCurrentRow();
                }
            }
            else if (focused_col == colImage)
            {
                string CodeMau = gV.GetRowCellValue(e.RowHandle, colCodeMau)?.ToString();
                string TenMau = gV.GetRowCellValue(e.RowHandle, colTenMau)?.ToString();
                if (string.IsNullOrEmpty(CodeMau) || string.IsNullOrEmpty(TenMau))
                {
                    ShowWarning("Vui lòng nhập <b>tên màu</b> và <b>Code màu</b> trước!!");
                    return;
                }

                using (var dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.webp;*.svg";
                    dialog.Multiselect = false;
                    if (dialog.ShowDialog(this) != DialogResult.OK) return;

                    Image resizedImage = null;
                    bool success = false;
                    try
                    {
                        // Kiểm tra kích thước file (>50MB)
                        FileInfo fileInfo = new FileInfo(dialog.FileName);
                        if (fileInfo.Length > 50 * 1024 * 1024)
                        {
                            MessageBox.Show("File ảnh quá lớn (>50MB). Vui lòng chọn ảnh nhỏ hơn.");
                            return;
                        }

                        // Load và resize thủ công (an toàn hơn GetThumbnailImage)
                        using (var origin = Image.FromFile(dialog.FileName))
                        {
                            resizedImage = new Bitmap(50, 50);
                            using (var g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(origin, new Rectangle(0, 0, 50, 50), 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel);
                            }
                        }

                        // Gán vào cache (không SetRowCellValue cho unbound)
                        int dataSourceIndex = gV.GetDataSourceRowIndex(e.RowHandle);
                        imageCache[dataSourceIndex] = resizedImage;

                        // Mark edited nếu thành công
                        gV.SetRowCellValue(e.RowHandle, colIsEdit, true);
                        success = true;
                    }
                    catch (OutOfMemoryException)
                    {
                        MessageBox.Show("Kích thước ảnh quá lớn. Thử ảnh nhỏ hơn.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi load và resize ảnh: {ex.Message}");
                    }

                    // Refresh row để trigger CustomUnboundColumnData và hiển thị ảnh ngay (fix không chờ focus)
                    if (success && gV.IsValidRowHandle(e.RowHandle))
                    {
                        gV.RefreshRow(e.RowHandle);
                    }
                    else
                    {
                        // Fallback nếu lỗi: Clear cache và mark not edited
                        int dataSourceIndex = gV.GetDataSourceRowIndex(e.RowHandle);
                        imageCache.Remove(dataSourceIndex);
                        gV.SetRowCellValue(e.RowHandle, colIsEdit, false);
                    }

                    // Không dispose resizedImage - để cache/Grid manage
                }
            }
        }


        /*Write log when modify row*/
        List<LogThuvienEntity> lstLog = new List<LogThuvienEntity>();

        private void gV_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow row_focus = gV.GetFocusedDataRow() as DataRow;

            if (e.RowHandle >= 0 && !string.IsNullOrEmpty(e.Value?.ToString()) && e.Column != colIsEdit)
            {
                #region Comment code
                //int.TryParse(row_focus["ID"]?.ToString(), out int ID);
                //if (ID > 0)
                //{
                //    string content = clsWriteLogThuVienLib.FormatRow(row_focus);
                //    var Query = lstLog.FirstOrDefault(x => x.ID == ID);
                //    if (Query != null)
                //    {
                //        Query.Content = content;
                //    }
                //    else
                //    {
                //        lstLog.Add(new LogThuvienEntity
                //        {
                //            ID = ID,
                //            Action = $"Sửa {this.Text}",
                //            Module = this.Name,
                //            Content = content,
                //            UserID = GlobleData.UserName,
                //            CreatedDate = DateTime.Now

                //        });
                //    }
                //}
                #endregion

                gV.SetRowCellValue(e.RowHandle, colIsEdit, true);

            }
            #region Thoai
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
                    //row["NgayTao"] = DateTime.Now;
                }
            }
            catch
            {
                clsWaitForm.ShowErrorForm(this, 1500);
            }
            #endregion
        }

        private void btnAddTagMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.ActiveControl = button1;

            bool IsAddNewRow = tblTheMau.AsEnumerable().Any(x => x["MaTheMau"]?.ToString() == "NONE");
            if (!IsAddNewRow)
            {

                object TheMau = grvTheMau.GetRowCellValue(grvTheMau.FocusedRowHandle, colTheMau);
                DataRow drTagMau = tblTheMau.NewRow();

                drTagMau["MaTheMau"] = "NONE";
                drTagMau["IsEdit"] = true;
                tblTheMau.Rows.InsertAt(drTagMau, 0);


                grcTheMau.DataSource = tblTheMau;
                grvTheMau.FocusedRowHandle = 0;
                grcTheMau.RefreshDataSource();
                ClearImageCache();
                this.gV.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gV_CustomUnboundColumnData);
                if (tblThuVien?.Rows?.Count > 0)
                {
                    tblThuVien.Clear();
                    gC.DataSource = tblThuVien;
                    gV.UpdateCurrentRow();
                }

                //DataRow dr = tblThuVien.NewRow();
                //dr["ID"] = 0;
                //dr["MauVTID"] = DBNull.Value;
                //dr["TheMau"] = TheMau?.ToString();
                //dr["MaTheMau"] = "NONE";
                //dr["IsEdit"] = true;
                //dr["UserName"] = GlobleData.UserName;
                //tblThuVien.Rows.InsertAt(dr, 0);

                this.gV.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gV_CustomUnboundColumnData);
            }
        }

        private void grvTheMau_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column == colTheMau)
            {
                string MaTheMau = gV.GetFocusedRowCellValue(colMaTheMau)?.ToString();
                if (string.IsNullOrEmpty(MaTheMau)) MaTheMau = "NONE";

                if (tblThuVien?.Rows?.Count > 0)
                {
                    foreach (DataRow row in tblThuVien.Rows)
                    {
                        if (row["MaTheMau"]?.ToString() == MaTheMau)
                        {
                            row["TheMau"] = e.Value;
                        }
                    }
                }
                gV.SetRowCellValue(e.RowHandle, colIsEdit_Tag, true);
            }
        }

        private void grvTheMau_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (grvTheMau.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();
                if (e.HitInfo.InRow || e.HitInfo.InGroupRow)
                {
                    bool.TryParse(grvTheMau.GetFocusedRowCellValue(colIsUse_Tag)?.ToString(), out bool IsUse);
                    if (_allowDelete && !IsUse)
                    {
                        DevExpress.Utils.Menu.DXMenuItem menuDelete = new DevExpress.Utils.Menu.DXMenuItem("Xóa Thẻ Màu", btnXoaTag_Click);
                        e.Menu.Items.Add(menuDelete);
                    }
                }
            }

        }

        private void gV_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gV.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();
                if (e.HitInfo.InRow || e.HitInfo.InGroupRow)
                {
                    bool.TryParse(gV.GetFocusedRowCellValue(colIsUse)?.ToString(), out bool IsUse);
                    if (_allowDelete && !IsUse)
                    {
                        DevExpress.Utils.Menu.DXMenuItem menuDelete = new DevExpress.Utils.Menu.DXMenuItem("Xóa", btnXoa_Click);
                        e.Menu.Items.Add(menuDelete);
                    }
                }
            }
        }
        /* private void btnXoaTag_Click(object sender, EventArgs e)
         {
             try
             {

                 DataRow dr = grvTheMau.GetFocusedDataRow();
                 if (dr == null) return;
                 if (string.IsNullOrEmpty(dr["MaTheMau"].ToString()) || dr["MaTheMau"].ToString() == "NONE")
                 {
                     tblTheMau.Clear();
                     LoadTheMau();
                     return;
                 }
                 DialogResult messResult = MessageBox.Show($"Bạn có muốn xóa Thẻ Màu: {dr["TheMau"]} này không và sẽ các Màu chi tiết thuộc Thẻ Màu sẽ xóa hết ? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                 if (messResult == DialogResult.Yes)
                 {
                     string url = $"{URL}ERPThuVienMau/Delete?Action=DeleteTheMau&para={dr["MaTheMau"]}&para2={GlobleData.UserName}";
                     string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                     if (result.ToLower() == "true")
                     {
                         tblTheMau.Clear();
                         LoadTheMau();
                         clsWaitForm.ShowSuccessForm(this, 1000);
                     }

                     this.ActiveControl = button1;

                 }
             }
             catch (Exception ex)
             {


             }
         }
         private void btnXoa_Click(object sender, EventArgs e)
         {
             try
             {
                 DataRow dr = gV.GetFocusedDataRow();
                 if (dr == null) return;

                 string urlGET = $"{URL}ERPThuVienMau/Get?Action=GETDELETE&para={dr["MauVTID"].ToString()}";
                 string jsonGET = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;
                 if (jsonGET != "[]")
                 {
                     XtraMessageBox.Show("Màu này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                     return;
                 }

                 int deletedIndex = gV.GetDataSourceRowIndex(gV.FocusedRowHandle);  // Tính index trước remove để shift

                 if (dr["ID"]?.ToString() == "0")
                 {
                     tblThuVien.Rows.Remove(dr);
                 }
                 else
                 {
                     DialogResult messResult = MessageBox.Show($"Bạn có muốn xóa Màu: {dr["MauVT"]} - Code Màu : {dr["MaMauVT"]} này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                     if (messResult == DialogResult.Yes)
                     {
                         string url = $"{URL}ERPThuVienMau/Delete?Action=DELETE&para={dr["ID"]}&para2={GlobleData.UserName}";
                         string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                         if (result.ToLower() == "true")
                         {
                             clsWaitForm.ShowSuccessForm(this, 1000);
                         }

                         tblThuVien.Rows.Remove(dr);
                     }
                     else return;  
                 }

                 gC.DataSource = tblThuVien;


                 var newImageCache = new Dictionary<int, Image>();
                 foreach (var kvp in imageCache)
                 {
                     int newKey = kvp.Key > deletedIndex ? kvp.Key - 1 : kvp.Key;
                     newImageCache[newKey] = kvp.Value;
                 }
                 imageCache.Clear();
                 foreach (var kvp in newImageCache) imageCache[kvp.Key] = kvp.Value;

                 var newLoadingRows = new HashSet<int>();
                 foreach (int oldKey in loadingRows)
                 {
                     int newKey = oldKey > deletedIndex ? oldKey - 1 : oldKey;
                     newLoadingRows.Add(newKey);
                 }
                 loadingRows.Clear();
                 loadingRows = newLoadingRows;

                 gV.BeginUpdate();
                 gV.RefreshData();
                 gV.EndUpdate();

                 this.ActiveControl = button1;
             }
             catch (Exception ex)
             {

             }
         }*/
        private void btnXoaTag_Click(object sender, EventArgs e)
        {
            try
            {
                int[] selectedHandles = grvTheMau.GetSelectedRows();
                if (selectedHandles.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn ít nhất một Thẻ Màu để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa các Thẻ Màu đã chọn không? Các Màu chi tiết thuộc Thẻ Màu sẽ bị xóa hết.",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    DataRow dr = grvTheMau.GetDataRow(selectedHandles[i]);
                    if (dr == null) continue;

                    if (string.IsNullOrEmpty(dr["MaTheMau"]?.ToString()) || dr["MaTheMau"].ToString() == "NONE")
                    {

                        continue;
                    }
                    if (dr["IsUse"]?.ToString()?.ToLower() == "true")
                    {
                        XtraMessageBox.Show($"Thẻ Màu '{dr["TheMau"]}' đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }
                    string url = $"{URL}ERPThuVienMau/Delete?Action=DeleteTheMau&para={dr["MaTheMau"]}&para2={GlobleData.UserName}";
                    string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                    }
                }


                LoadTheMau();
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi xóa Thẻ Màu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteMauVT()
        {
            try
            {
                int[] selectedHandles = gV.GetSelectedRows();
                if (selectedHandles.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn ít nhất một Màu để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa các Màu đã chọn không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                // Duyệt ngược để tránh lỗi handle
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    DataRow dr = gV.GetDataRow(selectedHandles[i]);
                    if (dr == null) continue;

                    //string urlGET = $"{URL}ERPThuVienMau/Get?Action=GETDELETE&para={dr["MauVTID"]}";
                    //string jsonGET = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;
                    if (dr["IsUse"]?.ToString()?.ToLower() == "true")
                    {
                        XtraMessageBox.Show($"Màu '{dr["MauVT"]}' đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    if (dr["ID"]?.ToString() == "0")
                    {
                        tblThuVien.Rows.Remove(dr);
                    }
                    else
                    {
                        string url = $"{URL}ERPThuVienMau/Delete?Action=DELETE&para={dr["ID"]}&para2={GlobleData.UserName}";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                            tblThuVien.Rows.Remove(dr);
                        }
                    }
                }

                gC.DataSource = tblThuVien;

                // Làm sạch cache ảnh (nếu có)
                imageCache.Clear();
                loadingRows.Clear();

                gV.BeginUpdate();
                gV.RefreshData();
                gV.EndUpdate();

                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa đã xảy ra lỗi. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            DeleteMauVT();
        }


        //private void gV_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        //{
        //    if (e.Column != colImage) return;

        //    if (e.IsGetData)
        //    {

        //        object cachedValue;
        //        if (imageCache.TryGetValue(e.ListSourceRowIndex, out cachedValue))
        //        {
        //            e.Value = cachedValue; 
        //        }
        //        else
        //        {

        //            e.Value = "";
        //        }
        //    }
        //    else if (e.IsSetData)
        //    {
        //        //tblThuVien.Rows[e.ListSourceRowIndex]["Image"]=
        //        imageCache[e.ListSourceRowIndex] = e.Value;               
        //        gV.RefreshRow(gV.GetRowHandle(e.ListSourceRowIndex));
        //    }
        //}

        private void GridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
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

        private void BandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;

            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);
            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {
                ControlPaint.DrawBorder3D(e.Graphics, rect);
                rect.Inflate(-1, -1);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color backColor = ColorTranslator.FromHtml("#FFD480"); // default
                    /* switch (e.Band.Name)
                     {
                         case "bgThieu": backColor = ColorTranslator.FromHtml("#F8B6B6"); break;
                         case "gbThua": backColor = ColorTranslator.FromHtml("#C5E3BF"); break;
                     }*/


                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }

                }

                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                    e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.None,
                            FormatFlags = StringFormatFlags.LineLimit
                        };


                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }

                }


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



        //private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        this.ActiveControl = button1;
        //        if (tblThuVien == null || tblThuVien.Rows.Count == 0) return;



        //        //clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_MauVTTV");

        //        string url = $"{URL}ERPThuVienMau/Post?Action=POST";
        //        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblThuVien); }).Result;

        //        if (msResult.ToLower() == "true")
        //        {
        //            clsWaitForm.ShowSuccessForm(this, 3000);
        //            LoadTheMau();

        //        }
        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //}


        private async void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gV.CloseEditor();
            gV.UpdateCurrentRow();
            this.ActiveControl = button1;
            tblSave = new DataTable();
            btnLuu.Enabled = false;
            btnLuu.Caption = "Đang lưu...";
            string apiUrl = $"{Host}/api/ERPThuVienVT/UpLoadImg";
            string url = $"{URL}ERPThuVienMau/Post?Action=POST";

            imageList = new List<dynamic>();
            // Kiểm tra và xử lý dữ liệu trước khi lưu
            if (ValidateAndCleanData())
            {
                // Nếu kiểm tra thất bại, return sớm
                btnLuu.Enabled = true;
                btnLuu.Caption = "Lưu (Ctrl + S)";
                return;
            }
            if (tblSave == null || tblSave.Rows.Count == 0)
            {
                tblSave = tblThuVien.Clone();
            }

            try
            {
                // Lấy toàn bộ dữ liệu từ DataSource không bị filter
                DataTable dtSource = tblThuVien;

                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    DataRow dataRow = dtSource.Rows[i];
                    bool.TryParse(dataRow["IsEdit"]?.ToString(), out bool IsEdit);

                    if (IsEdit)
                    {
                        DataRow row = tblSave.NewRow();
                        row["MaTheMau"] = dataRow["MaTheMau"];
                        row["TheMau"] = dataRow["TheMau"];
                        row["UserName"] = GlobleData.UserName;
                        row["NguoiTao"] = GlobleData.UserName;
                        row["NguoiSua"] = DBNull.Value;
                        tblSave.Rows.Add(row);
                    }
                }

                if (imageList.Count > 0)
                {
                    string getFileName = $"{grvTheMau.GetFocusedRowCellValue(colMaTheMau)?.ToString()}-{grvTheMau.GetFocusedRowCellValue(colTheMau)?.ToString()}";
                    string libFolder = "MauVatTu";
                    JArray imageDatas = JArray.FromObject(imageList);
                    var response = await UploadImagesToApi(apiUrl, imageDatas, ReplaceSpecialCharacters(RemoveVietnameseTone(getFileName)), libFolder);

                    if (response != null && response.Count > 0)
                    {
                        foreach (var item in response)
                        {
                            if (item.ID != null)
                            {
                                string idStr = item.ID.ToString();
                                if (int.TryParse(idStr, out int Index))
                                {
                                    if (Index >= 0 && Index < tblSave.Rows.Count)
                                    {
                                        tblSave.Rows[Index]["Image"] = item.url ?? "";
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Index không hợp lệ: {Index}");
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"ID không parse được: {idStr}");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Item ID là null, bỏ qua.");
                            }
                        }

                        tblThuVien.AcceptChanges();
                        gV.RefreshData();

                        System.Diagnostics.Debug.WriteLine($"Lưu thành công! {response.Count} ảnh đã upload.\nURLs: {string.Join(", ", response.Select(r => r.url ?? "N/A"))}");
                    }
                }

                foreach (int index in indicesToRemove.OrderByDescending(x => x))
                {
                    tblSave.Rows.RemoveAt(index);
                }

                if (tblSave.Columns.Contains("IsEdit"))
                {
                    tblSave.Columns.Remove("IsEdit");
                }
                if (tblSave.Columns.Contains("IsUse"))
                {
                    tblSave.Columns.Remove("IsUse");
                }

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadTheMau();
                    ClearImageCache();
                    gV.RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}\nStack: {ex.StackTrace}");
            }
            finally
            {
                btnLuu.Enabled = true;
                btnLuu.Caption = "Lưu (Ctrl + S)";
            }
        }

        private bool ValidateAndCleanData()
        {
            if (tblThuVien == null || tblThuVien.Rows.Count == 0) return false;

            tblSave = tblThuVien.Copy();
            var errors = new List<string>();
            var combinedKeyList = new HashSet<string>();
            indicesToRemove = new List<int>();

         
            DataTable dtSource = gC.DataSource as DataTable;
            if(dtSource!=null && dtSource?.Rows?.Count > 0)
            {
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    DataRow dataRow = dtSource.Rows[i];

                    // Lấy row handle tương ứng trong GridView (nếu cần xử lý image)
                    int handle = gV.GetRowHandle(i);

                    bool isEdit = Convert.ToBoolean(dataRow["IsEdit"] ?? false);

                    string MaMauVTID = dataRow["MauVTID"]?.ToString();
                    string TenMau = dataRow["MauVT"]?.ToString();
                    string CodeMau = dataRow["MaMauVT"]?.ToString();

                    // Xử lý image nếu có trong cache
                    if (handle >= 0 && imageCache.TryGetValue(handle, out Image imgObj) && imgObj is Image img)
                    {
                        string base64String = ImageToBase64String(img, ImageFormat.Png);
                        string imgName = $"{ReplaceSpecialCharacters(RemoveVietnameseTone(CodeMau))}-{ReplaceSpecialCharacters(RemoveVietnameseTone(TenMau))}";

                        imageList.Add(new
                        {
                            img = $"data:image/png;base64,{base64String}",
                            name = imgName,
                            ID = i
                        });
                    }

                    if (!isEdit)
                    {
                        indicesToRemove.Add(i);
                        continue;
                    }

                    string codeMau = dataRow["MaMauVT"]?.ToString()?.Trim();
                    string tenMau = dataRow["MauVT"]?.ToString()?.Trim();

                    // Kiểm tra CodeMau
                    if (string.IsNullOrEmpty(codeMau))
                    {
                        errors.Add($"• <color=blue>Dòng {i + 1}</color>: <b><color=red>Code Màu</color></b> không được bỏ trống.\n");
                        continue;
                    }

                    // Kiểm tra TenMau
                    if (string.IsNullOrEmpty(tenMau))
                    {
                        errors.Add($"• <color=blue>Dòng {i + 1}</color>: <b><color=red>Tên Màu</color></b> không được bỏ trống.\n");
                        continue;
                    }

                    // Tạo key ghép: CodeMau + TenMau
                    string combinedKey = $"{codeMau}|{tenMau}";
                    if (combinedKeyList.Contains(combinedKey))
                    {
                        errors.Add($"• <color=blue>Dòng {i + 1}</color>: Code Màu <b><color=red>'{codeMau}'</color></b> và Tên Màu <b><color=red>'{tenMau}'</color></b> bị trùng lặp.\n");
                        continue;
                    }

                    combinedKeyList.Add(combinedKey);
                }
            }

         

            DataTable tblCheck = tblSave.Copy();

            // Xóa 2 cột thừa khỏi bản sao TẠM THỜI
            if (tblCheck.Columns.Contains("IsEdit"))
            {
                tblCheck.Columns.Remove("IsEdit");
            }
            if (tblCheck.Columns.Contains("IsUse"))
            {
                tblCheck.Columns.Remove("IsUse");
            }

            string url = $"{URL}ERPThuVienMau/Get?Action=CheckMauVT";
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblCheck); }).Result;

            if (json != "[]")
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl?.Rows.Count > 0)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        bool.TryParse(row["Isval"]?.ToString(), out bool Isval);
                        if (Isval)
                        {
                            errors.Add($"• {row["Msg"]?.ToString()}.\n");
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                string htmlMessage = "<b><color=red> Dữ liệu hiện chưa lưu được vì:</color></b>\n\n" + string.Join("", errors);
                ShowWarning(htmlMessage);
                return true; // Có lỗi
            }

            return false;
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




        // Helper functions (nếu chưa có)
        private string ImageToBase64String(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        private async Task<List<dynamic>> UploadImagesToApi(string apiUrl, JArray imageDatas, string getFileName, string libFolder)
        {
            using (var client = new HttpClient())
            {
                var queryParams = $"?getFileName={Uri.EscapeDataString(getFileName)}&LibFolder={Uri.EscapeDataString(libFolder)}";
                string fullUrl = apiUrl + queryParams;

                string jsonBody = imageDatas.ToString(Formatting.None);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(fullUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<dynamic>>(responseJson);
                }
                return null;
            }
        }

        // Thêm method để clear cache
        private void ClearImageCache()
        {
            foreach (var img in imageCache.Values)
            {
                img?.Dispose();
            }
            imageCache.Clear();
            loadingRows.Clear();
        }

        private void gV_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            if (e.RowHandle == GridControl.AutoFilterRowHandle)
            {
                e.RowHeight = 30;
                return;
            }
            if (gV.IsGroupRow(e.RowHandle))
            {
                e.RowHeight = 30;
                return;
            }
            e.RowHeight = 50;

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
                        gridView.IndicatorWidth = nNewSize + 10;
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
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "*";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            //GridView gridview = ((GridView)sender);
            //if (!gridview.GridControl.IsHandleCreated) return;
            //Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            //SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            //gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 10;
        }

        private void grvTheMau_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && grvTheMau?.RowCount > 0)
            {
                e.Appearance.ForeColor = ColorTranslator.FromHtml("#1A0000");
                e.HighPriority = true;
                if (grvTheMau.FocusedRowHandle == e.RowHandle)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#FFFBE6");
                }
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Bold);

            }
        }

        private void gV_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && gV?.RowCount > 0)
            {
                e.Appearance.ForeColor = ColorTranslator.FromHtml("#1A0000");
                e.HighPriority = true;
                if (gV.FocusedRowHandle == e.RowHandle)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#FFFBE6");
                }
                //e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Bold);
            }

        }

        private void grv_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gV.FocusedRowHandle < 0) return;

            if (this.gV.FocusedColumn != null)
            {
                e.Valid = true;

                if (this.gV.FocusedColumn == colTheMau)
                {
                    DataTable tbl = this.grcTheMau.DataSource as DataTable;
                    bool IsTagColor = tbl.AsEnumerable().Any(x => x["TheMau"]?.ToString() == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Thẻ màu không được bỏ trống";
                        e.Valid = false;
                    }
                    else if (IsTagColor)
                    {

                        e.ErrorText = $"Thẻ màu {e.Value.ToString()} đã bị trùng";
                        e.Valid = false;
                    }

                }
                //else if (this.grvMaSeal.FocusedColumn == colLoaiSeal)
                //{
                //    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                //    {
                //        e.ErrorText = $" Loại Seal không được bỏ trống";
                //        e.Valid = false;
                //    }
                //}


            }

        }

        private void gV_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoMauVT || e.Column == colNguoiSuaMauVT)
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
        private void gV_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            if (gV.FocusedRowHandle < 0) return;
            DataRow row_focused = gV.GetFocusedDataRow();
            if (row_focused == null) return;

            string codeMau = row_focused["MaMauVT"].ToString();
            string tenMau = row_focused["MauVT"].ToString();

         
            if (gV.FocusedColumn == colCodeMau)
            {
                codeMau = e.Value?.ToString();
            }
           
            else if (gV.FocusedColumn == colTenMau)
            {
                tenMau = e.Value?.ToString();
            }

            DataTable tblCheck = gC.DataSource as DataTable;
            if (tblCheck == null && tblCheck?.Rows?.Count == 0) return;
            foreach (DataRow row in tblCheck.Rows)
            {
                if (row == row_focused) continue; // bỏ qua dòng hiện tại
                if (row["MaMauVT"].ToString() == codeMau && row["MauVT"].ToString() == tenMau)
                {
                    e.Valid = false;
                    e.ErrorText = $"Code màu: {codeMau} - Tên màu : {tenMau} đã bị trùng vui lòng thử lại.!";
                    break;
                }
            }
        }


        private void gridView1_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();


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
    }
}