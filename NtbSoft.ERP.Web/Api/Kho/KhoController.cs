
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Models.THIETBI;
using NtbSoft.ERP.Web.Repository.THIETBI;

namespace NtbSoft.ERP.Web.Api.THIETBI
{
    //[HMACAuthentication]
    [RoutePrefix("api/Kho")]
    public class KhoController : ApiController
    {
        IKhoRepository _repo = new KhoRepository();

        [HttpGet]
        [Route("Get")]
        public List<KhoViewModels> Get()
        {
            return _repo.Get();
        }
        [HttpGet]
        [Route("GetPhieuDD")]
        public DataTable GetPhieuDD(string matb)
        {
            return _repo.GetPhieuDD(matb);
        }
        [HttpGet]
        [Route("GetDetail")]
        public DataTable/*List<KhoDetaiViewModels>*/ GetDetail()
        {
            return _repo.GetDetail();
        }
        [HttpGet]
        [Route("DeleteMaTB")]
        public DataTable DeleteMaTB(string makho)
        {
            return _repo.DeleteMaTB(makho);
        }
        [HttpPost]
        [Route("")]
        public string Post(List<KhoViewModels> item)
        {
            return _repo.Post(item);
        }
        [HttpDelete]
        [Route("{IdKho}")]
        public string Delete(string IdKho)
        {
            return _repo.Delete(IdKho);
        }

    }
}