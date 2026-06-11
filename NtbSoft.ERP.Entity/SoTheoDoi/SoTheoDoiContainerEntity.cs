using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SoTheoDoi
{
    public class SoTheoDoiContainerEntity
    {
        public SoTheoDoiContainerEntity(){}
        public SoTheoDoiContainerEntity(DateTime _dataTime) {
            this.NgayThangNam = _dataTime;
            this.NgayCap = _dataTime;
        }
        public int ID { get; set; }
        public string MaSealCont { get; set; }
        public DateTime NgayThangNam { get; set; }
        public string SoSeal { get; set; }
        public DateTime NgayCap { get; set; }
        public string TinhTrangSeal { get; set; }
        public string SoContBamSeal { get; set; }
        public string NguoiBamSeal { get; set; }
    }
}
