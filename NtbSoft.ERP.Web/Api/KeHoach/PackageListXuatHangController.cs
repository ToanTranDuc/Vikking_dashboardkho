using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
using NtbSoft.ERP.Model.KeHoach;
using NtbSoft.ERP.Model.ThuVien;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/XuatHang")]
    public class PackageListXuatHangController : ApiController
    {
        #region Liên kết android

        /// <summary>
        /// Lấy thông tin phiếu
        /// </summary>
        /// <param name="action">GetCTPhieuScan</param>
        /// <param name="Para1">MaPhieu</param>
        /// <param name="Para2">Cont = All</param>
        /// <param name="Para3"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2, string Para3 = "")
        {
            return new PackageListXuatHangModel().Get(action, Para1, Para2, Para3);
        }

        /// <summary>
        /// Lưu dữ liệu quét được
        /// </summary>
        /// <param name="action">POST</param>
        /// <param name="dtSave"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateScanV2")]
        public string UpdateScanV2(string action, DataTable dtSave)
        {
            return new ScanBarcodeModel().UpdateScan(action, dtSave);
        }
        /// <summary>
        ///  Lưu barcode vào bảng cài đặt barcode
        /// </summary>
        /// <param name="action">POST</param>
        /// <param name="Para1"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string SaveCaiDatBarcode(string action, string Para1, DataTable dt)
        {
            return new CaiDatBarCodeModel().Post(action, Para1, dt);
        }
        #endregion

        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataSet ds)
        {
            return new PackageListXuatHangModel().Post(action, ds.Tables[0], ds.Tables[1], ds.Tables[2]);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, string Para, DataSet ds)
        {
            return new PackageListXuatHangModel().Delete(action, Para, ds.Tables[0]);
        }
        //-----------Scan QR ---------
        [HttpGet]
        [Route("GetDS")]
        public dynamic GetDS(string action, string Para1, string Para2, string Para3 = "")
        {
            DataSet ds = new PackageListXuatHangModel().GetDS(action, Para1, Para2, Para3);
            return new { data1 = ds.Tables[0], data2 = ds.Tables[1] };
        }

        [HttpPost]
        [Route("UpdateScan")]
        public string UpdateScan(string action, dynamic data)
        {
            // return "false";
            var dtSave = CreateTblSave();
            var drNew = dtSave.NewRow();
            drNew["ID"] = 0;
            drNew["MaDH"] = data.MaDH;
            drNew["MaPKL"] = data.MaPKL;
            drNew["QRCode"] = data.Barcode;
            drNew["MaPKL"] = data.MaPKL;
            drNew["DeliveryTo"] = data.MaPKL_XH;
            dtSave.Rows.Add(drNew);

            return new PackageListXuatHangModel().UpdateScan(action, dtSave);
        }


        [HttpPost]
        [Route("UpdateBarcodeLink")]
        public string UpdateBarcodeLink(string action, dynamic data)
        {
            // return "false";
            var dtSave = CreateTblSave();
            var drNew = dtSave.NewRow();
            drNew["ID"] = 0;
            drNew["MaHang"] = data.MaHang;
            drNew["DotSX"] = data.Season;
            drNew["POID"] = data.POID;
            drNew["PO"] = data.PO;
            drNew["DauSizeID"] = data.DauSizeID;
            drNew["DauSize"] = data.DauSize;
            drNew["ColorID"] = data.ColorID;
            drNew["TenMau"] = data.TenMau;
            drNew["SizeID"] = data.SizeID;
            drNew["Size"] = data.Size;
            drNew["Cont"] = data.Cont;
            //drNew["MaDH"] = data.MaDH;           
            drNew["QRCode"] = data.Barcode;
            //drNew["MaPKL"] = data.MaPKL;
            drNew["DeliveryTo"] = data.MaPKL_XH;
            drNew["IsScan"] = data.IsCreate;
            drNew["IsThungLe"] = data.IsThungLe;
            drNew["NVien"] = data.NVien;
            drNew["KieuLap"] = 0;
            drNew["KieuLapPCB"] = data.SLGop;
            dtSave.Rows.Add(drNew);

            return new PackageListXuatHangModel().UpdateScan(action, dtSave);
        }

        [HttpPost]
        [Route("UpdateScanWin")]
        public string UpdateScanWin(string action, DataTable dtSave)
        {
            return new PackageListXuatHangModel().UpdateScan(action, dtSave);
        }
        [HttpPost]
        [Route("UpdateBarcodeLinkWin")]
        public string UpdateDelete(string action, DataTable dtSave)
        {
            return new PackageListXuatHangModel().UpdateScan(action, dtSave);
        }
        [HttpPost]
        [Route("PostAlarm")]
        public string PostAlarm(string action, DataTable dtSave)
        {
            return new PackageListXuatHangModel().PostAlarm(action, dtSave);
        }
        private DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPKL", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaLenh", typeof(string));
            tbl.Columns.Add("DotSX", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("Store", typeof(string));
            tbl.Columns.Add("ColorID", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("NgayLapKH", typeof(DateTime));
            tbl.Columns.Add("ChieuDai", typeof(double));
            tbl.Columns.Add("ChieuRong", typeof(double));
            tbl.Columns.Add("ChieuCao", typeof(double));
            tbl.Columns.Add("TrongLuong", typeof(double));
            tbl.Columns.Add("KhoiLuong", typeof(double));
            tbl.Columns.Add("SoLuongThung", typeof(int));
            tbl.Columns.Add("TuThung", typeof(int));
            tbl.Columns.Add("DenThung", typeof(int));
            tbl.Columns.Add("SttThung", typeof(int));
            tbl.Columns.Add("SoLuongSP", typeof(int));
            tbl.Columns.Add("PCB_Pack", typeof(int));
            tbl.Columns.Add("Pack_Ctn", typeof(int));
            tbl.Columns.Add("IsDongThung", typeof(bool));
            tbl.Columns.Add("NgayDongThung", typeof(DateTime));
            tbl.Columns.Add("QRCode", typeof(string));
            tbl.Columns.Add("IsScan", typeof(bool));
            tbl.Columns.Add("IsNhapKho", typeof(bool));
            tbl.Columns.Add("NgayNhapKho", typeof(DateTime));
            tbl.Columns.Add("KyHieu", typeof(string));
            tbl.Columns.Add("Chon", typeof(bool));
            tbl.Columns.Add("SttThung_temp", typeof(int));
            tbl.Columns.Add("SttThung_LapMau", typeof(int));
            tbl.Columns.Add("SttThung_decat", typeof(int));
            tbl.Columns.Add("SttThung_start", typeof(int));
            tbl.Columns.Add("IsThungLe", typeof(int));
            tbl.Columns.Add("IsStoreThieu", typeof(int));
            tbl.Columns.Add("Destination", typeof(string));
            tbl.Columns.Add("DeliveryTo", typeof(string));
            tbl.Columns.Add("Terms", typeof(string));
            tbl.Columns.Add("CountryOfOrigin", typeof(string));
            tbl.Columns.Add("StyleName", typeof(string));
            tbl.Columns.Add("DeptNo", typeof(string));
            tbl.Columns.Add("KieuLapPCB", typeof(int));
            tbl.Columns.Add("KieuLap", typeof(int));
            tbl.Columns.Add("Cont", typeof(string));
            tbl.Columns.Add("MaDH_XH", typeof(string));
            tbl.Columns.Add("POID_XH", typeof(string));
            tbl.Columns.Add("NVien", typeof(string));
            return tbl;
        }

        /////////////////////////////
        #region manh
        [HttpPost]
        [Route("PostDL")]
        public string PostDL(dynamic data)
        {
            List<PackageListXuatHangEntity> lstData = new List<PackageListXuatHangEntity>();
            foreach (var item in data)
            {
                lstData.Add(new PackageListXuatHangEntity()
                {
                    MaPKL_XH = item.MaPKL_XH.ToString(),
                    MaCont = item.VCCont.ToString(),
                    MaHang = item.MaHang.ToString(),
                    POID = item.PO.ToString(),
                    GhiChu = item.GhiChu.ToString(),
                    SoBooking = item.SoBooking.ToString(),
                });
            }

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            return new PackageListXuatHangModel().PostBBXh("PostBBXuatHang", dt, "@TypePackageList");
        }
        [HttpPost]
        [Route("PostImage")]
        public string PostImage(dynamic data)
        {
            if (data is null) return null;
            List<BienBanXuatHang> lstData = new List<BienBanXuatHang>();
            foreach (var items in data)
            {
                string nameImgae = "";
                string checkAnh = items.ListAnh.ToString();
                int currentMinute = DateTime.Now.Minute;
                int currentSecond = DateTime.Now.Second;
                string nameImageColumn = items.NameImage.ToString();
                string[] imageDataParts = items[nameImageColumn].ToString().Split(',');
                string imageName = "";
                string imageName1 = "";
                string tenBienban = items.TenBienBan.ToString().Replace("|", "_");
                string imageDataA = items[nameImageColumn].ToString();
                if (!string.IsNullOrEmpty(imageDataA))
                {
                    byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        string tenhinh = nameImgae + "_" + checkAnh;
                        Image image = Image.FromStream(ms);
                        string uploadPath = HttpContext.Current.Server.MapPath("~/Images/hinhanh_" + items.TenMaHang.ToString() + "_" + tenBienban);
                        string uploadPath1 = HttpContext.Current.Server.MapPath("~/Images/All");
                        if (!System.IO.Directory.Exists(uploadPath))
                            System.IO.Directory.CreateDirectory(uploadPath);
                        if (!System.IO.Directory.Exists(uploadPath1))
                            System.IO.Directory.CreateDirectory(uploadPath1);
                        imageName = tenhinh + ".png";
                        imageName1 = tenhinh + nameImageColumn + currentSecond + currentMinute + ".png";
                        var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);
                        var mPath1 = string.Format(@"{0}\{1}", uploadPath1, imageName1);
                        if (System.IO.File.Exists(mPath))
                            System.IO.File.Delete(mPath);

                        image.Save(mPath);
                        image.Save(mPath1);
                    }
                }
                lstData.Add(new BienBanXuatHang()
                {
                    MaPhieuBB = items.MaBienBan.ToString(),
                    Image0 = 1 == (int)items.SoXe ? imageName1.ToString() : "",
                    Image25 = 2 == (int)items.SoXe ? imageName1.ToString() : "",
                    Image50 = 3 == (int)items.SoXe ? imageName1.ToString() : "",
                    Image75 = 4 == (int)items.SoXe ? imageName1.ToString() : "",
                    Image100 = 5 == (int)items.SoXe ? imageName1.ToString() : "",
                    ImageDoAm = 6 == (int)items.SoXe ? imageName1.ToString() : "",
                    ImageDongCua = 7 == (int)items.SoXe ? imageName1.ToString() : "",
                    ImageTruocChot = 8 == (int)items.SoXe ? imageName1.ToString() : "",
                    ImageChotAT = 9 == (int)items.SoXe ? imageName1.ToString() : "",
                    SoXe = items.SoXe.ToString(),
                });
            }

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            return new PackageListXuatHangModel().PostDL("PostImage", dt, 50);
        }
        [HttpPost]
        [Route("PostImageInFolder")]
        public string PostFolder(dynamic data)
        {
            string nameImgae = "";
            string currentMinuteNgayThang = DateTime.Now.ToString("dd-MM-yyyy");
            int currentMinute = DateTime.Now.Minute;
            int currentSecond = DateTime.Now.Second;
            string checkAnh = data.ListAnh.ToString();
            string[] imageDataParts = data.ImageHinh.ToString().Split(',');
            string imageName = "";
            string tenBienban = data.TenBienBan.ToString().Replace("|", "_");
            string imageDataA = data.ImageHinh.ToString();
            if (!string.IsNullOrEmpty(imageDataA))
            {
                byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    string tenhinh = nameImgae + "_" + checkAnh;
                    Image image = Image.FromStream(ms);
                    string uploadPath = HttpContext.Current.Server.MapPath("~/ImagesHinhNew/HinhNew" + data.TenMaHang.ToString() + "_" + tenBienban);
                    if (!System.IO.Directory.Exists(uploadPath))
                        System.IO.Directory.CreateDirectory(uploadPath);
                    imageName = tenhinh + currentSecond + currentMinute + currentMinuteNgayThang + ".png";
                    var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);

                    image.Save(mPath);
                }
            }
            return null;
        }
        [HttpPost]
        [Route("PostBBXuatHang")]
        public string PostBBXuatHang(dynamic item)
        {
            List<BienBanXuatHang> lstData = new List<BienBanXuatHang>();
            foreach (var data in item)
            {
                string ngaXeDen = data.NgayXeDen;
                DateTime ngaXeDenDT = DateTime.Parse(ngaXeDen);
                string NgayGioCheckXeRoMoc = data.NgayGioCheckXeRoMoc;
                DateTime NgayGioCheckXeRoMocDT = DateTime.Parse(NgayGioCheckXeRoMoc);
                string NgayGioDongHang = data.NgayGioDongHang;
                DateTime NgayGioDongHangDT = DateTime.Parse(NgayGioDongHang);
                string NgayGioKTDongHang = data.NgayGioKTDongHang;
                DateTime NgayGioKTDongHangDT = DateTime.Parse(NgayGioKTDongHang);
                string NgayGioXeDi = data.NgayGioXeDi;
                DateTime NgayGioXeDiDT = DateTime.Parse(NgayGioXeDi);

                string TGBVCheckBD = data.TGBVCheckBD;
                DateTime TGBVCheckBDDT = DateTime.Parse(TGBVCheckBD);
                string TGBVCheckKT = data.TGBVCheckKT;
                DateTime TGBVCheckKTDT = DateTime.Parse(TGBVCheckKT);

                string TGNVCheckBD = data.TGNVCheckBD;
                DateTime TGNVCheckBDDT = DateTime.Parse(TGNVCheckBD);
                string TGNVCheckKT = data.TGNVCheckKT;
                DateTime TGNVCheckKTDT = DateTime.Parse(TGNVCheckKT);

                lstData.Add(new BienBanXuatHang
                {
                    MaPhieuBB = data.MaPhieuBB.ToString(),
                    NgayLap = DateTime.Parse(data.NgayLap.ToString()),
                    VanChuyen = (int)data.VanChuyen,
                    LoaiFeets = data.LoaiFeets.ToString(),
                    LoaiTan = data.LoaiTan.ToString(),
                    SoXe = data.SoXe.ToString() ?? "",
                    SoCont = data.SoCont.ToString(),
                    SoRoMoc = data.SoRoMoc.ToString() ?? "",
                    TenTaiXe = data.TenTaiXe.ToString(),
                    CMND = data.CMND.ToString(),
                    CongTy = data.CongTy.ToString(),
                    NgayXeDen = ngaXeDenDT,
                    NgayGioCheckXeRoMoc = NgayGioCheckXeRoMocDT,
                    NgayGioDongHang = NgayGioDongHangDT,
                    NgayGioKTDongHang = NgayGioKTDongHangDT,
                    MaSeal = data.MaSeal.ToString(),
                    NgayGioXeDi = NgayGioXeDiDT,
                    XeRomoc = (int)data.XeRomoc,
                    Ma_PKL = data.MaPKL_XH.ToString(),
                    MaCongTy = data.MaCongTy.ToString(),
                    NhanVienKiem = data.NhanVienKiem.ToString(),
                    TenBienBan = data.TenBienBan.ToString() ?? "",
                    MaSoCont = data.SoContCu.ToString(),
                    ChungTu = data.ChungTu.ToString(),
                    TGBVCheckBD = TGBVCheckBDDT,
                    TGBVCheckKT = TGBVCheckKTDT,
                    TGNVCheckBD = TGNVCheckBDDT,
                    TGNVCheckKT = TGNVCheckKTDT
                });
            }

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            return new PackageListXuatHangModel().PostBBXh("PostBBXuatHang", dt, "@TypeBienBienHX", item[0].SoContCu.ToString(), Convert.ToInt16(item[0].KetLuan), item[0].CangDen.ToString());
        }

        [HttpPost]
        [Route("PostImageKyTen")]
        public string PostImageKyTen(dynamic items)
        {
            if (items is null) return null;
            string imageDataA = string.Empty;
            imageDataA = items.KyTen;

            int currentMinute = DateTime.Now.Minute;
            int currentSecond = DateTime.Now.Second;
            string[] imageDataParts = imageDataA.Split(',');
            string imageName = "";
            if (!string.IsNullOrEmpty(imageDataA))
            {
                byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    string tenhinh = items.MaBBXuatHang.ToString() + "_" + currentSecond + currentMinute + currentSecond;
                    Image image = Image.FromStream(ms);
                    string uploadPath = HttpContext.Current.Server.MapPath("~/Images/KytenXuatHang");
                    if (!System.IO.Directory.Exists(uploadPath))
                        System.IO.Directory.CreateDirectory(uploadPath);
                    imageName = tenhinh + ".png";
                    var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);

                    image.Save(mPath);
                }
            }

            List<KyTenBienBan> lstData = new List<KyTenBienBan>();
            lstData.Add(new KyTenBienBan()
            {
                MaBBXuatHang = items.MaBBXuatHang.ToString(),
                MaHoTen = items.MaHoTen.ToString(),
                HoTen = items.HoTen.ToString(),
                BoPhan = items.BoPhan.ToString(),
                KyTen = imageName,
            });
            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            return new PackageListXuatHangModel().PostBBXh("PostImageKyTen", dt, "@TypeKyTenXuatHang");
        }
        [HttpPost]
        [Route("PostNDKT")]
        public string PostNDKT(dynamic data)
        {
            List<NoiDungKiemTra> lstData = new List<NoiDungKiemTra>();
            foreach (var item in data)
            {
                lstData.Add(new NoiDungKiemTra
                {
                    MaBBPhieu = (int)item.MaBBPhieu,
                    MaNoiDung = item.MaNoiDung.ToString(),
                    NoiDung = item.NoiDung.ToString(),
                    StatusCont = (int)item.StatusCont,
                    StatusRoMoc = (int)item.StatusRoMoc,
                    StatusXeTai = (int)item.StatusXeTai,
                    StatusKL = (int)item.StatusKL,
                });
            }

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            return new PackageListXuatHangModel().PostBBXh("PostNoDungKT", dt, "@TypeNoiDungKiemTra");
        }
        [HttpPost]
        [Route("DeletePhieu")]
        public string DeletePhieu(dynamic data)
        {
            return new PackageListXuatHangModel().DeletePhieu(data.MaPhieu.ToString(), (int)data.Delete, data.SoCont.ToString());
        }

        [HttpGet]
        [Route("BCExcel")]
        public HttpResponseMessage BCExcel(string action, string Para1, string Para2)
        {
            try
            {
                DataTable dtTable = new PackageListXuatHangModel().Get(action, Para1, Para2);
                DataTable dtImageOther = new TheoDoiDonHangModel().Get("GetImageOther", Para1, Para2, "");
                DataTable dtTableKyTen = new PackageListXuatHangModel().Get("GetKyTenXuatHang", Para1, Para2);
                string imagePathRelative = "/Content/Templates/bienbanhangxuat.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

                dtTable = dtTable.AsEnumerable().Where(x => x["MaPhieuBB"].ToString() == Para1).CopyToDataTable();
                var foder = HttpContext.Current.Server.MapPath("~/Images/KytenXuatHang");
                var savefoder = HttpContext.Current.Server.MapPath("~/Images/All");
                var savefoder1 = HttpContext.Current.Server.MapPath("~/Images/Other");
                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["sample_ Hinh anh t.d hang xuat"];
                    ExcelRange range = worksheet.Cells;
                    if (Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) != 0)
                    {
                        range = worksheet.Cells["D6"]; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1 ? "✔️" : "";
                        if (Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1)
                        {
                            range = worksheet.Cells["i6"]; range.Value = $"{dtTable.Rows[0]["LoaiFeets"]} feets"; worksheet.Column(9).Width = 6;
                        }
                        else
                        {
                            range = worksheet.Cells["o6"]; range.Value = $"{dtTable.Rows[0]["LoaiTan"]} tấn"; worksheet.Column(9).Width = 6;
                        }
                        range = worksheet.Cells["J6"]; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "✔️" : "";
                    }
                    DateTime dateTime = Convert.ToDateTime(dtTable.Rows[0]["NgayGioDong"]);
                    string formattedDateNgayGioDong = dateTime.ToString("dd/MM/yyyy HH:mm");
                    DateTime dateGioKt = Convert.ToDateTime(dtTable.Rows[0]["NgayGioKT"]);
                    string formattedDateNgayKT = dateGioKt.ToString("dd/MM/yyyy HH:mm");
                    string kienAndSp = dtTable.Rows[0]["SLThung"] + " kiện = " + dtTable.Rows[0]["SoLuong"] + " sp";
                    range = worksheet.Cells["F8:J8"]; range.Merge = true; range.Value = dtTable.Rows[0]["KhachHang"].ToString();
                    range = worksheet.Cells["o8:s8"]; range.Merge = true; range.Value = dtTable.Rows[0]["MaHang"].ToString();
                    range = worksheet.Cells["v8:v8"]; range.Value = dtTable.Rows[0]["PO"].ToString();
                    range = worksheet.Cells["f9:j9"]; range.Merge = true; range.Value = kienAndSp.ToString();
                    range = worksheet.Cells["o9:s9"]; range.Merge = true; range.Value = formattedDateNgayGioDong;
                    range = worksheet.Cells["x9"]; range.Value = formattedDateNgayKT;
                    range = worksheet.Cells["f10:k10"]; range.Merge = true; range.Value = dtTable.Rows[0]["Soxe"].ToString();
                    range = worksheet.Cells["o10:s10"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "" : dtTable.Rows[0]["SoCont"].ToString();
                    range = worksheet.Cells["W10:aa10"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "" : dtTable.Rows[0]["SoRoMoc"].ToString();
                    range = worksheet.Cells["f11:k11"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenTaiXe"].ToString();
                    range = worksheet.Cells["o11:s11"]; range.Merge = true; range.Value = dtTable.Rows[0]["CMND"].ToString();
                    range = worksheet.Cells["W11:W11"]; range.Value = dtTable.Rows[0]["CongTy"].ToString();
                    range = worksheet.Cells["M50:P51"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenSeal1"].ToString();

                    range = worksheet.Cells["b14:k22"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image0"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image0"].ToString(), 13, 1, 5, savefoder, 240, 140);

                    range = worksheet.Cells["M14:P22"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageDoAm"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageDoAm"].ToString(), 13, 12, 5, savefoder, 180, 140);

                    range = worksheet.Cells["R14:aa22"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image25"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image25"].ToString(), 13, 17, 5, savefoder, 240, 140);
                    range = worksheet.Cells["b25:k33"]; range.Merge = true;


                    if (dtTable.Rows[0]["Image50"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image50"].ToString(), 24, 1, 5, savefoder, 240, 140);
                    range = worksheet.Cells["R25:aa33"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image75"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image75"].ToString(), 24, 17, 5, savefoder, 240, 140);

                    range = worksheet.Cells["b36:k44"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image100"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image100"].ToString(), 35, 1, 5, savefoder, 240, 140);
                    range = worksheet.Cells["R36:aa44"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageDongCua"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageDongCua"].ToString(), 35, 17, 5, savefoder, 240, 140);

                    range = worksheet.Cells["b47:k55"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageTruocChot"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageTruocChot"].ToString(), 46, 1, 5, savefoder, 240, 140);
                    range = worksheet.Cells["R47:aa55"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageChotAT"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageChotAT"].ToString(), 46, 17, 5, savefoder, 240, 140);

                    int rowImage = 57;
                    int index = 0;
                    int indexAddKiten = 0;
                    foreach (DataRow item in dtImageOther.Rows)
                    {
                        if (index == 2)
                        {
                            rowImage += 2;
                            index = 0;
                            rowImage += 9;
                        }
                        if (index == 0)
                        {
                            worksheet.InsertRow(rowImage, 9);
                            indexAddKiten += 9;
                            range = worksheet.Cells[rowImage - 1, 2]; range.Value = item["NoiDung"].ToString();
                            range = worksheet.Cells[rowImage, 2, rowImage + 8, 10]; range.Merge = true;
                            if (item["Image"].ToString() != "") Addpicutre(worksheet, item["Image"].ToString(), rowImage, 1, 5, savefoder1, 240, 140);
                        }
                        else
                        {
                            range = worksheet.Cells[rowImage - 1, 18]; range.Value = item["NoiDung"].ToString();
                            range = worksheet.Cells[rowImage, 18, rowImage + 8, 26]; range.Merge = true;
                            if (item["Image"].ToString() != "") Addpicutre(worksheet, item["Image"].ToString(), rowImage, 17, 5, savefoder1, 240, 140);
                        }
                        index++;
                    }

                    int rowKyten = rowImage + indexAddKiten + 3;
                    int rowKytenMoi = rowImage + indexAddKiten + 3;
                    int checkIndexKT = 0;
                    for (int i = 0; i < dtTableKyTen.Rows.Count; i++)
                    {

                        if (checkIndexKT == 5)
                        {
                            range = worksheet.Cells[rowKytenMoi, 14, rowKytenMoi, 20]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["HoTen"].ToString();
                            range = worksheet.Cells[rowKytenMoi, 21, rowKytenMoi, 23]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["BoPhan"].ToString();
                            range = worksheet.Cells[rowKytenMoi, 24, rowKytenMoi, 26]; range.Merge = true;
                            if (dtTableKyTen.Rows[i]["KyTen"].ToString() != "") Addpicutre(worksheet, dtTableKyTen.Rows[i]["KyTen"].ToString(), rowKytenMoi - 1, 23, 5, foder, 70, 50);
                            worksheet.Row(rowKytenMoi).Height = 50;
                            rowKytenMoi++;
                        }
                        else
                        {
                            range = worksheet.Cells[rowKyten, 1, rowKyten, 7]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["HoTen"].ToString();
                            range = worksheet.Cells[rowKyten, 8, rowKyten, 10]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["BoPhan"].ToString();
                            range = worksheet.Cells[rowKyten, 11, rowKyten, 13]; range.Merge = true;
                            if (dtTableKyTen.Rows[i]["KyTen"].ToString() != "") Addpicutre(worksheet, dtTableKyTen.Rows[i]["KyTen"].ToString(), rowKyten - 1, 10, 5, foder, 70, 50);
                            worksheet.Row(rowKyten).Height = 50;
                            rowKyten++;
                            checkIndexKT++;
                        }

                    }


                    byte[] fileBytes = package.GetAsByteArray();

                    // Lưu tệp vào đường dẫn đã chỉ định

                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"BCTheoDoiHangXuat.xlsx";
                    package.SaveAs(new FileInfo(savefoder + "/" + fileName));

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet]
        [Route("BCExcelBB")]
        public HttpResponseMessage BCExcelBB(string action, string Para1, string Para2, string para3)
        {
            try
            {
                DataTable dtTable = new PackageListXuatHangModel().Get(action, Para1, Para2);
                DataTable dtTableKyTen = new PackageListXuatHangModel().Get("GetKyTenXuatHang", Para1, Para2);
                DataTable dtTableKiemTra = new PackageListXuatHangModel().Get("GetnoiDungXuatHang", Para1, Para2);
                DataTable dtTableTenhang = new PackageListXuatHangModel().Get("GetTenHang", Para1, para3);
              
                DataTable dtTableTD = new PackageListXuatHangModel().Get("GetMaPhieuBienBan", Para1, Para2);
                DataTable dtImageOtherTD = new TheoDoiDonHangModel().Get("GetImageOther", Para1, Para2, "");
                DataTable dtTableTDKyTen = new PackageListXuatHangModel().Get("GetKyTenXuatHang", Para1, Para2);
               

                string imagePathRelative = "/Content/Templates/bienbanKTCont.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

                var foderA = HttpContext.Current.Server.MapPath("~/Images/KytenXuatHang");
                var savefoder = HttpContext.Current.Server.MapPath("~/Images/All");
                var savefoder1 = HttpContext.Current.Server.MapPath("~/Images/Other");

                var foder = HttpContext.Current.Server.MapPath("~/Images/KytenXuatHang");
                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Inspection - edit"];
                    ExcelRange range = worksheet.Cells;
                    if (Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) != 0)
                    {
                        range = worksheet.Cells["F7"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1 ? "✔️" : "";
                        range = worksheet.Cells["l7"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["XeRomoc"]) == 1 ? "✔️" : "";
                        range = worksheet.Cells["r7"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "✔️" : "";

                        range = worksheet.Cells["q10"]; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1 ? dtTable.Rows[0]["SoCont"].ToString() : "";
                        range = worksheet.Cells["d10"]; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1 ? dtTable.Rows[0]["SoRoMoc"].ToString() : "";
                    }


                    range = worksheet.Cells["f8"]; range.Value = dtTable.Rows[0]["LoaiFeets"].ToString();
                    range = worksheet.Cells["p8"]; range.Value = dtTable.Rows[0]["LoaiTan"].ToString();
                    range = worksheet.Cells["f9"]; range.Value = dtTable.Rows[0]["SoXe"].ToString();
                    range = worksheet.Cells["q9"]; range.Value = dtTable.Rows[0]["Seal"].ToString();


                    range = worksheet.Cells["d11"]; range.Value = dtTable.Rows[0]["TenTaiXe"].ToString();
                    range = worksheet.Cells["l11"]; range.Value = dtTable.Rows[0]["CMND"].ToString();
                    range = worksheet.Cells["t11"]; range.Value = dtTable.Rows[0]["CongTy"].ToString(); range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range = worksheet.Cells["i12"]; range.Value = Convert.ToDateTime(dtTable.Rows[0]["NgayXeDen"]).ToString("dd/MM/yyyy hh:mm");
                    range = worksheet.Cells["y13"]; range.Value = dtTable.Rows[0]["NhanVienKiem"].ToString();
                    if (Convert.ToInt32(dtTableKiemTra.Rows[0]["StatusKL"]) == 1)
                    {
                        range = worksheet.Cells["h13"]; range.Value = Convert.ToDateTime(dtTable.Rows[0]["TGBVCheckBD"]).ToString("dd/MM/yyyy HH:mm");
                        range = worksheet.Cells["p13"]; range.Value = Convert.ToDateTime(dtTable.Rows[0]["TGBVCheckKT"]).ToString("dd/MM/yyyy HH:mm");
                        range = worksheet.Cells["h14"]; range.Value = Convert.ToDateTime(dtTable.Rows[0]["TGNVCheckBD"]).ToString("dd/MM/yyyy HH:mm");
                        range = worksheet.Cells["p14"]; range.Value = Convert.ToDateTime(dtTable.Rows[0]["TGNVCheckKT"]).ToString("dd/MM/yyyy HH:mm");
                        range = worksheet.Cells["t12"]; range.Value = Convert.ToDateTime(dtTable.Rows[0]["NgayGioXeDi"]).ToString("dd/MM/yyyy HH:mm");
                    }
                    int index = 0;
                    string status = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1 ? "StatusCont" : "StatusXeTai";
                    for (int i = 18; i < 38; i++)
                    {
                        string checkColoumn = "";
                        checkColoumn = status == "StatusCont" ? (dtTableKiemTra.Rows[index][status].ToString() == "1" ? "M" : "P") : dtTableKiemTra.Rows[index][status].ToString() == "1" ? "S" : "v";
                        if (i == 35) continue;
                        range = worksheet.Cells[$"{checkColoumn}{i}"]; range.Value = dtTableKiemTra.Rows[index][status].ToString() == "1" ? "✔️" : "X"; range.Style.Font.Bold = true;
                        index++;
                    }

                    range = worksheet.Cells["i38"];
                    range.Value = Convert.ToInt32(dtTableKiemTra.Rows[0]["StatusKL"]) == 1 ? "✔️" : "";
                    range = worksheet.Cells["s38"];
                    range.Value = Convert.ToInt32(dtTableKiemTra.Rows[0]["StatusKL"]) == 2 ? "✔️" : "";
                    string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/') ;
                    worksheet.Cells["m43"].Value = $"Ngày {dateTime[0]} tháng {dateTime[1]} năm {dateTime[2]}";

                    ExcelWorksheet worksheetTD = package.Workbook.Worksheets["sample_ Hinh anh t.d hang xuat"];
                    ExcelRange rangeTD = worksheetTD.Cells;
                    if (Convert.ToInt32(dtTableTD.Rows[0]["VanChuyen"]) != 0)
                    {
                        rangeTD = worksheetTD.Cells["D6"]; rangeTD.Value = Convert.ToInt32(dtTableTD.Rows[0]["VanChuyen"]) == 1 ? "✔️" : "";
                        if (Convert.ToInt32(dtTableTD.Rows[0]["VanChuyen"]) == 1)
                        {
                            rangeTD = worksheetTD.Cells["i6"]; rangeTD.Value = $"{dtTableTD.Rows[0]["LoaiFeets"]} feets"; worksheetTD.Column(9).Width = 6;
                        }
                        else
                        {
                            rangeTD = worksheetTD.Cells["o6"]; rangeTD.Value = $"{dtTableTD.Rows[0]["LoaiTan"]} tấn"; worksheetTD.Column(9).Width = 6;
                        }
                        rangeTD = worksheetTD.Cells["J6"]; rangeTD.Value = Convert.ToInt32(dtTableTD.Rows[0]["VanChuyen"]) == 2 ? "✔️" : "";
                    }
                    DateTime dateTimeTD = Convert.ToDateTime(dtTableTD.Rows[0]["NgayGioDong"]);
                    string formattedDateNgayGioDong = dateTimeTD.ToString("dd/MM/yyyy HH:mm");
                    DateTime dateGioKt = Convert.ToDateTime(dtTableTD.Rows[0]["NgayGioKT"]);
                    string formattedDateNgayKT = dateGioKt.ToString("dd/MM/yyyy HH:mm");
                    string kienAndSp = dtTableTD.Rows[0]["SLThung"] + " kiện = " + dtTableTD.Rows[0]["SoLuong"] + " sp";
                    rangeTD = worksheetTD.Cells["F8:J8"]; rangeTD.Merge = true; rangeTD.Value = dtTableTD.Rows[0]["KhachHang"].ToString();
                    rangeTD = worksheetTD.Cells["o8:s8"]; rangeTD.Merge = true; rangeTD.Value = dtTableTD.Rows[0]["MaHang"].ToString();
                    rangeTD = worksheetTD.Cells["v8:v8"]; rangeTD.Value = dtTableTD.Rows[0]["PO"].ToString();
                    rangeTD = worksheetTD.Cells["f9:j9"]; rangeTD.Merge = true; rangeTD.Value = kienAndSp.ToString();
                    rangeTD = worksheetTD.Cells["o9:s9"]; rangeTD.Merge = true; rangeTD.Value = formattedDateNgayGioDong;
                    rangeTD = worksheetTD.Cells["x9"]; rangeTD.Value = formattedDateNgayKT;
                    rangeTD = worksheetTD.Cells["f10:k10"]; rangeTD.Merge = true; rangeTD.Value = dtTableTD.Rows[0]["Soxe"].ToString();
                    rangeTD = worksheetTD.Cells["o10:s10"]; rangeTD.Merge = true; rangeTD.Value = Convert.ToInt32(dtTableTD.Rows[0]["VanChuyen"]) == 2 ? "" : dtTableTD.Rows[0]["SoCont"].ToString();
                    rangeTD = worksheetTD.Cells["W10:aa10"]; rangeTD.Merge = true; rangeTD.Value = Convert.ToInt32(dtTableTD.Rows[0]["VanChuyen"]) == 2 ? "" : dtTableTD.Rows[0]["SoRoMoc"].ToString();
                    rangeTD = worksheetTD.Cells["f11:k11"]; rangeTD.Merge = true; rangeTD.Value = dtTableTD.Rows[0]["TenTaiXe"].ToString();
                    rangeTD = worksheetTD.Cells["o11:s11"]; rangeTD.Merge = true; rangeTD.Value = dtTableTD.Rows[0]["CMND"].ToString();
                    rangeTD = worksheetTD.Cells["W11:W11"]; rangeTD.Value = dtTableTD.Rows[0]["CongTy"].ToString();
                    rangeTD = worksheetTD.Cells["M50:P51"]; rangeTD.Merge = true; rangeTD.Value = dtTableTD.Rows[0]["TenSeal1"].ToString();

                    rangeTD = worksheetTD.Cells["b14:k22"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["Image0"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["Image0"].ToString(), 13, 1, 5, savefoder, 240, 140);

                    rangeTD = worksheetTD.Cells["M14:P22"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["ImageDoAm"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["ImageDoAm"].ToString(), 13, 12, 5, savefoder, 180, 140);

                    rangeTD = worksheetTD.Cells["R14:aa22"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["Image25"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["Image25"].ToString(), 13, 17, 5, savefoder, 240, 140);
                    rangeTD = worksheetTD.Cells["b25:k33"]; rangeTD.Merge = true;


                    if (dtTableTD.Rows[0]["Image50"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["Image50"].ToString(), 24, 1, 5, savefoder, 240, 140);
                    rangeTD = worksheetTD.Cells["R25:aa33"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["Image75"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["Image75"].ToString(), 24, 17, 5, savefoder, 240, 140);

                    rangeTD = worksheetTD.Cells["b36:k44"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["Image100"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["Image100"].ToString(), 35, 1, 5, savefoder, 240, 140);
                    rangeTD = worksheetTD.Cells["R36:aa44"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["ImageDongCua"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["ImageDongCua"].ToString(), 35, 17, 5, savefoder, 240, 140);

                    rangeTD = worksheetTD.Cells["b47:k55"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["ImageTruocChot"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["ImageTruocChot"].ToString(), 46, 1, 5, savefoder, 240, 140);
                    rangeTD = worksheetTD.Cells["R47:aa55"]; rangeTD.Merge = true;
                    if (dtTableTD.Rows[0]["ImageChotAT"].ToString() != "") Addpicutre(worksheetTD, dtTableTD.Rows[0]["ImageChotAT"].ToString(), 46, 17, 5, savefoder, 240, 140);

                    int rowImage = 57;
                    int indexTD = 0;
                    int indexTDAddKiten = 0;
                    foreach (DataRow item in dtImageOtherTD.Rows)
                    {
                        if (indexTD == 2)
                        {
                            rowImage += 2;
                            indexTD = 0;
                            rowImage += 9;
                        }
                        if (indexTD == 0)
                        {
                            worksheetTD.InsertRow(rowImage, 9);
                            indexTDAddKiten += 9;
                            rangeTD = worksheetTD.Cells[rowImage - 1, 2]; rangeTD.Value = item["NoiDung"].ToString();
                            rangeTD = worksheetTD.Cells[rowImage, 2, rowImage + 8, 10]; rangeTD.Merge = true;
                            if (item["Image"].ToString() != "") Addpicutre(worksheetTD, item["Image"].ToString(), rowImage, 1, 5, savefoder1, 240, 140);
                        }
                        else
                        {
                            rangeTD = worksheetTD.Cells[rowImage - 1, 18]; rangeTD.Value = item["NoiDung"].ToString();
                            rangeTD = worksheetTD.Cells[rowImage, 18, rowImage + 8, 26]; rangeTD.Merge = true;
                            if (item["Image"].ToString() != "") Addpicutre(worksheetTD, item["Image"].ToString(), rowImage, 17, 5, savefoder1, 240, 140);
                        }
                        indexTD++;
                    }

                    int rowKyten = rowImage + indexTDAddKiten + 3;
                    int rowKytenMoi = rowImage + indexTDAddKiten + 3;
                    int checkIndexKT = 0;
                    for (int i = 0; i < dtTableTDKyTen.Rows.Count; i++)
                    {

                        if (checkIndexKT == 5)
                        {
                            rangeTD = worksheetTD.Cells[rowKytenMoi, 14, rowKytenMoi, 20]; rangeTD.Merge = true; rangeTD.Value = dtTableTDKyTen.Rows[i]["HoTen"].ToString();
                            rangeTD = worksheetTD.Cells[rowKytenMoi, 21, rowKytenMoi, 23]; rangeTD.Merge = true; rangeTD.Value = dtTableTDKyTen.Rows[i]["BoPhan"].ToString();
                            rangeTD = worksheetTD.Cells[rowKytenMoi, 24, rowKytenMoi, 26]; rangeTD.Merge = true;
                            if (dtTableTDKyTen.Rows[i]["KyTen"].ToString() != "") Addpicutre(worksheetTD, dtTableTDKyTen.Rows[i]["KyTen"].ToString(), rowKytenMoi - 1, 23, 5, foderA, 70, 50);
                            worksheetTD.Row(rowKytenMoi).Height = 50;
                            rowKytenMoi++;
                        }
                        else
                        {
                            rangeTD = worksheetTD.Cells[rowKyten, 1, rowKyten, 7]; rangeTD.Merge = true; rangeTD.Value = dtTableTDKyTen.Rows[i]["HoTen"].ToString();
                            rangeTD = worksheetTD.Cells[rowKyten, 8, rowKyten, 10]; rangeTD.Merge = true; rangeTD.Value = dtTableTDKyTen.Rows[i]["BoPhan"].ToString();
                            rangeTD = worksheetTD.Cells[rowKyten, 11, rowKyten, 13]; rangeTD.Merge = true;
                            if (dtTableTDKyTen.Rows[i]["KyTen"].ToString() != "") Addpicutre(worksheetTD, dtTableTDKyTen.Rows[i]["KyTen"].ToString(), rowKyten - 1, 10, 5, foderA, 70, 50);
                            worksheetTD.Row(rowKyten).Height = 50;
                            rowKyten++;
                            checkIndexKT++;
                        }

                    }


                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"BienBanKiemTra.xlsx";


                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet]
        [Route("BCExcel")]
        public HttpResponseMessage BCExcel(string action, string Para1, string Para2, string Para3)
        {
            try
            {
                DataTable dtTable = new PackageListXuatHangModel().Get(action, Para1, Para2);
                DataTable dtImageOther = new TheoDoiDonHangModel().Get("GetImageOther", Para1, Para2, "");
                //string imagePathRelative = "/Content/Templates/bienbanhangxuat.xlsx";
                string imagePathRelative = $"/Content/Templates/{(Para3 == "TGI" ? "bienbanhangxuat _TGI" : "bienbanhangxuat _TGA")}.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);
                dtTable = dtTable.AsEnumerable().Where(x => x["MaPhieuBB"].ToString() == Para1).CopyToDataTable();

                var savefoder = HttpContext.Current.Server.MapPath("~/Images/All");
                var savefoder1 = HttpContext.Current.Server.MapPath("~/Images/Other");
                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["sample_ Hinh anh t.d hang xuat"];
                    ExcelRange range = worksheet.Cells;
                    if (Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) != 0)
                    {
                        range = worksheet.Cells["D6"]; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1 ? "✔️" : "";
                        if (Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1)
                        {
                            range = worksheet.Cells["i6"]; range.Value = dtTable.Rows[0]["LoaiFeets"]; worksheet.Column(9).Width = 6;
                        }
                        else
                        {
                            range = worksheet.Cells["o6"]; range.Value = dtTable.Rows[0]["LoaiTan"]; worksheet.Column(9).Width = 6;
                        }
                        range = worksheet.Cells["J6"]; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "✔️" : "";
                    }
                    DateTime dateTime = Convert.ToDateTime(dtTable.Rows[0]["NgayGioDong"]);
                    string formattedDateNgayGioDong = dateTime.ToString("dd/MM/yyyy HH:mm");
                    DateTime dateGioKt = Convert.ToDateTime(dtTable.Rows[0]["NgayGioKT"]);
                    string formattedDateNgayKT = dateGioKt.ToString("dd/MM/yyyy HH:mm");
                    string kienAndSp = dtTable.Rows[0]["SLThung"] + " kiện = " + dtTable.Rows[0]["SoLuong"] + " sp";
                    range = worksheet.Cells["F8:J8"]; range.Merge = true; range.Value = dtTable.Rows[0]["KhachHang"].ToString();
                    range = worksheet.Cells["o8:s8"]; range.Merge = true; range.Value = dtTable.Rows[0]["MaHang"].ToString();
                    range = worksheet.Cells["v8:v8"]; range.Value = dtTable.Rows[0]["PO"].ToString();
                    range = worksheet.Cells["f9:j9"]; range.Merge = true; range.Value = kienAndSp.ToString();
                    range = worksheet.Cells["p9:s9"]; range.Merge = true; range.Value = formattedDateNgayGioDong;
                    range = worksheet.Cells["x9:aa9"]; range.Merge = true; range.Value = formattedDateNgayKT;
                    range = worksheet.Cells["E10:k10"]; range.Merge = true; range.Value = dtTable.Rows[0]["Soxe"].ToString();
                    range = worksheet.Cells["o10:s10"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "" : dtTable.Rows[0]["SoCont"].ToString();
                    range = worksheet.Cells["W10:aa10"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "" : dtTable.Rows[0]["SoRoMoc"].ToString();
                    range = worksheet.Cells["E11:k11"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenTaiXe"].ToString();
                    range = worksheet.Cells["o11:s11"]; range.Merge = true; range.Value = dtTable.Rows[0]["CMND"].ToString();
                    range = worksheet.Cells["W11:W11"]; range.Value = dtTable.Rows[0]["CongTy"].ToString();
                    range = worksheet.Cells["M50:P51"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenSeal1"].ToString();

                    range = worksheet.Cells["b14:k22"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image0"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image0"].ToString(), 13, 1, 5, savefoder, 240, 140);

                    range = worksheet.Cells["M14:P22"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageDoAm"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageDoAm"].ToString(), 13, 12, 5, savefoder, 180, 140);

                    range = worksheet.Cells["R14:aa22"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image25"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image25"].ToString(), 13, 17, 5, savefoder, 240, 140);
                    range = worksheet.Cells["b25:k33"]; range.Merge = true;


                    if (dtTable.Rows[0]["Image50"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image50"].ToString(), 24, 1, 5, savefoder, 240, 140);
                    range = worksheet.Cells["R25:aa33"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image75"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image75"].ToString(), 24, 17, 5, savefoder, 240, 140);

                    range = worksheet.Cells["b36:k44"]; range.Merge = true;
                    if (dtTable.Rows[0]["Image100"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["Image100"].ToString(), 35, 1, 5, savefoder, 240, 140);
                    range = worksheet.Cells["R36:aa44"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageDongCua"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageDongCua"].ToString(), 35, 17, 5, savefoder, 240, 140);

                    range = worksheet.Cells["b47:k55"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageTruocChot"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageTruocChot"].ToString(), 46, 1, 5, savefoder, 240, 140);
                    range = worksheet.Cells["R47:aa55"]; range.Merge = true;
                    if (dtTable.Rows[0]["ImageChotAT"].ToString() != "") Addpicutre(worksheet, dtTable.Rows[0]["ImageChotAT"].ToString(), 46, 17, 5, savefoder, 240, 140);

                    int rowImage = 57;
                    int index = 0;
                    foreach (DataRow item in dtImageOther.Rows)
                    {
                        if (index == 2)
                        {
                            rowImage += 2;
                            index = 0;
                            rowImage += 9;
                        }
                        if (index == 0)
                        {
                            range = worksheet.Cells[rowImage - 1, 2]; range.Value = item["NoiDung"].ToString();
                            range = worksheet.Cells[rowImage, 2, rowImage + 8, 10]; range.Merge = true;
                            if (item["Image"].ToString() != "") Addpicutre(worksheet, item["Image"].ToString(), rowImage, 1, 5, savefoder1, 240, 140);
                        }
                        else
                        {
                            range = worksheet.Cells[rowImage - 1, 18]; range.Value = item["NoiDung"].ToString();
                            range = worksheet.Cells[rowImage, 18, rowImage + 8, 26]; range.Merge = true;
                            if (item["Image"].ToString() != "") Addpicutre(worksheet, item["Image"].ToString(), rowImage, 17, 5, savefoder1, 240, 140);
                        }
                        index++;
                    }




                    byte[] fileBytes = package.GetAsByteArray();

                    // Lưu tệp vào đường dẫn đã chỉ định

                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"BCTheoDoiHangXuat.xlsx";
                    package.SaveAs(new FileInfo(savefoder + "/" + fileName));

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet]
        [Route("BCExcelBB")]
        public HttpResponseMessage BCExcelBB(string action, string Para1, string Para2, string para3, string para4)
        {
            try
            {
                DataTable dtTable = new PackageListXuatHangModel().Get(action, Para1, Para2);
                DataTable dtTableKyTen = new PackageListXuatHangModel().Get("GetKyTenXuatHang", Para1, Para2);
                DataTable dtTableKiemTra = new PackageListXuatHangModel().Get("GetnoiDungXuatHang", Para1, Para2);
                DataTable dtTableTenhang = new PackageListXuatHangModel().Get("GetTenHang", Para1, para3);
                //string imagePathRelative = "/Content/Templates/bienbanKTCont.xlsx";
                string imagePathRelative = $"/Content/Templates/{(para4 == "TGI" ? "bienbanKTCont_TGI" : "bienbanKTCont_TGA")}.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

                var foder = HttpContext.Current.Server.MapPath("~/Images/KytenXuatHang");
                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Inspection - edit"];
                    ExcelRange range = worksheet.Cells;
                    if (Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) != 0)
                    {
                        range = worksheet.Cells["F6"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 1 ? "✔️" : "";
                        range = worksheet.Cells["l6"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["XeRomoc"]) == 1 ? "✔️" : "";
                        range = worksheet.Cells["r6"]; range.Merge = true; range.Value = Convert.ToInt32(dtTable.Rows[0]["VanChuyen"]) == 2 ? "✔️" : "";
                    }


                    range = worksheet.Cells["D7:J7"]; range.Merge = true; range.Value = "Loại   " + dtTable.Rows[0]["LoaiFeets"].ToString() + "    FEET";
                    range = worksheet.Cells["P7:U7"]; range.Merge = true; range.Value = "Loại   " + dtTable.Rows[0]["LoaiTan"].ToString() + "     TẤN";
                    range = worksheet.Cells["C9:F9"]; range.Merge = true; range.Value = dtTable.Rows[0]["SoXe"].ToString();
                    range = worksheet.Cells["j9:p9"]; range.Merge = true; range.Value = dtTable.Rows[0]["SoCont"].ToString();
                    range = worksheet.Cells["T9:z9"]; range.Merge = true; range.Value = dtTable.Rows[0]["SoRoMoc"].ToString();
                    range = worksheet.Cells["D10:j10"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenTaiXe"].ToString();
                    range = worksheet.Cells["m10:r10"]; range.Merge = true; range.Value = dtTable.Rows[0]["CMND"].ToString();
                    range = worksheet.Cells["V10:v10"]; range.Value = dtTable.Rows[0]["CongTy"].ToString(); range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range = worksheet.Cells["e11:l11"]; range.Merge = true; range.Value = Convert.ToDateTime(dtTable.Rows[0]["NgayXeDen"]).ToString("dd/MM/yyyy hh:mm");
                    range = worksheet.Cells["u11:z11"]; range.Merge = true; range.Value = Convert.ToDateTime(dtTable.Rows[0]["NgayGioCheckXeRoMoc"]).ToString("dd/MM/yyyy hh:mm");
                    if (Convert.ToInt32(dtTableKiemTra.Rows[0]["StatusKL"]) == 1)
                    {
                        range = worksheet.Cells["f44:l44"]; range.Merge = true; range.Value = Convert.ToDateTime(dtTable.Rows[0]["NgayGioDongHang"]).ToString("dd/MM/yyyy hh:mm");
                        range = worksheet.Cells["u44:z44"]; range.Merge = true; range.Value = Convert.ToDateTime(dtTable.Rows[0]["NgayGioKTDongHang"]).ToString("dd/MM/yyyy hh:mm");
                        range = worksheet.Cells["c52:h52"]; range.Merge = true; range.Value = dtTable.Rows[0]["Seal"].ToString();
                        range = worksheet.Cells["v52:z52"]; range.Merge = true; range.Value = Convert.ToDateTime(dtTable.Rows[0]["NgayGioXeDi"]).ToString("dd/MM/yyyy hh:mm");
                    }

                    int row = 26;
                    int index1 = 0;
                    for (int i = 0; i < dtTableKiemTra.Rows.Count + 1; i++)
                    {
                        if (row == 35)
                        {
                            row++;
                            continue;
                        };
                        if (row < 37 || row > 41)
                        {
                            range = worksheet.Cells[row, 9, row, 11]; range.Merge = true; range.Value = Convert.ToInt32(dtTableKiemTra.Rows[index1]["StatusCont"]) == 1 ? "✔️" : "";
                            range = worksheet.Cells[row, 12, row, 14]; range.Merge = true; range.Value = Convert.ToInt32(dtTableKiemTra.Rows[index1]["StatusCont"]) == 2 ? "✔️" : "";
                        }
                        if (row < 26 || row > 36)
                        {
                            range = worksheet.Cells[row, 15, row, 17]; range.Merge = true; range.Value = Convert.ToInt32(dtTableKiemTra.Rows[index1]["StatusRoMoc"]) == 1 ? "✔️" : "";
                            range = worksheet.Cells[row, 18, row, 20]; range.Merge = true; range.Value = Convert.ToInt32(dtTableKiemTra.Rows[index1]["StatusRoMoc"]) == 2 ? "✔️" : "";
                        }
                        if (row < 31 || row > 32 && row < 37 || row > 41)
                        {
                            range = worksheet.Cells[row, 21, row, 23]; range.Merge = true; range.Value = Convert.ToInt32(dtTableKiemTra.Rows[index1]["StatusXeTai"]) == 1 ? "✔️" : "";
                            range = worksheet.Cells[row, 24, row, 26]; range.Merge = true; range.Value = Convert.ToInt32(dtTableKiemTra.Rows[index1]["StatusXeTai"]) == 2 ? "✔️" : "";
                        }
                        row++;
                        index1++;

                    }
                    int rowKyten = 55;
                    int rowKytenMoi = 55;
                    for (int i = 0; i < dtTableKyTen.Rows.Count; i++)
                    {
                        if (rowKyten >= 60)
                        {
                            range = worksheet.Cells[rowKytenMoi, 14, rowKytenMoi, 20]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["HoTen"].ToString();
                            range = worksheet.Cells[rowKytenMoi, 21, rowKytenMoi, 23]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["BoPhan"].ToString();
                            range = worksheet.Cells[rowKytenMoi, 24, rowKytenMoi, 26]; range.Merge = true;
                            if (dtTableKyTen.Rows[i]["KyTen"].ToString() != "") Addpicutre(worksheet, dtTableKyTen.Rows[i]["KyTen"].ToString(), rowKytenMoi - 1, 23, 5, foder, 70, 50);
                            rowKytenMoi++;
                        }
                        else
                        {
                            range = worksheet.Cells[rowKyten, 1, rowKyten, 7]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["HoTen"].ToString();
                            range = worksheet.Cells[rowKyten, 8, rowKyten, 10]; range.Merge = true; range.Value = dtTableKyTen.Rows[i]["BoPhan"].ToString();
                            range = worksheet.Cells[rowKyten, 11, rowKyten, 13]; range.Merge = true;
                            if (dtTableKyTen.Rows[i]["KyTen"].ToString() != "") Addpicutre(worksheet, dtTableKyTen.Rows[i]["KyTen"].ToString(), rowKyten - 1, 10, 5, foder, 70, 50);
                            rowKyten++;
                        }

                    }

                    int rowTenhang = 47;
                    int index = dtTableTenhang.Rows.Count - 4;
                    int dtTableTenhang1 = dtTableTenhang.AsEnumerable().Sum(x => Convert.ToInt32(x["SoLuong"]));
                    int dtTableTenhangSoKien = dtTableTenhang.AsEnumerable().Sum(x => Convert.ToInt32(x["SLThung"]));
                    foreach (DataRow item in dtTableTenhang.Rows)
                    {
                        if (index > 4)
                        {
                            worksheet.InsertRow(rowTenhang, 1);
                        }
                        range = worksheet.Cells[rowTenhang, 1, rowTenhang, 4]; range.Value = item["TenHang"].ToString(); range.Merge = true;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; range.Style.Font.Size = 11;
                        range = worksheet.Cells[rowTenhang, 5, rowTenhang, 11]; range.Value = item["MaHang"].ToString(); range.Merge = true;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; range.Style.Font.Size = 11;
                        range = worksheet.Cells[rowTenhang, 12, rowTenhang, 16]; range.Value = item["PO"].ToString(); range.Merge = true;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; range.Style.Font.Size = 11;
                        range = worksheet.Cells[rowTenhang, 17, rowTenhang, 21]; range.Value = Convert.ToInt32(item["SoLuong"]); range.Merge = true;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; range.Style.Font.Size = 11;
                        range = worksheet.Cells[rowTenhang, 22, rowTenhang, 26]; range.Value = item["GhiChu"].ToString(); range.Merge = true;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; range.Style.Font.Size = 11;
                        rowTenhang++;
                        index++;
                    }
                    var borderData = worksheet.Cells[47, 1, rowTenhang - 1, 26].Style.Border;
                    borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = ExcelBorderStyle.Thin;
                    if (rowTenhang < 51)
                    {
                        rowTenhang = 51;
                    }

                    range = worksheet.Cells["e42"];
                    range.Value = Convert.ToInt32(dtTableKiemTra.Rows[0]["StatusKL"]) == 1 ? "✔️" : "";
                    range = worksheet.Cells["m42"];
                    range.Value = Convert.ToInt32(dtTableKiemTra.Rows[0]["StatusKL"]) == 2 ? "✔️" : "";
                    range = worksheet.Cells[rowTenhang, 22]; range.Value = dtTableTenhangSoKien + " kiện = " + dtTableTenhang1 + " SP"; range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"BienBanKiemTra.xlsx";


                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }
        private void Addpicutre(ExcelWorksheet worksheet, string sign, int row, int col, int toado, string foder, int chieudai, int chieurong)
        {
            var pathFolder = foder;
            string imagePath = Path.Combine(pathFolder, sign);
            // Chèn ảnh vào worksheet
            if (File.Exists(imagePath))
            {
                FileInfo imageFile = new FileInfo(imagePath);
                ExcelPicture picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);
                picture.SetPosition(row, 5, col, toado);
                picture.SetSize(chieudai, chieurong);
            }
        }

        #endregion
    }
}