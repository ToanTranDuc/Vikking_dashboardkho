using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Modules.ThuVien;
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
    public partial class frmKhoiTaoDMCopy : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public static DataTable _tbl = new DataTable();
        DataRow _dr;
        string _madot = string.Empty, _dot = string.Empty;

        public frmKhoiTaoDMCopy(DataTable tbl, DataRow dr, string madot, string dot)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _tbl = tbl.Copy();
            _dr = dr;
            _madot = madot;
            _dot = dot;

        }
        protected override void OnLoad(EventArgs e)
        {
            loadTXT();
            loadVatTu();
        }
        private void loadTXT()
        {
            txtMaVT.Text = _dr["MaVT"].ToString();
            txtChiTiet.Text = _dr["ChiTiet"].ToString();
            txtTenDVVT.Text = _dr["TenDVVT"].ToString();
            txtKhoVai.Text = _dr["KhoVai"].ToString();
            txtMaMauVT.Text = _dr["MaMauVT"].ToString();
            txtMauVT.Text = _dr["MauVT"].ToString();
            txtTenMau.Text = _dr["TenMau"].ToString();
            txtDinhMucChung.Text = _dr["DinhMucChung"].ToString();
            txtDinhMucHaoHut.Text = _dr["DinhMucHaoHut"].ToString();
            txtGhiChu.Text = _dr["GhiChu"].ToString();
        }
        private void loadVatTu()
        {
     //       var rowsToDelete = _tbl.AsEnumerable().Where(row =>
     //row["MaVTID"].Equals(_dr["MaVTID"]) &&
     //row["MauVTID"].Equals(_dr["MauVTID"]) &&
     //row["KhoVaiID"].Equals(_dr["KhoVaiID"]) &&
     //row["MaNhom"].Equals(_dr["MaNhom"])).ToList();

     //       foreach (var row in rowsToDelete)
     //       {
     //           row.Delete();
     //       }
            gC.DataSource = _tbl;



        }
        private void gV_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            DataRowView rowView = (DataRowView)gV.GetRow(e.ListSourceRow);
            if (rowView == null) return;
            if (
                rowView["MaVTID"].Equals(_dr["MaVTID"]) &&
                rowView["MauVTID"].Equals(_dr["MauVTID"]) &&
                rowView["KhoVaiID"].Equals(_dr["KhoVaiID"]) &&
                rowView["MaNhom"].Equals(_dr["MaNhom"]))
            {
                e.Visible = false; // Ẩn dòng này
                e.Handled = true;
            }
        }
        private void gV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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
            if (info.Column == gridColumn7)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gV.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gV.GetRowLevel(e.RowHandle);
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

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;


            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            DataTable tbl = new DataTable("tblSaveDMC");
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("DinhMuc", typeof(Decimal));
            tbl.Columns.Add("MaDot", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(string));
            tbl.Columns.Add("MaMauID", typeof(string));
            tbl.Columns.Add("TachMau", typeof(bool));
            foreach (int handle in gV.GetSelectedRows())
            {
                if (handle < 0) continue;
                DataRow dr = gV.GetDataRow(handle);
                if (dr != null)
                {
                    dr["DinhMucChung"] = _dr["DinhMucChung"].ToString();
                    dr["TachMau"] = _dr["TachMau"].ToString();
                    dr["IsNeww"] = 1;
                    DataRow newRow = tbl.NewRow();
                    newRow["ID"] = 0;
                    newRow["MaKH"] = dr["MaKH"];
                    newRow["MaHang"] = dr["MaHang"];
                    newRow["MaNhom"] = dr["MaNhom"];
                    newRow["MaVTID"] = dr["MaVTID"];
                    newRow["MauVTID"] = dr["MauVTID"];
                    newRow["KhoVaiID"] = dr["KhoVaiID"];
                    newRow["MaNhomSize"] = "";
                    newRow["MaSize"] = "";
                    newRow["DinhMuc"] = dr["DinhMucChung"].ToString();
                    newRow["MaDot"] = _madot;
                    newRow["Dot"] = _dot;
                    newRow["NguoiTao"] = GlobleData.UserName;
                    newRow["NgayTao"] = null;
                    newRow["MaMauID"] = dr["MaMau"].ToString();

                    tbl.Rows.Add(newRow);
                }

            }
            _tbl.AcceptChanges();
            if(tbl==null||tbl.Rows.Count==0)
            {
                this.DialogResult = DialogResult.OK;
                return;
            }
            // đường dẫn khỏi tạo nhưng gọi đến SP_ERPSIZESP
            string url = $"{URL}KhoiTaoDM/PostCopyVT?makh={_dr["MaKH"].ToString()}&mahang={_dr["MaHang"].ToString()}&manhom={_dr["MaNhom"].ToString()}&mavtid={_dr["MaVTID"].ToString()}&mauvtid={_dr["MauVTID"].ToString()}&khovaiid={_dr["KhoVaiID"].ToString()}&madot={_madot}&mamauid={_dr["MaMau"].ToString()}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;

            this.DialogResult = DialogResult.OK;
        }

       
    }
}