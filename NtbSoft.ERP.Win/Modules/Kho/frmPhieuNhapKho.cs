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
   
    public partial class frmPhieuNhapKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty,_tuNgay = string.Empty,_denNgay= string.Empty,_ngaylap = string.Empty,_ngaynhap = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, isCheckFirst = false;
        private HttpClientExtension _clientExtension;
        KeyDownControlHandler keyDownControlHandler;
        int _soPhieu = 0,focusRow = 0,_soNo = 0;
        DataTable tbl = new DataTable();
        public frmPhieuNhapKho()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
      
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            tuNgay.EditValue = DateTime.Now;
            denNgay.EditValue = DateTime.Now;
            CheckPerminsion();
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtnEdit, _allowEdit, ActionType.Edit);
            AddActionControl(_lstActionControl, BtnRefresh, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, BtnAdd, _allowAdd, ActionType.LapKH);
            AddActionControl(_lstActionControl, BtnDelete, _allowDelete, ActionType.Delete);

            return _lstActionControl;
        }
        private void BtnAdd()
        {
            frmCreatePhieuNhapKho frm = new frmCreatePhieuNhapKho(_soPhieu, _soNo, _ngaylap,_ngaynhap);
            frm.ShowDialog();
            LoadDS();
            LoadData();
        }
        private void btnAddNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnAdd();
           
        }
        private void BtnRefresh()
        {

        }
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnRefresh();
        }
        private void BtnEdit()
        {
            if(tbl.Rows.Count != 0)
            {
                frmCreatePhieuNhapKho frm = new frmCreatePhieuNhapKho(_soPhieu, _soNo, _ngaylap, _ngaynhap, true);
                frm.ShowDialog();
                LoadDS();
                LoadData();
            }
            else
                MessageBox.Show("Chưa có phiếu nhập kho", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        private void btnAddorEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnEdit();
        }

        private void dateEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            _tuNgay = tuNgay.EditValue.ToString();
            if (isCheckFirst)
                LoadDS();
            isCheckFirst = true;
        }

        private void dateEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            _denNgay = denNgay.EditValue.ToString();
            if (isCheckFirst)
                LoadDS();
            isCheckFirst = true;
        }

        private void BtnDelete()
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa phiếu này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string url = $"{URL}ThongKeDongThung/Delete?Para1={_soPhieu}";
                string delete = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (delete.ToLower() == "true")
                {
                    MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDS();
                    // Tạo một đối tượng Timer với thời gian chờ là 2 giây
                    Timer timer = new Timer();
                    timer.Interval = 2000; // 2 giây
                    timer.Tick += (s, e) =>
                    {
                        // Khi hết thời gian chờ, dừng timer và ẩn thông báo
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                }
               
            }

        }

        private void grvSoPhieu_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            focusRow = grvSoPhieu.FocusedRowHandle;
            if (focusRow > -1)
            {
                _soPhieu = Convert.ToInt32(grvSoPhieu.GetRowCellValue(focusRow, colSoPhieu));
                _soNo = Convert.ToInt32(grvSoPhieu.GetRowCellValue(focusRow, colSoTo));
                _ngaylap = grvSoPhieu.GetRowCellValue(focusRow, colNgayLap).ToString();
                _ngaynhap = grvSoPhieu.GetRowCellValue(focusRow, colNgayNhap).ToString();
                LoadChiTietPhieu();
            }
        }
        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnDelete();
        }

        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);  
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserModuleEntity> List = JsonConvert.DeserializeObject<List<SystemUserModuleEntity>>(json);
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                //btnLapKeHoach.Enabled = false;
            }
            if (!_allowEdit)
            {
                btnAddorEdit.Enabled = false;
                btSave.Enabled = false;
            }
            if (!_allowDelete)
                btnDelete.Enabled = false;
        }
       
        private void LoadDS()
        {
            string url = string.Format("{0}", URL + $"ThongKeDongThung/Get?Action=GetPhieuNhapKho&Para1={_tuNgay}&Para2={_denNgay}&Para3=A&Para4=A&Para5=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0)
            {
                grcSoPhieu.DataSource = null;
                grcChiTietSoPhieu.DataSource = null;
                return;
            }
            var objectNhapkho = tbl.AsEnumerable().Select(x => new 
            {
                MaPhieu = Convert.ToInt32(x["MaPhieu"]),
                SoTo = Convert.ToInt32(x["SoTo"]),
                NgayLapPhieu = x["NgayLapPhieu"].ToString(),
                NgayNhapKho = x["NgayNhapKho"].ToString()
            }).Distinct().ToList();
            List<PhieuNhapkho> lstNhapKho = objectNhapkho.Select(x => new PhieuNhapkho
            {
                MaPhieu = x.MaPhieu,
                SoTo = x.SoTo,
                NgayLapPhieu = x.NgayLapPhieu,
                NgayNhapKho = x.NgayNhapKho,
            }).ToList();
            _soPhieu = Convert.ToInt32(tbl.Rows[0]["MaPhieu"]);
            grcSoPhieu.DataSource = lstNhapKho;
           
            LoadChiTietPhieu();
        }
        private void LoadChiTietPhieu()
        {
            if (tbl.Rows.Count == 0) {
                grcSoPhieu.DataSource = null;
                grcChiTietSoPhieu.DataSource = null;
                return;
            }
            
            DataTable dtTable = tbl.AsEnumerable().Where(x => Convert.ToInt32(x["MaPhieu"]) == Convert.ToInt32(_soPhieu)).CopyToDataTable();
            grcChiTietSoPhieu.DataSource = dtTable;
        }
    }
    public class PhieuNhapkho
    {
        public int MaPhieu { get; set; }
        public int SoTo { get; set; }
        public string NgayLapPhieu { get; set; }
        public string NgayNhapKho { get; set; }
    }
}