using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Utils;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using DevExpress.SpreadsheetSource;
using Newtonsoft.Json;

namespace NtbSoft.ERP.Win.Modules.Erp.Kehoach
{
    public partial class frmErpImport_DMNL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        string _maDH = string.Empty, _malenhsanxuat = string.Empty, _makh = string.Empty;
        bool _ChkCT = false, _ChkTH = false;
        CustomImportDataExcel _customImport;
        // Sự kiện để thông báo khi Form đóng và trả dữ liệu về
        public event EventHandler<string> DataClosed;
        System.Data.DataTable tbl_sheet;
        private HttpClientExtension _clientExtension;
        public frmErpImport_DMNL(string maDH, string malenhsanxuat, bool ChkCT, bool ChkTH, string makh)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _maDH = maDH;
            _malenhsanxuat = malenhsanxuat;
            _makh = makh;
            _ChkCT = ChkCT;
            _ChkTH = ChkTH;
            _customImport = new CustomImportDataExcel();
            textEdit1.Enabled = false;
            _clientExtension = new HttpClientExtension();
        }
        private void btGetLink_Click(object sender, EventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                tbl_sheet = new DataTable();
                AddColumn_tbl_Sheet();
                textEditLink.Text = Sfd.FileName;
                CreateDefaultLookUp(textEditLink.Text);
                textEdit1.Enabled = true;
            }
        }

        private void AddColumn_tbl_Sheet()
        {
            tbl_sheet.Columns.Add("Name");
        }

        private void CreateDefaultLookUp(string pathExecel)
        {
            ISpreadsheetSource spreadsheetSource = SpreadsheetSourceFactory.CreateSource(pathExecel);
            IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
            int worksheetcount = worksheetCollection.Count();
            for (int i = 0; i < worksheetcount; i++)
            {
                DataRow r = tbl_sheet.NewRow();
                r["Name"] = worksheetCollection[i].Name;
                tbl_sheet.Rows.Add(r);
            }
            textEdit1.Properties.DataSource = tbl_sheet;
            textEdit1.Properties.ValueMember = "Name";
            textEdit1.Properties.DisplayMember = "Name";
            //textEdit1.EditValue = 0;
        }
        private async void btImport_Click(object sender, EventArgs e)
        {
            try
            {
                btImport.Enabled = false;
                string newFilePath = string.Empty;
                if (string.IsNullOrEmpty(textEditLink.Text))
                {
                    XtraMessageBox.Show(Properties.Resources.DataValid, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    btImport.Enabled = true;
                    return;
                }
                if (textEdit1.EditValue == null)
                {
                    MessageBox.Show("Chưa chọn sheet chứa thông tin cấp phát định mức nguyên liệu", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string urlNPL = string.Format("{0}", URL + $"VatTuChiTiet/Get");
                if (string.IsNullOrEmpty(textEditLink.Text))
                {
                    XtraMessageBox.Show(Properties.Resources.DataValid, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    btImport.Enabled = true;
                    return;
                }
                //string Path = textEditLink.Text;
                //string ExcelExtension = System.IO.Path.GetExtension(Path);
                string sheetIndex = this.textEdit1.EditValue.ToString();
                string url = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPL", _maDH.ToString(), _malenhsanxuat.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    string ss = await _customImport.Import_DMNPL(_maDH, _malenhsanxuat, _makh, textEditLink.Text, sheetIndex, urlNPL, URL, _ChkCT, _ChkTH);
                    if (string.Compare(ss, "True") == 0)
                    {
                        //clsWaitForm.ShowSuccessForm(this, 3000);
                    }
                    else
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(ss, "[a-zA-Z]"))
                        {
                            XtraMessageBox.Show(ss, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                        else
                        {
                            //string url = string.Format("{0}/{1}", URL + ResourceURL.UrlErpLenhSX, ss);
                            //string ms = await _serviceBase.Deleted(url);
                        }
                    }
                    btImport.Enabled = true;
                    this.Close();
                    DataClosed?.Invoke(this, ss);
                }
                else 
                {
                    string ss = await _customImport.Import_CT(_maDH, _malenhsanxuat, _makh, textEditLink.Text, sheetIndex, urlNPL, URL, _ChkCT, _ChkTH);
                    if (string.Compare(ss, "True") == 0)
                    {
                        clsWaitForm.ShowSuccessForm(this, 3000);
                    }
                    else
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(ss, "[a-zA-Z]"))
                        {
                            XtraMessageBox.Show(ss, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                        else
                        {
                            //string url = string.Format("{0}/{1}", URL + ResourceURL.UrlErpLenhSX, ss);
                            //string ms = await _serviceBase.Deleted(url);
                        }
                    }
                    btImport.Enabled = true;
                    this.Close();
                    DataClosed?.Invoke(this, ss);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}