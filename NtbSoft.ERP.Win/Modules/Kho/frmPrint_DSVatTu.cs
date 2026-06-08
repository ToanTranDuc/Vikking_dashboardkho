using DevExpress.XtraReports.UI;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmPrint_DSVatTu : DevExpress.XtraEditors.XtraForm
    {
        DataTable _lstPrint;
        List<ExcelPicture> listBarcode = new List<ExcelPicture>();
        string filePath = string.Empty;
        public frmPrint_DSVatTu(string filePathInput)
        {
            InitializeComponent();
            filePath = filePathInput;
            LoadDataNhap();
        }
        private void LoadDataNhap()
        {
            DataTable _dataTB = ReadExcelFile(this.filePath);
            if (_dataTB == null || _dataTB.Rows.Count == 0) return;
            //List<DataTable> lstData = SplitDataTable(_dataTB, 50);
            List<DataTable> lstData = SplitDataTable(_dataTB, 6);
           List<List<ExcelPicture>> lstBC = new List<List<ExcelPicture>>();
            int batchSize = 1; // Số phần tử tối đa trong mỗi danh sách con

            for (int i = 0; i < listBarcode.Count; i += batchSize)
            {
                List<ExcelPicture> subList = listBarcode
                    .Skip(i)
                    .Take(batchSize)
                    .ToList();
                lstBC.Add(subList);
            }
            string a = "";


            XtraReport masterReport = new XtraReport
            {
                Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20)
            };

            DetailBand detailBand = new DetailBand();
            masterReport.Bands.Add(detailBand);

            int reportsPerRow = 2;
            float subReportWidth = (masterReport.PageWidth - masterReport.Margins.Left - masterReport.Margins.Right) / reportsPerRow;
            float subReportHeight = 200; // Chiều cao tùy chỉnh cho báo cáo con
            int index = 1;
            for (int i = 0; i < lstData.Count; i++)
            {
                DataTable dataTB = lstData[i];
                XRSubreport subReport = new XRSubreport
                {
                    ReportSource = new frmPrintReviewTheBai(dataTB, lstBC[i]),
                    SizeF = new System.Drawing.SizeF(subReportWidth, subReportHeight)
                };
                subReport.Borders = DevExpress.XtraPrinting.BorderSide.All;
                subReport.BorderWidth = 2;
                subReport.BorderColor = System.Drawing.Color.Black;
                subReport.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;

                // Đặt vị trí cho từng subreport
                subReport.LocationF = new System.Drawing.PointF(
                    (i % reportsPerRow) * subReportWidth,   // Vị trí ngang
                    (i / reportsPerRow) * subReportHeight); // Vị trí dọc

                detailBand.Controls.Add(subReport);
                if (index==10)
                {
                    XRPageBreak pageBreak = new XRPageBreak
                    {
                        LocationF = new System.Drawing.PointF(0, subReport.LocationF.Y + subReportHeight)
                    };
                    detailBand.Controls.Add(pageBreak);
                    index = 0;
                }
                index += 1;
            }

            documentViewer.DocumentSource = masterReport;
            masterReport.CreateDocument();






        }
        public DataTable ReadExcelFile(string filePath)
        {
            DataTable dataTable = new DataTable();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; 
                int rowCount = worksheet.Dimension.Rows; 
                int colCount = worksheet.Dimension.Columns; 

                // Add columns to the DataTable
                for (int col = 1; col <= colCount; col++)
                {
                    dataTable.Columns.Add("Col_" + col.ToString()); 
                }
                // Add rows to the DataTable
                for (int row = 1; row <= rowCount; row++)
                {
                 
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= colCount/2; col++)
                    {
                       
                        dataRow[col - 1] = worksheet.Cells[row, col].Text;
                       
                    }
                    dataTable.Rows.Add(dataRow);
                   
                   
                }
                for (int row = 1; row <= rowCount; row++)
                {

                    DataRow dataRow2 = dataTable.NewRow();
                    int index = 1;
                    for (int col = (colCount / 2) + 1; col <= colCount; col++)
                    {

                        dataRow2[index - 1] = worksheet.Cells[row, col].Text;
                        index++;

                    }
                    dataTable.Rows.Add(dataRow2);

                }

                // lấy danh sách hình ảnh barcode
                int tongSoTheBai = 2 * (dataTable.Rows.Count / 6);

                for (int i = 0; i < tongSoTheBai; i+=2)
                {
                    if (worksheet.Drawings.Count > i && worksheet.Drawings[i] is OfficeOpenXml.Drawing.ExcelPicture excelPicture)
                    {
                        // Lấy hình ảnh từ Excel gán vào listBarcode
                        listBarcode.Add(excelPicture);
                   
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i < tongSoTheBai; i+=2)
                {
                    if (worksheet.Drawings.Count > i && worksheet.Drawings[i] is OfficeOpenXml.Drawing.ExcelPicture excelPicture)
                    {
                        // Lấy hình ảnh từ Excel gán vào listBarcode
                        listBarcode.Add(excelPicture);
                    
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return dataTable;
        }
        static List<DataTable> SplitDataTable(DataTable originalTable, int maxRows)
        {
            List<DataTable> tables = new List<DataTable>();
            int totalRows = originalTable.Rows.Count;

            for (int i = 0; i < totalRows; i += maxRows)
            {
                DataTable newTable = originalTable.Clone(); // Clone structure
                for (int j = i; j < i + maxRows && j < totalRows; j++)
                {
                    newTable.ImportRow(originalTable.Rows[j]);
                }
                tables.Add(newTable);
            }
            return tables;
        }
        public List<ExcelPicture> MergeOddEvenIndexes(List<ExcelPicture> list)
        {
            List<ExcelPicture> oddIndexList = new List<ExcelPicture>();
            List<ExcelPicture> evenIndexList = new List<ExcelPicture>();

            for (int i = 0; i < list.Count; i++)
            {
                if (i % 2 == 0)
                {
                    evenIndexList.Add(list[i]);
                }
                else
                {
                    oddIndexList.Add(list[i]);
                }
            }

            // Thêm các phần tử chẵn vào sau các phần tử lẻ
            evenIndexList.AddRange(oddIndexList);

            return evenIndexList;
        }
        private void printPreviewBarItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}
