/// API
const API = {
    async Get(action, para = {}, textSuccess = '') {
        const paraConvert = Object.entries(para || {})
            .map(([key, value]) => `${key}=${value}`)
            .join("&");

        const url = `/api/capthemngoaidh/get?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error(`Response status: ${response.status}`);

            const data = await response.json();
            if (textSuccess) showToast("success", textSuccess);
            return data;
        } catch (error) {
            console.error(error);
        }
    },

    async Post(router = 'Post', action, arrSave, textSuccess = "Lưu thành công!", para = {}) {
        const paraConvert = Object.entries(para || {})
            .map(([key, value]) => `${key}=${value}`)
            .join("&");

        const url = `/api/capthemngoaidh/${router}?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(arrSave),
            });

            if (!response.ok) throw new Error(`Response status: ${response.status}`);

            const data = await response.json();
            if (data == "True") showToast('success', textSuccess, 1500);
        } catch (error) {
            console.error(error);
        }
    }
};

/// API CALLS
async function GetMaLenhDangKyVatTu() {
    const maKH = $("#khachhang").val() || "all";
    const $maLenhSelect = $("#malenhdangkyvattu");

    // Destroy select2 trước khi khởi tạo lại
    if ($maLenhSelect.hasClass("select2-hidden-accessible")) {
        $maLenhSelect.select2("destroy");
    }

    $maLenhSelect.empty().append(`<option ></option>`);

    const data = await API.Get("getlenhcapthem", { param1: isNPL, param2: maKH });
    lenhDangKyVatTuOptions = Array.isArray(data) ? data : [];
    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaLenhSanXuat}">${x.Display}</option>`).join('');
        $maLenhSelect.append(html);
    }

    $maLenhSelect.select2({
        dropdownParent: $("#modalAddPhieu")
    });
}

/// check1
async function GetPhieuDangKyXuat() {
    const maLenh = $("#malenhdangkyvattu").val();
    const data = await API.Get("GetPhieuDKTHLenh", { param1: isNPL, param2: maLenh });
    batchData = data;
    renderList(data);
}

async function GetChiTietLenhThuHoi() {
    const maLenh = $("#malenhdangkyvattu").val();

    const data = await API.Get("chitietlenhthuhoi", { param1: isNPL, param2: maLenh, param3: 1 })
    const maDH = Array.isArray(data) && data.length > 0
        ? ((data.find(x => (x.MaDH || "").toString().trim() !== "") || data[0]).MaDH || "")
        : "";
    $("#maDHNhap").val(maDH || "");
    createViewDxDataGridDangKyVatTu(data)
}

async function GetMaLenh2() {
    const $maLenhSelect = $("#malenh");
    const data = await API.Get("GetPYCDNTH");
    $maLenhSelect.empty();

    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaLenhSX}">${x.MaLenh}</option>`).join('');
        $maLenhSelect.append(html);
    }

    $maLenhSelect.select2();
    /*   await GetPhieu();*/
    await GetPhieu(false);
}

async function GetDSPhieuDNTHVT() {
    const data = await API.Get("GetViewPDKVT", { param1: 'all', param2: isNPL })

    lstDSPhieuDKVT = data
    updateGrid(dxDataPhieuDKVT, data)
}

async function GetLenhChiTiet(maLenhSanXuat) {
    return await API.Get("GetChiTietLenh", { param1: isNPL, param2: maLenhSanXuat });
}

async function GetKhachHang() {
    const data = await API.Get("GetMaKH");
    const $khachHangSelect = $("#khachhang");

    if ($khachHangSelect.data('select2')) $khachHangSelect.select2('destroy');

    $khachHangSelect.empty().append(`<option value="all">Tất cả</option>`);

    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaKH}">${x.TenKH}</option>`).join('');
        $khachHangSelect.append(html);
    }

    $khachHangSelect.select2({ dropdownParent: $('#modalAddPhieu') });
}

