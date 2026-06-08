using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmHangHoaChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _themau = string.Empty, _theInSeam = string.Empty,
           _theSize = string.Empty, selectValueTS = string.Empty, _mahang = string.Empty, _maKH = string.Empty, _maCL = string.Empty
            , _loaiHH = string.Empty, _ghiChu = string.Empty, _tenHang = string.Empty;
        bool _checkEdit, intheu, intheuCT, hutam, dokim;
        int maxSortSize = 0, trangthai = 0, maxSortSIs = 0;
        public string selectMaHang { get; private set; }
        private HttpClientExtension _clientExtension;
        List<DataRow> selectedRowsTM;
        List<DataRow> selectedRowsM;
        List<DataRow> selectedRowsTIS;
        List<DataRow> selectedRowsTS;
        public frmHangHoaChiTiet(string mahang = "", string tenhang = "", string makhachhang = "", string machungloai = "", string loaiHH = "", string ghichu = "", bool checkEdit = false, int trangthai = 0, bool intheu = false, bool intheuCT = false, bool hutAm = false, bool dokim = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            selectedRowsTM = new List<DataRow>();
            selectedRowsM = new List<DataRow>();
            selectedRowsTIS = new List<DataRow>();
            selectedRowsTS = new List<DataRow>();
            this._mahang = mahang;
            this._tenHang = tenhang;
            this._maKH = makhachhang;
            this._maCL = machungloai;
            this._loaiHH = loaiHH;
            this._ghiChu = ghichu;
            this._checkEdit = checkEdit;
            this.trangthai = trangthai;
            this.intheu = intheu;
            this.intheuCT = intheuCT;
            this.hutam = hutAm;
            this.dokim = dokim;
            grvMau.CustomUnboundColumnData += GrvMau_CustomUnboundColumnData;



            if (!string.IsNullOrEmpty(_tenHang))
                txt_MaHang.Properties.ReadOnly = true;


            if (checkEdit == true)
                GetMauMH();
            if (trangthai == 1)
                txt_MaHang.Properties.ReadOnly = false;
            txt_MaHang.EditValue = _tenHang.ToString();
            txtGhiChu.EditValue = _ghiChu.ToString();
            checkBox1.Checked = intheu;
            checkBox2.Checked = intheuCT;
            checkBox3.Checked = hutam;
            checkBox4.Checked = dokim;
            InIt();
        }

        #region Int
        private void InIt()
        {
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            searchLookUpEditKH.Properties.NullValuePrompt = "Chọn khách hàng";

            searchLookUpEditCL.Properties.ValueMember = "MaCL";
            searchLookUpEditCL.Properties.DisplayMember = "TenCL";
            searchLookUpEditCL.Properties.NullValuePrompt = "Chọn chủng loại";

            searchLookUpEditLH.Properties.ValueMember = "MaLHH";
            searchLookUpEditLH.Properties.DisplayMember = "TenLHH";
            searchLookUpEditLH.Properties.NullValuePrompt = "Chọn loại hàng";

            searchLookUpEditTM.Properties.ValueMember = "MaTheMau";
            searchLookUpEditTM.Properties.DisplayMember = "TheMau";

            searchLookUpEditM.Properties.ValueMember = "MaMau";
            searchLookUpEditM.Properties.DisplayMember = "TenMau";



            searchLookUpEditTIS.Properties.ValueMember = "TheInseam";
            searchLookUpEditTIS.Properties.DisplayMember = "Inseam";

            searchLookUpEditIS.Properties.ValueMember = "MaInSeam";
            searchLookUpEditIS.Properties.DisplayMember = "InSeam";


            searchLookUpEditTS.Properties.ValueMember = "MaTheSize";
            searchLookUpEditTS.Properties.DisplayMember = "TheSize";

            searchLookUpEditS.Properties.ValueMember = "MaSize";
            searchLookUpEditS.Properties.DisplayMember = "TenSize";

            GetKhachHang();
            GetChungLoai();
            GetLoaiHang();
            //GetTheMau();
            //GetTheInSeam();
            //GetTheSize();
            GetSize("");
            GetMau("");
            GetInSeam("");
        }
        private void GetKhachHang()
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=GetKhachHang";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditKH.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditKH.Properties.DataSource = tbl;
                if (!string.IsNullOrEmpty(_maKH))
                {
                    bool exists = tbl.AsEnumerable().Any(r => r["MaKH"].ToString() == _maKH);
                    if (exists)
                    {
                        searchLookUpEditKH.EditValue = _maKH;
                        searchLookUpEditKH.Properties.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void GetChungLoai()
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=GetChungLoai";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditCL.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditCL.Properties.DataSource = tbl;
                if (!string.IsNullOrEmpty(_maCL))
                {
                    bool exists = tbl.AsEnumerable().Any(r => r["MaCL"].ToString().ToUpper() == _maCL.ToString().ToUpper());
                    if (exists)
                    {
                        searchLookUpEditCL.EditValue = _maCL;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void GetLoaiHang()
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=GetLoaiHH";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditLH.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditLH.Properties.DataSource = tbl;
                if (!string.IsNullOrEmpty(_loaiHH))
                {
                    bool exists = tbl.AsEnumerable().Any(r => r["MaLHH"].ToString() == _loaiHH.ToString());
                    if (exists)
                    {
                        searchLookUpEditLH.EditValue = _loaiHH;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void GetTheMau()
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=GetTheMau";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditTM.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditTM.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetMau(string mamau)
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=GetChiTietTheMau&para={mamau}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditM.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditM.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }



        private void GetTheInSeam()
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=ERPTheInSeam";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditTIS.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditTIS.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }



        private void GetInSeam(string inSeam)
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=ERPInSeam&para={inSeam}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditIS.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditIS.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetTheSize()
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=GetTheSize";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditTS.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditTS.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetSize(string size)
        {
            try
            {
                string url = $"{URL}ERPHangHoa/Get?action=GetChiTietTheSize&para={size}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditS.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditS.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetMauMH()
        {
            try
            {

                string mahang = _mahang;
                string khachHang = _maKH;
                string url = $"{URL}ERPHangHoa/Get?action=GetMauMH&para={mahang}&para2={khachHang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcMau.DataSource = null;
                    return;
                }
                grcMau.DataSource = tbl;
                GetISMH();
                GetSizeMH();
            }
            catch (Exception ex)
            {

            }

        }
        private void GetISMH()
        {
            try
            {

                string mahang = _mahang;
                string khachHang = _maKH;
                string url = $"{URL}ERPHangHoa/Get?action=GetInSeamMH&para={mahang}&para2={khachHang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcInSeam.DataSource = null;
                    return;
                }
                grcInSeam.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetSizeMH()
        {
            try
            {

                string mahang = _mahang;
                string khachHang = _maKH;
                string url = $"{URL}ERPHangHoa/Get?action=GetSizeMH&para={mahang}&para2={khachHang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcSize.DataSource = null;
                    return;
                }
                maxSortSize = tbl.AsEnumerable()
                 .Max(r => Convert.ToInt32(r["Sort"]));

                grcSize.DataSource = tbl;


            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Event
        private void searchLookUpEdit3View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            string selectedValues = string.Join(";", searchLookUpEdit3View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit3View.GetRowCellValue(rowHandle2, searchLookUpEditTM.Properties.ValueMember)));
            searchLookUpEditTM.EditValue = selectedValues;
            if (searchLookUpEditTM.EditValue is null) return;
            _themau = searchLookUpEditTM.EditValue.ToString();
            GetMau(_themau);

        }
        string selectValueTMau = "";
        string selectValueTIS = "";
        private void searchLookUpEditTM_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = searchLookUpEdit3View.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => searchLookUpEdit3View.GetRowCellValue(rowHandle, searchLookUpEditTM.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            selectValueTMau = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn thẻ màu";
            e.DisplayText = selectValueTMau;
        }



        private void searchLookUpEdit5View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            string selectedValues = string.Join(";", searchLookUpEdit5View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit5View.GetRowCellValue(rowHandle2, searchLookUpEditTIS.Properties.ValueMember)));
            searchLookUpEditTIS.EditValue = selectedValues;
            if (searchLookUpEditTIS.EditValue is null) return;
            _theInSeam = searchLookUpEditTIS.EditValue.ToString();
            GetInSeam(_theInSeam);

        }

        private void searchLookUpEditTIS_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

            var selectedRows = searchLookUpEdit5View.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => searchLookUpEdit5View.GetRowCellValue(rowHandle, searchLookUpEditTIS.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            selectValueTIS = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn thẻ inseam";
            e.DisplayText = selectValueTIS;
        }
        private void searchLookUpEdit7View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            string selectedValues = string.Join(";", searchLookUpEdit7View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit7View.GetRowCellValue(rowHandle2, searchLookUpEditTS.Properties.ValueMember)));
            searchLookUpEditTS.EditValue = selectedValues;
            if (searchLookUpEditTS.EditValue is null) return;
            _theSize = searchLookUpEditTS.EditValue.ToString();
            GetSize(_theSize);
        }
        private void searchLookUpEditTS_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = searchLookUpEdit7View.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => searchLookUpEdit7View.GetRowCellValue(rowHandle, searchLookUpEditTS.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            selectValueTS = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn thẻ size";
            e.DisplayText = selectValueTS;

        }
        bool isInternalSelectionChange = false;
        private void searchLookUpEdit4View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (isInternalSelectionChange) return;
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);
                string maMau = row["MaMau"].ToString();
                string maTheMau = row["MaTheMau"].ToString();
                DataTable dtMau = grcMau.DataSource as DataTable;
                if (dtMau == null || dtMau.Rows.Count == 0)
                {
                    AddGridMau(row, true);
                    return;
                };
                if (view.IsRowSelected(rowHandle3))
                {
                    //bool isExistThe = dtMau.AsEnumerable()
                    //       .Any(r => r["MaTheMau"].ToString() == maTheMau);
                    //if (!isExistThe)
                    //{
                    //    XtraMessageBox.Show("Vui lòng chọn 1 thẻ màu cho 1 hàng hóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    isInternalSelectionChange = true;
                    //    view.UnselectRow(rowHandle3);
                    //    isInternalSelectionChange = false;
                    //    return;
                    //}
                    if (_checkEdit)
                    {
                        bool isExist = dtMau.AsEnumerable()
                            .Any(r => r["MaMau"].ToString() == maMau && r["MaTheMau"].ToString() == maTheMau);

                        if (isExist)
                        {
                            XtraMessageBox.Show("Màu này đã tồn tại trong bảng Màu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            isInternalSelectionChange = true;
                            view.UnselectRow(rowHandle3);
                            isInternalSelectionChange = false;
                            return;
                        }

                    }
                    AddGridMau(row, true);
                }
                else
                {
                    AddGridMau(row, false);
                }
            }
            string selectedValues = string.Join(";", searchLookUpEdit4View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit4View.GetRowCellValue(rowHandle2, searchLookUpEditM.Properties.ValueMember)));
            searchLookUpEditM.EditValue = selectedValues;
            if (searchLookUpEditM.EditValue is null) return;
        }
        string selectValueM = "";
        private void searchLookUpEditM_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = searchLookUpEdit4View.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => searchLookUpEdit4View.GetRowCellValue(rowHandle, searchLookUpEditM.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            selectValueM = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn màu";
            e.DisplayText = selectValueM;
        }
        private void searchLookUpEdit6View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (isInternalSelectionChange) return;
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);
                string maInSeam = row["MaInSeam"].ToString();
                string maTheInSeam = row["MaTheInSeam"].ToString();
                DataTable dtInSeam = grcInSeam.DataSource as DataTable;
                if (dtInSeam == null || dtInSeam.Rows.Count == 0)
                {
                    AddGridInSeam(row, true);
                    return;
                };

                if (view.IsRowSelected(rowHandle3))
                {
                    //bool isExistThe = dtInSeam.AsEnumerable()
                    //        .Any(r => r["MaTheInSeam"].ToString() == maTheInSeam);
                    //if (!isExistThe)
                    //{
                    //    XtraMessageBox.Show("Vui lòng chọn 1 thẻ InSeam cho 1 hàng hóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    isInternalSelectionChange = true;
                    //    view.UnselectRow(rowHandle3);
                    //    isInternalSelectionChange = false;
                    //    return;
                    //}
                    if (_checkEdit)
                    {
                        bool isExist = dtInSeam.AsEnumerable()
                            .Any(r => r["MaInSeam"].ToString() == maInSeam && r["MaTheInSeam"].ToString() == maTheInSeam);

                        if (isExist)
                        {
                            XtraMessageBox.Show("InSeam này đã tồn tại trong bảng InSeam.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            isInternalSelectionChange = true;
                            view.UnselectRow(rowHandle3);
                            isInternalSelectionChange = false;
                            return;
                        }
                    }
                    AddGridInSeam(row, true);

                }
                else
                {
                    AddGridInSeam(row, false);
                }
            }
            string selectedValues = string.Join(";", searchLookUpEdit6View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit6View.GetRowCellValue(rowHandle2, searchLookUpEditIS.Properties.ValueMember)));
            searchLookUpEditIS.EditValue = selectedValues;
            if (searchLookUpEditIS.EditValue is null) return;
        }

        string selectValueIS = "";

        private void searchLookUpEditIS_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = searchLookUpEdit6View.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => searchLookUpEdit6View.GetRowCellValue(rowHandle, searchLookUpEditIS.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            selectValueIS = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn nhóm size";
            e.DisplayText = selectValueIS;
        }

        private void searchLookUpEdit8View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (isInternalSelectionChange) return;
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);
                string maSize = row["MaSize"].ToString();
                string maTheSize = row["MaTheSize"].ToString();
                DataTable dtSize = grcSize.DataSource as DataTable;
                if (dtSize == null || dtSize.Rows.Count == 0)
                {
                    AddGridSize(row, true);
                    return;
                };
                if (view.IsRowSelected(rowHandle3))
                {
                    //bool isExistThe = dtSize.AsEnumerable()
                    //       .Any(r => r["MaTheSize"].ToString() == maTheSize);
                    //if (!isExistThe)
                    //{
                    //    XtraMessageBox.Show("Vui lòng chọn 1 thẻ size cho 1 hàng hóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    isInternalSelectionChange = true;
                    //    view.UnselectRow(rowHandle3);
                    //    isInternalSelectionChange = false;
                    //    return;
                    //}
                    if (_checkEdit)
                    {
                        bool isExist = dtSize.AsEnumerable()
                            .Any(r => r["MaSize"].ToString() == maSize && r["MaTheSize"].ToString() == maTheSize);

                        if (isExist)
                        {
                            XtraMessageBox.Show("Size này đã tồn tại trong bảng Size.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            isInternalSelectionChange = true;
                            view.UnselectRow(rowHandle3);
                            isInternalSelectionChange = false;
                            return;
                        }

                    }
                    AddGridSize(row, true);
                }
                else
                {
                    AddGridSize(row, false);
                }
            }
            string selectedValues = string.Join(";", searchLookUpEdit8View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit8View.GetRowCellValue(rowHandle2, searchLookUpEditS.Properties.ValueMember)));
            searchLookUpEditS.EditValue = selectedValues;
            if (searchLookUpEditS.EditValue is null) return;
        }
        string selectValueS = "";
        private void searchLookUpEditS_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = searchLookUpEdit8View.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => searchLookUpEdit8View.GetRowCellValue(rowHandle, searchLookUpEditS.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            selectValueS = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn size";
            e.DisplayText = selectValueS;
        }
        private void btn_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            Save();
        }

        private void grvMau_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                DataRow row = grvMau.GetFocusedDataRow();
                if (row == null) return;
                if (e.Menu == null || !_checkEdit || Convert.ToInt32(row["ID"]) == -1) return;
                e.Menu.Items.Clear();

                DXMenuItem DeleteMau = new DXMenuItem();
                DeleteMau.Caption = "Xóa";
                DeleteMau.Click += Delete;
                e.Menu.Items.Add(DeleteMau);
            }
            catch (Exception ex)
            {

            }
        }
        private void grvInSeam_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                //DataRow row = grvInSeam.GetFocusedDataRow();
                //if (row == null) return;
                //if (e.Menu == null || !_checkEdit || Convert.ToInt32(row["ID"]) == -1) return;
                //e.Menu.Items.Clear();

                //DXMenuItem DeleteMau = new DXMenuItem();
                //DeleteMau.Caption = "Xóa";
                //DeleteMau.Click += DeleteInItem;
                //e.Menu.Items.Add(DeleteMau);
            }
            catch (Exception ex)
            {


            }
        }
        private void grvSize_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                //DataRow row = grvSize.GetFocusedDataRow();
                //if (row == null) return;
                //if (e.Menu == null || !_checkEdit || Convert.ToInt32(row["ID"]) == -1) return;
                //e.Menu.Items.Clear();

                //DXMenuItem DeleteMau = new DXMenuItem();
                //DeleteMau.Caption = "Xóa";
                //DeleteMau.Click += DeleteS;
                //e.Menu.Items.Add(DeleteMau);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
        private void DeleteS(object sender, EventArgs e)
        {
            //this.ActiveControl = button2;
            //DataRow row = grvSize.GetFocusedDataRow();
            //if (row == null) return;

            //string mahang = _mahang;
            //string khachHang = _maKH;
            //string maSize = row["MaSize"].ToString();
            //string maTheSize = row["MaTheSize"].ToString();
            //string url = string.Format("{0}", URL + $"ERPHangHoa/Delete?action=DeleteSize&id={mahang}&para2={khachHang}&para3={maSize}&para4={maTheSize}");
            //var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            //if (msResult.ToUpper() == "TRUE")
            //{
            //    clsWaitForm.ShowSuccessForm(this, 2000);
            //    GetSizeMH();
            //}
            DataTable dtXoaSize = this.grcSize.DataSource as DataTable;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaHang", typeof(string));
            dtSave.Columns.Add("MaKH", typeof(string));
    
            dtSave.Columns.Add("MaTheSize", typeof(string));
            dtSave.Columns.Add("MaSize", typeof(string));
            int[] selectedRows = grvSize.GetSelectedRows();

            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle >= 0)
                {
                    DataRow dr = grvSize.GetDataRow(rowHandle);
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaKH"] = _maKH;
                    _dr["MaHang"] = _mahang;
                    _dr["MaTheSize"] = dr["MaTheSize"];
                    _dr["MaSize"] = dr["MaSize"];
                    dtSave.Rows.Add(_dr);
                }
            }
            //foreach (DataRow dr in dtXoaSize.Rows)
            //{
            //    if ((bool)dr["Chon"] == true && dr["Chon"] != "")
            //    {
            //        DataRow _dr = dtSave.NewRow();
            //        _dr["MaKH"] = _maKH;
            //        _dr["MaHang"] = _mahang;
            //        _dr["MaTheSize"] = dr["MaTheSize"];
            //        _dr["MaSize"] = dr["MaSize"];
            //        dtSave.Rows.Add(_dr);
            //    }
            //}
            if (dtSave.Rows.Count > 0 && dtSave != null)
            {
                string urldvsx = string.Format("{0}", URL + $"ERPHangHoa/GetKiemTra?action=KIEMTRA&type=@TypeHangHoa");
                string jsondvsx = Task.Run(async () => { return await _clientExtension.PostAsync(urldvsx, dtSave); }).Result;
                if (jsondvsx == "[]")
                {
                    string urled = string.Format("{0}", URL + $"ERPHangHoa/DeleteSize?action=DELETE&type=@TypeHangHoa");
                    var msResulted = Task.Run(async () => { return await _clientExtension.PostAsync(urled, dtSave); }).Result;
                    if (msResulted.ToUpper() == "TRUE")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        GetSizeMH();
                    }
                }
                else
                {
                    DataTable tbldvsx = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
                    var sizes = tbldvsx.Rows.Cast<DataRow>().Select(r => r["Size"]?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

                    string _size = string.Join(";", sizes);
                    XtraMessageBox.Show("Size "+ _size  + " đã được sử dụng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }    
               
            }
        }
        private void DeleteInItem(object sender, EventArgs e)
        {
            DataRow row = grvInSeam.GetFocusedDataRow();
            if (row == null) return;

            //string mahang = _mahang;
            //string khachHang = _maKH;
            //string maInSeam = row["MaInSeam"].ToString();
            //string maTheInSeam = row["MaTheInSeam"].ToString();
            //string url = string.Format("{0}", URL + $"ERPHangHoa/Delete?action=DeleteIS&id={mahang}&para2={khachHang}&para3={maInSeam}&para4={maTheInSeam}");
            //var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            //if (msResult.ToUpper() == "TRUE")
            //{
            //    clsWaitForm.ShowSuccessForm(this, 2000);
            //    GetISMH();
            //}
            DataTable dtXoaSize = this.grvInSeam.DataSource as DataTable;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaHang", typeof(string));
            dtSave.Columns.Add("MaKH", typeof(string));

            dtSave.Columns.Add("MaTheSize", typeof(string));
            dtSave.Columns.Add("MaSize", typeof(string));
            int[] selectedRows = grvInSeam.GetSelectedRows();

            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle >= 0)
                {
                    DataRow dr = grvInSeam.GetDataRow(rowHandle);
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaKH"] = _maKH;
                    _dr["MaHang"] = _mahang;
                    _dr["MaTheSize"] = dr["MaTheInSeam"];
                    _dr["MaSize"] = dr["MaInSeam"];
                    dtSave.Rows.Add(_dr);
                }
            }
            if (dtSave.Rows.Count > 0 && dtSave != null)
            {
                string urldvsx = string.Format("{0}", URL + $"ERPHangHoa/GetKiemTra?action=KIEMTRAIS&type=@TypeHangHoa");
                string jsondvsx = Task.Run(async () => { return await _clientExtension.PostAsync(urldvsx, dtSave); }).Result;
                if (jsondvsx == "[]")
                {
                    string urled = string.Format("{0}", URL + $"ERPHangHoa/DeleteSize?action=DELETEIS&type=@TypeHangHoa");
                    var msResulted = Task.Run(async () => { return await _clientExtension.PostAsync(urled, dtSave); }).Result;
                    if (msResulted.ToUpper() == "TRUE")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        GetISMH();
                        GetSizeMH(); 
                    }
                }
                else
                {
                    DataTable tbldvsx = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
                    var inseam = tbldvsx.Rows.Cast<DataRow>().Select(r => r["DauSize"]?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

                    string _inseam = string.Join(";", inseam);
                    XtraMessageBox.Show("Inseam " + _inseam + " đã được sử dụng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
        }

        private void Delete(object sender, EventArgs e)
        {
            DataRow row = grvMau.GetFocusedDataRow();
            if (row == null) return;
            if (_checkEdit)
            {
                string maHang = row["MaHang"].ToString();
                string maKH = row["MaKH"].ToString();
                string maMau = row["MaMau"].ToString();

                string urlCheck = $"{URL}ERPHangHoa/Get?action=CheckMauUsed&para={maHang}&para2={maKH}&para3={maMau}";
                string jsonCheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlCheck)).Result;

                DataTable dtCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                if (dtCheck != null && dtCheck.Rows.Count > 0)
                {
                    XtraMessageBox.Show("Màu đã được sử dụng, không thể xóa!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string IDDelete = row["ID"].ToString();
                string url = string.Format("{0}", URL + $"ERPHangHoa/Delete?action=DeleteMau&id={IDDelete}");
                var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    GetMauMH();
                }
            }
            else
            {
                // Xóa dòng có MaMau trùng
                var dtSource = grcMau.DataSource as DataTable;
                dtSource = dtSource.AsEnumerable().Where(x => x["MaMau"].ToString() != row["MaMau"].ToString() && x["MaTheMau"].ToString() != row["MaTheMau"].ToString()).CopyToDataTable();
                grcMau.DataSource = dtSource;
            }
        }
        private void Save()
        {
            string makhachHang = (searchLookUpEditKH.EditValue as string) ?? "";
            string mahang = (txt_MaHang.EditValue as string) ?? "";
            string chungloai = (searchLookUpEditCL.EditValue as string) ?? "";
            string maloaihang = (searchLookUpEditLH.EditValue as string) ?? "";
            if (makhachHang == "")
            {
                MessageBox.Show("Vui lòng chọn khách hàng");
                return;
            }
            if (mahang == "")
            {
                MessageBox.Show("Vui lòng nhập mã hàng");
                return;
            }
            if (chungloai == "")
            {
                MessageBox.Show("Vui lòng chọn chủng loại");
                return;
            }
            string url = $"{URL}ERPHangHoa/Get?action=GetMaHang";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            string[] parsMH = txt_MaHang.EditValue.ToString().Split(':');
            if (_mahang == "")
            {
                foreach (var item in parsMH)
                {
                    bool tblExists = tbl.AsEnumerable()
                        .Any(x => x["MaHang"].ToString().ToUpper() == item.ToString().ToUpper() && x["MaKH"].ToString() == makhachHang);

                    if (tblExists)
                    {
                        XtraMessageBox.Show($"Mã hàng: {item} đã tồn tại trong thư viện", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }


            SaveSize();
            if (trangthai == 1 && _checkEdit)
            {

                DataTable dteditMH = CreateSaveEditMH();
                string[] parsMHs = txt_MaHang.EditValue.ToString().Split(':');
                foreach (var item in parsMH)
                {
                    var drNewSource = dteditMH.NewRow();
                    drNewSource["ID"] = 0;
                    drNewSource["MaHang"] = RemoveVietnameseTone(ReplaceSpecialCharactersPLUS(item)).ToUpper();
                    drNewSource["TenHang"] = item;
                    drNewSource["MaKH"] = searchLookUpEditKH.EditValue;
                    drNewSource["MaHangCu"] = _mahang;
                    dteditMH.Rows.Add(drNewSource);
                }
                string urled = string.Format("{0}", URL + $"ERPHangHoa/PostEdit?action=PostMHEdit&type=@TypeHangHoa");
                var msResulted = Task.Run(async () => { return await _clientExtension.PostAsync(urled, dteditMH); }).Result;
                if (msResulted.ToUpper() == "TRUE")
                {
                }
            }

        }
        private void SaveMH()
        {
            try
            {
                DataTable dtHangHoa = CreateSaveHangHoa();
                string[] parsMH = txt_MaHang.EditValue.ToString().Split(':');
                foreach (var item in parsMH)
                {
                    var drNewSource = dtHangHoa.NewRow();
                    drNewSource["ID"] = 0;
                    drNewSource["MaHang"] = !_checkEdit ? RemoveVietnameseTone(ReplaceSpecialCharactersPLUS(item)).ToUpper() : _mahang;
                    drNewSource["TenHang"] = item;
                    drNewSource["NhomHang"] = "";
                    drNewSource["GhiChu"] = txtGhiChu.EditValue;
                    drNewSource["MaLHH"] = (searchLookUpEditLH.EditValue as string) ?? "";
                    drNewSource["MaKH"] = searchLookUpEditKH.EditValue;
                    drNewSource["MaCL"] = searchLookUpEditCL.EditValue;
                    drNewSource["InTheu"] = checkBox1.Checked;
                    drNewSource["InTheuCT"] = checkBox2.Checked;
                    drNewSource["HutAm"] = checkBox3.Checked;
                    drNewSource["DoKim"] = checkBox4.Checked;
                    dtHangHoa.Rows.Add(drNewSource);
                }
                string url = string.Format("{0}", URL + $"ERPHangHoa/Post?action=PostMH&type=@TypeHangHoa");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtHangHoa); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    selectMaHang = RemoveVietnameseTone(ReplaceSpecialCharactersPLUS(parsMH[0])).ToUpper();

                    // Đóng form và báo là OK
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void SaveMau()
        {
            try
            {
                DataTable dtMau = CreateSaveMau();
                string[] parsMH = txt_MaHang.EditValue.ToString().Split(':');
                DataTable tblMau = grcMau.DataSource as DataTable;
                if (tblMau is null) return;
                foreach (var item in parsMH)
                {
                    foreach (DataRow itemMau in tblMau.Rows)
                    {
                        var drNewSource = dtMau.NewRow();
                        drNewSource["ID"] = 0;
                        drNewSource["MaMau"] = itemMau["MaMau"];
                        drNewSource["MaHang"] = !_checkEdit ? RemoveVietnameseTone(ReplaceSpecialCharactersPLUS(item)).ToUpper() : _mahang;
                        drNewSource["CodeMau"] = itemMau["CodeMau"];
                        drNewSource["TenMau"] = itemMau["TenMau"];
                        drNewSource["GhiChu"] = itemMau["GhiChu"];
                        drNewSource["MaKH"] = searchLookUpEditKH.EditValue;
                        drNewSource["MaTheMau"] = itemMau["MaTheMau"];
                        dtMau.Rows.Add(drNewSource);
                    }

                }
                string url = string.Format("{0}", URL + $"ERPHangHoa/Post?action=PostMau&type=@TypeMau");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtMau); }).Result;

            }
            catch (Exception ex)
            {

            }
        }
        private void SaveSize()
        {
            try
            {
                DataTable dtSize = CreateSaveSize();
                string[] parsMH = txt_MaHang.EditValue.ToString().Split(':');
                DataTable tblSize = grcSize.DataSource as DataTable;
                DataTable tblInSeam = grcInSeam.DataSource as DataTable;
                if (tblSize == null || tblSize.Rows.Count == 0)
                {
                    SaveMau();
                    SaveMH();
                    return;
                }
                bool isExistsIS = tblInSeam == null || tblInSeam.Rows.Count == 0;

                foreach (var item in parsMH)
                {
                    // Nếu tblInSeam có dữ liệu, duyệt như bình thường
                    if (!isExistsIS)
                    {
                        foreach (DataRow itemIS in tblInSeam.Rows)
                        {
                            foreach (DataRow itemSize in tblSize.Rows)
                            {
                                var drNewSource = dtSize.NewRow();
                                drNewSource["ID"] = 0;
                                drNewSource["MaSize"] = itemSize["MaSize"];
                                drNewSource["MaHang"] = !_checkEdit ? RemoveVietnameseTone(ReplaceSpecialCharactersPLUS(item)).ToUpper() : _mahang;
                                drNewSource["CodeSize"] = itemSize["CodeSize"];
                                drNewSource["TenSize"] = itemSize["TenSize"];
                                drNewSource["SizeSanXuat"] = itemSize["SizeSanXuat"];
                                drNewSource["GhiChu"] = itemSize["GhiChu"];
                                drNewSource["NhomSize"] = itemIS["InSeam"];
                                drNewSource["MaKH"] = searchLookUpEditKH.EditValue;
                                drNewSource["MaNhomSize"] = itemIS["MaInSeam"];
                                drNewSource["Sort"] = itemSize["Sort"];
                                drNewSource["MaTheSize"] = itemSize["MaTheSize"];
                                drNewSource["MaTheInSeam"] = itemIS["MaTheInSeam"];
                                drNewSource["SortIs"] = itemIS["SortIs"];
                                dtSize.Rows.Add(drNewSource);
                            }
                        }
                    }
                    else
                    {
                        // tblInSeam null hoặc rỗng → dùng giá trị mặc định
                        foreach (DataRow itemSize in tblSize.Rows)
                        {
                            var drNewSource = dtSize.NewRow();
                            drNewSource["ID"] = 0;
                            drNewSource["MaSize"] = itemSize["MaSize"];
                            drNewSource["MaHang"] = !_checkEdit ? RemoveVietnameseTone(ReplaceSpecialCharactersPLUS(item)).ToUpper() : _mahang;
                            drNewSource["CodeSize"] = itemSize["CodeSize"];
                            drNewSource["TenSize"] = itemSize["TenSize"];
                            drNewSource["SizeSanXuat"] = itemSize["SizeSanXuat"];
                            drNewSource["GhiChu"] = itemSize["GhiChu"];
                            drNewSource["NhomSize"] = "0";               // giá trị mặc định
                            drNewSource["MaKH"] = searchLookUpEditKH.EditValue;
                            drNewSource["MaNhomSize"] = "0";            // giá trị mặc định
                            drNewSource["Sort"] = itemSize["Sort"];
                            drNewSource["MaTheSize"] = itemSize["MaTheSize"];
                            drNewSource["MaTheInSeam"] = "THE_INSEAM_1"; // giá trị mặc định
                            drNewSource["SortIs"] = 1;
                            dtSize.Rows.Add(drNewSource);
                        }
                    }
                }

                string url = string.Format("{0}", URL + $"ERPHangHoa/Post?action=PostSize&type=@TypeSize");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSize); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    SaveMau();
                    SaveMH();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void AddGridMau(DataRow row, bool checkAdd)
        {
            DataTable dtMau;

            if (grcMau.DataSource == null)
            {
                dtMau = CreateTblMau();
                grcMau.DataSource = dtMau;
            }
            else
                dtMau = grcMau.DataSource as DataTable;

            string maMau = row["MaMau"].ToString();
            string maTheMau = row["MaTheMau"].ToString();
            if (checkAdd)
            {
                // Kiểm tra có bị trùng MaMau không trước khi thêm
                bool isExist = dtMau.AsEnumerable().Any(r => r["MaMau"].ToString() == maMau && r["MaTheMau"].ToString() == maTheMau);
                if (!isExist)
                {
                    var drNewSource = dtMau.NewRow();
                    drNewSource["ID"] = -1;
                    drNewSource["MaTheMau"] = row["MaTheMau"];
                    drNewSource["TheMau"] = row["TheMau"];
                    drNewSource["MaMau"] = row["MaMau"];
                    drNewSource["TenMau"] = row["TenMau"];
                    drNewSource["CodeMau"] = row["CodeMau"];
                    drNewSource["HinhAnh"] = row["HinhAnh"];
                    dtMau.Rows.Add(drNewSource);
                }
            }
            else
            {
                // Xóa dòng có MaMau trùng
                var rowsToDelete = dtMau.AsEnumerable()
                    .Where(r => r["MaMau"].ToString() == maMau && r["MaTheMau"].ToString() == maTheMau)
                    .ToList();

                foreach (var r in rowsToDelete)
                {
                    dtMau.Rows.Remove(r);
                }
            }

            grcMau.DataSource = dtMau;
        }



        private void AddGridInSeam(DataRow row, bool checkAdd)
        {
            DataTable dtInSeam;

            if (grcInSeam.DataSource == null)
            {
                dtInSeam = CreateTblInSeam();
                grcInSeam.DataSource = dtInSeam;
            }
            else
                dtInSeam = grcInSeam.DataSource as DataTable;

            string maInSeam = row["MaInSeam"].ToString();
            string maTheInSeam = row["MaTheInSeam"].ToString();
            if (checkAdd)
            {

                // Kiểm tra có bị trùng MaMau không trước khi thêm
                bool isExist = dtInSeam.AsEnumerable().Any(r => r["MaInSeam"].ToString() == maInSeam && r["MaTheInSeam"].ToString() == maTheInSeam);
                if (!isExist)
                {
                    maxSortSIs++;
                    var drNewSource = dtInSeam.NewRow();
                    drNewSource["ID"] = -1;
                    drNewSource["MaTheInSeam"] = row["MaTheInSeam"];
                    drNewSource["TenTheInSeam"] = row["TenTheInSeam"];
                    drNewSource["MaInSeam"] = row["MaInSeam"];
                    drNewSource["InSeam"] = row["InSeam"];
                    drNewSource["SortIs"] = maxSortSIs;
                    dtInSeam.Rows.Add(drNewSource);
                }
            }
            else
            {
                // Xóa dòng có MaMau trùng
                var rowsToDelete = dtInSeam.AsEnumerable()
                    .Where(r => r["MaInSeam"].ToString() == maInSeam && r["MaTheInSeam"].ToString() == maTheInSeam)
                    .ToList();

                foreach (var r in rowsToDelete)
                {
                    dtInSeam.Rows.Remove(r);
                }
            }

           // grcInSeam.DataSource = dtInSeam;
            DataView dv = dtInSeam.DefaultView;
            dv.Sort = "SortIs ASC";
            DataTable sortedDt = dv.ToTable();
            grcInSeam.DataSource = sortedDt;
        }



        private void AddGridSize(DataRow row, bool checkAdd)
        {
            DataTable dtSize;

            if (grcSize.DataSource == null)
            {
                dtSize = CreateTblSize();
                grcSize.DataSource = dtSize;
            }
            else
                dtSize = grcSize.DataSource as DataTable;

            string maSize = row["MaSize"].ToString();
            string maTheSize = row["MaTheSize"].ToString();
            if (checkAdd)
            {

                // Kiểm tra có bị trùng MaMau không trước khi thêm
                bool isExist = dtSize.AsEnumerable().Any(r => r["MaSize"].ToString() == maSize && r["MaTheSize"].ToString() == maTheSize);
                if (!isExist)
                {
                    maxSortSize++;
                    var drNewSource = dtSize.NewRow();
                    drNewSource["ID"] = -1;
                    drNewSource["MaTheSize"] = row["MaTheSize"];
                    drNewSource["TheSize"] = row["TheSize"];
                    drNewSource["MaSize"] = row["MaSize"];
                    drNewSource["TenSize"] = row["TenSize"];
                    drNewSource["CodeSize"] = row["CodeSize"];
                    drNewSource["Sort"] = maxSortSize;
                    drNewSource["GhiChu"] = row["GhiChu"];
                    drNewSource["SizeSanXuat"] = row["SizeSanXuat"];
                    dtSize.Rows.Add(drNewSource);

                }
            }
            else
            {
                // Xóa dòng có MaMau trùng
                var rowsToDelete = dtSize.AsEnumerable()
                    .Where(r => r["MaSize"].ToString() == maSize && r["MaTheSize"].ToString() == maTheSize)
                    .ToList();

                foreach (var r in rowsToDelete)
                {
                    dtSize.Rows.Remove(r);
                }
            }
            DataView dv = dtSize.DefaultView;
            dv.Sort = "Sort ASC";
            DataTable sortedDt = dv.ToTable();
            grcSize.DataSource = sortedDt;
        }
        #region Create Table
        private DataTable CreateTblMau()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheMau", typeof(string));
            dt.Columns.Add("TheMau", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("CodeMau", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("HinhAnh", typeof(string));
            return dt;
        }
        private DataTable CreateTblInSeam()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheInSeam", typeof(string));
            dt.Columns.Add("TenTheInSeam", typeof(string));
            dt.Columns.Add("MaInSeam", typeof(string));
            dt.Columns.Add("InSeam", typeof(string));
            dt.Columns.Add("SortIs", typeof(int));
            return dt;
        }


        private DataTable CreateTblSize()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheSize", typeof(string));
            dt.Columns.Add("TheSize", typeof(string));
            dt.Columns.Add("MaSize", typeof(string));
            dt.Columns.Add("TenSize", typeof(string));
            dt.Columns.Add("CodeSize", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("SizeSanXuat", typeof(string));
            return dt;
        }
        #endregion



        public static DataTable CreateSaveEditMH()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("TenHang", typeof(string));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHangCu", typeof(string));
            dt.Columns.Add("InTheu", typeof(bool));
            dt.Columns.Add("InTheuCT", typeof(bool));
            return dt;
        }

        public static DataTable CreateSaveHangHoa()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("TenHang", typeof(string));
            dt.Columns.Add("NhomHang", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("MaLHH", typeof(string));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaCL", typeof(string));
            dt.Columns.Add("InTheu", typeof(bool));
            dt.Columns.Add("InTheuCT", typeof(bool));
            dt.Columns.Add("HutAm", typeof(bool));
            dt.Columns.Add("DoKim", typeof(bool));
            return dt;
        }


        public static DataTable CreateSaveMau()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("CodeMau", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaTheMau", typeof(string));
            return dt;
        }


        public static DataTable CreateSaveSize()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaSize", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("CodeSize", typeof(string));
            dt.Columns.Add("TenSize", typeof(string));
            dt.Columns.Add("SizeSanXuat", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NhomSize", typeof(string));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaNhomSize", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("MaTheSize", typeof(string));
            dt.Columns.Add("MaTheInSeam", typeof(string));
            dt.Columns.Add("SortIs", typeof(int));
            return dt;
        }
        public static string ReplaceSpecialCharactersPLUS(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(input, replacement);
            int plusCount = input.Count(c => c == '+');
            result += new string('_', plusCount);

            return result;
        }
        public static string RemoveVietnameseTone(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
            result = result.Replace('đ', 'd');
            return result;
        }
        private void GrvMau_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                var urlHost = (string)settingsReader.GetValue("URLV2", typeof(String));
                //if (e.Column.FieldName == "STT" && e.IsGetData)
                //    e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
                if ((e.Column.FieldName == "UrlAnh") && e.IsGetData)
                {
                    DataRow dr = grvMau.GetDataRow(e.ListSourceRowIndex);
                    if (dr == null) return;
                    string url = urlHost + "/Images/BangMau/" + dr["HinhAnh"].ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            using (var wc = new System.Net.WebClient())
                            {
                                byte[] data = wc.DownloadData(url);
                                using (var ms = new MemoryStream(data))
                                    e.Value = Image.FromStream(ms);
                            }
                        }
                        catch
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            using (var frm = new frmERP_BangInSeam())
            {
                frm.ShowDialog();

                GetInSeam("");
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var frm = new frmERP_BangMau())
            {
                frm.ShowDialog();

                GetMau("");
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var frm = new frmERP_BangSize())
            {
                frm.ShowDialog();
                GetSize("");
            }
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            var focusedRowHandle = grvSize.FocusedRowHandle;
            if (focusedRowHandle <= 0)
                return;
            DataTable dt = (DataTable)grcSize.DataSource;
            int currentSort = Convert.ToInt32(grvSize.GetFocusedRowCellValue("Sort"));
            int previousSort = Convert.ToInt32(grvSize.GetRowCellValue(focusedRowHandle - 1, "Sort"));
            grvSize.SetRowCellValue(focusedRowHandle, "Sort", previousSort);
            grvSize.SetRowCellValue(focusedRowHandle - 1, "Sort", currentSort);
            DataView dv = dt.DefaultView;
            dv.Sort = "Sort ASC";
            grcSize.DataSource = dv.ToTable();
            grvSize.FocusedRowHandle = focusedRowHandle - 1;
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            var focusedRowHandle = grvSize.FocusedRowHandle;
            if (focusedRowHandle < 0 || focusedRowHandle >= grvSize.RowCount - 1)
                return;
            DataTable dt = (DataTable)grcSize.DataSource;
            int currentSort = Convert.ToInt32(grvSize.GetFocusedRowCellValue("Sort"));
            int nextSort = Convert.ToInt32(grvSize.GetRowCellValue(focusedRowHandle + 1, "Sort"));
            grvSize.SetRowCellValue(focusedRowHandle, "Sort", nextSort);
            grvSize.SetRowCellValue(focusedRowHandle + 1, "Sort", currentSort);
            DataView dv = dt.DefaultView;
            dv.Sort = "Sort ASC";
            grcSize.DataSource = dv.ToTable();
            grvSize.FocusedRowHandle = focusedRowHandle + 1;
        }
        private void btnUp1_Click(object sender, EventArgs e)
        {
            var focusedRowHandle = grvInSeam.FocusedRowHandle;
            if (focusedRowHandle <= 0)
                return;
            DataTable dt = (DataTable)grcInSeam.DataSource;
            int currentSort = Convert.ToInt32(grvInSeam.GetFocusedRowCellValue("SortIs"));
            int previousSort = Convert.ToInt32(grvInSeam.GetRowCellValue(focusedRowHandle - 1, "SortIs"));
            grvInSeam.SetRowCellValue(focusedRowHandle, "SortIs", previousSort);
            grvInSeam.SetRowCellValue(focusedRowHandle - 1, "SortIs", currentSort);
            DataView dv = dt.DefaultView;
            dv.Sort = "SortIs ASC";
            grcInSeam.DataSource = dv.ToTable();
            grvInSeam.FocusedRowHandle = focusedRowHandle - 1;
        }

        private void btnDown1_Click(object sender, EventArgs e)
        {
            var focusedRowHandle = grvInSeam.FocusedRowHandle;
            if (focusedRowHandle < 0 || focusedRowHandle >= grvInSeam.RowCount - 1)
                return;
            DataTable dt = (DataTable)grcInSeam.DataSource;
            int currentSort = Convert.ToInt32(grvInSeam.GetFocusedRowCellValue("SortIs"));
            int nextSort = Convert.ToInt32(grvInSeam.GetRowCellValue(focusedRowHandle + 1, "SortIs"));
            grvInSeam.SetRowCellValue(focusedRowHandle, "SortIs", nextSort);
            grvInSeam.SetRowCellValue(focusedRowHandle + 1, "SortIs", currentSort);
            DataView dv = dt.DefaultView;
            dv.Sort = "SortIs ASC";
            grcInSeam.DataSource = dv.ToTable();
            grvInSeam.FocusedRowHandle = focusedRowHandle + 1;
        }

        private void btnUp2_Click(object sender, EventArgs e)
        {
            if (!_checkEdit) return;
            DataTable dtXoaSize = this.grcSize.DataSource as DataTable;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaHang", typeof(string));
            dtSave.Columns.Add("MaKH", typeof(string));

            dtSave.Columns.Add("MaTheSize", typeof(string));
            dtSave.Columns.Add("MaSize", typeof(string));
            int[] selectedRows = grvSize.GetSelectedRows();

            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle >= 0)
                {
                    DataRow dr = grvSize.GetDataRow(rowHandle);
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaKH"] = _maKH;
                    _dr["MaHang"] = _mahang;
                    _dr["MaTheSize"] = dr["MaTheSize"];
                    _dr["MaSize"] = dr["MaSize"];
                    dtSave.Rows.Add(_dr);
                }
            }
            //foreach (DataRow dr in dtXoaSize.Rows)
            //{
            //    if ((bool)dr["Chon"] == true && dr["Chon"] != "")
            //    {
            //        DataRow _dr = dtSave.NewRow();
            //        _dr["MaKH"] = _maKH;
            //        _dr["MaHang"] = _mahang;
            //        _dr["MaTheSize"] = dr["MaTheSize"];
            //        _dr["MaSize"] = dr["MaSize"];
            //        dtSave.Rows.Add(_dr);
            //    }
            //}
            if (dtSave.Rows.Count > 0 && dtSave != null)
            {
                string urldvsx = string.Format("{0}", URL + $"ERPHangHoa/GetKiemTra?action=KIEMTRA&type=@TypeHangHoa");
                string jsondvsx = Task.Run(async () => { return await _clientExtension.PostAsync(urldvsx, dtSave); }).Result;
                if (jsondvsx == "[]")
                {
                    string urled = string.Format("{0}", URL + $"ERPHangHoa/DeleteSize?action=DELETE&type=@TypeHangHoa");
                    var msResulted = Task.Run(async () => { return await _clientExtension.PostAsync(urled, dtSave); }).Result;
                    if (msResulted.ToUpper() == "TRUE")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        GetSizeMH();
                    }
                }
                else
                {
                    DataTable tbldvsx = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
                    var sizes = tbldvsx.Rows.Cast<DataRow>().Select(r => r["Size"]?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

                    string _size = string.Join(";", sizes);
                    XtraMessageBox.Show("Size " + _size + " đã được sử dụng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
        }
        private void btnUp21_Click(object sender, EventArgs e)
        {
            //DataRow row = grvInSeam.GetFocusedDataRow();
            //if (row == null) return;
            if (!_checkEdit) return;
            //string mahang = _mahang;
            //string khachHang = _maKH;
            //string maInSeam = row["MaInSeam"].ToString();
            //string maTheInSeam = row["MaTheInSeam"].ToString();
            //string url = string.Format("{0}", URL + $"ERPHangHoa/Delete?action=DeleteIS&id={mahang}&para2={khachHang}&para3={maInSeam}&para4={maTheInSeam}");
            //var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            //if (msResult.ToUpper() == "TRUE")
            //{
            //    clsWaitForm.ShowSuccessForm(this, 2000);
            //    GetISMH();
            //}
            DataTable dtXoaSize = this.grvInSeam.DataSource as DataTable;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaHang", typeof(string));
            dtSave.Columns.Add("MaKH", typeof(string));

            dtSave.Columns.Add("MaTheSize", typeof(string));
            dtSave.Columns.Add("MaSize", typeof(string));
            int[] selectedRows = grvInSeam.GetSelectedRows();

            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle >= 0)
                {
                    DataRow dr = grvInSeam.GetDataRow(rowHandle);
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaKH"] = _maKH;
                    _dr["MaHang"] = _mahang;
                    _dr["MaTheSize"] = dr["MaTheInSeam"];
                    _dr["MaSize"] = dr["MaInSeam"];
                    dtSave.Rows.Add(_dr);
                }
            }
            if (dtSave.Rows.Count > 0 && dtSave != null)
            {
                string urldvsx = string.Format("{0}", URL + $"ERPHangHoa/GetKiemTra?action=KIEMTRAIS&type=@TypeHangHoa");
                string jsondvsx = Task.Run(async () => { return await _clientExtension.PostAsync(urldvsx, dtSave); }).Result;
                if (jsondvsx == "[]")
                {
                    string urled = string.Format("{0}", URL + $"ERPHangHoa/DeleteSize?action=DELETEIS&type=@TypeHangHoa");
                    var msResulted = Task.Run(async () => { return await _clientExtension.PostAsync(urled, dtSave); }).Result;
                    if (msResulted.ToUpper() == "TRUE")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        GetISMH();
                        GetSizeMH();
                    }
                }
                else
                {
                    DataTable tbldvsx = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
                    var inseam = tbldvsx.Rows.Cast<DataRow>().Select(r => r["DauSize"]?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

                    string _inseam = string.Join(";", inseam);
                    XtraMessageBox.Show("Inseam " + _inseam + " đã được sử dụng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
        }

    }
}