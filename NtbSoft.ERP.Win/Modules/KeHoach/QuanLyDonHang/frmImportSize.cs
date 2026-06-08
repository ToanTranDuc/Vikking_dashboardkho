using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
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
    public partial class frmImportSize : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private List<string> _lstSize;
        public List<BangSizeEntity> SelectedSizes { get; private set; }
        string _mkh = string.Empty;
        string _mh = string.Empty;
        string _inseam = string.Empty;
        private HttpClientExtension _clientExtension;
        private HashSet<DataRow> selectedRowsSize = new HashSet<DataRow>();
        private List<string> _preselectedSizeIds = new List<string>();
        public frmImportSize(List<string> lstSize, string mkh, string mh, string inseam, List<string> preselectedSizeIds = null)
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _mkh = mkh;
            _mh = mh;
            _inseam = inseam;
            InitializeComponent();
            _lstSize = lstSize;
            _preselectedSizeIds = preselectedSizeIds ?? new List<string>();
            SelectedSizes = new List<BangSizeEntity>();

        }


        protected override void OnLoad(EventArgs e)
        {
            DataTable dttbSize = new DataTable();
            //string urlSize = string.Format("{0}?mahang={1}&&makh={2}&&dausize={3}", URL + "DonHangTong/GetSizeEdit", _mh, _mkh, _inseam);
            string urlSize = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
            dttbSize = JsonConvert.DeserializeObject<DataTable>(json);
            if (_lstSize != null && _lstSize.Count > 0)
            {
                //var rowsToRemove = dttbSize.AsEnumerable()
                //    .Where(r => _lstSize.Contains(r["MaSize"].ToString()))
                //    .ToList();
                //foreach (var r in rowsToRemove)
                //{
                //    dttbSize.Rows.Remove(r);
                //}
                //dttbSize.AcceptChanges();

                var rowsToRemove = dttbSize.AsEnumerable()
                                    .Where(r =>
                                    {
                                        string maSize = r["MaSize"].ToString();
                                        return _lstSize.Contains(maSize) &&
                                               (_preselectedSizeIds == null || !_preselectedSizeIds.Contains(maSize));
                                    })
                                    .ToList();

                foreach (var r in rowsToRemove)
                    dttbSize.Rows.Remove(r);

                dttbSize.AcceptChanges();
            }
            searchLookUpEdit1.Properties.DataSource = dttbSize;
            searchLookUpEdit1.Properties.DisplayMember = "TenSize";
            searchLookUpEdit1.Properties.ValueMember = "MaSize";
            if (searchLookUpEdit1View != null && searchLookUpEdit1View.GridControl != null)
            {
                try
                {
                    searchLookUpEdit1View.GridControl.BeginUpdate();
                    searchLookUpEdit1View.GridControl.DataSource = dttbSize;
                    searchLookUpEdit1View.PopulateColumns();

                    // 🟢 Hiển thị chỉ 2 cột: TenSize và TheSize
                    foreach (DevExpress.XtraGrid.Columns.GridColumn col in searchLookUpEdit1View.Columns)
                    {
                        col.Visible = false;
                    }

                    if (searchLookUpEdit1View.Columns["TheSize"] != null)
                    {
                        searchLookUpEdit1View.Columns["TheSize"].Visible = true;
                        searchLookUpEdit1View.Columns["TheSize"].GroupIndex = 0;
                        searchLookUpEdit1View.Columns["TheSize"].VisibleIndex = 0;
                    }

                    if (searchLookUpEdit1View.Columns["TenSize"] != null)
                    {
                        searchLookUpEdit1View.Columns["TenSize"].Visible = true;
                        searchLookUpEdit1View.Columns["TenSize"].VisibleIndex = 1;
                        searchLookUpEdit1View.Columns["TenSize"].Caption = "Size";

                        // Nếu có cột Sort thì sắp theo Sort
                        if (dttbSize.Columns.Contains("Sort"))
                        {
                            searchLookUpEdit1View.Columns["TenSize"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
                            searchLookUpEdit1View.SortInfo.Clear();
                            searchLookUpEdit1View.SortInfo.Add(
                                new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                                    dttbSize.Columns.Contains("Sort") ? searchLookUpEdit1View.Columns["Sort"] : searchLookUpEdit1View.Columns["TenSize"],
                                    DevExpress.Data.ColumnSortOrder.Ascending
                                )
                            );
                        }
                    }

                    // ⚙️ Cấu hình group hiển thị
                    searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
                    searchLookUpEdit1View.OptionsBehavior.AutoExpandAllGroups = true;
                    searchLookUpEdit1View.Appearance.GroupRow.Font = new Font(searchLookUpEdit1View.Appearance.GroupRow.Font, FontStyle.Bold);
                    searchLookUpEdit1View.Appearance.GroupRow.ForeColor = Color.DarkSlateGray;
                    searchLookUpEdit1View.Appearance.GroupRow.Options.UseFont = true;
                    searchLookUpEdit1View.Appearance.GroupRow.Options.UseForeColor = true;

                    searchLookUpEdit1View.GridControl.EndUpdate();
                    searchLookUpEdit1View.RefreshData();
                }
                catch
                {
                    // ignore, sẽ thử fallback phía dưới
                }
            }

            // nếu vẫn chưa có dòng nào, ép SearchLookUpEdit khởi tạo grid nội bộ (fallback)
            if (searchLookUpEdit1View == null || searchLookUpEdit1View.DataRowCount == 0)
            {
                try
                {
                    searchLookUpEdit1.ShowPopup();
                    searchLookUpEdit1.ClosePopup();
                    if (searchLookUpEdit1View != null)
                    {
                        searchLookUpEdit1View.PopulateColumns();

                        foreach (DevExpress.XtraGrid.Columns.GridColumn col in searchLookUpEdit1View.Columns)
                        {
                            col.Visible = false;
                        }

                        if (searchLookUpEdit1View.Columns["TheSize"] != null)
                        {
                            searchLookUpEdit1View.Columns["TheSize"].Visible = true;
                            searchLookUpEdit1View.Columns["TheSize"].GroupIndex = 0;
                            searchLookUpEdit1View.Columns["TheSize"].VisibleIndex = 0;
                        }

                        if (searchLookUpEdit1View.Columns["TenSize"] != null)
                        {
                            searchLookUpEdit1View.Columns["TenSize"].Visible = true;
                            searchLookUpEdit1View.Columns["TenSize"].VisibleIndex = 1;
                            searchLookUpEdit1View.Columns["TenSize"].Caption = "Size";
                        }

                        searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
                        searchLookUpEdit1View.OptionsBehavior.AutoExpandAllGroups = true;
                        searchLookUpEdit1View.RefreshData();
                    }
                }
                catch
                {
                    // ignore
                }
            }

            // chạy selection trong BeginInvoke để chắc UI đã bind xong
            this.BeginInvoke(new Action(() =>
            {
                var view = searchLookUpEdit1View; // dùng view designer-generated trực tiếp
                if (view == null) return;

                if (_preselectedSizeIds != null && _preselectedSizeIds.Count > 0)
                {
                    view.BeginSelection();
                    view.ClearSelection();

                    for (int i = 0; i < view.DataRowCount; i++)
                    {
                        DataRow row = view.GetDataRow(i);
                        if (row == null) continue;
                        string maSize = row["MaSize"]?.ToString();
                        if (!string.IsNullOrEmpty(maSize) && _preselectedSizeIds.Contains(maSize))
                            view.SelectRow(i);
                    }

                    view.EndSelection();

                    // cập nhật selectedRowsSize và text hiển thị
                    selectedRowsSize.Clear();
                    foreach (int h in view.GetSelectedRows())
                    {
                        var dr = view.GetDataRow(h);
                        if (dr != null) selectedRowsSize.Add(dr);
                    }
                    searchLookUpEdit1.Text = string.Join(":", selectedRowsSize.Select(r => r["TenSize"].ToString()));
                }
            }));
        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            int[] selectedHandles = searchLookUpEdit1View.GetSelectedRows();
            List<string> sizeNames = new List<string>();

            foreach (int handle in selectedHandles)
            {
                var rowView = searchLookUpEdit1View.GetRow(handle) as DataRowView;
                if (rowView != null)
                {
                    BangSizeEntity size = new BangSizeEntity()
                    {
                        MaSize = rowView["MaSize"].ToString(),
                        SizeSanXuat = rowView["SizeSanXuat"].ToString(),
                        MaHang = _mh
                    };

                    SelectedSizes.Add(size);
                    sizeNames.Add(size.SizeSanXuat);
                }
            }

            // Ghép chuỗi các size lại bằng dấu :
            string selectedSizeString = string.Join(":", sizeNames);

            // Nếu muốn truyền chuỗi này về form cha thì có thể gán vào Tag
            this.Tag = selectedSizeString;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void searchLookUpEdit1_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //int[] selectedHandles = searchLookUpEdit1View.GetSelectedRows();
            //List<string> sizeNames = new List<string>();

            //foreach (int handle in selectedHandles)
            //{
            //    var rowView = searchLookUpEdit1View.GetRow(handle) as DataRowView;
            //    if (rowView != null)
            //    {
            //        sizeNames.Add(rowView["SizeSanXuat"].ToString());
            //    }
            //}

            //e.DisplayText = string.Join(":", sizeNames);

            if (selectedRowsSize == null || selectedRowsSize.Count == 0)
            {
                e.DisplayText = string.Empty;
                return;
            }

            var sortedList = selectedRowsSize
                                .Where(r => r["SizeSanXuat"] != DBNull.Value)
                                .OrderBy(r =>
                                {
                                    if (r.Table.Columns.Contains("Sort") && int.TryParse(r["Sort"].ToString(), out int sortVal))
                                        return sortVal;
                                    return int.MaxValue;
                                })
                                .ThenBy(r => r["SizeSanXuat"].ToString())
                                .ToList();

            e.DisplayText = string.Join(":", sortedList.Select(r => r["SizeSanXuat"].ToString()));
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            DataRow row = view.GetDataRow(e.ControllerRow); // cần using DevExpress.XtraGrid.Views.Grid;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (row != null && !selectedRowsSize.Contains(row))
                {
                    selectedRowsSize.Add(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                if (row != null)
                {
                    selectedRowsSize.Remove(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                selectedRowsSize.Clear();
                int[] selectedRowHandles = view.GetSelectedRows();
                foreach (int rowHandle2 in selectedRowHandles)
                {
                    if (rowHandle2 >= 0)
                    {
                        row = view.GetDataRow(rowHandle2);
                        if (row != null)
                        {
                            selectedRowsSize.Add(row);
                        }
                    }
                }
            }

            // Ghép các SizeSanXuat bằng dấu :
            //var selectedValues = new List<string>();
            //foreach (DataRow dr in selectedRowsSize)
            //{
            //    if (dr["SizeSanXuat"] != DBNull.Value)
            //        selectedValues.Add(dr["SizeSanXuat"].ToString());
            //}

            //string displayText = string.Join(":", selectedValues);

            var sortedList = selectedRowsSize
                                .Where(r => r["SizeSanXuat"] != DBNull.Value)
                                .OrderBy(r =>
                                {
                                        // Nếu có cột Sort, dùng giá trị đó
                                        if (r.Table.Columns.Contains("Sort") && int.TryParse(r["Sort"].ToString(), out int sortVal))
                                        return sortVal;
                                        // Nếu không có Sort, sắp theo SizeSanXuat
                                        return int.MaxValue;
                                })
                                .ThenBy(r => r["SizeSanXuat"].ToString()) // fallback nếu trùng sort
                                .ToList();

            // Hiển thị text theo thứ tự Sort
            string displayText = string.Join(":", sortedList.Select(r => r["SizeSanXuat"].ToString()));

            // Gán text hiển thị cho SearchLookUpEdit
            searchLookUpEdit1.Text = displayText;
        }

        private void searchLookUpEdit1View_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info == null)
                return;

            // Lấy giá trị của nhóm (ví dụ: "TheInSeam1")
            string groupValue = info.GroupValueText;

            // Gán lại caption hiển thị (bỏ tên cột đi, chỉ để lại giá trị)
            info.GroupText = string.Format("{0} :", groupValue);
        }
    }
}