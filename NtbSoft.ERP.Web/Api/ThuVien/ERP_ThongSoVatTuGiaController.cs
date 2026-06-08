using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/ERPVatTuBOM")]
    public class ERPThongSoVatTuGiaController : ApiController
    {
        private VatTuGiaModel model = new VatTuGiaModel();
     
        [HttpGet]
        [Route("GetTienTe")]
        public DataTable GetTienTe()
        {
            try
            {
                return model.GetTienTe();
            }
            catch
            {
                return new DataTable();
            }
        }

        [HttpGet]
        [Route("GetGia")]
        public DataTable GetGia(string maNhom = null)
        {
            try
            {
                return model.Get(maNhom);
            }
            catch
            {
                return new DataTable();
            }
        }





        [HttpGet]
        [Route("GetGiaByTenNhom")]
        public DataTable GetGiaByTenNhom(string tenNhom)
        {
            try
            {
                return model.GetByTenNhom(tenNhom);
            }
            catch
            {
                return new DataTable();
            }
        }

 
        [HttpPost]
        [Route("PostVatTuGia")]
        public string PostVatTuGia([FromBody] List<VatTuGiaDto> data)
        {
            try
            {
                return model.Save(data);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("GetGiaHistory")]
        public DataTable GetGiaHistory(string maVTID, string extra)
        {
            try
            {
                return model.GetHistory(maVTID, extra);
            }
            catch
            {
                return new DataTable();
            }
        }
        [HttpGet]
        [Route("GetVatTuByMaVT")]
        public DataTable GetVatTuByMaVT(string keyword)
        {
            try
            {
                return model.GetVatTuByMaVT(keyword);
            }
            catch
            {
                return new DataTable();
            }
        }
        [HttpPost]
        [Route("DeleteVatTuGiaHistory")]
        public string DeleteVatTuGiaHistory([FromBody] List<VatTuGiaDto> data)
        {
            try
            {
                return model.DeleteHistory(data);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}