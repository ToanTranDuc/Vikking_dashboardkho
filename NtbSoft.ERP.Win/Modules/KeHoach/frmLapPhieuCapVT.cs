using DevExpress.XtraGrid.Views.Grid;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmLapPhieuCapVT : Form
    {
        GetDataService _service = new GetDataService();
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private string createUrlPhieuYeuCau(string action, string maphieu, string malenhSX)
        {
            return string.Format(@"{0}PhieuYeuCau/GetData?action={1}&&maphieu={2}&&malenhsx={3}", URL, action, maphieu, malenhSX);
        }
        private DataTable GetDataTable(string urlStr)
        {
            return Task.Run(
                async () => { return await _service.GetDataTable(urlStr); }
                ).Result;
        }
        private string PostDataTable(string urlStr, DataTable dataPost)
        {
            return Task.Run(
                async () => { return await _service.PostData(urlStr, dataPost); }
                ).Result;
        }
        private void load()
        {
            LoadDS_PhieuCapVatTu();
            Load_slueMaPhieu();
            try
            {
                if (slue_ChonPhieu.EditValue == null)
                    return;
                string malenhsx = slue_ChonPhieu.EditValue.ToString();
                gridControl2.DataSource = createDataCTPhieuCap();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        public frmLapPhieuCapVT()
        {
            InitializeComponent();
            URL = settingsReader.GetValue("URL", typeof(string)).ToString();
            tabCPage.SelectedTabPage = tabList;

            load();

        }
        private void ButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // Thực hiện hành động khi nhấn nút

        }
        private void LoadDS_PhieuCapVatTu()
        {
            try
            {
                string urlStr = URL + "PhieuCap/GetData?action=GET_PhieuCap&&maphieu=&&malenhsx=";
                DataTable result = GetDataTable(urlStr);
                slue_ListPhieuCapVatTu.Properties.DataSource = result;
                slue_ListPhieuCapVatTu.Properties.DisplayMember = "TenTQ";
                slue_ListPhieuCapVatTu.Properties.ValueMember = "MaPhieu";
                if (result != null && result.Rows.Count > 0)
                {
                    slue_ListPhieuCapVatTu.EditValue = result.Rows[0]["MaPhieu"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo ngoại lệ chưa được xử lí", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Load_slueMaPhieu()
        {
            string urlGet = createUrlPhieuYeuCau("GET_Phieu", "", "");
            DataTable searchLookUpEdit = GetDataTable(urlGet);
            slue_ChonPhieu.Properties.DataSource = searchLookUpEdit;
            slue_ChonPhieu.Properties.DisplayMember = "TenTQ";
            slue_ChonPhieu.Properties.ValueMember = "MaPhieu";
            if (searchLookUpEdit.Rows.Count > 0)
                slue_ChonPhieu.EditValue = searchLookUpEdit.Rows[0]["MaPhieu"].ToString();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string malenhsx = slueMaLenhSX.EditValue.ToString();
            tabCPage.SelectedTabPage = tabAdd;
        }

        private void gridView4_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView gridView = gridControl1.MainView as GridView;
            if (e.Column.FieldName == "btnChonCayVai")
            {
                if (e.IsGetData)
                {
                    // Gán giá trị nút cho cột
                    e.Value = "Cấp phát!"; // Hoặc có thể để null nếu không có giá trị cần thiết.
                }
            }
        }

        private void gridView4_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            GridView gridView = gridControl1.MainView as GridView;
            if (e.Column.FieldName == "btnChonCayVai")
            {
                object mp = slue_ChonPhieu.EditValue;
                if (mp == null)
                    return;
                string phieuYC = mp.ToString();
                string maVTMau = gridView.GetRowCellValue(e.RowHandle, "MaVTMau").ToString();
                frmChonCayVaiCap frm = new frmChonCayVaiCap(phieuYC, maVTMau);
                frm.ShowDialog();
                DataTable resultFrm = frm.result;
                if (resultFrm == null || resultFrm.Rows.Count == 0)
                    return;



                object rowData = gridView.GetRow(e.RowHandle);
                DataTable ctietCap = gridControl2.DataSource as DataTable;
                for (int i = 0; i < resultFrm.Rows.Count; i++)
                {
                    DataRow row = ctietCap.NewRow();
                    row["Barcode"] = resultFrm.Rows[i]["Barcode"].ToString();
                    row["MaVTMau"] = resultFrm.Rows[i]["MaVTMau"].ToString();
                    row["TenVT"] = resultFrm.Rows[i]["TenVT"].ToString();
                    row["CodeMau"] = gridView.GetRowCellValue(e.RowHandle, "CodeMau").ToString();
                    row["Mau"] = gridView.GetRowCellValue(e.RowHandle, "Mau").ToString();
                    row["AnhMau"] = resultFrm.Rows[i]["AnhMau"].ToString();
                    row["LoanMau"] = resultFrm.Rows[i]["LoangMau"].ToString();
                    row["DinhMuc"] = float.Parse(gridView.GetRowCellValue(e.RowHandle, "DinhMuc").ToString());
                    row["KeHoachCap"] = float.Parse(gridView.GetRowCellValue(e.RowHandle, "CapPhat").ToString());
                    row["SLYC"] = float.Parse(gridView.GetRowCellValue(e.RowHandle, "SoLuong").ToString());
                    row["SoLuong"] = float.Parse(resultFrm.Rows[i]["SoLuong"].ToString());
                    row["SLCap"] = float.Parse(resultFrm.Rows[i]["SoLuongCap"].ToString());
                    row["GhiChu"] = resultFrm.Rows[i]["GhiChu"].ToString();

                    ctietCap.Rows.Add(row);
                }
                gridControl2.DataSource = ctietCap;
                gridControl2.RefreshDataSource();
            }
        }
        private DataTable createDataCTPhieuCap()
        {
            DataTable result = new DataTable();
            result.Columns.Add("Barcode", typeof(string));
            result.Columns.Add("MaVTMau", typeof(string));
            result.Columns.Add("TenVT", typeof(string));
            result.Columns.Add("CodeMau", typeof(string));
            result.Columns.Add("Mau", typeof(string));
            result.Columns.Add("AnhMau", typeof(string));
            result.Columns.Add("LoanMau", typeof(string));
            result.Columns.Add("DinhMuc", typeof(float));
            result.Columns.Add("SoLuong", typeof(float));
            result.Columns.Add("KeHoachCap", typeof(float));
            result.Columns.Add("SLYC", typeof(float));
            result.Columns.Add("SLCap", typeof(float));
            result.Columns.Add("GhiChu", typeof(string));

            return result;
        }
        private DataTable createDataSave()
        {
            DataTable result = new DataTable();
            result.Columns.Add("ID", typeof(int));
            result.Columns.Add("MaPhieu", typeof(string));
            result.Columns.Add("BarcodeVatTu", typeof(string));
            result.Columns.Add("BarcodeNew", typeof(string));
            result.Columns.Add("SoLuong", typeof(string));
            result.Columns.Add("XacNhan", typeof(bool));
            result.Columns.Add("GhiChu", typeof(string));
            return result;
        }
        private DataRow createDataRowCTPhieuCap()
        {
            DataTable db = createDataCTPhieuCap();
            return db.NewRow();
        }
        private void AddRowToDataSave(ref DataTable tb)
        {

        }

        private void gridView4_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.BackColor = System.Drawing.Color.LightBlue;

            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);

            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.ForeColor = System.Drawing.Color.ForestGreen;

            e.Handled = false;
        }

        private void gridView4_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle == gridView4.FocusedRowHandle)
            {
                // Kiểm tra nếu cột không phải là "btnChonCayVai"
                if (e.Column.FieldName != "btnChonCayVai")
                {
                    e.Appearance.BackColor = Color.FromArgb(201, 228, 214);  // Đặt màu BurlyWood
                }
                else
                {
                    // Cột "btnChonCayVai" - có thể có màu sắc khác
                    e.Appearance.BackColor = Color.FromArgb(137, 137, 137); // Màu nền cho cell
                    e.Appearance.ForeColor = Color.Red; // Màu chữ cho cell

                    // Tạo viền giống như nút bấm
                    e.Appearance.BorderColor = Color.DarkGray; // Màu viền
                    e.Appearance.Options.UseBorderColor = true;  // Bật sử dụng màu viền

                    // Thêm padding để tạo khoảng cách giữa nội dung và viền

                }
            }
        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.BackColor = System.Drawing.Color.LightBlue;

            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);

            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.ForeColor = System.Drawing.Color.ForestGreen;

            e.Handled = false;
        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            e.Appearance.BackColor = Color.FromArgb(201, 228, 214);
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            int rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle < 0)
                return;
            if (gridView1.FocusedColumn.FieldName == "SLCap")
            {
                string inputValue = e.Value?.ToString();

                // Kiểm tra nếu giá trị nhập vào không phải là null hoặc rỗng
                if (string.IsNullOrWhiteSpace(inputValue))
                {
                    e.Valid = false;  // Không hợp lệ
                    e.ErrorText = "Số lượng cấp không thể để trống.";  // Thông báo lỗi
                    return;
                }

                if (float.TryParse(inputValue, out float floatValue))
                {
                    // Kiểm tra nếu giá trị là số thực và lớn hơn 0
                    if (floatValue <= 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số lượng cấp phải lớn hơn 0.";
                    }
                }
                else
                {
                    // Nếu không phải số hợp lệ (cả int và float)
                    e.Valid = false;
                    e.ErrorText = "Số lượng cấp phải là một số hợp lệ.";
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            tabCPage.SelectedTabPage = tabList;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string maPhieu = "PCVT_" + GlobleData.UserName + DateTime.Now.ToString("yyyyMMdd_hhmmss");

            DataTable dataGV = gridControl2.DataSource as DataTable;
            if (dataGV == null || dataGV.Rows.Count == 0)
            {
                MessageBox.Show("Chưa cấp vật tư cho phiếu!", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool isTT = true;
            DataTable dbSave = createDataSave();
            foreach (DataRow row in dataGV.Rows)
            {
                if (float.Parse(row["SLCap"].ToString()) == 0)
                {
                    isTT = false;
                }
                DataRow rowNew = dbSave.NewRow();
                rowNew["ID"] = 0;
                rowNew["MaPhieu"] = maPhieu;
                rowNew["BarcodeVatTu"] = row["Barcode"].ToString();
                rowNew["BarcodeNew"] = "";
                rowNew["SoLuong"] = float.Parse(row["SLCap"].ToString());
                rowNew["XacNhan"] = false;
                rowNew["GhiChu"] = row["GhiChu"].ToString();
                dbSave.Rows.Add(rowNew);
            }
            if (!isTT)
            {
                MessageBox.Show("Có dòng chưa nhập số lượng cấp!", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string urlStr = URL + string.Format(@"PhieuCap/PostDaTa?maphieu={0}&&malenhsx={1}&&nguoitao={2}&&thietbi={3}&&ghichu={4}",
                maPhieu, slue_ChonPhieu.EditValue, HttpUtility.UrlEncode(GlobleData.UserName), HttpUtility.UrlEncode(Environment.MachineName), HttpUtility.UrlEncode(txtGhiChu.Text)
                );

            string ms = PostDataTable(urlStr, dbSave);

            if (ms.ToString().ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2500);
                load();
                tabCPage.SelectedTabPage = tabList;
            }
            else
            {
                MessageBox.Show(ms, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void gridView1_CellMerge(object sender, CellMergeEventArgs e)
        {
            // Kiểm tra nếu cột hiện tại là cột SoLuongCap và cột MaVTMau
            if ((e.Column.FieldName != "Barcode" && e.Column.FieldName == "SoLuong" && e.Column.FieldName == "SLCap" && e.Column.FieldName != "GhiChu") && e.RowHandle1 != e.RowHandle2)
            {
                var maVTMau1 = gridView1.GetRowCellValue(e.RowHandle1, "MaVTMau");
                var maVTMau2 = gridView1.GetRowCellValue(e.RowHandle2, "MaVTMau");
                if (maVTMau1.Equals(maVTMau2))
                {
                    e.Merge = true; // Merge các ô
                    e.Handled = true; // Đánh dấu sự kiện đã xử lý
                }
                else
                {
                    e.Merge = false; // Không merge nếu giá trị khác nhau
                }
            }
        }

        private void slue_ListPhieuCapVatTu_EditValueChanged(object sender, EventArgs e)
        {
            loadChiTiet_PhieuCap();
        }
        private void loadChiTiet_PhieuCap()
        {
            try
            {
                object valObj = slue_ListPhieuCapVatTu.EditValue;
                if (valObj == null)
                    return;
                string val = valObj.ToString();

                string urlStr = URL + string.Format(@"PhieuCap/GetData?action=GET_ChiTiet&&maphieu={0}&&malenhsx=", val);
                DataTable result = GetDataTable(urlStr);
                if (result == null || result.Rows.Count == 0)
                {
                    gridControl11.DataSource = null;
                    return;
                }

                gridControl11.DataSource = result;

                DataRow row0 = result.Rows[0];
                txte_PhieuYeuCau.Text = row0["MaPhieuYeuCau"].ToString();
                txte_TrangThaiPYC.Text = row0["TrangThaiPYC"].ToString();
                txte_NguoiTao.Text = row0["NguoiTao"].ToString();
                txte_NgayTao.Text = row0["NgayTao"].ToString();
                txte_NguoiXacNhan.Text = row0["NguoiXacNhan"].ToString().ToLower() == "null" ? "<color=red>Chưa xác nhận</color>" : string.Format(@"<color=forestgreen>{0}</color>", row0["NguoiXacNhan"].ToString());
                txte_NgayXacNhan.Text = row0["NgayXacNhan"].ToString() == "01/01/1901" ? "<color=red>Chưa xác nhận</color>" : string.Format(@"<color=forestgreen>{0}</color>", row0["NgayXacNhan"].ToString());
                txte_TrangThaiPhieuCap.Text = row0["TrangThai"].ToString() == "0" ? "<color=red>Chưa xử lí</color>" : "<color=forestgreen>Đã xử lí</color>";
                txt_GhiChuL.Text = row0["GhiChuT"].ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo ngoại lẹ chưa được xử lí", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView3_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.BackColor = System.Drawing.Color.LightBlue;

            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);

            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.ForeColor = System.Drawing.Color.ForestGreen;

            e.Handled = false;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tabCPage.SelectedTabPage = tabList;
            load();
        }

        private void slue_ChonPhieu_EditValueChanged(object sender, EventArgs e)
        {
            object mp = slue_ChonPhieu.EditValue;
            if (mp == null)
                return;
            string str = createUrlPhieuYeuCau("GET_CT_Phieu", mp.ToString(), "");
            DataTable result = GetDataTable(str);
            gridControl1.DataSource = result;
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                object maphieucap = slue_ListPhieuCapVatTu.EditValue;
                if (maphieucap == null)
                    return;

                string urlStr = URL + string.Format(@"PhieuCap/GetData?action=deletePhieuCap&&maphieu={0}&&malenhsx=", maphieucap.ToString());
                DataTable result = GetDataTable(urlStr);
                string resultstr = result.Rows[0]["result"].ToString();
                if (resultstr.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2500);
                    this.load();
                }
                else
                {
                    MessageBox.Show(resultstr, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
