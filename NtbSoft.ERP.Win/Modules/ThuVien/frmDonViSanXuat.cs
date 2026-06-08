using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.THIETBI;
using NtbSoft.ERP.Win.Modules.ResourceForm;
using NtbSoft.ERP.Win.Properties;
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
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Controls;
using System.Data.SqlClient;
using DevExpress.XtraGrid.Columns;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmDonViSanXuat : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;

        private ResourceURL.EventStatus _statusKho;
        private KhoModel _khoModel;

        int _rowAdd = -1;
        string CheckStatus = "0"; //0 là Thêm, 1 là sửa, 2 là view dữ liệu
        int _rowFocus = 0;
        string _MaDVSX = "";
        //List<DonViSanXuatEntity> lstDonViSanXuat;
        //DataTable tbl;
        DataTable tbl_DVSX_BanDau;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;

        List<int> lstRowUpdate = new List<int>();
        List<DonViSanXuatEntity> lstDonViSanXuatEntity;
        List<KhoEntity> lstKhoEntity;
        bool indicatorIcon = true;
        int FocusedIndex = 0;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        public frmDonViSanXuat()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDonViSanXuatEntity = new List<DonViSanXuatEntity>();
            #region QLKho Thoai
            _khoModel = new KhoModel();
            lstKhoEntity = new List<KhoEntity>();
            this.gridDonViSanXuat.Enter += new System.EventHandler(this.Grids_Enter);
            this.grcQLKho.Enter += new System.EventHandler(this.Grids_Enter);
            this.gridDonViSanXuat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Grid_KeyDown_Handler);
            this.grcQLKho.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Grid_KeyDown_Handler);
            #endregion QLKho Thoai
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(BtThem, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(BtSua, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(Xoadulieu, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(Luudulieu, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            CheckPermission();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSDonViSanXuat(false);
        }
        private void CheckPermission()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
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
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
            }
            if (_allowAdd || _allowEdit)
            {
                Luu.Enabled = true;
            }
            else
            {
                Luu.Enabled = false;
            }

            if (!_allowDelete)
                Xoa.Enabled = false;

        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridViewDonViSanXuat.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                case ResourceURL.EventStatus.Add:
                    gridViewDonViSanXuat.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                    //case ResourceURL.EventStatus.Add:
                    //    gridViewDonViSanXuat.OptionsBehavior.Editable = true;
                    //    if (_allowAdd)
                    //    {
                    //        Them.Enabled = false;
                    //        actionControlAdd.Enabled = false;
                    //    }

                    //    if (_allowEdit)
                    //    {
                    //        Sua.Enabled = false;
                    //        actionControlEdit.Enabled = false;
                    //    }
                    //    if (_allowDelete)
                    //    {
                    //        Xoa.Enabled = false;
                    //        actionControlDelete.Enabled = false;
                    //    }
                    //    Sua.Enabled = false;
                    //    if (_allowAdd || _allowEdit)
                    //    {
                    //        Luu.Enabled = true;
                    //        actionControlSave.Enabled = true;
                    //    }

                    //    break;
            }
        }
        #region QLKho Thoai
        private void KhoGridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            _statusKho = status;
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    btnThemKho.Enabled = true;
                    //btnSuaQLKho.Enabled = true;
                    btnXoaKho.Enabled = true;
                    //btnLuuKho.Enabled = false;
                    gridViewDSKho.OptionsBehavior.Editable = false;
                    break;
                case ResourceURL.EventStatus.Edit:
                case ResourceURL.EventStatus.Add:
                    gridViewDSKho.OptionsBehavior.Editable = true;
                    btnThemKho.Enabled = false;
                    //btnSuaQLKho.Enabled = false;
                    btnXoaKho.Enabled = false;
                    btnLuuKho.Enabled = true;
                    break;
            }
        }
        #endregion QLKho Thoai
        private void LoadDSDonViSanXuat(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (!string.IsNullOrEmpty(json))
                {
                    lstDonViSanXuatEntity = JsonConvert.DeserializeObject<List<DonViSanXuatEntity>>(json);
                }

                gridDonViSanXuat.DataSource = lstDonViSanXuatEntity;
                tbl_DVSX_BanDau = JsonConvert.DeserializeObject<DataTable>(json);

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewDonViSanXuat.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridDonViSanXuat;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadKho(string maDVSX)
        {
            string url = string.Format("{0}", URL + $"DonViSanXuat/GetCaiDatKho?action=Get&para1={maDVSX}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dtKho = JsonConvert.DeserializeObject<DataTable>(json);
            grcKho.DataSource = dtKho;
        }

        private void BtThem()
        {
            int itemMaxSort = lstDonViSanXuatEntity.Max(x => x.Sort);
            DonViSanXuatEntity obj = new DonViSanXuatEntity(itemMaxSort + 1);
            //_bindingHangHoaEntity.Add(obj);
            lstDonViSanXuatEntity.Add(obj);

            _rowAdd = gridViewDonViSanXuat.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridViewDonViSanXuat.FocusedRowHandle = _rowAdd;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtThem();
        }

        private void BtSua()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtSua();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Xoadulieu();
        }
        private async void Xoadulieu()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    DonViSanXuatEntity row = gridViewDonViSanXuat.GetRow(gridViewDonViSanXuat.FocusedRowHandle) as DonViSanXuatEntity;
                    string url = string.Format("{0}?Parameter={1}", URL + "DonViSanXuat/DeleteDonViSanXuat", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadDSDonViSanXuat(false);
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Luudulieu();
        }
        private string CheckDuplicateDataUpdate(string colName, string value, DonViSanXuatEntity itemUpdate, int currentIndex)
        {
            //List<DonViSanXuatEntity> lstDataSource = gridViewDonViSanXuat.DataSource as List<DonViSanXuatEntity>;
            //for (int i = 0; i < lstDataSource.Count; i++)
            //{
            //    // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không Update
            //    switch (colName)
            //    {
            //        case null:
            //            if (currentIndex != i && (itemUpdate.Sort.Equals(lstDataSource[i].Sort)))
            //            {
            //                return "Số thứ tự đã bị trùng. Vui lòng nhập lại thông tin!";
            //            }
            //            break;
            //    }
            //}
            return "";
        }

        private void Luudulieu()
        {
            try
            {
                this.ActiveControl = button1;

                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewDonViSanXuat.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewDonViSanXuat.FocusedRowHandle;
                }
                List<DonViSanXuatEntity> _lstUpdate = new List<DonViSanXuatEntity>();
                //this.ActiveControl = this.button1;
                DonViSanXuatEntity row = gridViewDonViSanXuat.GetRow(gridViewDonViSanXuat.FocusedRowHandle) as DonViSanXuatEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }

                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                //lstRowUpdate.Sort();

                // check trùng số thứ tự
                DonViSanXuatEntity item;
                switch (_status)
                {
                    case ResourceURL.EventStatus.Edit:

                        for (int i = 0; i < lstRowUpdate.Count; i++)
                        {
                            item = gridViewDonViSanXuat.GetRow(lstRowUpdate[i]) as DonViSanXuatEntity;
                            string resultCheckDuplicate = CheckDuplicateDataUpdate(null, null, item, lstRowUpdate[i]);
                            if (item != null && string.IsNullOrEmpty(resultCheckDuplicate))
                            {
                                _lstUpdate.Add(item);
                            }
                            else
                            {
                                XtraMessageBox.Show(resultCheckDuplicate, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        break;

                    case ResourceURL.EventStatus.Add:
                        item = gridViewDonViSanXuat.GetRow(lstRowUpdate[0]) as DonViSanXuatEntity;
                        if (item != null)
                        {
                            _lstUpdate.Add(item);

                        }
                        break;
                }

                //
                //for (int i = 0; i < lstRowUpdate.Count; i++)
                //{
                //    DonViSanXuatEntity item = gridViewDonViSanXuat.GetRow(lstRowUpdate[i]) as DonViSanXuatEntity;
                //    if (item != null && !string.IsNullOrEmpty(item.TenDVSX))
                //    {
                //        //item.ID = 0;
                //        _lstUpdate.Add(item);
                //    }
                //}
                string msResult = "";

                //if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                //{
                //    return;
                //}

                if (CheckStatus == "1" && (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))) // Update
                {
                    DateTime DateNow = DateTime.Now;
                    DataTable tbl_DVSX_BanDau2 = ConvertListToDataTable(_lstUpdate);
                    DataTable changedRows = GetChangedRows(tbl_DVSX_BanDau, tbl_DVSX_BanDau2, "MaDVSX", "GiaCong");
                    DataTable tblColumn = GetColumns(changedRows);
                    DataColumn colNgayTao = new DataColumn("NgayTao", typeof(DateTime));
                    DataColumn colNguoiTao = new DataColumn("NguoiTao", typeof(string));
                    colNgayTao.DefaultValue = DateNow;
                    colNguoiTao.DefaultValue = GlobleData.UserName;
                    tblColumn.Columns.Add(colNgayTao);
                    tblColumn.Columns.Add(colNguoiTao);
                    if (tblColumn.Rows.Count > 0)
                    {
                        string url1 = string.Format("{0}?", URL + "DonViSanXuat/PostDonViSanXuatDetail");
                        string msResult1 = "";
                        msResult1 = Task.Run(async () => { return await _clientExtension.PostAsync(url1, tblColumn); }).Result;
                    }

                }

                if (CheckStatus == "0") //Add
                {
                    DateTime DateNow = DateTime.Now;
                    DataTable tbl_DVSX_BanDau2 = ConvertListToDataTable(_lstUpdate);
                    DataTable tblColumn = GetColumns(tbl_DVSX_BanDau2);
                    DataColumn colNgayTao = new DataColumn("NgayTao", typeof(DateTime));
                    DataColumn colNguoiTao = new DataColumn("NguoiTao", typeof(string));
                    colNgayTao.DefaultValue = DateNow;
                    colNguoiTao.DefaultValue = GlobleData.UserName;
                    tblColumn.Columns.Add(colNgayTao);
                    tblColumn.Columns.Add(colNguoiTao);

                    if (tblColumn.Rows.Count > 0)
                    {
                        string url1 = string.Format("{0}?", URL + "DonViSanXuat/PostDonViSanXuatDetail");
                        string msResult1 = "";
                        msResult1 = Task.Run(async () => { return await _clientExtension.PostAsync(url1, tblColumn); }).Result;
                    }
                }

                if (_lstUpdate.Count > 0)
                {
                    string url = string.Format("{0}?", URL + "DonViSanXuat/PostDonViSanXuat");
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        LoadDSDonViSanXuat(true);
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else XtraMessageBox.Show(msResult);
                    _status = ResourceURL.EventStatus.View;
                    GridViewUpdateStatus(_status);
                    lstRowUpdate.Clear();
                }
                else { clsWaitForm.ShowSuccessForm(this, 2000); }

            }
            catch (Exception ex)
            {

                XtraMessageBox.Show(" Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //
        private static DataTable GetColumns(DataTable sourceTable)
        {
            // Tạo DataTable mới với cấu trúc giống DataTable gốc
            DataTable destinationTable = sourceTable.Clone();

            // Xóa các cột không mong muốn
            foreach (DataColumn column in sourceTable.Columns.Cast<DataColumn>().ToArray())
            {
                if (column.ColumnName != "MaDVSX" && column.ColumnName != "GiaCong")
                {
                    sourceTable.Columns.Remove(column.ColumnName);
                }
            }

            return sourceTable;
        }

        //
        private static DataTable GetChangedRows(DataTable dataTable1, DataTable dataTable2, string keyColumnName, string changedColumnName)
        {
            // Tạo DataTable mới để lưu trữ các dòng có giá trị thay đổi
            DataTable changedRows = dataTable2.Clone();

            // Duyệt qua từng dòng của DataTable 2
            foreach (DataRow row2 in dataTable2.Rows)
            {
                string maDVSX = row2[keyColumnName].ToString();
                string giaCong2 = row2[changedColumnName].ToString();

                // Tìm dòng tương ứng trong DataTable 1
                DataRow row1 = dataTable1.AsEnumerable().FirstOrDefault(r => r[keyColumnName].ToString() == maDVSX);

                if (row1 != null)
                {
                    string giaCong1 = row1[changedColumnName].ToString();

                    // So sánh giá trị cột "GiaCong" để kiểm tra sự thay đổi
                    if (giaCong1 != giaCong2)
                    {
                        // Nếu có thay đổi, thêm dòng vào DataTable mới
                        changedRows.Rows.Add(row2.ItemArray);
                    }
                }
            }

            return changedRows;
        }

        //Chuyển 
        public static DataTable ConvertListToDataTable(List<DonViSanXuatEntity> list)
        {
            DataTable dataTable = new DataTable();

            // Create columns based on the properties of DonViSanXuatEntity
            foreach (var prop in typeof(DonViSanXuatEntity).GetProperties())
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }

            // Populate rows
            foreach (var entity in list)
            {
                DataRow row = dataTable.NewRow();

                foreach (var prop in typeof(DonViSanXuatEntity).GetProperties())
                {
                    row[prop.Name] = prop.GetValue(entity);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private void NapLaiDong()
        {
            LoadDSDonViSanXuat(false);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            gridViewDonViSanXuat.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            //gridViewDonVi.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void gridViewDonViSanXuat_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }
        private void gridViewDonViSanXuat_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
            //DataRow dr = gridViewDonViSanXuat.GetFocusedDataRow(); // hàm này để trả về row nếu khi chuyền vào DataSource
            DonViSanXuatEntity selectedEntity = (DonViSanXuatEntity)gridViewDonViSanXuat.GetRow(e.FocusedRowHandle);
            if (selectedEntity == null) return;
            _MaDVSX = selectedEntity.MaDVSX;
            LoadKho(_MaDVSX);

        }

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd && !(view.FocusedColumn == colMaDVSX || (view.FocusedColumn.Equals(this.colSort))))
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == colMaDVSX)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
            }
        }
        private void gridViewDonViSanXuat_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridViewDonViSanXuat_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView view = sender as GridView;

            HangHoaEntity row = gridViewDonViSanXuat.GetRow(gridViewDonViSanXuat.FocusedRowHandle) as HangHoaEntity;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            // Kiểm tra xem ký tự được nhập vào có phải là chữ cái hay không
            // char.IsPunctuation -> Kiểm tra phải số hoặc chữ cái không
            // char.IsControl -> Kiểm tra phải kí tự điều khiển không( control character)

            Console.WriteLine("KeyPress: " + e.KeyChar);


            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
                (view.FocusedColumn == colMaDVSX || view.FocusedColumn == colTenDVSX))
            {
                // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
                if (char.IsWhiteSpace(e.KeyChar))
                {
                    if (view.FocusedColumn == colMaDVSX)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenDVSX)
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }

                // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
                if (!char.IsPunctuation(e.KeyChar))
                {
                    if (view.FocusedColumn == colMaDVSX)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenDVSX)
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }

            if (view.FocusedColumn == colMaDVSX || view.FocusedColumn == colTenDVSX || view.FocusedColumn == colMaHienThi)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void gridDonViSanXuat_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gridViewDonViSanXuat_KeyPress(grid.FocusedView, e);
        }

        private void gridViewDonViSanXuat_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            //if (gridViewDonViSanXuat.FocusedColumn==this.colSort)
            //{
            //    int valueChanged = -1;
            //    if (int.TryParse(e.Value.ToString(), out valueChanged)&&valueChanged>0)
            //    {
            //List<DonViSanXuatEntity> lstCount = lstDonViSanXuatEntity.Where(x => x.Sort == int.Parse(e.Value.ToString())).ToList();
            //if (lstCount != null && lstCount.Count > 0)
            //{
            //    e.Valid = false;
            //    e.ErrorText = "Thứ tự bị trùng. Vui lòng nhập số thứ tự khác.!";
            //}
            //    }
            //}

            if (gridViewDonViSanXuat.FocusedColumn == this.colMaDVSX)
            {
                if (e.Value.ToString() == "")
                {
                    e.Valid = false;
                    e.ErrorText = "Thông tin không được để trống.!";
                }
                else
                {
                    List<DonViSanXuatEntity> LstDVSX = gridViewDonViSanXuat.DataSource as List<DonViSanXuatEntity>;
                    DonViSanXuatEntity dvsx = LstDVSX.Where(item => item.MaDVSX == e.Value.ToString()).FirstOrDefault();
                    if (dvsx != null)
                    {
                        e.Valid = false;
                        e.ErrorText = "Mã đơn vị sản xuất đã bị trùng. Vui lòng nhập mã đơn vị sản xuất mới.!";
                    }
                    if (ContainsVietnamese(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Mã đơn vị sản xuất không được chứa dấu tiếng việt.!";
                    }
                }
            }
            if (gridViewDonViSanXuat.FocusedColumn == this.colTenDVSX)
            {
                if (string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Thông tin không được để trống.!";
                }
                else
                {
                    List<DonViSanXuatEntity> Lstdvsx = gridViewDonViSanXuat.DataSource as List<DonViSanXuatEntity>;
                    DonViSanXuatEntity dvsxx = Lstdvsx.Where(item => CheckDupLiCateString(item.TenDVSX, e.Value.ToString())).FirstOrDefault();
                    if (dvsxx != null)
                    {
                        e.Valid = false;
                        e.ErrorText = "Khách hàng đã bị trùng. Vui lòng nhập tên khách hàng khác.!";
                    }
                }


            }
            if (e.Valid == false)
                Luu.Enabled = false;
            else Luu.Enabled = true;
        }
        private bool CheckDupLiCateString(string firstString, string secondString)
        {
            if (firstString == null || (firstString != null && string.IsNullOrEmpty(firstString)))
            {
                return false;
            }
            return firstString.Replace(" ", "").ToUpper().Equals(secondString.Replace(" ", "").ToUpper());
        }
        private void gridViewDonViSanXuat_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        private void gridViewDonViSanXuat_RowCellClick(object sender, RowCellClickEventArgs e)
        {

        }

        private void gridViewDonViSanXuat_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridViewDonViSanXuat.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InGroupRow || e.HitInfo.InRow)
                {
                    DevExpress.Utils.Menu.DXMenuItem menuTCItem = new DevExpress.Utils.Menu.DXMenuItem("Chi Tiết", ItemsTC_Click);
                    e.Menu.Items.Add(menuTCItem);

                }
            }
        }

        private void ItemsTC_Click(object sender, EventArgs e)
        {
            LoadNBTP();
        }

        private void LoadNBTP()
        {
            NtbSoft.ERP.Win.Modules.ThuVien.frmDonViSanXuatDetail frm = new NtbSoft.ERP.Win.Modules.ThuVien.frmDonViSanXuatDetail(_MaDVSX);
            frm.ShowDialog();
            _status = ResourceURL.EventStatus.Edit;
        }

        private void LoadData()
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmWait), true, true, false);
        }

        private void gridViewDonViSanXuat_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewDonViSanXuat_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            Console.WriteLine("end_cell_value_changing");
        }
        #region QLKho Thoai
        private void btnQLKho_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pnlQLKho.Visible = true;
            layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
           // emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            LoadDSKho();
        }
        private void btnCloseKho_Click(object sender, EventArgs e)
        {
            if (_statusKho == ResourceURL.EventStatus.Add || _statusKho == ResourceURL.EventStatus.Edit)
            {
                if (_statusKho == ResourceURL.EventStatus.Add && gridViewDSKho.IsNewItemRow(gridViewDSKho.FocusedRowHandle))
                {
                    gridViewDSKho.CancelUpdateCurrentRow();
                }
                else if (_statusKho == ResourceURL.EventStatus.Edit)
                {
                    gridViewDSKho.CancelUpdateCurrentRow();
                }
            }
            pnlQLKho.Visible = false;
            layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
          //  emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            KhoGridViewUpdateStatus(ResourceURL.EventStatus.View);
        }

        private void btnThemKho_Click(object sender, EventArgs e)
        {
            ThemKho();
        }

        private void LoadDSKho()
        {
            gridViewDSKho.CellValueChanged -= gridViewDSKho_CellValueChanged;
            gridViewDSKho.InitNewRow -= gridViewDSKho_InitNewRow;
            gridViewDSKho.RowUpdated -= gridViewDSKho_RowUpdated;
            try
            {
                _statusKho = ResourceURL.EventStatus.View;

                DataTable dsKho = new DataTable();
                DataTable dsDVSX = new DataTable();
                string url = string.Format("{0}?", URL + "DonViSanXuat/GetKhoList");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (!string.IsNullOrEmpty(json))
                {
                    dsKho = JsonConvert.DeserializeObject<DataTable>(json);
                }
                string url2 = string.Format("{0}?", URL + "DonViSanXuat/GetLookupDVSX");
                string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;

                if (!string.IsNullOrEmpty(json2))
                {
                    dsDVSX = JsonConvert.DeserializeObject<DataTable>(json2);
                }



                grcQLKho.DataSource = dsKho;

                repoLookUpDVSX.DataSource = dsDVSX;
                repoLookUpDVSX.DisplayMember = "TenKho";
                repoLookUpDVSX.ValueMember = "MaKho";
                repoLookUpDVSX.NullText = "";
                repoLookUpDVSX.ShowHeader = false;
                colQLKhoDVSX.ColumnEdit = repoLookUpDVSX;
                colQLKhoDVSX.FieldName = "ParentMaKho";

                colQLKhoParentMaKho.FieldName = "ParentMaKho";

                gridViewDSKho.RefreshData();
                gridViewDSKho.OptionsBehavior.Editable = true;
                foreach (GridColumn col in gridViewDSKho.Columns)
                    col.OptionsColumn.AllowEdit = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi tải danh mục kho:\n{ex.Message}",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            gridViewDSKho.CellValueChanged += gridViewDSKho_CellValueChanged;
            gridViewDSKho.InitNewRow += gridViewDSKho_InitNewRow;
            gridViewDSKho.RowUpdated += gridViewDSKho_RowUpdated;
            colQLKhoID.Visible = false;
            colQLKhoMaKho.Visible = false;
            colQLKhoStatusVT.Visible = false;
            colQLKhoParentMaKho.Visible = false;
            colQLKhoCBM.Visible = false;
        }

        private void ThemKho()
        {
            gridViewDSKho.CloseEditor();
            _statusKho = ResourceURL.EventStatus.Add;
            gridViewDSKho.AddNewRow();
        }

        private void btnXoaKho_Click(object sender, EventArgs e)
        {
            XoaKho();
        }

        private void btnLuuKho_Click(object sender, EventArgs e)
        {
            LuuKho();
        }

        private void btnNapLaiQLKho_Click(object sender, EventArgs e)
        {
            gridViewDSKho.CancelUpdateCurrentRow();
            _statusKho = ResourceURL.EventStatus.View;
            LoadDSKho();
        }
        private void LuuKho()
        {
            gridViewDSKho.CloseEditor();
            gridViewDSKho.UpdateCurrentRow();

            DataTable dt = grcQLKho.DataSource as DataTable;

            bool hasErrors = false;
            string lastError = "";

            DataTable dtAdded = dt.GetChanges(DataRowState.Added).Copy();

            if (dtAdded != null)
            {
                var rowsToDelete = dtAdded.AsEnumerable()
    .Where(r =>
        (r.IsNull("TenKho") || string.IsNullOrWhiteSpace(r.Field<string>("TenKho"))) &&
        (r.IsNull("ParentMaKho") || string.IsNullOrWhiteSpace(r.Field<string>("ParentMaKho"))))
    .ToList();

                foreach (var row in rowsToDelete)
                    dtAdded.Rows.Remove(row);
                var duplicatedKeys = dtAdded.AsEnumerable()
                    .GroupBy(r => new
                    {
                        TenKho = (r.Field<string>("TenKho") ?? string.Empty).ToUpper().Trim(),
                        ParentMaKho = (r.Field<string>("ParentMaKho") ?? string.Empty).ToUpper().Trim(),
                        //TenDVSX = (r.Field<string>("TenDVSX") ?? string.Empty).ToUpper().Trim()
                    })
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicatedKeys.Any())
                {
                    var message = string.Join(Environment.NewLine, duplicatedKeys.Select(k => $"Kho: {k.TenKho}"));
                    XtraMessageBox.Show($"Phát hiện dữ liệu trùng:{Environment.NewLine}{message}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                   // throw new InvalidOperationException($"Phát hiện dữ liệu trùng:{Environment.NewLine}{message}");
                }
                if (dtAdded.Columns.Contains("TenDVSX"))
                {
                    dtAdded.Columns.Remove("TenDVSX");
                }    
                try
                {
                    //string maKho = row.Table.Columns.Contains("MaKho") ? row["MaKho"]?.ToString() : null;
                    //string tenKho = row["TenKho"]?.ToString();
                    //if (string.IsNullOrWhiteSpace(tenKho)) continue;
                    string url1 = $"{URL}DonViSanXuat/PostKho";
                    string result = "";
                    result = Task.Run(async () => { return await _clientExtension.PostAsync(url1, dtAdded); }).Result;


                    if (!string.Equals(result, "True", StringComparison.OrdinalIgnoreCase) && !result.StartsWith("KH"))
                    {
                        XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


            }
            //if (!hasErrors)
            //{
            //    DataTable dtModified = dt.GetChanges(DataRowState.Modified);
            //    if (dtModified != null)
            //    {
            //        foreach (DataRow row in dtModified.Rows)
            //        {
            //            try
            //            {
            //                string maKho = row["MaKho"]?.ToString();
            //                string tenKho = row["TenKho"]?.ToString();
            //                string ghiChu = row["GhiChu"]?.ToString();
            //                string maDVSX = row["ParentMaKho"]?.ToString();
            //                string tenDVSX = null;
            //                int? statusVT = row["Status_VT"] != DBNull.Value ? Convert.ToInt32(row["Status_VT"]) : (int?)null;
            //                double? cbm = row["CBM"] != DBNull.Value ? Convert.ToDouble(row["CBM"]) : (double?)null;

            //                if (string.IsNullOrWhiteSpace(maKho) || string.IsNullOrWhiteSpace(tenKho)) continue;

            //                //string url1 = string.Format("{0}?", URL + "DonViSanXuat/UpdateKho?makho=");
            //                string url1 = $"{URL}DonViSanXuat/UpdateKho?makho={maKho}&tenKho={tenKho}&parentMaKho={maDVSX}&tenDVSX={tenDVSX}&ghiChu={ghiChu}&statusVT={statusVT}&cbm={cbm}";
            //                string result = "";
            //                result = Task.Run(async () =>
            //                {
            //                    return await _clientExtension.PostAsync(url1, null);
            //                }).Result;

            //                // result = _khoModel.UpdateKho(maKho, tenKho, maDVSX, tenDVSX, ghiChu, statusVT, cbm);

            //                if (!string.Equals(result, "True", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    hasErrors = true;
            //                    lastError = $"Lỗi khi cập nhật kho '{tenKho}': {result}";
            //                    break;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                hasErrors = true;
            //                lastError = $"Lỗi nghiêm trọng khi cập nhật: {ex.Message}";
            //                break;
            //            }
            //        }
            //    }
            //}
            if (hasErrors)
            {
                clsWaitForm.ShowErrorForm(this, 3000);
                XtraMessageBox.Show(lastError, "Lỗi khi lưu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dtAdded == null && dt.GetChanges(DataRowState.Modified) == null)
            {
                clsWaitForm.ShowWaningFormCustomV3(this, 2000, "Không có thay đổi nào để lưu!");
            }
            else
            {
                dt.AcceptChanges();
                clsWaitForm.ShowSuccessForm(this, 2000);
                _statusKho = ResourceURL.EventStatus.View;
                LoadDSKho();
                string currentMaDVSX = GetCurrentMaDVSX();
                if (!string.IsNullOrEmpty(currentMaDVSX))
                {
                    LoadKho(currentMaDVSX);
                }
            }
            //btnLuuKho.Enabled = false;
        }

        private void XoaKho()
        {
            int[] selectedRowHandles = gridViewDSKho.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;
            if (XtraMessageBox.Show($"Bạn có chắc muốn xóa những dòng kho đã chọn không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int successCount = 0;
                int errorCount = 0;
                string lastError = "";

                List<int> newRowHandles = new List<int>();
                List<string> maKhoToDelete = new List<string>();

                foreach (int rowHandle in selectedRowHandles)
                {
                    string maKho = gridViewDSKho.GetRowCellValue(rowHandle, colQLKhoMaKho)?.ToString();
                    if (string.IsNullOrEmpty(maKho))
                    {
                        newRowHandles.Add(rowHandle);
                    }
                    else
                    {
                        maKhoToDelete.Add(maKho);
                    }
                }
                gridViewDSKho.BeginUpdate();
                try
                {
                    newRowHandles.Sort();
                    newRowHandles.Reverse();
                    foreach (int rowHandle in newRowHandles)
                    {
                        gridViewDSKho.DeleteRow(rowHandle);
                        successCount++;
                    }
                    if (maKhoToDelete.Count > 0)
                    {
                        foreach (string maKho in maKhoToDelete)
                        {
                            try
                            {
                               // string url = string.Format("{0}?Parameter={1}", URL + "HinhThuc/Delete", id);
                                string url = $"{URL}DonViSanXuat/DeleteKho?makho={maKho}";
                                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                                //if (result.ToLower() == "true")
                                 //   LoadData();
                               // string result = _khoModel.DeleteKho(maKho);
                                if (result.ToUpper() == "TRUE")
                                {
                                    successCount++;
                                }
                                else { errorCount++; lastError = result; }
                            }
                            catch (Exception ex)
                            {
                                errorCount++; lastError = ex.Message;
                            }
                        }
                    }
                }
                finally
                {
                    gridViewDSKho.EndUpdate();
                }
                if (errorCount > 0)
                {
                    XtraMessageBox.Show($"Xóa thành công: {successCount} kho.\nXóa thất bại: {errorCount} kho.\nLỗi: {lastError}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                LoadDSKho();
                string currentMaDVSX = GetCurrentMaDVSX();
                if (!string.IsNullOrEmpty(currentMaDVSX))
                {
                    LoadKho(currentMaDVSX);
                }
            }
        }
        private void Grids_Enter(object sender, EventArgs e)
        {
            bool isMainGrid = (sender == gridDonViSanXuat || sender == gridViewDonViSanXuat);

            if (isMainGrid)
            {
                actionControlAdd.action = BtThem;
                actionControlAdd.permission = _allowAdd;
                actionControlAdd.Enabled = this.Them.Enabled;

                actionControlEdit.action = BtSua;
                actionControlEdit.permission = _allowEdit;
                actionControlEdit.Enabled = this.Sua.Enabled;

                actionControlDelete.action = Xoadulieu;
                actionControlDelete.permission = _allowDelete;
                actionControlDelete.Enabled = this.Xoa.Enabled;

                actionControlSave.action = Luudulieu;
                actionControlSave.permission = (_allowAdd || _allowEdit);
                actionControlSave.Enabled = this.Luu.Enabled;

                actionControlRefresh.action = NapLaiDong;
                actionControlRefresh.permission = true;
                actionControlRefresh.Enabled = this.Naplai.Enabled;
            }
            else
            {
                actionControlAdd.action = ThemKho;
                actionControlAdd.permission = _allowAdd;
                actionControlAdd.Enabled = this.btnThemKho.Enabled;

                actionControlDelete.action = XoaKho;
                actionControlDelete.permission = _allowDelete;
                actionControlDelete.Enabled = this.btnXoaKho.Enabled;

                actionControlSave.action = LuuKho;
                actionControlSave.permission = (_allowAdd || _allowEdit);
                actionControlSave.Enabled = this.btnLuuKho.Enabled;

                actionControlRefresh.action = () => btnNapLaiQLKho_Click(null, null);
                actionControlRefresh.permission = true;
                actionControlRefresh.Enabled = this.btnNapLaiKho.Enabled;
            }
        }
        private void Grid_KeyDown_Handler(object sender, KeyEventArgs e)
        {
            frm_KeyDown(sender, e);
        }
        private void gridViewDSKho_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_statusKho == ResourceURL.EventStatus.View)
                _statusKho = ResourceURL.EventStatus.Edit;
            if (e.Column == colQLKhoDVSX)
            {
                gridViewDSKho.SetRowCellValue(e.RowHandle, colQLKhoParentMaKho, e.Value);
            }
            //btnLuuKho.Enabled = true;
        }

        private void gridViewDSKho_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            _statusKho = ResourceURL.EventStatus.Add;
            //btnLuuKho.Enabled = true;
        }

        private void gridViewDSKho_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            btnLuuKho.Enabled = true;
        }
        private string GetCurrentMaDVSX()
        {
            DonViSanXuatEntity focusedDVSX = gridViewDonViSanXuat.GetFocusedRow() as DonViSanXuatEntity;
            return focusedDVSX?.MaDVSX;
        }
        #endregion QLKho Thoai
        private void btnSaveKho_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            var dtKho = grcKho.DataSource as DataTable;
            DataTable dtSave = CreatedtKhoSave();
            foreach (DataRow dr in dtKho.Rows)
            {
                if (!(bool)dr["Chon"]) continue;
                var drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaDVSX"] = _MaDVSX;
                drNew["MaKho"] = dr["MaKho"];
                dtSave.Rows.Add(drNew);
            }
            string url1 = string.Format("{0}", URL + $"DonViSanXuat/PostCaiDatKho?MaDVSX={_MaDVSX}");
            string msResult1 = "";
            msResult1 = Task.Run(async () => { return await _clientExtension.PostAsync(url1, dtSave); }).Result;
            if (msResult1.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
        }

        private void gridViewDonViSanXuat_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

     
       

        private void btnNapLaiKho_Click(object sender, EventArgs e)
        {
            gridViewDSKho.CancelUpdateCurrentRow();
            _statusKho = ResourceURL.EventStatus.View;
            LoadDSKho();
        }

        private void btnCloseKho_Click_1(object sender, EventArgs e)
        {
            if (_statusKho == ResourceURL.EventStatus.Add || _statusKho == ResourceURL.EventStatus.Edit)
            {
                if (_statusKho == ResourceURL.EventStatus.Add && gridViewDSKho.IsNewItemRow(gridViewDSKho.FocusedRowHandle))
                {
                    gridViewDSKho.CancelUpdateCurrentRow();
                }
                else if (_statusKho == ResourceURL.EventStatus.Edit)
                {
                    gridViewDSKho.CancelUpdateCurrentRow();
                }
            }
            pnlQLKho.Visible = false;
            layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            KhoGridViewUpdateStatus(ResourceURL.EventStatus.View);
        }

        private void gridViewDonViSanXuat_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
        private DataTable CreatedtKhoSave()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaDVSX", typeof(string));
            dt.Columns.Add("MaKho", typeof(string));
            return dt;
        }
    }
}
