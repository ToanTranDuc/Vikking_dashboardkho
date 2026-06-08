using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    public partial class frmCanDoiDinhMucNguyenLieu : Form
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string maLenhSX = string.Empty;
        string lenhSX = string.Empty;
        string maDH = string.Empty;
        string maHang = string.Empty;
        string dot = string.Empty;
        string maGop = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        int _SLTong = 0;
        DataRow mg;
        public frmCanDoiDinhMucNguyenLieu(DataRow _currentRow)
        {

            InitializeComponent();
            if (_currentRow != null)
            {
                maLenhSX = _currentRow["MaLenhSanXuat"]?.ToString() ?? "";
                lenhSX = _currentRow["MaLenh"]?.ToString() ?? "";
                maDH = _currentRow["MaDH"]?.ToString() ?? "";
                maHang = _currentRow["MaHang"]?.ToString() ?? "";
                dot = _currentRow["DotSX"]?.ToString() ?? "";
                maGop = _currentRow["MaGop"]?.ToString() ?? "";
                mg = _currentRow;
            }

            init();
        }

        private void init()
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            txtMaDH.EditValue = maDH;
            txtMaHang.EditValue = maHang;
            txtLenhSX.EditValue = lenhSX;
            txtDot.EditValue = dot;
            LoadData();
            CheckPerminsion();
        }

        private void LoadData()
        {
            // Load số lượng cân đối Bom Vat Tu
            string urlCanDoi = string.Format("{0}?madh={1}&&maLenhSX={2}", URL + "CanDoiDMNL/GetCanDoiBomVatTu", maDH, maLenhSX);
            string jsonCanDoi = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCanDoi); }).Result;
            DataTable tblSL = JsonConvert.DeserializeObject<DataTable>(jsonCanDoi);
            gridControl2.DataSource = tblSL;

            string urlDM = string.Format("{0}?mahang={1}", URL + "CanDoiDMNL/GetDinhMucBOM", maHang);
            string jsonDM = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDM); }).Result;
            DataTable tblDM = JsonConvert.DeserializeObject<DataTable>(jsonDM);

            string urlDMCD = string.Format("{0}?madh={1}&&maLenhSX={2}", URL + "CanDoiDMNL/GetDinhMucCD", maGop, maLenhSX);
            string jsonDMCD = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDMCD); }).Result;
            DataTable tblDMCD = JsonConvert.DeserializeObject<DataTable>(jsonDMCD);
            if (tblDM.Rows.Count > 0 && tblDM != null)
            {

                int totalQuantity = 0;
                foreach (DataRow row2 in tblDM.Rows)
                {
                    totalQuantity = 0;
                    string dausize2 = row2["DauSizeID"].ToString();
                    string mau2 = row2["MaMau"].ToString();
                    string validSizes = row2["SizeID"].ToString().Trim().Replace(" ", "");

                    // Trường hợp 1: MaMau có, DauSizeID có và Size có
                    if (validSizes != "ALL" && mau2 != "ALL" && dausize2 != "ALL")
                    {
                        foreach (DataRow row1 in tblSL.Rows)
                        {
                            string mau1 = row1["MaMau"].ToString();
                            string dausize1 = row1["DauSizeID"].ToString();
                            string size = row1["SizeID"].ToString().Trim().Replace(" ", "");
                            int soLuong = Convert.ToInt32(row1["SoLuong"]);
                            if (mau1 == mau2 && dausize1 == dausize2 && validSizes.Split(';').Contains(size))
                            {
                                totalQuantity += soLuong;
                            }
                        }
                    }
                    // Trường hợp 2: MaMau có, DauSizeID có và Size = "ALL"
                    else if (validSizes == "ALL" && mau2 != "ALL" && dausize2 != "ALL")
                    {
                        foreach (DataRow row1 in tblSL.Rows)
                        {
                            string mau1 = row1["MaMau"].ToString();
                            string dausize1 = row1["DauSizeID"].ToString();
                            string size = row1["SizeID"].ToString().Trim().Replace(" ", "");
                            int soLuong = Convert.ToInt32(row1["SoLuong"]);
                            if (mau1 == mau2 && dausize1 == dausize2)
                            {
                                totalQuantity += soLuong;
                            }
                        }
                    }
                    // Trường hợp 3: MaMau có, DauSizeID = "ALL" và Size có
                    else if (validSizes != "ALL" && mau2 != "ALL" && dausize2 == "ALL")
                    {
                        foreach (DataRow row1 in tblSL.Rows)
                        {
                            string mau1 = row1["MaMau"].ToString();
                            string dausize1 = row1["DauSizeID"].ToString();
                            string size = row1["SizeID"].ToString().Trim().Replace(" ", "");
                            int soLuong = Convert.ToInt32(row1["SoLuong"]);
                            if (mau1 == mau2 && validSizes.Split(';').Contains(size))
                            {
                                totalQuantity += soLuong;
                            }
                        }
                    }
                    // Trường hợp 4: MaMau có, DauSizeID = "ALL" và Size = "ALL"
                    else if (validSizes == "ALL" && mau2 != "ALL" && dausize2 == "ALL")
                    {
                        foreach (DataRow row1 in tblSL.Rows)
                        {
                            string mau1 = row1["MaMau"].ToString();
                            string dausize1 = row1["DauSizeID"].ToString();
                            string size = row1["SizeID"].ToString().Trim().Replace(" ", "");
                            int soLuong = Convert.ToInt32(row1["SoLuong"]);
                            if (mau1 == mau2)
                            {
                                totalQuantity += soLuong;
                            }
                        }
                    }
                    // Trường hợp 5: MaMau = "ALL", DauSizeID = "ALL" và Size = "ALL"
                    else if (validSizes == "ALL" && mau2 == "ALL" && dausize2 == "ALL")
                    {
                        foreach (DataRow row1 in tblSL.Rows)
                        {
                            int soLuong = Convert.ToInt32(row1["SoLuong"]);
                            totalQuantity += soLuong; 
                        }
                    }
                    // Trường hợp 6: MaMau = "ALL", DauSizeID = "ALL" và Size có
                    else if (validSizes != "ALL" && mau2 == "ALL" && dausize2 == "ALL")
                    {
                        foreach (DataRow row1 in tblSL.Rows)
                        {
                            string size = row1["SizeID"].ToString().Trim().Replace(" ", "");
                            int soLuong = Convert.ToInt32(row1["SoLuong"]);
                            if (validSizes.Split(';').Contains(size))
                            {
                                totalQuantity += soLuong;
                            }
                        }
                    }
                    // Trường hợp 7: MaMau = "ALL", DauSizeID có và Size có
                    else if (validSizes != "ALL" && mau2 == "ALL" && dausize2 != "ALL")
                    {
                        foreach (DataRow row1 in tblSL.Rows)
                        {
                            string mau1 = row1["MaMau"].ToString();
                            string dausize1 = row1["DauSizeID"].ToString();
                            string size = row1["SizeID"].ToString().Trim().Replace(" ", "");
                            int soLuong = Convert.ToInt32(row1["SoLuong"]);
                            if (dausize1 == dausize2 && validSizes.Split(';').Contains(size))
                            {
                                totalQuantity += soLuong;
                            }
                        }
                    }
                    else
                    {
                        // Trường hợp 8: MaMau = "ALL", DauSizeID có và Size = "ALL"
                        if (mau2 == "ALL" && dausize2 != "ALL" && validSizes == "ALL")
                        {
                            foreach (DataRow row1 in tblSL.Rows)
                            {
                                string mau1 = row1["MaMau"].ToString();
                                string dausize1 = row1["DauSizeID"].ToString();
                                int soLuong = Convert.ToInt32(row1["SoLuong"]);
                                if (dausize1 == dausize2)
                                {
                                    totalQuantity += soLuong;
                                }
                            }
                        }
                    }    
                    row2["SoLuong"] = totalQuantity;
                    row2["MaDH"] = maDH.ToString();
                    row2["MaLenh"] = lenhSX.ToString();
                    row2["MaLenhSanXuat"] = maLenhSX.ToString();
                    row2["CapPhat"] = Math.Round(totalQuantity * Convert.ToDouble(row2["DinhMuc"]), 4);
                }
                if(tblDMCD != null && tblDMCD.Rows.Count > 0)
                {
                    foreach (DataRow rowDM in tblDM.Rows)  
                    {
                        string maLenhSanXuat = rowDM["MaLenhSanXuat"].ToString(); 
                        string maMau = rowDM["MaMau"].ToString();  
                        string dauSize = rowDM["DauSizeID"].ToString();  
                        string size = rowDM["SizeID"].ToString().Trim();  
                        string idNPL = rowDM["ID_DM"].ToString(); 

                        foreach (DataRow rowDMCD in tblDMCD.Rows) 
                        {
                            string maLenhSanXuatCD = rowDMCD["MaLenhSanXuat"].ToString();  
                            string maMauCD = rowDMCD["MaMauLenh"].ToString(); 
                            string dauSizeCD = rowDMCD["DauSizeLenh"].ToString();  
                            string sizeCD = rowDMCD["SizeLenh"].ToString().Trim();  
                            string idNPLCD = rowDMCD["MaNPL"].ToString(); 

                            if (maLenhSanXuat == maLenhSanXuatCD && maMau == maMauCD && dauSize == dauSizeCD && size == sizeCD && idNPL == idNPLCD)
                            {
                                rowDM["TinhTrang"] = "Đã cân đối NPL";
                                rowDM["TrangThai"] = 1;
                                rowDM["SoLuong"] = rowDMCD["SoLuong"];
                                rowDM["CapPhat"] = rowDMCD["CapPhat"];
                                rowDM["CapThem"] = rowDMCD["CapThem"];
                                rowDM["GhiChu"] = rowDMCD["GhiChu"];
                            }
                        }
                    }
                }    
                gridControl3.DataSource = tblDM;
            }
            else
            {
                gridControl3.DataSource = null;
            }
            DataTable tblNewDM = tblDM.Clone();
            tblNewDM.Columns.Add("IsXetDuyet", typeof(string));
            foreach (DataRow row in tblDM.Rows)
            {
                DataRow newRow = tblNewDM.NewRow();
                foreach (DataColumn col in tblDM.Columns)
                {
                    newRow[col.ColumnName] = row[col.ColumnName];
                }
                if ( tblDMCD.Rows.Count>0)
                {
                    DataRow[] matchingRows = tblDMCD.Select(
                       $"MaLenhSanXuat = '{row["MaLenhSanXuat"]}' AND MaMauLenh = '{row["MaMau"]}' AND DauSizeLenh = '{row["DauSizeID"]}' AND SizeLenh = '{row["SizeID"]}' AND MaNPL = '{row["ID_DM"]}'"
                   );

                    if (matchingRows.Length > 0)
                    {
                        newRow["IsXetDuyet"] = matchingRows[0]["IsXetDuyet"];
                    }
                    else
                    {
                        newRow["IsXetDuyet"] = "False"; 
                    }
                }
                else 
                {
                    newRow["IsXetDuyet"] = "False";
                }
                tblNewDM.Rows.Add(newRow);

            }
            gridControl1.DataSource = tblNewDM;

        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            this.ActiveControl = this.button1;

            DataTable _tblSaveTemp = (gridView1.DataSource as DataView).Table;
            DataTable _tblSave = CreateTableSave();

            foreach (DataRow row in _tblSaveTemp.Rows)
            {
                DataRow _rowInSert = _tblSave.NewRow();
                _rowInSert["ID"] = -1;
                _rowInSert["MaDH"] = maGop.ToString();
                _rowInSert["MaLenhSanXuat"] = maLenhSX.ToString();
                _rowInSert["MaNPL"] = row["ID_DM"]?.ToString() ?? "";
                _rowInSert["MaVT"] = row["MaVatTu"]?.ToString() ?? "";
                _rowInSert["TenVT"] = row["TenVatTu"]?.ToString() ?? "";
                _rowInSert["MaMau"] = row["Mau"]?.ToString() ?? "";
                _rowInSert["KhoVai"] = row["KhoVai"]?.ToString() ?? "";
                _rowInSert["MaDV"] = row["DonViTinh"]?.ToString() ?? "";
                _rowInSert["DinhMuc"] = row["DinhMuc"] ?? 0.0;
                _rowInSert["SoLuong"] = row["SoLuong"] ?? 0;
                _rowInSert["CapPhat"] = row["CapPhat"] ?? 0.0;
                _rowInSert["CapThem"] = row["CapThem"] ?? 0.0;
                _rowInSert["TrangThai"] = 0;
                _rowInSert["GhiChu"] = row["GhiChu"] ?? "";
                _rowInSert["NguoiTao"] = GlobleData.UserName;
                _rowInSert["NguoiSua"] = "";
                _rowInSert["MaMauLenh"] = row["TenMau"];
                _rowInSert["DauSizeLenh"] = row["DauSize"];
                _rowInSert["SizeLenh"] = row["Size"];
                _rowInSert["MaBom"] = row["MaBom"];
                _rowInSert["MaVTMau"] = row["MaVTMau"]; 
                _tblSave.Rows.Add(_rowInSert);
            }
            string urlSaveDM = string.Format("{0}", URL + "CanDoiDonHangTong/PostDinhMuc");
            string savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, _tblSave); }).Result;
            if (string.Compare(savelistDM, "True") != 0)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi trong quá trình thực hiện. Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                this.Close();
            }    
        }

        private DataTable CreateTableSave()
        {
            DataTable _tblCreate = new DataTable();
            _tblCreate.Columns.Add("ID", typeof(int));
            _tblCreate.Columns.Add("MaDH", typeof(string));
            _tblCreate.Columns.Add("MaLenhSanXuat", typeof(string));
            _tblCreate.Columns.Add("MaNPL", typeof(string));
            _tblCreate.Columns.Add("MaVT", typeof(string));
            _tblCreate.Columns.Add("TenVT", typeof(string));
            _tblCreate.Columns.Add("MaMau", typeof(string));
            _tblCreate.Columns.Add("KhoVai", typeof(string));
            _tblCreate.Columns.Add("MaDV", typeof(string));
            _tblCreate.Columns.Add("DinhMuc", typeof(double));
            _tblCreate.Columns.Add("SoLuong", typeof(int));
            _tblCreate.Columns.Add("CapPhat", typeof(double));
            _tblCreate.Columns.Add("CapThem", typeof(double));
            _tblCreate.Columns.Add("ThuHoi", typeof(double));
            _tblCreate.Columns.Add("TrangThai", typeof(int));
            _tblCreate.Columns.Add("GhiChu", typeof(string));
            _tblCreate.Columns.Add("NguoiTao", typeof(string));
            _tblCreate.Columns.Add("NguoiSua", typeof(string));
            _tblCreate.Columns.Add("MaMauLenh", typeof(string));
            _tblCreate.Columns.Add("DauSizeLenh", typeof(string));
            _tblCreate.Columns.Add("SizeLenh", typeof(string));
            _tblCreate.Columns.Add("MaBom", typeof(string));
            _tblCreate.Columns.Add("MaVTMau", typeof(string));
            return _tblCreate;
        }

        private DataTable CreateTableSave1()
        {
            DataTable _tblCreate = new DataTable();
            _tblCreate.Columns.Add("ID", typeof(int));
            _tblCreate.Columns.Add("MaDH", typeof(string));
            _tblCreate.Columns.Add("MaLenhSanXuat", typeof(string));
            _tblCreate.Columns.Add("MaNPL", typeof(string));
            _tblCreate.Columns.Add("MaVT", typeof(string));
            _tblCreate.Columns.Add("TenVT", typeof(string));
            _tblCreate.Columns.Add("MaMau", typeof(string));
            _tblCreate.Columns.Add("KhoVai", typeof(string));
            _tblCreate.Columns.Add("MaDV", typeof(string));
            _tblCreate.Columns.Add("DinhMuc", typeof(double));
            _tblCreate.Columns.Add("SoLuong", typeof(int));
            _tblCreate.Columns.Add("CapPhat", typeof(double));
            _tblCreate.Columns.Add("CapThem", typeof(double));
            _tblCreate.Columns.Add("ThuHoi", typeof(double));
            _tblCreate.Columns.Add("TrangThai", typeof(int));
            _tblCreate.Columns.Add("GhiChu", typeof(string));
            _tblCreate.Columns.Add("NguoiTao", typeof(string));
            _tblCreate.Columns.Add("NguoiSua", typeof(string));
            _tblCreate.Columns.Add("MaMauLenh", typeof(string));
            _tblCreate.Columns.Add("DauSizeLenh", typeof(string));
            _tblCreate.Columns.Add("SizeLenh", typeof(string));
            _tblCreate.Columns.Add("MaBom", typeof(string));
            _tblCreate.Columns.Add("IsXetDuyet", typeof(bool));
            _tblCreate.Columns.Add("NguoiDuyet", typeof(string));
            _tblCreate.Columns.Add("NguoiHuy", typeof(string));
            return _tblCreate;
        }


        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            DataRow _rowFocused = gridView1.GetFocusedDataRow();
            if (_rowFocused == null) return;
            string focusedColumn = gridView1.FocusedColumn.FieldName;
            if (focusedColumn == "CapThem")
            {
                if (_rowFocused["CapPhat"].ToString() == "0")
                {
                    XtraMessageBox.Show("Chưa được cấp phát, vui lòng cấp phát.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    e.Cancel = true;
                }
            }
            if (_rowFocused["IsXetDuyet"].ToString() == "True")
            {
                e.Cancel = true; 
            }

        }

        private void gridView2_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridView3_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }

        private void gridView2_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;

        }
        private void gridView2_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                var trangThai = gridView1.GetRowCellValue(e.RowHandle, "TrangThai");
                if (trangThai != null && Convert.ToInt32(trangThai) == 1)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 239, 204);
                }
                
            }
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {

            DataRow _rowFocus = gridView1.GetFocusedDataRow();
            if (_rowFocus == null || e.Menu == null) return;
            DXMenuItem menuCanDoiDMNL = new DXMenuItem();
            menuCanDoiDMNL.Caption = "Xóa dòng";
            menuCanDoiDMNL.Appearance.Font = new Font("Arial", 9);
            menuCanDoiDMNL.Click += MenuCanDoiDMNL;
            e.Menu.Items.Add(menuCanDoiDMNL);

            DXMenuItem menuDuyet = new DXMenuItem();
            menuDuyet.Caption = "Duyệt Dòng";
            menuDuyet.Appearance.Font = new Font("Arial", 9);
            menuDuyet.Click += MenuDuyetDong;
            menuDuyet.Enabled = _allowEdit;
            e.Menu.Items.Add(menuDuyet);

            DXMenuItem menuHuy = new DXMenuItem();
            menuHuy.Caption = "Hủy Dòng";
            menuHuy.Appearance.Font = new Font("Arial", 9);
            menuHuy.Click += MenuHuyDong;
            menuHuy.Enabled = _allowEdit;
            e.Menu.Items.Add(menuHuy);
        }
        private void MenuCanDoiDMNL(object sender, EventArgs e)
        {
            DataRow _rowFocus = gridView1.GetFocusedDataRow();
            if (_rowFocus != null)
            {
                int focusedRowHandle = gridView1.FocusedRowHandle;
                gridView1.DeleteRow(focusedRowHandle);
                
            }
        }
        private void MenuDuyetDong(object sender, EventArgs e)
        {
            int focusedRowHandle = gridView1.FocusedRowHandle;
            DataRow row = gridView1.GetFocusedDataRow();
            DataTable _tblSave = CreateTableSave1();
            DataRow _rowInSert = _tblSave.NewRow();
            _rowInSert["ID"] = -1;
            _rowInSert["MaDH"] = maGop.ToString();
            _rowInSert["MaLenhSanXuat"] = maLenhSX.ToString();
            _rowInSert["MaNPL"] = row["ID_DM"]?.ToString() ?? "";
            _rowInSert["MaVT"] = row["MaVatTu"]?.ToString() ?? "";
            _rowInSert["TenVT"] = row["TenVatTu"]?.ToString() ?? "";
            _rowInSert["MaMau"] = row["Mau"]?.ToString() ?? "";
            _rowInSert["KhoVai"] = row["KhoVai"]?.ToString() ?? "";
            _rowInSert["MaDV"] = row["DonViTinh"]?.ToString() ?? "";
            _rowInSert["DinhMuc"] = row["DinhMuc"] ?? 0.0;
            _rowInSert["SoLuong"] = row["SoLuong"] ?? 0;
            _rowInSert["CapPhat"] = row["CapPhat"] ?? 0.0;
            _rowInSert["CapThem"] = row["CapThem"] ?? 0.0;
            _rowInSert["TrangThai"] = 0;
            _rowInSert["GhiChu"] = row["GhiChu"] ?? "";
            _rowInSert["NguoiTao"] = GlobleData.UserName;
            _rowInSert["NguoiSua"] = "";
            _rowInSert["MaMauLenh"] = row["TenMau"];
            _rowInSert["DauSizeLenh"] = row["DauSize"];
            _rowInSert["SizeLenh"] = row["Size"];
            _rowInSert["MaBom"] = row["MaBom"];
            _rowInSert["IsXetDuyet"] = 1;
            _rowInSert["NguoiSua"] = GlobleData.UserName;
            _rowInSert["NguoiHuy"] = GlobleData.UserName;
            _tblSave.Rows.Add(_rowInSert);
            string urlSaveDM = string.Format("{0}", URL + "Duyet/PostDuyet");
            string savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, _tblSave); }).Result;
            if (string.Compare(savelistDM, "True") != 0)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi trong quá trình thực hiện. Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                LoadData();
                return;

            }
        }
        private void MenuHuyDong(object sender, EventArgs e)
        {
            int focusedRowHandle = gridView1.FocusedRowHandle;
            DataRow row = gridView1.GetFocusedDataRow();
            DataTable _tblSave = CreateTableSave1();
            DataRow _rowInSert = _tblSave.NewRow();
            _rowInSert["ID"] = -1;
            _rowInSert["MaDH"] = maGop.ToString();
            _rowInSert["MaLenhSanXuat"] = maLenhSX.ToString();
            _rowInSert["MaNPL"] = row["ID_DM"]?.ToString() ?? "";
            _rowInSert["MaVT"] = row["MaVatTu"]?.ToString() ?? "";
            _rowInSert["TenVT"] = row["TenVatTu"]?.ToString() ?? "";
            _rowInSert["MaMau"] = row["Mau"]?.ToString() ?? "";
            _rowInSert["KhoVai"] = row["KhoVai"]?.ToString() ?? "";
            _rowInSert["MaDV"] = row["DonViTinh"]?.ToString() ?? "";
            _rowInSert["DinhMuc"] = row["DinhMuc"] ?? 0.0;
            _rowInSert["SoLuong"] = row["SoLuong"] ?? 0;
            _rowInSert["CapPhat"] = row["CapPhat"] ?? 0.0;
            _rowInSert["CapThem"] = row["CapThem"] ?? 0.0;
            _rowInSert["TrangThai"] = 0;
            _rowInSert["GhiChu"] = row["GhiChu"] ?? "";
            _rowInSert["NguoiTao"] = GlobleData.UserName;
            _rowInSert["NguoiSua"] = "";
            _rowInSert["MaMauLenh"] = row["TenMau"];
            _rowInSert["DauSizeLenh"] = row["DauSize"];
            _rowInSert["SizeLenh"] = row["Size"];
            _rowInSert["MaBom"] = row["MaBom"];
            _rowInSert["IsXetDuyet"] = 0;
            _rowInSert["NguoiSua"] = GlobleData.UserName;
            _rowInSert["NguoiHuy"] = GlobleData.UserName;
            _tblSave.Rows.Add(_rowInSert);
            string urlSaveDM = string.Format("{0}", URL + "Duyet/PostHuy");
            string savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, _tblSave); }).Result;
            if (string.Compare(savelistDM, "True") != 0)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi trong quá trình thực hiện. Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                LoadData();
                return;

            }

        }

        private void btnCapThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string MaDh = txtMaDH.EditValue.ToString();
            string Lenh = txtLenhSX.EditValue.ToString();
            frmCapThem frm = new frmCapThem(mg, MaDh, Lenh);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            LoadData();
        }

        private void Duyet()
        {
            this.ActiveControl = this.button1;
            string urlDMCD = string.Format("{0}?madh={1}&&maLenhSX={2}", URL + "CanDoiDMNL/GetDinhMucCD", maGop, maLenhSX);
            string jsonDMCD = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDMCD); }).Result;
            DataTable _tblSaveTemp = JsonConvert.DeserializeObject<DataTable>(jsonDMCD);

            DataTable _tblSave = CreateTableSave1();

            foreach (DataRow row in _tblSaveTemp.Rows)
            {
                DataRow _rowInSert = _tblSave.NewRow();
                _rowInSert["ID"] = -1;
                _rowInSert["MaDH"] = maGop.ToString();
                _rowInSert["MaLenhSanXuat"] = maLenhSX.ToString();
                _rowInSert["MaNPL"] = row["MaNPL"]?.ToString() ?? "";
                _rowInSert["MaVT"] = row["MaVT"]?.ToString() ?? "";
                _rowInSert["TenVT"] = row["TenVT"]?.ToString() ?? "";
                _rowInSert["MaMau"] = row["MaMau"]?.ToString() ?? "";
                _rowInSert["KhoVai"] = row["KhoVai"]?.ToString() ?? "";
                _rowInSert["MaDV"] = row["MaDV"]?.ToString() ?? "";
                _rowInSert["DinhMuc"] = row["DinhMuc"] ?? 0.0;
                _rowInSert["SoLuong"] = row["SoLuong"] ?? 0;
                _rowInSert["CapPhat"] = row["CapPhat"] ?? 0.0;
                _rowInSert["CapThem"] = row["CapThem"] ?? 0.0;
                _rowInSert["TrangThai"] = 0;
                _rowInSert["GhiChu"] = row["GhiChu"] ?? "";
                _rowInSert["NguoiTao"] = GlobleData.UserName;
                _rowInSert["NguoiSua"] = "";
                _rowInSert["MaMauLenh"] = row["MaMauL"];
                _rowInSert["DauSizeLenh"] = row["DauSizeL"];
                _rowInSert["SizeLenh"] = row["SizeL"];
                _rowInSert["MaBom"] = row["MaBom"];
                _rowInSert["IsXetDuyet"] = 1;
                _rowInSert["NguoiDuyet"] = GlobleData.UserName;
                _rowInSert["NguoiHuy"] = "";
                _tblSave.Rows.Add(_rowInSert);
            }
            string urlSaveDM = string.Format("{0}", URL + "Duyet/PostDuyet");
            string savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, _tblSave); }).Result;
            if (string.Compare(savelistDM, "True") != 0)
            {
                XtraMessageBox.Show("Chưa Cân Đối NPL.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                LoadData();
                return;

            }
        }

        private void HuyDuyet()
        {
            this.ActiveControl = this.button1;
            string urlDMCD = string.Format("{0}?madh={1}&&maLenhSX={2}", URL + "CanDoiDMNL/GetDinhMucCD", maGop, maLenhSX);
            string jsonDMCD = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDMCD); }).Result;
            DataTable _tblSaveTemp = JsonConvert.DeserializeObject<DataTable>(jsonDMCD);

            DataTable _tblSave = CreateTableSave1();

            foreach (DataRow row in _tblSaveTemp.Rows)
            {
                DataRow _rowInSert = _tblSave.NewRow();
                _rowInSert["ID"] = -1;
                _rowInSert["MaDH"] = maGop.ToString();
                _rowInSert["MaLenhSanXuat"] = maLenhSX.ToString();
                _rowInSert["MaNPL"] = row["MaNPL"]?.ToString() ?? "";
                _rowInSert["MaVT"] = row["MaVT"]?.ToString() ?? "";
                _rowInSert["TenVT"] = row["TenVT"]?.ToString() ?? "";
                _rowInSert["MaMau"] = row["MaMau"]?.ToString() ?? "";
                _rowInSert["KhoVai"] = row["KhoVai"]?.ToString() ?? "";
                _rowInSert["MaDV"] = row["MaDV"]?.ToString() ?? "";
                _rowInSert["DinhMuc"] = row["DinhMuc"] ?? 0.0;
                _rowInSert["SoLuong"] = row["SoLuong"] ?? 0;
                _rowInSert["CapPhat"] = row["CapPhat"] ?? 0.0;
                _rowInSert["CapThem"] = row["CapThem"] ?? 0.0;
                _rowInSert["TrangThai"] = 0;
                _rowInSert["GhiChu"] = row["GhiChu"] ?? "";
                _rowInSert["NguoiTao"] = GlobleData.UserName;
                _rowInSert["NguoiSua"] = "";
                _rowInSert["MaMauLenh"] = row["MaMauL"];
                _rowInSert["DauSizeLenh"] = row["DauSizeL"];
                _rowInSert["SizeLenh"] = row["SizeL"];
                _rowInSert["MaBom"] = row["MaBom"];
                _rowInSert["IsXetDuyet"] = 0;
                _rowInSert["NguoiDuyet"] = GlobleData.UserName;
                _rowInSert["NguoiHuy"] = GlobleData.UserName;
                _tblSave.Rows.Add(_rowInSert);
            }
            string urlSaveDM = string.Format("{0}", URL + "Duyet/PostHuy");
            string savelistDM = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDM, _tblSave); }).Result;
            if (string.Compare(savelistDM, "True") != 0)
            {
                XtraMessageBox.Show("Chưa Cân Đối NPL.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                LoadData();
                return;

            }
            
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Duyet();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            HuyDuyet();
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.Column == gridColumnTrangThaiDuyet)
            {
                object isXetDuyetValue = e.Value;
                if (isXetDuyetValue != null && bool.TryParse(isXetDuyetValue.ToString(), out bool isXetDuyet))
                {
                    e.DisplayText = isXetDuyet ? "Đã Duyệt" : "Chưa Duyệt";
                   
                }
                else
                {
                    return;
                }
                
            }

        }

        private void repositoryItemCheckEdit1_Click(object sender, EventArgs e)
        {

        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            //CheckEdit checkEdit = sender as CheckEdit;

            //GridView view = gridControl1.FocusedView as GridView;

            //int rowHandle = view.FocusedRowHandle;


            //DataRow focusedRow = view.GetDataRow(rowHandle); // Lấy DataRow của dòng hiện tại
            //if (focusedRow != null)
            //{
            //    bool isChecked = checkEdit.Checked;

            //    if (isChecked)
            //    {
            //        DuyetTheoDong();
            //    }
            //    else
            //    {
            //        HuyTheoDong();
            //    }

            //}
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            //GridView view = sender as GridView;

            //if (e.Column.FieldName == "IsXetDuyet")
            //{
            //    string isXetDuyet = view.GetRowCellValue(e.RowHandle, "IsXetDuyet")?.ToString();
            //    if (isXetDuyet == "True")
            //    {
            //        e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit()
            //        {
            //            ReadOnly = true
            //        };
            //    }
            //}
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url = string.Format("{0}?madh={1}&&malenh={2}", URL + "Duyet/GetEx", maGop, maLenhSX);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                if (dtTable.Rows.Count > 0)
                {
                    SaveFileDialog Sfd = new SaveFileDialog();
                    Sfd.Title = "File To Save";
                    Sfd.Filter = "Excel | *.xlsx";
                    Sfd.FileName = string.Format("CPNPL_{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), dtTable.Rows[0]["MaHang"].ToString(), dtTable.Rows[0]["TenDVSX"].ToString());
                    if (dtTable.Rows.Count == 0 && dtTable == null) return;
                    if (Sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                            DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                            op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                            op.ShowGridLines = true;
                            op.SheetName = string.Format("Bao Cao");

                            string fileName = "CP411-21-0038- 603970.xlsx";
                            string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                            string TemplateFileName = path;
                            string ExportFileName = Sfd.FileName;

                            ExportExcel(TemplateFileName, ExportFileName, dtTable);
                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                        }
                    }
                }
                else 
                {
                    XtraMessageBox.Show("Chưa Cân Đối.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ExportExcel(string TemplateFileName, string ExportFileName, DataTable dtTable)
        {
            try
            {
                _SLTong = dtTable.AsEnumerable().Sum(x => Convert.ToInt32(x["SoLuong"]));
                string date = DateTime.Now.ToString("dd MM yyyy");
                string[] dateParts = date.Split(' ');

                string day = dateParts[0]; // Ngày
                string month = dateParts[1]; // Tháng
                string year = dateParts[2]; // Năm

                //Lấy cột po tác ra mảng
                var poValues = dtTable.AsEnumerable().Select(row => row.Field<string>("po")).Distinct().ToArray();

                // Chuyển mảng thành chuỗi bằng cách nối các giá trị với dấu phẩy ","
                string concatenatedString = string.Join(",", poValues);

                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    string templateFilePath = TemplateFileName;

                    string resultFilePath = ExportFileName;
                    FileInfo templateFile = new FileInfo(templateFilePath);
                    ExcelPackage templatePackage = new ExcelPackage(templateFile);
                    ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets[0];
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    int index = 1;
                    int row = 13;
                    Decimal sumSL_CapPhap = 0;
                    for (int i = 10; i < 50; i++)
                    {
                        worksheet.Row(i).Height = 18;
                    }
                    foreach (DataRow item in dtTable.Rows)
                    {
                        sumSL_CapPhap += Convert.ToDecimal(item["CapPhat"]);
                        worksheet.Cells[row, 1].Value = index;
                        worksheet.Cells[row, 2].Value = item["MaVT"].ToString();
                        worksheet.Cells[row, 3].Value = item["TenVT"].ToString();
                        worksheet.Cells[row, 4].Value = item["TenMau"].ToString();
                        worksheet.Cells[row, 5].Value = item["KhoVai"].ToString();
                        worksheet.Cells[row, 6].Value = item["TenDV"].ToString();
                        worksheet.Cells[row, 8].Value = item["SoLuong"].ToString();
                        worksheet.Cells[row, 7].Value = Convert.ToDecimal(item["DinhMuc"]);
                        worksheet.Cells[row, 9].Value = Convert.ToDecimal(item["CapPhat"]);
                        worksheet.Cells[row, 10].Value = item["GhiChu"].ToString();
                        worksheet.Column(3).Width = 70; worksheet.Column(3).Style.WrapText = true;
                        worksheet.Column(4).AutoFit(); worksheet.Column(6).AutoFit();
                        worksheet.Row(row).Height = -1;
                        index++;
                        row++;

                    }
                    worksheet.Cells["C6"].Value = dtTable.Rows[0]["TenHang"].ToString();
                    worksheet.Cells["C7"].Value = dtTable.Rows[0]["TenDVSX"].ToString();
                    worksheet.Cells["C8"].Value = dtTable.Rows[0]["TenKH"].ToString();
                    worksheet.Cells["C9"].Value = concatenatedString;
                    worksheet.Cells["C10"].Value = dtTable.Rows[0]["Dot"].ToString();

                    worksheet.Cells["I7"].Value = _SLTong + " PCS";
                    worksheet.Cells["I8"].Value = sumSL_CapPhap + " PCS";

                    range = worksheet.Cells[row, 3]; range.Value = "Tổng cộng"; range.Style.Font.Bold = true;

                    range = worksheet.Cells[row, 9]; range.Value = sumSL_CapPhap; range.Style.Font.Bold = true;


                    var borderData = worksheet.Cells[13, 1, row, 10].Style.Border;
                    borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = ExcelBorderStyle.Thin;
                    row += 2;
                    worksheet.Row(row + 1).Height = 18;
                    range = worksheet.Cells[row, 8, row, 9]; range.Value = "Ngày " + day + "Tháng " + month + "Năm " + year; range.Merge = true;
                    range = worksheet.Cells[row + 1, 3]; range.Value = "Người lập/Prepared By"; range.Merge = true;
                    range = worksheet.Cells[row + 1, 8, row + 1, 9]; range.Value = "PHÒNG KH - KD"; range.Style.Font.Size = 12; range.Merge = true; range.Style.Font.Bold = true; range.Style.Font.Italic = false;


                    FileInfo resultFile = new FileInfo(resultFilePath);
                    templatePackage.SaveAs(resultFile);
                }

            }
            catch (Exception EE)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lưu file!");
                return;
            }

        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;

            // Kiểm tra cột đang được vẽ có phải là "gridColumnTrangThaiDuyet" không
            if (e.Column == gridColumnTrangThaiDuyet)
            {
                object isXetDuyetValue = view.GetRowCellValue(e.RowHandle, e.Column);
                if (isXetDuyetValue != null && bool.TryParse(isXetDuyetValue.ToString(), out bool isXetDuyet))
                {
                    // Thay đổi màu chữ tùy thuộc vào trạng thái
                    if (isXetDuyet)
                    {
                        e.Appearance.ForeColor = Color.Green; 
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Red; 
                    }
                }
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void CheckPerminsion()
        {
            HttpClientExtension _clientextension = new HttpClientExtension();
            string url = string.Format("{0}/GetUser?userID={1}", URL + ResourceURL.UrlUserDuyet, GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientextension.GetAsnyc(url); }).Result;
            List<SystemUserDuyetConfigEntity> List = JsonConvert.DeserializeObject<List<SystemUserDuyetConfigEntity>>(json);
            SystemUserDuyetConfigEntity obj = (from m in List
                                          where m.UserID == GlobleData.UserName
                                               select m).FirstOrDefault();
            if (obj == null) return;
            _allowEdit = obj.AllowEdit;
            this.barButtonItem1.Enabled = _allowEdit;
            this.barButtonItem2.Enabled = _allowEdit;
        }
    }
}
