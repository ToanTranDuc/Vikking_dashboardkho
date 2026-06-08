using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SysData = System.Data;
using SysDraw = System.Drawing;
using winForm = System.Windows.Forms;
using InteropExcel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using NtbSoft.ERP.Win.Modules.CanDoiDonHang;
using NtbSoft.ERP.Win.Modules.ThuVien;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using OfficeOpenXml;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmNhapDMNL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        int sizeList = -1;
        List<int> lstRowUpdate = new List<int>();
        List<DinhMucNPLEntity> lstDMNPLEntity = new List<DinhMucNPLEntity>();
        List<DinhMucNPLEntity> lstDMNPLTempEntity = new List<DinhMucNPLEntity>();
        string maDonHang;
        string maHang;
        int option = -1; // 1 là Cấp phát, 2 là Cấp thêm, 3 là Thu hồi
        bool indicatorIcon = true;
        public frmNhapDMNL(string _maDonHang, string _maHang, int _option)
        {
            // maDonHang = 
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            maDonHang = _maDonHang;
            maHang = _maHang;
            option = _option;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            createSearchLookUp();
            LoadDSDMNPL();
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.N:
                    if (e.Control)
                    {
                        // Tổ hợp phím Ctrl + N
                        ThemRow();
                    }
                    break;
                case Keys.E:
                    if (e.Control)
                    {
                        // Tổ hợp phím Ctrl + E
                        SuaRow();
                    }
                    break;
                case Keys.Delete:
                    // Phím Delete
                    Xoadulieu();
                    break;
                case Keys.S:
                    if ( e.Control)
                    {
                        // Tổ hợp phím Ctrl + S
                        Luudulieu();
                    }
                    break;
                case Keys.F5:
                    NapLaiDuLieu();
                    break;
                case Keys.P:
                    if (e.Control)
                    {
                        // Tổ hợp phím Ctrl + P
                        XuatExCel();
                    }
                    break;
            }
        }

        private void gridViewNhapDMNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }
        private void gridControlNhapDMNL_ProcessGridKey(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    ThemRow();
                    break;
                case Keys.F2:
                    SuaRow();
                    break;
                case Keys.F3:
                    Xoadulieu();
                    break;
                case Keys.F4:
                    Luudulieu();
                    break;
                case Keys.F5:
                    NapLaiDuLieu();
                    break;
                default:
                    break;
            }
        }


        private void createSearchLookUp()
        {
            string urlNPL = string.Format("{0}NguyenPL/GetNPL", URL);
            string jsonNPL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNPL); }).Result;
            SysData.DataTable tblNPL = JsonConvert.DeserializeObject<SysData.DataTable>(jsonNPL);
            this.searchLookUpEditNPL.DataSource = tblNPL;



            // create searchLookUp Đơn vị
            string urlDonVi = string.Format("{0}?", URL + "DonVi/GetDonVi");
            string jsonDonVi = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDonVi); }).Result;
            SysData.DataTable tblDonVi = JsonConvert.DeserializeObject<SysData.DataTable>(jsonDonVi);
            this.searchLookUpEditMaDonVi.DataSource = tblDonVi;

            // Màu
           this.repositoryItemSearchLookUpEdit1.DataSource = tblNPL;
        }

        // Hàm focused dùng để ràng buộc focused vào cell nào và không được focused vào cell nào
        private void focused()
        {

            DinhMucNPLEntity nplEntity = gridViewNhapDMNL.GetFocusedRow() as DinhMucNPLEntity;

            switch (option)
            {
                // 1: Cấp phát
                case 1:
                    if (_status == ResourceURL.EventStatus.Add)
                    {
                        if ((gridViewNhapDMNL.FocusedRowHandle < sizeList) || gridViewNhapDMNL.FocusedColumn == this.colMaVT || gridViewNhapDMNL.FocusedColumn == this.colTenVT || gridViewNhapDMNL.FocusedColumn == this.colMaMau || gridViewNhapDMNL.FocusedColumn == this.colCapThem || gridViewNhapDMNL.FocusedColumn == this.colThuHoi)
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = true;
                        }
                    }
                    else
                    {
                        if (_status == ResourceURL.EventStatus.Edit)
                        {
                            if (gridViewNhapDMNL.FocusedColumn == this.colMaNPL || gridViewNhapDMNL.FocusedColumn == this.colMaVT || gridViewNhapDMNL.FocusedColumn == this.colTenVT || gridViewNhapDMNL.FocusedColumn == this.colMaMau || gridViewNhapDMNL.FocusedColumn == this.colCapThem || gridViewNhapDMNL.FocusedColumn == this.colThuHoi)
                                gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = false;
                            else
                                gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = true;

                            if (nplEntity.TrangThai == 1 && (gridViewNhapDMNL.FocusedColumn == this.colDinhMuc || gridViewNhapDMNL.FocusedColumn == this.colSoLuong || gridViewNhapDMNL.FocusedColumn == this.colCapPhat || gridViewNhapDMNL.FocusedColumn == this.colCapThem || gridViewNhapDMNL.FocusedColumn == this.colThuHoi))
                            {
                                gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = false;
                            }
                        }
                    }
                    break;
                // 2: Cấp thêm
                case 2:
                    if (_status == ResourceURL.EventStatus.Add)
                    {
                        if ((gridViewNhapDMNL.FocusedRowHandle < sizeList) || gridViewNhapDMNL.FocusedColumn == this.colMaVT || gridViewNhapDMNL.FocusedColumn == this.colTenVT || gridViewNhapDMNL.FocusedColumn == this.colMaMau || gridViewNhapDMNL.FocusedColumn == this.colCapPhat || gridViewNhapDMNL.FocusedColumn == this.colThuHoi)
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = true;
                        }
                    }
                    else if (_status == ResourceURL.EventStatus.Edit)
                    {
                        if (gridViewNhapDMNL.FocusedColumn == this.colCapThem)
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = true;
                        }
                        else
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = false;
                        }
                        //if (nplEntity.TrangThai == 1 && (view.FocusedColumn == this.colDinhMuc || view.FocusedColumn == this.colSoLuong || view.FocusedColumn == this.colCapPhat || view.FocusedColumn == this.colCapThem || view.FocusedColumn == this.colThuHoi))
                        //{
                        //    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                        //}
                    }
                    break;

                    // Thu hồi
                case 3:
                    if (_status == ResourceURL.EventStatus.Add)
                    {
                        if ((gridViewNhapDMNL.FocusedRowHandle < sizeList) || gridViewNhapDMNL.FocusedColumn == this.colMaVT || gridViewNhapDMNL.FocusedColumn == this.colTenVT || gridViewNhapDMNL.FocusedColumn == this.colMaMau || gridViewNhapDMNL.FocusedColumn == this.colCapPhat || gridViewNhapDMNL.FocusedColumn == this.colCapThem)
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = true;
                        }
                    }
                    else if (_status == ResourceURL.EventStatus.Edit)
                    {
                        if (gridViewNhapDMNL.FocusedColumn == this.colThuHoi)
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = true;
                        }
                        else
                        {
                            gridViewNhapDMNL.FocusedColumn.OptionsColumn.AllowEdit = false;
                        }
                        //if (nplEntity.TrangThai == 1 && (view.FocusedColumn == this.colDinhMuc || view.FocusedColumn == this.colSoLuong || view.FocusedColumn == this.colCapPhat || view.FocusedColumn == this.colCapThem || view.FocusedColumn == this.colThuHoi))
                        //{
                        //    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                        //}
                    }
                    break;
            }

            //if (_status == ResourceURL.EventStatus.Edit && nplEntity.TrangThai == 1 && (view.FocusedColumn == this.colDinhMuc || view.FocusedColumn == this.colSoLuong || view.FocusedColumn == this.colCapPhat || view.FocusedColumn == this.colCapThem || view.FocusedColumn == this.colThuHoi))
            //{
            //    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            //}
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridViewNhapDMNL.OptionsBehavior.Editable = false;
                    Luu.Enabled = false;
                    Them.Enabled = true;
                    Sua.Enabled = true;
                    break;
                case ResourceURL.EventStatus.Edit:
                    gridViewNhapDMNL.OptionsBehavior.Editable = true;
                    Them.Enabled = false;
                    Sua.Enabled = false;
                    Luu.Enabled = true;
                    Xoa.Enabled = false;
                    break;
                case ResourceURL.EventStatus.Add:
                    gridViewNhapDMNL.OptionsBehavior.Editable = true;
                    Them.Enabled = false;
                    Sua.Enabled = false;
                    Luu.Enabled = true;
                    Xoa.Enabled = false;
                    break;
            }
        }

        private void LoadDSDMNPL()
        {
            try
            {
                string url = string.Format("{0}?maDH={1}", URL + "DinhMucNPL/GetDMNPL", maDonHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                lstDMNPLTempEntity = JsonConvert.DeserializeObject<List<DinhMucNPLEntity>>(json);
                lstDMNPLEntity = JsonConvert.DeserializeObject<List<DinhMucNPLEntity>>(json);

                gridControlNhapDMNL.DataSource = lstDMNPLEntity;

                sizeList = lstDMNPLEntity.Count;



                //DinhMucNPLEntity obj = new DinhMucNPLEntity();
                //lstDMNPLEntity.Add(obj);


                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                if (lstDMNPLEntity!=null&&lstDMNPLEntity.Count>0&&lstDMNPLEntity[0].TrangThai == 1)
                {
                    //gridViewNhapDMNL.OptionsBehavior.Editable = false;
                    Them.Enabled = false;
                    Xoa.Enabled = false;
                    Sua.Enabled = false;
                    Luu.Enabled = false;
                }
                focused();
            }
            catch (Exception e)
            {
                Console.WriteLine("exception: " + e);
            }
        }

        private void searchLookUpEditNPL_EditValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("EditValueChanged");
            ChangingEventArgs args = e as ChangingEventArgs;
            int indexFocused = gridViewNhapDMNL.FocusedRowHandle;
            if (args.NewValue != null)
            {
                string maNPL = args.NewValue.ToString();
                string maVT, tenVT, mau = string.Empty;
                SysData.DataTable tblSearchLookUpNPL = (sender as SearchLookUpEdit).Properties.DataSource as SysData.DataTable;

                foreach (SysData.DataRow row in tblSearchLookUpNPL.Rows)
                {
                    if (maNPL.Equals(row["MaNPL"]))
                    {
                        if ((gridViewNhapDMNL.FocusedRowHandle == lstDMNPLEntity.Count - 1) && (_status == ResourceURL.EventStatus.Add))
                        {
                            ThemRow();
                        }
                        maVT = row["MaVatTu"].ToString();
                        tenVT = row["TenNPL"].ToString();
                        mau = row["MaMauNPL"].ToString();
                        gridViewNhapDMNL.SetRowCellValue(indexFocused, this.colMaVT, maVT);
                        gridViewNhapDMNL.SetRowCellValue(indexFocused, this.colTenVT, tenVT);
                        gridViewNhapDMNL.SetRowCellValue(indexFocused, this.colMaMau, mau);
                        break;
                    }
                }
            }
            else
            {
                gridViewNhapDMNL.SetRowCellValue(indexFocused, this.colMaVT, null);
                gridViewNhapDMNL.SetRowCellValue(indexFocused, this.colTenVT, null);
                gridViewNhapDMNL.SetRowCellValue(indexFocused, this.colMaMau, null);
            }

        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemRow();
        }

        private void ThemRow()
        {
            DinhMucNPLEntity obj = new DinhMucNPLEntity();
            lstDMNPLEntity.Add(obj);

            gridViewNhapDMNL.RefreshData();

            //_rowAdd = gridViewNhapDMNL.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            if ((lstDMNPLEntity.Count - 1) == sizeList)
            {
                gridViewNhapDMNL.FocusedRowHandle = lstDMNPLEntity.Count - 1;
            }
            focused();
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaRow();
        }

        private void SuaRow()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Xoadulieu();
        }
        private async void Xoadulieu()
        {
            winForm.DialogResult messResult = winForm.MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", winForm.MessageBoxButtons.YesNo, winForm.MessageBoxIcon.Question);
            if (messResult == winForm.DialogResult.Yes)
            {
                DinhMucNPLEntity row = gridViewNhapDMNL.GetRow(gridViewNhapDMNL.FocusedRowHandle) as DinhMucNPLEntity;
                string url = string.Format("{0}?id={1}", URL + "DinhMucNPL/DeleteDMNPL", row.ID);
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                    LoadDSDMNPL();
                else XtraMessageBox.Show(result);
            }
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Luudulieu();
        }
        private void Luudulieu()
        {
            List<DinhMucNPLEntity> _lstUpdate = new List<DinhMucNPLEntity>();
            this.ActiveControl = this.button1;
            DinhMucNPLEntity row = gridViewNhapDMNL.GetRow(gridViewNhapDMNL.FocusedRowHandle) as DinhMucNPLEntity;
            lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
            for (int i = 0; i < lstRowUpdate.Count; i++)
            {
                DinhMucNPLEntity item = gridViewNhapDMNL.GetRow(lstRowUpdate[i]) as DinhMucNPLEntity;
                if (item != null)
                {
                    // Check_error. Nguyên phụ liệu
                    if (string.IsNullOrEmpty(item.MaNPL))
                    {
                        XtraMessageBox.Show("Nguyên phụ liệu không được để trống!", Resources.Warning, winForm.MessageBoxButtons.OK, winForm.MessageBoxIcon.Warning, winForm.MessageBoxDefaultButton.Button1);
                        return;
                    }

                    // Check_error. Khổ vải
                    if (string.IsNullOrEmpty(item.KhoVai))
                    {
                        XtraMessageBox.Show("Khổ vải không được để trống!", Resources.Warning, winForm.MessageBoxButtons.OK, winForm.MessageBoxIcon.Warning, winForm.MessageBoxDefaultButton.Button1);
                        return;
                    }

                    // Check_error. Đơn vị
                    if (string.IsNullOrEmpty(item.MaDV))
                    {
                        XtraMessageBox.Show("Đơn vị không được để trống!", Resources.Warning, winForm.MessageBoxButtons.OK, winForm.MessageBoxIcon.Warning, winForm.MessageBoxDefaultButton.Button1);
                        return;
                    }

                    // Check_error. Cấp phát
                    if (option == 1 && item.CapPhat <= 0 && item.DinhMuc <= 0)
                    {
                        XtraMessageBox.Show("Thực hiện cấp phát lỗi!\nCột cấp phát và định mức không được trống.", Resources.Warning, winForm.MessageBoxButtons.OK, winForm.MessageBoxIcon.Warning, winForm.MessageBoxDefaultButton.Button1);
                        return;
                    }

                    // Check_error. Cấp thêm
                    if (option == 2 && row.CapThem <= 0)
                    {
                        XtraMessageBox.Show("Thực hiện cấp thêm lỗi!\nCột cấp thêm không được để trống.", Resources.Warning, winForm.MessageBoxButtons.OK, winForm.MessageBoxIcon.Warning, winForm.MessageBoxDefaultButton.Button1);
                        return;
                    }

                    // Check_error. Thu hồi
                    if (option == 3 && row.ThuHoi <= 0)
                    {
                        XtraMessageBox.Show("Thực hiện Thu hồi lỗi!\nCột thu hồi không được để trống.", Resources.Warning, winForm.MessageBoxButtons.OK, winForm.MessageBoxIcon.Warning, winForm.MessageBoxDefaultButton.Button1);
                        return;
                    }
                    //
                    item.MaDH = maDonHang;
                    item.TrangThai = 0;
                    // Gán trạng thái mặc định bằng 0 khi thêm vào.
                    // Nếu trạng thái == 1 => Không cho chỉnh sửa: Định mức, Số lượng, Cấp phát.
                    // trạng thái != 1 => Cho chỉnh sửa: Định mức, Số lượng, Cấp phát.
                    _lstUpdate.Add(item);
                }
            }
            string msResult = "";
            if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
            {
                return;
            }
            string url = string.Format("{0}", URL + "DinhMucNPL/PostDMNPL");

            SysData.DataTable tblSave = JsonConvert.DeserializeObject<SysData.DataTable>(JsonConvert.SerializeObject(_lstUpdate));

            SysData.DataTable tbl = XoaCotTenMau(tblSave);
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
            if (msResult.ToLower() == "true")
                LoadDSDMNPL();
            else XtraMessageBox.Show(msResult);
            //ThemRow();
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            lstRowUpdate.Clear();
        }

        private SysData.DataTable XoaCotTenMau(SysData.DataTable tbl)
        {
            foreach (SysData.DataColumn column in tbl.Columns)
            {
                if (column.ColumnName.Contains("TenMau"))
                {
                    tbl.Columns.Remove(column);
                    break;
                }
            }
            return tbl;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDuLieu();
        }

        private void NapLaiDuLieu()
        {
            LoadDSDMNPL();
            //ThemRow();
        }
        private void XuatExCel()
        {
            winForm.SaveFileDialog Sfd = new winForm.SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "(.xls)|*.xls";
            //Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            string fileName = string.Empty;
            //Sfd.FileName = string.Format("PhieuCapPhatNguyenLieu_{0}", DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy")));
            Sfd.FileName = string.Format("PhieuCapPhatNguyenLieu_{0}_{1}", string.Format("{0:dd-MM-yyyy}", DateTime.Now), DateTime.Now.Millisecond);

            if (Sfd.ShowDialog() == winForm.DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");

                // Đường dẫn file template
                fileName = "TheKho.xlsx";

                string path = Path.Combine(winForm.Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                SysData.DataTable _dtReportMain = new SysData.DataTable();
                try
                {
                    gridViewNhapDMNL.ExportToXls(ExportFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ex: " + ex);
                }

                //ExportExcel(TemplateFileName, ExportFileName, lstDMNPLEntity);

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Mở File", "Xác nhận", winForm.MessageBoxButtons.YesNo, winForm.MessageBoxIcon.Question) == winForm.DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(Sfd.FileName))
                            System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                    catch
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Error..");
                    }
                }

            }
        }
        private void btExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XuatExCel();
        }

        private void ExportExcel(string templateFileName, string exportFileName, List<DinhMucNPLEntity> lstDMNPL)
        {
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            Microsoft.Office.Interop.Excel.Sheets sheets;
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
            //int step = 0;

            // Create excel application
            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                if (templateFileName != string.Empty)
                {

                    // Load the work book
                    workbook = ExcelApp.Workbooks.Open(templateFileName, 0, false, 5, "", "", false,
                        Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);

                    sheets = workbook.Sheets;
                    worksheet = sheets["sheet1"];

                    int rowCount = 12;
                    double tongCapPhat = 0;

                    int tongSoLuong = 0;

                    int soTT = 0;
                    // Gán data cột mã hàng
                    Range rangeMH = worksheet.Cells[6, 3];
                    rangeMH.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    rangeMH.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    rangeMH.Font.Size = 11;
                    worksheet.Cells[6, 3] = maHang;


                    // Thêm các dòng dữ liệu vào excel
                    foreach (DinhMucNPLEntity itemDMNPL in lstDMNPL)
                    {
                        // 13;1
                        rowCount += 1;
                        soTT += 1;

                        tongCapPhat += itemDMNPL.CapPhat;
                        tongSoLuong += itemDMNPL.SoLuong;

                        //cell.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        //cell.Borders.Weight = Excel.XlBorderWeight.xlThin;

                        AddRowExcel(worksheet, rowCount, soTT, itemDMNPL);

                    }
                    // Cột tổng số lượng PCS
                    Range rangeSLPCS = worksheet.Cells[7, 9];
                    rangeSLPCS.HorizontalAlignment = XlHAlign.xlHAlignRight;
                    rangeSLPCS.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    rangeSLPCS.Font.Size = 11;
                    worksheet.Cells[7, 9] = tongSoLuong;

                    // Cột tổng cấp phát PCS
                    Range rangeCPPCS = worksheet.Cells[8, 9];
                    rangeCPPCS.HorizontalAlignment = XlHAlign.xlHAlignRight;
                    rangeCPPCS.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    rangeCPPCS.Font.Size = 11;
                    worksheet.Cells[8, 9] = tongCapPhat;

                    //

                    rowCount += 1;
                    AddRowExcel(worksheet, rowCount, 0, null);

                    rowCount += 1;

                    DinhMucNPLEntity itemDMNPLTong = new DinhMucNPLEntity();
                    itemDMNPLTong.TenVT = "Tổng cộng";
                    itemDMNPLTong.CapPhat = tongCapPhat;

                    AddRowExcel(worksheet, rowCount, 0, itemDMNPLTong);

                    //
                    rowCount += 1;
                    Range mergeRange1 = worksheet.Range[worksheet.Cells[rowCount, 2], worksheet.Cells[rowCount, 3]];
                    mergeRange1.Merge();
                    worksheet.Cells[rowCount, 2] = "(Yêu cầu đối chiếu bảng màu khi cấp phát)";
                    mergeRange1.Font.Bold = true;
                    mergeRange1.Font.Size = 12;
                    mergeRange1.Font.Italic = true;
                    mergeRange1.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    mergeRange1.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    worksheet.Rows[rowCount].RowHeight = 15;


                    //
                    rowCount += 1;
                    Range mergeRange2 = worksheet.Range[worksheet.Cells[rowCount, 2], worksheet.Cells[rowCount, 3]];
                    mergeRange2.Merge();

                    worksheet.Cells[rowCount, 2] = "(See color card when distribute material is required)";
                    mergeRange2.Font.Bold = true;
                    mergeRange2.Font.Size = 12;
                    mergeRange2.Font.Italic = true;
                    mergeRange2.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    mergeRange2.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    worksheet.Rows[rowCount].RowHeight = 15;

                    Range mergeRange3 = worksheet.Range[worksheet.Cells[rowCount, 8], worksheet.Cells[rowCount, 9]];
                    mergeRange3.Merge();

                    worksheet.Cells[rowCount, 8] = string.Format("Ngày {0} tháng {1} năm {2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    mergeRange3.Font.Size = 12;
                    mergeRange3.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    mergeRange3.VerticalAlignment = XlVAlign.xlVAlignCenter;

                    //
                    rowCount += 1;
                    Range mergeRange4 = worksheet.Range[worksheet.Cells[rowCount, 2], worksheet.Cells[rowCount, 4]];
                    mergeRange4.Merge();
                    worksheet.Cells[rowCount, 2] = "Người lập/Prepared By";
                    mergeRange4.Font.Size = 12;
                    mergeRange4.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    mergeRange4.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    worksheet.Rows[rowCount].RowHeight = 15;

                    Range mergeRange5 = worksheet.Range[worksheet.Cells[rowCount, 7], worksheet.Cells[rowCount, 9]];
                    mergeRange5.Merge();

                    worksheet.Cells[rowCount, 7] = "PHÒNG KH - KD";
                    mergeRange5.Font.Bold = true;
                    mergeRange5.Font.Size = 12;
                    mergeRange5.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    mergeRange5.VerticalAlignment = XlVAlign.xlVAlignCenter;


                }

                // Save the workbook with saving option
                workbook.Close(true, exportFileName, Type.Missing);
                //int percentage = step * 100 / LineCollection.Rows.Count;
                //worker.ReportProgress(percentage);
                ExcelApp.Workbooks.Close();
                ExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);
            }
            catch (Exception ex)
            {
                object misValue = System.Reflection.Missing.Value;

                if (workbook == null) return;
                workbook.Close(false, misValue, misValue);
                ExcelApp.Workbooks.Close();
                ExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);
                throw new Exception(ex.Message);
            }

        }

        private void AddRowExcel(Worksheet worksheet, int rowCount, int soTT, DinhMucNPLEntity itemDMNPL)
        {
            Range rangeSTT = worksheet.Cells[rowCount, 1];
            rangeSTT.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            rangeSTT.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeSTT.Font.Size = 12;
            rangeSTT.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeSTT.Borders.Weight = XlBorderWeight.xlThin;

            if (soTT > 0)
            {
                worksheet.Cells[rowCount, 1] = soTT;
            }

            // Mã vật tư
            Range rangeMVT = worksheet.Cells[rowCount, 2];

            rangeMVT.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            rangeMVT.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeMVT.Font.Size = 9;
            rangeMVT.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeMVT.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.MaVT != null)
            {
                worksheet.Cells[rowCount, 2] = itemDMNPL.MaVT;
            }

            // Tên vật tư
            Range rangeTVT = worksheet.Cells[rowCount, 3];

            rangeTVT.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeTVT.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.TenVT != null)
            {
                worksheet.Cells[rowCount, 3] = itemDMNPL.TenVT;
            }

            if (itemDMNPL != null && itemDMNPL.TenVT != null && itemDMNPL.MaVT == null)
            {
                rangeTVT.Font.Bold = true;
                rangeTVT.Font.Size = 10;
                rangeTVT.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                rangeTVT.VerticalAlignment = XlVAlign.xlVAlignCenter;
            }
            else
            {
                rangeTVT.Font.Bold = false;
                rangeTVT.Font.Size = 9;
                rangeTVT.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                rangeTVT.VerticalAlignment = XlVAlign.xlVAlignBottom;
            }

            // Mã màu
            Range rangeMaMau = worksheet.Cells[rowCount, 4];

            rangeMaMau.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            rangeMaMau.VerticalAlignment = XlVAlign.xlVAlignCenter;
            rangeMaMau.Font.Size = 9;
            rangeMaMau.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeMaMau.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.MaMau != null)
            {
                worksheet.Cells[rowCount, 4] = itemDMNPL.MaMau;
            }

            // Khổ vải
            Range rangeKhoVai = worksheet.Cells[rowCount, 5];

            rangeKhoVai.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            rangeKhoVai.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeKhoVai.Font.Size = 12;
            rangeKhoVai.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeKhoVai.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.KhoVai != null)
            {
                worksheet.Cells[rowCount, 5] = itemDMNPL.KhoVai;
            }

            // Đơn vị
            Range rangeDonVi = worksheet.Cells[rowCount, 6];

            rangeDonVi.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            rangeDonVi.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeDonVi.Font.Size = 12;
            rangeDonVi.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeDonVi.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.MaDV != null)
            {
                worksheet.Cells[rowCount, 6] = itemDMNPL.MaDV;
            }

            // Định mức
            Range rangeDinhMuc = worksheet.Cells[rowCount, 7];

            rangeDinhMuc.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            rangeDinhMuc.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeDinhMuc.Font.Size = 12;
            rangeDinhMuc.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeDinhMuc.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.DinhMuc != null && itemDMNPL.DinhMuc != 0)
            {
                worksheet.Cells[rowCount, 7] = itemDMNPL.DinhMuc;
            }

            // Số lượng
            Range rangeSoLuong = worksheet.Cells[rowCount, 8];

            rangeSoLuong.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            rangeSoLuong.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeSoLuong.Font.Size = 12;
            rangeSoLuong.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeSoLuong.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.SoLuong != null && itemDMNPL.SoLuong != 0)
            {
                worksheet.Cells[rowCount, 8] = itemDMNPL.SoLuong;
            }

            // Cấp phát
            Range rangeCapPhat = worksheet.Cells[rowCount, 9];

            rangeCapPhat.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            rangeCapPhat.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeCapPhat.Font.Size = 12;
            rangeCapPhat.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeCapPhat.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL == null || (itemDMNPL != null && itemDMNPL.MaVT == null))
            {
                worksheet.Rows[rowCount].RowHeight = 15;
            }

            if (itemDMNPL != null && itemDMNPL.CapPhat != null)
            {
                worksheet.Cells[rowCount, 9] = itemDMNPL.CapPhat;
            }

            if (itemDMNPL != null && itemDMNPL.CapPhat != null && itemDMNPL.CapPhat != 0 && itemDMNPL.MaVT == null)
            {
                rangeCapPhat.Font.Bold = true;
            }
            else
            {
                rangeCapPhat.Font.Bold = false;
            }

            // Ghi chú
            Range rangeGhiChu = worksheet.Cells[rowCount, 10];

            rangeGhiChu.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            rangeGhiChu.VerticalAlignment = XlVAlign.xlVAlignBottom;
            rangeGhiChu.Font.Size = 12;
            rangeGhiChu.Borders.LineStyle = XlLineStyle.xlContinuous;
            rangeGhiChu.Borders.Weight = XlBorderWeight.xlThin;

            if (itemDMNPL != null && itemDMNPL.GhiChu != null)
            {
                worksheet.Cells[rowCount, 10] = itemDMNPL.GhiChu;
            }

        }

        private void gridViewNhapDMNL_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            //focused(sender);
        }
        private void gridViewNhapDMNL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused();
        }
        private void gridViewNhapDMNL_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused();
        }

        // Fires when an in-place editor is active
        private void gridControlNhapDMNL_EditorKeyPress(object sender, winForm.KeyPressEventArgs e)
        {
            // comment lại tạm thời
            GridView view = gridViewNhapDMNL as GridView;
            //GridView view = grid.FocusedView as GridView;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            if (view.FocusedColumn == colMaVT)
            {
                e.Handled = CheckCharacterID(view, e);
            }

            Console.WriteLine("KeyPress: " + e.KeyChar);


            if (view.FocusedColumn == colMaVT)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }
        private bool CheckCharacterID(GridView view, winForm.KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Check ID
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu không phải là kí tự đặc biệt -> Cho nhập
                // Nếu không phải là dấu câu -> Cho nhập
                if ((!char.IsWhiteSpace(e.KeyChar) && !char.IsSymbol(e.KeyChar) && !char.IsPunctuation(e.KeyChar)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private void gridViewNhapDMNL_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView gridView = sender as GridView;
            bool flagDinhMuc = true;
            if (gridViewNhapDMNL.GetFocusedDataSourceRowIndex() >= 0 && e.Value != null)
            {
                // validate_1. Check dữ liệu khổ vải không chứa kí tự nháy đôi
                if (gridViewNhapDMNL.FocusedColumn == this.colKhoVai)
                {
                    if (e.Value.ToString().Contains("\""))
                    {
                        e.Valid = false;
                        e.ErrorText = "Khổ vải không được chứa kí tự nháy đôi.!";
                    }
                }

                // validate_2. Check dữ liệu các cột số lượng không được nhập chữ, kí tự
                else if (gridViewNhapDMNL.FocusedColumn == this.colDinhMuc || gridViewNhapDMNL.FocusedColumn == this.colSoLuong ||
                   gridViewNhapDMNL.FocusedColumn == this.colCapPhat || gridViewNhapDMNL.FocusedColumn == this.colCapThem || gridViewNhapDMNL.FocusedColumn == this.colThuHoi)
                {

                    int soLuong = 0;
                    bool flag = int.TryParse(e.Value.ToString(), out soLuong);
                    if (!flag)
                    {
                        string errorText = "Vui lòng nhập số.!";
                        showValidError(e, errorText);
                        flagDinhMuc = false;
                    }
                }

                // validate_3. Check dữ liệu cột MaNPL và cột DinhMuc không được đồng thời trùng nhau với dữ liệu đã có sẵn
                if (flagDinhMuc && (gridViewNhapDMNL.FocusedColumn == this.colMaNPL || gridViewNhapDMNL.FocusedColumn == this.colDinhMuc))
                {
                    DinhMucNPLEntity nplEntity = (sender as GridView).GetFocusedRow() as DinhMucNPLEntity;

                    List<DinhMucNPLEntity> lstDinhMucNPL = (sender as GridView).DataSource as List<DinhMucNPLEntity>;

                    foreach (DinhMucNPLEntity npl in lstDinhMucNPL)
                    {
                        if (npl.MaNPL != null)
                        {
                            if (gridViewNhapDMNL.FocusedColumn == this.colMaNPL)
                            {
                                if (npl.MaNPL.Equals(e.Value.ToString()) && npl.DinhMuc == nplEntity.DinhMuc)
                                {
                                    string errorText = string.Format("Vật tư {0} đã được khai báo định mức {1}.\nVui lòng nhập định mức khác.!", npl.TenVT, npl.DinhMuc);
                                    showValidError(e, errorText);
                                    break;
                                }
                            }
                            else if (gridViewNhapDMNL.FocusedColumn == this.colDinhMuc)
                            {
                                if (npl.MaNPL.Equals(nplEntity.MaNPL) && npl.DinhMuc == int.Parse(e.Value.ToString()))
                                {
                                    string errorText = string.Format("Vật tư {0} đã được khai báo định mức {1}.\nVui lòng nhập định mức khác.!", npl.TenVT, npl.DinhMuc);
                                    showValidError(e, errorText);
                                    break;
                                }
                            }
                        }
                    }
                }

                // validate_4. Cộng dồn giá trị vào cột cấp thêm
                if (gridViewNhapDMNL.FocusedColumn == this.colCapThem && gridView.FocusedRowHandle < lstDMNPLTempEntity.Count)
                {
                    int focusRow = -1;
                    bool flagParse = int.TryParse(e.Value.ToString(), out focusRow);
                    if (flagParse)
                    {
                        e.Value = lstDMNPLTempEntity[gridView.FocusedRowHandle].CapThem + int.Parse(e.Value.ToString());
                    }
                }
                if (e.Valid == false)
                    Luu.Enabled = false;
                else Luu.Enabled = true;
            }
        }

        private void showValidError(DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e, string errorText)
        {
            e.Valid = false;
            e.ErrorText = errorText;
        }

        private void gridViewNhapDMNL_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            SysDraw.Rectangle rect = e.Bounds;
            winForm.ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            SysDraw.Brush brush =
                e.Cache.GetGradientBrush(rect, SysDraw.Color.FromArgb(255, 212, 128), SysDraw.Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridViewNhapDMNL_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridViewNhapDMNL_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}