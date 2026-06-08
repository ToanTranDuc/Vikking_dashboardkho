using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NtbSoft.ERP.Entity.Kho;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Properties;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmTienTe : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        DataTable tbl;
        DataTable tbltiente;
        DataTable tblquydoi;

        List<int> lstRowUpdatett = new List<int>();

        List<int> lstRowUpdateqd = new List<int>();
        List<TienTeEntity> lstUpdateTT = new List<TienTeEntity>();
        List<QuyDoiTienTeEntity> lstUpdateQD = new List<QuyDoiTienTeEntity>();
        BindingList<TienTeEntity> _listDataTT;
        BindingList<QuyDoiTienTeEntity> _listDataQD;

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        int _rowAdd2 = -1;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;

        List<ActionControl> lstActionControls;
        Bitmap dragBitmap = null; // Bitmap để lưu hình ảnh của dòng
        private int draggedRowHandle1 = GridControl.InvalidRowHandle; // Khai báo biến cấp lớp
        bool isDragging = false;
        bool isEditing = false;

        public frmTienTe()
        {
            InitializeComponent();

            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            tbl = new DataTable();
            _listDataTT = new BindingList<TienTeEntity>();
            _listDataQD = new BindingList<QuyDoiTienTeEntity>();
        }


        protected override void OnLoad(EventArgs e)
        {
            LoadTienTe();
            //CreatesearchlookupQD();
        }


        private async void LoadTienTe()
        {
            try
            {
                string url = $"{URL}TIENTE/Get?action=GETTIENTE&para=&para2=&para3=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    //_listDataTT = JsonConvert.DeserializeObject<List<TienTeEntity>>(json);
                    var list = JsonConvert.DeserializeObject<List<TienTeEntity>>(json);
                    _listDataTT = new BindingList<TienTeEntity>(list);
                    tbltiente = JsonConvert.DeserializeObject<DataTable>(json);
                }

                gridControl1.DataSource = _listDataTT;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            string maTienTe = gridView1.GetFocusedRowCellValue("TienTeID")?.ToString();
            LoadQuyDoi(maTienTe);
        }

        private async void LoadQuyDoi(string matiente)
        {
            try
            {
                string url = $"{URL}TIENTE/Get?action=GETQUYDOI&para={matiente}&para2=&para3=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    gridControl2.DataSource = null;
                }

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonConvert.DeserializeObject<List<QuyDoiTienTeEntity>>(json);
                    _listDataQD = new BindingList<QuyDoiTienTeEntity>(list);
                    tblquydoi = JsonConvert.DeserializeObject<DataTable>(json);
                }

                gridControl2.DataSource = _listDataQD;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDongTT();
        }

        private void ThemDongTT()
        {
            TienTeEntity obj = new TienTeEntity();
            obj.ID = 0;
            _listDataTT.Add(obj);
            _rowAdd = gridView1.RowCount - 1;
            gridView1.FocusedRowHandle = _rowAdd;
            gridView1.MakeRowVisible(gridView1.FocusedRowHandle);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDongQD();
        }

        private void ThemDongQD()
        {
            QuyDoiTienTeEntity objqd = new QuyDoiTienTeEntity();
            TienTeEntity itemtt = gridView1.GetRow(gridView1.FocusedRowHandle) as TienTeEntity;
            objqd.ID = 0;
            objqd.MaTienTe = itemtt.TienTeID;
            objqd.MaTT = itemtt.MaTienTe;
            objqd.Gia = 0;
            objqd.Ngay = DateTime.Now;
            _listDataQD.Add(objqd);
            _rowAdd = gridView2.RowCount - 1;
            gridView2.FocusedRowHandle = _rowAdd;
            gridView2.MakeRowVisible(gridView2.FocusedRowHandle);
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {

        }

        private void gridView2_ShowingEditor(object sender, CancelEventArgs e)
        {

        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadTienTe();

            string maTienTe = gridView1.GetFocusedRowCellValue("TienTeID")?.ToString();
            if (!string.IsNullOrWhiteSpace(maTienTe))
            {
                LoadQuyDoi(maTienTe);
            }
        }

        

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private async void LuuDong()
        {
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ResourceForm.frmWait));
                this.ActiveControl = Button1;
                TienTeEntity row = gridView1.GetRow(gridView1.FocusedRowHandle) as TienTeEntity;
                lstRowUpdatett = lstRowUpdatett.Distinct().OrderBy(x => x).ToList();
                //lstRowUpdateqd = lstRowUpdateqd.Distinct().OrderBy(y => y).ToList();

                if (lstRowUpdatett.Count > 0)
                {
                    for (int i = 0; i < lstRowUpdatett.Count; i++)
                    {

                        TienTeEntity item = gridView1.GetRow(lstRowUpdatett[i]) as TienTeEntity;
                        if (item == null)
                        {
                            return;
                        }
                        if (CheckDuplicated(gridView1, item.MaTienTe, lstRowUpdatett[i]))
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                            XtraMessageBox.Show($"Tiền Tệ '{item.MaTienTe}' đã tồn tại.!",
                                "Dữ liệu trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (item != null)
                        {
                            lstUpdateTT.Add(item);

                        }
                    }
                }



                if (lstRowUpdateqd.Count > 0)
                {
                    for (int i = 0; i < lstRowUpdateqd.Count; i++)
                    {
                        QuyDoiTienTeEntity item2 = gridView2.GetRow(lstRowUpdateqd[i]) as QuyDoiTienTeEntity;
                        if (item2 != null)
                        {
                            if (CheckDuplicated1(gridView2, item2.Ngay, lstRowUpdateqd[i]))
                            {
                                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                                XtraMessageBox.Show($"Giá ngày'{item2.Ngay.ToString("dd/MM/yyyy")}' đã được thêm.!",
                                    "Dữ liệu trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (item2 != null)
                            {
                                item2.UserID = GlobleData.UserName;
                                item2.NguoiSua = GlobleData.UserName;
                                lstUpdateQD.Add(item2);

                            }
                        }

                    }
                }

                string mgTT = string.Empty;
                string mgQD = string.Empty;
                if (lstUpdateTT != null && lstUpdateTT.Count > 0)
                {
                    clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_TIENTE");
                    mgTT = await _clientExtension.PostAsync(URL + $"TIENTE/Post?action=POSTTIENTE", lstUpdateTT);
                    if (mgTT == string.Empty)
                    {
                        clsWaitForm.ShowErrorForm(this, 3000);
                    }
                    lstRowUpdatett.Clear();
                    lstUpdateTT.Clear();
                }

                if (lstUpdateQD != null && lstUpdateQD.Count > 0)
                {
                    clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_QUYDOITIENTE");
                    mgQD = await _clientExtension.PostAsync(URL + $"TIENTE/Post2?action=POSTQUYDOI", lstUpdateQD);
                    if (mgQD == string.Empty)
                    {
                        clsWaitForm.ShowErrorForm(this, 3000);
                    }
                    lstRowUpdateqd.Clear();
                    lstUpdateQD.Clear();
                }

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                //if (string.Compare("True", mg) != 0)
                //if (mgTT == string.Empty || mgQD == string.Empty)
                //{
                //    clsWaitForm.ShowErrorForm(this, 3000);
                //}
                if (mgQD == "True" || mgTT == "True")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                }
                _status = ResourceURL.EventStatus.View;

            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            LoadTienTe();
        }


        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DeletedRow();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa dữ liệu lỗi!Vui lòng thử lại", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        private async void DeletedRow()
        {
            object Id = gridView1.GetFocusedRowCellValue(colID);
            object matt = gridView1.GetFocusedRowCellValue("TienTeID")?.ToString();
            string urlGET = $"{URL}TIENTE/Get?Action=GetCheckDeleteTienTe&para={matt}&para2=&para3=";
            string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
            if (jsonGET != "[]")
            {
                XtraMessageBox.Show("Tiền tệ này đã được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                //DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ResourceForm.frmWait));

                string url = string.Format("{0}?action=DeleteTIENTIE&id={1}&matiente={2}", URL + "TIENTE/Delete", Id, matt);
                if (Id == null) return;

                try
                {
                    string ss = await _clientExtension.DeletedAsync(url);
                    if (string.Compare(ss, "True") != 0)
                        XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception e)
                {
                    return;
                }
                //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                LoadTienTe();

                string maTienTe = gridView1.GetFocusedRowCellValue("TienTeID")?.ToString();
                if (!string.IsNullOrWhiteSpace(maTienTe))
                {
                    LoadQuyDoi(maTienTe);
                }
            }
        }


        private void gridView2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            var menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridView2.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();
                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {
                        //DevExpress.Utils.Menu.DXMenuItem menuCoppyPasteItem = new DevExpress.Utils.Menu.DXMenuItem("▲ Up", ItemUP_Click);
                        var menudelete = new DevExpress.Utils.Menu.DXMenuItem("Xóa Giá", DeleteGia);
                        //e.Menu.Items.Add(menuCoppyPasteItem);
                        e.Menu.Items.Add(menudelete);

                    }

                }
            }
        }

        private async void DeleteGia(object sender, EventArgs e)
        {
            object Id = gridView2.GetFocusedRowCellValue(colID);
            object quydoiid = gridView2.GetFocusedRowCellValue(colQuyDoiID);
            string urlGET = $"{URL}TIENTE/Get?Action=GetCheckDeleteQuyDoi&para={quydoiid}&para2=&para3=";
            string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
            if (jsonGET != "[]")
            {
                XtraMessageBox.Show("Giá tiền này đã được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                //DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ResourceForm.frmWait));

                string url = string.Format("{0}?action=DeleteQUYDOI&id={1}&matiente={2}", URL + "TIENTE/Delete", Id, "");
                if (Id == null) return;

                try
                {
                    string ss = await _clientExtension.DeletedAsync(url);
                    if (string.Compare(ss, "True") != 0)
                        XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ev)
                {
                    return;
                }
                //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                LoadTienTe();

                string maTienTe = gridView1.GetFocusedRowCellValue("TienTeID")?.ToString();
                if (!string.IsNullOrWhiteSpace(maTienTe))
                {
                    LoadQuyDoi(maTienTe);
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            object Id = gridView2.GetFocusedRowCellValue(colID);
            object quydoiid = gridView2.GetFocusedRowCellValue(colQuyDoiID);
            string urlGET = $"{URL}TIENTE/Get?Action=GetCheckDeleteQuyDoi&para={quydoiid}&para2=&para3=";
            string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
            if (jsonGET != "[]")
            {
                XtraMessageBox.Show("Giá tiền này đã được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                string url = string.Format("{0}?action=DeleteQUYDOI&id={1}&matiente={2}", URL + "TIENTE/Delete", Id, "");
                if (Id == null) return;

                try
                {
                    string ss = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (string.Compare(ss, "True") != 0)
                        XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ev)
                {
                    return;
                }
                LoadTienTe();

                string maTienTe = gridView1.GetFocusedRowCellValue("TienTeID")?.ToString();
                if (!string.IsNullOrWhiteSpace(maTienTe))
                {
                    LoadQuyDoi(maTienTe);
                }
            }
           
        }

        private void gridView2_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Ngay" && e.Value != null && e.Value is DateTime dt)
            {
                e.DisplayText = dt.ToString("dd/MM/yyyy");
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

        }

        private async void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            object quydoiid = gridView2.GetFocusedRowCellValue(colQuyDoiID);
            string urlGET = $"{URL}TIENTE/Get?Action=GetCheckDeleteQuyDoi&para={quydoiid}&para2=&para3=";
            string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
            if (jsonGET != "[]")
            {
                XtraMessageBox.Show("Giá tiền này đã được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
          
                //DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ResourceForm.frmWait));
               object Id = gridView2.GetFocusedRowCellValue(colID);
            string url = string.Format("{0}?action=DeleteQUYDOI&id={1}&matiente={2}", URL + "TIENTE/Delete", Id, "");
            if (Id == null) return;

            try
            {
                string ss = await _clientExtension.DeletedAsync(url);
                if (string.Compare(ss, "True") != 0)
                    XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ev)
            {
                return;
            }
            //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            LoadTienTe();

            string maTienTe = gridView1.GetFocusedRowCellValue("TienTeID")?.ToString();
            if (!string.IsNullOrWhiteSpace(maTienTe))
            {
                LoadQuyDoi(maTienTe);
            }
        }

        private bool CheckDuplicated1(GridView gridView, DateTime ngayKiemTra, int currentRowHandle)
        {
            for (int i = 0; i < gridView.RowCount; i++)
            {
                if (i == currentRowHandle) continue; // Bỏ qua dòng hiện tại

                object objNgay = gridView.GetRowCellValue(i, "Ngay");
                if (objNgay != null && DateTime.TryParse(objNgay.ToString(), out DateTime ngay))
                {
                    if (ngay.Date == ngayKiemTra.Date)
                    {
                        return true; // Trùng ngày
                    }
                }
            }

            return false; // Không trùng
        }

        private bool CheckDuplicated(GridView gridView, string maTienTe, int currentRowHandle)
        {
            for (int i = 0; i < gridView.RowCount; i++)
            {
                if (i == currentRowHandle) continue;

                object objMaTienTe = gridView.GetRowCellValue(i, "MaTienTe");
                if (objMaTienTe != null && objMaTienTe.ToString().Trim().Equals(maTienTe.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Trùng mã tiền tệ
                }
            }

            return false; // Không trùng
        }

        /*Write log when modify row*/
        List<LogThuvienEntity> lstLog = new List<LogThuvienEntity>();

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdatett.Add(e.RowHandle);

            TienTeEntity row_focus = gridView1.GetFocusedRow() as TienTeEntity;
            if (row_focus != null && row_focus.ID > 0)
            {
                string content = clsWriteLogThuVienLib.FormatRow(row_focus);
                var Query = lstLog.FirstOrDefault(x => x.ID == row_focus.ID);
                if (Query != null)
                {
                    Query.Content = content;
                }
                else
                {
                    lstLog.Add(new LogThuvienEntity
                    {
                        ID = row_focus.ID,
                        Action = $"Sửa {this.Text}",
                        Module = this.Name,
                        Content = content,
                        UserID = GlobleData.UserName,
                        CreatedDate = DateTime.Now

                    });
                }
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdateqd.Add(e.RowHandle);

            QuyDoiTienTeEntity row_focus = gridView2.GetFocusedRow() as QuyDoiTienTeEntity;
            if (row_focus != null && row_focus.ID > 0)
            {
                string content = clsWriteLogThuVienLib.FormatRow(row_focus);
                var Query = lstLog.FirstOrDefault(x => x.ID == row_focus.ID);
                if (Query != null)
                {
                    Query.Content = content;
                }
                else
                {
                    lstLog.Add(new LogThuvienEntity
                    {
                        ID = row_focus.ID,
                        Action = $"Sửa {this.Text}",
                        Module = this.Name,
                        Content = content,
                        UserID = GlobleData.UserName,
                        CreatedDate = DateTime.Now

                    });
                }
            }

        }
    }
}