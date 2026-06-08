using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.Http;
using System.Web.Http.Cors;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPThongSoVatTu")]
    public class ERPThongSoVatTuController : ApiController
    {
        ERPThongSoVatTuModel _model = new ERPThongSoVatTuModel();
        [HttpGet]
        [Route("GetAll")]
        public DataTable GetAll()
        {
            return _model.GetAll();
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string makh, string mahang)
        {
            return _model.Get(makh,mahang);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {
            return _model.Post(tbl);
        }
        [HttpDelete]
        [Route("DeleteAll")]
        public DataTable DeleteAll( string mahang)
        {
            return _model.DeleteAll(mahang);
        }
        [HttpDelete]
        [Route("Delete")]
        public DataTable Delete(string id)
        {
            return _model.Delete(id);
        }
        [HttpGet]
        [Route("GetVatTuGoiY")]
        public DataTable GetVatTuGoiY()
        {
            return _model.GetVatTuGoiY();
        }
        [HttpGet]
        [Route("GetMauVatTuGoiY")]
        public DataTable GetMauVatTuGoiY()
        {
            return _model.GetMauVatTuGoiY();
        }
        [HttpGet]
        [Route("GetKhoVaiGoiY")]
        public DataTable GetKhoVaiGoiY()
        {
            return _model.GetKhoVaiGoiY();
        }
        [HttpGet]
        [Route("GetNhomGoiY")]
        public DataTable GetNhomGoiY()
        {
            return _model.GetNhomGoiY();
        }
        [HttpGet]
        [Route("GetKH")]
        public DataTable GetKH()
        {
            return _model.GetKH();
        }
        [HttpGet]
        [Route("GetMH")]
        public DataTable GetMH(string makh)
        {
            return _model.GetMH(makh);
        }
        #region màu vật tư
        [HttpGet]
        [Route("GetMauVatTu")]
        public DataTable GetMauVatTu(string makh, string mahang)
        {
            return _model.GetMauVatTu(makh, mahang);
        }
        [HttpPost]
        [Route("PostMauVatTu")]
        public string PostMauVatTu(DataTable tbl)
        {
            return _model.PostMauVatTu(tbl);
        }
        [HttpDelete]
        [Route("DeleteAllMauVatTu")]
        public DataTable DeleteAllMauVatTu(string makh,string mahang)
        {
            return _model.DeleteAllMauVatTu(makh,mahang);
        }
        [HttpDelete]
        [Route("DeleteMauVatTu")]
        public DataTable DeleteMauVatTu(string id)
        {
            return _model.DeleteMauVatTu(id);
        }
        #endregion
        #region Khổ vải
        [HttpGet]
        [Route("GetKhoVai")]
        public DataTable GetKhoVai(string makh, string mahang)
        {
            return _model.GetKhoVai(makh, mahang);
        }
        [HttpPost]
        [Route("PostKhoVai")]
        public string PostKhoVai(DataTable tbl)
        {
            return _model.PostKhoVai(tbl);
        }
        [HttpDelete]
        [Route("DeleteAllKhoVai")]
        public DataTable DeleteAllKhoVai(string makh,string mahang)
        {
            return _model.DeleteAllKhoVai(makh,mahang);
        }
        [HttpDelete]
        [Route("DeleteKhoVai")]
        public DataTable DeleteKhoVai(string id)
        {
            return _model.DeleteKhoVai(id);
        }
        #endregion
        #region Vật tư
        [HttpGet]
        [Route("GetVatTu")]
        public DataTable GetVatTu()
        {
            return _model.GetVatTu();
        }
        [HttpPost]
        [Route("PostVatTu")]
        public string PostVatTu(DataTable tbl)
        {
            return _model.PostVatTu(tbl);
        }
        
        [HttpDelete]
        [Route("DeleteVatTu")]
        public DataTable DeleteVatTu(string id)
        {
            return _model.DeleteVatTu(id);
        }
        #endregion
        [HttpGet]
        [Route("GetDV")]
        public DataTable GetDV()
        {
            return _model.GetDV();
        }
    }
}