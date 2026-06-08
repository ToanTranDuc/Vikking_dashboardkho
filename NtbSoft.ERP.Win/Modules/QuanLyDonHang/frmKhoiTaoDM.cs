using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmKhoiTaoDM : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _makh = string.Empty, _mahang = string.Empty, _mavtID = string.Empty, _mauID = string.Empty, _tenVT = string.Empty, _mauVT = string.Empty, _nhomVT = string.Empty,
                       _khachhang = string.Empty, _tenhang = string.Empty, _chitiet = string.Empty;


        private int _stt = 0;
        DataTable _dt = new DataTable();
        DataTable tblVatTu = new DataTable();
        DataTable tblSave;
        int indexFocusedRow = -1;

        Dictionary<int, object> dataDict = new Dictionary<int, object>();
        public frmKhoiTaoDM(string makh = "", string mahang = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            _makh = makh;
            _mahang = mahang;

        }
        protected override void OnLoad(EventArgs e)
        {
            tblSave = createTableSave();

            CreateSearchLookupKH();
            if (!string.IsNullOrWhiteSpace(_makh))
            {
                searchLookUpEditKH.EditValue = _makh;
                loadSearchLookUpMH();
                if (!string.IsNullOrWhiteSpace(_mahang))
                {
                    searchLookUpEditMH.EditValue = _mahang;
                }
            }
            loadSearchLookUpCopy();
        }
        private DataTable createTableSave()
        {
            //cột cho bảng lưu
            DataTable tbl = new DataTable("tblSave");
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaDot", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(string));
            tbl.Columns.Add("STT", typeof(string));
            tbl.Columns.Add("DinhMucChung", typeof(Decimal));
            tbl.Columns.Add("DinhMucChiTiet", typeof(Decimal));
            tbl.Columns.Add("TachMau", typeof(bool));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            return tbl;
        }
        private void loadVatTu()
        {
            string url = string.Format("{0}?makh={1}&mahang={2}", URL + $"KhoiTaoDM/Get", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gCVatTu.DataSource = null;
                return;
            }
            tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblVatTu == null || tblVatTu.Rows.Count == 0)
            {
                gCVatTu.DataSource = null;
                return;
            }
            if (!tblVatTu.Columns.Contains("DinhMucChung"))
            {
                tblVatTu.Columns.Add("DinhMucChung", typeof(float));
            }
            if (!tblVatTu.Columns.Contains("DinhMucHaoHut"))
            {
                tblVatTu.Columns.Add("DinhMucHaoHut", typeof(float));
            }
            if (!tblVatTu.Columns.Contains("IsNeww"))
            {
                tblVatTu.Columns.Add("IsNeww", typeof(int));
            }
            tblVatTu.AsEnumerable().ToList().ForEach(row => row["IsNeww"] = 0);
            gCVatTu.DataSource = tblVatTu;

        }
        private void loadVatTuDinhMuc()
        {
            try
            {
                string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
                string url = string.Format("{0}?makh={1}&mahang={2}&madot={3}", URL + $"KhoiTaoDM/GetDM", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString(), madot);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    gCVatTu.DataSource = null;
                    return;
                }
                tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblVatTu == null || tblVatTu.Rows.Count == 0)
                {
                    gCVatTu.DataSource = null;
                    return;
                }
                if (!tblVatTu.Columns.Contains("IsNeww"))
                {
                    tblVatTu.Columns.Add("IsNeww", typeof(int));
                }
                foreach (DataRow dr in tblVatTu.Rows)
                {
                    if (dr["DinhMucChung"] == "0")
                    {
                        dr["IsNeww"] = 0;
                    }
                    else
                    {
                        dr["IsNeww"] = 1;
                    }
                }
                gCVatTu.DataSource = tblVatTu;

            }
            catch (Exception ex) { }

        }
        private void CreateSearchLookupKH()
        {
            try
            {
                string urlKH = string.Format("{0}?", URL + "KhoiTaoDM/GetKH");
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                searchLookUpEditKH.Properties.DataSource = tblKH;
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";

            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                loadSearchLookUpMH();
                loadSearchLookUpCopy();
            }
            catch (Exception ex)
            {

            }
        }
        private void loadSearchLookUpMH()
        {

            string urlMH = string.Format("{0}?makh={1}", URL + "KhoiTaoDM/GetMH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
            string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
            DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);
            searchLookUpEditMH.Properties.DataSource = tblMH;
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";
            searchLookUpEditMH.EditValue = null;
        }
        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                loadVatTu();

                string url = string.Format("{0}?makh={1}&mahang={2}", URL + "KhoiTaoDM/GetSttDot", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                _stt = Convert.ToInt32(tbl.Rows[0][0]) + 1;
                if (_stt > 1)
                {
                    btnCheckCopy.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtNhapDot.Text = searchLookUpEditKH.Text.ToString() + '-' + searchLookUpEditMH.Text.ToString() + '-' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;

                barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                emptySpaceItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                emptySpaceItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                loadSearchLookUpCopy();
            }
            catch (Exception ex)
            { }

        }




        private void btnBangMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBangMau frm = new frmBangMau();
            frm.ShowDialog();

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBangMau frm = new frmBangMau();
            frm.ShowDialog();

        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBangSize frm = new frmBangSize();
            frm.ShowDialog();

        }

        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }

        static string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = searchLookUpEditMH;
            DataTable tblGrid = gCVatTu.DataSource as DataTable;
            DataTable tblPost = setupTableSave(tblGrid);
            if (tblPost == null || tblPost.Rows.Count == 0) return;
            foreach (DataRow row in tblGrid.Rows)
            {
                bool checkSave = CheckSave(row, row["DinhMucChung"].ToString());
                if (!checkSave && row["DinhMucChung"].ToString() != "" && row["DinhMucChung"].ToString() != "0")
                {
                    string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;

                    string urlCT = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}&isnew={8}", URL + $"KhoiTaoDM/GetSizeDinhMucChung", row["MaKH"].ToString(), row["MaHang"].ToString(), madot.ToString(),
                        row["MaMau"].ToString(), row["MauVTID"].ToString(), row["MaVTID"].ToString(), row["KhoVaiID"].ToString(), 0);
                    string jsonCT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCT); }).Result;
                    DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonCT);

                    //string url2 = $"{URL}KhoiTaoDM/SaveSizeSP?para={row["MaMau"].ToString()}";


                    string[] maMauArr = row["MaMau"].ToString().Split('|');
                    string[] tenMauArr = row["TenMau"].ToString().Split('|');
                    var distinctGroups = tblSize.AsEnumerable()
                                                .Select(r => r["MaNhomSize"].ToString())
                                                .Distinct()
                                                .ToList();

                    int groupCount = distinctGroups.Count;
                    int colorCount = Math.Min(maMauArr.Length, tenMauArr.Length);

                    if (colorCount == 1)
                    {
                        foreach (DataRow rows in tblSize.Rows)
                        {
                            rows["MaMau"] = maMauArr[0].ToString().Trim();
                            rows["TenMau"] = tenMauArr[0].ToString().Trim();
                        }
                    }
                    else
                    {
                        foreach (DataRow rows in tblSize.Rows)
                        {
                            rows["MaMau"] = maMauArr[0].ToString().Trim();
                            rows["TenMau"] = tenMauArr[0].ToString().Trim();
                        }

                        List<DataRow> originalRows = tblSize.AsEnumerable()
                                                            .Where(r => r["MaMau"].ToString() == maMauArr[0].ToString().Trim())
                                                            .ToList();

                        for (int colorIndex = 1; colorIndex < colorCount; colorIndex++)
                        {
                            foreach (DataRow baseRow in originalRows)
                            {
                                DataRow newRow = tblSize.NewRow();
                                newRow.ItemArray = baseRow.ItemArray.Clone() as object[];
                                newRow["MaMau"] = maMauArr[colorIndex].ToString().Trim();
                                newRow["TenMau"] = tenMauArr[colorIndex].ToString().Trim();
                                tblSize.Rows.Add(newRow);
                            }
                        }
                    }

                    DataTable tbl = new DataTable("tblSaveDMC");
                    tbl.Columns.Add("ID", typeof(string));
                    tbl.Columns.Add("MaKH", typeof(string));
                    tbl.Columns.Add("MaHang", typeof(string));
                    tbl.Columns.Add("MaNhom", typeof(string));
                    tbl.Columns.Add("MaVTID", typeof(string));
                    tbl.Columns.Add("MauVTID", typeof(string));
                    tbl.Columns.Add("KhoVaiID", typeof(string));
                    tbl.Columns.Add("MaNhomSize", typeof(string));
                    tbl.Columns.Add("MaSize", typeof(string));
                    tbl.Columns.Add("DinhMuc", typeof(Decimal));
                    tbl.Columns.Add("MaDot", typeof(string));
                    tbl.Columns.Add("Dot", typeof(string));
                    tbl.Columns.Add("NguoiTao", typeof(string));
                    tbl.Columns.Add("NgayTao", typeof(string));
                    tbl.Columns.Add("MaMauID", typeof(string));
                    tbl.Columns.Add("TachMau", typeof(bool));
                    tbl.Columns.Add("MaVTGhep", typeof(string));
                    //DataRow newRow = tbl.NewRow();
                    //newRow["ID"] = 0;
                    //newRow["MaKH"] = row["MaKH"];
                    //newRow["MaHang"] = row["MaHang"];
                    //newRow["MaNhom"] = row["MaNhom"];
                    //newRow["MaVTID"] = row["MaVTID"];
                    //newRow["MauVTID"] = row["MauVTID"];
                    //newRow["KhoVaiID"] = row["KhoVaiID"];
                    //newRow["MaNhomSize"] = "";
                    //newRow["MaSize"] = "";
                    //newRow["DinhMuc"] = row["DinhMucChung"].ToString();
                    //newRow["MaDot"] = madot;
                    //newRow["Dot"] = txtNhapDot.Text;
                    //newRow["NguoiTao"] = GlobleData.UserName;
                    //newRow["NgayTao"] = null;
                    //tbl.Rows.Add(newRow);

                    int columnsCount = tblSize.Columns.Count;

                    foreach (DataRow dr in tblSize.Rows)
                    {
                        for (int i = 7; i < columnsCount; i++)
                        {
                            DataRow _newRow = tbl.NewRow();
                            _newRow["ID"] = 0;
                            _newRow["MaKH"] = dr["MaKH"];
                            _newRow["MaHang"] = dr["MaHang"];
                            _newRow["MaNhom"] = row["MaNhom"];
                            _newRow["MaVTID"] = row["MaVTID"];
                            _newRow["MauVTID"] = row["MauVTID"];
                            _newRow["KhoVaiID"] = row["KhoVaiID"];
                            _newRow["MaNhomSize"] = dr["MaNhomSize"];
                            _newRow["MaDot"] = madot;
                            _newRow["Dot"] = txtNhapDot.Text.ToString().Trim();
                            _newRow["NguoiTao"] = GlobleData.UserName;
                            _newRow["NgayTao"] = null;
                            _newRow["MaMauID"] = dr["MaMau"].ToString().Trim();
                            string[] arr = tblSize.Columns[i].ToString().Split('@');
                            _newRow["MaSize"] = arr[0];
                            _newRow["DinhMuc"] = row["DinhMucChung"].ToString();
                            _newRow["TachMau"] = row["TachMau"].ToString();
                            _newRow["MaVTGhep"] = row["MaVTGhep"].ToString();
                            tbl.Rows.Add(_newRow);
                        }

                    }
                    string url2 = $"{URL}KhoiTaoDM/PostDMChiTiet?para={row["MaMau"].ToString()}";

                    string msResult2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, tbl); }).Result;


                    // string msResult2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, tbl); }).Result;
                }

            }
            string url = string.Format("{0}", URL + "KhoiTaoDM/Post");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblPost); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                txtNhapDot.ReadOnly = true;

                loadVatTuDinhMuc();
            }

        }
        private void luuDinhMucChungKhac(DataTable tblPost)
        {
            DataTable tblGrid = gCVatTu.DataSource as DataTable;
            foreach (var item in dataDict)
            {
                DataRow row = tblGrid.Rows[item.Key];
                if (item.Value != row["DinhMucChung"])
                {
                    string url = string.Format("{0}?makh={1}&mahang={2}", URL + $"KhoiTaoDM/GetSizeDMCT", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString());
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json == "[]")
                    {

                        return;
                    }
                    DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tblSize == null || tblSize.Rows.Count == 0) return;
                    DataTable tbl = new DataTable("tblSaveDMC");
                    tbl.Columns.Add("ID", typeof(string));
                    tbl.Columns.Add("MaKH", typeof(string));
                    tbl.Columns.Add("MaHang", typeof(string));
                    tbl.Columns.Add("MaNhom", typeof(string));
                    tbl.Columns.Add("MaVTID", typeof(string));
                    tbl.Columns.Add("MauVTID", typeof(string));
                    tbl.Columns.Add("KhoVaiID", typeof(string));
                    tbl.Columns.Add("MaNhomSize", typeof(string));
                    tbl.Columns.Add("MaSize", typeof(string));
                    tbl.Columns.Add("DinhMuc", typeof(Decimal));
                    tbl.Columns.Add("MaDot", typeof(string));
                    tbl.Columns.Add("Dot", typeof(string));
                    tbl.Columns.Add("NguoiTao", typeof(string));
                    tbl.Columns.Add("NgayTao", typeof(string));
                    tbl.Columns.Add("MaMauID", typeof(string));
                    foreach (DataRow rowP in tblPost.Rows)
                    {
                        foreach (DataRow rowS in tblSize.Rows)
                        {
                            for (int i = 4; i < tblSize.Columns.Count; i++)
                            {
                                string masize = tblSize.Columns[i].ToString().Split('@')[0];
                                DataRow newRow = tbl.NewRow();
                                newRow["ID"] = 0;
                                newRow["MaKH"] = rowP["MaKH"];
                                newRow["MaHang"] = rowP["MaHang"];
                                newRow["MaNhom"] = rowP["MaNhom"];
                                newRow["MaVTID"] = rowP["MaVTID"];
                                newRow["MauVTID"] = rowP["MauVTID"];
                                newRow["KhoVaiID"] = rowP["KhoVaiID"];
                                newRow["MaNhomSize"] = rowS["MaNhomSize"];
                                newRow["MaSize"] = masize;
                                newRow["DinhMuc"] = rowP["DinhMucChung"];
                                newRow["MaDot"] = rowP["MaDot"];
                                newRow["Dot"] = rowP["Dot"];
                                newRow["NguoiTao"] = GlobleData.UserName;
                                newRow["NgayTao"] = null;
                                if (newRow["DinhMuc"].ToString() != "" && !string.IsNullOrWhiteSpace(newRow["DinhMuc"].ToString()))
                                {
                                    tbl.Rows.Add(newRow);
                                }
                            }


                        }
                    }

                    string urlPOST = string.Format("{0}", URL + "KhoiTaoDM/PostDMChiTiet");

                    string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlPOST, tbl); }).Result;
                }
            }
        }
        private void luuDinhMucChung(DataTable tblPost)
        {
            string url = string.Format("{0}?makh={1}&mahang={2}", URL + $"KhoiTaoDM/GetSizeDMCT", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {

                return;
            }
            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblSize == null || tblSize.Rows.Count == 0) return;
            DataTable tbl = new DataTable("tblSaveDMC");
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("DinhMuc", typeof(Decimal));
            tbl.Columns.Add("MaDot", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(string));

            foreach (DataRow rowP in tblPost.Rows)
            {
                foreach (DataRow rowS in tblSize.Rows)
                {
                    for (int i = 4; i < tblSize.Columns.Count; i++)
                    {
                        string masize = tblSize.Columns[i].ToString().Split('@')[0];
                        DataRow newRow = tbl.NewRow();
                        newRow["ID"] = 0;
                        newRow["MaKH"] = rowP["MaKH"];
                        newRow["MaHang"] = rowP["MaHang"];
                        newRow["MaNhom"] = rowP["MaNhom"];
                        newRow["MaVTID"] = rowP["MaVTID"];
                        newRow["MauVTID"] = rowP["MauVTID"];
                        newRow["KhoVaiID"] = rowP["KhoVaiID"];
                        newRow["MaNhomSize"] = rowS["MaNhomSize"];
                        newRow["MaSize"] = masize;
                        newRow["DinhMuc"] = rowP["DinhMucChung"];
                        newRow["MaDot"] = rowP["MaDot"];
                        newRow["Dot"] = rowP["Dot"];
                        newRow["NguoiTao"] = GlobleData.UserName;
                        newRow["NgayTao"] = null;

                        if (newRow["DinhMuc"].ToString() != "" && !string.IsNullOrWhiteSpace(newRow["DinhMuc"].ToString()))
                        {
                            tbl.Rows.Add(newRow);
                        }

                    }


                }
            }

            string urlPOST = string.Format("{0}", URL + "KhoiTaoDM/PostDMChungNULL");

            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlPOST, tbl); }).Result;

        }


        private DataTable setupTableSave(DataTable tbl)
        {
            tblSave.Clear();
            string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
            foreach (DataRow dr in tbl.Rows)
            {
                DataRow _newrow = tblSave.NewRow();
                _newrow["ID"] = 0;
                _newrow["MaKH"] = searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString();
                _newrow["MaHang"] = searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString();
                _newrow["MaNhom"] = dr["MaNhom"];
                _newrow["MaVTID"] = dr["MaVTID"];
                _newrow["MauVTID"] = dr["MauVTID"];
                _newrow["KhoVaiID"] = dr["KhoVaiID"];
                _newrow["MaDot"] = madot;
                _newrow["Dot"] = txtNhapDot.Text;
                _newrow["NguoiTao"] = GlobleData.UserName;
                _newrow["NgayTao"] = null;
                _newrow["STT"] = _stt;
                _newrow["DinhMucChung"] = dr["DinhMucChung"];
                _newrow["DinhMucChiTiet"] = dr["DinhMucHaoHut"];
                _newrow["TachMau"] = dr["TachMau"];
                _newrow["MaVTGhep"] = dr["MaVTGhep"];
                if (_newrow["DinhMucChung"].ToString() != "" && !string.IsNullOrWhiteSpace(_newrow["DinhMucChung"].ToString()) && _newrow["DinhMucChung"].ToString() != "0")
                {
                    tblSave.Rows.Add(_newrow);
                }

            }
            return tblSave;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }
        private void Xoa_Click(object sender, EventArgs e)
        {
            try
            {

                GridView view = bandedGridView1;
                object DauSizeID = null;
                if (view.IsGroupRow(view.FocusedRowHandle))
                {
                    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                    DauSizeID = view.GetRowCellValue(childHandle, bandedGridColumn1);
                }
                else
                {
                    DauSizeID = view.GetFocusedRowCellValue(bandedGridColumn1);
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa dòng này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&mauID={4}&&dausize={5}&&madot={6}", URL + "PhanTichBom/Delete", _makh.ToString() == "" ? "" : _makh.ToString(),
                    _mahang.ToString() == "" ? "" : _mahang.ToString(), _mavtID.ToString() == "" ? "" : _mavtID.ToString(),
                    _mauID.ToString() == "" ? "" : _mauID.ToString(), DauSizeID == null ? "" : DauSizeID.ToString(), "");
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void btnXacNhan_Click(object sender, EventArgs e)
        {

        }


        private void searchLookUpEditKH_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                e.DisplayText = "Chọn khách hàng";
            }
        }


        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gVVatTu_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            //if (info.Column == gridColumn24)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            if (info.Column == colTenNhom)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gVVatTu.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gVVatTu.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void gVVatTu_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;

            if (column.FieldName == "DinhMucChung" || column.FieldName == "DinhMucHaoHut")
            {
                var value = view.GetFocusedValue();

                if (value != null &&
                    ((value is int && (int)value == 0) ||
                     (value is decimal && (decimal)value == 0) ||
                     (value is double && (double)value == 0) ||
                     (value is float && (float)value == 0)))
                {

                    view.SetFocusedValue(null);

                }
            }

        }
        public static void FocusFieldName(KeyPressEventArgs e, GridView grv, int soluong)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
            else
            {
                string currentValue = grv.EditingValue?.ToString() ?? "";
                string[] newValue = (currentValue + e.KeyChar).Split('.');
                if (newValue.Length > 1 && e.KeyChar != '\b')
                {
                    int CountSoLuong = newValue[1].Length;
                    if (CountSoLuong > soluong)
                        e.Handled = true;

                }
            }
        }
        private void loadDinhMucChiTiet()
        {
            string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
            DataRow row = gVVatTu.GetFocusedDataRow();
            if (row == null) return;
            string url = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}", URL + $"KhoiTaoDM/GetDMChiTiet",
                searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString()
                , row["MaNhom"].ToString(), row["MaVTID"].ToString(), row["MauVTID"].ToString(), row["KhoVaiID"].ToString(), ReplaceSpecialCharacters(madot)
                );
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null | tbl.Rows.Count == 0) return;

            double dmCT = 0;
            double sum = 0;
            int index = 0;
            foreach (DataRow dr in tbl.Rows)
            {
                double result = double.TryParse(dr["DinhMuc"].ToString(), out double temp) ? temp : 0;
                if (result > 0)
                {
                    sum += result;
                    index++;
                }
            }
            if (index == 0) index = 1;
            dmCT = Math.Round(sum / index, 4);


            try
            {
                GridView view = gVVatTu;
                if (view.FocusedRowHandle >= 0)
                {

                    view.SetRowCellValue(view.FocusedRowHandle, "DinhMucChung", dmCT);
                    dataDict.Add(view.FocusedRowHandle, dmCT);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void gCVatTu_EditorKeyPress(object sender, KeyPressEventArgs e)
        {

            if (gVVatTu.FocusedColumn.FieldName == "DinhMucHaoHut" || gVVatTu.FocusedColumn.FieldName == "DinhMuc")
                FocusFieldName(e, gVVatTu, 4);
        }



        private void gVVatTu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds; 
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true; 
        }

        private void btnCheckCopy_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var barItem = e.Item as DevExpress.XtraBars.BarCheckItem;
            if (barItem != null)
            {
                if (barItem.Checked)
                {
                    layoutControlItemSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItembtnCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutControlItemSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItembtnCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
        }

        private void barbtnTVMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPMaMauVT frm = new frmERPMaMauVT();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
            };
            frm.ShowDialog();
        }



        private void barbtnTVKhoVai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPKhoVai frm = new frmERPKhoVai();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.ShowDialog();
        }
        private void barbtnTVDonViVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPDonViVT frm = new frmERPDonViVT();
            frm.StartPosition = FormStartPosition.CenterScreen;
            /*  frm.FormClosing += (s, args) =>
              {
              };*/
            frm.ShowDialog();
        }



        private void barbtnTVNhom_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmNhomNPL frm = new frmNhomNPL();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }
        private void barTVVatTu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPVatTu frm = new frmERPVatTu();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }
        private void btnTVThongSo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Kho.frmERPThongSoVatTu frm = new Kho.frmERPThongSoVatTu();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }

        private bool CheckSave(DataRow dr, string value)
        {
            string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
            string urlCT = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}", URL + $"KhoiTaoDM/GetCheckSave",
                searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString(), ReplaceSpecialCharacters(madot), dr["MaMau"].ToString(), dr["MauVTID"].ToString(), dr["MaVTID"].ToString(), dr["KhoVaiID"].ToString());
            string jsonCT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCT); }).Result;

            if (jsonCT == "[]") return false;
            else
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonCT);
                if (tbl.Rows[0]["Value"].ToString() == value)

                    return true;
                else return false;
            };
        }

        private void repobtnDMCT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            this.ActiveControl = txtNhapDot;
            if (searchLookUpEditKH.EditValue != null && searchLookUpEditMH.EditValue != null)
            {
                this.ActiveControl = txtNhapDot;
                DataRow dr = gVVatTu.GetFocusedDataRow();
                bool checkSave = CheckSave(dr, dr["DinhMucChung"].ToString());
                string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;

                if (dr == null) return;
                frmDinhMucChiTiet frm = new frmDinhMucChiTiet(searchLookUpEditKH.EditValue.ToString(), searchLookUpEditKH.Text, searchLookUpEditMH.EditValue.ToString(), dr["TenNhom"].ToString(), dr["MaVTID"].ToString()
                                                               , dr["MaVT"].ToString(), dr["ChiTiet"].ToString(), dr["MauVT"].ToString(), dr["TenMau"].ToString(), ReplaceSpecialCharacters(madot), txtNhapDot.Text,
                                                               dr["MaNhom"].ToString(), dr["MauVTID"].ToString(), dr["KhoVaiID"].ToString(), Convert.ToBoolean(dr["TachMau"]), dr["IsNeww"].ToString(), dr["MaVTGhep"].ToString(), dr["DinhMucChung"].ToString(), dr["MaMau"].ToString(), checkSave);


                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    dr["TachMau"] = frm._isCheck;

                    dr["IsNeww"] = 1;


                    bool checComfirm = frmDinhMucChiTiet.checkOutView;
                    if (checComfirm) loadDinhMucChiTiet();
                }
                this.ActiveControl = simpleButton1;
            }

        }
        private void btnXem_Click(object sender, EventArgs e)
        {
            if (_stt - 1 == 0)
            {
                XtraMessageBox.Show($"Không có định mức gần nhất. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            frmKhoiTaoDMGanNhat frm = new frmKhoiTaoDMGanNhat(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
            frm.ShowDialog();
        }
        private void loadVatTuDinhMucCopy()
        {
            if (_stt <= 1) return;
            string madot = searchLookUpEditDotCopy.EditValue.ToString();
            string url = string.Format("{0}?makh={1}&mahang={2}&madot={3}", URL + $"KhoiTaoDM/GetDM", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString(), madot);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gCVatTu.DataSource = null;
                return;
            }
            tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblVatTu == null || tblVatTu.Rows.Count == 0)
            {
                gCVatTu.DataSource = null;
                return;
            }
            if (!tblVatTu.Columns.Contains("DinhMucChung"))
            {
                tblVatTu.Columns.Add("DinhMucChung", typeof(float));
            }
            if (!tblVatTu.Columns.Contains("DinhMucHaoHut"))
            {
                tblVatTu.Columns.Add("DinhMucHaoHut", typeof(float));
            }
            if (!tblVatTu.Columns.Contains("IsNeww"))
            {
                tblVatTu.Columns.Add("IsNeww", typeof(int));
            }
            tblVatTu.AsEnumerable().ToList().ForEach(row => row["IsNeww"] = 1);

            gCVatTu.DataSource = tblVatTu;

        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue.ToString() == "")
            {
                XtraMessageBox.Show($"Vui lòng chọn khách hàng!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue.ToString() == "")
            {
                XtraMessageBox.Show($"Vui lòng chọn mã hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (searchLookUpEditDotCopy.EditValue == null || searchLookUpEditDotCopy.EditValue.ToString() == "")
            {
                XtraMessageBox.Show($"Vui lòng chọn đợt muốn copy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string makh = searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue.ToString();
            string madotcopy = searchLookUpEditDotCopy.EditValue.ToString();
            string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
            string dot = txtNhapDot.Text;
            string username = GlobleData.UserName;

            string url = $"{URL}KhoiTaoDM/PostCopy?makh={makh}&mahang={mahang}&madotcopy={madotcopy}&madot={madot}&dot={dot}&username={username}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "True")
            {
                loadVatTuDinhMucCopy();
            }


        }
        private void loadSearchLookUpCopy()
        {
            try
            {
                searchLookUpEditDotCopy.Properties.ValueMember = "MaDot";
                searchLookUpEditDotCopy.Properties.DisplayMember = "Dot";
                string url = string.Format("{0}?makh={1}&mahang={2}", URL + "KhoiTaoDM/GetDot", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEditDotCopy.Properties.DataSource = tblDot;

            }
            catch (Exception ex)
            { }
        }
        private void gVVatTu_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVVatTu.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Copy định mức", CopyDinhMuc);
                        e.Menu.Items.Add(menuCopyMauItem);
                        //DevExpress.Utils.Menu.DXMenuItem menuCopyCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", CopyDong);
                        //e.Menu.Items.Add(menuCopyCopyItem);

                    }

                }
            }
        }
        //private void CopyDong(object sender, EventArgs e)
        //{

        //    GridView view = gVVatTu;
        //    int rowHandle = view.FocusedRowHandle;

        //    if (rowHandle >= 0)
        //    {
        //        DataTable table = gCVatTu.DataSource as DataTable;
        //        if (table == null) return;

        //        DataRow currentRow = view.GetDataRow(rowHandle);
        //        if (currentRow == null) return;

        //        string currentMaVTID = currentRow["MaVTID"].ToString();
        //        string currentMauVTID = currentRow["MauVTID"].ToString();
        //        string currentKhoVaiID = currentRow["KhoVaiID"].ToString();


        //        // Tìm STT lớn nhất với MaVTID hiện tại
        //        var maxSTT = table.AsEnumerable()
        //                          .Where(r => r["MaVTID"].ToString() == currentMaVTID && r["MauVTID"].ToString() == currentMauVTID && r["KhoVaiID"].ToString() == currentKhoVaiID && r["MaVTID"].ToString() == currentMaVTID && int.TryParse(r["STTCode"].ToString(), out _))
        //                          .Select(r => Convert.ToInt32(r["STTCode"]))
        //                          .DefaultIfEmpty(0)
        //                          .Max();

        //        int newSTT = maxSTT + 1;

        //        // Tạo dòng mới
        //        DataRow newRow = table.NewRow();

        //        // Copy dữ liệu từ dòng hiện tại
        //        foreach (DataColumn col in table.Columns)
        //        {
        //            newRow[col.ColumnName] = currentRow[col.ColumnName];
        //        }

        //        // Gán lại MaVTID và STT
        //        newRow["ThemDong"] = 1;
        //        newRow["STTCode"] = newSTT;
        //        newRow["MaCode"] = $"{currentMaVTID}|{newSTT}";

        //        // Chèn dòng vào bảng
        //        int insertIndex = view.GetDataSourceRowIndex(rowHandle) + 1;
        //        table.Rows.InsertAt(newRow, insertIndex);

        //        // Làm mới lưới và focus dòng mới
        //        view.RefreshData();
        //        int newRowHandle = view.GetRowHandle(insertIndex);
        //        view.FocusedRowHandle = newRowHandle;
        //        view.MakeRowVisible(newRowHandle);
        //    }
        //}
        private void CopyDinhMuc(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = gVVatTu.GetFocusedDataRow();
                if (dr == null) return;
                DataTable tbl = gCVatTu.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 1) //nếu có 1 dòng thì không copy đc
                    return;
                string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
                frmKhoiTaoDMCopy frm = new frmKhoiTaoDMCopy(tbl, dr, madot, txtNhapDot.Text);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    DataTable _tbl = frmKhoiTaoDMCopy._tbl;
                    gCVatTu.DataSource = _tbl;
                }



            }
            catch (Exception ex)
            {


            }

        }

        private void gVVatTu_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["ThemDong"]);
                if (Strflth == "1")
                {

                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff99");


                }


            }
        }

    }
}

