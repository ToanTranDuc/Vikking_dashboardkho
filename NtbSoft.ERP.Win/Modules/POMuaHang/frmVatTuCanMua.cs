using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmVatTuCanMua : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataTable gridControl1Table;
        private DataTable vattuTable;

        public frmVatTuCanMua()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            getVatTuCanMua();
            calcVatTuCanMua();
            loadGridControl1();

            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
        }



        private void loadGridControl1()
        {

            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView1.ExpandAllGroups();
        }
        private void getVatTuCanMua()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getvattucanmua";
            string urlGetListDataTable = URL + "VatTuMinMax/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!vattuTable.Columns.Contains("generatedID"))
                vattuTable.Columns.Add("generatedID");
            if (!vattuTable.Columns.Contains("IsNPLText"))
                vattuTable.Columns.Add("IsNPLText", typeof(string));
            if (!vattuTable.Columns.Contains("haveBought"))
                vattuTable.Columns.Add("haveBought", typeof(string));
            if (!vattuTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                vattuTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in vattuTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";

                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();

                row["haveBought"] = row["MaPhieuMH"] == null || row["MaPhieuMH"].ToString() == "" ? "Chưa tạo phiếu mua hàng" : "Đã tạo phiếu mua hàng";
            }
        }
        private void calcVatTuCanMua()
        {
            DataTable dt = vattuTable.Copy();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            foreach (DataRow row in dt.Rows)
            {
                row["TonToiThieu"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiThieu"].ToString());
                row["TonToiDa"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiDa"].ToString());
                row["TonKho"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho"].ToString());
            }
            gridControl1Table = dt.Copy();
        }
        public string padToThree(string input)
        {
            // Nếu input null thì trả về rỗng
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Dùng PadLeft để thêm '0' cho đủ 3 ký tự
            return input.PadLeft(3, '0');
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            // Vẽ border quanh cột header
            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            // Vẽ text trong vùng caption
            TextRenderer.DrawText(e.Graphics, e.Info.Caption, e.Appearance.Font,
                                  e.Info.Bounds, Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            e.Handled = true;
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "TonToiThieu")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonToiDa")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
        }
        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info != null)
            {
                string originalText = info.GroupText;
                string[] parts = originalText.Split(new string[] { "--" }, StringSplitOptions.None);

                if (parts.Length > 1)
                {
                    info.GroupText = parts[1].Trim();
                }

                e.Painter.DrawObject(info);
                e.Handled = true;
            }
        }

        private void resetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            try
            {
                getVatTuCanMua();
                calcVatTuCanMua();
                loadGridControl1();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                SplashScreenManager.CloseDefaultWaitForm();
            }
            finally
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
        }

        private void muaBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridView1;
            int focusedHandle = view.FocusedRowHandle;

            DataTable dt = CreateDatatable();
            dt.Columns.Add("SLMuaThem");
            DataRow focusedRow;

            if (focusedHandle >= 0)
            {
                focusedRow = view.GetDataRow(focusedHandle);
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn vật tư cần tạo phiếu mua hàng", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                //if (focusedRow != null)
                //{
                //    if (focusedRow["haveBought"].ToString() == "Đã tạo phiếu mua hàng")
                //    {
                //        XtraMessageBox.Show("Đã tạo phiếu mua hàng", "Thông báo",
                //                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return;
                //    }

                //    DataRow newRow = dt.NewRow();
                //    newRow["MaVTID"] = focusedRow["MaVTID"];
                //    newRow["MauVTID"] = focusedRow["MauVTID"];
                //    newRow["KhoVaiID"] = focusedRow["KhoVaiID"];
                //    newRow["MaDVVT"] = focusedRow["MaDVVT"];
                //    newRow["ItemCode"] = focusedRow["MaVT"];
                //    newRow["NPL"] = focusedRow["IsNPL"];
                //    newRow["DonGia"] = 0;
                //    newRow["TongSL"] = 0;
                //    decimal sltoithieu = XuLyVTUnits.SmartTryParse<decimal>(focusedRow["TonToiThieu"].ToString());
                //    decimal sltonkho = XuLyVTUnits.SmartTryParse<decimal>(focusedRow["TonKho"].ToString());
                //    newRow["TongSLMuaThem"] = sltoithieu - sltonkho;
                //    newRow["ThanhTien"] = 0;
                //    newRow["MoTa"] = focusedRow["ChiTiet"];
                //    newRow["MaCLVT"] = focusedRow["MaNhom"];
                //    newRow["KhoVai"] = focusedRow["KhoVai"];
                //    newRow["TenDVVT"] = focusedRow["TenDVVT"];
                //    newRow["MauVT"] = focusedRow["MauVT"];
                //    newRow["TenCL"] = focusedRow["ChungLoaiVatTu"];
                //    newRow["Thue"] = 0;
                //    newRow["PTThanhToan"] = "";
                //    newRow["ChiPhiVanChuyen"] = 0;
                //    newRow["ChiPhiKhac"] = "";
                //    newRow["PTVanChuyen"] = 0;
                //    newRow["NgayDuKienHV"] = "";
                //    newRow["GhiChu"] = "";
                //    newRow["ChiPhiVT"] = 0;
                //    newRow["NgayHieuLuc"] = DBNull.Value;
                //    newRow["TienTeID"] = "";
                //    newRow["MaTienTe"] = "";
                //    newRow["SoNgayGHSom"] = DBNull.Value;
                //    newRow["SoNgayGHTre"] = DBNull.Value;
                //    newRow["ColorCode"] = focusedRow["MaMauVT"];
                //    newRow["ChietKhau"] = 0;
                //    newRow["ThanhTienVND"] = 0;
                //    newRow["ChiPhiSauCK"] = 0;
                //    newRow["MaThue_Gop"] = "";
                //    newRow["MaChietKhau_Gop"] = "";
                //    //newRow.ItemArray = focusedRow.ItemArray;
                //    dt.Rows.Add(newRow);
                //    frmPhieuMuaHang frm = new frmPhieuMuaHang(null, false, false, false, dt, true);
                //    frm.ShowDialog();
                //    frm.MaximizeBox = true;
                //    if (frm.DialogResult == DialogResult.OK)
                //    {
                //        loadGridControl1();
                //    }
                //}
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("ItemCode", typeof(string));
            tbl.Columns.Add("NPL", typeof(bool));
            tbl.Columns.Add("DonGia", typeof(float));
            tbl.Columns.Add("TongSL", typeof(float));
            tbl.Columns.Add("TongSLMuaThem", typeof(float));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("MoTa", typeof(string));
            tbl.Columns.Add("MaCLVT", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("TenCL", typeof(string));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("ChiPhiVanChuyen", typeof(string));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("PTVanChuyen", typeof(string));
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("TGGiao", typeof(DateTime));
            tbl.Columns.Add("ChiPhiVT", typeof(decimal));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("MaTienTe", typeof(string));
            tbl.Columns.Add("SoNgayGHSom", typeof(int));
            tbl.Columns.Add("SoNgayGHTre", typeof(int));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("ThanhTienVND", typeof(decimal));
            tbl.Columns.Add("ChiPhiSauCK", typeof(decimal));
            tbl.Columns.Add("MaThue_Gop", typeof(string));
            tbl.Columns.Add("MaChietKhau_Gop", typeof(string));
            return tbl;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var frm = new frmTongQuanVT())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                }
            }
        }
    }
}