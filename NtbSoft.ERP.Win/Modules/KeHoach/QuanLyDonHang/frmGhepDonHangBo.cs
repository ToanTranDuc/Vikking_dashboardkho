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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmGhepDonHangBo : DevExpress.XtraEditors.XtraForm
    {
        DataRow rowDH;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();
        DataTable tblDsDonHangGhepBo;
        public frmGhepDonHangBo()
        {
            InitializeComponent();
            _clientExtension = new HttpClientExtension();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            LoadDonHang();
        }
        private void LoadDonHang()
        {
            try
            {
                string urlDsDonHangGoc = string.Format("{0}", URL + "GhepDonHangBo/GET_DH");
                string jsonDsDonHangGoc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsDonHangGoc); }).Result;
                DataTable dbResult = JsonConvert.DeserializeObject<DataTable>(jsonDsDonHangGoc);
                if (dbResult == null && dbResult.Rows.Count == 0)
                {
                    slue_ChonDonHang.Properties.DataSource = new DataTable();
                    return;
                }
                slue_ChonDonHang.Properties.DataSource = dbResult;
                slue_ChonDonHang.Properties.DisplayMember = "MaDH";
                slue_ChonDonHang.Properties.ValueMember = "MaDH";
                slue_ChonDonHang.Properties.NullText = "Chọn đơn hàng";
                //if (dbResult.Rows.Count > 0)
                //    slue_ChonDonHang.EditValue = dbResult.Rows[0]["MaDH"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void slue_ChonDonHang_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (slue_ChonDonHang.EditValue.ToString().ToLower() == "null")
                {
                    txtMaDH.Text = string.Empty;
                    txtMaHang.Text = string.Empty;
                    txtChungLoai.Text = string.Empty;
                    txtDot.Text = string.Empty;
                    return;
                }
                DataTable dbDHGoc = (DataTable)slue_ChonDonHang.Properties.DataSource;
                for (int i = 0; i < dbDHGoc.Rows.Count; i++)
                {
                    if (slue_ChonDonHang.EditValue.ToString() == dbDHGoc.Rows[i]["MaDH"].ToString())
                    {
                        txtMaDH.Text = dbDHGoc.Rows[i]["MaDH"].ToString();
                        txtMaHang.Text = dbDHGoc.Rows[i]["MaHang"].ToString();
                        txtChungLoai.Text = dbDHGoc.Rows[i]["TenCL"].ToString();
                        txtDot.Text = dbDHGoc.Rows[i]["Dot"].ToString();
                        break;
                    }
                }
                string urlDsDonHangGhep = string.Format("{0}", URL + "GhepDonHangBo/GET_DH_GHEP?maDH=" + slue_ChonDonHang.EditValue);
                string jsonDsDonHangGhep = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsDonHangGhep); }).Result;
                DataTable dbResult = JsonConvert.DeserializeObject<DataTable>(jsonDsDonHangGhep);
                if (dbResult == null && dbResult.Rows.Count == 0)
                {
                    gc_DonHangGhep.DataSource = new DataTable();
                    return;
                }
                gc_DonHangGhep.DataSource = dbResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void gv_GhepDH_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataTable dtSave = (DataTable)gc_DonHangGhep.DataSource;
                DataTable result = new DataTable();
                result.Columns.Add("MaDHGoc", typeof(string));
                result.Columns.Add("MaDHGhep", typeof(string));
                for (int i = 0; i < dtSave.Rows.Count; i++)
                {
                    if ((bool)dtSave.Rows[i]["Chon"] == true)
                    {
                        DataRow row = result.NewRow();
                        result.Rows.Add(slue_ChonDonHang.EditValue.ToString(), dtSave.Rows[i]["MaDH"].ToString());
                    }
                }
                string urlDsDonHangGhep = string.Format("{0}", URL + "GhepDonHangBo/POST?maDH=" + slue_ChonDonHang.EditValue);
                string jsonDsDonHangGhep = Task.Run(async () => { return await _clientExtension.PostAsync(urlDsDonHangGhep, result); }).Result;
                if (jsonDsDonHangGhep.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadDonHang();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void slue_ChonDonHang_EdiValueChanged(object sender, EventArgs e)
        {

        }

        private void gv_GhepDH_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "Chon")
            {
                bool newval = (bool)e.Value;
                gv_GhepDH.SetRowCellValue(e.RowHandle, "Chon", newval);
            }
        }
    }
}
