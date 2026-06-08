using DevExpress.XtraEditors;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmCreatePhieuNhapKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _ngay = string.Empty, _ngayNhap = string.Empty;
        private HttpClientExtension _clientExtension;
        KeyDownControlHandler keyDownControlHandler;
        int _soPhieu = 0, _soTo = 0;
        bool _ischeckEdit = false;
        public frmCreatePhieuNhapKho(int soPhieu, int soTo, string ngayThang, string ngayNhap, bool isCheckEdit = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            this._soPhieu = soPhieu;
            this._ischeckEdit = isCheckEdit;
            this._soTo = soTo;
            this._ngay = ngayThang;
            this._ngayNhap = ngayNhap;
        }
        protected override void OnLoad(EventArgs e)
        {
            dateEdit1.EditValue = DateTime.Now;
            if (_ischeckEdit) loadEditDS();
            else
                LoadDS();
            LoadMaPhieu();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());

            CreateTable();
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            AddActionControl(_lstActionControl, BtnLuu, true, ActionType.Save);
            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void loadEditDS()
        {
            DateTime ngayNhapKho = DateTime.ParseExact(_ngayNhap, "dd/MM/yyyy", null);
            string url = $"{URL}ThongKeDongThung/Get?Action=GetEditPhieuNhapKho&Para1={ngayNhapKho}&Para2=A&Para3=A&Para4=A&Para5=A";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = tbl;
            textEdit3.EditValue = tbl.Rows[0]["GhiChu"].ToString();
            searchLookUpEdit1.Properties.DisplayMember = "NgayNhapKho";
            searchLookUpEdit1.Properties.ValueMember = "NgayNhapKho";
            searchLookUpEdit1.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit1.EditValue = tbl.Rows[0]["NgayNhapKho"].ToString();

        }
        DataTable saveTable = new DataTable();
        private void CreateTable()
        {
            saveTable.Columns.Add("MaPhieu", typeof(int));
            saveTable.Columns.Add("SoTo", typeof(int));
            saveTable.Columns.Add("NgayLap", typeof(DateTime));
            saveTable.Columns.Add("MaHang", typeof(string));
            saveTable.Columns.Add("PO", typeof(string));
            saveTable.Columns.Add("KhachHang", typeof(string));
            saveTable.Columns.Add("MaDVSX", typeof(string));
            saveTable.Columns.Add("SLThung", typeof(int));
            saveTable.Columns.Add("SLSP", typeof(int));
            saveTable.Columns.Add("SoNo", typeof(string));
            saveTable.Columns.Add("MaDH", typeof(string));
            saveTable.Columns.Add("DotSX", typeof(string));
            saveTable.Columns.Add("NgayNhapKho", typeof(DateTime));
            saveTable.Columns.Add("GhiChu", typeof(string));

        }
        private void BtnLuu()
        {
            this.ActiveControl = grcPhieuNhapKho;
            if (textEdit2.EditValue == null || textEdit2.EditValue == "")
            {
                MessageBox.Show("Vui lòng chỉ nhập số Tờ", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable tbl = grcPhieuNhapKho.DataSource as DataTable;
            foreach (DataRow item in tbl.Rows)
            {
                DataRow newRow = saveTable.NewRow();
                newRow["MaPhieu"] = _soPhieu;
                newRow["SoTo"] = textEdit2.EditValue;
                newRow["NgayLap"] = dateEdit1.EditValue;
                newRow["MaHang"] = item["MaHang"].ToString();
                newRow["PO"] = item["POID"].ToString();
                newRow["MaDVSX"] = item["MaDVSX"].ToString();
                newRow["SLThung"] = Convert.ToInt32(item["SLThung"]);
                newRow["SLSP"] = Convert.ToInt32(item["SLSP"]);
                newRow["DotSX"] = item["DotSX"].ToString();
                //newRow["SoNo"] = item["SoNo"].ToString();
                newRow["MaDH"] = item["MaDH"].ToString();
                newRow["KhachHang"] = item["MaKH"].ToString();
                newRow["NgayNhapKho"] = DateTime.ParseExact(searchLookUpEdit1.EditValue.ToString(), "dd/MM/yyyy", null);
                newRow["GhiChu"] = textEdit3.EditValue;
                saveTable.Rows.Add(newRow);
            }
            string url = string.Format("{0}", URL + $"ThongKeDongThung/Post");
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, saveTable); }).Result;
            if (json.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                this.Close();
            }
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnLuu();
        }
        DataTable tbl = new DataTable();

        private void LoadDS()
        {
            string url = string.Format("{0}", URL + $"ThongKeDongThung/Get?Action=GetCreatePhieuNhapKho&Para1=A&Para2=A&Para3=A&Para4=A&Para5=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0) return;
            List<string> lstNgay = tbl.AsEnumerable().OrderByDescending(x => x["NgayNhapKho"].ToString()).Select(x => x["NgayNhapKho"].ToString()).Distinct().ToList();
            DataTable dtNgay = new DataTable();
            dtNgay.Columns.Add("NgayNhapKho", typeof(string));
            foreach (var item in lstNgay)
            {
                DataRow newRow = dtNgay.NewRow();
                newRow["NgayNhapKho"] = item;
                dtNgay.Rows.Add(newRow);
            }
            searchLookUpEdit1.Properties.DataSource = dtNgay;
            searchLookUpEdit1.Properties.DisplayMember = "NgayNhapKho";
            searchLookUpEdit1.Properties.ValueMember = "NgayNhapKho";
            searchLookUpEdit1.Properties.NullText = "[Chọn giá trị]";

            searchLookUpEdit1.EditValue = dtNgay.Rows[0]["NgayNhapKho"].ToString();
            LoadTable();
        }
        private void LoadTable()
        {
            DataTable dttbl = tbl.AsEnumerable().Where(x => x["NgayNhapKho"].ToString() == searchLookUpEdit1.EditValue.ToString()).CopyToDataTable();
            grcPhieuNhapKho.DataSource = dttbl;
        }
        private void LoadMaPhieu()
        {
            string url = string.Format("{0}", URL + $"ThongKeDongThung/Get?Action=GetMaPhieu&Para1=A&Para2=A&Para3=A&Para4=A&Para5=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (!_ischeckEdit)
                _soPhieu = Convert.ToInt32(tbl.Rows[0]["MaPhieu"]) + 1;
            else
            {
                searchLookUpEdit1.Properties.ReadOnly = true;
                textEdit2.EditValue = _soTo;
                dateEdit1.EditValue = DateTime.ParseExact(_ngay, "dd/MM/yyyy", null);
                
            }
            textEdit1.EditValue = _soPhieu;
        }

        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
           LoadTable();
        }

        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Nếu ký tự không phải là số và không phải là ký tự điều khiển (ví dụ như backspace), hủy bỏ ký tự đó
                e.Handled = true;
            }
        }
    }
}
