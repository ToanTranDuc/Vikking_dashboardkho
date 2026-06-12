var userNameSave = localStorage.getItem("username1")
var dxDataGhiNhanKiemKe;
var rowComfirmtSLKK = {}
var lstRowComfirmtSLKK = []
var lstDataChiTietGhiNhanKiemKe = []
var dataChiTietKiemKeOriginal = []; // Lưu data gốc
var lstRowDeleteKK = [];
/// Event
$(document).ready(function () {
    createViewDxGhiNhanKiemKe();
    createViewDxGridDanhSachItemCode();
    GetPhieuDanhSachVatTuKiemKe();
    createViewDxGridChiTiet([]);
});

$(function () {
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })

    $(".select_2").select2();

    $("#btnShowSaveSLKK").on("click", function (e) {
        if (lstRowComfirmtSLKK.length <= 0) {
            showToast("warning", "Vui lòng chọn vật tư để xác nhận");
            return;
        }

        // Tạo HTML cho danh sách các item đã chọn
        let htmlList = "";
        lstRowComfirmtSLKK.forEach((item, index) => {
            const slThayDoi = item.SLKiemKeEdit || item.SLKiemKe || 0;
            const slGoc = item.SLNK || 0;
            const isChanged = slThayDoi !== slGoc;

            htmlList += `
            <div class="info-group mb-2">
                <div class="info-row" style="display: flex; justify-content: space-between; flex-wrap: wrap;">
                    <span style="flex: 2; min-width: 150px;">
                        <span class="label">Item Code:</span>
                        <span class="value">${item.MaVT || "-"}</span>
                    </span>
                    <span style="flex: 1
                    ; min-width: 120px;">
                        <span class="label">Màu:</span>
                        <span class="value">${item.MauVT || "-"}</span>
                    </span>
                    <span style="flex: 1; min-width: 120px;">
                        <span class="label">Khổ vải:</span>
                        <span class="value">${item.KhoVai || "-"}</span>
                    </span>
                </div>
                <div class="info-row ${isChanged ? 'highlight' : 'highlight-green'}" style="margin-top: 5px;">
                    <span class="label">Số lượng kiểm kê thay đổi:</span>
                    <span class="value" style="color: ${isChanged ? '#e74c3c' : '#28a745'}; font-weight: bold;">
                        ${slGoc} → ${slThayDoi}
                    </span>
                </div>
            </div>
        ${index < lstRowComfirmtSLKK.length - 1 ? '<hr style="margin: 10px 0;">' : ''}
    `;
        });

        // Thay thế nội dung modal body
        $("#modalConfirmSLKK .modal-body").html(`
        <p class="mb-3">
                <strong>Bạn muốn xác nhận ${lstRowComfirmtSLKK.length} vật tư sau:</strong>
            </p>
            <div style="max-height: 400px; overflow-y: auto;">
                ${htmlList}
         </div>
    `);

        $("#modalConfirmSLKK").modal("show");
    });

    // Khi click nút "Xác nhận tất cả" - Mở modal xác nhận
    $("#btnSaveAllSLKK").on("click", async function (e) {
        if (!lstDataChiTietGhiNhanKiemKe || lstDataChiTietGhiNhanKiemKe.length === 0) {
            showToast("warning", "Không có vật tư cần xác nhận!");
            return;
        }

        const isCompleted = lstDataChiTietGhiNhanKiemKe.every(item =>
            item.IsXacNhan === 1 || item.IsLog === 0
        );


        if (isCompleted) {
            showToast("warning", "Phiếu này đã hoàn tất kiểm kê");
            return;
        }


        const lstFilterEqual0 = lstDataChiTietGhiNhanKiemKe.filter(item => item.SLKiemKe == 0)

        if (lstFilterEqual0.length > 0) {
            showToast("warning", "Có vật tư chưa được kiểm kê")
            return;
        }


        // Hiển thị số lượng trong modal
        $("#confirmAllTotalItems").text(lstDataChiTietGhiNhanKiemKe.length);

        // Tính tổng số dòng chi tiết (có thể tính async nếu cần chính xác)
        let totalDetails = 0;
        for (const row of lstDataChiTietGhiNhanKiemKe) {
            const dataChiTiet = await loadDataChiTietKiemKe(row.MaNPL);
            totalDetails += dataChiTiet.length;
        }
        $("#confirmAllTotalDetails").text(totalDetails);


        // Mở modal xác nhận
        $("#modalConfirmAll").modal("show");
    });

    // Xử lý khi click nút "Xác nhận" trong modal
    $("#btnConfirmAllSave").on("click", async function (e) {
        const url = `/api/PhieuXuatHangNPL/PostChiTietKiemKeNPL?action=UpdateXacNhan`;

        try {
            // Disable nút để tránh click nhiều lần
            $(this).prop("disabled", true).html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...');

            // Tạo danh sách cần xác nhận từ lstDataChiTietGhiNhanKiemKe
            const lstKiemKeXacNhan = [];
            for (const row of lstDataChiTietGhiNhanKiemKe) {
                const dataChiTiet = await loadDataChiTietKiemKe(row.MaNPL);
                const itemsToConfirm = dataChiTiet.map(i => ({
                    ...i,
                    SLKiemKeEdit: i.SLKiemKeEdit ?? i.SLKiemKe,
                    GhiChu: i.GhiChu ?? "",
                    IsXacNhan: 1
                }));
                lstKiemKeXacNhan.push(...itemsToConfirm);
            }

            // POST tất cả
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(lstKiemKeXacNhan)
            });

            const result = await response.json();

            if (result == "True") {
                showToast("success", `Đã xác nhận ${lstDataChiTietGhiNhanKiemKe.length} vật tư thành công!`);
                $("#modalConfirmAll").modal("hide");

                // Clear selection
                lstDataChiTietGhiNhanKiemKe = [];
                lstRowComfirmtSLKK = [];
                dxDataGhiNhanKiemKe.clearSelection();
                await loadDataGhiNhanKiemKe();
            } else {
                showToast("error", "Có lỗi xảy ra khi xác nhận!");
            }
        } catch (err) {
            console.error("Lỗi:", err);
            showToast("error", "Có lỗi xảy ra khi xác nhận!");
        } finally {
            // Reset nút về trạng thái ban đầu
            $("#btnConfirmAllSave").prop("disabled", false).html('<i class="fas fa-check"></i> Xác nhận');
        }
    });

    // Reset modal khi đóng
    $("#modalConfirmAll").on("hidden.bs.modal", function () {
        $("#btnConfirmAllSave").prop("disabled", false).html('<i class="fas fa-check"></i> Xác nhận');
    });

    $("#khachhangkiemke").on("change", function () {
        loadMHKiemKe();
    });

    $("#mahangkiemke").on("change", function () {
        GetVatTuKiemKe();
    });

    $("#cayvaikiemke").on("change", async function () {
        await loadDataGhiNhanKiemKe();
    });


    const picker3 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker3'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        },
        defaultDate: getFirstDayOfCurrentMonth()
    });

    const picker4 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker4'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        },
        defaultDate: getLastDayOfCurrentMonth()
    });


    $(".dateInput").trigger("click")
    picker3.hide()
    picker4.hide()
    $("#tuNgay").on("change", function () {
        GetPhieuDanhSachVatTuKiemKe();
    })
    $("#denNgay").on("change", function () {
        GetPhieuDanhSachVatTuKiemKe();
    })

    // Sự kiện tìm kiếm vật tư
    $("#searchVatTu").on("input", function () {
        filterDataChiTiet();
    });

    // Sự kiện lọc theo ngày kiểm kê
    $("#ngayKK").on("change", function () {
        filterDataChiTiet();
    });

    // Reset khi đóng modal
    $("#modalInputSLKK").on("hidden.bs.modal", function () {
        $("#searchVatTu").val("");
        $("#ngayKK").val("");
        $("#colClearFilter").addClass("d-none");
        dataChiTietKiemKeOriginal = [];
        rowEditSLKK = null;
    });

    $("#btnClearFilter").on("click", function () {
        // Clear các input
        $("#searchVatTu").val("");
        $("#ngayKK").val("");
        picker3.dates.clear();

        // Ẩn COLUMN bỏ lọc
        $("#colClearFilter").addClass("d-none");

        // Render lại toàn bộ data gốc
        renderTableChiTiet(dataChiTietKiemKeOriginal);
        createViewDxGridChiTiet(dataChiTietKiemKeOriginal)
    });

    // Xử lý khi thay đổi lọc
    $("#loc").on("change", function () {
        const value = $(this).val()

        if (value == "2") {
            $(".filterThoiGian").removeClass("d-none")
            //$(".btnHuyKiemKe").removeClass("col-lg-6 col-xl-6")
            //$(".btnHuyKiemKe").addClass("col-lg-12 col-xl-12")
        } else {
            $(".filterThoiGian").addClass("d-none")
            //$(".btnHuyKiemKe").removeClass("col-4 col-md-5 col-lg-2 col-xl-2")
            //$(".btnHuyKiemKe").addClass("col-12 col-md-12 col-lg-6 col-xl-6")

        }
    })


    $("#modalTimNhanh").on("shown.bs.modal", function () {
        setTimeout(function () {
            $("#inputTimKiem").focus();
        }, 100);
    });

    $("#modalTimNhanh").on("hide.bs.modal", function () {
        $("#inputTimKiem").val("").trigger("input")
    });

    $("#btnTimNhanh").on("click", function () {
        GetDSItemCode();

        $("#modalTimNhanh").modal('show')
    })

    // Nút tìm kiếm
    let searchTimeout;
    $("#inputTimKiem").on("input", function () {
        clearTimeout(searchTimeout);
        const search = $(this).val().trim().toLowerCase();

        searchTimeout = setTimeout(() => {
            if (search === "") {
                // Nếu rỗng thì hiện tất cả
                updateGrid(dxDanhSachItemCode, lstDanhSachItemCode);
            } else {
                // Lọc dữ liệu
                const dataFiltered = lstDanhSachItemCode.filter(item =>
                    String(item.MaVT).trim().toLowerCase().includes(search) ||
                    String(item.MauVT).trim().toLowerCase().includes(search)
                );
                updateGrid(dxDanhSachItemCode, dataFiltered);
            }
        }, 100); // Delay 300ms sau khi ngừng gõ
    });

    $("#phieuvattukiemke").on("change", function () {
        loadDataGhiNhanKiemKe()
    })

    $("#btnSaveKK").on("click", function () {
        if (lstRowComfirmtSLKK.length === 0) {
            showToast("warning", "Không có thông tin vật tư để xác nhận kiểm kê")
            return;
        }
        // Hiển thị số lượng trong modal
        $(".txtSLVTKK").text(lstRowComfirmtSLKK.length);

        // Mở modal xác nhận
        $("#modalConfirmKK").modal("show");
    })

    $("#btnConfirmSave").on("click", async function () {
        const url = `/api/PhieuXuatHangNPL/PostChiTietKiemKeNPL?action=UpdateXacNhan`;

        try {
            // Disable nút để tránh click nhiều lần
            $(this).prop("disabled", true).html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...');

            const lstKiemKeXacNhan = [];
            for (const row of lstRowComfirmtSLKK) {
                const dataChiTiet = await loadDataChiTietKiemKe(row.MaNPL);
                const itemsToConfirm = dataChiTiet.map(i => ({
                    ...i,
                    SLKiemKeEdit: i.SLKiemKeEdit ?? i.SLKiemKe,
                    GhiChu: i.GhiChu ?? "",
                    IsXacNhan: 1
                }));
                lstKiemKeXacNhan.push(...itemsToConfirm);
            }

            // POST tất cả
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(lstKiemKeXacNhan)
            });

            const result = await response.json();

            if (result == "True") {
                showToast("success", `Đã xác nhận ${lstRowComfirmtSLKK.length} vật tư thành công!`);
                $("#modalConfirmKK").modal("hide");

                // Clear selection
                lstRowComfirmtSLKK = [];
                dxDataGhiNhanKiemKe.clearSelection();
                await loadDataGhiNhanKiemKe();
            } else {
                showToast("error", "Có lỗi xảy ra khi xác nhận!");
            }
        } catch (err) {
            console.error("Lỗi:", err);
            showToast("error", "Có lỗi xảy ra khi xác nhận!");
        } finally {
            // Reset nút về trạng thái ban đầu
            $("#btnConfirmAllSave").prop("disabled", false).html('<i class="fas fa-check"></i> Xác nhận');
        }
    })

    // Nút show modal Hủy Kiểm Kê
    $("#btnShowModalDelete").on("click", function () {
        if (lstRowDeleteKK.length === 0) {
            showToast("warning", "Không có thông tin để hủy kiểm kê")
            return;
        }
        $("#modalCancelInventory").modal("show")
    })

    // Nút xác nhận hủy kiểm kê
    $("#btnCancelInventory").on("click", async function () {
        const reason = $("#cancelReason").val().trim();
        var dataUser = !window.CefSharp ? userNameSave : userName

        if (!reason || reason == '') {
            showToast("warning", "Vui lòng nhập lí do hủy");
            return;
        }

        var arr = [];
        lstRowDeleteKK.forEach(item => {
            const object = {
                PhieuVatTuKK: item.PhieuVatTuKK,
                MaNPL: item.MaNPL,
                MaVT: "",
                MauVT: "",
                KhoVai: "",
                MaDVVT: "",
                NguoiTao: dataUser,
                IsLog: "",
                IsNPL: isNPL,
                GhiChu: reason,
                Dot: "",
                TonDKy: "",
                TonCKy: "",
                TuNgay: "",
                DenNgay: "",
                SLNhapKho: "",
                ChiTiet: "",
            }

            arr.push(object)
        })
        await DeleteKiemKe(arr)
    })

    $("#modalCancelInventory").on("hide.bs.modal", function () {
        $("#cancelReason").val('')
    })

})

