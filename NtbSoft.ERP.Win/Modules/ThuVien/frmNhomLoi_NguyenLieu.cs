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
    public partial class frmNhomLoi_NguyenLieu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        public frmNhomLoi_NguyenLieu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvNhomLoi.FocusedRowChanged += GrvNhomLoi_FocusedRowChanged;
            grvNhomLoi.InitNewRow += GrvNhomLoi_InitNewRow;
            grvCTNhomLoi.InitNewRow += GrvCTNhomLoi_InitNewRow;
            LoadNhomLoi();
        }

        private void GrvCTNhomLoi_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            grvCTNhomLoi.SetRowCellValue(e.RowHandle, grvCTNhomLoi.Columns["Pid"], 0);
        }

        private void GrvNhomLoi_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            grvNhomLoi.SetRowCellValue(e.RowHandle, grvNhomLoi.Columns["ID"], 0);
            //throw new NotImplementedException();
        }

        private void LoadNhomLoi()
        {
            string url = string.Format("{0}", URL + $"NhomLoiNL/Get?action=GetNhomLoi");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            int rowHandle = grvNhomLoi.FocusedRowHandle;
            grcNhomLoi.DataSource = tbl;
            grvNhomLoi.FocusedRowHandle = rowHandle;
            if (rowHandle <= 0)
            {
                LoadCTNhomLoi();
            }
        }
        private void LoadCTNhomLoi()
        {
            var drFocus = grvNhomLoi.GetFocusedDataRow() as DataRow;
            if (drFocus is null) return;
            string IDNhomLoi = drFocus["ID"].ToString();
            string url = string.Format("{0}", URL + $"NhomLoiNL/Get?action=GetCTNhomLoi&&Para1={IDNhomLoi}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            grcCTNhomLoi.DataSource = tbl.Rows.Count == 0 ? CreateTblNhomLoiCT() : tbl;
        }
        private void GrvNhomLoi_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            LoadCTNhomLoi();
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadNhomLoi();
        }

        private void btnLuuNL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = grcCTNhomLoi;
            var dtSource = grcNhomLoi.DataSource as DataTable;
            if (dtSource.Rows.Count == 0) return;
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr["BackColor"].ToString().Equals(""))
                    dr["BackColor"] = ParseHexa("fff");
                else
                    dr["BackColor"] = ParseHexa(dr["BackColor"]);

                if (dr["ForColor"].ToString().Equals(""))
                    dr["ForColor"] = ParseHexa("fff");
                else
                    dr["ForColor"] = ParseHexa(dr["ForColor"]);
            }
            string url = string.Format("{0}", URL + $"NhomLoiNL/Post?action=SaveNL");
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSource); }).Result;
            if (json.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNhomLoi();
            }
        }

        private void btnLuuCT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = grcNhomLoi;
            var dtSource = grcCTNhomLoi.DataSource as DataTable;
            if (dtSource.Rows.Count == 0) return;
            DataTable dtSave = CreateTblNhomLoiCT();
            DataRow drFocusNL = grvNhomLoi.GetFocusedDataRow();
            if (drFocusNL is null) return;
            foreach(DataRow dr in dtSource.Rows)
            {
                var drNew = dtSave.NewRow();
                drNew["Pid"] = dr["Pid"];
                drNew["MaLoi"] = dr["MaLoi"];
                drNew["NhomLoi"] = drFocusNL["ID"];
                drNew["TenLoi"] = dr["TenLoi"];
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["Sort"] = 0;
                drNew["LoaiLoi"] = 0;
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"NhomLoiNL/Post?action=SaveCT");
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (json.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadCTNhomLoi();
            }
        }

        private void btnXoaNL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"Nếu đã phát sinh dữ liệu cẩn thận mất dữ liệu!{Environment.NewLine}Bạn có chắc muốn xóa không ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes) return;
            var drFocus = grvNhomLoi.GetFocusedDataRow() as DataRow;
            if (drFocus is null) return;
            string IDNhomLoi = drFocus["ID"].ToString();
            string url = string.Format("{0}", URL + $"NhomLoiNL/Delete?action=DeleteNL&&Id={IDNhomLoi}");
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, new DataTable()); }).Result;
            if (json.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNhomLoi();
            }
        }

        private void btnXoaCT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"Nếu đã phát sinh dữ liệu cẩn thận mất dữ liệu!{Environment.NewLine}Bạn có chắc muốn xóa không ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes) return;
            var drFocus = grvCTNhomLoi.GetFocusedDataRow() as DataRow;
            if (drFocus is null) return;
            string IDCTNhomLoi = drFocus["Pid"].ToString();
            string url = string.Format("{0}", URL + $"NhomLoiNL/Delete?action=DeleteCT&&Id={IDCTNhomLoi}");
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, new DataTable()); }).Result;
            if (json.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadCTNhomLoi();
            }
        }
        private DataTable CreateTblNhomLoiCT()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Pid", typeof(int));
            dt.Columns.Add("MaLoi", typeof(string));
            dt.Columns.Add("NhomLoi", typeof(int));
            dt.Columns.Add("TenLoi", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("LoaiLoi", typeof(int));
            dt.Columns.Add("MaLoiHide", typeof(string));
            return dt;

        }
        public string ParseHexa(object value)
        {
            if (value == null || value.ToString().Trim() == string.Empty) return "";
            int v = 0;
            if (int.TryParse(value.ToString(), out v))
            {
                v = v & 0x00ffffff;
            }
            else
            {
                v = Int32.Parse(value.ToString().TrimStart(new char[] { '#' }),
                    System.Globalization.NumberStyles.HexNumber) & 0x00ffffff;
            }
            string result = string.Format("{0}{1}", "#", v.ToString("X"));
            return result;
        }
    }
}
