using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmChonVTPhieu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public DataTable tblGrid = new DataTable();
        public DataTable _tbl = new DataTable();
        public List<DataRow> lstSelect = new List<DataRow>();
        private int _stt = 1;
        public DataTable ListExistingVT = new DataTable();
        string _mancc = string.Empty, _maphieubg = string.Empty, _matiente = string.Empty;
        DataTable tblgoc;
        DataTable tblMau;
        DataTable tblKho;
        DataTable tblDVVT;
        bool _IsVT;
        public DataTable SelectedRows { get; private set; } 
        public DataTable SelectedDetail { get; private set; }
        HashSet<string> _selectedKeys = new HashSet<string>();
        bool _isRestoring = false;
        public frmChonVTPhieu( string mancc, string maphieubg, string matt, bool IsMuaTheoVT = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _mancc = mancc;
            _maphieubg = maphieubg;
            _IsVT = IsMuaTheoVT;
            _matiente = matt;
        }
        protected override void OnLoad(EventArgs e)
        {

            loadGridVatTu();
        }

        private void loadGridVatTu()
        {
            SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            try
            {
                string url1 = $"{URL}NhaCC/GePMH?action=GETALLPHIEUMH&para1=&para2=&para3=&para4=&para5=";
                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

                string urlVT = string.Empty;
                string jsonVT = string.Empty;
                if (_IsVT == true)
                {
                    //urlVT = $"{URL}NhaCC/GePMH?action=GetVatTuKho&para1={_mancc}&para2={_maphieubg}&para3=&para4=&para5=";
                    urlVT = $"{URL}NhaCC/GePMH?action=GetVatTuKho&para1={_mancc}&para2=&para3=&para4=&para5=";
                    jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                }
                else
                {
                    //urlVT = $"{URL}NhaCC/GePMH?action=GetVatTu&para1={_mancc}&para2={_maphieubg}&para3=&para4=&para5=";
                    urlVT = $"{URL}NhaCC/GePMH?action=GetVatTu&para1={_mancc}&para2=&para3=&para4=&para5=";
                    jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                }

                if (jsonVT != "[]")
                {
                    tblGrid = JsonConvert.DeserializeObject<DataTable>(jsonVT);
                    if (tblGrid == null || tblGrid.Rows.Count == 0)
                    {
                        gridControl1.DataSource = null;
                        return;
                    }

                    if (!tblGrid.Columns.Contains("STT"))
                    {
                        tblGrid.Columns.Add("STT", typeof(int));
                    }
                    gridControl1.DataSource = tblGrid;

                    if (ListExistingVT != null && ListExistingVT.Rows.Count > 0)
                    {
                        for (int i = tblGrid.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow row = tblGrid.Rows[i];

                            string rowMaVT = row["MaVTID"]?.ToString().Trim() ?? "";
                            string rowMauVT = row["MauVTID"]?.ToString().Trim() ?? "";
                            string rowKhoVai = row["KhoVaiID"]?.ToString().Trim() ?? "";
                            bool exists = ListExistingVT.AsEnumerable().Any(x =>
                            {
                                string exMaVT = x["MaVTID"]?.ToString().Trim() ?? "";
                                string exMauVT = x["MauVTID"]?.ToString().Trim() ?? "";
                                string exKhoVai = x["KhoVaiID"]?.ToString().Trim() ?? "";

                                return exMaVT == rowMaVT
                                    && exMauVT == rowMauVT
                                    && exKhoVai == rowKhoVai;
                            });

                            if (exists)
                                tblGrid.Rows.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {

            }
            finally
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }

        }


        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;

            GridView view = gridView1;
            int[] selectedRows = view.GetSelectedRows();

            // Tạo DataTable kết quả
            DataTable dtSelected = new DataTable();
            dtSelected.Columns.Add("MaVTID", typeof(string));
            dtSelected.Columns.Add("MauVTID", typeof(string));
            dtSelected.Columns.Add("KhoVaiID", typeof(string));
            dtSelected.Columns.Add("MaDVVT", typeof(string));
            dtSelected.Columns.Add("ItemCode", typeof(string));
            dtSelected.Columns.Add("NPL", typeof(bool));
            dtSelected.Columns.Add("DonGia", typeof(float));
            dtSelected.Columns.Add("TongSL", typeof(float));
            dtSelected.Columns.Add("TongSLMuaThem", typeof(float));
            dtSelected.Columns.Add("ChiPhiVT", typeof(float));
            dtSelected.Columns.Add("MoTa", typeof(string));
            dtSelected.Columns.Add("MaCLVT", typeof(string));
            dtSelected.Columns.Add("KhoVai", typeof(string));
            dtSelected.Columns.Add("TenDVVT", typeof(string));
            dtSelected.Columns.Add("MauVT", typeof(string));
            dtSelected.Columns.Add("TenCL", typeof(string));
            dtSelected.Columns.Add("Thue", typeof(float));
            dtSelected.Columns.Add("PTThanhToan", typeof(string));
            dtSelected.Columns.Add("ChiPhiVanChuyen", typeof(string));
            dtSelected.Columns.Add("ChiPhiKhac", typeof(string));
            dtSelected.Columns.Add("PTVanChuyen", typeof(string));
            dtSelected.Columns.Add("GhiChu", typeof(string));
            dtSelected.Columns.Add("NgayBDHieuLuc", typeof(DateTime));
            dtSelected.Columns.Add("TienTeID", typeof(string));
            dtSelected.Columns.Add("ThanhTien", typeof(float));
            dtSelected.Columns.Add("SoNgayGHSom", typeof(int));
            dtSelected.Columns.Add("SoNgayGHTre", typeof(int));
            dtSelected.Columns.Add("MaDHGOP", typeof(string));
            dtSelected.Columns.Add("MaMauVT", typeof(string));
            dtSelected.Columns.Add("ChietKhau", typeof(float));
            dtSelected.Columns.Add("ThanhTienVND", typeof(decimal));
            dtSelected.Columns.Add("ChiPhiSauCK", typeof(decimal));
            dtSelected.Columns.Add("MaThue_Gop", typeof(string));
            dtSelected.Columns.Add("MaChietKhau_Gop", typeof(string));
            dtSelected.Columns.Add("DVTTPBG", typeof(string));
            dtSelected.Columns.Add("MaPhieuBG", typeof(string));
            dtSelected.Columns.Add("TenPhieu", typeof(string));
            // Lặp qua từng dòng được chọn
            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle < 0) continue; // Bỏ qua group row

                DataRow row = view.GetDataRow(rowHandle);
                if (row != null)
                {
                    string matiente = row["TienTeID"].ToString();
                    string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                    decimal thue = row["Thue"] == null ? 0 : GetThuePercentFromValue(Convert.ToDecimal(row["Thue"]));
                    decimal chietkhau = row["ChietKhau"] == DBNull.Value ? 0 : GetThuePercentFromValue(Convert.ToDecimal(row["ChietKhau"]));
                    decimal chiphi = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]);
                    decimal chiphichietkhau = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) - (chietkhau * (Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"])));
                    decimal thanhtien = chiphichietkhau + (thue * chiphichietkhau);
                    //decimal thanhTienvnd = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) * (1 + thue - chietkhau);
                    decimal giaqd = tbl.Rows[0]["TyGia"] == null ? 0 : Convert.ToDecimal(tbl.Rows[0]["TyGia"]);

                    DataRow newRow = dtSelected.NewRow();
                    newRow["MaVTID"] = row["MaVTID"];
                    newRow["MauVTID"] = row["MauVTID"];
                    newRow["KhoVaiID"] = row["KhoVaiID"];
                    newRow["MaDVVT"] = row["MaDVVT"];
                    newRow["ItemCode"] = row["ItemCode"];
                    newRow["NPL"] = row["NPL"];
                    newRow["DonGia"] = row["DonGia"];
                    newRow["TongSLMuaThem"] = row["TongSLMuaThem"];
                    newRow["ChiPhiVT"] = chiphi;
                    newRow["MoTa"] = row["MoTa"];
                    newRow["MaCLVT"] = row["MaCLVT"];
                    newRow["KhoVai"] = row["KhoVai"];
                    newRow["TenDVVT"] = row["TenDVVT"];
                    newRow["MauVT"] = row["MauVT"];
                    newRow["TenCL"] = row["TenCL"];
                    newRow["Thue"] = row["Thue"];
                    newRow["PTThanhToan"] = row["PTThanhToan"];
                    newRow["PTVanChuyen"] = row["PTVanChuyen"];
                    newRow["GhiChu"] = row["GhiChu"];
                    newRow["NgayBDHieuLuc"] = row["NgayBDHieuLuc"] == null ? DBNull.Value : row["NgayBDHieuLuc"];
                    newRow["TienTeID"] = row["TienTeID"];
                    newRow["ThanhTien"] = thanhtien;
                    newRow["SoNgayGHSom"] = row["SoNgayGHSom"];
                    newRow["SoNgayGHTre"] = row["SoNgayGHTre"];
                    if (_IsVT == false)
                    {
                        newRow["MaDHGOP"] = row["MaDH_Gop"];
                    }
                    else 
                    {
                        newRow["MaDHGOP"] = DBNull.Value;
                    }
                    newRow["MaMauVT"] = row["MaMauVT"];
                    newRow["ChietKhau"] = row["ChietKhau"];

                    //if (matiente.Trim() =="VND")
                    //{

                    //    newRow["ThanhTienVND"] = thanhtien;
                    //}
                    //else 
                    //{
                    //    newRow["ThanhTienVND"] = thanhtien*giaqd;
                    //}
                    newRow["ThanhTienVND"] = thanhtien * giaqd;
                    newRow["ChiPhiSauCK"] = chiphichietkhau;
                    newRow["MaThue_Gop"] = row["MaThue_Gop"];
                    newRow["MaChietKhau_Gop"] = row["MaChietKhau_Gop"];
                    newRow["DVTTPBG"] = "";
                    newRow["MaPhieuBG"] = row["MaPhieuBG"];
                    newRow["TenPhieu"] = row["TenPhieu"];
                    dtSelected.Rows.Add(newRow);
                    lstSelect.Add(row);
                }
            }

            _tbl = dtSelected;

            // Trả về OK để form cha lấy dữ liệu
            this.DialogResult = DialogResult.OK;
        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "NPL"
                               && info.EditValue != null);

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn1 )
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }
            if (info.Column == gridColumn13)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            int groupIndex = gridView1.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
            }

            e.Handled = false;
        }

        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            
            if (_IsVT == true)
            {
                Search();
            }
        }

        private void Search()
        {
            SplashScreenManager.ShowDefaultWaitForm("Đang lọc dữ liệu", "Xin vui lòng chờ...");
            try
            {
                string keyword = barEditItem1.EditValue?.ToString() ?? "";

                string urlVT = $"{URL}NhaCC/GePMH?action=SearchVT&para1={_mancc}&para2={keyword}&para3=&para4=&para5=";

                string jsonVT = Task.Run(async () => await _clientExtension.GetAsnyc(urlVT)).Result;

                DataTable tblAll = JsonConvert.DeserializeObject<DataTable>(jsonVT);

                gridControl1.DataSource = tblAll;
                RestoreSelection();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Action == CollectionChangeAction.Add)
            {
                DataRow row = view.GetDataRow(e.ControllerRow);
                if (row != null)
                    _selectedKeys.Add(GetRowKey(row));
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                DataRow row = view.GetDataRow(e.ControllerRow);
                if (row != null)
                    _selectedKeys.Remove(GetRowKey(row));
            }
        }

        private decimal GetThuePercentFromValue(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;

            string str = value.ToString().Replace("%", "").Trim();
            if (decimal.TryParse(str, out decimal percent))
                return percent / 100m;

            return 0m;
        }

        private void gridView1_ColumnFilterChanged(object sender, EventArgs e)
        {
            RestoreSelection();
        }

        private string GetRowKey(DataRow row)
        {
            return $"{row["MaVTID"]}|{row["MauVTID"]}|{row["KhoVaiID"]}|{row["MaCLVT"]}|{row["MaDVVT"]}|{row["MaPhieuBG"]}";
        }

        private void RestoreSelection()
        {

            if (_isRestoring) return;
            _isRestoring = true;

            gridView1.BeginSelection();
            gridView1.ClearSelection();

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                var row = gridView1.GetDataRow(i);
                if (row != null && _selectedKeys.Contains(GetRowKey(row)))
                    gridView1.SelectRow(i);
            }

            gridView1.EndSelection();
            _isRestoring = false;
        }

    }
}