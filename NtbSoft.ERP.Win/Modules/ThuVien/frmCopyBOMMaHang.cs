using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmCopyBOMMaHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        private string _mahang = string.Empty, _mau = string.Empty;
        public string URL = string.Empty;
        HttpClientExtension _clientExtension;
        DataTable _dtData;
        DataTable tblBOM;
        public frmCopyBOMMaHang(string mahang)
        {
            InitializeComponent();
            _clientExtension = new HttpClientExtension();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            this._mahang = mahang;
            _dtData = new DataTable();
            tblBOM = new DataTable();

        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookup();
            LoadData();
        }
        public void CreateSearchLookup()
        {
            string urlHH = string.Format("{0}?", URL + "BOM/GETMH");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEditTuMaHang.Properties.DataSource = tblHH;
            searchLookUpEditTuMaHang.Properties.ValueMember = "MaHang";
            searchLookUpEditTuMaHang.Properties.DisplayMember = "TenHang";
            searchLookUpEditTuMaHang.EditValue = _mahang;

            var rowsToKeep = tblHH.AsEnumerable()
           .Where(row => row.Field<string>("MaHang") != _mahang)
           .ToList();
            if (rowsToKeep.Any())
            {
                DataTable tblDHH = rowsToKeep.CopyToDataTable();
                searchLookUpEditDenMaHang.Properties.DataSource = tblDHH;
                searchLookUpEditDenMaHang.Properties.ValueMember = "MaHang";
                searchLookUpEditDenMaHang.Properties.DisplayMember = "TenHang";
            }
        }
        private void LoadData()
        {
            string urlTK = string.Format("{0}?_mahang={1}", URL + "BOM/Get_TikKiem", _mahang);
            string jsonTK = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTK); }).Result;
            tblBOM = JsonConvert.DeserializeObject<DataTable>(jsonTK);
            if (tblBOM.Rows.Count > 0 && tblBOM != null)
            {
                foreach (DataRow dr in tblBOM.Rows)
                {
                    dr["MaMau"] = "";
                }
                gridCtr_ChiTiet.DataSource = tblBOM;
                _dtData = tblBOM;
            }
            else
                gridCtr_ChiTiet.DataSource = null;
        }

        private void searchLookUpEditDenMaHang_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditDenMaHang.EditValue != null)
            {
                string urlMau_DH = string.Format("{0}?_mahang={1}", URL + "BOM/GET_Mau", searchLookUpEditDenMaHang.EditValue.ToString());
                string jsonMau_DH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau_DH); }).Result;
                DataTable tblMau_DH = JsonConvert.DeserializeObject<DataTable>(jsonMau_DH);
                searchLookUpEditMau.Properties.DataSource = tblMau_DH;
                searchLookUpEditMau.Properties.ValueMember = "MaMau";
                searchLookUpEditMau.Properties.DisplayMember = "TenMau";
                LoadData();
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column == colSTT)
            {
                if (e.RowHandle < 0)
                {
                    e.DisplayText = "0";
                }
                else
                {
                    string sttValue = Convert.ToString(e.RowHandle + 1);
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);

                }
            }
        }

        private void searchLookUpEditMau_EditValueChanged(object sender, EventArgs e)
        {
            //    if (searchLookUpEditMau.EditValue != null)
            //    {
            //        var selectedColorValue = searchLookUpEditMau.Text;
            //        for (int i = 0; i < gridView1.RowCount; i++)
            //        {
            //            gridView1.SetRowCellValue(i, "MaMau", selectedColorValue);
            //        }
            //    }

        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            searchLookUpEditDenMaHang.EditValue = null;
            searchLookUpEditMau.EditValue = null;
            searchLookUpEditMau.Properties.View.ClearSelection();
            CreateSearchLookup();
            LoadData();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEditDenMaHang.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn mã hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (_mau.ToString() == "")
                {
                    XtraMessageBox.Show("Vui lòng chọn màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                DataTable tblBOMSave = gridCtr_ChiTiet.DataSource as DataTable;
                if (tblBOMSave.Rows.Count > 0 && tblBOMSave != null)
                {
                    DataTable tblSave = new DataTable();

                    tblSave.Columns.Add("ID", typeof(int));
                    tblSave.Columns.Add("MaBom", typeof(string));
                    tblSave.Columns.Add("MaDH", typeof(string));
                    tblSave.Columns.Add("MaVatTu", typeof(string));
                    tblSave.Columns.Add("TenVatTu", typeof(string));
                    tblSave.Columns.Add("Mau", typeof(string));
                    tblSave.Columns.Add("KhoVai", typeof(string));
                    tblSave.Columns.Add("DonViTinh", typeof(string));
                    tblSave.Columns.Add("DinhMucMuaHang", typeof(double));
                    tblSave.Columns.Add("DinhMucThucTe", typeof(double));
                    tblSave.Columns.Add("DinhMucKH", typeof(double));
                    tblSave.Columns.Add("GhiChu", typeof(string));
                    tblSave.Columns.Add("NgayLap", typeof(string));
                    tblSave.Columns.Add("MaSP", typeof(string));
                    tblSave.Columns.Add("MaTiLe", typeof(string));
                    tblSave.Columns.Add("SLChungTu", typeof(int));
                    tblSave.Columns.Add("PhanLoaiVT", typeof(string));
                    tblSave.Columns.Add("NhaCungCap", typeof(string));
                    tblSave.Columns.Add("POID", typeof(string));
                    tblSave.Columns.Add("MaMau", typeof(string));
                    tblSave.Columns.Add("DauSizeID", typeof(string));
                    tblSave.Columns.Add("SizeID", typeof(string));
                    tblSave.Columns.Add("SLSanPham", typeof(int));
                    tblSave.Columns.Add("SLDinhMuc", typeof(int));
                    tblSave.Columns.Add("HaoHut", typeof(double));
                    tblSave.Columns.Add("SL", typeof(int));
                    tblSave.Columns.Add("SoChungTu", typeof(string));
                    tblSave.Columns.Add("SanPham", typeof(int));
                    tblSave.Columns.Add("ChieuDaiCuon", typeof(double));
                    tblSave.Columns.Add("SoLuongCan", typeof(int));
                    tblSave.Columns.Add("SLCanBangMau", typeof(double));
                    tblSave.Columns.Add("DonGia", typeof(double));
                    tblSave.Columns.Add("ThanhTien", typeof(double));
                    tblSave.Columns.Add("SLThucTe", typeof(int));
                    tblSave.Columns.Add("SLDat", typeof(int));
                    tblSave.Columns.Add("Chon", typeof(int));
                    tblSave.Columns.Add("NgayCN", typeof(DateTime));
                    tblSave.Columns.Add("ID_DM", typeof(string));
                    tblSave.Columns.Add("MaKeToan", typeof(string));
                    tblSave.Columns.Add("MaVTMau", typeof(string));
                    tblSave.Columns.Add("TiLeSP", typeof(double));
                    tblSave.Columns.Add("ChieuDai", typeof(double));
                    tblSave.Columns.Add("MaHangCopy", typeof(string));
                    tblSave.Columns.Add("NguoiCopy", typeof(string));
                    for (int i = 0; i < tblBOMSave.Rows.Count; i++)
                    {
                        DataRow _dr = tblSave.NewRow();
                        _dr["ID"] = 0;
                        _dr["ID_DM"] = tblBOMSave.Rows[i]["ID_DM"];
                        _dr["MaVTMau"] = tblBOMSave.Rows[i]["MaVTMau"];
                        _dr["TiLeSP"] = tblBOMSave.Rows[i]["TiLeSP"].ToString() == "" ? 0 : tblBOMSave.Rows[i]["TiLeSP"];
                        _dr["ChieuDai"] = tblBOMSave.Rows[i]["ChieuDai"].ToString() == "" ? 0 : tblBOMSave.Rows[i]["ChieuDai"];
                        _dr["MaBom"] = searchLookUpEditDenMaHang.EditValue.ToString() + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day;
                        _dr["MaDH"] = searchLookUpEditDenMaHang.EditValue.ToString();
                        _dr["MaVatTu"] = tblBOMSave.Rows[i]["MaVatTu"];
                        _dr["TenVatTu"] = tblBOMSave.Rows[i]["TenVatTu"];
                        _dr["Mau"] = tblBOMSave.Rows[i]["Mau"];
                        _dr["KhoVai"] = tblBOMSave.Rows[i]["KhoVai"];
                        _dr["DonViTinh"] = tblBOMSave.Rows[i]["DonViTinh"];
                        _dr["DinhMucMuaHang"] = tblBOMSave.Rows[i]["DinhMucMuaHang"];
                        _dr["DinhMucThucTe"] = tblBOMSave.Rows[i]["DinhMucThucTe"];
                        _dr["DinhMucKH"] = tblBOMSave.Rows[i]["DinhMucKH"];
                        _dr["GhiChu"] = tblBOMSave.Rows[i]["GhiChu"];
                        _dr["NgayLap"] = DateTime.Now;
                        _dr["MaSP"] = "";
                        _dr["MaTiLe"] = "";
                        _dr["SLChungTu"] = 0;
                        _dr["PhanLoaiVT"] = tblBOMSave.Rows[i]["MaNhomVT"];
                        _dr["NhaCungCap"] = "";
                        _dr["POID"] = "";
                        _dr["MaMau"] = tblBOMSave.Rows[i]["MaMau"].ToString() == "" ? "ALL" : tblBOMSave.Rows[i]["MaMau"];
                        _dr["DauSizeID"] = tblBOMSave.Rows[i]["DauSizeID"].ToString() == "" ? "ALL" : tblBOMSave.Rows[i]["DauSizeID"];
                        _dr["SizeID"] = tblBOMSave.Rows[i]["SizeID"].ToString() == "" ? "ALL" : tblBOMSave.Rows[i]["SizeID"];
                        _dr["SLSanPham"] = 0;
                        _dr["SLDinhMuc"] = 0;
                        _dr["HaoHut"] = 0;
                        _dr["SL"] = 0;
                        _dr["SoChungTu"] = "";
                        _dr["SanPham"] = 0;
                        _dr["ChieuDaiCuon"] = 0;
                        _dr["SoLuongCan"] = 0;
                        _dr["SLCanBangMau"] = 0;
                        _dr["DonGia"] = 0;
                        _dr["ThanhTien"] = 0;
                        _dr["SLThucTe"] = 0;
                        _dr["SLDat"] = 0;
                        _dr["Chon"] = 0;
                        _dr["NgayCN"] = DateTime.Now;
                        _dr["MaKeToan"] = "";
                        _dr["MaHangCopy"] = searchLookUpEditTuMaHang.EditValue.ToString();
                        _dr["NguoiCopy"] = GlobleData.UserName;
                        tblSave.Rows.Add(_dr);
                    }
                    string urlSaveBOMDH = string.Format("{0}?", URL + "BOM/PostBomVTCopy");
                    string mssSaveBOMDH = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveBOMDH, tblSave); }).Result;
                    if (mssSaveBOMDH.ToLower() != "true")
                        XtraMessageBox.Show(mssSaveBOMDH);
                    else
                        clsWaitForm.ShowSuccessForm(this, 3000);
                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridView3_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            _mau = string.Join(";", gridView3.GetSelectedRows().Select(rowHandle => gridView3.GetRowCellValue(rowHandle, searchLookUpEditMau.Properties.ValueMember)));
            searchLookUpEditMau.EditValue = _mau;
            string mau = string.Join(";", gridView3.GetSelectedRows().Select(rowHandle => gridView3.GetRowCellValue(rowHandle, searchLookUpEditMau.Properties.DisplayMember)));
            UpdateGridWithColorValues(mau);
        }
        private void searchLookUpEditMau_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string mau = string.Join(";", gridView3.GetSelectedRows().Select(rowHandle => gridView3.GetRowCellValue(rowHandle, searchLookUpEditMau.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(mau.ToString()))
                e.DisplayText = "Chọn Màu";
            else
                e.DisplayText = mau.ToString();
        }
        private void UpdateGridWithColorValues(string mau)
        {
            var colorLists = mau.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var colorSets = new HashSet<string>(colorLists);
            if (colorLists.Length >= 1)
            {
                for (int i = tblBOM.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = tblBOM.Rows[i];
                    string rowColor = row["MaMau"]?.ToString();
                    if (!colorSets.Contains(rowColor) && rowColor.ToString() != "")
                    {
                        tblBOM.Rows.Remove(row);
                    }
                }
            }
            else
            {
                //foreach (DataRow dr in tblBOM.Rows)
                //{
                //    dr["MaMau"] = "";
                //}
                LoadData();
            }
            if (_dtData.Rows.Count > 0 && _dtData != null)
            {

                if (_mau.ToString() != "")
                {
                    var selectedColors = searchLookUpEditMau.Text.ToString();
                    var colorList = selectedColors.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (colorList.Length == 1)
                    {
                        for (int i = 0; i < gridView1.RowCount; i++)
                        {
                            gridView1.SetRowCellValue(i, "MaMau", colorList[0]);
                        }
                    }
                    else if (colorList.Length > 1)
                    {
                        for (int i = 0; i < gridView1.RowCount; i++)
                        {
                            gridView1.SetRowCellValue(i, "MaMau", colorList[0]);
                        }
                        for (int i = 1; i < colorList.Length; i++)
                        {
                            int rowCount = _dtData.Rows.Count;
                            for (int j = 0; j < rowCount; j++)
                            {
                                DataRow newRow = _dtData.NewRow();
                                newRow.ItemArray = _dtData.Rows[j].ItemArray.Clone() as object[];
                                tblBOM.Rows.Add(newRow);
                            }

                            for (int x = rowCount; x < tblBOM.Rows.Count; x++)
                            {
                                gridView1.SetRowCellValue(x, "MaMau", colorList[i]);
                            }
                        }

                    }

                }
            }

        }
    }
}
