using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
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
    public partial class frmPackageListCheckTon : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        private string _madh = string.Empty, _poid = string.Empty, _maPKL = string.Empty, _maPKLXH = string.Empty;
        string URL = string.Empty;
        private DataTable dtDonHang = new DataTable();
        private DataTable dtPO = new DataTable();
        private DataTable dtSize = new DataTable();
        private DataTable dtPKLTon = new DataTable();
        public static string _donhang = string.Empty;
        KeyDownControlHandler keyDownControlHandler;
        public frmPackageListCheckTon(string maPKLXH = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maPKLXH = maPKLXH;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateDefault();
            LoadDonHang();
            if (_maPKLXH != "") LoadPKLXuatHang();

            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, SaveXuatHang, true, ActionType.Save);
            AddActionControl(_lstActionControl, BtnRefresh, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, BtnXoa, true, ActionType.Delete);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType,true);
            list.Add(actionControl);
        }
       
        private void CreateDefault()
        {
            searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            searchLookUpEdit_DonHang.Properties.DisplayMember = "TenHangDisplay";
            searchLookUpEdit_DonHang.Properties.NullText = "[Chọn đơn hàng]";

            searchLookUpEdit_MaPKL.Properties.ValueMember = "MaPKL";
            searchLookUpEdit_MaPKL.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL.Properties.NullText = "[Chọn PKL]";           

            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn PO]";
           
        }
        private void LoadPKLXuatHang()
        {
            DataTable dtData = new DataTable();
            string url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetCTPhieuTK&Para1={_maPKLXH}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang1 = JsonConvert.DeserializeObject<DataTable>(json);
            url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetCTPhieuTKTon&Para1={_maPKLXH}&Para2=Para");
            json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang2 = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtPKLXuatHang2 is null) dtData = dtPKLXuatHang1;
            else
            {
                dtData = dtPKLXuatHang2;
                dtData.Merge(dtPKLXuatHang1);
            }

            CreateBandSizeXuathang(dtData);
            KHDongThungLib.ProcessSttTrung1(dtData);
            grcPKLXuatHang.DataSource = dtData;
            KHDongThungLib.AllowVieworNotPack(dtData, BandPCBA, BandPackA,BandStoreA);

            //string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXH&Para1={_maPKLXH}&Para2=Para");
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //var dtPKLXuatHang = JsonConvert.DeserializeObject<DataTable>(json);
            //CreateBandSizeXuathang(dtPKLXuatHang);
            //grcPKLXuatHang.DataSource = dtPKLXuatHang;
        }
        private void LoadDonHang()
        {
            try
            {
                string url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetDH&Para1=Para&Para2=Para");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
                var lstTemp = dtDonHang.AsEnumerable().GroupBy(x => new
                {
                    MaDH = x["MaDH"].ToString(),
                    Mahang = x["MaHang"].ToString(),
                    Tenhang = x["TenHang"].ToString(),
                    TenHangDisplay = x["TenHangDisplay"].ToString(),
                    TenKH = x["TenKH"].ToString(),
                    GopDH = x["GopDH"].ToString(),

                }).Select(group => new
                {
                    MaDH = group.Key.MaDH,
                    GopDH = group.Key.GopDH,
                    Mahang = group.Key.Mahang,
                    Tenhang = group.Key.Tenhang,
                    TenHangDisplay = group.Key.TenHangDisplay,
                    TenKH = group.Key.TenKH,
                    SLKH = group.Sum(row => Convert.ToInt32(row["SoLuong"])),
                });
                string jsonT = JsonConvert.SerializeObject(lstTemp);
                DataTable _dtDonHang = JsonConvert.DeserializeObject<DataTable>(jsonT);
                searchLookUpEdit_DonHang.Properties.DataSource = _dtDonHang;
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadPO()
        {
            string url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetPO&Para1={_madh}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtPO = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_PO.Properties.DataSource = dtPO;
        }
        private void LoadMaPKL()
        {
            //if (cbxPO.EditValue is null) return;
            //_poid = dtPO.AsEnumerable().Where(x => x["PO"].ToString() == cbxPO.EditValue.ToString()).FirstOrDefault()["POID"].ToString();
            string url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetMaPKL&Para1={_madh}&Para2={_poid}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtMaPKL = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaPKL.Properties.DataSource = dtMaPKL;
            if (dtMaPKL.Rows.Count > 0)
            {
                searchLookUpEdit_MaPKL.EditValue = null;
                searchLookUpEdit_MaPKL.EditValue = dtMaPKL.Rows[0]["MaPKL"].ToString();
            }
            else
            {
                grcPKLDongThung.DataSource = new DataTable();
            }
        }        
        private void LoadKHDongThung(string controller, string madh, string maPKL)
        {
            if (searchLookUpEdit_MaPKL.EditValue is null) return;
            string url = string.Format("{0}", URL + $"{controller}/Get?Action=GetPivotKHDongThung&MaDH={madh}&POID={madh +"||"+_poid}&SizeTypeID={3}&ColorID=Para&MaPKL={madh +"||"+_poid + "||" +maPKL}");
            //string url = string.Format("{0}", URL + $"{controller}/Get?Action=GetPivotKHDongThung&MaDH={madh}&MaDVSX=Para&DotSX=Para&POID={_poid}&SizeTypeID={3}&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtTemp = JsonConvert.DeserializeObject<DataTable>(json);
            var dtTempA = dtTemp.AsEnumerable().Where(x => KHDongThungLib.CheckRole(x["MaDVSX"].ToString()));
            DataTable dtPivot = dtTempA.Count() > 0 ? dtTempA.CopyToDataTable() : new DataTable();
            CreateBandSize(dtPivot);
            KHDongThungLib.ProcessSttTrung(dtPivot);
            grcPKLDongThung.DataSource = dtPivot;
            KHDongThungLib.AllowVieworNotPack(dtPivot, BandPCB, BandPack,BandStore);
        }
        private void LoadSize()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetSizeA&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtSize = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void GetDataToPKL()
        {
            var dtKHDT = grcPKLDongThung.DataSource as DataTable;
            if (dtKHDT is null || dtKHDT.Rows.Count == 0) return;
            int sttThungOld = 0;
            foreach (DataRow item in dtKHDT.Rows)
            {
                int index = dtKHDT.Rows.IndexOf(item);
                if (Convert.ToInt16(item["SttThung"]) == sttThungOld) item["SLXuat"] = dtKHDT.Rows[index - 1]["SLXuat"];
                else sttThungOld = Convert.ToInt16(item["SttThung"]);
            }
            var tempFilter = dtKHDT.AsEnumerable().Where(x => Convert.ToInt16(x["SLXuat"]) != 0);
            var dtXuatHang = tempFilter.Count() > 0 ? tempFilter.CopyToDataTable() : new DataTable();

            var dtOldXuatHang = grcPKLXuatHang.DataSource as DataTable;
            if (dtOldXuatHang != null)
            {
                foreach (DataRow dr in dtXuatHang.Rows)
                {
                    var drRepeat = dtOldXuatHang.AsEnumerable().Where(x => x["MaPKL"].ToString() == dr["MaPKL"].ToString()
                                                                        && x["MaDH"].ToString() == dr["MaDH"].ToString()
                                                                        && x["POID"].ToString() == dr["POID"].ToString()
                                                                        && x["ID"].ToString() == dr["ID"].ToString());
                    if (drRepeat.Count() > 0) dtOldXuatHang.Rows.Remove(drRepeat.FirstOrDefault());
                }
            }

            foreach (DataRow dr in dtXuatHang.Rows)
            {
                int SLXuat = Convert.ToInt16(dr["SLXuat"]);
                var SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : (Convert.ToInt16(dr["SttDaXuat"]) + Convert.ToInt16(dr["TuThung"]) - 1);
                dr["TuThung"] = SttDaXuat == 0 ? dr["TuThung"] : SttDaXuat + 1;
                dr["DenThung"] = SttDaXuat == 0 ? Convert.ToInt16(dr["TuThung"]) + SLXuat - 1 : SttDaXuat + SLXuat;
                dr["SLThung"] = dr["SLXuat"];
                dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
            }

            if (dtOldXuatHang != null) dtOldXuatHang.Merge(dtXuatHang);
            else dtOldXuatHang = dtXuatHang;
            CreateBandSizeXuathang(dtOldXuatHang);            
            grcPKLXuatHang.DataSource = dtOldXuatHang;
            KHDongThungLib.AllowVieworNotPack(dtOldXuatHang, BandPCBA, BandPackA,BandStoreA);
        }
        private void SaveXuatHang()
        {
            try
            {
                var _dtTempData = grcPKLXuatHang.DataSource as DataTable;

                //var _dtDataTempNew = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() != "1");
                //var _dtDataTempTon = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() == "1");
                //var _dtData = _dtDataTempNew.Count() == 0 ? new DataTable() : _dtDataTempNew.CopyToDataTable();
                //var _dtDataTon = _dtDataTempTon.Count() == 0 ? new DataTable() : _dtDataTempTon.CopyToDataTable();
                List<PackageListCheckTonKhoEntity> listSaveXH = new List<PackageListCheckTonKhoEntity>();
                var _dtData = _dtTempData;
                //DataTable tblSaveXuatHang = CreateTblSaveXuatHang();
                string TenHang = "", PO = "", KhachHang = "";

                PO = _dtData.AsEnumerable().Select(x => x["PO"].ToString()).Distinct().Aggregate((s1, s2) => s1 + "," + s2);
                KhachHang = _dtData.AsEnumerable().Select(x => x["KhachHang"].ToString()).Distinct().Aggregate((s1, s2) => s1 + "," + s2);
                //KhachHang = _dtData.AsEnumerable().Select(x => x["KhachHang"].ToString()).FirstOrDefault();
                TenHang = _dtTempData.AsEnumerable().Select(x => x["TenHang"].ToString()).Distinct().Aggregate((s1, s2) => s1 + "," + s2);

                var distinctDH = _dtData.AsEnumerable().Select(x => new
                {
                    MaDH = x["MaDH"].ToString(),
                    MaHang = x["MaHang"].ToString(),
                    Tenhang = x["Tenhang"].ToString(),
                    KhachHang = x["KhachHang"].ToString()
                }).Distinct().ToList();

                foreach (var itemMaDH in distinctDH)
                {
                    var distinctPO = _dtData.AsEnumerable().Where(x => x["MaDH"].ToString() == itemMaDH.MaDH).Select(y => new
                    {
                        POID = y["POID"].ToString(),
                        PO = y["PO"].ToString()
                    }).Distinct().ToList();
                    foreach (var itemPO in distinctPO)
                    {
                        int carton = _dtTempData.AsEnumerable().Where(y => y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID).Select(z => new
                        {
                            SttThung = z["SttThung"],
                            SLThung = z["SLThung"]
                        }).Distinct().ToList().Sum(x => Convert.ToInt32(x.SLThung));

                        int SLPCS = _dtTempData.AsEnumerable().Where(y => y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID).Select(z => new
                        {
                            SttThung = z["SttThung"],
                            TotalPiece = z["TotalPiece"]
                        }).Distinct().ToList().Sum(x => Convert.ToInt32(x.TotalPiece));
                        //var drNew = tblSaveXuatHang.NewRow();
                        PackageListCheckTonKhoEntity ojSave = new PackageListCheckTonKhoEntity()
                        {
                            Id = 0,
                            MaPKL_TK = _maPKLXH,
                            MaDH = itemMaDH.MaDH,
                            MaHang = itemMaDH.MaHang,
                            KhachHang = itemMaDH.KhachHang,
                            POID = itemPO.POID,
                            PO = itemPO.PO,
                            Carton = carton,
                            SoLuong = SLPCS,
                            CreateDate = DateTime.Now,
                            ExportDate = DateTime.Now,
                            NhanVien = GlobleData.UserName ?? "Test"
                        };
                        listSaveXH.Add(ojSave);
                    }
                }
                string json = JsonConvert.SerializeObject(listSaveXH);
                DataTable tblSaveXuatHang = JsonConvert.DeserializeObject<DataTable>(json);
                DataTable tblSave = KHDongThungLib.CreateTblSave();               
                foreach (DataRow dr in _dtData.Rows)
                {
                    int slThung = Convert.ToInt32(dr["SLThung"].ToString());
                    int TuThung = Convert.ToInt32(dr["TuThung"].ToString());
                    int DenThung = Convert.ToInt32(dr["DenThung"].ToString());
                    int SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : Convert.ToInt16(dr["SttDaXuat"]);
                    int SttThungMin = Convert.ToInt32(dr["SttThungMin"].ToString()) + SttDaXuat;
                    foreach (DataColumn dc in _dtData.Columns)
                    {
                        string colName = dc.ColumnName;
                        if (!colName.Contains("@")) continue;
                        string _sizeID = colName.Split('@')[1];
                        string _size = colName.Split('@')[0];
                        if (dr[colName].ToString() == "" || dr[colName].ToString() == "0") continue;
                        for (int i = 0; i < slThung; i++)
                        {
                            DataRow drNewRow = tblSave.NewRow();
                            drNewRow["ID"] = 0;
                            drNewRow["MaDH"] = dr["MaDH"].ToString();
                            drNewRow["MaPKL"] = dr["MaPKL"].ToString();
                            drNewRow["POID"] = dr["POID"].ToString();
                            drNewRow["ColorID"] = dr["ColorID"].ToString();
                            drNewRow["TenMau"] = dr["TenMau"].ToString();
                            drNewRow["DauSize"] = dr["DauSize"].ToString();
                            drNewRow["DauSizeID"] = dr["DauSizeID"].ToString();
                            drNewRow["SizeID"] = _sizeID;
                            drNewRow["Size"] = _size;
                            drNewRow["TuThung"] = dr["TuThung"].ToString();
                            drNewRow["DenThung"] = dr["DenThung"].ToString();
                            if (dr["Stt_size"].ToString() == "1") drNewRow["SttThung"] = SttThungMin + i;
                            else drNewRow["SttThung"] = SttThungMin + i;
                            tblSave.Rows.Add(drNewRow);
                        }
                    }
                }
               
                if (tblSave == null || tblSave.Rows.Count == 0)
                {
                    MessageBox.Show("PKL xuất hàng dữ liệu đang trống!");
                    return;
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(tblSaveXuatHang);
                ds.Tables.Add(tblSave);               
                string url = string.Format("{0}", URL + "CheckTonKho/Post?action=Save");               
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    _donhang = searchLookUpEdit_DonHang.EditValue is null ? _donhang : searchLookUpEdit_DonHang.EditValue.ToString();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
            }

        }
        private void ClearDataToRefresh()
        {
            var dt = grcPKLXuatHang.DataSource as DataTable;
            grcPKLXuatHang.DataSource = new DataTable();
            if (_maPKLXH == "") return;
            LoadPKLXuatHang();
        }
        private void ResetSLThungLap()
        {
            var check = CheckResetSThung.Checked;
            var dt = grcPKLDongThung.DataSource as DataTable;
            foreach (DataRow dr in dt.Rows)
            {
                if (check)
                    dr["SLXuat"] = 0;
                else dr["SLXuat"] = Convert.ToInt32(dr["SLThung"]) - Convert.ToInt32(dr["SLTDaXuat"]);
            }
        }
        private void DeleteRow(DataRow dr)
        {
            try
            {
                DataTable dtDelete = CreateTblSave();


                int slThung = Convert.ToInt32(dr["SLThung"].ToString());
                int TuThung = Convert.ToInt32(dr["TuThung"].ToString());
                int DenThung = Convert.ToInt32(dr["DenThung"].ToString());
                int SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : Convert.ToInt16(dr["SttDaXuat"]);
                int SttThungMin = Convert.ToInt32(dr["SttThungMin"].ToString()) + SttDaXuat;
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    string colName = dc.ColumnName;
                    if (!colName.Contains("@")) continue;
                    string _sizeID = colName.Split('@')[1];
                    string _size = colName.Split('@')[0];
                    if (dr[colName].ToString() == "" || dr[colName].ToString() == "0") continue;
                    for (int i = 0; i < slThung; i++)
                    {
                        DataRow drNewRow = dtDelete.NewRow();
                        drNewRow["ID"] = 0;
                        drNewRow["MaDH"] = dr["MaDH"].ToString();
                        drNewRow["MaPKL"] = dr["MaPKL"].ToString();
                        drNewRow["POID"] = dr["POID"].ToString();
                        drNewRow["ColorID"] = dr["ColorID"].ToString();
                        drNewRow["TenMau"] = dr["TenMau"].ToString();
                        drNewRow["DauSize"] = dr["DauSize"].ToString();
                        drNewRow["DauSizeID"] = dr["DauSizeID"].ToString();
                        drNewRow["SizeID"] = _sizeID;
                        drNewRow["Size"] = _size;
                        drNewRow["TuThung"] = dr["TuThung"].ToString();
                        drNewRow["DenThung"] = dr["DenThung"].ToString();
                        if (dr["Stt_size"].ToString() == "1") drNewRow["SttThung"] = SttThungMin + i;
                        else drNewRow["SttThung"] = TuThung + i;
                        dtDelete.Rows.Add(drNewRow);
                    }
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dtDelete);
                string action = "";
                if ((bool)dr["IsTon"]) action = "DeletePKLXH_Row_Ton";
                else action = "DeletePKLXH_Row";
                string url = string.Format("{0}", URL + $"XuatHang/Delete?action={action}&Para={_maPKLXH}");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
            }
            catch (Exception ex)
            {

            }

        }
        private void CreateBandSize(DataTable dt)
        {
            ClearBand();
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
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
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                bandgrvDongThung.Columns.AddRange(new BandedGridColumn[] { col });
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
        private void ClearBand()
        {
            gbSize.Children.Clear();
        }
        private bool CheckExistBand(string size)
        {

            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }     

        #region Event DongThung
        private void bandgrvDongThung_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "SLTDaXuat" || e.Column.FieldName == "SLXuat" || e.Column.FieldName == "SLNhapKho") return;
            if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }
        private void bandgrvDongThung_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                this.ActiveControl = grcPKLXuatHang;
                var dr = bandgrvDongThung.GetFocusedDataRow();
                if (dr is null) return;
                int TongSLThung = Convert.ToInt16(dr["SLNhapKho"]);
                int SLThungDX = Convert.ToInt16(dr["SLTDaXuat"]);
                int SLThungXuat = Convert.ToInt16(e.Value);
                if (SLThungXuat + SLThungDX > TongSLThung)
                {
                    e.Valid = false;
                    e.ErrorText = "SL Thùng vượt SL thùng cho phép!";
                }
            }
            catch (Exception ex)
            {
                e.Valid = false;
                e.ErrorText = "Vui lòng nhập đúng cú pháp!";
            }

        }
        private void bandgrvDongThung_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "SLXuat")
            {
                // Perform your condition to determine if cells should be merged
                if (e.RowHandle > 0)
                {
                    var preSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, "SttThung");
                    var curSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle, "SttThung");
                    object prevValue = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, e.Column);
                    object currValue = bandgrvDongThung.GetRowCellValue(e.RowHandle, e.Column);

                    if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            else if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung" || e.Column.FieldName == "SLXuat" || e.Column.FieldName == "SLTDaXuat")
            {
                var preSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, "SttThung");
                var curSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle, "SttThung");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                {
                    e.Handled = true;
                    return;
                }
            }

            e.Handled = false;
        }
        private void btnGetData_Click(object sender, EventArgs e)
        {
            GetDataToPKL();
        }
        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveXuatHang();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearDataToRefresh();
        }
        private void CheckResetSThung_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetSLThungLap();
        }
        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        private void bandgrvXuatHang_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }
        private void bandgrvXuatHang_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = bandgrvXuatHang.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            DXMenuItem menuDelete = new DXMenuItem();
            menuDelete.Caption = "Xóa";
            menuDelete.Click += MenuDelete_Click;
            DXMenuItem menuCopyandCreate = new DXMenuItem();
            menuCopyandCreate.Caption = "Copy và tạo dòng mới";
            menuCopyandCreate.Click += MenuCopyandCreate_Click;
            e.Menu.Items.Add(menuCopyandCreate);
            e.Menu.Items.Add(menuDelete);
        }
        private void bandgrvXuatHang_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "SLThung")
            {
                // Perform your condition to determine if cells should be merged
                if (e.RowHandle > 0)
                {
                    var preSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle - 1, "SttThung");
                    var curSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle, "SttThung");
                    object prevValue = bandgrvXuatHang.GetRowCellValue(e.RowHandle - 1, e.Column);
                    object currValue = bandgrvXuatHang.GetRowCellValue(e.RowHandle, e.Column);

                    if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            else if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
            {
                var preSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle - 1, "SttThung");
                var curSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle, "SttThung");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                {
                    e.Handled = true;
                    return;
                }
            }

            e.Handled = false;
        }

        private void MenuCopyandCreate_Click(object sender, EventArgs e)
        {
            var drCopy = bandgrvXuatHang.GetFocusedDataRow();
            DataTable tbl = grcPKLXuatHang.DataSource as DataTable;
            if (tbl.Columns.Count == 0) return;
            DataRow drAdd = tbl.NewRow();
            drAdd["MaDH"] = drCopy["MaDH"];
            drAdd["MaPKL"] = drCopy["MaPKL"];
            drAdd["POID"] = drCopy["POID"];
            drAdd["PO"] = drCopy["PO"];
            drAdd["MaHang"] = drCopy["MaHang"];
            drAdd["MaDVSX"] = drCopy["MaDVSX"];
            drAdd["TenDVSX"] = drCopy["TenDVSX"];
            drAdd["MaLenh"] = drCopy["MaLenh"];
            drAdd["DotSX"] = drCopy["DotSX"];
            drAdd["DauSizeID"] = drCopy["DauSizeID"];
            drAdd["DauSize"] = drCopy["DauSize"];
            drAdd["ColorID"] = drCopy["ColorID"];
            drAdd["TenMau"] = drCopy["TenMau"];
            drAdd["SoLuong"] = 0;
            drAdd["SLThung"] = 0;
            drAdd["TotalPiece"] = 0;
            drAdd["KyHieu"] = "(L x W x H)";
            drAdd["IsSave"] = false;
            drAdd["Chon"] = false;
            tbl.Rows.Add(drAdd);
            grcPKLXuatHang.RefreshDataSource();
        }
        private void MenuDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow drRemove = bandgrvXuatHang.GetFocusedDataRow();
                DataTable tblPivot = grcPKLXuatHang.DataSource as DataTable;
                DataTable tblDelete = tblPivot.Clone();
                if (drRemove == null) return;
                if (_maPKLXH == "")
                {
                    int index = tblPivot.Rows.IndexOf(drRemove);
                    tblPivot.Rows.RemoveAt(index);
                    grcPKLXuatHang.RefreshDataSource();
                }
                else
                {
                    DialogResult resultDialog = DialogResult.None;
                    resultDialog = MessageBox.Show("Xác nhận xóa dữ liệu?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resultDialog == DialogResult.Yes)
                    {
                        int index = tblPivot.Rows.IndexOf(drRemove);
                        DeleteRow(drRemove);
                        tblPivot.Rows.RemoveAt(index);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void searchLookUpEdit_DonHang_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_DonHang.EditValue is null) return;
            _madh = searchLookUpEdit_DonHang.EditValue.ToString();
            LoadSize();
            ClearBand();
            LoadPO();
            //dtPO = dtDonHang.AsEnumerable().Where(x => x["MaDH"].ToString() == searchLookUpEdit_DonHang.EditValue.ToString()).CopyToDataTable();
            //cbxPO.Properties.Items.Clear();
            //foreach (DataRow dr in dtPO.Rows)
            //{
            //    cbxPO.Properties.Items.Add(dr["PO"]);
            //}
            //if (dtPO.Rows.Count > 0)
            //{
            //    cbxPO.EditValue = null;
            //    cbxPO.EditValue = dtPO.Rows[0]["PO"];
            //}
        }
        private void searchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_PO.EditValue is null) return;
            _poid = searchLookUpEdit_PO.EditValue.ToString();
            LoadMaPKL();
        }       
        private void searchLookUpEdit_MaPKL_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MaPKL.EditValue is null) return;
            _maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
            LoadKHDongThung("KeHoachDongThung", _madh, _maPKL);
            //searchLookUpEdit_MaPKL_Ton.EditValue = null;
        } 
        #endregion
        private void CreateBandSizeXuathang(DataTable dt)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0]; ;
                if (!CheckExistBandXuatHang(_sizeID)) continue;
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
                bandgrvXuatHang.Columns.AddRange(new BandedGridColumn[] { col });
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
                gbSizeXuatHang.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }     

        private bool CheckExistBandXuatHang(string size)
        {

            GridBand gbCheck = gbSizeXuatHang.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPKL", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaLenh", typeof(string));
            tbl.Columns.Add("DotSX", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("ColorID", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("NgayLapKH", typeof(DateTime));
            tbl.Columns.Add("ChieuDai", typeof(double));
            tbl.Columns.Add("ChieuRong", typeof(double));
            tbl.Columns.Add("ChieuCao", typeof(double));
            tbl.Columns.Add("TrongLuong", typeof(double));
            tbl.Columns.Add("KhoiLuong", typeof(double));
            tbl.Columns.Add("SoLuongThung", typeof(int));
            tbl.Columns.Add("TuThung", typeof(int));
            tbl.Columns.Add("DenThung", typeof(int));
            tbl.Columns.Add("SttThung", typeof(int));
            tbl.Columns.Add("SoLuongSP", typeof(int));
            tbl.Columns.Add("IsDongThung", typeof(bool));
            tbl.Columns.Add("NgayDongThung", typeof(DateTime));
            tbl.Columns.Add("QRCode", typeof(string));
            tbl.Columns.Add("IsScan", typeof(bool));
            tbl.Columns.Add("IsNhapKho", typeof(bool));
            tbl.Columns.Add("NgayNhapKho", typeof(DateTime));
            tbl.Columns.Add("KyHieu", typeof(string));
            tbl.Columns.Add("Chon", typeof(bool));
            tbl.Columns.Add("SttThung_decat", typeof(int));
            tbl.Columns.Add("IsThungLe", typeof(bool));
            return tbl;
        }

        

        private DataTable CreateTblSaveXuatHang()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("Id", typeof(int));
            tbl.Columns.Add("MaPKL_XH", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("KhachHang", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("Carton", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(string));
            tbl.Columns.Add("VCCont", typeof(string));
            tbl.Columns.Add("VCTai", typeof(string));
            tbl.Columns.Add("Cont", typeof(string));
            tbl.Columns.Add("Romoc", typeof(string));
            tbl.Columns.Add("Seal", typeof(string));
            tbl.Columns.Add("Consignee", typeof(string));
            tbl.Columns.Add("InVoice", typeof(string));
            tbl.Columns.Add("TenTaiXe", typeof(string));
            tbl.Columns.Add("CMND", typeof(string));
            tbl.Columns.Add("Congty", typeof(string));
            tbl.Columns.Add("Shipper", typeof(string));
            tbl.Columns.Add("Image", typeof(string));
            tbl.Columns.Add("Video", typeof(string));
            tbl.Columns.Add("PackDate", typeof(DateTime));
            tbl.Columns.Add("FinishDate", typeof(DateTime));
            tbl.Columns.Add("CreateDate", typeof(DateTime));
            tbl.Columns.Add("ExportDate", typeof(DateTime));
            tbl.Columns.Add("IsExport", typeof(bool));
            tbl.Columns.Add("Dot", typeof(int));
            tbl.Columns.Add("NhanVien", typeof(string));
            return tbl;
        }

      
        #region Manh
        private void bandgrvDongThung_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender,e);
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bandgrvDongThung_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = grcPKLDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
        }
       

        private void bandgrvXuatHang_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            var dt = grcPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e,true);
        }
       
        private void bandgrvXuatHang_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e,true);
        }
        private void BtnXoa()
        {

        }
        private void BtnRefresh()
        {

        }
        #endregion
    }
}
