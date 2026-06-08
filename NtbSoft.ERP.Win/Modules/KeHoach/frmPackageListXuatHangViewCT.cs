using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmPackageListXuatHangViewCT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
      
        private string _controller = string.Empty, _maDH = string.Empty, _maPKL = string.Empty, _poid = string.Empty,_statusTonKho;

        public frmPackageListXuatHangViewCT(string controller,string maDH,string maPKL,string poid,string maPKLDisPlay,string statusTonKho)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maDH = maDH;
            _maPKL = maPKL;
            _poid = poid;
            _statusTonKho = statusTonKho;
            _controller = controller;
            this.Text = maPKLDisPlay;
            LoadPKL();
        }
        private void LoadPKL()
        {
            string url = string.Format("{0}", URL + $"{_controller}/Get?Action=GetPivotKHDongThung&MaDH={_maDH}&MaDVSX=Para&DotSX=Para&POID={_poid}&SizeTypeID={1}&ColorID={_statusTonKho}&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLDetail = JsonConvert.DeserializeObject<DataTable>(json);
            CreateBandSize(dtPKLDetail);
            grcPKLDongThung.DataSource = dtPKLDetail;
        }
        private void CreateBandSize(DataTable dt)
        {
            ClearBand();
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
                if (!CheckExistBand(_sizeID)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = _size + "@" + _sizeID;
                col.Name = "col" + _sizeID;              
                col.ColumnEdit = this.repotxtN0;
                col.Visible = true;
                col.Width = 40;
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                bandgrvDongThung.Columns.AddRange(new BandedGridColumn[] { col });
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

       

        private void bandgrvDongThung_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "SLThung" || e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
            {
                var preSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle - 1, "SttThung");
                var curSTTThung = bandgrvDongThung.GetRowCellValue(e.RowHandle, "SttThung");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                {
                    e.Handled = true;
                    return;
                }
            }
        }        

        private void bandgrvDongThung_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }
        private void bandgrvDongThung_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            GridView view = sender as GridView;
            bool ShouldMerge(string fieldName)
            {
                e.Handled = true;
                e.Merge = false;
                if (!object.Equals(e.CellValue1, e.CellValue2))
                    return false;
                string id1 = view.GetRowCellValue(e.RowHandle1, "SttThung").ToString();
                string id2 = view.GetRowCellValue(e.RowHandle2, "SttThung").ToString();
                return id1 == id2;
            }

            if (e.Column.FieldName == "SLThung" || e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung" 
                || e.Column.FieldName == "SLNhapKho" || e.Column.FieldName == "SLTDaXuat" || e.Column.FieldName == "SLNhapTK" || e.Column.FieldName == "SLXuat")
            {
                e.Merge = ShouldMerge(e.Column.FieldName);
            }
        }
        private void bandgrvDongThung_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            int _valueSumaryCaton = 0;
            if (e.Item == null)
            {
                return;
            }
            GridSummaryItem item = e.Item as GridSummaryItem;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                List<string> lstSum = new List<string>()
                {
                    "SLThung","SLNhapKho","SLTDaXuat","SLXuat","SLNhapTK"
                 };
                if (lstSum.Contains(item.FieldName))
                {
                    _valueSumaryCaton = 0;
                    _valueSumaryCaton = SumItemCatonDongThung(item.FieldName);
                    e.TotalValue = _valueSumaryCaton;
                }
            }
        }
        private int SumItemCatonDongThung(string sumFooter)
        {
            int SumCaton = 0;
            var dt = grcPKLDongThung.DataSource as DataTable;
            if (dt != null && dt.Rows.Count > 0)
            {

                var query = dt.AsEnumerable().Select(x =>
                   new
                   {
                       SttThung = x["SttThung"],
                       MaPKL = x["MaPKL"],
                       SLThung = x[sumFooter]
                   }).ToList().Distinct();

                SumCaton = query.Sum(item => Convert.ToInt32(item.SLThung));
                return SumCaton;
            }
            return 0;
        }
        private void ClearBand()
        {
            gbSize.Children.Clear();
        }
        private bool CheckExistBand(string size)
        {
            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
    }
}
