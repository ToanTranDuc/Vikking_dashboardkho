using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    public partial class frmViTriKho : DevExpress.XtraEditors.XtraForm
    {
        HttpClientExtension _clientExtension = new HttpClientExtension();
        System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        DataTable _tblKho = new DataTable();
        DataTable _tblViTriKho = new DataTable();
        string _maKho = GlobleData.MaKho;
        bool _isAdd = false;

        public frmViTriKho()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            LoadKho();
        }


        private void LoadKho()
        {
            try
            {
                string url = string.Format("{0}/Kho/GetDetail", URL);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblKho = JsonConvert.DeserializeObject<DataTable>(json);
                List<DataRow> lstDR = _tblKho.AsEnumerable().Where(x => x["Status_VT"].ToString() == "0").ToList();
                DevExpress.Skins.Skin skin = DevExpress.Skins.GridSkins.GetSkin(tlKho.LookAndFeel);
                skin.Properties[DevExpress.Skins.GridSkins.OptShowTreeLine] = true;
                tlKho.LookAndFeel.UpdateStyleSettings();
                if (lstDR == null || lstDR.Count == 0) tlKho.DataSource = new DataTable();
                else tlKho.DataSource = lstDR.CopyToDataTable();
                //tlKho.DataSource = _tblKho;
                tlKho.ExpandAll();
            }
            catch (Exception ex) { }
        }

        private void LoadViTriKho(string MaKho)
        {
            try
            {
                string url = string.Format("{0}/ViTriKho/Get?action=Get&&parameter={1}", URL, MaKho);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblViTriKho = JsonConvert.DeserializeObject<DataTable>(json);
                DevExpress.Skins.Skin skin = DevExpress.Skins.GridSkins.GetSkin(tlViTriKho.LookAndFeel);
                skin.Properties[DevExpress.Skins.GridSkins.OptShowTreeLine] = true;
                tlViTriKho.LookAndFeel.UpdateStyleSettings();
                tlViTriKho.DataSource = _tblViTriKho;
                tlViTriKho.ExpandAll();
                if (_tblViTriKho == null || _tblViTriKho.Rows.Count == 0)
                {
                    dgrViTriKho.DataSource = new List<ViTriKhoEntity>();
                    _tblViTriKho = new DataTable();
                }
            }
            catch (Exception ex) { }
        }

        private void AddRow()
        {
            try
            {
                //colParentMaVT.OptionsColumn.AllowEdit = true;
                ViTriKhoEntity item = new ViTriKhoEntity();
                List<ViTriKhoEntity> lstDataSource = new List<ViTriKhoEntity>();
                dgrViTriKho.DataSource = lstDataSource;
                DataRow rowKhoFocus = tlKho.GetFocusedDataRow();
                DataRow rowVTKFocus = tlViTriKho.GetFocusedDataRow();
                if (rowKhoFocus == null) return;
                if (rowVTKFocus == null || tlKho.Focused)
                    item.ParentMaVT = "";
                else
                    item.ParentMaVT = rowVTKFocus["MaVT"].ToString();
                item.MaKho = rowKhoFocus["Id"].ToString();
                item.MaVT = CreateMaVT(item.ParentMaVT);
                item.Dai = 0;
                item.Rong = 0;
                item.Cao = 0;
                item.NguyenLieu = rowVTKFocus == null ? false : (bool)rowVTKFocus["NguyenLieu"];
                item.PhuLieu = rowVTKFocus == null ? false : (bool)rowVTKFocus["PhuLieu"];
                lstDataSource.Add(item);
                dgrViTriKho.RefreshDataSource();
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.ToString()); }
        }

        private void BinddingRow(DataRow dr)
        {
            try
            {
                colParentMaVT.OptionsColumn.AllowEdit = false;
                ViTriKhoEntity item = new ViTriKhoEntity();
                List<ViTriKhoEntity> lstDataSource = new List<ViTriKhoEntity>();
                dgrViTriKho.DataSource = lstDataSource;
                if (dr == null) return;
                item.ID = Convert.ToInt32(dr["ID"].ToString());
                item.MaKho = dr["MaKho"].ToString();
                item.MaVT = dr["MaVT"].ToString();
                item.TenVT = dr["TenVT"].ToString();
                item.ParentMaVT = dr["MaVT"].ToString();
                item.CBM = double.Parse(dr["CBM"].ToString());
                item.Dai = double.Parse(dr["Dai"].ToString());
                item.Rong = double.Parse(dr["Rong"].ToString());
                item.Cao = double.Parse(dr["Cao"].ToString());
                item.NguyenLieu = (bool)dr["NguyenLieu"];
                item.PhuLieu = (bool)dr["PhuLieu"];
                item.GhiChu = dr["GhiChu"].ToString();
                if (item.MaVT.Contains("00")) btnThem.Enabled = true;
                else btnThem.Enabled = false;
                lstDataSource.Add(item);
                dgrViTriKho.RefreshDataSource();
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.ToString()); }
        }

        private void Refresh()
        {
            DataRow drFocus = tlKho.GetFocusedDataRow();
            if (drFocus == null) return;
            TreeListNode nodeFocus = tlViTriKho.FocusedNode;
            if (nodeFocus != null)
            {
                LoadViTriKho(drFocus["Id"].ToString());
                tlViTriKho.FocusedNode = tlViTriKho.FindNodeByID(nodeFocus.Id);
            }

        }

        private string CreateMaVT(string parentMaVT)
        {
            DataRow dr = null;
            if (_tblViTriKho != null)
                dr = _tblViTriKho.AsEnumerable().Where(x => x["ParentMaVT"].ToString() == parentMaVT).OrderByDescending(x => x["MaVT"].ToString()).FirstOrDefault();
            if (dr != null)
            {
                string[] arr = dr["MaVT"].ToString().Trim().Split('.');
                if (dr["MaVT"].ToString().Contains("00.00")) arr[0] = "0" + (Convert.ToInt32(arr[0]) + 1).ToString();
                else if (dr["MaVT"].ToString().Contains("00")) arr[1] = "0" + (Convert.ToInt32(arr[1]) + 1).ToString();
                else if (!dr["MaVT"].ToString().Contains("00")) arr[2] = "0" + (Convert.ToInt32(arr[2]) + 1).ToString();
                string maVT = string.Format("{0}.{1}.{2}", arr[0].Substring(arr[0].Length - 2, 2), arr[1].Substring(arr[1].Length - 2, 2), arr[2].Substring(arr[2].Length - 2, 2));
                return maVT;
            }
            else
            {
                if (parentMaVT == "") return "01.00.00";
                else
                {
                    string[] arr = parentMaVT.Trim().Split('.');
                    if (parentMaVT.Contains("00.00")) arr[1] = "0" + (Convert.ToInt32(arr[1]) + 1).ToString();
                    else if (parentMaVT.Contains("00")) arr[2] = "0" + (Convert.ToInt32(arr[2]) + 1).ToString();
                    string maVT = string.Format("{0}.{1}.{2}", arr[0].Substring(arr[0].Length - 2, 2), arr[1].Substring(arr[1].Length - 2, 2), arr[2].Substring(arr[2].Length - 2, 2));
                    return maVT;
                }
            }
            return "";
        }

        private void tlKho_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            TreeList view = sender as TreeList;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = e.NodeID;
            }
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddRow();
            _isAdd = true;
        }

        private void gridViewViTriKho_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            switch (e.Column.FieldName)
            {
                case "MaKho":
                    DataRow drKho = _tblKho.AsEnumerable().Where(x => x["Id"].ToString() == e.Value.ToString()).FirstOrDefault();
                    if (drKho == null) return;
                    e.DisplayText = drKho["TenKho"].ToString();
                    break;
                case "ParentMaVT":
                    DataRow dr = _tblViTriKho.AsEnumerable().Where(x => x["MaVT"].ToString() == e.Value.ToString()).FirstOrDefault();
                    if (dr == null) return;
                    e.DisplayText = dr["TenVT"].ToString();
                    break;
                default:
                    break;
            }
        }

        private void gridViewViTriKho_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                ViTriKhoEntity item = gridViewViTriKho.GetFocusedRow() as ViTriKhoEntity;
                if (e.Column == colParentMaVT)
                {
                    item.MaVT = CreateMaVT(e.Value.ToString());
                }
                if (e.Column == colDai || e.Column == colRong || e.Column == colCao)
                {
                    item.CBM = item.Dai * item.Rong * item.Cao;
                }
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.ToString()); }
        }

        private void tlViTriKho_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            _isAdd = false;
            DataRow drFocus = tlViTriKho.GetFocusedDataRow();
            BinddingRow(drFocus);
        }

        private void tlKho_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            DataRow drFocus = tlKho.GetFocusedDataRow();
            if (drFocus == null) return;
            LoadViTriKho(drFocus["Id"].ToString());
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = groupBox1;
                List<ViTriKhoEntity> lstSave = dgrViTriKho.DataSource as List<ViTriKhoEntity>;
                if (_isAdd == true)
                {
                    string url = string.Format("{0}/ViTriKho/PostViTriKho?action=Insert&&parameter={0}", URL, "para");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSave); }).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        Refresh();
                    }
                    else XtraMessageBox.Show(result);
                }
                else
                {
                    string url = string.Format("{0}/ViTriKho/PostViTriKho?action=Update&&parameter={0}", URL, "para");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSave); }).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        Refresh();
                    }
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.ToString()); }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Refresh();
        }

        private void tlViTriKho_CustomDrawRow(object sender, CustomDrawRowEventArgs e)
        {
            string value = e.Info.Cells[0].Value.ToString();
            if (value.Contains("00.00"))
                e.Appearance.BackColor = Color.FromArgb(255, 165, 165);
            else if (value.Contains("00"))
                e.Appearance.BackColor = Color.FromArgb(207, 249, 255);
            //else e.Appearance.BackColor = Color.FromArgb(207, 255, 218);
        }

        private void btnQrCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string json = JsonConvert.SerializeObject(_tblViTriKho);
            List<ViTriKhoEntity> lst = JsonConvert.DeserializeObject<List<ViTriKhoEntity>>(json);
            frmQRCodeViewer frm = new frmQRCodeViewer(lst);
            frm.ShowDialog();
        }

        private void repoCheckPL_CheckedChanged(object sender, EventArgs e)
        {
            ViTriKhoEntity rowFocus = gridViewViTriKho.GetFocusedRow() as ViTriKhoEntity;
            if (rowFocus == null) return;
            rowFocus.NguyenLieu = false;
            dgrViTriKho.RefreshDataSource();
        }

        private void repoCheckNL_CheckedChanged(object sender, EventArgs e)
        {
            ViTriKhoEntity rowFocus = gridViewViTriKho.GetFocusedRow() as ViTriKhoEntity;
            if (rowFocus == null) return;
            rowFocus.PhuLieu = false;
            dgrViTriKho.RefreshDataSource();
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drFocus = tlViTriKho.GetFocusedDataRow();
            if (drFocus == null) return;
            if (XtraMessageBox.Show("Khi xóa vị trí phần mềm sẽ xóa các dữ liệu liên quan đến vị trí này. Bạn có chắc muốn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string url = string.Format("{0}/ViTriKho/DeleteViTriKho?maViTri={1}", URL, drFocus["MaVT"]);
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    Refresh();
                }
                else XtraMessageBox.Show(result);
            }
        }

        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Workbook|*.xlsx|Excel Workbook 97-2003|*.xls";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                if (fileName == "") return;
                DataTable tblSaveVTK = ReadExcel(fileName);
                if (tblSaveVTK == null || tblSaveVTK.Rows.Count == 0) return;
                CheckDataExcel(tblSaveVTK);
                string json = JsonConvert.SerializeObject(tblSaveVTK);
                List<ViTriKhoEntity> lstSave = JsonConvert.DeserializeObject<List<ViTriKhoEntity>>(json);
                string url = string.Format("{0}/ViTriKho/PostViTriKho?action=Insert&&parameter={0}", URL, "para");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstSave); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    Refresh();
                }
                else XtraMessageBox.Show(result);

            }
        }

        public DataTable ReadExcel(string fileName)
        {
            string conString = string.Empty;
            DataTable dtexcel = new DataTable();
            string extension = System.IO.Path.GetExtension(fileName);
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString, fileName);
                    break;
                case ".xlsx": //Excel 07 or higher
                    conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, fileName);
                    break;
            }
            using (OleDbConnection conn = new OleDbConnection(conString))
            {
                try
                {
                    OleDbDataAdapter oleAdpt =
                        new OleDbDataAdapter("select 0 as ID, RTRIM(MaKho) as MaKho, RTRIM(MaVT) as MaVT, RTRIM(TenVT) as TenVT, " +
                        "RTRIM(ParentMaVT) as ParentMaVT, CBM, Dai, Rong, Cao, NguyenLieu, PhuLieu, GhiChu from [Sheet1$]", conn); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                conn.Dispose();
                conn.Close();
            }

            return dtexcel;
        }

        private void CheckDataExcel(DataTable tblSaveLK)
        {
            bool check = true;
            List<DataRow> lstDataEmpty = tblSaveLK.AsEnumerable().Where(x => x["MaVT"].ToString() == "").ToList();
            foreach (DataRow item in lstDataEmpty)
            {
                tblSaveLK.Rows.Remove(item);
            }
        }
    }
}
