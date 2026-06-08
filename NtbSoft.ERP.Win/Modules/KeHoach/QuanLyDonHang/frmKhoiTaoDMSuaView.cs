using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
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
    public partial class frmKhoiTaoDMSuaView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _makh = string.Empty, _mahang = string.Empty, _mavtID = string.Empty, _mauID = string.Empty, _tenVT = string.Empty, _mauVT = string.Empty, _nhomVT = string.Empty,
                       _khachhang = string.Empty, _tenhang = string.Empty, _chitiet = string.Empty, _madot = string.Empty;


        private int _stt = 0;
        DataTable _dt = new DataTable();
        DataTable tblVatTu = new DataTable();
        DataTable tblSave;
        int indexFocusedRow = -1;
        Dictionary<int, object> dataDict = new Dictionary<int, object>();
        public frmKhoiTaoDMSuaView(string makh = "", string mahang = "", string madot = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            _makh = makh;
            _mahang = mahang;
            _madot = madot;
        }
        protected override void OnLoad(EventArgs e)
        {
            tblSave = createTableSave();
            CreateSearchLookupKH();
            InIt();
            if (!string.IsNullOrWhiteSpace(_makh))
            {
                searchLookUpEditKH.EditValue = _makh;
                loadSearchlookUpMH();
                if (!string.IsNullOrWhiteSpace(_mahang))
                {
                    searchLookUpEditMH.EditValue = _mahang;
                    loadMadot();
                    if (!string.IsNullOrWhiteSpace(_madot))
                    {
                        searchLookUpEditDot.EditValue = _madot;
                    }
                }
            }
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
            return tbl;
        }
        private DataTable createTableSaveEdit()
        {
            //cột cho bảng lưu
            DataTable tbl = new DataTable("tblSave");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("NPL", typeof(string));
            tbl.Columns.Add("Sort", typeof(int));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("DinhMucChung", typeof(Decimal));
            tbl.Columns.Add("MaDot", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DinhMucChiTiet", typeof(Decimal));
            return tbl;
        }
        private void InIt()
        {
            searchLookUpEditVatTu.Properties.ValueMember = "MaVT";
            searchLookUpEditVatTu.Properties.DisplayMember = "MaVT";

        }

        private void loadVatTuDinhMuc()
        {
            try
            {
                string url = string.Format("{0}?makh={1}&mahang={2}&madot={3}", URL + $"KhoiTaoDM/GetEditDinhMuc", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());
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
            catch (Exception ex)
            {

            }
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
                loadSearchlookUpMH();
                searchLookUpEditDot.EditValue = null;

            }
            catch (Exception ex)
            {

            }
        }
        private void loadSearchlookUpMH()
        {
            string urlMH = string.Format("{0}?makh={1}", URL + "KhoiTaoDM/GetMH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
            string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
            DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);
            searchLookUpEditMH.Properties.DataSource = tblMH;
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";
            searchLookUpEditMH.EditValue = null;
            searchLookUpEditDot.EditValue = null;
        }
        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            loadMadot();
            searchLookUpEditDot.EditValue = null;

        }
        private void loadMadot()
        {
            try
            {
                string url = string.Format("{0}?makh={1}&mahang={2}", URL + "KhoiTaoDM/GetDot", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEditDot.Properties.DataSource = tblDot;
                searchLookUpEditDot.Properties.ValueMember = "MaDot";
                searchLookUpEditDot.Properties.DisplayMember = "Dot";
            }
            catch (Exception ex)
            {

            }

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
            if (tblGrid == null || tblGrid.Rows.Count == 0) return;
            DataTable tblPost = setupTableSave(tblGrid);
            string url = string.Format("{0}", URL + "KhoiTaoDM/Post");

            foreach (DataRow row in tblGrid.Rows)
            {
                bool checkSave = CheckSave(row, row["DinhMucChung"].ToString());
                if (!checkSave && row["DinhMucChung"].ToString() != "" && row["DinhMucChung"].ToString() != "0")
                {

                    // string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;

                    string urlCT = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}&isnew={8}", URL + $"KhoiTaoDM/GetSizeDinhMucChung", row["MaKH"].ToString(), row["MaHang"].ToString(), searchLookUpEditDot.EditValue.ToString(),
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
                            rows["MaMau"] = maMauArr[0];
                            rows["TenMau"] = tenMauArr[0];
                        }
                    }
                    else
                    {
                        foreach (DataRow rows in tblSize.Rows)
                        {
                            rows["MaMau"] = maMauArr[0];
                            rows["TenMau"] = tenMauArr[0];
                        }

                        List<DataRow> originalRows = tblSize.AsEnumerable()
                                                            .Where(r => r["MaMau"].ToString() == maMauArr[0])
                                                            .ToList();

                        for (int colorIndex = 1; colorIndex < colorCount; colorIndex++)
                        {
                            foreach (DataRow baseRow in originalRows)
                            {
                                DataRow newRow = tblSize.NewRow();
                                newRow.ItemArray = baseRow.ItemArray.Clone() as object[];
                                newRow["MaMau"] = maMauArr[colorIndex];
                                newRow["TenMau"] = tenMauArr[colorIndex];
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
                        for (int i = 6; i < columnsCount; i++)
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
                            _newRow["MaDot"] = searchLookUpEditDot.EditValue;
                            _newRow["Dot"] = searchLookUpEditDot.Text.ToString().Trim();
                            _newRow["NguoiTao"] = GlobleData.UserName;
                            _newRow["NgayTao"] = null;
                            _newRow["MaMauID"] = dr["MaMau"];
                            string[] arr = tblSize.Columns[i].ToString().Split('@');
                            _newRow["MaSize"] = arr[0];
                            _newRow["DinhMuc"] = row["DinhMucChung"].ToString();
                            _newRow["TachMau"] = row["TachMau"].ToString() == "" ? false : Convert.ToBoolean(row["TachMau"].ToString());
                            tbl.Rows.Add(_newRow);
                        }

                    }
                    string url2 = $"{URL}KhoiTaoDM/PostDMChiTiet?para={row["MaMau"].ToString()}";

                    string msResult2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, tbl); }).Result;
                }

            }
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblPost); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);


                //luuDinhMucChung(tblPost);
                //luuDinhMucChungKhac(tblPost);
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
                    int index = 0;
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
                                index++;
                                tbl.Rows.Add(newRow);
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
            tbl.Columns.Add("MaMauID", typeof(string));
            int index = 0;
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
                        index++;
                        tbl.Rows.Add(newRow);
                    }


                }
            }

            string urlPOST = string.Format("{0}", URL + "KhoiTaoDM/PostDMChungNULL");

            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlPOST, tbl); }).Result;

        }


        private DataTable setupTableSave(DataTable tbl)
        {
            tblSave.Clear();
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
                _newrow["MaDot"] = searchLookUpEditDot.EditValue.ToString();
                _newrow["Dot"] = searchLookUpEditDot.Text;
                _newrow["NguoiTao"] = GlobleData.UserName;
                _newrow["NgayTao"] = null;
                _newrow["STT"] = _stt;
                _newrow["DinhMucChung"] = dr["DinhMucChung"];
                _newrow["DinhMucChiTiet"] = dr.Table.Columns.Contains("DinhMucHaoHut") ? dr["DinhMucHaoHut"] : dr["DinhMucChiTiet"];
                _newrow["TachMau"] = dr["TachMau"];
                tblSave.Rows.Add(_newrow);
            }
            return tblSave;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {



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


        private void searchLookUpEditDot_EditValueChanged(object sender, EventArgs e)
        {
            loadVatTuDinhMuc();
            loadSearchlookUpVatTu();

        }
        private void loadSearchlookUpVatTu()
        {
            if (searchLookUpEditDot.EditValue == null) return;
            string url = string.Format("{0}?makh={1}&mahang={2}&madot={3}", URL + "KhoiTaoDM/GetSuaDM", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblDM = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblDM == null || tblDM.Rows.Count == 0)
            {

                searchLookUpEditVatTu.Properties.DataSource = null;
                return;
            }
            if (!tblDM.Columns.Contains("DinhMucChung"))
            {
                tblDM.Columns.Add("DinhMucChung", typeof(double));
            }
            if (!tblDM.Columns.Contains("MaDot"))
            {
                tblDM.Columns.Add("MaDot", typeof(string));
            }
            if (!tblDM.Columns.Contains("DinhMucHaoHut"))
            {
                tblDM.Columns.Add("DinhMucHaoHut", typeof(double));
            }
            foreach (DataRow dr in tblDM.Rows)
            {
                dr["MaDot"] = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            }
            tblDM.Columns["DinhMucChung"].SetOrdinal(17);
            tblDM.Columns["MaDot"].SetOrdinal(18);
            searchLookUpEditVatTu.Properties.DataSource = tblDM;

        }
        private void gVVatTu_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
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

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", ItemDelete_Click);
                        e.Menu.Items.Add(menuDeleteItem);



                    }

                }
            }
        }
        private void ItemDelete_Click(object sender, EventArgs e)
        {
            DataRow row = gVVatTu.GetFocusedDataRow();
            if (row == null) return;
            string url = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}", URL + $"KhoiTaoDM/DeleteDM",
                  searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString()
                  , row["MaNhom"].ToString(), row["MaVTID"].ToString(), row["MauVTID"].ToString(), row["KhoVaiID"].ToString(), searchLookUpEditDot.EditValue
                  );
            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            loadVatTuDinhMuc();
        }
        string _VatTu = "", _VatTuDisplay = "";
        private void gVSearchVatTu_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (searchLookUpEditDot.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn đợt trước khi thêm vật tư.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedValues = string.Join(";", gVSearchVatTu.GetSelectedRows().Select(rowHandle2 => gVSearchVatTu.GetRowCellValue(rowHandle2, searchLookUpEditVatTu.Properties.ValueMember)));
            searchLookUpEditVatTu.EditValue = selectedValues;
            if (searchLookUpEditVatTu.EditValue is null) return;
            _VatTu = searchLookUpEditVatTu.EditValue.ToString();

            int[] selectedRowsgop = gVSearchVatTu.GetSelectedRows();
            if (selectedRowsgop.Length > 0)
            {
                foreach (int rowHandles in selectedRowsgop)
                {
                    DataRow drRow = gVSearchVatTu.GetDataRow(rowHandles);
                    int rowHandle = gVSearchVatTu.FocusedRowHandle;
                    //if (e.Action == CollectionChangeAction.Add)
                    //{

                    AddRowGCNL(drRow);
                    gridView1.SelectionChanged -= gVSearchVatTu_SelectionChanged;
                    gridView1.UnselectRow(rowHandle);
                    gridView1.SelectionChanged += gVSearchVatTu_SelectionChanged;

                    //}
                    //if (e.Action == CollectionChangeAction.Remove)
                    //{
                    //    Remove(drRow);
                    //}

                }
            }

            //if (e.Action == CollectionChangeAction.Remove)
            //{
            //    Remove(drRow);
            //}
            int totalRows = gVSearchVatTu.RowCount;
            for (int i = 0; i < totalRows; i++)
            {
                if (!Array.Exists(selectedRowsgop, selectedRow => selectedRow == i))
                {
                    DataRow drRow = gVSearchVatTu.GetDataRow(i);
                    Remove(drRow);
                }
            }
            gCVatTu.RefreshDataSource();





        }
        private void searchLookUpEditVatTu_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            _VatTuDisplay = string.Join("; ", gVSearchVatTu.GetSelectedRows().Select(rowHandle => gVSearchVatTu.GetRowCellValue(rowHandle, searchLookUpEditVatTu.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(_VatTuDisplay))
            {
                e.DisplayText = "Thêm vật tư";
            }
            else
            {
                e.DisplayText = _VatTuDisplay;
            }
        }

        private void AddRowGCNL(DataRow rowAdd)
        {
            try
            {
                DataTable tbl = gCVatTu.DataSource as DataTable;
                if (tbl == null)
                {
                    tbl = createTableSaveEdit();
                }
                //DataRow newRow = tbl.NewRow();
                ////newRow.ItemArray = rowAdd.ItemArray.Clone() as object[];
                //newRow["ID"] = rowAdd["ID"];
                //newRow["MaHang"] = rowAdd["MaHang"];
                //newRow["MaKH"] = rowAdd["MaKH"];
                //newRow["MaNhom"] = rowAdd["MaNhom"];
                //newRow["TenNhom"] = rowAdd["TenNhom"];
                //newRow["NPL"] = rowAdd["NPL"];
                //newRow["Sort"] = rowAdd["Sort"];
                //newRow["MaVTID"] = rowAdd["MaVTID"];
                //newRow["MaVT"] = rowAdd["MaVT"];
                //newRow["ChiTiet"] = rowAdd["ChiTiet"];
                //newRow["MaDVVT"] = rowAdd["MaDVVT"];
                //newRow["TenDVVT"] = rowAdd["TenDVVT"];
                //newRow["MauVTID"] = rowAdd["MauVTID"];
                //newRow["MaMauVT"] = rowAdd["MaMauVT"];
                //newRow["MauVT"] = rowAdd["MauVT"];
                //newRow["KhoVaiID"] = rowAdd["KhoVaiID"];
                //newRow["KhoVai"] = rowAdd["KhoVai"];
                //newRow["DinhMucChung"] = rowAdd["DinhMucChung"];
                //newRow["MaDot"] = rowAdd["MaDot"];
                //newRow["MaMau"] = rowAdd["MaMau"];
                //newRow["TenMau"] = rowAdd["TenMau"];
                //newRow["DinhMucHaoHut"] = rowAdd["DinhMucHaoHut"];
                //newRow["GhiChu"] = rowAdd["GhiChu"];

                //tbl.Rows.Add(newRow);
                //gCVatTu.DataSource = tbl;

                string filter = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND MaMau = '{2}' AND MaNhom = '{3}'",
                                              rowAdd["MaVTID"], rowAdd["MauVTID"], rowAdd["MaMau"], rowAdd["MaNhom"]);
                DataRow[] existingRows = tbl.Select(filter);
                if (existingRows.Length == 0)
                {
                    DataRow newRow = tbl.NewRow();
                    newRow["ID"] = rowAdd["ID"];
                    newRow["MaHang"] = rowAdd["MaHang"];
                    newRow["MaKH"] = rowAdd["MaKH"];
                    newRow["MaNhom"] = rowAdd["MaNhom"];
                    newRow["TenNhom"] = rowAdd["TenNhom"];
                    newRow["NPL"] = rowAdd["NPL"];
                    newRow["Sort"] = rowAdd["Sort"];
                    newRow["MaVTID"] = rowAdd["MaVTID"];
                    newRow["MaVT"] = rowAdd["MaVT"];
                    newRow["ChiTiet"] = rowAdd["ChiTiet"];
                    newRow["MaDVVT"] = rowAdd["MaDVVT"];
                    newRow["TenDVVT"] = rowAdd["TenDVVT"];
                    newRow["MauVTID"] = rowAdd["MauVTID"];
                    newRow["MaMauVT"] = rowAdd["MaMauVT"];
                    newRow["MauVT"] = rowAdd["MauVT"];
                    newRow["KhoVaiID"] = rowAdd["KhoVaiID"];
                    newRow["KhoVai"] = rowAdd["KhoVai"];
                    newRow["DinhMucChung"] = rowAdd["DinhMucChung"];
                    newRow["MaDot"] = rowAdd["MaDot"].ToString() == "" ? (searchLookUpEditDot.EditValue == null ? searchLookUpEditDot.EditValue.ToString() : "") : rowAdd["MaDot"].ToString();
                    newRow["MaMau"] = rowAdd["MaMau"];
                    newRow["TenMau"] = rowAdd["TenMau"];
                    newRow["DinhMucHaoHut"] = rowAdd["DinhMucHaoHut"];
                    newRow["GhiChu"] = rowAdd["GhiChu"];
                    tbl.Rows.Add(newRow);
                    gCVatTu.DataSource = tbl;

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Remove(DataRow rowAdd)
        {
            try
            {
                DataTable tbl = gCVatTu.DataSource as DataTable;
                var row = tbl.AsEnumerable().Where(x => x["MaVTID"].ToString() == rowAdd["MaVTID"].ToString() && x["MauVTID"].ToString() == rowAdd["MauVTID"].ToString()
               && x["MaMau"].ToString() == rowAdd["MaMau"].ToString() && x["MaNhom"].ToString() == rowAdd["MaNhom"].ToString()).ToList();

                tbl.Rows.Remove(row[0]);

            }
            catch (Exception ex)
            {


            }
        }
        private string GetSelectedValues(SearchLookUpEdit search, string _tencot)
        {
            List<string> selectedValues = new List<string>();
            foreach (var rowHandle in search.Properties.View.GetSelectedRows())
            {
                object value = search.Properties.View.GetRowCellValue(rowHandle, _tencot);
                if (value != null)
                {
                    selectedValues.Add(value.ToString());
                }
            }

            return string.Join(", ", selectedValues);
        }



        private void btnThemVT_Click(object sender, EventArgs e)
        {

            GridView view = searchLookUpEditVatTu.Properties.View;
            int[] selectedRowHandles = view.GetSelectedRows();

            List<DataRow> selectedRows = new List<DataRow>();

            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle >= 0)
                {
                    DataRowView rowView = view.GetRow(rowHandle) as DataRowView;
                    if (rowView != null)
                    {
                        selectedRows.Add(rowView.Row);
                    }
                }
            }
            foreach (DataRow row in selectedRows)
            {
                DataRow newRow = tblVatTu.NewRow();
                newRow.ItemArray = row.ItemArray.Clone() as object[];
                tblVatTu.Rows.Add(newRow);
            }

        }

        private void gCVatTu_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (gVVatTu.FocusedColumn.FieldName == "DinhMucHaoHut" || gVVatTu.FocusedColumn.FieldName == "DinhMuc")
                frmKhoiTaoDM.FocusFieldName(e, gVVatTu, 4);
        }

        private void loadDinhMucChiTiet()
        {
            DataRow row = gVVatTu.GetFocusedDataRow();
            if (row == null) return;
            string url = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}", URL + $"KhoiTaoDM/GetDMChiTiet",
                searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString()
                , row["MaNhom"].ToString(), row["MaVTID"].ToString(), row["MauVTID"].ToString(), row["KhoVaiID"].ToString(), searchLookUpEditDot.EditValue
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

        private void searchLookUpEditVatTu_AddNewValue(object sender, DevExpress.XtraEditors.Controls.AddNewValueEventArgs e)
        {



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
            frmNhomNguyenPhuLieu frm = new frmNhomNguyenPhuLieu();
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
            string urlCT = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}", URL + $"KhoiTaoDM/GetCheckSave",
                searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString(), searchLookUpEditDot.EditValue.ToString(), dr["MaMau"].ToString(), dr["MauVTID"].ToString(), dr["MaVTID"].ToString(), dr["KhoVaiID"].ToString());
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
            this.ActiveControl = searchLookUpEditDot;
            if (searchLookUpEditKH.EditValue != null && searchLookUpEditMH.EditValue != null)
            {
                this.ActiveControl = searchLookUpEditDot;
                DataRow dr = gVVatTu.GetFocusedDataRow();
                bool checkSave = CheckSave(dr, dr["DinhMucChung"].ToString());

                if (dr == null) return;
                frmDinhMucChiTietView frm = new frmDinhMucChiTietView(searchLookUpEditKH.EditValue.ToString(), searchLookUpEditKH.Text, searchLookUpEditMH.EditValue.ToString(), dr["TenNhom"].ToString(), dr["MaVTID"].ToString()
                                                               , dr["MaVT"].ToString(), dr["ChiTiet"].ToString(), dr["MauVT"].ToString(), dr["TenMau"].ToString(), searchLookUpEditDot.EditValue.ToString(), searchLookUpEditDot.Text,
                                                               dr["MaNhom"].ToString(), dr["MauVTID"].ToString(), dr["KhoVaiID"].ToString(), dr["TachMau"].ToString() == "" ? false : Convert.ToBoolean(dr["TachMau"].ToString()), "1", dr["MaVTGhep"].ToString(), dr["DinhMucChung"].ToString(), dr["MaMau"].ToString(), checkSave);

                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    dr["TachMau"] = frm._isCheck;
                    //if (frm._isCheck)
                    dr["IsNeww"] = 1;


                    bool checComfirm = frmDinhMucChiTietView.checkOutView;
                    if (checComfirm) loadDinhMucChiTiet();
                }

            }

        }
        private void gVVatTu_ShowingEditor(object sender, CancelEventArgs e)
        {
            //DataRow dr = gVVatTu.GetFocusedDataRow();
            //if (dr == null) return;
            //bool checkCD = CheckCanDoi(dr);
            //if (checkCD)
            //{
            //    MessageBox.Show("Vật tư này đã được nhập cấp phát.Không thể sửa định mức.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    e.Cancel = true;
            //};
        }
        private bool CheckCanDoi(DataRow dr)
        {
            string urlCT = string.Format("{0}?para={1}&para1={2}&para2={3}", URL + $"KhoiTaoDM/CheckCanDoi",
                          dr["MaDot"].ToString(), dr["MaVTID"].ToString(), dr["MauVTID"].ToString());
            string jsonCT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCT); }).Result;

            if (jsonCT == "[]") return false;
            else return true;

        }

        private void searchLookUpEditVatTu_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnOK"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnOK - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "OK"
                var okButton = new SimpleButton() { Name = "btnOK", Text = "OK" };
                okButton.Click += OkButton_Click;
                var layoutItemOK = new LayoutControlItem()
                {
                    Control = okButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(80, 30),
                    MaxSize = new Size(80, 30)
                };
                layoutItemOK.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemOK.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemOK);

                // 3. Nút "Khai báo vật tư"
                var cancelButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo vật tư" };
                cancelButton.Click += cancelButton_Click;
                var layoutItemCancel = new LayoutControlItem()
                {
                    Control = cancelButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemCancel.OptionsTableLayoutItem.ColumnIndex = 2;
                layoutItemCancel.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemCancel);

                // 4. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }
        private void OkButton_Click(object sender, EventArgs e)
        {
            searchLookUpEditVatTu.ClosePopup();
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            string makh = searchLookUpEditKH.EditValue != null ? searchLookUpEditKH.EditValue.ToString() : "";
            string mahang = searchLookUpEditMH.EditValue != null ? searchLookUpEditMH.EditValue.ToString() : "";
            frmERP_VatTu frm = new frmERP_VatTu(makh, mahang);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.FormClosing += (s, args) =>
            {
                loadSearchlookUpVatTu();

            };
            frm.ShowDialog();
        }
    }
}

