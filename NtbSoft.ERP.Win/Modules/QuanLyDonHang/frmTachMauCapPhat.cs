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
    public partial class frmTachMauCapPhat : DevExpress.XtraEditors.XtraForm
    {
        DataTable _tblMau = new DataTable();
        public string _mamau = "", _tenmau = "";
        public frmTachMauCapPhat(DataTable tblMau, DataRow dr)
        {
            InitializeComponent();
            _tblMau = tblMau;
            textBox1.Text = dr["ChiTiet"].ToString();
            textBox2.Text = dr["MauVT"].ToString();
            textBox3.Text = dr["KhoVai"].ToString();
            textBox4.Text = dr["TenDVVT"].ToString();
            searchLookUpEditMaMau.Properties.DisplayMember = "TenMau";
            searchLookUpEditMaMau.Properties.ValueMember = "MaMau";
            searchLookUpEditMaMau.Properties.DataSource = tblMau;
            this.AcceptButton = button1;
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit1View.GetRowCellValue(rowHandle2, searchLookUpEditMaMau.Properties.ValueMember)));
            searchLookUpEditMaMau.EditValue = selectedValues;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _mamau = string.Join("| ", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit1View.GetRowCellValue(rowHandle2, searchLookUpEditMaMau.Properties.ValueMember)));
            _tenmau = string.Join("| ", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditMaMau.Properties.DisplayMember)));
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void searchLookUpEditMaMau_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string displaytest = string.Join("; ", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditMaMau.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(displaytest))
            {
                e.DisplayText = "Chọn Màu sản phẩm";
            }
            else
            {
                e.DisplayText = displaytest;
            }
        }
    }
}
