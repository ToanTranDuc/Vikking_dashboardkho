using Newtonsoft.Json;
using NtbSoft.ERP.Model.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.Kho
{
    public interface IVatTuRepository
    {
        DataTable Get(); // Lấy danh sách vật tư
        DataTable GetKhoVatTu(string action, string para, string para2);
        DataTable GetNL(); // Lấy danh sách vật tư
        DataTable GetPL();
        string PostVT(DataTable tblVatTu); // Thêm hoặc cập nhật vật tư
        string SaveKhoVT(string action, string para, object lstSave);
        string DeleteVT(int ID); // Xóa vật tư theo ID
        string DeleteKhoVT(string action, string ID); // Xóa vật tư theo ID
    }
    public class VatTuRepository : IVatTuRepository
    {
        // Khởi tạo đối tượng model để thao tác với CSDL
        VatTuModel _model = new VatTuModel();

        public DataTable Get()
        {
            return _model.Get();
        }
        public DataTable GetKhoVatTu(string action, string para, string para2)
        {
            return _model.GetKhoVatTu(action, para, para2);
        }
        public DataTable GetNL()
        {
            return _model.GetNL();
        }
        public DataTable GetPL()
        {
            return _model.GetPL();
        }

        // Thêm hoặc cập nhật vật tư
        public string PostVT(DataTable tblVatTu)
        {
            return _model.Post(tblVatTu);
        }

        public string SaveKhoVT(string action, string para, object lstSave)
        {
            string json = JsonConvert.SerializeObject(lstSave);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return _model.SaveKhoVT(action, para, tblSave);
        }
        // Xóa vật tư theo ID
        public string DeleteVT(int ID)
        {
            return _model.Delete(ID);
        }
        public string DeleteKhoVT(string action, string ID)
        {
            return _model.DeleteKhoVT(action, ID);
        }
    }
}