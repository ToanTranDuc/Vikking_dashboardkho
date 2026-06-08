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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmKhaiBaoKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tbl_Kho = new DataTable();
        public frmKhaiBaoKho()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadDVSX();
        }
        private void CreateTbl()
        {
            tbl_Kho = new DataTable();
            tbl_Kho.Columns.Add("MaKho", typeof(string));
            tbl_Kho.Columns.Add("TenKho", typeof(string));
            tbl_Kho.Columns.Add("Ghichu", typeof(string));
            tbl_Kho.Columns.Add("Status_VT", typeof(int));
            tbl_Kho.Columns.Add("ParentMaKho", typeof(string));
            tbl_Kho.Columns.Add("CBM", typeof(float));
        }
        private void LoadDVSX()
        {
            string userName = GlobleData.UserName;
            string url = $"{URL}ViTriKhoNew/Get?Action=GetDVSX&Para1={userName}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                grcDVSX.DataSource = null;
            else grcDVSX.DataSource = tbl;
        }
        private void LoadMaKho(string dv)
        {
            string madvsx = dv;
            string url = $"{URL}ViTriKhoNew/Get?Action=GetTenKho&Para1={madvsx}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                CreateTbl();
                grcAddKho.DataSource = tbl_Kho;

            }
            else grcAddKho.DataSource = tbl;
        }
        string maxMaKho = "";
        private void LoadMaxMaKho()
        {
         
            string url = $"{URL}ViTriKhoNew/Get?Action=GetMaxKho&Para1=a&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                maxMaKho = "KH000";
            else maxMaKho = tbl.Rows[0]["MaKho"].ToString() ;
        }
        private void grvDVSX_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow drRow = grvDVSX.GetFocusedDataRow();
            if (drRow == null) {
                grcAddKho.DataSource = null;
                return;
            }
            
            string maDV = drRow["MaDVSX"].ToString();
            LoadMaKho(maDV);
        }
        private string CreateMa(string MaKho)
        {
            var match = Regex.Match(MaKho, @"\d+");

            return match.Value;
        }
        private void SaveKHo()
        {
            this.ActiveControl = grcDVSX;
            LoadMaxMaKho();
            CreateTbl();
            DataRow drRowDV = grvDVSX.GetFocusedDataRow();
            DataTable tbl = grcAddKho.DataSource as DataTable;
            foreach (DataRow item in tbl.Rows)
            {
                string Makho = "";
                if (item["MaKho"] == DBNull.Value)
                {
                    Makho = "KH00" + (Convert.ToInt32(CreateMa(maxMaKho)) + 1);
                    maxMaKho = Makho;
                }
                else
                    Makho = item["MaKho"].ToString();
                DataRow drRow = tbl_Kho.NewRow();
                drRow["MaKho"] = Makho;
                drRow["TenKho"] = item["TenKho"];
                drRow["Ghichu"] = item["GhiChu"];
                drRow["Status_VT"] = item["Status_VT"];
                drRow["ParentMaKho"] = drRowDV["MaDVSX"];
                drRow["CBM"] = drRow["CBM"];
                tbl_Kho.Rows.Add(drRow);
            }
            string url = $"{URL}ViTriKhoNew/PostKho?Action=SaveKho";
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl_Kho); }).Result;
            clsWaitForm.ShowSuccessForm(this, 1000);
            LoadMaKho(drRowDV["MaDVSX"].ToString());
        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveKHo();
        }
        private void deleteRow()
        {
            DataRow drRow = grvAddKho.GetFocusedDataRow();
            if(drRow == null)
            {
                MessageBox.Show("Vui lòng chọn kho để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string makho = drRow["MaKho"].ToString();
            string madvsx = drRow["ParentMaKho"].ToString();
            string url = $"{URL}ViTriKhoNew/Get?Action=Delete&Para1={makho}&Para2={madvsx}&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            LoadMaKho(madvsx);
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = MessageBox.Show("Bạn muốn xóa kho này chưa", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if(result == DialogResult.Yes)
            {
                deleteRow();
            }
        }

        private void barButtonItemNL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDVSX();
        }
    }
}

