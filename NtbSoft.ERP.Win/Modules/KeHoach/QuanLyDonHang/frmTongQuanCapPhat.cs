using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTab;
using DevExpress.XtraWaitForm;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ResourceForm;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{

    public partial class frmTongQuanCapPhat : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        //private const string DB_Name_QLSX = "PMS_DONGAMEX_2025";

        List<DevExpress.XtraGrid.Columns.GridColumn> lstColGroupDraw = new List<DevExpress.XtraGrid.Columns.GridColumn>();
        int _rowhandle = 0;
        int indexFocusedRow;
        string _inseamid = string.Empty, _mamau = string.Empty, _poid = string.Empty, mahangselected = string.Empty, poselected = string.Empty;
        bool indicatorIcon = true;

        string _selectedMaKH;
        string _selectedMaHang;
        DataTable tbldh;
        bool isall;
        public frmTongQuanCapPhat()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            InitComponentSearchLookup();            
            SetupGroupLevelColors();
            loadcombobox();
        }

        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;
        private void SetupGroupLevelColors()
        {
            groupLevelColorBackground = new Dictionary<int, Color>
            {
                { -1, ColorTranslator.FromHtml("#DDEBFB") },
                { 0, ColorTranslator.FromHtml("#DDEBFB") }, 
                { 1, ColorTranslator.FromHtml("#FFE2D3") }, 
                { 2, ColorTranslator.FromHtml("#D7F5E8") }  
            };

            groupLevelColors = new Dictionary<int, Color>
            {
                { -1, ColorTranslator.FromHtml("#2A5D9F") },
                { 0, ColorTranslator.FromHtml("#2A5D9F") }, 
                { 1, ColorTranslator.FromHtml("#A53E25") },
                { 2, ColorTranslator.FromHtml("#2E7D5B") }  
            };


        }
        private void InitComponentSearchLookup()
        {

            rSearchLookupEdit_KhachHang.ValueMember = "MaKH";
            rSearchLookupEdit_KhachHang.DisplayMember = "TenKH";

            rSearchLookupEdit_MaHang.ValueMember = "MaHang";
            rSearchLookupEdit_MaHang.DisplayMember = "TenHang";

           
            lstColGroupDraw.AddRange(new[] { colIsNPL, colTenNhom ,colTenDVSX_Detail});
            LoadKhachHang();
        }
        private void LoadMaHang(string MaKH)
        {
            try
            {
                string url = string.Format("{0}?action={1}&&para1={2}", URL + "TongQuanCapPhat/Get", "GetMaHang", MaKH);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    rSearchLookupEdit_MaHang.DataSource = null;
                    ItemSearchLookup_MaHang.EditValue = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                rSearchLookupEdit_MaHang.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }


        }
        private void LoadKhachHang()
        {
            try
            {
                string url = string.Format("{0}?action={1}&&para1=NONE", URL + "TongQuanCapPhat/Get", "GetKH");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    rSearchLookupEdit_KhachHang.DataSource = null;
                    ItemSearchLookup_KhachHang.EditValue = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                rSearchLookupEdit_KhachHang.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }

        }
        private void LoadTQ_CapPhat(string MaKH, string MaHang = "NONE")
        {
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Lấy Dữ Liệu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");

                barEditItem1.Visibility = BarItemVisibility.Never;
                barEditItem2.Visibility = BarItemVisibility.Never;
                barEditItem3.Visibility = BarItemVisibility.Never;

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}", URL + "TongQuanCapPhat/Get", "GetTongQuanCapPhat", MaKH, MaHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcTongQuanVatTu.DataSource = null;
                    grcCapPhatNPL_Detail.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                grcTongQuanVatTu.DataSource = tbl;
                grcTongQuanVatTu.RefreshDataSource();
                grvTongQuanVatTu.ExpandAllGroups();
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }

        private void ItemSearchLookup_KhachHang_EditValueChanged(object sender, EventArgs e)
        {
            // Lấy giá trị được chọn
            var barItem = sender as BarEditItem;
            if (barItem != null)
            {
                string MaKH = barItem.EditValue?.ToString();
                ItemSearchLookup_MaHang.EditValue = null;
                _selectedMaKH = barItem.EditValue?.ToString();
                //string MaHang = ItemSearchLookup_MaHang.EditValue?.ToString();
                if (!string.IsNullOrEmpty(MaKH))
                {
                    
                    LoadMaHang(MaKH);
                    if(TabSelected.SelectedTabPage == tabCapPhatNPL)
                    {
                        LoadTQ_CapPhat(MaKH);
                    }
                    else if(TabSelected.SelectedTabPage == tabDonHang)
                    {
                        LoadDonHang(MaKH, "");
                    }
                   
                   
                    
                }
            }
        }

        private void ItemSearchLookup_MaHang_EditValueChanged(object sender, EventArgs e)
        {
            var barItem = sender as BarEditItem;
            if (barItem != null)
            {
                string MaHang = barItem.EditValue?.ToString();
                string MaKH = ItemSearchLookup_KhachHang.EditValue?.ToString();
                _selectedMaHang = barItem.EditValue?.ToString();
           

                if (!string.IsNullOrEmpty(MaHang))
                {
                    if (TabSelected.SelectedTabPage == tabCapPhatNPL)
                    {
                        LoadTQ_CapPhat(MaKH, MaHang);
                    }
                    else if (TabSelected.SelectedTabPage == tabDonHang)
                    {
                        LoadDonHang(MaKH, MaHang);
                    }
                   
                    
                }
                else
                {

                    if (TabSelected.SelectedTabPage == tabCapPhatNPL)
                    {
                        LoadTQ_CapPhat(MaKH);
                    }
                    else if (TabSelected.SelectedTabPage == tabDonHang)
                    {
                        LoadDonHang(MaKH, "");
                    }

                   
                   
                }
                //_isTabCapPhatLoaded = false;
                //_isTabDonHangLoaded = false;
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int PreFocusedRow_NPL = grvTongQuanVatTu.FocusedRowHandle;
            int PreFocusedRow_DonHang = gVtong.FocusedRowHandle;
            string MaKH = ItemSearchLookup_KhachHang.EditValue?.ToString();
            string MaHang = ItemSearchLookup_MaHang.EditValue?.ToString();
            if (!string.IsNullOrEmpty(MaKH))
            {
                LoadMaHang(MaKH);
                if (string.IsNullOrEmpty(MaHang))
                {
                    if (TabSelected.SelectedTabPage == tabCapPhatNPL)
                    {
                        LoadTQ_CapPhat(MaKH);
                    }
                    else if (TabSelected.SelectedTabPage == tabDonHang)
                    {
                        LoadDonHang(MaKH, "");
                    }
                    
                }
                else
                {
                    if (TabSelected.SelectedTabPage == tabCapPhatNPL)
                    {
                        LoadTQ_CapPhat(MaKH, MaHang);
                    }
                    else if (TabSelected.SelectedTabPage == tabDonHang)
                    {
                        LoadDonHang(MaKH, MaHang);
                    }
                   
                }
                grvTongQuanVatTu.FocusedRowHandle = PreFocusedRow_NPL;
                gVtong.FocusedRowHandle = PreFocusedRow_DonHang;
            }
        }

        /**/
        private void gridView_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)

        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (lstColGroupDraw.Contains(info.Column))
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            
            

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }
                
               
             
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }

        }
        private void gridView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void gridView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }
        private void grvTongQuanVatTu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
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
           
        /***/     
        private void HandleLoadDataChiTiet()
        {
            if (gVtong.FocusedRowHandle >= 0)
            {
                indexFocusedRow = gVtong.FocusedRowHandle;
            }
            GridView view = this.gVtong;
            //if (view.IsGroupRow(view.FocusedRowHandle))
            //{
            int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);

            //object mahang = view.GetRowCellValue(childHandle, gridColumn1);
            //object po = view.GetRowCellValue(childHandle, gridColumn2);
            object inseamid = view.GetRowCellValue(childHandle, gridColumn4);
            object mamau = view.GetRowCellValue(childHandle, gridColumn9);
            object poid = view.GetFocusedRowCellValue(gridColumn8);
            //object nguoitao = view.GetRowCellValue(childHandle, colNguoiTao);


            if (inseamid != null) _inseamid = inseamid.ToString();
            if (mamau != null) _mamau = mamau.ToString();
            if (poid != null) _poid = poid.ToString();
            // if (nguoitao != null) _nguoitao = nguoitao.ToString();
            GetChiTietDonHangTongPO();
            bandedGridView1.ExpandAllGroups();
        }

        private void GetChiTietDonHangTongPO()
        {
            try
            {
                GridView view = gVtong;
                string _makh = ItemSearchLookup_KhachHang?.EditValue.ToString();
                string _mahang;
                if (ItemSearchLookup_MaHang.EditValue != null)
                {
                    _mahang = ItemSearchLookup_MaHang.EditValue.ToString();
                }
                else
                {
                    _mahang = view.GetFocusedRowCellValue(this.gridColumn1) as string;
                }
                string inseamId = view.GetFocusedRowCellValue(this.gridColumn4) as string;
                string maMau = view.GetFocusedRowCellValue(this.gridColumn9) as string;
                string poId = view.GetFocusedRowCellValue(this.gridColumn8) as string;
                string MaDH = view.GetFocusedRowCellValue(this.colDH_Tong) as string;
                if (!string.IsNullOrEmpty(_makh))
                {
                    string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}",
                        URL + "TongQuanCapPhat/Get", "GetChiTiet", MaDH, _mahang, poId, maMau, inseamId);
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    //gCCT.MainView = GetBandGridViewAmount(tbl);
                    //CreateBandGridSize_Detail(tbl,bandedGridView1 , bgSize_PO_DH);
                    createColBandsSize(tbl);
                    AddColumnSumRow(tbl);
                    gCCT.DataSource = tbl;


                }
                else
                {
                    gCCT.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void createColBandsSize(DataTable dt)
        {
            try
            {
                ClearBand(bgSize_PO_DH,bandedGridView1);

                //ClearDataBandAndCol();
                int visibleIndex = 0;
                foreach (DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    if (!colName.Contains('@')) continue;
                    var _sizeID = colName.Split('@')[2];
                    var _size = colName.Split('@')[0]; ;
                    if (!CheckExistBand(_sizeID)) continue;
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = _size;
                    col.FieldName = _size + "@Size@" + _sizeID;
                    col.Name = "col" + _sizeID;
                    col.OptionsColumn.AllowEdit = true;
                    col.ColumnEdit = null;
                    col.Visible = true;
                    col.Width = 40;
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                    gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                    gb.AppearanceHeader.Options.UseBackColor = true;
                    gb.AppearanceHeader.Options.UseFont = true;
                    gb.AppearanceHeader.Options.UseForeColor = true;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.Caption = _size;
                    gb.Columns.Add(col);
                    gb.Name = "gb" + "@Size@" + _sizeID;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;

                    GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                    itemSize.FieldName = col.FieldName;
                    itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    itemSize.DisplayFormat = "{0:n0}";
                    itemSize.ShowInGroupColumnFooter = col;
                    bandedGridView1.GroupSummary.Add(itemSize);

                    bgSize_PO_DH.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }

              

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private DataTable AddColumnSumRow(DataTable tbl)
        {
            tbl.Columns.Add("TongSL", typeof(int));
            foreach (DataRow dataRow in tbl.Rows)
            {
                int tong = 0;
                foreach (DataColumn dataColumn in tbl.Columns)
                {
                    if (dataColumn.ColumnName.Contains("@Size@"))
                    {
                        int sl = 0;
                        bool parse = int.TryParse(dataRow[dataColumn.ColumnName].ToString(), out sl);
                        if (parse)
                        {
                            tong += sl;
                        }
                    }
                }
                dataRow["TongSL"] = tong;
            }
            return tbl;
        }

        private void BandedView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void BandedView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }
        
        private int FindFirstDataRowInGroup(GridView view, int groupRowHandle)
        {
            int childCount = view.GetChildRowCount(groupRowHandle);

            for (int i = 0; i < childCount; i++)
            {
                int childHandle = view.GetChildRowHandle(groupRowHandle, i);

                if (view.IsGroupRow(childHandle))
                {
                    int result = FindFirstDataRowInGroup(view, childHandle);
                    if (result >= 0)
                        return result;
                }
                else
                {
                    return childHandle;
                }
            }

            return -1; 
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
       
            if (_selectedMaKH == null) return;
            
            if (e.Page == tabCapPhatNPL)
            {
              
                if (!string.IsNullOrEmpty(_selectedMaHang))
                    LoadTQ_CapPhat(_selectedMaKH, _selectedMaHang);
                else
                    LoadTQ_CapPhat(_selectedMaKH);
              
            }
            else if (e.Page == tabDonHang )
            {
              
                LoadDonHang(_selectedMaKH, _selectedMaHang ?? "");
                
            }
        }

        private void grvTongQuanVatTu_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if ((e.Column == colCapPhat || e.Column == colSLXuat || e.Column == colThucNhap || e.Column == colChuaNhan) && e.Value != null && e.Value != DBNull.Value)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.DisplayText = value.ToString("##,0.00");
                }
                else
                {
                    e.DisplayText = "-";
                }
            }
            else if((e.Column == colThieu || e.Column == colThua ))
            {
                if(e.Value == null || e.Value == DBNull.Value)
                {
                    e.DisplayText = "-";
                }
                else if (decimal.TryParse(e.Value.ToString(), out decimal value) && value != 0)
                {
                    e.DisplayText = value.ToString("##,0.00");
                }
                else 
                {
                    e.DisplayText = "-";
                }
            }
            
        }

        private int CalculateTotalValueFromRow(DataRow row)
        {
            int sum = 0;

            foreach (DataColumn col in row.Table.Columns)
            {
                if (col.ColumnName.Contains("@"))
                {
                    var val = row[col];
                    if (val != DBNull.Value && int.TryParse(val.ToString(), out int num))
                    {
                        sum += num;
                    }
                }
            }

            return sum;
        }


        /*Split Concontrol Event*/

        private void splitContainerControl2_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl2.SplitterPosition = (int)(splitContainerControl2.Height * 0.6);
        }
        
        private void splitContainerControl4_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl3.SplitterPosition = (int)(splitContainerControl3.Height * 0.5);
            splitContainerControl4.SplitterPosition = (int)(splitContainerControl4.Width * 0.5);
         
        }

        private void BandedView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DauSize")
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "0";
                }

            }
            else if(e.Column.FieldName.Contains("@"))
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "-";
                }
                else
                {
                    string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Count() > 1)
                    {
                        if (Convert.ToInt32(e.Value.ToString()) == 0)
                            e.DisplayText = "-";
                    }
                }
            }
        }
              
        private void CreateBandGridSize_Detail(DataTable dt, BandedGridView BandedGridView_Detail, GridBand GridBandSize)
        {

            ClearBand(GridBandSize, BandedGridView_Detail);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[2];
                if (!CheckExistBand(_sizeID, GridBandSize)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName =  $"Size@{_sizeID}@{_size}";
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
               
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                itemSize.FieldName = col.FieldName;
                itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                itemSize.DisplayFormat = "{0:n0}";
                itemSize.ShowInGroupColumnFooter = col;
                BandedGridView_Detail.GroupSummary.Add(itemSize);

                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1)
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
                BandedGridView_Detail.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                GridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private void ClearBand(GridBand GridBandSize, BandedGridView BandedGridView_Detail)
        {
            GridBandSize.Children.Clear();
            RemoveColumnSize(BandedGridView_Detail);

        }
        private void RemoveColumnSize(BandedGridView BandedGridView_Detail)
        {
            for (int i = 0; i < BandedGridView_Detail.Columns.Count;)
            {
                if (BandedGridView_Detail.Columns[i].FieldName.Contains("@Size@"))
                {
                    BandedGridView_Detail.Columns.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }
        }
        private bool CheckExistBand(string size, GridBand GridBandSize)
        {

            GridBand gbCheck = GridBandSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private bool CheckExistBand(string size)
        {
            GridBand gbCheck = gridBand8.Children.Where(x => x.Name == "gb" + "@Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private void LoadDetail()
        {
            DataRow row_focused = gVtong.GetFocusedDataRow();
            if (row_focused != null)
            {
                string MaDH = row_focused["MaDH"]?.ToString();
                string MaHang = row_focused["MaHang"]?.ToString();
                string POID = row_focused["POID"]?.ToString();
                string MaMau = row_focused["MaMau"]?.ToString();
                string DauSizeID = row_focused["DauSizeID"]?.ToString();
           
                var selectedTab = XtraTabSelected_Detail.SelectedTabPage; // hoặc ((XtraTabControl)sender).SelectedTabPage;

                switch (selectedTab.Name)
                {
                    case "TabDonHangPO_Detail":
                        HandleLoadDataChiTiet();
                        break;

                    case "TabLSX_PO_Detail":
                       LoadTH_PO_SX_Detail(MaDH, POID, MaHang, MaMau, DauSizeID);
                        break;

                    case "TabCat_Detail":
                        LoadCat_Detail(MaDH, POID, MaHang, MaMau, DauSizeID);
                        break;

                    case "TabMay_Detail":
                        LoadMay_Detail(MaDH, POID, MaHang, MaMau, DauSizeID);
                        break;

                    case "TabDongThung_Detail":
                        LoadDongThung_Detail(MaDH, POID, MaHang, MaMau, DauSizeID);
                        break;

                    case "TabNhapKho_Detail":
                        LoadNhapKho_Detail(MaDH, POID, MaHang, MaMau, DauSizeID);
                        break;

                    default:
                       
                        break;
                }
            }

        }
        private void XtraTabSelected_Detail_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            var tabControl = sender as DevExpress.XtraTab.XtraTabControl;

            if (tabControl != null)
            {
                    
                LoadDetail();
            }
        }

        #region GridView Tổng Quan NPL

        private void grvTongQuanVatTu_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int Focused_RowHandel = view.FocusedRowHandle;
            if (view == null) return;
            if (!view.IsGroupRow(Focused_RowHandel) && Focused_RowHandel != GridControl.AutoFilterRowHandle)
            {
                DataRow row_focused = view.GetDataRow(Focused_RowHandel);
                if (row_focused != null)
                {
                    string MaVTID = row_focused["MaVTID"]?.ToString();
                    string MaMauID = row_focused["MaMauVT"]?.ToString();
                    string MaNhom = row_focused["MaNhomVT"]?.ToString();
                    string KhoVaiID = row_focused["MaKhoVT"]?.ToString();
                    string MaHang = row_focused["MaHang"]?.ToString();
                    LoadCapPhat_NPL_Detail(MaVTID, MaMauID, MaNhom, KhoVaiID, MaHang);
                }
            }
        }

        private void grvTongQuanVatTu_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (view.RowCount > 0)
            {
                DataRow row_focused = view.GetDataRow(0);
                if (row_focused != null)
                {
                    string MaVTID = row_focused["MaVTID"]?.ToString();
                    string MaMauID = row_focused["MaMauVT"]?.ToString();
                    string MaNhom = row_focused["MaNhomVT"]?.ToString();
                    string KhoVaiID = row_focused["MaKhoVT"]?.ToString();
                    string MaHang = row_focused["MaHang"]?.ToString();
                    LoadCapPhat_NPL_Detail(MaVTID, MaMauID, MaNhom, KhoVaiID, MaHang);
                }
            }
        }

        /*draw group màu footer gird NPL*/
        private void grvTongQuanVatTu_CustomDrawRowFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;


            int groupLevel = view.GetRowLevel(e.RowHandle);

            Color backColor = Color.FromArgb(255, 239, 204);


            if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color levelColor))
            {
                backColor = levelColor;
            }


            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, backColor, backColor, 90))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }


            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void grvTongQuanVatTu_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);


            if (groupLevel == 2 && groupLevelColors != null && groupLevelColors.TryGetValue(groupLevel, out Color groupColor))
            {
                e.Appearance.ForeColor = groupColor;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //e.Handled = true;
            }
            else if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color fallbackColor))
            {
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, fallbackColor, fallbackColor, 90))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }


                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);

                e.Handled = true;
            }

        }

        #endregion

        #region GridView Cấp Phát Nguyên Phụ Liệu QLSX
        private void LoadCapPhat_NPL_Detail(string MaVTID, string MauVTID, string MaNhom, string KhoVaiID,string MaHang)
        {
            try
            {
                //if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null && !DevExpress.XtraSplashScreen.SplashScreenManager.Default.IsSplashFormVisible)
                //{
                //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                //    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Lấy Dữ Liệu");
                //    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                //}

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}", URL + "TongQuanCapPhat/Get", "Get_CapPhat_LSX", MaVTID, MauVTID, MaNhom, KhoVaiID,MaHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcCapPhatNPL_Detail.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                grcCapPhatNPL_Detail.DataSource = tbl;
                grcCapPhatNPL_Detail.RefreshDataSource();
                grvCapPhatNPL_Detail.ExpandAllGroups();
            }
            catch (Exception ex)
            {
              
                           
                //if (DevExpress.XtraSplashScreen.SplashScreenManager.Default.IsSplashFormVisible)
                //{
                //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                //}

            }
            finally
            {
               
                //if (DevExpress.XtraSplashScreen.SplashScreenManager.Default.IsSplashFormVisible)
                //{
                //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                //}


            }

        }
        private void grvCapPhatNPL_Detail_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (lstColGroupDraw.Contains(info.Column))
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
          
            if (grvCapPhatNPL_Detail.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
        private void grvCapPhatNPL_Detail_CustomDrawRowFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;


            int groupLevel = view.GetRowLevel(e.RowHandle);

            Color backColor = Color.FromArgb(255, 239, 204);


            if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color levelColor))
            {
                backColor = levelColor;
            }


            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, backColor, backColor, 90))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }


            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void grvCapPhatNPL_Detail_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);


            if (groupLevelColors != null && groupLevelColors.TryGetValue(groupLevel, out Color groupColor))
            {
                e.Appearance.ForeColor = groupColor;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //e.Handled = true;
            }
            else if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color fallbackColor))
            {
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, fallbackColor, fallbackColor, 90))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }


                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);

                e.Handled = true;
            }


        }
        #endregion

        #region GridView Đơn Hàng Tổng
        private void LoadDonHang(string makh, string mahang = "NONE")
        {
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Lấy Dữ Liệu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                barEditItem3.Visibility = BarItemVisibility.Always;

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}", URL + "TongQuanCapPhat/Get", "GetDH", makh, mahang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    gCtong.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gCtong.DataSource = tbl;
                //gCtong.RefreshDataSource();
                //gVtong.ExpandAllGroups();
                gVtong.ExpandAllGroups();
                gVtong.BeginUpdate();
                try
                {

                    int rowHandle = -1;

                    for (int i = 0; i < gVtong.RowCount; i++)
                    {
                        if (!gridView1.IsGroupRow(i))
                        {
                            rowHandle = i;
                            break;
                        }
                    }

                    if (rowHandle >= 0)
                    {
                        gVtong.FocusedRowHandle = rowHandle;
                    }
                }
                finally
                {



                    gVtong.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }
        private void LoadDonHang(string makh, string mahang, string datefrom = "NONE", string dateto = "NONE")
        {
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Lấy Dữ Liệu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");

                barEditItem3.Visibility = BarItemVisibility.Always;

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}", URL + "TongQuanCapPhat/Get", "GetDH", makh, mahang, datefrom, dateto);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcTongQuanVatTu.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gCtong.DataSource = tbl;
                //gCtong.RefreshDataSource();
                //gVtong.ExpandAllGroups();
                gVtong.ExpandAllGroups();
                gVtong.BeginUpdate();
                try
                {

                    int rowHandle = -1;

                    for (int i = 0; i < gVtong.RowCount; i++)
                    {
                        if (!gridView1.IsGroupRow(i))
                        {
                            rowHandle = i;
                            break;
                        }
                    }

                    if (rowHandle >= 0)
                    {
                        gVtong.FocusedRowHandle = rowHandle;
                    }
                }
                finally
                {



                    gVtong.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }
        private void gVtong_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
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
        private void gVtong_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            _rowhandle = gVtong.FocusedRowHandle;
            if (!view.IsGroupRow(_rowhandle) && _rowhandle != GridControl.AutoFilterRowHandle)
            {
                HandleLoadDataChiTiet();
                DataRow row_focused = gVtong.GetFocusedDataRow();
                if(row_focused!= null)
                {
                    string MaDH = row_focused["MaDH"]?.ToString();
                    string MaHang = row_focused["MaHang"]?.ToString();
                    string POID = row_focused["POID"]?.ToString();
                    string MaMau = row_focused["MaMau"]?.ToString();
                    string DauSizeID = row_focused["DauSizeID"]?.ToString();
                    LoadTH_TongHopPO(MaDH, POID, MaHang,MaMau,DauSizeID);
                    LoadDetail();
                }
            }


        }
        private void gVtong_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (view == null || info == null) return;
            GridColumn groupColumn = info.Column;
            int groupLevel = view.GetRowLevel(e.RowHandle);

            if (groupColumn == gridColumn2)
            {
                GridGroupSummaryItem summaryItem = view.GroupSummary
                    .OfType<GridGroupSummaryItem>()
                    .FirstOrDefault(x => x.FieldName == "SoLuong" && x.SummaryType == SummaryItemType.Sum);

                if (summaryItem != null)
                {
                    object sumValue = view.GetGroupSummaryValue(e.RowHandle, summaryItem);
                    decimal soLuong = sumValue != null && sumValue != DBNull.Value ? Convert.ToDecimal(sumValue) : 0;

                    // Gán lại GroupText
                    info.GroupText = $"PO:{info.GroupValueText} - Số Lượng: {soLuong:N0}";
                }
                else
                {
                    // Nếu không tìm thấy SummaryItem, fallback
                    info.GroupText = info.GroupValueText;
                }
            }
            if (groupColumn == gridColumn1)
            {
                GridGroupSummaryItem summaryItem = view.GroupSummary
                    .OfType<GridGroupSummaryItem>()
                    .FirstOrDefault(x => x.FieldName == "SoLuong" && x.SummaryType == SummaryItemType.Sum);

                if (summaryItem != null)
                {
                    object sumValue = view.GetGroupSummaryValue(e.RowHandle, summaryItem);
                    decimal soLuong = sumValue != null && sumValue != DBNull.Value ? Convert.ToDecimal(sumValue) : 0;

                    // Gán lại GroupText
                    info.GroupText = $"Mã Hàng: {info.GroupValueText} - Số Lượng: {soLuong:N0}";
                }
                else
                {
                    // Nếu không tìm thấy SummaryItem, fallback
                    info.GroupText = info.GroupValueText;
                }
            }
            if (groupColumn == gridColumn7)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (view.IsGroupRow(e.RowHandle))
            {

                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.DefaultDraw();
                e.Handled = true;
            }



        }
        private void gVtong_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if ((e.Column == colSLTongDH) && e.Value != null && e.Value != DBNull.Value)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.DisplayText = string.Format("{0:#,0}", value);
                }
                else
                {
                    e.DisplayText = "-";
                }
            }

        }

        /*draw group màu footer  DH Tổng*/
        private void gVtong_CustomDrawRowFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            int groupLevel = view.GetRowLevel(e.RowHandle);

            Color backColor = Color.FromArgb(255, 239, 204);


            if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color levelColor))
            {
                backColor = levelColor;
            }


            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, backColor, backColor, 90))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }


            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void gVtong_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            int groupLevel = view.GetRowLevel(e.RowHandle); // Xác định group level

            // Tô màu nền
            if (groupLevelColorBackground.ContainsKey(groupLevel))
            {
                e.Appearance.BackColor = groupLevelColorBackground[groupLevel];
            }

            // Tô màu chữ
            if (groupLevelColors.ContainsKey(groupLevel))
            {
                e.Appearance.ForeColor = groupLevelColors[groupLevel];
            }

            // In đậm chữ
            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
        }
        #endregion

        #region GridView Thực Hiện PO Tổng Hợp
        private void LoadTH_TongHopPO(string MaDH,string POID,string MaHang,string MaMau,string DauSizeID)
        {
            try
            {
                
                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}", URL + "TongQuanCapPhat/Get", "GetPO_ThucHien", MaDH, MaHang, POID, MaMau, DauSizeID);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcPO_TH_TongHop.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                grcPO_TH_TongHop.DataSource = tbl;
                grcPO_TH_TongHop.RefreshDataSource();
                grvPO_TH_TongHop.ExpandAllGroups();
            }
            catch (Exception ex)
            {
          

            }
            
        }
        private void BandedGridView_TH_TongHop_PO_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column.FieldName == "TenDVSX")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (grvPO_TH_TongHop.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
        #endregion

        #region GridView PO Đơn Hàng Tổng

        #endregion

        #region GridView Kế hoạch PO QLSX
        private void LoadTH_PO_SX_Detail(string MaDH, string POID, string MaHang, string MaMau, string DauSizeID)
        {
            try
            {

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}", URL + "TongQuanCapPhat/Get", "GetKHPO_SX_Detail", MaDH, MaHang, POID, MaMau, DauSizeID);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcKH_PO_SX_Detail.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if(tbl?.Rows?.Count > 0)
                {
                    CreateBandGridSize_Detail(tbl, bgvKH_PO_SX_Detail, gbSize_PO_SX_Detail);
                    grcKH_PO_SX_Detail.DataSource = tbl;               
                }
                grcKH_PO_SX_Detail.RefreshDataSource();
                bgvKH_PO_SX_Detail.ExpandAllGroups();
            }
            catch (Exception ex)
            {




            }

        }
        private void BandedGridView_TH_PO_SX_Detail_DrawGroup(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column.FieldName == "TenDVSX")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (bgvKH_PO_SX_Detail.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
        private void bgvKH_PO_SX_Detail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (!e.IsGetData) return;

            var view = sender as BandedGridView;
            var rowView = e.Row as DataRowView;
            if (rowView == null) return;

            e.Value = CalculateTotalValueFromRow(rowView.Row);
        }

        #endregion

        #region GridView Cắt
        private void LoadCat_Detail(string MaDH, string POID, string MaHang, string MaMau, string DauSizeID)
        {
            try
            {

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}", URL + "TongQuanCapPhat/Get", "GetCat_PO_Detail", MaDH, MaHang, POID, MaMau, DauSizeID);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcCat_Detail.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl?.Rows?.Count > 0)
                {
                    CreateBandGridSize_Detail(tbl, bgvCat_Detail, gbSize_Cat_Detail);
                    grcCat_Detail.DataSource = tbl;
                }
                grcCat_Detail.RefreshDataSource();
                bgvCat_Detail.ExpandAllGroups();
            }
            catch (Exception ex)
            {




            }

        }
        private void BandedGridView_Cat_Detail_DrawGroup(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if ( info.Column.FieldName == "TenDVSX")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (bgvCat_Detail.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void bgvCat_Detail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (!e.IsGetData) return;

            var view = sender as BandedGridView;
            var rowView = e.Row as DataRowView;
            if (rowView == null) return;

            e.Value = CalculateTotalValueFromRow(rowView.Row);
        }
        #endregion

        #region GridView May

        private void LoadMay_Detail(string MaDH, string POID, string MaHang, string MaMau, string DauSizeID)
        {
            try
            {

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}", URL + "TongQuanCapPhat/Get", "GetRC_Po_Detail", MaDH, MaHang, POID, MaMau, DauSizeID);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcMay_Detail.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl?.Rows?.Count > 0)
                {
                    CreateBandGridSize_Detail(tbl, bgrvMay_Detail, gbSize_May_Detail);
                    grcMay_Detail.DataSource = tbl;
                }
                grcMay_Detail.RefreshDataSource();
                bgrvMay_Detail.ExpandAllGroups();
            }
            catch (Exception ex)
            {




            }

        }
        private void BandedGridView_May_Detail_DrawGroup(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column.FieldName == "TenDVSX")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (bgrvMay_Detail.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
        private void bgrvMay_Detail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (!e.IsGetData) return;

            var view = sender as BandedGridView;
            var rowView = e.Row as DataRowView;
            if (rowView == null) return;

            e.Value = CalculateTotalValueFromRow(rowView.Row);
        }

        #endregion

        #region GridView Đóng Thùng
        private void LoadDongThung_Detail(string MaDH, string POID, string MaHang, string MaMau, string DauSizeID)
        {
            try
            {

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}", URL + "TongQuanCapPhat/Get", "GetDongThung_PO_Detail", MaDH, MaHang, POID, MaMau, DauSizeID);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    gcDongThung_Detail.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl?.Rows?.Count > 0)
                {
                    CreateBandGridSize_Detail(tbl, bgvDongThung_Detail, gbSize_DongThung_Detail);
                    gcDongThung_Detail.DataSource = tbl;
                }
                gcDongThung_Detail.RefreshDataSource();
                bgvDongThung_Detail.ExpandAllGroups();
            }
            catch (Exception ex)
            {




            }

        }
        private void BandedGridView_DongThung_Detail_DrawGroup(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if ( info.Column.FieldName == "TenDVSX")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (bgvDongThung_Detail.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
        private void bgvDongThung_Detail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (!e.IsGetData) return;

            var view = sender as BandedGridView;
            var rowView = e.Row as DataRowView;
            if (rowView == null) return;

            e.Value = CalculateTotalValueFromRow(rowView.Row);
        }
        #endregion

        #region GridView Nhập Kho
        private void LoadNhapKho_Detail(string MaDH, string POID, string MaHang, string MaMau, string DauSizeID)
        {
            try
            {

                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}&&para5={6}", URL + "TongQuanCapPhat/Get", "GetNhapKho_PO_Detail", MaDH, MaHang, POID, MaMau, DauSizeID);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    grcNhapKho_Detail.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl?.Rows?.Count > 0)
                {
                    CreateBandGridSize_Detail(tbl, bgvNhapKho_Detail, gbSize_NhapKho_Detail);
                    grcNhapKho_Detail.DataSource = tbl;
                }
                grcNhapKho_Detail.RefreshDataSource();
                bgvNhapKho_Detail.ExpandAllGroups();
            }
            catch (Exception ex)
            {

            }

        }

        private void BandedGridView_NhapKho_Detail_DrawGroup(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if ( info.Column.FieldName == "TenDVSX")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

            if (bgvNhapKho_Detail.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void bgvNhapKho_Detail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (!e.IsGetData) return;

            var view = sender as BandedGridView;
            var rowView = e.Row as DataRowView;
            if (rowView == null) return;

            e.Value = CalculateTotalValueFromRow(rowView.Row);
        }
        #endregion

        #region Style Grid View

        /*HEADER*/
        private void BandedViewCapPhatNPL_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;

            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);
            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {
                ControlPaint.DrawBorder3D(e.Graphics, rect);
                rect.Inflate(-1, -1);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color backColor = ColorTranslator.FromHtml("#FFD480"); // default
                    switch (e.Band.Name)
                    {
                        case "bgThieu": backColor = ColorTranslator.FromHtml("#F8B6B6"); break;
                        case "gbThua": backColor = ColorTranslator.FromHtml("#C5E3BF"); break;
                    }

                
                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }

                }

                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                    e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
                        };

                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }
                }


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

        private void BandedViewChiTietPO_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;


            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);

           
            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {
              
                ControlPaint.DrawBorder3D(e.Graphics, rect);

              
                rect.Inflate(-1, -1);

                
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color backColor = ColorTranslator.FromHtml("#FFD480");
                   
                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }
                }

             
                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                 e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
                        };

                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }
                }

               
                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }

                e.Handled = true;
            }
            catch (ArgumentException ex)
            {

            }
        }

        private void BandedGridView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;

            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);
            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {
                ControlPaint.DrawBorder3D(e.Graphics, rect);
                rect.Inflate(-1, -1);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color backColor = ColorTranslator.FromHtml("#FFD480"); // default
                    switch (e.Band.Name)
                    {
                        case "bgThieu": backColor = ColorTranslator.FromHtml("#F8B6B6"); break;
                        case "gbThua": backColor = ColorTranslator.FromHtml("#C5E3BF"); break;
                    }


                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }

                }

                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                    e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.None, 
                            FormatFlags = StringFormatFlags.LineLimit
                        };

                       
                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }

                }


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
        /*GROUP*/


        /*FOOTER*/
        private void BandGridView_Detail_CustomDrawRowFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;


            int groupLevel = view.GetRowLevel(e.RowHandle);

            Color backColor = Color.FromArgb(255, 239, 204);


            if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color levelColor))
            {
                backColor = levelColor;
            }


            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, backColor, backColor, 90))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }


            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void BandGridView_Detail_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);


            if (groupLevelColors != null && groupLevelColors.TryGetValue(groupLevel, out Color groupColor))
            {
                e.Appearance.ForeColor = groupColor;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //e.Handled = true;
            }
            else if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color fallbackColor))
            {
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, fallbackColor, fallbackColor, 90))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }


                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);

                e.Handled = true;
            }


        }


        #endregion

        /*Export excel*/
        private void btnExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (TabSelected.SelectedTabPage == tabDonHang)
                {

                    DataTable tblDH_PO = gCtong.DataSource as DataTable;
                    if (tblDH_PO != null && tblDH_PO?.Rows.Count > 0)
                    {
                        BarButtonItemLink link = (BarButtonItemLink)e.Link;
                        if (link != null)
                        {
                            // Lấy BarManager từ BarButtonItem
                            BarManager barManager = link.Item.Manager;
                            // Lấy tọa độ trên màn hình của BarButtonItem
                            Point barButtonLocation = link.Bounds.Location;
                            // Chuyển đổi tọa độ từ thanh công cụ sang màn hình
                            Point screenLocation = barManager.Form.PointToScreen(barButtonLocation);
                            string TenHang = rSearchLookupEdit_MaHang.GetDisplayValueByKeyValue(ItemSearchLookup_MaHang.EditValue)?.ToString(); ;
                            string KhachHang = rSearchLookupEdit_KhachHang.GetDisplayValueByKeyValue(ItemSearchLookup_KhachHang.EditValue)?.ToString();
                            List<DataRow> query = tblDH_PO.AsEnumerable()
                                                    .GroupBy(x => new
                                                    {
                                                        MaHang = x["MaHang"]?.ToString(),
                                                        TenHang = x["TenHang"]?.ToString(),
                                                        POID = x["POID"]?.ToString(),
                                                        PO = x["PO"]?.ToString()
                                                    })
                                                    .Select(g => g.First())
                                                    .ToList();

                            frmExportDonHangTQ frm = new frmExportDonHangTQ(screenLocation.X - 450, screenLocation.Y + link.Bounds.Height, KhachHang, TenHang, query);

                            frm.ShowDialog();
                            if (frm.DialogResult == DialogResult.OK)
                            {
                                if (frm.tblSelectedPO.Rows.Count > 0)
                                {
                                    mahangselected = string.Join(";",
                                                   frm.tblSelectedPO.AsEnumerable()
                                                       .Select(r => r["MaHang"].ToString())
                                                       .Distinct()
                                                );
                                    poselected = string.Join(";",
                                                frm.tblSelectedPO.AsEnumerable()
                                                    .Select(r => r["POID"].ToString())
                                                    .Distinct()
                                            );
                                }
                                else
                                {
                                    mahangselected = "NONE";
                                    poselected = "NONE";
                                }



                                isall = frm.IsAll;
                            }
                            else
                            {
                                return;
                            }
                        }

                    }
                }
                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
                Sfd.FileName = TabSelected.SelectedTabPage == tabDonHang
                                ? string.Format("TongQuanDonHang{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString())
                                : string.Format("TongQuanCapPhat{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());

                if (Sfd.ShowDialog() == DialogResult.OK)
                {

                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    string fileName = string.Empty;
                    if (TabSelected.SelectedTabPage == tabDonHang)
                    {
                        fileName = "TongQuanDonHangTemplate.xlsx";
                    }
                    else if (TabSelected.SelectedTabPage == tabCapPhatNPL)
                    {
                        fileName = "TongQuanCapPhatTemplate.xlsx";
                    }
                    string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                    string TemplateFileName = path;
                    string ExportFileName = Sfd.FileName;
                    if (TabSelected.SelectedTabPage == tabDonHang)
                    {
                        ExportDH(TemplateFileName, ExportFileName);
                    }
                    else if (TabSelected.SelectedTabPage == tabCapPhatNPL)
                    {
                        Export(TemplateFileName, ExportFileName);
                    }

                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Mở File", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            if (File.Exists(Sfd.FileName))
                                System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                        }
                        catch
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Error..");
                        }
                    }
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                }

            }
            catch (Exception ex)
            {
            }
        }
        public void Export(string TemplateFileName, string ExportFileName)
        {
            DataTable tbl = grcTongQuanVatTu.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;

            string[] mahangarr = tbl.AsEnumerable()
                   .Select(r => r["MaHang"].ToString())
                   .Distinct()?.ToArray();

            FileInfo file = new FileInfo(ExportFileName);
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "Cty NTB";
                excelPackage.Workbook.Properties.Title = "";

                FileInfo templateFile = new FileInfo(TemplateFileName);
                int sheetIndex = 0;

                using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                {
                    sheetIndex++;
                    foreach (string mahangtonghop in mahangarr)
                    {
                        int index = 1;
                        int rowTH = 9;
                        string sheetNameTongHop = $"{mahangtonghop}";
                        ExcelWorksheet templateSheetTH = templatePackage.Workbook.Worksheets["Sheet1"];
                        ExcelWorksheet newSheetTH = excelPackage.Workbook.Worksheets.Add(sheetNameTongHop, templateSheetTH);
                        string selectedTenHang = tbl.AsEnumerable()
                                                    .Where(r => r["MaHang"].ToString() == mahangtonghop.ToString())
                                                    .Select(r => r["TenHang"].ToString())
                                                    .FirstOrDefault();
                        string selectedTenKH = tbl.AsEnumerable()
                                                  .Where(r => r["MaKH"].ToString() == ItemSearchLookup_KhachHang.EditValue.ToString())
                                                  .Select(r => r["TenKH"].ToString())
                                                  .FirstOrDefault();

                        newSheetTH.Cells[6, 1].Value = "Khách Hàng:";
                        newSheetTH.Cells[6, 1].Style.Font.Bold = true;

                        newSheetTH.Cells[6, 2].Value = selectedTenKH.ToString();

                        newSheetTH.Cells[6, 3].Value = "Mã Hàng:";
                        newSheetTH.Cells[6, 3].Style.Font.Bold = true;

                        newSheetTH.Cells[6, 4].Value = selectedTenHang.ToString();
                        // Header cố định
                        string[] headers = { "Mã Vật Tư", "ItemCode", "Vật Tư", "Đơn Vị", "Khổ/Size", "Mã Màu Vật Tư", "Màu Vật Tư", "Nhu Cầu", "Nhập Về", "Xuất Đi", "Đáp Ứng", "", "" };
                        for (int i = 0; i < headers.Length - 2; i++)
                            newSheetTH.Cells[rowTH, i + 1].Value = headers[i];

                        newSheetTH.Cells[rowTH, 11, rowTH, 12].Merge = true;
                        newSheetTH.Cells[rowTH, 11].Value = "Chưa Nhận";
                        newSheetTH.Cells[rowTH, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        newSheetTH.Cells[rowTH, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        string[] subHeaders = { "", "", "", "", "", "", "", "", "", "", "Thừa", "Thiếu" };
                        for (int i = 0; i < subHeaders.Length; i++)
                        {
                            newSheetTH.Cells[rowTH + 1, i + 1].Value = subHeaders[i];
                        }

                        for (int i = 1; i <= 10; i++)
                        {
                            newSheetTH.Cells[rowTH, i, rowTH + 1, i].Merge = true;
                            newSheetTH.Cells[rowTH, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowTH, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }

                        using (var range = newSheetTH.Cells[rowTH, 1, rowTH + 1, 12])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            range.Style.WrapText = true;
                        }

                        for (int i = 0; i < headers.Length; i++)
                        {
                            int headerLength = headers[i].Length;
                            newSheetTH.Column(i + 1).Width = headerLength + 4;
                        }
                        newSheetTH.Column(1).Width = 20;
                        newSheetTH.Column(3).Width = 25;
                        newSheetTH.Column(7).Width = 20;
                        newSheetTH.Column(11).Width = 15;
                        newSheetTH.Column(12).Width = 15;
                        rowTH += 2;

                        var groupByNPL = tbl.AsEnumerable()
                            .Where(r => mahangtonghop.Contains(r["MaHang"].ToString()))
                            .GroupBy(r => r.Field<bool>("NPL") ? "Nguyên Liệu" : "Phụ Liệu");

                        foreach (var nplGroup in groupByNPL)
                        {
                            var nplCell = newSheetTH.Cells[rowTH, 1, rowTH, 12];
                            nplCell.Merge = true;
                            nplCell.Value = nplGroup.Key;
                            nplCell.Style.Font.Bold = true;
                            nplCell.Style.Font.Color.SetColor(Color.Red);
                            nplCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            nplCell.Style.Fill.BackgroundColor.SetColor(Color.White);
                            nplCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            nplCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            nplCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            nplCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            rowTH++;

                            var groupByNhom = nplGroup.GroupBy(r => r["TenNhom"].ToString());
                            int dataRowIndex = 0;
                            foreach (var nhomGroup in groupByNhom)
                            {
                                var nhomCell = newSheetTH.Cells[rowTH, 1, rowTH, 12];
                                nhomCell.Merge = true;
                                nhomCell.Value = nhomGroup.Key;
                                nhomCell.Style.Font.Bold = true;
                                nhomCell.Style.Font.Color.SetColor(Color.Green);
                                nhomCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                nhomCell.Style.Fill.BackgroundColor.SetColor(Color.White);
                                nhomCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                nhomCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                nhomCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                nhomCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                rowTH++;


                                decimal tongCapPhat = 0, tongThucNhap = 0, tongSLXuat = 0, tongSLChuaNhan = 0, tongthua = 0, tongthieu = 0;
                                foreach (var items in nhomGroup)
                                {
                                    newSheetTH.Row(rowTH).Height = 40;
                                    newSheetTH.Cells[rowTH, 1].Value = items["MaVTGhep"];
                                    newSheetTH.Cells[rowTH, 1].Style.WrapText = true;
                                    newSheetTH.Cells[rowTH, 2].Value = items["MaVatTu"];
                                    newSheetTH.Cells[rowTH, 3].Value = items["ChiTiet"];
                                    newSheetTH.Cells[rowTH, 3].Style.WrapText = true;
                                    newSheetTH.Cells[rowTH, 4].Value = items["TenDV"];
                                    newSheetTH.Cells[rowTH, 5].Value = items["KhoVai"];
                                    newSheetTH.Cells[rowTH, 6].Value = items["MauVTCode"];
                                    newSheetTH.Cells[rowTH, 7].Value = items["MauVT"];
                                    newSheetTH.Cells[rowTH, 7].Style.WrapText = true;
                                    newSheetTH.Cells[rowTH, 8].Value = items["CapPhat"];
                                    newSheetTH.Cells[rowTH, 9].Value = items["ThucNhap"];
                                    newSheetTH.Cells[rowTH, 10].Value = items["SLXuat"];
                                    double capPhat = 0;
                                    double thucNhap = 0;
                                    double slthua = 0;
                                    double slthieu = 0;
                                    double thuaValue = 0;
                                    double thieuValue = 0;
                                    double.TryParse(items["CapPhat"]?.ToString(), out capPhat);
                                    double.TryParse(items["ThucNhap"]?.ToString(), out thucNhap);
                                    double.TryParse(items["SLThua"]?.ToString(), out slthua);
                                    double.TryParse(items["SLThieu"]?.ToString(), out slthieu);

                                    if (capPhat < thucNhap)
                                    {
                                        thuaValue = slthua;
                                        thieuValue = 0;
                                        newSheetTH.Cells[rowTH, 11].Value = thuaValue;
                                        newSheetTH.Cells[rowTH, 12].Value = "-";
                                        newSheetTH.Cells[rowTH, 11].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#2A5D9F"));
                                    }
                                    else if (capPhat > thucNhap)
                                    {
                                        thuaValue = 0;
                                        thieuValue = slthieu;
                                        newSheetTH.Cells[rowTH, 11].Value = "-";
                                        newSheetTH.Cells[rowTH, 12].Value = slthieu;
                                        newSheetTH.Cells[rowTH, 12].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#A53E25"));
                                    }
                                    else if (capPhat == thucNhap)
                                    {
                                        thuaValue = 0;
                                        thieuValue = 0;
                                        newSheetTH.Cells[rowTH, 11].Value = "-";
                                        newSheetTH.Cells[rowTH, 12].Value = "-";
                                    }
                                    newSheetTH.Cells[rowTH, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    newSheetTH.Cells[rowTH, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    var range = newSheetTH.Cells[rowTH, 1, rowTH, 12];
                                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                    if (dataRowIndex % 2 == 0)
                                    {
                                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        range.Style.Fill.BackgroundColor.SetColor(Color.Azure);
                                    }
                                    newSheetTH.Cells[rowTH, 8].Style.Numberformat.Format = "#,##0.00";
                                    newSheetTH.Cells[rowTH, 9].Style.Numberformat.Format = "#,##0.00";
                                    newSheetTH.Cells[rowTH, 10].Style.Numberformat.Format = "#,##0.00";
                                    newSheetTH.Cells[rowTH, 11].Style.Numberformat.Format = "#,##0.00";
                                    newSheetTH.Cells[rowTH, 12].Style.Numberformat.Format = "#,##0.00";
                                    tongCapPhat += Convert.ToDecimal(items["CapPhat"]);
                                    tongThucNhap += Convert.ToDecimal(items["ThucNhap"]);
                                    tongSLXuat += Convert.ToDecimal(items["SLXuat"]);
                                    tongthua += (decimal)thuaValue;
                                    tongthieu += (decimal)thieuValue;
                                    dataRowIndex++;
                                    rowTH++;
                                }
                                newSheetTH.Cells[rowTH, 1].Value = "";
                                newSheetTH.Cells[rowTH, 1, rowTH, 7].Merge = true;
                                newSheetTH.Cells[rowTH, 1].Style.Font.Bold = true;
                                newSheetTH.Cells[rowTH, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                newSheetTH.Cells[rowTH, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                newSheetTH.Cells[rowTH, 8].Value = tongCapPhat;
                                newSheetTH.Cells[rowTH, 9].Value = tongThucNhap;
                                newSheetTH.Cells[rowTH, 10].Value = tongSLXuat;
                                newSheetTH.Cells[rowTH, 11].Value = tongthua;
                                newSheetTH.Cells[rowTH, 12].Value = tongthieu;

                                newSheetTH.Cells[rowTH, 8].Style.Numberformat.Format = "#,##0.00";
                                newSheetTH.Cells[rowTH, 9].Style.Numberformat.Format = "#,##0.00";
                                newSheetTH.Cells[rowTH, 10].Style.Numberformat.Format = "#,##0.00";
                                newSheetTH.Cells[rowTH, 11].Style.Numberformat.Format = "#,##0.00";
                                newSheetTH.Cells[rowTH, 12].Style.Numberformat.Format = "#,##0.00";

                                var sumRange = newSheetTH.Cells[rowTH, 1, rowTH, 12];
                                sumRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                sumRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                sumRange.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#2E7D5B"));
                                sumRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D7F5E8"));
                                sumRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                sumRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                sumRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                sumRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                sumRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                sumRange.Style.Font.Bold = true;
                                rowTH++;
                            }
                        }
                    }
                }
                excelPackage.SaveAs(file);

            }
        }

        public void ExportDH(string TemplateFileName, string ExportFileName)
        {
            try
            {
                string url = string.Format("{0}?action={1}&&para1={2}&&para2={3}&&para3={4}&&para4={5}", URL + "TongQuanCapPhat/Get", "GetPOChitiet_Export", ItemSearchLookup_KhachHang.EditValue.ToString(), mahangselected, poselected, isall);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0) return;

                string[] mahangarr = tbl.AsEnumerable()
                       .Select(r => r["MaHang"].ToString())
                       .Distinct()?.ToArray();

                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";
                    FileInfo templateFile = new FileInfo(TemplateFileName);
                    int sheetIndex = 0;

                    using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                    {
                        sheetIndex++;
                        foreach (string mahangtonghop in mahangarr)
                        {
                            int index = 1;
                            int rowTH = 10;
                            int rowHeader = 9;
                            string sheetNameTongHop = $"{mahangtonghop}";
                            ExcelWorksheet templateSheetTH = templatePackage.Workbook.Worksheets["Sheet1"];
                            ExcelWorksheet newSheetTH = excelPackage.Workbook.Worksheets.Add(sheetNameTongHop, templateSheetTH);
                            string selectedTenHang = tbl.AsEnumerable()
                            .Where(r => r["MaHang"].ToString() == mahangtonghop.ToString())
                            .Select(r => r["TenHang"].ToString())
                            .FirstOrDefault();
                            string selectedTenKH = tbl.AsEnumerable()
                              .Where(r => r["MaKH"].ToString() == ItemSearchLookup_KhachHang.EditValue.ToString())
                              .Select(r => r["TenKH"].ToString())
                              .FirstOrDefault();

                            newSheetTH.Cells[6, 1].Value = "Khách Hàng:";
                            newSheetTH.Cells[6, 1].Style.Font.Bold = true;
                            newSheetTH.Cells[6, 2].Value = selectedTenKH.ToString();
                            newSheetTH.Cells[6, 3].Value = "Mã Hàng:";
                            newSheetTH.Cells[6, 3].Style.Font.Bold = true;
                            newSheetTH.Cells[6, 4].Value = selectedTenHang.ToString();
                            var groupByPO = tbl.AsEnumerable()
                                                .Where(r => r["MaHang"].ToString() == mahangtonghop.ToString())
                                                .GroupBy(r => r["PO"].ToString());

                            var sizeList = tbl.AsEnumerable()
                                .Where(r => mahangtonghop.Contains(r["MaHang"].ToString()))
                                .Select(r => new
                                {
                                    Size = r["Size"].ToString(),
                                    Sort = Convert.ToInt32(r["Sort"])
                                })
                                .GroupBy(x => x.Size)
                                .Select(g => g.First())
                                .OrderBy(x => x.Sort)
                                .Select(x => x.Size)
                                .ToList();

                            newSheetTH.Cells[rowHeader, 1, rowTH, 1].Merge = true;
                            newSheetTH.Cells[rowHeader, 1].Value = "Đơn Hàng";
                            newSheetTH.Cells[rowHeader, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 1].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            newSheetTH.Cells[rowHeader, 2, rowTH, 2].Merge = true;
                            newSheetTH.Cells[rowHeader, 2].Value = "Số Booking";
                            newSheetTH.Cells[rowHeader, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 2].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            newSheetTH.Cells[rowHeader, 3, rowTH, 3].Merge = true;
                            newSheetTH.Cells[rowHeader, 3].Value = "Code Màu";
                            newSheetTH.Cells[rowHeader, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 3].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            newSheetTH.Cells[rowHeader, 4, rowTH, 4].Merge = true;
                            newSheetTH.Cells[rowHeader, 4].Value = "InSeam";
                            newSheetTH.Cells[rowHeader, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 4].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            newSheetTH.Cells[rowHeader, 5, rowTH, 5].Merge = true;
                            newSheetTH.Cells[rowHeader, 5].Value = "Quốc Gia";
                            newSheetTH.Cells[rowHeader, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 5].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                            newSheetTH.Cells[rowHeader, 6, rowTH, 6].Merge = true;
                            newSheetTH.Cells[rowHeader, 6].Value = "Ngày Giao";
                            newSheetTH.Cells[rowHeader, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, 6].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            newSheetTH.Column(6).Width = 15;

                            index = 7;
                            int sizeStartCol = index;
                            int sizeEndCol = index + sizeList.Count - 1;
                            newSheetTH.Cells[rowHeader, sizeStartCol, rowHeader, sizeEndCol].Merge = true;
                            newSheetTH.Cells[rowHeader, sizeStartCol].Value = "Size";
                            newSheetTH.Cells[rowHeader, sizeStartCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, sizeStartCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, sizeStartCol].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, sizeStartCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, sizeStartCol].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                            foreach (var size in sizeList)
                            {
                                newSheetTH.Cells[rowTH, index].Value = size;
                                newSheetTH.Cells[rowTH, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                newSheetTH.Cells[rowTH, index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                newSheetTH.Cells[rowTH, index].Style.Font.Bold = true;
                                newSheetTH.Cells[rowTH, index].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                newSheetTH.Cells[rowTH, index].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                                index++;
                            }

                            newSheetTH.Cells[rowHeader, index, rowTH, index].Merge = true;
                            newSheetTH.Cells[rowHeader, index].Value = "Tổng";
                            newSheetTH.Cells[rowHeader, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[rowHeader, index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[rowHeader, index].Style.Font.Bold = true;
                            newSheetTH.Cells[rowHeader, index].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newSheetTH.Cells[rowHeader, index].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);



                            int dataRow = rowTH + 1;
                            var maHangSizeTotal = new Dictionary<string, int>();

                            foreach (var group in groupByPO)
                            {
                                int sumSoLuongPO = group.Sum(r => Convert.ToInt32(r["SoLuong"]));

                                var nhomCell = newSheetTH.Cells[dataRow, 1, dataRow, index];
                                nhomCell.Merge = true;
                                nhomCell.Value = $"PO:{group.Key} - Số Lượng:{sumSoLuongPO}";
                                nhomCell.Style.Font.Bold = true;
                                nhomCell.Style.Font.Color.SetColor(Color.Red);
                                nhomCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                nhomCell.Style.Fill.BackgroundColor.SetColor(Color.White);
                                nhomCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                nhomCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                nhomCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                nhomCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                dataRow++;

                                var groupbyMau = group.GroupBy(r => r["TenMau"].ToString());
                                foreach (var grmau in groupbyMau)
                                {
                                    var mauCell = newSheetTH.Cells[dataRow, 1, dataRow, index];
                                    mauCell.Merge = true;
                                    mauCell.Value = grmau.Key;
                                    mauCell.Style.Font.Bold = true;
                                    mauCell.Style.Font.Color.SetColor(Color.Green);
                                    mauCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    mauCell.Style.Fill.BackgroundColor.SetColor(Color.White);
                                    mauCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    mauCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    mauCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    mauCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    dataRow++;

                                    var grouped = grmau
                                        .GroupBy(r => new
                                        {
                                            POID = r["POID"].ToString(),
                                            PO = r["PO"].ToString(),
                                            Mau = r["TenMau"].ToString(),
                                            MaMau = r["MaMau"].ToString(),
                                            DausizeID = r["DauSizeID"].ToString(),
                                            Dausize = r["DauSize"].ToString(),
                                            TenQG = r["TenQG"].ToString(),
                                            NgayGH = r["NgayGH"].ToString(),
                                            ColorCode = r["CodeMau"].ToString(),
                                            Booking = r["BookingMaHang"].ToString(),
                                            MaDH = r["MaDH"].ToString()
                                        });
                                    int rowIndex = 0;
                                    foreach (var item in grouped)
                                    {
                                        int c = 1;
                                        Color backgroundColor = rowIndex % 2 == 0 ? Color.White : Color.Azure;
                                        newSheetTH.Cells[dataRow, c].Value = item.Key.MaDH.ToString();
                                        newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                        c++;

                                        newSheetTH.Cells[dataRow, c].Value = item.Key.Booking.ToString();
                                        newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                        c++;

                                        newSheetTH.Cells[dataRow, c].Value = item.Key.ColorCode.ToString();
                                        newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                        c++;

                                        newSheetTH.Cells[dataRow, c].Value = item.Key.Dausize.ToString();
                                        newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                        c++;

                                        newSheetTH.Cells[dataRow, c].Value = item.Key.TenQG.ToString();
                                        newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                        c++;

                                        if (DateTime.TryParse(item.Key.NgayGH, out DateTime ngayGH))
                                        {
                                            newSheetTH.Cells[dataRow, c].Value = ngayGH;
                                            newSheetTH.Cells[dataRow, c].Style.Numberformat.Format = "dd/MM/yyyy";
                                        }
                                        else
                                        {
                                            newSheetTH.Cells[dataRow, c].Value = (item.Key.NgayGH);
                                        }
                                        newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                        c++;

                                        var slBySize = item
                                            .GroupBy(x => x["Size"].ToString())
                                            .ToDictionary(g => g.Key, g => g.Sum(x => Convert.ToInt32(x["SoLuong"])));

                                        int total = 0;
                                        foreach (var size in sizeList)
                                        {
                                            int sl = slBySize.ContainsKey(size) ? slBySize[size] : 0;
                                            if (sl == 0)
                                            {
                                                newSheetTH.Cells[dataRow, c].Value = "-";
                                                newSheetTH.Cells[dataRow, c].Style.Numberformat.Format = "@"; // Định dạng text
                                            }
                                            else
                                            {
                                                newSheetTH.Cells[dataRow, c].Value = sl;
                                            }
                                            newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                            newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                            newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                            total += sl;
                                            c++;
                                        }

                                        newSheetTH.Cells[dataRow, c].Value = total;
                                        newSheetTH.Cells[dataRow, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, c].Style.Fill.BackgroundColor.SetColor(backgroundColor);
                                        dataRow++;
                                        rowIndex++;
                                    }

                                    int cStart = 1;
                                    int cFixed = 6;
                                    int cSizeStart = cStart + cFixed;
                                    newSheetTH.Cells[dataRow, cStart].Value = "";
                                    newSheetTH.Cells[dataRow, cStart, dataRow, cSizeStart - 1].Merge = true;
                                    newSheetTH.Cells[dataRow, cStart].Style.Font.Bold = true;
                                    newSheetTH.Cells[dataRow, cStart].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#2E7D5B"));
                                    newSheetTH.Cells[dataRow, cStart].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    newSheetTH.Cells[dataRow, cStart].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D7F5E8"));
                                    newSheetTH.Cells[dataRow, cStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    newSheetTH.Cells[dataRow, cStart].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    var allSizeTotal = grouped
                                                        .SelectMany(x => x)
                                                        .GroupBy(x => x["Size"].ToString())
                                                        .ToDictionary(g => g.Key, g => g.Sum(x => Convert.ToInt32(x["SoLuong"])));
                                    int totalAll = 0;
                                    foreach (var size in sizeList)
                                    {
                                        int sl = allSizeTotal.ContainsKey(size) ? allSizeTotal[size] : 0;
                                        object cellValue;
                                        if (sl > 0)
                                            cellValue = sl;
                                        else
                                            cellValue = "-";

                                        newSheetTH.Cells[dataRow, cSizeStart].Value = cellValue;
                                        newSheetTH.Cells[dataRow, cSizeStart].Style.Font.Bold = true;
                                        newSheetTH.Cells[dataRow, cSizeStart].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#2E7D5B"));
                                        newSheetTH.Cells[dataRow, cSizeStart].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        newSheetTH.Cells[dataRow, cSizeStart].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D7F5E8"));
                                        newSheetTH.Cells[dataRow, cSizeStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        newSheetTH.Cells[dataRow, cSizeStart].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        totalAll += sl;
                                        if (maHangSizeTotal.ContainsKey(size))
                                            maHangSizeTotal[size] += sl;
                                        else
                                            maHangSizeTotal[size] = sl;
                                        cSizeStart++;
                                    }

                                    newSheetTH.Cells[dataRow, cSizeStart].Value = totalAll;
                                    newSheetTH.Cells[dataRow, cSizeStart].Style.Font.Bold = true;
                                    newSheetTH.Cells[dataRow, cSizeStart].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#2E7D5B"));
                                    newSheetTH.Cells[dataRow, cSizeStart].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    newSheetTH.Cells[dataRow, cSizeStart].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D7F5E8"));
                                    newSheetTH.Cells[dataRow, cSizeStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    newSheetTH.Cells[dataRow, cSizeStart].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    dataRow++;
                                }

                            }

                            int cStartMH = 1;
                            int cFixedMH = 6;
                            int cSizeStartMH = cStartMH + cFixedMH;

                            newSheetTH.Cells[dataRow, cStartMH].Value = "";
                            newSheetTH.Cells[dataRow, cStartMH, dataRow, cSizeStartMH - 1].Merge = true;
                            newSheetTH.Cells[dataRow, cStartMH].Style.Font.Bold = true;
                            newSheetTH.Cells[dataRow, cStartMH].Style.Font.Color.SetColor(Color.Red);
                            newSheetTH.Cells[dataRow, cStartMH].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            newSheetTH.Cells[dataRow, cStartMH].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 239, 204));
                            newSheetTH.Cells[dataRow, cStartMH].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[dataRow, cStartMH].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[dataRow, cStartMH].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            newSheetTH.Cells[dataRow, cStartMH].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            newSheetTH.Cells[dataRow, cStartMH].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            newSheetTH.Cells[dataRow, cStartMH].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            // Hiển thị tổng theo từng size
                            int totalMaHang = 0;
                            foreach (var size in sizeList)
                            {
                                int sl = maHangSizeTotal.ContainsKey(size) ? maHangSizeTotal[size] : 0;
                                object cellValue;
                                if (sl > 0)
                                    cellValue = sl;
                                else
                                    cellValue = "-";

                                newSheetTH.Cells[dataRow, cSizeStartMH].Value = cellValue;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Font.Bold = true;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Font.Color.SetColor(Color.Red);
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 239, 204));
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                totalMaHang += sl;
                                cSizeStartMH++;
                            }

                            newSheetTH.Cells[dataRow, cSizeStartMH].Value = totalMaHang;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Font.Bold = true;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Font.Color.SetColor(Color.Red);
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 239, 204));
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            newSheetTH.Cells[dataRow, cSizeStartMH].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            dataRow++;

                            var tableRange = newSheetTH.Cells[rowHeader, 1, dataRow - 1, index];
                            tableRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            tableRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            tableRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            tableRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    excelPackage.SaveAs(file);

                }
            }
            catch (Exception ez)
            {

            }

        }
        private void rItemCheckBox_ALL_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit Check_ALL_MaHang = sender as CheckEdit;
            if (Check_ALL_MaHang == null) return;
            if (TabSelected.SelectedTabPage == tabDonHang)
            {
                string MaKH = ItemSearchLookup_KhachHang.EditValue?.ToString();
                string MaHang = ItemSearchLookup_MaHang.EditValue?.ToString();
                if (!string.IsNullOrEmpty(MaKH) && string.IsNullOrEmpty(MaHang))
                {
                    LoadDonHang(MaKH, MaHang ?? "NONE");
                }
            }




        }


        private void loadcombobox()
        {
            repositoryItemComboBox2.Items.Add("Khoảng Ngày");
            repositoryItemComboBox2.Items.Add("Tất Cả");
            repositoryItemComboBox2.Items.Add("Tháng Hiện Tại");

            barEditItem3.EditValue = "Tháng Hiện Tại";
            barEditItem1.Visibility = BarItemVisibility.Never;
            barEditItem2.Visibility = BarItemVisibility.Never;
            barEditItem3.Visibility = BarItemVisibility.Never;
        }

        private void barEditItem3_EditValueChanged(object sender, EventArgs e)
        {
            string selected = barEditItem3.EditValue?.ToString();

            if (selected.ToString() == "Khoảng Ngày")
            {
                barEditItem1.Visibility = BarItemVisibility.Always;
                barEditItem2.Visibility = BarItemVisibility.Always;
            }
            else if (selected.ToString() == "Tất Cả")
            {
                barEditItem1.Visibility = BarItemVisibility.Never;
                barEditItem2.Visibility = BarItemVisibility.Never;
                if (_selectedMaKH != null && string.IsNullOrEmpty(_selectedMaHang))
                {
                    LoadDonHang(_selectedMaKH, "");
                }
                else
                {
                    LoadDonHang(_selectedMaKH, _selectedMaHang);
                }

            }
            else if (selected.ToString() == "Tháng Hiện Tại")
            {
                barEditItem1.Visibility = BarItemVisibility.Always;
                barEditItem2.Visibility = BarItemVisibility.Always;
                loaddate();
            }
        }



        private void loaddate()
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            barEditItem1.EditValue = firstDayOfMonth;
            barEditItem2.EditValue = lastDayOfMonth;

            repositoryItemDateEdit1.Mask.EditMask = "dd/MM/yyyy";
            repositoryItemDateEdit1.Mask.UseMaskAsDisplayFormat = true;
            repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yyyy";
            repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

            repositoryItemDateEdit2.Mask.EditMask = "dd/MM/yyyy";
            repositoryItemDateEdit2.Mask.UseMaskAsDisplayFormat = true;
            repositoryItemDateEdit2.DisplayFormat.FormatString = "dd/MM/yyyy";
            repositoryItemDateEdit2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

            barEditItem1.Edit = repositoryItemDateEdit1;
            barEditItem2.Edit = repositoryItemDateEdit2;
        }

        private void barEditItem2_EditValueChanged(object sender, EventArgs e)
        {
            string fromdate = barEditItem1.EditValue?.ToString();
            string todate = barEditItem2.EditValue?.ToString();
            if (Convert.ToDateTime(fromdate.ToString()) > Convert.ToDateTime(todate.ToString()))
            {
                XtraMessageBox.Show($"không thể chọn ngày nhỏ hơn{fromdate}.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_selectedMaKH != null && string.IsNullOrEmpty(_selectedMaHang))
            {
                LoadDonHang(_selectedMaKH, "", fromdate, todate);
            }
            else
            {
                LoadDonHang(_selectedMaKH, _selectedMaHang, fromdate, todate);
            }
        }

        private void repositoryItemComboBox2_EditValueChanged(object sender, EventArgs e)
        {

        }


        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            string fromdate = barEditItem1.EditValue?.ToString();
            string todate = barEditItem2.EditValue?.ToString();
            if (string.IsNullOrEmpty(fromdate) || string.IsNullOrEmpty(todate))
            {
                return;
            }
            if (Convert.ToDateTime(fromdate.ToString()) > Convert.ToDateTime(todate.ToString()))
            {
                XtraMessageBox.Show($"không thể chọn ngày lớn hơn ngày {todate}.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_selectedMaKH != null && string.IsNullOrEmpty(_selectedMaHang))
            {
                LoadDonHang(_selectedMaKH, "", fromdate, todate);
            }
            else
            {
                LoadDonHang(_selectedMaKH, _selectedMaHang, fromdate, todate);
            }
        }
       

    }


}