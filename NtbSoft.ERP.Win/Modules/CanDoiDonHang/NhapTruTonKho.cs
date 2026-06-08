using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Microsoft.Win32;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    public partial class NhapTruTonKho : DevExpress.XtraEditors.XtraForm
    {
        List<string> lstSize;

        //DataTable tblKho;

        List<DataTable> lstResultSize;

        DataTable tbl_TruTonKho = new DataTable();

        DataTable tblKho = new DataTable();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();

        // Sự kiện để thông báo khi Form2 đóng và trả dữ liệu về
        public event EventHandler<List<DataTable>> DataClosed;
        bool indicatorIcon = true;
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlSave;
        DataTable tblSoLuongCanDoi;
        List<int> lstSLOfSize;
        public NhapTruTonKho(DataTable _tbl_SLKH, DataTable _tbl_SLKHDefault, DataTable _tblKho)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            lstSLOfSize = new List<int>();
            lstSize = new List<string>();
            this.Resize += MainForm_Resize;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            //tblKho = _tblKho;
            tblKho = _tblKho;

            // tbl_TruTonKho -> Lưu nhập trừ tồn kho
            tbl_TruTonKho = _tbl_SLKHDefault;
            tbl_TruTonKho = RemoveColumnSLSX(tbl_TruTonKho);
            XuLyChuoiInitTBLNhapTru(tbl_TruTonKho);
            bandedGridView_TTK = createColBandsSizeTTK(tbl_TruTonKho);
            tblSoLuongCanDoi = CreateDataTableCanDoi(_tbl_SLKH);
            bandedGridView_CanDoiDonHang = createColBandsSizeCanDoiView(tblSoLuongCanDoi);
            gridControl1.DataSource = tblSoLuongCanDoi;
            //gridControl_TTK.DataSource = tbl_TruTonKho;
            
            onLoad(tbl_TruTonKho);
            lstResultSize = new List<DataTable>();
            DataTable tblSLTruKho = CreateTableSLTruKho(_tbl_SLKH, tblSoLuongCanDoi);
            // SetData(lstSize, _tbl_SLKH, _tblKho);
            
            
            SetDataNew(tbl_TruTonKho, _tblKho, tblSoLuongCanDoi, tblSLTruKho);

            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }

        // Xử lý column name của tblInitNhapTru
        private void XuLyChuoiInitTBLNhapTru(DataTable tblInitNhapTru)
        {
            foreach (DataColumn column in tblInitNhapTru.Columns)
            {
                if (column.ColumnName.Contains("Size_SLKH@"))
                {
                    Console.WriteLine("Size@");
                    column.ColumnName = column.ColumnName.Replace("_SLKH", "");
                }
            }
        }

        // CreateTableSLDaTruKho:Tạo ra DataTable với số lượng đã trừ kho của từng size
        // Dựa vào tblSLKHFocused truyền từ form DonHangTonKho vào
        private DataTable CreateTableSLTruKho(DataTable slKHFocused, DataTable slSXChuaCanDoi)
        {
            // SLKH
            DataTable tblSLKH = slSXChuaCanDoi.Clone();
            DataRow rowAddSLKH = tblSLKH.NewRow();
            tblSLKH.Rows.Add(rowAddSLKH);
            DataRow rowSLKH = tblSLKH.Rows[0];

            // SLSX
            DataTable tblSLSX = slSXChuaCanDoi.Clone();
            DataRow rowAddSLSX = tblSLSX.NewRow();
            tblSLSX.Rows.Add(rowAddSLSX);
            DataRow rowSLSX = tblSLSX.Rows[0];

            // SLTruKho
            DataTable tblSLTruKho = slSXChuaCanDoi.Clone();
            DataRow rowAddSLTruKho = tblSLTruKho.NewRow();
            tblSLTruKho.Rows.Add(rowAddSLTruKho);
            DataRow rowSLTruKho = tblSLTruKho.Rows[0];

            DataRow rowSLKHFocused = slKHFocused.Rows[0];
            // Mục đích: Lấy ra SLKH và SLSX của từng Size
            // Sau đó SLKH - SLSX tính ra số lượng check trừ kho 
            // của từng size

            // Lấy SLKH vào tblSLKH
            foreach (DataColumn column in slKHFocused.Columns)
            {
                if (column.ColumnName.Contains("Size_SLKH"))
                {
                    string colName = column.ColumnName.Replace("_SLKH", "");
                    if (tblSLKH.Columns.Contains(colName))
                    {
                        rowSLKH[colName] = rowSLKHFocused[column.ColumnName];
                    }
                }
            }

            // Lấy SLKH vào tblSLSX
            foreach (DataColumn column in slKHFocused.Columns)
            {
                if (column.ColumnName.Contains("Size_SLSX"))
                {
                    string colName = column.ColumnName.Replace("_SLSX", "");
                    if (tblSLSX.Columns.Contains(colName))
                    {
                        rowSLSX[colName] = rowSLKHFocused[column.ColumnName];
                    }
                }
            }


            // Gán giá trị cho SLTruKho
            foreach (DataColumn column in tblSLKH.Columns)
            {
                if (column.ColumnName.Contains("Size@"))
                {
                    int slKH = int.Parse(rowSLKH[column.ColumnName].ToString());
                    int slSX = int.Parse(rowSLSX[column.ColumnName].ToString());
                    int slTruKho = slKH - slSX;
                    rowSLTruKho[column.ColumnName] = slTruKho;
                }
            }

            // Chỉ giữ cột Size
            for (int i = 0; i < tblSLTruKho.Columns.Count;)
            {
                DataColumn column = tblSLTruKho.Columns[i];
                if (column.ColumnName.Contains("Size@"))
                {
                    i += 1;
                }
                else
                {
                    tblSLTruKho.Columns.RemoveAt(i);
                }
            }
            return tblSLTruKho;
        }

        private DataTable CreateDataTableCanDoi(DataTable _tblSLKHSX)
        {
            try
            {
                // _tblTemp sau khi xử lý xong sẽ luôn luôn có 3 dòng DataRow
                // Dòng thứ nhất là số lượng SX
                // Dòng thứ hai là số lượng Cân Đối
                // Dòng thứ ba là số lượng Còn Lại để check trừ tồn kho

                DataTable _tblTemp = _tblSLKHSX.Copy();

                // Thêm một cột TenDuLieu vào DataTable
                _tblTemp.Columns.Add("TenDuLieu", typeof(string));

                // Thêm dữ liệu dòng thứ nhất, Xóa cột SLKH:
                for (int i = 0; i < _tblTemp.Columns.Count;)
                {
                    DataColumn column = _tblTemp.Columns[i];
                    if (column.ColumnName.Equals("MaDH") || column.ColumnName.Equals("POID") || column.ColumnName.Equals("PO") ||
                        column.ColumnName.Equals("DauSizeID") || column.ColumnName.Equals("DauSize") || column.ColumnName.Equals("MaMau") ||
                        column.ColumnName.Equals("TenMau") || column.ColumnName.Equals("TenDuLieu") || column.ColumnName.Contains("Size_SLSX@"))
                    {
                        i += 1;
                        if (column.ColumnName.Contains("Size_SLSX@"))
                        {
                            _tblTemp.Columns[column.ColumnName].ColumnName = _tblTemp.Columns[column.ColumnName].ColumnName.Replace("_SLSX", "");
                        }
                    }
                    else
                    {
                        _tblTemp.Columns.Remove(column);
                    }
                }
                _tblTemp.Rows[0]["TenDuLieu"] = "Số lượng Sản Xuất : ";

                // Thêm dữ liệu dòng thứ hai:
                //DataTable _tblCanDoi = null;
                //if (_tblTemp.Rows[0] != null)
                //{
                string _maDH = _tblTemp.Rows[0]["MaDH"].ToString();
                string _poID = _tblTemp.Rows[0]["POID"].ToString();
                string _dauSizeID = _tblTemp.Rows[0]["DauSizeID"].ToString();
                string _maMau = _tblTemp.Rows[0]["MaMau"].ToString();
                string url = string.Format("{0}?maDH={1}&&poID={2}&&dauSizeID={3}&&maMau={4}", URL + "CanDoiDonHangTong/GetDsCanDoiDonHang", _maDH, _poID, _dauSizeID, _maMau);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable _tblCanDoi = JsonConvert.DeserializeObject<DataTable>(json);
                //}
                //DataTable _tblCanDoi = null;
                //
                DataRow _rowSLCanDoi = _tblTemp.NewRow();
                if (_tblCanDoi != null && _tblCanDoi.Rows.Count > 0)
                {
                    foreach (DataColumn column in _tblTemp.Columns)
                    {
                        if (column.ColumnName.Contains("Size@"))
                        {
                            _rowSLCanDoi[column] = 0;
                        }
                    }
                    //_rowSLCanDoi.ItemArray = _tblCanDoi.Rows[0].ItemArray;
                    foreach (DataColumn column in _tblCanDoi.Columns)
                    {
                        _rowSLCanDoi[column.ColumnName] = _tblCanDoi.Rows[0][column.ColumnName];
                    }
                }
                else
                {
                    foreach (DataColumn column in _tblTemp.Columns)
                    {
                        if (!column.ColumnName.Contains("Size@"))
                        {
                            _rowSLCanDoi[column] = _tblTemp.Rows[0][column];
                        }
                        else
                        {
                            _rowSLCanDoi[column] = 0;
                        }
                    }
                }
                _rowSLCanDoi["TenDuLieu"] = "Số lượng Cân Đối : ";
                // Import vào row Số lượng Cân Đối
                _tblTemp.Rows.Add(_rowSLCanDoi);

                // Thêm dữ liệu dòng thứ ba:
                DataRow _rowSLConLai = _tblTemp.NewRow();
                foreach (DataColumn column in _tblTemp.Columns)
                {
                    if (!column.ColumnName.Contains("Size@"))
                    {
                        if (column.ColumnName.Contains("TenDuLieu"))
                        {
                            _rowSLConLai[column] = "Số lượng Còn Lại : ";
                        }
                        else
                        {
                            _rowSLConLai[column] = _tblTemp.Rows[0][column];
                        }

                    }
                    else
                    {
                        int tryParse = -1;
                        string _soLuongRowSX = _tblTemp.Rows[0][column].ToString();
                        string _soLuongRowCanDoi = _tblTemp.Rows[1][column].ToString();

                        if (int.TryParse(_soLuongRowSX, out tryParse) && int.TryParse(_soLuongRowCanDoi, out tryParse))
                        {
                            _rowSLConLai[column] = int.Parse(_soLuongRowSX) - int.Parse(_soLuongRowCanDoi);
                            lstSLOfSize.Add(int.Parse(_rowSLConLai[column].ToString()));
                        }
                        else
                        {
                            lstSLOfSize.Add(0);
                        }

                    }
                }
                // Import vào row Số lượng Còn Lại
                _tblTemp.Rows.Add(_rowSLConLai);

                // Xóa dòng dữ liệu đầu tiên: Số lượng Sản Xuất
                _tblTemp.Rows.RemoveAt(0);
                return _tblTemp;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex");
                return new DataTable();
            }


        }

        private BandedGridView createColBandsSizeCanDoiView(DataTable _tblSLKHSX)
        {
            BandedGridView bandedGridview = new BandedGridView();
            bandedGridview = bandedGridView_CanDoiDonHang;
            try
            {
                if (lstSize != null && lstSize.Count > 0)
                {
                    lstSize.Clear();
                }
                gridBandSize_CanDoi.Children.Clear();
                //bandedGridview.Columns.Clear();
                foreach (DataColumn dc in _tblSLKHSX.Columns)
                {
                    if (dc.ColumnName.Contains("Size@"))
                    {
                        string colName = dc.ColumnName;
                        lstSize.Add(colName.Replace("Size@", ""));
                        //colName.Replace("@Size", "");
                        BandedGridColumn col = new BandedGridColumn();
                        col.AppearanceCell.Options.UseTextOptions = true;
                        col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.AppearanceHeader.Options.UseTextOptions = true;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //col.Caption = colName.Replace("Size@", "");
                        col.Caption = colName.Split('@')[1];
                        col.FieldName = dc.ColumnName;
                        col.Name = "col" + colName;
                        col.OptionsColumn.AllowEdit = true;
                        col.Visible = true;
                        col.Width = 50;
                        //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        col.DisplayFormat.FormatString = "{0:##,0}";
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        col.OptionsColumn.AllowEdit = false;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { col });
                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gb.AppearanceHeader.Options.UseFont = true;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        //gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(239)))), ((int)(((byte)(230)))));
                        gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                        gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gb.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        gb.AppearanceHeader.Options.UseBackColor = true;
                        gb.AppearanceHeader.Options.UseForeColor = true;
                        //gb.Caption = colName.Replace("Size@", "");
                        gb.Caption = colName.Split('@')[2];
                        gb.Columns.Add(col);
                        gb.Name = "gb" + "Size_" + col;
                        gb.VisibleIndex = 0;
                        gb.Width = 60;
                        gridBandSize_CanDoi.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                }
                return bandedGridview;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return new BandedGridView();
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlSave = new ActionControl(BtLuu, true, ActionType.Save, true);
            return new List<ActionControl> {
                actionControlSave}; ;
        }
        private DataTable RemoveColumnSLSX(DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if (dataTable.Columns[i].ColumnName.Contains("Size_SLSX@"))
                {
                    dataTable.Columns.Remove(dataTable.Columns[i].ColumnName);
                }
            }
            return dataTable;
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Xử lý sự kiện thay đổi kích thước màn hình
            //MessageBox.Show("Display settings changed!");
            this.Luu.Enabled = true;
            Console.WriteLine("Form_Reszie");

        }

        private void onLoad(DataTable _tbl_TruTonKho)
        {
            gridControl_TTK.DataSource = _tbl_TruTonKho;
            Console.WriteLine("onLoad");
        }


        private void SaveLuuTruTonKho()
        {
            DataTable tbl = gridControl_TTK.DataSource as DataTable;
            DataTable tblUnpivot = UnPivot(tbl);
            //List<DonHangTongPOChiTietEntity> lst = JsonConvert.DeserializeObject<List<DonHangTongPOChiTietEntity>>(JsonConvert.SerializeObject(tblUnpivot));
            //string url = string.Format("{0}", URL + "DonHangTong/PostDonHangTongPOChiTiet");
            //string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lst); }).Result;
            //Console.WriteLine(result);
            Console.WriteLine("result");
            DataTable tblSave = tblUnpivot.Clone();
            foreach (DataRow row in tblUnpivot.Rows)
            {
                if (int.Parse(row["SoLuong"].ToString()) != 0)
                {
                    tblSave.ImportRow(row);
                }
            }
            if (tblSave == null || tblSave.Rows.Count == 0)
                return;
            // Gọi api lưu
            string url = string.Format("{0}", URL + "DonHangTonKho/InsertDataTruTonKho");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
        }

        private DataTable UnPivot(DataTable tblThongTonDonHang)
        {
            DataTable tblSave = createTableLuuTruTonKho();
            if (tblThongTonDonHang == null || tblThongTonDonHang.Rows.Count == 0) return null;
            DataRow tbl_kho = tblKho.Rows[0];
            foreach (DataRow dataRows in tblThongTonDonHang.Rows)
            {
                foreach (DataColumn column in tblThongTonDonHang.Columns)
                {
                    if (column.ColumnName.Contains("Size@"))
                    {
                        Console.WriteLine("dataRows");
                        //tblSave[""]
                        DataRow dataRowSave = tblSave.NewRow();
                        dataRowSave["MaHang"] = tbl_kho["MaHang"];
                        dataRowSave["MaDH"] = dataRows["MaDH"];
                        dataRowSave["DauSizeID"] = dataRows["DauSizeID"];
                        dataRowSave["DauSize"] = dataRows["DauSize"];
                        dataRowSave["MaMau"] = dataRows["MaMau"];
                        string sizeID = column.ColumnName.Split('@')[1];
                        string size = column.ColumnName.Split('@')[2];
                        dataRowSave["SizeID"] = sizeID;
                        dataRowSave["Size"] = size;
                        dataRowSave["SoLuong"] = dataRows[column];
                        dataRowSave["GhiChu"] = dataRows["GhiChu"];
                        tblSave.Rows.Add(dataRowSave);
                    }
                }
            }
            Console.WriteLine("end for");
            return tblSave;
        }

        private DataTable createTableLuuTruTonKho()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(string));
            tbl.Columns.Add("NgayTru", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        // tblSLCL -> số lượng SX chưa cân đối của từng size
        private void SetDataNew(DataTable tblInitNhapTru, DataTable tblSLTonKho, DataTable tblSLCD, DataTable tblSLTruTonKho)
        {
            // New
            // Tạo một List<DataTable> lstResult với 4 phần tử
            // item0: DataTable lưu số lượng nhập của từng size
            // item1: DataTable lưu số lượng tồn kho của từng size
            // item2: DataTable lưu số lượng SX chưa cân đối của từng size
            // item3: DataTable lưu số lượng đã trừ kho

            // Add InitNhapTru
            // Xử lý bỏ chuỗi _SLKH
            lstResultSize.Add(tblInitNhapTru);

            // Add SLTonKho
            lstResultSize.Add(tblSLTonKho);

            // Add SLCL -> Số lượng còn lại chưa cân đối
            DataTable tblSLCL = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(tblSLCD));
            // tblSLCL, Row vị trí 0: Là SLCD; vị trí 1: Là SLCL
            // Xóa row ở vị trí 0 -> DataTable chỉ còn lại SLCL
            tblSLCL.Rows.RemoveAt(0);
            lstResultSize.Add(tblSLCL);

            // Add SLTruTonKho
            lstResultSize.Add(tblSLTruTonKho);
        }
        private void SetData(List<string> lstSize, DataTable tblSLKH, DataTable tblKho)
        {
            // Old
            // lstResultSize là mảng hai chiều
            // Có 4 dòng
            // dòng thứ 0 lưu các size
            // dòng thứ 1 lưu số lượng nhập của từng size
            // dòng thứ 2 lưu số lượng tồn kho của từng size
            // dòng thứ 3 lưu số lượng kế hoạch của từng size
            // dòng thứ 4 lưu số lượng sản xuất của từng size

            //// Add tên size
            //lstResultSize.Add(lstSize);

            //// Add dữ liệu nhập ban đầu = 0 cho các Size
            //List<string> lstDataSize = new List<string>();
            //// Gán giá trị mặc định = 0
            //for (int j = 0; j < lstSize.Count; j++)
            //{
            //    lstDataSize.Add("0");
            //}
            //lstResultSize.Add(lstDataSize);

            //// Add dữ liệu còn lại trong kho của các size
            //List<string> lstKho = new List<string>();

            //for (int i = 0; i < lstSize.Count; i++)
            //{
            //    lstKho.Add("0");
            //}

            //foreach (DataRow row in tblKho.Rows)
            //{
            //    foreach (DataColumn column in tblKho.Columns)
            //    {
            //        if (column.ColumnName.Contains("Size_SLCLK@"))
            //        {
            //            string data = row[column.ColumnName].ToString();

            //            if (!string.IsNullOrEmpty(data))
            //            {
            //                //lstKho.Add(data);

            //                if (column.ColumnName.Split('@').Length >= 1)
            //                {
            //                    int index = lstSize.IndexOf(column.ColumnName.Split('@')[1]);
            //                    lstKho[index] = data;
            //                }
            //            }
            //            //else
            //            //{
            //            //    lstKho.Add("0");
            //            //}

            //        }
            //    }
            //}
            //lstResultSize.Add(lstKho);

            //// Add dữ liệu Số lượng kế hoạch của các size
            //List<string> lstSLKH = new List<string>();

            //List<string> lstSLSX = new List<string>();

            //foreach (DataRow row in tblSLKH.Rows)
            //{
            //    foreach (DataColumn column in tblSLKH.Columns)
            //    {
            //        if (column.ColumnName.Contains("Size_SLKH@"))
            //        {
            //            string data = row[column.ColumnName].ToString();
            //            if (!string.IsNullOrEmpty(data))
            //            {
            //                lstSLKH.Add(data);
            //            }
            //            else
            //            {
            //                lstSLKH.Add("0");
            //            }
            //        }



            //        if (column.ColumnName.Contains("Size_SLSX@"))
            //        {
            //            string data = row[column.ColumnName].ToString();
            //            if (!string.IsNullOrEmpty(data))
            //            {
            //                lstSLSX.Add(data);
            //            }
            //            else
            //            {
            //                lstSLSX.Add("0");
            //            }
            //        }
            //    }
            //    lstResultSize.Add(lstSLKH);
            //    lstResultSize.Add(lstSLSX);
            //}

            //Console.WriteLine("SLKH");
        }

        private void bandedGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //Console.WriteLine("cell_valued_changed");
            //string size = lstResultSize[0].Where(item => item == e.Column.Caption).ToList().First();
            //int index = lstResultSize[0].IndexOf(e.Column.Caption);

            //lstResultSize[1][index] = e.Value.ToString();
            ////lstResultSize[0];
            ///
            DataTable tblNhapTru = lstResultSize[0];
            DataRow dataRowTblNhapTru = tblNhapTru.Rows[0];
            dataRowTblNhapTru[e.Column.FieldName] = e.Value;
        }

        private void bandedGridView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            Console.WriteLine("Validating_editor");
            int value = -1;
            bool flagParse = int.TryParse(e.Value.ToString(), out value);
            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.Value = "0";
                this.Luu.Enabled = true;

                return;
            }

            if (flagParse)
            {

                // New
                // Tạo một List<DataTable> lstResultSize với 4 phần tử
                // item0: DataTable lưu số lượng nhập của từng size
                // item1: DataTable lưu số lượng tồn kho của từng size
                // item2: DataTable lưu số lượng SX chưa cân đối của từng size
                // item3: DataTable lưu số lượng đã trừ kho

                string focusedColumn = bandedGridView_TTK.FocusedColumn.FieldName;
                string sizeID = focusedColumn.Split('@')[1];
                string size = focusedColumn.Split('@')[2];
                string sizeSLCLK = string.Format("Size_SLCLK@{0}@{1}", sizeID, size);


                DataTable tblNhapTru = lstResultSize[0];
                DataRow dataRowTblNhapTru = tblNhapTru.Rows[0];

                DataTable tblSLCLKho = lstResultSize[1];
                DataRow dataRowTblSLCLKho = tblSLCLKho.Rows[0];
                int _slCLKho = 0;
                if (tblSLCLKho.Columns.Contains(sizeSLCLK))
                {
                    _slCLKho = int.Parse(dataRowTblSLCLKho[sizeSLCLK].ToString());
                }

                DataTable tblSLSXChuaCanDoi = lstResultSize[2];
                DataRow dataRowTblSLSXChuaCanDoi = tblSLSXChuaCanDoi.Rows[0];
                int _slSXChuaCanDoi = 0;
                string focusedColumnName = focusedColumn.Replace("_SLKH", "");
                if (tblSLSXChuaCanDoi.Columns.Contains(focusedColumnName))
                {
                    _slSXChuaCanDoi = int.Parse(dataRowTblSLSXChuaCanDoi[focusedColumnName].ToString());
                }

                DataTable tblSLTruKho = lstResultSize[3];
                DataRow dataRowTblSLTruKho = tblSLTruKho.Rows[0];
                int _slTruKho = 0;
                
                if (tblSLTruKho.Columns.Contains(focusedColumnName))
                {
                    _slTruKho = int.Parse(dataRowTblSLTruKho[focusedColumnName].ToString());
                }

                // Số lượng nhập trừ tồn kho
                int slNhapTru = int.Parse(e.Value.ToString());

                // Kiểm tra số lượng nhập trừ kho với _slSXChuaCanDoi
                if (slNhapTru > _slSXChuaCanDoi)
                {
                    ShowErrorValidate(e, "Số lượng nhập vào không được lớn hơn số lượng sản xuất.\nVui lòng nhập số khác.!");
                    return;
                }

                if (slNhapTru < 0)
                {
                    if (Math.Abs(slNhapTru) > _slTruKho)
                    {
                        ShowErrorValidate(e, "Số lượng trả lại kho không được lớn hơn số lượng đã trừ kho trước đó.\nVui lòng nhập số khác.!");
                        return;
                    }
                }

                if (slNhapTru > 0 && _slCLKho == 0)
                {
                    //ShowErrorValidate(e, "Số lượng của size hiện tại không còn lại trong kho.\nVui lòng nhập số khác!");
                    ShowErrorValidate(e, string.Format("Số lượng của size {0} không có trong kho!", size));
                    return;
                }

                if (slNhapTru > _slCLKho)
                {
                    ShowErrorValidate(e, "Số lượng nhập vào lớn hơn số lượng còn lại trong kho.\nVui lòng nhập số khác!");
                    return;
                }
                this.Luu.Enabled = true;
            }
            else
            {
                ShowErrorValidate(e, "Vui lòng nhập số!");
            }
        }

        private void ShowErrorValidate(BaseContainerValidateEditorEventArgs e, string errorText)
        {
            e.Valid = false;
            e.ErrorText = errorText;
            this.Luu.Enabled = false;
        }

        private BandedGridView createColBandsSizeTTK(DataTable _thongTinDonHang)
        {
            BandedGridView bandedGridview = new BandedGridView();
            bandedGridview = bandedGridView_TTK;
            try
            {
                if (lstSize != null && lstSize.Count > 0)
                {
                    lstSize.Clear();
                }
                gridBandSize_TTK.Children.Clear();
                bandedGridview.Columns.Clear();
                foreach (DataColumn dc in _thongTinDonHang.Columns)
                {
                    if (dc.ColumnName.Contains("Size@"))
                    {
                        string colName = dc.ColumnName;
                        lstSize.Add(colName.Replace("Size@", ""));
                        //colName.Replace("@Size", "");
                        BandedGridColumn col = new BandedGridColumn();
                        col.AppearanceCell.Options.UseTextOptions = true;
                        col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.AppearanceHeader.Options.UseTextOptions = true;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //col.Caption = colName.Replace("Size@", "");
                        col.Caption = colName.Split('@')[1];
                        col.FieldName = dc.ColumnName;
                        col.Name = "col" + colName;
                        col.OptionsColumn.AllowEdit = true;
                        col.Visible = true;
                        col.Width = 50;
                        //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        col.DisplayFormat.FormatString = "{0:##,0}";
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { col });
                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gb.AppearanceHeader.Options.UseFont = true;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //
                        //gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(239)))), ((int)(((byte)(230)))));
                        gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                        gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gb.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        gb.AppearanceHeader.Options.UseBackColor = true;
                        gb.AppearanceHeader.Options.UseForeColor = true;
                        //gb.Caption = colName.Replace("Size@", "");
                        gb.Caption = colName.Split('@')[2];
                        gb.Columns.Add(col);
                        gb.Name = "gb" + "Size_" + col;
                        gb.VisibleIndex = 0;
                        gb.Width = 60;
                        gridBandSize_TTK.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "poid")
                    {
                        BandedGridColumn colPoID = new BandedGridColumn();
                        colPoID.AppearanceCell.Options.UseTextOptions = true;
                        colPoID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPoID.AppearanceHeader.Options.UseTextOptions = true;
                        colPoID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPoID.Caption = dc.ColumnName.Replace("Size@", "");
                        colPoID.FieldName = dc.ColumnName;
                        colPoID.Name = "col" + dc.ColumnName;
                        colPoID.OptionsColumn.AllowEdit = false;
                        colPoID.Visible = true;
                        colPoID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPoID });
                        gridBandPOID_TTK.Columns.Add(colPoID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "po")
                    {
                        BandedGridColumn colPo = new BandedGridColumn();
                        colPo.AppearanceCell.Options.UseTextOptions = true;
                        colPo.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPo.AppearanceHeader.Options.UseTextOptions = true;
                        colPo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colPo.Caption = dc.ColumnName.Replace("Size@", "");
                        colPo.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colPo.FieldName = dc.ColumnName;
                        colPo.Name = "col" + dc.ColumnName;
                        colPo.OptionsColumn.AllowEdit = false;
                        colPo.Visible = true;
                        colPo.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colPo });
                        gridBandPO_TTK.Columns.Add(colPo);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "mamau")
                    {
                        BandedGridColumn colMaMau = new BandedGridColumn();
                        colMaMau.AppearanceCell.Options.UseTextOptions = true;
                        colMaMau.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaMau.AppearanceHeader.Options.UseTextOptions = true;
                        colMaMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaMau.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaMau.FieldName = dc.ColumnName;
                        colMaMau.Name = "col" + dc.ColumnName;
                        colMaMau.OptionsColumn.AllowEdit = false;
                        colMaMau.Visible = true;
                        colMaMau.Width = 50;
                        //colMaMau.ColumnEdit = searchLookUpEditMau1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaMau });
                        gridBandMaMau_TTK.Columns.Add(colMaMau);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "tenmau")
                    {
                        BandedGridColumn colTenMau = new BandedGridColumn();
                        colTenMau.AppearanceCell.Options.UseTextOptions = true;
                        colTenMau.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colTenMau.AppearanceHeader.Options.UseTextOptions = true;
                        colTenMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colTenMau.Caption = dc.ColumnName.Replace("Size@", "");
                        colTenMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colTenMau.FieldName = dc.ColumnName;
                        colTenMau.Name = "col" + dc.ColumnName;
                        colTenMau.OptionsColumn.AllowEdit = false;
                        colTenMau.Visible = true;
                        colTenMau.Width = 50;
                        //colMaMau.ColumnEdit = searchLookUpEditMau1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colTenMau });
                        gridBandTenMau_TTK.Columns.Add(colTenMau);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausize")
                    {
                        BandedGridColumn colDauSize = new BandedGridColumn();
                        colDauSize.AppearanceCell.Options.UseTextOptions = true;
                        colDauSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSize.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSize.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSize.FieldName = dc.ColumnName;
                        colDauSize.Name = "col" + dc.ColumnName;
                        colDauSize.OptionsColumn.AllowEdit = false;
                        colDauSize.Visible = true;
                        colDauSize.Width = 125;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSize });
                        gridBandDauSize_TTK.Columns.Add(colDauSize);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "dausizeid")
                    {
                        BandedGridColumn colDauSizeID = new BandedGridColumn();
                        colDauSizeID.AppearanceCell.Options.UseTextOptions = true;
                        colDauSizeID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSizeID.AppearanceHeader.Options.UseTextOptions = true;
                        colDauSizeID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colDauSizeID.Caption = dc.ColumnName.Replace("Size@", "");
                        colDauSizeID.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colDauSizeID.FieldName = dc.ColumnName;
                        colDauSizeID.Name = "col" + dc.ColumnName;
                        colDauSizeID.OptionsColumn.AllowEdit = false;
                        colDauSizeID.Visible = true;
                        colDauSizeID.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colDauSizeID });
                        gridBandDauSizeID_TTK.Columns.Add(colDauSizeID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "madh")
                    {
                        BandedGridColumn colMaDH = new BandedGridColumn();
                        colMaDH.AppearanceCell.Options.UseTextOptions = true;
                        colMaDH.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaDH.AppearanceHeader.Options.UseTextOptions = true;
                        colMaDH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaDH.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaDH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaDH.FieldName = dc.ColumnName;
                        colMaDH.Name = "col" + dc.ColumnName;
                        colMaDH.OptionsColumn.AllowEdit = false;
                        colMaDH.Visible = true;
                        colMaDH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaDH });
                        gridBandMaDH_TTK.Columns.Add(colMaDH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "maqg")
                    {
                        BandedGridColumn colMaQG = new BandedGridColumn();
                        colMaQG.AppearanceCell.Options.UseTextOptions = true;
                        colMaQG.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaQG.AppearanceHeader.Options.UseTextOptions = true;
                        colMaQG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colMaQG.Caption = dc.ColumnName.Replace("Size@", "");
                        colMaQG.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colMaQG.FieldName = dc.ColumnName;
                        colMaQG.Name = "col" + dc.ColumnName;
                        colMaQG.OptionsColumn.AllowEdit = false;
                        colMaQG.Visible = true;
                        colMaQG.Width = 50;
                        //colMaQG.ColumnEdit = searchLookUpEditQuocGia1;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colMaQG });
                        gridBandMaQuocGia_TTK.Columns.Add(colMaQG);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ngaygh")
                    {
                        BandedGridColumn colNgayGH = new BandedGridColumn();
                        colNgayGH.AppearanceCell.Options.UseTextOptions = true;
                        colNgayGH.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colNgayGH.AppearanceHeader.Options.UseTextOptions = true;
                        colNgayGH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colNgayGH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                        colNgayGH.DisplayFormat.FormatString = "dd/MM/yyyy";
                        colNgayGH.Caption = dc.ColumnName.Replace("Size@", "");
                        colNgayGH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colNgayGH.OptionsColumn.AllowEdit = false;
                        colNgayGH.FieldName = dc.ColumnName;
                        colNgayGH.Name = "col" + dc.ColumnName;
                        colNgayGH.Visible = true;
                        colNgayGH.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colNgayGH });
                        gridBandNgayGH_TTK.Columns.Add(colNgayGH);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ghichu")
                    {
                        BandedGridColumn colGhiChu = new BandedGridColumn();
                        colGhiChu.AppearanceCell.Options.UseTextOptions = true;
                        colGhiChu.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colGhiChu.AppearanceHeader.Options.UseTextOptions = true;
                        colGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colGhiChu.Caption = dc.ColumnName.Replace("Size@", "");
                        colGhiChu.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        colGhiChu.FieldName = dc.ColumnName;
                        colGhiChu.Name = "col" + dc.ColumnName;
                        colGhiChu.OptionsColumn.AllowEdit = false;
                        colGhiChu.Visible = true;
                        colGhiChu.Width = 50;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { colGhiChu });
                        gridBandGhiChu_TTK.Columns.Add(colGhiChu);
                    }
                }
                return bandedGridview;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return new BandedGridView();
        }

        private void BtLuu()
        {
            this.ActiveControl = this.button1;
            this.Close();
            Console.WriteLine("bt_save_item_click");
            SaveLuuTruTonKho();
            // Kích hoạt sự kiện và truyền dữ liệu cần trả về
            DataClosed?.Invoke(this, lstResultSize);
        }

        private void BtSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            BtLuu();
        }

        private void BtRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            onLoad(tbl_TruTonKho);
        }

        private void bandedGridView_TTK_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void bandedGridView_TTK_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void bandedGridView_TTK_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null || string.IsNullOrEmpty(e.Band.Caption)) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void NhapTruTonKho_Load(object sender, EventArgs e)
        {

        }

        private void bandedGridView_CanDoiDonHang_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;

            // Kiểm tra nếu đây là ô cần được tùy chỉnh
            if (e.Column.FieldName.Equals(this.bandedGridColumnTenSoLuong.FieldName))
            {
                // Thiết lập màu nền và màu chữ của ô
                //e.Appearance.BackColor = Color.Yellow;
                e.Appearance.ForeColor = Color.Red;
                // Thiết lập font của ô
                e.Appearance.Font = new Font("Arial", 8, FontStyle.Bold);
            }
        }
    }
}
