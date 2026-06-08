using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using Newtonsoft.Json;
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
    public partial class frmERPKeHoachDongThung_SMS : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                          new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string _maKH = "", _Season = "", _DonHang = "", _DVSX = "", _POID = "", _DauSize = "", _Mau = "", _Size = "";
        int TuThung = 1, DenThung = 1, SttThung = 1, TuThungEdit = 0, DenThungEdit = 0, SttThungEdit = 0;
        //private bool flagEdit = false;
        string URL = string.Empty;
        private DataTable dtQuiCach = new DataTable();
        public static string _maDHSMS = "", _maPKLSMS = "";
        public frmERPKeHoachDongThung_SMS(string maDH = "", string maPKL = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maPKLSMS = maPKL;
            _maDHSMS = maDH;
            if (maDH != "")
            {                           
                txtMaDot.Text = _maDHSMS;
                txtMaDot.Enabled = false;
                LoadKHDongThung();
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateDefault();
            KHDongThungLib.InitQuiCach(repoQuiCach);
            LoadKhachHang();
            dtQuiCach = KHDongThungLib.GetAllQuiCach(repoQuiCach, URL, "Para", _clientExtension);
        }
        private void CreateDefault()
        {

            searchLookUpEdit_KhachHang.Properties.ValueMember = "MaKH";
            searchLookUpEdit_KhachHang.Properties.DisplayMember = "TenKH";
            searchLookUpEdit_KhachHang.Properties.NullText = "[Chọn KH]";

            //searchLookUpEdit_Season.Properties.ValueMember = "Dot";
            //searchLookUpEdit_Season.Properties.DisplayMember = "Dot";
            //searchLookUpEdit_Season.Properties.NullText = "[Chọn mùa]";

            searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            searchLookUpEdit_DonHang.Properties.DisplayMember = "GopDH";
            searchLookUpEdit_DonHang.Properties.NullText = "[Chọn ĐH]";

            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn PO]";

            searchLookUpEdit_Mau.Properties.ValueMember = "MaMau";
            searchLookUpEdit_Mau.Properties.DisplayMember = "TenMau";
            searchLookUpEdit_Mau.Properties.NullText = "[Chọn màu]";

            searchLookUpEdit_DauSize.Properties.ValueMember = "DauSizeID";
            searchLookUpEdit_DauSize.Properties.DisplayMember = "DauSize";
            searchLookUpEdit_DauSize.Properties.NullText = "[Chọn màu]";
            //searchLookUpEdit_Size.Properties.ValueMember = "SizeID";
            //searchLookUpEdit_Size.Properties.DisplayMember = "Size";
            //searchLookUpEdit_Size.Properties.NullText = "[Chọn khách hàng]";


        }
        private void LoadKHDongThung()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung_SMS&MaDH={_maDHSMS}&MaDVSX=Para&DotSX=Para&POID=A&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKLSMS}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtDataPiVot = JsonConvert.DeserializeObject<DataTable>(json);
            KHDongThungLib.ProcessSttTrung(dtDataPiVot);
            CreateBandForSize(dtDataPiVot);
            dgrKHDongThung.DataSource = dtDataPiVot;
            if (dtDataPiVot != null && dtDataPiVot.Rows.Count != 0)
            {
                SttThung = dtDataPiVot.AsEnumerable().Max(x => Convert.ToInt16(x["SttThung"])) + 1;
                TuThung = DenThung = SttThung;
            }

        }
        private void LoadKhachHang()
        {
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_KhachHang.Properties.DataSource = dt;
        }
        private DataTable GetSQL(string Action, string KhachHang = "Para", string Season = "Para", string MaDH = "Para", string POID = "Para", string ColorID = "Para")
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={Action}&MaDH={MaDH}&MaDVSX=Para&DotSX=Para&POID={POID}&SizeTypeID=Para&ColorID={ColorID}&ProductID={Season}&SizeID={KhachHang}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt is null) return new DataTable();
            return dt;
        }
        private void LoadSeaSon()
        {
            //searchLookUpEdit_Season.Properties.DataSource = GetSQL("GetSeason_SMS", _maKH);
        }
        private void LoadDH()
        {
            searchLookUpEdit_DonHang.Properties.DataSource = GetSQL("GetDHA", _maKH, "");
        }
        private void LoadPO()
        {
            searchLookUpEdit_PO.Properties.DataSource = GetSQL("GetPO_SMS", _maKH, _Season, _DonHang);
        }
        private void LoadInseam()
        {
            searchLookUpEdit_DauSize.Properties.DataSource = GetSQL("GetInSeam_SMS", _maKH, "", _DonHang, _POID);
        }
        private void LoadMau()
        {
            searchLookUpEdit_Mau.Properties.DataSource = GetSQL("GetMau_SMS", _maKH, _DauSize, _DonHang, _POID);
        }
        private void LoadData()
        {
            grcDetail.DataSource = GetSQL("GetSize_SMS", _maKH, _DauSize, _DonHang, _POID, _Mau);
        }

        private void ProcessPackage(bool flagEdit = false)
        {
            try
            {
                if (flagEdit && SttThungEdit < 0)
                {
                    MessageBox.Show("Vui lòng chọn thùng để chỉnh sửa!");
                    return;
                }
                var dtTemp = grcDetail.DataSource as DataTable;
                var dtTemp1 = dtTemp.AsEnumerable().Where(x => Convert.ToInt32(x["SLDT"]) > 0);

                if (dtTemp1.Count() <= 0) return;
                var dtOld = dgrKHDongThung.DataSource as DataTable;
                var dtGetData = dtTemp1.CopyToDataTable();
                var _dtTemp1 = dtGetData.AsEnumerable().Select(x => new
                {
                    SizeID = x["SizeID"].ToString(),
                    Size = x["Size"].ToString()
                }).Distinct().ToList();
                string json = JsonConvert.SerializeObject(_dtTemp1);
                DataTable _dtTempSize = JsonConvert.DeserializeObject<DataTable>(json);
                //CreateBandSize(_dtTempSize);
                var dtData = KHDongThungLib.CreateTblPackage();
                foreach (DataRow drSize in _dtTempSize.Rows)
                {
                    string colSize = drSize["Size"].ToString() + "@" + drSize["SizeID"].ToString();
                    dtData.Columns.Add(colSize, typeof(int));
                }
                int SumSL = 0;
                double SumKLuong = 0, SumTLuong = 0;
                if (flagEdit && dtOld != null)
                {
                    var dtTempSum = dtOld.AsEnumerable().Where(x => Convert.ToInt16(x["SttThung"]) == SttThungEdit);
                    var dtSum = dtTempSum.Count() > 0 ? dtTempSum.CopyToDataTable() : new DataTable();
                    if (dtTempSum.Count() > 0)
                    {
                        dtSum = dtTempSum.CopyToDataTable();
                        foreach (DataRow drA in dtSum.Rows)
                        {
                            foreach (DataColumn dcA in dtSum.Columns)
                            {
                                var colName = dcA.ColumnName;
                                if (!colName.Contains("@")) continue;
                                if (drA[colName].ToString() == "") continue;
                                SumSL += Convert.ToInt32(drA[colName]);
                                //SumKLuong += Convert.ToDouble(drA["NW"]) * Convert.ToInt32(drA["SLDT"]);                  
                            }
                        }
                    }
                    //foreach( DataRow dr in dtTempSum)
                    //{
                    //    dr["SoLuong"] = SumSL;
                    //    dr["TotalPiece"] = SumSL;
                    //    dr["KhoiLuong"] = SumKLuong;
                    //}
                }
                var distinctGetSize = dtGetData.AsEnumerable().Select(x => new
                {
                    MaDH = x["MaDH"].ToString(),
                    Dot = x["Dot"].ToString(),
                    POID = x["POID"].ToString(),
                    DotSX = x["DotSX"].ToString(),
                    DauSizeID = x["DauSizeID"].ToString(),
                    ColorID = x["MaMau"].ToString()
                }).Distinct().ToList();
                foreach (var item in distinctGetSize)
                {
                    var dtGetData1 = dtGetData.AsEnumerable().Where(x => x["MaDH"].ToString() == item.MaDH && x["Dot"].ToString() == item.Dot.ToString()
                                                                       && x["POID"].ToString() == item.POID.ToString() && x["DotSX"].ToString() == item.DotSX
                                                                       && x["DauSizeID"].ToString() == item.DauSizeID && x["MaMau"].ToString() == item.ColorID).CopyToDataTable();
                    var drNew = dtData.NewRow();
                    var dr = dtGetData1.Rows[0];
                    drNew["ID"] = 0;
                    drNew["TuThung"] = flagEdit ? TuThungEdit : TuThung;
                    drNew["DenThung"] = flagEdit ? TuThungEdit : TuThung;
                    drNew["SttThung"] = flagEdit ? SttThungEdit : SttThung;
                    drNew["MaDH"] = dr["MaDH"];
                    drNew["Dot"] = dr["Dot"];
                    drNew["MaDVSX"] = dr["MaDVSX"];
                    drNew["TenDVSX"] = dr["TenDVSX"];
                    drNew["DotSX"] = dr["DotSX"];
                    drNew["MaLenh"] = dr["MaLenhSanXuat"];
                    drNew["MaHang"] = dr["MaHang"];
                    drNew["MaDVSX"] = dr["MaDVSX"];
                    drNew["DotSX"] = dr["DotSX"];
                    drNew["POID"] = dr["POID"];
                    drNew["PO"] = dr["PO"];
                    drNew["DauSizeID"] = dr["DauSizeID"];
                    drNew["DauSize"] = dr["DauSize"];
                    drNew["ColorID"] = dr["MaMau"];
                    drNew["TenMau"] = dr["TenMau"];
                   
                    foreach (DataRow drA in dtGetData1.Rows)
                    {
                        string colSize = drA["Size"].ToString() + "@" + drA["SizeID"].ToString();
                        drNew[colSize] = drA["SLDT"];
                        SumSL += Convert.ToInt32(drA["SLDT"]);
                        SumKLuong += Convert.ToDouble(drA["NW"]) * Convert.ToInt32(drA["SLDT"]);
                        if (dtOld != null && !dtOld.Columns.Contains(colSize))
                        {
                            dtOld.Columns.Add(colSize);
                        }
                    }
                    drNew["SLThung"] = 1;
                    drNew["TrongLuong"] = 0;
                    drNew["KhoiLuong"] = 0;
                    drNew["TrongLuongA"] = 0;
                    drNew["IsSave"] = false;
                    dtData.Rows.Add(drNew);
                }

                if (!flagEdit)
                {
                    foreach (DataRow dr in dtData.Rows)
                    {
                        dr["SoLuong"] = SumSL;
                        dr["TotalPiece"] = SumSL;
                        dr["KhoiLuong"] = SumKLuong;
                    }
                }
               

                if (dtOld != null && dtOld.Rows.Count > 0)
                {
                    dtOld.Merge(dtData, true, MissingSchemaAction.Ignore);
                    if (flagEdit)
                    {
                        var dtEdit = dtOld.AsEnumerable().Where(x => Convert.ToInt16(x["SttThung"]) == SttThungEdit);
                        foreach(DataRow dr in dtEdit)
                        {
                            dr["SoLuong"] = SumSL;
                            dr["TotalPiece"] = SumSL;
                            dr["KhoiLuong"] = SumKLuong;
                        }
                    }
                    dtOld = dtOld.AsEnumerable().OrderBy(x => Convert.ToInt16(x["SttThung"])).CopyToDataTable();
                    dtData = dtOld;
                }
                
                
                CreateBandForSize(dtData);
                dgrKHDongThung.DataSource = dtData;
                if (!flagEdit)
                {
                    TuThung++;
                    SttThung++;
                }                
                ProcessWhenGetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ProcessEditPackage()
        {
            var dtTemp = grcDetail.DataSource as DataTable;
            var dtTemp1 = dtTemp.AsEnumerable().Where(x => Convert.ToInt32(x["SLDT"]) > 0);

            if (dtTemp1.Count() <= 0) return;
        }
        private void ProcessWhenGetData()
        {
            var dtTemp = grcDetail.DataSource as DataTable;
            foreach (DataRow dr in dtTemp.Rows)
            {
                dr["SLDaLapPKL"] = Convert.ToInt16(dr["SLDaLapPKL"]) + Convert.ToInt16(dr["SLDT"]);
                dr["SLDT"] = 0;
            }
        }
        private void ProcessWhenClearData()
        {
            try
            {
                this.ActiveControl = grcDetail;
                DataTable dt = dgrKHDongThung.DataSource as DataTable;
                var dtDetail = grcDetail.DataSource as DataTable;
                if (dtDetail is null) return;
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        var colName = dc.ColumnName;
                        if (!colName.Contains('@')) continue;
                        var sizeID = colName.Split('@')[1];
                        foreach (DataRow drDetail in dtDetail.Rows)
                        {
                            if (drDetail["MaDH"].ToString() == dr["MaDH"].ToString() && drDetail["Dot"].ToString() == dr["Dot"].ToString()
                               && drDetail["MaDVSX"].ToString() == dr["MaDVSX"].ToString() && drDetail["POID"].ToString() == dr["POID"].ToString()
                               && drDetail["DauSizeID"].ToString() == dr["DauSizeID"].ToString() && drDetail["MaMau"].ToString() == dr["ColorID"].ToString()
                               && drDetail["SizeID"].ToString() == sizeID)
                            {
                                drDetail["SLDaLapPKL"] = Convert.ToInt16(drDetail["SLDaLapPKL"]) - (dr[colName] == DBNull.Value ? 0 : Convert.ToInt16(dr[colName]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Save()
        {
            try
            {
                //if (txtMaDot.Text == "")
                //{
                //    MessageBox.Show("Vui lòng nhập mã đợt để lưu PKL");
                //    return;
                //}
                DataTable tblSave = KHDongThungLib.CreateTblSave();
                DateTime dtNow = DateTime.Now;
                this.ActiveControl = grcDetail;
                var _dtData = dgrKHDongThung.DataSource as DataTable;
                int SttThung_temp = 0;
                int SttThungOld = 0;
                int SttThungDeCat_Start = 0;
                int SttThung = 0;
                int index = 0;
                foreach (DataRow dr in _dtData.Rows)
                {
                    foreach (DataColumn dc in _dtData.Columns)
                    {
                        var colName = dc.ColumnName;
                        if (!colName.Contains("@")) continue;
                        if (dr[colName].ToString() == "" || dr[colName].ToString() == "0") continue;
                        var splitCol = colName.Split('@');
                        var _sizeID = splitCol[1];
                        var _size = splitCol[0];
                        DataRow drNewRow = tblSave.NewRow();
                        drNewRow["ID"] = 0;
                        drNewRow["MaPKL"] = _maPKLSMS;
                        drNewRow["MaDH"] = _maDHSMS;
                        drNewRow["MaDVSX"] = dr["MaDVSX"];
                        drNewRow["MaLenh"] = dr["MaLenh"];
                        drNewRow["DotSX"] = dr["DotSX"];
                        drNewRow["MaHang"] = dr["MaHang"];
                        drNewRow["POID"] = dr["POID"];
                        drNewRow["PO"] = dr["PO"];
                        drNewRow["ColorID"] = dr["ColorID"].ToString();
                        drNewRow["TenMau"] = dr["TenMau"].ToString();
                        drNewRow["DauSize"] = dr["DauSize"].ToString();
                        drNewRow["DauSizeID"] = dr["DauSizeID"].ToString();
                        drNewRow["SizeID"] = _sizeID;
                        drNewRow["Size"] = _size;
                        drNewRow["NgayLapKH"] = dtNow;
                        drNewRow["ChieuDai"] = dr["ChieuDai"];
                        drNewRow["ChieuRong"] = dr["ChieuRong"];
                        drNewRow["ChieuCao"] = dr["ChieuCao"];
                        drNewRow["KyHieu"] = dr["KyHieu"];
                        drNewRow["TrongLuong"] = Math.Round(Convert.ToDouble(dr["TrongLuong"]), 1);
                        drNewRow["KhoiLuong"] = Math.Round(Convert.ToDouble(dr["KhoiLuong"]), 1);
                        drNewRow["SoLuongThung"] = 1;
                        drNewRow["TuThung"] = dr["TuThung"];
                        drNewRow["DenThung"] = dr["DenThung"];

                        if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                        {
                            SttThung++;
                            index = 0;
                            SttThung_temp++;
                            SttThungDeCat_Start = 0;
                            SttThungOld = Convert.ToInt32(dr["SttThung"]);
                        }
                        drNewRow["SttThung_decat"] = 0;
                        drNewRow["SttThung_LapMau"] = 0;
                        drNewRow["SttThung_temp"] = index == 0 ? 1 : index;
                        drNewRow["SttThung"] = SttThung;
                        if (SttThungDeCat_Start == 0) SttThungDeCat_Start = Convert.ToInt32(drNewRow["SttThung"]);
                        drNewRow["SttThung_start"] = 0;

                        drNewRow["SoLuongSP"] = Convert.ToInt32(dr[colName].ToString());
                        drNewRow["IsDongThung"] = false;
                        drNewRow["NgayDongThung"] = dtNow;
                        drNewRow["NgayNhapKho"] = dtNow;
                        drNewRow["QRCode"] = "";
                        drNewRow["IsNhapKho"] = false;
                        //drNewRow["KyHieu"] = string.Format("{0} -> {1}", TuThung, DenThung);
                        drNewRow["Chon"] = false;
                        drNewRow["IsThungLe"] = false; //-dr["IsThungLe"];
                        drNewRow["NVien"] = GlobleData.UserName;
                        drNewRow["KieuLap"] = "0";
                        //drNewRow["StyleName"] = txtStyleName.Text;
                        //drNewRow["DeptNo"] = txtDeptNo.Text;
                        //drNewRow["Destination"] = txtCangDen.Text;
                        drNewRow["DeliveryTo"] = Convert.ToBoolean(dr["IsSave"]) ? dr["MaDH_SMS"] : dr["MaDH"];
                        //drNewRow["Terms"] = txtTerms.Text;
                        //drNewRow["CountryOfOrigin"] = txtCountry.Text;
                        drNewRow["KieuLapPCB"] = "4";
                        tblSave.Rows.Add(drNewRow);
                    }
                }
                if (tblSave == null || tblSave.Rows.Count == 0)
                {
                    MessageBox.Show("Số lượng kế hoạch không được để trống, phải có ít nhất số lượng của 1 size");
                    return;
                }
                string url = string.Format("{0}", URL + "KeHoachDongThung/Post?action=InsertKHDT");
                //return;
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
                if (result.ToLower() == "true")
                {
                    _maDHSMS = _maDHSMS;
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnGetData_Click(object sender, EventArgs e)
        {
            ProcessPackage();
        }
        private void btnEditData_Click(object sender, EventArgs e)
        {
            ProcessPackage(true);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ProcessWhenClearData();
            dgrKHDongThung.DataSource = new DataTable();
            TuThung = 1;
            SttThung = 1;
            LoadKHDongThung();
        }
        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        #region Event SearchLookup     
        private void grvSeason_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //var selectedValuesSS = string.Join(";", grvSeason.GetSelectedRows().Select(rowHandle => grvSeason.GetRowCellValue(rowHandle, searchLookUpEdit_Season.Properties.ValueMember)));
            //// Thiết lập giá trị EditValue cho control
            //searchLookUpEdit_Season.EditValue = selectedValuesSS;
            //if (searchLookUpEdit_Season.EditValue == null || searchLookUpEdit_Season.EditValue.ToString() == "") return;
            //_Season = searchLookUpEdit_Season.EditValue.ToString();
            //LoadDH();
        }

        private void searchLookUpEdit_Season_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //var selectedValuesSS = string.Join("; ", grvSeason.GetSelectedRows().Select(rowHandle => grvSeason.GetRowCellValue(rowHandle, searchLookUpEdit_Season.Properties.DisplayMember)));
            //if (string.IsNullOrEmpty(selectedValuesSS.ToString()))
            //{
            //    e.DisplayText = "----Chọn mùa----";
            //}
            //else
            //{
            //    e.DisplayText = selectedValuesSS.ToString();
            //}
        }

        private void grvPO_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var selectedValuesPO = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_PO.EditValue = selectedValuesPO;
            if (searchLookUpEdit_PO.EditValue is null || searchLookUpEdit_PO.EditValue.ToString() == "") return;
            _POID = searchLookUpEdit_PO.EditValue.ToString();
            LoadInseam();
           
        }

        private void searchLookUpEdit_PO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuesPO = string.Join("; ", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuesPO.ToString()))
            {
                e.DisplayText = "----Chọn PO----";
            }
            else
            {
                e.DisplayText = selectedValuesPO.ToString();
            }
        }
        private void grvDH_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var selectedValuesDH = string.Join(";", grvDH.GetSelectedRows().Select(rowHandle => grvDH.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_DonHang.EditValue = selectedValuesDH;
            if (searchLookUpEdit_DonHang.EditValue is null || searchLookUpEdit_DonHang.EditValue.ToString() == "") return;
            _DonHang = searchLookUpEdit_DonHang.EditValue.ToString();
            LoadPO();
            //Console.WriteLine(_dausize);
            // GetSLDM();
        }

        private void grvMau_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var selectedValuesMau = string.Join(";", grvMau.GetSelectedRows().Select(rowHandle => grvMau.GetRowCellValue(rowHandle, searchLookUpEdit_Mau.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_Mau.EditValue = selectedValuesMau;
            if (searchLookUpEdit_Mau.EditValue is null || searchLookUpEdit_Mau.EditValue.ToString() == "") return;
            _Mau = searchLookUpEdit_Mau.EditValue.ToString();
            LoadData();
        }

        private void grvSize_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //var selectedValuesSize = string.Join(";", grvSize.GetSelectedRows().Select(rowHandle => grvSize.GetRowCellValue(rowHandle, searchLookUpEdit_Size.Properties.ValueMember)));
            //// Thiết lập giá trị EditValue cho control
            //searchLookUpEdit_Size.EditValue = selectedValuesSize;
            //if (searchLookUpEdit_Size.EditValue != null)
            //    _Size = searchLookUpEdit_Size.EditValue.ToString();
        }

        private void grvDauSize_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var selectedValuesDauSize = string.Join(";", grvDauSize.GetSelectedRows().Select(rowHandle => grvDauSize.GetRowCellValue(rowHandle, searchLookUpEdit_DauSize.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_DauSize.EditValue = selectedValuesDauSize;
            if (searchLookUpEdit_DauSize.EditValue != null)
                _DauSize = searchLookUpEdit_DauSize.EditValue.ToString();
            LoadMau();
        }

        private void searchLookUpEdit_DauSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuesDS = string.Join("; ", grvDauSize.GetSelectedRows().Select(rowHandle => grvDauSize.GetRowCellValue(rowHandle, searchLookUpEdit_DauSize.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuesDS.ToString()))
            {
                e.DisplayText = "----Chọn mùa----";
            }
            else
            {
                e.DisplayText = selectedValuesDS.ToString();
            }
        }

        private void grvDVSX_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var selectedValuesDVSX = string.Join(";", grvDauSize.GetSelectedRows().Select(rowHandle => grvMau.GetRowCellValue(rowHandle, searchLookUpEdit_DauSize.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_DauSize.EditValue = selectedValuesDVSX;
            if (searchLookUpEdit_DauSize.EditValue is null) return;
            _Mau = searchLookUpEdit_DauSize.EditValue.ToString();
            LoadData();
        }

        private void bandedGridViewKHDT_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e,false,false, true);
        }

        private void bandedGridViewKHDT_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e);
        }

        private void searchLookUpEdit_Season1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

        }

        private void bandedGridViewKHDT_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            e.Handled = KHDongThungLib.MergeAllowChangeValue(bandedGridViewKHDT, e);
        }

        private void bandedGridViewKHDT_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var dtKeHoach = dgrKHDongThung.DataSource as DataTable;
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            //if (e.Column.FieldName.Contains("@") || e.Column.FieldName.Contains("SLThung"))
            //{
            //    int sumSL = 0;
            //    foreach (DataColumn dc in drFocus.Table.Columns)
            //    {
            //        if (!dc.ColumnName.Contains('@')) continue;
            //        sumSL += drFocus[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(drFocus[dc.ColumnName]);
            //    }
            //    drFocus["SoLuong"] = sumSL;
            //    if (drFocus["SLThung"].ToString() != "0" || drFocus["SLThung"].ToString() != "")
            //        drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["SLThung"]);
            //    btSave.Enabled = true;
            //}
            //if (e.Column.FieldName == "TrongLuong" || e.Column.FieldName == "KhoiLuong")
            //{
            //    var dt = dgrKHDongThung.DataSource as DataTable;
            //    KHDongThungLib.ProcessChangeNW_GW(dt, drFocus);
            //}
        }

        private void repoQuiCach_EditValueChanged(object sender, EventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            var drfocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drfocus is null) return;
            SearchLookUpEdit sr = sender as SearchLookUpEdit;
            var quicach = sr.EditValue.ToString();
            var dtTemp = dt.AsEnumerable().Where(x => x["SttThung"].ToString() == drfocus["SttThung"].ToString());
            var TrongLuong = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).FirstOrDefault()["TLThung"].ToString();
            var TotalPice = Convert.ToInt32(drfocus["TotalPiece"]);
            var SLThung = Convert.ToInt32(drfocus["SLThung"]);
            var KhoiLuong = Convert.ToDouble(drfocus["KhoiLuong"]);
            var value = Math.Round(KhoiLuong + SLThung * Convert.ToDouble(TrongLuong), 1);

            if (dtTemp.Count() > 0)
            {
                var dtTrung = dtTemp.CopyToDataTable();
                foreach (DataRow dr1 in dtTemp)
                {
                    dr1["KyHieu"] = quicach;
                    dr1["TrongLuong"] = value;
                    dr1["KyHieu"] = quicach;
                }
            }

            //var dtTemp = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).CopyToDataTable();

        }

        private void bandedGridViewKHDT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }

        private void btnLapKH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bandedGridViewKHDT_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            var dr = bandedGridViewKHDT.GetFocusedDataRow();
            SttThungEdit = Convert.ToInt16(dr["SttThung"]);
            TuThungEdit = DenThungEdit = SttThungEdit;


        }

        private void gridviewDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }



        private void gridviewDetail_ShowingEditor(object sender, CancelEventArgs e)
        {

        }



        private void gridviewDetail_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                this.ActiveControl = dgrKHDongThung;
                var dr = gridviewDetail.GetFocusedDataRow();
                if (dr is null) return;
                var SLKH = Convert.ToInt32(dr["SLKH"]);
                var SLDL = Convert.ToInt32(dr["SLDaLapPKL"]);
                var SLLap = Convert.ToInt32(e.Value);
                if (SLLap > (SLKH - SLDL))
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng vượt số lượng cho phép";
                }
            }
            catch (Exception ex)
            {
                e.Valid = false;
            }            
        }

        private void searchLookUpEdit_DonHang_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuesDH = string.Join("; ", grvDH.GetSelectedRows().Select(rowHandle => grvDH.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuesDH.ToString()))
            {
                e.DisplayText = "----Chọn ĐH----";
            }
            else
            {
                e.DisplayText = selectedValuesDH.ToString();
            }
        }

        private void searchLookUpEdit_Mau_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuesMau = string.Join("; ", grvMau.GetSelectedRows().Select(rowHandle => grvMau.GetRowCellValue(rowHandle, searchLookUpEdit_Mau.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuesMau.ToString()))
            {
                e.DisplayText = "----Chọn Màu----";
            }
            else
            {
                e.DisplayText = selectedValuesMau.ToString();
            }
        }

        private void searchLookUpEdit_Size_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //var selectedValuesSize = string.Join("; ", grvSize.GetSelectedRows().Select(rowHandle => grvSize.GetRowCellValue(rowHandle, searchLookUpEdit_Size.Properties.DisplayMember)));
            //if (string.IsNullOrEmpty(selectedValuesSize.ToString()))
            //{
            //    e.DisplayText = "----Chọn Size----";
            //}
            //else
            //{
            //    e.DisplayText = selectedValuesSize.ToString();
            //}
        }

        private void searchLookUpEdit_KhachHang_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_KhachHang.EditValue is null) return;
            _maKH = searchLookUpEdit_KhachHang.EditValue.ToString();
            LoadDH();
        }
        #endregion

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadKhachHang();
        }
        private void CreateBandSize(DataTable dt)
        {
            gbSize.Children.Clear();
            foreach (DataRow drsize in dt.Rows)
            {
                var _sizeID = drsize["SizeID"].ToString();
                var _size = drsize["Size"].ToString();
                if (!CheckExistBand(_sizeID)) continue;
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
                col.Width = 50;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                bandedGridViewKHDT.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
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
                gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private void CreateBandForSize(DataTable dt)
        {
            try
            {
                ClearDataBandAndCol();
                foreach (DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    if (!colName.Contains('@')) continue;
                    var _sizeID = colName.Split('@')[1];
                    var _size = colName.Split('@')[0]; ;
                    if (!CheckExistBand(_sizeID)) continue;
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
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridViewKHDT.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
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
                    gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void ClearDataBandAndCol()
        {
            try
            {
                //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                gbSize.Children.Clear();
                dgrKHDongThung.DataSource = new DataTable();
                List<GridColumn> lst = new List<GridColumn>();
                lst = bandedGridViewKHDT.Columns.Where(x => x.FieldName.Contains(@"Size_")).ToList();
                if (lst != null && lst.Count > 0)
                    foreach (GridColumn gc in lst) bandedGridViewKHDT.Columns.Remove(gc);
            }
            catch (Exception ex) { };
        }
        private bool CheckExistBand(string size)
        {
            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
    }
   
}
