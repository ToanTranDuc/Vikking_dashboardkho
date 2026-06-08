using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    public class CustomImportDataExcel
    {
        List<DataImport> _listImport;
        private HttpClientExtension _clientExtension = new HttpClientExtension();
        private string madonhang = string.Empty;
        public CustomImportDataExcel()
        {
            _listImport = new List<DataImport>();

        }

        private string ReplaceTenDonVi(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Sử dụng regex để chuyển đổi dấu thành không dấu
            string pattern = @"\p{IsCombiningDiacriticalMarks}+";
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            string result = Regex.Replace(normalizedString, pattern, string.Empty);
            result = Regex.Replace(result, @"Đ", "D");
            result = result.Replace(" ", "_");

            // Thêm hậu tố "_"+_nextID
            //result +=string.Format("{0}_{1}");
            //return string.Format("{0}_{1}",result,_nextID);
            return result;

        }
        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
        public async Task<string> Import_DMNPL(string madonhang, string malenhsanxuat, string makh, string pathExcel, string sheetIndexExcel, string urlNPL, string URL, bool _ChkCT, bool _ChkTH)
        {
            string conString = "";
            using (OleDbConnection excel_con = new OleDbConnection(conString))
            {
                try
                {
                    string worksheetName = sheetIndexExcel;
                    using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExcel))
                    {
                        DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                        worksheetName = sheetIndexExcel;
                    }

                    var source = new ExcelDataSource();
                    source.FileName = pathExcel;
                    var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$C3:D9");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                    source.Fill();
                    DataTable tbl_Styles = new DataTable();
                    tbl_Styles = source.ToDataTable();
                    int temp_tbl_Styles = tbl_Styles.Columns.Count;

                    var worksheetSettings_dt = new ExcelWorksheetSettings(worksheetName, "$A27:BZ500");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings_dt);
                    source.Fill();
                    DataTable tblColorPO = new DataTable();
                    tblColorPO = source.ToDataTable();
                    int temp_tbColorPO = tblColorPO.Columns.Count;
                    excel_con.Close();

                    List<DinhMucSaveImportEntity> listDMNL = new List<DinhMucSaveImportEntity>();
                    List<NguyenPhuLieuEntity> ListNPL = new List<NguyenPhuLieuEntity>();

                    string urlvt = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPL", madonhang, malenhsanxuat);
                    string jsonvt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlvt); }).Result;
                    DataTable tblvt = JsonConvert.DeserializeObject<DataTable>(jsonvt);

                    string urlvtct = string.Format(URL + "CanDoiDonHangTong/GetTenVTGoiY");
                    string jsonvtct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlvtct); }).Result;
                    DataTable tblvtct = JsonConvert.DeserializeObject<DataTable>(jsonvtct);

                    string colorStr = string.Empty;
                    string StrNo = string.Empty;
                    int totalAmount = 0;
                    string C_Size = string.Empty;
                    string C_DauSize = string.Empty;

                    bool isNew = true;
                    int maxID = 0;
                    string cusID = string.Empty;

                    string MaVT = string.Empty;
                    string TenVT = string.Empty;
                    string Mau = string.Empty, MaMau = string.Empty;
                    Boolean isMaNPL;

                    string idPO = string.Empty;
                    string _tenVT = "";
                    string _maVT = "";
                    string _maMau = "";
                    string _khoSize = "";
                    string _a2 = "";
                    string _a3 = "";
                    string _a4 = "";
                    string previousNhompl = string.Empty;
                    string previoustenav = string.Empty;
                    string previoustenvt = string.Empty;
                    string previoussizekho = string.Empty;
                    // Danh sách các từ khóa cần kiểm tra
                    //HashSet<string> validValues = new HashSet<string> { "VẢI CHÍNH", "VẢI PHỐI", "KEO", "VẢI DỰNG", "VẢI LÓT" };
                    List<string> mess = new List<string>();
                    List<string> messvt = new List<string>();
                    for (int i = 1; i < tblColorPO.Rows.Count; i++)
                    {
                        if (tblColorPO.Rows[i][2].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][2].ToString() != "")
                        {
                            //Replace(" ,", "\n")
                            _a2 = tblColorPO.Rows[i][2].ToString().Trim();
                            //_tenVT = tblColorPO.Rows[i][2].ToString().Trim();
                        }
                        //var rowvt = tblvt.AsEnumerable().FirstOrDefault(r => r["TenNhom"].ToString() == tblColorPO.Rows[i][2].ToString());
                        if (tblColorPO.Rows[i][3].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][3].ToString() != "")
                        {
                            _a3 = tblColorPO.Rows[i][3].ToString().Trim();
                        }

                        if (tblColorPO.Rows[i][4].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][4].ToString() != "")
                        {
                            _a4 = tblColorPO.Rows[i][4].ToString().Trim();
                        }

                        if (tblColorPO.Rows[i][5].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][5].ToString() != "")
                        {
                            _maMau = tblColorPO.Rows[i][5].ToString().Trim().Replace("\"", "");
                        }
                        else
                        {
                            return "Màu không được bỏ trống!";
                        }
                        if (tblColorPO.Rows[i][7].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][7].ToString() != "")
                        {
                            _khoSize = tblColorPO.Rows[i][7].ToString().Trim();
                        }
                        //else
                        //{
                        //    return "Khổ size không được bỏ trống!";
                        //}

                        //bool existsMaVT = tblvtct.AsEnumerable().Any(x =>
                        //    x["MaVT"].ToString() == tblColorPO.Rows[i][1].ToString().Trim());

                        //bool existsTenVT = tblvtct.AsEnumerable().Any(x =>
                        //    x["TenVT"].ToString() == tblColorPO.Rows[i][4].ToString().Trim());

                        //bool existsBoth = tblvtct.AsEnumerable().Any(x =>
                        //    x["MaVT"].ToString() == tblColorPO.Rows[i][1].ToString().Trim() &&
                        //    x["TenVT"].ToString() == tblColorPO.Rows[i][4].ToString().Trim());

                        //bool notExistsBoth = !existsMaVT && !existsTenVT;

                        //// Kiểm tra song song, không dùng "||"
                        //bool rowvtct = (existsMaVT && existsTenVT && existsBoth) || notExistsBoth;

                        //if (!rowvtct) 
                        //{
                        //    string thongbao = "Tên Vật Tư hoặc Mã Vật tư không hợp lệ, vui lòng kiểm tra lại!";
                        //    messvt.Add(thongbao);
                        //}
                        if (string.IsNullOrEmpty(tblColorPO.Rows[i][4].ToString().Trim()))
                        {
                            return "Tên Vật Tư không được để trống.!";
                        }
                        _tenVT = tblColorPO.Rows[i][4].ToString();

                        string StrDonVi = tblColorPO.Rows[i][8].ToString();

                        bool containsNumber = StrDonVi.Any(char.IsDigit);
                        if (containsNumber)
                        {
                            return "File import không đúng định dạng. Vui lòng kiểm tra lại file import.!";
                        }
                        if (tblColorPO.Rows[i][0].ToString().Trim().Replace("'", "").Contains(" "))
                        {
                            break;
                        }
                        DinhMucSaveImportEntity DMNLObj = new DinhMucSaveImportEntity();
                        DMNLObj.MaDH = madonhang;
                        DMNLObj.MaLenhSanXuat = malenhsanxuat;

                        DMNLObj.MaVT = string.IsNullOrEmpty(tblColorPO.Rows[i][1].ToString().Trim()) ? "" : tblColorPO.Rows[i][1].ToString().Trim();
                        string currentValuetenvt = Regex.Replace(_tenVT, @"\s+", " ").Trim();
                        if (currentValuetenvt == "\"")
                        {
                            // Gán MaDV bằng giá trị của dòng phía trên
                            DMNLObj.TenVT = previoustenvt;
                        }
                        else
                        {
                            // Gán MaDV bằng giá trị hiện tại
                            DMNLObj.TenVT = currentValuetenvt;
                            // Cập nhật giá trị của biến tạm
                            previoustenvt = currentValuetenvt;
                        }

                        string currentValuentenav = tblColorPO.Rows[i][3].ToString().Trim(); //tên tiếng anh này của nhóm npl
                        if (currentValuentenav == "\"")
                        {
                            // Gán MaDV bằng giá trị của dòng phía trên
                            DMNLObj.TenTAVT = previoustenav;
                        }
                        else
                        {
                            // Gán MaDV bằng giá trị hiện tại
                            DMNLObj.TenTAVT = currentValuentenav;
                            // Cập nhật giá trị của biến tạm
                            previoustenav = currentValuentenav;
                        }
                        DMNLObj.MaMau = Regex.Replace(_maMau, @"\s+", " ");
                        string currentValuesizekho = _khoSize;
                        if (currentValuesizekho == "\"")
                        {
                            // Gán MaDV bằng giá trị của dòng phía trên
                            DMNLObj.KhoVai = previoussizekho;
                        }
                        else
                        {
                            // Gán MaDV bằng giá trị hiện tại
                            DMNLObj.KhoVai = currentValuesizekho;
                            // Cập nhật giá trị của biến tạm
                            previoussizekho = currentValuesizekho;
                        }
                        if (string.IsNullOrEmpty(tblColorPO.Rows[i][8].ToString()))
                        {
                            return "Đơn Vị Tính không được để trống";
                        }
                        DMNLObj.MaDV = tblColorPO.Rows[i][8].ToString().Trim().Replace("\"", "");
                        if (string.IsNullOrEmpty(tblColorPO.Rows[i][9].ToString()))
                        {
                            return "Số Lượng không được để trống";
                        }
                        double doubleNumber = double.Parse(tblColorPO.Rows[i][9].ToString());
                        DMNLObj.SoLuong = Convert.ToInt32(doubleNumber);
                        if (!_ChkCT && !_ChkTH)
                        {
                            if (string.IsNullOrEmpty(tblColorPO.Rows[i][12].ToString()))
                            {
                                return "Cấp Phát Không được để trống";
                            }
                            DMNLObj.CapPhat = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][12]), 2);
                            if (string.IsNullOrEmpty(tblColorPO.Rows[i][10].ToString()))
                            {
                                return "Định Mức không được để trống";
                            }
                            DMNLObj.DinhMuc = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][10]), 4);
                        }
                        else
                        {
                            if (!_ChkTH)
                            {
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][12].ToString()))
                                {
                                    return "Cấp Phát Không được để trống";
                                }
                                DMNLObj.CapPhat = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][12]), 2);
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][10].ToString()))
                                {
                                    return "Định Mức không được để trống";
                                }
                                DMNLObj.DinhMuc = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][10]), 4);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][12].ToString()))
                                {
                                    return "Cấp Phát Không được để trống";
                                }
                                DMNLObj.CapPhat = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][12]), 2);
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][10].ToString()))
                                {
                                    return "Định Mức không được để trống";
                                }
                                DMNLObj.DinhMuc = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][10]), 4);
                            }

                        }
                        DMNLObj.GhiChu = tblColorPO.Rows[i][14].ToString().Trim().Replace("'", "");
                        DMNLObj.NguoiTao = GlobleData.UserName;
                        //DMNLObj.IsNL = string.IsNullOrEmpty(rowvt["IsNL"].ToString()) ? 0 : Convert.ToInt32(rowvt["IsNL"].ToString());
                        //object cellObj = tblColorPO.Rows[i][2];
                        DMNLObj.MaKH = makh;
                        DMNLObj.DinhMucHaoHut = tblColorPO.Rows[i][11].ToString() == "" ? 0 : Math.Round(Convert.ToDouble(tblColorPO.Rows[i][11]), 4);
                        DMNLObj.ThucNhan = tblColorPO.Rows[i][13].ToString() == "" ? 0 : Convert.ToDouble(tblColorPO.Rows[i][13]);
                        DMNLObj.TenLoaiVT = tblColorPO.Rows[i][2].ToString().Trim();
                        string currentValuenhomnpl = _a2;
                        if (string.IsNullOrEmpty(tblColorPO.Rows[i][2].ToString()))
                        {
                            return "Loại vật tư không được để trống";
                        }
                        else
                        {
                            if (currentValuenhomnpl == "\"")
                            {
                                // Gán MaDV bằng giá trị của dòng phía trên
                                DMNLObj.NhomNPL = previousNhompl.ToString().ToUpper().Trim();
                            }
                            else
                            {
                                // Gán MaDV bằng giá trị hiện tại
                                DMNLObj.NhomNPL = currentValuenhomnpl.ToString().ToUpper().Trim();
                                // Cập nhật giá trị của biến tạm
                                previousNhompl = currentValuenhomnpl.ToString().ToUpper().Trim();
                            }
                        }
                        DMNLObj.MauSP = Regex.Replace(tblColorPO.Rows[i][6].ToString(), @"\s+", " ").Trim();
                        string cellValue = DMNLObj.NhomNPL.ToString().ToUpper().Trim();
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            //if (validValues.Contains(cellValue))
                            //{
                            //    DMNLObj.IsNL = 1;
                            //}
                            //else
                            //{
                            //    DMNLObj.IsNL = 0;
                            //}
                            DMNLObj.IsNL = 1;
                        }
                        else
                        {
                            DMNLObj.IsNL = null;
                        }
                        bool isDuplicate = listDMNL.Any(x =>
                                            x.NhomNPL == DMNLObj.NhomNPL &&
                                            x.TenVT == DMNLObj.TenVT &&
                                            x.MaMau == DMNLObj.MaMau &&
                                            x.KhoVai == DMNLObj.KhoVai &&
                                            x.MaVT == DMNLObj.MaVT &&
                                            x.MaDV == x.MaDV);

                        if (isDuplicate)
                        {
                            string message = ($"STT: {i}. Nhóm NPL: {DMNLObj.NhomNPL} Tên VT: {DMNLObj.TenVT}, Màu: {DMNLObj.MaMau}, Khổ: {DMNLObj.KhoVai}, Đơn vị: {DMNLObj.MaDV}.");
                            mess.Add(message);
                        }
                        else
                        {
                            listDMNL.Add(DMNLObj);// Chỉ thêm nếu không trùng
                        }

                    }
                    if (messvt.Count > 0)
                    {
                        MessageBox.Show("Tên Vật Tư hoặc Mã Vật tư không hợp lệ, vui lòng kiểm tra lại!",
                                            "Cảnh báo",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                    }
                    if (mess.Count > 0)
                    {
                        string error = "Các Dòng bị trùng:\n" + string.Join("\n", mess);
                        return error;
                    }
                    frmImportCanDoiNPL frm = new frmImportCanDoiNPL(listDMNL);
                    frm.WindowState = FormWindowState.Maximized;
                    frm.ShowDialog();
                    //string json = JsonConvert.SerializeObject(listDMNL);
                    //DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
                    //string urlSaveDM = string.Format("{0}", URL + "CanDoiDonHangTong/PostDinhMucNhap");
                    //string savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, listDMNL); }).Result;
                    //if (string.Compare(savelistDM, "True") != 0)
                    //{
                    //    XtraMessageBox.Show("Lỗi import nguyên phụ liệu!(" + savelistDM + ")", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                    //}
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "True";
        }


        public async Task<string> Import_CT(string madonhang, string malenhsanxuat, string makh, string pathExcel, string sheetIndexExcel, string urlNPL, string URL, bool _ChkCT, bool _ChkTH)
        {
            string conString = "";
            using (OleDbConnection excel_con = new OleDbConnection(conString))
            {
                try
                {
                    string worksheetName = sheetIndexExcel;
                    using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExcel))
                    {
                        DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                        worksheetName = sheetIndexExcel;
                    }

                    var source = new ExcelDataSource();
                    source.FileName = pathExcel;
                    var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$C3:D9");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                    source.Fill();
                    DataTable tbl_Styles = new DataTable();
                    tbl_Styles = source.ToDataTable();
                    int temp_tbl_Styles = tbl_Styles.Columns.Count;

                    var worksheetSettings_dt = new ExcelWorksheetSettings(worksheetName, "$A27:BZ500");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings_dt);
                    source.Fill();
                    DataTable tblColorPO = new DataTable();
                    tblColorPO = source.ToDataTable();
                    int temp_tbColorPO = tblColorPO.Columns.Count;
                    excel_con.Close();

                    List<DinhMucSaveImportEntity> listDMNL = new List<DinhMucSaveImportEntity>();
                    List<NguyenPhuLieuEntity> ListNPL = new List<NguyenPhuLieuEntity>();

                    string urlvt = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPL", madonhang, malenhsanxuat);
                    string jsonvt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlvt); }).Result;
                    DataTable tblvt = JsonConvert.DeserializeObject<DataTable>(jsonvt);

                    string urlvtct = string.Format(URL + "CanDoiDonHangTong/GetTenVTGoiY");
                    string jsonvtct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlvtct); }).Result;
                    DataTable tblvtct = JsonConvert.DeserializeObject<DataTable>(jsonvtct);

                    string urlct = string.Format("{0}?madh={1}&&malenh={2}", URL + "CanDoiDonHangTong/GetCDNPL", madonhang, malenhsanxuat);
                    string jsonct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlct); }).Result;
                    DataTable tblct = JsonConvert.DeserializeObject<DataTable>(jsonct);

                    string colorStr = string.Empty;
                    string StrNo = string.Empty;
                    int totalAmount = 0;
                    string C_Size = string.Empty;
                    string C_DauSize = string.Empty;

                    bool isNew = true;
                    int maxID = 0;
                    string cusID = string.Empty;

                    string MaVT = string.Empty;
                    string TenVT = string.Empty;
                    string Mau = string.Empty, MaMau = string.Empty;
                    Boolean isMaNPL;

                    string idPO = string.Empty;
                    string _tenVT = "";
                    string _maVT = "";
                    string _maMau = "";
                    string _khoSize = "";
                    string _a2 = "";
                    string _a3 = "";
                    string _a4 = "";
                    string previousNhompl = string.Empty;
                    string previoustenav = string.Empty;
                    string previoustenvt = string.Empty;
                    string previoussizekho = string.Empty;
                    // Danh sách các từ khóa cần kiểm tra
                    //HashSet<string> validValues = new HashSet<string> { "VẢI CHÍNH", "VẢI PHỐ", "KEO", "VẢI DỰNG", "VẢI LÓT" };
                    List<string> mess = new List<string>();
                    for (int i = 1; i < tblColorPO.Rows.Count; i++)
                    {
                        if (tblColorPO.Rows[i][2].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][2].ToString() != "")
                        {
                            //Replace(" ,", "\n")
                            _a2 = tblColorPO.Rows[i][2].ToString().Trim();
                            //_tenVT = tblColorPO.Rows[i][2].ToString().Trim();
                        }
                        //var rowvt = tblvt.AsEnumerable().FirstOrDefault(r => r["TenNhom"].ToString() == tblColorPO.Rows[i][2].ToString());
                        if (tblColorPO.Rows[i][3].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][3].ToString() != "")
                        {
                            _a3 = tblColorPO.Rows[i][3].ToString().Trim();
                        }

                        if (tblColorPO.Rows[i][4].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][4].ToString() != "")
                        {
                            _a4 = tblColorPO.Rows[i][4].ToString().Trim();
                        }

                        if (tblColorPO.Rows[i][5].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][5].ToString() != "")
                        {
                            _maMau = tblColorPO.Rows[i][5].ToString().Trim().Replace("\"", "");
                        }
                        //else
                        //{
                        //    return "Màu không được bỏ trống!";
                        //}
                        if (tblColorPO.Rows[i][7].ToString().Replace("\"", "").Replace(" ", "") != "" && tblColorPO.Rows[i][7].ToString() != "")
                        {
                            _khoSize = tblColorPO.Rows[i][7].ToString().Trim().Replace("\"", "");
                        }
                        //else
                        //{
                        //    return "Khổ size không được bỏ trống!";
                        //}    

                        //_maVT = RemoveVietnameseTone(_a2.ToString().Trim().Replace(" ", ""));
                        //_tenVT = _a2.ToString().Trim() + " " + _a3.ToString().Trim() + " " + _a4.ToString().Trim();
                        //bool existsMaVT = tblvtct.AsEnumerable().Any(x =>
                        //                    x["MaVT"].ToString() == tblColorPO.Rows[i][1].ToString().Trim());

                        //bool existsTenVT = tblvtct.AsEnumerable().Any(x =>
                        //                    x["TenVT"].ToString() == tblColorPO.Rows[i][4].ToString().Trim());

                        //bool existsBoth = tblvtct.AsEnumerable().Any(x =>
                        //                            x["MaVT"].ToString() == tblColorPO.Rows[i][1].ToString().Trim() &&
                        //                             x["TenVT"].ToString() == tblColorPO.Rows[i][4].ToString().Trim());

                        //bool notExistsBoth = !existsMaVT && !existsTenVT;

                        //// Kiểm tra song song, không dùng "||"
                        //bool rowvtct = (existsMaVT && existsTenVT && existsBoth) || notExistsBoth;

                        //if (!rowvtct)
                        //{
                        //    MessageBox.Show("Tên Vật Tư hoặc Mã Vật tư không hợp lệ, vui lòng kiểm tra lại!",
                        //                    "Cảnh báo",
                        //                    MessageBoxButtons.OK,
                        //                    MessageBoxIcon.Warning);
                        //}

                        _tenVT = tblColorPO.Rows[i][4].ToString();
                        string StrDonVi = tblColorPO.Rows[i][8].ToString();

                        bool containsNumber = StrDonVi.Any(char.IsDigit);
                        if (containsNumber)
                        {
                            return "File import không đúng định dạng. Vui lòng kiểm tra lại file import.!";
                        }
                        if (tblColorPO.Rows[i][0].ToString().Trim().Replace("'", "").Contains(" "))
                        {
                            break;
                        }
                        DinhMucSaveImportEntity DMNLObj = new DinhMucSaveImportEntity();
                        DMNLObj.MaDH = madonhang;
                        DMNLObj.MaLenhSanXuat = malenhsanxuat;
                        DMNLObj.MaVT = tblColorPO.Rows[i][1].ToString().Trim();
                        string currentValuetenvt = Regex.Replace(_tenVT, @"\s+", " ").Trim();
                        if (currentValuetenvt == "\"")
                        {
                            // Gán MaDV bằng giá trị của dòng phía trên
                            DMNLObj.TenVT = previoustenvt;
                        }
                        else
                        {
                            // Gán MaDV bằng giá trị hiện tại
                            DMNLObj.TenVT = currentValuetenvt;
                            // Cập nhật giá trị của biến tạm
                            previoustenvt = currentValuetenvt;
                        }

                        string currentValuentenav = tblColorPO.Rows[i][3].ToString().Trim(); //tên tiếng anh này của nhóm npl
                        if (currentValuentenav == "\"")
                        {
                            // Gán MaDV bằng giá trị của dòng phía trên
                            DMNLObj.TenTAVT = previoustenav;
                        }
                        else
                        {
                            // Gán MaDV bằng giá trị hiện tại
                            DMNLObj.TenTAVT = currentValuentenav;
                            // Cập nhật giá trị của biến tạm
                            previoustenav = currentValuentenav;
                        }
                        DMNLObj.MaMau = Regex.Replace(_maMau, @"\s+", " ");
                        string currentValuesizekho = _khoSize;
                        if (currentValuesizekho == "\"")
                        {
                            // Gán MaDV bằng giá trị của dòng phía trên
                            DMNLObj.KhoVai = previoussizekho;
                        }
                        else
                        {
                            // Gán MaDV bằng giá trị hiện tại
                            DMNLObj.KhoVai = currentValuesizekho;
                            // Cập nhật giá trị của biến tạm
                            previoussizekho = currentValuesizekho;
                        }
                        DMNLObj.MaDV = tblColorPO.Rows[i][8].ToString().Trim().Replace("\"", "");

                        double doubleNumber = tblColorPO.Rows[i][9].ToString() == "" ? 0 : double.Parse(tblColorPO.Rows[i][9].ToString());
                        DMNLObj.SoLuong = Convert.ToInt32(doubleNumber);
                        if (!_ChkCT && !_ChkTH)
                        {
                            if (string.IsNullOrEmpty(tblColorPO.Rows[i][12].ToString()))
                            {
                                return "Cấp Phát Không được để trống";
                            }
                            DMNLObj.CapPhat = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][12]), 2);
                            if (string.IsNullOrEmpty(tblColorPO.Rows[i][10].ToString()))
                            {
                                return "Định Mức không được để trống";
                            }
                            DMNLObj.DinhMuc = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][10]), 4);
                        }
                        else
                        {
                            if (!_ChkTH)
                            {
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][12].ToString()))
                                {
                                    return "Cấp Thêm Không được để trống";
                                }
                                DMNLObj.CapThem = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][12]), 2);
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][10].ToString()))
                                {
                                    return "Định Mức không được để trống";
                                }
                                DMNLObj.DinhMuc = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][10]), 4);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][12].ToString()))
                                {
                                    return "Cấp Phát Không được để trống";
                                }
                                DMNLObj.ThuHoi = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][12]), 2);
                                if (string.IsNullOrEmpty(tblColorPO.Rows[i][10].ToString()))
                                {
                                    return "Định Mức không được để trống";
                                }
                                DMNLObj.DinhMuc = Math.Round(Convert.ToDouble(tblColorPO.Rows[i][10]), 4);
                            }

                        }
                        DMNLObj.DinhMucHaoHut = tblColorPO.Rows[i][11].ToString() == "" ? 0 : Math.Round(Convert.ToDouble(tblColorPO.Rows[i][11]), 4);
                        DMNLObj.NguoiTao = GlobleData.UserName;
                        //DMNLObj.IsNL = string.IsNullOrEmpty(rowvt["IsNL"].ToString()) ? 0 : Convert.ToInt32(rowvt["IsNL"].ToString());
                        //object cellObj = tblColorPO.Rows[i][2];
                        DMNLObj.MaKH = makh;
                        DMNLObj.ThucNhan = tblColorPO.Rows[i][13].ToString() == "" ? 0 : Convert.ToDouble(tblColorPO.Rows[i][13]);
                        DMNLObj.GhiChu = tblColorPO.Rows[i][14].ToString().Trim().Replace("'", "");
                        DMNLObj.TenLoaiVT = tblColorPO.Rows[i][2].ToString().Trim();
                        string currentValuenhomnpl = Regex.Replace(_a2.ToUpper().Trim(), @"\s+", " ");
                        if (string.IsNullOrEmpty(tblColorPO.Rows[i][2].ToString()))
                        {
                            return "Loại vật tư không được để trống";
                        }
                        else
                        {
                            if (currentValuenhomnpl == "\"")
                            {
                                // Gán MaDV bằng giá trị của dòng phía trên
                                DMNLObj.NhomNPL = previousNhompl.ToString();
                            }
                            else
                            {
                                // Gán MaDV bằng giá trị hiện tại
                                DMNLObj.NhomNPL = currentValuenhomnpl.ToString().ToUpper().Trim();
                                // Cập nhật giá trị của biến tạm
                                previousNhompl = currentValuenhomnpl.ToString().ToUpper().Trim();
                            }
                        }
                        //DMNLObj.MauSP = tblColorPO.Rows[i][6].ToString().Trim();
                        DMNLObj.MauSP = Regex.Replace(tblColorPO.Rows[i][6].ToString(), @"\s+", " ").Trim();
                        string mavttt = tblColorPO.Rows[i][1].ToString().Trim();
                        string mamauvt = Regex.Replace(_maMau, @"\s+", " ").Trim();
                        DataRow rowct = null;
                        //var rowct = tblct.AsEnumerable().FirstOrDefault(x => x["MaVT"].ToString().Trim() == mavttt.ToString()
                        //            && x["TenVT"].ToString() == currentValuetenvt.ToString()
                        //            && x["MaMau"].ToString() == Regex.Replace(_maMau, @"\s+", " ")
                        //            && x["KhoVai"].ToString().Trim().Replace("\"", "") == currentValuesizekho.ToString()
                        //            && x["MaDH"].ToString() == madonhang.ToString()
                        //            && x["MaLenhSanXuat"].ToString() == malenhsanxuat.ToString()
                        //            && x["TenNhom"].ToString() == currentValuenhomnpl.ToString());


                        foreach (DataRow row in tblct.Rows)
                        {
                            string mavtkt = row["MaVT"].ToString().Trim();
                            string tenvtkt = row["TenVT"].ToString();
                            string mamauvtkt = row["MaMau"].ToString();
                            string khovaivtkt = row["KhoVai"].ToString().Trim().Replace("\"", "");
                            string madhvtkt = row["MaDH"].ToString();
                            string mlssvtkt = row["MaLenhSanXuat"].ToString();
                            string tn = "";
                            string tntemp = RemoveVietnameseTone(row["TenNhom"].ToString());
                            string tncheck = "";
                            string tncheckTemp = RemoveVietnameseTone(currentValuenhomnpl.ToString().Trim());
                            List<byte> lstByte = new List<byte>();

                            foreach (char c in tncheckTemp)
                            {
                                if (c > 0 && c < 255)
                                    tncheck += c.ToString();
                            }
                            foreach (char c in tntemp)
                            {
                                if (c > 0 && c < 255)
                                    tn += c.ToString();
                            }
                            if (mavtkt == mavttt &&
                                 tenvtkt == currentValuetenvt.ToString() &&
                                 mamauvtkt == mamauvt.ToString() &&
                                khovaivtkt == currentValuesizekho.ToString() &&
                                madhvtkt == madonhang.ToString() &&
                                mlssvtkt == malenhsanxuat.ToString()
                                && tn.ToString().Trim() == tncheck.ToString())
                            {
                                rowct = row;
                            }
                        }
                        if (rowct != null)
                        {
                            //if (rowct.Any())
                            //{
                            //    DMNLObj.MaNPL = rowct.First()["MaNPL"].ToString(); 
                            //}
                            DMNLObj.MaNPL = rowct["MaNPL"].ToString();
                        }
                        else
                        {
                            DMNLObj.MaNPL = "";
                        }

                        string cellValue = DMNLObj.NhomNPL.ToString().Trim();
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            //if (validValues.Contains(cellValue))
                            //{
                            //    DMNLObj.IsNL = 1;
                            //}
                            //else
                            //{
                            //    DMNLObj.IsNL = 0;
                            //}
                            DMNLObj.IsNL = 1;
                        }
                        else
                        {
                            DMNLObj.IsNL = null;
                        }
                        bool isDuplicate = listDMNL.Any(x =>
                                            x.NhomNPL == DMNLObj.NhomNPL &&
                                            x.TenVT == DMNLObj.TenVT &&
                                            x.MaMau == DMNLObj.MaMau &&
                                            x.KhoVai == DMNLObj.KhoVai &&
                                            x.MaVT == DMNLObj.MaVT &&
                                            x.MaDV == x.MaDV);

                        if (isDuplicate)
                        {
                            string message = ($"STT: {i}. Nhóm NPL: {DMNLObj.NhomNPL} Tên VT: {DMNLObj.TenVT}, Màu: {DMNLObj.MaMau}, Khổ: {DMNLObj.KhoVai}, Đơn vị: {DMNLObj.MaDV}.");
                            mess.Add(message);
                        }
                        else
                        {
                            listDMNL.Add(DMNLObj);// Chỉ thêm nếu không trùng
                        }


                    }
                    frmImportCanDoiNPL frm = new frmImportCanDoiNPL(listDMNL);
                    frm.WindowState = FormWindowState.Maximized;
                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "True";
        }
    }

    class DataImport
    {
        public string StyleID { get; set; }
        public string CustomerName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Amount { get; set; }
        public string SeaSon { get; set; }
    }
    class DataImportExcel
    {
        public string MaLenh { get; set; }
        public string MaKhachHang { get; set; }
        public string MaQG { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string SPOID { get; set; }
        public string PO { get; set; }
        public string ColorID { get; set; }
        public string SizeStyleID { get; set; }
        public string Size { get; set; }
        public DateTime NgayMua { get; set; }
        public DateTime NgayGH { get; set; }
        public int Amount { get; set; }
    }
}
