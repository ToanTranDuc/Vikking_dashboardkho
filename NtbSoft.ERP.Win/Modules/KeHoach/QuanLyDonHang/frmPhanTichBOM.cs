using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Kho;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBOM : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _checkUserName = false;
        bool _allowDuyet = false, _allowHuyDuyet = false, _allowActive = false;
        private int _rowhandle = 0, _stt = 0;
        private bool isAdd = false;
        DataTable _dtSize;
        private string _madot = string.Empty;
        DataTable _tblmhvt;
        private bool isReset = false;
        private bool _isDuyet = false;
        DataTable _tblMauSP;
        DataTable tblMauVT = new DataTable();
        DataTable tblChungLoaiChiTiet = new DataTable();
        DataTable tblSizeChung = new DataTable();
        DataTable _tblKhoSize = new DataTable();
        int _sttVT = 1;
        int _sttVTPL = 1;
        public frmPhanTichBOM()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            _dtSize = new DataTable();
            _tblMauSP = new DataTable();
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckUserXetDuyet();
            LoadTVMauVT();
            CreateSearchLookup();
            loadKhoSizeBOM();
            CreateRepoSearchLookUpChungLoaiCT();
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        private void CreateSearchLookup()
        {
            try
            {
                string urlKH = string.Format("{0}?", URL + "PhanTichBom/GetKH");
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                searchLookUpEditKH.Properties.DataSource = tblKH;
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSearchLookUpDot(bool isSua = false)
        {
            searchLookUpEditDot.Properties.DisplayMember = "Dot";
            searchLookUpEditDot.Properties.ValueMember = "MaDot";

            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "KhoiTaoDM/GetThongSoDot", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDot.Properties.DataSource = tblDot;
            if (tblDot == null || tblDot.Rows.Count == 0)
            {
                btnCopyBOM.Enabled = false;
                searchLookUpEditDot.EditValue = "";
                gridControl1.DataSource = new DataTable();

            }
            else
            {
                if (isSua) return;
                searchLookUpEditDot.EditValue = tblDot.AsEnumerable()
        .OrderByDescending(row =>
        {
            int.TryParse(row["MaDot"].ToString().Split('|').Last(), out int value);
            return value;
        })
        .First()["MaDot"];
                //btnCopyBOM.Enabled = true;
            }

        }
        private void CheckUserXetDuyet()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=CheckUserXetDuyet&para1={GlobleData.UserName}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                btnDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnHuyDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                toggleIsActive.Enabled = false;
            }
            else
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    btnDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnHuyDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    toggleIsActive.Enabled = false;
                    return;
                }
                _allowActive = tbl.Rows[0]["AllowActive"].ToString() == "True" ? true : false;
                _allowDuyet = tbl.Rows[0]["AllowAdd"].ToString() == "True" ? true : false;
                _allowHuyDuyet = tbl.Rows[0]["AllowEdit"].ToString() == "True" ? true : false;

                btnDuyetAll.Visibility = _allowDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                btnHuyDuyetAll.Visibility = _allowHuyDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                toggleIsActive.Enabled = _allowActive;
            }
        }
        private void LoadData()
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
                string url = string.Format("{0}?makh={1}&&mahang={2}&&madot={3}", URL + "KhoiTaoDM/GetKTBom", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                    searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl.Rows.Count > 0)
                {
                    //if (gridBandMauSP != null)
                    //    AppendColumnsFromBand(tbl, gridBandMauSP);
                    gridControl1.DataSource = tbl;
                    bandedGridView1.FocusedRowHandle = _rowhandle;
                    _isDuyet = tbl.Rows[0]["IsXetDuyet"].ToString() == "Đã duyệt" ? true : false;
                    toggleIsActive.Toggled -= toggleIsActive_Toggled;
                    //toggleIsActive.EditValueChanging -= toggleIsActive_EditValueChanging;

                    toggleIsActive.EditValue = Convert.ToBoolean(tbl.Rows[0]["IsActive"]);
                    toggleIsActive.Properties.Appearance.ForeColor = toggleIsActive.IsOn ? Color.ForestGreen : Color.DimGray;
                    toggleIsActive.Toggled += toggleIsActive_Toggled;
                    //toggleIsActive.EditValueChanging += toggleIsActive_EditValueChanging;
                    // Assuming _sttVT is an integer variable and "STT" column contains integer values
                    _sttVT = tbl.AsEnumerable()
                      .Where(row => row.Field<bool>("NPL"))
                      .Select(row => (int)row.Field<long>("STT"))
                      .DefaultIfEmpty(0)
                      .Max() + 1;

                    _sttVTPL = tbl.AsEnumerable()
                   .Where(row => !row.Field<bool>("NPL"))
                   .Select(row => (int)row.Field<long>("STT"))
                   .DefaultIfEmpty(0)
                   .Max() + 1;
                    //_sttVTPL = (int)tbl.AsEnumerable().Where(row => !row.Field<bool>("NPL"))
                    //                               .Max(row => (int)row.Field<long>("STT")) + 1;
                    //_sttVT = (int)tbl.AsEnumerable().Max(row => row.Field<long>("STT")) + 1;

                    //if (tbl.Rows[0]["IsXetDuyet"].ToString() == "Đã duyệt")
                    //    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //else
                    //    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    gridControl1.DataSource = null;

                }
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                toggleIsActive.Toggled += toggleIsActive_Toggled;
                SplashScreenManager.CloseForm(false);
            }


        }

        private void CreateDotSTT()
        {
            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSTTDOT&para1={kh.ToString()}&para2={mh.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                textBox1.Text = "";

                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _stt = Convert.ToInt32(tbl.Rows[0][0]) + 1;
            textBox1.Text = searchLookUpEditKH.Text.ToString() + '-' + searchLookUpEditMH.Text.ToString() + '-' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
            _madot = kh.ToString() + '_' + mh.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
        }

        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string urlMH = string.Format("{0}?makh={1}", URL + "PhanTichBom/GetMH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
                DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);
                searchLookUpEditMH.Properties.DataSource = tblMH;
                searchLookUpEditMH.Properties.ValueMember = "MaHang";
                searchLookUpEditMH.Properties.DisplayMember = "TenHang";
                if (tblMH.Rows.Count == 1)
                {
                    searchLookUpEditMH.EditValue = tblMH.Rows[0]["MaHang"];
                }
                else
                    searchLookUpEditMH.EditValue = null;
                //if (isAdd)
                //    CreateSearchLookUpVT();
            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                loadSizeChung();
                if (!isAdd)
                    CreateSearchLookUpDot();
                else
                {
                    CreateDotSTT();
                    LoadSizeDM();
                }
                CreateMauSPColumns();

            }
            catch (Exception ex)
            {

            }
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null, TenVT = null, MauVT = null, NhomVT = null, Chitiet = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaVTID = view.GetRowCellValue(childHandle, gridColumn3);
                MauID = view.GetRowCellValue(childHandle, gridColumn8);
                TenVT = view.GetRowCellValue(childHandle, gridColumn5);
                MauVT = view.GetRowCellValue(childHandle, gridColumn10);
                NhomVT = view.GetRowCellValue(childHandle, gridColumn2);
                Chitiet = view.GetRowCellValue(childHandle, gridColumn6);
            }
            else
            {
                MaVTID = view.GetFocusedRowCellValue(gridColumn3);
                MauID = view.GetFocusedRowCellValue(gridColumn8);
                TenVT = view.GetFocusedRowCellValue(gridColumn5);
                MauVT = view.GetFocusedRowCellValue(gridColumn10);
                NhomVT = view.GetFocusedRowCellValue(gridColumn2);
                Chitiet = view.GetFocusedRowCellValue(gridColumn6);
            }
            //frmKhoiTaoDM frm = new frmKhoiTaoDM();
            //frm.ShowDialog();
            //LoadChiTiet();

        }


        private int FindFirstDataRowHandle(GridView view, int groupHandle)
        {
            int childCount = view.GetChildRowCount(groupHandle);
            for (int i = 0; i < childCount; i++)
            {
                int childHandle = view.GetChildRowHandle(groupHandle, i);
                if (view.IsGroupRow(childHandle))
                {
                    // Đệ quy vào group con
                    int result = FindFirstDataRowHandle(view, childHandle);
                    if (result >= 0)
                        return result;
                }
                else
                {
                    // Đây là data row thực sự
                    return childHandle;
                }
            }
            return -1;
        }
        private void LoadChiTiet()
        {

            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null, MaKhoVT = null, TachMau = null, MaNhom = null, MaCode = null;

            int levelGroup = bandedGridView1.GetRowLevel(bandedGridView1.FocusedRowHandle);
            int focusedHandle = view.FocusedRowHandle;

            if (view.IsGroupRow(focusedHandle))
            {
                int dataHandle = FindFirstDataRowHandle(view, focusedHandle);
                if (dataHandle >= 0)
                {
                    MaVTID = view.GetRowCellValue(dataHandle, gridColumn8);
                    MauID = view.GetRowCellValue(dataHandle, gridColumn13);
                    MaKhoVT = view.GetRowCellValue(dataHandle, gridColumn18);
                    TachMau = view.GetRowCellValue(dataHandle, gridColumn33);
                    MaNhom = view.GetRowCellValue(dataHandle, gridColumn4);
                    MaCode = view.GetRowCellValue(dataHandle, gridColumn49);

                }
            }
            else
            {
                MaVTID = view.GetRowCellValue(focusedHandle, gridColumn8);
                MauID = view.GetRowCellValue(focusedHandle, gridColumn13);
                MaKhoVT = view.GetRowCellValue(focusedHandle, gridColumn18);
                TachMau = view.GetRowCellValue(focusedHandle, gridColumn33);
                MaNhom = view.GetRowCellValue(focusedHandle, gridColumn4);
                MaCode = view.GetRowCellValue(focusedHandle, gridColumn49);
            }
            if (MaVTID == null && MauID == null && MaKhoVT == null && TachMau == null && MaNhom == null && MaCode == null)
            {

                return;
            }
            int focusedRowHandle = view.FocusedRowHandle;
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&mauID={4}&&makhoID={5}&&madot={6}&&tachmau={7}&&manhom={8}&&macode={9}", URL + "PhanTichBom/GetChiTietVTDotDM", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                           searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID == null ? "" : MaVTID.ToString(), MauID == null ? "" : MauID.ToString(), MaKhoVT == null ? "" : MaKhoVT.ToString(),
                           searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), TachMau.ToString() != "" ? Convert.ToBoolean(TachMau.ToString()) : false, MaNhom.ToString(), MaCode.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl.Rows.Count > 0)
            {

                DataRow dr = bandedGridView1.GetFocusedDataRow();
                string[] mamauArr = dr["MaMau"].ToString().Split('|');

                HashSet<string> mamauSet = new HashSet<string>(mamauArr);

                // Duyệt ngược để tránh lỗi khi xóa dòng trong quá trình duyệt
                for (int i = tbl.Rows.Count - 1; i >= 0; i--)
                {
                    string maMau = tbl.Rows[i]["MaMau"].ToString();
                    if (!mamauSet.Contains(maMau))
                    {
                        tbl.Rows.RemoveAt(i);
                    }
                }
                //// Cuối cùng gọi AcceptChanges để cập nhật thay đổi
                tbl.AcceptChanges();
                var groupedRows = tbl.AsEnumerable()
                                         .GroupBy(row => row["MaMau"])
                                         .ToList();

                foreach (var group in groupedRows)
                {
                    Decimal totalForGroup = 0;
                    int countColumnsWithAt = 0;
                    int rowsInGroup = group.Count();

                    foreach (var row in group)
                    {
                        int columnCount = tbl.Columns.Count;

                        for (int i = 0; i < columnCount; i++)
                        {
                            string columnName = tbl.Columns[i].ColumnName;
                            if (columnName.Contains("@"))
                            {
                                var columnValue = row[columnName];
                                if (columnValue != DBNull.Value && Convert.ToDecimal(columnValue) != 0)
                                {
                                    totalForGroup += Convert.ToDecimal(columnValue) * 1;
                                    countColumnsWithAt++;
                                }
                            }
                            else if (i + 1 < columnCount && tbl.Columns[i + 1].ColumnName.Contains("@"))
                            {
                                var nextColumnValue = row[tbl.Columns[i + 1].ColumnName];
                                if (nextColumnValue != DBNull.Value && Convert.ToDecimal(nextColumnValue) != 0)
                                {
                                    totalForGroup += Convert.ToDecimal(nextColumnValue);
                                    countColumnsWithAt++;
                                }
                            }
                        }
                    }
                    if (countColumnsWithAt > 0)
                    {
                        Decimal average = Math.Round(totalForGroup / countColumnsWithAt, 4);

                        foreach (var row in group)
                        {
                            row["DinhMucGY"] = average;
                        }
                    }
                }
                DataTable newTbl = new DataTable();
                foreach (DataColumn col in tbl.Columns)
                {
                    if (col.ColumnName == "IsXetDuyet")
                        newTbl.Columns.Add(col.ColumnName, typeof(string));
                    else
                        newTbl.Columns.Add(col.ColumnName, col.DataType);
                }
                foreach (DataRow row in tbl.Rows)
                {
                    DataRow newRow = newTbl.NewRow();
                    foreach (DataColumn col in tbl.Columns)
                    {
                        if (col.ColumnName == "IsXetDuyet")
                            newRow[col.ColumnName] = row[col.ColumnName].ToString();
                        else
                            newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    newTbl.Rows.Add(newRow);
                }

                foreach (DataRow row in newTbl.Rows)
                {

                    string maMau = row["MaMau"].ToString();
                    if (string.IsNullOrEmpty(maMau)) continue;
                    string manhomsize = row["MaNhomSize"].ToString();
                    row["MaVTID"] = dr["MaVTID"];
                    row["MauVTID"] = dr["MauVTID"];
                    row["KhoVaiID"] = dr["KhoVaiID"];
                    row["MaNhom"] = dr["MaNhom"];
                    row["MaCode"] = dr["MaCode"];

                    // Kiểm tra trùng
                    bool isDuplicate = _dtSize.AsEnumerable().Any(r =>
                        r["MaVTID"].ToString() == dr["MaVTID"].ToString() &&
                        r["MauVTID"].ToString() == dr["MauVTID"].ToString() &&
                        r["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() &&
                        r["MaNhom"].ToString() == dr["MaNhom"].ToString() &&
                        r["MaMau"].ToString() == maMau &&
                        r["MaNhomSize"].ToString() == manhomsize &&
                        r["MaCode"].ToString() == dr["MaCode"].ToString()
                    );

                    if (!isDuplicate)
                    {
                        _dtSize.ImportRow(row);
                    }



                }
                //foreach(DataRow dr1 in tbl.Rows)
                //{
                //    foreach(DataRow dr2 in _dtSize.Rows)
                //    {
                //        if(dr1["MaVTID"].ToString()==dr2["MaVTID"].ToString()&& dr1["MauVTID"].ToString() == dr2["MauVTID"].ToString()&& dr1["KhoVaiID"].ToString() == dr2["KhoVaiID"].ToString()
                //            && dr1["MaNhom"].ToString() == dr2["MaNhom"].ToString()&& dr1["MaCode"].ToString() == dr2["MaCode"].ToString())
                //        {
                //            dr2["IsXetDuyet"] = dr1["IsXetDuyet"].ToString();
                //        }    
                //    }    
                //}   
                var query = from dr1 in tbl.AsEnumerable()
                            join dr2 in _dtSize.AsEnumerable()
                            on new
                            {
                                MaVTID = dr1["MaVTID"].ToString(),
                                MauVTID = dr1["MauVTID"].ToString(),
                                KhoVaiID = dr1["KhoVaiID"].ToString(),
                                MaNhom = dr1["MaNhom"].ToString(),
                                MaCode = dr1["MaCode"].ToString()
                            }
                            equals new
                            {
                                MaVTID = dr2["MaVTID"].ToString(),
                                MauVTID = dr2["MauVTID"].ToString(),
                                KhoVaiID = dr2["KhoVaiID"].ToString(),
                                MaNhom = dr2["MaNhom"].ToString(),
                                MaCode = dr2["MaCode"].ToString()
                            }
                            select new { dr1, dr2 };

                foreach (var item in query)
                {
                    item.dr2["IsXetDuyet"] = item.dr1["IsXetDuyet"].ToString();
                }

                //createTable(_dtSize);


                DataTable tblSizeFiltered = _dtSize.Clone();
                var filteredRows = _dtSize.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                              row.Field<string>("MauVTID") == dr["MauVTID"].ToString() &&
                              row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                              row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                              row.Field<string>("MaCode") == dr["MaCode"].ToString());
                foreach (var row in filteredRows)
                {
                    tblSizeFiltered.ImportRow(row);
                }







            }
            else
            {
                DataRow dr = bandedGridView1.GetFocusedDataRow();
                DataTable tblSizeFiltered = _dtSize.Clone();
                var filteredRows = _dtSize.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                              row.Field<string>("MauVTID") == dr["MauVTID"].ToString() &&
                              row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                              row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                               row.Field<string>("MaCode") == dr["MaCode"].ToString());
                foreach (var row in filteredRows)
                {
                    tblSizeFiltered.ImportRow(row);
                }




            }

            //if ((bool)checkBox1.Checked == true)
            //{
            //    //layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //}
            //else
            //   // layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }


        //private void createTable(DataTable tab)
        //{
        //    bandedbandedGridView1.OptionsView.AllowCellMerge = true;
        //    GridBand parentBand = bandedbandedGridView1.Bands["gridBandSizeDM"];
        //    if (parentBand != null)
        //    {
        //        parentBand.Children.Clear();
        //        parentBand.Columns.Clear();
        //        for (int i = 0; i < bandedbandedGridView1.Columns.Count;)
        //        {
        //            if (bandedbandedGridView1.Columns[i].FieldName.Contains("@Size@"))
        //            {
        //                bandedbandedGridView1.Columns.RemoveAt(i);
        //            }
        //            else
        //            {
        //                i += 1;
        //            }
        //        }
        //    }
        //    int demColIndex = -1;

        //    foreach (DataColumn column in tab.Columns)
        //    {
        //        demColIndex++;
        //        if (demColIndex > 21 && demColIndex < tab.Columns.Count)
        //        {
        //            string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
        //            string colName = arrName[0];
        //            BandedGridColumn col = new BandedGridColumn();
        //            col.AppearanceCell.Options.UseTextOptions = true;
        //            col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            col.AppearanceHeader.Options.UseTextOptions = true;
        //            col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            col.Caption = colName;
        //            col.FieldName = column.ColumnName;
        //            col.Name = "col" + colName;
        //            //col.OptionsColumn.AllowEdit = false;
        //            col.Visible = true;
        //            col.Width = 85;
        //            //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
        //            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        //            //col.DisplayFormat.FormatString = "{0:##,0}";
        //            col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
        //            bandedbandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
        //            GridBand gb = new GridBand();
        //            gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
        //            gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
        //            gb.AppearanceHeader.Options.UseTextOptions = true;
        //            gb.Caption = colName;
        //            gb.Columns.Add(col);
        //            gb.Name = "gbColBandSize" + col;
        //            gb.VisibleIndex = 0;
        //            gb.Width = 85;
        //            gridBandSizeDM.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
        //        }
        //    }

        //}

        private void bandedGridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);

        }



        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void searchLookUpEditKH_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                e.DisplayText = "Chọn khách hàng";
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPThongSoVatTu frm = new frmERPThongSoVatTu();
            frm.ShowDialog();
            LoadData();
            LoadChiTiet();


        }
        private void NapLai(bool isSua = false)
        {

            isAdd = false;
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            CreateSearchLookUpDot(isSua);
            CreateDotSTT();

            LoadData();


        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai(!isAdd);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isAdd = true;
            _sttVT = 1;
            _sttVTPL = 1;
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            btnCopyBOM.Enabled = true;
            CreateDotSTT();

            LoadSizeDM();

            gridControl1.DataSource = null;
            //gridControl2.DataSource = null;
        }


        private DataTable createTablesave()
        {
            DataTable _tblSave = new DataTable();
            _tblSave.Columns.Add("ID", typeof(int));
            _tblSave.Columns.Add("MaKH", typeof(string));
            _tblSave.Columns.Add("MaHang", typeof(string));
            _tblSave.Columns.Add("MaVTID", typeof(string));
            _tblSave.Columns.Add("MauID", typeof(string));
            _tblSave.Columns.Add("KhoVaiID", typeof(string));
            _tblSave.Columns.Add("MaDot", typeof(string));
            _tblSave.Columns.Add("Dot", typeof(string));
            _tblSave.Columns.Add("NguoiTao", typeof(string));
            _tblSave.Columns.Add("NgayTao", typeof(DateTime));
            _tblSave.Columns.Add("NguoiXet", typeof(string));
            _tblSave.Columns.Add("NgayXet", typeof(DateTime));
            _tblSave.Columns.Add("NguoiSua", typeof(string));
            _tblSave.Columns.Add("NgaySua", typeof(DateTime));
            _tblSave.Columns.Add("IsXetDuyet", typeof(int));
            _tblSave.Columns.Add("NguoiHuy", typeof(string));
            _tblSave.Columns.Add("NgayHuy", typeof(DateTime));
            return _tblSave;
        }
        private void simpleButton11_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn khách hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn mã hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Duyet();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn khách hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn mã hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            HuyDuyet();
        }

        private void Duyet()
        {
            //this.ActiveControl = this.Button1;
            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaVTID = view.GetRowCellValue(childHandle, gridColumn3);
                MauID = view.GetRowCellValue(childHandle, gridColumn8);
            }
            else
            {
                MaVTID = view.GetFocusedRowCellValue(gridColumn3);
                MauID = view.GetFocusedRowCellValue(gridColumn8);
            }
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}", URL + "PhanTichBom/GetVTSPDotDM", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                       searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID == null ? "" : MaVTID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblvt = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable tblsave = createTablesave();

            foreach (DataRow row in tblvt.Rows)
            {
                DataRow _InsertRow = tblsave.NewRow();
                _InsertRow["ID"] = -1;
                _InsertRow["MaKH"] = searchLookUpEditKH.EditValue.ToString().Trim();
                _InsertRow["MaHang"] = searchLookUpEditMH.EditValue.ToString().Trim();
                _InsertRow["MaVTID"] = MaVTID;
                _InsertRow["MauID"] = row["MauID"].ToString() ?? "";
                _InsertRow["KhoVaiID"] = row["KhoVaiID"].ToString() ?? "";
                _InsertRow["MaDot"] = row["MaDot"].ToString() ?? "";
                _InsertRow["Dot"] = row["Dot"].ToString() ?? "";
                _InsertRow["NguoiTao"] = "";
                _InsertRow["NgayTao"] = (object)null ?? DBNull.Value;
                _InsertRow["NguoiXet"] = GlobleData.UserName;
                _InsertRow["NgayXet"] = DateTime.Now;
                _InsertRow["NguoiSua"] = "";
                _InsertRow["NgaySua"] = (object)null ?? DBNull.Value;
                _InsertRow["IsXetDuyet"] = 1;
                _InsertRow["NguoiHuy"] = "";
                _InsertRow["NgayHuy"] = (object)null ?? DBNull.Value;
                tblsave.Rows.Add(_InsertRow);
            }
            string urlSaveDuyet = string.Format("{0}", URL + "PhanTichBom/POSTDUYETALL");
            string savelistduyet = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDuyet, tblsave); }).Result;

            if (string.Compare(savelistduyet, "True") != 0)
            {
                XtraMessageBox.Show("Lỗi vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                LoadChiTiet();
                return;

            }

        }
        private void HuyDuyet()
        {
            //this.ActiveControl = this.Button1;
            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaVTID = view.GetRowCellValue(childHandle, gridColumn3);
                MauID = view.GetRowCellValue(childHandle, gridColumn8);
            }
            else
            {
                MaVTID = view.GetFocusedRowCellValue(gridColumn3);
                MauID = view.GetFocusedRowCellValue(gridColumn8);
            }
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}", URL + "PhanTichBom/GetVTSPDotDM", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                       searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID == null ? "" : MaVTID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblvt = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable tblsave = createTablesave();

            foreach (DataRow row in tblvt.Rows)
            {
                DataRow _InsertRow = tblsave.NewRow();
                _InsertRow["ID"] = -1;
                _InsertRow["MaKH"] = searchLookUpEditKH.EditValue.ToString().Trim();
                _InsertRow["MaHang"] = searchLookUpEditMH.EditValue.ToString().Trim();
                _InsertRow["MaVTID"] = MaVTID;
                _InsertRow["MauID"] = row["MauID"].ToString() ?? "";
                _InsertRow["KhoVaiID"] = row["KhoVaiID"].ToString() ?? "";
                _InsertRow["MaDot"] = row["MaDot"].ToString() ?? "";
                _InsertRow["Dot"] = row["Dot"].ToString() ?? "";
                _InsertRow["NguoiTao"] = "";
                _InsertRow["NgayTao"] = (object)null ?? DBNull.Value;
                _InsertRow["NguoiXet"] = "";
                _InsertRow["NgayXet"] = (object)null ?? DBNull.Value;
                _InsertRow["NguoiSua"] = "";
                _InsertRow["NgaySua"] = (object)null ?? DBNull.Value;
                _InsertRow["IsXetDuyet"] = 0;
                _InsertRow["NguoiHuy"] = GlobleData.UserName;
                _InsertRow["NgayHuy"] = DateTime.Now;
                tblsave.Rows.Add(_InsertRow);
            }
            string urlSaveHuy = string.Format("{0}", URL + "PhanTichBom/POSTHUYALL");
            string savelistHuy = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveHuy, tblsave); }).Result;
            if (string.Compare(savelistHuy, "True") != 0)
            {
                XtraMessageBox.Show("Lỗi vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                LoadChiTiet();
                return;

            }

        }

        private void bandedbandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            //GridView View = sender as GridView;
            //if (e.Column == bandedGridColumn11)
            //{
            //    object isXetDuyetValue = e.Value;
            //    if (isXetDuyetValue != null && bool.TryParse(isXetDuyetValue.ToString(), out bool isXetDuyet))
            //    {
            //        e.DisplayText = isXetDuyet ? "Đã Duyệt" : "Chưa Duyệt";

            //    }
            //    else
            //    {
            //        return;
            //    }

            //}
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.DisplayText = "-";
            }
            else
            {
                string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Count() > 1)
                {
                    if (Convert.ToDecimal(e.Value.ToString()) == 0)
                        e.DisplayText = "-";
                }
            }
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
                barButtonItem3.Enabled = false;
                barButtonItem7.Enabled = false;
            }
            if (!_allowEdit)
            {
                barButtonItem5.Enabled = false;
            }

        }

        private void bandedbandedGridView1_CellMerge(object sender, CellMergeEventArgs e)
        {
        }

        private void MenuDuyetDong(object sender, EventArgs e)
        {
            XetDuyet("duyệt", true, true);

        }


        private void MenuHuyDong(object sender, EventArgs e)
        {
            XetDuyet("hủy duyệt", false, true);

        }


        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            XetDuyet("duyệt", true, false);
        }
        private void XetDuyet(string title, bool xetduyet, bool checkduyet)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();

            if (dr == null) return;
            string MauVTID = dr["MauVTID"].ToString();

            string MaKH = dr["MaKH"].ToString();
            string MaHang = dr["MaHang"].ToString();
            string checkDuyetDot = checkduyet ? "" : "";

            if (!xetduyet)
            {
                string url2 = $"{URL}UpdateXetDuyet/Get?Action=CheckVT&para={MauVTID}&para2={checkDuyetDot}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                if (json != "[]")
                {
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (!checkduyet)
                        MessageBox.Show($"Có {tbl.Rows.Count} Đợt đã được xét duyện không thể hủy ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        MessageBox.Show($"Đợt này đã được cấp phát không thể hủy ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            //var result = checkduyet ? DialogResult.Yes : MessageBox.Show($"Bạn muốn {title} cây vải này", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (result == DialogResult.Yes)
            //{
            string url = $"{URL}UpdateXetDuyet/UpdateXetDuyet?Action=UpdateXetDuyen&para={xetduyet.ToString()}&para2={GlobleData.UserName}&para3={MaKH}&para4={MaHang}&para5={checkDuyetDot}&para6={MauVTID}";
            string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            LoadChiTiet();
            clsWaitForm.ShowSuccessForm(this, 2000);
            //}
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            XetDuyet("hủy duyệt", false, false);
        }



        private void bandedGridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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
            if (info.Column == gridColumn5)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn10)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (bandedGridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = bandedGridView1.GetRowLevel(e.RowHandle);
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
                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void bandedbandedGridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            bool isXetDuyetValue = view.GetRowCellValue(view.FocusedRowHandle, "IsXetDuyet").ToString() == "Đã duyệt";
            string inputValues = e.Value?.ToString();
            if (isXetDuyetValue && !string.IsNullOrEmpty(inputValues))
            {
                e.ErrorText = "Định mức đã được xét duyệt. Không thể sửa.!";
                e.Value = view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn);

                return;
            }

            if (view.FocusedColumn.FieldName.Contains("@") || view.FocusedColumn.FieldName.ToString() == "AllSize")
            {
                decimal outParse = -1;
                if (e != null && e.Value != null)
                {
                    string inputValue = e.Value.ToString();
                    bool isValidDecimal = decimal.TryParse(inputValue, out outParse);
                    if (string.IsNullOrEmpty(inputValue))
                    {
                        e.Value = 0;
                    }
                    else if (isValidDecimal)
                    {
                        if (outParse < 0)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập > 0!";
                            return;
                        }
                        int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(outParse)[3])[2];
                        if (decimalPlaces > 4)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập tối đa 4 chữ số sau dấu thập phân!";
                            return;
                        }
                    }
                    else
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!";
                        return;
                    }
                }
            }
        }






        #region excel
        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                string dot = string.Empty;
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                    return;
                if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                    return;

                if (textBox1.Text != null)
                {
                    dot = textBox1.Text;
                }
                if (searchLookUpEditDot.EditValue != null)
                {
                    dot = searchLookUpEditDot.EditValue.ToString();
                }
                if (dot == null || string.IsNullOrWhiteSpace(dot.ToString()))
                    return;

                DataTable tblGC = gridControl1.DataSource as DataTable;
                if (tblGC == null || tblGC.Rows.Count == 0)
                    return;

                DataTable _tblAllSize = loadDataExcel(searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString());
                if (_tblAllSize == null || _tblAllSize.Rows.Count == 0) return;

                frmPhanTichBom_Excel frm = new frmPhanTichBom_Excel(tblGC, _tblAllSize, searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString(), dot);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();

            }
            catch (Exception ex)
            {
            }
        }

        private DataTable loadDataExcel(string kh, string mh)
        {
            string url = $"{URL}PhanTichBom/GetExcel2?makh={kh}&mahang={mh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return null;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return null;
            return tbl;
        }

        private void searchLookUpEditDot_EditValueChanged(object sender, EventArgs e)
        {
            LoadSizeDM();
            LoadData();


        }

        #endregion

        #region duyệt/ hủy duyệt all
        private void btnDuyetAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraDuyet", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền Duyệt BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            duyetAll(1);
        }

        private void btnCopyBOM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(isAdd)
            {
                string maKH = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string maHang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string maDot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                DataTable tblGrid = gridControl1.DataSource as DataTable;
                if (tblGrid == null)
                    tblGrid = createTableSaveEdit();
                frmPhanTichBOM_Copy frmCopy = new frmPhanTichBOM_Copy(maKH, maHang, maDot, tblGrid);

                if (frmCopy.ShowDialog() == DialogResult.OK)
                {
                    DataTable tbl = frmCopy.ResultTable;
                    if (tbl != null)
                    {
                        gridControl1.DataSource = tbl;

                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Chức năng sao chép BOM chỉ khả dụng khi bạn đang khai báo đợt mới. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
        }


        private DataTable createTableSaveEdit()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("NPL", typeof(string));
            tbl.Columns.Add("Sort", typeof(int));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DinhMucChung", typeof(Decimal));
            tbl.Columns.Add("DinhMucHaoHut", typeof(Decimal));
            tbl.Columns.Add("TachMau", typeof(bool));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            tbl.Columns.Add("IsNew", typeof(int));
            tbl.Columns.Add("STTCode", typeof(int));
            tbl.Columns.Add("MaCode", typeof(string));
            tbl.Columns.Add("IsActive", typeof(bool));
            tbl.Columns.Add("MaNhomChiTiet", typeof(string));
            tbl.Columns.Add("MauVTIDChung", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("MaSizeChung", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("STT", typeof(int));
            if (gridBandMauSP != null)
                AppendColumnsFromBand(tbl, gridBandMauSP);
            return tbl;
        }
        private void AppendColumnsFromBand(DataTable tbl, GridBand band)
        {
            foreach (BandedGridColumn column in band.Columns)
            {
                string fieldName = column.FieldName;
                if (!string.IsNullOrEmpty(fieldName) && !tbl.Columns.Contains(fieldName))
                    tbl.Columns.Add(fieldName, typeof(string));
            }
            foreach (GridBand childBand in band.Children)
                AppendColumnsFromBand(tbl, childBand);
        }

        private void RemoveRowGCNL(DataRow rowRemove)
        {
            try
            {
                if (rowRemove == null) return;
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null) return;




                string filter = string.Format("MaVTID = '{0}' AND KhoVaiID = '{1}' AND MaNhom = '{2}' AND MaDVVT = '{3}'",
                                              rowRemove["MaVTID"], rowRemove["KhoVaiID"], rowRemove["MaNhom"], rowRemove["MaDVVT"]);

                DataRow[] rowsToDelete = tbl.Select(filter);

                foreach (DataRow dr in rowsToDelete)
                {
                    tbl.Rows.Remove(dr);
                }



            }
            catch (Exception ex)
            { }


        }



        private void btnHuyDuyetAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url1 = string.Format("{0}?madot={1}", URL + "KhoiTaoDM/KiemTraDot", searchLookUpEditDot.EditValue.ToString());
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            if (json1 != "[]")
            {
                //XtraMessageBox.Show("Đợt đã được cấp phát. Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                //return;
            }
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            duyetAll(0);
            NapLai();
        }
        private void duyetAll(int isduyet)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                    return;
                if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                    return;

                if (isduyet == 0 && toggleIsActive.IsOn)
                {

                    XtraMessageBox.Show("Đợt này đang được Active. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                string url = $"{URL}KhoiTaoDM/DuyetAll?makh={searchLookUpEditKH.EditValue.ToString()}&mahang={searchLookUpEditMH.EditValue.ToString()}&isduyet={isduyet}&username={GlobleData.UserName}&madot={searchLookUpEditDot?.EditValue?.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "True")
                {
                    string userName = GlobleData.UserName ?? "";
                    string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                    string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                    string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                    string urlTS = $"{URL}KhoiTaoBOMV1/Get?action=GET_TSNEW&para1={makh}&para2={mahang}&para6={userName}&para7={madot}";
                    string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;


                    clsWaitForm.ShowSuccessForm(this, 2000);
                    if (SplashScreenManager.Default != null)
                    {
                        SplashScreenManager.CloseForm(false);
                    }

                    //LoadData();
                    NapLai(true);

                }
            }
            catch (Exception ex)
            {
                if (SplashScreenManager.Default != null)
                {
                    SplashScreenManager.CloseForm(false);
                }

            }

        }
        #endregion



        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || searchLookUpEditDot.EditValue == null || searchLookUpEditMH.EditValue == null) return;
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa Đợt này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string url = $"{URL}PhanTichBom/DeleteDotBOM?makh={searchLookUpEditKH.EditValue.ToString()}&mahang={searchLookUpEditMH.EditValue.ToString()}&madot={searchLookUpEditDot.EditValue.ToString()}&userid={GlobleData.UserName}";
                    string json = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;

                    if (json.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this);
                        CreateSearchLookUpDot();
                        LoadData();
                        LoadChiTiet();

                    }

                }
            }
            catch (Exception ex) { }
        }


        private void LoadSizeDM()
        {
            try
            {
                _dtSize = new DataTable();
                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZEDM&para1={kh.ToString()}&para2={mh.ToString()}&para7={0}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _dtSize = JsonConvert.DeserializeObject<DataTable>(json);

                if (!_dtSize.Columns.Contains("AllSize"))
                {
                    _dtSize.Columns.Add("AllSize", typeof(float));
                }
                if (!_dtSize.Columns.Contains("CheckAll"))
                {
                    _dtSize.Columns.Add("CheckAll", typeof(bool)).DefaultValue = true;
                }
            }
            catch (Exception ex) { }
        }
        private void LoadSizeDMNew(DataRow _dr)
        {
            try
            {
                if (_dr == null) return;
                string maVTID = _dr["MaVTID"].ToString();
                string maMauVT = _dr["MauVTID"].ToString();
                string khoVaiID = _dr["KhoVaiID"].ToString();
                string MaNhom = _dr["MaNhom"].ToString();
                string MaCode = _dr["MaCode"].ToString();

                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZEDM&para1={kh.ToString()}&para2={mh.ToString()}&para7={0}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable _dt = JsonConvert.DeserializeObject<DataTable>(json);
                foreach (DataRow dr in _dt.Rows)
                {


                    dr["MaMau"] = _dr["MaMau"].ToString();
                    dr["TenMau"] = _dr["TenMau"].ToString();

                }
                if (!_dt.Columns.Contains("AllSize"))
                {
                    _dt.Columns.Add("AllSize", typeof(float));
                }
                if (!_dt.Columns.Contains("CheckAll"))
                {
                    _dt.Columns.Add("CheckAll", typeof(bool)).DefaultValue = true;
                }
                if (_dr["TachMau"].ToString() != "True")
                {

                    foreach (DataRow dr in _dtSize.Rows)
                    {

                        if (dr["MaVTID"].ToString() == maVTID &&
                             dr["MauVTID"].ToString() == maMauVT &&
                             dr["KhoVaiID"].ToString() == khoVaiID &&
                             dr["MaNhom"].ToString() == MaNhom &&
                              dr["MaCode"].ToString() == MaCode)
                        {
                            dr["MaMau"] = _dr["MaMau"].ToString();
                            dr["TenMau"] = _dr["TenMau"].ToString();
                        }
                    }

                    foreach (DataRow row in _dt.Rows)
                    {

                        string maMau = row["MaMau"].ToString();
                        if (string.IsNullOrEmpty(maMau)) continue;
                        string manhomsize = row["MaNhomSize"].ToString();
                        // Gán giá trị vào row
                        row["MaVTID"] = maVTID;
                        row["MauVTID"] = maMauVT;
                        row["KhoVaiID"] = khoVaiID;
                        row["MaNhom"] = MaNhom;
                        row["MaCode"] = MaCode;

                        // Kiểm tra trùng
                        bool isDuplicate = _dtSize.AsEnumerable().Any(r =>
                            r["MaVTID"].ToString() == maVTID &&
                            r["MauVTID"].ToString() == maMauVT &&
                            r["KhoVaiID"].ToString() == khoVaiID &&
                            r["MaNhom"].ToString() == MaNhom &&
                            r["MaMau"].ToString() == maMau &&
                            r["MaNhomSize"].ToString() == manhomsize &&
                            r["MaCode"].ToString() == MaCode
                        );

                        if (!isDuplicate)
                        {
                            _dtSize.ImportRow(row);
                        }

                    }
                }
                else
                {
                    DataRow drF = bandedGridView1.GetFocusedDataRow();
                    string[] maMauArr = drF["MaMau"].ToString().Replace(" ", "").Split('|');
                    string[] tenMauArr = drF["TenMau"].ToString().Replace(" ", "").Split('|');
                    var rowsToDelete = _dtSize.AsEnumerable()
                           .Where(dr => string.IsNullOrEmpty(dr["MaMau"]?.ToString()) || !maMauArr.Contains(dr["MaMau"].ToString().Replace(" ", ""))
                                        && dr["MaVTID"].ToString() == drF["MaVTID"].ToString()
                                         && dr["MaUVTID"].ToString() == drF["MaUVTID"].ToString()
                                          && dr["KhoVaiID"].ToString() == drF["KhoVaiID"].ToString()
                                           && dr["MaNhom"].ToString() == drF["MaNhom"].ToString()
                                             && dr["MaCode"].ToString() == drF["MaCode"].ToString()
                                        )


                           .ToList();

                    foreach (var row in rowsToDelete)
                    {
                        _dtSize.Rows.Remove(row);
                    }
                    int indexTenMau = 0;
                    foreach (string maMau in maMauArr)
                    {
                        string mm = maMau.Trim();


                        foreach (DataRow row in _dt.Rows)
                        {
                            DataRow newRow = _dtSize.NewRow();

                            newRow.ItemArray = row.ItemArray.Clone() as object[];
                            newRow["MaVTID"] = drF["MaVTID"].ToString();
                            newRow["MaUVTID"] = drF["MaUVTID"].ToString();
                            newRow["KhoVaiID"] = drF["KhoVaiID"].ToString();
                            newRow["MaNhom"] = drF["MaNhom"].ToString();
                            newRow["MaMau"] = mm;
                            newRow["TenMau"] = tenMauArr[indexTenMau];
                            newRow["MaCode"] = drF["MaCode"].ToString();
                            newRow["MaNhomChiTiet"] = drF["MaNhomChiTiet"].ToString();
                            newRow["IsActive"] = drF["IsActive"];
                            bool exists = _dtSize.AsEnumerable().Any(dr =>
                               dr["MaMau"].ToString().Replace(" ", "") == mm &&
                               dr["MaVTID"].ToString() == newRow["MaVTID"].ToString() &&
                               dr["MaUVTID"].ToString() == newRow["MaUVTID"].ToString() &&
                               dr["KhoVaiID"].ToString() == newRow["KhoVaiID"].ToString() &&
                               dr["MaNhom"].ToString() == newRow["MaNhom"].ToString() &&
                                dr["MaNhomSize"].ToString() == newRow["MaNhomSize"].ToString() &&
                                dr["MaCode"].ToString() == newRow["MaCode"].ToString()
                           );
                            if (!exists)
                                _dtSize.Rows.Add(newRow);

                        }




                        indexTenMau++;
                    }

                }





                DataTable tblSizeFiltered = _dtSize.Clone();
                var filteredRows = _dtSize.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == maVTID &&
                              row.Field<string>("MauVTID") == maMauVT &&
                              row.Field<string>("KhoVaiID") == khoVaiID &&
                              row.Field<string>("MaNhom") == MaNhom &&
                              row.Field<string>("MaCode") == MaCode);
                foreach (var row in filteredRows)
                {
                    tblSizeFiltered.ImportRow(row);
                }



            }

            catch (Exception ex)
            {

            }
        }


        private void repositoryItemButtonEditMauSP_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRow row = bandedGridView1.GetFocusedDataRow();
                if (row == null) return;
                if (row.Table.Columns.Contains("IsXetDuyet"))
                {
                    bool IsXetDuyet = false;
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                    if (IsXetDuyet)
                    {
                        XtraMessageBox.Show("Vật tư đã được xét duyêt. Không thể chỉnh sửa màu sản phẩm. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }

                }
                frmMauSanPhamV1 frm = new frmMauSanPhamV1(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), row["MaMau"].ToString(), row["TenMau"].ToString());
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK) return;
                List<KeyValuePair<string, string>> result = frm.GetSelectedDataAsList();

                if (result != null && result.Count > 0)
                {
                    row["MaMau"] = string.Join("|", result.Select(x => x.Value));
                    row["TenMau"] = string.Join("|", result.Select(x => x.Key));

                }
                else
                {
                    row["MaMau"] = "";
                    row["TenMau"] = "";
                }
                row["TachMau"] = true;
                this.ActiveControl = button1;
                LoadSizeDMNew(row);


            }
            catch (Exception ex)
            {


            }
        }







        private void bandedGridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (!isAdd) return;
            DataRow drF = bandedGridView1.GetFocusedDataRow();

            if (e.Column.FieldName == "MaNhomChiTiet")
            {



                foreach (DataRow row in _dtSize.Rows)
                {
                    if (row["MaVTID"].ToString() == drF["MaVTID"].ToString() && row["MauVTID"].ToString() == drF["MauVTID"].ToString() && row["KhoVaiID"].ToString() == drF["KhoVaiID"].ToString() && row["MaNhom"].ToString() == drF["MaNhom"].ToString())
                    {
                        row["MaNhomChiTiet"] = drF["MaNhomChiTiet"];

                    }

                }


            }





        }

        private async void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {



            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                SplashScreenManager.Default.SetWaitFormDescription("Đang chuẩn bị dữ liệu...");

                // Truy cập UI an toàn ở đây
                _rowhandle = bandedGridView1.FocusedRowHandle;
                string madotpost = "";
                string tendotpost = "";
                if (isAdd)
                {
                    madotpost = _madot;
                    tendotpost = textBox1.Text.ToString();
                }
                else
                {
                    madotpost = searchLookUpEditDot.EditValue?.ToString() ?? "";
                    tendotpost = searchLookUpEditDot.Text.ToString();
                }

                this.ActiveControl = button1;

                if (string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue?.ToString()) ||
                    string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue?.ToString()))
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                var tblGC1 = gridControl1.DataSource as DataTable;
                bool tonTaiNull = tblGC1 != null && tblGC1.AsEnumerable().Any(r => string.IsNullOrWhiteSpace(r["MaNhomChiTiet"].ToString()));
                if (tonTaiNull)
                {
                    XtraMessageBox.Show(this, "Vui lòng chọn đầy đủ chủng loại chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SplashScreenManager.CloseForm();
                    return;
                }
                List<DataRow> dongLoi = tblGC1.AsEnumerable()
                                                              .Where(row =>
                                                              {
                                                                  IEnumerable<DataColumn> cotMau = tblGC1.Columns.Cast<DataColumn>()
                                                                      .Where(col => col.ColumnName.Contains("@Mau@"));
                                                                  return cotMau.All(col =>
                                                                  {
                                                                      var value = row[col];
                                                                      return value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(value));
                                                                  });
                                                              })
                                                              .ToList();

                if (dongLoi.Count > 0)
                {
                    var loiTheoLoai = dongLoi
                        .GroupBy(row =>
                        {
                            bool laNguyenLieu = row["NPL"] != DBNull.Value && Convert.ToBoolean(row["NPL"]);
                            return laNguyenLieu ? "Nguyên liệu" : "Phụ liệu";
                        })
                        .Select(g => new
                        {
                            Loai = g.Key,
                            DanhSachStt = g.Select(row => Convert.ToString(row["STT"]))
                                .Where(stt => !string.IsNullOrWhiteSpace(stt))
                                .ToList()
                        })
                        .Where(g => g.DanhSachStt.Count > 0)
                        .ToList();

                    if (loiTheoLoai.Count > 0)
                    {
                        var builder = new StringBuilder();
                        foreach (var item in loiTheoLoai)
                        {
                            builder.AppendLine($"{item.Loai} thiếu màu vật tư ở các dòng: {string.Join(", ", item.DanhSachStt)}.");
                        }

                        XtraMessageBox.Show(this, builder.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SplashScreenManager.CloseForm();
                        return;
                    }
                }
                if (tblGC1 == null || tblGC1.Rows.Count == 0)
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                //var filteredRows = _dtSize.AsEnumerable().Where(row =>
                //    !string.IsNullOrWhiteSpace(row["MaMau"]?.ToString()) &&
                //    !string.IsNullOrWhiteSpace(row["MaVTID"]?.ToString()));
                //DataTable tblGC2 = filteredRows.Any() ? filteredRows.CopyToDataTable() : _dtSize.Clone();

                //if (tblGC2.Rows.Count == 0)
                //{
                //    SplashScreenManager.CloseForm();
                //    return;
                //}

                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mahang = searchLookUpEditMH.EditValue.ToString();

                // Thực thi logic nặng trên thread nền
                bool success = await Task.Run(async () =>
                {
                    DataTable dtMauSP = createTypeMauSP();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        foreach (DataColumn _cl in tblGC1.Columns)
                        {
                            if (_cl.ColumnName.Contains("@Mau@") && !string.IsNullOrWhiteSpace(_dr[_cl.ColumnName].ToString()))
                            {
                                string mamau = _cl.ColumnName.ToString().Split('@')[2];
                                DataRow _nr = dtMauSP.NewRow();
                                _nr["ID"] = 0;
                                _nr["MaKH"] = _makh;
                                _nr["MaHang"] = _mahang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = _dr[_cl.ColumnName];
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaMau"] = mamau.Trim();
                                _nr["MaDot"] = madotpost;
                                _nr["MaCode"] = _dr["MaCode"];
                                _nr["IsActive"] = _dr["IsActive"];
                                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                dtMauSP.Rows.Add(_nr);
                            }
                        }

                    }

                    DataTable dtSizeSP = createTypeSizeSP();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {

                        string maSizeChung = Convert.ToString(_dr["MaSizeChung"]);
                        Dictionary<string, string[]> sizeMap;

                        if (string.IsNullOrEmpty(maSizeChung))
                        {

                            sizeMap = tblSizeChung.AsEnumerable()
                                .GroupBy(row => row.Field<string>("MaNhomSize"))
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(row => row.Field<string>("MaSize")).Distinct().ToArray()
                                );
                        }
                        else
                        {

                            sizeMap = maSizeChung.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(item => item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                                .Where(parts => parts.Length == 2)
                                .ToDictionary(
                                    parts => parts[0].Trim(),
                                    parts => parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .ToArray()
                                );
                        }

                        foreach (DataColumn _cl in tblGC1.Columns)
                        {
                            if (_cl.ColumnName.Contains("@Mau@") && !string.IsNullOrWhiteSpace(_dr[_cl.ColumnName].ToString()))
                            {
                                string mamau = _cl.ColumnName.ToString().Split('@')[2];
                                //string mamau = _cl.ColumnName.Replace("@Mau@", "");
                                foreach (var kv in sizeMap)
                                {
                                    foreach (string size in kv.Value)
                                    {
                                        DataRow _nr = dtSizeSP.NewRow();
                                        _nr["MaKH"] = _makh;
                                        _nr["MaHang"] = _mahang;
                                        _nr["MaNhom"] = _dr["MaNhom"];
                                        _nr["MaVTID"] = _dr["MaVTID"];
                                        _nr["MauVTID"] = _dr[_cl.ColumnName];
                                        _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                        _nr["MaNhomSize"] = kv.Key;
                                        _nr["MaSize"] = size;
                                        _nr["DinhMuc"] = _dr["DinhMucChung"];
                                        _nr["MaDot"] = madotpost;
                                        _nr["Dot"] = tendotpost;
                                        _nr["NguoiTao"] = GlobleData.UserName;
                                        _nr["MaMau"] = mamau.Trim();
                                        _nr["TachMau"] = _dr["TachMau"];
                                        _nr["MaCode"] = _dr["MaCode"];
                                        _nr["IsActive"] = _dr["IsActive"];
                                        _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                        dtSizeSP.Rows.Add(_nr);
                                    }
                                }
                            }
                        }
                    }

                    DataTable dtVatTuSP = createTypeVattuSP();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        foreach (DataColumn _cl in tblGC1.Columns)
                        {
                            if (_cl.ColumnName.Contains("@Mau@") && !string.IsNullOrWhiteSpace(_dr[_cl.ColumnName].ToString()))
                            {

                                DataRow _nr = dtVatTuSP.NewRow();
                                _nr["ID"] = 0;
                                _nr["MaKH"] = _makh;
                                _nr["MaHang"] = _mahang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = _dr[_cl.ColumnName];
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaDot"] = madotpost;
                                _nr["Dot"] = tendotpost;
                                _nr["NguoiTao"] = GlobleData.UserName;
                                _nr["DinhMucChung"] = _dr["DinhMucChung"];
                                _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
                                _nr["TachMau"] = _dr["TachMau"];
                                _nr["STT"] = _dr["STT"];
                                _nr["MaCode"] = _dr["MaCode"];
                                _nr["STTCode"] = _dr["STTCode"];
                                _nr["IsActive"] = _dr["IsActive"];
                                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                _nr["MaVTGhep"] = _dr["GhiChu"];
                                dtVatTuSP.Rows.Add(_nr);
                            }
                        }
                    }
                    // SplashScreenManager.Default.SetWaitFormDescription("Đang lưu màu sản phẩm... (40%)");
                    string url1 = $"{URL}KhoiTaoBOMV1/Post1?Action=POSTMAUSP&para1={GlobleData.UserName}";
                    string result1 = await _clientExtension.PostAsync(url1, dtMauSP);
                    if (result1 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Đang lưu định mức size... (70%)");
                    string url2 = $"{URL}KhoiTaoBOMV1/Post2?Action=POSTDMSIZE&para1={GlobleData.UserName}";
                    string result2 = await _clientExtension.PostAsync(url2, dtSizeSP);
                    if (result2 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Đang lưu vật tư... (90%)");
                    string url3 = $"{URL}KhoiTaoBOMV1/Post3?Action=POSTVTSP&para1={GlobleData.UserName}&para2={madotpost}";
                    string result3 = await _clientExtension.PostAsync(url3, dtVatTuSP);
                    if (result3 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Hoàn tất... (100%)");
                    return true;
                });

                SplashScreenManager.CloseForm(false);

                if (success)
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    NapLai(!isAdd);
                    bandedGridView1.FocusedRowHandle = _rowhandle;
                    btnCopyBOM.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                //MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }


        }
        private DataTable createTypeMauSP()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private DataTable createTypeSizeSP()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaNhomSize", typeof(string));
            dt.Columns.Add("MaSize", typeof(string));
            dt.Columns.Add("DinhMuc", typeof(double));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TachMau", typeof(bool));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private DataTable createTypeVattuSP()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("DinhMucChung", typeof(double));
            dt.Columns.Add("DinhMucChiTiet", typeof(double));
            dt.Columns.Add("TachMau", typeof(bool));
            dt.Columns.Add("MaVTGhep", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("STTCode", typeof(int));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            int cursorPosition = textEdit.SelectionStart;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                string newText = currentText.Insert(cursorPosition, e.KeyChar.ToString());
                if (newText.Contains("."))
                {
                    int indexOfDot = newText.IndexOf('.');
                    string decimalPart = newText.Substring(indexOfDot + 1);
                    if (decimalPart.Length > 4)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        private void bandedGridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {


            try
            {
                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (bandedGridView1.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {

                            bool IsXetDuyet = false;
                            DataRow row_focused = bandedGridView1.GetFocusedDataRow();
                            if (row_focused == null) return;
                            if (row_focused.Table.Columns.Contains("IsXetDuyet"))
                            {
                                IsXetDuyet = row_focused["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                            }
                            bool IsCopyMauDM = false;
                            if (!IsXetDuyet)
                            {

                                DevExpress.Utils.Menu.DXMenuItem menuThemDongItem = new DevExpress.Utils.Menu.DXMenuItem("Thêm dòng", ThemDong);
                                e.Menu.Items.Add(menuThemDongItem);

                                DevExpress.Utils.Menu.DXMenuItem menuCopyCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", CopyDong);
                                e.Menu.Items.Add(menuCopyCopyItem);
                                //if (!string.IsNullOrEmpty(row_focused["MaMau"]?.ToString()) && IsCopyMauDM)
                                //{
                                //    DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Copy định mức màu", CopyDinhMuc_Mau);
                                //    e.Menu.Items.Add(menuCopyMauItem);

                                //}
                                DevExpress.Utils.Menu.DXMenuItem menuRemoveVatTuItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", RemoveVatTu);
                                e.Menu.Items.Add(menuRemoveVatTuItem);

                            }

                        }
                        //if (!isAdd)
                        //{
                        //    DevExpress.Utils.Menu.DXMenuItem menuXetduyetdong = new DevExpress.Utils.Menu.DXMenuItem("Xét duyệt dòng", xetduyetdong);
                        //    e.Menu.Items.Add(menuXetduyetdong);
                        //    DevExpress.Utils.Menu.DXMenuItem menuhuyduyetdong = new DevExpress.Utils.Menu.DXMenuItem("Hủy duyệt dòng", huyduyetdong);
                        //    e.Menu.Items.Add(menuhuyduyetdong);
                        //}

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //private void xetduyetdong(object sender, EventArgs e)
        //{
        //    _rowhandle = bandedGridView1.FocusedRowHandle;
        //    DataRow currentRow = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);
        //    if (currentRow == null) return;
        //    string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]")
        //    {
        //        XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
        //        return;
        //    }
        //    string urls = $"{URL}KhoiTaoBOMV1/XetDuyetDong?Action=XETDUYETDONG&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}" +
        //        $"&para3={searchLookUpEditDot.EditValue.ToString()} &para4={currentRow["MaVTID"].ToString()} &para5={currentRow["MauVTID"].ToString()} &para6={currentRow["KhoVaiID"].ToString()}" +
        //        $"&para7={currentRow["MaCode"].ToString()} &para8={1}";
        //    string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(urls); }).Result;
        //    if (jsons == "True")
        //    {
        //        clsWaitForm.ShowSuccessForm(this, 2000);
        //        LoadData();
        //        LoadChiTiet();
        //    }


        //}
        //private void huyduyetdong(object sender, EventArgs e)
        //{
        //    _rowhandle = bandedGridView1.FocusedRowHandle;
        //    DataRow currentRow = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);
        //    if (currentRow == null) return;
        //    string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]")
        //    {
        //        XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
        //        return;
        //    }
        //    if (toggleIsActive.IsOn)
        //    {
        //        XtraMessageBox.Show("Đợt này đang đucợ active. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
        //        return;
        //    }
        //    string urls = $"{URL}KhoiTaoBOMV1/XetDuyetDong?Action=HUYDUYETDONG&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}" +
        //        $"&para3={searchLookUpEditDot.EditValue.ToString()} &para4={currentRow["MaVTID"].ToString()} &para5={currentRow["MauVTID"].ToString()} &para6={currentRow["KhoVaiID"].ToString()}" +
        //        $"&para7={currentRow["MaCode"].ToString()} &para8={0}";
        //    string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(urls); }).Result;
        //    if (jsons == "True")
        //    {
        //        clsWaitForm.ShowSuccessForm(this, 2000);
        //        LoadData();
        //        LoadChiTiet();
        //    }
        //}
        private void CopyDong(object sender, EventArgs e)
        {

            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                DataTable table = gridControl1.DataSource as DataTable;
                if (table == null) return;

                DataRow currentRow = view.GetDataRow(rowHandle);
                if (currentRow == null) return;

                string currentMaVTID = currentRow["MaVTID"].ToString();
                string currentMaNhom = currentRow["MaNhom"].ToString();
                string currentKhoVaiID = currentRow["KhoVaiID"].ToString();
                bool isNL = currentRow["NPL"].ToString() == "True" ? true : false;
                int currentSTT = Convert.ToInt32(currentRow["STT"]);
                int newSTTP = currentSTT + 1;
                // Tìm STT lớn nhất với MaVTID hiện tại
                var maxSTT = table.AsEnumerable()
                                  .Where(r => r["MaVTID"].ToString() == currentMaVTID && int.TryParse(r["STTCode"].ToString(), out _))
                                  .Select(r => Convert.ToInt32(r["STT"]))
                                  .DefaultIfEmpty(0)
                                  .Max();

                int newSTT = maxSTT + 1;

                // Tạo dòng mới
                DataRow newRow = table.NewRow();
                foreach (DataColumn col in table.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }

                newRow["STTCode"] = newSTT;
                newRow["MaCode"] = $"{currentMaVTID}|{newSTT}";

                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 0;
                newRow["STT"] = newSTTP;
                var rowsToUpdate = table.AsEnumerable()
           .Where(r => r["NPL"].ToString() == isNL.ToString() && Convert.ToInt32(r["STT"]) >= newSTTP)
           .ToList();

                // Tăng STT của các dòng phía sau lên 1
                foreach (DataRow row in rowsToUpdate)
                {
                    row["STT"] = Convert.ToInt32(row["STT"]) + 1;
                }
                if (isNL)
                {

                    _sttVT++;
                }
                else
                {

                    _sttVTPL++;

                }

                // Chèn dòng vào bảng
                int insertIndex = view.GetDataSourceRowIndex(rowHandle) + 1;
                table.Rows.InsertAt(newRow, insertIndex);

                // Làm mới lưới và focus dòng mới
                view.RefreshData();
                int newRowHandle = view.GetRowHandle(insertIndex);
                view.FocusedRowHandle = newRowHandle;
                view.MakeRowVisible(newRowHandle);
            }
        }
        private void RemoveVatTu(object sender, EventArgs e)
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                int Rowfocus_Idx = bandedGridView1.FocusedRowHandle;
                if (Rowfocus_Idx < 0) return;
                DataRow rowFocus = bandedGridView1.GetFocusedDataRow();
                if (rowFocus != null)
                {

                    DialogResult result = XtraMessageBox.Show(
                     "Bạn có chắc chắn muốn xóa không?",
                     "XÁC NHẬN XÓA",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        object MaVTID = rowFocus["MaVTID"];
                        //object MauVatID = rowFocus["MauVTID"];
                        object MaNhom = rowFocus["MaNhom"];
                        object KhoVaiID = rowFocus["KhoVaiID"];
                        object MaCode = rowFocus["MaCode"];

                        string[] mauvtidchung = rowFocus["MauVTIDChung"].ToString().Split(',');
                        foreach(string mauvtid in mauvtidchung)
                        {
                            dynamic JPara = new
                            {
                                MaVTID = MaVTID,
                                MauVTID = mauvtid,
                                MaNhom = MaNhom,
                                KhoVaiID = KhoVaiID,
                                MaCode = MaCode,
                                MaKH = makh,
                                MaHang = mahang,
                                MaDot = madot,
                                UserID = GlobleData.UserName
                            };



                            string urlRemove = $"{URL}DeleteBOM_DM/Delete";
                            string resultRemove = Task.Run(async () => { return await _clientExtension.PostAsync(urlRemove, JPara); }).Result;
                            if (string.Compare(resultRemove?.ToLower(), "true") != 0)
                            {
                                XtraMessageBox.Show($"Xóa đã xảy ra lỗi.Vui lòng thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            RemoveRowDM(rowFocus, Rowfocus_Idx);
                        }    
                      


                    }
                }
            }
            catch (Exception ex)
            {

            }


        }


        private void RemoveRowDM(DataRow rowRm, int Rowfocus_Idx)
        {
            if (rowRm != null)
            {
                bool isNL = rowRm["NPL"].ToString() == "True" ? true : false;
                int currentSTT = Convert.ToInt32(rowRm["STT"]);

                DataTable table = gridControl1.DataSource as DataTable;
                var rowsToUpdate = table.AsEnumerable()
          .Where(r => r["NPL"].ToString() == isNL.ToString() && Convert.ToInt32(r["STT"]) >= currentSTT)
          .ToList();

                // Tăng STT của các dòng phía sau lên 1
                foreach (DataRow row in rowsToUpdate)
                {
                    row["STT"] = Convert.ToInt32(row["STT"]) - 1;
                }

                bandedGridView1.DeleteRow(Rowfocus_Idx);
                bandedGridView1.RefreshData();
                if (isNL)
                {

                    _sttVT--;
                }
                else
                {

                    _sttVTPL--;

                }


            }
        }


        private string GetLocalIPAddress()
        {
            string localIP = "Không xác định";
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        private void WriteLogRemove_DM(object MaVTID, object MauVatID, object MaNhom, object KhoVaiID, object MaCode)
        {
            try
            {
                string IpAdress = GetLocalIPAddress();
                List<Log_Erp_KhoiTaoBom_Delete> lstLogSave = new List<Log_Erp_KhoiTaoBom_Delete>();
                lstLogSave.Add(new Log_Erp_KhoiTaoBom_Delete
                {
                    Action = "Xóa BOM",
                    //TableName = "ERPVatTuSP",
                    Modules = this.Name,
                    MaVTID = MaVTID?.ToString(),
                    MauVTID = MauVatID?.ToString(),
                    MaNhom = MaNhom?.ToString(),
                    KhoVaiID = KhoVaiID?.ToString(),
                    MaCode = MaCode?.ToString(),
                    Content = "",
                    IP = IpAdress,
                    UserID = GlobleData.UserName,
                    CreatedDate = DateTime.Now

                });


                clsWriteLogThuVienLib.WriteLog_BOM_DM(URL, _clientExtension, lstLogSave);
            }
            catch (Exception ex)
            {

            }

        }






        #region Phú sửa BOM
        private void toggleIsActive_Toggled(object sender, EventArgs e)
        {
            toggleIsActive.Properties.Appearance.ForeColor = toggleIsActive.IsOn ? Color.ForestGreen : Color.DimGray;
            bool isActive = toggleIsActive.IsOn;
            DoActive(isActive);

        }
        private void toggleIsActive_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            bool isOn = Convert.ToBoolean(e.NewValue);
            if (!_isDuyet & isOn)
            {
                XtraMessageBox.Show($"Không thể active vì đợt chưa được duyệt.Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }

        }

        private void CreateRepoSearchLookUpChungLoaiCT()
        {
            try
            {
                repoSearchCLCT.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT.ValueMember = "MaNhomChiTiet";
                searchLookUpEditCLCT.Properties.DisplayMember = "TenNhomChiTiet";
                searchLookUpEditCLCT.Properties.ValueMember = "MaNhomChiTiet";
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tblChungLoaiChiTiet;
                searchLookUpEditCLCT.Properties.DataSource = tblChungLoaiChiTiet;

            }
            catch (Exception ex)
            {
            }
        }

        private void repositoryItemSearchLookUpEdit1View_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {



            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null)
            {
                return;
            }
            string tenNhom = dr["TenNhom"].ToString();
            if (string.IsNullOrEmpty(tenNhom))
                return; // Không có điều kiện ⇒ hiển thị toàn bộ

            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            var value = Convert.ToString(
                view.GetListSourceRowCellValue(e.ListSourceRow, "TenNhom")
            );

            if (!string.Equals(value, tenNhom, StringComparison.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }
        private void DoActive(bool IsActive)
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            if (madot.ToString() == "") return;
            int active = IsActive ? 1 : 0;
            string url = $"{URL}KhoiTaoBOMV1/Post1?action=DoActive&para1={active}&para2={makh}&para3={mahang}&para4={madot}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if (msResult.ToLower() != "true") return;
            else
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }
        private void gridView7_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {

            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;

            string manhonm = dr["MaNhom"].ToString();
            string tenNhom = dr["TenNhom"].ToString();
            if (string.IsNullOrEmpty(tenNhom))
                return; // Không có điều kiện ⇒ hiển thị toàn bộ

            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            var value = Convert.ToString(
                view.GetListSourceRowCellValue(e.ListSourceRow, "TenNhom")
            );

            if (!string.Equals(value, tenNhom, StringComparison.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }


        }
        private void searchLookUpEditCLCT_QueryPopUp(object sender, CancelEventArgs e)
        {
            int[] selectedRowHandles = bandedGridView1.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;
            List<DataRow> rowsFromTbl = new List<DataRow>();
            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle < 0) continue;
                if (!bandedGridView1.IsValidRowHandle(rowHandle)) continue;
                DataRow drSL = bandedGridView1.GetDataRow(rowHandle);
                if (drSL != null)
                    rowsFromTbl.Add(drSL);
            }
            if (rowsFromTbl.Count == 0) return;
            string maNhomDau = rowsFromTbl[0].Field<string>("MaNhom");
            bool allSame = rowsFromTbl.All(r => string.Equals(r.Field<string>("MaNhom"), maNhomDau, StringComparison.OrdinalIgnoreCase));

            if (!allSame)
            {
                XtraMessageBox.Show(this, "Các dòng được chọn phải chung một chủng loại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

        }
        private void searchLookUpEditCLCT_EditValueChanged(object sender, EventArgs e)
        {
            int[] selectedRowHandles = bandedGridView1.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;

            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle < 0) continue;
                if (!bandedGridView1.IsValidRowHandle(rowHandle)) continue;
                DataRow drSL = bandedGridView1.GetDataRow(rowHandle);
                if (drSL != null)
                    drSL["MaNhomChiTiet"] = searchLookUpEditCLCT.EditValue.ToString();
                bandedGridView1.RefreshRow(rowHandle);
                bandedGridView1_CellValueChanged(bandedGridView1, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(rowHandle, bandedGridView1.Columns["MaNhomChiTiet"], drSL["MaNhomChiTiet"]));

            }
            this.ActiveControl = button1;
        }

        private void createTableCT(DataTable tab)
        {
            DataTable tbl = gridControl1.DataSource as DataTable;
            GridBand parentBand = bandedGridView1.Bands["gridBandMauSP"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView1.Columns.Count;)
                {
                    if (bandedGridView1.Columns[i].FieldName.Contains("gridBandMauSPa"))
                    {
                        bandedGridView1.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }

            foreach (DataRow column in tab.Rows)
            {
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = column[4].ToString();
                col.FieldName = column[4].ToString() + "@Mau@" + column[1].ToString();
                col.Name = "col" + column[4].ToString();
                // col.OptionsColumn.AllowEdit = false; // REMOVE DÒNG NÀY!
                col.Visible = true;
                col.Width = 50;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                // GÁN EDITOR TRƯỚC KHI ADD COLUMN
                col = CreateSearchLookUpMauVT(col);
                bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });

                GridBand gb = new GridBand();
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.Caption = column[4].ToString();
                gb.Columns.Add(col);
                gb.Name = "gridBandMauSPa" + col;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gridBandMauSP.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                if (tbl != null)
                {
                    tbl.Columns.Add(column[1].ToString(), typeof(string));
                }

            }
            gridControl1.DataSource = tbl;
        }



        private BandedGridColumn CreateSearchLookUpMauVT(BandedGridColumn col)
        {
            try
            {
                // KIỂM TRA DATA SOURCE
                if (tblMauVT == null || tblMauVT.Rows.Count == 0)
                {
                    MessageBox.Show("tblMauVT chưa có dữ liệu!");
                    return col;
                }

                RepositoryItemButtonEdit btnEdit = new RepositoryItemButtonEdit();
                btnEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

                btnEdit.Buttons.Clear();
                DevExpress.XtraEditors.Controls.EditorButton button =
            new DevExpress.XtraEditors.Controls.EditorButton(
                DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);

                button.ImageOptions.Image = Properties.Resources.group_16x16;


                btnEdit.Buttons.Add(button);


                gridControl1.RepositoryItems.Add(btnEdit);

                btnEdit.ButtonClick += (s, e) =>
                {
                    DataRow dr = bandedGridView1.GetFocusedDataRow();
                    if (dr == null) return;
                    frmPhanTichBOM_ChonMau frm = new frmPhanTichBOM_ChonMau(dr);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        dr["MauVTIDChung"] = frm._mauvtidchung;
                        LoadTVMauVT();
                        DataTable tbl = frm.tblGrid;
                        if (tbl == null || tbl.Rows.Count == 0) return;
                        foreach (DataRow r in tbl.Rows)
                        {
                            string columnName = r["MauSPID"].ToString();
                            string mauVTID = r["MauVTID"].ToString();
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                if (dc.ColumnName == columnName)
                                {
                                    dr[columnName] = mauVTID;
                                }
                            }

                        }

                    }
                    this.ActiveControl = button1;
                };

                col.ColumnEdit = btnEdit;

                return col;
            }
            catch (Exception ex)
            {

                return col;
            }
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                using (var frm = new frmImportEXBOM())
                {
                    var result = frm.ShowDialog();

                    if (result == DialogResult.OK)
                    {

                        loadSizeChung();
                        CreateSearchLookUpDot();
                        CreateMauSPColumns();
                        LoadData();
                    }
                    else if (result == DialogResult.No)
                    {

                        this.Close();
                        return;
                    }
                    loadSizeChung();
                    CreateSearchLookUpDot();
                    CreateMauSPColumns();
                    LoadData();

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateMauSPColumns()
        {
            if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue.ToString() == "" || searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue.ToString() == "")
            {
                return;
            }
            string makh = searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUSPCOLUMNS&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblMauSP = JsonConvert.DeserializeObject<DataTable>(json);
            createTableCT(_tblMauSP);
        }

        private void repositoryItemSearchLookUpEditSizeSP_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var repo = sender as DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit;
            var popupView = repo?.View as DevExpress.XtraGrid.Views.Grid.GridView;
            if (popupView == null) return;

            var rows = popupView.GetSelectedRows()
                .Select(handle => popupView.GetDataRow(handle))
                .Where(r => r != null)
                .ToList();
            if (rows.Count == 0) return;

            var text = string.Join("; ",
                rows.GroupBy(r => r["NhomSize"])
                    .Select(g => $"{g.Key}:{string.Join(",", g.Select(r => r["TenSize"]))}"));
            e.DisplayText = text;
        }

        private void LoadTVMauVT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUVTTV";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);
        }

        private void bandedGridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains("@Mau@"))
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    string mauVTID = e.Value.ToString();


                    DataRow[] rows = tblMauVT.Select($"MauVTID = '{mauVTID}'");
                    if (rows.Length > 0)
                    {
                        e.DisplayText = rows[0]["MaMauVT"].ToString();
                    }
                }
            }
        }



        private void btnChonSize_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRow row = bandedGridView1.GetFocusedDataRow();
                if (row == null) return;
                if (row.Table.Columns.Contains("IsXetDuyet"))
                {
                    bool IsXetDuyet = false;
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                    if (IsXetDuyet)
                    {
                        XtraMessageBox.Show("Vật tư đã được xét duyêt. Không thể chỉnh sửa Size sản phẩm. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }

                }
                frmPhanTichBOM_ChonSize frm = new frmPhanTichBOM_ChonSize(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), row["MaSizeChung"].ToString());
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK) return;
                string result = frm.SelectedResult;
                row["Size"] = frm.SelectedResultName;
                row["MaSizeChung"] = frm.SelectedResult;
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {


            }
        }

        private void btnNapLaiCLCT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CreateRepoSearchLookUpChungLoaiCT();
            NapLai(!isAdd);
        }
        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            // Tô màu cả dòng khi MaVT rỗng hoặc chỉ toàn khoảng trắng
            var maVT = view.GetRowCellValue(e.RowHandle, "MaVT") as string;
            if (string.IsNullOrWhiteSpace(maVT))
            {
                e.Appearance.BackColor = Color.MistyRose;
                e.Appearance.ForeColor = Color.DarkRed;
                //e.HighPriority = true; // đảm bảo màu này không bị override bởi rule khác
                return;
            }
            if (e.Column.FieldName == "IsXetDuyet")
            {
                // Lấy giá trị của ô hiện tại trong cột đang xét
                object cellValue = e.CellValue;

                if (cellValue != null)
                {
                    if (cellValue.ToString() == "Đã duyệt")
                    {

                        e.Appearance.ForeColor = Color.Green;
                    }
                    else
                    {

                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
        }



        private void bandedGridView1_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (bandedGridView1.FocusedColumn == null)
                    return;
                DataRow row = bandedGridView1.GetFocusedDataRow();
                if (row == null) return;
                if (row.Table.Columns.Contains("IsXetDuyet"))
                {
                    bool IsXetDuyet = false;
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                    if (IsXetDuyet)
                    {
                        XtraMessageBox.Show("Đợt đã được xét duyệt. Không thể chỉnh sửa. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        this.ActiveControl = button1;
                        return;
                    }

                }
                if ((bandedGridView1.FocusedColumn.FieldName == "MaVT" || bandedGridView1.FocusedColumn.FieldName == "Size" || bandedGridView1.FocusedColumn.FieldName.Contains("@Mau@")) && bandedGridView1.ActiveEditor is ButtonEdit buttonEdit)
                {
                    gridControl1.BeginInvoke(new Action(() =>
                    {
                        buttonEdit.PerformClick(buttonEdit.Properties.Buttons[0]);
                    }));
                }
                else if (bandedGridView1.FocusedColumn.FieldName == "MaNhomChiTiet")
                {

                    gridControl1.BeginInvoke(new Action(() =>
                    {
                        if (bandedGridView1.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                        {
                            if (!searchLookUpEdit.IsPopupOpen)
                            {
                                searchLookUpEdit.ShowPopup();
                            }
                        }
                    }));

                }
            }
            catch (Exception ex)
            {


            }
        }


        private void btnXoaMauVT_Click(object sender, EventArgs e)
        {

            DataTable tbl = gridControl1.DataSource as DataTable;
            var view = bandedGridView1;
            GridCell[] selected = view.GetSelectedCells();
            if (selected.Length == 0) return;
            view.BeginUpdate();
            try
            {
                foreach (var cell in selected)
                {
                    if (!view.IsDataRow(cell.RowHandle)) continue;
                    if (!cell.Column.OptionsColumn.AllowEdit) continue;
                    view.SetRowCellValue(cell.RowHandle, cell.Column, string.Empty);
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }
        private void loadSizeChung()
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP&para1={makh.ToString()}&para2={mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblSizeChung = JsonConvert.DeserializeObject<DataTable>(json);

            }
        }



        private void btnXuatMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("BOM{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "BOMTemplate.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName);

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
        public void Export(string TemplateFileName, string ExportFileName)
        {
            try
            {


                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "BOM";

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {

                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    worksheet.Cells["C3"].Value = "";
                    worksheet.Cells["C3"].Style.Font.Bold = true;
                    worksheet.Cells["C3"].Style.Font.Size = 12;
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

        }
        #endregion


        private void btnChonVatTu_Click(object sender, EventArgs e)
        {
            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string dot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            if (dot == "" && textBox1.Text == "")
            {
                return;
            }
            DataTable tbl = gridControl1.DataSource as DataTable;
            frmPhanTichBOM_ChonVatTu frm = new frmPhanTichBOM_ChonVatTu(kh, mh, dot, tbl);
            if (frm.ShowDialog() == DialogResult.OK)
            {

                List<DataRow> lstSelect = frm.lstSelect.OrderBy(row => Convert.ToInt32(row["STT"])).ToList();
                foreach (DataRow row in lstSelect)
                {
                    AddRowGCNL(row);
                }
            }
        }
        private void addRowEmpty(List<DataRow> lst, int isNPL, DataTable tbl)
        {
            DataRow newRow = bandedGridView1.GetFocusedDataRow();
            if (newRow == null) return;
            DataRow firstRow = lst?.FirstOrDefault();
            if (firstRow == null)
                return;

            newRow["MaNhom"] = firstRow["MaNhom"];
            newRow["TenNhom"] = firstRow["TenNhom"];
            newRow["NPL"] = firstRow["NPL"];
            newRow["Sort"] = firstRow["Sort"];
            newRow["MaVTID"] = firstRow["MaVTID"];
            newRow["MaVT"] = firstRow["MaVT"];
            newRow["ChiTiet"] = firstRow["VatTu"];
            newRow["MaDVVT"] = firstRow["MaDVVT"];
            newRow["TenDVVT"] = firstRow["TenDVVT"];
            newRow["KhoVaiID"] = firstRow["KhoVaiID"];
            newRow["KhoVai"] = firstRow["KhoVai"];
            //newRow["STT"] = isNL ? _sttVT : _sttVTPL;
            newRow["DinhMucChung"] = 0;
            newRow["DinhMucHaoHut"] = 0;
            newRow["TachMau"] = false;
            newRow["IsNew"] = 1;

            newRow["STTCode"] = 0;
            newRow["MaCode"] = firstRow["MaVTID"].ToString() + "|" + "0";
            newRow["IsActive"] = false;

            // Logic tìm kiếm và gán MaNhomChiTiet
            string maNhomChiTiet = "";
            bool nplValue = Convert.ToBoolean(firstRow["NPL"]);

            if (nplValue)
            {
                // NPL = true: Tìm theo MaNhom
                string filterChungLoai = string.Format("MaNhom = '{0}'", firstRow["MaNhom"]);
                DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                // Chỉ gán nếu tìm được đúng 1 dòng
                if (foundRows.Length == 1)
                {
                    maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                }
            }
            else
            {
                // NPL = false: Tìm theo TenNhom = TenNhomChiTiet
                string filterChungLoai = string.Format("TenNhomChiTiet = '{0}'", firstRow["TenNhom"].ToString().Replace("'", "''"));
                DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                // Gán nếu tìm được ít nhất 1 dòng (lấy dòng đầu tiên)
                if (foundRows.Length > 0)
                {
                    maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                }
            }

            newRow["MaNhomChiTiet"] = maNhomChiTiet;
            newRow["MauVTIDChung"] = firstRow["MauVTIDChung"].ToString();
            newRow["Size"] = "";
            foreach (DataColumn col in newRow.Table.Columns)
            {
                if (col.ColumnName.Contains("@Mau@"))
                {
                    newRow[col.ColumnName] = "";
                }
            }
            //tbl.Rows.Add(newRow);

            // gridControl1.DataSource = tbl;

        }
        private void AddRowGCNL(DataRow rowAdd)
        {
            try
            {
                if (rowAdd == null) return;
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null)
                {
                    tbl = createTableSaveEdit();
                }
                string filter = string.Format("MaVTID = '{0}' AND KhoVaiID = '{1}' AND MaNhom = '{2}' AND MaDVVT = '{3}'",
                                              rowAdd["MaVTID"], rowAdd["KhoVaiID"], rowAdd["MaNhom"], rowAdd["MaDVVT"]);
                DataRow[] existingRows = tbl.Select(filter);
                if (existingRows.Length == 0)
                {
                    DataRow newRow = tbl.NewRow();
                    bool isNL = rowAdd["NPL"].ToString() == "True" ? true : false;
                    newRow["MaNhom"] = rowAdd["MaNhom"];
                    newRow["TenNhom"] = rowAdd["TenNhom"];
                    newRow["NPL"] = rowAdd["NPL"];
                    newRow["Sort"] = rowAdd["Sort"];
                    newRow["MaVTID"] = rowAdd["MaVTID"];
                    newRow["MaVT"] = rowAdd["MaVT"];
                    newRow["ChiTiet"] = rowAdd["VatTu"];
                    newRow["MaDVVT"] = rowAdd["MaDVVT"];
                    newRow["TenDVVT"] = rowAdd["TenDVVT"];
                    newRow["KhoVaiID"] = rowAdd["KhoVaiID"];
                    newRow["KhoVai"] = rowAdd["KhoVai"];
                    newRow["STT"] = isNL ? _sttVT : _sttVTPL;
                    newRow["DinhMucChung"] = 1;
                    newRow["DinhMucHaoHut"] = 0;
                    newRow["TachMau"] = false;
                    newRow["IsNew"] = 1;


                    newRow["STTCode"] = isNL ? _sttVT : _sttVTPL;

                    //newRow["STTCode"] = 0;
                    newRow["MaCode"] = rowAdd["MaVTID"].ToString() + "|" + (isNL ? _sttVT : _sttVTPL).ToString();
                    newRow["IsActive"] = false;

                    // Logic tìm kiếm và gán MaNhomChiTiet
                    string maNhomChiTiet = "";
                    bool nplValue = Convert.ToBoolean(rowAdd["NPL"]);

                    if (nplValue)
                    {
                        // NPL = true: Tìm theo MaNhom
                        string filterChungLoai = string.Format("MaNhom = '{0}'", rowAdd["MaNhom"]);
                        DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                        // Chỉ gán nếu tìm được đúng 1 dòng
                        if (foundRows.Length == 1)
                        {
                            maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                        }
                    }
                    else
                    {
                        // NPL = false: Tìm theo TenNhom = TenNhomChiTiet
                        string filterChungLoai = string.Format("TenNhomChiTiet = '{0}'", rowAdd["TenNhom"].ToString().Replace("'", "''"));
                        DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                        // Gán nếu tìm được ít nhất 1 dòng (lấy dòng đầu tiên)
                        if (foundRows.Length > 0)
                        {
                            maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                        }
                    }

                    newRow["MaNhomChiTiet"] = maNhomChiTiet;
                    newRow["MauVTIDChung"] = rowAdd["MauVTIDChung"].ToString();
                    newRow["Size"] = "";
                    string[] arrmauvtid = rowAdd["MauVTIDChung"].ToString().Split(',');
                    if(arrmauvtid.Length==1)
                    {
                        foreach(DataColumn dc in newRow.Table.Columns)
                        {
                            if(dc.ColumnName.Contains("@Mau@"))
                            {
                                newRow[dc.ColumnName] = arrmauvtid[0];
                            }    
                        }    
                    }    
                    tbl.Rows.Add(newRow);
                    if (isNL)
                    {
                        _sttVT++;
                    }
                    else
                    {
                        _sttVTPL++;
                    }
                    gridControl1.DataSource = tbl;

                }
            }
            catch (Exception ex)
            {

            }
        }



        private void ExportEx_btn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BandedGridView bandedView = gridControl1.MainView as BandedGridView;
            bandedView.OptionsPrint.PrintBandHeader = true;
            bandedView.OptionsPrint.PrintHeader = false;

            if (bandedView == null || bandedView.DataRowCount == 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có dữ liệu!!!");
            }
            else
            {
                using (SaveFileDialog Sfd = new SaveFileDialog())
                {
                    Sfd.Title = "File To Save";
                    Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
                    string mahang = searchLookUpEditMH.Text.ToString();
                    Sfd.FileName = string.Format("{0}-BOM-{1}", mahang, DateTime.Now.Millisecond.ToString());
                    if (Sfd.ShowDialog() == DialogResult.OK)
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                        exportFileExcel(bandedView, Sfd.FileName);
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
            }
        }


        private void exportFileExcel(BandedGridView bandedView, string filePath)
        {
            try
            {
                XlsxExportOptionsEx options = new XlsxExportOptionsEx()
                {
                    ExportType = DevExpress.Export.ExportType.WYSIWYG,

                    AllowGrouping = DevExpress.Utils.DefaultBoolean.True,
                    ShowBandHeaders = DevExpress.Utils.DefaultBoolean.False,
                    ShowGridLines = true,
                    SheetName = "Danh sách BOM"
                };
                gridControl1.ExportToXlsx(filePath, options);
                Workbook wb = new Workbook();
                wb.LoadDocument(filePath);
                Worksheet sheet = wb.Worksheets[0];
                int headerTableIndex = 6;
                int startStaticHeader = 11;

                sheet.Rows.Insert(0, headerTableIndex);
                sheet.Columns[startStaticHeader - 1].WidthInCharacters = 20;
                sheet.Columns[startStaticHeader - 1].Alignment.WrapText = true;


                int rangeLeft = sheet.GetUsedRange().LeftColumnIndex;
                int rangeRight = sheet.GetUsedRange().RightColumnIndex;
                int rangeBottom = sheet.GetUsedRange().BottomRowIndex;
                int rangeTop = headerTableIndex;

                int currentCol = 0;
                int currentRow = 0;

                Range headerRange = sheet.Rows[headerTableIndex];
                Range beginHeaderRange = sheet.Range.FromLTRB(rangeLeft, rangeTop, rangeLeft + startStaticHeader - 1, rangeTop);
                Range colorRange = sheet.Range.FromLTRB(rangeLeft + startStaticHeader, rangeTop, rangeRight - 2, rangeTop + 1);
                Range gridRange = sheet.Range.FromLTRB(rangeLeft, rangeTop, rangeRight, rangeBottom);

                beginHeaderRange.FillColor = Color.FromArgb(255, 228, 179);
                colorRange.FillColor = Color.FromArgb(0, 218, 218);
                sheet.Cells[rangeTop, rangeRight].FillColor = Color.FromArgb(255, 228, 179);
                sheet.Cells[rangeTop, rangeRight - 1].FillColor = Color.FromArgb(255, 228, 179);

                sheet.Range["A2:D2"].Merge();
                sheet.Range["A3:D3"].Merge();
                sheet.Range["A4:D4"].Merge();
                sheet.Range["A5:D5"].Merge();
                sheet.Range["A6:D6"].Merge();

                sheet.Cells["A2"].Value = "Khách hàng";
                sheet.Cells["E2"].Value = searchLookUpEditKH.Text;
                sheet.Cells["A3"].Value = "Mã hàng";
                sheet.Cells["E3"].Value = searchLookUpEditMH.Text;
                sheet.Cells["A4"].Value = "Đợt";
                if (!isAdd)
                    sheet.Cells["E4"].Value = searchLookUpEditDot.Text;
                else
                    sheet.Cells["E4"].Value = textBox1.Text;
                //sheet.Cells["A5"].Value = "Chủng loại CT";
                //sheet.Cells["E5"].Value = searchLookUpEditCLCT.Text;

                sheet.Range["A1:A5"].Font.Bold = true;
                headerRange.Font.Bold = true;

                currentRow = headerTableIndex + 2;
                currentCol = startStaticHeader - 1;
                while (currentRow <= rangeBottom + 1)
                {
                    Cell cell = sheet.Cells[currentRow, currentCol];
                    string txt = cell.DisplayText;
                    if (string.IsNullOrEmpty(txt))
                    {
                        currentRow++;
                        continue;
                    }

                    // xử lý: xuống dòng tại dấu phẩy gần giới hạn
                    StringBuilder sb = new StringBuilder();
                    int lastBreak = 0;
                    for (int i = 0; i < txt.Length; i++)
                    {
                        if (i - lastBreak >= 20)
                        {
                            int commaPos = sb.ToString().LastIndexOf(',', i);
                            if (commaPos > lastBreak && commaPos < sb.Length - 1)
                            {
                                sb.Insert(commaPos + 1, Environment.NewLine);
                                lastBreak = commaPos + 2; // +2 để bỏ qua newline
                            }
                        }
                    }

                    string newText = sb.Length > 0 ? sb.ToString() : txt;
                    cell.Value = newText;
                    cell.Alignment.WrapText = true;
                    currentRow++;
                }

                currentRow = headerTableIndex + 2;
                currentCol = 0;
                bool wroteNL = false;
                for (int i = sheet.Pictures.Count - 1; i >= 0; i--)
                {
                    Picture pic = sheet.Pictures[i];
                    if (pic.TopLeftCell.ColumnIndex == 0) // cột A = 0
                    {
                        sheet.Pictures.RemoveAt(i);
                    }
                }
                while (currentRow <= rangeBottom + 1)
                {
                    Color NPLCell = sheet.Cells[currentRow, currentCol].FillColor;
                    Color nhomCell = sheet.Cells[currentRow, currentCol + 1].FillColor;
                    Color vattuCell = sheet.Cells[currentRow, currentCol + 2].FillColor;
                    Cell sttCell = sheet.Cells[currentRow, currentCol + 3];
                    if (NPLCell != Color.Empty && !wroteNL)
                    {
                        sheet.Cells[currentRow, currentCol].Value = "Nguyên liệu";
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Blue;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                        wroteNL = true;
                    }
                    else if (NPLCell != Color.Empty && wroteNL)
                    {
                        sheet.Cells[currentRow, currentCol].Value = "Phụ liệu";
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Blue;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    else if (nhomCell != Color.Empty)
                    {
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Red;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    else if (vattuCell != Color.Empty)
                    {
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.IndianRed;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    else if (sttCell.Value.Type == CellValueType.Numeric)
                    {
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Black;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    currentRow++;
                }

                currentCol = rangeLeft + startStaticHeader;
                foreach (GridBand band in bandedView.Bands)
                {
                    if (band.Caption != null && band.Caption.Contains("Màu sản phẩm"))
                    {
                        foreach (GridBand colorCol in band.Children)
                        {
                            if (colorCol.Columns.Count > 0)
                            {
                                var col = colorCol.Columns[0];
                                string fieldName = col.FieldName;
                                string headerText = col.Caption;
                                currentRow = headerTableIndex + 2;
                                for (int i = 0; i < bandedView.RowCount; i++)
                                {
                                    int rowHandle = bandedView.GetVisibleRowHandle(i);
                                    if (!bandedView.IsDataRow(rowHandle)) continue;
                                    object id = bandedView.GetRowCellValue(rowHandle, col);
                                    string display = bandedView.GetRowCellDisplayText(rowHandle, col);
                                    //while (currentRow < 50 && (string.IsNullOrEmpty(sheet.Cells[currentRow, currentCol].DisplayText?.Trim()) || sheet.Cells[currentRow, currentCol].IsMerged))
                                    while (currentRow <= rangeBottom + 1 && sheet.Cells[currentRow, currentCol].DisplayText?.Trim() != id.ToString())
                                    {
                                        currentRow++;
                                    }
                                    sheet.Cells[currentRow, currentCol].Value = display;
                                    currentRow++;
                                }
                            }
                            currentCol++;
                        }
                    }
                    //else if (band.Caption != null && band.Caption.Contains("STT"))
                    //{
                    //    foreach(GridBand sttTitle in band.Children)
                    //    {
                    //        if (sttTitle.Columns.Count > 0)
                    //        {
                    //            var c = sttTitle.Columns;
                    //        }
                    //    }
                    //    int q = 0;
                    //}
                }

                gridRange.AutoFitColumns();
                gridRange.AutoFitRows();
                for (int i = gridRange.TopRowIndex; i <= gridRange.BottomRowIndex; i++)
                {
                    Row row = sheet.Rows[i];
                    row.Height *= 1.3;
                }
                for (int j = gridRange.LeftColumnIndex; j <= gridRange.RightColumnIndex; j++)
                {
                    Column col = sheet.Columns[j];
                    col.Width += 5;
                }
                wb.SaveDocument(filePath);

                //XtraMessageBox.Show("Đã xuất file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi xuất file:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
            }

        }
        private void simpleButton1_Click_2(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn khách hàng", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn mã hàng", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditDot.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn đợt", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string makh = searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue.ToString();
            frmXemDMLenh frm = new frmXemDMLenh(makh, mahang, madot);
            frm.ShowDialog();
        }
        private void loadKhoSizeBOM()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETKHOSIZEBOM";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblKhoSize = JsonConvert.DeserializeObject<DataTable>(json);
            repoKhoSize.DisplayMember = "KhoVai";
            repoKhoSize.ValueMember = "KhoVaiID";
            repoKhoSize.DataSource = _tblKhoSize;
        }
        private void repoItemcode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string dot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;
            int isNPLDisplay = dr["NPL"].ToString() == "True" ? 1 : 2;
            DataTable tbl = gridControl1.DataSource as DataTable;
            frmPhanTichBOM_ChonVatTu frm = new frmPhanTichBOM_ChonVatTu(kh, mh, dot, tbl, isNPLDisplay);
            if (frm.ShowDialog() == DialogResult.OK)
            {

                //List<DataRow> lstSelect = frm.lstSelect.OrderBy(row => Convert.ToInt32(row["STT"])).ToList();
                addRowEmpty(frm.lstSelect, isNPLDisplay, tbl);
            }
            this.ActiveControl = button1;
        }
        private void ThemDong(object sender, EventArgs e)
        {
            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                DataTable table = gridControl1.DataSource as DataTable;
                if (table == null) return;

                DataRow currentRow = view.GetDataRow(rowHandle);
                if (currentRow == null) return;

                string currentMaVTID = currentRow["MaVTID"].ToString();

                string currentKhoVaiID = currentRow["KhoVaiID"].ToString();
                bool isNL = currentRow["NPL"].ToString() == "True" ? true : false;
                int currentSTT = Convert.ToInt32(currentRow["STT"]);
                int newSTTP = currentSTT + 1;
                // Tìm STT lớn nhất với MaVTID hiện tại


                // Tạo dòng mới
                DataRow newRow = table.NewRow();


                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 0;
                newRow["STT"] = newSTTP;
                newRow["NPL"] = currentRow["NPL"];
                var rowsToUpdate = table.AsEnumerable()
           .Where(r => r["NPL"].ToString() == isNL.ToString() && Convert.ToInt32(r["STT"]) >= newSTTP)
           .ToList();

                // Tăng STT của các dòng phía sau lên 1
                foreach (DataRow row in rowsToUpdate)
                {
                    row["STT"] = Convert.ToInt32(row["STT"]) + 1;
                }
                if (isNL)
                {

                    _sttVT++;
                }
                else
                {

                    _sttVTPL++;

                }
                int insertIndex = view.GetDataSourceRowIndex(rowHandle) + 1;
                table.Rows.InsertAt(newRow, insertIndex);
                view.RefreshData();

            }
            this.ActiveControl = button1;
        }

        private void gridView1_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null)
                return;
            string mavtid = dr["MaVTID"].ToString();
            string manhom = dr["MaNhom"].ToString();

            if (string.IsNullOrEmpty(manhom) || string.IsNullOrEmpty(mavtid))
                return;
            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            string listMaNhom = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "MaNhom"));
            string listMaVTID = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "MaVTID"));
            if (!string.Equals(listMaNhom, manhom, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(listMaVTID, mavtid, StringComparison.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }
        private void repoKhoSize_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;
            var oldValue = e.OldValue;
            var newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            string maNhom = dr["MaNhom"].ToString();
            string maVTID = dr["MaVTID"].ToString();
            string khovaiid = newValue.ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHECKKHOSIZE&para1={maNhom}&para2={maVTID}&para3={khovaiid}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblMauVTIDNew = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow checkMau = tblMauVTIDNew.AsEnumerable().FirstOrDefault();
            if (checkMau == null) return;
            string mauVTIDChung = checkMau["MauVTIDChung"]?.ToString() ?? "";
            bool isMau = true;
            foreach (DataColumn col in dr.Table.Columns)
            {
                if (col.ColumnName.Contains("@Mau@"))
                {
                    string giaTriCot = dr[col.ColumnName]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(giaTriCot) && !mauVTIDChung.Contains(giaTriCot))
                    {
                        isMau = false;
                        break;
                    }
                }
            }

            var maxSTT = (gridControl1.DataSource as DataTable).AsEnumerable()
                                  .Where(r => r["MaNhom"].ToString() == maNhom && r["KhoVaiID"].ToString() == khovaiid && r["MaVTID"].ToString() == maVTID && int.TryParse(r["STTCode"].ToString(), out _))
                                  .Select(r => Convert.ToInt32(r["STT"]))
                                  .DefaultIfEmpty(0)
                                  .Max();

            int newSTT = maxSTT + 1;
            dr["STTCode"] = newSTT;
            if (isMau)
            {
                return;
            }
            else
            {
                DialogResult result = MessageBox.Show(
              "Lưu ý: màu vật tư bạn chọn sẽ đc làm mới. Bạn có chắc chắn muốn thay đổi không?",
              "Xác nhận",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Question
          );

                // Nếu chọn No thì hủy thay đổi
                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Hủy việc thay đổi giá trị
                }
                else if (result == DialogResult.Yes)
                {
                    foreach (DataColumn col in dr.Table.Columns)
                    {
                        if (col.ColumnName.Contains("@Mau@"))
                        {
                            dr[col.ColumnName] = "";
                        }
                    }

                }
            }

        }

    }
}
