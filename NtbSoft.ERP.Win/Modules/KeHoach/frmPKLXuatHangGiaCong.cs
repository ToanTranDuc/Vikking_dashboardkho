using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmPKLXuatHangGiaCong : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
         public static bool _checkSave = false;
        DataRow drRow;
        public frmPKLXuatHangGiaCong(DataRow dr)
        {
            InitializeComponent();
            _checkSave = false;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            dateEdit1.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";  // Định dạng ngày và giờ 24h, có giây
            dateEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;  // Sử dụng định dạng này cho hiển thị
            dateEdit1.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";  // Định dạng 24 giờ và giây cho hiển thị
            dateEdit1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dateEdit1.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";  // Định dạng 24 giờ và giây cho chỉnh sửa
            dateEdit1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.drRow = dr;
            // Đặt ngày giờ hiện tại cho DateEdit

            if (dr["ExportDate"] == null || dr["ExportDate"].ToString() == "")
                dateEdit1.DateTime = DateTime.Now;
            else
            {
                string url = $"{URL}SoTheoDoiXuatHangController/Get?Action=GetDateTime&Para1={dr["MaPKL_XH"]}&Para2=A&Para3=A&Para4=a";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                DateTime ngayxuat2 = Convert.ToDateTime(tbl.Rows[0]["ExportDate"]);
                dateEdit1.DateTime = ngayxuat2;
            }



            txt_pkl_xh.Text = drRow["MaPKL_XH"].ToString();
            txt_madh.Text = drRow["MaHang"].ToString(); 
            txt_cont.Text = drRow["Cont"].ToString();
            txt_khachhang.Text = drRow["KhachHang"].ToString();
            txt_po.Text = drRow["PO"].ToString();
            cbxtinhtrang.EditValue = drRow["TinhTrang"].ToString();
            txt_seal.Text = drRow["Seal"].ToString();
            txt_soxe.Text = drRow["SoXe"].ToString();
            txt_booking.Text = drRow["SoBooking"].ToString();
        }

        private void comboBoxEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxtinhtrang.EditValue.ToString() == "Đã xuất ")
            {

            }

        }
        private void Save()
        {
            DateTime ngayxuat = DateTime.Parse(dateEdit1.EditValue.ToString());
            string userName = GlobleData.UserName;
            List<PackageListXuatHangEntity> lst = new List<PackageListXuatHangEntity>();
            lst.Add(new PackageListXuatHangEntity()
            {
                MaPKL_XH = drRow["MaPKL_XH"].ToString(),
                MaDH = drRow["MaDH"].ToString(),
                POID = drRow["POID"].ToString(),
                MaSeal = txt_seal.Text.ToString(),
                KhachHang = drRow["KhachHang"].ToString(),
                IsExport = cbxtinhtrang.EditValue.ToString().Trim() != "Đã xuất" ? 0 : 3,
                Soxe = txt_soxe.Text.ToString(),
                SoBooking = txt_booking.Text.ToString(),
                MaCont = drRow["MaCont"].ToString(),
                Cont = txt_cont.Text.ToString(),
                ExportDate = ngayxuat,
                NhanVienKiem = userName
            }); ;
            var josn = JsonConvert.SerializeObject(lst);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(josn);
            string url = $"{URL}SoTheoDoiXuatHangController/Post";

            //return;
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
            if (result.ToLower() == "true")
            {
                _checkSave = true;
                clsWaitForm.ShowSuccessForm(this, 2000);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}