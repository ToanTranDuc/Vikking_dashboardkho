using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPL_TachKien : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private DataRow _dr;
        private DataTable _tblKienGOC = new DataTable();
        public DataTable tblNhapKho { get; set;}
        int _sokien = 1;
        private decimal _tileNW = 0m, _tileGW = 0m;
        string _barcodeGhep = string.Empty;
        public frmERPNhapKhoNPL_TachKien()
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            InitializeComponent();
        }
        public frmERPNhapKhoNPL_TachKien(DataRow row, int _sk=0,DataTable tblGC = null)
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            InitializeComponent();

            if (row == null) this.DialogResult = DialogResult.OK;
            _tblKienGOC = tblGC;
            tblNhapKho = new DataTable();
            CreateTableNhapKho();
            _dr = tblNhapKho.NewRow();
            txtKien.Text = row["SoKienHienThi"].ToString();
            txtSoGhiDauCay.Text = row["TonKho"].ToString();
            setDataRow(row);
            _barcodeGhep = GetBarCodeGhep(row["BarCodeGoc"]?.ToString(), row["KienGoc"]?.ToString(), tblGC);
            //if(_sk>_sokien)
            //{


            //}    
            _sokien = _sk;
            loadSoKien(row["BarCode"]?.ToString());
            this.ActiveControl = simpleButton1;
        }

        private string GetBarCodeGhep(string BarCodeGoc, string KienGoc,DataTable tblGC)
        {
            if (tblGC?.Rows?.Count > 0)
            {
                var Query = tblGC.AsEnumerable().FirstOrDefault(x => x["SoKienParent"]?.ToString() == x["BarCode"]?.ToString() && x["BarCode"]?.ToString() == BarCodeGoc);
                if (Query != null)
                {
                    KienGoc = Query["KienGoc"]?.ToString();
                }
            }


            Func<string, string> RemoveSuffix = input =>
            {
                if (string.IsNullOrWhiteSpace(input)) return input;

                string trimmed = input.Trim();
                if (trimmed.EndsWith(KienGoc, StringComparison.OrdinalIgnoreCase))
                {
                    return trimmed.Substring(0, trimmed.Length - KienGoc.Length).Trim();
                }
                return trimmed;
            };

            
            string cleanedBarcode = RemoveSuffix(BarCodeGoc);
            

            return cleanedBarcode;
        }
        private bool HasChildItems(string skhienthi)
        {
           
            var match = Regex.Match(skhienthi, @"\.(\d+)$");

            if (match.Success)
            {
                // Tạo prefix để tìm kiếm - tối ưu hơn regex
                string prefix = skhienthi + ".";

                // Kiểm tra có item nào bắt đầu bằng prefix và là con trực tiếp
                return _tblKienGOC.AsEnumerable().Select(x=>x["SoKienHienThi"]?.ToString()).Any(item =>
                    item.StartsWith(prefix) &&
                    IsDirectChild(item, prefix));
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra item có phải là con trực tiếp không (helper method)
        /// </summary>
        private bool IsDirectChild(string item, string prefix)
        {
            // Lấy phần sau prefix
            string suffix = item.Substring(prefix.Length);

            // Kiểm tra suffix chỉ chứa số (không có dấu chấm) - là con trực tiếp
            return Regex.IsMatch(suffix, @"^\d+$");
        }
        private void loadSoKien(string BarcodeGoc)
        {
            try
            {
                if(_sokien == 0)
                {
                    string _skhienthi, SoKienGoc = string.Empty;
                    string url = $"{URL}ERPNhapKhoNPLTachKien/Get?Action=GETSOKIEN&para={BarcodeGoc}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json != "[]")
                    {
                        DataTable tblSK = JsonConvert.DeserializeObject<DataTable>(json);
                        _skhienthi = tblSK.Rows[0]["SoKien"].ToString();
                        SoKienGoc = tblSK.Rows[0]["KienGoc"].ToString();
                        if (_skhienthi == SoKienGoc)
                        {
                            _sokien = 1;
                            return;
                        }

                        var match = Regex.Match(_skhienthi, @"\.(\d+)$");
                        //if (match.Success)
                         if (HasChildItems(_skhienthi))
                        {
                            string[] arrSoKien = _skhienthi.Split('.');
                            int _sokienAPI = (arrSoKien.Length > 0 && int.TryParse(arrSoKien[arrSoKien.Length - 1], out int temp) ? temp : 0) + 1;
                            if (_sokienAPI > _sokien)
                            {
                                _sokien = _sokienAPI;
                            }
                        }

                    }
                    else
                    {
                        if (_dr != null)
                        {
                            _skhienthi = _dr["SoKienHienThi"].ToString();
                            string BarCodeGoc = _dr["SoKienParent"].ToString();
                            string BarCode = _dr["BarCode"].ToString();
                            if (BarCodeGoc == BarCode)
                            {
                                _sokien = 1;
                                return;
                            }
                            else
                            {
                                var match = Regex.Match(_skhienthi, @"\.(\d+)$");
                                if (HasChildItems(_skhienthi))
                                {
                                    string[] arrSoKien = _skhienthi.Split('.');
                                    int _sokienAPI = (arrSoKien.Length > 0 && int.TryParse(arrSoKien[arrSoKien.Length - 1], out int temp) ? temp : 0) + 1;
                                    if (_sokienAPI > _sokien)
                                    {
                                        _sokien = _sokienAPI;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    _sokien = _sokien + 1;
                }
                
            }
            catch (Exception ex)
            {

                
            }
            
        }
        private void setDataRow(DataRow row)
        {
            try
            {
                if (row == null) return;
                _dr["ID"] = row["ID"];
                _dr["SoLoID"] = row["SoLoID"];
                _dr["MaNPL"] = row["MaNPL"];
                _dr["MaVTID"] = row["MaVTID"];
                _dr["MaMauVT"] = row["MaMauVT"];
                _dr["MauVT"] = row["MauVT"];
                _dr["SoKien"] = row["SoKien"];
                _dr["SoLot"] = row["SoLot"];
                _dr["MaHaiQuan"] = row["MaHaiQuan"];
                _dr["MaKeToan"] = row["MaKeToan"];
                _dr["SoGhiDauCay"] = row["SLCTView"];
                _dr["NW"] = row["NWView"];
                _dr["GW"] = row["GWView"];
                _dr["Barcode"] = row["Barcode"];
                _dr["GhiChu"] = row["GhiChu"];
                _dr["IsNPL"] = row["IsNPL"];
                _dr["KhoVaiID"] = row["KhoVaiID"];
                _dr["SoKienParent"] = row["SoKienParent"];
                _dr["SoLuongThucTe"] = row["SLTTView"];
                _dr["DonGia"] = row["DonGia"];
                _dr["ThanhTien"] = row["ThanhTien"];
                _dr["Pallet"] = row["Pallet"];
                _dr["MaDVVT"] = row["MaDVVT"];
                _dr["MauVTID"] = row["MauVTID"];
                _dr["SoKienHienThi"] = row["SoKienHienThi"];
                _dr["IsNK"] = row["IsNK"];
                _dr["NgayNKDuKien"] = row["NgayNKDuKien"];
                _dr["MaDVCD"] = row["MaDVCD"];
                _dr["TenDVCD"] = row["TenDVCD"];
                _dr["tileNW"] = row["tileNW"];
                _dr["tileGW"] = row["tileGW"];
               
                _dr["BarCodeGoc"] = row["BarCodeGoc"];
                _dr["NgayNhapKho"] = row["NgayNhapKho"];
                _dr["TonKho"] = row["TonKho"];
                _dr["KienGoc"] = row["KienGoc"];

                _dr["TienTe"] = row["TienTe"];
                _dr["MaVTGhep"] = row["MaVTGhep"];
                _dr["MaNhom"] = row["MaNhom"];
                _dr["QuyDoiID"] = row["QuyDoiID"];
                _dr["POMua"] = row["POMua"];
                _dr["Batch"] = row["Batch"];
                _dr["SLTong"] = row["SLTong"];
                if (!string.IsNullOrWhiteSpace(row["tileNW"].ToString()))
                {
                    _tileNW = decimal.TryParse(row["tileNW"].ToString(), out var val) ? val : 0m;
                }
                else
                {
                    //lấy số lượng theo chứng từ chia cho khối lượng
                    decimal soGhiDauCay = decimal.TryParse(row["SoGhiDauCay"].ToString(), out var val) ? val : 0m;
                    decimal nw = decimal.TryParse(row["NW"].ToString(), out var val1) ? val1 : 0m;
                    _tileNW = nw != 0 ? Math.Round(soGhiDauCay / nw, 2) : 0m;
                }
                if (!string.IsNullOrWhiteSpace(row["tileGW"].ToString()))
                {
                    _tileGW = decimal.TryParse(row["tileGW"].ToString(), out var val) ? val : 0m;
                }
                else
                {
                    _tileGW = (decimal.TryParse(row["GW"].ToString(), out var val) ? val : 0m) - (decimal.TryParse(row["NW"].ToString(), out var val2) ? val2 : 0m);
                }

            }
            catch (Exception ex)
            {

            }

        }
        private void CreateTableNhapKho()
        {
            tblNhapKho = new DataTable("tblNhapKho");
            tblNhapKho.Columns.Add("ID", typeof(int));
            tblNhapKho.Columns.Add("SoLoID", typeof(string));
            tblNhapKho.Columns.Add("MaNPL", typeof(string));
            tblNhapKho.Columns.Add("MaVTID", typeof(string));
            tblNhapKho.Columns.Add("MaMauVT", typeof(string));
            tblNhapKho.Columns.Add("MauVT", typeof(string));
            tblNhapKho.Columns.Add("SoKien", typeof(string));
            tblNhapKho.Columns.Add("SoLot", typeof(string));
            tblNhapKho.Columns.Add("MaHaiQuan", typeof(string));
            tblNhapKho.Columns.Add("MaKeToan", typeof(string));
            tblNhapKho.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblNhapKho.Columns.Add("NW", typeof(decimal));
            tblNhapKho.Columns.Add("GW", typeof(decimal));
            tblNhapKho.Columns.Add("BarCode", typeof(string));
            tblNhapKho.Columns.Add("GhiChu", typeof(string));
            tblNhapKho.Columns.Add("IsNPL", typeof(bool));
            tblNhapKho.Columns.Add("KhoVaiID", typeof(string));
            tblNhapKho.Columns.Add("SoKienParent", typeof(string));//barcode gốc
            tblNhapKho.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblNhapKho.Columns.Add("DonGia", typeof(decimal));
            tblNhapKho.Columns.Add("ThanhTien", typeof(decimal));
            tblNhapKho.Columns.Add("Pallet", typeof(string));
            tblNhapKho.Columns.Add("MaDVVT", typeof(string));
            tblNhapKho.Columns.Add("MauVTID", typeof(string));
            tblNhapKho.Columns.Add("SoKienHienThi", typeof(string));
            tblNhapKho.Columns.Add("IsNK", typeof(bool));
            tblNhapKho.Columns.Add("STT", typeof(string));
            tblNhapKho.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblNhapKho.Columns.Add("MaDVCD", typeof(string));
            tblNhapKho.Columns.Add("TenDVCD", typeof(string));
            tblNhapKho.Columns.Add("tileNW", typeof(decimal));
            tblNhapKho.Columns.Add("tileGW", typeof(decimal));
            //tblNhapKho.Columns.Add("IsNK", typeof(bool));
            tblNhapKho.Columns.Add("NgayNhapKho", typeof(DateTime));
            tblNhapKho.Columns.Add("TonKho", typeof(decimal));
            tblNhapKho.Columns.Add("KienGoc", typeof(string));
            //row dư
            tblNhapKho.Columns.Add("SLCTView", typeof(decimal));
            tblNhapKho.Columns.Add("SLTTview", typeof(decimal));
            tblNhapKho.Columns.Add("NWView", typeof(decimal));
            tblNhapKho.Columns.Add("GWView", typeof(decimal));
            tblNhapKho.Columns.Add("ThanhTienView", typeof(decimal));
            tblNhapKho.Columns.Add("BarCodeGoc", typeof(string));
           
            tblNhapKho.Columns.Add("TienTe", typeof(string));
            tblNhapKho.Columns.Add("MaVTGhep", typeof(string));
            tblNhapKho.Columns.Add("MaNhom", typeof(string));
            tblNhapKho.Columns.Add("QuyDoiID", typeof(string));
            tblNhapKho.Columns.Add("POMua", typeof(string));
            tblNhapKho.Columns.Add("Batch", typeof(string));
            tblNhapKho.Columns.Add("SLTong", typeof(decimal));
        }

        private void txtTuKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDenKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSoGhiDauCay_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }
            if (currentText.Contains("."))
            {
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);
                if (decimalPart.Length >= 2 && e.KeyChar != '\b') // '\b' là phím Backspace
                {
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
            }
        }
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }
        private string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result.ToUpper();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSLTach.Text.ToString() == "")
                {
                    MessageBox.Show("Vui lòng nhập Số kiện tách.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSLTach.Focus();
                    return;
                }
                if (txtSL.Text.ToString() == "")
                {
                    MessageBox.Show("Vui lòng nhập Số lượng tách.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSL.Focus();
                    return;
                }
                if (_sokien == 0) _sokien = _sokien + 1;
                decimal _qty = decimal.TryParse(txtSL.Text.ToString(), out var val) ? val : 0m;
                int _sltach = Convert.ToInt32(txtSLTach.Text.ToString());
                decimal _qtyGoc = decimal.TryParse(_dr["SoGhiDauCay"].ToString(), out var val2) ? val2 : 0m;
                decimal _sltt = decimal.TryParse(_dr["SoLuongThucTe"].ToString(), out var val3) ? val3 : 0m;
                decimal _dongia = decimal.TryParse(_dr["DonGia"].ToString(), out var val4) ? val4 : 0m;
                decimal _TonKhoGoc = decimal.TryParse(_dr["TonKho"].ToString(), out var val5) ? val5 : 0m;

                decimal _NWGoc = decimal.TryParse(_dr["NW"].ToString(), out var GW) ? GW : 0m;
                decimal _GWGoc = decimal.TryParse(_dr["GW"].ToString(), out var NW) ? NW : 0m;

                if (_qty * _sltach >= _TonKhoGoc)
                {
                    MessageBox.Show($"Vui lòng nhập Số lượng tách nhỏ hơn SL theo CT của kiện {txtKien.Text}.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSL.Focus();
                    return;
                }
                if(_qtyGoc == 0)
                {
                    return;
                }
                for (int i = 0; i < _sltach; i++)
                {
                    DataRow _new = tblNhapKho.NewRow();
                    string SplipBarCode_Char = _dr["BarCodeGoc"].ToString().Split('|')[0];
                    string _sokienhientai = _dr["SoKienHienThi"].ToString() + "." + _sokien;
                                     

                    _new["Barcode"] = _barcodeGhep + _sokienhientai;
                    decimal GW_Child = Math.Round((_qty* _GWGoc)/ _sltt, 2);
                    decimal NW_Child = Math.Round((_qty * _NWGoc) / _sltt, 2);

                    _new["ID"] = 0;
                    _new["SoLoID"] = _dr["SoLoID"];
                    _new["MaNPL"] = _dr["MaNPL"];
                    _new["MaVTID"] = _dr["MaVTID"];
                    _new["MaMauVT"] = _dr["MaMauVT"];
                    _new["MauVT"] = _dr["MauVT"];
                    _new["SoKien"] = _sokienhientai;
                    _new["SoLot"] = _dr["SoLot"];
                    _new["MaHaiQuan"] = _dr["MaHaiQuan"];
                    _new["MaKeToan"] = _dr["MaKeToan"];
                    _new["SoGhiDauCay"] = _qty;
                    _new["SLCTView"] = _qty;
                    /*  _new["NW"] = _qty * _tileNW;
                      _new["GW"] = _qty * _tileNW + _tileGW;*/
                    _new["NW"] = NW_Child;
                    _new["GW"] = GW_Child;
                    _new["NWView"] = NW_Child;
                    _new["GWView"] = GW_Child;
                   
                    _new["GhiChu"] = txtGhiChu.Text;
                    _new["IsNPL"] = _dr["IsNPL"];
                    _new["KhoVaiID"] = _dr["KhoVaiID"];
                    _new["SoKienParent"] = _dr["BarCode"];// barcodeGoc
                    _new["SoLuongThucTe"] = _qty;
                    _new["SLTTView"] = _qty;
                    _new["DonGia"] = _dr["DonGia"];
                    _new["ThanhTien"] = _dongia * _qty;
                    _new["ThanhTienView"] = _dongia * _qty;
                    _new["Pallet"] = _dr["Pallet"];
                    _new["MaDVVT"] = _dr["MaDVVT"];
                    _new["MauVTID"] = _dr["MauVTID"];
                    _new["SoKienHienThi"] = _sokienhientai;
                    _new["IsNK"] = _dr["IsNK"];

                    _new["NgayNKDuKien"] = _dr["NgayNKDuKien"];
                    _new["MaDVCD"] = _dr["MaDVCD"];
                    _new["TenDVCD"] = _dr["TenDVCD"];
                    _new["tileNW"] = _dr["tileNW"];
                    _new["tileGW"] = _dr["tileGW"];
                    _new["NgayNhapKho"] = _dr["NgayNhapKho"];
                    _new["BarCodeGoc"] = _dr["BarCodeGoc"];
                    _new["TienTe"] = _dr["TienTe"];
                    _new["MaVTGhep"] = _dr["MaVTGhep"];
                    _new["MaNhom"] = _dr["MaNhom"];
                    _new["QuyDoiID"] = _dr["QuyDoiID"];
                    _new["POMua"] = _dr["POMua"];
                    _new["Batch"] = _dr["Batch"];
                    _new["SLTong"] = _dr["SLTong"];
                    _new["TonKho"] = _qty;
                    _sokien++;
                    tblNhapKho.Rows.Add(_new);
                }
                decimal _slconlai = _qtyGoc - (_qty * _sltach);
                _dr["SLTTView"] = _slconlai;

                //_dr["NWView"] = _slconlai * _tileNW;
                //_dr["GWView"] = _slconlai * _tileNW + _tileGW;

                _dr["NWView"] = Math.Round((_slconlai * _GWGoc) / _sltt, 2); ;
                _dr["GWView"] = Math.Round((_slconlai * _NWGoc) / _sltt, 2);

                _dr["ThanhTienView"] = _dongia * _slconlai;
                decimal _slttconlai = _sltt - (_qty * _sltach);
                _dr["SLCTView"] =  _slttconlai ;


                //_dr["SoGhiDauCay"] = _qtyGoc;
                //_dr["NW"] = _NWGoc;
                //_dr["GW"] = _GWGoc;
                //_dr["ThanhTien"] = _dongia * _qtyGoc;
                //_dr["SoLuongThucTe"] = _sltt;

                tblNhapKho.Rows.Add(_dr);
                this.DialogResult = DialogResult.OK;


            }
            catch (Exception ex)
            {


            }
        }
        //public DataTable getDataTable()
        //{
        //    return tblNhapKho;
        //}
    }

}