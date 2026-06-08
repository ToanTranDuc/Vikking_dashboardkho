using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmScanBarcodeXuatHangV : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                          new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        bool isStartFillBarcode = false;
        private DataTable dtDataT = new DataTable();
        private DataTable dtData = new DataTable();
        private DataTable dtDSPhieu = new DataTable();
        private DataTable dtBarcode = new DataTable();
        private DataTable dtListBarcode = new DataTable();
        private DataTable dtSize = new DataTable();
        System.Timers.Timer tmrGetData = new System.Timers.Timer();
        private string Barcode = "", MaPhieuXH_Win = "", MaPhieuXH = "", Cont = "", Cont_Link = "", MaHang_SS = "", MaHang_SSLink = "",
                        Poid = "", POID_Link = "", DauSize = "", Mau = "", Size = "", IsThungLe = "0", Store_Link = "", Store = "";
        private int statusCreate = 0;
        private object keylock = new object();
        DataRow drLinkTab = null;
        DataRow drFocusCT = null;
        private int SLGop = 1;
        private int CheckSLGop = 0;
        private bool flagReloadData = false;
        private bool flagCarton_AllCont = false;
        public frmScanBarcodeXuatHangV()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tmrGetData.Interval = 3000;
            tmrGetData.AutoReset = false;
            tmrGetData.Elapsed += TmrGetData_Elapsed;
            chbIsKien.Checked = false;
            //tmrGetData.Start();
            searchLookUpEdit_Phieu.EditValueChanged += SearchLookUpEdit_Phieu_EditValueChanged;
            searchLookUpEdit_PO.EditValueChanged += SearchLookUpEdit_PO_EditValueChanged;
            searchLookUpEdit_Store.EditValueChanged += SearchLookUpEdit_Store_EditValueChanged;
            searchLookUpEdit_Mau.EditValueChanged += SearchLookUpEdit_Mau_EditValueChanged;
            searchLookUpEdit_DauSize.EditValueChanged += SearchLookUpEdit_DauSize_EditValueChanged;
            searchLookUpEdit_Size.EditValueChanged += SearchLookUpEdit_Size_EditValueChanged;
            searchLookUpEdit_MaHang.EditValueChanged += SearchLookUpEdit_MaHang_EditValueChanged;
            grvPKLXuatHang.CustomColumnDisplayText += GrvPKLXuatHang_CustomColumnDisplayText;
            grvPKLXuatHang.CellMerge += GrvPKLXuatHang_CellMerge;
            grvPKLXuatHang.CustomSummaryCalculate += GrvPKLXuatHang_CustomSummaryCalculate;
            barcbxAllSize.CheckedChanged += BarcbxAllSize_CheckedChanged;
        }

        private void BarcbxAllSize_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
            GetDataXuatHang(MaPhieuXH_Win, Cont, false);
        }
        private void cbxScanKTheoCont_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            flagCarton_AllCont = cbxScanKTheoCont.Checked;
            GetDataXuatHang(MaPhieuXH_Win, Cont, false);
            GetDataCTPhieu(MaPhieuXH_Win, Cont);
        }
        private void GrvPKLXuatHang_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }

        private void GrvPKLXuatHang_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }

        private void GrvPKLXuatHang_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Init();
            GetPhieu();
        }
        #region Init
        private void Init()
        {
            searchLookUpEdit_Phieu.Properties.DisplayMember = "MaPKLXH_Display";
            searchLookUpEdit_Phieu.Properties.ValueMember = "value";
            searchLookUpEdit_Phieu.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_MaHang.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEdit_MaHang.Properties.ValueMember = "valueMaHang";
            searchLookUpEdit_MaHang.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_Store.Properties.DisplayMember = "Store";
            searchLookUpEdit_Store.Properties.ValueMember = "Store";
            searchLookUpEdit_Store.Properties.NullText = "[Chọn giá trị]";

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
        private void TmrGetData_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmrGetData.Stop();
                GetBarcodeFromTab();
            }
            catch (Exception ex)
            {
                System.Threading.Thread.Sleep(2000);
                tmrGetData.Start();
            }

        }
        private void SearchLookUpEdit_Phieu_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Phieu.EditValue is null || searchLookUpEdit_Phieu.EditValue.ToString() == "") return;
            MaPhieuXH_Win = searchLookUpEdit_Phieu.EditValue.ToString();

            var value = searchLookUpEdit_Phieu.EditValue.ToString().Split('|');
            MaPhieuXH_Win = value[0];
            Cont = value[1];
            //GetBarcodeInfo();
            barcodePhieu.Text = MaPhieuXH_Win;
            GetDataXuatHang(MaPhieuXH_Win, Cont, false);
            GetDataCTPhieu(MaPhieuXH_Win, Cont);
            //GetDataCTPhieu(MaPhieuXH_Win);
        }
        private void SearchLookUpEdit_MaHang_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MaHang.EditValue is null || searchLookUpEdit_MaHang.EditValue.ToString() == "") return;
            MaHang_SS = searchLookUpEdit_MaHang.EditValue.ToString();
            MaHang_SSLink = searchLookUpEdit_MaHang.EditValue.ToString();
            BindingPO();
        }
        private void SearchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_PO.EditValue is null || searchLookUpEdit_PO.EditValue.ToString() == "") return;
            Poid = searchLookUpEdit_PO.EditValue.ToString();
            POID_Link = searchLookUpEdit_PO.EditValue.ToString();
            flagReloadData = true;
            BindingStore();
        }
        private void SearchLookUpEdit_Store_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Store.EditValue is null) return;
            Store = searchLookUpEdit_Store.EditValue.ToString();
            BindingDauSize();
        }
        private void SearchLookUpEdit_DauSize_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_DauSize.EditValue is null || searchLookUpEdit_DauSize.EditValue.ToString() == "") return;
            DauSize = searchLookUpEdit_DauSize.EditValue.ToString();
            BindingMau();
        }
        private void SearchLookUpEdit_Mau_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Mau.EditValue is null || searchLookUpEdit_Mau.EditValue.ToString() == "") return;
            Mau = searchLookUpEdit_Mau.EditValue.ToString();
            BindingSize();
        }
        private void SearchLookUpEdit_Size_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Size.EditValue is null || searchLookUpEdit_Size.EditValue.ToString() == "") return;
            Size = searchLookUpEdit_Size.EditValue.ToString();
        }
        private void grvTong_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsThungLe = Convert.ToInt16(grvTong.GetRowCellValue(e.RowHandle, "IsThungLe"));
            if (IsThungLe == 1) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }
        #endregion

        private void GetPhieu()
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetPhieuXHScan&Para1=All&Para2={DateTime.Now.ToString("yyyy-MM-dd")}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtDSPhieu = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Phieu.Properties.DataSource = dtDSPhieu;
        }
        private void GetBarcodeInfo()
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetBarcode&Para1={MaPhieuXH}&Para2={Cont}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtBarcode = JsonConvert.DeserializeObject<DataTable>(json);
            BindingPO();
        }
        private void BindingMaHang()
        {
            if (dtDataT is null || dtDataT.Rows.Count == 0) return;
            var lstMaHang = dtData.AsEnumerable().Select(x => new
            {
                valueMaHang = x["valueMaHang"].ToString(),
                TenHang = x["TenHang"].ToString(),
                Dot = x["Dot"].ToString(),
                MaHangDisplay = x["MaHangDisplay"].ToString()
            }).Distinct().ToList();
            string json = JsonConvert.SerializeObject(lstMaHang);
            DataTable dtMaHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaHang.Properties.DataSource = dtMaHang;
            searchLookUpEdit_MaHang.EditValue = null;
            searchLookUpEdit_MaHang.EditValue = dtMaHang.Rows[0]["valueMaHang"];
        }
        private void BindingPO()
        {
            if (dtDataT is null || dtDataT.Rows.Count == 0) return;
            var lstPO = dtDataT.AsEnumerable().Where(y => y["valueMaHang"].ToString() == MaHang_SS)
                                                        .Select(x => new { POID = x["POID"].ToString(), PO = x["PO"].ToString() }).Distinct().ToList();
            string json = JsonConvert.SerializeObject(lstPO);
            DataTable dtPO = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_PO.Properties.DataSource = dtPO;
            searchLookUpEdit_PO.EditValue = null;
            searchLookUpEdit_PO.EditValue = dtPO.Rows[0]["POID"];
        }
        private void BindingStore()
        {
            if (dtBarcode is null || dtBarcode.Rows.Count == 0) return;
            var lstStore = dtBarcode.AsEnumerable().Where(y => y["valueMaHang"].ToString() == MaHang_SS && y["POID"].ToString() == Poid)
                                                        .Select(x => new { Store = x["Store"].ToString() }).Distinct().ToList();
            string json = JsonConvert.SerializeObject(lstStore);
            DataTable dtStore = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Store.Properties.DataSource = dtStore;
            searchLookUpEdit_Store.EditValue = null;
            searchLookUpEdit_Store.EditValue = dtStore.Rows[0]["Store"];
        }
        private void BindingDauSize()
        {
            var lstDauSize = dtBarcode.AsEnumerable().Where(y => y["valueMaHang"].ToString() == MaHang_SS && y["POID"].ToString() == Poid && y["Store"].ToString() == Store)
                                                    .Select(x => new { DauSizeID = x["DauSizeID"].ToString(), DauSize = x["DauSize"].ToString() }).Distinct().ToList();
            if (lstDauSize.Count == 0) return;
            string json = JsonConvert.SerializeObject(lstDauSize);
            DataTable dtDauSize = JsonConvert.DeserializeObject<DataTable>(json);

            searchLookUpEdit_DauSize.Properties.DataSource = dtDauSize;
            searchLookUpEdit_DauSize.EditValue = null;
            searchLookUpEdit_DauSize.EditValue = dtDauSize.Rows[0]["DauSizeID"];
        }
        private void BindingMau()
        {
            var lstMau = dtBarcode.AsEnumerable().Where(y => y["valueMaHang"].ToString() == MaHang_SS && y["POID"].ToString() == Poid && y["Store"].ToString() == Store && y["DauSizeID"].ToString() == DauSize)
                                                .Select(x => new { ColorID = x["ColorID"].ToString(), TenMau = x["TenMau"].ToString() }).Distinct().ToList();
            string json = JsonConvert.SerializeObject(lstMau);
            DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Mau.Properties.DataSource = dtMau;
            searchLookUpEdit_Mau.EditValue = null;
            searchLookUpEdit_Mau.EditValue = dtMau.Rows[0]["ColorID"];
        }
        private void BindingSize()
        {
            var lstSize = dtBarcode.AsEnumerable().Where(y => y["valueMaHang"].ToString() == MaHang_SS && y["POID"].ToString() == Poid && y["Store"].ToString() == Store && y["DauSizeID"].ToString() == DauSize && y["ColorID"].ToString() == Mau)
                                             .Select(x => new { SizeID = x["SizeID"].ToString(), Size = x["Size"].ToString(), SLGop = x["SLGop"].ToString() }).Distinct().ToList();
            string json = JsonConvert.SerializeObject(lstSize);
            dtSize = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Size.Properties.DataSource = dtSize;
            searchLookUpEdit_Size.EditValue = null;
            searchLookUpEdit_Size.EditValue = dtSize.Rows[0]["SizeID"];
        }
        private void GetDataXuatHang(string maPhieuXH, string cont, bool flagBindingWin = false, bool flagPreventPopup = false)
        {
            try
            {
                if (cont == "") return;
                string cont_T = cbxScanKTheoCont.Checked ? "All" : cont;
                string url = string.Format("{0}", URL + $"ScanBarcode/Get?Action=GetCTPhieuScan&Para1={maPhieuXH}&Para2={cont_T}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                dtDataT = JsonConvert.DeserializeObject<DataTable>(json);
                var dtBarcodeTemp = dtDataT.AsEnumerable().Where(x => x["IsThungLe"].ToString() == IsThungLe);
                dtBarcode = dtBarcodeTemp.Count() > 0 ? dtBarcodeTemp.CopyToDataTable() : new DataTable();

                if (flagBindingWin)
                {
                    if (!barcbxAllSize.Checked && !flagCarton_AllCont)
                        dtData = dtDataT.AsEnumerable().Where(x => x["valueMaHang"].ToString() == MaHang_SSLink && x["POID"].ToString() == POID_Link).CopyToDataTable();
                    else dtData = dtDataT;
                    dgcTong.DataSource = dtData;
                    BindingDataToView(0, flagPreventPopup);
                    return;
                }
                dtData = dtDataT;
                BindingMaHang();
                if (!barcbxAllSize.Checked && !flagCarton_AllCont)
                    dtData = dtDataT.AsEnumerable().Where(x => x["valueMaHang"].ToString() == MaHang_SSLink && x["POID"].ToString() == POID_Link).CopyToDataTable();

                //var dtDataTemp = dtData.AsEnumerable().Where(x => x["Cont"].ToString() == Cont);
                //dtData = dtDataTemp.Count() > 0 ? dtDa taTemp.CopyToDataTable() : new DataTable();
                var dtTemp = dtData.Copy();
                //CreateListBarcode();
                //CreateBandSize(dtData, grvPKLXuatHang, gbSize);
                //KHDongThungLib.ProcessSttTrung1(dtData);
                if (dtData.Rows.Count > 0)
                {
                    if (dtData.Rows[0]["Store"].ToString() != "")
                        colStoreA.Visible = true;
                    else colStoreA.Visible = false;
                }
                dgcTong.DataSource = dtData;
                //UpdateBarcodeLinkWin();
                BindingDataToView(0, flagPreventPopup);
            }
            catch (Exception ex)
            {

            }

            //grvPKLXuatHang.ExpandAllGroups();

            //DataTable processedData = KHDongThungLib.sumToTalPCS_Scan(dtTemp);
            //KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
            //dgcTong.DataSource = processedData;
        }
        private void GetDataCTPhieu(string maPhieuXH, string cont)
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXH&Para1={maPhieuXH}&Para2=All");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var dtTempA = JsonConvert.DeserializeObject<DataTable>(json);
            var dtTemp = dtTempA.AsEnumerable().Where(x => x["Cont"].ToString() == cont);
            var dtDSCTPhieu = dtTemp.Count() > 0 ? dtTemp.CopyToDataTable() : new DataTable();
            CreateBandSize(dtDSCTPhieu, grvPKLXuatHang, gbSize);
            dgrPKLXuatHang.DataSource = dtDSCTPhieu;
            KHDongThungLib.AllowVieworNotPack(dtDSCTPhieu, BandPCB, BandPack, BandStore);
        }
        private void CreateListBarcode()
        {
            dtListBarcode = dtData.Clone();
            //var drNew = dtListBarcode.NewRow();
            var dtDataTemp = dtData.AsEnumerable().Where(x => Convert.ToInt16(x["SLGop"]) > 1);
            var dtDataThungGop = dtDataTemp.Count() > 0 ? dtDataTemp.CopyToDataTable() : new DataTable();
            foreach (DataRow dr in dtDataThungGop.Rows)
            {
                var barcode = dr["Barcode"].ToString();
                if (barcode == "") continue;
                var splitBarcode = barcode.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitBarcode.Length < 2) return;
                foreach (var item in splitBarcode)
                {
                    var drNew = dtListBarcode.NewRow();
                    KHDongThungLib.CopyDataRow(dr, drNew);
                    drNew["Barcode"] = item;
                    dtListBarcode.Rows.Add(drNew);
                }
            }
        }
        private string Save(string action, DataRow dr)
        {
            DataTable dtSave = KHDongThungLib.CreateTblSave();
            DataRow drNew = dtSave.NewRow();
            drNew["ID"] = 0;
            drNew["MaDH"] = dr["MaDH"];
            drNew["POID"] = dr["POID"];
            drNew["Cont"] = dr["Cont"];
            drNew["QRCode"] = dr["Barcode"];
            drNew["DeliveryTo"] = dr["MaPKL_XH"];
            drNew["IsThungLe"] = dr["IsThungLe"];
            drNew["SttThung"] = dr["SttThung"];
            dtSave.Rows.Add(drNew);
            string url = string.Format("{0}", URL + $"XuatHang/UpdateScanWin?action={action}");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            return result;
        }
        private string SaveV2(string action, DataRow dr)
        {
            DataTable dtSave = CreatetblScanBarcodeXH();
            DataRow drNew = dtSave.NewRow();
            drNew["MaPKL_XH"] = dr["MaPKL_XH"];
            drNew["Cont"] = dr["Cont"];
            drNew["MaHang"] = dr["MaHang"];
            drNew["Season"] = dr["Dot"];
            drNew["POID"] = dr["POID"];
            drNew["Store"] = dr["Store"];
            drNew["DauSizeID"] = dr["DauSizeID"];
            drNew["ColorID"] = dr["ColorID"];
            drNew["SizeID"] = dr["SizeID"];
            drNew["STQuet"] = dr["STDaQuet"];
            drNew["IsThungLe"] = dr["IsThungLe"];
            drNew["Barcode"] = dr["Barcode"];
            drNew["NhanVien"] = GlobleData.UserName;
            dtSave.Rows.Add(drNew);
            string url = string.Format("{0}", URL + $"ScanBarcode/UpdateScanV2?action={action}");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            return result;
        }

        private void AlarmScanBarcode(string code, string content, string barcode = "")
        {
            DataTable dtAlarm = CreatetblAlarm();
            var drNew = dtAlarm.NewRow();
            drNew["ID"] = 0;
            drNew["MaPKL_XH"] = MaPhieuXH;
            drNew["POID"] = POID_Link;
            drNew["MaCont"] = Cont_Link;
            drNew["Barcode"] = Barcode + " || " + barcode;
            drNew["MaLoi"] = code;
            drNew["Content"] = content;
            drNew["NhanVien"] = GlobleData.UserName;
            dtAlarm.Rows.Add(drNew);
            string url = string.Format("{0}", URL + "XuatHang/PostAlarm?action=Post");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtAlarm); }).Result;
        }
        #region function
        private DataTable CreatetblAlarm()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPKL_XH", typeof(string));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("MaPKL", typeof(string));
            dt.Columns.Add("SttThungMin", typeof(string));
            dt.Columns.Add("MaCont", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Barcode", typeof(string));
            dt.Columns.Add("NhanVien", typeof(string));
            dt.Columns.Add("MaLoi", typeof(string));
            dt.Columns.Add("Content", typeof(string));
            return dt;
        }
        private void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA)
        {
            ClearBand(gbSizeA);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
                if (!CheckExistBand(_sizeID, gbSizeA)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = _size + "@" + _sizeID;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = this.repotxtN0;
                col.Visible = true;
                col.Width = 40;
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                //if (grvShared == dgwTong)
                //{
                //    if (col.FieldName.Contains("@"))
                //    {
                //        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                //        itemSize.FieldName = col.FieldName;
                //        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                //        itemSize.DisplayFormat = "{0:n0}";
                //        itemSize.ShowInGroupColumnFooter = col;
                //        grvShared.GroupSummary.Add(itemSize);
                //    }

                //    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                //    if (arrName.Length > 1)
                //    {
                //        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                //    }
                //}
                grvShared.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }
        private bool CheckExistBand(string size, GridBand gbSizeA)
        {
            GridBand gbCheck = gbSizeA.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }

        private void btnScan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = txtScanQR1;
            txtScanQR1.Text = "";
        }

        private void btnCreateBarcode_Click(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Size is null || searchLookUpEdit_Size.ToString() == "")
            {
                MessageBox.Show("Vui lòng chọn size để cài barcode!");
                return;
            }
            BindingDataToView(1);
            //UpdateBarcodeLinkWin(1);
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetDataXuatHang(MaPhieuXH, Cont_Link);
            GetDataCTPhieu(MaPhieuXH, Cont_Link);
            SumSlThungDaQuet();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (flagReloadData)
            {
                GetDataXuatHang(drLinkTab["MaPKL_XH"].ToString(), drLinkTab["Cont"].ToString(), true);
                flagReloadData = false;
            }
            BindingDataToView();
            //UpdateBarcodeLinkWin();
        }
        private void BindingDataToView(int IsScan = 0, bool flagPreventPopup = false)
        {
            if (searchLookUpEdit_Size.EditValue is null || searchLookUpEdit_Size.EditValue.ToString() == "")
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!");
                return;
            }
            //dtData = dtDataT.AsEnumerable().Where(x => x["valueMaHang"].ToString() == MaHang_SSLink && x["POID"].ToString() == POID_Link).CopyToDataTable();
            var drScanInfo = dtData.AsEnumerable().Where(y => y["valueMaHang"].ToString() == MaHang_SS && y["POID"].ToString() == Poid && y["DauSizeID"].ToString() == DauSize
                                                                && y["ColorID"].ToString() == Mau && y["SizeID"].ToString() == Size && y["IsThungLe"].ToString() == IsThungLe && y["Store"].ToString() == Store).FirstOrDefault();
            ;
            if (drScanInfo is null) return;
            if (IsScan == 0 && drScanInfo["Barcode"].ToString() == "" && !flagPreventPopup && !chbIsKien.Checked)
            {
                DialogResult resultA = MessageBox.Show("Size chưa khai báo barcode, Bạn có muốn tạo barcode lần đầu tiên!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultA == DialogResult.Yes) IsScan = 1;
            }
            if (drScanInfo is null) return;
            drLinkTab = drScanInfo;
            //if (IsScan == 1) statusCreate = 0;
            txtMaPhieu.Text = drScanInfo["MaPKL_XH"].ToString();
            txtCont.Text = drScanInfo["Cont"].ToString();
            txtMaHang.Text = drScanInfo["MaHang"].ToString();
            txtPO.Text = drScanInfo["PO"].ToString();
            txtDauSize.Text = drScanInfo["DauSize"].ToString();
            txtMau.Text = drScanInfo["TenMau"].ToString();
            txtSize.Text = drScanInfo["Size"].ToString();

            int SLThung = 0, SLTDaQuet = 0;
            if (!barcbxAllSize.Checked && !flagCarton_AllCont)
            {
                SLThung = Convert.ToInt16(drScanInfo["SLThung"]);
                SLTDaQuet = Convert.ToInt16(drScanInfo["STDaQuet"]);
            }
            else
            {
                SLThung = dtData.AsEnumerable().Sum(y => Convert.ToInt16(y["SLThung"]));
                SLTDaQuet = dtData.AsEnumerable().Sum(y => Convert.ToInt16(y["STDaQuet"]));
            }


            lblSTDaQuet.Text = SLTDaQuet.ToString() + "/" + SLThung.ToString();
            if (drScanInfo["Barcode"].ToString() != "" && IsScan == 0 && drScanInfo["SLGop"].ToString() == "1")
            {
                this.ActiveControl = txtScanQR1;
            }
            else
                this.ActiveControl = txtScanQRCreate;


        }
        private void UpdateBarcodeLinkWin(int IsScan = 0)
        {
            if (searchLookUpEdit_Size.EditValue is null || searchLookUpEdit_Size.EditValue.ToString() == "")
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!");
                return;
            }
            var drScanInfo = dtDataT.AsEnumerable().Where(y => y["valueMaHang"].ToString() == MaHang_SS && y["POID"].ToString() == Poid && y["DauSizeID"].ToString() == DauSize
                                                                && y["ColorID"].ToString() == Mau && y["SizeID"].ToString() == Size && y["IsThungLe"].ToString() == IsThungLe && y["Store"].ToString() == Store).FirstOrDefault();
            ;
            if (drScanInfo is null) return;
            if (IsScan == 0 && drScanInfo["Barcode"].ToString() == "")
            {
                DialogResult resultA = MessageBox.Show("Size chưa khai báo barcode, Bạn có muốn tạo barcode lần đầu tiên!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultA == DialogResult.Yes) IsScan = 1;
            }
            if (drScanInfo is null) return;
            DataTable dtSave = KHDongThungLib.CreateTblSave();
            DataRow drNew = dtSave.NewRow();
            drNew["ID"] = 0;
            drNew["DeliveryTo"] = MaPhieuXH_Win;
            drNew["MaHang"] = MaHang_SS.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0];
            drNew["DotSX"] = MaHang_SS.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[1];
            drNew["Cont"] = Cont;
            drNew["POID"] = Poid;
            drNew["PO"] = searchLookUpEdit_PO.Text;
            drNew["Store"] = drScanInfo["Store"];
            drNew["DauSizeID"] = DauSize;
            drNew["DauSize"] = searchLookUpEdit_DauSize.Text;
            drNew["ColorID"] = Mau;
            drNew["TenMau"] = searchLookUpEdit_Mau.Text;
            drNew["SizeID"] = Size;
            drNew["Size"] = searchLookUpEdit_Size.Text;
            drNew["QRCode"] = drScanInfo["Barcode"];
            drNew["IsScan"] = IsScan;
            drNew["NVien"] = GlobleData.UserName;
            drNew["IsThungLe"] = IsThungLe;
            drNew["KieuLap"] = drScanInfo["SLGop"];
            drNew["KieuLapPCB"] = drScanInfo["KieuLapPCB"];
            dtSave.Rows.Add(drNew);
            string url = string.Format("{0}", URL + "XuatHang/UpdateBarcodeLinkWin?action=UpdateBarcodeLink");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (IsScan == 1) statusCreate = 0;
            if (drScanInfo["Barcode"].ToString() != "" && IsScan == 0 && drScanInfo["SLGop"].ToString() == "1")
            {
                this.ActiveControl = txtScanQR1;
            }
        }
        private void grvPKLXuatHang_DoubleClick(object sender, EventArgs e)
        {
            if (drFocusCT is null) return;
            frmChiTietScanThungXuatHang fr = new frmChiTietScanThungXuatHang(drFocusCT, false, false, true);
            fr.ShowDialog();
            int rowHandle = grvPKLXuatHang.FocusedRowHandle;
            GetDataXuatHang(MaPhieuXH_Win, Cont);
            GetDataCTPhieu(MaPhieuXH_Win, Cont);
            grvPKLXuatHang.FocusedRowHandle = rowHandle;
        }
        private void grvPKLXuatHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {


        }
        private void grvPKLXuatHang_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (e.RowHandle < 0)
            {
                drFocusCT = null;
                return;
            }
            drFocusCT = grvPKLXuatHang.GetFocusedDataRow();
        }
        private void btnBarcodePhieu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var lstPhieuT = dtDSPhieu.AsEnumerable().OrderByDescending(y => Convert.ToInt16(y["Sort"])).Select(x => new
            {
                MaPhieu = x["MaPKL_XH"].ToString(),
                SoBooking = x["SoBooking"].ToString(),
                TenKH = x["TenKH"].ToString(),
                MaPKLXH_Display = x["MaPKLXH_View"].ToString(),
            }).Distinct().ToList();
            string json = JsonConvert.SerializeObject(lstPhieuT);
            List<DanhSachPhieuXHEntity> lstPhieu = JsonConvert.DeserializeObject<List<DanhSachPhieuXHEntity>>(json);
            frmBarcodeView frm = new frmBarcodeView(lstPhieu);
            frm.ShowDialog();
        }

        private void cbxThungLe_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit cbxThungLe = sender as CheckEdit;
            IsThungLe = cbxThungLe.Checked ? "1" : "0";
            var dtBarcodeTemp = dtData.AsEnumerable().Where(x => x["IsThungLe"].ToString() == IsThungLe);
            dtBarcode = dtBarcodeTemp.Count() > 0 ? dtBarcodeTemp.CopyToDataTable() : new DataTable();
            BindingDauSize();
        }
        private void tmrGetDataFromTab_Tick(object sender, EventArgs e)
        {
            tmrGetDataFromTab.Enabled = false;
            GetBarcodeFromTab();
            //Console.WriteLine("Start timer");
            tmrGetDataFromTab.Enabled = true;
        }

        private void cbxRealtimeData_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (cbxRealtimeData.Checked)
                tmrRealTimeData.Enabled = true;
            else
                tmrRealTimeData.Enabled = false;
        }

        private void tmrRealTimeData_Tick(object sender, EventArgs e)
        {
            try
            {
                tmrRealTimeData.Enabled = false;
                GetDataXuatHang(drLinkTab["MaPKL_XH"].ToString(), drLinkTab["Cont"].ToString(), true, true);
                GetDataCTPhieu(drLinkTab["MaPKL_XH"].ToString(), drLinkTab["Cont"].ToString());
                tmrRealTimeData.Enabled = true;
            }
            catch (Exception ex)
            {
                tmrRealTimeData.Enabled = true;
            }
        }

       

        bool flagScan = false;
        private void txtScanQR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtScanQR1.Text == "") return;
                string find = "Barcode = '" + txtScanQR1.Text + "'";
                var dtCheckT = dtData.Select(find);
                //DataRow dr = dtData.Select(find).FirstOrDefault();
                if (dtCheckT.Count() <= 0)
                {
                    txtScanQR1.Text = "";
                    txtScanQR1.Refresh();
                    AlarmScanBarcode("3", "Barcode không trùng khớp với size đang chọn. Vui lòng kiểm tra lại", txtScanQR1.Text);
                    clsWaitForm.ShowWaningFormCustom(this, 500, "Barcode không trùng khớp với size đang chọn. Vui lòng kiểm tra lại");
                    //Console.WriteLine("Barcode:" + txtScanQR1.Text);
                    return;
                }
                flagScan = false;
                tmrScan.Enabled = false;
                Console.WriteLine(txtScanQR1.Text);
                //if (dr == null) return;
                lock (keylock)
                {
                    if (txtScanQR1.Text != Barcode && !barcbxAllSize.Checked && !flagCarton_AllCont)
                    {
                        txtScanQR1.Text = "";
                        txtScanQR1.Refresh();
                        AlarmScanBarcode("2", "Barcode không trùng khớp với size đang chọn. Vui lòng kiểm tra lại!");
                        clsWaitForm.ShowWaningFormCustom(this, 1000, "Barcode không trùng khớp với size đang chọn. Vui lòng kiểm tra lại!");
                        return;
                    }
                }

                var dtCheck = dtCheckT.CopyToDataTable();
                var drCheck = dtCheck.AsEnumerable().Where(x => Convert.ToInt16(x["STDaQuet"]) < Convert.ToInt16(x["SLThung"])).FirstOrDefault();
                if (drCheck is null)
                {
                    txtScanQR1.Text = "";
                    txtScanQR1.Refresh();
                    AlarmScanBarcode("2", "Số lượng > SLKH.");
                    clsWaitForm.ShowWaningFormCustomV2(this, 1000, "Số lượng > SLKH.");
                    return;
                }
                foreach (DataRow dr in dtData.Rows)
                {
                    if (dr["ID"].ToString() == drCheck["ID"].ToString())
                    {
                        dr["STDaQuet"] = Convert.ToInt16(dr["STDaQuet"]) + 1;
                        dgcTong.DataSource = dtData;
                        grvTong.RefreshData();
                        break;
                    }
                }
                //var result = Save("UpdateScan", drCheck);
                drCheck["STDaQuet"] = Convert.ToInt16(drCheck["STDaQuet"]) + 1;
                var result = SaveV2("UpdateScan", drCheck);
                if (result == "false")
                {
                    foreach (DataRow dr in dtData.Rows)
                    {
                        if (dr["ID"].ToString() == drCheck["ID"].ToString())
                        {
                            dr["STDaQuet"] = Convert.ToInt16(dr["STDaQuet"]) - 1;
                            dgcTong.DataSource = dtData;
                            grvTong.RefreshData();
                            break;
                        }
                    }
                    txtScanQR1.Text = "";
                    txtScanQR1.Refresh();
                    AlarmScanBarcode("3", "Lưu thất bại. Vui lòng kiểm tra lại!");
                    //clsWaitForm.ShowWaningFormCustom(this, 1000, "Lưu thất bại. Vui lòng kiểm tra lại!");
                    return;
                }
                else
                {
                    lock (keylock)
                    {
                        int SLThung = 0, SLTDaQuet = 0;
                        if (barcbxAllSize.Checked || flagCarton_AllCont)
                        {
                            SLThung = dtData.AsEnumerable().Sum(y => Convert.ToInt16(y["SLThung"]));
                            SLTDaQuet = dtData.AsEnumerable().Sum(y => Convert.ToInt16(y["STDaQuet"]));
                        }
                        else
                        {
                            SLThung = dtData.AsEnumerable().Where(x => x["Barcode"].ToString() == txtScanQR1.Text).Sum(y => Convert.ToInt16(y["SLThung"]));
                            SLTDaQuet = dtData.AsEnumerable().Where(x => x["Barcode"].ToString() == txtScanQR1.Text).Sum(y => Convert.ToInt16(y["STDaQuet"]));
                        }

                        txtDauSize.Text = drCheck["DauSize"].ToString();
                        txtMau.Text = drCheck["TenMau"].ToString();
                        txtSize.Text = drCheck["Size"].ToString();
                        lblSTDaQuet.Text = SLTDaQuet.ToString() + "/" + SLThung.ToString();
                    }
                }
                txtScanQR1.Text = "";
                txtScanQR1.Refresh();
            }
        }
        bool flagNoNeedChange = false;
        private void tmrQRCreate_Tick(object sender, EventArgs e)
        {
            //if (isStartFillBarcode)
            //{
            //    tmrQRCreate.Enabled = false;
            //    //Thread.Sleep(500);

            //}
        }
        private void txtScanQRCreate_TextChanged(object sender, EventArgs e)
        {
            //if (txtScanQRCreate.Text.Trim() == "" || flagNoNeedChange)
            //{
            //    flagNoNeedChange = false;
            //    return;
            //}
            //if (!isStartFillBarcode)
            //{
            //    tmrQRCreate.Interval = 500;
            //    tmrQRCreate.Enabled = true;
            //    isStartFillBarcode = true;
            //}
        }
        private void txtScanQRCreate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtScanQRCreate.Text == "") return;
                CheckSLGop++;
                isStartFillBarcode = false;
                if (SLGop > 1)
                {
                    var splitCheckTrungBarcode = txtScanQRCreate.Text.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    var checkTrung = splitCheckTrungBarcode.GroupBy(x => x).Where(ageGroup => ageGroup.Count() > 1).ToList();
                    if (checkTrung.Count > 0)
                    {
                        string barcodeTemp = "";
                        for (int i = 0; i < splitCheckTrungBarcode.Length - 1; i++)
                        {
                            if (splitCheckTrungBarcode[i] == "") continue;
                            barcodeTemp += splitCheckTrungBarcode[i] + "||";
                        }

                        flagNoNeedChange = true;
                        txtScanQRCreate.Text = barcodeTemp;
                        txtScanQRCreate.SelectionStart = txtScanQRCreate.Text.Length;
                        txtScanQRCreate.SelectionLength = 0;
                        CheckSLGop--;
                        AlarmScanBarcode("4", $"Thùng gọp barcode trùng. Barcode hiện tại: {barcodeTemp}!");
                        return;
                    }
                }
                if (CheckSLGop < SLGop)
                {
                    txtScanQRCreate.Text = txtScanQRCreate.Text + "||";
                    txtScanQRCreate.SelectionStart = txtScanQRCreate.Text.Length;
                    txtScanQRCreate.SelectionLength = 0;
                    isStartFillBarcode = false;
                    return;
                }
                CheckSLGop = 0;

                if (SLGop == 1 && !CheckBarcode())
                {
                    txtScanQRCreate.Text = "";
                    this.ActiveControl = txtScanQRCreate;
                    //tmrQRCreate.Enabled = true;

                    AlarmScanBarcode("3", "Barcode đã tồn tại. Tạo barcode thất bại!");
                    clsWaitForm.ShowWaningFormCustomV3(this, 1000, "Barcode đã tồn tại. Tạo barcode thất bại!");
                    return;
                }

                foreach (DataRow dr in dtData.Rows)
                {
                    if (dr["valueMaHang"].ToString() == drLinkTab["valueMaHang"].ToString()
                        && dr["POID"].ToString() == drLinkTab["POID"].ToString()
                        && dr["DauSizeID"].ToString() == drLinkTab["DauSizeID"].ToString()
                        && dr["ColorID"].ToString() == drLinkTab["ColorID"].ToString()
                        && dr["SizeID"].ToString() == drLinkTab["SizeID"].ToString()
                        && dr["IsThungLe"].ToString() == drLinkTab["IsThungLe"].ToString()
                        && dr["Store"].ToString() == drLinkTab["Store"].ToString())
                    {
                        if (dr["Barcode"].ToString() != "" && dr["Barcode"].ToString() != txtScanQRCreate.Text)
                        {
                            //Save("UpdateWhenChangeBarcode", dr);
                            dr["STDaQuet"] = 0;
                        }
                        dr["Barcode"] = txtScanQRCreate.Text;
                        dr["STDaQuet"] = SLGop > 1 ? Convert.ToInt16(dr["STDaQuet"]) + 1 : Convert.ToInt16(dr["STDaQuet"]) + 1;
                        var result = SaveCaiDatBarcode();
                        if (result == "true") this.ActiveControl = txtScanQR1;
                        SaveV2("UpdateScan", dr);
                        //Save("UpdateScan", dr);
                        //CreateListBarcode();
                        //if (SLGop > 1)
                        //{

                        //}                      
                        break;
                    }
                }
                int SLThung = 0, SLTDaQuet = 0;
                if (!barcbxAllSize.Checked && !flagCarton_AllCont)
                {
                    SLThung = dtData.AsEnumerable().Where(x => x["Barcode"].ToString() == txtScanQRCreate.Text).Sum(y => Convert.ToInt16(y["SLThung"]));
                    SLTDaQuet = dtData.AsEnumerable().Where(x => x["Barcode"].ToString() == txtScanQRCreate.Text).Sum(y => Convert.ToInt16(y["STDaQuet"]));
                }
                else
                {
                    SLThung = dtData.AsEnumerable().Sum(y => Convert.ToInt16(y["SLThung"]));
                    SLTDaQuet = dtData.AsEnumerable().Sum(y => Convert.ToInt16(y["STDaQuet"]));
                }
                //var SLThung = dtData.AsEnumerable().Where(x => x["Barcode"].ToString() == txtScanQRCreate.Text).Sum(y => Convert.ToInt16(y["SLThung"]));
                //var SLTDaQuet = dtData.AsEnumerable().Where(x => x["Barcode"].ToString() == txtScanQRCreate.Text).Sum(y => Convert.ToInt16(y["STDaQuet"]));
                lblSTDaQuet.Text = SLTDaQuet.ToString() + "/" + SLThung.ToString();

                dgcTong.DataSource = dtData;
                grvTong.RefreshData();
                lock (keylock)
                {
                    txtScanQRCreate.Text = "";
                }
            }
        }
        private bool CheckBarcode()
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetBarCodeCheck&Para1={drLinkTab["valueMaHang"].ToString()}&Para2=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtCheck = JsonConvert.DeserializeObject<DataTable>(json);
            if (drLinkTab["IsThungLe"].ToString() == "1")
            {
                var drCheck = dtCheck.AsEnumerable().Any(x => x["BarcodeLe"].ToString() == txtScanQRCreate.Text && x["POID"].ToString() == drLinkTab["POID"].ToString());
                if (drCheck) return false;
                else return true;
            }
            else
            {
                var drCheck = dtCheck.AsEnumerable().Any(x => x["BarcodeChan"].ToString() == txtScanQRCreate.Text && x["POID"].ToString() == drLinkTab["POID"].ToString());
                if (drCheck) return false;
                else return true;
            }
        }
        private string SaveCaiDatBarcode()
        {
            Barcode = txtScanQRCreate.Text.Clone().ToString();
            var tblSave = KHDongThungLib.CreateTblCaiDatBarcode();
            var drSave = tblSave.NewRow();
            var dr = drLinkTab;
            drSave["ID"] = 0;
            drSave["StyleID"] = dr["MaHang"];
            drSave["Season"] = dr["val_Dot"];
            drSave["POID"] = dr["POID"];
            drSave["Store"] = dr["Store"];
            drSave["DauSizeID"] = dr["DauSizeID"];
            drSave["MaMau"] = dr["ColorID"];
            drSave["SizeID"] = dr["SizeID"];
            drSave["BarCodeChan"] = dr["IsThungLe"].ToString() != "1" ? txtScanQRCreate.Text : "Default";
            drSave["SoLuongChan"] = 0;
            drSave["BarCodeLe"] = dr["IsThungLe"].ToString() == "1" ? txtScanQRCreate.Text : "Default";
            drSave["SoLuongLe"] = 0;
            drSave["CreateDate"] = DateTime.Now.ToString("yyyy-MM-dd");
            tblSave.Rows.Add(drSave);
            string url = string.Format("{0}?", URL + "CaiDatBarCode/Post?action=POST&Para1=A");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToLower() == "true")
            {
                SaveBarcodeCurrent();
                AlarmScanBarcode("1", "Tạo barcode và lưu thành công!");
                clsWaitForm.ShowSuccessFormCustom(this, 1000, "Tạo barcode thành công!");
            }
            else
            {
                Barcode = "";
                AlarmScanBarcode("2", "Tạo barcode thất bại!");
                //clsWaitForm.ShowWaningFormCustom(this, 1000, "Tạo barcode thất bại!");
            }
            return msResult.ToLower();

        }
        private void SaveBarcodeCurrent()
        {

            DataTable dtSave = KHDongThungLib.CreateTblSave();
            DataRow drNew = dtSave.NewRow();
            drNew["ID"] = 0;
            drNew["DeliveryTo"] = drLinkTab["MaPKL_XH"];
            drNew["Cont"] = drLinkTab["Cont"];
            drNew["QRCode"] = txtScanQRCreate.Text;
            drNew["NVien"] = GlobleData.UserName;
            dtSave.Rows.Add(drNew);
            string url = string.Format("{0}", URL + "XuatHang/UpdateBarcodeLinkWin?action=UpdateBarcodeLinkWin");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
        }
        private DataTable CreatetblScanBarcodeXH()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPKL_XH", typeof(string));
            dt.Columns.Add("Cont", typeof(string));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("STQuet", typeof(int));
            dt.Columns.Add("SttThungMin", typeof(int));
            dt.Columns.Add("IsThungLe", typeof(int));
            dt.Columns.Add("Barcode", typeof(string));
            dt.Columns.Add("NhanVien", typeof(string));
            return dt;
        }
        private void GetBarcodeFromTab()
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetBarCodeFromTab&Para1={GlobleData.UserName}&Para2=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt is null || dt.Rows.Count <= 0) return;
            lock (keylock)
            {
                Barcode = dt.Rows[0]["Barcode"].ToString();
            }
            var dtTemp = dt.Copy();
            BigndingInfoScan(dtTemp);
        }
        private void BigndingInfoScan(DataTable dt)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    BindingDataFromTab(dt);
                });
            }
            else
            {
                BindingDataFromTab(dt);
            }

        }
        private void BindingDataFromTab(DataTable dt)
        {
            //if (MaPhieuXH != dt.Rows[0]["MaPKL_XH"].ToString() || Cont != dt.Rows[0]["MaCont"].ToString())
            //{
            //    MaPhieuXH = dt.Rows[0]["MaPKL_XH"].ToString();
            //    Cont = dt.Rows[0]["MaCont"].ToString();
            //    searchLookUpEdit_Phieu.EditValue = MaPhieuXH;
            //    //GetDataXuatHang();
            //}

            if (MaPhieuXH != dt.Rows[0]["MaPKL_XH"].ToString() || dt.Rows[0]["MaCont"].ToString() != Cont_Link || dt.Rows[0]["valueMaHang"].ToString() != MaHang_SSLink || dt.Rows[0]["POID"].ToString() != POID_Link)
            {
                var MaPhieuXHT = dt.Rows[0]["MaPKL_XH"].ToString();
                MaHang_SSLink = dt.Rows[0]["valueMaHang"].ToString();
                POID_Link = dt.Rows[0]["POID"].ToString();
                var Cont_LinkT = dt.Rows[0]["MaCont"].ToString();
                barcodePhieu.Text = dt.Rows[0]["MaPKL_XH"].ToString();
                //drLinkTab = dt.Rows[0];
                //searchLookUpEdit_Phieu.EditValue = MaPhieuXH;
                GetDataXuatHang(MaPhieuXHT, Cont_LinkT);
                GetDataCTPhieu(MaPhieuXHT, Cont_LinkT);
            }

            if (MaPhieuXH != dt.Rows[0]["MaPKL_XH"].ToString() || Cont_Link != dt.Rows[0]["MaCont"].ToString() || txtPO.Text != dt.Rows[0]["PO"].ToString()
            || txtDauSize.Text != dt.Rows[0]["DauSize"].ToString() || txtMau.Text != dt.Rows[0]["TenMau"].ToString() || txtSize.Text != dt.Rows[0]["Size"].ToString()
            || dt.Rows[0]["IsCreate"].ToString() != statusCreate.ToString() || Store_Link != dt.Rows[0]["Store"].ToString())
            {
                MaPhieuXH = dt.Rows[0]["MaPKL_XH"].ToString();
                Cont_Link = dt.Rows[0]["MaCont"].ToString();
                txtMaPhieu.Text = dt.Rows[0]["MaPKL_XH"].ToString();
                txtCont.Text = dt.Rows[0]["Cont"].ToString();
                txtMaHang.Text = dt.Rows[0]["TenHang"].ToString();
                txtPO.Text = dt.Rows[0]["PO"].ToString();
                txtDauSize.Text = dt.Rows[0]["DauSize"].ToString();
                txtMau.Text = dt.Rows[0]["TenMau"].ToString();
                txtSize.Text = dt.Rows[0]["Size"].ToString();
                CheckSLGop = 0;
                SLGop = Convert.ToInt16(dt.Rows[0]["SLGop"]);
                Store_Link = dt.Rows[0]["Store"].ToString();
                txtScanQRCreate.Text = "";
                statusCreate = 0;

                lock (keylock)
                {
                    drLinkTab = dt.Rows[0];
                    SumSlThungDaQuet();


                    statusCreate = Convert.ToInt16(dt.Rows[0]["IsCreate"]);
                    if (statusCreate == 1 || SLGop > 1)
                    {
                        //tmrQRCreate.Enabled = true;
                        var currentControl = this.ActiveControl;
                        if (currentControl == txtScanQRCreate) return;
                        if (txtScanQRCreate.Visible && txtScanQRCreate.Enabled)
                            this.ActiveControl = txtScanQRCreate;
                    }
                    else
                    {
                        if (txtScanQR1.Visible && txtScanQR1.Enabled)
                            this.ActiveControl = txtScanQR1;
                    }
                }
            }
            //Console.WriteLine("End timer");
            // tmrGetData.Start();
        }
        private void SumSlThungDaQuet()
        {
            var dtSum = dtData.AsEnumerable().Where(x => x["valueMaHang"].ToString() == drLinkTab["valueMaHang"].ToString()
                                                            && x["POID"].ToString() == drLinkTab["POID"].ToString()
                                                            && x["DauSizeID"].ToString() == drLinkTab["DauSizeID"].ToString()
                                                            && x["ColorID"].ToString() == drLinkTab["ColorID"].ToString()
                                                            && x["SizeID"].ToString() == drLinkTab["SizeID"].ToString()
                                                            && x["IsThungLe"].ToString() == drLinkTab["IsThungLe"].ToString()
                                                            && x["Store"].ToString() == drLinkTab["Store"].ToString());
            var SLThung = dtSum.Sum(y => Convert.ToInt16(y["SLThung"]));
            var SLTDaQuet = dtSum.Sum(y => Convert.ToInt16(y["STDaQuet"]));
            lblSTDaQuet.Text = SLTDaQuet.ToString() + "/" + SLThung.ToString();
        }
        #endregion
    }
}
