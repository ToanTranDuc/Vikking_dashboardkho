using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/ERPThuVienVT")]
    public class ERPThuVienVTController : ApiController
    {
        ERPMauVatTuModel _model1 = new ERPMauVatTuModel();
        ERPKhoVaiModel _model2 = new ERPKhoVaiModel();
        ERPDonViVTModel _model3 = new ERPDonViVTModel();

        //MauVT
        [HttpGet]
        [Route("GetAllMMVT")]
        public DataTable GetAll()
        {
            return _model1.GetALL();
        }
        [HttpGet]
        [Route("GetMMVT")]
        public DataTable GetMMVT(string makh, string mahang)
        {
            return _model1.Get(makh,mahang);
        }
        [HttpPost]
        [Route("PostMMVT")]
        public string PostMMVT(object tbl)
        {
            if (tbl == null) return "false";
            string json = JsonConvert.SerializeObject(tbl);
            DataTable tblmvt = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblmvt.Columns.Contains("isNew")) 
            {
                tblmvt.Columns.Remove("isNew");
            }
            return _model1.Post(tblmvt);
        }
        [HttpDelete]
        [Route("DeleteAllMMVT")]
        public DataTable DeleteAllMMVT( string mahang, string makh)
        {
            return _model1.DeleteAll(mahang, makh);
        }
        [HttpDelete]
        [Route("DeleteMMVT")]
        public string DeleteMMVT(int id)
        {
            return _model1.Delete(id);
        }

        //KhoVai


        [HttpGet]
        [Route("GetAllKV")]
        public DataTable GetAllKV()
        {
            return _model2.GetALL();
        }
        [HttpGet]
        [Route("GetKVNL")]
        public DataTable GetKVNL()
        {
            return _model2.GetNL();
        }
        [HttpGet]
        [Route("GetKVPL")]
        public DataTable GetKVPL()
        {
            return _model2.GetPL();
        }
        [HttpGet]
        [Route("GetKH")]
        public DataTable GetKH()
        {
            return _model2.GetKH();
        }
        [HttpGet]
        [Route("GetHH")]
        public DataTable GetHH(string makh)
        {
            return _model2.GetHH(makh);
        }
        [HttpPost]
        [Route("PostKV")]
        public string PostKV(object tblkv, string action="")
        {
            if (tblkv == null) return "false";
            string json = JsonConvert.SerializeObject(tblkv);
            DataTable tblkhovai = JsonConvert.DeserializeObject<DataTable>(json);
            return _model2.Post(tblkhovai, action);
        }

        [HttpDelete]
        [Route("DeleteAllKV")]
        public DataTable DeleteAllKV(string mahang, string makh)
        {
            return _model2.DeleteAll(mahang, makh);
        }
        [HttpDelete]
        [Route("DeleteKV")]
        public string DeleteKV(string id,string action,string UserName)
        {
            return _model2.Delete(id, action, UserName);
        }

        //DonViVT
        [HttpGet]
        [Route("GetDVVT")]
        public DataTable GetDVVT()
        {
            return _model3.Get();
        }
        [HttpGet]
        [Route("GetCheckDV")]
        public DataTable GetCheckDV(string madvvt)
        {
            return _model3.GetCheckDV(madvvt);
        }
        [HttpPost]
        [Route("PostDVT")]
        public string PostDVVT(object tbldvt)
        {
            if (tbldvt == null) return "false";
            string json = JsonConvert.SerializeObject(tbldvt);
            DataTable tbldonvivt = JsonConvert.DeserializeObject<DataTable>(json);
            return _model3.Post(tbldonvivt);
        }
        [HttpDelete]
        [Route("DeleteDVT")]
        public string DeleteDVVT(string id, string user)
        {
            return _model3.Delete(id, user);
        }
        [HttpGet]
        [Route("GetCheckKV")]
        public DataTable GetCheckKV(string khovaiid)
        {
            return _model2.GetCheckKV(khovaiid);
        }

        #region Phú
        [HttpGet]
        [Route("GetMH")]
        public DataTable GetMH(string makh)
        {
            return _model1.GetMH(makh);
        }
        [HttpGet]
        [Route("GetChungKV")]
        public DataTable GetChungKV(string action,string para="", string para1="", string para2 = "", string para3 = "", string para4 = "", string para5 = "")
        {
            return _model2.GetChungKV(action,para, para1,para2,para3,para4,para5);
        }
        #endregion
        #region Luân
        [HttpPost]
        [Route("UpLoadImg")]
        public async Task<List<dynamic>> UpLoadImg([FromBody] JArray imageDatas, string getFileName, string LibFolder)
        {
            var results = new List<dynamic>();

            try
            {
                string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "ImageThuVien", LibFolder);
                string folderName;
                if (getFileName == null) folderName = "";
                else folderName = getFileName.Replace("|", "-").Replace(" ", "-");
                string newFolderPath = Path.Combine(baseDirectory, folderName);

                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }

                var tasks = new List<Task>();

                foreach (var imageData in imageDatas)
                {
                    string imgString = imageData["img"]?.ToString();
                    string imgName = imageData["name"]?.ToString();
                    string ID = imageData["ID"]?.ToString();

                    if (!string.IsNullOrEmpty(imgString) && !string.IsNullOrEmpty(imgName))
                    {
                        string[] imageDataParts = imgString.Split(',');

                        if (imageDataParts.Length == 2)
                        {
                            string base64Data = imageDataParts[1];
                            byte[] imageBytes = Convert.FromBase64String(base64Data);
                            string sanitizedFileName = imgName.Replace(" ", "").Replace("|", "-") + ".png";
                            string fullImagePath = Path.Combine(newFolderPath, sanitizedFileName);
                            string publicPath = Path.Combine("/Images/ImageThuVien/" + LibFolder, folderName, sanitizedFileName).Replace("\\", "/");

                            tasks.Add(Task.Run(() =>
                            {
                                System.IO.File.WriteAllBytes(fullImagePath, imageBytes);
                                lock (results)
                                {
                                    results.Add(new
                                    {
                                        name = imgName,
                                        url = publicPath,
                                        ID = ID
                                    });
                                }
                            }));
                        }
                        else
                        {
                            Console.WriteLine("Invalid image data format.");
                        }
                    }
                }
                //await RemoveSignature(MaLenh);
                await Task.WhenAll(tasks);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading images: {ex.Message}");
            }

            return results;
        }


        public void RemoveImg(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string imagePath = HostingEnvironment.MapPath(path);

                if (File.Exists(imagePath))
                {
                    try
                    {
                        File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        [HttpPost]
        [Route("Get")]
        public DataTable Get(string action, [FromBody] DataTable tbl, string para = "", string para2 = "")
        {
            return _model2.GetChungKV(action, tbl, para, para2);
        }
        #endregion
    }
}