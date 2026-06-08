using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.XuLyVT;
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

namespace NtbSoft.ERP.Win.Modules.MuaHang
{
    public partial class frmTaoPhieuXLVT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private XtraUserControl currentUC;
        private DataRow currentRow;
        private bool isLoad;        
        private string newMaPXLVT;
        private string srcID;
        private string srcName;

        private DataTable fromGrpMuaHangTable;
        private Dictionary<string, object> fromGrpMuaHangData;

        public frmTaoPhieuXLVT(DataRow row = null, DataRow another = null)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            currentRow = row;
            isLoad = row != null;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SplashScreenManager.ShowForm(this, typeof(frmLoading), true, true, false);
            try
            {
                loadBase();             
                loadIndentNeedsOption();
                loadPanelControl1();
                if (isLoad)
                {
                    saveBtn.Visibility = BarItemVisibility.Never;
                    loadIndentToUpdate();
                }
                else
                {
                    changeBtn.Visibility = BarItemVisibility.Never;
                }                
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }
        private void loadBase()
        {
            //TGHTDuKien.Properties.DisplayFormat.FormatString = "yyyy-MM-dd";
            this.WindowState = FormWindowState.Maximized;            
        }
        private void loadPanelControl1()
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 1;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            LabelControl lblDefault = new LabelControl();
            lblDefault.Text = "Chưa chọn nhu cầu...";
            lblDefault.Appearance.Font = new Font("Tahoma", 14, FontStyle.Bold);
            lblDefault.Appearance.ForeColor = Color.Gray;
            lblDefault.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblDefault.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            lblDefault.Appearance.Options.UseTextOptions = true;
            lblDefault.Dock = DockStyle.Fill;
            lblDefault.AutoSizeMode = LabelAutoSizeMode.None;

