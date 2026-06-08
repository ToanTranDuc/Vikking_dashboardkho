using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraGrid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmKhaiBaoALL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _nguoiDD = string.Empty, _ma;
        bool checkSaveT = true;
        DataTable tblKhaiBao;
        DataTable tblKhaiBaoSize;   
        DataTable tblCodeSize;
        private HttpClientExtension _clientExtension;
        public frmKhaiBaoALL()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblKhaiBao = new DataTable();
            tblKhaiBaoSize = new DataTable();
            tblCodeSize = new DataTable();
            InIt();
            GetKH();
            GetNS();
            GetTS();
            GetMaHang();
            DataTableKBMau();
            GetMauKB();
        }
        private void DataTableKBMau()
        {
            tblKhaiBao.Columns.Add("MaHang", typeof(string));
            tblKhaiBao.Columns.Add("TenHang", typeof(string));
            tblKhaiBao.Columns.Add("KhachHang", typeof(string));
            tblKhaiBao.Columns.Add("TenKH", typeof(string));
            tblKhaiBao.Columns.Add("MaMau", typeof(string));
            tblKhaiBao.Columns.Add("TenMau", typeof(string));
            tblKhaiBao.Columns.Add("NhomSize", typeof(string));
            tblKhaiBao.Columns.Add("TenNhomSize", typeof(string));
            tblKhaiBao.Columns.Add("SizeID", typeof(string));
            tblKhaiBao.Columns.Add("Size", typeof(string));
            tblKhaiBao.Columns.Add("DateThucHien", typeof(string));
            tblKhaiBao.Columns.Add("NguoiDaiDien", typeof(string));
            tblKhaiBao.Columns.Add(new DataColumn("IsNew", typeof(bool)) { DefaultValue = false });
            tblKhaiBaoSize = tblKhaiBao.Clone();

            tblCodeSize.Columns.Add("MaHang", typeof(string));
            tblCodeSize.Columns.Add("TenHang", typeof(string));
            tblCodeSize.Columns.Add("MaMau", typeof(string));
            tblCodeSize.Columns.Add("TenMau", typeof(string));
            tblCodeSize.Columns.Add("NguoiDaiDien", typeof(string));

        }
        private void comboBoxEdit11_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxMH.EditValue.ToString() == "Đã khai báo")
            {
                NameMHB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameMHA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (Convert.ToBoolean(barEditItem2.EditValue))
                {
                    MessageBox.Show("Hiện tại bạn đang chọn copy thông số mã hàng.Vui lòng không chọn đã khai báo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbxMH.EditValue = "Khai báo mới";
                    return;
                }
            }
            else
            {
                NameMHB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                NameMHA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                cbxNS.EditValueChanged -= cbxMH1_Properties_EditValueChanged;
                cbxNS.EditValue = "Khai báo mới";
                NameNSA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (!Convert.ToBoolean(barEditItem2.EditValue))
                    NameNSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                cbxNS.EditValueChanged += cbxMH1_Properties_EditValueChanged;
            }
        }

        private void comboBoxEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxKH.EditValue.ToString() == "Đã khai báo")
            {
                NameKHB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameKHA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtKH.EditValue = null;
                txtKH.Properties.NullValuePrompt = "Nhập khách hàng";
            }
            else
            {
                NameKHB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                NameKHA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                searchLookUpEditKH.EditValue = null;
                searchLookUpEditKH.Properties.NullValuePrompt = "Chọn khách hàng";
            }
            GetMaHang();
        }
        private void cbxMH1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxMH.EditValue.ToString() != "Đã khai báo")
            {
                cbxNS.EditValueChanged -= cbxMH1_Properties_EditValueChanged;
                MessageBox.Show("Mã hàng chưa được khai báo.Nhóm size phải tự khai báo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxNS.EditValue = "Khai báo mới";
                cbxNS.EditValueChanged += cbxMH1_Properties_EditValueChanged;
                return;
            }
            if (cbxNS.EditValue == "Đã khai báo")
            {
                NameNSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameNSA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (searchLookUpEditMH.EditValue != null && searchLookUpEditMH.EditValue != "")
                    GetNS();
            }
            else
            {
                NameNSA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameNSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }

        }
        private void ResetValue()
        {
            txtKH.EditValue = null;
            txtKH.Properties.NullValuePrompt = "Nhập khách hàng";
            txtNDD.EditValue = null;
            txtMH.EditValue = null;
            txtMH.Properties.NullValuePrompt = "Nhập mã hàng";
            txtMau.EditValue = null;
            txtMau.Properties.NullValuePrompt = "M1:M2";
            txtNhomSize.EditValue = null;
            txtNhomSize.Properties.NullValuePrompt = "N1:N2";
            txtSize.EditValue = null;
            txtSize.Properties.NullValuePrompt = "S1:S2";
            

            searchLookUpEditKH.EditValue = null;
            searchLookUpEditKH.Properties.NullValuePrompt = "Chọn khách hàng";

            searchLookUpEditMH.EditValue = null;
            searchLookUpEditMH.Properties.NullValuePrompt = "Chọn mã hàng";

            searchLookUpEditMH1.EditValue = null;
            searchLookUpEditMH1.Properties.NullValuePrompt = "Chọn mã hàng copy";

            searchLookUpEditNS.EditValue = null;
            searchLookUpEditNS.Properties.NullValuePrompt = "Chọn nhóm size";

            grcThongSo.DataSource = null;
            gridcodeMau.DataSource = null;
            grcTHSize.DataSource = null;

            searchLookUpEdit1.EditValue = null;
            searchLookUpEdit2.EditValue = null;

            txtKH.EditValue = "";
            txtMH.EditValue = "";
            txtMau.EditValue = "";
            txtNhomSize.EditValue = "";
            txtSize.EditValue = "";
            GetNS();
            GetTS();
            GetMaHang();
            txtVietTatKH.EditValue = null;

        }
        private void GetMauKB()
        {
            string url = $"{URL}KhaiBaoAll/Get?action=GetMauKB";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditMauKB.Properties.DataSource = tbl;
        }
        private void GetSize()
        {
            try
            {
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string nhomsize = searchLookUpEditNS.EditValue == null ? "" : searchLookUpEditNS.EditValue.ToString();
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string url = $"{URL}KhaiBaoAll/Get?action=GetSize&para1={mahang}&para2={nhomsize}&para3={makh}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    txtSize.EditValue = "";
                    return;
                }
                string joinedSize = string.Join(":", tbl.AsEnumerable().Select(x => x["TenSize"].ToString()).Distinct());
                txtSize.EditValue = joinedSize;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetNS()
        {
            try
            {

                string mahang = "";
                string makh = "";
                string url = $"{URL}KhaiBaoAll/Get?action=GetNS&para1={mahang}&para2={makh}";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                    return;
                searchLookUpEdit3.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetTS()
        {
            try
            {

                string mahang = "";
                string makh = "";
                string url = $"{URL}KhaiBaoAll/Get?action=GetSizeS&para1={mahang}&para2={makh}";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                    return;
                searchLookUpEdit4.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                checkSaveT = true;
                GetDataTable();
            }
            catch (Exception ex)
            {


            }
        }
        private void InIt()
        {
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            searchLookUpEditKH.Properties.NullValuePrompt = "Chọn khách hàng";

            searchLookUpEdit1.Properties.ValueMember = "MaKH";
            searchLookUpEdit1.Properties.DisplayMember = "TenKH";
            searchLookUpEdit1.Properties.NullValuePrompt = "Chọn khách hàng";

            searchLookUpEdit2.Properties.ValueMember = "MaHang";
            searchLookUpEdit2.Properties.DisplayMember = "TenHang";
            searchLookUpEdit2.Properties.NullValuePrompt = "Chọn khách hàng";

            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";
            searchLookUpEditMH.Properties.NullValuePrompt = "Chọn mã hàng";

            searchLookUpEditNS.Properties.ValueMember = "MaNhomSize";
            searchLookUpEditNS.Properties.DisplayMember = "NhomSize";
            searchLookUpEditNS.Properties.NullValuePrompt = "Chọn nhóm size";

            searchLookUpEdit3.Properties.ValueMember = "MaNhomSize";
            searchLookUpEdit3.Properties.DisplayMember = "NhomSize";
            searchLookUpEdit3.Properties.NullValuePrompt = "";

            searchLookUpEdit4.Properties.ValueMember = "MaSize";
            searchLookUpEdit4.Properties.DisplayMember = "TenSize";
            searchLookUpEdit4.Properties.NullValuePrompt = "";

            searchLookUpEditMH1.Properties.ValueMember = "MaHang";
            searchLookUpEditMH1.Properties.DisplayMember = "TenHang";
            searchLookUpEditMH1.Properties.NullValuePrompt = "Chọn mã hàng copy";

            searchLookUpEditMauKB.Properties.ValueMember = "MaMau";
            searchLookUpEditMauKB.Properties.DisplayMember = "TenMau";
        }
        private void GetKH()
        {
            try
            {
                string url = $"{URL}KhaiBaoAll/Get?action=GetKH";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditKH.Properties.DataSource = null;
                    searchLookUpEditMH.Properties.DataSource = null;
                    return;
                }
                searchLookUpEdit1.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }

        }
        private void GetMaHang()
        {
            try
            {
                string makh = "";
                string url = $"{URL}KhaiBaoAll/Get?action=GetMH&para1={makh}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEdit2.Properties.DataSource = null;
                    return;
                }
                searchLookUpEdit2.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetMaHangCopy()
        {
            try
            {
                string makh = cbxKH.EditValue.ToString() == "Đã khai báo" ? searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString() : "";
                string url = $"{URL}KhaiBaoAll/Get?action=GetMH&para1={makh}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditMH1.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditMH1.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetDataTable()
        {
            if ((searchLookUpEditMH1.EditValue?.ToString() ?? "") == "" && (searchLookUpEditMH.EditValue?.ToString() ?? "") == "" && txtMH.EditValue?.ToString() == "")
            {
                MessageBox.Show("Vui lòng chọn mã hàng hoặc nhập mã hàng ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Convert.ToBoolean(barEditItem2.EditValue)) ViewNorMal();
            if (Convert.ToBoolean(barEditItem2.EditValue) && checkSaveT) ViewCopyMH();
        }
        private void ViewNorMal()
        {
            DataTable tbl = gridcodeMau.DataSource as DataTable;
            bool checkNS = cbxNS.EditValue.ToString() == "Đã khai báo";
            string listCodemauTable = tbl == null || tbl.Rows.Count == 0 ? "" : string.Join(":", tbl.AsEnumerable().Select(x => x["NguoiDaiDien"].ToString()));
            string[] listMau = txtMau.EditValue?.ToString().Split(':').Where(s => !string.IsNullOrEmpty(s)).ToArray() ?? new string[] { };
            string[] listCodeMau = tbl == null || tbl.Rows.Count == 0 ? new string[] { } : listCodemauTable.Split(':').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            string[] listNhomSize = checkNS ?  searchLookUpEditNS.EditValue?.ToString().Split(':') ?? new string[] { "0" } :
                txtNhomSize.EditValue == null || txtNhomSize.EditValue.ToString() == "" ? new string[] { "0" } : txtNhomSize.EditValue.ToString().Split(':').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            string[] listSize = txtSize.EditValue == null || txtSize.EditValue.ToString() == "" ? new string[] { "0" } : txtSize.EditValue.ToString().Split(':').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            int indexCodeMau = 0;
            //if (listNhomSize.Length > 0 && listSize.Length == 0)
            //{
            //    MessageBox.Show("Vui lòng nhập size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            if (listNhomSize.Length > 0)
                ParaSize(listNhomSize, listSize);
            if (listMau.Length > 0) ParaMau(listMau, listCodeMau);
        }
        private int GetMaxSortSize(string nhomsizeValue)
        {
            string mahang = txtMH.EditValue?.ToString() ?? "";
            DataRow row = gridView2.GetFocusedDataRow();
           
            string nhomsize = nhomsizeValue;
            string makh = row == null ? txtKH.EditValue?.ToString() : row["MaKH"].ToString();
            string url = $"{URL}KhaiBaoAll/Get?action=GetSortSize&para1={mahang}&para2={nhomsize}&para3={makh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                return 1;
            }
            return Convert.ToInt32(tbl.Rows[0]["Sort"]) + 1;
        }
        private List<string> ListTS()
        {
            bool checkKH = cbxKH.EditValue.ToString() == "Đã khai báo";
            bool checkMH = cbxMH.EditValue.ToString() == "Đã khai báo";

            string MaKH = checkKH ? ReplaceSpecialCharacters(RemoveVietnameseTone(searchLookUpEditKH.Text.ToString())).ToString().ToUpper()
                                   : ReplaceSpecialCharacters(RemoveVietnameseTone(txtKH.EditValue?.ToString().Trim() ?? "")).ToString().ToUpper();
            string TenKH = checkKH ? MaKH == "" ? "" : searchLookUpEditKH.Text.ToString() : txtKH.EditValue?.ToString() ?? "";
            string MaHang = checkMH ? searchLookUpEditMH.EditValue?.ToString() ?? ""
                                    : ReplaceSpecialCharacters(RemoveVietnameseTone(txtMH.EditValue?.ToString().Trim() ?? "")).ToString().ToUpper();

            string TenHang = checkMH ? MaHang == "" ? "" : searchLookUpEditMH.Text?.ToString().Trim() ?? "" : txtMH.EditValue?.ToString().Trim() ?? "";
            List<string> ListPara = new List<string> { MaKH, TenKH, MaHang, TenHang };
            return ListPara;
        }
        int minNotiSort = 0;
        private void ParaSize(string[] listNhomSize, string[] listSize)
        {
            List<string> ListPara = ListTS();
            if (!tblKhaiBaoSize.Columns.Contains("IsNew"))
            {
                tblKhaiBaoSize.Columns.Add(new DataColumn("IsNew", typeof(bool)) { DefaultValue = false });

            }
            DataTable tbl = tblKhaiBaoSize.Clone();
            string maKH = MaKH();
            string maHang = MaHang(maKH);
            DataTable tblSize = GetDSSize(maKH, maHang);
            foreach (var _nhomsize in listNhomSize)
            {
                string NhomSize = ReplaceSpecialCharacters(RemoveVietnameseTone(_nhomsize.ToString())).ToString().ToUpper();
                string TenNhomSize = _nhomsize.ToString();
                int indexSize = GetMaxSortSize(NhomSize);
                foreach (var _size in listSize)
                {
                    string Size = $"SIZE_{ReplaceSpecialCharacters(RemoveVietnameseTone(_size.ToString())).ToString().ToUpper()}";
                    string TenSize = _size.ToString();

                    var drRowDub = tblSize.AsEnumerable().Where(x => x["MaNhomSize"].ToString() == NhomSize && x["TenSize"].ToString() == TenSize).FirstOrDefault();
                    if (drRowDub != null)
                    {
                        MessageBox.Show($"Tên size {TenSize} đã được khai báo ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    DataRow newRow = tbl.NewRow();
                    newRow["MaHang"] = ListPara[2];
                    newRow["TenHang"] = ListPara[3];
                    newRow["KhachHang"] = ListPara[0];
                    newRow["TenKH"] = ListPara[1];
                    newRow["MaMau"] = indexSize;
                    newRow["TenMau"] = "";
                    newRow["NhomSize"] = NhomSize;
                    newRow["TenNhomSize"] = TenNhomSize;
                    newRow["SizeID"] = Size;
                    newRow["Size"] = TenSize;
                    newRow["IsNew"] = false;
                    newRow["DateThucHien"] = "0";
                    newRow["NguoiDaiDien"] = "";
                    tbl.Rows.Add(newRow);
                    indexSize++;
                }
            }
            if (checkSaveT)
                grcTHSize.DataSource = tbl;
            else tblKhaiBaoSize = tbl.Copy();
        }
        public string MaKH()
        {
            string tenKH = txtKH.Text.ToString();
            string url = $"{URL}KhaiBaoAll/Get?action=GetMaKH&para1={tenKH}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return "";
            return tbl.Rows[0]["MaKH"].ToString();
        }
        public string MaHang(string makh)
        {
            string tenKH = txtMH.Text.ToString();
            string url = $"{URL}KhaiBaoAll/Get?action=GetMaHang&para1={tenKH}&para2={makh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return "";
            return tbl.Rows[0]["MaHang"].ToString();
        }
        public DataTable GetDSMau(string makh,string mahang)
        {
            string url = $"{URL}KhaiBaoAll/Get?action=GetMauDub&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return new DataTable();
            return tbl;
        }
        public DataTable GetDSSize(string makh, string mahang)
        {
            string url = $"{URL}KhaiBaoAll/Get?action=GetSizeDub&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return new DataTable();
            return tbl;
        }
        private void ParaMau(string[] listMau, string[] listCodeMau)
        {
            List<string> ListPara = ListTS();
            DataTable tbl = tblKhaiBaoSize.Clone();
            int indexCodeMau = 0;
            int indexMau = 1;
            string maKH = MaKH();
            string maHang = MaHang(maKH);
            DataTable tblMau = GetDSMau(maKH, maHang);
            foreach (var _mau in listMau)
            {
                string MaMau = ReplaceSpecialCharacters(RemoveVietnameseTone(_mau.ToString())).ToString().ToUpper();
                string TenMau = _mau.ToString();
                string CodeMau = (indexCodeMau >= 0 && indexCodeMau < listCodeMau.Length) ? listCodeMau[indexCodeMau] : "";
                if (!tbl.Columns.Contains("IsNew"))
                    tbl.Columns.Add(new DataColumn("IsNew", typeof(bool)) { DefaultValue = false });
                var drRowDub = tblMau.AsEnumerable().Where(x => x["TenMau"].ToString() == TenMau).FirstOrDefault();
                if(drRowDub != null)
                {
                    MessageBox.Show($"Tên màu {TenMau} đã được khai báo ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                       return;
                }
                DataRow newRow = tbl.NewRow();
                newRow["MaHang"] = ListPara[2];
                newRow["TenHang"] = ListPara[3];
                newRow["KhachHang"] = ListPara[0];
                newRow["TenKH"] = ListPara[1];
                newRow["MaMau"] = MaMau;
                newRow["TenMau"] = TenMau;
                newRow["NhomSize"] = "";
                newRow["TenNhomSize"] = "";
                newRow["SizeID"] = "";
                newRow["Size"] = "";
                newRow["DateThucHien"] = indexMau;
                newRow["NguoiDaiDien"] = CodeMau;
                newRow["IsNew"] = false;
                tbl.Rows.Add(newRow);
                indexCodeMau++;
                indexMau++;
            }
            if (checkSaveT)
                grcThongSo.DataSource = tbl;
            else tblKhaiBao = tbl.Copy();
        }
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            searchLookUpEditMH.EditValue = null;
            searchLookUpEditMH.Properties.NullValuePrompt = "Chọn mã hàng";

            searchLookUpEditNS.EditValue = null;
            searchLookUpEditNS.Properties.NullValuePrompt = "Chọn nhóm size";
            if (cbxNS.EditValue.ToString() == "Đã khai báo")
            {
                txtSize.EditValue = null;
                txtSize.Properties.NullValuePrompt = "S1:S2";
            }

            GetMaHang();
            GetMaHangCopy();

        }
        private bool CheckTextNull()
        {
            try
            {
                //string CheckKHSearch = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                //string CheckKHText = txtKH.EditValue == null ? "" : txtKH.EditValue.ToString();
                //if (cbxKH.EditValue.ToString() != "Đã khai báo" && CheckKHText == ""
                //    || cbxKH.EditValue.ToString() == "Đã khai báo" && CheckKHSearch == "")
                //{
                //    PopNotifi("khách hàng");
                //    return true;
                //}
                //bool checkKH = CheckTrungKH();
                //if (checkKH) return true;

                //string CheckMHSearch = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                //string CheckMHText = txtMH.EditValue == null ? "" : txtMH.EditValue.ToString();
                //if (cbxMH.EditValue.ToString() != "Đã khai báo" && CheckMHText == ""
                //    || cbxMH.EditValue.ToString() == "Đã khai báo" && CheckMHSearch == "")
                //{
                //    PopNotifi("mã hàng");
                //    return true;
                //}
                bool CheckMH = CheckTrungMH();
                if (CheckMH) return true;

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool CheckTrungMH()
        {
            try
            {
                bool checkMH = cbxMH.EditValue.ToString() == "Đã khai báo";
                bool checkKH = cbxKH.EditValue.ToString() == "Đã khai báo";
                string TenKH = checkKH ? searchLookUpEditKH.Text.ToString() : txtKH.EditValue.ToString();
                if (checkMH) return false;
                string mahang = txtMH.EditValue?.ToString() ?? "";
                string url = $"{URL}KhaiBaoAll/Get?action=GetCheckMH&para1={mahang}&para2={TenKH}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count != 0)
                {
                    CheckTrungNotifi($"Mã hàng {mahang}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        private bool CheckTrungKH()
        {
            try
            {
                bool checkKH = cbxKH.EditValue.ToString() == "Đã khai báo";
                if (checkKH) return false;

                string khachhang = ReplaceSpecialCharacters(RemoveVietnameseTone(txtKH.EditValue?.ToString() ?? "")).ToString().ToUpper();
                string url = $"{URL}KhaiBaoAll/Get?action=GetCheckKH&para1={khachhang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count != 0)
                {
                    CheckTrungNotifi($"Khách hàng {txtKH.EditValue?.ToString()}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {

            }

        }
        private void Save()
        {
            string txtVTKh = txtVietTatKH.EditValue.ToString();
            if(txtVTKh == "")
            {
                MessageBox.Show("Vui lòng nhập tên viết tắt khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            tblKhaiBao.Clear();
            tblKhaiBaoSize.Clear();
            //bool checkKHTrung = CheckTrungKH();
            //bool checkMHTrung = CheckTrungMH();
            //if (checkKHTrung || checkMHTrung) return;
            this.ActiveControl = searchLookUpEditKH;
            //bool checkData = CreateData();
            //if (!checkData) return;
            checkSaveT = false;

            string maKH = MaKH();
            string maHang = MaHang(maKH);

            string checkKH = maKH != "" ? "0" : "1";
            string checkMH = maHang != "" ? "0" : "1";
            string mahangcheck = checkKH == "0" ? maKH
                                   : ReplaceSpecialCharacters(RemoveVietnameseTone(txtKH.EditValue?.ToString().Trim() ?? "")).ToString().ToUpper();

            GetDataTable();
            DataTable tblThongSoMau = grcThongSo.DataSource as DataTable;
            DataTable tblThongSoSize = grcTHSize.DataSource as DataTable;

            //return;
            if (tblThongSoMau != null && tblThongSoMau.Rows.Count != 0)
            {
                tblKhaiBao.Clear();
                tblKhaiBao = tblThongSoMau.Copy();
            }
            if (tblThongSoSize != null && tblThongSoSize.Rows.Count != 0)
            {
                tblKhaiBaoSize.Clear();
                tblKhaiBaoSize = tblThongSoSize.Copy();
            }
            foreach (DataRow item in tblKhaiBao.Rows)
            {
                if ((item["MaHang"]?.ToString() ?? "") == "")
                {
                    MessageBox.Show("Vui lòng chọn mã hàng hoặc nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (item["TenMau"].ToString() == "")
                {
                    MessageBox.Show("Vui lòng đầy đủ tên màu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (item["NguoiDaiDien"].ToString() == "" && Convert.ToBoolean(barEditItem1.EditValue))
                {
                    MessageBox.Show("Vui lòng đầy đủ code màu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                item["MaMau"] = ReplaceSpecialCharacters(RemoveVietnameseTone(item["TenMau"].ToString())).ToString().ToUpper();
            }
            foreach (DataRow item in tblKhaiBaoSize.Rows)
            {
                if ((item["MaHang"]?.ToString() ?? "") == "")
                {
                    MessageBox.Show("Vui lòng chọn mã hàng hoặc nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (item["TenNhomSize"].ToString() == "")
                {
                    item["TenNhomSize"] = "0";
                }
                if (item["Size"].ToString() == "")
                {
                    MessageBox.Show("Vui lòng đầy đủ tên size", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                item["NhomSize"] = ReplaceSpecialCharacters(RemoveVietnameseTone(item["TenNhomSize"].ToString())).ToString().ToUpper();
                item["SizeID"] = "SIZE_" + ReplaceSpecialCharactersPLUS(RemoveVietnameseTone(item["Size"].ToString())).ToString().ToUpper();
            }

            bool checkSave = false;
            if (tblKhaiBao != null && tblKhaiBao.Rows.Count != 0)
            {
                tblKhaiBao.Columns.Remove("IsNew");
                string url = $"{URL}KhaiBaoAll/Post?action=Post&para1={checkKH}&para2={checkMH}&para3={mahangcheck}&para4={txtVTKh}";
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblKhaiBao); }).Result;
                if (json.ToUpper() == "TRUE")
                    checkSave = true;
            }
            if (tblKhaiBaoSize != null && tblKhaiBaoSize.Rows.Count != 0)
            {
                tblKhaiBaoSize.Columns.Remove("IsNew");
                string url = $"{URL}KhaiBaoAll/Post?action=PostSize&para1={checkKH}&para2={checkMH}&para3={mahangcheck}&para4={txtVTKh}";
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblKhaiBaoSize); }).Result;
                if (json.ToUpper() == "TRUE")
                    checkSave = true;
            }
            if (checkSave)
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                if (!Convert.ToBoolean(barEditItem2.EditValue))
                {
                    GetKH();
                    GetMaHang();
                    ResetValue();
                    GetMauKB();
                }
                else
                {
                    grcThongSo.DataSource = null;
                    grcTHSize.DataSource = null;
                    GetMaHangCopy();
                }
            }
            else clsWaitForm.ShowErrorForm(this, 2000);
            checkSaveT = true;
        }
        private void PopNotifi(string value)
        {
            MessageBox.Show($"Vui lòng nhập {value}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void CheckTrungNotifi(string value)
        {
            MessageBox.Show($"{value} đã tồn tại trong bảng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        static string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }
        static string ReplaceSpecialCharactersPLUS(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(input, replacement);
            int plusCount = input.Count(c => c == '+');
            result += new string('_', plusCount);

            return result;
        }
        private void txtSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (cbxNS.EditValue.ToString() != "Đã khai báo" && txtNhomSize.EditValue == null || txtNhomSize.EditValue == "")
            //{
            //    PopNotifi("nhóm size");
            //    e.Handled = true;
            //    return;
            //}
            if (IsVietnameseChar(e.KeyChar))
                e.Handled = true;
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void txtMau_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (/*IsVietnameseChar(e.KeyChar) ||*/ e.KeyChar == '_')
            {
                e.Handled = true;
            }
            e.KeyChar = char.ToUpper(e.KeyChar);

        }

        private void txtNhomSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (/*IsVietnameseChar(e.KeyChar) ||*/ e.KeyChar == '_')
                e.Handled = true;
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void txtKH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (/*IsVietnameseChar(e.KeyChar) ||*/ e.KeyChar == '_')
                e.Handled = true;
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void txtMH_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (/*IsVietnameseChar(e.KeyChar) ||*/ e.KeyChar == '_')
                e.Handled = true;
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private bool IsVietnameseChar(char c)
        {
            string vietnameseChars = "àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ";
            return vietnameseChars.Contains(c);
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxNS.EditValue.ToString() == "Đã khai báo" && searchLookUpEditMH.EditValue != null && searchLookUpEditMH.EditValue.ToString() != "")
                GetNS();
        }

        private void searchLookUpEditNS_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxNS.EditValue.ToString() == "Đã khai báo") GetSize();
        }

        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(barEditItem1.EditValue))
            {
                layoutControlGroup2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else layoutControlGroup2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }


        private void gridcodeMau_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (grvCodeMau.FocusedColumn.FieldName == "NguoiDaiDien")
            {
                if (/*IsVietnameseChar(e.KeyChar) ||*/ e.KeyChar == '_')
                    e.Handled = true;

                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }




        private void grcThongSo_EditorKeyPress(object sender, KeyPressEventArgs e)
        {

            if (grvThongSo.FocusedColumn.FieldName == "NguoiDaiDien" || grvThongSo.FocusedColumn.FieldName == "TenMau")
            {
                if (/*IsVietnameseChar(e.KeyChar) ||*/ e.KeyChar == '_')
                    e.Handled = true;

                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void searchLookUpEditMH1_EditValueChanged(object sender, EventArgs e)
        {
            ViewCopyMH();
        }

        private void txtKH_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(barEditItem2.EditValue) && (searchLookUpEditMH1.EditValue?.ToString() ?? "") != "")
            {
                List<string> ListPara = ListTS();
                DataTable tblThongSoMau = grcThongSo.DataSource as DataTable;
                DataTable tblThongSoSize = grcTHSize.DataSource as DataTable;
                if (tblThongSoMau != null && tblThongSoMau.Rows.Count != 0)
                {
                    tblThongSoMau.AsEnumerable().ToList().ForEach(item =>
                    {
                        item["MaHang"] = ListPara[2];
                        item["TenHang"] = ListPara[3];
                        item["KhachHang"] = ListPara[0];
                        item["TenKH"] = ListPara[1];
                    });
                }
                if (tblThongSoSize != null && tblThongSoSize.Rows.Count != 0)
                {
                    tblThongSoSize.AsEnumerable().ToList().ForEach(item =>
                    {
                        item["MaHang"] = ListPara[2];
                        item["TenHang"] = ListPara[3];
                        item["KhachHang"] = ListPara[0];
                        item["TenKH"] = ListPara[1];
                    });
                }
                //MauCopy();
                //Size();
            }
        }

        private void Size()
        {
            try
            {
                bool checkKH = cbxKH.EditValue.ToString() == "Đã khai báo";
                string mahang = searchLookUpEditMH1.EditValue.ToString();
                string makh = checkKH ? searchLookUpEditKH.EditValue?.ToString() ?? "All" : "All";
                string url = $"{URL}KhaiBaoAll/Get?action=GetNSSize&para1={mahang}&para2={makh}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0) return;
                DataTable tbl2 = tblKhaiBao.Clone();
                List<string> ListPara = ListTS();
                int sortSize = 0;
                if (!tbl2.Columns.Contains("IsNew"))
                    tbl2.Columns.Add(new DataColumn("IsNew", typeof(bool)) { DefaultValue = false });
                foreach (DataRow item in tbl.Rows)
                {
                    DataRow newRow = tbl2.NewRow();
                    newRow["MaHang"] = ListPara[2];
                    newRow["TenHang"] = ListPara[3];
                    newRow["KhachHang"] = ListPara[0];
                    newRow["TenKH"] = ListPara[1];
                    newRow["MaMau"] = item["Sort"] ?? 0;
                    newRow["NhomSize"] = item["MaNhomSize"];
                    newRow["TenNhomSize"] = item["NhomSize"];
                    newRow["SizeID"] = item["MaSize"];
                    newRow["Size"] = item["TenSize"];
                    newRow["IsNew"] = false;
                    newRow["DateThucHien"] = item["Sort"] ?? 0;
                    newRow["NguoiDaiDien"] = "";
                    tbl2.Rows.Add(newRow);
                }
                grcTHSize.DataSource = tbl2;
            }
            catch (Exception ex)
            {

            }

        }
        private void MauCopy()
        {
            bool checkKH = cbxKH.EditValue.ToString() == "Đã khai báo";
            string mahang = searchLookUpEditMH1.EditValue.ToString();
            string makh = checkKH ? searchLookUpEditKH.EditValue?.ToString() ?? "All" : "All";
            string url = $"{URL}KhaiBaoAll/Get?action=GetColor&para1={mahang}&para2={makh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            string[] joinedTenMau = string.Join(":", tbl.AsEnumerable().Select(x => x["TenMau"].ToString()).Distinct()).Split(':');
            string[] joinedCode = string.Join(":", tbl.AsEnumerable().Select(x => x["CodeMau"].ToString()).Distinct()).Split(':');
            ParaMau(joinedTenMau, joinedCode);
        }
        private void ViewCopyMH()
        {
            if ((searchLookUpEditMH1.EditValue?.ToString() ?? "") == "") return;
            MauCopy();
            Size();
        }
        private void barEditItem2_EditValueChanged(object sender, EventArgs e)
        {

            if (Convert.ToBoolean(barEditItem2.EditValue))
            {
                if (cbxMH.EditValue.ToString() == "Đã khai báo")
                {
                    MessageBox.Show("Vui lòng chọn khai báo mới mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    barEditItem2.EditValue = false;
                    return;
                }
                NameMHA1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                NameSize.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameMau.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameNSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameNSA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                splitContainerControl1.SplitterPosition = 160;
                GetMaHangCopy();
            }
            else
            {
                NameMHA1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                NameSize.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                NameMau.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (cbxNS.EditValue == "Đã khai báo")
                    NameNSA.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                else
                    NameNSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                splitContainerControl1.SplitterPosition = 230;
            }
            grcThongSo.DataSource = null;
            grcTHSize.DataSource = null;
        }
        bool CheckGrc;
        private void grvThongSo_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            CheckGrc = true;
            try
            {
                DXMenuItem menuXoa = new DXMenuItem();
                menuXoa.Caption = "Xóa dòng";
                menuXoa.Click += MenuClickDelete;
                e.Menu.Items.Add(menuXoa);
            }
            catch (Exception ex)
            {
            }

        }

        private void grvThongSoSize_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            CheckGrc = false;
            try
            {
                DXMenuItem menuXoa = new DXMenuItem();
                menuXoa.Caption = "Xóa dòng";
                menuXoa.Click += MenuClickDelete;
                e.Menu.Items.Add(menuXoa);
            }
            catch (Exception ex)
            {
            }
        }
        private void MenuClickDelete(object sender, EventArgs e)
        {
            DataRow row = CheckGrc ? grvThongSo.GetFocusedDataRow() : grvThongSoSize.GetFocusedDataRow();
            if (row == null) return;
            DataTable dt = (CheckGrc ? grcThongSo.DataSource : grcTHSize.DataSource) as DataTable;
            dt.Rows.Remove(row);
            if (CheckGrc)
                grcThongSo.DataSource = dt;

            else
                grcTHSize.DataSource = dt;


        }

        private void grcTHSize_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (grvThongSoSize.FocusedColumn.FieldName == "Size")
            {
                if (e.KeyChar == '_')
                    e.Handled = true;
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
            if (grvThongSoSize.FocusedColumn.FieldName == "TenNhomSize")
            {
                if (/*IsVietnameseChar(e.KeyChar) ||*/ e.KeyChar == '_')
                {
                    e.Handled = true;
                }
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        bool checkTrung;
        private DXErrorProvider dxErrorProvider = new DXErrorProvider();
        private void txtMau_Properties_Validating(object sender, CancelEventArgs e)
        {
            CheckTrungTxt(txtMau.EditValue?.ToString() ?? "", "màu", "Màu", txtMau, e);
        }

        private void txtNhomSize_Properties_Validating(object sender, CancelEventArgs e)
        {
            CheckTrungTxt(txtNhomSize.EditValue?.ToString() ?? "", "nhóm size", "Nhóm size", txtNhomSize, e);

        }
        private void txtSize_Properties_Validating(object sender, CancelEventArgs e)
        {
            CheckTrungTxt(txtSize.EditValue?.ToString() ?? "", " size", "Size", txtSize, e);
        }
        private void CheckTrungTxt(string textName, string tenvalue, string tenhoavalue, TextEdit txt, CancelEventArgs e)
        {
            string text = textName;
            if (text == "") return;
            string[] textSplit = text.Split(':');
            var duplicatedSizes = textSplit.GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => new { Text = g.Key, Count = g.Count() });
            string joined = string.Join(",", duplicatedSizes.AsEnumerable().Select(x => x.Text.ToString()));
            if (joined.Length != 0)
            {
                dxErrorProvider.SetError(txt, $"Vui lòng không nhập {tenvalue} trùng nhau!\n{tenhoavalue} bị trùng: {joined}");
                e.Cancel = true;
            }
            else dxErrorProvider.SetError(txt, "");


        }

        private void grvThongSoSize_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            string valueTxt = e.Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(valueTxt)) return;

            DataTable tblTHS = grcTHSize.DataSource as DataTable;
            if (tblTHS == null) return;
            DataRow drow = grvThongSoSize.GetFocusedDataRow();
            if (grvThongSoSize.FocusedColumn.FieldName == "Size")
            {
                if ((drow["TenKH"]?.ToString() ?? "") == "")
                {
                    e.ErrorText = $"Vui lòng nhập  khách hàng hoặc chọn khách hàng";
                    e.Valid = false;
                }
                if ((drow["MaHang"]?.ToString() ?? "") == "")
                {
                    e.ErrorText = $"Vui lòng nhập  mã hàng hoặc chọn mã hàng";
                    e.Valid = false;
                }
                bool isExist = tblTHS.AsEnumerable().Where(row => row != drow).Any(row =>
                    row["TenKH"].ToString() == drow["TenKH"].ToString() &&
                    row["MaHang"].ToString() == drow["MaHang"].ToString() &&
                    row["TenNhomSize"].ToString() == drow["TenNhomSize"].ToString() &&
                    row["Size"].ToString() == valueTxt
                );

                if (isExist)
                {
                    e.ErrorText = $"Nhóm size {valueTxt} và size {valueTxt} đã tồn tại";
                    e.Valid = false;
                }
            }
            if (grvThongSoSize.FocusedColumn.FieldName == "MaMau")
            {
                int indexSize = GetMaxSortSize(drow["NhomSize"].ToString());
                if (Convert.ToInt32(valueTxt) < indexSize || Convert.ToInt32(drow["MaMau"]) < 0)
                {
                    e.ErrorText = $"Cột sắp xếp không được nhỏ hơn {indexSize}";
                    e.Valid = false;
                }
                bool isExist = tblTHS.AsEnumerable().Where(row => row != drow).Any(row =>
                   row["TenKH"].ToString() == drow["TenKH"].ToString() &&
                   row["MaHang"].ToString() == drow["MaHang"].ToString() &&
                   row["TenNhomSize"].ToString() == drow["TenNhomSize"].ToString() &&
                  Convert.ToInt32(row["MaMau"]) == Convert.ToInt32(valueTxt));
                if (isExist)
                {
                    e.ErrorText = $"Cột sắp xếp {valueTxt} theo nhóm size đã tồn tại";
                    e.Valid = false;
                }
            }
            if (grvThongSoSize.FocusedColumn.FieldName == "TenNhomSize")
            {
                if ((drow["TenKH"]?.ToString() ?? "") == "")
                {
                    e.ErrorText = $"Vui lòng nhập  khách hàng hoặc chọn khách hàng";
                    e.Valid = false;
                }
                if ((drow["MaHang"]?.ToString() ?? "") == "")
                {
                    e.ErrorText = $"Vui lòng nhập  mã hàng hoặc chọn mã hàng";
                    e.Valid = false;
                }
                bool isExist = tblTHS.AsEnumerable().Where(row => row != drow).Any(row =>
                    row["TenKH"].ToString() == drow["TenKH"].ToString() &&
                    row["MaHang"].ToString() == drow["MaHang"].ToString() &&
                    row["TenNhomSize"].ToString() == valueTxt &&
                    row["Size"].ToString() == drow["Size"].ToString()
                );
                if (isExist)
                {
                    e.ErrorText = $"Nhóm size {drow["TenNhomSize"].ToString()} và size {drow["Size"].ToString()} đã tồn tại";
                    e.Valid = false;
                }
            }
        }

        private void grvThongSo_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            string valueTxt = e.Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(valueTxt)) return;

            DataTable tblTHS = grcThongSo.DataSource as DataTable;
            if (tblTHS == null) return;
            DataRow drow = grvThongSo.GetFocusedDataRow();

            if (grvThongSo.FocusedColumn.FieldName == "TenMau")
            {
                if ((drow["TenKH"]?.ToString() ?? "") == "")
                {
                    e.ErrorText = $"Vui lòng nhập  khách hàng hoặc chọn khách hàng";
                    e.Valid = false;
                }
                if ((drow["MaHang"]?.ToString() ?? "") == "")
                {
                    e.ErrorText = $"Vui lòng nhập  mã hàng hoặc chọn mã hàng";
                    e.Valid = false;
                }
                bool isExist = tblTHS.AsEnumerable().Where(row => row != drow).Any(row =>
                    row["TenKH"].ToString() == drow["TenKH"].ToString() &&
                    row["MaHang"].ToString() == drow["MaHang"].ToString() &&
                    row["TenMau"].ToString() == valueTxt //&&
                                                         // row["NguoiDaiDien"].ToString() == drow["NguoiDaiDien"].ToString()
                );
                if (isExist)
                {
                    e.ErrorText = $"Tên màu { valueTxt}  đã tồn tại";
                    e.Valid = false;
                }
            }
        }


        private void grvThongSoSize_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            ClickItemNewRow(e, grcTHSize);
        }
        private void ClickItemNewRow(DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e, GridControl grc, bool checkView = true)
        {
            if (e.FocusedRowHandle == GridControl.NewItemRowHandle)
            {
                List<string> ListPara = ListTS();

                if (ListPara[1] == "")
                {
                    MessageBox.Show("Vui lòng chọn khách hàng hoặc nhập khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataTable tbl = grc.DataSource as DataTable;

                if (tbl == null) return;
                if (tbl.Rows.Count > 0)
                {
                    DataRow newRow = tbl.NewRow();
                    DataRow drLast = tbl.Rows[tbl.Rows.Count - 1] as DataRow;
                    int sttSort = 0;
                    if (checkView)
                        sttSort = Convert.ToInt32(drLast["MaMau"]?.ToString() ?? "0") + 1;
                    int sttSortM = Convert.ToInt32((drLast["DateThucHien"]?.ToString() ?? "") ?? "0") + 1;
                    newRow["MaHang"] = ListPara[2]?.ToString() ?? "";
                    newRow["TenHang"] = ListPara[3]?.ToString() ?? "";
                    newRow["KhachHang"] = ListPara[0]?.ToString() ?? "";
                    newRow["TenKH"] = ListPara[1]?.ToString() ?? "";
                    newRow["IsNew"] = true;
                    newRow["DateThucHien"] = sttSortM;
                    if (checkView)
                        newRow["MaMau"] = sttSort;
                    tbl.Rows.Add(newRow);
                    grc.DataSource = tbl;
                }
            }
        }

        private void grvThongSo_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            ClickItemNewRow(e, grcThongSo, false);
        }

        private void grvThongSo_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            //if(gr)
            try
            {
                if (e.RowHandle < 0) return;
                var IsThungLe = (bool)grvThongSo.GetRowCellValue(e.RowHandle, "IsNew");
                if (IsThungLe) e.Appearance.BackColor = Color.Aqua;
            }
            catch (Exception ex)
            {

            }
        }

        private void grvThongSoSize_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var IsThungLe = (bool)grvThongSoSize.GetRowCellValue(e.RowHandle, "IsNew");
                if (IsThungLe) e.Appearance.BackColor = Color.Aqua;
            }
            catch (Exception ex)
            {

            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditMH == null && cbxMH.EditValue.ToString() == "Đã khai báo" && txtMH.EditValue == null || (txtMH.EditValue?.ToString() ?? "") == "" && cbxMH.EditValue.ToString() != "Đã khai báo")
            {
                MessageBox.Show("Vui lòng chọn mã hàng hoặc nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (searchLookUpEditMauKB.IsPopupOpen)
                searchLookUpEditMauKB.ClosePopup();
            else
                searchLookUpEditMauKB.ShowPopup();
        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {

            if (searchLookUpEdit1.IsPopupOpen)
                searchLookUpEdit1.ClosePopup();
            else
                searchLookUpEdit1.ShowPopup();
        }
        private void simpleButton5_Click(object sender, EventArgs e)
        {

            if (searchLookUpEdit2.IsPopupOpen)
                searchLookUpEdit2.ClosePopup();
            else
                searchLookUpEdit2.ShowPopup();
        }
        private void simpleButton6_Click(object sender, EventArgs e)
        {

            if (searchLookUpEdit3.IsPopupOpen)
                searchLookUpEdit3.ClosePopup();
            else
                searchLookUpEdit3.ShowPopup();
        }
        private void simpleButton7_Click(object sender, EventArgs e)
        {

            if (searchLookUpEdit4.IsPopupOpen)
                searchLookUpEdit4.ClosePopup();
            else
                searchLookUpEdit4.ShowPopup();
        }
        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            DataRow drRow = gridView1.GetFocusedDataRow();
            int rowHandle = gridView1.FocusedRowHandle;
            if (drRow == null) return;
            string tenMau = drRow["TenMau"].ToString();

            if (e.Action == CollectionChangeAction.Add)
            {
                bool checkTrungNext = checkTrungMau(tenMau);
                if (checkTrungNext)
                {
                    gridView1.SelectionChanged -= gridView1_SelectionChanged;
                    gridView1.UnselectRow(rowHandle);
                    gridView1.SelectionChanged += gridView1_SelectionChanged;
                }
                else
                {
                    AddTextMau(tenMau);
                    string[] tenMauArry = tenMau.Split(':');
                    DataCodeMau(tenMauArry, drRow["CodeMau"].ToString());
                }
            }
            if (e.Action == CollectionChangeAction.Remove)
            {
                RemoveTextMau(tenMau);
                string[] tenMauArr = tenMau.Split(':');
                RemoveRowCodeMau(tenMauArr);
            }
        }
        private void RemoveTextMau(string tenMau)
        {
            string textMau = txtMau.EditValue?.ToString() ?? "";
            txtMau.EditValue = RemoveText(textMau, tenMau);
        }
        static string RemoveText(string input, string removeItem)
        {
            string removeWithColon = $":{removeItem}";
            string removeWithoutColon = $"{removeItem}:";

            if (input.Contains(removeWithColon))
                return input.Replace(removeWithColon, "");
            else if (input.Contains(removeWithoutColon))
                return input.Replace(removeWithoutColon, "");
            else
                return input.Replace(removeItem, "");
        }
        private void AddTextMau(string tenMau)
        {
            string textMau = txtMau.EditValue?.ToString() ?? "";
            bool isLastCharColon = textMau.EndsWith(":");
            txtMau.EditValue = textMau == "" ? $"{textMau}{tenMau}" : isLastCharColon ? $"{textMau}{tenMau}" : $"{textMau}:{tenMau}";
        }
        private bool checkTrungMau(string tenMau)
        {
            string[] mauPart = txtMau.EditValue?.ToString().Split(':').Where(s => !string.IsNullOrEmpty(s)).ToArray() ?? new string[] { };
            bool checkTrungNext = mauPart.Any(part => part.Trim().Equals(tenMau.Trim(), StringComparison.OrdinalIgnoreCase));
            if (checkTrungNext)
            {
                MessageBox.Show("Màu này đã có trong ô nhập.Vui lòng chọn màu khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return checkTrungNext;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditMH == null && cbxMH.EditValue.ToString() == "Đã khai báo" && txtMH.EditValue == null || (txtMH.EditValue?.ToString() ?? "") == "" && cbxMH.EditValue.ToString() != "Đã khai báo")
            {
                MessageBox.Show("Vui lòng chọn mã hàng hoặc nhập mã hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtMau.EditValue == null || txtMau.EditValue.ToString() == "")
            {
                MessageBox.Show("Vui lòng nhập màu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable tbl = gridcodeMau.DataSource as DataTable;
            string[] joinedTenMau = new string[] { };
            if (tbl != null && tbl.Rows.Count != 0)
                joinedTenMau = string.Join(":", tbl.AsEnumerable().Select(x => x["TenMau"].ToString()).Distinct()).Split(':');

            string[] listMau = txtMau.EditValue?.ToString().Split(':').Where(s => !string.IsNullOrEmpty(s)).ToArray() ?? new string[] { };
            if (listMau.Length == joinedTenMau.Length) return;
            if (listMau.Length > joinedTenMau.Length)
            {
                string listMauPart = RemoveDuplicates(listMau, joinedTenMau);
                if (listMauPart == "") return;
                DataCodeMau(listMauPart.Split(':'), "");
            }
            else
            {
                string listMauPart = RemoveDuplicates(joinedTenMau, listMau);
                if (listMauPart == "") return;
                RemoveRowCodeMau(listMauPart.Split(':'));
            }
        }
        static string RemoveDuplicates(string[] input1, string[] input2)
        {
            var filtered = input1.Except(input2);
            return string.Join(":", filtered);
        }

        private void RemoveRowCodeMau(string[] listMau)
        {
            DataTable tbl2 = gridcodeMau.DataSource as DataTable;
            var rowsToDelete = tbl2.AsEnumerable()
                           .Where(row => listMau.Contains(row["TenMau"].ToString()))
                           .ToList();

            foreach (var row in rowsToDelete)
            {
                tbl2.Rows.Remove(row);
            }
        }
        private void DataCodeMau(string[] listMau, string CodeMau)
        {
            tblCodeSize.Clear();

            bool checkMH = cbxMH.EditValue.ToString() == "Đã khai báo";
            string MaHang = checkMH ? searchLookUpEditMH.EditValue.ToString()
                                       : ReplaceSpecialCharacters(RemoveVietnameseTone(txtMH.EditValue.ToString())).ToString().ToUpper();

            string TenHang = checkMH ? searchLookUpEditMH.Text.ToString() : txtMH.EditValue.ToString();


            DataTable tbl = tblCodeSize.Clone();
            DataTable tbl2 = gridcodeMau.DataSource as DataTable;
            if (tbl2 != null && tbl2.Rows.Count != 0)
            {
                tbl = tbl2.Copy();
            }
            foreach (var _mau in listMau)
            {
                string MaMau = ReplaceSpecialCharacters(RemoveVietnameseTone(_mau.ToString())).ToString().ToUpper();
                string TenMau = _mau.ToString();
                DataRow newRow = tbl.NewRow();
                newRow["MaHang"] = MaHang;
                newRow["TenHang"] = TenHang;
                newRow["MaMau"] = MaMau;
                newRow["TenMau"] = TenMau;
                newRow["NguoiDaiDien"] = CodeMau;
                tbl.Rows.Add(newRow);

            }
            gridcodeMau.DataSource = tbl;
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            string tenKH = searchLookUpEdit1.Text.ToString();
           
            txtKH.EditValue = tenKH;
            if (tenKH == "") return;
            DataRow row = gridView2.GetFocusedDataRow();
            if (row == null) return;
            txtVietTatKH.EditValue = row["VietTat"].ToString();
        }

        private void searchLookUpEdit3View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            DataRow drRow = searchLookUpEdit3View.GetFocusedDataRow();
            int rowHandle = searchLookUpEdit3View.FocusedRowHandle;
            if (drRow == null) return;
            string tenNhom = drRow["NhomSize"].ToString();

            if (e.Action == CollectionChangeAction.Add)
                AddTextNS(tenNhom);
                
            if (e.Action == CollectionChangeAction.Remove)
                RemoveTextNS(tenNhom);
        }
        private void RemoveTextNS(string tenNhom)
        {
            string textNS = txtNhomSize.EditValue?.ToString() ?? "";
            txtNhomSize.EditValue = RemoveText(textNS, tenNhom);
        }
        private void AddTextNS(string tenNhom)
        {
            string textNhomSize = txtNhomSize.EditValue?.ToString() ?? "";
            bool isLastCharColon = textNhomSize.EndsWith(":");
            txtNhomSize.EditValue = textNhomSize == "" ? $"{textNhomSize}{tenNhom}" : isLastCharColon ? $"{textNhomSize}{tenNhom}" : $"{textNhomSize}:{tenNhom}";
        }

        private void searchLookUpEdit4View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            DataRow drRow = searchLookUpEdit4View.GetFocusedDataRow();
            int rowHandle = searchLookUpEdit4View.FocusedRowHandle;
            if (drRow == null) return;
            string tenSize = drRow["TenSize"].ToString();

            if (e.Action == CollectionChangeAction.Add)
                AddTextTS(tenSize);

            if (e.Action == CollectionChangeAction.Remove)
            {
               string tenSizeRemove= CheckMulSelect();
                txtSize.EditValue = tenSizeRemove;
            }  
        }
        private string CheckMulSelect()
        {

            int[] selectedHandles = searchLookUpEdit4View.GetSelectedRows();
            var tenSizes = new List<string>();
            foreach (int rowHandle in selectedHandles)
            {
                DataRow row = searchLookUpEdit4View.GetDataRow(rowHandle);
                if (row != null)
                {
                    string tenSize = row["TenSize"].ToString();
                    tenSizes.Add(row["TenSize"].ToString());
                }
            }
            string tensizejoin = string.Join(":", tenSizes);
            return tensizejoin;
        }
        private void AddTextTS(string tenSize)
        {
            string textSize = txtSize.EditValue?.ToString() ?? "";
            bool isLastCharColon = textSize.EndsWith(":");
            txtSize.EditValue = textSize == "" ? $"{textSize}{tenSize}" : isLastCharColon ? $"{textSize}{tenSize}" : $"{textSize}:{tenSize}";
        }
        private void RemoveTextTS(string tenSize)
        {
            string textTS = txtSize.EditValue?.ToString() ?? "";
            txtSize.EditValue = RemoveText(textTS, tenSize);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetValue();
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
           if (IsVietnameseChar(e.KeyChar) || e.KeyChar == '_')
            {
                e.Handled = true;
            }
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void searchLookUpEdit2_EditValueChanged_1(object sender, EventArgs e)
        {
            string tenHang = searchLookUpEdit2.Text.ToString();
            txtMH.EditValue = tenHang;
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

    }
}