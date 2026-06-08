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
using NtbSoft.ERP.Win.Utils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Entity.SYSTEM;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Globalization;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using NtbSoft.ERP.Win.Modules.POMuaHang;
using NtbSoft.ERP.Win.Modules.Kho;
using DevExpress.XtraBars;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERPNhaCungCapNK : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblThuVien;
        private string _currentSearchText = "";
        private string _searchTargetMaCLMMTB = null;
        private string _searchTargetMaCLLK = null;
        private DataTable _dtSearchResult;
        List<string> listLoaiGoc = new List<string>();
        List<string> listCLoaiGoc = new List<string>();
        List<string> listMayMocThietBi = new List<string>();
        List<string> listLinhKienMMTB = new List<string>();
        string _mancc = string.Empty, _maloai = string.Empty, _tencl = string.Empty, _maclvt = string.Empty, _barDisplayText = string.Empty;
        Dictionary<string, string> _dicQG = new Dictionary<string, string>();
        private DataTable _dtPaidBy;
        private SearchLookUpEdit _barSearchEditor;
        private DataTable _tblNhomNCC;
        private Dictionary<string, string> _dicNhom = new Dictionary<string, string>();
        public frmERPNhaCungCapNK()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            CreateDropdownMenu();
            this.gridView3.ColumnFilterChanged += (s, e) =>
            {
                if (gridView3.DataSource == null) return;
                _isLock = true;
                gridView3.BeginSelection();

                for (int i = 0; i < gridView3.DataRowCount; i++)
                {
                    string maCL = gridView3.GetRowCellValue(i, "MaCL")?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(maCL) && listLinhKienMMTB.Contains(maCL))
                    {
                        gridView3.SelectRow(i);
                    }
                }

                gridView3.EndSelection();
                _isLock = false;
            };
        }
        protected override void OnLoad(EventArgs e)
        {
            tblThuVien = new DataTable();
            CreateTableThuVien();
            loadData();
            this.ActiveControl = simpleButton1;
        }
        private void CreateTableThuVien()
        {
            tblThuVien = new DataTable("tblThuVien");
            tblThuVien.Columns.Add("ID", typeof(int));
            tblThuVien.Columns.Add("MaKH", typeof(string));
            tblThuVien.Columns.Add("TenKH", typeof(string));
            tblThuVien.Columns.Add("DiaChi", typeof(string));
            tblThuVien.Columns.Add("SoDienThoai", typeof(string));
            tblThuVien.Columns.Add("NguoiDaiDien", typeof(string));
            tblThuVien.Columns.Add("MaSoThue", typeof(string));
            tblThuVien.Columns.Add("LoaiDT", typeof(bool));
            tblThuVien.Columns.Add("MaLoaiNCC", typeof(string));
            tblThuVien.Columns.Add("MaNhaCC", typeof(string));
            tblThuVien.Columns.Add("MaChungLoai", typeof(string));
            tblThuVien.Columns.Add("TienTE", typeof(string));
            tblThuVien.Columns.Add("Office", typeof(string));
            tblThuVien.Columns.Add("Factory", typeof(string));
            tblThuVien.Columns.Add("Sales", typeof(string));
            tblThuVien.Columns.Add("Mail", typeof(string));
            tblThuVien.Columns.Add("Website", typeof(string));
            tblThuVien.Columns.Add("Group", typeof(string));
            tblThuVien.Columns.Add("Deliveryterms", typeof(string));
            tblThuVien.Columns.Add("TermsOfPayment", typeof(string));
            tblThuVien.Columns.Add("Leadtime", typeof(string));
            tblThuVien.Columns.Add("PaidBy", typeof(string));
            tblThuVien.Columns.Add("IsMMTB", typeof(int));
            tblThuVien.Columns.Add("NhomHangHoa", typeof(string));
        }

        private DataTable CreateTypeTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaCTNCC", typeof(string));
            dt.Columns.Add("MaNhaCC", typeof(string));
            dt.Columns.Add("MaLoaiNCC", typeof(string));
            dt.Columns.Add("MaChungLoai", typeof(string));
            return dt;
        }
        private void CreateDropdownMenu()
        {
            PopupMenu popupMenu = new PopupMenu(barManager1);
            BarButtonItem btnThemVatTu = new BarButtonItem(barManager1, "Thêm nhà cung cấp vật tư");
            btnThemVatTu.ItemClick += BtnThemVatTu_ItemClick;
            BarButtonItem btnThemMMTB = new BarButtonItem(barManager1, "Thêm nhà cung cấp máy móc thiết bị");
            btnThemMMTB.ItemClick += BtnThemMMTB_ItemClick;
            popupMenu.AddItems(new BarItem[] { btnThemVatTu, btnThemMMTB });
            btnThem.DropDownControl = popupMenu;
            btnThem.ActAsDropDown = true;
        }
        private void loadData()
        {
            try
            {
                CreateTableThuVien();

                // Load dữ liệu NCC thông thường (VT)
                string urlVT = $"{URL}NhaCC/Get?action=GETNCC&para=&para1=&para2=&para3=&para4=&para5=";
                string jsonVT = Task.Run(async () => await _clientExtension.GetAsnyc(urlVT)).Result;
                if (!string.IsNullOrEmpty(jsonVT) && jsonVT != "[]")
                {
                    DataTable dtVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
                    foreach (DataRow rowVT in dtVT.Rows)
                    {
                        DataRow newRow = tblThuVien.NewRow();
                        newRow["ID"] = rowVT.Table.Columns.Contains("ID") ? rowVT["ID"] : DBNull.Value;
                        newRow["MaKH"] = rowVT["MaKH"];
                        newRow["TenKH"] = rowVT["TenKH"];
                        newRow["DiaChi"] = rowVT["DiaChi"];
                        newRow["SoDienThoai"] = rowVT["SoDienThoai"];
                        newRow["Mail"] = rowVT["Mail"];
                        bool loaiDT = rowVT.Table.Columns.Contains("LoaiDT") && rowVT["LoaiDT"] != DBNull.Value
                            ? Convert.ToBoolean(rowVT["LoaiDT"])
                            : false;
                        newRow["LoaiDT"] = loaiDT;

                        newRow["MaNhaCC"] = rowVT["MaNhaCC"];
                        newRow["IsMMTB"] = 0;

                        if (rowVT.Table.Columns.Contains("Group"))
                            newRow["Group"] = rowVT["Group"];
                        if (rowVT.Table.Columns.Contains("NguoiDaiDien"))
                            newRow["NguoiDaiDien"] = rowVT["NguoiDaiDien"];
                        if (rowVT.Table.Columns.Contains("MaSoThue"))
                            newRow["MaSoThue"] = rowVT["MaSoThue"];
                        if (rowVT.Table.Columns.Contains("TienTE"))
                            newRow["TienTE"] = rowVT["TienTE"];
                        if (rowVT.Table.Columns.Contains("Office"))
                            newRow["Office"] = rowVT["Office"];
                        if (rowVT.Table.Columns.Contains("Factory"))
                            newRow["Factory"] = rowVT["Factory"];
                        if (rowVT.Table.Columns.Contains("Sales"))
                            newRow["Sales"] = rowVT["Sales"];
                        if (rowVT.Table.Columns.Contains("Website"))
                            newRow["Website"] = rowVT["Website"];
                        if (rowVT.Table.Columns.Contains("Deliveryterms"))
                            newRow["Deliveryterms"] = rowVT["Deliveryterms"];
                        if (rowVT.Table.Columns.Contains("TermsOfPayment"))
                            newRow["TermsOfPayment"] = rowVT["TermsOfPayment"];
                        if (rowVT.Table.Columns.Contains("Leadtime"))
                            newRow["Leadtime"] = rowVT["Leadtime"];
                        if (rowVT.Table.Columns.Contains("PaidBy"))
                            newRow["PaidBy"] = rowVT["PaidBy"];

                        tblThuVien.Rows.Add(newRow);
                    }
                }

                string urlMMTB = $"{URL}NhaCC/Get?action=GETNCCMMTB&para=&para1=&para2=&para3=&para4=&para5=";
                string jsonMMTB = Task.Run(async () => await _clientExtension.GetAsnyc(urlMMTB)).Result;
                if (!string.IsNullOrEmpty(jsonMMTB) && jsonMMTB != "[]")
                {
                    DataTable dtMMTB = JsonConvert.DeserializeObject<DataTable>(jsonMMTB);
                    foreach (DataRow rowMMTB in dtMMTB.Rows)
                    {
                        DataRow newRow = tblThuVien.NewRow();
                        newRow["ID"] = rowMMTB.Table.Columns.Contains("ID") ? rowMMTB["ID"] : DBNull.Value;
                        newRow["MaKH"] = rowMMTB["MaKH"];
                        newRow["TenKH"] = rowMMTB["TenKH"];
                        newRow["DiaChi"] = rowMMTB["DiaChi"];
                        newRow["SoDienThoai"] = rowMMTB["SoDienThoai"];
                        newRow["Mail"] = rowMMTB["Mail"];
                        newRow["LoaiDT"] = true;
                        newRow["MaNhaCC"] = rowMMTB["MaKH"];
                        newRow["IsMMTB"] = 1;

                        if (rowMMTB.Table.Columns.Contains("NhomHangHoa") && rowMMTB["NhomHangHoa"] != DBNull.Value)
                        {
                            newRow["NhomHangHoa"] = rowMMTB["NhomHangHoa"].ToString();
                        }
                        else
                        {
                            newRow["NhomHangHoa"] = "";
                        }

                        if (rowMMTB.Table.Columns.Contains("NguoiDaiDien"))
                            newRow["NguoiDaiDien"] = rowMMTB["NguoiDaiDien"];
                        if (rowMMTB.Table.Columns.Contains("MaSoThue"))
                            newRow["MaSoThue"] = rowMMTB["MaSoThue"];
                        if (rowMMTB.Table.Columns.Contains("TienTE"))
                            newRow["TienTE"] = rowMMTB["TienTE"];
                        if (rowMMTB.Table.Columns.Contains("Office"))
                            newRow["Office"] = rowMMTB["Office"];
                        if (rowMMTB.Table.Columns.Contains("Factory"))
                            newRow["Factory"] = rowMMTB["Factory"];
                        if (rowMMTB.Table.Columns.Contains("Sales"))
                            newRow["Sales"] = rowMMTB["Sales"];
                        if (rowMMTB.Table.Columns.Contains("Website"))
                            newRow["Website"] = rowMMTB["Website"];
                        if (rowMMTB.Table.Columns.Contains("Deliveryterms"))
                            newRow["Deliveryterms"] = rowMMTB["Deliveryterms"];
                        if (rowMMTB.Table.Columns.Contains("TermsOfPayment"))
                            newRow["TermsOfPayment"] = rowMMTB["TermsOfPayment"];
                        if (rowMMTB.Table.Columns.Contains("Leadtime"))
                            newRow["Leadtime"] = rowMMTB["Leadtime"];
                        if (rowMMTB.Table.Columns.Contains("PaidBy"))
                            newRow["PaidBy"] = rowMMTB["PaidBy"];

                        tblThuVien.Rows.Add(newRow);
                    }
                }
                gCThuVien.DataSource = tblThuVien;
                CreateSearchlookupTienTe();
                CreateSearchlookupQG();
                CreateSearchlookupPaidBy();
                CreateSearchlookupNhomNCC(); 
                CreateSearchlookupNCC();
                gVThuVien.BeginUpdate();
                gVThuVien.ClearGrouping();
                if (gVThuVien.Columns["IsMMTB"] != null)
                {
                    gVThuVien.Columns["IsMMTB"].GroupIndex = 0;
                }
                gVThuVien.OptionsView.ShowGroupPanel = false;
                gVThuVien.ExpandAllGroups();
                gVThuVien.EndUpdate();

                this.ActiveControl = simpleButton1;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi load data: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnThemMMTB_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataRow dr = tblThuVien.NewRow();
                string prefix = "NCC";
                int nextNumber = 1;

                if (tblThuVien.Rows.Count > 0)
                {
                    var maxVal = tblThuVien.AsEnumerable()
                        .Where(row => row["MaKH"] != DBNull.Value && row["MaKH"].ToString().StartsWith(prefix))
                        .Select(row => int.Parse(row["MaKH"].ToString().Substring(prefix.Length)))
                        .DefaultIfEmpty(0)
                        .Max();

                    nextNumber = maxVal + 1;
                }
                string newMa = prefix + nextNumber.ToString("D4");
                dr["MaKH"] = newMa;
                dr["MaNhaCC"] = dr["MaKH"];
                dr["ID"] = 0;
                dr["LoaiDT"] = true;
                dr["IsMMTB"] = 1;
                tblThuVien.Rows.InsertAt(dr, 0);
                gCThuVien.DataSource = tblThuVien;
                //gVThuVien.RefreshData();
                gVThuVien.FocusedRowHandle = gVThuVien.LocateByValue("MaKH", newMa);
                gVThuVien.ShowEditor();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnThemVatTu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataRow dr = tblThuVien.NewRow();
                string prefix = "NCC";
                int nextNumber = 1;

                if (tblThuVien.Rows.Count > 0)
                {
                    var maxVal = tblThuVien.AsEnumerable()
                        .Where(row => row["MaNhaCC"] != DBNull.Value && row["MaNhaCC"].ToString().StartsWith(prefix))
                        .Select(row => int.Parse(row["MaNhaCC"].ToString().Substring(prefix.Length)))
                        .DefaultIfEmpty(0)
                        .Max();

                    nextNumber = maxVal + 1;
                }
                string newMa = prefix + nextNumber.ToString("D4");
                dr["MaNhaCC"] = newMa;
                dr["ID"] = 0;
                dr["LoaiDT"] = true;
                dr["IsMMTB"] = 0;
                tblThuVien.Rows.InsertAt(dr, 0);
                gCThuVien.DataSource = tblThuVien;
                gVThuVien.FocusedRowHandle = gVThuVien.LocateByValue("MaNhaCC", newMa);
                gVThuVien.ShowEditor();
            }

            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public bool KiemTraVaCanhBao(DataTable dt)
        {
            bool canhBao = false;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];

                string tenKH = row["TenKH"]?.ToString().Trim();
                string soDienThoai = row["SoDienThoai"]?.ToString().Trim();
                string diaChi = row["DiaChi"]?.ToString().Trim();
                string nguoiDaiDien = row["NguoiDaiDien"]?.ToString().Trim();
                string maSoThue = row["MaSoThue"]?.ToString().Trim();

                bool coTenKH = !string.IsNullOrEmpty(tenKH);
                bool coDuLieuKhac = !string.IsNullOrEmpty(soDienThoai) || !string.IsNullOrEmpty(diaChi) ||
                                    !string.IsNullOrEmpty(nguoiDaiDien) || !string.IsNullOrEmpty(maSoThue);

                if (string.IsNullOrEmpty(tenKH) && !coDuLieuKhac)
                {

                    dt.Rows.RemoveAt(i);
                }
                else if (string.IsNullOrEmpty(tenKH) && coDuLieuKhac)
                {
                    canhBao = true;
                }
            }

            return canhBao;
        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                var duplicateRows = GetDuplicateRows(tblThuVien);
                if (duplicateRows.Any())
                {

                    string message = "Các dòng sau bị trùng:\n";
                    foreach (var row in duplicateRows)
                    {
                        int rowIndex = tblThuVien.Rows.IndexOf(row) + 1;
                        message += $"- Dòng {rowIndex}: {row["TenKH"]}, {row["SoDienThoai"]}\n";
                    }
                    XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                bool coCanhBao = KiemTraVaCanhBao(tblThuVien);
                if (coCanhBao)
                {
                    Console.WriteLine("Có dòng bị thiếu thông tin Nhà cung cấp");
                    return;
                }
                clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_KhachHangNK");

                DataTable tblVT = tblThuVien.Clone();
                DataTable tblMMTB = tblThuVien.Clone();

                foreach (DataRow row in tblThuVien.Rows)
                {
                    int isMMTB = Convert.ToInt32(row["IsMMTB"]);
                    if (isMMTB == 1)
                        tblMMTB.ImportRow(row);
                    else
                        tblVT.ImportRow(row);
                }
                if (tblVT.Columns.Contains("IsMMTB"))
                    tblVT.Columns.Remove("IsMMTB");
                if (tblVT.Columns.Contains("NhomHangHoa"))
                    tblVT.Columns.Remove("NhomHangHoa");
                foreach (DataRow row in tblMMTB.Rows)
                {
                    if (row["NhomHangHoa"] != DBNull.Value
                        && !string.IsNullOrEmpty(row["NhomHangHoa"].ToString().Trim()))
                    {
                        row["MaLoaiNCC"] = row["NhomHangHoa"].ToString().Trim();
                    }
                    if (row["MaNhaCC"] != DBNull.Value
                        && !string.IsNullOrEmpty(row["MaNhaCC"].ToString().Trim()))
                    {
                        row["MaNhaCC"] = row["MaKH"].ToString().Trim();
                    }
                }

                if (tblMMTB.Columns.Contains("IsMMTB"))
                    tblMMTB.Columns.Remove("IsMMTB");
                if (tblMMTB.Columns.Contains("NhomHangHoa"))
                    tblMMTB.Columns.Remove("NhomHangHoa");
                bool success = true;
                bool success2 = true;
                if (tblVT.Rows.Count > 0)
                {
                    string urlVT = $"{URL}NhaCC/PostKH?Action=POSTKH";
                    string resultVT = Task.Run(async () => { return await _clientExtension.PostAsync(urlVT, tblVT); }).Result;
                    success = success && (resultVT.ToLower() == "true");
                }
                if (tblMMTB.Rows.Count > 0)
                {
                    string urlMMTB = $"{URL}NhaCC/InsertOrUpdate";
                    string resultMMTB = Task.Run(async () => { return await _clientExtension.PostAsync(urlMMTB, tblMMTB); }).Result;
                    success2 = success2 && (resultMMTB.ToLower() == "true");
                }

                if (success || success2)
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    loadData();
                }

                if (success)
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    loadData();
                }
                else
                {
                    XtraMessageBox.Show("Có lỗi xảy ra khi lưu dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex){}
        }
        private List<DataRow> GetDuplicateRows(DataTable dt)
        {
            List<DataRow> duplicateRows = new List<DataRow>();

            if (dt == null || dt.Rows.Count == 0)
                return duplicateRows;

            var groups = dt.AsEnumerable()
              .GroupBy(row => new
              {
                  TenKH = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("TenKH"))),
                  SoDienThoai = row.Field<string>("SoDienThoai")
              })
              .Where(g => g.Count() > 1);

            foreach (var group in groups)
            {
                duplicateRows.AddRange(group.Skip(1));
            }
            return duplicateRows;
        }
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gVThuVien.GetFocusedDataRow();
                if (dr == null) return;
                string makh = dr["MaKH"].ToString();
                string maNCC = dr["MaNhaCC"].ToString();
                int isMMTB = Convert.ToInt32(dr["IsMMTB"]);
                int id = 0;
                int.TryParse(dr["ID"]?.ToString(), out id);

                if (isMMTB == 0)
                {
                    string urlCheck = $"{URL}ERPThuVienNK/Get?Action=GETCHECKNCC&para={makh}";
                    string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                    if (jsonCheck != "[]")
                    {
                        XtraMessageBox.Show("Nhà cung cấp này đã được sử dụng. Không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlcheckpmh = $"{URL}NhaCC/GetCP?action=GETCNCCPMH&para1={maNCC}&para2=&para3=&para4=&para5=";
                    string jsoncheckpmh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpmh); }).Result;
                    DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheckpmh);

                    if (tblcheck != null && tblcheck.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Nhà cung cấp này đã phát sinh phiếu mua hàng. Không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlcheckpbg = $"{URL}NhaCC/GetCP?action=GETCNCCPBG&para1={maNCC}&para2=&para3=&para4=&para5=";
                    string jsoncheckpbg = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpbg); }).Result;
                    DataTable tblcheckpbg = JsonConvert.DeserializeObject<DataTable>(jsoncheckpbg);

                    if (tblcheckpbg != null && tblcheckpbg.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Nhà cung cấp này đã phát sinh phiếu báo giá. Không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (CheckIsUsedPhieuBaoGia(maNCC, "", "", ""))
                    {
                        XtraMessageBox.Show("Nhà cung cấp này đã được sử dụng trong phiếu báo giá. Không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (CheckIsUsedPhieuMuaHang(maNCC, "", ""))
                    {
                        XtraMessageBox.Show("Nhà cung cấp này đã được sử dụng trong phiếu mua hàng. Không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlcheck1 = $"{URL}NhaCC/Get?action=CHECKNCCMMTB&para={maNCC}&para1=&para2=&para3=&para4=&para5=";
                    string jsCheck1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck1); }).Result;
                    if (!string.IsNullOrEmpty(jsCheck1))
                    {
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsCheck1);
                        if (tbl != null && tbl.Rows.Count > 0)
                        {
                            int isUsed = 0;
                            int.TryParse(tbl.Rows[0]["IsUsed"]?.ToString(), out isUsed);
                            if (isUsed == 1)
                            {
                                XtraMessageBox.Show("Nhà cung cấp này đã được sử dụng trong thiết bị. Không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (id > 0)
                    {
                        if (isMMTB == 0)
                        {
                            string url = $"{URL}ERPThuVienNK/Delete?Action=DELETEKH&para={id}&para2={GlobleData.UserName}";
                            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                            if (result.ToLower() != "true")
                            {
                                XtraMessageBox.Show("Lỗi khi xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            string url = $"{URL}NhaCC/DeleteNCC?para={maNCC}";
                            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                            if (result.ToLower() != "true")
                            {
                                XtraMessageBox.Show("Lỗi khi xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        clsWaitForm.ShowSuccessForm(this, 1000);
                    }
                    tblThuVien.Rows.Remove(dr);
                    gCThuVien.DataSource = tblThuVien;
                    this.ActiveControl = simpleButton1;
                }
            }
            catch (Exception ex){}
        }
        private bool checkDup(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return false;


            var duplicates = dt.AsEnumerable()
                .GroupBy(row => new
                {
                    TenKH = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("TenKH"))),
                    SoDienThoai = row.Field<string>("SoDienThoai")
                })
                .Where(g => g.Count() > 1);


            return duplicates.Any();

        }
        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView5.ActiveFilterString = "";
            gridView3.ActiveFilterString = "";
            _searchTargetMaCLMMTB = null;
            _searchTargetMaCLLK = null;
            loadData();
            DataRow rowNCC = gVThuVien.GetFocusedDataRow();
            if (rowNCC != null)
            {
                int isMMTB = Convert.ToInt32(rowNCC["IsMMTB"]);

                if (isMMTB == 0)
                {
                    LoadLoai(_mancc);
                    if (_tencl.Equals("Nguyên phụ liệu", StringComparison.OrdinalIgnoreCase))
                    {
                        LoadCL(_mancc, _maloai);
                    }
                    else
                    {
                        gridControl2.DataSource = null;
                    }
                }
                else
                {
                    gridControl5.DataSource = null;
                    gridControl3.DataSource = null;
                }
            }
        }
        private string ReplaceSpecialCharacters(string input)
        {
            try
            {
                if (input == null) return input;
                string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
                string replacement = "_";
                Regex regex = new Regex(pattern);
                return regex.Replace(input, replacement);
            }
            catch (Exception ex)
            {

                return input;
            }

        }
        public string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {

                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'Đ')
                    {
                        stringBuilder.Append('D');
                    }
                    else if (c == 'đ')
                    {
                        stringBuilder.Append('d');
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        #region phân quyền
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;

        private void gCThuVien_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gVNL_KeyPress(grid.FocusedView, e);
        }

        private void gVThuVien_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView != null && gridView.FocusedColumn != null)
            {
                string columnName = gridView.FocusedColumn.FieldName;
                if (columnName == "TenKH" || columnName == "TenKH")
                {
                    if (Char.IsLetter(e.KeyChar))

                        e.KeyChar = Char.ToUpper(e.KeyChar);
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
                btnThem.Enabled = false;
            }
            if (!_allowEdit && !_allowAdd)
            {
                btnLuu.Enabled = false;
            }

            if (!_allowDelete)
                btnXoa.Enabled = false;

        }

        #endregion
        private void gC_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {

                GridControl grid = sender as GridControl;
                GridView view = grid.FocusedView as GridView;
                if (view != null && view.OptionsBehavior.Editable)
                {
                    string clipboardText = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(clipboardText) && (clipboardText.Contains("\n") || clipboardText.Contains("\r")))
                    {
                        string singleLineText = Regex.Replace(clipboardText, @"\r\n?|\n", " ");
                        view.ShowEditor();
                        if (view.ActiveEditor is DevExpress.XtraEditors.TextEdit editor)
                        {
                            editor.SelectedText = singleLineText;
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        else
                        {
                            view.SetFocusedValue(singleLineText);
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                    }
                }
            }
        }

        private void gVThuVien_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow row = gVThuVien.GetFocusedDataRow();

            if (row == null) return;
            int isMMTB = Convert.ToInt32(row["IsMMTB"]);

            if (isMMTB == 1)
                _mancc = row["MaKH"]?.ToString();
            else
                _mancc = row["MaNhaCC"]?.ToString();
            if (isMMTB == 1)
            {
                xtraTabPage3.PageVisible = true;
                xtraTabPage4.PageVisible = false;
                xtraTabControl2.SelectedTabPage = xtraTabPage4;
                layoutControlGroup5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlGroup3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlGroup1.Text = "CHỦNG LOẠI MÁY MÓC THIẾT BỊ - LINH KIỆN";

                string maNhomNCC = row["NhomHangHoa"]?.ToString();
                LoadLoai(_mancc);
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (gridView1.DataRowCount <= 0)
                        {
                            gridControl3.DataSource = null;
                            return;
                        }
                        int targetHandle = -1;
                        for (int i = 0; i < gridView1.DataRowCount; i++)
                        {
                            int rowHandle = gridView1.GetRowHandle(i);
                            if (gridView1.IsGroupRow(rowHandle)) continue;

                            string tenNhom = gridView1.GetRowCellValue(rowHandle, "TenNhom")?.ToString();
                            if (!string.IsNullOrEmpty(tenNhom) &&
                                tenNhom.Equals("Máy móc thiết bị", StringComparison.OrdinalIgnoreCase))
                            {
                                targetHandle = rowHandle;
                                break;
                            }
                        }

                        if (targetHandle < 0) return;
                        gridView1.FocusedRowHandle = targetHandle;
                        gridView1.MakeRowVisible(targetHandle);
                       
                        _maloai = gridView1.GetRowCellValue(targetHandle, "MaLoaiNCC")?.ToString();
                        _tencl = "Máy móc thiết bị";

                        if (!string.IsNullOrEmpty(maNhomNCC))
                            LoadCLMMTBLK(maNhomNCC);
                    }
                    catch { }
                }));
            }
            else
            {
                xtraTabPage3.PageVisible = true;
                xtraTabPage4.PageVisible = false;
                xtraTabControl2.SelectedTabPage = xtraTabPage3;
                layoutControlGroup5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlGroup3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlGroup6.Text = "CHỦNG LOẠI HÀNG HÓA";
                layoutControlGroup1.Text = "CHỦNG LOẠI VẬT TƯ";

                LoadLoai(_mancc);
            }
        }
        /*Write log when modify row*/
        List<LogThuvienEntity> lstLog = new List<LogThuvienEntity>();

        private void gridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {

            DataRow row_focus = gVThuVien.GetFocusedDataRow() as DataRow;

            if (row_focus != null)
            {
                int.TryParse(row_focus["ID"]?.ToString(), out int ID);
                if (ID > 0)
                {
                    string content = clsWriteLogThuVienLib.FormatRow(row_focus);
                    var Query = lstLog.FirstOrDefault(x => x.ID == ID);
                    if (Query != null)
                    {
                        Query.Content = content;
                    }
                    else
                    {
                        lstLog.Add(new LogThuvienEntity
                        {
                            ID = ID,
                            Action = $"Sửa {this.Text}",
                            Module = this.Name,
                            Content = content,
                            UserID = GlobleData.UserName,
                            CreatedDate = DateTime.Now

                        });
                    }
                }


            }
        }

        private void LoadLoai(string mancc)
        {
            string url = $"{URL}NhaCC/Get?action=GETLOAINCC&para={mancc}&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            string urlmmtb = $"{URL}NhaCC/Get?action=GETLOAINCCMMTB&para={mancc}&para1=&para2=&para3=&para4=&para5=";
            string jsmmtb = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmmtb); }).Result;
            DataTable tblmmtb = JsonConvert.DeserializeObject<DataTable>(jsmmtb);
            if (tbl != null && tblmmtb != null)
            {
                foreach (DataRow rowMMTB in tblmmtb.Rows)
                {
                    string maLoaiMMTB = rowMMTB["MaLoaiNCC"].ToString();
                    string tenNhom = rowMMTB["TenNhom"]?.ToString() ?? "";
                    if (!tenNhom.Equals("Máy móc thiết bị", StringComparison.OrdinalIgnoreCase))
                        continue;

                    DataRow targetRow = tbl.AsEnumerable()
                                           .FirstOrDefault(r => r["MaLoaiNCC"].ToString() == maLoaiMMTB);
                    if (targetRow != null)
                    {
                        targetRow["MaNhaCC"] = rowMMTB["MaNhaCC"];
                    }
                }
            }
            gridControl1.DataSource = tbl;
            gridView1.ClearSelection();
            listLoaiGoc.Clear();
            listMayMocThietBi.Clear();
            if (tbl == null || tbl.Rows.Count == 0)
                return;

            //for (int i = 0; i < tbl.Rows.Count; i++)
            //{
            //    bool isChecked = tbl.Rows[i]["MaNhaCC"] != DBNull.Value
            //                     && !string.IsNullOrEmpty(tbl.Rows[i]["MaNhaCC"].ToString());
            //    if (isChecked)
            //    {
            //        gridView1.SelectRow(i);
            //        listLoaiGoc.Add(tbl.Rows[i]["MaLoaiNCC"].ToString());
            //    }
            //}

            //if (_tencl.Equals("Nguyên phụ liệu", StringComparison.OrdinalIgnoreCase))
            //{
            //    LoadCL(_mancc, _maloai);
            //}
            //else if (_tencl.Equals("Máy móc thiết bị", StringComparison.OrdinalIgnoreCase))
            //{
            //    LoadCLMMTBLK(gVThuVien.GetFocusedRowCellValue("NhomHangHoa")?.ToString());
            //}
            //else
            //{
            //    gridControl2.DataSource = null;
            //}
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                bool isChecked = tbl.Rows[i]["MaNhaCC"] != DBNull.Value
                                 && !string.IsNullOrEmpty(tbl.Rows[i]["MaNhaCC"].ToString());
                if (!isChecked) continue;

                string maLoai = tbl.Rows[i]["MaLoaiNCC"].ToString();
                string tenNhom = tbl.Rows[i]["TenNhom"]?.ToString() ?? "";
                gridView1.SelectRow(i);
                listLoaiGoc.Add(maLoai);
                if (tenNhom.Equals("Máy móc thiết bị", StringComparison.OrdinalIgnoreCase))
                    listMayMocThietBi.Add(maLoai);
            }

            DataRow rowKH = gVThuVien.GetFocusedDataRow();
            int isMMTBNCC = rowKH != null ? Convert.ToInt32(rowKH["IsMMTB"]) : 0;

            if (isMMTBNCC == 0)
            {
                if (_tencl.Equals("Nguyên phụ liệu", StringComparison.OrdinalIgnoreCase))
                    LoadCL(_mancc, _maloai);
                else
                    gridControl2.DataSource = null;
            }
        }
        private void ReapplySelection()
        {
            if (gridView3.DataSource == null) return;

            _isLock = true;
            gridView3.BeginSelection();
            for (int i = 0; i < gridView3.DataRowCount; i++)
            {
                string maCL = gridView3.GetRowCellValue(i, "MaCL")?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(maCL) && listLinhKienMMTB.Contains(maCL))
                {
                    gridView3.SelectRow(i);
                }
            }
            gridView3.EndSelection();
            _isLock = false;
        }
        private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            string maloai = gridView1.GetFocusedRowCellValue("MaLoaiNCC")?.ToString();
            string tenloai = gridView1.GetFocusedRowCellValue("TenNhom")?.ToString();
            _tencl = tenloai;
            _maloai = gridView1.GetFocusedRowCellValue("MaLoaiNCC")?.ToString();
            string manhom = gVThuVien.GetFocusedRowCellValue("NhomHangHoa")?.ToString();
            var selectedGroup = tabbedControlGroup1.SelectedTabPage;
            //if (selectedGroup == layoutControlGroup3)
            //{
            //    LoadCL(_mancc, maloai);
            //}
            //else 
            //{
            //    Loadmmtb(_mancc, maloai);
            //}
            if (tenloai.Equals("Máy móc thiết bị", StringComparison.OrdinalIgnoreCase))
            {
                LoadCLMMTBLK(manhom);
            }
            else if (tenloai.Equals("Nguyên phụ liệu", StringComparison.OrdinalIgnoreCase))
            {
                LoadCL(_mancc, maloai);
            }
            else
            {
                gridControl2.DataSource = null;
                gridControl2.RefreshDataSource();
            }
        }

        private void LoadCL(string mancc, string maloai)
        {
            string url = $"{URL}NhaCC/Get?action=GETCHUNGLOAIVT&para={mancc}&para1={maloai}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            gridControl2.DataSource = tbl;
            gridView2.ClearSelection();
            listCLoaiGoc.Clear();

            if (tbl == null || tbl.Rows.Count == 0)
                return;

            //for (int i = 0; i < tbl.Rows.Count; i++)
            //{
            //    bool isChecked = tbl.Rows[i]["MaNhaCC"] != DBNull.Value
            //                     && !string.IsNullOrEmpty(tbl.Rows[i]["MaNhaCC"].ToString());
            //    if (isChecked)
            //    {
            //        gridView2.SelectRow(i);
            //        listCLoaiGoc.Add(tbl.Rows[i]["MaCLVT"].ToString());
            //    }
            //}
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                var maNhom = tbl.Rows[i]["MaCLVT"].ToString();
                bool isChecked = tbl.Rows[i]["MaNhaCC"] != DBNull.Value
                                 && !string.IsNullOrEmpty(tbl.Rows[i]["MaNhaCC"].ToString());
                if (isChecked)
                {
                    int rowHandle = gridView2.GetRowHandle(i); // chuyển data index -> row handle
                    if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                    {
                        gridView2.SelectRow(rowHandle);
                        listCLoaiGoc.Add(maNhom);
                    }
                }
            }

        }
        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            // Nếu là dòng đang focus
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.LightYellow;
                e.Appearance.ForeColor = Color.DarkBlue;
                e.HighPriority = true;
            }
        }

        private void SaveLoaiNCC(DataTable dt)
        {
            int rowHandle = gVThuVien.FocusedRowHandle;
            object idFocus = null;
            if (rowHandle >= 0)
                idFocus = gVThuVien.GetRowCellValue(rowHandle, "MaNhaCC");

            string url = string.Format("{0}?", URL + "NhaCC/PostKHLoai");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dt); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                loadData();

            }


            if (idFocus != null)
            {
                for (int i = 0; i < gVThuVien.RowCount; i++)
                {
                    var id = gVThuVien.GetRowCellValue(i, "MaNhaCC");
                    if (id != null && id.ToString() == idFocus.ToString())
                    {
                        gVThuVien.FocusedRowHandle = i;
                        gVThuVien.SelectRow(i);
                        break;
                    }
                }
            }
        }

        private void XoaLoaiNCC(string maNhaCC, string maLoai)
        {
            string url = string.Format("{0}?para={1}&para1={2}", URL + "NhaCC/DeleteLoaiNCC", maNhaCC, maLoai);
            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                loadData();

            }
            //loadData();
        }

        private void gridView2_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
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

            if (info.Column == gridColumn16)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            int groupIndex = gridView2.GetRowLevel(e.RowHandle);

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

        private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
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

        private void gridView2_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;
                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow rowKH = gVThuVien.GetFocusedDataRow();
                if (rowKH == null)
                {
                    XtraMessageBox.Show("Không tìm thấy nhà cung cấp.");
                    return;
                }

                int isMMTB = Convert.ToInt32(rowKH["IsMMTB"]);

                if (isMMTB == 0)
                {
                    string maNhaCC = rowKH["MaNhaCC"].ToString();

                    List<string> listLoaiMoi = new List<string>();
                    foreach (int rh in gridView1.GetSelectedRows())
                    {
                        if (rh < 0) continue;
                        DataRow dr = gridView1.GetDataRow(rh);
                        if (dr != null)
                            listLoaiMoi.Add(dr["MaLoaiNCC"].ToString());
                    }

                    var loaiThem = listLoaiMoi.Where(x => !listLoaiGoc.Contains(x)).ToList();
                    var loaiXoa = listLoaiGoc.Where(x => !listLoaiMoi.Contains(x)).ToList();

                    if (loaiThem.Count > 0)
                    {
                        DataTable dt = CreateTypeTable();
                        foreach (string loai in loaiThem)
                            dt.Rows.Add(0, "", maNhaCC, loai, "");
                        SaveLoaiNCC(dt);
                    }
                    if (loaiXoa.Count > 0)
                    {
                        foreach (string loai in loaiXoa)
                            XoaLoaiNCC(maNhaCC, loai);
                    }
                    LoadLoai(_mancc);
                }
                else
                {
                    string maNhaCCMB = rowKH["MaKH"].ToString();
                    string maNhom = rowKH["NhomHangHoa"]?.ToString();

                    if (string.IsNullOrEmpty(maNhom))
                    {
                        XtraMessageBox.Show("Nhà cung cấp này chưa có Nhóm hàng hóa. Vui lòng chọn group nhóm trước!");
                        return;
                    }
                    List<string> dsMaNhom = maNhom
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Distinct()
                        .ToList();

                    List<string> listLoaiMoi = new List<string>();
                    foreach (int rh in gridView1.GetSelectedRows())
                    {
                        if (rh < 0) continue;
                        DataRow dr = gridView1.GetDataRow(rh);
                        if (dr == null) return;
                        string tenNhom = dr["TenNhom"]?.ToString() ?? "";
                        if (!tenNhom.Equals("Máy móc thiết bị", StringComparison.OrdinalIgnoreCase))
                            continue;
                        listLoaiMoi.Add(dr["MaLoaiNCC"].ToString());
                    }

                    var clThem = listLoaiMoi.Distinct().Except(listMayMocThietBi).ToList();
                    var clXoa = listMayMocThietBi.Distinct().Except(listLoaiMoi).ToList();

                    if (clThem.Count > 0)
                    {
                        DataTable dt = CreateTypeTable();
                        foreach (string loai in clThem)
                        {
                            foreach (string nhom in dsMaNhom)
                            {
                                dt.Rows.Add(0, "", maNhaCCMB, loai, nhom);
                            }
                        }
                        string urlPost = $"{URL}NhaCC/PostCLMMTB";
                        string msResult = Task.Run(async () => await _clientExtension.PostAsync(urlPost, dt)).Result;
                    }
                    if (clXoa.Count > 0)
                    {
                        foreach (string loai in clXoa)
                            XoaLoaiNCCMMTB(maNhaCCMB, loai);
                    }
                    LoadLoai(maNhaCCMB);
                    LoadCLMMTBLK(maNhom);
                    clsWaitForm.ShowSuccessForm(this, 1000);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu không thành công: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow rowKH = gVThuVien.GetFocusedDataRow();
                if (rowKH == null)
                {
                    XtraMessageBox.Show("Không tìm thấy nhà cung cấp.");
                    return;
                }
                int isMMTB = Convert.ToInt32(rowKH["IsMMTB"]);

                if (isMMTB == 0)
                {
                    DataRow rowLoai = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    if (rowLoai == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn một Loại NCC ở lưới trên trước.");
                        return;
                    }
                    string maNhaCC = rowKH["MaNhaCC"].ToString();
                    string maloai = rowLoai["MaLoaiNCC"].ToString();

                    List<string> listCLMoi = new List<string>();
                    foreach (int rh in gridView2.GetSelectedRows())
                    {
                        if (rh < 0) continue;
                        DataRow dr = gridView2.GetDataRow(rh);
                        if (dr != null)
                            listCLMoi.Add(dr["MaCLVT"].ToString());
                    }

                    var clThem = listCLMoi.Distinct().Except(listCLoaiGoc).ToList();
                    var clXoa = listCLoaiGoc.Distinct().Except(listCLMoi).ToList();

                    if (clThem.Count > 0)
                    {
                        DataTable dt = CreateTypeTable();
                        foreach (string cloai in clThem)
                            dt.Rows.Add(0, "", maNhaCC, maloai, cloai);
                        SaveChungLoaiNCC(dt);
                    }
                    if (clXoa.Count > 0)
                    {
                        foreach (string cloai in clXoa)
                            XoaChungLoaiNCC(maNhaCC, maloai, cloai);
                    }

                    LoadCL(_mancc, _maloai);
                }
                else
                {
                    SaveLinhKienMMTB();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu không thành công: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveLinhKienMMTB()
        {
            try
            {
                gridView3.ActiveFilterString = "";

                DataRow rowKH = gVThuVien.GetFocusedDataRow();
                DataRow rowLoaiNCC = gridView1.GetFocusedDataRow();
                if (rowKH == null) return;
                string maNhaCC = rowKH["MaKH"].ToString();
                string maNhom = rowKH["NhomHangHoa"]?.ToString();
                if (string.IsNullOrEmpty(maNhom))
                {
                    XtraMessageBox.Show("Vui lòng chọn Nhóm hàng hóa trước!");
                    return;
                }
                var listMMTBMoi = new List<string>();
                var listLKMoi = new List<string>();

                foreach (int rh in gridView3.GetSelectedRows())
                {
                    if (rh < 0) continue;
                    DataRow dr = gridView3.GetDataRow(rh);
                    if (dr == null || dr["MaCL"] == DBNull.Value) continue;

                    string maCL = dr["MaCL"].ToString().Trim();
                    string loai = dr["LoaiHienThi"]?.ToString();

                    if (loai == "CLMMTB") listMMTBMoi.Add(maCL);
                    else if (loai == "LK") listLKMoi.Add(maCL);
                }
                
                string maLoaiNCC = string.Empty;
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    int rh = gridView1.GetRowHandle(i);
                    if (gridView1.IsGroupRow(rh)) continue;
                    string tenNhom = gridView1.GetRowCellValue(rh, "TenNhom")?.ToString();
                    if (tenNhom != null && tenNhom.Equals("Máy móc thiết bị", StringComparison.OrdinalIgnoreCase))
                    {
                        maLoaiNCC = gridView1.GetRowCellValue(rh, "MaLoaiNCC")?.ToString() ?? string.Empty;
                        break;
                    }
                }
                listMMTBMoi = listMMTBMoi.Distinct().ToList();
                listLKMoi = listLKMoi.Distinct().ToList();


                string urlOld = $"{URL}NhaCC/Get?action=GETCLMMTBLKSELECTED&para={maNhaCC}&para1={maNhom}&para2=&para3=&para4=&para5=";
                string jsonOld = Task.Run(async () => await _clientExtension.GetAsnyc(urlOld)).Result;

                var listMMTBCu = new List<string>();
                var listLKCu = new List<string>();

                if (!string.IsNullOrEmpty(jsonOld) && jsonOld != "[]")
                {
                    DataTable tblOld = JsonConvert.DeserializeObject<DataTable>(jsonOld);
                    foreach (DataRow r in tblOld.Rows)
                    {
                        string maCL = r["MaCL"]?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(maCL)) continue;
                        if (r["LoaiHienThi"]?.ToString() == "CLMMTB") listMMTBCu.Add(maCL);
                        else if (r["LoaiHienThi"]?.ToString() == "LK") listLKCu.Add(maCL);
                    }
                }

                var mmtbThem = listMMTBMoi.Except(listMMTBCu).ToList();
                var mmtbXoa = listMMTBCu.Except(listMMTBMoi).ToList();
                var lkThem = listLKMoi.Except(listLKCu).ToList();
                var lkXoa = listLKCu.Except(listLKMoi).ToList();

                bool hasError = false;
                if (mmtbThem.Count > 0 || lkThem.Count > 0)
                {
                    DataTable dt = CreateTypeTable();
                    foreach (string m in mmtbThem) dt.Rows.Add(0, "MMTB", maNhaCC, maLoaiNCC, m);
                    foreach (string l in lkThem) dt.Rows.Add(0, "LK", maNhaCC, maLoaiNCC, l);

                    string urlPost = string.Format("{0}?para={1}", URL + "NhaCC/PostCLLKMMTB", "");
                    string result = Task.Run(async () => await _clientExtension.PostAsync(urlPost, dt)).Result;
                    if (result.ToLower() != "true")
                    {
                        XtraMessageBox.Show("Lưu không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        hasError = true;
                    }
                }
                foreach (string m in mmtbXoa)
                    XoaCLLKMMTB(maNhaCC, maNhom, m, "");
                foreach (string l in lkXoa)
                    XoaCLLKMMTB(maNhaCC, maNhom, "", l);

                if (!hasError)
                {
                    LoadCLMMTBLK(maNhom);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu không thành công. Vui lòng thử lại !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveChungLoaiNCC(DataTable dt)
        {
            int rowHandle = gVThuVien.FocusedRowHandle;
            object idFocus = null;
            if (rowHandle >= 0)
                idFocus = gVThuVien.GetRowCellValue(rowHandle, "MaNhaCC");

            string url = string.Format("{0}?", URL + "NhaCC/PostCLKH");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dt); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                loadData();

            }

            if (idFocus != null)
            {
                for (int i = 0; i < gVThuVien.RowCount; i++)
                {
                    var id = gVThuVien.GetRowCellValue(i, "MaNhaCC");
                    if (id != null && id.ToString() == idFocus.ToString())
                    {
                        gVThuVien.FocusedRowHandle = i;
                        gVThuVien.SelectRow(i);
                        break;
                    }
                }
            }
        }

        private void XoaChungLoaiNCC(string maNhaCC, string maLoai, string chungloai)
        {

            //string urlcheckpmh = $"{URL}NhaCC/GetCP?action=GETCLNCC&para1={chungloai}&para2=&para3=&para4=&para5=";
            //string jsoncheckpmh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpmh); }).Result;
            //DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheckpmh);

            //if (tblcheck != null || tblcheck.Rows.Count > 0)
            //{
            //    XtraMessageBox.Show("Chi phí này đã có trong phiếu mua hàng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            string url = string.Format("{0}?para={1}&para1={2}&para2={3}", URL + "NhaCC/DeleteChungLoaiNCC", maNhaCC, maLoai, chungloai);
            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                loadData();

            }
            //loadData();
        }

        private void gVThuVien_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == GridMenuType.Row)
            {
                GridView view = sender as GridView;
                var menu = new DevExpress.XtraGrid.Menu.GridViewMenu(view);

                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("COPY loại và chủng loại", CopyLoai_Click));
                menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("PASTE vào NCC này", PasteLoai_Click));

                e.Menu = menu;
            }
        }

        private string _copyFrom = null;

        private void CopyLoai_Click(object sender, EventArgs e)
        {
            DataRow dr = gVThuVien.GetFocusedDataRow();
            if (dr == null) return;

            string tennhom = dr["TenKH"].ToString();
            _copyFrom = dr["MaNhaCC"].ToString();
            XtraMessageBox.Show("Đã COPY từ NCC: " + tennhom);
        }

        private void PasteLoai_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_copyFrom))
            {
                XtraMessageBox.Show("Chưa COPY dữ liệu. Chuột phải → COPY trước.");
                return;
            }

            DataRow dr = gVThuVien.GetFocusedDataRow();
            if (dr == null) return;

            string pasteTo = dr["MaNhaCC"].ToString();

            if (pasteTo == _copyFrom)
            {
                XtraMessageBox.Show("Không thể PASTE vào chính NCC nguồn.");
                return;
            }

            // Gọi hàm copy dữ liệu
            CopyLoaiNCC(_copyFrom, pasteTo);
            CopyCLVT(_copyFrom, pasteTo);
            CopyMMTB(_copyFrom, pasteTo);

            XtraMessageBox.Show("PASTE thành công!");

            loadData();
        }

        private void CopyLoaiNCC(string src, string dest)
        {
            string url = $"{URL}NhaCC/Get?action=GETCOPPYLOAINCC&para={src}&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable dt = CreateTypeTable();
            foreach (DataRow row in tbl.Rows)
            {
                dt.Rows.Add(0, "", dest, row["MaLoaiNCC"].ToString(), "");
            }

            Task.Run(async () =>
            {
                await _clientExtension.PostAsync($"{URL}NhaCC/PostKHLoai", dt);
            }).Wait();
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPhieuMuaHang frm = new frmPhieuMuaHang();
            frm.ShowDialog();
        }

        //private void simpleButton3_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (xtraTabControl2.SelectedTabPage == xtraTabPage3)
        //        {
        //            DataRow rowKH = gVThuVien.GetDataRow(gVThuVien.FocusedRowHandle);
        //            if (rowKH == null)
        //            {
        //                XtraMessageBox.Show("Không tìm thấy nhà cung cấp.");
        //                return;
        //            }

        //            string maNhaCC = rowKH["MaNhaCC"].ToString();

        //            List<string> listLoaiMoi = new List<string>();
        //            foreach (int rowHandle in gridView1.GetSelectedRows())
        //            {
        //                DataRow dr = gridView1.GetDataRow(rowHandle);
        //                if (dr != null)
        //                {
        //                    listLoaiMoi.Add(dr["MaLoaiNCC"].ToString());
        //                }
        //            }

        //            var loaiThem = listLoaiMoi.Where(x => !listLoaiGoc.Contains(x)).ToList();
        //            var loaiXoa = listLoaiGoc.Where(x => !listLoaiMoi.Contains(x)).ToList();

        //            if (loaiThem.Count > 0)
        //            {
        //                DataTable dt = CreateTypeTable();
        //                foreach (string loai in loaiThem)
        //                {
        //                    dt.Rows.Add(0, "", maNhaCC, loai, "");
        //                }
        //                SaveLoaiNCC(dt);
        //            }
        //            if (loaiXoa.Count > 0)
        //            {
        //                foreach (string loai in loaiXoa)
        //                {
        //                    XoaLoaiNCC(maNhaCC, loai);
        //                }
        //            }
        //            LoadLoai(_mancc);
        //        }
        //        else if (xtraTabControl2.SelectedTabPage == xtraTabPage4)
        //        {
        //            DataRow rowNCCMMTB = gVThuVien.GetDataRow(gVThuVien.FocusedRowHandle);
        //            if (rowNCCMMTB == null)
        //            {
        //                XtraMessageBox.Show("Không tìm thấy nhà cung cấp.");
        //                return;
        //            }

        //            string maNhaCCMB = rowNCCMMTB["MaKH"].ToString();
        //            string maNhom = rowNCCMMTB["NhomHangHoa"].ToString();
        //            List<string> listCLMoi = new List<string>();
        //            int[] selectedRows = gridView5.GetSelectedRows();

        //            foreach (int rowHandle in selectedRows)
        //            {
        //                if (rowHandle < 0) continue;

        //                DataRow dr = gridView5.GetDataRow(rowHandle);
        //                if (dr != null && dr["MaCL"] != DBNull.Value)
        //                {
        //                    listCLMoi.Add(dr["MaCL"].ToString());
        //                }
        //            }

        //            var clThem = listCLMoi.Distinct().Except(listMayMocThietBi).ToList();
        //            var clXoa = listMayMocThietBi.Distinct().Except(listCLMoi).ToList();


        //            if (clThem.Count > 0)
        //            {
        //                DataTable dt = CreateTypeTable();
        //                foreach (string macl in clThem)
        //                {
        //                    dt.Rows.Add(0, "", maNhaCCMB, maNhom, macl);
        //                }
        //                SaveCLNCCMMTB(dt);
        //            }
        //            if (clXoa.Count > 0)
        //            {
        //                foreach (string macl in clXoa)
        //                {
        //                    XoaCLMMTB(maNhaCCMB, maNhom, macl);
        //                }
        //            }
        //            LoadCLMMTB(maNhom);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Lưu không thành công. Vui lòng kiểm tra lại!");
        //        return;
        //    }     
        //}

        private bool _isLock = false;

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (_isLock) return;
            if (e.Action == CollectionChangeAction.Remove)
            {
                int rowHandle = e.ControllerRow;

                // Lấy dữ liệu của row đó
                DataRow rowData = gridView1.GetDataRow(rowHandle);
                if (rowData == null) return;
                DataRow rowKH = gVThuVien.GetDataRow(gVThuVien.FocusedRowHandle);
                if (rowKH == null)
                {
                    XtraMessageBox.Show("Không tìm thấy nhà cung cấp.");
                    return;
                }
                string maNhaCC = rowKH["MaNhaCC"].ToString();
                string maloai = rowData["MaLoaiNCC"].ToString();

                string urlcheck = $"{URL}NhaCC/Get?action=GETPhieuBaoGia&para={maNhaCC}&para1={maloai}&para2=&para3=&para4=&para5=";
                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck); }).Result;
                DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                if (tblcheck != null && tblcheck.Rows.Count > 0)
                {
                    XtraMessageBox.Show("Loại của nhà cung cấp này đã có trong phiếu báo giá. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isLock = true;
                    gridView1.SelectRow(rowHandle);
                    _isLock = false;
                    return;
                }
            }

        }

        private void gridView2_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (_isLock) return;
            if (e.Action == CollectionChangeAction.Remove)
            {
                int rowHandle = e.ControllerRow;

                // Lấy dữ liệu của row đó
                DataRow rowData = gridView2.GetDataRow(rowHandle);
                if (rowData == null) return;
                DataRow rowKH = gVThuVien.GetDataRow(gVThuVien.FocusedRowHandle);
                DataRow rowloai = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                if (rowKH == null)
                {
                    XtraMessageBox.Show("Không tìm thấy nhà cung cấp.");
                    return;
                }

                string maNhaCC = rowKH["MaNhaCC"].ToString();
                string maloai = rowloai["MaLoaiNCC"].ToString();
                string chungloai = rowData["MaCLVT"].ToString();

                string urlcheck = $"{URL}NhaCC/Get?action=GETPhieuBaoGiaDot&para={maNhaCC}&para1={maloai}&para2={chungloai}&para3=&para4=&para5=";
                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck); }).Result;
                DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                if (tblcheck != null && tblcheck.Rows.Count > 0)
                {
                    XtraMessageBox.Show("Chủng loại vật tư của nhà cung cấp này đã được sử dụng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isLock = true;
                    gridView2.SelectRow(rowHandle);
                    _isLock = false;
                    return;
                }
            }
        }

        private void CopyCLVT(string src, string dest)
        {
            //string url = $"{URL}NhaCC/Get?action=GETCHUNGLOAIVT&para={src}";
            string url = $"{URL}NhaCC/Get?action=GETCOPPYCHUNGLOAIVT&para={src}&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable dt = CreateTypeTable();
            foreach (DataRow row in tbl.Rows)
            {
                dt.Rows.Add(0, "", dest, row["MaLoaiNCC"].ToString(), row["MaCLVT"].ToString());
            }

            Task.Run(async () =>
            {
                await _clientExtension.PostAsync($"{URL}NhaCC/PostCLKH", dt);
            }).Wait();
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

        }

        private void gridView3_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (_isLock) return;

            if (e.Action == CollectionChangeAction.Add)
            {
                string maAdd = gridView3.GetRowCellValue(e.ControllerRow, "MaCL")?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(maAdd) && !listLinhKienMMTB.Contains(maAdd))
                {
                    listLinhKienMMTB.Add(maAdd);
                }
            }

            if (e.Action != CollectionChangeAction.Remove) return;

            int rowHandle = e.ControllerRow;
            if (rowHandle < 0) return;

            DataRow row = gridView3.GetDataRow(rowHandle);
            if (row == null) return;

            string loaiHienThi = row["LoaiHienThi"]?.ToString();
            string maCL = row["MaCL"]?.ToString()?.Trim();

            DataRow rowNCC = gVThuVien.GetFocusedDataRow();
            if (rowNCC == null) return;
            string maNCC = rowNCC["MaKH"]?.ToString();

            string paraMMTB = loaiHienThi == "CLMMTB" ? maCL : "";
            string paraLK = loaiHienThi == "LK" ? maCL : "";

            string urlCheck = $"{URL}NhaCC/Get?action=GETNHOM" +
                              $"&para={paraMMTB}&para1={paraLK}&para2=&para3=&para4=&para5=";
            string jsonNhom = Task.Run(async () => await _clientExtension.GetAsnyc(urlCheck)).Result;

            if (!string.IsNullOrEmpty(jsonNhom) && jsonNhom != "[]")
            {
                DataTable tblInfo = JsonConvert.DeserializeObject<DataTable>(jsonNhom);
                foreach (DataRow rowInfo in tblInfo.Rows)
                {
                    string maNhomDonLe = rowInfo["MaNhom"]?.ToString();
                    bool isUsedPBG = CheckIsUsedPhieuBaoGia(maNCC, maNhomDonLe, paraMMTB, paraLK);
                    bool isUsedPMH = CheckIsUsedPhieuMuaHang(maNCC, maNhomDonLe, maCL);

                    if (isUsedPBG || isUsedPMH)
                    {
                        string reason = isUsedPBG ? "Phiếu báo giá" : "Phiếu mua hàng";
                        XtraMessageBox.Show(
                            $"{(loaiHienThi == "CLMMTB" ? "Máy móc thiết bị" : "Linh kiện")} " +
                            $"đã được sử dụng trong {reason}. Không thể bỏ chọn!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        _isLock = true;
                        gridView3.SelectRow(rowHandle);
                        _isLock = false;
                        return;
                    }
                }
            }
            //listLinhKienMMTB.Remove(maCL);
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmTienTe frm = new frmTienTe();
            frm.ShowDialog();

            loadData();
            LoadLoai(_mancc);
            if (_tencl.Equals("Nguyên phụ liệu", StringComparison.OrdinalIgnoreCase))
            {
                LoadCL(_mancc, _maloai);
            }
            else
            {
                gridControl2.DataSource = null;
            }
        }

        private void gVThuVien_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void gridView2_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "Sort")
            {
                e.Result = e.ListSourceRowIndex1.CompareTo(e.ListSourceRowIndex2);
                e.Handled = true;
            }
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmLoaiNhaCungCap frm = new frmLoaiNhaCungCap();
            frm.ShowDialog();

            loadData();
            LoadLoai(_mancc);
            if (_tencl.Equals("Nguyên phụ liệu", StringComparison.OrdinalIgnoreCase))
            {
                LoadCL(_mancc, _maloai);
            }
            else
            {
                gridControl2.DataSource = null;
            }
        }

        private void CopyMMTB(string src, string dest)
        {
            string url = $"{URL}NhaCC/Get?action=GETCOPPYMMTB&para={src}&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable dt = CreateTypeTable(); // dùng lại hoặc tạo table MMTB riêng
            foreach (DataRow row in tbl.Rows)
            {
                dt.Rows.Add(0, "", dest, row["MaLoaiNCC"].ToString(), row["MaNhom"].ToString());
            }

            Task.Run(async () =>
            {
                await _clientExtension.PostAsync($"{URL}NhaCC/PostMMTB", dt);
            }).Wait();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmDanhGiaNCC frm = new frmDanhGiaNCC();
            //frm.ShowDialog();
        }

        private void barEditItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void CreateSearchlookupTienTe()
        {
            string url = $"{URL}NhaCC/Get?action=GetTIENTE&para==&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return;
            RepositoryItemSearchLookUpEdit rCountryEditvt = new RepositoryItemSearchLookUpEdit();
            rCountryEditvt.DataSource = tbl;
            rCountryEditvt.DisplayMember = "TenTienTe";
            rCountryEditvt.ValueMember = "TienTeID";
            rCountryEditvt.ShowClearButton = false;
            rCountryEditvt.NullText = "[Chọn Currency]";

            GridView dvViewvt = rCountryEditvt.View;
            if (dvViewvt.Columns.Count == 0)
            {
                dvViewvt.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewvt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewvt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewvt.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewvt.Columns.Add(new GridColumn { FieldName = "TenTienTe", Caption = "Tên Tiền Tệ", Name = "colMaHang", Visible = true });
                dvViewvt.Columns.Add(new GridColumn { FieldName = "MaTienTe", Caption = "Mã Tiền Tệ", Name = "colTenHang", Visible = true });

            }
            gridColumn10.ColumnEdit = rCountryEditvt;

        }

        private void CreateSearchlookupQG()
        {
            string url = $"{URL}NhaCC/Get?action=GetQG&para==&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                _dicQG = new Dictionary<string, string>();
                return;
            }
            _dicQG = tbl.AsEnumerable()
                        .ToDictionary(r => r["MaQG"].ToString(), r => r["TenQG"].ToString());
            RepositoryItemSearchLookUpEdit rCountryEditvt = new RepositoryItemSearchLookUpEdit();
            rCountryEditvt.DataSource = tbl;
            rCountryEditvt.DisplayMember = "TenQG";
            rCountryEditvt.ValueMember = "MaQG";
            rCountryEditvt.ShowClearButton = false;
            rCountryEditvt.NullText = "[Chọn QG]";

            GridView dvViewvt = rCountryEditvt.View;
            if (dvViewvt.Columns.Count == 0)
            {
                dvViewvt.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewvt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewvt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewvt.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewvt.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Tên Quốc Gia", Name = "colMaHang", Visible = true });
                dvViewvt.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã Quốc Gia", Name = "colTenHang", Visible = false });

            }
            gridColumn26.ColumnEdit = rCountryEditvt;
            //gridColumn27.ColumnEdit = rCountryEditvt;

            string url1 = $"{URL}NhaCC/GetInvoice?action=GETINVOICE&para=&para1=&para2=&para3=&para4=&para5=";
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            DataTable tbl1 = JsonConvert.DeserializeObject<DataTable>(json1);
            RepositoryItemSearchLookUpEdit rCountryEditpaidby = new RepositoryItemSearchLookUpEdit();
            rCountryEditpaidby.DataSource = tbl1;
            rCountryEditpaidby.DisplayMember = "Company";
            rCountryEditpaidby.ValueMember = "CompanyID";
            rCountryEditpaidby.ShowClearButton = false;
            rCountryEditpaidby.NullText = "";

            GridView dvViewpaidby = rCountryEditpaidby.View;
            if (dvViewpaidby.Columns.Count == 0)
            {
                dvViewpaidby.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewpaidby.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewpaidby.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewpaidby.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewpaidby.Columns.Add(new GridColumn { FieldName = "CompanyID", Caption = "ID company", Name = "colCompanyID", Visible = false });
                dvViewpaidby.Columns.Add(new GridColumn { FieldName = "Company", Caption = "Company", Name = "colCompany", Visible = true });
                dvViewpaidby.Columns.Add(new GridColumn { FieldName = "Address", Caption = "Address", Name = "colAddress", Visible = true });
                dvViewpaidby.Columns.Add(new GridColumn { FieldName = "TAX", Caption = "TAX", Name = "colTAX", Visible = true });
                dvViewpaidby.Columns.Add(new GridColumn { FieldName = "VAT", Caption = "VAT", Name = "colVAT", Visible = true });
            }
            gridColumn35.ColumnEdit = rCountryEditpaidby;

        }

        private void gVThuVien_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            //if (e.Column.FieldName != "Factory")
            //    return;

            //DataRow row = gVThuVien.GetDataRow(e.RowHandle);
            //if (row == null) return;

            //string factoryValue = row["Factory"]?.ToString(); // VD: VN;CN

            //frmChonQuocGia frm = new frmChonQuocGia(factoryValue);
            //if (frm.ShowDialog() == DialogResult.OK)
            //{
            //    row["Factory"] = string.Join(";", frm.SelectedMaQG);
            //}
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            DataRow row = gVThuVien.GetFocusedDataRow();
            if (row == null) return;

            string factoryValue = row["Factory"]?.ToString(); // VD: VN;CN

            frmChonQuocGia frm = new frmChonQuocGia(factoryValue);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                row["Factory"] = string.Join(";", frm.SelectedMaQG);
            }

            this.ActiveControl = simpleButton1;
        }

        private void gVThuVien_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName == "Factory" && e.Value != null)
            //{
            //    string[] maQGs = e.Value.ToString().Split(';');
            //    List<string> tenQGs = new List<string>();

            //    foreach (string ma in maQGs)
            //    {
            //        if (_dicQG.ContainsKey(ma))
            //            tenQGs.Add(_dicQG[ma]);
            //    }

            //    e.DisplayText = string.Join("; ", tenQGs);
            //}
            if (e.Column.FieldName == "NhomHangHoa" && e.Value != null)
            {
                string val = e.Value.ToString();
                if (string.IsNullOrEmpty(val)) return;

                string[] mas = val.Split(';');
                List<string> tens = new List<string>();

                foreach (string ma in mas)
                {
                    string key = ma.Trim();
                    if (_dicNhom.ContainsKey(key))
                        tens.Add(_dicNhom[key]);
                    else if (!string.IsNullOrEmpty(key))
                        tens.Add(key);
                }

                e.DisplayText = string.Join("; ", tens);
            }
        }

        private void CreateSearchlookupPaidBy()
        {
            string url = $"{URL}NhaCC/Get?action=GetPaidBy&para==&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtPaidBy = JsonConvert.DeserializeObject<DataTable>(json);

            if (_dtPaidBy == null || _dtPaidBy.Rows.Count == 0)
            {
                _dtPaidBy = new DataTable();
            }

            if (_dtPaidBy.Columns.Count == 0)
            {
                _dtPaidBy.Columns.Add("PaidBy", typeof(string));
            }
            repositoryItemLookUpEdit1.DataSource = _dtPaidBy;
            repositoryItemLookUpEdit1.ValueMember = "PaidBy";
            repositoryItemLookUpEdit1.DisplayMember = "PaidBy";
        }

        private void repositoryItemLookUpEdit1_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {
            if (e.DisplayValue == null) return;

            string newText = e.DisplayValue.ToString().Trim();
            if (string.IsNullOrEmpty(newText)) return;

            // 1. Kiểm tra đã tồn tại chưa
            DataRow existRow = _dtPaidBy.AsEnumerable()
                .FirstOrDefault(r => r["PaidBy"].ToString().Equals(newText, StringComparison.OrdinalIgnoreCase));

            if (existRow == null)
            {
                // 2. Thêm dòng mới vào datasource
                DataRow newRow = _dtPaidBy.NewRow();
                newRow["PaidBy"] = newText;
                _dtPaidBy.Rows.Add(newRow);
            }

            gridView1.SetFocusedRowCellValue(gridColumn35, newText);
            e.Handled = true;
        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            //var editor = sender as SearchLookUpEdit;
            ////string mancc = editor.EditValue?.ToString();
            //_maclvt = editor.EditValue?.ToString();
            ////tenncc = editor.Text?.ToString();
            //loadData1(_maclvt.ToString());
        }

        private void barEditItem1_ShownEditor(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _barSearchEditor = barManager1.ActiveEditor as SearchLookUpEdit;
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            BtThem();
        }

        private void BtThem()
        {
            frmInvoice frm = new frmInvoice();
            frm.Show();
            frm.FormClosed += (s, e) =>
            {
                CreateSearchlookupQG();
            };
        }
        private void barEditItem1_HiddenEditor(object sender, ItemClickEventArgs e)
        {
            _barSearchEditor = null;
        }

        private void loadData1(string maclvt)
        {
            try
            {
                //string url = $"{URL}ERPThuVienNK/Get?Action=GETNCC";
                string url = $"{URL}NhaCC/Get?action=GETFILTERNCC&para={maclvt}&para1=&para2=&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    tblThuVien.Clear();
                    gCThuVien.DataSource = null;
                    return;
                }

                tblThuVien = JsonConvert.DeserializeObject<DataTable>(json);
                if (!tblThuVien.Columns.Contains("IsMMTB"))
                {
                    tblThuVien.Columns.Add("IsMMTB", typeof(int));
                }
                foreach (DataRow row in tblThuVien.Rows)
                {
                    row["IsMMTB"] = 0;
                }
                gCThuVien.DataSource = tblThuVien;
                CreateSearchlookupTienTe();
                CreateSearchlookupQG();
                //CreateSearchlookupPaidBy();
                //CreateSearchlookupNCC();
                //this.ActiveControl = simpleButton1;
            }
            catch (Exception ex)
            {


            }

        }

        private void CreateSearchlookupNCC()
        {
            string url = $"{URL}NhaCC/Get?action=GETCLVT&para==&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit1.DataSource = tbl;
            repositoryItemSearchLookUpEdit1.ValueMember = "MaCLVT";
            repositoryItemSearchLookUpEdit1.DisplayMember = "ChungLoaiVatTu";

        }

        private void gVThuVien_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info == null) return;

            if (info.Column?.FieldName == "IsMMTB")
            {
                bool isMMTB = Convert.ToBoolean(info.EditValue);
                info.GroupText = isMMTB
                    ? "Nhà cung cấp Máy móc - Thiết bị"
                    : "Nhà cung cấp Vật tư";
                if (isMMTB)
                {
                    e.Appearance.ForeColor = Color.DarkRed;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    e.Appearance.ForeColor = Color.Navy;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                e.Handled = true;
            }
        }
        private void repositoryItemSearchLookUpEdit1View_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "IsNPL"
                               && info.EditValue != null);

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn16)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            int groupIndex = gridView2.GetRowLevel(e.RowHandle);

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


        private void repositoryItemSearchLookUpEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = _barDisplayText;
        }
        private void repositoryItemSearchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (_barSearchEditor == null) return;
            GridView view = sender as GridView;
            if (view == null) return;

            // Lấy tất cả row đang chọn
            int[] selectedRows = view.GetSelectedRows();
            if (selectedRows == null || selectedRows.Length == 0)
            {
                _barDisplayText = null;
                _barSearchEditor.EditValue = "";
                loadData();
                return;
            }


            List<string> maCLVTList = new List<string>();
            List<string> chungLoaiList = new List<string>();
            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle < 0) continue;

                object maCLVT = view.GetRowCellValue(rowHandle, "MaCLVT");
                object chungLoai = view.GetRowCellValue(rowHandle, "ChungLoaiVatTu");

                if (maCLVT != null)
                    maCLVTList.Add(maCLVT.ToString().Trim());

                if (chungLoai != null)
                    chungLoaiList.Add(chungLoai.ToString().Trim());

            }
            string maCLVTString = string.Join(";", maCLVTList.Distinct());
            string displayText = string.Join(";", chungLoaiList.Distinct());

            // Cập nhật DisplayText
            _barDisplayText = string.Join(";", chungLoaiList.Distinct());
            _barSearchEditor.EditValue = maCLVTString;

            loadData1(maCLVTString);
        }
        private void LoadCLMMTBLK(string maCLMMTB)
        {
            try
            {
                string url = $"{URL}NhaCC/Get?action=GETCLMBLK&para={maCLMMTB}&para1=&para2=&para3=&para4=&para5=";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

                if (string.IsNullOrEmpty(json) || json == "[]")
                {
                    gridControl3.DataSource = null;
                    return;
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                gridControl3.DataSource = tbl;

                gridView3.BeginUpdate();
                gridView3.ClearGrouping();
                gridView3.ClearSelection();

                if (gridView3.Columns["LoaiHienThi"] != null)
                {
                    gridView3.Columns["LoaiHienThi"].GroupIndex = 0;
                    gridView3.Columns["LoaiHienThi"].Visible = false;
                }
                
                gridView3.OptionsView.ShowGroupPanel = false;
                gridView3.ExpandAllGroups();
                gridView3.EndUpdate();
                if (string.IsNullOrEmpty(maCLMMTB))
                {
                    listLinhKienMMTB.Clear();
                    return;
                }

                DataRow rowNCC = gVThuVien.GetFocusedDataRow();
                if (rowNCC == null) return;

                string maNCC = rowNCC["MaKH"]?.ToString();
                string urlSelectedLK = $"{URL}NhaCC/Get?action=GETCLMMTBLKSELECTED&para={maNCC}&para1={maCLMMTB}&para2=&para3=&para4=&para5=";
                string jsonSelected = Task.Run(async () => await _clientExtension.GetAsnyc(urlSelectedLK)).Result;

                listLinhKienMMTB.Clear();

                if (!string.IsNullOrEmpty(jsonSelected) && jsonSelected != "[]")
                {
                    DataTable tblSelected = JsonConvert.DeserializeObject<DataTable>(jsonSelected);
                    HashSet<string> selectedMMTB = new HashSet<string>(
                        tblSelected.AsEnumerable()
                                   .Where(r => r["LoaiHienThi"]?.ToString() == "CLMMTB")
                                   .Select(r => r["MaCL"]?.ToString()?.Trim())
                                   .Where(x => !string.IsNullOrEmpty(x)),
                        StringComparer.OrdinalIgnoreCase
                    );

                    HashSet<string> selectedLK = new HashSet<string>(
                        tblSelected.AsEnumerable()
                                   .Where(r => r["LoaiHienThi"]?.ToString() == "LK")
                                   .Select(r => r["MaCL"]?.ToString()?.Trim())
                                   .Where(x => !string.IsNullOrEmpty(x)),
                        StringComparer.OrdinalIgnoreCase
                    );

                    listLinhKienMMTB.Clear();

                    //for (int i = 0; i < tbl.Rows.Count; i++)
                    //{
                    //    string loai = tbl.Rows[i]["LoaiHienThi"]?.ToString();
                    //    string maCL = tbl.Rows[i]["MaCL"]?.ToString()?.Trim();
                    //    if (string.IsNullOrEmpty(maCL)) continue;

                    //    bool shouldSelect = (loai == "CLMMTB" && selectedMMTB.Contains(maCL))
                    //                     || (loai == "LK" && selectedLK.Contains(maCL));

                    //    if (shouldSelect)
                    //    {
                    //        int rowHandle = gridView3.GetRowHandle(i);
                    //        if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                    //        {
                    //            gridView3.SelectRow(rowHandle);
                    //            listLinhKienMMTB.Add(maCL);
                    //        }
                    //    }
                    //}
                    for (int i = 0; i < gridView3.DataRowCount; i++)
                    {
                        string maCL = gridView3.GetRowCellValue(i, "MaCL")?.ToString()?.Trim();
                        string loai = gridView3.GetRowCellValue(i, "LoaiHienThi")?.ToString();
                        if (string.IsNullOrEmpty(maCL)) continue;

                        bool shouldSelect = (loai == "CLMMTB" && selectedMMTB.Contains(maCL))
                                         || (loai == "LK" && selectedLK.Contains(maCL));
                        if (shouldSelect)
                        {
                            gridView3.SelectRow(i);
                            listLinhKienMMTB.Add(maCL);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi load MMTB & LK: " + ex.Message,
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridView3_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return; if (e.RowHandle == view.FocusedRowHandle && e.RowHandle >= 0)
            {
                e.Appearance.BackColor = Color.FromArgb(255, 242, 198);
                e.Appearance.ForeColor = Color.Black;
                e.HighPriority = true;
            }
        }
        private void SaveLoaiMMTB(DataTable dt)
        {
            try
            {
                DataRow rowKH = gVThuVien.GetFocusedDataRow();
                if (rowKH == null) return;

                string maNCC = rowKH["MaKH"].ToString();
                string maNhomStuffNCC = rowKH["NhomHangHoa"]?.ToString();

                if (string.IsNullOrEmpty(maNhomStuffNCC))
                {
                    XtraMessageBox.Show("Nhà cung cấp này chưa được gán Nhóm hàng hóa!", "Thông báo");
                    return;
                }
                string[] dsMaNhomCuaNCC = maNhomStuffNCC.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                DataTable tblToSave = CreateTypeTable();

                foreach (DataRow rowGrid in dt.Rows)
                {
                    string maCL = rowGrid["MaChungLoai"]?.ToString();
                    if (string.IsNullOrEmpty(maCL)) continue;

                    string urlCheck = $"{URL}NhaCC/Get?action=GETCL&para=&para1={maCL}&para2=&para3=&para4=&para5=";
                    string jsonCheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlCheck)).Result;

                    if (!string.IsNullOrEmpty(jsonCheck) && jsonCheck != "[]")
                    {
                        DataTable tblInfo = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                        foreach (DataRow rowInfo in tblInfo.Rows)
                        {
                            string maNhomDonLe = rowInfo["MaNhom"]?.ToString();
                            if (dsMaNhomCuaNCC.Any(x => x.Trim() == maNhomDonLe?.Trim()))
                            {
                                DataRow newRow = tblToSave.NewRow();
                                newRow["MaNhaCC"] = maNCC;
                                newRow["MaLoaiNCC"] = maNhomDonLe;
                                newRow["MaChungLoai"] = maCL;
                                tblToSave.Rows.Add(newRow);
                            }
                        }
                    }
                }

                if (tblToSave.Rows.Count == 0) return;
                string urlPost = $"{URL}NhaCC/PostCLMMTB";
                string msResult = Task.Run(async () => await _clientExtension.PostAsync(urlPost, tblToSave)).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi save CL MMTB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gVThuVien_ShownEditor(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (view.FocusedColumn.FieldName != "NhomHangHoa")
                return;
            DataRow row = view.GetFocusedDataRow();
            if (row == null) return;
            int isMMTB = 0;
            if (row["IsMMTB"] != DBNull.Value)
                int.TryParse(row["IsMMTB"].ToString(), out isMMTB);

            if (isMMTB == 0)
            {
                view.CloseEditor();
                return;
            }
            int id = 0;
            if (row["ID"] != DBNull.Value)
                int.TryParse(row["ID"].ToString(), out id);
            if (id > 0)
            {
                string maNCC = row["MaKH"]?.ToString();
                string tenNCC = row["TenKH"]?.ToString();
                bool IsUsed = false;
                string urlcheck1 = $"{URL}NhaCC/Get?action=CHECKNCCMMTB&para={maNCC}&para1=&para2=&para3=&para4=&para5=";
                string jsCheck1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck1); }).Result; if (!string.IsNullOrEmpty(jsCheck1))
                {
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsCheck1);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        int isUsed = 0;
                        int.TryParse(tbl.Rows[0]["IsUsed"]?.ToString(), out isUsed);
                        if (isUsed == 1)
                        {
                            IsUsed = true;
                        }
                        else
                        {
                            IsUsed = false;
                        }
                    }
                }
                bool usedInPBG = CheckIsUsedPhieuBaoGia(maNCC, "", "", "");
                bool usedInPMH = CheckIsUsedPhieuMuaHang(maNCC, "", ""); if (IsUsed || usedInPBG || usedInPMH)
                {
                    string reason = "";
                    if (IsUsed) reason += "đã có trong Thiết bị; ";
                    if (usedInPBG) reason += "đã có trong Phiếu báo giá; ";
                    if (usedInPMH) reason += "đã có trong Phiếu mua hàng; "; XtraMessageBox.Show(
                     $"Nhà cung cấp '{tenNCC}' {reason.TrimEnd(';', ' ')}.\nKhông thể sửa Nhóm hàng hóa!",
                     "Cảnh báo",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Warning); view.CloseEditor();
                                return;
                }
            }
        }
        private void btnSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmSearchMMTB frmSearch = new frmSearchMMTB(this.Location.X + 100, this.Location.Y + 50);
            frmSearch.OnDataUpdate += OnSearchResult;
            frmSearch.Show(this);
        }
        private void OnSearchResult(object sender, DataTable dtResult)
        {
            if (sender is frmSearchMMTB frmSender)
            {
                frmSender.OnDataUpdate -= OnSearchResult;
                frmSender.Close();
            }

            if (dtResult == null || dtResult.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không tìm thấy kết quả phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var setMaKH = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var setMaCLMMTB = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var setMaCLLK = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            string targetMaKH = dtResult.Rows[0]["MaKH"]?.ToString()?.Trim();

            foreach (DataRow r in dtResult.Rows)
            {
                string maKH = r["MaKH"]?.ToString()?.Trim();
                string maCLMMTB = r["MaCLMMTB"]?.ToString()?.Trim();
                string maCLLK = r["MaCLLK"]?.ToString()?.Trim();

                if (!string.IsNullOrEmpty(maKH)) setMaKH.Add(maKH);
                if (!string.IsNullOrEmpty(maCLMMTB)) setMaCLMMTB.Add(maCLMMTB);
                if (!string.IsNullOrEmpty(maCLLK)) setMaCLLK.Add(maCLLK);
            }
            DataTable tblFiltered = tblThuVien.Clone();
            foreach (DataRow row in tblThuVien.Rows)
            {
                string maKH = row["MaKH"]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(maKH) && setMaKH.Contains(maKH))
                    tblFiltered.ImportRow(row);
            }

            if (tblFiltered.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không có NCC phù hợp.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            gCThuVien.DataSource = tblFiltered;
            gVThuVien.BeginUpdate();
            gVThuVien.ClearGrouping();
            if (gVThuVien.Columns["IsMMTB"] != null)
                gVThuVien.Columns["IsMMTB"].GroupIndex = 0;
            gVThuVien.OptionsView.ShowGroupPanel = false;
            gVThuVien.ExpandAllGroups();
            gVThuVien.EndUpdate();

            this.BeginInvoke(new Action(() =>
            {
                int targetHandle = gVThuVien.LocateByValue("MaKH", targetMaKH);
                if (targetHandle < 0) targetHandle = 0;
                gVThuVien.FocusedRowHandle = targetHandle;
                gVThuVien.SelectRow(targetHandle);

                DataRow rowNCC = gVThuVien.GetFocusedDataRow();
                if (rowNCC == null) return;

                int isMMTB = Convert.ToInt32(rowNCC["IsMMTB"]);
                _mancc = rowNCC["MaKH"]?.ToString();

                if (isMMTB == 1)
                {
                    string maNhom = rowNCC["NhomHangHoa"]?.ToString();
                    if (!string.IsNullOrEmpty(maNhom))
                    {
                        LoadCLMMTBLK(maNhom);
                        if (setMaCLMMTB.Count > 0 || setMaCLLK.Count > 0)
                        {
                            var filterParts = new System.Collections.Generic.List<string>();
                            foreach (string m in setMaCLMMTB)
                                filterParts.Add($"[MaCL] = '{m.Replace("'", "''")}'");
                            foreach (string l in setMaCLLK)
                                filterParts.Add($"[MaCL] = '{l.Replace("'", "''")}'");

                            gridView3.ActiveFilterString = string.Join(" OR ", filterParts);
                        }
                    }
                }
                else
                {
                    LoadLoai(_mancc);
                }

                gVThuVien.RefreshData();
            }));
        }
        private void gridView3_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;
                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }

        private void gridView3_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info == null) return;

            if (info.Column?.FieldName == "LoaiHienThi")
            {
                string val = info.EditValue?.ToString();

                if (val == "CLMMTB")
                {
                    info.GroupText = "Máy móc - Thiết bị";
                }
                else if (val == "LK")
                {
                    info.GroupText = "Linh kiện";
                }

                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                e.DefaultDraw();
                e.Handled = true;
                return;
            }
        }
        private bool CheckIsUsedPhieuBaoGia(string maNCC, string maNhom,string maCL, string maCLLK)
        {
            bool IsUsedInPBG = false;
            string urlcheck1 = $"{URL}NhaCC/Get?action=CHECKPBG&para={maNCC}&para1={maNhom}&para2={maCL}&para3={maCLLK}&para4=&para5=";
            string jsCheck1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck1); }).Result; if (!string.IsNullOrEmpty(jsCheck1))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsCheck1);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int isUsed = 0;
                    int.TryParse(tbl.Rows[0]["IsUsedInPBG"]?.ToString(), out isUsed);
                    if (isUsed == 1)
                    {
                        IsUsedInPBG = true;
                    }
                    else
                    {
                        IsUsedInPBG = false;
                    }
                }
            }
            return IsUsedInPBG;
        }
        private bool CheckIsUsedPhieuMuaHang(string maNCC, string maNhom, string maCL)
        {
            bool IsUsedInPMH = false;
            string urlcheck1 = $"{URL}NhaCC/Get?action=CHECKPMH&para={maNCC}&para1={maNhom}&para2={maCL}&para3=&para4=&para5=";

            string jsCheck1 = Task.Run(async () => await _clientExtension.GetAsnyc(urlcheck1)).Result;

            if (!string.IsNullOrEmpty(jsCheck1) && jsCheck1 != "[]")
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsCheck1);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int isUsed = 0;
                    int.TryParse(tbl.Rows[0]["IsUsedInPMH"]?.ToString(), out isUsed);
                    IsUsedInPMH = (isUsed == 1);
                }
            }
            return IsUsedInPMH;
        }
        private void XoaLoaiNCCMMTB(string maNCC, string maLoaiNCC)
        {
            try
            {
                if (CheckIsUsedPhieuBaoGia(maNCC, "", "",""))
                {
                    XtraMessageBox.Show("Chủng loại này đã được sử dụng ở phiếu báo giá nên không được xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (CheckIsUsedPhieuMuaHang(maNCC, "", ""))
                {
                    XtraMessageBox.Show("Chủng loại này đã được sử dụng ở phiếu mua hàng nên không được xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string url = $"{URL}NhaCC/DeleteMMTB_LK?action=DeleteLoaiNCCMMTB&para={maNCC}&para1={maLoaiNCC}&para2=&para3=&para4=&para5=";
                string result = Task.Run(async () => {return await _clientExtension.DeletedAsync(url);}).Result; 
                if (result.ToLower() != "true")
                {
                    XtraMessageBox.Show("Lưu không thành công, vui lòng kiểm tra lại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                clsWaitForm.ShowSuccessForm(this, 1000);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi xóa CL MMTB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void XoaCLLKMMTB(string maNCC, string maNhom, string maCLMMTB, string maCLLK)
        {
            try
            {
                if (CheckIsUsedPhieuBaoGia(maNCC, maNhom, maCLMMTB, maCLLK))
                {
                    XtraMessageBox.Show("Linh kiện này đã được sử dụng ở phiếu báo giá nên không được xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (CheckIsUsedPhieuMuaHang(maNCC, maNhom, maCLLK))
                {
                    XtraMessageBox.Show("Linh kiện này đã được sử dụng ở phiếu mua hàng nên không được xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string url = $"{URL}NhaCC/DeleteMMTB_LK?action=DeleteCLLKMMTB&para={maNCC}&para1={maNhom}&para2={maCLMMTB}&para3={maCLLK}&para4=&para5=";
                string result = Task.Run(async () =>{return await _clientExtension.DeletedAsync(url);}).Result; 
                if (result.ToLower() != "true")
                {
                    //XtraMessageBox.Show("Lưu không thành công, vui lòng kiểm tra lại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                clsWaitForm.ShowSuccessForm(this, 1000);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi xóa LK: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateSearchlookupNhomNCC()
        {
            try
            {
                string url = $"{URL}NhaCC/Get?action=GETNHOM&para=&para1=&para2=&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblNhomNCC = JsonConvert.DeserializeObject<DataTable>(json);
                if (_tblNhomNCC == null || _tblNhomNCC.Rows.Count == 0) return;
                if (_tblNhomNCC != null && _tblNhomNCC.Rows.Count > 0)
                {
                    _dicNhom = _tblNhomNCC.AsEnumerable()
                        .ToDictionary(r => r["MaNhom"].ToString(), r => r["TenNhom"].ToString());
                }
                RepositoryItemButtonEdit rBtn = new RepositoryItemButtonEdit();
                rBtn.Buttons.Clear();
                rBtn.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton());
                rBtn.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                rBtn.ButtonClick += (sender, e) =>
                {
                    this.ActiveControl = simpleButton1;
                    DataRow row = gVThuVien.GetFocusedDataRow();
                    if (row == null) return; string currentValue = row["NhomHangHoa"]?.ToString();
                    frmChonNhom frm = new frmChonNhom(currentValue, _tblNhomNCC);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        row["NhomHangHoa"] = string.Join(";", frm.SelectedMaNhom);
                        gVThuVien.RefreshRow(gVThuVien.FocusedRowHandle);   
                    }
                    this.ActiveControl = simpleButton1;
                }; if (gVThuVien.Columns["NhomHangHoa"] != null)
                {
                    gVThuVien.Columns["NhomHangHoa"].ColumnEdit = rBtn;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi tạo lookup nhóm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 
