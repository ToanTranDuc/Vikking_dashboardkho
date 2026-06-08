using NtbSoft.ERP.Libs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BaoCaoTongHop.Service
{
    public class CustomDelegatingHandler : DelegatingHandler
    {
        Security _objSecurity;
        public CustomDelegatingHandler()
        {
            _objSecurity = new Security();
        }
        System.Configuration.AppSettingsReader settingsReader =
                                               new AppSettingsReader();

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            string requestContentBase64String = string.Empty;
            string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
            string requestHttpMethod = request.Method.Method;
            response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}