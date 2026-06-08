using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmLichSuPhieuBG : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        Helper helper = new Helper();
        DataRow _rowFocused = null;
        private string _maVatTu = string.Empty, _maNCc = string.Empty;
        private DateTime? _tuNgay = null;
        private DateTime? _denNgay = null;

        public frmLichSuPhieuBG()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateSearchlookupVatTu();
            CreateSearchlookupNCC();

            LoadData();
        }

        private void CreateSearchlookupVatTu()
        {
            string url = $"{URL}LichSuPBG/Get?Action=GetVatTu&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit1.DataSource = tbl;
            repositoryItemSearchLookUpEdit1.ValueMember = "MaVTID";
            repositoryItemSearchLookUpEdit1.DisplayMember = "ChiTiet";
            

        }
        private void CreateSearchlookupNCC()
        {
            string url = $"{URL}LichSuPBG/Get?Action=GetNCC&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit2.DataSource = tbl;
            repositoryItemSearchLookUpEdit2.ValueMember = "MaNhaCC";
            repositoryItemSearchLookUpEdit2.DisplayMember = "TenKH";

        }

        private void repositoryItemSearchLookUpEdit1View_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "NPL"
                               && info.EditValue != null);


            int groupIndex = repositoryItemSearchLookUpEdit1View.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
            }

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                e.Appearance.ForeColor = Color.Red;
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn19)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }
            if (info.Column == gridColumn18)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            e.Handled = false;
        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            if (edit != null && edit.EditValue != null)
            {
                _maVatTu = edit.EditValue.ToString();
            }
            else
            {
                _maVatTu = string.Empty;
            }
            //LoadData();
        }

        private void repositoryItemDateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            DateEdit dateEdit = sender as DateEdit;
            if (dateEdit != null && dateEdit.EditValue != null)
            {
                _tuNgay = Convert.ToDateTime(dateEdit.EditValue);
            }
            else
            {
                _tuNgay = null;
            }
            //LoadData();
        }

        private void repositoryItemDateEdit2_EditValueChanged(object sender, EventArgs e)
        {
            DateEdit dateEdit = sender as DateEdit;
            if (dateEdit != null && dateEdit.EditValue != null)
            {
                _denNgay = Convert.ToDateTime(dateEdit.EditValue);
            }
            else
            {
                _denNgay = null;
            }
            //LoadData();
        }

        private void LoadData()
        {
            string tuNgayStr = _tuNgay.HasValue ? _tuNgay.Value.ToString("yyyy-MM-dd") : string.Empty;
            string denNgayStr = _denNgay.HasValue ? _denNgay.Value.ToString("yyyy-MM-dd") : string.Empty;

            string url = $"{URL}LichSuPBG/Get?Action=GetChiTietVT&para1={tuNgayStr}&para2={denNgayStr}&para3={_maVatTu}&para4={_maNCc}&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl2.DataSource = tbl;

            GridView view = gridControl2.MainView as GridView;
            if (view == null || view.RowCount == 0)
                return;

            int firstDataRowHandle = view.GetVisibleRowHandle(0);

            if (view.IsGroupRow(firstDataRowHandle))
            {
                firstDataRowHandle = view.GetNextVisibleRow(firstDataRowHandle);
            }
            view.FocusedRowHandle = firstDataRowHandle;
            view.MakeRowVisible(firstDataRowHandle);
        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "HieuLuc"
                               && info.EditValue != null);

            int groupIndex = gridView1.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
            }

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Còn Hiệu Lực" : "Hết Hiệu Lực";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn3)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }


            e.Handled = false;
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = gridView1.GetFocusedDataRow() as DataRow;
                if (rowFocused != null)
                {
                    bool IsDuyet = true;
                    string MaPhieuMH = rowFocused["MaPhieuBG"]?.ToString();
                    string TenPhieuMH = rowFocused["TenPhieu"]?.ToString();
                    frmERPBaoGiaNCC frm = new frmERPBaoGiaNCC("edit", IsDuyet, MaPhieuMH, TenPhieuMH);
                    frm.ShowDialog();
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (gridView2.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = gridView2.GetFocusedDataRow() as DataRow;
                if (rowFocused != null)
                {
                    bool IsDuyet = true;
                    string mavt = rowFocused["MaVTID"]?.ToString();
                    string mancc = rowFocused["MaNCC"]?.ToString();
                    LoadDataPhieu(mavt,mancc);
                }
            }
            else
            {
                return;
            }
        }

        private void repositoryItemSearchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            if (edit != null && edit.EditValue != null)
            {
                _maNCc = edit.EditValue.ToString();
            }
            else
            {
                _maNCc = string.Empty;
            }

            //LoadData();
        }

        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "NPL"
                               && info.EditValue != null);

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                e.Appearance.ForeColor = Color.Red;
                e.Handled = false;
                return;
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            //if (info.Column == gridColumn24)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            if (info.Column == gridColumn27)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn10)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn28)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            int groupIndex = gridView2.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
            }
            // Gán lại thuộc tính
            e.Appearance.ForeColor = e.Appearance.ForeColor;
            e.Appearance.Font = e.Appearance.Font;
            e.DefaultDraw();

            e.Handled = true;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            gridControl1.DataSource = null;
        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue == null || string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                _maVatTu = string.Empty;
                //LoadData(); 
            }
        }

        private void repositoryItemSearchLookUpEdit1_QueryPopUp(object sender, CancelEventArgs e)
        {
            SearchLookUpEdit slup = sender as SearchLookUpEdit;
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int desiredWidth = Math.Max(500, (int)(screenWidth * 0.6));
            slup.Properties.PopupFormWidth = desiredWidth;
        }

        private void gridView2_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            if (e.Column.FieldName == "HieuLuc")
            {
                string trangThai = view.GetRowCellValue(e.RowHandle, "HieuLuc")?.ToString();

                if (string.IsNullOrEmpty(trangThai)) return;
                switch (trangThai)
                {
                    case "Gần hết hạn – cần update":
                        e.Appearance.BackColor = Color.Orange;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "Còn hiệu lực":
                        e.Appearance.BackColor = Color.LightGreen;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    //case "Thành công":
                    //    e.Appearance.BackColor = Color.LightSkyBlue;
                    //    e.Appearance.ForeColor = Color.Black;
                    //    break;

                    //case "Đã hủy":
                    //    e.Appearance.BackColor = Color.Red;
                    //    e.Appearance.ForeColor = Color.Black;
                    //    break;

                    default:
                        break;
                }
            }

            if (e.Column.FieldName == "DonGia")
            {
                e.Appearance.ForeColor = Color.Red;
            }
        }

        private void LoadDataPhieu(string mavt, string mancc)
        {
            string tuNgayStr = _tuNgay.HasValue ? _tuNgay.Value.ToString("yyyy-MM-dd") : string.Empty;
            string denNgayStr = _denNgay.HasValue ? _denNgay.Value.ToString("yyyy-MM-dd") : string.Empty;

            string url = $"{URL}LichSuPBG/Get?Action=GetPBG&para1={tuNgayStr}&para2={denNgayStr}&para3={mavt}&para4={mancc}&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl1.DataSource = tbl;

        }

        private void grv_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
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
    }
}