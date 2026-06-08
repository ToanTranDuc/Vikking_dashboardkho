using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
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
    public partial class frmChiTietXuatHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maPhieuXH = string.Empty, _poID =string.Empty;

        private HttpClientExtension _clientExtension;
        public frmChiTietXuatHang(string PoID)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._poID = PoID.ToString();
        }
        protected override void OnLoad(EventArgs e)
        {
            Init();
            
        }
        private void Init()
        {
            searchLookUpEdit_MaPKL.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL.Properties.ValueMember = "MaPKL_XH";
            searchLookUpEdit_MaPKL.Properties.NullText = "[Chọn PKL]";
            GetMaPKL_XuatHang();
        }
        private void GetMaPKL_XuatHang()
        {
            string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GetMaPKL_XH&Para1={_poID}&Para2=A&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaPKL.Properties.DataSource = tbl;
            if (tbl.Rows.Count != 0) searchLookUpEdit_MaPKL.EditValue = tbl.Rows[0]["MaPKL_XH"];
            else return;
            LoadPKLXuatHang();
        }
        private void LoadPKLXuatHang()
        {
            _maPhieuXH = searchLookUpEdit_MaPKL.EditValue.ToString();
            if (_maPhieuXH == "") return;
            DataTable dtData = new DataTable();
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXH&Para1={_maPhieuXH}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang1 = JsonConvert.DeserializeObject<DataTable>(json);
            url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXHTon&Para1={_maPhieuXH}&Para2=Para");
            json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang2 = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtPKLXuatHang2 is null) dtData = dtPKLXuatHang1;
            else
            {
                dtData = dtPKLXuatHang2;
                dtData.Merge(dtPKLXuatHang1);
            }

            CreateBandSize(dtData);
            KHDongThungLib.ProcessSttTrung1(dtData);
            dgrPKLXuatHang.DataSource = dtData;
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
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = this.repotxtN0;
                col.Visible = true;
                col.Width = 40;
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                grvPKLXuatHang.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.Black;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
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
        private void ClearBand()
        {
            gbSize.Children.Clear();
        }

        private void searchLookUpEdit_MaPKL_EditValueChanged(object sender, EventArgs e)
        {
            LoadPKLXuatHang();
        }

        private void grvPKLXuatHang_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e);
        }

        private void grvPKLXuatHang_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
            //int _valueSumaryCaton = 0;
            //if (e.Item == null)
            //{
            //    return;
            //}
            //GridSummaryItem item = e.Item as GridSummaryItem;
            //if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            //{

            //    if (item.FieldName.Equals("SLThung"))
            //    {
            //        _valueSumaryCaton = SumItemCaton();
            //        e.TotalValue = _valueSumaryCaton;
            //    }
            //}
        }

        private void grvPKLXuatHang_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void grvPKLXuatHang_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            //if (e.RowHandle < 0) return;
            //var Check = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "IsTon").ToString();
            //if (Check != "0")
            //    e.Appearance.BackColor = Color.DarkOrange;
        }

       
       
        private bool CheckExistBand(string size)
        {

            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
      
    }
}