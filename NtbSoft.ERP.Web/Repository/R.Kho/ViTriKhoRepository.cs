using NtbSoft.ERP.Model.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.Kho
{
    public interface IViTriKhoRepository
    {
        DataTable GetDatatable(string action, string parameter);
        DataTable GetPN_VTK(string action, string parameter);
        string Post(string action, string para, DataTable tblViTriKho);
        string PostPN_VTK(string action, string para, DataTable tblViTriKho);
        string DeleteViTriKho(string maViTri);
    }
    public class ViTriKhoRepository : IViTriKhoRepository
    {
        ViTriKhoModel model = new ViTriKhoModel();
        public DataTable GetDatatable(string action, string parameter)
        {
            var dt = model.Get(action, parameter);
            return dt;
        }
        public string Post(string action, string para, DataTable tblViTriKho)
        {
            var result = model.Post(action, para, tblViTriKho);
            return result;
        }
        public DataTable GetPN_VTK(string action, string parameter)
        {
            var dt = model.GetPN_VTK(action, parameter);
            return dt;
        }
        public string PostPN_VTK(string action, string para, DataTable tblPN_VTK)
        {
            var result = model.PostPN_VTK(action, para, tblPN_VTK);
            return result;
        }
        public string DeleteViTriKho(string maViTri)
        {
            return model.DeleteViTriKho(maViTri);
        }
    }
}