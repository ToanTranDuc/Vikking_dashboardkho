using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
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
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmERPPhieuBaoGiaIndex : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<string> lstFormatFieldName = new List<string> { "DonGiaSuaThueCK", "DonGia", "ChiPhiPhatSinh" };

        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        int pageIndex = 1;
        int pageSize = 50;
        public frmERPPhieuBaoGiaIndex()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
        }
     
        protected override void OnLoad(EventArgs e)
        {
            barStaticItemPageIndex.EditValue = "1";
            LoadPhieuBaoGia();


            grvPhieuBaoGiaTQ.ColumnPanelRowHeight = 50;
         


          
            grvPhieuBaoGiaTQ.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            

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
                //barSubItem1.Enabled = false;
                Them.Enabled = false;
                //barButtonItem1.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
            }
            if (!_allowAdd && !_allowEdit)
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
            {
                Xoa.Enabled = false;
                //barButtonItem10.Enabled = false;
            }

        }
        private DataTable CreateTableSavePhieuBG()
        {
            DataTable dt = new DataTable("PhieuBaoGia");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("TenPhieu", typeof(string));
            dt.Columns.Add("NhomCLCC", typeof(string));
            dt.Columns.Add("MaNCC", typeof(string));
            dt.Columns.Add("IsDuyet", typeof(bool));
            dt.Columns.Add("NgayDuyet", typeof(string));
            dt.Columns.Add("NgayBDHieuLuc", typeof(string));
            dt.Columns.Add("SoNgayHieuLuc", typeof(int));
            dt.Columns.Add("SoNgayGHSom", typeof(int));
            dt.Columns.Add("SoNgayGHTre", typeof(int));
            dt.Columns.Add("NgayTao", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NgaySua", typeof(string));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("MaDVTTe", typeof(string));
            dt.Columns.Add("HinhThucThanhToan", typeof(string));
            dt.Columns.Add("PTThanhToan", typeof(string));
            dt.Columns.Add("PTVanChuyen", typeof(string));
            dt.Columns.Add("DonViTG_HieuLuc", typeof(string));
            dt.Columns.Add("Action", typeof(string));
            return dt;
        }
        private void LoadPhieuBaoGia()
        {
            try
            {
                string url = string.Empty;
                DataTable tbl = new DataTable();
                if (xtabPhieuBaoGia.SelectedTabPage == xtabNguyenPhuLieu)
                {
                    url = $"{URL}PhieuBaoGia/GET?action=GetTQBaoGia&para1={pageIndex}&para2={pageSize}&para3={txtSearchNhaCC.EditValue}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json != "[]")
                    {
                        tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    }


                    grcPhieuBaoGiaTQ.DataSource = tbl;
                    grvPhieuBaoGiaTQ.ExpandAllGroups();
                }
                else if (xtabPhieuBaoGia.SelectedTabPage == xtabMayMocThietBi)
                {
                    url = $"{URL}PhieuBaoGiaMMTB/GET?action=GetTQBaoGia&para1={pageIndex}&para2={pageSize}&para3={txtSearchNhaCC.EditValue}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json != "[]")
                    {
                        tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    }


                    gridControl1.DataSource = tbl;
                    gridView1.ExpandAllGroups();
                }
               
               
             
                
                
                            
            }
            catch (Exception ex)
            {

            }
        }
        private void grvPhieuBaoGiaTQ_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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

        private void grvPhieuBaoGiaTQ_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle == grvPhieuBaoGiaTQ.FocusedRowHandle)
            {
                // Màu xanh dương nhạt - chuyên nghiệp cho bảng cha
                e.Appearance.BackColor = Color.FromArgb(220, 237, 252); // #DCEDFC
                e.HighPriority = true;
            }
        }

        // Custom draw header với chiều cao tăng thêm
        private void grv_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        
        private void grvPhieuBaoGiaTQ_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (lstFormatFieldName.Contains(e.Column.FieldName))
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
                    decimals = decimals == 0 ? 0 : Math.Min(decimals, 2);

                    string format = decimals == 0 ? "N0" : $"N{decimals}";

                    e.DisplayText = value.ToString(format, nfi);
                    return;
                }else if(e.Column == colNgayBDHieuLuc || e.Column == colNgayKTHieuLuc)
                {
                    if (e.Value == null || e.Value == DBNull.Value || string.IsNullOrWhiteSpace(e.Value?.ToString()))
                    {
                        e.DisplayText = "";
                        return;
                    }
                    //DateTime dt = clsForrmatUtils.ConvertDate(e.Value.ToString());
                    //e.DisplayText = dt == DateTime.MinValue ? "" : dt.ToString("dd-MM-yyyy");
                }
                

            }
            catch (Exception ex)
            {

            }
        }
     
        private void bttRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadPhieuBaoGia();
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            if(xtabPhieuBaoGia.SelectedTabPage == xtabNguyenPhuLieu)
            {
                frmERPBaoGiaNCC frm = new frmERPBaoGiaNCC("add", false);
                frm.Show();
                frm.FormClosed += Frm_FormClosed;
            }
            else if(xtabPhieuBaoGia.SelectedTabPage == xtabMayMocThietBi)
            {
                frmERPBaoGiaNCC_MayMocTB frm = new frmERPBaoGiaNCC_MayMocTB("add", false);
                frm.Show();
                frm.FormClosed += Frm_FormClosed;
            }


                   
        }

        private void Frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadPhieuBaoGia();
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
    
            if (xtabPhieuBaoGia.SelectedTabPage == xtabNguyenPhuLieu)
            {
                int rowHandle = grvPhieuBaoGiaTQ.FocusedRowHandle;


                if (grvPhieuBaoGiaTQ.IsGroupRow(rowHandle))
                {
                    rowHandle = grvPhieuBaoGiaTQ.GetDataRowHandleByGroupRowHandle(rowHandle);
                }

                if (rowHandle < 0)
                {

                    return;
                }

                DataRow rowFocused = grvPhieuBaoGiaTQ.GetDataRow(rowHandle);
                if (rowFocused == null)
                {
                    return;
                }

                string MaPhieuBG = rowFocused["MaPhieuBG"]?.ToString();
                string TenPhieuBG = rowFocused["TenPhieu"]?.ToString();

                if (string.IsNullOrEmpty(MaPhieuBG))
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để sửa",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                frmERPBaoGiaNCC frm = new frmERPBaoGiaNCC("edit", IsDuyet, MaPhieuBG, TenPhieuBG);

                frm.Show();
                frm.MaximizeBox = true;

                frm.FormClosed += Frm_FormClosed;
            }
            else if (xtabPhieuBaoGia.SelectedTabPage == xtabMayMocThietBi)
            {
                int rowHandle = gridView1.FocusedRowHandle;


                if (gridView1.IsGroupRow(rowHandle))
                {
                    rowHandle = gridView1.GetDataRowHandleByGroupRowHandle(rowHandle);
                }

                if (rowHandle < 0)
                {

                    return;
                }

                DataRow rowFocused = gridView1.GetDataRow(rowHandle);
                if (rowFocused == null)
                {
                    return;
                }

                string MaPhieuBG = rowFocused["MaPhieuBG"]?.ToString();
                string TenPhieuBG = rowFocused["TenPhieu"]?.ToString();

                if (string.IsNullOrEmpty(MaPhieuBG))
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để sửa",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                frmERPBaoGiaNCC_MayMocTB frm = new frmERPBaoGiaNCC_MayMocTB("edit", IsDuyet, MaPhieuBG, TenPhieuBG);

                frm.Show();
                frm.MaximizeBox = true;

                frm.FormClosed += Frm_FormClosed;
            }

  
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow rowFocused = null;
                if (xtabPhieuBaoGia.SelectedTabPage == xtabNguyenPhuLieu)
                {
                    int rowHandle = grvPhieuBaoGiaTQ.FocusedRowHandle;


                    if (grvPhieuBaoGiaTQ.IsGroupRow(rowHandle))
                    {
                        rowHandle = grvPhieuBaoGiaTQ.GetDataRowHandleByGroupRowHandle(rowHandle);
                    }


                    if (rowHandle < 0)
                    {

                        return;
                    }

                     rowFocused = grvPhieuBaoGiaTQ.GetDataRow(rowHandle);
                   
                }
                else if (xtabPhieuBaoGia.SelectedTabPage == xtabMayMocThietBi)
                {
                    int rowHandle = gridView1.FocusedRowHandle;


                    if (gridView1.IsGroupRow(rowHandle))
                    {
                        rowHandle = gridView1.GetDataRowHandleByGroupRowHandle(rowHandle);
                    }


                    if (rowHandle < 0)
                    {

                        return;
                    }

                    rowFocused = gridView1.GetDataRow(rowHandle);
                }
                if (rowFocused == null)
                {

                    return;
                }

                string MaPhieuBG = rowFocused["MaPhieuBG"]?.ToString();
                string TenPhieu = rowFocused["TenPhieu"]?.ToString();

                if (string.IsNullOrEmpty(MaPhieuBG))
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu để xóa.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataTable tbl = new DataTable();
                string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={MaPhieuBG}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tbl?.Rows?.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                        {
                            XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                }

                bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                if (IsDuyet)
                {
                    XtraMessageBox.Show($"Phiếu {TenPhieu} đã duyệt nên không thể xóa", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn xóa {TenPhieu} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                string url = $"{URL}PhieuBaoGia/Delete?action=DeletePhieu&Para1={Uri.EscapeDataString(MaPhieuBG)}";
                string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

                if (result.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    LoadPhieuBaoGia();
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        private void grvPhieuBaoGiaTQ_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {

                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;

                if (view.FocusedColumn.FieldName == "IsDuyet")
                {
                    DataRow rowFocused = null;
                    if (xtabPhieuBaoGia.SelectedTabPage == xtabNguyenPhuLieu)
                    {
                        int rowHandle = grvPhieuBaoGiaTQ.FocusedRowHandle;


                        if (grvPhieuBaoGiaTQ.IsGroupRow(rowHandle))
                        {
                            rowHandle = grvPhieuBaoGiaTQ.GetDataRowHandleByGroupRowHandle(rowHandle);
                        }


                        if (rowHandle < 0)
                        {

                            return;
                        }

                        rowFocused = grvPhieuBaoGiaTQ.GetDataRow(rowHandle);

                    }
                    else if (xtabPhieuBaoGia.SelectedTabPage == xtabMayMocThietBi)
                    {
                        int rowHandle = gridView1.FocusedRowHandle;


                        if (gridView1.IsGroupRow(rowHandle))
                        {
                            rowHandle = gridView1.GetDataRowHandleByGroupRowHandle(rowHandle);
                        }


                        if (rowHandle < 0)
                        {

                            return;
                        }

                        rowFocused = gridView1.GetDataRow(rowHandle);
                    }
                    if (rowFocused == null)
                    {

                        return;
                    }

                    bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.02.00" &&
                            r["AllowDuyet"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowDuyet"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show($"Bạn không có quyền {(!IsDuyet ? "Duyệt" : "Hủy Duyệt")}!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                   
                    DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn {(!IsDuyet ? "Duyệt" : "Hủy Duyệt")} Phiếu {rowFocused["TenPhieu"]?.ToString()} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                    if (messResult != DialogResult.Yes) return;
                    string url = $"{URL}PhieuBaoGia/POST?action=XetDuyet";

                    string maPhieuBG = rowFocused["MaPhieuBG"]?.ToString();
                    if (string.IsNullOrEmpty(maPhieuBG))
                    {
                        XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    DataTable tbl = new DataTable();
                    string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={maPhieuBG}&para2=XetDuyet";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                    if (json != "[]")
                    {
                        tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        if (tbl?.Rows?.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                            {
                                XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                    }                
                                   
                    DataTable tblPhieu = CreateTableSavePhieuBG();
                    DataRow rowPhieu = tblPhieu.NewRow();
                    rowPhieu["ID"] = 0;
                    rowPhieu["Action"] = "EditPhieu";
                    rowPhieu["MaPhieuBG"] = !string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null;
                    rowPhieu["TenPhieu"] = rowFocused["TenPhieu"]?.ToString();
                    rowPhieu["NhomCLCC"] =  "";
                    rowPhieu["MaNCC"] =  "";
                    rowPhieu["IsDuyet"] = !IsDuyet;
                    rowPhieu["NgayDuyet"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                
                    rowPhieu["SoNgayHieuLuc"] = 0;
                    tblPhieu.Rows.Add(rowPhieu);

                    string jsonData = JsonConvert.SerializeObject(tblPhieu);
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                    if (result.Trim().ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        LoadPhieuBaoGia();
                        string Title = $"Quotation sheet {(!IsDuyet ? "đã duyệt" : "đã hủy duyệt")}";
                        string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}";
                        SendNotify(Title, Detail,"PKH");
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu:\n" + result, "Lỗi lưu dữ liệu",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {

            }
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

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }
    
        private void xtabPhieuBaoGia_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            DevExpress.XtraTab.TabPageChangedEventArgs tabSelected = e as DevExpress.XtraTab.TabPageChangedEventArgs;
            if(tabSelected != null)
            {
                LoadPhieuBaoGia();
            }
        }

        private void grvPhieuBaoGiaTQ_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {

        }

        

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmLichSuPhieuBG frm = new frmLichSuPhieuBG();
            frm.Show();
        }

        private void btnCopy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtabPhieuBaoGia.SelectedTabPage == xtabNguyenPhuLieu)
                {
                    int rowHandle = grvPhieuBaoGiaTQ.FocusedRowHandle;


                    if (grvPhieuBaoGiaTQ.IsGroupRow(rowHandle))
                    {
                        rowHandle = grvPhieuBaoGiaTQ.GetDataRowHandleByGroupRowHandle(rowHandle);
                    }

                    if (rowHandle < 0)
                    {

                        return;
                    }

                    DataRow rowFocused = grvPhieuBaoGiaTQ.GetDataRow(rowHandle);
                    if (rowFocused == null)
                    {
                        return;
                    }

                    string MaPhieuBG = rowFocused["MaPhieuBG"]?.ToString();
                    string TenPhieuBG = rowFocused["TenPhieu"]?.ToString();

                    if (string.IsNullOrEmpty(MaPhieuBG))
                    {
                        XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để sửa",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                   
                    frmERPBaoGiaNCC frm = new frmERPBaoGiaNCC("copy", false, "", TenPhieuBG, MaPhieuBG);

                    frm.Show();
                    frm.MaximizeBox = true;

                    frm.FormClosed += Frm_FormClosed;
                }
                else if (xtabPhieuBaoGia.SelectedTabPage == xtabMayMocThietBi)
                {
                    int rowHandle = gridView1.FocusedRowHandle;


                    if (gridView1.IsGroupRow(rowHandle))
                    {
                        rowHandle = gridView1.GetDataRowHandleByGroupRowHandle(rowHandle);
                    }

                    if (rowHandle < 0)
                    {

                        return;
                    }

                    DataRow rowFocused = gridView1.GetDataRow(rowHandle);
                    if (rowFocused == null)
                    {
                        return;
                    }

                    string MaPhieuBG = rowFocused["MaPhieuBG"]?.ToString();
                    string TenPhieuBG = rowFocused["TenPhieu"]?.ToString();

                    if (string.IsNullOrEmpty(MaPhieuBG))
                    {
                        XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để sửa",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    frmERPBaoGiaNCC_MayMocTB frm = new frmERPBaoGiaNCC_MayMocTB("copy", false, "", TenPhieuBG, MaPhieuBG);

                    frm.Show();
                    frm.MaximizeBox = true;

                    frm.FormClosed += Frm_FormClosed;
                }
            }
            catch(Exception ex)
            {

            }
        }

        

        private void btnNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.EditValue = pageIndex.ToString();
            LoadPhieuBaoGia();
        }

        private void btnPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.EditValue = pageIndex.ToString();
                LoadPhieuBaoGia();
            }
        }
        private void btnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Point mousePos = Control.MousePosition;
            frmSearchPhieuBaoGia frm = new frmSearchPhieuBaoGia(mousePos.X, mousePos.Y, xtabPhieuBaoGia.SelectedTabPageIndex);
     
            frm.StartPosition = FormStartPosition.Manual;
            frm.OnDataUpdate += (s, tblSearch) =>
            {
                if (xtabPhieuBaoGia.SelectedTabPage == xtabNguyenPhuLieu)
                {

                    grcPhieuBaoGiaTQ.DataSource =tblSearch;
                    grcPhieuBaoGiaTQ.RefreshDataSource();
                    grvPhieuBaoGiaTQ.ExpandAllGroups();
                }
                else if (xtabPhieuBaoGia.SelectedTabPage == xtabMayMocThietBi)
                {
                    gridControl1.DataSource = tblSearch;
                    gridControl1.RefreshDataSource();
                    gridView1.ExpandAllGroups();
                }
            };
            frm.ShowDialog();
        }

        #region Send To Notify
        private void SendNotify(string Title, string Detail, string SendTo, string BoPhan = "ALL", int Status = -1)
        {

            try
            {
                string url = $"{URL}SendToNotification/PushNotificationFrm?" +
                $"UserIDTao={HttpUtility.UrlEncode(GlobleData.UserName)}&" +
                $"FrmName={HttpUtility.UrlEncode(this.Name)}&" +
                $"Title={HttpUtility.UrlEncode(Title)}&" +
                $"Detail={HttpUtility.UrlEncode(Detail)}&" +
                $"SendTo={HttpUtility.UrlEncode(SendTo)}&" +
                $"BoPhan={HttpUtility.UrlEncode(BoPhan)}&" +
                $"Status={Status}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;

            }
            catch (Exception e)
            {

            }

        }
        #endregion
    

}
}