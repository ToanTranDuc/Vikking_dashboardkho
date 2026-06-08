using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmXemDMLenh : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _makh = string.Empty, _mahang = string.Empty, _madot = string.Empty;
        DataTable tblMauVT = new DataTable();
        DataTable tblChungLoaiChiTiet = new DataTable();
        public frmXemDMLenh(string makh, string mahang, string madot)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _makh = makh;
            _mahang = mahang;
            _madot = madot;
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadLenh();
            LoadTVMauVT();
            CreateRepoSearchLookUpChungLoaiCT();
            CreateMauSPColumns();
        }

        private void LoadLenh()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GetMaLenh&para1={_madot}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            searchLookUpEdit1.Properties.DataSource = tbl;
            searchLookUpEdit1.Properties.DisplayMember = "MaLenh";
            searchLookUpEdit1.Properties.ValueMember = "MaLenh";
        }

        private async void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            //LoadData();
            SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu...", "Vui lòng chờ");

            try
            {
                await LoadData(); // chạy async
            }
            finally
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
        }

        private async Task LoadData()
        {
            string malenh = searchLookUpEdit1.EditValue.ToString();

            string urldata = $"{URL}KhoiTaoBOMV1/Get?Action=GetdataLenh&para1={_makh.ToString()}&para2={_mahang.ToString()}&para3={_madot.ToString()}&para4={malenh.ToString()}";
            string jsondata = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldata); }).Result;
            DataTable tbldata = JsonConvert.DeserializeObject<DataTable>(jsondata);

            gridControl11.DataSource = tbldata;

        }

        private void CreateMauSPColumns()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUSPCOLUMNS&para1={_makh}&para2={_mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _tblMauSP = JsonConvert.DeserializeObject<DataTable>(json);
            createTableCT(_tblMauSP);
        }

        private void createTableCT(DataTable tab)
        {
            DataTable tbl = gridControl11.DataSource as DataTable;
            GridBand parentBand = bandedGridView11.Bands["gridBandMauSP"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView11.Columns.Count;)
                {
                    if (bandedGridView11.Columns[i].FieldName.Contains("gridBandMauSPa"))
                    {
                        bandedGridView11.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }

            foreach (DataRow column in tab.Rows)
            {
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = column[4].ToString();
                col.FieldName = column[4].ToString() + "@Mau@" + column[1].ToString();
                col.Name = "col" + column[4].ToString();
                // col.OptionsColumn.AllowEdit = false; // REMOVE DÒNG NÀY!
                col.Visible = true;
                col.Width = 50;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                // GÁN EDITOR TRƯỚC KHI ADD COLUMN
                col = CreateSearchLookUpMauVT(col);
                bandedGridView11.Columns.AddRange(new BandedGridColumn[] { col });

                GridBand gb = new GridBand();
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.Caption = column[4].ToString();
                gb.Columns.Add(col);
                gb.Name = "gridBandMauSPa" + col;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gridBandMauSP.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                if (tbl != null)
                {
                    tbl.Columns.Add(column[1].ToString(), typeof(string));
                }

            }
            gridControl11.DataSource = tbl;
        }


        private BandedGridColumn CreateSearchLookUpMauVT(BandedGridColumn col)
        {
            try
            {
                // KIỂM TRA DATA SOURCE
                if (tblMauVT == null || tblMauVT.Rows.Count == 0)
                {
                    MessageBox.Show("tblMauVT chưa có dữ liệu!");
                    return col;
                }

                RepositoryItemButtonEdit btnEdit = new RepositoryItemButtonEdit();
                btnEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

                btnEdit.Buttons.Clear();
                DevExpress.XtraEditors.Controls.EditorButton button =
            new DevExpress.XtraEditors.Controls.EditorButton(
                DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);

                button.ImageOptions.Image = Properties.Resources.group_16x16;


                btnEdit.Buttons.Add(button);


                gridControl11.RepositoryItems.Add(btnEdit);

                btnEdit.ButtonClick += (s, e) =>
                {
                    DataRow dr = bandedGridView11.GetFocusedDataRow();
                    if (dr == null) return;
                    frmPhanTichBOM_ChonMau frm = new frmPhanTichBOM_ChonMau(dr);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        dr["MauVTIDChung"] = frm._mauvtidchung;
                        LoadTVMauVT();
                        DataTable tbl = frm.tblGrid;
                        if (tbl == null || tbl.Rows.Count == 0) return;
                        foreach (DataRow r in tbl.Rows)
                        {
                            string columnName = r["MauSPID"].ToString();
                            string mauVTID = r["MauVTID"].ToString();
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                if (dc.ColumnName == columnName)
                                {
                                    dr[columnName] = mauVTID;
                                }
                            }

                        }

                    }
                    this.ActiveControl = button1;
                };

                col.ColumnEdit = btnEdit;

                return col;
            }
            catch (Exception ex)
            {

                return col;
            }
        }

        private void bandedGridView11_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains("@Mau@"))
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    string mauVTID = e.Value.ToString();


                    DataRow[] rows = tblMauVT.Select($"MauVTID = '{mauVTID}'");
                    if (rows.Length > 0)
                    {
                        e.DisplayText = rows[0]["MaMauVT"].ToString();
                    }
                }
            }
        }

        private void LoadTVMauVT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUVTTV";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            CreateRepoSearchLookUpChungLoaiCT();
        }

        private void bandedGridView11_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "IsXetDuyet")
            {
                // Lấy giá trị của ô hiện tại trong cột đang xét
                object cellValue = e.CellValue;

                if (cellValue != null)
                {
                    if (cellValue.ToString() == "Đã duyệt")
                    {

                        e.Appearance.ForeColor = Color.Green;
                    }
                    else
                    {

                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
        }

        private void bandedGridView11_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            //if (info.Column == gridColumn24)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            if (info.Column == gridColumn5)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn10)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (bandedGridView11.IsGroupRow(e.RowHandle))
            {


                int groupIndex = bandedGridView11.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                else if (groupIndex == 2)
                {
                    textColor = Color.Maroon;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void CreateRepoSearchLookUpChungLoaiCT()
        {
            try
            {
                repoSearchCLCT.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT.ValueMember = "MaNhomChiTiet";
                searchLookUpEditCLCT.Properties.DisplayMember = "TenNhomChiTiet";
                searchLookUpEditCLCT.Properties.ValueMember = "MaNhomChiTiet";
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tblChungLoaiChiTiet;
                searchLookUpEditCLCT.Properties.DataSource = tblChungLoaiChiTiet;

            }
            catch (Exception ex)
            {
            }
        }
    }
}