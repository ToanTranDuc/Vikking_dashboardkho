using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Service.ERP.QuanLyDonHang
{
    public class ErpNhap_TNCService
    { 
        public async Task<DataTable> GetNPL(string url)
        {
            using (CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler())
            {
                using (HttpClient client = HttpClientFactory.Create(customDelegatingHandler))
                {
                    HttpResponseMessage responseString = await client.GetAsync(url);
                    if (responseString.IsSuccessStatusCode)
                    {
                        try
                        {
                            string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\\\", "\\")
                                               .Trim(new char[1] { '"' });
                            if (!string.IsNullOrEmpty(jsonString))
                            {
                                var deserializedTable = (DataTable)JsonConvert.DeserializeObject(jsonString, typeof(DataTable));
                                return deserializedTable;
                            }
                            else
                                return new DataTable();
                        }
                        catch (JsonException js)
                        {
                        }
                        try
                        {
                            string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                               .Trim(new char[1] { '"' });
                            if (!string.IsNullOrEmpty(jsonString))
                            {
                                var deserializedTable = (DataTable)JsonConvert.DeserializeObject(jsonString, typeof(DataTable));
                                return deserializedTable;
                            }
                            else
                                return new DataTable();
                        }
                        catch (JsonException js)
                        {
                        }
                        try
                        {
                            string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\\\\"", "\\\"").Replace("\\\"", "\"")
                                               .Trim(new char[1] { '"' });
                            if (!string.IsNullOrEmpty(jsonString))
                            {
                                var deserializedTable = (DataTable)JsonConvert.DeserializeObject(jsonString, typeof(DataTable));
                                return deserializedTable;
                            }
                            else
                                return new DataTable();
                        }
                        catch (JsonException js)
                        {
                        }
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }
        public async Task<bool> AllowStatus(string url)
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
                        {
                            if (string.Compare(jsonString, "true") == 0)
                                return true;
                            return false;
                        }
                        else
                            return false;
                    }
                    throw new Exception(Properties.Resources.ServerFailure);
                }
            }
        }
        public async Task<string> DeleteSoDoEdit(string url)
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
                        ss = result.Content.ReadAsStringAsync().Result.Replace("\\\\\\", "\\\\")
                                               .Trim(new char[1] { '"' });
                    }
                    return ss;
                }
            }
        }
    }
}
