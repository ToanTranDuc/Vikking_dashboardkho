
function ddmmyyyyToYmd(dateStr) {
    if (!dateStr) return "1990-01-01";
    const [d, m, y] = dateStr.split("/");
    return `${y}-${m}-${d}`;
}

function PlayAudio() {
    const beepSound = document.getElementById("beepSound");
    beepSound.currentTime = 0;
    beepSound.play();
}

function PlayAudioError() {
    const beepSoundE = document.getElementById("beepSoundError");
    beepSoundE.currentTime = 0;
    beepSoundE.play();
}

/// HELPER FUNCTIONS
function updateGrid(grid, dataSource) {
    grid.beginUpdate();
    grid.option({ dataSource: dataSource });
    grid.endUpdate();
}

function normalizeKey(rawKey) {
    return String(rawKey || "")
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .replace(/[^a-zA-Z0-9]/g, "")
        .toLowerCase();
}

function toNumberSafe(value) {
    if (value === null || value === undefined || value === "") return null;
    const num = parseFloat(String(value).replace(",", "."));
    return Number.isFinite(num) ? num : null;
}

function openFloatingTagBox(options, currentValue, cellData, fieldName, $displayEl, placeholder, $anchor) {
    floatingTagBoxInstance.close();
    floatingTagBoxInstance.option("items", options);
    floatingTagBoxInstance.option("value", currentValue.map(v =>
        typeof v === "object" ? JSON.stringify(v) : v
    ));

    // ✅ Sync tempSelectedValue với giá trị hiện tại của cell
    tempSelectedValue = currentValue.map(v =>
        typeof v === "object" ? JSON.stringify(v) : v
    );

    activeCell = { data: cellData, fieldName, $displayEl, placeholder };

    const offset = $anchor.offset();
    const winHeight = $(window).height();
    const dropdownHeight = 320;
    let top = offset.top + $anchor.outerHeight() + 2;
    if (top + dropdownHeight > winHeight) {
        top = offset.top - dropdownHeight - 2;
    }

    $floatingTagBox.css({
        top: top,
        left: offset.left,
        minWidth: Math.max($anchor.outerWidth(), 220)
    }).show();

    setTimeout(() => {
        floatingTagBoxInstance.open();
        floatingTagBoxInstance.focus();
    }, 0);
}

function InsertThuVien() {
    if (!selectedItemsChungLoai || selectedItemsChungLoai.length === 0) {
        showToast("warning", "Vui lòng chọn ít nhất một vật tư!");
        return;
    }
    const dataSource = dxDataGridDangKyVatTu.option("dataSource") || [];
    const maDHNhap = getManualMaDH();
    const maLenhSanXuatCurrent = $("#malenhdangkyvattu").val() || "";
    const maLenhDisplayCurrent = ($("#malenhdangkyvattu option:selected").text() || "").trim();
    const previouslySelectedRows = [...selectedRowsToSave];
    const insertedItems = [];

    // Lấy giá trị từ dòng đầu tiên của dataSource
    const firstRow = dataSource.length > 0 ? dataSource[0] : null;
    const defaultMaLenhSanXuat = firstRow?.MaLenhSanXuat || maLenhSanXuatCurrent;
    const defaultMaLenh = firstRow?.MaLenh || maLenhDisplayCurrent;
    const defaultMaDH = firstRow?.MaDH || maDHNhap;

    selectedItemsChungLoai.forEach(function (item) {
        // Kiểm tra trùng theo MaNPL (đã tồn tại trong lưới DangKyVatTu chưa)
        const alreadyInGrid = dataSource.some(row => row.MaNPL === item.MaNPL);
        if (alreadyInGrid) {
            showToast("warning", `Vật tư ${item.MaNPL} đã tồn tại trong danh sách đăng ký!`);
            return; // bỏ qua item này
        }
        const newItem = {
            ...item,
            MaLenhSanXuat: defaultMaLenhSanXuat,
            MaLenh: defaultMaLenh,
            MaDH: defaultMaDH,
            GhiChu: '',
            __thuVienImportId: `TV_${Date.now()}_${Math.random().toString(16).slice(2)}`
        };
        insertedItems.push(newItem);
        dataSource.unshift(newItem);
    });

    selectedRowsToSave = [...previouslySelectedRows, ...insertedItems];
    const sortedDataSource = sortDangKyDataSource(dataSource);
    const selectedBaseKeys = new Set(selectedRowsToSave.map(getDangKyRowBaseKey));
    selectedRowsToSave = sortedDataSource.filter(row => {
        return selectedBaseKeys.has(getDangKyRowBaseKey(row));
    });
    dxDataGridDangKyVatTu.option("dataSource", sortedDataSource);
    dxDataGridDangKyVatTu.refresh();

    selectedItemsChungLoai = [];
    const thuVienSource = dxDataGridThuVien.option("dataSource");
    dxDataGridThuVien.option("dataSource", [...thuVienSource]);
    $("#modalThuVien").modal("hide");
}

