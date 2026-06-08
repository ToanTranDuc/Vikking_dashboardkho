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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDHXoaChuyen : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maDH = string.Empty, _maLenhSX = string.Empty, _LineGoc = string.Empty;
        private HttpClientExtension _clientExtension;
        public frmCanDoiDHXoaChuyen(string maDH, string maLenhSX)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._maDH = maDH;
            this._maLenhSX = maLenhSX;
            InIt();
        }



        private void InIt()
        {
            searchLookUpEditLine.Properties.ValueMember = "Line";
            searchLookUpEditLine.Properties.DisplayMember = "Name";
            searchLookUpEditLine.Properties.NullValuePrompt = "Chọn chuyền";

            GetLine();
            GetChiTietDH();
        }
        private void GetLine()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetLineDelete&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditLine.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditLine.Properties.DataSource = tbl;


            }
            catch (Exception ex)
            {

            }
        }
        private void GetChiTietDH()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetChiTiet&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    txtDH.EditValue = null;
                    txtLenhSX.EditValue = null;
                    txtMaHang.EditValue = null;
                    txtDotSX.EditValue = null;
                    return;
                }

                txtDH.EditValue = tbl.Rows[0]["MaDH"];
                txtLenhSX.EditValue = tbl.Rows[0]["MaLenh"];
                txtMaHang.EditValue = tbl.Rows[0]["MaHang"];

                txtDotSX.EditValue = tbl.Rows[0]["DotSX"];
                _LineGoc = tbl.Rows[0]["Line"].ToString();
                return;

            }
            catch (Exception ex)
            {

            }
        }
        private void btLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CheckSoLuongChuyen();
        }
        private void CheckSoLuongChuyen()
        {
            string Line = (searchLookUpEditLine.EditValue as string) ?? "";
            if (Line == "")
            {
                XtraMessageBox.Show("Vui lòng chọn chuyền cần xóa", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = $"{URL}ERPDonHangTong/Get?action=GetCheckSoLuongChuyenDelete&para={_maDH}&para5={_maLenhSX}&para2={Line}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (Convert.ToInt32(tbl.Rows[0]["SoLuong"]) > 0)
            {
                DialogResult result = XtraMessageBox.Show(
                        $"Chuyền {searchLookUpEditLine.Text} đã có số lượng!\n Nếu bạn đồng ý xóa thì tôi sẽ trả số lượng về chuyền gốc?",   // Nội dung
                        "Cảnh báo",                        // Tiêu đề
                        MessageBoxButtons.YesNo,           // Nút Yes / No
                        MessageBoxIcon.Question            // Biểu tượng dấu hỏi
                   );
                if (result == DialogResult.Yes)
                {
                    DeleteChuyen();

                }

            }
            else
            {
                DeleteChuyen();
            }
        }
        private void DeleteChuyen()
        {
            string Line = (searchLookUpEditLine.EditValue as string) ?? "";
            string url = $"{URL}ERPDonHangTong/GetChuyen?action=DeleteChuyen&para={_maDH}&para2={Line}&para5={_maLenhSX}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (json.ToUpper() == "TRUE")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}