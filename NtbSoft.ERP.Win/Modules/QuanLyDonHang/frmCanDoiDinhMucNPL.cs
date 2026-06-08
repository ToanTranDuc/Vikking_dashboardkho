using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Win.Modules.CanDoiDonHang;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDinhMucNPL : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _madh = string.Empty, _mahang = string.Empty, _tenhang = string.Empty, _malenh = string.Empty, _tenlenh = string.Empty, _malenhsanxuat = string.Empty, _dot = string.Empty, _madvsx = string.Empty, _tendvsx = string.Empty, _po = string.Empty, _magop = string.Empty;
        private int _total = 0;
        DataTable tblNPL;
        DataRow _drRow;
        DataTable _dtData;
        DataTable tblNPLCT;
        KeyDownControlHandler keyDownControlHandler;

        public frmCanDoiDinhMucNPL(DataRow _dr, DataTable dtData)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblNPL = new DataTable();
            tblNPLCT = new DataTable();
            _dtData = new DataTable();
            this._madh = _dr["MaDH"].ToString();
            this._mahang = _dr["MaHang"].ToString();
            this._tenhang = _dr["TenHang"].ToString();
            this._malenh = _dr["MaLenh"].ToString();
            this._tenlenh = _dr["TenLenh"].ToString();
            this._malenhsanxuat = _dr["MaLenhSanXuat"].ToString();
            this._dot = _dr["DotSX"].ToString();
            this._madvsx = _dr["MaDVSX"].ToString();
            this._tendvsx = _dr["TenDVSX"].ToString();
            // this._total = total;
            //this._po = po;
            this._magop = _dr["MaGop"].ToString();
            _drRow = _dr;
            _dtData = dtData;
        }
        protected override void OnLoad(EventArgs e)
        {
            List<DataRow> lst = _dtData.AsEnumerable()
                   .Where(x => x["MaLenh"].ToString() == _malenh.ToString())
                   .ToList();
            List<string> lstPO = lst.AsEnumerable()
                            .Select(x => x["PO"].ToString())
                            .ToList();
            int total = _dtData.AsEnumerable().Sum(x => Convert.ToInt32(x["SoLuong"]));
            txtDH.Text = _madh;
            txtTH.Text = _tenhang;
            txtMaLenhSX.Text = _malenh;
            txtTenLenh.Text = _tenlenh;
            txtDVSX.Text = _tendvsx;
            txtDot.Text = _dot;
            txtSL.Text = _total.ToString();
            txtPO.Text = string.Join(";", lstPO.Distinct());
            txtSL.Text = total.ToString();
            LoadData();
            TaoTable();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, Luu, true, ActionType.Save);

            return _lstActionControl;
        }


        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        private void TaoTable()
        {
            tblNPLCT = new DataTable("tblNPLCT");
            tblNPLCT.Columns.Add("ID", typeof(int));
            tblNPLCT.Columns.Add("MaNPL", typeof(string));
            tblNPLCT.Columns.Add("MaVT", typeof(string));
            tblNPLCT.Columns.Add("TenVT", typeof(string));
            tblNPLCT.Columns.Add("MaMau", typeof(string));
            tblNPLCT.Columns.Add("TenMau", typeof(string));
            tblNPLCT.Columns.Add("KhoVai", typeof(string));
            tblNPLCT.Columns.Add("MaDV", typeof(string));
            tblNPLCT.Columns.Add("TenDV", typeof(string));
            tblNPLCT.Columns.Add("DinhMuc", typeof(double));
            tblNPLCT.Columns.Add("CapPhat", typeof(decimal));
            tblNPLCT.Columns.Add("SoLuong", typeof(decimal));
            tblNPLCT.Columns.Add("GhiChu", typeof(string));
        }

        private void LoadData()
        {
            string url = string.Format("{0}?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetNPL", _magop, _malenhsanxuat);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblNPL = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl1.DataSource = tblNPL;
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GridView gridView = sender as GridView;
            //if (gridView1.OptionsSelection.MultiSelect)
            //{
            //    // Get the selected rows
            //    int[] selectedRows = gridView1.GetSelectedRows();
            //    if (selectedRows.Length == 1)
            //    {
            //        if (gridView != null && gridView.SelectedRowsCount > 0)
            //        {
            //            DataRow selectedRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //            if (selectedRow == null) return;
            //            selectedRow["CapPhat"] = Math.Round(Convert.ToDecimal(selectedRow["DinhMuc"]) * _total,4);
            //            tblNPLCT.Rows.Add(selectedRow.ItemArray);
            //            gridControl2.DataSource = tblNPLCT;
            //            gridControl1.RefreshDataSource();
            //            tblNPL.Rows.Remove(selectedRow);
            //        }

            //    }
            //    else
            //    {
            //        if (selectedRows.Length > 1)
            //        {
            //            foreach (DataRow row in tblNPL.Rows)
            //            {
            //                row["CapPhat"] = Convert.ToDouble(row["DinhMuc"]) * _total;
            //                tblNPLCT.ImportRow(row);

            //            }
            //            tblNPL.Clear();
            //            gridControl2.DataSource = tblNPLCT;
            //            gridControl1.RefreshDataSource();
            //        }
            //    }
            //}
        }

        private void gridView3_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GridView gridView = sender as GridView;
            //if (gridView1.OptionsSelection.MultiSelect)
            //{
            //    // Get the selected rows
            //    int[] selectedRows = gridView3.GetSelectedRows();
            //    if (selectedRows.Length == 1)
            //    {
            //        if (gridView != null && gridView.SelectedRowsCount > 0)
            //        {
            //            DataRow selectedRow = gridView3.GetDataRow(gridView3.FocusedRowHandle);
            //            if (selectedRow == null) return;
            //            if (selectedRow == null) return;
            //            selectedRow["CapPhat"] = selectedRow["SoLuong"];
            //            tblNPL.Rows.Add(selectedRow.ItemArray);
            //            gridControl1.DataSource = tblNPL;
            //            gridControl2.RefreshDataSource();
            //            tblNPLCT.Rows.Remove(selectedRow);
            //        }

            //    }
            //    else
            //    {
            //        if (selectedRows.Length > 1)
            //        {
            //            foreach (DataRow row in tblNPLCT.Rows)
            //            {
            //                tblNPL.ImportRow(row);

            //            }
            //            tblNPLCT.Clear();
            //            gridControl1.DataSource = tblNPL;
            //            gridControl2.RefreshDataSource();
            //        }
            //    }
            //}

        }

        private void Luu()
        {
            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                this.ActiveControl = this.button1;
                List<DinhMucSaveEntity> Listcandoinpl = new List<DinhMucSaveEntity>();

                if (tblNPL != null || tblNPL.Rows.Count > 0)
                {
                    for (int i = 0; i <= tblNPL.Rows.Count - 1; i++)
                    {
                        if (Convert.ToInt32(tblNPL.Rows[i][16]) == 2)
                        {
                            DinhMucSaveEntity addcandoinpl = new DinhMucSaveEntity();
                            addcandoinpl.ID = Convert.ToInt32(tblNPL.Rows[i][0]);
                            addcandoinpl.MaDH = tblNPL.Rows[i][1].ToString();
                            addcandoinpl.MaLenhSanXuat = tblNPL.Rows[i][2].ToString();
                            addcandoinpl.MaNPL = tblNPL.Rows[i][3].ToString();
                            addcandoinpl.MaVT = tblNPL.Rows[i][4].ToString();
                            addcandoinpl.TenVT = tblNPL.Rows[i][5].ToString();
                            addcandoinpl.MaMau = tblNPL.Rows[i][6].ToString();
                            addcandoinpl.KhoVai = tblNPL.Rows[i][8].ToString();
                            addcandoinpl.MaDV = tblNPL.Rows[i][9].ToString();
                            addcandoinpl.DinhMuc = Convert.ToDouble(tblNPL.Rows[i][11]);
                            addcandoinpl.CapPhat = Convert.ToDouble(tblNPL.Rows[i][13]);
                            addcandoinpl.CapThem = Convert.ToDouble(tblNPL.Rows[i][14]);
                            addcandoinpl.ThuHoi = Convert.ToDouble(tblNPL.Rows[i][15]);
                            addcandoinpl.TrangThai = 1;
                            addcandoinpl.GhiChu = tblNPL.Rows[i][17].ToString();
                            addcandoinpl.NguoiSua = GlobleData.UserName;
                            addcandoinpl.MaMauLenh = tblNPL.Rows[i][18].ToString();
                            addcandoinpl.DauSizeLenh = tblNPL.Rows[i][19].ToString();
                            addcandoinpl.SizeLenh = tblNPL.Rows[i][20].ToString();
                            addcandoinpl.MaBom = tblNPL.Rows[i][21].ToString();
                            Listcandoinpl.Add(addcandoinpl);
                        }
                    }
                    if (Listcandoinpl.Count > 0)
                    {
                        string urlPNPL = string.Format("{0}?", URL + "CanDoiDonHangTong/PostNPL");
                        string msPNPL = Task.Run(async () => { return await _clientExtension.PostAsync(urlPNPL, Listcandoinpl); }).Result;
                        if (msPNPL.ToLower() != "true")
                            XtraMessageBox.Show(msPNPL);
                    }
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void btnCanDoiNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCanDoiDinhMucNguyenLieu frm = new frmCanDoiDinhMucNguyenLieu(_drRow);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            LoadData();
        }

        private void btLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Luu();
        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridView3_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridView3_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e.Value == null || e.Value.ToString() == "") return;
            //if (e.Column.FieldName == "DinhMuc")
            //{
            //    DataRow dr = gridView3.GetFocusedDataRow();
            //    dr["CapPhat"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(e.Value) * Convert.ToInt32(txtSL.Text)), 4);
            //}
        }

        private void gridView3_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "DinhMuc")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Giá trị không được để trống!";
                }
            }
        }

        private void gridView3_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "DinhMuc")
            {
                // Tạo một RepositoryItemTextEdit
                var repositoryItemTextEdit = new RepositoryItemTextEdit();
                repositoryItemTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                repositoryItemTextEdit.Mask.EditMask = "n4"; // Cho phép nhập số thập phân với 2 chữ số sau dấu phẩy

                e.RepositoryItem = repositoryItemTextEdit;
            }
        }

        private void btNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Bạn có muốn xóa định mức lệnh sản xuất này không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/DeleteNPL", _magop, _malenhsanxuat);
                    string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            if (e.Value == null || e.Value == "") return;
            GridView view = (GridView)sender;
            DataRow drChange = view.GetFocusedDataRow();
            if (e.Column.FieldName == "CapPhat" || e.Column.FieldName == "ThuHoi" || e.Column.FieldName == "CapThem")
            {
                drChange["TrangThai"] = 2;
                //if (e.Column.FieldName == "DinhMuc")
                //{
                //    drChange["CapPhat"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(e.Value) * Convert.ToInt32(txtSL.Text)), 4);
                //}
                //if (e.Column.FieldName == "CapPhat")
                //{
                //    if (Convert.ToInt32(txtSL.Text) == 0)
                //    {
                //        MessageBox.Show("Số lượng tổng bằng 0. Không thể tính lại định mức.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return;
                //    }
                //    else
                //    {
                //        drChange["DinhMuc"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(e.Value) / Convert.ToInt32(txtSL.Text)), 4);
                //    }

                //}
            }
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName == "DinhMuc" && view.FocusedColumn.FieldName == "CapPhat" && view.FocusedColumn.FieldName == "CapThem" && view.FocusedColumn.FieldName == "ThuHoi")
            {
                if (Convert.ToInt32(e.Value) < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng phải lớn hơn 0";
                }
            }
        }
    }
}