function setPenColor(color) {
    penColor = color;
    $('#penColor').val(color);
}

function setPenSize(size) {
    penSize = size;
    $('.pen-size-btn').removeClass('active');
    $(`.pen-size-btn[data-size="${size}"]`).addClass('active');
}

function initCanvas() {
    const container = canvas.parentElement;
    const containerWidth = container.offsetWidth;

    // Xác định kích thước canvas dựa trên màn hình
    if (window.innerWidth < 768) {
        canvas.width = Math.min(containerWidth - 40, 500);
        canvas.height = 250;
    } else {
        canvas.width = Math.min(containerWidth - 40, 700);
        canvas.height = 300;
    }

    // Cấu hình lại context sau khi thay đổi kích thước
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
}

function getMousePos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.clientX - rect.left) * scaleX,
        y: (e.clientY - rect.top) * scaleY
    };
}

function getTouchPos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.touches[0].clientX - rect.left) * scaleX,
        y: (e.touches[0].clientY - rect.top) * scaleY
    };
}

function startDrawing(e) {
    isDrawing = true;
    const pos = getMousePos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function draw(e) {
    if (!isDrawing) return;

    const pos = getMousePos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function stopDrawing() {
    if (isDrawing && currentStroke.length > 0) {
        signatureHistory.push([...currentStroke]);
        currentStroke = [];
    }
    isDrawing = false;
}

function handleTouchStart(e) {
    e.preventDefault();
    isDrawing = true;
    const pos = getTouchPos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function handleTouchMove(e) {
    e.preventDefault();
    if (!isDrawing) return;

    const pos = getTouchPos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function clearSignature() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    signatureHistory = [];
    currentStroke = [];
}

function undoSignature() {
    if (signatureHistory.length === 0) return;

    signatureHistory.pop();
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    signatureHistory.forEach(stroke => {
        if (stroke.length === 0) return;

        ctx.beginPath();
        ctx.moveTo(stroke[0].x, stroke[0].y);

        stroke.forEach((point, index) => {
            if (index === 0) return;
            ctx.strokeStyle = point.color;
            ctx.lineWidth = point.size;
            ctx.lineTo(point.x, point.y);
            ctx.stroke();
        });
    });
}

function parseJsonSafe(v) {
    if (!v) return null;
    if (typeof v === "object") return v; 
    try {
        return JSON.parse(v);
    } catch {
        return null;
    }
}

function handleAutoCheckThuVien(rowData, rowIndex) {
    const hasData = (rowData.SLDK !== null && rowData.SLDK !== "" && rowData.SLDK !== 0)
        || (rowData.LiDo && rowData.LiDo.trim() !== "");

    const isSelected = isThuVienRowSelected(rowData);

    if (hasData && !isSelected) {
        selectedItemsChungLoai.push(rowData);
    }
    else if (!hasData && isSelected) {
        selectedItemsChungLoai = selectedItemsChungLoai.filter(item =>
            !(item.MaNPL === rowData.MaNPL)
        );
    }

    const $row = $(dxDataGridThuVien.getRowElement(rowIndex));
    const $cb = $row.find('.row-checkbox');
    if ($cb.length) {
        $cb.prop('checked', hasData);
    }
    setThuVienRowHighlight($row, hasData);
}

function formatThousands(numStr) {
    return String(numStr || 0).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function formatInputDisplay(value) {
    if (value === null || value === undefined || value === "") return "";
    let str = value.toString();
    let [intPart, decPart] = str.split(".");

    let formattedInt = intPart.replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    return decPart !== undefined ? `${formattedInt}.${decPart.substring(0, 4)}` : formattedInt;
}

function loadPhongBanOptions() {
    const $sel = $("#tkPhongBan");
    const currentVal = $sel.val();

    const pbSet = new Map();
    lstDataDangKyVatTu.forEach(item => {
        if (item.MaPB && item.TenPB) {
            pbSet.set(item.MaPB, item.TenPB);
        }
    });

    $sel.find("option:not(:first)").remove();
    pbSet.forEach((tenPB, maPB) => {
        $sel.append(`<option value="${maPB}">${tenPB}</option>`);
    });

    if (currentVal) $sel.val(currentVal);
}

function filterByTrangThai() {
    const trangThai = $("#trangThaiDuyet").val() || "all";
    const tuNgayStr = $("#filterTuNgay").val() || "";
    const denNgayStr = $("#filterDenNgay").val() || "";
    const maHang = ($("#tkMaHang").val() || "").trim().toLowerCase();
    const maLenh = ($("#tkMaLenh").val() || "").trim().toLowerCase();
    const nguoiLap = ($("#tkNguoiLap").val() || "").trim().toLowerCase();
    const phongBan = $("#tkPhongBan").val() || "";

    function parseDate(str) {
        if (!str) return null;
        const [d, m, y] = str.split("/");
        if (!d || !m || !y) return null;
        return new Date(y, m - 1, d);
    }

    const tuNgay = parseDate(tuNgayStr);
    const denNgay = parseDate(denNgayStr);
    if (denNgay) denNgay.setHours(23, 59, 59);

    let filtered = [...lstDataDangKyVatTu];

    if (trangThai === "done") {
        filtered = filtered.filter(item =>
            item.SignTBPMer && item.SignTBPMer.trim() !== ''
        );
    } else if (trangThai === "pending") {
        filtered = filtered.filter(item =>
            !item.SignTBPMer || item.SignTBPMer.trim() === ''
        );
    }

    if (tuNgay || denNgay) {
        filtered = filtered.filter(item => {
            if (!item.NgayTH) return false;
            const ngayTH = new Date(item.NgayTH);
            if (tuNgay && ngayTH < tuNgay) return false;
            if (denNgay && ngayTH > denNgay) return false;
            return true;
        });
    }

    if (maHang || maLenh || nguoiLap || phongBan) {
        filtered = filtered.filter(item => {
            const okMaHang = !maHang || String(item.MaDH || "").toLowerCase().includes(maHang);
            const okMaLenh = !maLenh || String(item.MaLenh || "").toLowerCase().includes(maLenh);
            const okNguoi = !nguoiLap
                || String(item.TenUserNgDK_Ten || "").toLowerCase().includes(nguoiLap);
            const okPB = !phongBan || String(item.MaPB || "") === phongBan;
            return okMaHang && okMaLenh && okNguoi && okPB;
        });
    }

    updateGrid(dxDataGridDanhSachDangKy, filtered);
    setTimeout(function () {
        mergeSignCells();
    }, 100);
}
function mergeSignCells() {
    const grid = dxDataGridDanhSachDangKy;
    if (!grid) return;

    const $rows = grid.element().find(".dx-datagrid-rowsview .dx-data-row");
    if ($rows.length === 0) return;

    $rows.each(function () {
        $(this).find("td").removeAttr("rowspan").show();
    });

    const visibleRows = grid.getVisibleRows().filter(r => r.rowType === "data");
    if (visibleRows.length === 0) return;

    const mergeFields = ["SignNgDK", "SignTBPNgDK", "SignMer", "SignTBPMer"];

    const allVisibleCols = grid.getVisibleColumns();

    mergeFields.forEach(function (dataField) {
        const colIndex = allVisibleCols.findIndex(col => col.dataField === dataField);
        if (colIndex === -1) return;

        let i = 0;
        while (i < visibleRows.length) {
            const currentPhieu = visibleRows[i].data?.PhieuCT_NgoaiDH;
            let spanCount = 1;
            let j = i + 1;

            while (
                j < visibleRows.length &&
                visibleRows[j].data?.PhieuCT_NgoaiDH === currentPhieu
            ) {
                spanCount++;
                j++;
            }

            if (spanCount > 1) {
                const $firstTd = $($rows[i]).find("td").eq(colIndex);
                $firstTd.attr("rowspan", spanCount).css("vertical-align", "middle");

                for (let k = i + 1; k < i + spanCount; k++) {
                    $($rows[k]).find("td").eq(colIndex).hide();
                }
            }

            i += spanCount;
        }
    });
}