using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmChiTietThongKeDTNK : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maPKL = string.Empty, _maDH = string.Empty, _poID = string.Empty;

        private HttpClientExtension _clientExtension;
        public frmChiTietThongKeDTNK(string maDH, string PoID)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._maDH = maDH.ToString();
            this._poID = PoID.ToString();
        }
        protected override void OnLoad(EventArgs e)
        {
            Init();            
        }
        private void Init()
        {
            searchLookUpEdit_MaPKL.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL.Properties.ValueMember = "MaPKL";
            searchLookUpEdit_MaPKL.Properties.NullText = "[Chọn PKL]";
            GetDotPKL();
        }
        private void GetDotPKL()
        {
            string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GetDotLapPKL&Para1={_maDH}&Para2={_poID}&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaPKL.Properties.DataSource = tbl;
            if (tbl.Rows.Count != 0) searchLookUpEdit_MaPKL.EditValue = tbl.Rows[0]["MaPKL"];
            else return;
            GetChiTietDTNK();
        }
        private void GetChiTietDTNK()
        {
            _maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
            //string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung&MaDH={_maDH}&MaDVSX=Para&DotSX=Para&POID={_poID}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung&MaDH={_maDH}&POID={_maDH + "||" + _poID}&SizeTypeID=Para&ColorID=Para&MaPKL={_maDH +"||"+_poID + "||" + _maPKL}");
            // string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GetChiTietNKDK&Para1={_maPKL}&Para2={_maDH}&Para3={_poID}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
           
            CreateBandForSize(tbl);
            KHDongThungLib.ProcessSttTrung(tbl);
            dgrKHDongThung.DataSource = tbl;
            dgrKHDongThung.RefreshDataSource();
        }

        private void searchLookUpEdit_MaPKL_EditValueChanged(object sender, EventArgs e)
        {
            GetChiTietDTNK();
        }

        private void bandedGridViewKHDT_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            //GridView view = sender as GridView;
            KHDongThungLib.Merge(sender, e);
            //bool ShouldMerge(string fieldName)
            //{
            //    e.Handled = true;
            //    e.Merge = false;
            //    if (!object.Equals(e.CellValue1, e.CellValue2))
            //        return false;
            //    string id1 = view.GetRowCellValue(e.RowHandle1, "SttThung").ToString();
            //    string id2 = view.GetRowCellValue(e.RowHandle2, "SttThung").ToString();
            //    return id1 == id2;
            //}

            //if (e.Column.FieldName == "SLThung" || e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung" || e.Column.FieldName == "SLDDThung" || e.Column.FieldName == "SLDNK")
            //{
            //    e.Merge = ShouldMerge(e.Column.FieldName);
            //}
        }

        private void bandedGridViewKHDT_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
            //int _valueSumaryCaton = 0;
            //if (e.Item == null)
            //{
            //    return;
            //}
            //GridSummaryItem item = e.Item as GridSummaryItem;
            //if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            //{
            //    List<string> lstSum = new List<string>()
            //    {
            //        "SLThung","SLDDThung","SLDNK"
            //     };
            //    if (lstSum.Contains(item.FieldName))
            //    {
            //        _valueSumaryCaton = 0;
            //        _valueSumaryCaton = SumItemCatonDongThung(item.FieldName);
            //        e.TotalValue = _valueSumaryCaton;
            //    }


            //}
        }        
        private void CreateBandForSize(DataTable dt)
        {
            try
            {
                ClearDataBandAndCol();
                foreach (DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    if (!colName.Contains('@')) continue;
                    var _sizeID = colName.Split('@')[1];
                    var _size = colName.Split('@')[0]; ;
                    if (!CheckExistBand(_sizeID)) continue;
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = _size;
                    col.FieldName = _size + "@" + _sizeID;
                    col.Name = "col" + _sizeID;
                    col.OptionsColumn.AllowEdit = true;
                    col.ColumnEdit = this.repotxtN0;
                    col.Visible = true;
                    col.Width = 40;
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridViewKHDT.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                    gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                    gb.AppearanceHeader.Options.UseBackColor = true;
                    gb.AppearanceHeader.Options.UseFont = true;
                    gb.AppearanceHeader.Options.UseForeColor = true;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.Caption = _size;
                    gb.Columns.Add(col);
                    gb.Name = "gb" + "Size@" + _sizeID;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;
                    gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        

        private void bandedGridViewKHDT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }
        private void ClearDataBandAndCol()
        {
            try
            {
                //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                gbSize.Children.Clear();
                dgrKHDongThung.DataSource = new DataTable();
                List<GridColumn> lst = new List<GridColumn>();
                lst = bandedGridViewKHDT.Columns.Where(x => x.FieldName.Contains(@"Size@")).ToList();
                if (lst != null && lst.Count > 0)
                    foreach (GridColumn gc in lst) bandedGridViewKHDT.Columns.Remove(gc);
            }
            catch (Exception ex) { };
        }
        private bool CheckExistBand(string size)
        {
            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
    }
}