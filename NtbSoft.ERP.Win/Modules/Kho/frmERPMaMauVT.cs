using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
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
    public partial class frmERPMaMauVT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        List<int> lstRowUpdate = new List<int>();
        List<ERPMaMauVTEntity> lstMaMauVTEntity;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;

     
        Helper helper = new Helper();

        
        public frmERPMaMauVT()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstMaMauVTEntity = new List<ERPMaMauVTEntity>();
         
        }

       

        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookup();
            CreateSearchLookUpEditLocKH();
            LoadDSMaMauVT(false);
        }

        private void LoadDSMaMauVT(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "ERPThuVienVT/GetAllMMVT");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstMaMauVTEntity = JsonConvert.DeserializeObject<List<ERPMaMauVTEntity>>(json);
                }
                gridMauVT.DataSource = lstMaMauVTEntity.OrderBy(x => x.MaHang).ToList();

               
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewMauVT.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridMauVT;
                }
                gridViewMauVT.OptionsBehavior.Editable = true;
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ThemDong()
        {
            if(searchLookUpEditLocKH.EditValue==null|| searchLookUpEditLocKH.EditValue.ToString()=="")
            {
                XtraMessageBox.Show(" Vui lòng chọn khách hàng!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (searchLookUpEditMH.EditValue == null)
            {
                XtraMessageBox.Show(" Vui lòng chọn mã hàng!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMaMauVT.Text) && string.IsNullOrWhiteSpace(txtMauVT.Text)) return;
            string[] arrMaMauVT = txtMaMauVT.Text.Trim().Split('|');
            string[] arrMau = txtMauVT.Text.Trim().Split('|');
          
            if (txtMaMauVT.Text == "" || txtMauVT.Text == "")
            {
                XtraMessageBox.Show(" Vui lòng điền đủ thông tin vật tư!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (arrMaMauVT.Length != arrMau.Length)
            {
                XtraMessageBox.Show(" Vui lòng điền đủ thông tin vật tư!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (lstMaMauVTEntity == null)
            {
                lstMaMauVTEntity = new List<ERPMaMauVTEntity>();
            }
            for(int i=0;i<arrMaMauVT.Length;i++)
            {
                ERPMaMauVTEntity obj = new ERPMaMauVTEntity();
                obj.ID = 0;
                obj.isNew = true;
                obj.MaKH = searchLookUpEditLocKH.EditValue.ToString();
                obj.MaHang=searchLookUpEditMH.EditValue.ToString();
                obj.MaMauVT = arrMaMauVT[i];
                obj.MauVT = arrMau[i];
                lstMaMauVTEntity.Insert(0, obj);
            }    
           
            gridViewMauVT.FocusedRowHandle = 0;
            gridMauVT.DataSource = lstMaMauVTEntity;
            gridMauVT.RefreshDataSource();
            gridViewMauVT.MakeRowVisible(0, true);
            gridViewMauVT.OptionsBehavior.Editable = true;
            txtMaMauVT.Text = "";
            txtMauVT.Text = "";
        }



        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
        }



        private void NapLaiDong()
        {
            CreateSearchLookup();
            LoadDSMaMauVT(false);
            //_status = ResourceURL.EventStatus.View;
            //GridViewUpdateStatus(_status);
            //gridViewMauVT.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }

        private void gridViewMauVT_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }

        private void gridViewMauVT_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }



        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.Button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewMauVT.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewMauVT.FocusedRowHandle;
                }
                List<ERPMaMauVTEntity> _lstUpdate= gridMauVT.DataSource as List<ERPMaMauVTEntity>;
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0) || !Validatemamau(_lstUpdate))
                {
                    //Validatemamau(_lstUpdate);
                    return;
                }
                string url = string.Format("{0}?", URL + "ERPThuVienVT/PostMMVT");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
                if (msResult.ToLower() == "true")
                {
                    LoadDSMaMauVT(false);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                
                gridViewMauVT.OptionsBehavior.Editable = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gridViewMauVT.FocusedRowHandle >= 0)
                    {
                        int ID = Int32.Parse(gridViewMauVT.GetFocusedRowCellValue(colID)?.ToString());
                        string url = URL + $"ERPThuVienVT/DeleteMMVT?id={ID}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                            LoadDSMaMauVT(false);
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void Naplai_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private bool Validatemamau(List<ERPMaMauVTEntity> lstmamau)
        {
            // Danh sách chứa các thông báo lỗi
            List<string> errorMessages = new List<string>();

            foreach (var row in lstmamau)
            {
                // Kiểm tra nếu giá trị bị trống
                if (string.IsNullOrEmpty(row.MaMauVT))
                {
                    errorMessages.Add($"Mã màu vật tư không được để trống!");
                }
                if (string.IsNullOrEmpty(row.MaKH))
                {
                    errorMessages.Add($"Khách hàng không được để trống!");
                }
                if (string.IsNullOrEmpty(row.MaHang))
                {
                    errorMessages.Add($"Mã hàng không được để trống!");
                }

                // Kiểm tra trùng lặp (phải có đủ 3 giá trị để kiểm tra)
                if (!string.IsNullOrEmpty(row.MaMauVT) && !string.IsNullOrEmpty(row.MaKH) && !string.IsNullOrEmpty(row.MaHang))
                {
                    int duplicateCount = lstmamau.Count(mm =>
                        mm.MaMauVT == row.MaMauVT &&
                        mm.MaKH == row.MaKH &&
                        mm.MaHang == row.MaHang
                    );

                    if (duplicateCount > 1) // Nếu xuất hiện hơn 1 lần thì bị trùng
                    {
                        errorMessages.Add($"Mã màu '{row.MaMauVT}' đã bị trùng'!");
                        break; // Dừng kiểm tra sau khi phát hiện lỗi trùng lặp
                    }
                }
            }

            // Nếu có lỗi, hiển thị toàn bộ thông báo lỗi và trả về false
            if (errorMessages.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errorMessages), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private void gridViewMauVT_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
           
        }
        private void CreateSearchLookUpEditLocKH()
        {
            searchLookUpEditLocKH.Properties.ValueMember = "MaKH";
            searchLookUpEditLocKH.Properties.DisplayMember = "TenKH";

          

            string url = $"{URL}KhaiBaoAll/Get?action=GetKHBangSize";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

          

            searchLookUpEditLocKH.Properties.DataSource = tbl;
         
        }
        private void CreateSearchLookUpEditLocMH()
        {
            searchLookUpEditLocMH.Properties.DataSource = null;
            searchLookUpEditLocMH.EditValue = null;
            searchLookUpEditLocMH.Properties.ValueMember = "MaHang";
            searchLookUpEditLocMH.Properties.DisplayMember = "MaHang";
            

            string khachhang = searchLookUpEditLocKH.EditValue?.ToString() ?? "All";
            string url2 = $"{URL}KhaiBaoAll/Get?action=GetHangHoaBangMau&para1={khachhang}";
            string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
            DataTable tbl2 = JsonConvert.DeserializeObject<DataTable>(json2);

            searchLookUpEditLocMH.Properties.DataSource = tbl2;
            //searchLookUpEditMH.Properties.DataSource = tbl2;
        }

        private void searchLookUpEditLocKH_Properties_EditValueChanged(object sender, EventArgs e)
        {
            gridViewMauVT.ActiveFilterString = string.Empty;
            ApplyFilter();
            CreateSearchLookUpEditLocMH();
            CreateSearchLookUpEditMH();
        }

        private void CreateSearchLookUpEditMH()
        {
          
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "MaHang";

            string khachhang = searchLookUpEditLocKH.EditValue?.ToString() ?? "All";
            
            string url2 = $"{URL}ERPThuVienVT/GetMH?&makh={khachhang}";
            string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
            DataTable tbl2 = JsonConvert.DeserializeObject<DataTable>(json2);

          
            searchLookUpEditMH.Properties.DataSource = tbl2;
        }
        private void searchLookUpEditLocMH_Properties_EditValueChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }
        private void ApplyFilter()
        {
            // Lấy giá trị từ SearchLookUpEdit
            string selectedKH = searchLookUpEditLocKH.EditValue?.ToString();
            string selectedMH = searchLookUpEditLocMH.EditValue?.ToString();

            // Nếu giá trị là "All", thì bỏ qua bộ lọc cho giá trị đó
            string filterKH = (!string.IsNullOrEmpty(selectedKH) && selectedKH != "All") ? $"[MaKH] = '{selectedKH}'" : "";
            string filterMH = (!string.IsNullOrEmpty(selectedMH) && selectedMH != "All") ? $"[MaHang] = '{selectedMH}'" : "";

            // Kết hợp cả 2 điều kiện nếu đều có giá trị hợp lệ
            if (!string.IsNullOrEmpty(filterKH) && !string.IsNullOrEmpty(filterMH))
            {
                gridViewMauVT.ActiveFilterString = $"{filterKH} AND {filterMH}";
            }
            else if (!string.IsNullOrEmpty(filterKH))
            {
                gridViewMauVT.ActiveFilterString = filterKH;
            }
            else if (!string.IsNullOrEmpty(filterMH))
            {
                gridViewMauVT.ActiveFilterString = filterMH;
            }
            else
            {
                gridViewMauVT.ActiveFilterString = string.Empty; // Xóa bộ lọc nếu không có điều kiện nào
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ThemDong();
        }

        private void CreateSearchLookup()
        {
            string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenHang";
            rCountryEdit.ValueMember = "MaHang";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKH", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Khách hàng", Name = "colTenKH", Visible = true });

            }
            colMaHang.ColumnEdit = rCountryEdit;
            rCountryEdit.EditValueChanged += RCountryEdit_EditValueChanged;

            

            string urlkh = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkh); }).Result;
            DataTable tblkh = JsonConvert.DeserializeObject<DataTable>(jsonkh);
            RepositoryItemSearchLookUpEdit rCountryEditkh = new RepositoryItemSearchLookUpEdit();
            rCountryEditkh.DataSource = tblkh;
            rCountryEditkh.DisplayMember = "TenKH";
            rCountryEditkh.ValueMember = "MaKH";
            rCountryEditkh.ShowClearButton = false;
            rCountryEditkh.NullText = "[Chọn giá trị]";

            GridView dvViewkh = rCountryEditkh.View;
            if (dvViewkh.Columns.Count == 0)
            {
                dvViewkh.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewkh.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewkh.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewkh.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewkh.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Tên khách hàng", Name = "colMaHang", Visible = true });

            }
            colMaKH.ColumnEdit = rCountryEditkh;
        }

        private void RCountryEdit_EditValueChanged(object sender, EventArgs e)
        {
            var rCountryEdit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (rCountryEdit == null) return;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)rCountryEdit.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridViews = (DevExpress.XtraGrid.Views.Grid.GridView)gridMauVT.MainView;
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "MaKH", focusedRow["MaKH"].ToString());
                }
            }
        }
    }
}