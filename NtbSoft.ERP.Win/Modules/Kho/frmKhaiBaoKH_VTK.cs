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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmKhaiBaoKH_VTK : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _make = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tbl_KhaiBao = new DataTable();
        public frmKhaiBaoKH_VTK()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadInit();
            LoadDVSX();
        }
        private void loadInit()
        {
            searchLookUpEdit1.Properties.ValueMember = "MaDVSX";
            searchLookUpEdit1.Properties.DisplayMember = "TenDVSX";
            searchLookUpEdit1.Properties.NullText = "[Chọn đơn vị sx]";

            searchLookUpEdit2.Properties.ValueMember = "MaKho";
            searchLookUpEdit2.Properties.DisplayMember = "TenKho";
            searchLookUpEdit2.Properties.NullText = "[Chọn kho]";
        }
        private void CreateTbl()
        {
            tbl_KhaiBao = new DataTable();
            tbl_KhaiBao.Columns.Add("MaDay", typeof(string));
            tbl_KhaiBao.Columns.Add("TenDay", typeof(string));
            tbl_KhaiBao.Columns.Add("TenKH", typeof(string));
            tbl_KhaiBao.Columns.Add("MaKH", typeof(string));
            tbl_KhaiBao.Columns.Add("IsCheck", typeof(int));
            tbl_KhaiBao.Columns.Add("MaDVSX", typeof(string));
            tbl_KhaiBao.Columns.Add("MaKho", typeof(string));
        }
        private void LoadDVSX()
        {
            string userName = GlobleData.UserName;
            string url = $"{URL}KhaiBaoKH_VTK/Get?Action=GetNhaMayKho&Para1={userName}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
                return;
            else
                searchLookUpEdit1.Properties.DataSource = tbl;

        }
        private void LoadMaKho()
        {
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string url = $"{URL}KhaiBaoKH_VTK/Get?Action=GetViTriKho&Para1={madvsx}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
                return;
            else
            {
                searchLookUpEdit2.Properties.DataSource = tbl;
                searchLookUpEdit2.EditValue = tbl.Rows[0]["MaKho"].ToString();
                LoadKhuVucKho();
            }

        }
        private void LoadKhuVucKho()
        {
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue.ToString();
            string url = $"{URL}KhaiBaoKH_VTK/Get?Action=GetDay&Para1={madvsx}&Para2={makho}&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return;
            else
                grcDay.DataSource = tbl;

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            LoadMaKho();
        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            LoadKhuVucKho();
        }

        private void grwDay_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadKH();
        }

        private void LoadKH()
        {
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue == null ? "" : searchLookUpEdit2.EditValue.ToString();
            DataRow drRow = grwDay.GetFocusedDataRow();
            if (drRow == null) return;
            string maday = drRow["MaDay"].ToString();
            string url = $"{URL}KhaiBaoKH_VTK/Get?Action=GetKH&Para1={madvsx}&Para2={makho}&Para3={maday}&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return;
            else
                grcKH.DataSource = tbl;
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            CreateTbl();
            this.ActiveControl = grcDay;
            DataTable tbl = grcKH.DataSource as DataTable;
            DataRow drRow = grwDay.GetFocusedDataRow();
            if (tbl == null || tbl.Rows.Count == 0 || drRow == null) return;
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho =  searchLookUpEdit2.EditValue.ToString();
            string maday = drRow["MaDay"].ToString();
            string tenday = drRow["NameDay"].ToString();
            foreach (DataRow item in tbl.Rows)
            {
                if (Convert.ToBoolean(item["CheckStatus"]))
                {
                    DataRow dr = tbl_KhaiBao.NewRow();
                    dr["MaDay"] = maday;
                    dr["TenDay"] = tenday;
                    dr["TenKH"] = item["TenKH"];
                    dr["MaKH"] = item["MaKH"];
                    dr["IsCheck"] = 1;
                    dr["MaDVSX"] = madvsx;
                    dr["MaKho"] = makho;
                    tbl_KhaiBao.Rows.Add(dr);
                }
            }
            string url = $"{URL}KhaiBaoKH_VTK/Post";
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl_KhaiBao); }).Result;
            clsWaitForm.ShowSuccessForm(this, 1000);
            LoadKH();
        }
    }
}