/// Load data kiểm kê
async function DeleteKiemKe(arr) {
    const url = `/api/DangKyVatTu/PostKiemke?action=CancelPhieuDanhSach`

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(arr)
        })

        const result = await response.json()
        if (result == "True") {
            showToast("success", `Xóa ${lstRowDeleteKK.length} vật tư cần kiểm kê thành công!`)
        }

        // Reset
        $("#cancelReason").val('')
        lstRowDeleteKK = [];
        $("#modalCancelInventory").modal("hide");
        await loadDataGhiNhanKiemKe();
    } catch (err) {
        console.err(err)
    }

}
async function GetDSItemCode() {
    const url = `/api/DangKyVatTu/Get?action=GetDSItemCode&para1=${isNPL}`

    try {
        const response = await fetch(url)
        const data = await response.json()

        // update data
        lstDanhSachItemCode = data
        updateGrid(dxDanhSachItemCode, data)
    } catch (err) {
        console.error(err)
    }
}

async function GetPhieuDanhSachVatTuKiemKe() {
    const loc = $("#loc").val()
    const tuNgay = loc == "1" ? '1990-01-01' : ddmmyyyyToYmd($("#tuNgay").val());
    const denNgay = loc == "1" ? '1990-01-01' : ddmmyyyyToYmd($("#denNgay").val());
    const $phieuSlect = $("#phieuvattukiemke")
    const url = `/api/DangKyVatTu/Get?action=GetPhieuDSVatTuKiemKe&para1=${tuNgay}&para2=${denNgay}&para3=${isNPL}`

    $phieuSlect.empty()
    try {
        const response = await fetch(url)
        const data = await response.json()
        let html = ''

        if (data.length > 0) {
            data.map(phieu => {
                html += `<option value="${phieu.PhieuVatTuKK}">${phieu.Display}</option>`
            })
        }
        $phieuSlect.append(html)
        $phieuSlect.trigger("change")
    } catch (err) {
        console.error(err)
    }
}

