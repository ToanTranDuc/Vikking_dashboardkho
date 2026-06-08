using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmERP_PhieuBaoGia : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable _tblPhieuBaoGia = new DataTable();
        List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColEdit = new List<BandedGridColumn>();
        public frmERP_PhieuBaoGia()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            layoutItemInputSoPhieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            CreateSearchLookup();
            lstColEdit.AddRange(new[]
            {
             colPTVanChuyen,colTGGiao, colSoLuong,colThue,colChiPhiVanChuyen,colChiPhiKhac,
              colPTThanhToan,colDonGia,colDonViTienTe,colGhiChu
            });

        }

        private DataTable CreateDataTable_PhieuBaoGia_ChiTiet()
        {
            DataTable dt = new DataTable("PhieuBaoGia_ChiTiet");          
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("SoDot", typeof(int));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));      
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("MaNhomVT", typeof(string));
            dt.Columns.Add("KhoSizeID", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("DonGia", typeof(double));
            dt.Columns.Add("DonViTienTe", typeof(string));
            dt.Columns.Add("ThanhTien", typeof(double));
            dt.Columns.Add("PTThanhToan", typeof(string));
            dt.Columns.Add("Thue", typeof(string));
            dt.Columns.Add("ChiPhiKhac", typeof(double));
            dt.Columns.Add("ChiPhiVanChuyen", typeof(double));
            dt.Columns.Add("PTVanChuyen", typeof(string));
            dt.Columns.Add("TGGiao", typeof(string));         
            dt.Columns.Add("TenPhieu", typeof(string));
            dt.Columns.Add("NhomCLCC", typeof(string));
            dt.Columns.Add("MaNCC", typeof(string));
            dt.Columns.Add("ChungLoaiCC", typeof(string));
            dt.Columns.Add("SLKH", typeof(int));
            dt.Columns.Add("IsDuyet", typeof(bool));
            dt.Columns.Add("GhiChu", typeof(string));    
            dt.Columns.Add("MoTa", typeof(string));            
            dt.Columns.Add("TenDVVT", typeof(string));          
            dt.Columns.Add("MauVT", typeof(string));             
            dt.Columns.Add("MaMauVT", typeof(string));            
            dt.Columns.Add("KhoVai", typeof(string));            
            dt.Columns.Add("Sort", typeof(int));                  
           dt.Columns.Add("TenCL", typeof(string));        
           
            dt.Columns.Add("LoaiNPL", typeof(string));            
            dt.Columns.Add("IsNPL", typeof(bool));

       
            dt.Columns.Add("NgayDuyet", typeof(DateTime));
            dt.Columns.Add("NguoiDuyet", typeof(string));
            dt.Columns.Add("NgayHieuLuc", typeof(DateTime));

        
            

            return dt;
        }
        private DataTable CreateTableSavePhieuBG()
        {
            DataTable dt = new DataTable("PhieuBaoGia");

            dt.Columns.Add("ID", typeof(int));     
            dt.Columns.Add("MaPhieuBG", typeof(string));    
            dt.Columns.Add("TenPhieu", typeof(string));  
            dt.Columns.Add("NhomCLCC", typeof(string));   
            dt.Columns.Add("MaNCC", typeof(string));   
            dt.Columns.Add("ChungLoaiCC", typeof(string));  
            dt.Columns.Add("SLKH", typeof(int));      
            dt.Columns.Add("IsDuyet", typeof(bool));      
            dt.Columns.Add("NgayDuyet", typeof(DateTime)); 
            dt.Columns.Add("NguoiDuyet", typeof(string));   
            dt.Columns.Add("NgayTao", typeof(string));   
            dt.Columns.Add("NguoiTao", typeof(string));    
            dt.Columns.Add("NgayHieuLuc", typeof(DateTime));  
            dt.Columns.Add("GhiChu", typeof(string));   

            

            return dt;
        }
        private DataTable CreateTableSaveBaoGiaDotGH()
        {
            DataTable dt = new DataTable("BaoGia_DotGH");

            dt.Columns.Add("ID", typeof(int));    
            dt.Columns.Add("MaPhieuBG", typeof(string));  
            dt.Columns.Add("NhomCLCC", typeof(string));   
            dt.Columns.Add("MaNCC", typeof(string));    
            dt.Columns.Add("ChungLoaiCC", typeof(string));    
            dt.Columns.Add("SoDot", typeof(int));     
            dt.Columns.Add("Dot", typeof(string)); 
            dt.Columns.Add("ItemCode", typeof(string));  
            dt.Columns.Add("MaVTID", typeof(string));  
            dt.Columns.Add("MauVTID", typeof(string));    
            dt.Columns.Add("MaNhomVT", typeof(string));   
            dt.Columns.Add("KhoSizeID", typeof(string));    
            dt.Columns.Add("MaDVVT", typeof(string));    
            dt.Columns.Add("SoLuong", typeof(int));      
            dt.Columns.Add("DonGia", typeof(double));   
            dt.Columns.Add("DonViTienTe", typeof(string));  
            dt.Columns.Add("ThanhTien", typeof(double));    
            dt.Columns.Add("PTThanhToan", typeof(string));   
            dt.Columns.Add("Thue", typeof(string));   
            dt.Columns.Add("ChiPhiKhac", typeof(double));    
            dt.Columns.Add("ChiPhiVanChuyen", typeof(double));    
            dt.Columns.Add("PTVanChuyen", typeof(string));    
            dt.Columns.Add("TGGiao", typeof(string));   
            dt.Columns.Add("DDGiaoHang", typeof(string));  
            dt.Columns.Add("DDNhanHang", typeof(string)); 
            dt.Columns.Add("GhiChu", typeof(string));   
            return dt;
        }

        private void CreateSearchLookup()
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=LoaiCC";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblLoaiCC = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEditLoaiCC.Properties.DataSource = tblLoaiCC;
                searchLookUpEditLoaiCC.Properties.ValueMember = "MaLoaiNCC";
                searchLookUpEditLoaiCC.Properties.DisplayMember = "TenNhom";

            }
            catch (Exception ex)
            {

            }
        }

        private void SetUpColTienTe()
        {
            List<TienTeEntity> lstTienTe = new List<TienTeEntity>();
            string url = $"{URL}PhieuBaoGia/GET?action=GetTienTe";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstTienTe = JsonConvert.DeserializeObject<List<TienTeEntity>>(json);
            }
            RepositoryItemSearchLookUpEdit rDonViTTEdit = new RepositoryItemSearchLookUpEdit();
            rDonViTTEdit.DataSource = lstTienTe;
            rDonViTTEdit.DisplayMember = "TenTienTe";
            rDonViTTEdit.ValueMember = "TienTeID";
            rDonViTTEdit.ShowClearButton = false;
            rDonViTTEdit.NullText = "[Chọn Tiền Tệ]";
            rDonViTTEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rDonViTTEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rDonViTTEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TienTeID", Caption = "TienTeID", Name = "rColTienTeID", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "MaTienTe", Caption = "Mã Tiền Tệ", Name = "rMaTienTe", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenTienTe", Caption = "Tiền Tệ", Name = "rColTenTienTe", Visible = true });
            }
                
            colDonViTienTe.ColumnEdit = rDonViTTEdit;

        }
       
        private RepositoryItemSpinEdit CreateRepositoryThuePercent()
        {
            var repo = new RepositoryItemSpinEdit
            {
                Name = "repoThuePercent",
                IsFloatValue = true,
                MinValue = 0,
                MaxValue = 100,                  
                Increment = 1,
                AllowNullInput = DevExpress.Utils.DefaultBoolean.True,
                NullText = "0"
            };

     
            repo.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            repo.DisplayFormat.FormatString = "n2";  // 10.00%

            repo.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            repo.EditFormat.FormatString = "n2";           // Khi edit thì không hiện %, dễ nhập

           

            return repo;
        }
        private void SetUpColVatTuBGEdit()
        {
            var rItemSpinEdit = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit
            {
                IsFloatValue = false,
                MinValue = 0,
                MaxValue = int.MaxValue,
                Increment = 1,
                AllowNullInput = DevExpress.Utils.DefaultBoolean.True,
                DisplayFormat = { FormatType = DevExpress.Utils.FormatType.Numeric, FormatString = "n0" },
                EditFormat = { FormatType = DevExpress.Utils.FormatType.Numeric, FormatString = "n0" }
            };

            var rItemMoneySpinEdit = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit
            {
             
                IsFloatValue = true,

                MinValue = 0,
                MaxValue = decimal.MaxValue,   

                AllowNullInput = DevExpress.Utils.DefaultBoolean.True,
                Mask = { MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric },
            };

         
            
            rItemMoneySpinEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            rItemMoneySpinEdit.EditFormat.FormatString = "#,##0.####";
         
            colChiPhiVanChuyen.ColumnEdit = rItemMoneySpinEdit;
            colChiPhiKhac.ColumnEdit = rItemMoneySpinEdit;
            colThue.ColumnEdit = CreateRepositoryThuePercent();
            colThanhTien.ColumnEdit = rItemMoneySpinEdit;
            colSoLuong.ColumnEdit = rItemSpinEdit;



        }

    
        private void searchLookUpEditLoaiCC_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchLookUpEditLoaiCC.EditValue?.ToString()))
                {
                    layoutItemInputSoPhieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutItemSelecteSoPhieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    string url = $"{URL}PhieuBaoGia/GET?action=GetNCC&para1={searchLookUpEditLoaiCC.EditValue?.ToString()}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tblNCC = JsonConvert.DeserializeObject<DataTable>(json);
                    searchLookUpEditNCC.Properties.DataSource = tblNCC;
                    searchLookUpEditNCC.Properties.ValueMember = "MaNhaCC";
                    searchLookUpEditNCC.Properties.DisplayMember = "TenNCC";
                    //LoadTenPhieu();
                    grcVatTuPhieuBG_NPL.DataSource = null;
                    _tblPhieuBaoGia = new DataTable();
                    searchLookUpEditPhieuBG.EditValue = null;
                }
              
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadPhieuBG(string SoPhieuBG)
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=Get&para1={SoPhieuBG}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblPhieuBaoGia = CreateDataTable_PhieuBaoGia_ChiTiet();
                if (json != "[]")
                {
                    _tblPhieuBaoGia = JsonConvert.DeserializeObject<DataTable>(json);
                }
                SetUpColTienTe();
                SetUpColVatTuBGEdit();
                grcVatTuPhieuBG_NPL.DataSource = _tblPhieuBaoGia;
                if (_tblPhieuBaoGia?.Rows?.Count > 0)
                {
                    bool.TryParse(_tblPhieuBaoGia?.Rows[0]["IsDuyet"]?.ToString(), out bool IsDuyet);
                    toggleDuyet.EditValue = IsDuyet;
                }
                bgvVatTuPhieuBG_NPL.ExpandAllGroups();
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadBaoGia()
        {
            try
            {
                layoutItemInputSoPhieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutItemSelecteSoPhieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                string maLoaiCC = searchLookUpEditLoaiCC.EditValue?.ToString();
                string maNCC = searchLookUpEditNCC.EditValue?.ToString();
                if (string.IsNullOrEmpty(maLoaiCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string url = $"{URL}PhieuBaoGia/GET?action=GetPhieuBG&para1={maLoaiCC}&para2={maNCC}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblPhieuBG = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEditPhieuBG.Properties.DataSource = tblPhieuBG;
                searchLookUpEditPhieuBG.Properties.ValueMember = "MaPhieuBG";
                searchLookUpEditPhieuBG.Properties.DisplayMember = "TenPhieu";
                LoadTenPhieu();
                grcVatTuPhieuBG_NPL.DataSource = null;
                _tblPhieuBaoGia = new DataTable();
                searchLookUpEditPhieuBG.EditValue = null;
            }
            catch (Exception ex)
            {

            }
        }
        private void searchLookUpEditNCC_EditValueChanged(object sender, EventArgs e)
        {

            LoadBaoGia();


        }
        private void searchLookUpEditPhieuBG_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchLookUpEditPhieuBG.EditValue?.ToString()))
            {
                LoadPhieuBG(searchLookUpEditPhieuBG.EditValue?.ToString());
            }
        }

        private void bgvVatTuPhieuBG_NPL_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;

            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);
            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {
                ControlPaint.DrawBorder3D(e.Graphics, rect);
                rect.Inflate(-1, -1);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color backColor = ColorTranslator.FromHtml("#FFD480"); // default
                    


                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }

                }

                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                    e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.None,
                            FormatFlags = StringFormatFlags.LineLimit
                        };


                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }

                }


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

        private void bgvVatTuPhieuBG_NPL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;

                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;

                int groupLevel = view.GetRowLevel(e.RowHandle);

                GridColumn groupColumn = info.Column;


                info.GroupText = string.Format("{0}", info.GroupValueText);

                if (view.IsGroupRow(e.RowHandle))

                {

                    Color textColor = Color.Black;

                    switch (groupLevel)

                    {

                        case 0: textColor = Color.MediumBlue; break;

                        case 1: textColor = Color.Maroon; break;

                    }

                    e.Appearance.ForeColor = textColor;

                    e.DefaultDraw();

                    e.Handled = true;

                }

            }
            catch (Exception ex)
            {

            }
        }
      
        private void btnChonVatTu_Click(object sender, EventArgs e)
        {
            string maPhieuBG = searchLookUpEditPhieuBG.EditValue?.ToString();
            string maLoaiCC = searchLookUpEditLoaiCC.EditValue?.ToString();
            string maNCC = searchLookUpEditNCC.EditValue?.ToString();
            if (string.IsNullOrEmpty(maLoaiCC))
            {
                XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(maNCC))
            {
                XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (layoutItemInputSoPhieu.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never && string.IsNullOrEmpty(maPhieuBG))
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để cập nhật", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable tbl = grcVatTuPhieuBG_NPL.DataSource as DataTable;
            frmSelectVTBaoGia frm = new frmSelectVTBaoGia(maLoaiCC, maNCC, searchLookUpEditNCC?.Text, !string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null, tbl);
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                if (_tblPhieuBaoGia == null || _tblPhieuBaoGia?.Rows?.Count == 0)
                {
                    _tblPhieuBaoGia = CreateDataTable_PhieuBaoGia_ChiTiet();
                    SetUpColVatTuBGEdit();
                    SetUpColTienTe();
                }

                if (frm.tblVatTuSelected != null && frm.tblVatTuSelected.Rows.Count > 0)
                {
                   
                    int stt = _tblPhieuBaoGia.Rows.Count + 1;

                  
            
                    int maxSoDot = 0;

                    if (_tblPhieuBaoGia?.Rows.Count > 0)
                    {
                        maxSoDot = _tblPhieuBaoGia.AsEnumerable()
                            .Select(r => r["SoDot"])
                            .Where(val => val != null && val != DBNull.Value)
                            .Select(val =>
                            {
                                int.TryParse(val.ToString(), out int result);
                                return (int?)result;
                            })
                            .Where(x => x.HasValue)
                            .Max() ?? 0;
                    }

                    int currentDot = maxSoDot +1;
                  
                    for (int dotIndex = 1; dotIndex <= frm.SoDotGH; dotIndex++)
                    {
                        int soDotHienTai = currentDot++;

                        foreach (DataRow vt in frm.tblVatTuSelected.Rows)
                        {
                            string maVTID = vt["MaVTID"]?.ToString() ?? "";
                            string mauVTID = vt["MauVTID"]?.ToString() ?? "";
                            string chungLoaiCC = vt["MaCLVT"]?.ToString() ?? "";
                            string khoSizeID = vt["KhoVaiID"]?.ToString() ?? "";
                            string maDVVT = vt["MaDVVT"]?.ToString() ?? "";


                         var Query = _tblPhieuBaoGia.AsEnumerable().Where(x =>                    
                        (x["MaVTID"]?.ToString() ?? "") == maVTID &&
                        (x["MauVTID"]?.ToString() ?? "") == mauVTID &&
                        (x["ChungLoaiCC"]?.ToString() ?? "") == chungLoaiCC &&
                        (x["KhoSizeID"]?.ToString() ?? "") == khoSizeID &&
                        (x["MaDVVT"]?.ToString() ?? "") == maDVVT);
                            if(Query!= null)
                            {
                                foreach(var item in Query)
                                {
                                    item["SoDot"] = maxSoDot + frm.SoDotGH;
                                }
                            }
                     
                            DataRow newRow = _tblPhieuBaoGia.NewRow();

                            newRow["STT"] = 0;
                            newRow["ID"] = 0;
                            newRow["MaPhieuBG"] =null;
                            newRow["SoDot"] = maxSoDot +  frm.SoDotGH;
                            newRow["Dot"] = soDotHienTai;                         
                            newRow["ItemCode"] = vt["ItemCode"]?.ToString() ?? "";
                            newRow["MaVTID"] = maVTID;
                            newRow["MaNhomVT"] =  "";
                            newRow["MauVTID"] = mauVTID;
                            newRow["MaMauVT"] = vt["MaMauVT"]?.ToString() ?? "";
                            newRow["KhoSizeID"] = khoSizeID;
                            newRow["MaDVVT"] = maDVVT;
                            newRow["TenCL"] = vt["TenCL"]?.ToString() ?? "";
                            newRow["MoTa"] = vt["MoTa"]?.ToString() ?? "";
                            newRow["TenDVVT"] = vt["TenDVVT"]?.ToString() ?? "";
                            newRow["MauVT"] = vt["MauVT"]?.ToString() ?? "";
                            newRow["KhoVai"] = vt["KhoVai"]?.ToString() ?? "";
                            newRow["Sort"] = vt["Sort"];
                            bool isNPL = vt["IsNPL"] != DBNull.Value && Convert.ToBoolean(vt["IsNPL"]);
                            newRow["IsNPL"] = isNPL;
                            newRow["LoaiNPL"] = isNPL ? "Nguyên Liệu" : "Phụ Liệu";
                         
                            newRow["NhomCLCC"] = maLoaiCC;
                            newRow["MaNCC"] = maNCC;
                            newRow["ChungLoaiCC"] = chungLoaiCC;
                            newRow["TenPhieu"] = txtSoPhieu?.Text?.ToString();
                            newRow["SLKH"] = 0;

                         
                            newRow["IsDuyet"] = false;
                            newRow["NgayDuyet"] = DBNull.Value;
                            newRow["NguoiDuyet"] = "";
                            newRow["NgayHieuLuc"] = DBNull.Value;

                           
                            newRow["SoLuong"] = 0;
                            newRow["DonGia"] = 0.0;
                            newRow["ThanhTien"] = 0.0;
                            newRow["DonViTienTe"] = null;
                            newRow["PTThanhToan"] = null;
                            newRow["Thue"] = null;
                            newRow["ChiPhiKhac"] = 0.0;
                            newRow["ChiPhiVanChuyen"] = 0.0;
                            newRow["PTVanChuyen"] = "";
                            newRow["TGGiao"] = "";
                            newRow["GhiChu"] = "";
                        
                            _tblPhieuBaoGia.Rows.InsertAt(newRow, 0);
                        }
                    }

                  
                    grcVatTuPhieuBG_NPL.DataSource = null;
                    grcVatTuPhieuBG_NPL.DataSource = _tblPhieuBaoGia;                 
                    bgvVatTuPhieuBG_NPL.ExpandAllGroups();
                   
                }

            }
        }

        private void btnThemPhieu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                layoutItemInputSoPhieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutItemSelecteSoPhieu.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;             
                LoadTenPhieu();
                grcVatTuPhieuBG_NPL.DataSource = null;
                _tblPhieuBaoGia = new DataTable();
                searchLookUpEditPhieuBG.EditValue = null;
            }
            catch(Exception ex)
            {

            }
            

        }
        private void LoadTenPhieu()
        {
            try
            {
                string maLoaiCC = searchLookUpEditLoaiCC.EditValue?.ToString();
                string maNCC = searchLookUpEditNCC.EditValue?.ToString();
                if (string.IsNullOrEmpty(maLoaiCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string url = $"{URL}PhieuBaoGia/GET?action=GetCountPhieu&para1={maLoaiCC}&para2={maNCC}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                int CountSoPhieu = 0;
                if (json != "[]")
                {
                    DataTable tblPhieu = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tblPhieu != null && tblPhieu?.Rows?.Count > 0)
                    {
                        var validCounts = tblPhieu.AsEnumerable()
                        .Select(row => row["CountPhieu"]?.ToString().Trim())
                        .Where(s => !string.IsNullOrEmpty(s) && int.TryParse(s, out _))
                        .Select(s => int.Parse(s));

                        if (validCounts.Any())
                        {
                            CountSoPhieu = validCounts.Max();
                        }
                    }
                }

                txtSoPhieu.Text = $"{GlobleData.UserName}|{searchLookUpEditLoaiCC.Text}|{searchLookUpEditNCC.Text}|{CountSoPhieu + 1}";
            }
            catch(Exception ex)
            {

            }
        }
     
        private async void btnLuuPhieuBG_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                string url = $"{URL}PhieuBaoGia/POST?action=POST";
                DataTable tblHienThi = grcVatTuPhieuBG_NPL.DataSource as DataTable;
                if (tblHienThi == null || tblHienThi.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Chưa có dữ liệu chi tiết vật tư để lưu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maLoaiCC = searchLookUpEditLoaiCC.EditValue?.ToString();
                string maNCC = searchLookUpEditNCC.EditValue?.ToString();
                if (string.IsNullOrEmpty(maLoaiCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool coDuLieu = tblHienThi.AsEnumerable()
                    .Any(r => (r["SoLuong"] == DBNull.Value ? 0 : Convert.ToInt32(r["SoLuong"])) > 0);

                if (!coDuLieu)
                    
                {
                    XtraMessageBox.Show("Chưa nhập số lượng cho bất kỳ vật tư nào", "Xác nhận", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                bool coDuLieuDG = tblHienThi.AsEnumerable()
                  .Any(r => (r["DonGia"] == DBNull.Value ? 0 : Convert.ToInt32(r["DonGia"])) > 0);
                if (!coDuLieuDG)

                {
                    XtraMessageBox.Show("Chưa nhập đơn cho bất kỳ vật tư nào", "Xác nhận", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataSet dsSave = new DataSet();

                string maPhieuBG = searchLookUpEditPhieuBG.EditValue?.ToString();
                DataTable tblPhieu = CreateTableSavePhieuBG();
                DataRow rowPhieu = tblPhieu.NewRow();
                rowPhieu["ID"] = 0; 
                rowPhieu["MaPhieuBG"] = !string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null;
                rowPhieu["TenPhieu"] = txtSoPhieu.Text.Trim();
                rowPhieu["NhomCLCC"] = maLoaiCC ?? "";
                rowPhieu["MaNCC"] = maNCC ?? "";
                rowPhieu["ChungLoaiCC"] = null;
                rowPhieu["SLKH"] = 0;
                rowPhieu["IsDuyet"] = false;
                rowPhieu["NgayDuyet"] = DateTime.Now;
                rowPhieu["NguoiDuyet"] = GlobleData.UserName;
                rowPhieu["NgayTao"] = DateTime.Now;
                rowPhieu["NguoiTao"] = GlobleData.UserName;
                rowPhieu["NgayHieuLuc"] = DateTime.Now;
                rowPhieu["GhiChu"] = null;
                tblPhieu.Rows.Add(rowPhieu);
                dsSave.Tables.Add(tblPhieu);

              
                DataTable tblChiTiet = CreateTableSaveBaoGiaDotGH();

                foreach (DataRow row in tblHienThi.Rows)
                {
                  
                  

                    DataRow newRow = tblChiTiet.NewRow();

                    newRow["ID"] = row["ID"] == DBNull.Value || Convert.ToInt64(row["ID"]) <= 0 ? 0 : row["ID"];
                    newRow["MaPhieuBG"] = !string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null;
                    newRow["NhomCLCC"] = maLoaiCC;
                    newRow["MaNCC"] = maNCC;
                    newRow["ChungLoaiCC"] = row["ChungLoaiCC"]?.ToString() ?? "";
                    newRow["SoDot"] = row["SoDot"] == DBNull.Value ? 1 : Convert.ToInt32(row["SoDot"]);
                    newRow["Dot"] = row["Dot"]?.ToString() ?? "1";
                    newRow["ItemCode"] = row["ItemCode"]?.ToString();
                    newRow["MaVTID"] = row["MaVTID"]?.ToString() ;
                    newRow["MauVTID"] = row["MauVTID"]?.ToString();
                    newRow["MaNhomVT"] = row["MaNhomVT"]?.ToString();
                    newRow["KhoSizeID"] = row["KhoSizeID"]?.ToString();
                    newRow["MaDVVT"] = row["MaDVVT"]?.ToString();
                    newRow["SoLuong"] = row["SoLuong"] == DBNull.Value ? 0 : Convert.ToInt32(row["SoLuong"]);
                    newRow["DonGia"] = row["DonGia"] == DBNull.Value ? 0.0 : Convert.ToDouble(row["DonGia"]);
                    newRow["DonViTienTe"] = row["DonViTienTe"]?.ToString();
                    newRow["ThanhTien"] = row["ThanhTien"] == DBNull.Value ? 0.0 : Convert.ToDouble(row["ThanhTien"]);
                    newRow["PTThanhToan"] = row["PTThanhToan"]?.ToString();
                    newRow["Thue"] = row["Thue"]?.ToString();
                    newRow["ChiPhiKhac"] = row["ChiPhiKhac"] == DBNull.Value ? 0.0 : Convert.ToDouble(row["ChiPhiKhac"]);
                    newRow["ChiPhiVanChuyen"] = row["ChiPhiVanChuyen"] == DBNull.Value ? 0.0 : Convert.ToDouble(row["ChiPhiVanChuyen"]);
                    newRow["PTVanChuyen"] = row["PTVanChuyen"]?.ToString() ?? "";
                    newRow["TGGiao"] = row["TGGiao"]?.ToString();
                    newRow["DDGiaoHang"] = "";
                    newRow["DDNhanHang"] = "";
                    newRow["GhiChu"] = row["GhiChu"]?.ToString();

                    tblChiTiet.Rows.Add(newRow);
                }

                dsSave.Tables.Add(tblChiTiet);

              
                dsSave.Tables[0].TableName = "PhieuBaoGia";
                dsSave.Tables[1].TableName = "BaoGia_DotGH";
                  
                string jsonData = JsonConvert.SerializeObject(dsSave);
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (result.Trim().ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadBaoGia();


                }
                else
                {
                    XtraMessageBox.Show("Lỗi khi lưu:\n" + result, "Lỗi lưu dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
               
                XtraMessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDuyet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/PostXetDuyet";
                string maPhieuBG = searchLookUpEditPhieuBG.EditValue?.ToString();
                string maLoaiCC = searchLookUpEditLoaiCC.EditValue?.ToString();
                string maNCC = searchLookUpEditNCC.EditValue?.ToString();
                if (string.IsNullOrEmpty(maLoaiCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maPhieuBG))
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataTable tblPhieu = CreateTableSavePhieuBG();
                DataRow rowPhieu = tblPhieu.NewRow();
                rowPhieu["ID"] = 0;
                rowPhieu["MaPhieuBG"] = maPhieuBG;
                rowPhieu["TenPhieu"] = txtSoPhieu.Text.Trim();
                rowPhieu["NhomCLCC"] = maLoaiCC ?? "";
                rowPhieu["MaNCC"] = maNCC ?? "";
                rowPhieu["ChungLoaiCC"] = null;
                rowPhieu["SLKH"] = 0;
                rowPhieu["IsDuyet"] = true;
                rowPhieu["NgayDuyet"] = DateTime.Now;
                rowPhieu["NguoiDuyet"] = GlobleData.UserName;
                rowPhieu["NgayTao"] = DateTime.Now;
                rowPhieu["NguoiTao"] = GlobleData.UserName;
                rowPhieu["NgayHieuLuc"] = DateTime.Now;
                rowPhieu["GhiChu"] = null;
                tblPhieu.Rows.Add(rowPhieu);

                string jsonData = JsonConvert.SerializeObject(tblPhieu);
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (result.Trim().ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadBaoGia();

                }
                else
                {
                    XtraMessageBox.Show("Lỗi khi lưu:\n" + result, "Lỗi lưu dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch(Exception ex)
            {

            }
        }

        private void btnHuyDuyet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/PostXetDuyet";
                string maPhieuBG = searchLookUpEditPhieuBG.EditValue?.ToString();
                string maLoaiCC = searchLookUpEditLoaiCC.EditValue?.ToString();
                string maNCC = searchLookUpEditNCC.EditValue?.ToString();

                if (string.IsNullOrEmpty(maLoaiCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maPhieuBG))
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu báo giá để hủy duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataTable tblPhieu = CreateTableSavePhieuBG();
                DataRow rowPhieu = tblPhieu.NewRow();
                rowPhieu["ID"] = 0;
                rowPhieu["MaPhieuBG"] = maPhieuBG;
                rowPhieu["TenPhieu"] = txtSoPhieu.Text.Trim();
                rowPhieu["NhomCLCC"] = maLoaiCC ?? "";
                rowPhieu["MaNCC"] = maNCC ?? "";
                rowPhieu["ChungLoaiCC"] = null;
                rowPhieu["SLKH"] = 0;
                rowPhieu["IsDuyet"] = false;
                rowPhieu["NgayDuyet"] = DateTime.Now;
                rowPhieu["NguoiDuyet"] = GlobleData.UserName;
                rowPhieu["NgayTao"] = DateTime.Now;
                rowPhieu["NguoiTao"] = GlobleData.UserName;
                rowPhieu["NgayHieuLuc"] = DateTime.Now;
                rowPhieu["GhiChu"] = null;
                tblPhieu.Rows.Add(rowPhieu);

                string jsonData = JsonConvert.SerializeObject(tblPhieu);
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (result.Trim().ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadBaoGia();

                }
                else
                {
                    XtraMessageBox.Show("Lỗi khi lưu:\n" + result, "Lỗi lưu dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void bttRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadBaoGia();
        }
        private void btnXoaPhieuBG_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string MaPhieu = searchLookUpEditPhieuBG.EditValue?.ToString();
                if (string.IsNullOrEmpty(MaPhieu))
                {
                    XtraMessageBox.Show("Vui lòng chọn mã phiếu để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string TenPhieu = searchLookUpEditPhieuBG.Text?.ToString();
                DialogResult messResult = MessageBox.Show(
                     $"Bạn có muốn xóa {TenPhieu} không?",
                     "Thông báo",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;
                DataTable tbl = new DataTable();
                string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={MaPhieu}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tbl?.Rows?.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                        {
                            XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                }

                string url = $"{URL}PhieuBaoGia/Delete?action=DeletePhieu&Para1={MaPhieu}&Para2=None";
                string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                    LoadBaoGia();

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void btnDeleteVTBaoGia_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string MaPhieu = searchLookUpEditPhieuBG.EditValue?.ToString();
                DataTable tblThongSo = grcVatTuPhieuBG_NPL.DataSource as DataTable;
                if (tblThongSo == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = bgvVatTuPhieuBG_NPL.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = bgvVatTuPhieuBG_NPL.GetDataRow(rowHandle);
                    if (dr == null) continue;

                                 

                    if (dr["ID"].ToString() != "0")
                    {
                        DataTable tbl = new DataTable();
                        string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={MaPhieu}";
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                        if (json != "[]")
                        {
                            tbl = JsonConvert.DeserializeObject<DataTable>(json);
                            if (tbl?.Rows?.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                                {
                                    XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }

                        }

                        string url = $"{URL}PhieuBaoGia/Delete?action=Delete&Para1={dr["ID"]}&Para2=None";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }

                   
                    tblThongSo.Rows.Remove(dr);
                }

                grcVatTuPhieuBG_NPL.DataSource = tblThongSo;
                grcVatTuPhieuBG_NPL.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgvVatTuPhieuBG_NPL_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "SoLuong" || e.Column.FieldName == "DonGia" || e.Column.FieldName == "Thue")
            {
                var view = sender as GridView;
                if (view == null || e.RowHandle < 0) return;

             
                view.CellValueChanging -= bgvVatTuPhieuBG_NPL_CellValueChanging;

                try
                {
                 
                    decimal soLuong = 0, donGia = 0, thuePercent = 0;

                    switch (e.Column.FieldName)
                    {
                        case "SoLuong":
                            decimal.TryParse(e.Value?.ToString(), out soLuong);
                            donGia = GetDecimalValue(view, e.RowHandle, "DonGia");
                            thuePercent = GetThuePercent(view, e.RowHandle, "Thue");
                            break;

                        case "DonGia":
                            soLuong = GetDecimalValue(view, e.RowHandle, "SoLuong");
                            decimal.TryParse(e.Value?.ToString(), out donGia);
                            thuePercent = GetThuePercent(view, e.RowHandle, "Thue");
                            break;

                        case "Thue":
                            soLuong = GetDecimalValue(view, e.RowHandle, "SoLuong");
                            donGia = GetDecimalValue(view, e.RowHandle, "DonGia");
                            thuePercent = GetThuePercentFromValue(e.Value); // xử lý riêng vì là chuỗi
                            break;
                    }

                 
                    decimal thanhTien = soLuong * donGia * (1 + thuePercent);

                 
                    view.SetRowCellValue(e.RowHandle, "ThanhTien", Math.Round(thanhTien, 2));
                }
                finally
                {
                    // Bật lại sự kiện
                    view.CellValueChanging += bgvVatTuPhieuBG_NPL_CellValueChanging;
                }
            }
        }

        // === Hàm hỗ trợ lấy số an toàn ===
        private decimal GetDecimalValue(GridView view, int rowHandle, string fieldName)
        {
            var val = view.GetRowCellValue(rowHandle, fieldName);
            if (val is decimal d) return d;
            if (val is double db) return (decimal)db;
            if (val is int i) return i;
            decimal.TryParse(val?.ToString(), out decimal result);
            return result;
        }
        // Hàm hỗ trợ lấy số an toàn
        private decimal GetDecimal(DataRow row, string fieldName)
        {
            if (row[fieldName] == null || row[fieldName] == DBNull.Value)
                return 0m;

            if (decimal.TryParse(row[fieldName].ToString(), out decimal result))
                return result;

            return 0m;
        }

        // === Hàm xử lý thuế từ nhiều định dạng: "10%", "10", 10, null... ===
        private decimal GetThuePercent(GridView view, int rowHandle, string fieldName)
        {
            var val = view.GetRowCellValue(rowHandle, fieldName);
            return GetThuePercentFromValue(val);
        }

        private decimal GetThuePercentFromValue(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;

            string str = value.ToString().Replace("%", "").Trim();
            if (decimal.TryParse(str, out decimal percent))
                return percent / 100m;

            return 0m;
        }

        private void bgvVatTuPhieuBG_NPL_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "TongTien")
            {
                DataRow row = bgvVatTuPhieuBG_NPL.GetDataRow(e.ListSourceRowIndex);
                if (row == null) return;

                decimal thanhTien = GetDecimal(row, "ThanhTien");
                decimal chiPhiVC = GetDecimal(row, "ChiPhiVanChuyen");
                decimal chiPhiKhac = GetDecimal(row, "ChiPhiKhac");

                e.Value = thanhTien + chiPhiVC + chiPhiKhac;
            }
        }

        private void bgvVatTuPhieuBG_NPL_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void bgvVatTuPhieuBG_NPL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
            try
            {
                bool.TryParse(_tblPhieuBaoGia?.Rows[0]["IsDuyet"]?.ToString(), out bool IsDuyet);
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (IsDuyet)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
                else if (lstColEdit.Contains(view.FocusedColumn))
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void grvBaoGia_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            int rowHandle = view.FocusedRowHandle;
            if (rowHandle < 0) return;
            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;

            if (col_focused != colSoLuong || col_focused != colThanhTien) return; // Chỉ kiểm tra cho cột này

            string newValue = e.Value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(newValue) || string.IsNullOrWhiteSpace(newValue))
            {
                e.Valid = false;
                e.ErrorText = $"{col_focused.Caption} không được bỏ trống!";
                return;
            }

        
            e.Valid = true;
            e.ErrorText = string.Empty;
        }

        private void gridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();


        }
    }
}