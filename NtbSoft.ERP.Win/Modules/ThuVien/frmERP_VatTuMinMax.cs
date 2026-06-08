using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_VatTuMinMax : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataTable chungloaiTable;
        private DataTable vattuchitietTable;
        private DataTable gridControl1Table;
        private DataTable gridControl1TableCopy;

        private int isCL;
        private string maVTID;
        private string maVT;

        public frmERP_VatTuMinMax(int theochungloai = 1, string mavtid = "", string mavt = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            this.isCL = theochungloai;
            this.maVTID = mavtid;
            this.maVT = mavt;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (isCL == 1)
            {
                gridView1.Columns["MaVT"].Visible = false;
                gridView1.Columns["ChiTiet"].Visible = false;
                gridView1.Columns["MaMauVT"].Visible = false;
                gridView1.Columns["MauVT"].Visible = false;
                gridView1.Columns["KhoVai"].Visible = false;
                getChungLoai();
                calcChungLoai();                
            }
            else
            {
                getVatTuChiTiet();
                calcVatTuChiTiet();
            }
            loadGridControl1();

            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
        }
        public DataTable ResultTable { get; private set; }

        private void loadGridControl1()
        {

            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView1.Columns["TonToiThieu"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["TonToiDa"].OptionsColumn.AllowEdit = true;
            gridView1.ExpandAllGroups();
        }
        private void getChungLoai()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getchungloai";
            string urlGetListDataTable = URL + "VatTuMinMax/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            chungloaiTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!chungloaiTable.Columns.Contains("generatedID"))
                chungloaiTable.Columns.Add("generatedID");
            if (!chungloaiTable.Columns.Contains("IsNPLText"))
                chungloaiTable.Columns.Add("IsNPLText", typeof(string));
            foreach (DataRow row in chungloaiTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";
            }
        }
        private void calcChungLoai()
        {
            DataTable dt = chungloaiTable.Copy();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            foreach (DataRow row in dt.Rows)
            {
                row["TonToiThieu"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiThieu"].ToString());
                row["TonToiDa"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiDa"].ToString());
            }
            gridControl1Table = dt.Copy();
        }
        private void getVatTuChiTiet()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getvattuchitiet";
            request.Parameter = maVTID;
            request.Parameter1 = maVT;
            string urlGetListDataTable = URL + "VatTuMinMax/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            vattuchitietTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!vattuchitietTable.Columns.Contains("generatedID"))
                vattuchitietTable.Columns.Add("generatedID");
            if (!vattuchitietTable.Columns.Contains("IsNPLText"))
                vattuchitietTable.Columns.Add("IsNPLText", typeof(string));
            foreach (DataRow row in vattuchitietTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";
            }
        }
        private void calcVatTuChiTiet()
        {
            DataTable dt = vattuchitietTable.Copy();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            foreach (DataRow row in dt.Rows)
            {
                row["TonToiThieu"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiThieu"].ToString());
                row["TonToiDa"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiDa"].ToString());
            }
            gridControl1Table = dt.Copy();
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            // Vẽ border quanh cột header
            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            // Vẽ text trong vùng caption
            TextRenderer.DrawText(e.Graphics, e.Info.Caption, e.Appearance.Font,
                                  e.Info.Bounds, Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            e.Handled = true;
        }
        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;

            // Chỉ hiện menu khi chuột phải trên cell của cột TyLeMuaThem
            if (e.HitInfo.InRowCell && e.HitInfo.Column != null && (e.HitInfo.Column.FieldName == "TonToiThieu" || e.HitInfo.Column.FieldName == "TonToiDa"))
            {
                e.Menu.Items.Clear(); // bỏ menu mặc định nếu muốn

                // Fill tất cả
                var fillAllItem = new DXMenuItem("Áp dụng tất cả", (o, args) =>
                {
                    int rowHandle = e.HitInfo.RowHandle;
                    var col = e.HitInfo.Column;
                    object value = view.GetRowCellValue(rowHandle, col);

                    view.BeginUpdate();
                    try
                    {
                        for (int i = 0; i < view.DataRowCount; i++)
                        {
                            int rh = view.GetRowHandle(i);
                            view.SetRowCellValue(rh, col, value);
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                // Fill group
                //var fillGroupItem = new DXMenuItem("Áp dụng theo nhóm", (o, args) =>
                //{
                //    int rowHandle = e.HitInfo.RowHandle;
                //    var col = e.HitInfo.Column;
                //    object value = view.GetRowCellValue(rowHandle, col);

                //    // Xác định group row của row hiện tại
                //    int groupRowHandle = view.GetParentRowHandle(rowHandle);

                //    view.BeginUpdate();
                //    try
                //    {
                //        // Duyệt tất cả row trong group đó
                //        for (int i = 0; i < view.GetChildRowCount(groupRowHandle); i++)
                //        {
                //            int childHandle = view.GetChildRowHandle(groupRowHandle, i);
                //            if (view.IsDataRow(childHandle))
                //                view.SetRowCellValue(childHandle, col, value);
                //        }
                //    }
                //    finally { view.EndUpdate(); }
                //});

                e.Menu.Items.Add(fillAllItem);
                //e.Menu.Items.Add(fillGroupItem);
            }
        }


        private void xacnhanBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            if(!checkMax())
            {
                XtraMessageBox.Show("Số tồn tối đa không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isCL == 1) saveMinMaxChungLoai();
            else saveMinMaxVatTu();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void saveMinMaxChungLoai()
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("VatTuMinMax");
            request.Action = "saveminmaxchungloai";
            foreach (DataRow row in dt.Rows)
            {
                decimal tonTT = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiThieu"].ToString());
                decimal tonTD = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiDa"].ToString());

                if (tonTT > 0 || tonTD > 0)
                {
                    DataRow newRow = request.TypeTable.NewRow();
                    newRow["Nguon"] = "chungloai";
                    newRow["MaNhomVT"] = row["MaCLVT"];
                    newRow["TonToiThieu"] = tonTT;
                    newRow["TonToiDa"] = tonTD;
                    newRow["NguoiTao"] = GlobleData.UserName;
                    newRow["NgayTao"] = DateTime.Now;
                    request.TypeTable.Rows.Add(newRow);
                }
            }
            string urlGetListDataTable = URL + "VatTuMinMax/Post";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private void saveMinMaxVatTu()
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("VatTuMinMax");
            request.Action = "saveminmaxvattu";
            foreach (DataRow row in dt.Rows)
            {
                decimal tonTT = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiThieu"].ToString());
                decimal tonTD = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiDa"].ToString());

                if (tonTT > 0 || tonTD > 0)
                {
                    DataRow newRow = request.TypeTable.NewRow();
                    newRow["Nguon"] = "vattu";
                    newRow["MaNhomVT"] = row["MaCLVT"];
                    newRow["MaVTID"] = row["MaVTID"];
                    newRow["MaMauVT"] = row["MauVTID"];
                    newRow["MaKhoVT"] = row["KhoVaiID"];
                    newRow["TonToiThieu"] = tonTT;
                    newRow["TonToiDa"] = tonTD;
                    newRow["NguoiTao"] = GlobleData.UserName;
                    newRow["NgayTao"] = DateTime.Now;
                    request.TypeTable.Rows.Add(newRow);
                }
            }
            string urlGetListDataTable = URL + "VatTuMinMax/Post";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private bool checkMax()
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return true;
            foreach (DataRow row in dt.Rows)
            {
                decimal tonTT = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiThieu"].ToString());
                decimal tonTD = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiDa"].ToString());
                if (tonTD > 0 && tonTD < tonTT) return false;
            }
            
            return true;
        }
        
    }
}