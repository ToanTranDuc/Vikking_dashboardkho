using NtbSoft.ERP.Model.KeHoach;
using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/ScanBarcode")]
    public class ScanBarcodeController : ApiController
    {
        #region Liên kết android      

        [HttpGet]
        [Route("GetAllInfo")]
        public dynamic GetScan(string Para1, string Para2, string Para3 = "", string Para4 = "", string Para5 = "",
                                                                           string Para6 = "", string Para7 = "", string Para8 = "")
        {
            DataSet dsPhieu = new ScanBarcodeModel().Get("GetPhieuXHScanV2", Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8); // DS phiếu
            DataSet dsSize = new ScanBarcodeModel().Get("GetCTPhieuScan", Para1, "All", Para3, Para4, Para5, Para6, Para7, Para8);
            DataSet dsKien = new ScanBarcodeModel().Get("GetCTPhieuScan_CT", Para1, "All", Para3, Para4, Para5, Para6, Para7, Para8);
            return new { DataPhieu = dsPhieu.Tables[0] , DataSize = dsSize.Tables[0],
                        DataThungT = dsKien.Tables[0], DataThungCT = dsKien.Tables[1]};
        }
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
        public dynamic GetScan(string action, string Para1, string Para2, string Para3 = "", string Para4 = "", string Para5 = "", 
                                                                            string Para6 = "", string Para7="", string Para8 = "")
        {
            DataSet ds = new ScanBarcodeModel().Get(action, Para1, Para2, Para3,Para4, Para5, Para6, Para7, Para8);
            return ds.Tables[0];
        }
        [HttpGet]
        [Route("GetDS")]
        public dynamic GetDS(string action, string Para1, string Para2, string Para3 = "")
        {
            DataSet ds = new ScanBarcodeModel().Get(action, Para1, Para2, Para3);
            return new { data1 = ds.Tables[0], data2 = ds.Tables[1] };
        }
        /// <summary>
        /// Lấy thông tin phiếu
        /// </summary>
        /// <param name="action">GetCTPhieuScan_CT</param>
        /// <param name="Para1">MaPhieu</param>
        /// <param name="Para2">Cont = All</param>
        /// <param name="Para3"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCT")]
        public dynamic GetScanCT(string action, string Para1, string Para2, string Para3 = "")
        {
            DataSet ds = new ScanBarcodeModel().Get(action, Para1, Para2, Para3);
            return new { DataT = ds.Tables[0], DataChiTiet = ds.Tables[1] };
        }
        /// <summary>
        /// Lưu dữ liệu quét được
        /// </summary>
        /// <param name="action">UpdateScan or UpdateScan_CT</param>
        /// <param name="dtSave"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateScanV2")]
        public string UpdateScanV2(string action, DataTable dtSave)
        {
            return new ScanBarcodeModel().UpdateScan(action, dtSave);
        }
        [HttpPost]
        [Route("UpdateScanApp")]
        public string UpdateScanApp(string action, DataTable dtSave)
        {

            if (SaveCaiDatBarcode("Post", "", dtSave) == "True")
            {
                var tblConvertSave = CreatetblScanBarcodeXH();
                foreach (DataRow dr in dtSave.Rows)
                {
                    var drNew = tblConvertSave.NewRow();
                    drNew["ID"] = dr["ID"];
                    drNew["MaPKL_XH"] = dr["MaPKL_XH"];
                    drNew["Cont"] = dr["Cont"];
                    drNew["MaDH"] = dr["MaDH"];
                    drNew["MaHang"] = dr["MaHang"];
                    drNew["Season"] = dr["Dot"];
                    drNew["POID"] = dr["POID"];
                    drNew["Store"] = dr["Store"];
                    drNew["DauSizeID"] = dr["DauSizeID"];
                    drNew["ColorID"] = dr["ColorID"];
                    drNew["SizeID"] = dr["SizeID"];
                    drNew["STQuet"] = dr["STDaQuet"];
                    drNew["SttThungMin"] = 0;
                    drNew["IsThungLe"] = (bool)dr["IsThungLe"] ? 1 : 0;
                    drNew["Barcode"] = dr["Barcode"];
                    drNew["NhanVien"] = "";

                    tblConvertSave.Rows.Add(drNew);
                }
                return new ScanBarcodeModel().UpdateScan(action, tblConvertSave);
            }
            else
            {
                return "False";
            }
        }
        [HttpPost]
        [Route("UpdateScanAppV2")]
        public string UpdateScanAppV2(string action, DataTable dtSave)
        {
            var tblConvertSave = CreateTblSave();
            foreach (DataRow dr in dtSave.Rows)
            {
                var drNew = tblConvertSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaPKL"] = dr["MaPKL"];
                drNew["MaDH"] = dr["MaDH"];
                drNew["POID"] = dr["POID"];   
                drNew["SttThung"] = dr["SttThung"];
                drNew["QRCode"] = dr["Barcode"];

                tblConvertSave.Rows.Add(drNew);
            }
            return new ScanBarcodeModel().UpdateScan(action, tblConvertSave);
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
            var tblConvertSaveCaiDaBarcode = CreateTblSaveCaiDatBarcode();
            foreach (DataRow dr in dt.Rows)
            {
                if (!CaiDatBarCodeModel.CheckBarCodeFromApp(tblConvertSaveCaiDaBarcode, dr)) continue;
                //  var dtCheckA = tblConvertSaveCaiDaBarcode.AsEnumerable().Where(x => x["POID"].ToString() == dr["POID"].ToString()
                //                                                                && x["Season"].ToString() == dr["Dot"].ToString()
                //                                                                && x["DauSizeID"].ToString() == dr["DauSizeID"].ToString()
                //                                                                && x["MaMau"].ToString() == dr["ColorID"].ToString()
                //                                                                && x["SizeID"].ToString() == dr["SizeID"].ToString());
                //if(dtCheckA != null)
                //{
                //    var a = dtCheckA.
                //    var dtCheckTemp = dtCheckA.CopyToDataTable();
                //    var drLap = dtCheckTemp.Rows[0];
                //    if ((bool)dr["IsThungLe"]) drLap["BarCodeChan"] =dr["Barcode"];
                //    foreach(DataRow x in tblConvertSaveCaiDaBarcode.Rows)
                //    {
                //        if (x["POID"].ToString() == drLap["POID"].ToString()
                //           && x["Season"].ToString() == drLap["Dot"].ToString()
                //           && x["DauSizeID"].ToString() == drLap["DauSizeID"].ToString()
                //           && x["MaMau"].ToString() == drLap["ColorID"].ToString()
                //           && x["SizeID"].ToString() == drLap["SizeID"].ToString())
                //        {
                //            if ((bool)dr["IsThungLe"]) drLap["BarCodeChan"] = dr["Barcode"];
                //            else drLap["BarCodeLe"] = dr["Barcode"];
                //        }
                //        break;
                //    }
                //    continue;
                //}
                var drNew = tblConvertSaveCaiDaBarcode.NewRow();
                drNew["ID"] = dr["ID"];
                drNew["StyleID"] = dr["MaHang"];
                drNew["Season"] = dr["Dot"];
                drNew["POID"] = dr["POID"];
                drNew["Store"] = dr["Store"];
                drNew["DauSizeID"] = dr["DauSizeID"];
                drNew["MaMau"] = dr["ColorID"];
                drNew["SizeID"] = dr["SizeID"];
                drNew["BarCodeChan"] = (bool)dr["IsThungLe"] ? "default" : dr["Barcode"];
                drNew["SoLuongChan"] = 0;
                drNew["BarCodeLe"] = (bool)dr["IsThungLe"] ? dr["Barcode"] : "default";
                drNew["SoLuongLe"] = 0;
                drNew["CreateDate"] = DateTime.Now;
                tblConvertSaveCaiDaBarcode.Rows.Add(drNew);
            }
            return new CaiDatBarCodeModel().Post(action, Para1, tblConvertSaveCaiDaBarcode);
        }
        private DataTable CreatetblScanBarcodeXH()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPKL_XH", typeof(string));
            dt.Columns.Add("Cont", typeof(string));
            dt.Columns.Add("MaDH", typeof(string)); // k cần
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("STQuet", typeof(int));
            dt.Columns.Add("SttThungMin", typeof(int));  // k cần
            dt.Columns.Add("IsThungLe", typeof(int));
            dt.Columns.Add("Barcode", typeof(string));
            dt.Columns.Add("NhanVien", typeof(string));
            return dt;
        }
        private DataTable CreateTblSaveCaiDatBarcode()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));           
            dt.Columns.Add("BarCodeChan", typeof(string));
            dt.Columns.Add("SoLuongChan", typeof(int));
            dt.Columns.Add("BarCodeLe", typeof(string));
            dt.Columns.Add("SoLuongLe", typeof(int));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            return dt;
        }
        private DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int)); //Cần
            tbl.Columns.Add("MaPKL", typeof(string)); //Cần
            tbl.Columns.Add("MaDH", typeof(string)); //Cần
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaLenh", typeof(string));
            tbl.Columns.Add("DotSX", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("POID", typeof(string)); //Cần
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
            tbl.Columns.Add("SttThung", typeof(int)); //Cần
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
        #endregion
    }
}