async function loadMHKiemKe() {
    const maKH = $("#khachhangkiemke").val();
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GETMHTQVT&para1=1&para2=${maKH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;
        data.map(x => {
            html += `<option value="${x.MaHang}">${x.MaHang == "" ? "Mã hàng rỗng" : x.MaHang}</option>`;
        });
        $("#mahangkiemke").html(html);
        $("#mahangkiemke").select2();
        GetVatTuKiemKe();
    } catch (error) {
        console.error("Lỗi khi tải danh sách mã hàng kiểm kê:", error.message);
    }
}

async function GetVatTuKiemKe() {
    const makh = $("#khachhangkiemke").val();
    const mahang = $("#mahangkiemke").val();
    const tuNgayFormat = "1990-01-01";
    const denNgayFormat = "1990-01-01";

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetVTMKKiemKe&para1=1&para2=${makh}&para3=${mahang}&para4=${tuNgayFormat}&para5=${denNgayFormat}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;

        if (data.length > 0) {
            data.map(x => {
                html += `<option value="${x.MaNPL}">${x.Display}</option>`;
            });
        }

        $("#cayvaikiemke").html(html);
        $("#cayvaikiemke").select2();

        await loadDataGhiNhanKiemKe();
    } catch (error) {
        console.error("Lỗi khi tải vật tư kiểm kê:", error.message);
    }
}

