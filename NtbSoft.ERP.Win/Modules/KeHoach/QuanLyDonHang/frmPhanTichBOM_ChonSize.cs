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
    public partial class frmPhanTichBOM_ChonSize : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string _makh = string.Empty, _mahang = string.Empty,_masizechung = string.Empty;
        //private Dictionary<string, string> selectRows = new Dictionary<string, string>();
        public string SelectedResult { get; set; }
        public string SelectedResultName { get; set; }
        public frmPhanTichBOM_ChonSize(string makh, string mahang, string masizechung)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._makh = makh;
            this._mahang = mahang;
            this._masizechung = masizechung;
           
        }
        protected override void OnLoad(EventArgs e)
        {
            //List<string> listMaMau = _mamau.Split('|').Select(s => s.Trim()).ToList();
            //List<string> listTenMau = _tenmau.Split('|').Select(s => s.Trim()).ToList();
            //selectRows = listMaMau.Zip(listTenMau, (maMau, tenMau) => new { maMau, tenMau })
            //                                                 .ToDictionary(x => x.maMau, x => x.tenMau);
            loadSize();
        }
        private void loadSize()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP&para1={_makh.ToString()}&para2={_mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblMau == null || tblMau.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    return;
                }

                //if (!tblMau.Columns.Contains("IsChecked"))
                //    tblMau.Columns.Add("IsChecked", typeof(bool));
                //if (!tblMau.Columns.Contains("RowIndex"))
                //    tblMau.Columns.Add("RowIndex", typeof(int));
                //for (int i = 0; i < tblMau.Rows.Count; i++)
                //{
                //    object value = tblMau.Rows[i]["MaMau"];
                //    tblMau.Rows[i]["IsChecked"] = (value != null && selectRows.ContainsKey(value.ToString()));
                //    tblMau.Rows[i]["RowIndex"] = i;
                //}
               
                gridControl1.DataSource = tblMau;
            }
            checkSize();
        }
        private void checkSize()
        {
            gridView1.SelectionChanged -= gVMau_SelectionChanged;
            gridView1.ClearSelection();

            var mapping = ParseMaSizeChung(_masizechung);
            gridView1.BeginUpdate();
            gridView1.ClearSelection();
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                var maNhomSize = Convert.ToString(gridView1.GetRowCellValue(i, "MaNhomSize"));
                var maSize = Convert.ToString(gridView1.GetRowCellValue(i, "MaSize"));
                if (mapping.TryGetValue(maNhomSize, out var sizeSet) && sizeSet.Contains(maSize))
                    gridView1.SelectRow(i);
            }
            gridView1.EndUpdate();
            gridView1.SelectionChanged += gVMau_SelectionChanged;

        }
        Dictionary<string, HashSet<string>> ParseMaSizeChung(string source)
        {
            var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            var groups = source.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var group in groups)
            {
                var parts = group.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) continue;
                var key = parts[0].Trim();
                var sizes = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!result.TryGetValue(key, out var set))
                {
                    set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    result[key] = set;
                }
                foreach (var s in sizes)
                    set.Add(s.Trim());
            }
            return result;
        }
        private void gVMau_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null) return;
        
            //int[] selectedRowHandles = view.GetSelectedRows();
            //foreach (int rowHandle in selectedRowHandles)
            //{
            //    if (rowHandle >= 0) 
            //    {
            //        object maMauObj = view.GetRowCellValue(rowHandle, "MaMau");
            //        object tenMauObj = view.GetRowCellValue(rowHandle, "TenMau");

            //        if (maMauObj != null && tenMauObj != null)
            //        {
            //            string maMau = maMauObj.ToString().Trim();
            //            string tenMau = tenMauObj.ToString().Trim();
                        
            //        }
            //    }
            //}
        }

       

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SelectedResult = BuildResultStringLinq(gridView1);
            SelectedResultName = BuildResultStringLinqName(gridView1);
            this.DialogResult = DialogResult.OK;
        }
        string BuildResultStringLinq(GridView view)
        {
            var handles = view.GetSelectedRows()
                .Where(h => h >= 0)
                .Select(h => new
                {
                    MaNhomSize = Convert.ToString(view.GetRowCellValue(h, "MaNhomSize")),
                    MaSize = Convert.ToString(view.GetRowCellValue(h, "MaSize"))
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.MaNhomSize) && !string.IsNullOrWhiteSpace(x.MaSize))
                .GroupBy(x => x.MaNhomSize, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Key + ":" + string.Join(",", g.Select(x => x.MaSize).Distinct(StringComparer.OrdinalIgnoreCase)))
                .ToArray();

            return string.Join("; ", handles);
        }
        string BuildResultStringLinqName(GridView view)
        {
            var handles = view.GetSelectedRows()
                .Where(h => h >= 0)
                .Select(h => new
                {
                    MaNhomSize = Convert.ToString(view.GetRowCellValue(h, "NhomSize")),
                    MaSize = Convert.ToString(view.GetRowCellValue(h, "TenSize"))
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.MaNhomSize) && !string.IsNullOrWhiteSpace(x.MaSize))
                .GroupBy(x => x.MaNhomSize, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Key + ":" + string.Join(",", g.Select(x => x.MaSize).Distinct(StringComparer.OrdinalIgnoreCase)))
                .ToArray();

            return string.Join("; ", handles);
        }
    }
}
