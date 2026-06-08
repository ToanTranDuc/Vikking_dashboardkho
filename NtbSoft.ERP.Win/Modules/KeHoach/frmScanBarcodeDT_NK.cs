using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmScanBarcodeDT_NK : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                          new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        private object keylock = new object();
        private DataTable dtDonHang = new DataTable();
        private DataTable dtData = new DataTable();
        private DataTable dtCTThung = new DataTable();

        private ConcurrentQueue<dynamic> lstSaveBarCode = new ConcurrentQueue<dynamic>();
        private List<string> lstBarcodeGop = new List<string>();
        private string str_BarcodeGop = "";
        System.Timers.Timer tmrSaveData = new System.Timers.Timer();
        private int SLGop = 1;
        private string BarcodeOld = "";
        private string cbxDT_NK = "";
        public frmScanBarcodeDT_NK(string option)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tmrSaveData.Interval = 500;
            tmrSaveData.AutoReset = false;
            tmrSaveData.Elapsed += TmrSaveData_Elapsed;
            tmrSaveData.Start();
            cbxDT_NK = option;
            Init();
            GetDH();
        }


        private void Init()
        {
            searchLookUpEdit_MaDH.Properties.DisplayMember = "valueDisplay";
            searchLookUpEdit_MaDH.Properties.ValueMember = "MaGop";
            searchLookUpEdit_MaDH.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_DauSize.Properties.DisplayMember = "DauSize";
            searchLookUpEdit_DauSize.Properties.ValueMember = "DauSizeID";
            searchLookUpEdit_DauSize.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_Mau.Properties.DisplayMember = "TenMau";
            searchLookUpEdit_Mau.Properties.ValueMember = "ColorID";
            searchLookUpEdit_Mau.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_Size.Properties.DisplayMember = "Size";
            searchLookUpEdit_Size.Properties.ValueMember = "SizeID";
            searchLookUpEdit_Size.Properties.NullText = "[Chọn giá trị]";
        }
        private void GetDH()
        {
            try
            {
                string url = string.Format("{0}", URL + $"ScanBarcodeDT_NK/Get?Action=GetDH");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                var ds = JsonConvert.DeserializeObject<DataSet>(json);
                dtDonHang = ds.Tables[0];
                var lstDonHang = dtDonHang.AsEnumerable().Select(x => new
                {
                    MaGop = x["MaGop"].ToString(),
                    valueDisplay = x["valueDisplay"].ToString(),
                    GopDH = x["GopDH"].ToString(),
                    TenHang = x["TenHang"].ToString(),
                    KhachHang = x["KhachHang"].ToString(),
                    SoLuong = x["SoLuong"].ToString()
                }).Distinct();
                string jsonDH = JsonConvert.SerializeObject(lstDonHang);
                DataTable _dtDH = JsonConvert.DeserializeObject<DataTable>(jsonDH);
                searchLookUpEdit_MaDH.Properties.DataSource = _dtDH;
                searchLookUpEdit_MaDH.EditValue = null;
            }
            catch
            {

            }

        }
        private void BindingPO(string POIDBarcode = "")
        {
            var lstPO = dtDonHang.AsEnumerable().Where(x => x["MaGop"].ToString() == searchLookUpEdit_MaDH.EditValue.ToString())
                                                .Select(x => new { PO = x["PO"].ToString(), POID = x["POID"].ToString() });
            string jsonPO = JsonConvert.SerializeObject(lstPO);
            DataTable _dtPO = JsonConvert.DeserializeObject<DataTable>(jsonPO);
            searchLookUpEdit_PO.Properties.DataSource = _dtPO;
            searchLookUpEdit_PO.EditValue = null;
            if (POIDBarcode != "") searchLookUpEdit_PO.EditValue = POIDBarcode;
        }
        private void LoadData()
        {
            try
            {
                string url = string.Format("{0}", URL + $"ScanBarcodeDT_NK/Get?Action=GetCTDongThung&Para1={searchLookUpEdit_MaDH.EditValue.ToString()}&Para2={searchLookUpEdit_PO.EditValue.ToString()}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                var ds = JsonConvert.DeserializeObject<DataSet>(json);
                dtData = ds.Tables[0];
                dtCTThung = ds.Tables.Count > 1 ? ds.Tables[1] : new DataTable();
                dgcTong.DataSource = dtData;
            }
            catch
            {

            }
        }
        private void BindingInSeam()
        {
            var lstDauSize = dtData.AsEnumerable().Where(x => x["SLGop"].ToString() != "1").Select(x => new { DauSizeID = x["DauSizeID"].ToString(), DauSize = x["DauSize"].ToString() });
            string json = JsonConvert.SerializeObject(lstDauSize);
            DataTable _dtDauSize = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_DauSize.Properties.DataSource = _dtDauSize;
            searchLookUpEdit_DauSize.EditValue = null;
            if (_dtDauSize.Rows.Count != 0)
                searchLookUpEdit_DauSize.EditValue = _dtDauSize.Rows[0]["DauSizeID"];
        }
        private void BindingMau()
        {
            var lstMau = dtData.AsEnumerable().Where(x => x["DauSizeID"].ToString() == searchLookUpEdit_DauSize.EditValue.ToString() && x["SLGop"].ToString() != "1")
                                                    .Select(x => new { ColorID = x["ColorID"].ToString(), TenMau = x["TenMau"].ToString() });
            string json = JsonConvert.SerializeObject(lstMau);
            DataTable _dtMau = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Mau.Properties.DataSource = _dtMau;
            searchLookUpEdit_Mau.EditValue = null;
            if (_dtMau.Rows.Count != 0)
                searchLookUpEdit_Mau.EditValue = _dtMau.Rows[0]["ColorID"];


        }
        private void BindingSize()
        {
            var lstSize = dtData.AsEnumerable().Where(x => x["DauSizeID"].ToString() == searchLookUpEdit_DauSize.EditValue.ToString()
            && x["ColorID"].ToString() == searchLookUpEdit_Mau.EditValue.ToString()
            && x["SLGop"].ToString() != "1")
                                                    .Select(x => new { SizeID = x["SizeID"].ToString(), Size = x["Size"].ToString() });
            string json = JsonConvert.SerializeObject(lstSize);
            DataTable _dtSize = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Size.Properties.DataSource = _dtSize;
            searchLookUpEdit_Size.EditValue = null;
            if (_dtSize.Rows.Count != 0)
                searchLookUpEdit_Size.EditValue = _dtSize.Rows[0]["SizeID"];


        }
        private bool GetDH_Barcode(string barcode)
        {
            try
            {
                string url = string.Format("{0}", URL + $"ScanBarcodeDT_NK/Get?Action=GetDH_Barcode&Para1={Uri.EscapeDataString(barcode)}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                var ds = JsonConvert.DeserializeObject<DataSet>(json);
                var dtDonHangT = ds.Tables[0];
                if (dtDonHangT.Rows.Count > 0)
                {
                    searchLookUpEdit_MaDH.EditValue = dtDonHangT.Rows[0]["MaGop"];
                    BindingPO(dtDonHangT.Rows[0]["POID"].ToString());
                    return true;
                }
                return false;


            }
            catch
            {
                return false;
            }

        }
        private void CheckBarCode(string Barcode)
        {
            try
            {
                var drCheck = dtData.AsEnumerable().Where(x => x["Barcode"].ToString().Split(',').OrderBy(y => y).SequenceEqual(Barcode.Split(',').OrderBy(y => y))).FirstOrDefault();
                DataRow drCheck2 = null;
                if (drCheck is null)
                {
                    lock (keylock)
                    {
                        drCheck2 = dtCTThung.AsEnumerable().Where(x => x["Barcode"].ToString().Split(',').OrderBy(y => y).SequenceEqual(Barcode.Split(',').OrderBy(y => y))).FirstOrDefault();
                        if (drCheck2 is null)
                        {
                            MessageBox.Show("Barcode không trùng khớp trong list đã khai báo!", "Thông báo");
                            return;
                        }
                        if (drCheck2["IsDongThung"].ToString() == "1" && cbxDT_NK == "DT")
                        {
                            MessageBox.Show("Thùng đã được đóng", "Thông báo");
                            return;
                        }
                        if (drCheck2["IsNhapKho"].ToString() == "1" && cbxDT_NK == "NK")
                        {
                            MessageBox.Show("Thùng đã nhập kho", "Thông báo");
                            return;
                        }
                    }
                }
                if (drCheck == null)
                {
                    drCheck = dtData.AsEnumerable().Where(x => x["MaDH"].ToString() == drCheck2["MaDH"].ToString()
                                                                && x["POID"].ToString() == drCheck2["POID"].ToString()
                                                                && x["DauSizeID"].ToString() == drCheck2["DauSizeID"].ToString()
                                                                && x["ColorID"].ToString() == drCheck2["ColorID"].ToString()
                                                                && x["SizeID"].ToString() == drCheck2["SizeID"].ToString()).FirstOrDefault();
                    drCheck["BarcodeTemp"] = drCheck2["Barcode"];
                }
                DataRow drThung = null;
                DataRow item = drCheck;
                if (cbxDT_NK == "DT")
                {
                    drThung = dtCTThung.AsEnumerable().Where(x => x["MaDH"].ToString() == item["MaDH"].ToString()
                                                              && x["POID"].ToString() == item["POID"].ToString()
                                                              && x["DauSizeID"].ToString() == item["DauSizeID"].ToString()
                                                              && x["ColorID"].ToString() == item["ColorID"].ToString()
                                                              && x["SizeID"].ToString() == item["SizeID"].ToString()
                                                              && x["IsThungLe"].ToString() == item["IsThungLe"].ToString()
                                                              && x["Barcode"].ToString() == (item["BarcodeTemp"].ToString() == "" ? x["Barcode"].ToString() : item["BarcodeTemp"].ToString())
                                                              && x["IsDongThung"].ToString() == "0").Take(1).FirstOrDefault();
                }
                else
                {
                    drThung = dtCTThung.AsEnumerable().Where(x => x["MaDH"].ToString() == item["MaDH"].ToString()
                                                           && x["POID"].ToString() == item["POID"].ToString()
                                                           && x["DauSizeID"].ToString() == item["DauSizeID"].ToString()
                                                           && x["ColorID"].ToString() == item["ColorID"].ToString()
                                                           && x["SizeID"].ToString() == item["SizeID"].ToString()
                                                           && x["IsThungLe"].ToString() == item["IsThungLe"].ToString()
                                                           && x["Barcode"].ToString() == (item["BarcodeTemp"].ToString() == "" ? x["Barcode"].ToString() : item["BarcodeTemp"].ToString())
                                                           && x["IsDongThung"].ToString() == "1"
                                                           && x["IsNhapKho"].ToString() == "0").Take(1).FirstOrDefault();
                }
               
                if (drThung != null)
                {
                    DataRow drRowQueue = dtCTThung.NewRow();
                    KHDongThungLib.CopyDataRow(drThung, drRowQueue);
                    if (cbxDT_NK == "DT") drThung["IsDongThung"] = 1;
                    else drThung["IsNhapKho"] = 1;
                    Console.WriteLine(drThung["Barcode"].ToString());
                    lstSaveBarCode.Enqueue(drRowQueue);
                }
                  
                var SLThung = Convert.ToInt16(drCheck["SLThung"]);
                var STDaDong = Convert.ToInt16(drCheck["STDaDong"]);
                var STDaNhap = Convert.ToInt16(drCheck["STDaNhap"]);


                if (cbxDT_NK == "DT")
                {
                    if (STDaDong >= SLThung)
                    {
                        MessageBox.Show("SL đóng > SLKH", "Thông báo");
                        BigndingInfoScan(drCheck);
                        return;
                    }
                    if (drThung != null) drCheck["STDaDong"] = STDaDong + 1;
                }
                else
                {
                    if (STDaNhap >= STDaDong)
                    {
                        MessageBox.Show("SL nhập > SL đóng", "Thông báo");
                        BigndingInfoScan(drCheck);
                        return;
                    }
                    if (drThung != null) drCheck["STDaNhap"] = STDaNhap + 1;
                }
                dgcTong.DataSource = dtData;
                BigndingInfoScan(drCheck);
            }
            catch (Exception ex)
            {

            }

        }
        private void BigndingInfoScan(DataRow drCheck)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    BingdingText(drCheck);
                });
            }
            else
            {
                BingdingText(drCheck);
            }

        }
        private void BingdingText(DataRow drCheck)
        {
            txtMaDH.Text = drCheck["GopDH"].ToString();
            txtPO.Text = drCheck["PO"].ToString();
            txtPO.Text = drCheck["PO"].ToString();
            txtMaHang.Text = drCheck["TenHang"].ToString();
            txtDauSize.Text = drCheck["DauSize"].ToString();
            txtMau.Text = drCheck["TenMau"].ToString();
            txtSize.Text = drCheck["Size"].ToString();
            lblSTDaQuet.Text = (cbxDT_NK == "DT" ? drCheck["STDaDong"].ToString() : drCheck["STDaNhap"].ToString()) + "/"
                               + (cbxDT_NK == "DT" ? drCheck["SLThung"].ToString() : drCheck["STDaDong"].ToString());
        }
        private void TmrSaveData_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrSaveData.Stop();
                if (lstSaveBarCode.Count == 0)
                {
                    tmrSaveData.Start();
                    return;
                }
                string action = "";
                DataTable dtSave = CreatetblSave();
                while (lstSaveBarCode.TryDequeue(out dynamic drThung))
                {
                    lock (keylock)
                    { 
                        var dr = dtSave.NewRow();
                        dr["MaDH"] = drThung["MaDH"].ToString();
                        dr["POID"] = drThung["POID"].ToString();
                        dr["MaPKL"] = drThung["MaPKL"].ToString();
                        dr["SttThung"] = drThung["SttThung"].ToString();
                        dtSave.Rows.Add(dr);                      
                        action = cbxDT_NK == "DT" ? "PostDT" : "PostNK";
                    }
                }
                string url = string.Format("{0}", URL + $"ScanBarcodeDT_NK/Post?action={action}");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                tmrSaveData.Start();
            }
            catch (Exception ex)
            {
                tmrSaveData.Start();
            }


        }
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEdit_MaDH.EditValue is null || searchLookUpEdit_PO.EditValue is null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Thông báo");
                return;
            }
           
            LoadData();
        }
        private void searchLookUpEdit_MaDH_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MaDH.EditValue is null) return;
            BindingPO();
            ClearData();
        }
        private void searchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_PO.EditValue is null) return;
            LoadData();
        }
        private void txtScanQR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (cbxDT_NK == "")
                {
                    MessageBox.Show("Vui lòng chọn module để quét!");
                    txtScanQR1.Text = "";
                    return;
                }
                if (txtScanQR1.Text == "") return;
                if (searchLookUpEdit_MaDH.EditValue is null)
                {
                    if (!GetDH_Barcode(txtScanQR1.Text)) return;
                }
                if (SLGop > 1)
                {
                    if (lstBarcodeGop.Contains(txtScanQR1.Text))
                    {
                        txtScanQR1.Text = BarcodeOld;
                        txtScanQR1.SelectionStart = txtScanQR1.Text.Length;
                        txtScanQR1.SelectionLength = 0;
                        return;
                    }
                    txtScanQR1.Text = txtScanQR1.Text + ",";
                    txtScanQR1.SelectionStart = txtScanQR1.Text.Length;
                    txtScanQR1.SelectionLength = 0;
                    str_BarcodeGop = txtScanQR1.Text.TrimEnd(',');
                    BarcodeOld = txtScanQR1.Text;
                    if (str_BarcodeGop.Split(',').Length < SLGop) return;
                    txtScanQR1.Text = str_BarcodeGop;
                }
                Console.WriteLine("Real: " + txtScanQR1.Text);
                CheckBarCode(txtScanQR1.Text);
                txtScanQR1.Text = "";

                //Thread.Sleep(200);
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            searchLookUpEdit_MaDH.EditValue = null;
            searchLookUpEdit_PO.EditValue = null;
            ClearData();
        }
        private void searchLookUpEdit_DauSize_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_DauSize.EditValue is null) return;
            BindingMau();
        }
        private void searchLookUpEdit_Mau_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Mau.EditValue is null) return;
            BindingSize();
        }
        private void searchLookUpEdit_Size_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Size.EditValue is null) return;
            var dr = dtData.AsEnumerable().Where(x => x["DauSizeID"].ToString() == searchLookUpEdit_DauSize.EditValue.ToString()
                                                  && x["ColorID"].ToString() == searchLookUpEdit_Mau.EditValue.ToString()
                                                  && x["SizeID"].ToString() == searchLookUpEdit_Size.EditValue.ToString()).FirstOrDefault();
            if (dr != null)
            {
                SLGop = Convert.ToInt16(dr["SLGop"]);

            }
        }
        private void cbxThungLe_CheckedChanged(object sender, EventArgs e)
        {
            var value = cbxThungLe.Checked;
            if (value)
                BindingInSeam();
            else
            {
                SLGop = 0;
                searchLookUpEdit_DauSize.Properties.DataSource = null;
                searchLookUpEdit_Mau.Properties.DataSource = null;
                searchLookUpEdit_Size.Properties.DataSource = null;
            }
        }
        private void barcbxModule_EditValueChanged(object sender, EventArgs e)
        {
            if (barcbxModule.EditValue is null)
            {
                cbxDT_NK = "";
                return;
            }
            cbxDT_NK = barcbxModule.EditValue.ToString() == "Đóng thùng" ? "DT" : "NK";

        }
        private void grvTong_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsThungLe = Convert.ToInt16(grvTong.GetRowCellValue(e.RowHandle, "IsThungLe"));
            if (IsThungLe == 1) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }
        private DataTable CreatetblSave()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("MaPKL", typeof(string));
            dt.Columns.Add("SttThung", typeof(string));
            dt.Columns.Add("NgayNhapKho_TC", typeof(string));
            return dt;
        }
        private void ClearData()
        {
            dgcTong.DataSource = null;

            cbxThungLe.Checked = false;
            txtMaDH.Text = "";
            txtPO.Text = "";
            txtPO.Text = "";
            txtMaHang.Text = "";
            txtDauSize.Text = "";
            txtMau.Text = "";
            txtSize.Text = "";
            lblSTDaQuet.Text = "0/0";
        }

    }

}