async function loadDataGhiNhanKiemKe() {
    const maPhieu = $("#phieuvattukiemke").val();
    const loc = $("#loc").val();
    const tuNgayFormat = loc == 1 ? "1990-01-01" : ddmmyyyyToYmd($("#tuNgay").val());
    const denNgayFormat = loc == 1 ? "1990-01-01" : ddmmyyyyToYmd($("#denNgay").val());

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetTongQuanKiemKeV2&para1=${maPhieu}&para2=${tuNgayFormat}&para3=${denNgayFormat}&para4=${isNPL}`;

    try {
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error("HTTP error " + response.status);
        }

        const data = await response.json();
        lstDataChiTietGhiNhanKiemKe = data.filter(item => (item.IsXacNhan === 0 || !item.IsXacNhan) && item.IsLog === 1)

        updateGrid(dxDataGhiNhanKiemKe, data)

    } catch (err) {
        console.error("Lỗi khi load dữ liệu ghi nhận kiểm kê:", err);
    }
}

async function loadDataChiTietKiemKe(maNPL) {
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetChiTietKiemKe&para1=${maNPL}`;
    try {
        const response = await fetch(url)

        const data = await response.json();

        return data
    } catch (err) {
        console.error(err)
    }
}

// Hàm render bảng chi tiết
function formatNumber(num) {
    let rounded = Math.round(num * 10000) / 10000;
    return parseFloat(rounded.toFixed(4));
}

function renderTableChiTiet(dataList) {
    let html = "";
    let sumSLNK = 0;
    let sumSLKiemKe = 0;
    dataList.forEach((item, index) => {
        if (item) {
            const slnk = parseFloat(item.SLNK) || 0;
            const slkkEdit = Number(item.SLKiemKeEdit ?? item.SLKiemKe) || 0;
            const colorStyle = slnk !== slkkEdit ? 'background:#FFF3CD;color: black; font-weight: 500;' : '';
            sumSLNK += slnk;
            sumSLKiemKe += slkkEdit;

            html += `
                <tr data-index="${index}" data-barcode="${item.BarCode || ''}">
                    <td class="text-break text-center">${item.MaVT || ""}</td>
                    <td class="text-center">${item.MauVT || ""}</td>
                    <td class="text-center">${item.SoLo || ""}</td>
                    <td class="text-center">${item.SoKienHienThi || ""}</td>
                    <td class="text-center" style="${colorStyle}">
                        ${slnk}
                    </td>
                    <td class="text-center" style="${colorStyle}">
                        ${slkkEdit}
                    </td>
                    <td class="text-center">${formatDate(item.DateKiemKe)}</td>
                    <td class="text-center">${item.GhiChu || ""}</td>
                </tr>`;
        }
    });
    if (dataList.length === 0) {
        $("#btnSaveSLKK").prop("disabled", true);
        html = `
            <tr>
                <td colspan="8" class="text-center py-4">
                    <i class="fa-solid fa-inbox" style="font-size:24px;color:#999;"></i>
                    <p class="mb-0" style="color:#666;">Không tìm thấy dữ liệu</p>
                </td>
            </tr>`;
        $("#sumSLNK").text("");
        $("#sumSLKiemKe").text("");
    } else {
        $("#btnSaveSLKK").prop("disabled", false);
        $("#sumSLNK").text(formatNumber(sumSLNK));
        $("#sumSLKiemKe").text(formatNumber(sumSLKiemKe));
    }
    $("#tbodyChiTietKiemKe").html(html);
    const searchText = $("#searchVatTu").val().trim();
    const ngayKK = $("#ngayKK").val();
    if (!searchText && !ngayKK) {
        $("#colClearFilter").addClass("d-none");
    }
}

