using DevExpress.XtraEditors;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmPhieuNhapKhoV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private int _stt = 0;
        public frmPhieuNhapKhoV1()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            dateEditNgayNhapKho.EditValue = DateTime.Now;
            LoadSTT();
            CreateSearchLookUpSoLo();
            LoadData();
        }
        private void CreateSearchLookUpSoLo()
        {
            string url = string.Format("{0}?", URL + "PhieuNhapKho/GetSoLo");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditSoLo.Properties.DataSource = null;
                searchLookUpEditSoLo.EditValue = null;
                return;
            } 
               
 
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditSoLo.Properties.DataSource = tbl;
            searchLookUpEditSoLo.Properties.DisplayMember = "SoLo";
            searchLookUpEditSoLo.Properties.ValueMember = "SoLoID";
            searchLookUpEditSoLo.RefreshEditValue();
            searchLookUpEditSoLo.Refresh();
        }
        private void LoadSTT()
        {
            string url = string.Format("{0}?", URL + "PhieuNhapKho/GetMaxSTT");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _stt = Convert.ToInt32(tbl.Rows[0][0])+1;
        }
        private void LoadData()
        {
            string url = string.Format("{0}?soloid={1}", URL + "PhieuNhapKho/GetDanhSach", searchLookUpEditSoLo.EditValue == null ? "" : searchLookUpEditSoLo.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gridControl2.DataSource = null;
                return;
            }
            DataTable tblDanhSach = JsonConvert.DeserializeObject<DataTable>(json);
            var Rows = tblDanhSach.AsEnumerable()
             .GroupBy(row => new
             {
                 NhaCungCap = row["NhaCungCap"],
                 SoChungTu = row["SoChungTu"],
                 NgayChungTu = row["NgayChungTu"],
                 SoBienBan = row["SoBienBan"],
                 NgayBienBan = row["NgayBienBan"],
                 SoHopDong = row["SoHopDong"]
             })
            .Select(group => group.First())
             .ToList();
            gridControl2.DataSource = tblDanhSach;
        }

        private void searchLookUpEditSoLo_EditValueChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CreateSearchLookUpSoLo();
            //LoadData();
        }

        private void gridView2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "DonGia" && e.Value.ToString() != "")
            {
                decimal theoCT = Convert.ToDecimal(gridView2.GetRowCellValue(e.RowHandle, "TheoCT"));
                decimal donGia = Convert.ToDecimal(e.Value);
                decimal thanhTien = theoCT * donGia;
                gridView2.SetRowCellValue(e.RowHandle, "ThanhTien", thanhTien);
            }
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tbl = gridControl2.DataSource as DataTable;
            if (tbl == null) return;
            var rows = tbl.AsEnumerable()
                                    .Where(row => row.IsNull("DonGia") || Convert.ToDecimal(row["DonGia"]) <= 0)
                                    .ToList();

            if (rows.Any())
            {
                XtraMessageBox.Show("Đơn giá không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("SoPhieu", typeof(string));
            dtSave.Columns.Add("NgayNhapKho", typeof(string));
            dtSave.Columns.Add("SoLoID", typeof(string));
            dtSave.Columns.Add("MaVTID", typeof(string));
            dtSave.Columns.Add("MaMauVT", typeof(string));
            dtSave.Columns.Add("MaDVVT", typeof(string));
            dtSave.Columns.Add("KhoVaiID", typeof(string));
            dtSave.Columns.Add("TheoCT", typeof(decimal));
            dtSave.Columns.Add("ThucNhap", typeof(decimal));
            dtSave.Columns.Add("DonGia", typeof(decimal));
            dtSave.Columns.Add("ThanhTien", typeof(decimal));
            dtSave.Columns.Add("GhiChu", typeof(string));
            dtSave.Columns.Add("STT", typeof(int));
            dtSave.Columns.Add("NguoiTao", typeof(string));
            foreach (DataRow dr in tbl.Rows)
            {

                DataRow _dr = dtSave.NewRow();
                _dr["SoPhieu"] = "";
                _dr["NgayNhapKho"] = Convert.ToDateTime(dateEditNgayNhapKho.EditValue).ToString("dd-MM-yyyy");
                _dr["SoLoID"] = dr["SoLoID"]; 
                _dr["MaVTID"] = dr["MaVTID"];
                _dr["MaMauVT"] = dr["MaMauVT"];
                _dr["MaDVVT"] = dr["MaDVVT"];
                _dr["KhoVaiID"] = dr["KhoVaiID"]; 
                _dr["TheoCT"] = dr["TheoCT"];
                _dr["ThucNhap"] = dr["ThucNhap"];
                _dr["DonGia"] = dr["DonGia"];
                _dr["ThanhTien"] = dr["ThanhTien"];
                _dr["GhiChu"] = dr["GhiChu"];
                _dr["STT"] = _stt;
                _dr["NguoiTao"] = GlobleData.UserName;
                dtSave.Rows.Add(_dr);
            }
            string url = string.Format("{0}?", URL + "PhieuNhapKho/Post");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                CreateSearchLookUpSoLo();
                LoadSTT();
                searchLookUpEditSoLo.EditValue = null;
              

            }

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmLichSuPhieuNhapKhoV1 frm = new frmLichSuPhieuNhapKhoV1();
            frm.ShowDialog();
            CreateSearchLookUpSoLo();
            LoadData();
        }

        private void gridView2_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridView2.FocusedColumn.FieldName.Contains("@"))
            {
                int outParse = -1;
                if (e != null && e.Value != null)
                {
                    bool flagParse = int.TryParse(e.Value.ToString(), out outParse);
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Value = 0;
                    }
                    else if (flagParse && outParse < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập > 0!.";
                        return;
                    }
                    else if (!flagParse)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!.";
                        return;
                    }
                }
            }
        }

        private void gridView2_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;


            if (column.FieldName == "TheoCT" || column.FieldName == "ThucNhap" || column.FieldName == "DonGia" || column.FieldName == "ThanhTien")
            {
                var value = view.GetFocusedValue();

                if (value != null &&
                    ((value is int && (int)value == 0) ||
                     (value is decimal && (decimal)value == 0) ||
                     (value is double && (double)value == 0) ||
                     (value is float && (float)value == 0)))
                {

                    view.SetFocusedValue(null);
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

    }
}
