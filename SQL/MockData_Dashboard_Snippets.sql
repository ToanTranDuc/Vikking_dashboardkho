USE [PMS_QLDH_VIKING_2025];
GO

DECLARE @Action NVARCHAR(50) = 'GetDayDetail'; -- Đổi thành 'GetChuanBiXuat' hoặc 'GetTodoDetail' để test các phần khác
DECLARE @Ngay DATETIME = GETDATE();
DECLARE @Itemcode NVARCHAR(200) = 'itemcode_cho_nk';
DECLARE @TuNgay DATETIME = NULL;
DECLARE @DenNgay DATETIME = NULL;

-- 1. Dành cho phần: HOẠT ĐỘNG KHO TRONG NGÀY (GetDayDetail)
IF @Action = 'GetDayDetail'
BEGIN
    -- Result 1: Phân công công việc (Assignments)
    SELECT
        ''                         AS MaLenhSX,      -- Mã lệnh sản xuất (VD: SX_001). JS dùng in đậm ở góc trên.
        ''                         AS MaKhachHang,   -- Mã khách hàng.
        ''                         AS TenBrand,      -- Tên thương hiệu (Nike...). JS hiển thị kế bên Mã lệnh.
        0                          AS CoCanhBao,     -- Dạng Số (0/1). 1 -> JS hiển thị chuông cảnh báo trễ tiến độ. (Hiện tại gán 0 để ẩn)
        0                          AS ThieuNPL,      -- Dạng Số (0/1). 1 -> JS hiển thị chữ/icon 'Thiếu NPL'. (Hiện tại gán 0 để ẩn)
        ''                         AS MaNV,          -- Mã nhân viên phụ trách.
        ''                         AS TenNV,         -- Tên nhân viên. JS render kèm icon avatar user.
        0                          AS TrangThai,     -- Dạng Số. Quy ước: 0=Chờ, 1=Đang, 2=Xong, 3=Trễ. JS dùng ttBadge() sinh màu tương ứng. Cần if (0) return '' để ẩn.
        ''                         AS GhiChu,        -- Nội dung chú thích/ghi chú của task.
        ''                         AS NgayThucHien,  -- Dạng chuỗi ngày yyyy-MM-dd.
        ''                         AS GioThucHien,   -- Dạng chuỗi giờ.
        ''                         AS MoTaCongViec   -- Nội dung công việc.
    FROM dbo.ERP_LichPhanCongPhuLieu_Task t
    -- WHERE CONVERT(DATE, t.NgayThucHien) = CONVERT(DATE, @Ngay);

    -- Result 2: Phụ liệu - Soạn hàng (Pick Orders)
    SELECT
        ''                         AS MaLenhSX,      -- Mã lệnh sản xuất.
        ''                         AS MaKhachHang,   -- Mã khách hàng.
        ''                         AS TenBrand,      -- Tên thương hiệu.
        0                          AS TrangThai,     -- Dạng Số. Trạng thái soạn hàng. JS gọi ttBadge() để tô màu.
        0                          AS SoPLThieu,     -- Dạng Số. Lượng phụ liệu đang thiếu.
        ''                         AS TenNV,         -- Tên NV thực hiện.
        ''                         AS MaNV,          -- Mã NV.
        ''                         AS NgaySoan,      -- Dạng chuỗi ngày.
        ''                         AS GioSoan,       -- Dạng chuỗi giờ.
        0                          AS SoLoaiPL,      -- Dạng Số. Số loại phụ liệu.
        0                          AS TongSLCanSoan, -- Dạng Số. JS thường gộp lại hiển thị: "SL: x / y".
        ''                         AS GhiChu         -- Ghi chú.
    FROM dbo.ERP_LichPhanCongPhuLieu_Task t
    -- WHERE CONVERT(DATE, t.NgayThucHien) = CONVERT(DATE, @Ngay);
    RETURN;
END


-- 2. Dành cho phần: CHUẨN BỊ XUẤT (GetChuanBiXuat)
IF @Action = 'GetChuanBiXuat'
BEGIN
    SELECT
        '' AS MaLenhSanXuat, -- Mã lệnh SX
        '' AS MaLenh,        -- Mã lệnh thu gọn
        '' AS MaDVSX,        -- Mã đơn vị sản xuất
        '' AS KhachHang,     -- Tên khách hàng (JS hiển thị trên thẻ giao việc)
        '' AS TenHang,       -- Tên hàng hóa sản xuất
        0  AS SoLuongYeuCau, -- Dạng số. Tổng số lượng yêu cầu (đã gán 0 để ẩn số liệu thật).
        MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END)                       AS KHCat,     -- Ngày KH Cắt (Datetime).
        DATEADD(day, 7, MAX(CASE WHEN tp.StepCode = 'TTCat' THEN tp.kh_date END))      AS DuKienCat  -- Ngày dự kiến (Datetime).
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


-- 3. Dành cho phần: CẢNH BÁO / TASK PENDING (GetTodoDetail)
IF @Action = 'GetTodoDetail'
BEGIN
    DECLARE @TodoType NVARCHAR(50) = ISNULL(@Itemcode, 'itemcode_cho_nk');

    IF @TodoType = 'itemcode_cho_nk'
    BEGIN
        SELECT
            ROW_NUMBER() OVER (ORDER BY t1.SoLoID, t1.MaNPL) AS STT, -- Số thứ tự
            '' AS ItemCode,  -- Mã Item
            '' AS TenVT,     -- Tên chi tiết vật tư
            '' AS POMua,     -- Số PO (Purchase Order)
            '' AS MaMauVT,   -- Mã màu (Chuỗi)
            '' AS MauVT,     -- Tên màu (Chuỗi)
            '' AS WidthSize, -- Khổ vải/Kích cỡ (Chuỗi)
            '' AS NgayTao,   -- Dạng chuỗi ngày. Ngày nhập kho.
            '' AS NCC,       -- Nhà cung cấp
            0  AS SLMua,     -- Dạng số. SL Mua (gán 0 để ẩn)
            0  AS SLVe,      -- Dạng số. SL Về (gán 0 để ẩn)
            'itemcode_cho_nk' AS Type -- Loại dữ liệu để phân biệt các Tab trên JS
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
        WHERE ISNULL(t1.IsDuyetNK, 0) <> 1;
    END
END
