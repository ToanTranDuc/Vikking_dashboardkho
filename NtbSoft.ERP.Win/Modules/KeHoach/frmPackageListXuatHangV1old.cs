using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
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
    public partial class frmPackageListXuatHangV1OLd : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        private string _madh = string.Empty, _poid = string.Empty, _po = string.Empty, _maPKL = string.Empty, _maPKLXH = string.Empty, _maHang = string.Empty, _tenHang = string.Empty, _khachHang = string.Empty,
                _Cont = string.Empty, _oldCont = string.Empty, _Seal = string.Empty, _oldSeal = string.Empty;
        string URL = string.Empty;
        public static string _donHang = string.Empty;
        private DataTable dtDonHang = new DataTable();
        private DataTable dtPO = new DataTable();
        private DataTable dtSize = new DataTable();
        private DataTable dtPKLTon = new DataTable();
        private DataTable dtSLLayTon = new DataTable();
        private DataTable dtSeal = new DataTable();
        private bool IsEdit = false;
        KeyDownControlHandler keyDownControlHandler;
        public frmPackageListXuatHangV1OLd(string maPKLXH = "", string maDH = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maPKLXH = maPKLXH;
            _madh = maDH;
            IsEdit = maPKLXH == "" ? false : true;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateDefault();
            LoadDonHang();
            LoadSeal();
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
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void CreateDefault()
        {
            searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            searchLookUpEdit_DonHang.Properties.DisplayMember = "TenHangDisplay";
            searchLookUpEdit_DonHang.Properties.NullText = "[Chọn đơn hàng]";

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

            repoSearchLookUp_Seal.ValueMember = "MaSeal";
            repoSearchLookUp_Seal.DisplayMember = "Seal";
            repoSearchLookUp_Seal.NullText = "[Chọn Seal]";
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
            grcPKLXuatHang.DataSource = dtData;
            KHDongThungLib.AllowVieworNotPack(dtData, BandPCB, BandPack);
        }
        private void LoadDonHang()
        {
            try
            {
                string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetDH&Para1=Para&Para2=Para");
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
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetPO&Para1={_madh}&Para2=Para");
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
            if (searchLookUpEdit_PO.EditValue is null) return;
            // _poid = dtPO.AsEnumerable().Where(x => x["POID"].ToString() == searchLookUpEdit_PO.EditValue.ToString()).FirstOrDefault()["POID"].ToString();
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetMaPKL&Para1={_madh}&Para2={_poid}");
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
            //_poid = dtPO.AsEnumerable().Where(x => x["PO"].ToString() == cbxPO.EditValue.ToString()).FirstOrDefault()["POID"].ToString();
            var _maHang = dtDonHang.AsEnumerable().Where(x => x["MaDH"].ToString() == _madh).FirstOrDefault()["MaHang"].ToString();
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
        private void LoadKHDongThungV2(string controller, string madh, string maPKL, string poid, string TonNew, string IsTonKho)
        {
            //if (searchLookUpEdit_MaPKL.EditValue is null) return;
            string url = string.Format("{0}", URL + $"{controller}/Get?Action=GetPivotKHDongThung&MaDH={madh}&POID={poid}&SizeTypeID={1}&ColorID={IsTonKho}&MaPKL={maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPivot = JsonConvert.DeserializeObject<DataTable>(json);
            CreateBandSize(dtPivot);
            KHDongThungLib.ProcessSttTrung1(dtPivot);
            ProcessDataWhenReloadPKL(dtPivot);
            grcPKLDongThung.DataSource = dtPivot;
            ResetSLThungLap();
        }
        private void LoadSize()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetSizeA&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtSize = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void GetDataToPKL()
        {
            string maPKL = "";
            var dtKHDT = grcPKLDongThung.DataSource as DataTable;
            if (dtKHDT.Rows.Count == 0) return;
            var sumCheck = dtKHDT.AsEnumerable().Sum(x => Convert.ToInt32(x["SLXuat"].ToString() == "" ? 0 : x["SLXuat"]));
            if (sumCheck == 0)
            {
                MessageBox.Show("Vui lòng kiểm tra số lượng xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var CheckIsTon = dtKHDT.Rows[0]["IsTon"].ToString();
            // = searchLookUpEdit_PO.EditValue.ToString();
            if (CheckIsTon != "0")
            {
                var lstCheckPO = _poid.Split(';');
                if (lstCheckPO.Length > 1)
                {
                    MessageBox.Show("Vui lòng chỉ chọn 1 PO để xuất cho PKL tồn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show($"Xác nhận xuất PKL tồn kho cho đơn hàng PO: {_poid}", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
                foreach (DataRow dr in dtKHDT.Rows)
                {
                    dr["POIDTemp"] = _poid;
                }
            }
            if (txtCont.Text == "")
            {
                MessageBox.Show("Vui lòng nhập Cont!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (searchLookUpEdit_Seal.EditValue is null)
            {
                MessageBox.Show("Vui lòng chọn Seal!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int sttThungOld = 0;
            string tempSeal = searchLookUpEdit_Seal.EditValue.ToString();
            if ((_Cont == txtCont.Text && _Seal != tempSeal) || (_Cont != txtCont.Text && _Seal == tempSeal))
            {
                MessageBox.Show("Cont và Seal không thể trùng lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            foreach (DataRow item in dtKHDT.Rows)
            {
                int index = dtKHDT.Rows.IndexOf(item);
                if (Convert.ToInt16(item["SttThung"]) == sttThungOld) item["SLXuat"] = dtKHDT.Rows[index - 1]["SLXuat"];
                else sttThungOld = Convert.ToInt16(item["SttThung"]);
                item["Cont"] = txtCont.Text;
                item["Seal"] = tempSeal;
                item["POID_XH"] = item["IsTon"].ToString() == "0" ? item["POID"] : item["POIDTemp"];
                item["MaDH_XH"] = _madh;
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
                                                                        && _Cont == txtCont.Text);
                    if (drRepeat.Count() > 0)
                    {
                        var SLXuat = Convert.ToInt16(dr["SLXuat"]) + Convert.ToInt16(drRepeat.FirstOrDefault()["SLXuat"]);
                        //var SLToiDaChuyen = Convert.ToInt32(dr["SLNhapKho"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTXuatHang_CC"]);
                        dr["SLXuatTemp"] = Convert.ToInt16(dr["SLXuat"]);
                        dr["SLXuat"] = SLXuat;
                        dtOldXuatHang.Rows.Remove(drRepeat.FirstOrDefault());
                    }
                }
            }

            foreach (DataRow dr in dtXuatHang.Rows)
            {
                dr["SLXuatTemp"] = dr["SLXuatTemp"].ToString() == "" ? Convert.ToInt16(dr["SLXuat"]) : dr["SLXuatTemp"];
                int SLXuat = Convert.ToInt16(dr["SLXuat"]);
                var SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : (Convert.ToInt16(dr["SttDaXuat"]) + Convert.ToInt16(dr["TuThung"]) - 1);
                //if (dr["TuThung"].ToString() == dr["DenThung"].ToString() && dr["Stt_size"].ToString() == "1") dr["SttDaXuat_Decat"] = 0;
                dr["TuThung"] = SttDaXuat == 0 ? dr["TuThung"] : SttDaXuat + 1;
                dr["DenThung"] = SttDaXuat == 0 ? Convert.ToInt16(dr["TuThung"]) + SLXuat - 1 : SttDaXuat + SLXuat;
                dr["SLThung"] = dr["SLXuat"];
                dr["TotalPiece"] = SLXuat * Convert.ToInt16(dr["SoLuong"]);
            }

            if (dtOldXuatHang != null) dtOldXuatHang.Merge(dtXuatHang);
            else dtOldXuatHang = dtXuatHang;
            ProcessAfterGetData(dtXuatHang, true);
            CreateBandSizeXuathang(dtOldXuatHang);
            grcPKLXuatHang.DataSource = dtOldXuatHang;
            KHDongThungLib.AllowVieworNotPack(dtOldXuatHang, BandPCBA, BandPackA);
            _Cont = txtCont.Text;
            _Seal = tempSeal;
        }
        private void ProcessAfterGetData(DataTable dtGet, bool flagAdd)
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

                        dr["SLTDaXuat"] = flagAdd ? Convert.ToInt16(dr["SLTDaXuat"]) + Convert.ToInt16(drg["SLXuatTemp"]) : Convert.ToInt16(dr["SLTDaXuat"]) - Convert.ToInt16(drg["SLXuat"]);
                        dr["SLXuat"] = CheckResetSThung.Checked ? 0 : dr["IsTon"].ToString() == "2" ? Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTDaXuat"])
                                                                  : Convert.ToInt32(dr["SLThung"]) - Convert.ToInt32(dr["SLNhapTK"]) - Convert.ToInt32(dr["SLTDaXuat"]);
                        dr["SLXuat"] = Convert.ToInt16(dr["SLXuat"]) < 0 ? 0 : dr["SLXuat"];
                    }
                }
            }
        }
        private void ProcessDataWhenReloadPKL(DataTable dt)
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
                if (drRepeat.Count() > 0)
                {
                    dr["SLTDaXuat"] = Convert.ToInt16(dr["SLTDaXuat"]) + drRepeat.CopyToDataTable().AsEnumerable().Sum(x => Convert.ToInt16(x["SLXuat"]));
                }
            }
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
                List<PackageListXuatHangEntity> listSaveXH = new List<PackageListXuatHangEntity>();

                var checkRowNew = _dtData1.Rows.Count;
                var distinctDH = _dtTempData.AsEnumerable().Select(x => new
                {
                    MaDH = x["MaDH"].ToString(),
                    MaHang = x["MaHang"].ToString(),
                    Tenhang = x["Tenhang"].ToString(),
                    KhachHang = x["KhachHang"].ToString(),

                }).Distinct().ToList();

                foreach (var itemMaDH in distinctDH)
                {
                    var distinctPO = _dtTempData.AsEnumerable().Where(x => x["MaDH"].ToString() == itemMaDH.MaDH).Select(y => new
                    {
                        POID = y["POID"].ToString(),
                        PO = y["PO"].ToString(),
                        IsTon = y["IsTon"].ToString(),
                        Cont = y["Cont"].ToString(),
                        Seal = y["Seal"].ToString(),
                    }).Distinct().ToList();
                    foreach (var itemPO in distinctPO)
                    {
                        if (itemPO.IsTon != "0" && checkRowNew != 0) continue;
                        int carton = _dtData.AsEnumerable().Where(y => (((y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID)
                                                                        || y["POIDTemp"].ToString() == itemPO.POID))
                                                                       && y["Cont"].ToString() == itemPO.Cont)
                        .Select(z => new
                        {
                            SttThung = z["SttThung"],
                            SLThung = z["SLThung"]
                        }).Distinct().ToList().Sum(x => Convert.ToInt32(x.SLThung));

                        int SLPCS = _dtData.AsEnumerable().Where(y => (((y["MaDH"].ToString() == itemMaDH.MaDH && y["POID"].ToString() == itemPO.POID)
                                                                        || y["POIDTemp"].ToString() == itemPO.POID))
                                                                        && y["Cont"].ToString() == itemPO.Cont)
                        .Select(z => new
                        {
                            SttThung = z["SttThung"],
                            TotalPiece = z["TotalPiece"]
                        }).Distinct().ToList().Sum(x => Convert.ToInt32(x.TotalPiece));

                        int carton1 = _dtDataTon.AsEnumerable().Where(y => y["Cont"].ToString() == itemPO.Cont).Select(z => new
                        {
                            SttThung = z["SttThung"],
                            SLThung = z["SLThung"]
                        }).Distinct().ToList().Sum(x => Convert.ToInt32(x.SLThung));

                        int SLPCS1 = _dtDataTon.AsEnumerable().Where(y => y["Cont"].ToString() == itemPO.Cont).Select(z => new
                        {
                            SttThung = z["SttThung"],
                            TotalPiece = z["TotalPiece"]
                        }).Distinct().ToList().Sum(x => Convert.ToInt32(x.TotalPiece));

                        if (_dtData1.Rows.Count == 0)
                        {
                            var dr = dtDonHang.AsEnumerable().Where(x => x["MaDH"].ToString() == _madh).FirstOrDefault();
                            _maHang = dr["MaHang"].ToString();
                            _tenHang = dr["TenHang"].ToString();
                            _khachHang = dr["TenKH"].ToString();
                            var drPO = dtPO.AsEnumerable().Where(x => x["POID"].ToString() == _poid).FirstOrDefault();
                            _poid = drPO["POID"].ToString();
                            _po = drPO["PO"].ToString();
                        }
                        //var drNew = tblSaveXuatHang.NewRow();
                        PackageListXuatHangEntity ojSave = new PackageListXuatHangEntity()
                        {
                            Id = 0,
                            MaPKL_XH = _maPKLXH,
                            MaDH = checkRowNew == 0 ? _madh : itemMaDH.MaDH,
                            MaHang = checkRowNew == 0 ? _maHang : itemMaDH.MaHang,
                            KhachHang = checkRowNew == 0 ? _khachHang : itemMaDH.KhachHang,
                            POID = checkRowNew == 0 ? _poid : itemPO.POID,
                            PO = checkRowNew == 0 ? _po : itemPO.PO,
                            Carton = carton + carton1,
                            SoLuong = SLPCS + SLPCS1,
                            MaSeal = itemPO.Seal,
                            Cont = itemPO.Cont,
                            FinishDate = DateTime.Now,
                            PackDate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            ExportDate = DateTime.Now,
                            NhanVien = GlobleData.UserName ?? "Test"
                        };
                        listSaveXH.Add(ojSave);
                        if (_dtData1.Rows.Count == 0) break;
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
                int SttDaXuat = dr["Stt_size"].ToString() == "1" ? Convert.ToInt16(dr["SttDaXuat_Decat"]) : Convert.ToInt16(dr["SttDaXuat"]);
                //int SttThungMin = Convert.ToInt32(dr["SttThungMin"].ToString()) + SttDaXuat;
                int SttThungMin = Convert.ToInt32(dr["SttThungMin"].ToString()) + SttDaXuat;
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
            _Cont = _Seal = "";
            var dt = grcPKLXuatHang.DataSource as DataTable;
            ProcessAfterGetData(dt, false);
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
        private void grvPO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValuesPO = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_PO.EditValue = selectedValuesPO;
            if (searchLookUpEdit_PO.EditValue is null) return;
            _poid = searchLookUpEdit_PO.EditValue.ToString();
            this.ActiveControl = grcPKLDongThung;
            LoadMaPKL();
            LoadMaPKLTon();

        }
        string selectedValuessPO = "";
        private void searchLookUpEdit_PO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessPO = string.Join("; ", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessPO))
            {
                e.DisplayText = "[Chọn PO]";
            }
            else
            {
                e.DisplayText = selectedValuessPO;
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
               // return;
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
        }
        private void searchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {

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
                if (result != DialogResult.Yes) {
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
           
            LoadKHDongThung(IsTon ? "KeHoachDongThungTonKho" : "KeHoachDongThung", madh, maPKL, poid, IsTon ? "1" : "3", "1");
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


        private void bandgrvXuatHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

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
            KHDongThungLib.SumGroup(dt, e, true);
        }
        #endregion
    }
}
