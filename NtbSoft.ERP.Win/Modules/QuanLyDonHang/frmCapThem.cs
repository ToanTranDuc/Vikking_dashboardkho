using DevExpress.XtraEditors;
using Newtonsoft.Json;
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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCapThem : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string maLenhSX = string.Empty;
        string lenhSX = string.Empty;
        string maDH = string.Empty;
        string maHang = string.Empty;
        string dot = string.Empty;
        string maGop = string.Empty;
        int focusedRowHandle;
        DataTable _dtSave;
        private bool isDataSourceChanging = false;
        private GridColumn editableColumn;

        public frmCapThem(DataRow _currentRow, string MaDh, string Lenh)
        {
            InitializeComponent();
            if (_currentRow != null)
            {
                maLenhSX = _currentRow["MaLenhSanXuat"]?.ToString() ?? "";
                lenhSX = Lenh;
                maDH = MaDh;
                maHang = _currentRow["MaHang"]?.ToString() ?? "";
                //dot = _currentRow["DotSX"]?.ToString() ?? "";
                maGop = _currentRow["MaGop"]?.ToString() ?? "";
            }
            
            init();
        }
        private void init()
        {


            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            LoadData();
            LoadDot();
        }

        private void LoadData()
        {

            string urlDMCD = string.Format("{0}?madh={1}&&maLenhSX={2}", URL + "Dot/GetDinhMucCD", maGop, maLenhSX);
            string jsonDMCD = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDMCD); }).Result;
            DataTable tblDMCD = JsonConvert.DeserializeObject<DataTable>(jsonDMCD);
            isDataSourceChanging = true;
            gridControl1.DataSource = tblDMCD;
            if (tblDMCD != null)
            {
                foreach (DataRow row in tblDMCD.Rows)
                {
                    row["MaLenh"] = lenhSX;
                    row["MaDH"] = maDH;
                    //row["CapThem"] = 0;
                }
            }
            isDataSourceChanging = false;
            gridView1.FocusedRowHandle = focusedRowHandle;
            LoadDot();
        }
        private void LoadDot() 
        {
            try
            {
                if (gridView1.RowCount != 0)
                {
                    string mavtMau = gridView1.GetFocusedRowCellValue(gridColumn42).ToString();
                    string maNpl = gridView1.GetFocusedRowCellValue(gridColumn1).ToString();
                    string maVt = gridView1.GetFocusedRowCellValue(gridColumn2).ToString();
                    string Dausize = gridView1.GetFocusedRowCellValue(gridColumn27).ToString();
                    string maMau = gridView1.GetFocusedRowCellValue(gridColumn19).ToString();
                    string siZe = gridView1.GetFocusedRowCellValue(gridColumn31).ToString();
                    string maBom = gridView1.GetFocusedRowCellValue(gridColumn37).ToString();
                    string MaDH = maGop;
                    string maLenh = maLenhSX;
                    string url = URL + $"Dot/GetLichSu?madh={MaDH}&&malenh={maLenh}&&manpl={maNpl}&&mavt={maVt}&&mamau={maMau}&&dausize={Dausize}&&size={siZe}&&mabom={maBom}&&mavtmau={mavtMau}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable _dtSave = JsonConvert.DeserializeObject<DataTable>(json);
                    gridLichSuControl.DataSource = _dtSave;
                    if (_dtSave != null)
                    {
                        foreach (DataRow row in _dtSave.Rows)
                        {
                            row["MaLenh"] = lenhSX;
                            row["MaDH"] = maDH;
                        }
                    }
                }
                else 
                {
                    return;
                }

            }
            catch (Exception e) 
            {
                return;
            }

        }
        private DataTable CreateTableSave()
        {
            DataTable _tblCreate = new DataTable();
            _tblCreate.Columns.Add("ID", typeof(int));
            _tblCreate.Columns.Add("MaDH", typeof(string));
            _tblCreate.Columns.Add("MaLenhSanXuat", typeof(string));
            _tblCreate.Columns.Add("MaNPL", typeof(string));
            _tblCreate.Columns.Add("MaVT", typeof(string));
            _tblCreate.Columns.Add("TenVT", typeof(string));
            _tblCreate.Columns.Add("MaMau", typeof(string));
            _tblCreate.Columns.Add("KhoVai", typeof(string));
            _tblCreate.Columns.Add("MaDV", typeof(string));
            _tblCreate.Columns.Add("DinhMuc", typeof(double));
            _tblCreate.Columns.Add("SoLuong", typeof(int));
            _tblCreate.Columns.Add("CapPhat", typeof(double));
            _tblCreate.Columns.Add("CapThem", typeof(double));
            _tblCreate.Columns.Add("ThuHoi", typeof(double));
            _tblCreate.Columns.Add("TrangThai", typeof(int));
            _tblCreate.Columns.Add("GhiChu", typeof(string));
            _tblCreate.Columns.Add("MaMauLenh", typeof(string));
            _tblCreate.Columns.Add("DauSizeLenh", typeof(string));
            _tblCreate.Columns.Add("SizeLenh", typeof(string));
            _tblCreate.Columns.Add("MaBom", typeof(string));
            _tblCreate.Columns.Add("NguoiCap", typeof(string));
            _tblCreate.Columns.Add("NgayCap", typeof(DateTime));
            _tblCreate.Columns.Add("Dot", typeof(string));
            _tblCreate.Columns.Add("TenDot", typeof(string));
            _tblCreate.Columns.Add("MaVtMau", typeof(string));
            return _tblCreate;
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.ActiveControl = this.Button1;
            DataTable _tblSaveTemp = (gridView1.DataSource as DataView).Table;
            DataTable _tblSave = CreateTableSave();
            bool isValid = _tblSaveTemp.AsEnumerable()
                .Any(row => int.TryParse(row["CapThem"]?.ToString(), out int value) && value > 0);
            bool isValidcp = _tblSaveTemp.AsEnumerable()
                .Any(row => int.TryParse(row["CapPhat"]?.ToString(), out int value1) && value1 > 0);

            if (!isValid)
            {
                return;
            }
            if (!isValidcp) 
            {
                XtraMessageBox.Show("Chưa cấp phát không thể cấp thêm.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1); ;
            }
            foreach (DataRow row in _tblSaveTemp.Rows)
            {
                DataRow _rowInSert = _tblSave.NewRow();
                _rowInSert["ID"] = -1;
                _rowInSert["MaDH"] = maGop.ToString();
                _rowInSert["MaLenhSanXuat"] = maLenhSX.ToString();
                _rowInSert["MaNPL"] = row["MaNPL"]?.ToString() ?? "";
                _rowInSert["MaVT"] = row["MaVT"]?.ToString() ?? "";
                _rowInSert["TenVT"] = row["TenVT"]?.ToString() ?? "";
                _rowInSert["MaMau"] = row["MaMau"]?.ToString() ?? "";
                _rowInSert["KhoVai"] = row["KhoVai"]?.ToString() ?? "";
                _rowInSert["MaDV"] = row["MaDV"]?.ToString() ?? "";
                _rowInSert["DinhMuc"] = row["DinhMuc"] ?? 0.0;
                _rowInSert["SoLuong"] = row["SoLuong"] ?? 0;
                _rowInSert["CapPhat"] = row["CapPhat"] ?? 0.0;
                _rowInSert["CapThem"] = row["CapThem"] ?? 0.0;
                _rowInSert["TrangThai"] = 0;
                _rowInSert["GhiChu"] = row["GhiChu"] ?? "";
                _rowInSert["MaMauLenh"] = row["MaMauL"];
                _rowInSert["DauSizeLenh"] = row["DauSizeL"];
                _rowInSert["SizeLenh"] = row["SizeL"];
                _rowInSert["MaBom"] = row["MaBom"];
                _rowInSert["NguoiCap"] = GlobleData.UserName;
                _rowInSert["NgayCap"] = DateTime.Now;
                _rowInSert["MaVtMau"] = row["MaVTMau"];
                _tblSave.Rows.Add(_rowInSert);

            }
            string urlSaveDM = string.Format("{0}", URL + "Dot/PostDotDinhMuc");
            string savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, _tblSave); }).Result;
            if (string.Compare(savelistDM, "True") != 0)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi trong quá trình thực hiện. Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                LoadData();
            }

        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (isDataSourceChanging) return; //khi gan datasource = false
            if (e.FocusedRowHandle >= 0)
            {
                focusedRowHandle = e.FocusedRowHandle;
                LoadDot();
            }
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridLichSuView.RowCount >0) 
                {
                    DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.Yes)
                    {
                        string mavtMau = gridLichSuView.GetFocusedRowCellValue(gridColumnMaVtMau).ToString();
                        string maNpl = gridLichSuView.GetFocusedRowCellValue(gridColMaNPL).ToString();
                        string maVt = gridLichSuView.GetFocusedRowCellValue(gridColMaVatTu).ToString();
                        string Dausize = gridLichSuView.GetFocusedRowCellValue(gridColumn23).ToString();
                        string maMau = gridLichSuView.GetFocusedRowCellValue(gridColumn21).ToString();
                        string siZe = gridLichSuView.GetFocusedRowCellValue(gridColumn24).ToString();
                        string maBom = gridLichSuView.GetFocusedRowCellValue(gridColumn38).ToString();
                        string dot = gridLichSuView.GetFocusedRowCellValue(colDot).ToString();
                        string MaDH = maGop;
                        string maLenh = maLenhSX;
                        string url = URL + $"Dot/DeleteDot?madh={MaDH}&&malenhsanxuat={maLenh}&&dot={dot}&&manpl={maNpl}&&mavt={maVt}&&mamau={maMau}&&dausize={Dausize}&&size={siZe}&&mabom={maBom}&&mavtmau={mavtMau}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                        {
                            LoadDot();
                        }
                        else XtraMessageBox.Show(result);
                    }
                    else 
                    {
                        return;
                    }

                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Không có dữ liệu vui lòng nhập.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDot();
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            Color gradientStartColor;
            Color gradientEndColor;
            if (e.Column.FieldName == "CapThem" || e.Column.Caption == "Cấp Thêm")
            {
                gradientStartColor = Color.Yellow;
                gradientEndColor = Color.Yellow;
            }
            else
            {
                gradientStartColor = Color.FromArgb(255, 212, 128); // Màu mặc định
                gradientEndColor = Color.FromArgb(255, 212, 128);   // Màu mặc định
            }
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, gradientStartColor, gradientEndColor, e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridLichSuView_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            Color gradientStartColor;
            Color gradientEndColor;
            if (e.Column.FieldName == "CapThem" || e.Column.Caption == "Cấp Thêm")
            {
                gradientStartColor = Color.Yellow;
                gradientEndColor = Color.Yellow; 
            }
            else
            {
                gradientStartColor = Color.FromArgb(255, 212, 128); // Màu mặc định
                gradientEndColor = Color.FromArgb(255, 212, 128);   // Màu mặc định
            }
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, gradientStartColor, gradientEndColor, e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridLichSuView_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {

        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (gridView1.FocusedRowHandle < 0 || view.FocusedColumn == null) return;
            if (view == null) return;

            if (gridView1.FocusedColumn == gridColumn13)
            {
                string inputValue = e.Value.ToString() ?? string.Empty;

                if (System.Text.RegularExpressions.Regex.IsMatch(inputValue, @"[^a-zA-Z0-9\s]"))
                {
                    e.ErrorText = "Giá trị không được chứa ký tự đặc biệt hoặc dấu.";
                    e.Valid = false;
                    return;
                }
                if (Int32.Parse(inputValue)<0)
                {
                    e.ErrorText = "Giá trị phải là số lớn hơn hoặc bằng 0.";
                    e.Valid = false;
                }
            }
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            DataRow _rowFocused = gridView1.GetFocusedDataRow();
            if (_rowFocused == null) return;
            string focusedColumn = gridView1.FocusedColumn.FieldName;
            if (focusedColumn == "CapThem")
            {
                if (_rowFocused["CapPhat"].ToString() == "0")
                {
                    e.Cancel = true;
                }
                else 
                {
                    e.Cancel = false;
                }
            }
            else 
            {
                e.Cancel = true;
            }
        }
    }
}