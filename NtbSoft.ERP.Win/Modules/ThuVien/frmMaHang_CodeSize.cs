using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
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

    public partial class frmMaHang_CodeSize : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;

        List<MaHang_CodeSizeEntity> lstMaHangUpdate;
        List<MaHangConfigEntity> lstMaHangView;
        List<int> lstRowUpdate = new List<int>();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        bool _allowAdd = true, _allowEdit = true, _allowDelete = true;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;
        string MaHang = string.Empty;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        public frmMaHang_CodeSize()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstMaHangUpdate = new List<MaHang_CodeSizeEntity>();
            lstMaHangView = new List<MaHangConfigEntity>();

        }

        protected override void OnLoad(EventArgs e)
        {

            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadMaHang();


        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }



        private List<ActionControl> InitActionKeyDown()
        {
        
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                
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

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    grvMaHangCT.OptionsBehavior.Editable = false;
                
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
                    grvMaHangCT.OptionsBehavior.Editable = true;
                    

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
                    grvMaHangCT.OptionsBehavior.Editable = true;
            
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



        public void LoadMaHang()
        {
            try
            {
                //_status = ResourceURL.EventStatus.View;
                //GridViewUpdateStatus(_status);
                DataTable dtMaHang = new DataTable();      
                string url = $"{URL}MaHang_CodeSize/Get?Action=GetMaHang&Para1=para1&Para2=para2s";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    dtMaHang = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcMaHang.DataSource = dtMaHang;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadMHDetail(string MaHang)
        {
            try
            {

                //_status = ResourceURL.EventStatus.View;
                //GridViewUpdateStatus(_status);
                grvMaHang.FocusedRowHandle = FocusedIndex;
                string url = $"{URL}MaHang_CodeSize/Get?Action=Get&Para1={MaHang}&Para2=para2s";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstMaHangView = JsonConvert.DeserializeObject<List<MaHangConfigEntity>>(json);
                }
                
                if(lstMaHangView?.Count==0)
                {
                    this.Xoa.Enabled = false;
                }
                grcMaHangCT.DataSource = lstMaHangView.Count > 0 ? lstMaHangView : null;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NapLaiDong()
        {
            LoadMHDetail(MaHang);
        }

        public string GetCodeSize(string CodeSize)
        {
            try
            {
                string msg = string.Empty;
                List<MaHangConfigEntity> lst = new List<MaHangConfigEntity>();
                string url = $"{URL}MaHang_CodeSize/Get?Action=GetCodeSize&Para1={CodeSize}&Para2=para2s";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lst = JsonConvert.DeserializeObject<List<MaHangConfigEntity>>(json);
                }
                
             if(lst.Count > 0)
                {
                    msg = $"CodeSize đã trùng với  màu {lst[0].TenMau}  mã hàng {lst[0].MaHang}";
                }

             return msg;

            }
            catch (Exception ex)
            {
                return ex.Message;

                //XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void focused(object sender)
        {
          
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
          

            if (view.FocusedRowHandle >= 0 && view.FocusedColumn == colCodeSize)
            {
                view.FocusedColumn.OptionsColumn.AllowEdit = true;
            }
            else
            {
                view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
        }

        private void LuuDong()
        {
            this.ActiveControl = this.button1;
            List<MaHangConfigEntity> lstview = grcMaHangCT.DataSource as List<MaHangConfigEntity>;

            var query = lstview.Where(x => !string.IsNullOrEmpty(x.CodeSize)).ToList();

            if (query?.Count == 0)
            {
                return;
            }
            foreach(var item in query)
            {
                lstMaHangUpdate.Add(new MaHang_CodeSizeEntity
                {
                    ID = 0,
                    MaHang = item.MaHang,
                    MaMau = item.MaMau,
                    CodeMau = item.CodeMau,
                    CodeSize = item.CodeSize,
                    MaSize = item.MaSize
                });
            }
          
            string msResult = "";
            if (lstMaHangUpdate?.Count == 0)
            {
                return;
            }


            string url = string.Format("{0}", URL + "MaHang_CodeSize/Post");
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstMaHangUpdate); }).Result;
          
            
            if (msResult.ToLower() == "true")
            {
                LoadMHDetail(MaHang);
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            else XtraMessageBox.Show(msResult);
           
            lstMaHangUpdate.Clear();

        }
        private void XoaDong()
        {
            try
            {
                if (lstMaHangView.Count == 0)
                {
                    return;
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    MaHangConfigEntity row = grvMaHangCT.GetFocusedRow() as MaHangConfigEntity;
                    string url = $"{URL}MaHang_CodeSize/Delete?Para1={row.MaHang}&Para2={row.MaMau}&Para3={row.MaSize}";
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadMHDetail(MaHang);
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

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void grvMaHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            MaHang = grvMaHang.GetFocusedValue()?.ToString();
            
            if (e.FocusedRowHandle >= 0 && !string.IsNullOrEmpty(MaHang))
            {
                //clsWaitForm.ShowWaitForm(this, 2000);
                FocusedIndex = e.FocusedRowHandle;
                LoadMHDetail(MaHang);

            }
        }

        private void grvMaHangCT_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvMaHangCT_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }


        #region Styte grview
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

        private void grvMaHangCT_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }

        private void grvMaHangCT_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {

            e.Valid = true;
            if (grvMaHangCT.FocusedColumn == colCodeSize)
            {
                List<MaHangConfigEntity> lstMaHangCT = grcMaHangCT.DataSource as List<MaHangConfigEntity>;
                var query=lstMaHangCT.FirstOrDefault(x=>x.CodeSize?.ToString()==e.Value?.ToString());
                //string msg = GetCodeSize(e.Value?.ToString());


                if (query !=null)
                {
                    e.ErrorText = $"Code Size đã trùng với màu {query.TenMau}";
                    e.Valid = false;

                }
                //else if (!string.IsNullOrEmpty(msg))
                //{
                //    e.ErrorText = msg;
                //    e.Valid = false;
                //}

            }
          
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

        private void grvMaHangCT_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
        //    _status = ResourceURL.EventStatus.Edit;
        //    GridViewUpdateStatus(_status);
        }

        private void grvMaHang_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && e.RowHandle == grvMaHang.FocusedRowHandle)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("#E6FBFF");
                e.HighPriority = true;
            }
        }
        private void grvMaHangCT_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && e.RowHandle == grvMaHangCT.FocusedRowHandle)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("#EFDECD");
                e.HighPriority = true;
            }
        }

        #endregion


    }
}