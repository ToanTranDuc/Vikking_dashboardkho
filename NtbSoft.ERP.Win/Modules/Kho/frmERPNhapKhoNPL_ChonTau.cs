using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPL_ChonTau : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _tenTau="";
        public frmERPNhapKhoNPL_ChonTau()
        {
            InitializeComponent();
         
            gridView1.Appearance.FocusedRow.BackColor = Color.Empty;
            gridView1.Appearance.FocusedRow.ForeColor = Color.Empty;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            loadTau();
        }
        private void loadTau()
        {
            try
            {
                string urlTau = $"{URL}ERPThuVienNK/Get?Action=GETTAU";
                string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
                if (jsonTau == "[]")
                {
                    gridControl1.DataSource = null;
                }
                else
                {
                    DataTable tblTau = JsonConvert.DeserializeObject<DataTable>(jsonTau);
                    gridControl1.DataSource = tblTau;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void btnXacNhan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.DialogResult = DialogResult.OK;
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

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;
            _tenTau = dr["TenTau"].ToString();
        }
        public string getTau()
        {
            return _tenTau;
        }

        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            //GridView view = sender as GridView;
        
            //if (e.RowHandle == view.FocusedRowHandle)
            //{
            //    e.Appearance.BackColor = Color.Red;
            //    e.Appearance.ForeColor = Color.Black;
            //}
        }
    }
}
