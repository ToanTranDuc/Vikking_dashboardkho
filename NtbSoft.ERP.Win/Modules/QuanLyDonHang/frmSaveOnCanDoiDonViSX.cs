using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{


    public partial class frmSaveOnCanDoiDonViSX : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        private string _MaDH = string.Empty;
        private DataTable _tblLenhSX, _tblPOChiTiet;
        private DataTable tblCanDoiLenhSX;
        private DataTable _tblCanDoiLenhSX_BeforeEdit;
        private DataRow rowSelected;

        List<EditCanDoiLenhSanXuatEntity> lstSave = new List<EditCanDoiLenhSanXuatEntity>();
        public DialogResult _result { get; set; }

        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;
        public frmSaveOnCanDoiDonViSX()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        public frmSaveOnCanDoiDonViSX(DataTable tblLenhSX, string MaDH)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _tblLenhSX = tblLenhSX;
            _tblPOChiTiet = new DataTable();
            _MaDH = MaDH;
            
        }
        protected override void OnLoad(EventArgs e)
        {
            setTitleCanDoi();
            LoadCanDoiLenhSX();
            groupLevelColors = new Dictionary<int, Color>
          {
            { -1, ColorTranslator.FromHtml("#2A5D9F") },
            { 0, ColorTranslator.FromHtml("#2A5D9F") },
            { 1, ColorTranslator.FromHtml("#A53E25") },
            { 2, ColorTranslator.FromHtml("#2E7D5B") }
          };
            groupLevelColorBackground = new Dictionary<int, Color>
            {
                { -1, ColorTranslator.FromHtml("#DDEBFB") },
                { 0, ColorTranslator.FromHtml("#DDEBFB") },
                { 1, ColorTranslator.FromHtml("#FFE2D3") },
                { 2, ColorTranslator.FromHtml("#D7F5E8") }
            };
            bandViewCanDoiLSX.ExpandAllGroups();
            //bandViewCanDoiLSX.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.EditForm;
        }

        private void setTitleCanDoi()
        {
            rowSelected = _tblLenhSX.Rows[0];
            txtDonVi.EditValue = rowSelected["TenDVSX"];
            txtMaDH.EditValue = _MaDH;
            //txtPO.EditValue = rowSelected["PO"];
            //txtInSeam.EditValue = rowSelected["DauSize"];
            //txtMau.EditValue = rowSelected["TenMau"];
            //txtCodeMau.EditValue = rowSelected["CodeMau"];
        }
        private void LoadCanDoiLenhSX()
        {
            try
            {
                tblCanDoiLenhSX = new DataTable();
                string url = $"{URL}SaveOnCanDoiDonViSX/Get?Action=GetCanDoi&para={txtMaDH.Text}&para2={rowSelected["POID"]}&para3={rowSelected["DauSizeID"]}&para4={rowSelected["MaMau"]}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblCanDoiLenhSX = JsonConvert.DeserializeObject<DataTable>(json);
                    _tblCanDoiLenhSX_BeforeEdit = JsonConvert.DeserializeObject<DataTable>(json);
                    CreateBandGridSize_Detail(tblCanDoiLenhSX, bandViewCanDoiLSX, gbSize);
                    grcCanDoiLSX.DataSource = tblCanDoiLenhSX;
                    grcCanDoiLSX.RefreshDataSource();
                }
            }
            catch(Exception ex)
            {

            }
            
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }
        private void rItemTextEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
          
            if (!char.IsDigit(e.KeyChar) &&
                e.KeyChar != (char)Keys.Back &&
                e.KeyChar != (char)Keys.Delete &&
                e.KeyChar != (char)Keys.Tab &&
                e.KeyChar != (char)Keys.Enter)
            {
                e.Handled = true; 
                return;
            }

            if (e.KeyChar == '-')
            {
                e.Handled = true; 
                return;
            }

        }
        private void Save()
        {
            try
            {
                this.ActiveControl = this.txtDonVi;

                string macAddress = "";
                string ipAddress = "";

                // Lấy MAC + IP từ card mạng đang hoạt động (không loopback, không ảo)
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                    .OrderByDescending(ni => ni.Speed)
                    .FirstOrDefault();

                if (networkInterface != null)
                {
                  
                    PhysicalAddress pa = networkInterface.GetPhysicalAddress();
                    macAddress = string.Join("-", pa.GetAddressBytes().Select(b => b.ToString("X2")));

                   
                    var ipProps = networkInterface.GetIPProperties();
                    var ipv4 = ipProps.UnicastAddresses
                        .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);

                    if (ipv4 != null)
                    {
                        ipAddress = ipv4.Address.ToString();
                    }
                }
                lstSave = new List<EditCanDoiLenhSanXuatEntity>();
                DataTable tblCanDoi =grcCanDoiLSX.DataSource as DataTable;
                if (tblCanDoi?.Rows?.Count > 0)
                {
                  
                    foreach (DataRow row in tblCanDoi.Rows)
                    {
                        foreach (DataColumn col in tblCanDoi.Columns)
                        {
                            if (col.ColumnName.Contains("@Size@"))
                            {
                                int Old_SL = 0;
                                string[] arraySize = col.ColumnName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                                if (Int32.TryParse(row[col.ColumnName]?.ToString(), out int SoLuong))
                                {
                                    if (SoLuong < 0) continue;
                                    var Query = _tblCanDoiLenhSX_BeforeEdit.AsEnumerable().
                                        FirstOrDefault(x=> x["MaLenhSanXuat"]?.ToString() == row["MaLenhSanXuat"]?.ToString() && x["MaLenh"]?.ToString() == row["MaLenh"]?.ToString() && x["MaDH"]?.ToString() == row["MaDH"]?.ToString()
                                        && x["POID"]?.ToString() == row["POID"]?.ToString() && x["MaMau"]?.ToString() == row["MaMau"]?.ToString() && x["DauSizeID"]?.ToString() == row["DauSizeID"]?.ToString()
                                         && x["MaGop"]?.ToString() == row["MaGop"]?.ToString() && x["Line"]?.ToString() == row["Line"]?.ToString()
                                    );
                                    if (Query != null)
                                    {
                                        Int32.TryParse(Query[col.ColumnName]?.ToString(), out Old_SL);
                                    }
                                    EditCanDoiLenhSanXuatEntity objSave = new EditCanDoiLenhSanXuatEntity();
                                    objSave.MaLenhSanXuat = row["MaLenhSanXuat"]?.ToString();
                                    objSave.MaLenh = row["MaLenh"]?.ToString();
                                    objSave.TenLenh = row["TenLenh"]?.ToString();
                                    objSave.MaDH = row["MaDH"]?.ToString();
                                    objSave.POID = row["POID"]?.ToString();
                                    objSave.PO = row["PO"]?.ToString();
                                    objSave.MaMau = row["MaMau"]?.ToString();
                                    objSave.DauSizeID = row["DauSizeID"]?.ToString();
                                    objSave.DauSize = row["DauSize"]?.ToString();
                                    objSave.Size = arraySize[1];
                                    objSave.SizeID = arraySize[0];
                                    objSave.SoLuong = SoLuong;
                                    objSave.Old_SL = Old_SL;
                                    objSave.MaGop = row["MaGop"]?.ToString();
                                    objSave.Line = row["Line"]?.ToString();
                                    objSave.SawDep = row["SawDep"]?.ToString();
                                    objSave.UserName = GlobleData.UserName;
                                    objSave.CreaDate = DateTime.Now;
                                    objSave.Mac = macAddress;
                                    objSave.IPAdress = ipAddress;
                                    objSave.MachineName = Environment.MachineName;
                                    lstSave.Add(objSave);
                                }

                            }

                        }
                    }
                    
                    string url = $"{URL}SaveOnCanDoiDonViSX/Post?action=Post";
                    string msResult = Task.Run(async () =>
                    {
                        return await _clientExtension.PostAsync(url, lstSave);
                    }).Result;

                    if (msResult.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 3000);

                        List<EditCanDoiLenhSanXuatEntity> lstItemDongBo = lstSave.GroupBy(x => new { x.MaLenhSanXuat, x.MaLenh })
                                                                           .Select(g => new EditCanDoiLenhSanXuatEntity
                                                                           {
                                                                               MaLenhSanXuat = g.Key.MaLenhSanXuat,
                                                                               MaLenh = g.Key.MaLenh
                                                                           })
                                                                           .ToList();


                        string MaLenh = string.Join(";", lstItemDongBo.Select(x => x.MaLenh));

                        XtraMessageBox.Show($"Lệnh {MaLenh} đã thay đổi số lượng cần xác nhận Chia chuyền và duyệt lệnh sản xuất.Sau đó trưởng bộ phận duyệt lại!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        //if (Show("Bạn Có Muốn Đồng Bộ Dữ Liệu Về <b><color=red>QLSX</color></b>") == DialogResult.Yes)
                        //{
                        //    foreach (EditCanDoiLenhSanXuatEntity item in lstItemDongBo)
                        //{

                        //        string urlCheckGiaCong = $"{URL}ERPDonHangTong/Get?action=CheckGiaCong&para={item.MaLenhSanXuat}";
                        //        string jsonCheckGiaCong= Task.Run(async () => await _clientExtension.GetAsnyc(urlCheckGiaCong)).Result;
                        //        if (string.IsNullOrEmpty(jsonCheckGiaCong)) continue;

                        //        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonCheckGiaCong);
                        //        if (tbl == null || tbl.Rows.Count == 0) continue;

                        //        int giaCong = 0;
                        //        int.TryParse(tbl.Rows[0]["GiaCong"]?.ToString(), out giaCong);

                        //        if (giaCong == 1) continue;

                        //        string urlDB = $"{URL}SaveOnCanDoiDonViSX/DongBoQLSX?action=DongBo&para={item.MaLenh}";
                        //        string msResultDB = Task.Run(async () =>
                        //        {
                        //            return await _clientExtension.PostAsync(urlDB, null);
                        //        }).Result;
                        //        if(msResultDB == "2")
                        //        {
                        //            XtraMessageBox.Show($"Đồng Bộ Mã Lệnh {item} Thất Bại . Vui Lòng Thử Lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //            return;
                        //        }

                        //}

                        //}
                        this.DialogResult = DialogResult.OK;
                    }


                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Thực hiện lỗi.Vui lòng thử lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }
          
        private void RemoveColumnSize(BandedGridView BandedGridView_Detail)
        {
            for (int i = 0; i < BandedGridView_Detail.Columns.Count;)
            {
                if (BandedGridView_Detail.Columns[i].FieldName.Contains("@Size@"))
                {
                    BandedGridView_Detail.Columns.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }
        }

        private void ClearBand(GridBand GridBandSize, BandedGridView BandedGridView_Detail)
        {
            GridBandSize.Children.Clear();
            RemoveColumnSize(BandedGridView_Detail);
        }

        private bool CheckExistBand(string size, GridBand GridBandSize)
        {
            GridBand gbCheck = GridBandSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private void CreateBandGridSize_Detail(DataTable dt, BandedGridView BandedGridView_Detail, GridBand GridBandSize)
        {
            ClearBand(GridBandSize, BandedGridView_Detail);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                string[] parts = colName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                var _sizeID = parts[0];
                var _size = parts[1];
                if (!CheckExistBand(_sizeID, GridBandSize)) continue;

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

            


                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = $"{_sizeID}@Size@{_size}";
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.ReadOnly = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                col.ColumnEdit = rItemSpinEdit;

                //rItemSpinEdit.KeyPress += rItemTextEdit_KeyPress;

                GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                itemSize.FieldName = col.FieldName;
                itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                itemSize.DisplayFormat = "{0:n0}";
                itemSize.ShowInGroupColumnFooter = col;
                BandedGridView_Detail.GroupSummary.Add(itemSize);

                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1)
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
                BandedGridView_Detail.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();             
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
                GridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
            
        }
        private void BandedGridView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
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
                    Color backColor = ColorTranslator.FromHtml("#FFD480");

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
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
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
            catch (ArgumentException ex)
            {

            }
        }

        private void GridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (view == null || info == null) return;
            GridColumn groupColumn = info.Column;
            int groupLevel = view.GetRowLevel(e.RowHandle);

            if (groupColumn == colMaLenh)
            {
                info.GroupText = $"Mã Lệnh : {info.GroupValueText}";
            }
         
            if (view.IsGroupRow(e.RowHandle))
            {

                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel + 1];
                }

                e.Appearance.ForeColor = textColor;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
  
        private void BandedView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DauSize")
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "0";
                }

            }
            else if (e.Column.FieldName.Contains("@"))
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "-";
                }
                
            }
        }

        private void BandGridView_Detail_CustomDrawRowFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;


            int groupLevel = view.GetRowLevel(e.RowHandle);

            Color backColor = Color.FromArgb(255, 239, 204);


            if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color levelColor))
            {
                backColor = levelColor;
            }


            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, backColor, backColor, 90))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }


            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void BandGridView_Detail_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);


            if (groupLevelColors != null && groupLevelColors.TryGetValue(groupLevel, out Color groupColor))
            {
                e.Appearance.ForeColor = groupColor;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //e.Handled = true;
            }
            else if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color fallbackColor))
            {
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, fallbackColor, fallbackColor, 90))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }


                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);

                e.Handled = true;
            }


        }

        private void bandViewCanDoiLSX_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData)
                {
                    decimal tong = 0;
                    foreach (BandedGridColumn col in bandViewCanDoiLSX.Columns)
                    {

                        if (col.FieldName != null && col.FieldName.Contains("@Size@"))
                        {
                            object cellValue = bandViewCanDoiLSX.GetRowCellValue(e.ListSourceRowIndex, col);
                            if (cellValue != null && decimal.TryParse(cellValue.ToString(), out decimal val))
                            {
                                tong += val;
                            }
                        }
                    }


                    e.Value = tong;
                }
            }
            catch(Exception ex)
            {

            }
           
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            setTitleCanDoi();
            LoadCanDoiLenhSX();
            bandViewCanDoiLSX.ExpandAllGroups();
        }

        private DialogResult Show(string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs
            {
                Caption = Resources.Warning,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Text = message,
                Buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No },
                Icon = System.Drawing.SystemIcons.Warning,
                MessageBeepSound = MessageBeepSound.Warning,
                DefaultButtonIndex = 1 // Mặc định là "No"
            };

            return XtraMessageBox.Show(args);
        }
    }
}