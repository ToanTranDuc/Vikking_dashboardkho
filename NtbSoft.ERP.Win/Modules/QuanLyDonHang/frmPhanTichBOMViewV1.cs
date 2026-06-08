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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBOMViewV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _checkUserName = false;
        private int _rowhandle = 0, _stt = 0;
        private bool isAdd = false;
        DataTable _dtSize;
        private string _madot = string.Empty;
        DataTable _tblmhvt;
        private bool isReset = false;
        public frmPhanTichBOMViewV1(string makh, string mahang, string madot)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            _dtSize = new DataTable();
            searchLookUpEditKH.EditValue = makh;
            searchLookUpEditMH.EditValue = mahang;
            searchLookUpEditDot.EditValue = madot;
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookup();
            CheckUserXetDuyet();
            bandedGridView1.OptionsView.AllowCellMerge = true;
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
          
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
                gridControl2.DataSource = new DataTable();
            }
            else
            {
                if (isSua) return;
                searchLookUpEditDot.EditValue = tblDot.Rows[0][0];
                btnCopyBOM.Enabled = true;
            }

        }
        private void CheckUserXetDuyet()
        {
            string url = $"{URL}UpdateXetDuyet/Get?Action=GETPhanQuyen&para={GlobleData.UserName}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                // layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            else _checkUserName = true;
        }
        private void LoadData()
        {
            string url = string.Format("{0}?makh={1}&&mahang={2}&&madot={3}", URL + "KhoiTaoDM/GetKTBom", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                     searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl.Rows.Count > 0)
            {
                gridControl1.DataSource = tbl;
                gridView1.FocusedRowHandle = _rowhandle;
                //if (tbl.Rows[0]["IsXetDuyet"].ToString() == "Đã duyệt")
                //    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //else
                //    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
            }

        }
        private void CreateSearchLookUpVT()
        {
            //string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            //string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTU&para1={kh.ToString()}";
            //string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
            //DataTable tblVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
            //searchLookUpEditVatTu.Properties.DataSource = tblVT;
            //searchLookUpEditVatTu.Properties.ValueMember = "MaVTID";
            //searchLookUpEditVatTu.Properties.DisplayMember = "MaVT";
            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            //string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTU&para1={kh.ToString()}";
            string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTUMH&para1={kh.ToString()}";
            string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
            DataTable tblVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
            _tblmhvt = tblVT;
            if (!tblVT.Columns.Contains("SortIndex"))
                tblVT.Columns.Add("SortIndex", typeof(int));

            string firstGroup = null;
            foreach (DataRow row in tblVT.Rows)
            {
                string maHangFirst = row["MaHangFirst"]?.ToString();
                if (!string.IsNullOrWhiteSpace(maHangFirst))
                {
                    firstGroup = maHangFirst;
                    break;
                }
            }


            var groupOrder = new Dictionary<string, int>();
            int currentSortIndex = 2;

            foreach (DataRow row in tblVT.Rows)
            {
                string maHangFirst = row["MaHangFirst"]?.ToString();

                if (!string.IsNullOrWhiteSpace(maHangFirst))
                {
                    if (maHangFirst == firstGroup)
                    {
                        row["SortIndex"] = 0;
                    }
                    else
                    {
                        if (!groupOrder.ContainsKey(maHangFirst))
                        {
                            groupOrder[maHangFirst] = currentSortIndex++;
                        }
                        row["SortIndex"] = groupOrder[maHangFirst];
                    }
                }
                else
                {
                    row["SortIndex"] = 1;
                }
            }



            searchLookUpEditVatTu.Properties.DataSource = tblVT;
            searchLookUpEditVatTu.Properties.ValueMember = "MaVTID";
            searchLookUpEditVatTu.Properties.DisplayMember = "MaVT";


            gridView2.Columns["SortIndex"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
            gridView2.Columns["SortIndex"].GroupIndex = 0;

            gridView2.Columns["TenNhom"].GroupIndex = 1;
            gridView2.Columns["TenNhom"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
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
                searchLookUpEditMH.EditValue = null;
                if (isAdd)
                    CreateSearchLookUpVT();
            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {


                if (!isAdd)
                    CreateSearchLookUpDot();
                else
                {
                    CreateDotSTT();
                    LoadSizeDM();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            GridView view = gridView1;
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

        private void repositoryItemButtonEdit2_Click(object sender, EventArgs e)
        {
            GridView view = gridView1;
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
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&mauID={4}", URL + "PhanTichBom/GetMauSanPham", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID.ToString(), MauID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count > 0)
            {
                var result = tbl.AsEnumerable().Where(row => row["CheckMau"].ToString() == "1").FirstOrDefault();
                if (result != null)
                {
                    frmDauSizeSize frm = new frmDauSizeSize(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                    searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID.ToString(), MauID.ToString());
                    frm.ShowDialog();
                    LoadChiTiet();
                }
                else
                {
                    XtraMessageBox.Show("Vui lòng chọn Màu sản phẩm trước khi chọn InSeam/Size.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
            }
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

            GridView view = gridView1;
            object MaVTID = null, MauID = null, MaKhoVT = null, TachMau = null, MaNhom = null, MaCode = null;

            int levelGroup = gridView1.GetRowLevel(gridView1.FocusedRowHandle);
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
                gridControl2.DataSource = null;
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

                DataRow dr = gridView1.GetFocusedDataRow();
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

                createTable(_dtSize);
                checkBox1.CheckedChanged -= checkBox1_CheckedChanged;

                checkBox1.Checked = (bool)dr["TachMau"];
                checkBox1.CheckedChanged += checkBox1_CheckedChanged;
                //if (!newTbl.Columns.Contains("AllSize"))
                //{
                //    newTbl.Columns.Add("AllSize", typeof(float));
                //}
                //if (!newTbl.Columns.Contains("CheckAll"))
                //{
                //    newTbl.Columns.Add("CheckAll", typeof(bool)).DefaultValue = false;
                //}
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



                gridControl2.DataSource = tblSizeFiltered;



            }
            else
            {
                DataRow dr = gridView1.GetFocusedDataRow();
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



                gridControl2.DataSource = tblSizeFiltered;
                checkBox1.CheckedChanged -= checkBox1_CheckedChanged;

                checkBox1.Checked = true;// (bool)dr["TachMau"];

                checkBox1.CheckedChanged += checkBox1_CheckedChanged;

            }
            tinhDinhMucGoiY();
            tinhDinhMucChung();
            //if ((bool)checkBox1.Checked == true)
            //{
            //    //layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //}
            //else
            //   // layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

            gridView6.ClearSelection();
            searchLookUpEdit3View.ClearSelection();
            gridView5.ClearSelection();
            searchLookUpEditMauSP.EditValue = null;
            searchLookUpEditInseam.EditValue = null;
            searchLookUpEditSize.EditValue = null;

            txtDinhMuc.Text = "";
            loadSearchLookUpMauSP1();
            loadSearchLookUpInseam();

            if (isAdd)
            {
                DataRow dr = gridView1.GetFocusedDataRow();
                LoadSizeDMNew(dr);
            }
            else
            {
                LoadChiTiet();
            }

        }
        private void createTable(DataTable tab)
        {
            bandedGridView1.OptionsView.AllowCellMerge = true;
            GridBand parentBand = bandedGridView1.Bands["gridBandSizeDM"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView1.Columns.Count;)
                {
                    if (bandedGridView1.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView1.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }
            int demColIndex = -1;

            foreach (DataColumn column in tab.Columns)
            {
                demColIndex++;
                if (demColIndex > 19 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    //col.OptionsColumn.AllowEdit = false;
                    col.Visible = true;
                    col.Width = 85;
                    //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 85;
                    gridBandSizeDM.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);

        }

        private void bandedGridView1_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
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
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            CreateSearchLookUpDot(isSua);
            CreateDotSTT();
            LoadSizeDM();
            LoadData();
            LoadChiTiet();

        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai(!isAdd);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isAdd = true;
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            CreateDotSTT();
            CreateSearchLookUpVT();
            LoadSizeDM();

            gridControl1.DataSource = null;
            gridControl2.DataSource = null;
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
            GridView view = gridView1;
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
            GridView view = gridView1;
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

        private void bandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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

        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            // Kiểm tra cột đang được vẽ có phải là "gridColumnTrangThaiDuyet" không
            if (e.Column == bandedGridColumn11)
            {
                object isXetDuyetValue = view.GetRowCellValue(e.RowHandle, e.Column);
                if (isXetDuyetValue != null && isXetDuyetValue.ToString() == "Đã duyệt")
                {
                    e.Appearance.ForeColor = Color.Green;


                }
                else
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
        }

        private void bandedGridView1_RowStyle(object sender, RowStyleEventArgs e)
        {

        }

        private void bandedGridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (bandedGridView1.FocusedColumn != null && bandedGridView1.FocusedColumn.FieldName != null && bandedGridView1.FocusedColumn.FieldName != "IsXetDuyet" && _checkUserName)
                {
                    DXMenuItem menuDuyet = new DXMenuItem();
                    menuDuyet.Caption = "Duyệt Đợt";
                    menuDuyet.Appearance.Font = new Font("Arial", 9);
                    menuDuyet.Click += MenuDuyetDong;
                    menuDuyet.Enabled = _allowEdit;
                    e.Menu.Items.Add(menuDuyet);

                    DXMenuItem menuHuy = new DXMenuItem();
                    menuHuy.Caption = "Hủy Duyệt Đợt";
                    menuHuy.Appearance.Font = new Font("Arial", 9);
                    menuHuy.Click += MenuHuyDong;
                    menuHuy.Enabled = _allowEdit;
                    e.Menu.Items.Add(menuHuy);

                }



            }
            catch (Exception ex)
            {


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
                btnLuu.Enabled = false;
            }
            if (!_allowEdit)
            {
                barButtonItem5.Enabled = false;
            }

        }

        private void bandedGridView1_CellMerge(object sender, CellMergeEventArgs e)
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

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            XetDuyet("duyệt", true, false);
        }
        private void XetDuyet(string title, bool xetduyet, bool checkduyet)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            DataRow drA = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;
            string MauVTID = dr["MauVTID"].ToString();
            string DotID = drA["MaDot"].ToString();
            string MaKH = dr["MaKH"].ToString();
            string MaHang = dr["MaHang"].ToString();
            string checkDuyetDot = checkduyet ? DotID : "";

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



        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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

        private void bandedGridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
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

        private void bandedGridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                object oldValue = bandedGridView1.GetRowCellValue(e.RowHandle, e.Column);


                if (e.Column.FieldName == "AllSize")
                {
                    bool isChecked = false;
                    bool.TryParse(bandedGridView1.GetRowCellValue(e.RowHandle, "CheckAll")?.ToString(), out isChecked);

                    if (isChecked && !string.IsNullOrEmpty(e.Value?.ToString()))
                    {
                        decimal soLuong;
                        if (decimal.TryParse(e.Value.ToString(), out soLuong) && soLuong >= 0)
                        {
                            foreach (GridColumn column in bandedGridView1.Columns)
                            {
                                if (column.FieldName.Contains("@"))
                                {
                                    bandedGridView1.SetRowCellValue(e.RowHandle, column, soLuong.ToString());
                                }
                            }
                        }
                    }
                }
                else if (e.Column.FieldName == "CheckAll")
                {
                    bool isChecked = false;
                    bool.TryParse(e.Value?.ToString(), out isChecked);

                    string allSizeValue = bandedGridView1.GetRowCellValue(e.RowHandle, "AllSize")?.ToString();
                    decimal soLuong;
                    bool hasAllSize = decimal.TryParse(allSizeValue, out soLuong);

                    if (isChecked)
                    {
                        foreach (GridColumn column in bandedGridView1.Columns)
                        {
                            if (column.FieldName.Contains("@"))
                            {
                                object currentValue = bandedGridView1.GetRowCellValue(e.RowHandle, column);
                                decimal currentSoLuong;

                                // Trường hợp AllSize có giá trị => đặt hết
                                if (hasAllSize && soLuong > 0)
                                {
                                    bandedGridView1.SetRowCellValue(e.RowHandle, column, soLuong.ToString());
                                }
                                // Trường hợp AllSize == null => chỉ set giá trị cho các ô null hoặc = 0
                                else if (!hasAllSize)
                                {
                                    if (currentValue == null ||
                                        !decimal.TryParse(currentValue.ToString(), out currentSoLuong) ||
                                        currentSoLuong == 0)
                                    {
                                        bandedGridView1.SetRowCellValue(e.RowHandle, column, "0");
                                    }
                                }
                            }
                        }
                    }
                }


                DataTable _tbl = gridControl2.DataSource as DataTable;
                if (_tbl.Rows.Count > 0)
                {
                    var groupedRows = _tbl.AsEnumerable()
                                          .GroupBy(row => row["MaMau"])
                                          .ToList();

                    foreach (var group in groupedRows)
                    {
                        Decimal totalForGroup = 0;
                        int countColumnsWithAt = 0;
                        int rowsInGroup = group.Count();

                        foreach (var row in group)
                        {
                            int columnCount = _tbl.Columns.Count;

                            for (int i = 0; i < columnCount; i++)
                            {
                                string columnName = _tbl.Columns[i].ColumnName;
                                if (columnName.Contains("@"))
                                {
                                    var columnValue = row[columnName];
                                    if (columnValue != DBNull.Value && Convert.ToDecimal(columnValue) != 0)
                                    {
                                        totalForGroup += Convert.ToDecimal(columnValue) * 1;
                                        countColumnsWithAt++;
                                    }
                                }
                                else if (i + 1 < columnCount && _tbl.Columns[i + 1].ColumnName.Contains("@"))
                                {
                                    var nextColumnValue = row[_tbl.Columns[i + 1].ColumnName];
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
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnXoaDot_Click_1(object sender, EventArgs e)
        {

            DataRow row = bandedGridView1.GetFocusedDataRow();
            if (row == null) return;
            string tendot = string.Empty;
            if (row["Dot"] != null && !string.IsNullOrWhiteSpace(row["Dot"].ToString()))
            {
                tendot = row["Dot"].ToString();
            }
            object _makh = null, _mahang = null, _mauvtID = null, _madot = null, _mavtID = null, _makhovt = null;
            DataRow row2 = gridView1.GetFocusedDataRow();
            if (row2 == null) return;
            _makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            _mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            _mauvtID = row2["MauVTID"].ToString();
            _madot = row["MaDot"].ToString();
            _mavtID = row2["MaVTID"].ToString();
            _makhovt = row2["KhoVaiID"].ToString();
            string urlcheck = string.Format("{0}?action=CheckXoaDot&para={1}&para2={2}", URL + "UpdateXetDuyet/Get",
              _mavtID.ToString() == "" ? "" : _mavtID.ToString(), _madot.ToString() == "" ? "" : _madot.ToString());
            string jsoncheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck); }).Result;
            if (jsoncheck != "[]")
            {
                XtraMessageBox.Show($"Đợt {tendot} đã được cấp phát. Không thể xóa.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult messResult = MessageBox.Show($"Bạn có muốn xóa đợt {tendot} không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {



                string url = string.Format("{0}?action=XoaDot&para={1}&para2={2}&para3={3}&para4={4}&para5={5}", URL + "UpdateXetDuyet/XoaDot", _makh.ToString() == "" ? "" : _makh.ToString(),
                _mahang.ToString() == "" ? "" : _mahang.ToString(), _madot.ToString() == "" ? "" : _madot.ToString(), _mauvtID.ToString() == "" ? "" : _mauvtID.ToString(), _makhovt.ToString() == "" ? "" : _makhovt.ToString());
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this);
                    LoadData();
                    LoadChiTiet();
                }

            }
        }
        private double tinhDMChung()
        {
            DataTable tbl = gridControl2.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return 0;

            double dmCT = 0;
            double sum = 0;
            int index = 0;

            foreach (DataRow dr in tbl.Rows)
            {
                foreach (DataColumn col in tbl.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        if (double.TryParse(dr[col].ToString(), out double temp))
                        {
                            sum += temp;
                            index++;
                        }
                    }
                }
            }
            if (index == 0) index = 1;
            dmCT = Math.Round(sum / index, 4);
            return dmCT;
        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                // this.ActiveControl = simpleButton4;
                _rowhandle = gridView1.FocusedRowHandle;
                DataTable _dtTable = gridControl2.DataSource as DataTable;
                if (_dtTable == null || _dtTable.Rows.Count == 0)
                {
                    return;
                }
                DataRow row2 = gridView1.GetFocusedDataRow();
                double _dmChung = tinhDMChung();

                DataTable tblSave = new DataTable();
                tblSave.Columns.Add("MaKH", typeof(string));
                tblSave.Columns.Add("MaHang", typeof(string));
                tblSave.Columns.Add("MaNhom", typeof(string));
                tblSave.Columns.Add("MaVTID", typeof(string));
                tblSave.Columns.Add("MauVTID", typeof(string));
                tblSave.Columns.Add("KhoVaiID", typeof(string));
                tblSave.Columns.Add("DinhMucChung", typeof(decimal));
                tblSave.Columns.Add("DinhMucHaoHut", typeof(decimal));
                tblSave.Columns.Add("MaDot", typeof(string));


                DataRow _dr = tblSave.NewRow();
                _dr["MaKH"] = row2["MaKH"].ToString();
                _dr["MaHang"] = row2["MaHang"].ToString();
                _dr["MaNhom"] = row2["MaNhom"].ToString();
                _dr["MaVTID"] = row2["MaVTID"].ToString();
                _dr["MauVTID"] = row2["MauVTID"].ToString();
                _dr["KhoVaiID"] = row2["KhoVaiID"].ToString();
                _dr["MaDot"] = searchLookUpEditDot.EditValue.ToString();
                _dr["DinhMucHaoHut"] = row2["DinhMucHaoHut"].ToString();
                _dr["DinhMucChung"] = _dmChung;
                tblSave.Rows.Add(_dr);

                DataTable tbl = new DataTable();
                tbl.Columns.Add("MaKH", typeof(string));
                tbl.Columns.Add("MaHang", typeof(string));
                tbl.Columns.Add("MaNhom", typeof(string));
                tbl.Columns.Add("MaVTID", typeof(string));
                tbl.Columns.Add("MauVTID", typeof(string));
                tbl.Columns.Add("KhoVaiID", typeof(string));
                tbl.Columns.Add("MaNhomSize", typeof(string));
                tbl.Columns.Add("MaSize", typeof(string));
                tbl.Columns.Add("DinhMuc", typeof(decimal));
                tbl.Columns.Add("MaDot", typeof(string));
                tbl.Columns.Add("NguoiTao", typeof(string));
                tbl.Columns.Add("MaMau", typeof(string));

                if (row2["TachMau"].ToString() == "" || row2["TachMau"].ToString() == "False")
                {
                    string mamau = "", tenmau = "";
                    for (int i = 0; i <= _dtTable.Rows.Count - 1; i++)
                    {
                        mamau = _dtTable.Rows[i][11].ToString();
                        tenmau = _dtTable.Rows[i][12].ToString();
                        if (_dtTable.Rows[i][15] != DBNull.Value && _dtTable.Rows[i][15].ToString() == "True")
                        {
                            continue;
                        }
                        for (int j = 19; j <= _dtTable.Columns.Count - 1; j++)
                        {
                            if (_dtTable.Columns[j].ColumnName.Contains("@"))
                            {
                                string sizeSX = string.Empty;
                                string[] arrNewHeader = _dtTable.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                                string sizeID = arrNewHeader[0];
                                sizeID = arrNewHeader[2];
                                sizeSX = arrNewHeader[0];
                                DataRow dr = tbl.NewRow();
                                dr["MaKH"] = _dtTable.Rows[i][0].ToString();
                                dr["MaHang"] = _dtTable.Rows[i][1].ToString();
                                dr["MaNhom"] = _dtTable.Rows[i][2].ToString();
                                dr["MaVTID"] = _dtTable.Rows[i][3].ToString();
                                dr["MauVTID"] = _dtTable.Rows[i][4].ToString();
                                dr["KhoVaiID"] = _dtTable.Rows[i][5].ToString();
                                dr["MaNhomSize"] = _dtTable.Rows[i][13].ToString();
                                dr["MaSize"] = sizeID;
                                dr["MaDot"] = _dtTable.Rows[i][9].ToString();
                                dr["DinhMuc"] = _dtTable.Rows[i][j].ToString();
                                dr["NguoiTao"] = GlobleData.UserName;
                                dr["MaMau"] = _dtTable.Rows[i][11].ToString();
                                tbl.Rows.Add(dr);
                            }
                        }

                    }
                    string[] maMauArr = mamau.Split('|');
                    string[] tenMauArr = tenmau.Split('|');
                    var distinctGroups = tbl.AsEnumerable()
                                                .Select(r => r["MaNhomSize"].ToString())
                                                .Distinct()
                                                .ToList();

                    int groupCount = distinctGroups.Count;
                    int colorCount = Math.Min(maMauArr.Length, tenMauArr.Length);

                    if (colorCount == 1)
                    {
                        foreach (DataRow row in tbl.Rows)
                        {
                            row["MaMau"] = maMauArr[0].ToString().Trim();
                        }
                    }
                    else
                    {
                        foreach (DataRow row in tbl.Rows)
                        {
                            row["MaMau"] = maMauArr[0].ToString().Trim();
                        }

                        List<DataRow> originalRows = tbl.AsEnumerable()
                                                            .Where(r => r["MaMau"].ToString() == maMauArr[0].ToString().Trim())
                                                            .ToList();

                        for (int colorIndex = 1; colorIndex < colorCount; colorIndex++)
                        {
                            foreach (DataRow baseRow in originalRows)
                            {
                                DataRow newRow = tbl.NewRow();
                                newRow.ItemArray = baseRow.ItemArray.Clone() as object[];
                                newRow["MaMau"] = maMauArr[colorIndex].ToString().Trim();
                                tbl.Rows.Add(newRow);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i <= _dtTable.Rows.Count - 1; i++)
                    {
                        if (_dtTable.Rows[i][15] != DBNull.Value && _dtTable.Rows[i][15].ToString() == "True")
                        {
                            continue;
                        }
                        for (int j = 19; j <= _dtTable.Columns.Count - 1; j++)
                        {
                            if (_dtTable.Columns[j].ColumnName.Contains("@"))
                            {
                                string sizeSX = string.Empty;
                                string[] arrNewHeader = _dtTable.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                                string sizeID = arrNewHeader[0];
                                sizeID = arrNewHeader[2];
                                sizeSX = arrNewHeader[0];
                                DataRow dr = tbl.NewRow();
                                dr["MaKH"] = _dtTable.Rows[i][0].ToString();
                                dr["MaHang"] = _dtTable.Rows[i][1].ToString();
                                dr["MaNhom"] = _dtTable.Rows[i][2].ToString();
                                dr["MaVTID"] = _dtTable.Rows[i][3].ToString();
                                dr["MauVTID"] = _dtTable.Rows[i][4].ToString();
                                dr["KhoVaiID"] = _dtTable.Rows[i][5].ToString();
                                dr["MaNhomSize"] = _dtTable.Rows[i][13].ToString();
                                dr["MaSize"] = sizeID;
                                dr["MaDot"] = _dtTable.Rows[i][9].ToString();
                                dr["DinhMuc"] = _dtTable.Rows[i][j].ToString();
                                dr["NguoiTao"] = GlobleData.UserName;
                                dr["MaMau"] = _dtTable.Rows[i][11].ToString();
                                tbl.Rows.Add(dr);
                            }
                        }

                    }
                }

                if (tbl == null && tbl.Rows.Count <= 0) return;
                string url = string.Format("{0}", URL + "PhanTichBom/PostDM");
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (msResult.ToLower() == "true")
                {
                    string urls = string.Format("{0}", URL + "PhanTichBom/PostDMChung");
                    string msResults = Task.Run(async () => { return await _clientExtension.PostAsync(urls, tblSave); }).Result;
                    if (msResults.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        LoadData();
                        LoadChiTiet();
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }



        private void bandedGridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == bandedGridColumn2)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }


        }


        #region excel
        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                    return;
                if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                    return;

                DataTable tblGC = gridControl1.DataSource as DataTable;
                if (tblGC == null || tblGC.Rows.Count == 0)
                    return;

                DataTable _tblAllSize = loadDataExcel(searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString());
                if (_tblAllSize == null || _tblAllSize.Rows.Count == 0) return;

                frmPhanTichBom_Excel frm = new frmPhanTichBom_Excel(tblGC, _tblAllSize, searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString());
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
        private void LoadVTV1()
        {
            try
            {
                //string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                //string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                //if (searchLookUpEditDot.EditValue.ToString() == "") return;
                //string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTUV1&para1={kh.ToString()}&para2={mh.ToString()}&para3={searchLookUpEditDot.EditValue.ToString()}";
                //string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                //DataTable tblVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);


                ////string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                ////string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTUMH&para1={kh.ToString()}";
                ////string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                ////DataTable tblVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
                //_tblmhvt = tblVT;
                //searchLookUpEditVatTu.Properties.DataSource = tblVT;
                //searchLookUpEditVatTu.Properties.ValueMember = "MaVTID";
                //searchLookUpEditVatTu.Properties.DisplayMember = "MaVT";

                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                if (searchLookUpEditDot.EditValue.ToString() == "") return;
                //string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTUV1&para1={kh.ToString()}&para2={mh.ToString()}&para3={searchLookUpEditDot.EditValue.ToString()}";
                string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTUMHV1&para1={kh.ToString()}&para2={mh.ToString()}&para3={searchLookUpEditDot.EditValue.ToString()}";
                string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                DataTable tblVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
                _tblmhvt = tblVT;
                searchLookUpEditVatTu.Properties.DataSource = tblVT;
                searchLookUpEditVatTu.Properties.ValueMember = "MaVTID";
                searchLookUpEditVatTu.Properties.DisplayMember = "MaVT";
            }
            catch (Exception ex)
            {

            }
        }
        private void searchLookUpEditDot_EditValueChanged(object sender, EventArgs e)
        {
            LoadSizeDM();
            LoadData();
            LoadVTV1();

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
            string maKH = searchLookUpEditKH.EditValue.ToString();
            string maHang = searchLookUpEditMH.EditValue.ToString();
            string maDot = searchLookUpEditDot.EditValue.ToString();
            //frmPhanTichBOM_Copy frmCopy = new frmPhanTichBOM_Copy(maKH, maHang, maDot);
            //frmCopy.ShowDialog();
        }

        private void gridView2_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);

                if (view.IsRowSelected(rowHandle3))
                {
                    selectedRows.Add(row);

                }
                else
                {
                    selectedRows.Remove(row);

                }
                //gridControl1.RefreshDataSource();
            }
            string selectedValues = string.Join(";", gridView2.GetSelectedRows().Select(rowHandle2 => gridView2.GetRowCellValue(rowHandle2, searchLookUpEditVatTu.Properties.ValueMember)));
            searchLookUpEditVatTu.EditValue = selectedValues;
            if (searchLookUpEditVatTu.EditValue is null) return;
            if (e.Action == CollectionChangeAction.Add)
            {
                // Khi người dùng chọn dòng
                int[] selectedRows = gridView2.GetSelectedRows();
                foreach (int rowHandle in selectedRows)
                {
                    if (rowHandle < 0) continue;
                    DataRow drRow = gridView2.GetDataRow(rowHandle);
                    AddRowGCNL(drRow);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                DataRow removedRow = gridView2.GetDataRow(e.ControllerRow);
                RemoveRowGCNL(removedRow);
            }
            if (e.Action == CollectionChangeAction.Refresh)
            {
                int selectedCount = gridView2.SelectedRowsCount;
                int totalRowCount = gridView2.RowCount;

                if (selectedCount == totalRowCount && selectedCount > 0)
                {
                    for (int i = 0; i < totalRowCount; i++)
                    {
                        if (!gridView2.IsRowSelected(i)) continue;
                        DataRow drRow = gridView2.GetDataRow(i);
                        AddRowGCNL(drRow);
                    }
                }
                else if (selectedCount == 0)
                {
                    gridControl1.DataSource = null;
                }
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
            return tbl;
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
                string filter = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND KhoVaiID = '{2}' AND MaNhom = '{3}' AND MaDVVT = '{4}'",
                                              rowAdd["MaVTID"], rowAdd["MauVTID"], rowAdd["KhoVaiID"], rowAdd["MaNhom"], rowAdd["MaDVVT"]);
                DataRow[] existingRows = tbl.Select(filter);
                if (existingRows.Length == 0)
                {
                    DataRow newRow = tbl.NewRow();
                    newRow["ID"] = 0;
                    newRow["MaNhom"] = rowAdd["MaNhom"];
                    newRow["TenNhom"] = rowAdd["TenNhom"];
                    newRow["NPL"] = rowAdd["NPL"];
                    newRow["Sort"] = rowAdd["Sort"];
                    newRow["MaVTID"] = rowAdd["MaVTID"];
                    newRow["MaVT"] = rowAdd["MaVT"];
                    newRow["ChiTiet"] = rowAdd["VatTu"];
                    newRow["MaDVVT"] = rowAdd["MaDVVT"];
                    newRow["TenDVVT"] = rowAdd["TenDVVT"];
                    newRow["MauVTID"] = rowAdd["MauVTID"];
                    newRow["MaMauVT"] = rowAdd["MaMauVT"];
                    newRow["MauVT"] = rowAdd["MauVT"];
                    newRow["KhoVaiID"] = rowAdd["KhoVaiID"];
                    newRow["KhoVai"] = rowAdd["KhoVai"];
                    newRow["MaMau"] = "";
                    newRow["TenMau"] = "";
                    newRow["DinhMucChung"] = 0;
                    newRow["DinhMucHaoHut"] = 0;
                    newRow["TachMau"] = false;
                    newRow["MaVTGhep"] = rowAdd["MaVTGhep"];
                    newRow["IsNew"] = 1;
                    newRow["STTCode"] = 0;
                    newRow["MaCode"] = rowAdd["MaVTID"].ToString() + "|" + "0";
                    tbl.Rows.Add(newRow);
                    gridControl1.DataSource = tbl;

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void RemoveRowGCNL(DataRow rowRemove)
        {
            try
            {
                if (rowRemove == null) return;
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null) return;
                DataTable tbl2 = gridControl2.DataSource as DataTable;



                string filter = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND KhoVaiID = '{2}' AND MaNhom = '{3}' AND MaDVVT = '{4}'",
                                              rowRemove["MaVTID"], rowRemove["MauVTID"], rowRemove["KhoVaiID"], rowRemove["MaNhom"], rowRemove["MaDVVT"]);
                string filter2 = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND KhoVaiID = '{2}' AND MaNhom = '{3}'",
                                             rowRemove["MaVTID"], rowRemove["MauVTID"], rowRemove["KhoVaiID"], rowRemove["MaNhom"]);
                DataRow[] rowsToDelete = tbl.Select(filter);
                DataRow[] rowsToDelete2 = _dtSize.Select(filter2);
                DataRow[] rowsToDelete3 = tbl2.Select(filter2);
                foreach (DataRow dr in rowsToDelete)
                {
                    tbl.Rows.Remove(dr);
                }
                foreach (DataRow dr in rowsToDelete2)
                {
                    _dtSize.Rows.Remove(dr);
                }
                foreach (DataRow dr in rowsToDelete3)
                {
                    tbl2.Rows.Remove(dr);
                }

                loadSearchLookUpMauSP1();
            }
            catch (Exception ex)
            { }


        }

        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;


            //string caption = info.Column.Caption;
            //if (info.Column.Caption == string.Empty)
            //    caption = info.Column.ToString();

            //if (info.Column == gridColumn26)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            //if (info.Column == gridColumn27)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            //if (gridView2.IsGroupRow(e.RowHandle))
            //{


            //    int groupIndex = gridView2.GetRowLevel(e.RowHandle);
            //    Color textColor = Color.White;
            //    if (groupIndex == 0)
            //    {
            //        textColor = Color.MediumBlue;
            //    }
            //    else if (groupIndex == 1)
            //    {
            //        textColor = Color.Maroon;
            //    }

            //    e.Appearance.ForeColor = textColor;
            //    e.DefaultDraw();

            //    e.Handled = true;

            //}
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (view == null || info == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);

            GridColumn groupColumn = info.Column;

            if (groupColumn == gridColumn51)
            {
                object groupSortIndex = view.GetGroupRowValue(e.RowHandle);
                string maHangText = "";

                if (_tblmhvt != null)
                {
                    DataRow[] matchingRows = _tblmhvt.Select($"SortIndex = '{groupSortIndex?.ToString()}'");
                    if (matchingRows.Length > 0)
                    {
                        string maHang = matchingRows[0]["MaHang"]?.ToString();
                        maHangText = string.IsNullOrWhiteSpace(maHang) ? "[Trống]" : maHang;
                    }
                }

                info.GroupText = $"Mã hàng: {maHangText}";
            }
            if (groupColumn == gridColumn26)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (groupColumn == gridColumn27)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

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

                string url = $"{URL}KhoiTaoDM/DuyetAll?makh={searchLookUpEditKH.EditValue.ToString()}&mahang={searchLookUpEditMH.EditValue.ToString()}&isduyet={isduyet}&username={GlobleData.UserName}&madot={searchLookUpEditDot.EditValue.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "True")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadChiTiet();
                    NapLai(true);
                    LoadVTV1();
                }
            }
            catch (Exception ex)
            {

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
                        LoadVTV1();
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
                    DataRow drF = gridView1.GetFocusedDataRow();
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

                        //    string maMau = row["MaMau"].ToString();
                        //    if (string.IsNullOrEmpty(maMau)) continue;
                        //    string manhomsize = row["MaNhomSize"].ToString();
                        //    // Gán giá trị vào row
                        //    row["MaVTID"] = maVTID;
                        //    row["MauVTID"] = maMauVT;
                        //    row["KhoVaiID"] = khoVaiID;
                        //    row["MaNhom"] = MaNhom;

                        //    // Kiểm tra trùng
                        //    bool isDuplicate = _dtSize.AsEnumerable().Any(r =>
                        //        r["MaVTID"].ToString() == maVTID &&
                        //        r["MauVTID"].ToString() == maMauVT &&
                        //        r["KhoVaiID"].ToString() == khoVaiID &&
                        //        r["MaNhom"].ToString() == MaNhom &&
                        //        r["MaMau"].ToString() == maMau &&
                        //        r["MaNhomSize"].ToString() == manhomsize
                        //    );

                        //    if (!isDuplicate)
                        //    {
                        //        _dtSize.ImportRow(row);
                        //    }

                        //}



                        indexTenMau++;
                    }

                }

                createTable(_dtSize);



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



                gridControl2.DataSource = tblSizeFiltered;
                checkBox1.Checked = true;//(bool)_dr["TachMau"];
            }

            catch (Exception ex) { }
        }


        private void repositoryItemButtonEditMauSP_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRow row = gridView1.GetFocusedDataRow();
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
                loadSearchLookUpMauSP1();
                loadSearchLookUpInseam();
                checkBox1.Checked = true;

            }
            catch (Exception ex)
            {


            }
        }

        private void bandedGridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e.Column.FieldName.Contains("@"))
            //{




            //}
            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView == null) return;
            gridView.BeginUpdate();
            try
            {
                DataRowView changedRowView = gridView.GetRow(e.RowHandle) as DataRowView;
                if (changedRowView != null)
                {
                    DataRow changedDataRow = changedRowView.Row;
                    var targetRow = _dtSize.AsEnumerable().FirstOrDefault(row =>
                        row["MaVTID"].ToString() == changedDataRow["MaVTID"].ToString() &&
                        row["MauVTID"].ToString() == changedDataRow["MauVTID"].ToString() &&
                        row["MaMau"].ToString() == changedDataRow["MaMau"].ToString() &&
                        row["KhoVaiID"].ToString() == changedDataRow["KhoVaiID"].ToString() &&
                        row["MaNhom"].ToString() == changedDataRow["MaNhom"].ToString() &&
                        row["MaNhomSize"].ToString() == changedDataRow["MaNhomSize"].ToString() &&
                        row["MaCode"].ToString() == changedDataRow["MaCode"].ToString()
                    );

                    if (targetRow != null)
                    {
                        if (changedDataRow.Table.Columns.Contains(e.Column.FieldName))
                        {
                            targetRow[e.Column.FieldName] = changedDataRow[e.Column.FieldName];
                        }
                    }
                }
            }
            finally
            {
                gridView.EndUpdate();
            }
            tinhDinhMucGoiY();
            tinhDinhMucChung();
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if ((bool)checkBox1.Checked == true)
            {
                DataRow dr = gridView1.GetFocusedDataRow();
                DataTable tbl = gridControl2.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count <= 0)
                {
                    //XtraMessageBox.Show("Không có dữ liệu để Tách màu.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    //checkBox1.Checked = false;
                    return;
                }
                layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                string[] mamauArr = dr["MaMau"].ToString().Replace(" ", "").Split('|');
                string[] tenmauArr = dr["TenMau"].ToString().Replace(" ", "").Split('|');
                DataTable tblMau = new DataTable();
                tblMau.Columns.Add("MaMau", typeof(string));
                tblMau.Columns.Add("TenMau", typeof(string));
                for (int i = 0; i < mamauArr.Length; i++)
                {
                    tblMau.Rows.Add(mamauArr[i], tenmauArr[i]);
                }
                searchLookUpEditMauSP.Properties.ValueMember = "MaMau";
                searchLookUpEditMauSP.Properties.DisplayMember = "TenMau";
                searchLookUpEditMauSP.Properties.DataSource = tblMau;
            }
            else
            {
                layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                searchLookUpEditMauSP.EditValue = null;
                searchLookUpEditMauSP.Properties.DataSource = null;
                DataRow dr = gridView1.GetFocusedDataRow();
                dr["DinhMucChung"] = 0;

            }
            LoadTachMau();
        }


        private void LoadTachMau()
        {
            DataTable tbl = gridControl2.DataSource as DataTable;
            DataRow dr = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            bool check = (bool)checkBox1.Checked;
            foreach (DataRow drTBL in tbl.Rows)
            {
                drTBL["TachMau"] = check;
            }
            foreach (DataRow drdtSize in _dtSize.Rows)
            {
                if (drdtSize["MaVTID"].ToString() == dr["MaVTID"].ToString().Trim() &&
                                                    drdtSize["MauVTID"].ToString() == dr["MauVTID"].ToString().Trim() &&
                                                    drdtSize["MaNhom"].ToString() == dr["MaNhom"].ToString().Trim() &&
                                                    drdtSize["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString().Trim() &&
                                                   drdtSize["MaCode"].ToString() == dr["MaCode"].ToString().Trim())
                {
                    drdtSize["TachMau"] = check;
                }

            }
            if (check)
            {
                dr["TachMau"] = true;

                string _mamauGoc = dr["MaMau"].ToString();
                string _tenmauGoc = dr["TenMau"].ToString();
                string[] maMauArr = dr["MaMau"].ToString().Split('|');
                string[] tenMauArr = dr["TenMau"].ToString().Split('|');
                var distinctGroups = tbl.AsEnumerable()
                                            .Select(r => r["MaNhomSize"].ToString())
                                            .Distinct()
                                            .ToList();

                int groupCount = distinctGroups.Count;
                int colorCount = Math.Min(maMauArr.Length, tenMauArr.Length);

                if (colorCount == 1)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        row["MaMau"] = maMauArr[0].ToString().Trim();
                        row["TenMau"] = tenMauArr[0].ToString().Trim();
                    }
                }
                else
                {

                    List<DataRow> originalRows = tbl.AsEnumerable()
                                                        .Where(r => r["MaMau"].ToString() == _mamauGoc.ToString() &&
                                                        r["MaVTID"].ToString() == dr["MaVTID"].ToString().Trim() &&
                                                        r["MauVTID"].ToString() == dr["MauVTID"].ToString().Trim() &&
                                                        r["MaNhom"].ToString() == dr["MaNhom"].ToString().Trim() &&
                                                        r["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString().Trim() &&
                                                        r["MaCode"].ToString() == dr["MaCode"].ToString().Trim()
                                                        )
                                                        .ToList();

                    for (int colorIndex = 0; colorIndex < colorCount; colorIndex++)
                    {
                        string maMau = maMauArr[colorIndex].ToString().Trim();
                        string tenMau = tenMauArr[colorIndex].ToString().Trim();
                        foreach (DataRow baseRow in originalRows)
                        {
                            string maVTID = baseRow["MaVTID"].ToString();
                            string mauVTID = baseRow["MauVTID"].ToString();
                            string khoVaiID = baseRow["KhoVaiID"].ToString();
                            string maNhom = baseRow["MaNhom"].ToString();
                            string maNhomSize = baseRow["MaNhomSize"].ToString();
                            string maCode = baseRow["MaCode"].ToString();
                            bool isDuplicate = _dtSize.AsEnumerable().Any(r =>
                                r["MaVTID"].ToString() == maVTID &&
                                r["MauVTID"].ToString() == mauVTID &&
                                r["KhoVaiID"].ToString() == khoVaiID &&
                                r["MaNhom"].ToString() == maNhom &&
                                r["MaMau"].ToString() == maMau &&
                                r["MaNhomSize"].ToString() == maNhomSize &&
                                r["MaCode"].ToString() == maCode
                            );

                            if (!isDuplicate)
                            {
                                DataRow newRow = tbl.NewRow();
                                newRow.ItemArray = baseRow.ItemArray.Clone() as object[];
                                newRow["MaMau"] = maMau;
                                newRow["TenMau"] = tenMau;

                                tbl.Rows.Add(newRow);
                                _dtSize.ImportRow(newRow);
                            }
                        }
                    }
                    string filter = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND KhoVaiID = '{2}' AND MaNhom = '{3}' AND MaMau = '{4}'  AND MaCode = '{5}'",
                                          dr["MaVTID"], dr["MauVTID"], dr["KhoVaiID"], dr["MaNhom"], _mamauGoc, dr["Macode"]);
                    DataRow[] rowsToDelete = tbl.Select(filter);
                    DataRow[] rowsToDelete2 = _dtSize.Select(filter);
                    foreach (DataRow dr2 in rowsToDelete)
                    {
                        tbl.Rows.Remove(dr2);
                    }
                    foreach (DataRow dr2 in rowsToDelete2)

                    {
                        _dtSize.Rows.Remove(dr2);
                    }

                }
                // bandedGridColumn5.GroupIndex = 0;
            }
            else
            {

                dr["TachMau"] = false;
                foreach (DataRow drTBL in tbl.Rows)
                {
                    drTBL["TachMau"] = false;
                }
                string filter = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND KhoVaiID = '{2}' AND MaNhom = '{3}' AND MaCode = '{4}",
                                        dr["MaVTID"], dr["MauVTID"], dr["KhoVaiID"], dr["MaNhom"], dr["MaCode"]);
                DataRow[] rowsToDelete = tbl.Select(filter);
                DataRow[] rowsToDelete2 = _dtSize.Select(filter);
                foreach (DataRow dr2 in rowsToDelete)
                {
                    tbl.Rows.Remove(dr2);
                }
                foreach (DataRow dr2 in rowsToDelete2)

                {
                    _dtSize.Rows.Remove(dr2);
                }
                LoadSizeDMNew(dr);
                //DataTable distinctTblSize = _dtSize.AsEnumerable()
                //    .Distinct(DataRowComparer.Default)
                //    .CopyToDataTable();
                //_dtSize = distinctTblSize.Copy();
            }

        }

        private void searchLookUpEditVatTu_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuessVT = string.Join("; ", gridView2.GetSelectedRows()
                .Select(rowHandle => gridView2.GetRowCellValue(rowHandle, searchLookUpEditVatTu.Properties.DisplayMember)?.ToString().Trim())
                .Where(val => !string.IsNullOrEmpty(val))
                .Distinct());

            if (string.IsNullOrEmpty(selectedValuessVT))
            {
                e.DisplayText = "----Chưa chọn vật tư----";
            }
            else
            {
                e.DisplayText = selectedValuessVT;
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (!isAdd) return;
            DataRow drF = gridView1.GetFocusedDataRow();
            if (string.IsNullOrWhiteSpace(drF["MaMau"].ToString())) return;

            if (e.Column.FieldName == "DinhMucChung")
            {
                double haoHut;
                if (double.TryParse(e.Value?.ToString(), out haoHut))
                {
                    //DataRow dr = gridView1.GetFocusedDataRow();

                    for (int i = 0; i < bandedGridView1.RowCount; i++)
                    {
                        foreach (DevExpress.XtraGrid.Columns.GridColumn col in bandedGridView1.Columns)
                        {
                            if (col.FieldName.Contains("@"))
                            {
                                bandedGridView1.SetRowCellValue(i, col, haoHut);
                            }
                        }
                    }

                    DataRowView changedRowView = gridView1.GetRow(e.RowHandle) as DataRowView;
                    if (changedRowView != null)
                    {
                        foreach (DataRow row in _dtSize.Rows)
                        {
                            if (row["MaVTID"].ToString() == drF["MaVTID"].ToString() && row["MauVTID"].ToString() == drF["MauVTID"].ToString() && row["KhoVaiID"].ToString() == drF["KhoVaiID"].ToString() && row["MaNhom"].ToString() == drF["MaNhom"].ToString())
                            {
                                foreach (DataColumn cl in _dtSize.Columns)
                                {
                                    if (cl.ColumnName.Contains("@"))
                                    {
                                        row[cl.ColumnName] = haoHut;
                                    }
                                }
                            }

                        }
                    }
                }
            }
            DataTable tbl = gridControl2.DataSource as DataTable;
        }

        private async void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            //try
            //{
            //    bool success = await Task.Run(async () =>
            //    {
            //        SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
            //        SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
            //        SplashScreenManager.Default.SetWaitFormDescription("Đang chuẩn bị dữ liệu... (10%)");
            //        _rowhandle = gridView1.FocusedRowHandle;
            //        string madotpost = "";
            //        string tendotpost = "";
            //        if (isAdd)
            //        {
            //            madotpost = _madot;
            //            tendotpost = textBox1.Text.ToString();
            //        }
            //        else
            //        {
            //            madotpost = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            //            tendotpost = searchLookUpEditDot.Text.ToString();
            //        }

            //        this.ActiveControl = button1;
            //        if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
            //            return false;
            //        if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
            //            return false;
            //        DataTable tblGC1 = gridControl1.DataSource as DataTable;
            //        DataTable _dtSizeTong = gridControl2.DataSource as DataTable;
            //        if (tblGC1 == null || tblGC1.Rows.Count == 0) return false;
            //        //var duplicateGroups = tblGC1.AsEnumerable().GroupBy(row => new
            //        //{
            //        //    MaVTID = row["MaVTID"]?.ToString(),
            //        //    MaNhom = row["MaNhom"]?.ToString(),
            //        //    MauVTID = row["MauVTID"]?.ToString(),
            //        //    MaMau = row["MaMau"]?.ToString(),
            //        //    KhoVai = row["KhoVai"]?.ToString(),
            //        //    MaDVVT = row["MaDVVT"]?.ToString()
            //        //}).Where(g => g.Count() > 1).ToList();

            //        //if (duplicateGroups.Any())
            //        //{
            //        //    MessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
            //        //    return;
            //        //}

            //        //DataTable tblGC2 = _dtSize.AsEnumerable()
            //        //.Where(row =>
            //        //    !string.IsNullOrWhiteSpace(row["MaMau"]?.ToString()) &&
            //        //    !string.IsNullOrWhiteSpace(row["MaVTID"]?.ToString())
            //        //)
            //        //.CopyToDataTable();
            //        var filteredRows = _dtSize.AsEnumerable().Where(row => !string.IsNullOrWhiteSpace(row["MaMau"]?.ToString()) && !string.IsNullOrWhiteSpace(row["MaVTID"]?.ToString()));

            //        DataTable tblGC2 = filteredRows.Any() ? filteredRows.CopyToDataTable() : _dtSize.Clone();
            //        if (tblGC2 == null || tblGC2.Rows.Count == 0) return false;

            //        string _makh = searchLookUpEditKH.EditValue.ToString(), _mahang = searchLookUpEditMH.EditValue.ToString();
            //        DataTable dtMauSP = createTypeMauSP();
            //        foreach (DataRow _dr in tblGC1.Rows)
            //        {

            //            string[] mamauArr = _dr["MaMau"].ToString().Split('|');
            //            foreach (string mamau in mamauArr)
            //            {
            //                DataRow _nr = dtMauSP.NewRow();
            //                _nr["ID"] = 0;

            //                _nr["MaKH"] = _makh;
            //                _nr["MaHang"] = _mahang;
            //                _nr["MaNhom"] = _dr["MaNhom"].ToString();

            //                _nr["MaVTID"] = _dr["MaVTID"].ToString();
            //                _nr["MauVTID"] = _dr["MauVTID"].ToString();
            //                _nr["KhoVaiID"] = _dr["KhoVaiID"].ToString();
            //                _nr["MaMau"] = mamau.ToString().Trim();
            //                _nr["MaDot"] = madotpost;
            //                _nr["MaCode"] = _dr["MaCode"];
            //                dtMauSP.Rows.Add(_nr);
            //            }
            //        }



            //        DataTable dtSizeSP = createTypeSizeSP();
            //        foreach (DataRow _dr in tblGC2.Rows)
            //        {
            //            string[] mamauArr = _dr["MaMau"].ToString().Split('|');

            //            foreach (string mamau in mamauArr)
            //            {
            //                for (int i = 20; i < tblGC2.Columns.Count; i++)
            //                {
            //                    DataRow _nr = dtSizeSP.NewRow();
            //                    _nr["MaKH"] = _makh;

            //                    _nr["MaHang"] = _mahang;

            //                    _nr["MaNhom"] = _dr["MaNhom"];
            //                    _nr["MaVTID"] = _dr["MaVTID"];
            //                    _nr["MauVTID"] = _dr["MauVTID"];
            //                    _nr["KhoVaiID"] = _dr["KhoVaiID"];

            //                    _nr["MaNhomSize"] = _dr["MaNhomSize"];

            //                    _nr["MaSize"] = tblGC2.Columns[i].ColumnName.Split('@')[1].ToString();

            //                    _nr["DinhMuc"] = _dr[i].ToString() == "" ? 0 : _dr[i];
            //                    _nr["MaDot"] = madotpost;
            //                    _nr["Dot"] = tendotpost;

            //                    _nr["NguoiTao"] = GlobleData.UserName;
            //                    _nr["MaMau"] = mamau.ToString().Trim();
            //                    _nr["TachMau"] = _dr["TachMau"];
            //                    _nr["MaCode"] = _dr["MaCode"];
            //                    dtSizeSP.Rows.Add(_nr);
            //                }

            //            }

            //        }

            //        DataTable dtVatTuSP = createTypeVattuSP();
            //        foreach (DataRow _dr in tblGC1.Rows)
            //        {
            //            if (_dr["MaMau"] == "") continue;
            //            DataRow _nr = dtVatTuSP.NewRow();
            //            _nr["ID"] = 0;
            //            _nr["MaKH"] = _makh;
            //            _nr["MaHang"] = _mahang;
            //            _nr["MaNhom"] = _dr["MaNhom"];
            //            _nr["MaVTID"] = _dr["MaVTID"];
            //            _nr["MauVTID"] = _dr["MauVTID"];
            //            _nr["KhoVaiID"] = _dr["KhoVaiID"];
            //            _nr["MaDot"] = madotpost;
            //            _nr["Dot"] = tendotpost;
            //            _nr["NguoiTao"] = GlobleData.UserName;
            //            _nr["DinhMucChung"] = _dr["DinhMucChung"];
            //            _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
            //            _nr["TachMau"] = _dr["TachMau"];
            //            _nr["STT"] = _stt;
            //            _nr["MaCode"] = _dr["MaCode"];
            //            _nr["STTCode"] = _dr["STTCode"];
            //            dtVatTuSP.Rows.Add(_nr);
            //        }
            //        SplashScreenManager.Default.SetWaitFormDescription("Đang lưu màu sản phẩm... (40%)");
            //        string url1 = $"{URL}KhoiTaoBOMV1/Post1?Action=POSTMAUSP&para1={GlobleData.UserName}";

            //        string result1 = Task.Run(async () => { return await _clientExtension.PostAsync(url1, dtMauSP); }).Result;
            //        if (result1 != "True") return false;
            //        SplashScreenManager.Default.SetWaitFormDescription("Đang lưu định mức size... (70%)");
            //        string url2 = $"{URL}KhoiTaoBOMV1/Post2?Action=POSTDMSIZE&para1={GlobleData.UserName}";
            //        string result2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, dtSizeSP); }).Result;
            //        if (result2 != "True") return false;
            //        SplashScreenManager.Default.SetWaitFormDescription("Đang lưu vật tư sản phẩm... (90%)");
            //        string url3 = $"{URL}KhoiTaoBOMV1/Post3?Action=POSTVTSP&para1={GlobleData.UserName}";
            //        string result3 = Task.Run(async () => { return await _clientExtension.PostAsync(url3, dtVatTuSP); }).Result;
            //        if (result3 != "True") return false;


            //        SplashScreenManager.Default.SetWaitFormDescription("Hoàn tất... (100%)");

            //        await Task.Delay(500);

            //        return true;





            //    });
            //    SplashScreenManager.CloseForm(false);

            //    if (success)
            //    {

            //        NapLai(!isAdd);
            //        gridView1.FocusedRowHandle = _rowhandle;
            //    }
            //    else
            //    {
            //        //MessageBox.Show("Lưu dữ liệu thất bại. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }


            //}
            //catch (Exception ex) { }
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                SplashScreenManager.Default.SetWaitFormDescription("Đang chuẩn bị dữ liệu... (10%)");

                // Truy cập UI an toàn ở đây
                _rowhandle = gridView1.FocusedRowHandle;
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
                var _dtSizeTong = gridControl2.DataSource as DataTable;
                if (tblGC1 == null || tblGC1.Rows.Count == 0)
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                var filteredRows = _dtSize.AsEnumerable().Where(row =>
                    !string.IsNullOrWhiteSpace(row["MaMau"]?.ToString()) &&
                    !string.IsNullOrWhiteSpace(row["MaVTID"]?.ToString()));
                DataTable tblGC2 = filteredRows.Any() ? filteredRows.CopyToDataTable() : _dtSize.Clone();

                if (tblGC2.Rows.Count == 0)
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mahang = searchLookUpEditMH.EditValue.ToString();

                // Thực thi logic nặng trên thread nền
                bool success = await Task.Run(async () =>
                {
                    DataTable dtMauSP = createTypeMauSP();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        string[] mamauArr = _dr["MaMau"].ToString().Split('|');
                        foreach (string mamau in mamauArr)
                        {
                            DataRow _nr = dtMauSP.NewRow();
                            _nr["ID"] = 0;
                            _nr["MaKH"] = _makh;
                            _nr["MaHang"] = _mahang;
                            _nr["MaNhom"] = _dr["MaNhom"];
                            _nr["MaVTID"] = _dr["MaVTID"];
                            _nr["MauVTID"] = _dr["MauVTID"];
                            _nr["KhoVaiID"] = _dr["KhoVaiID"];
                            _nr["MaMau"] = mamau.Trim();
                            _nr["MaDot"] = madotpost;
                            _nr["MaCode"] = _dr["MaCode"];
                            dtMauSP.Rows.Add(_nr);
                        }
                    }

                    DataTable dtSizeSP = createTypeSizeSP();
                    foreach (DataRow _dr in tblGC2.Rows)
                    {
                        string[] mamauArr = _dr["MaMau"].ToString().Split('|');
                        for (int i = 20; i < tblGC2.Columns.Count; i++)
                        {
                            foreach (string mamau in mamauArr)
                            {
                                DataRow _nr = dtSizeSP.NewRow();
                                _nr["MaKH"] = _makh;
                                _nr["MaHang"] = _mahang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = _dr["MauVTID"];
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaNhomSize"] = _dr["MaNhomSize"];
                                _nr["MaSize"] = tblGC2.Columns[i].ColumnName.Split('@')[1];
                                _nr["DinhMuc"] = string.IsNullOrWhiteSpace(_dr[i].ToString()) ? 0 : _dr[i];
                                _nr["MaDot"] = madotpost;
                                _nr["Dot"] = tendotpost;
                                _nr["NguoiTao"] = GlobleData.UserName;
                                _nr["MaMau"] = mamau.Trim();
                                _nr["TachMau"] = _dr["TachMau"];
                                _nr["MaCode"] = _dr["MaCode"];
                                dtSizeSP.Rows.Add(_nr);
                            }
                        }
                    }

                    DataTable dtVatTuSP = createTypeVattuSP();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(_dr["MaMau"].ToString())) continue;
                        DataRow _nr = dtVatTuSP.NewRow();
                        _nr["ID"] = 0;
                        _nr["MaKH"] = _makh;
                        _nr["MaHang"] = _mahang;
                        _nr["MaNhom"] = _dr["MaNhom"];
                        _nr["MaVTID"] = _dr["MaVTID"];
                        _nr["MauVTID"] = _dr["MauVTID"];
                        _nr["KhoVaiID"] = _dr["KhoVaiID"];
                        _nr["MaDot"] = madotpost;
                        _nr["Dot"] = tendotpost;
                        _nr["NguoiTao"] = GlobleData.UserName;
                        _nr["DinhMucChung"] = _dr["DinhMucChung"];
                        _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
                        _nr["TachMau"] = _dr["TachMau"];
                        _nr["STT"] = _stt;
                        _nr["MaCode"] = _dr["MaCode"];
                        _nr["STTCode"] = _dr["STTCode"];
                        dtVatTuSP.Rows.Add(_nr);
                    }

                    string url1 = $"{URL}KhoiTaoBOMV1/Post1?Action=POSTMAUSP&para1={GlobleData.UserName}";
                    string result1 = await _clientExtension.PostAsync(url1, dtMauSP);
                    if (result1 != "True") return false;

                    string url2 = $"{URL}KhoiTaoBOMV1/Post2?Action=POSTDMSIZE&para1={GlobleData.UserName}";
                    string result2 = await _clientExtension.PostAsync(url2, dtSizeSP);
                    if (result2 != "True") return false;

                    string url3 = $"{URL}KhoiTaoBOMV1/Post3?Action=POSTVTSP&para1={GlobleData.UserName}";
                    string result3 = await _clientExtension.PostAsync(url3, dtVatTuSP);
                    if (result3 != "True") return false;

                    return true;
                });

                SplashScreenManager.CloseForm(false);

                if (success)
                {
                    NapLai(!isAdd);
                    gridView1.FocusedRowHandle = _rowhandle;
                }
                else
                {
                    MessageBox.Show("Lưu dữ liệu thất bại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            return dt;
        }


        private void loadSearchLookUpInseam()
        {
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETINSEAM&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            searchLookUpEditInseam.Properties.DataSource = tbl;
            searchLookUpEditInseam.Properties.ValueMember = "MaNhomSize";
            searchLookUpEditInseam.Properties.DisplayMember = "NhomSize";
            searchLookUpEditInseam.EditValue = null;

            gridView5.OptionsSelection.MultiSelect = true;

            gridView5.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;

            gridView5.ClearSelection();

            if (tbl.Rows.Count == 1)
            {
                gridView5.OptionsSelection.MultiSelect = false;
                gridView5.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
                searchLookUpEditInseam.EditValue = tbl.Rows[0]["MaNhomSize"];
                string url2 = $"{URL}KhoiTaoBOMV1/Get?Action=GETINSEAMSIZE&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}&para3={tbl.Rows[0]["MaNhomSize"].ToString()}";
                string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                DataTable tbl2 = JsonConvert.DeserializeObject<DataTable>(json2);
                searchLookUpEditSize.Properties.DataSource = tbl2;
                searchLookUpEditSize.Properties.ValueMember = "SizeID";
                searchLookUpEditSize.Properties.DisplayMember = "Size";
            }
        }

        private void loadSearchLookUpSize()
        {
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETINSEAMSIZE&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            searchLookUpEditSize.Properties.DataSource = tbl;
            searchLookUpEditSize.Properties.ValueMember = "SizeID";
            searchLookUpEditSize.Properties.DisplayMember = "Size";
            searchLookUpEditInseam.EditValue = null;
        }

        private void gridView5_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", gridView5.GetSelectedRows().Select(rowHandle2 => gridView5.GetRowCellValue(rowHandle2, searchLookUpEditInseam.Properties.ValueMember)));
            searchLookUpEditInseam.EditValue = selectedValues;
            if (searchLookUpEditInseam.EditValue is null) return;
            if (string.IsNullOrWhiteSpace(selectedValues)) return;
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETINSEAMSIZE&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}&para3={selectedValues.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditSize.Properties.DataSource = tbl;
            searchLookUpEditSize.Properties.ValueMember = "SizeID";
            searchLookUpEditSize.Properties.DisplayMember = "Size";
        }

        private void searchLookUpEditInseam_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (gridView5.OptionsSelection.MultiSelect)
                {
                    var selectedValuessIS = string.Join("; ", gridView5.GetSelectedRows()
                      .Select(rowHandle => gridView5.GetRowCellValue(rowHandle, searchLookUpEditInseam.Properties.DisplayMember)?.ToString().Trim())
                      .Where(val => !string.IsNullOrEmpty(val))
                      .Distinct());

                    if (string.IsNullOrEmpty(selectedValuessIS))
                    {
                        e.DisplayText = "---Chưa chọn InSeam---";
                    }
                    else
                    {
                        e.DisplayText = selectedValuessIS;
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void txtDinhMuc_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void searchLookUpEdit3View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", searchLookUpEdit3View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit3View.GetRowCellValue(rowHandle2, searchLookUpEditSize.Properties.ValueMember)));
            searchLookUpEditSize.EditValue = selectedValues;
            if (searchLookUpEditSize.EditValue is null) return;
        }

        private void searchLookUpEditSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {

                var selectedValuessS = string.Join("; ", searchLookUpEdit3View.GetSelectedRows()
          .Select(rowHandle => searchLookUpEdit3View.GetRowCellValue(rowHandle, searchLookUpEditSize.Properties.DisplayMember)?.ToString().Trim())
          .Where(val => !string.IsNullOrEmpty(val))
          .Distinct());

                if (string.IsNullOrEmpty(selectedValuessS))
                {
                    e.DisplayText = "---Chưa chọn Size---";
                }
                else
                {
                    e.DisplayText = selectedValuessS;
                }

            }
            catch (Exception ex)
            {


            }

        }

        private void gridView6_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", gridView6.GetSelectedRows().Select(rowHandle2 => gridView6.GetRowCellValue(rowHandle2, searchLookUpEditMauSP.Properties.ValueMember)));
            searchLookUpEditMauSP.EditValue = selectedValues;
            if (searchLookUpEditMauSP.EditValue is null) return;



        }

        private void searchLookUpEditMauSP_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (gridView6.OptionsSelection.MultiSelect)
                {
                    var selectedValuessS = string.Join("; ", gridView6.GetSelectedRows()
                  .Select(rowHandle => gridView6.GetRowCellValue(rowHandle, searchLookUpEditMauSP.Properties.DisplayMember)?.ToString().Trim())
                  .Where(val => !string.IsNullOrEmpty(val))
                  .Distinct());

                    if (string.IsNullOrEmpty(selectedValuessS))
                    {
                        e.DisplayText = "---Chưa chọn màu sản phẩm---";
                    }
                    else
                    {
                        e.DisplayText = selectedValuessS;
                    }
                }

            }
            catch (Exception ex)
            {


            }

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


        private void btnXacNhanDinhMuc_Click(object sender, EventArgs e)
        {
            gridControl2.BeginUpdate();
            try
            {
                if (ckbox_DinhMuc_ALL.Checked)
                {
                    FillDinhMucALL();
                }
                else
                {
                    DataRow dr1 = gridView1.GetFocusedDataRow();
                    string tachMau = dr1["TachMau"].ToString();
                    decimal _dm = decimal.TryParse(txtDinhMuc.Text, out var val) ? val : 0;

                    int[] selectedRowHandles = searchLookUpEdit3View.GetSelectedRows();
                    if (selectedRowHandles.Length == 0) return;
                    DataTable selectedData = ((DataView)searchLookUpEdit3View.DataSource).Table.Clone();

                    foreach (int handle in selectedRowHandles)
                    {
                        DataRowView rowView = (DataRowView)searchLookUpEdit3View.GetRow(handle);
                        if (rowView != null)
                        {
                            selectedData.ImportRow(rowView.Row);
                        }
                    }

                    DataTable tblGC = gridControl2.DataSource as DataTable;
                    if (isReset)
                    {

                        foreach (DataRow dr in tblGC.Rows)
                        {
                            // Lặp qua các cột size (cột có chứa '@')
                            foreach (DataColumn col in tblGC.Columns)
                            {
                                if (col.ColumnName.Contains("@"))
                                {
                                    // Reset giá trị về 0
                                    dr[col] = 0;
                                }
                            }
                        }
                    }
                    if (tachMau == "True")
                    {
                        DataTable tblMauSP = new DataTable();
                        DataTable searchMauSP = searchLookUpEditMauSP.Properties.DataSource as DataTable;
                        tblMauSP = searchMauSP.Clone();
                        int[] selectedRowHandles2 = gridView6.GetSelectedRows();
                        if (selectedRowHandles2.Length == 0 && searchMauSP.Rows.Count == 0)
                        {
                            XtraMessageBox.Show("Vui lòng chọn Màu sản phẩm.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;

                        }
                        else
                        {
                            if (selectedRowHandles2.Length > 0)
                            {
                                foreach (int handle in selectedRowHandles2)
                                {
                                    DataRowView rowView = (DataRowView)gridView6.GetRow(handle);
                                    if (rowView != null)
                                    {
                                        tblMauSP.ImportRow(rowView.Row);
                                    }
                                }
                            }
                            else
                            {
                                tblMauSP = searchMauSP;
                            }
                        }



                        HashSet<string> validMaMauSet = new HashSet<string>(
                        tblMauSP.AsEnumerable().Select(r => r["MaMau"].ToString())
);

                        var selectedDict = new Dictionary<string, HashSet<string>>();

                        foreach (DataRow row in selectedData.Rows)
                        {
                            string maNhomSize = row["MaNhomSize"].ToString();
                            string sizeID = row["SizeID"].ToString();

                            if (!selectedDict.ContainsKey(maNhomSize))
                                selectedDict[maNhomSize] = new HashSet<string>();

                            selectedDict[maNhomSize].Add(sizeID);
                        }
                        foreach (DataRow dr in tblGC.Rows)
                        {
                            string maMau = dr["MaMau"].ToString();
                            if (!validMaMauSet.Contains(maMau)) continue;
                            string maNhomSize = dr["MaNhomSize"].ToString();

                            if (!selectedDict.ContainsKey(maNhomSize)) continue;

                            var sizeIDs = selectedDict[maNhomSize];

                            for (int i = 19; i < tblGC.Columns.Count; i++)
                            {
                                string[] parts = tblGC.Columns[i].ColumnName.Split('@');
                                if (parts.Length < 2) continue;

                                string colSizeID = parts[1];

                                if (sizeIDs.Contains(colSizeID))
                                {
                                    dr[i] = _dm;
                                }
                            }
                        }
                        Dictionary<string, DataRow> dictSize = _dtSize.AsEnumerable()
                          .ToDictionary(
                              row => $"{row["MaVTID"]}_{row["MauVTID"]}_{row["KhoVaiID"]}_{row["MaNhom"]}_{row["MaMau"]}_{row["MaNhomSize"]}_{row["MaCode"]}",
                              row => row
                          );

                        // Lặp qua tblGC và gán dữ liệu nếu tìm thấy key khớp
                        foreach (DataRow dr in tblGC.Rows)
                        {
                            string maMau = dr["MaMau"].ToString();
                            if (!validMaMauSet.Contains(maMau)) continue;  // Bỏ qua nếu MaMau không hợp lệ
                            string key = $"{dr["MaVTID"]}_{dr["MauVTID"]}_{dr["KhoVaiID"]}_{dr["MaNhom"]}_{dr["MaMau"]}_{dr["MaNhomSize"]}_{dr["MaCode"]}";

                            if (dictSize.TryGetValue(key, out DataRow matchedRow))
                            {
                                foreach (DataColumn col in tblGC.Columns)
                                {
                                    if (col.ColumnName.Contains("@"))
                                    {
                                        matchedRow[col.ColumnName] = dr[col.ColumnName];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //foreach(DataRow dr in tblGC.Rows)
                        // {
                        //     foreach(DataRow r in selectedData.Rows)
                        //     {
                        //         if (dr["MaNhomSize"].ToString() == r["MaNhomSize"].ToString())
                        //         {
                        //             for (int i = 19; i < tblGC.Columns.Count; i++)
                        //             {
                        //                if(tblGC.Columns[i].ColumnName.Split('@')[1].ToString()== r["SizeID"].ToString())
                        //                 {
                        //                     dr[i] = _dm;
                        //                 }    

                        //             }
                        //         }
                        //     }    
                        // }    

                        var selectedDict = new Dictionary<string, HashSet<string>>();

                        foreach (DataRow row in selectedData.Rows)
                        {
                            string maNhomSize = row["MaNhomSize"].ToString();
                            string sizeID = row["SizeID"].ToString();

                            if (!selectedDict.ContainsKey(maNhomSize))
                                selectedDict[maNhomSize] = new HashSet<string>();

                            selectedDict[maNhomSize].Add(sizeID);
                        }
                        foreach (DataRow dr in tblGC.Rows)
                        {
                            string maNhomSize = dr["MaNhomSize"].ToString();

                            if (!selectedDict.ContainsKey(maNhomSize)) continue;

                            var sizeIDs = selectedDict[maNhomSize];

                            for (int i = 19; i < tblGC.Columns.Count; i++)
                            {
                                string[] parts = tblGC.Columns[i].ColumnName.Split('@');
                                if (parts.Length < 2) continue;

                                string colSizeID = parts[1];

                                if (sizeIDs.Contains(colSizeID))
                                {
                                    dr[i] = _dm;
                                }
                            }
                        }
                        //foreach(DataRow dr in tblGC.Rows)
                        //{
                        //    foreach(DataRow _dr in _dtSize.Rows)
                        //    {
                        //        if(dr["MaVTID"].ToString()==_dr["MaVTID"].ToString()&& dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["MaMau"].ToString() == _dr["MaMau"].ToString() && dr["MaNhomSize"].ToString() == _dr["MaNhomSize"].ToString())
                        //        {
                        //            foreach (DataColumn col in tblGC.Columns)
                        //            {
                        //                if (col.ColumnName.Contains("@"))
                        //                {
                        //                    _dr[col.ColumnName] = dr[col.ColumnName];
                        //                }
                        //            }
                        //        }    
                        //    }    
                        //}    
                        // Tạo dictionary để tra cứu từ _dtSize
                        Dictionary<string, DataRow> dictSize = _dtSize.AsEnumerable()
                            .ToDictionary(
                                row => $"{row["MaVTID"]}_{row["MauVTID"]}_{row["KhoVaiID"]}_{row["MaNhom"]}_{row["MaMau"]}_{row["MaNhomSize"]}_{row["MaCode"]}",
                                row => row
                            );

                        // Lặp qua tblGC và gán dữ liệu nếu tìm thấy key khớp
                        foreach (DataRow dr in tblGC.Rows)
                        {
                            string key = $"{dr["MaVTID"]}_{dr["MauVTID"]}_{dr["KhoVaiID"]}_{dr["MaNhom"]}_{dr["MaMau"]}_{dr["MaNhomSize"]}_{dr["MaCode"]}";

                            if (dictSize.TryGetValue(key, out DataRow matchedRow))
                            {
                                foreach (DataColumn col in tblGC.Columns)
                                {
                                    if (col.ColumnName.Contains("@"))
                                    {
                                        matchedRow[col.ColumnName] = dr[col.ColumnName];
                                    }
                                }
                            }
                        }
                    }
                }

                tinhDinhMucGoiY();
                tinhDinhMucChung();
            }
            finally
            {
                gridControl2.EndUpdate();
            }

        }
        private void tinhDinhMucGoiY()
        {
            try
            {
                DataTable tbl = gridControl2.DataSource as DataTable;

                if (tbl == null || tbl.Rows.Count == 0) return;

                List<string> sizeColumns = tbl.Columns
              .Cast<DataColumn>()
              .Where(c => c.ColumnName.Contains("@"))
              .Select(c => c.ColumnName)
              .ToList();

                //// 2. Nhóm theo MaMau
                //var groupedByMaMau = tbl.AsEnumerable()
                //    .Where(row => !row.IsNull("MaMau"))
                //    .GroupBy(row => row["MaMau"].ToString());

                //// 3. Tính trung bình theo từng nhóm MaMau
                //foreach (var group in groupedByMaMau)
                //{
                //    double total = 0;
                //    int count = 0;

                //    foreach (DataRow row in group)
                //    {
                //        foreach (string col in sizeColumns)
                //        {
                //            if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                //            {
                //                total += val;
                //                count++;
                //            }
                //        }
                //    }

                //    double average = count > 0 ? total / count : 0;

                //    // Gán giá trị trung bình cho tất cả dòng cùng MaMau
                //    foreach (DataRow row in group)
                //    {
                //        row["DinhMucGY"] = Math.Round(average, 4);
                //    }
                //}
                var groupedByMaMau = _dtSize.AsEnumerable()
                 .Where(row => !row.IsNull("MaMau") && !row.IsNull("MaCode"))
                 .GroupBy(row => new
                 {
                     MaMau = row["MaMau"].ToString().Trim(),
                     STTIndex = row["MaCode"].ToString().Trim()
                 });
                var groupedByMaMau2 = tbl.AsEnumerable()
              .Where(row => !row.IsNull("MaMau") && !row.IsNull("MaCode"))
              .GroupBy(row => new
              {
                  MaMau = row["MaMau"].ToString().Trim(),
                  STTIndex = row["MaCode"].ToString().Trim()
              });
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
                foreach (var group in groupedByMaMau2)
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


        private void tinhDinhMucChung()
        {

            DataRow dr = gridView1.GetFocusedDataRow();
            DataTable tbl = gridControl2.DataSource as DataTable;
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
            dr["DinhMucChung"] = Math.Round(average, 4);
        }

        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.RowHandle >= 0)
            {
                string IsNew = view.GetRowCellDisplayText(e.RowHandle, view.Columns["IsNew"]);

                if (!string.IsNullOrWhiteSpace(IsNew) && IsNew.ToString() == "1")
                {
                    e.Appearance.BackColor = Color.Honeydew;
                }
            }
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            /* DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
             if (gridView1.RowCount > 0)
             {
                 if (e.Menu == null)
                     return;
                 e.Menu.Items.Clear();

                 if (e.HitInfo.InRow)
                 {
                     if (e.HitInfo.Column != null)
                     {
                         DevExpress.Utils.Menu.DXMenuItem menuCopyCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", CopyDong);
                         e.Menu.Items.Add(menuCopyCopyItem);
                     }
                 }
             }*/

            try
            {
                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (gridView1.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {

                            bool IsXetDuyet = false;
                            DataRow row_focused = gridView1.GetFocusedDataRow();
                            if (row_focused == null) return;
                            if (row_focused.Table.Columns.Contains("IsXetDuyet"))
                            {
                                IsXetDuyet = row_focused["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                            }
                            bool IsCopyMauDM = false;
                            if (!IsXetDuyet)
                            {
                                IsCopyMauDM = (bool)(gridControl1.DataSource as DataTable)?.AsEnumerable()?
                                 .Any(x =>
                                     decimal.TryParse(x["DinhMucChung"]?.ToString(), out var dinhMuc) && dinhMuc != 0 &&
                                     !string.IsNullOrEmpty(x["MaMau"]?.ToString()?.Trim()) &&
                                     !x.Equals(row_focused)
                                 );

                                DevExpress.Utils.Menu.DXMenuItem menuCopyCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", CopyDong);
                                e.Menu.Items.Add(menuCopyCopyItem);
                                if (!string.IsNullOrEmpty(row_focused["MaMau"]?.ToString()) && IsCopyMauDM)
                                {
                                    DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Copy định mức màu", CopyDinhMuc_Mau);
                                    e.Menu.Items.Add(menuCopyMauItem);

                                }
                                DevExpress.Utils.Menu.DXMenuItem menuRemoveVatTuItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", RemoveVatTu);
                                e.Menu.Items.Add(menuRemoveVatTuItem);

                            }

                        }
                        if (!isAdd)
                        {
                            DevExpress.Utils.Menu.DXMenuItem menuXetduyetdong = new DevExpress.Utils.Menu.DXMenuItem("Xét duyệt dòng", xetduyetdong);
                            e.Menu.Items.Add(menuXetduyetdong);
                            DevExpress.Utils.Menu.DXMenuItem menuhuyduyetdong = new DevExpress.Utils.Menu.DXMenuItem("Hủy duyệt dòng", huyduyetdong);
                            e.Menu.Items.Add(menuhuyduyetdong);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void xetduyetdong(object sender, EventArgs e)
        {
            _rowhandle = gridView1.FocusedRowHandle;
            DataRow currentRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (currentRow == null) return;
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string urls = $"{URL}KhoiTaoBOMV1/XetDuyetDong?Action=XETDUYETDONG&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}" +
                $"&para3={searchLookUpEditDot.EditValue.ToString()} &para4={currentRow["MaVTID"].ToString()} &para5={currentRow["MauVTID"].ToString()} &para6={currentRow["KhoVaiID"].ToString()}" +
                $"&para7={currentRow["MaCode"].ToString()} &para8={1}";
            string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(urls); }).Result;
            if (jsons == "True")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadData();
                LoadChiTiet();
            }


        }
        private void huyduyetdong(object sender, EventArgs e)
        {
            _rowhandle = gridView1.FocusedRowHandle;
            DataRow currentRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (currentRow == null) return;
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string urls = $"{URL}KhoiTaoBOMV1/XetDuyetDong?Action=HUYDUYETDONG&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}" +
                $"&para3={searchLookUpEditDot.EditValue.ToString()} &para4={currentRow["MaVTID"].ToString()} &para5={currentRow["MauVTID"].ToString()} &para6={currentRow["KhoVaiID"].ToString()}" +
                $"&para7={currentRow["MaCode"].ToString()} &para8={0}";
            string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(urls); }).Result;
            if (jsons == "True")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadData();
                LoadChiTiet();
            }
        }
        private void CopyDong(object sender, EventArgs e)
        {

            GridView view = gridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                DataTable table = gridControl1.DataSource as DataTable;
                if (table == null) return;

                DataRow currentRow = view.GetDataRow(rowHandle);
                if (currentRow == null) return;

                string currentMaVTID = currentRow["MaVTID"].ToString();
                string currentMauVTID = currentRow["MauVTID"].ToString();
                string currentKhoVaiID = currentRow["KhoVaiID"].ToString();


                // Tìm STT lớn nhất với MaVTID hiện tại
                var maxSTT = table.AsEnumerable()
                                  .Where(r => r["MaVTID"].ToString() == currentMaVTID && r["MauVTID"].ToString() == currentMauVTID && r["KhoVaiID"].ToString() == currentKhoVaiID && r["MaVTID"].ToString() == currentMaVTID && int.TryParse(r["STTCode"].ToString(), out _))
                                  .Select(r => Convert.ToInt32(r["STTCode"]))
                                  .DefaultIfEmpty(0)
                                  .Max();

                int newSTT = maxSTT + 1;

                // Tạo dòng mới
                DataRow newRow = table.NewRow();

                // Copy dữ liệu từ dòng hiện tại
                foreach (DataColumn col in table.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }

                // Gán lại MaVTID và STT
                // newRow["ThemDong"] = 1;
                newRow["STTCode"] = newSTT;
                newRow["MaCode"] = $"{currentMaVTID}|{newSTT}";
                newRow["MaMau"] = "";
                newRow["TenMau"] = "";
                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 0;
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
        private HashSet<DataRow> selectedRows = new HashSet<DataRow>();

        private void gridView2_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }
        private void loadSearchLookUpMauSP1()
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;

            //layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            string[] mamauArr = dr["MaMau"].ToString().Replace(" ", "").Split('|');
            string[] tenmauArr = dr["TenMau"].ToString().Replace(" ", "").Split('|');
            DataTable tblMau = new DataTable();
            tblMau.Columns.Add("MaMau", typeof(string));
            tblMau.Columns.Add("TenMau", typeof(string));
            for (int i = 0; i < mamauArr.Length; i++)
            {
                if (mamauArr[i] == "") continue;
                tblMau.Rows.Add(mamauArr[i], tenmauArr[i]);
            }
            searchLookUpEditMauSP.Properties.ValueMember = "MaMau";
            searchLookUpEditMauSP.Properties.DisplayMember = "TenMau";
            searchLookUpEditMauSP.Properties.DataSource = tblMau;
            searchLookUpEditMauSP.EditValue = null;
            gridView6.OptionsSelection.MultiSelect = true;
            gridView6.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            gridView6.ClearSelection();
            if (tblMau.Rows.Count == 1)
            {
                gridView6.OptionsSelection.MultiSelect = false;
                gridView6.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
                searchLookUpEditMauSP.EditValue = tblMau.Rows[0]["MaMau"];


            }
        }
        private void ckDinhMuc_ALL_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbox_DinhMuc_ALL.Checked)
            {
                layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                searchLookUpEditMauSP.EditValue = null;
                searchLookUpEditInseam.EditValue = null;
                searchLookUpEditSize.EditValue = null;
            }
            else
            {

                layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
        }

        private void FillDinhMucALL()
        {
            //DataTable tblDinhMucSize = gridControl2.DataSource as DataTable;
            DataRow rowFoused = gridView1.GetFocusedDataRow();
            object MaVTID = rowFoused["MaVTID"];
            object MauVatID = rowFoused["MauVTID"];
            object MaNhom = rowFoused["MaNhom"];
            object KhoVaiID = rowFoused["KhoVaiID"];
            object MaCode = rowFoused["MaCode"];

            var tblFilter = _dtSize.AsEnumerable()
                        .Where(x =>
                            x["MaVTID"]?.ToString() == MaVTID?.ToString() &&
                            x["MauVTID"]?.ToString() == MauVatID?.ToString() &&
                            x["MaNhom"]?.ToString() == MaNhom?.ToString() &&
                            x["KhoVaiID"]?.ToString() == KhoVaiID?.ToString() &&
                            x["MaCode"]?.ToString() == MaCode?.ToString()
                        ).ToList();

            if (tblFilter?.Count == 0)
            {
                //XtraMessageBox.Show($"Vui lòng chọn màu SP để nhập định mức lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            double.TryParse(txtDinhMuc?.EditValue?.ToString(), out double dinhmuc_size);

            foreach (var row in tblFilter)
            {
                foreach (DataColumn col in _dtSize.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        row[col.ColumnName] = dinhmuc_size;
                    }
                }
            }
            gridControl2.DataSource = tblFilter?.CopyToDataTable();
            gridControl2.RefreshDataSource();
            tinhDinhMucGoiY();
            tinhDinhMucChung();
        }
        private void CopyDinhMuc_Mau(object sender, EventArgs e)
        {

            DataTable _dtDM_VatTu = gridControl1.DataSource as DataTable;
            int PreFocusedRowHandle = gridView1.FocusedRowHandle;
            DataRow row_focus = gridView1.GetFocusedDataRow();
            var selectedRow_MH = searchLookUpEditMH.GetSelectedDataRow() as DataRowView;
            var selectedRow_KH = searchLookUpEditKH.GetSelectedDataRow() as DataRowView;
            if (selectedRow_MH == null || selectedRow_KH == null)
            {
                XtraMessageBox.Show($"Lỗi lấy khách hàng hoặc mã hàng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (_dtDM_VatTu?.Rows?.Count == 0)
            {
                XtraMessageBox.Show($"Chưa có vật từ nào để Copy.Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (row_focus == null)
            {
                XtraMessageBox.Show($"Chọn dòng để Copy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var query = _dtDM_VatTu.AsEnumerable()
                          .Where(x =>
                              decimal.TryParse(x["DinhMucChung"]?.ToString(), out var dinhMuc) && dinhMuc != 0 &&
                              !string.IsNullOrEmpty(x["MaMau"]?.ToString()?.Trim()) &&
                              !x.Equals(row_focus)
                          ).ToList();
            if (query?.Count == 0)
            {
                XtraMessageBox.Show($"Chưa có vật tư nào để Copy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string madot = searchLookUpEditKH.EditValue.ToString() + '_' + searchLookUpEditMH.EditValue.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_" + _stt;
            DataTable dataSizePasre = _dtSize.Copy();
            frmCopyDinhMucMau frm = new frmCopyDinhMucMau(query?.CopyToDataTable(), row_focus, textBox1.Text, madot, selectedRow_KH, selectedRow_MH, _dtSize.Copy());
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                gridView1.SetRowCellValue(PreFocusedRowHandle, colDinhMucChung, frm.DinhMucChung);
                this._dtSize = frm.tblPaste;
                LoadChiTiet();

            }
        }



        private void RemoveVatTu(object sender, EventArgs e)
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                int Rowfocus_Idx = gridView1.FocusedRowHandle;
                if (Rowfocus_Idx < 0) return;
                DataRow rowFocus = gridView1.GetFocusedDataRow();
                if (rowFocus != null)
                {
                    if (rowFocus["ID"].ToString() == "0")
                    {

                        RemoveRowDM(rowFocus, Rowfocus_Idx);
                        LoadChiTiet();
                        // gridView1.FocusedRowHandle = 0;
                        return;

                    }
                    //if (isAdd)
                    //{
                    //    RemoveRowDM(rowFocus, Rowfocus_Idx);
                    //    LoadChiTiet();
                    //   // gridView1.FocusedRowHandle = 0;

                    //}
                    else
                    {
                        DialogResult result = XtraMessageBox.Show(
                         "BẠN CÓ CHẮC CHẮN MUỐN XÓA!\n\n" +
                         $"VẬT TƯ: {rowFocus["ChiTiet"]}\n" +
                         $"MÀU   : {rowFocus["MauVT"]}\n" +
                         $"KHỔ   : {rowFocus["KhoVai"]}\n\n",
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
                            object MauVatID = rowFocus["MauVTID"];
                            object MaNhom = rowFocus["MaNhom"];
                            object KhoVaiID = rowFocus["KhoVaiID"];
                            object MaCode = rowFocus["MaCode"];
                            dynamic JPara = new
                            {
                                MaVTID = MaVTID,
                                MauVTID = MauVatID,
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
                                XtraMessageBox.Show($"Xóa đã xảy ra lỗi.Vui lòng thực hiệu lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            RemoveRowDM(rowFocus, Rowfocus_Idx);

                        }
                    }
                    if (isAdd)
                    {

                        DataRow dr = gridView1.GetFocusedDataRow();
                        LoadSizeDMNew(dr);
                    }
                    else
                    {
                        LoadChiTiet();
                        LoadVTV1();
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
                object MaVTID = rowRm["MaVTID"];
                object MauVatID = rowRm["MauVTID"];
                object MaNhom = rowRm["MaNhom"];
                object KhoVaiID = rowRm["KhoVaiID"];
                object MaCode = rowRm["MaCode"];
                object MaVTGhep = rowRm["MaVTGhep"];
                for (int i = _dtSize.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = _dtSize.Rows[i];

                    bool IsRowDelete =
                        Equals(MaVTID, row["MaVTID"]) &&
                        Equals(MauVatID, row["MauVTID"]) &&
                        Equals(MaNhom, row["MaNhom"]) &&
                        Equals(KhoVaiID, row["KhoVaiID"]) &&
                        Equals(MaCode, row["MaCode"]) && !string.IsNullOrEmpty(row["MaMau"]?.ToString());

                    if (IsRowDelete)
                    {
                        _dtSize.Rows.RemoveAt(i); // xóa an toàn
                    }
                }

                gridView1.DeleteRow(Rowfocus_Idx);
                gridView1.RefreshData();

                /*uncheck Lưới searchlookup vật tư*/
                int count_VTCopy = 0;
                //int rowHandle_gridVatTu_selectect = gridView2.LocateByValue("MaVTGhep", MaVTGhep);
                int rowHandle_gridVatTu_selectect = Enumerable.Range(0, gridView2.RowCount)
                    .FirstOrDefault(i =>
                    Equals(gridView2.GetRowCellValue(i, "MaVTID"), MaVTID) &&
                    Equals(gridView2.GetRowCellValue(i, "MauVTID"), MauVatID) &&
                    Equals(gridView2.GetRowCellValue(i, "MaNhom"), MaNhom) &&
                    Equals(gridView2.GetRowCellValue(i, "KhoVaiID"), KhoVaiID));

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (i == Rowfocus_Idx) continue; // Bỏ qua dòng đang focus

                    object MaVTID2 = gridView1.GetRowCellValue(i, "MaVTID");
                    object MauVTID2 = gridView1.GetRowCellValue(i, "MauVTID");
                    object MaNhom2 = gridView1.GetRowCellValue(i, "MaNhom");
                    object KhoVaiID2 = gridView1.GetRowCellValue(i, "KhoVaiID");

                    if (Equals(MaVTID, MaVTID2) &&
                        Equals(MauVatID, MauVTID2) &&
                        Equals(MaNhom, MaNhom2) &&
                        Equals(KhoVaiID, KhoVaiID2))
                    {
                        count_VTCopy++;
                    }
                }

                if (rowHandle_gridVatTu_selectect >= 0 && count_VTCopy == 0)
                {
                    gridView2.UnselectRow(rowHandle_gridVatTu_selectect);
                    gridView2.RefreshData();
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
        private void checkBoxReset_CheckedChanged(object sender, EventArgs e)
        {
            isReset = (bool)checkBoxReset.Checked;
        }
    }
}
