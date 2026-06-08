using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NtbSoft.ERP.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
           name: "BangDieuKhien",
           url: "bangdieukhien",
           defaults: new { controller = "Home", action = "Dashboard" }
            );
            //
            routes.MapRoute(
            name: "BaoCaoTQ",
            url: "bao-cao-tong-hop/bao-cao-tong-quan-san-xuat-tgi",
            defaults: new { controller = "BaoCaoTongHop", action = "AllBaoCao" }
          );
            //
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               name: "ReportList",
               url: "danh-sach-bao-cao",
               defaults: new { controller = "Home", action = "ReportList" }
           );
            // báo cáo chất lượng / báo cáo sau ủi / báo cáo năng suất - doanh thu 
            routes.MapRoute(
              name: "BaoCaoChatLuong",
              url: "bao-cao-tong-hop/bao-cao-chat-luong-ma-hang-tren-chuyen",
              defaults: new { controller = "BaoCaoTongHop", action = "ChatLuongMaHangTrenTungChuyen" }
            );
            routes.MapRoute(
              name: "BaoCaoChatLuongSauUi",
              url: "bao-cao-tong-hop/bao-cao-chat-luong-ma-hang-sau-ui",
              defaults: new { controller = "BaoCaoTongHop", action = "ChatLuongMaHangSauUi" }
            );
            routes.MapRoute(
              name: "BaoCaoNangSuatDoanhThu",
              url: "bao-cao-tong-hop/bao-cao-nang-suat-doanh-thu",
              defaults: new { controller = "BaoCaoTongHop", action = "NangSuatDoanhThu" }
            );
            // báo cáo cắt ngày / báo cáo lỗi
            routes.MapRoute(
              name: "BaoCaoKeHoachCatNgay",
              url: "bao-cao-tong-hop/bao-cao-ke-hoach-cat-ngay",
              defaults: new { controller = "BaoCaoTongHop", action = "KeHoachCatNgay" }
            );
            routes.MapRoute(
              name: "BaoCaoMaHangLoiTrenChuyenMay",
              url: "bao-cao-tong-hop/bao-cao-ma-hang-loi-tren-chuyen-may",
              defaults: new { controller = "BaoCaoTongHop", action = "MHLoiTrenTungChuyenMay" }
            );
            //báo cáo tiến độ / báo cáo nhập kho thành phẩm
            routes.MapRoute(
              name: "BaoCaoTienDoCelShap",
              url: "bao-cao-tong-hop/bao-cao-tien-do/{username}/{dvsx}",
              defaults: new { controller = "BaoCaoTongHop", action = "BaoCaoTienDo", username = UrlParameter.Optional, dvsx = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "BaoCaoTienDoDefault",
              url: "bao-cao-tong-hop/bao-cao-tien-do",
              defaults: new { controller = "BaoCaoTongHop", action = "BaoCaoTienDo", username = "web", dvsx = "web" }
            );

            routes.MapRoute(
              name: "BaoCaoNhapKhoTP",
              url: "bao-cao-tong-hop/bao-cao-nhap-kho-tp",
              defaults: new { controller = "BaoCaoTongHop", action = "BaoCaoNhapKho" }
            );
            // báo cáo ao phí / báo cáo cắt ngày / báo cáo nhân sự
            routes.MapRoute(
             name: "BaoCaoHaoPhi",
             url: "bao-cao-tong-hop/bao-cao-thoi-gian-hao-phi",
             defaults: new { controller = "BaoCaoTongHop", action = "BCThoiGianHaoPhi" }
           );
            routes.MapRoute(
             name: "BaoCaoNhanLuc",
             url: "bao-cao-tong-hop/bao-cao-nhan-su",
             defaults: new { controller = "BaoCaoTongHop", action = "BCNguonNhanLuc" }
           );
            //
            routes.MapRoute(
             name: "BaoCaoDongThungCelShap",
             url: "bao-cao-tong-hop/bao-cao-dong-thung/{username}/{dvsx}",
             defaults: new { controller = "BaoCaoTongHop", action = "BaoCaoDongThung", username = UrlParameter.Optional, dvsx = UrlParameter.Optional }
           );
            routes.MapRoute(
             name: "BaoCaoDongThungDefault",
             url: "bao-cao-tong-hop/bao-cao-dong-thung",
             defaults: new { controller = "BaoCaoTongHop", action = "BaoCaoDongThung", username = "web", dvsx = "web" }
           );
            //

            //
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
