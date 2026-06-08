using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmNhapCapPhatDHTThem : DevExpress.XtraEditors.XtraForm
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

        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        bool isCal = true;
        DataTable tblAddRow;
        DataTable tblSave;
        DataTable tblGoc;
        DataTable tblVTGoiY;
        public frmNhapCapPhatDHTThem()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }

        public frmNhapCapPhatDHTThem(string _magop, string _nguoitao, string _maLSX, string _tenKH, string _mahang, string _madh, string _malenh, string _soluong, string _tenhang, int _iskeove, string _dot)
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
            tblAddRow = new DataTable();
            ConfigureSearchLookUpEditCopyCP();
            this.ActiveControl = button1;
   
        }

        private List<ActionControl> InitActionKeyDown()
        {

            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
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

            ConfigureComboBoxLoaiVT();
            loadCapPhat(_magop, true);
            txtDonHang.Text = _madh;
            txtMaHang.Text = _tenhang;
            txtLenhSX.Text = _malenh;
            txtDot.Text = _dot;
            loadSoLuong();
            loadSoBooking();
            //CreateSearchLookUpGoiY("TenVT", "MaVT", "MaVT");
            CreateSearchLookUpGoiY("TenVT", "TenVT", "TenVT");
            CreateSearchLookUpGoiY("Mau", "CodeMauKH", "MaMau");
            CreateSearchLookUpGoiY("KhoSize", "KhoSize", "KhoVai");
            CreateSearchLookUpGoiY("DVTinh", "TenDVCL", "MaDV");
            CreateSearchLookUpGoiY("Nhom", "TenNhom", "NhomNPL");
            
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
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("QtyES", typeof(float));
            tblSave.Columns.Add("CostES", typeof(float));
            tblSave.Columns.Add("NeedES", typeof(float));
            tblSave.Columns.Add("ToTalRec", typeof(float));
            tblSave.Columns.Add("IsNL", typeof(bool));
            tblSave.Columns.Add("MaKH", typeof(string));
            tblSave.Columns.Add("ThucNhan", typeof(float));
            tblSave.Columns.Add("TenTAVT", typeof(string));
            tblSave.Columns.Add("NhomNPL", typeof(string));
            tblSave.Columns.Add("DinhMucHaoHut", typeof(float));
            tblSave.Columns.Add("IsNew", typeof(string));
            tblAddRow = tblSave.Clone();
        }

        private void ThemDong(bool isNL)
        {
            try
            {
                if (isNL)
                {
                    DataTable tblNL1 = gCNPL2.DataSource as DataTable;
                    if (tblNL1 == null)
                        tblNL1 = tblSave.Clone();

                    DataRow row = AddNewRow(isNL, tblNL1);
                    //tblNL1.Rows.Add(row);
                    tblNL1.Rows.InsertAt(row, 0);
                    gCNPL2.DataSource = tblNL1;
                }
                else
                {
                    DataTable tblNL2 = gCPhuLieu.DataSource as DataTable;
                    if (tblNL2 == null)
                        tblNL2 = tblSave.Clone();
                    DataRow row = AddNewRow(isNL, tblNL2);
                    //tblNL2.Rows.Add(row);
                    tblNL2.Rows.InsertAt(row, 0);
                    gCPhuLieu.DataSource = tblNL2;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private DataRow AddNewRow(bool isNL, DataTable tbl)
        {
            DataRow row = tbl.NewRow();
            row["ID"] = 0;
            row["MaDH"] = _magop;
            row["MaLenhSanXuat"] = _maLSX;
            row["NguoiTao"] = _nguoitao;
            row["MaKH"] = _tenKH;
            row["IsNL"] = isNL;
            row["CapThem"] = 0;
            row["ThuHoi"] = 0;
            row["TrangThai"] = 0;
            row["ThucNhan"] = 0;
            row["DinhMucHaoHut"] = 0;
            row["IsNew"] = "them";
            //row["SoLuong"] = txtSoLuong.Text.ToString() == "" ? 0 : Convert.ToInt32(txtSoLuong.Text);
            return row;
        }
        private void NapLaiDong()
        {
            //if(tblGoc.Rows.Count==0|| tblGoc==null)
            //{
            //    gCNPL2.DataSource = null;
            //    return;
            //}    
            //gCNPL2.DataSource = tblGoc;
            loadCapPhat(_magop, true);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }


        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //XoaDong();
        }
        private async void XoaDong()
        {
            try
            {
                //
                if (_iskeove == 1)
                {
                    XtraMessageBox.Show("Lệnh này đã được thực hiện sản xuất.\nKhông thể xóa được!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa tất cả không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string url = string.Format("{0}?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/DeleteNPL", _magop == null ? "" : _magop.ToString(),
                        _maLSX == null ? "" : _maLSX.ToString());
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;

                    if (result.ToLower() == "true")
                    {
                        tblSave.Clear();
                        gCNPL2.DataSource = tblSave.Copy();
                    }

                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LuuDong();
        }
        private void LuuDong(GridControl grc, GridView grv)
        {

            try
            {
                this.ActiveControl = button1;
                for (int i = 0; i < grv.RowCount; i++)
                {
                    var rowHandle = grv.GetRowHandle(i);

                    // Kiểm tra dòng hiện tại có hợp lệ hay không
                    if (!grv.IsValidRowHandle(rowHandle))
                        continue;

                    // Duyệt qua tất cả các cột
                    for (int j = 0; j < grv.Columns.Count; j++)
                    {
                        var column = grv.Columns[j];
                        if ( column.FieldName == "TenVT" || column.FieldName == "KhoVai"
                            || column.FieldName == "MaDV" || column.FieldName == "DinhMuc" || column.FieldName == "SoLuong" || column.FieldName == "CapPhat"
                           )
                        {
                            var cellValue = grv.GetRowCellValue(rowHandle, column);
                            if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                            {
                                MessageBox.Show($"Lỗi: Dòng {i + 1}, Cột \"{column.Caption}\" bị trống.",
                                                "Thông báo lỗi",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }
                DataTable tbl = grc.DataSource as DataTable;
                if (HasDuplicateRows(tbl))
                {
                    return;
                }

                DataTable tbl2 = tbl.Copy();
                tbl2.Select("IsNew = 'khongsua'").ToList().ForEach(row => row.Delete());

                // Cập nhật trạng thái của bảng
                tbl2.AcceptChanges();
                if (tbl2.Columns.Contains("IsNew"))
                {
                    tbl2.Columns.Remove("IsNew");
                }
              /*  if (!checktbl2Save(tbl2))
                { 
                    XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại Mã vật tư và Tên vật tư", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }*/
                if(tbl2==null|tbl2.Rows.Count==0)
                {
                    //XtraMessageBox.Show("Không có hàng nào được thêm vào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }    
                //post
                string url = string.Format("{0}", URL + "CanDoiDonHangTong/PostDinhMucNhap");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl2); }).Result;
                if (msResult.ToLower() == "true")
                {

                    clsWaitForm.ShowSuccessForm(this, 2000);
                    //this.DialogResult = DialogResult.OK;
                    loadCapPhat(_magop, true);
                    CreateSearchLookUpGoiY("TenVT", "TenVT", "TenVT");
                    CreateSearchLookUpGoiY("Mau", "CodeMauKH", "MaMau");
                    CreateSearchLookUpGoiY("KhoSize", "KhoSize", "KhoVai");
                    CreateSearchLookUpGoiY("DVTinh", "TenDVCL", "MaDV");
                    CreateSearchLookUpGoiY("Nhom", "TenNhom", "NhomNPL");

                }
                else XtraMessageBox.Show(msResult);

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
       /* public bool checktbl2Save(DataTable tbl2)
        {
            if (tblVTGoiY == null || tblVTGoiY.Rows.Count == 0) return true;
            foreach (DataRow rowTbl2 in tbl2.Rows)
            {
                string maVT_tbl2 = rowTbl2["MaVT"].ToString();
                string tenVT_tbl2 = rowTbl2["TenVT"].ToString();

                foreach (DataRow rowGoiY in tblVTGoiY.Rows)
                {
                    string maVT_tblTenVTGoiY = rowGoiY["MaVT"].ToString();
                    string tenVT_tblTenVTGoiY = rowGoiY["TenVT"].ToString();

                    // Kiểm tra điều kiện: nếu chỉ có 1 trong 2 bằng nhau thì return false
                    bool isMaVTEqual = maVT_tbl2 == maVT_tblTenVTGoiY;
                    bool isTenVTEqual = tenVT_tbl2 == tenVT_tblTenVTGoiY;

                    if ((isMaVTEqual && !isTenVTEqual) || (!isMaVTEqual && isTenVTEqual))
                    {
                        return false; // Chỉ có 1 trong 2 giống nhau
                    }
                }
            }

            return true;
        }*/

        public bool HasDuplicateRows(DataTable tblSave)
        {
            // Kiểm tra nếu bảng rỗng hoặc không đủ cột
            if (tblSave == null || tblSave.Rows.Count == 0)
            {
                XtraMessageBox.Show("Dữ liệu trống, không có hàng nào đc lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            // Sử dụng HashSet để lưu trữ các giá trị đã kiểm tra
            var rowSet = new HashSet<string>();

            foreach (DataRow row in tblSave.Rows)
            {
                // Ghép giá trị của 3 cột thành một chuỗi duy nhất để kiểm tra
                var key = $"{row["MaVT"]}_{row["TenVT"]}_{row["MaMau"]}_{row["KhoVai"]}";

                // Nếu HashSet đã chứa key, nghĩa là có trùng lặp
                if (rowSet.Contains(key))
                {
                    XtraMessageBox.Show("Có hàng trùng lặp, vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }


                // Thêm key vào HashSet
                rowSet.Add(key);
            }

            // Không có hàng nào trùng lặp
            return false;
        }


        private void checkEditCapPhat_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void gVNPL2_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            CellValue(sender, e, gVNPL2);
        }

        private void CellValue(object sender, CellValueChangedEventArgs e, GridView grv)
        {
            if (isCal)
            {
                if (e.Column.FieldName == "SoLuong")
                {
                    var DinhMucValue = grv.GetRowCellValue(e.RowHandle, "DinhMuc");
                    decimal DinhMucHaoHutValue = decimal.TryParse(grv.GetRowCellValue(e.RowHandle, "DinhMucHaoHut").ToString(),out decimal dmhh) && dmhh > 0 ? dmhh:1;
                    
                    if (DinhMucValue != null && !string.IsNullOrEmpty(DinhMucValue.ToString()))
                    {
                        var soLuongValue = Convert.ToDecimal(e.Value);
                        decimal capPhat = soLuongValue * Convert.ToDecimal(DinhMucValue)* DinhMucHaoHutValue;
                        grv.SetRowCellValue(e.RowHandle, "CapPhat", (float)Math.Round(capPhat, 2));
                    }

                }
                if (e.Column.FieldName == "DinhMuc")
                {
                    var soLuongValue = grv.GetRowCellValue(e.RowHandle, "SoLuong");
                    decimal DinhMucHaoHutValue = decimal.TryParse(grv.GetRowCellValue(e.RowHandle, "DinhMucHaoHut").ToString(), out decimal dmhh) && dmhh > 0 ? dmhh : 1;
                    if (soLuongValue != null && !string.IsNullOrEmpty(soLuongValue.ToString()))
                    {
                        Single capPhat = Convert.ToSingle(e.Value) * Convert.ToSingle(soLuongValue)* Convert.ToSingle(DinhMucHaoHutValue);
                        grv.SetRowCellValue(e.RowHandle, "CapPhat", (float)Math.Round(capPhat, 2));
                    }

                }
                if (e.Column.FieldName == "DinhMucHaoHut")
                {
                    var soLuongValue = grv.GetRowCellValue(e.RowHandle, "SoLuong");
                    var DinhMucValue = grv.GetRowCellValue(e.RowHandle, "DinhMuc");
                    decimal DinhMucHaoHutValue = decimal.TryParse(grv.GetRowCellValue(e.RowHandle, "DinhMucHaoHut").ToString(), out decimal dmhh) && dmhh > 0 ? dmhh : 1;
                    if (soLuongValue != null && !string.IsNullOrEmpty(soLuongValue.ToString())&& DinhMucValue != null && !string.IsNullOrEmpty(DinhMucValue.ToString()))
                    {
                        Single capPhat = Convert.ToSingle(DinhMucValue) * Convert.ToSingle(soLuongValue) * Convert.ToSingle(DinhMucHaoHutValue);
                        grv.SetRowCellValue(e.RowHandle, "CapPhat", (float)Math.Round(capPhat, 2));
                    }

                } 
            }
            if(e.Column.FieldName == "MaMau")
            {
                loadSLMau(grv);
            }    
            if (e.Column.FieldName == "IsNL") // Chỉ xử lý cho cột bạn cần
            {
                grv.RefreshRow(e.RowHandle); // Làm mới hàng hiện tại
            }
            GridView view = sender as GridView;

/*            // Tắt sự kiện trước khi cập nhật 

            try
            {
                if (e.Column.FieldName == "MaVT") // Nếu chọn _tenCot (ví dụ: MaVT)
                {

                    DataRow dataRow = grv.GetFocusedDataRow();
                    if (dataRow == null) return;
                    string maVT = dataRow["MaVT"].ToString();
                    if (maVT == null || maVT == "" || maVT == string.Empty) return;
                    if (tblVTGoiY == null || tblVTGoiY.Rows.Count == 0) return;
                    string TenVT = tblVTGoiY.AsEnumerable().FirstOrDefault(x => x["MaVT"].ToString() == m aVT)?["TenVT"].ToString();
                    if (TenVT == null || TenVT == "" || TenVT == string.Empty) return;
                    if (!string.IsNullOrWhiteSpace(TenVT))
                    {
                        dataRow["TenVT"] = TenVT;
                    }
                    this.ActiveControl = button1;
                }

                if (e.Column.FieldName == "TenVT") // Nếu chọn _tenCot (ví dụ: MaVT)
                {
                    DataRow dataRow = grv.GetFocusedDataRow();
                    if (dataRow == null) return;
                    string TenVT = dataRow["TenVT"].ToString();
                    if (TenVT == null || TenVT == "" || TenVT == string.Empty) return;
                    if (tblVTGoiY == null || tblVTGoiY.Rows.Count == 0) return;
                    string MaVT = tblVTGoiY.AsEnumerable().FirstOrDefault(x => x["TenVT"].ToString() == TenVT)?["MaVT"].ToString();
                    if (MaVT == null || MaVT == "" || MaVT == string.Empty) return;
                    if (!string.IsNullOrWhiteSpace(MaVT))
                    {
                        dataRow["MaVT"] = MaVT;
                    }
                    this.ActiveControl = button1;
                }
            }
            catch (Exception ex)
            {
              
            }*/
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
            /*if (gVNPL2.FocusedColumn.FieldName == "MaVT")
            {
                        
                if (e.Value != null)
                {
                    string originalText = e.Value.ToString();
                    string processedText = RemoveVietnameseTone(originalText);
                    e.Value = processedText; // Gán chuỗi đã xử lý lại vào ô
                }
            }*/
        }
        private void gCPhuLieu_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            /* if (gVPhuLieu.FocusedColumn.FieldName == "MaVT")
             {

                 if (e.Value != null)
                 {
                     string originalText = e.Value.ToString();
                     string processedText = RemoveVietnameseTone(originalText);
                     e.Value = processedText;
                 }
             }*/
        }
        private void gVNPL2_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "IsNL")
            {

            }
        }

        private void searchLookUpEditCopyCP_EditValueChanged(object sender, EventArgs e)
        {
            /* SearchLookUpEdit editor = sender as SearchLookUpEdit;

             if (editor != null)
             {
                 object row = editor.GetSelectedDataRow();

                 if (row is DataRowView dataRowView) // Kiểm tra nếu row là DataRowView
                 {

                     DataView dataView = dataRowView.DataView;
                     foreach (DataRowView drv in dataView)
                     {
                         if (drv["MaLenhSanXuat"].ToString() != editor.EditValue.ToString()) continue;
                         string madh = dataRowView["MaGop"].ToString();
                         string malenhsx = dataRowView["MaLenhSanXuat"].ToString();
                         loadCapPhat(madh, false, malenhsx);
                     }
                 }


                 var selectedValue = editor.EditValue;

             }*/


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

                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", ItemCopy_Click);
                        e.Menu.Items.Add(menuCopyItem);


                        DevExpress.Utils.Menu.DXMenuItem menuCopyDongItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", ItemCopyDong_Click);
                        e.Menu.Items.Add(menuCopyDongItem);
                        DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", ItemPaste_Click);
                        e.Menu.Items.Add(menuPasteItem);

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", ItemDelete_Click);
                        e.Menu.Items.Add(menuDeleteItem);

                    }

                }
            }
        }
        bool checkPopupMenu = true;
        private void gVPhuLieu_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            checkPopupMenu = true;
            CopyPast(e, gVPhuLieu);

        }
        private void gVNPL2_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            checkPopupMenu = false;
            CopyPast(e, gVNPL2);

        }

        private void ItemCopy_Click(object sender, EventArgs e)
        {
            // Lấy GridView hiện tại
            GridView view = (!checkPopupMenu ? gCNPL2.MainView : gCPhuLieu.MainView) as GridView;
            if (view == null) return;
            view.CopyToClipboard();
            //    // Lấy dòng và cột đang được chọn
            //    int rowHandle = view.FocusedRowHandle;
            //    GridColumn col = view.FocusedColumn;

            //    if (rowHandle >= 0 && col != null)
            //    {
            //        // Nếu merge cell đang bật, tìm dòng gốc chứa dữ liệu thực sự
            //        if (view.OptionsView.AllowCellMerge)
            //        {
            //            while (rowHandle > 0 && view.GetRowCellValue(rowHandle, col)
            //                   .Equals(view.GetRowCellValue(rowHandle - 1, col)))
            //            {
            //                rowHandle--;
            //            }
            //        }

            //        // Lấy giá trị của ô cần copy
            //        object cellValue = view.GetRowCellValue(rowHandle, col);
            //        if (cellValue != null && cellValue.ToString() != "")
            //        {
            //            Clipboard.SetText(cellValue.ToString());
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Không có dữ liệu để sao chép!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
        }
        private void ItemCopyDong_Click(object sender, EventArgs e)
        {
            // Lấy GridView hiện tại từ GridControl
            GridView view = (!checkPopupMenu ? gCNPL2.MainView : gCPhuLieu.MainView) as GridView;
            if (view == null) return;

            // Kiểm tra nếu chưa chọn bất kỳ dòng nào
            if (view.SelectedRowsCount == 0)
            {
                MessageBox.Show("Bạn chưa chọn hàng nào để sao chép!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy danh sách các dòng được chọn
            int[] selectedRowHandles = view.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;

            // Lấy DataTable từ GridControl
            DataTable tblGird = (!checkPopupMenu ? gCNPL2.DataSource : gCPhuLieu.DataSource) as DataTable;
            if (tblGird == null) return;

            // Sắp xếp danh sách các dòng được chọn để xử lý theo thứ tự tăng dần
            Array.Sort(selectedRowHandles);

            // Biến "offset" để điều chỉnh vị trí chèn khi thêm dòng mới
            int offset = 0;

            // Ngăn chặn GridView vẽ lại khi đang cập nhật dữ liệu
            view.BeginUpdate();

            try
            {
                // Duyệt qua từng dòng đã chọn và sao chép
                foreach (int rowHandle in selectedRowHandles)
                {
                    if (rowHandle >= 0) // Kiểm tra dòng hợp lệ
                    {
                        // Lấy dữ liệu từ dòng gốc trong GridView
                        DataRow selectedRow = tblGird.Rows[rowHandle + offset];

                        // Tạo dòng mới
                        DataRow newRow = tblGird.NewRow();

                        // Thực hiện ánh xạ dữ liệu từ dòng gốc sang dòng mới
                        newRow["ID"] = 0;
                        newRow["MaDH"] = selectedRow["MaDH"];
                        newRow["MaLenhSanXuat"] = selectedRow["MaLenhSanXuat"];
                        newRow["MaVT"] = selectedRow["MaVT"];
                        newRow["TenVT"] = selectedRow["TenVT"];
                        newRow["MaMau"] = selectedRow["MaMau"];
                        newRow["KhoVai"] = selectedRow["KhoVai"];
                        newRow["MaDV"] = selectedRow["MaDV"];
                        newRow["SoLuong"] = selectedRow["SoLuong"];
                        newRow["DinhMuc"] = selectedRow["DinhMuc"];
                        newRow["CapPhat"] = selectedRow["CapPhat"];
                        newRow["CapThem"] = selectedRow["CapThem"];
                        newRow["IsNL"] = selectedRow["IsNL"];
                        newRow["GhiChu"] = selectedRow["GhiChu"];
                        newRow["ThucNhan"] = selectedRow["ThucNhan"];
                        newRow["NhomNPL"] = selectedRow["NhomNPL"];
                        newRow["DinhMucHaoHut"] = selectedRow["DinhMucHaoHut"];
                        // Gán thêm các giá trị do bạn chỉ định
                        newRow["NguoiTao"] = GlobleData.UserName;
                        newRow["MaKH"] = _tenKH;
                        //newRow["IsNL"] = true;
                        newRow["IsNew"] = "copy";
                        // Xác định vị trí chèn dòng mới
                        int insertIndex = rowHandle + 1 + offset;
                        tblGird.Rows.InsertAt(newRow, insertIndex);

                        // Cập nhật offset (vì thêm dòng mới sẽ làm thay đổi vị trí của các dòng)
                        offset++;
                    }
                }

            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
               
            }
            finally
            {
                // Kích hoạt lại GridView để hiển thị thay đổi
                view.EndUpdate();
            }
        }
        private void ItemDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int focusedRowHandle = !checkPopupMenu ? gVNPL2.FocusedRowHandle : gVPhuLieu.FocusedRowHandle;
                if (focusedRowHandle >= 0)
                {

                    // Lấy index của dòng trong GridView
                    int rowIndex = !checkPopupMenu ? gVNPL2.GetDataSourceRowIndex(focusedRowHandle) : gVPhuLieu.GetDataSourceRowIndex(focusedRowHandle);
                    DataTable tblGrid = (!checkPopupMenu ? gCNPL2.DataSource : gCPhuLieu.DataSource) as DataTable;
                    if (tblGrid == null || tblGrid.Rows.Count == 0) return;
                    int idRow = (int.TryParse(tblGrid.Rows[rowIndex]["ID"]?.ToString(), out int id) ? id : 0);
                    if (idRow != 0)
                    {
                        if (_iskeove == 1)
                        {
                            XtraMessageBox.Show("Lệnh này đã được thực hiện sản xuất.\nKhông thể xóa được!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        string url = string.Format("{0}?id={1}", URL + "CanDoiDonHangTong/DeleteNPLNhap", id);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        string url2 = string.Format("{0}?para={1}", URL + "CanDoiDonHangTong/DeleteCapThem", tblGrid.Rows[rowIndex]["MaNPL"] == null ? "" : tblGrid.Rows[rowIndex]["MaNPL"].ToString());
                        string result2 = Task.Run(async () => { return await _clientExtension.DeletedAsync(url2); }).Result;
                    }

                    tblGrid.Rows.RemoveAt(rowIndex);


                }
            }
            catch (Exception ex)
            {
                //XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gVNPL2_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }
        bool checkFlag = true;

        private void gVNPL2_MouseDown(object sender, MouseEventArgs e)
        {
            checkGridView = true;
            GridHitInfo hitInfo = gVNPL2.CalcHitInfo(e.Location);

            if (hitInfo.InRowCell == true)
            {
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "IsNL")
                {
                    gVNPL2.FocusedColumn = hitInfo.Column;
                    gVNPL2.FocusedRowHandle = hitInfo.RowHandle;
                    gVNPL2.ShowEditor(); // Kích hoạt editor

                    // Kiểm tra nếu ActiveEditor có kiểu RepositoryItemImageComboBox thì hiển thị popup
                    /*  var activeEditor = gVNPL2.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
                      if (activeEditor?.Properties is RepositoryItemImageComboBox repositoryItemImageComboBox)
                      {
                          // Gọi phương thức ShowPopup() hoặc bất kỳ logic nào bạn muốn
                          activeEditor.Show();
                      }*/
                    if (gVNPL2.ActiveEditor is ImageComboBoxEdit editor)
                    {
                        // Hiển thị popup của editor
                        editor.ShowPopup();
                    }
                }
            }
        }

        private void gVNPL2_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            DataRowView row = view.GetFocusedRow() as DataRowView;

            if (row != null)
            {
                if (row["IsNew"] != null && row["IsNew"].ToString() == "khongsua")
                {
                    e.Cancel = true;
                }
            }
            if (gVNPL2.FocusedColumn != null && gVNPL2.FocusedColumn.FieldName == "IsNL")
            {
                gCNPL2.BeginInvoke(new Action(() =>
                {
                    if (gVNPL2.ActiveEditor is ImageComboBoxEdit editor)
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
        private void checkEditCapPhat2_Click(object sender, EventArgs e)
        {
            
        }
        private void TinhCapPhap(GridView grv)
        {
            for (int i = 0; i < grv.RowCount; i++)
            {
                var isNewValue = grv.GetRowCellValue(i, "IsNew");
                if (isNewValue != null && isNewValue.ToString().Equals("khongsua", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Bỏ qua dòng này nếu IsNew = "khongsua"
                }
                var soLuongValue = grv.GetRowCellValue(i, "SoLuong");
                var dinhMucValue = grv.GetRowCellValue(i, "DinhMuc");
                decimal DinhMucHaoHutValue = decimal.TryParse(grv.GetRowCellValue(i, "DinhMucHaoHut").ToString(), out decimal dmhh) && dmhh > 0 ? dmhh : 1;
                if (soLuongValue != null && dinhMucValue != null &&
                    !string.IsNullOrEmpty(soLuongValue.ToString()) &&
                    !string.IsNullOrEmpty(dinhMucValue.ToString()))
                {
                    if (decimal.TryParse(soLuongValue.ToString(), out decimal soLuongDec) &&
                        decimal.TryParse(dinhMucValue.ToString(), out decimal dinhMucDec))
                    {
                        decimal capPhat = soLuongDec * dinhMucDec* DinhMucHaoHutValue;
                        grv.SetRowCellValue(i, "CapPhat", capPhat);
                    }
                }
                else
                {
                    grv.SetRowCellValue(i, "CapPhat", 0);
                }
            }
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var repositoryItem = barEditItem11.Edit as DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit;
            if (repositoryItem != null)
            {
                var editorValue = barEditItem11.EditValue;
                if (editorValue != null)
                {
                    DataTable dataSource = repositoryItem.DataSource as DataTable;
                    if (dataSource != null)
                    {
                        DataRow[] selectedRows = dataSource.Select($"MaLenhSanXuat = '{editorValue.ToString()}'");
                        if (selectedRows.Length > 0)
                        {
                            DataRow selectedRow = selectedRows[0];

                            string madh = selectedRow["MaGop"].ToString();
                            string malenhsx = selectedRow["MaLenhSanXuat"].ToString();
                            loadCapPhat(madh, false, malenhsx);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy dữ liệu phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không có dữ liệu trong nguồn dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Không có giá trị được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("RepositoryItem không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ItemPaste_Click(object sender, EventArgs e)
        {
            GridView view = (!checkPopupMenu ? gCNPL2.MainView : gCPhuLieu.MainView) as GridView;
            // string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string clipboardData = Clipboard.GetText();
            byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
            string decodedClipboardData = Encoding.UTF8.GetString(bytes);
            string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (data.Length < 1) return;
            int startRow = view.FocusedRowHandle;
            foreach (string row in data)
            {
                object isNewValue = view.GetRowCellValue(startRow, "IsNew"); // Lấy giá trị của cột IsNew

                if (isNewValue != null && isNewValue.ToString() == "khongsua")
                {
                    startRow++; // Chuyển sang dòng tiếp theo mà không dán dữ liệu vào dòng này
                    continue;
                }
                if (!checkAddRow)
                {
                    checkAddRow = true;
                    break;
                }
                AddRow(row, startRow++, view);
                if (!view.IsValidRowHandle(startRow)) break;
            }
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
                                //MessageBox.Show($"Giá trị '{rowData[i]}' không hợp lệ cho cột '{targetColumn.FieldName}' yêu cầu kiểu số nguyên (int).",
                                //                "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.ActiveControl = button1;
                                //return;
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
                                //MessageBox.Show($"Giá trị '{rowData[i]}' không hợp lệ cho cột '{targetColumn.FieldName}' yêu cầu kiểu số thực (float).",
                                //                "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.ActiveControl = button1;
                                //return;
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
                    //MessageBox.Show($"Đã xảy ra lỗi khi dán dữ liệu vào cột '{view.VisibleColumns[columnIndex + i].FieldName}': {ex.Message}",
                    //                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong(true);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong(false);
        }

        private void gCNPL2_ProcessGridKey(object sender, KeyEventArgs e)
        {
            Keypass(e, gCNPL2);
        }
        private void gCPhuLieu_ProcessGridKey(object sender, KeyEventArgs e)
        {
            Keypass(e, gCPhuLieu);
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
                    object isNewValue = view.GetRowCellValue(startRow, "IsNew"); // Lấy giá trị của cột IsNew

                    if (isNewValue != null && isNewValue.ToString() == "khongsua")
                    {
                        startRow++; // Chuyển sang dòng tiếp theo mà không dán dữ liệu vào dòng này
                        continue;
                    }
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
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ThemDong(true);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            ThemDong(false);
        }

        private void gVPhuLieu_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            CellValue(sender, e, gVPhuLieu);
        }

        private void gVPhuLieu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {

        }

        private void gVPhuLieu_MouseDown(object sender, MouseEventArgs e)
        {
            checkGridView = false;
            GridHitInfo hitInfo = gVPhuLieu.CalcHitInfo(e.Location);

            if (hitInfo.InRowCell == true)
            {
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "IsNL")
                {
                    gVPhuLieu.FocusedColumn = hitInfo.Column;
                    gVPhuLieu.FocusedRowHandle = hitInfo.RowHandle;
                    gVPhuLieu.ShowEditor(); // Kích hoạt editor

                    // Kiểm tra nếu ActiveEditor có kiểu RepositoryItemImageComboBox thì hiển thị popup
                    /*  var activeEditor = gVNPL2.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
                      if (activeEditor?.Properties is RepositoryItemImageComboBox repositoryItemImageComboBox)
                      {
                          // Gọi phương thức ShowPopup() hoặc bất kỳ logic nào bạn muốn
                          activeEditor.Show();
                      }*/
                    if (gVPhuLieu.ActiveEditor is ImageComboBoxEdit editor)
                    {
                        // Hiển thị popup của editor
                        editor.ShowPopup();
                    }
                }
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            LuuDong(gCNPL2, gVNPL2);
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            LuuDong(gCPhuLieu, gVPhuLieu);
        }

        private void gCNPL2_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
           /* if (gVNPL2.FocusedColumn.FieldName == "MaVT")
            {
                if (IsVietnameseChar(e.KeyChar))
                {
                    e.Handled = true;
                }

                e.KeyChar = char.ToUpper(e.KeyChar);
            }*/
            if (gVNPL2.FocusedColumn.FieldName == "SoLuong")
            {
                string currentValue = gVNPL2.EditingValue?.ToString() ?? "";
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                {
                    e.Handled = true;
                }
                if (!char.IsControl(e.KeyChar) && currentValue.Length >= 10)
                {
                    e.Handled = true;
                }
            }
            if (gVNPL2.FocusedColumn.FieldName == "CapPhat")
                FocusFieldName(e, gVNPL2, 2);
            if (gVNPL2.FocusedColumn.FieldName == "CapThem")
                FocusFieldName(e, gVNPL2, 2);
            if (gVNPL2.FocusedColumn.FieldName == "ThucNhan")
                FocusFieldName(e, gVNPL2, 2);
            if (gVNPL2.FocusedColumn.FieldName == "DinhMuc")
                FocusFieldName(e, gVNPL2, 4);
            if (gVNPL2.FocusedColumn.FieldName == "DinhMucHaoHut")
                FocusFieldName(e, gVNPL2, 4);
        }
        private void FocusFieldName(KeyPressEventArgs e, GridView grv, int soluong)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
            else
            {
                string currentValue = grv.EditingValue?.ToString() ?? ""; // Lấy giá trị đang nhập
                string[] newValue = (currentValue + e.KeyChar).Split('.');
                if (newValue.Length > 1 && e.KeyChar != '\b')
                {
                    int CountSoLuong = newValue[1].Length;
                    if (CountSoLuong > soluong)
                        e.Handled = true;

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
            /*if (gVPhuLieu.FocusedColumn.FieldName == "MaVT")
            {
                if (IsVietnameseChar(e.KeyChar))
                {
                    e.Handled = true;
                }

                e.KeyChar = char.ToUpper(e.KeyChar);
            }*/
            if (gVPhuLieu.FocusedColumn.FieldName == "SoLuong")
            {
                string currentValue = gVPhuLieu.EditingValue?.ToString() ?? "";
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                {
                    e.Handled = true;
                }
                if (!char.IsControl(e.KeyChar) && currentValue.Length >= 10)
                {
                    e.Handled = true;
                }
            }
            if (gVPhuLieu.FocusedColumn.FieldName == "CapPhat")
                FocusFieldName(e, gVPhuLieu, 2);
            if (gVPhuLieu.FocusedColumn.FieldName == "CapThem")
                FocusFieldName(e, gVPhuLieu, 2);
            if (gVPhuLieu.FocusedColumn.FieldName == "ThucNhan")
                FocusFieldName(e, gVPhuLieu, 2);
            if (gVPhuLieu.FocusedColumn.FieldName == "DinhMuc")
                FocusFieldName(e, gVPhuLieu, 4);
            if (gVPhuLieu.FocusedColumn.FieldName == "DinhMucHaoHut")
                FocusFieldName(e, gVPhuLieu, 4);
        }

        private void Naplai1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void gVPhuLieu_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            DataRowView row = view.GetFocusedRow() as DataRowView;

            if (row != null)
            {
                if (row["IsNew"] != null && row["IsNew"].ToString() == "khongsua")
                {
                    e.Cancel = true;
                }
            }
            if (gVPhuLieu.FocusedColumn != null && gVPhuLieu.FocusedColumn.FieldName == "IsNL")
            {
                gCPhuLieu.BeginInvoke(new Action(() =>
                {
                    if (gVPhuLieu.ActiveEditor is ImageComboBoxEdit editor)
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

        private void gVNPL2_CellMerge(object sender, CellMergeEventArgs e)
        {
            //if (e.Column.FieldName == "TenVT")
            //{
            //    object val1 = gVNPL2.GetRowCellValue(e.RowHandle1, e.Column);
            //    object val2 = gVNPL2.GetRowCellValue(e.RowHandle2, e.Column);

            //    object isNew1 = gVNPL2.GetRowCellValue(e.RowHandle1, "IsNew");
            //    object isNew2 = gVNPL2.GetRowCellValue(e.RowHandle2, "IsNew");

            //    // Kiểm tra nếu một trong hai giá trị của IsNew là "copy" thì không merge
            //    if ((isNew1 != null && isNew1.ToString() == "copy") || (isNew2 != null && isNew2.ToString() == "copy"))
            //    {
            //        e.Merge = false;
            //    }
            //    else if (val1 != null && val2 != null &&
            //             !string.IsNullOrWhiteSpace(val1.ToString()) &&
            //             !string.IsNullOrWhiteSpace(val2.ToString()) &&
            //             val1.Equals(val2))
            //    {
            //        e.Merge = true;
            //    }
            //    else
            //    {
            //        e.Merge = false;
            //    }

            //    e.Handled = true;
            //}
            //else
            //{
            //    e.Merge = false;
            //    e.Handled = true;
            //}
        }

        private void gVPhuLieu_CellMerge(object sender, CellMergeEventArgs e)
        {
            //if (e.Column.FieldName == "TenVT")
            //{
            //    object val1 = gVPhuLieu.GetRowCellValue(e.RowHandle1, e.Column);
            //    object val2 = gVPhuLieu.GetRowCellValue(e.RowHandle2, e.Column);

            //    object isNew1 = gVPhuLieu.GetRowCellValue(e.RowHandle1, "IsNew");
            //    object isNew2 = gVPhuLieu.GetRowCellValue(e.RowHandle2, "IsNew");

            //    // Kiểm tra nếu một trong hai giá trị của IsNew là "copy" thì không merge
            //    if ((isNew1 != null && isNew1.ToString() == "copy") || (isNew2 != null && isNew2.ToString() == "copy"))
            //    {
            //        e.Merge = false;
            //    }
            //    else if (val1 != null && val2 != null &&
            //             !string.IsNullOrWhiteSpace(val1.ToString()) &&
            //             !string.IsNullOrWhiteSpace(val2.ToString()) &&
            //             val1.Equals(val2))
            //    {
            //        e.Merge = true;
            //    }
            //    else
            //    {
            //        e.Merge = false;
            //    }

            //    e.Handled = true;
            //}
            //else
            //{
            //    e.Merge = false;
            //    e.Handled = true;
            //}
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
        bool checkGridView = true;
        #endregion
        private void gVNPL2_Click(object sender, EventArgs e)
        {
           
        }

        private void gVPhuLieu_Click(object sender, EventArgs e)
        {
           
        }

        private void gVNPL2_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
        }

        private void gVPhuLieu_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
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

        private void repositoryItemCheckEdit1_Click(object sender, EventArgs e)
        {
            isCal = !isCal;

            if (isCal == true)
            {
                TinhCapPhap(gVNPL2);
                TinhCapPhap(gVPhuLieu);
            }
        }

        private void btnNhapCapThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
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
        private void ConfigureSearchLookUpEditCopyCP()
        {
            string url = string.Format("{0}?madh={1}&_madh={2}&_malenhsx={3}&_magop={4}", URL + "CanDoiDonHangTong/GetCapPhat", _mahang, _madh, _maLSX, _magop);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditCopyCP1.DataSource = tbl;
            searchLookUpEditCopyCP1.DisplayMember = "MaGop";
            searchLookUpEditCopyCP1.ValueMember = "MaLenhSanXuat";
            GridView gridView = searchLookUpEditCopyCP1.View;

            gridView.Columns.Clear();

            gridView.Columns.AddVisible("MaHang", "Mã hàng");
            gridView.Columns.AddVisible("MaDH", "Mã đơn hàng");
            gridView.Columns.AddVisible("Dot", "Đợt");
            gridView.Columns.AddVisible("MaLenhSanXuat", "Mã lệnh sản xuất");
            //gridView.Columns.AddVisible("MaGop", "Mã lệnh sản xuất");
            gridView.OptionsView.ShowGroupPanel = false;

            /* gridView.Columns["MaGop"].Width = 150;      
             gridView.Columns["Dot"].Width = 100;*/

        }
        private void searchLookUpEditCopyCP_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            var edit = sender as RepositoryItemSearchLookUpEdit;
            if (edit != null)
            {
                if (e.Value != null)
                {
                    // Lấy giá trị từ EditValue
                    DataRowView row = searchLookUpEditCopyCP.GetRowByKeyValue(e.Value) as DataRowView;
                    if (row != null)
                    {
                        // Kết hợp nhiều cột để hiển thị
                        e.DisplayText = $"{row["MaHang"]}|{row["Dot"]}|{row["MaDH"]}|{row["MaLenhSanXuat"]}";
                    }
                }
            }
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
            gVNPL2.Columns["IsNL"].ColumnEdit = comboBox;
            gVPhuLieu.Columns["IsNL"].ColumnEdit = comboBox;
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
        DataTable tblVTGoiYNPL = new DataTable();
        private void CreateSearchLookUpGoiY(string _cot, string _tenCot, string _tenCot2)
        {
            string url = string.Format(URL + "CanDoiDonHangTong/Get" + _cot + "GoiY");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (_tenCot == "MaVT")
            {
                //tblVTGoiY.Clear();
                //tblVTGoiYNPL.Clear();
                tblVTGoiY = tbl;
                tblVTGoiYNPL = tbl;
            }

            // Tạo RepositoryItemLookUpEdit
            RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
            repositoryItemLookUpEdit.DataSource = tbl;
            repositoryItemLookUpEdit.DisplayMember = _tenCot;
            repositoryItemLookUpEdit.ValueMember = _tenCot;
            repositoryItemLookUpEdit.SearchMode = SearchMode.AutoFilter;
            repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.Standard;
            repositoryItemLookUpEdit.ShowHeader = false; // Ẩn dòng tiêu đề của popup
            repositoryItemLookUpEdit.NullText = "";
            repositoryItemLookUpEdit.ImmediatePopup = true;
            //repositoryItemLookUpEdit.Aut = true;

            // Làm phẳng giao diện
            repositoryItemLookUpEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            repositoryItemLookUpEdit.PopupBorderStyle = DevExpress.XtraEditors.Controls.PopupBorderStyles.NoBorder;
            repositoryItemLookUpEdit.Columns.Clear(); // Xóa các cột mặc định
            if (tbl.Columns.Contains("MaVT") && tbl.Columns.Contains("TenVT"))
            {
                repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo(_tenCot)); // Chỉ hiển thị cột được chọn
            }
            else
            {
                // Nếu chỉ có một cột, hiển thị bình thường
                foreach (DataColumn col in tbl.Columns)
                {
                    repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo(col.ColumnName));
                }
            }
            repositoryItemLookUpEdit.KeyUp += (sender, e) =>
            {

                LookUpEdit lookUpEdit = sender as LookUpEdit;
                if (lookUpEdit != null && lookUpEdit.Properties.DataSource is DataTable dataTable)
                {
                    // Lấy văn bản hiện tại trong ô tìm kiếm


                    if (e.KeyCode == Keys.Enter)
                    {
                        //DataRow drRow = gVNPL2.GetFocusedDataRow();
                        //string tenMauFilter = drRow[_tenCot2].ToString();
                        this.ActiveControl = button1;
                    }
                    else
                    {
                        string filterText = lookUpEdit.Text.Trim().ToLower();

                        // Lọc dữ liệu
                        var filteredRows = dataTable.AsEnumerable()
                                            .Where(row => row[_tenCot].ToString().ToLower().Contains(filterText))
                                            .ToList();
                        //if (filteredRows.Count == 0)
                        //{
                        //    // Nếu không có dữ liệu, đóng popup
                        //    lookUpEdit.ClosePopup();
                        //}
                        //else
                        //{
                        //    // Nếu có dữ liệu phù hợp, mở popup
                        //    lookUpEdit.ShowPopup();
                        //}
                    }

                }

            };
            /*repositoryItemLookUpEdit.EditValueChanged += (sender, e) =>
            {
                LookUpEdit lookUpEdit = sender as LookUpEdit;
                string filterText = lookUpEdit.EditValue?.ToString().Trim() ??"";
                if (gVNPL2.FocusedColumn.FieldName == "MaVT") // Nếu chọn _tenCot (ví dụ: MaVT)
                {
                    DataRow dataRow = gVNPL2.GetFocusedDataRow();
                    if (dataRow == null) return;
                    string maVT = filterText;
                    if (tblVTGoiY == null || tblVTGoiY.Rows.Count == 0) return;
                    string TenVT = tblVTGoiY.AsEnumerable().FirstOrDefault(x => x["MaVT"].ToString() == maVT)?["TenVT"].ToString();
                    if (TenVT == null) return;
                    if (checkGridView)
                        dataRow["TenVT"] = TenVT;
                }

                if (gVNPL2.FocusedColumn.FieldName == "TenVT") // Nếu chọn _tenCot (ví dụ: MaVT)
                {
                    DataRow dataRow = gVNPL2.GetFocusedDataRow();
                    if (dataRow == null) return;
                    string TenVT = filterText;
                    if (tblVTGoiY == null || tblVTGoiY.Rows.Count == 0) return;
                    string MaVT = tblVTGoiY.AsEnumerable().FirstOrDefault(x => x["TenVT"].ToString() == TenVT)?["MaVT"].ToString();
                    if (MaVT == null) return;
                    if (checkGridView)
                        dataRow["MaVT"] = MaVT;
                }
                if (gVPhuLieu.FocusedColumn.FieldName == "MaVT") // Nếu chọn _tenCot (ví dụ: MaVT)
                {
                    DataRow dataRow = gVPhuLieu.GetFocusedDataRow();
                    if (dataRow == null) return;
                    string maVT = filterText;
                    if (tblVTGoiY == null || tblVTGoiY.Rows.Count == 0) return;
                    string TenVT = tblVTGoiYNPL.AsEnumerable().FirstOrDefault(x => x["MaVT"].ToString() == maVT)?["TenVT"].ToString();
                    if (TenVT == null) return;
                    if (!checkGridView)
                        dataRow["TenVT"] = TenVT;
                }

                if (gVPhuLieu.FocusedColumn.FieldName == "TenVT") // Nếu chọn _tenCot (ví dụ: MaVT)
                {
                    DataRow dataRow = gVPhuLieu.GetFocusedDataRow();
                    if (dataRow == null) return;
                    string TenVT = filterText;
                    if (tblVTGoiY == null || tblVTGoiY.Rows.Count == 0) return;
                    string MaVT = tblVTGoiYNPL.AsEnumerable().FirstOrDefault(x => x["TenVT"].ToString() == TenVT)?["MaVT"].ToString();
                    if (MaVT == null) return;
                    if (!checkGridView)
                        dataRow["MaVT"] = MaVT;
                }
            };
*/


            gCNPL2.RepositoryItems.Add(repositoryItemLookUpEdit);
            gVNPL2.Columns[_tenCot2].ColumnEdit = repositoryItemLookUpEdit;
            gCPhuLieu.RepositoryItems.Add(repositoryItemLookUpEdit);
            gVPhuLieu.Columns[_tenCot2].ColumnEdit = repositoryItemLookUpEdit;

        }
        private void loadSLMau( GridView grv)
        {
            DataRow dtRow = grv.GetFocusedDataRow();
            if (dtRow == null || dtRow["MaMau"].ToString()==""|| dtRow["MaMau"].ToString()==string.Empty) return;
            string url = string.Format("{0}?para={1}&&para1={2}&&para2={3}", URL + "CanDoiDonHangTong/GetSoLuongByMauDH", _maLSX, dtRow["MaMau"],_magop);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                grv.SetFocusedRowCellValue("SoLuong", 0);
                return;
            }
          
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grv.SetFocusedRowCellValue("SoLuong", 0);
                return;
            } 
                
               
            int rs = 0;
            foreach(DataRow a in tbl.Rows)
            {
                if(a!=null)
                {
                    rs += int.TryParse(a["SoLuong"]?.ToString(), out int soluong) ? soluong : 0;
                }    
            }
            grv.SetFocusedRowCellValue("SoLuong", rs);
        }

        private void loadCapPhat(string madh, bool isGoc, string malenhsx = "")
        {

            if (isGoc)
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
                    if (isGoc)
                    {
                        _dr["ID"] = row["ID"];
                    }
                    else
                    {
                        _dr["ID"] = 0;
                    }
                    _dr["MaDH"] = _magop;
                    _dr["MaLenhSanXuat"] = _maLSX;
                    _dr["MaNPL"] = row["MaNPL"];
                    _dr["NguoiTao"] = _nguoitao;
                    _dr["MaKH"] = _tenKH;
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
                    _dr["IsNL"] = row["IsNL"] != null && bool.TryParse(row["IsNL"].ToString(), out bool isNL) ? isNL : true;
                    _dr["ThucNhan"] = row["ThucNhan"];
                    _dr["TenTAVT"] = row["TenTAVT"];
                    _dr["NhomNPL"] = row["NhomNPL"];
                    _dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
                    _dr["IsNew"] = "khongsua";
                    tblSave.Rows.Add(_dr);
                }
                DataTable tblNL = tblSave.Clone();    // Bảng chứa IsNL = true
                DataTable tblPL = tblSave.Clone(); ;  // Bảng chứa IsNL = false
                var tblNL2 = tblSave.AsEnumerable().Where(x => Convert.ToBoolean(x["IsNL"])).ToList();
                var tblPL2 = tblSave.AsEnumerable().Where(x => !Convert.ToBoolean(x["IsNL"])).ToList();
                if (tblNL2.Any())
                    tblNL = tblNL2.CopyToDataTable();
                if (tblPL2.Any())
                    tblPL = tblPL2.CopyToDataTable();

                gCNPL2.DataSource = tblNL;
                gCPhuLieu.DataSource = tblPL;
            }
            else
            {
                string url = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPL", madh, malenhsx);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                foreach (DataRow row in tbl.Rows)
                {
                    DataRow _dr = tblSave.NewRow();
                    if (isGoc)
                    {
                        _dr["ID"] = row["ID"];
                    }
                    else
                    {
                        _dr["ID"] = 0;
                    }
                    _dr["MaDH"] = _magop;
                    _dr["MaLenhSanXuat"] = _maLSX;
                    _dr["NguoiTao"] = _nguoitao;
                    _dr["MaKH"] = _tenKH;
                    _dr["MaNPL"] = row["MaNPL"];
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
                    _dr["IsNL"] = row["IsNL"] != null && bool.TryParse(row["IsNL"].ToString(), out bool isNL) ? isNL : true;
                    _dr["ThucNhan"] = row["ThucNhan"];
                    _dr["TenTAVT"] = row["TenTAVT"];
                    _dr["NhomNPL"] = row["NhomNPL"];
                    _dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
                    _dr["IsNew"] = "copycp";
                    bool isDuplicate = false;
                    foreach (DataRow _row in tblSave.Rows)
                    {
                        if (_row["MaVT"].ToString() == row["MaVT"].ToString() &&
                            _row["MaMau"].ToString() == row["MaMau"].ToString() &&
                            _row["KhoVai"].ToString() == row["KhoVai"].ToString())
                        {
                            isDuplicate = true;
                            break;
                        }


                    }
                    if (isDuplicate)
                    {
                        break;
                    }
                    tblSave.Rows.Add(_dr);

                }
                DataTable tblNLGoc = gCNPL2.DataSource as DataTable;
                DataTable tblPLGoc = gCPhuLieu.DataSource as DataTable;
                DataTable tblNL = tblNLGoc == null ? tblSave.Clone() : tblNLGoc;// Bảng chứa IsNL = true
                DataTable tblPL = tblPLGoc == null ? tblSave.Clone() : tblPLGoc; // Bảng chứa IsNL = false

                var tblNL2 = tblSave.AsEnumerable().Where(x => Convert.ToBoolean(x["IsNL"])).ToList();
                var tblPL2 = tblSave.AsEnumerable().Where(x => !Convert.ToBoolean(x["IsNL"])).ToList();
                if (tblNL2.Any())
                    tblNL = tblNL2.CopyToDataTable();
                if (tblPL2.Any())
                    tblPL = tblPL2.CopyToDataTable();

                gCNPL2.DataSource = tblNL;
                gCPhuLieu.DataSource = tblPL;
            }
        }
    }
}
