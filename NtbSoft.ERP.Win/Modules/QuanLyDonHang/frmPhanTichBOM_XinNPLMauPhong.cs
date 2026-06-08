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
    public partial class frmPhanTichBOM_XinNPLMauPhong : DevExpress.XtraEditors.XtraForm
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
        public frmPhanTichBOM_XinNPLMauPhong(string makh, string mahang, string mamauchung, DataTable tbl, string madot="")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._makh = makh;
            this._mahang = mahang;
            this._mamauchung = mamauchung;
            this.tblGrid = tbl;

        }
        protected override void OnLoad(EventArgs e)
        {
            loadSize();
        }
        private void loadSize()
        {
            string url = $"{URL}XinNPLMayMau/Get?action=GETMAUCOLUMNSPHONG&para1={_makh.ToString()}&para2={_mahang.ToString()}";
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
                gridView1.ClearSelection();
                CheckByPattern("@MA_P1@");
                CheckByPattern("@MA_P2@");
                CheckByPattern("@MA_P3@");
                CheckByPattern("@MA_P4@");
               
            }
       
        }
      
        private void CheckByPattern(string pattern)
        {
            try
            {
                //gridView1.SelectionChanged -= gVMau_SelectionChanged;
               
                gridView1.BeginUpdate();

                foreach (DataColumn col in tblGrid.Columns)
                {
                    string columnName = col.ColumnName;

                    if (columnName.Contains(pattern))
                    {
                        string[] parts = columnName.Split('@');
                        if (parts.Length < 3)
                            continue;

                        string maPhongValue = parts[1];
                        string maMauValue = parts[2];

                        for (int i = 0; i < gridView1.DataRowCount; i++)
                        {
                            object phong = gridView1.GetRowCellValue(i, "MaPhong");
                            object mau = gridView1.GetRowCellValue(i, "MaMau");

                            if (phong != null && mau != null &&
                                phong.ToString() == maPhongValue &&
                                mau.ToString() == maMauValue)
                            {
                                gridView1.SelectRow(i);
                            }
                        }
                    }
                }

                gridView1.EndUpdate();
                gridView1.RefreshData();
                gridControl1.Refresh();
                //gridView1.SelectionChanged += gVMau_SelectionChanged;
            }
            catch (Exception ex)
            {
               
            }
        }
      
        private void gVMau_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            
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
     
    }
}
