using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmDonViSanXuatDetail : DevExpress.XtraEditors.XtraForm
    {
        

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        private string _maDVSX = string.Empty;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        int _rowAdd = -1;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();

        List<DonViSanXuatDetailEntity> lstDonViSanXuatDetailEntity;
        bool indicatorIcon = true;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        public frmDonViSanXuatDetail(string MaDVSX)
        {
            InitializeComponent();
            this._maDVSX = MaDVSX;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadDSDonViSanXuatDetail();
        }

        private void LoadDSDonViSanXuatDetail()
        {
            string url = string.Format("{0}?MaDVSX={1}", URL + "DonViSanXuat/GetDonViSanXuatDetail", _maDVSX);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            lstDonViSanXuatDetailEntity = JsonConvert.DeserializeObject<List<DonViSanXuatDetailEntity>>(json);
            gridDonViSanXuat.DataSource = lstDonViSanXuatDetailEntity;
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDSDonViSanXuatDetail();
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

        private void gridViewDonViSanXuat_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
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

        private void gridDonViSanXuat_ProcessGridKey(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //case Keys.F1:
                //    if (_allowAdd)
                //        BtThem();
                //    break;
                //case Keys.F2:
                //    if (_allowEdit)
                //        BtSua();
                //    break;
                //case Keys.F3:
                //    if (_allowDelete)
                //        Xoadulieu();
                //    break;
                //case Keys.F4:
                //    if (_allowAdd || _allowEdit)
                //        Luudulieu();
                //    break;
                case Keys.F5:
                    LoadDSDonViSanXuatDetail();
                    break;
            }
        }
    }
}
