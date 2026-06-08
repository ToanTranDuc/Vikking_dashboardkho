using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraLayout.Utils;
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
    public partial class frmMaSeal : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;

        List<MaSealEntity> lstMaSeal;
        List<int> lstRowUpdate = new List<int>();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        private string _username = string.Empty;

        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;
        DataTable dtKho;

        KeyDownControlHandler keyDownControlHandler;

        //ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        public frmMaSeal()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            dtKho = new DataTable();
            lstMaSeal = new List<MaSealEntity>();
        }
        protected override void OnLoad(EventArgs e)
        {
            _username = GlobleData.UserName;
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            CreateSearchLoockupDefault();
            LoadMaSeal();
        }

        private List<ActionControl> InitActionKeyDown()
        {
            //actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(MutiDelete, _allowDelete, ActionType.Delete, this.Xoa1.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, _username);
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
                Xoa1.Enabled = false;

        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    //grvMaSeal.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        //actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa1.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    grvMaSeal.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        //actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa1.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    grvMaSeal.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        //actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa1.Enabled = false;
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

        private void InitMaKhoEdit(DataTable dtKho)
        {

            RepositoryItemSearchLookUpEdit rKhoEdit = new RepositoryItemSearchLookUpEdit();
            rKhoEdit.DataSource = dtKho;
            rKhoEdit.DisplayMember = "KhoView";
            rKhoEdit.ValueMember = "MaKho";
            rKhoEdit.ShowClearButton = false;
            rKhoEdit.NullText = "---Chưa Chọn Kho---";
            //rKhoEdit.EditValueChanged += rKhoEdit_EditValueChanged;
            GridView dvView = rKhoEdit.View;

            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKho", Caption = " Mã Kho", Name = "rColMaKho", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKho", Caption = "Tên Kho", Name = "rColKho", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "DVSX", Caption = "DVSX", Name = "rColDVSX", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "KhoView", Caption = "Tên Kho + DVSX", Name = "rColKhoView", Visible = false });
                dvView.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(gridView_CustomDrawColumnHeader);

            }

            colKho.ColumnEdit = rKhoEdit;


        }

        private void StyleCbx()
        {

            cbxLoaiSeal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor; 
            cbxLoaiSeal.SelectedIndex = 0;


            cbxLoaiSeal.Properties.Appearance.Font = new Font("Tahoma", 8, FontStyle.Bold);
            cbxLoaiSeal.Properties.Appearance.ForeColor = ColorTranslator.FromHtml("#FF6619");

            cbxLoaiSeal.Properties.AppearanceDropDown.Font = new Font("Tahoma", 8);
            cbxLoaiSeal.Properties.AppearanceDropDown.BackColor = Color.White;


            cbxLoaiSeal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            cbxLoaiSeal.Size = new Size(200, 30);

            
        }

        private DataTable LoadMaKho()
        {
            dtKho = new DataTable();
            string url = string.Format("{0}", URL + $"MaSeal/GetKho?username={_username}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                dtKho = JsonConvert.DeserializeObject<DataTable>(json);
            }
            return dtKho;
        }

        private void setDefaultSeachLookup()
        {
            //txtSoBD.Properties.DisplayFormat.FormatType = FormatType.Custom;
            //txtSoBD.Properties.DisplayFormat.FormatString = "SealFormat";
            //txtSoBD.Properties.DisplayFormat.Format = new SealFormat();
            txtSoBD.EditValue = 0;

            txtSL.EditValue = 0;

            //spinEditSoKetThuc.Properties.DisplayFormat.FormatType = FormatType.Custom;
            //spinEditSoKetThuc.Properties.DisplayFormat.FormatString = "SealFormat";
            //spinEditSoKetThuc.Properties.DisplayFormat.Format = new SealFormat();
            spinEditSoKetThuc.EditValue = 0;

            cbxLoaiSeal.EditValue = null;
            searchLookUpEdit_Kho.EditValue = null;
        }

        private void CreateSearchLoockupDefault()
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("The Bezier");
            DataTable dtKho = LoadMaKho();
            searchLookUpEdit_Kho.Properties.DataSource = dtKho;
            searchLookUpEdit_Kho.Properties.DisplayMember = "KhoView";
            searchLookUpEdit_Kho.Properties.ValueMember = "MaKho";
            searchLookUpEdit_Kho.Properties.ShowClearButton = false;
            searchLookUpEdit_Kho.ForeColor = ColorTranslator.FromHtml("#FF6619");
            searchLookUpEdit_Kho.Font = new Font("Tahoma", 8, FontStyle.Bold);


            InitMaKhoEdit(dtKho);

            //cbxLoaiSeal.Properties.Items.Clear();
            //cbxLoaiSeal.Properties.Items.Add("Nội bộ");
            //cbxLoaiSeal.Properties.Items.Add("Xuất hàng");

            lookUp_DVSX.ForeColor = ColorTranslator.FromHtml("#FF6619");
            lookUp_DVSX.Font = new Font("Tahoma", 8, FontStyle.Bold);

            txtSoBD.ForeColor = ColorTranslator.FromHtml("#FF6619");
            txtSoBD.Font = new Font("Tahoma", 8, FontStyle.Bold);

            spinEditSoKetThuc.ForeColor = ColorTranslator.FromHtml("#FF6619");
            spinEditSoKetThuc.Font = new Font("Tahoma", 8, FontStyle.Bold);

            txtSL.ForeColor = ColorTranslator.FromHtml("#FF6619");
            txtSL.Font = new Font("Tahoma", 8, FontStyle.Bold);


            if(dtKho?.Rows?.Count > 0)
            {
                lookUp_DVSX.Properties.DataSource = dtKho.AsEnumerable()
                                                    .Select(x=>new {MaDVSX=x["MaDVSX"]?.ToString(),DVSX= x["DVSX"]?.ToString()})?.Distinct()?.ToList(); 
            }
            lookUp_DVSX.Properties.NullText = "---Chọn ĐVSX---";         
         

        }

        private void InitLayout()
        {
            colChon.Visible = false;

            layoutKho.Visibility = LayoutVisibility.Always;
            layoutSeal.Visibility = LayoutVisibility.Never;
            layoutControlItem4.Visibility = LayoutVisibility.Always;
            layoutControlItem6.Visibility = LayoutVisibility.Never;

          
            emptySpaceItem2.Visibility = LayoutVisibility.Always;
           
            //emty1.Visibility = LayoutVisibility.Always;
            emptySpaceItem9.Visibility = LayoutVisibility.Never;
            
            emptySpaceItem1.Visibility = LayoutVisibility.Never;

            InitSLSeal("all"); 
            ckbxALL.Checked = true;


        }

        private void InitSLSeal(string MaDVSX)
        {
           
            int SoSealCDung = 0; int SoSealDung = 0;
            int SoSealLoi = 0;
            if (MaDVSX == "all")
            {
                SoSealLoi = lstMaSeal.Count == 0 ? 0 : lstMaSeal.Count(x => x.Status?.ToLower() == "bị lỗi");
                SoSealCDung = lstMaSeal.Count(x => x.Status?.ToLower() == "chưa xuất");
                 SoSealDung = lstMaSeal.Count - SoSealCDung- SoSealLoi;
                layoutControlItem7.Text = "Seal ĐD";
                layoutControlItem8.Text = "Seal CD";
                layoutControlItem11.Text = $"Seal Lỗi";
            }
            else
            {
                var query = dtKho.AsEnumerable().FirstOrDefault(x => x["MaDVSX"]?.ToString() == MaDVSX);
                var queryMaSeal = lstMaSeal.Where(x => x.MaDVSX == MaDVSX).ToList();
                SoSealLoi = queryMaSeal.Count == 0 ? 0 : queryMaSeal.Count(x => x.Status?.ToLower() == "bị lỗi");
                SoSealCDung = queryMaSeal.Count(x => x.Status?.ToLower() == "chưa xuất");
                SoSealDung = queryMaSeal.Count - SoSealCDung- SoSealLoi;
            

                layoutControlItem7.Text = $"Seal {query["DVSX"]} ĐD";
                layoutControlItem8.Text = $"Seal {query["DVSX"]} CD";
                layoutControlItem11.Text = $"Seal {query["DVSX"]} Lỗi";
            }

            txtCDung.EditValue = SoSealCDung < 0 ? 0: SoSealCDung ;
            txtSealDung.EditValue = SoSealDung < 0 ? 0: SoSealDung;
            txtSealLoi.EditValue = SoSealLoi;
        }
   
        private void LoadMaSeal()
        {
            try
            {
                clsWaitForm.ShowWaitForm(this, 1000);
                setDefaultSeachLookup();
                _status = ResourceURL.EventStatus.View;             
                string url = string.Format("{0}", URL + $"MaSeal/Get?username={_username}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstMaSeal = JsonConvert.DeserializeObject<List<MaSealEntity>>(json);
                }
                grcMaSeal.DataSource = lstMaSeal.Count > 0 ? lstMaSeal : null;
                grcMaSeal.RefreshDataSource();
                lookUp_DVSX.EditValue = null;
                GridViewUpdateStatus(_status);
                InitLayout();
          
            }
            catch
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            /*focused(this.grvMaSeal);*/
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            ThemDong();
        }

        private async void XoaDong()
        {
            try
            {
                if (grvMaSeal.FocusedRowHandle < 0) return;

                MaSealEntity row = grvMaSeal.GetRow(grvMaSeal.FocusedRowHandle) as MaSealEntity;
                CheckPKL(row.MaSeal, row.Seal, out string msg);
                if (!string.IsNullOrEmpty(msg))
                {
                    XtraMessageBox.Show(msg, "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                string url = string.Format("{0}?Parameter={1}", URL + "MaSeal/Delete", row.MaSeal);
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                    LoadMaSeal();
                else XtraMessageBox.Show(result);
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

        private void LuuDong()
        {
            try
            {

                this.ActiveControl = this.button1;
                bool IsUpdate = _status == ResourceURL.EventStatus.Edit;

                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && grvMaSeal.FocusedRowHandle >= 0)
                {
                    FocusedIndex = grvMaSeal.FocusedRowHandle;
                }
                List<MaSealEntity> _lstUpdate = new List<MaSealEntity>();

                List<MaSealEntity> lstMaSeal = grcMaSeal.DataSource as List<MaSealEntity>;

                List<MaSealEntity> query = lstMaSeal.Where(x => x.IsEdit == !IsUpdate).ToList();

                if (IsUpdate)
                {
                    lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                    for (int i = 0; i < lstRowUpdate.Count; i++)
                    {
                        MaSealEntity item = grvMaSeal.GetRow(lstRowUpdate[i]) as MaSealEntity;
                        if (!string.IsNullOrEmpty(item?.Seal) && !string.IsNullOrEmpty(item?.LoaiSeal) && !string.IsNullOrEmpty(item?.MaKho))
                        {

                            _lstUpdate.Add(item);

                        }
                        else
                        {
                            XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            return;
                        }
                    }

                }
                else
                {
                    foreach (var item in query)
                    {
                        if (!string.IsNullOrEmpty(item?.Seal) && !string.IsNullOrEmpty(item?.LoaiSeal) && !string.IsNullOrEmpty(item?.MaKho))
                        {

                            _lstUpdate.Add(item);

                        }
                        else
                        {
                            XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            return;
                        }
                    }
                }


                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate.Count == 0))
                {
                    return;
                }


                string url = URL + "MaSeal/Post?Action=POST";
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
                
                if (msResult.ToLower() == "true")
                {
                    LoadMaSeal();
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

        private void NapLaiDong()
        {
            LoadMaSeal();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void grvMaSeal_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }

        private void grvMaSeal_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (grvMaSeal.FocusedRowHandle < 0) return;

            if (this.grvMaSeal.FocusedColumn != null)
            {
                e.Valid = true;
               
                if (this.grvMaSeal.FocusedColumn == colSeal)
                {
                    List<MaSealEntity> lst = this.grcMaSeal.DataSource as List<MaSealEntity>;
                    bool IsSeal = lst.Any(x => x.Seal == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Seal không được bỏ trống";
                        e.Valid = false;
                    }
                    else if (IsSeal)
                    {

                        e.ErrorText = $"Seal {e.Value.ToString()} đã bị trùng";
                        e.Valid = false;
                    }

                }
                //else if (this.grvMaSeal.FocusedColumn == colLoaiSeal)
                //{
                //    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                //    {
                //        e.ErrorText = $" Loại Seal không được bỏ trống";
                //        e.Valid = false;
                //    }
                //}


            }

        }

        private void ThemDong()
        {

            MaSealEntity obj = new MaSealEntity();
            obj.Status = "Chưa Xuất";
            obj.NVTao = GlobleData.UserName;
            obj.IsEdit = true;

            lstMaSeal.Add(obj);
            _rowAdd = grvMaSeal.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            grvMaSeal.FocusedRowHandle = 0;

            grcMaSeal.RefreshDataSource();

        }

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

            if (_status.Equals(ResourceURL.EventStatus.Add))
            {
                MaSealEntity row = view.GetRow(view.FocusedRowHandle) as MaSealEntity;
                if (view.FocusedColumn == colLoaiSeal || view.FocusedColumn == colKho && row.IsEdit)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }

                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }

            else if (_status.Equals(ResourceURL.EventStatus.Edit))
            {
                if (view.FocusedColumn == colSeal || view.FocusedColumn == colLoaiSeal || view.FocusedColumn == colKho)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }


                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else if (_status.Equals(ResourceURL.EventStatus.View))
            {

                if (view.FocusedColumn == colChon)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
                   
            }
        }    

        private void gridView1_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();


        }

        private void grvMaSeal_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if(e.FocusedRowHandle >= 0)
            {
                string MaSeal = grvMaSeal.GetFocusedRowCellValue(colMaSeal)?.ToString();
                if (!string.IsNullOrEmpty(MaSeal))
                {
                   
                    LoadPKLMaSeal(MaSeal);
                }

            }
            else
            {
                grcPKLMaSeal.DataSource = null;
            }
           
            focused(sender);
        }

        private void grvMaSeal_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
           focused(sender);
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

        private void gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void ValidateTTMaSeal(ref string msg)
        {
            bool IsAdd = layoutControlItem4.Visibility == LayoutVisibility.Always;
        
            if (IsAdd && string.IsNullOrEmpty(searchLookUpEdit_Kho.EditValue?.ToString()))
            {
                msg = "Vui lòng chọn Kho";
            }
            //else if (string.IsNullOrEmpty(cbxLoaiSeal.EditValue?.ToString()))
            //{
            //    msg = "Vui lòng chọn loại seal";
            //}
            else if(txtSoBD.Value <= 0)
            {
                msg = "Số bắt đầu phải lớn hơn 0 ";
            }
            else if(txtSL.Value <= 0)
            {
                msg = "Số Lượng phải lớn hơn 0 ";
            }
        }

        private void txtSoBD_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("txtSoBD_ValueChanged");

            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;

            if (spinEditSoKetThuc.Value != null && !string.IsNullOrEmpty(spinEditSoKetThuc.Value.ToString()) && spinEditSoKetThuc.Value is Decimal
                && changingEventArgs.NewValue != null && !string.IsNullOrEmpty(changingEventArgs.NewValue.ToString()) && changingEventArgs.NewValue is Decimal)
            {
                
                decimal soBD = (decimal)changingEventArgs.NewValue;
                decimal soKT = spinEditSoKetThuc.Value;
                if (soBD > 0 && soKT > 0 && soKT >= soBD)
                {
                    txtSL.Value = soKT - soBD + 1;
                }
                else
                {
                    txtSL.Value = 0;
                }
            }
        }

        private void spinEditSoKetThuc_ValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            Console.WriteLine("spinEditSoKetThuc_ValueChanged");
            if (txtSoBD.Value!=null && !string.IsNullOrEmpty(txtSoBD.Value.ToString())&& txtSoBD.Value is Decimal
                && changingEventArgs.NewValue !=null && !string.IsNullOrEmpty(changingEventArgs.NewValue.ToString())&&changingEventArgs.NewValue is Decimal)
            {
                decimal soBD = txtSoBD.Value;
                decimal soKT = (decimal)changingEventArgs.NewValue;
                if (soBD > 0 && soKT > 0&&soKT>=soBD)
                {
                    txtSL.Value = soKT - soBD +1;
                }
                else
                {
                    txtSL.Value = 0;
                }
            }
        }

        private void txtSL_ValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            Console.WriteLine("txtSL_ValueChanged");
            if (txtSoBD.Value != null && !string.IsNullOrEmpty(txtSoBD.Value.ToString()) && txtSoBD.Value is Decimal
                && changingEventArgs.NewValue != null && !string.IsNullOrEmpty(changingEventArgs.NewValue.ToString()) && changingEventArgs.NewValue is Decimal)
            {
                decimal soBD = txtSoBD.Value;
                decimal soSL = (decimal)changingEventArgs.NewValue;
                if (soBD > 0 && soSL > 0)
                {
                    spinEditSoKetThuc.Value = soSL + soBD -1;
                }
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            ValidateTTMaSeal(ref msg);
            if (!string.IsNullOrEmpty(msg))
            {
                XtraMessageBox.Show(msg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True); return;
                           
            }
            ThemDong((int)txtSoBD.Value,(int) txtSL.Value);
            grvMaSeal.FocusedRowHandle = 0;
        }
  
        private void ThemDong(int SoBD, int SoLuong)
        {
            lookUp_DVSX.EditValue = null;
            List<MaSealEntity> lstTmp = new List<MaSealEntity>();
            int MinChuSoSeal = 6;
            int ContChuSo = MinChuSoSeal - SoBD.ToString().Length;
            string Seal = string.Empty;
            string MaSealSoDB = ContChuSo < 0 ? SoBD.ToString() : SoBD.ToString().PadLeft(MinChuSoSeal, '0');
            int MaxID = lstMaSeal.Count==0?1:lstMaSeal.Max(x => x.ID);
                 
                for (int i = SoBD; i < SoLuong + SoBD; i++)
                {
                Seal = ContChuSo < 0 ? i.ToString() : i.ToString().PadLeft(MinChuSoSeal, '0');

                bool IsDuplicate = lstMaSeal.Any(x => x.Seal == Seal);
                if (IsDuplicate)
                {

                    XtraMessageBox.Show($"Seal số bắt đầu <color=red>{Seal}</color>  đã có ! Vui Lòng chọn số bắt đầu khác", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);

                    return;
                }

                lstTmp.Add(new MaSealEntity
                    {
                        ID = MaxID++,
                        MaSeal = $"MS_{Seal}",
                        Seal = Seal,
                        Status = "Chưa Xuất",
                        NgayTao = DateTime.Now,
                        NVTao = GlobleData.UserName,
                        LoaiSeal = "Nội Bộ",
                        MaKho = searchLookUpEdit_Kho.EditValue?.ToString(),
                        IsEdit = true
                    });
                }


       
            string url = URL + "MaSeal/checkDuplicateSeal";
            string  json = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstTmp); }).Result;
            List<MaSealEntity> lstDuplicate = JsonConvert.DeserializeObject<List<MaSealEntity>>(json);
            if (lstDuplicate?.Count >0 )
            {               
                XtraMessageBox.Show($"Seal từ <color=red>{lstDuplicate[0].Seal}</color> đến <color=red>{lstDuplicate[lstDuplicate.Count-1].Seal}</color>  ĐVSX: <color=navy> {string.Join(", ", lstDuplicate.Select(x => x.MaDVSX).Distinct())} </color>đã có ! " +
                    $"Vui Lòng nhập lại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                return;
            }

            lstMaSeal.AddRange(lstTmp);
            InitSLSeal(string.IsNullOrEmpty(lookUp_DVSX.EditValue?.ToString())|| lookUp_DVSX.EditValue== "Chọn ĐVSX" ? "all":lookUp_DVSX.EditValue?.ToString());
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            grcMaSeal.DataSource = lstMaSeal;
            grcMaSeal.RefreshDataSource();
           

        }
   
        private void grvMaSeal_RowStyle(object sender, RowStyleEventArgs e)
        {
            if(e.RowHandle >= 0 && grvMaSeal?.RowCount > 0)
            {
                string Status = grvMaSeal.GetRowCellValue(e.RowHandle, colStatus)?.ToString();
                bool IsEdit = Convert.ToBoolean(grvMaSeal.GetRowCellValue(e.RowHandle, colIsEdit)?.ToString());
                e.Appearance.ForeColor = ColorTranslator.FromHtml("#1A0000");
                e.HighPriority = true;
                if (grvMaSeal.FocusedRowHandle == e.RowHandle)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#80EAFF");
                }
                else if (IsEdit)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#FFD9B3");
                   
                }
                else if (Status?.ToLower() == "đã xuất")
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#F2FFE6");
                }else if(Status?.ToLower() == "bị lỗi")
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#FF884D");
                }
                //e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Bold);

            }

        }

        private void btnXN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            layoutControlItem4.Visibility = LayoutVisibility.Never;
            layoutControlItem6.Visibility = LayoutVisibility.Always;
            layoutKho.Visibility = LayoutVisibility.Never;
            layoutSeal.Visibility = LayoutVisibility.Never;
            emptySpaceItem9.Visibility = LayoutVisibility.Never;
           
            emptySpaceItem2.Visibility= LayoutVisibility.Never;
            //emptySpaceItem5.Visibility = LayoutVisibility.Never;
            //emty1.Visibility = LayoutVisibility.Always;
            emptySpaceItem9.Visibility = LayoutVisibility.Always;
        }      

        private void MutiDelete()
        {
            bool IsDeleteChoose = colChon.Visible;
            if (IsDeleteChoose)
            {
                XoaDong(IsDeleteChoose);
            }
            else
            {
                XoaDong();
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                MutiDelete();
            }
            else
            {
                return;
            }
          
        }

        private void btnDeleteChoose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            layoutControlItem4.Visibility = LayoutVisibility.Never;
            emptySpaceItem9.Visibility = LayoutVisibility.Never;
            //emty1.Visibility = LayoutVisibility.Always;     
            colChon.Visible = true;

        }

        private void btnXNXoa_Click(object sender, EventArgs e)
        {
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                XoaDong(false);
            }
            else
            {
                return;
            }
        
        }

        private void XoaDong(bool IsChoose)
        {
            try
            {
                List<MaSealEntity> _lstUpdate = new List<MaSealEntity>();

                List<MaSealEntity> lstMaSeal = grcMaSeal.DataSource as List<MaSealEntity>;
                this.ActiveControl = this.button1;
                string msg = string.Empty;
                ValidateTTMaSeal(ref msg);
                if (!string.IsNullOrEmpty(msg)&&!IsChoose)
                {
                    XtraMessageBox.Show(msg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True); return;

                }
       
                if (IsChoose)
                {
                    _lstUpdate = lstMaSeal.Where(x=>x.IsChoose).ToList();

                    foreach(var row in _lstUpdate)
                    {
                        CheckPKL(row.MaSeal, row.Seal, out  msg);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            XtraMessageBox.Show(msg, "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                            return;
                        }
                    }

                }
                else
                {
                    int IdxStart= (int)txtSoBD.Value;
                    int IdxEnd = (int)spinEditSoKetThuc.Value;
                    int MinChuSoSeal = 6;
                    int ContChuSo = MinChuSoSeal - IdxStart.ToString().Length;
                    string Seal = string.Empty;
               
                    for (int i = IdxStart; i <= IdxEnd; i++)
                    {

                        Seal = ContChuSo < 0 ? i.ToString() : i.ToString().PadLeft(MinChuSoSeal, '0');
                        bool IsDuplicate = lstMaSeal.Any(x => x.Seal == Seal);
                        if (!IsDuplicate)
                        {

                            continue;
                        }

                        CheckPKL($"MS_{Seal}", Seal, out msg);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            XtraMessageBox.Show(msg, "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);
                            return;
                        }

                        _lstUpdate.Add(new MaSealEntity
                        {
                            ID =0,
                            MaSeal = $"MS_{Seal}",
                            Seal = Seal,
                            Status = "Chưa Xuất",
                            NgayTao = DateTime.Now,
                            NVTao = GlobleData.UserName,
                            LoaiSeal =null,
                            MaKho = null,
                            IsEdit = true
                        });
                    }
                    

                }

                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate.Count == 0))
                {
                    XtraMessageBox.Show("Không tìm thấy dữ liệu để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                string url = URL + "MaSeal/Post?Action=MUTIDELETE";
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
               
                if (msResult.ToLower() == "true")
                {
                    LoadMaSeal();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
     

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        
        private void LoadPKLMaSeal(string MaSeal) 
        {
          DataTable  dtPkLMaSeal = new DataTable();
            string url = string.Format("{0}", URL + $"MaSeal/GetPKLMaSeal?action=GetPakageListMaSeal&&MaSeal={MaSeal}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                dtPkLMaSeal = JsonConvert.DeserializeObject<DataTable>(json);
            }
            grcPKLMaSeal.DataSource = dtPkLMaSeal;
        }

        private void ckbxALL_CheckStateChanged(object sender, EventArgs e)
        { 
            string MaDVSX = lookUp_DVSX?.EditValue?.ToString();

            if (ckbxALL.Checked)
            {
                var query = string.IsNullOrEmpty(MaDVSX) ? lstMaSeal : lstMaSeal.Where(x => x?.MaDVSX == MaDVSX).ToList();
                grcMaSeal.DataSource = query;

                if (lstMaSeal.Count > 0)
                {
                   object MaSeal = grvMaSeal.GetRowCellValue(0,colMaSeal);

                    LoadPKLMaSeal(MaSeal?.ToString());
                }
            }
            else
            {
                var query = string.IsNullOrEmpty(MaDVSX) ? lstMaSeal.Where(x => x.Status?.ToLower() == "chưa xuất").ToList(): lstMaSeal.Where(x => x.Status?.ToLower() == "chưa xuất" && x.MaDVSX == MaDVSX).ToList();
                grcMaSeal.DataSource = query;
                if (query.Count > 0)
                {
                    LoadPKLMaSeal(query[0].MaSeal);
                }
            }  
            grcMaSeal.RefreshDataSource();
        }

        private void CheckPKL(string MaSeal,string Seal,out string msg)
        {
            msg = string.Empty;
            var query = lstMaSeal.FirstOrDefault(x => x.MaSeal?.ToLower() == MaSeal?.ToLower());
            if(query?.Status?.ToLower() == "bị lỗi")
            {
                msg = "Không thể xóa Seal này";
            }
           else if(query!=null && (bool)query?.IsUsed)
            {
                DataTable dtPkLMaSeal = new DataTable();
                string url = string.Format("{0}", URL + $"MaSeal/GetPKLMaSeal?action=CheckPKL&&MaSeal={query.MaSeal}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    dtPkLMaSeal = JsonConvert.DeserializeObject<DataTable>(json);
                }
                msg = dtPkLMaSeal.Rows.Count > 0 ? $"Mã Seal {Seal} đã có ở ĐH <color=red>{string.Join(", ", dtPkLMaSeal.AsEnumerable().Select(row => row["GopDH"].ToString())) }</color> " +
                    $"và PKL <color=red> {string.Join(", ", dtPkLMaSeal.AsEnumerable().Select(row => row["MaPKL_XH"].ToString()))} </color>" : string.Empty;
            }
            

           
        }
    
        private void grvPKLMaSeal_RowStyle(object sender, RowStyleEventArgs e)
        {
            if(e.RowHandle >= 0)
            {
                if (grvPKLMaSeal.FocusedRowHandle == e.RowHandle)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#FFF7CC");
                    e.HighPriority = true;
                }
            }
        }

        private void lookUp_DVSX_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.Controls.ChangingEventArgs changedValue = e as DevExpress.XtraEditors.Controls.ChangingEventArgs;
            if ( changedValue?.NewValue != changedValue?.OldValue)
            {
                string MaDVSX = changedValue?.NewValue?.ToString();
                InitSLSeal(string.IsNullOrEmpty(MaDVSX)||MaDVSX== "Chọn ĐVSX" ? "all":MaDVSX);
                string filterKey = ckbxALL.Checked ? string.Empty : "đã xuất";
                if (!string.IsNullOrEmpty(MaDVSX))
                {
                    grcMaSeal.DataSource = lstMaSeal.Count > 0 ? lstMaSeal.Where(x => x.Status?.ToLower() != filterKey && x.MaDVSX == MaDVSX).ToList() : null;
                }
                else
                {
                   
                    grcMaSeal.DataSource = lstMaSeal.Count > 0 ? lstMaSeal.Where(x => x.Status?.ToLower() != filterKey && !x.IsUsed).ToList() : null;
                }

                grcMaSeal.RefreshDataSource();


            }

        }


    }
}

