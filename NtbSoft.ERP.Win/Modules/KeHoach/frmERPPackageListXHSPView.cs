using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmERPPackageListXHSPView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        int checkLoc = 1;
        bool checkLoadData = false;
        string URL = string.Empty;
        KeyDownControlHandler keyDownControlHandler;
        public frmERPPackageListXHSPView()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            checkLoc = 1;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            checkLoadData = false;
            dateTuNgay.EditValue = date;
            dateDenNgay.EditValue = date;
            dateConfimXH.EditValue = date;

            InIt();
            checkLoadData = true;
        }
        private void InIt()
        {
            searchLookUpEditPhieuXH.Properties.ValueMember = "PhieuXH";
            searchLookUpEditPhieuXH.Properties.DisplayMember = "PhieuXH";

            searchLookUpEditMH.Properties.ValueMember = "MaGop";
            searchLookUpEditMH.Properties.DisplayMember = "MaHang";

            loadDataTong();

        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void btnLapKH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var frm = new frmERPPackageListXHSP())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    loadDataTong();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }

        }

        private void btnAddorEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = grvTong.GetFocusedDataRow();
                if (dr == null) return;
                string drPhieu = dr["PhieuXH"].ToString();
                string ngayxuatkho = Convert.ToDateTime(dateConfimXH.EditValue).ToString("yyyy-MM-dd");
                DataTable tblTong = grcTong.DataSource as DataTable;
                if (tblTong == null) return;

                DataRow rowDaXuat = tblTong.AsEnumerable().FirstOrDefault(r =>
                    (r.Field<string>("PhieuXH") ?? "") == drPhieu.ToString()
                );
                if (rowDaXuat["TrangThai"].ToString() == "Đã xuất")
                {
                    XtraMessageBox.Show(
                          $"Phiếu {drPhieu} đã được xuất trước đó. Bạn không thể sửa kế hoạch!",
                          "Thông báo",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning
                      );
                    return;   // Không xử lý tiếp

                }
                string madh = rowDaXuat["MaGopEdit"].ToString();
                string po = rowDaXuat["POIDEdit"].ToString();
                string mapkl = rowDaXuat["MaPKLEdit"].ToString();
                string khachhang = rowDaXuat["MaKH"].ToString();
                using (var frm = new frmERPPackageListXHSP(drPhieu, madh, khachhang, mapkl, po, true))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        loadDataTong();
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e)
        {

            string textBox = comboBoxEdit1.EditValue.ToString();
            if (textBox == "Phiếu xuất hàng")
            {
                checkLoc = 0;
                layoutControlItemTuNgay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemDenNgay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemMH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemPXH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                LoadPhieuXH();
            }
            else if (textBox == "Thời gian")
            {
                checkLoc = 1;
                layoutControlItemTuNgay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItemDenNgay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItemMH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemPXH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                loadDataTong();
            }
            else
            {
                checkLoc = 2;
                layoutControlItemTuNgay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemDenNgay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemMH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItemPXH.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LoadMaHang();
            }

        }

        private void dateTuNgay_EditValueChanged(object sender, EventArgs e)
        {
            if (checkLoc != 1 || !checkLoadData) return;
            loadDataTong();
        }

        private void dateDenNgay_EditValueChanged(object sender, EventArgs e)
        {
            if (checkLoc != 1 || !checkLoadData) return;
            loadDataTong();
        }
        private void LoadPhieuXH()
        {

            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetPhieuXH");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditPhieuXH.Properties.DataSource = null;
                grcTong.DataSource = null;
                grcDetailPhieu.DataSource = null;
                grcDetailSize.DataSource = null;
                return;
            }
            else
            {
                searchLookUpEditPhieuXH.Properties.DataSource = tbl;
                searchLookUpEditPhieuXH.EditValue = tbl.Rows[0]["PhieuXH"];
            }
        }
        private void LoadMaHang()
        {
            string tungay = checkLoc != 1 ? "1990-01-01" : Convert.ToDateTime(dateTuNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string denngay = checkLoc != 1 ? "1990-01-01" : Convert.ToDateTime(dateDenNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string mahang = checkLoc != 2 ? "" : (searchLookUpEditMH.EditValue as string);

            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetMaHang&Para1={tungay}&Para2={denngay}&para3=${mahang}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditMH.Properties.DataSource = null;
                grcTong.DataSource = null;
                grcDetailPhieu.DataSource = null;
                grcDetailSize.DataSource = null;
                return;
            }
            else
            {
                searchLookUpEditMH.Properties.DataSource = tbl;
                searchLookUpEditMH.EditValue = tbl.Rows[0]["MaGop"];

            }
        }
        private void loadDataTong()
        {
            string tungay = checkLoc != 1 ? "1990-01-01" : Convert.ToDateTime(dateTuNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string denngay = checkLoc != 1 ? "1990-01-01" : Convert.ToDateTime(dateDenNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string mahang = checkLoc != 2 ? "" : (searchLookUpEditMH.EditValue as string);
            string maphieu = checkLoc != 0 ? "" : (searchLookUpEditPhieuXH.EditValue as string);
            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetDataTong&Para1={tungay}&Para2={denngay}&para3={mahang}&para4={maphieu}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcTong.DataSource = null;
                grcDetailPhieu.DataSource = null;
                grcDetailSize.DataSource = null;
                return;
            }
            else
            {
                grcTong.DataSource = tbl;
            }
        }
        private void loadDetailPhieu(string phieuxh)
        {

            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetDetailPhieu&Para1={phieuxh}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {

                grcDetailPhieu.DataSource = null;
                grcDetailSize.DataSource = null;
                return;
            }
            else
            {
                grcDetailPhieu.DataSource = tbl;
            }
        }
        private void loadDetailSize(string madh, string poid, string mapkl, string sizeid, string phieuXH)
        {

            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetDetailSize&Para1={madh}&Para2={poid}&Para3={mapkl}&Para4={sizeid}&Para5={phieuXH}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcDetailSize.DataSource = null;
                return;
            }
            else
            {
                grcDetailSize.DataSource = tbl;
            }
        }

        private void searchLookUpEditPhieuXH_EditValueChanged(object sender, EventArgs e)
        {
            loadDataTong();
        }



        private void searchLookUpEditMH_Properties_EditValueChanged(object sender, EventArgs e)
        {
            loadDataTong();
        }

        private void grvTong_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = grvTong.GetFocusedDataRow();
            if (dr == null) return;
            string drPhieu = dr["PhieuXH"].ToString();
            loadDetailPhieu(drPhieu);
        }

        private void grvTong_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return; // bỏ header

            GridView view = sender as GridView;
            string value = view.GetRowCellValue(e.RowHandle, "TrangThai")?.ToString();

            if (value == "Đã xuất")
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.ForeColor = Color.Black;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
        }

        private void grvDetailPhieu_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = grvDetailPhieu.GetFocusedDataRow();
            if (dr == null) return;
            string drDH = dr["MaGop"].ToString();
            string drPOID = dr["POID"].ToString();
            string drMaPKL = dr["MaPKL"].ToString();
            string drSizeID = dr["SizeID"].ToString();
            string phieuXH = dr["PhieuXH"].ToString();
            loadDetailSize(drDH, drPOID, drMaPKL, drSizeID, phieuXH);
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadDataTong();
        }

        private void btnConfirmXH_Click(object sender, EventArgs e)
        {
            DataRow dr = grvTong.GetFocusedDataRow();
            if (dr == null) return;
            string drPhieu = dr["PhieuXH"].ToString();
            string ngayxuatkho = Convert.ToDateTime(dateConfimXH.EditValue).ToString("yyyy-MM-dd");
            DataTable tblTong = grcTong.DataSource as DataTable;
            if (tblTong == null) return;

            DataRow rowDaXuat = tblTong.AsEnumerable().FirstOrDefault(r =>
                (r.Field<string>("PhieuXH") ?? "") == drPhieu.ToString()
            );
            if (rowDaXuat["TrangThai"].ToString() == "Đã xuất")
            {
                XtraMessageBox.Show(
                      $"Phiếu {drPhieu} đã được xuất trước đó. Bạn không thể xuất lại!",
                      "Thông báo",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning
                  );
                return;   // Không xử lý tiếp

            }
            DialogResult rs = MessageBox.Show(
                $"Bạn có muốn xuất hàng cho phiếu {drPhieu} này không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (rs == DialogResult.No)
            {
                return;
            }


            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/PostXH?Action=UpdateIsXH&Para1={drPhieu}&para2={ngayxuatkho}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json.ToUpper() == "TRUE")
            {
                if (rowDaXuat != null)
                {
                    rowDaXuat["TrangThai"] = "Đã xuất";
                    rowDaXuat["NgayXacNhanXH"] = Convert.ToDateTime(dateConfimXH.EditValue).ToString("dd/MM/yyyy");
                    rowDaXuat["SLDXuat"] = rowDaXuat["SLKH"];
                }
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            else
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }

        }

        private void btnDeletePhieu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = grvTong.GetFocusedDataRow();
            if (dr == null) return;
            string drPhieu = dr["PhieuXH"].ToString();
            string ngayxuatkho = Convert.ToDateTime(dateConfimXH.EditValue).ToString("yyyy-MM-dd");
            DataTable tblTong = grcTong.DataSource as DataTable;
            if (tblTong == null) return;

            DataRow rowDaXuat = tblTong.AsEnumerable().FirstOrDefault(r =>
                (r.Field<string>("PhieuXH") ?? "") == drPhieu.ToString()
            );
            if (rowDaXuat["TrangThai"].ToString() == "Đã xuất")
            {
                DialogResult rs = MessageBox.Show(
                   $"Phiếu  {drPhieu} này đã được xuất trước đó.Bạn có muốn xóa luôn không?",
                   "Xác nhận",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
               );
                if (rs == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                DialogResult rs = MessageBox.Show(
                   $"Bạn có muốn xóa  phiếu xuất hàng {drPhieu} này không?",
                   "Xác nhận",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
               );
                if (rs == DialogResult.No)
                {
                    return;
                }
            }

            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/PostXH?Action=DeletePhieu&Para1={drPhieu}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json.ToUpper() == "TRUE")
            {
                loadDataTong();
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
        }


        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();



            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
    }
}