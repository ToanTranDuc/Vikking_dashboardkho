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
    public partial class frmPackageListXuatHangChiTiet : DevExpress.XtraEditors.XtraForm
    {
        DataTable _tblSource = new DataTable();
        DataTable _tbl = new DataTable();
        public static DataTable tblXH = new DataTable();
        public static int SLThung = 0;
        public static string ThungTemp;
        public frmPackageListXuatHangChiTiet(DataTable dt, DataTable tbl)
        {
            InitializeComponent();
            SLThung = 0;
            tblXH = new DataTable();
            this._tblSource = dt;
            this._tbl = tbl;
            ThungTemp = string.Empty;
            LoadData();
        }
        private void LoadData()
        {
            string[] danhSach = _tbl.Rows[0]["ThungTemp"].ToString().Split(',');
            foreach (DataRow item in _tblSource.Rows)
            {
                if (danhSach.Contains(item["SttThung"].ToString()))
                {
                    item["Chon"] = true;
                    item["IsXuatHang"] = true;
                }
            }
            grcXuatHangDetail.DataSource = _tblSource;
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {


        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            DataTable _tblCopy = grcXuatHangDetail.DataSource as DataTable;
            int index = 0;
            int SttThungDisplayBD = Convert.ToInt32(_tblCopy.Rows[0]["SttThungDisplay"]);
            int SttThungBD = Convert.ToInt32(_tblCopy.Rows[0]["SttThung"]);
            _tbl.Columns.Add("SLXuatTemp", typeof(int));
            foreach (DataRow item in _tblCopy.Rows)
            {
                var SttThung = Convert.ToInt32(item["SttThung"]);
                var SttThungDisplay = Convert.ToInt32(item["SttThungDisplay"]);
                if (Convert.ToBoolean(item["Chon"]) == false && Convert.ToBoolean(item["IsXuatHang"]) != true || Convert.ToBoolean(item["Chon"]) == true && Convert.ToBoolean(item["IsXuatHang"]) == true)
                {
                    SttThungBD = SttThung + 1;
                    SttThungDisplayBD = SttThungDisplay + 1;
                    index++;
                    continue;
                }
                else SLThung += 1;
                int SttThungNext = (_tblCopy.Rows.Count > index + 1 &&
                     Convert.ToBoolean(_tblCopy.Rows[index + 1]["Chon"]) == true)
                     ? Convert.ToInt32(_tblCopy.Rows[index + 1]["SttThung"])
                     : 0;

                if (SttThung + 1 != SttThungNext)
                {
                    DataRow newRow = _tbl.NewRow();
                    var SLThung = SttThung - SttThungBD + 1;
                    newRow.ItemArray = _tbl.Rows[0].ItemArray.Clone() as object[];
                    newRow["TuThung"] = SttThungDisplayBD;
                    newRow["DenThung"] = SttThungDisplay;
                    newRow["SLThung"] = SLThung;
                    newRow["TotalPiece"] = (SLThung) * Convert.ToInt32(item["SoLuongSP"]);
                    newRow["SttThung"] = SttThung;
                    newRow["SttThungMin"] = SttThungBD;
                    newRow["SLXuatTemp"] = SLThung;
                    ThungTemp += "," + KHDongThungLib.LayChuoiThung(Convert.ToInt32(SttThungBD), Convert.ToInt32(SttThung));
                    SttThungBD = SttThung + 1;
                    SttThungDisplayBD = SttThungDisplay + 1;

                    _tbl.Rows.Add(newRow);
                }
                index++;
            }
            tblXH = _tbl.AsEnumerable().Where(x => x["SLXuatTemp"].ToString() != "").CopyToDataTable();
            this.Close();
        }
    }
}
