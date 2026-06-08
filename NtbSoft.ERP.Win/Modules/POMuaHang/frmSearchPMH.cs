using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmSearchPMH : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();

        public event EventHandler<DataTable> OnDataUpdate;
        private HttpClientExtension _clientExtension;
        private int tabSelected = -1;

        public frmSearchPMH(int x, int y,int TabSelected)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            SetPosition(x, y);
            _clientExtension = new HttpClientExtension();
            tabSelected = TabSelected;
        }

        private void SetPosition(int x, int y)
        {
            this.Location = new Point(x -300, y-30);
        }

        private void frmSearchPMH_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {

            DataTable _tblSearch = new DataTable();
            if(tabSelected == 0)
            {
                _tblSearch = SearchDonHang();
            }
            else if(tabSelected == 1)
            {
                _tblSearch = SearchMuaHangMMTB();
            }
        
       
            OnDataUpdate?.Invoke(this, _tblSearch);
        }

        private DataTable SearchMuaHangMMTB()
        {
            DataTable tblSearch = new DataTable();
            try
            {
                string _pomh = (textEdit1.EditValue != null) ? textEdit1.EditValue.ToString() : "";
                string _ncc = (textEdit2.EditValue != null) ? textEdit2.EditValue.ToString() : "";
                DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
                DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);
                string para4 = FromDate == DateTime.MinValue ? "NONE" : FromDate.ToString("yyyy-MM-dd");
                string para5 = ToDate == DateTime.MinValue ? "NONE" : ToDate.ToString("yyyy-MM-dd");

                string urldt = $"{URL}NhaCC/GePMHMMTB?action=SearchPOMH&para1={_pomh}&para2={_ncc}&para3={txtCreater.EditValue}&para4={para4}&para5={para5}";

                string jsondt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldt); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsondt);
                if (tbl != null && tbl?.Rows.Count > 0)
                {
                    tblSearch = tbl.Copy();
                   
                }
            }
            catch (Exception ex)
            {

            }

            return tblSearch;
        }

        private DataTable SearchDonHang()
        {
            DataTable tblSearch = new DataTable();
            try
            {
                string _pomh = (textEdit1.EditValue != null) ? textEdit1.EditValue.ToString() : "";
                string _ncc = (textEdit2.EditValue != null) ? textEdit2.EditValue.ToString() : "";
                DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
                DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);
                //string para4 = FromDate == DateTime.MinValue ? "NONE" : FromDate.ToString("yyyy-MM-dd");
                //string para5 = ToDate == DateTime.MinValue ? "NONE" : ToDate.ToString("yyyy-MM-dd");

                string urldt = $"{URL}NhaCC/GePMH?action=SearchPOMH&para1={_pomh}&para2={_ncc}&para3={txtCreater.EditValue}&para4={txtItems.EditValue}&para5=NONE";

                string jsondt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldt); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsondt);
                if (tbl != null && tbl?.Rows.Count > 0)
                {
                    if (FromDate == DateTime.MinValue && ToDate == DateTime.MinValue)
                    {
                        tblSearch = tbl.Copy();
                    }
                    else
                    {
                        tblSearch = tbl.Clone();
                        foreach (DataRow row in tbl.Rows)
                        {
                            if (string.IsNullOrEmpty(row["NgayDuKienHV"]?.ToString())) continue;

                            DateTime NgayDuKienHV = clsForrmatUtils.ConvertDate(row["NgayDuKienHV"]).Date;
                            if (NgayDuKienHV == DateTime.MinValue) continue;

                            bool inRange = true;

                          
                            if (FromDate.Date != DateTime.MinValue && ToDate.Date == DateTime.MinValue)
                                inRange = NgayDuKienHV >= FromDate.Date;

                           
                            else if (FromDate.Date == DateTime.MinValue && ToDate.Date != DateTime.MinValue)
                                inRange = NgayDuKienHV <= ToDate.Date;

                       
                            else if (FromDate.Date != DateTime.MinValue && ToDate.Date != DateTime.MinValue)
                            {
                                inRange = NgayDuKienHV >= FromDate.Date && NgayDuKienHV <= ToDate.Date;
                            }
                              
                            

                            if (inRange)
                                tblSearch.ImportRow(row);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
           
            return tblSearch;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            textEdit1.EditValue = null;
            textEdit2.EditValue = null;
            fromDate.EditValue = null;
            toDate.EditValue = null;
            txtCreater.EditValue = null;

        }
    }
}