using DevExpress.XtraEditors;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
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
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NtbSoft.ERP.Entity.ThuVien;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmNguyenLieu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        string URL = string.Empty;
        private ResourceURL.EventStatus _status;
        List<NguyenLieuEntity> listNguyenlieuEntity;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        DataTable tbNL;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        public frmNguyenLieu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            listNguyenlieuEntity = new List<NguyenLieuEntity>();
            tbNL = new DataTable();
        }

        private void frmNguyenLieu_Load(object sender, EventArgs e)
        {

        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }
        protected override void OnLoad(EventArgs e)
        {

            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSNguyenLieu(false);
            
           
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);//SystemUser/GetPer/userID=...
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
            Them.Enabled = false;
            Sua.Enabled = false;
            Luu.Enabled = true;
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gvNguyenLieu.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    gvNguyenLieu.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    gvNguyenLieu.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        private void LoadDSNguyenLieu(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "KhoNPL/GetNL");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if(json=="[]")
                {
                    listNguyenlieuEntity = new List<NguyenLieuEntity>();
                    gridNguyenLieu.DataSource = listNguyenlieuEntity;
                    Xoa.Enabled = false;
                    Sua.Enabled = false;
                    btnXuatExcel.Enabled = false;

                    txtID.Text = "0";

                    txtMaVatTu.Text = "";
                    txtTenVatTu.Text = "";
                    txtNhaCungCap.Text = "";
                    txtMaHaiQuan.Text = "";
                    txtItemCode.Text = "";

                    txtKhoVai.Text = "";
                    txtCodeMau.Text = "";
                    txtTenCodeMau.Text = "";
                    txtNhomNPL.Text = "";
                    txtLot.Text = "";
                    txtAnhMau.Text = "";

                    txtMaKeToan.Text = "";
                    txtDonViTinh.Text = "";
                    return;
                }    
                if (!string.IsNullOrEmpty(json))
                {
                    tbNL = JsonConvert.DeserializeObject<DataTable>(json);
                    listNguyenlieuEntity = JsonConvert.DeserializeObject<List<NguyenLieuEntity>>(json);
                    if(listNguyenlieuEntity.Count==0)
                    { return; }    
                    loadThongTinNguyenLieu(listNguyenlieuEntity[0].ID);
                    Xoa.Enabled = true;
                    Sua.Enabled = true;
                    btnXuatExcel.Enabled = true;
                }
                
                gridNguyenLieu.DataSource = listNguyenlieuEntity;

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gvNguyenLieu.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridNguyenLieu;
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadThongTinNguyenLieu(int ID)
        {
          
            NguyenLieuEntity result = listNguyenlieuEntity.First(nl => nl.ID == ID);
            
            txtID.Text = ID.ToString();
           
            txtMaVatTu.Text = result.MaVatTu;
            txtTenVatTu.Text = result.TenVatTu;
            txtNhaCungCap.Text = result.NhaCungCap;
            txtMaHaiQuan.Text = result.MaHaiQuan;
            txtItemCode.Text = result.ItemCode;
            
            txtKhoVai.Text = result.KhoVai;
            txtCodeMau.Text = result.CodeMau;
            txtTenCodeMau.Text = result.TenCodeMau;
          
            txtLot.Text = result.Lot;
            txtAnhMau.Text = result.AnhMau;
       
            txtNhomNPL.Text = "Mã nhóm: " + result.NhomNPL + ". Tên nhóm: " + result.TenNhom + ".";
          
            txtDonViTinh.Text = result.DonViTinh;
            txtMaKeToan.Text = result.MaKeToan;
            txtMaVatTu.Properties.ReadOnly = true;
            txtTenVatTu.Properties.ReadOnly = true;
            txtNhaCungCap.Properties.ReadOnly = true;
            txtMaHaiQuan.Properties.ReadOnly = true;
            txtItemCode.Properties.ReadOnly = true;
           
            txtKhoVai.Properties.ReadOnly = true;
            txtCodeMau.Properties.ReadOnly = true;
            txtTenCodeMau.Properties.ReadOnly = true;
          
            txtLot.Properties.ReadOnly = true;
            txtAnhMau.Properties.ReadOnly = true;
            txtNhomNPL.Properties.ReadOnly = true;
           
          
            txtDonViTinh.Properties.ReadOnly = true;
            txtMaKeToan.Properties.ReadOnly = true;

        }
        private void ThemDong()
        {
            DateTime ngayHomNay = DateTime.Now;

            // Chuyển đổi DateTime sang chuỗi với định dạng yyyy-MM-dd
            string ngayHomNayString = ngayHomNay.ToString("yyyy-MM-dd");

            txtID.Text = "0";
           
            txtMaVatTu.Text = "";
            txtTenVatTu.Text = "";
            txtNhaCungCap.Text = "";
            txtMaHaiQuan.Text = "";
            txtItemCode.Text = "";
           
            txtKhoVai.Text = "";
            txtCodeMau.Text = "";
            txtTenCodeMau.Text = "";
            txtNhomNPL.Text = "";
            txtLot.Text = "";
            txtAnhMau.Text = "";

            txtMaKeToan.Text = "";
            txtDonViTinh.Text = "";
          
           
            txtMaVatTu.Properties.ReadOnly = false;
            txtTenVatTu.Properties.ReadOnly = false;
            txtNhaCungCap.Properties.ReadOnly = false;
            txtMaHaiQuan.Properties.ReadOnly = false;
            txtItemCode.Properties.ReadOnly = false;
           
            txtKhoVai.Properties.ReadOnly = false;
            txtCodeMau.Properties.ReadOnly = false;
            txtTenCodeMau.Properties.ReadOnly = false;
            txtLot.Properties.ReadOnly = false;
            txtAnhMau.Properties.ReadOnly = false;
            txtNhomNPL.Properties.ReadOnly = false;
           
           
            txtDonViTinh.Properties.ReadOnly = false;
            txtMaKeToan.Properties.ReadOnly = false;
            LoadNhomNPL();

            LoadDVCL();
        }
        private void NapLaiDong()
        {
            /* LoadDSKhachHang(false);
             _status = ResourceURL.EventStatus.View;
             GridViewUpdateStatus(_status);
             gridViewKhachHang.OptionsView.NewItemRowPosition = NewItemRowPosition.None;*/
            LoadDSNguyenLieu(false);
        }
        private void SuaDong()
        {
            NguyenLieuEntity row = gvNguyenLieu.GetRow(gvNguyenLieu.FocusedRowHandle) as NguyenLieuEntity;
            /* _status = ResourceURL.EventStatus.Edit;
             GridViewUpdateStatus(_status);
             focused(this.gridViewKhachHang);*/
           
            txtMaVatTu.Properties.ReadOnly = false;
            txtTenVatTu.Properties.ReadOnly = false;
            txtNhaCungCap.Properties.ReadOnly = false;
            txtMaHaiQuan.Properties.ReadOnly = false;
            txtItemCode.Properties.ReadOnly = false;
           
            txtKhoVai.Properties.ReadOnly = false;
            txtCodeMau.Properties.ReadOnly = false;
            txtTenCodeMau.Properties.ReadOnly = false;
           
            txtLot.Properties.ReadOnly = false;
            txtAnhMau.Properties.ReadOnly = false;
            txtNhomNPL.Properties.ReadOnly = false;
          
           
            txtDonViTinh.Properties.ReadOnly = false;
            txtMaKeToan.Properties.ReadOnly = false;
            Luu.Enabled = true;
            LoadNhomNPL(row.NhomNPL.ToString());
            LoadDVCL(row.DonViTinh.ToString());
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

      
        private void gvNguyenLieu_Click(object sender, EventArgs e)
        {
            Them.Enabled = true;
            Sua.Enabled = true;
            Luu.Enabled = false;
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {
              
                int rowHandle = view.FocusedRowHandle;

          
                if (rowHandle >= 0)
                {
                 
                    int idValue = int.Parse(view.GetRowCellValue(rowHandle, "ID").ToString());
                    loadThongTinNguyenLieu(idValue);
                 
                }
            }
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    NguyenLieuEntity row = gvNguyenLieu.GetRow(gvNguyenLieu.FocusedRowHandle) as NguyenLieuEntity;
                    string url = string.Format("{0}?Parameter={1}", URL + "KhoNPL/DeleteNL", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                    {
                       
                        LoadDSNguyenLieu(false);
                        if (gvNguyenLieu.RowCount == 0)
                        {
                            txtID.Text = "0";

                            txtMaVatTu.Text = "";
                            txtTenVatTu.Text = "";
                            txtNhaCungCap.Text = "";
                            txtMaHaiQuan.Text = "";
                            txtItemCode.Text = "";
                            
                            txtKhoVai.Text = "";
                            txtCodeMau.Text = "";
                            txtTenCodeMau.Text = "";
                            txtNhomNPL.Text = "";
                            txtLot.Text = "";
                            txtAnhMau.Text = "";


                            txtDonViTinh.Text = "";
                        }
                    }
                    else XtraMessageBox.Show(result);
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        
        private void btnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportExcel();
        }
        private void ImportExcel()
        {

            try
            {
                string url = string.Format("{0}?", URL + "KhoNPL/GetNL");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    listNguyenlieuEntity = JsonConvert.DeserializeObject<List<NguyenLieuEntity>>(json);
                }
                HashSet<string> existingMaVatTuSet = new HashSet<string>(listNguyenlieuEntity.Select(nl => nl.MaVatTu));
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";
                openFileDialog1.Title = "Import Excel";
                openFileDialog1.Multiselect = false;
                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult != DialogResult.OK) return;
                string pathExecel = openFileDialog1.FileName;
                if (string.IsNullOrEmpty(pathExecel)) return;
                string conString = "";
                string extension = System.IO.Path.GetExtension(pathExecel);
                switch (extension)
                {
                    case ".xls": //Excel 97-03
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString, pathExecel);
                        //conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                    case ".xlsx": //Excel 07 or higher
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                }
                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    if (excel_con.State == ConnectionState.Closed)
                    {
                        excel_con.Open();
                    }
                    //OleDbCommand _oleCmdSelect;
                    OleDbDataAdapter oleAdapter = new OleDbDataAdapter(); ;
                    DataTable sheets = GetSchemaTable(conString);


                    //OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                    //                             @"SELECT * FROM [Sheet1$] ", excel_con);
                    OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                                              @"SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString() + "] ", excel_con);
                    DataSet excelDataSet = new DataSet();
                    _oleCmdSelect.Fill(excelDataSet);
                    DataTable dt = new DataTable();
                    dt = excelDataSet.Tables[0];
                    DataTable dtSave = CreateTblSave();
                    for (int i = 1; i < dt.Rows.Count; i++) 
                    {
                        var drnew = dtSave.NewRow();
                        drnew["ID"] = 0; 
                        drnew["MaDH"] = dt.Rows[i][1].ToString();
                       
                        drnew["MaHaiQuan"] = dt.Rows[i][2].ToString();
                       
                        drnew["MaVatTu"] = dt.Rows[i][3].ToString();
                        //kiểm tra xem mã vật tư có trùng k
                        string mvt = dt.Rows[i][3].ToString();
                        /*if (existingMaVatTuSet.Contains(mvt))
                        {
                            XtraMessageBox.Show("Trùng lặp mã vật tư: " + mvt, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Dừng import
                        }*/
                        drnew["TenVatTu"] = dt.Rows[i][4].ToString();
                        drnew["NhaCungCap"] = dt.Rows[i][5].ToString();
                        drnew["ItemCode"] = dt.Rows[i][6].ToString();
                        drnew["CodeMau"] = dt.Rows[i][7].ToString();
                        drnew["TenCodeMau"] = dt.Rows[i][8].ToString();
                        drnew["KhoVai"] = dt.Rows[i][9].ToString();
                        drnew["TongKien"] = dt.Rows[i][10].ToString();
                        drnew["Kien"] = dt.Rows[i][11].ToString();
                        drnew["Lot"] = dt.Rows[i][12].ToString();
                        drnew["AnhMau"] = dt.Rows[i][13].ToString();
                        drnew["NhomNPL"] = dt.Rows[i][14].ToString();
                        drnew["TenNhom"] ="";
                        drnew["SoLuong"] = dt.Rows[i][15] == DBNull.Value ? 0.0 : Convert.ToDouble(dt.Rows[i][15]);

                        drnew["DonViTinh"] = dt.Rows[i][16].ToString();
                        string rawValue = dt.Rows[i][17].ToString();
                        string format = "dddd, MMMM dd, yyyy";
                        rawValue = rawValue.Trim();
                        // Chuyển đổi chuỗi ngày tháng sang DateTime
                        DateTime loNhapDate = DateTime.ParseExact(rawValue, format, CultureInfo.InvariantCulture);

                        // Gán giá trị đã định dạng (yyyy-MM-dd) vào drnew
                        drnew["LoNhap"] = loNhapDate.ToString("yyyy-MM-dd");
                        drnew["BarCode"] = dt.Rows[i][18].ToString();
                       
                  
                        dtSave.Rows.Add(drnew);
                    }

                    SaveData(dtSave);
                }
                XtraMessageBox.Show("Đã nhập xong dữ liệu từ Excel" , "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDSNguyenLieu(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                 XtraMessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu nhóm NPL. Vui lòng kiểm tra lại!"+ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            }

        }
        private void SaveData(DataTable dtSave)
        {

            string url = string.Format("{0}?", URL + "KhoNPL/PostNL");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
        }


        private DataTable GetSchemaTable(string connectionString)
        {
            using (OleDbConnection connection = new
                       OleDbConnection(connectionString))
            {
                connection.Open();
                DataTable schemaTable = connection.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                return schemaTable;
            }
        }
        private DataTable CreateTblSave()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("MaHaiQuan", typeof(string));
            dt.Columns.Add("MaVatTu", typeof(string));
            dt.Columns.Add("TenVatTu", typeof(string));
            dt.Columns.Add("NhaCungCap", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("CodeMau", typeof(string));
            dt.Columns.Add("TenCodeMau", typeof(string));
            dt.Columns.Add("KhoVai", typeof(string));
            dt.Columns.Add("TongKien", typeof(string));
            dt.Columns.Add("Kien", typeof(string));
            dt.Columns.Add("Lot", typeof(string));
            dt.Columns.Add("AnhMau", typeof(string));
            dt.Columns.Add("NhomNPL", typeof(string));
            dt.Columns.Add("TenNhom", typeof(string));
            dt.Columns.Add("SoLuong", typeof(double));
            dt.Columns.Add("DonViTinh", typeof(string));
            dt.Columns.Add("LoNhap", typeof(string));
            dt.Columns.Add("BarCode", typeof(string));
            return dt;
        }
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }

        private void gvNguyenLieu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (tbNL is null || tbNL.Rows.Count == 0) return;
            DataTable dtTbl = tbNL;
            if (!dtTbl.Columns.Contains("STT"))
            {
                dtTbl.Columns.Add("STT", typeof(int));
            }
            for (int i = 0; i < dtTbl.Rows.Count; i++)
            {
                dtTbl.Rows[i]["STT"] = i + 1;
            }
            dtTbl.Columns["STT"].SetOrdinal(0);
           
            dtTbl.Columns["MaHaiQuan"].SetOrdinal(2);
            dtTbl.Columns["LoNhap"].SetOrdinal(dtTbl.Rows.Count - 1);
            dtTbl.Columns.Remove("ID");
            dtTbl.Columns.Remove("MaNguyenLieu");
            dtTbl.Columns.Remove("TenNhom");
            //tenfileexcel
            Sfd.FileName = string.Format("TongHopNguyenLieu");
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Nguyên liệu");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG; 
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("NguyenLieu");
                    ExportExcel(Sfd.FileName, dtTbl);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                }
            }
        }
        private void ExportExcel(string path, DataTable dtNguyenLieu)
        {
            try
            {
                string PathLoGo = KHDongThungLibP.getImgPath("texgiang.jpg");
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("NguyenLieu");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "CÔNG TY TNHH PHÁT TRIỂN CÔNG NGHỆ NAM THANH BÌNH"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "NGUYÊN LIỆU"; range.Style.Font.Bold = true;
                    int colum = 6;
                    range = worksheet.Cells["A1:B3"]; range.Merge = true;
                    Image image = Image.FromFile(PathLoGo);
                    OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
                    picture.SetPosition(0, 0, 0, 0);
                    picture.SetSize(105, 55);
                    worksheet.Cells.Style.Font.Size = 11;
                    for (int i = 0; i < dtNguyenLieu.Columns.Count-1; i++)
                    {
                        
                        worksheet.Cells[4, i + 1].Value = dtNguyenLieu.Columns[i].ColumnName;
                        worksheet.Cells[4, i + 1].Style.Font.Bold = true; // Đặt tiêu đề in đậm
                    }

                    // Thêm dữ liệu vào worksheet
                    for (int i = 0; i < dtNguyenLieu.Rows.Count; i++)
                    {
                       
                        for (int j = 0; j < dtNguyenLieu.Columns.Count-1; j++)
                        {
                            worksheet.Cells[i + 5, j + 1].Value = dtNguyenLieu.Rows[i][j];
                        }
                    }

                    // Tự động điều chỉnh độ rộng cột
                    worksheet.Cells.AutoFitColumns();
                    //dtXuatEX(dtPKLXuatHang, worksheet);
                   
                    excelPackage.SaveAs(file);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }
        public static void dtXuatEX(DataTable dtPKLXuatHang, ExcelWorksheet worksheet, bool flagFilter = true)
        {
           /* ExcelRange range = worksheet.Cells;
            List<string> columnNamesWithSize = dtPKLXuatHang.Columns.Cast<DataColumn>()
                        .Where(column => column.ColumnName.Contains("@"))
                        .Select(column => column.ColumnName)
                        .Distinct()
                        .ToList();
            int colum = 7;
            range = worksheet.Cells["A9:b9"]; range.Merge = true; range.Value = "Carton Number";
            range = worksheet.Cells["c9"]; range.Merge = true; range.Value = "PO";
            range = worksheet.Cells["d9"]; range.Merge = true; range.Value = "Supplier";
            range = worksheet.Cells["e9"]; range.Merge = true; range.Value = "Size/Inseam";
            range = worksheet.Cells["f9"]; range.Merge = true; range.Value = "Color";
            foreach (var colums in columnNamesWithSize)
            {
                string splitColum = colums.Split('@')[0];
                worksheet.Cells[9, colum].Value = splitColum.ToString();
                worksheet.Cells[9, colum].Style.Font.Bold = true;
                worksheet.Column(colum).AutoFit();
                colum++;
            }
            if (!flagFilter)
                KHDongThungLibP.ProcessSttTrung1(dtPKLXuatHang);

            var uniqueValues = dtPKLXuatHang.AsEnumerable().
            Select(x => new
            {
                MaPKL = x["MaPKLDisplay"],
                SttThung = x["SttThung"],
                SLThung = x["SLThung"],
                TotalPiece = x["TotalPiece"],
                SoLuong = x["SoLuong"],
                TrongLuong = x["TrongLuong"],
                KhoiLuong = x["KhoiLuong"],
            }).Distinct().ToList();

            int totalSLThung = uniqueValues.Sum(x => Convert.ToInt32(x.SLThung));
            int totalTotalPiece = uniqueValues.Sum(x => Convert.ToInt32(x.TotalPiece));
            int totalSoLuong = uniqueValues.Sum(x => Convert.ToInt32(x.SoLuong));
            float totalTrongLuong = uniqueValues.Sum(x => Convert.ToSingle(x.TrongLuong));
            float roundedTotalTrongLuong = (float)Math.Round(totalTrongLuong, 1);
            float totalKhoiLuong = uniqueValues.Sum(x => Convert.ToSingle(x.KhoiLuong));
            float roundedTotalKhoiLuong = (float)Math.Round(totalKhoiLuong, 1);
            List<string> sumToTalAll = new List<string>() { totalSoLuong.ToString(), totalSLThung.ToString(), totalTotalPiece.ToString(), roundedTotalKhoiLuong.ToString(), roundedTotalTrongLuong.ToString(), };
            int totalPCB_Pack = dtPKLXuatHang.AsEnumerable().Sum(x => Convert.ToInt32(x["PCB_Pack"]));
            if (totalPCB_Pack != 0)
            {
                range = worksheet.Cells[9, colum]; range.Value = "PCB_Pack "; worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[9, colum]; range.Value = "Pack_Ctn "; worksheet.Column(colum).AutoFit(); colum++;
            }
            range = worksheet.Cells[9, colum]; range.Value = "QTY "; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "Carton "; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "ToTal Pieces"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "NW"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "GW"; worksheet.Column(colum).AutoFit(); colum++;
            range = worksheet.Cells[9, colum]; range.Value = "Carton"; worksheet.Column(colum).AutoFit(); colum++;
            _columns = colum;
            int row = 10;
            int index = 0;
            int rowMerge = 10;
            int indexMerge = 0;
            bool checkCountMerge = false;
            foreach (DataRow rows in dtPKLXuatHang.Rows)
            {
                colum = 7;
                worksheet.Cells[row, 1].Value = Convert.ToInt32(rows["TuThung"]);
                worksheet.Cells[row, 2].Value = Convert.ToInt32(rows["DenThung"]);
                worksheet.Cells[row, 3].Value = rows["PO"].ToString();
                worksheet.Cells[row, 4].Value = rows["TenDVSX"].ToString();
                worksheet.Cells[row, 5].Value = rows["DauSize"].ToString();
                worksheet.Cells[row, 6].Value = rows["TenMau"].ToString();
                worksheet.Column(3).AutoFit(); worksheet.Column(4).AutoFit(); worksheet.Column(5).AutoFit(); worksheet.Column(6).AutoFit();
                foreach (var colums in columnNamesWithSize)
                {
                    if (rows[colums.ToString()].ToString() == "")
                    {
                        colum++;
                        continue;
                    }
                    int cellValue = Convert.ToInt32(rows[colums.ToString()]);
                    worksheet.Cells[row, colum].Value = cellValue == 0 ? (object)" " : cellValue;
                    worksheet.Column(colum).AutoFit();
                    colum++;
                }
                double khoiLuong = Convert.ToDouble(rows["KhoiLuong"]);
                double roundedKhoiLuong = Math.Round(khoiLuong, 1);
                double trongLuong = Convert.ToDouble(rows["TrongLuong"]);
                if (totalPCB_Pack != 0)
                {
                    range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["PCB_Pack"]); worksheet.Column(colum).AutoFit(); colum++;
                    range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["Pack_Ctn"]); worksheet.Column(colum).AutoFit(); colum++;
                }

                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["SoLuong"]); worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["SLThung"]); worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = Convert.ToInt32(rows["TotalPiece"]); worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = khoiLuong == 0 ? (object)" " : roundedKhoiLuong;
                worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = trongLuong == 0 ? (object)" " : Math.Round(trongLuong, 1);
                worksheet.Column(colum).AutoFit(); colum++;
                range = worksheet.Cells[row, colum]; range.Value = rows["KyHieu"].ToString(); worksheet.Column(colum).AutoFit(); colum++;
                int currentSttThung = Convert.ToInt32(rows["SttThung"]);
                int previousSttThung = (index + 1 < dtPKLXuatHang.Rows.Count) ? Convert.ToInt32(dtPKLXuatHang.Rows[index + 1]["SttThung"]) : 192836403;
                if (currentSttThung == previousSttThung)
                {
                    indexMerge++;
                    checkCountMerge = true;
                }
                else
                {
                    if (checkCountMerge)
                    {
                        rowMerge = row - indexMerge;
                        checkCountMerge = false;
                    }
                    else
                        rowMerge = row;
                    for (int i = 0; i < 5; i++)
                    {
                        MergeCellsByRow(worksheet, colum - i - 2, rowMerge, rowMerge + indexMerge, "center");
                    }
                    MergeCellsByRow(worksheet, 1, rowMerge, rowMerge + indexMerge, "center");
                    MergeCellsByRow(worksheet, 2, rowMerge, rowMerge + indexMerge, "center");
                    rowMerge = row;
                    indexMerge = 0;
                }

                index++;
                row++;

            }
            int indexsumAll = 6;
            foreach (string sumAll in sumToTalAll)
            {
                worksheet.Cells[row, colum - indexsumAll].Value = sumAll;
                worksheet.Cells[row, colum - indexsumAll].Style.Font.Bold = true;
                indexsumAll--;
            }
            var borderData = worksheet.Cells[9, 1, row - 1, colum - 1].Style.Border;
            borderData.Bottom.Style =
                borderData.Top.Style =
                borderData.Left.Style =
                borderData.Right.Style = ExcelBorderStyle.Thin;
            row++;
            range = worksheet.Cells[row, 5]; range.Merge = true; range.Value = "Size/Inseam";
            range = worksheet.Cells[row, 6]; range.Merge = true; range.Value = "Color";
            colum = 7;
            foreach (var colums in columnNamesWithSize)
            {
                string splitColum = colums.Split('@')[0];
                worksheet.Cells[row, colum].Value = splitColum.ToString();
                worksheet.Cells[row, colum].Style.Font.Bold = true;
                worksheet.Column(colum).AutoFit();
                colum++;
            }

            range = worksheet.Cells[row, colum]; range.Merge = true; range.Value = "Total PCS ";
            row++;

            colum = 7;
            rowMerge = row;
            var dauSize = dtPKLXuatHang.AsEnumerable().Select(x => new { DauSize = x["DauSizeID"], TenMau = x["TenMau"], ColorID = x["ColorID"] }).Distinct().ToList();
            foreach (var dausize in dauSize)
            {
                worksheet.Cells[row, 5].Value = dausize.DauSize;
                worksheet.Cells[row, 6].Value = dausize.TenMau;
                colum = 7;
                int sumPCS = 0;
                foreach (var colums in columnNamesWithSize)
                {
                    int sumTotalSize = dtPKLXuatHang.AsEnumerable()
                        .Where(x => x["DauSizeID"].ToString() == dausize.DauSize.ToString() && x["ColorID"].ToString() == dausize.ColorID.ToString())
                        .Sum(y =>
                        {
                            int sizeValue = Convert.ToInt32(y[colums].ToString() == "" ? 0 : y[colums]);
                            int quantity = Convert.ToInt32(y["SLThung"]);
                            int PackCtn = Convert.ToInt32(y["Pack_Ctn"]);
                            return y["PCB_Pack"].ToString() != "0" ? sizeValue * quantity * PackCtn : sizeValue * quantity;
                        });
                    worksheet.Cells[row, colum].Value = sumTotalSize == 0 ? (object)" " : sumTotalSize;
                    worksheet.Column(colum).AutoFit();
                    colum++;
                    sumPCS += sumTotalSize;
                    worksheet.Cells[row, colum].Value = sumPCS;

                }
                indexMerge++;
                row++;
            }
            range = worksheet.Cells[row, 5, row, 6]; range.Merge = true; range.Value = "Tổng";
            colum = 7;
            int sumTotal = 0;
            for (int i = 0; i < columnNamesWithSize.Count + 1; i++)
            {
                worksheet.Cells[row, colum].Formula = "=SUM(" + worksheet.Cells[rowMerge, colum].Address + ":" + worksheet.Cells[rowMerge + indexMerge - 1, colum].Address + ")";
                worksheet.Cells[row, colum].Style.Font.Bold = true;
                colum++;
            }
            row += 2;
            range = worksheet.Cells[row, 7]; range.Merge = true; range.Value = "pcs";
            range = worksheet.Cells[row, 6]; range.Merge = true; range.Value = totalSLThung;
            range = worksheet.Cells[row + 1, 7]; range.Merge = true; range.Value = "set";
            range = worksheet.Cells[row + 1, 6]; range.Merge = true; range.Value = totalTotalPiece;
            range = worksheet.Cells[row + 1, 9]; range.Merge = true; range.Value = "pcs";
            range = worksheet.Cells[row + 1, 8]; range.Merge = true; range.Value = totalTotalPiece * 2;
            range = worksheet.Cells[row + 2, 7]; range.Merge = true; range.Value = "kg";
            range = worksheet.Cells[row + 2, 6]; range.Merge = true; range.Value = roundedTotalKhoiLuong.ToString();
            range = worksheet.Cells[row + 3, 7]; range.Merge = true; range.Value = "kg";
            range = worksheet.Cells[row + 3, 6]; range.Merge = true; range.Value = roundedTotalTrongLuong.ToString();
            range = worksheet.Cells[row + 4, 6]; range.Merge = true; range.Value = dtPKLXuatHang.Rows[0]["KyHieu"].ToString();
            range = worksheet.Cells[row + 5, 6]; range.Merge = true; range.Value = Convert.ToInt32(0.6 * 0.4 * 0.42 * totalSLThung);
            range = worksheet.Cells[row + 5, 7]; range.Merge = true; range.Value = "CBM";

            range = worksheet.Cells[row, 4, row, 5]; range.Merge = true; range.Value = "Total boxes "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 1, 4, row + 1, 5]; range.Merge = true; range.Value = "Total quantity  "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 2, 4, row + 2, 5]; range.Merge = true; range.Value = "Total net weight"; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 3, 4, row + 3, 5]; range.Merge = true; range.Value = "Total Gross weight"; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 4, 4, row + 4, 5]; range.Merge = true; range.Value = "Cnt Measurements "; range.Style.Font.Bold = true;
            range = worksheet.Cells[row + 5, 4, row + 5, 5]; range.Merge = true; range.Value = "Total CBM "; range.Style.Font.Bold = true;
            for (int i = 1; i < 7; i++)
            {
                worksheet.Cells[row + i - 1, 4, row + i - 1, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }
            var borderData1 = worksheet.Cells[rowMerge - 1, 5, rowMerge + indexMerge, colum - 1].Style.Border;
            borderData1.Bottom.Style =
                borderData1.Top.Style =
                borderData1.Left.Style =
                borderData1.Right.Style = ExcelBorderStyle.Thin;*/

        }
        private async void LuuDong()
        {
            if ( 
                string.IsNullOrWhiteSpace(txtMaVatTu.Text) ||
                string.IsNullOrWhiteSpace(txtTenVatTu.Text) ||
                string.IsNullOrWhiteSpace(txtNhaCungCap.Text) ||
                string.IsNullOrWhiteSpace(txtMaHaiQuan.Text) 
               )
            {
                XtraMessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi API GetNL để lấy danh sách nguyên liệu hiện có
            string urlGetNL = string.Format("{0}?", URL + "KhoNPL/GetNL");
            string jsonGetNL = await _clientExtension.GetAsnyc(urlGetNL);
            List<NguyenLieuEntity> existingNguyenLieuList = JsonConvert.DeserializeObject<List<NguyenLieuEntity>>(jsonGetNL);

            /* // Kiểm tra sự trùng lặp của MaVatTu
             if (existingNguyenLieuList.Any(nl => nl.MaVatTu == txtMaVatTu.Text))
             {
                 XtraMessageBox.Show("Mã vật tư đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
             }*/
            
            string[] parts = txtNhomNPL.Text.Split(new string[] { ". " }, StringSplitOptions.None);
            string idPart = "";
            string tenNhomPart = "";
            if (parts.Length >= 2) // Đảm bảo mảng có ít nhất 2 phần tử
            {       
                idPart = parts[0].Replace("Mã nhóm: ", "").Trim();
                tenNhomPart = parts[1].Replace("Tên Nhóm: ", "").Trim();
            }
            NguyenLieuEntity nguyenLieu = new NguyenLieuEntity
            {
                ID = int.TryParse(txtID.Text, out int id) ? id : 0,
                MaDH = "",
                MaVatTu = txtMaVatTu.Text,
                TenVatTu = txtTenVatTu.Text,
                NhaCungCap = txtNhaCungCap.Text,
                MaHaiQuan = txtMaHaiQuan.Text,
                ItemCode = txtItemCode.Text,
                LoNhap = "",
                KhoVai = txtKhoVai.Text,
                CodeMau = txtCodeMau.Text,
                TenCodeMau = txtTenCodeMau.Text,
                TongKien = "",
                Kien = "",
                Lot = txtLot.Text,
                AnhMau = txtAnhMau.Text, 
                NhomNPL = idPart,
                TenNhom = tenNhomPart,
                SoLuong = 0,
                DonViTinh = string.IsNullOrWhiteSpace(txtDonViTinh.Text)? "" : txtDonViTinh.Text,

                BarCode = "",
                MaKeToan=txtMaKeToan.Text,
                MaNguyenLieu = txtMaVatTu.Text+ txtKhoVai.Text,
                TenDVCL=""
            };

            List<NguyenLieuEntity> lstPost = new List<NguyenLieuEntity> { nguyenLieu };
            string urlPostNL = string.Format("{0}?", URL + "KhoNPL/PostNL");
            string msResult = await _clientExtension.PostAsync(urlPostNL, lstPost);

            if (msResult.ToLower() == "true")
            {
                LoadDSNguyenLieu(false);
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            else
            {
                XtraMessageBox.Show(msResult);
            }

           
            txtMaVatTu.Properties.ReadOnly = true;
            txtTenVatTu.Properties.ReadOnly = true;
            txtNhaCungCap.Properties.ReadOnly = true;
            txtMaHaiQuan.Properties.ReadOnly = true;
            txtItemCode.Properties.ReadOnly = true;
           
            txtKhoVai.Properties.ReadOnly = true;
            txtCodeMau.Properties.ReadOnly = true;
            txtTenCodeMau.Properties.ReadOnly = true;
            
            txtLot.Properties.ReadOnly = true;
            txtAnhMau.Properties.ReadOnly = true;
            txtNhomNPL.Properties.ReadOnly = true;
          
            txtDonViTinh.Properties.ReadOnly = true;
            txtMaKeToan.Properties.ReadOnly = true;

        }

        private void btnXuatMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("TemplateNguyenLieu{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateNguyenLieu.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName);

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Mở File", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(Sfd.FileName))
                            System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                    catch
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Error..");
                    }
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }

        }
        public void Export(string TemplateFileName, string ExportFileName)
        {
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        // đặt tên người tạo file
                        excelPackage.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        excelPackage.Workbook.Properties.Title = "NguyenLieu";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                       
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                object misValue = System.Reflection.Missing.Value;
                throw new Exception(ex.Message);
            }


        }
        private void LoadNhomNPL(string NhomNPL = "")
        {
            try
            {
                // Định nghĩa URL API
                string url = string.Format("{0}?", URL + "KhoNPL/GetNhomNL");

                // Gọi API để lấy danh sách NhomNPLEntity
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                // Kiểm tra nếu API trả về dữ liệu
                if (!string.IsNullOrEmpty(json)&&json!="[]")
                {
                    // Deserialize JSON thành danh sách NhomNPLEntity
                    List<KhoNPLEntity> listNhomNPL = JsonConvert.DeserializeObject<List<KhoNPLEntity>>(json);
                    // Xóa sạch các mục hiện tại trong combo box
                    txtNhomNPL.Properties.Items.Clear();

                    // Thêm từng mục vào combo box
                    foreach (var item in listNhomNPL)
                    {
                        txtNhomNPL.Properties.Items.Add("Mã nhóm: "+item.MaNhom.ToString()+ ". Tên Nhóm:"+ item.TenNhom.ToString());
 
                    }

                    // Tìm phần tử có ID bằng với ID truyền vào
                    KhoNPLEntity nhomNPL = listNhomNPL.FirstOrDefault(n => n.ID == NhomNPL);

                    // Kiểm tra nếu tìm thấy phần tử
                    if (nhomNPL != null)
                    {
                        // Gán giá trị cho txtMaNhom và txtTenNhom
                        txtNhomNPL.SelectedItem = nhomNPL.ID.ToString();
                       
                    }
                    else
                    {
                        if (listNhomNPL != null && listNhomNPL.Count > 0)
                        {
                            var firstNhom = listNhomNPL[0]; // Lấy phần tử đầu tiên
                            txtNhomNPL.SelectedItem = firstNhom.ID.ToString(); // Gán ID của phần tử đầu tiên
                           
                        }
                        else
                        {
                            // Nếu danh sách trống, set giá trị mặc định trống
                            txtNhomNPL.SelectedItem = null;
                           
                        }
                    }
                }
                else
                {
                    // Nếu API không trả về dữ liệu, gán giá trị mặc định
                    txtNhomNPL.SelectedItem = "";
                 
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu xảy ra
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu nhóm NPL. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadDVCL(string donvitinh = "")
        {
            try
            {
                // Định nghĩa URL API
                string url = string.Format("{0}?", URL + "DonViChungLoai/Get");

                // Gọi API để lấy danh sách NhomNPLEntity
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                // Kiểm tra nếu API trả về dữ liệu
                if (!string.IsNullOrEmpty(json) && json !="[]")
                {
                    // Deserialize JSON thành danh sách NhomNPLEntity
                    List<DonViChungLoaiEntity> listDVT = JsonConvert.DeserializeObject<List<DonViChungLoaiEntity>>(json);
                    // Xóa sạch các mục hiện tại trong combo box
                    txtDonViTinh.Properties.Items.Clear();

                    // Thêm từng mục vào combo box
                    foreach (var item in listDVT)
                    {
                        txtDonViTinh.Properties.Items.Add(item.TenDVCL.ToString());

                    }

                    // Tìm phần tử có ID bằng với ID truyền vào
                    DonViChungLoaiEntity nhomNPL = listDVT.FirstOrDefault(n => n.MaDVCL == donvitinh);

                    // Kiểm tra nếu tìm thấy phần tử
                    if (nhomNPL != null)
                    {
                        // Gán giá trị cho txtMaNhom và txtTenNhom
                        txtDonViTinh.SelectedItem = nhomNPL.ID.ToString();

                    }
                    else
                    {
                        if (listDVT != null && listDVT.Count > 0)
                        {
                            var firstNhom = listDVT[0]; // Lấy phần tử đầu tiên
                            txtDonViTinh.SelectedItem = firstNhom.ID.ToString(); // Gán ID của phần tử đầu tiên

                        }
                        else
                        {
                            // Nếu danh sách trống, set giá trị mặc định trống
                            txtDonViTinh.SelectedItem = null;

                        }
                    }
                }
                else
                {
                    // Nếu API không trả về dữ liệu, gán giá trị mặc định
                    txtDonViTinh.SelectedItem = "";

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu xảy ra
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu nhóm NPL. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}