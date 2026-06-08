using DevExpress.Utils.Menu;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmViTriKhoNew : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _make = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tbl_ViTriKho = new DataTable();
        DataTable tbl_SaveViTriKho = new DataTable();
        DataTable tblCopy = new DataTable();
        DataTable tblTangCopy = new DataTable();
        DataTable tblOCopy = new DataTable();
        private bool checkTrue = false;
        private bool checkKeTrue = false;
        private bool checkTangTrue = false;
        public frmViTriKhoNew()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadInit();
            LoadDVSX();
        }
        private void CreateTbl()
        {
            tbl_ViTriKho = new DataTable();
            tbl_ViTriKho.Columns.Add("NameKe", typeof(string));
            tbl_ViTriKho.Columns.Add("NameTang", typeof(string));
            tbl_ViTriKho.Columns.Add("NameO", typeof(string));
            tbl_ViTriKho.Columns.Add("NameDay", typeof(string));
            tbl_ViTriKho.Columns.Add("MaKe", typeof(string));
            tbl_ViTriKho.Columns.Add("MaTang", typeof(string));
            tbl_ViTriKho.Columns.Add("MaO", typeof(string));
            tbl_ViTriKho.Columns.Add("MaDay", typeof(string));
            tbl_ViTriKho.Columns.Add("Dai", typeof(float));
            tbl_ViTriKho.Columns.Add("Rong", typeof(float));
            tbl_ViTriKho.Columns.Add("Cao", typeof(float));
            tbl_ViTriKho.Columns.Add("CBM", typeof(float));
        }
        private void CreateSaveTbale()
        {
            tbl_SaveViTriKho = new DataTable();
            tbl_SaveViTriKho.Columns.Add("MaDay", typeof(string));
            tbl_SaveViTriKho.Columns.Add("TenDay", typeof(string));
            tbl_SaveViTriKho.Columns.Add("MaKe", typeof(string));
            tbl_SaveViTriKho.Columns.Add("TenKe", typeof(string));
            tbl_SaveViTriKho.Columns.Add("MaTang", typeof(string));
            tbl_SaveViTriKho.Columns.Add("TenTang", typeof(string));
            tbl_SaveViTriKho.Columns.Add("MaO", typeof(string));
            tbl_SaveViTriKho.Columns.Add("TenO", typeof(string));
            tbl_SaveViTriKho.Columns.Add("Dai", typeof(float));
            tbl_SaveViTriKho.Columns.Add("Cao", typeof(float));
            tbl_SaveViTriKho.Columns.Add("Rong", typeof(float));
            tbl_SaveViTriKho.Columns.Add("CBM", typeof(float));
            tbl_SaveViTriKho.Columns.Add("NgayTao", typeof(string));
            tbl_SaveViTriKho.Columns.Add("MaDVSX", typeof(string));
            tbl_SaveViTriKho.Columns.Add("MaKho", typeof(string));
        }
        private void loadInit()
        {
            searchLookUpEdit1.Properties.ValueMember = "MaDVSX";
            searchLookUpEdit1.Properties.DisplayMember = "TenDVSX";
            searchLookUpEdit1.Properties.NullText = "[Chọn đơn vị sx]";

            searchLookUpEdit2.Properties.ValueMember = "MaKho";
            searchLookUpEdit2.Properties.DisplayMember = "TenKho";
            searchLookUpEdit2.Properties.NullText = "[Chọn kho]";
        }
        private string CreateMa(string maViTri)
        {
            var match = Regex.Match(maViTri, @"\d+");

            return match.Value;
        }
        private void LoadDVSX()
        {
            string userName = GlobleData.UserName;
            string url = $"{URL}ViTriKhoNew/Get?Action=GetDVSX&Para1={userName}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
                return;
            else
                searchLookUpEdit1.Properties.DataSource = tbl;

        }
        private void LoadMaKho()
        {
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string url = $"{URL}ViTriKhoNew/Get?Action=GetViTriKho&Para1={madvsx}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
            {

                return;
            }
            else
            {
                searchLookUpEdit2.Properties.DataSource = tbl;
                searchLookUpEdit2.EditValue = tbl.Rows[0]["MaKho"].ToString();
                LoadKhuVucKho();
            }
        }
        private void LoadKhuVucKho()
        {
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho =   searchLookUpEdit2.EditValue.ToString();
            string url = $"{URL}ViTriKhoNew/Get?Action=GetDay&Para1={madvsx}&Para2={makho}&Para3=a&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcKe.DataSource = null;
                grcTang.DataSource = null;
                grcO.DataSource = null;
                CreateTbl();
                grcDay.DataSource = tbl_ViTriKho;

            }
            else
                grcDay.DataSource = tbl;

        }
        private void loadKe(string maday)
        {

            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue.ToString();
            string url = $"{URL}ViTriKhoNew/Get?Action=GetKe&Para1={madvsx}&Para2={makho}&Para3={maday}&Para4=a&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                CreateTbl();
                grcKe.DataSource = tbl_ViTriKho;
                grcTang.DataSource = null;
                grcO.DataSource = null;
            }
            else
            {
                grcKe.DataSource = tbl;
                string make = tbl.Rows[0]["MaKe"].ToString();
                loadTang(make, maday);
            }

        }
        private void loadTang(string make, string makv)
        {
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue.ToString();
            string url = $"{URL}ViTriKhoNew/Get?Action=GetTang&Para1={madvsx}&Para2={makho}&Para3={makv}&Para4={make}&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                CreateTbl();
                grcTang.DataSource = tbl_ViTriKho;
                grcO.DataSource = null;
            }
            else
            {
                grcTang.DataSource = tbl;
                string matang = tbl.Rows[0]["MaTang"].ToString();
                loadO(make, matang, makv);
            }
               

        }
        private void loadO(string make, string matang, string makv)
        {
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue.ToString();
            string url = $"{URL}ViTriKhoNew/Get?Action=GetO&Para1={madvsx}&Para2={makho}&Para3={makv}&Para4={make}&Para5={matang}&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                CreateTbl();
                grcO.DataSource = tbl_ViTriKho;

            }
            else
                grcO.DataSource = tbl;

        }
        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            grcDay.DataSource = null;
            grcKe.DataSource = null;
            grcTang.DataSource = null;
            grcO.DataSource = null;
          
            LoadMaKho();
        }

        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            LoadKhuVucKho();
        }

        private void grvKe_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = grvKe.GetFocusedDataRow();
            if (dr == null) return;
            if (e.Column.FieldName == "NameKe")
            {
                string tenName = replacekhoanCach(RemoveVietnameseTone(dr["NameKe"].ToString()));
                dr["MaKe"] = "MK_" + tenName;
            }
            if (e.Column.FieldName == "Dai" || e.Column.FieldName == "Rong" || e.Column.FieldName == "Cao")
            {
                float dai = dr["Dai"] == DBNull.Value ? 0 : Convert.ToSingle(dr["Dai"]);
                float rong = dr["Rong"] == DBNull.Value ? 0 : Convert.ToSingle(dr["Rong"]);
                float cao = dr["Cao"] == DBNull.Value ? 0 : Convert.ToSingle(dr["Cao"]);
                if (dai == 0 || rong == 0 || cao == 0) return;
                dr["CBM"] = dai * rong * cao;
            }
        }
        private void grvKe_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                DataRow drRow = grvKe.GetFocusedDataRow();
                if (e.Value == null || e.Value.ToString() == "") return;
                string valueKe = CreateMa(e.Value.ToString());

                string inputValue = e.Value.ToString();

                DataTable tbl = grcKe.DataSource as DataTable;
                foreach (DataRow item in tbl.Rows)
                {
                    if (item["NameKe"].ToString() == inputValue.ToString() && drRow["NameKe"].ToString() != inputValue.ToString())
                    {
                        e.Valid = false;
                        e.ErrorText = $"Tên {inputValue} đã tồn tại";
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void grvTang_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = grvTang.GetFocusedDataRow();
            DataRow drKe = grvKe.GetFocusedDataRow();
            DataRow drDay = grvDay.GetFocusedDataRow();
            if (dr == null || drKe == null) return;
            string namDay = sliptTang(drKe["NameKe"].ToString());
            if (e.Column.FieldName == "NameTang")
            {
                string tenName = dr["NameTang"].ToString();
                dr["MaTang"] = "MT_0" + CreateMa(tenName);
                dr["NameTang"] = CreateMa(tenName) + namDay;
                dr["MaKe"] = drKe["MaKe"];
                dr["NameKe"] = drKe["NameKe"];
                dr["Dai"] = drKe["Dai"];
                dr["Rong"] = drKe["Rong"];
            }
            if (e.Column.FieldName == "Dai" || e.Column.FieldName == "Rong" || e.Column.FieldName == "Cao")
            {
                float dai = dr["Dai"] == DBNull.Value ? 0 : Convert.ToSingle(dr["Dai"]);
                float rong = dr["Rong"] == DBNull.Value ? 0 : Convert.ToSingle(dr["Rong"]);
                float cao = dr["Cao"] == DBNull.Value ? 0 : Convert.ToSingle(dr["Cao"]);
                dr["CBM"] = dai * rong * cao;
            }
        }
        public string sliptTang(string tangNew)
        {
            string tang = tangNew.Split(' ')[1];
            return tang;
        }
        private void grvTang_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                if (grvTang.FocusedColumn.FieldName != "NameTang")
                {
                    return;
                }
                DataRow dr = grvTang.GetFocusedDataRow();
                if (e.Value == null || e.Value.ToString() == "") return;
                string valueKe = CreateMa(e.Value.ToString());
                if (valueKe == "")
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập stt tầng";
                }
                string inputValue = e.Value.ToString();
                if (!Regex.IsMatch(inputValue, @"^\d+$"))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng chỉ nhập số.";
                    return;
                }
                string inputAddTang = "Tầng " + inputValue;
                DataTable tbl = grcTang.DataSource as DataTable;
                foreach (DataRow item in tbl.Rows)
                {
                    if (item["NameTang"].ToString() == inputAddTang && dr["NameTang"].ToString() != inputAddTang)
                    {
                        e.Valid = false;
                        e.ErrorText = $"Tên tầng {inputValue} đã tồn tại";
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void MenuClickCopyDataRow(object sender, EventArgs e)
        {
            checkTrue = true;
            tblCopy = grcKe.DataSource as DataTable;
        }
        private void MenuClickPastDataRow(object sender, EventArgs e)
        {
            checkTrue = false;
            if (tblCopy.Rows.Count == 0) return;
            grcKe.DataSource = tblCopy;
        }

        public string replacekhoanCach(string text)
        {
            string pattern = "[^a-zA-Z0-9]";
            string replaced = Regex.Replace(text, pattern, "_");
            return replaced;
        }

        private void MenuClickCopyKeDataRow(object sender, EventArgs e)
        {
            checkKeTrue = true;
            tblTangCopy = grcTang.DataSource as DataTable;
        }
        private void MenuClickKePastDataRow(object sender, EventArgs e)
        {
            checkKeTrue = false;
            if (tblTangCopy.Rows.Count == 0) return;
            DataRow drRowKe = grvKe.GetFocusedDataRow();
            string namDay = sliptTang(drRowKe["NameKe"].ToString());
            int index = 1;
            foreach (DataRow item in tblTangCopy.Rows)
            {
                item["NameTang"] = index + namDay;
                index++;
            }
            grcTang.DataSource = tblTangCopy;
        }


        private void MenuClickDeleteKVDataRow(object sender, EventArgs e)
        {
            try
            {
                DataRow drRow = grvDay.GetFocusedDataRow();
                if (drRow == null) return;
                string makv = drRow["MaDay"].ToString();
                string url = $"{URL}ViTriKhoNew/Get?Action=DeleteKV&Para1={makv}&Para2=a&Para3=a&Para4=a&Para5=a&Para6=a";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                LoadKhuVucKho();
                clsWaitForm.ShowSuccessForm(this, 1000);
            }
            catch(Exception ex)
            {

            }
            

        }

        private void MenuClickDeleteKeDataRow(object sender, EventArgs e)
        {
            try
            {
                DataRow drRow = grvDay.GetFocusedDataRow();
                DataRow drRowKe = grvKe.GetFocusedDataRow();
                if (drRow == null) return;
                string makv = drRow["MaDay"].ToString();
                string make = drRowKe["MaKe"].ToString();
                string url = $"{URL}ViTriKhoNew/Get?Action=DeleteKe&Para1={makv}&Para2={make}&Para3=a&Para4=a&Para5=a&Para6=a";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                loadKe(makv);
                clsWaitForm.ShowSuccessForm(this, 1000);
            }
            catch (Exception ex)
            {

            }
           
        }
        private void MenuClickDeleteTangDataRow(object sender, EventArgs e)
        {
            try
            {
                DataRow drRow = grvDay.GetFocusedDataRow();
                DataRow drRowKe = grvKe.GetFocusedDataRow();
                DataRow drRowTang = grvTang.GetFocusedDataRow();
                if (drRow == null) return;
                string makv = drRow["MaDay"].ToString();
                string make = drRowKe["MaKe"].ToString();
                string matang = drRowTang["MaTang"].ToString();
                string url = $"{URL}ViTriKhoNew/Get?Action=DeleteTang&Para1={makv}&Para2={make}&Para3={matang}&Para4=a&Para5=a&Para6=a";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                loadTang(make, makv);
                clsWaitForm.ShowSuccessForm(this, 1000);
            }
            catch (Exception ex)
            {

            }


        }


        private void MenuClickCopyTangDataRow(object sender, EventArgs e)
        {
            checkTangTrue = true;
            tblOCopy = grcO.DataSource as DataTable;
        }
        private void MenuClickPastTangDataRow(object sender, EventArgs e)
        {
            try
            {
                checkTangTrue = false;
                DataRow drRowTang = grvTang.GetFocusedDataRow();
                string nameTang = drRowTang["NameTang"].ToString();
                DataRow drRowKV = grvDay.GetFocusedDataRow();
                string nameKV = drRowKV["NameDay"].ToString();
                if (tblOCopy.Rows.Count == 0) return;
                foreach (DataRow item in tblOCopy.Rows)
                {
                    item["NameO"] = ReplacePart(item["NameO"].ToString(), nameTang, nameKV);
                }
                grcO.DataSource = tblOCopy;
            }
            catch (Exception)
            {

            }
            
        }
        public static string ReplacePart(string original, string replacement,string kv)
        {
            string[] parts = original.Split('.');
            parts[1] = replacement;
            parts[0] = kv;
            string result = string.Join(".", parts);
            return result;
        }
        private void MenuClickDeleteODataRow(object sender, EventArgs e)
        {
            DataRow drRow = grvDay.GetFocusedDataRow();
            DataRow drRowKe = grvKe.GetFocusedDataRow();
            DataRow drRowTang = grvTang.GetFocusedDataRow();
            DataRow drRowO = grvO.GetFocusedDataRow();
            if (drRow == null) return;
            string makv = drRow["MaDay"].ToString();
            string make = drRowKe["MaKe"].ToString();
            string matang = drRowTang["MaTang"].ToString();
            string maO = drRowO["MaO"].ToString();
            string url = $"{URL}ViTriKhoNew/Get?Action=DeleteO&Para1={makv}&Para2={make}&Para3={matang}&Para4={maO}&Para5=a&Para6=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            loadO(make, matang, makv);
            clsWaitForm.ShowSuccessForm(this, 1000);
        }
        private void grvDay_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                DataRow dataRow = grvDay.GetFocusedDataRow();
                if (dataRow == null || e.Menu == null) return;
                DXMenuItem menuCopy = new DXMenuItem();
                menuCopy.Caption = "Sao chép";
                menuCopy.Click += MenuClickCopyDataRow;
                e.Menu.Items.Add(menuCopy);
                if (checkTrue)
                {
                    DXMenuItem menuPast = new DXMenuItem();
                    menuPast.Caption = "Dán";
                    menuPast.Click += MenuClickPastDataRow;
                    e.Menu.Items.Add(menuPast);
                }
                DXMenuItem menuXoa = new DXMenuItem();
                menuXoa.Caption = "Xóa";
                menuXoa.Click += MenuClickDeleteKVDataRow;
                e.Menu.Items.Add(menuXoa);

            }
            catch (Exception ex)
            {
            }
          
        }
        private void grvDay_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = grvDay.GetFocusedDataRow();
            if (dr == null) return;
            string makv = dr["MaDay"].ToString();
            loadKe(makv);
        }

        private void grvDay_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {

                DataRow dr = grvDay.GetFocusedDataRow();
                if (e.Value == null || e.Value.ToString() == "") return;
                string inputValue = e.Value.ToString();
                DataTable tbl = grcDay.DataSource as DataTable;
                if (tbl == null) return;
                foreach (DataRow item in tbl.Rows)
                {
                    if (item["NameDay"].ToString() == inputValue && dr["NameDay"].ToString() != inputValue)
                    {
                        e.Valid = false;
                        e.ErrorText = $"Tên  {inputValue} đã tồn tại";
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void grvDay_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                CreateSaveTbale();
                DataRow dr = grvDay.GetFocusedDataRow();
                if (dr == null) return;
                string TenKhuVuc = dr["NameDay"].ToString();

                string madvsx = searchLookUpEdit1.EditValue.ToString();
                string makho = searchLookUpEdit2.EditValue.ToString();
                if (madvsx == null || makho == null)
                {
                    MessageBox.Show("Vui lòng chọn đơn vị sx hoặc chọn kho", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow drRow = tbl_SaveViTriKho.NewRow();
                if (dr["MaDay"] == DBNull.Value)
                {
                    drRow["MaDay"] = "MaKV_" + replacekhoanCach(RemoveVietnameseTone(TenKhuVuc));
                }
                else drRow["MaDay"] = dr["MaDay"];
                drRow["TenDay"] = TenKhuVuc;
                drRow["MaDVSX"] = madvsx;
                drRow["MaKho"] = makho;
                tbl_SaveViTriKho.Rows.Add(drRow);
                string url = $"{URL}ViTriKhoNew/Post?Action=PostDay";
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl_SaveViTriKho); }).Result;

                LoadKhuVucKho();
            }
            catch (Exception ex)
            {

            }

        }
        private void grvKe_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                DataRow dataRow = grvDay.GetFocusedDataRow();
                if (dataRow == null || e.Menu == null) return;
                DXMenuItem menuCopy = new DXMenuItem();
                menuCopy.Caption = "Sao chép";
                menuCopy.Click += MenuClickCopyKeDataRow;
                e.Menu.Items.Add(menuCopy);
                if (checkKeTrue)
                {
                    DXMenuItem menuPast = new DXMenuItem();
                    menuPast.Caption = "Dán";
                    menuPast.Click += MenuClickKePastDataRow;
                    e.Menu.Items.Add(menuPast);
                }
                DXMenuItem menuXoa = new DXMenuItem();
                menuXoa.Caption = "Xóa";
                menuXoa.Click += MenuClickDeleteKeDataRow;
                e.Menu.Items.Add(menuXoa);
            }
            catch (Exception ex)
            {

            }
           
        }
        private void grvTang_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                DataRow dataRow = grvDay.GetFocusedDataRow();
                if (dataRow == null || e.Menu == null) return;
                DXMenuItem menuCopy = new DXMenuItem();
                menuCopy.Caption = "Sao chép";
                menuCopy.Click += MenuClickCopyTangDataRow;
                e.Menu.Items.Add(menuCopy);
                if (checkTangTrue)
                {
                    DXMenuItem menuPast = new DXMenuItem();
                    menuPast.Caption = "Dán";
                    menuPast.Click += MenuClickPastTangDataRow;
                    e.Menu.Items.Add(menuPast);
                }
                DXMenuItem menuXoa = new DXMenuItem();
                menuXoa.Caption = "Xóa";
                menuXoa.Click += MenuClickDeleteTangDataRow;
                e.Menu.Items.Add(menuXoa);
            }
            catch (Exception ex)
            {

            }
           
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
          
        }

        private void btnLuuKe_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void grvTang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow drRow = grvKe.GetFocusedDataRow();
            DataRow drRowTang = grvTang.GetFocusedDataRow();
            DataRow dr = grvDay.GetFocusedDataRow();
            if (drRow == null || drRowTang == null || dr == null) return;
            _make = drRow["MaKe"].ToString();
            string matang = drRowTang["MaTang"].ToString();
            string makv = dr["MaDay"].ToString();
            loadO(_make, matang, makv);
        }

        private void grvKe_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow drRow = grvKe.GetFocusedDataRow();
            DataRow dr = grvDay.GetFocusedDataRow();
            if (drRow == null || dr == null) return;
            _make = drRow["MaKe"].ToString();
            string makv = dr["MaDay"].ToString();
            loadTang(_make, makv);
        }

        private void grvO_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = grvTang.GetFocusedDataRow();
            DataRow drKe = grvKe.GetFocusedDataRow();
            DataRow drRowKV = grvDay.GetFocusedDataRow();
            DataRow drRowO = grvO.GetFocusedDataRow();
            if (dr == null || drKe == null || drRowKV == null) return;
            string nameKV = drRowKV["NameDay"].ToString();
            string namDay = sliptTang(drKe["NameKe"].ToString());
            string nameTang = dr["NameTang"].ToString();
            if (e.Column.FieldName == "NameO")
            {
                string tenName = drRowO["NameO"].ToString();
                drRowO["MaO"] = "MO_0" + CreateMa(tenName);
                drRowO["NameO"] = $"{nameKV}.{nameTang}.{CreateMa(tenName)}" ;
                drRowO["Rong"] = dr["Rong"];
                drRowO["Cao"] = dr["Cao"];
              
            }
            if (e.Column.FieldName == "Dai" || e.Column.FieldName == "Rong" || e.Column.FieldName == "Cao")
            {
                float dai = drRowO["Dai"] == DBNull.Value ? 0 : Convert.ToSingle(drRowO["Dai"]);
                float rong = drRowO["Rong"] == DBNull.Value ? 0 : Convert.ToSingle(drRowO["Rong"]);
                float cao = drRowO["Cao"] == DBNull.Value ? 0 : Convert.ToSingle(drRowO["Cao"]);
                drRowO["CBM"] = dai * rong * cao;
            }
        }

        private void grvO_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                if (grvO.FocusedColumn.FieldName != "NameO")
                {
                    return;
                }
                DataRow dr = grvO.GetFocusedDataRow();
                if (e.Value == null || e.Value.ToString() == "") return;
                string valueKe = CreateMa(e.Value.ToString());
                if (valueKe == "")
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập stt tầng";
                }
                string inputValue = e.Value.ToString();
                if (!Regex.IsMatch(inputValue, @"^\d+$"))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng chỉ nhập số.";
                    return;
                }
                string inputAddTang = "Ô " + inputValue;
                DataTable tbl = grcO.DataSource as DataTable;
                foreach (DataRow item in tbl.Rows)
                {
                    if (item["NameO"].ToString() == inputAddTang && dr["NameO"].ToString() != inputAddTang)
                    {
                        e.Valid = false;
                        e.ErrorText = $"Tên ô {inputValue} đã tồn tại";
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void grvO_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                DXMenuItem menuXoa = new DXMenuItem();
                menuXoa.Caption = "Xóa";
                menuXoa.Click += MenuClickDeleteODataRow;
                e.Menu.Items.Add(menuXoa);
            }
            catch (Exception ex)
            {

            }
        }

        private void grcKe_Click(object sender, EventArgs e)
        {
           
        }

        private void btnSaveTang_Click(object sender, EventArgs e)
        {
            CreateSaveTbale();
            DataTable tbl = grcTang.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
                return;
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue.ToString();
            DataRow drRowGrvDay = grvDay.GetFocusedDataRow();
            DataRow dataRowgrvKE = grvKe.GetFocusedDataRow();
            if (drRowGrvDay == null || dataRowgrvKE == null) return;
            foreach (DataRow item in tbl.Rows)
            {
                DataRow drRow = tbl_SaveViTriKho.NewRow();
                drRow["MaDay"] = drRowGrvDay["MaDay"];
                drRow["TenDay"] = drRowGrvDay["NameDay"];
                drRow["MaKe"] = dataRowgrvKE["MaKe"];
                drRow["TenKe"] = dataRowgrvKE["NameKe"];
                drRow["MaTang"] = item["MaTang"];
                drRow["TenTang"] = item["NameTang"];
                drRow["Dai"] = item["Dai"];
                drRow["Rong"] = item["Rong"];
                drRow["Cao"] = item["Cao"];
                drRow["CBM"] = item["CBM"];
                drRow["NgayTao"] = "";
                drRow["MaDVSX"] = madvsx;
                drRow["MaKho"] = makho;
                tbl_SaveViTriKho.Rows.Add(drRow);
            }
            string url = $"{URL}ViTriKhoNew/Post?Action=PostTang";
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl_SaveViTriKho); }).Result;
            clsWaitForm.ShowSuccessForm(this, 1000);
        }

        private void btnSaveKe_Click(object sender, EventArgs e)
        {
            CreateSaveTbale();
            DataTable tbl = grcKe.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
                return;
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue.ToString();
            DataRow drRowGrvDay = grvDay.GetFocusedDataRow();

            foreach (DataRow item in tbl.Rows)
            {
                DataRow drRow = tbl_SaveViTriKho.NewRow();
                drRow["MaDay"] = drRowGrvDay["MaDay"];
                drRow["TenDay"] = drRowGrvDay["NameDay"];
                drRow["MaKe"] = item["MaKe"];
                drRow["TenKe"] = item["NameKe"];
                drRow["Dai"] = item["Dai"];
                drRow["Rong"] = item["Rong"];
                drRow["Cao"] = item["Cao"];
                drRow["CBM"] = item["CBM"];
                drRow["NgayTao"] = "";
                drRow["MaDVSX"] = madvsx;
                drRow["MaKho"] = makho;
                tbl_SaveViTriKho.Rows.Add(drRow);
            }
            string url = $"{URL}ViTriKhoNew/Post?Action=PostKe";
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl_SaveViTriKho); }).Result;
            clsWaitForm.ShowSuccessForm(this, 1000);
        }

        private void btnSaveO_Click(object sender, EventArgs e)
        {
            CreateSaveTbale();
            DataTable tbl = grcO.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
                return;
            string madvsx = searchLookUpEdit1.EditValue.ToString();
            string makho = searchLookUpEdit2.EditValue.ToString();
            DataRow drRowGrvDay = grvDay.GetFocusedDataRow();
            DataRow dataRowgrvKe = grvKe.GetFocusedDataRow();
            DataRow dataRowgrvTang = grvTang.GetFocusedDataRow();
            if (drRowGrvDay == null || dataRowgrvKe == null || dataRowgrvTang == null) return;
            foreach (DataRow item in tbl.Rows)
            {
                DataRow drRow = tbl_SaveViTriKho.NewRow();
                drRow["MaDay"] = drRowGrvDay["MaDay"];
                drRow["TenDay"] = drRowGrvDay["NameDay"];
                drRow["MaKe"] = dataRowgrvKe["MaKe"];
                drRow["TenKe"] = dataRowgrvKe["NameKe"];
                drRow["MaTang"] = dataRowgrvTang["MaTang"];
                drRow["TenTang"] = dataRowgrvTang["NameTang"];
                drRow["MaO"] = item["MaO"];
                drRow["TenO"] = item["NameO"];
                drRow["Dai"] = item["Dai"];
                drRow["Rong"] = item["Rong"];
                drRow["Cao"] = item["Cao"];
                drRow["CBM"] = item["CBM"];
                drRow["NgayTao"] = ExtractNumberAfterA(item["NameO"].ToString());
                drRow["MaDVSX"] = madvsx;
                drRow["MaKho"] = makho;
                tbl_SaveViTriKho.Rows.Add(drRow);
            }
            string url = $"{URL}ViTriKhoNew/Post?Action=PostO";
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl_SaveViTriKho); }).Result;
            clsWaitForm.ShowSuccessForm(this, 1000);
        }
        public static string ExtractNumberAfterA(string input)
        {

            string[] past = input.Split('.');
            return past[2]; 
        }
        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
    }
}


