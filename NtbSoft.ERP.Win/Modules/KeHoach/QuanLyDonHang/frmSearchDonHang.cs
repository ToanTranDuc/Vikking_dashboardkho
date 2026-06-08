using Newtonsoft.Json;
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
    public partial class frmSearchDonHang : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();
        // Khai báo sự kiện
        public event EventHandler<DataTable> OnDataUpdate;
        private HttpClientExtension _clientExtension;
        public frmSearchDonHang(int x, int y)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            SetPosition(x, y);
            _clientExtension = new HttpClientExtension();
        }
        private void SetPosition(int x, int y)
        {
            this.Location = new Point(x, y);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            // Gọi API và lấy dữ liệu
            DataTable _tblSearch = SearchDonHang();
            // Kích hoạt sự kiện và truyền dữ liệu về Form1
            OnDataUpdate?.Invoke(this, _tblSearch);
        }

        private DataTable SearchDonHang()
        {
            string _maHang = (textEdit1.EditValue!=null) ?textEdit1.EditValue.ToString():"";
            string _khachHang = (textEdit2.EditValue != null) ? textEdit2.EditValue.ToString() : "";
            string _chungLoai= (textEdit3.EditValue != null) ? textEdit3.EditValue.ToString() : "";
            string _dot = (textEdit5.EditValue != null) ? textEdit5.EditValue.ToString() : "";
            int _soLuong = 0;

            if (textEdit6.EditValue != null) {
                int.TryParse(textEdit6.EditValue.ToString(), out _soLuong);
            }
            
            string url = string.Format("{0}?maHang={1}&&khachHang={2}&&chungLoai={3}&&dot={4}&&soLuong={5}",
                URL + "DonHangTong/SearchDH", _maHang, _khachHang, _chungLoai,_dot,_soLuong);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return tbl;
        }

        private void textEdit6_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra xem ký tự có phải là số (0-9) hoặc phím điều khiển (backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // Nếu không phải là số thì hủy thao tác nhập
                e.Handled = true;
            }
        }
    }
}
