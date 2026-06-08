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
    public partial class frmPhanTichBOM_TONG : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _tenTau="";
        public frmPhanTichBOM_TONG()
        {
            InitializeComponent();
         
         
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
         
        }
        protected override void OnLoad(EventArgs e)
        {
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
                string urlTau = $"{URL}KhoiTaoBOMV1/Get?Action=GETTRANGTHAIBOM&para1={kh}&para2={mh}&para3={tt}";
                
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
              
                string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETKH_TQ";
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
                string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMH_TQ&para1={kh}";
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
                string url = $"{URL}ERPNhapKhoNPLPOMUA/Get?Action=GETTRANGTHAI";
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
            frmPhanTichBOM frm = new frmPhanTichBOM();
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
            frm.FormClosed += (s, agrs) =>
            {
                loadTQ();
            };
        }

        private void btnSuaLo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         
            frmERPNhapKhoNPLSua frm = new frmERPNhapKhoNPLSua();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
            frm.FormClosed += (s, args) =>
            {
                loadTQ();
            };
        }

        private void btnSuaVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            frmERPNhapKhoNPLSuaCay frm = new frmERPNhapKhoNPLSuaCay();
            frm.StartPosition = FormStartPosition.CenterScreen;
           
            frm.Show();
            frm.FormClosed += (s, args) =>
            {
                loadTQ();
            };
        }

        private void repoChiTiet_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow dr = gV.GetFocusedDataRow();
            if (dr == null) return;

            string kh = dr["MaKH"].ToString();
            string mh = dr["MaHang"].ToString();
            string dot = dr["MaDot"].ToString();

            frmPhanTichBOM frm = new frmPhanTichBOM(kh,mh, dot);
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

            if (trangThai == "Đã active BOM")
            {
                e.Appearance.ForeColor = Color.Green;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (trangThai == "Đã xác nhận BOM")
            {
                e.Appearance.ForeColor = Color.Orange;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (trangThai == "Đã tạo BOM")
            {
                e.Appearance.ForeColor = Color.Black;
            }
            else if (trangThai == "Đã hủy BOM")
            {
                e.Appearance.ForeColor = Color.Red;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
        }
    }
}
