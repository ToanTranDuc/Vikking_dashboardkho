using NtbSoft.ERP.Api.Models.SYSTEM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Api.Repository.Interface.SYSTEM
{
    public interface ISystemUserRepository
    {
        List<SystemUserViewModel> Get();
        DataTable Get(string action);
        string Post(SystemUserViewModel item);
        string Delete(string id);
        DataTable GetKhoUser();
        string UpdateMaKho(SystemUserViewModel list);
    }
}
