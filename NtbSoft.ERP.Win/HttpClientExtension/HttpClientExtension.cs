using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win
{
   public class HttpClientExtension
    {
       public async Task<string> GetAsnyc(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    HttpResponseMessage reponse = await client.GetAsync(url);
                    if (reponse.IsSuccessStatusCode)
                    {
                        string jsonResponse = reponse.Content.ReadAsStringAsync().Result.Replace(@"\\", "")
                                               .Trim(new char[1] { '"' });

                        return jsonResponse;//JsonConvert.DeserializeObject<object>(jsonResponse);
                    }
                    return string.Empty;
                }
            }
        }

        public async Task<string> PostAsync(string url, object _list)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new Uri(url);
                    string serializedObj = JsonConvert.SerializeObject(_list);
                    HttpContent Content = new StringContent(serializedObj, Encoding.UTF8, "application/json");
                    HttpResponseMessage result = await client.PostAsync(url, Content);
                    string resultResponse = string.Empty;
                    if (result.IsSuccessStatusCode)
                    {
                        resultResponse = result.Content.ReadAsStringAsync().Result.Replace(@"\\", "")
                                               .Trim(new char[1] { '"' });
                    }
                    return resultResponse;
                }
            }
        }

        public async Task<string> DeletedAsync(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    client.BaseAddress = new Uri(url);
                    HttpResponseMessage result = await client.DeleteAsync(url);
                    string resultResponse = string.Empty;
                    if (result.IsSuccessStatusCode)
                    {
                        resultResponse = result.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                    }
                    return resultResponse;
                }
            }
        }

    }
}
