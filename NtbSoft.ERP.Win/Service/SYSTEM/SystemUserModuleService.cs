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
    public class SystemUserModuleService
    {
        public async Task<List<SystemUserModuleEntity>> UserModuleGet(string url)
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
                            return JsonConvert.DeserializeObject<SystemUserModuleEntity[]>(jsonString).ToList();
                        else
                            return new List<SystemUserModuleEntity>();
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }

        public async Task<List<SystemUserModuleDVSX>> UserModuleGetDVSX(string url)
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
                            return JsonConvert.DeserializeObject<SystemUserModuleDVSX[]>(jsonString).ToList();
                        else
                            return new List<SystemUserModuleDVSX>();
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }
        public async Task<List<SystemUserWeb>> UserModuleWeb(string url)
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
                            return JsonConvert.DeserializeObject<SystemUserWeb[]>(jsonString).ToList();
                        else
                            return new List<SystemUserWeb>();
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }
        public async Task<string> AddData(List<SystemUserModuleConfig> tempTB, string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new System.Uri(url);
                    String serializedProduct = JsonConvert.SerializeObject(tempTB);
                    HttpContent Content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    HttpResponseMessage result = await client.PostAsync(url, Content);
                    string ss = string.Empty;
                    if (result.IsSuccessStatusCode)
                    {
                        ss = result.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                    }
                    else
                    {
                        throw new Exception(Properties.Resources.ServerFailure);
                    }
                    return ss;
                }
            }

        }

        public async Task<string> AddDataDVSX(List<SystemUserModuleDVSX> tempTB, string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new System.Uri(url);
                    String serializedProduct = JsonConvert.SerializeObject(tempTB);
                    HttpContent Content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    HttpResponseMessage result = await client.PostAsync(url, Content);
                    string ss = string.Empty;
                    if (result.IsSuccessStatusCode)
                    {
                        ss = result.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                    }
                    else
                    {
                        throw new Exception(Properties.Resources.ServerFailure);
                    }
                    return ss;
                }
            }

        }

       


    }
}
