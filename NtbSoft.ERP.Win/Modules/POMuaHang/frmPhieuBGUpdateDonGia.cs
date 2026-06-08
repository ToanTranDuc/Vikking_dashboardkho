using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmPhieuBGUpdateDonGia : DevExpress.XtraEditors.XtraForm
    {
        
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataRow _rowFocused = null;string _tenPhieu = string.Empty;
        string _maPhieu = string.Empty;
        public frmPhieuBGUpdateDonGia(DataRow rowFocused,string MaPhieu,string TenPhieu)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CreateTableSaveBaoGiaChiTiet();
            _rowFocused = rowFocused;
            _maPhieu = MaPhieu;
            _tenPhieu = TenPhieu;
        }
        protected override void OnLoad(EventArgs e)
        {
            SetLayoutPhieuBG();
        }

        private void SetLayoutPhieuBG()
        {
            if(_rowFocused != null)
            {
                spinDG.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                spinDG.Properties.DisplayFormat.FormatString = "n2";
                spinDG.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                spinDG.Properties.EditFormat.FormatString = "n2";
                spinDG.Properties.Mask.UseMaskAsDisplayFormat = true;
                spinDG.Properties.Mask.EditMask = "n2";

                // spinDGCu
                spinDGCu.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                spinDGCu.Properties.DisplayFormat.FormatString = "n2";
                spinDGCu.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                spinDGCu.Properties.EditFormat.FormatString = "n2";
                spinDGCu.Properties.Mask.UseMaskAsDisplayFormat = true;
                spinDGCu.Properties.Mask.EditMask = "n2";

                double.TryParse(_rowFocused["DonGia"]?.ToString(), out double DonGia);
                txtItemCode.Text = _rowFocused["ItemCode"]?.ToString();
                txtPhieuBG.Text = _tenPhieu;
                txtMoTa.Text = _rowFocused["MoTa"]?.ToString();
                txtKhoSize.Text = _rowFocused["KhoVai"]?.ToString();
                txtMau.Text = _rowFocused["MauVT"]?.ToString();
                txtTenCL.Text = _rowFocused["TenCL"]?.ToString();
                spinDG.EditValue = 0;
                spinDGCu.EditValue = DonGia;
                textEdit1.EditValue = _rowFocused["DVTienTeVT"]?.ToString();
            }
        }
        private DataTable CreateTableSaveBaoGiaChiTiet()
        {

            DataTable dt = new DataTable("BaoGia_ChiTiet");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("NhomCLCC", typeof(string));
            dt.Columns.Add("MaNCC", typeof(string));
            dt.Columns.Add("ChungLoaiCC", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoSizeID", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("Thue", typeof(string));
            dt.Columns.Add("ChietKhau", typeof(double));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NgaySua", typeof(string));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("ThoiGianSua", typeof(string));
            dt.Columns.Add("DonViTienTe", typeof(string));
            dt.Columns.Add("Leadtime", typeof(string));
            return dt;
        }
        private void btnSubmit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            try
            {

                string url = $"{URL}PhieuBaoGia/POST?action=UpdateDonGia";
                DataTable tblChiTiet = CreateTableSaveBaoGiaChiTiet();
                double.TryParse(spinDG.EditValue?.ToString(), out double DonGia);
                if(DonGia <= 0)
                {
                    XtraMessageBox.Show("Vui lòng nhập đơn giá lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataRow newRow = tblChiTiet.NewRow();
                newRow["ID"] =0;
                newRow["MaPhieuBG"] =_maPhieu;
                newRow["NhomCLCC"] = "";
                newRow["MaNCC"] = "";
                newRow["ChungLoaiCC"] = _rowFocused["ChungLoaiCC"]?.ToString() ?? "";
                newRow["ItemCode"] = _rowFocused["ItemCode"]?.ToString();
                newRow["MaVTID"] = _rowFocused["MaVTID"]?.ToString();
                newRow["MauVTID"] = _rowFocused["MauVTID"]?.ToString();
                newRow["KhoSizeID"] = _rowFocused["KhoSizeID"]?.ToString();
                newRow["MaDVVT"] = _rowFocused["MaDVVT"]?.ToString();
                newRow["DonGia"] = DonGia;                               
                newRow["Thue"] = 0;
                newRow["ChietKhau"] = 0;                 
                newRow["GhiChu"] =txtGhiChu.Text;
                newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                newRow["NguoiSua"] = GlobleData.UserName;
                newRow["ThoiGianSua"] = clsForrmatUtils.ParseMinutesFromTime(DateTime.Now.TimeOfDay.ToString());
                newRow["DonViTienTe"] = _rowFocused["DVTienTeVT"]?.ToString();
                tblChiTiet.Rows.Add(newRow);
                string jsonData = JsonConvert.SerializeObject(tblChiTiet);
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (result.Trim().ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;

                }
                else
                {
                    XtraMessageBox.Show("Lỗi khi lưu:\n" + result, "Lỗi lưu dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}