using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/ChiTietTungLenh")]
    public class ChiTietTungLenhApiController : ApiController
    {
        ChiTietTungLenhModel _model = new ChiTietTungLenhModel();
        [HttpGet]
        [Route("GetTongQuanTungLenh")]
        public DataTable GetTongQuanTungLenh(string pageIndex, string pageSize)
        {
            return _model.GetTongQuanTungLenh(pageIndex, pageSize);
        }
        [HttpGet]
        [Route("GetTongQuanTungLenhTheoMaHang")]
        public DataTable GetTongQuanTungLenhTheoMaHang(string maHang)
        {
            return _model.GetTongQuanTungLenhTheoMaHang(maHang);
        }
        [HttpGet]
        [Route("GetMaHang")]
        public DataTable GetMaHang()
        {
            return _model.GetMaHang();
        }
        [HttpGet]
        [Route("GetChiTietTungLenh")]
        public DataTable GetChiTietTungLenh(string maLenhSX)
        {
            DataTable modelResult = _model.GetChiTietTungLenh(maLenhSX);
            DataTable apiResult = new DataTable();
            apiResult.Columns.Add("MaDVSX", typeof(string));
            apiResult.Columns.Add("MaDH", typeof(string));
            apiResult.Columns.Add("PO", typeof(string));
            apiResult.Columns.Add("POID", typeof(string));
            apiResult.Columns.Add("MaMau", typeof(string));
            apiResult.Columns.Add("TenMau", typeof(string));
            apiResult.Columns.Add("DauSize", typeof(string));
            apiResult.Columns.Add("DauSizeID", typeof(string));

            List<DataRow> distinctSizes = modelResult.Select("1=1")
                                .AsEnumerable()
                                .GroupBy(row => new { Size = row["Size"], SizeId = row["SizeId"] })
                                .Select(group => group.First())
                                .ToList();

            for (int i = 0; i < distinctSizes.Count; i++)
            {
                apiResult.Columns.Add(distinctSizes[i]["Size"] + "@@Size@@" + distinctSizes[i]["SizeId"], typeof(int));
            }
            apiResult.Columns.Add("Tong", typeof(int));

            foreach (DataRow row in modelResult.Rows)
            {
                bool isExit = false;
                foreach (DataRow row2 in apiResult.Rows)
                {
                    if (row["MaDVSX"].ToString() == row2["MaDVSX"].ToString() &&
                        row["POID"].ToString() == row2["POID"].ToString() &&
                        row["MaMau"].ToString() == row2["MaMau"].ToString() &&
                        row["DauSizeID"].ToString() == row2["DauSizeID"].ToString()
                            )
                        isExit = true;
                }
                if (isExit == false)
                {
                    DataRow newRow = apiResult.NewRow();
                    newRow["MaDVSX"] = row["MaDVSX"];
                    newRow["MaDH"] = row["MaDH"];
                    newRow["PO"] = row["PO"];
                    newRow["POID"] = row["POID"];
                    newRow["MaMau"] = row["MaMau"];
                    newRow["TenMau"] = row["TenMau"];
                    newRow["DauSize"] = row["DauSize"];
                    newRow["DauSizeID"] = row["DauSizeID"];
                    for (int i = 8; i < apiResult.Columns.Count; i++)
                    {
                        newRow[apiResult.Columns[i].ColumnName] = 0;
                    }
                    newRow["Tong"] = 0;
                    apiResult.Rows.Add(newRow);
                }

                for (int i = 0; i < apiResult.Rows.Count; i++)
                {
                    DataRow row2 = apiResult.Rows[i];
                    if (row["MaDVSX"].ToString() == row2["MaDVSX"].ToString() &&
                        row["POID"].ToString() == row2["POID"].ToString() &&
                        row["MaMau"].ToString() == row2["MaMau"].ToString() &&
                         row["DauSizeID"].ToString() == row2["DauSizeID"].ToString()
                            )
                    {
                        string colName = row["Size"] + "@@Size@@" + row["SizeId"];
                        apiResult.Rows[i][colName] = int.Parse(row["SoLuong"].ToString());
                    }
                }
            }

            for (int i = 0; i < apiResult.Rows.Count; i++)
            {
                int sum = 0;
                for (int j = 8; j < apiResult.Columns.Count - 1; j++)
                    sum += int.Parse(apiResult.Rows[i][apiResult.Columns[j].ColumnName].ToString());
                apiResult.Rows[i]["Tong"] = sum;
            }

            return apiResult;
        }
    }
}