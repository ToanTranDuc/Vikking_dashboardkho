using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmCopyERPKHDongThung : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        string maDHS = "",maDHD="", poIdS = "",poIdD="";
        bool flagCopy = false;
        public frmCopyERPKHDongThung()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            searchLookUpEditDonHangS.EditValueChanged += SearchLookUpEditDonHangS_EditValueChanged;
            searchLookUpEditDonHangD.EditValueChanged += SearchLookUpEditDonHangD_EditValueChanged;
            Init();
            LoadDonHang();
        }

        private void SearchLookUpEditDonHangD_EditValueChanged(object sender, EventArgs e)
        {
            if (flagCopy)
            {
                DialogResult result = MessageBox.Show("Dữ liệu copy ở trước. Bạn có muốn lưu lại không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    ConfirmCopyPaste();
                }
                else
                {
                    CancelCopyPaste();
                }
            }

            if (searchLookUpEditDonHangD.EditValue is null || searchLookUpEditDonHangD.EditValue.ToString() == "") return;
            poIdD = "";
            int focus = grvDonHangD.FocusedRowHandle;
            var tblDes = LoadCTDonHang(searchLookUpEditDonHangD.EditValue.ToString());
            grcDonHangDes.DataSource = tblDes;
            grcDonHangDes.RefreshDataSource();     
            if(focus == 0 && tblDes.Rows.Count > 0)
            {
                //poIdD = tblDes.Rows[0]["POID"].ToString();
                DHDChange(0);
            }
        }

        private void SearchLookUpEditDonHangS_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditDonHangS.EditValue is null || searchLookUpEditDonHangS.EditValue.ToString() == "") return;
            poIdS = "";
            int focus = grvDonHang.FocusedRowHandle;
            var tblSource = LoadCTDonHang(searchLookUpEditDonHangS.EditValue.ToString());
            grcDonHangSource.DataSource = tblSource;
            grcDonHangSource.RefreshDataSource();
            if (focus == 0 && tblSource.Rows.Count > 0)
            {
                //poIdS = tblSource.Rows[0]["POID"].ToString();
                DHChange(0);
            }
        }
        private void Init()
        {
            searchLookUpEditDonHangS.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEditDonHangS.Properties.ValueMember = "MaDH";
            searchLookUpEditDonHangS.Properties.NullText = "[Chọn đơn hàng]";

            searchLookUpEditDonHangD.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEditDonHangD.Properties.ValueMember = "MaDH";
            searchLookUpEditDonHangD.Properties.NullText = "[Chọn đơn hàng]";

            searchLookUpEdit_MaPKLS.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKLS.Properties.ValueMember = "MaPKL";
            searchLookUpEdit_MaPKLS.Properties.NullText = "[Chọn PKL]";

            searchLookUpEdit_MaPKLD.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKLD.Properties.ValueMember = "MaPKL";
            searchLookUpEdit_MaPKLD.Properties.NullText = "[Chọn PKL]";
        }
        private void LoadDonHang()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetDH&MaDH=DH00000064&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var _dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDonHangS.Properties.DataSource = _dtDonHang;
            searchLookUpEditDonHangD.Properties.DataSource = _dtDonHang;
           
           // return _dtDonHang;
        }
        private DataTable LoadCTDonHang(string maDH,string action = "GetCTDH")
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={action}&MaDH={maDH}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var _dtCTDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            if (_dtCTDonHang is null) return new DataTable();
            return _dtCTDonHang;
            var rowfocus = grvDonHang.FocusedRowHandle;
        }
        private DataTable LoadPKL(string maDH,string poid,string action = "GetDotLapPKL")
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={action}&MaDH={maDH}&MaDVSX=Para&DotSX=Para&POID={poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var _dtCTDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            if (_dtCTDonHang is null) return new DataTable();
            return _dtCTDonHang;
        }
        private DataTable LoadKHDongThung(string maDH,string poid,string maPKL,string codeCopy = "Para")
        {            
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung&MaDH={maDH}&MaDVSX=Para&DotSX=Para&POID={poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID={codeCopy}&MaPKL={maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtDataPiVot = JsonConvert.DeserializeObject<DataTable>(json);
           

            DataTable tbl = new DataTable();
            tbl = dtDataPiVot;
            KHDongThungLib.ProcessSttTrung(tbl);
            return tbl;
            //if (tbl == null || tbl.Rows.Count == 0)
            //    dgrKHDongThung.DataSource = new DataTable();
            //else
            //{
            //    DataTable dt = new DataTable();                
            //    CreateBandForSize(tbl);
            //    dgrKHDongThung.DataSource = tbl;
                //DataTable processedData = KHDongThungLib.sumToTalPCS(tbl);
                //KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
                //dgcTong.DataSource = processedData;
                //dgrKHDongThung.RefreshDataSource();
                //KHDongThungLib.AllowVieworNotPackV2(tbl, BandMaHang, BandDot);
                //KHDongThungLib.AllowVieworNotPack(tbl, BandPCB, BandPack, BandStore);
            //}
            //DataTable tbl = GetPivotKHDongThung();
            //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
            //btnLapKeHoachNhanh.Enabled = true;
        }
        private void CreateBandForSize(DataTable dt,GridBand gband,BandedGridView bandgrv)
        {
            try
            {
               
                if(gband.Name.ToString() == "gbSize") ClearDataBandAndCol();
                else ClearDataBandAndColD();
                foreach (DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    if (!colName.Contains('@')) continue;
                    var _sizeID = colName.Split('@')[1];
                    var _size = colName.Split('@')[0];
                    if (!CheckExistBand(_sizeID, gband)) continue;
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
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandgrv.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
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
                    gband.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void ClearDataBandAndCol()
        {
            try
            {
                //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                gbSize.Children.Clear();
                dgrKHDongThung.DataSource = new DataTable();
                List<GridColumn> lst = new List<GridColumn>();
                lst = bandedGridViewKHDT.Columns.Where(x => x.FieldName.Contains(@"Size@")).ToList();
                if (lst != null && lst.Count > 0)
                    foreach (GridColumn gc in lst) bandedGridViewKHDT.Columns.Remove(gc);
            }
            catch (Exception ex) { };
        }
        private void ClearDataBandAndColD()
        {
            try
            {
                //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                gbSizeD.Children.Clear();
                dgrKHDongThungD.DataSource = new DataTable();
                List<GridColumn> lst = new List<GridColumn>();
                lst = bandedGridViewKHDTD.Columns.Where(x => x.FieldName.Contains(@"Size@")).ToList();
                if (lst != null && lst.Count > 0)
                    foreach (GridColumn gc in lst) bandedGridViewKHDTD.Columns.Remove(gc);
            }
            catch (Exception ex) { };
        }
        private bool CheckExistBand(string size, GridBand gband)
        {
            GridBand gbCheck = gband.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private void btnCopyAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CopyData();
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            CopyData();
        }
        private void CopyData()
        {
            //if (flagCopy) return;
            if (searchLookUpEditDonHangD.EditValue is null || searchLookUpEditDonHangD.EditValue.ToString() == "")
            {
                MessageBox.Show("Vui lòng chọn đơn hàng nguồn!", "Thông báo");
                return;
            }

            if (searchLookUpEditDonHangS.EditValue is null || searchLookUpEditDonHangS.EditValue.ToString() == "")
            {
                MessageBox.Show("Vui lòng chọn đơn hàng copy!", "Thông báo");
                return;
            }
            DialogResult dialogresult = MessageBox.Show("Xác nhận copy đơn hàng!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogresult != DialogResult.Yes) return;
            var dtSource = grcDonHangSource.DataSource as DataTable;
            var dtCopy = grcDonHangDes.DataSource as DataTable;
            var dtSave = KHDongThungLib.CreateTblSave();
            var dtCopyMaLenh = CreateTblCopy();
            foreach (DataRow dr in dtSource.Rows)
            {
                var drCheck = dtCopy.AsEnumerable().Where(x => x["POID"].ToString() == dr["POID"].ToString() &&
                                                        x["MaDVSX"].ToString() == dr["MaDVSX"].ToString() &&
                                                        x["DotSX"].ToString() == dr["DotSX"].ToString() &&
                                                        x["SLKH"].ToString() == dr["SLKH"].ToString()&&
                                                        dr["SLDaLapPKL"].ToString() != "0").FirstOrDefault();
                if (drCheck is null) continue;
                var drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaDH"] = dr["MaDH"];
                drNew["POID"] = dr["POID"];
                drNew["MaLenh"] = dr["MaLenh"];
                dtSave.Rows.Add(drNew);
                var drNewML = dtCopyMaLenh.NewRow();
                drNewML["MaLenhSource"] = dr["MaLenh"];
                drNewML["MaLenhDes"] = drCheck["MaLenh"];
                dtCopyMaLenh.Rows.Add(drNewML);
            }
            if (dtSave.Rows.Count == 0)
            {
                MessageBox.Show("Không có đơn vị, số lượng trùng khớp để copy!", "Thông báo");
                return;
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dtSave);
            ds.Tables.Add(dtCopyMaLenh);
            //return;
            string url = string.Format("{0}", URL + $"KeHoachDongThung/CopyPaste?action=CopyPasteKHDongThung&MaDHDes={searchLookUpEditDonHangD.EditValue.ToString()}&MaPKL=Para");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
            if (result.ToLower() == "true")
            {
                flagCopy = true;
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            grcDonHangDes.DataSource = LoadCTDonHang(searchLookUpEditDonHangD.EditValue.ToString(), "GetCTDH_Copy");
            searchLookUpEdit_MaPKLD.Properties.DataSource = LoadPKL(searchLookUpEditDonHangD.EditValue.ToString(), poIdD, "GetDotLapPKL_Copy");
            //LoadPKL(searchLookUpEditDonHangD.EditValue.ToString(), poIdD);
        }
        private void searchLookUpEdit_MaPKLS_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MaPKLS.EditValue is null || searchLookUpEdit_MaPKLS.EditValue.ToString() == "") return;
            var tbl = LoadKHDongThung(searchLookUpEditDonHangS.EditValue.ToString(), poIdS, searchLookUpEdit_MaPKLS.EditValue.ToString());
            if (tbl == null || tbl.Rows.Count == 0)
                dgrKHDongThung.DataSource = new DataTable();
            else
            {
                DataTable dt = new DataTable();
                CreateBandForSize(tbl,gbSize,bandedGridViewKHDT);
                dgrKHDongThung.DataSource = tbl;                
                dgrKHDongThung.RefreshDataSource();
               
            }
        }
        private void searchLookUpEdit_MaPKLD_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MaPKLD.EditValue is null || searchLookUpEdit_MaPKLD.EditValue.ToString() == "") return;
            var tbl = LoadKHDongThung(searchLookUpEditDonHangD.EditValue.ToString(), poIdD, searchLookUpEdit_MaPKLD.EditValue.ToString(), flagCopy ? "Copy" : "Para");
            if (tbl == null || tbl.Rows.Count == 0)
                dgrKHDongThungD.DataSource = new DataTable();
            else
            {
                DataTable dt = new DataTable();
                CreateBandForSize(tbl, gbSizeD,bandedGridViewKHDTD);
                dgrKHDongThungD.DataSource = tbl;
                dgrKHDongThungD.RefreshDataSource();

            }
        }

        private void bandedGridViewKHDT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }
        private void bandedGridViewKHDTD_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }
        private void frmCopyERPKHDongThung_FormClosing(object sender, FormClosingEventArgs e)
        {
            //CancelCopyPaste();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Xác nhận hủy copy?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                CancelCopyPaste();
            }           
        }
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Xác nhận copy dữ liệu?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ConfirmCopyPaste();
            }           
        }
        private void ConfirmCopyPaste()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/CancelCopyPaste?action=ConfirmCopyPaste&MaDH={searchLookUpEditDonHangD.EditValue.ToString()}&POID={poIdD}");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;

            if (result.ToLower() == "true")
            {
                flagCopy = false;
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
        }
        private string CancelCopyPaste()
        {
            if (searchLookUpEditDonHangD.EditValue is null || searchLookUpEditDonHangD.EditValue.ToString() == "" || !flagCopy) return "false";
            string url = string.Format("{0}", URL + $"KeHoachDongThung/CancelCopyPaste?action=CancelCopyPaste&MaDH={searchLookUpEditDonHangD.EditValue.ToString()}&POID={poIdD}");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if (result.ToLower() == "true")
            {
                flagCopy = false;
                grcDonHangDes.DataSource = LoadCTDonHang(searchLookUpEditDonHangD.EditValue.ToString());
                searchLookUpEdit_MaPKLD.Properties.DataSource = LoadPKL(searchLookUpEditDonHangD.EditValue.ToString(), poIdD);
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            return result.ToLower();            
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            grcDonHangDes.DataSource = LoadCTDonHang(searchLookUpEditDonHangD.EditValue.ToString(), "GetCTDH_Copy");
            searchLookUpEdit_MaPKLD.Properties.DataSource = LoadPKL(searchLookUpEditDonHangD.EditValue.ToString(), poIdD, "GetDotLapPKL_Copy");
        }

        private DataTable CreateTblCopy()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLenhSource", typeof(string));
            dt.Columns.Add("MaLenhDes", typeof(string));
            return dt;
        }

        private void grvDonHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;

            DHChange(e.FocusedRowHandle);
        }

       
        private void DHChange(int rowHandle)
        {
            var tempPOID = grvDonHang.GetRowCellValue(rowHandle, "POID").ToString();
            if (poIdS != tempPOID)
            {
                poIdS = tempPOID;
                searchLookUpEdit_MaPKLS.Properties.DataSource = LoadPKL(searchLookUpEditDonHangS.EditValue.ToString(), poIdS);
            }
        }

        private void grvDonHangD_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            DHDChange(e.FocusedRowHandle);
        }
        private void DHDChange(int rowHandle)
        {
            var tempPOID = grvDonHangD.GetRowCellValue(rowHandle, "POID").ToString();
            if (poIdD != tempPOID)
            {
                poIdD = tempPOID;
                searchLookUpEdit_MaPKLD.Properties.DataSource = LoadPKL(searchLookUpEditDonHangD.EditValue.ToString(), poIdD);
            }
        }
        private void bandedGridViewKHDT_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e);
        }

        private void bandedGridViewKHDT_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
        }

        private void bandedGridViewKHDTD_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e);
        }

        private void bandedGridViewKHDTD_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrKHDongThungD.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
        }

    }
}
