USE [PMS_QLDH_VIKING_2025]
GO

CREATE PROCEDURE [dbo].[BieuDo_GetFlowTrendByRange]
    @TuNgay DATETIME,
    @DenNgay DATETIME,
    @MaNPL NVARCHAR(50) = 'all',
    @SoLoID NVARCHAR(50) = 'all',
    @MaHang NVARCHAR(50) = 'all',
    @MaKH NVARCHAR(50) = 'all',
    @KhoLoi NVARCHAR(10) = '0',
    @Nhom NVARCHAR(50) = 'all',
    @IsNPL INT = 2
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartDateRange DATE = CAST(@TuNgay AS DATE);
    DECLARE @EndDateRange DATE = CAST(@DenNgay AS DATE);
    DECLARE @TotalDaysRange INT = DATEDIFF(DAY, @StartDateRange, @EndDateRange) + 1;
    IF @TotalDaysRange < 1 SET @TotalDaysRange = 1;
    IF @TotalDaysRange > 400 SET @TotalDaysRange = 400;

        -- 1. XÂY DỰNG LÕI DỮ LIỆU CHUẨN (Core Logic)
        DECLARE @IsNPL_Check NVARCHAR(200);
        SELECT TOP 1 @IsNPL_Check = IsNPL FROM ERP_ChiTietNhapKhoNPL WHERE MaNPL = @MaNPL;

        IF OBJECT_ID('tempdb..#tempNhapKhoNPL') IS NOT NULL DROP TABLE #tempNhapKhoNPL;
        SELECT t1.* INTO #tempNhapKhoNPL FROM ERP_NhapKhoNPL t1
        WHERE (@MaKH = 'all' OR t1.MaKH = @MaKH) 
          AND (@MaHang = 'all' OR t1.MaHang = @MaHang) 
          AND (@SoLoID = 'all' OR t1.SoLoID = @SoLoID)  
          AND (@IsNPL = 2 OR IsNPL = @IsNPL_Check);

        IF OBJECT_ID('tempdb..#tblVTLoi') IS NOT NULL DROP TABLE #tblVTLoi;
        SELECT t1.* INTO #tblVTLoi FROM ERP_VatTuCBM t1
        INNER JOIN ERP_ONPL o ON o.TenO = t1.MaONPL
        INNER JOIN ERP_DayNPL d ON d.DayID = o.DayID AND d.Status = 2
        WHERE t1.MaONPL IS NOT NULL;

        IF OBJECT_ID('tempdb..#tblKV') IS NOT NULL DROP TABLE #tblKV;
        SELECT t1.KhoVaiID, CONCAT(t1.KhoVai, ' ', t2.TenDVVT) AS KhoVai INTO #tblKV
        FROM ERP_KhoVai t1 LEFT JOIN ERP_DonViVT t2 ON t1.MaDVVT = t2.MaDVVT
        UNION ALL SELECT DISTINCT MaSize AS KhoVaiID, TenSize AS KhoVai FROM BangSize;

        IF OBJECT_ID('tempdb..#tblBarcodeHopLe') IS NOT NULL DROP TABLE #tblBarcodeHopLe;
        SELECT Barcode INTO #tblBarcodeHopLe FROM ERP_VatTuCBM WHERE ISNULL(MaONPL, '') <> ''
        UNION SELECT t9.Barcode FROM ERP_VatTuCBM t9 INNER JOIN PhieuXuatHang px ON px.BarCodeGoc = t9.Barcode
        UNION SELECT t9.Barcode FROM ERP_VatTuCBM t9 INNER JOIN PhieuThuHoiNPL th ON th.BarCode = t9.Barcode;

        IF OBJECT_ID('tempdb..#tempTKho') IS NOT NULL DROP TABLE #tempTKho;
        SELECT t1.ID, t1.SoloID, t1.MaNPL, t1.IsNPL, t1.BarCode, t1.SoLuongThucTeBanDau
        INTO #tempTKho 
        FROM ERP_ChiTietNhapKhoNPL t1
        INNER JOIN #tempNhapKhoNPL tt ON t1.SoLoID = tt.SoLoID AND t1.IsNPL = tt.IsNPL
        INNER JOIN #tblBarcodeHopLe bh ON bh.Barcode = t1.BarCode
        WHERE (@Nhom = 'all' OR t1.MaNhom = @Nhom) 
          AND (@MaNPL = 'all' OR t1.MaNPL = @MaNPL) 
          AND t1.SoLuongThucTeBanDau <> 0 
          AND (@SoLoID = 'all' OR t1.SoLoID = @SoLoID) 
          AND (@MaHang = 'all' OR ISNULL(tt.MaHang, '') = @MaHang) 
          AND (@MaKH = 'all' OR ISNULL(tt.MaKH, '') = @MaKH) 
          AND ((@KhoLoi = '1' AND EXISTS (SELECT 1 FROM #tblVTLoi vtl WHERE t1.BarCode = vtl.Barcode))
            OR (@KhoLoi = '0' AND NOT EXISTS (SELECT 1 FROM #tblVTLoi vtl WHERE t1.BarCode = vtl.Barcode)));

        -- 2. TÍNH TỒN ĐẦU KỲ
        DECLARE @TonDauKy DECIMAL(18,4) = 0;
        DECLARE @TonNhapDK DECIMAL(18,4) = 0;
        DECLARE @TonXuatDK DECIMAL(18,4) = 0;
        DECLARE @TonThuDK DECIMAL(18,4) = 0;
        DECLARE @TonChenhDK DECIMAL(18,4) = 0;

        SELECT @TonChenhDK = ROUND(SUM(ISNULL(SLKiemKeBanDau,0) - ISNULL(SLKiemKeEdit,0)), 4)
        FROM ERPPhieuKiemKe_NPL t1
        WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL AND CONVERT(DATE, DateDuyetKK) < @StartDateRange
          AND EXISTS (SELECT 1 FROM #tempTKho t2 WHERE t1.BarCode = t2.BarCode);

        SELECT @TonNhapDK = SUM(t1.SoLuongThucTeBanDau)
        FROM ERP_ChiTietNhapKhoNPL t1
        INNER JOIN #tempNhapKhoNPL t2 ON t1.SoLoID = t2.SoLoID AND t1.IsNPL = t2.IsNPL
        INNER JOIN #tempTKho t4 ON t1.ID = t4.ID
        WHERE CONVERT(DATE, t1.NgayNhapKho) < @StartDateRange;

        SELECT @TonXuatDK = SUM(SLNhap)
        FROM PhieuXuatHang t1
        INNER JOIN #tempTKho t2 ON t1.BarCodeGoc = t2.BarCode
        WHERE CONVERT(DATE, t1.NgayXuatHang) < @StartDateRange AND ISNULL(MaHang,'') NOT LIKE '%PSH_%';

        SELECT @TonThuDK = SUM(ThuHoi)
        FROM PhieuThuHoiNPL t1
        LEFT JOIN PhieuXuatHang t2 ON t1.BarCode = t2.BarCode
        WHERE CONVERT(DATE, t1.NgayTH) < @StartDateRange
          AND EXISTS (SELECT 1 FROM #tempTKho t3 WHERE t2.BarCodeGoc = t3.BarCode);

        SET @TonDauKy = ISNULL(@TonNhapDK, 0) - ISNULL(@TonXuatDK, 0) + ISNULL(@TonThuDK, 0) - ISNULL(@TonChenhDK, 0);

        -- 3. TRONG KỲ THEO NGÀY
        IF OBJECT_ID('tempdb..#tempChenhLechTK') IS NOT NULL DROP TABLE #tempChenhLechTK;
        SELECT CONVERT(DATE, DateDuyetKK) AS Ngay, ROUND(SUM(ISNULL(SLKiemKeBanDau,0) - ISNULL(SLKiemKeEdit,0)), 4) AS SLChenhLenhTK
        INTO #tempChenhLechTK FROM ERPPhieuKiemKe_NPL t1
        WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL AND (CONVERT(DATE, DateDuyetKK) BETWEEN @StartDateRange AND @EndDateRange)
          AND EXISTS (SELECT 1 FROM #tempTKho t2 WHERE t1.BarCode = t2.BarCode)
        GROUP BY CONVERT(DATE, DateDuyetKK);

        IF OBJECT_ID('tempdb..#tempNhapKhoTK') IS NOT NULL DROP TABLE #tempNhapKhoTK;
        SELECT CONVERT(DATE, t1.NgayNhapKho) AS Ngay, SUM(CAST(t1.SoLuongThucTeBanDau AS DECIMAL(18,4))) AS SLNhapTK
        INTO #tempNhapKhoTK FROM ERP_ChiTietNhapKhoNPL t1
        INNER JOIN #tempNhapKhoNPL t2 ON t1.SoLoID = t2.SoLoID AND t1.IsNPL = t2.IsNPL
        INNER JOIN #tempTKho t4 ON t1.ID = t4.ID
        WHERE (CONVERT(DATE, t1.NgayNhapKho) BETWEEN @StartDateRange AND @EndDateRange)
        GROUP BY CONVERT(DATE, t1.NgayNhapKho);

        IF OBJECT_ID('tempdb..#tempXuatHangTK') IS NOT NULL DROP TABLE #tempXuatHangTK;
        SELECT CONVERT(DATE, t1.NgayXuatHang) AS Ngay, SUM(SLNhap) AS SLXuatTK
        INTO #tempXuatHangTK FROM PhieuXuatHang t1
        INNER JOIN #tempTKho t2 ON t1.BarCodeGoc = t2.BarCode
        WHERE (CONVERT(DATE, t1.NgayXuatHang) BETWEEN @StartDateRange AND @EndDateRange) AND ISNULL(MaHang,'') NOT LIKE '%PSH_%'
        GROUP BY CONVERT(DATE, t1.NgayXuatHang);

        IF OBJECT_ID('tempdb..#tempThuHoiTK') IS NOT NULL DROP TABLE #tempThuHoiTK;
        SELECT CONVERT(DATE, t1.NgayTH) AS Ngay, SUM(ThuHoi) AS SLThuTK
        INTO #tempThuHoiTK FROM PhieuThuHoiNPL t1
        LEFT JOIN PhieuXuatHang t2 ON t1.BarCode = t2.BarCode
        WHERE (CONVERT(DATE, t1.NgayTH) BETWEEN @StartDateRange AND @EndDateRange)
          AND EXISTS (SELECT 1 FROM #tempTKho t3 WHERE t2.BarCodeGoc = t3.BarCode)
        GROUP BY CONVERT(DATE, t1.NgayTH);

        -- 4. TỔNG HỢP RA BIỂU ĐỒ
        ;WITH Days AS (
            SELECT TOP (@TotalDaysRange)
                CAST(DATEADD(DAY, number, @StartDateRange) AS DATE) AS Ngay
            FROM master..spt_values
            WHERE type = 'P' AND number BETWEEN 0 AND 399
        ),
        DailyFlow AS (
            SELECT 
                d.Ngay,
                ISNULL(n.SLNhapTK, 0) AS TotalIn,
                ISNULL(x.SLXuatTK, 0) AS TotalOut,
                ISNULL(th.SLThuTK, 0) AS TotalRecover,
                ISNULL(c.SLChenhLenhTK, 0) AS TotalDiscrepancy
            FROM Days d
            LEFT JOIN #tempNhapKhoTK n ON d.Ngay = n.Ngay
            LEFT JOIN #tempXuatHangTK x ON d.Ngay = x.Ngay
            LEFT JOIN #tempThuHoiTK th ON d.Ngay = th.Ngay
            LEFT JOIN #tempChenhLechTK c ON d.Ngay = c.Ngay
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
        DROP TABLE #tblVTLoi;
        DROP TABLE #tblKV;
        DROP TABLE #tblBarcodeHopLe;
        DROP TABLE #tempTKho;
        DROP TABLE #tempChenhLechTK;
        DROP TABLE #tempNhapKhoTK;
        DROP TABLE #tempXuatHangTK;
        DROP TABLE #tempThuHoiTK;
    END