// Hàm filter data
/*function filterDataChiTiet() {
    const searchText = $("#searchVatTu").val().toLowerCase().trim();
    const ngayKK = $("#ngayKK").val(); // format: dd/MM/yyyy

    let filteredData = [...dataChiTietKiemKeOriginal];

    // Kiểm tra xem có filter nào đang active không
    const hasActiveFilter = searchText !== "" || ngayKK !== "";

    // Hiển thị/ẩn nút "Bỏ lọc"
    if (hasActiveFilter) {
        $("#colClearFilter").removeClass("d-none");
    } else {
        $("#colClearFilter").addClass("d-none");
    }

    // Filter theo text search (tìm trong MaVT, MauVT, SoKienHienThi)
    if (searchText) {
        filteredData = filteredData.filter(item => {
            const soKien = (item.SoKienHienThi || "").toLowerCase();
            const maVT = (item.MaVT || "").toLowerCase();
            return soKien.includes(searchText) || maVT.includes(searchText);
        });
    }

    // Filter theo ngày kiểm kê
    if (ngayKK) {
        filteredData = filteredData.filter(item => {
            const itemDate = formatDate(item.DateKiemKe);
            return itemDate === ngayKK;
        });
    }

    renderTableChiTiet(filteredData);
}*/
/// DxDataGrid
function updateGrid(grid, dataSource) {
    grid.beginUpdate();
    grid.option({ dataSource: dataSource });
    grid.endUpdate();
}
DevExpress.localization.loadMessages({
    "vi": {
        "dxDataGrid-filterRowShowAllText": "(Tất cả)",
        "dxList-selectAll": "Tất cả"
    }
});
DevExpress.localization.locale("vi");
function createViewDxGhiNhanKiemKe() {
    dxDataGhiNhanKiemKe = $("#dxDataGhiNhanKiemKe").dxDataGrid({
        dataSource: [],
        rowAlternationEnabled: true,
        columnAutoWidth: false,
        allowColumnResizing: true,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: {
            visible: true, applyFilter: "auto", operationDescriptions: {}
        },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        width: "100%",
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                dataField: "IsXacNhan",
                caption: "Duyệt",
                alignment: "center",
                allowFiltering: false,
                allowSorting: false,
                dataType: "number",
                minWidth: 100,
                /*  lookup: {
                      dataSource: [
                          { id: 1, text: "Đã duyệt" },
                          { id: 0, text: "Chưa duyệt" }
                      ],
                      valueExpr: "id",
                      displayExpr: "text"
                  },*/
                cellTemplate: function (container, options) {
                    $("<i>")
                        .addClass(options.value === 1
                            ? "fas fa-check-circle"
                            : "fas fa-times-circle")
                        .css({
                            color: options.value === 1 ? "#28a745" : "#dc3545",
                            fontSize: "20px"
                        })
                        .appendTo(container);
                }
            },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", width: 140 },
            {
                dataField: "ChiTiet",
                caption: "Mô Tả",
                alignment: "left",
                minWidth: 500
            },
            {
                dataField: "IsLog",
                caption: "Trạng Thái",
                alignment: "center",
                cellTemplate: function (container, options) {
                    const $icon =
                        $(`<span style="font-size:14px; font-weight: 500; color: ${options.data.IsLog === 0 ? "green" : "#ffa200"}" class="">
                        <i class="${options.data.IsLog === 0 ? "fa-solid fa-circle-check" : "fa-solid fa-spinner"}"></i>
                        ${options.data.IsLog === 0
                                ? "Hoàn Thành Kiểm Kê"
                                : "Đang Kiểm Kê"}
                        </span>`);
                    container.append($icon);
                },
                minWidth: 170,
            },

            { dataField: "MauVT", caption: "Màu", alignment: "center", width: 140 },
            { dataField: "KhoVai", caption: "Width/Size", alignment: "center", width: 90 },
            {
                dataField: "SLNK",
                caption: "Tồn Kho",
                alignment: "center",
                dataType: "number",
                minWidth: 140,
                cellTemplate: function (container, options) {

                    const value = Number(options.data.SLNK)
                    const truncated = Math.trunc(value * 10000) / 10000;

                    const valueConvert = Number(truncated).toLocaleString()
                    container.append(valueConvert)
                }
            },
            {
                dataField: "SLKiemKe",
                caption: "SL Kiểm Kê",
                alignment: "center",
                dataType: "number",
                minWidth: 140,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    const slKiemKe =
                        options.data.SLKiemKeEdit != null &&
                            options.data.SLKiemKeEdit !== 0
                            ? options.data.SLKiemKeEdit
                            : options.data.SLKiemKe;
                    const valueDisplay = slKiemKe ?? 0;

                    container.css({
                        overflow: "visible",
                        position: "relative"
                    });

                    const $wrapper = $("<div></div>").css({
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        gap: "6px",
                        width: "100%",
                        height: "100%",
                        position: "relative"
                    });

                    const $text = $("<span></span>").text(valueDisplay.toLocaleString());
                    const $editBtn = $("<i></i>")
                        .addClass("fa-solid fa-eye")
                        .css({
                            cursor: "pointer",
                            fontSize: "14px",
                            marginTop: "1px",
                            position: 'absolute',
                            top: '-8px',
                            right: '0px'
                        })
                        .on("click", async function (e) {
                            e.stopPropagation();

                            const dataChiTietKiemKe = await loadDataChiTietKiemKe(options.data.MaNPL);

                            rowEditSLKK = {
                                mainData: options.data,
                                details: dataChiTietKiemKe
                            };

                            // Lưu data gốc để filter
                            dataChiTietKiemKeOriginal = [...dataChiTietKiemKe];

                            /*renderTableChiTiet(dataChiTietKiemKe);*/
                            createViewDxGridChiTiet(dataChiTietKiemKe)
                            $("#modalInputSLKK").modal("show");
                        });

                    $wrapper.append($text, $editBtn);
                    container.append($wrapper);
                }
            },
            {
                dataField: "ChenhLechMet",
                caption: "Chênh Lệch Mét",
                alignment: "center",
                minWidth: 140,
                cellTemplate: function (container, options) {

                    const value = Number(options.data.ChenhLechMet)
                    const truncated = Math.trunc(value * 10000) / 10000;

                    const valueConvert = Number(truncated).toLocaleString();
                    container.append(valueConvert)
                }
            },
            {
                dataField: "ChenhLechTT",
                caption: "Chênh Lệch Thành Tiền",
                alignment: "center",
                minWidth: 120,
            },
            {
                dataField: "KhoangNgay",
                caption: "Khoảng Ngày",
                alignment: "center",
                minWidth: 180,
            },
            {
                caption: "Hủy",
                alignment: "center",
                allowSorting: false,
                allowFiltering: false,
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const isVisible = Number(options.data.IsXacNhan) === 1 ||
                        Number(options.data.IsLog === 0)

                    if (isVisible) {
                        const $checkbox = $("<input>")
                            .attr("type", "checkbox")
                            .css({
                                width: 23,
                                height: 23,
                                cursor: "pointer",
                                opacity: "1",
                                accentColor: "rgb(228 105 75)",
                                pointerEvents: isVisible ? "auto" : "none"
                            });
                        $checkbox.on("change", function (e) {
                            e.stopPropagation();

                            const index = lstRowDeleteKK.findIndex(row =>
                                row.MaNPL === options.data.MaNPL
                            );

                            if ($(this).is(":checked")) {
                                // Chọn
                                if (index === -1) {
                                    lstRowDeleteKK.push(options.data);
                                }
                            } else {
                                // Bỏ chọn
                                if (index > -1) {
                                    lstRowDeleteKK.splice(index, 1);
                                }
                            }

                        });

                        container.append($checkbox);
                    } else {
                        container.append($(`<div></div>`));
                    }

                }
            },
            {
                caption: "Xác nhận",
                alignment: "center",
                allowSorting: false,
                allowFiltering: false,
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const isDisabled = Number(options.data.IsXacNhan) === 1 ||
                        options.data.SLKiemKe === 0 ||
                        !options.data.SLKiemKe ||
                        userNameSave !== "admin";

                    const isSelected = lstRowComfirmtSLKK.some(row =>
                        row.MaNPL === options.data.MaNPL
                    );

                    const $checkbox = $("<input>")
                        .attr("type", "checkbox")
                        .prop("checked", isSelected)
                        .prop("disabled", isDisabled)
                        .css({
                            width: 23,
                            height: 23,
                            cursor: isDisabled ? "not-allowed" : "pointer",
                            opacity: "1",
                            accentColor: "#28a745",
                            pointerEvents: isDisabled ? "none" : "auto"
                        });

                    if (!isDisabled) {
                        $checkbox.on("change", function (e) {
                            e.stopPropagation();

                            const index = lstRowComfirmtSLKK.findIndex(row =>
                                row.MaNPL === options.data.MaNPL
                            );

                            if ($(this).is(":checked")) {
                                // Chọn
                                if (index === -1) {
                                    lstRowComfirmtSLKK.push(options.data);
                                }
                            } else {
                                // Bỏ chọn
                                if (index > -1) {
                                    lstRowComfirmtSLKK.splice(index, 1);
                                }
                            }

                            $("#btnShowSaveSLKK").prop("disabled", lstRowComfirmtSLKK.length === 0);
                        });
                    }


                    container.append($checkbox);
                }
            },
        ],

        summary: {
            totalItems: [
                {
                    name: "tongKho",
                    summaryType: "custom",
                    showInColumn: "ChiTiet",
                },
                {
                    column: "SLNK",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value)
                        const truncated = Math.trunc(value * 10000) / 10000;

                        return truncated.toLocaleString();
                    }
                },

                {
                    column: "SLKiemKe",
                    summaryType: "sum",
                    calculateCustomSummary(options) {
                        if (options.summaryProcess === "start") {
                            options.totalValue = 0;
                        }

                        if (options.summaryProcess === "calculate") {
                            const data = options.value;
                            const value =
                                data.SLKiemKeEdit != null && data.SLKiemKeEdit !== 0
                                    ? data.SLKiemKeEdit
                                    : data.SLKiemKe;

                            options.totalValue += Number(value || 0);
                        }
                    },
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value)
                        const truncated = Math.trunc(value * 10000) / 10000;

                        return truncated.toLocaleString();
                    }
                },

                {
                    column: "ChenhLechMet",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value)
                        const truncated = Math.trunc(value * 10000) / 10000;

                        return truncated.toLocaleString();
                    }
                },
                {
                    column: "ChenhLechTT",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value)
                        const truncated = Math.trunc(value * 10000) / 10000;

                        return truncated.toLocaleString();
                    }
                },
            ],
            calculateCustomSummary: function (options) {
                if (options.name === "tongKho") {
                    if (options.summaryProcess === "start") {
                        options.totalValue = { totalRoll: 0, };
                    }
                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalRoll += 1
                    }
                    if (options.summaryProcess === "finalize") {
                        const { totalRoll, totalNH } = options.totalValue;
                        options.totalValue = `${totalRoll.toLocaleString()} Vật tư`;
                    }
                }
            }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data") {

                const slKiemKe =
                    e.data.SLKiemKeEdit != null && e.data.SLKiemKeEdit !== 0
                        ? e.data.SLKiemKeEdit
                        : e.data.SLKiemKe;

                const slNhapKho = e.data.SLNK;

                const isKhacSL =
                    slKiemKe != null &&
                    slNhapKho != null &&
                    slKiemKe !== 0 &&
                    slKiemKe !== slNhapKho;

                // SL kiểm kê = 0
                if (e.column.dataField === "SLKiemKe" && e.data.SLKiemKe === 0) {
                    e.cellElement.css({
                        background: "rgb(206 250 255)",
                        color: "black",
                        fontWeight: "600"
                    });
                }

                // Nếu khác SLNK → tô CẢ HAI CỘT
                if (isKhacSL) {
                    if (
                        e.column.dataField === "SLKiemKe" ||
                        e.column.dataField === "SLNK"
                    ) {
                        e.cellElement.css({
                            background: "#FFF3CD",
                        });
                    }
                }
            }
            // header
            if (e.rowType === "header") {
                $(e.cellElement)
                    .addClass("col-header text-center")
                    .css("vertical-align", "middle");
            }

            // data căn giữa
            if (e.rowType === "data") {
                $(e.cellElement)
                    .addClass("text-center")
                    .css("vertical-align", "middle");
            }
        }


    }).dxDataGrid("instance");

    $("#Layer_1").click()
}


