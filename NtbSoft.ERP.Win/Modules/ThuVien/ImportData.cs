
using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class ImportData : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        public static List<KHDT_StoreEntity> lstExcel = new List<KHDT_StoreEntity>();
        DataTable dtQuiCach = new DataTable();
        public ImportData()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }
        protected override void OnLoad(EventArgs e)
        {


           


        }


        private DataTable LoadQuiCach()
        {
            DataTable resulit = new DataTable();
            string url = $"{URL}KHDT_Store/Get?Action=GetQuiCach&Para1=ALL&Para2=para2";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                resulit = JsonConvert.DeserializeObject<DataTable>(json);
            }
            return resulit;
        }

        private void btGetLink_Click(object sender, EventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                textEditLink.Text = Sfd.FileName;
            }
        }

        private void btImport_Click(object sender, EventArgs e)
        {
            btImport.Enabled = false;
            //string newFilePath = string.Empty;
            if (string.IsNullOrEmpty(textEditLink.Text))
            {
                XtraMessageBox.Show(Properties.Resources.DataValid, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                btImport.Enabled = true;
                return;
            }

            string Path = textEditLink.Text;
            ReadExcelKHDT_Store(Path);
            
        }


        private void ReadExcelKHDT_Store(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                lstExcel.Clear();
                List<string> lstColurm = new List<string> { "Carton Number ", "PO", "Supplier",
                    "Style \nNO", "Color", "Color number", "QTY" , "Carton", "ToTal \nPieces", "Store Number" , "Column5" , "Column3", "Column2" ,"NW","GW"};
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;
               
                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A13:ZZ500");
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();
                DataTable dtSave = new DataTable();
                dtSave = source.ToDataTable();
                AddList(dtSave, lstColurm);
                Save();

            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        private void AddList(DataTable dtSave,List<string>lstColurm)
        {
             dtQuiCach = LoadQuiCach();
            int SapXep = 0;string PO = string.Empty, POID = string.Empty, MaHang = string.Empty, MaMau = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            foreach (DataRow row in dtSave.Rows)
            {
                MaHang = row[6].ToString();
                PO = row[3].ToString();
                MaMau = row[7].ToString();
                if (!Int32.TryParse(row[lastIdxCol-4].ToString(), out int SLuongThung))
                {

                }

                foreach (DataColumn col in dtSave.Columns)
                {

                    if (string.IsNullOrEmpty(row[0]?.ToString()) && string.IsNullOrEmpty(row[2]?.ToString()))
                    {
                        break;
                    } 

                    if (!lstColurm.Any(x=>x.Trim()== col.ColumnName.ToString().Trim()) && !string.IsNullOrEmpty(row[col].ToString()))
                    {
                        string Size = col.ColumnName;
                        var query = dtQuiCach.AsEnumerable().FirstOrDefault(x => x["KiHieu"]?.ToString() == row[lastIdxCol]?.ToString() && x["StyleID"]?.ToString() == ReplaceSpecialCharacters(MaHang));
                        if(query == null)
                        {
                            XtraMessageBox.Show($"Qui Cách {row[lastIdxCol]?.ToString()} Mã Hàng {MaHang} Chưa Khai Báo !!! Vui Lòng Thử Lại ", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            return;
                        }
                        if(!Int32.TryParse(row[col].ToString(),out int SLuong))
                        {
                            continue;
                        }
                        else
                        {
                          
                            lstExcel.Add(new KHDT_StoreEntity
                            {

                                StyleID = ReplaceSpecialCharacters(MaHang),
                                MaHang = MaHang,
                                PO = PO,
                                POID = PO,
                                Store = row[4].ToString(),
                                SizeID = $"SIZE_{ReplaceSpecialCharacters(Size)}",
                                Size = Size,
                                ColorID = $"MAU_{ReplaceSpecialCharacters(MaMau)}",
                                MaMau = MaMau,
                                SLuong = SLuong,
                                SLThung = SLuongThung,
                                QuiCach = query["MaQuiCach"]?.ToString(),
                               
                                SapXep = SapXep



                            }); 

                          
                        }
                        
                        
                    }

                   
                }
                SapXep++;




            }
           


         
        }


        private void Save()
        {
            string msResult = "";
            if (lstExcel?.Count == 0)
            {
                return;
            }


            string url = string.Format("{0}", URL + "KHDT_Store/Post");
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstExcel) ; }).Result;


            if (msResult.ToLower() == "true")
            {            
                clsWaitForm.ShowSuccessForm(this, 2000);
                this.Close();
            }
            else XtraMessageBox.Show(msResult);

            //lstMaHangUpdate.Clear();
        }

        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }

    }
}