using NtbSoft.ERP.Model.Kho;
using System;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/DashboardKho")]
    public class DashboardKhoController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var fromDate = (tuNgay ?? DateTime.Today).Date;
            var toDate = (denNgay ?? fromDate.AddDays(14)).Date;
            return DashboardKhoModel.GetChuanBiVe(fromDate, toDate);
        }

        [HttpGet]
        [Route("GetRacks")]
        public IHttpActionResult GetRacks()
        {
            try
            {
                DataTable dt = DashboardKhoModel.GetWarehouseEfficiency();
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetSummary")]
        public IHttpActionResult GetSummary()
        {
            try
            {
                DataTable dt = DashboardKhoModel.GetWarehouseEfficiencySummary();
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetCustomers")]
        public IHttpActionResult GetCustomers()
        {
            try
            {
                DataTable dt = DashboardKhoModel.GetWarehouseCustomers();
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetOverallCapacity")]
        public IHttpActionResult GetOverallCapacity()
        {
            try
            {
                DataTable dt = DashboardKhoModel.GetOverallCapacity();
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet]
[Route("GetChuanBiXuat")]
public IHttpActionResult GetChuanBiXuat()
{
    try
    {
        var toDate = DateTime.Today.AddDays(14);
        var fromDate = DateTime.Today.AddDays(-30);
        DataTable dt = DashboardKhoModel.GetChuanBiXuat(fromDate, toDate);


        var list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();
        foreach (System.Data.DataRow row in dt.Rows)
        {
            var dict = new System.Collections.Generic.Dictionary<string, object>();
            foreach (System.Data.DataColumn col in dt.Columns)
            {
                dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
            }
            list.Add(dict);
        }

        return Ok(list);
    }
    catch (Exception ex)
    {
        return BadRequest($"Error: {ex.Message}");
    }
}

[HttpGet]
[Route("GetDangXuat")]
public IHttpActionResult GetDangXuat(DateTime? tuNgay = null, DateTime? denNgay = null)
{
    try
    {
        var toDate = (denNgay ?? DateTime.Today).Date;
        var fromDate = (tuNgay ?? toDate.AddDays(-14)).Date;
        if (fromDate > toDate)
        {
            var swap = fromDate;
            fromDate = toDate;
            toDate = swap;
        }
        DataTable dt = DashboardKhoModel.GetDangXuat(fromDate, toDate);

        var list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();
        foreach (System.Data.DataRow row in dt.Rows)
        {
            var dict = new System.Collections.Generic.Dictionary<string, object>();
            foreach (System.Data.DataColumn col in dt.Columns)
            {
                dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
            }
            list.Add(dict);
        }

        return Ok(list);
    }
    catch (Exception ex)
    {
        return BadRequest($"Error: {ex.Message}");
    }
}
    }
}
