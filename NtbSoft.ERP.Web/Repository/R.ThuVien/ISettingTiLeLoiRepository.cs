using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtbSoft.ERP.Model.ThuVien;
using System.Data;
namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface ISettingTiLeLoiRepository
    {
        DataTable GetLineX();
        DataTable GetLenh(string lineX);
        DataTable GetSettingTiLeLoi(string line, string lenh, string maHang);
        DataTable SaveSettingTiLeLoi(string line, string lenh, string maHang,
                                      string tiLeLoi, string ngayCaiDat, string id);
        DataTable DeleteSettingTiLeLoi(string id, string jsonIds = "");
    }
    public class SettingTiLeLoiRepository : ISettingTiLeLoiRepository
    {
        private readonly SettingTiLeLoiModel _model = new SettingTiLeLoiModel();
        public DataTable GetLineX()
            => _model.GetLineX();
        public DataTable GetLenh(string lineX)
            => _model.GetLenh(lineX);
        public DataTable GetSettingTiLeLoi(string line, string lenh, string maHang)
            => _model.GetSettingTiLeLoi(line, lenh, maHang);
        public DataTable SaveSettingTiLeLoi(string line, string lenh, string maHang,
                                             string tiLeLoi, string ngayCaiDat, string id)
            => _model.SaveSettingTiLeLoi(line, lenh, maHang, tiLeLoi, ngayCaiDat, id);
        public DataTable DeleteSettingTiLeLoi(string id, string jsonIds = "")
            => _model.DeleteSettingTiLeLoi(id, jsonIds);
    }
}
