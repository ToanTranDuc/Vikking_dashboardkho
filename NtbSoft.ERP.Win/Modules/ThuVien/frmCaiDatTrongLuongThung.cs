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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmCaiDatTrongLuongThung : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public static string MaHang = "All";
        public frmCaiDatTrongLuongThung()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadMaHang();
        }
        private void LoadMaHang()
        {
            string url = string.Format("{0}", URL + $"DicQCDongThung/GetPKL?action=GetMH_PKH&para1=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            grcMaHang.DataSource = tbl;
            if(MaHang != "All")
            {
                var drCheckMH = tbl.AsEnumerable().Where(x => x["MaHang"].ToString() == MaHang).FirstOrDefault();
                if (drCheckMH is null) return;
                var index = tbl.Rows.IndexOf(drCheckMH);
                grvMaHang.BeginUpdate();
                grvMaHang.FocusedRowHandle = index;
                grvMaHang.MakeRowVisible(index);
                grvMaHang.TopRowIndex = index;
                grvMaHang.EndUpdate();
            }
            LoadQuiCach();
        }
        private void LoadQuiCach()
        {
            try
            {
                var drFocus = grvMaHang.GetFocusedDataRow();
                if (drFocus is null) return;
                string url = string.Format("{0}", URL + $"DicQCDongThung/GetPKL?action=GetPKH&para1={drFocus["MaHang"].ToString()}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);                
                grcQuiCach.DataSource = tbl;
            }
            catch
            {
               
            }
        }
        private void LoadPCB_KL()
        {
            var drFocus = grvMaHang.GetFocusedDataRow();
            if (drFocus is null) return;
            string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?action=Get_PKH&para1={drFocus["MaHang"].ToString()}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            dgrTLKL.DataSource = tbl;
        }
        private void SaveQC()
        {
            this.ActiveControl = button1;
            var dtSource = grcQuiCach.DataSource as DataTable;
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            DataTable dtSave = KHDongThungLib.CreateTblQC();
            foreach(DataRow dr in dtSource.Rows)
            {
                var drSave = dtSave.NewRow();
                drSave["MaQuiCach"] = dr["MaQuiCach"];
                drSave["CanNang"] = dr["CanNang"];
                drSave["NguoiTao"] = GlobleData.UserName;
                dtSave.Rows.Add(drSave);
            }

            string url = string.Format("{0}", URL + $"DicQCDongThung/PostPKH?action=UpdateTL_PKH");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadQuiCach();
            }

        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveQC();
        }

        private void grvMaHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadQuiCach();
            LoadPCB_KL();
        }

        private void btnLuuKL_Click(object sender, EventArgs e)
        {
            this.ActiveControl = button1;
            var dtSource = dgrTLKL.DataSource as DataTable;
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            var drFocus = grvMaHang.GetFocusedDataRow();
            DataTable dtSave = KHDongThungLib.CreateTblPCB();
            
            foreach (DataRow dr in dtSource.Rows)
            {
                var drSave = dtSave.NewRow();
                drSave["StyleID"] = drFocus["MaHang"];
                drSave["DauSizeID"] = dr["DauSizeID"];
                drSave["SizeID"] = dr["SizeID"];
                drSave["SL_KhoiLuong"] = dr["SL_KhoiLuong"];             
                dtSave.Rows.Add(drSave);
            }

            string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Post?action=PostTL_KL_PKH");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadPCB_KL();
            }
        }

        private void btnLuuQC_Click(object sender, EventArgs e)
        {
            SaveQC();
        }

        private void btnNaplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadQuiCach();
            LoadPCB_KL();
        }

        private void txtKL_EditValueChanged(object sender, EventArgs e)
        {
            DataTable tbl = dgrTLKL.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;

            if (!decimal.TryParse(txtKL.EditValue?.ToString(), out decimal pcbValue))
                pcbValue = 0;
            foreach(DataRow dr in tbl.Rows)
            {
                dr["SL_KhoiLuong"] = pcbValue;
            }
            //for (int i = 0; i < tbl.Rows.Count; i++)
            //{
            //   // if ((bool)tbl.Rows[i]["Chon"])
            //   tbl.Rows[i]["SL_KhoiLuong"] = pcbValue;
            //}
            dgrTLKL.DataSource = tbl;
        }
        private void barCheckLap_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barCheckLap.Checked)
            {
                barCheckLap2.Checked = false;
            }
        }


        private void barCheckLap2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barCheckLap2.Checked)
            {
                barCheckLap.Checked = false;
            }
        }

        private void grvCaiDatTS_KeyPress(object sender, KeyPressEventArgs e)
        {
            var editor = sender as TextEdit;
            if (editor == null) return;

            GridView view = grvCaiDatTS;
            string field = view.FocusedColumn?.FieldName;
            if (field == null) return;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (!barCheckLap.Checked && !barCheckLap2.Checked) return;

            var drFocus = view.GetFocusedDataRow();
            if (drFocus == null) return;

            var data = dgrTLKL.DataSource as DataTable;
            if (data == null || data.Rows.Count == 0) return;

            string curText = editor.EditValue?.ToString() ?? "";
            string newText = "";
            if (e.KeyChar == '\b')
            {
                if (editor.SelectionLength > 0)
                {
                    int start = editor.SelectionStart;
                    newText = curText.Remove(start, editor.SelectionLength);
                }
                else if (curText.Length > 0 && editor.SelectionLength > 0)
                {
                    newText = curText.Remove(editor.SelectionLength - 1, 1);
                }
                newText = string.IsNullOrEmpty(newText) ? "0" : newText;
            }
            else if (char.IsDigit(e.KeyChar) || e.KeyChar == '.')
            {
                if (editor.SelectionLength > 0)
                {
                    int start = editor.SelectionStart;
                    newText = curText.Remove(start, editor.SelectionLength);
                    newText = newText.Insert(start, e.KeyChar.ToString());
                }
                else
                {
                    int start = editor.SelectionStart;
                    newText = curText.Insert(start, e.KeyChar.ToString());
                }
            }
            else
                return;

            object newValue = null;
            if (field == "SL_KhoiLuong")
            {
                if (double.TryParse(newText, out double dValue))
                    newValue = dValue;
            }
            else
            {
                if (double.TryParse(newText, out double iValue))
                    newValue = iValue;
            }
            if (newValue == null) return;

            if (barCheckLap.Checked)
            {
                string focusedSizeID = drFocus["SizeID"].ToString();
                foreach (DataRow dr in data.Rows)
                {
                    if (dr["SizeID"].ToString() == focusedSizeID)
                    {
                        dr[field] = newValue;
                    }
                }
            }
            else if (barCheckLap2.Checked)
            {
                string focusedDauSizeID = drFocus["DauSizeID"].ToString();
                foreach (DataRow dr in data.Rows)
                {
                    if (dr["DauSizeID"].ToString() == focusedDauSizeID)
                    {
                        dr[field] = newValue;
                    }
                }
            }
            view.RefreshData();
        }
    }
}
