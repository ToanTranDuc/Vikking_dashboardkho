using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.KeHoach;
using NtbSoft.ERP.Web.Models.THIETBI;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/ThongKeDongThung")]
    public class ThongKeDongThungController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2, string Para3, string Para4, string Para5,string Para6 = "",string Para7 = "",string Para8="")
        {
            return new ThongKeDongThungModel().Get(action, Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8);
        }
        [HttpGet]
        [Route("GetNK")]
        public DataTable GetNK(string action, string Para1, string Para2, string Para3, string Para4, string Para5 = "")
        {
            return new ThongKeDongThungModel().GetNK(action, Para1, Para2, Para3, Para4, Para5);
        }
        [HttpGet]
        [Route("GetCTDH")]
        public CTDHResult GetCTDH(string action, string Para1, string Para2, string Para3, string Para4, string Para5)
        {
            DataTable tbl = new ThongKeDongThungModel().Get(action, Para1, Para2, Para3, Para4, Para5);
            DataTable dtTable = ThongKeDongThung.ProcessSttTrung(tbl);
            DataTable tblToToal = ThongKeDongThung.sumToTalPCS(tbl);
            return new CTDHResult { ChiTiet = dtTable, Total = tblToToal };
        }
        [HttpGet]
        [Route("GetCTXH")]
        public DataTable GetCTXH(string Para1, string Para2, string Para3)
        {
            DataTable tbl = new ThongKeDongThungModel().GetChiTiet("GetCTPhieuXHTon", Para1, Para2, Para3);
            DataTable dtTable = new ThongKeDongThungModel().GetChiTiet("GetCTPhieuXH", Para1, Para2, Para3);
            tbl.Merge(dtTable);
            DataTable dataTable = ThongKeDongThung.ProcessSttTrung1(tbl);
            return dataTable;
        }
        [HttpGet]
        [Route("GetCTXHTK")]
        public DataTable GetCTXHTK(string Para1, string Para2, string Para3)
        {
            DataTable tbl = new ThongKeDongThungModel().GetCTXHTK("GetChiTietXH", Para1, Para2, Para3);
            DataTable dtTable = new ThongKeDongThungModel().GetCTXHTK("GetChiTietXHTon", Para1, Para2, Para3);
            DataTable dtTable1 = new ThongKeDongThungModel().GetCTXHTK("GetCTXHChuyenKho", Para1, Para2, Para3);
            tbl.Merge(dtTable);
            tbl.Merge(dtTable1);
            DataTable dataTable = ThongKeDongThung.ProcessSttTrung1(tbl);
            return dataTable;
        }
        [HttpGet]
        [Route("GetCTNK")]
        public DataTable GetCTNK(string action, string Para1, string Para2, string Para3, string Para4, string Para5 = "")
        {
            DataTable tbl = new ThongKeDongThungModel().GetNK(action, Para1, Para2, Para3, Para4, Para5);
            DataTable dtTable = ThongKeDongThung.ProcessSttTrung(tbl);
            return dtTable;
        }
        [HttpPost]
        [Route("PostNK")]
        public string PostNK(dynamic data)
        {
            string Para1 = data.DonHang.ToString();
            string Para2 = data.MaHang.ToString();
            string Para3 = data.MaPKL.ToString();
            string Para4 = data.NgayNhapKho.ToString();
            string Para5 = data.User.ToString();
            return new ThongKeDongThungModel().PostNK(Para1, Para2, Para3, Para4, Para5);
        }

        [HttpGet]
        [Route("GetExcel")]
        public HttpResponseMessage GetExcel(string action, string Para1, string Para2, string Para3, string Para4, string Para5, string Para6)
        {
            try
            {
                DataTable tbl = new ThongKeDongThungModel().Get(action, Para1, Para2, Para3, Para4, Para5);

                DateTime currentTime = DateTime.Now;

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(Para2 + "_" + Para3);
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    ExcelRange range = worksheet.Cells;
                    range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "TEX-GIANG JOINT STOCK COMPANY"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "PACKING LIST"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["A1:B3"]; range.Merge = true;
                    Addpicutre(worksheet, "texgiang.jpg", 0, 0, 30);

                    range = worksheet.Cells["D4"]; range.Value = "STYLE :";
                    range = worksheet.Cells["e4:f4"]; range.Merge = true; range.Value = Para6;
                    range = worksheet.Cells["D5"]; range.Value = "Shipper: :";
                    range = worksheet.Cells["D6"]; range.Value = "Invoice No : ";
                    range = worksheet.Cells["D7"]; range.Value = "Consignee : ";
                    DataTable dtTable = ThongKeDongThung.ProcessSttTrung(tbl);
                    ThongKeDongThung.dtXuatEX(dtTable, worksheet);



                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"ThongKeDonHang{currentTime}.xlsx";

                    // Create a response message with the file as content
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        private void Addpicutre(ExcelWorksheet worksheet, string sign, int row, int col, int toado)
        {
            var pathFolder = HttpContext.Current.Server.MapPath("~/Content");
            string imagePath = Path.Combine(pathFolder, sign);
            // Chèn ảnh vào worksheet
            if (File.Exists(imagePath))
            {
                FileInfo imageFile = new FileInfo(imagePath);
                ExcelPicture picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);
                picture.SetPosition(row, 5, col, toado);
                picture.SetSize(70, 50);
            }
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable dt)
        {
            return new ThongKeDongThungModel().Post(dt);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int Para1)
        {
            return new ThongKeDongThungModel().Delete(Para1);
        }
    }
}