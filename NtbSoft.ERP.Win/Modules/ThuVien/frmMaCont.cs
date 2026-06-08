using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmMaCont : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;

        List<MaContEntity> lstMaCont;
        List<int> lstRowUpdate = new List<int>();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

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


        public frmMaCont()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstMaCont = new List<MaContEntity>();
        }

        protected override void OnLoad(EventArgs e)
        {

            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadMaCont();
        }


        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
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

        public void ThemDong()
        {
            string MaCont = string.Empty;
            if (lstMaCont.Count == 0)
            {
                MaCont = "1";
            }
            else
            {
                DataTable objMaCont = new DataTable();
                string url = string.Format("{0}?Action={1}", URL + "MaCont/Get", "GETMaCont");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    objMaCont = JsonConvert.DeserializeObject<DataTable>(json);
                    MaCont = objMaCont.Rows[0]["MaCont"].ToString();
                }
            }



            List<MaContEntity> lst = this.grcMaCont.DataSource as List<MaContEntity>;
            MaContEntity obj = new MaContEntity();
        
            obj.NVTao = GlobleData.UserName;
            obj.MaCont = $"Cont_{MaCont}";

            lstMaCont.Add(obj);
            _rowAdd = grvMaCont.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            grvMaCont.FocusedRowHandle = _rowAdd;

            grcMaCont.RefreshDataSource();
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }

        public void SuaDong()
        {
            if (lstMaCont.Count == 0)
            {
                return;
            }
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);

        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        public void XoaDong()
        {
            try
            {
                if (lstMaCont.Count == 0)
                {
                    return;
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    MaContEntity row = grvMaCont.GetRow(grvMaCont.FocusedRowHandle) as MaContEntity;
                    string url = string.Format("{0}?Parameter={1}", URL + "MaCont/Delete", row.MaCont);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadMaCont();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        public void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && grvMaCont.FocusedRowHandle >= 0)
                {
                    FocusedIndex = grvMaCont.FocusedRowHandle;
                }
                List<MaContEntity> _lstUpdate = new List<MaContEntity>();

                MaContEntity row = grvMaCont.GetRow(grvMaCont.FocusedRowHandle) as MaContEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                //lstRowUpdate.Sort();
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    MaContEntity item = grvMaCont.GetRow(lstRowUpdate[i]) as MaContEntity;

                    if (item != null && !string.IsNullOrEmpty(item.MaCont) && !string.IsNullOrEmpty(item.TenCont))
                    {

                        _lstUpdate.Add(item);

                    }
                    else
                    {
                        XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate.Count == 0))
                {
                    return;
                }


                string url = string.Format("{0}?", URL + "MaCont/Post");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
                //MessageBox.Show("Lưu thành công");

                //
                //string url = string.Format("{0}?", URL + "BangMau/PostBangMau");
                //msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstBangMau); }).Result;
                //
                if (msResult.ToLower() == "true")
                {
                    LoadMaCont();
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

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        public void NapLaiDong()
        {
            LoadMaCont();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

       

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    grvMaCont.OptionsBehavior.Editable = false;

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
                    grvMaCont.OptionsBehavior.Editable = true;
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
                    grvMaCont.OptionsBehavior.Editable = true;
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

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status.Equals(ResourceURL.EventStatus.Add))
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd && view.FocusedColumn == colTenCont)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }

               
            }
            else if (_status.Equals(ResourceURL.EventStatus.Edit))
            {
                if (view.FocusedColumn == colTenCont)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }

            }
        }



        private void grvMaCont_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvMaCont_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvMaCont_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            if(grvMaCont.FocusedColumn != null)
            {
                e.Valid = true;
                if (grvMaCont.FocusedColumn == colMaCont)
                {
                    List<MaContEntity> lstMaCont = grcMaCont.DataSource as List<MaContEntity>;
                    //var query = lstMaCont.FirstOrDefault(x => x.MaCont == e.Value.ToString());                  
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.ErrorText = "Vui lòng nhập mã Cont";
                        e.Valid = false;
                    //}else if(query?.MaCont != null)
                    //{
                    //    e.ErrorText = $"Mã Cont đã trùng với Cont :{query?.TenCont}";
                    //    e.Valid = false;
                    }
                }
                else if (grvMaCont.FocusedColumn == colTenCont)
                {
                    var query = lstMaCont.FirstOrDefault(x => x.TenCont?.ToLower()?.Replace(" ","") == e.Value.ToString().ToLower().Replace(" ", ""));
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.ErrorText = "Vui lòng nhập Tên Cont";
                        e.Valid = false;
                    }
                    else if (query?.TenCont != null)
                    {
                        e.ErrorText = $"Cont đã trùng với {query?.TenCont}";
                        e.Valid = false;
                    }
                }

            }
        }

        private void grvMaCont_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }

        private void LoadMaCont()
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                string url = string.Format("{0}?Action={1}", URL + "MaCont/Get", "GET");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstMaCont = JsonConvert.DeserializeObject<List<MaContEntity>>(json);
                }
                grcMaCont.DataSource = lstMaCont;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void grvMaCont_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }

        ///styte grvMaCont
        ///

        private void gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }



    }
}