using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
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
    public partial class frmChungLoaiVT : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _isNhapXuat = false;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();

        public frmChungLoaiVT()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            LoadDSChungLoai();
        }
        private void LoadDSChungLoai()
        {
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/GetKhoVatTu", "GetCLVT", "para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow dr = tbl.AsEnumerable().Where(x => x["ID"].ToString() == "").FirstOrDefault();
            if (dr != null)
                tbl.Rows.Remove(dr);
            dgrChungLoaiVT.DataSource = tbl;
        }

        private void gridViewChungLoaiVT_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0 && e.FocusedRowHandle != int.MinValue)
            {
                DataTable tbl = dgrChungLoaiVT.DataSource as DataTable;
                DataRow drNew = tbl.NewRow();
                drNew["ID"] = "0";
                drNew["STT"] = "0";
                drNew["MaCLVT"] = "";
                drNew["TenCLVT"] = "";
                tbl.Rows.InsertAt(drNew, 0);
            }
        }
        private void SaveChungLoaiVT()
        {
            this.ActiveControl = simpleButton1;
            DataTable tblSource = dgrChungLoaiVT.DataSource as DataTable;
            string json = JsonConvert.SerializeObject(tblSource);
            List<ERPChungLoaiVatTuEntiy> lstSave = JsonConvert.DeserializeObject<List<ERPChungLoaiVatTuEntiy>>(json);
            string url = string.Format("{0}?action={1}&&para={2}&&para2={2}", URL + "KhoVatTu/SaveKhoVT", "SaveCLVT", "Para");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSave); }).Result;
            if (result.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 800);
                LoadDSChungLoai();
            }
            else XtraMessageBox.Show(result);
        }

        private void gridViewChungLoaiVT_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;

            if (view.FocusedColumn.FieldName == "MaVT")
            {
                if (string.IsNullOrEmpty(e.Value?.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Mã vật tư không được để trống!";
                }
                else if (CheckMaCLVT(e.Value?.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Mã vật tư không được trùng lặp!";
                }
            }

        }
        private bool CheckMaCLVT(string maCLVT)
        {
            DataTable tblSource = dgrChungLoaiVT.DataSource as DataTable;
            List<DataRow> lstDR = tblSource.AsEnumerable().Where(x => x["MaCLVT"].ToString() == maCLVT).ToList();
            if (lstDR.Count() > 1)
                return true;
            else return false;
        }

        private void gridViewChungLoaiVT_ShownEditor(object sender, EventArgs e)
        {
            
        }

        private void gridViewChungLoaiVT_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            DataRow drCheck = view.GetFocusedDataRow();
            if (drCheck == null) return;
            if (view.FocusedColumn.FieldName == "MaCLVT")
            {
                string idValue = drCheck["ID"].ToString();
                if (idValue != "0")
                {
                    e.Cancel = true;
                }
            }
        }

        private void frmChungLoaiVT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveChungLoaiVT();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteCLVT();
            }
            else if (e.KeyCode == Keys.F5)
            {
                LoadDSChungLoai();
            }
        }

        private void DeleteCLVT()
        {
            DataRow drFocus = gridViewChungLoaiVT.GetFocusedDataRow();
            if (drFocus == null) return;
            DialogResult op = XtraMessageBox.Show("Bạn có muốn xoá không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (op == DialogResult.Yes)
            {
                string url = string.Format("{0}?action={1}&&ID={2}", URL + "KhoVatTu/DeleteVT", "DeleteCLVT", drFocus["MaCLVT"].ToString());
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 800);
                    LoadDSChungLoai();
                }
            }
        }

        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveChungLoaiVT();
        }
        private void btDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DeleteCLVT();
        }
    }
}
