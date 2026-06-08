using DevExpress.XtraEditors;
using Newtonsoft.Json;
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
    public partial class frmQuyDoi : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        public string URL = string.Empty;
        HttpClientExtension _clientExtension;
        public frmQuyDoi()
        {
            InitializeComponent();
            _clientExtension = new HttpClientExtension();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            string url = string.Format("{0}?", URL + "QuyDoi/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count > 0 && tbl != null)
                gridControl1.DataSource = tbl;
            else
                gridControl1.DataSource = null;
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

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridView1.FocusedColumn.FieldName == "SoLuong")
            {
                string enteredValue = e.Value.ToString();
                if (!decimal.TryParse(enteredValue, out decimal result))
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng phải là một số hợp lệ.";
                }
                else
                {
                    e.Valid = true;
                }
            }
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable _dtData = gridControl1.DataSource as DataTable;
            if(_dtData.Rows.Count > 0 && _dtData != null)
            {
                string url = string.Format("{0}?", URL + "QuyDoi/Post");
                string mss = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtData); }).Result;
                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu Quy đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
            }    

        }
    }
}
