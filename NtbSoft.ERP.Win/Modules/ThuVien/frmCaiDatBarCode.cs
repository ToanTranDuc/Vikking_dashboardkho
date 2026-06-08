using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmCaiDatBarCode : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string valueMaHang = "", PO = "";
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        System.Timers.Timer tmrTest = new System.Timers.Timer();
        private DataTable dtData = new DataTable();
        private DataTable tblCopy = new DataTable();
        bool _isCopied = false;
        DataRow drFocus = null;

        bool isAddSeperate = false;
        string cellValueChanging = "";
        bool indicatorIcon = true;

        bool isStartFillBarcode = false;
        bool FlagChecked = false;
        int RowHandel = 0;

        bool isEnterGridView1 = false;
        bool isDeleteGridview1 = false;
        string cellvaluechangingChiTiet = "";
        public frmCaiDatBarCode()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            searchLookUpEdit_MH.EditValueChanged += SearchLookUpEdit_MH_EditValueChanged;
            searchLookUpEdit_PO.EditValueChanged += SearchLookUpEdit_PO_EditValueChanged;
            bandedGridView1.OptionsView.ShowColumnHeaders = false;
            this.gridBand19.Visible = false;
            this.gridBand20.Visible = false;
            this.gridBand21.Visible = false;
            //tmrTest.Interval = 3000;
            //tmrTest.AutoReset = false;
            //tmrTest.Elapsed += TmrTest_Elapsed;
            //tmrTest.Start();
            //timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //this.ActiveControl = txtScanQR;
            //CheckBarCodeReal("");

            //if (isStartFillBarcode)
            //{
            //    timer1.Interval = 50;
            //    //Thread.Sleep(1000);
            //    FlagChecked = true;
            //    isStartFillBarcode = false;
            //    DataTable dt = grcBarCode.DataSource as DataTable;
            //    if (RowHandel >= dt.Rows.Count - 1) RowHandel = -1;
            //    bandedGridView1.FocusedRowHandle = RowHandel + 1;
            //}
            //else if (FlagChecked)
            //{
            //    timer1.Enabled = false;
            //    FlagChecked = false;
            //    DataTable dt = grcBarCode.DataSource as DataTable;

            //    var checkRow = dt.AsEnumerable().Where(x => x["BarcodeChan"].ToString() != "").GroupBy(row => row["BarcodeChan"].ToString())
            //                        .Where(ageGroup => ageGroup.Count() > 1).ToList();
            //    if (checkRow.Count() > 0)
            //    {
            //        bandedGridView1.FocusedRowHandle = RowHandel;
            //        bandedGridView1.SetRowCellValue(RowHandel, "BarCodeChan", "");
            //        MessageBox.Show("Barcode đã trùng vui lòng kiểm tra lại!");
            //    }

            //    var checkRowLe = dt.AsEnumerable().Where(x => x["BarcodeLe"].ToString() != "").GroupBy(row => row["BarcodeLe"].ToString())
            //                       .Where(ageGroup => ageGroup.Count() > 1).ToList();
            //    if (checkRowLe.Count() > 0)
            //    {
            //        bandedGridView1.FocusedRowHandle = RowHandel;
            //        bandedGridView1.SetRowCellValue(RowHandel, "BarCodeLe", "");
            //        MessageBox.Show("Barcode đã trùng vui lòng kiểm tra lại!");
            //    }

            //}
        }
        private void CheckBarCodeReal(string barcode)
        {
            barcode = "8935217400157";

            // Bước 1: Tính tổng lẻ và tổng chẵn
            int oddSum = 0;
            int evenSum = 0;

            for (int i = 0; i < barcode.Length; i++)
            {
                int digit = int.Parse(barcode[i].ToString());
                if (i % 2 == 0) // Vị trí lẻ (0-based index)
                {
                    oddSum += digit;
                }
                else // Vị trí chẵn
                {
                    evenSum += digit;
                }
            }

            // Bước 2: Tính tổng có trọng số
            int weightedSum = 3 * oddSum + evenSum;

            // Bước 3: Tính checksum
            int checksum = (10 - (weightedSum % 10)) % 10;

            // In kết quả
            Console.WriteLine("Checksum: " + checksum);

        }
        private void grvBarCode_ShowingEditor(object sender, CancelEventArgs e)
        {

            if (bandedGridView1.FocusedRowHandle < 0) return;
            drFocus = bandedGridView1.GetFocusedDataRow();

            string _fieldName = bandedGridView1.FocusedColumn.FieldName;
            string _rowCellValue = "";

            List<String> lstColumn = new List<String> { "BarCodeChan", "BarCodeLe", "BarCodeXacNhan", "BarCodeBao", "BarCodeTheBai", "Ecode" };
            if (lstColumn.Contains(_fieldName))
            {
                switch (_fieldName)
                {
                    case "BarCodeChan":
                        _rowCellValue = "SoLuongChan";
                        break;
                    case "BarCodeLe":
                        _rowCellValue = "SoLuongLe";
                        break;
                    case "BarCodeXacNhan":
                        _rowCellValue = "SoLuongXacNhan";
                        break;
                    case "BarCodeBao":
                        _rowCellValue = "SoLuongBao";
                        break;
                    case "BarCodeTheBai":
                        _rowCellValue = "SoLuongTheBai";
                        break;
                    case "Ecode":
                        _rowCellValue = "SoLuongEcode";
                        break;
                }
                string _barcode = bandedGridView1.GetFocusedRowCellValue(_fieldName)?.ToString();
                if (_barcode != null && !string.IsNullOrEmpty(_barcode))
                {
                    isAddSeperate = true;
                }
                else
                {
                    isAddSeperate = false;
                }
                //if (bandedGridView1.FocusedColumn.FieldName == "BarCodeChan")
                //{
                //    txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue("SoLuongChan").ToString();
                //}
                //else if (bandedGridView1.FocusedColumn.FieldName == "BarCodeLe")
                //{
                //    txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue("SoLuongLe").ToString();
                //}
                txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue(_rowCellValue).ToString();
                txtDauSize.Text = drFocus["DauSize"].ToString();
                txtMau.Text = drFocus["TenMau"].ToString();
                txtSize.Text = drFocus["Size"].ToString();
                this.ActiveControl = txtScanQR;
            }
        }

        private void grvBarCode_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            bool isCheckReplace = true;
            if (e.RowHandle < 0)
                return;
            cellValueChanging = e.Value?.ToString();
            if (!isStartFillBarcode)
            {
                //timer1.Interval = 500;
                //timer1.Enabled = true;
                isStartFillBarcode = true;
            }
            RowHandel = e.RowHandle;

            // Tai - Thêm code xử lý Quét nhiều barcode
            int length = e.Value.ToString().Length;
            string _fieldName = bandedGridView1.FocusedColumn.FieldName;
            string _rowCellValue = "";
            List<String> lstColumn = new List<String> { "BarCodeXacNhan", "BarCodeBao", "BarCodeTheBai", "Ecode" };
            switch (_fieldName)
            {
                case "BarCodeXacNhan":
                    _rowCellValue = "SoLuongXacNhan";
                    break;
                case "BarCodeBao":
                    _rowCellValue = "SoLuongBao";
                    break;
                case "BarCodeTheBai":
                    _rowCellValue = "SoLuongTheBai";
                    break;
                case "Ecode":
                    _rowCellValue = "SoLuongEcode";
                    break;
            }
            int _soLuongXacNhan = int.Parse(bandedGridView1.GetFocusedRowCellValue("SoLuongXacNhan").ToString());
            if (lstColumn.Contains(_fieldName) && checkEdit1.Checked)
            // Trường hợp Quét cho Barcode Xác Nhận hoặc( Barcode Bao || Barcode Thẻ Bài|| Ecode và số lượng barcode Xác Nhận lớn )
            //if ((_fieldName == "BarCodeXacNhan" || (_fieldName != "BarCodeXacNhan" && _soLuongXacNhan > 1)) && checkEdit1.Checked)
            {
                // check nếu cuối chuỗi là kí tự ',' thì xóa kí tự ','
                if (length > 0)
                {
                    string lastChar = e.Value?.ToString().Substring(length - 1, 1);
                    if (lastChar == ",")
                    {
                        isAddSeperate = true;
                        customSetRowCellValue(bandedGridView1.FocusedRowHandle, bandedGridView1.FocusedColumn.FieldName, e.Value.ToString().Substring(0, length - 1));
                        string _barcode = bandedGridView1.GetFocusedRowCellValue(_fieldName)?.ToString();
                        if (_fieldName == "BarCodeXacNhan" || (_fieldName != "BarCodeXacNhan" && _soLuongXacNhan > 1))
                        {
                            int _soLuong = _barcode.Split(',').Length;
                            bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, _soLuong);
                            txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue(_rowCellValue).ToString();
                        }
                        //else
                        //{
                        //    int _soLuong = int.Parse(bandedGridView1.GetFocusedRowCellValue(_rowCellValue)?.ToString());
                        //    if (_soLuong <1)
                        //    {
                        //        bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, 1);
                        //        txtSoLuong.Text = "1";
                        //    }
                        //}
                        mapDataChiTiet(false);
                        return;
                    }
                }
                if (isAddSeperate && length > 0)
                {

                    Console.WriteLine("grvBarCode_CellValueChanging");
                    string _value = e.Value?.ToString().Substring(0, length - 1);
                    string _valueTemp = e.Value?.ToString().Substring(length - 1, 1);

                    if (!string.IsNullOrEmpty(_value))
                    {
                        customSetRowCellValue(bandedGridView1.FocusedRowHandle, bandedGridView1.FocusedColumn.FieldName, string.Concat(_value, ",", _valueTemp));
                        isCheckReplace = false;
                    }

                    isAddSeperate = false;
                }
                mapDataChiTiet(isCheckReplace);
                string barcode = isCheckReplace ? e.Value?.ToString() : bandedGridView1.GetFocusedRowCellValue(_fieldName).ToString();
                if (_fieldName == "BarCodeXacNhan" || (_fieldName != "BarCodeXacNhan" && _soLuongXacNhan > 1))
                {
                    int soLuong = barcode.Split(',').Length;
                    if (string.IsNullOrEmpty(barcode))
                    {
                        soLuong = 0;
                    }
                    bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, soLuong);
                    txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue(_rowCellValue).ToString();
                }
                //else
                //{
                //    int _soLuong = int.Parse(bandedGridView1.GetFocusedRowCellValue(_rowCellValue)?.ToString());
                //    if (_soLuong < 1)
                //    {
                //        bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, 1);
                //        txtSoLuong.Text = "1";
                //    }
                //}

            }
            else
            {
                mapDataChiTiet(true);
            }

        }

        // hàm này sử dụng riêng cho bandedGridView1
        // Dùng để cập nhật giá trị cho các cell nhập chuỗi và đưa focused đến cuôi chuỗi
        private void customSetRowCellValue(int rowHandle, string fieldName, dynamic value)
        {

            bandedGridView1.SetRowCellValue(rowHandle, fieldName, value);
            // Lấy control editor
            BaseEdit editor = bandedGridView1.ActiveEditor;
            // Đặt vị trí con trỏ đến cuối chuỗi
            if (editor is TextEdit textEdit)
            {
                textEdit.SelectionStart = textEdit.Text.Length;
            }
        }

        private void customSetRowCellValueChiTiet(int rowHandle, string fieldName, dynamic value)
        {

            gridView1.SetRowCellValue(rowHandle, fieldName, value);
            // Lấy control editor
            BaseEdit editor = gridView1.ActiveEditor;
            // Đặt vị trí con trỏ đến cuối chuỗi
            if (editor is TextEdit textEdit)
            {
                textEdit.SelectionStart = textEdit.Text.Length;
            }
        }
        private void grcBarCode_ProcessGridKey(object sender, KeyEventArgs e)
        {
            string _fieldName = bandedGridView1.FocusedColumn.FieldName;
            if (e.Control && e.KeyCode == Keys.V)
            {
                var lstAccept = new List<string>() { "BarCodeChan", "BarCodeLe", "BarCodeXacNhan", "BarCodeBao", "BarCodeTheBai", "ECode" };
                if (!lstAccept.Contains(_fieldName)) return;
                DataTable dt = grcBarCode.DataSource as DataTable;
                KHDongThungLib.CopyPasteFor1Col(bandedGridView1, dt.Rows.Count, _fieldName);

            }
            if (e.KeyCode == Keys.Back)
            {
                isAddSeperate = false;
            }
            if (e.KeyCode == Keys.Enter && isStartFillBarcode && bandedGridView1.FocusedRowHandle >= 0)
            {
                List<String> lstColumn = new List<String> { "BarCodeXacNhan", "BarCodeBao", "BarCodeTheBai", "Ecode" };
                if (this.checkEdit1.Checked && lstColumn.Contains(_fieldName))
                {
                    string _rowCellValue = "";
                    switch (_fieldName)
                    {
                        case "BarCodeXacNhan":
                            _rowCellValue = "SoLuongXacNhan";
                            break;
                        case "BarCodeBao":
                            _rowCellValue = "SoLuongBao";
                            break;
                        case "BarCodeTheBai":
                            _rowCellValue = "SoLuongTheBai";
                            break;
                        case "Ecode":
                            _rowCellValue = "SoLuongEcode";
                            break;
                    }

                    e.Handled = true;
                    isAddSeperate = true;

                    int _soLuongXacNhan = int.Parse(bandedGridView1.GetFocusedRowCellValue("SoLuongXacNhan").ToString());
                    //checkTrung item in list
                    List<string> lstItem = cellValueChanging.Split(',').ToList();
                    List<string> lstCheckTrungInList = CheckTrungItemInList(lstItem);

                    // Chỉ trường hợp quét nhiều barcode khác nhau mới
                    // kiểm tra chuỗi trong barcode có bị trùng lại không
                    if (_fieldName == "BarCodeXacNhan" || (_fieldName != "BarCodeXacNhan" && _soLuongXacNhan > 1))
                    {
                        // Trường hợp quét nhiều barcode khác nhau
                        if (lstCheckTrungInList.Count > 0)
                        {
                            MessageBox.Show(string.Format("Barcode: {0} đã trùng vui lòng kiểm tra lại!", lstCheckTrungInList[0]));
                            customSetRowCellValue(RowHandel, _fieldName, RemoveLastItem(lstItem));
                            cellValueChanging = string.Join(",", lstItem);
                        }
                    }
                    else
                    {
                        // Trường hợp quét nhiều lần cho 1 barcode
                        if (lstCheckTrungInList.Count > 0)
                        {
                            //MessageBox.Show(string.Format("Barcode: {0} đã trùng vui lòng kiểm tra lại!", lstCheckTrungInList[0]));
                            customSetRowCellValue(RowHandel, _fieldName, RemoveLastItem(lstItem));
                            cellValueChanging = string.Join(",", lstItem);
                        }
                    }

                    // Biến isValid để check có tăng số lượng hay không
                    // Chỉ dành riêng cho trường hợp quét 1 barcode lặp lại nhiều lần
                    bool isValid = true;

                    // Nếu  là Barcode Bao hoặc Barcode Thẻ Bài hoặc E-Code
                    // Check chuỗi Barcode có tồn tại trong Barcode Xác Nhận không
                    switch (_fieldName)
                    {
                        case "BarCodeBao":
                        case "BarCodeTheBai":
                        case "Ecode":
                            string _lstBarcodeXacNhan = bandedGridView1.GetFocusedRowCellValue("BarCodeXacNhan")?.ToString();
                            if (_lstBarcodeXacNhan != null && !string.IsNullOrEmpty(_lstBarcodeXacNhan))
                            {
                                if (!CheckItemExist(lstItem, _lstBarcodeXacNhan))
                                {
                                    isValid = false;
                                    MessageBox.Show(string.Format("Barcode: {0} chưa được khai báo trong Barcode Xác Nhận.!", lstItem[lstItem.Count - 1]));
                                    customSetRowCellValue(RowHandel, _fieldName, RemoveLastItem(lstItem));
                                    cellValueChanging = string.Join(",", lstItem);
                                }
                            }
                            else
                            {
                                isValid = false;
                                MessageBox.Show("Barcode Xác Nhận chưa được khai báo. Vui lòng khai báo Barcode Xác Nhận trước!");
                                customSetRowCellValue(RowHandel, _fieldName, RemoveLastItem(lstItem));
                                cellValueChanging = string.Join(",", lstItem);
                            }
                            break;
                    }

                    // Set số lượng
                    if (_fieldName == "BarCodeXacNhan" || (_fieldName != "BarCodeXacNhan" && _soLuongXacNhan > 1))
                    {

                        int soLuong = cellValueChanging.Split(',').Length;
                        if (string.IsNullOrEmpty(cellValueChanging))
                        {
                            soLuong = 0;
                        }
                        bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, soLuong);
                        txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue(_rowCellValue).ToString();
                    }
                    else
                    {
                        if (isValid)
                        {
                            int soLuong = 0;
                            if (int.TryParse(bandedGridView1.GetFocusedRowCellValue(_rowCellValue)?.ToString(), out soLuong))
                            {
                                soLuong += 1;
                                bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, soLuong);
                                txtSoLuong.Text = soLuong.ToString();
                            }
                        }
                    }
                }
                else
                {
                    DataTable dt = grcBarCode.DataSource as DataTable;

                    if (RowHandel >= dt.Rows.Count - 1) RowHandel = -1;
                    bandedGridView1.FocusedRowHandle = RowHandel + 1;

                    CheckTrung();
                    isStartFillBarcode = false;
                    string _rowCellValue = "";

                    /// _rowCellValueSoLuongTemp mang giá trị là "SoLuongChan" hoặc "SoLuongLe"
                    /// Nếu _rowCellValue là "SoLuongChan" -> _rowCellValueSoLuongTemp là "SoLuongLe"
                    /// Nếu _rowCellValue là "SoLuongLe" -> _rowCellValueSoLuongTemp là "SoLuongChan"
                    /// Được dùng khi người dùng muốn khai báo chung cho barcode thùng chẵn và barcode thùng lẻ
                    /// 
                    /// _rowCellValueBarcodeTemp mang giá trị là "BarCodeChan" hoặc "BarCodeLe"
                    /// Nếu _rowCellValue là "BarCodeChan" -> _rowCellValueBarcodeTemp là "BarCodeLe"
                    /// Nếu _rowCellValue là "BarCodeLe" -> _rowCellValueBarcodeTemp là "BarCodeChan"
                    /// Được dùng khi người dùng muốn khai báo chung cho barcode thùng chẵn và barcode thùng lẻ
                    /// 
                    string _rowCellValueBarcodeTemp = "";
                    string _rowCellValueSoLuongTemp = "";
                    switch (_fieldName)
                    {
                        case "BarCodeChan":
                            _rowCellValue = "SoLuongChan";
                            _rowCellValueBarcodeTemp = "BarCodeLe";
                            _rowCellValueSoLuongTemp = "SoLuongLe";
                            break;
                        case "BarCodeLe":
                            _rowCellValue = "SoLuongLe";
                            _rowCellValueBarcodeTemp = "BarCodeChan";
                            _rowCellValueSoLuongTemp = "SoLuongChan";
                            break;
                        //case "BarCodeXacNhan":
                        //    _rowCellValue = "SoLuongXacNhan";
                        //    break;
                        case "BarCodeBao":
                            _rowCellValue = "SoLuongBao";
                            break;
                        case "BarCodeTheBai":
                            _rowCellValue = "SoLuongTheBai";
                            break;
                        case "Ecode":
                            _rowCellValue = "SoLuongEcode";
                            break;
                    }
                    bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, 0);
                    txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue(_rowCellValue).ToString();

                    if (ckcEditScan.Checked && !string.IsNullOrEmpty(_rowCellValueBarcodeTemp))
                    {
                        string barcodeTemp = (grcBarCode.DataSource as DataTable).Rows[RowHandel][_fieldName].ToString();
                        bandedGridView1.SetRowCellValue(RowHandel, _rowCellValueSoLuongTemp, 0);
                        bandedGridView1.SetRowCellValue(RowHandel, _rowCellValueBarcodeTemp, barcodeTemp);
                    }
                }
                mapDataChiTiet(true);
            }

        }
        private List<string> CheckTrungItemInList(List<string> lstItem)
        {

            List<string> hasDuplicates = lstItem.GroupBy(x => x).Where(g => g.Count() > 1)
                       .Select(g => g.Key).ToList();

            return hasDuplicates;
        }

        private bool CheckItemExist(List<string> lstItem, string strCheck)
        {

            List<string> _lstStrCheck = strCheck.Split(',').ToList();
            bool hasAllExist = lstItem.All(item => _lstStrCheck.Contains(item));
            return hasAllExist;
        }

        private string RemoveLastItem(List<string> lstItem)
        {
            lstItem.RemoveAt(lstItem.Count - 1);
            return string.Join(",", lstItem);
        }
        private void txtScanQR_KeyPress(object sender, KeyPressEventArgs e)
        {
            string _fieldName = bandedGridView1.FocusedColumn.FieldName;
            List<String> lstColumn = new List<String> { "BarCodeChan", "BarCodeLe", "BarCodeBao", "BarCodeTheBai", "Ecode" };
            if (e.KeyChar == (char)Keys.Enter && lstColumn.Contains(_fieldName))
            {
                RowHandel = bandedGridView1.FocusedRowHandle;
                drFocus = bandedGridView1.GetFocusedDataRow();

                /// _rowCellValueBarcodeTemp mang giá trị là "BarCodeChan" hoặc "BarCodeLe"
                /// Nếu _rowCellValue là "BarCodeChan" -> _rowCellValueBarcodeTemp là "BarCodeLe"
                /// Nếu _rowCellValue là "BarCodeLe" -> _rowCellValueBarcodeTemp là "BarCodeChan"
                /// Được dùng khi người dùng muốn khai báo chung cho barcode thùng chẵn và barcode thùng lẻ
                /// 
                string _rowCellValueBarcodeTemp = "";
                string _rowCellValue = "";

                switch (_fieldName)
                {
                    case "BarCodeChan":
                        _rowCellValue = "SoLuongChan";
                        _rowCellValueBarcodeTemp = "BarCodeLe";
                        break;
                    case "BarCodeLe":
                        _rowCellValue = "SoLuongLe";
                        _rowCellValueBarcodeTemp = "BarCodeChan";
                        break;
                    case "BarCodeBao":
                        _rowCellValue = "SoLuongBao";
                        break;
                    case "BarCodeTheBai":
                        _rowCellValue = "SoLuongTheBai";
                        break;
                    case "Ecode":
                        _rowCellValue = "SoLuongEcode";
                        break;
                }
                if (drFocus[_fieldName].ToString() == "")
                {
                    // Tạo barcode
                    drFocus[_fieldName] = txtScanQR.Text;
                    if (ckcEditScan.Checked && !string.IsNullOrEmpty(_rowCellValueBarcodeTemp))
                    {
                        string barcodeTemp = (grcBarCode.DataSource as DataTable).Rows[RowHandel][_fieldName].ToString();
                        bandedGridView1.SetRowCellValue(RowHandel, _rowCellValueBarcodeTemp, barcodeTemp);
                    }
                }
                if (drFocus[_fieldName].ToString() != txtScanQR.Text)
                {
                    MessageBox.Show("Barcode không trùng với hàng, cột đang chọn!");
                    txtScanQR.Text = "";
                    txtScanQR.Refresh();
                    return;
                }
                if (string.IsNullOrEmpty(_rowCellValue))
                    return;
                int SoLuong = Convert.ToInt16(bandedGridView1.GetFocusedRowCellValue(_rowCellValue)) + 1;
                bandedGridView1.SetRowCellValue(RowHandel, _rowCellValue, SoLuong);
                txtSoLuong.Text = SoLuong.ToString();

                //if (bandedGridView1.FocusedColumn.FieldName == "BarCodeChan")
                //{
                //    if (drFocus["BarCodeChan"].ToString() == "") drFocus["BarCodeChan"] = txtScanQR.Text;
                //    if (drFocus["BarCodeChan"].ToString() != txtScanQR.Text)
                //    {
                //        MessageBox.Show("Barcode không trùng với hàng, cột đang chọn!");
                //        txtScanQR.Text = "";
                //        txtScanQR.Refresh();
                //        return;
                //    }
                //    int SoLuong = Convert.ToInt16(bandedGridView1.GetFocusedRowCellValue("SoLuongChan")) + 1;
                //    bandedGridView1.SetRowCellValue(RowHandel, "SoLuongChan", SoLuong);
                //    txtSoLuong.Text = SoLuong.ToString();
                //    //bandedGridView1.SetRowCellValue(RowHandel, "BarCodeChan", "");
                //}
                //else if (bandedGridView1.FocusedColumn.FieldName == "BarCodeLe")
                //{
                //    if (drFocus["BarCodeLe"].ToString() == "") drFocus["BarCodeLe"] = txtScanQR.Text;

                //    if (drFocus["BarCodeLe"].ToString() != txtScanQR.Text)
                //    {
                //        MessageBox.Show("Barcode không trùng với hàng, cột đang chọn!");
                //        txtScanQR.Text = "";
                //        txtScanQR.Refresh();
                //        return;
                //    }
                //    int SoLuong = Convert.ToInt16(bandedGridView1.GetFocusedRowCellValue("SoLuongLe")) + 1;
                //    bandedGridView1.SetRowCellValue(RowHandel, "SoLuongLe", SoLuong);
                //    txtSoLuong.Text = SoLuong.ToString();
                //}
                bandedGridView1.RefreshData();
                txtScanQR.Text = "";
                txtScanQR.Refresh();
            }
        }
        private void CheckTrung()
        {
            DataTable dt = grcBarCode.DataSource as DataTable;

            string _fieldName = bandedGridView1.FocusedColumn.FieldName;

            var checkRow = dt.AsEnumerable().Where(x => x[_fieldName].ToString() != "").GroupBy(row => row[_fieldName].ToString())
                                .Where(ageGroup => ageGroup.Count() > 1).ToList();
            if (checkRow.Count() > 0)
            {
                bandedGridView1.FocusedRowHandle = RowHandel;
                bandedGridView1.SetRowCellValue(RowHandel, _fieldName, "");
                MessageBox.Show("Barcode đã trùng vui lòng kiểm tra lại!");
                //clsWaitForm.ShowWaningFormCustomV3(this, 1000, "");
            }

            //var checkRowLe = dt.AsEnumerable().Where(x => x["BarcodeLe"].ToString() != "").GroupBy(row => row["BarcodeLe"].ToString())
            //                   .Where(ageGroup => ageGroup.Count() > 1).ToList();
            //if (checkRowLe.Count() > 0)
            //{
            //    bandedGridView1.FocusedRowHandle = RowHandel;
            //    bandedGridView1.SetRowCellValue(RowHandel, "BarCodeLe", "");
            //    MessageBox.Show("Barcode đã trùng vui lòng kiểm tra lại!");
            //    //clsWaitForm.ShowWaningFormCustomV3(this, 1000, "");
            //}
        }
        private void SearchLookUpEdit_MH_EditValueChanged(object sender, EventArgs e)
        {
            var value = searchLookUpEdit_MH.EditValue;
            if (value is null || value.ToString() == "") return;
            valueMaHang = value.ToString();
            LoadPO();
        }
        private void SearchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {
            var value = searchLookUpEdit_PO.EditValue;
            if (value is null || value.ToString() == "") return;
            PO = value.ToString();
            LoadData();
        }

        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            Init();
            LoadMaHang();
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
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                //Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }
        private void Init()
        {
            searchLookUpEdit_MH.Properties.DisplayMember = "valueDisplay";
            searchLookUpEdit_MH.Properties.ValueMember = "value";
            searchLookUpEdit_MH.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn giá trị]";
        }
        private void LoadMaHang()
        {
            string url = string.Format("{0}", URL + "CaiDatBarCode/Get?action=GetMaHang&Para1=A&Para2=A&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MH.Properties.DataSource = dt;


        }
        private void LoadPO()
        {
            string url = string.Format("{0}", URL + $"CaiDatBarCode/Get?action=GetPO&Para1={valueMaHang}&Para2=A&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_PO.Properties.DataSource = dt;
            if (dt.Rows.Count > 0)
            {
                searchLookUpEdit_PO.EditValue = null;
                searchLookUpEdit_PO.EditValue = dt.Rows[0]["POID"];
            }
        }
        private void LoadData()
        {
            string url = string.Format("{0}", URL + $"CaiDatBarCode/GetNew?Para1={valueMaHang}&Para2={PO}&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtData = JsonConvert.DeserializeObject<DataTable>(json);
            int rowfocus = bandedGridView1.FocusedRowHandle;
            grcBarCode.DataSource = dtData;
            bandedGridView1.FocusedRowHandle = rowfocus;
        }
        private void SaveData(DataTable dtSave)
        {
            string url = string.Format("{0}?", URL + "CaiDatBarCode/PostNew?action=Post&Para1=A");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToLower() == "true")
            {
                LoadData();
                clsWaitForm.ShowSuccessForm(this, 1000);

            }
        }


        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportExcel();
        }

        private void ImportExcel()
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";
                openFileDialog1.Title = "Import Excel";
                openFileDialog1.Multiselect = false;

                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult != DialogResult.OK) return;
                string pathExecel = openFileDialog1.FileName;
                if (string.IsNullOrEmpty(pathExecel)) return;
                string conString = "";
                string extension = System.IO.Path.GetExtension(pathExecel);
                switch (extension)
                {
                    case ".xls": //Excel 97-03
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString, pathExecel);
                        //conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                    case ".xlsx": //Excel 07 or higher
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                }
                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    if (excel_con.State == ConnectionState.Closed)
                    {
                        excel_con.Open();
                    }
                    //OleDbCommand _oleCmdSelect;
                    OleDbDataAdapter oleAdapter = new OleDbDataAdapter(); ;
                    DataTable sheets = GetSchemaTable(conString);


                    //OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                    //                             @"SELECT * FROM [Sheet1$] ", excel_con);
                    OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                                              @"SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString() + "] ", excel_con);
                    DataSet excelDataSet = new DataSet();
                    _oleCmdSelect.Fill(excelDataSet);
                    DataTable dt = new DataTable();
                    dt = excelDataSet.Tables[0];
                    DataTable dtSave = KHDongThungLib.CreateTblCaiDatBarcodeNew();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][1].ToString() == "") break;
                        var drnew = dtSave.NewRow();
                        drnew["ID"] = 0;
                        drnew["StyleID"] = GetNameID("TenHang", "MaHang", dt.Rows[i][0].ToString());
                        drnew["Season"] = GetNameID("Season", "SeasonID", dt.Rows[i][1].ToString());
                        drnew["POID"] = GetNameID("PO", "POID", dt.Rows[i][2].ToString());
                        drnew["DauSizeID"] = GetNameID("DauSize", "DauSizeID", dt.Rows[i][3].ToString());
                        drnew["MaMau"] = GetNameID("TenMau", "MaMau", dt.Rows[i][4].ToString());
                        drnew["SizeID"] = GetNameID("Size", "SizeID", dt.Rows[i][5].ToString());
                        drnew["BarCodeChan"] = dt.Rows[i][6];
                        drnew["SoLuongChan"] = dt.Rows[i][7];
                        drnew["BarCodeLe"] = dt.Rows[i][8];
                        drnew["SoLuongLe"] = dt.Rows[i][9];
                        drnew["CreateDate"] = DateTime.Now.ToString("yyyy-MM-dd");
                        dtSave.Rows.Add(drnew);
                    }
                    SaveData(dtSave);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private string GetNameID(string colName, string colNameID, string value)
        {
            var valueID = dtData.AsEnumerable().Where(x => x[colName].ToString() == value)?.FirstOrDefault()?[colNameID].ToString();
            return valueID;
        }
        private DataTable GetSchemaTable(string connectionString)
        {
            using (OleDbConnection connection = new
                       OleDbConnection(connectionString))
            {
                connection.Open();
                DataTable schemaTable = connection.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                return schemaTable;
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = grcBarCode.DataSource as DataTable;
            var drNew = dt.NewRow();
            var drLastRow = dt.Rows[dt.Rows.Count - 1];
            drNew["StyleID"] = drLastRow["StyleID"];
            drNew["MaHang"] = drLastRow["MaHang"];
            drNew["MaMau"] = drLastRow["MaMau"];
            drNew["TenMau"] = drLastRow["TenMau"];
            dt.Rows.InsertAt(drNew, 0);

        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }
        private void Save()
        {
            this.ActiveControl = searchLookUpEdit_MH;
            var dtSave = grcBarCode.DataSource as DataTable;
            var checkRowA = dtSave.AsEnumerable().Where(x => x["BarCodeChan"].ToString() != "").GroupBy(row => row["BarCodeChan"].ToString())
                            .Where(ageGroup => ageGroup.Count() > 1).ToList();
            if (checkRowA.Count > 0)
            {
                string message = "";
                foreach (var item in checkRowA)
                {
                    message += item.Key.Trim() + Environment.NewLine;
                }
                MessageBox.Show($"Đã tồn tại barcode trùng cột Barcode chẵn. Vui lòng kiểm tra lại! \r\n{message}");
                return;
            }
            var checkRowB = dtSave.AsEnumerable().Where(x => x["BarCodeLe"].ToString() != "").GroupBy(row => row["BarCodeLe"].ToString())
                          .Where(ageGroup => ageGroup.Count() > 1).ToList();
            if (checkRowB.Count > 0)
            {
                string message = "";
                foreach (var item in checkRowB)
                {
                    message += item.Key.Trim() + Environment.NewLine;
                }
                MessageBox.Show($"Đã tồn tại barcode trùng cột Barcode lẽ. Vui lòng kiểm tra lại! \r\n{message}");
                return;
            }

            var tblSave = KHDongThungLib.CreateTblCaiDatBarcodeNew();
            foreach (DataRow dr in dtSave.Rows)
            {
                var drSave = tblSave.NewRow();
                drSave["ID"] = 0;
                drSave["StyleID"] = dr["MaHang"];
                drSave["Season"] = dr["Season"];
                drSave["POID"] = dr["POID"];
                drSave["DauSizeID"] = dr["DauSizeID"];
                drSave["MaMau"] = dr["ColorID"];
                drSave["SizeID"] = dr["SizeID"];
                drSave["BarCodeChan"] = dr["BarCodeChan"];
                drSave["SoLuongChan"] = (!string.IsNullOrEmpty(dr["SoLuongChan"].ToString()) ? dr["SoLuongChan"] : 0);
                drSave["BarCodeLe"] = dr["BarCodeLe"];
                drSave["SoLuongLe"] = (!string.IsNullOrEmpty(dr["SoLuongLe"].ToString()) ? dr["SoLuongLe"] : 0);
                drSave["BarCodeBao"] = dr["BarCodeBao"];
                drSave["SoLuongBao"] = (!string.IsNullOrEmpty(dr["SoLuongBao"].ToString()) ? dr["SoLuongBao"] : 0);
                drSave["BarCodeTheBai"] = dr["BarCodeTheBai"];
                drSave["SoLuongTheBai"] = (!string.IsNullOrEmpty(dr["SoLuongTheBai"].ToString()) ? dr["SoLuongTheBai"] : 0);
                drSave["Ecode"] = dr["Ecode"];
                drSave["SoLuongEcode"] = (!string.IsNullOrEmpty(dr["SoLuongEcode"].ToString()) ? dr["SoLuongEcode"] : 0);
                drSave["BarCodeXacNhan"] = dr["BarCodeXacNhan"];
                drSave["SoLuongXacNhan"] = (!string.IsNullOrEmpty(dr["SoLuongXacNhan"].ToString()) ? dr["SoLuongXacNhan"] : 0);
                drSave["CreateDate"] = DateTime.Now.ToString("yyyy-MM-dd");
                tblSave.Rows.Add(drSave);
            }
            SaveData(tblSave);
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void grvBarCode_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();

            DXMenuItem Copy = new DXMenuItem();
            Copy.Caption = "Sao chép";
            // Copy.Image = imageCollection1.Images[2];
            Copy.Click += Copy_Click; ; ;

            e.Menu.Items.Add(Copy);

            if (_isCopied == true)
            {
                DXMenuItem Paste = new DXMenuItem();
                Paste.Caption = "Dán";
                //Paste.Image = imageCollection1.Images[3];
                Paste.Click += Paste_Click;
                e.Menu.Items.Add(Paste);
            }

        }
        private void Copy_Click(object sender, EventArgs e)
        {
            _isCopied = true;
            tblCopy = grcBarCode.DataSource as DataTable;
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            _isCopied = false;
            var tblSource = grcBarCode.DataSource as DataTable;
            foreach (DataRow dr in tblSource.Rows)
            {
                var drCheck = tblCopy.AsEnumerable().Where(x => x["DauSizeID"].ToString() == dr["DauSizeID"].ToString() &&
                                                              x["ColorID"].ToString() == dr["ColorID"].ToString() &&
                                                              x["SizeID"].ToString() == dr["SizeID"].ToString()).FirstOrDefault();
                if (drCheck is null) continue;
                dr["BarcodeChan"] = drCheck["BarcodeChan"].ToString() == "" ? dr["BarcodeChan"] : drCheck["BarcodeChan"].ToString();
                dr["BarcodeLe"] = drCheck["BarcodeLe"].ToString() == "" ? dr["BarcodeLe"] : drCheck["BarcodeLe"].ToString();
            }
            grcBarCode.DataSource = tblSource;
            bandedGridView1.RefreshData();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string url = string.Format("{0}?", URL + "CaiDatBarCode/PostNew?action=Delete&Para1=A");
                var dtSave = grcBarCode.DataSource as DataTable;
                if (dtSave is null || dtSave.Rows.Count == 0) return;

                var tblSave = KHDongThungLib.CreateTblCaiDatBarcodeNew();
                var dr = dtSave.Rows[0];
                var drSave = tblSave.NewRow();
                drSave["ID"] = 0;
                drSave["StyleID"] = dr["MaHang"];
                drSave["Season"] = dr["Season"];
                drSave["POID"] = dr["POID"];
                drSave["DauSizeID"] = dr["DauSizeID"];
                drSave["MaMau"] = dr["ColorID"];
                drSave["SizeID"] = dr["SizeID"];
                drSave["BarCodeChan"] = dr["BarCodeChan"];
                drSave["SoLuongChan"] = dr["SoLuongChan"];
                drSave["BarCodeLe"] = dr["BarCodeLe"];
                drSave["SoLuongLe"] = dr["SoLuongLe"] ?? 0;
                drSave["CreateDate"] = DateTime.Now.ToString("yyyy-MM-dd");
                tblSave.Rows.Add(drSave);

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
                if (msResult.ToLower() == "true")
                {
                    LoadData();
                    clsWaitForm.ShowSuccessForm(this, 1000);
                }
            }

        }

        private void btnScan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = layoutControl1;
            this.ActiveControl = txtScanQR;
        }

        private void checkEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("checkEdit1_EditValueChanged");
            ChangingEventArgs changing = e as ChangingEventArgs;
            if (bool.Parse(changing.NewValue.ToString()))
            {
                //txtScanQR.Enabled = false;
                this.gridBand19.Visible = true;
                this.gridBand20.Visible = true;
                this.gridBand21.Visible = true;
            }
            else
            {
                //txtScanQR.Enabled = true;
                this.gridBand19.Visible = false;
                this.gridBand20.Visible = false;
                this.gridBand21.Visible = false;
            }
        }

        private void bandedGridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            cellValueChanging = "";
            Console.WriteLine("bandedGridView1_FocusedRowChanged");
            mapDataChiTiet();
        }

        private void bandedGridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            cellValueChanging = "";
            Console.WriteLine("bandedGridView1_FocusedColumnChanged");
            mapDataChiTiet();
        }

        private void mapDataChiTiet(bool isFromEnter = false)
        {
            // Field name View Chi Tiết
            string _columnChiTietDauSize = "DauSize";
            string _columnChiTietMau = "Mau";
            string _columnChiTietSize = "Size";
            string _columnChiTietCode = "Code";

            string _fieldName = bandedGridView1.FocusedColumn.FieldName;

            switch (_fieldName)
            {
                case "SoLuongChan":
                    _fieldName = "BarCodeChan";
                    break;
                case "SoLuongLe":
                    _fieldName = "BarCodeLe";
                    break;
                case "SoLuongXacNhan":
                    _fieldName = "BarCodeXacNhan";
                    break;
                case "SoLuongBao":
                    _fieldName = "BarCodeBao";
                    break;
                case "SoLuongTheBai":
                    _fieldName = "BarCodeTheBai";
                    break;
                case "SoLuongEcode":
                    _fieldName = "Ecode";
                    break;
            }

            List<String> lstColumn = new List<String> { "BarCodeChan", "BarCodeLe", "BarCodeXacNhan", "BarCodeBao", "BarCodeTheBai", "Ecode" };
            int rowHandle = bandedGridView1.FocusedRowHandle;
            DataTable tbl = createDataTableChiTiet();
            DataRow _focusedRow = bandedGridView1.GetFocusedDataRow();
            string barcode = isFromEnter ? cellValueChanging : bandedGridView1.GetFocusedRowCellValue(_fieldName)?.ToString();
            if (barcode != null && lstColumn.Contains(_fieldName) && rowHandle >= 0)
            {
                List<string> lstStrBarcode = barcode.Split(',').ToList();
                foreach (string item in lstStrBarcode)
                {
                    DataRow _dtRow = tbl.NewRow();

                    _dtRow[_columnChiTietDauSize] = _focusedRow["DauSize"];
                    _dtRow[_columnChiTietMau] = _focusedRow["TenMau"];
                    _dtRow[_columnChiTietSize] = _focusedRow["Size"];

                    _dtRow[_columnChiTietCode] = item;

                    tbl.Rows.Add(_dtRow);
                }
            }
            else
            {
                tbl = null;
            }
            gridControl1.DataSource = tbl;
        }

        private DataTable createDataTableChiTiet()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("Mau", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            return dt;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void bandedGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            cellValueChanging = "";
            mapDataChiTiet();
        }


        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e.RowHandle >= 0)
            //{
            //    Console.WriteLine("gridView1_CellValueChanging");
            //    SetDataCode(e.Value.ToString());
            //}
            if (e.RowHandle >= 0)
            {
                cellvaluechangingChiTiet = e.Value?.ToString();
            }
        }

        // Cập nhật lại barcode khi thay đổi ở chi tiết
        private void SetDataCode()
        {
            int chiTietRowHandle = gridView1.FocusedRowHandle;
            int barcodeRowHandle = bandedGridView1.FocusedRowHandle;

            if (chiTietRowHandle < 0 || barcodeRowHandle < 0)
            {
                return;
            }

            string _barcodeCheck = gridView1.GetFocusedRowCellValue("Code")?.ToString();
            string _fieldName = bandedGridView1.FocusedColumn.FieldName;


            if (_fieldName == "BarCodeXacNhan" && !CheckContains(_barcodeCheck, false))
            {
                customSetRowCellValueChiTiet(chiTietRowHandle, "Code", _barcodeCheck);
                return;
            }

            List<string> lstItem = cellvaluechangingChiTiet.Split(',').ToList();

            List<string> lstCheck = (gridView1.DataSource as DataView).Table.AsEnumerable()
                          .Select(row => row.Field<string>("Code"))
                          .ToList();

            lstCheck[chiTietRowHandle] = cellvaluechangingChiTiet;
            List<string> lstCheckTrungInList = CheckTrungItemInList(lstCheck);

            if (lstCheckTrungInList.Count > 0)
            {
                MessageBox.Show(string.Format("Barcode: {0} đã trùng vui lòng kiểm tra lại!", lstCheckTrungInList[0]));
                customSetRowCellValueChiTiet(chiTietRowHandle, "Code", gridView1.GetFocusedRowCellValue("Code")?.ToString());
                cellvaluechangingChiTiet = string.Join(",", lstItem);
                return;
            }

            if (this.checkEdit1.Checked)
            {
                switch (_fieldName)
                {
                    case "BarCodeBao":
                    case "BarCodeTheBai":
                    case "Ecode":
                        string _lstBarcodeXacNhan = bandedGridView1.GetFocusedRowCellValue("BarCodeXacNhan")?.ToString();
                        if (_lstBarcodeXacNhan != null && !string.IsNullOrEmpty(_lstBarcodeXacNhan))
                        {
                            if (!CheckItemExist(lstItem, _lstBarcodeXacNhan))
                            {
                                MessageBox.Show(string.Format("Barcode: {0} chưa được khai báo trong Barcode Xác Nhận.!", lstItem[0]));
                                customSetRowCellValueChiTiet(chiTietRowHandle, "Code", gridView1.GetFocusedRowCellValue("Code")?.ToString());
                                return;
                                //customSetRowCellValue(RowHandel, _fieldName, RemoveLastItem(lstItem));
                                //cellvaluechangingChiTiet = string.Join(",", lstItem);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Barcode Xác Nhận chưa được khai báo. Vui lòng khai báo Barcode Xác Nhận trước!");
                            customSetRowCellValueChiTiet(chiTietRowHandle, "Code", gridView1.GetFocusedRowCellValue("Code")?.ToString());
                            return;
                            //customSetRowCellValue(RowHandel, _fieldName, RemoveLastItem(lstItem));
                            //cellvaluechangingChiTiet = string.Join(",", lstItem);
                        }
                        break;
                }
            }

            switch (_fieldName)
            {
                case "SoLuongChan":
                    _fieldName = "BarCodeChan";
                    break;
                case "SoLuongLe":
                    _fieldName = "BarCodeLe";
                    break;
                case "SoLuongXacNhan":
                    _fieldName = "BarCodeXacNhan";
                    break;
                case "SoLuongBao":
                    _fieldName = "BarCodeBao";
                    break;
                case "SoLuongTheBai":
                    _fieldName = "BarCodeTheBai";
                    break;
                case "SoLuongEcode":
                    _fieldName = "Ecode";
                    break;
            }

            List<string> lstBarCode = bandedGridView1.GetFocusedRowCellValue(_fieldName).ToString().Split(',').ToList();

            if (chiTietRowHandle <= lstBarCode.Count - 1)
            {
                lstBarCode[chiTietRowHandle] = cellvaluechangingChiTiet ?? "";

                customSetRowCellValue(barcodeRowHandle, _fieldName, string.Join(",", lstBarCode));


                //// check nếu cuối chuỗi là kí tự ',' thì xóa kí tự ','
                //string barcode = bandedGridView1.GetFocusedRowCellValue(_fieldName)?.ToString();
                //int length = barcode.Length;
                //if (length > 0)
                //{
                //    string firstChar = barcode.Substring(0, 1);
                //    string _barcodeHandle = barcode.Substring(1, barcode.Length - 1);
                //    string _rowCellValue = "";
                //    int _soLuong = 0;
                //    switch (_fieldName)
                //    {
                //        case "BarCodeXacNhan":
                //            _rowCellValue = "SoLuongXacNhan";
                //            break;
                //        case "BarCodeBao":
                //            _rowCellValue = "SoLuongBao";
                //            break;
                //        case "BarCodeTheBai":
                //            _rowCellValue = "SoLuongTheBai";
                //            break;
                //        case "Ecode":
                //            _rowCellValue = "SoLuongEcode";
                //            break;
                //    }
                //    if (firstChar == ",")
                //    {
                //        //isAddSeperate = true;
                //        customSetRowCellValue(barcodeRowHandle, _fieldName, _barcodeHandle);
                //        _soLuong = _barcodeHandle.Split(',').Length;
                //    }
                //    else
                //    {
                //        _soLuong = barcode.Split(',').Length;
                //    }

                //    bandedGridView1.SetRowCellValue(barcodeRowHandle, _rowCellValue, _soLuong);
                //    txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue(_rowCellValue).ToString();
                //}
            }
        }
        Keys keyPressGridView1 = Keys.None;
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            Console.WriteLine("gridControl1_ProcessGridKey");
            keyPressGridView1 = e.KeyCode;
            if (e.KeyCode == Keys.Enter)
            {
                SetDataCode();
            }
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();

            DXMenuItem deleeteItem = new DXMenuItem();
            deleeteItem.Caption = "Xóa item";
            // Copy.Image = imageCollection1.Images[2];
            deleeteItem.Click += DeleteItemBarcode;
            deleeteItem.Appearance.Font = new Font("Tahoma", 8.0F, System.Drawing.FontStyle.Bold);

            e.Menu.Items.Add(deleeteItem);
        }

        private void DeleteItemBarcode(object sender, EventArgs e)
        {
            int rowHandleChiTiet = gridView1.FocusedRowHandle;
            int rowHandleBarCode = bandedGridView1.FocusedRowHandle;
            string _itemDelete = gridView1.GetFocusedRowCellValue("Code")?.ToString();
            if (rowHandleChiTiet >= 0 && rowHandleBarCode >= 0 && _itemDelete != null)
            {

                // check xem barcode muốn xóa có được khai bao trong barcode bao, thẻ bài hay ecode chưa, nếu có thì cảnh báo yêu cầu xóa trước

                string _fieldName = bandedGridView1.FocusedColumn.FieldName;

                switch (_fieldName)
                {
                    case "SoLuongChan":
                        _fieldName = "BarCodeChan";
                        break;
                    case "SoLuongLe":
                        _fieldName = "BarCodeLe";
                        break;
                    case "SoLuongXacNhan":
                        _fieldName = "BarCodeXacNhan";
                        break;
                    case "SoLuongBao":
                        _fieldName = "BarCodeBao";
                        break;
                    case "SoLuongTheBai":
                        _fieldName = "BarCodeTheBai";
                        break;
                    case "SoLuongEcode":
                        _fieldName = "Ecode";
                        break;
                }

                if (_fieldName == "BarCodeXacNhan")
                {
                    List<string> _lstBarCodeXacNhan = bandedGridView1.GetFocusedRowCellValue("BarCodeXacNhan").ToString().Split(',').ToList();

                    if (!CheckContains(_itemDelete))
                    {
                        return;
                    }
                }

                DialogResult messResult = MessageBox.Show(string.Format("Bạn có muốn xóa BarCode: {0} không? ", _itemDelete), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    gridView1.DeleteRow(rowHandleChiTiet);

                    List<string> lstBarCode = bandedGridView1.GetFocusedRowCellValue(_fieldName).ToString().Split(',').ToList();
                    lstBarCode.RemoveAt(rowHandleChiTiet);

                    customSetRowCellValue(rowHandleBarCode, _fieldName, string.Join(",", lstBarCode));


                    string _rowCellValue = "";
                    int _soLuong = 0;
                    switch (_fieldName)
                    {
                        case "BarCodeChan":
                            _rowCellValue = "SoLuongChan";
                            break;
                        case "BarCodeLe":
                            _rowCellValue = "SoLuongLe";
                            break;
                        case "BarCodeXacNhan":
                            _rowCellValue = "SoLuongXacNhan";
                            break;
                        case "BarCodeBao":
                            _rowCellValue = "SoLuongBao";
                            break;
                        case "BarCodeTheBai":
                            _rowCellValue = "SoLuongTheBai";
                            break;
                        case "Ecode":
                            _rowCellValue = "SoLuongEcode";
                            break;
                    }
                    _soLuong = lstBarCode.Count;

                    bandedGridView1.SetRowCellValue(rowHandleBarCode, _rowCellValue, _soLuong);
                    txtSoLuong.Text = bandedGridView1.GetFocusedRowCellValue(_rowCellValue).ToString();
                }

            }

        }

        private bool CheckContains(string _itemDelete, bool isDelete = true)
        {
            List<string> _lstBarCodeBao = bandedGridView1.GetFocusedRowCellValue("BarCodeBao").ToString().Split(',').ToList();
            List<string> _lstBarCodeTheBai = bandedGridView1.GetFocusedRowCellValue("BarCodeTheBai").ToString().Split(',').ToList();
            List<string> _lstEcode = bandedGridView1.GetFocusedRowCellValue("Ecode").ToString().Split(',').ToList();

            if (_lstBarCodeBao.Contains(_itemDelete))
            {
                XtraMessageBox.Show(string.Format("Barcode: {0} đã được khai báo trong Barcode Bao. Vui lòng {1} trong Barcode Bao trước.!", _itemDelete, isDelete ? "Xóa" : "Chỉnh sửa"), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_lstBarCodeTheBai.Contains(_itemDelete))
            {
                XtraMessageBox.Show(string.Format("Barcode: {0} đã được khai báo trong Barcode Thẻ Bài. Vui lòng {1} trong Barcode Thẻ Bài trước.!", _itemDelete, isDelete ? "Xóa" : "Chỉnh sửa"), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_lstEcode.Contains(_itemDelete))
            {
                XtraMessageBox.Show(string.Format("Barcode: {0} đã được khai báo trong E-Code. Vui lòng {1} trong E-Code trước.!", _itemDelete, isDelete ? "Xóa" : "Chỉnh sửa"), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }
        private DataTable CreateTblSave()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("BarCodeChan", typeof(string));
            dt.Columns.Add("BarCodeLe", typeof(string));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            return dt;
        }
        private void TmrTest_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmrTest.Stop();
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {

                });
            }
            else
            {
                int RowHandle = bandedGridView1.FocusedRowHandle;
                bandedGridView1.FocusedRowHandle = RowHandle + 1;
            }
            tmrTest.Start();

        }
    }
}
