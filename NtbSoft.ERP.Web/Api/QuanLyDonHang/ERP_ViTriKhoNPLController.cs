using Newtonsoft.Json;
using NtbSoft.ERP.Model.ThuVien;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
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

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/ViTriKhoNPL")]
    public class ERP_ViTriKhoNPLController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataSet Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new ERP_ViTriKhoNPLModel().Get(action, para1, para2, para3, para4, para5, para6);
        }
        [HttpGet]
        [Route("GetSoDoKhoDragDrop")]
        public DataTable GetSoDoKhoDragDrop(string action, string para1 = "", string para2 = "", string para3 = "")
        {
            return new ERP_ViTriKhoNPLModel().GetSoDoKhoDragDrop(action, para1, para2, para3);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string para1, dynamic data)
        {
            string type = "";
            string action = "";
            object listData;
            string json = JsonConvert.SerializeObject(data);
            switch (para1)
            {
                case "1":
                    listData = JsonConvert.DeserializeObject<List<DayNPL>>(json);
                    type = "@typeTableDay";
                    action = "PostDay";
                    break;

                case "2":
                    listData = JsonConvert.DeserializeObject<List<KeNPL>>(json);
                    type = "@typeTableKe";
                    action = "PostKe";
                    break;

                case "3":
                    listData = JsonConvert.DeserializeObject<List<TangNPL>>(json);
                    type = "@typeTableTang";
                    action = "PostTang";
                    break;
                default:
                    listData = JsonConvert.DeserializeObject<List<ONPL>>(json);
                    type = "@typeTableO";
                    action = "PostO";
                    break;
            }

            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);

            return new ERP_ViTriKhoNPLModel().Post(action, tbl, type);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string para1, string para2, dynamic data)
        {
            string type = "";
            string action = "";
            object listData;
            string json = JsonConvert.SerializeObject(data);
            switch (para1)
            {
                case "1":
                    listData = JsonConvert.DeserializeObject<List<DayNPL>>(json);
                    type = "@typeTableDay";
                    action = "DeleteDay";
                    break;

                case "2":
                    listData = JsonConvert.DeserializeObject<List<KeNPL>>(json);
                    type = "@typeTableKe";
                    action = "DeleteKe";
                    break;

                case "3":
                    listData = JsonConvert.DeserializeObject<List<TangNPL>>(json);
                    type = "@typeTableTang";
                    action = "DeleteTang";
                    break;
                default:
                    listData = JsonConvert.DeserializeObject<List<ONPL>>(json);
                    type = "@typeTableO";
                    action = "DeleteO";
                    break;
            }

            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);

            return new ERP_ViTriKhoNPLModel().Delete(action, tbl, type, para2);
        }
    }
    public class DayNPL
    {
        public int ID { get; set; }              // NOT NULL
        public string DayID { get; set; }       // NULL
        public string TenDay { get; set; }      // NULL
        public string NguoiTao { get; set; }
        public int Module { get; set; }
        public int Status { get; set; }
    }
    public class KeNPL
    {
        public int ID { get; set; }             // NOT NULL
        public string KeID { get; set; }       // NULL
        public string TenKe { get; set; }      // NULL
        public string DayID { get; set; }      // NULL
        public double? Dai { get; set; }        // NULL
        public double? Rong { get; set; }       // NULL
        public double? Cao { get; set; }        // NULL
        public string NguoiTao { get; set; }
        public string TextKe { get; set; }
        public int Module { get; set; }
    }
    public class TangNPL
    {
        public int ID { get; set; }             // NOT NULL
        public string TangID { get; set; }     // NULL
        public string TenTang { get; set; }    // NULL
        public string DayID { get; set; }
        public string KeID { get; set; }       // NULL

        public double? Cao { get; set; }        // NULL
        public double? Rong { get; set; }       // NULL
        public double? Dai { get; set; }        // NULL
        public string NguoiTao { get; set; }
        public string TextTang { get; set; }
        public int Module { get; set; }
    }


    public class ONPL
    {
        public int ID { get; set; }             // NOT NULL
        public string OID { get; set; }        // NULL
        public string TenO { get; set; }       // NULL
        public string DayID { get; set; }     // NULL
        public string KeID { get; set; }     // NULL
        public string TangID { get; set; }     // NULL

        public double? Dai { get; set; }        // NULL
        public double? Cao { get; set; }        // NULL
        public double? Rong { get; set; }       // NULL
        public string NguoiTao { get; set; }
        public string TextO { get; set; }
        public int Module { get; set; }

    }

}