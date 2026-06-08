using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmThuVienMauSP : DevExpress.XtraEditors.XtraForm
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

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        List<DataTable> lstThuVienDaDung = new List<DataTable>();

        public frmThuVienMauSP()
        {
            InitializeComponent();
            this.KeyPreview = true;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }
        private void CreateSearchLookUpTheMau()
        {
            searchLookUpEditTheMau.Properties.DisplayMember = "TheMau";
            searchLookUpEditTheMau.Properties.ValueMember = "MaTheMau";

            string url = string.Format("{0}", URL + "ThuVien/Get?Action=GETTHEMAU");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditTheMau.Properties.DataSource = tbl;
        }

        private void searchLookUpEditTheMau_EditValueChanged(object sender, EventArgs e)
        {

            string theMau = searchLookUpEditTheMau.Text.ToString();
            txtTheMau.EditValue = theMau;
        }
        protected override void OnLoad(EventArgs e)
        {
            layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            LoadData();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditTheMau.IsPopupOpen)
                searchLookUpEditTheMau.ClosePopup();
            else
            {
                CreateSearchLookUpTheMau();
                searchLookUpEditTheMau.ShowPopup();
            }

        }

        private void LoadData()
        {
            string url = string.Format("{0}", URL + "ThuVien/Get?Action=GETTHEMAUCT");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            gridBangMau.DataSource = tbl;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            DataTable tbl = gridBangMau.DataSource as DataTable;
            string theMau = txtTheMau.Text.Trim();
            string mau = txtMau.Text.Trim();
            string codeMau = txtCodeMau.Text.Trim();

            if (string.IsNullOrWhiteSpace(theMau) || string.IsNullOrWhiteSpace(mau))
            {
                MessageBox.Show("Vui lòng nhập Thẻ màu và Màu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tbl == null)
            {
                tbl = new DataTable();
                tbl.Columns.Add("TheMau");
                tbl.Columns.Add("Mau");
                tbl.Columns.Add("CodeMau");
                gridBangMau.DataSource = tbl;
            }

            string[] mauArray = mau.Split(':');
            string[] codeArray = codeMau.Split(':');
            int maxLength = Math.Max(mauArray.Length, codeArray.Length);

            for (int i = 0; i < maxLength; i++)
            {
                string mauItem = i < mauArray.Length ? mauArray[i].Trim() : "";
                string codeItem = i < codeArray.Length ? codeArray[i].Trim() : "";

                if (i >= codeArray.Length)
                    codeItem = "";

                // Kiểm tra trùng trong DataTable
                bool isDuplicate = tbl.AsEnumerable().Any(r =>
                    r.Field<string>("TheMau") == theMau &&
                    r.Field<string>("TenMau") == mauItem);

                if (!isDuplicate)
                {
                    tbl.Rows.Add(0,"",theMau, "", mauItem, codeItem);
                }
            }
            gridBangMau.DataSource = tbl;
        }
        private DataTable CreateTableSave(DataTable tblGrid)
        {

            DataTable tblSave = new DataTable("tblSave");
            #region tạo cột 
            tblSave.Columns.Add("MaTheMau", typeof(string));
            tblSave.Columns.Add("TheMau", typeof(string));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("TenMau", typeof(string));
            tblSave.Columns.Add("CodeMau", typeof(string));
            tblSave.Columns.Add("HinhAnh", typeof(string));
            tblSave.Columns.Add("GhiChu", typeof(string));
          
            #endregion
            foreach (DataRow row in tblGrid.Rows)
            {
                DataRow dr = tblSave.NewRow();
                dr["MaTheMau"] = row["MaTheMau"].ToString() == "" ? "" : row["MaTheMau"].ToString();
                dr["TheMau"] = row["TheMau"].ToString();
                dr["MaMau"] = row["MaMau"].ToString() == "" ? "" : row["MaNPL"].ToString();
                dr["TenMau"] = row["TenMau"];
                dr["CodeMau"] = row["CodeMau"];
                dr["HinhAnh"] = row["HinhAnh"];
                dr["GhiChu"] = "";
                tblSave.Rows.Add(dr);
            }
            return tblSave;
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGrid = gridBangMau.DataSource as DataTable;
                if (tblGrid == null || tblGrid.Rows.Count == 0)
                {
                    return;
                }
                DataTable _dtSave = CreateTableSave(tblGrid);

                if (_dtSave == null || _dtSave.Rows.Count == 0)
                {
                    return;
                }
                string url = string.Format("{0}", URL + "ThuVien/Post?Action=POSTTHEMAU");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                if (msResult.ToLower() == "true")
                {


                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}
