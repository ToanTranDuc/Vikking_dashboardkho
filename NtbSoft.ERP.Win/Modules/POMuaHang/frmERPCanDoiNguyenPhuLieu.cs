using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Web.Api.POMuaHang;
using NtbSoft.ERP.Win.Modules.ThuVien;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmERPCanDoiNguyenPhuLieu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<string> lstFormatFieldName = new List<string> { "SLCanDoi", "SLCapPhat", "SLTonKhoSauCanDoi", "SLMuaThem" };

        private string maPhieu;

        private DataTable grcPhieuCDVatTuTable;
        private DataTable gridControl11Table;
        private DataTable phieuCDTable;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        public frmERPCanDoiNguyenPhuLieu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
        }

        protected override void OnLoad(EventArgs e)
        {

            //LoadPhieuCanDoi();
            getPhieuCanDoi();
            calcPhieuCanDoi();
            loadGrcPhieuCDVatTu();

            calcCanDoiTheoVT();
            loadGridControl11();
        
            grvPhieuCDVatTu.ColumnPanelRowHeight = 50;   
            grvPhieuCDVatTu.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;;

            suaBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            xoaBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
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
                themBtn.Enabled = false;
                //barButtonItem1.Enabled = false;
            }
            if (!_allowEdit)
            {
                suaBtn.Enabled = false;
            }
            if (!_allowDelete)
            {
                xoaBtn.Enabled = false;
                //barButtonItem10.Enabled = false;
            }

        }

        private void loadGrcPhieuCDVatTu()
        {
            grcPhieuCDVatTu.DataSource = grcPhieuCDVatTuTable;
            foreach (GridColumn col in grvPhieuCDVatTu.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            //grvPhieuCDVatTu.Columns["SLCanDoiKho"].OptionsColumn.AllowEdit = true;
            //grvPhieuCDVatTu.Columns["SLMuaThem"].OptionsColumn.AllowEdit = true;
            //grvPhieuCDVatTu.Columns["TyLeMuaThem"].OptionsColumn.AllowEdit = true;
            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            textEdit.Mask.EditMask = "n2"; // 2 số thập phân
            textEdit.Mask.UseMaskAsDisplayFormat = true;
            grvPhieuCDVatTu.Columns["SLNhuCau"].ColumnEdit = textEdit;
            grvPhieuCDVatTu.Columns["SLMuaThemTong"].ColumnEdit = textEdit;
            grvPhieuCDVatTu.Columns["SLCanDoiKho"].ColumnEdit = textEdit;
            grvPhieuCDVatTu.Columns["SLMuaThem"].ColumnEdit = textEdit;
            grvPhieuCDVatTu.Columns["TyLeMuaThem"].ColumnEdit = textEdit;
        }
        private void getPhieuCanDoi()
        {
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "getphieucandoi";
            request.Parameter = maPhieu;
            string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            phieuCDTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!phieuCDTable.Columns.Contains("generatedID"))
                phieuCDTable.Columns.Add("generatedID");
            if (!phieuCDTable.Columns.Contains("IsNPLText"))
                phieuCDTable.Columns.Add("IsNPLText", typeof(string));
            if (!phieuCDTable.Columns.Contains("TenPhieuSort"))
                phieuCDTable.Columns.Add("TenPhieuSort", typeof(int));
            if (!phieuCDTable.Columns.Contains("SLNhuCau"))
                phieuCDTable.Columns.Add("SLNhuCau", typeof(decimal));
            if (!phieuCDTable.Columns.Contains("SLMuaThemTong"))
                phieuCDTable.Columns.Add("SLMuaThemTong", typeof(decimal));
            if (!phieuCDTable.Columns.Contains("Sort"))
                phieuCDTable.Columns.Add("Sort", typeof(int));
            foreach (DataRow row in phieuCDTable.Rows)
            {
                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                var match = Regex.Match(row["TenPhieu"].ToString(), @"\d+");
                row["TenPhieuSort"] = match.Success ? int.Parse(match.Value) : 0;

                if ((bool)row["IsDuyet"] == true) row["TenPhieu"] = row["TenPhieu"].ToString() + " - Đã duyệt";
                else row["TenPhieu"] = row["TenPhieu"].ToString() + " - Chưa duyệt";
            }
        }
        private void calcPhieuCanDoi()
        {
            DataTable dt = phieuCDTable.Copy();
            grcPhieuCDVatTuTable = phieuCDTable.Clone();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            foreach (DataRow row in dt.Rows)
            {
                //if (row["DinhMucHaoHut"] == DBNull.Value || row["DinhMucHaoHut"].ToString() == "") row["DinhMucHaoHut"] = 0;
                //if (row["DinhMucChung"] == DBNull.Value || row["DinhMucChung"].ToString() == "") row["DinhMucChung"] = 0;
                //float slCapPhat = XuLyVTUnits.SmartTryParse<float>(row["DinhMucChung"].ToString()) *
                //    XuLyVTUnits.SmartTryParse<float>(row["SoLuong"].ToString()) *
                //    (1 + XuLyVTUnits.SmartTryParse<float>(row["DinhMucHaoHut"].ToString()) / 100);               
                //slCapPhat = (float)Math.Round(slCapPhat, 2, MidpointRounding.AwayFromZero);
                //row["SLNhuCau"] = slCapPhat;

                float slMuaThemTong = XuLyVTUnits.SmartTryParse<float>(row["SLMuaThem"].ToString()) *
                    (1 + XuLyVTUnits.SmartTryParse<float>(row["TyLeMuaThem"].ToString()) / 100);
                row["SLMuaThemTong"] = slMuaThemTong;
            }
            grcPhieuCDVatTuTable = dt.Copy();
        }


        private void loadGridControl11()
        {
            gridControl11.DataSource = gridControl11Table;
            foreach (GridColumn col in gridView11.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            //gridView11.Columns["SLCanDoiKho"].OptionsColumn.AllowEdit = true;
            //gridView11.Columns["SLMuaThem"].OptionsColumn.AllowEdit = true;
            //gridView11.Columns["TyLeMuaThem"].OptionsColumn.AllowEdit = true;
            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            textEdit.Mask.EditMask = "n2"; // 2 số thập phân
            textEdit.Mask.UseMaskAsDisplayFormat = true;
            gridView11.Columns["SLNhuCau"].ColumnEdit = textEdit;
            gridView11.Columns["SLMuaThemTong"].ColumnEdit = textEdit;
            gridView11.Columns["SLCanDoiKho"].ColumnEdit = textEdit;
            gridView11.Columns["SLMuaThem"].ColumnEdit = textEdit;
            gridView11.Columns["TyLeMuaThem"].ColumnEdit = textEdit;
        }
        private void calcCanDoiTheoVT()
        {
            DataTable dt = phieuCDTable.Copy();
            gridControl11Table = phieuCDTable.Clone();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            var groupedResult = from row in dt.AsEnumerable()
                                group row by new
                                {
                                    MaVTID = row.Field<string>("MaVTID"),
                                    MaNhomVT = row.Field<string>("MaNhomVT"),
                                    ChungLoaiVatTu = row.Field<string>("ChungLoaiVatTu"),
                                    MaVT = row.Field<string>("MaVT"),
                                    ChiTiet = row.Field<string>("ChiTiet"),
                                    CodeMau = row.Field<string>("CodeMau"),
                                    MaMauVT = row.Field<string>("MaMauVT"),
                                    MauVT = row.Field<string>("MauVT"),
                                    MaKhoVT = row.Field<string>("MaKhoVT"),
                                    KhoVai = row.Field<string>("KhoVai"),
                                    TenDVVT = row.Field<string>("TenDVVT"),
                                    IsNPL = row.Field<string>("IsNPLText"),

                                } into grp
                                select new
                                {
                                    grp.Key.MaVTID,
                                    grp.Key.MaNhomVT,
                                    grp.Key.ChungLoaiVatTu,
                                    grp.Key.MaVT,
                                    grp.Key.ChiTiet,
                                    grp.Key.CodeMau,
                                    grp.Key.MaMauVT,
                                    grp.Key.MauVT,
                                    grp.Key.MaKhoVT,
                                    grp.Key.KhoVai,
                                    grp.Key.TenDVVT,
                                    grp.Key.IsNPL,
                                    SoLuong = grp.Sum(r => XuLyVTUnits.SmartTryParse<decimal>(r.Field<Int64>("SoLuong").ToString())),
                                    SLNhuCau = grp.Sum(r => XuLyVTUnits.SmartTryParse<decimal>(r.Field<double>("SLNhuCau").ToString())),
                                    SLCanDoiKho = grp.Sum(r => XuLyVTUnits.SmartTryParse<decimal>(r.Field<double>("SLCanDoiKho").ToString())),
                                    SLMuaThemTong = grp.Sum(r =>
                                    {
                                        decimal slMua = XuLyVTUnits.SmartTryParse<decimal>(r.Field<double>("SLMuaThem").ToString());
                                        decimal tyLe = XuLyVTUnits.SmartTryParse<decimal>(r.Field<double>("TyLeMuaThem").ToString());
                                        return slMua * (1 + tyLe / 100m);
                                    })
                                    //Count = grp.Count(), // Example: count rows in each group
                                    //Cities = string.Join(", ", grp.Select(r => r.Field<string>("City")).Distinct()) // Example: list unique cities
                                };
            foreach (var item in groupedResult)
            {
                DataRow newRow = gridControl11Table.NewRow();
                newRow["MaVTID"] = item.MaVTID;
                newRow["MaNhomVT"] = item.MaNhomVT;
                newRow["ChungLoaiVatTu"] = item.ChungLoaiVatTu;
                newRow["MaVT"] = item.MaVT;
                newRow["ChiTiet"] = item.ChiTiet;
                newRow["CodeMau"] = item.CodeMau;
                newRow["MaMauVT"] = item.MaMauVT;
                newRow["MauVT"] = item.MauVT;
                newRow["MaKhoVT"] = item.MaKhoVT;
                newRow["KhoVai"] = item.KhoVai;
                newRow["TenDVVT"] = item.TenDVVT;
                newRow["IsNPLText"] = item.IsNPL;
                newRow["SoLuong"] = item.SoLuong;
                newRow["SLNhuCau"] = item.SLNhuCau;
                newRow["SLCanDoiKho"] = item.SLCanDoiKho;
                newRow["SLMuaThemTong"] = item.SLMuaThemTong;

                gridControl11Table.Rows.Add(newRow);
            }
        }
        private void gridView11_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "TyLeMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThemTong")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLCanDoiKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }

            }
        }
        private void gridView11_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.IsGroupRow(e.RowHandle))
            {
                // Luôn tô màu cho group row
                e.Appearance.BackColor = Color.WhiteSmoke;
                e.Appearance.ForeColor = Color.Black;
                e.HighPriority = true; // đảm bảo override
            }
        }



        private void grvPhieuCDVatTu_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            // Chỉ xử lý group cha (level 0)
            int level = view.GetRowLevel(e.RowHandle);
            if (level != 0) return;

            // Kiểm tra duyệt
            bool allApproved = true;
            int childCount = view.GetChildRowCount(e.RowHandle);

            for (int i = 0; i < childCount; i++)
            {
                int childHandle = view.GetChildRowHandle(e.RowHandle, i);

                if (view.IsGroupRow(childHandle))
                {
                    // Duyệt group con → các data row bên trong
                    int subChildCount = view.GetChildRowCount(childHandle);
                    for (int j = 0; j < subChildCount; j++)
                    {
                        int dataHandle = view.GetChildRowHandle(childHandle, j);
                        bool isDuyet = Convert.ToBoolean(view.GetRowCellValue(dataHandle, view.Columns["IsDuyet"]));
                        if (!isDuyet)
                        {
                            allApproved = false;
                            break;
                        }
                    }
                    if (!allApproved) break;
                }
                else
                {
                    // Data row trực tiếp
                    bool isDuyet = Convert.ToBoolean(view.GetRowCellValue(childHandle, view.Columns["IsDuyet"]));
                    if (!isDuyet)
                    {
                        allApproved = false;
                        break;
                    }
                }
            }

            // Chỉ đổi màu, KHÔNG đổi/chèn chữ
            if (allApproved)
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.ForeColor = Color.DarkGreen;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else
            {
                e.Appearance.BackColor = Color.LightCoral;
                e.Appearance.ForeColor = Color.DarkRed;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }

            // Đảm bảo dùng màu được set ở Appearance
            e.Appearance.Options.UseBackColor = true;
            e.Appearance.Options.UseForeColor = true;
            e.Appearance.Options.UseFont = true;

            // Vẽ đối tượng group row mặc định với Appearance đã chỉnh
            e.Painter.DrawObject(e.Info);
            e.Handled = true;
        }    
        private void grvPhieuCDVatTu_RowStyle_1(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.IsGroupRow(e.RowHandle))
            {
                // Luôn tô màu cho group row
                e.Appearance.BackColor = Color.WhiteSmoke;
                e.Appearance.ForeColor = Color.Black;
                e.HighPriority = true; // đảm bảo override
            }
        }
        private void grvPhieuCDVatTu_CustomColumnSort_1(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "TenPhieu")
            {
                int x = ExtractNumber(e.Value1 as string);
                int y = ExtractNumber(e.Value2 as string);

                // đổi chiều nếu muốn Desc ở cấp group (kết hợp với SortOrder Desc ở trên)
                e.Result = x.CompareTo(y);
                e.Handled = true;
            }
        }
        private int ExtractNumber(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;
            var m = System.Text.RegularExpressions.Regex.Match(input, @"\d+");
            return m.Success ? int.Parse(m.Value) : 0;
        }
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
            //try
            //{
            //    if (e.Column == null) return;

            //    Rectangle rect = e.Bounds;
            //    ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);

            //    Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102),
            //                                            Color.FromArgb(255, 204, 102),
            //                                            e.Column.AppearanceHeader.GradientMode);
            //    rect.Inflate(-1, -1);
            //    e.Graphics.FillRectangle(brush, rect);

            //    Font font = new Font(e.Appearance.Font, FontStyle.Bold);

            //    StringFormat stringFormat = new StringFormat();
            //    stringFormat.Alignment = StringAlignment.Center;
            //    stringFormat.LineAlignment = StringAlignment.Center;
            //    stringFormat.Trimming = StringTrimming.None;
            //    stringFormat.FormatFlags = StringFormatFlags.NoClip;
            //    stringFormat.FormatFlags &= ~StringFormatFlags.NoWrap; // Cho phép wrap

            //    SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102));

            //    Rectangle textRect = e.Info.CaptionRect;
            //    textRect.Inflate(-4, -2); // Padding để text không sát viền

            //    SizeF textSize = e.Graphics.MeasureString(e.Info.Caption, font, textRect.Width, stringFormat);

            //    // Tự động điều chỉnh chiều cao header nếu text cao hơn
            //    GridView view = sender as GridView;

            //    // Vẽ chữ với khả năng wrap
            //    e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, textRect, stringFormat);

            //    // Vẽ các element khác (sort arrow, filter button, etc.)
            //    foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //    {
            //        DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //    }

            //    e.Handled = true;
            //}
            //catch (Exception ex)
            //{
            //    // Log error nếu cần
            //}
        }
        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;
                DataRow rowFocused = grvPhieuCDVatTu.GetFocusedDataRow() as DataRow;
                if (rowFocused == null) return;
                bool.TryParse(rowFocused["IsUse"]?.ToString(), out bool IsUse);
                if (IsUse)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
                else if (view.FocusedColumn == colIsDuyet)
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
        private void grvPhieuCDVatTu_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                DataRow rowFocused = grvPhieuCDVatTu.GetFocusedDataRow() as DataRow;
                if (rowFocused == null) return;

                if (grvPhieuCDVatTu.FocusedColumn == colIsDuyet)
                {
                    bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                    DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn {(!IsDuyet ? "Duyệt" : "Hủy Duyệt")} Phiếu {rowFocused["TenPhieu"]?.ToString()} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                    if (messResult != DialogResult.Yes) return;
                    //string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                    //ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                    //objXetDuyet.Action = "DuyetCanDoi";
                    //objXetDuyet.MaPhieu = rowFocused["MaPhieu"]?.ToString();
                    //objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    //objXetDuyet.IsXacNhan = !IsDuyet;
                    //objXetDuyet.TrangThai = !IsDuyet ? "Đang Xử Lý" : "Đã Hủy Xét Duyệt";
                    //objXetDuyet.NguoiXN = GlobleData.UserName;
                    //objXetDuyet.GhiChu = "";
               
                    //string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;

                    var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
                    request.Action = "updateduyetphieucandoi";
                    request.Parameter = rowFocused["MaPhieu"]?.ToString();
                    request.Parameter1 = IsDuyet ? "cancel" : "accept";
                    string urlGetListDataTable = URL + "CanDoiNPL/Post";
                    string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                    if (response.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        this.DialogResult = DialogResult.OK;
                        getPhieuCanDoi();
                        calcPhieuCanDoi();
                        loadGrcPhieuCDVatTu();
                    }
                    else
                    {
                        XtraMessageBox.Show(response, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            
        }
        private void grvPhieuCDVatTu_FocusedRowChanged_1(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (grvPhieuCDVatTu.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = grvPhieuCDVatTu.GetFocusedDataRow() as DataRow;
                if (rowFocused != null)
                {
                    string MaPhieu = rowFocused["MaPhieu"]?.ToString();
                }
            }
            focused(sender);
        }
        private void grvPhieuCDVatTu_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "TyLeMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThemTong")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLCanDoiKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }

            }
        }



        //private void themBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
        //    frmCanDoiNPLVT frm = new frmCanDoiNPLVT();
        //    frm.ShowDialog();
        //    frm.MaximizeBox = true;
        //    if (frm.DialogResult == DialogResult.OK)
        //    {
        //        getPhieuCanDoi();
        //        calcPhieuCanDoi();
        //        loadGrcPhieuCDVatTu();
        //    }
        //}
        private void themBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmSelectModeCanDoi frmSelect = new frmSelectModeCanDoi();
            if (frmSelect.ShowDialog() != DialogResult.OK) return;

            //SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            if (frmSelect.IsCanDoiVatTu)
            {
                frmCanDoiNPLVT frm = new frmCanDoiNPLVT();
                frm.MaximizeBox = true;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    getPhieuCanDoi();
                    calcPhieuCanDoi();
                    loadGrcPhieuCDVatTu();
                }
            }
            else if (frmSelect.IsCanDoiDonHang)
            {
                //SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
                frmCanDoiNPLDH frm = new frmCanDoiNPLDH();
                //frmCanDoiNPL frm = new frmCanDoiNPL();
                frm.MaximizeBox = true;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    getPhieuCanDoi();
                    calcPhieuCanDoi();
                    loadGrcPhieuCDVatTu();
                }
            }
        }
        private void suaBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = grvPhieuCDVatTu;
            GridColumn colMaPhieu = view.Columns["MaPhieu"];
            GridColumn colMaDH = view.Columns["MaDH"];
            GridColumn colDuyet = view.Columns["IsDuyet"];

            string MaPhieu = null;
            string DonHang = null;
            bool IsDuyet = false;

            int focusedHandle = view.FocusedRowHandle;

            if (view.IsGroupRow(focusedHandle))
            {
                int level = view.GetRowLevel(focusedHandle);

                if (level == 0)
                {
                    // Group cha → lấy group con đầu tiên
                    int childGroupHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childGroupHandle < 0 && view.IsGroupRow(childGroupHandle))
                    {
                        // Lấy data row đầu tiên trong group con
                        int childDataHandle = view.GetChildRowHandle(childGroupHandle, 0);
                        if (childDataHandle >= 0)
                        {
                            MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                            DonHang = view.GetRowCellValue(childDataHandle, colMaDH)?.ToString();
                            IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                        }
                    }
                }
                else if (level == 1)
                {
                    // Group con → lấy data row đầu tiên
                    int childDataHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childDataHandle >= 0)
                    {
                        MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                        DonHang = view.GetRowCellValue(childDataHandle, colMaDH)?.ToString();
                        IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                    }
                }
            }
            else if (focusedHandle >= 0)
            {
                // Data row bình thường
                MaPhieu = view.GetFocusedRowCellValue(colMaPhieu)?.ToString();
                DonHang = view.GetFocusedRowCellValue(colMaDH)?.ToString();
                IsDuyet = Convert.ToBoolean(view.GetFocusedRowCellValue(colDuyet));
            }

            if (string.IsNullOrEmpty(MaPhieu))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cân đối để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (IsDuyet)
            {
                XtraMessageBox.Show("Phiếu cân đối đã được duyệt. Không thể sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MaPhieu.Contains("group_dhvt"))
            {
                using (var frm = new frmCanDoiNPLDH(MaPhieu))
                {
                    frm.MaximizeBox = true;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        getPhieuCanDoi();
                        calcPhieuCanDoi();
                        loadGrcPhieuCDVatTu();

                        calcCanDoiTheoVT();
                        loadGridControl11();
                    }
                }
            }
            else if (MaPhieu.Contains("group"))
            {
                using (var frm = new frmCanDoiNPLVT(MaPhieu))
                {
                    frm.MaximizeBox = true;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        getPhieuCanDoi();
                        calcPhieuCanDoi();
                        loadGrcPhieuCDVatTu();

                        calcCanDoiTheoVT();
                        loadGridControl11();
                    }
                }
            }
            else
            {
                using (var frm = new frmCanDoiNPLTheoDH(DonHang))
                {
                    frm.MaximizeBox = true;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        getPhieuCanDoi();
                        calcPhieuCanDoi();
                        loadGrcPhieuCDVatTu();

                        calcCanDoiTheoVT();
                        loadGridControl11();
                    }
                }
            }
        }
        private void xoaBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = grvPhieuCDVatTu;
            GridColumn colMaPhieu = view.Columns["MaPhieu"];
            GridColumn colTenPhieu = view.Columns["TenPhieu"];
            GridColumn colMaDH = view.Columns["MaDH"];
            GridColumn colDuyet = view.Columns["IsDuyet"];

            string MaPhieu = null;
            string TenPhieu = null;
            string DonHang = null;
            bool IsDuyet = false;

            int focusedHandle = view.FocusedRowHandle;

            if (view.IsGroupRow(focusedHandle))
            {
                int level = view.GetRowLevel(focusedHandle);

                if (level == 0)
                {
                    // Group cha → lấy group con đầu tiên
                    int childGroupHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childGroupHandle < 0 && view.IsGroupRow(childGroupHandle))
                    {
                        // Lấy data row đầu tiên trong group con
                        int childDataHandle = view.GetChildRowHandle(childGroupHandle, 0);
                        if (childDataHandle >= 0)
                        {
                            MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                            TenPhieu = view.GetRowCellValue(childDataHandle, colTenPhieu)?.ToString();
                            DonHang = view.GetRowCellValue(childDataHandle, colMaDH)?.ToString();
                            IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                        }
                    }
                }
                else if (level == 1)
                {
                    // Group con → lấy data row đầu tiên
                    int childDataHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childDataHandle >= 0)
                    {
                        MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                        TenPhieu = view.GetRowCellValue(childDataHandle, colTenPhieu)?.ToString();
                        DonHang = view.GetRowCellValue(childDataHandle, colMaDH)?.ToString();
                        IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                    }
                }
            }
            else if (focusedHandle >= 0)
            {
                // Data row bình thường
                MaPhieu = view.GetFocusedRowCellValue(colMaPhieu)?.ToString();
                TenPhieu = view.GetFocusedRowCellValue(colTenPhieu)?.ToString();
                DonHang = view.GetFocusedRowCellValue(colMaDH)?.ToString();
                IsDuyet = Convert.ToBoolean(view.GetFocusedRowCellValue(colDuyet));
            }

            if (string.IsNullOrEmpty(MaPhieu))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cân đối để xoá", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(MaPhieu))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cân đối để xoá", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (IsDuyet)
            {
                XtraMessageBox.Show("Phiếu cân đối đã được duyệt. Không thể xoá", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] parts = TenPhieu.Split('-');
            string result = parts[0].Trim();

            DialogResult messResult = MessageBox.Show(
               $"Bạn có muốn xóa {result} không?",
               "Thông báo",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);
            if (messResult != DialogResult.Yes) return;
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "deletephieucandoi";
            request.Parameter = MaPhieu;
            string urlGetListDataTable = URL + "CanDoiNPL/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            if (response == "OK")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                getPhieuCanDoi();
                calcPhieuCanDoi();
                loadGrcPhieuCDVatTu();

                calcCanDoiTheoVT();
                loadGridControl11();
            }
            else
            {
                XtraMessageBox.Show("Phiếu cân đối đã được tạo phiếu mua hàng. Không thể xoá", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void duyetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
                    r["ModuleID"]?.ToString() == "M.12.01.00" &&
                    r["AllowDuyet"] != DBNull.Value &&
                    Convert.ToBoolean(r["AllowDuyet"]) == true
                );

                if (!coQuyenDuyet)
                {
                    XtraMessageBox.Show("Bạn không có quyền duyệt!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            GridView view = grvPhieuCDVatTu;
            GridColumn colMaPhieu = view.Columns["MaPhieu"];
            GridColumn colTenPhieu = view.Columns["TenPhieu"];
            GridColumn colDuyet = view.Columns["IsDuyet"];

            string MaPhieu = null;
            string TenPhieu = null;
            bool IsDuyet = false;

            int focusedHandle = view.FocusedRowHandle;

            if (view.IsGroupRow(focusedHandle))
            {
                int level = view.GetRowLevel(focusedHandle);

                if (level == 0)
                {
                    // Group cha → lấy group con đầu tiên
                    int childGroupHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childGroupHandle < 0 && view.IsGroupRow(childGroupHandle))
                    {
                        // Lấy data row đầu tiên trong group con
                        int childDataHandle = view.GetChildRowHandle(childGroupHandle, 0);
                        if (childDataHandle >= 0)
                        {
                            MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                            TenPhieu = view.GetRowCellValue(childDataHandle, colTenPhieu)?.ToString();
                            IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                        }
                    }
                }
                else if (level == 1)
                {
                    // Group con → lấy data row đầu tiên
                    int childDataHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childDataHandle >= 0)
                    {
                        MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                        TenPhieu = view.GetRowCellValue(childDataHandle, colTenPhieu)?.ToString();
                        IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                    }
                }
            }
            else if (focusedHandle >= 0)
            {
                // Data row bình thường
                MaPhieu = view.GetFocusedRowCellValue(colMaPhieu)?.ToString();
                TenPhieu = view.GetFocusedRowCellValue(colTenPhieu)?.ToString();
                IsDuyet = Convert.ToBoolean(view.GetFocusedRowCellValue(colDuyet));
            }

            if (string.IsNullOrEmpty(MaPhieu))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cân đối để duyệt", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(MaPhieu))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cân đối để duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (IsDuyet) return;
            DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn {(!IsDuyet ? "Duyệt" : "Hủy Duyệt")} Phiếu {TenPhieu} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (messResult != DialogResult.Yes) return;
            

            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "updateduyetphieucandoi";
            request.Parameter = MaPhieu;
            request.Parameter1 = IsDuyet ? "cancel" : "accept";
            string urlGetListDataTable = URL + "CanDoiNPL/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            if (response.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                getPhieuCanDoi();
                calcPhieuCanDoi();
                loadGrcPhieuCDVatTu();

                calcCanDoiTheoVT();
                loadGridControl11();
            }
        }
        private void huyduyetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

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
                    r["ModuleID"]?.ToString() == "M.12.01.00" &&
                    r["AllowDuyet"] != DBNull.Value &&
                    Convert.ToBoolean(r["AllowDuyet"]) == true
                );

                if (!coQuyenDuyet)
                {
                    XtraMessageBox.Show("Bạn không có quyền hủy duyệt!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            GridView view = grvPhieuCDVatTu;
            GridColumn colMaPhieu = view.Columns["MaPhieu"];
            GridColumn colTenPhieu = view.Columns["TenPhieu"];
            GridColumn colDuyet = view.Columns["IsDuyet"];

            string MaPhieu = null;
            string TenPhieu = null;
            bool IsDuyet = false;

            int focusedHandle = view.FocusedRowHandle;

            if (view.IsGroupRow(focusedHandle))
            {
                int level = view.GetRowLevel(focusedHandle);

                if (level == 0)
                {
                    // Group cha → lấy group con đầu tiên
                    int childGroupHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childGroupHandle < 0 && view.IsGroupRow(childGroupHandle))
                    {
                        // Lấy data row đầu tiên trong group con
                        int childDataHandle = view.GetChildRowHandle(childGroupHandle, 0);
                        if (childDataHandle >= 0)
                        {
                            MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                            TenPhieu = view.GetRowCellValue(childDataHandle, colTenPhieu)?.ToString();
                            IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                        }
                    }
                }
                else if (level == 1)
                {
                    // Group con → lấy data row đầu tiên
                    int childDataHandle = view.GetChildRowHandle(focusedHandle, 0);
                    if (childDataHandle >= 0)
                    {
                        MaPhieu = view.GetRowCellValue(childDataHandle, colMaPhieu)?.ToString();
                        TenPhieu = view.GetRowCellValue(childDataHandle, colTenPhieu)?.ToString();
                        IsDuyet = Convert.ToBoolean(view.GetRowCellValue(childDataHandle, colDuyet));
                    }
                }
            }
            else if (focusedHandle >= 0)
            {
                // Data row bình thường
                MaPhieu = view.GetFocusedRowCellValue(colMaPhieu)?.ToString();
                TenPhieu = view.GetFocusedRowCellValue(colTenPhieu)?.ToString();
                IsDuyet = Convert.ToBoolean(view.GetFocusedRowCellValue(colDuyet));
            }

            if (string.IsNullOrEmpty(MaPhieu))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cân đối để duyệt", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(MaPhieu))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cân đối để duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsDuyet) return;
            DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn {(!IsDuyet ? "Duyệt" : "Hủy Duyệt")} Phiếu {TenPhieu} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (messResult != DialogResult.Yes) return;

            var check = XuLyVTRequestPost.createDefault("CanDoiNPL");
            check.Action = "gettontaidonhang";
            check.Parameter = MaPhieu;
            string urlGetListDataTablecheck = URL + "CanDoiNPL/Post";
            string responsecheck = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTablecheck, check); }).Result;
            if (responsecheck == "no")
            {

            }
            else
            {
                XtraMessageBox.Show("Phiếu cân đối đã được tạo phiếu mua hàng. Không thể huỷ duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "updateduyetphieucandoi";
            request.Parameter = MaPhieu;
            request.Parameter1 = IsDuyet ? "cancel" : "accept";
            string urlGetListDataTable = URL + "CanDoiNPL/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            if (response.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                getPhieuCanDoi();
                calcPhieuCanDoi();
                loadGrcPhieuCDVatTu();

                calcCanDoiTheoVT();
                loadGridControl11();
            }
        }

        private void gridView11_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void resetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            getPhieuCanDoi();
            calcPhieuCanDoi();
            loadGrcPhieuCDVatTu();

            calcCanDoiTheoVT();
            loadGridControl11();
        }



        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page.Name == "xtraTabPage1") // tên tab bạn muốn
            {
                suaBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                xoaBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                suaBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                xoaBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        
    }
}