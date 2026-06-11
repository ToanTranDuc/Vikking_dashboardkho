USE PMS_QLDH_VIKING_2025;
GO

-- =========================================================================
-- TÃ¡Â»Â° Ã„ÂÃ¡Â»ËœNG FIX LÃ¡Â»â€“I THIÃ¡ÂºÂ¾U CÃ¡Â»ËœT & BÃ¡ÂºÂ¢NG KHI TÃ¡ÂºÂ O STORE (DÃƒâ‚¬NH CHO DB CHÃ†Â¯A CÃ¡ÂºÂ¬P NHÃ¡ÂºÂ¬T)
-- =========================================================================
IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPLV2') IS NULL
BEGIN
    EXEC('CREATE VIEW dbo.ERPPhieuKiemKe_NPLV2 AS SELECT 
        CAST(NULL AS NVARCHAR(50)) AS PhieuKiemKe, CAST(NULL AS NVARCHAR(50)) AS MaNPL,
        CAST(NULL AS DATETIME) AS DateKiemKe, CAST(NULL AS FLOAT) AS SLKiemKe,
        CAST(NULL AS NVARCHAR(50)) AS SoLo, CAST(NULL AS NVARCHAR(50)) AS UserKK,
        CAST(NULL AS NVARCHAR(MAX)) AS GhiChu, CAST(NULL AS BIT) AS IsXacNhan,
        CAST(NULL AS INT) AS MaVTID, CAST(NULL AS INT) AS MauVTID,
        CAST(NULL AS NVARCHAR(50)) AS SoLoID, CAST(NULL AS FLOAT) AS SLKiemKeBanDau
        WHERE 1=0');
END
GO

IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPL', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'DateKiemKe') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD DateKiemKe DATETIME;
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'SoLo') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD SoLo NVARCHAR(200);
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'SLKiemKe') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD SLKiemKe FLOAT;
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'SLKiemKeBanDau') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD SLKiemKeBanDau FLOAT;
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'UserKK') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD UserKK NVARCHAR(200);
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'GhiChu') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD GhiChu NVARCHAR(MAX);
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'MaNPL') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD MaNPL NVARCHAR(200);
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'PhieuKiemKe') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD PhieuKiemKe NVARCHAR(200);
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'MaVTID') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD MaVTID INT;
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'MauVTID') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD MauVTID INT;
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'SoLoID') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD SoLoID NVARCHAR(200);
    IF COL_LENGTH('dbo.ERPPhieuKiemKe_NPL', 'IsXacNhan') IS NULL ALTER TABLE dbo.ERPPhieuKiemKe_NPL ADD IsXacNhan BIT;
END
GO

USE PMS_QLDH_VIKING_2025;
GO

CREATE OR ALTER PROCEDURE dbo.SP_BANG_THONG_KE

    @Action VARCHAR(100),

    @TuNgay DATETIME = NULL,

    @DenNgay DATETIME = NULL,

    @Itemcode NVARCHAR(200) = NULL,

    @IsNhieuNhat INT = NULL,

    @LoaiNPL INT = NULL,

    @Ngay DATETIME = NULL

AS

