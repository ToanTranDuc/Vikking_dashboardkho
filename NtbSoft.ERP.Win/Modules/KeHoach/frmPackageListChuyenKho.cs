using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
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
    public partial class frmPackageListChuyenKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        private string _madh = string.Empty, _poid = string.Empty, _maPKL = string.Empty, _maPKLXH = string.Empty, _maKH = string.Empty, _tenHang = string.Empty,
                       _khachHang = string.Empty, _po = string.Empty, _oldCont = string.Empty, _oldSeal = string.Empty, _seal = string.Empty, _tenSeal = string.Empty;
        string URL = string.Empty;
        public static string _donhang = string.Empty;
        private DataTable dtDonHang = new DataTable();
        private DataTable dtPO = new DataTable();
        private DataTable dtSize = new DataTable();
        private DataTable dtKho = new DataTable();
        private DataTable dtPKL_DongThung = new DataTable();
        private DataTable dtPKLTon = new DataTable();
        private DataTable dtSeal = new DataTable();
        private DataTable dtCont = new DataTable();
        DataTable tblCopy = new DataTable();
        KeyDownControlHandler keyDownControlHandler;
        private bool IsEdit = false;
        public frmPackageListChuyenKho(string maPKLXH = "", string maKH = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maPKLXH = maPKLXH;
            _maKH = maKH;
            IsEdit = maPKLXH == "" ? false : true;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            //keyDownControlHandler = new KeyDownControlHandler();
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GetKho();
            CreateDefault();
            LoadKhachHang();
            LoadSeal();
            LoadCont();
            //LoadDonHang();
            if (_maPKLXH != "") LoadPKLXuatHang();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, SaveXuatHang, true, ActionType.Save);
            AddActionControl(_lstActionControl, BtnRefresh, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, BtnDelete, true, ActionType.Delete);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void CreateDefault()
        {
            searchLookUpEdit_KhachHang.Properties.ValueMember = "MaKH";
            searchLookUpEdit_KhachHang.Properties.DisplayMember = "TenKH";
            searchLookUpEdit_KhachHang.Properties.NullText = "[Chọn KH]";

            searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            searchLookUpEdit_DonHang.Properties.DisplayMember = "TenHangDisplay";
            searchLookUpEdit_DonHang.Properties.NullText = "[Chọn đơn hàng]";

            searchLookUpEdit_MaPKL.Properties.ValueMember = "Value";
            searchLookUpEdit_MaPKL.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL.Properties.NullText = "[Chọn PKL]";

            searchLookUpEdit_MaPKL_Ton.Properties.ValueMember = "MaPKLValue";
            searchLookUpEdit_MaPKL_Ton.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL_Ton.Properties.NullText = "[Chọn PKL]";

            searchLookUpEdit_Seal.Properties.ValueMember = "MaSeal";
            searchLookUpEdit_Seal.Properties.DisplayMember = "Seal";
            searchLookUpEdit_Seal.Properties.NullText = "[Chọn Seal]";

            searchLookUpEdit_Cont.Properties.ValueMember = "MaCont";
            searchLookUpEdit_Cont.Properties.DisplayMember = "TenCont";
            searchLookUpEdit_Cont.Properties.NullText = "[Chọn Cont]";


            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.ShowClearButton = false;
            searchLookUpEdit_PO.Properties.NullText = "[Chọn PO]";
            GridView dvViewPO = searchLookUpEdit_PO.Properties.View;
            if (dvViewPO.Columns.Count == 0)
            {
                dvViewPO.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewPO.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewPO.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewPO.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewPO.Columns.Add(new GridColumn { FieldName = "POID", Caption = "POID", Name = "colPOID", Visible = false });
                dvViewPO.Columns.Add(new GridColumn { FieldName = "PO", Caption = "PO", Name = "colPO", Visible = true });
                dvViewPO.Columns.Add(new GridColumn { FieldName = "SLXuatHangT", Caption = "SL tổng", Name = "colSlTong", Visible = true });
                dvViewPO.Columns.Add(new GridColumn { FieldName = "SLXuatHang", Caption = "SL ĐX", Name = "colSLSX", Visible = true });
                dvViewPO.Columns.Add(new GridColumn { FieldName = "SLConLai", Caption = "Còn lại", Name = "colConLai", Visible = true });
            }


            repoSearchSeal.ValueMember = "MaSeal";
            repoSearchSeal.DisplayMember = "Seal";
            repoSearchSeal.NullText = "[Chọn Seal]";
            GridView dvView = repoSearchSeal.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaSeal", Caption = "MaSeal", Name = "colMaSealA", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "Seal", Caption = "Seal", Name = "SealA", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TinhTrang", Caption = "Tình trạng", Name = "colTinhTrangA", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "NgayLap", Caption = "Ngày lập", Name = "colNgayLapA", Visible = true });
            }
            //searchLookUpEdit_PO.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditPO_CustomDisplayText);
            searchLookUpEdit_PO.CustomDisplayText += SearchLookUpEdit_PO_CustomDisplayText;
            // dvViewPO.SelectionChanged += DvViewPO_SelectionChanged;
            repoSearch_Cont.ValueMember = "MaCont";
            repoSearch_Cont.DisplayMember = "TenCont";
            repoSearch_Cont.NullText = "[Chọn Cont]";
            GridView dvViewCont = repoSearch_Cont.View;
            if (dvViewCont.Columns.Count == 0)
            {
                dvViewCont.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewCont.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewCont.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewCont.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewCont.Columns.Add(new GridColumn { FieldName = "MaCont", Caption = "Mã cont", Name = "colMaContA", Visible = false });
                dvViewCont.Columns.Add(new GridColumn { FieldName = "TenCont", Caption = "Tên cont", Name = "cContA", Visible = true });

            }
        }
        private void LoadKhachHang()
        {
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_KhachHang.Properties.DataSource = dt;
            if (_maKH != "")
            {
                searchLookUpEdit_KhachHang.EditValue = _maKH;
            }
        }
        private void LoadSeal()
        {
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetSeal&Para1=Para&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtSeal = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Seal.Properties.DataSource = dtSeal;
            repoSearchSeal.DataSource = dtSeal;
        }
        private void LoadCont()
        {
            string url = string.Format("{0}?Action={1}", URL + "MaCont/Get", "GET");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtCont = JsonConvert.DeserializeObject<DataTable>(json);

            if (dtCont == null && dtCont.Rows.Count == 0) return;
            searchLookUpEdit_Cont.Properties.DataSource = dtCont;
            repoSearch_Cont.DataSource = dtCont;
        }
        private void LoadPKLXuatHang()
        {
            DataTable dtData = new DataTable();
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetCTPhieuXH&Para1={_maPKLXH}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang1 = JsonConvert.DeserializeObject<DataTable>(json);
            url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetCTPhieuXHTon&Para1={_maPKLXH}&Para2=Para");
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
            dtData = KHDongThungLib.CalculatorTB(dtData);
            grcPKLXuatHang.DataSource = dtData;
            tblCopy = dtData;
            if (dtData.Rows.Count > 0)
            {
                var dr0 = dtData.Rows[0];
                cbxTuKho.EditValue = dr0["TenDVSX_NK"].ToString();
                cbxDenKho.EditValue = dr0["DenKho"].ToString();
            }

        }
        private void LoadDonHang()
        {
            try
            {
                string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetDH&Para1={searchLookUpEdit_KhachHang.EditValue.ToString()}&Para2=Para");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
                var lstTemp = dtDonHang.AsEnumerable().GroupBy(x => new
                {
                    MaDH = x["MaDH"].ToString(),
                    GopDH = x["GopDH"].ToString(),
                    TenHang = x["TenHang"].ToString(),
                    TenKH = x["TenKH"].ToString(),
                    TenHangDisplay = x["TenHangDisplay"].ToString(),

                }).Select(group => new
                {
                    MaDH = group.Key.MaDH,
                    GopDH = group.Key.GopDH,
                    TenHang = group.Key.TenHang,
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
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetPO&Para1={_madh}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtPO = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_PO.Properties.DataSource = dtPO;
            if (dtPO.Rows.Count == 0) return;
            //searchLookUpEdit_PO.EditValue = null;
            //searchLookUpEdit_PO.EditValue = dtPO.Rows[0]["POID"];
        }
        DataTable dtMaPKL = new DataTable();
        private void LoadMaPKL()
        {
            //if (cbxPO.EditValue is null) return;
            //_poid = dtPO.AsEnumerable().Where(x => x["PO"].ToString() == cbxPO.EditValue.ToString()).FirstOrDefault()["POID"].ToString();
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetMaPKL&Para1={_madh}&Para2={_poid}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtMaPKL = JsonConvert.DeserializeObject<DataTable>(json);
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
        private void LoadMaPKLTon()
        {
            if (dtDonHang is null || dtDonHang.Rows.Count == 0) return;
            var lstMaDH = _madh.Split(';');
            //_poid = dtPO.AsEnumerable().Where(x => x["PO"].ToString() == cbxPO.EditValue.ToString()).FirstOrDefault()["POID"].ToString();
            var _maHang = dtDonHang.AsEnumerable().Where(x => lstMaDH.Contains(x["MaDH"].ToString())).FirstOrDefault()["MaHang"].ToString();
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetMaPKLTon&Para1={_maHang}&Para2={_poid}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtPKLTon = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaPKL_Ton.Properties.DataSource = dtPKLTon;
            searchLookUpEdit_MaPKL_Ton.EditValue = null;
        }
        private void LoadKHDongThung(string controller, string madh, string maPKL, string poid, string TonNew, string IsTonKho)
        {
            //if (searchLookUpEdit_MaPKL.EditValue is null) return;
            // _maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
            string url = string.Format("{0}", URL + $"{controller}/Get?Action=GetPivotKHDongThung&MaDH={madh}&MaDVSX=Para&DotSX=Para&POID={poid}&SizeTypeID={2}&ColorID={IsTonKho}&ProductID=Para&SizeID=Para&MaPKL={maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPivot = JsonConvert.DeserializeObject<DataTable>(json);
            foreach (DataRow dr in dtPivot.Rows)
            {
                dr["KyHieu"] = dr["KyHieuA"];
            }
            CreateBandSize(dtPivot);
            KHDongThungLib.ProcessSttTrung(dtPivot);
            dtPKL_DongThung = dtPivot;
            grcPKLDongThung.DataSource = dtPivot;
            GetTuKho(dtPivot);
        }
        private void LoadKHDongThungV2(string controller, string madh, string maPKL, string poid, string TonNew, string IsTonKho)
        {
            string url = string.Format("{0}", URL + $"{controller}/Get?Action=GetPivotKHDongThung&MaDH=Para&POID={poid}&SizeTypeID={2}&ColorID={IsTonKho}&MaPKL={maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var dtPivot = JsonConvert.DeserializeObject<DataTable>(json);
            CreateBandSize(dtPivot);
            KHDongThungLib.ProcessSttTrung1(dtPivot);
            ProcessDataWhenReloadPKL(dtPivot);
            dtPKL_DongThung = dtPivot;
            dtPivot = KHDongThungLib.CalculatorTB(dtPivot);

            grcPKLDongThung.DataSource = dtPivot;
            GetTuKho(dtPivot);
            KHDongThungLib.AllowVieworNotPack(dtPivot, BandPCB, BandPack, BandStore);
        }
        private void LoadSize()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetSizeA&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtSize = JsonConvert.DeserializeObject<DataTable>(json);
        }
        List<rangeList> lstRangeTable = new List<rangeList>();
        public void rangeIntTbale(DataTable tbl)
        {
            try
            {
                lstRangeTable.Clear();
                lstRangeTable = new List<rangeList>();
                int indexSttThung = 0;
                int indexRow = 0;
                int batDau = 0;
                int ketThuc = 0;
                foreach (DataRow item in tbl.Rows)
                {
                    int indexCurrent = Convert.ToInt32(item["Sttthung"]) + 1;
                    int indenext = indexSttThung + 2 > tbl.Rows.Count ? 123131 : Convert.ToInt32(tbl.Rows[indexSttThung + 1]["Sttthung"]);
                    if (indexCurrent != indenext)
                    {
                        batDau = Convert.ToInt32(tbl.Rows[indexSttThung - indexRow]["Sttthung"]);
                        ketThuc = Convert.ToInt32(item["Sttthung"]);
                        lstRangeTable.Add(new rangeList
                        {
                            batDau = batDau,
                            KetThuc = ketThuc
                        });
                        indexRow = -1;
                    }


                    indexSttThung++;
                    indexRow++;
                }
            }
            catch (Exception ex)
            {

            }

        }
        bool isheckFlag = true;
        private void GetDataToPKL()
        {
            var dtKHDT = grcPKLDongThung.DataSource as DataTable;
            int sttThungOld = 0;
            if (dtKHDT.Rows.Count == 0) return;
            var sumCheckSLChuyen = dtKHDT.AsEnumerable().Sum(x => Convert.ToInt32(x["SLChuyen"].ToString() == "" ? 0 : x["SLChuyen"]));
            var sumCheckSLNhapKho = dtKHDT.AsEnumerable().Sum(x => Convert.ToInt32(x["SLNhapKho"].ToString() == "" ? 0 : x["SLNhapKho"]));
            if (sumCheckSLNhapKho == 0)
            {
                MessageBox.Show("PKL chưa nhập kho!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (sumCheckSLChuyen == 0)
            {
                MessageBox.Show("Vui lòng kiểm tra số lượng xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (searchLookUpEdit_Cont.EditValue is null)
            {
                MessageBox.Show("Vui lòng chọn Cont!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //if (txtCont.Text == "")
            //{
            //    MessageBox.Show("Vui lòng nhập số xe!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            var CheckIsTon = dtKHDT.Rows[0]["IsTon"].ToString();
            if (CheckIsTon != "0")
            {
                var lstCheckPO = _poid.Split(';');
                var lstCheckDH = _madh.Split(';');
                if (lstCheckDH.Length > 1)
                {
                    MessageBox.Show("Vui lòng chỉ chọn 1 đơn hàng để xuất cho PKL tồn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (lstCheckPO.Length > 1)
                {
                    MessageBox.Show("Vui lòng chỉ chọn 1 PO để xuất cho PKL tồn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult result = MessageBox.Show($"Xác nhận xuất PKL tồn kho cho đơn hàng PO: {_poid}", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
                foreach (DataRow dr in dtKHDT.Rows)
                {
                    dr["POIDTemp"] = _poid.Split(new string[] { "||" }, StringSplitOptions.None)[1];
                    var drPO = dtPO.AsEnumerable().Where(x => x["POID"].ToString() == _poid).FirstOrDefault();
                    var drDH = dtDonHang.AsEnumerable().Where(x => x["MaDH"].ToString() == _madh).FirstOrDefault();
                    dr["PO_XH"] = drPO["PO"].ToString();
                    dr["MaHang"] = drDH["MaHang"].ToString();
                    dr["TenHang"] = drDH["TenHang"].ToString();
                    dr["MaKH"] = drDH["MaKH"].ToString();
                    dr["KhachHang"] = drDH["TenKH"].ToString();
                }
            }
            foreach (DataRow item in dtKHDT.Rows)
            {
                int index = dtKHDT.Rows.IndexOf(item);
                var drPre = dtKHDT.Rows[index == 0 ? 0 : index - 1];
                if (Convert.ToInt16(item["SttThung"]) == sttThungOld && drPre["MaPKLDisplay"].ToString() == item["MaPKLDisplay"].ToString()) item["SLChuyen"] = dtKHDT.Rows[index - 1]["SLChuyen"];
                else sttThungOld = Convert.ToInt16(item["SttThung"]);
                item["Seal"] = _tenSeal;
                item["Cont"] = searchLookUpEdit_Cont.EditValue.ToString();
                item["POID_XH"] = item["IsTon"].ToString() == "0" ? item["POID_G"] : item["POIDTemp"];
                item["PO_XH"] = item["IsTon"].ToString() == "0" ? item["PO_G"] : item["PO_XH"];
                item["MaDH_XH"] = item["IsTon"].ToString() == "0" ? item["MaDH"] : _madh;
            }
            var tempFilter = dtKHDT.AsEnumerable().Where(x => Convert.ToInt16(x["SLChuyen"]) != 0);

            var dtXuatHang = tempFilter.Count() > 0 ? tempFilter.CopyToDataTable() : new DataTable();
            var colTemp = dtXuatHang.Columns.Add("SLChuyenTemp", typeof(int));
            colTemp.DefaultValue = 0;
            var dtOldXuatHang = grcPKLXuatHang.DataSource as DataTable;
            if (dtOldXuatHang != null && dtOldXuatHang.Rows.Count > 0)
            {
                var maKhoOld = dtOldXuatHang.Rows[0]["MaDVSX"].ToString();
                var tempdtCheck = dtXuatHang.AsEnumerable().Where(x => x["MaDVSX"].ToString() != maKhoOld);
                if (tempdtCheck.Count() > 0)
                {
                    MessageBox.Show("Đã trùng đơn vị kho. Vui lòng kiểm tra lại!");
                    return;
                }
                foreach (DataRow dr in dtXuatHang.Rows)
                {
                    var drRepeat = dtOldXuatHang.AsEnumerable().Where(x => x["MaPKL"].ToString() == dr["MaPKL"].ToString()
                                                                        && x["MaDH"].ToString() == dr["MaDH"].ToString()
                                                                        && x["POID"].ToString() == dr["POID"].ToString()
                                                                        && x["ID"].ToString() == dr["ID"].ToString()
                                                                        && x["IsTon"].ToString() == dr["IsTon"].ToString()
                                                                        && x["Cont"].ToString() == dr["Cont"].ToString());
                    //if (drRepeat.Count() > 0) {
                    //    var SLChuyen = Convert.ToInt16(dr["SLChuyen"]) + Convert.ToInt16(drRepeat.FirstOrDefault()["SLChuyen"]);
                    //    //var SLToiDaChuyen = Convert.ToInt32(dr["SLNhapKho"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTXuatHang_CC"]);
                    //    dr["SLChuyenTemp"] = Convert.ToInt16(dr["SLChuyen"]);
                    //    dr["SLChuyen"] = SLChuyen;

                    //    //dtOldXuatHang.Rows.Remove(drRepeat.FirstOrDefault());
                    //}
                }
            }

            //foreach (DataRow dr in dtXuatHang.Rows)
            //{
            //    int SLXuat = Convert.ToInt16(dr["SLChuyen"]);
            //    var SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : (Convert.ToInt16(dr["SttDaXuat"]) + Convert.ToInt16(dr["TuThung"]) - 1);
            //    dr["TuThung"] = SttDaXuat == 0 ? dr["TuThung"] : SttDaXuat + 1;
            //    dr["DenThung"] = SttDaXuat == 0 ? Convert.ToInt16(dr["TuThung"]) + SLXuat - 1 : SttDaXuat + SLXuat;
            //    dr["SLThung"] = dr["SLChuyen"];
            //    dr["SLChuyenTemp"] = dr["SLChuyenTemp"].ToString() == "" ? Convert.ToInt16(dr["SLChuyen"]) : dr["SLChuyenTemp"];
            //    dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
            //}
            DataTable tblPKL = grcPKLXuatHang.DataSource as DataTable;
            DataTable tblDown = new DataTable();
            List<DataRow> newRows = new List<DataRow>();
            for (int i = 0; i < dtXuatHang.Rows.Count; i++)
            {
                DataRow dr = dtXuatHang.Rows[i];
                try
                {
                    if (tblPKL != null)
                    {
                        var tblpkl1 = tblPKL.AsEnumerable().Where(x => Convert.ToInt32(x["ID"]) == Convert.ToInt32(dr["ID"])
                                                                        && x["MaPKL_Chuyen"].ToString() == ""
                                                                         && x["MaDH"].ToString() == dr["MaDH"].ToString()
                                                                         && x["MaPKL"].ToString() == dr["MaPKL"].ToString()
                                                                         && x["POID"].ToString() == dr["POID"].ToString()).ToList();
                        if (tblpkl1.Any())
                            tblDown = tblpkl1.CopyToDataTable();
                    }
                    List<int> numbers = new List<int>();
                    foreach (DataRow row in tblDown.Rows)
                    {
                        int start = Convert.ToInt32(row["Tuthung"]);
                        int end = Convert.ToInt32(row["Denthung"]);
                        for (int x = start; x <= end; x++)
                        {
                            numbers.Add(x);
                        }
                    }

                    int sttthungmin = Convert.ToInt32(dr["SttThungMin"]) + (isheckFlag == true ? Convert.ToInt32(dr["TuThung"]) : Convert.ToInt32(dr["TuThungX"])) - Convert.ToInt32(dr["TuThung"]);
                    int sttthungmax = (sttthungmin + (isheckFlag == true ? Convert.ToInt32(dr["DenThung"]) : Convert.ToInt32(dr["DenThungX"])) - (isheckFlag == true ? Convert.ToInt32(dr["TuThung"]) : Convert.ToInt32(dr["TuThungX"])));
                    string madh = dr["MaDH"].ToString();
                    string poiD = dr["POID"].ToString();
                    string maPKL1 = dr["MaPKL"].ToString();
                    string url = $"{URL}KeHoachDongThung/Get?Action=GetISNK&MaDH={madh}&POID={poiD}&SizeTypeID={sttthungmin.ToString()}&ColorID={sttthungmax.ToString()}&MaPKL={maPKL1}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (numbers.Count > 0)
                    {
                        foreach (DataRow row in tbl.Rows)
                        {
                            int number = Convert.ToInt32(row["SttThung"]);
                            if (numbers.Contains(number))
                            {
                                row["IsXuatHang"] = true;
                            }
                        }
                    }
                    string url1 = $"{URL}KeHoachDongThung/Get?Action=GetISNK&MaDH={madh}&POID={poiD}&SizeTypeID={dr["TuThung"].ToString()}&ColorID={dr["DenThung"].ToString()}&MaPKL={maPKL1}";
                    string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                    DataTable tbl5 = JsonConvert.DeserializeObject<DataTable>(json1);

                    List<int> tt1 = tbl.AsEnumerable()
                        .Where(x => Convert.ToBoolean(x["IsXuatHang"]) == true)
                        .Select(x => Convert.ToInt32(x["SttThung"]))
                        .ToList();
                    DataTable tbl6 = new DataTable();
                    var table5 = tbl5.AsEnumerable().Where(x => (bool)x["IsXuatHang"] == true).ToList();
                    if (table5.Any())
                        tbl6 = table5.CopyToDataTable();

                    if (tt1 == null || tt1.Count == 0)
                    {
                        int SLXuat = Convert.ToInt16(dr["SLChuyen"]);// Convert.ToInt32(dr["SLDaChuyenKho"]) != 0 ? Convert.ToInt16(dr["SLDaChuyenKho"]) : Convert.ToInt16(dr["SLChuyen"]);
                        var SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : (Convert.ToInt16(dr["SttDaXuat"]) + Convert.ToInt16(dr["TuThung"]) - 1);
                        dr["TuThung"] = dr["TuThungX"];
                        dr["DenThung"] = dr["DenThungX"];
                        dr["SLThung"] = SLXuat;
                        dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
                        dr["SLChuyenTemp"] = Convert.ToInt32(dr["SLDaChuyenKho"]) + SLXuat;
                        dr["SttThungMin"] = sttthungmin;
                    }
                    else
                    {
                        DataTable tbl1 = null;
                        DataTable tbl2 = null;
                        var trueRows = tbl.AsEnumerable().Where(x => Convert.ToBoolean(x["IsXuatHang"]) == true);
                        var falseRows = tbl.AsEnumerable().Where(x => Convert.ToBoolean(x["IsXuatHang"]) == false);
                        if (falseRows == null || !falseRows.Any())
                        {
                            dtXuatHang.Rows.RemoveAt(i); // Xóa hàng hiện tại
                            continue;
                        }
                        if (trueRows.Any())
                            tbl1 = trueRows.CopyToDataTable();
                        if (falseRows.Any())
                            tbl2 = falseRows.CopyToDataTable();
                        int tbl6Count = tbl6 == null ? 0 : tbl6.Rows.Count;
                        int tbl2Count = tbl6 == null ? 0 : tbl2.Rows.Count;
                        int numCount = numbers.Count == 0 ? 0 : numbers.Count;
                        dr["SLChuyenTemp"] = tbl6Count + tbl2Count + numCount;
                        dr["TuThung"] = tbl2.Rows[0]["SttThung"];
                        dr["DenThung"] = Convert.ToInt16(tbl1.Rows[0]["SttThung"]) - 1;
                        dr["SttThung"] = Convert.ToInt16(tbl1.Rows[0]["SttThung"]) - 1;
                        int SLXuat = Convert.ToInt16(dr["DenThung"]) - Convert.ToInt16(dr["TuThung"]) + 1;
                        dr["SLThung"] = SLXuat;
                        dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
                        dr["SttThungMin"] = tbl2.Rows[0]["SttThung"];

                        var tblNew = tbl.AsEnumerable().Where(x => (bool)x["IsXuatHang"] == false).ToList();
                        if (tblNew.Any())
                        {
                            tbl = tblNew.CopyToDataTable();
                        }

                        int indexdenthung = 0;
                        rangeIntTbale(tbl);
                        foreach (var item in lstRangeTable)
                        {
                            int tuThung = item.batDau;
                            int denthung = item.KetThuc;
                            int SlXuat = item.KetThuc - item.batDau + 1;
                            int sttThung = item.batDau;
                            int TotalPiece = SlXuat * Convert.ToInt16(dr["SoLuong"]);
                            if (indexdenthung != 0)
                            {
                                DataRow newRow = dtXuatHang.NewRow();
                                newRow.ItemArray = dr.ItemArray.Clone() as object[];
                                newRow["TuThung"] = tuThung;
                                newRow["DenThung"] = denthung;
                                newRow["SLThung"] = SlXuat;
                                newRow["TotalPiece"] = TotalPiece;
                                newRow["SttThung"] = sttThung;
                                newRow["SttThungMin"] = tuThung;
                                newRows.Add(newRow);
                            }
                            else
                            {
                                dr["SttThungMin"] = tuThung;
                                dr["TuThung"] = tuThung;
                                dr["DenThung"] = denthung;
                                dr["SttThung"] = tuThung;
                                dr["SLThung"] = SlXuat;
                                dr["TotalPiece"] = TotalPiece;
                            }
                            indexdenthung++;
                        }



                        //foreach (DataRow item in tbl.Rows)
                        //{
                        //    bool isXuatHang = Convert.ToBoolean(item["IsXuatHang"]);
                        //    bool nextIsXuatHang = (index + 1 < tbl.Rows.Count) ? Convert.ToBoolean(tbl.Rows[index + 1]["IsXuatHang"]) : !isXuatHang;
                        //    if (isXuatHang && isXuatHang != nextIsXuatHang)
                        //    {
                        //        int denThung = 0;
                        //        int denThungMax = tbl.AsEnumerable().Max(x => Convert.ToInt32(x["SttThung"]));
                        //        for (int j = indexCheck; j < tbl.Rows.Count; j++)
                        //        {
                        //            if (Convert.ToBoolean(tbl.Rows[j]["IsXuatHang"]))
                        //            {
                        //                denThung = Convert.ToInt32(tbl.Rows[j]["SttThung"]);
                        //                break;
                        //            }
                        //        }
                        //        denThung = denThung == 0 ? denThungMax : denThung;
                        //        if (Convert.ToInt32(dr["DenThung"]) > Convert.ToInt32(dr["TuThung"]))
                        //        {
                        //            if (denThung >= Convert.ToInt32(dr["TuThung"]) + indexCheck)
                        //            {
                        //                DataRow newRow = dtXuatHang.NewRow();
                        //                newRow.ItemArray = dr.ItemArray.Clone() as object[];

                        //                newRow["TuThung"] = Convert.ToInt32(dr["TuThung"]) + indexCheck;
                        //                newRow["DenThung"] = denThung;
                        //                newRow["SLThung"] = denThung - (Convert.ToInt32(dr["TuThung"]) + indexCheck - 1);
                        //                newRow["TotalPiece"] = (denThung - (Convert.ToInt32(dr["TuThung"]) + indexCheck - 1)) * Convert.ToInt16(dr["SoLuong"]);
                        //                newRow["SttThung"] = denThung;
                        //                newRow["SttThungMin"] = Convert.ToInt32(dr["TuThung"]) + indexCheck;
                        //                newRows.Add(newRow);
                        //            }

                        //        }
                        //        else
                        //        {
                        //            dr["DenThung"] = denThung;
                        //            dr["TuThung"] = tbl2.Rows[0]["SttThung"];
                        //            dr["SttThung"] = denThung;
                        //            SLXuat = denThung - Convert.ToInt32(tbl2.Rows[0]["SttThung"]) + 1;
                        //            dr["SLThung"] = SLXuat;
                        //            dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
                        //        }
                        //    }
                        //    index++;
                        //    indexCheck++;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ tại đây
                }
            }
            foreach (DataRow newRow in newRows)
            {
                dtXuatHang.Rows.Add(newRow);
            }
            if (dtXuatHang == null || dtXuatHang.Rows.Count == 0)
            {
                MessageBox.Show("Thùng đã được xuất hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //dtXuatHang = dtXuatHang.AsEnumerable().OrderBy(x => Convert.ToInt32(x["TuThung"])).CopyToDataTable();
            if (dtOldXuatHang != null && dtOldXuatHang.Rows.Count != 0) dtOldXuatHang.Merge(dtXuatHang);
            else dtOldXuatHang = dtXuatHang;
            ProcessAfterGetData(dtXuatHang, true);
            CreateBandSizeXuathang(dtOldXuatHang);
            dtOldXuatHang = KHDongThungLib.CalculatorTB(dtOldXuatHang);
            grcPKLXuatHang.DataSource = dtOldXuatHang;
            KHDongThungLib.AllowVieworNotPack(dtOldXuatHang, BandPCBA, BandPackA, BandStore);
            isheckFlag = true;
        }
        private void SaveXuatHang()
        {
            try
            {
                var _dtTempData = grcPKLXuatHang.DataSource as DataTable;
                if (_dtTempData is null || _dtTempData.Rows.Count == 0) return;
                var _dtDataTempNew = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() != "1");
                var _dtDataTempNew1 = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() == "0");
                var _dtDataTempTon = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() == "1");
                var _dtData = _dtDataTempNew.Count() == 0 ? new DataTable() : _dtDataTempNew.CopyToDataTable();
                var _dtData1 = _dtDataTempNew1.Count() == 0 ? new DataTable() : _dtDataTempNew1.CopyToDataTable();
                var _dtDataTon = _dtDataTempTon.Count() == 0 ? new DataTable() : _dtDataTempTon.CopyToDataTable();

                var TuKho = _dtTempData.Rows[0]["MaDVSX_NK"].ToString();
                if (TuKho is null)
                {
                    MessageBox.Show("Vui lòng chọn kho cần xuất đi!");
                    return;
                }
                else if (cbxDenKho.EditValue is null)
                {
                    MessageBox.Show("Vui lòng chọn kho cần xuất đến!");
                    return;
                }
                if (cbxDenKho.EditValue.ToString() == cbxTuKho.EditValue.ToString())
                {
                    MessageBox.Show("Kho trùng với kho đi. Vui lòng chọn kho khác!");
                    return;
                }
                var checkRowNew = _dtData1.Rows.Count;
                DataTable tblSaveXuatHang = CreateTblSaveXuatHang();

                var distinctDH = _dtTempData.AsEnumerable().Select(x => new
                {
                    MaDH = x["MaDH_XH"].ToString(),
                    MaHang = x["MaHang"].ToString(),
                    //Tenhang = x["Tenhang"].ToString(),
                    KhachHang = x["MaKH"].ToString()
                }).Distinct().ToList();
                //bool flag = true;
                foreach (var itemMaDH in distinctDH)
                {
                    //var checkPOID = _dtData.AsEnumerable().Where(x => x["MaDH"].ToString() == itemMaDH.MaDH)
                    var distinctPO = _dtTempData.AsEnumerable().Where(x => x["MaDH_XH"].ToString() == itemMaDH.MaDH).Select(y => new
                    {
                        POID = y["POID_XH"].ToString(),
                        PO = y["PO_XH"].ToString(),
                        // IsTon = y["IsTon"].ToString(),
                        Cont = y["Cont"].ToString(),
                        Seal = y["Seal"].ToString(),
                    }).Distinct().ToList();
                    foreach (var itemPO in distinctPO)
                    {
                        #region tam dong
                        //if (itemPO.IsTon != "0" && checkRowNew != 0) continue;
                        //int carton = _dtData.AsEnumerable().Where(y => ((y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID)
                        //                                                || (y["POIDTemp"].ToString() == itemPO.POID)) && y["Cont"].ToString() == itemPO.Cont.ToString()).Select(z => new
                        //                                                {
                        //                                                    SttThung = z["SttThung"],
                        //                                                    SLThung = z["SLThung"]
                        //                                                }).Distinct().ToList().Sum(x => Convert.ToInt32(x.SLThung));

                        //int SLPCS = _dtData.AsEnumerable().Where(y => ((y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID)
                        //                                        || (y["POIDTemp"].ToString() == itemPO.POID)) && y["Cont"].ToString() == itemPO.Cont).Select(z => new
                        //                                        {
                        //                                            SttThung = z["SttThung"],
                        //                                            TotalPiece = z["TotalPiece"]
                        //                                        }).Distinct().ToList().Sum(x => Convert.ToInt32(x.TotalPiece));

                        //int carton1 = _dtDataTon.AsEnumerable().Where(y => y["Cont"].ToString() == itemPO.Cont).Select(z => new
                        //{
                        //    SttThung = z["SttThung"],
                        //    SLThung = z["SLThung"]
                        //}).Distinct().ToList().Sum(x => Convert.ToInt32(x.SLThung));

                        //int SLPCS1 = _dtDataTon.AsEnumerable().Where(y => y["Cont"].ToString() == itemPO.Cont).Select(z => new
                        //{
                        //    SttThung = z["SttThung"],
                        //    TotalPiece = z["TotalPiece"]
                        //}).Distinct().ToList().Sum(x => Convert.ToInt32(x.TotalPiece));

                        //if (_dtData1.Rows.Count == 0)
                        //{
                        //    var dr = dtDonHang.AsEnumerable().Where(x => x["MaDH"].ToString() == _madh).FirstOrDefault();
                        //    _tenHang = dr["TenHang"].ToString();
                        //    _khachHang = dr["TenKH"].ToString();
                        //    var drPO = dtPO.AsEnumerable().Where(x => x["POID"].ToString() == _poid.Trim()).FirstOrDefault();
                        //    _poid = drPO["POID"].ToString();
                        //    _po = drPO["PO"].ToString();
                        //}
                        #endregion
                        var lstSeal = itemPO.Seal.Split(';');
                        foreach (var seal in lstSeal)
                        {
                            var drNew = tblSaveXuatHang.NewRow();
                            drNew["Id"] = 0;
                            drNew["MaPKL_XH"] = _maPKLXH;
                            drNew["MaDH"] = itemMaDH.MaDH;
                            drNew["MaHang"] = itemMaDH.MaHang;
                            drNew["KhachHang"] = itemMaDH.KhachHang;
                            drNew["POID"] = itemPO.POID;
                            drNew["PO"] = itemPO.PO;
                            drNew["TuKho"] = TuKho;
                            drNew["DenKho"] = GetMaKho(cbxDenKho.EditValue.ToString());
                            drNew["Carton"] = 0;
                            drNew["SoLuong"] = 0;
                            drNew["Seal"] = GetTenSeal(seal.Trim());
                            drNew["Cont"] = itemPO.Cont;
                            drNew["PackDate"] = DateTime.Now;
                            drNew["FinishDate"] = DateTime.Now;
                            drNew["ImportDate"] = DateTime.Now;
                            drNew["ExportDate"] = DateTime.Now;
                            drNew["NVienXuat"] = GlobleData.UserName ?? "Test";
                            tblSaveXuatHang.Rows.Add(drNew);
                        }
                        //if (_dtData1.Rows.Count == 0) break;
                    }
                }

                DataTable tblSave = ProcessTblSave(_dtData);// CreateTblSave();
                DataTable tblSaveTonKho = ProcessTblSave(_dtDataTon);
                var checkKho = (GetMaKho(cbxTuKho.EditValue.ToString()) == "DVSX_1" && GetMaKho(cbxDenKho.EditValue.ToString()) == "DVSX_5") ||
                    (GetMaKho(cbxTuKho.EditValue.ToString()) == "DVSX_5" && GetMaKho(cbxDenKho.EditValue.ToString()) == "DVSX_1");


                if ((tblSave == null || tblSave.Rows.Count == 0) && (tblSaveTonKho == null || tblSaveTonKho.Rows.Count == 0))
                {
                    MessageBox.Show("PKL xuất hàng dữ liệu đang trống!");
                    return;
                }
                DataSet ds = new DataSet();
                ds.Tables.Add(tblSaveXuatHang);
                ds.Tables.Add(tblSave);
                ds.Tables.Add(tblSaveTonKho);
                //return;
                string url = string.Format("{0}", URL + "ChuyenKho/Post?action=Save");

                //return;
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
                if (checkKho)
                {
                    string url2 = $"{URL}ChuyenKho/PostKho?action=UpdateKhoTanHuong&makho={GetMaKho(cbxDenKho.EditValue.ToString())}";
                    string result2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, tblSave); }).Result;
                }
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    _donhang = _madh;
                    this.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void ProcessAfterGetData(DataTable dtGet, bool flagAdd)
        {
            try
            {
                var dtKHDT = grcPKLDongThung.DataSource as DataTable;
                if (dtKHDT is null || dtGet is null) return;
                var dtTemp = dtGet.AsEnumerable().Where(x => x["MaPKL_Chuyen"].ToString() == "");
                var dtTempA = dtTemp.Count() > 0 ? dtTemp.CopyToDataTable() : new DataTable();
                foreach (DataRow drg in dtTempA.Rows)
                {
                    foreach (DataRow dr in dtKHDT.Rows)
                    {
                        if (drg["MaPKL"].ToString() == dr["MaPKL"].ToString()
                           && drg["MaDH"].ToString() == dr["MaDH"].ToString()
                           && drg["POID"].ToString() == dr["POID"].ToString()
                           && drg["ID"].ToString() == dr["ID"].ToString()
                           && drg["IsTon"].ToString() == dr["IsTon"].ToString())
                        {

                            dr["SLDaChuyenKho"] = flagAdd ? Convert.ToInt16(drg["SLChuyenTemp"]) : Convert.ToInt16(drg["SLTDaChuyenKho"]) - Convert.ToInt16(drg["SLChuyen"]);
                            dr["SLChuyen"] = CheckResetSThung.Checked ? 0 : dr["IsTon"].ToString() == "2" ? Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTXuatHang_CC"]) - Convert.ToInt32(dr["SLDaChuyenKho"])
                                                                      : Convert.ToInt32(dr["SLNhapKho"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTXuatHang_CC"]) - Convert.ToInt32(dr["SLDaChuyenKho"]);
                            dr["SLChuyen"] = Convert.ToInt16(dr["SLChuyen"]) < 0 ? 0 : dr["SLChuyen"];
                            dr["SttDaXuat"] = dr["SLDaChuyenKho"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void ProcessDataWhenReloadPKL(DataTable dt)
        {
            var grcKHXH = grcPKLXuatHang.DataSource as DataTable;
            if (grcKHXH is null || grcKHXH.Rows.Count == 0) return;
            if (!grcKHXH.Columns.Contains("SLChuyen")) return;
            foreach (DataRow dr in dt.Rows)
            {
                var drRepeat = grcKHXH.AsEnumerable().Where(x => x["MaPKL"].ToString() == dr["MaPKL"].ToString()
                                                            && x["MaDH"].ToString() == dr["MaDH"].ToString()
                                                            && x["POID"].ToString() == dr["POID"].ToString()
                                                            && x["ID"].ToString() == dr["ID"].ToString()
                                                            && x["IsTon"].ToString() == dr["IsTon"].ToString());
                if (drRepeat.Count() > 0 && drRepeat.First()["SLChuyen"] != DBNull.Value)
                {
                    dr["SLDaChuyenKho"] = Convert.ToInt16(dr["SLDaChuyenKho"]) + drRepeat.CopyToDataTable().AsEnumerable().Sum(x => Convert.ToInt16(x["SLChuyen"]));
                }
            }
        }
        private DataTable ProcessTblSave(DataTable dt)
        {
            DataTable tblSave = KHDongThungLib.CreateTblSave();
            foreach (DataRow dr in dt.Rows)
            {
                int slThung = Convert.ToInt32(dr["SLThung"].ToString());
                int TuThung = Convert.ToInt32(dr["TuThung"].ToString());
                int DenThung = Convert.ToInt32(dr["DenThung"].ToString());
                int SttThungMin = Convert.ToInt32(dr["SttThungMin"].ToString());
                foreach (DataColumn dc in dt.Columns)
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
                        drNewRow["Cont"] = dr["Cont"].ToString();
                        drNewRow["MaDH_XH"] = dr["MaDH_XH"].ToString();
                        drNewRow["POID_XH"] = dr["POID_XH"].ToString();
                        if (dr["Stt_size"].ToString() == "1") drNewRow["SttThung"] = SttThungMin + i;
                        else drNewRow["SttThung"] = SttThungMin + i;
                        tblSave.Rows.Add(drNewRow);
                    }
                }
            }
            return tblSave;
        }
        private void ClearDataToRefresh()
        {
            try
            {
                if (tblCopy != null && tblCopy.Rows.Count != 0)
                {
                    tblCopy = tblCopy.AsEnumerable().Where(x => x["MaPKL_Chuyen"].ToString() != "").CopyToDataTable();
                    grcPKLXuatHang.DataSource = tblCopy;
                }
                else grcPKLXuatHang.DataSource = new DataTable();

                if (grcPKLDongThung.DataSource != null)
                    LoadKHDongThungV2("KeHoachDongThung", _madh, _maPKL, _poid, "1", "0");
            }
            catch (Exception ex)
            {
            }
        }
        private void ResetSLThungLap()
        {
            if (dtPKL_DongThung.Rows.Count == 0) return;
            var check = CheckResetSThung.Checked;
            var dt = grcPKLDongThung.DataSource as DataTable;
            var maKho = GetMaKho(cbxTuKho.EditValue.ToString());
            var dtTemp = dtPKL_DongThung.AsEnumerable().Where(x => x["MaDVSX_NK"].ToString() == maKho).CopyToDataTable();
            foreach (DataRow dr in dtTemp.Rows)
            {
                if (check)
                    dr["SLChuyen"] = 0;
                else if (dr["MaDVSX_NK"].ToString() == maKho)
                    dr["SLChuyen"] = dr["IsTon"].ToString() == "2" ? Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTXuatHang_CC"]) - Convert.ToInt32(dr["SLDaChuyenKho"])
                                                                   : Convert.ToInt32(dr["SLNhapKho"]) - Convert.ToInt32(dr["SLTXuatHang_CC"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLDaChuyenKho"]);
                else
                    dr["SLChuyen"] = 0;
                dr["SLChuyen"] = Convert.ToInt16(dr["SLChuyen"]) < 0 ? 0 : dr["SLChuyen"];
            }

            grcPKLDongThung.DataSource = dtTemp;
        }
        private void DeleteRow(DataRow dr)
        {
            try
            {
                DataTable dtDelete = KHDongThungLib.CreateTblSave();


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
                if (dr["IsTon"].ToString() == "1") action = "DeletePKLXH_Row_Ton";
                else action = "DeletePKLXH_Row";
                string url = string.Format("{0}", URL + $"ChuyenKho/Delete?action={action}&Para={_maPKLXH}");
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
        private void GetTuKho(DataTable dt)
        {
            var dtKhoTemp = dt.AsEnumerable().Where(y => KHDongThungLib.CheckRole(y["MaDVSX_NK"].ToString())).Select(x => x["TenDVSX_NK"].ToString()).Distinct().ToList();
            cbxTuKho.Properties.Items.Clear();
            foreach (var item in dtKhoTemp)
            {
                cbxTuKho.Properties.Items.Add(item);
            }
            if (dtKhoTemp.Count() > 0)
            {
                cbxTuKho.EditValue = null;
                cbxTuKho.EditValue = dtKhoTemp[0];
            }

        }
        private void GetKho()
        {
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetKho&Para1=Para&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtKho = JsonConvert.DeserializeObject<DataTable>(json);
            foreach (DataRow dr in dtKho.Rows)
            {
                cbxDenKho.Properties.Items.Add(dr["TenDVSX"]);
            }

        }
        private string GetMaKho(string tenKho)
        {
            return dtKho.AsEnumerable().Where(x => x["TenDVSX"].ToString() == tenKho).FirstOrDefault()["MaDVSX"].ToString();
        }
        private string GetTenSeal(string Seal)
        {
            var drSeal = dtSeal.AsEnumerable().Where(x => x["Seal"].ToString() == Seal).FirstOrDefault();
            if (drSeal != null)
                return drSeal["MaSeal"].ToString();
            else return "";
        }
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
        private DataTable CreateTblSaveXuatHang()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("Id", typeof(int));
            tbl.Columns.Add("MaPKL_XH", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("KhachHang", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("TuKho", typeof(string));
            tbl.Columns.Add("DenKho", typeof(string));
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
            tbl.Columns.Add("ImportDate", typeof(DateTime));
            tbl.Columns.Add("ExportDate", typeof(DateTime));
            tbl.Columns.Add("Status", typeof(int));
            tbl.Columns.Add("Dot", typeof(int));
            tbl.Columns.Add("NVienNhap", typeof(string));
            tbl.Columns.Add("NVienXuat", typeof(string));
            return tbl;
        }
        #region Event DongThung        
        private void grvDH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValues_DH = string.Join(";", grvDH.GetSelectedRows().Select(rowHandle => grvDH.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_DonHang.EditValue = selectedValues_DH;
            if (searchLookUpEdit_DonHang.EditValue is null) return;
            _madh = searchLookUpEdit_DonHang.EditValue.ToString();
            ClearBand();
            LoadPO();
            this.ActiveControl = grcPKLDongThung;
        }
        private void searchLookUpEdit_DonHang_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string selectedValuessDH = string.Join("; ", grvDH.GetSelectedRows().Select(rowHandle => grvDH.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessDH))
            {
                e.DisplayText = "[Chọn ĐH]";
            }
            else
            {
                e.DisplayText = selectedValuessDH;
            }
        }
        private void grvPO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValuesPO = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_PO.EditValue = selectedValuesPO;
            if (searchLookUpEdit_PO.EditValue is null) return;
            _poid = searchLookUpEdit_PO.EditValue.ToString();
            LoadMaPKL();
            LoadMaPKLTon();
        }
        private void SearchLookUpEdit_PO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string selectedValuessPO = string.Join("; ", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessPO))
            {
                e.DisplayText = "[Chọn PO]";
            }
            else
            {
                e.DisplayText = selectedValuessPO;
            }
        }
        private void grvSeal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValuesSeal = string.Join(";", grvSeal.GetSelectedRows().Select(rowHandle => grvSeal.GetRowCellValue(rowHandle, searchLookUpEdit_Seal.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_Seal.EditValue = selectedValuesSeal;
            if (searchLookUpEdit_Seal.EditValue is null) return;

            _seal = searchLookUpEdit_Seal.EditValue.ToString();
        }
        private void searchLookUpEdit_Seal_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string selectedValuessSeal = string.Join("; ", grvSeal.GetSelectedRows().Select(rowHandle => grvSeal.GetRowCellValue(rowHandle, searchLookUpEdit_Seal.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessSeal))
            {
                e.DisplayText = "[Chọn Seal]";
            }
            else
            {
                e.DisplayText = selectedValuessSeal;
                _tenSeal = selectedValuessSeal;
            }

        }
        private void searchLookUpEdit1_CustomDisplayTextA(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

        }
        private void grvPKL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValuesMaPKL = string.Join(";", grvPKL.GetSelectedRows().Select(rowHandle => grvPKL.GetRowCellValue(rowHandle, searchLookUpEdit_MaPKL.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_MaPKL.EditValue = selectedValuesMaPKL;
            if (searchLookUpEdit_MaPKL.EditValue is null) return;

            _maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
            LoadKHDongThungV2("KeHoachDongThung", _madh, _maPKL, _poid, "1", "0");
            searchLookUpEdit_MaPKL_Ton.EditValue = null;
        }

        private void searchLookUpEdit_MaPKL_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string selectedValuessMaPKL = string.Join("; ", grvPKL.GetSelectedRows().Select(rowHandle => grvPKL.GetRowCellValue(rowHandle, searchLookUpEdit_MaPKL.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessMaPKL))
            {
                e.DisplayText = "[Chọn PKL]";
            }
            else
            {
                e.DisplayText = selectedValuessMaPKL;
            }
        }
        private void bandgrvDongThung_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "SLDaChuyenKho" || e.Column.FieldName == "SLChuyen") return;
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
                int SLThungDaChuyen = Convert.ToInt16(dr["SLDaChuyenKho"]);
                int SLThungDX = Convert.ToInt16(dr["SLTXuatHang_CC"]);
                int SLThungXuat = Convert.ToInt16(e.Value);
                int SLNhapTK = Convert.ToInt32(dr["SLNhapTK"]);
                string IsTon = dr["IsTon"].ToString();

                if (IsTon == "2" && ((SLThungXuat + SLThungDaChuyen > SLNhapTK) || (SLThungXuat + SLThungDX + SLThungDaChuyen > TongSLThung)))
                {
                    e.Valid = false;
                    e.ErrorText = "SL Thùng vượt SL thùng cho phép!";
                }
                else if (SLThungXuat + SLThungDX + SLThungDaChuyen + (IsTon != "0" ? 0 : SLNhapTK) > TongSLThung)
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
            if (e.Column.FieldName == "SLChuyen")
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
            else if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
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
        private void cbxTuKho_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbxTuKho.EditValue is null) return;
            ResetSLThungLap();
        }
        private void CheckResetSThung_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetSLThungLap();
        }
        private void bandgrvXuatHang_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
            //if(e.Column.FieldName == "Seal")
            //{
            //    string newValue = "";
            //    var splitValue = e.Value.ToString().Split(';');
            //    foreach(var item in splitValue)
            //    {
            //        newValue += ";" + GetTenSeal(item.Trim());
            //    }
            //    newValue = newValue.TrimStart(';');
            //    e.DisplayText = newValue;
            //}
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
            //if (e.Column.FieldName == "SLThung")
            //{
            //    // Perform your condition to determine if cells should be merged
            //    if (e.RowHandle > 0)
            //    {
            //        var preSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle - 1, "SttThung");
            //        var curSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle, "SttThung");
            //        object prevValue = bandgrvXuatHang.GetRowCellValue(e.RowHandle - 1, e.Column);
            //        object currValue = bandgrvXuatHang.GetRowCellValue(e.RowHandle, e.Column);

            //        if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
            //        {
            //            e.Handled = true;
            //            return;
            //        }
            //    }
            //}
            //else if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
            //{
            //    var preSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle - 1, "SttThung");
            //    var curSTTThung = bandgrvXuatHang.GetRowCellValue(e.RowHandle, "SttThung");
            //    if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
            //    {
            //        e.Handled = true;
            //        return;
            //    }
            //}

            //e.Handled = false;
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
        private void repoSearchSeal_Click(object sender, EventArgs e)
        {
            if (bandgrvXuatHang.FocusedColumn == colSeal) _oldSeal = bandgrvXuatHang.GetFocusedRowCellValue(colSeal).ToString();
            Console.WriteLine(_oldSeal);
        }

        private void bandgrvXuatHang_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (bandgrvXuatHang.FocusedColumn == colCont) _oldCont = bandgrvXuatHang.GetFocusedRowCellValue(colCont).ToString();
            else if (bandgrvXuatHang.FocusedColumn == colSeal) _oldSeal = bandgrvXuatHang.GetFocusedRowCellValue(colSeal).ToString();
        }
        private void bandgrvXuatHang_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var dtXuatHang = grcPKLXuatHang.DataSource as DataTable;
            var drFocus = bandgrvXuatHang.GetFocusedDataRow();
            var value = e.Value.ToString();
            if (bandgrvXuatHang.FocusedColumn == colCont)
            {
                if (_oldCont == value) return;
                foreach (DataRow dr in dtXuatHang.Rows)
                {
                    if (dr["Cont"].ToString() == _oldCont && drFocus["MaPKL_Chuyen"].ToString() == dr["MaPKL_Chuyen"].ToString() && drFocus["Seal"].ToString() == dr["Seal"].ToString())
                        dr["Cont"] = value;
                }
            }
        }

        private void repoSearchSeal_EditValueChanged(object sender, EventArgs e)
        {
            //SearchLookUpEdit sr = sender as SearchLookUpEdit;
            //var value = sr.EditValue;
            //if (_oldSeal == value) return;
            //var dtXuatHang = grcPKLXuatHang.DataSource as DataTable;
            //var drFocus = bandgrvXuatHang.GetFocusedDataRow();
            //foreach (DataRow dr in dtXuatHang.Rows)
            //{
            //    if (dr["Seal"].ToString() == _oldSeal && drFocus["MaPKL_Chuyen"].ToString() == dr["MaPKL_Chuyen"].ToString()
            //        && dr["Cont"].ToString() == drFocus["Cont"].ToString())
            //        dr["Seal"] = value;
            //}
        }

        private void searchLookUpEdit_DonHang_EditValueChanged(object sender, EventArgs e)
        {
            //if (searchLookUpEdit_DonHang.EditValue is null) return;
            //_madh = searchLookUpEdit_DonHang.EditValue.ToString();
            //LoadSize();
            //ClearBand();
            //LoadPO();
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
        private void cbxPO_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bandgrvDongThung_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu == null) return;
                e.Menu.Items.Clear();
                if (bandgrvDongThung.FocusedColumn == bandedGridColumn4)
                {
                    DXMenuItem CopyCD = new DXMenuItem();
                    CopyCD.Caption = "Nhập thùng chuyển";
                    CopyCD.Click += frmSttXuatHang;
                    e.Menu.Items.Add(CopyCD);
                }
            }
            catch (Exception ex)
            {

            }
        }
        List<string> CreateRanges(DataTable dttable, bool isXuatHang)
        {
            List<string> ranges = new List<string>();
            int start = -1;
            int end = -1;

            foreach (DataRow row in dttable.Rows)
            {
                bool currentIsXuatHang = (bool)row["IsXuatHang"];
                int currentSttThung = Convert.ToInt32(row["SttThung"]);

                if (currentIsXuatHang == isXuatHang)
                {
                    if (start == -1)
                    {
                        start = currentSttThung;
                    }
                    end = currentSttThung;
                }
                else
                {
                    if (start != -1)
                    {
                        ranges.Add($"{start}-{end}");
                        start = -1;
                        end = -1;
                    }
                }
            }

            if (start != -1)
            {
                ranges.Add($"{start}-{end}");
            }

            return ranges;
        }
        private void frmSttXuatHang(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = bandgrvDongThung.GetFocusedDataRow();
                string tuThung = dr["TuThung"].ToString();
                string denthung = dr["DenThung"].ToString();
                DataTable tblDown = new DataTable();
                DataTable tblpkl = grcPKLXuatHang.DataSource as DataTable;
                if (tblpkl != null)
                {
                    var tblpkl1 = tblpkl.AsEnumerable().Where(x => Convert.ToInt32(x["ID"]) == Convert.ToInt32(dr["ID"])
                                                                && x["MaPKL_Chuyen"].ToString() == ""
                                                                && x["MaPKLDisPlay"].ToString() == dr["MaPKLDisPlay"].ToString()
                                                                && x["MaDH"].ToString() == dr["MaDH"].ToString()).ToList();
                    if (tblpkl1.Any())
                        tblDown = tblpkl1.CopyToDataTable();


                }
                List<int> numbers = new List<int>();
                foreach (DataRow row in tblDown.Rows)
                {
                    int start = Convert.ToInt32(row["Tuthung"]);
                    int end = Convert.ToInt32(row["Denthung"]);
                    for (int x = start; x <= end; x++)
                    {
                        numbers.Add(x);
                    }
                }

                frmThungXuat frm = new frmThungXuat(Convert.ToInt32(tuThung), Convert.ToInt32(denthung));
                frm.ShowDialog();
                int tuthungx = frmThungXuat.tuthung;
                int denthungx = frmThungXuat.denthung;
                if (tuthungx == 0) return;
                string madh = dr["MaDH"].ToString();
                string poiD = dr["POID"].ToString();
                string maPKL1 = dr["MaPKL"].ToString();
                int sldaxuat = Convert.ToInt32(dr["SLDaChuyenKho"]);
                string url = $"{URL}KeHoachDongThung/Get?Action=GetISNK&MaDH={madh}&POID={poiD}&SizeTypeID={tuthungx.ToString()}&ColorID={denthungx.ToString()}&MaPKL={maPKL1}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                string url1 = $"{URL}KeHoachDongThung/Get?Action=GetISNK&MaDH={madh}&POID={poiD}&SizeTypeID={dr["TuThung"].ToString()}&ColorID={dr["DenThung"].ToString()}&MaPKL={maPKL1}";
                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                DataTable tbl5 = JsonConvert.DeserializeObject<DataTable>(json1);
                int minNumber = 1;
                int MaxNumber = 0;
                if (numbers.Count > 0)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        int number = Convert.ToInt32(row["SttThung"]);
                        if (numbers.Contains(number))
                        {
                            row["IsXuatHang"] = true;
                        }
                    }

                    foreach (DataRow row in tbl5.Rows)
                    {
                        int number = Convert.ToInt32(row["SttThung"]);
                        if (numbers.Contains(number))
                        {
                            row["IsXuatHang"] = true;
                        }
                    }
                    minNumber = numbers.AsEnumerable().Min();
                    MaxNumber = numbers.AsEnumerable().Max();
                }
                else
                    minNumber = tuthungx;

                DataTable tbl2 = null;
                var falseRows = tbl.AsEnumerable().Where(x => Convert.ToBoolean(x["IsXuatHang"]) == false);

                if (falseRows.Any())
                    tbl2 = falseRows.CopyToDataTable();
                DataTable tbl6 = new DataTable();
                var table5 = tbl5.AsEnumerable().Where(x => (bool)x["IsXuatHang"] == true).ToList();

                if (table5.Any())
                    tbl6 = table5.CopyToDataTable();

                int tbl6Count = tbl6 == null ? 0 : tbl6.Rows.Count;
                int tbl2Count = tbl2 == null ? 0 : tbl2.Rows.Count;

                if (frmThungXuat.tuthung == 0) return;

                List<string> trueRanges = CreateRanges(tbl, true);
                List<string> falseRanges = CreateRanges(tbl, false);

                string trueRow = string.Join(", ", trueRanges);
                string falseRow = string.Join(", ", falseRanges);
                if (trueRanges.Count > 0)
                {
                    var result = MessageBox.Show($"Thùng: {trueRow} đã xuất hàng \n Xác nhận xuất thùng {falseRow} ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        dr["TuThungX"] = tuthungx;// tuthungx > minNumber ? minNumber : tuthungx;
                        dr["DenThungX"] = denthungx;// denthungx > MaxNumber ? denthungx : MaxNumber;
                        dr["TuThungXM"] = tuthungx > minNumber ? minNumber : tuthungx;
                        dr["DenThungXM"] = denthungx > MaxNumber ? denthungx : MaxNumber;
                        //dr["SLTDaXuat"] = tbl6Count + tbl2Count;
                        dr["SLChuyen"] = /*tbl6Count +*/ tbl2Count;
                    }
                    else isheckFlag = true;
                }
                else
                {
                    dr["TuThungX"] = tuthungx;// tuthungx > minNumber ? minNumber : tuthungx;
                    dr["DenThungX"] = denthungx;// denthungx > MaxNumber ? denthungx : MaxNumber;
                    dr["TuThungXM"] = tuthungx > minNumber ? minNumber : tuthungx;
                    dr["DenThungXM"] = denthungx > MaxNumber ? denthungx : MaxNumber;
                    //dr["SLTDaXuat"] = tbl6Count + tbl2Count;
                    dr["SLChuyen"] = /*tbl6Count +*/ tbl2Count;
                }
                isheckFlag = false;
                frmThungXuat.tuthung = 0;
                frmThungXuat.denthung = 0;
            }
            catch (Exception ex)
            {

            }

        }

        private void bandgrvDongThung_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (bandgrvDongThung.FocusedColumn == bandedGridColumn4 && Convert.ToInt32(e.Value) != 0)
            {
                DataTable tblDown = new DataTable();
                DataRow drRow = bandgrvDongThung.GetFocusedDataRow();
                DataTable tblpkl = grcPKLXuatHang.DataSource as DataTable;
                if (tblpkl != null)
                {
                    var tblpkl1 = tblpkl.AsEnumerable().Where(x => Convert.ToInt32(x["ID"]) == Convert.ToInt32(drRow["ID"])
                                                                    && x["MaPKL_Chuyen"].ToString() == ""
                                                                    && x["MaPKLDisPlay"].ToString() == drRow["MaPKLDisPlay"].ToString()
                                                                    && x["MaDH"].ToString() == drRow["MaDH"].ToString()).ToList();
                    if (tblpkl1.Any())
                        tblDown = tblpkl1.CopyToDataTable();
                }
                List<int> numbers = new List<int>();
                foreach (DataRow row in tblDown.Rows)
                {
                    int start = Convert.ToInt32(row["Tuthung"]);
                    int end = Convert.ToInt32(row["Denthung"]);
                    for (int x = start; x <= end; x++)
                    {
                        numbers.Add(x);
                    }
                }
                int tuThung = Convert.ToInt32(drRow["TuThung"]);
                int slxuat = Convert.ToInt32(drRow["SLChuyen"]);
                int sldaxuat = Convert.ToInt32(drRow["SLDaChuyenKho"]);
                int denthung = Convert.ToInt32(drRow["DenThung"]);
                int slXuatOld = denthung - tuThung + 1 - sldaxuat;
                DataTable tblCopy = new DataTable();
                int sttthungmin = Convert.ToInt32(drRow["SttThungMin"]) + (Convert.ToInt32(drRow["TuThungX"]) - Convert.ToInt32(drRow["TuThung"]));
                int sttthungmax = (sttthungmin + Convert.ToInt32(drRow["DenThungX"]) - Convert.ToInt32(drRow["TuThungX"]));
                string madh = drRow["MaDH"].ToString();
                string poiD = drRow["POID"].ToString();
                string maPKL1 = drRow["MaPKL"].ToString();
                string url = $"{URL}KeHoachDongThung/Get?Action=GetISNK&MaDH={madh}&POID={poiD}&SizeTypeID={drRow["TuThung"].ToString()}&ColorID={drRow["DenThung"].ToString()}&MaPKL={maPKL1}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (numbers.Count > 0)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        int number = Convert.ToInt32(row["SttThung"]);
                        if (numbers.Contains(number))
                        {
                            row["IsXuatHang"] = true;
                        }
                    }
                }
                tblCopy = tbl;
                var trueRow1 = tbl.AsEnumerable().Where(x => !(bool)x["IsXuatHang"]).ToList();
                if (trueRow1.Any())
                    tbl = trueRow1.Take(slxuat).CopyToDataTable();

                int denthungx = tbl.AsEnumerable().Max(x => Convert.ToInt32(x["SttThung"]));
                tblCopy = tblCopy.AsEnumerable().Take(sldaxuat + slxuat).CopyToDataTable();

                List<string> trueRanges = CreateRanges(tblCopy, true);
                List<string> falseRanges = CreateRanges(tblCopy, false);

                string trueRow = string.Join(", ", trueRanges);
                string falseRow = string.Join(", ", falseRanges);
                if (trueRanges.Count > 0)
                {
                    //var result = MessageBox.Show($"Thùng: {trueRow} đã xuất hàng \n Xác nhận xuất thùng {falseRow} ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //if (result == DialogResult.Yes)
                    //{
                    drRow["TuThungX"] = tuThung;
                    drRow["DenThungX"] = denthungx;
                    //drRow["SLTDaXuat"] = sldaxuat + tbl.Rows.Count; Convert.ToInt32(drRow["DenThung"]) - Convert.ToInt32(drRow["TuThung"]) + 1 - (sldaxuat + tbl.Rows.Count)
                    drRow["SLChuyen"] = /*sldaxuat +*/ tbl.Rows.Count;

                    //}
                    //else
                    //    drRow["SLChuyen"] = slXuatOld;
                }
                else
                {
                    drRow["TuThungX"] = tuThung;
                    drRow["DenThungX"] = denthungx;
                    //drRow["SLTDaXuat"] = sldaxuat + tbl.Rows.Count;
                    drRow["SLChuyen"] = /*sldaxuat +*/ tbl.Rows.Count;
                }
                isheckFlag = false;
            }
        }

        private void searchLookUpEdit_Seal_EditValueChanged(object sender, EventArgs e)
        {

        }
        private void searchLookUpEdit_KhachHang_EditValueChanged(object sender, EventArgs e)
        {
            LoadDonHang();
        }
        private void searchLookUpEdit_MaPKL_EditValueChanged(object sender, EventArgs e)
        {
            //if (searchLookUpEdit_MaPKL.EditValue is null) return;
            //_maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
            //LoadKHDongThung("KeHoachDongThung", _madh, _maPKL, _poid, "1", "0");
            //searchLookUpEdit_MaPKL_Ton.EditValue = null;
        }
        private void searchLookUpEdit_MaPKL_Ton_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MaPKL_Ton.EditValue is null) return;
            var tempMaPKL = searchLookUpEdit_MaPKL_Ton.EditValue.ToString();

            var dr = dtPKLTon.AsEnumerable().Where(x => x["MaPKLValue"].ToString() == tempMaPKL).FirstOrDefault();
            var madh = dr["MaDH"].ToString();
            var maPKL = dr["MaPKL"].ToString();
            var poid = dr["POID"].ToString();
            bool IsTon = Convert.ToBoolean(dr["IsTonOld"]);
            if (IsTon)
                LoadKHDongThung(IsTon ? "KeHoachDongThungTonKho" : "KeHoachDongThung", madh, maPKL, poid, IsTon ? "1" : "3", "1");
            else
                LoadKHDongThungV2("KeHoachDongThung", madh, madh + "||" + poid + "||" + maPKL, madh + "||" + poid, "3", "1");
            searchLookUpEdit_MaPKL.Properties.DataSource = null;
            searchLookUpEdit_MaPKL.Properties.DataSource = dtMaPKL;
        }
        #endregion



        #region Manh


        private void BtnRefresh()
        {

        }
        private void BtnDelete()
        {

        }
        private void bandgrvXuatHang_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
        private void bandgrvXuatHang_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = grcPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true, true);
        }

        private void bandgrvDongThung_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
        private void bandgrvDongThung_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            var dt = grcPKLDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }

        #endregion
    }
}
