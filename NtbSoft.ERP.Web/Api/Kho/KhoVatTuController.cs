using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NtbSoft.ERP.Web.Repository.R.Kho;
using System.Data;
using NtbSoft.ERP.Entity.Kho;
using Newtonsoft.Json;

namespace NtbSoft.ERP.Web.Api.Kho
{
    public class KhoVatTuController: ApiController
    {
        [RoutePrefix("api/KhoVatTu")]
        public class VatTuController : ApiController
        {
            IVatTuRepository _repo = new VatTuRepository();

            // GET: api/VatTu/Get
            [HttpGet]
            [Route("Get")]
            public DataTable Get()
            {
                return _repo.Get();
            }
            [HttpGet]
            [Route("GetNL")]
            public DataTable GetNL()
            {
                return _repo.GetNL();
            }

            [HttpGet]
            [Route("GetPL")]
            public DataTable GetPL()
            {
                return _repo.GetPL();
            }

            // POST: api/VatTu/Post
            [HttpPost]
            [Route("Post")]
            public string Post(List<VatTuEntity> objVatTu)
            {
                if (objVatTu == null) return "false";

                // Chuyển đổi danh sách đối tượng VatTuEntity sang DataTable
                string json = JsonConvert.SerializeObject(objVatTu);
                DataTable tbVatTu = JsonConvert.DeserializeObject<DataTable>(json);

                return _repo.PostVT(tbVatTu);
            }

            // DELETE: api/VatTu/Delete
            [HttpDelete]
            [Route("Delete")]
            public string Delete(int ID)
            {
                return _repo.DeleteVT(ID);
            }

            //Kho Vat Tu Xuat Nhap Ton
            [HttpGet]
            [Route("GetKhoVatTu")]
            public DataTable GetKhoVatTu(string action, string para, string para2)
            {
                return _repo.GetKhoVatTu(action, para, para2);
            }

            [HttpPost]
            [Route("SaveKhoVT")]
            public string SaveKhoVT(string action, string para, object lstSave)
            {
                return _repo.SaveKhoVT(action, para, lstSave);
            }
            [HttpDelete]
            [Route("DeleteVT")]
            public string DeleteVT(string action, string ID)
            {
                return _repo.DeleteKhoVT(action, ID);
            }
        }
    }
}