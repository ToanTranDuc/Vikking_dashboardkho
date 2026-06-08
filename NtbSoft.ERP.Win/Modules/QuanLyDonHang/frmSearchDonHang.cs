using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
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
        private List<DonViSanXuatEntity> _listDVSX = new List<DonViSanXuatEntity>();
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
            LoadDonViSanXuat();
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
            string _maLenh = (txtLenhSX.EditValue != null) ? txtLenhSX.EditValue.ToString() : "";
            string _po = (txtPO.EditValue != null) ? txtPO.EditValue.ToString() : "";
            string _dvsx = (searchLookUpEditDVSX.EditValue != null) ? searchLookUpEditDVSX.EditValue.ToString() : "";
            int _soLuong = 0;

            if (textEdit6.EditValue != null) {
                int.TryParse(textEdit6.EditValue.ToString(), out _soLuong);
            }
            
            string url = string.Format("{0}?maHang={1}&&khachHang={2}&&chungLoai={3}&&dot={4}&&soLuong={5}&&malenh={6}&&dvsx={7}&&po={8}",
                URL + "DonHangTong/SearchDH", _maHang, _khachHang, _chungLoai,_dot,_soLuong,_maLenh,_dvsx,_po);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return tbl;
        }

        private void textEdit6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void LoadDonViSanXuat()
        {
            string url = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                _listDVSX = JsonConvert.DeserializeObject<List<DonViSanXuatEntity>>(json);

                searchLookUpEditDVSX.Properties.DataSource = _listDVSX;
                searchLookUpEditDVSX.Properties.DisplayMember = "TenDVSX";
                searchLookUpEditDVSX.Properties.ValueMember = "MaDVSX";
                searchLookUpEditDVSX.Properties.NullText = "Chọn DVSX";

                searchLookUpEditDVSX.Properties.View.Columns.Clear();
                searchLookUpEditDVSX.Properties.View.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
                {
                    FieldName = "TenDVSX",
                    Caption = "Đơn vị sản xuất",
                    Visible = true,
                    Width = 200
                });
            }
        }
    }
}
