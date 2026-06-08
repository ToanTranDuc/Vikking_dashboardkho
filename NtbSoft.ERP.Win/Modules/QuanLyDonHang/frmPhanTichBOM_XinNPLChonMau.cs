using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBOM_XinNPLChonMau : DevExpress.XtraEditors.XtraForm
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
        public frmPhanTichBOM_XinNPLChonMau(string makh, string mahang, string mamauchung,DataTable tbl, string madot="")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._makh = makh;
            this._mahang = mahang;
            this._mamauchung = mamauchung;
            this.tblGrid = tbl;
            //bool isCheck = AreCountsEqual(tbl);
            //if(isCheck)
            //{

            //}    
        }
        protected override void OnLoad(EventArgs e)
        {
            loadSize();
        }
        private void loadSize()
        {
            string url = $"{URL}XinNPLMayMau/Get?action=GETMAUSPCOLUMNS&para1={_makh.ToString()}&para2={_mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblMau == null || tblMau.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    return;
                }

               
                gridControl1.DataSource = tblMau;
                checkMau();
            }
           
        }
        //public bool AreCountsEqual(DataTable dt)
        //{
        //    if (dt == null || dt.Rows.Count == 0)
        //        return false;

        //    string[] targetColumns = { "@Mau@", "@Ma_P1@", "@MA_P2@", "@MA_P3@", "@MA_P4@" };
        //    int[] counts = new int[targetColumns.Length];

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        for (int i = 0; i < targetColumns.Length; i++)
        //        {
        //            foreach (DataColumn col in dt.Columns)
        //            {
        //                if (row[col] != DBNull.Value && row[col].ToString().Contains(targetColumns[i]))
        //                    counts[i]++;
        //            }
        //        }
        //    }

        //    return counts.Distinct().Count() == 1;
        //}
        private void checkMau()
        {
            try
            {
                gridView1.SelectionChanged -= gVMau_SelectionChanged;
                gridView1.ClearSelection();
                gridView1.BeginUpdate();
                foreach (DataColumn col in tblGrid.Columns)
                {
                    string columnName = col.ColumnName;

                    if (columnName.Contains("@Mau@"))
                    {
                        string[] parts = columnName.Split('@');
                        string maMau = parts[2];

                        int rowHandle = gridView1.LocateByValue("MaMau", maMau);
                        if (rowHandle >= 0)
                            gridView1.SelectRow(rowHandle);
                    }
                }
                gridView1.EndUpdate();
                gridView1.SelectionChanged += gVMau_SelectionChanged;
            }
            catch (Exception ex)
            {

              
            }
            

        }
        Dictionary<string, HashSet<string>> ParseMaSizeChung(string source)
        {
            var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            var groups = source.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var group in groups)
            {
                var parts = group.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) continue;
                var key = parts[0].Trim();
                var sizes = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!result.TryGetValue(key, out var set))
                {
                    set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    result[key] = set;
                }
                foreach (var s in sizes)
                    set.Add(s.Trim());
            }
            return result;
        }
        private void gVMau_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null) return;
        
            //int[] selectedRowHandles = view.GetSelectedRows();
            //foreach (int rowHandle in selectedRowHandles)
            //{
            //    if (rowHandle >= 0) 
            //    {
            //        object maMauObj = view.GetRowCellValue(rowHandle, "MaMau");
            //        object tenMauObj = view.GetRowCellValue(rowHandle, "TenMau");

            //        if (maMauObj != null && tenMauObj != null)
            //        {
            //            string maMau = maMauObj.ToString().Trim();
            //            string tenMau = tenMauObj.ToString().Trim();
                        
            //        }
            //    }
            //}
        }

       

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridView1;
            DataTable sourceTable = (DataTable)gridControl1.DataSource;
            selectedTable = sourceTable.Clone();

            int[] selectedHandles = view.GetSelectedRows();
            foreach (int rowHandle in selectedHandles)
            {
                DataRow dataRow = view.GetDataRow(rowHandle);
                if (dataRow != null)
                {
                    selectedTable.ImportRow(dataRow);
                }
            }

            this.DialogResult = DialogResult.OK;
        }
     
    }
}