            layout.Controls.Add(lblDefault, 0, 0);
            panelControl1.Controls.Clear();
            panelControl1.Controls.Add(layout);
        }
        private void loadIndentToUpdate()
        {
            indentName.Text = currentRow["Ten"]?.ToString() ?? "";           
            //indentQty.EditValue = XuLyVTUnits.SmartTryParse<float>(currentRow["SoLuong"]);
            TGHTDuKien.EditValue = currentRow["TGHTDuKien"] == DBNull.Value ? null : (DateTime?)currentRow["TGHTDuKien"];
            Ghichu.Text = currentRow["GhiChu"]?.ToString() ?? "";

            indentNeedsOption.EditValue = currentRow["NhuCau"] == DBNull.Value ? null : currentRow["NhuCau"].ToString();
            indentNeedsOption_SelectedIndexChanged(indentNeedsOption, EventArgs.Empty);
        }
        private void loadIndentNeedsOption()
        {
            indentNeedsOption.Properties.Items.AddRange(new string[] { "Mua hàng" });//, "Gia công", "Luân chuyển kho" });
        }
        private void indentNeedsOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelControl1.Controls.Clear();
            switch (indentNeedsOption.SelectedIndex)
            {
                case -1:
                    break;
                case 0: // Mua hàng
                    currentUC = new grpMuaHang(currentRow);
                    if(!isLoad) indentName.EditValue = loadIndentName("Phiếu");
                    break;

                case 1: // Gia công
                    currentUC = new grpGiaCong(currentRow);
                    if (!isLoad) indentName.EditValue = loadIndentName("Phiếu gia công");
                    break;

                case 2: // Luân chuyển
                    currentUC = new grpLuanChuyen(currentRow);
                    if (!isLoad) indentName.EditValue = loadIndentName("Phiếu luân chuyển");
                    //((ucLuanChuyen)uc).LoadData(MockDataLuanChuyen());
                    break;
            }

            if (currentUC != null)
            {
                currentUC.Dock = DockStyle.Fill;
                panelControl1.Controls.Add(currentUC);
            }
        }
        private string loadIndentName(string indentType)
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getindentid";
            request.Parameter = indentType + ' ';
            string urlGetListDataTable = URL + "XLVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            string indentName = string.Empty;
            if (dt.Columns.Count == 0 || dt.Rows.Count == 0) return indentType + " 1";
            indentName = dt.Rows[0]["TenPXLVT"].ToString() ?? "";
            int stt = XuLyVTUnits.SmartTryParse<int>(indentName.Split(' ')[1]) + 1;
            return indentType + ' ' + stt;
        }
        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void saveBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (currentUC == null)
            {
                XtraMessageBox.Show("Chưa có loại hàng");
                return;
            }
            getFromGrpMuaHang();
            if (fromGrpMuaHangData == null)
            {
                XtraMessageBox.Show("Thiếu dữ liệu");
                return;
            }
            foreach(KeyValuePair<string, object> entry in fromGrpMuaHangData)
            {
                if (entry.Value == null || entry.Value.ToString() == "")
                {
                    XtraMessageBox.Show("Thiếu dữ liệu");
                    return;
                }
            }
            if (fromGrpMuaHangTable == null || fromGrpMuaHangTable.Columns.Count == 0 || fromGrpMuaHangTable.Rows.Count == 0)
            {
                XtraMessageBox.Show("Thiếu dữ liệu vật tư chi tiết");
                return;
            }

            srcID = fromGrpMuaHangData["MaKH"].ToString() + "||" + fromGrpMuaHangData["MaHang"] + "||" + fromGrpMuaHangData["MaDH"] + "||" + fromGrpMuaHangData["MaDot"];
            srcName = fromGrpMuaHangData["TenKH"].ToString() + "||" + fromGrpMuaHangData["TenHang"] + "||" + fromGrpMuaHangData["TenDH"] + "||" + fromGrpMuaHangData["Dot"];

            DialogResult result = XtraMessageBox.Show(
                "Bạn có chắc chắn muốn lưu dữ liệu này không?",
                "Xác nhận lưu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes)
                return;
            try
            {
                saveToXLVT();
                saveToXLVTVT();
                this.DialogResult = DialogResult.OK;
                this.Close();
                XtraMessageBox.Show("Lưu phiếu xử lý vật tư thành công!");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi thay đổi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void saveToXLVT()
        {
            newMaPXLVT = XuLyVTUnits.generatedTimeKey("PXLVT");

            var request = XuLyVTRequestPost.createDefault("XLVT");
            request.Action = "create";
            DataRow row = request.TypeTable.NewRow();

            row["TrangThai"] = "Đang xử lý";
            row["MaPXLVT"] = newMaPXLVT;
            row["TenPXLVT"] = indentName.EditValue ?? DBNull.Value;
            row["LoaiXLVT"] = fromGrpMuaHangData["LoaiHang"];
            row["MaNguon"] = srcID;
            row["TenNguon"] = srcName;
            row["NhuCau"] = indentNeedsOption.EditValue ?? DBNull.Value;
            row["SLVT"] = fromGrpMuaHangTable.Rows.Count;
            row["SoDonMuaHang"] = 0;
            row["SoDonGiaCong"] = 0;
            row["ChiPhiTong"] = 0;
            row["TGHTDuKien"] = TGHTDuKien.EditValue ?? DBNull.Value;
            row["Ghichu"] = Ghichu.EditValue ?? DBNull.Value;

            row["NgayTao"] = DateTime.Now;

            request.TypeTable.Rows.Add(row);
            string urlGetListDataTable = URL + "XLVT/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private void saveToXLVTVT()
        {
            var request = XuLyVTRequestPost.createDefault("XLVTVT");
            request.Action = "create";

            foreach (DataRow gridRow in fromGrpMuaHangTable.Rows)
            {
                DataRow reqRow = request.TypeTable.NewRow();

                float lossPercent = XuLyVTUnits.SmartTryParse<float>(gridRow["DinhMucHaoHut"]) / 100;
                float customerQuotar = XuLyVTUnits.SmartTryParse<float>(gridRow["DinhMucChung"]);
                float soLuongItem = XuLyVTUnits.SmartTryParse<float>(gridRow["SoLuong"]);

                float haveCustomerQuotar = lossPercent + customerQuotar == 0 ? 1 : (lossPercent + 1) * customerQuotar;
                
                reqRow["TrangThai"] = "Đang xử lý";
                reqRow["MaPVT"] = XuLyVTUnits.generatedTimeKey("PVT");               
                reqRow["MaPXLVT"] = newMaPXLVT;
                reqRow["TenPXLVT"] = indentName.EditValue ?? DBNull.Value;
                reqRow["MaVT"] = gridRow["MaVTID"] ?? DBNull.Value;
                reqRow["TenVT"] = gridRow["MaVT"] ?? DBNull.Value;
                reqRow["ChiTietVT"] = gridRow["ChiTiet"] ?? DBNull.Value;
                reqRow["LoaiVT"] = gridRow["LoaiVT"] ?? DBNull.Value;
                reqRow["InSeamMauVT"] = gridRow["DauSizeID"] ?? DBNull.Value;
                reqRow["MaMauVT"] = gridRow["MauVTID"] ?? DBNull.Value;
                reqRow["TenMauVT"] = gridRow["MauVT"] ?? DBNull.Value;
                reqRow["MaSizeVT"] = gridRow["KhoVaiID"] ?? DBNull.Value;
                reqRow["TenSizeVT"] = gridRow["KhoVai"] ?? DBNull.Value;
                reqRow["MaDVVT"] = gridRow["MaDVVT"] ?? DBNull.Value;
                reqRow["TenDVVT"] = gridRow["TenDVVT"] ?? DBNull.Value;
                reqRow["SLTong"] = soLuongItem * haveCustomerQuotar;
                reqRow["SLTonKhoSuDung"] = 0;
                reqRow["SoDonMuaHang"] = 0;
                reqRow["SLMuaHang"] = 0;
                reqRow["SLCanMuaConLai"] = 0;
                reqRow["TVGiaCong"] = gridRow["TVGiaCong"] ?? DBNull.Value;
                reqRow["SoDonGiaCong"] = 0;
                reqRow["NgayTao"] = DateTime.Now;
                request.TypeTable.Rows.Add(reqRow);
            }

            string urlGetListDataTable = URL + "XLVTVT/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private void getFromGrpMuaHang()
        {
            var getDataMethod = currentUC.GetType().GetMethod("shareData");
            fromGrpMuaHangData = getDataMethod?.Invoke(currentUC, null) as Dictionary<string, object>;
            var getVTTableMethod = currentUC.GetType().GetMethod("shareVTTable");
            fromGrpMuaHangTable = getVTTableMethod?.Invoke(currentUC, null) as DataTable;
        }
        
        private void naplaiBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            indentNeedsOption.SelectedIndex = -1;
            indentName.EditValue = null;
            TGHTDuKien.EditValue = null;
            Ghichu.EditValue = null;
            panelControl1.Controls.Clear();
        }
    }
}