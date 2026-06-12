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

namespace BaoCaoTongHop.Api.BaoCaoChatLuong
{
    [RoutePrefix("api/sqlQueryBaoCao")]
    public class BaoCaoChatLuongController : ApiController
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        [HttpPost]
        [Route("BaoCaoChatLuong")]
        public DataTable BCChatLuong(RunStoredViewModel objView)
        {
            string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
            string sqlQuery_Str_tb1 = string.Format(@"EXEC {0} {1}", "sp_ReportChatLuongThang_getReport_tb1", objView.paramStr);
            string sqlQuery_Str_tb2 = string.Format(@"EXEC {0} {1}", "sp_ReportChatLuongThang_getReport_tb2", objView.paramStr);
            string sqlQuery_Str_tb3 = string.Format(@"EXEC {0} {1}", "sp_ReportChatLuongThang_getReport_tb3", objView.paramStr);
            DataTable dtView = getDBFromOtherServer(SV_api, sqlQuery_Str_tb1); //getData( "select * from Product").Result;
            DataTable dtTruocUi = getDBFromOtherServer(SV_api, sqlQuery_Str_tb2);
            DataTable dtSauUi = getDBFromOtherServer(SV_api, sqlQuery_Str_tb3);
            DataTable tbl = new DataTable();
            tbl = BindingData(dtView, dtTruocUi, dtSauUi);
            return tbl;
        }
        private DataTable BindingData(DataTable tbl, DataTable tblTruocUi, DataTable tblSauUi)
        {
            if (tbl != null && tbl.Rows.Count > 0)
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    var Line = dr["Chuyền"].ToString();
                    var MaHang = dr["Mã hàng"].ToString();
                    var drSauUi = tblSauUi.AsEnumerable().Where(x => x["Chuyền"].ToString() == Line && x["Mã hàng"].ToString() == MaHang).FirstOrDefault();
                    var drTruocUi = tblTruocUi.AsEnumerable().Where(x => x["Chuyền"].ToString() == Line && x["Mã hàng"].ToString() == MaHang).FirstOrDefault();
                    List<Tuple<string, int>> lstSLLoi = new List<Tuple<string, int>>();
                    List<Tuple<string, int>> lstSLLoiSauUi = new List<Tuple<string, int>>();
                    for (int i = 7; i < tblTruocUi.Columns.Count - 1; i++)
                    {
                        if (drTruocUi == null) break;
                        if (drTruocUi[i] == null) continue;
                        if (drTruocUi[i].ToString() != "")
                        {
                            string tenLoi = tblTruocUi.Columns[i].ColumnName;
                            //Console.WriteLine(tenLoi);
                            Tuple<string, int> Loi = Tuple.Create(tblTruocUi.Columns[i].ColumnName, Convert.ToInt32(drTruocUi[i]));
                            lstSLLoi.Add(Loi);
                        }
                    }
                    for (int i = 7; i < tblSauUi.Columns.Count; i++)
                    {
                        if (drSauUi == null) break;
                        if (drSauUi[i].ToString() != "")
                        {
                            string tenLoi = tblSauUi.Columns[i].ColumnName;
                            Tuple<string, int> Loi = Tuple.Create(tblSauUi.Columns[i].ColumnName, Convert.ToInt32(drSauUi[i]));
                            lstSLLoiSauUi.Add(Loi);
                        }
                    }
                    lstSLLoi = lstSLLoi.OrderByDescending(x => x.Item2).Take(3).ToList();
                    lstSLLoiSauUi = lstSLLoiSauUi.OrderByDescending(x => x.Item2).Take(3).ToList();
                    string NoiComTruocUi = "";
                    string NoiComSauUi = "";
                    foreach (Tuple<string, int> Loi in lstSLLoi)
                        NoiComTruocUi += Loi.Item1 + " + ";
                    foreach (Tuple<string, int> Loi in lstSLLoiSauUi)
                        NoiComSauUi += Loi.Item1 + " + ";
                    //   dr["LoiHuNoiCom"] = string.Format("Trước ủi: {0} \nSau ủi: ", NoiComTruocUi.Substring(0, NoiComTruocUi.Length - 3));

                    dr["LoiHuNoiCom"] = string.Format("Trước ủi: {0} \nSau ủi: {1} ", NoiComTruocUi != "" ? NoiComTruocUi.Substring(0, NoiComTruocUi.Length - 3) : "", NoiComSauUi != "" ? NoiComSauUi.Substring(0, NoiComSauUi.Length - 3) : "");
                }
            }
            return tbl;
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