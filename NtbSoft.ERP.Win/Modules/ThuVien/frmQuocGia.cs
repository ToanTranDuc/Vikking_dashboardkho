using DevExpress.DataAccess.Excel;
using DevExpress.DataProcessing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmQuocGia : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        List<int> lstRowUpdate = new List<int>();
        List<QuocGiaEntity> lstQuocGiaEntity;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();

        List<DataTable> lstThuVienDaDung = new List<DataTable>();
        private HashSet<DataRow> selectedRowsCang = new HashSet<DataRow>();
        private Dictionary<string, string> oldCangByCountry = new Dictionary<string, string>();
        #region Thoai tạo biến lấy ds tên nv
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        #endregion
        public frmQuocGia()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
            lstQuocGiaEntity = new List<QuocGiaEntity>();
            lstThuVienDaDung = GET_ListTableThuVienDaDung();
        }
        public List<DataTable> GET_ListTableThuVienDaDung()
        {
            string urlGetListDataTable = URL + "GetThuVien/GetThuVienDaDung";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetListDataTable); }).Result;
            List<DataTable> _ListDataTable = JsonConvert.DeserializeObject<List<DataTable>>(jsonLstDataTable);
            return _ListDataTable;
        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
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
        private void CheckPermission()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit; ;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPermission();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            #region Thoai gettennv
            tenNVHienThi = GetTenNhanVien();
            #endregion
            InitCountryFlag();
            LoadDSQuocGia(false);
            AddMissingCountriesToGrid();
        }

        private void InitCountryFlag()
        {
            // Tạo và cấu hình ImageList
            ImageList imageList = LoadFlagsIntoImageList();
            imageList.ImageSize = new Size(20, 16);
            repositoryItemImageComboBox1.SmallImages = imageList;


            //repositoryItemImageComboBox1.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Flag", 0, 1));
        }
        private ImageList LoadFlagsIntoImageList()
        {
            ImageList imageList = new ImageList();
            for (int i = 0; i < helper.lstQuocKy.Count; i++)
            {
                QuocKy itemQuocKy = helper.lstQuocKy[i];
                imageList.Images.Add(itemQuocKy.image);
                repositoryItemImageComboBox1.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(itemQuocKy.ma, i));
            }
            return imageList;
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridViewQuocGia.OptionsBehavior.Editable = false;

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
                    gridViewQuocGia.OptionsBehavior.Editable = true;
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
                case ResourceURL.EventStatus.Add:
                    gridViewQuocGia.OptionsBehavior.Editable = true;
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
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        private void LoadDSQuocGia(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstQuocGiaEntity = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(json);
                }
                gridQuocGia.DataSource = lstQuocGiaEntity;

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewQuocGia.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridQuocGia;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void ThemDong()
        {
            QuocGiaEntity obj = new QuocGiaEntity();
            //_bindingHangHoaEntity.Add(obj);
            if (lstQuocGiaEntity.Count > 0)
            {
                // Lấy MaQG max hiện có
                var maxMaQG = lstQuocGiaEntity
                    .Where(x => !string.IsNullOrEmpty(x.MaQG))
                    .Select(x =>
                    {
                        int num;
                        return int.TryParse(x.MaQG.Replace("QG_", ""), out num) ? num : 0;
                    })
                    .DefaultIfEmpty(0)
                    .Max();

                obj.MaQG = "QG_" + (maxMaQG + 1);
            }
            else
            {
                obj.MaQG = "QG_1";
            }
            #region Thoai thêm hiển thị tên khi ấn thêm dòng mới
            var currentUser = GlobleData.UserName;
            string tenNV = currentUser;
            if (!string.IsNullOrEmpty(currentUser))
            {
                obj.NguoiTao = currentUser;
                string upperUser = currentUser.Trim().ToUpper();
                if (tenNV != null && tenNV.Contains(upperUser)) tenNV = tenNVHienThi[upperUser];
            }
            #endregion
            lstQuocGiaEntity.Add(obj);

            _rowAdd = gridViewQuocGia.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridViewQuocGia.FocusedRowHandle = _rowAdd;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
            Them.Enabled = false;
            Sua.Enabled = false;
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            focused(this.gridViewQuocGia);
        }
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }
        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    lstThuVienDaDung = GET_ListTableThuVienDaDung();

                    QuocGiaEntity row = gridViewQuocGia.GetRow(gridViewQuocGia.FocusedRowHandle) as QuocGiaEntity;

                    string maQG_Delete = row.MaQG;

                    DataTable _dtKhachHang = lstThuVienDaDung[3];
                    bool isAcceptDelete = true;
                    for (int i = 0; i < _dtKhachHang.Rows.Count; i++)
                    {
                        if (maQG_Delete == _dtKhachHang.Rows[i]["MaQG"].ToString())
                            isAcceptDelete = false;
                    }
                    if (isAcceptDelete == false)
                    {
                        string message = string.Format(@"Mã quốc gia đang được sử dụng, không thể xóa!");
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string url = string.Format("{0}?Parameter={1}", URL + "QuocGia/DeleteQuocgia", row.MaQG);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadDSQuocGia(false);
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void LuuDong()
        {
            try
            {
                List<QuocGiaEntity> _lstUpdate = new List<QuocGiaEntity>();
                this.ActiveControl = this.button1;

                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewQuocGia.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewQuocGia.FocusedRowHandle;
                }
                QuocGiaEntity row = gridViewQuocGia.GetRow(gridViewQuocGia.FocusedRowHandle) as QuocGiaEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (string.IsNullOrEmpty(row.TenQG))
                {
                    DialogResult messResult = MessageBox.Show("Vui lòng nhập tên quốc gia.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(item => item).ToList();

                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    QuocGiaEntity item = gridViewQuocGia.GetRow(lstRowUpdate[i]) as QuocGiaEntity;
                    if (item != null && !string.IsNullOrEmpty(item.TenQG))
                    {
                        string maCangStr = item.MaCang ?? "";
                        string tenCangStr = item.TenCang ?? "";

                        var maTokens = maCangStr.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(s => s.Trim()).ToArray();
                        var tenTokens = tenCangStr.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(s => s.Trim()).ToArray();

                        if (maTokens.Length > 1 || tenTokens.Length > 1)
                        {
                            int count = Math.Max(maTokens.Length, tenTokens.Length);
                            for (int j = 0; j < count; j++)
                            {
                                string ma = j < maTokens.Length ? maTokens[j] : "0";   // nếu thiếu mã, mặc định "0" (theo thỏa thuận để API tạo mới)
                                string ten = j < tenTokens.Length ? tenTokens[j] : "";

                                // Nếu cả mã và tên đều rỗng thì bỏ qua
                                if (string.IsNullOrWhiteSpace(ma) && string.IsNullOrWhiteSpace(ten))
                                    continue;

                                QuocGiaEntity clone = new QuocGiaEntity
                                {
                                    ID = item.ID,
                                    MaQG = item.MaQG,
                                    TenQG = item.TenQG,
                                    GhiChu = item.GhiChu,
                                    MaBuuChinh = item.MaBuuChinh,
                                    MaDienThoai = item.MaDienThoai,
                                    MaQuocKy = item.MaQuocKy,
                                    TenTiengAnh = item.TenTiengAnh,
                                    // Gán đúng: MaCang là mã (hoặc "0" nếu cần API tạo mới), TenCang là tên hiển thị
                                    MaCang = ma,
                                    TenCang = ten,
                                    //Khởi tạo 4 tham số mới
                                    NguoiTao = item.NguoiTao,
                                    NgayTao = item.NgayTao,
                                    NguoiSua = item.NguoiSua,
                                    NgaySua = item.NgaySua
                                };
                                _lstUpdate.Add(clone);
                            }
                        }
                        else
                        {
                            _lstUpdate.Add(item);
                        }
                    }
                }
                string msResult = "";
                string msResult1 = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }
                string url = string.Format("{0}?", URL + "QuocGia/PostQuocGia");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                string urlcang = string.Format("{0}?", URL + "QuocGia/PostCang");
                msResult1 = Task.Run(async () => { return await _clientExtension.PostAsync(urlcang, _lstUpdate); }).Result;

                foreach (var item in _lstUpdate)
                {
                    if (oldCangByCountry.ContainsKey(item.MaQG))
                    {
                        var oldList = oldCangByCountry[item.MaQG]
                            .Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList();

                        var newList = item.MaCang?
                            .Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList() ?? new List<string>();

                        var removed = oldList.Except(newList).ToList();
                        foreach (var maCang in removed)
                        {
                            try
                            {
                                string urlDel = string.Format("{0}?parameter={1}&parameter1={2}",
                                    URL + "QuocGia/DeleteCangQuocGia",
                                    item.MaQG, maCang);

                                string rs = Task.Run(async () => await _clientExtension.DeletedAsync(urlDel)).Result;
                                Console.WriteLine($"Đã xóa cảng {maCang} khỏi quốc gia {item.MaQG}: {rs}");
                            }
                            catch (Exception ex)
                            {
                                XtraMessageBox.Show($"Lỗi khi xóa cảng {maCang}: {ex.Message}");
                            }
                        }
                    }
                }

                if (msResult.ToLower() == "true" && msResult1.ToLower() == "true")
                {
                    LoadDSQuocGia(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                lstRowUpdate.Clear();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void NapLaiDong()
        {
            LoadDSQuocGia(false);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            gridViewQuocGia.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void gridViewQuocGia_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            #region Thoai hiển thị tên nv
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.RowHandle < 0) return;
            QuocGiaEntity item = view.GetRow(e.RowHandle) as QuocGiaEntity;
            if (item == null) return;
            var currentUser = GlobleData.UserName;
            if (string.IsNullOrEmpty(currentUser)) return;

            if(item.ID == 0|| item.ID == null)
            {
                if (string.IsNullOrEmpty(item.NguoiTao)) item.NguoiTao = currentUser;
            }
            else
            {
                item.NguoiSua = currentUser;
            }
            #endregion
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }
        #region Thoai
        private void gridViewQuocGia_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTao || e.Column == colNguoiSua)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
                {
                    string upperUser = username.Trim().ToUpper();
                    if (tenNVHienThi.ContainsKey(upperUser))
                    {
                        e.DisplayText = tenNVHienThi[upperUser];
                    }
                }
            }
        }
        #endregion
        private void gridViewQuocGia_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd)
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                if (view.FocusedColumn == colMaQG)
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == colMaQG || view.FocusedColumn == colCang)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
            }
        }

        private void gridViewQuocGia_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridViewQuocGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView view = sender as GridView;

            HangHoaEntity row = gridViewQuocGia.GetRow(gridViewQuocGia.FocusedRowHandle) as HangHoaEntity;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            // Kiểm tra xem ký tự được nhập vào có phải là chữ cái hay không
            // char.IsPunctuation -> Kiểm tra phải số hoặc chữ cái không
            // char.IsControl -> Kiểm tra phải kí tự điều khiển không( control character)

            Console.WriteLine("KeyPress: " + e.KeyChar);


            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
                (view.FocusedColumn == colMaQG || view.FocusedColumn == colTenQG))
            {
                // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
                if (char.IsWhiteSpace(e.KeyChar))
                {
                    if (view.FocusedColumn == colMaQG)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenQG)
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
                    if (view.FocusedColumn == colMaQG)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenQG)
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }

            if (view.FocusedColumn == colMaQG || view.FocusedColumn == colTenQG || view.FocusedColumn == colGhiChu)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        // Fires when an in-place editor is active
        private void gridControlQuocGia_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gridViewQuocGia_KeyPress(grid.FocusedView, e);
        }

        private void gridViewQuocGia_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridViewQuocGia.FocusedRowHandle < 0)
                return;
            if (gridViewQuocGia.FocusedColumn == this.colMaQG)
            {
                if (e.Value.ToString() == "")
                {
                    e.Valid = false;
                    e.ErrorText = "Thông tin không được để trống.!";
                }
                else
                {
                    //DataRow dr = ((DataTable)gridViewHangHoa.DataSource).AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();
                    //DataRow dr = gridViewHangHoa.DataSource as BindingList<HangHoaEntity>();
                    //    .AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();
                    List<QuocGiaEntity> LstHangHoa = gridViewQuocGia.DataSource as List<QuocGiaEntity>;
                    QuocGiaEntity hangHoa = LstHangHoa.Where(item => item.MaQG == e.Value.ToString()).FirstOrDefault();
                    if (hangHoa != null)
                    {
                        e.Valid = false;
                        e.ErrorText = "Mã quốc gia đã bị trùng. Vui lòng nhập mã quốc gia mới.!";
                    }
                }
            }
            if (gridViewQuocGia.FocusedColumn == this.colTenQG)
            {
                //if (e.Value.ToString() != "")
                //{
                //    ReplaceMaMau(e.Value.ToString());

                //}
                if (string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Quốc gia không được để trống.!";
                }
                else
                {
                    List<QuocGiaEntity> LstQuocGia = gridViewQuocGia.DataSource as List<QuocGiaEntity>;
                    //QuocGiaEntity hangHoa = LstHangHoa.Where(item => item.TenQG == e.Value.ToString()).FirstOrDefault();
                    QuocGiaEntity quocGia = LstQuocGia.Where(item => CheckDupLiCateString(item.TenQG, e.Value.ToString())).FirstOrDefault();
                    if (quocGia != null)
                    {
                        e.Valid = false;
                        e.ErrorText = "Quốc gia đã bị trùng. Vui lòng nhập tên Quốc gia khác.!";
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

        private void gridViewQuocGia_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private string ReplaceMaMau(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Sử dụng regex để chuyển đổi dấu thành không dấu
            string pattern = @"\p{IsCombiningDiacriticalMarks}+";
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            string result = Regex.Replace(normalizedString, pattern, string.Empty);
            result = Regex.Replace(result, @"Đ", "D");
            result = result.Replace(" ", "_");

            // Thêm hậu tố "_"+_nextID
            //result +=string.Format("{0}_{1}");
            //return string.Format("{0}_{1}",result,_nextID);
            return result;
        }

        private void gridViewQuocGia_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void repositoryItemImageComboBox1_EditValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("repositoryItemImageComboBox1_EditValueChanged");
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string thuVien = "QuocGia";
            frmImPortExcelThuVien f = new frmImPortExcelThuVien(thuVien);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
            //OpenFileDialog Sfd = new OpenFileDialog();
            //Sfd.Title = "File To Save";
            //Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            //if (Sfd.ShowDialog() == DialogResult.OK)
            //{
            //    ReadExcelPL(Sfd.FileName);
            //}
            LoadDSQuocGia(false);
        }

        private void ReadExcelPL(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                lstQuocGiaEntity.Clear();
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;

                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A1:ZZ500");
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();
                DataTable dtSave = new DataTable();
                dtSave = source.ToDataTable();
                AddList(dtSave);
            }

            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        private void AddList(DataTable dtSave)
        {

            string TenQG = string.Empty, GhiChu = string.Empty, MaBuuChinh = string.Empty, MaDienThoai = string.Empty, MaQuocKy = string.Empty, TenTiengAnh = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstQuocGiaEntity = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(json);
            }
            List<string> danhsachtrung = new List<string>();
            foreach (DataRow row in dtSave.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }

                TenQG = row[0].ToString();
                TenTiengAnh = row[1].ToString();
                MaBuuChinh = row[2].ToString();
                MaDienThoai = row[3].ToString();
                MaQuocKy = row[4].ToString();
                GhiChu = row[5].ToString();
                bool Istrung = lstQuocGiaEntity.Any(x => x.TenQG == TenQG);
                if (Istrung)
                {
                    //XtraMessageBox.Show($"Mã Khách Hàng bị trùng: {MaDT}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhsachtrung.Add(TenQG + "\n");
                }
                else
                {
                    lstQuocGiaEntity.Add(new QuocGiaEntity
                    {
                        TenQG = TenQG,
                        TenTiengAnh = TenTiengAnh,
                        MaBuuChinh = MaBuuChinh,
                        MaDienThoai = MaDienThoai,
                        MaQuocKy = MaQuocKy,
                        GhiChu = GhiChu
                    });
                    string msResult = "";
                    string urlpost = string.Format("{0}?", URL + "QuocGia/PostQuocGia");
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlpost, lstQuocGiaEntity); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        LoadDSQuocGia(false);
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else XtraMessageBox.Show(msResult);
                    //_status = ResourceURL.EventStatus.View;
                    //GridViewUpdateStatus(_status);
                    //lstRowUpdate.Clear();
                }
            }
            if (danhsachtrung.Any())
            {
                string dstrungma = string.Join(", ", danhsachtrung.Distinct());
                XtraMessageBox.Show($"Quốc Gia: {dstrungma} đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void gridViewQuocGia_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewQuocGia_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Column == colCang)
            {
                int rowHandle = e.RowHandle;

                string tenCangCur = gridViewQuocGia.GetRowCellValue(rowHandle, "TenCang")?.ToString() ?? "";
                string maQGCur = gridViewQuocGia.GetRowCellValue(rowHandle, "MaQG")?.ToString() ?? "";
                string maCangCur = gridViewQuocGia.GetRowCellValue(rowHandle, "MaCang")?.ToString() ?? "";
                if (!string.IsNullOrEmpty(maQGCur))
                {
                    oldCangByCountry[maQGCur] = maCangCur;
                }
                ImportCang frm = new ImportCang();
                frm.InitialTenCangText = tenCangCur;
                frm.InitialMaCangText = maCangCur;
                frm.StartPosition = FormStartPosition.CenterScreen;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // Gán lại cả 2 giá trị
                    gridViewQuocGia.SetRowCellValue(rowHandle, "TenCang", frm.SelectedTenCangText);
                    gridViewQuocGia.SetRowCellValue(rowHandle, "MaCang", frm.SelectedMaCangText);

                    // Cột hiển thị chính
                    //gridViewQuocGia.SetRowCellValue(rowHandle, colCang, frm.SelectedTenCangText);

                    if (!lstRowUpdate.Contains(rowHandle))
                        lstRowUpdate.Add(rowHandle);

                    gridViewQuocGia.RefreshRow(rowHandle);
                    LuuDong();
                }
            }
        }

        private void gridViewQuocGia_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridViewQuocGia_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private string NormalizeCountryName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            normalized = sb.ToString().Normalize(NormalizationForm.FormC);
            normalized = Regex.Replace(normalized, @"\s+", "");
            return normalized.ToUpperInvariant();
        }

        

        private void AddMissingCountriesToGrid()
        {
            try
            {
                if (lstQuocGiaEntity == null)
                    lstQuocGiaEntity = new List<QuocGiaEntity>();

                // Danh sách hiện có (TenQG đã có trong DB)
                var existingNames = lstQuocGiaEntity
                    .Where(x => !string.IsNullOrEmpty(x.TenQG))
                    .Select(x => NormalizeCountryName(x.TenQG))
                    .ToHashSet();

                // Tính số lớn nhất hiện có theo logic ThemDong()
                int maxMaQGNum = lstQuocGiaEntity
                    .Where(x => !string.IsNullOrEmpty(x.MaQG))
                    .Select(x =>
                    {
                        int num;
                        return int.TryParse(x.MaQG.Replace("QG_", ""), out num) ? num : 0;
                    })
                    .DefaultIfEmpty(0)
                    .Max();

                // Lấy danh sách quốc gia chuẩn từ .NET
                var allRegions = CultureInfo
                    .GetCultures(CultureTypes.SpecificCultures)
                    .Select(c => new RegionInfo(c.LCID))
                    .GroupBy(r => r.TwoLetterISORegionName)
                    .Select(g => g.First())
                    .OrderBy(r => r.EnglishName)
                    .ToList();

                int addedCount = 0;
                var newCountries = new List<QuocGiaEntity>();

                foreach (var region in allRegions)
                {
                    string normName = NormalizeCountryName(region.EnglishName);
                    if (!existingNames.Contains(normName))
                    {
                        maxMaQGNum++;
                        string newMaQG = "QG_" + maxMaQGNum;

                        QuocGiaEntity newCountry = new QuocGiaEntity
                        {
                            ID = 0,
                            MaQG = newMaQG,
                            // Không tạo MaQG để backend tự sinh khi lưu
                            TenQG = region.EnglishName.ToString().ToUpper(),
                            TenTiengAnh = region.EnglishName.ToString().ToUpper(),
                            MaQuocKy = "",
                            MaBuuChinh = "",
                            MaDienThoai = "",
                            GhiChu = ""
                        };

                        lstQuocGiaEntity.Add(newCountry);
                        existingNames.Add(normName);
                        addedCount++;
                    }
                }
                if (addedCount > 0)
                {
                    lstQuocGiaEntity.AddRange(newCountries);

                    gridQuocGia.DataSource = null;
                    gridQuocGia.DataSource = lstQuocGiaEntity;
                    gridViewQuocGia.RefreshData();

                    // Chuẩn bị danh sách index các hàng mới để LuuDong xử lý
                    int firstNewIndex = lstQuocGiaEntity.Count - addedCount;
                    lstRowUpdate = Enumerable.Range(firstNewIndex, addedCount).ToList();

                    // Đặt trạng thái Add để enable nút Lưu trong GridViewUpdateStatus
                    _status = ResourceURL.EventStatus.Add;
                    GridViewUpdateStatus(_status);

                    // Focus vào hàng đầu tiên mới thêm để LuuDong không báo lỗi
                    gridViewQuocGia.FocusedRowHandle = firstNewIndex;

                    // Gọi lưu
                    LuuDong();
                }
                else
                {
                    // Không có gì mới — chỉ refresh UI
                    gridQuocGia.DataSource = null;
                    gridQuocGia.DataSource = lstQuocGiaEntity;
                    gridViewQuocGia.RefreshData();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi thêm quốc gia còn thiếu: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Thoai
        private Dictionary<string, string> GetTenNhanVien()
        {
            try
            {
                string url = string.Format("{0}?", URL + "GetTenNV/GetTenNV");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    DataTable dtNhanVien = JsonConvert.DeserializeObject<DataTable>(json);
                    string userNameCol = "UserName";
                    string tenNVCol = "TenNV";
                    return dtNhanVien.AsEnumerable()
                        .Where(row => row[userNameCol] != DBNull.Value && row[userNameCol] != null && !string.IsNullOrWhiteSpace(row.Field<string>(userNameCol))) // Lọc row null/rỗng
                        .GroupBy(row => row.Field<string>(userNameCol).Trim().ToUpper())
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().Field<string>(tenNVCol) ?? g.Key
                        );
                }

            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
            return new Dictionary<string, string>();
        }
        #endregion
    }
}
