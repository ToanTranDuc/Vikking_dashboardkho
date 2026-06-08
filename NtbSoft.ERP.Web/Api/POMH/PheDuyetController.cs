using System.Web.Http;
using System.Threading.Tasks;
using System.Data;
using NtbSoft.ERP.Model.POMH;
using NtbSoft.ERP.Entity.POMH;
using Newtonsoft.Json;
using System.Collections.Generic;
using NtbSoft.ERP.Model.QuanLyDonHang;
using System.Linq;
using System;

namespace NtbSoft.ERP.Web.Api.POMH
{
    [RoutePrefix("api/PheDuyet")]
    public class PheDuyetController : ApiController
    {

        private PheDuyetModel _model = new PheDuyetModel();

        [HttpGet]
        [Route("GET")]
        public async Task<DataTable> GET(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Get(action, para1, para2, para3, para4, para5);
        }
        [HttpGet]
        [Route("GETDS")]
        public async Task<DataSet> GETDS(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.GETDS(action, para1, para2, para3, para4, para5);
        }


        [HttpPost]
        [Route("Post")]
        public async Task<string> Post(List<PheDuyetPhieuEntity> lstPheDuyetPhieu)
        {
            if (lstPheDuyetPhieu == null) return "false";
            string json = JsonConvert.SerializeObject(lstPheDuyetPhieu);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.Post(lstPheDuyetPhieu[0].Action, tblSave);
        }


        [HttpPost]
        [Route("PostDanhGiaNCC")]
        public async Task<string> PostDanhGiaNCC(List<XacNhanDanhGiaNhaCCEntiy> lstPheDuyetPhieu)
        {
            if (lstPheDuyetPhieu == null) return "false";
            string json = JsonConvert.SerializeObject(lstPheDuyetPhieu);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.PostDanhGiaNCC(tblSave);
        }


