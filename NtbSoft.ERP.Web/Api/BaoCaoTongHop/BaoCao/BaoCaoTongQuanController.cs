using BaoCaoTongHop.Models.GetData;
using BaoCaoTongHop.Service;
using Newtonsoft.Json;
using NtbSoft.ERP.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BaoCaoTongHop.Api.BaoCao
{
    [RoutePrefix("api/BaoCaoTongQuan")]
    public class BaoCaoTongQuanController : ApiController
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        [HttpPost]
        [Route("GetData")]
        public DataTable BCChatLuong(RunStoredViewModel objView)
        {
            string sqlQuery_Str = string.Format(@"EXEC {0} {1}", "sp_DB_BaoCaoTongKhu", objView.paramStr);
           // sqlQuery_Str = "EXEC sp_DB_BaoCaoTongKhu @Action = 'Get', @para1 = '2024-09-04', @para2 = '2024-10-09', @para3 = 'null'";
            DataTable dt_TanHuong1 = getDBFromOtherServer(ConfigurationManager.AppSettings["DVSX_1"].ToString(), sqlQuery_Str);
            DataTable dt_TanHuong2 = getDBFromOtherServer(ConfigurationManager.AppSettings["DVSX_5"].ToString(), sqlQuery_Str);
            DataTable dt_ChoGao = getDBFromOtherServer(ConfigurationManager.AppSettings["DVSX_2"].ToString(), sqlQuery_Str);
            DataTable dt_CaiLay = getDBFromOtherServer(ConfigurationManager.AppSettings["DVSX_3"].ToString(), sqlQuery_Str);

            List<DataTable> lst_Dt = new List<DataTable>();
            lst_Dt.Add(dt_TanHuong1);
            lst_Dt.Add(dt_TanHuong2);
            lst_Dt.Add(dt_CaiLay);
            lst_Dt.Add(dt_ChoGao);


            // 
            DataTable db = new DataTable();
            db.Columns.Add("KhuVuc", typeof(string));
            db.Columns.Add("NangSuat", typeof(string));
            db.Columns.Add("SLLoi", typeof(string));
            db.Columns.Add("TLLoi", typeof(string));
            db.Columns.Add("SuaDat", typeof(string));
            db.Columns.Add("SoLaoDong", typeof(string));
            db.Columns.Add("DoanhThu", typeof(string));
            
            foreach (DataTable item_dt in lst_Dt)
            {
                DataRow row = db.NewRow();
                row["KhuVuc"] = "Tân Hương 1";
                row["NangSuat"] = item_dt.Rows[0]["NangSuat"];
                row["SLLoi"] = item_dt.Rows[0]["SLLoi"];
                row["TLLoi"] = item_dt.Rows[0]["TLLoi"];
                row["SuaDat"] = item_dt.Rows[0]["SuaDat"];
                row["SoLaoDong"] = item_dt.Rows[0]["SoLaoDong"];
                row["DoanhThu"] = item_dt.Rows[0]["DoanhThu"];
                db.Rows.Add(row);
            }
            DataRow row_total = db.NewRow();
            int nang_suat = 0;
            int sl_loi = 0;
            int sua_dat = 0;
            double so_lao_dong = 0.0;
            double doanh_thu = 0.0;

            foreach (DataTable item_dt in lst_Dt)
            {
                nang_suat += Convert.ToInt32(item_dt.Rows[0]["NangSuat"].ToString());
                sl_loi += Convert.ToInt32(item_dt.Rows[0]["SLLoi"].ToString());
                sua_dat += Convert.ToInt32(item_dt.Rows[0]["SuaDat"].ToString());
                so_lao_dong += Convert.ToDouble(item_dt.Rows[0]["SoLaoDong"].ToString());
                doanh_thu += Convert.ToDouble(item_dt.Rows[0]["DoanhThu"].ToString());
            }

            row_total["KhuVuc"] = "TGI";
            row_total["NangSuat"] = nang_suat;
            row_total["SLLoi"] = sl_loi;
            row_total["TLLoi"] = (sl_loi + sua_dat == 0) ? 0 : (100 - ((sua_dat * 100) / (sl_loi + sua_dat)));
            row_total["SuaDat"] = sua_dat;
            row_total["SoLaoDong"] = so_lao_dong;
            row_total["DoanhThu"] = doanh_thu;
            db.Rows.Add(row_total);
            
            return db;
        }
        public static DataTable getDBFromOtherServer(string server_api, string queryString)
        {
            QueryObjViewModel objViewM = new QueryObjViewModel { query = queryString };
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(server_api, objViewM); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
                return null;
            if (tbl.Rows.Count == 0)
                return null;
            return tbl;
        }
    }
}