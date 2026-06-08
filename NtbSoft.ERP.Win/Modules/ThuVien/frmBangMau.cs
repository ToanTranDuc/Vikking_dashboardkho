using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmBangMau : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<BangMauEntity> lstBangMau;
        List<BangMauEntity> lstBangMauKT;
        List<BangMauEntity> lstBangMauKiemTra;
        List<int> lstRowUpdate = new List<int>();
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
        List<DataTable> lstThuVienDaDung = new List<DataTable>();
        public frmBangMau()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstBangMau = new List<BangMauEntity>();
            lstBangMauKT = new List<BangMauEntity>();
            lstBangMauKiemTra = new List<BangMauEntity>();
            lstThuVienDaDung = GET_ListTableThuVienDaDung();
        }
        public List<DataTable> GET_ListTableThuVienDaDung()
        {
            string urlGetListDataTable = URL + "GetThuVien/GetThuVienDaDung";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetListDataTable); }).Result;
            List<DataTable> _ListDataTable = JsonConvert.DeserializeObject<List<DataTable>>(jsonLstDataTable);
            return _ListDataTable;
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {

            CreateSearchLookup();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            Init();

        }
        private void Init()
        {

            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";

            searchLookUpEditFilter.Properties.ValueMember = "MaHang";
            searchLookUpEditFilter.Properties.DisplayMember = "TenHang";
            GetKH();
        }
        private void GetKH()
        {

            string url = $"{URL}KhaiBaoAll/Get?action=GetKHBangSize";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditKH.Properties.DataSource = null;
                return;
            }
            searchLookUpEditKH.Properties.DataSource = tbl;
            searchLookUpEditKH.EditValue = tbl.Rows[0]["MaKH"];
        }
        private void GetMH()
        {
            string khachhang = searchLookUpEditKH.EditValue?.ToString() ?? "All";

            string url = $"{URL}KhaiBaoAll/Get?action=GetHangHoaBangMau&para1={khachhang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditFilter.Properties.DataSource = null;
                return;
            }
            searchLookUpEditFilter.Properties.DataSource = tbl;
            searchLookUpEditFilter.EditValue = tbl.Rows[0]["MaHang"];
            LoadDSBangMau(false);
        }
        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            GetMH();
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

        private void CheckPerminsion()
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
                Luu.Enabled = false;
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
                    gridViewBangMau.OptionsBehavior.Editable = false;

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
                    gridViewBangMau.OptionsBehavior.Editable = true;
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
                    gridViewBangMau.OptionsBehavior.Editable = true;
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
        private void LoadDSBangMau(bool isFromSave)
        {
            try
            {
                string mahang = searchLookUpEditFilter.EditValue?.ToString() ?? "All";
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "All";
                _status = ResourceURL.EventStatus.View;
                //string url = string.Format("{0}?", URL + "BangMau/GetBangMau");
                string url = $"{URL}BangMau/GetBangMau2?para={mahang}&para2={makh}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);
                    //lstBangMauKT = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);
                }
                gridBangMau.DataSource = lstBangMau;
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewBangMau.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridBangMau;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void CreateSearchLookup()
        {
            string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenHang";
            rCountryEdit.ValueMember = "MaHang";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKH", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Khách hàng", Name = "colTenKH", Visible = true });

            }
            colMaHang.ColumnEdit = rCountryEdit;
            rCountryEdit.EditValueChanged += RCountryEdit_EditValueChanged;

            searchLookUpEditMaHang.Properties.DataSource = tbl;

            //searchLookUpEditFilter.Properties.DataSource = tbl;

            string urlkh = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkh); }).Result;
            DataTable tblkh = JsonConvert.DeserializeObject<DataTable>(jsonkh);
            RepositoryItemSearchLookUpEdit rCountryEditkh = new RepositoryItemSearchLookUpEdit();
            rCountryEditkh.DataSource = tblkh;
            rCountryEditkh.DisplayMember = "TenKH";
            rCountryEditkh.ValueMember = "MaKH";
            rCountryEditkh.ShowClearButton = false;
            rCountryEditkh.NullText = "[Chọn giá trị]";

            GridView dvViewkh = rCountryEditkh.View;
            if (dvViewkh.Columns.Count == 0)
            {
                dvViewkh.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewkh.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewkh.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewkh.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewkh.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaHang", Visible = true });
                dvViewkh.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Tên khách hàng", Name = "colTenHang", Visible = true });

            }
            colMaKH.ColumnEdit = rCountryEditkh;
            searchLookUpEditKhachHang.Properties.DataSource = tblkh;
        }
        private void RCountryEdit_EditValueChanged(object sender, EventArgs e)
        {
            var rCountryEdit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (rCountryEdit == null) return;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)rCountryEdit.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridViews = (DevExpress.XtraGrid.Views.Grid.GridView)gridBangMau.MainView;
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "MaKH", focusedRow["MaKH"].ToString());
                }
            }
        }

        private void ThemDong()
        {
            BangMauEntity obj = new BangMauEntity();
            //_bindingHangHoaEntity.Add(obj);
            lstBangMau.Add(obj);

            _rowAdd = gridViewBangMau.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridViewBangMau.FocusedRowHandle = _rowAdd;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
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
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewBangMau.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewBangMau.FocusedRowHandle;
                }
                List<BangMauEntity> _lstUpdate = new List<BangMauEntity>();

                BangMauEntity row = gridViewBangMau.GetRow(gridViewBangMau.FocusedRowHandle) as BangMauEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }

                //lstRowUpdate.Sort();
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    BangMauEntity item = gridViewBangMau.GetRow(lstRowUpdate[i]) as BangMauEntity;
                    // Lấy giá trị TenMau, bỏ dấu tiếng việt, thay k " " bằng "_" -> Gán cho MaMau   
                    if (item != null && !string.IsNullOrEmpty(item.TenMau))
                    {
                        if (item.MaHang == null)
                        {
                            MessageBox.Show("Vui lòng chọn mã hàng");
                            return;
                        }
                        else
                        {
                            _lstUpdate.Add(item);


                        }
                        //item.ID = 0;

                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    LoadDSBangMau(true);
                    return;
                }
                //HashSet<BangMauEntity> checkedEntities = new HashSet<BangMauEntity>();

                //foreach (var entity1 in lstBangMauKT)
                //{
                //    // Kiểm tra xem entity1 đã được kiểm tra chưa
                //    if (!checkedEntities.Contains(entity1))
                //    {
                //        foreach (var entity2 in lstBangMauKT)
                //        {
                //            // Kiểm tra xem entity2 đã được kiểm tra chưa và có giống entity1 không
                //            if (!checkedEntities.Contains(entity2) && entity1 != entity2 &&
                //                entity1.TenMau == entity2.TenMau && entity1.MaHang == entity2.MaHang)
                //            {
                //                // Có hai đối tượng giống nhau
                //                Console.WriteLine("Có hai đối tượng trùng nhau:");
                //                Console.WriteLine("Entity 1: " + entity1.ToString());
                //                Console.WriteLine("Entity 2: " + entity2.ToString());
                //            }
                //        }

                //        // Đánh dấu entity1 đã được kiểm tra
                //        checkedEntities.Add(entity1);
                //    }
                //}
                foreach (BangMauEntity bm in _lstUpdate)
                {
                    BangMauEntity bangMauKT = lstBangMauKT.Where(item => RemoveVietnameseTone(item.TenMau.ToString().Trim().Replace(" ", "") + item.MaHang).ToUpper().Trim() == (_lstUpdate[0].TenMau.ToString().Trim().Replace(" ", "") + _lstUpdate[0].MaHang).ToUpper().Trim()).FirstOrDefault();
                    BangMauEntity bangMau = lstBangMauKT.Where(item => RemoveVietnameseTone(item.TenMau.ToString().Trim().Replace(" ", "")).ToUpper().Trim() == (_lstUpdate[0].TenMau.ToString().Trim().Replace(" ", "")).ToUpper().Trim()).FirstOrDefault();
                    //if (bangMauKT != null)
                    //{
                    //    MessageBox.Show("Màu và mã hàng bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}
                    if (_status == ResourceURL.EventStatus.Add)
                    {
                        if (bangMau != null)
                            bm.TenMau = bangMau.TenMau;
                    }

                }

                //if(_lstUpdate.)
                string url = string.Format("{0}?", URL + "BangMau/PostBangMau");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
                //MessageBox.Show("Lưu thành công");

                //
                //string url = string.Format("{0}?", URL + "BangMau/PostBangMau");
                //msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstBangMau); }).Result;
                //
                if (msResult.ToLower() == "true")
                {
                    LoadDSBangMau(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                //_status = ResourceURL.EventStatus.View;
                //GridViewUpdateStatus(_status);
                lstRowUpdate.Clear();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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

                    BangMauEntity row = gridViewBangMau.GetRow(gridViewBangMau.FocusedRowHandle) as BangMauEntity;
                    string maMau_delete = row.MaMau;
                    string mhMau_delete = row.MaHang;
                    string khMau_delete = row.MaKH;
                    DataTable _dtKhachHang = lstThuVienDaDung[4];

                    bool isAcceptDelete = true;
                    for (int i = 0; i < _dtKhachHang.Rows.Count; i++)
                    {
                        if (maMau_delete == _dtKhachHang.Rows[i]["MaMau"].ToString() && mhMau_delete == _dtKhachHang.Rows[i]["MaHang"].ToString() 
                            && khMau_delete == _dtKhachHang.Rows[i]["MaKH"])
                            isAcceptDelete = false;
                    }
                    if (isAcceptDelete == false)
                    {
                        string message = string.Format(@"Màu đang được sử dụng, không thể xóa!");
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (row == null) return;
                    string url = string.Format("{0}?Parameter={1}", URL + "BangMau/DeleteBangMau", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadDSBangMau(false);
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void NapLaiDong()
        {
            CreateSearchLookup();
            LoadDSBangMau(false);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }
        private void gridViewBangMau_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            // Tìm index của dòng đang focus trong thứ tự của all danh sách bảng size( Không filter)

            //List<BangMauEntity> _lstBangMau = gridViewBangMau.DataSource as List<BangMauEntity>;
            //BangMauEntity _itemBangMau = gridViewBangMau.GetFocusedRow() as BangMauEntity;
            //int index = _lstBangMau.IndexOf(_itemBangMau);

            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }
        private void gridViewBangMau_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

            focused(sender);
        }

        private void gridViewBangMau_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
            //DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            //if (_status == ResourceURL.EventStatus.Add)
            //{
            //    if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd && !(view.FocusedColumn == colMaMau))
            //        view.FocusedColumn.OptionsColumn.AllowEdit = true;
            //    else
            //        view.FocusedColumn.OptionsColumn.AllowEdit = false;
            //}
            //else
            //{
            //    if (_status == ResourceURL.EventStatus.Edit)
            //    {
            //        if (view.FocusedColumn == colMaMau)
            //            view.FocusedColumn.OptionsColumn.AllowEdit = false;
            //        else
            //            view.FocusedColumn.OptionsColumn.AllowEdit = true;
            //    }
            //}
        }

        private void gridViewBangMau_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridViewBangMau.GetFocusedDataSourceRowIndex() >= 0)
            {
                // Lấy dòng dữ liệu hiện tại
                var focusedRowHandle = gridViewBangMau.GetFocusedDataSourceRowIndex();
                var focusedRow = gridViewBangMau.GetDataRow(focusedRowHandle);

                // Kiểm tra nếu đang chỉnh sửa cột TenMau
                if (gridViewBangMau.FocusedColumn == colTenMau)
                {
                    // Kiểm tra giá trị nhập vào
                    if (e.Value.ToString() == "")
                    {
                        e.Valid = false;
                        e.ErrorText = "Thông tin không được để trống!";
                    }
                    else if (ContainsVietnamese(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Màu không được chứa dấu tiếng Việt!";
                    }


                    var tempLstBangMau = new List<BangMauEntity>(lstBangMau);
                    tempLstBangMau[focusedRowHandle].TenMau = e.Value.ToString();
                    var duplicateItems = tempLstBangMau
                        .GroupBy(bm => new { bm.TenMau, bm.MaHang })
                        .Where(group => group.Count() > 1)
                        .Select(group => new { group.Key.TenMau, group.Key.MaHang })
                        .ToList();
                    if (duplicateItems.Any())
                    {
                        string duplicates = string.Join(", ", duplicateItems.Select(item => $"{item.TenMau} - {item.MaHang}"));
                        e.Valid = false;
                        e.ErrorText = $"Có các mã màu và mã hàng bị trùng: {duplicates}";
                    }
                }
                if (gridViewBangMau.FocusedColumn == colMaHang)
                {
                    var tempLstBangMau = new List<BangMauEntity>(lstBangMau);
                    tempLstBangMau[focusedRowHandle].MaHang = e.Value.ToString();
                    var duplicateItems = tempLstBangMau
                        .GroupBy(bm => new { bm.TenMau, bm.MaHang })
                        .Where(group => group.Count() > 1)
                        .Select(group => new { group.Key.TenMau, group.Key.MaHang })
                        .ToList();
                    if (duplicateItems.Any())
                    {
                        string duplicates = string.Join(", ", duplicateItems.Select(item => $"{item.TenMau} - {item.MaHang}"));
                        e.Valid = false;
                        e.ErrorText = $"Có các mã màu và mã hàng bị trùng: {duplicates}";
                    }
                }
                if (e.Valid == false)
                    Luu.Enabled = false;
                else
                    Luu.Enabled = true;
            }
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        // Bỏ dấu Tiếng Việt của chuỗi
        // Replace kí tự khoảng trắng thành kí tự "_"
        // Thêm hậu tố "_"+ nextID
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

        private void gridViewBangMau_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridControlBangMau_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            GridView view = grid.FocusedView as GridView;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            if (view.FocusedColumn == colMaMau)
            {
                e.Handled = CheckCharacterID(view, e);
            }
            else if (view.FocusedColumn == colTenMau)
            {
                //e.Handled = CheckCharacterName(view, e);
            }

            Console.WriteLine("KeyPress: " + e.KeyChar);


            if (view.FocusedColumn == colMaMau || view.FocusedColumn == colTenMau || view.FocusedColumn == colGhiChu)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }

        }


        private bool CheckCharacterID(GridView view, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Check name
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu không phải là kí tự đặc biệt -> Cho nhập
                // Nếu không phải là dấu câu -> Cho nhập
                if ((!char.IsSymbol(e.KeyChar) && !char.IsPunctuation(e.KeyChar)))
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


        private bool CheckCharacterName(GridView view, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Check name
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu không phải là kí tự đặc biệt -> Cho nhập
                // Nếu không phải là dấu câu -> Cho nhập
                if (char.IsWhiteSpace(e.KeyChar) || (char.IsSymbol(e.KeyChar) && char.IsPunctuation(e.KeyChar)))
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

        private void gridViewBangMau_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridViewBangMau_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }
        static string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        private void button2_Click(object sender, EventArgs e)
        {


            string urlKT = $"{URL}BangMau/GetBangMau";
            string jsonKT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
            if (!string.IsNullOrEmpty(jsonKT))
            {
                lstBangMauKiemTra = JsonConvert.DeserializeObject<List<BangMauEntity>>(jsonKT);
            }
            string _txtMau = txtMau?.EditValue?.ToString() ?? "";
            List<string> _lstMau = _txtMau.Split(':').ToList();
            string _maHang = searchLookUpEditMaHang?.EditValue?.ToString() ?? "";
            List<string> _lstMaHang = _maHang.Split(':').ToList();
            string _txtCodeMau = textBox1?.EditValue?.ToString() ?? "";
            List<string> _lstCodeMau = _txtCodeMau.Split(':').ToList();

            if (string.IsNullOrEmpty(_maHang))
            {
                XtraMessageBox.Show("Mã hàng không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(_txtMau))
            {
                XtraMessageBox.Show("Màu không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_txtCodeMau))
            {

            }

            List<BangMauEntity> _lstAddBangMau = new List<BangMauEntity>();
            int focusedRow = lstBangMau.Count;
            int mauIndex = -1;
            foreach (var mau in _lstMau)
            {
                mauIndex++;
                string codeMauIndex = "";
                int lstCodeMauCount = _lstCodeMau.Count;
                string ghichu = textBox2.Text;
                if (mauIndex < lstCodeMauCount)
                    codeMauIndex = _lstCodeMau[mauIndex];

                if (!string.IsNullOrEmpty(mau))
                {
                    BangMauEntity mauEntity = new BangMauEntity(_maHang.ToString().Trim(), mau.ToString().Trim(), _maKH.ToString().Trim(), "MAU_" + ReplaceSpecialCharacters(RemoveVietnameseTone(mau.ToString().Trim())).ToString().ToUpper(), codeMauIndex);
                    bool existsInList = lstBangMauKiemTra.Any(existing =>
                        existing.MaHang == mauEntity.MaHang && existing.MaKH == mauEntity.MaKH &&
                        existing.TenMau == mauEntity.TenMau
                    );
                    bool existsInList2 = lstBangMau.Any(existing =>
                            existing.MaHang == mauEntity.MaHang && existing.MaKH == mauEntity.MaKH &&
                            existing.TenMau == mauEntity.TenMau
                        );
                    if (existsInList || existsInList2)
                    {
                        XtraMessageBox.Show($"Màu '{mauEntity.TenMau}' cho mã hàng '{mauEntity.MaHang}' đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                    {
                        bool existsInAddList = _lstAddBangMau.Any(existing =>
                            existing.MaHang == mauEntity.MaHang && existing.MaKH == mauEntity.MaKH &&
                            existing.TenMau == mauEntity.TenMau
                        );
                        if (!existsInAddList)
                        {
                            _lstAddBangMau.Add(mauEntity);
                            lstRowUpdate.Add(focusedRow);
                            focusedRow += 1;
                        }
                    }
                }
            }

            lstBangMau.AddRange(_lstAddBangMau);
            gridBangMau.DataSource = lstBangMau;
            gridViewBangMau.FocusedRowHandle = lstBangMau.Count - lstRowUpdate.Count;
            gridViewBangMau.RefreshData();
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            txtMau.EditValue = null;
            searchLookUpEditMaHang.EditValue = "";
            textBox2.Text = "";
            textBox1.EditValue = null;
        }

        private void searchLookUpEditFilter_EditValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("filter");
            //ChangingEventArgs changing = e as ChangingEventArgs;
            //if (changing == null)
            //    return;
            //if (changing.NewValue !=null)
            //{
            //    gridBangMau.DataSource = lstBangMau.Where(x=>x.MaHang == changing.NewValue.ToString());
            //}
            //else
            //{
            //    gridBangMau.DataSource = lstBangMau;
            //}
            LoadDSBangMau(false);
        }

        private void txtMau_KeyPress(object sender, KeyPressEventArgs e)
        {
            string vietnameseCharacters = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđíị";

            if (vietnameseCharacters.Contains(e.KeyChar.ToString().ToLower()))
            {
                // Nếu là ký tự có dấu, không cho phép nhập
                e.Handled = true;
            }
            else
            {
                // Nếu không phải ký tự có dấu, chuyển ký tự thành chữ hoa
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string vietnameseCharacters = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđíị";

            if (vietnameseCharacters.Contains(e.KeyChar.ToString().ToLower()))
            {
                // Nếu là ký tự có dấu, không cho phép nhập
                e.Handled = true;
            }
            else
            {
                // Nếu không phải ký tự có dấu, chuyển ký tự thành chữ hoa
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }
        string _maKH = "";
        private void searchLookUpEditMaHang_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)searchLookUpEditMaHang.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    _maKH = focusedRow["MaKH"].ToString();
                }
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                ReadExcelPL(Sfd.FileName);
            }
        }
        private void ReadExcelPL(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                lstBangMau.Clear();
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

            string MaMau = string.Empty, MaHang = string.Empty, CodeMau = string.Empty, TenMau = string.Empty, GhiChu = string.Empty, MaKH = string.Empty, ColorCode = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}?", URL + "BangMau/GetBangMau");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);
            }
            string urlhh = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonhh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlhh); }).Result;
            DataTable dthh = JsonConvert.DeserializeObject<DataTable>(jsonhh);


            string urlkh = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkh); }).Result;
            DataTable dtkh = JsonConvert.DeserializeObject<DataTable>(jsonkh);
            List<string> danhsachtrung = new List<string>();
            List<string> chuakhaibaohh = new List<string>();
            List<string> chuakhaibaokh = new List<string>();
            foreach (DataRow row in dtSave.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                var rowhh = dthh.AsEnumerable().FirstOrDefault(r => r["TenHang"].ToString().Trim() == row[0].ToString().Trim());
                var rowlkh = dtkh.AsEnumerable().FirstOrDefault(r => r["TenKH"].ToString().Trim() == row[1].ToString().Trim());

                if (rowhh == null)
                {
                    chuakhaibaohh.Add(row[0].ToString());
                }
                if (rowlkh == null)
                {
                    chuakhaibaokh.Add(row[1].ToString());
                }

                if (rowhh == null || rowlkh == null)
                {
                    continue;
                }

                MaMau = "MAU_" + ReplaceSpecialCharacters(RemoveVietnameseTone(row[2].ToString().Trim().ToUpper()));
                MaHang = rowhh["MaHang"].ToString();
                MaKH = rowlkh["MaKH"].ToString();
                TenMau = row[2].ToString();
                CodeMau = row[3].ToString();
                GhiChu = row[4].ToString();
                bool Istrung = lstBangMau.Any(x => x.MaMau == MaMau);
                if (Istrung)
                {
                    //XtraMessageBox.Show($"Mã Khách Hàng bị trùng: {MaDT}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhsachtrung.Add(TenMau + "\n");
                }
                else
                {
                    lstBangMau.Add(new BangMauEntity
                    {
                        MaMau = MaMau,
                        MaHang = MaHang,
                        CodeMau = CodeMau,
                        TenMau = TenMau,
                        MaKH = MaKH,
                        GhiChu = GhiChu
                    });
                    string msResult = "";
                    string urlpost = string.Format("{0}?", URL + "BangMau/PostBangMau");
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlpost, lstBangMau); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        LoadDSBangMau(false);
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else XtraMessageBox.Show(msResult);
                    //_status = ResourceURL.EventStatus.View;
                    //GridViewUpdateStatus(_status);
                    //lstRowUpdate.Clear();
                }
            }
            if (chuakhaibaohh.Any())
            {
                XtraMessageBox.Show($"Các Hàng Hóa chưa khai báo:\n{string.Join("\n", chuakhaibaohh.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chuakhaibaokh.Any())
            {
                XtraMessageBox.Show($"Các Khách Hàng chưa khai báo:\n{string.Join("\n", chuakhaibaokh.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (danhsachtrung.Any())
            {
                string dstrungma = string.Join(", ", danhsachtrung.Distinct());
                XtraMessageBox.Show($"Các mã Màu bị trùng:{dstrungma} của mã hàng :{MaHang}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void gridViewBangMau_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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



        private void gridViewBangMau_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridBangMau_Click(object sender, EventArgs e)
        {

        }

        //private bool ValidateDataAddMulti()
        //{
        //    List<BangMauEntity> lstDataSource = gridViewBangMau.DataSource as List<BangMauEntity>;
        //    for (int i = 0; i < lstRowUpdate.Count; i++)
        //    {
        //        //BangSizeEntity itemSize = lstRowUpdate[i];
        //        BangMauEntity itemSize = (gridViewBangMau.DataSource as List<BangMauEntity>)[lstRowUpdate[i]];

        //        if (string.IsNullOrEmpty(itemSize.TenMau))
        //        {
        //            XtraMessageBox.Show("Màu không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        //            return false;
        //        }
        //        else if (string.IsNullOrEmpty(itemSize.MaHang))
        //        {
        //            XtraMessageBox.Show("Mã hàng không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        //            return false;
        //        }
        //        //
        //        for (int j = 0; j < lstDataSource.Count; j++)
        //        {
        //            // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không insert
        //            if (lstRowUpdate[i] != j && (itemSize.TenMau.Equals(lstDataSource[j].TenMau)
        //                &&itemSize.MaHang.Equals(lstDataSource[j].MaHang)))
        //            {
        //                XtraMessageBox.Show("Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        private void searchLookUpEditKhachHang_EditValueChanged(object sender, EventArgs e)
        {
            CreateSearchLookupMH();
        }
        private void CreateSearchLookupMH()
        {
            string mh = searchLookUpEditMaHang.EditValue == null ? "All" : searchLookUpEditMaHang.EditValue.ToString();
            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "HangHoa/GetHangHoaByKH", searchLookUpEditKhachHang.EditValue.ToString(), mh);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblmh = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tblmh;
            rCountryEdit.DisplayMember = "TenHang";
            rCountryEdit.ValueMember = "MaHang";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKH", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Khách hàng", Name = "colTenKH", Visible = true });
            }
            searchLookUpEditMaHang.Properties.DataSource = tblmh;
        }
    }
}