async function GetPhieuMax() {
    const data = await API.Get("getmax_ycth", { param1: isNPL });
    dot = data[0].PCT_NDH;
    $("#txtPhieu").val(`PDNCT_BN_${dot}`);
}

//async function GetPhieu() {
//    const maLenhSanXuat = $("#malenh").val();
//    const data = await API.Get("GetPhieuDNTH");
//    const $soPhieu = $("#maphieu");

//    if ($soPhieu.hasClass("select2-hidden-accessible")) {
//        $soPhieu.select2("destroy");
//    }
//    $soPhieu.empty();

//    if (data.length > 0) {
//        const html = data.map(x => `<option data-display="${x.Display}" value="${x.PhieuCT_NgoaiDH}">${x.Display}</option>`).join('');
//        selectSoPhieuDisplay = data[0].Display
//        $soPhieu.append(html);
//    }

//    $soPhieu.select2();

//    await GetDSPhieuDNTH();
//}
async function GetPhieu(autoLoadDS = true) {
    const data = await API.Get("GetPhieuDNTH");
    const $soPhieu = $("#maphieu");

    if ($soPhieu.hasClass("select2-hidden-accessible")) {
        $soPhieu.select2("destroy");
    }
    $soPhieu.empty();

    if (data && data.length > 0) {
        const html = data.map(x =>
            `<option data-display="${x.Display}" value="${x.PhieuCT_NgoaiDH}">${x.Display}</option>`
        ).join('');
        selectSoPhieuDisplay = data[0].Display;
        $soPhieu.append(html);
    }

    $soPhieu.select2();

    if (autoLoadDS) {
        await GetDSPhieuDNTH();
    }
}
//async function GetDSPhieuDNTH() {
//    const soPhieu = $("#maphieu option:selected").val() || '';
//    const data = await API.Get("GetDSChiTietPhieu", { param1: soPhieu });
//    lstDataDangKyVatTu = data;
//    updateGrid(dxDataGridDanhSachDangKy, data);
//}
async function GetDSPhieuDNTH() {
    const grid = dxDataGridDanhSachDangKy;
    grid.beginCustomLoading("Đang tải...");

    try {
        const soPhieu = $("#maphieu option:selected").val() || 'all';
        const isAll = soPhieu === 'all';

        grid.beginUpdate();
        grid.columnOption("PhieuCT_NgoaiDH", "groupIndex", isAll ? 0 : undefined);
        grid.endUpdate();

        const action = isAll ? "GetDSChiTietPhieuFast" : "GetDSChiTietPhieu";
        const data = await API.Get(action, { param1: soPhieu });

        const snapshot = Array.isArray(data) ? data : [];
        lstDataDangKyVatTu = snapshot;

        updateGrid(grid, snapshot);
        filterByTrangThai();
        loadPhongBanOptions();
        const maNPLList = [...new Set(snapshot.map(x => x.MaNPL).filter(Boolean))];
        if (maNPLList.length > 0) {
            const tonKhoPromise = fetchTonKhoMap(maNPLList);
            applyTonKhoLazy(snapshot, tonKhoPromise);
        }

    } catch (err) {
        console.error("GetDSPhieuDNTH error:", err);
        showToast("error", "Lỗi khi tải dữ liệu phiếu");
    } finally {
        grid.endCustomLoading();
    }
}
//async function fetchTonKhoMap(maNPLList) {
//    const BATCH_SIZE = 50;
//    const batches = [];
//    for (let i = 0; i < maNPLList.length; i += BATCH_SIZE) {
//        batches.push(maNPLList.slice(i, i + BATCH_SIZE));
//    }
//    const results = await Promise.all(
//        batches.map(b => API.Get("GetTonKhoByNPL", { param1: b.join(';') }))
//    );
//    const map = {};
//    results.flat().forEach(x => { if (x?.MaNPL) map[x.MaNPL] = x; });
//    return map;
//}
async function fetchTonKhoMap(maNPLList) {
    if (!maNPLList?.length) return {};

    const BATCH_SIZE = 100;
    const batches = [];
    for (let i = 0; i < maNPLList.length; i += BATCH_SIZE) {
        batches.push(maNPLList.slice(i, i + BATCH_SIZE));
    }

    const results = await Promise.allSettled(
        batches.map(b =>
            fetch('/api/capthemngoaidh/PostGetTonKho', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ maNPLList: b })
            }).then(r => r.json())
        )
    );

    const map = {};
    results.forEach(r => {
        if (r.status === 'fulfilled' && Array.isArray(r.value)) {
            r.value.forEach(x => { if (x?.MaNPL) map[x.MaNPL] = x; });
        }
    });
    return map;
}
async function applyTonKhoLazy(snapshot, tonKhoPromise) {

    const snapshotId = snapshot.length > 0
        ? (snapshot[0]?.PhieuCT_NgoaiDH + '_' + snapshot.length)
        : 'empty';

    setTimeout(() => {
        dxDataGridDanhSachDangKy.columnOption("TonKho", "caption", "Tồn kho ⏳");
    }, 10);

    try {
        const tonKhoMap = await tonKhoPromise;

        const currentId = lstDataDangKyVatTu.length > 0
            ? (lstDataDangKyVatTu[0]?.PhieuCT_NgoaiDH + '_' + lstDataDangKyVatTu.length)
            : 'empty';

        if (currentId !== snapshotId) {
            console.log("Snapshot đã thay đổi, bỏ qua apply TonKho");
            return;
        }

        lstDataDangKyVatTu.forEach(item => {
            if (tonKhoMap[item.MaNPL] !== undefined) {
                item.TonKho = tonKhoMap[item.MaNPL]?.TonKho ?? null;
            }
        });

        dxDataGridDanhSachDangKy.refresh();

    } catch (err) {
        console.error("applyTonKhoLazy error:", err);
    } finally {
        setTimeout(() => {
            dxDataGridDanhSachDangKy.columnOption("TonKho", "caption", "SL Tồn kho");
        }, 10);
    }
}
async function SavePhieu(arrSave) {
    await API.Post("PostDNCT", "postcapthemngoaidh", arrSave, "Lưu phiếu thành công");
    const phieuVuaTao = `PDNCT_BN_${dot}`;

    resetModalState();

    await GetMaLenh2();
    await GetPhieuMax();

    selectSoPhieu = phieuVuaTao;
    $("#maphieu").val(phieuVuaTao).trigger("change");
    $("#modalAddPhieu").modal("hide")
}
async function UpdateKiTenPhieu(arrSave) {
    await API.Post("PostDNCT", "postcapthemngoaidh", arrSave, "Lưu phiếu thành công");
    await GetDSPhieuDNTH();
}
async function DeletePhieuDNTH() {
    await API.Get("DeletePhieuDNTH", {
        param1: selectSoPhieu,
        param2: MaNPL
    }, "Xóa thành công");

    await GetDSPhieuDNTH();
    await GetMaLenh2();

    if (lstDataDangKyVatTu.length === 0) {
        await resetToDefault();
    }

    $("#modalComfimrtDeleteVT").modal('hide');
}

