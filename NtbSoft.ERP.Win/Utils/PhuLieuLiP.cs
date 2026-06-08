using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    public static class PhuLieuLiP
    {
        public static void dtXuatEX(DataTable dtSave, ExcelWorksheet worksheet, bool flagFilter = true)
        {
            ExcelRange range = worksheet.Cells;
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            range = worksheet.Cells["A4"]; range.Value = "MaVatTu"; range.Style.Font.Bold = true;
            range = worksheet.Cells["B4"]; range.Merge = true; range.Value = "TenVatTu"; range.Style.Font.Bold = true;
            range = worksheet.Cells["C4"]; range.Merge = true; range.Value = "CodeMau"; range.Style.Font.Bold = true;
            range = worksheet.Cells["D4"]; range.Merge = true; range.Value = "TenCodeMau"; range.Style.Font.Bold = true;
            range = worksheet.Cells["E4"]; range.Merge = true; range.Value = "TenKeToan"; range.Style.Font.Bold = true;
            range = worksheet.Cells["F4"]; range.Merge = true; range.Value = "DonViTinh"; range.Style.Font.Bold = true;
            range = worksheet.Cells["G4"]; range.Merge = true; range.Value = "Nhom";
            int row = 5;
            int index = 0;
            int rowMerge = 10;
            int indexMerge = 0;
            bool checkCountMerge = false;
            foreach (DataRow rows in dtSave.Rows)
            {
                worksheet.Cells[row, 1].Value = rows["MaVatTu"].ToString(); worksheet.Column(1).AutoFit();
                 worksheet.Cells[row, 2].Value = rows["TenVatTu"].ToString(); range.Style.Font.Bold = true; worksheet.Column(2).AutoFit();
                worksheet.Cells[row, 3].Value = rows["CodeMau"].ToString(); range.Style.Font.Bold = true; worksheet.Column(3).AutoFit();
                worksheet.Cells[row, 4].Value = rows["TenCodeMau"].ToString(); range.Style.Font.Bold = true; worksheet.Column(4).AutoFit();
                worksheet.Cells[row, 5].Value = rows["MaKeToan"].ToString(); range.Style.Font.Bold = true; worksheet.Column(5).AutoFit();
                worksheet.Cells[row, 6].Value = rows["DonViTinh"].ToString(); range.Style.Font.Bold = true; worksheet.Column(6).AutoFit();
                worksheet.Cells[row, 7].Value = rows["Nhom"].ToString(); range.Style.Font.Bold = true; worksheet.Column(7).AutoFit();
                index++;
                row++;
            }
            for (int col = 1; col <= 7; col++) // A=1, B=2, ..., G=7
            {
                worksheet.Column(col).AutoFit();
            }
        }

        public static void dtXuatEXNhom(DataTable dtSave, ExcelWorksheet worksheet, bool flagFilter = true)
        {
            ExcelRange range = worksheet.Cells;
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            range = worksheet.Cells["A4"]; range.Value = "MaNhom"; range.Style.Font.Bold = true;
            range = worksheet.Cells["B4"]; range.Merge = true; range.Value = "TenNhom"; range.Style.Font.Bold = true;
            int row = 5;
            int index = 0;
            int rowMerge = 10;
            int indexMerge = 0;
            bool checkCountMerge = false;
            foreach (DataRow rows in dtSave.Rows)
            {
                worksheet.Cells[row, 1].Value = rows["MaNhom"].ToString(); worksheet.Column(1).AutoFit();
                worksheet.Cells[row, 2].Value = rows["TenNhom"].ToString(); range.Style.Font.Bold = true; worksheet.Column(2).AutoFit();
                index++;
                row++;
            }
            for (int col = 1; col <= 7; col++) // A=1, B=2, ..., G=7
            {
                worksheet.Column(col).AutoFit();
            }
        }
    }
}
