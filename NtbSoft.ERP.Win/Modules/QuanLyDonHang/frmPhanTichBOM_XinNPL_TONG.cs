using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmPhanTichBOM_XinNPL_TONG : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _tenTau="";
        DataTable tblAllSize = new DataTable();
        public frmPhanTichBOM_XinNPL_TONG()
        {
            InitializeComponent();
         
         
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }
        protected override void OnLoad(EventArgs e)
        {
            loadAllSize();
            CreateSearchLookupKH();
            //CreateSearchLookupTrangThai();
            loadTQ();
            base.OnLoad(e);
        }
        private void loadTQ()
        {
            try
            {
              
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");

                string kh = searchLookUpEditKH.EditValue == null ? "ALL" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "ALL" : searchLookUpEditMH.EditValue.ToString();
                string tt = searchLookUpEditTT.EditValue == null ? "ALL" : searchLookUpEditTT.EditValue.ToString();
                string urlTau = $"{URL}XinNPLMayMau/Get?Action=GETTONGQUAN_V1&para1={kh}&para2={mh}&para3={tt}";
                
                string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
                if (jsonTau == "[]")
                {
                    gC.DataSource = null;
                    SplashScreenManager.CloseForm(false);
                }
                else
                {
                    DataTable tblTau = JsonConvert.DeserializeObject<DataTable>(jsonTau);
                    if (!tblTau.Columns.Contains("btnChiTiet"))
                        tblTau.Columns.Add("btnChiTiet", typeof(string));
                    gC.DataSource = tblTau;
                    SplashScreenManager.CloseForm(false);
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
            }
        }

        private void CreateSearchLookupKH() 
        {
            try
            {
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";
              
                string url = $"{URL}XinNPLMayMau/Get?Action=GETKH_TQ";
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                if (tblKH == null||tblKH.Rows.Count==0)
                {
                    searchLookUpEditKH.Properties.DataSource = null;
                }    
                searchLookUpEditKH.Properties.DataSource = tblKH;
             
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSearchLookupMH()
        {
            try
            {
                //string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                //string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
              
                searchLookUpEditMH.Properties.ValueMember = "MaHang";
                searchLookUpEditMH.Properties.DisplayMember = "TenHang";
                string url = $"{URL}XinNPLMayMau/Get?Action=GETMH_TQ&para1={kh}";
                string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (jsonMH == "[]") return;
                DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);
                if (tblMH == null || tblMH.Rows.Count == 0)
                {
                    searchLookUpEditMH.Properties.DataSource = null;
                }
                searchLookUpEditMH.Properties.DataSource = tblMH;
                if(tblMH.Rows.Count==1)
                {
                    searchLookUpEditMH.EditValue = tblMH.Rows[0]["MaHang"];
                }    

            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            CreateSearchLookupMH();
            loadTQ();
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            loadTQ();
        }
        private void CreateSearchLookupTrangThai()
        {
            try
            {
                searchLookUpEditTT.Properties.ValueMember = "MaTrangThai";
                searchLookUpEditTT.Properties.DisplayMember = "TrangThai";
                string url = $"{URL}XinNPLMayMau/Get?Action=GETTRANGTHAI";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditTT.Properties.DataSource = null;
                }
                searchLookUpEditTT.Properties.DataSource = tbl;

            }
            catch (Exception ex)
            {

            }
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            loadTQ();
        }

        private void repobtnNhapKho_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow dr = gV.GetFocusedDataRow();
            if (dr == null) return;

            frmERPNhapKhoNPL frm = new frmERPNhapKhoNPL(dr);
            frm.Show();
            frm.FormClosed += (s, args) =>
            {
                loadTQ();
            };
        }

        private void searchLookUpEditTT_EditValueChanged(object sender, EventArgs e)
        {
            loadTQ();
        }

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai();
        }
        private void NapLai()
        {
            CreateSearchLookupKH();
            CreateSearchLookupTrangThai();
            loadTQ();
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPhanTichBom_XinNPL frm = new frmPhanTichBom_XinNPL();
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
            frm.FormClosed += (s, agrs) =>
            {
                loadTQ();
            };
        }

        private void btnSuaLo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         
            //frmERPNhapKhoNPLSua frm = new frmERPNhapKhoNPLSua();
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.WindowState = FormWindowState.Maximized;
            //frm.Show();
            //frm.FormClosed += (s, args) =>
            //{
            //    loadTQ();
            //};
        }

        private void btnSuaVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            //frmERPNhapKhoNPLSuaCay frm = new frmERPNhapKhoNPLSuaCay();
            //frm.StartPosition = FormStartPosition.CenterScreen;
           
            //frm.Show();
            //frm.FormClosed += (s, args) =>
            //{
            //    loadTQ();
            //};
        }

        private void repoChiTiet_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow dr = gV.GetFocusedDataRow();
            if (dr == null) return;

            string kh = dr["MaKH"].ToString();
            string mh = dr["MaHang"].ToString();
            string dot = dr["MaDot"].ToString();
            string phieu = dr["MaPhieu"].ToString();
            frmPhanTichBom_XinNPL frm = new frmPhanTichBom_XinNPL(kh,mh, dot, phieu);
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
            frm.FormClosed += (s, agrs) =>
            {
                loadTQ();
            };
        }

        private void gV_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName != "TrangThai")
                return;

            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;

            string trangThai = Convert.ToString(view.GetRowCellValue(e.RowHandle, e.Column));

            if (trangThai == "Đã duyệt tổng")
            {
                e.Appearance.ForeColor = Color.Green;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (trangThai == "Đã soát xét")
            {
                e.Appearance.ForeColor = Color.Orange;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if(trangThai == "Đã xác nhận")
            {
                e.Appearance.ForeColor = Color.Blue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            //else if (trangThai == "Đã xác nhận BOM")
            //{
            //    e.Appearance.ForeColor = Color.Orange;
            //    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            //}
            //else if (trangThai == "Đã tạo BOM")
            //{
            //    e.Appearance.ForeColor = Color.Black;
            //}
        }
        private void loadAllSize()
        {
            string url = $"{URL}XinNPLMayMau/Get?action=GETSIZESP_TQ";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblAllSize = JsonConvert.DeserializeObject<DataTable>(json);
            }

        }

        private void gV_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "MaSizeKTTK" && e.Value != null)
            {
                string maSizeKTTK = e.Value.ToString();
                if (!string.IsNullOrEmpty(maSizeKTTK))
                {
                    e.DisplayText = ConvertMaSizeToDisplayText(maSizeKTTK);
                }
            }
        }
        private string ConvertMaSizeToDisplayText(string maSizeKTTK)
        {
            // Input: MaNhomSize1:MaSize1,MaSize2;MaNhomSize2:MaSize3
            // Output: NhomSize1:Size1,Size2;NhomSize2:Size3

            if (string.IsNullOrEmpty(maSizeKTTK)) return string.Empty;

            List<string> resultParts = new List<string>();

            // Tách theo dấu ; (các nhóm size)
            string[] nhomSizeParts = maSizeKTTK.Split(';');

            foreach (string nhomPart in nhomSizeParts)
            {
                if (string.IsNullOrEmpty(nhomPart.Trim())) continue;

                // Tách theo dấu : (MaNhomSize và danh sách MaSize)
                string[] parts = nhomPart.Split(':');
                if (parts.Length != 2) continue;

                string maNhomSize = parts[0].Trim();
                string[] maSizes = parts[1].Split(',');

                // Tìm tên nhóm size
                DataRow[] nhomRows = tblAllSize.Select($"MaNhomSize = '{maNhomSize}'");
                string tenNhomSize = nhomRows.Length > 0 ? nhomRows[0]["NhomSize"].ToString() : maNhomSize;

                // Tìm tên các size
                List<string> tenSizes = new List<string>();
                foreach (string maSize in maSizes)
                {
                    string maSizeTrim = maSize.Trim();
                    DataRow[] sizeRows = tblAllSize.Select($"MaSize = '{maSizeTrim}'");
                    string tenSize = sizeRows.Length > 0 ? sizeRows[0]["TenSize"].ToString() : maSizeTrim;
                    tenSizes.Add(tenSize);
                }

                // Ghép kết quả: NhomSize:Size1,Size2
                resultParts.Add($"{tenNhomSize}:{string.Join(",", tenSizes)}");
            }

            return string.Join(";", resultParts);
        }

        private void gV_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "MaSizeKTTK")
            {
                e.Cancel = true;
            }
        }
    }
}