async function DeleteAllPhieu() {
    let res = await API.Get("DeletePhieuDNTH", { param1: selectSoPhieu });

    if (res && res[0].SoDongDaXoa > 0) {
        showToast("success", "Xóa phiếu thành công");

        await resetToDefault();
        chiTietLenhCache = {};

        $("#modalComfimrtDeleteVT").modal('hide');
    } else {
        showToast("error", res[0].Message || "Không thể xóa phiếu");
    }
}
async function UpdatePhieu() {
    const dataSource = dxDataGridEditVatTu.option("dataSource");
    const arrSave = [];
    if (dataSource.length === 0) {
        showToast("warning", "Không có dữ liệu để cập nhật");
        PlayAudioError();
        return;
    }

    const ngayThuHoi = ddmmyyyyToYmd($("#ngayCapEdit").val());
    dataSource.map(item => {

        const objectUpdate = {
            PhieuCT_NgoaiDH: item.PhieuCT_NgoaiDH,
            MaLenhSX: item.MaLenhSX,
            MaDH: item.MaDH,
            MaLenh: item.MaLenh,
            MaNPL: item.MaNPL,
            SLDK: item.SLDK,
            GhiChu: item.GhiChu,
            MaNhom: item.MaNhom,
            MaVTID: item.MaVTID,
            MauVTID: item.MauVTID,
            KhoVaiID: item.KhoVaiID,
            MaVT: item.MaVT,
            MauVT: item.MauVT,
            KhoVai: item.KhoVai,
            MaDVVT: item.MaDVVT,
            NgayDK: item.NgayDK,
            NgayTH: ngayThuHoi,
            NgayTao: new Date(),
            IsNPL: isNPL,
            NguoiTH: userNameSave,
            PhieuDK: "",
            SignNgDK: "",
            NgayKi: "",
            SignTBPNgDK: "",
            NgayKiTBPNgDK: "",
            SignMer: "",
            NgaySignMer: "",
            SignTBPMer: "",
            NgaySignTBPMer: "",
            IsTV: item.IsTV,
            MaNhomChiTiet: item.ChungLoaiChiTiet,
            PhatSinhChiPhi: item.PhatSinhChiPhi ? 1 : 0,
            LiDo: item.LiDo
        }

        arrSave.push(objectUpdate)
    })
    await API.Post("PostDNCT", "postcapthemngoaidh", arrSave, "Cập nhật thành công");
    $('#modalEditDKVT').modal('hide');
    await GetDSPhieuDNTH();

    // trả về lại null
    picker4.dates.setValue('');
}

