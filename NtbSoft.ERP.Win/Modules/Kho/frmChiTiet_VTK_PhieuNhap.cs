using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    public partial class frmChiTiet_VTK_PhieuNhap : DevExpress.XtraEditors.XtraForm
    {
        HttpClientExtension _clientExtension = new HttpClientExtension();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();
        string _maKho = GlobleData.MaKho;
        string URL = string.Empty; string dsMaKho = ""; string _oldValueQrCode = "";

        DataTable _tblKho = new DataTable();
        DataTable _tblPhieuNhap = new DataTable();
        DataTable _tblViTriKho = new DataTable();
        List<DataRow> _lstPNScan = new List<DataRow>();
        List<string> lstNodesChild = new List<string>();

        bool _statusChecked = true, _statusScanQR = false, _allowAdd = false, _allowEdit = false, _allowDelete = false;
        int _numberMaPhieu = 1;

        public frmChiTiet_VTK_PhieuNhap()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            LoadKho();
            LoadViTriKho(_maKho);
            LoadPhieuNhap();


        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {

            }
            if (!_allowEdit)
            {

            }


        }

        private void LoadKho()
        {
            string url = URL + ResourceURL.UrlUser + "/GetKhoUser";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblKho = JsonConvert.DeserializeObject<DataTable>(json);
        }

        private void LoadViTriKho(string MaKho)
        {
            dsMaKho = "";
            lstNodesChild = new List<string>();
            dsMaKho += string.Format("'{0}',", GetChildNode(MaKho));
            foreach (string makho in lstNodesChild)
                dsMaKho += string.Format("'{0}',", makho);
            dsMaKho = dsMaKho.Substring(0, dsMaKho.Length - 1);

            _tblViTriKho = new DataTable();
            string url = string.Format("{0}/ViTriKho/Get?action=GetAll&&parameter={1}", URL, dsMaKho);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblViTriKho = JsonConvert.DeserializeObject<DataTable>(json);

            if (_tblViTriKho.Rows.Count > 0 && _tblViTriKho.Columns.Count > 0) _tblViTriKho.Columns.Add("Chon", typeof(bool));
            List<DataRow> lstDR = _tblViTriKho.AsEnumerable().Where(x => x["MaVT"].ToString().Contains("00") == false).OrderBy(x => x["MaVT"].ToString()).ToList();
            foreach (DataRow o in lstDR)
            {
                List<string> arr = new List<string>();
                arr.Add(o["TenVT"].ToString());
                DataRow tang = _tblViTriKho.AsEnumerable().Where(x => x["MaVT"].ToString() == o["ParentMaVT"].ToString()).FirstOrDefault();
                arr.Add(tang["TenVT"].ToString());
                DataRow ke = _tblViTriKho.AsEnumerable().Where(x => x["MaVT"].ToString() == tang["ParentMaVT"].ToString()).FirstOrDefault();
                arr.Add(ke["TenVT"].ToString());
                o["TenVT"] = string.Format("{0} - {1} - {2}", arr[2], arr[1], arr[0]);
            }
            if (lstDR == null || lstDR.Count == 0)
                dgrViTriKho.DataSource = new DataTable();
            else
            {
                DataTable tblDisplay = lstDR.CopyToDataTable();
                dgrViTriKho.DataSource = tblDisplay;
            }
        }

        private void LoadPhieuNhap()
        {
            string url = string.Format("{0}/ViTriKho/Get?action=GetPhieuNhap&&parameter={1}", URL, dsMaKho);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblPhieuNhap = JsonConvert.DeserializeObject<DataTable>(json);
        }

        private DataRow GetMaViTri()
        {
            DataTable tbl = dgrViTriKho.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return null;
            DataRow dr = tbl.AsEnumerable().Where(x => (bool)x["Chon"] == true).FirstOrDefault();
            if (dr == null) return null;
            return dr;
        }

        private string GetChildNode(string maKho)
        {
            List<DataRow> lstChild = _tblKho.AsEnumerable().Where(x => x["ParentMaKho"].ToString() == maKho).ToList();
            if (lstChild != null)
            {
                foreach (DataRow drChild in lstChild)
                {
                    lstNodesChild.Add(GetChildNode(drChild["MaKho"].ToString()));
                }
            }
            return maKho;
        }

        private void Refresh()
        {
            LoadKho();
            LoadViTriKho(_maKho);
            LoadPhieuNhap();
            _lstPNScan = new List<DataRow>();
            dgrPhieuNhap.DataSource = new DataTable();
        }

        private void gridViewViTriKho_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            switch (e.Column.FieldName)
            {
                case "MaKho":
                    if (e.Value == null) return;
                    DataRow drKho = _tblKho.AsEnumerable().Where(x => x["MaKho"].ToString() == e.Value.ToString()).FirstOrDefault();
                    if (drKho == null) return;
                    e.DisplayText = drKho["TenKho"].ToString();
                    break;
                default:
                    break;
            }
        }

        private void repoColChon_CheckedChanged(object sender, EventArgs e)
        {
            DataTable tbl = dgrViTriKho.DataSource as DataTable;
            foreach (DataRow dr in tbl.Rows)
                dr["Chon"] = false;
            dgrViTriKho.RefreshDataSource();
            this.ActiveControl = dgrPhieuNhap;
            //DataTable tbl2 = dgrViTriKho.DataSource as DataTable;
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Refresh();
        }

        private void btnScanKho_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = txtQRCodeVTK;
            txtQRCodeVTK.Text = "";
        }

        private void gridViewPhieuNhap_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if(e.Column.FieldName == "MaCTVT" && e.Value != null)
            {
                List<string> lst = e.Value.ToString().Split('|').ToList();
                e.DisplayText = lst[lst.Count - 1];
            }
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drVT = GetMaViTri();
            if(drVT == null)
            {
                XtraMessageBox.Show("Chưa chọn vị trí để nhập");
                return;
            }
            List<ChiTietPhieuNhap_VTKEntity> lstSave = new List<ChiTietPhieuNhap_VTKEntity>();
            DateTime dt = DateTime.Now;
            foreach (DataRow dr in _lstPNScan)
            {
                ChiTietPhieuNhap_VTKEntity item = new ChiTietPhieuNhap_VTKEntity();
                item.ID = 0;
                item.MaVT = drVT["MaVT"].ToString();
                item.MaKho = drVT["MaKho"].ToString();
                item.MaPhieuNhap = dr["MaPhieu"].ToString();
                item.MaCTVT = dr["MaCTVT"].ToString();
                item.SoLuong = Convert.ToDouble(dr["SoLuong"].ToString());
                item.KhoiLuong = Convert.ToDouble(dr["KhoiLuong"].ToString());
                item.TrongLuong = Convert.ToDouble(dr["TrongLuong"].ToString());
                item.NgayNhapKho = dt;
                item.UserName = GlobleData.UserName;
                lstSave.Add(item);
            }    
            string url = string.Format("{0}/ViTriKho/PostPN_VTK?action=Insert&&parameter={1}", URL, "MaVT");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSave); }).Result;
            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                Refresh();
            }
            else XtraMessageBox.Show(result);
        }

        private void txtDisplayQrCode_Validating(object sender, CancelEventArgs e)
        {
            var baseEdit = (BaseEdit)sender;
            _oldValueQrCode = baseEdit.OldEditValue == null ? "" : baseEdit.OldEditValue.ToString();
        }

        private void txtQRCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtQRCode.Text == "") return;
                string find = "QrCode = '" + txtQRCode.Text + "'";
                DataRow dr = _tblPhieuNhap.Select(find).FirstOrDefault();
                if (dr == null) return;
                if (_lstPNScan.Contains(dr)) return;
                _lstPNScan.Add(dr);
                DataTable tblSource = _lstPNScan.CopyToDataTable();
                dgrPhieuNhap.DataSource = tblSource;
                txtQRCode.Text = "";
            }
            catch (Exception ex)
            {
                txtQRCode.Text = "";
            }
        }

        private void gridViewViTriKho_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            DataRow dr = gridViewViTriKho.GetDataRow(e.RowHandle);
            if (dr == null) return;
            if (Convert.ToDouble(dr["CBM"]) <= Convert.ToDouble(dr["CBMIsUse"]) && e.Column.FieldName == "TenVT")
                e.Appearance.BackColor = Color.FromArgb(255, 194, 181);
            if (Convert.ToDouble(dr["CBM"]) > Convert.ToDouble(dr["CBMIsUse"]) && e.Column.FieldName == "TenVT")
                e.Appearance.BackColor = Color.FromArgb(181, 255, 220);


        }

        private void txtQRCodeVTK_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtQRCodeVTK.Text == "") return;
                string find = "MaVT = '" + txtQRCodeVTK.Text + "'";
                DataRow dr = _tblViTriKho.Select(find).FirstOrDefault();
                if (dr == null) return;
                DataTable tblSource = dgrViTriKho.DataSource as DataTable;
                tblSource.Rows.Clear();
                dr["Chon"] = true;
                tblSource.Rows.Add(dr.ItemArray);
                dgrViTriKho.RefreshDataSource();
                txtQRCodeVTK.Text = "";
            }
            catch (Exception ex)
            {
                txtQRCodeVTK.Text = "";
            }
        }

        private void btnScanNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = txtQRCode;
            txtQRCode.Text = "";
        }

        int count = 0;
        private void timerResetTXTQrcode_Tick(object sender, EventArgs e)
        {
            if(txtQRCodeVTK.IsEditorActive)
            {
                if (_oldValueQrCode == txtQRCodeVTK.Text)
                {
                    count++;
                    if (count == 1)
                    {
                        _oldValueQrCode = "";
                        txtQRCodeVTK.Text = "";
                        txtQRCodeVTK.Refresh();
                        count = 0;
                    }
                }
                else _oldValueQrCode = txtQRCodeVTK.Text;
            }
            
            if(txtQRCode.IsEditorActive)
            {
                if (_oldValueQrCode == txtQRCode.Text)
                {
                    count++;
                    if (count == 1)
                    {
                        _oldValueQrCode = "";
                        txtQRCode.Text = "";
                        txtQRCode.Refresh();
                        count = 0;
                    }
                }
                else _oldValueQrCode = txtQRCode.Text;
            }
            
        }
    }
}
