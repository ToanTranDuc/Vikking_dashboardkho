using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Models.THIETBI;
using NtbSoft.ERP.Web.Repository.THIETBI;
using NtbSoft.ERP.Web.Repository.R.Kho;
using NtbSoft.ERP.Entity.Kho;
using Newtonsoft.Json;
namespace NtbSoft.ERP.Web.Controllers
{
    //[HMACAuthentication]
    [RoutePrefix("api/KhoNPL")]
    public class KhoNPLController : ApiController

    {
        IKhoRepository _repo = new KhoRepository();
        IPhuLieuRepository _repoPL = new PhuLieuRepository();
        INguyenLieuRepository _repoNL = new NguyenLieuRepository();
        // GET: KhoNPL
        [HttpGet]
        [Route("Get")]
        public List<KhoViewModels> Get()
        {
            return _repo.Get();
        }
        //GET: KhoNPL
       [HttpGet]
       [Route("GetNhomNPL")]
        public DataTable GetNhomNPL()
        {
            return _repo.GetNhomNPL();
        }
        //GET: KhoNPL
        [HttpGet]
        [Route("GetNhomNL")]
        public DataTable GetNhomNL()
        {
            return _repo.GetNhomNL();
        }
        //GET: KhoNPL
        [HttpGet]
        [Route("GetNhomPL")]
        public DataTable GetNhomPL()
        {
            return _repo.GetNhomPL();
        }

        
        [HttpGet]
        [Route("GetNL")]
        public DataTable GetNL()
        {
            return _repoNL.GetNL();
        }
        [HttpPost]
        [Route("PostNL")]
        public string PostNL(List<NguyenLieuEntity> objNguyenLieu)
        {
            if (objNguyenLieu == null) return "false";
            string json = JsonConvert.SerializeObject(objNguyenLieu);
            DataTable tbNguyenLieu = JsonConvert.DeserializeObject<DataTable>(json);
            return _repoNL.PostNL(tbNguyenLieu);
        }

        [HttpDelete]
        [Route("DeleteNL")]
        public string DeleteNL(int Parameter)
        {
            return _repoNL.DeleteNL(Parameter);
        }

      
        [HttpPost]
        [Route("PostNNPL")]
        public string PostNNPL(List<NhomNguyenPhuLieuEntity> objPhuLieu)
        {
            if (objPhuLieu == null) return "false";
            string json = JsonConvert.SerializeObject(objPhuLieu);
            DataTable tbNNPL = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostNNPL(tbNNPL);
        }

        [HttpDelete]
        [Route("DeleteNNPL")]
        public string DeleteNNPL(int ID)
        {
            return _repo.DeleteNNPL(ID);
        }

        [HttpGet]
        [Route("GetPL")]
        public DataTable GetPL()
        {
            return _repo.GetPL();
        }
        [HttpPost]
        [Route("PostPL")]
        public string PostPL(List<PhuLieuEntity> objPhuLieu)
        {
            if (objPhuLieu == null) return "false";
            string json = JsonConvert.SerializeObject(objPhuLieu);
            DataTable tbPhuLieu = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostPL(tbPhuLieu);
        }

        [HttpDelete]
        [Route("DeletePL")]
        public string DeletePL(int ID)
        {
            return _repo.DeletePL(ID);
        }


    }
}