using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IMaSealRepository
    {
        DataTable Get(string username);
        DataTable GetKho(string username);
        DataTable Get(string action, string MaSeal);
        string Post(string Action, object ojMaSeal);
        string Delete(string parameter);
        DataTable checkDuplicateSeal(object objMaSeal);
    }
    public class MaSealRepository : IMaSealRepository
    {
        MaSealModel MaSealModel = new MaSealModel();
        public string Delete(string parameter)
        {
            var result = MaSealModel.Delete(parameter);
            return result;
        }

        public DataTable Get(string username)
        {
            var dt = MaSealModel.Get(username);
            return dt;
        }

        public DataTable Get(string action,string MaSeal)
        {
            var dt = MaSealModel.Get( action,MaSeal);
            return dt;
        }

        public DataTable checkDuplicateSeal(object objMaSeal)
        {
            var dt = MaSealModel.Get(objMaSeal);
            return dt;
        }

        public DataTable GetKho(string username)
        {
            var dt = MaSealModel.GetKho(username);
            return dt;
        }

        public string Post(string Action,object ojMaSeal)
        {
            var result = MaSealModel.Post(Action,ojMaSeal);
            return result;
        }
    }
}