using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Service.SYSTEM
{
    public class GetDataService
    {
        public async Task<DataTable> GetDataTable(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    HttpResponseMessage responseString = await client.GetAsync(url);
                    if (responseString.IsSuccessStatusCode)
                    {
                        string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\\\\\", "\\\\")
                                               .Trim(new char[1] { '"' });
                        if (!string.IsNullOrEmpty(jsonString))
                            return (DataTable)JsonConvert.DeserializeObject(jsonString, typeof(DataTable));
                        else
                            return new DataTable();
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }

        public async Task<string> GetStr(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    HttpResponseMessage reponse = await client.GetAsync(url);
                    if (reponse.IsSuccessStatusCode)
                    {
                        string productJsonString = reponse.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                        return productJsonString;
                    }
                    return string.Empty;
                }
            }
        }

        public async Task<string> PostData(string url, DataTable dataUpdate)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new Uri(url);
                    string serializedObj = JsonConvert.SerializeObject(dataUpdate);
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
        public async Task<DataTable> GETBYPOST(string url, DataTable dataUpdate)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new Uri(url);
                    string serializedObj = JsonConvert.SerializeObject(dataUpdate);
                    HttpContent Content = new StringContent(serializedObj, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseString = await client.PostAsync(url, Content);
                    if (responseString.IsSuccessStatusCode)
                    {
                        string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\\\\\", "\\\\")
                                               .Trim(new char[1] { '"' });
                        if (!string.IsNullOrEmpty(jsonString))
                            return (DataTable)JsonConvert.DeserializeObject(jsonString, typeof(DataTable));
                        else
                            return new DataTable();
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }
    }
}
