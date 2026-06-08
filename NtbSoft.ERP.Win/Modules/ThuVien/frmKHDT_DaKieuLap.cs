using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmKHDT_DaKieuLap : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;

        List<KHDT_StoreEntity> lstStoreUpdate;
        List<int> lstRowUpdate = new List<int>();
        DataTable dtSize = new DataTable();
        DataTable dtKHDT_Store = new DataTable();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        bool _allowAdd = true, _allowEdit = true, _allowDelete = true;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;
        string MaDH=string.Empty,MaHang = string.Empty; string StyteID = string.Empty; string PO = string.Empty; string POID = string.Empty;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        DataTable dtQuiCach = new DataTable();

        public frmKHDT_DaKieuLap()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstStoreUpdate = new List<KHDT_StoreEntity>();

        }

        protected override void OnLoad(EventArgs e)
        {

            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadMaHang();


        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLai, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
            }
            if (_allowAdd || _allowEdit)
            {
                Luu.Enabled = true;
            }
            else
            {
                Luu.Enabled = false;
            }

            if (!_allowDelete)
                Xoa.Enabled = false;

        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    //grvKHDTStore.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }


                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    //grvKHDTStore.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }


                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    //grvKHDTStore.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }


                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        public void LoadMaHang()
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                DataTable dtMaHang = new DataTable();
                string url = $"{URL}KHDT_DaKieuLap/Get?Action=GetMaHang&Para1=para1&Para2=para2s";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    dtMaHang = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcMaHang.DataSource = dtMaHang;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadKHDT_Store(string StyteID, string POID)
        {
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            string url = $"{URL}KHDT_DaKieuLap/Get?Action=GET&Para1={StyteID}&Para2={POID}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtKHDT_Store.Clear();
            if (!string.IsNullOrEmpty(json))
            {
                dtKHDT_Store = JsonConvert.DeserializeObject<DataTable>(json);
                if (dtKHDT_Store.Rows.Count > 0)
                    InitGrvKHDT_Store(grvKHDTStore);

            }
            grcKHDTStore.DataSource = dtKHDT_Store;
            grcKHDTStore.RefreshDataSource();
        }

        private void LoadSize(string StyteID, string POID)
        {
            dtSize?.Rows?.Clear();
            string url = $"{URL}KHDT_DaKieuLap/Get?Action=GETSize&Para1={StyteID}&Para2={POID}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                dtSize = JsonConvert.DeserializeObject<DataTable>(json);
            }

        }

        private DataTable LoadQuiCach(string maHang)
        {

            string url = $"{URL}KHDT_DaKieuLap/Get?Action=GetQuiCach&Para1={maHang}&Para2=para2";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                dtQuiCach = JsonConvert.DeserializeObject<DataTable>(json);
            }
            return dtQuiCach;
        }

        private void ThemDong()
        {
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);

            DataRow row = dtKHDT_Store.NewRow();
            row["SapXep"] = -1;
            row["IsEdit"] = 1;
            row["ColorID"] = dtKHDT_Store.Rows[0]["ColorID"].ToString();
            row["MaMau"] = dtKHDT_Store.Rows[0]["MaMau"].ToString();
            row["SLThung"] = 1;
            dtKHDT_Store.Rows.InsertAt(row, 0);
            _rowAdd = 0;
            grvKHDTStore.FocusedRowHandle = _rowAdd;
            grcKHDTStore.RefreshDataSource();
        }

        private void btnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                lstStoreUpdate.Clear();
                OpenFileDialog Sfd = new OpenFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    string FilePath = Sfd.FileName;
                    string worksheetName = string.Empty;

                    List<string> lstColurm = new List<string> { "Carton Number ", "PO", "Supplier",
                    "Style \nNO", "Color", "Color number", "QTY" , "Carton", "ToTal \nPieces", "Store Number" , "Column5" , "Column3", "Column2" ,"NW","GW"};
                    using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                    {
                        DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                        worksheetName = worksheetCollection[0].Name;
                    }
                    var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                    source.FileName = FilePath;
                    var worksheetSettings = new ExcelWorksheetSettings(worksheetName);
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                    source.Fill();
                    DataTable dtSave = new DataTable();
                    dtSave = source.ToDataTable();
                    DataTable dtProcess = ProcessDataExcel(dtSave);
                    SaveDataExcel(dtProcess);
                }
            }
            catch (Exception ex) { }

            //ImportData frm = new ImportData();
            //frm.ShowDialog();
            //LoadMaHang();
        }


        private void grvMaHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {


            if (e.FocusedRowHandle >= 0)
            {
                FocusedIndex = e.FocusedRowHandle;
                var row = grvMaHang.GetDataRow(FocusedIndex);

                //MaHang = row[3]?.ToString();
                //StyteID = row[2]?.ToString();
                //PO = row[0]?.ToString();
                //POID = row[1]?.ToString();
                MaDH = row["MaDH"].ToString();
                LoadSize(row["MaDH"].ToString(), POID);
                LoadKHDT_Store(row["MaDH"].ToString(), POID);
                //LoadMHDetail(MaHang);

            }
        }

        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }

        private void InitGrvKHDT_Store(GridView grv)
        {
            RepositoryItemSearchLookUpEdit rQuiCachEdit;
            int idxCol = -1;
            idxCol = (int)dtSize?.Rows?.Count;
            if (grv == null || grv?.Columns?.Count == 0)
            {

                rQuiCachEdit = InitQuiCachEdit();
                grv.Columns.AddRange(new GridColumn[] {

                new GridColumn {
                FieldName = "PO",
                Caption = "PO",
                Name = "colPO",
                Fixed= DevExpress.XtraGrid.Columns.FixedStyle.Left,
                Visible = true,
                Width = 80,
                VisibleIndex = 0
            },
                  new GridColumn {
                FieldName = "DauSize",
                Caption = "Đầu size",
                Name = "colDauSize",
                Fixed= DevExpress.XtraGrid.Columns.FixedStyle.Left,
                Visible = true,
                Width = 80,
                VisibleIndex = 0 },
                new GridColumn {
                FieldName = "MaMau",
                Caption = "Màu",
                Name = "colMaMau",
                Visible = true,
                 Fixed= DevExpress.XtraGrid.Columns.FixedStyle.Left,
                Width = 80,
                VisibleIndex = 1,

            },
                new GridColumn {
                FieldName = "ColorID",
                Caption = "ColorID",
                Name = "colColorID",
                Visible = false,
                Width = 75,
                VisibleIndex = -1
            },
                new GridColumn {
                FieldName = "SapXep",
                Caption = "SapXep",
                Name = "colSapXep",
                Visible = false,
                Fixed= DevExpress.XtraGrid.Columns.FixedStyle.Left,
                Width = 75,
                //SortOrder=DevExpress.Data.ColumnSortOrder.Ascending,
                VisibleIndex = -1
            },
                new GridColumn {
                FieldName = "SLThung",
                Caption = "SL Thùng",
                Name = "colSLThung",
                Visible = true,
                Fixed= DevExpress.XtraGrid.Columns.FixedStyle.Right,
                Width = 80,
                VisibleIndex = idxCol + 3
            },
            //    new GridColumn {
            //    FieldName = "QuiCach",
            //    Caption = "Carton",
            //    Name = "colkiHieu",
            //    ColumnEdit=rQuiCachEdit,
            //    Visible = true,
            //    Fixed= DevExpress.XtraGrid.Columns.FixedStyle.Right,
            //    Width = 75,
            //    VisibleIndex = idxCol + 4,
               
            //},



        });
            }
            else
            {
                rQuiCachEdit = InitQuiCachEdit();
                RemoveDynamicColumn(grv);
            }
            //grv.Columns[1].OptionsColumn.ReadOnly = true;
            //grv.Columns[1].OptionsColumn.AllowEdit = false;

            idxCol = 2;

            foreach (DataRow row in dtSize.Rows)
            {
                GridColumn newColumn = new GridColumn
                {
                    FieldName = $"{row["Size"].ToString()}@{row["SizeID"].ToString()}",
                    Caption = row["Size"].ToString(),
                    Name = $"col{row["SizeID"].ToString()}",
                    Visible = true,
                    Width = 70,
                    VisibleIndex = idxCol,

                };
                grv.Columns.Insert(idxCol, newColumn);
                idxCol++;
            }

        }

        private RepositoryItemSearchLookUpEdit InitQuiCachEdit()
        {
            DataTable dtQuiCach = LoadQuiCach(StyteID);
            RepositoryItemSearchLookUpEdit rQuiCachEdit = new RepositoryItemSearchLookUpEdit();

            rQuiCachEdit.DisplayMember = "KiHieu";
            rQuiCachEdit.ValueMember = "MaQuiCach";
            rQuiCachEdit.ShowClearButton = false;
            rQuiCachEdit.NullText = "[Chọn giá trị]";
            //rQuiCachEdit.EditValueChanged += rQuiCachEdit_EditValueChanged;
            GridView dvView = rQuiCachEdit.View;

            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaQuiCach", Caption = " Mã Qui Cách", Name = "rColMaQuiCach", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenQuiCach", Caption = "Qui Cách", Name = "rColQuiCach", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "KiHieu", Caption = "Carton", Name = "rColKiHieu", Visible = true });
                dvView.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(gridView_CustomDrawColumnHeader);


            }
            rQuiCachEdit.DataSource = dtQuiCach;

            return rQuiCachEdit;
        }


        private void RemoveDynamicColumn(GridView grv)
        {

            List<GridColumn> lstCol = grv?.Columns?.OfType<GridColumn>().ToList();
            foreach (var col in lstCol)
            {
                if (col.FieldName.Contains("@"))
                {
                    grv.Columns.Remove(col);
                }
            }


        }



        private void grvKHDTStore_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            int IdxSapXep = -1;
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);

            if (e.RowHandle >= 0 && e.Column.FieldName == "Store")
            {

                string StoreValue = e.Value?.ToString();
                int MaxIdxSapXep = (int)dtKHDT_Store.AsEnumerable().Max(x => Int32.Parse(x["SapXep"]?.ToString()));
                List<DataRow> query = dtKHDT_Store.AsEnumerable().Where(x => x["Store"]?.ToString()?.Trim() == StoreValue?.Trim()).ToList();
                IdxSapXep = (int)query.Max(x => Int32.Parse(x["SapXep"]?.ToString()));
                int index = e.RowHandle;
                dtKHDT_Store.Rows[index]["SapXep"] = IdxSapXep + 1;
                dtKHDT_Store.Rows[index]["IsEdit"] = 0;


                if (IdxSapXep == MaxIdxSapXep || IdxSapXep == -1)
                {
                    dtKHDT_Store.Rows[index]["SapXep"] = MaxIdxSapXep + 1;
                    dtKHDT_Store.Rows[index]["IsEdit"] = 0;
                }
                else
                {
                    index = dtKHDT_Store.Rows.IndexOf(query.SingleOrDefault(x => Int32.Parse(x["SapXep"]?.ToString()) == IdxSapXep)) + 1;
                    for (int i = index; i <= MaxIdxSapXep; i++)
                    {

                        dtKHDT_Store.Rows[i]["SapXep"] = i;

                    }
                }





            }

        }
        private void NapLai()
        {
            LoadSize(MaDH, POID);
            LoadKHDT_Store(MaDH, POID);
        }

        private void LuuDong()
        {
            this.ActiveControl = this.button1;
            DataTable dtSave = grcKHDTStore.DataSource as DataTable;
            if (dtSave?.Rows?.Count > 0)
            {
                lstStoreUpdate?.Clear();

                foreach (DataRow row in dtSave.Rows)
                {
                    string MaQuiCach = row["QuiCach"].ToString();
                    string Store = row["Store"].ToString();
                    foreach (DataColumn col in dtSave.Columns)
                    {
                        string colName = col.Caption;
                        if (colName.Contains('@'))
                        {
                            if (!Int32.TryParse(row[col].ToString(), out int SLuong))
                            {
                                continue;
                            }
                            else
                            {
                                if (SLuong > 0 && !string.IsNullOrEmpty(MaQuiCach) && !string.IsNullOrEmpty(Store))
                                {
                                    string SizeID = colName.Split('@')[0];
                                    lstStoreUpdate.Add(new KHDT_StoreEntity
                                    {

                                        StyleID = StyteID,
                                        MaHang = MaHang,
                                        PO = PO,
                                        POID = POID,
                                        Store = Store,
                                        SizeID = SizeID,
                                        Size = dtSize.AsEnumerable().FirstOrDefault(x => x["SizeID"]?.ToString() == SizeID)["Size"]?.ToString(),
                                        ColorID = row["ColorID"].ToString(),
                                        MaMau = row["MaMau"].ToString(),
                                        SLuong = SLuong,
                                        SLThung = Int32.Parse(row["SLThung"].ToString()),
                                        QuiCach = MaQuiCach,
                                        SapXep = Int32.Parse(row["SapXep"]?.ToString())



                                    });
                                }



                            }
                        }


                    }
                }




                string msResult = "";
                if (lstStoreUpdate?.Count == 0)
                {
                    return;
                }

                string url = string.Format("{0}", URL + "KHDT_Store/Post");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstStoreUpdate); }).Result;


                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    NapLai();
                }
                else XtraMessageBox.Show(msResult);

            }
        }

        private void XoaDong()
        {
            try
            {
                int idxFocus = grvKHDTStore.FocusedRowHandle;
                if (dtKHDT_Store?.Rows?.Count == 0 || idxFocus < 0)
                {
                    return;
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    object SapXep = grvKHDTStore.GetRowCellValue(idxFocus, "SapXep");
                    string url = $"{URL}KHDT_DaKieuLap/Delete?Para1={MaDH}&Para2=A&Para3=A";
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        NapLai();

                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai();
        }
        private DataTable ProcessDataExcel(DataTable dt)
        {
            DataColumn newColumn = new DataColumn("SLThung", typeof(int));
            newColumn.DefaultValue = 1;
            dt.Columns.Add(newColumn);
         
            DataTable tblClone = dt.Clone();
            DataRow drOld = null;
            int SttThungTrung = 1;
            int yindex = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if(dr["Carton"].ToString() == "30")
                {
                   
                }    
                if (drOld is null || !CheckRow(drOld, dr))
                {
                    drOld = tblClone.NewRow();
                    if (SttThungTrung > 1)
                    {
                        tblClone.Rows[tblClone.Rows.Count - 1]["SLThung"] = SttThungTrung;
                        SttThungTrung = 1;
                    }
                    KHDongThungLib.CopyDataRow(dr, drOld);
                    tblClone.Rows.Add(drOld);
                    drOld = dr;
                }
                else if (dr == dt.Rows[dt.Rows.Count - 1])
                {
                    
                    
                        tblClone.Rows[tblClone.Rows.Count - 1]["SLThung"] = SttThungTrung + 1;
                    
                }
                else
                {
                    SttThungTrung += 1;
                }
                yindex++;
            }
            return tblClone;
        }
        private bool CheckRow(DataRow dr1, DataRow dr2)
        {
            if (dr1["Carton"].ToString() == dr2["Carton"].ToString() || dr1["PO"].ToString() != dr2["PO"].ToString() || dr1["Inseam"].ToString() != dr2["Inseam"].ToString() || dr1["Mau"].ToString() != dr2["Mau"].ToString())
            {
                return false;
            }
            int index = -1;
            foreach (DataColumn dc in dr1.Table.Columns)
            {
                index++;
                if (index < 6 || index == dr1.Table.Columns.Count - 1)
                {
                    continue;
                }
                if (dr1[dc.ColumnName].ToString() != dr2[dc.ColumnName].ToString())
                {
                    return false;
                }
            }
            return true;
        }
        private void SaveDataExcel(DataTable dt)
        {
            DataTable tblSave = CreateTblSave();
            int STT = 0;
            DataRow drOld = null;
            foreach (DataRow dr in dt.Rows)
            {
                int index = -1;               
                foreach (DataColumn dc in dt.Columns)
                {
                    index++;
                    if (index < 6 || index == dt.Columns.Count - 1)
                    {
                        continue;
                    }

                    string colName = dc.ColumnName.Trim();
                    if (dr[dc].ToString() == "") continue;

                    if (drOld is null || drOld["Carton"].ToString() != dr["Carton"].ToString())
                    {
                        STT++;
                        drOld = dr;
                    }
                    DataRow drNew = tblSave.NewRow();
                    drNew["MaDH"] = dr["MaDH"].ToString().Trim();
                    drNew["StyleID"] = ReplaceSpecialCharacters(dr["MaHang"].ToString().Trim());
                    drNew["MaHang"] = dr["MaHang"].ToString().Trim();
                    drNew["POID"] = ReplaceSpecialCharacters(dr["PO"].ToString().Trim());
                    drNew["PO"] = dr["PO"].ToString().Trim();
                    drNew["STT"] = STT;
                    drNew["DauSizeID"] = ReplaceSpecialCharacters(dr["InSeam"].ToString().Trim()); ;
                    drNew["DauSize"] = dr["InSeam"].ToString().Trim();
                    drNew["ColorID"] = ReplaceSpecialCharacters(dr["Mau"].ToString().Trim()); ;
                    drNew["MaMau"] = "MAU_" + dr["Mau"].ToString().Trim();
                    drNew["SizeID"] = "SIZE_" + ReplaceSpecialCharacters(colName); ;
                    drNew["Size"] = colName;
                    drNew["SLuong"] = dr[dc];
                    drNew["SLThung"] = dr["SLThung"];
                    tblSave.Rows.Add(drNew);                  
                }
            }
            string url = string.Format("{0}", URL + "KHDT_DaKieuLap/Post");
            //return;
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadMaHang();
            }
        }
        //private string ReplaceSpecialCharacters(string input)
        //{
        //    string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
        //    string replacement = "_";
        //    Regex regex = new Regex(pattern);
        //    return regex.Replace(input.Trim(), replacement);
        //}
        private DataTable CreateTblSave()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("PO", typeof(string));
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("SLuong", typeof(int));
            dt.Columns.Add("SLThung", typeof(int));
            dt.Columns.Add("QuiCach", typeof(string));
            dt.Columns.Add("SapXep", typeof(int));
            return dt;
        }

        #region styte Grv

        private void gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }

        }


        private void grv_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {

            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }



        private void grvMaHang_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && e.RowHandle == grvMaHang.FocusedRowHandle)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("#E6FBFF");
                e.HighPriority = true;
            }
        }
        private void grvKHDTStore_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && e.RowHandle == grvKHDTStore.FocusedRowHandle)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("#EFDECD");
                e.HighPriority = true;
            }
        }

        #endregion

    }
}