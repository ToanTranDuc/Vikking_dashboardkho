using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmChonDauSize_Size : DevExpress.XtraEditors.XtraForm
    {
        SearchCheckSelection gridCheckMarks_Size;

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();

        DataTable tblBOM, tblVT, tblSave;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        private string _mahang = string.Empty, nhomsize = string.Empty, _mavt = string.Empty;
        bool IsCheckHT = true;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        string _nguoiTao = string.Empty;

        // Property lưu giá trị để gửi về form cha
        public string Data { get; private set; } = string.Empty;

        public frmChonDauSize_Size(bool Is_HienThi, string mahang)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this._mahang = mahang;
            this.IsCheckHT = Is_HienThi;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            _clientExtension = new HttpClientExtension();
            tblBOM = new DataTable();
            tblVT = new DataTable();
            tblSave = new DataTable();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            //_allowAdd = allowAdd;
            //_allowEdit = allowEdit;
            //_allowDelete = allowDelete;
            //this._nguoiTao = nguoitao;
        }

        private void BtLoad()
        {
            try
            {
                createSearchGridView();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        protected override void OnLoad(EventArgs e)
        {
            BtLoad();
            //CheckPerminsion();
            //keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }

        private void btn_XacNhan_Click(object sender, EventArgs e)
        {
            if (IsCheckHT)
            {
                if(searchLookUpEdit_InSeam.EditValue == null)
                {
                    return;
                }    
                // Lấy dữ liệu từ textbox hoặc các input khác trong form con
                Data = searchLookUpEdit_InSeam.Text.ToString();
            }
            else
            {
                if (searchLookUpEdit_Size.EditValue == null)
                {
                    return;
                }
                Data = searchLookUpEdit_Size.Text.ToString();
            }    
            // Đóng form con
            this.Close();
        }

        private void createSearchGridView()
        {
            if (IsCheckHT)
            {
                layoutControlItem4.Enabled = false;
                layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                string urlInSeam = string.Format("{0}?_mahang={1}", URL + "BOM/GET_InSeam", _mahang);
                string jsonInSeam = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlInSeam); }).Result;
                DataTable tblInSeam = JsonConvert.DeserializeObject<DataTable>(jsonInSeam);


                searchLookUpEdit_InSeam.Properties.DataSource = tblInSeam;
                searchLookUpEdit_InSeam.Properties.ValueMember = "MaNhomSize";
                searchLookUpEdit_InSeam.Properties.DisplayMember = "NhomSize";
            }
            else
            {
                layoutControlItem1.Enabled = false;
                layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                // Lấy dữ liệu từ API
                string urlSize = string.Format("{0}?_mahang={1}", URL + "BOM/GET_Size", _mahang);
                string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);

                searchLookUpEdit_Size.Properties.DataSource = tblSize;
                searchLookUpEdit_Size.Properties.ValueMember = "MaSize";
                searchLookUpEdit_Size.Properties.DisplayMember = "TenSize";

                searchLookUpEdit_Size.Properties.View.OptionsSelection.MultiSelect = true;
                searchLookUpEdit_Size.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEdit_Size_CustomDisplayText);
                searchLookUpEdit_Size.Properties.PopulateViewColumns();
                //searchLookUpEditPO.Properties.ShowAddNewButton = true;
                //searchLookUpEditPO.AddNewValue += searchLookUpEditPO_AddNewValue;

                gridCheckMarks_Size = new SearchCheckSelection(searchLookUpEdit_Size.Properties);
                gridCheckMarks_Size.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit_Size_SelectionChanged);
                searchLookUpEdit_Size.Properties.Tag = gridCheckMarks_Size;
            }

            //----------2


            //----------3


        }
        private void searchLookUpEdit_Size_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Size.EditValue is DataRowView)
            {
                gridCheckMarks_Size.Selection.Add(((DataRowView)searchLookUpEdit_Size.EditValue));
            }
            else
            {
                searchLookUpEdit_Size.Properties.Appearance.ForeColor = Color.DarkBlue;
            }

            StringBuilder sb = new StringBuilder();
            foreach (DataRowView rv in gridCheckMarks_Size.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append(";"); }
                sb.Append(rv["MaSize"].ToString());
            }
            searchLookUpEdit_Size.Text = sb.ToString();
        }

        void searchLookUpEdit_Size_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append("; "); }
                sb.Append(rv["TenSize"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chưa chọn Màu----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
            //e.DisplayText = sb.ToString();
        }
        void searchLookUpEdit_Size_SelectionChanged(object sender, EventArgs e)
        {
            Control c = this.ActiveControl;
            //Control c = FindSearchLookUpEditInLayout(this.ActiveControl);

            if (c is DevExpress.XtraLayout.LayoutControl)
            {
                if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
                {
                    c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
                }
            }
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["MaSize"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
            }
        }
        
        private Control FindSearchLookUpEditInLayout(Control control)
        {
            // Kiểm tra null
            if (control == null)
                return null;

            // Nếu control hiện tại là SearchLookUpEdit, trả về nó
            if (control is DevExpress.XtraEditors.SearchLookUpEdit)
                return control;

            // Nếu control là LayoutControl, lặp qua các item của nó
            if (control is DevExpress.XtraLayout.LayoutControl layoutControl)
            {
                foreach (var item in layoutControl.Items)
                {
                    if (item is DevExpress.XtraLayout.LayoutControlItem layoutItem && layoutItem.Control != null)
                    {
                        // Tìm SearchLookUpEdit trong LayoutControlItem
                        var result = FindSearchLookUpEditInLayout(layoutItem.Control);
                        if (result != null)
                            return result;
                    }
                }
            }

            // Nếu control có ActiveControl, tiếp tục tìm
            if (control is ContainerControl containerControl && containerControl.ActiveControl != null)
            {
                return FindSearchLookUpEditInLayout(containerControl.ActiveControl);
            }

            // Không tìm thấy
            return null;
        }
    }
}
