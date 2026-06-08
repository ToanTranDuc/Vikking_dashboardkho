using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBOM_ChonVatTuV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string _makh = string.Empty, _mahang = string.Empty,_dot=string.Empty, _masizechung = string.Empty;

        DataTable _tbl = new DataTable();
        DataTable tblMau = new DataTable();

        DataTable tblMauTV = new DataTable();
        public DataTable tblGrid = new DataTable();
        public List<DataRow> lstSelect = new List<DataRow>();
        private int _stt = 1;
        private int isNPLDisplay = 0;
        public frmPhanTichBOM_ChonVatTuV1()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            
        }
        public frmPhanTichBOM_ChonVatTuV1(DataTable tbl)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._tbl = tbl;

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                // Kiểm tra số dòng HIỂN THỊ trong GridView (sau filter)
                if (gV.RowCount == 3)
                {
                    // Nếu chỉ còn 1 dòng hiển thị, lấy dòng đó
                    int dataRowHandle = gV.GetVisibleRowHandle(2);
                    DataRow singleRow = gV.GetDataRow(dataRowHandle);
                    singleRow["STT"] = 1;
                   
                    lstSelect.Clear();
                    lstSelect.Add(singleRow);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return true;
                }
                //else
                //{
                //    // Nếu có nhiều hơn 1 dòng hiển thị, thực hiện logic cũ
                //    barButtonItem7_ItemClick(null, null);
                //    return true;
                //}
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        protected override void OnLoad(EventArgs e)
        {

            loadGridVatTu();
           
            loadMauVTTV();
        }

        private void gV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            if (view == null || info == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);

            GridColumn groupColumn = info.Column;

          
            if (groupColumn == gridColumn11)

            {

                info.GroupText = string.Format("{0}", info.GroupValueText);

            }

            if (groupColumn == gridColumn1)

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

        private void gV_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (e.Column.FieldName == "IsCheck")
            {
                view.UpdateCurrentRow();
            }
        }

        private void gV_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.Column.FieldName == "IsCheck")
            {
                if (Convert.ToBoolean(e.Value))
                {
                    view.SetRowCellValue(e.RowHandle, "STT", _stt);
                    _stt++;
                }
                else
                {
                    view.SetRowCellValue(e.RowHandle, "STT", null);
                    _stt--;
                }
            }
        }

        private void loadGridVatTu()
        {
            
            SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
            SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
            try
            {
                string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETVATTUPOMAU&para1={_makh.ToString()}&para2={_mahang.ToString()}&para3={_dot.ToString()}";
                string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                if (jsonVT != "[]")
                {
                    tblGrid = JsonConvert.DeserializeObject<DataTable>(jsonVT);
                    if (tblGrid == null || tblGrid.Rows.Count == 0)
                    {
                        gC.DataSource = null;
                        return;
                    }
                    if (!tblGrid.Columns.Contains("STT"))
                    {
                        tblGrid.Columns.Add("STT", typeof(int));
                    }
                    if (_tbl != null && _tbl.Rows.Count != 0)
                    {
                        var keysToRemove = new HashSet<string>(
                     _tbl.AsEnumerable()
                         .Select(r => $"{r.Field<string>("MaVTID")}_{r.Field<string>("MaCLVT")}_{r.Field<string>("KhoVaiID")}_{r.Field<string>("MauVTID")}")
                 );

                        var rowsToDelete = tblGrid.AsEnumerable()
                            .Where(r => keysToRemove.Contains($"{r.Field<string>("MaVTID")}_{r.Field<string>("MaNhom")}_{r.Field<string>("KhoVaiID")}_{r.Field<string>("MauVTID")}"))
                            .ToList();
                        foreach (var row in rowsToDelete)
                            tblGrid.Rows.Remove(row);
                    }
                    if (isNPLDisplay != 0)
                    {
                        if (tblGrid.Columns.Contains("NPL") && tblGrid.Columns["NPL"].DataType == typeof(bool))
                        {
                            for (int i = tblGrid.Rows.Count - 1; i >= 0; i--)
                            {
                                var row = tblGrid.Rows[i];
                                bool npl = row.IsNull("NPL") ? false : (bool)row["NPL"];

                                if ((isNPLDisplay == 1 && npl == false) ||
                                    (isNPLDisplay == 2 && npl == true))
                                {
                                    tblGrid.Rows.RemoveAt(i);
                                }
                            }
                        }
                    }

                    gC.DataSource = tblGrid;


                }
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);

            }
           

        }
        private void loadMauVTTV()
        {
            //string url = $"{URL}KhoiTaoBOMV1/Get?action=GETMAUVTTV";
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //if (json != "[]")
            //{
            //    tblMauTV = JsonConvert.DeserializeObject<DataTable>(json);
            //    if (tblMauTV == null || tblMauTV.Rows.Count == 0)
            //    {
                    
            //        return;
            //    }


            

            //}

        }

      
 
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            //var rows = tblGrid.AsEnumerable()
            //      .Where(r => !r.IsNull("IsCheck") && r.Field<bool>("IsCheck"));
            //lstSelect.AddRange(rows);
            if (gV.RowCount == 3)
            {
                int dataRowHandle = gV.GetVisibleRowHandle(2);
                DataRow singleRow = gV.GetDataRow(dataRowHandle);
                singleRow["STT"] = 1;
                lstSelect.Clear();
                lstSelect.Add(singleRow);
            }
            else
            {
                // Logic cũ: lấy các dòng đã check
                var rows = tblGrid.AsEnumerable()
                      .Where(r => !r.IsNull("IsCheck") && r.Field<bool>("IsCheck"));
                lstSelect.Clear();
                lstSelect.AddRange(rows);
            }

            this.DialogResult = DialogResult.OK;
        }

    }
}

