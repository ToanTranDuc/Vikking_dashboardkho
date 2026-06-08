using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using Quartz;
using Quartz.Impl;



namespace NtbSoft.ERP.Win.Utils
{
    public class TyGiaSetUp
    {
        private static IScheduler _scheduler;
        private static readonly JobKey _jobKey =
            new JobKey("job_tygia", "group_tygia");
        private static readonly TriggerKey _triggerKey =
            new TriggerKey("trigger_tygia", "group_tygia");
        public static async Task startAsync()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await _scheduler.Start();

            if (!await _scheduler.CheckExists(_jobKey))
            {
                IJobDetail job = JobBuilder.Create<TyGia>()
                    .WithIdentity(_jobKey)
                    .StoreDurably(true)
                    .Build();

                await _scheduler.AddJob(job, false);
            }

            // load giờ từ config
            TimeSpan time = new TyGiaConfig().loadTyGiaTime();

            await updateTrigger(time);
        }
        public static async Task updateTrigger(TimeSpan time)
        {
            if (_scheduler == null)
                _scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            string cron = $"0 {time.Minutes} {time.Hours} * * ?";

            ITrigger newTrigger = TriggerBuilder.Create()
                .WithIdentity(_triggerKey)
                .ForJob(_jobKey)
                .WithCronSchedule(cron)
                .Build();

            if (await _scheduler.CheckExists(_triggerKey))
            {
                await _scheduler.RescheduleJob(_triggerKey, newTrigger);
            }
            else
            {
                await _scheduler.ScheduleJob(newTrigger);
            }
        }
    }
    public class TyGia : IJob
    {
        private System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        private SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        private string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        private KeyDownControlHandler keyDownControlHandler;

        private DataTable tygiatheoAPITable;

        public async Task Execute(IJobExecutionContext context)
        {
            if (!getTyGiaHomNay())
            {
                initTyGiaTable();
                bool ok = getTyGiaFromVietcombankAPI();
                if (ok)
                {
                    saveTyGia();
                }
            }
            //initTyGiaTable();
            //bool ok = getTyGiaFromVietcombankAPI();
            //if (ok)
            //{
            //    saveTyGia();
            //}
        }
        public TyGia()
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        
        private void initTyGiaTable()
        {
            tygiatheoAPITable = new DataTable();
            tygiatheoAPITable.Columns.Add("MaTienTe", typeof(string));
            tygiatheoAPITable.Columns.Add("BanTienMat", typeof(decimal));
            tygiatheoAPITable.Columns.Add("NgayTao", typeof(DateTime));
        }
        private bool getTyGiaFromVietcombankAPI()
        {
            try
            {
                string apiUrl = "https://portal.vietcombank.com.vn/Usercontrols/TVPortal.TyGia/pXML.aspx";

                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(apiUrl);
                request.Method = "GET";
                request.Timeout = 15000;

                using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var doc = new System.Xml.XmlDocument();
                    doc.Load(stream);

                    var dateNode = doc.SelectSingleNode("//DateTime");
                    DateTime ngayTyGia = DateTime.Now;

                    //if (!DateTime.TryParse(dateNode?.InnerText, out ngayTyGia))
                    //{
                    //    ngayTyGia = DateTime.Today;
                    //}

                    //if (ngayTyGia < new DateTime(1753, 1, 1))
                    //{
                    //    ngayTyGia = DateTime.Today;
                    //}

                    var exrateNodes = doc.SelectNodes("//Exrate");
                    if (exrateNodes == null || exrateNodes.Count == 0)
                        return false;

                    tygiatheoAPITable.Clear();

                    foreach (System.Xml.XmlNode node in exrateNodes)
                    {
                        string maTienTe = node.Attributes["CurrencyCode"]?.Value;
                        string banTienMatStr = node.Attributes["Sell"]?.Value;

                        decimal banTienMat;
                        if (!decimal.TryParse(
                                banTienMatStr,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out banTienMat))
                        {
                            continue;
                        }

                        DataRow row = tygiatheoAPITable.NewRow();
                        row["MaTienTe"] = maTienTe;
                        row["BanTienMat"] = banTienMat;
                        row["NgayTao"] = ngayTyGia;

                        tygiatheoAPITable.Rows.Add(row);
                    }
                }

                return tygiatheoAPITable.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                // log lỗi nếu cần
                return false;
            }
        }
        private void saveTyGia()
        {
            var request = XuLyVTRequestPost.createDefault("TyGia");
            request.Action = "createtygia";
            foreach (DataRow row in tygiatheoAPITable.Rows)
            {
                DataRow newRow = request.TypeTable.NewRow();
                newRow["MaTienTe"] = row["MaTienTe"];
                newRow["BanTienMat"] = row["BanTienMat"];
                newRow["NgayTao"] = row["NgayTao"];
                request.TypeTable.Rows.Add(newRow);
            }
            string urlGetListDataTable = URL + "TyGiaMH/Post";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private bool getTyGiaHomNay()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "gettygia";
            request.Parameter6 = DateTime.Today;
            string urlGetListDataTable = URL + "TyGiaMH/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public class TyGiaConfig
    {
        public TimeSpan loadTyGiaTime()
        {
            string timeStr = ConfigurationManager.AppSettings["TyGiaTime"];

            TimeSpan time;
            if (!TimeSpan.TryParse(timeStr, out time))
            {
                time = new TimeSpan(8, 0, 0); // default
            }
            return time;
        }
    }
    
}