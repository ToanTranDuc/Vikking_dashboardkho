using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ResourceForm;
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
    public partial class frmCongDoanIE : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();      
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _styleID, _maHang, _status, _rap;
        int _version = 1;
        public frmCongDoanIE(string StyleID, string Status, string MaHang, int Version, string Rap)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _styleID = StyleID;
            _status = Status;
            _maHang = MaHang;
            _version = Version;
            _rap = Rap;
            gridViewCongDoanIE.PopupMenuShowing += GridViewCongDoanIE_PopupMenuShowing;
            gridViewCongDoanIE.CellValueChanged += GridViewCongDoanIE_CellValueChanged;
            LoadDsCongDoan();
        }

        private void GridViewCongDoanIE_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow drNewRow = gridViewCongDoanIE.GetFocusedDataRow();
            DataTable tbl = dgrCongDoanIE.DataSource as DataTable;
            int count = tbl.Rows.Count;
            if (drNewRow["StyleID"] == null || drNewRow["StyleID"].ToString() == "")
            {
                drNewRow["ID"] = 0;
                drNewRow["STT"] = count + 1;
                drNewRow["SapXep"] = count + 1;
                drNewRow["StyleID"] = _styleID;
                drNewRow["MaHang"] = _maHang;
                tbl.Rows.Add(drNewRow);
            }
        }

        private void GridViewCongDoanIE_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();
            DXMenuItem xoa = new DXMenuItem();
            xoa.Caption = "Xóa";
            xoa.Click += Xoa_Click;
            e.Menu.Items.Add(xoa);
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = btnControl;
            DataTable tbl = dgrCongDoanIE.DataSource as DataTable;
            DataTable tblSave = CreateTblSave();
            foreach (DataRow dr in tbl.Rows)
            {
                if (dr["Name"].ToString().Trim() == "") continue;
                DataRow itemAdd = tblSave.NewRow();
                itemAdd["ID"] = dr["ID"];
                itemAdd["StyleID"] = dr["StyleID"];
                itemAdd["NO"] = dr["NO"];
                itemAdd["SapXep"] = dr["SapXep"];
                itemAdd["Name"] = dr["Name"].ToString().Trim();
                itemAdd["Status"] = _status;
                itemAdd["Version"] = _version;
                itemAdd["Rap"] = _rap;
                tblSave.Rows.Add(itemAdd);
            }
            if (tblSave.Rows.Count == 0) return;
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/PostCongDoanIE?action=SaveCongDoan");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
            }
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDsCongDoan();
        }

        private void Xoa_Click(object sender, EventArgs e)
        {
            DataTable tbl = dgrCongDoanIE.DataSource as DataTable;
            DataTable tblSave = CreateTblSave();
            DataRow drFocus = gridViewCongDoanIE.GetFocusedDataRow();
            if (drFocus == null) return;           
            if (XtraMessageBox.Show("Xóa công đoạn sẽ xóa tất cả thông số có công đoạn này. Bạn có muốn tiếp tục?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            var drDelete = tblSave.NewRow();
            drDelete["ID"] = drFocus["ID"];
            tblSave.Rows.Add(drDelete);
            string url = string.Format("{0}", URL + "QTY_MaHangThongSoDo/PostCongDoanIE?action=DeleteV2");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(frmExcSuccess), true, true, false, true);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1500, this);
            }

        }

        private void LoadDsCongDoan()
        {
            string url = string.Format("{0}", URL + $"QTY_MaHangThongSoDo/GetCongDoanIE?action=GetDsCongDoan&para={_styleID}&para1={_status}&para2={_version}&para3={_rap}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var tbl = JsonConvert.DeserializeObject<DataTable>(json);
            dgrCongDoanIE.DataSource = tbl.Rows.Count == 0 ? CreateTblSave() : tbl;
        }
        private DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("StyleID", typeof(string));
            tbl.Columns.Add("InSeam", typeof(string));
            tbl.Columns.Add("NO", typeof(string));
            tbl.Columns.Add("Code", typeof(string));
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("SapXep", typeof(int));
            tbl.Columns.Add("Status", typeof(int));
            tbl.Columns.Add("Version", typeof(int));
            tbl.Columns.Add("Rap", typeof(int));
            return tbl;
        }
    }
}
