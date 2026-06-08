using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SoTheoDoi
{
    public class SoTheoDoiSealNoiBoEntity
    {
        public SoTheoDoiSealNoiBoEntity(DateTime _dataTime)
        {
            this.Ngay = _dataTime;
        }
        public int ID { get; set; }
        public DateTime Ngay { get; set; }
        public int SoLuong { get; set; }
        public string NoiDung { get; set; }
        public string TinhTrang { get; set; }
        public string SoXe { get; set; }
        public string SoSeal { get; set; }
        public string TaiXe { get; set; }
        public string NguoiBamSeal { get; set; }
        public string NoiDi { get; set; }
        public string NoiDen { get; set; }
    }
}
