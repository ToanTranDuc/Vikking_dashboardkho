using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
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
    public partial class frmNhapCapPhatDHT : DevExpress.XtraEditors.XtraForm
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
        string _magop = string.Empty, _nguoitao = string.Empty, _maLSX = string.Empty,_tenKH=string.Empty,_mahang=string.Empty,_madh=string.Empty,_malenh=string.Empty,_soluong=string.Empty,_tenhang=string.Empty,_dot=string.Empty;
        int _iskeove = 0;
        bool checkAddRow = true;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
       
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        bool isCal =false;
        bool isVatTu = true;
        DataTable tblSave;
        DataTable tblGoc;
        public frmNhapCapPhatDHT()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
         
        }

        public frmNhapCapPhatDHT(string _magop,string _nguoitao,string _maLSX,string _tenKH,string _mahang,string _madh,string _malenh, string _soluong,string _tenhang,int _iskeove,string _dot)
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
            ConfigureSearchLookUpEditCopyCP();
            this.ActiveControl = button1;
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlDelete = new ActionControl(SuaDong, _allowDelete, ActionType.Edit, this.Sua.Enabled);
           
            lstActionControls = new List<ActionControl> {
                actionControlAdd,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
        }
     
        protected override void OnLoad(EventArgs e)
        {          
            CreateTableSave();
           
            ConfigureComboBoxLoaiVT();
            loadCapPhat(_magop, true);
            txtDonHang.Text = _madh;
            txtMaHang.Text = _tenhang;
            txtLenhSX.Text = _malenh;
            //txtSoLuong.Text = _soluong;
            txtDot.Text = _dot;
            loadSoLuong();
            loadSoBooking();
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
         
        }
        private void ThemDong()
        {
            using (frmNhapCapPhatDHTThem frm = new frmNhapCapPhatDHTThem(_magop == null ? "" : _magop.ToString(), _nguoitao, _maLSX == null ? "" : _maLSX.ToString(), _tenKH == null ? "" : _tenKH.ToString(), _mahang == null ? "" : _mahang.ToString(), _madh == null ? "" : _madh.ToString(), _malenh == null ? "" : _malenh.ToString(), _soluong == null ? "" : _soluong.ToString(), _tenhang == null ? "" : _tenhang.ToString(), _iskeove == null ? 0 : _iskeove, _dot == null ? "" : _dot.ToString()))
            {
                frm.WindowState = FormWindowState.Maximized;
                frm.FormClosing += (s, args) =>
                {
                    loadCapPhat(_magop, true);

                };
                frm.ShowDialog();
            }

           
 

        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
      
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }
        private async void SuaDong()
        {
            try
            {
                //
                /* if (_iskeove == 1)
                 {
                     XtraMessageBox.Show("Lệnh này đã được thực hiện sản xuất.\nKhông thể xóa được!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                     return;
                 }*/
               
                    using (frmNhapCapPhatDHTSua frm = new frmNhapCapPhatDHTSua(_magop == null ? "" : _magop.ToString(), _nguoitao, _maLSX == null ? "" : _maLSX.ToString(), _tenKH == null ? "" : _tenKH.ToString(), _mahang == null ? "" : _mahang.ToString(), _madh == null ? "" : _madh.ToString(), _malenh == null ? "" : _malenh.ToString(), _soluong == null ? "" : _soluong.ToString(), _tenhang == null ? "" : _tenhang.ToString(), _iskeove == null ? 0 : _iskeove, _dot == null ? "" : _dot.ToString()))
                    {
                        frm.WindowState = FormWindowState.Maximized;
                        frm.FormClosing += (s, args) =>
                        {
                            loadCapPhat(_magop, true);
                            
                        };
                        frm.ShowDialog();
                    }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gVNPL2_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if(isCal)
            {
                if (e.Column.FieldName == "SoLuong")
                {
                    var DinhMucValue = gVNPL2.GetRowCellValue(e.RowHandle, "DinhMuc");
                    if (DinhMucValue != null && !string.IsNullOrEmpty(DinhMucValue.ToString()))
                    {
                        var soLuongValue = Convert.ToDecimal(e.Value);
                        decimal capPhat = soLuongValue * Convert.ToDecimal(DinhMucValue);
                        gVNPL2.SetRowCellValue(e.RowHandle, "CapPhat", capPhat);
                    }

                }
                if (e.Column.FieldName == "DinhMuc")
                {
                    var soLuongValue = gVNPL2.GetRowCellValue(e.RowHandle, "SoLuong");
                    if (soLuongValue != null && !string.IsNullOrEmpty(soLuongValue.ToString()))
                    {
                        var dinhmucValue = Convert.ToDecimal(e.Value);
                        decimal capPhat = dinhmucValue * Convert.ToDecimal(soLuongValue);
                        gVNPL2.SetRowCellValue(e.RowHandle, "CapPhat", capPhat);
                    }

                }
            }
            if (e.Column.FieldName == "IsNL") // Chỉ xử lý cho cột bạn cần
            {
                gVNPL2.RefreshRow(e.RowHandle); // Làm mới hàng hiện tại
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

        private void txtDinhMucItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEditDinhMuc(sender, e);
        }

        private void txtCapPhatItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void gVNPL2_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gVNPL2.FocusedColumn.FieldName == "MaVT")
            {

                if (e.Value != null)
                {
                    string originalText = e.Value.ToString();
                    string processedText = RemoveVietnameseTone(originalText);
                    e.Value = processedText; // Gán chuỗi đã xử lý lại vào ô
                }
            }
        }
        private void txtCapThemItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void searchLookUpEditCopyCP_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit editor = sender as SearchLookUpEdit;

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
            }
        }
        private void txtSLItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        
        #region  xử lý copy paste
        private void gVNPL2_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }
        bool checkFlag = true;

        private void gVNPL2_MouseDown(object sender, MouseEventArgs e)
        {
            GridHitInfo hitInfo = gVNPL2.CalcHitInfo(e.Location);

            if (hitInfo.InRowCell == true)
            {
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "IsNL" )
                {
                    gVNPL2.FocusedColumn = hitInfo.Column;
                    gVNPL2.FocusedRowHandle = hitInfo.RowHandle;
                    gVNPL2.ShowEditor(); // Kích hoạt editor

                    if (gVNPL2.ActiveEditor is ImageComboBoxEdit editor)
                    {
                        // Hiển thị popup của editor
                        editor.ShowPopup();
                    }
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
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

                    string url2 = string.Format("{0}?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/DeleteAllCapThem", _magop == null ? "" : _magop.ToString(),
                       _maLSX == null ? "" : _maLSX.ToString());
                    string result2 = Task.Run(async () => { return await _clientExtension.DeletedAsync(url2); }).Result;

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

        private void gVNPL2_CellMerge(object sender, CellMergeEventArgs e)
        {
            if (e.Column.FieldName == "TenVT")
            {
                object val1 = gVNPL2.GetRowCellValue(e.RowHandle1, e.Column);
                object val2 = gVNPL2.GetRowCellValue(e.RowHandle2, e.Column);

                if (val1 != null && val2 != null && val1.Equals(val2))
                {
                    e.Merge = true; // Merge nếu giá trị giống nhau
                }
                else
                {
                    e.Merge = false;
                }
                e.Handled = true; // Đánh dấu đã xử lý merge
            }
            else
            {
                e.Merge = false;   // Không merge các cột khác
                e.Handled = true;  // Đánh dấu đã xử lý để DevExpress không tự merge
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadCapPhat(_magop, true);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (frmNhapCapThemDHT frm = new frmNhapCapThemDHT(_magop == null ? "" : _magop.ToString(), _nguoitao, _maLSX == null ? "" : _maLSX.ToString(), _tenKH == null ? "" : _tenKH.ToString(), _mahang == null ? "" : _mahang.ToString(), _madh == null ? "" : _madh.ToString(), _malenh == null ? "" : _malenh.ToString(), _soluong == null ? "" : _soluong.ToString(), _tenhang == null ? "" : _tenhang.ToString(), _iskeove == null ? 0 : _iskeove, _dot == null ? "" : _dot.ToString()))
            {
                frm.WindowState = FormWindowState.Maximized;
                frm.FormClosing += (s, args) =>
                {
                    loadCapPhat(_magop, true);

                };
                frm.ShowDialog();
            }
        }

        private void gVNPL2_ShowingEditor(object sender, CancelEventArgs e)
        {
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
                    if (row["IsNew"]=="copy")
                    {
                        //e.Appearance.BackColor = Color.LightYellow; // Đổi màu nền thành vàng nhạt
                    }
                    else if(row["IsNew"]=="them")
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
        private void ConfigureSearchLookUpEditCopyCP()
        {
            string url = string.Format("{0}?madh={1}&_madh={2}&_malenhsx={3}&_magop={4}", URL + "CanDoiDonHangTong/GetCapPhat",_mahang,_madh,_maLSX,_magop);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditCopyCP.DataSource = tbl;
            searchLookUpEditCopyCP.DisplayMember = "MaGop"; 
            searchLookUpEditCopyCP.ValueMember = "MaLenhSanXuat";
            GridView gridView = searchLookUpEditCopyCP.View;

            gridView.Columns.Clear();

            gridView.Columns.AddVisible("MaHang", "Mã hàng");
            gridView.Columns.AddVisible("MaDH", "Mã đơn hàng");
            gridView.Columns.AddVisible("Dot", "Đợt");
            gridView.Columns.AddVisible("MaLenhSanXuat", "Mã lệnh sản xuất");
            //gridView.Columns.AddVisible("MaGop", "Mã lệnh sản xuất");
            gridView.OptionsView.ShowGroupPanel = false;
        }
        private void searchLookUpEditCopyCP_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            var edit = sender as RepositoryItemSearchLookUpEdit;
            if(edit !=null)
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
            comboBox.TextEditStyle= TextEditStyles.Standard;
            gVNPL2.Columns["IsNL"].ColumnEdit = comboBox;

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
            if(tbl.Rows.Count>0)
            {
                rs = string.Join("|",
                tbl.AsEnumerable().Select(r => r["BookingMaHang"].ToString()));
            }    
            txtSoBooking.Text = rs.ToString();
        }
        private void loadCapPhat(string madh,bool isGoc,string malenhsx="")
        {
            if(isGoc)
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

                    tblSave.Rows.Add(_dr);
                }
                gCNPL2.DataSource = tblSave.Copy();
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
                    if(isDuplicate)
                    {
                        break;
                    }    
                        tblSave.Rows.Add(_dr);
                   
                }
                gCNPL2.DataSource = tblSave.Copy();
            }
          
        }
    }
}