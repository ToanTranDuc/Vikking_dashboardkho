using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SqlClient;
using DevExpress.XtraGrid.Views.Grid;
using NtbSoft.ERP.Model.ThuVien;
using System.Net.Http;
using Newtonsoft.Json;
using NtbSoft.ERP.Model.Kho;
using NtbSoft.ERP.Entity.Kho;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using System.IO;
using OfficeOpenXml;
using NtbSoft.ERP.Win.Utils;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{

    public partial class frmERP_VatTuNganHangGia : DevExpress.XtraEditors.XtraForm
    {
        private string _maVTID;
        private string _maVT;
        private string _tenVT;
        private HttpClientExtension _clientExtension;
        private string URL = "";
        private HttpClient _client = new HttpClient();
        private DataTable _tblVatTu = new DataTable();
        private bool _isUpdatingFromDateEdit = false;
        public string TenNhom { get; set; }
        public string MaNhom { get; set; }
        private bool _isVatTuLoaded = false;

        private bool _isHistoryMode = false;
        private bool _isSearchMode = false;
        private int _currentEffectiveRowHandle = -1;

        private bool _allowAdd = false;
        private bool _allowEdit = false;
        private bool _allowDelete = false;

        private SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        public frmERP_VatTuNganHangGia()
        {
            InitializeComponent();



            URL = System.Configuration.ConfigurationManager.AppSettings["URL"];

      
            _clientExtension = new HttpClientExtension();
        }
        private void InitCheckbox()
        {
            repoChonNhieuVT.ValueChecked = true;
            repoChonNhieuVT.ValueUnchecked = false;
            repoChonNhieuVT.ValueGrayed = false;
            repoChonNhieuVT.AllowGrayed = false;
            gridViewGia.CellValueChanged += gridViewGia_CellValueChanged;


            gridViewGia.FocusedRowChanged += gridViewGia_FocusedRowChanged;




            gridViewGia.OptionsSelection.MultiSelect = false;
            gridViewGia.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridViewGia.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridViewGia.CustomColumnDisplayText += gridViewGia_CustomColumnDisplayText;

        }
        private DateTime? ParseDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            if (value is DateTime dt)
            {
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            }

            string input = value.ToString().Trim();

            DateTime result;

            if (DateTime.TryParseExact(
                    input,
                    new[]
                    {
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-dd"
                    },
                    CultureInfo.GetCultureInfo("vi-VN"),
                    DateTimeStyles.None,
                    out result))
            {
               
                return new DateTime(
                    result.Year,
                    result.Month,
                    result.Day,
                    result.Hour,
                    result.Minute,
                    0
                );
            }

            return null;
        }
        private DataTable NormalizeDateColumns(DataTable dt)
        {
            if (dt == null) return null;

            if (dt.Columns.Contains("NgayApDung") &&
             dt.Columns["NgayApDung"].DataType == typeof(DateTime))
            {
                return dt;
            }
            DataTable newDt = dt.Clone();



            if (newDt.Columns.Contains("NgayApDung"))
                newDt.Columns["NgayApDung"].DataType = typeof(DateTime);

            if (newDt.Columns.Contains("NgayKetThuc"))
                newDt.Columns["NgayKetThuc"].DataType = typeof(DateTime);
            foreach (DataRow r in dt.Rows)
            {
                DataRow newRow = newDt.NewRow();

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName == "NgayApDung" || col.ColumnName == "NgayKetThuc")
                    {
                        if (r[col] != DBNull.Value)
                        {
                          
                            newRow[col.ColumnName] = Convert.ToDateTime(r[col]);
                        }
                    }
                    else
                    {
                        newRow[col.ColumnName] = r[col];
                    }
                }

                newDt.Rows.Add(newRow);
            }

            return newDt;
        }

        private void ApplyGridFormat()
        {
            gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridColumn6.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";

            gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridColumn1.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";

            #region ENABLE DATETIME FULL

            repositoryItemDateEdit1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            repositoryItemDateEdit1.Mask.EditMask = "dd/MM/yyyy HH:mm";
            repositoryItemDateEdit1.Mask.UseMaskAsDisplayFormat = true;

            repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";

            repositoryItemDateEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit1.EditFormat.FormatString = "dd/MM/yyyy HH:mm";

            repositoryItemDateEdit1.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            repositoryItemDateEdit1.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            repositoryItemDateEdit1.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;

            #endregion



            repositoryItemDateEdit2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            repositoryItemDateEdit2.Mask.EditMask = "dd/MM/yyyy HH:mm";
            repositoryItemDateEdit2.Mask.UseMaskAsDisplayFormat = true;

            repositoryItemDateEdit2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit2.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";

            repositoryItemDateEdit2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit2.EditFormat.FormatString = "dd/MM/yyyy HH:mm";

      
            repositoryItemDateEdit2.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            repositoryItemDateEdit2.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            repositoryItemDateEdit2.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            #region FIX 24H POPUP (QUAN TRỌNG)

           
            repositoryItemDateEdit1.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit1.CalendarTimeProperties.DisplayFormat.FormatString = "HH:mm";

            repositoryItemDateEdit1.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit1.CalendarTimeProperties.EditFormat.FormatString = "HH:mm";

        
            repositoryItemDateEdit1.CalendarTimeProperties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            repositoryItemDateEdit1.CalendarTimeProperties.Mask.EditMask = "HH:mm";
            repositoryItemDateEdit1.CalendarTimeProperties.Mask.UseMaskAsDisplayFormat = true;

         
            repositoryItemDateEdit1.CalendarTimeProperties.TimeEditStyle = DevExpress.XtraEditors.Repository.TimeEditStyle.TouchUI;

            #endregion
            #region FIX 24H POPUP (DATEEDIT 2)

            repositoryItemDateEdit2.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit2.CalendarTimeProperties.DisplayFormat.FormatString = "HH:mm";

            repositoryItemDateEdit2.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repositoryItemDateEdit2.CalendarTimeProperties.EditFormat.FormatString = "HH:mm";

            repositoryItemDateEdit2.CalendarTimeProperties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            repositoryItemDateEdit2.CalendarTimeProperties.Mask.EditMask = "HH:mm";
            repositoryItemDateEdit2.CalendarTimeProperties.Mask.UseMaskAsDisplayFormat = true;

            repositoryItemDateEdit2.CalendarTimeProperties.TimeEditStyle = DevExpress.XtraEditors.Repository.TimeEditStyle.TouchUI;

            #endregion

            RepositoryItemTextEdit repoGia = new RepositoryItemTextEdit();

           
            repoGia.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            repoGia.DisplayFormat.FormatString = "n2";

            
            repoGia.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            repoGia.EditFormat.FormatString = "n2";

            
            repoGia.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            repoGia.Mask.EditMask = "n2";
            repoGia.Mask.UseMaskAsDisplayFormat = true;

            
            gridControlGia.RepositoryItems.Add(repoGia);
            gridColumn7.ColumnEdit = repoGia;

        }
        private DateTime GetToday()
        {
            return DateTime.Today;
        }

        private string GetTodayVN()
        {
            return DateTime.Today.ToString("dd/MM/yyyy");
        }

        private void gridViewGia_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;

            DataRow row = gridViewGia.GetDataRow(e.FocusedRowHandle);
            if (row == null) return;

            _isUpdatingFromDateEdit = true;


            _isUpdatingFromDateEdit = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                CheckPerminsion();
                InitCheckbox();
                LoadNhom();
                InitTienTeDropdown();
                LoadEmptyData();
                this.KeyPreview = true;
                this.KeyDown += Frm_KeyDown;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi OnLoad: " + ex.Message);
            }




            gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridColumn7.DisplayFormat.FormatString = "n2";

            gridViewGia.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            gridViewGia.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridViewGia.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridViewGia.OptionsSelection.MultiSelect = true;
            gridViewGia.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;



            gridViewGia.RefreshData();
            gridViewGia.RowCellStyle += gridViewGia_RowCellStyle;
            gridViewGia.ShowingEditor += gridViewGia_ShowingEditor;
        }

        private void gridViewGia_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                string fieldName = view.FocusedColumn.FieldName;

                if (fieldName == "IsCheck")
                    return;

             
                bool allowEdit =
                    fieldName == "NgayApDung" ||
                    fieldName == "NgayKetThuc" ||
                    fieldName == "DonGia" ||
                    fieldName == "DonViTienTe";

                if (!allowEdit)
                {
                    e.Cancel = true;
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private void gridViewGia_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            GridView view = sender as GridView;
            DataRow row = view.GetDataRow(e.RowHandle);
            if (row == null) return;

            bool isExpired = false;
            if (row["NgayApDung"] != DBNull.Value &&
                 row["NgayKetThuc"] != DBNull.Value)
            {
                DateTime from = Convert.ToDateTime(row["NgayApDung"]);
                DateTime to = Convert.ToDateTime(row["NgayKetThuc"]);

             
                if (from == to)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 204, 204);
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    return;
                }

              
                if (DateTime.Today >= to)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 204, 204);
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
       
            if (row.Table.Columns.Contains("NgayKetThuc") &&
                row["NgayKetThuc"] != DBNull.Value)
            {
                DateTime ngayKT = Convert.ToDateTime(row["NgayKetThuc"]).Date;




                if (DateTime.Today >= ngayKT)
                {
                    isExpired = true;

                    e.Appearance.BackColor = Color.FromArgb(255, 204, 204);
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }

 
            if (!isExpired &&
                row.Table.Columns.Contains("MaPhieuBG") &&
                row["MaPhieuBG"] != DBNull.Value &&
                !string.IsNullOrEmpty(row["MaPhieuBG"].ToString()))
            {
                e.Appearance.BackColor = Color.FromArgb(255, 255, 204);
                e.Appearance.ForeColor = Color.Black;
            }

     
            if (row.Table.Columns.Contains("IsOldPrice") &&
                row["IsOldPrice"] != DBNull.Value &&
                Convert.ToInt32(row["IsOldPrice"]) == 1)
            {
                e.Appearance.ForeColor = Color.Gray;
            }

       
            if (row.Table.Columns.Contains("IsImportChanged") &&
                row["IsImportChanged"] != DBNull.Value &&
                Convert.ToBoolean(row["IsImportChanged"]))
            {
                e.Appearance.BackColor = Color.FromArgb(255, 255, 204);
                e.Appearance.ForeColor = Color.Black;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }

            if (_isHistoryMode &&
                _currentEffectiveRowHandle >= 0 &&
                e.RowHandle == _currentEffectiveRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(255, 255, 204);
                e.Appearance.ForeColor = Color.Black;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                return;
            }

        }
        // INSERT HERE: frmERP_VatTuNganHangGia.cs -> SetCurrentEffectiveRowHandle

        private void SetCurrentEffectiveRowHandle()
        {
            _currentEffectiveRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;

            if (!_isHistoryMode) return;

            GridView view = gridViewGia;
            if (view.RowCount <= 0) return;

            DateTime today = DateTime.Today;

            int bestCurrentHandle = -1;
            DateTime bestCurrentDate = DateTime.MinValue;

            int bestFutureHandle = -1;
            DateTime bestFutureDate = DateTime.MaxValue;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row == null || row["NgayApDung"] == DBNull.Value)
                    continue;

                DateTime ngayApDung = Convert.ToDateTime(row["NgayApDung"]).Date;

                // ưu tiên ngày <= hôm nay gần nhất
                if (ngayApDung <= today)
                {
                    if (ngayApDung > bestCurrentDate)
                    {
                        bestCurrentDate = ngayApDung;
                        bestCurrentHandle = i;
                    }
                }
                else
                {
                    // nếu không có thì lấy ngày tương lai gần nhất
                    if (ngayApDung < bestFutureDate)
                    {
                        bestFutureDate = ngayApDung;
                        bestFutureHandle = i;
                    }
                }
            }

            _currentEffectiveRowHandle =
                bestCurrentHandle >= 0
                ? bestCurrentHandle
                : bestFutureHandle;

            gridViewGia.RefreshData();
        }
        private void Frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.E)
            {
                btnXuatMauExcel_ItemClick(null, null);
            }
        }
        private async void InitTienTeDropdown()
        {
            try
            {
                repoTienTe = new RepositoryItemComboBox();

                string url = $"{URL}ERPVatTuBOM/GetTienTe";
                var json = await _client.GetStringAsync(url);

                DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, typeof(DataTable));

                repoTienTe.Items.Clear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string maTienTe = row["MaTienTe"]?.ToString()?.Trim();

                        if (!string.IsNullOrEmpty(maTienTe))
                            repoTienTe.Items.Add(maTienTe);
                    }
                }

                repoTienTe.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

         
                gridColumn8.FieldName = "DonViTienTe";

                gridControlGia.RepositoryItems.Add(repoTienTe);
                gridColumn8.ColumnEdit = repoTienTe;

                gridViewGia.RefreshData();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi load tiền tệ: " + ex.Message);
            }
        }
        private void LoadEmptyData()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaVTID");    
            dt.Columns.Add("MaVT");
            dt.Columns.Add("ChiTiet");
            dt.Columns.Add("MaCLVTID");
            dt.Columns.Add("MauVTID");
            if (!dt.Columns.Contains("MaMauVT"))
                dt.Columns.Add("MaMauVT", typeof(string));

            dt.Columns.Add("MauVT");

            dt.Columns.Add("KhoVaiID");  
            dt.Columns.Add("KhoVai");

            dt.Columns.Add("MaDVVT");
            dt.Columns.Add("TenDVVT");

            dt.Columns.Add("NgayApDung", typeof(DateTime));
            dt.Columns.Add("NgayKetThuc", typeof(DateTime));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("DonViTienTe");

            dt.Columns.Add("GhiChu");
            dt.Columns.Add("NguoiTao");

            dt.Columns.Add("IsCheck", typeof(bool)).DefaultValue = false;

            gridControlGia.DataSource = dt;
        }


        private void ApplyFromUIToCheckedRows(DataRow sourceRow)
        {

            DataTable dt = _tblVatTu;
            if (dt == null || sourceRow == null) return;


            if (sourceRow["IsCheck"] == DBNull.Value || !(bool)sourceRow["IsCheck"])
                return;

            DateTime? ngayApDung = sourceRow["NgayApDung"] == DBNull.Value
                ? (DateTime?)null
                : Convert.ToDateTime(sourceRow["NgayApDung"]);

            DateTime? ngayKetThuc = sourceRow["NgayKetThuc"] == DBNull.Value
                ? (DateTime?)null
                : Convert.ToDateTime(sourceRow["NgayKetThuc"]);

            object donGia = sourceRow["DonGia"];
            object donViTienTe = sourceRow["DonViTienTe"];

            string sourceMaCLVTID = sourceRow.Table.Columns.Contains("MaCLVTID")
                ? sourceRow["MaCLVTID"]?.ToString()?.Trim()
                : null;

            string sourceKhoVaiID = sourceRow.Table.Columns.Contains("KhoVaiID")
                ? sourceRow["KhoVaiID"]?.ToString()?.Trim()
                : null;

            gridViewGia.BeginUpdate();

            try
            {
                foreach (DataRow row in dt.Rows)
                {
              
                    if (row["IsCheck"] == DBNull.Value || !(bool)row["IsCheck"])
                        continue;

                   
                    if (row == sourceRow)
                        continue;

                    

     
                    if (ngayApDung.HasValue)
                        row["NgayApDung"] = ngayApDung.Value;

                    if (ngayKetThuc.HasValue)
                        row["NgayKetThuc"] = ngayKetThuc.Value;

                    if (donGia != DBNull.Value)
                        row["DonGia"] = donGia;

                    if (donViTienTe != DBNull.Value)
                        row["DonViTienTe"] = donViTienTe;
                }
            }
            finally
            {
                gridViewGia.EndUpdate();
            }

            gridViewGia.RefreshData();
            (gridControlGia.DataSource as DataView)?.Table.AcceptChanges();
        }

        private DialogResult ShowConfirmVN(string message)
        {
            XtraForm frm = new XtraForm();
            frm.Text = "Xác nhận";
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.Size = new Size(450, 230);
            frm.FormBorderStyle = FormBorderStyle.FixedDialog;
            frm.MaximizeBox = false;
            frm.MinimizeBox = false;

            LabelControl lbl = new LabelControl();
            lbl.Text = message;
            lbl.AutoSizeMode = LabelAutoSizeMode.Vertical;
            lbl.Dock = DockStyle.Top;
            lbl.Padding = new Padding(10);

            SimpleButton btnYes = new SimpleButton();
            btnYes.Text = "Đồng ý";
            btnYes.DialogResult = DialogResult.Yes;
            btnYes.Width = 100;

            SimpleButton btnNo = new SimpleButton();
            btnNo.Text = "Không";
            btnNo.DialogResult = DialogResult.No;
            btnNo.Width = 100;

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.RightToLeft;
            panel.Dock = DockStyle.Bottom;
            panel.Height = 50;
            panel.Padding = new Padding(10);

            panel.Controls.Add(btnNo);
            panel.Controls.Add(btnYes);

            frm.Controls.Add(lbl);
            frm.Controls.Add(panel);

            frm.AcceptButton = btnYes;
            frm.CancelButton = btnNo;

            return frm.ShowDialog();
        }

        private void gridViewGia_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {



            if (e.RowHandle < 0) return;

            var view = sender as GridView;
        
            view.PostEditor();
            view.UpdateCurrentRow();

            var row = view.GetDataRow(e.RowHandle);
            if (row == null) return;
            if (row.Table.Columns.Contains("IsChanged"))
                row["IsChanged"] = true;



            if (IsDuplicateRow(row))
            {
                XtraMessageBox.Show(
                    "Dữ liệu đã tồn tại trong lịch sử hoặc đang bị trùng trên lưới.\n" +
                    "Không được phép lưu cùng ngày + cùng giờ + cùng đơn giá.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                RestoreOldValue(row, e.Column.FieldName);
                view.RefreshData();
                return;
            }



            if (row["NgayApDung"] != DBNull.Value && row["NgayKetThuc"] != DBNull.Value)
            {
                DateTime from = Convert.ToDateTime(row["NgayApDung"]);
                DateTime to = Convert.ToDateTime(row["NgayKetThuc"]);

                if (from > to)
                {
                    XtraMessageBox.Show("Ngày áp dụng không được lớn hơn ngày kết thúc. Vui lòng chỉnh lại.");


                    return;
                }
            }

      


            if (_isUpdatingFromDateEdit) return;


            if (e.Column.FieldName == "IsCheck")
            {
                bool isCheckedNow = row["IsCheck"] != DBNull.Value && (bool)row["IsCheck"];

                if (isCheckedNow)
                {
                    view.PostEditor();
                    view.UpdateCurrentRow();

                    ApplyFromUIToCheckedRows(row);
                }

                return;
            }
     

            bool isChecked = row["IsCheck"] != DBNull.Value && (bool)row["IsCheck"];

            if (!isChecked)
                return;

            if (e.Column.FieldName == "NgayApDung" ||
                e.Column.FieldName == "NgayKetThuc" ||
                e.Column.FieldName == "DonGia" ||
                e.Column.FieldName == "DonViTienTe")
            {
              
                ApplyFromUIToCheckedRows(row);
            }


       
            if (row["NgayApDung"] == DBNull.Value ||
         
                row["DonGia"] == DBNull.Value ||
                row["DonViTienTe"] == DBNull.Value ||
                string.IsNullOrWhiteSpace(row["DonViTienTe"]?.ToString()))
            {
                return;
            }

           

        }

        private bool IsDuplicateRow(DataRow currentRow)
        {
            if (currentRow == null) return false;

            string maVTID = GetString(currentRow, "MaVTID");
            string maDVVT = GetString(currentRow, "MaDVVT");
            string maCLVTID = GetString(currentRow, "MaCLVTID");
            string mauVTID = GetString(currentRow, "MauVTID");
            string khoVaiID = GetString(currentRow, "KhoVaiID");

            DateTime? ngayAD = GetDate(currentRow, "NgayApDung");
            decimal? donGia = GetDecimal(currentRow, "DonGia");

            if (!ngayAD.HasValue || !donGia.HasValue)
                return false;

            return _tblVatTu.AsEnumerable().Any(r =>
            {
                if (r == currentRow) return false;

                return GetString(r, "MaVTID") == maVTID
                    && GetString(r, "MaDVVT") == maDVVT
                    && GetString(r, "MaCLVTID") == maCLVTID
                    && GetString(r, "MauVTID") == mauVTID
                    && GetString(r, "KhoVaiID") == khoVaiID
                    && GetDate(r, "NgayApDung")?.ToString("dd/MM/yyyy HH:mm")
                        == ngayAD.Value.ToString("dd/MM/yyyy HH:mm")
                    && GetDecimal(r, "DonGia") == donGia;
            });
        }
        private void RestoreOldValue(DataRow row, string fieldName)
{
    switch (fieldName)
    {
        case "NgayApDung":
            row["NgayApDung"] = row["NgayApDung_Old"];
            break;

        case "NgayKetThuc":
            row["NgayKetThuc"] = row["NgayKetThuc_Old"];
            break;

        case "DonGia":
            row["DonGia"] = row["DonGia_Old"];
            break;

        case "DonViTienTe":
            row["DonViTienTe"] = row["DonViTienTe_Old"];
            break;
    }
}
        private bool ValidateData(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row["IsCheck"] != DBNull.Value && (bool)row["IsCheck"])
                {
                    if (row["NgayApDung"] == DBNull.Value)
                        return false;

                    
                    if (row["NgayKetThuc"] != DBNull.Value)
                    {
                        if ((DateTime)row["NgayApDung"] > (DateTime)row["NgayKetThuc"])
                            return false;
                    }

                    if ((DateTime)row["NgayApDung"] > (DateTime)row["NgayKetThuc"])
                        return false;
                }
            }
            return true;
        }


        private void BindDonViLookup(DataTable dtDonVi)
        {
            RepositoryItemLookUpEdit repoDV = new RepositoryItemLookUpEdit();

            repoDV.DataSource = dtDonVi;
            repoDV.ValueMember = "MaDVVT";
            repoDV.DisplayMember = "TenDVVT";

            repoDV.NullText = "";

            gridControlGia.RepositoryItems.Add(repoDV);

            gridViewGia.Columns["MaDVVT"].ColumnEdit = repoDV;
        }
        private async void LoadNhom()
        {
            try
            {
                string url = $"{URL}ERPVatTuBOM/GetGia";

                var json = await _client.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(json) || json == "[]")
                {
                    searchLookUpEditNhom.Properties.DataSource = null;
                    return;
                }

                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                if (dt == null || dt.Rows.Count == 0)
                    return;


                DataTable dtNhom = new DataTable();
                dtNhom.Columns.Add("MaCLVT");
                dtNhom.Columns.Add("TenCLVT");
                dtNhom.Columns.Add("SoLuongVT", typeof(int));

                foreach (DataRow item in dt.Rows)
                {
                    DataRow row = dtNhom.NewRow();
                    row["MaCLVT"] = item["MaNhom"];
                    row["TenCLVT"] = item["TenCLVT"];
                    row["SoLuongVT"] = item["SoLuongVT"];


                    dtNhom.Rows.Add(row);
                }

                searchLookUpEditNhom.Properties.DataSource = dtNhom;
                searchLookUpEditNhom.Properties.DisplayMember = "TenCLVT";
                searchLookUpEditNhom.Properties.ValueMember = "MaCLVT";

                GridView view = searchLookUpEditNhom.Properties.View as GridView;

                view.Columns.Clear();
                view.Columns.AddVisible("TenCLVT", "Tên Nhóm");
                view.Columns.AddVisible("SoLuongVT", "Số lượng");

                view.Columns["SoLuongVT"].AppearanceCell.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;

                view.Columns["SoLuongVT"].Width = 80;
                view.OptionsView.ShowColumnHeaders = true;
                view.OptionsView.ShowIndicator = false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi load nhóm: " + ex.Message);
            }
        }
        private void btnAddPrice_Click(object sender, EventArgs e)
        {

        }





        private void LoadData(string maNhom)
        {
            try
            {
                ERP_VatTuModel model = new ERP_VatTuModel();

                DataTable dt = model.Get("", ""); 

                if (!string.IsNullOrEmpty(maNhom))
                {
                    DataRow[] rows = dt.Select($"MaNhom = '{maNhom}'");

                    if (rows.Length > 0)
                        gridControlGia.DataSource = _tblVatTu;

                    else
                        gridControlGia.DataSource = dt.Clone();
                }
                else
                {
                    gridControlGia.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi load data: " + ex.Message);
            }
        }
        private async Task<DataTable> FetchVatTu(string url)
        {
            var json = await _client.GetStringAsync(url);

            return await Task.Run(() =>
                JsonConvert.DeserializeObject<DataTable>(json)
            );
        }
        private void searchLookUpEditVatTu_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string maVTID = Convert.ToString(searchLookUpEditVatTu.EditValue);

                if (string.IsNullOrWhiteSpace(maVTID))
                    return;

                gridViewGia.BeginUpdate();
                try
                {
                    gridViewGia.CloseEditor();
                    gridViewGia.UpdateCurrentRow();

                    gridViewGia.ActiveFilterString = $"[MaVTID] = '{maVTID}'";
                }
                finally
                {
                    gridViewGia.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi chọn vật tư: " + ex.Message);
            }
        }

        private void BindVatTuLookup()
        {
            if (_tblVatTu == null || _tblVatTu.Rows.Count == 0) return;

            searchLookUpEditVatTu.Properties.DataSource = _tblVatTu;

            if (!_isVatTuLoaded)
            {
                searchLookUpEditVatTu.Properties.DisplayMember = "MaVT";
                searchLookUpEditVatTu.Properties.ValueMember = "MaVTID";

                GridView view = searchLookUpEditVatTu.Properties.View as GridView;
                view.Columns.Clear();
                view.Columns.AddVisible("MaVT", "Item Code");
                view.Columns.AddVisible("ChiTiet", "Mô Tả");
                view.Columns.AddVisible("MauVT", "Màu");
                view.Columns.AddVisible("KhoVai", "Khổ/Size");

                _isVatTuLoaded = true;
            }
        }

        private bool IsDateOnlyChanged(DataRow row)
        {
            DateTime? ngayAD = GetDate(row, "NgayApDung");
            DateTime? ngayADOld = GetDate(row, "NgayApDung_Old");

            decimal? gia = GetDecimal(row, "DonGia");
            decimal? giaOld = GetDecimal(row, "DonGia_Old");

            if (!ngayAD.HasValue || !ngayADOld.HasValue)
                return false;

            bool changedDate =
                ngayAD.Value.Date != ngayADOld.Value.Date;

            bool changedPrice =
                gia != giaOld;

            bool changedTime =
                ngayAD.Value.TimeOfDay != ngayADOld.Value.TimeOfDay;

            // chỉ đổi ngày, không đổi giá, không đổi giờ
            return changedDate && !changedPrice && !changedTime;
        }
        private bool IsRowChanged(DataRow r)
        {
          
            DateTime? ngayAD = GetDate(r, "NgayApDung");
            DateTime? ngayADOld = GetDate(r, "NgayApDung_Old");

            DateTime? ngayKT = GetDate(r, "NgayKetThuc");
            DateTime? ngayKTOld = GetDate(r, "NgayKetThuc_Old");

            decimal? gia = GetDecimal(r, "DonGia");
            decimal? giaOld = GetDecimal(r, "DonGia_Old");

            string tienTe = GetString(r, "DonViTienTe")?.Trim();
            string tienTeOld = GetString(r, "DonViTienTe_Old")?.Trim();

            return ngayAD != ngayADOld
                || ngayKT != ngayKTOld
                || gia != giaOld
                || tienTe != tienTeOld;

        }
        private void LoadGiaFromSelected(string maVTID)
        {
            try
            {
                if (_tblVatTu == null)
                {
                    gridControlGia.DataSource = null;
                    return;
                }

                DataRow[] rows = _tblVatTu.AsEnumerable()
                    .Where(r => r["MaVTID"] != DBNull.Value &&
                                r["MaVTID"].ToString().Trim() == maVTID.Trim())
                    .ToArray();

                DataTable viewTable;

                if (rows.Length > 0)
                    viewTable = rows.CopyToDataTable();
                else
                    viewTable = _tblVatTu.Clone();

                gridControlGia.DataSource = _tblVatTu;

                gridViewGia.ActiveFilterString = $"MaVTID = '{maVTID}'";

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi load giá: " + ex.Message);
            }

        }


        private void SnapshotOldValues(DataTable dt)
        {
            if (dt == null) return;

            if (!dt.Columns.Contains("NgayApDung_Old")) dt.Columns.Add("NgayApDung_Old", typeof(DateTime));
            if (!dt.Columns.Contains("NgayKetThuc_Old")) dt.Columns.Add("NgayKetThuc_Old", typeof(DateTime));
            if (!dt.Columns.Contains("DonGia_Old")) dt.Columns.Add("DonGia_Old", typeof(decimal));
            if (!dt.Columns.Contains("DonViTienTe_Old")) dt.Columns.Add("DonViTienTe_Old", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                row["NgayApDung_Old"] = row["NgayApDung"];
                row["NgayKetThuc_Old"] = row["NgayKetThuc"];
                row["DonGia_Old"] = row["DonGia"];
                row["DonViTienTe_Old"] = row["DonViTienTe"];
            }
        }
        private async Task LoadVatTuByTenNhom(string tenNhom)
        {
            try
            {
                string encoded = Uri.EscapeDataString(tenNhom);
                string url = $"{URL}ERPVatTuBOM/GetGiaByTenNhom?tenNhom={encoded}";

                var dt = await FetchVatTu(url);

                if (dt == null || dt.Rows.Count == 0)
                {
                    LoadEmptyData();
                    return;
                }

                _tblVatTu = NormalizeDateColumns(dt);


                if (!_tblVatTu.Columns.Contains("KhoVaiID"))
                    _tblVatTu.Columns.Add("KhoVaiID", typeof(string));

                if (!_tblVatTu.Columns.Contains("MauVTID"))
                    _tblVatTu.Columns.Add("MauVTID", typeof(string));

                if (!_tblVatTu.Columns.Contains("MaCLVTID"))
                    _tblVatTu.Columns.Add("MaCLVTID", typeof(string));


                if (!_tblVatTu.Columns.Contains("IsImportChanged"))
                    _tblVatTu.Columns.Add("IsImportChanged", typeof(bool)).DefaultValue = false;

                if (!_tblVatTu.Columns.Contains("IsCheck"))
                    _tblVatTu.Columns.Add("IsCheck", typeof(bool)).DefaultValue = false;

 
                foreach (DataRow r in _tblVatTu.Rows)
                {
                    if (r["MaCLVTID"] == DBNull.Value || string.IsNullOrEmpty(r["MaCLVTID"].ToString()))
                    {
                        if (_tblVatTu.Columns.Contains("MaNhom") && r["MaNhom"] != DBNull.Value)
                            r["MaCLVTID"] = r["MaNhom"];
                    }
                }

                SnapshotOldValues(_tblVatTu);
                gridControlGia.DataSource = _tblVatTu;
               
                ApplyGridFormat();

                string keyword = txtMaVT.EditValue?.ToString()?.Trim();
                string maVTID = Convert.ToString(searchLookUpEditVatTu.EditValue);

           
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    string safeKeyword = keyword.Replace("'", "''");

                    if (_tblVatTu == null)
                        return;

                    DataView dv = new DataView(_tblVatTu);

                    if (!string.IsNullOrWhiteSpace(maVTID))
                    {
                        dv.RowFilter = $"MaVTID = '{maVTID}' AND MaVT LIKE '%{safeKeyword}%'";
                    }
                    else
                    {
                        dv.RowFilter = $"MaVT LIKE '%{safeKeyword}%'";
                    }

           
                    gridControlGia.DataSource = dv;

                    _isSearchMode = true;
                }
                else
                {
                  
                    gridControlGia.DataSource = null;
                    gridControlGia.DataSource = _tblVatTu;

                    _isSearchMode = false;
                }


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi load vật tư: " + ex.Message);
            }
        }
        private bool _isLoading = false;

        private async void searchLookUpEditNhom_EditValueChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            try
            {
                _isLoading = true;
                GridView view = searchLookUpEditNhom.Properties.View as GridView;
                DataRow row = view.GetFocusedDataRow();
                if (row == null) return;
                string tenNhom = row["TenCLVT"]?.ToString()?.Trim();
                if (string.IsNullOrEmpty(tenNhom)) return;

                await LoadVatTuByTenNhom(tenNhom);
        
                searchLookUpEditVatTu.EditValueChanged -= searchLookUpEditVatTu_EditValueChanged;
                searchLookUpEditVatTu.EditValue = null;
                searchLookUpEditVatTu.Properties.DataSource = null;
                BindVatTuLookup();
           
                if (_tblVatTu != null && _tblVatTu.Rows.Count > 0)
                {
                    gridControlGia.DataSource = _tblVatTu;
                    ApplyGridFormat();
                    _isHistoryMode = false;
                    btnChangeHistory.Enabled = true;

                }
                else
                {
                    LoadEmptyData();
                }




            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi chọn nhóm: " + ex.Message);
            }
            finally
            {
                searchLookUpEditVatTu.EditValueChanged += searchLookUpEditVatTu_EditValueChanged;
                _isLoading = false;
            }
        }
        private string GetString(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value
                ? row[col].ToString()
                : null;
        }

        private DateTime? GetDate(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return null;

            object value = row[col];

            if (value is DateTime dt)
                return dt;

            string input = value.ToString().Trim();

            DateTime result;

            if (DateTime.TryParseExact(
                input,
                new[]
                {
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-dd"
                },
                CultureInfo.GetCultureInfo("vi-VN"),
                DateTimeStyles.None,
                out result))
            {
                return result;
            }

            return null;
        }

        private decimal? GetDecimal(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value)
                return null;

            decimal result;
            return decimal.TryParse(row[col].ToString(), out result)
                ? result
                : (decimal?)null;
        }
        private async Task ReloadGiaAfterSave()
        {
            try
            {
                string tenNhom = searchLookUpEditNhom.Text;
                if (string.IsNullOrEmpty(tenNhom)) return;

                string encoded = Uri.EscapeDataString(tenNhom);
                string url = $"{URL}ERPVatTuBOM/GetGiaByTenNhom?tenNhom={encoded}";

                var dt = await FetchVatTu(url);
                if (dt == null) return;

                var normalized = NormalizeDateColumns(dt);

                if (!normalized.Columns.Contains("IsCheck"))
                    normalized.Columns.Add("IsCheck", typeof(bool)).DefaultValue = false;

                SnapshotOldValues(normalized);

                _tblVatTu = normalized;
                gridControlGia.DataSource = null;
                gridControlGia.DataSource = _tblVatTu;
                
                string keyword = txtMaVT.EditValue?.ToString()?.Trim();
                string maVTID = Convert.ToString(searchLookUpEditVatTu.EditValue);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    
                    string safeKeyword = keyword.Replace("'", "''");
                    DataView dv = new DataView(_tblVatTu);

                    if (!string.IsNullOrWhiteSpace(maVTID))
                        dv.RowFilter = $"MaVTID = '{maVTID}' AND MaVT LIKE '%{safeKeyword}%'";
                    else
                        dv.RowFilter = $"MaVT LIKE '%{safeKeyword}%'";

                    gridControlGia.DataSource = null;
                    gridControlGia.DataSource = dv;
                    _isSearchMode = true;
                }
                else
                {
                    
                    gridControlGia.DataSource = null;
                    gridControlGia.DataSource = _tblVatTu;
                    _isSearchMode = false;
                }
                ApplyGridFormat();
                gridViewGia.RefreshData();

                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi reload giá: " + ex.Message);
            }
        }


       
 

