using NtbSoft.ERP.Libs;
using NtbSoft.ERP.Win.Account;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Utils
{
    public class CustomDelegatingHandler : DelegatingHandler
    {
        NtbSoft.ERP.Libs.Security _objSecurity;
        //MD5Password _md5Crypt;
     
        public CustomDelegatingHandler()
        {
            _objSecurity = new NtbSoft.ERP.Libs.Security();
            //_md5Crypt = new MD5Password();
            
        }
        System.Configuration.AppSettingsReader settingsReader =
                                               new AppSettingsReader();
      

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string APPId = (string)settingsReader.GetValue("APPId",
                                                             typeof(String));

            string APIKey = (string)settingsReader.GetValue("APIKey",
                                                             typeof(String));

            //string _Path = _objSecurity.mRegKey + "\\" + _objSecurity.mSessionID.ToString("0000");
            //object obj = _objSecurity.get_RegistryKey(_Path, "APPId");
            //if(obj==null)
            //{
            //    frmActiveKey frm = new frmActiveKey();
            //    frm.ShowDialog();
            //    //return request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            //obj = _objSecurity.get_RegistryKey(_Path, "APPId");


            //string key = CryptorEnginePro.Decrypt(obj.ToString());
            //string clientStr = key.Remove(0, 10);
            //string newKey = CryptorEnginePro.GetRendomValue() + clientStr;
            //string APIKey = CryptorEnginePro.Encrypt(newKey);

            HttpResponseMessage response = null;
            //string key = CryptorEnginePro.Decrypt(obj.ToString());
            string requestContentBase64String = string.Empty;//_md5Crypt.Encrypt(key,true);



            string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

            string requestHttpMethod = request.Method.Method;



            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();


            string nonce = Guid.NewGuid().ToString("N");

            //if (request.Content != null)
            //{
            //    byte[] content = await request.Content.ReadAsByteArrayAsync();
            //    MD5 md5 = MD5.Create();
            //    //Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
            //    byte[] requestContentHash = md5.ComputeHash(content);
            //    requestContentBase64String = Convert.ToBase64String(requestContentHash);
            //}

            //Creating the raw signature string
            string signatureRawData = String.Format("{0}{1}{2}{3}{4}{5}", APIKey, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyByteArray = Convert.FromBase64String(APIKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (HMACMD5 hmac = new HMACMD5(secretKeyByteArray))
            {
                //byte[] signatureBytes = hmac.(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signature);
                //Setting the values in the Authorization header using custom scheme (amx)
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("amx", string.Format("{0}:{1}:{2}:{3}", APPId, requestSignatureBase64String, nonce, requestTimeStamp));
            }
            //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("amx", string.Format("{0}:{1}:{2}:{3}", APPId, signatureRawData, nonce, requestTimeStamp));
            response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
