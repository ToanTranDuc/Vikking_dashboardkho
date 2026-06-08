using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmDanhGiaNhaCungCap : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        string _maPhieu = string.Empty;
        string _action = string.Empty;
        string _tenPhieu = string.Empty;


        DataTable _tblSaveDGTQ = new DataTable();
        DataTable _tblSaveDGNoiLV = new DataTable();

        public frmDanhGiaNhaCungCap(string action, string MaPhieu = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _action = action;
            _maPhieu = MaPhieu;
            SetupLayout();
            LoadNhaCC();
            LoadDanhGiaTQ();
            LoadDGNoiLV();
        }
        private DataTable CreateTableSavePhieuDGTQ()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuDG", typeof(string));
            dt.Columns.Add("TieuChiID", typeof(string));
            dt.Columns.Add("Diem1", typeof(bool));
            dt.Columns.Add("Diem2", typeof(bool));
            dt.Columns.Add("Diem3", typeof(bool));
            dt.Columns.Add("Diem4", typeof(bool));
            dt.Columns.Add("Diem5", typeof(bool));
            dt.Columns.Add("NgayDG", typeof(string));
            dt.Columns.Add("NguoiDanhGia", typeof(string));
            return dt;
        }
        private DataTable CreateTableSaveDGNoiLV()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuDG", typeof(string));
            dt.Columns.Add("NhomDanhGia", typeof(long));
            dt.Columns.Add("TieuChiNoiLVID", typeof(int));
            dt.Columns.Add("ApDung", typeof(bool));
            dt.Columns.Add("DiemDatDuoc", typeof(decimal));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NgayDG", typeof(string));
            return dt;
        }

        private DataTable CreatePhieuDanhGiaNCC()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));             
            dt.Columns.Add("MaPhieuDG", typeof(string));    
            dt.Columns.Add("TenPhieuDG", typeof(string));   
            dt.Columns.Add("KetQuaDat", typeof(bool));      
            dt.Columns.Add("KetQuaLamviecNCC", typeof(bool));
            dt.Columns.Add("LoaiPhieu", typeof(string));    
            dt.Columns.Add("Quy", typeof(string));          
            dt.Columns.Add("Nam", typeof(string));          
            dt.Columns.Add("MaNCC", typeof(string));       
            dt.Columns.Add("NgayDG", typeof(string));      
            dt.Columns.Add("NguoiDanhGia", typeof(string)); 

            return dt;
        }

        private void SetupLayout()
        {

            RepositoryItemMemoEdit memoEdit = new RepositoryItemMemoEdit();
            memoEdit.WordWrap = true;
            gridControl2.RepositoryItems.Add(memoEdit);

            var noteColumn = gridView2.Columns["TieuChi"];
            if (noteColumn != null)
            {
                noteColumn.ColumnEdit = memoEdit;
                noteColumn.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                noteColumn.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                //noteColumn.Width = 200;
                //noteColumn.MinWidth = 100;
            }
            if (_action == "add")
            {
              
                GenerateMaPhieu();
             

            }
            else if (_action == "edit")
            {

              
            }
           
        }

        private void GenerateMaPhieu()
        {
            try
            {
                string url = $"{URL}PhieuDanhGiaNhaCC/GET?action=GeneratePhieu";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                int CountSoPhieu = 0;
                if (json != "[]")
                {
                    DataTable tblMaPhieu = JsonConvert.DeserializeObject<DataTable>(json);
                    _maPhieu = tblMaPhieu.Rows[0]["SoPhieu"]?.ToString();
                }
                else
                {
                    XtraMessageBox.Show("Lỗi tạo mã phiếu vui lòng thử lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void LoadTenPhieu()
        {
            try
            {
            
                string maNCC = SearchLookupNhaCC.EditValue?.ToString();              
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string url = $"{URL}PhieuDanhGiaNhaCC/GET?action=GetCountPhieu&para1={maNCC}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                int CountSoPhieu = 0;
                if (json != "[]")
                {
                    DataTable tblPhieu = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tblPhieu != null && tblPhieu?.Rows?.Count > 0)
                    {
                        var validCounts = tblPhieu.AsEnumerable()
                        .Select(row => row["CountPhieu"]?.ToString().Trim())
                        .Where(s => !string.IsNullOrEmpty(s) && int.TryParse(s, out _))
                        .Select(s => int.Parse(s));

                        if (validCounts.Any())
                        {
                            CountSoPhieu = validCounts.Max();
                        }
                    }
                }
                _tenPhieu = $"PĐG|{SearchLookupNhaCC?.Text?.ToString()}|{DateTime.Now.ToString("ddMMyyyy-hhss")}|{CountSoPhieu + 1}";
                txtSoPhieu.Text = _tenPhieu;
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadNhaCC()
        {
            try
            {
                string url = $"{URL}PhieuDanhGiaNhaCC/GET?action=GetNCC";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblNCC = JsonConvert.DeserializeObject<DataTable>(json);
                SearchLookupNhaCC.Properties.DataSource = tblNCC;
                SearchLookupNhaCC.Properties.ValueMember = "MaNhaCC";
                SearchLookupNhaCC.Properties.DisplayMember = "TenNCC";

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        private void SearchLookupNhaCC_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;

            DataRow row = edit.Properties.View.GetFocusedDataRow();
            if (edit.EditValue == null || edit.EditValue == DBNull.Value || row == null)
            {
                return;
            }
            if (_action == "add")
            {
                LoadTenPhieu();
               
            }
           

        }

        private void LoadDanhGiaTQ()
        {
            try
            {
                string url = $"{URL}PhieuDanhGiaNhaCC/GET?action=GetPhieuDG_TQ&para1={_maPhieu}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = new DataTable();
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);
                }

                gridControl1.DataSource = tbl;
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadDGNoiLV()
        {
            try
            {
                string url = $"{URL}PhieuDanhGiaNhaCC/GET?action=GetPhieuDGNoiLV&para1={_maPhieu}&para2=1";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = new DataTable();
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);
                }

                gridControl2.DataSource = tbl;
                gridView2.ExpandAllGroups();
                gridControl2.RefreshDataSource();
            }
            catch (Exception ex)
            {

            }
        }
  
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDicDanhGiaNCC frm = new frmDicDanhGiaNCC();
            frm.ShowDialog();
        }

        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;

                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;

                int groupLevel = view.GetRowLevel(e.RowHandle);

                GridColumn groupColumn = info.Column;


                info.GroupText = string.Format("{0}", info.GroupValueText);

                if (view.IsGroupRow(e.RowHandle))

                {

                    Color textColor = Color.Black;

                    switch (groupLevel)

                    {

                        case 0: textColor = Color.MediumBlue; break;

                        case 1: textColor = Color.Maroon; break;

                    }

                    e.Appearance.ForeColor = textColor;

                    e.DefaultDraw();

                    e.Handled = true;

                }

            }
            catch (Exception ex)
            {

            }
        }
        private void gridView2_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize + 20;
                    }

                    e.Info.DisplayText = "*";
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize + 20;
                    }

                    e.Info.DisplayText = "*";
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }
    }
}