using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmMaHangV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string _makh = string.Empty, _mahang = string.Empty, _tenhang = string.Empty;
        private Dictionary<string, string> selectRows = new Dictionary<string, string>();
        public frmMaHangV1(string makh, string mahang)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._makh = makh;
            this._mahang = mahang;
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        protected override void OnLoad(EventArgs e)
        {
            List<string> listMaHang = _mahang.Split('|').Select(s => s.Trim()).ToList();
            selectRows = listMaHang.ToDictionary(maHang => maHang);
            loadHangHoa();
        }


        private void loadHangHoa()
        {
            string urlmh = string.Format("{0}?makh={1}", URL + "ERPVatTuBOM/GetMH", _makh);
            string jsonmh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmh); }).Result;
            if (jsonmh != "[]")
            {
                DataTable tblhh = JsonConvert.DeserializeObject<DataTable>(jsonmh);
                if (tblhh == null || tblhh.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    return;
                }

                if (!tblhh.Columns.Contains("IsChecked"))
                    tblhh.Columns.Add("IsChecked", typeof(bool));
                if (!tblhh.Columns.Contains("RowIndex"))
                    tblhh.Columns.Add("RowIndex", typeof(int));
                for (int i = 0; i < tblhh.Rows.Count; i++)
                {
                    object value = tblhh.Rows[i]["MaHang"];
                    tblhh.Rows[i]["IsChecked"] = (value != null && selectRows.ContainsKey(value.ToString()));
                    tblhh.Rows[i]["RowIndex"] = i;
                }
                DataView dv = tblhh.DefaultView;
                dv.Sort = "IsChecked DESC, RowIndex ASC";
                tblhh = dv.ToTable();
                gridControl1.DataSource = tblhh;
            }
            checkMau();
        }


        private void checkMau()
        {
            gridView1.SelectionChanged -= gVMau_SelectionChanged;
            gridView1.ClearSelection();

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                object value = gridView1.GetRowCellValue(i, "MaHang");
                object key = gridView1.GetRowCellValue(i, "TenHang");

                if (value != null && selectRows.ContainsKey(value.ToString()))
                {
                    gridView1.SelectRow(i);
                }
            }
            gridView1.SelectionChanged += gVMau_SelectionChanged;

        }
        private void gVMau_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            selectRows.Clear();
            int[] selectedRowHandles = view.GetSelectedRows();
            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle >= 0)
                {
                    object maHangObj = view.GetRowCellValue(rowHandle, "MaHang");
                    object tenHangObj = view.GetRowCellValue(rowHandle, "TenHang");

                    if (maHangObj != null && tenHangObj != null)
                    {
                        string maHh = maHangObj.ToString().Trim();
                        string tenHh = tenHangObj.ToString().Trim();
                        if (!selectRows.ContainsValue(maHh))
                        {
                            selectRows.Add(tenHh, maHh);
                        }
                    }
                }
            }
        }

        public List<KeyValuePair<string, string>> GetSelectedDataAsList()
        {
            return selectRows.Where(x => x.Key != "")
                             .ToList();
        }
    }
}