private bool IsValidRow(DataRow row)
{
    if (row == null) return false;

    bool hasNgayAD = row["NgayApDung"] != DBNull.Value;
    bool hasGia = row["DonGia"] != DBNull.Value;
    bool hasTienTe = row["DonViTienTe"] != DBNull.Value &&
                     !string.IsNullOrWhiteSpace(row["DonViTienTe"].ToString());

    return hasNgayAD && hasGia && hasTienTe;
}
        private VatTuGiaDto MapDto(DataRow row)
        {
            string maCLVTID = GetString(row, "MaCLVTID")
                           ?? GetString(row, "MaNhom")
                           ?? searchLookUpEditNhom.EditValue?.ToString()
                           ?? "";

            return new VatTuGiaDto
            {
  
                ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
                MaVTID = GetString(row, "MaVTID")?.Trim(),
                MaDVVT = GetString(row, "MaDVVT")?.Trim(),
                MaCLVTID = maCLVTID?.Trim(),
                MaMauVT = GetString(row, "MaMauVT"),
                MauVTID = GetString(row, "MauVTID")?.Trim(),
                KhoVaiID = GetString(row, "KhoVaiID")?.Trim(),

        
                DonGia = GetDecimal(row, "DonGia"),
                DonViTienTe = GetString(row, "DonViTienTe")?.Trim(),
                NgayApDung = row["NgayApDung"] == DBNull.Value
    ? (DateTime?)null
    : (DateTime)row["NgayApDung"],
                NgayKetThuc = row["NgayKetThuc"] == DBNull.Value
    ? (DateTime?)null
    : Convert.ToDateTime(row["NgayKetThuc"]),

                NguoiTao = GlobleData.UserName


            };
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
                        excelPackage.Workbook.Properties.Title = "GiaThongSoVatTu";

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

        private void btnXuatMauExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();

            Sfd.Filter = "Excel (*.xlsx)|*.xlsx";

       
            string autoFileName = $"GiaThongSoTemplate_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            Sfd.FileName = autoFileName;

  
            Sfd.OverwritePrompt = false;

            if (Sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string templatePath = Path.Combine(Application.StartupPath, @"Templates\", "GiaThongSoTemplate.xlsx");

                if (!File.Exists(templatePath))
                {
                    XtraMessageBox.Show("Không tìm thấy file template");
                    return;
                }

     
                string selectedFolder = Path.GetDirectoryName(Sfd.FileName);

                string finalPath = Path.Combine(selectedFolder, autoFileName);

        
                File.Copy(templatePath, finalPath, true);

                DataTable dt = (gridViewGia.DataSource as DataView)?.Table
              ?? gridControlGia.DataSource as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất");
                    return;
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(new FileInfo(finalPath)))
                {
                    var ws = package.Workbook.Worksheets[0];

                    int startRow = 4;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var row = dt.Rows[i];
                        int r = startRow + i;

                     

                        ws.Cells[r, 1].Value = searchLookUpEditNhom.Text;
                        ws.Cells[r, 2].Value = row["MaVT"];
                        ws.Cells[r, 3].Value = row["ChiTiet"];

             
                        ws.Cells[r, 4].Value = row.Table.Columns.Contains("MaMauVT")
                            ? row["MaMauVT"]
                            : "";

              
                        ws.Cells[r, 5].Value = row["MauVT"];

              
                        ws.Cells[r, 6].Value = row["KhoVai"];

              
                        ws.Cells[r, 7].Value = row["TenDVVT"];

          
                        ws.Cells[r, 8].Value = null;
                        ws.Cells[r, 9].Value = null;
                        ws.Cells[r, 10].Value = null;
                        ws.Cells[r, 11].Value = null;

                        ws.Cells[r, 12].Value = row["GhiChu"];
                    }

  
                    ws.Column(8).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                    ws.Column(9).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                    ws.Column(10).Style.Numberformat.Format = "#,##0.00"; 


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int r = startRow + i;

                        ws.Cells[r, 8].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                        ws.Cells[r, 9].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                    }

                    package.Save();
                }

                if (XtraMessageBox.Show("Mở file?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(finalPath);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("" + ex.Message);
            }
        }
        private object SafeValue(object value)
        {
            if (value == DBNull.Value || value == null)
                return "";

            return value;
        }
        private object SafeDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return null; 

            if (value is DateTime dt)
                return dt; 

            DateTime parsed;
            if (DateTime.TryParse(value.ToString(), out parsed))
                return parsed;

            return null; 
        }

        private object SafeDecimal(object value)
        {
            if (value == DBNull.Value || value == null)
                return "";

            decimal d;
            return decimal.TryParse(value.ToString(), out d)
                ? (object)d
                : "";
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.xlsx)|*.xlsx";
            sfd.FileName = $"GiaVatTu_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang xử lý...");

                DataTable dt = gridControlGia.DataSource as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất");
                    return;
                }

                string templatePath = Path.Combine(Application.StartupPath, "Templates", "GiaThongSoTemplate.xlsx");

                if (!File.Exists(templatePath))
                {
                    XtraMessageBox.Show("Không tìm thấy file template");
                    return;
                }

         
                File.Copy(templatePath, sfd.FileName, true);

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(new FileInfo(sfd.FileName)))
                {
                    var ws = package.Workbook.Worksheets[0];

                    int startRow = 4;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var row = dt.Rows[i];
                        int r = startRow + i;

                      

                        ws.Cells[r, 1].Value = SafeValue(searchLookUpEditNhom.Text);
                        ws.Cells[r, 2].Value = SafeValue(row["MaVT"]);
                        ws.Cells[r, 3].Value = SafeValue(row["ChiTiet"]);

                        
                        ws.Cells[r, 4].Value = SafeValue(
                            dt.Columns.Contains("MaMauVT") ? row["MaMauVT"] : ""
                        );

                  
                        ws.Cells[r, 5].Value = SafeValue(row["MauVT"]);

                        
                        ws.Cells[r, 6].Value = SafeValue(row["KhoVai"]);

                    
                        ws.Cells[r, 7].Value = SafeValue(row["TenDVVT"]);

                        ws.Cells[r, 8].Value = SafeDate(row["NgayApDung"]);

                        var ngayKT = GetDate(row, "NgayKetThuc");
                        if (ngayKT.HasValue)
                            ws.Cells[r, 9].Value = ngayKT.Value;

                        ws.Cells[r, 10].Value = SafeDecimal(row["DonGia"]);
                        ws.Cells[r, 11].Value = SafeValue(row["DonViTienTe"]);

                        ws.Cells[r, 12].Value = dt.Columns.Contains("GhiChu")
                            ? SafeValue(row["GhiChu"])
                            : "";
                    }

                    if (dt.Rows.Count > 0)
                    {
                        ws.Column(8).Style.Numberformat.Format = "[$-vi-VN]dd/mm/yyyy hh:mm";
                        ws.Column(9).Style.Numberformat.Format = "[$-vi-VN]dd/mm/yyyy hh:mm";
                        ws.Column(10).Style.Numberformat.Format = "#,##0.00";
                    }

                    package.Save();
                }

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                if (XtraMessageBox.Show("Mở file?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show("Lỗi: " + ex.Message);
            }
        }



        private void ImportExcelToGrid(string filePath)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var ws = package.Workbook.Worksheets[0];
                    DataTable dtGrid = gridControlGia.DataSource as DataTable;

                    if (!dtGrid.Columns.Contains("MaMauVT")) dtGrid.Columns.Add("MaMauVT", typeof(string));
                    if (!dtGrid.Columns.Contains("KhoVai")) dtGrid.Columns.Add("KhoVai", typeof(string));
                    if (!dtGrid.Columns.Contains("TenDVVT")) dtGrid.Columns.Add("TenDVVT", typeof(string));
                    if (!dtGrid.Columns.Contains("ChiTiet")) dtGrid.Columns.Add("ChiTiet", typeof(string));

                    if (dtGrid == null || dtGrid.Rows.Count == 0)
                    {
                        XtraMessageBox.Show("Grid chưa có dữ liệu");
                        return;
                    }

                    if (dtGrid.Columns.Contains("IsImportChanged"))
                    {
                        foreach (DataRow r in dtGrid.Rows) r["IsImportChanged"] = false;
                    }

                    int startRow = 4;
                    int row = startRow;
                    int updatedCount = 0;
              

                   
                    while (true)
                    {
                        if (ws.Cells[row, 2].Value == null) break;

                        string maVT = ws.Cells[row, 2].Text?.Trim() ?? "";        // B
                        string excelChiTiet = ws.Cells[row, 3].Text?.Trim() ?? ""; // C
                        string maMauVT = ws.Cells[row, 4].Text?.Trim() ?? "";      // D
                        string mauVT = ws.Cells[row, 5].Text?.Trim() ?? "";        // E
                        string khoVai = ws.Cells[row, 6].Text?.Trim() ?? "";       // F
                        string excelDonVi = ws.Cells[row, 7].Text?.Trim() ?? "";   // G

                        if (string.IsNullOrEmpty(maVT))
                        {
                            row++;
                            continue;
                        }


                        #region MATCH ROW (ROBUST - NULL SAFE)

                        string Normalize(string val) => (val ?? "").Trim().ToUpper();

                        var existRow = dtGrid.AsEnumerable().FirstOrDefault(r =>
                        {
                            string rMaVT = Normalize(r["MaVT"]?.ToString());
                            string rMau = Normalize(r.Table.Columns.Contains("MauVT") ? r["MauVT"]?.ToString() : null);
                            string rKhoVai = Normalize(r.Table.Columns.Contains("KhoVai") ? r["KhoVai"]?.ToString() : null);
                            string rDonVi = Normalize(r.Table.Columns.Contains("TenDVVT") ? r["TenDVVT"]?.ToString() : null);
                            string rChiTiet = Normalize(r.Table.Columns.Contains("ChiTiet") ? r["ChiTiet"]?.ToString() : null);
                            string rMaMau = Normalize(r.Table.Columns.Contains("MaMauVT") ? r["MaMauVT"]?.ToString() : null);

                            string eMaVT = Normalize(maVT);
                            string eMau = Normalize(mauVT);
                            string eKhoVai = Normalize(khoVai);
                            string eDonVi = Normalize(excelDonVi);
                            string eChiTiet = Normalize(excelChiTiet);
                            string eMaMau = Normalize(maMauVT);

                            
                            if (rMaVT != eMaVT) return false;

                            
                            if (!string.IsNullOrEmpty(eMau) && rMau != eMau) return false;
                            if (!string.IsNullOrEmpty(eKhoVai) && rKhoVai != eKhoVai) return false;
                            if (!string.IsNullOrEmpty(eDonVi) && rDonVi != eDonVi) return false;
                            if (!string.IsNullOrEmpty(eChiTiet) && rChiTiet != eChiTiet) return false;
                            if (!string.IsNullOrEmpty(eMaMau) && rMaMau != eMaMau) return false;

                            return true;
                        });

                        #endregion
                        if (existRow == null)
                        {
                            XtraMessageBox.Show(
                                $"Dòng {row}: Không tìm thấy vật tư trong hệ thống.\nMaVT: {maVT}\nMàu: {mauVT}\nKhổ: {khoVai}\n\nImport bị dừng.",
                                "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        bool hasChange = false;

                        
                        var ngayAD = GetExcelDate(ws.Cells[row, 8]);
                        if (ngayAD.HasValue)
                        {
                            existRow["NgayApDung"] = ngayAD.Value;
                            hasChange = true;
                        }


                        var ngayKT = GetExcelDate(ws.Cells[row, 9]);
                        if (ngayKT.HasValue)
                        {
                            existRow["NgayKetThuc"] = ngayKT.Value;
                            hasChange = true;
                        }
                        else
                        {
                            existRow["NgayKetThuc"] = DBNull.Value;
                            hasChange = true;
                        }

                     
                        if (ws.Cells[row, 10].Value != null)
                        {
                            if (decimal.TryParse(ws.Cells[row, 10].Value.ToString(), out decimal gia))
                            {
                                existRow["DonGia"] = gia;
                                hasChange = true;
                            }
                        }

                       
                        string tienTe = ws.Cells[row, 11].Text?.Trim() ?? "";
                        if (!string.IsNullOrEmpty(tienTe))
                        {
                            existRow["DonViTienTe"] = tienTe;
                            hasChange = true;
                        }
              

                      
                        if (hasChange && dtGrid.Columns.Contains("IsImportChanged"))
                        {
                            existRow["IsImportChanged"] = true;
                            updatedCount++;
                        }

                        row++;
                    }

                    gridViewGia.RefreshData();
                    XtraMessageBox.Show($"Import thành công dòng.");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi import: " + ex.Message);
            }
        }
        private DateTime? GetExcelDate(ExcelRange cell)
        {
            if (cell == null || cell.Value == null) return null;

       
            if (cell.Value is DateTime dt)
                return dt;

            
            if (cell.Value is double oa || double.TryParse(cell.Value.ToString(), out oa))
            {
                try
                {
                    return DateTime.FromOADate(oa);
                }
                catch { }
            }

         
            string text = cell.Text?.Trim() ?? "";
            if (DateTime.TryParse(text, out DateTime res1)) return res1;

            if (DateTime.TryParseExact(text,
                new[] { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy HH:mm:ss", "M/d/yyyy" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime res2))
            {
                return res2;
            }

            return null;
        }

        //private void btnTimKiem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
              

        //        var frm = new frmERP_VatTuNganHangGiaBoLoc(_maVTID, _maVT, _tenVT);

        
        //        frm.TenNhom = searchLookUpEditNhom.Text;
        //        frm.MaNhom = searchLookUpEditNhom.EditValue?.ToString();

            
        //        frm.ExternalGrid = gridControlGia;
        //        frm.ExternalGridView = gridViewGia;

           
        //        DataTable dt = gridControlGia.DataSource as DataTable;
        //        if (dt != null)
        //        {
        //            frm.SetData(dt);
        //        }

        //        frm.ShowDialog();
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Lỗi mở bộ lọc: " + ex.Message);
        //    }
        //}

        private void gridViewGia_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "SoThuTu")
            {
                GridView view = sender as GridView;

                if (view == null) return;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                if (rowHandle >= 0)
                {
                    e.DisplayText = (rowHandle + 1).ToString();
                }
            }
        }




        //private async void txtMaVT_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        string keyword = txtMaVT.EditValue?.ToString().Trim();

        //        if (string.IsNullOrWhiteSpace(keyword))
        //        {
        //            XtraMessageBox.Show("Nhập mã vật tư");
        //            return;
        //        }


        //        string url = $"{URL}ERPVatTuBOM/GetVatTuByMaVT?keyword={Uri.EscapeDataString(keyword)}";

        //        var json = await _client.GetStringAsync(url);
        //        DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            XtraMessageBox.Show("Không tìm thấy vật tư");
        //            return;
        //        }

        //        string tenNhom = dt.Rows[0]["TenCLVT"]?.ToString()?.Trim();

        //        if (string.IsNullOrEmpty(tenNhom))
        //        {
        //            XtraMessageBox.Show("Không xác định được nhóm");
        //            return;
        //        }


        //        await LoadVatTuByTenNhom(tenNhom);

        //        if (!_tblVatTu.Columns.Contains("MaVT_Search"))
        //        {
        //            _tblVatTu.Columns.Add("MaVT_Search", typeof(string));
        //        }

        //        foreach (DataRow r in _tblVatTu.Rows)
        //        {
        //            r["MaVT_Search"] = r["MaVT"]?.ToString().Trim().ToUpper();
        //        }


        //        string keywordSearch = keyword.Trim().ToUpper();

        //        DataView dv = _tblVatTu.DefaultView;
        //        dv.RowFilter = $"MaVT_Search LIKE '%{keywordSearch}%'";


        //        if (gridControlGia != null)
        //        {
        //            gridControlGia.DataSource = dv;
        //            gridControlGia.RefreshDataSource();
        //        }

        //        if (gridViewGia != null)
        //        {
        //            gridViewGia.RefreshData();
        //            gridViewGia.BestFitColumns();
        //        }


        //        if (!_tblVatTu.Columns.Contains("MaVT_Search"))
        //        {
        //            _tblVatTu.Columns.Add("MaVT_Search", typeof(string));

        //            foreach (DataRow r in _tblVatTu.Rows)
        //            {
        //                r["MaVT_Search"] = r["MaVT"]?.ToString().Trim().ToUpper();
        //            }
        //        }




        //        dv.RowFilter = $"MaVT_Search LIKE '%{keywordSearch}%'";
        //        _isSearchMode = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
        //    }
        //}
        private void txtMaVT_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
          
                string keyword = txtMaVT.EditValue?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    XtraMessageBox.Show("Nhập item code");
                    return;
                }

                string maVTID = Convert.ToString(searchLookUpEditVatTu.EditValue);
                string safeKeyword = keyword.Replace("'", "''");
              
                gridViewGia.BeginUpdate();
                try
                {
                    gridViewGia.CloseEditor();
                    gridViewGia.UpdateCurrentRow();
                

                    if (_tblVatTu == null)
                    {
                        XtraMessageBox.Show("Chưa có dữ liệu");
                        return;
                    }

                
                    DataView dv = new DataView(_tblVatTu);

                    if (!string.IsNullOrWhiteSpace(maVTID))
                    {
                        dv.RowFilter = $"MaVTID = '{maVTID}' AND MaVT LIKE '%{safeKeyword}%'";
                    }
                    else
                    {
                        dv.RowFilter = $"MaVT LIKE '%{safeKeyword}%'";
                    }

                gridControlGia.DataSource = dv;
                gridControlGia.RefreshDataSource();
            }
                finally
                {
                    gridViewGia.EndUpdate();
                }

     


            
           
        }
      

        private void btnGiaVTHetHieuLuc_Click(object sender, EventArgs e)
        {
            try
            {
             
                DataTable source = (gridControlGia.DataSource as DataView)?.Table
                                 ?? gridControlGia.DataSource as DataTable;

                if (source == null || source.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu");
                    return;
                }

                DateTime today = DateTime.Today;

                var filtered = source.AsEnumerable()
                    .Where(r =>
                    {
                        if (!source.Columns.Contains("NgayKetThuc") ||
                            r["NgayKetThuc"] == DBNull.Value)
                            return false;

                        DateTime end;

               
                if (r["NgayKetThuc"] is DateTime dt)
                            end = dt.Date;
                        else if (!DateTime.TryParse(r["NgayKetThuc"].ToString(), out end))
                            return false;

                        return end <= today;
                    })
                    .OrderByDescending(r =>
                    {
                        if (r["NgayKetThuc"] is DateTime dt)
                            return dt;

                        DateTime tmp;
                        DateTime.TryParse(r["NgayKetThuc"].ToString(), out tmp);
                        return tmp;
                    });

                DataTable result = filtered.Any()
                    ? filtered.CopyToDataTable()
                    : source.Clone();

      
                gridControlGia.DataSource = null;
                gridControlGia.DataSource = result;

                gridViewGia.RefreshData();

                if (result.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có giá nào hết hiệu lực");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi lọc giá hết hiệu lực: " + ex.Message);
            }
        }
       



     

        private void ApplyDateRangeFilter()
        {
            try
            {
                if (dateEdit111.EditValue == null || dateEdit22.EditValue == null)
                {
                   
                    gridViewGia.ActiveFilterString = "";
                    return;
                }

                DateTime from = Convert.ToDateTime(dateEdit111.EditValue).Date;
                DateTime to = Convert.ToDateTime(dateEdit22.EditValue).Date;

                if (from > to)
                    return;

                string filter = $@"
            [NgayApDung] >= #{from:MM/dd/yyyy}# 
            AND 
            [NgayKetThuc] <= #{to:MM/dd/yyyy}#";

                gridViewGia.ActiveFilterString = filter;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi filter ngày: " + ex.Message);
            }
        }


        private void dateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (dateEdit111.EditValue == null || dateEdit22.EditValue == null)
                return;

            DateTime from = Convert.ToDateTime(dateEdit111.EditValue).Date;
            DateTime to = Convert.ToDateTime(dateEdit22.EditValue).Date;

            if (from > to)
            {
                XtraMessageBox.Show("Từ ngày không được lớn hơn đến ngày");
                dateEdit111.EditValue = null;
                return;
            }

          
            ApplyDateRangeFilter();
        }

        private void dateEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (dateEdit111.EditValue == null || dateEdit22.EditValue == null)
                return;

            DateTime from = Convert.ToDateTime(dateEdit111.EditValue).Date;
            DateTime to = Convert.ToDateTime(dateEdit22.EditValue).Date;

            if (from > to)
            {
                XtraMessageBox.Show("Đến ngày phải >= từ ngày");
                dateEdit22.EditValue = null;
                return;
            }

      
            ApplyDateRangeFilter();
        }

        private void dateEdit11_ItemClick(object sender, EventArgs e)
        {
            if (dateEdit111.EditValue == null || dateEdit22.EditValue == null)
                return;

            DateTime from = Convert.ToDateTime(dateEdit111.EditValue).Date;
            DateTime to = Convert.ToDateTime(dateEdit22.EditValue).Date;

            if (from > to)
            {
                XtraMessageBox.Show("Từ ngày không được lớn hơn đến ngày");
                dateEdit111.EditValue = null;
                return;
            }

       
            ApplyDateRangeFilter();
        }

        private void dateEdit2_ItemClick(object sender, EventArgs e)
        {
            if (dateEdit111.EditValue == null || dateEdit22.EditValue == null)
                return;

            DateTime from = Convert.ToDateTime(dateEdit111.EditValue).Date;
            DateTime to = Convert.ToDateTime(dateEdit22.EditValue).Date;

            if (from > to)
            {
                XtraMessageBox.Show("Đến ngày phải >= từ ngày");
                dateEdit22.EditValue = null;
                return;
            }

    
            ApplyDateRangeFilter();
        }

        private void btnXuatMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();

            Sfd.Filter = "Excel (*.xlsx)|*.xlsx";

   
            string autoFileName = $"GiaThongSoTemplate_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            Sfd.FileName = autoFileName;

            Sfd.OverwritePrompt = false;

            if (Sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string templatePath = Path.Combine(Application.StartupPath, @"Templates\", "GiaThongSoTemplate.xlsx");

                if (!File.Exists(templatePath))
                {
                    XtraMessageBox.Show("Không tìm thấy file template");
                    return;
                }

    
                string selectedFolder = Path.GetDirectoryName(Sfd.FileName);

                string finalPath = Path.Combine(selectedFolder, autoFileName);

            
                File.Copy(templatePath, finalPath, true);

                DataTable dt = (gridViewGia.DataSource as DataView)?.Table
              ?? gridControlGia.DataSource as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất");
                    return;
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(new FileInfo(finalPath)))
                {
                    var ws = package.Workbook.Worksheets[0];

                    int startRow = 4;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var row = dt.Rows[i];
                        int r = startRow + i;

                        
                        ws.Cells[r, 1].Value = searchLookUpEditNhom.Text;
                        ws.Cells[r, 2].Value = row["MaVT"];
                        ws.Cells[r, 3].Value = row["ChiTiet"];

                        
                        ws.Cells[r, 4].Value = row.Table.Columns.Contains("MaMauVT")
                            ? row["MaMauVT"]
                            : "";

                        
                        ws.Cells[r, 5].Value = row["MauVT"];

                        
                        ws.Cells[r, 6].Value = row["KhoVai"];

                     
                        ws.Cells[r, 7].Value = row["TenDVVT"];

                   
                        ws.Cells[r, 8].Value = null;
                        ws.Cells[r, 9].Value = null;
                        ws.Cells[r, 10].Value = null;
                        ws.Cells[r, 11].Value = null;


                        ws.Cells[r, 12].Value = row.Table.Columns.Contains("DanhSachPhieuMH")
                                ? row["DanhSachPhieuMH"]
                                : "";

                        ws.Cells[r, 13].Value = row["GhiChu"];
                    }

                
                    ws.Column(8).Style.Numberformat.Format = "dd/MM/yyyy HH:mm"; 
                    ws.Column(9).Style.Numberformat.Format = "dd/MM/yyyy HH:mm"; 
                    ws.Column(10).Style.Numberformat.Format = "#,##0.00";  

                  
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int r = startRow + i;

                        ws.Cells[r, 8].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                        ws.Cells[r, 9].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                    }

                    package.Save();
                }

                if (XtraMessageBox.Show("Mở file?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(finalPath);
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnNhapExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel (*.xlsx)|*.xlsx";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            ImportExcelToGrid(ofd.FileName);
        }

        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.xlsx)|*.xlsx";
            sfd.FileName = $"GiaVatTu_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang xử lý...");

                DataTable dt = gridControlGia.DataSource as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất");
                    return;
                }

                string templatePath = Path.Combine(Application.StartupPath, "Templates", "GiaThongSoTemplate.xlsx");

                if (!File.Exists(templatePath))
                {
                    XtraMessageBox.Show("Không tìm thấy file template");
                    return;
                }

            
                File.Copy(templatePath, sfd.FileName, true);

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(new FileInfo(sfd.FileName)))
                {
                    var ws = package.Workbook.Worksheets[0];

                    int startRow = 4;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var row = dt.Rows[i];
                        int r = startRow + i;

                       

                        ws.Cells[r, 1].Value = SafeValue(searchLookUpEditNhom.Text);
                        ws.Cells[r, 2].Value = SafeValue(row["MaVT"]);
                        ws.Cells[r, 3].Value = SafeValue(row["ChiTiet"]);

                     
                        ws.Cells[r, 4].Value = SafeValue(
                            dt.Columns.Contains("MaMauVT") ? row["MaMauVT"] : ""
                        );

                  
                        ws.Cells[r, 5].Value = SafeValue(row["MauVT"]);

                     
                        ws.Cells[r, 6].Value = SafeValue(row["KhoVai"]);

                   
                        ws.Cells[r, 7].Value = SafeValue(row["TenDVVT"]);

                        ws.Cells[r, 8].Value = SafeDate(row["NgayApDung"]);

                        var ngayKT = GetDate(row, "NgayKetThuc");
                        if (ngayKT.HasValue)
                            ws.Cells[r, 9].Value = ngayKT.Value;

                        ws.Cells[r, 10].Value = SafeDecimal(row["DonGia"]);
                        ws.Cells[r, 11].Value = SafeValue(row["DonViTienTe"]);

                        ws.Cells[r, 12].Value = dt.Columns.Contains("DanhSachPhieuMH")
                            ? SafeValue(row["DanhSachPhieuMH"])
                            : "";

                        ws.Cells[r, 13].Value = dt.Columns.Contains("GhiChu")
                            ? SafeValue(row["GhiChu"])
                            : "";
                    }

                    if (dt.Rows.Count > 0)
                    {
                        ws.Column(8).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                        ws.Column(9).Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                        ws.Column(10).Style.Numberformat.Format = "#,##0.00";
                    }

                    package.Save();
                }

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                if (XtraMessageBox.Show("Mở file?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void MarkCurrentEffectiveRow()
        {
            _currentEffectiveRowHandle = -1;

            GridView view = gridViewGia;
            if (view == null || view.RowCount == 0) return;

            DateTime now = DateTime.Now;

            int matchedHandle = -1;
            DateTime maxPastDate = DateTime.MinValue;

            int nearestFutureHandle = -1;
            DateTime minFutureDate = DateTime.MaxValue;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row == null) continue;

                DateTime? ngayAD = GetDate(row, "NgayApDung");
                if (!ngayAD.HasValue) continue;

                if (ngayAD.Value <= now)
                {
                    if (ngayAD.Value > maxPastDate)
                    {
                        maxPastDate = ngayAD.Value;
                        matchedHandle = i;
                    }
                }
                else
                {
                    if (ngayAD.Value < minFutureDate)
                    {
                        minFutureDate = ngayAD.Value;
                        nearestFutureHandle = i;
                    }
                }
            }

            _currentEffectiveRowHandle = matchedHandle >= 0
                ? matchedHandle
                : nearestFutureHandle;
        }

        private async void btnChangeHistory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (_isHistoryMode)
                {
                    XtraMessageBox.Show("Đang ở chế độ lịch sử giá, không thể thực hiện lại.");
                    return;
                }

                if (gridViewGia.FocusedRowHandle < 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn vật tư");
                    return;
                }

                DataRow row = gridViewGia.GetDataRow(gridViewGia.FocusedRowHandle);


                if (row == null)
                {
                    XtraMessageBox.Show("Không có dữ liệu");
                    return;
                }

  
                string maVTID = row["MaVTID"]?.ToString();
                string maDVVT = row["MaDVVT"]?.ToString();
                string maCLVTID =
     row.Table.Columns.Contains("MaCLVTID") &&
     !string.IsNullOrEmpty(row["MaCLVTID"]?.ToString())
         ? row["MaCLVTID"].ToString()
         : row.Table.Columns.Contains("MaNhom")
             ? row["MaNhom"]?.ToString()
             : "";
                string mauVTID = row.Table.Columns.Contains("MauVTID")
                    ? row["MauVTID"]?.ToString()
                    : "";

                string khoVaiID = row.Table.Columns.Contains("KhoVaiID")
                    ? row["KhoVaiID"]?.ToString()
                    : "";
                if (string.IsNullOrEmpty(maVTID))
                {
                    XtraMessageBox.Show("Thiếu MaVTID");
                    return;
                }

       
                string extra = $"{maDVVT}|{maCLVTID}|{mauVTID}|{khoVaiID}";

 
                string url = $"{URL}ERPVatTuBOM/GetGiaHistory" +
                             $"?maVTID={Uri.EscapeDataString(maVTID)}" +
                             $"&extra={Uri.EscapeDataString(extra)}";

                var json = await _client.GetStringAsync(url);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có lịch sử giá");
                    return;
                }

                foreach (DataRow r in dt.Rows)
                {
                    if (r["NgayApDung"] != DBNull.Value)
                        r["NgayApDung"] = Convert.ToDateTime(r["NgayApDung"]);

                    if (r["NgayKetThuc"] != DBNull.Value)
                        r["NgayKetThuc"] = Convert.ToDateTime(r["NgayKetThuc"]);
                }


                if (!dt.Columns.Contains("IsCheck"))
                    dt.Columns.Add("IsCheck", typeof(bool)).DefaultValue = false;

     
                gridControlGia.DataSource = null;
                gridControlGia.DataSource = dt;
                MarkCurrentEffectiveRow();
                gridViewGia.RefreshData();
                gridViewGia.BestFitColumns();

                _isHistoryMode = true;
                SetCurrentEffectiveRowHandle();
                UpdateHistoryUI();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi history: " + ex.Message);
            }
        }




        private void btnGiaVTHetHieuLuc_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {


                DataTable source = (gridControlGia.DataSource as DataView)?.Table
                                 ?? gridControlGia.DataSource as DataTable;

                if (source == null || source.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu");
                    return;
                }

                DateTime today = DateTime.Today;

                var filtered = source.AsEnumerable()
                    .Where(r =>
                    {
                        if (r["NgayApDung"] == DBNull.Value ||
                            r["NgayKetThuc"] == DBNull.Value)
                            return false;

                        DateTime from = Convert.ToDateTime(r["NgayApDung"]);
                        DateTime to = Convert.ToDateTime(r["NgayKetThuc"]);

      
                              if (from == to)
                            return true;

      
                            if (DateTime.Today >= to.Date)
                            return true;

                        return false;
                    })
                    .OrderByDescending(r =>
                    {
                        if (r["NgayKetThuc"] is DateTime dt)
                            return dt;

                        DateTime tmp;
                        DateTime.TryParse(r["NgayKetThuc"].ToString(), out tmp);
                        return tmp;
                    });

                DataTable result = filtered.Any()
                    ? filtered.CopyToDataTable()
                    : source.Clone();

   
                gridControlGia.DataSource = null;
                gridControlGia.DataSource = result;

                gridViewGia.RefreshData();

                if (result.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có giá nào hết hiệu lực");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi lọc giá hết hiệu lực: " + ex.Message);
            }
        }

        private async void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewGia.CloseEditor();
                gridViewGia.UpdateCurrentRow();
                gridViewGia.CloseEditor();
                DataTable dt = (gridViewGia.DataSource as DataView)?.Table
                             ?? gridControlGia.DataSource as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu");
                    return;
                }

      
                var importRows = dt.AsEnumerable()
             .Where(r =>
               dt.Columns.Contains("IsImportChanged") &&
               r["IsImportChanged"] != DBNull.Value &&
               (bool)r["IsImportChanged"])
             .ToList();

                var changedRows = dt.AsEnumerable()
                    .Where(r => IsRowChanged(r))
                    .ToList();

                List<VatTuGiaDto> list = new List<VatTuGiaDto>();

        

                HashSet<string> uniqueKeys = new HashSet<string>();
                int skippedInvalid = 0;
                foreach (DataRow row in dt.Rows)
                {
                    
                    bool isImported = dt.Columns.Contains("IsImportChanged") && row["IsImportChanged"] != DBNull.Value && (bool)row["IsImportChanged"];
                    bool isManuallyChanged = IsRowChanged(row) || row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added;

                 
                    if (!isImported && !isManuallyChanged)
                        continue;

             
                    bool hasNgayAD = row["NgayApDung"] != DBNull.Value;
                    bool hasGia = row["DonGia"] != DBNull.Value;

                    if (!hasNgayAD || !hasGia)
                    {
                        skippedInvalid++;
                        continue;
                    }

                    if (!IsValidRow(row))
                        continue;

                    if (IsDateOnlyChanged(row))
                    {
                        XtraMessageBox.Show(
                            $"Vật tư [{row["MaVT"]}] đã có dữ liệu.\n" +
                            "Bạn chỉ sửa ngày mà không đổi giá nên không được lưu.",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        row["NgayApDung"] = row["NgayApDung_Old"];
                        row["NgayKetThuc"] = row["NgayKetThuc_Old"];

                        continue;
                    }

                    DateTime? ngayAD = GetDate(row, "NgayApDung");
                    DateTime? ngayADOld = GetDate(row, "NgayApDung_Old");

                    decimal? gia = GetDecimal(row, "DonGia");
                    decimal? giaOld = GetDecimal(row, "DonGia_Old");

                    bool changedDate = ngayAD?.Date != ngayADOld?.Date;
                    bool changedTime = ngayAD?.TimeOfDay != ngayADOld?.TimeOfDay;
                    bool changedPrice = gia != giaOld;

                    if ((changedDate || changedTime) && !changedPrice)
                    {
                        XtraMessageBox.Show(
                            $"Vật tư [{row["MaVT"]}] đã có dữ liệu.\n" +
                            "Bạn sửa ngày / giờ nhưng không đổi giá nên không được lưu.",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        row["NgayApDung"] = row["NgayApDung_Old"];
                        row["NgayKetThuc"] = row["NgayKetThuc_Old"];

                        continue;
                    }
                    var dto = MapDto(row);
                    string tienTeSafe = string.IsNullOrWhiteSpace(dto.DonViTienTe) ? "" : dto.DonViTienTe;

                    string key = $"{dto.ID}|{dto.MaVTID}|{dto.MaCLVTID}|{dto.MauVTID}|{dto.KhoVaiID}|{dto.NgayApDung:yyyyMMddHHmmss}|{dto.DonGia}|{tienTeSafe}";

                    if (uniqueKeys.Contains(key))
                        continue;

                    uniqueKeys.Add(key);
                    list.Add(dto);
                }

                
                if (list.Count == 0)
                {
                    if (skippedInvalid > 0)
                        XtraMessageBox.Show("Vui lòng điều chỉnh hoặc xóa và áp dụng lại.");
                    else
                        XtraMessageBox.Show("Không có dữ liệu nào bị thay đổi để lưu.");
                    return;
                }
                var dup = list
                .GroupBy(x => new
                {
                    x.MaVTID,
                    x.MaDVVT,
                    x.MaCLVTID,
                    x.MauVTID,
                    x.KhoVaiID,
                        Date = x.NgayApDung.HasValue
                        ? x.NgayApDung.Value.Date
                        : (DateTime?)null,

                    x.DonGia
                })
                .Where(g => g.Count() > 1)
                .ToList();

                if (dup.Any())
                {
                    string debug = string.Join("\n", dup.Select(g =>
                        $"{g.Key.MaVTID} | {g.Key.Date:dd/MM/yyyy} | {g.Key.DonGia} | Count={g.Count()}"
                    ));

                    XtraMessageBox.Show(
                        "Trùng dữ liệu: Cùng NGÀY + CÙNG GIÁ → KHÔNG ĐƯỢC LƯU\n\n" + debug
                    );
                    return;
                }




                string apiUrl = $"{URL}ERPVatTuBOM/PostVatTuGia";
                string result = await _clientExtension.PostAsync(apiUrl, list);

                if (result == "True")
                {
                
                  
                    foreach (DataRow r in dt.Rows)
                    {
                        if (dt.Columns.Contains("IsImportChanged"))
                            r["IsImportChanged"] = false;
                    }

                    gridViewGia.RefreshData();
                    await ReloadGiaAfterSave();


                }
                else
                {
                    XtraMessageBox.Show("" + result);
                }

                if (_isHistoryMode)
                {
                    XtraMessageBox.Show("Đang ở chế độ lịch sử, không thể lưu.");
                    return;
                }
                if (!ValidateData(dt))
                {
                    XtraMessageBox.Show("Dữ liệu không hợp lệ. Vui lòng kiểm tra Ngày áp dụng và Ngày kết thúc.");
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

    
                string tenNhom = TenNhom;



                if (string.IsNullOrWhiteSpace(tenNhom) &&
                    _tblVatTu != null &&
                    _tblVatTu.Rows.Count > 0 &&
                    _tblVatTu.Columns.Contains("TenCLVT"))
                {
                    tenNhom = _tblVatTu.Rows[0]["TenCLVT"]?.ToString()?.Trim();
                }

                if (string.IsNullOrWhiteSpace(tenNhom))
                {
                    XtraMessageBox.Show("Chưa có nhóm vật tư để reload");
                    return;
                }


                await LoadVatTuByTenNhom(tenNhom);

           

                if (_isSearchMode)
                {
                    
                    _isSearchMode = false;
                }

      

              
                _isUpdatingFromDateEdit = true;
                dateEdit111.EditValue = null;
                dateEdit22.EditValue = null;

                _isUpdatingFromDateEdit = false;


                _isHistoryMode = false;
                _currentEffectiveRowHandle = -1;
                gridViewGia.RefreshData();
                UpdateHistoryUI();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi reload: " + ex.Message);
            }
        }


        private void UpdateHistoryUI()
        {
            btnChangeHistory.Enabled = !_isHistoryMode;
            btnSave.Enabled = !_isHistoryMode;

            gridViewGia.OptionsBehavior.Editable = true;

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewGia.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.ReadOnly = true;
            }

            var checkCol = gridViewGia.Columns["IsCheck"];
            if (checkCol != null)
            {
                checkCol.OptionsColumn.AllowEdit = true;
                checkCol.OptionsColumn.ReadOnly = false;
                checkCol.ColumnEdit = repoChonNhieuVT;
            }

            if (!_isHistoryMode)
            {
                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewGia.Columns)
                {
                    col.OptionsColumn.AllowEdit = true;
                    col.OptionsColumn.ReadOnly = false;
                }
            }

            gridViewGia.RefreshData();
        }
    
        private async void btnXoaLichSu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewGia.CloseEditor();
                gridViewGia.UpdateCurrentRow();          
                if (!_isHistoryMode)
                {
                    XtraMessageBox.Show("Chỉ được xóa khi đang xem lịch sử giá.");
                    return;
                }
                DataTable dt = gridControlGia.DataSource as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xóa.");
                    return;
                }
                var rowsDelete = dt.AsEnumerable()
                    .Where(r =>
                        r.Table.Columns.Contains("IsCheck") &&
                        r["IsCheck"] != DBNull.Value &&
                        Convert.ToBoolean(r["IsCheck"]) == true &&
                        r["ID"] != DBNull.Value &&
                        Convert.ToInt32(r["ID"]) > 0
                    ).ToList();

                // check bản ghi xác nhận
                foreach (DataRow row in rowsDelete)
                {
                    if (row.Table.Columns.Contains("IsXacNhan") &&
                        row["IsXacNhan"] != DBNull.Value &&
                        Convert.ToBoolean(row["IsXacNhan"]))
                    {
                        XtraMessageBox.Show("Bản ghi đã xác nhận không thể xóa.");
                        return;
                    }
                }

                if (rowsDelete.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng tick chọn bản ghi lịch sử cần xóa.");
                    return;
                }
                if (ShowConfirmVN($"Bạn có chắc muốn xóa {rowsDelete.Count} bản ghi lịch sử?") != DialogResult.Yes)
                    return;
                List<VatTuGiaDto> listDelete = rowsDelete
                    .Select(r => new VatTuGiaDto
                    {
                        ID = Convert.ToInt32(r["ID"])
                    }).ToList();
                string url = $"{URL}ERPVatTuBOM/DeleteVatTuGiaHistory";
                var json = JsonConvert.SerializeObject(listDelete);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();
                if (result.Replace("\"", "") == "True")
                {
                    clsWaitForm.ShowSuccessForm(this, 1500);

             
                    _isHistoryMode = false;
                    UpdateHistoryUI();

                    await ReloadGiaAfterSave();
                }
                else
                {
                    XtraMessageBox.Show(result);
                }
       
                   
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi xóa: " + ex.Message);
            }
        }

     

        private void CheckPerminsion()
        {
            try
            {
                string url = string.Format("{0}/GetPer?userID={1}",
                    URL + ResourceURL.UrlUserModule,
                    GlobleData.UserName);

                List<SystemUserModuleEntity> list =
                    Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;

                SystemUserModuleEntity obj = list
                    .Where(m => m.FormShow == this.Name)
                    .FirstOrDefault();

                if (obj == null) return;

                _allowAdd = obj.AllowAdd;
                _allowEdit = obj.AllowEdit;
                _allowDelete = obj.AllowDelete;

               

                
                if (!_allowDelete)
                {
                    btnXoaLichSu.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi phân quyền: " + ex.Message);
            }
        }



    }
}