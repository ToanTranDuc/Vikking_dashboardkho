using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_VatTuChonMau : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string _makh = string.Empty, _mahang = string.Empty, _mavtID = string.Empty,_mauvtID=string.Empty ,_mauVT = string.Empty, _nhomVT = string.Empty, _khovaiID=string.Empty,
                       _khachhang = string.Empty, _tenhang = string.Empty, _chitiet = string.Empty,_mausp=string.Empty,_mavt=string.Empty,_manhom=string.Empty,_tenmausp;

        DataTable tblMau = new DataTable();
        DataTable tblSave = new DataTable();
        private Dictionary<string, string> selectRows = new Dictionary<string, string>();
        public frmERP_VatTuChonMau(string makh,string khachhang, string mahang, string nhomvt,string mavtid,string mavt, string chitiet, string mauvt,string mausp,string manhom,string mauvtid,string khovaiid,string tenmausp )
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._makh = makh;
            this._khachhang = khachhang;
            this._mahang = mahang;
            this._nhomVT = nhomvt;
            this._mavtID = mavtid;
            this._mavt = mavt;
            this._chitiet = chitiet;
            this._mauVT = mauvt;
            this._mausp = tenmausp;
            this._tenmausp = mausp ;
            this._manhom = manhom;
            this._mauvtID = mauvtid;
            this._khovaiID = khovaiid;
           
            
        }
        protected override void OnLoad(EventArgs e)
        {
            txtKH.Text = _khachhang;
            txtMaHang.Text = _mahang;
            txtNhom.Text = _nhomVT;
            txtMaVT.Text = _mavt;
            txtChiTiet.Text = _chitiet;
            txtMauVT.Text = _mauVT;
            txtMauSP.Text = _mausp;
       
          
            tblSave = createTableSave();
            List<string> listMaMau = _mausp.Split('|').Select(s => s.Trim()).ToList();
            List<string> listTenMau = _tenmausp.Split('|').Select(s => s.Trim()).ToList();


           
            selectRows = listMaMau.Zip(listTenMau, (maMau,tenMau ) => new { maMau,tenMau   })
                                                             .ToDictionary(x => x.maMau, x => x.tenMau);
            loadMau();
        }
        private DataTable createTableSave()
        {
          
            DataTable tbl = new DataTable("tblSave");
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
            return tbl;
        }
        private void loadMau()
        {
            
            string urlCT = string.Format("{0}?makh={1}&mahang={2}", URL + $"ERPVatTuBOM/GetBangMau", _makh, _mahang);
            string jsonCT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCT); }).Result;
            if (jsonCT != "[]")
            {
                tblMau = JsonConvert.DeserializeObject<DataTable>(jsonCT);
                if (tblMau == null || tblMau.Rows.Count == 0)
                {
                    gCMau.DataSource = null;
                    return;
                }

                if (!tblMau.Columns.Contains("IsChecked"))
                    tblMau.Columns.Add("IsChecked", typeof(bool));
                if (!tblMau.Columns.Contains("RowIndex"))
                    tblMau.Columns.Add("RowIndex", typeof(int));
                for (int i = 0; i < tblMau.Rows.Count; i++)
                {
                    object value = tblMau.Rows[i]["MaMau"];
                    tblMau.Rows[i]["IsChecked"] = (value != null && selectRows.ContainsValue(value.ToString()));
                    tblMau.Rows[i]["RowIndex"] = i;
                }

                // Sort lại: IsChecked DESC, RowIndex ASC
                DataView dv = tblMau.DefaultView;
                dv.Sort = "IsChecked DESC, RowIndex ASC";
                tblMau = dv.ToTable();
                gCMau.DataSource = tblMau;
            }
            checkMau();
        }
        private void checkMau()
        {
            gVMau.SelectionChanged -= gVMau_SelectionChanged;
            gVMau.ClearSelection(); // Xóa các dòng đã chọn trước đó

            for (int i = 0; i < gVMau.RowCount; i++)
            {
                // Lấy giá trị của cột "MaMau"
                object value = gVMau.GetRowCellValue(i, "MaMau");
                object key = gVMau.GetRowCellValue(i, "TenMau");

                if (value != null && selectRows.ContainsValue(value.ToString()))
                {
                    gVMau.SelectRow(i);
                    //selectRows.Add(key.ToString(), value.ToString());
                }
            }
            gVMau.SelectionChanged += gVMau_SelectionChanged;

        }
        private void gVMau_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null) return;


            //object maMauObj = view.GetRowCellValue(e.ControllerRow, "MaMau");
            //object tenMauObj = view.GetRowCellValue(e.ControllerRow, "TenMau");

            //if (maMauObj != null && tenMauObj != null)
            //{
            //    string maMau = maMauObj.ToString().Trim();
            //    string tenMau = tenMauObj.ToString().Trim();

            //    if (e.Action == CollectionChangeAction.Add)
            //    {
                    
            //        if (!selectRows.ContainsValue(maMau))
            //        {
            //            selectRows.Add(tenMau, maMau);
            //        }
            //    }
            //    else if (e.Action == CollectionChangeAction.Remove)
            //    {
                   
            //        if (selectRows.ContainsValue(maMau))
            //        {
            //            selectRows.Remove(tenMau);
            //        }
            //    }


            //}
            GridView view = sender as GridView;
            if (view == null) return;

            // Xóa toàn bộ selectRows trước để đồng bộ lại
            selectRows.Clear();

            // Lấy tất cả các hàng được chọn
            int[] selectedRowHandles = view.GetSelectedRows();
            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle >= 0) // Đảm bảo rowHandle hợp lệ
                {
                    object maMauObj = view.GetRowCellValue(rowHandle, "MaMau");
                    object tenMauObj = view.GetRowCellValue(rowHandle, "TenMau");

                    if (maMauObj != null && tenMauObj != null)
                    {
                        string maMau = maMauObj.ToString().Trim();
                        string tenMau = tenMauObj.ToString().Trim();

                        // Thêm vào selectRows nếu chưa có
                        if (!selectRows.ContainsValue(maMau))
                        {
                            selectRows.Add(tenMau, maMau);
                        }
                    }
                }
            }
            string result = string.Join("|", selectRows.Keys);
            txtMauSP.Text = result;
        }
        public List<KeyValuePair<string, string>> GetSelectedDataAsList()
        {
            return selectRows.Where(x => x.Key != "")
                             .ToList();
        }
        private void btnXacNhan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}