async function setAutoMaDHByMaLenh() {
    const maLenh = $("#malenhdangkyvattu").val();
    if (!maLenh) {
        $("#maDHNhap").val("");
        return;
    }

    let maDH = "";

    const selectedLenh = lenhDangKyVatTuOptions.find(x => x.MaLenhSanXuat == maLenh);
    if (selectedLenh) {
        maDH = selectedLenh.MaDH || selectedLenh.MDH || selectedLenh.MaDonHang || "";
    }

    if (!maDH) {
        try {
            const response = await fetch(`/api/DeNghiCapThemNPL/GetMaDHByLenh?maLenhSX=${encodeURIComponent(maLenh)}&isNPL=${isNPL}`);
            if (response.ok) {
                const payload = await response.json();
                maDH = payload?.MaDH || "";
            }
        } catch (error) {
            console.error(error);
        }
    }

    $("#maDHNhap").val(maDH || "");
}

async function SavePhieuThuVien(arrTV, signatureImage) {

    const payload = arrTV.flatMap(item => {
        const mauList = Array.isArray(item.MauSP) && item.MauSP.length > 0
            ? item.MauSP.map(v => {
                // v có thể là option object {value, text} hoặc string JSON hoặc object thẳng
                const inner = parseJsonSafe(v.MaMau); // parse cái string lồng bên trong
                return {
                    MaMauSP: inner?.MaMau ?? v.MaMau ?? null,
                    TenMauSP: inner?.TenMau ?? v.TenMau ?? null
                };
            })
            : (() => {
                const raw = (typeof item.MauSP === "object" && item.MauSP?.value !== undefined)
                    ? item.MauSP.value
                    : item.MauSP;
                const obj = parseJsonSafe(raw);
                return [{
                    MaMauSP: obj?.MaMau ?? null,
                    TenMauSP: obj?.TenMau ?? null
                }];
            })();
        const sizeList = Array.isArray(item.SizeInfo) && item.SizeInfo.length > 0
            ? item.SizeInfo.map(v => {
                const obj = typeof v === "object" ? v : (() => { try { return JSON.parse(v); } catch { return {}; } })();
                return {
                    MaNhomSize: obj.MaNhomSize ?? null,
                    NhomSize: obj.NhomSize ?? obj.TenNhomSize ?? null,
                    MaSize: obj.MaSize ?? null,
                    TenSize: obj.TenSize ?? null
                };
            })
            : [{
                MaNhomSize: item.MaNhomSize ?? null,
                NhomSize: item.NhomSize ?? null,
                MaSize: item.MaSize ?? null,
                TenSize: item.TenSize ?? null
            }];

        return mauList.flatMap(mau =>
            sizeList.map(size => ({
                PhieuTH: item.PhieuTH ?? "",
                Dot: item.Dot ?? null,
                MaLenhSX: item.MaLenhSX ?? "",
                MaDH: item.MaDH ?? "",
                MaLenh: item.MaLenh ?? "",
                MaNPL: item.MaNPL ?? "",
                MaNhom: item.MaNhom ?? "",
                MaVTID: item.MaVTID ?? "",
                MauVTID: item.MauVTID ?? "",
                KhoVaiID: item.KhoVaiID ?? "",
                IsTV: item.IsTV ?? 0,
                MaMauSP: mau.MaMauSP,
                TenMauSP: mau.TenMauSP,
                MaNhomSize: size.MaNhomSize,
                NhomSize: size.NhomSize,
                MaSize: size.MaSize,
                TenSize: size.TenSize,
                SoLuong: item.SoLuong
            }))
        );
    });
    console.log()
    await API.Post("postcapthem_chitiet_ngoaidh", "postcapthem_chitiet_ngoaidh", payload, "", { param1: isNPL });

}

