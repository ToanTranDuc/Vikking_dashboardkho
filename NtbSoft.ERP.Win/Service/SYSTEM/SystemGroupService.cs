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
    public class SystemGroupService
    {
        public async Task<List<SystemGroupEntity>> GroupGet(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    HttpResponseMessage responseString = await client.GetAsync(url);
                    if (responseString.IsSuccessStatusCode)
                    {
                        string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                        if (!string.IsNullOrEmpty(jsonString))
                            return JsonConvert.DeserializeObject<SystemGroupEntity[]>(jsonString).ToList();
                        else
                            return new List<SystemGroupEntity>();
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }

        public async Task<string> UpdateData(SystemGroupEntity item, string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new Uri(url);
                    string serializedObj = JsonConvert.SerializeObject(item);
                    HttpContent content = new StringContent(serializedObj, Encoding.UTF8, "application/json");
                    HttpResponseMessage result = await client.PostAsync(url, content);
                    if (result.IsSuccessStatusCode)
                        return result.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                    throw new Exception(Properties.Resources.ServerFailure);
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
                        ss = result.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                        return ss;
                    }
                    throw new Exception(Properties.Resources.ServerFailure);

                }
            }
        }
    }
}
