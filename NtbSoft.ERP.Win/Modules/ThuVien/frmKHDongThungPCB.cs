using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
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

    public partial class frmKHDongThungPCB : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        List<String> listSize = new List<string>();
        DataTable _dtKHDongThung = new DataTable();
        RepositoryItemSearchLookUpEdit searchLookupEdit = new RepositoryItemSearchLookUpEdit();
        GridView view;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;

        KeyDownControlHandler keyDownControlHandler;

        //ActionControl actionControlAdd;     
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        public frmKHDongThungPCB()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        private List<ActionControl> InitActionKeyDown()
        {
           
           
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, true);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, true);

            lstActionControls = new List<ActionControl> {
                actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDataMaHang();
            GetDSQC_DongThungAsync();
        }
        private async void LoadDataMaHang()
        {
            try
            {
                //showSplashScreenForm();
                string url = URL + $"ErpKHDongThung_PCB/Get?action=GetMaHang&styleID=Para";
                //DataTable _listDataMaHang = await _serviceERPKHDongThungPCB.GetDT(url);
                //gridViewMaHang.DataSource = _listDataMaHang;
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dtMaHang = JsonConvert.DeserializeObject<DataTable>(json);
                dgrMaHang.DataSource = dtMaHang;
                //hideSplashScreenForm();
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private async Task GetDSQC_DongThungAsync()
        {
            try
            {
                string url = URL + $"DicQCDongThung/Get";
                //DataTable _listDataMaHang = await _serviceERPKHDongThungPCB.GetDT(url);
                //gridViewMaHang.DataSource = _listDataMaHang;
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                BindingList<DicQCDongThungEntity> bindingList = JsonConvert.DeserializeObject<BindingList<DicQCDongThungEntity>>(json);
                searchLookupEdit.DataSource = bindingList;
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowEdit)
            {
                btSave.Enabled = false;
                bandedGridView1.OptionsBehavior.Editable = false;
                barEditItem1.Enabled = false;
            }
        }

        private void GetKHDongTHungPCB(string styleID)
        {
            try
            {
                string url = URL + $"ErpKHDongThung_PCB/Get?action=GetKHDongThungPCB&styleID={styleID}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _dtKHDongThung = JsonConvert.DeserializeObject<DataTable>(json);
                this.bandedGridView1.OptionsView.ShowColumnHeaders = false;
                createColBandsSize();
                dgrKHDongThung.DataSource = _dtKHDongThung;
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
        }
        private DataTable UnPivot()
        {
            DataTable tblPivot = dgrKHDongThung.DataSource as DataTable;
            DataTable tblSave = createTableKHDT();
            if (tblPivot == null || tblPivot.Rows.Count == 0) return null;
            DataTable dataConvertPivot = new DataTable();
            foreach (DataRow dataRows in tblPivot.Rows)
            {
                for (int i = 4; i < tblPivot.Columns.Count; i++)
                {
                    Console.WriteLine("dataRows");
                    //tblSave[""]
                    DataRow dataRowSave = tblSave.NewRow();
                    dataRowSave["ID"] = 0;
                    dataRowSave["StyleID"] = dataRows["StyleID"];
                    dataRowSave["MaHang"] = dataRows["MaHang"].ToString();
                    dataRowSave["Ma_QC_DongThung"] = dataRows["Ma_QC_DongThung"].ToString();
                    dataRowSave["GhiChu"] = dataRows["GhiChu"].ToString();
                    dataRowSave["SizeID"] = listSize[i - 4].Split('@')[0];
                    dataRowSave["Size"] = listSize[i - 4].Split('@')[1];
                    dataRowSave["SL_PCS_Thung"] = dataRows[tblPivot.Columns[i].ColumnName];
                    tblSave.Rows.Add(dataRowSave);
                }
            }
            return tblSave;
        }
        private DataTable createTableKHDT()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("StyleID", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SL_PCS_Thung", typeof(int));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("Ma_QC_DongThung", typeof(string));
            return tbl;
        }
        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private void LuuDong()
        {
            try
            {
                this.ActiveControl = dgrMaHang;
                string url = URL + $"ErpKHDongThung_PCB/Post";
                DataTable dtsave = UnPivot();
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtsave); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
            catch(Exception ex)
            {
                XtraMessageBox.Show(" Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         
        }
        private void gridViewMaHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            var styleID = gridViewMaHang.GetFocusedRowCellValue("StyleID").ToString();
            GetKHDongTHungPCB(styleID);
        }
        private void createColBandsSize()
        {
            try
            {
                if (listSize != null && listSize.Count > 0)
                {
                    listSize.Clear();
                }
                gridBandSize.Children.Clear();
                bandedGridView1.Columns.Clear();
                foreach (DataColumn dc in _dtKHDongThung.Columns)
                {
                    if (dc.ColumnName.Contains("Size@"))
                    {
                        string colName = dc.ColumnName;
                        listSize.Add(colName.Replace("Size@", ""));
                        //colName.Replace("@Size", "");
                        BandedGridColumn col = new BandedGridColumn();
                        col.AppearanceCell.Options.UseTextOptions = true;
                        col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.AppearanceHeader.Options.UseTextOptions = true;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.Caption = colName.Split('@')[2]; ;
                        col.FieldName = dc.ColumnName;
                        col.Name = "col" + colName;
                        col.OptionsColumn.AllowEdit = true;
                        col.Visible = true;
                        col.Width = 50;
                        bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(223)))), ((int)(((byte)(253)))));
                        gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                        gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                        gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        gb.AppearanceHeader.Options.UseBackColor = true;
                        gb.AppearanceHeader.Options.UseFont = true;
                        gb.AppearanceHeader.Options.UseForeColor = true;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gb.Caption = colName.Split('@')[2]; ;
                        gb.Columns.Add(col);
                        gb.Name = "gb" + "Size_" + col;
                        gb.VisibleIndex = 0;
                        gb.Width = 60;
                        gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "tenmau")
                    {
                        BandedGridColumn colColorID = new BandedGridColumn();
                        colColorID.AppearanceCell.Options.UseTextOptions = true;
                        colColorID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colColorID.AppearanceHeader.Options.UseTextOptions = true;
                        colColorID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colColorID.Caption = dc.ColumnName.Replace("Size@", "");
                        colColorID.FieldName = dc.ColumnName;
                        colColorID.Name = "col" + dc.ColumnName;
                        colColorID.OptionsColumn.AllowEdit = false;
                        colColorID.Visible = true;
                        colColorID.Width = 50;
                        bandedGridView1.Columns.AddRange(new BandedGridColumn[] { colColorID });
                        gridBandColorID.Columns.Add(colColorID);
                    }
                    else if (dc.ColumnName.ToString().ToLower() == "ghichu")
                    {
                        BandedGridColumn colGC = new BandedGridColumn();
                        colGC.AppearanceCell.Options.UseTextOptions = true;
                        colGC.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colGC.AppearanceHeader.Options.UseTextOptions = true;
                        colGC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colGC.Caption = dc.ColumnName.Replace("Size@", "");
                        colGC.FieldName = dc.ColumnName;
                        colGC.Name = "col" + dc.ColumnName;
                        colGC.OptionsColumn.AllowEdit = true;
                        colGC.Visible = true;
                        colGC.Width = 50;
                        bandedGridView1.Columns.AddRange(new BandedGridColumn[] { colGC });
                        gridBandGC.Columns.Add(colGC);
                    }
                    if (dc.ColumnName.ToString().ToLower() == "ma_qc_dongthung")
                    {
                        searchLookupEdit.DisplayMember = "MaQuiCach"; // Tên cột hiển thị
                        searchLookupEdit.ValueMember = "MaQuiCach"; // Tên cột chứa giá trị
                        //searchLookupEdit.PopupView = this.searchLookUp_QC_DongThung_View;

                        // Thêm các tùy chọn tùy chỉnh khác (nếu cần)
                        searchLookupEdit.NullText = "Chọn qui cách đóng thùng";
                        searchLookupEdit.EditValueChanged += new System.EventHandler(searchLookUpEdit_EditValueChanged);
                        searchLookupEdit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEdit_CustomDisplay);


                        view = searchLookupEdit.View;
                        if (view.Columns.Count == 0)
                        {
                            view.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            view.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            view.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "MaQuiCach", Name = "colMaQuiCach", Caption = "Mã qui cách", Visible = true });
                            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "TenQuiCach", Name = "colTenQuiCach", Caption = "Tên qui cách", Visible = true, Width = 100 });
                            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "ChieuDai", Name = "colChieuDai", Caption = "Chiều dài", Visible = true });
                            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "ChieuRong", Name = "colChieuRong", Caption = "Chiều rộng", Visible = true });
                            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "ChieuCao", Name = "colChieuCao", Caption = "Chiều cao", Visible = true });
                            view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = "CanNang", Name = "colCanNang", Caption = "Cân nặng", Visible = true });
                        }


                        BandedGridColumn colQC_DongThung = new BandedGridColumn();
                        colQC_DongThung.AppearanceCell.Options.UseTextOptions = true;
                        colQC_DongThung.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colQC_DongThung.AppearanceHeader.Options.UseTextOptions = true;
                        colQC_DongThung.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        colQC_DongThung.FieldName = dc.ColumnName;
                        colQC_DongThung.Name = "col" + dc.ColumnName;
                        colQC_DongThung.OptionsColumn.AllowEdit = true;
                        colQC_DongThung.Visible = true;
                        colQC_DongThung.Width = 50;
                        colQC_DongThung.ColumnEdit = searchLookupEdit;
                        bandedGridView1.Columns.AddRange(new BandedGridColumn[] { colQC_DongThung });
                        gridBand_QC_DongThung.Columns.Add(colQC_DongThung);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void searchLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            bool isSearch = sender is SearchLookUpEdit;
            dynamic a;
            BindingList<DicQCDongThungEntity> dataSourceSearchLookup;
            //new BindingList<DicQCDongThungEntity>(_listMaHang);
            if (sender is SearchLookUpEdit)
            {
                SearchLookUpEdit search = sender as SearchLookUpEdit;
                a = search.Properties.Tag;
                dataSourceSearchLookup = new BindingList<DicQCDongThungEntity>((IList<DicQCDongThungEntity>)search.Properties.DataSource);
            }
            else if (sender is RepositoryItemSearchLookUpEdit)
            {
                RepositoryItemSearchLookUpEdit repo = sender as RepositoryItemSearchLookUpEdit;
                a = repo.Tag;
            }
            Console.WriteLine("searchLookUpEdit_EditValueChanged");
        }

        private void bandedGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }


        private void NapLaiDong()
        {
            LoadDataMaHang();
            GetDSQC_DongThungAsync();
        }
        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void searchLookUpEdit_CustomDisplay(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            List<DicQCDongThungEntity> dataSourceSearchLookup = new List<DicQCDongThungEntity>();
            if (sender is SearchLookUpEdit)
            {
                SearchLookUpEdit search = sender as SearchLookUpEdit;
                if (search.Properties.DataSource != null)
                {
                    dataSourceSearchLookup = new List<DicQCDongThungEntity>((IList<DicQCDongThungEntity>)search.Properties.DataSource);
                }
            }
            else if (sender is RepositoryItemSearchLookUpEdit)
            {
                RepositoryItemSearchLookUpEdit repo = sender as RepositoryItemSearchLookUpEdit;
                if (repo.DataSource != null)
                {
                    dataSourceSearchLookup = new List<DicQCDongThungEntity>((IList<DicQCDongThungEntity>)repo.DataSource);
                }
            }
            if (e.Value != null && e.Value.ToString() != null && e.Value.ToString() != "")
            {
                e.DisplayText = stringDisplaySearchLookup(e.Value.ToString(), dataSourceSearchLookup);
            }
        }
        private string stringDisplaySearchLookup(string value, List<DicQCDongThungEntity> lstSource)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lstSource.Count; i++)
            {
                if (value == lstSource[i].MaQuiCach)
                {
                    sb.Append(lstSource[i].ChieuDai);
                    sb.Append(" x ");
                    sb.Append(lstSource[i].ChieuRong);
                    sb.Append(" x ");
                    sb.Append(lstSource[i].ChieuCao);
                    break;
                }
            }
            return sb.ToString();
        }


    }
}
