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
    public partial class frmCanDoiDHDoiChuyen : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maDH = string.Empty, _maLenhSX = string.Empty, _LineGoc = string.Empty;
        private HttpClientExtension _clientExtension;
        public frmCanDoiDHDoiChuyen(string maDH, string maLenhSX)
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

            searchLookUpEditLineChange.Properties.ValueMember = "Line";
            searchLookUpEditLineChange.Properties.DisplayMember = "Name";
            searchLookUpEditLineChange.Properties.NullValuePrompt = "Chọn chuyền";

            GetLine();
            GetLineChia();
            GetChiTietDH();
        }

       

        private void GetLine()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetLineDSChia&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditLineChange.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditLineChange.Properties.DataSource = tbl;


            }
            catch (Exception ex)
            {

            }
        }
        private void GetLineChia()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetLineChia&para={_maDH}&para5={_maLenhSX}";
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
            string Line = (searchLookUpEditLineChange.EditValue as string) ?? "";
            string Linechange = (searchLookUpEditLine.EditValue as string) ?? "";
            if (Line == "" || Linechange == "")
            {
                XtraMessageBox.Show("Vui lòng chọn chuyền cần đổi hoặc chuyền đổi", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string url = $"{URL}ERPDonHangTong/GetChuyen?action=ChangeChuyen&para={_maDH}&para2={Line}&para3={Linechange}&para5={_maLenhSX}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json.ToUpper() == "TRUE")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}