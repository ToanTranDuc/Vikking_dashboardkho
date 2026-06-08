using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using NtbSoft.ERP.Win.Modules.Kho;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace NtbSoft.ERP.Win.Modules.POMau
{
    public partial class frmDanhSachPOMau : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private const int CHECK_SIZE = 16;
        private const int CHECK_LEFT = 6;
        private DataTable _nccTable;
        bool _PQThem, _PQSua, _PQXoa, _PQDuyetTao, _PQDuyetNCC, _PQDuyetSMV = false;
        public frmDanhSachPOMau()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _nccTable = new DataTable();
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadData();
            CheckPerminsion();
            CreateSearchLookUpNCC();
        }
        private void CheckPerminsion()
        {

            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenCosting/Get", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                    return;
                _PQThem = Convert.ToBoolean(tbl.Rows[0]["AllowAdd"].ToString());
                _PQSua = Convert.ToBoolean(tbl.Rows[0]["AllowEdit"].ToString());
                _PQXoa = Convert.ToBoolean(tbl.Rows[0]["AllowDelete"].ToString());
                _PQDuyetTao = Convert.ToBoolean(tbl.Rows[0]["AllowCreateApproval"].ToString());
                _PQDuyetNCC = Convert.ToBoolean(tbl.Rows[0]["AllowSupplierApproval"].ToString());
                _PQDuyetSMV = Convert.ToBoolean(tbl.Rows[0]["AllowSMVApproval"].ToString());
            }
        }
        private void CreateSearchLookUpNCC()
        {
            try
            {
                string urlNCC = $"{URL}POMau/Get?action=GetDSNCC";
                string jsonNCC = Task.Run(async () => await _clientExtension.GetAsnyc(urlNCC)).Result;
                _nccTable = JsonConvert.DeserializeObject<DataTable>(jsonNCC);

                repoNCC.DataSource = _nccTable;
                repoNCC.ValueMember = "MaNCC";
                repoNCC.DisplayMember = "TenNCC";
                repoNCC.NullText = "";

                var view = repoNCC.View;
                view.Columns.Clear();
                repoNCC.PopulateViewColumns();

                if (view.Columns["MaNCC"] != null)
                {
                    view.Columns["MaNCC"].Visible = false;
                    view.Columns["MaNCC"].VisibleIndex = -1;
                }
                if (view.Columns["TenNCC"] != null)
                {
                    view.Columns["TenNCC"].Caption = "Tên nhà cung cấp";
                    view.Columns["TenNCC"].VisibleIndex = 0;
                    view.Columns["TenNCC"].Width = 280;
                }

                view.OptionsBehavior.Editable = false;
                view.OptionsSelection.EnableAppearanceFocusedCell = true;


                // Gán repository vào cột MaNCC của grid
                GridColumn colMaNCC = gridView2.Columns["MaNCC"];
                if (gridColumn36 != null)
                {
                    gridColumn36.ColumnEdit = repoNCC;
                    gridColumn36.OptionsColumn.AllowEdit = false;
                    gridColumn36.Visible = true;
                    gridColumn36.Caption = "Nhà cung cấp";
                }

                gridView2.LayoutChanged();
            }
            catch { }
        }
        private void LoadData()
        {
            string currentMaPhieu = null;
            if (gridView1.FocusedRowHandle >= 0)
                currentMaPhieu = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "MaPhieu")?.ToString();

            string urlID = string.Format("{0}", URL + "POMau/Get?action=Get");
            string jsonID = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlID); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonID);
            foreach (DataRow row in tbl.Rows)
            {
                row["DuyetTao"] = Convert.ToBoolean(row["DuyetTao"]);
                row["DuyetNCC"] = Convert.ToBoolean(row["DuyetNCC"]);
                row["DuyetSMV"] = Convert.ToBoolean(row["DuyetSMV"]);
            }
            gridControl1.DataSource = tbl;
            if (!string.IsNullOrEmpty(currentMaPhieu))
            {
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (gridView1.IsGroupRow(i)) continue;
                    string ma = gridView1.GetRowCellValue(i, "MaPhieu")?.ToString();
                    if (ma == currentMaPhieu)
                    {
                        gridView1.FocusedRowHandle = i;
                        gridView1.MakeRowVisible(i);
                        break;
                    }
                }
            }
        }
        private void LoadDataCT(string maPhieu)
        {
            string urlCT = string.Format($"{URL}POMau/Get?action=GetDSCT&para1={maPhieu}");
            string jsonCT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCT); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonCT);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                gridControl2.DataSource = null;
                return;
            }
            if (!tbl.Columns.Contains("LoaiVT"))
                tbl.Columns.Add("LoaiVT", typeof(string));

            foreach (DataRow row in tbl.Rows)
            {
                bool isNPL = row["IsNPL"] != null && row["IsNPL"] != DBNull.Value
                             && Convert.ToBoolean(row["IsNPL"]);
                row["LoaiVT"] = isNPL ? "Nguyên liệu" : "Phụ liệu";
            }

            gridControl2.DataSource = tbl;
            gridView2.ExpandAllGroups();
        }
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!_PQThem)
            {
                XtraMessageBox.Show("User không có quyền thêm.Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmPOMau frm = new frmPOMau();
            frm.ShowDialog();
            LoadData();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //GridView view = gridView1;
            //if (view.FocusedRowHandle < 0)
            //    return;

            //object _objMaHang = null, objKhachHang = null, _objMaPhieu = null, _objSoLuong = null, _objTyLeKichThuoc = null, _objMauSac = string.Empty, _objKichThuocBaoGia = string.Empty;
            //GridView views = gridView1;
            //objKhachHang = view.GetFocusedRowCellValue(gridColumn3);
            //_objMaHang = view.GetFocusedRowCellValue(gridColumn4);
            //_objMaPhieu = view.GetFocusedRowCellValue(gridColumn1);
            //frmPOMauSMV frm = new frmPOMauSMV(objKhachHang.ToString(), _objMaHang.ToString(), _objMaPhieu.ToString());
            //frm.ShowDialog();
            //LoadData();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridView1;
            if (view.FocusedRowHandle < 0)
                return;

            object _objMaHang = null, objKhachHang = null, _objMaPhieu = null, _objSoLuong = null, _objTyLeKichThuoc = null, _objMauSac = string.Empty, _objKichThuocBaoGia = string.Empty;
            GridView views = gridView1;
            _objMaHang = view.GetFocusedRowCellValue(gridColumn4);
            _objMaPhieu = view.GetFocusedRowCellValue(gridColumn1);
            objKhachHang = view.GetFocusedRowCellValue(gridColumn3);
            if (CheckDuyet(_objMaPhieu.ToString()))
            {
                frmPOMauNhaCC frm = new frmPOMauNhaCC(objKhachHang.ToString(), _objMaHang.ToString(), _objMaPhieu.ToString());
                frm.ShowDialog();
                LoadData();
                LoadDataCT(_objMaPhieu.ToString());
            }
            else
            {
                XtraMessageBox.Show("Vui lòng hoàn tất xác nhận tại ô Created By trước khi tiến hành nhập dữ liệu cho nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!_PQSua)
            {
                XtraMessageBox.Show("User không có quyền sửa.Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            GridView view = gridView1;
            if (view.FocusedRowHandle < 0)
                return;

            object _objMaHang = null, objKhachHang = null, _objMaPhieu = null, _objSoLuong = null, _objTyLeKichThuoc = null, _objMauSac = string.Empty, _objKichThuocBaoGia = string.Empty;
            GridView views = gridView1;
            _objMaHang = view.GetFocusedRowCellValue(gridColumn4);
            _objMaPhieu = view.GetFocusedRowCellValue(gridColumn1);
            objKhachHang = view.GetFocusedRowCellValue(gridColumn3);

            frmPOMauEdit frm = new frmPOMauEdit(objKhachHang.ToString(), _objMaHang.ToString(), _objMaPhieu.ToString());
            frm.ShowDialog();
            LoadData();
        }
        private bool CheckDuyet(string maPhieu)
        {
            string url = $"{URL}POMau/Get?action=GetDuyet&para1={maPhieu}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int duyettao = 0;
                    int.TryParse(tbl.Rows[0]["DuyetTao"]?.ToString(), out duyettao);
                    return duyettao != 0;
                }
            }
            return false;
        }
        private bool CheckDuyetNCC(string maPhieu)
        {
            string url = $"{URL}POMau/Get?action=GetDuyetNCC&para1={maPhieu}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int duyetncc = 0;
                    int.TryParse(tbl.Rows[0]["DuyetNCC"]?.ToString(), out duyetncc);
                    return duyetncc != 0;
                }
            }
            return false;
        }
        private bool CheckDuyetSMV(string maPhieu)
        {
            string url = $"{URL}POMau/Get?action=GetDuyetSMV&para1={maPhieu}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int duyetsmv = 0;
                    int.TryParse(tbl.Rows[0]["DuyetSMV"]?.ToString(), out duyetsmv);
                    return duyetsmv != 0;
                }
            }
            return false;
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                int rowHandle = view.FocusedRowHandle;
                if (rowHandle < 0)
                {
                    //btnNhapSMV.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    return;
                }
                if (view.IsGroupRow(rowHandle))
                    rowHandle = view.GetChildRowHandle(rowHandle, 0);

                //bool daDuyetNCC = Convert.ToBoolean(view.GetRowCellValue(rowHandle, "DuyetNCC"));
                //btnNhapNCC.Enabled = !daDuyetNCC;

                object maPhieu = view.GetRowCellValue(rowHandle, "MaPhieu");
                if (maPhieu != null && maPhieu != DBNull.Value && !string.IsNullOrEmpty(maPhieu.ToString()))
                    LoadDataCT(maPhieu.ToString());
            }
            catch { }
        }
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.RowHandle < 0) return;
            if (e.Column.FieldName == "SMV")
            {
                string maPhieu = gridView1.GetRowCellValue(e.RowHandle, "MaPhieu")?.ToString();

                if (!CheckDuyetNCC(maPhieu))
                {
                    XtraMessageBox.Show("Vui lòng duyệt nhà cung cấp trước khi nhập SMV!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool daDuyetSMV = Convert.ToBoolean(gridView1.GetRowCellValue(e.RowHandle, "DuyetSMV"));
                if (daDuyetSMV) return;

                decimal currentSMV = 0;
                var smvVal = gridView1.GetRowCellValue(e.RowHandle, "SMV");
                if (smvVal != null && smvVal != DBNull.Value)
                    decimal.TryParse(smvVal.ToString(), out currentSMV);

                using (var frm = new frmPOMau_NhapSMV(maPhieu, currentSMV))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        gridView1.SetRowCellValue(e.RowHandle, "SMV", frm.SMVValue);
                        gridView1.RefreshRow(e.RowHandle);
                    }
                }
                return;
            }

            string colBool = null;
            string colNguoi = null;
            string loaiDuyet = null;

            if (e.Column.FieldName == "DuyetTao")
            {
                colBool = "DuyetTao";
                colNguoi = "NguoiDuyetTao";
                loaiDuyet = "DuyetTao";
            }
            else if (e.Column.FieldName == "DuyetNCC")
            {
                colBool = "DuyetNCC";
                colNguoi = "NguoiDuyetNCC";
                loaiDuyet = "DuyetNCC";
            }
            else if (e.Column.FieldName == "DuyetSMV")
            {
                colBool = "DuyetSMV";
                colNguoi = "NguoiDuyetSMV";
                loaiDuyet = "DuyetSMV";
            }
            else return;

            bool currentValue = Convert.ToBoolean(gridView1.GetRowCellValue(e.RowHandle, colBool));
            bool newValue = !currentValue;

            if (newValue)
            {
                if (loaiDuyet == "DuyetTao" && !_PQDuyetTao)
                {
                    XtraMessageBox.Show("User không có quyền duyệt Created By. Vui lòng kiểm tra lại!",
                        Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (loaiDuyet == "DuyetNCC" && !_PQDuyetNCC)
                {
                    XtraMessageBox.Show("User không có quyền duyệt Approved 1. Vui lòng kiểm tra lại!",
                        Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (loaiDuyet == "DuyetSMV" && !_PQDuyetSMV)
                {
                    XtraMessageBox.Show("User không có quyền duyệt SMV. Vui lòng kiểm tra lại!",
                        Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (loaiDuyet == "DuyetNCC" || loaiDuyet == "DuyetSMV")
                {
                    object duyetTao = gridView1.GetRowCellValue(e.RowHandle, "DuyetTao");
                    bool daDuyetTao = duyetTao != null
                                   && duyetTao != DBNull.Value
                                   && Convert.ToBoolean(duyetTao);
                    if (!daDuyetTao)
                    {
                        XtraMessageBox.Show("Vui lòng hoàn tất xác nhận tại ô Created By trước!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (loaiDuyet == "DuyetNCC")
                {
                    string maPhieu = gridView1.GetRowCellValue(e.RowHandle, "MaPhieu")?.ToString();
                    string urlCT = $"{URL}POMau/Get?action=GetCTNCC&para1={maPhieu}";
                    string jsonCT = Task.Run(async () => await _clientExtension.GetAsnyc(urlCT)).Result;
                    DataTable dtCT = JsonConvert.DeserializeObject<DataTable>(jsonCT);

                    if (dtCT == null || dtCT.Rows.Count == 0)
                    {
                        XtraMessageBox.Show("Chưa có dữ liệu vật tư!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    bool coNCC = dtCT.AsEnumerable().All(r =>
                        r["MaNCC"] != null &&
                        r["MaNCC"] != DBNull.Value &&
                        !string.IsNullOrEmpty(r["MaNCC"].ToString()));

                    if (!coNCC)
                    {
                        XtraMessageBox.Show("Chưa có đầy đủ dữ liệu, vui lòng nhập nhà cung cấp cho tất cả vật tư trước khi duyệt!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (loaiDuyet == "DuyetSMV")
                {
                    object duyetNCC = gridView1.GetRowCellValue(e.RowHandle, "DuyetNCC");
                    bool daDuyetNCC = duyetNCC != null
                                   && duyetNCC != DBNull.Value
                                   && Convert.ToBoolean(duyetNCC);
                    if (!daDuyetNCC)
                    {
                        XtraMessageBox.Show("Vui lòng hoàn tất xác nhận tại ô Approved 1 trước!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    object smv = gridView1.GetRowCellValue(e.RowHandle, "SMV");
                    bool coSMV = smv != null && smv != DBNull.Value && !string.IsNullOrWhiteSpace(smv.ToString()) && smv.ToString() != "0";
                    if (!coSMV)
                    {
                        XtraMessageBox.Show("Chưa có dữ liệu, vui lòng nhập SMV trước khi duyệt!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                var result = XtraMessageBox.Show("Bạn có chắc muốn duyệt không?", "Xác nhận duyệt",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                gridView1.SetRowCellValue(e.RowHandle, colBool, true);
                gridView1.SetRowCellValue(e.RowHandle, colNguoi, GlobleData.UserName);
                LuuDuyet(e.RowHandle, loaiDuyet, true);
            }
            else
            {
                if (loaiDuyet == "DuyetTao" && !_PQDuyetTao)
                {
                    XtraMessageBox.Show("User không có quyền hủy duyệt Created By. Vui lòng kiểm tra lại!",
                        Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (loaiDuyet == "DuyetNCC" && !_PQDuyetNCC)
                {
                    XtraMessageBox.Show("User không có quyền hủy duyệt Approved 1. Vui lòng kiểm tra lại!",
                        Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (loaiDuyet == "DuyetSMV" && !_PQDuyetSMV)
                {
                    XtraMessageBox.Show("User không có quyền hủy duyệt SMV. Vui lòng kiểm tra lại!",
                        Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = XtraMessageBox.Show("Bạn có chắc muốn hủy duyệt không?", "Xác nhận hủy duyệt",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;

                gridView1.SetRowCellValue(e.RowHandle, colBool, false);
                gridView1.SetRowCellValue(e.RowHandle, colNguoi, "");
                LuuDuyet(e.RowHandle, loaiDuyet, false);
            }
            gridView1.RefreshRow(e.RowHandle);
        }
        private void LuuDuyet(int rowHandle, string loaiDuyet, bool isDuyet)
        {
            try
            {
                object maPhieu = gridView1.GetRowCellValue(rowHandle, "MaPhieu");
                if (maPhieu == null) return;
                string action = isDuyet ? "DuyetPhieu" : "HuyDuyetPhieu";
                string url = $"{URL}POMau/Get?action={action}&para1={maPhieu}&para2={loaiDuyet}&para3={GlobleData.UserName}";
                string result = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                LoadData();
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            catch { }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            string colBool = null;
            string colNguoi = null;
            DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit rItem = null;

            if (e.Column.FieldName == "DuyetTao")
            {
                colBool = "DuyetTao";
                colNguoi = "NguoiDuyetTaoTen";
                rItem = rpItemCheckEditCreated;
            }
            else if (e.Column.FieldName == "DuyetNCC")
            {
                colBool = "DuyetNCC";
                colNguoi = "NguoiDuyetNCCTen";
                rItem = rpItemCheckEditSup;
            }
            else if (e.Column.FieldName == "DuyetSMV")
            {
                colBool = "DuyetSMV";
                colNguoi = "NguoiDuyetSMVTen";
                rItem = rpItemCheckEditSMV;
            }

            if (rItem == null) return;

            e.Graphics.FillRectangle(
                new SolidBrush(e.Appearance.GetBackColor()),
                e.Bounds
            );

            bool isChecked = Convert.ToBoolean(view.GetRowCellValue(e.RowHandle, colBool));
            string tenNguoi = view.GetRowCellValue(e.RowHandle, colNguoi)?.ToString() ?? "";

            var checkInfo = new CheckEditViewInfo(rItem);
            var painter = new CheckEditPainter();

            checkInfo.EditValue = isChecked;
            checkInfo.Bounds = new Rectangle(
                e.Bounds.X + CHECK_LEFT,
                e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                CHECK_SIZE,
                CHECK_SIZE
            );
            checkInfo.CalcViewInfo(e.Graphics);

            var args = new ControlGraphicsInfoArgs(checkInfo, new GraphicsCache(e.Graphics), checkInfo.Bounds);
            painter.Draw(args);

            if (!string.IsNullOrEmpty(tenNguoi))
            {
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 4,
                    e.Bounds.Y,
                    e.Bounds.Width - CHECK_LEFT - CHECK_SIZE - 4,
                    e.Bounds.Height
                );
                e.Appearance.DrawString(e.Cache, tenNguoi, textRect);
            }

            e.Handled = true;
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            string fieldName = view.FocusedColumn?.FieldName;
            if (fieldName == "DuyetTao" || fieldName == "DuyetNCC" || fieldName == "DuyetSMV" || fieldName == "SMV")
            {
                e.Cancel = true;
            }
        }
        private string ToRoman(int number)
        {
            if (number < 1) return string.Empty;
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            return string.Empty;
        }
        //private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    GridView view = gridView1;
        //    if (view.FocusedRowHandle < 0) return;
        //    string maKH = view.GetFocusedRowCellValue("MaKH")?.ToString();
        //    string maHang = view.GetFocusedRowCellValue("MaHang")?.ToString();
        //    string maPhieu = view.GetFocusedRowCellValue("MaPhieu")?.ToString();
        //    string customer = view.GetFocusedRowCellValue("KhachHang")?.ToString();
        //    string style = view.GetFocusedRowCellValue("TenHang")?.ToString();
        //    string forecastQty = view.GetFocusedRowCellValue("SoLuong")?.ToString();
        //    string sizeRatio = view.GetFocusedRowCellValue("TyLeKichThuoc")?.ToString();
        //    string colorways = view.GetFocusedRowCellValue("MauSac")?.ToString();
        //    string quotaSize = view.GetFocusedRowCellValue("KichThuocBaoGia")?.ToString();
        //    string tenNgTao = view.GetFocusedRowCellValue("NguoiTaoTen")?.ToString();

        //    if (!CheckDuyetSMV(maPhieu))
        //    {
        //        XtraMessageBox.Show("Vui lòng thực hiện nhập và xác nhận đầy đủ dữ liệu trước khi xuất", "Xác nhận", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    string urlDetail = $"{URL}POMau/Get?action=GetCTNCC&para1={maPhieu}";
        //    string jsonDetail = Task.Run(async () => await _clientExtension.GetAsnyc(urlDetail)).Result;
        //    DataTable dtDetail = JsonConvert.DeserializeObject<DataTable>(jsonDetail);
        //    if (dtDetail == null) return;

        //    SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx", FileName = $"Costing_{maPhieu}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx" };
        //    if (sfd.ShowDialog() != DialogResult.OK) return;

        //    Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        //    Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(Application.StartupPath + @"\Templates\POMau.xlsx");
        //    Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];

        //    Dictionary<string, DataTable> cacheNCC = new Dictionary<string, DataTable>();

        //    try
        //    {
        //        xlWorksheet.Cells.Font.Name = "Times New Roman";
        //        xlWorksheet.Cells[5, 3] = customer;
        //        xlWorksheet.Cells[6, 3] = style;
        //        xlWorksheet.Cells[7, 3] = int.TryParse(forecastQty, out int fqInt) ? (object)fqInt : forecastQty;
        //        xlWorksheet.Cells[8, 3] = sizeRatio;
        //        xlWorksheet.Cells[9, 3] = colorways;
        //        xlWorksheet.Cells[10, 3] = quotaSize;

        //        string hinhAnh = "";
        //        try
        //        {
        //            string urlHA = $"{URL}POMau/Get?action=GetThongTin&para1={maPhieu}";
        //            string jsonHA = Task.Run(async () => await _clientExtension.GetAsnyc(urlHA)).Result;
        //            DataTable dtHA = JsonConvert.DeserializeObject<DataTable>(jsonHA);
        //            if (dtHA != null && dtHA.Rows.Count > 0)
        //                hinhAnh = dtHA.Rows[0]["TenFile"]?.ToString() ?? "";
        //        }
        //        catch { }

        //        if (!string.IsNullOrEmpty(hinhAnh))
        //        {
        //            try
        //            {
        //                string baseUrl = URL.Replace("api/", "");
        //                string imageUrl = baseUrl + "Content/Image/POMau/" + hinhAnh;
        //                System.Diagnostics.Debug.WriteLine("IMAGE URL EXCEL: " + imageUrl);

        //                byte[] imageBytes;
        //                using (var client = new System.Net.Http.HttpClient())
        //                {
        //                    imageBytes = Task.Run(async () => await client.GetByteArrayAsync(imageUrl)).Result;
        //                }

        //                string tempImagePath = Path.Combine(Path.GetTempPath(), "pomau_temp.jpg");
        //                File.WriteAllBytes(tempImagePath, imageBytes);

        //                Microsoft.Office.Interop.Excel.Range picRange = xlWorksheet.Range["F5:G10"];
        //                float picLeft = (float)(double)((Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[5, 6]).Left;
        //                float picTop = (float)(double)((Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[5, 6]).Top;
        //                float maxH = (float)(double)picRange.Height;
        //                float maxW = 100f;

        //                using (Image img = Image.FromFile(tempImagePath))
        //                {
        //                    float ratio = Math.Min(maxW / img.Width, maxH / img.Height);
        //                    float newW = img.Width * ratio;
        //                    float newH = img.Height * ratio;

        //                    var shape = xlWorksheet.Shapes.AddPicture(
        //                        tempImagePath,
        //                        Microsoft.Office.Core.MsoTriState.msoFalse,
        //                        Microsoft.Office.Core.MsoTriState.msoCTrue,
        //                        picLeft, picTop, newW, newH
        //                    );
        //                    shape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
        //                    shape.Placement = Microsoft.Office.Interop.Excel.XlPlacement.xlMoveAndSize;
        //                    //shape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoCTrue;
        //                    //shape.Line.ForeColor.RGB = ColorTranslator.ToOle(Color.Black);
        //                    //shape.Line.Weight = 0.5f;
        //                }
        //                File.Delete(tempImagePath);
        //            }
        //            catch (Exception exImg)
        //            {
        //                System.Diagnostics.Debug.WriteLine("LỖI ẢNH: " + exImg.Message);
        //            }
        //        }
        //        int currentRow = 13;
        //        int groupIdx = 1;
        //        decimal totalTongTien = 0;
        //        var groups = dtDetail.AsEnumerable()
        //                             .GroupBy(r => new { Ma = r["MaCLVT"]?.ToString(), Ten = r["ChungLoaiVatTu"]?.ToString() })
        //                             .OrderBy(g => g.Key.Ma);

        //        foreach (var group in groups)
        //        {
        //            // decimal groupTotal = group
        //            //.Where(r => r["TongTien"] != null && r["TongTien"] != DBNull.Value)
        //            //.Sum(r => Convert.ToDecimal(r["TongTien"]));
        //            decimal groupTotal = group.Sum(r =>
        //            {
        //                decimal dm = (r["DinhMuc"] != null && r["DinhMuc"] != DBNull.Value) ? Convert.ToDecimal(r["DinhMuc"]) : 0;
        //                decimal hh = (r["HaoHut"] != null && r["HaoHut"] != DBNull.Value) ? Convert.ToDecimal(r["HaoHut"]) : 0;
        //                decimal sl = !string.IsNullOrEmpty(forecastQty) ? Convert.ToDecimal(forecastQty) : 0;
        //                decimal gt = (r["DonGia"] != null && r["DonGia"] != DBNull.Value) ? Convert.ToDecimal(r["DonGia"]) : 0;
        //                return (dm + hh) * sl * gt;
        //            });
        //            Microsoft.Office.Interop.Excel.Range groupRange =
        //            xlWorksheet.Range[xlWorksheet.Cells[currentRow, 1], xlWorksheet.Cells[currentRow, 13]];
        //            groupRange.Merge();
        //            groupRange.Value = $"{ToRoman(groupIdx)}. {group.Key.Ten} (Total: {groupTotal:N0})";
        //            groupRange.Font.Bold = true;
        //            groupRange.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(235, 241, 222));
        //            groupRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

        //            currentRow++;
        //            groupIdx++;
        //            DataTable dtNCC;
        //            if (!cacheNCC.TryGetValue(group.Key.Ma, out dtNCC))
        //            {
        //                string urlNCC = $"{URL}POMau/Get?action=GetNCC&para1={group.Key.Ma}";
        //                string jsonNCC = Task.Run(async () => await _clientExtension.GetAsnyc(urlNCC)).Result;
        //                dtNCC = JsonConvert.DeserializeObject<DataTable>(jsonNCC);
        //                cacheNCC[group.Key.Ma] = dtNCC;
        //            }

        //            int itemIdx = 1;
        //            foreach (var row in group)
        //            {
        //                string maNCC = row["MaNCC"]?.ToString();
        //                string tenNCC = "";
        //                if (dtNCC != null && !string.IsNullOrEmpty(maNCC))
        //                {
        //                    var nccRow = dtNCC.AsEnumerable().FirstOrDefault(r => r["MaNCC"]?.ToString() == maNCC);
        //                    if (nccRow != null) tenNCC = nccRow["TenNCC"]?.ToString();
        //                }
        //                xlWorksheet.Cells[currentRow, 1] = itemIdx++;
        //                ((Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[currentRow, 1]).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
        //                xlWorksheet.Cells[currentRow, 2] = row["MaVT"];
        //                Microsoft.Office.Interop.Excel.Range desRange = xlWorksheet.Range[xlWorksheet.Cells[currentRow, 3], xlWorksheet.Cells[currentRow, 4]];
        //                desRange.Merge();
        //                desRange.Value = row["ChiTiet"];
        //                xlWorksheet.Cells[currentRow, 5] = row["Position"]; ;
        //                xlWorksheet.Cells[currentRow, 6] = row["MauVT"];
        //                xlWorksheet.Cells[currentRow, 7] = row["KhoVai"] + "" + row["TenDVKV"]; ;
        //                xlWorksheet.Cells[currentRow, 8] = row["TenDVVT"];
        //                xlWorksheet.Cells[currentRow, 9] = row["DinhMuc"];
        //                xlWorksheet.Cells[currentRow, 10] = tenNCC;
        //                xlWorksheet.Cells[currentRow, 11] = row["DonGia"];
        //                ((Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[currentRow, 11]).NumberFormat = "#,##0";
        //                //decimal tongTien = 0;
        //                //if (row["TongTien"] != null && row["TongTien"] != DBNull.Value)
        //                //    tongTien = Convert.ToDecimal(row["TongTien"]);
        //                decimal dm = (row["DinhMuc"] != null && row["DinhMuc"] != DBNull.Value) ? Convert.ToDecimal(row["DinhMuc"]) : 0;
        //                decimal hh = (row["HaoHut"] != null && row["HaoHut"] != DBNull.Value) ? Convert.ToDecimal(row["HaoHut"]) : 0;
        //                decimal sl = !string.IsNullOrEmpty(forecastQty) ? Convert.ToDecimal(forecastQty) : 0;
        //                decimal gt = (row["DonGia"] != null && row["DonGia"] != DBNull.Value) ? Convert.ToDecimal(row["DonGia"]) : 0;
        //                decimal tongTien = (dm + hh) * sl * gt;
        //                totalTongTien += tongTien;
        //                xlWorksheet.Cells[currentRow, 12] = tongTien;
        //                ((Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[currentRow, 12]).NumberFormat = "#,##0";
        //                var ghiChu = row["GhiChu"];
        //                xlWorksheet.Cells[currentRow, 13] = (ghiChu == null || ghiChu == DBNull.Value ||
        //                    ghiChu.ToString() == "0" || string.IsNullOrWhiteSpace(ghiChu.ToString())) ? "" : ghiChu.ToString();
        //                currentRow++;
        //            }
        //        }
        //        Microsoft.Office.Interop.Excel.Range finalRange = xlWorksheet.Range[xlWorksheet.Cells[13, 1], xlWorksheet.Cells[currentRow - 1, 13]];
        //        finalRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

        //        int summaryRow = currentRow + 2;
        //        int colStart = 7;
        //        int colMid = 12;
        //        int colEnd = 13;

        //        string[,] summaryItems = new string[,]
        //        {
        //            { "(1) Total material cost price per pc(I)+(II)+(III)+(IV)+(V)", "", "" },
        //            { "(2) Total material cost price per pc with wastage", "", "" },
        //            { "(3) Total material cost with handling cost", "", "" },
        //            { "(4) Manufacturer cost(cutting, sewing, +checking + packing)", "", "" },
        //            { "(5) Profit for CM", "", "" },
        //            { "(6) Cost for special tool( if have)", "", "" },
        //            { "(7) Im-export cost", "", "" },
        //            { "(8) Cost for renting Gore machine + Spare part", "", "" },
        //            { "(9)Cost for third party", "", "" },
        //        };

        //        string TotalCPSPHP = "0", TotalHCost = "0", ManuCost = "0", ProfitfCM = "0",
        //               CostSpeTool = "0", IECost = "0", CostGMSP = "0", Cost3P = "0";


        //        try
        //        {
        //            string urlTV = $"{URL}POMau/Get?action=GetTVCosting&para1={maKH}&para2={maHang}";
        //            string jsonTV = Task.Run(async () => await _clientExtension.GetAsnyc(urlTV)).Result;
        //            DataTable dtTV = JsonConvert.DeserializeObject<DataTable>(jsonTV);

        //            if (dtTV != null && dtTV.Rows.Count > 0)
        //            {
        //                DataRow rowTV = dtTV.Rows[0];
        //                TotalCPSPHP = rowTV["TotalCPSPHP"] == DBNull.Value ? "0" : rowTV["TotalCPSPHP"].ToString();
        //                TotalHCost = rowTV["TotalHCost"] == DBNull.Value ? "0" : rowTV["TotalHCost"].ToString();
        //                ManuCost = rowTV["ManuCost"] == DBNull.Value ? "0" : rowTV["ManuCost"].ToString();
        //                ProfitfCM = rowTV["ProfitfCM"] == DBNull.Value ? "0" : rowTV["ProfitfCM"].ToString();
        //                CostSpeTool = rowTV["CostSpeTool"] == DBNull.Value ? "0" : rowTV["CostSpeTool"].ToString();
        //                IECost = rowTV["IECost"] == DBNull.Value ? "0" : rowTV["IECost"].ToString();
        //                CostGMSP = rowTV["CostGMSP"] == DBNull.Value ? "0" : rowTV["CostGMSP"].ToString();
        //                Cost3P = rowTV["Cost3P"] == DBNull.Value ? "0" : rowTV["Cost3P"].ToString();
        //            }
        //        }
        //        catch { }
        //        string[] midValues = new string[]
        //        {
        //            "",TotalCPSPHP,TotalHCost,ManuCost,ProfitfCM,"","","",""
        //        };
        //        decimal val1 = totalTongTien;
        //        decimal val2 = val1 * (string.IsNullOrEmpty(TotalCPSPHP) ? 0 : Convert.ToDecimal(TotalCPSPHP));
        //        decimal smv = string.IsNullOrEmpty(view.GetFocusedRowCellValue("SMV")?.ToString()) ? 0
        //                       : Convert.ToDecimal(view.GetFocusedRowCellValue("SMV")?.ToString());
        //        decimal manu = string.IsNullOrEmpty(ManuCost) ? 0 : Convert.ToDecimal(ManuCost);
        //        decimal prof = string.IsNullOrEmpty(ProfitfCM) ? 0 : Convert.ToDecimal(ProfitfCM);
        //        decimal hcost = string.IsNullOrEmpty(TotalHCost) ? 0 : Convert.ToDecimal(TotalHCost);
        //        decimal tool = string.IsNullOrEmpty(CostSpeTool) ? 0 : Convert.ToDecimal(CostSpeTool);
        //        decimal ie = string.IsNullOrEmpty(IECost) ? 0 : Convert.ToDecimal(IECost);
        //        decimal gmsp = string.IsNullOrEmpty(CostGMSP) ? 0 : Convert.ToDecimal(CostGMSP);
        //        decimal c3p = string.IsNullOrEmpty(Cost3P) ? 0 : Convert.ToDecimal(Cost3P);
        //        decimal val3 = val2 + (val2 * hcost);
        //        decimal val4 = smv * manu;
        //        decimal val5 = val4 * prof;
        //        decimal val6 = tool;
        //        decimal val7 = ie;
        //        decimal val8 = gmsp;
        //        decimal val9 = c3p;

        //        decimal[] cellValues = new decimal[]
        //        {
        //           val1,val2,val3,val4,val5,val6,val7,val8,val9
        //        };
        //        for (int i = 0; i < summaryItems.GetLength(0); i++)
        //        {
        //            string label = summaryItems[i, 0];
        //            string mid = summaryItems[i, 1];
        //            int r = summaryRow + i;

        //            int labelEndCol = (i == 3) ? colMid - 2 : (i == 0 ? colMid : colMid - 1);
        //            Microsoft.Office.Interop.Excel.Range labelRange =
        //                xlWorksheet.Range[xlWorksheet.Cells[r, colStart], xlWorksheet.Cells[r, labelEndCol]];
        //            labelRange.Merge();
        //            labelRange.Value = label;
        //            labelRange.Font.Bold = true;
        //            labelRange.WrapText = true;
        //            labelRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
        //            labelRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

        //            if (i == 3)
        //            {
        //                var smvCell = (Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[r, colMid - 1];
        //                smvCell.Value = smv;
        //                smvCell.NumberFormat = "#,##0.00";
        //                smvCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
        //                smvCell.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(189, 215, 238));
        //            }

        //            string midVal = (i == 0) ? "" : (midValues[i] ?? "");

        //            if (!string.IsNullOrEmpty(midVal))
        //            {
        //                xlWorksheet.Cells[r, colMid] = midVal;
        //                var midCell = (Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[r, colMid];
        //                midCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
        //                midCell.NumberFormat = "#,##0.00";
        //            }

        //            decimal cellValue = cellValues[i];
        //            xlWorksheet.Cells[r, colEnd] = cellValue;
        //            ((Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[r, colEnd]).NumberFormat = "#,##0";
        //        }

        //        int totalRow = summaryRow + summaryItems.GetLength(0);
        //        Microsoft.Office.Interop.Excel.Range totalLabelRange =
        //            xlWorksheet.Range[xlWorksheet.Cells[totalRow, colStart], xlWorksheet.Cells[totalRow, colMid - 1]];
        //        totalLabelRange.Merge();
        //        totalLabelRange.Value = "SALE COST TOTAL [sum (3), (9)] (usd)";
        //        totalLabelRange.Font.Bold = true;
        //        totalLabelRange.Interior.Color = ColorTranslator.ToOle(Color.Yellow);
        //        totalLabelRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
        //        totalLabelRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

        //        var totalValueCell = (Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[totalRow, colEnd];
        //        totalValueCell.Value = val3 + val4 + val5 + val6 + val7 + val8 + val9;
        //        totalValueCell.NumberFormat = "#,##0";
        //        totalValueCell.Font.Bold = true;
        //        totalValueCell.Font.Size = 13;
        //        totalValueCell.Interior.Color = ColorTranslator.ToOle(Color.Yellow);
        //        ((Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[totalRow, colMid]).Interior.Color = ColorTranslator.ToOle(Color.Yellow);
        //        // 2. Border toàn bộ bảng summary
        //        Microsoft.Office.Interop.Excel.Range summaryTableRange =
        //        xlWorksheet.Range[xlWorksheet.Cells[summaryRow, colStart], xlWorksheet.Cells[totalRow, colEnd]];
        //        summaryTableRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
        //        summaryTableRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        //        xlWorkbook.SaveAs(sfd.FileName);
        //        var result = XtraMessageBox.Show("Xuất file thành công! Bạn có muốn mở file không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //        if (result == DialogResult.Yes)
        //        {
        //            xlApp.Visible = true;
        //        }
        //        else
        //        {
        //            xlWorkbook.Close();
        //            xlApp.Quit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Lỗi: " + ex.Message);
        //        xlWorkbook?.Close(false);
        //        xlApp.Quit();
        //    }
        //}
        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridView1;
            if (view.FocusedRowHandle < 0) return;

            string maKH = view.GetFocusedRowCellValue("MaKH")?.ToString();
            string maHang = view.GetFocusedRowCellValue("MaHang")?.ToString();
            string maPhieu = view.GetFocusedRowCellValue("MaPhieu")?.ToString();
            string customer = view.GetFocusedRowCellValue("KhachHang")?.ToString();
            string style = view.GetFocusedRowCellValue("TenHang")?.ToString();
            string forecastQty = view.GetFocusedRowCellValue("SoLuong")?.ToString();
            string sizeRatio = view.GetFocusedRowCellValue("TyLeKichThuoc")?.ToString();
            string colorways = view.GetFocusedRowCellValue("MauSac")?.ToString();
            string quotaSize = view.GetFocusedRowCellValue("KichThuocBaoGia")?.ToString();
            string tenNguoiTao = view.GetFocusedRowCellValue("NguoiTaoTen")?.ToString();

            if (!CheckDuyetSMV(maPhieu))
            {
                XtraMessageBox.Show("Vui lòng thực hiện nhập và xác nhận đầy đủ dữ liệu trước khi xuất",
                    "Xác nhận", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string urlDetail = $"{URL}POMau/Get?action=GetCTNCC&para1={maPhieu}";
            string jsonDetail = Task.Run(async () => await _clientExtension.GetAsnyc(urlDetail)).Result;
            DataTable dtDetail = JsonConvert.DeserializeObject<DataTable>(jsonDetail);
            if (dtDetail == null) return;

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = $"Costing_{maPhieu}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            string templatePath = Path.Combine(Application.StartupPath, "Templates", "POMau.xlsx");

            using (var package = new ExcelPackage(new FileInfo(templatePath)))
            {
                var ws = package.Workbook.Worksheets[0];

                try
                {
                    ws.Cells.Style.Font.Name = "Times New Roman";

                    ws.Cells[5, 3].Value = customer;
                    ws.Cells[6, 3].Value = style;
                    ws.Cells[7, 3].Value = int.TryParse(forecastQty, out int fqInt) ? (object)fqInt : forecastQty;
                    ws.Cells[8, 3].Value = sizeRatio;
                    ws.Cells[9, 3].Value = colorways;
                    ws.Cells[10, 3].Value = quotaSize;
                    string hinhAnh = "";
                    try
                    {
                        string urlHA = $"{URL}POMau/Get?action=GetThongTin&para1={maPhieu}";
                        string jsonHA = Task.Run(async () => await _clientExtension.GetAsnyc(urlHA)).Result;
                        DataTable dtHA = JsonConvert.DeserializeObject<DataTable>(jsonHA);
                        if (dtHA != null && dtHA.Rows.Count > 0)
                            hinhAnh = dtHA.Rows[0]["TenFile"]?.ToString() ?? "";
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(hinhAnh))
                    {
                        try
                        {
                            string imageUrl = URL.Replace("api/", "") + "Content/Image/POMau/" + hinhAnh;
                            byte[] imageBytes;
                            using (var client = new System.Net.Http.HttpClient())
                                imageBytes = Task.Run(async () => await client.GetByteArrayAsync(imageUrl)).Result;

                            using (var ms = new MemoryStream(imageBytes))
                            using (var img = System.Drawing.Image.FromStream(ms))
                            {
                                var picture = ws.Drawings.AddPicture("POMauImg", img);

                                double colWidth = 0;
                                for (int col = 6; col <= 7; col++)
                                    colWidth += (ws.Column(col).Width > 0 ? ws.Column(col).Width : 8.43) * 7;

                                double rowHeight = 0;
                                for (int row = 5; row <= 10; row++)
                                    rowHeight += (ws.Row(row).Height > 0 ? ws.Row(row).Height : 15) * 1.33;

                                int maxWidth = (int)colWidth;
                                int maxHeight = (int)rowHeight;

                                float ratio = Math.Min((float)maxWidth / img.Width,
                                                         (float)maxHeight / img.Height);
                                int newWidth = (int)(img.Width * ratio);
                                int newHeight = (int)(img.Height * ratio);

                                picture.SetPosition(4, 0, 5, 0);
                                picture.SetSize(newWidth, newHeight);
                            }
                        }
                        catch (Exception exImg)
                        {
                            System.Diagnostics.Debug.WriteLine("LỖI ẢNH: " + exImg.Message);
                        }
                    }
                    int currentRow = 13;
                    int groupIdx = 1;
                    decimal totalTongTien = 0;
                    decimal slForecast = decimal.TryParse(forecastQty, out decimal fqDec) ? fqDec : 0;

                    var cacheNCC = new Dictionary<string, DataTable>();

                    var groups = dtDetail.AsEnumerable()
                                         .GroupBy(r => new
                                         {
                                             Ma = r["MaCLVT"]?.ToString(),
                                             Ten = r["ChungLoaiVatTu"]?.ToString()
                                         })
                                         .OrderBy(g => g.Key.Ma);

                    foreach (var group in groups)
                    {
                        decimal groupTotal = group.Sum(r =>
                        {
                            decimal dm = (r["DinhMuc"] != null && r["DinhMuc"] != DBNull.Value) ? Convert.ToDecimal(r["DinhMuc"]) : 0;
                            decimal hh = (r["HaoHut"] != null && r["HaoHut"] != DBNull.Value) ? Convert.ToDecimal(r["HaoHut"]) : 0;
                            decimal gt = (r["DonGia"] != null && r["DonGia"] != DBNull.Value) ? Convert.ToDecimal(r["DonGia"]) : 0;
                            return (dm + hh) * slForecast * gt;
                        });

                        // Dòng tiêu đề nhóm
                        var groupRange = ws.Cells[currentRow, 1, currentRow, 13];
                        groupRange.Merge = true;
                        groupRange.Value = $"{ToRoman(groupIdx)}. {group.Key.Ten} (Total: {groupTotal:N0})";
                        groupRange.Style.Font.Bold = true;
                        groupRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        groupRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 241, 222));
                        groupRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentRow++;
                        groupIdx++;

                        // Cache NCC
                        if (!cacheNCC.TryGetValue(group.Key.Ma, out DataTable dtNCC))
                        {
                            string urlNCC = $"{URL}POMau/Get?action=GetNCC&para1={group.Key.Ma}";
                            string jsonNCC = Task.Run(async () => await _clientExtension.GetAsnyc(urlNCC)).Result;
                            dtNCC = JsonConvert.DeserializeObject<DataTable>(jsonNCC);
                            cacheNCC[group.Key.Ma] = dtNCC;
                        }

                        int itemIdx = 1;
                        foreach (var row in group)
                        {
                            string maNCC = row["MaNCC"]?.ToString();
                            string tenNCC = "";
                            if (dtNCC != null && !string.IsNullOrEmpty(maNCC))
                            {
                                var nccRow = dtNCC.AsEnumerable().FirstOrDefault(r => r["MaNCC"]?.ToString() == maNCC);
                                if (nccRow != null) tenNCC = nccRow["TenNCC"]?.ToString();
                            }

                            ws.Cells[currentRow, 1].Value = itemIdx++;
                            ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells[currentRow, 2].Value = row["MaVT"];

                            var desRange = ws.Cells[currentRow, 3, currentRow, 4];
                            desRange.Merge = true;
                            desRange.Value = row["ChiTiet"];

                            ws.Cells[currentRow, 5].Value = row["Position"];
                            ws.Cells[currentRow, 6].Value = row["MauVT"];
                            ws.Cells[currentRow, 7].Value = row["KhoVai"]?.ToString() + row["TenDVKV"]?.ToString();
                            ws.Cells[currentRow, 8].Value = row["TenDVVT"];
                            ws.Cells[currentRow, 9].Value = row["DinhMuc"];
                            ws.Cells[currentRow, 10].Value = tenNCC;

                            ws.Cells[currentRow, 11].Value = row["DonGia"];
                            ws.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0";

                            decimal dm2 = (row["DinhMuc"] != null && row["DinhMuc"] != DBNull.Value) ? Convert.ToDecimal(row["DinhMuc"]) : 0;
                            decimal hh2 = (row["HaoHut"] != null && row["HaoHut"] != DBNull.Value) ? Convert.ToDecimal(row["HaoHut"]) : 0;
                            decimal gt2 = (row["DonGia"] != null && row["DonGia"] != DBNull.Value) ? Convert.ToDecimal(row["DonGia"]) : 0;
                            decimal tongTien = (dm2 + hh2) * slForecast * gt2;
                            totalTongTien += tongTien;

                            ws.Cells[currentRow, 12].Value = tongTien;
                            ws.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0";

                            var ghiChu = row["GhiChu"];
                            ws.Cells[currentRow, 13].Value =
                                (ghiChu == null || ghiChu == DBNull.Value ||
                                 ghiChu.ToString() == "0" || string.IsNullOrWhiteSpace(ghiChu.ToString()))
                                ? "" : ghiChu.ToString();

                            currentRow++;
                        }
                    }

                    // Border toàn bộ vùng data
                    var dataRange = ws.Cells[13, 1, currentRow - 1, 13];
                    dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    // ── Summary ───────────────────────────────────────────────
                    int summaryRow = currentRow + 2;
                    int colStart = 7;
                    int colMid = 12;
                    int colEnd = 13;

                    string TotalCPSPHP = "0", TotalHCost = "0", ManuCost = "0", ProfitfCM = "0",
                           CostSpeTool = "0", IECost = "0", CostGMSP = "0", Cost3P = "0";

                    try
                    {
                        string urlTV = $"{URL}POMau/Get?action=GetTVCosting&para1={maKH}&para2={maHang}";
                        string jsonTV = Task.Run(async () => await _clientExtension.GetAsnyc(urlTV)).Result;
                        DataTable dtTV = JsonConvert.DeserializeObject<DataTable>(jsonTV);

                        if (dtTV != null && dtTV.Rows.Count > 0)
                        {
                            DataRow rowTV = dtTV.Rows[0];
                            TotalCPSPHP = rowTV["TotalCPSPHP"] == DBNull.Value ? "0" : rowTV["TotalCPSPHP"].ToString();
                            TotalHCost = rowTV["TotalHCost"] == DBNull.Value ? "0" : rowTV["TotalHCost"].ToString();
                            ManuCost = rowTV["ManuCost"] == DBNull.Value ? "0" : rowTV["ManuCost"].ToString();
                            ProfitfCM = rowTV["ProfitfCM"] == DBNull.Value ? "0" : rowTV["ProfitfCM"].ToString();
                            CostSpeTool = rowTV["CostSpeTool"] == DBNull.Value ? "0" : rowTV["CostSpeTool"].ToString();
                            IECost = rowTV["IECost"] == DBNull.Value ? "0" : rowTV["IECost"].ToString();
                            CostGMSP = rowTV["CostGMSP"] == DBNull.Value ? "0" : rowTV["CostGMSP"].ToString();
                            Cost3P = rowTV["Cost3P"] == DBNull.Value ? "0" : rowTV["Cost3P"].ToString();
                        }
                    }
                    catch { }

                    decimal smv = string.IsNullOrEmpty(view.GetFocusedRowCellValue("SMV")?.ToString()) ? 0
                                   : Convert.ToDecimal(view.GetFocusedRowCellValue("SMV").ToString());
                    decimal val1 = totalTongTien;
                    decimal val2 = val1 * (string.IsNullOrEmpty(TotalCPSPHP) ? 0 : Convert.ToDecimal(TotalCPSPHP));
                    decimal hcost = string.IsNullOrEmpty(TotalHCost) ? 0 : Convert.ToDecimal(TotalHCost);
                    decimal manu = string.IsNullOrEmpty(ManuCost) ? 0 : Convert.ToDecimal(ManuCost);
                    decimal prof = string.IsNullOrEmpty(ProfitfCM) ? 0 : Convert.ToDecimal(ProfitfCM);
                    decimal tool = string.IsNullOrEmpty(CostSpeTool) ? 0 : Convert.ToDecimal(CostSpeTool);
                    decimal ie = string.IsNullOrEmpty(IECost) ? 0 : Convert.ToDecimal(IECost);
                    decimal gmsp = string.IsNullOrEmpty(CostGMSP) ? 0 : Convert.ToDecimal(CostGMSP);
                    decimal c3p = string.IsNullOrEmpty(Cost3P) ? 0 : Convert.ToDecimal(Cost3P);
                    decimal val3 = val2 + (val2 * hcost);
                    decimal val4 = smv * manu;
                    decimal val5 = val4 * prof;
                    decimal val6 = tool;
                    decimal val7 = ie;
                    decimal val8 = gmsp;
                    decimal val9 = c3p;

                    string[,] summaryItems = new string[,]
                    {
                { "(1) Total material cost price per pc(I)+(II)+(III)+(IV)+(V)", "" },
                { "(2) Total material cost price per pc with wastage",            TotalCPSPHP },
                { "(3) Total material cost with handling cost",                   TotalHCost },
                { "(4) Manufacturer cost(cutting, sewing, +checking + packing)", ManuCost },
                { "(5) Profit for CM",                                            ProfitfCM },
                { "(6) Cost for special tool( if have)",                          "" },
                { "(7) Im-export cost",                                           "" },
                { "(8) Cost for renting Gore machine + Spare part",               "" },
                { "(9)Cost for third party",                                      "" },
                    };

                    decimal[] cellValues = { val1, val2, val3, val4, val5, val6, val7, val8, val9 };

                    for (int i = 0; i < summaryItems.GetLength(0); i++)
                    {
                        string label = summaryItems[i, 0];
                        string midVal = summaryItems[i, 1];
                        int r = summaryRow + i;

                        int labelEndCol = (i == 3) ? colMid - 2 : (i == 0 ? colMid : colMid - 1);
                        var labelRange = ws.Cells[r, colStart, r, labelEndCol];
                        labelRange.Merge = true;
                        labelRange.Value = label;
                        labelRange.Style.Font.Bold = true;
                        labelRange.Style.WrapText = true;
                        labelRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        labelRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        if (i == 3)
                        {
                            var smvCell = ws.Cells[r, colMid - 1];
                            smvCell.Value = smv;
                            smvCell.Style.Numberformat.Format = "#,##0.00";
                            smvCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            smvCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            smvCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(189, 215, 238));
                        }

                        if (!string.IsNullOrEmpty(midVal))
                        {
                            ws.Cells[r, colMid].Value = midVal;
                            ws.Cells[r, colMid].Style.Numberformat.Format = "#,##0.00";
                            ws.Cells[r, colMid].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }

                        ws.Cells[r, colEnd].Value = cellValues[i];
                        ws.Cells[r, colEnd].Style.Numberformat.Format = "#,##0";
                    }

                    int totalRow = summaryRow + summaryItems.GetLength(0);

                    var totalLabelRange = ws.Cells[totalRow, colStart, totalRow, colMid - 1];
                    totalLabelRange.Merge = true;
                    totalLabelRange.Value = "SALE COST TOTAL [sum (3), (9)] (usd)";
                    totalLabelRange.Style.Font.Bold = true;
                    totalLabelRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    totalLabelRange.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    totalLabelRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    totalLabelRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    var totalValueCell = ws.Cells[totalRow, colEnd];
                    totalValueCell.Value = val3 + val4 + val5 + val6 + val7 + val8 + val9;
                    totalValueCell.Style.Numberformat.Format = "#,##0";
                    totalValueCell.Style.Font.Bold = true;
                    totalValueCell.Style.Font.Size = 13;
                    totalValueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    totalValueCell.Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    ws.Cells[totalRow, colMid].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[totalRow, colMid].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    var summaryTableRange = ws.Cells[summaryRow, colStart, totalRow, colEnd];
                    summaryTableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    summaryTableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    summaryTableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    summaryTableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    package.SaveAs(new FileInfo(sfd.FileName));

                    var result = XtraMessageBox.Show(
                        "Xuất file thành công! Bạn có muốn mở file không?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (view == null || info == null) return;
            int groupLevel = view.GetRowLevel(e.RowHandle);
            GridColumn groupColumn = info.Column;
            if (groupColumn == gridColumn32)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (view.IsGroupRow(e.RowHandle))
            {
                Color textColor = Color.Black;
                switch (groupLevel)
                {
                    case 0: textColor = Color.MediumBlue; break;
                    case 1: textColor = Color.Maroon; break;
                }
                e.Appearance.ForeColor = textColor;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void btnTVCosting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var frm = new frmPOMau_ThuVienCost();
            frm.ShowDialog();
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!_PQXoa)
            {
                XtraMessageBox.Show("User không có quyền xóa.Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null) return;
            bool daDuyetTao = row["DuyetTao"] != null && row["DuyetTao"] != DBNull.Value
                     && Convert.ToBoolean(row["DuyetTao"]);
            if (daDuyetTao)
            {
                XtraMessageBox.Show("Costing này đã được duyệt, không thể xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (XtraMessageBox.Show("Bạn có chắc muốn xóa costing này không?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string maPhieu = row["MaPhieu"].ToString();
            string url = $"{URL}POMau/Get?action=Delete&para1={maPhieu}&para2={GlobleData.UserName}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            LoadData();
            if (gridView1.RowCount == 0)
                gridControl2.DataSource = null;
        }

        private void barButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                GridView view = gridView1;
                if (view == null) return;
                if (XtraMessageBox.Show("Bạn có muốn đồng bộ không?", "Xác nhận",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                int rowHandle = view.FocusedRowHandle;
                if (rowHandle < 0)
                {
                    return;
                }

                object maPhieu = view.GetRowCellValue(rowHandle, "MaPhieu");
                if (maPhieu != null && maPhieu != DBNull.Value && !string.IsNullOrEmpty(maPhieu.ToString()))
                {
                    string urlCT = $"{URL}POMau/Post?Action=PostDB&para1={maPhieu.ToString()}&para2={GlobleData.UserName}";
                    string msResultCT = Task.Run(async () => { return await _clientExtension.PostAsync(urlCT, null); }).Result;
                    if (msResultCT.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        LoadData();
                    }
                }
            }
            catch { }

        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName != "TrangThai")
                return;
            if (e.CellValue == null)
                return;
            if (e.CellValue.ToString() == "Đang xử lý")
            {
                e.Appearance.ForeColor = Color.Orange;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
            else if (e.CellValue.ToString() == "Đã xác nhận")
            {
                e.Appearance.ForeColor = Color.DarkGreen;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
            else if (e.CellValue.ToString() == "Đã cập nhật NCC")
            {
                e.Appearance.ForeColor = Color.Green;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
            else if (e.CellValue.ToString() == "Đã cập nhật SMV")
            {
                e.Appearance.ForeColor = Color.DarkBlue;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
        }
    }
}
