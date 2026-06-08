using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmImportCanDoiNPL : DevExpress.XtraEditors.XtraForm
    {
        List<DinhMucSaveImportEntity> lstsave = new List<DinhMucSaveImportEntity>();
        System.Configuration.AppSettingsReader settingsReader =
                                       new AppSettingsReader();
        private HttpClientExtension _clientExtension = new HttpClientExtension();
        string _URL = string.Empty;
        public frmImportCanDoiNPL(List<DinhMucSaveImportEntity> listDMNL)
        {
            InitializeComponent();

            gridControl1.DataSource = listDMNL;
            bool iscapthem = listDMNL.All(x => x.CapPhat == 0);
            if (iscapthem)
            {
                this.ColCapPhat.Visible = false;
            }
            else 
            {
                this.ColCapThem.Visible = false;
            }
            lstsave = listDMNL;
            _URL = (string)settingsReader.GetValue("URL", typeof(String));
        }

        public frmImportCanDoiNPL(string madonhang, string malenhsanxuat, string makh, string pathExcel, string sheetIndexExcel, string urlNPL, string URL, bool _ChkCT, bool _ChkTH)
        {
            InitializeComponent();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string urlSaveDot = string.Empty;
            string savelistDot = string.Empty;

            string urlSaveDM = string.Empty;
            string savelistDM = string.Empty;
            try
            {
                clsWaitForm.ShowWaitForm(this, 7000);
                string json = JsonConvert.SerializeObject(lstsave);
                DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
                urlSaveDM = string.Format("{0}", _URL + "CanDoiDonHangTong/PostDinhMucNPL");
                savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, lstsave); }).Result;

                bool iscapthem = lstsave.All(x => x.CapPhat == 0);
                if (iscapthem)
                {
                    urlSaveDot = string.Format("{0}", _URL + "CanDoiDonHangTong/PostDot");
                    savelistDot = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDot, lstsave); }).Result;
                }
            }
            catch (Exception x) 
            {

            }


            if (string.Compare(savelistDM, "True") != 0)
            {
                XtraMessageBox.Show("Lỗi import nguyên phụ liệu!(" + savelistDM + ")", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }
            else 
            {
                this.Close();
                clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }

        private void bandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.Column == ColChonLoai)
            {
                object isNL = e.Value;
                if (isNL != null)
                {
                    int intValue = Convert.ToInt32(isNL); // Chuyển đổi giá trị về int
                    e.DisplayText = (intValue == 1) ? "Nguyên Liệu" : "Phụ Liệu";
                }

            }
        }

        private void bandedGridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                e.Value = e.ListSourceRowIndex + 1;
            }
        }

        private void bandedGridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "STT") // Cột STT
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void bandedGridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void bandedGridView1_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e)
        {
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }
    }
}