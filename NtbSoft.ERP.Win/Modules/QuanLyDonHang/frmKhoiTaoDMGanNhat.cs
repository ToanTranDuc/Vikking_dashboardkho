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
    public partial class frmKhoiTaoDMGanNhat : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _makh = string.Empty, _mahang = string.Empty, _mavtID = string.Empty, _mauID = string.Empty, _tenVT = string.Empty, _mauVT = string.Empty, _nhomVT = string.Empty,
                       _khachhang = string.Empty, _tenhang = string.Empty, _chitiet = string.Empty,_madot=string.Empty;


        private int _stt = 0;
        DataTable _dt = new DataTable();
        DataTable tblVatTu = new DataTable();
        DataTable tblSave;
        int indexFocusedRow = -1;

        Dictionary<int, object> dataDict = new Dictionary<int, object>();
        public frmKhoiTaoDMGanNhat(string makh="", string mahang="",string madot="",int stt=0)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            _makh = makh;
            _mahang = mahang;
            _stt = stt;
        }
        protected override void OnLoad(EventArgs e)
        {
            tblSave = createTableSave();
            CreateSearchLookupKH();
            if(!string.IsNullOrWhiteSpace(_makh))
            {
                searchLookUpEditKH.EditValue = _makh;
                loadSearchLookUpMH();
                if(!string.IsNullOrWhiteSpace(_mahang))
                {
                    searchLookUpEditMH.EditValue = _mahang;
                }    
            }
            loadMaDot();
            loadVatTuDinhMuc();
        }
        private void loadMaDot()
        {
            string url = string.Format("{0}?para={1}&para1={2}&para2={3}", URL + $"KhoiTaoDM/GetMaDot", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString(),_stt);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                this.DialogResult = DialogResult.OK;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _madot = tbl.Rows[0][0].ToString();
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
            return tbl;
        }
        private void loadVatTu()
        {
            string url = string.Format("{0}?makh={1}&mahang={2}", URL + $"KhoiTaoDM/Get", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString());
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
            gCVatTu.DataSource = tblVatTu;

        }
        private void loadVatTuDinhMuc()
        {
            //truyền xuống sai mã đợt
         
           
            string url = string.Format("{0}?makh={1}&mahang={2}&madot={3}", URL + $"KhoiTaoDM/GETDMGN", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString(), _madot);
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
          
            gCVatTu.DataSource = tblVatTu;

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
        }
        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            //loadVatTu();

            string url = string.Format("{0}?makh={1}&mahang={2}", URL + "KhoiTaoDM/GetSttDot", searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _stt = Convert.ToInt32(tbl.Rows[0][0]);

            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            txtNhapDot.Text = searchLookUpEditKH.Text.ToString() + '-' + searchLookUpEditMH.Text.ToString() + '-' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;

            barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;


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
            string url = string.Format("{0}", URL + "KhoiTaoDM/Post");

            foreach (DataRow row in tblGrid.Rows)
            {
                bool checkSave = CheckSave(row, row["DinhMucChung"].ToString());
                if (!checkSave && row["DinhMucChung"].ToString() != "" && row["DinhMucChung"].ToString() != "0")
                {
                    string url2 = $"{URL}KhoiTaoDM/SaveSizeSP?para={row["MaMau"].ToString()}";
                    string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
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

                    DataRow newRow = tbl.NewRow();
                    newRow["ID"] = 0;
                    newRow["MaKH"] = row["MaKH"];
                    newRow["MaHang"] = row["MaHang"];
                    newRow["MaNhom"] = row["MaNhom"];
                    newRow["MaVTID"] = row["MaVTID"];
                    newRow["MauVTID"] = row["MauVTID"];
                    newRow["KhoVaiID"] = row["KhoVaiID"];
                    newRow["MaNhomSize"] = "";
                    newRow["MaSize"] = "";
                    newRow["DinhMuc"] = row["DinhMucChung"].ToString();
                    newRow["MaDot"] = madot;
                    newRow["Dot"] = txtNhapDot.Text;
                    newRow["NguoiTao"] = GlobleData.UserName;
                    newRow["NgayTao"] = null;
                    tbl.Rows.Add(newRow);

                    string msResult2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, tbl); }).Result;
                }
              
            }
           
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

                if (_newrow["DinhMucChung"].ToString() != "" && !string.IsNullOrWhiteSpace(_newrow["DinhMucChung"].ToString())&& _newrow["DinhMucChung"].ToString() != "0")
                {
                    tblSave.Rows.Add(_newrow);
                }

            }
            return tblSave;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            loadVatTuDinhMuc();

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
                XtraMessageBox.Show("Xóa dòng đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DataRow row = gVVatTu.GetFocusedDataRow();
            if (row == null) return;
            string url = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}", URL + $"KhoiTaoDM/GetDMChiTiet",
                searchLookUpEditKH.EditValue.ToString() == "" ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString() == "" ? "" : searchLookUpEditMH.EditValue.ToString()
                , row["MaNhom"].ToString(), row["MaVTID"].ToString(), row["MauVTID"].ToString(), row["KhoVaiID"].ToString(), ReplaceSpecialCharacters(txtNhapDot.Text)
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
            string urlCT = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}", URL + $"KhoiTaoDM/GetCheckSave",
                searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString(), ReplaceSpecialCharacters(txtNhapDot.Text), dr["MaMau"].ToString(), dr["MauVTID"].ToString(), dr["MaVTID"].ToString(), "");
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

                if (dr == null) return;
                frmDinhMucChiTiet frm = new frmDinhMucChiTiet(searchLookUpEditKH.EditValue.ToString(), searchLookUpEditKH.Text, searchLookUpEditMH.EditValue.ToString(), dr["TenNhom"].ToString(), dr["MaVTID"].ToString()
                                                               , dr["MaVT"].ToString(), dr["ChiTiet"].ToString(), dr["MauVT"].ToString(), dr["TenMau"].ToString(), ReplaceSpecialCharacters(txtNhapDot.Text), txtNhapDot.Text,
                                                               dr["MaNhom"].ToString(), dr["MauVTID"].ToString(), dr["KhoVaiID"].ToString(), false, "1", dr["MaVTGhep"].ToString(), dr["DinhMucChung"].ToString(), dr["MaMau"].ToString(),checkSave);

                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                bool checComfirm = frmDinhMucChiTiet.checkOutView;
                if (checComfirm) loadDinhMucChiTiet();
            }

        }
    }
}

