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
    public partial class frmLoaiNhaCungCap : DevExpress.XtraEditors.XtraForm
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
        DataTable tblPhieu = new DataTable();
        DataTable tblVT = new DataTable();
        public frmLoaiNhaCungCap()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateTableThuVien();
            LoadData();
        }
        private void LoadData()
        {
            string url = $"{URL}NhaCC/Get?action=GETLOAI&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                gridControl1.DataSource = tbl;
            }
            else
            {
                CreateTableThuVien();
                gridControl1.DataSource = null;
            }

        }

        private void CreateTableThuVien()
        {
            tbl = new DataTable("tbl");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaLoaiNCC", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
        }


        private void gridView3_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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
                dr["MaLoaiNCC"] = "";

                tbl.Rows.InsertAt(dr, 0);
                gridControl1.DataSource = tbl;
            }

            catch (Exception ex)
            {


            }
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            if (tbl.Rows.Count > 0 && tbl != null)
            {
                string url = string.Format("{0}?", URL + "NhaCC/Post");
                string mss = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu Quy đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
                LoadData();
            }
        }

        private void gridView3_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                if (e.ListSourceRowIndex >= 0)
                {
                    DataView dv = (gridControl1.DataSource as DataTable).DefaultView;
                    DataRowView row = dv[e.ListSourceRowIndex];

                    if (row["ID"].ToString() == "0")
                    {
                        e.DisplayText = "0";
                    }
                    else
                    {
                        int stt = 1;
                        for (int i = 0; i < dv.Count; i++)
                        {
                            if (dv[i]["ID"].ToString() != "0")
                            {
                                if (i == e.ListSourceRowIndex)
                                {
                                    e.DisplayText = stt.ToString();
                                    return;
                                }
                                stt++;
                            }
                        }
                    }
                }
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

                    DataRow dr = gridView3.GetFocusedDataRow();
                    if (dr == null) return;
                    int id = Convert.ToInt32(dr["ID"].ToString());
                    string macl = dr["MaLoaiNCC"].ToString();
                    string urlcheck = $"{URL}NhaCC/Get?action=GETPhieuBaoGiacheckcl&para={macl}&para1=&para2=&para3=&para4=&para5=";
                    string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck); }).Result;
                    DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);

                    if (tblcheck != null && tblcheck.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Chủng loại này đã có trong phiếu báo giá. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string urlcheckkh = $"{URL}NhaCC/Get?action=GETChiTietCLCHECK&para={macl}&para1=&para2=&para3=&para4=&para5=";
                    string jsonCheckkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckkh); }).Result;
                    DataTable tblcheckkh = JsonConvert.DeserializeObject<DataTable>(jsonCheckkh);

                    if (tblcheckkh != null && tblcheckkh.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Chủng loại này đã được gán cho nhà cung cấp. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string url = string.Format("{0}?Parameter={1}", URL + "NhaCC/Delete", id);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}