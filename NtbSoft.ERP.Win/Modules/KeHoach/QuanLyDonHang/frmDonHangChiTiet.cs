using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmDonHangChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty, _donhang = string.Empty,_dausize =string.Empty,_po = string.Empty,_mamau = string.Empty;
        string _mahang = string.Empty, _khachhang = string.Empty, _chungloai = string.Empty, _soluong = string.Empty, _ngaytao = string.Empty;
        private int _valueDemSize = 0, sumtong = 0;

        //private void bgrViewSize_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        //{
           
        //}

        private void bgrViewSize_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void bgrViewSize_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            DataRow cutDetail = view.GetDataRow(e.RowHandle) as DataRow;
            int rowHandle = e.RowHandle;
            if (e.Column.FieldName.Contains("@"))
            {
                int sumSizeValues = 0;

                foreach (DataColumn column in cutDetail.Table.Columns)
                {
                    if (column.ColumnName.Contains("Size@"))
                    {

                        sumSizeValues += Convert.ToInt32(cutDetail[column]);
                    }
                }

                view.SetRowCellValue(view.FocusedRowHandle, colSoLuong, sumSizeValues);
            }
            foreach (GridColumn col in bgrViewSize.Columns)
            {
                if (col.FieldName == "soluong2")
                {
                    col.Summary.Clear();
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
            }
        }

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;

        public frmDonHangChiTiet(string donhang,string mahang,string soluong,string ngaytao, string chungloai)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._donhang = donhang;
            this._mahang = mahang;
            this._chungloai = chungloai;
            this._soluong = soluong;
            if (DateTime.TryParse(ngaytao, out DateTime ngayTaoDateTime))
            {
                this._ngaytao = ngayTaoDateTime.ToString("dd/MM/yyyy");
            }
           
            LoadSize();
            
        }
        private void sumTotalRow(DataTable data)
        {
            for (int i = 0; i < data.Rows.Count; i++)
            {
                int sumSizeValues = 0;
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    if (data.Columns[j].ColumnName.Contains("Size@"))

                    {
                        sumSizeValues += Convert.ToInt32(data.Rows[i][j]);
                    }
                }
                bgrViewSize.SetRowCellValue(bgrViewSize.FocusedRowHandle + i, colSoLuong, sumSizeValues);
            }
            foreach (GridColumn col in bgrViewSize.Columns)
            {
                if (col.FieldName == "soluong2")
                {
                    col.Summary.Clear();
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
            }
        }

        private void BgrViewSize_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            throw new NotImplementedException();
        }

    
        private void LoadSize()
        {
            txtDonhang.Text = _donhang.ToString();
            txtMahang.Text = _mahang.ToString();
            txtChungloai.Text = _chungloai.ToString();
            txtNgaygiaohang.Text = _ngaytao.ToString();
            txtSoLuong.Text = _soluong.ToString();

            string url = string.Format("{0}/?donhang={1}", URL + "DonHangTong/GetMaHangCT", _donhang);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);


            if (dt.Rows.Count > 0)
            {
                ClearDataBandAndCol();
                creatbandsize(dt);
                gridControl2.DataSource = dt;
                sumTotalRow(dt);
                grdSL.Width = 200;
            }
            else
            {
                gridControl2.DataSource = null;
            }

        }
        private void creatbandsize(DataTable _dt)
        {
            gbSize.Width = (_dt.Rows.Count - 10) * 50;
            
            _valueDemSize = 0;
            for (int i = 0; i < _dt.Columns.Count; i++)
            {
               
                sumtong = 0;
                if (_dt.Columns[i].ColumnName.Contains("@"))
                {
                    _valueDemSize++;
                    //BandedGridView bandedView = new BandedGridView();
                    string[] arrSize = _dt.Columns[i].ColumnName.Split(new char[] { '@' });
                    string size = arrSize[1];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = size;
                    col.FieldName = "Size@" + size ;
                    col.Name = "col" + size;
                    col.OptionsColumn.AllowEdit = true;
                    sumtong += Convert.ToInt32(_dt.Rows[0][_dt.Columns[i].ColumnName]);
                    //col.ColumnEdit = this.repotxtN0;
                    col.Visible = true;
                    col.Width = 600;
                    //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, "Size@" + size, "{ 0:n0}");
                    if (col.FieldName.Contains("@"))
                    {
                        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                        itemSize.FieldName = col.FieldName;
                        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        itemSize.DisplayFormat = "{0:n0}";
                        itemSize.ShowInGroupColumnFooter = col;
                        bgrViewSize.GroupSummary.Add(itemSize);
                    }

                    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrName.Length > 0)
                    {
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    }

                    bgrViewSize.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                    gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Bold);
                    gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                    gb.AppearanceHeader.Options.UseBackColor = true;
                    gb.AppearanceHeader.Options.UseFont = true;
                    gb.AppearanceHeader.Options.UseForeColor = true;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.Caption = size;
                    gb.Columns.Add(col);
                    gb.Name = "gb" + "Size@" + size;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;
                    gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });

                   

                }

            }
        }
        private void ClearDataBandAndCol()
        {
            try
            {
                bgrViewSize.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                gbSize.Children.Clear();
                gbSize.Columns.Clear();
                gridControl2.DataSource = new DataTable();

            }
            catch (Exception ex) { };
        }
    }
}
