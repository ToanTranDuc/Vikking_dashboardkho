using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPKhoVT : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty, _maDVVT = "", _maCLVT = "";
        int _rowAdd = -1, FocusedIndex = 0;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _isNhapXuat = false;

        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        List<ERPPhieuNhapVTEntiy> _lstPhieuNhap = new List<ERPPhieuNhapVTEntiy>();
        List<ERPPhieuXuatVTEntiy> _lstPhieuXuat = new List<ERPPhieuXuatVTEntiy>();

        public frmERPKhoVT()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            LoadDonViTinh();
            LoadDSChungLoai();
            LoadKhoVatTu();
            CheckPerminsion();
        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);//SystemUser/GetPer/userID=...
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                gridViewKhoVatTu.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                btnNhapXuatVT.Enabled = false;
            }
            if (!_allowEdit)
            {
                gridViewKhoVatTu.OptionsBehavior.Editable = false;
                btSave.Enabled = false;
            }
            if (!_allowDelete)
            {
                btDelete.Enabled = false;
            }
        }

        private void LoadDonViTinh()
        {
            string url = string.Format("{0}?", URL + "ERPThuVienVT/GetDVVT");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count > 0) _maDVVT = tbl.Rows[0]["MaDVVT"].ToString();
            repositoryItemSearchLookUpDonVi.DataSource = tbl;
        }
        private void LoadDSChungLoai()
        {
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/GetKhoVatTu", "GetCLVT", "para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow dr = tbl.AsEnumerable().Where(x => x["ID"].ToString() == "").FirstOrDefault();
            if (dr != null)
                tbl.Rows.Remove(dr);
            if (tbl.Rows.Count > 0) _maCLVT = tbl.Rows[0]["MaCLVT"].ToString();
            repositoryItemSearchLookUpChungLoai.DataSource = tbl;
        }

        private void LoadKhoVatTu()
        {
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/GetKhoVatTu", "GetKVT", "para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow dr = tbl.AsEnumerable().Where(x => x["ID"].ToString() == "").FirstOrDefault();
            if (dr != null)
                tbl.Rows.Remove(dr);
            if (_isNhapXuat)
            {
                foreach (ERPPhieuNhapVTEntiy itemVT_Nhap in _lstPhieuNhap)
                    tbl.AsEnumerable().ToList().ForEach(x => { if (x["MaVT"].ToString() == itemVT_Nhap.MaVT) { x["Chon"] = true; } });
                foreach (ERPPhieuXuatVTEntiy itemVT_Xuat in _lstPhieuXuat)
                    tbl.AsEnumerable().ToList().ForEach(x => { if (x["MaVT"].ToString() == itemVT_Xuat.MaVT) { x["Chon"] = true; } });
            }
            int focusRowIndex = gridViewKhoVatTu.FocusedRowHandle;
            int topHandle = gridViewKhoVatTu.TopRowIndex;
            dgrKhoVatTu.DataSource = tbl;
            gridViewKhoVatTu.FocusedRowHandle = focusRowIndex;
            gridViewKhoVatTu.TopRowIndex = topHandle;
            ChanLapPhieu(true);
        }
        private void SaveKhoVT()
        {
            DataTable tblSource = dgrKhoVatTu.DataSource as DataTable;
            List<DataRow> lstDR = tblSource.AsEnumerable().Where(x => x["ID"].ToString() == "0" || (bool)x["IsUpdate"] == true).ToList();
            if (lstDR == null || lstDR.Count == 0) return;
            string json = JsonConvert.SerializeObject(lstDR.CopyToDataTable());
            List<ERPKhoVatTuEntiy> lstSave = JsonConvert.DeserializeObject<List<ERPKhoVatTuEntiy>>(json);
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/SaveKhoVT", "SaveKhoVT", "Para");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSave); }).Result;
            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 800);
                LoadKhoVatTu();
            }
            else XtraMessageBox.Show(result);
        }

        private void SaveKhoVT(DataTable tblSource)
        {
            List<DataRow> lstDR = tblSource.AsEnumerable().Where(x => x["ID"].ToString() == "0" || (bool)x["IsUpdate"] == true).ToList();
            if (lstDR == null || lstDR.Count == 0) return;
            string json = JsonConvert.SerializeObject(lstDR.CopyToDataTable());
            List<ERPKhoVatTuEntiy> lstSave = JsonConvert.DeserializeObject<List<ERPKhoVatTuEntiy>>(json);
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/SaveKhoVT", "SaveKhoVT", "Para");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSave); }).Result;
            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 800);
                LoadKhoVatTu();
            }
            else XtraMessageBox.Show(result);
        }

        private void DeleteKhoVT()
        {
            DataRow drFocus = gridViewKhoVatTu.GetFocusedDataRow();
            if (drFocus == null) return;
            DialogResult op = XtraMessageBox.Show("Xoá vật tư bạn có muốn xoá luôn phiếu nhập xuất của vật tư không?", "Thông báo", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (op == DialogResult.Yes)
            {
                string url = string.Format("{0}?action={1}&&ID={2}", URL + "KhoVatTu/DeleteVT", "DeleteALL", drFocus["MaVT"].ToString());
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 800);
                    LoadKhoVatTu();
                }
            }
            if (op == DialogResult.No)
            {
                string url = string.Format("{0}?action={1}&&ID={2}", URL + "KhoVatTu/DeleteVT", "Delete", drFocus["MaVT"].ToString());
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 800);
                    LoadKhoVatTu();
                }
            }
        }

        private bool CheckMaVT(string maVT)
        {
            DataTable tblSource = dgrKhoVatTu.DataSource as DataTable;
            List<DataRow> lstDR = tblSource.AsEnumerable().Where(x => x["MaVT"].ToString() == maVT).ToList();
            if (lstDR.Count() > 1)
                return true;
            else return false;
        }

        private void gridViewKhoVatTu_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0 && e.FocusedRowHandle != int.MinValue)
            {
                DataTable tbl = dgrKhoVatTu.DataSource as DataTable;
                DataRow drNew = tbl.NewRow();
                drNew["ID"] = "0";
                drNew["STT"] = "0";
                drNew["DonVi"] = _maDVVT;
                drNew["SoLuong"] = 0;
                drNew["IsUpdate"] = false;
                drNew["MaCLVT"] = _maCLVT;
                tbl.Rows.InsertAt(drNew, 0);
                ChanLapPhieu(false);
                gridViewKhoVatTu.FindFilterText = string.Empty;
            }
        }
        private void gridViewKhoVatTu_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow drEdit = gridViewKhoVatTu.GetFocusedDataRow();
            if (drEdit == null) return;
            drEdit["IsUpdate"] = true;
        }
        private void gridViewKhoVatTu_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;

            if (view.FocusedColumn.FieldName == "MaVT")
            {
                if (string.IsNullOrEmpty(e.Value?.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Mã vật tư không được để trống!";
                }
                else if (CheckMaVT(e.Value?.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Mã vật tư không được trùng lặp!";
                }
            }
        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DongTabNhapXuat();
            LoadDonViTinh();
            LoadKhoVatTu();
        }

        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = splitContainerControl1;
            SaveKhoVT();
        }

        private void btDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DeleteKhoVT();
        }

        private void repoCheckChonVT_CheckedChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.CheckEdit ck = sender as DevExpress.XtraEditors.CheckEdit;
            DataRow drFocus = gridViewKhoVatTu.GetFocusedDataRow();
            if (drFocus == null) return;
            if (xtraTabNhapXuat.SelectedTabPageIndex == 0)
                CreateListPhieuNhap(ck.Checked, drFocus);
            else CreateListPhieuXuat(ck.Checked, drFocus);

        }

        private void ChanLapPhieu(bool isEnable)
        {
            if (_isNhapXuat)
            {
                btnLapPhieuNhap.Enabled = isEnable;
                btnLapPhieuXuat.Enabled = isEnable;
                if (isEnable)
                {
                    lbTB_Nhap.Text = "";
                    lbTB_Xuat.Text = "";
                    colChon.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    lbTB_Nhap.Text = "Thông báo: Có dữ liệu vật tư chưa lưu, không thể lập phiếu nhập, vui lòng lưu dữ liệu!";
                    lbTB_Xuat.Text = "Thông báo: Có dữ liệu vật tư chưa lưu, không thể lập phiếu xuất, vui lòng lưu dữ liệu!";
                    colChon.OptionsColumn.AllowEdit = false;
                }
            }
            else
            {
                btnNhapXuatVT.Enabled = isEnable;
            }

        }

        #region Phiếu nhập vật tư ----------------------------------------------------------------------------------------->

        private void CreateListPhieuNhap(bool isChecked, DataRow drFocus)
        {
            try
            {
                if (isChecked)
                {
                    ERPPhieuNhapVTEntiy itemNew = _lstPhieuNhap.Where(x => x.MaVT == drFocus["MaVT"].ToString()).FirstOrDefault();// new ERPPhieuNhapVTEntiy();
                    if (itemNew == null)
                    {
                        itemNew = new ERPPhieuNhapVTEntiy();
                        _lstPhieuNhap.Add(itemNew);
                    }
                    itemNew.ID = 0;
                    itemNew.MaPhieu = txtMaPhieuNhap.Text;
                    itemNew.MaVT = drFocus["MaVT"].ToString();
                    itemNew.TenVT = drFocus["TenVT"].ToString();
                    itemNew.SoLuong = 0;
                    itemNew.SoLuongTong = Convert.ToDouble(drFocus["SoLuong"]);
                    itemNew.NgayNhap = ((DateTime)dtNgayLapPhieuNhap.EditValue).ToString("yyyy-MM-dd");
                }
                else
                {
                    ERPPhieuNhapVTEntiy drItem = _lstPhieuNhap.Where(x => x.MaVT == drFocus["MaVT"].ToString()).FirstOrDefault();
                    if (drItem == null) return;
                    _lstPhieuNhap.Remove(drItem);
                }
                dgrLapPhieuNhap.DataSource = _lstPhieuNhap;
                dgrLapPhieuNhap.RefreshDataSource();
            }
            catch (Exception ex) { }
        }

        private void btnClearPN_Click(object sender, EventArgs e)
        {
            _lstPhieuNhap = new List<ERPPhieuNhapVTEntiy>();
            dgrLapPhieuNhap.DataSource = _lstPhieuNhap;
            dgrLapPhieuNhap.RefreshDataSource();
            ClearChonVatTu();
        }

        private void gridViewLapPhieu_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }

        private void btnLapPhieuNhap_Click(object sender, EventArgs e)
        {
            SavePhieuNhapKhoVT();
        }

        private void SavePhieuNhapKhoVT()
        {
            if (_lstPhieuNhap == null || _lstPhieuNhap.Count == 0) return;
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/SaveKhoVT", "SavePhieuNhap", "Para");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstPhieuNhap); }).Result;
            if (result.ToLower() == "true")
            {
                _lstPhieuNhap = new List<ERPPhieuNhapVTEntiy>();
                dgrLapPhieuNhap.DataSource = _lstPhieuNhap;
                dgrLapPhieuNhap.RefreshDataSource();
                TaoMoiMaPhieu();
                clsWaitForm.ShowSuccessForm(this, 800);
                LoadKhoVatTu();
            }
            else XtraMessageBox.Show(result);
        }

        #endregion

        #region Phiếu xuất vật tư ------------------------------------------------------------------------------------------------>
        private void CreateListPhieuXuat(bool isChecked, DataRow drFocus)
        {
            try
            {
                if (isChecked)
                {
                    ERPPhieuXuatVTEntiy itemNew = _lstPhieuXuat.Where(x => x.MaVT == drFocus["MaVT"].ToString()).FirstOrDefault();//new ERPPhieuXuatVTEntiy();
                    if (itemNew == null)
                    {
                        itemNew = new ERPPhieuXuatVTEntiy();
                        _lstPhieuXuat.Add(itemNew);
                    }
                    itemNew.ID = 0;
                    itemNew.MaPhieu = txtMaPhieuXuat.Text;
                    itemNew.MaVT = drFocus["MaVT"].ToString();
                    itemNew.TenVT = drFocus["TenVT"].ToString();
                    itemNew.SoLuong = 0;
                    itemNew.SoLuongTong = Convert.ToDouble(drFocus["SoLuong"]);
                    itemNew.NgayXuat = ((DateTime)dtNgayLapPhieuXuat.EditValue).ToString("yyyy-MM-dd");

                }
                else
                {
                    ERPPhieuXuatVTEntiy drItem = _lstPhieuXuat.Where(x => x.MaVT == drFocus["MaVT"].ToString()).FirstOrDefault();
                    if (drItem == null) return;
                    _lstPhieuXuat.Remove(drItem);
                }
                dgrLapPhieuXuat.DataSource = _lstPhieuXuat;
                dgrLapPhieuXuat.RefreshDataSource();
            }
            catch (Exception ex) { }
        }

        private void gridViewLapPhieuXuat_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }

        private void btnClearPX_Click(object sender, EventArgs e)
        {
            _lstPhieuXuat = new List<ERPPhieuXuatVTEntiy>();
            dgrLapPhieuXuat.DataSource = _lstPhieuXuat;
            dgrLapPhieuXuat.RefreshDataSource();
            ClearChonVatTu();
        }
        private void btnLapPhieuXuat_Click(object sender, EventArgs e)
        {
            SavePhieuXuatKhoVT();
        }
        private void SavePhieuXuatKhoVT()
        {
            if (_lstPhieuXuat == null || _lstPhieuXuat.Count == 0) return;
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/SaveKhoVT", "SavePhieuXuat", "Para");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstPhieuXuat); }).Result;
            if (result.ToLower() == "true")
            {
                _lstPhieuXuat = new List<ERPPhieuXuatVTEntiy>();
                dgrLapPhieuXuat.DataSource = _lstPhieuXuat;
                dgrLapPhieuXuat.RefreshDataSource();
                TaoMoiMaPhieu();
                clsWaitForm.ShowSuccessForm(this, 800);
                LoadKhoVatTu();
            }
            else XtraMessageBox.Show(result);
        }

        #endregion

        #region Hàm xử lý chung ------------------------------------------------------------------------------------------------->
        private void ClearChonVatTu()
        {
            DataTable tblSource = dgrKhoVatTu.DataSource as DataTable;
            if (tblSource == null) return;
            tblSource.AsEnumerable().ToList().ForEach(row => row["Chon"] = false);
        }

        private void MoTabNhapXuat()
        {
            _isNhapXuat = true;
            TaoMoiMaPhieu();
            colChon.Visible = true;
            btDelete.Enabled = false;
            splitContainerControl1.SplitterPosition = (int)(this.Height * 0.4);
        }
        private void DongTabNhapXuat()
        {
            _isNhapXuat = false;
            ReloadDataSource();
            colChon.Visible = false;
            btDelete.Enabled = true;
            splitContainerControl1.SplitterPosition = (int)(this.Height);

        }

        private void ReloadDataSource()
        {
            _lstPhieuNhap = new List<ERPPhieuNhapVTEntiy>();
            dgrLapPhieuNhap.DataSource = _lstPhieuNhap;
            dgrLapPhieuNhap.RefreshDataSource();

            _lstPhieuXuat = new List<ERPPhieuXuatVTEntiy>();
            dgrLapPhieuXuat.DataSource = _lstPhieuXuat;
            dgrLapPhieuXuat.RefreshDataSource();
        }
        private void btnNhapXuatVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MoTabNhapXuat();
        }

        private void gridViewLapPhieuNhap_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {

        }

        private void gridViewLapPhieuXuat_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            ERPPhieuXuatVTEntiy itemFocus = gridViewLapPhieuXuat.GetFocusedRow() as ERPPhieuXuatVTEntiy;
            if (view.FocusedColumn.FieldName == "SoLuong")
            {
                if (Convert.ToDouble(e.Value) > itemFocus.SoLuongTong)
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng xuất lớn hơn số lượng tồn. Vui lòng kiểm tra lại số lượng!";
                }
            }
        }

        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel File|*.xlsx;*.xls";
            openFileDialog.Title = "Import Excel";
            openFileDialog.Multiselect = false;

            DialogResult result = openFileDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            string filePath = openFileDialog.FileName;
            DataTable dataTable = new DataTable();

            FileInfo fileInfo = new FileInfo(filePath);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            DataTable tblDVT = repositoryItemSearchLookUpDonVi.DataSource as DataTable;
            DataTable tblCL = repositoryItemSearchLookUpChungLoai.DataSource as DataTable;
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                bool hasHeader = true; // `true` nếu file Excel của bạn có header row
                bool isMissCol = true;
                // Thêm các cột vào DataTable
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dataTable.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
                }
                dataTable.Columns.Add("ID", typeof(string));
                dataTable.Columns.Add("IsUpdate", typeof(bool));
                dataTable.Columns.Add("SoLuong", typeof(int));
                // Thêm các hàng vào DataTable
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                {
                    var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                    DataRow row = dataTable.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text.Trim();
                    }
                    row["ID"] = "0";
                    row["SoLuong"] = 0;
                    row["IsUpdate"] = true;
                    DataRow drDV = tblDVT.AsEnumerable().Where(x => x["TenDVVT"].ToString().ToLower() == row["DonVi"].ToString().Trim().ToLower()).FirstOrDefault();
                    DataRow drCL = tblCL.AsEnumerable().Where(x => x["TenCLVT"].ToString().ToLower() == row["MaCLVT"].ToString().Trim().ToLower()).FirstOrDefault();
                    if (drDV == null) row["DonVi"] = _maDVVT;
                    else row["DonVi"] = drDV["MaDVVT"].ToString();
                    if (drCL == null) row["MaCLVT"] = _maCLVT;
                    else row["MaCLVT"] = drCL["MaCLVT"].ToString();
                    dataTable.Rows.Add(row);
                }
                if (CheckDataImport(dataTable)) SaveKhoVT(dataTable);
            }
        }

        private bool CheckDataImport(DataTable tblSave)
        {
            string result = "";
            var duplicatedIds = tblSave.AsEnumerable()
                                .GroupBy(row => row["MaVT"])
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .ToList();
            if (duplicatedIds.Any())
            {
                foreach (var MaVT in duplicatedIds)
                {
                    result += MaVT.ToString() + "\r\n";
                }
                MessageBox.Show(string.Format("Mã vật tư trong file bị trùng lặp: \r\n {0}", result));
                return false;
            }
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/GetKhoVatTu", "GetKVT", "para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            List<string> lstMaVT = tbl.AsEnumerable().Where(x => x["MaVT"].ToString() != "").Select(x => x["MaVT"].ToString()).ToList();

            var matchedMaVT = tblSave.AsEnumerable()
                            .Select(row => row["MaVT"].ToString())
                            .Where(MaVT => lstMaVT.Contains(MaVT))
                            .Distinct()
                            .ToList();
            if (matchedMaVT.Any())
            {
                foreach (var MaVT in matchedMaVT)
                {
                    result += MaVT.ToString() + "\r\n";
                }
                MessageBox.Show(string.Format("Mã vật tư đã tồn tại trong dữ liệu: \r\n {0}", result));
                return false;
            }
            return true;
        }

        private void gridViewKhoVatTu_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            DataRow drCheck = view.GetFocusedDataRow();
            if (drCheck == null) return;
            if (view.FocusedColumn.FieldName == "MaVT")
            {
                string idValue = drCheck["ID"].ToString();
                if (idValue != "0")
                {
                    e.Cancel = true;
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridViewKhoVatTu.OptionsFind.AlwaysVisible)
            {
                gridViewKhoVatTu.OptionsFind.AlwaysVisible = false;
                barButtonItem1.Caption = "Tìm kiếm (CTRL+F)";
            }
            else
            {
                gridViewKhoVatTu.OptionsFind.AlwaysVisible = true;
                barButtonItem1.Caption = "Ẩn tìm kiếm";
            }
        }

        private void repobtnSLTon_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            dgrDSPhieuNhapXuat.Visible = true;
            dgrDSPhieuNhapXuat.DataSource = new DataTable();
            DataRow drFocus = gridViewKhoVatTu.GetFocusedDataRow();
            if (drFocus == null) return;
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/GetKhoVatTu", "GetDetailNX", drFocus["MaVT"].ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow dr = tbl.AsEnumerable().Where(x => x["ID"].ToString() == "").FirstOrDefault();
            if (dr != null)
                tbl.Rows.Remove(dr);
            dgrDSPhieuNhapXuat.DataSource = tbl;
        }

        private void colRepoTxtSoLuongNhap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-' || e.KeyChar == '+') // chặn dấu trừ
                e.Handled = true;
        }

        private void repositoryItemTextEdit3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-' || e.KeyChar == '+') // chặn dấu trừ
                e.Handled = true;
        }

        private void btnFileMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CopyTemplateFile();
        }

        private void btnChungLoaiVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmChungLoaiVT frm = new frmChungLoaiVT();
            frm.ShowDialog();
            LoadDSChungLoai();
        }

        private void xtraTabControl1_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            TaoMoiMaPhieu();
            ClearChonVatTu();
            ReloadDataSource();
        }
        private void xtraTabControl1_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            DongTabNhapXuat();
            ClearChonVatTu();
        }
        private void TaoMoiMaPhieu()
        {
            dtNgayLapPhieuNhap.EditValue = DateTime.Now;
            dtNgayLapPhieuXuat.EditValue = DateTime.Now;
            txtMaPhieuNhap.Text = string.Format("PNVT_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            txtMaPhieuXuat.Text = string.Format("PXVT_{0}", DateTime.Now.ToString("ddMMyyyyHHmmss"));
        }
        private void frmERPKhoVT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                this.ActiveControl = splitContainerControl1;
                SaveKhoVT();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteKhoVT();
            }
            else if (e.KeyCode == Keys.F5)
            {
                LoadDonViTinh();
                LoadKhoVatTu();
            }
        }
        private void frmERPKhoVT_Resize(object sender, EventArgs e)
        {
            if (_isNhapXuat)
            {
                splitContainerControl1.SplitterPosition = (int)(this.Height * 0.4);
            }
            else
            {
                splitContainerControl1.SplitterPosition = (int)(this.Height);
            }

        }
        private void CopyTemplateFile()
        {
            string templateFileName = "TemplateImport_VatTu.xlsx"; // Tên file trong thư mục Templates
            string templateFilePath = Path.Combine(Application.StartupPath, "Templates", templateFileName);

            if (!File.Exists(templateFilePath))
            {
                MessageBox.Show("Không tìm thấy file mẫu.");
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Chọn nơi lưu file";
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                saveDialog.FileName = templateFileName; // Gợi ý tên file lưu

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(templateFilePath, saveDialog.FileName, true);
                        DialogResult result = MessageBox.Show("Bạn có muốn mở file vừa lưu không?", "Mở file", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = saveDialog.FileName,
                                UseShellExecute = true // Để mở bằng ứng dụng mặc định
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lưu file:\n" + ex.Message);
                    }
                }
            }
        }
        #endregion
    }
}
