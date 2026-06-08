using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    public partial class frmSoDoKho : DevExpress.XtraEditors.XtraForm
    {
        HttpClientExtension _clientExtension = new HttpClientExtension();
        System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        DataTable _tblKho = new DataTable();
        DataTable _tblViTriKho = new DataTable();
        public frmSoDoKho()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            LoadKhoVT();
        }

        private void LoadViTriKho(string MaKho)
        {
            string url = string.Format("{0}/ViTriKho/Get?action=GetShowVTK&&parameter={1}", URL, MaKho);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblViTriKho = JsonConvert.DeserializeObject<DataTable>(json);
        }

        private void LoadKhoVT()
        {
            string url = string.Format("{0}/ViTriKho/Get?action=GetKhoVT&&parameter={1}", URL, "para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblKho = JsonConvert.DeserializeObject<DataTable>(json);
            dgrViTriKho.DataSource = _tblKho;
            gridViewViTriKho.ExpandAllGroups();
        }

        private void gridViewViTriKho_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colMaKho)
            {
                DataRow dr = _tblKho.AsEnumerable().Where(x => x["MaKho"].ToString() == e.Value).FirstOrDefault();
                e.DisplayText = dr["TenKho"].ToString();
            }
        }

        private void LoadSoDoKho(string maVT, string TenVT)
        {
            int i = 0;
            DataTable tblSoDoKho = new DataTable();
            List<DataRow> lstTang = new List<DataRow>();
            List<DataRow> lstO = new List<DataRow>();
            GridControl dgrView = CreateGridview(maVT);
            lstTang = _tblViTriKho.AsEnumerable().Where(x => x["ParentMaVT"].ToString() == maVT).OrderByDescending(x => x["MaVT"].ToString()).ToList();
            foreach (DataRow tang in lstTang)
            {
                lstO = _tblViTriKho.AsEnumerable().Where(x => x["ParentMaVT"].ToString() == tang["MaVT"].ToString()).OrderBy(x => x["MaVT"]).ToList();
                if (i == 0)
                {
                    tblSoDoKho = CreateDataTableSoDoKho(lstO);
                    CreateGridviewSoDoKho((GridView)dgrView.MainView, lstO);
                    i = 1;
                }
                DataRow dradd = tblSoDoKho.NewRow();
                dradd["Title"] = maVT;
                foreach (DataRow o in lstO)
                {
                    string[] arr = o["MaVT"].ToString().Trim().Split('.');
                    dradd[arr[2]] = string.Format("{0}", o["MaVT"].ToString());
                    dradd["Value_" + arr[2]] = string.Format("{0}", o["CBM_IsUsed"].ToString());
                }
                dradd["Tang"] = tang["MaVT"].ToString();
                tblSoDoKho.Rows.Add(dradd);
            }
            dgrView.Height = tblSoDoKho.Rows.Count * 35;
            dgrView.DataSource = tblSoDoKho;
        }

        private GridControl CreateGridview(string MaVT)
        {
            GridControl dgrView = new GridControl();
            GridView gridView = new GridView();

            gridView.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(255)))));
            gridView.Appearance.FocusedCell.Options.UseBackColor = true;
            gridView.Appearance.Row.BorderColor = System.Drawing.Color.Navy;
            gridView.Appearance.Row.Options.UseBorderColor = true;
            gridView.ColumnPanelRowHeight = 30;
            gridView.GridControl = dgrView;
            gridView.Name = "gridView" + MaVT.Replace(".", "_");
            gridView.OptionsMenu.ShowConditionalFormattingItem = true;
            gridView.OptionsView.AllowCellMerge = true;
            gridView.OptionsView.ShowColumnHeaders = false;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsView.ShowIndicator = false;
            gridView.RowHeight = 30;
            gridView.DoubleClick += GridView_DoubleClick; ;
            gridView.CustomColumnDisplayText += GridView_CustomColumnDisplayText;

            dgrView.Dock = System.Windows.Forms.DockStyle.Top;
            dgrView.Location = new System.Drawing.Point(0, 0);
            dgrView.LookAndFeel.SkinName = "Office 2019 Colorful";
            dgrView.LookAndFeel.UseDefaultLookAndFeel = false;
            dgrView.LookAndFeel.UseWindowsXPTheme = true;
            dgrView.MainView = gridView;
            dgrView.MenuManager = this.barManager1;
            dgrView.Name = "dgr" + MaVT.Replace(".", "_");
            dgrView.Size = new System.Drawing.Size(985, 157);
            dgrView.TabIndex = 0;
            dgrView.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {gridView});

            xtraScrollableControl1.Controls.Add(dgrView);
            return dgrView;
        }

        private void GridView_DoubleClick(object sender, EventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (gridView.GetFocusedValue() == null) return;
            string MaVT = gridView.GetFocusedValue().ToString();
            DataRow dr = _tblViTriKho.Select(string.Format("MaVT = '{0}'", MaVT)).FirstOrDefault();

            string url = string.Format("{0}/ViTriKho/GetPN_VTK?action=GetPN_VTK&&parameter={1}", URL, MaVT);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);

            frmChiTiet_NL_ViTri frm = new frmChiTiet_NL_ViTri(tblChiTiet, dr);
            frm.ShowDialog();
        }

        private void GridView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            DataRow dr = _tblViTriKho.Select(string.Format("MaVT = '{0}'", e.Value)).FirstOrDefault();
            if (dr != null)
            {
                List<string> lstName = e.Value.ToString().Split('.').ToList();
                string dispalyValue = string.Format("T {3}_{0} [{1}/{2}]", dr["TenVT"], dr["CBM_IsUsed"], dr["CBM"], lstName[1]);
                if (dr["MaVT"].ToString().Contains("00"))
                    e.DisplayText = dr["TenVT"].ToString();
                else
                    e.DisplayText = dispalyValue;
            }
        }

        private DataTable CreateDataTableSoDoKho(List<DataRow> lstO)
        {
            try
            {
                DataTable tblSoDoKho = new DataTable();
                tblSoDoKho.Columns.Add("Title", typeof(string));
                foreach (DataRow o in lstO)
                {
                    string[] arr = o["MaVT"].ToString().Trim().Split('.');
                    tblSoDoKho.Columns.Add(arr[2], typeof(string));
                    tblSoDoKho.Columns.Add("Value_" + arr[2], typeof(double));
                }
                tblSoDoKho.Columns.Add("Tang", typeof(string));
                return tblSoDoKho;
            }
            catch (Exception ex) { return new DataTable(); }
        }

        private void CreateGridviewSoDoKho(GridView gridView, List<DataRow> lstO)
        {
            int i = 1;
            gridView.Columns.Clear();
            gridView.FormatRules.Clear();
            GridColumn colTitle = new GridColumn();
            colTitle.AppearanceCell.Options.UseTextOptions = true;
            colTitle.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            colTitle.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            colTitle.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            colTitle.AppearanceCell.Options.UseBackColor = true;
            colTitle.AppearanceCell.Options.UseFont = true;
            colTitle.FieldName = "Title";
            colTitle.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            colTitle.MaxWidth = 100;
            colTitle.Name = "colTitle";
            colTitle.OptionsColumn.AllowEdit = false;
            colTitle.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            colTitle.Visible = true;
            colTitle.VisibleIndex = 0;
            colTitle.Width = 100;
            gridView.Columns.Add(colTitle);
            foreach (DataRow o in lstO)
            {
                string[] arr = o["MaVT"].ToString().Trim().Split('.');
                GridColumn colO = new GridColumn();
                colO.AppearanceCell.Options.UseTextOptions = true;
                colO.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                colO.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                colO.FieldName = arr[2];
                colO.MaxWidth = 150;
                colO.MinWidth = 150;
                colO.Name = "col" + arr[2];
                colO.OptionsColumn.AllowEdit = false;
                colO.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                colO.Visible = true;
                colO.VisibleIndex = i;
                colO.Width = 180;

                GridColumn colO_Value = new GridColumn();
                colO_Value.AppearanceCell.Options.UseTextOptions = true;
                colO_Value.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                colO_Value.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                colO_Value.FieldName = "Value_" + arr[2];
                colO_Value.MaxWidth = 150;
                colO_Value.MinWidth = 150;
                colO_Value.Name = "col_value" + arr[2];
                colO_Value.OptionsColumn.AllowEdit = false;
                colO_Value.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                colO_Value.Visible = true;
                colO_Value.VisibleIndex = i + 1;
                colO_Value.Width = 180;
                colO_Value.Visible = false;

                gridView.Columns.Add(colO);
                gridView.Columns.Add(colO_Value);

                GridFormatRule gridFormatRule1 = new GridFormatRule();
                FormatConditionRuleDataBar formatConditionRuleDataBar1 = new FormatConditionRuleDataBar();
                gridFormatRule1.Column = colO_Value;
                gridFormatRule1.ColumnApplyTo = colO;
                gridFormatRule1.Name = "Format" + arr[2];
                formatConditionRuleDataBar1.Appearance.BackColor = System.Drawing.Color.Aqua;
                formatConditionRuleDataBar1.Appearance.BackColor2 = System.Drawing.Color.White;
                formatConditionRuleDataBar1.Appearance.BorderColor = System.Drawing.Color.Black;
                formatConditionRuleDataBar1.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                formatConditionRuleDataBar1.Appearance.Options.UseBackColor = true;
                formatConditionRuleDataBar1.Appearance.Options.UseBorderColor = true;
                formatConditionRuleDataBar1.Maximum = (decimal)(Convert.ToDecimal(o["CBM"]));
                formatConditionRuleDataBar1.MaximumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
                formatConditionRuleDataBar1.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
                formatConditionRuleDataBar1.MinimumType = DevExpress.XtraEditors.FormatConditionValueType.Number;
                formatConditionRuleDataBar1.PredefinedName = null;
                gridFormatRule1.Rule = formatConditionRuleDataBar1;

                gridView.FormatRules.Add(gridFormatRule1);
                i += 2;
            }
            #region
            //GridColumn colTang = new GridColumn();
            //colTang.AppearanceCell.Options.UseTextOptions = true;
            //colTang.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            //colTang.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            //colTang.AppearanceCell.Options.UseFont = true;
            //colTang.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0);
            //colTang.AppearanceCell.Options.UseForeColor = true;
            //colTang.FieldName = "Tang";
            //colTang.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            //colTang.MaxWidth = 150;
            //colTang.Name = "colTang";
            //colTang.OptionsColumn.AllowEdit = false;
            //colTang.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            //colTang.Visible = true;
            //colTang.VisibleIndex = i;
            //colTang.Width = 150;
            //gridViewSoDoKho.Columns.Add(colTang);
            #endregion
        }

        private void gridViewSoDoKho_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;
            e.Menu.Items.Clear();
            DXMenuItem XemChiTiet = new DXMenuItem();
            XemChiTiet.Caption = "Xem chi tiết";
            XemChiTiet.Click += XemChiTiet_Click;
            e.Menu.Items.Add(XemChiTiet);
        }

        private void XemChiTiet_Click(object sender, EventArgs e)
        {
            if (gridViewSoDoKho.GetFocusedValue() == null) return;
            string MaVT = gridViewSoDoKho.GetFocusedValue().ToString();
            DataRow dr = _tblViTriKho.Select(string.Format("MaVT = '{0}'", MaVT)).FirstOrDefault();

            string url = string.Format("{0}/ViTriKho/GetPN_VTK?action=GetPN_VTK&&parameter={1}", URL, MaVT);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);

            frmChiTiet_NL_ViTri frm = new frmChiTiet_NL_ViTri(tblChiTiet, dr);
            frm.ShowDialog();
        }

        private void gridViewSoDoKho_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            DataRow dr = _tblViTriKho.Select(string.Format("MaVT = '{0}'", e.Value)).FirstOrDefault();
            if (dr != null)
            {
                List<string> lstName = e.Value.ToString().Split('.').ToList();
                string dispalyValue = string.Format("T {3}_{0} [{1}/{2}]", dr["TenVT"], dr["CBM_IsUsed"], dr["CBM"], lstName[1]);
                if (dr["MaVT"].ToString().Contains("00"))
                    e.DisplayText = dr["TenVT"].ToString();
                else
                    e.DisplayText = dispalyValue;
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            this.ActiveControl = dgrSoDoKho;
            DataRow dr = gridViewViTriKho.GetFocusedDataRow();
            if (dr == null) return;
            if(bool.Parse(dr["Chon"].ToString()) == true)
            {
                string MaKho = dr["MaKho"].ToString();
                LoadViTriKho(MaKho);
                LoadSoDoKho(dr["MaVT"].ToString(), dr["TenVT"].ToString());
            }
            else
            {
                xtraScrollableControl1.Controls.RemoveByKey("dgr" + dr["MaVT"].ToString().Replace(".", "_"));
            }
            
        }
    }
}
