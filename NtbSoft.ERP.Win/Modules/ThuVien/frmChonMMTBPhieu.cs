using DevExpress.XtraEditors;
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
    public partial class frmChonMMTBPhieu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public DataTable tblGridTB = new DataTable();
        public DataTable tblGridLK = new DataTable();
        public DataTable _tbl = new DataTable();
        public List<DataRow> lstSelect = new List<DataRow>();
        private int _stt = 1;
        public DataTable ListExistingVT = new DataTable();
        string _mancc = string.Empty, _maphieubg = string.Empty, _matiente = string.Empty, _mapyc;
        public DataTable SelectedRows { get; private set; }
        public DataTable SelectedDetail { get; private set; }
        public frmChonMMTBPhieu(string mancc, string maphieubg, string matt,string mapdk)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _mancc = mancc;
            //_maphieubg = maphieubg;
            _matiente = matt;
            _mapyc = mapdk;
        }

        protected override void OnLoad(EventArgs e)
        {

            loadGridMMTB();
        }
        private void loadGridMMTB()
        {
            SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            try
            {
                string url1 = $"{URL}NhaCC/GePMHMMTB?action=GETALLPHIEUMH&para1=&para2=&para3=&para4=&para5=";
                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

                string urlVT = string.Empty;
                string jsonVT = string.Empty;

                //urlVT = $"{URL}NhaCC/GePMHMMTB?action=GetMMTB&para1={_mancc}&para2={_maphieubg}&para3=&para4=&para5=";
                urlVT = $"{URL}NhaCC/GePMHMMTB?action=GetMMTB&para1={_mancc}&para2={_mapyc}&para3=&para4=&para5=";
                jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;

                if (jsonVT != "[]")
                {

                    DataTable tblAll = JsonConvert.DeserializeObject<DataTable>(jsonVT);
                    if (tblAll == null || tblAll.Rows.Count == 0)
                    {
                        gridControl1.DataSource = null;
                        gridControl2.DataSource = null;
                        return;
                    }
                    DataView dvTB = new DataView(tblAll);
                    dvTB.RowFilter = "Status = 1";
                    tblGridTB = dvTB.ToTable();

                    DataView dvLK = new DataView(tblAll);
                    dvLK.RowFilter = "Status = 0";
                    tblGridLK = dvLK.ToTable();

                    AddSTT(tblGridTB);
                    AddSTT(tblGridLK);

                    gridControl1.DataSource = tblGridTB;
                    gridControl2.DataSource = tblGridLK;

                    if (ListExistingVT != null && ListExistingVT.Rows.Count > 0)
                    {
                        for (int i = tblGridTB.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow row = tblGridTB.Rows[i];

                            string rowMaTB = row["MaHang"]?.ToString().Trim() ?? "";
                            string rowMaCL = row["MaCL"]?.ToString().Trim() ?? "";
                            string rowMaNhom = row["MaNhom"]?.ToString().Trim() ?? "";

                            bool exists = ListExistingVT.AsEnumerable().Any(x =>
                            {
                                return (x["MaTB"]?.ToString().Trim() ?? "") == rowMaTB
                                    && (x["MaCL"]?.ToString().Trim() ?? "") == rowMaCL
                                    && (x["MaNhom"]?.ToString().Trim() ?? "") == rowMaNhom
                                    && (x["IsTB"]?.ToString() ?? "") == "True";
                            });

                            if (exists)
                                tblGridTB.Rows.RemoveAt(i);
                        }

                        for (int i = tblGridLK.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow row = tblGridLK.Rows[i];

                            string rowMaLK = row["MaHang"]?.ToString().Trim() ?? "";
                            string rowMaCL = row["MaCL"]?.ToString().Trim() ?? "";
                            bool exists = ListExistingVT.AsEnumerable().Any(x =>
                            {
                                return (x["MaLK"]?.ToString().Trim() ?? "") == rowMaLK
                                    && (x["MaCLLK"]?.ToString() ?? "") == rowMaCL
                                    && (x["IsTB"]?.ToString() ?? "") == "False";
                            });

                            if (exists)
                                tblGridLK.Rows.RemoveAt(i);
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

        private void AddSTT(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;

            if (!dt.Columns.Contains("STT"))
                dt.Columns.Add("STT", typeof(int));

            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["STT"] = i + 1;
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

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            if (info.Column == gridColumn1)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn9)
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

        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            if (info.Column == gridColumn15)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn17)
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            lstSelect = new List<DataRow>();
            GridView view = gridView1;
            int[] selectedRows = view.GetSelectedRows();

            // Tạo DataTable kết quả
            DataTable dtSelected = new DataTable();

            dtSelected.Columns.Add("Status", typeof(int)); // 1 = TB, 0 = LK

            dtSelected.Columns.Add("MaHang", typeof(string));
            dtSelected.Columns.Add("MaCL", typeof(string));
            dtSelected.Columns.Add("MaNhom", typeof(string));


            //dtSelected.Columns.Add("MaKho", typeof(string));

            dtSelected.Columns.Add("TenHang", typeof(string));   // CHUNG LK TB
            dtSelected.Columns.Add("TenCL", typeof(string));
            dtSelected.Columns.Add("TenNhom", typeof(string));
            dtSelected.Columns.Add("MauMa", typeof(string));
            dtSelected.Columns.Add("XuatXu", typeof(string));
            dtSelected.Columns.Add("HangSX", typeof(string));
            dtSelected.Columns.Add("DonVi", typeof(string));

            //dtSelected.Columns.Add("MaNhaCungCap", typeof(string));
            dtSelected.Columns.Add("MaPhieuBG", typeof(string));
            dtSelected.Columns.Add("TenPhieu", typeof(string));
            dtSelected.Columns.Add("DonGia", typeof(float));
            dtSelected.Columns.Add("TongSLMuaThem", typeof(float));

            dtSelected.Columns.Add("PTThanhToan", typeof(string));
            dtSelected.Columns.Add("PTVanChuyen", typeof(string));
            //dtSelected.Columns.Add("GhiChu", typeof(string));
            dtSelected.Columns.Add("NgayBDHieuLuc", typeof(DateTime));
            dtSelected.Columns.Add("SoNgayHieuLuc", typeof(int));
            dtSelected.Columns.Add("SoNgayGHSom", typeof(int));
            dtSelected.Columns.Add("SoNgayGHTre", typeof(int));

            dtSelected.Columns.Add("Thue", typeof(float));
            dtSelected.Columns.Add("ChietKhau", typeof(float));
            dtSelected.Columns.Add("TienTeID", typeof(string));

            dtSelected.Columns.Add("ChiPhiVT", typeof(decimal));
            dtSelected.Columns.Add("ChiPhiSauCK", typeof(decimal));
            dtSelected.Columns.Add("ThanhTien", typeof(decimal));
            dtSelected.Columns.Add("ThanhTienVND", typeof(decimal));

            CollectSelectedFromGrid(gridView1, dtSelected); // thiết bị
            CollectSelectedFromGrid(gridView2, dtSelected); // linh kiện
                                                            // Lặp qua từng dòng được chọn
                                                            //foreach (int rowHandle in selectedRows)
                                                            //{
                                                            //    if (rowHandle < 0) continue; // Bỏ qua group row

            //    DataRow row = view.GetDataRow(rowHandle);
            //    if (row != null)
            //    {
            //        string matiente = row["TienTeID"].ToString();
            //        string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
            //        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            //        decimal thue = row["Thue"] == null ? 0 : GetThuePercentFromValue(Convert.ToDecimal(row["Thue"]));
            //        decimal chietkhau = row["ChietKhau"] == DBNull.Value ? 0 : GetThuePercentFromValue(Convert.ToDecimal(row["ChietKhau"]));
            //        decimal chiphi = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]);
            //        decimal chiphichietkhau = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) - (chietkhau * (Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"])));
            //        decimal thanhtien = chiphichietkhau + (thue * chiphichietkhau);
            //        //decimal thanhTienvnd = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) * (1 + thue - chietkhau);
            //        decimal giaqd = tbl.Rows[0]["TyGia"] == null ? 0 : Convert.ToDecimal(tbl.Rows[0]["TyGia"]);
            //        int status = Convert.ToInt32(row["Status"]);
            //        DataRow newRow = dtSelected.NewRow();
            //        newRow["Status"] = status;

            //        newRow["MaNhom"] = row["MaNhom"];
            //        newRow["TenNhom"] = row["TenNhom"];
            //        //newRow["MaKho"] = row["MaKho"];
            //        //newRow["MaNhaCungCap"] = row["MaNhaCungCap"];

            //        newRow["DonGia"] = row["DonGia"];
            //        newRow["TongSLMuaThem"] = row["TongSLMuaThem"];
            //        newRow["PTThanhToan"] = row["PTThanhToan"];
            //        newRow["PTVanChuyen"] = row["PTVanChuyen"];
            //        //newRow["GhiChu"] = row["GhiChu"];
            //        newRow["NgayBDHieuLuc"] = row["NgayBDHieuLuc"] == null ? DBNull.Value : row["NgayBDHieuLuc"];
            //        newRow["SoNgayHieuLuc"] = row["SoNgayHieuLuc"];
            //        newRow["SoNgayGHSom"] = row["SoNgayGHSom"];
            //        newRow["SoNgayGHTre"] = row["SoNgayGHTre"];
            //        newRow["Thue"] = row["Thue"];
            //        newRow["ChietKhau"] = row["ChietKhau"];
            //        newRow["TienTeID"] = row["TienTeID"];

            //        if (status == 1)
            //        {
            //            // ==== THIẾT BỊ ====
            //            newRow["MaTB"] = row["MaTB"];
            //            newRow["TenTB"] = row["TenTB"];
            //            newRow["MauMa"] = row["MauMa"];
            //            newRow["XuatXu"] = row["XuatXu"];
            //            newRow["HangSX"] = row["HangSX"];
            //            newRow["MaCL"] = row["MaCL"];
            //            newRow["TenCL"] = row["TenCL"];
            //            newRow["MaLK"] = DBNull.Value;
            //            newRow["DonVi"] = DBNull.Value;
            //        }
            //        else
            //        {
            //            // ==== LINH KIỆN ====
            //            newRow["MaTB"] = DBNull.Value;
            //            newRow["MauMa"] = DBNull.Value;
            //            newRow["XuatXu"] = DBNull.Value;
            //            newRow["HangSX"] = DBNull.Value;
            //            newRow["MaTB_KT"] = DBNull.Value;
            //            newRow["MaCL"] = row["MaCLLK"];
            //            newRow["TenCL"] = row["TenCLLK"];
            //            newRow["MaLK"] = row["MaLK"];
            //            newRow["DonVi"] = row["DonVi"];

            //            // ⭐ QUAN TRỌNG: TenTB = TenLK
            //            newRow["TenTB"] = row["TenLK"];
            //        }

            //        newRow["ChiPhiVT"] = chiphi;
            //        newRow["ChiPhiSauCK"] = chiphichietkhau;
            //        newRow["ThanhTien"] = thanhtien;
            //        newRow["ThanhTienVND"] = thanhtien * giaqd;
            //        //if (matiente.Trim() =="VND")
            //        //{

            //        //    newRow["ThanhTienVND"] = thanhtien;
            //        //}
            //        //else 
            //        //{
            //        //    newRow["ThanhTienVND"] = thanhtien*giaqd;
            //        //}

            //        dtSelected.Rows.Add(newRow);
            //        lstSelect.Add(row);
            //    }
            //}
            if (dtSelected.Rows.Count == 0)
            {
                XtraMessageBox.Show(
                    "Vui lòng chọn ít nhất 1 thiết bị hoặc linh kiện.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            _tbl = dtSelected;

            // Trả về OK để form cha lấy dữ liệu
            this.DialogResult = DialogResult.OK;
        }

        private void repositoryItemLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void barEditItem2_EditValueChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            try
            {
               string keyword = barEditItem2.EditValue?.ToString() ?? "";

                string urlVT = $"{URL}NhaCC/GePMHMMTB?action=SearchMMTB&para1={_mancc}&para2={keyword}&para3=&para4=&para5=";

                string jsonVT = Task.Run(async () => await _clientExtension.GetAsnyc(urlVT)).Result;

                DataTable tblAll = JsonConvert.DeserializeObject<DataTable>(jsonVT);

                DataView dvTB = new DataView(tblAll);
                dvTB.RowFilter = "Status = 1";

                DataView dvLK = new DataView(tblAll);
                dvLK.RowFilter = "Status = 0";


                gridControl1.DataSource = dvTB.ToTable();
                gridControl2.DataSource = dvLK.ToTable();
            }
            catch (Exception ex)
            {

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


        private void CollectSelectedFromGrid(GridView view, DataTable dtSelected)
        {
            if (view == null) return;

            int[] selectedRows = view.GetSelectedRows();

            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle < 0) continue;

                DataRow row = view.GetDataRow(rowHandle);
                if (row == null) continue;
                string matiente = row["TienTeID"].ToString();
                string url = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                decimal thue = row["Thue"] == null || row["Thue"].ToString() == "" ? 0 : GetThuePercentFromValue(Convert.ToDecimal(row["Thue"]));
                decimal chietkhau = row["ChietKhau"] == DBNull.Value ? 0 : GetThuePercentFromValue(Convert.ToDecimal(row["ChietKhau"]));
                decimal chiphi = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]);
                decimal chiphichietkhau = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) - (chietkhau * (Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"])));
                decimal thanhtien = chiphichietkhau + (thue * chiphichietkhau);
                //decimal thanhTienvnd = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) * (1 + thue - chietkhau);
                decimal giaqd = tbl.Rows[0]["TyGia"] == null ? 0 : Convert.ToDecimal(tbl.Rows[0]["TyGia"]);
                int status = Convert.ToInt32(row["Status"]);

                DataRow newRow = dtSelected.NewRow();
                newRow["Status"] = status;

                newRow["MaNhom"] = row["MaNhom"];
                newRow["TenNhom"] = row["TenNhom"];
                //newRow["MaKho"] = row["MaKho"];
                //newRow["MaNhaCungCap"] = row["MaNhaCungCap"];

                newRow["DonGia"] = row["DonGia"];
                newRow["TongSLMuaThem"] = row["TongSLMuaThem"];
                newRow["PTThanhToan"] = row["PTThanhToan"];
                newRow["PTVanChuyen"] = row["PTVanChuyen"];
                //newRow["GhiChu"] = row["GhiChu"];
                newRow["NgayBDHieuLuc"] = row["NgayBDHieuLuc"] == null ? DBNull.Value : row["NgayBDHieuLuc"];
                newRow["SoNgayHieuLuc"] = row["SoNgayHieuLuc"];
                newRow["SoNgayGHSom"] = row["SoNgayGHSom"];
                newRow["SoNgayGHTre"] = row["SoNgayGHTre"];
                newRow["Thue"] = row["Thue"];
                newRow["ChietKhau"] = row["ChietKhau"];
                newRow["TienTeID"] = row["TienTeID"];

                newRow["MaPhieuBG"] = row["MaPhieuBG"];
                newRow["TenPhieu"] = row["TenPhieu"];

                if (status == 1)
                {
                    // ==== THIẾT BỊ ====
                    newRow["MaHang"] = row["MaTB"];
                    newRow["TenHang"] = row["TenTB"];
                    newRow["MauMa"] = row["MauMa"];
                    newRow["XuatXu"] = row["XuatXu"];
                    newRow["HangSX"] = row["HangSX"];
                    newRow["MaCL"] = row["MaCL"];
                    newRow["TenCL"] = row["TenCL"];
                    newRow["DonVi"] = DBNull.Value;
                }
                else
                {
                    // ==== LINH KIỆN ====
                    newRow["MaHang"] = row["MaLK"];
                    newRow["TenHang"] = row["TenLK"];
                    newRow["MauMa"] = DBNull.Value;
                    newRow["XuatXu"] = DBNull.Value;
                    newRow["HangSX"] = DBNull.Value;
                    newRow["MaCL"] = row["MaCLLK"];
                    newRow["TenCL"] = row["TenCLLK"];
                    newRow["DonVi"] = row["DonVi"];

                    // ⭐ QUAN TRỌNG: TenTB = TenLK

                }

                newRow["ChiPhiVT"] = chiphi;
                newRow["ChiPhiSauCK"] = chiphichietkhau;
                newRow["ThanhTien"] = thanhtien;
                newRow["ThanhTienVND"] = thanhtien * giaqd;
                //if (matiente.Trim() =="VND")
                //{

                //    newRow["ThanhTienVND"] = thanhtien;
                //}
                //else 
                //{
                //    newRow["ThanhTienVND"] = thanhtien*giaqd;
                //}

                dtSelected.Rows.Add(newRow);
                lstSelect.Add(row);
            }
        }

    }
}