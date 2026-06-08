using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Libs;
using NtbSoft.ERP.Win.CanDoiDonHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    //public partial class frmDonHangTonKho : DevExpress.XtraEditors.XtraFormpublic partial class frmSuaDonHangTong : DevExpress.XtraEditors.XtraForm
    public partial class frmDonHangTonKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<string> lstSize;
        DataTable tblSLKho;
        DataTable tblSLKH;
        clsCommonBS clsCommonBS;
        int indexKho = -1;
        string maDH = string.Empty;
        string maHang = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlHistory;
        public frmDonHangTonKho()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            lstSize = new List<string>();
            tblSLKho = new DataTable();
            tblSLKH = new DataTable();
            clsCommonBS = new clsCommonBS();
            _clientExtension = new HttpClientExtension();
            //donHangTong = _donHangTong;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            CreateSearchLookup();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlHistory = new ActionControl(GetLichSu, true, ActionType.History, this.LichSu.Enabled);
            return new List<ActionControl> {
                actionControlHistory}; ;
        }

        private void CheckPerminsion()
        {
            SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
        }
        private void CreateSearchLookup()
        {
            string urlHH = string.Format("{0}", URL + "DonHangTonKho/GetDonHangTonKho");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEditMaDH.Properties.DataSource = tblHH;
        }


        //private void SearchLookupEditMaDH_EditValueChanged(object sender, EventArgs e)
        //{
        //    if (e != null && e is ChangingEventArgs)
        //    {
        //        LichSu.Enabled = true;
        //        ChangingEventArgs changingEvent = e as ChangingEventArgs;
        //        maDH = changingEvent.NewValue.ToString();
        //        GetSoLuongKHSX_Kho(changingEvent.NewValue.ToString());

        //        SearchLookUpEdit searchLookup = sender as SearchLookUpEdit;
        //        DataTable tbl = searchLookup.Properties.DataSource as DataTable;

        //        foreach (DataRow row in tbl.Rows)
        //        {
        //            if (row["MaDH"] == maDH)
        //            {
        //                maHang = row["MaHang"].ToString();
        //                break;
        //            }
        //        }
        //    }
        //}
        private void SearchLookupEditMaDH_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            DataTable dtSourceSearchLookup = new DataTable();
            Console.WriteLine("custom_displaytext");
            if (sender is SearchLookUpEdit)
            {
                SearchLookUpEdit search = sender as SearchLookUpEdit;
                if (search.Properties.DataSource != null)
                {
                    //dataSourceSearchLookup = new List<DicQCDongThungEntity>((IList<DicQCDongThungEntity>)search.Properties.DataSource);
                    dtSourceSearchLookup = search.Properties.DataSource as DataTable;
                }
            }
            if (e.Value != null && e.Value.ToString() != null && e.Value.ToString() != "")
            {
                //e.DisplayText = stringDisplaySearchLookup(e.Value.ToString(), dtSourceSearchLookup);
                StringBuilder sb = new StringBuilder();

                foreach (DataRow row in dtSourceSearchLookup.Rows)
                {
                    if (e.Value.ToString() == row["MaDH"])
                    {
                        AppendText(sb, "Mã hàng: ", string.Format("{0}, ", row["MaHang"].ToString()));
                        AppendText(sb, "Mã đơn hàng: ", string.Format("{0}, ", row["MaDH"].ToString()));
                        AppendText(sb, "Tên hàng: ", string.Format("{0}, ", row["TenHang"].ToString()));
                        AppendText(sb, "Số lượng: ", string.Format("{0}, ", row["SoLuong"].ToString()));
                        AppendText(sb, "Đợt: ", string.Format("{0}.", row["Dot"].ToString()));
                        break;
                    }
                }
                e.DisplayText = sb.ToString();
            }
        }

        private void AppendText(StringBuilder sb, string name, string content)
        {
            sb.Append(name);
            sb.Append(content);
        }

        private void gridviewLookupEditMH_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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


        private void btNapLai(object sender, ItemClickEventArgs e)
        {
            GetSoLuongKHSX_Kho(maDH,maHang);
        }
        private void GetLichSu()
        {
            if (!string.IsNullOrEmpty(maDH))
            {
                frmLichSuTruTonKho lichSuTruTonKho = new frmLichSuTruTonKho(maHang);

                lichSuTruTonKho.ShowDialog();
            }
        }
        private void btLichSu(object sender, ItemClickEventArgs e)
        {
            GetLichSu();
        }

        private DataTable RemoveNumberOfSize(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.Contains("@"))
                    {
                        dr[dc.ColumnName] = 0;
                    }
                }
            }
            return dt;
        }

        private void GetSoLuongKHSX_Kho(string maDH,string maHang)
        {
            try
            {
                // api SLKH
                string url = string.Format("{0}?maDH={1}", URL + "DonHangTonKho/GetPivotSLKH_SLSX", maDH);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl_SLKHSX = JsonConvert.DeserializeObject<DataTable>(json);
                tblSLKH = JsonConvert.DeserializeObject<DataTable>(json);

                // api SLKho
                string urlTonKho = string.Format("{0}?maHang={1}", URL + "DonHangTonKho/GetPivotSLKho", maHang);
                string jsonTonKho = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTonKho); }).Result;
                DataTable tbl_SLKho = JsonConvert.DeserializeObject<DataTable>(jsonTonKho);
                tblSLKho = JsonConvert.DeserializeObject<DataTable>(jsonTonKho);

                // Gán trị = 0 cho cột số lượng pivot lên là null của tblSLKH
                foreach (DataColumn column in tbl_SLKHSX.Columns)
                {
                    if (column.ColumnName.Contains("Size@"))
                    {
                        foreach (DataRow row in tbl_SLKHSX.Rows)
                        {
                            if (string.IsNullOrEmpty(row[column.ColumnName].ToString()))
                            {
                                row[column.ColumnName] = "0@0";
                            }
                        }
                    }
                }

                tblSLKH = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(tbl_SLKHSX));

                // Gán trị = 0 cho cột số lượng pivot lên là null của tbl_SLKho
                foreach (DataColumn column in tbl_SLKho.Columns)
                {
                    if (column.ColumnName.Contains("Size@"))
                    {
                        foreach (DataRow row in tbl_SLKho.Rows)
                        {
                            if (string.IsNullOrEmpty(row[column.ColumnName].ToString()))
                            {
                                row[column.ColumnName] = 0;
                            }
                        }
                    }
                }
                tblSLKho = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(tbl_SLKho));

                bandedGridViewDanhSachSoLuongTonKho = createColBandsSizeSLTonKho(tbl_SLKho);
                tbl_SLKho = AddColumnDataTableKho(tbl_SLKho);
                tblSLKho = AddColumnDataTableKho(tblSLKho);
                gridControlDanhSachSoLuongTonKho.DataSource = tbl_SLKho;

                // Danh sách số lượng kế hoạch

                bandedGridViewDanhSachSoLuong_KHSX = createColBandsSizeSLKeHoach(tbl_SLKHSX);

                tbl_SLKHSX = AddColumnDataTable(tbl_SLKHSX);
                tblSLKH = AddColumnDataTable(tblSLKH);

                gridControlDanhSachSoLuong_KHSX.DataSource = tbl_SLKHSX;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Thêm cột vào DataTable
        private DataTable AddColumnDataTable(DataTable dataTable)
        {
            // dataTableTemp bảng tạm thời dùng để xử lý
            DataTable dataTableTemp = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(dataTable));

            string nameSizeSLKH = "Size_SLKH@";
            string nameSizeSLSX = "Size_SLSX@";

            // 1. Thêm cột mới vào DataTable
            // Thêm cột SLKH và SLSX cho các size
            // Ví dụ: Size_SLKH@42 | Size_SLSX@42
            foreach (DataColumn dc in dataTable.Columns)
            {
                if (dc.ColumnName.Contains("Size@"))
                {
                    string[] lstName = dc.ColumnName.Split('@');
                    if (lstName.Length == 3)
                    {
                        //// Name SLKH
                        //string newNameColumnSLKH = string.Format("{0}{1}", nameSizeSLKH, lstName[1]);
                        //DataColumn newColumnSLKH = new DataColumn(newNameColumnSLKH, typeof(int));
                        //dataTableTemp.Columns.Add(newColumnSLKH);

                        //// Name SLSX
                        //string newNameColumnSLSX = string.Format("{0}{1}", nameSizeSLSX, lstName[1]);
                        //DataColumn newColumnSLSX = new DataColumn(newNameColumnSLSX, typeof(int));
                        //dataTableTemp.Columns.Add(newColumnSLSX);

                        // Name SLKH
                        string newNameColumnSLKH = string.Format("{0}{1}@{2}", nameSizeSLKH, lstName[1], lstName[2]);
                        DataColumn newColumnSLKH = new DataColumn(newNameColumnSLKH, typeof(int));
                        dataTableTemp.Columns.Add(newColumnSLKH);

                        // Name SLSX
                        string newNameColumnSLSX = string.Format("{0}{1}@{2}", nameSizeSLSX, lstName[1], lstName[2]);
                        DataColumn newColumnSLSX = new DataColumn(newNameColumnSLSX, typeof(int));
                        dataTableTemp.Columns.Add(newColumnSLSX);

                    }
                }
            }

            // 2. Chép dữ liệu từ cột số lượng sang cột Số lượng kế hoạch
            // Ví dụ: Size@42 -> Size_SLKH@42 VÀ Size_SLSX@42
            foreach (DataRow row in dataTableTemp.Rows)
            {
                foreach (DataColumn column in dataTableTemp.Columns)
                {
                    if (column.ColumnName.Contains(nameSizeSLKH))
                    {
                        // column.ColumnName có format Size@42
                        string nameSize = column.ColumnName.Split('@')[1];
                        string sizeID = column.ColumnName.Split('@')[2];
                        // giá trị của row[string.Format("Size@{0}", nameSize)] là giá trị trả về từ sql
                        // row[Size@42]= "776@526"
                        // split giá trị ra 776 và 526
                        // 776 là giá trị của SLKH
                        // 526 là giá trị của SLSX
                        int temp = 0;
                        if (int.TryParse(row[string.Format("Size@{0}@{1}", nameSize, sizeID)].ToString().Split('@')[0].ToString(), out temp))
                        {
                            int slkh = int.Parse(row[string.Format("Size@{0}@{1}", nameSize, sizeID)].ToString().Split('@')[0].ToString());
                            row[column.ColumnName] = slkh;
                        }
                    }
                    else if (column.ColumnName.Contains(nameSizeSLSX))
                    {
                        // tương tự như slkh trên
                        string nameSize = column.ColumnName.Split('@')[1];
                        string sizeID = column.ColumnName.Split('@')[2];
                        int slsx = int.Parse(row[string.Format("Size@{0}@{1}", nameSize, sizeID)].ToString().Split('@')[1].ToString());
                        row[column.ColumnName] = slsx;
                    }
                }
            }

            // Xóa Cột Size@
            for (int i = 0; i < dataTableTemp.Columns.Count;)
            {
                if (dataTableTemp.Columns[i].ColumnName.Contains("Size@"))
                {
                    dataTableTemp.Columns.Remove(dataTableTemp.Columns[i].ColumnName);
                }
                else
                {
                    i++;
                }
            }

            // dataTablePivot là DataTable hoàn chỉnh( Đã thêm cột SLKH và SLSX)
            DataTable dataTablePivot = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(dataTableTemp));

            return dataTablePivot;
        }

        private DataTable AddColumnDataTableKho(DataTable dataTable)
        {
            // dataTableTemp bảng tạm thời dùng để xử lý
            DataTable dataTableTemp = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(dataTable));

            string nameSizeSLTK = "Size_SLTK@";
            string nameSizeSLCLK = "Size_SLCLK@";

            // 1. Thêm cột mới vào DataTable
            // Thêm cột SLTK và SLCLK cho các size
            // Ví dụ: Size_SLTK@42 | Size_SLCLK@42
            foreach (DataColumn dc in dataTable.Columns)
            {
                if (dc.ColumnName.Contains("Size@"))
                {
                    string[] lstName = dc.ColumnName.Split('@');
                    if (lstName.Length == 3)
                    {
                        // Name SLTK
                        string newNameColumnSLTK = string.Format("{0}{1}@{2}", nameSizeSLTK, lstName[1], lstName[2]);
                        DataColumn newColumnSLTK = new DataColumn(newNameColumnSLTK, typeof(int));
                        dataTableTemp.Columns.Add(newColumnSLTK);

                        // Name SLCLK
                        string newNameColumnSLCLK = string.Format("{0}{1}@{2}", nameSizeSLCLK, lstName[1], lstName[2]);
                        DataColumn newColumnSLCLK = new DataColumn(newNameColumnSLCLK, typeof(int));
                        dataTableTemp.Columns.Add(newColumnSLCLK);

                    }
                }
            }

            // 2. Chép dữ liệu từ cột số lượng sang cột Số lượng tồn kho
            // Ví dụ: Size@42 -> Size_SLTK@42 và Size_SLCLK@42
            foreach (DataRow row in dataTableTemp.Rows)
            {
                foreach (DataColumn column in dataTableTemp.Columns)
                {
                    if (column.ColumnName.Contains(nameSizeSLTK))
                    {
                        string nameSize = column.ColumnName.Split('@')[1];
                        string sizeID = column.ColumnName.Split('@')[2];
                        if (row[string.Format("Size@{0}@{1}", nameSize, sizeID)].ToString().Contains("@"))
                        {
                            // column.ColumnName có format Size@42                           
                            // giá trị của row[string.Format("Size@{0}", nameSize)] là giá trị trả về từ sql
                            // row[Size@42]= "776@526"
                            // split giá trị ra 776 và 526
                            // 776 là giá trị của SLTK
                            // 526 là giá trị của SLCLK
                            int sltk = int.Parse(row[string.Format("Size@{0}@{1}", nameSize, sizeID)].ToString().Split('@')[0].ToString());
                            row[column.ColumnName] = sltk;
                        }
                        else
                        {
                            row[column.ColumnName] = 0;
                        }
                    }
                    else if (column.ColumnName.Contains(nameSizeSLCLK))
                    {
                        string nameSize = column.ColumnName.Split('@')[1];
                        string sizeID = column.ColumnName.Split('@')[2];
                        if (row[string.Format("Size@{0}@{1}", nameSize, sizeID)].ToString().Contains("@"))
                        {
                            // tương tự như slclk trên

                            int slsx = int.Parse(row[string.Format("Size@{0}@{1}", nameSize, sizeID)].ToString().Split('@')[1].ToString());
                            row[column.ColumnName] = slsx;
                        }
                        else
                        {
                            row[column.ColumnName] = 0;
                        }
                    }
                }
            }

            // Xóa Cột Size@
            for (int i = 0; i < dataTableTemp.Columns.Count;)
            {
                if (dataTableTemp.Columns[i].ColumnName.Contains("Size@"))
                {
                    dataTableTemp.Columns.Remove(dataTableTemp.Columns[i].ColumnName);
                }
                else
                {
                    i++;
                }
            }

            // dataTablePivot là DataTable hoàn chỉnh( Đã thêm cột SLTK và SLCLK)
            DataTable dataTablePivot = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(dataTableTemp));

            return dataTablePivot;
        }

        private DataTable CheckSLKho(DataTable tblSLKHFocused, DataTable tblSLKho)
        {
            bool check = true;
            DataRow dataRowSLKHFocused = tblSLKHFocused.Rows[0];
            indexKho = -1;
            foreach (DataRow row in tblSLKho.Rows)
            {
                indexKho += 1;
                check = true;
                foreach (DataColumn column in tblSLKho.Columns)
                {
                    if (column.ColumnName.Equals("MaMau") || column.ColumnName.Equals("DauSize"))
                    {
                        Console.WriteLine("DataColumn");
                        if (!dataRowSLKHFocused[column.ColumnName].Equals(row[column.ColumnName]))
                        {
                            check = false;
                            break;
                        }
                    }
                }

                if (check)
                {
                    DataTable tblKho = row.Table.Clone();
                    tblKho.ImportRow(row);
                    return tblKho;
                }
            }
            return null;
            //foreach (DataColumn column in tblSLKho.Columns)
            //{
            //    check = true;
            //    if (column.ColumnName.Equals("PO")|| column.ColumnName.Equals("MaMau")|| column.ColumnName.Equals("DauSize"))
            //    {
            //        foreach (DataRow row in tblSLKho.Rows)
            //        {
            //            Console.WriteLine("DataColumn");
            //            if (!dataRowSLKHFocused[column.ColumnName].Equals(row[column.ColumnName]))
            //            {
            //                check = false;
            //                break;
            //            }
            //        }
            //    }
            //    //if (!check)
            //    //{
            //    //    break;
            //    //}
            //}
            //return check;
        }
        private void bandedGridviewSoLuong_KHSX_DoubleClick(object sender, EventArgs e)
        {
            if (_allowEdit)
            {    // Giá trị SLKH
                DataTable focusedSLKH = new DataTable();
                DataRow focusedDataRowSLKH = (bandedGridViewDanhSachSoLuong_KHSX.GetFocusedRow() as DataRowView).Row;
                focusedSLKH = focusedDataRowSLKH.Table.Clone();
                //dataTable.ImportRow(existingDataRow);
                focusedSLKH.ImportRow(focusedDataRowSLKH);

                // Gía trị sSLKH default
                //DataTable focusedSLKHDefault = RemoveNumberOfSize(focusedSLKH);
                DataTable focusedSLKHDefault = focusedDataRowSLKH.Table.Clone();
                focusedSLKHDefault.ImportRow(focusedDataRowSLKH);
                focusedSLKHDefault = RemoveNumberOfSize(focusedSLKHDefault);
                DataTable tblKho = CheckSLKho(focusedSLKH, tblSLKho);
                if (tblKho != null)
                {
                    NhapTruTonKho nhapTTK = new NhapTruTonKho(focusedSLKH, focusedSLKHDefault, tblKho);
                    nhapTTK.StartPosition = FormStartPosition.CenterScreen;
                    nhapTTK.DataClosed += frmNhapTruTonKho_Closed;
                    nhapTTK.ShowInTaskbar = false;
                    nhapTTK.ShowDialog();
                }
                else
                {
                    //clsCommonBS.ConfirmError("Đơn hàng hiện tại không còn trong kho.\n Vui lòng chọn đơn hàng khác!");
                    MessageBox.Show("Đơn hàng hiện tại không còn trong kho.\n Vui lòng chọn đơn hàng khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void bandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            if (e.Band.Name.Contains("gbSize@_"))
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

        private void frmNhapTruTonKho_Closed(object sender, List<DataTable> result)
        {
            Console.WriteLine("frmNhapTruTonKho_Closed");


            // New
            // Tạo một List<DataTable> lstResult với 3 phần tử
            // item1: DataTable lưu số lượng nhập của từng size
            // item2: DataTable lưu số lượng tồn kho của từng size
            // item3: DataTable lưu số lượng SX chưa cân đối của từng size

            int focusedRow = -1;
            // Cập nhật giá trị cho bandedGridViewDanhSachSoLuong_KHSX
            // gridControlDanhSachSoLuong_KHSX
            List<string> lstSizeSave = new List<string>();
            List<DataRow> lstDataRowSaveSLKH = new List<DataRow>();
            foreach (DataRow row in tblSLKH.Rows)
            {
                focusedRow += 1;
                if (focusedRow == bandedGridViewDanhSachSoLuong_KHSX.FocusedRowHandle)
                {
                    foreach (DataColumn column in tblSLKH.Columns)
                    {
                        if (column.ColumnName.Contains("Size_SLSX@"))
                        {
                            // Old
                            //string nameOfSize = column.ColumnName.Split('@')[1];
                            //int indexOfSize = result[0].IndexOf(nameOfSize);

                            //// Cập nhật giá trị cho DataRow
                            //row[column.ColumnName] = (int.Parse(row[column.ColumnName].ToString()) - int.Parse(result[1][indexOfSize]));


                            //if (int.Parse(result[1][indexOfSize]) != 0)
                            //{
                            //    lstSizeSave.Add(nameOfSize);
                            //    lstDataRowSaveSLKH.Add(row);
                            //}

                            // New
                            DataTable tblNhapTru = result[0];
                            DataRow dataRowNhapTru = tblNhapTru.Rows[0];
                            string colSizeNhapTru = column.ColumnName.Replace("_SLSX", "");
                            // Cập nhật giá trị cho DataRow
                            row[column.ColumnName] = (int.Parse(row[column.ColumnName].ToString()) - int.Parse(dataRowNhapTru[colSizeNhapTru].ToString()));


                            if (int.Parse(dataRowNhapTru[colSizeNhapTru].ToString()) != 0)
                            {
                                string sizeID = colSizeNhapTru.Split('@')[1];
                                lstSizeSave.Add(sizeID);
                                lstDataRowSaveSLKH.Add(row);
                            }
                        }
                    }
                }
            }

            SaveSLKH(lstDataRowSaveSLKH, lstSizeSave);

            gridControlDanhSachSoLuong_KHSX.DataSource = tblSLKH;


            // Cập nhật giá trị cho bandedGridViewDanhSachSoLuongTonKho
            // gridControlDanhSachSoLuongTonKho
            focusedRow = -1;
            List<DataRow> lstDataRowSaveSLKho = new List<DataRow>();
            foreach (DataRow row in tblSLKho.Rows)
            {
                focusedRow += 1;
                if (focusedRow == indexKho)
                {
                    foreach (DataColumn column in tblSLKho.Columns)
                    {
                        if (column.ColumnName.Contains("Size_SLCLK@"))
                        {
                            // Old
                            //string nameOfSize = column.ColumnName.Split('@')[1];
                            //int indexOfSize = result[0].IndexOf(nameOfSize);

                            //// Cập nhật giá trị cho DataRow
                            //row[column.ColumnName] = (int.Parse(row[column.ColumnName].ToString()) - int.Parse(result[1][indexOfSize]));
                            //if (int.Parse(result[1][indexOfSize]) != 0)
                            //{
                            //    lstSizeSave.Add(nameOfSize);
                            //    lstDataRowSaveSLKho.Add(row);
                            //}

                            // New
                            DataTable tblNhapTru = result[0];
                            DataRow dataRowNhapTru = tblNhapTru.Rows[0];
                            string colSizeNhapTru = column.ColumnName.Replace("_SLCLK", "");

                            //string nameOfSize = column.ColumnName.Split('@')[1];
                            //int indexOfSize = result[0].IndexOf(nameOfSize);

                            // Cập nhật giá trị cho DataRow
                            row[column.ColumnName] = (int.Parse(row[column.ColumnName].ToString()) - int.Parse(dataRowNhapTru[colSizeNhapTru].ToString()));
                            if (int.Parse(dataRowNhapTru[colSizeNhapTru].ToString()) != 0)
                            {
                                // lstSizeSave.Add(nameOfSize);
                                lstDataRowSaveSLKho.Add(row);
                            }
                        }
                    }
                }
            }


            //SaveSLKho(lstDataRowSaveSLKho, lstSizeSave);
            gridControlDanhSachSoLuongTonKho.DataSource = tblSLKho;
        }

        // lstDataRowSave là những DataRow có thay đổi dữ liệu
        // lstSizeSave là những Size trong DataRow có thay đổi dữ liệu => Lấy ra số lượng thay đổi của từng size => Size nào có thay đổi số lượng mới lưu size đó
        private void SaveSLKH(List<DataRow> lstDataRowSave, List<string> lstSizeSave)
        {
            if (lstDataRowSave.Count > 0 && lstSizeSave.Count > 0)
            {
                DataTable tblSLKH = lstDataRowSave[0].Table.Clone();

                foreach (DataRow row in lstDataRowSave)
                {
                    tblSLKH.ImportRow(row);
                }
                DataTable tblUnPivot = UnPivotSLKH(tblSLKH);
                DataTable tblSLKHSave = tblUnPivot.Clone();
                Console.WriteLine("unpivot");

                // Tìm Size thay đổi giá trị để update dữ liệu
                foreach (DataRow row in tblUnPivot.Rows)
                {
                    foreach (DataColumn column in tblUnPivot.Columns)
                    {
                        if (column.ColumnName.Equals("SizeID"))
                        {
                            //if(row[column.ColumnName].ToString())
                            if (lstSizeSave.Contains(row[column.ColumnName].ToString()))
                            {
                                row["CheckTruTK"] = 1;
                                tblSLKHSave.ImportRow(row);
                            }
                            //string size = lstSizeSave.Where(item => item.Equals(row[column.ColumnName].ToString())).ToList().First();
                            //if (!string.IsNullOrEmpty(size))
                            //{

                            //}
                        }
                    }
                }
                Console.WriteLine("tblSLKHSave");
                if (tblSLKHSave.Rows.Count > 0)
                {
                    string url = string.Format("{0}", URL + "DonHangTong/UpdateSLDonHangTongPOChiTiet");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSLKHSave); }).Result;
                }
            }
        }
        private DataTable UnPivotSLKH(DataTable tblSLKH)
        {
            DataTable tblSave = createTableSLKH();
            if (tblSLKH == null || tblSLKH.Rows.Count == 0) return null;
            foreach (DataRow dataRows in tblSLKH.Rows)
            {
                // Old
                //int j = 0;
                //for (int i = 10; i < dataRows.ItemArray.Length; i += 2)
                //{

                //    if (i % 2 == 0)
                //    {
                //        Console.WriteLine("dataRows");
                //        //tblSave[""]
                //        DataRow dataRowSave = tblSave.NewRow();
                //        dataRowSave["PO"] = dataRows["PO"];
                //        dataRowSave["DauSize"] = dataRows["DauSize"];
                //        dataRowSave["MaMau"] = dataRows["MaMau"];
                //        dataRowSave["POID"] = dataRows["POID"];
                //        dataRowSave["GhiChu"] = dataRows["GhiChu"];
                //        dataRowSave["DauSizeID"] = dataRows["DauSizeID"];
                //        dataRowSave["Size"] = lstSize[j];
                //        dataRowSave["SoLuong"] = dataRows.ItemArray[i];
                //        dataRowSave["SoLuongSX"] = dataRows.ItemArray[i + 1];
                //        //dataRowSave["CheckTruTK"] = 1;
                //        tblSave.Rows.Add(dataRowSave);
                //        j += 1;
                //    }
                //}

                // New
                
                foreach (DataColumn dataColumn in tblSLKH.Columns)
                {

                    if (dataColumn.ColumnName.Contains("Size_SLKH"))
                    {
                        Console.WriteLine("dataRows");
                        //tblSave[""]
                        DataRow dataRowSave = tblSave.NewRow();
                        dataRowSave["PO"] = dataRows["PO"];
                        dataRowSave["DauSize"] = dataRows["DauSize"];
                        dataRowSave["MaMau"] = dataRows["MaMau"];
                        dataRowSave["POID"] = dataRows["POID"];
                        dataRowSave["GhiChu"] = dataRows["GhiChu"];
                        dataRowSave["DauSizeID"] = dataRows["DauSizeID"];
                        string sizeID = dataColumn.ColumnName.Split('@')[1];
                        string size = dataColumn.ColumnName.Split('@')[2];
                        dataRowSave["SizeID"] = sizeID;
                        dataRowSave["Size"] = size;
                        dataRowSave["SoLuong"] = dataRows[dataColumn.ColumnName];
                        int indexOfSLKH = tblSLKH.Columns.IndexOf(dataColumn);
                        dataRowSave["SoLuongSX"] = dataRows.ItemArray[indexOfSLKH + 1];
                        //dataRowSave["CheckTruTK"] = 1;
                        tblSave.Rows.Add(dataRowSave);
                    }
                }
            }
            Console.WriteLine("end for");
            return tblSave;
        }


        // Không updateSLTonKho nữa
        private void SaveSLKho(List<DataRow> lstDataRowSave, List<string> lstSizeSave)
        {
            try
            {
                if (lstDataRowSave.Count > 0 && lstSizeSave.Count > 0)
                {
                    DataTable tblSLKho = lstDataRowSave[0].Table.Clone();

                    foreach (DataRow row in lstDataRowSave)
                    {
                        tblSLKho.ImportRow(row);
                    }
                    DataTable tblUnPivot = UnPivotSLKho(tblSLKho);
                    DataTable tblSLKhoSave = tblUnPivot.Clone();
                    Console.WriteLine("unpivot");

                    // Tìm Size thay đổi giá trị để update dữ liệu
                    foreach (DataRow row in tblUnPivot.Rows)
                    {
                        foreach (DataColumn column in tblUnPivot.Columns)
                        {
                            if (column.ColumnName.Equals("Size"))
                            {
                                //if(row[column.ColumnName].ToString())
                                if (lstSizeSave.Contains(row[column.ColumnName].ToString()))
                                {
                                    tblSLKhoSave.ImportRow(row);
                                }
                            }
                        }
                    }
                    Console.WriteLine("tblSLKhoSave");
                    if (tblSLKhoSave.Rows.Count > 0)
                    {
                        string url = string.Format("{0}", URL + "DonHangTonKho/UpdateSLTonKho");
                        string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSLKhoSave); }).Result;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataTable UnPivotSLKho(DataTable tblSLKho)
        {
            DataTable tblSave = createTableSLKho();
            if (tblSLKho == null || tblSLKho.Rows.Count == 0) return null;
            foreach (DataRow dataRows in tblSLKho.Rows)
            {
                int j = 0;
                for (int i = 8; i < dataRows.ItemArray.Length; i += 2)
                {

                    if (i % 2 == 0)
                    {
                        Console.WriteLine("dataRows");
                        //tblSave[""]
                        DataRow dataRowSave = tblSave.NewRow();
                        //dataRowSave["ID"]
                        dataRowSave["MaHang"] = dataRows["MaHang"];
                        dataRowSave["MaMau"] = dataRows["MaMau"];
                        dataRowSave["PO"] = dataRows["PO"];
                        dataRowSave["DauSizeID"] = dataRows["DauSizeID"];
                        dataRowSave["DauSize"] = dataRows["DauSize"];
                        dataRowSave["Size"] = lstSize[j];
                        dataRowSave["SoLuong"] = dataRows.ItemArray[i + 1];
                        dataRowSave["TrangThai"] = 0;
                        //dataRowSave["GhiChu"] = dataRows["GhiChu"];
                        tblSave.Rows.Add(dataRowSave);
                        j += 1;
                    }
                }
            }
            Console.WriteLine("end for");
            return tblSave;
        }

        private void BandedView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }

        private void bandedGridviewSoLuong_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

        private void bandedGridviewSoLuong_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private BandedGridView createColBandsSizeTTK(DataTable _thongTinDonHang)
        {
            BandedGridView bandedGridview = new BandedGridView();
            //bandedGridview = bandedGridViewDanhSachTruTonKho;
            try
            {
                if (lstSize != null && lstSize.Count > 0)
                {
                    lstSize.Clear();
                }
                gridBandSize.Children.Clear();
                bandedGridview.Columns.Clear();
                foreach (DataColumn dc in _thongTinDonHang.Columns)
                {
                    if (dc.ColumnName.Contains("Size@"))
                    {
                        string colName = dc.ColumnName;
                        lstSize.Add(colName.Replace("Size@", ""));
                        //colName.Replace("@Size", "");
                        BandedGridColumn col = new BandedGridColumn();
                        col.AppearanceHeader.Options.UseTextOptions = true;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.Caption = colName.Replace("Size@", "");
                        col.FieldName = dc.ColumnName;
                        col.Name = "col" + colName;
                        col.OptionsColumn.AllowEdit = true;
                        col.Visible = true;
                        col.Width = 50;
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        col.DisplayFormat.FormatString = "{0:##,0}";
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { col });
                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gb.AppearanceHeader.Options.UseFont = true;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        //gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(239)))), ((int)(((byte)(230)))));
                        gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(252)))), ((int)(((byte)(182)))));
                        gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gb.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        gb.AppearanceHeader.Options.UseBackColor = true;
                        gb.AppearanceHeader.Options.UseForeColor = true;
                        gb.Caption = colName.Replace("Size@", "");
                        gb.Columns.Add(col);
                        gb.Name = "gb" + "Size_" + col;
                        gb.VisibleIndex = 0;
                        gb.Width = 60;
                        gridBandSize_TTK.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "poid")
                    {
                        BandedGridColumn colPoID = new BandedGridColumn();
                        colPoID.AppearanceHeader.Options.UseTextOptions = true;
                        colPoID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPoID.Caption = dc.ColumnName.Replace("Size@", "");
                        colPoID.FieldName = dc.ColumnName;
                        colPoID.Name = "col" + dc.ColumnName;
                        colPoID.OptionsColumn.AllowEdit = false;
                        colPoID.Visible = true;
                        colPoID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPoID });
                        gridBandPOID_TTK.Columns.Add(colPoID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "po")
                    {
                        BandedGridColumn colPo = new BandedGridColumn();
                        colPo.AppearanceHeader.Options.UseTextOptions = true;
                        colPo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPo.Caption = dc.ColumnName.Replace("Size@", "");
                        colPo.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colPo.FieldName = dc.ColumnName;
                        colPo.Name = "col" + dc.ColumnName;
                        colPo.OptionsColumn.AllowEdit = false;
                        colPo.Visible = true;
                        colPo.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPo });
                        gridBandPO_TTK.Columns.Add(colPo);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "mamau")
                    {
                        BandedGridColumn colMaMau = new BandedGridColumn();
                        colMaMau.AppearanceHeader.Options.UseTextOptions = true;
                        colMaMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaMau.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaMau.FieldName = dc.ColumnName;
                        colMaMau.Name = "col" + dc.ColumnName;
                        colMaMau.OptionsColumn.AllowEdit = true;
                        colMaMau.Visible = true;
                        colMaMau.Width = 50;
                        //colMaMau.ColumnEdit = searchLookUpEditMau1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaMau });
                        gridBandMaMau_TTK.Columns.Add(colMaMau);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausize")
                    {
                        BandedGridColumn colDauSize = new BandedGridColumn();
                        colDauSize.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSize.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSize.FieldName = dc.ColumnName;
                        colDauSize.Name = "col" + dc.ColumnName;
                        colDauSize.OptionsColumn.AllowEdit = true;
                        colDauSize.Visible = true;
                        colDauSize.Width = 125;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSize });
                        gridBandDauSize_TTK.Columns.Add(colDauSize);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausizeid")
                    {
                        BandedGridColumn colDauSizeID = new BandedGridColumn();
                        colDauSizeID.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSizeID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSizeID.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSizeID.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSizeID.FieldName = dc.ColumnName;
                        colDauSizeID.Name = "col" + dc.ColumnName;
                        colDauSizeID.OptionsColumn.AllowEdit = true;
                        colDauSizeID.Visible = true;
                        colDauSizeID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSizeID });
                        gridBandDauSizeID_TTK.Columns.Add(colDauSizeID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "madh")
                    {
                        BandedGridColumn colMaDH = new BandedGridColumn();
                        colMaDH.AppearanceHeader.Options.UseTextOptions = true;
                        colMaDH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaDH.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaDH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaDH.FieldName = dc.ColumnName;
                        colMaDH.Name = "col" + dc.ColumnName;
                        colMaDH.OptionsColumn.AllowEdit = false;
                        colMaDH.Visible = true;
                        colMaDH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaDH });
                        gridBandMaDH_TTK.Columns.Add(colMaDH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "maqg")
                    {
                        BandedGridColumn colMaQG = new BandedGridColumn();
                        colMaQG.AppearanceHeader.Options.UseTextOptions = true;
                        colMaQG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaQG.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaQG.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaQG.FieldName = dc.ColumnName;
                        colMaQG.Name = "col" + dc.ColumnName;
                        colMaQG.OptionsColumn.AllowEdit = true;
                        colMaQG.Visible = true;
                        colMaQG.Width = 50;
                        //colMaQG.ColumnEdit = searchLookUpEditQuocGia1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaQG });
                        gridBandMaQuocGia_TTK.Columns.Add(colMaQG);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ngaygh")
                    {
                        BandedGridColumn colNgayGH = new BandedGridColumn();
                        colNgayGH.AppearanceHeader.Options.UseTextOptions = true;
                        colNgayGH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colNgayGH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                        colNgayGH.DisplayFormat.FormatString = "dd/MM/yyyy";
                        colNgayGH.Caption = dc.ColumnName.Replace("Size@", "");
                        colNgayGH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colNgayGH.OptionsColumn.AllowEdit = true;
                        colNgayGH.FieldName = dc.ColumnName;
                        colNgayGH.Name = "col" + dc.ColumnName;
                        colNgayGH.Visible = true;
                        colNgayGH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colNgayGH });
                        gridBandNgayGH_TTK.Columns.Add(colNgayGH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ghichu")
                    {
                        BandedGridColumn colGhiChu = new BandedGridColumn();
                        colGhiChu.AppearanceHeader.Options.UseTextOptions = true;
                        colGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colGhiChu.Caption = dc.ColumnName.Replace("Size@", "");
                        colGhiChu.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colGhiChu.FieldName = dc.ColumnName;
                        colGhiChu.Name = "col" + dc.ColumnName;
                        colGhiChu.OptionsColumn.AllowEdit = true;
                        colGhiChu.Visible = true;
                        colGhiChu.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colGhiChu });
                        gridBandGhiChu_TTK.Columns.Add(colGhiChu);
                    }
                }
                return bandedGridview;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return new BandedGridView();
        }

        private BandedGridView createColBandsSizeSLKeHoach(DataTable _thongTinDonHang)
        {
            BandedGridView bandedGridview = new BandedGridView();

            bandedGridview = bandedGridViewDanhSachSoLuong_KHSX;
            bandedGridview.CustomDrawFooter += bandedGridviewSoLuong_CustomDrawFooter;
            try
            {
                if (lstSize != null && lstSize.Count > 0)
                {
                    lstSize.Clear();
                }
                gridBandSize.Children.Clear();
                bandedGridview.Columns.Clear();


                // Thêm cột STT
                BandedGridColumn colSTT = (BandedGridColumn)bandedGridview.Columns.AddField("STT");

                colSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                colSTT.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                colSTT.AppearanceHeader.Options.UseBackColor = true;
                colSTT.AppearanceHeader.Options.UseFont = true;
                colSTT.OptionsColumn.AllowEdit = false;
                colSTT.Caption = "STT";
                colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                //colAmount.UnboundExpression = exp;
                colSTT.Visible = true;
                colSTT.OwnerBand = this.gridBandSTTKHSX;
                colSTT.AppearanceCell.Options.UseTextOptions = true;
                colSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                colSTT.Width = 45;
                colSTT.MinWidth = 45;

                //bandedGridview.Bands.Add(gridBandSTT);

                foreach (DataColumn dc in _thongTinDonHang.Columns)
                {
                    if (dc.ColumnName.Contains("Size@"))
                    {

                        string colName = dc.ColumnName;
                        lstSize.Add(colName.Replace("Size@", ""));

                        string[] colNameSplit = colName.Split('@');
                        //string fieldNameSLKH = string.Format("Size_SLKH@{0}", colNameSplit[1]);
                        //string fieldNameSLSX = string.Format("Size_SLSX@{0}", colNameSplit[1]);

                        string fieldNameSLKH = string.Format("Size_SLKH@{0}@{1}", colNameSplit[1], colNameSplit[2]);
                        string fieldNameSLSX = string.Format("Size_SLSX@{0}@{1}", colNameSplit[1], colNameSplit[2]);


                        //lstSize.Add(colName.Split('@')[1]);
                        //colName.Replace("@Size", "");
                        BandedGridColumn colSLKH = new BandedGridColumn();
                        colSLKH.AppearanceCell.Options.UseTextOptions = true;
                        colSLKH.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colSLKH.AppearanceCell.BackColor = System.Drawing.Color.White;
                        colSLKH.AppearanceCell.Options.UseBackColor = true;
                        colSLKH.AppearanceHeader.Options.UseTextOptions = true;
                        colSLKH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colSLKH.AppearanceCell.ForeColor = System.Drawing.ColorTranslator.FromHtml("#282b30");
                        colSLKH.AppearanceCell.Options.UseForeColor = true;
                        colSLKH.FieldName = fieldNameSLKH;
                        colSLKH.Name = "col_size_slkh" + colName;
                        colSLKH.OptionsColumn.AllowEdit = false;
                        colSLKH.Visible = true;
                        colSLKH.Width = 50;
                        colSLKH.Summary.Add(DevExpress.Data.SummaryItemType.Sum, colSLKH.FieldName, "{0:n0}");
                        colSLKH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        colSLKH.DisplayFormat.FormatString = "{0:##,0}";
                        colSLKH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                        GridBand gbSLKH = new GridBand();
                        gbSLKH.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gbSLKH.AppearanceHeader.Options.UseFont = true;
                        gbSLKH.AppearanceHeader.Options.UseTextOptions = true;
                        gbSLKH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        //gbSLKH.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(212)))), ((int)(((byte)(135)))));
                        gbSLKH.AppearanceHeader.BackColor = System.Drawing.Color.White;
                        gbSLKH.AppearanceHeader.Options.UseBackColor = true;
                        gbSLKH.AppearanceHeader.BackColor2 = System.Drawing.Color.White;

                        gbSLKH.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        //gbSLKH.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        //gbSLKH.AppearanceHeader.Options.UseForeColor = true;

                        //gbSLKH.Caption = colName.Replace("Size@", "");
                        gbSLKH.Caption = "SLKH";
                        gbSLKH.Columns.Add(colSLKH);
                        gbSLKH.Name = "gbSize_SLKH" + colSLKH;
                        gbSLKH.VisibleIndex = 0;
                        gbSLKH.Width = 60;

                        //string colNameSLSX = columnSLSX.ColumnName;
                        BandedGridColumn colSLSX = new BandedGridColumn();
                        colSLSX.AppearanceCell.Options.UseTextOptions = true;
                        colSLSX.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //colSLSX.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
                        colSLSX.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(221)))), ((int)(((byte)(255)))));
                        colSLSX.AppearanceCell.ForeColor = System.Drawing.ColorTranslator.FromHtml("#402d2d");
                        //colSLSX.AppearanceCell.Options.UseForeColor = true;
                        colSLSX.AppearanceCell.Options.UseBackColor = true;
                        colSLSX.AppearanceHeader.Options.UseTextOptions = true;
                        colSLSX.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //colSLSX.Caption = colNameSLSX.Split('@')[1];
                        colSLSX.FieldName = fieldNameSLSX;
                        colSLSX.Name = "col_size_slsx" + colName;
                        colSLSX.OptionsColumn.AllowEdit = false;
                        colSLSX.Visible = true;
                        colSLSX.Width = 50;
                        colSLSX.Summary.Add(DevExpress.Data.SummaryItemType.Sum, colSLSX.FieldName, "{0:n0}");
                        colSLSX.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        colSLSX.DisplayFormat.FormatString = "{0:##,0}";
                        colSLSX.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        //
                        GridBand gbSLSX = new GridBand();
                        gbSLSX.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gbSLSX.AppearanceHeader.Options.UseFont = true;
                        gbSLSX.AppearanceHeader.Options.UseTextOptions = true;
                        gbSLSX.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        gbSLSX.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(94)))), ((int)(((byte)(171)))));
                        gbSLSX.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gbSLSX.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        gbSLSX.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        gbSLSX.AppearanceHeader.Options.UseBackColor = true;
                        gbSLSX.AppearanceHeader.Options.UseForeColor = true;
                        //gbSLKH.Caption = colName.Replace("Size@", "");
                        gbSLSX.Caption = "SLSX";
                        gbSLSX.Columns.Add(colSLSX);
                        gbSLSX.Name = "gbSize_SLSX" + colSLSX;
                        gbSLSX.VisibleIndex = 0;
                        gbSLSX.Width = 60;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colSLKH, colSLSX });
                        //
                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gb.AppearanceHeader.Options.UseFont = true;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        // 255, 212, 128 => Màu cam dùng cho header
                        gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                        //gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(252)))), ((int)(((byte)(182)))));
                        gb.AppearanceHeader.Options.UseBackColor = true;
                        gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
                        string size = colName.Split('@')[2];
                        gb.Caption = size;
                        //gb.Caption = colNameSLSX.Split('@')[1];
                        //gb.Columns.Add(col);
                        gb.Name = string.Format("gbSize@_{0}", colName.Replace("Size@", ""));
                        gb.VisibleIndex = 0;
                        gb.Width = 60;
                        gb.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gbSLKH, gbSLSX });
                        //gb.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gbSLSX });
                        gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "poid")
                    {
                        BandedGridColumn colPoID = new BandedGridColumn();
                        colPoID.AppearanceHeader.Options.UseTextOptions = true;
                        colPoID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPoID.Caption = dc.ColumnName.Replace("Size@", "");
                        colPoID.FieldName = dc.ColumnName;
                        colPoID.Name = "col" + dc.ColumnName;
                        colPoID.OptionsColumn.AllowEdit = false;
                        colPoID.Visible = true;
                        colPoID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPoID });
                        gridBandPOID.Columns.Add(colPoID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "po")
                    {
                        BandedGridColumn colPo = new BandedGridColumn();
                        colPo.AppearanceHeader.Options.UseTextOptions = true;
                        colPo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPo.Caption = dc.ColumnName.Replace("Size@", "");
                        colPo.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colPo.FieldName = dc.ColumnName;
                        colPo.Name = "col" + dc.ColumnName;
                        colPo.OptionsColumn.AllowEdit = false;
                        colPo.Visible = true;
                        colPo.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPo });
                        gridBandPO.Columns.Add(colPo);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "mamau")
                    {
                        BandedGridColumn colMaMau = new BandedGridColumn();
                        colMaMau.AppearanceHeader.Options.UseTextOptions = true;
                        colMaMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaMau.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaMau.FieldName = dc.ColumnName;
                        colMaMau.Name = "col" + dc.ColumnName;
                        colMaMau.OptionsColumn.AllowEdit = false;
                        colMaMau.Visible = true;
                        colMaMau.Width = 50;
                        //colMaMau.ColumnEdit = searchLookUpEditMau1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaMau });
                        gridBandMaMau.Columns.Add(colMaMau);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "tenmau")
                    {
                        BandedGridColumn colTenMau = new BandedGridColumn();
                        colTenMau.AppearanceHeader.Options.UseTextOptions = true;
                        colTenMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //colTenMau.Caption = dc.ColumnName.Replace("Size@", "");
                        colTenMau.Caption = "123";
                        colTenMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colTenMau.FieldName = dc.ColumnName;
                        colTenMau.Name = "col" + dc.ColumnName;
                        colTenMau.OptionsColumn.AllowEdit = false;
                        colTenMau.Visible = true;
                        colTenMau.Width = 50;
                        //colMaMau.ColumnEdit = searchLookUpEditMau1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colTenMau });
                        gridBandTenMau.Columns.Add(colTenMau);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausize")
                    {
                        BandedGridColumn colDauSize = new BandedGridColumn();
                        colDauSize.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSize.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSize.FieldName = dc.ColumnName;
                        colDauSize.Name = "col" + dc.ColumnName;
                        colDauSize.OptionsColumn.AllowEdit = false;
                        colDauSize.Visible = true;
                        colDauSize.Width = 125;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSize });
                        gridBandDauSize.Columns.Add(colDauSize);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausizeid")
                    {
                        BandedGridColumn colDauSizeID = new BandedGridColumn();
                        colDauSizeID.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSizeID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSizeID.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSizeID.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSizeID.FieldName = dc.ColumnName;
                        colDauSizeID.Name = "col" + dc.ColumnName;
                        colDauSizeID.OptionsColumn.AllowEdit = false;
                        colDauSizeID.Visible = true;
                        colDauSizeID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSizeID });
                        gridBandDauSizeID.Columns.Add(colDauSizeID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "madh")
                    {
                        BandedGridColumn colMaDH = new BandedGridColumn();
                        colMaDH.AppearanceHeader.Options.UseTextOptions = true;
                        colMaDH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaDH.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaDH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaDH.FieldName = dc.ColumnName;
                        colMaDH.Name = "col" + dc.ColumnName;
                        colMaDH.OptionsColumn.AllowEdit = false;
                        colMaDH.Visible = true;
                        colMaDH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaDH });
                        gridBandMaDH.Columns.Add(colMaDH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "maqg")
                    {
                        BandedGridColumn colMaQG = new BandedGridColumn();
                        colMaQG.AppearanceHeader.Options.UseTextOptions = true;
                        colMaQG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaQG.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaQG.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaQG.FieldName = dc.ColumnName;
                        colMaQG.Name = "col" + dc.ColumnName;
                        colMaQG.OptionsColumn.AllowEdit = false;
                        colMaQG.Visible = true;
                        colMaQG.Width = 50;
                        //colMaQG.ColumnEdit = searchLookUpEditQuocGia1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaQG });
                        gridBandMaQuocGia.Columns.Add(colMaQG);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ngaygh")
                    {
                        BandedGridColumn colNgayGH = new BandedGridColumn();
                        colNgayGH.AppearanceHeader.Options.UseTextOptions = true;
                        colNgayGH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colNgayGH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                        colNgayGH.DisplayFormat.FormatString = "dd/MM/yyyy";
                        colNgayGH.Caption = dc.ColumnName.Replace("Size@", "");
                        colNgayGH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colNgayGH.OptionsColumn.AllowEdit = false;
                        colNgayGH.FieldName = dc.ColumnName;
                        colNgayGH.Name = "col" + dc.ColumnName;
                        colNgayGH.Visible = true;
                        colNgayGH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colNgayGH });
                        gridBandNgayGH.Columns.Add(colNgayGH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ghichu")
                    {
                        BandedGridColumn colGhiChu = new BandedGridColumn();
                        colGhiChu.AppearanceHeader.Options.UseTextOptions = true;
                        colGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colGhiChu.Caption = dc.ColumnName.Replace("Size@", "");
                        colGhiChu.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colGhiChu.FieldName = dc.ColumnName;
                        colGhiChu.Name = "col" + dc.ColumnName;
                        colGhiChu.OptionsColumn.AllowEdit = false;
                        colGhiChu.Visible = true;
                        colGhiChu.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colGhiChu });
                        gridBandGhiChu.Columns.Add(colGhiChu);
                    }
                }

                return bandedGridview;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return new BandedGridView();
        }

        private BandedGridView createColBandsSizeSLTonKho(DataTable _thongTinDonHang)
        {
            BandedGridView bandedGridview = new BandedGridView();
            bandedGridview = bandedGridViewDanhSachSoLuongTonKho;
            bandedGridview.CustomDrawFooter += bandedGridviewSoLuong_CustomDrawFooter;
            bandedGridview.CustomDrawFooterCell += this.bandedGridviewSoLuong_CustomDrawFooterCell;
            try
            {
                if (lstSize != null && lstSize.Count > 0)
                {
                    lstSize.Clear();
                }
                gridBandSize_Kho.Children.Clear();
                bandedGridview.Columns.Clear();

                // Thêm cột STT
                BandedGridColumn colSTT = (BandedGridColumn)bandedGridview.Columns.AddField("STT");

                colSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                colSTT.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                colSTT.AppearanceHeader.Options.UseBackColor = true;
                colSTT.AppearanceHeader.Options.UseFont = true;
                colSTT.OptionsColumn.AllowEdit = false;
                colSTT.Caption = "STT";
                colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                //colAmount.UnboundExpression = exp;
                colSTT.Visible = true;
                colSTT.OwnerBand = this.gridBandSTT_Kho;
                colSTT.AppearanceCell.Options.UseTextOptions = true;
                colSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                colSTT.Width = 45;
                colSTT.MinWidth = 45;

                foreach (DataColumn dc in _thongTinDonHang.Columns)
                {
                    if (dc.ColumnName.Contains("Size@"))
                    {
                        string colName = dc.ColumnName;
                        lstSize.Add(colName.Replace("Size@", ""));
                        //

                        string[] colNameSplit = colName.Split('@');
                        string fieldNameSLTK = string.Format("Size_SLTK@{0}@{1}", colNameSplit[1], colNameSplit[2]);
                        string fieldNameSLCLK = string.Format("Size_SLCLK@{0}@{1}", colNameSplit[1], colNameSplit[2]);
                        //
                        //colName.Replace("@Size", "");
                        BandedGridColumn colSLTK = new BandedGridColumn();
                        colSLTK.AppearanceHeader.Options.UseTextOptions = true;
                        colSLTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colSLTK.FieldName = fieldNameSLTK;
                        colSLTK.Name = "col_size_sltk" + colName;
                        colSLTK.OptionsColumn.AllowEdit = false;
                        colSLTK.Visible = true;
                        colSLTK.Width = 50;
                        colSLTK.Summary.Add(DevExpress.Data.SummaryItemType.Sum, colSLTK.FieldName, "{0:n0}");
                        colSLTK.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        colSLTK.DisplayFormat.FormatString = "{0:##,0}";
                        colSLTK.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                        colSLTK.AppearanceCell.Options.UseTextOptions = true;
                        colSLTK.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colSLTK.AppearanceCell.BackColor = System.Drawing.Color.White;
                        colSLTK.AppearanceCell.Options.UseBackColor = true;
                        colSLTK.AppearanceCell.ForeColor = System.Drawing.ColorTranslator.FromHtml("#282b30");
                        colSLTK.AppearanceCell.Options.UseForeColor = true;


                        GridBand gbSLTK = new GridBand();
                        gbSLTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gbSLTK.AppearanceHeader.Options.UseFont = true;
                        gbSLTK.AppearanceHeader.Options.UseTextOptions = true;
                        gbSLTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        //gbSLTK.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(223)))), ((int)(((byte)(253)))));
                        //gbSLTK.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(212)))), ((int)(((byte)(135)))));
                        //gbSLTK.AppearanceHeader.Options.UseBackColor = true;
                        gbSLTK.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gbSLTK.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        gbSLTK.AppearanceHeader.Options.UseForeColor = true;
                        gbSLTK.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;

                        //gbSLKH.Caption = colName.Replace("Size@", "");
                        gbSLTK.Caption = "SLTK";
                        gbSLTK.Columns.Add(colSLTK);
                        gbSLTK.Name = "gbSize_SLTK" + colSLTK;
                        gbSLTK.VisibleIndex = 0;
                        gbSLTK.Width = 60;
                        //
                        BandedGridColumn colSLCLK = new BandedGridColumn();
                        colSLCLK.AppearanceHeader.Options.UseTextOptions = true;
                        colSLCLK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colSLCLK.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(221)))), ((int)(((byte)(255)))));
                        colSLCLK.AppearanceCell.ForeColor = System.Drawing.ColorTranslator.FromHtml("#402d2d");
                        colSLCLK.AppearanceCell.Options.UseForeColor = true;
                        colSLCLK.AppearanceCell.Options.UseBackColor = true;

                        colSLCLK.AppearanceCell.Options.UseTextOptions = true;
                        colSLCLK.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colSLCLK.AppearanceCell.BackColor = System.Drawing.Color.White;
                        colSLCLK.AppearanceCell.Options.UseBackColor = true;
                        colSLCLK.AppearanceCell.ForeColor = System.Drawing.ColorTranslator.FromHtml("#282b30");
                        colSLCLK.AppearanceCell.Options.UseForeColor = true;

                        //colSLCLK.Caption = colName.Replace("Size@", "");
                        colSLCLK.FieldName = fieldNameSLCLK;
                        colSLCLK.Name = "col_size_slclk" + colName;
                        colSLCLK.OptionsColumn.AllowEdit = false;
                        colSLCLK.Visible = true;
                        colSLCLK.Width = 50;
                        colSLCLK.Summary.Add(DevExpress.Data.SummaryItemType.Sum, colSLCLK.FieldName, "{0:n0}");
                        colSLCLK.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        colSLCLK.DisplayFormat.FormatString = "{0:##,0}";
                        colSLCLK.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        //
                        GridBand gbSLCLK = new GridBand();
                        gbSLCLK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gbSLCLK.AppearanceHeader.Options.UseFont = true;
                        gbSLCLK.AppearanceHeader.Options.UseTextOptions = true;
                        gbSLCLK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        //gbSLCLK.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(223)))), ((int)(((byte)(253)))));
                        gbSLCLK.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(94)))), ((int)(((byte)(171)))));
                        gbSLCLK.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gbSLCLK.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        gbSLCLK.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        gbSLCLK.AppearanceHeader.Options.UseBackColor = true;
                        gbSLCLK.AppearanceHeader.Options.UseForeColor = true;
                        //gbSLKH.Caption = colName.Replace("Size@", "");
                        gbSLCLK.Caption = "SLCLK";
                        gbSLCLK.Columns.Add(colSLCLK);
                        gbSLCLK.Name = "gbSize_SLCLK" + colSLCLK;
                        gbSLCLK.VisibleIndex = 0;
                        gbSLCLK.Width = 60;

                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colSLTK, colSLCLK });
                        //
                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gb.AppearanceHeader.Options.UseFont = true;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                        gb.AppearanceHeader.Options.UseBackColor = true;
                        gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
                        gb.AppearanceHeader.Options.UseForeColor = true;
                        string size = colName.Split('@')[2];
                        gb.Caption = size;
                        //gb.Columns.Add(col);
                        gb.Name = "gbSize_SL_Kho" + colSLTK;
                        gb.Name = string.Format("gbSize@_TK_{0}", colName.Replace("Size@", ""));
                        gb.VisibleIndex = 0;
                        gb.Width = 60;
                        gb.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gbSLTK });
                        gb.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gbSLCLK });
                        gridBandSize_Kho.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "poid")
                    {
                        BandedGridColumn colPoID = new BandedGridColumn();
                        colPoID.AppearanceHeader.Options.UseTextOptions = true;
                        colPoID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPoID.Caption = dc.ColumnName.Replace("Size@", "");
                        colPoID.FieldName = dc.ColumnName;
                        colPoID.Name = "col" + dc.ColumnName;
                        colPoID.OptionsColumn.AllowEdit = false;
                        colPoID.Visible = true;
                        colPoID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPoID });
                        gridBandPOID_Kho.Columns.Add(colPoID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "po")
                    {
                        BandedGridColumn colPo = new BandedGridColumn();
                        colPo.AppearanceHeader.Options.UseTextOptions = true;
                        colPo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPo.Caption = dc.ColumnName.Replace("Size@", "");
                        colPo.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colPo.FieldName = dc.ColumnName;
                        colPo.Name = "col" + dc.ColumnName;
                        colPo.OptionsColumn.AllowEdit = false;
                        colPo.Visible = true;
                        colPo.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPo });
                        gridBandPO_Kho.Columns.Add(colPo);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "mamau")
                    {
                        BandedGridColumn colMaMau = new BandedGridColumn();
                        colMaMau.AppearanceHeader.Options.UseTextOptions = true;
                        colMaMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaMau.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaMau.FieldName = dc.ColumnName;
                        colMaMau.Name = "col" + dc.ColumnName;
                        colMaMau.OptionsColumn.AllowEdit = false;
                        colMaMau.Visible = true;
                        colMaMau.Width = 50;
                        //colMaMau.ColumnEdit = searchLookUpEditMau1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaMau });
                        gridBandMaMau_Kho.Columns.Add(colMaMau);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "tenmau")
                    {
                        BandedGridColumn colTenMau = new BandedGridColumn();
                        colTenMau.AppearanceHeader.Options.UseTextOptions = true;
                        colTenMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colTenMau.Caption = dc.ColumnName.Replace("Size@", "");
                        colTenMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colTenMau.FieldName = dc.ColumnName;
                        colTenMau.Name = "col" + dc.ColumnName;
                        colTenMau.OptionsColumn.AllowEdit = false;
                        colTenMau.Visible = true;
                        colTenMau.Width = 50;
                        //colMaMau.ColumnEdit = searchLookUpEditMau1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colTenMau });
                        gridBandTenMau_Kho.Columns.Add(colTenMau);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausize")
                    {
                        BandedGridColumn colDauSize = new BandedGridColumn();
                        colDauSize.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSize.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSize.FieldName = dc.ColumnName;
                        colDauSize.Name = "col" + dc.ColumnName;
                        colDauSize.OptionsColumn.AllowEdit = false;
                        colDauSize.Visible = true;
                        colDauSize.Width = 125;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSize });
                        gridBandDauSize_Kho.Columns.Add(colDauSize);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausizeid")
                    {
                        BandedGridColumn colDauSizeID = new BandedGridColumn();
                        colDauSizeID.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSizeID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSizeID.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSizeID.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSizeID.FieldName = dc.ColumnName;
                        colDauSizeID.Name = "col" + dc.ColumnName;
                        colDauSizeID.OptionsColumn.AllowEdit = false;
                        colDauSizeID.Visible = true;
                        colDauSizeID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSizeID });
                        gridBandDauSizeID_Kho.Columns.Add(colDauSizeID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "madh")
                    {
                        BandedGridColumn colMaDH = new BandedGridColumn();
                        colMaDH.AppearanceHeader.Options.UseTextOptions = true;
                        colMaDH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaDH.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaDH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaDH.FieldName = dc.ColumnName;
                        colMaDH.Name = "col" + dc.ColumnName;
                        colMaDH.OptionsColumn.AllowEdit = false;
                        colMaDH.Visible = true;
                        colMaDH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaDH });
                        gridBandMaDH_Kho.Columns.Add(colMaDH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "maqg")
                    {
                        BandedGridColumn colMaQG = new BandedGridColumn();
                        colMaQG.AppearanceHeader.Options.UseTextOptions = true;
                        colMaQG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaQG.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaQG.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaQG.FieldName = dc.ColumnName;
                        colMaQG.Name = "col" + dc.ColumnName;
                        colMaQG.OptionsColumn.AllowEdit = false;
                        colMaQG.Visible = true;
                        colMaQG.Width = 50;
                        //colMaQG.ColumnEdit = searchLookUpEditQuocGia1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaQG });
                        gridBandMaQuocGia_Kho.Columns.Add(colMaQG);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ngaygh")
                    {
                        BandedGridColumn colNgayGH = new BandedGridColumn();
                        colNgayGH.AppearanceHeader.Options.UseTextOptions = true;
                        colNgayGH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colNgayGH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                        colNgayGH.DisplayFormat.FormatString = "dd/MM/yyyy";
                        colNgayGH.Caption = dc.ColumnName.Replace("Size@", "");
                        colNgayGH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colNgayGH.OptionsColumn.AllowEdit = false;
                        colNgayGH.FieldName = dc.ColumnName;
                        colNgayGH.Name = "col" + dc.ColumnName;
                        colNgayGH.Visible = true;
                        colNgayGH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colNgayGH });
                        gridBandNgayGH_Kho.Columns.Add(colNgayGH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ghichu")
                    {
                        BandedGridColumn colGhiChu = new BandedGridColumn();
                        colGhiChu.AppearanceHeader.Options.UseTextOptions = true;
                        colGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colGhiChu.Caption = dc.ColumnName.Replace("Size@", "");
                        colGhiChu.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colGhiChu.FieldName = dc.ColumnName;
                        colGhiChu.Name = "col" + dc.ColumnName;
                        colGhiChu.OptionsColumn.AllowEdit = false;
                        colGhiChu.Visible = true;
                        colGhiChu.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colGhiChu });
                        gridBandGhiChu_Kho.Columns.Add(colGhiChu);
                    }
                }
                return bandedGridview;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return new BandedGridView();
        }

        private void Luu_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        //private List<DonHangTongEntity> GetCurrentData(int soLuong)
        //{
        //    List<DonHangTongEntity> _lstDonHangTong = new List<DonHangTongEntity>();
        //    DonHangTongEntity _donHangTong = new DonHangTongEntity();
        //    _donHangTong.MaDH = txtMaDH.Text;
        //    _donHangTong.MaHang = searchLookUpEditMH.EditValue.ToString();
        //    _donHangTong.MaKH = searchLookUpEditKH.EditValue.ToString();
        //    _donHangTong.MaCL = searchLookUpEditCL.EditValue.ToString();
        //    _donHangTong.Dot = txtDot.Text;
        //    _donHangTong.NguoiTao = txtNguoiTao.Text;
        //    _donHangTong.NgayTao = dateEditNgayTao.DateTime;
        //    _donHangTong.VND = spinEditVND.Value;
        //    _donHangTong.CM = spinEditUSD.Value;
        //    _donHangTong.GhiChu = txtGhiChu.Text;

        //    _donHangTong.ID = donHangTong.ID;
        //    _donHangTong.SoLuong = soLuong;
        //    _donHangTong.TrangThai = donHangTong.TrangThai;

        //    _donHangTong.NguoiSua = GlobleData.UserName;
        //    _donHangTong.NgaySua = DateTime.Now;

        //    _lstDonHangTong.Add(_donHangTong);

        //    return _lstDonHangTong;
        //}

        private DataTable createTableSLKH()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("SoLuongSX", typeof(int));
            tbl.Columns.Add("CheckTruTK", typeof(int));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        private DataTable createTableSLKho()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaHS", typeof(string));
            tbl.Columns.Add("SoVoice", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("TrangThai", typeof(int));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        private void frmDonHangTonKho_Load(object sender, EventArgs e)
        {

        }

        private void bandedGridViewDanhSachSoLuong_KHSX_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
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

        private void bandedGridViewDanhSachSoLuong_KHSX_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void bandedGridViewDanhSachSoLuong_KHSX_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void searchLookUpEditMaDH_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (e != null && e is ChangingEventArgs)
            {
                LichSu.Enabled = true;
                actionControlHistory.Enabled = true;
                ChangingEventArgs changingEvent = e as ChangingEventArgs;
                maDH = changingEvent.NewValue.ToString();
                

                SearchLookUpEdit searchLookup = sender as SearchLookUpEdit;
                DataTable tbl = searchLookup.Properties.DataSource as DataTable;
                DataRow row = tbl.AsEnumerable().Where(x => x["MaDH"].ToString() == maDH).FirstOrDefault();
                if (row != null)
                {
                    maHang = row["MaHang"].ToString();
                }
                GetSoLuongKHSX_Kho(maDH,maHang);

                //foreach (DataRow row in tbl.Rows)
                //{
                //    if (row["MaDH"] == maDH)
                //    {
                //        maHang = row["MaHang"].ToString();
                //        break;
                //    }
                //}
            }
        }

        private void gridViewDanhSachSoLuong_ProcessGridKey(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //case Keys.F1:
                //    if (_allowAdd)
                //        BtThem();
                //    break;
                //case Keys.F2:
                //    if (_allowEdit)
                //        BtSua();
                //    break;
                //case Keys.F3:
                //    if (_allowDelete)
                //        DeleteClcik();
                //    break;
                //case Keys.F4:
                //    if (_allowAdd || _allowEdit)
                //        LuuDong();
                //    break;
                //case Keys.F5:
                //    barEditItem1.EditValue = null;
                //    LoadDSDonHangTong(null);
                //    break;
                case Keys.F6:
                    if (!string.IsNullOrEmpty(maDH))
                    {
                        frmLichSuTruTonKho lichSuTruTonKho = new frmLichSuTruTonKho(maHang);

                        lichSuTruTonKho.ShowDialog();
                    }
                    break;
            }
        }

    }
}
