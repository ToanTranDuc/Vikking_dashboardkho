
USE [PMS_QLDH_VIKING_2025];
GO

-- =======================================================
IF OBJECT_ID('dbo.usp_DashboardKhoDesktop', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_DashboardKhoDesktop;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.usp_DashboardKhoDesktop
    @Action VARCHAR(100),
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL,
    @Itemcode NVARCHAR(200) = NULL,
    @IsNhieuNhat INT = NULL,
    @LoaiNPL INT = NULL,
    @Ngay DATETIME = NULL,
    @Loai NVARCHAR(500) = NULL,
    @MaNPL NVARCHAR(50) = 'all',
    @SoLoID NVARCHAR(30) = 'all',
    @MaHang NVARCHAR(200) = 'all',
    @MaKH NVARCHAR(200) = 'all',
    @KhoLoi NVARCHAR(200) = '0',
    @Nhom NVARCHAR(200) = 'all',
    @IsNPL INT = 2
AS
BEGIN
    SET NOCOUNT ON;

        -- [OPTIMIZED] Common Temp Tables
    IF OBJECT_ID('tempdb..#TempXuatChuaThuHoi') IS NOT NULL DROP TABLE #TempXuatChuaThuHoi;
    CREATE TABLE #TempXuatChuaThuHoi (BarCode NVARCHAR(500));
    CREATE NONCLUSTERED INDEX IX_TempXCTH_BarCode ON #TempXuatChuaThuHoi(BarCode);

    IF OBJECT_ID('tempdb..#TempSoanHang') IS NOT NULL DROP TABLE #TempSoanHang;
    CREATE TABLE #TempSoanHang (BarCode NVARCHAR(200));
    CREATE NONCLUSTERED INDEX IX_TempSH_BarCode ON #TempSoanHang(BarCode);

        -- Tối ưu: Bật công tắc an toàn, CHỈ TẢI Temp Tables cho đúng các Action cần dùng
    IF @Action IN (
        'GetOverallCapacity', 'GetDistinctMaterialCount', 'GetCustomers',
        'GetKiemKe', 'GetTop5', 'GetRackSlotDetail', 'GetThanhGiaHangTon',
        'GetTop5KhachHangTonKho', 'GetKhachHangTonKhoChiTiet', 'GetGiaTriTonKhoTheoNhom',
        'GetGiaTriNhomChiTiet', 'GetKiemKeChiTiet', 'GetTonKhoTheoKy', 'GetAllMaterialsInStock'
    )
    BEGIN
        INSERT INTO #TempXuatChuaThuHoi (BarCode)
        SELECT DISTINCT CAST(BarCode AS NVARCHAR(500))
        FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #TempSoanHang(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';
        END
    END



    DECLARE @CapNPL FLOAT = 0,
            @CapPL FLOAT = 0,
            @UsedNPL FLOAT = 0,
            @UsedPL FLOAT = 0,
            @TotalVatTuNPL FLOAT = 0,
            @TotalVatTuPL FLOAT = 0;

    DECLARE @TotalSKUInWarehouse FLOAT;

    DECLARE @MaVTID NVARCHAR(100) = NULL,
            @TenVT NVARCHAR(500) = N'',
            @Sql1 NVARCHAR(MAX) = NULL,
            @Sql2 NVARCHAR(MAX) = NULL,
            @Pattern NVARCHAR(120) = NULL,
            @TonRows INT = 0,
            @TonSL DECIMAL(20,2) = 0,
            @NhapRows INT = 0,
            @NhapSL DECIMAL(20,2) = 0,
            @XuatRows INT = 0,
            @XuatSL DECIMAL(20,2) = 0,
            @KKRows INT = 0,
            @KKSL DECIMAL(20,2) = 0,
            @DKRows INT = 0;

    DECLARE @StartDate12T DATE;

    DECLARE @N_Weekly INT = 12,
            @Today_Weekly DATE,
            @StartDateWeekly DATE;

    DECLARE @ThanhGia DECIMAL(20,2) = 0,
            @TongMa INT = 0,
            @SoMaCoGia INT = 0,
            @SoMaKhongGia INT = 0,
            @TongSL DECIMAL(20,2) = 0;

    DECLARE @HasV2R INT = 0;

    DECLARE @HasV2D INT = 0;

    DECLARE @StartDateCal DATE,
            @EndDateCal DATE,
            @TotalDaysCal INT,
            @EndPlus1Cal DATE;

    DECLARE @StartDateRange DATE,
            @EndDateRange DATE,
            @TotalDaysRange INT;

    DECLARE @MaxDate DATE,
            @ThisMonth DATE,
            @LastMonth DATE,
            @NextMonth DATE;

    -- ===================================================================
    -- [GetOverallCapacity]
    -- Lấy dung lượng lưu trữ tổng quan của kho, bao gồm dung lượng kệ NPL, PL.
    -- Tính toán tổng CBM khả dụng, đã dùng và tính % lấp đầy.
    -- ===================================================================
    IF @Action = 'GetOverallCapacity'
    BEGIN
        ;WITH tempCBMKe_OC AS (
            SELECT t1.KeID, ROUND(SUM(t2.Dai * t2.Cao * t2.Rong), 4) AS TongCBMTrongKe, t1.TenKe, t1.Module
            FROM ERP_KeNPL t1
            LEFT JOIN ERP_ONPL t2 ON t1.KeID = t2.KeID
            WHERE t1.Module <> 3
            GROUP BY t1.KeID, t1.TenKe, t1.Module
        ),
        tempCBMO_OC AS (
            SELECT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM),4) AS TongCBMTrongO, COUNT(*) AS SLVatTu
            FROM ERP_VatTuCBM t1
            LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
            WHERE t1.MaONPL IS NOT NULL
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode = t1.Barcode)
              AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL c WHERE c.BarCode = t1.Barcode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang s WHERE s.BarCode = t1.Barcode)
            GROUP BY t1.MaONPL, t2.KeID
        ),
        tempBaseResult_OC AS (
            SELECT t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module,
                ISNULL(SUM(t2.TongCBMTrongO),0) AS TongCBMSuDungTrongKe,
                ISNULL(t3.TongCBMTrongKe,0) AS TongCBMTrongKe,
                SUM(ISNULL(t2.SLVatTu,0)) AS SLVatTu
            FROM ERP_KeNPL t1
            LEFT JOIN tempCBMO_OC t2 ON t1.KeID = t2.KeID
            LEFT JOIN tempCBMKe_OC t3 ON t1.KeID = t3.KeID
            LEFT JOIN ERP_DayNPL t4 ON t1.DayID = t4.DayID
            WHERE t1.Module <> 3
              AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE '%co%'
              AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%lỗi%'
              AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%n%'
            GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe
        )
        SELECT
            @CapNPL        = ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMTrongKe ELSE 0 END),0),
            @CapPL         = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMTrongKe ELSE 0 END),0),
            @UsedNPL       = ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMSuDungTrongKe ELSE 0 END),0),
            @UsedPL        = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMSuDungTrongKe ELSE 0 END),0),
            @TotalVatTuNPL = ISNULL(SUM(CASE WHEN Module=1 THEN SLVatTu ELSE 0 END),0),
            @TotalVatTuPL  = ISNULL(SUM(CASE WHEN Module=2 THEN SLVatTu ELSE 0 END),0)
        FROM tempBaseResult_OC;

        SELECT
            (@CapNPL+@CapPL) AS TotalCapacity, @CapNPL AS CapacityNPL, @CapPL AS CapacityPL,
            @UsedNPL AS UsedNPL, @UsedPL AS UsedPL,
            @TotalVatTuNPL AS TotalVatTuNPL, @TotalVatTuPL AS TotalVatTuPL,
            (@TotalVatTuNPL+@TotalVatTuPL) AS TotalVatTu,
            CASE WHEN (@CapNPL+@CapPL)>(@UsedNPL+@UsedPL) THEN ((@CapNPL+@CapPL)-(@UsedNPL+@UsedPL)) ELSE 0 END AS TotalFreeCapacity,
            CASE WHEN @CapNPL>0 THEN ROUND((@UsedNPL/@CapNPL)*100,2) ELSE 0 END AS PercentNPL,
            CASE WHEN @CapPL>0  THEN ROUND((@UsedPL/@CapPL)*100,2)  ELSE 0 END AS PercentPL,
            CASE WHEN (@CapNPL>0 OR @CapPL>0)
                 THEN ROUND((CASE WHEN @CapNPL>0 THEN (@UsedNPL/@CapNPL)*100 ELSE 0 END)
                          + (CASE WHEN @CapPL>0  THEN (@UsedPL/@CapPL)*100  ELSE 0 END),2)
                 ELSE 0 END AS TotalPercent,
            CASE WHEN (@CapNPL>0 OR @CapPL>0)
                 THEN ROUND(100-((CASE WHEN @CapNPL>0 THEN (@UsedNPL/@CapNPL)*100 ELSE 0 END)
                               + (CASE WHEN @CapPL>0  THEN (@UsedPL/@CapPL)*100  ELSE 0 END)),2)
                 ELSE 100 END AS FreePercent;
    END

    -- ===================================================================
    -- [GetDistinctMaterialCount]
    -- Đếm số lượng mã vật tư (SKU) khác nhau đang tồn trong kho.
    -- ===================================================================
    ELSE IF @Action = 'GetDistinctMaterialCount'
    BEGIN
        IF COL_LENGTH('dbo.ERP_VatTuCBM','MaONPL') IS NULL
        BEGIN
            SELECT CAST(0 AS INT) AS SoMaVatTu;
            RETURN;
        END





        SELECT COUNT(DISTINCT ct.MaNPL) AS SoMaVatTu
        FROM dbo.ERP_VatTuCBM v
        INNER JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON ct.BarCode = v.Barcode
        WHERE v.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi t WHERE t.BarCode = v.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #TempSoanHang  t WHERE t.BarCode = v.Barcode);



    END

    -- ===================================================================
    -- [GetCustomers]
    -- Liệt kê danh sách các khách hàng có hàng tồn trong kho, cùng với
    -- số lượng vật tư và % không gian (SKU/CBM) chiếm dụng.
    -- ===================================================================
    ELSE IF @Action = 'GetCustomers'
    BEGIN




        ;WITH tempBarcodeCBM_Cust AS (
            SELECT MaNPL, SoLoID, SUM(CBM) AS CBM
            FROM ERP_VatTuCBM t1
            LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
            LEFT JOIN ERP_DayNPL t3 ON t2.DayID = t3.DayID
            WHERE t1.MaONPL IS NOT NULL
              AND t2.Module <> 3
              AND LOWER(t3.TenDay) NOT LIKE '%co%'
              AND LOWER(t3.TenDay) NOT LIKE N'%lỗi%'
              AND LOWER(t3.TenDay) NOT LIKE N'%n%'
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode = t1.Barcode)
              AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang t7 WHERE t1.Barcode = t7.BarCode)
            GROUP BY MaNPL, SoLoID
        ),
        TotalSKU AS (
            SELECT CAST(COUNT(DISTINCT CASE WHEN ISNULL(MaNPL,'') <> '' THEN CONCAT(MaNPL,'|',SoLoID) END) AS FLOAT) AS TotalSKUInWarehouse
            FROM tempBarcodeCBM_Cust
        )
        SELECT
            ISNULL(t3.MaKH, '') AS MaKH,
            ISNULL(t5.TenKH, '') AS TenKH,
            COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) AS SLVatTu,
            SUM(CBM) AS CBMSDTrongKho,
            CASE WHEN MAX(ts.TotalSKUInWarehouse) > 0
                THEN ROUND(COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) / MAX(ts.TotalSKUInWarehouse) * 100, 2)
                ELSE 0
            END AS PhanTramSKU
        FROM tempBarcodeCBM_Cust t1
        CROSS JOIN TotalSKU ts
        LEFT JOIN (SELECT DISTINCT MaNPL, SoLoID FROM ERP_ChiTietNhapKhoNPL) t2
            ON t1.MaNPL = t2.MaNPL AND t1.SoLoID = t2.SoLoID
        LEFT JOIN ERP_NhapKhoNPL t3 ON t2.SoLoID = t3.SoLoID
        LEFT JOIN KhachHang t5 ON t3.MaKH = t5.MaKH
        GROUP BY ISNULL(t3.MaKH, ''), ISNULL(t5.TenKH, '')
        ORDER BY SLVatTu DESC;
    END

    ELSE IF @Action = 'GetRacks'
    BEGIN
        ;WITH tempCBMKe_Racks AS (
            SELECT t1.KeID, ROUND(SUM(t2.Dai*t2.Cao*t2.Rong),4) AS TongCBMTrongKe, t1.TenKe, t1.Module
            FROM ERP_KeNPL t1 LEFT JOIN ERP_ONPL t2 ON t1.KeID=t2.KeID
            WHERE t1.Module<>3 GROUP BY t1.KeID, t1.TenKe, t1.Module
        ),
        tempCBMO_Racks AS (
            SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM),4) AS TongCBMTrongO, COUNT(*) AS SLVatTu
            FROM ERP_VatTuCBM t1 LEFT JOIN ERP_ONPL t2 ON t1.MaONPL=t2.TenO
            WHERE t1.MaONPL IS NOT NULL
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode=t1.Barcode)
              AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL c WHERE c.BarCode=t1.Barcode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang s WHERE s.BarCode=t1.Barcode)
            GROUP BY t1.MaONPL, t2.KeID
        )
        SELECT t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module,
            ISNULL(SUM(t2.TongCBMTrongO),0) AS TongCBMSuDungTrongKe,
            ISNULL(t3.TongCBMTrongKe,0) AS TongCBMTrongKe,
            SUM(ISNULL(t2.SLVatTu,0)) AS SLVatTu
        FROM ERP_KeNPL t1
        LEFT JOIN tempCBMO_Racks t2 ON t1.KeID=t2.KeID
        LEFT JOIN tempCBMKe_Racks t3 ON t1.KeID=t3.KeID
        LEFT JOIN ERP_DayNPL t4 ON t1.DayID=t4.DayID
        WHERE t1.Module<>3
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%lỗi%'
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%n%'
        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe
        ORDER BY t1.Module, t1.TenKe;
    END

    -- ===================================================================
    -- [GetChuanBiVe]
    -- Lấy danh sách hàng hóa/vật tư chuẩn bị về (đã có PO hoặc kế hoạch nhập).
    -- Dự kiến thời gian về dựa vào NgayNKDuKien.
    -- ===================================================================
    ELSE IF @Action = 'GetChuanBiVe'
    BEGIN

        ;WITH NhapKho AS
        (
            SELECT
                nk.SoLoID,
                nk.POMua,
                MAX(nk.SoLo) AS SoLo,
                MAX(ISNULL(nk.MaKH, '')) AS MaKH,
                MAX(ISNULL(nk.MaHang, '')) AS MaHang,
                MIN(CAST(nk.NgayNKDuKien AS date)) AS NgayNKDuKien
            FROM dbo.ERP_NhapKhoNPL nk
            WHERE nk.NgayNKDuKien IS NOT NULL
              AND (
                  (ISNULL(@Itemcode, '') = '' AND CAST(nk.NgayNKDuKien AS date) BETWEEN CAST(@TuNgay AS DATE) AND CAST(@DenNgay AS DATE))
                  OR (ISNULL(@Itemcode, '') <> '' AND (nk.POMua = @Itemcode OR nk.SoLoID = @Itemcode))
              )
            GROUP BY nk.SoLoID, nk.POMua
        )
        SELECT
            MIN(nk.SoLoID) AS SoLoID,
            MAX(nk.SoLo) AS SoLo,
            nk.POMua AS PO,
            nk.POMua,
            ISNULL((SELECT TOP 1 dh.MaDH FROM dbo.DonHangTong dh WHERE dh.MaHang = MAX(nk.MaHang) AND dh.MaKH = MAX(nk.MaKH)), '') AS MaDH,
            MAX(nk.MaHang) AS MaHang,
            ISNULL((SELECT TOP 1 hh.TenHang FROM dbo.HangHoa hh WHERE hh.MaHang = MAX(nk.MaHang) AND hh.MaKH = MAX(nk.MaKH)), '') AS TenHang,
            MIN(nk.NgayNKDuKien) AS NgayNKDuKien,
            MIN(nk.NgayNKDuKien) AS NgayNhapKho_Update,
            CAST(N'' AS NVARCHAR(200)) AS MaNPL,
            SUM(ISNULL(ct.SLTong, 0)) AS SoLuongSP,
            CAST(0 AS decimal(18, 2)) AS SoLuongThung,
            MAX(ISNULL(kh.TenKH, '')) AS TenKH
        FROM NhapKho nk
        LEFT JOIN (
            SELECT SoLoID, POMua, SUM(SLTong) AS SLTong
            FROM (
                SELECT SoLoID, POMua, MaNPL, MAX(ISNULL(SLTong, 0)) AS SLTong
                FROM dbo.ERP_ChiTietNhapKhoNPL
                GROUP BY SoLoID, POMua, MaNPL
            ) AS tmp
            GROUP BY SoLoID, POMua
        ) ct ON ct.SoLoID = nk.SoLoID AND ct.POMua = nk.POMua
        LEFT JOIN KhachHang kh ON kh.MaKH = nk.MaKH
        GROUP BY nk.POMua
        ORDER BY MIN(nk.NgayNKDuKien), nk.POMua;
    END

    -- ===================================================================
    -- [GetChuanBiXuat]
    -- Lấy danh sách lệnh sản xuất chuẩn bị xuất kho (đã có yêu cầu nhưng chưa xuất).
    -- Lấy thêm số lượng yêu cầu từ CanDoiDinhMucNPL.
    -- ===================================================================
    ELSE IF @Action = 'GetChuanBiXuat'
    BEGIN
        SELECT
            cs.MaLenhSanXuat AS MaLenhSanXuat, -- Mã lệnh SX
            cs.MaLenh AS MaLenh,        -- Mã lệnh thu gọn
            cs.MaDVSX AS MaDVSX,        -- Mã đơn vị sản xuất
            kh.TenKH AS KhachHang,     -- Tên khách hàng (JS hiển thị trên thẻ giao việc)
            hh.TenHang AS TenHang,       -- Tên hàng hóa sản xuất
            t1.SLYeuCau  AS SoLuongYeuCau, -- Dạng số. Tổng số lượng yêu cầu (đã gán 0 để ẩn số liệu thật).
            MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END)                       AS KHCat,     -- Ngày KH Cắt (Datetime).
            DATEADD(day, 7, MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END))      AS DuKienCat  -- Ngày dự kiến (Datetime).
        FROM dbo.CanDoiDonViSanXuat cs
        LEFT JOIN dbo.DonHangTong dh ON dh.MaDH = cs.MaDH
        LEFT JOIN dbo.HangHoa     hh ON hh.MaHang = dh.MaHang AND hh.MaKH = dh.MaKH
        LEFT JOIN dbo.KhachHang   kh ON kh.MaKH   = dh.MaKH
        LEFT JOIN dbo.WIP_DonHang_Chuyen wc
            ON wc.LenhSX = TRY_CONVERT(INT, REPLACE(ISNULL(cs.MaLenh, ''), 'SX_', ''))
        LEFT JOIN dbo.WIP_DonHang_Chuyen_TechProgress tp ON tp.WIPId = wc.WIPId
        OUTER APPLY (
            SELECT SUM(CapPhat) AS SLYeuCau
            FROM dbo.CanDoiDinhMucNPL c
            WHERE c.MaLenhSanXuat = cs.MaLenhSanXuat
        ) t1
        WHERE NOT EXISTS (SELECT 1 FROM dbo.PhieuXuatHang ph WHERE ph.MaLenhSX = cs.MaLenhSanXuat)
        GROUP BY cs.MaLenhSanXuat, cs.MaLenh, cs.MaDVSX, kh.TenKH, hh.TenHang, t1.SLYeuCau
        ORDER BY KHCat DESC, TRY_CAST(cs.MaLenh AS int) DESC;
    END

    ELSE IF @Action = 'GlobalSearchByItemcode'
    BEGIN
        SET @MaVTID = NULL;
        SET @TenVT = N'';

        IF OBJECT_ID('dbo.ERP_VatTuTV') IS NOT NULL
        BEGIN
            IF COL_LENGTH('dbo.ERP_VatTuTV', 'TenVT') IS NOT NULL
            BEGIN
                SET @Sql1 = N'
                    SELECT TOP 1 @MaVTID = vt.MaVTID, @TenVT = ISNULL(vt.ChiTiet, '''')
                    FROM dbo.ERP_VatTuTV vt
                    WHERE vt.MaVT LIKE ''%'' + LTRIM(RTRIM(@Itemcode)) + ''%''
                       OR vt.ChiTiet LIKE ''%'' + LTRIM(RTRIM(@Itemcode)) + ''%'';';
                EXEC sp_executesql @Sql1, N'@Itemcode NVARCHAR(200), @MaVTID NVARCHAR(100) OUTPUT, @TenVT NVARCHAR(500) OUTPUT',
                    @Itemcode = @Itemcode, @MaVTID = @MaVTID OUTPUT, @TenVT = @TenVT OUTPUT;
            END
            ELSE
            BEGIN
                SET @Sql2 = N'
                    SELECT TOP 1 @MaVTID = vt.MaVTID, @TenVT = ISNULL(vt.ChiTiet, '''')
                    FROM dbo.ERP_VatTuTV vt
                    WHERE vt.MaVT LIKE ''%'' + LTRIM(RTRIM(@Itemcode)) + ''%''
                       OR vt.ChiTiet LIKE ''%'' + LTRIM(RTRIM(@Itemcode)) + ''%'';';
                EXEC sp_executesql @Sql2, N'@Itemcode NVARCHAR(200), @MaVTID NVARCHAR(100) OUTPUT, @TenVT NVARCHAR(500) OUTPUT',
                    @Itemcode = @Itemcode, @MaVTID = @MaVTID OUTPUT, @TenVT = @TenVT OUTPUT;
            END
        END

        SET @Pattern = N'%@' + ISNULL(@MaVTID, N'') + N'@%@%';

        SET @TonRows = 0; SET @TonSL = 0;
        IF @MaVTID IS NOT NULL
        BEGIN
            SELECT
                @TonRows = COUNT(DISTINCT ct.BarCode),
                @TonSL   = ISNULL(SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)), 0)
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode
            WHERE v.MaONPL IS NOT NULL
              AND ct.MaNPL LIKE @Pattern;
        END

        SET @NhapRows = 0; SET @NhapSL = 0;
        IF @MaVTID IS NOT NULL
        BEGIN
            SELECT
                @NhapRows = COUNT(*),
                @NhapSL   = ISNULL(SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)), 0)
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            WHERE ct.MaNPL LIKE @Pattern
              AND ct.NgayNhapKho IS NOT NULL
              AND CAST(ct.NgayNhapKho AS DATE) >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE));
        END

        SET @XuatRows = 0; SET @XuatSL = 0;
        IF @MaVTID IS NOT NULL
        BEGIN
            SELECT
                @XuatRows = COUNT(*),
                @XuatSL   = ISNULL(SUM(ISNULL(xh.SLNhap, 0)), 0)
            FROM dbo.PhieuXuatHang xh
            WHERE xh.ModuleXH = 1
              AND xh.MaNPL LIKE @Pattern
              AND xh.NgayXuatHang IS NOT NULL
              AND CAST(xh.NgayXuatHang AS DATE) >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE));
        END

        SET @KKRows = 0; SET @KKSL = 0;
        IF @MaVTID IS NOT NULL
        BEGIN
            IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPLV2') IS NOT NULL
            BEGIN
                SELECT
                    @KKRows = COUNT(*),
                    @KKSL   = ISNULL(SUM(ISNULL(SLKiemKe, 0)), 0)
                FROM dbo.ERPPhieuKiemKe_NPLV2
                WHERE MaNPL LIKE @Pattern
                  AND DateKiemKe IS NOT NULL
                  AND CAST(DateKiemKe AS DATE) >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE));
            END
            ELSE IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPL') IS NOT NULL
            BEGIN
                SELECT
                    @KKRows = COUNT(*),
                    @KKSL   = ISNULL(SUM(ISNULL(SLKiemKe, 0)), 0)
                FROM dbo.ERPPhieuKiemKe_NPL
                WHERE MaNPL LIKE @Pattern
                  AND DateKiemKe IS NOT NULL
                  AND CAST(DateKiemKe AS DATE) >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE));
            END
        END

        SET @DKRows = 0;
        IF @MaVTID IS NOT NULL
        BEGIN
            SELECT @DKRows = COUNT(DISTINCT nk.SoLoID)
            FROM dbo.ERP_NhapKhoNPL nk
            WHERE nk.NgayNKDuKien IS NOT NULL
              AND CAST(nk.NgayNKDuKien AS DATE) BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, 60, CAST(GETDATE() AS DATE))
              AND EXISTS (
                  SELECT 1 FROM dbo.ERP_ChiTietNhapKhoNPL ct
                  WHERE ct.SoLoID = nk.SoLoID AND ct.MaNPL LIKE @Pattern);
        END

        SELECT
            @Itemcode    AS Itemcode,
            @MaVTID      AS MaVTID,
            @TenVT       AS TenVT,
            @TonRows     AS TonRows,    @TonSL    AS TonSL,
            @NhapRows    AS NhapRows,   @NhapSL   AS NhapSL,
            @XuatRows    AS XuatRows,   @XuatSL   AS XuatSL,
            @KKRows      AS KKRows,     @KKSL     AS KKSL,
            @DKRows      AS DKRows;
    END

    -- ===================================================================
    -- [GetDangXuat]
    -- Lấy tình trạng của các phiếu đang xuất kho (tiến độ xuất so với yêu cầu).
    -- Tính % đã xuất = SL DaXuat / SL YeuCau.
    -- ===================================================================
    ELSE IF @Action = 'GetDangXuat'
    BEGIN
        ;WITH YeuCau_DX AS (
            SELECT
                dk.MaLenhSX,
                dk.PhieuDK,
                SUM(ISNULL(dk.SLDK, 0))      AS SLYeuCau
            FROM dbo.ERP_PhieuDangKyXuatVT dk
            WHERE dk.IsNPL = 1
              AND ISNULL(dk.SLDK, 0) > 0
            GROUP BY dk.MaLenhSX, dk.PhieuDK
        ),
        DaXuat_DX AS (
            SELECT
                ph.PhieuYC,
                ph.MaLenhSX,
                MAX(ph.MaLenh)              AS MaLenh,
                MAX(ph.MaGop)               AS MaGop,
                MAX(ph.NgayXuatHang)        AS NgayXuatHang,
                SUM(ISNULL(ph.SLNhap, 0))   AS SLDaXuat
            FROM dbo.PhieuXuatHang ph
            WHERE ph.Moudule = 0 AND ph.NPL = 1
              AND ph.NgayXuatHang IS NOT NULL
              AND ph.NgayXuatHang >= CAST(@TuNgay AS DATE) AND ph.NgayXuatHang <= CAST(@DenNgay AS DATE)
            GROUP BY ph.PhieuYC, ph.MaLenhSX
        ),
        KhTH_DX AS (
            SELECT DISTINCT gd.MaGop,
                   ISNULL(hh.TenHang, '') AS TenHang,
                   ISNULL(kh.TenKH, '')   AS TenKH
            FROM dbo.GopDonHang gd
            INNER JOIN dbo.DonHangTong dh ON gd.MaDH = dh.MaDH
            LEFT JOIN dbo.HangHoa hh ON hh.MaHang = dh.MaHang AND hh.MaKH = dh.MaKH
            LEFT JOIN dbo.KhachHang kh ON kh.MaKH = dh.MaKH
        )
        SELECT
            x.MaLenh                                                    AS MaLenh,
            x.MaLenhSX                                                  AS MaLenhSX,
            x.PhieuYC                                                   AS PhieuDK,
            x.MaGop,
            ISNULL(k.TenHang, '')                                       AS TenHang,
            ISNULL(k.TenKH,   '')                                       AS TenKH,
            x.NgayXuatHang,
            ROUND(ISNULL(y.SLYeuCau, 0), 4)                             AS SoLuongYeuCau,
            ROUND(x.SLDaXuat, 4)                                        AS SLXuat,
            CASE WHEN ISNULL(y.SLYeuCau, 0) > 0
                 THEN ROUND(x.SLDaXuat / y.SLYeuCau * 100, 2)
                 ELSE 0 END                                             AS PctDaXuat
        FROM DaXuat_DX x
        LEFT JOIN YeuCau_DX y ON y.PhieuDK = x.PhieuYC
        LEFT JOIN KhTH_DX   k ON k.MaGop   = x.MaGop
        ORDER BY x.NgayXuatHang DESC;
    END

    -- ===================================================================
    -- [GetFlowTrend12T]
    -- Thống kê Nhập - Xuất - Tồn - Thu hồi theo 12 tháng gần nhất (Luồng kho).
    -- Dùng cho biểu đồ cột/đường dài hạn.
    -- ===================================================================
    ELSE IF @Action = 'GetFlowTrend12T'
    BEGIN
        SET @StartDate12T = DATEADD(MONTH, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1));

        ;WITH Months AS (
            SELECT TOP 12
                YEAR(DATEADD(MONTH, number, @StartDate12T))  AS Nam,
                MONTH(DATEADD(MONTH, number, @StartDate12T)) AS Thang
            FROM master..spt_values
            WHERE type = 'P' AND number BETWEEN 0 AND 11
        ),
        TonDauKy AS (
            SELECT SUM(SL) AS TonDau
            FROM (
                SELECT SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS SL FROM ERP_ChiTietNhapKhoNPL WHERE TRY_CONVERT(DATE, NgayNhapKho) < @StartDate12T
                UNION ALL
                SELECT -SUM(ISNULL(SLNhap, 0)) FROM PhieuXuatHang WHERE ModuleXH = 1 AND TRY_CONVERT(DATE, NgayXuatHang) < @StartDate12T
                UNION ALL
                SELECT SUM(ISNULL(ThuHoi, 0)) FROM PhieuThuHoiNPL WHERE TRY_CONVERT(DATE, NgayTH) < @StartDate12T
                UNION ALL
                SELECT -SUM(ISNULL(SLKiemKeBanDau, 0) - ISNULL(SLKiemKeEdit, ISNULL(SLKiemKe, 0)))
                FROM ERPPhieuKiemKe_NPL
                WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL
                  AND TRY_CONVERT(DATE, DateDuyetKK) < @StartDate12T
            ) t
        ),
        NhapTheoThang AS (
            SELECT YEAR(TRY_CONVERT(DATE, NgayNhapKho)) AS Nam, MONTH(TRY_CONVERT(DATE, NgayNhapKho)) AS Thang,
                   SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS TotalIn
            FROM ERP_ChiTietNhapKhoNPL WHERE TRY_CONVERT(DATE, NgayNhapKho) >= @StartDate12T
            GROUP BY YEAR(TRY_CONVERT(DATE, NgayNhapKho)), MONTH(TRY_CONVERT(DATE, NgayNhapKho))
        ),
        XuatTheoThang AS (
            SELECT YEAR(TRY_CONVERT(DATE, NgayXuatHang)) AS Nam, MONTH(TRY_CONVERT(DATE, NgayXuatHang)) AS Thang,
                   SUM(ISNULL(SLNhap, 0)) AS TotalOut
            FROM PhieuXuatHang WHERE ModuleXH = 1 AND TRY_CONVERT(DATE, NgayXuatHang) >= @StartDate12T
            GROUP BY YEAR(TRY_CONVERT(DATE, NgayXuatHang)), MONTH(TRY_CONVERT(DATE, NgayXuatHang))
        ),
        ThuHoiTheoThang AS (
            SELECT YEAR(TRY_CONVERT(DATE, NgayTH)) AS Nam, MONTH(TRY_CONVERT(DATE, NgayTH)) AS Thang,
                   SUM(ISNULL(ThuHoi, 0)) AS TotalTH
            FROM PhieuThuHoiNPL WHERE TRY_CONVERT(DATE, NgayTH) >= @StartDate12T
            GROUP BY YEAR(TRY_CONVERT(DATE, NgayTH)), MONTH(TRY_CONVERT(DATE, NgayTH))
        ),
        ChenhLechKKTheoThang AS (
            SELECT YEAR(TRY_CONVERT(DATE, DateDuyetKK))  AS Nam,
                   MONTH(TRY_CONVERT(DATE, DateDuyetKK)) AS Thang,
                   SUM(ISNULL(SLKiemKeBanDau, 0) - ISNULL(SLKiemKeEdit, ISNULL(SLKiemKe, 0))) AS TotalChenhLech
            FROM ERPPhieuKiemKe_NPL
            WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL
              AND TRY_CONVERT(DATE, DateDuyetKK) >= @StartDate12T
            GROUP BY YEAR(TRY_CONVERT(DATE, DateDuyetKK)), MONTH(TRY_CONVERT(DATE, DateDuyetKK))
        ),
        Combined AS (
            SELECT m.Nam, m.Thang,
                ISNULL(n.TotalIn, 0) AS TotalIn, ISNULL(x.TotalOut, 0) AS TotalOut, ISNULL(th.TotalTH, 0) AS TotalTH,
                ISNULL(kk.TotalChenhLech, 0) AS TotalChenhLech
            FROM Months m
            LEFT JOIN NhapTheoThang       n  ON n.Nam  = m.Nam AND n.Thang  = m.Thang
            LEFT JOIN XuatTheoThang       x  ON x.Nam  = m.Nam AND x.Thang  = m.Thang
            LEFT JOIN ThuHoiTheoThang     th ON th.Nam = m.Nam AND th.Thang = m.Thang
            LEFT JOIN ChenhLechKKTheoThang kk ON kk.Nam = m.Nam AND kk.Thang = m.Thang
        )
        SELECT c.Nam, c.Thang, c.TotalIn, c.TotalOut,
            ISNULL(d.TonDau, 0) + SUM(c.TotalIn + c.TotalTH - c.TotalOut - c.TotalChenhLech) OVER (ORDER BY c.Nam, c.Thang ROWS UNBOUNDED PRECEDING) AS TotalStock
        FROM Combined c CROSS JOIN TonDauKy d
        ORDER BY c.Nam, c.Thang;
    END

    -- ===================================================================
    -- [GetFlowTrendWeekly]
    -- Thống kê Nhập - Xuất - Tồn theo 12 tuần gần nhất (Luồng kho).
    -- Tương tự 12T nhưng chi tiết theo từng tuần (ISO Week).
    -- ===================================================================
    ELSE IF @Action = 'GetFlowTrendWeekly'
    BEGIN
        SET @N_Weekly = 12;
        SET @Today_Weekly = CAST(GETDATE() AS DATE);
        SET @StartDateWeekly = DATEADD(WEEK, -(@N_Weekly - 1),
            DATEADD(DAY, 1 - (DATEPART(WEEKDAY, @Today_Weekly) + @@DATEFIRST - 2) % 7, @Today_Weekly));

        ;WITH Weeks AS (
            SELECT TOP (@N_Weekly)
                DATEADD(WEEK, number, @StartDateWeekly) AS WeekStart,
                YEAR(DATEADD(WEEK, number, @StartDateWeekly))  AS Nam,
                DATEPART(ISO_WEEK, DATEADD(WEEK, number, @StartDateWeekly)) AS Tuan
            FROM master..spt_values
            WHERE type = 'P' AND number BETWEEN 0 AND @N_Weekly - 1
        ),
        TonDauKy AS (
            SELECT ISNULL(SUM(SL), 0) AS TonDau FROM (
                SELECT SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS SL FROM ERP_ChiTietNhapKhoNPL
                    WHERE TRY_CONVERT(DATE, NgayNhapKho) < @StartDateWeekly
                UNION ALL
                SELECT -SUM(ISNULL(SLNhap, 0)) FROM PhieuXuatHang
                    WHERE ModuleXH = 1 AND TRY_CONVERT(DATE, NgayXuatHang) < @StartDateWeekly
                UNION ALL
                SELECT SUM(ISNULL(ThuHoi, 0)) FROM PhieuThuHoiNPL
                    WHERE TRY_CONVERT(DATE, NgayTH) < @StartDateWeekly
                UNION ALL
                SELECT -SUM(ISNULL(SLKiemKeBanDau, 0) - ISNULL(SLKiemKeEdit, ISNULL(SLKiemKe, 0)))
                FROM ERPPhieuKiemKe_NPL
                WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL
                  AND TRY_CONVERT(DATE, DateDuyetKK) < @StartDateWeekly
            ) t
        ),
        NhapTheoTuan AS (
            SELECT DATEPART(ISO_WEEK, TRY_CONVERT(DATE, NgayNhapKho)) AS Tuan,
                   YEAR(TRY_CONVERT(DATE, NgayNhapKho)) AS Nam,
                   SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS TotalIn
            FROM ERP_ChiTietNhapKhoNPL
            WHERE TRY_CONVERT(DATE, NgayNhapKho) >= @StartDateWeekly
            GROUP BY YEAR(TRY_CONVERT(DATE, NgayNhapKho)), DATEPART(ISO_WEEK, TRY_CONVERT(DATE, NgayNhapKho))
        ),
        XuatTheoTuan AS (
            SELECT DATEPART(ISO_WEEK, TRY_CONVERT(DATE, NgayXuatHang)) AS Tuan,
                   YEAR(TRY_CONVERT(DATE, NgayXuatHang)) AS Nam,
                   SUM(ISNULL(SLNhap, 0)) AS TotalOut
            FROM PhieuXuatHang
            WHERE ModuleXH = 1 AND TRY_CONVERT(DATE, NgayXuatHang) >= @StartDateWeekly
            GROUP BY YEAR(TRY_CONVERT(DATE, NgayXuatHang)), DATEPART(ISO_WEEK, TRY_CONVERT(DATE, NgayXuatHang))
        ),
        ThuHoiTheoTuan AS (
            SELECT DATEPART(ISO_WEEK, TRY_CONVERT(DATE, NgayTH)) AS Tuan,
                   YEAR(TRY_CONVERT(DATE, NgayTH)) AS Nam,
                   SUM(ISNULL(ThuHoi, 0)) AS TotalTH
            FROM PhieuThuHoiNPL
            WHERE TRY_CONVERT(DATE, NgayTH) >= @StartDateWeekly
            GROUP BY YEAR(TRY_CONVERT(DATE, NgayTH)), DATEPART(ISO_WEEK, TRY_CONVERT(DATE, NgayTH))
        ),
        ChenhLechKKTheoTuan AS (
            SELECT DATEPART(ISO_WEEK, TRY_CONVERT(DATE, DateDuyetKK)) AS Tuan,
                   YEAR(TRY_CONVERT(DATE, DateDuyetKK))               AS Nam,
                   SUM(ISNULL(SLKiemKeBanDau, 0) - ISNULL(SLKiemKeEdit, ISNULL(SLKiemKe, 0))) AS TotalChenhLech
            FROM ERPPhieuKiemKe_NPL
            WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL
              AND TRY_CONVERT(DATE, DateDuyetKK) >= @StartDateWeekly
            GROUP BY YEAR(TRY_CONVERT(DATE, DateDuyetKK)), DATEPART(ISO_WEEK, TRY_CONVERT(DATE, DateDuyetKK))
        ),
        Combined AS (
            SELECT w.Nam, w.Tuan,
                ISNULL(n.TotalIn,  0) AS TotalIn,
                ISNULL(x.TotalOut, 0) AS TotalOut,
                ISNULL(th.TotalTH, 0) AS TotalTH,
                ISNULL(kk.TotalChenhLech, 0) AS TotalChenhLech
            FROM Weeks w
            LEFT JOIN NhapTheoTuan        n  ON n.Nam  = w.Nam AND n.Tuan  = w.Tuan
            LEFT JOIN XuatTheoTuan        x  ON x.Nam  = w.Nam AND x.Tuan  = w.Tuan
            LEFT JOIN ThuHoiTheoTuan      th ON th.Nam = w.Nam AND th.Tuan = w.Tuan
            LEFT JOIN ChenhLechKKTheoTuan kk ON kk.Nam = w.Nam AND kk.Tuan = w.Tuan
        )
        SELECT c.Nam, c.Tuan, c.TotalIn, c.TotalOut,
            (SELECT TonDau FROM TonDauKy) +
            SUM(c.TotalIn + c.TotalTH - c.TotalOut - c.TotalChenhLech) OVER (ORDER BY c.Nam, c.Tuan ROWS UNBOUNDED PRECEDING) AS TotalStock
        FROM Combined c
        ORDER BY c.Nam, c.Tuan;
    END

    -- ===================================================================
    -- [GetAllMaterialsInStock]
    -- Lấy tất cả mã vật tư hiện có tồn kho (Module 1, 2) phục vụ tìm kiếm.
    -- ===================================================================
    ELSE IF @Action = 'GetAllMaterialsInStock'
    BEGIN
        IF COL_LENGTH('dbo.ERP_VatTuCBM','MaONPL') IS NULL
           OR COL_LENGTH('dbo.ERP_ONPL','TenO') IS NULL
        BEGIN
            SELECT CAST(NULL AS NVARCHAR(200)) AS MaVT,
                   CAST(NULL AS NVARCHAR(500)) AS TenVT,
                   CAST(0    AS DECIMAL(18,4)) AS TonKho,
                   CAST(0    AS INT)           AS NPL
            WHERE 1 = 0;
            RETURN;
        END





        IF OBJECT_ID('tempdb..#MatFinal_AMIS') IS NOT NULL DROP TABLE #MatFinal_AMIS;
        SELECT
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVT,
            CAST(ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS NVARCHAR(500)) AS TenVT,
            CAST('' AS NVARCHAR(200))         AS Mau,
            CAST('' AS NVARCHAR(200))         AS KhoVai,
            CAST('' AS NVARCHAR(500))         AS ChiTiet,
            CAST('' AS NVARCHAR(50))          AS TenDVVT,
            r.TonKho,
            r.SoBarCode,
            CASE WHEN r.Module = 1 THEN N'NL' WHEN r.Module = 2 THEN N'PL' ELSE N'' END AS LoaiKho,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID
        INTO #MatFinal_AMIS
        FROM (
            SELECT
                ct.MaNPL,
                o.Module,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho,
                COUNT(*)                              AS SoBarCode
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode
            INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL
            WHERE v.MaONPL IS NOT NULL
              AND o.Module IN (1, 2)
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi t WHERE t.BarCode = v.Barcode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang   t WHERE t.BarCode = v.Barcode)
            GROUP BY ct.MaNPL, o.Module
            HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) > 0
        ) r;

        IF OBJECT_ID('dbo.ERP_MauVTTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f SET Mau = ISNULL(m.MauVT, '''')
                    FROM #MatFinal_AMIS f
                    LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID = f.MauVTID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f SET KhoVai = ISNULL(k.KhoVai, '''')
                    FROM #MatFinal_AMIS f
                    LEFT JOIN dbo.ERP_KhoVai k ON k.KhoVaiID = f.KhoVaiID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_VatTuTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f
                    SET MaVT    = ISNULL(vt.MaVT,    f.MaVT),
                        TenVT   = ISNULL(vt.ChiTiet, f.TenVT),
                        ChiTiet = ISNULL(vt.ChiTiet, '''')
                    FROM #MatFinal_AMIS f
                    LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = f.MaVT;';
            END TRY BEGIN CATCH END CATCH

            IF OBJECT_ID('dbo.ERP_DonViVT') IS NOT NULL
               AND OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
            BEGIN
                BEGIN TRY
                    EXEC sp_executesql N'
                        UPDATE f
                        SET TenDVVT = ISNULL(d.TenDVVT, '''')
                        FROM #MatFinal_AMIS f
                        LEFT JOIN dbo.ERP_KhoVai k ON k.KhoVaiID = f.KhoVaiID
                        LEFT JOIN dbo.ERP_DonViVT d ON d.MaDVVT = k.MaDVVT;';
                END TRY BEGIN CATCH END CATCH
            END
        END

        SELECT MaVT, TenVT, Mau, MauVTID AS MaMauVT, KhoVai, ChiTiet, TenDVVT, TonKho, SoBarCode, LoaiKho
        FROM #MatFinal_AMIS
        ORDER BY TonKho DESC;

        DROP TABLE #MatFinal_AMIS;


    END

    ELSE IF @Action = 'GetThanhGiaHangTon'
    BEGIN




        SET @ThanhGia = 0; SET @TongMa = 0; SET @SoMaCoGia = 0; SET @SoMaKhongGia = 0; SET @TongSL = 0;

        IF COL_LENGTH('dbo.ERP_ChiTietNhapKhoNPL', 'DonGia') IS NOT NULL
        BEGIN
            ;WITH RowsInStock AS (
                SELECT
                    ct.MaNPL,
                    ct.BarCode,
                    ISNULL(ct.SoLuongThucTeBanDau, 0)         AS SoLuong,
                    CAST(ISNULL(ct.DonGia, 0) AS DECIMAL(20,4)) AS DonGia
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode
                INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL
                WHERE v.MaONPL IS NOT NULL AND o.Module IN (1,2)
                  AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi t WHERE t.BarCode = v.Barcode)
                  AND NOT EXISTS (SELECT 1 FROM #TempSoanHang  t WHERE t.BarCode = v.Barcode)
            ),
            ByMa AS (
                SELECT MaNPL,
                       SUM(SoLuong) AS TongSL,
                       MAX(DonGia)  AS MaxDonGia,
                       SUM(SoLuong * DonGia) AS ThanhTien,
                       COUNT(DISTINCT BarCode) AS SoBC
                FROM RowsInStock
                GROUP BY MaNPL
            )
            SELECT
                @ThanhGia     = ISNULL(SUM(ThanhTien), 0),
                @TongMa       = COUNT(*),
                @SoMaCoGia    = SUM(CASE WHEN MaxDonGia > 0 THEN 1 ELSE 0 END),
                @SoMaKhongGia = SUM(CASE WHEN MaxDonGia = 0 THEN 1 ELSE 0 END),
                @TongSL       = ISNULL(SUM(TongSL), 0)
            FROM ByMa;
        END

        SELECT @ThanhGia AS ThanhGia,
               @TongMa   AS TongMaVT,
               @SoMaCoGia AS SoMaCoGia,
               @SoMaKhongGia AS SoMaKhongGia,
               @TongSL   AS TongSoLuong;



    END

    ELSE IF @Action = 'GetNKDuKienByRange'
    BEGIN
        SELECT
            CONVERT(VARCHAR(10), CAST(nk.NgayNKDuKien AS DATE), 120) AS NgayNKDuKien,
            ISNULL(nk.SoLo, '')                                      AS SoLo,
            ISNULL(nk.POMua, '')                                     AS PO,
            ISNULL(nk.MaKH, '')                                      AS MaKH,
            ISNULL(kh.TenKH, '')                                     AS TenKH,
            ISNULL((SELECT SUM(ISNULL(ct.SLTong, 0))
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    WHERE ct.SoLoID = nk.SoLoID
                      AND ct.POMua  = nk.POMua), 0)                  AS SoLuongDuKien
        FROM dbo.ERP_NhapKhoNPL nk
        LEFT JOIN dbo.KhachHang kh ON kh.MaKH = nk.MaKH
        WHERE nk.NgayNKDuKien IS NOT NULL
          AND nk.NgayNKDuKien >= CAST(@TuNgay AS DATE)
          AND nk.NgayNKDuKien < DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
        ORDER BY nk.NgayNKDuKien, nk.SoLo;
    END

    ELSE IF @Action = 'GetNhapDetailByRange'
    BEGIN
        IF OBJECT_ID('tempdb..#NhapFinalR_NDR') IS NOT NULL DROP TABLE #NhapFinalR_NDR;
        SELECT
            ISNULL(nk.SoLo, '')                                                    AS PINCC,
            a.PO                                                                   AS PO,
            a.MaNPL                                                                AS MaNPL,
            CAST('' AS NVARCHAR(200))                                              AS ItemCode,
            CAST('' AS NVARCHAR(200))                                              AS MaMauVT,
            CAST('' AS NVARCHAR(200))                                              AS MauVT,
            CAST('' AS NVARCHAR(200))                                              AS WidthSize,
            CAST('' AS NVARCHAR(200))                                              AS DonViVT,
            ISNULL(NULLIF(kh.TenKH, ''), N'Khách trống')                           AS TenKH,
            a.MaDVVT                                                               AS MaDVVT,
            a.SoLuong                                                              AS SoLuong,
            a.SoBarCode                                                            AS SoBarCode,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 3), '')                   AS MaVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 2), '')                   AS MauVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 1), '')                   AS KhoVaiID,
            CAST(a.NgayNhapTu AS DATE)                                             AS NgayNhap
        INTO #NhapFinalR_NDR
        FROM (
            SELECT
                ct.SoLoID,
                ct.MaNPL,
                MAX(ct.POMua)                          AS PO,
                MAX(ct.MaDVVT)                         AS MaDVVT,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuong,
                COUNT(*)                               AS SoBarCode,
                MIN(ct.NgayNhapKho)                    AS NgayNhapTu,
                MAX(ct.NgayNhapKho)                    AS NgayNhapDen
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            WHERE ct.NgayNhapKho IS NOT NULL
              AND ct.NgayNhapKho >= CAST(@TuNgay AS DATE)
              AND ct.NgayNhapKho <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
            GROUP BY ct.SoLoID, ct.MaNPL
        ) a
        LEFT JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = a.SoLoID
        LEFT JOIN dbo.KhachHang     kh ON kh.MaKH    = nk.MaKH
        ORDER BY a.SoLuong DESC;

        IF OBJECT_ID('dbo.ERP_VatTuTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'UPDATE f SET ItemCode = ISNULL(vt.MaVT, '''')
                    FROM #NhapFinalR_NDR f LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = f.MaVTID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_MauVTTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'UPDATE f SET MaMauVT = ISNULL(m.MaMauVT, ''''), MauVT = ISNULL(m.MauVT, '''')
                    FROM #NhapFinalR_NDR f LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID = f.MauVTID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'UPDATE f SET WidthSize = ISNULL(k.KhoVai,''''), DonViVT = ISNULL(d.TenDVVT,'''')
                    FROM #NhapFinalR_NDR f
                    LEFT JOIN dbo.ERP_KhoVai  k ON k.KhoVaiID = f.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT d ON d.MaDVVT   = f.MaDVVT;';
            END TRY BEGIN CATCH END CATCH
        END

        SELECT PINCC, PO, MaNPL, ItemCode, MaMauVT, MauVT, WidthSize, DonViVT, TenKH, SoLuong, SoBarCode, NgayNhap
        FROM #NhapFinalR_NDR
        ORDER BY NgayNhap, PINCC, SoLuong DESC;

        DROP TABLE #NhapFinalR_NDR;
    END

    ELSE IF @Action = 'GetXuatDetailByRange'
    BEGIN
        SELECT
            xh.MaLenh,
            COUNT(*)                        AS SoBarCode,
            SUM(ISNULL(xh.SLNhap, 0))       AS SoLuong,
            ISNULL(MAX(hh.TenHang), '')     AS TenHang,
            ISNULL(MAX(kh.TenKH), '')       AS TenKH,
            MIN(xh.NgayXuatHang)            AS NgayXuatTu,
            MAX(xh.NgayXuatHang)            AS NgayXuatDen
        FROM dbo.PhieuXuatHang xh
        LEFT JOIN dbo.GopDonHang  gd ON gd.MaGop  = xh.MaGop
        LEFT JOIN dbo.DonHangTong dh ON dh.MaDH   = ISNULL(gd.MaDH, xh.MaGop)
        LEFT JOIN dbo.HangHoa    hh ON hh.MaHang  = dh.MaHang AND hh.MaKH = dh.MaKH
        LEFT JOIN dbo.KhachHang  kh ON kh.MaKH    = dh.MaKH
        WHERE xh.ModuleXH = 1
          AND xh.NgayXuatHang IS NOT NULL
          AND xh.NgayXuatHang >= CAST(@TuNgay AS DATE)
          AND xh.NgayXuatHang <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
        GROUP BY xh.MaLenh
        ORDER BY SUM(xh.SLNhap) DESC;
    END

    -- ===================================================================
    -- [GetKiemKeDetailByRange]
    -- Lấy danh sách chi tiết các phiếu kiểm kê trong khoảng thời gian
    -- ===================================================================
    ELSE IF @Action = 'GetKiemKeDetailByRange'
    BEGIN
        SET @HasV2R = 0;
        IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPLV2', 'U') IS NOT NULL SET @HasV2R = 2;
        ELSE IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPL', 'U') IS NOT NULL SET @HasV2R = 1;

        IF @HasV2R = 0
        BEGIN
            SELECT CAST(NULL AS NVARCHAR(100)) AS PhieuKiemKe,
                   CAST(NULL AS NVARCHAR(100)) AS SoLo,
                   CAST(0 AS INT) AS SoBarCode,
                   CAST(0 AS DECIMAL(18,2)) AS SoLuong,
                   CAST(NULL AS NVARCHAR(100)) AS UserKK,
                   CAST(NULL AS NVARCHAR(500)) AS GhiChu,
                   CAST(NULL AS DATETIME) AS NgayKKTu,
                   CAST(NULL AS DATETIME) AS NgayKKDen
            WHERE 1 = 0;
            RETURN;
        END

        IF @HasV2R = 2
        BEGIN
            SELECT
                ISNULL(kk.PhieuKiemKe, '')     AS PhieuKiemKe,
                ISNULL(kk.SoLo, '')            AS SoLo,
                COUNT(*)                    AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu,
                MIN(kk.DateKiemKe)             AS NgayKKTu,
                MAX(kk.DateKiemKe)             AS NgayKKDen
            FROM dbo.ERPPhieuKiemKe_NPLV2 kk
            LEFT JOIN dbo.SYS_NhanVien nv ON nv.UserID = kk.UserKK
            WHERE kk.DateKiemKe IS NOT NULL
              AND kk.DateKiemKe >= CAST(@TuNgay AS DATE)
              AND kk.DateKiemKe <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
        ELSE
        BEGIN
            SELECT
                ISNULL(kk.PhieuKiemKe, '')     AS PhieuKiemKe,
                ISNULL(kk.SoLo, '')            AS SoLo,
                COUNT(*)                    AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu,
                MIN(kk.DateKiemKe)             AS NgayKKTu,
                MAX(kk.DateKiemKe)             AS NgayKKDen
            FROM dbo.ERPPhieuKiemKe_NPL kk
            LEFT JOIN dbo.SYS_NhanVien nv ON nv.UserID = kk.UserKK
            WHERE kk.DateKiemKe IS NOT NULL
              AND kk.DateKiemKe >= CAST(@TuNgay AS DATE)
              AND kk.DateKiemKe <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
    END

    -- ===================================================================
    -- [GetNhapDetailByDay]
    -- Lấy danh sách chi tiết nhập kho theo ngày cụ thể
    -- ===================================================================
    ELSE IF @Action = 'GetNhapDetailByDay'
    BEGIN
        IF OBJECT_ID('tempdb..#NhapFinal_NDD') IS NOT NULL DROP TABLE #NhapFinal_NDD;
        SELECT TOP 500
            ISNULL(nk.SoLo, '')                    AS PINCC,
            a.PO                                   AS PO,
            a.MaNPL                                AS MaNPL,
            CAST('' AS NVARCHAR(200))              AS ItemCode,
            CAST('' AS NVARCHAR(200))              AS MaMauVT,
            CAST('' AS NVARCHAR(200))              AS MauVT,
            CAST('' AS NVARCHAR(200))              AS WidthSize,
            CAST('' AS NVARCHAR(200))              AS DonViVT,
            ISNULL(NULLIF(kh.TenKH, ''), N'Khách trống')                   AS TenKH,
            a.MaDVVT                               AS MaDVVT,
            a.SoLuong                              AS SoLuong,
            a.SoBarCode                            AS SoBarCode,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 3), '') AS MaVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 2), '') AS MauVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 1), '') AS KhoVaiID
        INTO #NhapFinal_NDD
        FROM (
            SELECT
                ct.SoLoID,
                ct.MaNPL,
                MAX(ct.POMua)                          AS PO,
                MAX(ct.MaDVVT)                         AS MaDVVT,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuong,
                COUNT(*)                               AS SoBarCode
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            WHERE ct.NgayNhapKho IS NOT NULL
              AND ct.NgayNhapKho >= CAST(@Ngay AS DATE)
              AND ct.NgayNhapKho < DATEADD(DAY, 1, CAST(@Ngay AS DATE))
              AND EXISTS (
                  SELECT 1 FROM dbo.ERP_VatTuCBM vt
                  WHERE vt.Barcode = ct.BarCode
                    AND (ISNULL(vt.MaONPL, '') <> ''
                         OR EXISTS (SELECT 1 FROM PhieuXuatHang px WHERE px.BarCodeGoc = vt.Barcode)
                         OR EXISTS (SELECT 1 FROM PhieuThuHoiNPL th WHERE th.BarCode = vt.Barcode))
              )
            GROUP BY ct.SoLoID, ct.MaNPL
        ) a
        LEFT JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = a.SoLoID
        LEFT JOIN dbo.KhachHang     kh ON kh.MaKH    = nk.MaKH
        ORDER BY a.SoLuong DESC;

        IF OBJECT_ID('dbo.ERP_VatTuTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f SET ItemCode = ISNULL(vt.MaVT, '''')
                    FROM #NhapFinal_NDD f
                    LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = f.MaVTID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_MauVTTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f
                    SET MaMauVT = ISNULL(m.MaMauVT, ''''),
                        MauVT   = ISNULL(m.MauVT,   '''')
                    FROM #NhapFinal_NDD f
                    LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID = f.MauVTID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f
                    SET WidthSize = ISNULL(k.KhoVai, ''''),
                        DonViVT   = ISNULL(d.TenDVVT, '''')
                    FROM #NhapFinal_NDD f
                    LEFT JOIN dbo.ERP_KhoVai k  ON k.KhoVaiID = f.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT d ON d.MaDVVT   = f.MaDVVT;';
            END TRY BEGIN CATCH END CATCH
        END

        SELECT PINCC, PO, MaNPL, ItemCode, MaMauVT, MauVT, WidthSize, DonViVT, TenKH, SoLuong, SoBarCode
        FROM #NhapFinal_NDD
        ORDER BY SoLuong DESC;

        DROP TABLE #NhapFinal_NDD;
    END

    -- ===================================================================
    -- [GetXuatDetailByDay]
    -- Lấy danh sách chi tiết xuất kho theo ngày cụ thể
    -- ===================================================================
    ELSE IF @Action = 'GetXuatDetailByDay'
    BEGIN
        SELECT TOP 500
            xh.MaLenh,
            COUNT(*)                        AS SoBarCode,
            SUM(ISNULL(xh.SLNhap, 0))       AS SoLuong,
            ISNULL(MAX(hh.TenHang), '')     AS TenHang,
            ISNULL(MAX(kh.TenKH), '')       AS TenKH
        FROM dbo.PhieuXuatHang xh
        LEFT JOIN dbo.GopDonHang  gd ON gd.MaGop  = xh.MaGop
        LEFT JOIN dbo.DonHangTong dh ON dh.MaDH   = ISNULL(gd.MaDH, xh.MaGop)
        LEFT JOIN dbo.HangHoa    hh ON hh.MaHang  = dh.MaHang AND hh.MaKH = dh.MaKH
        LEFT JOIN dbo.KhachHang  kh ON kh.MaKH    = dh.MaKH
        WHERE xh.ModuleXH = 1
          AND xh.NgayXuatHang IS NOT NULL
          AND xh.NgayXuatHang >= CAST(@Ngay AS DATE)
          AND xh.NgayXuatHang < DATEADD(DAY, 1, CAST(@Ngay AS DATE))
          AND ISNULL(xh.MaHang,'') NOT LIKE '%PSH_%'
          AND EXISTS (
              SELECT 1 FROM dbo.ERP_VatTuCBM vt
              WHERE vt.Barcode = xh.BarCodeGoc
                AND (ISNULL(vt.MaONPL, '') <> ''
                     OR EXISTS (SELECT 1 FROM PhieuXuatHang px WHERE px.BarCodeGoc = vt.Barcode)
                     OR EXISTS (SELECT 1 FROM PhieuThuHoiNPL th WHERE th.BarCode = vt.Barcode))
          )
        GROUP BY xh.MaLenh
        ORDER BY SUM(xh.SLNhap) DESC;
    END

    -- ===================================================================
    -- [GetKiemKeDetailByDay]
    -- Lấy danh sách chi tiết kiểm kê theo ngày cụ thể
    -- ===================================================================
    ELSE IF @Action = 'GetKiemKeDetailByDay'
    BEGIN
        SET @HasV2D = 0;
        IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPLV2', 'U') IS NOT NULL SET @HasV2D = 2;
        ELSE IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPL', 'U') IS NOT NULL SET @HasV2D = 1;

        IF @HasV2D = 0
        BEGIN
            SELECT CAST(NULL AS NVARCHAR(100)) AS PhieuKiemKe,
                   CAST(NULL AS NVARCHAR(100)) AS SoLo,
                   CAST(0 AS INT) AS SoBarCode,
                   CAST(0 AS DECIMAL(18,2)) AS SoLuong,
                   CAST(NULL AS NVARCHAR(100)) AS UserKK,
                   CAST(NULL AS NVARCHAR(500)) AS GhiChu
            WHERE 1 = 0;
            RETURN;
        END

        IF @HasV2D = 2
        BEGIN
            SELECT TOP 500
                ISNULL(kk.PhieuKiemKe, '')     AS PhieuKiemKe,
                ISNULL(kk.SoLo, '')            AS SoLo,
                COUNT(*)                    AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu
            FROM dbo.ERPPhieuKiemKe_NPLV2 kk
            LEFT JOIN dbo.SYS_NhanVien nv ON nv.UserID = kk.UserKK
            WHERE kk.DateKiemKe IS NOT NULL
              AND kk.DateKiemKe >= CAST(@Ngay AS DATE)
              AND kk.DateKiemKe < DATEADD(DAY, 1, CAST(@Ngay AS DATE))
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
        ELSE
        BEGIN
            SELECT TOP 500
                ISNULL(kk.PhieuKiemKe, '')     AS PhieuKiemKe,
                ISNULL(kk.SoLo, '')            AS SoLo,
                COUNT(*)                    AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu
            FROM dbo.ERPPhieuKiemKe_NPL kk
            LEFT JOIN dbo.SYS_NhanVien nv ON nv.UserID = kk.UserKK
            WHERE kk.DateKiemKe IS NOT NULL
              AND kk.DateKiemKe >= CAST(@Ngay AS DATE)
              AND kk.DateKiemKe < DATEADD(DAY, 1, CAST(@Ngay AS DATE))
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
    END

    -- ===================================================================
    -- [GetRackSlotDetail]
    -- Lấy thông tin chi tiết sức chứa và vật tư của từng ô kệ
    -- ===================================================================
    ELSE IF @Action = 'GetRackSlotDetail'
    BEGIN




        SELECT
            ke.Module,
            d.TenDay,
            ke.TenKe,
            o.TenO,
            ISNULL(ct.MaNPL, '')                                                AS MaNPL,
            vt.Barcode                                                          AS BarCode,
            ISNULL(nk.MaKH, '')                                                 AS MaKH,
            ISNULL(kh.TenKH, '')                                                AS TenKH,
            ROUND(ISNULL(vt.CBM, 0), 4)                                         AS CBM,
            ISNULL(PARSENAME(REPLACE(ISNULL(ct.MaNPL, ''), '@', '.'), 3), '')   AS MaVTID,
            ISNULL(PARSENAME(REPLACE(ISNULL(ct.MaNPL, ''), '@', '.'), 2), '')   AS MauVTID,
            ISNULL(PARSENAME(REPLACE(ISNULL(ct.MaNPL, ''), '@', '.'), 1), '')   AS KhoVaiID
        INTO #tmpSlot_RSD
        FROM dbo.ERP_VatTuCBM vt
        INNER JOIN dbo.ERP_ONPL o  ON o.TenO = vt.MaONPL
        INNER JOIN dbo.ERP_KeNPL ke ON ke.KeID = o.KeID
        LEFT  JOIN dbo.ERP_DayNPL d ON d.DayID = ke.DayID
        LEFT  JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON ct.BarCode = vt.Barcode
        LEFT  JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = ct.SoLoID
        LEFT  JOIN dbo.KhachHang     kh ON kh.MaKH    = nk.MaKH
        WHERE vt.MaONPL IS NOT NULL
          AND ke.Module IN (1, 2)
          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE N'%lỗi%'
          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE N'%n%'
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi tx WHERE tx.BarCode = vt.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #TempSoanHang ts WHERE ts.BarCode = vt.Barcode);

        IF OBJECT_ID('tempdb..#SlotAgg_RSD') IS NOT NULL DROP TABLE #SlotAgg_RSD;
        SELECT
            Module,
            CASE WHEN Module = 1 THEN N'NL' WHEN Module = 2 THEN N'PL' ELSE N'?' END AS LoaiKho,
            TenDay,
            TenKe,
            TenO,
            COUNT(DISTINCT BarCode)                                          AS SoBarCode,
            COUNT(DISTINCT CASE WHEN MaKH  <> '' THEN MaKH  END)             AS SoKhachHang,
            STUFF((SELECT DISTINCT TOP 3 ', ' + s3.TenKH FROM #tmpSlot_RSD s3
                   WHERE s3.TenKe = s.TenKe AND s3.TenO = s.TenO AND s3.TenKH <> ''
                   FOR XML PATH('')), 1, 2, '')                              AS DanhSachKH,
            MAX(MaVTID)                                                      AS MaVTID,
            MAX(MauVTID)                                                     AS MauVTID,
            MAX(KhoVaiID)                                                    AS KhoVaiID,
            ROUND(SUM(CBM), 4)                                               AS TongCBM,
            CAST('' AS NVARCHAR(200))                                        AS MaMauVT,
            CAST('' AS NVARCHAR(200))                                        AS MauVT,
            CAST('' AS NVARCHAR(200))                                        AS WidthSize
        INTO #SlotAgg_RSD
        FROM #tmpSlot_RSD s
        GROUP BY Module, TenDay, TenKe, TenO;

        IF OBJECT_ID('dbo.ERP_MauVTTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE a
                    SET MaMauVT = ISNULL(m.MaMauVT, ''''),
                        MauVT   = ISNULL(m.MauVT,   '''')
                    FROM #SlotAgg_RSD a
                    LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID = a.MauVTID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE a
                    SET WidthSize = LTRIM(RTRIM(ISNULL(k.KhoVai, '''') + '' '' + ISNULL(d.TenDVVT, '''')))
                    FROM #SlotAgg_RSD a
                    LEFT JOIN dbo.ERP_KhoVai k  ON k.KhoVaiID = a.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT d ON d.MaDVVT   = k.MaDVVT;';
            END TRY BEGIN CATCH END CATCH
        END

        SELECT Module, LoaiKho, TenDay, TenKe, TenO,
               SoBarCode, SoKhachHang, DanhSachKH,
               MaMauVT, MauVT, WidthSize, TongCBM
        FROM #SlotAgg_RSD
        ORDER BY Module, TenDay, TenKe, TenO;

        DROP TABLE #SlotAgg_RSD;
        DROP TABLE #tmpSlot_RSD;


    END

    ELSE IF @Action = 'GetActivityCalendar'
    BEGIN
        SET @StartDateCal = CAST(@TuNgay  AS DATE);
        SET @EndDateCal   = CAST(@DenNgay AS DATE);
        SET @TotalDaysCal  = DATEDIFF(DAY, @StartDateCal, @EndDateCal) + 1;
        IF @TotalDaysCal < 1   SET @TotalDaysCal = 1;
        SET @EndPlus1Cal = DATEADD(DAY, 1, @EndDateCal);

        IF OBJECT_ID('tempdb..#CalKK_AC') IS NOT NULL DROP TABLE #CalKK_AC;
        CREATE TABLE #CalKK_AC (Ngay DATE PRIMARY KEY, TotalKiemKe DECIMAL(18,2));
        IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPLV2', 'U') IS NOT NULL
        BEGIN
            INSERT INTO #CalKK_AC (Ngay, TotalKiemKe)
            EXEC sp_executesql
                N'SELECT CAST(DateKiemKe AS DATE) AS Ngay,
                         SUM(ISNULL(SLKiemKe, 0)) AS TotalKiemKe
                  FROM dbo.ERPPhieuKiemKe_NPLV2
                  WHERE DateKiemKe >= @S AND DateKiemKe < @E
                  GROUP BY CAST(DateKiemKe AS DATE);',
                N'@S DATE, @E DATE', @S = @StartDateCal, @E = @EndPlus1Cal;
        END
        ELSE IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPL', 'U') IS NOT NULL
        BEGIN
            INSERT INTO #CalKK_AC (Ngay, TotalKiemKe)
            EXEC sp_executesql
                N'SELECT CAST(DateKiemKe AS DATE) AS Ngay,
                         SUM(ISNULL(SLKiemKe, 0)) AS TotalKiemKe
                  FROM dbo.ERPPhieuKiemKe_NPL
                  WHERE DateKiemKe >= @S AND DateKiemKe < @E
                  GROUP BY CAST(DateKiemKe AS DATE);',
                N'@S DATE, @E DATE', @S = @StartDateCal, @E = @EndPlus1Cal;
        END
        ELSE IF COL_LENGTH('dbo.ERP_ChiTietNhapKhoNPL', 'NgayKiemKe') IS NOT NULL
        BEGIN
            INSERT INTO #CalKK_AC (Ngay, TotalKiemKe)
            EXEC sp_executesql
                N'SELECT CAST(NgayKiemKe AS DATE) AS Ngay,
                         CAST(COUNT(*) AS DECIMAL(18,2)) AS TotalKiemKe
                  FROM dbo.ERP_ChiTietNhapKhoNPL
                  WHERE NgayKiemKe >= @S AND NgayKiemKe < @E
                  GROUP BY CAST(NgayKiemKe AS DATE);',
                N'@S DATE, @E DATE', @S = @StartDateCal, @E = @EndPlus1Cal;
        END

        IF OBJECT_ID('tempdb..#tempNhapAC') IS NOT NULL DROP TABLE #tempNhapAC;
        SELECT t1.BarCode, t1.NgayNhapKho, t1.SoLuongThucTeBanDau
        INTO #tempNhapAC
        FROM dbo.ERP_ChiTietNhapKhoNPL t1
        WHERE t1.NgayNhapKho >= @StartDateCal AND t1.NgayNhapKho < @EndPlus1Cal;

        DELETE t1 FROM #tempNhapAC t1
        WHERE NOT EXISTS (SELECT 1 FROM dbo.ERP_VatTuCBM vt WHERE vt.Barcode = t1.BarCode AND ISNULL(vt.MaONPL, '') <> '')
          AND NOT EXISTS (SELECT 1 FROM PhieuXuatHang px WHERE px.BarCodeGoc = t1.BarCode)
          AND NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL th WHERE th.BarCode = t1.BarCode);

        ;WITH E1(N) AS (SELECT 1 FROM (VALUES (1),(1),(1),(1),(1),(1),(1),(1),(1),(1)) t(N)),
              E2(N) AS (SELECT 1 FROM E1 a CROSS JOIN E1 b),
              E4(N) AS (SELECT 1 FROM E2 a CROSS JOIN E2 b),
              E8(N) AS (SELECT 1 FROM E4 a CROSS JOIN E4 b),
              Tally(N) AS (SELECT TOP (@TotalDaysCal) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 FROM E8),
              CalDays AS (
                  SELECT DATEADD(DAY, N, @StartDateCal) AS NgayHoatDong FROM Tally
              ),
              CalNhap AS (
                  SELECT CAST(NgayNhapKho AS DATE) AS Ngay,
                         SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS TotalIn
                  FROM #tempNhapAC
                  GROUP BY CAST(NgayNhapKho AS DATE)
              ),
              CalXuat AS (
                  SELECT CAST(t1.NgayXuatHang AS DATE) AS Ngay,
                         SUM(ISNULL(t1.SLNhap, 0)) AS TotalOut
                  FROM dbo.PhieuXuatHang t1
                  WHERE t1.ModuleXH = 1
                    AND t1.NgayXuatHang >= @StartDateCal
                    AND t1.NgayXuatHang <  @EndPlus1Cal
                    AND ISNULL(t1.MaHang,'') NOT LIKE '%PSH_%'
                    AND EXISTS (
                        SELECT 1 FROM dbo.ERP_VatTuCBM vt
                        WHERE vt.Barcode = t1.BarCodeGoc
                    )
                  GROUP BY CAST(t1.NgayXuatHang AS DATE)
              )
        SELECT
            CONVERT(VARCHAR(10), d.NgayHoatDong, 120) AS NgayHoatDong,
            ISNULL(n.TotalIn,     0) AS TotalIn,
            ISNULL(x.TotalOut,    0) AS TotalOut,
            ISNULL(k.TotalKiemKe, 0) AS TotalKiemKe,
            ISNULL(n.TotalIn, 0) + ISNULL(x.TotalOut, 0) + ISNULL(k.TotalKiemKe, 0) AS TotalActivity
        FROM CalDays d
        LEFT JOIN CalNhap n ON n.Ngay = d.NgayHoatDong
        LEFT JOIN CalXuat x ON x.Ngay = d.NgayHoatDong
        LEFT JOIN #CalKK_AC k ON k.Ngay = d.NgayHoatDong
        ORDER BY d.NgayHoatDong;

        DROP TABLE #CalKK_AC;
    END


    ELSE IF @Action = 'GetFlowTrendByRange'
    BEGIN
        SET @StartDateRange = CAST(@TuNgay AS DATE);
        SET @EndDateRange   = CAST(@DenNgay AS DATE);
        SET @TotalDaysRange  = DATEDIFF(DAY, @StartDateRange, @EndDateRange) + 1;
        IF @TotalDaysRange < 1 SET @TotalDaysRange = 1;

        -- 1. X�Y D?NG L�I D? LI?U CHU?N (Core Logic)
        DECLARE @IsNPL_Check NVARCHAR(200);
        SELECT TOP 1 @IsNPL_Check = IsNPL FROM ERP_ChiTietNhapKhoNPL WHERE MaNPL = @MaNPL;

        IF OBJECT_ID('tempdb..#tempNhapKhoNPL') IS NOT NULL DROP TABLE #tempNhapKhoNPL;
        SELECT t1.SoLoID, t1.IsNPL, t1.MaHang, t1.MaKH INTO #tempNhapKhoNPL FROM ERP_NhapKhoNPL t1
        WHERE (@MaKH = 'all' OR t1.MaKH = @MaKH)
          AND (@MaHang = 'all' OR t1.MaHang = @MaHang)
          AND (@SoLoID = 'all' OR t1.SoLoID = @SoLoID)
          AND (@IsNPL = 2 OR IsNPL = @IsNPL_Check);

        IF OBJECT_ID('tempdb..#tempTKho') IS NOT NULL DROP TABLE #tempTKho;
        SELECT t1.ID, t1.SoloID, t1.MaNPL, t1.IsNPL, t1.BarCode, t1.SoLuongThucTeBanDau
        INTO #tempTKho
        FROM ERP_ChiTietNhapKhoNPL t1
        INNER JOIN #tempNhapKhoNPL tt ON t1.SoLoID = tt.SoLoID AND t1.IsNPL = tt.IsNPL
        WHERE (@Nhom = 'all' OR t1.MaNhom = @Nhom)
          AND (@MaNPL = 'all' OR t1.MaNPL = @MaNPL)
          AND t1.SoLuongThucTeBanDau <> 0
          AND (@SoLoID = 'all' OR t1.SoLoID = @SoLoID)
          AND (@MaHang = 'all' OR ISNULL(tt.MaHang, '') = @MaHang)
          AND (@MaKH = 'all' OR ISNULL(tt.MaKH, '') = @MaKH);

        DELETE t1 FROM #tempTKho t1
        WHERE NOT EXISTS (SELECT 1 FROM dbo.ERP_VatTuCBM vt WHERE vt.Barcode = t1.BarCode AND ISNULL(vt.MaONPL, '') <> '')
          AND NOT EXISTS (SELECT 1 FROM PhieuXuatHang px WHERE px.BarCodeGoc = t1.BarCode)
          AND NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL th WHERE th.BarCode = t1.BarCode);

        IF @KhoLoi = '1'
        BEGIN
            DELETE t1 FROM #tempTKho t1
            WHERE NOT EXISTS (SELECT 1 FROM ERP_VatTuCBM vt INNER JOIN ERP_ONPL o ON o.TenO = vt.MaONPL INNER JOIN ERP_DayNPL d ON d.DayID = o.DayID AND d.Status = 2 WHERE vt.Barcode = t1.BarCode AND vt.MaONPL IS NOT NULL);
        END
        ELSE IF @KhoLoi = '0'
        BEGIN
            DELETE t1 FROM #tempTKho t1
            WHERE EXISTS (SELECT 1 FROM ERP_VatTuCBM vt INNER JOIN ERP_ONPL o ON o.TenO = vt.MaONPL INNER JOIN ERP_DayNPL d ON d.DayID = o.DayID AND d.Status = 2 WHERE vt.Barcode = t1.BarCode AND vt.MaONPL IS NOT NULL);
        END

        -- 2. T�NH T?N �?U K?
        DECLARE @TonDauKy DECIMAL(18,4) = 0;
        DECLARE @TonNhapDK DECIMAL(18,4) = 0;
        DECLARE @TonXuatDK DECIMAL(18,4) = 0;
        DECLARE @TonThuDK DECIMAL(18,4) = 0;
        DECLARE @TonChenhDK DECIMAL(18,4) = 0;

        SELECT @TonChenhDK = ROUND(SUM(ISNULL(SLKiemKeBanDau,0) - ISNULL(SLKiemKeEdit,0)), 4)
        FROM ERPPhieuKiemKe_NPL t1
        WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL AND DateDuyetKK < @StartDateRange
          AND EXISTS (SELECT 1 FROM #tempTKho t2 WHERE t1.BarCode = t2.BarCode);

        SELECT @TonNhapDK = SUM(t1.SoLuongThucTeBanDau)
        FROM ERP_ChiTietNhapKhoNPL t1
        INNER JOIN #tempNhapKhoNPL t2 ON t1.SoLoID = t2.SoLoID AND t1.IsNPL = t2.IsNPL
        INNER JOIN #tempTKho t4 ON t1.ID = t4.ID
        WHERE t1.NgayNhapKho < @StartDateRange;

        SELECT @TonXuatDK = SUM(SLNhap)
        FROM PhieuXuatHang t1
        INNER JOIN #tempTKho t2 ON t1.BarCodeGoc = t2.BarCode
        WHERE t1.NgayXuatHang < @StartDateRange AND ISNULL(MaHang,'') NOT LIKE '%PSH_%';

        SELECT @TonThuDK = SUM(ThuHoi)
        FROM PhieuThuHoiNPL t1
        LEFT JOIN PhieuXuatHang t2 ON t1.BarCode = t2.BarCode
        WHERE t1.NgayTH < @StartDateRange
          AND EXISTS (SELECT 1 FROM #tempTKho t3 WHERE t2.BarCodeGoc = t3.BarCode);

        -- PHỤC HỒI LOGIC TÍNH TỒN ĐẦU KỲ BỊ THIẾU
        SET @TonDauKy = ISNULL(@TonNhapDK, 0) - ISNULL(@TonXuatDK, 0) + ISNULL(@TonThuDK, 0) - ISNULL(@TonChenhDK, 0);

        IF OBJECT_ID('tempdb..#tempChenhLechTK') IS NOT NULL DROP TABLE #tempChenhLechTK;
        -- 3. TRONG KY THEO NGAY & 4. TONG HOP RA BIEU DO
        ;WITH tempChenhLechTK AS (
            SELECT CAST(DateDuyetKK AS DATE) AS Ngay, ROUND(SUM(ISNULL(SLKiemKeBanDau,0) - ISNULL(SLKiemKeEdit,0)), 4) AS SLChenhLenhTK
            FROM ERPPhieuKiemKe_NPL t1
            WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL AND (DateDuyetKK >= @StartDateRange AND DateDuyetKK < DATEADD(day, 1, @EndDateRange))
              AND EXISTS (SELECT 1 FROM #tempTKho t2 WHERE t1.BarCode = t2.BarCode)
            GROUP BY CAST(DateDuyetKK AS DATE)
        ),
        tempNhapKhoTK AS (
            SELECT CAST(t1.NgayNhapKho AS DATE) AS Ngay, SUM(CAST(t1.SoLuongThucTeBanDau AS DECIMAL(18,4))) AS SLNhapTK
            FROM ERP_ChiTietNhapKhoNPL t1
            INNER JOIN #tempNhapKhoNPL t2 ON t1.SoLoID = t2.SoLoID AND t1.IsNPL = t2.IsNPL
            INNER JOIN #tempTKho t4 ON t1.ID = t4.ID
            WHERE (t1.NgayNhapKho >= @StartDateRange AND t1.NgayNhapKho < DATEADD(day, 1, @EndDateRange))
            GROUP BY CAST(t1.NgayNhapKho AS DATE)
        ),
        tempXuatHangTK AS (
            SELECT CAST(t1.NgayXuatHang AS DATE) AS Ngay, SUM(SLNhap) AS SLXuatTK
            FROM PhieuXuatHang t1
            INNER JOIN #tempTKho t2 ON t1.BarCodeGoc = t2.BarCode
            WHERE (t1.NgayXuatHang >= @StartDateRange AND t1.NgayXuatHang < DATEADD(day, 1, @EndDateRange)) AND ISNULL(MaHang,'') NOT LIKE '%PSH_%'
            GROUP BY CAST(t1.NgayXuatHang AS DATE)
        ),
        tempThuHoiTK AS (
            SELECT CAST(t1.NgayTH AS DATE) AS Ngay, SUM(ThuHoi) AS SLThuTK
            FROM PhieuThuHoiNPL t1
            LEFT JOIN PhieuXuatHang t2 ON t1.BarCode = t2.BarCode
            WHERE (t1.NgayTH >= @StartDateRange AND t1.NgayTH < DATEADD(day, 1, @EndDateRange))
              AND EXISTS (SELECT 1 FROM #tempTKho t3 WHERE t2.BarCodeGoc = t3.BarCode)
            GROUP BY CAST(t1.NgayTH AS DATE)
        ),
        E1(N) AS (SELECT 1 FROM (VALUES (1),(1),(1),(1),(1),(1),(1),(1),(1),(1)) t(N)),
        E2(N) AS (SELECT 1 FROM E1 a CROSS JOIN E1 b),
        E4(N) AS (SELECT 1 FROM E2 a CROSS JOIN E2 b),
        E8(N) AS (SELECT 1 FROM E4 a CROSS JOIN E4 b),
        Tally(N) AS (SELECT TOP (@TotalDaysRange) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 FROM E8),
        Days AS (
            SELECT CAST(DATEADD(DAY, N, @StartDateRange) AS DATE) AS Ngay
            FROM Tally
        ),
        DailyFlow AS (
            SELECT
                d.Ngay,
                ISNULL(n.SLNhapTK, 0) AS TotalIn,
                ISNULL(x.SLXuatTK, 0) AS TotalOut,
                ISNULL(th.SLThuTK, 0) AS TotalRecover,
                ISNULL(c.SLChenhLenhTK, 0) AS TotalDiscrepancy
            FROM Days d
            LEFT JOIN tempNhapKhoTK n ON d.Ngay = n.Ngay
            LEFT JOIN tempXuatHangTK x ON d.Ngay = x.Ngay
            LEFT JOIN tempThuHoiTK th ON d.Ngay = th.Ngay
            LEFT JOIN tempChenhLechTK c ON d.Ngay = c.Ngay
        )
        SELECT
            CONVERT(VARCHAR(10), Ngay, 120) AS Ngay,
            TotalIn,
            TotalOut,
            TotalRecover,
            TotalDiscrepancy,
            @TonDauKy + SUM(TotalIn - TotalOut + TotalRecover - TotalDiscrepancy) OVER (ORDER BY Ngay ROWS UNBOUNDED PRECEDING) AS TotalStock
        FROM DailyFlow
        ORDER BY Ngay;

        DROP TABLE #tempNhapKhoNPL;
        DROP TABLE #tempTKho;
    END


    -- ===================================================================
    -- [GetMoMComparison]
    -- So sánh tổng quan Nhập/Xuất kho giữa tháng hiện tại và tháng trước (Month-over-Month)
    -- ===================================================================
    ELSE IF @Action = 'GetMoMComparison'
    BEGIN
        SET @MaxDate = NULL;
        SELECT @MaxDate = MAX(d) FROM (
            SELECT MAX(CAST(NgayNhapKho AS DATE)) AS d FROM ERP_ChiTietNhapKhoNPL WHERE NgayNhapKho IS NOT NULL
            UNION ALL
            SELECT MAX(CAST(NgayXuatHang AS DATE)) AS d FROM PhieuXuatHang WHERE ModuleXH=1 AND NgayXuatHang IS NOT NULL
        ) t;

        IF @MaxDate IS NULL SET @MaxDate = CAST(GETDATE() AS DATE);

        SET @ThisMonth = DATEFROMPARTS(YEAR(@MaxDate), MONTH(@MaxDate), 1);
        SET @LastMonth = DATEADD(MONTH, -1, @ThisMonth);
        SET @NextMonth = DATEADD(MONTH,  1, @ThisMonth);

        SELECT 'ThisMonth' AS KieuKy,
            YEAR(@ThisMonth) AS Nam, MONTH(@ThisMonth) AS Thang,
            (SELECT ISNULL(SUM(ISNULL(SoLuongThucTeBanDau,0)),0) FROM ERP_ChiTietNhapKhoNPL
             WHERE NgayNhapKho >= @ThisMonth AND NgayNhapKho < @NextMonth) AS TotalIn,
            (SELECT ISNULL(SUM(ISNULL(SLNhap,0)),0) FROM PhieuXuatHang
             WHERE ModuleXH=1 AND NgayXuatHang >= @ThisMonth AND NgayXuatHang < @NextMonth) AS TotalOut
        UNION ALL
        SELECT 'LastMonth' AS KieuKy,
            YEAR(@LastMonth) AS Nam, MONTH(@LastMonth) AS Thang,
            (SELECT ISNULL(SUM(ISNULL(SoLuongThucTeBanDau,0)),0) FROM ERP_ChiTietNhapKhoNPL
             WHERE NgayNhapKho >= @LastMonth AND NgayNhapKho < @ThisMonth) AS TotalIn,
            (SELECT ISNULL(SUM(ISNULL(SLNhap,0)),0) FROM PhieuXuatHang
             WHERE ModuleXH=1 AND NgayXuatHang >= @LastMonth AND NgayXuatHang < @ThisMonth) AS TotalOut;
    END

    -- ===================================================================
    -- [GetTop5]
    -- Lấy Top 5 vật tư tồn kho nhiều nhất hoặc ít nhất
    -- ===================================================================
    ELSE IF @Action = 'GetTop5'
    BEGIN
        IF OBJECT_ID('tempdb..#TopFinal_T5') IS NOT NULL DROP TABLE #TopFinal_T5;
        SELECT TOP 5
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID_Raw,
            CAST(ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS NVARCHAR(200)) AS MaVT,
            CAST('' AS NVARCHAR(500))         AS TenVT,
            CAST('' AS NVARCHAR(200))         AS Mau,
            CAST('' AS NVARCHAR(200))         AS KhoVai,
            CAST('' AS NVARCHAR(500))         AS ChiTiet,
            CAST('' AS NVARCHAR(50))          AS TenDVVT,
            r.TonKho,
            r.Module                          AS NPL,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID
        INTO #TopFinal_T5
        FROM (
            SELECT
                ct.MaNPL,
                o.Module,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode
            INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL
            WHERE v.MaONPL IS NOT NULL
              AND o.Module IN (1, 2)
              AND (@LoaiNPL = 0 OR o.Module = @LoaiNPL)
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi t WHERE t.BarCode = v.Barcode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang       t WHERE t.BarCode = v.Barcode)
            GROUP BY ct.MaNPL, o.Module
            HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) > 0
        ) r
        ORDER BY
            CASE WHEN @IsNhieuNhat = 1 THEN TonKho END DESC,
            CASE WHEN @IsNhieuNhat = 0 THEN TonKho END ASC;

        IF OBJECT_ID('dbo.ERP_MauVTTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f SET Mau = ISNULL(m.MauVT, '''')
                    FROM #TopFinal_T5 f
                    LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID = f.MauVTID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f SET KhoVai = ISNULL(k.KhoVai, '''')
                    FROM #TopFinal_T5 f
                    LEFT JOIN dbo.ERP_KhoVai k ON k.KhoVaiID = f.KhoVaiID;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_VatTuTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f
                    SET MaVT    = ISNULL(vt.MaVT,    f.MaVT),
                        TenVT   = ISNULL(vt.ChiTiet, ''''),
                        ChiTiet = ISNULL(vt.ChiTiet, '''')
                    FROM #TopFinal_T5 f
                    LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = f.MaVTID_Raw;';
            END TRY BEGIN CATCH END CATCH
        END

        IF OBJECT_ID('dbo.ERP_DonViVT') IS NOT NULL
           AND OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f
                    SET TenDVVT = ISNULL(d.TenDVVT, '''')
                    FROM #TopFinal_T5 f
                    LEFT JOIN dbo.ERP_KhoVai k ON k.KhoVaiID = f.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT d ON d.MaDVVT = k.MaDVVT;';
            END TRY BEGIN CATCH END CATCH
        END

        SELECT MaVT, TenVT, Mau, KhoVai, ChiTiet, TenDVVT, TonKho, NPL
        FROM #TopFinal_T5
        ORDER BY
            CASE WHEN @IsNhieuNhat = 1 THEN TonKho END DESC,
            CASE WHEN @IsNhieuNhat = 0 THEN TonKho END ASC;

        DROP TABLE #TopFinal_T5;


    END

    -- ===================================================================
    -- [GetCongViecChoXuLy]
    -- Lấy danh sách tổng hợp các công việc đang chờ xử lý (chờ nhập kho, chờ duyệt kiểm kê)
    -- ===================================================================
    ELSE IF @Action = 'GetCongViecChoXuLy'
    BEGIN
        DECLARE @ItemcodeChoNK INT = 0;
        DECLARE @KKChoDuyet INT = 0;

        SELECT @ItemcodeChoNK = COUNT(DISTINCT CONCAT(t1.SoLoID,'|',t1.MaNPL))
        FROM dbo.ERP_ChiTietNhapKhoNPL t1
        WHERE ISNULL(t1.IsDuyetNK, 0) <> 1
          AND (
              EXISTS (
                  SELECT 1 FROM dbo.QTY_KiemVaiV2 kv
                  INNER JOIN dbo.QTY_KiemVaiV2_XacNhan kx
                      ON kv.SoLoID = kx.SoLoID AND kv.MaNPL = kx.MaNPL
                  WHERE kv.SoLoID = t1.SoLoID AND kv.MaNPL = t1.MaNPL
                    AND kv.DuyetQC = 1
              )
              OR EXISTS (
                  SELECT 1 FROM dbo.Qty_KiemPL_XacNhan kp
                  WHERE kp.SoLoID = t1.SoLoID AND kp.MaNPL = t1.MaNPL
                    AND kp.Is_XN_SoLo = 1
              )
          );

        SELECT @KKChoDuyet = COUNT(DISTINCT CONCAT(t1.SoLoID, '|', t1.MaNPL))
        FROM dbo.ERPPhieuKiemKe_NPL t1
        WHERE t1.IsXacNhan IS NULL;

        SELECT 'itemcode_cho_nk' AS MaCV,
               N'ItemCode chờ nhập kho' AS TenCV,
               N'ItemCode QC đã ký duyệt nhưng chưa nhập kho' AS MoTa,
               @ItemcodeChoNK AS SoLuong,
               N'ItemCode' AS DonVi,
               'clipboard-check' AS Icon
        UNION ALL
        SELECT 'kk_cho_duyet',
               N'Kiểm kê chờ duyệt',
               N'ItemCode đã kiểm kê chờ bạn duyệt kết quả',
               @KKChoDuyet,
               N'ItemCode',
               'clipboard-list';
    END

    -- ===================================================================
    -- [GetTodoDetail]
    -- Lấy chi tiết danh sách công việc chờ xử lý theo từng loại (ItemCode, Kiểm kê)
    -- ===================================================================
    ELSE IF @Action = 'GetTodoDetail'
    BEGIN
        DECLARE @TodoType NVARCHAR(50) = ISNULL(@Itemcode, 'itemcode_cho_nk');

        IF @TodoType = 'itemcode_cho_nk'
        BEGIN
        ;WITH tblKV1 AS (
            SELECT t1.KhoVaiID, CONCAT(t1.KhoVai, ' ', t2.TenDVVT) as KhoVai
            FROM ERP_KhoVai t1
            LEFT JOIN ERP_DonViVT t2 ON t1.MaDVVT = t2.MaDVVT
            UNION ALL
            SELECT MaSize as KhoVaiID, MAX(TenSize) as KhoVai
            FROM BangSize
            GROUP BY MaSize
        )
        SELECT
            ROW_NUMBER() OVER (ORDER BY t1.SoLoID, t1.MaNPL) AS STT, -- Số thứ tự
            vt.MaVT AS ItemCode,  -- Mã Item
            vt.ChiTiet AS TenVT,     -- Tên chi tiết vật tư
            t1.POMua AS POMua,     -- Số PO (Purchase Order)
            mau.MaMauVT AS MaMauVT,   -- Mã màu (Chuỗi)
            mau.MauVT AS MauVT,     -- Tên màu (Chuỗi)
            kv.KhoVai AS WidthSize, -- Khổ vải/Kích cỡ (Chuỗi)
            t1.NgayNhapKho AS NgayTao,   -- Dạng chuỗi ngày. Ngày nhập kho.
            t3.TenKH AS NCC,       -- Nhà cung cấp
            MAX(SLTong)  AS SLMua,     -- Dạng số. SL Mua (gán 0 để ẩn)
            SUM(SoLuongThucTeBanDau)  AS SLVe,      -- Dạng số. SL Về (gán 0 để ẩn)
            'itemcode_cho_nk' AS Type -- Loại dữ liệu để phân biệt các Tab trên JS
        FROM dbo.ERP_ChiTietNhapKhoNPL t1
        LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = t1.MaVTID
        LEFT JOIN dbo.ERP_MauVTTV mau ON mau.MauVTID = t1.MauVTID
        LEFT JOIN tblKV1 kv ON kv.KhoVaiID = t1.KhoVaiID
        LEFT JOIN ERP_NhapKhoNPL t2 ON t1.SoLoID = t2.SoLoID AND t1.IsNPL = t2.IsNPL
        LEFT JOIN ERP_KhachHangNK t3 ON t2.NhaCungCap = t3.MaNhaCC
        WHERE ISNULL(t1.IsDuyetNK, 0) <> 1 AND t1.NgayNhapKho IS NOT NULL
        GROUP BY vt.MaVT, vt.ChiTiet, t1.POMua, mau.MaMauVT, mau.MauVT, kv.KhoVai, t1.NgayNhapKho, t3.TenKH, t1.SoLoID, t1.MaNPL;
        END
        ELSE IF @TodoType = 'kk_cho_duyet'
        BEGIN
            WITH tblKV AS (
                SELECT t1.KhoVaiID, CONCAT(t1.KhoVai, ' ', t2.TenDVVT) as KhoVai
                FROM dbo.ERP_KhoVai t1
                LEFT JOIN dbo.ERP_DonViVT t2 ON t1.MaDVVT = t2.MaDVVT
                UNION ALL
                SELECT DISTINCT MaSize AS KhoVaiID, TenSize AS KhoVai
                FROM dbo.BangSize
            )
            SELECT
                ROW_NUMBER() OVER (ORDER BY t1.SoLoID, t1.MaNPL) AS STT,
                ISNULL(vt.MaVT, '') AS ItemCode,
                ISNULL(vt.ChiTiet, '') AS TenVT,
                ISNULL(t1.SoLoID, '') AS POMua,
                '' AS MaMauVT,
                '' AS MauVT,
                '' AS WidthSize,
                ISNULL(ds.MaDVVT, '') AS DonVi,
                CONVERT(VARCHAR(10), t1.DateKiemKe, 103) AS NgayTao,
                ROUND(ISNULL(t1.SLKiemKe, 0), 2) AS TonKho,
                ROUND(ISNULL(t1.SLKiemKeBanDau, 0), 2) AS SLKiemKe,
                ROUND(ISNULL(t1.SLKiemKeBanDau, 0) - ISNULL(t1.SLKiemKe, 0), 2) AS ChenhLech,
                'kk_cho_duyet' AS Type
            FROM dbo.ERPPhieuKiemKe_NPL t1
            LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = t1.MaVTID
            LEFT JOIN dbo.ERP_MauVTTV mau ON mau.MauVTID = t1.MauVTID
            LEFT JOIN dbo.ERP_DanhSachVatTuKiemKe ds ON t1.PhieuKiemKe = ds.PhieuVatTuKK AND t1.MaNPL = ds.MaNPL
            OUTER APPLY (
                SELECT TOP 1 ct.KhoVaiID
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.SoLoID = t1.SoLoID AND ct.MaNPL = t1.MaNPL
            ) ct_kv
            LEFT JOIN tblKV kv ON kv.KhoVaiID = ISNULL(ct_kv.KhoVaiID, ISNULL(PARSENAME(REPLACE(t1.MaNPL, '@', '.'), 1), ''))
            WHERE t1.IsXacNhan IS NULL
            ORDER BY t1.SoLoID, t1.MaNPL;
        END
        ELSE
        BEGIN
            SELECT 0 AS STT, '' AS ItemCode, '' AS TenVT, '' AS POMua,
                   '' AS MaMauVT, '' AS MauVT, '' AS WidthSize,
                   NULL AS NgayTao, '' AS NCC, 0 AS SLMua, 0 AS SLVe, '' AS Type
            WHERE 1 = 0;
        END
    END

    -- ===================================================================
    -- [GetTinhHinhKiemKe]
    -- Thống kê tổng quan tình hình kiểm kê: Số phiếu đã duyệt, chưa duyệt, chênh lệch
    -- ===================================================================
    ELSE IF @Action = 'GetTinhHinhKiemKe'
    BEGIN
        SELECT
            DaKiem,
            (Tong - DaKiem) AS ChuaKiem,
            Tong,
            CAST(ROUND(CAST(DaKiem AS FLOAT) * 100.0 / NULLIF(Tong, 0), 1) AS DECIMAL(5,1)) AS PctDaKiem
        FROM (
            SELECT
                COUNT(*) AS Tong,
                SUM(ISNULL(DaKiemBit, 0)) AS DaKiem
            FROM dbo.ERP_DanhSachVatTuKiemKe ds
            OUTER APPLY (
                SELECT TOP 1 1 AS DaKiemBit
                FROM dbo.ERPPhieuKiemKe_NPL p
                WHERE p.MaNPL = ds.MaNPL AND p.IsXacNhan = 1
            ) p_check
        ) t;
    END

    -- ===================================================================
    -- [GlobalSearchAll]
    -- Tìm kiếm toàn cục (Global Search) mã vật tư, đơn hàng, khách hàng trên hệ thống
    -- ===================================================================
    ELSE IF @Action = 'GlobalSearchAll'
    BEGIN
        DECLARE @Keyword NVARCHAR(200) = LTRIM(RTRIM(ISNULL(@Itemcode, '')));
        DECLARE @PatternStr NVARCHAR(202) = '%' + @Keyword + '%';

        CREATE TABLE #SearchResults (
            Category NVARCHAR(50),
            Title NVARCHAR(500),
            Subtitle NVARCHAR(1000),
            TargetID NVARCHAR(200),
            SortOrder INT
        );

        IF LEN(@Keyword) >= 2
        BEGIN
            INSERT INTO #SearchResults (Category, Title, Subtitle, TargetID, SortOrder)
            SELECT TOP 10
                'PO',
                ISNULL(nk.POMua, nk.SoLoID),
                N'PO: ' + ISNULL(nk.POMua, '') + N' - Lệnh nhập: ' + ISNULL(nk.SoLoID, '') + N' - Khách: ' + ISNULL(kh.TenKH, ISNULL(nk.KhachHang, '')) + N' - Dự kiến: ' + ISNULL(CONVERT(VARCHAR(10), nk.NgayNKDuKien, 103), ''),
                nk.SoLoID, 1
            FROM dbo.ERP_NhapKhoNPL nk
            LEFT JOIN dbo.KhachHang kh ON nk.MaHang = kh.MaKH OR nk.MaKH = kh.MaKH
            WHERE nk.SoLoID LIKE @PatternStr
               OR nk.POMua LIKE @PatternStr
               OR nk.MaHang LIKE @PatternStr
               OR nk.KhachHang LIKE @PatternStr
               OR kh.TenKH LIKE @PatternStr;

            INSERT INTO #SearchResults (Category, Title, Subtitle, TargetID, SortOrder)
            SELECT TOP 15
                'ITEM_RACK',
                vt.MaVT,
                N'Vật tư: ' + ISNULL(vt.ChiTiet, '') + N' | Kệ: ' + ISNULL(kv.KhoVai, N'Chưa xếp kệ') + N' - Tồn: ' + CAST(CAST(SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS DECIMAL(18,2)) AS NVARCHAR(50)),
                vt.MaVT, 2
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            INNER JOIN dbo.ERP_VatTuCBM cbm ON cbm.Barcode = ct.BarCode
            INNER JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = PARSENAME(REPLACE(ct.MaNPL, '@', '.'), 2)
            LEFT JOIN dbo.ERP_KhoVai kv ON kv.KhoVaiID = ct.KhoVaiID
            WHERE vt.MaVT LIKE @PatternStr OR vt.ChiTiet LIKE @PatternStr
            GROUP BY vt.MaVT, vt.ChiTiet, kv.KhoVai;
        END

        SELECT Category, Title, Subtitle, TargetID
        FROM #SearchResults
        ORDER BY SortOrder, Title;

        DROP TABLE #SearchResults;
    END

    -- ===================================================================
    -- [GetTop5KhachHangTonKho]
    -- Lấy Top 5 khách hàng có giá trị/số lượng vật tư tồn kho lớn nhất
    -- ===================================================================
    ELSE IF @Action = 'GetTop5KhachHangTonKho'
    BEGIN




        ;WITH CTE AS (
            SELECT
                ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'Khách trống')) AS KhachHang,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS GiaTriTon
            FROM ERP_ChiTietNhapKhoNPL ct
            INNER JOIN ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
            LEFT JOIN KhachHang kh ON nk.MaKH = kh.MaKH
            WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode = ct.BarCode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang   s WHERE s.BarCode = ct.BarCode)
            GROUP BY ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'Khách trống'))
        ),
        TotalCTE AS (
            SELECT SUM(GiaTriTon) AS TotalValue FROM CTE
        )
        SELECT TOP 5
            ROW_NUMBER() OVER(ORDER BY c.GiaTriTon DESC) AS STT,
            c.KhachHang,
            c.GiaTriTon AS GiaTri,
            CASE WHEN ISNULL(t.TotalValue, 0) = 0 THEN 0.0 ELSE ROUND((c.GiaTriTon / t.TotalValue) * 100, 2) END AS TyTrong
        FROM CTE c
        CROSS JOIN TotalCTE t
        ORDER BY c.GiaTriTon DESC;



    END

    -- ===================================================================
    -- [GetKhachHangTonKhoChiTiet]
    -- Lấy danh sách chi tiết vật tư tồn kho của một khách hàng cụ thể
    -- ===================================================================
    ELSE IF @Action = 'GetKhachHangTonKhoChiTiet'
    BEGIN




        ;WITH CTE AS (
            SELECT
                ISNULL(nk.MaKH, '') AS MaKH,
                ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'Khách trống')) AS KhachHang,
                COUNT(DISTINCT ct.MaVTID) AS SoMaVT,
                SUM(ISNULL(cbm_agg.CBM, 0)) AS TongCBM,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS GiaTri
            FROM ERP_ChiTietNhapKhoNPL ct
            INNER JOIN ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
            LEFT JOIN KhachHang kh ON nk.MaKH = kh.MaKH
            LEFT JOIN (SELECT Barcode, MAX(ISNULL(CBM, 0)) AS CBM FROM ERP_VatTuCBM WHERE MaONPL IS NOT NULL GROUP BY Barcode) cbm_agg
                   ON ct.BarCode = cbm_agg.Barcode
            WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode = ct.BarCode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang   s WHERE s.BarCode = ct.BarCode)
            GROUP BY ISNULL(nk.MaKH, ''), ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'Khách trống'))
        ),
        TotalCTE AS (
            SELECT SUM(GiaTri) AS TotalValue FROM CTE
        )
        SELECT
            ROW_NUMBER() OVER(ORDER BY c.GiaTri DESC) AS STT,
            c.MaKH,
            c.KhachHang,
            c.SoMaVT,
            c.TongCBM,
            c.GiaTri,
            CASE WHEN ISNULL(t.TotalValue, 0) = 0 THEN 0.0 ELSE ROUND((c.GiaTri / t.TotalValue) * 100, 2) END AS TyTrong
        FROM CTE c
        CROSS JOIN TotalCTE t
        ORDER BY c.GiaTri DESC;



    END

    -- ===================================================================
    -- [GetGiaTriTonKhoTheoNhom]
    -- Thống kê giá trị tồn kho theo nhóm nguyên phụ liệu
    -- ===================================================================
    ELSE IF @Action = 'GetGiaTriTonKhoTheoNhom'
    BEGIN




        ;WITH BaseData AS (
            SELECT
                ISNULL(nh.TenNhom, N'Khác') AS Nhom,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(CAST(ct.DonGia AS DECIMAL(20,4)), 0)) AS GiaTri
            FROM ERP_ChiTietNhapKhoNPL ct
            LEFT JOIN (SELECT MaCLVT, MAX(TenNhom) AS TenNhom FROM NhomNguyenPhuLieu GROUP BY MaCLVT) nh ON ct.MaNhom = nh.MaCLVT
            WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode = ct.BarCode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang   s WHERE s.BarCode = ct.BarCode)
            GROUP BY ISNULL(nh.TenNhom, N'Khác')
        ),
        RankedData AS (
            SELECT
                Nhom,
                GiaTri,
                ROW_NUMBER() OVER (ORDER BY GiaTri DESC) AS RN
            FROM BaseData
        ),
        TopData AS (
            SELECT Nhom, GiaTri FROM RankedData WHERE RN <= 5
            UNION ALL
            SELECT N'Khác' AS Nhom, SUM(GiaTri) AS GiaTri
            FROM RankedData WHERE RN > 5 HAVING SUM(GiaTri) > 0
        ),
        TotalCTE AS (
            SELECT SUM(GiaTri) AS TotalValue FROM TopData
        )
        SELECT
            c.Nhom,
            c.GiaTri,
            CASE WHEN ISNULL(t.TotalValue, 0) = 0 THEN 0.0 ELSE ROUND((c.GiaTri / t.TotalValue) * 100, 2) END AS TyTrong
        FROM TopData c
        CROSS JOIN TotalCTE t
        ORDER BY CASE WHEN c.Nhom = N'Khác' THEN 1 ELSE 0 END, c.GiaTri DESC;



    END

    -- ===================================================================
    -- [GetGiaTriNhomChiTiet]
    -- Lấy danh sách chi tiết vật tư tồn kho thuộc một nhóm nguyên phụ liệu cụ thể
    -- ===================================================================
    ELSE IF @Action = 'GetGiaTriNhomChiTiet'
    BEGIN




        ;WITH RawDetails AS (
            SELECT
                ISNULL(nh.TenNhom, N'Khác') AS ParentNhom,
                ISNULL(vt.MaVT, '') AS MaVT,
                ISNULL(vt.ChiTiet, '') AS Nhom,
                ISNULL(m.MaMauVT, '') AS MaMauVT,
                ISNULL(m.MauVT, '') AS MauVT,
                ISNULL(k.KhoVai, '') AS WidthSize,
                ISNULL(d.TenDVVT, '') AS DonVi,
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(CAST(ct.DonGia AS DECIMAL(20,4)), 0)) AS GiaTri,
                SUM(ISNULL(cbm_agg.CBM, 0)) AS TongCBM
            FROM ERP_ChiTietNhapKhoNPL ct
            LEFT JOIN (SELECT Barcode, MAX(ISNULL(CBM, 0)) AS CBM FROM ERP_VatTuCBM WHERE MaONPL IS NOT NULL GROUP BY Barcode) cbm_agg
                   ON ct.BarCode = cbm_agg.Barcode
            LEFT JOIN (SELECT MaCLVT, MAX(TenNhom) AS TenNhom FROM NhomNguyenPhuLieu GROUP BY MaCLVT) nh ON ct.MaNhom = nh.MaCLVT
            LEFT JOIN ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
            LEFT JOIN ERP_MauVTTV m ON m.MauVTID = ISNULL(PARSENAME(REPLACE(ct.MaNPL, '@', '.'), 2), '')
            LEFT JOIN ERP_KhoVai k ON k.KhoVaiID = ISNULL(PARSENAME(REPLACE(ct.MaNPL, '@', '.'), 1), '')
            LEFT JOIN ERP_DonViVT d ON d.MaDVVT = k.MaDVVT
            WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
              AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode = ct.BarCode)
              AND NOT EXISTS (SELECT 1 FROM #TempSoanHang   s WHERE s.BarCode = ct.BarCode)
            GROUP BY
                ISNULL(nh.TenNhom, N'Khác'),
                vt.MaVT,
                ISNULL(vt.ChiTiet, ''),
                m.MaMauVT,
                m.MauVT,
                k.KhoVai,
                d.TenDVVT
        ),
        TotalValue AS (
            SELECT SUM(GiaTri) AS TotalGiaTri FROM RawDetails
        ),
        RawGroups AS (
            SELECT
                ParentNhom AS Nhom,
                SUM(GiaTri) AS GiaTri,
                COUNT(DISTINCT MaVT) AS SoMaVT,
                SUM(TongCBM) AS TongCBM,
                CASE WHEN (SELECT TotalGiaTri FROM TotalValue) > 0 THEN ROUND(SUM(GiaTri) / (SELECT TotalGiaTri FROM TotalValue) * 100, 2) ELSE 0 END AS TyTrong
            FROM RawDetails
            GROUP BY ParentNhom
        )
        SELECT
            1 AS IsGroup,
            Nhom,
            ROW_NUMBER() OVER (ORDER BY CASE WHEN Nhom = N'Khác' THEN 1 ELSE 0 END, GiaTri DESC) AS STT,
            SoMaVT,
            TongCBM,
            GiaTri,
            TyTrong,
            NULL AS ParentNhom,
            NULL AS MaVT,
            NULL AS MaMauVT,
            NULL AS MauVT,
            NULL AS WidthSize,
            NULL AS DonVi
        FROM RawGroups
        UNION ALL
        SELECT
            0 AS IsGroup,
            Nhom,
            NULL AS STT,
            NULL AS SoMaVT,
            TongCBM,
            GiaTri,
            NULL AS TyTrong,
            ParentNhom,
            MaVT,
            MaMauVT,
            MauVT,
            WidthSize,
            DonVi
        FROM RawDetails
        ORDER BY IsGroup DESC, STT ASC, ParentNhom ASC, GiaTri DESC;
    END

    -- ===================================================================
    -- [GetKiemKeChiTiet]
    -- Lấy chi tiết vật tư, số lượng thực tế vs số lượng kiểm kê của một phiếu kiểm kê
    -- ===================================================================
    ELSE IF @Action = 'GetKiemKeChiTiet'
    BEGIN
        SELECT
            ds.PhieuVatTuKK AS MaPhieu,
            MIN(ds.NgayTao) AS NgayBatDau,
            CASE WHEN MIN(ds.IsNPL) = 1 THEN N'Kho Nguyên Liệu' ELSE N'Kho Phụ Liệu' END AS KhuVuc,
            COUNT(*) AS SoMa,
            SUM(ISNULL(p_check.DaKiemBit, 0)) AS DaKiem,
            COUNT(*) AS Tong,
            CAST(ROUND(SUM(ISNULL(p_check.DaKiemBit, 0)) * 100.0 / NULLIF(COUNT(*), 0), 1) AS DECIMAL(5, 1)) AS Pct,
            MAX(ISNULL(ds.NguoiTao, '')) AS NguoiPT,
            CASE
                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = 0 THEN 3
                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = COUNT(*) THEN 1
                ELSE 2
            END AS Status,
            CASE
                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = 0 THEN N'Chưa kiểm'
                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = COUNT(*) THEN N'Đã kiểm đủ'
                ELSE N'Đang kiểm'
            END AS TrangThai
        FROM dbo.ERP_DanhSachVatTuKiemKe ds
        OUTER APPLY (
            SELECT TOP 1 1 AS DaKiemBit
            FROM dbo.ERPPhieuKiemKe_NPL p
            WHERE p.PhieuKiemKe = ds.PhieuVatTuKK AND p.MaNPL = ds.MaNPL AND p.IsXacNhan = 1
        ) p_check
        GROUP BY ds.PhieuVatTuKK
        ORDER BY ds.PhieuVatTuKK;
    END

    -- ===================================================================
    -- [GetAgeStock]
    -- Báo cáo tuổi tồn kho (Aging Report) theo tháng/năm nhập
    -- ===================================================================
    ELSE IF @Action = 'GetAgeStock'
    BEGIN
        SELECT
            ct.MaNhom,
            ISNULL(nhom.ChungLoaiVatTu, N'Khác') AS TenNhom,
            CAST(ct.IsNPL AS INT)              AS NPL,
            ROUND(SUM(ISNULL(ct.SoLuongThucTe, 0)), 2) AS TonKhoCT,
            YEAR(ct.NgayNhapKho)               AS Nam,
            MONTH(ct.NgayNhapKho)              AS Thang
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        LEFT JOIN dbo.ChungLoaiVatTu nhom ON nhom.MaCLVT = ct.MaNhom
        WHERE ct.IsNK = 1
          AND ct.MaNhom IS NOT NULL
          AND ISNULL(ct.SoLuongThucTe, 0) > 0
          AND EXISTS (SELECT 1 FROM dbo.ERP_VatTuCBM v WHERE v.Barcode = ct.BarCode AND v.MaONPL IS NOT NULL)
        GROUP BY
            ct.MaNhom,
            nhom.ChungLoaiVatTu,
            ct.IsNPL,
            YEAR(ct.NgayNhapKho),
            MONTH(ct.NgayNhapKho)
        HAVING ROUND(SUM(ISNULL(ct.SoLuongThucTe, 0)), 2) > 0
        ORDER BY Nam DESC, Thang DESC, TonKhoCT DESC;
    END

    -- ===================================================================
    -- [GetTonKhoTheoKy]
    -- Báo cáo xuất nhập tồn kho tổng quát trong một kỳ (Từ ngày - Đến ngày)
    -- ===================================================================
    ELSE IF @Action = 'GetTonKhoTheoKy'
    BEGIN
        DECLARE @ParaTu   DATE = ISNULL(TRY_CONVERT(DATE, @TuNgay),  '1900-01-01');
        DECLARE @ParaDen  DATE = ISNULL(TRY_CONVERT(DATE, @DenNgay), '2900-01-01');

        SELECT DISTINCT
            ct.MaNPL,
            ct.SoLoID,
            ct.BarCode,
            CAST(ISNULL(ct.SoLuongThucTeBanDau, 0) AS DECIMAL(18,4)) AS SoLuong,
            TRY_CONVERT(DATE, ct.NgayNhapKho) AS NgayNhapKho
        INTO #BaseTK
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode
        INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL
        WHERE v.MaONPL IS NOT NULL
          AND o.Module IN (1, 2)
          AND (ISNULL(@LoaiNPL, 0) = 0 OR o.Module = @LoaiNPL)
          AND (ISNULL(@Itemcode, '') = '' OR ct.MaNPL LIKE '%' + @Itemcode + '%')
          AND ct.SoLuongThucTeBanDau > 0
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi x WHERE x.BarCode = ct.BarCode)
          AND NOT EXISTS (SELECT 1 FROM #TempSoanHang s WHERE s.BarCode = ct.BarCode);



        SELECT TOP (0) CAST(N'' AS NVARCHAR(500)) AS MaNPL, CAST(0.0 AS DECIMAL(18,4)) AS SLDaSoan
        INTO #SoanHang_TK;
        IF OBJECT_ID('dbo.ERP_SoanHangNPL_BarCode', 'U') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            BEGIN TRY
                INSERT INTO #SoanHang_TK (MaNPL, SLDaSoan)
                EXEC sp_executesql N'
                    SELECT sh.MaNPL, SUM(ISNULL(sh.SLSoanHang_BC, 0))
                    FROM dbo.ERP_SoanHangNPL_BarCode sh
                    INNER JOIN #BaseTK b ON sh.BarCode = b.BarCode
                    GROUP BY sh.MaNPL;';
            END TRY BEGIN CATCH END CATCH
        END

        ;WITH ChenhLechDK_TK AS (
            SELECT t1.SoLoID, t1.BarCode,
                   ISNULL(t1.SLKiemKeBanDau, 0) - ISNULL(t1.SLKiemKeEdit, ISNULL(t1.SLKiemKe, 0)) AS SLChenhLech
            FROM dbo.ERPPhieuKiemKe_NPL t1
            WHERE t1.IsXacNhan = 1 AND t1.SLKiemKeEdit IS NOT NULL
              AND TRY_CONVERT(DATE, t1.DateDuyetKK) < @ParaTu
        ),
        ChenhLechTK_TK AS (
            SELECT t1.SoLoID, t1.BarCode,
                   ISNULL(t1.SLKiemKeBanDau, 0) - ISNULL(t1.SLKiemKeEdit, ISNULL(t1.SLKiemKe, 0)) AS SLChenhLech
            FROM dbo.ERPPhieuKiemKe_NPL t1
            WHERE t1.IsXacNhan = 1 AND t1.SLKiemKeEdit IS NOT NULL
              AND TRY_CONVERT(DATE, t1.DateDuyetKK) BETWEEN @ParaTu AND @ParaDen
        ),
        NhapDK_TK AS (
            SELECT b.MaNPL,
                   SUM(b.SoLuong - ISNULL(dk.SLChenhLech, 0)) AS SLNhapDK
            FROM #BaseTK b
            LEFT JOIN ChenhLechDK_TK dk ON b.SoLoID = dk.SoLoID AND b.BarCode = dk.BarCode
            WHERE b.NgayNhapKho < @ParaTu
            GROUP BY b.MaNPL
        ),
        XuatDK_TK AS (
            SELECT b.MaNPL, SUM(ISNULL(xh.SLNhap, 0)) AS SLXuatDK
            FROM dbo.PhieuXuatHang xh
            INNER JOIN #BaseTK b ON xh.BarCodeGoc = b.BarCode
            WHERE TRY_CONVERT(DATE, xh.NgayXuatHang) < @ParaTu
            GROUP BY b.MaNPL
        ),
        ThuDK_TK AS (
            SELECT b.MaNPL, SUM(ISNULL(th.ThuHoi, 0)) AS SLThuDK
            FROM dbo.PhieuThuHoiNPL th
            INNER JOIN dbo.PhieuXuatHang xh ON th.BarCode = xh.BarCode
            INNER JOIN #BaseTK b ON xh.BarCodeGoc = b.BarCode
            WHERE TRY_CONVERT(DATE, th.NgayTH) < @ParaTu
            GROUP BY b.MaNPL
        ),
        NhapTK_TK AS (
            SELECT b.MaNPL,
                   SUM(b.SoLuong - ISNULL(tk.SLChenhLech, 0)) AS SLNhapTK
            FROM #BaseTK b
            LEFT JOIN ChenhLechTK_TK tk ON b.SoLoID = tk.SoLoID AND b.BarCode = tk.BarCode
            WHERE b.NgayNhapKho BETWEEN @ParaTu AND @ParaDen
            GROUP BY b.MaNPL
        ),
        XuatTK_TK AS (
            SELECT b.MaNPL, SUM(ISNULL(xh.SLNhap, 0)) AS SLXuatTK
            FROM dbo.PhieuXuatHang xh
            INNER JOIN #BaseTK b ON xh.BarCodeGoc = b.BarCode
            WHERE TRY_CONVERT(DATE, xh.NgayXuatHang) BETWEEN @ParaTu AND @ParaDen
            GROUP BY b.MaNPL
        ),
        ThuTK_TK AS (
            SELECT b.MaNPL, SUM(ISNULL(th.ThuHoi, 0)) AS SLThuTK
            FROM dbo.PhieuThuHoiNPL th
            INNER JOIN dbo.PhieuXuatHang xh ON th.BarCode = xh.BarCode
            INNER JOIN #BaseTK b ON xh.BarCodeGoc = b.BarCode
            WHERE TRY_CONVERT(DATE, th.NgayTH) BETWEEN @ParaTu AND @ParaDen
            GROUP BY b.MaNPL
        ),
        Loi_TK AS (
            SELECT b.MaNPL, SUM(b.SoLuong) AS SLLoi
            FROM #BaseTK b
            WHERE EXISTS (
                SELECT 1 FROM dbo.ERP_VatTuCBM vt
                INNER JOIN dbo.ERP_ONPL   o2 ON o2.TenO   = vt.MaONPL
                INNER JOIN dbo.ERP_DayNPL d2 ON d2.DayID  = o2.DayID AND d2.Status = 2
                WHERE vt.Barcode = b.BarCode AND vt.MaONPL IS NOT NULL
            )
            GROUP BY b.MaNPL
        )
        SELECT DISTINCT
            b.MaNPL,
            ISNULL(PARSENAME(REPLACE(b.MaNPL, '@', '.'), 3), '')         AS MaVTID,
            ROUND(ISNULL(ndk.SLNhapDK, 0), 4)                            AS NKDK,
            ROUND(ISNULL(xdk.SLXuatDK, 0), 4)                            AS XHDK,
            ROUND(ISNULL(tdk.SLThuDK,  0), 4)                            AS THDK,
            ROUND(ISNULL(ntk.SLNhapTK, 0), 4)                            AS SLNK,
            ROUND(ISNULL(xtk.SLXuatTK, 0), 4)                            AS SLXH,
            ROUND(ISNULL(ttk.SLThuTK,  0), 4)                            AS SLTH,
            ROUND(ISNULL(sh.SLDaSoan,  0), 4)                            AS SLSoanHang,
            ROUND(ISNULL(lo.SLLoi,     0), 4)                            AS SLLoi,

            ROUND(  ISNULL(ndk.SLNhapDK, 0)
                  - ISNULL(xdk.SLXuatDK, 0)
                  + ISNULL(tdk.SLThuDK,  0), 4)                          AS TonKhoDK,

            ROUND(  ISNULL(ndk.SLNhapDK, 0) + ISNULL(ntk.SLNhapTK, 0)
                  - ISNULL(xdk.SLXuatDK, 0) - ISNULL(xtk.SLXuatTK, 0)
                  + ISNULL(tdk.SLThuDK,  0) + ISNULL(ttk.SLThuTK,  0)
                  - ISNULL(sh.SLDaSoan,  0), 4)                          AS TonKho
        FROM #BaseTK b
        LEFT JOIN NhapDK_TK  ndk ON b.MaNPL = ndk.MaNPL
        LEFT JOIN XuatDK_TK  xdk ON b.MaNPL = xdk.MaNPL
        LEFT JOIN ThuDK_TK   tdk ON b.MaNPL = tdk.MaNPL
        LEFT JOIN NhapTK_TK  ntk ON b.MaNPL = ntk.MaNPL
        LEFT JOIN XuatTK_TK  xtk ON b.MaNPL = xtk.MaNPL
        LEFT JOIN ThuTK_TK   ttk ON b.MaNPL = ttk.MaNPL
        LEFT JOIN #SoanHang_TK sh  ON b.MaNPL = sh.MaNPL
        LEFT JOIN Loi_TK      lo  ON b.MaNPL = lo.MaNPL
        ORDER BY TonKho DESC;

        DROP TABLE #BaseTK;
        DROP TABLE #SoanHang_TK;

    END

    -- ===================================================================
    -- [GetTonDauKyChiTiet]
    -- Lấy chi tiết tồn kho đầu kỳ của các vật tư
    -- ===================================================================
    ELSE IF @Action = 'GetTonDauKyChiTiet'
    BEGIN
        DECLARE @LoaiTDK NVARCHAR(10);
        SET @LoaiTDK = ISNULL(@Loai, 'all');

        ;WITH tempTKho_TDK AS (
            SELECT t1.ID, t1.SoloID, t1.MaNPL, t1.IsNPL, t1.BarCode, t1.SoLuongThucTeBanDau, t1.NgayNhapKho, t1.DonGia, t1.MaDVVT
            FROM dbo.ERP_ChiTietNhapKhoNPL t1
            WHERE t1.SoLuongThucTeBanDau <> 0
              AND EXISTS (
                  SELECT 1 FROM dbo.ERP_VatTuCBM t9
                  WHERE t1.BarCode = t9.Barcode
                    AND (ISNULL(t9.MaONPL, '') <> ''
                         OR EXISTS (SELECT 1 FROM dbo.PhieuXuatHang px WHERE px.BarCodeGoc = t9.Barcode)
                         OR EXISTS (SELECT 1 FROM dbo.PhieuThuHoiNPL th WHERE th.BarCode = t9.Barcode))
              )
        ),
        tempNhapKhoDK_TDK AS (
            SELECT t1.MaNPL, SUM(t1.SoLuongThucTeBanDau) AS SLNhapDK
            FROM tempTKho_TDK t1
            WHERE Convert(date, t1.NgayNhapKho) < @TuNgay
            GROUP BY t1.MaNPL
        ),
        tempXuatHangDK_TDK AS (
            SELECT t1.MaNPL, SUM(t1.SLNhap) AS SLXuatDK
            FROM dbo.PhieuXuatHang t1
            INNER JOIN tempTKho_TDK t2 ON t1.BarCodeGoc = t2.BarCode
            WHERE Convert(date, t1.NgayXuatHang) < @TuNgay
              AND ISNULL(t1.MaHang, '') NOT LIKE '%PSH_%'
            GROUP BY t1.MaNPL
        ),
        tempThuHoiDK_TDK AS (
            SELECT t2.MaNPL, SUM(t1.ThuHoi) AS SLThuDK
            FROM dbo.PhieuThuHoiNPL t1
            LEFT JOIN dbo.PhieuXuatHang t2 ON t1.BarCode = t2.BarCode
            WHERE Convert(date, t1.NgayTH) < @TuNgay
            GROUP BY t2.MaNPL
        ),
        tempChenhLechDK_TDK AS (
            SELECT t2.MaNPL, ROUND(SUM(ISNULL(t1.SLKiemKeBanDau, 0) - ISNULL(t1.SLKiemKeEdit, 0)), 4) AS SLChenhLenhDK
            FROM dbo.ERPPhieuKiemKe_NPL t1
            INNER JOIN tempTKho_TDK t2 ON t1.BarCode = t2.BarCode
            WHERE t1.IsXacNhan = 1
              AND t1.SLKiemKeEdit IS NOT NULL
              AND Convert(date, t2.NgayNhapKho) < @TuNgay
            GROUP BY t2.MaNPL
        ),
        tempUnique_TDK AS (
            SELECT DISTINCT MaNPL
            FROM tempTKho_TDK
        ),
        tempRollInfo_TDK AS (
            SELECT t1.MaNPL, COUNT(DISTINCT t1.BarCode) AS SoRoll, MAX(t1.DonGia) AS DonGia
            FROM tempTKho_TDK t1
            WHERE Convert(date, t1.NgayNhapKho) < @TuNgay
            GROUP BY t1.MaNPL
        ),
        tempAgg_TDK AS (
            SELECT
                p.MaNPL,
                ROUND(
                    ISNULL(n.SLNhapDK, 0)
                    - ISNULL(x.SLXuatDK, 0)
                    + ISNULL(t.SLThuDK, 0)
                    - ISNULL(c.SLChenhLenhDK, 0), 4
                ) AS SLTonDau,
                ISNULL(ri.SoRoll, 0) AS SoRoll,
                ISNULL(ri.DonGia, 0) AS DonGia
            FROM tempUnique_TDK p
            LEFT JOIN tempNhapKhoDK_TDK n ON p.MaNPL = n.MaNPL
            LEFT JOIN tempXuatHangDK_TDK x ON p.MaNPL = x.MaNPL
            LEFT JOIN tempThuHoiDK_TDK t ON p.MaNPL = t.MaNPL
            LEFT JOIN tempChenhLechDK_TDK c ON p.MaNPL = c.MaNPL
            LEFT JOIN tempRollInfo_TDK ri ON p.MaNPL = ri.MaNPL
            WHERE (ISNULL(n.SLNhapDK, 0)
                   - ISNULL(x.SLXuatDK, 0)
                   + ISNULL(t.SLThuDK, 0)
                   - ISNULL(c.SLChenhLenhDK, 0)) > 0
        ),
        tempParsed_TDK AS (
            SELECT
                r.*,
                ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,
                ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID,
                ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,
                ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID
            FROM tempAgg_TDK r
            WHERE (@LoaiTDK = 'all'
                   OR (@LoaiTDK = 'nl' AND r.MaNPL LIKE '%NL%')
                   OR (@LoaiTDK = 'pl' AND r.MaNPL NOT LIKE '%NL%'))
        )
        SELECT
            p.MaNPL,
            CASE WHEN ISNULL(ct_npl.IsNPL, 0) = 1 THEN N'NL' ELSE N'PL' END AS LoaiKho,
            ISNULL(vt.MaVT, p.MaVTID) AS ItemCode,
            ISNULL(vt.ChiTiet, p.MaVTID) AS TenVT,
            ISNULL(mv.MauVT, '') AS MauVT,
            ISNULL(kv.KhoVai, '') + CASE WHEN ISNULL(dv.TenDVVT, '') <> '' THEN ' ' + dv.TenDVVT ELSE '' END AS KhoVai,
            ISNULL(dv.TenDVVT, '') AS DonVi,
            p.SLTonDau,
            p.SoRoll,
            p.DonGia,
            CAST(p.SLTonDau * p.DonGia AS DECIMAL(18,2)) AS ThanhTien
        FROM tempParsed_TDK p
        LEFT JOIN dbo.ERP_VatTuTV vt ON p.MaVTID = vt.MaVTID
        LEFT JOIN dbo.ERP_MauVTTV mv ON p.MauVTID = mv.MauVTID
        LEFT JOIN dbo.ERP_KhoVai kv ON p.KhoVaiID = kv.KhoVaiID
        LEFT JOIN (
            SELECT MaNPL, MAX(CAST(IsNPL AS INT)) AS IsNPL, MAX(MaDVVT) AS MaDVVT
            FROM dbo.ERP_ChiTietNhapKhoNPL
            GROUP BY MaNPL
        ) ct_npl ON p.MaNPL = ct_npl.MaNPL
        LEFT JOIN dbo.ERP_DonViVT dv ON ct_npl.MaDVVT = dv.MaDVVT
        ORDER BY p.SLTonDau DESC;
    END

    -- ===================================================================
    -- [GetTonDauKyRollDetail]
    -- Lấy chi tiết tồn kho đầu kỳ theo từng cuộn/kiện (Roll/Barcode)
    -- ===================================================================
    ELSE IF @Action = 'GetTonDauKyRollDetail'
    BEGIN
        DECLARE @MaNPL_Filter NVARCHAR(500);
        SET @MaNPL_Filter = @Loai;

        SELECT
            ct.BarCode,
            ct.SoKienHienThi,
            ct.SoLuongThucTeBanDau AS SLTonDau,
            ct.SoLuongThucTe,
            ct.DonGia,
            ct.TienTe
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        WHERE ct.MaNPL = @MaNPL_Filter
          AND ct.SoLuongThucTeBanDau > 0
          AND ct.NgayNhapKho < @TuNgay
          AND EXISTS (
              SELECT 1 FROM dbo.ERP_VatTuCBM t9
              WHERE ct.BarCode = t9.Barcode
                AND (ISNULL(t9.MaONPL, '') <> ''
                     OR EXISTS (SELECT 1 FROM dbo.PhieuXuatHang px WHERE px.BarCodeGoc = t9.Barcode)
                     OR EXISTS (SELECT 1 FROM dbo.PhieuThuHoiNPL th WHERE th.BarCode = t9.Barcode))
          )
        ORDER BY ct.BarCode;
    END

    ELSE
    BEGIN
        SELECT 'Unknown action: ' + ISNULL(@Action, 'NULL') AS Error;
    END
END
GO

IF OBJECT_ID('dbo.ERP_LichPhanCongPhuLieu_Task', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ERP_LichPhanCongPhuLieu_Task (
        TaskID          INT           IDENTITY(1,1) PRIMARY KEY,
        MaLenhSX        NVARCHAR(100) NOT NULL,
        MaNV            NVARCHAR(50)  NULL,
        TenNV           NVARCHAR(200) NULL,
        TrangThai       INT           NOT NULL DEFAULT 0,   -- 0=Chờ, 1=Đang làm, 2=Hoàn thành, 3=Chưa HT
        NgayThucHien    DATETIME      NULL,
        GhiChu          NVARCHAR(500) NULL,
        CreatedAt       DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdateAt        DATETIME      NULL
    );
    PRINT 'Table ERP_LichPhanCongPhuLieu_Task created.';
END
ELSE
BEGIN
    IF COL_LENGTH('dbo.ERP_LichPhanCongPhuLieu_Task', 'UpdateAt') IS NULL
        ALTER TABLE dbo.ERP_LichPhanCongPhuLieu_Task ADD UpdateAt DATETIME NULL;
    IF COL_LENGTH('dbo.ERP_LichPhanCongPhuLieu_Task', 'GhiChu') IS NULL
        ALTER TABLE dbo.ERP_LichPhanCongPhuLieu_Task ADD GhiChu NVARCHAR(500) NULL;
    IF COL_LENGTH('dbo.ERP_LichPhanCongPhuLieu_Task', 'CreatedAt') IS NULL
        ALTER TABLE dbo.ERP_LichPhanCongPhuLieu_Task ADD CreatedAt DATETIME NULL DEFAULT GETDATE();
    PRINT 'Table ERP_LichPhanCongPhuLieu_Task already exists — columns checked.';
END
GO

IF OBJECT_ID('dbo.SP_LICH_PHAN_CONG_PHU_LIEU', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_LICH_PHAN_CONG_PHU_LIEU;
GO
-- Must be deployed to PMS_QLDH_VIKING_2025, not master.
-- Procedure name starts with SP_; missing proc may resolve to master and return wrong data.

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_LICH_PHAN_CONG_PHU_LIEU]
    @Action         NVARCHAR(50),
    @TuNgay         DATETIME = NULL,
    @DenNgay        DATETIME = NULL,
    @Ngay           DATETIME = NULL,
    @NgayThucHien   DATETIME = NULL,
    @MaLenhSX       NVARCHAR(100) = NULL,
    @MaNV           NVARCHAR(50)  = NULL,
    @TenNV          NVARCHAR(200) = NULL,
    @TrangThai      INT           = NULL,
    @GhiChu         NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- ===================================================================
    -- [GetCalendarMonth]
    -- Lấy dữ liệu hiển thị lịch phân công phụ liệu theo tháng (Lệnh giao việc & Phiếu soạn hàng)
    -- ===================================================================
    IF @Action = 'GetCalendarMonth'
    BEGIN
        -- [OPTIMIZED] Tính trước số lượng thiếu NPL của toàn bộ phiếu soạn hàng (Fast Indexing & Aggregation)
        SELECT
            sub1.MaLenhSX,
            CONVERT(DATE, sub1.NgaySoanHang) AS NgaySoanHang,
            sub1.NguoiSoanHang,
            MAX(sub1.SLCapPhat) AS CapPhat,
            ISNULL(SUM(sub2.SLSoanHang_BC), 0) AS SoanHang
        INTO #tblSoanHangTmp
        FROM ERP_PhieuSoanHangVatTu sub1
        LEFT JOIN ERP_SoanHangNPL_BarCode sub2 ON sub1.PhieuSH = sub2.PhieuSH AND sub1.MaNPL = sub2.MaNPL
        WHERE sub1.NgaySoanHang >= @TuNgay AND sub1.NgaySoanHang < DATEADD(DAY, 1, @DenNgay)
        GROUP BY sub1.MaLenhSX, CONVERT(DATE, sub1.NgaySoanHang), sub1.NguoiSoanHang, sub1.MaNPL;

        SELECT
            MaLenhSX,
            NgaySoanHang,
            NguoiSoanHang,
            SUM(CapPhat) - SUM(SoanHang) AS SoLuongThieu
        INTO #tblSoanHangThieu
        FROM #tblSoanHangTmp
        GROUP BY MaLenhSX, NgaySoanHang, NguoiSoanHang;

        ;WITH tblDVSX_Month AS (
            SELECT
                t1.MaLenhSanXuat, t1.MaLenh, kh.TenKH,
                STUFF((
                    SELECT DISTINCT ', ' + hh.TenHang
                    FROM CanDoiDonViSanXuat t2
                    INNER JOIN DonHangTong dh2 ON t2.MaDH = dh2.MaDH
                    INNER JOIN HangHoa hh ON dh2.MaKH = hh.MaKH AND dh2.MaHang = hh.MaHang
                    WHERE t2.MaLenhSanXuat = t1.MaLenhSanXuat AND t2.MaLenh = t1.MaLenh
                    FOR XML PATH('')
                ),1,2,'') AS TenHang
            FROM CanDoiDonViSanXuat t1
            INNER JOIN DonHangTong dh ON t1.MaDH = dh.MaDH
            INNER JOIN KhachHang kh ON dh.MaKH = kh.MaKH
            GROUP BY t1.MaLenhSanXuat, t1.MaLenh, kh.TenKH
        ),
        UnifiedTasks AS (
            SELECT
                CONVERT(DATE, ISNULL(t1.NgayXuatHang, t1.NgayGiaoViec)) AS NgayLam,
                t1.MaLenhSX,
                0 AS CoCanhBao, 0 AS ThieuNPL,
                '' AS MaNV, t1.NguoiGiaoViec AS TenNV,
                CASE
                    WHEN t1.NgayXuatHang IS NULL THEN 0
                    WHEN CONVERT(DATE, t1.NgayXuatHang) <= CONVERT(DATE, t1.NgayGiaoViec) THEN 2
                    ELSE 3
                END AS TrangThai,
                'TASK' AS GhiChu
            FROM ERP_KeHachGiaoViecNL t1
            WHERE (t1.NgayXuatHang >= CAST(@TuNgay AS DATE) AND t1.NgayXuatHang < DATEADD(day, 1, CAST(@DenNgay AS DATE)))
               OR (t1.NgayXuatHang IS NULL AND t1.NgayGiaoViec >= CAST(@TuNgay AS DATE) AND t1.NgayGiaoViec < DATEADD(day, 1, CAST(@DenNgay AS DATE)))
            UNION ALL
            SELECT
                t1.NgaySoanHang AS NgayLam,
                t1.MaLenhSX,
                0 AS CoCanhBao,
                CASE WHEN ROUND(t1.SoLuongThieu, 2) > 0 THEN 1 ELSE 0 END AS ThieuNPL,
                '' AS MaNV, t1.NguoiSoanHang AS TenNV,
                CASE WHEN ROUND(t1.SoLuongThieu, 2) > 0 THEN 3 ELSE 2 END AS TrangThai,
                'PICK' AS GhiChu
            FROM #tblSoanHangThieu t1
        )
        SELECT
            u.NgayLam,
            ISNULL(d.MaLenh, u.MaLenhSX) AS MaLenhSX,
            d.TenKH AS MaKhachHang,
            d.TenHang AS MaHang,
            u.CoCanhBao, u.ThieuNPL,
            u.MaNV, u.TenNV,
            u.TrangThai, u.GhiChu
        FROM UnifiedTasks u
        LEFT JOIN tblDVSX_Month d ON u.MaLenhSX = d.MaLenhSanXuat;

        RETURN;
    END

    -- ===================================================================
    -- [GetDayDetail]
    -- Lấy chi tiết các công việc, lệnh phân công và danh sách phiếu soạn phụ liệu trong một ngày cụ thể
    -- ===================================================================
    IF @Action = 'GetDayDetail'
    BEGIN
        -- Bảng tạm: map Lệnh SX → TenKH, TenHang (đã tối ưu: chỉ lấy lệnh của ngày hiện tại)
        SELECT
            t1.MaLenhSanXuat, t1.MaLenh, kh.TenKH,
            STUFF((
                SELECT DISTINCT ', ' + hh.TenHang
                FROM CanDoiDonViSanXuat t2
                INNER JOIN DonHangTong dh ON t2.MaDH = dh.MaDH
                INNER JOIN HangHoa hh ON dh.MaKH = hh.MaKH AND dh.MaHang = hh.MaHang
                WHERE t2.MaLenhSanXuat = t1.MaLenhSanXuat AND t2.MaLenh = t1.MaLenh
                FOR XML PATH('')
            ),1,2,'') AS TenHang
        INTO #tbldvsx
        FROM CanDoiDonViSanXuat t1
        INNER JOIN DonHangTong dh2 ON t1.MaDH = dh2.MaDH
        INNER JOIN KhachHang kh ON dh2.MaKH = kh.MaKH
        WHERE t1.MaLenhSanXuat IN (
            SELECT MaLenhSX FROM ERP_KeHachGiaoViecNL
            WHERE (NgayXuatHang >= CAST(@Ngay AS DATE) AND NgayXuatHang < DATEADD(day, 1, CAST(@Ngay AS DATE)))
               OR (NgayXuatHang IS NULL AND NgayGiaoViec >= CAST(@Ngay AS DATE) AND NgayGiaoViec < DATEADD(day, 1, CAST(@Ngay AS DATE)))
            UNION
            SELECT MaLenhSX FROM ERP_PhieuSoanHangVatTu
            WHERE NgaySoanHang >= CAST(@Ngay AS DATE) AND NgaySoanHang < DATEADD(day, 1, CAST(@Ngay AS DATE))
        )
        GROUP BY t1.MaLenhSanXuat, t1.MaLenh, kh.TenKH;

        -- ResultSet 1: Assignments (Phân công giao việc)
        SELECT
            ISNULL(MAX(t2.MaLenh), t1.MaLenhSX)                                      AS MaLenhSX,
            CASE
                WHEN MAX(t1.NgayXuatHang) IS NULL THEN 0
                WHEN CONVERT(DATE, MAX(t1.NgayXuatHang)) <= MAX(CONVERT(DATE, t1.NgayGiaoViec)) THEN 2
                ELSE 3
            END                                                 AS TrangThai,
            MAX(t1.NguoiGiaoViec)                               AS TenNV,
            ''                                                  AS MaNV,
            MAX(t2.TenKH)                                       AS MaKhachHang,
            MAX(t2.TenHang)                                     AS MaHang,
            CONVERT(VARCHAR(10), MAX(t1.NgayXuatHang), 120)     AS NgayThucHien,
            ''                                                  AS GioThucHien,
            ''                                                  AS MoTaCongViec,
            CAST(0 AS BIT)                                      AS ThieuNPL,
            STUFF((
                SELECT DISTINCT ', ' + ISNULL(t3.GhiChu,'')
                FROM ERP_KeHachGiaoViecNL t3
                WHERE t3.MaLenhSX = t1.MaLenhSX AND ISNULL(t3.GhiChu,'') <> ''
                FOR XML PATH('')
            ),1,2,'')                                           AS GhiChu
        FROM ERP_KeHachGiaoViecNL t1
        LEFT JOIN #tbldvsx t2 ON t1.MaLenhSX = t2.MaLenhSanXuat
        WHERE ISNULL(t1.NgayXuatHang, t1.NgayGiaoViec) >= CAST(@Ngay AS DATE) AND ISNULL(t1.NgayXuatHang, t1.NgayGiaoViec) < DATEADD(DAY, 1, CAST(@Ngay AS DATE))
        GROUP BY t1.MaLenhSX;

        -- Bảng tạm pick orders chi tiết
        SELECT
            t1.MaLenhSX AS MaLenhSanXuat, ISNULL(t2.MaLenh, t1.MaLenhSX) AS MaLenhSX,
            ISNULL(t2.TenKH, N' ') AS MaKhachHang, ISNULL(t2.TenHang, N' ') AS MaHang,
            ISNULL(SUM(t3.SLSoanHang_BC), 0) AS SLSoan,
            MAX(t1.SLCapPhat) AS TongSLCanSoan,
            t1.MaNPL, MAX(t3.NgaySoanHang) AS NgaySoan,
            MAX(t1.SLCapPhat) - ISNULL(SUM(t3.SLSoanHang_BC), 0) AS SoPLThieu,
            t1.NguoiSoanHang AS TenNV,
            STUFF((
                SELECT DISTINCT ', ' + ISNULL(t11.GhiChu,'')
                FROM ERP_PhieuSoanHangVatTu t11
                WHERE t11.MaLenhSX = t1.MaLenhSX AND t11.MaNPL = t1.MaNPL
                FOR XML PATH(''), TYPE
            ).value('.', 'nvarchar(max)'), 1, 2, '') AS GhiChu
        INTO #tblSHCT
        FROM ERP_PhieuSoanHangVatTu t1
        LEFT JOIN #tbldvsx t2 ON t1.MaLenhSX = t2.MaLenhSanXuat
        LEFT JOIN ERP_SoanHangNPL_BarCode t3 ON t1.PhieuSH = t3.PhieuSH AND t1.MaNPL = t3.MaNPL
        WHERE t1.NgaySoanHang >= CAST(@Ngay AS DATE) AND t1.NgaySoanHang < DATEADD(DAY, 1, CAST(@Ngay AS DATE))
        GROUP BY t1.MaLenhSX, t2.MaLenh, t2.TenKH, t2.TenHang, t1.MaNPL, t1.NguoiSoanHang;

        -- ResultSet 2: Pick Orders (Phụ liệu - Soạn hàng)
        SELECT
            MaLenhSX                                    AS MaLenhSX,
            CASE
                WHEN ROUND(SUM(TongSLCanSoan) - SUM(SLSoan), 2) <= 0 THEN 2 -- Đã HT (không thiếu)
                WHEN CONVERT(DATE, @Ngay) < CONVERT(DATE, GETDATE()) THEN 3 -- Quá hạn (nếu còn thiếu mà ngày soạn đã qua)
                WHEN CONVERT(DATE, @Ngay) = CONVERT(DATE, GETDATE()) THEN 1 -- Đang làm (hôm nay)
                ELSE 0 -- Chờ TH (tương lai)
            END                                         AS TrangThai,
            TenNV                                       AS TenNV,
            ''                                          AS MaNV,
            MaKhachHang                                 AS MaKhachHang,
            MaHang                                      AS MaHang,
            CONVERT(VARCHAR(10), MAX(NgaySoan), 120)    AS NgaySoan,
            ''                                          AS GioSoan,
            COUNT(DISTINCT MaNPL)                       AS SoLoaiPL,
            ROUND(SUM(SLSoan), 2)                       AS SLSoan,
            ROUND(SUM(TongSLCanSoan), 2)                AS TongSLCanSoan,
            ROUND(SUM(TongSLCanSoan) - SUM(SLSoan), 2) AS SoPLThieu,
            STUFF((
                SELECT DISTINCT ', ' + sub.GhiChu
                FROM #tblSHCT sub
                WHERE sub.MaLenhSX = O.MaLenhSX AND ISNULL(sub.GhiChu, '') <> ''
                FOR XML PATH(''), TYPE
            ).value('.', 'nvarchar(max)'), 1, 2, '') AS GhiChu
        FROM #tblSHCT O
        GROUP BY O.MaLenhSanXuat, O.MaLenhSX, O.MaKhachHang, O.MaHang, O.TenNV;

        DROP TABLE #tbldvsx;
        DROP TABLE #tblSHCT;
        RETURN;
    END

    -- ===================================================================
    -- [GetFilterLists]
    -- Lấy danh sách nhân viên kho để hiển thị lên bộ lọc tìm kiếm
    -- ===================================================================
    IF @Action = 'GetFilterLists'
    BEGIN
        SELECT DISTINCT
            ISNULL(MaNV, '')  AS MaNV,
            ISNULL(TenNV, '') AS TenNV,
            'PB01'            AS MaPhongBan,
            N'Kho Phụ Liệu'  AS TenPhongBan,
            CAST(1 AS BIT)    AS IsActive
        FROM dbo.ERP_LichPhanCongPhuLieu_Task
        WHERE MaNV IS NOT NULL AND MaNV <> '';
        RETURN;
    END

    -- ===================================================================
    -- [GetListPhuLieu]
    -- Dummy action (chưa implement) lấy danh sách phụ liệu
    -- ===================================================================
    IF @Action = 'GetListPhuLieu'
    BEGIN
        SELECT TOP 0 1 AS ID;
        RETURN;
    END

    -- ===================================================================
    -- [GetCanDoiNPL]
    -- Dummy action (chưa implement) lấy cân đối NPL
    -- ===================================================================
    IF @Action = 'GetCanDoiNPL'
    BEGIN
        SELECT TOP 0 1 AS ID;
        RETURN;
    END

    -- ===================================================================
    -- [GetPickOrderDetail]
    -- Lấy chi tiết số lượng cần soạn, tồn kho, đã soạn của từng phụ liệu trong lệnh SX
    -- ===================================================================
    IF @Action = 'GetPickOrderDetail'
    BEGIN
        IF OBJECT_ID('dbo.ERP_SoanHangNPL_BarCode', 'U') IS NOT NULL
        BEGIN
            EXEC sp_executesql N'
            SELECT
                ROW_NUMBER() OVER(ORDER BY sh.MaNPL) AS ID,
                sh.MaLenhSX,
                sh.MaNPL,
                sh.MaNPL                               AS TenNPL,
                SUM(ISNULL(sh.SLSoanHang_TK, 0))      AS SLCanSoan,
                SUM(ISNULL(sh.SLSoanHang_TK, 0))      AS SLTonKho,
                SUM(ISNULL(sh.SLSoanHang_BC, 0))      AS SLDaSoan,
                N''ĐVT''                               AS DonVi,
                MAX(ISNULL(sh.KhoVai, ''''))           AS MaViTri,
                CAST(CASE WHEN SUM(ISNULL(sh.SLSoanHang_TK,0)) > SUM(ISNULL(sh.SLSoanHang_BC,0))
                          THEN 1 ELSE 0 END AS BIT)    AS ThieuHang,
                N''''                                  AS GhiChu
            FROM dbo.ERP_SoanHangNPL_BarCode sh
            WHERE sh.MaLenhSX = @M
            GROUP BY sh.MaLenhSX, sh.MaNPL;
            ', N'@M NVARCHAR(100)', @M = @MaLenhSX;
        END
        ELSE
        BEGIN
            SELECT
                CAST(0   AS INT)           AS ID,
                CAST(N'' AS NVARCHAR(100)) AS MaLenhSX,
                CAST(N'' AS NVARCHAR(200)) AS MaNPL,
                CAST(N'' AS NVARCHAR(500)) AS TenNPL,
                CAST(0   AS FLOAT)         AS SLCanSoan,
                CAST(0   AS FLOAT)         AS SLTonKho,
                CAST(0   AS FLOAT)         AS SLDaSoan,
                CAST(N'' AS NVARCHAR(50))  AS DonVi,
                CAST(N'' AS NVARCHAR(200)) AS MaViTri,
                CAST(0   AS BIT)           AS ThieuHang,
                CAST(N'' AS NVARCHAR(500)) AS GhiChu
            WHERE 1 = 0;
        END
        RETURN;
    END

    -- ===================================================================
    -- [GetNhanVienList]
    -- Lấy danh sách toàn bộ nhân viên phòng ban Kho Phụ Liệu đang hoạt động
    -- ===================================================================
    IF @Action = 'GetNhanVienList'
    BEGIN
        IF OBJECT_ID('dbo.NhanVien', 'U') IS NOT NULL
        BEGIN
            EXEC sp_executesql N'
            SELECT MaNV, TenNV,
                   ISNULL(MaPB, '''') AS MaPhongBan,
                   N''Phòng ban''     AS TenPhongBan,
                   CAST(1 AS BIT)    AS IsActive
            FROM dbo.NhanVien
            WHERE TrangThaiLamViec = 1;';
        END
        ELSE
        BEGIN

            SELECT DISTINCT
                ISNULL(MaNV, '')  AS MaNV,
                ISNULL(TenNV, '') AS TenNV,
                'PB01'            AS MaPhongBan,
                N'Kho Phụ Liệu'  AS TenPhongBan,
                CAST(1 AS BIT)    AS IsActive
            FROM dbo.ERP_LichPhanCongPhuLieu_Task
            WHERE MaNV IS NOT NULL AND MaNV <> '';
        END
        RETURN;
    END

    -- ===================================================================
    -- [SavePhanCong]
    -- Lưu/Cập nhật thông tin phân công nhân viên thực hiện soạn phụ liệu cho lệnh SX
    -- ===================================================================
    IF @Action = 'SavePhanCong'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.ERP_LichPhanCongPhuLieu_Task WHERE MaLenhSX = @MaLenhSX)
            UPDATE dbo.ERP_LichPhanCongPhuLieu_Task
            SET
                MaNV         = @MaNV,
                TenNV        = @TenNV,
                NgayThucHien = @NgayThucHien,
                GhiChu       = @GhiChu,
                UpdateAt     = GETDATE()
            WHERE MaLenhSX = @MaLenhSX;
        ELSE
            INSERT INTO dbo.ERP_LichPhanCongPhuLieu_Task
                (MaLenhSX, MaNV, TenNV, TrangThai, NgayThucHien, GhiChu)
            VALUES
                (@MaLenhSX, @MaNV, @TenNV, 0, @NgayThucHien, @GhiChu);

        SELECT 1 AS Success, N'Đã phân công thành công!' AS [Error];
        RETURN;
    END

    -- ===================================================================
    -- [UpdateTrangThai]
    -- Cập nhật trạng thái xử lý của công việc phân công (Chờ, Đang làm, Hoàn thành)
    -- ===================================================================
    IF @Action = 'UpdateTrangThai'
    BEGIN
        UPDATE dbo.ERP_LichPhanCongPhuLieu_Task
        SET TrangThai = @TrangThai, UpdateAt = GETDATE()
        WHERE MaLenhSX = @MaLenhSX;

        SELECT 1 AS Success, N'Cập nhật trạng thái thành công!' AS [Error];
        RETURN;
    END

    SELECT 'Unknown action: ' + ISNULL(@Action, 'NULL') AS [Error];

END
GO