BEGIN

    SET NOCOUNT ON;



    -- Declarations for GetOverallCapacity

    DECLARE @CapNPL FLOAT = 0, 

            @CapPL FLOAT = 0, 

            @UsedNPL FLOAT = 0, 

            @UsedPL FLOAT = 0,

            @TotalVatTuNPL FLOAT = 0, 

            @TotalVatTuPL FLOAT = 0;



    -- Declarations for GetCustomers

    DECLARE @TotalSKUInWarehouse FLOAT;



    -- Declarations for GlobalSearchByItemcode

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



    -- Declarations for GetFlowTrend12T

    DECLARE @StartDate12T DATE;



    -- Declarations for GetFlowTrendWeekly

    DECLARE @N_Weekly INT = 12,

            @Today_Weekly DATE,

            @StartDateWeekly DATE;



    -- Declarations for GetThanhGiaHangTon

    DECLARE @ThanhGia DECIMAL(20,2) = 0,

            @TongMa INT = 0, 

            @SoMaCoGia INT = 0, 

            @SoMaKhongGia INT = 0,

            @TongSL DECIMAL(20,2) = 0;



    -- Declarations for GetKiemKeDetailByRange

    DECLARE @HasV2R INT = 0;



    -- Declarations for GetKiemKeDetailByDay

    DECLARE @HasV2D INT = 0;



    -- Declarations for GetActivityCalendar

    DECLARE @StartDateCal DATE,

            @EndDateCal DATE,

            @TotalDaysCal INT,

            @EndPlus1Cal DATE;



    -- Declarations for GetFlowTrendByRange

    DECLARE @StartDateRange DATE,

            @EndDateRange DATE,

            @TotalDaysRange INT;



    -- Declarations for GetMoMComparison

    DECLARE @MaxDate DATE,

            @ThisMonth DATE,

            @LastMonth DATE,

            @NextMonth DATE;



    -- =========================================================================

    -- 1. GetOverallCapacity

    -- =========================================================================

    IF @Action = 'GetOverallCapacity'

    BEGIN

        SELECT t1.KeID, ROUND(SUM(t2.Dai * t2.Cao * t2.Rong), 4) AS TongCBMTrongKe, t1.TenKe, t1.Module

        INTO #tempCBMKe_OC

        FROM ERP_KeNPL t1

        LEFT JOIN ERP_ONPL t2 ON t1.KeID = t2.KeID

        WHERE t1.Module <> 3

        GROUP BY t1.KeID, t1.TenKe, t1.Module;



        SELECT BarCode INTO #TempXuatChuaThuHoi_OC

        FROM PhieuXuatHang t1

        WHERE NOT EXISTS (

            SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh = t1.MaLenhSX

              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)

              AND t1.Dot = t2.Dot

        );



        SELECT TOP (0) BarCode INTO #tempBarcodeSH_OC FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL

           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL

        BEGIN

            INSERT INTO #tempBarcodeSH_OC(BarCode)

            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode

                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';

        END



        SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM),4) AS TongCBMTrongO, COUNT(*) AS SLVatTu

        INTO #tempCBMO_OC

        FROM ERP_VatTuCBM t1

        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO

        WHERE t1.MaONPL IS NOT NULL

          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi_OC x WHERE x.BarCode = t1.Barcode)

         AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL c WHERE c.BarCode = t1.Barcode)

          AND NOT EXISTS (SELECT 1 FROM #tempBarcodeSH_OC s WHERE s.BarCode = t1.Barcode)

        GROUP BY t1.MaONPL, t2.KeID;



        SELECT t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module,

            ISNULL(SUM(t2.TongCBMTrongO),0) AS TongCBMSuDungTrongKe,

            ISNULL(t3.TongCBMTrongKe,0) AS TongCBMTrongKe,

            SUM(ISNULL(t2.SLVatTu,0)) AS SLVatTu

        INTO #tempBaseResult_OC

        FROM ERP_KeNPL t1

        LEFT JOIN #tempCBMO_OC t2 ON t1.KeID = t2.KeID

        LEFT JOIN #tempCBMKe_OC t3 ON t1.KeID = t3.KeID

        LEFT JOIN ERP_DayNPL t4 ON t1.DayID = t4.DayID

        WHERE t1.Module <> 3

          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE '%co%'

          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%l?i%'

          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%n%'

        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe;



        SELECT

            @CapNPL        = ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMTrongKe ELSE 0 END),0),

            @CapPL         = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMTrongKe ELSE 0 END),0),

            @UsedNPL       = ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMSuDungTrongKe ELSE 0 END),0),

            @UsedPL        = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMSuDungTrongKe ELSE 0 END),0),

            @TotalVatTuNPL = ISNULL(SUM(CASE WHEN Module=1 THEN SLVatTu ELSE 0 END),0),

            @TotalVatTuPL  = ISNULL(SUM(CASE WHEN Module=2 THEN SLVatTu ELSE 0 END),0)

        FROM #tempBaseResult_OC;



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



        DROP TABLE #tempCBMO_OC; DROP TABLE #tempCBMKe_OC;

        DROP TABLE #TempXuatChuaThuHoi_OC; DROP TABLE #tempBarcodeSH_OC; DROP TABLE #tempBaseResult_OC;

    END



    -- =========================================================================

    -- 2. GetDistinctMaterialCount

    -- =========================================================================

    ELSE IF @Action = 'GetDistinctMaterialCount'

    BEGIN

        IF COL_LENGTH('dbo.ERP_VatTuCBM','MaONPL') IS NULL

        BEGIN

            SELECT CAST(0 AS INT) AS SoMaVatTu;

            RETURN;

        END



        SELECT BarCode

        INTO #TempXCTH_DMC

        FROM PhieuXuatHang t1

        WHERE NOT EXISTS (

            SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh = t1.MaLenhSX

              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)

              AND t1.Dot = t2.Dot

        );



        SELECT TOP (0) BarCode INTO #TempSH_DMC FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL

           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL

        BEGIN

          INSERT INTO #TempSH_DMC(BarCode)

            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode

                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';

        END



        SELECT COUNT(DISTINCT ct.MaNPL) AS SoMaVatTu

        FROM dbo.ERP_VatTuCBM v

        INNER JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON ct.BarCode = v.Barcode

        WHERE v.MaONPL IS NOT NULL

          AND NOT EXISTS (SELECT 1 FROM #TempXCTH_DMC t WHERE t.BarCode = v.Barcode)

          AND NOT EXISTS (SELECT 1 FROM #TempSH_DMC  t WHERE t.BarCode = v.Barcode);



        DROP TABLE #TempXCTH_DMC;

        DROP TABLE #TempSH_DMC;

    END



    -- =========================================================================

    -- 3. GetCustomers

    -- =========================================================================

    ELSE IF @Action = 'GetCustomers'

    BEGIN

        SELECT BarCode INTO #TempXCTH_Cust FROM PhieuXuatHang t1

        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);



        SELECT BarCode INTO #tempSH_Cust FROM ERP_SoanHangNPL_BarCode

        WHERE SLSoanHang_TK - SLSoanHang_BC = 0;



        SELECT MaNPL, SoLoID, SUM(CBM) AS CBM

        INTO #tempBarcodeCBM_Cust

        FROM ERP_VatTuCBM t1

        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO

        LEFT JOIN ERP_DayNPL t3 ON t2.DayID = t3.DayID

        WHERE t1.MaONPL IS NOT NULL

          AND t2.Module <> 3

          AND LOWER(t3.TenDay) NOT LIKE '%co%'

          AND LOWER(t3.TenDay) NOT LIKE N'%l?i%'

          AND LOWER(t3.TenDay) NOT LIKE N'%n%'

          AND NOT EXISTS (SELECT 1 FROM #TempXCTH_Cust x WHERE x.BarCode = t1.Barcode)

          AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)

          AND NOT EXISTS (SELECT 1 FROM #tempSH_Cust t7 WHERE t1.Barcode = t7.BarCode)

        GROUP BY MaNPL, SoLoID;



        SELECT @TotalSKUInWarehouse = CAST(COUNT(DISTINCT CASE WHEN ISNULL(MaNPL,'') <> '' THEN CONCAT(MaNPL,'|',SoLoID) END) AS FLOAT)

        FROM #tempBarcodeCBM_Cust;



        SELECT

            ISNULL(t3.MaKH, '') AS MaKH,

            ISNULL(t5.TenKH, '') AS TenKH,

            COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) AS SLVatTu,

            SUM(CBM) AS CBMSDTrongKho,

            CASE WHEN @TotalSKUInWarehouse > 0

                THEN ROUND(COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) / @TotalSKUInWarehouse * 100, 2)

                ELSE 0

            END AS PhanTramSKU

        FROM #tempBarcodeCBM_Cust t1

        LEFT JOIN (SELECT DISTINCT MaNPL, SoLoID FROM ERP_ChiTietNhapKhoNPL) t2

            ON t1.MaNPL = t2.MaNPL AND t1.SoLoID = t2.SoLoID

        LEFT JOIN ERP_NhapKhoNPL t3 ON t2.SoLoID = t3.SoLoID

        LEFT JOIN KhachHang t5 ON t3.MaKH = t5.MaKH

        GROUP BY ISNULL(t3.MaKH, ''), ISNULL(t5.TenKH, '')

        ORDER BY SLVatTu DESC;



        DROP TABLE #tempBarcodeCBM_Cust; DROP TABLE #tempSH_Cust; DROP TABLE #TempXCTH_Cust;

    END



    -- =========================================================================

    -- 4. GetRacks

    -- =========================================================================

    ELSE IF @Action = 'GetRacks'

    BEGIN

        SELECT t1.KeID, ROUND(SUM(t2.Dai*t2.Cao*t2.Rong),4) AS TongCBMTrongKe, t1.TenKe, t1.Module

        INTO #tempCBMKe_Racks FROM ERP_KeNPL t1 LEFT JOIN ERP_ONPL t2 ON t1.KeID=t2.KeID

        WHERE t1.Module<>3 GROUP BY t1.KeID, t1.TenKe, t1.Module;



        SELECT BarCode INTO #TempXCTH_Racks FROM PhieuXuatHang t1

        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);



        SELECT TOP (0) BarCode INTO #tempSH_Racks FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL

           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL

        BEGIN

            INSERT INTO #tempSH_Racks(BarCode)

            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode

                WHERE ISNULL(SLSoanHang_TK,0)-ISNULL(SLSoanHang_BC,0)=0;';

        END



        SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM),4) AS TongCBMTrongO, COUNT(*) AS SLVatTu

        INTO #tempCBMO_Racks

        FROM ERP_VatTuCBM t1 LEFT JOIN ERP_ONPL t2 ON t1.MaONPL=t2.TenO

        WHERE t1.MaONPL IS NOT NULL

          AND NOT EXISTS (SELECT 1 FROM #TempXCTH_Racks x WHERE x.BarCode=t1.Barcode)

          AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL c WHERE c.BarCode=t1.Barcode)

          AND NOT EXISTS (SELECT 1 FROM #tempSH_Racks s WHERE s.BarCode=t1.Barcode)

        GROUP BY t1.MaONPL, t2.KeID;



        SELECT t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module,

            ISNULL(SUM(t2.TongCBMTrongO),0) AS TongCBMSuDungTrongKe,

            ISNULL(t3.TongCBMTrongKe,0) AS TongCBMTrongKe,

            SUM(ISNULL(t2.SLVatTu,0)) AS SLVatTu

        FROM ERP_KeNPL t1

        LEFT JOIN #tempCBMO_Racks t2 ON t1.KeID=t2.KeID

        LEFT JOIN #tempCBMKe_Racks t3 ON t1.KeID=t3.KeID

        LEFT JOIN ERP_DayNPL t4 ON t1.DayID=t4.DayID

        WHERE t1.Module<>3

          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE '%co%'

          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%l?i%'

          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%n%'

        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe

        ORDER BY t1.Module, t1.TenKe;



        DROP TABLE #tempCBMO_Racks; DROP TABLE #tempCBMKe_Racks; DROP TABLE #TempXCTH_Racks; DROP TABLE #tempSH_Racks;

    END



    -- =========================================================================

    -- 5. GetChuanBiVe

    -- =========================================================================

    ELSE IF @Action = 'GetChuanBiVe'

    BEGIN

        ;WITH ChiTiet AS

        (

            SELECT

                ct.SoLoID,

                ct.POMua,

                ct.MaNPL,

                MAX(ISNULL(ct.SLTong, 0)) AS SLTong

            FROM dbo.ERP_ChiTietNhapKhoNPL ct

            GROUP BY ct.SoLoID, ct.POMua, ct.MaNPL

        ),

        ChiTietTheoSoLo AS

        (

            SELECT

                ct.SoLoID,

                ct.POMua,

                SUM(ISNULL(ct.SLTong, 0)) AS SLTong

            FROM ChiTiet ct

            GROUP BY ct.SoLoID, ct.POMua

        ),

        NgayDuKienTrongKhoang AS

        (

            SELECT DISTINCT

                CAST(nk.NgayNKDuKien AS date) AS NgayNKDuKien

            FROM dbo.ERP_NhapKhoNPL nk

            WHERE nk.NgayNKDuKien IS NOT NULL

              AND CAST(nk.NgayNKDuKien AS date) BETWEEN CAST(@TuNgay AS DATE) AND CAST(@DenNgay AS DATE)

        ),

        NhapKhoRaw AS

        (

            SELECT DISTINCT

                nk.SoLoID,

                nk.SoLo,

                nk.POMua,

                ISNULL(nk.MaKH, '') AS MaKH,

                CAST(N'' AS NVARCHAR(100)) AS MaDH,

                CAST(N'' AS NVARCHAR(100)) AS MaHang,

                CAST(nk.NgayNKDuKien AS date) AS NgayNKDuKien

            FROM dbo.ERP_NhapKhoNPL nk

            INNER JOIN NgayDuKienTrongKhoang m

                ON CAST(nk.NgayNKDuKien AS date) = m.NgayNKDuKien

        ),

        NhapKho AS

        (

            SELECT

                nk.SoLoID,

                nk.POMua,

                MAX(nk.SoLo) AS SoLo,

                MAX(nk.MaKH) AS MaKH,

                MAX(nk.MaDH) AS MaDH,

                MAX(nk.MaHang) AS MaHang,

                MIN(nk.NgayNKDuKien) AS NgayNKDuKien

            FROM NhapKhoRaw nk

            GROUP BY nk.SoLoID, nk.POMua

        )

        SELECT

            MIN(nk.SoLoID) AS SoLoID,

        MAX(nk.SoLo) AS SoLo,

            nk.POMua AS PO,

            nk.POMua,

            MAX(nk.MaDH) AS MaDH,

            MAX(nk.MaHang) AS MaHang,

            MIN(nk.NgayNKDuKien) AS NgayNKDuKien,

            MIN(nk.NgayNKDuKien) AS NgayNhapKho_Update,

            CAST(N'' AS NVARCHAR(200)) AS MaNPL,

            SUM(ISNULL(ct.SLTong, 0)) AS SoLuongSP,

            CAST(0 AS decimal(18, 2)) AS SoLuongThung,

            MAX(ISNULL(kh.TenKH, '')) AS TenKH

        FROM NhapKho nk

        LEFT JOIN ChiTietTheoSoLo ct ON ct.SoLoID = nk.SoLoID AND ct.POMua = nk.POMua

        LEFT JOIN KhachHang kh ON kh.MaKH = nk.MaKH

        GROUP BY nk.POMua

        ORDER BY MIN(nk.NgayNKDuKien), nk.POMua;

    END



    -- =========================================================================

    -- 6. GetChuanBiXuat

    -- =========================================================================

    ELSE IF @Action = 'GetChuanBiXuat'

    BEGIN

        SELECT

            cs.MaLenhSanXuat,

            cs.MaLenh,

            cs.MaDVSX,

            ISNULL(MAX(kh.TenKH), '')           AS KhachHang,

            ISNULL(MAX(hh.TenHang), '')         AS TenHang,

            SUM(ISNULL(cs.SoLuong, 0))          AS SoLuongYeuCau,

            MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END)                       AS KHCat,

            DATEADD(day, 7, MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END))      AS DuKienCat

        FROM dbo.CanDoiDonViSanXuat cs

        LEFT JOIN dbo.DonHangTong dh ON dh.MaDH = cs.MaDH

        LEFT JOIN dbo.HangHoa     hh ON hh.MaHang = dh.MaHang AND hh.MaKH = dh.MaKH

        LEFT JOIN dbo.KhachHang   kh ON kh.MaKH   = dh.MaKH

        LEFT JOIN dbo.WIP_DonHang_Chuyen wc

            ON wc.LenhSX = TRY_CONVERT(INT, REPLACE(ISNULL(cs.MaLenh, ''), 'SX_', ''))

        LEFT JOIN dbo.WIP_DonHang_Chuyen_TechProgress tp ON tp.WIPId = wc.WIPId

        WHERE NOT EXISTS (SELECT 1 FROM dbo.PhieuXuatHang ph WHERE ph.MaLenhSX = cs.MaLenhSanXuat)

        GROUP BY cs.MaLenhSanXuat, cs.MaLenh, cs.MaDVSX

        ORDER BY KHCat DESC;

    END



    -- =========================================================================

    -- 7. GlobalSearchByItemcode

    -- =========================================================================

    ELSE IF @Action = 'GlobalSearchByItemcode'

    BEGIN

        SET @MaVTID = NULL;

        SET @TenVT = N'';



        IF OBJECT_ID('dbo.ERP_VatTuTV') IS NOT NULL

        BEGIN

            IF COL_LENGTH('dbo.ERP_VatTuTV', 'TenVT') IS NOT NULL

            BEGIN

                SET @Sql1 = N'

                    SELECT TOP 1 @MaVTID = vt.MaVTID, @TenVT = ISNULL(vt.TenVT, '''')

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



    -- =========================================================================

    -- 8. GetDangXuat

    -- =========================================================================

    ELSE IF @Action = 'GetDangXuat'

    BEGIN

        IF OBJECT_ID('tempdb..#YeuCau_DX') IS NOT NULL DROP TABLE #YeuCau_DX;

        SELECT

            dk.MaLenhSX,

            dk.PhieuDK,

            SUM(ISNULL(dk.SLDK, 0))      AS SLYeuCau

        INTO #YeuCau_DX

        FROM dbo.ERP_PhieuDangKyXuatVT dk

        WHERE dk.IsNPL = 1

          AND ISNULL(dk.SLDK, 0) > 0

        GROUP BY dk.MaLenhSX, dk.PhieuDK;



        IF OBJECT_ID('tempdb..#DaXuat_DX') IS NOT NULL DROP TABLE #DaXuat_DX;

        SELECT

            ph.PhieuYC,

ph.MaLenhSX,

            MAX(ph.MaLenh)              AS MaLenh,

            MAX(ph.MaGop)               AS MaGop,

            MAX(ph.NgayXuatHang)        AS NgayXuatHang,

            SUM(ISNULL(ph.SLNhap, 0))   AS SLDaXuat

        INTO #DaXuat_DX

        FROM dbo.PhieuXuatHang ph

        WHERE ph.Moudule = 0 AND ph.NPL = 1

          AND ph.NgayXuatHang IS NOT NULL

          AND ph.NgayXuatHang >= CAST(@TuNgay AS DATE) AND ph.NgayXuatHang <= CAST(@DenNgay AS DATE)

        GROUP BY ph.PhieuYC, ph.MaLenhSX;



        IF OBJECT_ID('tempdb..#KhTH_DX') IS NOT NULL DROP TABLE #KhTH_DX;

        SELECT DISTINCT gd.MaGop,

               ISNULL(hh.TenHang, '') AS TenHang,

               ISNULL(kh.TenKH, '')   AS TenKH

        INTO #KhTH_DX

        FROM dbo.GopDonHang gd

        INNER JOIN dbo.DonHangTong dh ON gd.MaDH = dh.MaDH

        LEFT JOIN dbo.HangHoa hh ON hh.MaHang = dh.MaHang AND hh.MaKH = dh.MaKH

        LEFT JOIN dbo.KhachHang kh ON kh.MaKH = dh.MaKH;



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

        FROM #DaXuat_DX x

        LEFT JOIN #YeuCau_DX y ON y.PhieuDK = x.PhieuYC

        LEFT JOIN #KhTH_DX   k ON k.MaGop   = x.MaGop

        ORDER BY x.NgayXuatHang DESC;



        DROP TABLE #YeuCau_DX;

        DROP TABLE #DaXuat_DX;

        DROP TABLE #KhTH_DX;

    END



    -- =========================================================================

    -- 9. GetFlowTrend12T

    -- =========================================================================

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

        Combined AS (

            SELECT m.Nam, m.Thang,

                ISNULL(n.TotalIn, 0) AS TotalIn, ISNULL(x.TotalOut, 0) AS TotalOut, ISNULL(th.TotalTH, 0) AS TotalTH

            FROM Months m

            LEFT JOIN NhapTheoThang n ON n.Nam = m.Nam AND n.Thang = m.Thang

            LEFT JOIN XuatTheoThang x ON x.Nam = m.Nam AND x.Thang = m.Thang

            LEFT JOIN ThuHoiTheoThang th ON th.Nam = m.Nam AND th.Thang = m.Thang

        )

        SELECT c.Nam, c.Thang, c.TotalIn, c.TotalOut,

            ISNULL(d.TonDau, 0) + SUM(c.TotalIn + c.TotalTH - c.TotalOut) OVER (ORDER BY c.Nam, c.Thang ROWS UNBOUNDED PRECEDING) AS TotalStock

        FROM Combined c CROSS JOIN TonDauKy d

        ORDER BY c.Nam, c.Thang;

    END



    -- =========================================================================

    -- 10. GetFlowTrendWeekly

    -- =========================================================================

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

        Combined AS (

            SELECT w.Nam, w.Tuan,

                ISNULL(n.TotalIn, 0) AS TotalIn,

                ISNULL(x.TotalOut, 0) AS TotalOut,

                ISNULL(th.TotalTH, 0) AS TotalTH

            FROM Weeks w

            LEFT JOIN NhapTheoTuan n ON n.Nam = w.Nam AND n.Tuan = w.Tuan

            LEFT JOIN XuatTheoTuan x ON x.Nam = w.Nam AND x.Tuan = w.Tuan

            LEFT JOIN ThuHoiTheoTuan th ON th.Nam = w.Nam AND th.Tuan = w.Tuan

        )

        SELECT c.Nam, c.Tuan, c.TotalIn, c.TotalOut,

            (SELECT TonDau FROM TonDauKy) +

            SUM(c.TotalIn + c.TotalTH - c.TotalOut) OVER (ORDER BY c.Nam, c.Tuan ROWS UNBOUNDED PRECEDING) AS TotalStock

        FROM Combined c

        ORDER BY c.Nam, c.Tuan;

    END



    -- =========================================================================

    -- 11. GetAllMaterialsInStock

    -- =========================================================================

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



        SELECT BarCode INTO #tmpXCTH_AMIS FROM PhieuXuatHang t1

        WHERE NOT EXISTS (

            SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh = t1.MaLenhSX

              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)

              AND t1.Dot = t2.Dot

        );



        SELECT TOP (0) BarCode INTO #tmpSH_AMIS FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL

           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL

        BEGIN

            INSERT INTO #tmpSH_AMIS(BarCode)

            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode

                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';

        END



        IF OBJECT_ID('tempdb..#MatRaw_AMIS') IS NOT NULL DROP TABLE #MatRaw_AMIS;

        SELECT

            ct.MaNPL,

            o.Module,

            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho,

            COUNT(*)                              AS SoBarCode

        INTO #MatRaw_AMIS

        FROM dbo.ERP_ChiTietNhapKhoNPL ct

        INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode

        INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL

        WHERE v.MaONPL IS NOT NULL

          AND o.Module IN (1, 2)

          AND NOT EXISTS (SELECT 1 FROM #tmpXCTH_AMIS t WHERE t.BarCode = v.Barcode)

          AND NOT EXISTS (SELECT 1 FROM #tmpSH_AMIS   t WHERE t.BarCode = v.Barcode)

        GROUP BY ct.MaNPL, o.Module

        HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) > 0;



        IF OBJECT_ID('tempdb..#MatParsed_AMIS') IS NOT NULL DROP TABLE #MatParsed_AMIS;

        SELECT

            r.MaNPL,

            r.Module,

            r.TonKho,

            r.SoBarCode,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID

        INTO #MatParsed_AMIS

        FROM #MatRaw_AMIS r;



        IF OBJECT_ID('tempdb..#MatFinal_AMIS') IS NOT NULL DROP TABLE #MatFinal_AMIS;

        SELECT

            p.MaVTID                          AS MaVT,

            CAST(p.MaVTID AS NVARCHAR(500))   AS TenVT,

            CAST('' AS NVARCHAR(200))         AS Mau,

            CAST('' AS NVARCHAR(200))         AS KhoVai,

            CAST('' AS NVARCHAR(500))         AS ChiTiet,

            CAST('' AS NVARCHAR(50))          AS TenDVVT,

            p.TonKho,

            p.SoBarCode,

            CASE WHEN p.Module = 1 THEN N'NL' WHEN p.Module = 2 THEN N'PL' ELSE N'' END AS LoaiKho,

            p.MauVTID,

            p.KhoVaiID

        INTO #MatFinal_AMIS

        FROM #MatParsed_AMIS p;



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



        DROP TABLE #MatRaw_AMIS;

        DROP TABLE #MatParsed_AMIS;

        DROP TABLE #MatFinal_AMIS;

        DROP TABLE #tmpXCTH_AMIS;

        DROP TABLE #tmpSH_AMIS;

    END



    -- =========================================================================

    -- 12. GetThanhGiaHangTon

    -- =========================================================================

    ELSE IF @Action = 'GetThanhGiaHangTon'

    BEGIN

        SELECT BarCode INTO #tmpXTH_TGHT FROM PhieuXuatHang t1

        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);



        SELECT TOP (0) BarCode INTO #tmpSH_TGHT FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL

            INSERT INTO #tmpSH_TGHT(BarCode)

            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode WHERE ISNULL(SLSoanHang_TK,0)-ISNULL(SLSoanHang_BC,0)=0;';



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

                  AND NOT EXISTS (SELECT 1 FROM #tmpXTH_TGHT t WHERE t.BarCode = v.Barcode)

                  AND NOT EXISTS (SELECT 1 FROM #tmpSH_TGHT  t WHERE t.BarCode = v.Barcode)

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



        DROP TABLE #tmpXTH_TGHT;

        DROP TABLE #tmpSH_TGHT;

    END



    -- =========================================================================

    -- 13. GetNKDuKienByRange

    -- =========================================================================

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

          AND CAST(nk.NgayNKDuKien AS DATE) BETWEEN CAST(@TuNgay AS DATE) AND CAST(@DenNgay AS DATE)

        ORDER BY nk.NgayNKDuKien, nk.SoLo;

    END



    -- =========================================================================

    -- 14. GetNhapDetailByRange

    -- =========================================================================

    ELSE IF @Action = 'GetNhapDetailByRange'

    BEGIN

        IF OBJECT_ID('tempdb..#NhapAggR_NDR') IS NOT NULL DROP TABLE #NhapAggR_NDR;

        SELECT

            ct.SoLoID,

            ct.MaNPL,

            MAX(ct.POMua)                          AS PO,

            MAX(ct.MaDVVT)                         AS MaDVVT,

            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuong,

            COUNT(*)                               AS SoBarCode,

            MIN(ct.NgayNhapKho)                    AS NgayNhapTu,

            MAX(ct.NgayNhapKho)                    AS NgayNhapDen

        INTO #NhapAggR_NDR

        FROM dbo.ERP_ChiTietNhapKhoNPL ct

        WHERE ct.NgayNhapKho IS NOT NULL

          AND ct.NgayNhapKho >= CAST(@TuNgay AS DATE)

          AND ct.NgayNhapKho <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))

        GROUP BY ct.SoLoID, ct.MaNPL;



        IF OBJECT_ID('tempdb..#NhapFinalR_NDR') IS NOT NULL DROP TABLE #NhapFinalR_NDR;

        SELECT TOP 2000

            ISNULL(nk.SoLo, '')                                                    AS PINCC,

            a.PO                           AS PO,

            a.MaNPL                                                                AS MaNPL,

            CAST('' AS NVARCHAR(200))                                              AS ItemCode,

            CAST('' AS NVARCHAR(200))                                              AS MaMauVT,

            CAST('' AS NVARCHAR(200))                                              AS MauVT,

            CAST('' AS NVARCHAR(200))                                              AS WidthSize,

            CAST('' AS NVARCHAR(200))                                              AS DonViVT,

            ISNULL(kh.TenKH, '')                                                   AS TenKH,

            a.MaDVVT                                                               AS MaDVVT,

            a.SoLuong                                                              AS SoLuong,

            a.SoBarCode                                                            AS SoBarCode,

            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 3), '')                   AS MaVTID,

            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 2), '')                   AS MauVTID,

            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 1), '')                   AS KhoVaiID,

            CAST(a.NgayNhapTu AS DATE)                                             AS NgayNhap

        INTO #NhapFinalR_NDR

        FROM #NhapAggR_NDR a

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



        DROP TABLE #NhapAggR_NDR;

        DROP TABLE #NhapFinalR_NDR;

    END



    -- =========================================================================

    -- 15. GetXuatDetailByRange

    -- =========================================================================

    ELSE IF @Action = 'GetXuatDetailByRange'

    BEGIN

        SELECT TOP 2000

            xh.MaLenh,

            COUNT(*)                        AS SoBarCode,

            SUM(ISNULL(xh.SLNhap, 0))       AS SoLuong,

            ISNULL(MAX(hh.TenHang), '')     AS TenHang,

            ISNULL(MAX(kh.TenKH), '')       AS TenKH,

            MIN(xh.NgayXuatHang)            AS NgayXuatTu,

            MAX(xh.NgayXuatHang)            AS NgayXuatDen

        FROM dbo.PhieuXuatHang xh

        OUTER APPLY (
            SELECT TOP 1 dh.MaHang, dh.MaKH 
            FROM dbo.CanDoiDonViSanXuat cs 
            JOIN dbo.DonHangTong dh ON cs.MaDH = dh.MaDH 
            WHERE cs.MaLenh = xh.MaLenh AND xh.MaLenh IS NOT NULL AND xh.MaLenh <> ''
        ) AS dh_lenh

        LEFT JOIN dbo.DonHangTong dh_gop ON dh_gop.MaDH = xh.MaGop

        LEFT JOIN dbo.HangHoa    hh ON hh.MaHang  = ISNULL(dh_lenh.MaHang, dh_gop.MaHang) AND hh.MaKH = ISNULL(dh_lenh.MaKH, dh_gop.MaKH)

        LEFT JOIN dbo.KhachHang  kh ON kh.MaKH    = ISNULL(dh_lenh.MaKH, dh_gop.MaKH)

        WHERE xh.ModuleXH = 1

          AND xh.NgayXuatHang IS NOT NULL

          AND xh.NgayXuatHang >= CAST(@TuNgay AS DATE)

          AND xh.NgayXuatHang <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))

        GROUP BY xh.MaLenh

        ORDER BY SUM(xh.SLNhap) DESC;

    END



    -- =========================================================================

    -- 16. GetKiemKeDetailByRange

    -- =========================================================================

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

            SELECT TOP 2000

                ISNULL(k.PhieuKiemKe, '')     AS PhieuKiemKe,

                ISNULL(k.SoLo, '')            AS SoLo,

                COUNT(*)                    AS SoBarCode,

                SUM(ISNULL(k.SLKiemKe, 0))    AS SoLuong,

                ISNULL(MAX(nv.Ten), ISNULL(MAX(k.UserKK), '')) AS UserKK,

                ISNULL(MAX(k.GhiChu), '')     AS GhiChu,

                MIN(k.DateKiemKe)             AS NgayKKTu,

                MAX(k.DateKiemKe)             AS NgayKKDen

            FROM dbo.ERPPhieuKiemKe_NPLV2 k
            
            LEFT JOIN SYS_NhanVien nv ON nv.UserID = k.UserKK

            WHERE k.DateKiemKe IS NOT NULL

              AND k.DateKiemKe >= CAST(@TuNgay AS DATE)

              AND k.DateKiemKe <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))

            GROUP BY k.PhieuKiemKe, k.SoLo

            ORDER BY SUM(k.SLKiemKe) DESC;

        END

        ELSE

        BEGIN

            SELECT TOP 2000

                ISNULL(k.PhieuKiemKe, '')     AS PhieuKiemKe,

                ISNULL(k.SoLo, '')            AS SoLo,

                COUNT(*)                    AS SoBarCode,

                SUM(ISNULL(k.SLKiemKe, 0))    AS SoLuong,

                ISNULL(MAX(nv.Ten), ISNULL(MAX(k.UserKK), '')) AS UserKK,

                ISNULL(MAX(k.GhiChu), '')     AS GhiChu,

                MIN(k.DateKiemKe)             AS NgayKKTu,

                MAX(k.DateKiemKe)             AS NgayKKDen

            FROM dbo.ERPPhieuKiemKe_NPL k

            LEFT JOIN SYS_NhanVien nv ON nv.UserID = k.UserKK

            WHERE k.DateKiemKe IS NOT NULL

              AND k.DateKiemKe >= CAST(@TuNgay AS DATE)

              AND k.DateKiemKe <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))

            GROUP BY k.PhieuKiemKe, k.SoLo

            ORDER BY SUM(k.SLKiemKe) DESC;

        END

    END



    -- =========================================================================

    -- 17. GetNhapDetailByDay

    -- =========================================================================

    ELSE IF @Action = 'GetNhapDetailByDay'

    BEGIN

        IF OBJECT_ID('tempdb..#NhapAgg_NDD') IS NOT NULL DROP TABLE #NhapAgg_NDD;

        SELECT

            ct.SoLoID,

            ct.MaNPL,

            MAX(ct.POMua)                          AS PO,

            MAX(ct.MaDVVT)                         AS MaDVVT,

            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuong,

            COUNT(*)                               AS SoBarCode

        INTO #NhapAgg_NDD

        FROM dbo.ERP_ChiTietNhapKhoNPL ct

        LEFT JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = ct.SoLoID

        WHERE ct.NgayNhapKho IS NOT NULL

          AND CAST(ct.NgayNhapKho AS DATE) = CAST(@Ngay AS DATE)

          -- AND ISNULL(nk.IsDuyetNK, 0) = 1

        GROUP BY ct.SoLoID, ct.MaNPL;



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

            ISNULL(kh.TenKH, '')                   AS TenKH,

            a.MaDVVT                               AS MaDVVT,

            a.SoLuong                              AS SoLuong,

            a.SoBarCode                            AS SoBarCode,

            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 3), '') AS MaVTID,

            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 2), '') AS MauVTID,

            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 1), '') AS KhoVaiID

        INTO #NhapFinal_NDD

        FROM #NhapAgg_NDD a

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



        DROP TABLE #NhapAgg_NDD;

        DROP TABLE #NhapFinal_NDD;

    END



    -- =========================================================================

    -- 18. GetXuatDetailByDay

    -- =========================================================================

    ELSE IF @Action = 'GetXuatDetailByDay'

    BEGIN

        SELECT TOP 500

            xh.MaLenh,

            COUNT(*)                        AS SoBarCode,

            SUM(ISNULL(xh.SLNhap, 0))       AS SoLuong,

            ISNULL(MAX(hh.TenHang), '')     AS TenHang,

            ISNULL(MAX(kh.TenKH), '')       AS TenKH

        FROM dbo.PhieuXuatHang xh

        OUTER APPLY (
            SELECT TOP 1 dh.MaHang, dh.MaKH 
            FROM dbo.CanDoiDonViSanXuat cs 
            JOIN dbo.DonHangTong dh ON cs.MaDH = dh.MaDH 
            WHERE cs.MaLenh = xh.MaLenh AND xh.MaLenh IS NOT NULL AND xh.MaLenh <> ''
        ) AS dh_lenh

        LEFT JOIN dbo.DonHangTong dh_gop ON dh_gop.MaDH = xh.MaGop

        LEFT JOIN dbo.HangHoa    hh ON hh.MaHang  = ISNULL(dh_lenh.MaHang, dh_gop.MaHang) AND hh.MaKH = ISNULL(dh_lenh.MaKH, dh_gop.MaKH)

        LEFT JOIN dbo.KhachHang  kh ON kh.MaKH    = ISNULL(dh_lenh.MaKH, dh_gop.MaKH)

        WHERE xh.ModuleXH = 1

          AND xh.NgayXuatHang IS NOT NULL

          AND CAST(xh.NgayXuatHang AS DATE) = CAST(@Ngay AS DATE)

        GROUP BY xh.MaLenh

        ORDER BY SUM(xh.SLNhap) DESC;

    END



    -- =========================================================================

    -- 19. GetKiemKeDetailByDay

    -- =========================================================================

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

                ISNULL(k.PhieuKiemKe, '')     AS PhieuKiemKe,

                ISNULL(k.SoLo, '')            AS SoLo,

                COUNT(*)                    AS SoBarCode,

                SUM(ISNULL(k.SLKiemKe, 0))    AS SoLuong,

                ISNULL(MAX(nv.Ten), ISNULL(MAX(k.UserKK), '')) AS UserKK,

                ISNULL(MAX(k.GhiChu), '')     AS GhiChu

            FROM dbo.ERPPhieuKiemKe_NPLV2 k

            LEFT JOIN SYS_NhanVien nv ON nv.UserID = k.UserKK

            WHERE k.DateKiemKe IS NOT NULL

              AND CAST(k.DateKiemKe AS DATE) = CAST(@Ngay AS DATE)

            GROUP BY k.PhieuKiemKe, k.SoLo

            ORDER BY SUM(k.SLKiemKe) DESC;

        END

        ELSE

        BEGIN

            SELECT TOP 500

                ISNULL(k.PhieuKiemKe, '')     AS PhieuKiemKe,

                ISNULL(k.SoLo, '')            AS SoLo,

                COUNT(*)                    AS SoBarCode,

                SUM(ISNULL(k.SLKiemKe, 0))    AS SoLuong,

                ISNULL(MAX(nv.Ten), ISNULL(MAX(k.UserKK), '')) AS UserKK,

                ISNULL(MAX(k.GhiChu), '')     AS GhiChu

            FROM dbo.ERPPhieuKiemKe_NPL k

            LEFT JOIN SYS_NhanVien nv ON nv.UserID = k.UserKK

            WHERE k.DateKiemKe IS NOT NULL

              AND CAST(k.DateKiemKe AS DATE) = CAST(@Ngay AS DATE)

            GROUP BY k.PhieuKiemKe, k.SoLo

            ORDER BY SUM(k.SLKiemKe) DESC;

        END

    END



    -- =========================================================================

    -- 20. GetRackSlotDetail

    -- =========================================================================

    ELSE IF @Action = 'GetRackSlotDetail'

    BEGIN

        SELECT BarCode INTO #tmpXuat_RSD FROM PhieuXuatHang t1

        WHERE NOT EXISTS (

            SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh = t1.MaLenhSX

              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)

              AND t1.Dot = t2.Dot

        );



        SELECT TOP (0) BarCode INTO #tmpSoanHang_RSD FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL

           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL

        BEGIN

            INSERT INTO #tmpSoanHang_RSD(BarCode)

            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode

                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';

        END



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

          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE N'%l?i%'

          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE N'%n%'

          AND NOT EXISTS (SELECT 1 FROM #tmpXuat_RSD tx WHERE tx.BarCode = vt.Barcode)

          AND NOT EXISTS (SELECT 1 FROM #tmpSoanHang_RSD ts WHERE ts.BarCode = vt.Barcode);



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

        DROP TABLE #tmpXuat_RSD;

        DROP TABLE #tmpSoanHang_RSD;

    END



    -- =========================================================================

    -- 21. GetActivityCalendar

    -- =========================================================================

    ELSE IF @Action = 'GetActivityCalendar'

    BEGIN

        SET @StartDateCal = CAST(@TuNgay  AS DATE);

        SET @EndDateCal   = CAST(@DenNgay AS DATE);

        SET @TotalDaysCal  = DATEDIFF(DAY, @StartDateCal, @EndDateCal) + 1;

        IF @TotalDaysCal < 1   SET @TotalDaysCal = 1;

        IF @TotalDaysCal > 400 SET @TotalDaysCal = 400;



        IF OBJECT_ID('tempdb..#CalDays_AC') IS NOT NULL DROP TABLE #CalDays_AC;

        SELECT TOP (@TotalDaysCal)

            DATEADD(DAY, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) - 1, @StartDateCal) AS NgayHoatDong

        INTO #CalDays_AC

        FROM master..spt_values WHERE type = 'P';



        SET @EndPlus1Cal = DATEADD(DAY, 1, @EndDateCal);



        IF OBJECT_ID('tempdb..#CalNhap_AC') IS NOT NULL DROP TABLE #CalNhap_AC;

        SELECT CAST(ct.NgayNhapKho AS DATE) AS Ngay,

               SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TotalIn

        INTO #CalNhap_AC

        FROM dbo.ERP_ChiTietNhapKhoNPL ct

        LEFT JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = ct.SoLoID

        WHERE ct.NgayNhapKho >= @StartDateCal

          AND ct.NgayNhapKho <  @EndPlus1Cal

          -- AND ISNULL(nk.IsDuyetNK, 0) = 1

        GROUP BY CAST(ct.NgayNhapKho AS DATE);



        IF OBJECT_ID('tempdb..#CalXuat_AC') IS NOT NULL DROP TABLE #CalXuat_AC;

        SELECT CAST(NgayXuatHang AS DATE) AS Ngay,

               SUM(ISNULL(SLNhap, 0)) AS TotalOut

        INTO #CalXuat_AC

        FROM dbo.PhieuXuatHang

        WHERE ModuleXH = 1

          AND NgayXuatHang >= @StartDateCal

          AND NgayXuatHang <  @EndPlus1Cal

        GROUP BY CAST(NgayXuatHang AS DATE);



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



        SELECT

            CONVERT(VARCHAR(10), d.NgayHoatDong, 120) AS NgayHoatDong,

            ISNULL(n.TotalIn,     0) AS TotalIn,

            ISNULL(x.TotalOut,    0) AS TotalOut,

            ISNULL(k.TotalKiemKe, 0) AS TotalKiemKe,

            ISNULL(n.TotalIn, 0) + ISNULL(x.TotalOut, 0) + ISNULL(k.TotalKiemKe, 0) AS TotalActivity

        FROM #CalDays_AC d

        LEFT JOIN #CalNhap_AC n ON n.Ngay = d.NgayHoatDong

        LEFT JOIN #CalXuat_AC x ON x.Ngay = d.NgayHoatDong

        LEFT JOIN #CalKK_AC   k ON k.Ngay = d.NgayHoatDong

        ORDER BY d.NgayHoatDong;



        DROP TABLE #CalDays_AC;

        DROP TABLE #CalNhap_AC;

        DROP TABLE #CalXuat_AC;

        DROP TABLE #CalKK_AC;

    END



    -- =========================================================================

    -- 22. GetFlowTrendByRange

    -- =========================================================================

    ELSE IF @Action = 'GetFlowTrendByRange'

    BEGIN

        SET @StartDateRange = @TuNgay;

        SET @EndDateRange   = @DenNgay;

        SET @TotalDaysRange  = DATEDIFF(DAY, @StartDateRange, @EndDateRange) + 1;

        IF @TotalDaysRange < 1 SET @TotalDaysRange = 1;

        IF @TotalDaysRange > 400 SET @TotalDaysRange = 400;



        ;WITH Days AS (

            SELECT TOP (@TotalDaysRange)

                CAST(DATEADD(DAY, number, @StartDateRange) AS DATE) AS Ngay

            FROM master..spt_values

            WHERE type = 'P' AND number BETWEEN 0 AND 399

        ),

        TonDauKy AS (

            SELECT ISNULL(SUM(SL), 0) AS TonDau FROM (

                SELECT SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS SL

                FROM ERP_ChiTietNhapKhoNPL

                WHERE TRY_CONVERT(DATE, NgayNhapKho) < @StartDateRange

                UNION ALL

                SELECT -SUM(ISNULL(SLNhap, 0))

                FROM PhieuXuatHang

                WHERE ModuleXH = 1

                  AND TRY_CONVERT(DATE, NgayXuatHang) < @StartDateRange

                UNION ALL

                SELECT SUM(ISNULL(ThuHoi, 0))

                FROM PhieuThuHoiNPL

                WHERE TRY_CONVERT(DATE, NgayTH) < @StartDateRange

            ) t

        ),

        NhapTheoNgay AS (

            SELECT CAST(NgayNhapKho AS DATE) AS Ngay,

                   SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS TotalIn

            FROM ERP_ChiTietNhapKhoNPL

            WHERE NgayNhapKho >= @StartDateRange

              AND NgayNhapKho <  DATEADD(DAY, 1, @EndDateRange)

            GROUP BY CAST(NgayNhapKho AS DATE)

        ),

        XuatTheoNgay AS (

            SELECT CAST(NgayXuatHang AS DATE) AS Ngay,

                   SUM(ISNULL(SLNhap, 0)) AS TotalOut

            FROM PhieuXuatHang

            WHERE ModuleXH = 1

              AND NgayXuatHang >= @StartDateRange

              AND NgayXuatHang <  DATEADD(DAY, 1, @EndDateRange)

            GROUP BY CAST(NgayXuatHang AS DATE)

        ),

        ThuHoiTheoNgay AS (

            SELECT CAST(NgayTH AS DATE) AS Ngay,

                   SUM(ISNULL(ThuHoi, 0)) AS TotalTH

            FROM PhieuThuHoiNPL

            WHERE NgayTH >= @StartDateRange

              AND NgayTH <  DATEADD(DAY, 1, @EndDateRange)

            GROUP BY CAST(NgayTH AS DATE)

        ),

        Combined AS (

            SELECT d.Ngay,

                   ISNULL(n.TotalIn, 0)  AS TotalIn,

                   ISNULL(x.TotalOut, 0) AS TotalOut,

                   ISNULL(th.TotalTH, 0) AS TotalTH

            FROM Days d

            LEFT JOIN NhapTheoNgay   n  ON n.Ngay  = d.Ngay

            LEFT JOIN XuatTheoNgay   x  ON x.Ngay  = d.Ngay

            LEFT JOIN ThuHoiTheoNgay th ON th.Ngay = d.Ngay

        )

        SELECT

            CONVERT(VARCHAR(10), c.Ngay, 120) AS Ngay,

            c.TotalIn,

            c.TotalOut,

            ISNULL(d.TonDau, 0)

              + SUM(c.TotalIn + c.TotalTH - c.TotalOut)

                OVER (ORDER BY c.Ngay ROWS UNBOUNDED PRECEDING) AS TotalStock

        FROM Combined c CROSS JOIN TonDauKy d

        ORDER BY c.Ngay;

    END



    -- =========================================================================

    -- 23. GetMoMComparison

    -- =========================================================================

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



    -- =========================================================================

    -- 24. GetTop5

    -- =========================================================================

    ELSE IF @Action = 'GetTop5'

    BEGIN

        CREATE TABLE #tmpXuatChuaThuHoi_T5 (

            BarCode NVARCHAR(500) NOT NULL PRIMARY KEY

        );

        INSERT INTO #tmpXuatChuaThuHoi_T5 (BarCode)

        SELECT DISTINCT t1.BarCode

        FROM PhieuXuatHang t1

        WHERE t1.BarCode IS NOT NULL

          AND EXISTS (SELECT 1 FROM dbo.ERP_VatTuCBM v WHERE v.Barcode = t1.BarCode)

          AND NOT EXISTS (

            SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh = t1.MaLenhSX

              AND t2.BarCode = t1.BarCode

              AND t1.Dot = t2.Dot

          )

          AND NOT EXISTS (

            SELECT 1 FROM PhieuThuHoiNPL t2

            WHERE t2.MaLenh = t1.MaLenhSX

              AND t2.BarCode = t1.BarCodeGoc

              AND t1.Dot = t2.Dot

          );



        CREATE TABLE #tmpSH_T5 (

            BarCode NVARCHAR(500) NOT NULL PRIMARY KEY

        );

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL

           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL

        BEGIN

            INSERT INTO #tmpSH_T5(BarCode)

            EXEC sp_executesql N'SELECT DISTINCT BarCode FROM ERP_SoanHangNPL_BarCode

                WHERE BarCode IS NOT NULL AND ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';

        END





        IF OBJECT_ID('tempdb..#TopRaw_T5') IS NOT NULL DROP TABLE #TopRaw_T5;

        SELECT

            ct.MaNPL,

            o.Module,

            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho

        INTO #TopRaw_T5

        FROM dbo.ERP_ChiTietNhapKhoNPL ct

        INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode

        INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL

        WHERE v.MaONPL IS NOT NULL

          AND o.Module IN (1, 2)

          AND (@LoaiNPL = 0 OR o.Module = @LoaiNPL)

          AND NOT EXISTS (SELECT 1 FROM #tmpXuatChuaThuHoi_T5 t WHERE t.BarCode = v.Barcode)

          AND NOT EXISTS (SELECT 1 FROM #tmpSH_T5       t WHERE t.BarCode = v.Barcode)

        GROUP BY ct.MaNPL, o.Module

        HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) > 0;



        IF OBJECT_ID('tempdb..#TopParsed_T5') IS NOT NULL DROP TABLE #TopParsed_T5;

        SELECT

            r.MaNPL,

            r.Module,

            r.TonKho,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,

            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID

        INTO #TopParsed_T5

        FROM #TopRaw_T5 r;



        IF OBJECT_ID('tempdb..#TopFinal_T5') IS NOT NULL DROP TABLE #TopFinal_T5;

        SELECT

            p.MaVTID                          AS MaVTID_Raw,

            CAST(p.MaVTID AS NVARCHAR(200))   AS MaVT,

            CAST('' AS NVARCHAR(500))         AS TenVT,

            CAST('' AS NVARCHAR(200))         AS Mau,

            CAST('' AS NVARCHAR(200))         AS KhoVai,

            CAST('' AS NVARCHAR(500))         AS ChiTiet,

            CAST('' AS NVARCHAR(50))          AS TenDVVT,

            p.TonKho,

            p.Module                          AS NPL,

            p.MaCLVTID,

            p.MauVTID,

            p.KhoVaiID

        INTO #TopFinal_T5

        FROM #TopParsed_T5 p;



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



        DROP TABLE #TopRaw_T5;

        DROP TABLE #TopParsed_T5;

        DROP TABLE #TopFinal_T5;

        DROP TABLE #tmpXuatChuaThuHoi_T5;

        DROP TABLE #tmpSH_T5;

    END



    -- =========================================================================

    -- 25. GetCongViecChoXuLy

    -- Logic: ERP_ChiTietNhapKhoNPL WHERE IsDuyetNK <> 1

    --        AND EXISTS in (QTY_KiemVaiV2 DuyetQC=1 OR Qty_KiemPL_XacNhan Is_XN_SoLo=1)

    -- =========================================================================

    ELSE IF @Action = 'GetCongViecChoXuLy'

    BEGIN

        DECLARE @ItemcodeChoNK INT = 0;

        DECLARE @KKChoDuyet INT = 0;



        -- 1. ItemCode ch? nh?p kho:

        --    ERP_ChiTietNhapKhoNPL.IsDuyetNK <> 1 (NULL ho?c 0)

        --    AND da qua QC (t?n t?i trong b?ng ki?m v?i DuyetQC=1 ho?c Is_XN_SoLo=1)

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



        -- 2. Ki?m kÃª ch? duy?t

        SELECT @KKChoDuyet = COUNT(DISTINCT CONCAT(t1.SoLoID, '|', t1.MaNPL))

        FROM dbo.ERPPhieuKiemKe_NPL t1

        WHERE t1.IsXacNhan IS NULL;



        SELECT 'itemcode_cho_nk' AS MaCV,

               N'ItemCode ch? nh?p kho' AS TenCV,

               N'ItemCode QC da ky duy?t nhung chua nh?p kho' AS MoTa,

               @ItemcodeChoNK AS SoLuong,

               N'ItemCode' AS DonVi,

               'clipboard-check' AS Icon

        UNION ALL

        SELECT 'kk_cho_duyet',

               N'Ki?m kÃª ch? duy?t',

               N'ItemCode da ki?m kÃª ch? b?n duy?t k?t qu?',

               @KKChoDuyet,

               N'ItemCode',

               'clipboard-list';

    END



    -- =========================================================================

    -- 26. GetTodoDetail

    -- itemcode_cho_nk: t? ERP_ChiTietNhapKhoNPL JOIN ki?m tables

    -- kk_cho_duyet: t? ERPPhieuKiemKe_NPL

    -- =========================================================================

    ELSE IF @Action = 'GetTodoDetail'

    BEGIN

        DECLARE @TodoType NVARCHAR(50) = ISNULL(@Itemcode, 'itemcode_cho_nk');



        IF @TodoType = 'itemcode_cho_nk'

        BEGIN

            SELECT

                ROW_NUMBER() OVER (ORDER BY t1.SoLoID, t1.MaNPL) AS STT,

                ISNULL(vt.MaVT, '') AS ItemCode,

                ISNULL(vt.ChiTiet, '') AS TenVT,

                ISNULL(t1.SoLoID, '') AS POMua,

                ISNULL(mau.MaMauVT, '') AS MaMauVT,

                ISNULL(mau.MauVT, '') AS MauVT,

                ISNULL(kv.KhoVai, '') AS WidthSize,

                CONVERT(VARCHAR(10), t1.NgayTaoNhapKho, 103) AS NgayTao,

                ISNULL(kh.TenKH, '') AS NCC,

                ISNULL(t1.SLTong, 0) AS SLMua,

                ISNULL(t1.SoLuongThucTeBanDau, 0) AS SLVe,

                'itemcode_cho_nk' AS Type

            FROM dbo.ERP_ChiTietNhapKhoNPL t1

            LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = t1.MaVTID

            LEFT JOIN dbo.ERP_MauVTTV mau ON mau.MauVTID = t1.MauVTID

            LEFT JOIN dbo.ERP_KhoVai kv ON kv.KhoVaiID = t1.KhoVaiID

            LEFT JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = t1.SoLoID

            LEFT JOIN dbo.ERP_KhachHangNK kh ON nk.NhaCungCap = kh.MaNhaCC

            WHERE ISNULL(t1.IsDuyetNK, 0) <> 1

              AND (

                  EXISTS (

                      SELECT 1 FROM dbo.QTY_KiemVaiV2 kv2

                      INNER JOIN dbo.QTY_KiemVaiV2_XacNhan kx

                          ON kv2.SoLoID = kx.SoLoID AND kv2.MaNPL = kx.MaNPL

                      WHERE kv2.SoLoID = t1.SoLoID AND kv2.MaNPL = t1.MaNPL

                        AND kv2.DuyetQC = 1

                  )

                  OR EXISTS (

                      SELECT 1 FROM dbo.Qty_KiemPL_XacNhan kp

                      WHERE kp.SoLoID = t1.SoLoID AND kp.MaNPL = t1.MaNPL

                        AND kp.Is_XN_SoLo = 1

                  )

              )

            ORDER BY t1.SoLoID, t1.MaNPL;

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

                ISNULL(mau.MaMauVT, '') AS MaMauVT,

                ISNULL(mau.MauVT, '') AS MauVT,

                ISNULL(kv.KhoVai, '') AS WidthSize,

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



    -- =========================================================================

    -- 27. GetTinhHinhKiemKe

    -- =========================================================================

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



    -- =========================================================================

    -- 28. GetKiemKeChiTiet

    -- =========================================================================

    ELSE IF @Action = 'GetKiemKeChiTiet'

    BEGIN

        SELECT 

            ds.PhieuVatTuKK AS MaPhieu,

            MIN(ds.NgayTao) AS NgayBatDau,

            CASE WHEN MIN(ds.IsNPL) = 1 THEN N'Kho NguyÃªn Li?u' ELSE N'Kho Ph? Li?u' END AS KhuVuc,

            COUNT(*) AS SoMa,

            SUM(ISNULL(p_check.DaKiemBit, 0)) AS DaKiem,

            COUNT(*) AS Tong,

            CAST(ROUND(SUM(ISNULL(p_check.DaKiemBit, 0)) * 100.0 / NULLIF(COUNT(*), 0), 1) AS DECIMAL(5, 1)) AS Pct,

            MAX(ISNULL(ds.NguoiTao, '')) AS NguoiPT,

            CASE 

                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = 0 THEN 3 -- Chua ki?m

                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = COUNT(*) THEN 1 -- Da ki?m d?

                ELSE 2 -- Dang ki?m

            END AS Status,

            CASE 

                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = 0 THEN N'Chua ki?m'

                WHEN SUM(ISNULL(p_check.DaKiemBit, 0)) = COUNT(*) THEN N'Da ki?m d?'

                ELSE N'Dang ki?m'

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



    ELSE

    BEGIN

        SELECT 'Unknown action: ' + ISNULL(@Action, 'NULL') AS Error;

    END

END

  
GO  
USE PMS_QLDH_VIKING_2025;
GO


CREATE   PROCEDURE [dbo].[SP_DashboardKho]
    @Action NVARCHAR(100),
    @TuNgay DATE,
    @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'GET_DASHBOARD_KHO'
    BEGIN
        SELECT
            CAST(NULL AS INT) AS SoLoID,
            CAST(N'' AS NVARCHAR(100)) AS SoLo,
            nk.POMua AS PO,
            nk.POMua,
            po_date.NgayDuKien,
            po_date.NgayDuKien AS NgayNhapKho_Update,
            ISNULL(order_info.MaDH, N'') AS MaDH,
            ISNULL(order_info.TenHang, N'') AS TenHang,
            ISNULL(order_info.TenHang, N'') AS MaHang,
            CAST(N'' AS NVARCHAR(200)) AS MaNPL,
            ISNULL(ct.SoLuongMa, 0) AS SoLuongMa,
            ISNULL(ct.TongSoLuong, 0) AS TongSoLuong,
            ISNULL(ct.TongSoLuong, 0) AS SoLuongSP,
            CAST(0 AS DECIMAL(18, 2)) AS SoLuongThung
        FROM
        (
            SELECT DISTINCT POMua
            FROM ERP_NhapKhoNPL
            WHERE (IsKiemKe = 0 OR IsKiemKe IS NULL)
              AND (IsDuyetNK = 0 OR IsDuyetNK IS NULL)
        ) nk
        OUTER APPLY
        (
            SELECT
                MIN(COALESCE(
                    TRY_CONVERT(DATE, NULLIF(ISNULL(po.NgayDuKienHV, po.NgayGiaoHangYC), N''), 103),
                    TRY_CAST(NULLIF(ISNULL(po.NgayDuKienHV, po.NgayGiaoHangYC), N'') AS DATE)
                )) AS NgayDuKien
            FROM ERP_POMH_PhieuMuaHang po
            WHERE po.POMua = nk.POMua
        ) po_date
        OUTER APPLY
        (
            SELECT
                ISNULL(STUFF((
                    SELECT DISTINCT N', ' + t2.MaDH
                    FROM ERP_POMH_PhieuMuaHang po_sub
                    INNER JOIN ERP_POMH_PhieuMuaHangDH t2 ON po_sub.MaPhieuMH = t2.MaPhieuMH
                    WHERE po_sub.POMua = nk.POMua
                      AND ISNULL(t2.MaDH, N'') <> N''
                    FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)'), 1, 2, N''), N'') AS MaDH,
                ISNULL(STUFF((
                    SELECT DISTINCT N', ' + h.TenHang
                    FROM ERP_POMH_PhieuMuaHang po_sub
                    INNER JOIN ERP_POMH_PhieuMuaHangDH t2 ON po_sub.MaPhieuMH = t2.MaPhieuMH
                    INNER JOIN DonHangTong dht ON t2.MaDH = dht.MaDH
                    INNER JOIN HangHoa h ON dht.MaHang = h.MaHang
                    WHERE po_sub.POMua = nk.POMua
                      AND ISNULL(h.TenHang, N'') <> N''
                    FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)'), 1, 2, N''), N'') AS TenHang
        ) order_info
        LEFT JOIN
        (
            SELECT
                POMua,
                COUNT(MaNPL) AS SoLuongMa,
                SUM(MaxSLTong) AS TongSoLuong
            FROM
            (
                SELECT
                    POMua,
                    MaNPL,
                    MAX(ISNULL(SLTong, 0)) AS MaxSLTong
                FROM ERP_ChiTietNhapKhoNPL
                GROUP BY POMua, MaNPL
            ) SubCT
            GROUP BY POMua
        ) ct ON nk.POMua = ct.POMua
        WHERE po_date.NgayDuKien >= @TuNgay
          AND po_date.NgayDuKien <= @DenNgay
        ORDER BY
            po_date.NgayDuKien ASC,
            nk.POMua ASC;
    END

    ELSE IF @Action = 'GET_WAREHOUSE_EFFICIENCY'
    BEGIN
          IF COL_LENGTH('dbo.ERP_VatTuCBM', 'MaONPL') IS NULL
              OR COL_LENGTH('dbo.ERP_ONPL', 'TenO') IS NULL
        BEGIN
            SELECT
                CAST(NULL AS INT) AS DayID,
                CAST(NULL AS NVARCHAR(200)) AS TenDay,
                CAST(NULL AS INT) AS KeID,
                CAST(NULL AS NVARCHAR(200)) AS TenKe,
                CAST(NULL AS INT) AS Module,
                CAST(NULL AS FLOAT) AS TongCBMSuDungTrongKe,
                CAST(NULL AS FLOAT) AS TongCBMTrongKe,
                CAST(NULL AS INT) AS SLVatTu
            WHERE 1 = 0;
        END
        ELSE
        BEGIN
        SELECT t1.KeID, ROUND(SUM(t2.Dai * t2.Cao * t2.Rong), 4) AS TongCBMTrongKe, t1.TenKe, t1.Module
        INTO #tempCBMKe_EFF
        FROM ERP_KeNPL t1
        LEFT JOIN ERP_ONPL t2 ON t1.KeID = t2.KeID
        WHERE t1.Module <> 3
        GROUP BY t1.KeID, t1.TenKe, t1.Module;

        SELECT BarCode
        INTO #TempXuatChuaThuHoi_EFF
        FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1
            FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        SELECT TOP (0) BarCode
        INTO #tempBarcodeSH_EFF
        FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tempBarcodeSH_EFF (BarCode)
            EXEC sp_executesql N'
                SELECT BarCode
                FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK, 0) - ISNULL(SLSoanHang_BC, 0) = 0;';
        END

        SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM), 4) AS TongCBMTrongO, COUNT(*) AS SLVatTu
        INTO #tempCBMO_EFF
        FROM ERP_VatTuCBM t1
        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
        WHERE t1.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi_EFF t2 WHERE t2.BarCode = t1.Barcode)
          AND EXISTS (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)
          AND NOT EXISTS (SELECT 1 FROM #tempBarcodeSH_EFF t7 WHERE t1.Barcode = t7.BarCode)
        GROUP BY t1.MaONPL, t2.KeID;

        SELECT
            t1.DayID,
            t4.TenDay,
            t1.KeID,
            t1.TenKe,
            t1.Module,
            ISNULL(SUM(t2.TongCBMTrongO), 0) AS TongCBMSuDungTrongKe,
            ISNULL(t3.TongCBMTrongKe, 0) AS TongCBMTrongKe,
            SUM(ISNULL(t2.SLVatTu, 0)) AS SLVatTu
        FROM ERP_KeNPL t1
        LEFT JOIN #tempCBMO_EFF t2 ON t1.KeID = t2.KeID
        LEFT JOIN #tempCBMKe_EFF t3 ON t1.KeID = t3.KeID
        LEFT JOIN ERP_DayNPL t4 ON t1.DayID = t4.DayID
        WHERE t1.Module <> 3
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE N'%l?i%'
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE N'%n%'
        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe
        ORDER BY t1.Module, t1.TenKe;

        DROP TABLE #tempCBMO_EFF;
        DROP TABLE #tempCBMKe_EFF;
        DROP TABLE #TempXuatChuaThuHoi_EFF;
        DROP TABLE #tempBarcodeSH_EFF;
        END
    END
ELSE IF @Action = 'GET_WAREHOUSE_CUSTOMERS'
    BEGIN

--	SELECT BarCode INTO #TempXuatChuaThuHoi FROM PhieuXuatHang t1 
--WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2  WHERE t2.MaLenh = t1.MaLenhSX  AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc) AND t1.Dot = t2.Dot )

--select BarCode into #tempBarcodeSH from ERP_SoanHangNPL_BarCode
--WHERE SLSoanHang_TK - SLSoanHang_BC = 0

--SELECT  MaNPL,SoLoID, sum(CBM) AS CBM into #tempBarcodeCBM FROM ERP_VatTuCBM t1
--left join ERP_ONPL t2 on t1.MaONPL = t2.TenO
--left join ERP_DayNPL t3 ON t2.DayID = t3.DayID
--WHERE  t1.MaONPL IS NOT NULL AND  t2.Module <> 3 AND  LOWER(t3.TenDay) NOT LIKE '%co%' AND LOWER(t3.TenDay) NOT LIKE N'%l?i%' AND LOWER(t3.TenDay) NOT LIKE N'%n%'
--and not exists (SELECT 1 FROM #TempXuatChuaThuHoi t2 WHERE t2.BarCode = t1.Barcode)
--and exists (select 1 from ERP_ChiTietNhapKhoNPL t6 where t1.Barcode = t6.BarCode)
--and not exists (select 1 from #tempBarcodeSH t7 where t1.Barcode = t7.BarCode )
--GROUP BY MaNPL,SoLoID

--DECLARE @TotalSKUInWarehouse FLOAT;
--SELECT @TotalSKUInWarehouse = CAST(COUNT(CASE WHEN ISNULL(MaNPL,'') <> ''  THEN CONCAT(MaNPL,'|',SoLoID) END) AS FLOAT)
--FROM #tempBarcodeCBM;

--SELECT  isnull(t3.MaKH,'') AS MaKH,isnull(t5.TenKH,'') AS TenKH, COUNT(CASE WHEN ISNULL(t1.MaNPL,'') <> ''  THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) AS SLVatTu, sum(CBM) AS CBMSDTrongKho ,
--CASE WHEN @TotalSKUInWarehouse > 0  THEN ROUND(COUNT(CASE WHEN ISNULL(t1.MaNPL,'') <> ''  THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) / @TotalSKUInWarehouse * 100, 2) ELSE 0  END AS PhanTramSKU
--FROM #tempBarcodeCBM t1
--left join (select distinct MaNPL,SoLoID from ERP_ChiTietNhapKhoNPL) t2 ON t1.MaNPL =  t2.MaNPL AND t1.SoLoID = t2.SoLoID
--left join ERP_NhapKhoNPL t3 on t2.SoLoID = t3.SoLoID
--left join KhachHang t5 ON t3.MaKH = t5.MaKH
--GROUP BY isnull(t3.MaKH,''),isnull(t5.TenKH,'')
--ORDER BY SLVatTu DESC

--drop table #tempBarcodeCBM
--drop table #tempBarcodeSH
--drop table #TempXuatChuaThuHoi
          IF COL_LENGTH('dbo.ERP_VatTuCBM', 'MaONPL') IS NULL
              OR COL_LENGTH('dbo.ERP_ONPL', 'TenO') IS NULL
        BEGIN
            SELECT
                CAST(NULL AS NVARCHAR(50)) AS MaKH,
                CAST(NULL AS NVARCHAR(200)) AS TenKH,
                CAST(NULL AS INT) AS SLVatTu,
                CAST(NULL AS FLOAT) AS CBMSDTrongKho,
                CAST(NULL AS FLOAT) AS PhanTramSKU
            WHERE 1 = 0;
        END
        ELSE
        BEGIN
        
        SELECT BarCode 
        INTO #TempXuatChuaThuHoi_CUS 
        FROM PhieuXuatHang t1 
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2  
            WHERE t2.MaLenh = t1.MaLenhSX  
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc) 
              AND t1.Dot = t2.Dot 
        );

        SELECT TOP (0) BarCode INTO #tempBarcodeSH_CUS FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tempBarcodeSH_CUS (BarCode)
            EXEC sp_executesql N'
                SELECT BarCode 
                FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK, 0) - ISNULL(SLSoanHang_BC, 0) = 0;';
        END

        SELECT  
            t1.MaNPL,
            t1.SoLoID, 
            SUM(t1.CBM) AS CBM 
        INTO #tempBarcodeCBM_CUS 
        FROM ERP_VatTuCBM t1
        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
        LEFT JOIN ERP_DayNPL t3 ON t2.DayID = t3.DayID
        WHERE t1.MaONPL IS NOT NULL 
          AND ISNULL(t2.Module, 0) <> 3 
          AND LOWER(ISNULL(t3.TenDay, N'')) NOT LIKE '%co%' 
          AND LOWER(ISNULL(t3.TenDay, N'')) NOT LIKE N'%l?i%' 
          AND LOWER(ISNULL(t3.TenDay, N'')) NOT LIKE N'%n%'
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi_CUS t2 WHERE t2.BarCode = t1.Barcode)
          AND EXISTS (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)
          AND NOT EXISTS (SELECT 1 FROM #tempBarcodeSH_CUS t7 WHERE t1.Barcode = t7.BarCode)
        GROUP BY t1.MaNPL, t1.SoLoID;

        DECLARE @TotalSKUInWarehouse FLOAT;
        SELECT @TotalSKUInWarehouse = CAST(COUNT(DISTINCT CASE WHEN ISNULL(MaNPL,'') <> '' THEN MaNPL END) AS FLOAT)
        FROM #tempBarcodeCBM_CUS;

        SELECT  
            ISNULL(t3.MaKH,'') AS MaKH,
            ISNULL(t5.TenKH,'') AS TenKH, 
            COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN t1.MaNPL END) AS SLVatTu, 
            SUM(t1.CBM) AS CBMSDTrongKho,
            CASE WHEN @TotalSKUInWarehouse > 0  
                 THEN ROUND(COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN t1.MaNPL END) / @TotalSKUInWarehouse * 100, 2) 
                 ELSE 0  
            END AS PhanTramSKU
        FROM #tempBarcodeCBM_CUS t1
        LEFT JOIN (
SELECT DISTINCT MaNPL, SoLoID 
            FROM ERP_ChiTietNhapKhoNPL
        ) t2 ON t1.MaNPL = t2.MaNPL AND t1.SoLoID = t2.SoLoID
        LEFT JOIN ERP_NhapKhoNPL t3 ON t2.SoLoID = t3.SoLoID
        LEFT JOIN KhachHang t5 ON t3.MaKH = t5.MaKH
        GROUP BY ISNULL(t3.MaKH,''), ISNULL(t5.TenKH,'')
        ORDER BY SLVatTu DESC;

        DROP TABLE #tempBarcodeCBM_CUS;
        DROP TABLE #tempBarcodeSH_CUS;
        DROP TABLE #TempXuatChuaThuHoi_CUS;
        END
    END

    ELSE IF @Action = 'GET_WAREHOUSE_SUMMARY'
    BEGIN
        DECLARE @TotalCapNPL FLOAT = 0;
        DECLARE @TotalCapPL FLOAT = 0;
        DECLARE @UsedNPL FLOAT = 0;
        DECLARE @UsedPL FLOAT = 0;

        SELECT @TotalCapNPL = ISNULL(SUM(CAST(ISNULL(Dai, 0) * ISNULL(Rong, 0) * ISNULL(Cao, 0) AS FLOAT)), 0)
        FROM dbo.ERP_ONPL
        WHERE Module = 1
          AND ISNULL(TenO, N'') NOT LIKE '%N%'
          AND ISNULL(TenO, N'') NOT LIKE '%Co%'
          AND ISNULL(TenO, N'') NOT LIKE '%L1%';

        SELECT @TotalCapPL = ISNULL(SUM(CAST(ISNULL(Dai, 0) * ISNULL(Rong, 0) * ISNULL(Cao, 0) AS FLOAT)), 0)
        FROM dbo.ERP_ONPL
        WHERE Module = 2
          AND ISNULL(TenO, N'') NOT LIKE '%N%'
          AND ISNULL(TenO, N'') NOT LIKE '%Co%'
          AND ISNULL(TenO, N'') NOT LIKE '%L1%';

        IF COL_LENGTH('dbo.ERP_VatTuCBM', 'MaONPL') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_ONPL', 'TenO') IS NOT NULL
        BEGIN
            SELECT
                @UsedNPL = ISNULL(SUM(CASE WHEN ISNULL(o.Module, 0) = 1 THEN CAST(ISNULL(t1.CBM, 0) AS FLOAT) ELSE 0 END), 0),
                @UsedPL = ISNULL(SUM(CASE WHEN ISNULL(o.Module, 0) = 2 THEN CAST(ISNULL(t1.CBM, 0) AS FLOAT) ELSE 0 END), 0)
            FROM dbo.ERP_VatTuCBM t1
            LEFT JOIN dbo.ERP_ONPL o ON o.TenO = t1.MaONPL
            WHERE t1.MaONPL IS NOT NULL
              AND ISNULL(t1.Status, 0) = 1
              AND ISNULL(o.TenO, N'') NOT LIKE '%N%'
              AND ISNULL(o.TenO, N'') NOT LIKE '%Co%'
              AND ISNULL(o.TenO, N'') NOT LIKE '%L1%'
              AND NOT EXISTS (
                  SELECT 1
                  FROM dbo.PhieuXuatHang t2
                  WHERE t2.BarCode = t1.Barcode
              );
        END
        ELSE
        BEGIN
            SET @UsedNPL = 0;
            SET @UsedPL = 0;
        END

        ;WITH Capacity AS
        (
            SELECT
                @TotalCapNPL AS CapNPL,
                @TotalCapPL AS CapPL,
                (@TotalCapNPL + @TotalCapPL) AS TotalCapacity,
                @UsedNPL AS UsedNPL,
                @UsedPL AS UsedPL,
                CASE WHEN @TotalCapNPL > @UsedNPL THEN (@TotalCapNPL - @UsedNPL) ELSE 0 END AS FreeNPL,
                CASE WHEN @TotalCapPL > @UsedPL THEN (@TotalCapPL - @UsedPL) ELSE 0 END AS FreePL,
                CASE
                    WHEN (@TotalCapNPL + @TotalCapPL) > (@UsedNPL + @UsedPL)
                    THEN ((@TotalCapNPL + @TotalCapPL) - (@UsedNPL + @UsedPL))
                    ELSE 0
                END AS TotalFreeCapacity
        )
        SELECT
            N'NPL' AS MaterialType,
            UsedNPL AS TotalStored,
            TotalCapacity,
            CASE WHEN TotalCapacity > 0 THEN (UsedNPL / TotalCapacity) * 100 ELSE 0 END AS UsedPercent,
            TotalFreeCapacity AS FreeCapacity
        FROM Capacity

        UNION ALL

        SELECT
            N'PL' AS MaterialType,
            UsedPL AS TotalStored,
            TotalCapacity,
            CASE WHEN TotalCapacity > 0 THEN (UsedPL / TotalCapacity) * 100 ELSE 0 END AS UsedPercent,
            TotalFreeCapacity AS FreeCapacity
        FROM Capacity

        UNION ALL

        SELECT
            N'Total' AS MaterialType,
            (UsedNPL + UsedPL) AS TotalStored,
            TotalCapacity,
            CASE WHEN TotalCapacity > 0 THEN ((UsedNPL + UsedPL) / TotalCapacity) * 100 ELSE 0 END AS UsedPercent,
            TotalFreeCapacity AS FreeCapacity
        FROM Capacity;
    END
ELSE IF @Action = 'GET_OVERALL_CAPACITY'
    BEGIN
          IF COL_LENGTH('dbo.ERP_VatTuCBM', 'MaONPL') IS NULL
              OR COL_LENGTH('dbo.ERP_ONPL', 'TenO') IS NULL
        BEGIN
            SELECT
                CAST(0 AS FLOAT) AS TotalCapacity,
                CAST(0 AS FLOAT) AS CapacityNPL,
                CAST(0 AS FLOAT) AS CapacityPL,
                CAST(0 AS FLOAT) AS UsedNPL,
                CAST(0 AS FLOAT) AS UsedPL,
                CAST(0 AS FLOAT) AS TotalVatTuNPL,
                CAST(0 AS FLOAT) AS TotalVatTuPL,
                CAST(0 AS FLOAT) AS TotalVatTu,
                CAST(0 AS FLOAT) AS TotalFreeCapacity,
                CAST(0 AS FLOAT) AS PercentNPL,
                CAST(0 AS FLOAT) AS PercentPL,
                CAST(0 AS FLOAT) AS TotalPercent,
                CAST(100 AS FLOAT) AS FreePercent;
        END
        ELSE
        BEGIN
        SELECT t1.KeID, ROUND(SUM(t2.Dai * t2.Cao * t2.Rong), 4) AS TongCBMTrongKe, t1.TenKe, t1.Module
        INTO #tempCBMKe_OVR
        FROM ERP_KeNPL t1
        LEFT JOIN ERP_ONPL t2 ON t1.KeID = t2.KeID
        WHERE t1.Module <> 3
        GROUP BY t1.KeID, t1.TenKe, t1.Module;

        SELECT BarCode
        INTO #TempXuatChuaThuHoi_OVR
        FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        SELECT TOP (0) BarCode
        INTO #tempBarcodeSH_OVR
        FROM ERP_SoanHangNPL_BarCode;

        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tempBarcodeSH_OVR (BarCode)
            EXEC sp_executesql N'
                SELECT BarCode
                FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK, 0) - ISNULL(SLSoanHang_BC, 0) = 0;';
        END

        SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM), 4) AS TongCBMTrongO, COUNT(*) AS SLVatTu
        INTO #tempCBMO_OVR
        FROM ERP_VatTuCBM t1
        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
        WHERE t1.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi_OVR t2 WHERE t2.BarCode = t1.Barcode)
          AND EXISTS (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)
          AND NOT EXISTS (SELECT 1 FROM #tempBarcodeSH_OVR t7 WHERE t1.Barcode = t7.BarCode)
        GROUP BY t1.MaONPL, t2.KeID;

        SELECT
            t1.DayID,
            t4.TenDay,
            t1.KeID,
            t1.TenKe,
            t1.Module,
            ISNULL(SUM(t2.TongCBMTrongO), 0) AS TongCBMSuDungTrongKe,
            ISNULL(t3.TongCBMTrongKe, 0) AS TongCBMTrongKe,
            SUM(ISNULL(t2.SLVatTu, 0)) AS SLVatTu
        INTO #tempBaseResult
        FROM ERP_KeNPL t1
        LEFT JOIN #tempCBMO_OVR t2 ON t1.KeID = t2.KeID
        LEFT JOIN #tempCBMKe_OVR t3 ON t1.KeID = t3.KeID
        LEFT JOIN ERP_DayNPL t4 ON t1.DayID = t4.DayID
        WHERE t1.Module <> 3
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE N'%l?i%'
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE N'%n%'
        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe;

        DECLARE @CapNPL FLOAT = 0;
        DECLARE @CapPL FLOAT = 0;
        DECLARE @UsedNPL_OVR FLOAT = 0;
        DECLARE @UsedPL_OVR FLOAT = 0;
        DECLARE @TotalVatTuNPL FLOAT = 0;
        DECLARE @TotalVatTuPL FLOAT = 0;

        SELECT
            @CapNPL = ISNULL(SUM(CASE WHEN Module = 1 THEN TongCBMTrongKe ELSE 0 END), 0),
            @CapPL = ISNULL(SUM(CASE WHEN Module = 2 THEN TongCBMTrongKe ELSE 0 END), 0),
            @UsedNPL_OVR = ISNULL(SUM(CASE WHEN Module = 1 THEN TongCBMSuDungTrongKe ELSE 0 END), 0),
            @UsedPL_OVR = ISNULL(SUM(CASE WHEN Module = 2 THEN TongCBMSuDungTrongKe ELSE 0 END), 0)
        FROM #tempBaseResult;

        SELECT DISTINCT t1.MaNPL
        INTO #TempDistinctMaNPL_OVR
        FROM ERP_VatTuCBM t1
        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
        WHERE t1.MaONPL IS NOT NULL
          AND ISNULL(t1.MaNPL, N'') <> N''
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi_OVR t2 WHERE t2.BarCode = t1.Barcode)
          AND EXISTS (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)
          AND NOT EXISTS (SELECT 1 FROM #tempBarcodeSH_OVR t7 WHERE t1.Barcode = t7.BarCode);


        SELECT 
            @TotalVatTuNPL = ISNULL(SUM(CASE WHEN clvt.IsNPL = 1 THEN 1 ELSE 0 END), 0),
            @TotalVatTuPL  = ISNULL(SUM(CASE WHEN clvt.IsNPL = 0 THEN 1 ELSE 0 END), 0)
        FROM #TempDistinctMaNPL_OVR tmp
        LEFT JOIN ChungLoaiVatTu clvt 
            ON clvt.MaCLVT = LEFT(tmp.MaNPL, CHARINDEX('@', tmp.MaNPL + '@') - 1);


    
        SELECT
            (@CapNPL + @CapPL) AS TotalCapacity,
            @CapNPL AS CapacityNPL,
            @CapPL AS CapacityPL,
            @UsedNPL_OVR AS UsedNPL,
            @UsedPL_OVR AS UsedPL,
            @TotalVatTuNPL AS TotalVatTuNPL,
            @TotalVatTuPL AS TotalVatTuPL,
            (@TotalVatTuNPL + @TotalVatTuPL) AS TotalVatTu,
            CASE WHEN (@CapNPL + @CapPL) > (@UsedNPL_OVR + @UsedPL_OVR) THEN ((@CapNPL + @CapPL) - (@UsedNPL_OVR + @UsedPL_OVR)) ELSE 0 END AS TotalFreeCapacity,
            CASE WHEN @CapNPL > 0 THEN ROUND((@UsedNPL_OVR / @CapNPL) * 100, 2) ELSE 0 END AS PercentNPL,
            CASE WHEN @CapPL > 0 THEN ROUND((@UsedPL_OVR / @CapPL) * 100, 2) ELSE 0 END AS PercentPL,
            CASE
                WHEN (@CapNPL > 0 OR @CapPL > 0)
                THEN ROUND((CASE WHEN @CapNPL > 0 THEN (@UsedNPL_OVR / @CapNPL) * 100 ELSE 0 END)
                         + (CASE WHEN @CapPL > 0 THEN (@UsedPL_OVR / @CapPL) * 100 ELSE 0 END), 2)
                ELSE 0
            END AS TotalPercent,
            CASE
                WHEN (@CapNPL > 0 OR @CapPL > 0)
                THEN ROUND(100 - ((CASE WHEN @CapNPL > 0 THEN (@UsedNPL_OVR / @CapNPL) * 100 ELSE 0 END)
                               + (CASE WHEN @CapPL > 0 THEN (@UsedPL_OVR / @CapPL) * 100 ELSE 0 END)), 2)
                ELSE 100
            END AS FreePercent;

        DROP TABLE #tempCBMO_OVR;
        DROP TABLE #tempCBMKe_OVR;
        DROP TABLE #TempXuatChuaThuHoi_OVR;
        DROP TABLE #tempBarcodeSH_OVR;
        DROP TABLE #tempBaseResult;
        DROP TABLE #TempDistinctMaNPL_OVR;
        END
    END
    ELSE IF @Action = 'GET_CHUAN_BI_XUAT'
    BEGIN
        SET NOCOUNT ON;
        IF OBJECT_ID('tempdb..#chuacaptempDH') IS NOT NULL DROP TABLE #chuacaptempDH;
        IF OBJECT_ID('tempdb..#chuacaptempNgayVC') IS NOT NULL DROP TABLE #chuacaptempNgayVC;
        IF OBJECT_ID('tempdb..#chuacaptempCanDoiSXNL') IS NOT NULL DROP TABLE #chuacaptempCanDoiSXNL;
        IF OBJECT_ID('tempdb..#chuacaptempNgayVCDH') IS NOT NULL DROP TABLE #chuacaptempNgayVCDH;

        SELECT DISTINCT MaGop AS MaDH, NgayDKVC
        INTO #chuacaptempNgayVCDH
        FROM DongHangTongPO t1
        LEFT JOIN GopDonHang t2 ON t1.MaDH = t2.MaDH;

        SELECT MaDH,
               NgayDKVC = STUFF(
                  (SELECT DISTINCT ',' + CONVERT(nvarchar, NgayDKVC, 103)
                   FROM #chuacaptempNgayVCDH t2
                   WHERE t1.MaDH = t2.MaDH
                   FOR XML PATH('')), 1, 1, '' )
        INTO #chuacaptempNgayVC
        FROM #chuacaptempNgayVCDH t1
        GROUP BY MaDH;

        SELECT DISTINCT t1.MaDH, TenKH, t2.TenHang AS MaHang
        INTO #chuacaptempDH
        FROM DonHangTong t1
        LEFT JOIN HangHoa t2 ON t1.MaHang = t2.MaHang AND t1.MaKH = t2.MaKH
        LEFT JOIN KhachHang t3 ON t1.MaKH = t3.MaKH;

	
       SELECT DISTINCT t1.MaDH, t1.MaLenh,t1. MaDVSX, MaLenhSanXuat, t1.MaGop, GhiChu, NgayDKVC
        INTO #chuacaptempCanDoiSXNL
        FROM CanDoiDonViSanXuat t1
		inner join  [PMS_VIKING_2025].dbo.ERP_LENHSX t3 on t1.MaLenh = t3.MaLenh and t3.Status<>3
        LEFT JOIN #chuacaptempNgayVC t2 ON t1.MaGop = t2.MaDH;
        SELECT
            t2.MaDH,
            t2.MaGop,
            t1.MaLenhSanXuat,
            t2.MaLenh,
            t2.MaDVSX,
            SUM(CapPhat) AS SoMet,
            MaHang,
            TenKH AS KhachHang,
            CONCAT(t1.MaLenhSanXuat, ' | ', t2.MaDH) AS Display,
            IsNPL AS NPL,
            t7.SoLuong SoLuong,
            GiaCong,
            ISNULL(t2.GhiChu, '') AS GhiChu,
            NgayDKVC
        INTO #chuacaptempPYCTT
        FROM dbo.CanDoiDinhMucNPL t1
        INNER JOIN #chuacaptempCanDoiSXNL t2 ON t1.MaLenhSanXuat = t2.MaLenhSanXuat AND t1.MaDH = t2.MaGop
        INNER JOIN DonViSanXuat t3 ON t2.MaDVSX = t3.MaDVSX
        LEFT JOIN #chuacaptempDH t4 ON t2.MaDH = t4.MaDH
        LEFT JOIN ChungLoaiVatTu t6 ON t1.MaNhomVT = t6.MaCLVT
        LEFT JOIN (
            SELECT MaGop, MaLenhSanXuat, SUM(SoLuong) AS SoLuong
            FROM CanDoiDonViSanXuat
            GROUP BY MaGop, MaLenhSanXuat
        ) t7 ON t1.MaDH = t7.MaGop AND t1.MaLenhSanXuat = t7.MaLenhSanXuat
        WHERE GiaCong = 0
          AND IsNPL = 1
          AND MaDot IS NOT NULL
          AND t1.IsXacNhan = 1
        GROUP BY t2.MaLenh, t2.MaDVSX, MaHang, TenKH, t1.MaLenhSanXuat, t2.MaGop, t2.MaDH, IsNPL, t7.SoLuong, GiaCong, t2.GhiChu, NgayDKVC;

        SELECT PhieuXH, 1 AS SLNhap
        INTO #chuacaptempSLXuatYC
        FROM XacNhanHangXuat
        WHERE IsNPL = 1;

        SELECT
            MIN(NgayCap) AS NgayCap,
            SUM(SLDK) AS SLDK,
            MaLenhSX,
            MIN(ISNULL(SLNhap, 0)) AS SLNhap
        INTO #chuacaptempDKVTNL
        FROM ERP_PhieuDangKyXuatVT t1
        LEFT JOIN #chuacaptempSLXuatYC t2 ON t1.PhieuDK = t2.PhieuXH
        WHERE IsNPL = 1
          AND SLDK > 0
        GROUP BY MaLenhSX;

        SELECT MIN(NgayCap) AS NgayCap, MaLenhSX, SLNhap AS CheckNCap
        INTO #chuacaptempCheckNCap
        FROM #chuacaptempDKVTNL
        GROUP BY MaLenhSX, SLNhap;

        SELECT
            t1.MaLenhSanXuat,
            CAST(MaLenh AS int) AS MaLenh,
            t1.MaDVSX,
            t1.MaHang AS TenHang,
            t1.KhachHang,
            t1.MaLenhSanXuat AS Display,
            COALESCE(
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TTCAT' THEN t5.KH_Date END),
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_BOM' THEN t5.KH_Date END),
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_LENHSX' THEN t5.KH_Date END),
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_LENHSX' THEN t5.TT_Date END)
            ) AS KHCat,
            DATEADD(day, 7, COALESCE(
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TTCAT' THEN t5.KH_Date END),
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_BOM' THEN t5.KH_Date END),
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_LENHSX' THEN t5.KH_Date END),
                MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_LENHSX' THEN t5.TT_Date END)
            )) AS DuKienCat
        FROM #chuacaptempPYCTT t1
        INNER JOIN #chuacaptempCheckNCap t3 ON t1.MaLenhSanXuat = t3.MaLenhSX
        LEFT JOIN WIP_DonHang_Chuyen t4 ON t4.LenhSX = t1.MaLenh
        LEFT JOIN WIP_DonHang_Chuyen_TechProgress t5 ON t5.WIPId = t4.WIPId
        WHERE NOT EXISTS (SELECT 1 FROM PhieuXuatHang t2 WHERE t1.MaLenhSanXuat = t2.MaLenhSX)
        GROUP BY t1.MaLenhSanXuat, CAST(MaLenh AS int), t1.MaDVSX, t1.MaHang, t1.KhachHang, t1.MaLenhSanXuat
        ORDER BY COALESCE(
            MAX(CASE WHEN UPPER(t5.StepCode) = 'TTCAT' THEN t5.KH_Date END),
            MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_BOM' THEN t5.KH_Date END),
            MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_LENHSX' THEN t5.KH_Date END),
            MAX(CASE WHEN UPPER(t5.StepCode) = 'TT_LENHSX' THEN t5.TT_Date END)
        ) DESC;

        DROP TABLE #chuacaptempPYCTT;
        DROP TABLE #chuacaptempSLXuatYC;
        DROP TABLE #chuacaptempDKVTNL;
        DROP TABLE #chuacaptempCheckNCap;
        DROP TABLE #chuacaptempNgayVCDH;
        DROP TABLE #chuacaptempNgayVC;
        DROP TABLE #chuacaptempCanDoiSXNL;
        DROP TABLE #chuacaptempDH;
    END

    ELSE IF @Action = 'GET_DANG_XUAT'
    BEGIN
        SET NOCOUNT ON;
        DECLARE @FromDate DATE = ISNULL(@TuNgay, CAST(DATEADD(day, -10, GETDATE()) AS DATE));
        DECLARE @ToDate DATE = ISNULL(@DenNgay, CAST(GETDATE() AS DATE));

        IF @FromDate > @ToDate
        BEGIN
            DECLARE @SwapDate DATE = @FromDate;
            SET @FromDate = @ToDate;
            SET @ToDate = @SwapDate;
        END

        IF OBJECT_ID('tempdb..#tempTenHang') IS NOT NULL DROP TABLE #tempTenHang;
        IF OBJECT_ID('tempdb..#TempXuatHangNPL') IS NOT NULL DROP TABLE #TempXuatHangNPL;

        SELECT DISTINCT t1.MaGop, t3.TenHang, t5.TenKH
        INTO #tempTenHang
        FROM GopDonHang t1
        INNER JOIN DonHangTong t2 ON t1.MaDH = t2.MaDH
        LEFT JOIN HangHoa t3 ON t2.MaHang = t3.MaHang AND t2.MaKH = t3.MaKH
        LEFT JOIN KhachHang t5 ON t2.MaKH = t5.MaKH
        WHERE EXISTS (SELECT 1 FROM PhieuXuatHang t4 WHERE t4.MaGop = t1.MaGop);

        SELECT
            MaLenh,
            MaLenhSX,
            MaGop,
            MAX(NgayXuatHang) AS NgayXuatHang,
            MaNPL,
            SUM(SLNhap) AS SLXuat
        INTO #TempXuatHangNPL
        FROM PhieuXuatHang
        WHERE Moudule = 0
          AND NPL = 1
          AND NgayXuatHang >= @FromDate
          AND NgayXuatHang < DATEADD(DAY, 1, @ToDate)
        GROUP BY MaLenh, MaLenhSX, MaGop, MaNPL;

        SELECT
            MaLenh,
            MaLenhSX,
            t1.MaGop,
                        t2.TenKH,
                        t2.TenHang,
            NgayXuatHang AS NgayXuatHang,
            ROUND(t1.SLXuat, 4) AS SLXuat,
            COALESCE(
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TTCAT' THEN t4.KH_Date END),
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_BOM' THEN t4.KH_Date END),
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_LENHSX' THEN t4.KH_Date END),
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_LENHSX' THEN t4.TT_Date END)
            ) AS KHCat,
            DATEADD(day, 7, COALESCE(
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TTCAT' THEN t4.KH_Date END),
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_BOM' THEN t4.KH_Date END),
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_LENHSX' THEN t4.KH_Date END),
                MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_LENHSX' THEN t4.TT_Date END)
            )) AS DuKienCat
        FROM #TempXuatHangNPL t1
                INNER JOIN #tempTenHang t2 ON t1.MaGop = t2.MaGop
        LEFT JOIN WIP_DonHang_Chuyen t3 ON t3.LenhSX = t1.MaLenh
        LEFT JOIN WIP_DonHang_Chuyen_TechProgress t4 ON t4.WIPId = t3.WIPId
        GROUP BY MaLenh, MaLenhSX, t1.MaGop, t2.TenKH, t2.TenHang, NgayXuatHang, t1.SLXuat
        ORDER BY NgayXuatHang DESC, COALESCE(
            MAX(CASE WHEN UPPER(t4.StepCode) = 'TTCAT' THEN t4.KH_Date END),
            MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_BOM' THEN t4.KH_Date END),
            MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_LENHSX' THEN t4.KH_Date END),
            MAX(CASE WHEN UPPER(t4.StepCode) = 'TT_LENHSX' THEN t4.TT_Date END)
        ) DESC;

        DROP TABLE #tempTenHang;
        DROP TABLE #TempXuatHangNPL;
    END

    ELSE
    BEGIN
        RAISERROR(N'Unsupported action for SP_DashboardKho.', 16, 1);
    END
END


CREATE PROCEDURE dbo.SP_DASHBOARD_KHO_DESKTOP
    @Action       NVARCHAR(100),
    @TuNgay       DATETIME = NULL,
    @DenNgay      DATETIME = NULL,
    @Ngay         DATETIME = NULL,
    @IsNhieuNhat  INT      = 1,
    @LoaiNPL      INT      = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- ===================================================================
    -- [1] GetOverallCapacity
    -- ===================================================================
    IF @Action = 'GetOverallCapacity'
    BEGIN
        IF COL_LENGTH('dbo.ERP_VatTuCBM', 'MaONPL') IS NULL OR COL_LENGTH('dbo.ERP_ONPL', 'TenO') IS NULL
        BEGIN
            SELECT CAST(0 AS FLOAT) AS TotalCapacity, CAST(0 AS FLOAT) AS CapacityNPL, CAST(0 AS FLOAT) AS CapacityPL,
                   CAST(0 AS FLOAT) AS UsedNPL, CAST(0 AS FLOAT) AS UsedPL, CAST(0 AS FLOAT) AS TotalVatTuNPL,
                   CAST(0 AS FLOAT) AS TotalVatTuPL, CAST(0 AS FLOAT) AS TotalVatTu, CAST(0 AS FLOAT) AS TotalFreeCapacity,
                   CAST(0 AS FLOAT) AS PercentNPL, CAST(0 AS FLOAT) AS PercentPL, CAST(0 AS FLOAT) AS TotalPercent,
                   CAST(100 AS FLOAT) AS FreePercent;
            RETURN;
        END
        SELECT t1.KeID, ROUND(SUM(t2.Dai * t2.Cao * t2.Rong), 4) AS TongCBMTrongKe, t1.TenKe, t1.Module
        INTO #tempCBMKe FROM ERP_KeNPL t1
        LEFT JOIN ERP_ONPL t2 ON t1.KeID = t2.KeID
        WHERE t1.Module <> 3
        GROUP BY t1.KeID, t1.TenKe, t1.Module;

        SELECT BarCode INTO #TempXuatChuaThuHoi FROM PhieuXuatHang t1
        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc) AND t1.Dot = t2.Dot);

        SELECT TOP (0) BarCode INTO #tempBarcodeSH FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode', 'SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tempBarcodeSH(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode WHERE ISNULL(SLSoanHang_TK,0)-ISNULL(SLSoanHang_BC,0)=0;';
        END

        SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM), 4) AS TongCBMTrongO, COUNT(*) AS SLVatTu
        INTO #tempCBMO FROM ERP_VatTuCBM t1
        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
        WHERE t1.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi t WHERE t.BarCode = t1.Barcode)
          AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)
          AND NOT EXISTS (SELECT 1 FROM #tempBarcodeSH t WHERE t.BarCode = t1.Barcode)
        GROUP BY t1.MaONPL, t2.KeID;

        SELECT t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module,
               ISNULL(SUM(t2.TongCBMTrongO), 0) AS TongCBMSuDungTrongKe,
               ISNULL(t3.TongCBMTrongKe, 0) AS TongCBMTrongKe, SUM(ISNULL(t2.SLVatTu, 0)) AS SLVatTu
        INTO #tempBaseResult FROM ERP_KeNPL t1
        LEFT JOIN #tempCBMO t2 ON t1.KeID = t2.KeID
        LEFT JOIN #tempCBMKe t3 ON t1.KeID = t3.KeID
        LEFT JOIN ERP_DayNPL t4 ON t1.DayID = t4.DayID
        WHERE t1.Module <> 3
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE N'%l?i%'
          AND LOWER(ISNULL(t4.TenDay, N'')) NOT LIKE N'%n%'
        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe;

        DECLARE @CapNPL FLOAT=0, @CapPL FLOAT=0, @UsedNPL FLOAT=0, @UsedPL FLOAT=0, @TotalVatTuNPL FLOAT=0, @TotalVatTuPL FLOAT=0;
        SELECT @CapNPL = ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMTrongKe ELSE 0 END),0),
               @CapPL  = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMTrongKe ELSE 0 END),0),
               @UsedNPL= ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMSuDungTrongKe ELSE 0 END),0),
               @UsedPL = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMSuDungTrongKe ELSE 0 END),0),
               @TotalVatTuNPL = ISNULL(SUM(CASE WHEN Module=1 THEN SLVatTu ELSE 0 END),0),
               @TotalVatTuPL  = ISNULL(SUM(CASE WHEN Module=2 THEN SLVatTu ELSE 0 END),0)
        FROM #tempBaseResult;

        SELECT (@CapNPL+@CapPL) AS TotalCapacity, @CapNPL AS CapacityNPL, @CapPL AS CapacityPL,
               @UsedNPL AS UsedNPL, @UsedPL AS UsedPL,
               @TotalVatTuNPL AS TotalVatTuNPL, @TotalVatTuPL AS TotalVatTuPL,
               (@TotalVatTuNPL+@TotalVatTuPL) AS TotalVatTu,
               CASE WHEN (@CapNPL+@CapPL)>(@UsedNPL+@UsedPL) THEN (@CapNPL+@CapPL)-(@UsedNPL+@UsedPL) ELSE 0 END AS TotalFreeCapacity,
               CASE WHEN (@CapNPL+@CapPL)>0 THEN ROUND((@UsedNPL/(@CapNPL+@CapPL))*100,2) ELSE 0 END AS PercentNPL,
               CASE WHEN (@CapNPL+@CapPL)>0 THEN ROUND((@UsedPL /(@CapNPL+@CapPL))*100,2) ELSE 0 END AS PercentPL,
               CASE WHEN (@CapNPL+@CapPL)>0 THEN ROUND(((@UsedNPL+@UsedPL)/(@CapNPL+@CapPL))*100,2) ELSE 0 END AS TotalPercent,
               CASE WHEN (@CapNPL+@CapPL)>0 THEN ROUND(100-((@UsedNPL+@UsedPL)/(@CapNPL+@CapPL))*100,2) ELSE 100 END AS FreePercent;

        DROP TABLE #tempCBMO; DROP TABLE #tempCBMKe; DROP TABLE #TempXuatChuaThuHoi;
        DROP TABLE #tempBarcodeSH; DROP TABLE #tempBaseResult;
        RETURN;
    END

    -- ===================================================================
    -- [2] GetFlowTrend12T - Xuat-Nhap-Ton 12 thang
    -- ===================================================================
    IF @Action = 'GetFlowTrend12T'
    BEGIN
        DECLARE @StartDate12T DATE = DATEADD(MONTH, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1));

        ;WITH Months AS (
            SELECT TOP 12
                YEAR(DATEADD(MONTH, number, @StartDate12T)) AS Nam,
                MONTH(DATEADD(MONTH, number, @StartDate12T)) AS Thang
            FROM master..spt_values WHERE type='P' AND number BETWEEN 0 AND 11
        ),
        TonDauKy AS (
            SELECT ISNULL(SUM(SL),0) AS TonDau FROM (
                SELECT SUM(ISNULL(SoLuongThucTeBanDau,0)) AS SL FROM ERP_ChiTietNhapKhoNPL WHERE NgayNhapKho < @StartDate12T
                UNION ALL
                SELECT -SUM(ISNULL(SLNhap,0)) FROM PhieuXuatHang WHERE ModuleXH=1 AND NgayXuatHang < @StartDate12T
                UNION ALL
                SELECT SUM(ISNULL(ThuHoi,0)) FROM PhieuThuHoiNPL WHERE NgayTH < @StartDate12T
            ) t
        ),
        NhapTheoThang AS (
            SELECT YEAR(NgayNhapKho) AS Nam, MONTH(NgayNhapKho) AS Thang,
                   SUM(ISNULL(SoLuongThucTeBanDau,0)) AS TotalIn
            FROM ERP_ChiTietNhapKhoNPL
            WHERE NgayNhapKho >= @StartDate12T
            GROUP BY YEAR(NgayNhapKho), MONTH(NgayNhapKho)
        ),
        XuatTheoThang AS (
            SELECT YEAR(NgayXuatHang) AS Nam, MONTH(NgayXuatHang) AS Thang,
                   SUM(ISNULL(SLNhap,0)) AS TotalOut
            FROM PhieuXuatHang WHERE ModuleXH=1 AND NgayXuatHang >= @StartDate12T
            GROUP BY YEAR(NgayXuatHang), MONTH(NgayXuatHang)
        ),
        ThuHoiTheoThang AS (
            SELECT YEAR(NgayTH) AS Nam, MONTH(NgayTH) AS Thang,
                   SUM(ISNULL(ThuHoi,0)) AS TotalTH
            FROM PhieuThuHoiNPL WHERE NgayTH >= @StartDate12T
            GROUP BY YEAR(NgayTH), MONTH(NgayTH)
        ),
        Combined AS (
            SELECT m.Nam, m.Thang, ISNULL(n.TotalIn,0) AS TotalIn, ISNULL(x.TotalOut,0) AS TotalOut, ISNULL(th.TotalTH,0) AS TotalTH
            FROM Months m
            LEFT JOIN NhapTheoThang n ON n.Nam=m.Nam AND n.Thang=m.Thang
            LEFT JOIN XuatTheoThang x ON x.Nam=m.Nam AND x.Thang=m.Thang
            LEFT JOIN ThuHoiTheoThang th ON th.Nam=m.Nam AND th.Thang=m.Thang
        )
        SELECT c.Nam, c.Thang, c.TotalIn, c.TotalOut,
            ISNULL(d.TonDau,0) + SUM(c.TotalIn + c.TotalTH - c.TotalOut)
                OVER (ORDER BY c.Nam, c.Thang ROWS UNBOUNDED PRECEDING) AS TotalStock
        FROM Combined c CROSS JOIN TonDauKy d
        ORDER BY c.Nam, c.Thang;
        RETURN;
    END

    -- ===================================================================
    -- [3] GetActivityCalendar
    -- ===================================================================
    IF @Action = 'GetActivityCalendar'
    BEGIN
        IF @TuNgay IS NULL  SET @TuNgay  = DATEADD(DAY, -90, CAST(GETDATE() AS DATE));
        IF @DenNgay IS NULL SET @DenNgay = DATEADD(DAY,  30, CAST(GETDATE() AS DATE));

        DECLARE @StartA DATE = CAST(@TuNgay AS DATE);
        DECLARE @EndA   DATE = CAST(@DenNgay AS DATE);
        DECLARE @EndAPlus1 DATE = DATEADD(DAY, 1, @EndA);
        DECLARE @TotalDaysA INT = DATEDIFF(DAY, @StartA, @EndA) + 1;
        IF @TotalDaysA<1 SET @TotalDaysA=1;
        IF @TotalDaysA>400 SET @TotalDaysA=400;

        IF OBJECT_ID('tempdb..#CalDays') IS NOT NULL DROP TABLE #CalDays;
        SELECT TOP (@TotalDaysA)
            DATEADD(DAY, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) - 1, @StartA) AS NgayHoatDong
        INTO #CalDays FROM master..spt_values WHERE type='P';

        SELECT CAST(NgayNhapKho AS DATE) AS Ngay, SUM(ISNULL(SoLuongThucTeBanDau,0)) AS TotalIn
        INTO #CalNhap FROM dbo.ERP_ChiTietNhapKhoNPL
        WHERE NgayNhapKho >= @StartA AND NgayNhapKho < @EndAPlus1
        GROUP BY CAST(NgayNhapKho AS DATE);

        SELECT CAST(NgayXuatHang AS DATE) AS Ngay, SUM(ISNULL(SLNhap,0)) AS TotalOut
        INTO #CalXuat FROM dbo.PhieuXuatHang
        WHERE ModuleXH=1 AND NgayXuatHang>=@StartA AND NgayXuatHang<@EndAPlus1
        GROUP BY CAST(NgayXuatHang AS DATE);

        IF OBJECT_ID('tempdb..#CalKK') IS NOT NULL DROP TABLE #CalKK;
        CREATE TABLE #CalKK (Ngay DATE PRIMARY KEY, TotalKiemKe DECIMAL(18,2));
        IF OBJECT_ID('dbo.ERPPhieuKiemKe_NPLV2','U') IS NOT NULL
        BEGIN
            INSERT INTO #CalKK(Ngay, TotalKiemKe)
            EXEC sp_executesql N'SELECT CAST(DateKiemKe AS DATE), SUM(ISNULL(SLKiemKe,0))
                FROM dbo.ERPPhieuKiemKe_NPLV2 WHERE DateKiemKe>=@S AND DateKiemKe<@E
                GROUP BY CAST(DateKiemKe AS DATE);', N'@S DATE, @E DATE', @S=@StartA, @E=@EndAPlus1;
        END

        SELECT CONVERT(VARCHAR(10), d.NgayHoatDong, 120) AS NgayHoatDong,
               ISNULL(n.TotalIn,0) AS TotalIn, ISNULL(x.TotalOut,0) AS TotalOut,
               ISNULL(k.TotalKiemKe,0) AS TotalKiemKe,
               ISNULL(n.TotalIn,0)+ISNULL(x.TotalOut,0)+ISNULL(k.TotalKiemKe,0) AS TotalActivity
        FROM #CalDays d
        LEFT JOIN #CalNhap n ON n.Ngay=d.NgayHoatDong
        LEFT JOIN #CalXuat x ON x.Ngay=d.NgayHoatDong
        LEFT JOIN #CalKK   k ON k.Ngay=d.NgayHoatDong
        ORDER BY d.NgayHoatDong;

        DROP TABLE #CalDays; DROP TABLE #CalNhap; DROP TABLE #CalXuat; DROP TABLE #CalKK;
        RETURN;
    END

    -- ===================================================================
    -- [4] GetMoMComparison
    -- ===================================================================
    IF @Action = 'GetMoMComparison'
    BEGIN
        DECLARE @MaxDate DATE;
        SELECT @MaxDate = MAX(d) FROM (
            SELECT MAX(CAST(NgayNhapKho AS DATE)) AS d FROM ERP_ChiTietNhapKhoNPL WHERE NgayNhapKho IS NOT NULL
            UNION ALL
            SELECT MAX(CAST(NgayXuatHang AS DATE)) AS d FROM PhieuXuatHang WHERE ModuleXH=1 AND NgayXuatHang IS NOT NULL
        ) t;
        IF @MaxDate IS NULL SET @MaxDate = CAST(GETDATE() AS DATE);

        DECLARE @ThisM DATE = DATEFROMPARTS(YEAR(@MaxDate), MONTH(@MaxDate), 1);
        DECLARE @LastM DATE = DATEADD(MONTH, -1, @ThisM);
        DECLARE @NextM DATE = DATEADD(MONTH,  1, @ThisM);

        SELECT 'ThisMonth' AS KieuKy, YEAR(@ThisM) AS Nam, MONTH(@ThisM) AS Thang,
            (SELECT ISNULL(SUM(ISNULL(SoLuongThucTeBanDau,0)),0) FROM ERP_ChiTietNhapKhoNPL WHERE NgayNhapKho>=@ThisM AND NgayNhapKho<@NextM) AS TotalIn,
            (SELECT ISNULL(SUM(ISNULL(SLNhap,0)),0) FROM PhieuXuatHang WHERE ModuleXH=1 AND NgayXuatHang>=@ThisM AND NgayXuatHang<@NextM) AS TotalOut
        UNION ALL
        SELECT 'LastMonth', YEAR(@LastM), MONTH(@LastM),
            (SELECT ISNULL(SUM(ISNULL(SoLuongThucTeBanDau,0)),0) FROM ERP_ChiTietNhapKhoNPL WHERE NgayNhapKho>=@LastM AND NgayNhapKho<@ThisM),
            (SELECT ISNULL(SUM(ISNULL(SLNhap,0)),0) FROM PhieuXuatHang WHERE ModuleXH=1 AND NgayXuatHang>=@LastM AND NgayXuatHang<@ThisM);
        RETURN;
    END

    -- ===================================================================
    -- [5] GetTop5
    -- ===================================================================
    IF @Action = 'GetTop5'
    BEGIN
        IF OBJECT_ID('tempdb..#tmpXCTH') IS NOT NULL DROP TABLE #tmpXCTH;
        SELECT BarCode INTO #tmpXCTH FROM PhieuXuatHang t1
        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);

        SELECT TOP (0) BarCode INTO #tmpSH FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
        BEGIN
            INSERT INTO #tmpSH(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode WHERE ISNULL(SLSoanHang_TK,0)-ISNULL(SLSoanHang_BC,0)=0;';
        END

        SELECT ct.MaNPL, o.Module, SUM(ISNULL(ct.SoLuongThucTeBanDau,0)) AS TonKho
        INTO #TopRaw
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode=ct.BarCode
        INNER JOIN dbo.ERP_ONPL o ON o.TenO=v.MaONPL
        WHERE v.MaONPL IS NOT NULL AND o.Module IN (1,2)
          AND (@LoaiNPL=0 OR o.Module=@LoaiNPL)
          AND NOT EXISTS (SELECT 1 FROM #tmpXCTH t WHERE t.BarCode=v.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #tmpSH t WHERE t.BarCode=v.Barcode)
        GROUP BY ct.MaNPL, o.Module
        HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau,0))>0;

        SELECT r.MaNPL, r.Module, r.TonKho,
            ISNULL(PARSENAME(REPLACE(r.MaNPL,'@','.'),3),'') AS MaVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL,'@','.'),2),'') AS MauVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL,'@','.'),1),'') AS KhoVaiID
        INTO #TopParsed FROM #TopRaw r;

        SELECT p.MaVTID AS MaVT, CAST(p.MaVTID AS NVARCHAR(200)) AS TenVT,
            CAST('' AS NVARCHAR(200)) AS Mau, CAST('' AS NVARCHAR(200)) AS KhoVai,
            CAST('' AS NVARCHAR(500)) AS ChiTiet, CAST('' AS NVARCHAR(50)) AS TenDVVT,
            p.TonKho, p.Module AS NPL, p.MauVTID, p.KhoVaiID
        INTO #TopFinal FROM #TopParsed p;

        IF OBJECT_ID('dbo.ERP_MauVTTV') IS NOT NULL
        BEGIN TRY EXEC sp_executesql N'UPDATE f SET Mau=ISNULL(m.MauVT,'''') FROM #TopFinal f LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID=f.MauVTID;'; END TRY BEGIN CATCH END CATCH

        IF OBJECT_ID('dbo.ERP_KhoVai') IS NOT NULL
        BEGIN TRY EXEC sp_executesql N'UPDATE f SET KhoVai=ISNULL(k.KhoVai,'''') FROM #TopFinal f LEFT JOIN dbo.ERP_KhoVai k ON k.KhoVaiID=f.KhoVaiID;'; END TRY BEGIN CATCH END CATCH

        IF OBJECT_ID('dbo.ERP_VatTuTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'UPDATE f SET MaVT=ISNULL(vt.MaVT,f.MaVT), TenVT=ISNULL(vt.ChiTiet,''''), ChiTiet=ISNULL(vt.ChiTiet,'''')
                    FROM #TopFinal f LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID=f.MaVT;';
            END TRY BEGIN CATCH END CATCH

            IF OBJECT_ID('dbo.ERP_DonViVT') IS NOT NULL
            BEGIN TRY
                EXEC sp_executesql N'UPDATE f SET TenDVVT=ISNULL(d.TenDVVT,'''') FROM #TopFinal f
                    LEFT JOIN dbo.ERP_KhoVai k ON k.KhoVaiID=f.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT d ON d.MaDVVT=k.MaDVVT;';
            END TRY BEGIN CATCH END CATCH
        END

        SELECT TOP 15 MaVT, TenVT, Mau, KhoVai, ChiTiet, TenDVVT, TonKho, NPL FROM #TopFinal
        ORDER BY CASE WHEN @IsNhieuNhat=1 THEN TonKho END DESC,
                 CASE WHEN @IsNhieuNhat=0 THEN TonKho END ASC;

        DROP TABLE #TopRaw; DROP TABLE #TopParsed; DROP TABLE #TopFinal;
        DROP TABLE #tmpXCTH; DROP TABLE #tmpSH;
        RETURN;
    END

    -- ===================================================================
    -- [6] GetDistinctMaterialCount
    -- ===================================================================
    IF @Action = 'GetDistinctMaterialCount'
    BEGIN
        SELECT BarCode INTO #TempXCTH2 FROM PhieuXuatHang t1
        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);

        SELECT TOP (0) BarCode INTO #TempSH2 FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
            INSERT INTO #TempSH2(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode WHERE ISNULL(SLSoanHang_TK,0)-ISNULL(SLSoanHang_BC,0)=0;';

        SELECT COUNT(DISTINCT ct.MaNPL) AS SoMaVatTu
        FROM dbo.ERP_VatTuCBM v
        INNER JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON ct.BarCode=v.Barcode
        WHERE v.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXCTH2 t WHERE t.BarCode=v.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #TempSH2  t WHERE t.BarCode=v.Barcode);
        DROP TABLE #TempXCTH2; DROP TABLE #TempSH2;
        RETURN;
    END

    -- ===================================================================
    -- Default: return error
    -- ===================================================================
    SELECT 'Unknown action: ' + ISNULL(@Action, 'NULL') AS Error;
