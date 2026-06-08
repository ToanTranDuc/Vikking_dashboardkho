using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_VatTuDoiNhom : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _manhomn = string.Empty, _tennhom = string.Empty, _npl = string.Empty;
        List<DataRow> _rowSL = new List<DataRow>();
        DataTable _tbl = new DataTable();
        public frmERP_VatTuDoiNhom()
        {
           
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CreateSearchLookUpNhom();
        }
        public frmERP_VatTuDoiNhom(List<DataRow> rowSL,DataTable tbl,string manhom = "", string tennhom = "", string npl = "0" )
        {

            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _manhomn = manhom;
            _tennhom = tennhom;
            _npl = npl;
          
            _rowSL = rowSL;
            string result = string.Join(";",
                _rowSL
                .Select(r => r["TenNhom"].ToString())
                .Distinct()
            );
            textEdit1.Text = result;
            _tbl = tbl;
            CreateSearchLookUpNhom();
            this.ActiveControl = simpleButton1;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditNhom.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditNhom.EditValue.ToString()))
            {
                MessageBox.Show("Vui lòng chọn nhóm mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(checkVatTu())
            {
                MessageBox.Show("Trùng vật tư khi đổi sang nhóm mới. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }    
         
            this.DialogResult = DialogResult.OK;
        }

        private void CreateSearchLookUpNhom()
        {
            searchLookUpEditNhom.Properties.ValueMember = "MaNhom";
            searchLookUpEditNhom.Properties.DisplayMember = "TenNhom";
            searchLookUpEditNhom.Properties.NullText = "Chọn nhóm";
         
            string url = $"{URL}ERPVatTuBOM/GetDoiNhom?npl={_npl}&manhom={_manhomn}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tblNhom = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditNhom.Properties.DataSource = tblNhom;
        }
        public string getMaNhom()
        {
            if (searchLookUpEditNhom.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditNhom.EditValue.ToString()))
            {
                return _manhomn;
            }
            return searchLookUpEditNhom.EditValue.ToString();
        }

        private void searchLookUpEdit1View_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
           
            DataRowView rowView = (DataRowView)searchLookUpEdit1View.GetRow(e.ListSourceRow);
            if (rowView == null) return;
            bool isDuplicated = _rowSL.Any(dr =>
                dr["MaNhom"].Equals(rowView["MaNhom"])
              
            );

            if (isDuplicated)
            {
                e.Visible = false; // Ẩn dòng này
                e.Handled = true;
            }
           
        }

        public string getTenNhom()
        {
            if (searchLookUpEditNhom.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditNhom.EditValue.ToString()))
            {
                return _tennhom;
            }
            return searchLookUpEditNhom.Text.ToString();
        }
        public int getSortNhom()
        {
            if (searchLookUpEditNhom.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditNhom.EditValue.ToString()))
            {
                return 0;
            }
            DataRow dr = searchLookUpEdit1View.GetFocusedDataRow();
            return int.TryParse(dr["Sort"]?.ToString(), out var val1) ? val1 : 0;
        }
        private bool checkVatTu()
        {
          
            foreach (DataRow rowToCheck in _rowSL)
            {
                // Sử dụng LINQ để kiểm tra xem có dòng nào trong _tbl khớp với các cột chỉ định không
                bool isDuplicate = _tbl.AsEnumerable().Any(rowInTable =>
                    rowInTable.Field<object>("MaNhom")?.ToString() == searchLookUpEditNhom.EditValue.ToString() &&
                    rowInTable.Field<object>("MaVTID")?.ToString() == rowToCheck.Field<object>("MaVTID")?.ToString() &&
                    rowInTable.Field<object>("MauVTID")?.ToString() == rowToCheck.Field<object>("MauVTID")?.ToString() &&
                    rowInTable.Field<object>("KhoVaiID")?.ToString() == rowToCheck.Field<object>("KhoVaiID")?.ToString()
                );

              
                if (isDuplicate)
                {
                   
                    return true;
                }
            }

          
            return false;
        }
    }
}