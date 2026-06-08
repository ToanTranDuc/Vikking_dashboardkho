using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Libs
{
    /// <summary>
    /// cài đặt nghệ quy chiếu trước khi sử dụng 
    /// </summary>
    public static class LengthUnitConverter
    {
        public static List<LengthUnit> lstLengthUnit = new List<LengthUnit>();
        public static bool connectSQL = false;
        /// <summary>
        /// Khi hàm được gọi sẽ set hệ quy chiếu
        /// </summary>
        static LengthUnitConverter()
        {
            ResetHeQuyChieu();
        }
        /// <summary>
        /// Đặt lại nguồn dữ liệu cho hệ quy chiếu
        /// </summary>
        /// <param name="status">
        ///     true : nếu sử dụng hệ quy chiếu sql
        ///     false: nếu sử dụng hệ quy chiếu mặc định
        /// </param>
        /// <param name="new_lstLengthUnit">
        ///     null: chưa set hệ quy chiếu ngay.
        ///     List<LengthUnit>: Tạo 1 danh sách đơn vị lấy từ sql rồi truyền vào truyền vào
        /// </param>
        public static void SetStatusConnectSQL(bool status, List<LengthUnit> new_lstLengthUnit)
        {
            connectSQL = status;
            if (status)
                lstLengthUnit = new_lstLengthUnit == null ? ListLengthUnitOrigin() : new_lstLengthUnit;
            else
                lstLengthUnit = ListLengthUnitOrigin();
        }
        /// <summary>
        /// Kiểm tra lại trạng thái kết nối sql mỗi lần dùng, nếu param <connectSQL> là true thì sử dụng hệ quy chiếu được cài trước đó.
        /// Ngược lại thì lấy từ hệ quy chiếu mặc định
        /// </summary>
        public static void ResetHeQuyChieu()
        {
            if (!connectSQL)
                lstLengthUnit = ListLengthUnitOrigin();
        }
        public static List<LengthUnit> ListLengthUnitOrigin()
        {
            List<LengthUnit> listLengthUnitOrigin = new List<LengthUnit>();
            listLengthUnitOrigin.Add(new LengthUnit("cm", "Centimeter", "cm", 0.01));
            listLengthUnitOrigin.Add(new LengthUnit("yard", "Yard", "Yd", 0.9144));
            listLengthUnitOrigin.Add(new LengthUnit("meter", "Meter", "m", 0.1));
            listLengthUnitOrigin.Add(new LengthUnit("inch", "Inch", "\"", 0.0254));
            return listLengthUnitOrigin;
        }

        /// <summary>
        /// Chuyển đổi đơn vị
        /// </summary>
        /// <param name="oldUnit">Đơn vị cũ</param>
        /// <param name="newUnit">Đơn vị mới</param>
        /// <param name="lenghtVal">Giá trị cần chuyển đổi (tương ứng với đơn vị cũ)</param>
        /// <returns>
        ///      Kết quả (Tương ứng với đơn vị mới);
        /// </returns>
        public static double ConvertToUnit(string oldUnit, string newUnit, double lenghtVal)
        {
            double oldUnit_Centimet = -1;
            double newUnit_Centimet = -1;
            bool isTrue = false;
            foreach (LengthUnit unit in lstLengthUnit)
            {
                if (unit.maDV == oldUnit)
                    oldUnit_Centimet = unit.heQuyChieu_m;
                if (unit.maDV == newUnit)
                    newUnit_Centimet = unit.heQuyChieu_m;
                if (oldUnit_Centimet > -1 && newUnit_Centimet > -1)
                {
                    isTrue = true;
                    break;
                }
            }
            if (isTrue)
                return -1;
            return (double)(oldUnit_Centimet / newUnit_Centimet);
        }
    }
}
public class LengthUnit
{
    public string maDV { get; set; }
    public string tenDV { get; set; }
    public string tenRutGon { get; set; }
    public string kiHieu { get; set; }
    public double heQuyChieu_m { get; set; }
    public LengthUnit()
    {
        this.maDV = string.Empty;
        this.tenDV = string.Empty;
        this.kiHieu = string.Empty;
        this.heQuyChieu_m = 0;
    }
    public LengthUnit(string maDV, string tenDV, string kiHieu, double heQuyChieu_m)
    {
        this.maDV = maDV;
        this.tenDV = tenDV;
        this.kiHieu = kiHieu;
        this.heQuyChieu_m = heQuyChieu_m;
    }
}