END

CREATE OR ALTER PROCEDURE SP_LICH_PHAN_CONG_PHU_LIEU
    @Action NVARCHAR(50),
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL,
    @Ngay DATETIME = NULL,
    @NgayThucHien DATETIME = NULL,
    @MaLenhSX NVARCHAR(100) = NULL,
    @MaNV NVARCHAR(50) = NULL,
    @TenNV NVARCHAR(200) = NULL,
    @TrangThai INT = NULL,
    @GhiChu NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'GetCalendarMonth'
    BEGIN
        SELECT 
            CONVERT(VARCHAR(10), t.NgayThucHien, 120) AS NgayLam,
            t.MaLenhSX,
            ISNULL(p.MaKhachHang, '') AS MaKhachHang,
            ISNULL(p.TenBrand, '') AS TenBrand,
            0 AS CoCanhBao,
            0 AS ThieuNPL,
            ISNULL(t.MaNV, '') AS MaNV,
            ISNULL(t.TenNV, '') AS TenNV,
            ISNULL(t.TrangThai, 0) AS TrangThai,
            ISNULL(t.GhiChu, '') AS GhiChu
        FROM ERP_LichPhanCongPhuLieu_Task t
        LEFT JOIN ERP_ThongTinChungLenhSanXuat p ON t.MaLenhSX = p.MaLenhSX
        WHERE 
            (@TuNgay IS NULL OR t.NgayThucHien >= @TuNgay)
            AND (@DenNgay IS NULL OR t.NgayThucHien < DATEADD(day, 1, @DenNgay));
        RETURN;
    END

    IF @Action = 'GetDayDetail'
    BEGIN
        -- Result 1: Assignments
        SELECT 
            t.MaLenhSX,
            ISNULL(p.MaKhachHang, '') AS MaKhachHang,
            ISNULL(p.TenBrand, '') AS TenBrand,
            0 AS CoCanhBao,
            0 AS ThieuNPL,
            ISNULL(t.MaNV, '') AS MaNV,
            ISNULL(t.TenNV, '') AS TenNV,
            ISNULL(t.TrangThai, 0) AS TrangThai,
            ISNULL(t.GhiChu, '') AS GhiChu
        FROM ERP_LichPhanCongPhuLieu_Task t
        LEFT JOIN ERP_ThongTinChungLenhSanXuat p ON t.MaLenhSX = p.MaLenhSX
        WHERE CONVERT(DATE, t.NgayThucHien) = CONVERT(DATE, @Ngay);

        -- Result 2: PickOrders
        SELECT 
            t.MaLenhSX,
            ISNULL(p.MaKhachHang, '') AS MaKhachHang,
            ISNULL(p.TenBrand, '') AS TenBrand,
            ISNULL(t.TrangThai, 0) AS TrangThai,
            0 AS SoPLThieu,
            ISNULL(t.TenNV, '') AS TenNV,
            ISNULL(t.MaNV, '') AS MaNV,
            CONVERT(VARCHAR(10), t.NgayThucHien, 120) AS NgaySoan,
            '08:00' AS GioSoan,
            0 AS SoLoaiPL,
            0 AS TongSLCanSoan
        FROM ERP_LichPhanCongPhuLieu_Task t
        LEFT JOIN ERP_ThongTinChungLenhSanXuat p ON t.MaLenhSX = p.MaLenhSX
        WHERE CONVERT(DATE, t.NgayThucHien) = CONVERT(DATE, @Ngay);
        RETURN;
    END

    IF @Action = 'GetFilterLists'
    BEGIN
        SELECT 'NV001' AS MaNV, N'Nguyá»…n HoÃ ng Nam' AS TenNV, 'PB01' AS MaPhongBan, N'Kho Phá»¥ Liá»‡u' AS TenPhongBan, CAST(1 AS BIT) AS IsActive
        UNION ALL SELECT 'NV002', N'VÅ© Tháº¿ HÃ¹ng', 'PB01', N'Kho Phá»¥ Liá»‡u', CAST(1 AS BIT)
        UNION ALL SELECT 'NV003', N'Tráº§n VÄƒn A', 'PB01', N'Kho Phá»¥ Liá»‡u', CAST(1 AS BIT);
        RETURN;
    END

    IF @Action = 'GetListPhuLieu'
    BEGIN
        SELECT TOP 0 1 AS ID;
        RETURN;
    END

    IF @Action = 'GetCanDoiNPL'
    BEGIN
        SELECT TOP 0 1 AS ID;
        RETURN;
    END

    IF @Action = 'SavePhanCong'
    BEGIN
        IF EXISTS (SELECT 1 FROM ERP_LichPhanCongPhuLieu_Task WHERE MaLenhSX = @MaLenhSX)
            UPDATE ERP_LichPhanCongPhuLieu_Task
            SET MaNV=@MaNV, TenNV=@TenNV, NgayThucHien=@NgayThucHien, GhiChu=@GhiChu, UpdateAt=GETDATE()
            WHERE MaLenhSX = @MaLenhSX;
        ELSE
            INSERT INTO ERP_LichPhanCongPhuLieu_Task (MaLenhSX,MaNV,TenNV,TrangThai,NgayThucHien,GhiChu)
            VALUES (@MaLenhSX,@MaNV,@TenNV,0,@NgayThucHien,@GhiChu);
        SELECT 1 AS Success, N'ÄÃ£ phÃ¢n cÃ´ng thÃ nh cÃ´ng!' AS [Error];
        RETURN;
    END

    IF @Action = 'UpdateTrangThai'
    BEGIN
        UPDATE ERP_LichPhanCongPhuLieu_Task
        SET TrangThai=@TrangThai, UpdateAt=GETDATE()
        WHERE MaLenhSX = @MaLenhSX;
        SELECT 1 AS Success, N'Cáº­p nháº­t tráº¡ng thÃ¡i thÃ nh cÃ´ng!' AS [Error];
        RETURN;
    END

    IF @Action = 'GetCalendarInventory'
    BEGIN
        DECLARE @Start DATE = CAST(@TuNgay AS DATE);
        DECLARE @End DATE = DATEADD(DAY, 1, CAST(@DenNgay AS DATE));

        IF OBJECT_ID('dbo.ERP_ChiTietNhapKhoNPL', 'U') IS NOT NULL 
           AND OBJECT_ID('dbo.PhieuXuatHang', 'U') IS NOT NULL 
           AND OBJECT_ID('dbo.ERPPhieuKiemKe_NPL', 'U') IS NOT NULL
        BEGIN
            ;WITH CalendarCTE AS (
                SELECT @Start AS Ngay
                UNION ALL
                SELECT DATEADD(DAY, 1, Ngay)
                FROM   CalendarCTE
                WHERE  DATEADD(DAY, 1, Ngay) <= CAST(@DenNgay AS DATE)
            ),
            NhapKhoCTE AS (
                SELECT
                    CAST(ct.NgayNhapKho AS DATE)       AS Ngay,
                    SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuongNhapKho
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.NgayNhapKho >= @Start 
                  AND ct.NgayNhapKho < @End
                  AND ct.IsDuyetNK = 1
                GROUP BY CAST(ct.NgayNhapKho AS DATE)
            ),
            XuatHangCTE AS (
                SELECT
                    CAST(NgayXuatHang AS DATE)      AS Ngay,
                    SUM(ISNULL(SLNhap, 0))          AS SoLuongXuatHang
                FROM dbo.PhieuXuatHang
                WHERE NgayXuatHang >= @Start 
                  AND NgayXuatHang < @End
                GROUP BY CAST(NgayXuatHang AS DATE)
            ),
            KiemKeCTE AS (
                SELECT
                    CAST(DateKiemKe AS DATE)        AS Ngay,
                    COUNT(MaNPL)                    AS SoLuongKiemKe
                FROM dbo.ERPPhieuKiemKe_NPL
                WHERE DateKiemKe >= @Start 
                  AND DateKiemKe < @End
                GROUP BY CAST(DateKiemKe AS DATE)
            )
            SELECT
                c.Ngay,
                ISNULL(n.SoLuongNhapKho,  0) AS SoLuongNhapKho,
                ISNULL(x.SoLuongXuatHang, 0) AS SoLuongXuatHang,
                ISNULL(k.SoLuongKiemKe,   0) AS SoLuongKiemKe
            FROM CalendarCTE c
            LEFT JOIN NhapKhoCTE  n ON c.Ngay = n.Ngay
            LEFT JOIN XuatHangCTE x ON c.Ngay = x.Ngay
            LEFT JOIN KiemKeCTE   k ON c.Ngay = k.Ngay
            OPTION (MAXRECURSION 0);
        END
        ELSE
        BEGIN
            ;WITH CalendarCTE AS (
                SELECT @Start AS Ngay
                UNION ALL
                SELECT DATEADD(DAY, 1, Ngay)
                FROM   CalendarCTE
                WHERE  DATEADD(DAY, 1, Ngay) <= CAST(@DenNgay AS DATE)
            )
            SELECT Ngay, 0 AS SoLuongNhapKho, 0 AS SoLuongXuatHang, 0 AS SoLuongKiemKe
            FROM CalendarCTE
            OPTION (MAXRECURSION 0);
        END
        RETURN;
    END
    
    SELECT 'Unknown action: ' + ISNULL(@Action,'NULL') AS [Error];
