using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
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

namespace NtbSoft.ERP.Win.Modules
{
    public partial class frmCopyDinhMucMau : DevExpress.XtraEditors.XtraForm
    {

        public double DinhMucChung {get;set; }
        public DataTable tblPaste { get; set; } = new DataTable();

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        string _mahang = string.Empty; string _makh = string.Empty;
        string _madot = string.Empty;
        object MaVTID;
        object MauVatID;
        object MaNhom;
        object KhoVaiID;
        object MaCode;

        private HashSet<object> selectedRowsMauSP_Copy = new HashSet<object>();
        private HashSet<object> selecttedRowsMauSP_Paste = new HashSet<object>();

        DataTable _tbl_VatTuCopy = new DataTable();
        DataRow _row_focus;
        string _Dot = string.Empty;
        DataRow _KhachHang_Seleted;
        DataRow _MaHang_Seleted;
        DataTable tblSize = new DataTable();
        //DataTable _dt = new DataTable();
        DataTable tblSave = new DataTable();
        DataTable _tblSizeDMCopy = new DataTable();
        DataRow _rowSelected_VatTuCopy;
        public frmCopyDinhMucMau(DataTable tblVatTuCopy, DataRow row_focus, string Dot,string MaDot, DataRowView KhachHangSelected, DataRowView MaHangSelected,DataTable tblSizeDMCopy)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._tbl_VatTuCopy = tblVatTuCopy;
            this._row_focus = row_focus;
            this._Dot = Dot;
            this._madot = MaDot;
            this._KhachHang_Seleted = KhachHangSelected.Row;
            this._MaHang_Seleted = MaHangSelected.Row;
            this._tblSizeDMCopy = tblSizeDMCopy;
            _makh = _KhachHang_Seleted["MaKH"]?.ToString();
            _mahang = _MaHang_Seleted["MaHang"]?.ToString();
            tblPaste = tblSizeDMCopy.Copy();
            InitComponentCopyDMMau();
           
        }
       
