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
    public partial class frmERPNhapKhoNPL_TienTe : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable _tbl = new DataTable();
     
        public frmERPNhapKhoNPL_TienTe(DataTable tbl)
        {
            InitializeComponent();
         
            gridView1.Appearance.FocusedRow.BackColor = Color.Empty;
            gridView1.Appearance.FocusedRow.ForeColor = Color.Empty;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _tbl = tbl.Copy();
            CreateSearchLookUpTienTe();
            loadGia();
        }
        private void CreateSearchLookUpTienTe()
        {
            try
            {
                repositoryItemSearchLookUpEdit1.DisplayMember = "MaTienTe";
                repositoryItemSearchLookUpEdit1.ValueMember = "TienTeID";
                string urlTT = $"{URL}ERPNhapKhoNPL/Get?Action=GETTIENTE";
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                if (jsonTT == "[]")
                {
                    repositoryItemSearchLookUpEdit1.DataSource = null;
               
                }
                else
                {
                    DataTable tblSearchTienTe = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                    repositoryItemSearchLookUpEdit1.DataSource = tblSearchTienTe;
               
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void loadGia()
        {
            try
            {
                if(!_tbl.Columns.Contains("GiaTienQuyDoi"))
                {
                    _tbl.Columns.Add("GiaTienQuyDoi", typeof(decimal));
                }
                if (!_tbl.Columns.Contains("NgayQuyDoi"))
                {
                    _tbl.Columns.Add("NgayQuyDoi", typeof(string));
                }
                if (!_tbl.Columns.Contains("ThanhTienQuyDoi"))
                {
                    _tbl.Columns.Add("ThanhTienQuyDoi", typeof(decimal));
                }
                if (!_tbl.Columns.Contains("TienTeID"))
                {
                    _tbl.Columns.Add("TienTeID", typeof(string));
                }
                string urlTau = $"{URL}ERPNhapKhoNPL/Get?Action=GETQUYDOI";
                string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
                if (jsonTau == "[]")
                {
                    gridControl1.DataSource = null;
                }
                else
                {
                    DataTable tblTienTe = JsonConvert.DeserializeObject<DataTable>(jsonTau);
                    var tienTeLookup = tblTienTe.AsEnumerable()
                        .ToDictionary(
                            row => row["QuyDoiID"], 
                            row => row             
                        );

                    foreach (DataRow row_tbl in _tbl.Rows)
                    {
                        if (row_tbl["QuyDoiID"] == DBNull.Value)
                        {
                            continue;
                        }

                        object quyDoiID = row_tbl["QuyDoiID"];


                        if (tienTeLookup.ContainsKey(quyDoiID))
                        {
                            DataRow tienTeRow = tienTeLookup[quyDoiID];

                  
                            row_tbl["GiaTienQuyDoi"] = tienTeRow["Gia"];
                            row_tbl["NgayQuyDoi"] = Convert.ToDateTime( tienTeRow["Ngay"]).ToString("dd/MM/yyyy");
                            row_tbl["ThanhTienQuyDoi"] = decimal.TryParse(tienTeRow["Gia"]?.ToString(), out decimal gia) && decimal.TryParse(row_tbl["ThanhTien"]?.ToString(), out decimal thanhTienHienTai) ? gia * thanhTienHienTai : 0;
                            row_tbl["TienTeID"] = row_tbl["TienTe"];
                        }
                        else
                        {
                        
                            row_tbl["GiaTienQuyDoi"] = DBNull.Value;
                            row_tbl["NgayQuyDoi"] = DBNull.Value;
                            row_tbl["ThanhTienQuyDoi"] = 0;
                            row_tbl["TienTeID"] = row_tbl["TienTe"];
                        }
                    }
                    gridControl1.DataSource = _tbl;
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
           
        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn4)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn6)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {
                int groupIndex = gridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
    }
}