END


ALTER PROCEDURE dbo.usp_DashboardKhoDesktop
    @Action VARCHAR(100),
    @TuNgay DATETIME = NULL,
    @DenNgay DATETIME = NULL,
    @Itemcode NVARCHAR(200) = NULL,
    @IsNhieuNhat INT = NULL,
    @LoaiNPL INT = NULL,
    @Ngay DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Declarations for GetOverallCapacity
    DECLARE @CapNPL FLOAT = 0, 
            @CapPL FLOAT = 0, 
            @UsedNPL FLOAT = 0, 
            @UsedPL FLOAT = 0,
            @TotalVatTuNPL FLOAT = 0, 
            @TotalVatTuPL FLOAT = 0;

    -- Declarations for GetCustomers
    DECLARE @TotalSKUInWarehouse FLOAT;

    -- Declarations for GlobalSearchByItemcode
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

    -- Declarations for GetFlowTrend12T
    DECLARE @StartDate12T DATE;

    -- Declarations for GetFlowTrendWeekly
    DECLARE @N_Weekly INT = 12,
            @Today_Weekly DATE,
            @StartDateWeekly DATE;

    -- Declarations for GetThanhGiaHangTon
    DECLARE @ThanhGia DECIMAL(20,2) = 0,
            @TongMa INT = 0, 
            @SoMaCoGia INT = 0, 
            @SoMaKhongGia INT = 0,
            @TongSL DECIMAL(20,2) = 0;

    -- Declarations for GetKiemKeDetailByRange
    DECLARE @HasV2R INT = 0;

    -- Declarations for GetKiemKeDetailByDay
    DECLARE @HasV2D INT = 0;

    -- Declarations for GetActivityCalendar
    DECLARE @StartDateCal DATE,
            @EndDateCal DATE,
            @TotalDaysCal INT,
            @EndPlus1Cal DATE;

    -- Declarations for GetFlowTrendByRange
    DECLARE @StartDateRange DATE,
            @EndDateRange DATE,
            @TotalDaysRange INT;

    -- Declarations for GetMoMComparison
    DECLARE @MaxDate DATE,
            @ThisMonth DATE,
            @LastMonth DATE,
            @NextMonth DATE;

    -- =========================================================================
    -- 1. GetOverallCapacity
    -- =========================================================================
    IF @Action = 'GetOverallCapacity'
    BEGIN
        SELECT t1.KeID, ROUND(SUM(t2.Dai * t2.Cao * t2.Rong), 4) AS TongCBMTrongKe, t1.TenKe, t1.Module
        INTO #tempCBMKe_OC
        FROM ERP_KeNPL t1
        LEFT JOIN ERP_ONPL t2 ON t1.KeID = t2.KeID
        WHERE t1.Module <> 3
        GROUP BY t1.KeID, t1.TenKe, t1.Module;

        SELECT BarCode INTO #TempXuatChuaThuHoi_OC
        FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        SELECT TOP (0) BarCode INTO #tempBarcodeSH_OC FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tempBarcodeSH_OC(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';
        END

        SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM),4) AS TongCBMTrongO, COUNT(*) AS SLVatTu
        INTO #tempCBMO_OC
        FROM ERP_VatTuCBM t1
        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
        WHERE t1.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXuatChuaThuHoi_OC x WHERE x.BarCode = t1.Barcode)
          AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL c WHERE c.BarCode = t1.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #tempBarcodeSH_OC s WHERE s.BarCode = t1.Barcode)
        GROUP BY t1.MaONPL, t2.KeID;

        SELECT t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module,
            ISNULL(SUM(t2.TongCBMTrongO),0) AS TongCBMSuDungTrongKe,
            ISNULL(t3.TongCBMTrongKe,0) AS TongCBMTrongKe,
            SUM(ISNULL(t2.SLVatTu,0)) AS SLVatTu
        INTO #tempBaseResult_OC
        FROM ERP_KeNPL t1
        LEFT JOIN #tempCBMO_OC t2 ON t1.KeID = t2.KeID
        LEFT JOIN #tempCBMKe_OC t3 ON t1.KeID = t3.KeID
        LEFT JOIN ERP_DayNPL t4 ON t1.DayID = t4.DayID
        WHERE t1.Module <> 3
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%l?i%'
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%n%'
        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe;

        SELECT
            @CapNPL        = ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMTrongKe ELSE 0 END),0),
            @CapPL         = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMTrongKe ELSE 0 END),0),
            @UsedNPL       = ISNULL(SUM(CASE WHEN Module=1 THEN TongCBMSuDungTrongKe ELSE 0 END),0),
            @UsedPL        = ISNULL(SUM(CASE WHEN Module=2 THEN TongCBMSuDungTrongKe ELSE 0 END),0),
            @TotalVatTuNPL = ISNULL(SUM(CASE WHEN Module=1 THEN SLVatTu ELSE 0 END),0),
            @TotalVatTuPL  = ISNULL(SUM(CASE WHEN Module=2 THEN SLVatTu ELSE 0 END),0)
        FROM #tempBaseResult_OC;

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

        DROP TABLE #tempCBMO_OC; DROP TABLE #tempCBMKe_OC;
        DROP TABLE #TempXuatChuaThuHoi_OC; DROP TABLE #tempBarcodeSH_OC; DROP TABLE #tempBaseResult_OC;
    END

    -- =========================================================================
    -- 2. GetDistinctMaterialCount
    -- =========================================================================
    ELSE IF @Action = 'GetDistinctMaterialCount'
    BEGIN
        IF COL_LENGTH('dbo.ERP_VatTuCBM','MaONPL') IS NULL
        BEGIN
            SELECT CAST(0 AS INT) AS SoMaVatTu;
            RETURN;
        END

        SELECT BarCode
        INTO #TempXCTH_DMC
        FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        SELECT TOP (0) BarCode INTO #TempSH_DMC FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #TempSH_DMC(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';
        END

        SELECT COUNT(DISTINCT ct.MaNPL) AS SoMaVatTu
        FROM dbo.ERP_VatTuCBM v
        INNER JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON ct.BarCode = v.Barcode
        WHERE v.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXCTH_DMC t WHERE t.BarCode = v.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #TempSH_DMC  t WHERE t.BarCode = v.Barcode);

        DROP TABLE #TempXCTH_DMC;
        DROP TABLE #TempSH_DMC;
    END

    -- =========================================================================
    -- 3. GetCustomers
    -- =========================================================================
    ELSE IF @Action = 'GetCustomers'
    BEGIN
        SELECT BarCode INTO #TempXCTH_Cust FROM PhieuXuatHang t1
        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);

        SELECT BarCode INTO #tempSH_Cust FROM ERP_SoanHangNPL_BarCode
        WHERE SLSoanHang_TK - SLSoanHang_BC = 0;

        SELECT MaNPL, SoLoID, SUM(CBM) AS CBM
        INTO #tempBarcodeCBM_Cust
        FROM ERP_VatTuCBM t1
        LEFT JOIN ERP_ONPL t2 ON t1.MaONPL = t2.TenO
        LEFT JOIN ERP_DayNPL t3 ON t2.DayID = t3.DayID
        WHERE t1.MaONPL IS NOT NULL
          AND t2.Module <> 3
          AND LOWER(t3.TenDay) NOT LIKE '%co%'
          AND LOWER(t3.TenDay) NOT LIKE N'%l?i%'
          AND LOWER(t3.TenDay) NOT LIKE N'%n%'
          AND NOT EXISTS (SELECT 1 FROM #TempXCTH_Cust x WHERE x.BarCode = t1.Barcode)
          AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL t6 WHERE t1.Barcode = t6.BarCode)
          AND NOT EXISTS (SELECT 1 FROM #tempSH_Cust t7 WHERE t1.Barcode = t7.BarCode)
        GROUP BY MaNPL, SoLoID;

        SELECT @TotalSKUInWarehouse = CAST(COUNT(DISTINCT CASE WHEN ISNULL(MaNPL,'') <> '' THEN CONCAT(MaNPL,'|',SoLoID) END) AS FLOAT)
        FROM #tempBarcodeCBM_Cust;

        SELECT
            ISNULL(t3.MaKH, '') AS MaKH,
            ISNULL(t5.TenKH, '') AS TenKH,
            COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) AS SLVatTu,
            SUM(CBM) AS CBMSDTrongKho,
            CASE WHEN @TotalSKUInWarehouse > 0
                THEN ROUND(COUNT(DISTINCT CASE WHEN ISNULL(t1.MaNPL,'') <> '' THEN CONCAT(t1.MaNPL,'|',t1.SoLoID) END) / @TotalSKUInWarehouse * 100, 2)
                ELSE 0
            END AS PhanTramSKU
        FROM #tempBarcodeCBM_Cust t1
        LEFT JOIN (SELECT DISTINCT MaNPL, SoLoID FROM ERP_ChiTietNhapKhoNPL) t2
            ON t1.MaNPL = t2.MaNPL AND t1.SoLoID = t2.SoLoID
        LEFT JOIN ERP_NhapKhoNPL t3 ON t2.SoLoID = t3.SoLoID
        LEFT JOIN KhachHang t5 ON t3.MaKH = t5.MaKH
        GROUP BY ISNULL(t3.MaKH, ''), ISNULL(t5.TenKH, '')
        ORDER BY SLVatTu DESC;

        DROP TABLE #tempBarcodeCBM_Cust; DROP TABLE #tempSH_Cust; DROP TABLE #TempXCTH_Cust;
    END

    -- =========================================================================
    -- 4. GetRacks
    -- =========================================================================
    ELSE IF @Action = 'GetRacks'
    BEGIN
        SELECT t1.KeID, ROUND(SUM(t2.Dai*t2.Cao*t2.Rong),4) AS TongCBMTrongKe, t1.TenKe, t1.Module
        INTO #tempCBMKe_Racks FROM ERP_KeNPL t1 LEFT JOIN ERP_ONPL t2 ON t1.KeID=t2.KeID
        WHERE t1.Module<>3 GROUP BY t1.KeID, t1.TenKe, t1.Module;

        SELECT BarCode INTO #TempXCTH_Racks FROM PhieuXuatHang t1
        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);

        SELECT TOP (0) BarCode INTO #tempSH_Racks FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tempSH_Racks(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK,0)-ISNULL(SLSoanHang_BC,0)=0;';
        END

        SELECT DISTINCT t1.MaONPL, t2.KeID, ROUND(SUM(t1.CBM),4) AS TongCBMTrongO, COUNT(*) AS SLVatTu
        INTO #tempCBMO_Racks
        FROM ERP_VatTuCBM t1 LEFT JOIN ERP_ONPL t2 ON t1.MaONPL=t2.TenO
        WHERE t1.MaONPL IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM #TempXCTH_Racks x WHERE x.BarCode=t1.Barcode)
          AND EXISTS     (SELECT 1 FROM ERP_ChiTietNhapKhoNPL c WHERE c.BarCode=t1.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #tempSH_Racks s WHERE s.BarCode=t1.Barcode)
        GROUP BY t1.MaONPL, t2.KeID;

        SELECT t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module,
            ISNULL(SUM(t2.TongCBMTrongO),0) AS TongCBMSuDungTrongKe,
            ISNULL(t3.TongCBMTrongKe,0) AS TongCBMTrongKe,
            SUM(ISNULL(t2.SLVatTu,0)) AS SLVatTu
        FROM ERP_KeNPL t1
        LEFT JOIN #tempCBMO_Racks t2 ON t1.KeID=t2.KeID
        LEFT JOIN #tempCBMKe_Racks t3 ON t1.KeID=t3.KeID
        LEFT JOIN ERP_DayNPL t4 ON t1.DayID=t4.DayID
        WHERE t1.Module<>3
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%l?i%'
          AND LOWER(ISNULL(t4.TenDay,N'')) NOT LIKE N'%n%'
        GROUP BY t1.DayID, t4.TenDay, t1.KeID, t1.TenKe, t1.Module, t3.TongCBMTrongKe
        ORDER BY t1.Module, t1.TenKe;

        DROP TABLE #tempCBMO_Racks; DROP TABLE #tempCBMKe_Racks; DROP TABLE #TempXCTH_Racks; DROP TABLE #tempSH_Racks;
    END

    -- =========================================================================
    -- 5. GetChuanBiVe
    -- =========================================================================
    ELSE IF @Action = 'GetChuanBiVe'
    BEGIN
        ;WITH ChiTiet AS
        (
            SELECT
                ct.SoLoID,
                ct.POMua,
                ct.MaNPL,
                MAX(ISNULL(ct.SLTong, 0)) AS SLTong
            FROM dbo.ERP_ChiTietNhapKhoNPL ct
            GROUP BY ct.SoLoID, ct.POMua, ct.MaNPL
        ),
        ChiTietTheoSoLo AS
        (
            SELECT
                ct.SoLoID,
                ct.POMua,
                SUM(ISNULL(ct.SLTong, 0)) AS SLTong
            FROM ChiTiet ct
            GROUP BY ct.SoLoID, ct.POMua
        ),
        NgayDuKienTrongKhoang AS
        (
            SELECT DISTINCT
                CAST(nk.NgayNKDuKien AS date) AS NgayNKDuKien
            FROM dbo.ERP_NhapKhoNPL nk
            WHERE nk.NgayNKDuKien IS NOT NULL
              AND (
                  (ISNULL(@Itemcode, '') = '' AND CAST(nk.NgayNKDuKien AS date) BETWEEN CAST(@TuNgay AS DATE) AND CAST(@DenNgay AS DATE))
                  OR (ISNULL(@Itemcode, '') <> '' AND nk.POMua = @Itemcode)
                  OR (ISNULL(@Itemcode, '') <> '' AND nk.SoLoID = @Itemcode)
              )
        ),
        NhapKhoRaw AS
        (
            SELECT DISTINCT
                nk.SoLoID,
                nk.SoLo,
                nk.POMua,
                ISNULL(nk.MaKH, '') AS MaKH,
                ISNULL((SELECT TOP 1 dh.MaDH FROM dbo.DonHangTong dh WHERE dh.MaHang = nk.MaHang AND dh.MaKH = nk.MaKH), '') AS MaDH,
                ISNULL(nk.MaHang, '') AS MaHang,
                ISNULL((SELECT TOP 1 hh.TenHang FROM dbo.HangHoa hh WHERE hh.MaHang = nk.MaHang AND hh.MaKH = nk.MaKH), '') AS TenHang,
                CAST(nk.NgayNKDuKien AS date) AS NgayNKDuKien
            FROM dbo.ERP_NhapKhoNPL nk
            INNER JOIN NgayDuKienTrongKhoang m 
              ON CAST(nk.NgayNKDuKien AS date) = m.NgayNKDuKien
            WHERE (ISNULL(@Itemcode, '') = '' OR nk.POMua = @Itemcode OR nk.SoLoID = @Itemcode)
        ),
        NhapKho AS
        (
            SELECT
                nk.SoLoID,
                nk.POMua,
                MAX(nk.SoLo) AS SoLo,
                MAX(nk.MaKH) AS MaKH,
                MAX(nk.MaDH) AS MaDH,
                MAX(nk.MaHang) AS MaHang,
                MAX(nk.TenHang) AS TenHang,
                MIN(nk.NgayNKDuKien) AS NgayNKDuKien
            FROM NhapKhoRaw nk
            GROUP BY nk.SoLoID, nk.POMua
        )
        SELECT
            MIN(nk.SoLoID) AS SoLoID,
            MAX(nk.SoLo) AS SoLo,
            nk.POMua AS PO,
            nk.POMua,
            MAX(nk.MaDH) AS MaDH,
            MAX(nk.MaHang) AS MaHang,
            MAX(nk.TenHang) AS TenHang,
            MIN(nk.NgayNKDuKien) AS NgayNKDuKien,
            MIN(nk.NgayNKDuKien) AS NgayNhapKho_Update,
            CAST(N'' AS NVARCHAR(200)) AS MaNPL,
            SUM(ISNULL(ct.SLTong, 0)) AS SoLuongSP,
            CAST(0 AS decimal(18, 2)) AS SoLuongThung,
            MAX(ISNULL(kh.TenKH, '')) AS TenKH
        FROM NhapKho nk
        LEFT JOIN ChiTietTheoSoLo ct ON ct.SoLoID = nk.SoLoID AND ct.POMua = nk.POMua
        LEFT JOIN KhachHang kh ON kh.MaKH = nk.MaKH
        GROUP BY nk.POMua
        ORDER BY MIN(nk.NgayNKDuKien), nk.POMua;
    END

    -- =========================================================================
    -- 6. GetChuanBiXuat
    -- =========================================================================
    ELSE IF @Action = 'GetChuanBiXuat'
    BEGIN
        SELECT
            cs.MaLenhSanXuat,
            cs.MaLenh,
            cs.MaDVSX,
            ISNULL(MAX(kh.TenKH), '')           AS KhachHang,
            ISNULL(MAX(hh.TenHang), '')         AS TenHang,
            SUM(ISNULL(cs.SoLuong, 0))          AS SoLuongYeuCau,
            MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END)                       AS KHCat,
            DATEADD(day, 7, MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END))      AS DuKienCat
        FROM dbo.CanDoiDonViSanXuat cs
        LEFT JOIN dbo.DonHangTong dh ON dh.MaDH = cs.MaDH
        LEFT JOIN dbo.HangHoa     hh ON hh.MaHang = dh.MaHang AND hh.MaKH = dh.MaKH
        LEFT JOIN dbo.KhachHang   kh ON kh.MaKH   = dh.MaKH
        LEFT JOIN dbo.WIP_DonHang_Chuyen wc
            ON wc.LenhSX = TRY_CONVERT(INT, REPLACE(ISNULL(cs.MaLenh, ''), 'SX_', ''))
        LEFT JOIN dbo.WIP_DonHang_Chuyen_TechProgress tp ON tp.WIPId = wc.WIPId
        WHERE NOT EXISTS (SELECT 1 FROM dbo.PhieuXuatHang ph WHERE ph.MaLenhSX = cs.MaLenhSanXuat)
        GROUP BY cs.MaLenhSanXuat, cs.MaLenh, cs.MaDVSX
        ORDER BY KHCat DESC;
    END

    -- =========================================================================
    -- 7. GlobalSearchByItemcode
    -- =========================================================================
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

    -- =========================================================================
    -- 8. GetDangXuat
    -- =========================================================================
    ELSE IF @Action = 'GetDangXuat'
    BEGIN
        IF OBJECT_ID('tempdb..#YeuCau_DX') IS NOT NULL DROP TABLE #YeuCau_DX;
        SELECT
            dk.MaLenhSX,
            dk.PhieuDK,
            SUM(ISNULL(dk.SLDK, 0))      AS SLYeuCau
        INTO #YeuCau_DX
        FROM dbo.ERP_PhieuDangKyXuatVT dk
        WHERE dk.IsNPL = 1
          AND ISNULL(dk.SLDK, 0) > 0
        GROUP BY dk.MaLenhSX, dk.PhieuDK;

        IF OBJECT_ID('tempdb..#DaXuat_DX') IS NOT NULL DROP TABLE #DaXuat_DX;
        SELECT
            ph.PhieuYC,
            ph.MaLenhSX,
            MAX(ph.MaLenh)              AS MaLenh,
            MAX(ph.MaGop)               AS MaGop,
            MAX(ph.TenHang)             AS TenHang,
            MAX(ph.TenKH)               AS TenKH,
            MAX(ph.NgayXuatHang)        AS NgayXuatHang,
            SUM(ISNULL(ph.SLNhap, 0))   AS SLDaXuat
        INTO #DaXuat_DX
        FROM dbo.PhieuXuatHang ph
        WHERE ph.Moudule = 0 AND ph.NPL = 1
          AND ph.NgayXuatHang IS NOT NULL
          AND ph.NgayXuatHang >= CAST(@TuNgay AS DATE) AND ph.NgayXuatHang <= CAST(@DenNgay AS DATE)
        GROUP BY ph.PhieuYC, ph.MaLenhSX;

        IF OBJECT_ID('tempdb..#KhTH_DX') IS NOT NULL DROP TABLE #KhTH_DX;
        SELECT DISTINCT gd.MaGop,
               ISNULL(hh.TenHang, '') AS TenHang,
               ISNULL(kh.TenKH, '')   AS TenKH
        INTO #KhTH_DX
        FROM dbo.GopDonHang gd
        INNER JOIN dbo.DonHangTong dh ON gd.MaDH = dh.MaDH
        LEFT JOIN dbo.HangHoa hh ON hh.MaHang = dh.MaHang AND hh.MaKH = dh.MaKH
        LEFT JOIN dbo.KhachHang kh ON kh.MaKH = dh.MaKH;

        SELECT
            x.MaLenh                                                    AS MaLenh,
            x.MaLenhSX                                                  AS MaLenhSX,
            x.PhieuYC                                                   AS PhieuDK,
            x.MaGop,
            COALESCE(NULLIF(x.TenHang, ''), NULLIF(k.TenHang, ''), '')  AS TenHang,
            COALESCE(NULLIF(x.TenKH, ''), NULLIF(k.TenKH, ''), '')      AS TenKH,
            x.NgayXuatHang,
            ROUND(ISNULL(y.SLYeuCau, 0), 4)                             AS SoLuongYeuCau,
            ROUND(x.SLDaXuat, 4)                                        AS SLXuat,
            CASE WHEN ISNULL(y.SLYeuCau, 0) > 0
                 THEN ROUND(x.SLDaXuat / y.SLYeuCau * 100, 2)
                 ELSE 0 END                                             AS PctDaXuat
        FROM #DaXuat_DX x
        LEFT JOIN #YeuCau_DX y ON y.PhieuDK = x.PhieuYC
        LEFT JOIN #KhTH_DX   k ON k.MaGop   = x.MaGop
        ORDER BY x.NgayXuatHang DESC;

        DROP TABLE #YeuCau_DX;
        DROP TABLE #DaXuat_DX;
        DROP TABLE #KhTH_DX;
    END

    -- =========================================================================
    -- 9. GetFlowTrend12T
    -- =========================================================================
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
        Combined AS (
            SELECT m.Nam, m.Thang,
                ISNULL(n.TotalIn, 0) AS TotalIn, ISNULL(x.TotalOut, 0) AS TotalOut, ISNULL(th.TotalTH, 0) AS TotalTH
            FROM Months m
            LEFT JOIN NhapTheoThang n ON n.Nam = m.Nam AND n.Thang = m.Thang
            LEFT JOIN XuatTheoThang x ON x.Nam = m.Nam AND x.Thang = m.Thang
            LEFT JOIN ThuHoiTheoThang th ON th.Nam = m.Nam AND th.Thang = m.Thang
        )
        SELECT c.Nam, c.Thang, c.TotalIn, c.TotalOut,
            ISNULL(d.TonDau, 0) + SUM(c.TotalIn + c.TotalTH - c.TotalOut) OVER (ORDER BY c.Nam, c.Thang ROWS UNBOUNDED PRECEDING) AS TotalStock
        FROM Combined c CROSS JOIN TonDauKy d
        ORDER BY c.Nam, c.Thang;
    END

    -- =========================================================================
    -- 10. GetFlowTrendWeekly
    -- =========================================================================
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
        Combined AS (
            SELECT w.Nam, w.Tuan,
                ISNULL(n.TotalIn, 0) AS TotalIn,
                ISNULL(x.TotalOut, 0) AS TotalOut,
                ISNULL(th.TotalTH, 0) AS TotalTH
            FROM Weeks w
            LEFT JOIN NhapTheoTuan n ON n.Nam = w.Nam AND n.Tuan = w.Tuan
            LEFT JOIN XuatTheoTuan x ON x.Nam = w.Nam AND x.Tuan = w.Tuan
            LEFT JOIN ThuHoiTheoTuan th ON th.Nam = w.Nam AND th.Tuan = w.Tuan
        )
        SELECT c.Nam, c.Tuan, c.TotalIn, c.TotalOut,
            (SELECT TonDau FROM TonDauKy) +
            SUM(c.TotalIn + c.TotalTH - c.TotalOut) OVER (ORDER BY c.Nam, c.Tuan ROWS UNBOUNDED PRECEDING) AS TotalStock
        FROM Combined c
        ORDER BY c.Nam, c.Tuan;
    END

    -- =========================================================================
    -- 11. GetAllMaterialsInStock
    -- =========================================================================
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

        SELECT BarCode INTO #tmpXCTH_AMIS FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        SELECT TOP (0) BarCode INTO #tmpSH_AMIS FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tmpSH_AMIS(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';
        END

        IF OBJECT_ID('tempdb..#MatRaw_AMIS') IS NOT NULL DROP TABLE #MatRaw_AMIS;
        SELECT
            ct.MaNPL,
            o.Module,
            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho,
            COUNT(*)                              AS SoBarCode
        INTO #MatRaw_AMIS
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode
        INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL
        WHERE v.MaONPL IS NOT NULL
          AND o.Module IN (1, 2)
          AND NOT EXISTS (SELECT 1 FROM #tmpXCTH_AMIS t WHERE t.BarCode = v.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #tmpSH_AMIS   t WHERE t.BarCode = v.Barcode)
        GROUP BY ct.MaNPL, o.Module
        HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) > 0;

        IF OBJECT_ID('tempdb..#MatParsed_AMIS') IS NOT NULL DROP TABLE #MatParsed_AMIS;
        SELECT
            r.MaNPL,
            r.Module,
            r.TonKho,
            r.SoBarCode,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID
        INTO #MatParsed_AMIS
        FROM #MatRaw_AMIS r;

        IF OBJECT_ID('tempdb..#MatFinal_AMIS') IS NOT NULL DROP TABLE #MatFinal_AMIS;
        SELECT
            p.MaVTID                          AS MaVT,
            CAST(p.MaVTID AS NVARCHAR(500))   AS TenVT,
            CAST('' AS NVARCHAR(200))         AS Mau,
            CAST('' AS NVARCHAR(200))         AS MaMauVT,
            CAST('' AS NVARCHAR(200))         AS KhoVai,
            CAST('' AS NVARCHAR(500))         AS ChiTiet,
            CAST('' AS NVARCHAR(50))          AS TenDVVT,
            p.TonKho,
            p.SoBarCode,
            CASE WHEN p.Module = 1 THEN N'NL' WHEN p.Module = 2 THEN N'PL' ELSE N'' END AS LoaiKho,
            p.MauVTID,
            p.KhoVaiID
        INTO #MatFinal_AMIS
        FROM #MatParsed_AMIS p;

        IF OBJECT_ID('dbo.ERP_MauVTTV') IS NOT NULL
        BEGIN
            BEGIN TRY
                EXEC sp_executesql N'
                    UPDATE f SET Mau = ISNULL(m.MauVT, ''''), MaMauVT = ISNULL(m.MaMauVT, '''')
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

        SELECT MaVT, TenVT, Mau, MaMauVT, KhoVai, ChiTiet, TenDVVT, TonKho, SoBarCode, LoaiKho
        FROM #MatFinal_AMIS
        ORDER BY TonKho DESC;

        DROP TABLE #MatRaw_AMIS;
        DROP TABLE #MatParsed_AMIS;
        DROP TABLE #MatFinal_AMIS;
        DROP TABLE #tmpXCTH_AMIS;
        DROP TABLE #tmpSH_AMIS;
    END

    -- =========================================================================
    -- 12. GetThanhGiaHangTon
    -- =========================================================================
    ELSE IF @Action = 'GetThanhGiaHangTon'
    BEGIN
        SELECT BarCode INTO #tmpXTH_TGHT FROM PhieuXuatHang t1
        WHERE NOT EXISTS (SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh=t1.MaLenhSX AND (t2.BarCode=t1.BarCode OR t2.BarCode=t1.BarCodeGoc) AND t1.Dot=t2.Dot);

        SELECT TOP (0) BarCode INTO #tmpSH_TGHT FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
            INSERT INTO #tmpSH_TGHT(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode WHERE ISNULL(SLSoanHang_TK,0)-ISNULL(SLSoanHang_BC,0)=0;';

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
                  AND NOT EXISTS (SELECT 1 FROM #tmpXTH_TGHT t WHERE t.BarCode = v.Barcode)
                  AND NOT EXISTS (SELECT 1 FROM #tmpSH_TGHT  t WHERE t.BarCode = v.Barcode)
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

        DROP TABLE #tmpXTH_TGHT;
        DROP TABLE #tmpSH_TGHT;
    END

    -- =========================================================================
    -- 13. GetNKDuKienByRange
    -- =========================================================================
    ELSE IF @Action = 'GetNKDuKienByRange'
    BEGIN
        SELECT
            CONVERT(VARCHAR(10), CAST(nk.NgayNKDuKien AS DATE), 120) AS NgayNKDuKien,
            ISNULL(nk.SoLo, '')                                      AS SoLo,
            ISNULL(nk.POMua, '')                                     AS PO,
            COALESCE(NULLIF(nk.MaKH, ''), NULLIF(nk.NhaCungCap, ''), '') AS MaKH,
            COALESCE(NULLIF(kh.TenKH, ''), NULLIF(ncc.TenKH, ''), '')    AS TenKH,
            ISNULL((SELECT SUM(ISNULL(ct.SLTong, 0))
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    WHERE ct.SoLoID = nk.SoLoID
                      AND ct.POMua  = nk.POMua), 0)                  AS SoLuongDuKien
        FROM dbo.ERP_NhapKhoNPL nk
        LEFT JOIN dbo.KhachHang kh ON kh.MaKH = nk.MaKH
        LEFT JOIN dbo.ERP_KhachHangNK ncc ON ncc.MaNhaCC = nk.NhaCungCap
        WHERE nk.NgayNKDuKien IS NOT NULL
          AND CAST(nk.NgayNKDuKien AS DATE) BETWEEN CAST(@TuNgay AS DATE) AND CAST(@DenNgay AS DATE)
        ORDER BY nk.NgayNKDuKien, nk.SoLo;
    END

    -- =========================================================================
    -- 14. GetNhapDetailByRange
    -- =========================================================================
    ELSE IF @Action = 'GetNhapDetailByRange'
    BEGIN
        IF OBJECT_ID('tempdb..#NhapAggR_NDR') IS NOT NULL DROP TABLE #NhapAggR_NDR;
        SELECT
            ct.SoLoID,
            ct.MaNPL,
            MAX(ct.POMua)                          AS PO,
            MAX(ct.MaDVVT)                         AS MaDVVT,
            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuong,
            COUNT(*)                               AS SoBarCode,
            MIN(ct.NgayNhapKho)                    AS NgayNhapTu,
            MAX(ct.NgayNhapKho)                    AS NgayNhapDen
        INTO #NhapAggR_NDR
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        WHERE ct.NgayNhapKho IS NOT NULL
          AND ct.NgayNhapKho >= CAST(@TuNgay AS DATE)
          AND ct.NgayNhapKho <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
        GROUP BY ct.SoLoID, ct.MaNPL;

        IF OBJECT_ID('tempdb..#NhapFinalR_NDR') IS NOT NULL DROP TABLE #NhapFinalR_NDR;
        SELECT TOP 2000
            ISNULL(nk.SoLo, '')                                                    AS PINCC,
            a.PO                                                                   AS PO,
            a.MaNPL                                                                AS MaNPL,
            CAST('' AS NVARCHAR(200))                                              AS ItemCode,
            CAST('' AS NVARCHAR(200))                                              AS MaMauVT,
            CAST('' AS NVARCHAR(200))                                              AS MauVT,
            CAST('' AS NVARCHAR(200))                                              AS WidthSize,
            CAST('' AS NVARCHAR(200))                                              AS DonViVT,
            ISNULL(kh.TenKH, ISNULL(ncc.TenKH, ''))                                AS TenKH,
            a.MaDVVT                                                               AS MaDVVT,
            a.SoLuong                                                              AS SoLuong,
            a.SoBarCode                                                            AS SoBarCode,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 3), '')                   AS MaVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 2), '')                   AS MauVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 1), '')                   AS KhoVaiID,
            CAST(a.NgayNhapTu AS DATE)                                             AS NgayNhap
        INTO #NhapFinalR_NDR
        FROM #NhapAggR_NDR a
        LEFT JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = a.SoLoID
        LEFT JOIN dbo.KhachHang     kh ON kh.MaKH    = nk.MaKH
        LEFT JOIN dbo.ERP_KhachHangNK ncc ON ncc.MaNhaCC = nk.NhaCungCap
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

        DROP TABLE #NhapAggR_NDR;
        DROP TABLE #NhapFinalR_NDR;
    END

    -- =========================================================================
    -- 15. GetXuatDetailByRange
    -- =========================================================================
    ELSE IF @Action = 'GetXuatDetailByRange'
    BEGIN
        SELECT TOP 2000
            xh.MaLenh,
            COUNT(*)                        AS SoBarCode,
            SUM(ISNULL(xh.SLNhap, 0))       AS SoLuong,
            COALESCE(MAX(NULLIF(xh.TenHang, '')), MAX(NULLIF(hh.TenHang, '')), '') AS TenHang,
            COALESCE(MAX(NULLIF(xh.TenKH, '')), MAX(NULLIF(kh.TenKH, '')), '')     AS TenKH,
            MIN(xh.NgayXuatHang)            AS NgayXuatTu,
            MAX(xh.NgayXuatHang)            AS NgayXuatDen
        FROM dbo.PhieuXuatHang xh
        LEFT JOIN dbo.DonHangTong dh ON dh.MaDH   = xh.MaGop
        LEFT JOIN dbo.HangHoa    hh ON hh.MaHang  = dh.MaHang AND hh.MaKH = dh.MaKH
        LEFT JOIN dbo.KhachHang  kh ON kh.MaKH    = dh.MaKH
        WHERE xh.ModuleXH = 1
          AND xh.NgayXuatHang IS NOT NULL
          AND xh.NgayXuatHang >= CAST(@TuNgay AS DATE)
          AND xh.NgayXuatHang <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
        GROUP BY xh.MaLenh
        ORDER BY SUM(xh.SLNhap) DESC;
    END

    -- =========================================================================
    -- 16. GetKiemKeDetailByRange
    -- =========================================================================
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
            SELECT TOP 2000
                ISNULL(kk.PhieuKiemKe, '')     AS PhieuKiemKe,
                ISNULL(kk.SoLo, '')            AS SoLo,
                COUNT(*)                       AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu,
                MIN(kk.DateKiemKe)             AS NgayKKTu,
                MAX(kk.DateKiemKe)             AS NgayKKDen
            FROM dbo.ERPPhieuKiemKe_NPLV2 kk
            LEFT JOIN dbo.SYS_NhanVien nv ON kk.UserKK = nv.UserID
            WHERE kk.DateKiemKe IS NOT NULL
              AND kk.DateKiemKe >= CAST(@TuNgay AS DATE)
              AND kk.DateKiemKe <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
        ELSE
        BEGIN
            SELECT TOP 2000
                ISNULL(kk.PhieuKiemKe, '')     AS PhieuKiemKe,
                ISNULL(kk.SoLo, '')            AS SoLo,
                COUNT(*)                       AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu,
                MIN(kk.DateKiemKe)             AS NgayKKTu,
                MAX(kk.DateKiemKe)             AS NgayKKDen
            FROM dbo.ERPPhieuKiemKe_NPL kk
            LEFT JOIN dbo.SYS_NhanVien nv ON kk.UserKK = nv.UserID
            WHERE kk.DateKiemKe IS NOT NULL
              AND kk.DateKiemKe >= CAST(@TuNgay AS DATE)
              AND kk.DateKiemKe <  DATEADD(DAY, 1, CAST(@DenNgay AS DATE))
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
    END

    -- =========================================================================
    -- 17. GetNhapDetailByDay
    -- =========================================================================
    ELSE IF @Action = 'GetNhapDetailByDay'
    BEGIN
        IF OBJECT_ID('tempdb..#NhapAgg_NDD') IS NOT NULL DROP TABLE #NhapAgg_NDD;
        SELECT
            ct.SoLoID,
            ct.MaNPL,
            MAX(ct.MaDVVT)                         AS MaDVVT,
            MAX(ct.POMua)                          AS PO,
            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuong,
            COUNT(*)                               AS SoBarCode
        INTO #NhapAgg_NDD
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        WHERE ct.NgayNhapKho IS NOT NULL
          AND CAST(ct.NgayNhapKho AS DATE) = CAST(@Ngay AS DATE)
        GROUP BY ct.SoLoID, ct.MaNPL;

        IF OBJECT_ID('tempdb..#NhapFinal_NDD') IS NOT NULL DROP TABLE #NhapFinal_NDD;
        SELECT TOP 500
            ISNULL(nk.SoLo, '')                    AS PINCC,
            a.PO                                   AS PO,
            a.MaNPL                                AS MaNPL,
            CAST('' AS NVARCHAR(200))              AS ItemCode,
            CAST('' AS NVARCHAR(200))              AS MaMauVT,
            CAST('' AS NVARCHAR(200))              AS MauVT,
            CAST('' AS NVARCHAR(200))              AS WidthSize,
            ISNULL(kh.TenKH, ISNULL(ncc.TenKH, '')) AS TenKH,
            ISNULL(d.TenDVVT, '')                  AS DonViVT,
            a.SoLuong                              AS SoLuong,
            a.SoBarCode                            AS SoBarCode,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 3), '') AS MaVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 2), '') AS MauVTID,
            ISNULL(PARSENAME(REPLACE(a.MaNPL, '@', '.'), 1), '') AS KhoVaiID
        INTO #NhapFinal_NDD
        FROM #NhapAgg_NDD a
        LEFT JOIN dbo.ERP_NhapKhoNPL nk ON nk.SoLoID = a.SoLoID
        LEFT JOIN dbo.KhachHang     kh  ON kh.MaKH   = nk.MaKH
        LEFT JOIN dbo.ERP_KhachHangNK ncc ON ncc.MaNhaCC = nk.NhaCungCap
        LEFT JOIN dbo.ERP_DonViVT   d   ON d.MaDVVT  = a.MaDVVT
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
                    SET WidthSize = LTRIM(RTRIM(ISNULL(k.KhoVai, '''') + '' '' + ISNULL(d.TenDVVT, '''')))
                    FROM #NhapFinal_NDD f
                    LEFT JOIN dbo.ERP_KhoVai k  ON k.KhoVaiID = f.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT d ON d.MaDVVT   = k.MaDVVT;';
            END TRY BEGIN CATCH END CATCH
        END

        SELECT PINCC, PO, MaNPL, ItemCode, MaMauVT, MauVT, WidthSize, DonViVT, TenKH, SoLuong, SoBarCode
        FROM #NhapFinal_NDD
        ORDER BY SoLuong DESC;

        DROP TABLE #NhapAgg_NDD;
        DROP TABLE #NhapFinal_NDD;
    END

    -- =========================================================================
    -- 18. GetXuatDetailByDay
    -- =========================================================================
    ELSE IF @Action = 'GetXuatDetailByDay'
    BEGIN
        SELECT TOP 500
            xh.MaLenh,
            COUNT(*)                        AS SoBarCode,
            SUM(ISNULL(xh.SLNhap, 0))       AS SoLuong,
            COALESCE(MAX(NULLIF(xh.TenHang, '')), MAX(NULLIF(hh.TenHang, '')), '') AS TenHang,
            COALESCE(MAX(NULLIF(xh.TenKH, '')), MAX(NULLIF(kh.TenKH, '')), '')     AS TenKH
        FROM dbo.PhieuXuatHang xh
        LEFT JOIN dbo.DonHangTong dh ON dh.MaDH   = xh.MaGop
        LEFT JOIN dbo.HangHoa    hh ON hh.MaHang  = dh.MaHang AND hh.MaKH = dh.MaKH
        LEFT JOIN dbo.KhachHang  kh ON kh.MaKH    = dh.MaKH
        WHERE xh.ModuleXH = 1
          AND xh.NgayXuatHang IS NOT NULL
          AND CAST(xh.NgayXuatHang AS DATE) = CAST(@Ngay AS DATE)
        GROUP BY xh.MaLenh
        ORDER BY SUM(xh.SLNhap) DESC;
    END

    -- =========================================================================
    -- 19. GetKiemKeDetailByDay
    -- =========================================================================
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
                COUNT(*)                       AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu
            FROM dbo.ERPPhieuKiemKe_NPLV2 kk
            LEFT JOIN dbo.SYS_NhanVien nv ON kk.UserKK = nv.UserID
            WHERE kk.DateKiemKe IS NOT NULL
              AND CAST(kk.DateKiemKe AS DATE) = CAST(@Ngay AS DATE)
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
        ELSE
        BEGIN
            SELECT TOP 500
                ISNULL(kk.PhieuKiemKe, '')     AS PhieuKiemKe,
                ISNULL(kk.SoLo, '')            AS SoLo,
                COUNT(*)                       AS SoBarCode,
                SUM(ISNULL(kk.SLKiemKe, 0))    AS SoLuong,
                ISNULL(MAX(nv.Ten), ISNULL(MAX(kk.UserKK), '')) AS UserKK,
                ISNULL(MAX(kk.GhiChu), '')     AS GhiChu
            FROM dbo.ERPPhieuKiemKe_NPL kk
            LEFT JOIN dbo.SYS_NhanVien nv ON kk.UserKK = nv.UserID
            WHERE kk.DateKiemKe IS NOT NULL
              AND CAST(kk.DateKiemKe AS DATE) = CAST(@Ngay AS DATE)
            GROUP BY kk.PhieuKiemKe, kk.SoLo
            ORDER BY SUM(kk.SLKiemKe) DESC;
        END
    END

    -- =========================================================================
    -- 20. GetRackSlotDetail
    -- =========================================================================
    ELSE IF @Action = 'GetRackSlotDetail'
    BEGIN
        SELECT BarCode INTO #tmpXuat_RSD FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        SELECT TOP (0) BarCode INTO #tmpSoanHang_RSD FROM ERP_SoanHangNPL_BarCode;
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tmpSoanHang_RSD(BarCode)
            EXEC sp_executesql N'SELECT BarCode FROM ERP_SoanHangNPL_BarCode
                WHERE ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';
        END

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
        LEFT  JOIN dbo.ERP_KhachHangNK ncc ON ncc.MaNhaCC = nk.NhaCungCap
        WHERE vt.MaONPL IS NOT NULL
          AND ke.Module IN (1, 2)
          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE '%co%'
          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE N'%l?i%'
          AND LOWER(ISNULL(d.TenDay, N'')) NOT LIKE N'%n%'
          AND NOT EXISTS (SELECT 1 FROM #tmpXuat_RSD tx WHERE tx.BarCode = vt.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #tmpSoanHang_RSD ts WHERE ts.BarCode = vt.Barcode);

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
        DROP TABLE #tmpXuat_RSD;
        DROP TABLE #tmpSoanHang_RSD;
    END

    -- =========================================================================
    -- 21. GetActivityCalendar
    -- =========================================================================
    ELSE IF @Action = 'GetActivityCalendar'
    BEGIN
        SET @StartDateCal = CAST(@TuNgay  AS DATE);
        SET @EndDateCal   = CAST(@DenNgay AS DATE);
        SET @TotalDaysCal  = DATEDIFF(DAY, @StartDateCal, @EndDateCal) + 1;
        IF @TotalDaysCal < 1   SET @TotalDaysCal = 1;
        IF @TotalDaysCal > 400 SET @TotalDaysCal = 400;

        IF OBJECT_ID('tempdb..#CalDays_AC') IS NOT NULL DROP TABLE #CalDays_AC;
        SELECT TOP (@TotalDaysCal)
            DATEADD(DAY, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) - 1, @StartDateCal) AS NgayHoatDong
        INTO #CalDays_AC
        FROM master..spt_values WHERE type = 'P';

        SET @EndPlus1Cal = DATEADD(DAY, 1, @EndDateCal);

        IF OBJECT_ID('tempdb..#CalNhap_AC') IS NOT NULL DROP TABLE #CalNhap_AC;
        SELECT CAST(NgayNhapKho AS DATE) AS Ngay,
               SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS TotalIn
        INTO #CalNhap_AC
        FROM dbo.ERP_ChiTietNhapKhoNPL
        WHERE NgayNhapKho >= @StartDateCal
          AND NgayNhapKho <  @EndPlus1Cal
        GROUP BY CAST(NgayNhapKho AS DATE);

        IF OBJECT_ID('tempdb..#CalXuat_AC') IS NOT NULL DROP TABLE #CalXuat_AC;
        SELECT CAST(NgayXuatHang AS DATE) AS Ngay,
               SUM(ISNULL(SLNhap, 0)) AS TotalOut
        INTO #CalXuat_AC
        FROM dbo.PhieuXuatHang
        WHERE ModuleXH = 1
          AND NgayXuatHang >= @StartDateCal
          AND NgayXuatHang <  @EndPlus1Cal
        GROUP BY CAST(NgayXuatHang AS DATE);

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

        SELECT
            CONVERT(VARCHAR(10), d.NgayHoatDong, 120) AS NgayHoatDong,
            ISNULL(n.TotalIn,     0) AS TotalIn,
            ISNULL(x.TotalOut,    0) AS TotalOut,
            ISNULL(k.TotalKiemKe, 0) AS TotalKiemKe,
            ISNULL(n.TotalIn, 0) + ISNULL(x.TotalOut, 0) + ISNULL(k.TotalKiemKe, 0) AS TotalActivity
        FROM #CalDays_AC d
        LEFT JOIN #CalNhap_AC n ON n.Ngay = d.NgayHoatDong
        LEFT JOIN #CalXuat_AC x ON x.Ngay = d.NgayHoatDong
        LEFT JOIN #CalKK_AC   k ON k.Ngay = d.NgayHoatDong
        ORDER BY d.NgayHoatDong;

        DROP TABLE #CalDays_AC;
        DROP TABLE #CalNhap_AC;
        DROP TABLE #CalXuat_AC;
        DROP TABLE #CalKK_AC;
    END

    -- =========================================================================
    -- 22. GetFlowTrendByRange
    -- =========================================================================
    ELSE IF @Action = 'GetFlowTrendByRange'
    BEGIN
        SET @StartDateRange = @TuNgay;
        SET @EndDateRange   = @DenNgay;
        SET @TotalDaysRange  = DATEDIFF(DAY, @StartDateRange, @EndDateRange) + 1;
        IF @TotalDaysRange < 1 SET @TotalDaysRange = 1;
        IF @TotalDaysRange > 400 SET @TotalDaysRange = 400;

        ;WITH Days AS (
            SELECT TOP (@TotalDaysRange)
                CAST(DATEADD(DAY, number, @StartDateRange) AS DATE) AS Ngay
            FROM master..spt_values
            WHERE type = 'P' AND number BETWEEN 0 AND 399
        ),
        TonDauKy AS (
            SELECT ISNULL(SUM(SL), 0) AS TonDau FROM (
                SELECT SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS SL
                FROM ERP_ChiTietNhapKhoNPL
                WHERE TRY_CONVERT(DATE, NgayNhapKho) < @StartDateRange
                UNION ALL
                SELECT -SUM(ISNULL(SLNhap, 0))
                FROM PhieuXuatHang
                WHERE ModuleXH = 1
                  AND TRY_CONVERT(DATE, NgayXuatHang) < @StartDateRange
                UNION ALL
                SELECT SUM(ISNULL(ThuHoi, 0))
                FROM PhieuThuHoiNPL
                WHERE TRY_CONVERT(DATE, NgayTH) < @StartDateRange
            ) t
        ),
        NhapTheoNgay AS (
            SELECT CAST(NgayNhapKho AS DATE) AS Ngay,
                   SUM(ISNULL(SoLuongThucTeBanDau, 0)) AS TotalIn
            FROM ERP_ChiTietNhapKhoNPL
            WHERE NgayNhapKho >= @StartDateRange
              AND NgayNhapKho <  DATEADD(DAY, 1, @EndDateRange)
            GROUP BY CAST(NgayNhapKho AS DATE)
        ),
        XuatTheoNgay AS (
            SELECT CAST(NgayXuatHang AS DATE) AS Ngay,
                   SUM(ISNULL(SLNhap, 0)) AS TotalOut
            FROM PhieuXuatHang
            WHERE ModuleXH = 1
              AND NgayXuatHang >= @StartDateRange
              AND NgayXuatHang <  DATEADD(DAY, 1, @EndDateRange)
            GROUP BY CAST(NgayXuatHang AS DATE)
        ),
        ThuHoiTheoNgay AS (
            SELECT CAST(NgayTH AS DATE) AS Ngay,
                   SUM(ISNULL(ThuHoi, 0)) AS TotalTH
            FROM PhieuThuHoiNPL
            WHERE NgayTH >= @StartDateRange
              AND NgayTH <  DATEADD(DAY, 1, @EndDateRange)
            GROUP BY CAST(NgayTH AS DATE)
        ),
        Combined AS (
            SELECT d.Ngay,
                   ISNULL(n.TotalIn, 0)  AS TotalIn,
                   ISNULL(x.TotalOut, 0) AS TotalOut,
                   ISNULL(th.TotalTH, 0) AS TotalTH
            FROM Days d
            LEFT JOIN NhapTheoNgay   n  ON n.Ngay  = d.Ngay
            LEFT JOIN XuatTheoNgay   x  ON x.Ngay  = d.Ngay
            LEFT JOIN ThuHoiTheoNgay th ON th.Ngay = d.Ngay
        )
        SELECT
            CONVERT(VARCHAR(10), c.Ngay, 120) AS Ngay,
            c.TotalIn,
            c.TotalOut,
            ISNULL(d.TonDau, 0)
              + SUM(c.TotalIn + c.TotalTH - c.TotalOut)
                OVER (ORDER BY c.Ngay ROWS UNBOUNDED PRECEDING) AS TotalStock
        FROM Combined c CROSS JOIN TonDauKy d
        ORDER BY c.Ngay;
    END

    -- =========================================================================
    -- 23. GetMoMComparison
    -- =========================================================================
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

    -- =========================================================================
    -- 24. GetTop5
    -- =========================================================================
    ELSE IF @Action = 'GetTop5'
    BEGIN
        CREATE TABLE #tmpXuatChuaThuHoi_T5 (
            BarCode NVARCHAR(500) NOT NULL PRIMARY KEY
        );
        INSERT INTO #tmpXuatChuaThuHoi_T5 (BarCode)
        SELECT DISTINCT t1.BarCode
        FROM PhieuXuatHang t1
        WHERE t1.BarCode IS NOT NULL
          AND EXISTS (SELECT 1 FROM dbo.ERP_VatTuCBM v WHERE v.Barcode = t1.BarCode)
          AND NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND t2.BarCode = t1.BarCode
              AND t1.Dot = t2.Dot
          )
          AND NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND t2.BarCode = t1.BarCodeGoc
              AND t1.Dot = t2.Dot
          );

        CREATE TABLE #tmpSH_T5 (
            BarCode NVARCHAR(500) NOT NULL PRIMARY KEY
        );
        IF COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_TK') IS NOT NULL
           AND COL_LENGTH('dbo.ERP_SoanHangNPL_BarCode','SLSoanHang_BC') IS NOT NULL
        BEGIN
            INSERT INTO #tmpSH_T5(BarCode)
            EXEC sp_executesql N'SELECT DISTINCT BarCode FROM ERP_SoanHangNPL_BarCode
                WHERE BarCode IS NOT NULL AND ISNULL(SLSoanHang_TK,0) - ISNULL(SLSoanHang_BC,0) = 0;';
        END


        IF OBJECT_ID('tempdb..#TopRaw_T5') IS NOT NULL DROP TABLE #TopRaw_T5;
        SELECT
            ct.MaNPL,
            o.Module,
            SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho
        INTO #TopRaw_T5
        FROM dbo.ERP_ChiTietNhapKhoNPL ct
        INNER JOIN dbo.ERP_VatTuCBM v ON v.Barcode = ct.BarCode
        INNER JOIN dbo.ERP_ONPL    o ON o.TenO     = v.MaONPL
        WHERE v.MaONPL IS NOT NULL
          AND o.Module IN (1, 2)
          AND (@LoaiNPL = 0 OR o.Module = @LoaiNPL)
          AND NOT EXISTS (SELECT 1 FROM #tmpXuatChuaThuHoi_T5 t WHERE t.BarCode = v.Barcode)
          AND NOT EXISTS (SELECT 1 FROM #tmpSH_T5       t WHERE t.BarCode = v.Barcode)
        GROUP BY ct.MaNPL, o.Module
        HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) > 0;

        IF OBJECT_ID('tempdb..#TopParsed_T5') IS NOT NULL DROP TABLE #TopParsed_T5;
        SELECT
            r.MaNPL,
            r.Module,
            r.TonKho,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,
            ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID
        INTO #TopParsed_T5
        FROM #TopRaw_T5 r;

        IF OBJECT_ID('tempdb..#TopFinal_T5') IS NOT NULL DROP TABLE #TopFinal_T5;
        SELECT
            p.MaVTID                          AS MaVTID_Raw,
            CAST(p.MaVTID AS NVARCHAR(200))   AS MaVT,
            CAST('' AS NVARCHAR(500))         AS TenVT,
            CAST('' AS NVARCHAR(200))         AS Mau,
            CAST('' AS NVARCHAR(200))         AS KhoVai,
            CAST('' AS NVARCHAR(500))         AS ChiTiet,
            CAST('' AS NVARCHAR(50))          AS TenDVVT,
            p.TonKho,
            p.Module                          AS NPL,
            p.MaCLVTID,
            p.MauVTID,
            p.KhoVaiID
        INTO #TopFinal_T5
        FROM #TopParsed_T5 p;

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

        DROP TABLE #TopRaw_T5;
        DROP TABLE #TopParsed_T5;
        DROP TABLE #TopFinal_T5;
        DROP TABLE #tmpXuatChuaThuHoi_T5;
        DROP TABLE #tmpSH_T5;
    END

    -- =========================================================================
    -- 25. GetCongViecChoXuLy
    -- Logic: ERP_ChiTietNhapKhoNPL WHERE IsDuyetNK <> 1
    --        AND EXISTS in (QTY_KiemVaiV2 DuyetQC=1 OR Qty_KiemPL_XacNhan Is_XN_SoLo=1)
    -- =========================================================================
    ELSE IF @Action = 'GetCongViecChoXuLy'
    BEGIN
        DECLARE @ItemcodeChoNK INT = 0;
        DECLARE @KKChoDuyet INT = 0;

        -- 1. ItemCode ch? nh?p kho:
        --    ERP_ChiTietNhapKhoNPL.IsDuyetNK <> 1 (NULL ho?c 0)
        --    AND Ã°? qua QC (t?n t?i trong b?ng ki?m v?i DuyetQC=1 ho?c Is_XN_SoLo=1)
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

        -- 2. Ki?m kÃª ch? duy?t
        SELECT @KKChoDuyet = COUNT(DISTINCT CONCAT(t1.SoLoID, '|', t1.MaNPL))
        FROM dbo.ERPPhieuKiemKe_NPL t1
        WHERE t1.IsXacNhan IS NULL;

        SELECT 'itemcode_cho_nk' AS MaCV,
               N'ItemCode ch? nh?p kho' AS TenCV,
               N'ItemCode QC Ã°? k? duy?t nhÃ½ng chÃ½a nh?p kho' AS MoTa,
               @ItemcodeChoNK AS SoLuong,
               N'ItemCode' AS DonVi,
               'clipboard-check' AS Icon
        UNION ALL
        SELECT 'kk_cho_duyet',
               N'Ki?m kÃª ch? duy?t',
               N'ItemCode Ã°? ki?m kÃª ch? b?n duy?t k?t qu?',
               @KKChoDuyet,
               N'ItemCode',
               'clipboard-list';
    END

    -- =========================================================================
    -- 26. GetTodoDetail
    -- itemcode_cho_nk: t? ERP_ChiTietNhapKhoNPL JOIN ki?m tables
    -- kk_cho_duyet: t? ERPPhieuKiemKe_NPL
    -- =========================================================================
    ELSE IF @Action = 'GetTodoDetail'
    BEGIN
        DECLARE @TodoType NVARCHAR(50) = ISNULL(@Itemcode, 'itemcode_cho_nk');

        IF @TodoType = 'itemcode_cho_nk'
        BEGIN
            -- v2.7.2 FIX: DÃ¹ng OUTER APPLY TOP 1 cho nk Ã°? trÃ¡nh fanout khi 1 SoLoID cÃ³ nhi?u d?ng trong ERP_NhapKhoNPL
            SELECT
                ROW_NUMBER() OVER (ORDER BY t1.SoLoID, t1.MaNPL) AS STT,
                ISNULL(vt.MaVT, '') AS ItemCode,
                ISNULL(vt.ChiTiet, '') AS TenVT,
                ISNULL(t1.SoLoID, '') AS POMua,
                ISNULL(mau.MaMauVT, '') AS MaMauVT,
                ISNULL(mau.MauVT, '') AS MauVT,
                ISNULL(kv.KhoVai, '') AS WidthSize,
                CONVERT(VARCHAR(10), t1.NgayTaoNhapKho, 103) AS NgayTao,
                ISNULL(kh.TenKH, '') AS NCC,
                ISNULL(t1.SLTong, 0) AS SLMua,
                ISNULL(t1.SoLuongThucTeBanDau, 0) AS SLVe,
                'itemcode_cho_nk' AS Type
            FROM dbo.ERP_ChiTietNhapKhoNPL t1
            LEFT JOIN dbo.ERP_VatTuTV vt ON vt.MaVTID = t1.MaVTID
            LEFT JOIN dbo.ERP_MauVTTV mau ON mau.MauVTID = t1.MauVTID
            LEFT JOIN dbo.ERP_KhoVai kv ON kv.KhoVaiID = t1.KhoVaiID
            OUTER APPLY (
                SELECT TOP 1 nk.NhaCungCap
                FROM dbo.ERP_NhapKhoNPL nk
                WHERE nk.SoLoID = t1.SoLoID
                ORDER BY nk.SoLoID
            ) nk_top
            LEFT JOIN dbo.ERP_KhachHangNK kh ON nk_top.NhaCungCap = kh.MaNhaCC
            WHERE ISNULL(t1.IsDuyetNK, 0) <> 1
              AND (
                  EXISTS (
                      SELECT 1 FROM dbo.QTY_KiemVaiV2 kv2
                      INNER JOIN dbo.QTY_KiemVaiV2_XacNhan kx
                          ON kv2.SoLoID = kx.SoLoID AND kv2.MaNPL = kx.MaNPL
                      WHERE kv2.SoLoID = t1.SoLoID AND kv2.MaNPL = t1.MaNPL
                        AND kv2.DuyetQC = 1
                  )
                  OR EXISTS (
                      SELECT 1 FROM dbo.Qty_KiemPL_XacNhan kp
                      WHERE kp.SoLoID = t1.SoLoID AND kp.MaNPL = t1.MaNPL
                        AND kp.Is_XN_SoLo = 1
                  )
              )
            ORDER BY t1.SoLoID, t1.MaNPL;
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
                ISNULL(mau.MaMauVT, '') AS MaMauVT,
                ISNULL(mau.MauVT, '') AS MauVT,
                ISNULL(kv.KhoVai, '') AS WidthSize,
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

    -- =========================================================================
    -- 27. GetTinhHinhKiemKe
    -- =========================================================================
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

    -- =========================================================================
    -- 7b. GlobalSearchAll (T?m ki?m ToÃ n nÃ£ng)
    -- =========================================================================
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
            -- 1. T?m PO (Chu?n b? v?)
            INSERT INTO #SearchResults (Category, Title, Subtitle, TargetID, SortOrder)
            SELECT TOP 10 
                'PO', 
                ISNULL(nk.POMua, nk.SoLoID), 
                N'PO: ' + ISNULL(nk.POMua, '') + N' - L?nh nh?p: ' + ISNULL(nk.SoLoID, '') + N' - KhÃ¡ch: ' + ISNULL(kh.TenKH, ISNULL(nk.KhachHang, '')) + N' - D? ki?n: ' + ISNULL(CONVERT(VARCHAR(10), nk.NgayNKDuKien, 103), ''),
                nk.SoLoID, 1
            FROM dbo.ERP_NhapKhoNPL nk
            LEFT JOIN dbo.KhachHang kh ON nk.MaHang = kh.MaKH OR nk.MaKH = kh.MaKH
            WHERE nk.SoLoID LIKE @PatternStr 
               OR nk.POMua LIKE @PatternStr 
               OR nk.MaHang LIKE @PatternStr 
               OR nk.KhachHang LIKE @PatternStr
               OR kh.TenKH LIKE @PatternStr;

            -- 2. T?m Itemcode vÃ  V? trÃ­ k?
            INSERT INTO #SearchResults (Category, Title, Subtitle, TargetID, SortOrder)
            SELECT TOP 15
                'ITEM_RACK',
                vt.MaVT,
                N'V?t tÃ½: ' + ISNULL(vt.ChiTiet, '') + N' | K?: ' + ISNULL(kv.KhoVai, N'ChÃ½a x?p k?') + N' - T?n: ' + CAST(CAST(SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS DECIMAL(18,2)) AS NVARCHAR(50)),
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





    ELSE IF @Action = 'GetTop5KhachHangTonKho'
    BEGIN
        SELECT BarCode INTO #TempXCTH_T5KH FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );
        
        WITH CTE AS (
            SELECT 
                ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'KhÃ¡ch tr?ng')) AS KhachHang, 
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS GiaTriTon
            FROM ERP_ChiTietNhapKhoNPL ct 
            INNER JOIN ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
            LEFT JOIN KhachHang kh ON nk.MaKH = kh.MaKH
            WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
              AND (ct.BarCode IS NULL OR ct.BarCode NOT IN (SELECT BarCode FROM #TempXCTH_T5KH))
            GROUP BY ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'KhÃ¡ch tr?ng'))
        ),
        TotalCTE AS (
            SELECT SUM(GiaTriTon) AS TotalValue FROM CTE
        )
        SELECT TOP 5 
            ROW_NUMBER() OVER(ORDER BY c.GiaTriTon DESC) AS STT,
            c.KhachHang, 
            c.GiaTriTon AS GiaTri, -- Alias for JS 
            CASE WHEN ISNULL(t.TotalValue, 0) = 0 THEN 0.0 ELSE ROUND((c.GiaTriTon / t.TotalValue) * 100, 2) END AS TyTrong 
        FROM CTE c
        CROSS JOIN TotalCTE t
        ORDER BY c.GiaTriTon DESC;
    END

    ELSE IF @Action = 'GetKhachHangTonKhoChiTiet'
    BEGIN
        SELECT BarCode INTO #TempXCTH_KHTKCT FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        WITH CTE AS (
            SELECT 
                ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'KhÃ¡ch tr?ng')) AS KhachHang, 
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS GiaTri
            FROM ERP_ChiTietNhapKhoNPL ct 
            INNER JOIN ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
            LEFT JOIN KhachHang kh ON nk.MaKH = kh.MaKH
            WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
              AND (ct.BarCode IS NULL OR ct.BarCode NOT IN (SELECT BarCode FROM #TempXCTH_KHTKCT))
            GROUP BY ISNULL(kh.TenKH, ISNULL(nk.KhachHang, N'KhÃ¡ch tr?ng'))
        ),
        TotalCTE AS (
            SELECT SUM(GiaTri) AS TotalValue FROM CTE
        )
        SELECT 
            ROW_NUMBER() OVER(ORDER BY c.GiaTri DESC) AS STT,
            c.KhachHang, 
            c.GiaTri, 
            CASE WHEN ISNULL(t.TotalValue, 0) = 0 THEN 0.0 ELSE ROUND((c.GiaTri / t.TotalValue) * 100, 2) END AS TyTrong 
        FROM CTE c
        CROSS JOIN TotalCTE t
        ORDER BY c.GiaTri DESC;
    END

    ELSE IF @Action = 'GetGiaTriTonKhoTheoNhom'
    BEGIN
        SELECT BarCode INTO #TempXCTH_GTTK FROM PhieuXuatHang t1
        WHERE NOT EXISTS (
            SELECT 1 FROM PhieuThuHoiNPL t2
            WHERE t2.MaLenh = t1.MaLenhSX
              AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
              AND t1.Dot = t2.Dot
        );

        WITH BaseData AS (
            SELECT 
                ISNULL(nh.TenNhom, N'KhÃ¡c') AS Nhom, 
                SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(CAST(ct.DonGia AS DECIMAL(20,4)), 0)) AS GiaTri
            FROM ERP_ChiTietNhapKhoNPL ct 
            LEFT JOIN (SELECT MaCLVT, MAX(TenNhom) AS TenNhom FROM NhomNguyenPhuLieu GROUP BY MaCLVT) nh ON ct.MaNhom = nh.MaCLVT
            WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0 
              AND (ct.BarCode IS NULL OR ct.BarCode NOT IN (SELECT BarCode FROM #TempXCTH_GTTK))
            GROUP BY ISNULL(nh.TenNhom, N'KhÃ¡c')
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
            SELECT N'KhÃ¡c' AS Nhom, SUM(GiaTri) AS GiaTri 
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
        ORDER BY CASE WHEN c.Nhom = N'KhÃ¡c' THEN 1 ELSE 0 END, c.GiaTri DESC;
    END

                ELSE IF @Action = 'GetGiaTriNhomChiTiet'
      BEGIN
          SELECT BarCode INTO #TempXCTH_GTNCT FROM PhieuXuatHang t1
          WHERE NOT EXISTS (
              SELECT 1 FROM PhieuThuHoiNPL t2
              WHERE t2.MaLenh = t1.MaLenhSX
                AND (t2.BarCode = t1.BarCode OR t2.BarCode = t1.BarCodeGoc)
                AND t1.Dot = t2.Dot
          );

          SELECT 
              ISNULL(nh.TenNhom, N'KhÃ¡c') AS ParentNhom,
              ct.MaVTID AS MaVT,
              ISNULL(vt.ChiTiet, '') AS Nhom, 
              SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(CAST(ct.DonGia AS DECIMAL(20,4)), 0)) AS GiaTri,
              SUM(ISNULL(cbm.CBM, 0)) AS TongCBM
          INTO #RawDetails
          FROM ERP_ChiTietNhapKhoNPL ct
          LEFT JOIN ERP_VatTuCBM cbm ON ct.BarCode = cbm.Barcode
          LEFT JOIN (SELECT MaCLVT, MAX(TenNhom) AS TenNhom FROM NhomNguyenPhuLieu GROUP BY MaCLVT) nh ON ct.MaNhom = nh.MaCLVT
          LEFT JOIN ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID 
          WHERE ISNULL(ct.SoLuongThucTeBanDau, 0) > 0 
            AND (ct.BarCode IS NULL OR ct.BarCode NOT IN (SELECT BarCode FROM #TempXCTH_GTNCT))
          GROUP BY ISNULL(nh.TenNhom, N'KhÃ¡c'), ct.MaVTID, ISNULL(vt.ChiTiet, '');

          DECLARE @TotalGiaTri DECIMAL(18,2) = (SELECT SUM(GiaTri) FROM #RawDetails);

          SELECT 
              ParentNhom AS Nhom,
              SUM(GiaTri) AS GiaTri,
              COUNT(DISTINCT MaVT) AS SoMaVT,
              SUM(TongCBM) AS TongCBM,
              CASE WHEN ISNULL(@TotalGiaTri, 0) > 0 THEN ROUND(SUM(GiaTri) / @TotalGiaTri * 100, 2) ELSE 0 END AS TyTrong
          INTO #RawGroups
          FROM #RawDetails
          GROUP BY ParentNhom;

          (
              SELECT 
                  1 AS IsGroup,
                  Nhom,
                  ROW_NUMBER() OVER (ORDER BY CASE WHEN Nhom = N'KhÃ¡c' THEN 1 ELSE 0 END, GiaTri DESC) AS STT,
                  SoMaVT,
                  TongCBM,
                  GiaTri,
                  TyTrong,
                  NULL AS ParentNhom,
                  NULL AS MaVT
              FROM #RawGroups
          )
          UNION ALL
          (
              SELECT 
                  0 AS IsGroup,
                  Nhom,
                  NULL AS STT,
                  NULL AS SoMaVT,
                  TongCBM,
                  GiaTri,
                  NULL AS TyTrong,
                  ParentNhom,
                  MaVT
              FROM #RawDetails
          )
          ORDER BY IsGroup DESC, STT ASC, ParentNhom ASC, GiaTri DESC;
      END

    ELSE IF @Action = 'GetTinhHinhKiemKe'
    BEGIN
        SELECT 0 AS DaKiem, 0 AS Lech, 0 AS ChuaKiem;
    END

    ELSE IF @Action = 'GetKiemKeChiTiet'
    BEGIN
        SELECT TOP 0 '' AS MaVT;
    END
    ELSE
    BEGIN
        SELECT 'Unknown action: ' + ISNULL(@Action, 'NULL') AS Error;
    END
END