async function loadChungLoai() {
    const maCLVT = $("#chungloaiSelect").val();

    if (!maCLVT) {
        if (dxDataGridThuVien) {
            dxDataGridThuVien.option("dataSource", []);
            dxDataGridThuVien.refresh();
        }
        return;
    }

    await loadSoLuong();
    const data = await API.Get("GETTHUVIEN", { param1: maCLVT });
    cacheSoLuong = dataSoLuong || [];

    if (data.length > 0) {
        initFloatingTagBox();
        if (dxDataGridThuVien) {
            dxDataGridThuVien.option("dataSource", data);
            dxDataGridThuVien.refresh();
        } else {
            createViewdxDataGridThuVien(data);
        }
    } else if (dxDataGridThuVien) {
        dxDataGridThuVien.option("dataSource", []);
        dxDataGridThuVien.refresh();
    }
}

async function GetChungLoaiSelect() {

    const $maLenhSelect = $("#chungloaiSelect");

    // Destroy select2 trước khi khởi tạo lại
    if ($maLenhSelect.hasClass("select2-hidden-accessible")) {
        $maLenhSelect.select2("destroy");
    }

    $maLenhSelect.empty().append(`<option ></option>`);

    if (!Array.isArray(chungLoaiThuVienCache) || chungLoaiThuVienCache.length === 0) {
        const data = await API.Get("chungloai");
        chungLoaiThuVienCache = Array.isArray(data) ? data : [];
    }
    const data = chungLoaiThuVienCache;
    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaCLVT}">${x.ChungLoaiVatTu}</option>`).join('');
        $maLenhSelect.append(html);
    }

    $maLenhSelect.select2({
        dropdownParent: $("#modalThuVien")
    });
}
async function showModalThuVien() {
    resetThuVienModalState();
    await GetChungLoaiSelect();
    $("#modalThuVien").modal("show");
}

