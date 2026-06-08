using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmChiTietThungTonBarcodeV3 : DevExpress.XtraEditors.XtraForm
    {
        private DataRow drGetData = null;
        System.Configuration.AppSettingsReader settingsReader =
                                         new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        private string MaPKL_XH = "", Cont = "", MaDH = "", POID = "", MaPKL = "", MaLenh = "", SttThungMin = "", SttThungMax = "";
        private bool flagInitA = false, FlagKK = false;
        private bool isStartFillBarcode = false;
        private int RowHandle = 0;
        private DataTable dtDataT = new DataTable();
        string action = "";

        public frmChiTietThungTonBarcodeV3(DataRow dr, bool flagAllowEdit = true, bool flagInit = false, bool flagScan = false, bool flagKK = false,bool flagSMS = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            drGetData = dr;
            MaPKL_XH = "All";
            Cont = "All";
            MaDH = drGetData["MaDH"].ToString();
            POID = (flagSMS || drGetData["POID_G"].ToString() != "") ? "All" : drGetData["POID"].ToString();
            MaPKL = drGetData["MaPKL"].ToString();
            MaLenh = "A";
            SttThungMin = drGetData["SttThungMin"].ToString();
            SttThungMax = drGetData["SttThung"].ToString();
            flagInitA = flagInit;
            FlagKK = flagKK;
            action = (flagSMS || drGetData["POID_G"].ToString() != "") ? "GetDSThungPKL_SMS" : "GetDSThungPKL";
            if (flagInit)
            {
                MaPKL_XH = "All";
                Cont = "All";
                txtScanQR.Enabled = false;
                btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            }
            LoadData();

            if (!flagAllowEdit)
            {
                txtScanQR.Enabled = false;
                grvTong.OptionsBehavior.Editable = false;
                btnSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            if (flagScan)
            {
                txtScanQR.Enabled = true;
                grvTong.OptionsBehavior.Editable = false;
                btnSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
        }

        private void txtScanQR_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtScanQR.Text == "") return;
                string find = "Barcode = '" + txtScanQR.Text + "'";
                var dtCheckT = dtDataT.Select(find);
                if (dtCheckT.Count() <= 0)
                {
                    txtScanQR.Text = "";
                    txtScanQR.Refresh();
                    clsWaitForm.ShowWaningFormCustom(this, 1000, "Barcode không trùng khớp với size đang chọn. Vui lòng kiểm tra lại");
                    return;
                }
                var dtCheck = dtCheckT.CopyToDataTable();
                var drCheck = dtCheck.AsEnumerable().Where(x => Convert.ToInt16(x["STDaQuet"]) < Convert.ToInt16(x["SLThung"])).FirstOrDefault();
                if (drCheck is null)
                {
                    txtScanQR.Text = "";
                    txtScanQR.Refresh();
                    clsWaitForm.ShowWaningFormCustomV2(this, 1000, "Số lượng > SLKH.");
                    return;
                }
                foreach (DataRow dr in dtDataT.Rows)
                {
                    if (dr["Barcode"].ToString() == drCheck["Barcode"].ToString())
                    {
                        dr["Barcode"] = txtScanQR.Text;
                        dr["STDaQuet"] = 1;
                        dr["IsScan"] = true;
                        dgcTong.DataSource = dtDataT;
                        var index = dtDataT.Rows.IndexOf(dr);
                        grvTong.FocusedRowHandle = index;
                        grvTong.RefreshData();
                        break;
                    }
                }
                txtScanQR.Text = "";
                txtScanQR.Refresh();
            }
        }

        private void grvTong_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsDaQuet = (bool)grvTong.GetRowCellValue(e.RowHandle, "IsScan");
            if (IsDaQuet) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn xóa tát cả barcode không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                var dtSource = dgcTong.DataSource as DataTable;
                foreach (DataRow dr in dtSource.Rows)
                {
                    dr["Barcode"] = "";
                    dr["Barcode2"] = "";
                }
            }
        }

        private void btnPrintBarcode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dtSource = dgcTong.DataSource as DataTable;
            var dtCheck = dtSource.Clone();
            int[] selectedRows = grvTong.GetSelectedRows();
            if (selectedRows.Length == 0) {
                MessageBox.Show("Vui lòng chọn barcode để in!");
                return;
            }
            
            foreach (int rowHandle in selectedRows)
            {
                DataRow dr = grvTong.GetDataRow(rowHandle);
                DataRow drNew = dtCheck.NewRow();
                KHDongThungLib.CopyDataRow(dr, drNew);
                //var drNew = dtCheck.NewRow();               
                dtCheck.Rows.Add(drNew);
            }
            var lstQRCode = new List<QRCodeInfoEntity>();
            lstQRCode = dtCheck.AsEnumerable().Select(x => new QRCodeInfoEntity
            {
                Style = x["TenHang"].ToString(),
                PO = x["PO"].ToString(),
                Season = x["TenCL"].ToString(),
                Color = x["TenMau"].ToString(),
                Size = x["Size"].ToString(),
                PCB = x["PCB"].ToString(),
                CTN = x["SttThungDisplay"].ToString(),
                QRCode = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(x["Barcode"].ToString()))
            }).ToList();
            frmBarcodeView_TK frm = new frmBarcodeView_TK(lstQRCode,"",FlagKK);
            frm.ShowDialog();
        }

        private void LoadData()
        {
            string url = string.Format("{0}", URL + $"ScanBarcode/Get?Action={action}&Para1={MaPKL_XH}&Para2={Cont}&Para3={MaDH}&Para4={POID}" +
                                                    $"&Para5={MaPKL}&Para6={SttThungMin}&Para7={SttThungMax}&Para8={MaLenh}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtDataT = JsonConvert.DeserializeObject<DataTable>(json);
            dgcTong.DataSource = dtDataT;
        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }
        private void Save()
        {
            this.ActiveControl = txtScanQR;
            DataTable dtSource = dgcTong.DataSource as DataTable;
            var checkRowA = dtSource.AsEnumerable().Where(x => x["Barcode"].ToString() != "").GroupBy(row => row["Barcode"].ToString())
                             .Where(ageGroup => ageGroup.Count() > 1).ToList();
            if (checkRowA.Count > 0)
            {
                string message = "";
                foreach (var item in checkRowA)
                {
                    message += item.Key.Trim() + Environment.NewLine;
                }
                MessageBox.Show($"Đã tồn tại barcode trùng cột Barcode. Vui lòng kiểm tra lại! \r\n{message}");
                return;
            }
            var checkRowB = dtSource.AsEnumerable().Where(x => x["Barcode2"].ToString() != "").GroupBy(row => row["Barcode2"].ToString())
                             .Where(ageGroup => ageGroup.Count() > 1).ToList();
            if (checkRowB.Count > 0)
            {
                string message = "";
                foreach (var item in checkRowB)
                {
                    message += item.Key.Trim() + Environment.NewLine;
                }
                MessageBox.Show($"Đã tồn tại barcode trùng cột Barcode2. Vui lòng kiểm tra lại! \r\n{message}");
                return;
            }

            if (dtSource.Rows.Count == 0) return;
            var dtTemp = flagInitA ? dtSource.AsEnumerable().Where(x => x["Barcode"].ToString() != "") : dtSource.AsEnumerable().Where(x => Convert.ToInt16(x["STDaQuet"]) > 0);
            if (dtTemp.Count() == 0) return;
            DataTable dtSave = KHDongThungLib.CreateTblSave();

            foreach (DataRow dr in dtTemp)
            {
                DataRow drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaDH"] = dr["MaDH"];
                drNew["POID"] = dr["POID"];
                drNew["MaPKL"] = dr["MaPKL"];
                drNew["QRCode"] = dr["Barcode"];
                drNew["KyHieu"] = dr["Barcode2"];
                drNew["SttThung"] = dr["SttThung"];
                drNew["IsScan"] = dr["IsScan"];
                drNew["Destination"] = dr["GhiChu"];
                dtSave.Rows.Add(drNew);
            }
            string action = "";
            if (flagInitA) action = "Update_InitBarcode";
            else action = "UpdateScan_CT";
            string url = string.Format("{0}", URL + $"ScanBarcode/UpdateScanV2?action={action}");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 1500);
                this.Close();
            }
        }
        private void grvTong_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (!isStartFillBarcode)
            {
                isStartFillBarcode = true;
            }
            RowHandle = e.RowHandle;
        }
        private void dgcTong_ProcessGridKey(object sender, KeyEventArgs e)
        {

            if (e.Control && e.KeyCode == Keys.V)
            {
                string _fieldName = grvTong.FocusedColumn.FieldName;
                var lstAccept = new List<string>() { "Barcode", "Barcode2" };
                if (!lstAccept.Contains(_fieldName)) return;
                DataTable dt = dgcTong.DataSource as DataTable;
                KHDongThungLib.CopyPasteFor1Col(grvTong, dt.Rows.Count, _fieldName);

            }
            if (e.KeyCode == Keys.Enter && isStartFillBarcode)
            {
                DataTable dt = dgcTong.DataSource as DataTable;
                //grvTong.SetRowCellValue(RowHandle, "STDaQuet", "1");
                grvTong.FocusedRowHandle = RowHandle + 1;
                //if(grvTong.FocusedColumn.FieldName == "Barcode")
                var fieldnameFocus = grvTong.FocusedColumn.FieldName;
                CheckTrung(fieldnameFocus);
                if (RowHandle >= dt.Rows.Count - 1) RowHandle = -1;
                isStartFillBarcode = false;
            }
        }
        private void CheckTrung(string fieldname)
        {
            bool flagCheckTwoBarcodeDif = false;
            DataTable dt = dgcTong.DataSource as DataTable;

            var valueBarCode = grvTong.GetRowCellValue(RowHandle, "Barcode").ToString();
            var valueBarCode2 = grvTong.GetRowCellValue(RowHandle, "Barcode2").ToString();
            if (fieldname == "Barcode" && valueBarCode2 != "" && valueBarCode != valueBarCode2)
            {
                flagCheckTwoBarcodeDif = true;
            }
            else if (fieldname == "Barcode2" && valueBarCode != "" && valueBarCode != valueBarCode2) flagCheckTwoBarcodeDif = true;

            var checkRow = dt.AsEnumerable().Where(x => x[fieldname].ToString() != "").GroupBy(row => row[fieldname].ToString())
                            .Where(ageGroup => ageGroup.Count() > 1).ToList();
            if (checkRow.Count() > 0 || flagCheckTwoBarcodeDif)
            {
                grvTong.FocusedRowHandle = RowHandle;
                grvTong.SetRowCellValue(RowHandle, fieldname, "");
                grvTong.SetRowCellValue(RowHandle, "STDaQuet", "0");
                System.Media.SystemSounds.Beep.Play();
                if (checkRow.Count() > 0) clsWaitForm.ShowWaningFormCustomV3(this, 1000, "");
                else clsWaitForm.ShowWaningFormCustomV4(this, 1000, "");
            }
        }
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
