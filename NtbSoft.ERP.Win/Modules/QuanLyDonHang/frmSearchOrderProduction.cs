using DevExpress.XtraEditors;
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
    public partial class frmSearchOrderProduction : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        GetDataService _serviceGetData = new GetDataService();
        string URL = string.Empty;
        public string _resultDonHang = string.Empty;
        public frmSearchOrderProduction(int x, int y)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            SetPosition(x, y);
            GetListDVSX();
            txtMaLenh.Text = string.Empty;
        }
        private void SetPosition(int x, int y)
        {
            this.Location = new Point(x,y);
        }
        private void GetListDVSX()
        {
            string url = URL + "DonViSanXuat/GetDonViSanXuat";
            DataTable tbt = Task.Run(
                async () =>
                {
                    return await _serviceGetData.GetDataTable(url);
                }
                ).Result;
            //DataRow[] filteredRows = tbt.Select("GiaCong = False");
            //DataTable tb = tbt.Clone();
            //foreach (DataRow row in filteredRows)
            //{
            //    tb.ImportRow(row);
            //}
            if (tbt.Rows.Count == 0)
            {
                string messenger = "Chưa có dữ liệu đơn vị sản xuất hoặc lấy thông tin thất bại!";
                MessageBox.Show(messenger,"Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            slueChooseDVSX.Properties.DataSource = tbt;
            slueChooseDVSX.Properties.DisplayMember = "TenDVSX";
            slueChooseDVSX.Properties.ValueMember = "MaDVSX";
            slueChooseDVSX.EditValue = tbt.Rows[0]["MaDVSX"];
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (slueChooseDVSX.EditValue == null)
            {
                string messenger = "Chưa chọn đơn vị sản xuất!";
                MessageBox.Show(messenger, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtMaLenh.Text))
            {
                string messenger = "Chưa nhập lệnh sản xuất cần tìm!";
                MessageBox.Show(messenger, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string MaDVSX = slueChooseDVSX.EditValue.ToString();

            string url = URL + "CanDoiDonHangTong/GetDHClient?maDVSX=" + MaDVSX + "&maLenh=" + this.txtMaLenh.Text;
            string result = Task.Run(
                async () =>
                {
                    return await _serviceGetData.GetStr(url);
                }
                ).Result;
            if (result.ToLower() == "nodata")
            {
                string messenger = "Không có đơn hàng với mã lệnh tương ứng được kéo về ở đơn vị sản xuất đã chọn!";
                MessageBox.Show(messenger, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (result.ToLower() == "false")
            {
                string messenger = "Xảy ra lỗi khi lấy dữ liệu!";
                MessageBox.Show(messenger, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _resultDonHang = result;
            this.Close();
        }
        //private void frmSearchOrderProduction_Deactivate(object sender, EventArgs e)
        //{
        //    //_resultDonHang = string.Empty;
        //    //this.Close();
        //}

        private void btnClear_Click(object sender, EventArgs e)
        {
            _resultDonHang = "Clean";
            this.Close();
        }

        private void txtMaLenh_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thường
            if (char.IsLower(e.KeyChar))
            {
                // Chuyển đổi ký tự thành chữ hoa
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }
    }
}