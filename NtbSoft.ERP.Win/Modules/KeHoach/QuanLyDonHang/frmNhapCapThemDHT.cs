using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity;
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmNhapCapThemDHT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        string _magop = string.Empty, _nguoitao = string.Empty, _maLSX = string.Empty, _tenKH = string.Empty, _mahang = string.Empty, _madh = string.Empty, _malenh = string.Empty, _soluong = string.Empty, _tenhang = string.Empty, _dot = string.Empty;
        int _iskeove = 0;
        bool checkAddRow = true;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        bool isVatTu = true;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        bool isCal = true;
        DataTable tblSave;
        DataTable tblPhuLieu;
        DataTable tblNguyenLieu;

        DataTable tblLichSu;
        DataTable tblLichSuNL;
        DataTable tblLichSuPL;
        bool checkGridView = true;
        public frmNhapCapThemDHT()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }

        public frmNhapCapThemDHT(string _magop, string _nguoitao, string _maLSX, string _tenKH, string _mahang, string _madh, string _malenh, string _soluong, string _tenhang, int _iskeove, string _dot)
        {
            this._magop = _magop;
            this._nguoitao = _nguoitao;
            this._maLSX = _maLSX;
            this._tenKH = _tenKH;
            this._mahang = _mahang;
            this._madh = _madh;
            this._malenh = _malenh;
            this._soluong = _soluong;
            this._tenhang = _tenhang;
            this._iskeove = _iskeove;
            this._dot = _dot;
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblSave = new DataTable();
            tblNguyenLieu = new DataTable();
            tblPhuLieu = new DataTable();
            tblLichSu = new DataTable();
            tblLichSuNL = new DataTable();
            tblLichSuPL = new DataTable();

            this.ActiveControl = button1;
        }

        private List<ActionControl> InitActionKeyDown()
        {

            //actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            //actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);
            lstActionControls = new List<ActionControl> {
                actionControlAdd,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            //keyDownControlHandler.PressKeyDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateTableSave();
            CreateTableLichSu();


            /*loadLichSu(_magop, true);
            loadCapPhat(_magop, true);*/
            txtDonHang.Text = _madh;
            txtMaHang.Text = _tenhang;
            txtLenhSX.Text = _malenh;
            txtDot.Text = _dot;
            loadSoLuong();
            loadSoBooking();
            cbBoxLoaiVT.EditValue = "Nguyên liệu";
            //CreateSearchLookUpTenVTGoiY();
            ConfigureComboBoxLoaiVT();
        }
        private void CreateTableSave()
        {
            tblSave = new DataTable("tblSave");

            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("MaDH", typeof(string));
            tblSave.Columns.Add("MaLenhSanXuat", typeof(string));
            tblSave.Columns.Add("MaNPL", typeof(string));
            tblSave.Columns.Add("MaVT", typeof(string));
            tblSave.Columns.Add("TenVT", typeof(string));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("KhoVai", typeof(string));
            tblSave.Columns.Add("MaDV", typeof(string));
            tblSave.Columns.Add("DinhMuc", typeof(float));
            tblSave.Columns.Add("SoLuong", typeof(int));
            tblSave.Columns.Add("CapPhat", typeof(float));
            tblSave.Columns.Add("CapThem", typeof(float));
            tblSave.Columns.Add("ThuHoi", typeof(float));
            tblSave.Columns.Add("TrangThai", typeof(int));
            tblSave.Columns.Add("GhiChu", typeof(string));
            tblSave.Columns.Add("NguoiTao", typeof(string));
            tblSave.Columns.Add("NguoiSua", typeof(string));
            tblSave.Columns.Add("MaMauLenh", typeof(string));
            tblSave.Columns.Add("DauSizeLenh", typeof(string));
            tblSave.Columns.Add("SizeLenh", typeof(string));
            tblSave.Columns.Add("MaBom", typeof(string));
            tblSave.Columns.Add("IsXetDuyet", typeof(string));
            tblSave.Columns.Add("NguoiXet", typeof(string));
            tblSave.Columns.Add("NguoiHuy", typeof(string));
            tblSave.Columns.Add("NgayXet", typeof(string));
            tblSave.Columns.Add("NgayHuy", typeof(string));
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("QtyES", typeof(float));
            tblSave.Columns.Add("CostES", typeof(float));
            tblSave.Columns.Add("NeedES", typeof(float));
            tblSave.Columns.Add("ToTalRec", typeof(float));
            tblSave.Columns.Add("ID_MaNPL", typeof(string));

            tblSave.Columns.Add("IsNL", typeof(bool));
            tblSave.Columns.Add("MaKH", typeof(string));
            tblSave.Columns.Add("ThucNhan", typeof(float));
            tblSave.Columns.Add("TenTAVT", typeof(string));
            tblSave.Columns.Add("NhomNPL", typeof(string));
            tblSave.Columns.Add("DinhMucHaoHut", typeof(float));
            tblSave.Columns.Add("MaNhomVT", typeof(string));
            tblSave.Columns.Add("MaVTTheoCayVai", typeof(string));
            tblSave.Columns.Add("IsNew", typeof(string));
            tblNguyenLieu = tblSave.Clone();
            tblPhuLieu = tblSave.Clone();
        }
        private void CreateTableLichSu()
        {
            tblLichSu = new DataTable("tblLichSu");

            tblLichSu.Columns.Add("ID", typeof(int));
            tblLichSu.Columns.Add("MaDH", typeof(string));
            tblLichSu.Columns.Add("MaLenhSanXuat", typeof(string));
            tblLichSu.Columns.Add("MaNPL", typeof(string));
            tblLichSu.Columns.Add("MaVT", typeof(string));
            tblLichSu.Columns.Add("TenVT", typeof(string));
            tblLichSu.Columns.Add("MaMau", typeof(string));
            tblLichSu.Columns.Add("KhoVai", typeof(string));
            tblLichSu.Columns.Add("MaDV", typeof(string));
            tblLichSu.Columns.Add("DinhMuc", typeof(float));
            tblLichSu.Columns.Add("SoLuong", typeof(int));
            tblLichSu.Columns.Add("CapPhat", typeof(float));
            tblLichSu.Columns.Add("CapThem", typeof(float));
            tblLichSu.Columns.Add("ThuHoi", typeof(float));
            tblLichSu.Columns.Add("TrangThai", typeof(int));
            tblLichSu.Columns.Add("GhiChu", typeof(string));
            tblLichSu.Columns.Add("NguoiTao", typeof(string));
            tblLichSu.Columns.Add("NguoiSua", typeof(string));
            tblLichSu.Columns.Add("NgayTao", typeof(string));
            tblLichSu.Columns.Add("NgaySua", typeof(string));
            tblLichSu.Columns.Add("MaMauLenh", typeof(string));
            tblLichSu.Columns.Add("DauSizeLenh", typeof(string));
            tblLichSu.Columns.Add("SizeLenh", typeof(string));
            tblLichSu.Columns.Add("MaBom", typeof(string));

            tblLichSu.Columns.Add("IsXetDuyet", typeof(string));
            tblLichSu.Columns.Add("NguoiXet", typeof(string));
            tblLichSu.Columns.Add("NguoiHuy", typeof(string));
            tblLichSu.Columns.Add("NgayXet", typeof(string));
            tblLichSu.Columns.Add("NgayHuy", typeof(string));
            tblLichSu.Columns.Add("MaVTMau", typeof(string));
            tblLichSu.Columns.Add("QtyES", typeof(float));
            tblLichSu.Columns.Add("CostES", typeof(float));
            tblLichSu.Columns.Add("NeedES", typeof(float));
            tblLichSu.Columns.Add("ToTalRec", typeof(float));
            tblLichSu.Columns.Add("ID_MaNPL", typeof(string));

            tblLichSu.Columns.Add("ThucNhan", typeof(float));
            tblLichSu.Columns.Add("TenTAVT", typeof(string));
            tblLichSu.Columns.Add("NhomNPL", typeof(string));
            tblLichSu.Columns.Add("DinhMucHaoHut", typeof(float));
            tblLichSu.Columns.Add("MaNhomVT", typeof(string));
            tblLichSu.Columns.Add("MaVTTheoCayVai", typeof(string));
            tblLichSu.Columns.Add("IsNew", typeof(string));
            tblLichSu.Columns.Add("IsNL", typeof(bool));
            tblLichSu.Columns.Add("Dot", typeof(string));
            tblLichSuPL = tblLichSu.Clone();
            tblLichSuPL = tblLichSu.Clone();
        }

        private void ThemDong(bool isNL, DataRow dr)
        {

            try
            {
                if (isNL)
                {
                    DataTable tblNL = gCLichSu.DataSource as DataTable;
                    if (tblNL == null)
                        tblNL = tblLichSu.Clone();

                    DataRow row = AddNewRow(isNL, tblNL, dr);
                    if(checkCapThemTrungDot(row))
                    {

                        return;
                    }
                    tblNL.Rows.Add(row);
                    gCLichSu.DataSource = tblNL;
                }
                else
                {
                    DataTable tblPL = gCLichSu.DataSource as DataTable;
                    if (tblPL == null)
                        tblPL = tblLichSu.Clone();
                    DataRow row = AddNewRow(isNL, tblPL, dr);
                    tblPL.Rows.Add(row);
                    gCLichSu.DataSource = tblPL;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private DataRow AddNewRow(bool isNL, DataTable tbl, DataRow dr)
        {
            DataRow row = tbl.NewRow();
            row["ID"] = dr["ID"];
            row["MaDH"] = dr["MaDH"];
            row["MaLenhSanXuat"] = dr["MaLenhSanXuat"];
            row["MaNPL"] = dr["MaNPL"];
            row["MaVT"] = dr["MaVT"];
            row["TenVT"] = dr["TenVT"];
            row["MaMau"] = dr["MaMau"];
            row["KhoVai"] = dr["KhoVai"];
            row["MaDV"] = dr["MaDV"];
            row["DinhMuc"] = dr["DinhMuc"];
            row["SoLuong"] = dr["SoLuong"];
            row["CapPhat"] = dr["CapPhat"];
            row["CapThem"] = 0;
            row["ThuHoi"] = dr["ThuHoi"];
            row["TrangThai"] = dr["TrangThai"];
            row["GhiChu"] = dr["GhiChu"];
            row["NguoiTao"] = GlobleData.UserName;
            row["NguoiSua"] = dr["NguoiSua"];
            row["MaMauLenh"] = dr["MaMauLenh"];
            row["DauSizeLenh"] = dr["DauSizeLenh"];
            row["SizeLenh"] = dr["SizeLenh"];
            row["IsXetDuyet"] = dr["IsXetDuyet"];
            row["NguoiXet"] = dr["NguoiXet"];
            row["NguoiHuy"] = dr["NguoiHuy"];
            row["NgayXet"] = dr["NgayXet"];
            row["NgayHuy"] = dr["NgayHuy"];
            row["ID_MaNPL"] = dr["ID_MaNPL"];
            row["MaBom"] = dr["MaBom"];
            row["MaVTMau"] = dr["MaVTMau"];
            row["QtyES"] = dr["QtyES"];
            row["CostES"] = dr["CostES"];
            row["NeedES"] = dr["NeedES"];
            row["ToTalRec"] = dr["ToTalRec"];
            row["ThucNhan"] = dr["ThucNhan"];
            row["TenTAVT"] = dr["TenTAVT"];
            row["NhomNPL"] = dr["NhomNPL"];
            row["DinhMucHaoHut"] = dr["DinhMucHaoHut"];
            row["MaNhomVT"] = dr["MaNhomVT"];
            row["MaVTTheoCayVai"] = dr["MaVTTheoCayVai"];
            row["IsNew"] = "them";
            //row["Dot"] = "hehe";
            row["Dot"] = loadDotCapThem(dr["MaNPL"].ToString());
            return row;
        }
        private bool checkCapThemTrungDot(DataRow dr)
        {
            DataTable tbl = gCLichSu.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return false;

            foreach (DataRow row in tbl.Rows)
            {
                bool isDuplicate = true; // Giả định ban đầu là trùng

                foreach (DataColumn col in tbl.Columns)
                {
                    // Bỏ qua cột "CapThem" khi kiểm tra trùng
                    if (col.ColumnName == "CapThem")
                        continue;

                    if (!dr[col.ColumnName].Equals(row[col.ColumnName]))
                    {
                        isDuplicate = false;
                        break; // Nếu có một cột khác nhau, thoát khỏi vòng lặp
                    }
                }

                if (isDuplicate)
                    return true; // Nếu tìm thấy dòng trùng, trả về true
            }
            return false;
        }
        private void NapLaiDong()
        {
            loadCapPhat(_magop, isVatTu);
            loadLichSu(_magop, isVatTu);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }


        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //XoaDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LuuDong();
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = button1;

                DataTable tbl = gCLichSu.DataSource as DataTable;
                var filteredRows = tbl.AsEnumerable()
                                   .Where(row => row.Field<string>("IsNew") == "them");
                DataTable tbl2 = new DataTable();
                if (filteredRows.Any())
                {
                    tbl2 = filteredRows.CopyToDataTable();
                }
                else
                {
                    tbl2 = tbl.Clone();
                }
                if (tbl2.Columns.Contains("IsNew"))
                {
                    tbl2.Columns.Remove("IsNew");
                }
                if (tbl2.Columns.Contains("IsNL"))
                {
                    tbl2.Columns.Remove("IsNL");
                }
                if (tbl2.Rows.Count == 0 || tbl2 == null) return;
                for (int i = 0; i < tbl2.Rows.Count; i++)
                {
                    if (tbl2.Rows[i]["CapThem"] != DBNull.Value && tbl2.Rows[i]["CapThem"] != null)
                    {
                        if (float.TryParse(tbl2.Rows[i]["CapThem"].ToString(), out float dmhh))
                        {
                            tbl2.Rows[i]["CapThem"] = Math.Round(dmhh, 2); // Làm tròn giá trị bất kể dương hay âm
                        }
                        else
                        {
                            tbl2.Rows[i]["CapThem"] = 0; // Nếu không parse được (vd: không phải số), gán 0
                        }
                    }
                    else
                    {
                        tbl2.Rows[i]["CapThem"] = 0; // Nếu giá trị là null hoặc DBNull, gán 0
                    }
                }


                //post
                string url = string.Format("{0}", URL + "CanDoiDonHangTong/PostDinhMucNhapCapThem");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl2); }).Result;
                if (msResult.ToLower() == "true")
                {

                    clsWaitForm.ShowSuccessForm(this, 2000);
                    loadCapPhat(_magop, isVatTu);
                    loadLichSu(_magop, isVatTu);


                }
                else XtraMessageBox.Show(msResult);

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }





        private void gVNPL2_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            CellValue(e, gVVatTu);
        }

        private void CellValue(CellValueChangedEventArgs e, GridView grv)
        {

            if (e.Column.FieldName == "IsNL") // Chỉ xử lý cho cột bạn cần
            {
                grv.RefreshRow(e.RowHandle); // Làm mới hàng hiện tại
            }

            if (e.Column.FieldName == "CapThem") // Chỉ xử lý cho cột bạn cần
            {
                DataRow dr = gVLichSu.GetFocusedDataRow();
                if (dr != null)
                {
                    object maNPL = dr["MaNPL"];

                    int rowHandle = gVVatTu.LocateByValue("MaNPL", maNPL);

                    if (rowHandle >= 0)
                    {
                        DataRow foundRow = gVVatTu.GetDataRow(rowHandle);

                        if (float.TryParse(dr["CapThem"]?.ToString(), out float capThem1) &&
               float.TryParse(foundRow["CapThem"]?.ToString(), out float capThem2))
                        {

                            float sum = capThem1 + capThem2;

                            // Nếu tổng âm, hiển thị cảnh báo
                            if (sum < 0)
                            {
                                MessageBox.Show($"Tổng cấp thêm không thể âm!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }
                    }
                }

            }

        }
        private void gVNPL2_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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


        private void gVNPL2_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
        }







        #region  xử lý copy paste
        private void CopyPast(PopupMenuShowingEventArgs e, GridView grv)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (grv.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa cấp thêm", ItemXoa_Click);
                        e.Menu.Items.Add(menuCopyItem);



                    }

                }
            }
        }
        bool checkPopupMenu = true;
        private void gVPhuLieu_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            checkPopupMenu = true;
            CopyPast(e, gVLichSu);

        }
        private void gVNPL2_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            checkPopupMenu = false;
            CopyPast(e, gVVatTu);

        }

        private void ItemXoa_Click(object sender, EventArgs e)
        {
            DataRow dr = gVLichSu.GetFocusedDataRow();
            if (dr == null) return;
            string url = string.Format("{0}?para={1}&&para1={2}", URL + "CanDoiDonHangTong/DeleteCapThemByDot", dr["MaNPL"] == null ? "" : dr["MaNPL"].ToString(),
                    dr["Dot"] == null ? "" : dr["Dot"].ToString());
            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            NapLaiDong();
        }




        private void gVNPL2_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }
        bool checkFlag = true;

        private void gVNPL2_MouseDown(object sender, MouseEventArgs e)
        {
            checkGridView = true;
            GridHitInfo hitInfo = gVVatTu.CalcHitInfo(e.Location);

            if (hitInfo.InRowCell == true)
            {
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "IsNL")
                {
                    gVVatTu.FocusedColumn = hitInfo.Column;
                    gVVatTu.FocusedRowHandle = hitInfo.RowHandle;
                    gVVatTu.ShowEditor(); // Kích hoạt editor

                    if (gVVatTu.ActiveEditor is ImageComboBoxEdit editor)
                    {
                        // Hiển thị popup của editor
                        editor.ShowPopup();
                    }
                }
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "btnThem")
                {
                    gVVatTu.FocusedColumn = hitInfo.Column;
                    gVVatTu.FocusedRowHandle = hitInfo.RowHandle;
                    DataRow dr = gVVatTu.GetFocusedDataRow();
                    ThemDong(isVatTu, dr);
                }
            }
        }

        private void gVNPL2_ShowingEditor(object sender, CancelEventArgs e)
        {

            if (gVVatTu.FocusedColumn != null && gVVatTu.FocusedColumn.FieldName == "IsNL")
            {
                gCVatTu.BeginInvoke(new Action(() =>
                {
                    if (gVVatTu.ActiveEditor is ImageComboBoxEdit editor)
                    {
                        // Hiển thị popup của editor
                        editor.ShowPopup();
                    }
                }));
            }
        }

        private void gVNPL2_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.RowHandle >= 0) // Kiểm tra nếu đây là một dòng hợp lệ
            {
                // Lấy dòng hiện tại từ GridView
                DataRow row = view.GetDataRow(e.RowHandle);

                if (row != null && row.Table.Columns.Contains("IsNew") && row["IsNew"] != DBNull.Value)
                {
                    // Nếu cột "IsNew" có giá trị true thì đổi màu nền
                    if (row["IsNew"] == "copy")
                    {
                        //e.Appearance.BackColor = Color.LightYellow; // Đổi màu nền thành vàng nhạt
                    }
                    else if (row["IsNew"] == "them")
                    {
                        //e.Appearance.BackColor = Color.LightCyan;
                    }
                    else if (row["IsNew"] == "copycp")
                    {
                        //e.Appearance.BackColor = Color.LightGreen;
                    }
                }
            }
        }


        private void checkEditCapPhat_Click(object sender, EventArgs e)
        {

        }




        private void AddRow(string data, int rowHandle, GridView view)
        {
            if (string.IsNullOrEmpty(data)) return;

            string[] rowData = data.Split('\t');
            int columnIndex = view.FocusedColumn.VisibleIndex;

            int originalRowHandle = view.GetDataSourceRowIndex(rowHandle); // Đảm bảo lấy dòng gốc khi merge

            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= view.VisibleColumns.Count) break;

                try
                {
                    GridColumn targetColumn = view.VisibleColumns[columnIndex + i];

                    if (targetColumn.FieldName == "IsNL")
                    {
                        bool isNLValue = rowData[i].Trim() == "Nguyên liệu";
                        view.SetRowCellValue(originalRowHandle, targetColumn, isNLValue);
                    }
                    else
                    {
                        Type fieldType = view.Columns[targetColumn.FieldName].ColumnType;

                        if (fieldType == typeof(int))
                        {
                            if (int.TryParse(rowData[i].Replace(",", ""), out int intValue))
                            {
                                view.SetRowCellValue(originalRowHandle, targetColumn, intValue);
                            }
                            else
                            {

                                this.ActiveControl = button1;

                            }
                        }
                        else if (fieldType == typeof(float))
                        {
                            if (float.TryParse(rowData[i].Replace(",", ""), out float floatValue))
                            {
                                view.SetRowCellValue(originalRowHandle, targetColumn, floatValue);
                            }
                            else
                            {

                                this.ActiveControl = button1;

                            }
                        }
                        else
                        {
                            view.SetRowCellValue(originalRowHandle, targetColumn, rowData[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi dán dữ liệu vào cột '{view.VisibleColumns[columnIndex + i].FieldName}': {ex.Message}",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.ActiveControl = button1;
                    return;
                }
            }
        }




        private void gCNPL2_ProcessGridKey(object sender, KeyEventArgs e)
        {
            Keypass(e, gCVatTu);
        }
        private void gCPhuLieu_ProcessGridKey(object sender, KeyEventArgs e)
        {
            Keypass(e, gCLichSu);
        }
        private void Keypass(KeyEventArgs e, GridControl grc)
        {
            GridView view = grc.MainView as GridView;
            if (e.Control && e.KeyCode == Keys.V)
            {
                string clipboardData = Clipboard.GetText();
                byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
                string decodedClipboardData = Encoding.UTF8.GetString(bytes);
                string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                // string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length < 1) return;
                int startRow = view.FocusedRowHandle;
                foreach (string row in data)
                {
                    checkFlag = false;

                    AddRow(row, startRow++, view);
                    if (!view.IsValidRowHandle(startRow)) break;
                }
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = false;
                // Lấy GridView hiện tại
                GridView view2 = grc.MainView as GridView;
                if (view2 == null) return;

                // Lấy dòng và cột đang được chọn
                int rowHandle = view2.FocusedRowHandle;
                GridColumn col = view2.FocusedColumn;

                if (rowHandle >= 0 && col != null)
                {
                    // Nếu merge cell đang bật, tìm dòng gốc chứa dữ liệu thực sự
                    if (view2.OptionsView.AllowCellMerge)
                    {
                        while (rowHandle > 0 && view2.GetRowCellValue(rowHandle, col)
                               .Equals(view2.GetRowCellValue(rowHandle - 1, col)))
                        {
                            rowHandle--;
                        }
                    }

                    // Lấy giá trị của ô cần copy
                    object cellValue = view2.GetRowCellValue(rowHandle, col);
                    if (cellValue != null)
                    {
                        Clipboard.SetText(cellValue.ToString());
                    }
                    e.SuppressKeyPress = true;
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu để sao chép!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void gVPhuLieu_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            CellValue(e, gVLichSu);
        }

        private void gVPhuLieu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {

        }

        private void gVPhuLieu_MouseDown(object sender, MouseEventArgs e)
        {
            checkGridView = false;
            GridHitInfo hitInfo = gVLichSu.CalcHitInfo(e.Location);

            if (hitInfo.InRowCell == true)
            {
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "IsNL")
                {
                    gVLichSu.FocusedColumn = hitInfo.Column;
                    gVLichSu.FocusedRowHandle = hitInfo.RowHandle;
                    gVLichSu.ShowEditor(); // Kích hoạt editor

                    // Kiểm tra nếu ActiveEditor có kiểu RepositoryItemImageComboBox thì hiển thị popup
                    /*  var activeEditor = gVNPL2.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
                      if (activeEditor?.Properties is RepositoryItemImageComboBox repositoryItemImageComboBox)
                      {
                          // Gọi phương thức ShowPopup() hoặc bất kỳ logic nào bạn muốn
                          activeEditor.Show();
                      }*/
                    if (gVLichSu.ActiveEditor is ImageComboBoxEdit editor)
                    {
                        // Hiển thị popup của editor
                        editor.ShowPopup();
                    }
                }
            }
        }


        private void gCNPL2_EditorKeyPress(object sender, KeyPressEventArgs e)
        {

            if (gVVatTu.FocusedColumn.FieldName == "CapThem")
                FocusFieldName(e, gVVatTu, 2);

        }
        private void FocusFieldName(KeyPressEventArgs e, GridView grv, int soluong)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b')
            {
                // Ngăn chặn các ký tự không hợp lệ
                e.Handled = true;
            }
            else
            {
                string currentValue = grv.EditingValue?.ToString() ?? ""; // Lấy giá trị hiện tại đang nhập

                // Kiểm tra nếu ký tự là '-' (dấu âm)
                if (e.KeyChar == '-')
                {
                    // Không cho phép nhập '-' nếu không phải ở đầu chuỗi
                    if (currentValue.Length > 0)
                    {
                        e.Handled = true;
                    }
                }

                // Nếu người dùng nhập dấu '.' thì cần kiểm tra xem nó đã tồn tại hay chưa
                if (e.KeyChar == '.')
                {
                    if (currentValue.Contains("."))
                    {
                        e.Handled = true; // Không cho phép nhập thêm dấu '.' nếu đã có
                    }
                }

                // Kiểm tra số lượng chữ số sau dấu '.'
                string[] newValue = (currentValue + e.KeyChar).Split('.');
                if (newValue.Length > 1 && e.KeyChar != '\b')
                {
                    int CountSoLuong = newValue[1].Length;
                    if (CountSoLuong > soluong)
                    {
                        e.Handled = true; // Giới hạn số lượng chữ số sau dấu '.'
                    }
                }
            }


        }
        private bool IsVietnameseChar(char c)
        {
            string vietnameseChars = "àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ";
            return vietnameseChars.ToLower().Contains(c.ToString().ToLower());
        }

        private void gCPhuLieu_EditorKeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Naplai1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void gVPhuLieu_ShowingEditor(object sender, CancelEventArgs e)
        {

            if (gVLichSu.FocusedColumn != null && gVLichSu.FocusedColumn.FieldName == "IsNL")
            {
                gCLichSu.BeginInvoke(new Action(() =>
                {
                    if (gVLichSu.ActiveEditor is ImageComboBoxEdit editor)
                    {
                        // Hiển thị popup của editor
                        editor.ShowPopup();
                    }
                }));
            }
        }

        private void gVPhuLieu_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.RowHandle >= 0) // Kiểm tra nếu đây là một dòng hợp lệ
            {
                // Lấy dòng hiện tại từ GridView
                DataRow row = view.GetDataRow(e.RowHandle);

                if (row != null && row.Table.Columns.Contains("IsNew") && row["IsNew"] != DBNull.Value)
                {
                    // Nếu cột "IsNew" có giá trị true thì đổi màu nền
                    if (row["IsNew"] == "copy")
                    {
                        //e.Appearance.BackColor = Color.LightYellow; // Đổi màu nền thành vàng nhạt
                    }
                    else if (row["IsNew"] == "them")
                    {
                        //e.Appearance.BackColor = Color.LightCyan;
                    }
                    else if (row["IsNew"] == "copycp")
                    {
                        //e.Appearance.BackColor = Color.LightGreen;
                    }
                }
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = cbBoxLoaiVT.Text;
            labelVatTu.Text = selectedValue;
            labelLichSu.Text = "Lịch sử Cấp thêm " + selectedValue;
            if (selectedValue == "Nguyên liệu")
            {

                isVatTu = true;
                loadLichSu(_magop, isVatTu);
                loadCapPhat(_magop, isVatTu);
            }
            else if (selectedValue == "Phụ liệu")
            {
                isVatTu = false;
                loadLichSu(_magop, isVatTu);
                loadCapPhat(_magop, isVatTu);
            }


        }

        private void gVNPL2_CellMerge(object sender, CellMergeEventArgs e)
        {

        }

     
        private void gVPhuLieu_CellMerge(object sender, CellMergeEventArgs e)
        {

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            LuuDong();
        }

        private void btnThem_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private string ClipboardData
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return "";

                if (iData.GetDataPresent(DataFormats.Text))
                    return (string)iData.GetData(DataFormats.Text);
                return "";
            }
            set
            {
                Clipboard.SetDataObject(value);
            }
        }

        //

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
        private void setTypeTxtEditDinhMuc(object sender, KeyPressEventArgs e)
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

    
        private string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            string result = text.Trim().ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ", "y");
            result = Regex.Replace(result, "đ", "d");
            result = result.ToUpper();
            return result;
        }
        private void ConfigureComboBoxLoaiVT()
        {
            RepositoryItemImageComboBox comboBox = new RepositoryItemImageComboBox();
            comboBox.Items.Add(new ImageComboBoxItem("Nguyên liệu", true));  // Hiển thị "Nguyên liệu" nhưng giá trị là true
            comboBox.Items.Add(new ImageComboBoxItem("Phụ liệu", false));   // Hiển thị "Phụ liệu" nhưng giá trị là false
            comboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBox.EditValueChanged += (s, e) =>
            {
                this.ActiveControl = this.button1;
            };
            comboBox.ImmediatePopup = true;
            comboBox.TextEditStyle = TextEditStyles.Standard;
            gVVatTu.Columns["IsNL"].ColumnEdit = comboBox;
            gVLichSu.Columns["IsNL"].ColumnEdit = comboBox;

        }

        private void loadSoLuong()
        {
            string url = string.Format("{0}?para={1}&&para1={2}", URL + "CanDoiDonHangTong/GetSoLuongNhap", _maLSX, _magop);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null) return;
            txtSoLuong.Text = tbl.Rows[0]["SoLuong"].ToString();
        }
        private void loadSoBooking()
        {
            string url = string.Format("{0}?para={1}", URL + "CanDoiDonHangTong/GetSoBookingNhap", _madh);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null) return;
            string rs = string.Empty;
            if (tbl.Rows.Count > 0)
            {
                rs = string.Join("|",
                            tbl.AsEnumerable().Select(r => r["BookingMaHang"].ToString()));
            }
            txtSoBooking.Text = rs.ToString();
        }

        private string loadDotCapThem(string manpl)
        {
            string url = string.Format("{0}?para={1}", URL + "CanDoiDonHangTong/GetVatTuDotCapThem", manpl);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return "1";
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null) return "1";
            return tbl.Rows[0]["Dot"].ToString();
        }
        private void loadCapPhat(string madh, bool IsVT, string malenhsx = "")
        {
            malenhsx = _maLSX;
            string url = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPL", madh, malenhsx);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            tblSave.Clear();
            foreach (DataRow row in tbl.Rows)
            {
                DataRow _dr = tblSave.NewRow();
                
                _dr["ID"] = row["ID"];
                _dr["MaDH"] = row["MaDH"];
                _dr["MaLenhSanXuat"] = row["MaLenhSanXuat"];
                _dr["MaNPL"] = row["MaNPL"];
                _dr["NguoiTao"] = row["NguoiTao"];
                //_dr["MaKH"] = row["MaKH"];
                _dr["MaVT"] = row["MaVT"];
                _dr["TenVT"] = row["TenVT"];
                _dr["MaMau"] = row["MaMau"];
                _dr["KhoVai"] = row["KhoVai"];
                _dr["MaDV"] = row["MaDV"];
                _dr["DinhMuc"] = row["DinhMuc"];
                _dr["SoLuong"] = row["SoLuong"];
                _dr["CapPhat"] = row["CapPhat"];
                _dr["CapThem"] = row["CapThem"];
                _dr["GhiChu"] = row["GhiChu"];
                _dr["IsXetDuyet"] = row["IsXetDuyet"];
                _dr["NguoiXet"] = row["NguoiXet"];
                _dr["NguoiHuy"] = row["NguoiHuy"];
                _dr["NgayXet"] = row["NgayXet"];
                _dr["NgayHuy"] = row["NgayHuy"];
                _dr["ID_MaNPL"] = row["ID_MaNPL"];
                _dr["IsNL"] = row["IsNL"] != null && bool.TryParse(row["IsNL"].ToString(), out bool isNL) ? isNL : true;
                _dr["ThucNhan"] = row["ThucNhan"];
                _dr["TenTAVT"] = row["TenTAVT"];
                _dr["NhomNPL"] = row["NhomNPL"];
                _dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
                _dr["MaNhomVT"] = row["MaNhomVT"];
                _dr["MaVTTheoCayVai"] = row["MaVTTheoCayVai"];
                _dr["IsNew"] = "khongsua";
                tblSave.Rows.Add(_dr);
            }

            tblNguyenLieu = tblSave.AsEnumerable()
                    .Where(x => Convert.ToBoolean(x["IsNL"]))
                    .Any() ? tblSave.AsEnumerable()
                                    .Where(x => Convert.ToBoolean(x["IsNL"]))
                                    .CopyToDataTable()
                            : tblSave.Clone();
            tblPhuLieu = tblSave.AsEnumerable()
                    .Where(x => !Convert.ToBoolean(x["IsNL"]))
                    .Any() ? tblSave.AsEnumerable()
                                    .Where(x => !Convert.ToBoolean(x["IsNL"]))
                                    .CopyToDataTable()
                        : tblSave.Clone();

            gCVatTu.DataSource = IsVT? (tblNguyenLieu==null?null: tblNguyenLieu): (tblPhuLieu==null?null:tblPhuLieu);

        }
        private void loadLichSu(string madh, bool IsVT, string malenhsx = "")
        {
            malenhsx = _maLSX;
            string url = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPLCapThem", madh, malenhsx);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            tblLichSu.Clear();
            foreach (DataRow row in tbl.Rows)
            {
                DataRow _dr = tblLichSu.NewRow();

                _dr["ID"] = row["ID"];
                _dr["MaDH"] = row["MaDH"];
                _dr["MaLenhSanXuat"] = row["MaLenhSanXuat"];
                _dr["MaNPL"] = row["MaNPL"];
                _dr["NguoiTao"] = row["NguoiTao"];
                _dr["NgayTao"] = row["ConvertDay"].ToString();
                _dr["MaVT"] = row["MaVT"];
                _dr["TenVT"] = row["TenVT"];
                _dr["MaMau"] = row["MaMau"];
                _dr["KhoVai"] = row["KhoVai"];
                _dr["MaDV"] = row["MaDV"];
                _dr["DinhMuc"] = row["DinhMuc"];
                _dr["SoLuong"] = row["SoLuong"];
                _dr["CapPhat"] = row["CapPhat"];
                _dr["CapThem"] = row["CapThem"];
                _dr["GhiChu"] = row["GhiChu"];
                _dr["IsXetDuyet"] = row["IsXetDuyet"];
                _dr["NguoiXet"] = row["NguoiXet"];
                _dr["NguoiHuy"] = row["NguoiHuy"];
                _dr["NgayXet"] = row["NgayXet"];
                _dr["NgayHuy"] = row["NgayHuy"];
                _dr["ID_MaNPL"] = row["ID_MaNPL"];
                _dr["IsNL"] = row["IsNL"] != null && bool.TryParse(row["IsNL"].ToString(), out bool isNL) ? isNL : true;
                _dr["ThucNhan"] = row["ThucNhan"];
                _dr["TenTAVT"] = row["TenTAVT"];
                _dr["NhomNPL"] = row["NhomNPL"];
                _dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
                _dr["MaNhomVT"] = row["MaNhomVT"];
                _dr["MaVTTheoCayVai"] = row["MaVTTheoCayVai"];
                _dr["Dot"] = row["Dot"];
                _dr["IsNew"] = "khongsua";
                tblLichSu.Rows.Add(_dr);
            }
      
            tblLichSuNL = tblLichSu.AsEnumerable()
                 .Where(x => Convert.ToBoolean(x["IsNL"]))
                 .Any() ? tblLichSu.AsEnumerable()
                                 .Where(x => Convert.ToBoolean(x["IsNL"]))
                                 .CopyToDataTable()
                         : tblLichSu.Clone();
           
            tblLichSuPL = tblLichSu.AsEnumerable()
                 .Where(x => !Convert.ToBoolean(x["IsNL"]))
                 .Any() ? tblLichSu.AsEnumerable()
                                 .Where(x => !Convert.ToBoolean(x["IsNL"]))
                                 .CopyToDataTable()
                         : tblLichSu.Clone();
            gCLichSu.DataSource = IsVT ? (tblLichSuNL == null ? null : tblLichSuNL) : (tblLichSuPL == null ? null : tblLichSuPL);

        }

    }
}