        /*BOM*/
        [HttpGet]
        [Route("GETBOM")]
        public DataTable GETBOM(string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {

            DataTable dtResult = new DataTable();
            // Add ALL fixed columns
            dtResult.Columns.Add("ID", typeof(int));
            dtResult.Columns.Add("MaHang", typeof(string));
            dtResult.Columns.Add("MaKH", typeof(string));
            dtResult.Columns.Add("MaNhom", typeof(string));
            dtResult.Columns.Add("TenNhom", typeof(string));
            dtResult.Columns.Add("NPL", typeof(bool));
            dtResult.Columns.Add("Sort", typeof(int));
            dtResult.Columns.Add("MaVTID", typeof(string));
            dtResult.Columns.Add("MaVT", typeof(string));
            dtResult.Columns.Add("ChiTiet", typeof(string));
            dtResult.Columns.Add("MaDVVT", typeof(string));
            dtResult.Columns.Add("TenDVVT", typeof(string));
            dtResult.Columns.Add("MaMauVT", typeof(string));
            dtResult.Columns.Add("MauVT", typeof(string));
            dtResult.Columns.Add("GhiChu", typeof(string));
            dtResult.Columns.Add("STT", typeof(long));
            dtResult.Columns.Add("KhoVaiID", typeof(string));
            dtResult.Columns.Add("KhoVai", typeof(string));
            dtResult.Columns.Add("TachMau", typeof(bool));
            dtResult.Columns.Add("DinhMucHaoHut", typeof(decimal));
            dtResult.Columns.Add("DinhMucChung", typeof(decimal));
            dtResult.Columns.Add("TrangThai", typeof(string));
            dtResult.Columns.Add("MaCode", typeof(string));
            dtResult.Columns.Add("STTCode", typeof(int));
            dtResult.Columns.Add("IsActive", typeof(bool));
            dtResult.Columns.Add("MaNhomChiTiet", typeof(string));
            dtResult.Columns.Add("MauVTIDChung", typeof(string));
            dtResult.Columns.Add("MaSizeChung", typeof(string));
            dtResult.Columns.Add("Size", typeof(string));
            dtResult.Columns.Add("IsNew", typeof(int));
            dtResult.Columns.Add("TenNhomChiTiet", typeof(string));


            try
            {
                para1 = para1 ?? "";
                para2 = para2 ?? "";
                para3 = para3 ?? "";
                para4 = para4 ?? "";
                para5 = para5 ?? "";

                KhoiTaoBOMV1Model _modelBOM = new KhoiTaoBOMV1Model();
                DataTable dtMain = _modelBOM.Get("GETVTSP_V1", para1, para2, para3, "", "", "", "", "", "", "");



                if (dtMain != null && dtMain?.Rows?.Count == 0)
                {
                    return dtResult;
                }
                DataTable tblAllSize = _modelBOM.Get("GETSIZESP", para1, para2, "", "", "", "", "", "", "", "");
                DataTable dtSize = _modelBOM.Get("GETSIZESP_V1", para1, para2, para3, "", "", "", "", "", "", "");
                DataTable dtColorMap = _modelBOM.Get("GETMAUSP_V1", para1, para2, para3, "", "", "", "", "", "", "");
                DataTable dtColors = _modelBOM.Get("GETBANGMAU_V1", para1, para2, para3, "", "", "", "", "", "", "");
                DataTable tblChungLoaiChiTiet = _modelBOM.Get("GETCHUNGLOAICHITIET", "", "", "", "", "", "", "", "", "", "");
                DataTable tblMauVT = _modelBOM.Get("GETMAUVTTV", "", "", "", "", "", "", "", "", "", "");

                // Add dynamic color columns
                var colorColumnMapping = new Dictionary<string, string>();
                foreach (DataRow colorRow in dtColors.Rows)
                {
                    string maMau = colorRow["MaMau"].ToString();
                    string tenMau = colorRow["TenMau"].ToString();
                    string columnName = $"{tenMau}@Mau@{maMau}";

                    if (!dtResult.Columns.Contains(columnName))
                    {
                        dtResult.Columns.Add(columnName, typeof(string));
                        colorColumnMapping.Add(columnName, maMau);
                    }
                }

                var groupedData = dtMain.AsEnumerable()
                              .GroupBy(r => new
                              {
                                  MaHang = r["MaHang"]?.ToString() ?? "",
                                  MaKH = r["MaKH"]?.ToString() ?? "",
                                  MaNhom = r["MaNhom"]?.ToString() ?? "",
                                  TenNhom = r["TenNhom"]?.ToString() ?? "",
                                  NPL = r["NPL"] != DBNull.Value ? Convert.ToBoolean(r["NPL"]) : false,
                                  Sort = r["Sort"] != DBNull.Value ? Convert.ToInt32(r["Sort"]) : 0,
                                  MaVTID = r["MaVTID"]?.ToString() ?? "",
                                  MaVT = r["MaVT"]?.ToString() ?? "",
                                  ChiTiet = r["ChiTiet"]?.ToString() ?? "",
                                  MaDVVT = r["MaDVVT"]?.ToString() ?? "",
                                  TenDVVT = r["TenDVVT"]?.ToString() ?? "",
                                  GhiChu = r["GhiChu"]?.ToString() ?? "",
                                  STT = r["STT"] != DBNull.Value ? Convert.ToInt64(r["STT"]) : 0L,
                                  KhoVaiID = r["KhoVaiID"]?.ToString() ?? "",
                                  IsKV = r["IsKV"] != DBNull.Value ? Convert.ToBoolean(r["IsKV"]) : false,
                                  KhoVai = r["KhoVai"]?.ToString() ?? "",
                                  MaTheSize = r["MaTheSize"]?.ToString() ?? "",
                                  TachMau = r["TachMau"] != DBNull.Value ? Convert.ToBoolean(r["TachMau"]) : false,
                                  DinhMucHaoHut = r["DinhMucHaoHut"] != DBNull.Value ? Convert.ToDecimal(r["DinhMucHaoHut"]) : 0m,
                                  DinhMucChung = r["DinhMucChung"] != DBNull.Value ? Convert.ToDecimal(r["DinhMucChung"]) : 0m,
                                  TrangThai = r["IsXetDuyet"]?.ToString() ?? "",
                                  MaCode = r["MaCode"]?.ToString() ?? "",
                                  STTCode = r["STTCode"] != DBNull.Value ? Convert.ToInt32(r["STTCode"]) : 0,
                                  IsActive = r["IsActive"] != DBNull.Value ? Convert.ToBoolean(r["IsActive"]) : false,
                                  MaNhomChiTiet = r["MaNhomChiTiet"]?.ToString() ?? "",
                                  MaDot = r["MaDot"]?.ToString() ?? ""
                              })
                                  .OrderBy(g => g.Key.Sort)
                                  .ThenBy(g => g.Key.STT);

                int id = 1;
                foreach (var group in groupedData)
                {
                    string TenNhomCTCT = string.Empty;
                    if (tblChungLoaiChiTiet != null || tblChungLoaiChiTiet?.Rows?.Count > 0)
                    {
                        var Query = tblChungLoaiChiTiet.AsEnumerable().FirstOrDefault(x => x["MaNhomChiTiet"]?.ToString() == group.Key.MaNhomChiTiet);
                        if (Query != null)
                        {
                            TenNhomCTCT = Query["TenNhomChiTiet"]?.ToString();
                        }
                    }
                    DataRow resultRow = dtResult.NewRow();

                    // Fill fixed columns từ group key
                    resultRow["ID"] = id++;
                    resultRow["MaHang"] = group.Key.MaHang;
                    resultRow["MaKH"] = group.Key.MaKH;
                    resultRow["MaNhom"] = group.Key.MaNhom;
                    resultRow["TenNhom"] = group.Key.TenNhom;
                    resultRow["NPL"] = group.Key.NPL;
                    resultRow["Sort"] = group.Key.Sort;
                    resultRow["MaVTID"] = group.Key.MaVTID;
                    resultRow["MaVT"] = group.Key.MaVT;
                    resultRow["ChiTiet"] = group.Key.ChiTiet;
                    resultRow["MaDVVT"] = group.Key.MaDVVT;
                    resultRow["TenDVVT"] = group.Key.TenDVVT;
                    //resultRow["MauVTIDChung"] = group.Key.MauVTIDChung;// tôi mới bổ sung
                    resultRow["MauVTIDChung"] = group.First()["MauVTIDChung"]?.ToString() ?? "";
                    //resultRow["MauVT"] = group.Key.MauVT;
                    resultRow["GhiChu"] = group.Key.GhiChu;
                    resultRow["STT"] = group.Key.STT;
                    resultRow["KhoVaiID"] = group.Key.KhoVaiID;
                    resultRow["KhoVai"] = group.Key.KhoVai;
                    resultRow["TachMau"] = group.Key.TachMau;
                    resultRow["DinhMucHaoHut"] = group.Key.DinhMucHaoHut;
                    resultRow["DinhMucChung"] = group.Key.DinhMucChung;
                    resultRow["TrangThai"] = group.Key.TrangThai;
                    resultRow["MaCode"] = group.Key.MaCode;
                    resultRow["STTCode"] = group.Key.STTCode;
                    resultRow["IsActive"] = group.Key.IsActive;
                    resultRow["MaNhomChiTiet"] = group.Key.MaNhomChiTiet;
                    resultRow["IsNew"] = 0;
                    resultRow["TenNhomChiTiet"] = TenNhomCTCT;

                    // -------- XỬ LÝ MauVTIDChung: STUFF tất cả MauVTID trong group --------
                    var mauVTIDs = group
                        .Select(r => r["MauVTID"].ToString())
                        .Distinct()
                        .OrderBy(x => x);
                    //resultRow["MauVTIDChung"] = string.Join(", ", mauVTIDs);

                    // -------- XỬ LÝ Size Data --------
                    var sizeGroups = dtSize.AsEnumerable()
                        .Where(r => r["MaVTID"].ToString() == group.Key.MaVTID
                            && r["MaNhom"].ToString() == group.Key.MaNhom
                            && r["KhoVaiID"].ToString() == group.Key.KhoVaiID
                              && r["MaCode"].ToString() == group.Key.MaCode)
                        .GroupBy(r => new
                        {
                            MaNhomSize = r["MaNhomSize"].ToString(),
                            NhomSize = r["NhomSize"] != DBNull.Value ? r["NhomSize"].ToString() : r["MaNhomSize"].ToString()
                        })
                        .OrderBy(g => g.Key.MaNhomSize);

                    var maSizeChungParts = new List<string>();
                    var sizeChungParts = new List<string>();

                    foreach (var sizeGroup in sizeGroups)
                    {
                        var sizes = sizeGroup
                            .OrderBy(r => Convert.ToInt32(r["Sort"]))
                            .ThenBy(r => r["MaSize"].ToString())
                            .Select(r => r["MaSize"].ToString())
                            .Distinct();

                        var tenSizes = sizeGroup
                            .OrderBy(r => Convert.ToInt32(r["Sort"]))
                            .ThenBy(r => r["TenSize"] != DBNull.Value ? r["TenSize"].ToString() : r["MaSize"].ToString())
                            .Select(r => r["TenSize"] != DBNull.Value ? r["TenSize"].ToString() : r["MaSize"].ToString())
                            .Distinct();

                        maSizeChungParts.Add($"{sizeGroup.Key.MaNhomSize}: {string.Join(", ", sizes)}");
                        sizeChungParts.Add($"{sizeGroup.Key.NhomSize}: {string.Join(", ", tenSizes)}");
                    }

                    resultRow["MaSizeChung"] = string.Join("; ", maSizeChungParts);
                    resultRow["Size"] = string.Join("; ", sizeChungParts);

                    // -------- XỬ LÝ KhoVai/Size based on IsKV --------
                    if (!group.Key.IsKV)
                    {
                        var sizeInfo = dtSize.AsEnumerable()
                            .FirstOrDefault(r => r["MaSize"].ToString() == group.Key.KhoVaiID);

                        if (sizeInfo != null)
                        {
                            resultRow["KhoVaiID"] = sizeInfo["MaSize"].ToString();
                            resultRow["KhoVai"] = sizeInfo["TenSize"] != DBNull.Value
                                ? sizeInfo["TenSize"].ToString()
                                : group.Key.KhoVai;
                        }
                    }

                    // -------- XỬ LÝ PIVOT COLOR COLUMNS --------
                    // Initialize tất cả color columns = empty
                    foreach (var kvp in colorColumnMapping)
                    {
                        resultRow[kvp.Key] = "";
                    }

                    // Duyệt qua tất cả rows trong group để fill MauVTID vào đúng color column
                    // LẤY DANH SÁCH MauVTID của group này từ dtColorMap
                    var colorMappingsForGroup = dtColorMap.AsEnumerable()
                        .Where(r => r["MaVTID"].ToString() == group.Key.MaVTID
                            && r["MaNhom"].ToString() == group.Key.MaNhom
                            && r["KhoVaiID"].ToString() == group.Key.KhoVaiID
                             && r["MaCode"].ToString() == group.Key.MaCode)
                        .ToList();

                    // Duyệt qua từng mapping để fill vào đúng cột màu
                    foreach (DataRow colorMapping in colorMappingsForGroup)
                    {
                        string mauVTID = colorMapping["MauVTID"].ToString();
                        string maMau = colorMapping["MaMau"].ToString();
                        string tenMau = colorMapping["TenMau"].ToString();
                        // Tìm column name tương ứng với MaMau này
                        var matchedColumn = colorColumnMapping.FirstOrDefault(x => x.Value == maMau);

                        if (!string.IsNullOrEmpty(matchedColumn.Key))
                        {
                            // Fill MauVTID vào column tương ứng
                            string currentValue = resultRow[matchedColumn.Key].ToString();
                            if (string.IsNullOrEmpty(currentValue))
                            {
                                string TenMau = string.Empty;
                                if (tblMauVT != null || tblMauVT?.Rows?.Count > 0)
                                {
                                    var Query = tblMauVT.AsEnumerable().FirstOrDefault(x => x["MauVTID"]?.ToString() == mauVTID);
                                    if (Query != null)
                                    {
                                        TenMau = Query["MaMauVT"]?.ToString();
                                    }
                                }

                                resultRow[matchedColumn.Key] = TenMau;
                            }
                            //else
                            //{
                            //    // Nếu đã có giá trị thì append (trường hợp 1 màu có nhiều MauVTID)
                            //    resultRow[matchedColumn.Key] = currentValue + ", " + mauVTID;
                            //}
                            // tôi mới comneent lại 
                        }
                    }

                    dtResult.Rows.Add(resultRow);
                }
                ProcessSizeData(dtResult, tblAllSize);
            }
            catch (Exception ex)
            {

            }

            return dtResult;
        }


        private void ProcessSizeData(DataTable tbl, DataTable tblAllSize)
        {
            // Lấy tất cả cặp MaNhomSize-MaSize từ tblAllSize
            HashSet<string> allSizePairs = new HashSet<string>();

            foreach (DataRow row in tblAllSize.Rows)
            {
                string maNhomSize = row["MaNhomSize"].ToString().Trim();
                string maSize = row["MaSize"].ToString().Trim();

                if (!string.IsNullOrEmpty(maNhomSize) && !string.IsNullOrEmpty(maSize))
                {
                    string pair = $"{maNhomSize}-{maSize}";
                    allSizePairs.Add(pair);
                    Console.WriteLine($"  - '{pair}'");
                }
            }

            Console.WriteLine($"\nTổng số cặp MaNhomSize-MaSize trong tblAllSize: {allSizePairs.Count}\n");

            // Duyệt qua từng dòng trong tbl
            foreach (DataRow row in tbl.Rows)
            {
                string maSizeChung = row["MaSizeChung"].ToString();

                HashSet<string> sizePairsInRow = new HashSet<string>();

                // Tách chuỗi MaSizeChung theo format: "MaNhomSize1:MaSize1,MaSize2;MaNhomSize2:MaSize3,MaSize4"
                string[] nhomSizes = maSizeChung.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string nhomSize in nhomSizes)
                {
                    string[] parts = nhomSize.Split(':');
                    if (parts.Length == 2)
                    {
                        string maNhomSize = parts[0].Trim();

                        // Lấy các MaSize và ghép với MaNhomSize
                        string[] maSizes = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string maSize in maSizes)
                        {
                            string trimmedMaSize = maSize.Trim();
                            if (!string.IsNullOrEmpty(maNhomSize) && !string.IsNullOrEmpty(trimmedMaSize))
                            {
                                string pair = $"{maNhomSize}-{trimmedMaSize}";
                                sizePairsInRow.Add(pair);
                                Console.WriteLine($"  + Thêm: '{pair}'");
                            }
                        }
                    }
                }

                Console.WriteLine($"\nTổng số cặp tìm được: {sizePairsInRow.Count}");

                // Tìm cặp thiếu
                var missingPairs = allSizePairs.Except(sizePairsInRow).ToList();
                if (missingPairs.Any())
                {
                    Console.WriteLine("Cặp MaNhomSize-MaSize thiếu:");
                    foreach (var missing in missingPairs)
                    {
                        Console.WriteLine($"  - '{missing}'");
                    }
                }

                // Tìm cặp thừa
                var extraPairs = sizePairsInRow.Except(allSizePairs).ToList();
                if (extraPairs.Any())
                {
                    Console.WriteLine("Cặp MaNhomSize-MaSize thừa (không có trong tblAllSize):");
                    foreach (var extra in extraPairs)
                    {
                        Console.WriteLine($"  - '{extra}'");
                    }
                }

                // So sánh với allSizePairs
                bool isAllSize = allSizePairs.SetEquals(sizePairsInRow);
                Console.WriteLine($"\nKết quả: IsAllSize = {isAllSize}");

                if (isAllSize)
                {
                    row["Size"] = "All Size";
                    Console.WriteLine("=> Đã cập nhật Size = 'All Size'");
                }

                Console.WriteLine("===============================\n");
            }
        }
    }
}