        private void InitComponentCopyDMMau()
        {
            try
            {
                MaVTID = _row_focus["MaVTID"];
                MauVatID = _row_focus["MauVTID"];
                MaNhom = _row_focus["MaNhom"];
                KhoVaiID = _row_focus["KhoVaiID"];
                MaCode = _row_focus["MaCode"];

                txtKH.EditValue = _KhachHang_Seleted["TenKH"]?.ToString();
                txtMaHang.EditValue = _MaHang_Seleted["TenHang"]?.ToString();
                txtDot.EditValue = _Dot;
                txtNhom.EditValue = _row_focus["TenNhom"]?.ToString();
                txtMaVT.EditValue = _row_focus["MaVT"]?.ToString();
                txtChiTiet.EditValue = _row_focus["ChiTiet"]?.ToString();
                txtMauVT.EditValue = _row_focus["MauVT"]?.ToString();
                txtMauSP.EditValue = _row_focus["TenMau"]?.ToString();

                searchLookUpEdit_VatTuCopy.Properties.DataSource = _tbl_VatTuCopy;
                searchLookUpEdit_VatTuCopy.Properties.ValueMember = "MaCode";
                searchLookUpEdit_VatTuCopy.Properties.DisplayMember = "ChiTiet";
                searchLookUpEdit_VatTuCopy.Properties.NullText = "Chọn Vật Tư Copy";


                searchLookUpEdit_MauSpCopy.Properties.ValueMember = "MaMau";
                searchLookUpEdit_MauSpCopy.Properties.DisplayMember = "TenMau";
                searchLookUpEdit_MauSpCopy.Properties.NullText = "Chọn Màu Sản Phẩm Copy";

                searchLookUpEdit_MauSPPaste.Properties.ValueMember = "MaMau";
                searchLookUpEdit_MauSPPaste.Properties.DisplayMember = "TenMau";
                searchLookUpEdit_MauSPPaste.Properties.NullText = "Chọn Màu Sản Phẩm Paste";

            

                var tblFilter = _tblSizeDMCopy.AsEnumerable()
                        .Where(x =>
                            x["MaVTID"]?.ToString() == MaVTID?.ToString() &&
                            x["MauVTID"]?.ToString() == MauVatID?.ToString() &&
                            x["MaNhom"]?.ToString() == MaNhom?.ToString() &&
                            x["KhoVaiID"]?.ToString() == KhoVaiID?.ToString() &&
                            x["MaCode"]?.ToString() == MaCode?.ToString()
                        ).ToList();
               
                if(tblFilter?.Count > 0)
                {
                    
                   
                   createTable(tblFilter?.CopyToDataTable(),bgrvDinhMucPaste,gridBandSize, "gridBandSize");

                   var lstMauSPPaste = tblFilter?.AsEnumerable().Select(x => new { MaMau = x["MaMau"], TenMau = x["TenMau"] })?.Distinct()?.ToList();
                   searchLookUpEdit_MauSPPaste.Properties.DataSource = lstMauSPPaste;
                    if(lstMauSPPaste?.Count == 1)
                    {
                        searchLookUpEdit_MauSPPaste.EditValue = lstMauSPPaste[0].MaMau;
                    }
                }

                gcDinhMucPaste.DataSource = tblFilter.Count == 0 ? null :  tblFilter?.CopyToDataTable();
                gcDinhMucPaste.RefreshDataSource();

                BestFitCol(ItemSearchLookup_GridView);


                //tblSave = createTableSave();
            }
            catch(Exception ex)
            {

            }


           
        }
        #region Event for SearchLookup MutiSelected
        private void searchLookUpEditViewMauSP_Copy_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle = e.ControllerRow;
            if (view == null) return;
            object row = view.GetRow(e.ControllerRow);
            if (e.Action == CollectionChangeAction.Add)
            {
               
                if (row != null && !selectedRowsMauSP_Copy.Contains(row))
                {
                    selectedRowsMauSP_Copy.Add(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                //object row = view.GetRow(e.ControllerRow);
                if (row != null)
                {
                    selectedRowsMauSP_Copy.Remove(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                selectedRowsMauSP_Copy.Clear();


                int[] selectedRowHandles = view.GetSelectedRows();
                foreach (int rowHandle2 in selectedRowHandles)
                {
                    if (rowHandle2 >= 0)
                    {
                        row = view.GetRow(rowHandle2);
                        if (row != null)
                        {
                            selectedRowsMauSP_Copy.Add(row);
                        }
                    }
                }
            }
            var lstItemSelected = searchLookUpEditView_MauSp_Copy.GetSelectedRows().Select(rowHandle2 => searchLookUpEditView_MauSp_Copy.GetRowCellValue(rowHandle2, searchLookUpEdit_MauSpCopy.Properties.ValueMember))?.ToList();
          
            if(lstItemSelected.Count > 0)
            {
                grcDinhMucCopy.BeginUpdate();
                try
                {
                    try
                    {
                        bgrvDinhMucCopy.BeginUpdate();
                        string selectedValues = string.Join(";", lstItemSelected);
                        searchLookUpEdit_MauSpCopy.EditValue = selectedValues;

                        object  MaVTID = _rowSelected_VatTuCopy["MaVTID"];
                        object MauVatID = _rowSelected_VatTuCopy["MauVTID"];
                        object MaNhom = _rowSelected_VatTuCopy["MaNhom"];
                        object KhoVaiID = _rowSelected_VatTuCopy["KhoVaiID"];
                        object MaCode = _rowSelected_VatTuCopy["MaCode"];

                        var tblFilter = _tblSizeDMCopy.AsEnumerable()
                                .Where(x =>
                                    x["MaVTID"]?.ToString() == MaVTID?.ToString() &&
                                    x["MauVTID"]?.ToString() == MauVatID?.ToString() &&
                                    x["MaNhom"]?.ToString() == MaNhom?.ToString() &&
                                    x["KhoVaiID"]?.ToString() == KhoVaiID?.ToString() &&
                                    x["MaCode"]?.ToString() == MaCode?.ToString() &&
                                    lstItemSelected.Contains(x["MaMau"]?.ToString())
                                ).ToList();
                        
                        if(tblFilter?.Count > 0)
                        {
                            createTable(tblFilter?.CopyToDataTable(), bgrvDinhMucCopy, gridbandSizeCopy, "gridbandSizeCopy");
                        }
                        grcDinhMucCopy.DataSource = tblFilter?.Count == 0 ? null : tblFilter?.CopyToDataTable();
                      

                    }
                    finally
                    {
                        bgrvDinhMucCopy.EndUpdate();
                    }
                }
                finally
                {
                    grcDinhMucCopy.EndUpdate();
                }



            }
            else
            {
                grcDinhMucCopy.DataSource = null;
            }
            grcDinhMucCopy.RefreshDataSource();
            bgrvDinhMucCopy.ExpandAllGroups();
            if (searchLookUpEdit_MauSpCopy.EditValue is null) return;
        }

        private void searchLookUpEditViewMauSP_Copy_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRowsMauSP_Copy.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void searchLookUpEditViewMauSP_Copy_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
           
            if (selectedRowsMauSP_Copy == null || !selectedRowsMauSP_Copy.Any())
            {
                e.DisplayText = "Chọn Màu Sản Phẩm Copy";
                searchLookUpEdit_MauSpCopy.Properties.Appearance.ForeColor = Color.Red;
               // MaVTGhep["KhoSize"] = "";
            }
            else
            {
                var displayValues = new List<string>();
                for (int i = 0; i < searchLookUpEditView_MauSp_Copy.DataRowCount; i++)
                {
                    object rowValueObj = searchLookUpEditView_MauSp_Copy.GetRowCellValue(i, searchLookUpEdit_MauSpCopy.Properties.ValueMember);
                    string rowValue = rowValueObj?.ToString();
                    if (rowValue != null && selectedRowsMauSP_Copy.Any(dr => dr.GetType().GetProperty("MaMau").GetValue(dr, null).ToString() == rowValue))
                    {
                        string displayValue = searchLookUpEditView_MauSp_Copy.GetRowCellValue(i, searchLookUpEdit_MauSpCopy.Properties.DisplayMember)?.ToString();
                        if (displayValue != null)
                        {
                            displayValues.Add(displayValue);
                        }
                    }
                }

                e.DisplayText = string.Join(";", displayValues);
                searchLookUpEdit_MauSpCopy.Properties.Appearance.ForeColor = Color.Black;
                
            }
            
        }

        /*Selected màu SP paste*/
        private void searchLookUpEditViewMauSP_Paste_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle = e.ControllerRow;
            if (view == null) return;
            object row = view.GetRow(e.ControllerRow);
            if (e.Action == CollectionChangeAction.Add)
            {

                if (row != null && !selecttedRowsMauSP_Paste.Contains(row))
                {
                    selecttedRowsMauSP_Paste.Add(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
               //row = view.GetRow(e.ControllerRow);
                if (row != null)
                {
                    selecttedRowsMauSP_Paste.Remove(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                selecttedRowsMauSP_Paste.Clear();

                
                int[] selectedRowHandles = view.GetSelectedRows();
                foreach (int rowHandle2 in selectedRowHandles)
                {
                    row = view.GetRow(rowHandle2);
                    if (rowHandle2 >= 0)
                    {
                       
                        if (row != null)
                        {
                            selecttedRowsMauSP_Paste.Add(row);
                        }
                    }
                }
            }
           
            var lstItemSelected = searchLookUpEditView_MauSP_Paste.GetSelectedRows().Select(rowHandle2 => searchLookUpEditView_MauSP_Paste.GetRowCellValue(rowHandle2, searchLookUpEdit_MauSPPaste.Properties.ValueMember))?.ToList();
            string selectedValues = string.Join(";", lstItemSelected);
            searchLookUpEdit_MauSPPaste.EditValue = selectedValues;
            if (searchLookUpEdit_MauSpCopy.EditValue is null) return;
        }
        private void searchLookUpEditViewMauSP_Paste_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selecttedRowsMauSP_Paste.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void searchLookUpEditViewMauSP_Paste_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

            if (selecttedRowsMauSP_Paste == null || !selecttedRowsMauSP_Paste.Any())
            {
                e.DisplayText = "Chọn Màu Sản Phẩm Paste";
                searchLookUpEdit_MauSPPaste.Properties.Appearance.ForeColor = Color.Red;
                // MaVTGhep["KhoSize"] = "";
            }
            else
            {
                var displayValues = new List<string>();
                for (int i = 0; i < searchLookUpEditView_MauSP_Paste.DataRowCount; i++)
                {
                    object rowValueObj = searchLookUpEditView_MauSP_Paste.GetRowCellValue(i, searchLookUpEdit_MauSPPaste.Properties.ValueMember);
                    string rowValue = rowValueObj?.ToString();
                    if (rowValue != null && selecttedRowsMauSP_Paste.Any(dr => dr.GetType().GetProperty("MaMau").GetValue(dr, null).ToString() == rowValue))
                    {
                        string displayValue = searchLookUpEditView_MauSP_Paste.GetRowCellValue(i, searchLookUpEdit_MauSPPaste.Properties.DisplayMember)?.ToString();
                        if (displayValue != null)
                        {
                            displayValues.Add(displayValue);
                        }
                    }
                }

                e.DisplayText = string.Join(";", displayValues);
                searchLookUpEdit_MauSPPaste.Properties.Appearance.ForeColor = Color.Black;

            }

        }
        #endregion
        private void createTable(DataTable tab,BandedGridView Bandview, GridBand gridBandSize,string parentBandName)
        {
            Bandview.OptionsView.AllowCellMerge = false;
            GridBand parentBand = Bandview.Bands[$"{parentBandName}"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < Bandview.Columns.Count;)
                {
                    if (Bandview.Columns[i].FieldName.Contains("@"))
                    {
                        Bandview.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }
        
            foreach (DataColumn column in tab.Columns)
            {
             
                if (column.ColumnName.Contains("@"))
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[1];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = arrName[0];
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = true;
                    col.Visible = true;
                    col.Width = 60;
                    // col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.DisplayFormat.FormatString = "{0:0.####}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    Bandview.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = arrName[0];
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;
                    gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }
        private double tinhDMChung()
        {
            DataTable tbl = gcDinhMucPaste.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return 0;

            List<string> sizeColumns = tbl.Columns
                   .Cast<DataColumn>()
                   .Where(c => c.ColumnName.Contains("@"))
                   .Select(c => c.ColumnName)
                   .ToList();
            double total = 0;
            int count = 0;
            foreach (DataRow row in tbl.Rows)
            {


                foreach (string col in sizeColumns)
                {
                    if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                    {
                        total += val;
                        count++;
                    }
                }




            }
            double average = count > 0 ? total / count : 0;

            return Math.Round(average, 4);
        }
        private void tinhDinhMucGoiY()
        {
            try
            {
                DataTable tbl = gcDinhMucPaste.DataSource as DataTable;

                if (tbl == null || tbl.Rows.Count == 0) return;

                List<string> sizeColumns = tbl.Columns
              .Cast<DataColumn>()
              .Where(c => c.ColumnName.Contains("@"))
              .Select(c => c.ColumnName)
              .ToList();

                // 2. Nhóm theo MaMau
                var groupedByMaMau = tbl.AsEnumerable()
                    .Where(row => !row.IsNull("MaMau"))
                    .GroupBy(row => row["MaMau"].ToString());

                // 3. Tính trung bình theo từng nhóm MaMau
                foreach (var group in groupedByMaMau)
                {
                    double total = 0;
                    int count = 0;

                    foreach (DataRow row in group)
                    {
                        foreach (string col in sizeColumns)
                        {
                            if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                            {
                                total += val;
                                count++;
                            }
                        }
                    }

                    double average = count > 0 ? total / count : 0;

                    // Gán giá trị trung bình cho tất cả dòng cùng MaMau
                    foreach (DataRow row in group)
                    {
                        row["DinhMucGY"] = Math.Round(average, 4);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEdit_VatTuCopy_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as SearchLookUpEdit;
            if (editor == null) return;

            GridView view = editor.Properties.View;
            DataRow selectedRow = view.GetDataRow(view.FocusedRowHandle);
            
            if (selectedRow is DataRow row)
            {

                _rowSelected_VatTuCopy = selectedRow;
                object MaVTID = _rowSelected_VatTuCopy["MaVTID"];
                object MauVatID = _rowSelected_VatTuCopy["MauVTID"];
                object MaNhom = _rowSelected_VatTuCopy["MaNhom"];
                object KhoVaiID = _rowSelected_VatTuCopy["KhoVaiID"];
                object MaCode = _rowSelected_VatTuCopy["MaCode"];
                var query = _tblSizeDMCopy?.AsEnumerable().Where(x =>
                            x["MaVTID"]?.ToString() == MaVTID?.ToString() &&
                            x["MauVTID"]?.ToString() == MauVatID?.ToString() &&
                            x["MaNhom"]?.ToString() == MaNhom?.ToString() &&
                            x["KhoVaiID"]?.ToString() == KhoVaiID?.ToString() &&
                            x["MaCode"]?.ToString() == MaCode?.ToString()
                        ).ToList();
                var lstMauSPCopy = query.AsEnumerable().Select(x => new { MaMau = x["MaMau"], TenMau = x["TenMau"] })?.Distinct()?.ToList();
                searchLookUpEdit_MauSpCopy.Properties.DataSource = lstMauSPCopy;
                if (lstMauSPCopy?.Count == 1)
                {
                    searchLookUpEdit_MauSpCopy.EditValue = lstMauSPCopy[0].MaMau;
                }
                selectedRowsMauSP_Copy.Clear();
                selecttedRowsMauSP_Paste.Clear();
                searchLookUpEdit_MauSPPaste.EditValue = null;
                searchLookUpEdit_MauSpCopy.RefreshEditValue();
                //double.TryParse(selectedRow["DinhMucChung"]?.ToString(), out double DMChung);
                //int.TryParse(selectedRow["IsNeww"]?.ToString(), out int IsNew);
                //loadSize(selectedRow["MaMau"]?.ToString(), selectedRow["MauVTID"]?.ToString(), selectedRow["MaVTID"]?.ToString(),selectedRow["KhoVaiID"]?.ToString(), DMChung);
            }
        }

        private void searchLookUpEdit_VatTuCopy_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var editor = sender as SearchLookUpEdit;
            if (editor == null || e.Value == null) return;

            var view = editor.Properties.View as GridView;
            if (view == null) return;
            DataRow rowSelected  = view.GetDataRow(view.FocusedRowHandle);
            if(rowSelected != null)
            {
                bool isNL = false;
                bool.TryParse(rowSelected["NPL"]?.ToString(), out isNL);

                string chiTiet = rowSelected["ChiTiet"]?.ToString() ?? string.Empty;
                string mauVT = rowSelected["MauVT"]?.ToString() ?? string.Empty;
                string tenKho = rowSelected["KhoVai"]?.ToString() ?? string.Empty;

                e.DisplayText = string.Format("{0}: {1} - Màu: {2} - Khổ: {3}",
                    isNL ? "Nguyên Liệu" : "Phụ Liệu",
                    chiTiet,
                    mauVT,
                    tenKho);
            }
            
        }

        private void searchLookUpEdit_VatTuCopy_Popup(object sender, EventArgs e)
        {
            ItemSearchLookup_GridView.ExpandAllGroups();
            //BestFitCol(ItemSearchLookup_GridView);
        }
        private void searchLookUpEdit_VatTuCopy_CloseUp(object sender, CloseUpEventArgs e)
        {
            DevExpress.XtraEditors.SearchLookUpEdit editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor != null && e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate)
            {
                DevExpress.XtraGrid.Views.Grid.GridView popupGridView = editor.Properties.View;
                object selectedRowView = popupGridView.GetFocusedRow();

                if (selectedRowView != null)
                {
                    System.Data.DataRowView selectedRow = selectedRowView as System.Data.DataRowView;
                    if (selectedRow != null)
                    {

                        object MaVTGhepvalue = selectedRow["MaVTGhep"];
                        double.TryParse(selectedRow["DinhMucChung"]?.ToString(), out double DMChung);
                        //int.TryParse(selectedRow["IsNeww"]?.ToString(), out int IsNew);
                        //loadSize(selectedRow["MaMau"]?.ToString(), selectedRow["MauVTID"]?.ToString(), selectedRow["MaVTID"]?.ToString(), selectedRow["KhoVaiID"]?.ToString(), DMChung);
                        searchLookUpEdit_VatTuCopy.EditValue = MaVTGhepvalue;

                        //gVNL.SetFocusedRowCellValue("MaVT", maVtValue);
                        //gVNL.SetFocusedRowCellValue("ChiTiet", chiTietValue);

                        //gVNL.PostEditor();
                        //gVNL.UpdateCurrentRow();
                    }
                    else
                    {

                    }
                }
            }
        }
        private void bandedGVSize_DataSourceChanged(object sender, EventArgs e)
        {
            double DMChung = tinhDMChung();
            txtDMTamTinh.Text = DMChung.ToString();
            tinhDinhMucGoiY();
        }

        private void gVVatTu_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

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

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            //if (info.Column == gridColumn24)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            if (info.Column == rColTenNhom)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gVVatTu.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gVVatTu.GetRowLevel(e.RowHandle);
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

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;

            }
        }
        private void gVVatTu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
            catch(Exception ex)
            {

            }
            
        }
        private void bgv_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Band == null) return;

                Rectangle rect = e.Bounds;

            
                ControlPaint.DrawBorder3D(e.Graphics, rect);

                Color softOrange = Color.FromArgb(255, 204, 153);
                Brush brush = e.Cache.GetGradientBrush(
                    rect,
                    softOrange,
                    softOrange,
                    e.Band.AppearanceHeader.GradientMode
                );

                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

        
                Font boldFont = new Font(e.Appearance.Font, FontStyle.Bold);

           
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisWord,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                e.Graphics.DrawString(e.Info.Caption, boldFont, Brushes.Black, e.Info.CaptionRect, format);

     
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



        /*auto with và scroll*/

        private void BestFitCol(GridView view)
        {
            if (view == null || view.VisibleColumns.Count < 3) return;

            view.BeginUpdate();
            try
            {
                view.OptionsView.ColumnAutoWidth = false;
                view.BestFitColumns();

                for (int i = 0; i < 3; i++)
                {
                    var col = view.VisibleColumns[i];
                    if (col.MaxWidth != 100 ) //|| col.Fixed != DevExpress.XtraGrid.Columns.FixedStyle.Left)
                    {
                        col.MaxWidth = 100;
                        //col.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    }
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }

        private void btnSubmit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = txtDot;
            DataTable tblGrid = gcDinhMucPaste.DataSource as DataTable;
            if(tblGrid?.Rows.Count > 0)
            {
              
                foreach (DataRow row in tblGrid.Rows)
                {
                    foreach (DataColumn col in tblGrid.Columns)
                    {
                        // Chỉ cập nhật các cột chứa dấu "@"
                        if (col.ColumnName.Contains("@"))
                        {
                            var matchedRows = tblPaste.AsEnumerable()
                                .Where(x =>
                                    x["MauVTID"]?.ToString() == MauVatID?.ToString() &&
                                    x["MaVTID"]?.ToString() == MaVTID?.ToString() &&
                                    x["KhoVaiID"]?.ToString() == KhoVaiID?.ToString() &&
                                    x["MaNhom"]?.ToString() == MaNhom?.ToString() &&
                                    x["MaCode"]?.ToString() == MaCode?.ToString() &&
                                    x["MaMau"]?.ToString() == row["MaMau"]?.ToString() &&
                                    x["MaNhomSize"]?.ToString() == row["MaNhomSize"]?.ToString()
                                ).ToList();

                            // Nếu có dòng nào thỏa điều kiện
                            foreach (var rowPaste in matchedRows)
                            {
                                // Kiểm tra tồn tại dữ liệu trước khi gán tránh lỗi
                                if (row[col.ColumnName] != DBNull.Value && row[col.ColumnName] != null)
                                {
                                    rowPaste[col.ColumnName] = row[col.ColumnName];
                                }
                            }
                        }
                    }
                }



               
                double result;
                double.TryParse(txtDMTamTinh?.Text, out result);
                DinhMucChung = result;
                //IsNew = 1;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                XtraMessageBox.Show($"Vui lòng chọn vật từ để Copy và Paste!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           


        }


        /*custom group row + icon button*/
        Dictionary<int, Rectangle> groupButtonRects = new Dictionary<int, Rectangle>();


        private void gridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

                if (info.Column != null)
                {
                    string caption = info.Column.Caption;
                    if (info.Column.Caption == string.Empty)
                        caption = info.Column.ToString();
               
                    if (info.Column == colTenMau_DM || info.Column == colTenMau_Copy)
                    {
                        info.GroupText = string.Format("{0}", info.GroupValueText);
                    }
                
                    if (view.IsGroupRow(e.RowHandle))
                    {
                        int groupIndex = view.GetRowLevel(e.RowHandle);
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
                        else if (groupIndex == 2)
                        {
                            textColor = Color.Maroon;
                            font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                        }
                        Rectangle iconRect = new Rectangle(info.Bounds.Right - 40, info.Bounds.Y + 2, 20, 20);
                        // Lưu lại vị trí và đánh dấu đã vẽ
                        //groupButtonRects[info.RowHandle] = iconRect;
                        // Gán lại thuộc tính
                        e.Appearance.ForeColor = textColor;
                        e.Appearance.Font = font;
                        e.DefaultDraw();                        
                        e.Handled = true;

                    }
                }
                  
               
            }
            catch(Exception ex)
            {

            }
            
        }

        private void gridControl1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo hitInfo = bgrvDinhMucPaste.CalcHitInfo(e.Location);
                if (hitInfo.InGroupRow && groupButtonRects.TryGetValue(hitInfo.RowHandle, out Rectangle rect))
                {
                    string GroupMau = bgrvDinhMucPaste.GetGroupRowValue(hitInfo.RowHandle)?.ToString();
                    int RowHandle = bgrvDinhMucPaste.GetDataRowHandleByGroupRowHandle(hitInfo.RowHandle);
                    if (!string.IsNullOrEmpty(GroupMau) && RowHandle >= 0)
                    {
                        //MessageBox.Show("Bạn nhấn icon Group: " + bgrvDinhMucPaste.GetGroupRowValue(hitInfo.RowHandle));
                    }
                }
            }
            catch(Exception ex)
            {

            }
            
        }
     
        private void btnPaste_Click(object sender, EventArgs e)
        {
            try
            {
                gcDinhMucPaste.BeginUpdate();
                try
                {
                    bgrvDinhMucPaste.BeginUpdate();
                    DataTable tblCopyDM = grcDinhMucCopy.DataSource as DataTable;
                    if(tblCopyDM== null || tblCopyDM?.Rows.Count == 0)
                    {
                        XtraMessageBox.Show($"Vui lòng vật tư Copy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (selectedRowsMauSP_Copy.Count == selecttedRowsMauSP_Paste.Count)
                    {
                        DataTable tblPaste = gcDinhMucPaste.DataSource as DataTable;
                        var lstMauSPPaste = selecttedRowsMauSP_Paste.Select(dr =>dr.GetType().GetProperty("MaMau").GetValue(dr, null).ToString());
                        int GroupIdx = 0;
                        if (tblPaste.Rows.Count > 0)
                        {
                            var groups = tblCopyDM.AsEnumerable()
                            .GroupBy(row => row["MaMau"].ToString());
                            foreach (var group in groups)
                            {
                                foreach (DataRow rowCopy in group)
                                {
                                    foreach (DataColumn col in tblCopyDM.Columns)
                                    {
                                        if (col.ColumnName.Contains("@") && rowCopy[col.ColumnName] != null)
                                        {
                                            foreach (string MaMauPaste in lstMauSPPaste)
                                            {
                                                var query = tblPaste.AsEnumerable().Where(x => x["MaNhomSize"].Equals(rowCopy["MaNhomSize"]) && x["MaMau"]?.ToString() == MaMauPaste).ToList();
                                                foreach (DataRow row in query)
                                                {
                                                    if (row.Table.Columns.Contains(col.ColumnName))
                                                    {
                                                        row[col.ColumnName] = rowCopy[col.ColumnName];
                                                    }
                                                }
                                            }




                                        }
                                    }


                                }
                                GroupIdx++;
                                lstMauSPPaste= lstMauSPPaste.Skip(GroupIdx)?.ToList();
                            }
                        

                            gcDinhMucPaste.DataSource = tblPaste;
                            gcDinhMucPaste.RefreshDataSource();

                         

                        }
                        else
                        {
                            XtraMessageBox.Show($"Lấy dữ liệu đã xảy ra lỗi.Vui lòng thực hiệu lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show($"Số lượng màu SP Copy và Paste phải giống nhau .Vui lòng thực hiệu lại sau.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                finally
                {
                    bgrvDinhMucPaste.EndUpdate();
                }
            }
            finally
            {
                gcDinhMucPaste.EndUpdate();
            }
            
            
          
        }

        private void btnClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ClearGridValuesAtColumnsWithAtSign();


        }

        private void ClearGridValuesAtColumnsWithAtSign()
        {
            bgrvDinhMucPaste.BeginUpdate();
            try
            {
                var tblFilter = tblPaste.AsEnumerable()
                       .Where(x =>
                           x["MaVTID"]?.ToString() == MaVTID?.ToString() &&
                           x["MauVTID"]?.ToString() == MauVatID?.ToString() &&
                           x["MaNhom"]?.ToString() == MaNhom?.ToString() &&
                           x["KhoVaiID"]?.ToString() == KhoVaiID?.ToString() &&
                           x["MaCode"]?.ToString() == MaCode?.ToString()
                       ).ToList();

             

                gcDinhMucPaste.DataSource = tblFilter.Count == 0 ? null : tblFilter?.CopyToDataTable();
                gcDinhMucPaste.RefreshDataSource();
            }
            finally
            {
                bgrvDinhMucPaste.EndUpdate();
            }
        }

    }
}