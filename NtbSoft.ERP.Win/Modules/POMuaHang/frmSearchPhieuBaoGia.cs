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
    public partial class frmSearchPhieuBaoGia : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();

        public event EventHandler<DataTable> OnDataUpdate;
        private HttpClientExtension _clientExtension;
        private int tabSelected = -1;

        public frmSearchPhieuBaoGia(int x, int y,int TabSelected)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            SetPosition(x, y);
            _clientExtension = new HttpClientExtension();
            tabSelected = TabSelected;
        }

        private void SetPosition(int x, int y)
        {
            this.Location = new Point(x - 350, y-30);
        }

        private void frmSearchPMH_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {

            DataTable _tblSearch = new DataTable();
            _tblSearch = SearchDonHang();
            OnDataUpdate?.Invoke(this, _tblSearch);
        }

     

        private DataTable SearchDonHang()
        {
            DataTable tblSearch = new DataTable();
            try
            {
           
                string _ncc = (txtNhaCC.EditValue != null) ? txtNhaCC.EditValue.ToString() : "";
                DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
                DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);
                //string para4 = FromDate == DateTime.MinValue ? "NONE" : FromDate.ToString("yyyy-MM-dd");
                //string para5 = ToDate == DateTime.MinValue ? "NONE" : ToDate.ToString("yyyy-MM-dd");

                string urldt = "";
                if(tabSelected == 0)
                {
                    urldt=  $"{URL}PhieuBaoGia/GET?action=SearchPBG&para1={txtItems.EditValue ?? ""}&para2={_ncc}&para3=&para4=NONE&para5=NONE";
                }
                else if (tabSelected == 1)
                {
                    urldt = $"{URL}PhieuBaoGiaMMTB/GET?action=SearchPBG&para1={txtItems.EditValue ?? ""}&para2={_ncc}&para3=&para4=NONE&para5=NONE";
                }
                
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
                           

                            DateTime NgayDBHieuLuc = clsForrmatUtils.ConvertDate(row["NgayBDHieuLuc"]).Date;
                            DateTime NgayHetHieuLuc = clsForrmatUtils.ConvertDate(row["NgayHetHieuLuc"]).Date;
                            if (NgayDBHieuLuc == DateTime.MinValue) continue;

                            bool inRange = true;

                          
                            

                            if (FromDate.Date != DateTime.MinValue && ToDate.Date == DateTime.MinValue)
                                inRange = NgayHetHieuLuc >= FromDate.Date;


                            else if (FromDate.Date == DateTime.MinValue && ToDate.Date != DateTime.MinValue)
                                inRange = NgayHetHieuLuc >= ToDate.Date;


                            else if (FromDate.Date != DateTime.MinValue && ToDate.Date != DateTime.MinValue)
                            {
                                inRange = NgayDBHieuLuc >= FromDate.Date && NgayHetHieuLuc >= ToDate.Date;
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
            txtItems.EditValue = null;
            txtNhaCC.EditValue = null;
            fromDate.EditValue = null;
            toDate.EditValue = null;
           

        }
    }
}