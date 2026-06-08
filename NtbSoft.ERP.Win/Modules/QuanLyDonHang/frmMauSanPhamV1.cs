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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmMauSanPhamV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string _makh = string.Empty, _mahang = string.Empty, _mamau = string.Empty, _tenmau = string.Empty;
        private Dictionary<string, string> selectRows = new Dictionary<string, string>();
        public frmMauSanPhamV1(string makh, string mahang, string mamau, string tenmau)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._makh = makh;
            this._mahang = mahang;
            this._mamau = mamau;
            this._tenmau = tenmau;
        }
        protected override void OnLoad(EventArgs e)
        {
            List<string> listMaMau = _mamau.Split('|').Select(s => s.Trim()).ToList();
            List<string> listTenMau = _tenmau.Split('|').Select(s => s.Trim()).ToList();
            selectRows = listMaMau.Zip(listTenMau, (maMau, tenMau) => new { maMau, tenMau })
                                                             .ToDictionary(x => x.maMau, x => x.tenMau);
            loadMau();
        }
        private void loadMau()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETMASP&para1={_makh.ToString()}&para2={_mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblMau == null || tblMau.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    return;
                }

                if (!tblMau.Columns.Contains("IsChecked"))
                    tblMau.Columns.Add("IsChecked", typeof(bool));
                if (!tblMau.Columns.Contains("RowIndex"))
                    tblMau.Columns.Add("RowIndex", typeof(int));
                for (int i = 0; i < tblMau.Rows.Count; i++)
                {
                    object value = tblMau.Rows[i]["MaMau"];
                    tblMau.Rows[i]["IsChecked"] = (value != null && selectRows.ContainsKey(value.ToString()));
                    tblMau.Rows[i]["RowIndex"] = i;
                }
                DataView dv = tblMau.DefaultView;
                dv.Sort = "IsChecked DESC, RowIndex ASC";
                tblMau = dv.ToTable();
                gridControl1.DataSource = tblMau;
            }
            checkMau();
        }
        private void checkMau()
        {
            gridView1.SelectionChanged -= gVMau_SelectionChanged;
            gridView1.ClearSelection(); 

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                object value = gridView1.GetRowCellValue(i, "MaMau");
                object key = gridView1.GetRowCellValue(i, "TenMau");

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
                    object maMauObj = view.GetRowCellValue(rowHandle, "MaMau");
                    object tenMauObj = view.GetRowCellValue(rowHandle, "TenMau");

                    if (maMauObj != null && tenMauObj != null)
                    {
                        string maMau = maMauObj.ToString().Trim();
                        string tenMau = tenMauObj.ToString().Trim();
                        if (!selectRows.ContainsValue(maMau))
                        {
                            selectRows.Add(tenMau, maMau);
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

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

    }
}
