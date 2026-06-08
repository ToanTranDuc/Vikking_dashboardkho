using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPBienBanChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblLo,tblCay,tblBBCT,tblBienBanCheck;
        
        HashSet<Tuple<string, string,string>> CaySelected = new HashSet<Tuple<string, string,string>>();
        HashSet<DataRow> selectedRowsLo = new HashSet<DataRow>();
        HashSet<DataRow> selectedRowsCay = new HashSet<DataRow>();
        public frmERPBienBanChiTiet()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            tblLo = new DataTable();
            tblCay = new DataTable();
            tblBBCT = new DataTable();
            CreateTableLo();
            CreateTableCay();
            CreateTabletblBienBanCheck();
            loadLO();
            dateNgayMoKien.EditValue = DateTime.Now;
            this.ActiveControl = simpleButton1;
        }
        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl1.SplitterPosition = (int)(splitContainerControl1.Height * 0.35);
        }

        private void splitContainerControl2_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl2.SplitterPosition = (int)(splitContainerControl1.Width * 0.4);
        }

        private void dateNgayMoKien_EditValueChanged(object sender, EventArgs e)
        {

        }
        private void CreateTableLo()
        {
            tblLo = new DataTable("tblLo");
            tblLo.Columns.Add("SoLoID", typeof(string));
            tblLo.Columns.Add("SoLo", typeof(string));
            tblLo.Columns.Add("NhaCungCap", typeof(string));
            tblLo.Columns.Add("TenNhaCungCap", typeof(string));
            tblLo.Columns.Add("SoChungTu", typeof(string));
            tblLo.Columns.Add("SoHopDong", typeof(string));
            tblLo.Columns.Add("SoHoaDon", typeof(string));
            tblLo.Columns.Add("NgayTao", typeof(string));
          
        }
        private void CreateTableCay()
        {
            tblCay = new DataTable("tblCay");
            tblCay.Columns.Add("SoLoID", typeof(string));
            tblCay.Columns.Add("SoKien", typeof(string));
            tblCay.Columns.Add("MaNPL", typeof(string));
            tblCay.Columns.Add("MaVTID", typeof(string));
            tblCay.Columns.Add("MaVT", typeof(string));
            tblCay.Columns.Add("ChiTiet", typeof(string));
            tblCay.Columns.Add("MaMauVT", typeof(string));
            tblCay.Columns.Add("MauVT", typeof(string));
            tblCay.Columns.Add("KhoVaiID", typeof(string));
            tblCay.Columns.Add("KhoVai", typeof(string));
            tblCay.Columns.Add("MaDVVT", typeof(string));
            tblCay.Columns.Add("TenDVVT", typeof(string));
            tblCay.Columns.Add("TheoCT", typeof(decimal));
            tblCay.Columns.Add("ThucNhap", typeof(decimal));
            tblCay.Columns.Add("DonGia", typeof(decimal));
            tblCay.Columns.Add("ThanhTien", typeof(decimal));

        }
        private void CreateTableBBCT()
        {
            tblBBCT = new DataTable("tblBBCT");
            tblBBCT.Columns.Add("SoLoID", typeof(string));
            tblBBCT.Columns.Add("SoKien", typeof(string));
            tblBBCT.Columns.Add("SoLo", typeof(string));
            tblBBCT.Columns.Add("MaNPL", typeof(string));
            tblBBCT.Columns.Add("MaVTID", typeof(string));
            tblBBCT.Columns.Add("MaVT", typeof(string));
            tblBBCT.Columns.Add("ChiTiet", typeof(string));
            tblBBCT.Columns.Add("MaMauVT", typeof(string));
            tblBBCT.Columns.Add("MauVT", typeof(string));
            tblBBCT.Columns.Add("KhoVaiID", typeof(string));
            tblBBCT.Columns.Add("KhoVai", typeof(string));
            tblBBCT.Columns.Add("MaDVVT", typeof(string));
            tblBBCT.Columns.Add("TenDVVT", typeof(string));
            tblBBCT.Columns.Add("TheoCT", typeof(decimal));
            tblBBCT.Columns.Add("ThucNhap", typeof(decimal));
            tblBBCT.Columns.Add("DonGia", typeof(decimal));
            tblBBCT.Columns.Add("ThanhTien", typeof(decimal));

        }
        private void CreateTabletblBienBanCheck()
        {
            tblBienBanCheck = new DataTable("tblBienBanCheck");
            tblBienBanCheck.Columns.Add("ID", typeof(int));
            tblBienBanCheck.Columns.Add("BBChiTietID", typeof(string));
            tblBienBanCheck.Columns.Add("SoLoID", typeof(string));
            tblBienBanCheck.Columns.Add("MaNPL", typeof(string));
            tblBienBanCheck.Columns.Add("NgayTao", typeof(DateTime));
            tblBienBanCheck.Columns.Add("NguoiTao", typeof(string));
            tblBienBanCheck.Columns.Add("SoKien", typeof(string));

        }
        private void gVLo_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush =
                    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);
                e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
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

        private void gVNPL_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush =
                    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);
                e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
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

        private void gVBBCT_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush =
                    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);
                e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
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
        private void loadLO()
        {
            try
            {
                string url = $"{URL}ERPBBMKChiTiet/Get?Action=GETLO";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    tblLo = new DataTable();
                    CreateTableLo();
                    gCLo.DataSource = null;
                }
                tblLo = JsonConvert.DeserializeObject<DataTable>(json);
                gCLo.DataSource = tblLo;
            }
            catch (Exception ex)
            {
            }
           

        }

        private void gVLo_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gVLo.GetFocusedDataRow();
                if (dr == null) return;
                loadCay(dr["SoLoID"].ToString());
                loadCheckCay(dr["SoLoID"].ToString());
            }
            catch (Exception ex)
            {
            }
         
        }
        private void loadCheckCay(string soloID)
        {
            try
            {
                //selectedRowsCay.Clear();
                string _date;
                if (dateNgayMoKien.EditValue == null)
                {
                    _date = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    _date = ((DateTime)dateNgayMoKien.EditValue).ToString("yyyy-MM-dd");
                }
                
                  
                string url = $"{URL}ERPBBMKChiTiet/Get?Action=GETBBCTCAY&para={_date}&para2={soloID}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    gVNPL.ClearSelection();
                    foreach(DataRow dr in tblBBCT.Rows)
                    {
                        var maNPL = dr["MaNPL"].ToString();
                        var soLoID = dr["SoLoID"].ToString();
                        var soKien = dr["SoKien"].ToString();
                        for (int i = 0; i < gVNPL.RowCount; i++)
                        {
                            var rowView = gVNPL.GetDataRow(i);
                            if (rowView != null && rowView["MaNPL"].ToString() == maNPL&&rowView["SoLoID"].ToString()==soLoID&&rowView["SoKien"].ToString() == soKien)
                            {
                                gVNPL.SelectionChanged -= gVNPL_SelectionChanged;
                                selectedRowsCay.Add(rowView);
                                gVNPL.SelectRow(i);
                                gVNPL.SelectionChanged += gVNPL_SelectionChanged;
                            }
                        } 
                    }    
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                //gVNPL.ClearSelection();
                foreach (DataRow row in tbl.Rows)
                {

                    var maNPL = row["MaNPL"].ToString();
                    var soKien = row["SoKien"].ToString();
                    for (int i = 0; i < gVNPL.RowCount; i++)
                    {
                        var rowView = gVNPL.GetDataRow(i);
                        if (rowView != null && rowView["MaNPL"].ToString() == maNPL && rowView["SoKien"].ToString() == soKien)
                        {
                            gVNPL.SelectionChanged -= gVNPL_SelectionChanged;
                            selectedRowsCay.Add(rowView);
                            gVNPL.SelectRow(i);
                            gVNPL.SelectionChanged += gVNPL_SelectionChanged;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }

        }

        private void loadCay(string soloID)
        {
            try
            {
                string url = $"{URL}ERPBBMKChiTiet/Get?Action=GETCAY&para={soloID}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    tblCay = new DataTable();
                    CreateTableCay();
                    gCNPL.DataSource = null;
                }
                tblCay = JsonConvert.DeserializeObject<DataTable>(json);
                gCNPL.DataSource = tblCay;
            }
            catch (Exception ex)
            {
            }
         
        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (tblBienBanCheck == null || tblBienBanCheck.Rows.Count == 0) CreateTabletblBienBanCheck();
              
                if (dateNgayMoKien.EditValue==null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Ngày mở kiện", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dateNgayMoKien.Focus();
                    return;
                }
                tblBienBanCheck.Clear();
                foreach (DataRow dr in tblBBCT.Rows)
                {
                    DataRow _nr = tblBienBanCheck.NewRow();
                    _nr["ID"] = 0;
                    _nr["BBChiTietID"] = "";
                    _nr["SoLoID"] = dr["SoLoID"].ToString();
                    _nr["MaNPL"] = dr["MaNPL"].ToString();
                    _nr["NgayTao"] = dateNgayMoKien.EditValue;
                    _nr["NguoiTao"] = GlobleData.UserName;
                    _nr["SoKien"] = dr["SoKien"].ToString();
                    tblBienBanCheck.Rows.Add(_nr);
                }
                if (tblBienBanCheck == null || tblBienBanCheck.Rows.Count == 0) return;
                string url = $"{URL}ERPBBMKChiTiet/Post?Action=POSTBBCT";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblBienBanCheck); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                }

            }
            catch (Exception ex)
            {
            }
        }


        #region Xử lý sự kiện chọn
        private void gVLo_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                //GridView view = sender as GridView;
                //int rowHandle3 = e.ControllerRow;
                //if (view == null) return;
                //if (rowHandle3 >= 0)
                //{
                //    DataRow row = view.GetDataRow(rowHandle3);

                //    if (view.IsRowSelected(rowHandle3))
                //    {
                //        selectedRowsLo.Add(row);

                //    }
                //    else
                //    {
                //        selectedRowsLo.Remove(row);

                //    }
                //}
                ////DataRow dr = gVLo.GetFocusedDataRow();
                ////if (dr == null) return;
                //if (e.Action == CollectionChangeAction.Add)
                //{

                //    for (int i = 0; i < gVNPL.RowCount; i++)
                //    {
                //        gVNPL.SelectRow(i);
                //    }


                //}
                //else if (e.Action == CollectionChangeAction.Remove)
                //{
                //    for (int i = 0; i < gVNPL.RowCount; i++)
                //    {
                //        gVNPL.UnselectRow(i);
                //    }


                //}

            }
            catch (Exception ex)
            {
            }
        }

        private void gVNPL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //try
            //{

            //    GridView view = sender as GridView;
            //    int rowHandle3 = e.ControllerRow;
            //    if (view == null) return;



            //    if (rowHandle3 >= 0)
            //    {
            //        DataRow row = view.GetDataRow(rowHandle3);

            //        if (view.IsRowSelected(rowHandle3))
            //        {

            //            CaySelected.Add(Tuple.Create(row["SoLoID"].ToString(), row["MaNPL"].ToString()));
            //            selectedRowsCay.Add(row);

            //        }
            //        else
            //        {
            //            CaySelected.Remove(Tuple.Create(row["SoLoID"].ToString(), row["MaNPL"].ToString()));
            //            selectedRowsCay.Remove(row);
            //            XoaCay();

            //        }
            //    }
            //    int[] selectedRowsgop = gVNPL.GetSelectedRows();
            //    if (selectedRowsgop.Length > 0)
            //    {

            //        foreach (int rowHandles in selectedRowsgop)
            //        {
            //            DataRow drRow = gVNPL.GetDataRow(rowHandles);

            //            AddRowBBCT(drRow);

            //        }
            //    }
            //    int totalRows = gVNPL.RowCount;
            //    for (int i = 0; i < totalRows; i++)
            //    {
            //        if (!Array.Exists(selectedRowsgop, selectedRow => selectedRow == i))
            //        {
            //            DataRow drRow = gVNPL.GetDataRow(i);
            //            DeleteCayBBCT(drRow);

            //        }
            //    }
            //    gCNPL.RefreshDataSource();
            //    checkSelectionLo();

            //}
            //catch (Exception ex)
            //{
            //}
            try
            {
                GridView view = sender as GridView;
                int rowHandle3 = e.ControllerRow;
                if (view == null) return;

                if (rowHandle3 >= 0)
                {
                    DataRow row = view.GetDataRow(rowHandle3);

                    if (view.IsRowSelected(rowHandle3))
                    {
                        CaySelected.Add(Tuple.Create(row["SoLoID"].ToString(), row["MaNPL"].ToString(),row["SoKien"].ToString()));
                        selectedRowsCay.Add(row);
                    }
                    else
                    {
                        CaySelected.Remove(Tuple.Create(row["SoLoID"].ToString(), row["MaNPL"].ToString(), row["SoKien"].ToString()));
                        selectedRowsCay.Remove(row);
                        XoaCay();
                    }
                }
                int[] selectedRowsgop = gVNPL.GetSelectedRows();
                if (selectedRowsgop.Length > 0)
                {
                    foreach (int rowHandles in selectedRowsgop)
                    {
                        DataRow drRow = gVNPL.GetDataRow(rowHandles);
                        AddRowBBCT(drRow);
                    }
                }
                int totalRows = gVNPL.RowCount;
                for (int i = 0; i < totalRows; i++)
                {
                    if (!Array.Exists(selectedRowsgop, selectedRow => selectedRow == i))
                    {
                        DataRow drRow = gVNPL.GetDataRow(i);
                        DeleteCayBBCT(drRow);
                    }
                }
                gCNPL.RefreshDataSource();
                checkSelectionLo();
            }
            catch (Exception ex)
            {
            }
        }
        private void XoaCay()
        {
            try
            {
                DataRow dr = gVNPL.GetFocusedDataRow();
                if (dr == null) return;
                string _date;
                if (dateNgayMoKien.EditValue == null)
                {
                    _date = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    _date = ((DateTime)dateNgayMoKien.EditValue).ToString("yyyy-MM-dd");
                }

                string url = $"{URL}ERPBBMKChiTiet/Delete?Action=DELETECAY&para={_date}&para2={dr["SoLoID"].ToString()}&para3={dr["MaNPL"]}&para4={dr["SoKien"]}";
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                }
            }
            catch (Exception ex)
            {
            }
           

        }
        private void checkSelectionLo()
        {
            //int rowCount = gVLo.RowCount;
            //int[] selectedRowsgop = gVLo.GetSelectedRows();
            //if (selectedRowsgop.Length > 0)
            //{
            //
            //    foreach (int rowHandles in selectedRowsgop)
            //    {
            //        DataRow drRow = gVLo.GetDataRow(rowHandles);
            //        bool found = CaySelected.Any(item => item.Item1 == drRow["SoLoID"].ToString());
            //        if (!found)
            //        {
            //            gVLo.SelectionChanged -= gVLo_SelectionChanged;
            //            selectedRowsLo.Remove(drRow);
            //            gVLo.UnselectRow(rowHandles);
            //            gVLo.SelectionChanged += gVLo_SelectionChanged;
            //        }
            //    }
            //}
            //for (int i = 0; i < rowCount; i++)
            //{

            //    if (!gVLo.IsDataRow(i)) continue;


            //    if (Array.IndexOf(selectedRowsgop, i) == -1)
            //    {

            //        DataRow drRow = gVLo.GetDataRow(i);
            //        bool found = CaySelected.Any(item => item.Item1 == drRow["SoLoID"].ToString());
            //        if (found)
            //        {
            //            gVLo.SelectionChanged -= gVLo_SelectionChanged;
            //            selectedRowsLo.Add(drRow);
            //            gVLo.SelectRow(i);
            //            gVLo.SelectionChanged += gVLo_SelectionChanged;
            //        }

            //    }
            //}
            try
            {
                int rowCount = gVLo.RowCount;
                int[] selectedRowsgop = gVLo.GetSelectedRows();
                DataRow focusedLoRow = gVLo.GetFocusedDataRow();
                if (focusedLoRow == null) return;

                string soLoID = focusedLoRow["SoLoID"].ToString();
                int totalCayRows = gVNPL.RowCount;
                int[] selectedCayRows = gVNPL.GetSelectedRows();
                bool allCaySelected = totalCayRows > 0 && selectedCayRows.Length == totalCayRows;

                // Unselect rows in gVLo that no longer have all corresponding gVNPL rows selected
                foreach (int rowHandle in selectedRowsgop)
                {
                    DataRow drRow = gVLo.GetDataRow(rowHandle);
                    if (drRow["SoLoID"].ToString() == soLoID && !allCaySelected)
                    {
                        gVLo.SelectionChanged -= gVLo_SelectionChanged;
                        selectedRowsLo.Remove(drRow);
                        gVLo.UnselectRow(rowHandle);
                        gVLo.SelectionChanged += gVLo_SelectionChanged;
                    }
                }

                // Select the row in gVLo if all rows in gVNPL are selected
                for (int i = 0; i < rowCount; i++)
                {
                    if (!gVLo.IsDataRow(i)) continue;
                    DataRow drRow = gVLo.GetDataRow(i);
                    if (drRow["SoLoID"].ToString() == soLoID && allCaySelected && Array.IndexOf(selectedRowsgop, i) == -1)
                    {
                        gVLo.SelectionChanged -= gVLo_SelectionChanged;
                        selectedRowsLo.Add(drRow);
                        gVLo.SelectRow(i);
                        gVLo.SelectionChanged += gVLo_SelectionChanged;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void AddRowBBCT(DataRow dr)
        {
            try
            {
                DataRow _row = gVLo.GetFocusedDataRow();
                if (_row == null) return;
                if (tblBBCT == null || tblBBCT.Rows.Count == 0) CreateTableBBCT();
                DataRow _nr = tblBBCT.NewRow();
                _nr["SoLoID"] = dr["SoLoID"].ToString();
                _nr["SoKien"] = dr["SoKien"].ToString();
                _nr["SoLo"] = _row["SoLo"].ToString();
                _nr["MaNPL"] = dr["MaNPL"].ToString();
                _nr["MaVTID"] = dr["MaVTID"].ToString();
                _nr["MaVT"] = dr["MaVT"].ToString();
                _nr["ChiTiet"] = dr["ChiTiet"].ToString();
                _nr["MaMauVT"] = dr["MaMauVT"].ToString();
                _nr["MauVT"] = dr["MauVT"].ToString();
                _nr["KhoVaiID"] = dr["KhoVaiID"].ToString();
                _nr["KhoVai"] = dr["KhoVai"].ToString();
                _nr["MaDVVT"] = dr["MaDVVT"].ToString();
                _nr["TenDVVT"] = dr["TenDVVT"].ToString();
                _nr["TheoCT"] = dr["TheoCT"];
                _nr["ThucNhap"] = dr["ThucNhap"];
                _nr["DonGia"] = dr["DonGia"];
                _nr["ThanhTien"] = dr["ThanhTien"];
                DataRow[] rows = tblBBCT.Select($"SoLoID = '{dr["SoLoID"].ToString()}' AND MaNPL = '{dr["MaNPL"].ToString()}' AND SoKien= '{dr["SoKien"].ToString()}'");
                if(rows.Length<=0)
                {
                    tblBBCT.Rows.Add(_nr);
                }
                tblBBCT = SortDataTableBySoKien(tblBBCT);
                gCBBCT.DataSource = tblBBCT;

            }
            catch (Exception ex)
            {
            }
        }
        public DataTable SortDataTableBySoKien(DataTable table, bool ascending = true)
        {
            if (table == null || !table.Columns.Contains("SoKien"))
                return table;

     
            DataTable sortedTable = table.Clone();

  
            var sortedRows = ascending
                ? table.AsEnumerable().OrderBy(row => Convert.ToInt32(row["SoKien"].ToString()))
                : table.AsEnumerable().OrderByDescending(row => Convert.ToInt32(row["SoKien"].ToString()));

   
            foreach (var row in sortedRows)
                sortedTable.ImportRow(row);

            return sortedTable;
        }


        private void dateNgayMoKien_Properties_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    selectedRowsLo.Clear();
            //    if (dateNgayMoKien.EditValue != null && dateNgayMoKien.EditValue is DateTime)
            //    {
            //        DataRow dr = gVLo.GetFocusedDataRow();
            //        if (dr == null) return;
            //        string _date = ((DateTime)dateNgayMoKien.EditValue).ToString("yyyy-MM-dd");
            //        string url = $"{URL}ERPBBMKChiTiet/Get?Action=GETBBCT&para={_date}";
            //        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //        tblBienBanCheck = JsonConvert.DeserializeObject<DataTable>(json);
            //        gVLo.ClearSelection();
            //        CaySelected.Clear();
            //        foreach (DataRow row in tblBienBanCheck.Rows)
            //        {

            //            var soLoID = row["SoLoID"].ToString();

            //            for (int i = 0; i < gVLo.RowCount; i++)
            //            {
            //                var rowView = gVLo.GetDataRow(i);
            //                if (rowView != null && rowView["SoLoID"].ToString() == soLoID)
            //                {
            //                    gVLo.SelectionChanged -= gVLo_SelectionChanged;
            //                    selectedRowsLo.Add(rowView);
            //                    gVLo.SelectRow(i);
            //                    gVLo.SelectionChanged += gVLo_SelectionChanged;
            //                }
            //            }

            //        }
            //        loadGVBBCT(_date);
            //        loadCheckCay(dr["SoLoID"].ToString());

            //    }
            //    //else
            //    //{
            //    //
            //    //    string _date = "";
            //    //}
            //}
            //catch (Exception ex)
            //{
            //}
            try
            {
                selectedRowsLo.Clear();
                selectedRowsCay.Clear();
                CaySelected.Clear();
                if (dateNgayMoKien.EditValue != null && dateNgayMoKien.EditValue is DateTime)
                {
                    string _date = ((DateTime)dateNgayMoKien.EditValue).ToString("yyyy-MM-dd");
                    string url = $"{URL}ERPBBMKChiTiet/Get?Action=GETBBCT&para={_date}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    //if(json=="[]")
                    //{

                    //}    
                    tblBienBanCheck = JsonConvert.DeserializeObject<DataTable>(json);
                    loadGVBBCT(_date);

                    DataRow focusedLoRow = gVLo.GetFocusedDataRow();
                    if (focusedLoRow == null) return;
                    string soLoID = focusedLoRow["SoLoID"].ToString();

                  
                    loadCay(soLoID);
                    loadCheckCay(soLoID);
                    gVLo.ClearSelection();
                    foreach (DataRow dr in tblBienBanCheck.Rows)
                    {
                        string url2 = $"{URL}ERPBBMKChiTiet/Get?Action=GETCAY&para={dr["SoLoID"].ToString()}";
                        string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json2);
                        int totalCayRows = tbl.Rows.Count;
                        DataRow[] rows = tblBienBanCheck.Select($"SoLoID = '{dr["SoLoID"].ToString()}'");
                        int soLuong = rows.Length;
                    
                        bool allCaySelected = totalCayRows > 0 && soLuong == totalCayRows;

                       
                        if (allCaySelected)
                        {
                            for (int i = 0; i < gVLo.RowCount; i++)
                            {
                                var rowView = gVLo.GetDataRow(i);
                                if (rowView != null && rowView["SoLoID"].ToString() == dr["SoLoID"].ToString())
                                {
                                    gVLo.SelectionChanged -= gVLo_SelectionChanged;
                                    selectedRowsLo.Add(rowView);
                                    gVLo.SelectRow(i);
                                    gVLo.SelectionChanged += gVLo_SelectionChanged;
                                }
                            }
                        }
                    }    
                    
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                selectedRowsLo.Clear();
                if (dateNgayMoKien.EditValue != null && dateNgayMoKien.EditValue is DateTime)
                {
                    DataRow dr = gVLo.GetFocusedDataRow();
                    if (dr == null) return;
                    string _date = ((DateTime)dateNgayMoKien.EditValue).ToString("yyyy-MM-dd");
                    string url = $"{URL}ERPBBMKChiTiet/Get?Action=GETBBCT&para={_date}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    tblBienBanCheck = JsonConvert.DeserializeObject<DataTable>(json);
                    gVLo.ClearSelection();
                    CaySelected.Clear();
                    foreach (DataRow row in tblBienBanCheck.Rows)
                    {

                        var soLoID = row["SoLoID"].ToString();

                        for (int i = 0; i < gVLo.RowCount; i++)
                        {
                            var rowView = gVLo.GetDataRow(i);
                            if (rowView != null && rowView["SoLoID"].ToString() == soLoID)
                            {
                                gVLo.SelectionChanged -= gVLo_SelectionChanged;
                                selectedRowsLo.Add(rowView);
                                gVLo.SelectRow(i);
                                gVLo.SelectionChanged += gVLo_SelectionChanged;
                            }
                        }

                    }
                    loadGVBBCT(_date);
                    loadCheckCay(dr["SoLoID"].ToString());

                }
                //else
                //{
                //
                //    string _date = "";
                //}
            }
            catch (Exception ex)
            {
            }
        }

        private void gVLo_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRowsLo.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void gVNPL_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRowsCay.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void gVLo_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            if (hitInfo.InRowCell && hitInfo.Column.Caption == "Selection") // Nhấp vào cột checkbox
            {

                DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
            }
        }

        private void gVNPL_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gVNPL_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gVBBCT_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gVBBCT_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void loadGVBBCT(string _date)
        {
            string url = $"{URL}ERPBBMKChiTiet/Get?Action=GETGVBBCT&para={_date}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                tblBBCT = new DataTable();
                CreateTableBBCT();
                gCBBCT.DataSource = null;
                return;
            }
            tblBBCT = JsonConvert.DeserializeObject<DataTable>(json);
            foreach(DataRow row in tblBBCT.Rows)
            {
                CaySelected.Add(Tuple.Create(row["SoLoID"].ToString(), row["MaNPL"].ToString(), row["SoKien"].ToString()));

            }    
            gCBBCT.DataSource = tblBBCT;
        }
        private void DeleteCayBBCT(DataRow dr)
        {
            try
            {
                if (dr == null) return;
                DataRow[] rows = tblBBCT.Select($"SoLoID = '{dr["SoLoID"].ToString()}' AND MaNPL = '{dr["MaNPL"].ToString()}' AND SoKien= '{dr["SoKien"].ToString()}'");
                foreach (DataRow row in rows)
                {
                    tblBBCT.Rows.Remove(row);
                  
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void DeleteLoBBCT()
        {
            try
            {

            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }
}