async function PostCanDoiDinhMucNPL() {
    const dataSource = dxDataGridDanhSachDangKy.option("dataSource");
    const arrSave = [];
    if (dataSource.length === 0) {
        showToast("warning", "Không có dữ li");
        PlayAudioError();
        return;
    }


    dataSource.map(item => {

        const objectUpdate = {
            PhieuCT_NgoaiDH: item.PhieuCT_NgoaiDH,
            MaLenhSX: item.MaLenhSX,
            MaDH: item.MaDH,
            MaLenh: item.MaLenh,
            MaNPL: item.MaNPL,
            SLDK: item.SLDK,
            GhiChu: item.GhiChu,
            MaNhom: item.MaNhom,
            MaVTID: item.MaVTID,
            MauVTID: item.MauVTID,
            KhoVaiID: item.KhoVaiID,
            MaVT: item.MaVT,
            MauVT: item.MauVT,
            KhoVai: item.KhoVai,
            MaDVVT: item.MaDVVT,
            NgayDK: item.NgayDK,
            NgayTH: item.NgayTH,
            NgayTao: new Date(),
            IsNPL: isNPL,
            NguoiTH: userNameSave,
            PhieuDK: "",
            SignNgDK: "",
            NgayKi: "",
            SignTBPNgDK: "",
            NgayKiTBPNgDK: "",
            SignMer: "",
            NgaySignMer: "",
            SignTBPMer: "",
            NgaySignTBPMer: "",
            IsTV: item.IsTV,
            LiDo: item.LiDo
        }

        arrSave.push(objectUpdate)
    })
    await API.Post("PostDNCT", "PostCanDoiDinhMucNPL", arrSave, "Cập nhật thành công");
}

function GetChungLoaiChiTiet(maCLVT) {
    let result = [];

    $.ajax({
        url: `/api/DeNghiCapThemNPL/Get?action=GETCLCT_2&param1=${maCLVT}`,
        method: "GET",
        async: false, // <--- Quan trọng: Chuyển sang đồng bộ
        success: function (data) {
            if (data && data.length > 0) {
                result = data.map(item => ({
                    value: JSON.stringify({ MaCLCT: item.MaNhom, TenCLCT: item.TenNhom }),
                    text: item.TenNhom,
                    original: item
                }));
            }
        },
        error: function (err) {
            console.error("Lỗi API GetChungLoaiChiTiet:", err);
        }
    });

    return result;
}

async function loadSoLuong(forceReload = false) {
    const maLenh = $("#malenhdangkyvattu").val();

    if (!maLenh) {
        dataSoLuong = [];
        soLuongLoadedMaLenh = "";
        return;
    }

    if (!forceReload && soLuongLoadedMaLenh === maLenh && Array.isArray(dataSoLuong) && dataSoLuong.length > 0) {
        return;
    }

    dataSoLuong = await API.Get("GETSOLUONG", { param1: maLenh });
    soLuongLoadedMaLenh = maLenh;
}

function sendNotify(UserID, ModuleID, title, detail, sendTo, BoPhan = "ALL", Status = -1) {

    const url = `/api/SendToNotification/PushNotification?` +
        `UserIDTao=${encodeURIComponent(UserID)}&` +
        `ModuleID=${encodeURIComponent(ModuleID)}&` +
        `Title=${encodeURIComponent(title)}&` +
        `Detail=${encodeURIComponent(detail)}&` +
        `SendTo=${encodeURIComponent(sendTo)}&` +
        `BoPhan=${encodeURIComponent(BoPhan)}&` +
        `Status=${Status}`
        ;

    $.ajax({
        url: url,
        type: "POST",
        contentType: false,
        processData: false,
        success: function (result) {
            console.log("Gửi thông báo thành công:", result);
        },
        error: function (xhr, status, error) {
            console.error("Lỗi gửi thông báo:", error);
        }
    });
}

async function DeleteItemInPhieu(phieu, maNPL) {
    const res = await API.Get("delete_iteminphieu", {
        param1: phieu,
        param2: maNPL
    });

    if (res && res[0].SoDongDaXoa > 0) {
        showToast("success", "Xóa thành công");
        await GetDSPhieuDNTH();
        if (!lstDataDangKyVatTu || lstDataDangKyVatTu.length === 0) {
            await resetToDefault();
            updateGrid(dxDataGridDanhSachDangKy, []);
        }
    } else {
        showToast("error", res[0].Message);
    }
}