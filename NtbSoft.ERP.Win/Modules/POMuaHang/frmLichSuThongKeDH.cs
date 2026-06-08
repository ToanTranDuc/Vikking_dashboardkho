using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmLichSuThongKeDH : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataTable gridControl1Table;
        private DataTable gridControl2Table;
        private DataTable gridControl3Table;
        private DataTable candoiTable;
        private DataTable muahangTable;
        private DataTable vattuTable;

        public frmLichSuThongKeDH()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            loadComboBoxEdit1();

            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
        }


        // lọc theo
        private void loadComboBoxEdit1()
        {
            comboBoxEdit1.Properties.Items.Add("Vật tư");
            comboBoxEdit1.Properties.Items.Add("Nhà cung cấp");

            // Chọn item mặc định
            comboBoxEdit1.SelectedIndex = 0;
        }
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit combo = sender as ComboBoxEdit;
            int index = combo.SelectedIndex;

            switch (index)
            {
                case 0:
                    
                    getVatTu();
                    calcVatTu();
                    loadGridControl1();

                    break;
                case 1:
                    
                    break;
                default:
                    break;
            }
        }



        // danh sách vật tư
        private void loadGridControl1()
        {

            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView1.ExpandAllGroups();
        }
        private void getVatTu()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getvattusp";
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!vattuTable.Columns.Contains("generatedID"))
                vattuTable.Columns.Add("generatedID");
            if (!vattuTable.Columns.Contains("IsNPLText"))
                vattuTable.Columns.Add("IsNPLText", typeof(string));
            if (!vattuTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                vattuTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in vattuTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";



                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();
            }
        }
        private void calcVatTu()
        {
            if (vattuTable == null || vattuTable.Columns.Count <= 0 || vattuTable.Rows.Count == 0) return;
            DataTable dt = vattuTable.Copy();            
            foreach (DataRow row in dt.Rows)
            {
                
            }
            gridControl1Table = dt.Copy();
        }
        public string padToThree(string input)
        {
            // Nếu input null thì trả về rỗng
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Dùng PadLeft để thêm '0' cho đủ 3 ký tự
            return input.PadLeft(3, '0');
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info != null)
            {
                string originalText = info.GroupText;
                string[] parts = originalText.Split(new string[] { "--" }, StringSplitOptions.None);

                if (parts.Length > 1)
                {
                    info.GroupText = parts[1].Trim();
                }

                e.Painter.DrawObject(info);
                e.Handled = true;
            }
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            gridControl2Table = null;
            gridControl3Table = null;
            gridControl2.DataSource = null;
            gridControl3.DataSource = null;

            getCanDoi();
            calcCanDoi();
            loadGridControl2();

            getMuaHang();
            calcMuaHang();
            loadGridControl3();
        }



        // lịch sử cân dối
        private void loadGridControl2()
        {

            gridControl2.DataSource = gridControl2Table;
            foreach (GridColumn col in gridView2.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView2.ExpandAllGroups();
        }
        private void getCanDoi()
        {
            GridView view = gridView1;
            int focusedHandle = view.FocusedRowHandle;

            DataRow focusedRow;

            if (focusedHandle >= 0)
            {
                focusedRow = view.GetDataRow(focusedHandle);
            }
            else return;

            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "getlichsucandoitheovattu";
            DataRow newRow = request.TypeTable.NewRow();
            newRow["MaVTID"] = focusedRow["MaVTID"];
            newRow["MaNhomVT"] = focusedRow["MaNhom"];
            newRow["MaMauVT"] = focusedRow["MauVTID"];
            newRow["MaKhoVT"] = focusedRow["KhoVaiID"];
            request.TypeTable.Rows.Add(newRow);
            string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            candoiTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!candoiTable.Columns.Contains("generatedID"))
                candoiTable.Columns.Add("generatedID");
            if (!candoiTable.Columns.Contains("IsNPLText"))
                candoiTable.Columns.Add("IsNPLText", typeof(string));
            if (!candoiTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                candoiTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in candoiTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";

                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();
            }
        }
        private void calcCanDoi()
        {            
            if (candoiTable == null || candoiTable.Columns.Count <= 0 || candoiTable.Rows.Count == 0) return;
            DataTable dt = candoiTable.Copy();
            foreach (DataRow row in dt.Rows)
            {
                row["SLCanDoiKho"] = XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"].ToString());
            }
            gridControl2Table = dt.Copy();
        }



        // lịch sử mua hàng
        private void loadGridControl3()
        {

            gridControl3.DataSource = gridControl3Table;
            foreach (GridColumn col in gridView3.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView3.ExpandAllGroups();
        }
        private void getMuaHang()
        {
            GridView view = gridView1;
            int focusedHandle = view.FocusedRowHandle;

            DataRow focusedRow;

            if (focusedHandle >= 0)
            {
                focusedRow = view.GetDataRow(focusedHandle);
            }
            else return;

            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "getlichsumuahangtheovattu";
            DataRow newRow = request.TypeTable.NewRow();
            newRow["MaVTID"] = focusedRow["MaVTID"];
            newRow["MaNhomVT"] = focusedRow["MaNhom"];
            newRow["MaMauVT"] = focusedRow["MauVTID"];
            newRow["MaKhoVT"] = focusedRow["KhoVaiID"];
            request.TypeTable.Rows.Add(newRow);
            string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            muahangTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!muahangTable.Columns.Contains("generatedID"))
                muahangTable.Columns.Add("generatedID");
            if (!muahangTable.Columns.Contains("IsNPLText"))
                muahangTable.Columns.Add("IsNPLText", typeof(string));
            if (!muahangTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                muahangTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in muahangTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";

                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();
            }
        }
        private void calcMuaHang()
        {
            if (muahangTable == null || muahangTable.Columns.Count <= 0 || muahangTable.Rows.Count == 0) return;
            DataTable dt = muahangTable.Copy();
            foreach (DataRow row in dt.Rows)
            {
                row["SoLuongMuaThem"] = XuLyVTUnits.SmartTryParse<decimal>(row["SoLuongMuaThem"].ToString());
                row["SLMuaThem"] = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString());
                row["TyLeMuaThem"] = XuLyVTUnits.SmartTryParse<decimal>(row["TyLeMuaThem"].ToString());
            }
            gridControl3Table = dt.Copy();
        }
    }
}