using DevExpress.DataAccess.Excel;
using DevExpress.SpreadsheetSource;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmImPortExcelThuVien : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                      new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        string _maDH = string.Empty, _malenhsanxuat = string.Empty, _makh = string.Empty, _thuvien = string.Empty;
        bool _ChkCT = false, _ChkTH = false;
        CustomImportDataExcel _customImport;
        // Sự kiện để thông báo khi Form đóng và trả dữ liệu về
        public event EventHandler<string> DataClosed;
        List<KhachHangEntity> lstKhachHangEntity;
        List<QuocGiaEntity> lstQuocGiaEntity;
        List<ChungLoaiEntity> lstChungLoaiEntity;
        System.Data.DataTable tbl_sheet;
        private HttpClientExtension _clientExtension;
        private Dictionary<string, Action<DataTable>> importMap;

        public frmImPortExcelThuVien( string thuvien)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _customImport = new CustomImportDataExcel();
            textEdit3.Enabled = false;
            _clientExtension = new HttpClientExtension();
            _thuvien = thuvien;
            InitImportMap();
        }

        private void InitImportMap()
        {
            //importMap = new Dictionary<string, Action<DataTable>>
            //{
            //    { "KhachHang", AddList_KhachHang },
            //    { "QuocGia", AddList_QuocGia },
            //    { "ChungLoai", AddList_ChungLoai }
            //};

            importMap = new Dictionary<string, Action<DataTable>>
            {
                { "KhachHang", AddList_KhachHang },
                { "QuocGia", AddList_QuocGia },
                { "ChungLoai", AddList_ChungLoai_Optimized }
            };
        }

        private void btGetLink_Click(object sender, EventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                tbl_sheet = new DataTable();
                AddColumn_tbl_Sheet();
                textEditLink.Text = Sfd.FileName;
                CreateDefaultLookUp(textEditLink.Text);
                textEdit3.Enabled = true;
            }
        }

        private void AddColumn_tbl_Sheet()
        {
            tbl_sheet.Columns.Add("Name");
        }

        private DataTable CreateTblNhomCL()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaNhomCL", typeof(string));
            dt.Columns.Add("TenNhomCL", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            return dt;
        }

        private void CreateDefaultLookUp(string pathExecel)
        {
            ISpreadsheetSource spreadsheetSource = SpreadsheetSourceFactory.CreateSource(pathExecel);
            IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
            int worksheetcount = worksheetCollection.Count();
            for (int i = 0; i < worksheetcount; i++)
            {
                DataRow r = tbl_sheet.NewRow();
                r["Name"] = worksheetCollection[i].Name;
                tbl_sheet.Rows.Add(r);
            }
            textEdit3.Properties.DataSource = tbl_sheet;
            textEdit3.Properties.ValueMember = "Name";
            textEdit3.Properties.DisplayMember = "Name";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                simpleButton1.Enabled = false;
                string newFilePath = string.Empty;
                if (string.IsNullOrEmpty(textEditLink.Text))
                {
                    XtraMessageBox.Show(Properties.Resources.DataValid, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    simpleButton1.Enabled = true;
                    return;
                }
                if (textEdit3.EditValue == null)
                {
                    MessageBox.Show("Chưa chọn sheet!.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(textEditLink.Text))
                {
                    XtraMessageBox.Show(Properties.Resources.DataValid, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    simpleButton1.Enabled = true;
                    return;
                }

                string filePath = textEditLink.Text;
                string sheetName = textEdit3.EditValue.ToString();

                DataTable dt = ReadExcelSheet(filePath, sheetName);

                if (importMap.ContainsKey(_thuvien))
                    importMap[_thuvien].Invoke(dt);
                else
                    XtraMessageBox.Show("Thư viện chưa được hỗ trợ!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Close();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ReadExcelSheet(string filePath, string sheetName)
        {
            //lstKhachHangEntity.Clear();
            //lstQuocGiaEntity.Clear();
            //lstChungLoaiEntity.Clear();
            var source = new ExcelDataSource();
            source.FileName = filePath;
            var worksheetSettings = new ExcelWorksheetSettings(sheetName, "$A1:ZZ500");
            source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
            source.Fill();
            return source.ToDataTable();
        }


        private void AddList_KhachHang(DataTable dtSave)
        {
            try
            {
                string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                lstKhachHangEntity = !string.IsNullOrEmpty(json)
                    ? JsonConvert.DeserializeObject<List<KhachHangEntity>>(json)
                    : new List<KhachHangEntity>();

                List<string> danhsachtrung = new List<string>();
                List<string> danhsachqgchuakhaibao = new List<string>();

                foreach (DataRow row in dtSave.Rows)
                {
                    if (row[0].ToString() == "***") break;

                    string maKH = row.IsNull(0) ? "" : row[0].ToString().Trim();
                    string tenKH = row.IsNull(1) ? "" : row[1].ToString().Trim();
                    string viettat = row.IsNull(2) ? "" : row[2].ToString().Trim();
                    string tenqg = row.IsNull(3) ? "" : row[3].ToString().Trim();
                    string sdt = row.IsNull(4) ? "" : row[4].ToString().Trim();
                    string email = row.IsNull(5) ? "" : row[5].ToString().Trim();
                    string diaChi = row.IsNull(6) ? "" : row[6].ToString().Trim();
                    string maSoThue = row.IsNull(7) ? "" : row[7].ToString().Trim();
                    string nguoiDaiDien = row.IsNull(8) ? "" : row[8].ToString().Trim();
                    string ghiChu = row.IsNull(9) ? "" : row[9].ToString().Trim();
                    int maLoaiDT = 1;

                    string urlqg = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                    string jsonqg = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlqg); }).Result;
                    DataTable dtblqg = JsonConvert.DeserializeObject<DataTable>(jsonqg);
                    DataRow foundQG = dtblqg.AsEnumerable().FirstOrDefault(r => NormalizeString(r["TenQG"].ToString()) == NormalizeString(tenqg));

                    if (foundQG == null) 
                    {
                        danhsachqgchuakhaibao.Add(tenqg);
                        continue;
                    }

                    bool isTrung = lstKhachHangEntity.Any(x => x.MaKH == maKH);
                    if (isTrung)
                    {
                        danhsachtrung.Add(maKH);
                        continue;
                    }

                    lstKhachHangEntity.Add(new KhachHangEntity
                    {
                        ID = 0,
                        MaKH = "",
                        TenKH = tenKH,
                        GhiChu = ghiChu,
                        MaDT = "",
                        SDT = sdt,
                        Email = email,
                        DiaChi = diaChi,
                        MaSoThue = maSoThue,
                        NguoiDaiDien = nguoiDaiDien,
                        MaLoaiDT = maLoaiDT,
                        VietTat = viettat,
                        MaQG = foundQG["MaQG"].ToString(),
                        MaHoa = maKH
                    });

                    string urlPost = string.Format("{0}?", URL + "KhachHang/PostKhachHang");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlPost, lstKhachHangEntity); }).Result;

                    if (result.ToLower() == "true")
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    else
                        XtraMessageBox.Show(result);
                }

                if (danhsachtrung.Any())
                {
                    string dstrungmadt = string.Join(", ", danhsachtrung.Distinct());
                    XtraMessageBox.Show($"Các mã Khách Hàng bị trùng: {dstrungmadt}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (danhsachqgchuakhaibao.Any())
                {
                    string dschuakhaibao = string.Join(", ", danhsachqgchuakhaibao.Distinct());
                    XtraMessageBox.Show($"Quốc gia {dschuakhaibao} chưa được khai báo", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi Import Khách Hàng!\n" + ex.Message);
            }
        }

        private void AddList_QuocGia(DataTable dtSave)
        {
            try
            {
                string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                lstQuocGiaEntity = !string.IsNullOrEmpty(json)
                    ? JsonConvert.DeserializeObject<List<QuocGiaEntity>>(json)
                    : new List<QuocGiaEntity>();

                List<string> trung = new List<string>();

                foreach (DataRow row in dtSave.Rows)
                {
                    if (row[0].ToString() == "***") break;

                    string tenQG = row.IsNull(0) ? "" : row[0].ToString();
                    string tenTA = row.IsNull(1) ? "" : row[1].ToString();
                    string maBC = row.IsNull(2) ? "" : row[2].ToString();
                    string maDT = row.IsNull(3) ? "" : row[3].ToString();
                    string maQK = row.IsNull(4) ? "" : row[4].ToString();
                    string tenCang = row.IsNull(5) ? "" : row[5].ToString();
                    string ghiChu = row.IsNull(6) ? "" : row[6].ToString();

                    var maxMaQG = lstQuocGiaEntity
                                        .Where(x => !string.IsNullOrEmpty(x.MaQG))
                                        .Select(x => {
                                            int num;
                                            return int.TryParse(x.MaQG.Replace("QG_", ""), out num) ? num : 0;
                                        })
                                        .DefaultIfEmpty(0)
                                        .Max();

                    string urlcang = $"{URL}ERPThuVienNK/Get?Action=GETCANG";
                    string jsoncang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcang); }).Result;
                    DataTable dtblCang = JsonConvert.DeserializeObject<DataTable>(jsoncang);
                    DataRow foundCang = dtblCang.AsEnumerable().FirstOrDefault(r => NormalizeString(r["TenCang"].ToString()) == NormalizeString(tenCang));
                    bool isTrung = lstQuocGiaEntity.Any(x => x.TenQG == tenQG);
                    if (isTrung)
                    {
                        trung.Add(tenQG);
                        continue;
                    }

                    lstQuocGiaEntity.Add(new QuocGiaEntity
                    {
                        ID = 0,
                        MaQG = "QG_" + (maxMaQG + 1),
                        TenQG = tenQG,
                        GhiChu = ghiChu,
                        MaBuuChinh = maBC,
                        MaDienThoai = maDT,
                        MaQuocKy = maQK,
                        TenTiengAnh = tenTA,
                        MaCang = foundCang["MaCang"].ToString(),
                        TenCang = tenCang

                    });

                    string urlPost = string.Format("{0}?", URL + "QuocGia/PostQuocGia");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlPost, lstQuocGiaEntity); }).Result;

                    if (result.ToLower() == "true")
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    else
                        XtraMessageBox.Show(result);
                }

                if (trung.Any())
                    XtraMessageBox.Show($"Quốc gia đã tồn tại: {string.Join(", ", trung)}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi Import Quốc Gia!\n" + ex.Message);
            }
        }

        //private void AddList_ChungLoai(DataTable dtSave)
        //{
        //    try
        //    {
        //        string url = string.Format("{0}?", URL + "ChungLoai/GetChungLoai");
        //        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //        lstChungLoaiEntity = !string.IsNullOrEmpty(json)
        //            ? JsonConvert.DeserializeObject<List<ChungLoaiEntity>>(json)
        //            : new List<ChungLoaiEntity>();

        //        string urldvcl = string.Format("{0}?", URL + "DonViChungLoai/Get");
        //        string jsondvcl = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvcl); }).Result;
        //        DataTable dtdvcl = JsonConvert.DeserializeObject<DataTable>(jsondvcl);

        //        string urlnhomcl = string.Format("{0}", URL + $"ChungLoai_CongDoan/GetChungLoai?action=GetNhomCL");
        //        string jsonnhomcl = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlnhomcl); }).Result;
        //        DataTable dtnhomcl = JsonConvert.DeserializeObject<DataTable>(jsonnhomcl);

        //        List<string> trung = new List<string>();
        //        List<string> dvclChuaCo = new List<string>();
        //        List<string> nhomdvclChuaCo = new List<string>();

        //        foreach (DataRow row in dtSave.Rows)
        //        {
        //            if (row[0].ToString() == "***") break;

        //            var rowdvcl = dtdvcl.AsEnumerable().FirstOrDefault(r => r["TenDVCL"].ToString() == row[1].ToString());
        //            var rownhomcl = dtnhomcl.AsEnumerable().FirstOrDefault(r => r["TenNhomCL"].ToString().Trim() == row[0].ToString().Trim());
        //            if (rowdvcl == null)
        //            {
        //                dvclChuaCo.Add(row[1].ToString());
        //                continue;
        //            }

        //            if (rownhomcl == null)
        //            {
        //                //nhomdvclChuaCo.Add(row[0].ToString());
        //                //continue;
        //                var dts = CreateTblNhomCL();
        //                var drNew = dts.NewRow();
        //                drNew["ID"] = 0;
        //                drNew["MaNhomCL"] = "";
        //                drNew["TenNhomCL"] = row[0].ToString();
        //                drNew["GhiChu"] = "";
        //                dts.Rows.Add(drNew);
        //                string url1 = string.Format("{0}", URL + $"ChungLoai_CongDoan/PostChungLoai?action=PostNhomCL");
        //                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url1, dts); }).Result;
        //                if (msResult.ToUpper() == "TRUE")
        //                {
        //                    string urlnhomcl1 = string.Format("{0}", URL + $"ChungLoai_CongDoan/GetChungLoai?action=GetNhomCL");
        //                    string jsonnhomcl1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlnhomcl1); }).Result;
        //                    DataTable dtnhomcl1= JsonConvert.DeserializeObject<DataTable>(jsonnhomcl);
        //                    rownhomcl = dtnhomcl1.AsEnumerable().FirstOrDefault(r => r["TenNhomCL"].ToString().Trim() == row[0].ToString().Trim());
        //                }
        //            }

        //            string tenCL = row.IsNull(2) ? "" : row[2].ToString();
        //            string maDVCL = rowdvcl["MaDVCL"].ToString();
        //            string nhomchungloai = rownhomcl["MaNhomCL"].ToString();
        //            string ghiChu = row.IsNull(3) ? "" : row[3].ToString();

        //            bool isTrung = lstChungLoaiEntity.Any(x => x.TenCL == tenCL);
        //            if (isTrung)
        //            {
        //                trung.Add(tenCL);
        //                continue;
        //            }

        //            lstChungLoaiEntity.Add(new ChungLoaiEntity
        //            {
        //                TenCL = tenCL,
        //                MaNhomCL = nhomchungloai,
        //                DonViTinh = maDVCL,
        //                GhiChu = ghiChu
        //            });

        //        }

        //        if (lstChungLoaiEntity.Any())
        //        {
        //            string jsoncl = JsonConvert.SerializeObject(lstChungLoaiEntity);
        //            DataTable tbChungLoai = JsonConvert.DeserializeObject<DataTable>(jsoncl);

        //            string urlpost = string.Format("{0}", URL + $"ChungLoai_CongDoan/PostChungLoai?action=PostChungLoai");
        //            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlpost, tbChungLoai); }).Result;

        //            if (msResult.ToLower() == "true")
        //                clsWaitForm.ShowSuccessForm(this, 2000);
        //            else
        //                XtraMessageBox.Show(msResult);
        //        }

        //        if (dvclChuaCo.Any())
        //            XtraMessageBox.Show($"Đơn vị chủng loại chưa khai báo:\n{string.Join("\n", dvclChuaCo.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //        //if (nhomdvclChuaCo.Any())
        //        //    XtraMessageBox.Show($"nhóm đơn vị chủng loại chưa khai báo:\n{string.Join("\n", nhomdvclChuaCo.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //        if (trung.Any())
        //            XtraMessageBox.Show($"Chủng loại đã tồn tại: {string.Join(", ", trung)}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Lỗi Import Chủng Loại!\n" + ex.Message);
        //    }
        //}

        private void AddList_ChungLoai_Optimized(DataTable dtSave)
        {
            try
            {

                // Chạy trong Thread riêng để không block UI
                Task.Run(() =>
                {
                    // Load dữ liệu ban đầu
                    string urlCL = $"{URL}ChungLoai/GetChungLoai?";
                    string jsonCL = _clientExtension.GetAsnyc(urlCL).Result;
                    lstChungLoaiEntity = !string.IsNullOrEmpty(jsonCL)
                        ? JsonConvert.DeserializeObject<List<ChungLoaiEntity>>(jsonCL)
                        : new List<ChungLoaiEntity>();

                    string urlDVCL = $"{URL}DonViChungLoai/Get?";
                    DataTable dtdvcl =
                        JsonConvert.DeserializeObject<DataTable>(_clientExtension.GetAsnyc(urlDVCL).Result);

                    string urlNhomCL = $"{URL}ChungLoai_CongDoan/GetChungLoai?action=GetNhomCL";
                    DataTable dtnhomcl =
                        JsonConvert.DeserializeObject<DataTable>(_clientExtension.GetAsnyc(urlNhomCL).Result);

                    List<ChungLoaiEntity> lstNewCL = new List<ChungLoaiEntity>();
                    List<DataRow> lstNewNCL = new List<DataRow>();
                    List<string> trung = new List<string>();
                    List<string> dvclChuaCo = new List<string>();

                    // 1️⃣ Thu thập nhóm mới không gọi API trong vòng lặp
                    foreach (DataRow row in dtSave.Rows)
                    {
                        if (row[0].ToString() == "***") break;

                        var rowdvcl = dtdvcl.AsEnumerable()
                            .FirstOrDefault(r => r["TenDVCL"].ToString() == row[1].ToString());

                        if (rowdvcl == null)
                        {
                            dvclChuaCo.Add(row[1].ToString());
                            continue;
                        }

                        var existNCL = dtnhomcl.AsEnumerable().FirstOrDefault(r =>
                            r["TenNhomCL"].ToString().Trim().Equals(row[0].ToString().Trim(), StringComparison.OrdinalIgnoreCase));

                        bool isExistInNew = lstNewNCL.Any(n => n["TenNhomCL"].ToString().Equals(row[0].ToString(), StringComparison.OrdinalIgnoreCase));

                        if (existNCL == null && !isExistInNew)
                        {
                            DataRow dr = dtnhomcl.NewRow();
                            dr["ID"] = 0;
                            dr["MaNhomCL"] = "";
                            dr["TenNhomCL"] = row[0].ToString().Trim();
                            dr["GhiChu"] = "";
                            lstNewNCL.Add(dr);
                        }
                    }

                    // ✅ Post nhóm mới 1 lần
                    if (lstNewNCL.Any())
                    {
                        var dttPost = lstNewNCL.CopyToDataTable();
                        string urlPostNCL = $"{URL}ChungLoai_CongDoan/PostChungLoai?action=PostNhomCLexcel";
                        _clientExtension.PostAsync(urlPostNCL, dttPost).Wait();

                        // ✅ Refresh lại danh sách nhóm 1 lần
                        dtnhomcl = JsonConvert.DeserializeObject<DataTable>(_clientExtension.GetAsnyc(urlNhomCL).Result);
                    }

                    // 2️⃣ Xử lý chủng loại
                    foreach (DataRow row in dtSave.Rows)
                    {
                        if (row[0].ToString() == "***") break;

                        var rowdvcl = dtdvcl.AsEnumerable()
                            .FirstOrDefault(r => r["TenDVCL"].ToString() == row[1].ToString());

                        if (rowdvcl == null) continue;

                        var rowNCL = dtnhomcl.AsEnumerable()
                            .FirstOrDefault(r => r["TenNhomCL"].ToString().Trim() == row[0].ToString().Trim());

                        string tenCL = row.IsNull(2) ? "" : row[2].ToString();

                        if (lstChungLoaiEntity.Any(x => x.TenCL.Equals(tenCL, StringComparison.OrdinalIgnoreCase)))
                        {
                            trung.Add(tenCL);
                            continue;
                        }

                        lstNewCL.Add(new ChungLoaiEntity
                        {
                            MaCL = "",
                            TenCL = tenCL,
                            MaNhomCL = rowNCL["MaNhomCL"].ToString(),
                            DonViTinh = rowdvcl["MaDVCL"].ToString(),
                            GhiChu = row.IsNull(3) ? "" : row[3].ToString()
                        });
                    }

                    // ✅ Post chủng loại 1 lần
                    if (lstNewCL.Any())
                    {
                        DataTable tbCL = JsonConvert.DeserializeObject<DataTable>(
                            JsonConvert.SerializeObject(lstNewCL));

                        string urlPostCL = $"{URL}ChungLoai_CongDoan/PostChungLoai?action=PostChungLoai";
                        var msResult = _clientExtension.PostAsync(urlPostCL, tbCL).Result;

                        if (!msResult.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            XtraMessageBox.Show(msResult);
                        }
                    }
                    if (dvclChuaCo.Any())
                        XtraMessageBox.Show($"Đơn vị chủng loại chưa khai báo:\n{string.Join("\n", dvclChuaCo.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                    if (trung.Any())
                        XtraMessageBox.Show($"Chủng loại đã tồn tại: {string.Join(", ", trung)}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }).ContinueWith(_ =>
                {
                    
                });
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi Import Chủng Loại!\n" + ex.Message);
            }
        }



        private string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Chuẩn hóa Unicode
            string formD = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char ch in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            string noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            // Bỏ khoảng trắng và ký tự đặc biệt, chuyển sang chữ hoa
            noDiacritics = Regex.Replace(noDiacritics, @"\s+", ""); // bỏ khoảng trắng
            noDiacritics = noDiacritics.ToUpperInvariant();

            return noDiacritics;
        }

    }
    
}