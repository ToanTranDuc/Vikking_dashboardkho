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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDHChiaChuyen : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maDH = string.Empty, _maLenhSX = string.Empty,_LineGoc= string.Empty, _maGop = string.Empty;
        private HttpClientExtension _clientExtension;
        public frmCanDoiDHChiaChuyen(string maDH, string maLenhSX, string maGop)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._maDH = maDH;
            this._maLenhSX = maLenhSX;
            this._maGop = maGop;
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
                string url = $"{URL}ERPDonHangTong/Get?action=GetChiTiet&para={_maGop}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    txtDH.EditValue = null;
                    txtLenhSX.EditValue = null;
                    txtMaHang.EditValue = null;
                    txtChuyenGoc.EditValue = null;
                    txtDotSX.EditValue = null;
                    return;
                }

                txtDH.EditValue = tbl.Rows[0]["GopDH"];
                txtLenhSX.EditValue = tbl.Rows[0]["MaLenh"];
                txtMaHang.EditValue = tbl.Rows[0]["MaHang"];
                txtChuyenGoc.EditValue = tbl.Rows[0]["Name"];
                txtDotSX.EditValue = tbl.Rows[0]["DotSX"];
                _LineGoc = tbl.Rows[0]["Line"].ToString();
                return;

            }
            catch (Exception ex)
            {

            }
        }
       
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string Line = (searchLookUpEditLine.EditValue as string) ?? "";
            string lineGoc = txtChuyenGoc.EditValue.ToString();
            if (Line == "")
            {
                XtraMessageBox.Show("Vui lòng chọn chuyền cần chia", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
            string url = $"{URL}ERPDonHangTong/GetChuyen?action=PostChiaChuyen&para={_maGop}&para2={_LineGoc}&para3={Line}&para4=1&para5={_maLenhSX}&para6={GlobleData.UserName}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json.ToUpper() == "TRUE")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}