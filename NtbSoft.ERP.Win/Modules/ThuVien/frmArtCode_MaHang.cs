using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmArtCode_MaHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        public frmArtCode_MaHang()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            LoadMaHang();
            
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                //Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }
        private void LoadMaHang()
        {
            string url = string.Format("{0}", URL + "ArtSize_Mahang/Get?action=GetMaHang&Para1=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            grcMaHang.DataSource = dt;
        }
        private void LoadData()
        {
            var drFocus = grvMaHang.GetFocusedDataRow();
            if (drFocus is null) return;

            string url = string.Format("{0}", URL + $"ArtSize_Mahang/Get?action=Get&Para1={drFocus["StyleID"].ToString()}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            int rowfocus = grvArt.FocusedRowHandle;
            grcArtSize.DataSource = dt;
            grvArt.FocusedRowHandle = rowfocus;
        }
        private void SaveData(DataTable dtSave)
        {

            string url = string.Format("{0}?", URL + "ArtSize_Mahang/Post?action=POST&Para1=A");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult == "True") 
            {
                LoadMaHang();
                LoadData();

            }
        }


        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportExcel();
        }

        private void ImportExcel()
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";
                openFileDialog1.Title = "Import Excel";
                openFileDialog1.Multiselect = false;

                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult != DialogResult.OK) return;
                string pathExecel = openFileDialog1.FileName;
                if (string.IsNullOrEmpty(pathExecel)) return;
                string conString = "";
                string extension = System.IO.Path.GetExtension(pathExecel);
                switch (extension)
                {
                    case ".xls": //Excel 97-03
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString, pathExecel);
                        //conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                    case ".xlsx": //Excel 07 or higher
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                }
                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    if (excel_con.State == ConnectionState.Closed)
                    {
                        excel_con.Open();
                    }
                    //OleDbCommand _oleCmdSelect;
                    OleDbDataAdapter oleAdapter = new OleDbDataAdapter(); ;
                    DataTable sheets = GetSchemaTable(conString);


                    //OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                    //                             @"SELECT * FROM [Sheet1$] ", excel_con);
                    OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                                              @"SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString() + "] ", excel_con);
                    DataSet excelDataSet = new DataSet();
                    _oleCmdSelect.Fill(excelDataSet);
                    DataTable dt = new DataTable();
                    dt = excelDataSet.Tables[0];
                    DataTable dtSave = CreateTblSave();
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        var drnew = dtSave.NewRow();
                        drnew["ID"] = 0;
                        drnew["Material"] = dt.Rows[i][0];
                        drnew["StyleID"] = ReplaceSpecialCharacters(dt.Rows[i][1].ToString());
                        drnew["MaHang"] = dt.Rows[i][1];
                        drnew["MaMau"] = "MAU_" + ReplaceSpecialCharacters(dt.Rows[i][2].ToString());
                        drnew["TenMau"] = dt.Rows[i][2];
                        drnew["SizeID"] = "SIZE_" + ReplaceSpecialCharacters(dt.Rows[i][3].ToString());
                        drnew["Size"] = dt.Rows[i][3];
                        drnew["EANCode"] = dt.Rows[i][4];
                        drnew["ArtSize"] = dt.Rows[i][5];
                        drnew["VCD"] = dt.Rows[i][6];
                        drnew["ArtStyle"] = dt.Rows[i][7];
                        dtSave.Rows.Add(drnew);
                    }
                    SaveData(dtSave);

                    int rowCount = grvMaHang.RowCount;
                    {
                        grvMaHang.FocusedRowHandle = rowCount - 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private DataTable GetSchemaTable(string connectionString)
        {
            using (OleDbConnection connection = new
                       OleDbConnection(connectionString))
            {
                connection.Open();
                DataTable schemaTable = connection.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                return schemaTable;
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = grcArtSize.DataSource as DataTable;
            var drNew = dt.NewRow();
            var drLastRow = dt.Rows[dt.Rows.Count - 1];
            drNew["StyleID"] = drLastRow["StyleID"];
            drNew["MaHang"] = drLastRow["MaHang"];
            drNew["MaMau"] = drLastRow["MaMau"];
            drNew["TenMau"] = drLastRow["TenMau"];
            dt.Rows.InsertAt(drNew,0);
            
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = grcMaHang;

            DevExpress.XtraGrid.Views.Grid.GridView gridView = grcMaHang.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            int focusedRowHandle = gridView.FocusedRowHandle;
            var dtSave = grcArtSize.DataSource as DataTable;
            SaveData(dtSave);

            if (focusedRowHandle >= 0 && focusedRowHandle < gridView.RowCount)
            {
                gridView.FocusedRowHandle = focusedRowHandle;
            }
        }

        private void grvMaHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {          
            LoadData();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaMH();
        }

        private void XoaMH()
        {
            if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes) 
            {
                var styleid = grvMaHang.GetFocusedRowCellValue("StyleID");
                string url = string.Format("{0}", URL + $"ArtSize_Mahang/Delete?action=DELETE&Para1={styleid}");
                string msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (msResult == "True")
                {
                    LoadMaHang();
                    LoadData();
                }
            }

        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadMaHang();
            LoadData();
        }

        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }
        private DataTable CreateTblSave()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Material", typeof(string));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("EANCode", typeof(string));
            dt.Columns.Add("ArtSize", typeof(string));
            dt.Columns.Add("VCD", typeof(string));
            dt.Columns.Add("ArtStyle", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            return dt;
        }
    }
}
