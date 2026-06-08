using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
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
    public partial class frmPackageListXuatHangV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        private string _madh = string.Empty, _poid = string.Empty, _po = string.Empty, _maPKL = string.Empty, _maPKLXH = string.Empty, _maHang = string.Empty, _tenHang = string.Empty, _khachHang = string.Empty,
                _Cont = string.Empty, _oldCont = string.Empty, _Seal = string.Empty, _oldSeal = string.Empty, MaBooking = string.Empty, _maKH = string.Empty, _shippingCode = string.Empty;
        string URL = string.Empty;
        public static string _donHang = string.Empty;
        private DataTable dtDonHang = new DataTable();
        private DataTable dtPO = new DataTable();
        private DataTable dtSize = new DataTable();
        private DataTable dtPKLTon = new DataTable();
        private DataTable dtSLLayTon = new DataTable();
        private DataTable dtSeal = new DataTable();
        DataTable tblCopy = new DataTable();
        List<MaContEntity> lstMaCont = new List<MaContEntity>();
        private bool IsEdit = false;
        KeyDownControlHandler keyDownControlHandler;
        SearchCheckSelection gridCheckMarksPO;
        bool _checkPO = false;
        public frmPackageListXuatHangV1(string maPKLXH = "", string maDH = "", string maKH = "", string shippingCode = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maPKLXH = maPKLXH;
            _madh = maDH;
            _maKH = maKH;
            IsEdit = maPKLXH == "" ? false : true;
            _shippingCode = shippingCode;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateDefault();
            LoadKhachHang();
            //LoadSeal();
            LoadMaCont();
            if (_maPKLXH != "") LoadPKLXuatHang();
            LoadShipping();
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
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void CreateDefault()
        {
            searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            searchLookUpEdit_DonHang.Properties.DisplayMember = "TenHangDisplay";
            searchLookUpEdit_DonHang.Properties.NullText = "[Chọn đơn hàng]";

            searchLookUpEdit_KhachHang.Properties.ValueMember = "MaKH";
            searchLookUpEdit_KhachHang.Properties.DisplayMember = "TenKH";
            searchLookUpEdit_KhachHang.Properties.NullText = "[Chọn khách hàng]";

            gridCheckMarksPO = new SearchCheckSelection(searchLookUpEdit_PO.Properties);
            gridCheckMarksPO.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit_PO_SelectionChanged);
            searchLookUpEdit_PO.Properties.Tag = gridCheckMarksPO;

            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn PO]";

            searchLookUpEdit_MaPKL.Properties.ValueMember = "Value";
            searchLookUpEdit_MaPKL.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL.Properties.NullText = "[Chọn PKL]";

            searchLookUpEdit_MaPKL_Ton.Properties.ValueMember = "MaPKLValue";
            searchLookUpEdit_MaPKL_Ton.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL_Ton.Properties.NullText = "[Chọn PKL]";

            searchLookUpEdit_Seal.Properties.ValueMember = "MaSeal";
            searchLookUpEdit_Seal.Properties.DisplayMember = "Seal";
            searchLookUpEdit_Seal.Properties.NullText = "[Chọn Seal]";

            searchLookUpEdit_Shipping.Properties.ValueMember = "ShippingCode";
            searchLookUpEdit_Shipping.Properties.DisplayMember = "TenShipping";
            searchLookUpEdit_Shipping.Properties.NullText = "[Chọn Hình Thức XH]";

            repoSearchLookUp_Seal.ValueMember = "MaSeal";
            repoSearchLookUp_Seal.DisplayMember = "Seal";
            repoSearchLookUp_Seal.NullText = "[Chọn Seal]";

            searchLookUpEdit_MaCont.Properties.ValueMember = "MaCont";
            searchLookUpEdit_MaCont.Properties.DisplayMember = "TenCont";
            searchLookUpEdit_MaCont.Properties.NullText = "[Chọn Cont]";

            GridView dvView = repoSearchLookUp_Seal.View;
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
            repoSearchLookUp_Seal.EditValueChanged += RepoSearchLookUp_Seal_EditValueChanged;

            searchLookUpEdit_MaPKL.Properties.PopupFormSize = new Size(680, 400);
            searchLookUpEdit_PO.Properties.PopupFormSize = new Size(550, 400);
            searchLookUpEdit_KhachHang.Properties.PopupFormSize = new Size(300, 400);
            searchLookUpEdit_MaCont.Properties.PopupFormSize = new Size(300, 400);

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
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetSeal&Para1=Para&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtSeal = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Seal.Properties.DataSource = dtSeal;
            repoSearchLookUp_Seal.DataSource = dtSeal;
            //repoCbxSeal.Items.Clear();
            //foreach(DataRow dr in dtData.Rows)
            //{
            //    repoCbxSeal.Items.Add(dr["MaSeal"]);
            //}
        }
        private void LoadPKLXuatHang()
        {
            DataTable dtData = new DataTable();
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXH&Para1={_maPKLXH}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang1 = JsonConvert.DeserializeObject<DataTable>(json);
            url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXHTon&Para1={_maPKLXH}&Para2=Para");
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
            tblCopy = dtData;
            grcPKLXuatHang.DataSource = dtData;
            KHDongThungLib.AllowVieworNotPack(dtData, BandPCB, BandPack, BandStore);
        }
        private void LoadDonHang()
        {
            try
            {
                if (searchLookUpEdit_KhachHang.EditValue is null) return;
                string action = cbxSMS.Checked ? "GetDH_SMS" : "GetDH";
                string url = string.Format("{0}", URL + $"XuatHang/Get?Action={action}&Para1={searchLookUpEdit_KhachHang.EditValue.ToString()}&Para2=Para");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
                //var lstTemp = dtDonHang.AsEnumerable().GroupBy(x => new
                //{
                //    MaDH = x["MaDH"].ToString(),
                //    Mahang = x["MaHang"].ToString(),
                //    Tenhang = x["TenHang"].ToString(),
                //    TenKH = x["TenKH"].ToString(),
                //    SLKH = x["SLTong"].ToString()
                //}).Select(group => new
                //    {
                //        MaDH = group.Key.MaDH,
                //        Mahang = group.Key.Mahang,
                //        Tenhang = group.Key.Tenhang,
                //        TenKH = group.Key.TenKH,
                //        SLKH = group.Key.SLKH
                //    });
                //string jsonT = JsonConvert.SerializeObject(lstTemp);
                //DataTable _dtDonHang = JsonConvert.DeserializeObject<DataTable>(jsonT);
                searchLookUpEdit_DonHang.Properties.DataSource = dtDonHang;
                if (_madh == "") return;
                searchLookUpEdit_DonHang.EditValue = _madh;
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadPO()
        {
            if (_madh is null) return;
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetPO&Para1={_madh}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtPO = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtPO.Rows.Count > 0)
                _checkPO = true;
            else
            {
                _checkPO = false;
                searchLookUpEdit_PO.EditValue = null;
            }
            searchLookUpEdit_PO.Properties.DataSource = dtPO;
            if (dtPO.Rows.Count == 0) return;
            else
            {

            }
            //searchLookUpEdit_PO.EditValue = null;
            //searchLookUpEdit_PO.EditValue = dtPO.Rows[0]["POID"];
        }

        DataTable dtMaPKL = new DataTable();
        private void LoadMaPKL()
        {
            //Console.WriteLine(_poid);
            if (_poid == "" && !cbxSMS.Checked) return;
            string action = cbxSMS.Checked ? "GetMaPKL_SMS" : "GetMaPKL";
            // _poid = dtPO.AsEnumerable().Where(x => x["POID"].ToString() == searchLookUpEdit_PO.EditValue.ToString()).FirstOrDefault()["POID"].ToString();
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action={action}&Para1={_madh}&Para2={_poid}");
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
            var lstDH = _madh.Split(';');
            //_poid = dtPO.AsEnumerable().Where(x => x["PO"].ToString() == cbxPO.EditValue.ToString()).FirstOrDefault()["POID"].ToString();
            var _maHang = dtDonHang.AsEnumerable().Where(x => lstDH.Contains(x["MaDH"].ToString())).FirstOrDefault()["MaHang"].ToString();
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetMaPKLTon&Para1={_maHang}&Para2={_poid}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtPKLTon = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaPKL_Ton.Properties.DataSource = dtPKLTon;
            searchLookUpEdit_MaPKL_Ton.EditValue = null;
            //if (dtPKLTon.Rows.Count > 0)
            //{
            //    searchLookUpEdit_MaPKL_Ton.EditValue = null;
            //    searchLookUpEdit_MaPKL_Ton.EditValue = dtPKLTon.Rows[0]["MaPKL"].ToString();
            //}
        }
        private void LoadKHDongThung(string controller, string madh, string maPKL, string poid, string TonNew, string IsTonKho)
        {
            //if (searchLookUpEdit_MaPKL.EditValue is null) return;
            string url = string.Format("{0}", URL + $"{controller}/Get?Action=GetPivotKHDongThung&MaDH={madh}&MaDVSX=Para&DotSX=Para&POID={poid}&SizeTypeID={1}&ColorID={IsTonKho}&ProductID=Para&SizeID=Para&MaPKL={maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPivot = JsonConvert.DeserializeObject<DataTable>(json);
            foreach (DataRow dr in dtPivot.Rows)
            {
                dr["KyHieu"] = dr["KyHieuA"];
            }
            CreateBandSize(dtPivot);
            KHDongThungLib.ProcessSttTrung1(dtPivot);
            grcPKLDongThung.DataSource = dtPivot;
            ResetSLThungLap();
        }
        private DataTable CreateTblXuatHang()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Action", typeof(string));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("SizeTypeID", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("MaPKL", typeof(string));
            return dt;
        }
        private void LoadKHDongThungV2(string controller, string madh, string maPKL, string poid, string TonNew, string IsTonKho)
        {
            //if (searchLookUpEdit_MaPKL.EditValue is null) return;
            string action = cbxSMS.Checked ? "GetPivotKHDongThung_SMS" : "GetPivotKHDongThung";
            DataTable dtPost = CreateTblXuatHang();
            var drNew = dtPost.NewRow();
            drNew["Action"] = action;
            drNew["MaDH"] = madh;
            drNew["SizeTypeID"] = 1;
            drNew["ColorID"] = IsTonKho;
            drNew["POID"] = poid;
            drNew["MaPKL"] = maPKL;
            dtPost.Rows.Add(drNew);
            string url = "", json = "";
            if (action == "GetPivotKHDongThung")
            {
                url = string.Format("{0}", URL + $"{controller}/PostGetV2");
                json = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtPost); }).Result;
            }
            else
            {
                url = string.Format("{0}", URL + $"{controller}/Get?Action={action}&MaDH={madh}&POID={poid}&SizeTypeID={1}&ColorID={IsTonKho}&MaPKL={maPKL}");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            }
            var dtPivot = JsonConvert.DeserializeObject<DataTable>(json);
            CreateBandSize(dtPivot);
            KHDongThungLib.ProcessSttTrung1(dtPivot);
            ProcessDataWhenReloadPKL(dtPivot);
            dtPivot = KHDongThungLib.CalculatorTB(dtPivot);
            grcPKLDongThung.DataSource = dtPivot;

            ResetSLThungLap();
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
            try
            {
                string maPKL = "";
                //grcPKLXuatHang.DataSource = null;
                var dtKHDT = grcPKLDongThung.DataSource as DataTable;
                if (!dtKHDT.Columns.Contains("ThungTemp"))
                {
                    dtKHDT.Columns.Add("ThungTemp", typeof(string));

                }
                if (dtKHDT.Rows.Count == 0) return;
                var sumCheck = dtKHDT.AsEnumerable().Sum(x => Convert.ToInt32(x["SLXuat"].ToString() == "" ? 0 : x["SLXuat"]));
                if (sumCheck < 0)
                {
                    MessageBox.Show("Vui lòng kiểm tra số lượng xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(searchLookUpEdit_MaCont?.EditValue?.ToString()))
                {
                    MessageBox.Show("Vui lòng chọn Cont!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var CheckIsTon = dtKHDT.Rows[0]["IsTon"].ToString();
                // = searchLookUpEdit_PO.EditValue.ToString();
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

                //if (searchLookUpEdit_Seal.EditValue is null)
                //{
                //    MessageBox.Show("Vui lòng chọn Seal!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}
                int sttThungOld = 0;
                string tempSeal = "";
                //if ((_Cont == txtCont.Text && _Seal != tempSeal) || (_Cont != txtCont.Text && _Seal == tempSeal))
                //{
                //    MessageBox.Show("Cont và Seal không thể trùng lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                foreach (DataRow item in dtKHDT.Rows)
                {
                    int index = dtKHDT.Rows.IndexOf(item);
                    var drPre = dtKHDT.Rows[index == 0 ? 0 : index - 1];
                    if (Convert.ToInt16(item["SttThung"]) == sttThungOld && drPre["MaPKLDisplay"].ToString() == item["MaPKLDisplay"].ToString()) item["SLXuat"] = drPre["SLXuat"];
                    else sttThungOld = Convert.ToInt16(item["SttThung"]);
                    item["Cont"] = searchLookUpEdit_MaCont.EditValue.ToString();
                    item["Seal"] = tempSeal;
                    item["POID_XH"] = item["IsTon"].ToString() == "0" ? item["POID_G"] : item["POIDTemp"];
                    item["PO_XH"] = item["IsTon"].ToString() == "0" ? item["PO_G"] : item["PO_XH"];
                    item["MaDH_XH"] = item["IsTon"].ToString() == "0" ? item["MaDH"] : _madh;
                }
                var tempFilter = dtKHDT.AsEnumerable().Where(x => Convert.ToInt16(x["SLXuat"].ToString() == "" ? 0 : x["SLXuat"]) != 0);
                var dtXuatHang = tempFilter.Count() > 0 ? tempFilter.CopyToDataTable() : new DataTable();
                var colTemp = dtXuatHang.Columns.Add("SLXuatTemp", typeof(int));
                colTemp.DefaultValue = 0;
                var dtOldXuatHang = grcPKLXuatHang.DataSource as DataTable;
                if (dtOldXuatHang != null)
                {
                    foreach (DataRow dr in dtXuatHang.Rows)
                    {
                        var drRepeat = dtOldXuatHang.AsEnumerable().Where(x => x["MaPKL"].ToString() == dr["MaPKL"].ToString()
                                                                            && x["MaDH"].ToString() == dr["MaDH"].ToString()
                                                                            && x["POID"].ToString() == dr["POID"].ToString()
                                                                            && x["ID"].ToString() == dr["ID"].ToString()
                                                                            && x["IsTon"].ToString() == dr["IsTon"].ToString()
                                                                            && x["Cont"].ToString() == dr["Cont"].ToString()
                                                                            && _Cont == searchLookUpEdit_MaCont.EditValue?.ToString());
                        if (drRepeat.Count() > 0)
                        {
                            //var SLXuat = Convert.ToInt16(dr["SLXuat"]) + Convert.ToInt16(drRepeat.FirstOrDefault()["SLXuat"]);
                            ////var SLToiDaChuyen = Convert.ToInt32(dr["SLNhapKho"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTXuatHang_CC"]);
                            //dr["SLXuatTemp"] = Convert.ToInt16(dr["SLXuat"]);
                            //dr["SLXuat"] = SLXuat;
                            //dtOldXuatHang.Rows.Remove(drRepeat.FirstOrDefault());
                        }
                    }
                }
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
                                                                    && x["MaPKL_XH"].ToString() == ""
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
                        string url = $"{URL}KeHoachDongThung/Get?Action=GetISXH&MaDH={madh}&POID={poiD}&SizeTypeID={sttthungmin.ToString()}&ColorID={sttthungmax.ToString()}&MaPKL={maPKL1}";
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
                        string url1 = $"{URL}KeHoachDongThung/Get?Action=GetISXH&MaDH={madh}&POID={poiD}&SizeTypeID={dr["TuThung"].ToString()}&ColorID={dr["DenThung"].ToString()}&MaPKL={maPKL1}";
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
                            int SLXuat = Convert.ToInt16(dr["SLXuat"]);// Convert.ToInt32(dr["SLTDaXuat"]) != 0 ? Convert.ToInt16(dr["SLTDaXuat"]) : Convert.ToInt16(dr["SLXuat"]);
                            var SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : (Convert.ToInt16(dr["SttDaXuat"]) + Convert.ToInt16(dr["TuThung"]) - 1);

                            DataRow drRowgrCDT = dtKHDT.AsEnumerable().FirstOrDefault(x =>
                                   x["MaDH"].ToString() == dr["MaDH"].ToString() &&
                                   x["MaPKL"].ToString() == dr["MaPKL"].ToString() &&
                                   x["POID"].ToString() == dr["POID"].ToString() &&
                                   x["TuThung"].ToString() == dr["TuThung"].ToString() &&
                                   x["DenThung"].ToString() == dr["DenThung"].ToString());
                            drRowgrCDT["ThungTemp"] = KHDongThungLib.LayChuoiThung(Convert.ToInt32(dr["TuThungX"]), Convert.ToInt32(dr["DenThungX"]));

                            dr["TuThung"] = dr["TuThungX"];
                            dr["DenThung"] = dr["DenThungX"];
                            dr["SLThung"] = SLXuat;
                            dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
                            dr["SLXuatTemp"] = Convert.ToInt32(dr["SLTDaXuat"]) + SLXuat;
                            dr["SttThungMin"] = sttthungmin;
                            dr["SttThung"] = sttthungmax;
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
                            dr["SLXuatTemp"] = tbl6Count + tbl2Count + numCount;

                            int index = 0;
                            int indexCheck = 1;
                            int indexSttThung = 0;
                            var tblNew = tbl.AsEnumerable().Where(x => (bool)x["IsXuatHang"] == false).ToList();
                            if (tblNew.Any())
                            {
                                tbl = tblNew.CopyToDataTable();
                            }
                            DataRow drRowgrCDT = dtKHDT.AsEnumerable().FirstOrDefault(x =>
                                  x["MaDH"].ToString() == dr["MaDH"].ToString() &&
                                  x["MaPKL"].ToString() == dr["MaPKL"].ToString() &&
                                  x["POID"].ToString() == dr["POID"].ToString() &&
                                  x["TuThung"].ToString() == dr["TuThung"].ToString() &&
                                  x["DenThung"].ToString() == dr["DenThung"].ToString());
                            var thungTemp = drRowgrCDT["ThungTemp"].ToString();
                            rangeIntTbale(tbl);
                            int indexdenthung = 0;
                            foreach (var item in lstRangeTable)
                            {
                                int tuThung = item.batDau;
                                int denthung = item.KetThuc;
                                thungTemp += "," + KHDongThungLib.LayChuoiThung(Convert.ToInt32(tuThung), Convert.ToInt32(denthung));
                                drRowgrCDT["ThungTemp"] = thungTemp;
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
                            //    //bool isXuatHang = Convert.ToBoolean(item["IsXuatHang"]);
                            //    //bool nextIsXuatHang = (index + 1 < tbl.Rows.Count) ? Convert.ToBoolean(tbl.Rows[index + 1]["IsXuatHang"]) : !isXuatHang;
                            //    //if (isXuatHang != nextIsXuatHang || Convert.ToInt32(item["SttThung"]) != indexSttThung)
                            //    //{
                            //    //    int denThung = 0;
                            //    //    int denThungMax = tbl.AsEnumerable().Where(x => (bool)x["IsXuatHang"] == false).Max(x => Convert.ToInt32(x["SttThung"]));
                            //    //    for (int j = indexCheck; j < tbl.Rows.Count; j++)
                            //    //    {
                            //    //        if (Convert.ToInt32(item["SttThung"]) != indexSttThung)
                            //    //        {
                            //    //            denThung = Convert.ToInt32(tbl.Rows[j]["SttThung"]);
                            //    //            break;
                            //    //        }
                            //    //    }
                            //    //    indexSttThung = Convert.ToInt32(item["SttThung"]);
                            //    //    denThung = denThung == 0 ? denThungMax : denThung;
                            //    //    if (Convert.ToInt32(dr["DenThung"]) >= Convert.ToInt32(dr["TuThung"]))
                            //    //    {
                            //    //        if (denThung >= Convert.ToInt32(dr["TuThung"]) + indexCheck - 1)
                            //    //        {
                            //    //            DataRow newRow = dtXuatHang.NewRow();
                            //    //            newRow.ItemArray = dr.ItemArray.Clone() as object[];
                            //    //            if (indexdenthung == denThung) continue;
                            //    //            newRow["TuThung"] = Convert.ToInt32(item["SttThung"]);
                            //    //            newRow["DenThung"] = denThung;
                            //    //            newRow["SLThung"] = denThung - (Convert.ToInt32(dr["TuThung"]) + indexCheck - 1);
                            //    //            newRow["TotalPiece"] = (denThung - (Convert.ToInt32(dr["TuThung"]) + indexCheck - 1)) * Convert.ToInt16(dr["SoLuong"]);
                            //    //            newRow["SttThung"] = denThung;
                            //    //            newRow["SttThungMin"] = Convert.ToInt32(dr["TuThung"]) + indexCheck;
                            //    //            indexdenthung = Convert.ToInt32(newRow["DenThung"]);
                            //    //            newRows.Add(newRow);
                            //    //        }

                            //    //    }
                            //    //    else
                            //    //    {
                            //    //        dr["DenThung"] = denThung;
                            //    //        dr["TuThung"] = tbl2.Rows[0]["SttThung"];
                            //    //        dr["SttThung"] = denThung;
                            //    //        SLXuat = denThung - Convert.ToInt32(tbl2.Rows[0]["SttThung"]) + 1;
                            //    //        dr["SLThung"] = SLXuat;
                            //    //        dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
                            //    //    }
                            //    //}
                            //    index++;
                            //    indexCheck++;
                            //    indexSttThung++;
                            //}
                        }

                        // dr["TuThung_XHR"] = dr["TuThung"];
                    }
                    catch (Exception ex)
                    {
                        // Xử lý ngoại lệ tại đây
                    }
                }

                // Thêm các hàng mới vào DataTable
                foreach (DataRow newRow in newRows)
                {
                    dtXuatHang.Rows.Add(newRow);
                }
                foreach (DataRow dr in dtXuatHang.Rows)
                {
                    dr["TuThung_XHR"] = dr["TuThung"];
                    dr["DenThung_XHR"] = dr["DenThung"];
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
                if (cbxDanhLaiThung.Checked) ProcessRePackBox(dtOldXuatHang);
                dtOldXuatHang = KHDongThungLib.CalculatorTB(dtOldXuatHang);
                grcPKLXuatHang.DataSource = dtOldXuatHang;
                bandgrvXuatHang.ExpandAllGroups();
                KHDongThungLib.AllowVieworNotPack(dtOldXuatHang, BandPCBA, BandPackA, BandStoreA);
                _Cont = searchLookUpEdit_MaCont.EditValue.ToString();
                _Seal = tempSeal;
                isheckFlag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
        private void ProcessAfterGetData(DataTable dtGet, bool flagAdd)
        {
            try
            {
                var dtKHDT = grcPKLDongThung.DataSource as DataTable;
                if (dtKHDT is null || dtGet is null) return;
                var dtTemp = dtGet.AsEnumerable().Where(x => x["MaPKL_XH"].ToString() == "");
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

                            dr["SLTDaXuat"] = flagAdd ? Convert.ToInt16(drg["SLXuatTemp"]) : Convert.ToInt16(drg["SLTDaXuat"]) - Convert.ToInt16(drg["SLXuat"]);
                            dr["SLXuat"] = CheckResetSThung.Checked ? 0 : dr["IsTon"].ToString() == "2" ? Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTDaXuat"])
                                                                      : Convert.ToInt32(dr["SLThung"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTDaXuat"]);
                            dr["SLXuat"] = Convert.ToInt16(dr["SLXuat"]) < 0 ? 0 : dr["SLXuat"];
                            dr["SttDaXuat"] = dr["SLTDaXuat"];
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
            try
            {
                var grcKHXH = grcPKLXuatHang.DataSource as DataTable;
                if (grcKHXH is null || grcKHXH.Rows.Count == 0) return;
                if (!grcKHXH.Columns.Contains("SLXuat")) return;
                foreach (DataRow dr in dt.Rows)
                {
                    var drRepeat = grcKHXH.AsEnumerable().Where(x => x["MaPKL"].ToString() == dr["MaPKL"].ToString()
                                                                && x["MaDH"].ToString() == dr["MaDH"].ToString()
                                                                && x["POID"].ToString() == dr["POID"].ToString()
                                                                && x["ID"].ToString() == dr["ID"].ToString()
                                                                && x["IsTon"].ToString() == dr["IsTon"].ToString());
                    if (drRepeat.Count() > 0 && dr["SLXuat"] != DBNull.Value)
                    {
                        dr["SLTDaXuat"] = Convert.ToInt16(dr["SLTDaXuat"]) + drRepeat.CopyToDataTable().AsEnumerable().Sum(x => Convert.ToInt16(x["SLXuat"]));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void ProcessRePackBox(DataTable dt, bool flagRepack = true)
        {
            if (dt is null) return;
            int TuThung = 1;
            if (flagRepack)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["TuThung"] = TuThung;
                    dr["DenThung"] = TuThung + Convert.ToInt16(dr["SLXuat"]) - 1;
                    TuThung = TuThung + Convert.ToInt16(dr["SLXuat"]); ;
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["TuThung"] = dr["TuThung_XHR"];
                    dr["DenThung"] = dr["DenThung_XHR"];

                }
            }

        }
        private void SaveXuatHang()
        {
            try
            {
                MaBooking = txtSoBookingg.Text;
                string ShippingCode = searchLookUpEdit_Shipping.EditValue?.ToString() ?? _shippingCode;
                var _dtTempData = grcPKLXuatHang.DataSource as DataTable;
                if (_dtTempData is null || _dtTempData.Rows.Count == 0) return;
                var _dtDataTempNew = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() != "1");
                var _dtDataTempNew1 = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() == "0");
                var _dtDataTempTon = _dtTempData.AsEnumerable().Where(x => x["IsTon"].ToString() == "1");
                var _dtData = _dtDataTempNew.Count() == 0 ? new DataTable() : _dtDataTempNew.CopyToDataTable();
                var _dtData1 = _dtDataTempNew1.Count() == 0 ? new DataTable() : _dtDataTempNew1.CopyToDataTable();
                var _dtDataTon = _dtDataTempTon.Count() == 0 ? new DataTable() : _dtDataTempTon.CopyToDataTable();
                List<PackageListXuatHangEntity> listSaveXH = new List<PackageListXuatHangEntity>();

                var checkRowNew = _dtData1.Rows.Count;
                var distinctDH = _dtTempData.AsEnumerable().Select(x => new
                {
                    MaDH = x["MaDH_XH"].ToString(),
                    MaHang = x["SMS"].ToString() == "1" ? "" : x["MaHang"].ToString(),
                    //MaHang = x["MaHang"].ToString(),
                    //Tenhang = x["Tenhang"].ToString(),
                    KhachHang = x["MaKH"].ToString(),

                }).Distinct().ToList();

                foreach (var itemMaDH in distinctDH)
                {
                    var distinctPO = _dtTempData.AsEnumerable().Where(x => x["MaDH_XH"].ToString() == itemMaDH.MaDH).Select(y => new
                    {
                        POID = y["POID_XH"].ToString(),
                        PO = y["PO_XH"].ToString(),
                        //IsTon = y["IsTon"].ToString(),
                        Cont = y["Cont"].ToString(),
                        //Seal = y["Seal"].ToString(),
                    }).Distinct().ToList();
                    foreach (var itemPO in distinctPO)
                    {
                        //if (itemPO.IsTon != "0" && checkRowNew != 0) continue;
                        //int carton = _dtData.AsEnumerable().Where(y => (((y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID)
                        //                                                || y["POIDTemp"].ToString() == itemPO.POID))
                        //                                               && y["Cont"].ToString() == itemPO.Cont)
                        //.Select(z => new
                        //{
                        //    SttThung = z["SttThung"],
                        //    SLThung = z["SLThung"]
                        //}).Distinct().ToList().Sum(x => Convert.ToInt32(x.SLThung));

                        //int SLPCS = _dtData.AsEnumerable().Where(y => (((y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID)
                        //                                                || y["POIDTemp"].ToString() == itemPO.POID))
                        //                                                && y["Cont"].ToString() == itemPO.Cont)
                        //.Select(z => new
                        //{
                        //    SttThung = z["SttThung"],
                        //    TotalPiece = z["TotalPiece"]
                        //}).Distinct().ToList().Sum(x => Convert.ToInt32(x.TotalPiece));

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


                        //var drNew = tblSaveXuatHang.NewRow();
                        PackageListXuatHangEntity ojSave = new PackageListXuatHangEntity()
                        {
                            Id = 0,
                            MaPKL_XH = _maPKLXH,
                            MaDH = itemMaDH.MaDH,
                            MaHang = itemMaDH.MaHang,
                            KhachHang = itemMaDH.KhachHang,
                            POID = itemPO.POID,
                            PO = itemPO.PO,
                            Carton = 0,
                            SoLuong = 0,
                            MaSeal = "",
                            Cont = lstMaCont.FirstOrDefault(x => x.MaCont == itemPO.Cont)?.TenCont,
                            FinishDate = DateTime.Now,
                            PackDate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            ExportDate = DateTime.Now,
                            NhanVien = GlobleData.UserName ?? "Test",
                            SoBooking = string.IsNullOrEmpty(MaBooking) ? null : MaBooking,
                            MaBooking = string.IsNullOrEmpty(MaBooking) ? null : MaBooking,
                            GhiChu = ShippingCode,
                            MaCont = itemPO.Cont,
                        };
                        listSaveXH.Add(ojSave);
                        // if (_dtData1.Rows.Count == 0) break;
                    }
                }
                string json = JsonConvert.SerializeObject(listSaveXH);
                DataTable tblSaveXuatHang = JsonConvert.DeserializeObject<DataTable>(json);
                DataTable tblSave = ProcessTblSave(_dtData);// CreateTblSave();
                DataTable tblSaveTonKho = ProcessTblSave(_dtDataTon);
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
                string url = string.Format("{0}", URL + "XuatHang/Post?action=Save");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.Close();
                    _donHang = searchLookUpEdit_DonHang.EditValue == null ? "" : searchLookUpEdit_DonHang.EditValue.ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadSLLayTon()
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetSLLayTon&Para1={_poid}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtSLLayTon = JsonConvert.DeserializeObject<DataTable>(json);
            //if(dtSLLayTon is null || dtSLLayTon.Rows.Count == 0)
            //{
            //    searchLookUpEdit_MaPKL_Ton.Properties.DataSource = new DataTable();
            //}
        }
        private string GetPOFromPOID(string POID)
        {
            return dtPO.AsEnumerable().Where(x => x["POID"].ToString() == POID).FirstOrDefault()["PO"].ToString();
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
                        drNewRow["PCB_Pack"] = dr["TuThung_XHR"].ToString();
                        drNewRow["Pack_Ctn"] = dr["DenThung_XHR"].ToString();
                        drNewRow["Cont"] = dr["Cont"].ToString();// lstMaCont.FirstOrDefault(x => x.MaCont == dr["Cont"].ToString())?.TenCont;
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
            if (tblCopy != null && tblCopy.Rows.Count != 0)
            {
                tblCopy = tblCopy.AsEnumerable().Where(x => x["MaPKL_XH"].ToString() != "").CopyToDataTable();
                grcPKLXuatHang.DataSource = tblCopy;
            }
            else grcPKLXuatHang.DataSource = new DataTable();

            if (grcPKLDongThung.DataSource != null)
                LoadKHDongThungV2("KeHoachDongThung", _madh, _maPKL, _poid, "1", "0");

        }
        private void ResetSLThungLap()
        {
            var check = CheckResetSThung.Checked;
            var dt = grcPKLDongThung.DataSource as DataTable;
            if (dt == null) return;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["IsTon"].ToString() != "0")
                {
                    dr["SLXuat"] = 0;
                    continue;
                }
                if (check)
                    dr["SLXuat"] = 0;
                else dr["SLXuat"] = dr["IsTon"].ToString() == "2" ? Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTDaXuat"])
                                                                  : Convert.ToInt32(dr["SLThung"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTDaXuat"]);

                dr["SLXuat"] = Convert.ToInt16(dr["SLXuat"]) < 0 ? 0 : dr["SLXuat"];
            }
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
        private string GetSizeName(string SizeID)
        {
            var dt = dtSize.AsEnumerable().Where(x => x["SizeID"].ToString() == SizeID);

            return dtSize.AsEnumerable().Where(x => x["SizeID"].ToString() == SizeID).FirstOrDefault()["Size"].ToString();
        }

        #region Event DongThung
        private void searchLookUpEditView_DonHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValues_DH = string.Join(";", grvDonHang.GetSelectedRows().Select(rowHandle => grvDonHang.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_DonHang.EditValue = selectedValues_DH;
            if (searchLookUpEdit_DonHang.EditValue is null) return;
            _madh = searchLookUpEdit_DonHang.EditValue.ToString();
            ClearBand();
            if (cbxSMS.Checked)
            {
                LoadMaPKL();
                return;
            }
            LoadPO();
            this.ActiveControl = grcPKLDongThung;
        }
        private void searchLookUpEdit_DonHang_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string selectedValuessDH = string.Join("; ", grvDonHang.GetSelectedRows().Select(rowHandle => grvDonHang.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.DisplayMember)));
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
            //string selectedValuesPO = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.ValueMember)));
            //// Thiết lập giá trị EditValue cho control
            //searchLookUpEdit_PO.EditValue = selectedValuesPO;
            //if (searchLookUpEdit_PO.EditValue is null) return;
            //_poid = searchLookUpEdit_PO.EditValue.ToString();
            //this.ActiveControl = grcPKLDongThung;
            //LoadMaPKL();
            //LoadMaPKLTon();

        }
        string selectedValuessPO = "";
        private void searchLookUpEdit_PO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //selectedValuessPO = string.Join("; ", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.DisplayMember)));
            //if (string.IsNullOrEmpty(selectedValuessPO))
            //{
            //    e.DisplayText = "[Chọn PO]";
            //}
            //else
            //{
            //    e.DisplayText = selectedValuessPO;
            //}
            if (_checkPO)
            {
                gridCheckMarksPO.Selection.Clear();
                e.DisplayText = "Chọn PO";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
                if (gridCheckMark == null) return;
                foreach (DataRowView rv in gridCheckMark.Selection)
                {

                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["PO"].ToString());
                }
                if (string.IsNullOrEmpty(sb.ToString()))
                {
                    e.DisplayText = "Chọn PO";
                }
                else
                {

                    e.DisplayText = sb.ToString();
                }
            }
        }
        private void grvMaPKL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValuesMaPKL = string.Join(";", grvMaPKL.GetSelectedRows().Select(rowHandle => grvMaPKL.GetRowCellValue(rowHandle, searchLookUpEdit_MaPKL.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_MaPKL.EditValue = selectedValuesMaPKL;
            if (searchLookUpEdit_MaPKL.EditValue is null) return;

            _maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
            LoadKHDongThungV2("KeHoachDongThung", _madh, _maPKL, _poid, "1", "0");
            //LoadKHDongThung("KeHoachDongThung", _madh, _maPKL, _poid, "1", "0");
            searchLookUpEdit_MaPKL_Ton.EditValue = null;
        }
        private void searchLookUpEdit_MaPKL_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string selectedValuessMaPKL = string.Join("; ", grvMaPKL.GetSelectedRows().Select(rowHandle => grvMaPKL.GetRowCellValue(rowHandle, searchLookUpEdit_MaPKL.Properties.DisplayMember)));
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
            if (e.Column.FieldName == "SLTDaXuat" || e.Column.FieldName == "SLXuat" || e.Column.FieldName == "SLNhapKho") return;
            KHDongThungLib.CustomColumnDisplay(e);
            //if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }
        private void bandgrvDongThung_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                this.ActiveControl = grcPKLXuatHang;
                var dr = bandgrvDongThung.GetFocusedDataRow();
                if (dr is null) return;
                int TongSLThung = Convert.ToInt16(dr["SLThung"]);
                int SLThungDX = Convert.ToInt16(dr["SLTDaXuat"]);
                int SLNhapTK = Convert.ToInt16(dr["SLNhapTK"] ?? 0);
                string IsTonKho = dr["IsTon"].ToString();
                int SLThungXuat = Convert.ToInt16(e.Value);

                if (IsTonKho == "2" && SLThungXuat + SLThungDX > SLNhapTK)
                {
                    e.Valid = false;
                    e.ErrorText = "SL Thùng vượt SL thùng cho phép!";
                    return;
                }
                else if (SLThungXuat + SLThungDX + (IsTonKho != "0" ? 0 : SLNhapTK) > TongSLThung)
                {
                    e.Valid = false;
                    e.ErrorText = "SL Thùng vượt SL thùng cho phép!";
                    return;
                }
                return;
                if (dr["IsTon"].ToString() != "0")
                {
                    var dtSource = grcPKLDongThung.DataSource as DataTable;
                    int sum = 0;
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        string columnName = dc.ColumnName;
                        if (!columnName.Contains('@') || dr[columnName].ToString() == "0") continue;
                        dr["SLXuat"] = e.Value;
                        var drSum = dtSource.AsEnumerable().GroupBy(gr => new
                        {
                            DauSizeID = gr["DauSizeID"].ToString(),
                            MaMau = gr["ColorID"].ToString(),
                            SizeID = columnName.Split('@')[1],
                        }).Select(x => new
                        {
                            DauSizeID = x.Key.DauSizeID,
                            MaMau = x.Key.MaMau,
                            SizeID = x.Key.SizeID,
                            SoLuong = x.Sum(y => Convert.ToInt32(y[columnName]) * Convert.ToInt32(y["SLXuat"]))
                        }).FirstOrDefault();
                        var drFilter = dtSLLayTon.AsEnumerable().Where(x => x["DauSizeID"].ToString() == drSum.DauSizeID.ToString()
                                                                            && x["MaMau"].ToString() == drSum.MaMau.ToString()
                                                                            && x["SizeID"].ToString() == drSum.SizeID.ToString()).FirstOrDefault();
                        if (drFilter != null)
                        {
                            var SLTonConLai = Convert.ToInt32(drFilter["ConLai"]);
                            var SLKHTon = Convert.ToInt32(drFilter["SLCheckTon"]);
                            var SLDaXuat = Convert.ToInt32(drFilter["SLDaXuat"]);
                            if (SLTonConLai < drSum.SoLuong)
                            {
                                e.Valid = false;
                                e.Value = 0;
                                e.ErrorText = $"SL xuất tồn > SL lập KH xuất tồn\r\nSLKH nhập tồn: {SLKHTon}\r\nSL tồn đã xuất: {SLDaXuat}\r\nSL xuất: {drSum.SoLuong}";
                                return;
                            }
                        }
                        else if (drFilter is null)
                        {
                            e.Value = 0;
                            MessageBox.Show("Đơn hàng không nhận dữ liệu tồn kho!");
                            return;
                        }
                    }
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
            if (e.Column.FieldName == "SLThung")
            {
                // Perform your condition to determine if cells should be merged
                if (e.RowHandle > 0)
                {
                    var preSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, "SttThung");
                    var curSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle, "SttThung");
                    var prePKL = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, "MaPKLDisplay");
                    var curPKL = bandgrvDongThung.GetRowCellValue(e.RowHandle, "MaPKLDisplay");
                    object prevValue = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, e.Column);
                    object currValue = bandgrvDongThung.GetRowCellValue(e.RowHandle, e.Column);

                    if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung)
                        && prePKL != null && curPKL != null && prePKL.Equals(curPKL))
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
                var prePKL = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, "MaPKLDisplay");
                var curPKL = bandgrvDongThung.GetRowCellValue(e.RowHandle, "MaPKLDisplay");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung)
                     && prePKL != null && curPKL != null && prePKL.Equals(curPKL))
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
        private void searchLookUpEdit_DonHang_EditValueChanged(object sender, EventArgs e)
        {
            //if (searchLookUpEdit_DonHang.EditValue is null) return;
            //_madh = searchLookUpEdit_DonHang.EditValue.ToString();
            //LoadSize();
            //ClearBand();
            //LoadPO();
        }
        void searchLookUpEdit_PO_SelectionChanged(object sender, EventArgs e)
        {

            Control c = this.ActiveControl;
            StringBuilder sb = new StringBuilder();
            if (c is DevExpress.XtraLayout.LayoutControl)
            {
                if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
                {
                    c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
                }
            }
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {

                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(";"); }
                    sb.Append(rv["POID"].ToString());
                    //_poid = sb.ToString();
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();


            }
            string ss = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.ValueMember)));
            Console.WriteLine(ss.ToString());
            _poid = sb.ToString();
            // this.ActiveControl = grcPKLDongThung;
            LoadMaPKL();
            LoadMaPKLTon();
            // string selectedValuesPO = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            //searchLookUpEdit_PO.EditValue = sb;
            //if (sb is null) return;



        }
        private void searchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {
            // this.ActiveControl = searchLookUpEdit_MaPKL;

            if (searchLookUpEdit_PO.EditValue != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in gridCheckMarksPO.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(";"); }
                    sb.Append(rv["POID"].ToString());
                }
                //_poid = searchLookUpEdit_PO.EditValue.ToString();
                //Console.WriteLine(sb.ToString());
                //this.ActiveControl = grcPKLDongThung;
                //LoadMaPKL();
                //LoadMaPKLTon();
            }
        }

        private void searchLookUpEdit_MaPKL_EditValueChanged(object sender, EventArgs e)
        {

        }
        private void searchLookUpEdit_MaPKL_Ton_EditValueChanged(object sender, EventArgs e)
        {
            LoadProcessTon();
            if (searchLookUpEdit_MaPKL_Ton.EditValue is null) return;
            searchLookUpEdit_MaPKL.Properties.DataSource = null;
            searchLookUpEdit_MaPKL.Properties.DataSource = dtMaPKL;
        }
        private void searchLookUpEdit_Seal_EditValueChanged(object sender, EventArgs e)
        {
            var _maSeal = searchLookUpEdit_Seal.EditValue;
            if (_maSeal is null) return;
            var chMaSeal = dtSeal.AsEnumerable().Any(x => x["MaSeal"].ToString() == _maSeal && x["Status"].ToString() != "0");
            if (chMaSeal && !IsEdit)
            {
                MessageBox.Show("Seal đã được lập kế hoạch. Vui lòng chọn seal khác!");

                return;
            }
            else if (chMaSeal && IsEdit)
            {
                DialogResult result = MessageBox.Show("Seal đã được lập kế hoạch. Bạn có chắc muốn gọp?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    searchLookUpEdit_Seal.EditValue = null;
                    return;
                }
            }
        }
        private void LoadProcessTon()
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
                LoadKHDongThungV2("KeHoachDongThung", madh, madh + "||" + poid + "||" + maPKL, madh + "||" + poid, "1", "1");
            LoadSLLayTon();
        }



        #endregion

        private void CreateBandSizeXuathang(DataTable dt)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
                if (!CheckExistBandXuatHang(_sizeID)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = _size + "@" + _sizeID;
                col.Name = "col" + _sizeID;
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
        #region Manh
        private void BtnXoa()
        {

        }
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnRefresh();
        }



        private void BtnRefresh()
        {
            if (_madh == "") return;
            if (searchLookUpEdit_MaPKL_Ton.EditValue != null) LoadProcessTon();
            else LoadKHDongThungV2("KeHoachDongThung", _madh, _maPKL, _poid, "1", "0");
        }

        private void bandgrvDongThung_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu == null) return;
                e.Menu.Items.Clear();
                if (bandgrvDongThung.FocusedColumn == bandedGridColumn15)
                {
                    DXMenuItem CopyCD = new DXMenuItem();
                    CopyCD.Caption = "Nhập thùng xuất";
                    CopyCD.Click += frmSttXuatHang; ;
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
                                                        && x["MaPKL_XH"].ToString() == ""
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
                int sldaxuat = Convert.ToInt32(dr["SLTDaXuat"]);
                string url = $"{URL}KeHoachDongThung/Get?Action=GetISXH&MaDH={madh}&POID={poiD}&SizeTypeID={tuthungx.ToString()}&ColorID={denthungx.ToString()}&MaPKL={maPKL1}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                string url1 = $"{URL}KeHoachDongThung/Get?Action=GetISXH&MaDH={madh}&POID={poiD}&SizeTypeID={dr["TuThung"].ToString()}&ColorID={dr["DenThung"].ToString()}&MaPKL={maPKL1}";
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
                    if (falseRanges.Count == 0)
                        MessageBox.Show("Các thùng đã được xuất", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    else
                    {
                        var result = MessageBox.Show($"Thùng: {trueRow} đã xuất hàng \n Xác nhận xuất thùng {falseRow} ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            dr["TuThungX"] = tuthungx;// tuthungx > minNumber ? minNumber : tuthungx;
                            dr["DenThungX"] = denthungx;// denthungx > MaxNumber ? denthungx : MaxNumber;
                            dr["TuThungXM"] = tuthungx > minNumber ? minNumber : tuthungx;
                            dr["DenThungXM"] = denthungx > MaxNumber ? denthungx : MaxNumber;
                            //dr["SLTDaXuat"] = tbl6Count + tbl2Count;
                            dr["SLXuat"] = /*tbl6Count +*/ tbl2Count;
                        }
                        else isheckFlag = true;
                    }
                }
                else
                {
                    dr["TuThungX"] = tuthungx;//> minNumber ? minNumber : tuthungx;
                    dr["DenThungX"] = denthungx;// denthungx > MaxNumber ? denthungx : MaxNumber;
                    dr["TuThungXM"] = tuthungx > minNumber ? minNumber : tuthungx;
                    dr["DenThungXM"] = denthungx > MaxNumber ? denthungx : MaxNumber;
                    //dr["SLTDaXuat"] = tbl6Count + tbl2Count;
                    dr["SLXuat"] = /*tbl6Count +*/ tbl2Count;
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
            if (bandgrvDongThung.FocusedColumn == bandedGridColumn15 && Convert.ToInt32(e.Value) != 0)
            {
                DataTable tblDown = new DataTable();
                DataRow drRow = bandgrvDongThung.GetFocusedDataRow();
                DataTable tblpkl = grcPKLXuatHang.DataSource as DataTable;
                if (tblpkl != null)
                {
                    var tblpkl1 = tblpkl.AsEnumerable().Where(x => Convert.ToInt32(x["ID"]) == Convert.ToInt32(drRow["ID"])
                                            && x["MaPKL_XH"].ToString() == ""
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
                int slxuat = Convert.ToInt32(drRow["SLXuat"]);
                int sldaxuat = Convert.ToInt32(drRow["SLTDaXuat"]);
                int denthung = Convert.ToInt32(drRow["DenThung"]);
                int slXuatOld = denthung - tuThung + 1 - sldaxuat;
                DataTable tblCopy = new DataTable();
                int sttthungmin = Convert.ToInt32(drRow["SttThungMin"]) + (Convert.ToInt32(drRow["TuThungXM"]) - Convert.ToInt32(drRow["TuThung"]));
                int sttthungmax = (sttthungmin + Convert.ToInt32(drRow["DenThungXM"]) - Convert.ToInt32(drRow["TuThungXM"]));
                string madh = drRow["MaDH"].ToString();
                string poiD = drRow["POID"].ToString();
                string maPKL1 = drRow["MaPKL"].ToString();
                string url = $"{URL}KeHoachDongThung/Get?Action=GetISXH&MaDH={madh}&POID={poiD}&SizeTypeID={drRow["TuThung"].ToString()}&ColorID={drRow["DenThung"].ToString()}&MaPKL={maPKL1}";
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
                tblCopy = tbl.Copy();
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
                    if (falseRanges.Count == 0)
                        MessageBox.Show("Các thùng đã được xuất", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    else
                    {
                        //var result = MessageBox.Show($"Thùng: {trueRow} đã xuất hàng \n Xác nhận xuất thùng {falseRow} ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //if (result == DialogResult.Yes)
                        //{
                        drRow["TuThungX"] = tuThung;
                        drRow["DenThungX"] = denthungx;
                        //drRow["TuThungXM"] = tuthung > minNumber ? minNumber : tuthungx;
                        //drRow["DenThungXM"] = denthungx > MaxNumber ? denthungx : MaxNumber;
                        //drRow["SLTDaXuat"] = sldaxuat + tbl.Rows.Count; Convert.ToInt32(drRow["DenThung"]) - Convert.ToInt32(drRow["TuThung"]) + 1 - (sldaxuat + tbl.Rows.Count)
                        drRow["SLXuat"] = /*sldaxuat +*/ tbl.Rows.Count;
                        //}
                        //else
                        //    drRow["SLXuat"] = slXuatOld;
                    }
                }
                else
                {
                    drRow["TuThungX"] = tuThung;
                    drRow["DenThungX"] = denthungx;
                    //drRow["TuThungXM"] = tuthungx > minNumber ? minNumber : tuthungx;
                    //drRow["DenThungXM"] = denthungx > MaxNumber ? denthungx : MaxNumber;
                    //drRow["SLTDaXuat"] = sldaxuat + tbl.Rows.Count;
                    drRow["SLXuat"] = /*sldaxuat +*/ tbl.Rows.Count;
                }
                isheckFlag = false;
            }
        }

        private void cbxSMS_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDonHang();
        }

        private void cbxDanhLaiThung_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dt = grcPKLXuatHang.DataSource as DataTable;

            ProcessRePackBox(dt, cbxDanhLaiThung.Checked);
        }

        private void searchLookUpEdit_PO_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            _checkPO = false;
        }

        private void bandgrvDongThung_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                bandgrvDongThung.CloseEditor();
                bandgrvDongThung.UpdateCurrentRow();
                DataTable dt = grcPKLDongThung.DataSource as DataTable;
                string _fieldName = bandgrvDongThung.FocusedColumn.FieldName;
                if (_fieldName != "SLXuat") return;
                KHDongThungLib.HandleBandViewKey(e, bandgrvDongThung, _fieldName);
            }
            catch (Exception ex) { }
        }

        private void bandgrvDongThung_DoubleClick(object sender, EventArgs e)
        {

            try
            {
                DataRow dr = bandgrvDongThung.GetFocusedDataRow();
                DataTable tbl = grcPKLDongThung.DataSource as DataTable;
                if (!tbl.Columns.Contains("ThungTemp"))
                {
                    tbl.Columns.Add("ThungTemp", typeof(string));
                }
                if (dr is null) return;
                var l_MaDH = dr["MaDH"].ToString();
                var l_POID = dr["POID"].ToString();
                var l_MaPKL = dr["MaPKL"].ToString();
                var l_SttThungmin = dr["SttThungMin"].ToString();
                var l_SttThungmax = dr["SttThung"].ToString();
                tbl = tbl.AsEnumerable().Where(x => x["MaDH"].ToString() == l_MaDH.ToString() && x["POID"].ToString() == l_POID.ToString() && x["MaPKL"].ToString() == l_MaPKL.ToString()
                                                && x["SttThungMin"].ToString() == l_SttThungmin.ToString() && x["SttThung"].ToString() == l_SttThungmax.ToString()).CopyToDataTable();
                string url = $"{URL}KeHoachDongThung/Get?Action=GetCTDSThung&MaDH={l_MaDH}&POID={l_POID}&SizeTypeID={l_SttThungmin.ToString()}&ColorID={l_SttThungmax.ToString()}&MaPKL={l_MaPKL}";
                // string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTDSThung&Para1={l_MaDH}&Para2={l_POID}&Para3={l_MaPKL}&Para4={l_SttThungmin}&Para5={l_SttThungmax}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                frmPackageListXuatHangChiTiet frm = new frmPackageListXuatHangChiTiet(dt, tbl);
                frm.ShowDialog();
                int SLXuat = frmPackageListXuatHangChiTiet.SLThung;
                string dsThungTemp = frmPackageListXuatHangChiTiet.ThungTemp;
                if (dsThungTemp != "")
                    dr["ThungTemp"] = dsThungTemp;
                dr["SLTDaXuat"] = Convert.ToInt16(dr["SLTDaXuat"]) + SLXuat;
                var check = CheckResetSThung.Checked;
                if (check == false)
                    dr["SLXuat"] = Convert.ToInt16(dr["SLXuat"]) - SLXuat;
                DataTable tblXH = frmPackageListXuatHangChiTiet.tblXH;
                var dtOldXuatHang = grcPKLXuatHang.DataSource as DataTable;
                if (dtOldXuatHang != null)
                {
                    foreach (DataRow drA in tblXH.Rows)
                    {
                        var drRepeat = dtOldXuatHang.AsEnumerable().Where(x => x["MaPKL"].ToString() == drA["MaPKL"].ToString()
                                                                            && x["MaDH"].ToString() == drA["MaDH"].ToString()
                                                                            && x["POID"].ToString() == drA["POID"].ToString()
                                                                            && x["ID"].ToString() == drA["ID"].ToString()
                                                                            && x["IsTon"].ToString() == drA["IsTon"].ToString()
                                                                            && x["Cont"].ToString() == drA["Cont"].ToString()
                                                                            && _Cont == searchLookUpEdit_MaCont.EditValue?.ToString());
                        if (drRepeat.Count() > 0)
                        {

                        }
                    }
                }
                if (dtOldXuatHang != null && dtOldXuatHang.Rows.Count != 0) dtOldXuatHang.Merge(tblXH);
                else dtOldXuatHang = tblXH;
                //ProcessAfterGetData(tblXH, true);
                CreateBandSizeXuathang(dtOldXuatHang);
                if (cbxDanhLaiThung.Checked) ProcessRePackBox(dtOldXuatHang);
                dtOldXuatHang = KHDongThungLib.CalculatorTB(dtOldXuatHang);
                grcPKLXuatHang.DataSource = dtOldXuatHang;
                bandgrvXuatHang.ExpandAllGroups();
                KHDongThungLib.AllowVieworNotPack(dtOldXuatHang, BandPCBA, BandPackA, BandStoreA);
            }
            catch (Exception ex)
            {

            }


        }

        private void bandgrvXuatHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

        }

        private void searchLookUpEdit_KhachHang_EditValueChanged(object sender, EventArgs e)
        {
            LoadDonHang();
        }

        private void repoSearchLookUp_Seal_Click(object sender, EventArgs e)
        {
            if (bandgrvXuatHang.FocusedColumn == colSeal) _oldSeal = bandgrvXuatHang.GetFocusedRowCellValue(colSeal).ToString();
            Console.WriteLine(_oldSeal);
        }
        private void bandgrvXuatHang_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (bandgrvXuatHang.FocusedColumn == colCont) _oldCont = bandgrvXuatHang.GetFocusedRowCellValue(colCont).ToString();
            else if (bandgrvXuatHang.FocusedColumn == colSeal) _oldSeal = bandgrvXuatHang.GetFocusedRowCellValue(colSeal).ToString();

        }
        private void bandgrvXuatHang_RowCellClick(object sender, RowCellClickEventArgs e)
        {

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
                    if (dr["Cont"].ToString() == _oldCont && drFocus["MaPKL_XH"].ToString() == dr["MaPKL_XH"].ToString() && drFocus["Seal"].ToString() == dr["Seal"].ToString())
                        dr["Cont"] = value;
                }
            }
            else if (bandgrvXuatHang.FocusedColumn.FieldName == "TuThung")
            {
                var valueTuThung = e.Value.ToString();
                if (valueTuThung == "") return;
                drFocus["DenThung"] = Convert.ToInt16(valueTuThung) + Convert.ToInt16(drFocus["SLXuat"]) - 1;
            }
            //else if (bandgrvXuatHang.FocusedColumn == colSeal)
            //{
            //    if (_oldSeal == value) return;
            //    foreach (DataRow dr in dtXuatHang.Rows)
            //    {
            //        if (dr["Seal"].ToString() == _oldSeal) dr["Seal"] = value;
            //    }
            //}
        }
        private void RepoSearchLookUp_Seal_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit sr = sender as SearchLookUpEdit;
            var value = sr.EditValue;
            if (_oldSeal == value) return;
            var dtXuatHang = grcPKLXuatHang.DataSource as DataTable;
            var drFocus = bandgrvXuatHang.GetFocusedDataRow();
            foreach (DataRow dr in dtXuatHang.Rows)
            {
                if (dr["Seal"].ToString() == _oldSeal && drFocus["MaPKL_XH"].ToString() == dr["MaPKL_XH"].ToString()
                    && dr["Cont"].ToString() == drFocus["Cont"].ToString())
                    dr["Seal"] = value;
            }
        }



        private void bandgrvDongThung_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
        private void bandgrvDongThung_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = grcPKLDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }
        private void bandgrvXuatHang_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
        private void bandgrvXuatHang_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            var dt = grcPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true, true);
        }
        #endregion

        #region Luân
        private void LoadMaCont()
        {
            try
            {
                lstMaCont.Clear();
                string url = string.Format("{0}?Action={1}", URL + "MaCont/Get", "GET");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstMaCont = JsonConvert.DeserializeObject<List<MaContEntity>>(json);
                }

                if (lstMaCont == null && lstMaCont?.Count == 0) return;
                searchLookUpEdit_MaCont.Properties.DataSource = lstMaCont;

                RepositoryItemSearchLookUpEdit rMaContEdit = new RepositoryItemSearchLookUpEdit();
                rMaContEdit.DataSource = lstMaCont;
                rMaContEdit.DisplayMember = "TenCont";
                rMaContEdit.ValueMember = "MaCont";
                rMaContEdit.ShowClearButton = false;
                rMaContEdit.NullText = "[Chọn Mã Cont]";
                rMaContEdit.EditValueChanged += rMaContEdit_EditValueChanged;
                GridView dvView = rMaContEdit.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "MaCont", Caption = "Mã Cont", Name = "colMaContEdit", Visible = true });
                    dvView.Columns.Add(new GridColumn { FieldName = "TenCont", Caption = "Tên Cont", Name = "colTenContEdit", Visible = true });
                }

                colCont.ColumnEdit = rMaContEdit;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void rMaContEdit_EditValueChanged(object sender, EventArgs e)
        {
            {
                DevExpress.XtraEditors.Controls.ChangingEventArgs eventChange = e as DevExpress.XtraEditors.Controls.ChangingEventArgs;
                //if (e.RowHandle < 0) return;
                var dtXuatHang = grcPKLXuatHang.DataSource as DataTable;
                var drFocus = bandgrvXuatHang.GetFocusedDataRow();
                var value = eventChange.NewValue;
                if (bandgrvXuatHang.FocusedColumn == colCont)
                {
                    if (_oldCont == value) return;
                    foreach (DataRow dr in dtXuatHang.Rows)
                    {
                        if (dr["Cont"].ToString() == _oldCont && drFocus["MaPKL_XH"].ToString() == dr["MaPKL_XH"].ToString() && drFocus["Seal"].ToString() == dr["Seal"].ToString())
                            dr["Cont"] = value;
                    }
                }
            }
        }
        #endregion

        #region Modify Shipping Xuất Hàng 15-04-2025
        private void LoadShipping()
        {
            try
            {
                DataTable tblShippingXuatHang = new DataTable();
                string url = $"{URL}Dic_ShippingXuatHang/Get?action=Get&para1=NONE&para2=NONE";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    tblShippingXuatHang = JsonConvert.DeserializeObject<DataTable>(json);
                }

                if (tblShippingXuatHang == null && tblShippingXuatHang?.Rows?.Count == 0) return;

                searchLookUpEdit_Shipping.Properties.DataSource = tblShippingXuatHang;

                if (!string.IsNullOrEmpty(_shippingCode?.Trim()))
                {
                    searchLookUpEdit_Shipping.EditValue = _shippingCode;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion

    }
    public class rangeList
    {
        public int batDau { get; set; }
        public int KetThuc { get; set; }
    }

}
