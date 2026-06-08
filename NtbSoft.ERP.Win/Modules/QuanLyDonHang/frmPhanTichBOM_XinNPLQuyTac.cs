using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBOM_XinNPLQuyTac : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string _makh = string.Empty, _mahang = string.Empty,_mamauchung = string.Empty;
        //private Dictionary<string, string> selectRows = new Dictionary<string, string>();
        public string SelectedResult { get; set; }
        public string SelectedResultName { get; set; }

        public DataTable selectedTable = new DataTable();
        private DataTable tblGrid = new DataTable();
        int selectedPage = 0;
        public frmPhanTichBOM_XinNPLQuyTac()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
          

        }
        protected override void OnLoad(EventArgs e)
        {
            //tblGrid = createTabletbl();
            CreateSearchLookUpQT();
            CreateSearchLookUpPhong();
            CreateSearchLookUpDonVi();
            loadData();

        }
        private DataTable createTabletbl()
        {
            DataTable tbl = new DataTable();

            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaQT", typeof(string));
            tbl.Columns.Add("TenQT", typeof(string));
            tbl.Columns.Add("Sort", typeof(int));
            tbl.Columns.Add("TypeID", typeof(string));
            tbl.Columns.Add("LoaiQT", typeof(string));
            tbl.Columns.Add("MaPhong", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(decimal));
            tbl.Columns.Add("MaDVVT", typeof(string));
            return tbl;
        }
        private void loadData()
        {
            try
            {
                int index = xtraTabControl1.SelectedTabPageIndex;

                string _maphong = string.Empty;
                GridControl grid;
                if (index == 0)
                {
                    _maphong = "MA_P1";
                    grid = gridControl1;
                }
                else if (index == 1)
                {
                    _maphong = "MA_P2";
                    grid = gridControl2;
                }
                else if (index == 2)
                {
                    _maphong = "MA_P3";
                    grid = gridControl3;
                }
                else
                {
                    _maphong = "MA_P4";
                    grid = gridControl4;
                }


                string url = $"{URL}XinNPLMayMau/Get?action=GETQUITAC&para1={_maphong.ToString()}&para2={_mahang.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblGrid = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tblGrid == null || tblGrid.Rows.Count == 0)
                    {
                        grid.DataSource = null;
                        return;
                    }


                    grid.DataSource = tblGrid;



                }
                else
                {
                    tblGrid = createTabletbl();
                    grid.DataSource = tblGrid;

                }
            }
            catch (Exception ex)
            {

            }
          
       
        }
      
        
        private void btnLuuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Lưu
            this.ActiveControl = simpleButton1;
            if (tblGrid == null || tblGrid.Rows.Count == 0)
                return;

            DataTable tblSave = createTabletbl();

            foreach(DataRow dr in tblGrid.Rows)
            {
                DataRow _nr = tblSave.NewRow();

                _nr["ID"] = dr["ID"];
                _nr["MaQT"] = dr["MaQT"];
                _nr["TenQT"] = dr["TenQT"];
                _nr["Sort"] = dr["Sort"];
                _nr["TypeID"] = dr["TypeID"];
                _nr["LoaiQT"] = dr["LoaiQT"];
                _nr["MaPhong"] = dr["MaPhong"];
                _nr["SoLuong"] = dr["SoLuong"];
                _nr["MaDVVT"] = dr["MaDVVT"];
                tblSave.Rows.Add(_nr);
            }
            string url = $"{URL}XinNPLMayMau/PostT3?Action=POST_QUITAC&para1={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult == "True")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                loadData();
            }

        }

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Nạp lại
            loadData();
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Xóa
            int index = xtraTabControl1.SelectedTabPageIndex;

            string _maphong = string.Empty;
            GridView grid;
            GridControl gridControl;
            if (index == 0)
            {
                _maphong = "MA_P1";
                grid = gridView1;
                gridControl = gridControl1;
            }
            else if (index == 1)
            {
                _maphong = "MA_P2";
                grid = gridView3;
                gridControl = gridControl2;
            }
            else if (index == 2)
            {
                _maphong = "MA_P3";
                grid = gridView6;
                gridControl = gridControl3;
            }
            else
            {
                _maphong = "MA_P4";
                grid = gridView9;
                gridControl = gridControl4;
            }


            DataRow dr = grid.GetFocusedDataRow();
            if (dr == null) return;

            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {

                if (dr["ID"].ToString() != "0")
                {
                    string url = $"{URL}XinNPLMayMau/Delete?Action=DELETE_QUITAC&para1={dr["ID"]}&para2={GlobleData.UserName}";
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                }
                tblGrid.Rows.Remove(dr);
                gridControl.DataSource = tblGrid;
                this.ActiveControl = simpleButton1;

            }
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Thêm
            int index = xtraTabControl1.SelectedTabPageIndex;
            string _maphong = string.Empty;
            GridControl grid;
            if (index == 0)
            {
                _maphong = "MA_P1";
                grid = gridControl1;
            }
            else if (index == 1)
            {
                _maphong = "MA_P2";
                grid = gridControl2;
            }
            else if (index == 2)
            {
                _maphong = "MA_P3";
                grid = gridControl3;
            }
            else
            {
                _maphong = "MA_P4";
                grid = gridControl4;
            }


            DataRow dr = tblGrid.NewRow();
            dr["MaPhong"] = _maphong;
            dr["MaDVVT"] = "ALL";
            tblGrid.Rows.Add(dr);
            gridControl1.DataSource = tblGrid;



        }

     

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridView1;
            DataTable sourceTable = (DataTable)gridControl1.DataSource;
            selectedTable = sourceTable.Clone();

            int[] selectedHandles = view.GetSelectedRows();
            foreach (int rowHandle in selectedHandles)
            {
                if (rowHandle < 0) continue;
                DataRow dataRow = view.GetDataRow(rowHandle);
                if (dataRow != null)
                {
                    selectedTable.ImportRow(dataRow);
                }
            }

            this.DialogResult = DialogResult.OK;
        }

     
        private void CreateSearchLookUpQT()
        {
            try
            {
                repoSearch.DisplayMember = "TenQT";
                repoSearch.ValueMember = "MaQT";
                repoSearch2.DisplayMember = "TenQT";
                repoSearch2.ValueMember = "MaQT";
                repoSearch3.DisplayMember = "TenQT";
                repoSearch3.ValueMember = "MaQT";
                repoSearch4.DisplayMember = "TenQT";
                repoSearch4.ValueMember = "MaQT";
   

                string url = $"{URL}XinNPLMayMau/Get?action=GETQUITAC_SEARCH";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearch.DataSource = tbl;
                repoSearch2.DataSource = tbl;
                repoSearch3.DataSource = tbl;
                repoSearch4.DataSource = tbl;

            }
            catch (Exception ex)
            {
            }
        }
        private void CreateSearchLookUpDonVi()
        {
            try
            {
                repoDonVi.DisplayMember = "TenDVVT";
                repoDonVi.ValueMember = "MaDVVT";

                repoDonVi2.DisplayMember = "TenDVVT";
                repoDonVi2.ValueMember = "MaDVVT";

                repoDonVi3.DisplayMember = "TenDVVT";
                repoDonVi3.ValueMember = "MaDVVT";

                repoDonVi4.DisplayMember = "TenDVVT";
                repoDonVi4.ValueMember = "MaDVVT";

           

                string url = $"{URL}XinNPLMayMau/Get?action=GETQUITAC_SEARCHDONVI";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                repoDonVi.DataSource = tbl;
                repoDonVi2.DataSource = tbl;
                repoDonVi3.DataSource = tbl;
                repoDonVi4.DataSource = tbl;

            }
            catch (Exception ex)
            {
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            loadData();
        }

      
        private void CreateSearchLookUpPhong()
        {
            try
            {
                repoPhong.DisplayMember = "TenPhong";
                repoPhong.ValueMember = "MaPhong";

                string url = $"{URL}XinNPLMayMau/Get?action=GETPHONG";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                repoPhong.DataSource = tbl;


            }
            catch (Exception ex)
            {
            }
        }

       

        private void repoSearch_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor == null) return;

            int rowHandle = editor.Properties.View.FocusedRowHandle;
            var selectedRow = editor.Properties.View.GetRow(rowHandle);

            var dataRow = selectedRow as DataRowView;

            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;

            dr["LoaiQT"] = dataRow["LoaiQT"];
            dr["MaQT"] = dataRow["MaQT"];
            dr["TenQT"] = dataRow["TenQT"];
            dr["TypeID"] = dataRow["TypeID"];
            dr["Sort"] = dataRow["Sort"];
        }

     

        private void repoSearch2_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor == null) return;

            int rowHandle = editor.Properties.View.FocusedRowHandle;
            var selectedRow = editor.Properties.View.GetRow(rowHandle);

            var dataRow = selectedRow as DataRowView;

            DataRow dr = gridView3.GetFocusedDataRow();
            if (dr == null) return;

            dr["LoaiQT"] = dataRow["LoaiQT"];
            dr["MaQT"] = dataRow["MaQT"];
            dr["TenQT"] = dataRow["TenQT"];
            dr["TypeID"] = dataRow["TypeID"];
            dr["Sort"] = dataRow["Sort"];
        }
        private void repoSearch3_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor == null) return;

            int rowHandle = editor.Properties.View.FocusedRowHandle;
            var selectedRow = editor.Properties.View.GetRow(rowHandle);

            var dataRow = selectedRow as DataRowView;

            DataRow dr = gridView6.GetFocusedDataRow();
            if (dr == null) return;

            dr["LoaiQT"] = dataRow["LoaiQT"];
            dr["MaQT"] = dataRow["MaQT"];
            dr["TenQT"] = dataRow["TenQT"];
            dr["TypeID"] = dataRow["TypeID"];
            dr["Sort"] = dataRow["Sort"];
        }
        private void repoSearch4_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor == null) return;

            int rowHandle = editor.Properties.View.FocusedRowHandle;
            var selectedRow = editor.Properties.View.GetRow(rowHandle);

            var dataRow = selectedRow as DataRowView;

            DataRow dr = gridView9.GetFocusedDataRow();
            if (dr == null) return;

            dr["LoaiQT"] = dataRow["LoaiQT"];
            dr["MaQT"] = dataRow["MaQT"];
            dr["TenQT"] = dataRow["TenQT"];
            dr["TypeID"] = dataRow["TypeID"];
            dr["Sort"] = dataRow["Sort"];
        }
    }
}