function formatDate(dateStr) {
    if (!dateStr) return "";
    const d = new Date(dateStr);
    if (isNaN(d)) return dateStr; // phòng khi chuỗi không parse được

    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();

    return `${day}/${month}/${year}`;
}
function ddmmyyyyToYmd(dateStr) {
    if (!dateStr) return "1990-01-01";
    const [d, m, y] = dateStr.split("/");
    return `${y}-${m}-${d}`;
}

var isNPL = 1
function PlayAudioSS() {
    const beepSound = document.getElementById("beepSoundSuccess");
    beepSound.currentTime = 0;
    beepSound.play();
}
function PlayAudio() {
    const beepSound = document.getElementById("beepSoundSuccess");
    beepSound.currentTime = 0;
    beepSound.play();
}
function PlayAudioError() {
    const beepSoundE = document.getElementById("beepSoundError");
    beepSoundE.currentTime = 0;
    beepSoundE.play();
}
function showToast(type, message, delay = 2000) {
    let toastId, messageId;

    switch (type) {
        case 'success':
            toastId = 'successToast';
            messageId = 'successMessage';
            break;
        case 'error':
            toastId = 'errorToast';
            messageId = 'errorMessage';
            break;
        case 'warning':
            toastId = 'warningToast';
            messageId = 'warningMessage';
            break;
        default:
            console.error('Unknown toast type:', type);
            return;
    }

    $('#' + messageId).text(message);

    const toastElement = $('#' + toastId)[0];
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: delay
    });

    toast.show();
}
function createViewDxGridDanhSachItemCode() {
    dxDanhSachItemCode = $("#dxDanhSachItemCode").dxDataGrid({
        dataSource: [],
        rowAlternationEnabled: true,
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: false },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "PhieuVatTuKK",
                caption: "Số Phiếu",
                alignment: "center",
                minWidth: 40,
                cellTemplate: function (container, options) {
                    const $wrapper = $(`<div></div>`).css({ display: "flex", justifyContent: "center", height: "25px", gap: "8px" })

                    // Text
                    const $span = $(`<span>${options.data.PhieuVatTuKK}</span>`)

                    // Icon
                    const $btnNext = $(`<i class="fa-solid fa-circle-right"></i>`)
                        .css({ fontSize: '16px', color: 'green' })
                        .on("click", function () {
                            $("#phieuvattukiemke").val(`${options.data.PhieuVatTuKK}`).trigger("change");
                            $("#modalTimNhanh").modal('hide');
                            $("#inputTimKiem").val('').trigger('input');
                            updateGrid(dxDanhSachItemCode, []);
                        })

                    $wrapper.append($span, $btnNext).appendTo(container)

                }
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "KhoVai",
                caption: "Khổ/Size",
                alignment: "center",
                minWidth: 40,
            },
            {
                dataField: "MaDVVT",
                caption: "Đơn Vị",
                alignment: "center",
                minWidth: 40,
            },
            {
                dataField: "DisplayDate",
                caption: "Khoảng Ngày Kiểm Kê",
                alignment: "center",
                minWidth: 150
            },

        ],
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

                const searchTerm = $("#inputTimKiem").val().trim().toLowerCase();
                if (searchTerm && (e.column.dataField === "MaVT" || e.column.dataField === "MauVT")) {
                    const cellValue = String(e.value || "");
                    const cellValueLower = cellValue.toLowerCase();

                    if (cellValueLower.includes(searchTerm)) {
                        // Tìm vị trí bắt đầu của text match
                        const startIndex = cellValueLower.indexOf(searchTerm);
                        const endIndex = startIndex + searchTerm.length;

                        // Tạo HTML với phần match được highlight
                        const before = cellValue.substring(0, startIndex);
                        const match = cellValue.substring(startIndex, endIndex);
                        const after = cellValue.substring(endIndex);

                        const highlightedHTML = `${before}<mark style="background-color: #ffeb3b; font-weight: 600; padding: 2px 4px; border-radius: 3px;">${match}</mark>${after}`;

                        $(e.cellElement).html(highlightedHTML);
                    }
                }
            }
        },
        summary: {
            totalItems: [
                {
                    column: "TonDKy",
                    summaryType: "sum",
                    showInColumn: "TonDKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "TonCKy",
                    summaryType: "sum",
                    showInColumn: "TonCKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "SLNhapKho",
                    summaryType: "sum",
                    showInColumn: "SLNhapKho",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
            ],
        }

    }).dxDataGrid("instance");

    $("#Layer_1").click()
}
function createViewDxGridChiTiet(data) {
    $("#dxDataGridChiTiet").dxDataGrid({
        dataSource: data,
        rowAlternationEnabled: true,
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "MaVT",
                caption: "Item Code",
                alignment: "center",
                minWidth: 80,
            },

            {
                dataField: "MauVT",
                caption: "Màu",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "SoLo",
                caption: "Số Lô",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "SoKienHienThi",
                caption: "Số Roll/Số Kiện",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "SLNK",
                caption: "Tồn Kho",
                alignment: "center",
                minWidth: 80,
                cellTemplate: function (container,options) {

                    const value = Number(options.data.SLNK)
                    const truncated = Math.trunc(value * 10000) / 10000;

                    const valueConvert = Number(truncated).toLocaleString();
                    container.append(valueConvert)
                }
            },
            {
                dataField: "SLKiemKe",
                caption: "SL Kiểm Kê",
                alignment: "center",
                minWidth: 80,
                calculateCellValue: function (rowData) {
                    const slkkEdit = Number(rowData.SLKiemKeEdit ?? rowData.SLKiemKe) || 0;
                    return slkkEdit;
                },
                cellTemplate: function (container, options) {
                    const slkkEdit = Number(options.data.SLKiemKeEdit ?? options.data.SLKiemKe) || 0;
                    const value = slkkEdit
                    const truncated = Math.trunc(value * 10000) / 10000;

                    const valueConvert = Number(truncated).toLocaleString();
                    container.append(valueConvert)
                }
            },
            {
                dataField: "DateKiemKe",
                caption: "Ngày Kiểm Kê",
                alignment: "center",
                minWidth: 150,
                cellTemplate: function (container, options) {
                    const dateFormat = moment(options.data.DateKiemKe).format("DD/MM/YYYY")
                    container.append(dateFormat)
                }
            },
            {
                dataField: "GhiChu",
                caption: "Ghi Chú",
                alignment: "center",
                minWidth: 150
            },

        ],
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

                if (e.column.dataField === "SLKiemKe" || e.column.dataField === "SLNK") {
                    const slkk = Number(e.data.SLKiemKeEdit ?? e.data.SLKiemKe) || 0;
                    const slnk = Number(e.data.SLNK) || 0;

                    if (slkk != slnk) {
                        $(e.cellElement).css({
                            color: "black",
                            backgroundColor: "#FFF3CD"
                        });
                    }
                }

                const searchTerm = $("#inputTimKiem").val().trim().toLowerCase();
                if (searchTerm && (e.column.dataField === "MaVT" || e.column.dataField === "MauVT")) {
                    const cellValue = String(e.value || "");
                    const cellValueLower = cellValue.toLowerCase();

                    if (cellValueLower.includes(searchTerm)) {
                        // Tìm vị trí bắt đầu của text match
                        const startIndex = cellValueLower.indexOf(searchTerm);
                        const endIndex = startIndex + searchTerm.length;

                        // Tạo HTML với phần match được highlight
                        const before = cellValue.substring(0, startIndex);
                        const match = cellValue.substring(startIndex, endIndex);
                        const after = cellValue.substring(endIndex);

                        const highlightedHTML = `${before}<mark style="background-color: #ffeb3b; font-weight: 600; padding: 2px 4px; border-radius: 3px;">${match}</mark>${after}`;

                        $(e.cellElement).html(highlightedHTML);
                    }
                }
            }
        },
        summary: {
            totalItems: [
                {
                    column: "SLNK",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toLocaleString();
                    }
                },
                {
                    column: "SLKiemKe",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toLocaleString();
                    }
                },
            ],
        }

    })
}
function getFirstDayOfCurrentMonth() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1);
}

function getLastDayOfCurrentMonth() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth() + 1, 0);
}



function ExportFile() {
    const maPhieu = $("#phieuvattukiemke").val();
    const loc = $("#loc").val();
    const tuNgayFormat = loc == 1 ? "1990-01-01" : ddmmyyyyToYmd($("#tuNgay").val());
    const denNgayFormat = loc == 1 ? "1990-01-01" : ddmmyyyyToYmd($("#denNgay").val());
    const url = `/api/PhieuXuatHangNPL/ExportFileGhiNhanKiemKeNL?action=GetTongQuanKiemKeV2&para1=${maPhieu}&para2=${tuNgayFormat}&para3=${denNgayFormat}&para4=1`;
    var link = document.createElement('a');
    link.href = url;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}