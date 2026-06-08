using DevExpress.Utils;
using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmLapPhieuYCVT : Form
    {
        GetDataService _service = new GetDataService();
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        public frmLapPhieuYCVT()
        {
            InitializeComponent();
            URL = settingsReader.GetValue("URL", typeof(string)).ToString();
            tabCPage.SelectedTabPage = tabList;
            Load();
        }
        private string createUrl(string action, string maphieu, string malenhSX)
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
        private void Load()
        {
            //popupContainerControl1.Visible = false;
            Load_slueMaLenhSanXuat();
            Load_slueMaPhieu();
            try
            {
                string malenhsx = slueMaLenhSX.EditValue.ToString();
                Load_DS_VatTu(malenhsx);
                if (slue_ChonPhieu.EditValue == null)
                    return;
                string malenhsxct = slue_ChonPhieu.EditValue.ToString();
                Load_DS_VatTuCT(malenhsxct);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private void frmLapPhieuYCVT_Load(object sender, EventArgs e)
        {
            Load();
        }
        private void Load_slueMaLenhSanXuat()
        {
            string urlGet = createUrl("GET_MaLenhSanXuat", "", "");
            DataTable searchLookUpEdit = GetDataTable(urlGet);
            slueMaLenhSX.Properties.DataSource = searchLookUpEdit;
            slueMaLenhSX.Properties.DisplayMember = "TenTQ";
            slueMaLenhSX.Properties.ValueMember = "MaLenhSanXuat";
            if (searchLookUpEdit.Rows.Count > 0)
                slueMaLenhSX.EditValue = searchLookUpEdit.Rows[0]["MaLenhSanXuat"].ToString();
            else
            {
                MessageBox.Show("Chưa chia lệnh sản xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
        private void Load_slueMaPhieu()
        {
            string urlGet = createUrl("GET_Phieu", "", "");
            DataTable searchLookUpEdit = GetDataTable(urlGet);
            slue_ChonPhieu.Properties.DataSource = searchLookUpEdit;
            slue_ChonPhieu.Properties.DisplayMember = "TenTQ";
            slue_ChonPhieu.Properties.ValueMember = "MaPhieu";
            if (searchLookUpEdit.Rows.Count > 0)
                slue_ChonPhieu.EditValue = searchLookUpEdit.Rows[0]["MaPhieu"].ToString();
            else
                slue_ChonPhieu.EditValue = null;
        }
        private void Load_DS_VatTu(string maLenhSanXuat)
        {
            string urlGet = createUrl("GET_VatTuCap", "", maLenhSanXuat);
            DataTable searchLookUpEdit = GetDataTable(urlGet);
            gc1.DataSource = searchLookUpEdit;
        }
        private void Load_DS_VatTuCT(string maPhieu)
        {
            string urlGet = createUrl("GET_CT_Phieu", maPhieu, "");
            DataTable searchLookUpEdit = GetDataTable(urlGet);
            gridControl1.DataSource = searchLookUpEdit;
            if (searchLookUpEdit.Rows.Count > 0)
            {
                DataRow row = searchLookUpEdit.Rows[0];
                txtNguoiTao.Text = row["NguoiTao"].ToString();
                txtNgayTao.Text = row["NgayTao"].ToString();
                txtNguoiXacNhan.Text = row["NguoiXacNhan"].ToString().ToLower() == "null" ? "<color=red>Chưa xác nhận</color>" : string.Format(@"<color=forestgreen>{0}</color>", row["NguoiXacNhan"].ToString());
                txtNgayXacNhan.Text = row["NgayXacNhan"].ToString() == "01/01/1901" ? "<color=red>Chưa xác nhận</color>" : string.Format(@"<color=forestgreen>{0}</color>", row["NgayXacNhan"].ToString());
                txtTrangThai.Text = row["TrangThai"].ToString() == "0" ? "<color=red>Chưa xử lí</color>" : "<color=forestgreen>Đã xử lí</color>";
                txtGhiChuL.Text = row["GhiChuT"].ToString();
            }
        }
        private DataTable creaateSaveList()
        {
            DataTable _tb = new DataTable();
            _tb.Columns.Add("ID", typeof(int));
            _tb.Columns.Add("MaPhieu", typeof(string));
            _tb.Columns.Add("MaNPL", typeof(string));
            _tb.Columns.Add("MaBom", typeof(string));
            _tb.Columns.Add("SoLuong", typeof(float));
            _tb.Columns.Add("GhiChu", typeof(string));
            DataTable list = gc1.DataSource as DataTable;
            for (int i = 0; i < list.Rows.Count; i++)
            {
                if (Convert.ToBoolean(list.Rows[i]["Chon"].ToString()))
                {
                    DataRow row = _tb.NewRow();
                    row["ID"] = 0;
                    row["MaPhieu"] = "";
                    row["MaNPL"] = list.Rows[i]["MaNPL"].ToString();
                    row["MaBom"] = list.Rows[i]["MaBom"].ToString();
                    row["SoLuong"] = float.Parse(list.Rows[i]["SoLuong"].ToString());
                    row["GhiChu"] = list.Rows[i]["GhiChu"].ToString();
                    _tb.Rows.Add(row);
                }
            }
            return _tb;
        }
        private void resetPhieu()
        {
            slueMaLenhSX.Properties.DataSource = new DataTable();
            txtGhiChu.Text = "";
            gc1.DataSource = new DataTable();
        }
        private void gridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {

        }

        private void slueMaLenhSX_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string malenhsx = slueMaLenhSX.EditValue.ToString();
                Load_DS_VatTu(malenhsx);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            tabCPage.SelectedTabPage = tabList;
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            int rowHandle = gridView1.FocusedRowHandle;
            if (rowHandle < 0)
                return;
            if (gridView1.FocusedColumn.FieldName == "SoLuong")
            {
                string value = e.Value.ToString();

                if (float.TryParse(value.ToString(), out float quantity))
                {
                    if (quantity <= 0)
                    {
                        e.ErrorText = "Số lượng phải lớn hơn 0.";
                        e.Valid = false;
                        return;
                    }
                }
                else
                {
                    e.ErrorText = "Giá trị phải là một số hợp lệ.";
                    e.Valid = false;
                    return;
                }
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            // Kiểm tra nếu thay đổi giá trị trong cột "Số lượng" hoặc "Đơn giá"
            //if (e.Column.FieldName == "SoLuong" )
            //{
            //    // Lấy giá trị của cột "Số lượng" và "Đơn giá"
            //    int soLuong = Convert.ToInt32(gridView1.GetRowCellValue(e.RowHandle, "SoLuong"));

            //    // Gán giá trị cho cột "Tổng tiền"
            //    gridView1.SetRowCellValue(e.RowHandle, "TongTien", soLuong);
            //    gridView1.PostEditor();
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DataTable dbSave = creaateSaveList();
            if (dbSave.Rows.Count == 0)
            {
                MessageBox.Show("Chưa chọn vật tư muốn yêu cầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maPhieu = "PYCVT_" + GlobleData.UserName + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");
            for (int i = 0; i < dbSave.Rows.Count; i++)
            {
                dbSave.Rows[i]["MaPhieu"] = maPhieu;
            }

            string malenhsx = slueMaLenhSX.EditValue.ToString();
            string tenTB = Environment.MachineName;
            string urlStr = string.Format(@"{0}PhieuYeuCau/PostData?maphieu={1}&&malenhsx={2}&&nguoitao={3}&&thietbi={4}&&ghichu={5}", URL, maPhieu, malenhsx, GlobleData.UserName, tenTB, txtGhiChu.Text);
            string ms = PostDataTable(urlStr, dbSave);

            if (ms.ToString().ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2500);
                tabCPage.SelectedTabPage = tabList;
                resetPhieu();
                Load();
            }
            else
            {
                MessageBox.Show(ms, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void slue_ChonPhieu_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string malenhsx = slue_ChonPhieu.EditValue.ToString();
                Load_DS_VatTuCT(malenhsx);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gridView3_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.BackColor = System.Drawing.Color.LightBlue;

            e.Appearance.ForeColor = System.Drawing.Color.ForestGreen;

            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);

            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Handled = false;
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            e.Appearance.BackColor = System.Drawing.Color.LightBlue;

            e.Appearance.ForeColor = System.Drawing.Color.ForestGreen;

            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);

            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Handled = false;
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            int rowHandle = e.RowHandle;
            if (e.Column.FieldName == "SoLuong")
            {
                object value = e.CellValue;
                if (value != null && float.TryParse(value.ToString(), out float quantity))
                {
                    object capphatobj = gridView1.GetRowCellValue(rowHandle, "CapPhat");
                    if (capphatobj != null && float.TryParse(capphatobj.ToString(), out float capphat))
                    {
                        if (quantity <= 0 || quantity > capphat)
                        {
                            e.Appearance.ForeColor = Color.FromArgb(218, 165, 32);
                            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                            e.DisplayText = "⚠ " + quantity;
                        }
                    }
                }
            }
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string malenhsx = slueMaLenhSX.EditValue.ToString();
            tabCPage.SelectedTabPage = tabAdd;

        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tabCPage.SelectedTabPage = tabList;
            Load();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                object maphieucap = slue_ChonPhieu.EditValue;
                if (maphieucap == null)
                    return;

                string urlStr = URL + string.Format(@"PhieuYeuCau/GetData?action={0}&&maphieu={1}&&malenhsx=", "delete_PhieuYeuCau", maphieucap.ToString());
                DataTable result = GetDataTable(urlStr);
                string resultstr = result.Rows[0]["result"].ToString();
                if (resultstr.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2500);
                    this.Load();
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
        private void btnShowPC_Click(object sender, EventArgs e)
        {
            object maphieuYC = slue_ChonPhieu.EditValue;
            if (maphieuYC == null)
            {
                MessageBox.Show("Chưa chọn mã phiếu yêu cầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string urlStr = URL + "PhieuCap/GetData?action=GET_PhieuCap&&maphieu=&&malenhsx=";
            DataTable PhieuCap = GetDataTable(urlStr);

            if (PhieuCap == null || PhieuCap.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có phiếu cấp cho phiếu yêu cầu này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (DataRow row in PhieuCap.Rows)
                if (row["MaPhieuYC"].ToString() == maphieuYC.ToString())
                    cbxPhieuCap.Properties.Items.Add(row["MaPhieu"].ToString());

            cbxPhieuCap.SelectedItem = PhieuCap.Rows[0]["MaPhieu"].ToString();

            urlStr = URL + string.Format(@"PhieuCap/GetData?action=GET_ChiTiet&&maphieu={0}&&malenhsx=", cbxPhieuCap.SelectedItem.ToString());
            DataTable result = GetDataTable(urlStr);
            if (result == null || result.Rows.Count == 0)
            {
                gridControl11.DataSource = null;
                return;
            }

            gridControl11.DataSource = result;

            DataRow row0 = result.Rows[0];
            pc_txte_NguoiTao.Text = row0["NguoiTao"].ToString();
            pc_txte_NgayTao.Text = row0["NgayTao"].ToString();
            pc_txte_NguoiXacNhan.Text = row0["NguoiXacNhan"].ToString().ToLower() == "null" ? "<color=red>Chưa xác nhận</color>" : string.Format(@"<color=forestgreen>{0}</color>", row0["NguoiXacNhan"].ToString());
            pc_txte_NgayXacNhan.Text = row0["NgayXacNhan"].ToString() == "01/01/1901" ? "<color=red>Chưa xác nhận</color>" : string.Format(@"<color=forestgreen>{0}</color>", row0["NgayXacNhan"].ToString());
            pc_txte_GhiChu.Text = row0["GhiChuT"].ToString();

            gridControl11.DataSource = result;

            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            int popupWidth = popupContainerControl1.Width;
            int popupHeight = popupContainerControl1.Height;
            int xPosition = (screenWidth - popupWidth) / 3;
            int yPosition = (screenHeight - popupHeight) / 3;

            popupContainerControl1.Location = new Point(xPosition, yPosition);
            popupContainerControl1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            popupContainerControl1.Hide();
        }

        private void gridView5_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                object ID = gridView5.GetRowCellValue(gridView5.FocusedRowHandle, "ID");
                //object XacNhan = gridView5.GetRowCellValue(gridView5.FocusedRowHandle, "XacNhan");
                bool XacNhan = Convert.ToBoolean(e.Value);
                if (ID == null || XacNhan == null)
                {
                    return;
                }
                else
                {
                    string urlStr = URL + string.Format(@"PhieuCap/GetData?action=XacNhanPhieuCap&&maphieu={0}&&malenhsx={1}", ID.ToString(),XacNhan == true ? "1" : "0");
                    GetDataTable((urlStr));

                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}
