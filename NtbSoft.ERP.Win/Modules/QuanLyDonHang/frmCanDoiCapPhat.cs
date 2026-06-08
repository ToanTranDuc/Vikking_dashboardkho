using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    public partial class frmCanDoiCapPhat : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public frmCanDoiCapPhat()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            initSearchLookUp();
            CreateSearchLookUpCayVai();
        }
        private void initSearchLookUp()
        {

            searchLookUpEditCayVai.Properties.DisplayMember = "ChiTiet";
            searchLookUpEditCayVai.Properties.ValueMember = "ValueMember";

        }
        private void CreateSearchLookUpCayVai()
        {
            string url = $"{URL}CanDoiCapPhat/Get?Action=GETCAYVAI";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditCayVai.Properties.DataSource = null;
                searchLookUpEditCayVai.EditValue = null;
                return;
            }


            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditCayVai.Properties.DataSource = tbl;

            searchLookUpEditCayVai.RefreshEditValue();
            searchLookUpEditCayVai.Refresh();
         
        }

        private void searchLookUpEditCayVai_EditValueChanged(object sender, EventArgs e)
        {
            loadData();
        }
        private void loadData()
        {
            if (searchLookUpEditCayVai.EditValue == null || searchLookUpEditCayVai.EditValue.ToString() == "")
            {
                MessageBox.Show("Vui lòng chọn cây vải", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataRow dr = gV_SearchLookUpEditCayVai.GetFocusedDataRow();
            if (dr == null) return;
            string url = $"{URL}CanDoiCapPhat/Get?Action=GETTONG&para={dr["MaVTID"]}&para2={dr["MauVTID"]}&para3={dr["KhoVaiID"]}&para4={dr["MaNhom"]}&para5={dr["MaDVVT"]}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {

                gCTong.DataSource = null;
                return;
            }

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);


            gCTong.DataSource = tbl;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditCayVai.EditValue == null || searchLookUpEditCayVai.EditValue.ToString() == "") return;
          
            loadData();
        }

        private void gVTong_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn35)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn6)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gVTong.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gVTong.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void gV_SearchLookUpEditCayVai_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn19)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
           
        }
    }
    
}