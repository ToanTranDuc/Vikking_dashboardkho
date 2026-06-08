using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
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
    public partial class frmBarcodeView_TK : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        //List<DanhSachPhieuXHEntity> lstPhieuXH = new List<DanhSachPhieuXHEntity>();
        List<QRCodeInfoEntity> _lstQrCode = new List<QRCodeInfoEntity>();
        private HashSet<DataRow> selectedRows = new HashSet<DataRow>();
        private HashSet<DataRow> selectedRows_Size = new HashSet<DataRow>();
        private HashSet<DataRow> selectedRows_PKL = new HashSet<DataRow>();
        private string _poid, _maPKL, _size;
        private DataTable dtLine_Lenh = new DataTable();
        string _maDH = "", _line = "", _lenhSX = "";
        bool FlagKK = false;
        List<QRCodeInfoEntity> lstQRCode;
        public frmBarcodeView_TK(List<QRCodeInfoEntity> lstData, string maDH = "",bool flagKK =false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _lstQrCode = lstData;
           // Init();
            _maDH = maDH;
            if (flagKK)
            {
                //XtraQRCodePhieuXHV3 rpt = new XtraQRCodePhieuXHV3(flagKK);
                XtraQRCodePhieuXHV1_Kho rpt = new XtraQRCodePhieuXHV1_Kho(flagKK);
                rpt.DataSource = _lstQrCode;
                documentViewer1.DocumentSource = rpt;
                //documentViewer1.PrintingSystem = rpt.PrintingSystem;
                rpt.CreateDocument();
            }
            else
            {               
                XtraQRCodePhieuXHV1_Kho rpt = new XtraQRCodePhieuXHV1_Kho(flagKK);
                rpt.DataSource = _lstQrCode;
                documentViewer1.DocumentSource = rpt;
                //documentViewer1.PrintingSystem = rpt.PrintingSystem;
                rpt.CreateDocument();
            }
          
            //LoadLine_LSX();
            //LoadPO();
            //LoadData();
        }
        private void Init()
        {
            //ribbonControl1.PrintingSystem = ((DevExpress.XtraReports.UI.XtraReport)documentViewer1.DocumentSource).PrintingSystem;
            grvPO.ColumnFilterChanged += GrvPO_ColumnFilterChanged;
            grvPO.SelectionChanged += GrvPO_SelectionChanged;
            searchPO.CustomDisplayText += SearchPO_CustomDisplayText;

            grvSize.ColumnFilterChanged += GrvSize_ColumnFilterChanged;
            grvSize.SelectionChanged += GrvSize_SelectionChanged;
            searchSize.CustomDisplayText += SearchSize_CustomDisplayText;

            grvMaPKL.ColumnFilterChanged += GrvMaPKL_ColumnFilterChanged;
            grvMaPKL.SelectionChanged += GrvMaPKL_SelectionChanged;
            searchPKL.CustomDisplayText += SearchPKL_CustomDisplayText;
            searchPKL.EditValueChanged += SearchPKL_EditValueChanged;

            searchLookUpEdit_Line.EditValueChanged += SearchLookUpEdit_Line_EditValueChanged;
            searchLookUpEdit_Lenh.EditValueChanged += SearchLookUpEdit_Lenh_EditValueChanged;
            searchPO.Properties.ValueMember = "POID";
            searchPO.Properties.DisplayMember = "PO";
            searchPKL.Properties.ValueMember = "MaPKL";
            searchPKL.Properties.DisplayMember = "MaPKLView";
            searchSize.Properties.ValueMember = "SizeID";
            searchSize.Properties.DisplayMember = "Size";

            searchLookUpEdit_Line.Properties.ValueMember = "DepID";
            searchLookUpEdit_Line.Properties.DisplayMember = "Name";
            searchLookUpEdit_Lenh.Properties.ValueMember = "MaLenh";
            searchLookUpEdit_Lenh.Properties.DisplayMember = "MaLenhDisplay";
        }

        private void SearchPKL_EditValueChanged(object sender, EventArgs e)
        {
            if (searchPKL.EditValue != null)
            {
                _maPKL = searchPKL.EditValue.ToString();
                LoadSize();
                Console.WriteLine(_maPKL);
            }
        }

        private void SearchPKL_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuessPKL = string.Join("; ", grvMaPKL.GetSelectedRows().Select(rowHandle => grvMaPKL.GetRowCellValue(rowHandle, searchPKL.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessPKL.ToString()))
            {
                e.DisplayText = "--Chọn PKL--";
            }
            else
            {
                e.DisplayText = selectedValuessPKL.ToString();
            }
        }

        private void GrvMaPKL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);

                if (view.IsRowSelected(rowHandle3))
                {
                    selectedRows_PKL.Add(row);

                }
                else
                {
                    selectedRows_PKL.Remove(row);
                }
            }
            var selectedValuesPKL = string.Join(";", grvMaPKL.GetSelectedRows().Select(rowHandle => grvMaPKL.GetRowCellValue(rowHandle, searchPKL.Properties.ValueMember)));
            searchPKL.EditValue = selectedValuesPKL;
            if (searchPKL.EditValue != null)
            {
                _maPKL = searchPKL.EditValue.ToString();
                LoadSize();
                Console.WriteLine(_maPKL);
            }
        }

        private void GrvMaPKL_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows_PKL.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void SearchLookUpEdit_Line_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Line.EditValue is null)
            {
                _line = "";
                _lenhSX = "";
                return;
            }
            _line = searchLookUpEdit_Line.EditValue.ToString();
            LoadLenhSX();
        }
        private void SearchLookUpEdit_Lenh_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Lenh.EditValue is null)
            {
                _lenhSX = "";
                return;
            }
            _lenhSX = searchLookUpEdit_Lenh.EditValue.ToString();
            LoadPKL();
            //LoadSize();
        }


        private void SearchSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuessSize = string.Join("; ", grvSize.GetSelectedRows().Select(rowHandle => grvSize.GetRowCellValue(rowHandle, searchSize.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessSize.ToString()))
            {
                e.DisplayText = "--Chọn Size--";
            }
            else
            {
                e.DisplayText = selectedValuessSize.ToString();
            }
        }

        private void GrvSize_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);

                if (view.IsRowSelected(rowHandle3))
                {
                    selectedRows_Size.Add(row);

                }
                else
                {
                    selectedRows_Size.Remove(row);
                }
            }
            var selectedValuesSize = string.Join(";", grvSize.GetSelectedRows().Select(rowHandle => grvSize.GetRowCellValue(rowHandle, searchSize.Properties.ValueMember)));
            searchSize.EditValue = selectedValuesSize;
            if (searchSize.EditValue != null)
            {
                _size = searchSize.EditValue.ToString();
                Console.WriteLine(_size);
                LoadDataPrint();
            }
        }

        private void GrvSize_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows_Size.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }
        private void LoadDataPrint()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetQRCode&MaDH={_maDH}&MaDVSX={_lenhSX}&DotSX={_line}&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID={_size}&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtDataQRCode = JsonConvert.DeserializeObject<DataTable>(json);
            lstQRCode = new List<QRCodeInfoEntity>();
            lstQRCode = dtDataQRCode.AsEnumerable().Select(x => new QRCodeInfoEntity
            {
                Line = x["Line"].ToString(),
                MaLenh = x["MaLenh"].ToString(),
                Season = x["Season"].ToString(),
                NgaySX = x["NgaySX"].ToString(),
                Style = x["TenHang"].ToString(),
                PO = x["PO"].ToString(),
                ModelCode = x["ModelCode"].ToString(),
                Color = x["TenMau"].ToString(),
                Size = x["Size"].ToString(),
                PCB = x["PCB"].ToString(),
                CTN = x["SoThung"].ToString(),
                QRCode = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(x["QRCode"].ToString()))
            }).ToList();
            XtraQRCodePhieuXHV3 rpt = new XtraQRCodePhieuXHV3(FlagKK);
            rpt.DataSource = lstQRCode;
            documentViewer1.DocumentSource = rpt;
            //documentViewer1.PrintingSystem = rpt.PrintingSystem;
            rpt.CreateDocument();
        }
        private DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PO", typeof(string));
            var dr = dt.NewRow();
            dr["PO"] = "PO1";
            dt.Rows.Add(dr);
            var drA = dt.NewRow();
            drA["PO"] = "PO2";
            dt.Rows.Add(drA);
            return dt;
        }
        private void LoadLine_LSX()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetDep_LenhSX_TN&MaDH={_maDH}&MaDVSX=A&DotSX=A&POID=&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtLine_Lenh = JsonConvert.DeserializeObject<DataTable>(json);
            var dtLine = dtLine_Lenh.AsEnumerable().Select(x => new { DepID = x["DepID"].ToString(), Name = x["Name"].ToString() }).Distinct().ToList();
            searchLookUpEdit_Line.Properties.DataSource = dtLine;
        }
        private void LoadLenhSX()
        {
            if (searchLookUpEdit_Line.EditValue is null) return;
            var dtLenhSX = dtLine_Lenh.AsEnumerable().Where(x => x["DepID"].ToString() == searchLookUpEdit_Line.EditValue.ToString());
            searchLookUpEdit_Lenh.Properties.DataSource = dtLenhSX.Count() > 0 ? dtLenhSX.CopyToDataTable() : new DataTable();
        }
        private void LoadPO()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPO&MaDH={_maDH}&MaDVSX=A&DotSX=A&POID=&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPO = JsonConvert.DeserializeObject<DataTable>(json);
            searchPO.Properties.DataSource = dtPO;
        }
        private void LoadPKL()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPKL_QRCode&MaDH={_maDH}&MaDVSX=A&DotSX={_lenhSX}&POID={_poid ?? ""}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtSize = JsonConvert.DeserializeObject<DataTable>(json);
            searchPKL.Properties.DataSource = dtSize;
        }
        private void LoadSize()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetSize_QRCode&MaDH={_maDH}&MaDVSX=A&DotSX={_lenhSX}&POID={_poid ?? ""}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtSize = JsonConvert.DeserializeObject<DataTable>(json);
            searchSize.Properties.DataSource = dtSize;
        }
        private void GrvPO_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);

                if (view.IsRowSelected(rowHandle3))
                {
                    selectedRows.Add(row);

                }
                else
                {
                    selectedRows.Remove(row);
                }
            }
            var selectedValuesPO = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchPO.Properties.ValueMember)));
            searchPO.EditValue = selectedValuesPO;
            if (searchPO.EditValue != null)
            {
                _poid = searchPO.EditValue.ToString();
                LoadSize();
                Console.WriteLine(_poid);
            }


        }
        private void GrvPO_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }
        private void SearchPO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuessPO = string.Join("; ", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchPO.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessPO.ToString()))
            {
                e.DisplayText = "--Chọn PO--";
            }
            else
            {
                e.DisplayText = selectedValuessPO.ToString();
            }
        }

        public void LoadData()
        {
            XtraQRCodePhieuXHV3 rpt = new XtraQRCodePhieuXHV3();
            rpt.DataSource = _lstQrCode;
            documentViewer1.PrintingSystem = rpt.PrintingSystem;
            rpt.CreateDocument();

        }

        private void btnNapLai_Click(object sender, EventArgs e)
        {
            if (_size == "") return;
            LoadDataPrint();
        }

        private void btnGhiChu_Click(object sender, EventArgs e)
        {
            foreach (var item in lstQRCode)
            {
                item.GhiChu = txtGhiChu.Text;
            }
            XtraQRCodePhieuXHV3 rpt = new XtraQRCodePhieuXHV3();
            rpt.DataSource = lstQRCode;
            documentViewer1.DocumentSource = rpt;
            rpt.CreateDocument();
        }

        private void btnChangeC_Click(object sender, EventArgs e)
        {
            //frmChangeColorBarCode frm = new frmChangeColorBarCode(lstQRCode);
            //frm.ShowDialog();
            //if (frmChangeColorBarCode.checkFlag)
            //{
            //    DataTable tbl = frmChangeColorBarCode.tbl;
            //    foreach (var item in lstQRCode)
            //    {
            //        var rowTbl = tbl.AsEnumerable().FirstOrDefault(x => x["TenMau"].ToString() == item.Color);
            //        if (rowTbl != null)
            //        {
            //            item.Color = rowTbl["TenMauChange"].ToString();
            //            item.QRCode = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(item.QRCode.ToString()));
            //        }
            //        else item.QRCode = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(item.QRCode.ToString()));
            //    }
            //    XtraQRCodePhieuXHV3 rpt = new XtraQRCodePhieuXHV3();
            //    rpt.DataSource = lstQRCode;
            //    documentViewer1.DocumentSource = rpt;
            //    rpt.CreateDocument();
            //}

        }
        private void grvPhieuXH_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //DanhSachPhieuXHEntity rowFocus = (DanhSachPhieuXHEntity)grvPhieuXH.GetFocusedRow();
            //if (rowFocus == null) return;
            //if (e.Action == CollectionChangeAction.Add)
            //{
            //    if (_lstQrCode.Exists(x => x == rowFocus)) return;
            //    _lstQrCode.Add(rowFocus);
            //}
            //else if (e.Action == CollectionChangeAction.Remove)
            //{
            //    DanhSachPhieuXHEntity rowRemove = _lstQrCode.Where(x => x.MaPhieu == rowFocus.MaPhieu).FirstOrDefault();
            //    if (rowRemove != null) _lstQrCode.Remove(rowRemove);
            //}
            //XtraQRCodePhieuXH rpt = new XtraQRCodePhieuXH();
            //rpt.DataSource = _lstQrCode;
            //documentViewer1.PrintingSystem = rpt.PrintingSystem;
            //rpt.CreateDocument();
        }

        private void btnChiaChuyen_Click(object sender, EventArgs e)
        {
            //frmInit_Line_LenhSX_KHDongThung frm = new frmInit_Line_LenhSX_KHDongThung(_maDH);
            //frm.ShowDialog();
            //LoadPKL();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //grvPhieuXH.ClearSelection();
            //_lstQrCode = new List<DanhSachPhieuXHEntity>();
            //XtraQRCodePhieuXH rpt = new XtraQRCodePhieuXH();
            //rpt.DataSource = _lstQrCode;
            //documentViewer1.PrintingSystem = rpt.PrintingSystem;
            //rpt.CreateDocument();
        }
    }
}
