using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Service.SYSTEM
{
    public class SystemDepartmentService
    {
        public async Task<List<SystemDepartmentEntity>> DepGetAll(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    HttpResponseMessage reponse = await client.GetAsync(url);
                    if (reponse.IsSuccessStatusCode)
                    {
                        //string productJsonString = reponse.Content.ReadAsStringAsync().Result.Replace("\\", "")
                        //                       .Trim(new char[1] { '"' }); 
                        string productJsonString = reponse.Content.ReadAsStringAsync().Result;

                        return JsonConvert.DeserializeObject<SystemDepartmentEntity[]>(productJsonString).ToList();
                    }
                    return new List<SystemDepartmentEntity>();

                }
            }
        }
        public async Task<string> AddData(string url, List<SystemDepartmentEntity> _list)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new Uri(url);
                    string serializedObj = JsonConvert.SerializeObject(_list);
                    HttpContent Content = new StringContent(serializedObj, Encoding.UTF8, "application/json");
                    HttpResponseMessage result = await client.PostAsync(url, Content);
                    string ss = string.Empty;
                    if (result.IsSuccessStatusCode)
                    {
                        ss = result.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' }); 
                    }
                    return ss;
                }
            }
        }
      
        public async Task<string> Deleted(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new Uri(url);
                    HttpResponseMessage result = await client.DeleteAsync(url);
                    string ss = string.Empty;
                    if (result.IsSuccessStatusCode)
                    {
                        ss = result.Content.ReadAsStringAsync().Result;
                    }
                    return ss;
                }
            }
        }
    }
}
