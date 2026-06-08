using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmNhanVien : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        DataTable dttb;
        List<NhanVienEntity> lstNV;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        public frmNhanVien()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSNV(false);
            SetupUI();
            CreateSearchLookupPB();
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
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Sua.Enabled = false;
            btnXacNhan.Enabled = true; 
            textEdit11.Enabled = true;
            textEdit3.Enabled = true;
            searchlookupEditPB.Enabled = true;
            textEdit2.Enabled = true;
            textEdit41.Enabled = true;
            textEdit11.Text = "";
            textEdit3.Text = "";
            searchlookupEditPB.Text ="";
            textEdit2.Text = "";
            textEdit41.Text = "";
            textEdit1.Text = "";
        }
        private void ThemDong()
        {


        }
        private void LoadDSNV(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "NhanVien/GetAllNV");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                Console.WriteLine(json);
                if (!string.IsNullOrEmpty(json))
                {
                    dttb = JsonConvert.DeserializeObject<DataTable>(json);
                    lstNV = JsonConvert.DeserializeObject<List<NhanVienEntity>>(json);
                    Console.WriteLine($"Số lượng NV: {lstNV?.Count}");
                    //loadThongTin(lstNV[0].ID);
                }
                gridControl1.DataSource = lstNV;
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridView1.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridControl1;
                }
                btnXacNhan.Enabled = true;
                textEdit11.Enabled = true;
                textEdit3.Enabled = true;
                searchlookupEditPB.Enabled = true;
                textEdit2.Enabled = true;
                textEdit41.Enabled = true;
                textEdit11.Text = "";
                textEdit3.Text = "";
                searchlookupEditPB.Text = "";
                textEdit2.Text = "";
                textEdit41.Text = "";
                textEdit1.Text = "";
            }
            catch (Exception ex)
            {
                return;
            }

        }
        private void loadThongTin(int ID)
        {
            if (dttb != null)
            {
                NhanVienEntity result = lstNV.Find(nl => nl.ID == ID);
                textID.Text = ID.ToString();
                textEdit3.Text = result.TenNV;
                searchlookupEditPB.Text = result.PhongBan;
                textEdit11.Text = result.MaNV;
                textEdit2.Text = result.ChucVu;
                textEdit41.Text = result.GhiChu;
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
                this.ActiveControl = this.Button1;

                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridView1.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridView1.FocusedRowHandle;
                }

                List<NhanVienEntity> _lstUpdate = new List<NhanVienEntity> {};
                NhanVienEntity row = gridView1.GetRow(gridView1.FocusedRowHandle) as NhanVienEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    NhanVienEntity item = gridView1.GetRow(lstRowUpdate[i]) as NhanVienEntity;
                    if (item == null || (item != null && item.MaNV == null))
                    {
                        continue;
                    }
                    if (item != null && !string.IsNullOrEmpty(item.MaNV))
                    {
                        //item.ID = 0;
                        _lstUpdate.Add(item);
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    LoadDSNV(true);
                    return;
                }
                string url = string.Format("{0}", URL + "NhanVien/PostNV");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                if (msResult.ToLower() == "true")
                {
                    LoadDSNV(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                lstRowUpdate.Clear();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }
        private void NapLaiDong()
        {
            LoadDSNV(false);
        }

        private void gridView1_Click(object sender, EventArgs e)
        {

        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            textEdit11.Enabled = true;
            textEdit3.Enabled = true;
            searchlookupEditPB.Enabled = true;
            textEdit2.Enabled = true;
            textEdit41.Enabled = true;
            textEdit1.Enabled = true;
        }
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridView1.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        //btnXacNhan.Enabled = false;
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
                    gridView1.OptionsBehavior.Editable = true;
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
                    gridView1.OptionsBehavior.Editable = true;
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

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if(textEdit11.Text.ToString() == "")
            {
                XtraMessageBox.Show($"Mã nhân viên không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (textEdit3.Text.ToString() == "")
            {
                XtraMessageBox.Show($"Tên nhân viên không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string _tennv = textEdit3?.EditValue?.ToString() ?? "";
            string _manv = textEdit11?.EditValue?.ToString() ?? "";
            string _ten = textEdit1?.EditValue?.ToString() ?? "";
            var currentUser = GlobleData.UserName;
            List<NhanVienEntity> lstaddNV = new List<NhanVienEntity> { };
            int focusedRow = lstNV.Count;

            if (!string.IsNullOrEmpty(_manv))
            {
                NhanVienEntity nvEntity = new NhanVienEntity
                (
                    _manv,
                    _tennv,
                    textEdit2.Text.ToString(),
                    searchlookupEditPB.EditValue.ToString(),
                    textEdit41.Text.ToString(),
                    _ten,
                    //truyền tham số cho 4 cột (Người tạo, ngày tạo, người sửa, ngày sửa)
                    currentUser,
                    DateTime.Now,
                    null,
                    null
                 );
                bool existsInList = lstNV.Any(existing =>
                    existing.MaNV == nvEntity.MaNV
                );
                if (existsInList)
                {
                    XtraMessageBox.Show($"Nhân Viên '{nvEntity.MaNV}' đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                bool existsInAddList = lstaddNV.Any(existing =>
                  existing.MaNV == nvEntity.MaNV
               );

                if (!existsInAddList)
                {
                    lstaddNV.Add(nvEntity);
                    lstRowUpdate.Add(focusedRow);
                }

                focusedRow += 1;
            }
            lstNV.AddRange(lstaddNV);
            gridControl1.DataSource = null;
            gridControl1.DataSource = lstNV;
            gridView1.FocusedRowHandle = lstNV.Count - 1;
            lstaddNV.Clear();
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            textEdit11.EditValue = "";
            textEdit3.EditValue = "";
            searchlookupEditPB.EditValue = "";
            textEdit2.EditValue = "";
            textEdit41.EditValue = "";
            textEdit1.EditValue = "";
            gridColumn1.OptionsColumn.AllowEdit = false;
            gridColumn1.OptionsColumn.ReadOnly = true;
        }

        private void textEdit11_KeyPress(object sender, KeyPressEventArgs e)
        {
            //int ascii = Convert.ToInt32(e.KeyChar);
            //if (!(ascii == 58))
            //{
            //    if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            //    {
            //        e.Handled = true; // Chặn ký tự nhập vào
            //    }
            //}
        }

        private void textEdit3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            //{
            //    e.Handled = true; // Chặn ký tự đặc biệt
            //}
        }

        private void textEdit4_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            //{
            //    e.Handled = true; // Chặn ký tự đặc biệt
            //}
        }

        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            //{
            //    e.Handled = true; // Chặn ký tự đặc biệt
            //}
        }

        private void textEdit41_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            //{
            //    e.Handled = true; // Chặn ký tự đặc biệt
            //}
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "UrlAnh" && e.IsGetData)
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (view == null) return;

                    // ✅ Lấy object trực tiếp từ DataSource (List<NhanVienEntity>)
                    var dataSource = view.DataSource as List<NhanVienEntity>;
                    if (dataSource == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= dataSource.Count)
                        return;

                    var nv = dataSource[e.ListSourceRowIndex];
                    if (nv == null || string.IsNullOrEmpty(nv.HinhAnh))
                    {
                        e.Value = null;
                        return;
                    }

                    string urlHost = (string)settingsReader.GetValue("HostDH", typeof(String));
                    string url = urlHost + "/Images/NhanVien/" + nv.HinhAnh;

                    try
                    {
                        using (var wc = new WebClient())
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
            catch { }
        }

        private async void gridView1_DoubleClick(object sender, EventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var fieldName = view.FocusedColumn.FieldName;

            if (fieldName == "HinhAnh")
            {
                var nv = view.GetFocusedRow() as NhanVienEntity;
                if (nv == null) return;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = ofd.SafeFileName;
                    bool uploaded = await UploadImage("", ofd.FileName, fileName);
                    if (uploaded)
                    {
                        nv.HinhAnh = fileName;
                        view.RefreshRow(view.FocusedRowHandle);
                        lstRowUpdate.Add(view.FocusedRowHandle);
                        LuuDong();
                    }
                }
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
                    if (gridView1.FocusedRowHandle >= 0)
                    {
                        string maNv = gridView1.GetFocusedRowCellValue(gridColumnID).ToString();
                        string url = URL + $"NhanVien/DeleteNV?manv={maNv}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                            LoadDSNV(false);
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridView1_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            if (e.RowHandle == GridControl.AutoFilterRowHandle)
            {
                e.RowHeight = 30;
                return;
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {
                e.RowHeight = 30;
                return;
            }
            e.RowHeight = 70;
        }



        //private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        //{
        //    switch (status)
        //    {
        //        case ResourceURL.EventStatus.View:
        //            gridView1.OptionsBehavior.Editable = false;

        //            if (_allowAdd)
        //            {
        //                Them.Enabled = true;
        //                actionControlAdd.Enabled = true;
        //            }

        //            if (_allowEdit)
        //            {
        //                Sua.Enabled = true;
        //                actionControlEdit.Enabled = true;
        //            }

        //            if (_allowDelete)
        //            {
        //                Xoa.Enabled = true;
        //                actionControlDelete.Enabled = true;
        //            }

        //            if (_allowAdd || _allowEdit)
        //            {
        //                Luu.Enabled = false;
        //                actionControlSave.Enabled = false;
        //            }

        //            break;
        //        case ResourceURL.EventStatus.Edit:
        //            gridView1.OptionsBehavior.Editable = true;
        //            if (_allowAdd)
        //            {
        //                Them.Enabled = false;
        //                actionControlAdd.Enabled = false;
        //            }
        //            if (_allowEdit)
        //            {
        //                Sua.Enabled = false;
        //                actionControlEdit.Enabled = false;
        //            }

        //            if (_allowDelete)
        //            {
        //                Xoa.Enabled = false;
        //                actionControlDelete.Enabled = false;
        //            }

        //            if (_allowAdd || _allowEdit)
        //            {
        //                Luu.Enabled = true;
        //                actionControlSave.Enabled = true;
        //            }
        //            break;
        //        case ResourceURL.EventStatus.Add:
        //            gridView1.OptionsBehavior.Editable = true;
        //            if (_allowAdd)
        //            {
        //                Them.Enabled = false;
        //                actionControlAdd.Enabled = false;
        //            }

        //            if (_allowEdit)
        //            {
        //                Sua.Enabled = false;
        //                actionControlEdit.Enabled = false;
        //            }
        //            if (_allowDelete)
        //            {
        //                Xoa.Enabled = false;
        //                actionControlDelete.Enabled = false;
        //            }
        //            Sua.Enabled = false;
        //            if (_allowAdd || _allowEdit)
        //            {
        //                Luu.Enabled = true;
        //                actionControlSave.Enabled = true;
        //            }

        //            break;
        //    }
        //}


        private async Task<bool> UploadImage(string action, string sourceFileName, string fileName)
        {
            string UrlUpload = string.Format("{0}/NhanVien/UploadImage", URL); // API upload riêng cho nhân viên
            var client = new WebClient();
            try
            {
                client.Headers.Add("action", action);
                client.Headers.Add("fileName", fileName);
                var data = File.ReadAllBytes(sourceFileName);
                var responseBytes = await client.UploadDataTaskAsync(new Uri(UrlUpload), data);
                string result = Encoding.UTF8.GetString(responseBytes);
                return result.ToUpper() == "TRUE";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                // e.ListSourceRowIndex là vị trí thật trong data source
                if (e.ListSourceRowIndex >= 0)
                    e.DisplayText = (e.ListSourceRowIndex + 1).ToString();
            }
            #region Thoai hiển thị username -> Tên
            if (e.Column == gridColumnNguoiTao || e.Column == gridColumnNguoiSua)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && lstNV != null)
                {
                    var nv = lstNV.FirstOrDefault(x => !string.IsNullOrEmpty(x.UserID) &&
                            x.UserID.Equals(username, StringComparison.OrdinalIgnoreCase));
                    if(nv != null && !string.IsNullOrEmpty(nv.TenNV))
                    {
                        e.DisplayText = nv.Ten;
                    }
                    else { e.DisplayText = username; }
                }
            }
            #endregion
        }

        private void CreateSearchLookupPB()
        {
            string url = string.Format("{0}?", URL + "PhongBan/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbpb = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbpb;
            rCountryEdit.DisplayMember = "TenPB";
            rCountryEdit.ValueMember = "MaPB";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaPB", Caption = "Mã Phòng Ban", Name = "colMaHang", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenPB", Caption = "Tên Phòng Ban", Name = "colTenHang", Visible = true });

            }
            gridColumnPB.ColumnEdit = rCountryEdit;
            searchlookupEditPB.Properties.DataSource = tbpb;
            searchlookupEditPB.Properties.ValueMember = "MaPB";
            searchlookupEditPB.Properties.DisplayMember = "TenPB";


        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            #region Thoai hiển thị tên nv
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.RowHandle < 0) return;
            NhanVienEntity item = view.GetRow(e.RowHandle) as NhanVienEntity;
            if (item == null) return;
            var currentUser = GlobleData.UserName;
            if (string.IsNullOrEmpty(currentUser)) return;

            if (item.ID == 0 || item.ID == null)
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
        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd)
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == gridColumnMNV)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;

                }
            }
        }
        private void SetupUI()
        {
            // Tạo PictureEdit repository
            var riPicture = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            riPicture.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            riPicture.ShowMenu = true;
            riPicture.CustomHeight = 50;
            riPicture.NullText = " ";

            gridColumn2.ColumnEdit = riPicture;
            gridColumn2.MinWidth = 100;
            gridColumn2.Width = 100;
            gridColumn2.Width = 100;
            gridColumn2.MinWidth = 100;

            gridColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Object;

        }
    }
}