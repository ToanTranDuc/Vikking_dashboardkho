using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using NtbSoft.ERP.Entity.SoTheoDoi;
using NtbSoft.ERP.Entity.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.SoTheoDoi
{
    public partial class frmThemTheKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<TheKhoEntity> lstDsTheKho;
        List<TheKhoEntity> lstFilterDsTheKho;
        bool isThem = true;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        List<int> lstRowUpdate = new List<int>();

        public frmThemTheKho(DataTable tblDsKho, DataTable tblDsHangHoa, DataTable tblDsKhachHang, DataTable tblDsDonVi)
        {
            InitializeComponent();
            CreateSearchLookup(tblDsKho, tblDsHangHoa, tblDsKhachHang, tblDsDonVi);
        }

        public frmThemTheKho(bool _isThem, TheKhoEntity _itemTheKho, List<TheKhoEntity> _lstFilterDsTheKho,DataTable tblDsKho, DataTable tblDsHangHoa, DataTable tblDsKhachHang, DataTable tblDsDonVi)
        {
            InitializeComponent();
            isThem = _isThem;
            lstFilterDsTheKho = _lstFilterDsTheKho;
            CreateSearchLookup(tblDsKho, tblDsHangHoa, tblDsKhachHang, tblDsDonVi);
            MapThongTinKho(_itemTheKho, _lstFilterDsTheKho);
        }

        public frmThemTheKho()
        {
            InitializeComponent();
            //CreateSearchLookup(tblDsKho, tblDsHangHoa, tblDsKhachHang, tblDsDonVi);
        }

        protected override void OnLoad(EventArgs e)
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            dateEditNgayLapThe.EditValue = DateTime.Now;
            lstDsTheKho = new List<TheKhoEntity>();
            //gridControlChiTietTheKho.DataSource = lstDsTheKho;
            CheckPerminsion();
            if (isThem)
            {
                this.Sua.Visibility = BarItemVisibility.Never;
            }
            else
            {
                this.Them.Visibility= BarItemVisibility.Never;
            }
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

        private void MapThongTinKho(TheKhoEntity _itemTheKho, List<TheKhoEntity> _lstFilterDsTheKho)
        {
            searchLookUpEditKho.EditValue = _itemTheKho.MaKho;
            searchLookUpEditKho.Enabled = false;
            searchLookUpEditMH.EditValue = _itemTheKho.MaHang;
            searchLookUpEditMH.Enabled = false;
            txtToSo.EditValue = _itemTheKho.ToSo;
            txtToSo.Enabled = false;
            //dateEditNgayLapThe.EditValue = _itemTheKho.NgayLapThe;
            dateEditNgayLapThe.Enabled = false;
            searchLookUpEditKH.EditValue = _itemTheKho.MaKhachHang;
            searchLookUpEditKH.Enabled = false;

            gridControlChiTietTheKho.DataSource = _lstFilterDsTheKho;
            bandedGridViewChiTietTheKho.RefreshData();
        }

        private void CreateSearchLookup(DataTable tblDsKho, DataTable tblDsHangHoa, DataTable tblDsKhachHang, DataTable tblDsDonVi)
        {
            searchLookUpEditKho.Properties.DataSource = tblDsKho;
            searchLookUpEditKho.Properties.DisplayMember = "TenDVSX";
            searchLookUpEditKho.Properties.ValueMember = "MaDVSX";

            searchLookUpEditMH.Properties.DataSource = tblDsHangHoa;
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";
            searchLookUpEditMH.Properties.ValueMember = "MaHang";

            searchLookUpEditKH.Properties.DataSource = tblDsKhachHang;
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            searchLookUpEditKH.Properties.ValueMember = "MaKH";

            repositorySearchLookUpEditDVNhomVai.DataSource = tblDsDonVi;
            repositorySearchLookUpEditDVNhomVai.DisplayMember = "TenDV";
            repositorySearchLookUpEditDVNhomVai.ValueMember = "MaDV";
        }

        private void bandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);

            // Chỉnh font chữ cho bandHeader
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Chỉnh alignment cho bandHeader
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            e.Info.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }

        private void ThemDong()
        {
            if (string.IsNullOrEmpty(searchLookUpEditKho.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn Kho. Thông tin kho không được để trống.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
                
            if (string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn Hàng Hóa. Thông tin kho không được để trống.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            if (string.IsNullOrEmpty(txtToSo.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa nhập Tờ /Số. Thông tin kho không được để trống.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }
            TheKhoEntity obj = new TheKhoEntity(searchLookUpEditKho.EditValue.ToString(),searchLookUpEditMH.EditValue.ToString(), txtToSo.EditValue.ToString(), DateTime.Parse(dateEditNgayLapThe.EditValue.ToString()), searchLookUpEditKH.EditValue.ToString());
            lstDsTheKho.Add(obj);

            _rowAdd = bandedGridViewChiTietTheKho.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            gridControlChiTietTheKho.DataSource = lstDsTheKho;
            bandedGridViewChiTietTheKho.RefreshData();
            GridViewUpdateStatus(_status);
            bandedGridViewChiTietTheKho.FocusedRowHandle = _rowAdd;
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private void bandedGridViewChiTietTheKho_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            //focused(sender);
        }

        private void Sua_ItemClick(object sender, ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }

        private void LuuDong()
        {
            this.ActiveControl = this.button1;
            List<TheKhoEntity> _lstUpdate = new List<TheKhoEntity>();

            TheKhoEntity focusedRow = bandedGridViewChiTietTheKho.GetFocusedRow() as TheKhoEntity;

            if (focusedRow == null)
            {
                XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();

            for (int i = 0; i < lstRowUpdate.Count; i++)
            {
                TheKhoEntity item = bandedGridViewChiTietTheKho.GetRow(lstRowUpdate[i]) as TheKhoEntity;
                _lstUpdate.Add(item);
            }

            string msResult = "";
            if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
            {
                return;
            }
            string url = string.Format("{0}", URL + "TheKho/Post");
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                //LoadDSTheKho();
            }
            else XtraMessageBox.Show(msResult);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            lstRowUpdate.Clear();
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    bandedGridViewChiTietTheKho.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                        Them.Enabled = true;
                    if (_allowEdit)
                        Sua.Enabled = true;
                    if (_allowAdd || _allowEdit)
                        Luu.Enabled = false;
                    break;
                case ResourceURL.EventStatus.Edit:
                    bandedGridViewChiTietTheKho.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                        Them.Enabled = false;
                    if (_allowEdit)
                        Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                        Luu.Enabled = true;

                    break;
                case ResourceURL.EventStatus.Add:
                    bandedGridViewChiTietTheKho.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                        Them.Enabled = false;
                    if (_allowEdit)
                        Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                        Luu.Enabled = true;
                    break;
            }
        }
        private void gridViewTheKho_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewTheKho_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
    }
}
