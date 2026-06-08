using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmPhongBan : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();
        DataTable tbl = new DataTable();

        public frmPhongBan()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadData();
            CreateTableThuVien();
        }


        private void LoadData()
        {
            string url = string.Format("{0}?", URL + "PhongBan/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
             tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count > 0 && tbl != null)
                gridControl1.DataSource = tbl;
            else
                gridControl1.DataSource = null;
        }



        private void CreateTableThuVien()
        {
            tbl = new DataTable("tbl");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPB", typeof(string));
            tbl.Columns.Add("TenPB", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("MaHoa", typeof(string));
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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


        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            foreach (DataRow dr in tbl.Rows)
            {
                if (string.IsNullOrWhiteSpace(dr["TenPB"].ToString()))
                {

                    XtraMessageBox.Show("Vui lòng điền thông tin Phòng Ban", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(dr["MaHoa"].ToString()))
                {

                    XtraMessageBox.Show("Vui lòng nhập mã hóa", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (tbl.Rows.Count > 0 && tbl != null)
            {
                string url = string.Format("{0}?", URL + "PhongBan/Post");
                string mss = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu Quy đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
                LoadData();
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridControl1.DataSource != null)
                {
                    tbl = gridControl1.DataSource as DataTable;
                }
                this.ActiveControl = simpleButton1;
                DataRow dr = tbl.NewRow();
                dr["ID"] = 0;
                dr["MaPB"] = "";

                tbl.Rows.InsertAt(dr, 0);
                gridControl1.DataSource = tbl;
            }

            catch (Exception ex)
            {


            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                // e.ListSourceRowIndex là vị trí thật trong data source
                if (e.ListSourceRowIndex >= 0)
                    e.DisplayText = (e.ListSourceRowIndex + 1).ToString();
            }
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    DataRow dr = gridView1.GetFocusedDataRow();
                    if (dr == null) return;
                    int id = Convert.ToInt32(dr["ID"].ToString());


                    string url = string.Format("{0}?Parameter={1}", URL + "PhongBan/Delete", id);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}