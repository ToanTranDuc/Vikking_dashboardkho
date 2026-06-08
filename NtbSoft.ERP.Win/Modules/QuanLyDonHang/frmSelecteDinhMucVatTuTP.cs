using DevExpress.XtraEditors;
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmSelecteDinhMucVatTuTP : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private DataTable tblVatTu;
        DataTable _tbl = new DataTable();
       
        string _MaGop = string.Empty;
        string _MaLenhSanXuatlenhSX = string.Empty;
        public DataTable tblVatTuSelected { get; set; }
        public DataTable tblVatTuDM_DetailSelected { get; set; }
        public frmSelecteDinhMucVatTuTP(DataTable tbl, string MaGop,string MaLenhSanXuat)
        {
            
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._tbl = tbl;
            _MaGop = MaGop;
            _MaLenhSanXuatlenhSX = MaLenhSanXuat;
            LoadVatTuThanhPhan();
        }
        public DataTable GetBangSize(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetBangSize", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null)
            {
                return null;
            }
            else
                return tbl;
        }
        public DataTable GetERPSIZESP(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetERPSIZESP", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null)
            {
                return null;
            }
            else
                return tbl;
        }
        private DataTable XuLyCotSize(DataTable tblMain, DataTable dtAllSizes, DataTable dtERPSizes)
        {
            if (!tblMain.Columns.Contains("Size"))
            {
                tblMain.Columns.Add("Size", typeof(string));
            }

            // Dictionary chứa danh sách size theo NhomSize (Bảng 1 - BangSize)
            Dictionary<string, List<string>> dictAllSizes = new Dictionary<string, List<string>>();

            if (dtAllSizes != null && dtAllSizes.Rows.Count > 0)
            {
                var groupedAllSizes = dtAllSizes.AsEnumerable()
                    .GroupBy(r => r.Field<string>("NhomSize"))
                    .Select(g => new
                    {
                        NhomSize = g.Key,
                        Sizes = g.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                 .Distinct()
                                 .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                 .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                 .ThenBy(s => s)
                                 .ToList()
                    });

                foreach (var item in groupedAllSizes)
                {
                    dictAllSizes[item.NhomSize] = item.Sizes;
                }
            }
            Dictionary<string, Dictionary<string, List<string>>> dictVTSizes = new Dictionary<string, Dictionary<string, List<string>>>();

            if (dtERPSizes != null && dtERPSizes.Rows.Count > 0)
            {
                var groupedVTSizes = dtERPSizes.AsEnumerable()
                    .GroupBy(r => new
                    {
                        ID = r.Field<object>("ID")?.ToString(),
                        MaVTID = r.Field<object>("MaVTID")?.ToString(),
                        MaMauVT = r.Field<object>("MaMauVT")?.ToString(),
                        MaKhoVT = r.Field<object>("MaKhoVT")?.ToString(),
                        MaNhomVT = r.Field<object>("MaNhomVT")?.ToString(),
                        MaNhomChiTiet = r.Field<object>("MaNhomChiTiet")?.ToString(),
                        MaCode = r.Field<object>("MaCode")?.ToString()
                    })
                    .Select(g => new
                    {
                        Key = g.Key,
                        SizesByNhom = g.GroupBy(r => r.Field<string>("NhomSize"))
                                       .Select(ng => new
                                       {
                                           NhomSize = ng.Key,
                                           Sizes = ng.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                                     .Distinct()
                                                     .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                                     .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                                     .ThenBy(s => s)
                                                     .ToList()
                                       })
                                       .ToDictionary(x => x.NhomSize, x => x.Sizes)
                    });

                foreach (var item in groupedVTSizes)
                {
                    string key = item.Key.ID;
                    dictVTSizes[key] = item.SizesByNhom;
                }
            }
            foreach (DataRow row in tblMain.Rows)
            {
                string id = row["ID"]?.ToString();
                string sizeValue = "";

                if (!string.IsNullOrEmpty(id) && dictVTSizes.ContainsKey(id))
                {
                    var vtSizesByNhom = dictVTSizes[id];
                    bool isAllSize = true;
                    List<string> sizeStrings = new List<string>();
                    var sortedNhoms = vtSizesByNhom
                        .Select(kv => new
                        {
                            Key = kv.Key,
                            Value = kv.Value,
                            IsNum = int.TryParse(kv.Key, out int n),
                            NumVal = int.TryParse(kv.Key, out int n2) ? n2 : int.MaxValue
                        })
                        .OrderBy(x => x.IsNum ? 0 : 1)
                        .ThenBy(x => x.NumVal)
                        .ThenBy(x => x.Key);

                    foreach (var nhom in sortedNhoms)
                    {
                        string nhomSize = nhom.Key;
                        List<string> vtSizes = nhom.Value;
                        if (dictAllSizes.ContainsKey(nhomSize))
                        {
                            List<string> allSizes = dictAllSizes[nhomSize];
                            bool containsAllSizes = allSizes.All(s => vtSizes.Contains(s));

                            if (!containsAllSizes)
                            {
                                isAllSize = false;
                            }
                        }
                        else
                        {
                            isAllSize = false;
                        }
                        string sizeString = nhomSize + ":" + string.Join(",", vtSizes);
                        sizeStrings.Add(sizeString);
                    }
                    if (isAllSize && sizeStrings.Count > 0)
                    {
                        sizeValue = "AllSize";
                    }
                    else
                    {
                        sizeValue = string.Join("; ", sizeStrings);
                    }
                }

                row["Size"] = sizeValue;
            }

            return tblMain;
        }

        private void LoadVatTuThanhPhan()
        {
            try
            {
                string url = $"{URL}ERPVatTuThanhPhan/GET?action=GetCapPhatThanhPhan&para1={_MaGop}&para2={_MaLenhSanXuatlenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblVatTu = new DataTable();
                if (json != "[]")
                {
                    tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);
                }

                SetVatTu();
            }
            catch(Exception ex)
            {

            }
           
        }
        private void SetVatTu()
        {
            try
            {
                DataTable tblCopyVatTu = tblVatTu?.Copy();
                if (_tbl != null && _tbl.Rows.Count != 0)
                {
                    var keysToRemove = new HashSet<string>(
                 _tbl.AsEnumerable()
                     .Select(r => $"{r.Field<string>("MaCLVTID")}_{r.Field<string>("MaVTID")}_{r.Field<string>("MauVTID")}_{r.Field<string>("KhoVaiID")}"));

                    var rowsToDelete = tblCopyVatTu.AsEnumerable()
                        .Where(r => keysToRemove.Contains($"{r.Field<string>("MaNhomVT")}_{r.Field<string>("MaVTID")}_{r.Field<string>("MaMauVT")}_{r.Field<string>("MaKhoVT")}"))
                        .ToList();
                    foreach (var row in rowsToDelete)
                    {
                        tblCopyVatTu.Rows.Remove(row);

                    }

                }

                DataTable dtAllSizes = GetBangSize(_MaGop);
                DataTable dtERPSizes = GetERPSIZESP(_MaGop);
                grcDinhMucVTThanhPhan.DataSource = XuLyCotSize(tblCopyVatTu, dtAllSizes, dtERPSizes); ;


            }
            catch (Exception ex)
            {

            }

        }
        private void grvDinhMucVTThanhPhan_DataSourceChanged(object sender, EventArgs e)
        {
            if(grvDinhMucVTThanhPhan.RowCount > 0)
            {
                DataRow rowFocused = grvDinhMucVTThanhPhan.GetDataRow(0);
                if(rowFocused!= null)
                {
                    LoadVatTuThanhPhanDetail(rowFocused);
                }
            }
        }

        private void grvDinhMucVTThanhPhan_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow rowFocused = grvDinhMucVTThanhPhan.GetFocusedDataRow();
            if (rowFocused != null)
            {
                LoadVatTuThanhPhanDetail(rowFocused);
                focused(sender);
            }
        }
        private void grvChooseVatTu_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }


        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;
                if (view.FocusedColumn.FieldName == "IsCheck")
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;


                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {

            }


        }
        private void LoadVatTuThanhPhanDetail(DataRow rowVatTuThanhPhan)
        {
            try
            {
                DataTable tblVatTuThanhPhamDetail = new DataTable();
                string url = $"{URL}ERPVatTuThanhPhan/GET?Action=GetVatTuTPChiTiet_Selected&para1={rowVatTuThanhPhan["ID_VatTuThanhPhan"]}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblVatTuThanhPhamDetail = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcVattuTP_Detail.DataSource = tblVatTuThanhPhamDetail;
                grcVattuTP_Detail.RefreshDataSource();
                grvVattuTP_Detail.FocusedRowHandle = 0;
            }
            catch(Exception ex)
            {

            }
           
        }


        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == DevExpress.XtraGrid.GridControl.InvalidRowHandle)
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

        private void grvChooseVatTu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
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

        private void griview_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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

        private void grv_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null || e.Value == DBNull.Value || e.Value.ToString() == "")
                {
                    if (e.Column.FieldName == "DinhMuc") e.DisplayText = "-";
                    return;
                }

                var view = sender as GridView;

                if (e.Column.FieldName == "DinhMuc")
                {

                    if (e.Value == null || e.Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(e.Value?.ToString()) ||
                        !decimal.TryParse(e.Value.ToString(), out decimal value) ||
                        value == 0m)
                    {
                        e.DisplayText = "-";
                        return;
                    }


                    var nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    nfi.NumberGroupSeparator = ",";

                    int decimals = (decimal.GetBits(value)[3] >> 16) & 0x000000FF;
                    decimals = decimals == 0 ? 0 : Math.Min(decimals, 4);

                    string format = decimals == 0 ? "N0" : $"N{decimals}";

                    e.DisplayText = value.ToString(format, nfi);
                    return;
                }




            }
            catch (Exception ex)
            {

            }
        }

        private void GrvRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            if (e.Column.FieldName == "DinhMuc" || e.Column.FieldName == "DM_VatTuTP")
            {
                e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            }


        }

        private void btnSubmit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            DataTable tblSelected = grcDinhMucVTThanhPhan.DataSource as DataTable;
            if (tblSelected != null && tblSelected?.Rows?.Count > 0)
            {

                var lstRowSelect = tblSelected.AsEnumerable()
              .Where(r => !r.IsNull("IsCheck") && r.Field<bool>("IsCheck"));

                tblVatTuSelected = lstRowSelect.Any()
                    ? lstRowSelect.CopyToDataTable()
                    : tblSelected.Clone();

                if (lstRowSelect.Any())
                {
                    string IDSelected = string.Join(";", lstRowSelect.Select(x => x["ID_VatTuThanhPhan"]?.ToString()));
                    GetVatTuDMThanhPhan_Detail(IDSelected);
                }

                this.DialogResult = DialogResult.OK;


            }
        }

        private void GetVatTuDMThanhPhan_Detail(string IDSelected)
        {
            try
            {
                string url = $"{URL}ERPVatTuThanhPhan/GET?action=GetVatTuTPChiTiet_Selected&para1={IDSelected}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblVatTuDM_DetailSelected = new DataTable();
                if (json != "[]")
                {
                    tblVatTuDM_DetailSelected = JsonConvert.DeserializeObject<DataTable>(json);
                }

               
            }
            catch (Exception ex)
            {

            }
        }
    }
}