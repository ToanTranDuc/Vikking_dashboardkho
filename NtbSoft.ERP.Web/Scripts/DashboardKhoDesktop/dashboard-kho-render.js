/**
 * @file dashboard-kho-render.js
 * @description Xử lý logic vẽ giao diện chính (Bảng, Biểu đồ Highcharts, Lịch trình).
 * @version 2.7.26
 */

//#region TABLE RENDERERS
/**
 * Vẽ bảng danh sách chi tiết các kệ kho, hiển thị tỷ lệ lấp đầy kèm thanh tiến trình (progress bar).
 * Dữ liệu được sắp xếp giảm dần theo tỷ lệ lấp đầy (lấy top 12 kệ).
 */
function renderRacksTable() {
    var body = byId(ids.racksBody);
    if (!body) return;
    if (state.racks.length === 0) {
        body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có dữ liệu kệ kho</td></tr>';
        return;
    }

    var ranked = state.racks
        .map(function (item, index) {
            var used = toNumber(item.TongCBMSuDungTrongKe);
            var cap = toNumber(item.TongCBMTrongKe);
            return {
                sourceIndex: index,
                Module: item.Module == 1 ? "NL" : item.Module == 2 ? "PL" : String(item.Module || ""),
                TenKe: item.TenKe,
                used: used,
                rate: cap > 0 ? (used / cap) * 100 : 0,
            };
        })
        .sort(function (a, b) {
            return b.rate - a.rate;
        })
        .slice(0, 12);

    // v2.4.9 — pct bar gradient + module chip + top3 highlight
    var html = ranked
        .map(function (item, stt) {
            var pct = item.rate;
            var lvl = pct >= 100 ? "extreme" : pct >= 85 ? "high" : pct >= 50 ? "med" : "low";
            var modCls = item.Module === "NL" ? "dk-rack-mod-nl" : "dk-rack-mod-pl";
            var topCls = stt < 3 ? " dk-rack-row-top" : "";
            var pctBarHtml =
                '<div class="dk-rack-pct">' +
                '<div class="dk-rack-pct-bar dk-rack-pct-' +
                lvl +
                '" style="width:' +
                Math.min(100, pct).toFixed(1) +
                '%"></div>' +
                '<span class="dk-rack-pct-num">' +
                (pct >= 100 ? '<i class="fa-solid fa-triangle-exclamation"></i> ' : "") +
                formatNumber(pct, 1) +
                "%" +
                "</span>" +
                "</div>";
            return (
                '<tr class="dk-rack-row' +
                topCls +
                '" ' +
                rowDataAttr("rackRow", item.sourceIndex) +
                ">" +
                '<td class="text-center"><b>' +
                (stt + 1) +
                "</b></td>" +
                '<td class="text-center"><span class="dk-rack-module-chip ' +
                modCls +
                '">' +
                escapeHtml(item.Module) +
                "</span></td>" +
                "<td>" +
                escapeHtml(item.TenKe || "") +
                "</td>" +
                '<td class="text-end dk-cell-num">' +
                formatNumber(item.used, 2) +
                "</td>" +
                "<td>" +
                pctBarHtml +
                "</td>" +
                "</tr>"
            );
        })
        .join("");

    body.innerHTML = html;
}

/**
 * Vẽ bảng danh sách PO chuẩn bị nhập kho (hàng sắp về).
 * Sắp xếp theo ngày dự kiến nhập gần nhất.
 */
function renderInboundTable() {
    var body = byId(ids.inboundBody);
    if (!body) return;
    if (state.inbound.length === 0) {
        body.innerHTML = '<tr><td colspan="4" class="dk-empty">Không có lệnh chuẩn bị về</td></tr>';
        return;
    }

    var data = state.inbound
        .map(function (item, index) {
            return { sourceIndex: index, item: item };
        })
        .sort(function (a, b) {
            return toNumber(parseDate(a.item.NgayNKDuKien)) - toNumber(parseDate(b.item.NgayNKDuKien));
        })
        .slice(0, 12);

    var html = data
        .map(function (entry) {
            var item = entry.item;
            return (
                "<tr " +
                rowDataAttr("inboundRow", entry.sourceIndex) +
                ">" +
                "<td>" +
                escapeHtml(item.PO || item.POMua || "") +
                "</td>" +
                "<td>" +
                escapeHtml(item.MaHang || "") +
                "</td>" +
                '<td class="text-end">' +
                formatNumber(item.SoLuongSP, 0) +
                "</td>" +
                "<td>" +
                escapeHtml(formatDate(item.NgayNKDuKien)) +
                "</td>" +
                "</tr>"
            );
        })
        .join("");

    body.innerHTML = html;
}

/**
 * Vẽ bảng danh sách các Lệnh sản xuất chuẩn bị xuất kho.
 */
function renderOutboundReadyTable() {
    var body = byId(ids.outboundReadyBody);
    if (!body) return;
    if (state.outboundReady.length === 0) {
        body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có lệnh chuẩn bị xuất</td></tr>';
        return;
    }

    var html = state.outboundReady
        .slice(0, 12)
        .map(function (item, stt) {
            return (
                "<tr " +
                rowDataAttr("outboundReadyRow", stt) +
                ">" +
                "<td><b>" +
                (stt + 1) +
                "</b></td>" +
                '<td style="text-align:left;font-weight:700">' +
                escapeHtml(shortMaLenh(item.MaLenhSanXuat || "")) +
                "</td>" +

                "<td>" +
                escapeHtml(formatDate(item.KHCat)) +
                "</td>" +
                "<td>" +
                escapeHtml(formatDate(item.DuKienCat)) +
                "</td>" +
                "</tr>"
            );
        })
        .join("");

    body.innerHTML = html;
}

/**
 * Vẽ bảng danh sách các Lệnh sản xuất đang được xuất kho thực tế.
 */
function renderOutboundRunningTable() {
    var body = byId(ids.outboundRunningBody);
    if (!body) return;
    if (state.outboundRunning.length === 0) {
        body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có lệnh đang xuất</td></tr>';
        return;
    }

    var html = state.outboundRunning
        .slice(0, 12)
        .map(function (item, stt) {
            return (
                "<tr " +
                rowDataAttr("outboundRunningRow", stt) +
                ">" +
                "<td><b>" +
                (stt + 1) +
                "</b></td>" +
                '<td style="text-align:left;font-weight:700">' +
                escapeHtml(shortMaLenh(item.MaLenhSX || "")) +
                "</td>" +
                '<td style="text-align:left">' +
                escapeHtml(item.TenHang || "") +
                "</td>" +
                '<td style="text-align:left">' +
                escapeHtml(normalizeCustomerName(item.TenKH)) +
                "</td>" +
                "<td>" +
                escapeHtml(formatDate(item.NgayXuatHang)) +
                "</td>" +
                '<td class="text-end">' +
                formatNumber(item.SLXuat, 2) +
                "</td>" +
                "</tr>"
            );
        })
        .join("");

    body.innerHTML = html;
}

function colsNhap() {
    return [
        { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
        { key: "PINCC", label: "PI NCC", center: true, width: "10%" },
        { key: "PO", label: "POMUA", center: true, width: "9%" },
        { key: "ItemCode", label: "Itemcode", center: true, width: "10%" },
        { key: "MaMauVT", label: "Mã màu VT", center: true, width: "7%" },
        { key: "MauVT", label: "Màu", center: true, width: "8%" },
        { key: "WidthSize", label: "Width/Size", center: true, width: "8%" },
        { key: "DonViVT", label: "Đơn vị vật tư", center: true, width: "7%" },
        { key: "TenKH", label: "Khách hàng", center: true, width: "20%" },
        { key: "SoLuong", label: "Số lượng", number: 2, center: true, width: "8%" },
        { key: "SoBarCode", label: "Số BarCode", number: 0, center: true, width: "8%" },
    ];
}

function colsXuat() {
    return [
        { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
        { key: "MaLenh", label: "Mã lệnh", center: true, width: "15%" },
        { key: "TenHang", label: "Tên hàng", center: true, width: "30%" },
        { key: "TenKH", label: "Khách hàng", center: true, width: "25%" },
        { key: "SoLuong", label: "Số lượng", number: 2, center: true, width: "12%" },
        { key: "SoBarCode", label: "Số BarCode", number: 0, center: true, width: "13%" },
    ];
}

function colsKiemKe() {
    return [
        { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
        { key: "PhieuKiemKe", label: "Phiếu KK", center: true, width: "15%" },
        { key: "SoLo", label: "Số lô", center: true, width: "15%" },
        { key: "SoBarCode", label: "Số BarCode", number: 0, center: true, width: "15%" },
        { key: "SoLuong", label: "SL kiểm kê", number: 2, center: true, width: "15%" },
        { key: "UserKK", label: "Người KK", center: true, width: "15%" },
        { key: "GhiChu", label: "Ghi chú", center: true, width: "20%" },
    ];
}

function colsPlanned() {
    return [
        { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
        { key: "SoLo", label: "PI NCC", center: true, width: "20%" },
        { key: "PO", label: "POMUA", center: true, width: "15%" },
        { key: "TenKH", label: "Khách hàng", center: true, width: "30%" },
        { key: "SoLuongDuKien", label: "Số lượng dự kiến", number: 0, center: true, width: "15%" },
        { key: "NgayNKDuKien", label: "Ngày dự kiến", date: true, center: true, width: "15%" },
    ];
}

/**
 * Vẽ Sơ đồ lấp đầy kệ kho (Heatmap).
 * Phân chia theo từng Module (Kho NL, Kho PL) và từng dãy. 
 * Màu sắc mỗi ô (tile) biểu thị mức độ lấp đầy (<50%, 50-85%, >85%, >100%).
 */
function renderRacksHeatmap() {
    var container = byId("warehouseMapContainer");
    if (!container) return;
    if (!state.racks || state.racks.length === 0) {
        container.innerHTML = '<div class="dk-empty">Không có dữ liệu kệ kho</div>';
        return;
    }

    var modules = {};
    var moduleOrder = [];
    // NL (1) trước, PL (2) sau
    for (var i = 0; i < state.racks.length; i++) {
        var r = state.racks[i];
        var mod = toNumber(r.Module);
        var mKey = mod === 1 ? 1 : mod === 2 ? 2 : 3;
        var mName = mod === 1 ? "Kho Nguyên Liệu (NL)" : mod === 2 ? "Kho Phụ Liệu (PL)" : "Module Khác";
        if (!modules[mKey]) {
            modules[mKey] = { name: mName, racks: [] };
            moduleOrder.push(mKey);
        }
        modules[mKey].racks.push({ item: r, index: i });
    }
    moduleOrder.sort(function (a, b) {
        return a - b;
    });

    var html = '<div class="dk-wh-grid-wrap">';

    for (var mi = 0; mi < moduleOrder.length; mi++) {
        var modKey = moduleOrder[mi];
        var modObj = modules[modKey];
        var racks = modObj.racks;

        racks.sort(function (a, b) {
            // Sắp xếp theo dãy trước, rồi theo % sử dụng
            var dayA = (a.item.TenDay || "").toLowerCase();
            var dayB = (b.item.TenDay || "").toLowerCase();
            if (dayA !== dayB) return dayA < dayB ? -1 : 1;
            var capA = toNumber(a.item.TongCBMTrongKe);
            var capB = toNumber(b.item.TongCBMTrongKe);
            var pA = capA > 0 ? (toNumber(a.item.TongCBMSuDungTrongKe) / capA) * 100 : 0;
            var pB = capB > 0 ? (toNumber(b.item.TongCBMSuDungTrongKe) / capB) * 100 : 0;
            return pB - pA;
        });

        // Thống kê module
        var totalRacks = racks.length;
        var under50 = 0,
            between50and85 = 0,
            above85 = 0,
            over100 = 0;
        var totalUsed = 0,
            totalCap = 0;
        for (var k = 0; k < racks.length; k++) {
            var ri = racks[k].item;
            var usd = toNumber(ri.TongCBMSuDungTrongKe);
            var cp = toNumber(ri.TongCBMTrongKe);
            var pt = cp > 0 ? (usd / cp) * 100 : 0;
            totalUsed += usd;
            totalCap += cp;
            if (pt > 100) over100++;
            else if (pt >= 85) above85++;
            else if (pt >= 50) between50and85++;
            else under50++;
        }
        var modPct = totalCap > 0 ? (totalUsed / totalCap) * 100 : 0;

        html += '<div class="dk-wh-module-section">';
        html +=
            '<div class="dk-wh-module-title">' +
            escapeHtml(modObj.name) +
            '<span class="dk-wh-module-stats">' +
            formatNumber(modPct, 1) +
            "% lấp đầy &nbsp;|&nbsp; " +
            under50 +
            " kệ &lt;50% &nbsp;|&nbsp; " +
            above85 +
            " kệ &gt;85% &nbsp;|&nbsp; " +
            over100 +
            " kệ &gt;100%" +
            "</span></div>";

        // Nhóm theo dãy
        var dayGroups = {};
        var dayOrder = [];
        for (var j2 = 0; j2 < racks.length; j2++) {
            var rk = racks[j2];
            var dayName = rk.item.TenDay || "Không xác định";
            if (!dayGroups[dayName]) {
                dayGroups[dayName] = [];
                dayOrder.push(dayName);
            }
            dayGroups[dayName].push(rk);
        }
        // Remove duplicates in dayOrder
        var seenDays = {};
        dayOrder = dayOrder.filter(function (d) {
            return seenDays[d] ? false : (seenDays[d] = true);
        });

        html += '<div class="dk-wh-days-grid">';
        for (var di = 0; di < dayOrder.length; di++) {
            var dayName2 = dayOrder[di];
            var dayRacks = dayGroups[dayName2];
            html += '<div class="dk-wh-day-section">';
            html +=
                '<div class="dk-bg-soft dk-text-body" style="font-size:11px;font-weight:700;margin-bottom:4px">' +
                escapeHtml(dayName2) +
                "</div>";
            html += '<div class="dk-wh-tile-grid">';

            for (var j = 0; j < dayRacks.length; j++) {
                var rackData = dayRacks[j];
                var rItem = rackData.item;
                var rIndex = rackData.index;
                var used = toNumber(rItem.TongCBMSuDungTrongKe);
                var cap = toNumber(rItem.TongCBMTrongKe);
                var pct = cap > 0 ? (used / cap) * 100 : 0;
                var slVatTu = toNumber(rItem.SLVatTu);

                var tileClass = "dk-wh-safe";
                if (pct > 100) tileClass = "dk-wh-over";
                else if (pct >= 85) tileClass = "dk-wh-full";
                else if (pct >= 50) tileClass = "dk-wh-warn";

                var tooltipText = [
                    "Kệ: " + (rItem.TenKe || ""),
                    "Dãy: " + (rItem.TenDay || ""),
                    "Module: " + (rItem.Module == 1 ? "NL" : rItem.Module == 2 ? "PL" : rItem.Module),
                    "Sử dụng: " + formatNumber(used, 1) + " / " + formatNumber(cap, 1) + " CBM",
                    "Tỷ lệ: " + formatNumber(pct, 1) + "%",
                    "Vật tư: " + formatNumber(slVatTu, 0) + " mã",
                ].join("\n");

                // v2.4.16 — Tile với fill animation từ dưới lên
                var fillH = Math.min(100, Math.max(0, pct));
                var staggerDelay = j * 40;
                html +=
                    '<div class="dk-wh-tile ' +
                    tileClass +
                    ' js-open-detail" data-detail="rackRow" data-index="' +
                    rIndex +
                    '" data-pct="' +
                    fillH.toFixed(1) +
                    '" title="' +
                    escapeHtml(tooltipText) +
                    '" style="animation-delay:' +
                    staggerDelay +
                    'ms">';
                html += '<span class="dk-wh-tile-fill" style="height:' + fillH.toFixed(1) + '%"></span>';
                if (pct >= 100) html += '<i class="fa-solid fa-triangle-exclamation dk-wh-tile-warn-icon"></i>';
                html += '<span class="dk-wh-tile-name">' + escapeHtml(rItem.TenKe || "Kệ") + "</span>";
                html += '<span class="dk-wh-tile-pct">' + formatNumber(pct, 0) + "%</span>";
                html += '<span class="dk-wh-tile-sl">' + formatNumber(slVatTu, 0) + " mã</span>";
                html += "</div>";
            }
            html += "</div></div>";
        }
        html += "</div>"; // close dk-wh-days-grid

        html += "</div>";
    }

    html += "</div>";
    container.innerHTML = html;
}

// ════════════════════════════════════════════════════════════════
// v2.4.6 — Render bảng Todo group-by-PO (header row + sub-rows)
// ════════════════════════════════════════════════════════════════
/**
 * Hàm vẽ cấu trúc bảng (Table) đặc biệt: Gom nhóm các dòng theo PO (Mua)
 * Có khả năng mở rộng/thu gọn (collapse/expand) các dòng con thuộc từng PO.
 * @param {Array} cols Cấu hình các cột (columns config)
 * @param {Array} rows Dữ liệu dòng (rows data)
 * @param {boolean} isTraHang Đánh dấu nếu là bảng trả hàng (để hiện thêm cột NCC)
 * @returns {string} Chuỗi HTML của bảng
 */
function renderTodoGroupedTable(cols, rows, isTraHang) {
    if (!rows || rows.length === 0) {
        return '<div class="dk-empty" style="padding:20px">Không có dữ liệu</div>';
    }
    // Group by POMua
    var groups = {};
    var poOrder = [];
    for (var i = 0; i < rows.length; i++) {
        var po = rows[i].POMua || "(Không có PO)";
        if (!groups[po]) {
            groups[po] = [];
            poOrder.push(po);
        }
        groups[po].push(rows[i]);
    }
    // Header
    var headHtml = "";
    for (var c = 0; c < cols.length; c++) {
        var col = cols[c];
        var styles = [];
        if (col.center) styles.push("text-align:center");
        else if (col.number !== undefined || col.percent) styles.push("text-align:right");
        if (col.width) styles.push("width:" + col.width + "px");
        headHtml +=
            "<th" + (styles.length ? ' style="' + styles.join(";") + '"' : "") + ">" + escapeHtml(col.label) + "</th>";
    }
    // Body
    var bodyHtml = "";
    for (var pi = 0; pi < poOrder.length; pi++) {
        var po = poOrder[pi];
        var items = groups[po];
        var ncc = isTraHang && items[0] ? items[0].NCC || "" : "";
        var nccText = ncc ? ' · <span class="dk-text-muted">NCC: ' + escapeHtml(ncc) + "</span>" : "";
        bodyHtml +=
            '<tr class="dk-todo-po-row" data-po="' +
            escapeHtml(po) +
            '">' +
            '<td colspan="' +
            cols.length +
            '">' +
            '<span class="dk-todo-po-toggle" data-po-toggle="' +
            escapeHtml(po) +
            '">' +
            '<i class="fa-solid fa-chevron-down"></i>' +
            "</span>" +
            '<a href="#" class="dk-todo-po-link" data-po-link="' +
            escapeHtml(po) +
            '">' +
            '<i class="fa-solid fa-file-invoice"></i> PO: <b>' +
            escapeHtml(po) +
            "</b>" +
            "</a>" +
            ' — <span class="dk-text-muted">' +
            items.length +
            " itemcode</span>" +
            nccText +
            "</td>" +
            "</tr>";
        for (var k = 0; k < items.length; k++) {
            var row = items[k];
            var rowHtml = "";
            for (var cc = 0; cc < cols.length; cc++) {
                var colC = cols[cc];
                var v = row[colC.key];
                var cellStyle = "";
                if (colC.raw) {
                    var alignR = colC.center ? "center" : "left";
                    rowHtml += '<td style="text-align:' + alignR + '">' + (v || "") + "</td>";
                    continue;
                }
                if (colC.number !== undefined) {
                    v = formatNumber(v, colC.number);
                    cellStyle = ' style="text-align:right;font-variant-numeric:tabular-nums"';
                } else if (colC.center) {
                    cellStyle = ' style="text-align:center"';
                } else if (colC.date) {
                    v = formatDate(v);
                    cellStyle = ' style="text-align:center"';
                }
                if (colC.key === "ItemCode")
                    cellStyle = ' style="text-align:left;font-family:Consolas,monospace;font-weight:600"';
                rowHtml += "<td" + cellStyle + ">" + escapeHtml(v) + "</td>";
            }
            bodyHtml += '<tr class="dk-todo-sub-row" data-po-sub="' + escapeHtml(po) + '">' + rowHtml + "</tr>";
        }
    }
    return (
        '<table class="dk-detail-table dk-todo-table"><thead><tr>' +
        headHtml +
        "</tr></thead><tbody>" +
        bodyHtml +
        "</tbody></table>"
    );
}

function renderTop5VTTable() {
    var body = byId("top5VTBody");
    if (!body) return;
    var rows = state.top5VT || [];
    if (rows.length === 0) {
        body.innerHTML = '<tr><td colspan="6" class="dk-empty">Chưa có dữ liệu</td></tr>';
        return;
    }
    body.innerHTML = rows
        .map(function (r) {
            var loaiColor = r.LoaiKho === "NL" ? "#2563eb" : r.LoaiKho === "PL" ? "#d97706" : "#6b7280";
            return (
                "<tr>" +
                '<td class="text-center">' +
                escapeHtml(r.STT) +
                "</td>" +
                '<td class="dk-cell-mono"><a href="#" class="dk-link-inline">' +
                escapeHtml(r.MaVT || "") +
                "</a></td>" +
                "<td>" +
                escapeHtml(r.TenVT || "") +
                "</td>" +
                '<td class="text-center"><span style="color:' +
                loaiColor +
                ';font-weight:700">' +
                escapeHtml(r.LoaiKho || "") +
                "</span></td>" +
                '<td class="text-end dk-cell-num">' +
                formatNumber(toNumber(r.CBM), 1) +
                "</td>" +
                '<td class="text-end dk-cell-num">' +
                formatNumber(toNumber(r.TyTrong), 1) +
                "%</td>" +
                "</tr>"
            );
        })
        .join("");
}

function renderTop5KHTable() {
    var body = byId("top5KHBody");
    if (!body) return;
    var rows = state.top5KH || [];
    if (rows.length === 0) {
        body.innerHTML = '<tr><td colspan="4" class="dk-empty">Chưa có dữ liệu</td></tr>';
        return;
    }
    body.innerHTML = rows
        .map(function (r) {
            return (
                "<tr>" +
                '<td class="text-center">' +
                escapeHtml(r.STT) +
                "</td>" +
                "<td>" +
                escapeHtml(r.KhachHang || "") +
                "</td>" +
                '<td class="text-end dk-cell-num">' +
                formatNumber(toNumber(r.GiaTri), 0) +
                "</td>" +
                '<td class="text-end dk-cell-num">' +
                formatNumber(toNumber(r.TyTrong), 1) +
                "%</td>" +
                "</tr>"
            );
        })
        .join("");
}

function renderHetHanTable() {
    var body = byId("hetHanBody");
    if (!body) return;
    var rows = state.vtHetHan || [];
    if (rows.length === 0) {
        body.innerHTML = '<tr><td colspan="4" class="dk-empty">Không có vật tư sắp hết hạn</td></tr>';
        return;
    }
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    body.innerHTML = rows
        .map(function (r) {
            var dt = r.NgayHetHan ? new Date(r.NgayHetHan) : null;
            var daysLeft = dt ? Math.round((dt - today) / 86400000) : 999;
            var dateCls = daysLeft <= 7 ? "dk-date-urgent" : daysLeft <= 14 ? "dk-date-warn" : "";
            var dateStr = dt ? formatDate(dt) : "—";
            var donVi = r.DonVi ? " " + escapeHtml(r.DonVi) : "";
            return (
                "<tr>" +
                '<td class="dk-cell-mono">' +
                escapeHtml(r.MaVT || "") +
                "</td>" +
                "<td>" +
                escapeHtml(r.TenVT || "") +
                "</td>" +
                '<td class="text-center ' +
                dateCls +
                '">' +
                dateStr +
                "</td>" +
                '<td class="text-end dk-cell-num">' +
                formatNumber(toNumber(r.TonKho), 1) +
                donVi +
                "</td>" +
                "</tr>"
            );
        })
        .join("");
}

// ════════════════════════════════════════════════════════════════
// v2.4.4 — Sortable table helper (cho Top5 VT)
// ════════════════════════════════════════════════════════════════
function renderSortableTable(cols, rows, opts) {
    opts = opts || {};
    var head = "<thead><tr>";
    for (var i = 0; i < cols.length; i++) {
        var c = cols[i];
        var thCls = c.sortable ? ' class="dk-sortable-th" data-sort-key="' + c.key + '"' : "";
        var thStyles = [];
        if (c.width) thStyles.push("width:" + c.width + "px");
        if (c.center) {
            thStyles.push("text-align:center");
        } else if (c.number !== undefined || c.percent) {
            thStyles.push("text-align:right");
        }
        var thStyleStr = thStyles.length ? ' style="' + thStyles.join(";") + '"' : "";
        head +=
            "<th" +
            thCls +
            thStyleStr +
            ">" +
            escapeHtml(c.label) +
            (c.sortable ? ' <i class="fa-solid fa-sort dk-sort-icon"></i>' : "") +
            "</th>";
    }
    head += "</tr></thead>";
    return (
        '<div class="dk-detail-table-wrap"><table class="dk-detail-table dk-sortable-table">' +
        head +
        "<tbody>" +
        sortableTbody(cols, rows, opts) +
        "</tbody></table></div>"
    );
}

function sortableTbody(cols, rows, opts) {
    return rows
        .map(function (r, idx) {
            var trCls = opts.highlightTopN && idx < opts.highlightTopN ? ' class="dk-row-highlight-top"' : "";
            return (
                "<tr" +
                trCls +
                ">" +
                cols
                    .map(function (c) {
                        var v = r[c.key];
                        if (c.drillTo && c.key === "MaVT") {
                            return (
                                '<td class="dk-cell-mono"><a href="#" class="dk-link-inline" data-mavt-drill="1">' +
                                escapeHtml(v || "") +
                                "</a></td>"
                            );
                        }
                        if (c.number !== undefined)
                            return '<td class="text-end dk-cell-num">' + formatNumber(v, c.number) + "</td>";
                        if (c.percent) return '<td class="text-end dk-cell-num">' + formatNumber(v, 2) + "%</td>";
                        if (c.center) return '<td class="text-center">' + escapeHtml(v) + "</td>";
                        return "<td>" + escapeHtml(v) + "</td>";
                    })
                    .join("") +
                "</tr>"
            );
        })
        .join("");
}

function wireSortableTable(wrapEl, cols, rows, opts) {
    if (!wrapEl) return;
    var ths = wrapEl.querySelectorAll(".dk-sortable-th");
    var sortKey = null,
        sortDir = 1;
    for (var i = 0; i < ths.length; i++) {
        ths[i].addEventListener("click", function () {
            var key = this.getAttribute("data-sort-key");
            if (sortKey === key) sortDir = -sortDir;
            else {
                sortKey = key;
                sortDir = -1;
            }
            rows.sort(function (a, b) {
                return (toNumber(a[sortKey]) - toNumber(b[sortKey])) * sortDir;
            });
            wrapEl.querySelectorAll(".dk-sort-icon").forEach(function (ic) {
                ic.className = "fa-solid fa-sort dk-sort-icon";
            });
            this.querySelector(".dk-sort-icon").className =
                "fa-solid " + (sortDir > 0 ? "fa-sort-up" : "fa-sort-down") + " dk-sort-icon active";
            wrapEl.querySelector("tbody").innerHTML = sortableTbody(cols, rows, opts);
        });
    }
}

/**
 * Vẽ biểu đồ vòng tròn (Donut Chart) thể hiện tỷ lệ sức chứa kho (Đã dùng NL, Đã dùng PL, Còn trống).
 * Hàm này dùng mã vẽ SVG tùy biến thay vì Highcharts để tạo hiệu ứng UI đặc thù.
 */
function renderCapacityChart() {
    var ringNode = byId(ids.chartCapacityRing);
    var legendNode = byId(ids.chartCapacityLegend);
    if (!ringNode || !legendNode) return;

    var overall = state.overall.length > 0 ? state.overall[0] : {};

    // v2.6.0 — Use pre-calculated per-warehouse percentages from API
    // PercentNPL = UsedNPL/CapacityNPL*100, PercentPL = UsedPL/CapacityPL*100
    var usedNpl = Math.max(0, toNumber(overall.UsedNPL));
    var usedPl = Math.max(0, toNumber(overall.UsedPL));
    var totalCapacity = Math.max(0, toNumber(overall.TotalCapacity));
    var capNpl = Math.max(0, toNumber(overall.CapacityNPL));
    var capPl = Math.max(0, toNumber(overall.CapacityPL));
    var totalUsed = usedNpl + usedPl;
    // Ring segments: proportion of TOTAL capacity (for donut visual)
    var segNpl, segPl, segFree, pctTotal;
    // Per-kho fill % (for legend labels, matching gauge)
    var fillNpl, fillPl;
    if (totalCapacity > 0) {
        segNpl = (usedNpl / totalCapacity) * 100;
        segPl = (usedPl / totalCapacity) * 100;
        pctTotal = segNpl + segPl;
        segFree = Math.max(0, 100 - pctTotal);
        fillNpl = capNpl > 0 ? (usedNpl / capNpl) * 100 : 0;
        fillPl = capPl > 0 ? (usedPl / capPl) * 100 : 0;
    } else {
        segNpl = 0;
        segPl = 0;
        segFree = 0;
        pctTotal = 0;
        fillNpl = 0;
        fillPl = 0;
    }

    if (segNpl <= 0 && segPl <= 0 && segFree <= 0) {
        ringNode.innerHTML = '<div class="dk-empty">Không có dữ liệu</div>';
        return;
    }

    // v2.4.3 — Segment màu sáng hơn ở dark (NL/PL/Còn trống)
    var _darkSeg = dkIsDark();
    var colNL = _darkSeg ? "#60a5fa" : "#0b5bf0";
    var colPL = _darkSeg ? "#fbbf24" : "#f5a623";
    var colFree = _darkSeg ? "#94a3b8" : "#9fb3c6";
    // Dùng % làm giá trị vẽ vòng tròn (tổng = 100)
    var segments = [
        { name: "Đã dùng NL", value: segNpl, color: colNL, label: formatNumber(segNpl, 2) + "%" },
        { name: "Đã dùng PL", value: segPl, color: colPL, label: formatNumber(segPl, 2) + "%" },
        { name: "Còn trống", value: segFree, color: colFree, label: formatNumber(segFree, 2) + "%" },
    ];
    var totalPct = segNpl + segPl + segFree;
    // Nếu tổng chưa đủ 100 (do làm tròn), điều chỉnh phần trống
    if (totalPct < 99.5) {
        segments[2].value = Math.max(0, 100 - segNpl - segPl);
        segments[2].label = formatNumber(segments[2].value, 2) + "%";
    }

    var svgWidth = 900;
    var svgHeight = 440;
    var cx = 450;
    var cy = 220;
    var r = 145;
    var strokeWidth = 42;
    var c = 2 * Math.PI * r;
    var dashOffset = 0;
    var circles = [];
    var calloutLines = [];
    var calloutBoxes = [];
    var startAngle = -Math.PI / 2;

    var rightLastY = -999;
    var leftLastY = -999;
    var MIN_GAP = 68;
    var boxW = 196;
    var boxH = 57;
    var MARGIN = 6;

    var defs = "<defs>";

    for (var i = 0; i < segments.length; i++) {
        var seg = segments[i];

        defs +=
            '<marker id="arrow_' +
            i +
            '" markerWidth="8" markerHeight="8" refX="7" refY="4" orient="auto"><polygon points="0 0, 8 4, 0 8" fill="' +
            seg.color +
            '"></polygon></marker>';

        if (seg.value <= 0) continue;

        var segLength = (seg.value / 100) * c;
        circles.push(
            '<circle cx="' +
            cx +
            '" cy="' +
            cy +
            '" r="' +
            r +
            '" fill="none" stroke="' +
            seg.color +
            '" stroke-width="' +
            strokeWidth +
            '" ' +
            'stroke-dasharray="' +
            segLength +
            " " +
            (c - segLength) +
            '" stroke-dashoffset="' +
            -dashOffset +
            '" transform="rotate(-90 ' +
            cx +
            " " +
            cy +
            ')" ' +
            'data-cap-seg="' +
            i +
            '" style="transition:opacity 0.2s;" />',
        );
        dashOffset += segLength;

        var segAngle = (seg.value / 100) * Math.PI * 2;
        var midAngle = startAngle + segAngle / 2;
        var direction = Math.cos(midAngle) >= 0 ? 1 : -1;

        var lineStartX = cx + Math.cos(midAngle) * (r + strokeWidth / 2);
        var lineStartY = cy + Math.sin(midAngle) * (r + strokeWidth / 2);
        var bendX = cx + Math.cos(midAngle) * (r + strokeWidth / 2 + 28);
        var bendY = cy + Math.sin(midAngle) * (r + strokeWidth / 2 + 28);

        if (direction > 0) {
            if (bendY - rightLastY < MIN_GAP) bendY = rightLastY + MIN_GAP;
            rightLastY = bendY;
        } else {
            if (bendY - leftLastY < MIN_GAP) bendY = leftLastY + MIN_GAP;
            leftLastY = bendY;
        }

        var boxX, boxCX, endX;
        if (direction > 0) {
            boxX = svgWidth - boxW - MARGIN;
            endX = boxX;
        } else {
            boxX = MARGIN;
            endX = boxX + boxW;
        }
        boxCX = boxX + boxW / 2;
        var boxY = bendY - boxH / 2;

        calloutLines.push(
            '<polyline points="' +
            lineStartX +
            "," +
            lineStartY +
            " " +
            bendX +
            "," +
            bendY +
            " " +
            endX +
            "," +
            bendY +
            '" fill="none" stroke="' +
            seg.color +
            '" stroke-width="2" marker-end="url(#arrow_' +
            i +
            ')"/>',
        );
        calloutBoxes.push(
            '<g class="cap-lbl" data-seg="' +
            i +
            '" style="cursor:pointer;">' +
            '<rect class="dk-callout-bg" x="' +
            boxX +
            '" y="' +
            boxY +
            '" width="' +
            boxW +
            '" height="' +
            boxH +
            '" fill="#ffffff" stroke="' +
            seg.color +
            '" stroke-width="2" rx="6" />' +
            '<text x="' +
            boxCX +
            '" y="' +
            bendY +
            '" text-anchor="middle" class="dk-capacity-callout-label" style="dominant-baseline:middle; fill:' +
            seg.color +
            '; font-size:14.9px;">' +
            '<tspan x="' +
            boxCX +
            '" dy="-0.6em" font-weight="bold">' +
            escapeHtml(seg.name) +
            "</tspan>" +
            '<tspan x="' +
            boxCX +
            '" dy="1.3em">' +
            escapeHtml(seg.label) +
            "</tspan>" +
            "</text>" +
            "</g>",
        );
        startAngle += segAngle;
    }
    defs += "</defs>";

    var innerR = r - strokeWidth / 2 - 2;
    var centerPct = pctTotal > 0 ? pctTotal : segNpl + segPl;
    // v2.4.2 — Ring track + inner hole theo theme (light=trắng, dark=navy đậm)
    var _dark = dkIsDark();
    var trackStroke = _dark ? "#1e3a6b" : "#e4edf6";
    var innerFill = _dark ? "#0d1d35" : "#ffffff";
    ringNode.innerHTML =
        '<svg viewBox="0 0 ' +
        svgWidth +
        " " +
        svgHeight +
        '" style="width:100%; height:100%;">' +
        defs +
        calloutLines.join("") +
        '<circle cx="' +
        cx +
        '" cy="' +
        cy +
        '" r="' +
        r +
        '" fill="none" stroke="' +
        trackStroke +
        '" stroke-width="' +
        strokeWidth +
        '" />' +
        circles.join("") +
        '<circle cx="' +
        cx +
        '" cy="' +
        cy +
        '" r="' +
        innerR +
        '" fill="' +
        innerFill +
        '" />' +
        '<text x="' +
        cx +
        '" y="' +
        (cy - 6) +
        '" class="dk-capacity-center" style="font-size:40px;">' +
        formatNumber(centerPct, 1) +
        "%</text>" +
        '<text x="' +
        cx +
        '" y="' +
        (cy + 28) +
        '" class="dk-flow-label" style="font-size:15px; text-anchor:middle;">Lấp đầy</text>' +
        calloutBoxes.join("") +
        "</svg>";
    legendNode.innerHTML = "";

    // Hover: highlight hovered segment, dim others
    var capSvg = ringNode.querySelector("svg");
    if (capSvg) {
        capSvg.addEventListener("mouseover", function (e) {
            var g = e.target;
            while (g && g !== capSvg) {
                if (g.classList && g.classList.contains("cap-lbl")) break;
                g = g.parentNode;
            }
            if (!g || g === capSvg) return;
            var idx = g.getAttribute("data-seg");
            var allSegs = capSvg.querySelectorAll("[data-cap-seg]");
            for (var s = 0; s < allSegs.length; s++) {
                allSegs[s].style.opacity = allSegs[s].getAttribute("data-cap-seg") === idx ? "1" : "0.2";
            }
        });
        capSvg.addEventListener("mouseout", function (e) {
            var g = e.target;
            while (g && g !== capSvg) {
                if (g.classList && g.classList.contains("cap-lbl")) break;
                g = g.parentNode;
            }
            if (!g || g === capSvg) return;
            var related = e.relatedTarget;
            if (related && g.contains && g.contains(related)) return;
            var allSegs = capSvg.querySelectorAll("[data-cap-seg]");
            for (var s = 0; s < allSegs.length; s++) {
                allSegs[s].style.opacity = "1";
            }
        });
    }
}

function renderCustomerPieChart() {
    var node = byId(ids.chartCustomerPie);
    if (!node) return;
    if (state.customers.length === 0) {
        node.innerHTML = '<div class="dk-empty">Không có dữ liệu khách hàng</div>';
        return;
    }

    // Feature 11: customer filter flag (declared at top to avoid hoisting issues)
    var filterActive = activeCustomerFilter !== "";

    // Hiện TẤT CẢ khách hàng (không giới hạn top 8)
    var data = buildCustomerDetailRows();
    var total = data.reduce(function (sum, row) {
        return sum + row.CBMSDTrongKho;
    }, 0);
    if (total <= 0) {
        node.innerHTML = '<div class="dk-empty">Không đủ dữ liệu CBM để vẽ biểu đồ</div>';
        return;
    }

    var n = data.length;
    var halfN = Math.ceil(n / 2);
    var MIN_GAP_DYN = Math.min(60, Math.max(38, Math.floor(460 / halfN)));
    var height = Math.max(550, Math.round((halfN * MIN_GAP_DYN + 120) * 1.1));
    var width = 860;
    var cx = width / 2;
    var cy = height / 2;
    var r = 138;
    var strokeWidth = 61;
    var c = 2 * Math.PI * r;
    // v2.4.3 — Palette sáng hơn ở dark (bỏ #1a3a6b dark-navy, dùng full bright)
    var colors = dkIsDark()
        ? [
            "#60a5fa",
            "#34d399",
            "#fbbf24",
            "#a78bfa",
            "#22d3ee",
            "#f87171",
            "#f472b6",
            "#5eead4",
            "#fb923c",
            "#c4b5fd",
            "#6ee7b7",
        ]
        : [
            "#1a3a6b",
            "#2f7fd9",
            "#f5a623",
            "#8d6be6",
            "#00a6c7",
            "#e34f70",
            "#718096",
            "#5ebf5e",
            "#d97706",
            "#7c3aed",
            "#059669",
        ];
    var offset = 0;
    var startAngle = -Math.PI / 2;

    var segments = [];
    var labels = [];
    var defs = [];
    var anchors = [];

    // Add shadow filter
    defs.push(
        '<filter id="pieShadow" x="-20%" y="-20%" width="140%" height="140%"><feDropShadow dx="0" dy="6" stdDeviation="8" flood-color="#10283f" flood-opacity="0.15" /></filter>',
    );

    for (var i = 0; i < data.length; i++) {
        var row = data[i];
        var ratio = row.CBMSDTrongKho / total;
        var segLength = ratio * c;
        var color = colors[i % colors.length];

        var segOpacity = !filterActive || row.MaKH === activeCustomerFilter ? "1" : "0.15";
        segments.push(
            '<circle cx="' +
            cx +
            '" cy="' +
            cy +
            '" r="' +
            r +
            '" fill="none" stroke="' +
            color +
            '" stroke-width="' +
            strokeWidth +
            '" ' +
            'stroke-dasharray="' +
            segLength +
            " " +
            Math.max(0, c - segLength) +
            '" stroke-dashoffset="' +
            -offset +
            '" transform="rotate(-90 ' +
            cx +
            " " +
            cy +
            ')" ' +
            'class="js-open-detail" data-cust-seg="' +
            i +
            '" style="cursor:pointer; filter:url(#pieShadow); transition:opacity 0.2s; opacity:' +
            segOpacity +
            ';" data-detail="customerRow" data-index="' +
            row.sourceIndex +
            '"><title>' +
            escapeHtml(row.TenKH) +
            "</title></circle>",
        );
        offset += segLength;

        // Chia đều: nửa đầu sang trái, nửa sau sang phải để tự cân bằng
        var half = Math.ceil(data.length / 2);
        var forcedSide = i < half ? -1 : 1;

        anchors.push({
            i: i,
            row: row,
            color: color,
            side: forcedSide,
            angle: startAngle + ratio * Math.PI,
            preferY: cy + Math.sin(startAngle + ratio * Math.PI) * (r + 40),
        });
        startAngle += ratio * Math.PI * 2;
    }

    function distributeBySide(items) {
        if (items.length === 0) return;
        items.sort(function (a, b) {
            return a.preferY - b.preferY;
        });
        var minY = 32;
        var maxY = height - 32;
        // Reduce gap if needed to fit all items
        var minGap = Math.min(MIN_GAP_DYN || 60, Math.floor((maxY - minY) / Math.max(items.length, 1)));
        minGap = Math.max(minGap, 22); // never less than 22px

        for (var j = 0; j < items.length; j++) {
            var prevY = j > 0 ? items[j - 1].labelY : minY - minGap;
            items[j].labelY = Math.max(items[j].preferY, prevY + minGap);
        }
        // If last item exceeds maxY, shift everything up proportionally
        var last = items[items.length - 1];
        if (last.labelY > maxY) {
            var overflow = last.labelY - maxY;
            for (var k = items.length - 1; k >= 0; k--) {
                items[k].labelY -= overflow;
                overflow = 0;
                if (k > 0 && items[k].labelY - items[k - 1].labelY < minGap) {
                    items[k - 1].labelY = items[k].labelY - minGap;
                    overflow = Math.max(0, minY - items[0].labelY);
                }
            }
            // Final clamp
            for (var m = 0; m < items.length; m++) {
                items[m].labelY = Math.max(
                    minY + m * minGap,
                    Math.min(maxY - (items.length - 1 - m) * minGap, items[m].labelY),
                );
            }
        }
    }

    var rightSide = anchors.filter(function (a) {
        return a.side > 0;
    });
    var leftSide = anchors.filter(function (a) {
        return a.side < 0;
    });
    distributeBySide(rightSide);
    distributeBySide(leftSide);

    var boxW = 182;
    var boxH = 54;
    var labelLines = []; // drawn BEFORE ring
    var labelBoxes = []; // drawn AFTER ring + inner fill

    for (var a = 0; a < anchors.length; a++) {
        var item = anchors[a];

        var edgeX = cx + Math.cos(item.angle) * (r + strokeWidth / 2 + 5);
        var edgeY = cy + Math.sin(item.angle) * (r + strokeWidth / 2 + 5);
        var radialX = cx + Math.cos(item.angle) * (r + strokeWidth / 2 + 35);
        var radialY = cy + Math.sin(item.angle) * (r + strokeWidth / 2 + 35);
        var elbowX = cx + item.side * (r + strokeWidth / 2 + 50);
        // Box anchored to SVG edge; arrow tip at box edge
        var boxX, boxCX, endX;
        if (item.side > 0) {
            boxX = width - boxW - 6;
            endX = boxX;
        } else {
            boxX = 6;
            endX = boxX + boxW;
        }
        boxCX = boxX + boxW / 2;
        var boxY = item.labelY - boxH / 2;

        // Smooth bezier line + dot at box end (no triangle arrow)
        var lineStartX = cx + Math.cos(item.angle) * (r + strokeWidth / 2);
        var lineStartY = cy + Math.sin(item.angle) * (r + strokeWidth / 2);
        // Control point for smooth curve
        var ctrlX = (radialX + elbowX) / 2;
        var ctrlY = (radialY + item.labelY) / 2;
        labelLines.push(
            '<path d="M ' +
            lineStartX +
            " " +
            lineStartY +
            " C " +
            radialX +
            " " +
            radialY +
            " " +
            ctrlX +
            " " +
            ctrlY +
            " " +
            endX +
            " " +
            item.labelY +
            '" fill="none" stroke="' +
            item.color +
            '" stroke-width="2" stroke-opacity="0.5" stroke-linecap="round" />' +
            '<circle cx="' +
            endX +
            '" cy="' +
            item.labelY +
            '" r="4" fill="' +
            item.color +
            '" fill-opacity="0.7" />',
        );
        var lbOpacity = !filterActive || item.row.MaKH === activeCustomerFilter ? "1" : "0.15";
        labelBoxes.push(
            '<g class="cust-lbl js-open-detail" data-seg="' +
            item.i +
            '" data-detail="customerRow" data-index="' +
            item.row.sourceIndex +
            '" style="cursor:pointer;opacity:' +
            lbOpacity +
            ';">' +
            '<rect class="dk-callout-bg" x="' +
            boxX +
            '" y="' +
            boxY +
            '" width="' +
            boxW +
            '" height="' +
            boxH +
            '" fill="#ffffff" fill-opacity="0.95" stroke="' +
            item.color +
            '" stroke-width="1.5" rx="10" filter="url(#pieShadow)" />' +
            '<text x="' +
            boxCX +
            '" y="' +
            item.labelY +
            '" text-anchor="middle" class="dk-customer-callout" style="dominant-baseline: middle; fill: ' +
            item.color +
            '; font-size: 14px;">' +
            '<tspan x="' +
            boxCX +
            '" dy="-0.7em" font-weight="800">' +
            escapeHtml(item.row.TenKH || item.row.MaKH) +
            "</tspan>" +
            '<tspan x="' +
            boxCX +
            '" dy="1.4em">' +
            formatNumber(item.row.TyTrongCBM, 1) +
            "% (" +
            formatNumber(item.row.CBMSDTrongKho, 1) +
            " CBM)</tspan>" +
            "</text>" +
            "</g>",
        );
    }

    var innerRCust = r - strokeWidth / 2 - 2;
    // v2.4.2 — track + inner hole theo theme
    var _darkC = dkIsDark();
    var trackStrokeC = _darkC ? "#1e3a6b" : "#e6edf5";
    var innerFillC = _darkC ? "#0d1d35" : "#ffffff";
    node.innerHTML =
        '<svg viewBox="0 0 ' +
        width +
        " " +
        height +
        '" style="width:100%; height:100%; display:block;" role="img" aria-label="Tỷ trọng khách hàng theo CBM">' +
        "<defs>" +
        defs.join("") +
        "</defs>" +
        labelLines.join("") +
        '<circle cx="' +
        cx +
        '" cy="' +
        cy +
        '" r="' +
        r +
        '" fill="none" stroke="' +
        trackStrokeC +
        '" stroke-width="' +
        strokeWidth +
        '"/>' +
        segments.join("") +
        '<circle cx="' +
        cx +
        '" cy="' +
        cy +
        '" r="' +
        innerRCust +
        '" fill="' +
        innerFillC +
        '" />' +
        '<text x="' +
        cx +
        '" y="' +
        (cy - 6) +
        '" text-anchor="middle" class="dk-capacity-center" style="font-size:42px;">' +
        state.customers.length +
        "</text>" +
        '<text x="' +
        cx +
        '" y="' +
        (cy + 26) +
        '" text-anchor="middle" class="dk-flow-label" style="font-size:16px;">Khách hàng</text>' +
        labelBoxes.join("") +
        "</svg>";

    // Hover: highlight hovered segment, dim others
    var svgEl = node.querySelector("svg");
    if (svgEl) {
        svgEl.addEventListener("mouseover", function (e) {
            var g = e.target;
            while (g && g !== svgEl) {
                if (g.classList && g.classList.contains("cust-lbl")) break;
                g = g.parentNode;
            }
            if (!g || g === svgEl) return;
            var idx = g.getAttribute("data-seg");
            var allSegs = svgEl.querySelectorAll("[data-cust-seg]");
            for (var s = 0; s < allSegs.length; s++) {
                allSegs[s].style.opacity = allSegs[s].getAttribute("data-cust-seg") === idx ? "1" : "0.2";
            }
        });
        svgEl.addEventListener("mouseout", function (e) {
            var g = e.target;
            while (g && g !== svgEl) {
                if (g.classList && g.classList.contains("cust-lbl")) break;
                g = g.parentNode;
            }
            if (!g || g === svgEl) return;
            var related = e.relatedTarget;
            if (related && g.contains && g.contains(related)) return;
            var allSegs = svgEl.querySelectorAll("[data-cust-seg]");
            for (var s = 0; s < allSegs.length; s++) {
                allSegs[s].style.opacity = "1";
            }
        });
    }
}

function buildFlowSeries() {
    var today = new Date();
    var months = [];
    var thisMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    var startMonth = addMonths(thisMonth, -11);
    for (var i = 0; i < 12; i++) {
        months.push(addMonths(startMonth, i));
    }

    var labels = [];
    var inboundValues = [];
    var outboundValues = [];
    var stockValues = [];

    if (state.flowTrend12T && state.flowTrend12T.length > 0) {
        var trend = state.flowTrend12T;
        var trendMap = {};
        for (var t = 0; t < trend.length; t++) {
            var tk = trend[t].Nam + "-" + String(trend[t].Thang).padStart(2, "0");
            trendMap[tk] = {
                inn: toNumber(trend[t].TotalIn),
                out: toNumber(trend[t].TotalOut),
                stock: toNumber(trend[t].TotalStock),
            };
        }
        for (var d = 0; d < months.length; d++) {
            var md = months[d];
            var mk = md.getFullYear() + "-" + String(md.getMonth() + 1).padStart(2, "0");
            var ml = String(md.getMonth() + 1).padStart(2, "0") + "/" + String(md.getFullYear()).slice(-2);
            labels.push(ml);
            var row = trendMap[mk] || { inn: 0, out: 0, stock: 0 };
            inboundValues.push(row.inn);
            outboundValues.push(row.out);
            stockValues.push(row.stock);
        }
    } else {
        for (var d2 = 0; d2 < months.length; d2++) {
            var md2 = months[d2];
            labels.push(String(md2.getMonth() + 1).padStart(2, "0") + "/" + String(md2.getFullYear()).slice(-2));
            inboundValues.push(0);
            outboundValues.push(0);
            stockValues.push(0);
        }
    }

    return {
        labels: labels,
        inbound: inboundValues,
        outbound: outboundValues,
        stock: stockValues,
    };
}

function renderFlowTrendChart() {
    var node = byId(ids.chartFlowTrend);
    if (!node) return;
    var series = buildFlowSeries();

    function getNiceMax(value) {
        if (!isFinite(value) || value <= 0) return 10;
        var exp = Math.pow(10, Math.floor(Math.log(value) / Math.LN10));
        var base = value / exp;
        var niceBase = base <= 1 ? 1 : base <= 2 ? 2 : base <= 5 ? 5 : 10;
        return niceBase * exp;
    }

    // Max cho cả 3 series: Nhập, Xuất, Tồn (cùng trục)
    var maxVal = 1;
    var allValues = series.inbound.concat(series.outbound).concat(series.stock);
    for (var i = 0; i < allValues.length; i++) {
        if (allValues[i] > maxVal) maxVal = allValues[i];
    }
    maxVal = getNiceMax(maxVal * 1.15);

    var W = 820,
        H = 320;
    var L = 60,
        R = 20,
        T = 30,
        B = 46;
    var plotW = W - L - R,
        plotH = H - T - B;
    var axisY = T + plotH;
    var N = series.labels.length;

    function groupX(idx) {
        if (N <= 1) return L + plotW / 2;
        var margin = 28;
        return L + margin + (idx / (N - 1)) * (plotW - margin * 2);
    }
    function yScale(v) {
        return T + (1 - v / maxVal) * plotH;
    }

    var groupStep = N > 1 ? plotW / (N - 1) : plotW;
    var barW = Math.max(12, Math.min(30, groupStep * 0.32));
    var gap = 3;

    var colorIn = "#5bb8f5";
    var colorOut = "#a78bfa";
    var colorLine = "#f97316";

    var defs =
        "<defs>" +
        '<linearGradient id="gradIn" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stop-color="#74c7f8"/><stop offset="100%" stop-color="#3d9de8"/></linearGradient>' +
        '<linearGradient id="gradOut" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stop-color="#c4b5fd"/><stop offset="100%" stop-color="#8b5cf6"/></linearGradient>' +
        // v2.4.10 — Gradient area dưới line tồn kho
        '<linearGradient id="gradStockArea" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stop-color="#f97316" stop-opacity="0.25"/><stop offset="100%" stop-color="#f97316" stop-opacity="0"/></linearGradient>' +
        '<filter id="bsf" x="-20%" y="-20%" width="140%" height="140%"><feDropShadow dx="0" dy="2" stdDeviation="2" flood-color="#0f172a" flood-opacity="0.12"/></filter>' +
        "</defs>";

    // Lưới — 1 trục Y duy nhất (số lượng)
    var grid = "";
    for (var g = 0; g <= 4; g++) {
        var gv = (maxVal / 4) * g;
        var gy = yScale(gv);
        grid +=
            '<line x1="' +
            L +
            '" y1="' +
            gy +
            '" x2="' +
            (W - R) +
            '" y2="' +
            gy +
            '" stroke="#e2eaf4" stroke-dasharray="4 3" stroke-width="1"/>';
        grid +=
            '<text x="' +
            (L - 4) +
            '" y="' +
            (gy + 4) +
            '" text-anchor="end" class="dk-flow-label">' +
            formatNumber(gv, 0) +
            "</text>";
    }
    grid +=
        '<line x1="' +
        L +
        '" y1="' +
        axisY +
        '" x2="' +
        (W - R) +
        '" y2="' +
        axisY +
        '" stroke="#b0c4d8" stroke-width="1.5"/>';

    var barsIn = "",
        barsOut = "";
    for (var bi = 0; bi < N; bi++) {
        var gx = groupX(bi);
        var yin = yScale(series.inbound[bi]);
        var hin = Math.max(2, axisY - yin);
        var yout = yScale(series.outbound[bi]);
        var hout = Math.max(2, axisY - yout);
        // v2.4.10 — class dk-flow-bar để animate scaleY grow + stagger delay
        barsIn +=
            '<rect class="dk-flow-bar dk-flow-bar-in"  style="transform-origin:' +
            (gx - barW - gap / 2 + barW / 2) +
            "px " +
            axisY +
            "px;animation-delay:" +
            bi * 25 +
            'ms" x="' +
            (gx - barW - gap / 2) +
            '" y="' +
            yin +
            '" width="' +
            barW +
            '" height="' +
            hin +
            '" rx="3" fill="url(#gradIn)"  filter="url(#bsf)"/>';
        barsOut +=
            '<rect class="dk-flow-bar dk-flow-bar-out" style="transform-origin:' +
            (gx + gap / 2 + barW / 2) +
            "px " +
            axisY +
            "px;animation-delay:" +
            (bi * 25 + 80) +
            'ms" x="' +
            (gx + gap / 2) +
            '" y="' +
            yout +
            '" width="' +
            barW +
            '" height="' +
            hout +
            '" rx="3" fill="url(#gradOut)" filter="url(#bsf)"/>';
    }

    // v2.4.10 — Smooth spline (Catmull-Rom-ish via cubic bezier) cho line tồn kho + area gradient dưới
    var pts = [];
    for (var pti = 0; pti < N; pti++) pts.push({ x: groupX(pti), y: yScale(series.stock[pti]) });
    var smoothD = "";
    if (pts.length > 0) {
        smoothD = "M " + pts[0].x + " " + pts[0].y;
        for (var si = 0; si < pts.length - 1; si++) {
            var p0 = pts[si === 0 ? 0 : si - 1];
            var p1 = pts[si];
            var p2 = pts[si + 1];
            var p3 = pts[si + 2 < pts.length ? si + 2 : pts.length - 1];
            var t = 0.18;
            var cp1x = p1.x + (p2.x - p0.x) * t;
            var cp1y = p1.y + (p2.y - p0.y) * t;
            var cp2x = p2.x - (p3.x - p1.x) * t;
            var cp2y = p2.y - (p3.y - p1.y) * t;
            smoothD +=
                " C " +
                cp1x.toFixed(1) +
                " " +
                cp1y.toFixed(1) +
                " " +
                cp2x.toFixed(1) +
                " " +
                cp2y.toFixed(1) +
                " " +
                p2.x +
                " " +
                p2.y;
        }
    }
    // Area path: theo line rồi đóng xuống axis
    var areaD = smoothD;
    if (pts.length > 0) {
        areaD += " L " + pts[pts.length - 1].x + " " + axisY + " L " + pts[0].x + " " + axisY + " Z";
    }
    var dots = "",
        stockLabels = "";
    for (var pj = 0; pj < N; pj++) {
        var px = pts[pj].x,
            py = pts[pj].y;
        dots +=
            '<circle class="dk-flow-dot" style="animation-delay:' +
            (pj * 40 + 400) +
            'ms" cx="' +
            px +
            '" cy="' +
            py +
            '" r="5" fill="#fff" stroke="' +
            colorLine +
            '" stroke-width="2.5" filter="url(#bsf)"/>';
        stockLabels +=
            '<text class="dk-flow-dot-label" x="' +
            px +
            '" y="' +
            (py - 10) +
            '" text-anchor="middle" style="font-size:10px;font-weight:700;fill:' +
            colorLine +
            ";animation-delay:" +
            (pj * 40 + 600) +
            'ms">' +
            formatNumber(series.stock[pj], 0) +
            "</text>";
    }
    var stockArea = pts.length > 1 ? '<path class="dk-flow-area" d="' + areaD + '" fill="url(#gradStockArea)"/>' : "";
    var stockLine =
        '<path class="dk-flow-line" d="' +
        smoothD +
        '" fill="none" stroke="' +
        colorLine +
        '" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" style="filter:drop-shadow(0 2px 4px rgba(249,115,22,0.35))"/>';

    var xLabels = series.labels
        .map(function (lbl, idx) {
            var lx = groupX(idx);
            var ly = axisY + 14;
            return (
                '<text x="' +
                lx +
                '" y="' +
                ly +
                '" text-anchor="middle" class="dk-flow-x-label" style="font-size:10.5px; fill:#5f758b; font-weight:600;">' +
                escapeHtml(lbl) +
                "</text>"
            );
        })
        .join("");

    var axisTitles =
        '<text x="' + (L - 4) + '" y="' + (T - 10) + '" text-anchor="end" class="dk-flow-axis-title">Số lượng</text>';

    node.innerHTML =
        '<div class="dk-flow-legend">' +
        '<span class="dk-flow-legend-item"><span class="dk-flow-legend-dot" style="background:#5bb8f5"></span>Nhập</span>' +
        '<span class="dk-flow-legend-item"><span class="dk-flow-legend-dot" style="background:#a78bfa"></span>Xuất</span>' +
        '<span class="dk-flow-legend-item"><span class="dk-flow-legend-dot" style="background:' +
        colorLine +
        '"></span>Tồn Kho</span>' +
        "</div>" +
        '<svg viewBox="0 0 ' +
        W +
        " " +
        H +
        '" style="width:100%; height:auto; display:block; overflow:visible;">' +
        defs +
        axisTitles +
        grid +
        barsIn +
        barsOut +
        stockArea +
        stockLine +
        dots +
        stockLabels +
        xLabels +
        "</svg>";
}

/**
 * Khởi tạo và cập nhật toàn bộ các biểu đồ Highcharts.
 */
function renderCharts() {
    renderCapacityChart();
    renderCapacityBarChart();
    renderCustomerPieChart();
    // Flow trend: default 30-day range (Issue 1, v2.3.5). Fall back to
    // old 12-month chart if the date-range picker isn't on the page yet
    // (e.g. demo/test pages).
    var fromEl = byId("flowFromDate"),
        toEl = byId("flowToDate");
    if (fromEl && toEl) {
        var today = new Date();
        var defStart = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);
        if (!fromEl.value) fromEl.value = asIsoDate(defStart);
        if (!toEl.value) toEl.value = asIsoDate(today);
        var from = new Date(fromEl.value + "T00:00:00");
        var to = new Date(toEl.value + "T00:00:00");
        loadAndRenderFlowByRange(from, to);
    } else {
        renderFlowTrendChart();
    }
    renderTop5MaxChart();
    renderTop5MinChart();
    renderAgeStockChart();
}

function renderTop5Single(nodeId, loaiNPL) {
    var node = byId(nodeId);
    if (!node) return;
    if (typeof Highcharts === "undefined") return;

    function getLabel(d) {
        var code = null;
        if (d.MaVT && String(d.MaVT).trim()) code = String(d.MaVT).trim();
        else if (d.MaNPL) {
            var parts = String(d.MaNPL).split("@");
            for (var p = 0; p < parts.length; p++) {
                if (parts[p].indexOf("MAVT_") === 0) {
                    code = parts[p];
                    break;
                }
            }
            if (!code) code = parts[0] || null;
        }
        var desc = d.TenVT || d.ChiTiet || "";
        if (code) return desc ? code + " — " + desc : code;
        return desc || "N/A";
    }

    function processRows(rows) {
        var raw = normalizeArray(rows);
        var arr = raw.filter(function (d) {
            return toNumber(d.TonKho) > 0;
        });
        arr.sort(function (a, b) {
            return toNumber(b.TonKho) - toNumber(a.TonKho);
        });
        var top5 = arr.slice(0, 5);

        // Lưu raw để modal chi tiết hiện 15 dòng
        if (loaiNPL === 1) state.top15MaxNL = raw;
        else state.top15MaxPL = raw;

        if (top5.length === 0) {
            node.innerHTML = '<div class="dk-empty">Không có dữ liệu</div>';
            return;
        }

        var cats = top5.map(getLabel);
        var data = top5.map(function (d) {
            return toNumber(d.TonKho);
        });
        var seriesName = loaiNPL === 1 ? "Nguyên liệu" : "Phụ liệu";
        var color = loaiNPL === 1 ? "#2563eb" : "#f97316";

        node.innerHTML = "";
        var dark = dkIsDark();
        var lblColor = dark ? "#cbd5e1" : "#1e3a5f";
        var axisColor = dark ? "#9fb3d1" : "#6b7280";
        var gridColor = dark ? "rgba(96, 165, 250, 0.15)" : "#e5e7eb";
        var dlColor = dark ? "#e2eaf5" : "#1f2937";
        Highcharts.chart(node, {
            chart: {
                type: "bar",
                backgroundColor: "transparent",
                style: { fontFamily: "'Segoe UI', Tahoma, Arial, sans-serif" },
            },
            title: { text: null },
            xAxis: {
                categories: cats,
                lineColor: gridColor,
                tickColor: gridColor,
                labels: { style: { fontSize: "11px", fontWeight: "600", color: lblColor } },
            },
            yAxis: {
                min: 0,
                title: { text: "Số lượng tồn", style: { color: axisColor } },
                labels: { style: { color: axisColor } },
                gridLineDashStyle: "ShortDash",
                gridLineColor: gridColor,
            },
            legend: { enabled: false },
            tooltip: { pointFormat: seriesName + ": <b>{point.y:,.0f}</b><br/>" },
            plotOptions: {
                bar: {
                    borderRadius: 3,
                    dataLabels: {
                        enabled: true,
                        style: {
                            fontWeight: "700",
                            fontSize: "11px",
                            color: dlColor,
                            textOutline: dark ? "1px rgba(0,0,0,0.6)" : "1px rgba(255,255,255,0.6)",
                        },
                        formatter: function () {
                            return Highcharts.numberFormat(this.y, 0);
                        },
                    },
                },
            },
            series: [{ name: seriesName, data: data, color: color }],
            credits: { enabled: false },
        });
    }

    var cached = loaiNPL === 1 ? state.top15MaxNL : state.top15MaxPL;
    if (cached && cached.length > 0) {
        processRows(cached);
    } else {
        node.innerHTML = '<div class="dk-empty">Đang tải...</div>';
        requestJson("/api/DashboardKhoDesktop/GetTop5?isNhieuNhat=1&loaiNPL=" + loaiNPL)
            .then(function (rows) {
                processRows(rows);
            })
            .catch(function () {
                node.innerHTML = '<div class="dk-empty">Lỗi tải dữ liệu</div>';
            });
    }
}

function renderTop5MaxNLChart() {
    renderTop5Single("chartTop5MaxNL", 1);
}

function renderTop5MaxPLChart() {
    renderTop5Single("chartTop5MaxPL", 2);
}

function renderTop5MaxChart() {
    renderTop5MaxNLChart();
    renderTop5MaxPLChart();
}

function renderTop5MinChart() {
    /* removed in v2.3.18 */
}

function renderAgeStockChart() {
    var node = byId(ids.chartAgeStock);
    if (!node) return;
    if (typeof Highcharts === "undefined") return;
    if (!state.ageStock || state.ageStock.length === 0) {
        node.innerHTML = '<div class="dk-empty">Không có dữ liệu tuổi tồn kho</div>';
        return;
    }

    var buckets = [
        { label: "< 3 tháng", color: "#22c55e", min: 0, max: 2 },
        { label: "3-6 tháng", color: "#eab308", min: 3, max: 5 },
        { label: "6-12 tháng", color: "#f97316", min: 6, max: 11 },
        { label: "> 12 tháng", color: "#ef4444", min: 12, max: 9999 },
    ];

    var groups = {};
    var groupOrder = [];
    for (var ai = 0; ai < state.ageStock.length; ai++) {
        var ag = state.ageStock[ai];
        var nhom = ag.TenNhom || "Khác";
        var thang = toNumber(ag.Thang);
        var qty = toNumber(ag.TonKhoCT);
        if (!groups[nhom]) {
            groups[nhom] = {};
            groupOrder.push(nhom);
        }
        var bLabel = null;
        for (var bi = 0; bi < buckets.length; bi++) {
            if (thang >= buckets[bi].min && thang <= buckets[bi].max) {
                bLabel = buckets[bi].label;
                break;
            }
        }
        if (!bLabel) bLabel = "> 12 tháng";
        groups[nhom][bLabel] = (groups[nhom][bLabel] || 0) + qty;
    }

    var uniqueGroups = [];
    var seen = {};
    for (var gi = 0; gi < groupOrder.length; gi++) {
        if (!seen[groupOrder[gi]]) {
            uniqueGroups.push(groupOrder[gi]);
            seen[groupOrder[gi]] = true;
        }
    }

    var series = buckets.map(function (b) {
        return {
            name: b.label,
            color: b.color,
            data: uniqueGroups.map(function (g) {
                return groups[g][b.label] || 0;
            }),
        };
    });

    node.innerHTML = "";
    applyHighchartsTheme();
    Highcharts.chart(node, {
        chart: { type: "column", backgroundColor: "transparent" },
        title: { text: null },
        xAxis: { categories: uniqueGroups },
        yAxis: { min: 0, title: { text: "Số lượng" }, stackLabels: { enabled: true }, gridLineDashStyle: "ShortDash" },
        tooltip: {
            useHTML: true,
            backgroundColor: "#fff",
            borderRadius: 10,
            shadow: true,
            formatter: function () {
                var nhom = this.x;
                var bLabel = this.series.name;
                // Find individual months in this bucket for this nhom
                var monthDetails = [];
                for (var _ai = 0; _ai < state.ageStock.length; _ai++) {
                    var _r = state.ageStock[_ai];
                    if ((_r.TenNhom || "Khác") !== nhom) continue;
                    var _t = toNumber(_r.Thang);
                    var _b = null;
                    for (var _bi = 0; _bi < buckets.length; _bi++) {
                        if (_t >= buckets[_bi].min && _t <= buckets[_bi].max) {
                            _b = buckets[_bi].label;
                            break;
                        }
                    }
                    if (!_b) _b = "> 12 tháng";
                    if (_b !== bLabel) continue;
                    monthDetails.push({ thang: _t, qty: toNumber(_r.TonKhoCT) });
                }
                monthDetails.sort(function (a, b) {
                    return a.thang - b.thang;
                });
                var rows = monthDetails
                    .map(function (m) {
                        return (
                            "<tr><td style='padding:1px 6px;color:#64748b;font-size:10px;'>Tháng " +
                            m.thang +
                            "</td><td style='padding:1px 6px;font-weight:700;text-align:right;'>" +
                            formatNumber(m.qty, 0) +
                            "</td></tr>"
                        );
                    })
                    .join("");
                return (
                    "<div style='font-size:11px;min-width:160px;'>" +
                    "<div style='font-weight:800;color:#1e3a5f;margin-bottom:4px;'>" +
                    escapeHtml(nhom) +
                    "</div>" +
                    "<div style='color:" +
                    this.series.color +
                    ";font-weight:700;margin-bottom:4px;'>" +
                    bLabel +
                    ": <b>" +
                    formatNumber(this.y, 0) +
                    "</b></div>" +
                    (rows
                        ? "<table style='border-collapse:collapse;width:100%;'><thead><tr><th style='font-size:9px;color:#94a3b8;text-align:left;padding:0 6px;'>Tháng</th><th style='font-size:9px;color:#94a3b8;text-align:right;padding:0 6px;'>SL</th></tr></thead><tbody>" +
                        rows +
                        "</tbody></table>"
                        : "") +
                    "</div>"
                );
            },
        },
        plotOptions: { column: { stacking: "normal", borderRadius: 3, borderWidth: 0 } },
        legend: { enabled: true, itemStyle: { fontSize: "11px", fontWeight: "600" } },
        series: series,
        credits: { enabled: false },
    });
}

// ─── Feature 7: MoM Comparison Strip ────────────────────────────────────────
function renderMoMStrip() {
    var strip = byId("momStrip");
    if (!strip) return;
    var mom = state.momComparison || [];
    var thisRow = null,
        lastRow = null;
    for (var mi = 0; mi < mom.length; mi++) {
        var k = String(mom[mi].KieuKy || mom[mi].kieuKy || "")
            .trim()
            .toLowerCase();
        if (k === "thismonth") thisRow = mom[mi];
        else if (k === "lastmonth") lastRow = mom[mi];
    }
    if (!thisRow && !lastRow) {
        var now = new Date();
        var lastM = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        thisRow = { Nam: now.getFullYear(), Thang: now.getMonth() + 1, TotalIn: 0, TotalOut: 0 };
        lastRow = { Nam: lastM.getFullYear(), Thang: lastM.getMonth() + 1, TotalIn: 0, TotalOut: 0 };
    }
    var curIn = thisRow ? toNumber(thisRow.TotalIn) : 0;
    var curOut = thisRow ? toNumber(thisRow.TotalOut) : 0;
    var prvIn = lastRow ? toNumber(lastRow.TotalIn) : 0;
    var prvOut = lastRow ? toNumber(lastRow.TotalOut) : 0;
    var curMonth = thisRow ? thisRow.Thang + "/" + thisRow.Nam : "";
    var prvMonth = lastRow ? lastRow.Thang + "/" + lastRow.Nam : "";

    function deltaHtml(cur, prv) {
        if (prv <= 0) return "";
        var pct = ((cur - prv) / prv) * 100;
        var cls = pct > 2 ? "up" : pct < -2 ? "down" : "flat";
        var arrow = pct > 2 ? "▲" : pct < -2 ? "▼" : "→";
        return '<span class="dk-mom-delta ' + cls + '">' + arrow + " " + formatNumber(Math.abs(pct), 1) + "%</span>";
    }

    strip.innerHTML =
        '<div class="dk-mom-block dk-mom-in">' +
        '  <span class="dk-mom-icon">📥</span>' +
        '  <div><div class="dk-mom-label">Nhập kho — ' +
        escapeHtml(curMonth) +
        "</div>" +
        '    <div class="dk-mom-values">' +
        '      <span class="dk-mom-cur">' +
        formatNumber(curIn, 2) +
        "</span>" +
        '      <span class="dk-mom-prev">Tháng trước: ' +
        formatNumber(prvIn, 2) +
        "</span>" +
        "      " +
        deltaHtml(curIn, prvIn) +
        "    </div></div></div>" +
        '<div class="dk-mom-block dk-mom-out">' +
        '  <span class="dk-mom-icon">📤</span>' +
        '  <div><div class="dk-mom-label">Xuất kho — ' +
        escapeHtml(curMonth) +
        "</div>" +
        '    <div class="dk-mom-values">' +
        '      <span class="dk-mom-cur">' +
        formatNumber(curOut, 2) +
        "</span>" +
        '      <span class="dk-mom-prev">Tháng trước: ' +
        formatNumber(prvOut, 2) +
        "</span>" +
        "      " +
        deltaHtml(curOut, prvOut) +
        "    </div></div></div>";
}

// Biểu đồ mới phần Lịch phân công
// Biểu đồ mới phần Lịch phân công
function renderLpcpBottomCharts() {
    var isDark = document.body.classList.contains("dark-theme");
    var textColor = isDark ? "#f8fafc" : "#1e293b"; // Đậm hơn ở light mode, sáng hơn ở dark mode
    var gridColor = isDark ? "rgba(255,255,255,0.05)" : "#e2e8f0";
    var bgColor = isDark ? "#1e293b" : "#ffffff";

    // Hàm tiện ích
    function _asIso(d) {
        return (
            d.getFullYear() +
            "-" +
            String(d.getMonth() + 1).padStart(2, "0") +
            "-" +
            String(d.getDate()).padStart(2, "0")
        );
    }

    var now = new Date();
    function buildRange(nDays) {
        var to = new Date(now);
        to.setDate(to.getDate() - 1);
        var from = new Date(now);
        from.setDate(from.getDate() - nDays);
        return { from: _asIso(from), to: _asIso(to) };
    }
    function doFetch(url) {
        return typeof requestJson === "function"
            ? requestJson(url)
            : fetch(url).then(function (r) {
                return r.json();
            });
    }

    if (document.getElementById("chartVolumePie")) {
        var volDays = document.getElementById("volumeFilterSelect")
            ? parseInt(document.getElementById("volumeFilterSelect").value)
            : 30;
        var rVol = buildRange(volDays);
        var urlAct = "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" + rVol.from + "&denNgay=" + rVol.to;
        var urlNkdk = "/api/DashboardKhoDesktop/GetNKDuKienByRange?tuNgay=" + rVol.from + "&denNgay=" + rVol.to;

        document.getElementById("chartVolumePie").innerHTML =
            '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:' +
            textColor +
            ';font-size:12px;">Đang tải...</div>';

        Promise.all([doFetch(urlAct), doFetch(urlNkdk)])
            .then(function (results) {
                var actArr = Array.isArray(results[0])
                    ? results[0]
                    : results[0] && results[0].value
                        ? results[0].value
                        : results[0] && results[0].data
                            ? results[0].data
                            : [];
                var nkArr = Array.isArray(results[1])
                    ? results[1]
                    : results[1] && results[1].value
                        ? results[1].value
                        : results[1] && results[1].data
                            ? results[1].data
                            : [];

                var sumIn = 0,
                    sumOut = 0,
                    sumKiemKe = 0,
                    sumPlan = 0;

                actArr.forEach(function (d) {
                    sumIn += toNumber(d.TotalIn || d.totalIn);
                    sumOut += toNumber(d.TotalOut || d.totalOut);
                    sumKiemKe += toNumber(d.TotalKiemKe || d.totalKiemKe);
                });

                nkArr.forEach(function (d) {
                    sumPlan += toNumber(d.SoLuongDuKien || d.soLuongDuKien);
                });

                var sumTotal = sumIn + sumOut + sumKiemKe + sumPlan;

                var pieData = [
                    { name: "Nhập kho", y: sumIn, color: "#3b82f6" },
                    { name: "Xuất kho", y: sumOut, color: "#f97316" },
                    { name: "Kiểm kê", y: sumKiemKe, color: "#10b981" },
                    { name: "NK dự kiến", y: sumPlan, color: "#8b5cf6" },
                ];

                if (!document.getElementById("chartVolumePie")) return;
                var isFsOnLoad = !!document.getElementById("chartVolumePie").closest(".dk-panel-fullscreen");

                Highcharts.chart("chartVolumePie", {
                    chart: {
                        type: "pie",
                        backgroundColor: "transparent",
                        events: {
                            render: function () {
                                var chart = this;
                                var series = chart.series[0];
                                if (series && series.center) {
                                    var isDk = document.body.classList.contains("dark-theme");
                                    var txtCol = isDk ? "#f8fafc" : "#1e293b";
                                    var cx = chart.plotLeft + series.center[0];
                                    var cy = chart.plotTop + series.center[1];
                                    if (chart.centerLabel) {
                                        chart.centerLabel.destroy();
                                        chart.centerLabel = null;
                                    }
                                    var isFs = !!document
                                        .getElementById("chartVolumePie")
                                        .closest(".dk-panel-fullscreen");
                                    var labelFontSize = isFs ? "30px" : "18px";
                                    var valueFontSize = isFs ? "60px" : "26px";
                                    var offset = isFs ? 40 : 20;
                                    chart.centerLabel = chart.renderer
                                        .text(
                                            '<div style="text-align:center;font-size:' +
                                            labelFontSize +
                                            ";line-height:1.2;color:" +
                                            txtCol +
                                            ';">Tổng<br><span style="font-size:' +
                                            valueFontSize +
                                            ';font-weight:700;">' +
                                            Highcharts.numberFormat(sumTotal, 0, ".", ".") +
                                            "</span></div>",
                                            cx,
                                            cy,
                                            true,
                                        )
                                        .attr({ align: "center", zIndex: 10 })
                                        .add();
                                    chart.centerLabel.attr({ x: cx, y: cy - offset });
                                }
                            },
                        },
                    },
                    title: { text: null },
                    credits: { enabled: false },
                    plotOptions: {
                        pie: {
                            innerSize: "75%",
                            size: isFsOnLoad ? "95%" : "80%",
                            borderWidth: 0,
                            dataLabels: { enabled: false },
                            showInLegend: true,
                            center: isFsOnLoad ? ["40%", "50%"] : ["35%", "50%"],
                        },
                    },
                    legend: {
                        layout: "vertical",
                        align: "right",
                        x: 0,
                        verticalAlign: "middle",
                        y: 0,
                        itemStyle: { color: textColor, fontWeight: "600", fontSize: isFsOnLoad ? "26px" : "13px" },
                        itemMarginTop: isFsOnLoad ? 10 : 0,
                        itemMarginBottom: isFsOnLoad ? 10 : 0,
                        useHTML: true,
                        labelFormatter: function () {
                            var isDk = document.body.classList.contains("dark-theme");
                            var txtCol = isDk ? "#f8fafc" : "#1e293b";
                            var pct = sumTotal > 0 ? ((this.y / sumTotal) * 100).toFixed(1) : 0;
                            return '<span style="color:' + txtCol + ';">' + this.name + ": <b>" + pct + "%</b></span>";
                        },
                    },
                    series: [
                        {
                            name: "Khối lượng",
                            data: pieData,
                            showInLegend: true,
                        },
                    ],
                    tooltip: {
                        pointFormat: "<b>{point.y:,.0f}</b> ({point.percentage:.1f}%)",
                    },
                });
            })
            .catch(function () {
                var el = document.getElementById("chartVolumePie");
                if (el) el.innerHTML = '<div style="padding:20px;color:' + textColor + ';">Không thể tải dữ liệu</div>';
            });
    }

    // --- Biểu đồ 2 & 3: Lấy trực tiếp từ SQL API theo range được chọn ---
    var trendDays = document.getElementById("trendFilterSelect")
        ? parseInt(document.getElementById("trendFilterSelect").value)
        : 30;
    var loadDays = document.getElementById("loadFilterSelect")
        ? parseInt(document.getElementById("loadFilterSelect").value)
        : 30;

    // ── Biểu đồ 2: Xu hướng Nhập-Xuất ─────────────────────────────────────────
    if (document.getElementById("chartTrendLine")) {
        var rTrend = buildRange(trendDays);
        var urlTrend = "/api/DashboardKhoDesktop/GetFlowTrendByRange?tuNgay=" + rTrend.from + "&denNgay=" + rTrend.to;
        document.getElementById("chartTrendLine").innerHTML =
            '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:' +
            textColor +
            ';font-size:12px;">Đang tải...</div>';

        doFetch(urlTrend)
            .then(function (data) {
                var arr = Array.isArray(data) ? data : data && data.value ? data.value : [];
                var cats = [],
                    inArr = [],
                    outArr = [];
                for (var j = 0; j < arr.length; j++) {
                    var ngay = String(arr[j].Ngay || arr[j].ngay || "").substring(0, 10);
                    var pts = ngay.split("-");
                    cats.push(pts.length === 3 ? pts[2] + "/" + pts[1] : ngay);
                    inArr.push(toNumber(arr[j].TotalIn || arr[j].totalIn));
                    outArr.push(toNumber(arr[j].TotalOut || arr[j].totalOut));
                }
                if (!document.getElementById("chartTrendLine")) return;
                Highcharts.chart("chartTrendLine", {
                    chart: { type: "spline", backgroundColor: bgColor, spacingTop: 20, spacingBottom: 15 },
                    title: { text: "" },
                    xAxis: {
                        categories: cats,
                        labels: { style: { color: textColor, fontSize: "14px" } },
                        tickLength: 0,
                        lineColor: isDark ? "#334155" : "#e2e8f0",
                    },
                    yAxis: {
                        title: { text: "Khối lượng (cuộn/m)", style: { fontSize: "13px" } },
                        labels: {
                            style: { color: textColor, fontSize: "14px" },
                            formatter: function () {
                                var v = this.value;
                                return v >= 1000000
                                    ? (v / 1000000).toFixed(1) + "M"
                                    : v >= 1000
                                        ? (v / 1000).toFixed(0) + "K"
                                        : v;
                            },
                        },
                        gridLineColor: isDark ? "#334155" : "#e2e8f0",
                        min: 0,
                    },
                    legend: {
                        layout: "horizontal",
                        align: "center",
                        verticalAlign: "bottom",
                        y: 0,
                        itemStyle: { color: textColor, fontWeight: "600", fontSize: "14px" },
                    },
                    tooltip: {
                        shared: true,
                        formatter: function () {
                            var s = "<b>" + this.points[0].key + "</b><br/>";
                            this.points.forEach(function (p) {
                                s +=
                                    '<span style="color:' +
                                    p.color +
                                    '">\u25CF</span> ' +
                                    p.series.name +
                                    ": <b>" +
                                    Highcharts.numberFormat(p.y, 0, ".", ",") +
                                    "</b><br/>";
                            });
                            return s;
                        },
                    },
                    plotOptions: { spline: { marker: { radius: 3, symbol: "circle" }, lineWidth: 2 } },
                    series: [
                        { name: "Nhập kho", data: inArr, color: "#3b82f6" },
                        { name: "Xuất kho", data: outArr, color: "#f97316" },
                    ],
                    credits: { enabled: false },
                });
            })
            .catch(function () {
                var el = document.getElementById("chartTrendLine");
                if (el) el.innerHTML = '<div style="padding:20px;color:' + textColor + ';">Không thể tải dữ liệu</div>';
            });
    }

    // ── Biểu đồ 3: Tồn kho theo ngày (GetFlowTrendByRange → TotalStock) ────────
    if (document.getElementById("chartLoadBar")) {
        var rLoad = buildRange(loadDays);
        var urlLoad = "/api/DashboardKhoDesktop/GetFlowTrendByRange?tuNgay=" + rLoad.from + "&denNgay=" + rLoad.to;
        document.getElementById("chartLoadBar").innerHTML =
            '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:' +
            textColor +
            ';font-size:12px;">Đang tải...</div>';

        doFetch(urlLoad)
            .then(function (data) {
                var arr = Array.isArray(data) ? data : data && data.value ? data.value : [];
                var cats = [],
                    vals = [],
                    colors = [];
                var maxStock = 0;
                for (var j = 0; j < arr.length; j++) {
                    var s = toNumber(arr[j].TotalStock || arr[j].totalStock);
                    if (s > maxStock) maxStock = s;
                }
                for (var j = 0; j < arr.length; j++) {
                    var ngay = String(arr[j].Ngay || arr[j].ngay || "").substring(0, 10);
                    var pts = ngay.split("-");
                    cats.push(pts.length === 3 ? pts[2] + "/" + pts[1] : ngay);
                    var stock = toNumber(arr[j].TotalStock || arr[j].totalStock);
                    vals.push(stock);
                    var pct = maxStock > 0 ? (stock / maxStock) * 100 : 0;
                    colors.push(pct >= 95 ? "#ef4444" : pct >= 80 ? "#eab308" : "#22c55e");
                }
                if (!document.getElementById("chartLoadBar")) return;
                var pw = loadDays <= 7 ? 28 : loadDays <= 14 ? 16 : loadDays <= 30 ? 8 : 4;
                Highcharts.chart("chartLoadBar", {
                    chart: { type: "column", backgroundColor: bgColor, spacingTop: 20, spacingBottom: 15 },
                    title: { text: "" },
                    xAxis: {
                        categories: cats,
                        labels: { style: { color: textColor, fontSize: "14px" } },
                        tickLength: 0,
                        lineColor: isDark ? "#334155" : "#e2e8f0",
                    },
                    yAxis: {
                        title: { text: "Tồn kho (cuộn/m)", style: { fontSize: "13px" } },
                        labels: {
                            style: { color: textColor, fontSize: "14px" },
                            formatter: function () {
                                var v = this.value;
                                return v >= 1000000
                                    ? (v / 1000000).toFixed(1) + "M"
                                    : v >= 1000
                                        ? (v / 1000).toFixed(0) + "K"
                                        : v;
                            },
                        },
                        gridLineColor: isDark ? "#334155" : "#e2e8f0",
                        min: 0,
                    },
                    legend: { enabled: false },
                    tooltip: {
                        formatter: function () {
                            return (
                                "<b>" +
                                this.key +
                                "</b><br/>Tồn kho: <b>" +
                                Highcharts.numberFormat(this.y, 0, ".", ",") +
                                "</b>"
                            );
                        },
                    },
                    plotOptions: {
                        column: {
                            borderRadius: 4,
                            borderWidth: 0,
                            maxPointWidth: 28,
                            pointWidth: pw,
                            colorByPoint: true,
                            colors: colors,
                        },
                    },
                    series: [
                        {
                            name: "Tồn kho",
                            data: vals,
                            dataLabels: {
                                enabled: loadDays <= 14,
                                formatter: function () {
                                    var v = this.y;
                                    return v >= 1000000
                                        ? (v / 1000000).toFixed(1) + "M"
                                        : v >= 1000
                                            ? (v / 1000).toFixed(0) + "K"
                                            : String(v);
                                },
                                style: {
                                    color: textColor,
                                    fontSize: "14px",
                                    fontWeight: "normal",
                                    textOutline: "none",
                                },
                            },
                        },
                    ],
                    credits: { enabled: false },
                });
            })
            .catch(function () {
                var el = document.getElementById("chartLoadBar");
                if (el) el.innerHTML = '<div style="padding:20px;color:' + textColor + ';">Không thể tải dữ liệu</div>';
            });
    }
}

function renderFlowTrendQuarter() {
    if (!state.flowTrend12T || state.flowTrend12T.length === 0) {
        renderFlowTrendChart();
        return;
    }
    var quarterMap = {};
    var quarterOrder = [];
    for (var i = 0; i < state.flowTrend12T.length; i++) {
        var t = state.flowTrend12T[i];
        var q = Math.ceil(toNumber(t.Thang) / 3);
        var key = "Q" + q + "/" + String(t.Nam).slice(-2);
        if (!quarterMap[key]) {
            quarterMap[key] = { lbl: key, inn: 0, out: 0, stock: 0 };
            quarterOrder.push(key);
        }
        quarterMap[key].inn += toNumber(t.TotalIn);
        quarterMap[key].out += toNumber(t.TotalOut);
        quarterMap[key].stock = toNumber(t.TotalStock); // lấy tháng cuối quý
    }
    var labels = [],
        inbound = [],
        outbound = [],
        stock = [];
    for (var qi = 0; qi < quarterOrder.length; qi++) {
        var qk = quarterOrder[qi];
        labels.push(quarterMap[qk].lbl);
        inbound.push(quarterMap[qk].inn);
        outbound.push(quarterMap[qk].out);
        stock.push(quarterMap[qk].stock);
    }
    renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
}

function renderFlowTrendYear() {
    // Gộp 12 tháng thành từng năm
    if (!state.flowTrend12T || state.flowTrend12T.length === 0) {
        renderFlowTrendChart();
        return;
    }
    var yearMap = {};
    var yearOrder = [];
    for (var i = 0; i < state.flowTrend12T.length; i++) {
        var t = state.flowTrend12T[i];
        var yr = String(t.Nam);
        if (!yearMap[yr]) {
            yearMap[yr] = { inn: 0, out: 0, stock: 0 };
            yearOrder.push(yr);
        }
        yearMap[yr].inn += toNumber(t.TotalIn);
        yearMap[yr].out += toNumber(t.TotalOut);
        yearMap[yr].stock = toNumber(t.TotalStock);
    }
    var labels = [],
        inbound = [],
        outbound = [],
        stock = [];
    for (var yi = 0; yi < yearOrder.length; yi++) {
        labels.push(yearOrder[yi]);
        inbound.push(yearMap[yearOrder[yi]].inn);
        outbound.push(yearMap[yearOrder[yi]].out);
        stock.push(yearMap[yearOrder[yi]].stock);
    }
    renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
}

// Hàm vẽ biểu đồ flow với dữ liệu tùy chỉnh
function renderFlowTrendCustom(series) {
    var node = byId(ids.chartFlowTrend);
    if (!node) return;

    function getNiceMax(value) {
        if (!isFinite(value) || value <= 0) return 10;
        var exp = Math.pow(10, Math.floor(Math.log(value) / Math.LN10));
        var base = value / exp;
        var niceBase = base <= 1 ? 1 : base <= 2 ? 2 : base <= 5 ? 5 : 10;
        return niceBase * exp;
    }

    var N = series.labels.length;

    var maxBars = 1;
    for (var ii = 0; ii < N; ii++) {
        if (series.inbound[ii] > maxBars) maxBars = series.inbound[ii];
        if (series.outbound[ii] > maxBars) maxBars = series.outbound[ii];
    }
    var maxStock = 1;
    for (var si = 0; si < N; si++) {
        if (series.stock[si] > maxStock) maxStock = series.stock[si];
    }
    maxBars = getNiceMax(maxBars * 1.15);
    maxStock = getNiceMax(maxStock * 1.15);

    var W = 820,
        H = 320;
    var L = 64,
        R = 64,
        T = 30,
        B = 46;
    var plotW = W - L - R,
        plotH = H - T - B;
    var axisY = T + plotH;

    function groupX(idx) {
        if (N <= 1) return L + plotW / 2;
        var margin = 22;
        return L + margin + (idx / (N - 1)) * (plotW - margin * 2);
    }
    function yScaleBars(v) {
        return T + (1 - v / maxBars) * plotH;
    }
    function yScaleStock(v) {
        return T + (1 - v / maxStock) * plotH;
    }

    // Bar width auto-shrinks with density; ≥60 points → use line for bars too.
    var groupStep = N > 1 ? plotW / (N - 1) : plotW;
    var useBars = N <= 45;
    var barW = useBars ? Math.max(2, Math.min(20, groupStep * 0.32)) : 0;
    var gap = useBars ? 2 : 0;

    var defs =
        "<defs>" +
        '<linearGradient id="gradIn2" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stop-color="#74c7f8"/><stop offset="100%" stop-color="#3d9de8"/></linearGradient>' +
        '<linearGradient id="gradOut2" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stop-color="#c4b5fd"/><stop offset="100%" stop-color="#8b5cf6"/></linearGradient>' +
        '<filter id="bsf2" x="-20%" y="-20%" width="140%" height="140%"><feDropShadow dx="0" dy="2" stdDeviation="2" flood-color="#0f172a" flood-opacity="0.12"/></filter>' +
        "</defs>";

    // Grid + LEFT axis labels (bars)
    var grid = "";
    for (var g = 0; g <= 4; g++) {
        var gv = (maxBars / 4) * g;
        var gy = yScaleBars(gv);
        grid +=
            '<line x1="' +
            L +
            '" y1="' +
            gy +
            '" x2="' +
            (W - R) +
            '" y2="' +
            gy +
            '" stroke="#e2eaf4" stroke-dasharray="4 3" stroke-width="1"/>';
        grid +=
            '<text x="' +
            (L - 6) +
            '" y="' +
            (gy + 4) +
            '" text-anchor="end" class="dk-flow-label" style="font-size:10px;fill:#3d9de8;font-weight:700">' +
            formatNumber(gv, 0) +
            "</text>";
    }
    // RIGHT axis labels (stock)
    for (var gs = 0; gs <= 4; gs++) {
        var gvS = (maxStock / 4) * gs;
        var gyS = yScaleStock(gvS);
        grid +=
            '<text x="' +
            (W - R + 6) +
            '" y="' +
            (gyS + 4) +
            '" text-anchor="start" class="dk-flow-label" style="font-size:10px;fill:#f97316;font-weight:700">' +
            formatNumber(gvS, 0) +
            "</text>";
    }
    grid +=
        '<line x1="' +
        L +
        '" y1="' +
        axisY +
        '" x2="' +
        (W - R) +
        '" y2="' +
        axisY +
        '" stroke="#b0c4d8" stroke-width="1.5"/>';
    // axis titles
    var axisTitles =
        '<text x="' +
        (L - 6) +
        '" y="' +
        (T - 8) +
        '" text-anchor="end"   class="dk-flow-axis-title" style="font-size:10px;fill:#3d9de8;font-weight:800">Nhập/Xuất</text>' +
        '<text x="' +
        (W - R + 6) +
        '" y="' +
        (T - 8) +
        '" text-anchor="start" class="dk-flow-axis-title" style="font-size:10px;fill:#f97316;font-weight:800">Tồn kho</text>';

    var barsIn = "",
        barsOut = "";
    if (useBars) {
        for (var bi = 0; bi < N; bi++) {
            var gx = groupX(bi);
            var yin = yScaleBars(series.inbound[bi]);
            var hin = Math.max(0, axisY - yin);
            var yout = yScaleBars(series.outbound[bi]);
            var hout = Math.max(0, axisY - yout);
            if (series.inbound[bi] > 0)
                barsIn +=
                    '<rect x="' +
                    (gx - barW - gap / 2) +
                    '" y="' +
                    yin +
                    '" width="' +
                    barW +
                    '" height="' +
                    Math.max(2, hin) +
                    '" rx="2" fill="url(#gradIn2)"  filter="url(#bsf2)"/>';
            if (series.outbound[bi] > 0)
                barsOut +=
                    '<rect x="' +
                    (gx + gap / 2) +
                    '" y="' +
                    yout +
                    '" width="' +
                    barW +
                    '" height="' +
                    Math.max(2, hout) +
                    '" rx="2" fill="url(#gradOut2)" filter="url(#bsf2)"/>';
        }
    } else {
        var pIn = "",
            pOut = "";
        for (var bi2 = 0; bi2 < N; bi2++) {
            var gx2 = groupX(bi2);
            var yin2 = yScaleBars(series.inbound[bi2]);
            var yout2 = yScaleBars(series.outbound[bi2]);
            pIn += (bi2 === 0 ? "M " : " L ") + gx2 + " " + yin2;
            pOut += (bi2 === 0 ? "M " : " L ") + gx2 + " " + yout2;
        }
        barsIn =
            '<path d="' +
            pIn +
            '" fill="none" stroke="#3d9de8" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"/>';
        barsOut =
            '<path d="' +
            pOut +
            '" fill="none" stroke="#8b5cf6" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"/>';
    }

    var colorLine = "#f97316";
    var lineD = "",
        dots = "",
        stockLabels = "";
    var showStockLabels = N <= 15; // chỉ vẽ label số khi ít điểm
    var dotRadius = N > 60 ? 2 : N > 30 ? 3 : 5;
    for (var pi = 0; pi < N; pi++) {
        var px = groupX(pi);
        var py = yScaleStock(series.stock[pi]);
        if (pi === 0) lineD = "M " + px + " " + py;
        else lineD += " L " + px + " " + py;
        dots +=
            '<circle cx="' +
            px +
            '" cy="' +
            py +
            '" r="' +
            dotRadius +
            '" fill="#fff" stroke="' +
            colorLine +
            '" stroke-width="1.8"/>';
        if (showStockLabels) {
            stockLabels +=
                '<text x="' +
                px +
                '" y="' +
                (py - 10) +
                '" text-anchor="middle" style="font-size:10px;font-weight:700;fill:' +
                colorLine +
                ';">' +
                formatNumber(series.stock[pi], 0) +
                "</text>";
        }
    }
    var stockLine =
        '<path d="' +
        lineD +
        '" fill="none" stroke="' +
        colorLine +
        '" stroke-width="2.6" stroke-linecap="round" stroke-linejoin="round"/>';

    var maxLabelsShown = 12;
    var labelStep = Math.max(1, Math.ceil(N / maxLabelsShown));
    var xLabels = "";
    for (var li = 0; li < N; li++) {
        if (li !== 0 && li !== N - 1 && li % labelStep !== 0) continue;
        var lblText = String(series.labels[li] || "");
        var xPos = groupX(li);
        var rotateAttr = N > 30 ? ' transform="rotate(-30,' + xPos + "," + (axisY + 14) + ')"' : "";
        xLabels +=
            '<text x="' +
            xPos +
            '" y="' +
            (axisY + 14) +
            '" text-anchor="' +
            (N > 30 ? "end" : "middle") +
            '"' +
            rotateAttr +
            ' class="dk-flow-x-label" style="font-size:10px;fill:#5f758b;font-weight:600;">' +
            escapeHtml(lblText) +
            "</text>";
    }

    node.innerHTML =
        '<div class="dk-flow-legend">' +
        '<span class="dk-flow-legend-item"><span class="dk-flow-legend-dot" style="background:#5bb8f5"></span>Nhập</span>' +
        '<span class="dk-flow-legend-item"><span class="dk-flow-legend-dot" style="background:#a78bfa"></span>Xuất</span>' +
        '<span class="dk-flow-legend-item"><span class="dk-flow-legend-dot" style="background:' +
        colorLine +
        '"></span>Tồn Kho</span>' +
        '<span class="dk-text-muted" style="margin-left:auto;font-size:11px">' +
        N +
        " ngày" +
        (useBars ? "" : " (xem dạng đường)") +
        "</span>" +
        "</div>" +
        '<svg viewBox="0 0 ' +
        W +
        " " +
        H +
        '" style="width:100%;height:auto;display:block;overflow:visible;">' +
        defs +
        axisTitles +
        grid +
        barsIn +
        barsOut +
        stockLine +
        dots +
        stockLabels +
        xLabels +
        "</svg>";
}

// ─── Biểu đồ đồng hồ đo lấp đầy NL/PL (Feature 4) ─────────────────────────
function renderCapacityBarChart() {
    var node = byId("chartCapacityBar");
    if (!node) return;

    var overall = state.overall.length > 0 ? state.overall[0] : {};
    var usedNpl = Math.max(0, toNumber(overall.UsedNPL));
    var capNpl = Math.max(0, toNumber(overall.CapacityNPL));
    var usedPl = Math.max(0, toNumber(overall.UsedPL));
    var capPl = Math.max(0, toNumber(overall.CapacityPL));

    var pctNl = capNpl > 0 && isFinite(capNpl) ? Math.max(0, Math.min((usedNpl / capNpl) * 100, 100)) : 0;
    var pctPl = capPl > 0 && isFinite(capPl) ? Math.max(0, Math.min((usedPl / capPl) * 100, 100)) : 0;
    if (!isFinite(pctNl)) pctNl = 0;
    if (!isFinite(pctPl)) pctPl = 0;

    function gaugeColor(pct) {
        if (pct >= 100) return "#ef4444";
        if (pct >= 80) return "#f97316";
        if (pct >= 60) return "#eab308";
        return "#22c55e";
    }

    function semiArcPath(cx, cy, R, pct, stroke) {
        var bg = "M " + (cx - R) + " " + cy + " A " + R + " " + R + " 0 0 1 " + (cx + R) + " " + cy;
        // Guard: NaN, negative, or zero
        if (!isFinite(pct) || pct <= 0) return { bg: bg, fill: null };
        var ratio = Math.max(0, Math.min(pct / 100, 0.9999)); // tránh ratio = 1 → arc endpoint trùng start
        var theta = Math.PI * ratio;
        var ex = cx - R * Math.cos(theta);
        var ey = cy - R * Math.sin(theta);
        // Safety: nếu ex/ey không phải number → bỏ fill
        if (!isFinite(ex) || !isFinite(ey)) return { bg: bg, fill: null };
        var large = ratio > 0.5 ? 1 : 0;
        var fill =
            "M " +
            (cx - R) +
            " " +
            cy +
            " A " +
            R +
            " " +
            R +
            " 0 " +
            large +
            " 1 " +
            ex.toFixed(3) +
            " " +
            ey.toFixed(3);
        return { bg: bg, fill: fill };
    }

    function buildGaugeSvg(cx, cy, R, label, pct, used, total, gradId) {
        var visPct = pct;
        if (pct > 0 && pct < 22) visPct = 22;
        var arc = semiArcPath(cx, cy, R, visPct);
        var col = gaugeColor(pct);
        var col2 = pct >= 100 ? "#dc2626" : pct >= 80 ? "#ea580c" : pct >= 60 ? "#ca8a04" : "#16a34a";
        var strokeW = Math.round(R * 0.18);
        // v2.4.3 — theme-aware track + subtitle text + tag opacity
        var _darkG = dkIsDark();
        var trackColor = _darkG ? "#1e3a6b" : "#e8eef7";
        var subTxt = _darkG ? "#cbd5e1" : "#64748b";
        var tagOpac = _darkG ? "0.28" : "0.14";
        var html = "";
        // Gradient defs
        html +=
            "<defs>" +
            '<linearGradient id="' +
            gradId +
            '" x1="0" y1="0" x2="1" y2="0">' +
            '<stop offset="0%"  stop-color="' +
            col +
            '"/>' +
            '<stop offset="100%" stop-color="' +
            col2 +
            '"/>' +
            "</linearGradient>" +
            '<filter id="' +
            gradId +
            '_glow" x="-30%" y="-30%" width="160%" height="160%">' +
            '<feGaussianBlur stdDeviation="4" result="b"/>' +
            '<feMerge><feMergeNode in="b"/><feMergeNode in="SourceGraphic"/></feMerge>' +
            "</filter>" +
            "</defs>";
        // background arc with subtle shadow
        html +=
            '<path d="' +
            arc.bg +
            '" fill="none" stroke="' +
            trackColor +
            '" stroke-width="' +
            strokeW +
            '" stroke-linecap="round"/>';
        // fill arc — gradient + glow, animated draw-in
        if (arc.fill) {
            // pathLength: 1000 normalize → CSS animate stroke-dashoffset 1000 → 0
            html +=
                '<path class="dk-gauge-fill" d="' +
                arc.fill +
                '" fill="none" ' +
                'stroke="url(#' +
                gradId +
                ')" stroke-width="' +
                strokeW +
                '" stroke-linecap="round" ' +
                'pathLength="1000" stroke-dasharray="1000" filter="url(#' +
                gradId +
                '_glow)"/>';
        }
        // 85% warning tick (dashed red)
        var warnAngle = Math.PI * 0.85;
        var nxIn = cx - (R - strokeW / 2 - 2) * Math.cos(warnAngle);
        var nyIn = cy - (R - strokeW / 2 - 2) * Math.sin(warnAngle);
        var nxOut = cx - (R + strokeW / 2 + 3) * Math.cos(warnAngle);
        var nyOut = cy - (R + strokeW / 2 + 3) * Math.sin(warnAngle);
        html +=
            '<line x1="' +
            nxIn +
            '" y1="' +
            nyIn +
            '" x2="' +
            nxOut +
            '" y2="' +
            nyOut +
            '" stroke="#ef4444" stroke-width="2.5" stroke-dasharray="3 2"/>';
        html +=
            '<text x="' +
            cx +
            '" y="' +
            (cy - 10) +
            '" text-anchor="middle" class="dk-gauge-pct" style="font-size:32px;font-weight:900;fill:' +
            col +
            ';font-variant-numeric:tabular-nums;">' +
            formatNumber(pct, 1) +
            "%</text>";
        html +=
            '<text x="' +
            cx +
            '" y="' +
            (cy + 14) +
            '" text-anchor="middle" style="font-size:12px;font-weight:700;fill:' +
            subTxt +
            ';">' +
            formatNumber(used, 1) +
            " / " +
            formatNumber(total, 1) +
            " CBM</text>";
        var tagCol = label === "Kho NL" ? (_darkG ? "#60a5fa" : "#2563eb") : _darkG ? "#fbbf24" : "#d97706";
        html +=
            '<rect x="' +
            (cx - 26) +
            '" y="' +
            (cy + 22) +
            '" width="52" height="22" rx="11" fill="' +
            tagCol +
            '" opacity="' +
            tagOpac +
            '"/>';
        html +=
            '<text x="' +
            cx +
            '" y="' +
            (cy + 37) +
            '" text-anchor="middle" style="font-size:13px;font-weight:900;fill:' +
            tagCol +
            ';">' +
            escapeHtml(label) +
            "</text>";
        return html;
    }

    var W = 280,
        H = 210,
        cx = W / 2,
        cy = 155,
        R = 108;

    function makeSvg(label, pct, used, total, gradId) {
        var inner = buildGaugeSvg(cx, cy, R, label, pct, used, total, gradId);
        inner +=
            '<text x="' +
            cx +
            '" y="' +
            (H - 2) +
            '" text-anchor="middle" style="font-size:9px;fill:#ef4444;font-weight:700;">── 85% cảnh báo</text>';
        return '<svg class="dk-gauge-svg" viewBox="0 0 ' + W + " " + H + '">' + inner + "</svg>";
    }

    node.innerHTML =
        '<div class="dk-dual-gauge-wrap">' +
        '<div class="dk-gauge-half">' +
        makeSvg("Kho NL", pctNl, usedNpl, capNpl, "gradNL") +
        "</div>" +
        '<div class="dk-gauge-divider"></div>' +
        '<div class="dk-gauge-half">' +
        makeSvg("Kho PL", pctPl, usedPl, capPl, "gradPL") +
        "</div>" +
        "</div>";
}

function renderGiaTriTheoNhomChart() {
    var node = byId("chartGiaTriTheoNhom");
    if (!node) return;
    if (typeof Highcharts === "undefined") return;
    var rows = state.giaTriNhom || [];
    if (rows.length === 0) {
        node.innerHTML = '<div class="dk-empty">Chưa có dữ liệu</div>';
        return;
    }
    applyHighchartsTheme();
    var dark = dkIsDark();
    var data = rows
        .filter(function (r) {
            return r.IsGroup === undefined || toNumber(r.IsGroup) === 1;
        })
        .map(function (r) {
            return { name: r.Nhom, y: toNumber(r.GiaTri), pct: toNumber(r.TyTrong) };
        });
    var tongGiaTri = data.reduce(function (s, d) {
        return s + d.y;
    }, 0);

    var formattedTotal;
    if (tongGiaTri >= 1e9) formattedTotal = formatNumber(tongGiaTri / 1e9, 2) + " tỷ";
    else if (tongGiaTri >= 1e6) formattedTotal = formatNumber(tongGiaTri / 1e6, 1) + " tr";
    else formattedTotal = formatNumber(tongGiaTri, 0);

    node.innerHTML = "";
    node.classList.add("dk-donut-shadow");
    // v2.4.16 — Append icon SAU chart (DOM order = stacking order); align qua events
    var centerEl = document.createElement("div");
    centerEl.className = "dk-nhom-center-icon";
    centerEl.innerHTML = '<i class="fa-solid fa-sack-dollar"></i>';
    function alignCenterIcon(chart) {
        try {
            var series = chart && chart.series && chart.series[0];
            if (!series || !series.center) return;
            var cx = series.center[0];
            var cy = series.center[1];
            var plotLeft = chart.plotLeft || 0;
            var plotTop = chart.plotTop || 0;
            centerEl.style.left = plotLeft + cx + "px";
            centerEl.style.top = plotTop + cy + "px";
            centerEl.style.transform = "translate(-50%, -50%)";
        } catch (e) { }
    }
    Highcharts.chart(node, {
        chart: {
            type: "pie",
            backgroundColor: "transparent",
            spacing: [10, 6, 22, 6],
            animation: { duration: 1000, easing: "easeOutCubic" },
            events: {
                load: function () {
                    alignCenterIcon(this);
                },
                redraw: function () {
                    alignCenterIcon(this);
                },
            },
        },
        title: { text: null },
        credits: { enabled: false },
        tooltip: {
            pointFormat:
                '<span style="color:{point.color}">●</span> <b>{point.name}</b><br/>Giá trị: <b>{point.y:,.0f} VND</b><br/>Tỷ trọng: <b>{point.pct:.1f}%</b>',
        },
        plotOptions: {
            pie: {
                size: "55%",
                innerSize: "60%",
                borderWidth: 3,
                borderColor: dark ? "#0d1d35" : "#ffffff",
                borderRadius: 4,
                showInLegend: false,
                states: { hover: { brightness: 0.15, halo: { size: 8, opacity: 0.3 } } },
                dataLabels: {
                    enabled: true,
                    distance: 18,
                    softConnector: false,
                    connectorWidth: 1.2,
                    allowOverlap: true,
                    format: "{point.name}<br/>{point.pct:.1f}%",
                    style: {
                        color: dark ? "#e2eaf5" : "#1f2937",
                        fontSize: "11px",
                        fontWeight: "700",
                        textOutline: "none",
                    },
                },
            },
        },
        legend: { enabled: false },
        series: [
            {
                name: "Giá trị tồn",
                colorByPoint: true,
                data: data,
                slicedOffset: 8,
            },
        ],
    });
    // v2.4.16 — Append icon SAU khi chart render xong → stacking order trên SVG
    node.appendChild(centerEl);
    var totalEl = document.createElement("div");
    totalEl.className = "dk-nhom-total";
    totalEl.innerHTML = "Tổng: <b>" + formattedTotal + "</b>";
    node.appendChild(totalEl);
}

function renderActivityCalendar() {
    var node = byId("chartActivityCalendar");
    if (!node) return;
    var data = state.activityCalendar || [];
    var nWeeks = activityWeeksCount || 13;

    if (data.length === 0) {
        node.innerHTML = '<div class="dk-empty">Không có dữ liệu hoạt động</div>';
        return;
    }

    var dayMap = {};
    var maxActivity = 0;
    for (var di = 0; di < data.length; di++) {
        var d = data[di];
        var key = String(d.NgayHoatDong || "").substring(0, 10);
        var val = toNumber(d.TotalActivity);
        dayMap[key] = { totalIn: toNumber(d.TotalIn), totalOut: toNumber(d.TotalOut), total: val };
        if (val > maxActivity) maxActivity = val;
    }

    function actColor(val) {
        if (val <= 0) return "#ebedf0";
        var r = val / maxActivity;
        if (r < 0.2) return "#9be9a8";
        if (r < 0.45) return "#40c463";
        if (r < 0.7) return "#30a14e";
        return "#216e39";
    }

    function isoDateKey(d) {
        return (
            d.getFullYear() +
            "-" +
            String(d.getMonth() + 1).padStart(2, "0") +
            "-" +
            String(d.getDate()).padStart(2, "0")
        );
    }

    var today = new Date();
    today.setHours(0, 0, 0, 0);

    var endDay = new Date(today);
    var todayDow = (endDay.getDay() + 6) % 7;
    endDay.setDate(endDay.getDate() + (6 - todayDow));

    var startDay = new Date(endDay);
    startDay.setDate(startDay.getDate() - (nWeeks * 7 - 1));
    var startDow = (startDay.getDay() + 6) % 7;
    startDay.setDate(startDay.getDate() - startDow);

    var weeks = [];
    var cur = new Date(startDay);
    var maxWeeks = nWeeks + 2; // safety cap
    while (cur <= endDay && weeks.length < maxWeeks) {
        var week = [];
        for (var wd = 0; wd < 7; wd++) {
            var dateKey = isoDateKey(cur);
            var info = dayMap[dateKey] || { totalIn: 0, totalOut: 0, total: 0 };
            var isFuture = cur > today;
            week.push({ key: dateKey, info: info, isFuture: isFuture });
            cur.setDate(cur.getDate() + 1);
        }
        weeks.push(week);
    }

    var DAY_LABELS = ["T2", "", "T4", "", "T6", "", "CN"];

    var monthRowHtml = '<div class="dk-cal-month-row">';
    var lastMonth = -1;
    for (var wi = 0; wi < weeks.length; wi++) {
        var firstDay = new Date(weeks[wi][0].key);
        var m = firstDay.getMonth();
        var lbl = m !== lastMonth ? m + 1 + "/" + String(firstDay.getFullYear()).slice(-2) : "";
        if (m !== lastMonth) lastMonth = m;
        monthRowHtml +=
            '<div class="dk-cal-month-slot" style="' +
            (lbl ? "font-weight:800;color:#334155;" : "") +
            '">' +
            lbl +
            "</div>";
    }
    monthRowHtml += "</div>";

    // Day-label column
    var dayLabelHtml = '<div class="dk-cal-day-labels">';
    for (var dl = 0; dl < DAY_LABELS.length; dl++) {
        dayLabelHtml += '<div class="dk-cal-day-label">' + DAY_LABELS[dl] + "</div>";
    }
    dayLabelHtml += "</div>";

    // Week columns
    var weeksHtml = "";
    for (var wi2 = 0; wi2 < weeks.length; wi2++) {
        weeksHtml += '<div class="dk-cal-week">';
        for (var wd2 = 0; wd2 < weeks[wi2].length; wd2++) {
            var cell = weeks[wi2][wd2];
            var bg = cell.isFuture ? "transparent" : actColor(cell.info.total);
            var border = cell.isFuture ? "1px dashed #e2e8f0" : "none";
            var ttip = cell.isFuture
                ? ""
                : cell.key +
                "  Nhập: " +
                formatNumber(cell.info.totalIn, 2) +
                "  Xuất: " +
                formatNumber(cell.info.totalOut, 2) +
                "  Tổng: " +
                formatNumber(cell.info.total, 2);
            weeksHtml +=
                '<div class="dk-cal-cell" style="background:' +
                bg +
                ";border:" +
                border +
                ';" title="' +
                escapeHtml(ttip) +
                '"></div>';
        }
        weeksHtml += "</div>";
    }

    node.innerHTML =
        monthRowHtml +
        '<div class="dk-cal-outer">' +
        dayLabelHtml +
        '<div class="dk-cal-wrap">' +
        weeksHtml +
        "</div>" +
        "</div>";
}

// v2.3.23 — Fetch NK dự kiến từ ERP_NhapKhoNPL (full range, không bị giới hạn 14 ngày như GetChuanBiVe)
function loadNKDuKienForCalendar(callback) {
    if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    var from = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() - 1, 1);
    var to = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 2, 0);
    var url = "/api/DashboardKhoDesktop/GetNKDuKienByRange?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
    requestJson(url)
        .then(function (data) {
            state.nkDuKien = normalizeArray(data);
            if (callback) callback();
        })
        .catch(function () {
            state.nkDuKien = [];
            if (callback) callback();
        });
}

/**
 * Vẽ lịch trình công việc và hoạt động của kho theo tháng.
 */
function renderActivityCalendarMonthly() {

    var node = byId("chartActivityCalendarMonthly");
    if (!node) return;
    var data = state.activityCalendar || [];

    // Init calMonthDate to current month if not set
    if (!calMonthDate) {
        var now = new Date();
        calMonthDate = new Date(now.getFullYear(), now.getMonth(), 1);
    }

    // Update month title
    var titleEl = byId("calMonthTitle");
    if (titleEl) {
        var monthNames = [
            "Tháng 1",
            "Tháng 2",
            "Tháng 3",
            "Tháng 4",
            "Tháng 5",
            "Tháng 6",
            "Tháng 7",
            "Tháng 8",
            "Tháng 9",
            "Tháng 10",
            "Tháng 11",
            "Tháng 12",
        ];
        if (calRangeFrom && calRangeTo) {
            titleEl.textContent = "Từ: " + formatDateShort(calRangeFrom) + " - " + formatDateShort(calRangeTo);
        } else {
            titleEl.textContent = monthNames[calMonthDate.getMonth()] + " " + calMonthDate.getFullYear();
        }
    }

    // Build dayMap from state.activityCalendar
    var dayMap = {};
    for (var di = 0; di < data.length; di++) {
        var d = data[di];
        var key = String(d.NgayHoatDong || d.ngayHoatDong || "").substring(0, 10);
        dayMap[key] = {
            totalIn: toNumber(d.TotalIn || d.totalIn),
            totalOut: toNumber(d.TotalOut || d.totalOut),
            totalKK: toNumber(d.TotalKiemKe || d.totalKiemKe),
            total: toNumber(d.TotalActivity || d.totalActivity),
        };
    }

    // Build planned inbound dates
    var plannedDates = {};
    var plannedSource = state.nkDuKien || [];
    for (var pi = 0; pi < plannedSource.length; pi++) {
        var pDate = parseDate(plannedSource[pi].NgayNKDuKien || plannedSource[pi].ngayNKDuKien);
        if (pDate) {
            var pKey =
                pDate.getFullYear() +
                "-" +
                String(pDate.getMonth() + 1).padStart(2, "0") +
                "-" +
                String(pDate.getDate()).padStart(2, "0");
            plannedDates[pKey] = (plannedDates[pKey] || 0) + 1;
        }
    }

    // Determine display range
    var dispFrom, dispTo;
    if (calRangeFrom && calRangeTo) {
        dispFrom = new Date(calRangeFrom);
        dispTo = new Date(calRangeTo);
    } else {
        dispFrom = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth(), 1);
        dispTo = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 1, 0);
    }

    function isoKey(dt) {
        return (
            dt.getFullYear() +
            "-" +
            String(dt.getMonth() + 1).padStart(2, "0") +
            "-" +
            String(dt.getDate()).padStart(2, "0")
        );
    }

    // Build calendar grid for the display range
    var gridStart = new Date(dispFrom);
    var dow = (gridStart.getDay() + 6) % 7; // 0=Mon
    gridStart.setDate(gridStart.getDate() - dow);

    var gridEnd = new Date(dispTo);
    var dow2 = (gridEnd.getDay() + 6) % 7;
    gridEnd.setDate(gridEnd.getDate() + (6 - dow2));

    var today = new Date();
    today.setHours(0, 0, 0, 0);
    var DAY_NAMES = ["Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy", "Chủ Nhật"];

    var html = "";

    // Day-of-week header
    html += '<div class="dk-cal-monthly-header">';
    for (var dh = 0; dh < 7; dh++) {
        html += '<div class="dk-cal-monthly-dow">' + DAY_NAMES[dh] + "</div>";
    }
    html += "</div>";

    // Weeks
    html += '<div class="dk-cal-monthly-grid">';
    var cur = new Date(gridStart);
    while (cur <= gridEnd) {
        for (var wd = 0; wd < 7; wd++) {
            var dk = isoKey(cur);
            var isCurrentMonth =
                cur.getMonth() === calMonthDate.getMonth() && cur.getFullYear() === calMonthDate.getFullYear();
            var isFuture = cur > today;
            var isToday = isoKey(cur) === isoKey(today);
            var info = dayMap[dk] || { totalIn: 0, totalOut: 0, totalKK: 0, total: 0 };
            var planned = plannedDates[dk] || 0;

            var cellClass = "dk-cal-monthly-cell";
            var inlineStyle = "";
            var isHidden = false;

            if (calRangeFrom && calRangeTo) {
                var curTime = cur.getTime();
                if (curTime < calRangeFrom.getTime() || curTime > calRangeTo.getTime()) {
                    cellClass += " dk-cal-dimmed dk-cal-out-month";
                    inlineStyle += "visibility: hidden;";
                } else if (
                    calRangeFrom.getMonth() !== calRangeTo.getMonth() ||
                    calRangeFrom.getFullYear() !== calRangeTo.getFullYear()
                ) {
                    cellClass += " dk-cal-month-" + cur.getMonth();
                }
            } else {
                if (!isCurrentMonth) cellClass += " dk-cal-out-month";
            }

            if (isToday) cellClass += " dk-cal-today";
            if (isFuture && !planned) cellClass += " dk-cal-future";

            var actFilter = state.calActFilter || { in: true, out: true, kk: true, plan: true };

            function formatShort(n) {
                n = toNumber(n);
                if (n === 0) return "0";
                if (n >= 1000000) return (n / 1000000).toFixed(1).replace(/\.0$/, "") + "M";
                if (n >= 1000) return (n / 1000).toFixed(1).replace(/\.0$/, "") + "K";
                return formatNumber(n, 0);
            }

            var inVal = toNumber(info.totalIn);
            var outVal = toNumber(info.totalOut);
            var kkVal = toNumber(info.totalKK);
            var planVal = toNumber(planned);

            // ==========================================
            // XỬ LÝ GIAO DIỆN LPCP
            // ==========================================
            var lpcpDay = state.lpcpCalendar ? state.lpcpCalendar[dk] : null;
            var warnCount = 0,
                taskCount = 0,
                pickCount = 0;
            if (lpcpDay) {
                var lTasks = lpcpDay.Tasks || lpcpDay.tasks || [];
                var uniqueWarnTask = {};
                var uniqueTask = {};
                var warnPickCount = 0;
                var rawPickCount = 0;

                lTasks.forEach(function (t) {
                    var ma = t.MaLenhSX || t.maLenhSX || "";

                    if (t.GhiChu === 'TASK') {
                        if (ma) uniqueTask[ma] = 1;
                        else taskCount++;

                        if (t.ThieuNPL || t.thieuNPL || t.TrangThai === 3) {
                            if (ma) uniqueWarnTask[ma] = 1;
                            else warnCount++;
                        }
                    } else if (t.GhiChu === 'PICK') {
                        rawPickCount++;
                        if (t.ThieuNPL || t.thieuNPL || t.TrangThai === 3) {
                            warnPickCount++;
                        }
                    }
                });

                warnCount += Object.keys(uniqueWarnTask).length + warnPickCount;
                taskCount += Object.keys(uniqueTask).length;
                pickCount = rawPickCount;

                if (lpcpDay.ThieuNPL || lpcpDay.thieuNPL) warnCount = Math.max(warnCount, 1);

                if (pickCount === 0 && taskCount > 0) {
                    pickCount = taskCount;
                }
            }

            var iconIn =
                '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>';
            var iconOut =
                '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="17 8 12 3 7 8"/><line x1="12" y1="3" x2="12" y2="15"/></svg>';
            var iconKK =
                '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="9" y="3" width="6" height="4" rx="1"/><path d="M9 5H6a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-3M9 14l2 2 4-4"/></svg>';
            var iconPlan =
                '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/><rect x="10.5" y="14.5" width="3" height="3"/></svg>';
            var iconWarn =
                '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>';
            var iconTask =
                '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="8" r="4" /><path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" /></svg>';
            var iconPick =
                '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z" /></svg>';

            function buildSlot(key, icon, val, label, visible) {
                if (!visible || !val || val === 0 || val === "0") return "";
                var title = label + ": " + (key === "plan" ? val + " l\u00f4" : formatNumber(val, 0));
                return (
                    '<div class="dk-cal-act dk-cal-act-' +
                    key +
                    '" title="' +
                    escapeHtml(title) +
                    '" data-act="' +
                    key +
                    '">' +
                    '<span class="dk-cal-act-icon">' +
                    icon +
                    "</span>" +
                    '<span class="dk-cal-act-val">' +
                    formatShort(val) +
                    "</span>" +
                    "</div>"
                );
            }

            var visibleCount = 0;
            if (actFilter.in) visibleCount++;
            if (actFilter.out) visibleCount++;
            if (actFilter.kk) visibleCount++;
            if (actFilter.plan) visibleCount++;

            cellClass += " dk-cal-slots-" + visibleCount;

            var slots = "";
            var actListLeft = "";
            if (actFilter.in) actListLeft += buildSlot("in", iconIn, inVal, "Nh\u1eadp kho", true);
            if (actFilter.out) actListLeft += buildSlot("out", iconOut, outVal, "Xu\u1ea5t kho", true);
            if (actFilter.kk) actListLeft += buildSlot("kk", iconKK, kkVal, "Ki\u1ec3m k\u00ea", true);
            if (actFilter.plan) actListLeft += buildSlot("plan", iconPlan, planVal, "NK d\u1ef1 ki\u1ebfn", true);

            var actListRight = "";
            actListRight += buildSlot("warn", iconWarn, warnCount, "C\u1ea3nh b\u00e1o", true);
            actListRight += buildSlot("task", iconTask, taskCount, "Task ph\u00e2n c\u00f4ng", true);
            actListRight += buildSlot("pick", iconPick, pickCount, "L\u1ec7nh so\u1ea1n h\u00e0ng", true);

            if (actListLeft || actListRight) {
                slots =
                    '<div class="dk-cal-acts-container" style="display: flex; justify-content: center; gap: 12px; padding: 4px 2px 0;">' +
                    '<div class="dk-cal-acts-left" style="display: flex; flex-direction: column; gap: 4px;">' +
                    actListLeft +
                    "</div>" +
                    (actListRight
                        ? '<div class="dk-cal-acts-right" style="display: flex; flex-direction: column; gap: 4px;">' +
                        actListRight +
                        "</div>"
                        : "") +
                    "</div>";
            } else {
                slots = '<div class="dk-cal-acts dk-cal-acts-empty"></div>';
            }

            var tooltip =
                dk +
                " Nh\u1eadp: " +
                formatNumber(inVal, 2) +
                " Xu\u1ea5t: " +
                formatNumber(outVal, 2) +
                (kkVal ? " KK: " + formatNumber(kkVal, 2) : "") +
                (planVal ? " NK DK: " + planVal + " l\u00f4" : "");

            // Gộp chung 1 cell top
            var cellTop =
                '<div class="dk-cell-top" style="width: 100%; display: flex; flex-direction: column; align-items: center;">' +
                '<div class="dk-cal-day-num" style="font-size: 22px; text-align: center; width: 100%; margin: 0 auto;">' +
                cur.getDate() +
                "</div>" +
                slots +
                "</div>";

            html +=
                '<div class="' +
                cellClass +
                '" style="' +
                inlineStyle +
                '; justify-content: center;" data-date="' +
                dk +
                '" title="' +
                escapeHtml(tooltip) +
                '">' +
                (isHidden ? "" : cellTop) +
                "</div>";

            cur.setDate(cur.getDate() + 1);
        }
    }
    html += "</div>";

    node.innerHTML = html;

    // Bind day click: phần trên (bars) → modal, phần dưới (tasks) → sidebar
    var cells = node.querySelectorAll(".dk-cal-monthly-cell[data-date]");
    for (var ci = 0; ci < cells.length; ci++) {
        (function (cell) {
            var dt = cell.getAttribute("data-date");
            if (!dt) return;
            var cellInfo = dayMap[dt] || { totalIn: 0, totalOut: 0, totalKK: 0, total: 0 };
            var plCount = plannedDates[dt] || 0;

            // Chỉ cần bind 1 sự kiện click duy nhất cho toàn bộ ô lịch (gộp lại)
            cell.style.cursor = "pointer";
            cell.addEventListener("click", function () {
                // Luôn cho phép click mở Modal để người dùng không tưởng là bị lỗi (unclickable)
                showCalDayDetail(dt, cellInfo, plCount);

                // Luôn cập nhật Sidebar (phần dưới)
                if (currentPage === 3) {
                    showLpcpInlineDetail(dt);
                }
            });
        })(cells[ci]);
    }
}

function renderQuickStatus() {
    var statusNode = byId(ids.quickStatus);
    if (!statusNode) return;

    var racksCritical = state.racks.filter(function (item) {
        var used = toNumber(item.TongCBMSuDungTrongKe);
        var cap = toNumber(item.TongCBMTrongKe);
        if (cap <= 0) return false;
        return (used / cap) * 100 >= 90;
    }).length;

    var inboundOverdue = state.inbound.filter(function (item) {
        var date = parseDate(item.NgayNKDuKien);
        if (!date) return false;
        var today = new Date();
        var midnight = new Date(today.getFullYear(), today.getMonth(), today.getDate());
        return date.getTime() < midnight.getTime();
    }).length;

    var outboundHighVolume = state.outboundRunning.filter(function (item) {
        return toNumber(item.SLXuat) > 1000;
    }).length;

    var blocks = [];
    blocks.push(
        '<div class="dk-status-item ' +
        (racksCritical > 0 ? "warn" : "good") +
        '"><strong>Kệ vượt 90%:</strong> ' +
        formatNumber(racksCritical, 0) +
        " kệ</div>",
    );
    blocks.push(
        '<div class="dk-status-item ' +
        (inboundOverdue > 0 ? "bad" : "good") +
        '"><strong>Lệnh nhập quá hạn:</strong> ' +
        formatNumber(inboundOverdue, 0) +
        " lệnh</div>",
    );
    blocks.push(
        '<div class="dk-status-item ' +
        (outboundHighVolume > 0 ? "warn" : "good") +
        '"><strong>Lệnh xuất SL lớn (&gt;1000):</strong> ' +
        formatNumber(outboundHighVolume, 0) +
        " lệnh</div>",
    );
    blocks.push(
        '<div class="dk-status-item"><strong>Kho đang theo dõi:</strong> ' +
        formatNumber(state.customers.length, 0) +
        " khách hàng | " +
        formatNumber(state.racks.length, 0) +
        " kệ</div>",
    );

    statusNode.innerHTML = blocks.join("");
}

function buildCustomerDetailRows() {
    var totalCbm = state.customers.reduce(function (sum, item) {
        return sum + Math.max(0, toNumber(item.CBMSDTrongKho));
    }, 0);
    var sorted = state.customers
        .map(function (item, index) {
            var cbm = Math.max(0, toNumber(item.CBMSDTrongKho));
            return {
                sourceIndex: index,
                MaKH: item.MaKH || "",
                TenKH: normalizeCustomerName(item.TenKH),
                SLVatTu: toNumber(item.SLVatTu),
                CBMSDTrongKho: cbm,
                TyTrongCBM: totalCbm > 0 ? (cbm / totalCbm) * 100 : 0,
            };
        })
        .sort(function (a, b) {
            return b.CBMSDTrongKho - a.CBMSDTrongKho;
        });
    for (var i = 0; i < sorted.length; i++) {
        sorted[i].ThuHang = i + 1;
    }
    return sorted;
}

function toDateKey(raw) {
    var date = parseDate(raw);
    if (!date) return "";
    return (
        date.getFullYear() +
        "-" +
        String(date.getMonth() + 1).padStart(2, "0") +
        "-" +
        String(date.getDate()).padStart(2, "0")
    );
}

// ─── Top 5 grouped bar chart (NL + PL) ──────────────────────────────────

function dkIsDark() {
    return document.body.classList.contains("dark-theme");
}

function applyHighchartsTheme() {
    if (typeof Highcharts === "undefined") return;
    var dark = dkIsDark();
    var axisLbl = dark ? "#9fb3d1" : "#475569";
    var axisTitle = dark ? "#cbd5e1" : "#475569";
    var gridLine = dark ? "rgba(96,165,250,0.12)" : "#e5e7eb";
    var dataLbl = dark ? "#e2eaf5" : "#1f2937";
    var legendItm = dark ? "#cbd5e1" : "#1f2937";
    Highcharts.setOptions({
        accessibility: { enabled: false },
        colors: dark
            ? ["#60a5fa", "#34d399", "#fbbf24", "#f87171", "#a78bfa", "#22d3ee", "#f472b6", "#fb923c"]
            : ["#2563eb", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6", "#0ea5e9", "#ec4899", "#f97316"],
        chart: { style: { fontFamily: "'Segoe UI', Tahoma, Arial, sans-serif" }, backgroundColor: "transparent" },
        tooltip: {
            backgroundColor: dark ? "rgba(20, 42, 76, 0.96)" : "#ffffff",
            borderColor: dark ? "#3b82f6" : "#e5e7eb",
            borderRadius: 8,
            borderWidth: 1,
            shadow: {
                color: dark ? "rgba(0,0,0,0.6)" : "rgba(0,0,0,0.15)",
                offsetX: 0,
                offsetY: 2,
                opacity: 0.4,
                width: 6,
            },
            style: { color: dark ? "#f1f5fb" : "#1f2937", fontSize: "12px" },
        },
        xAxis: {
            lineColor: gridLine,
            tickColor: gridLine,
            gridLineColor: gridLine,
            labels: { style: { color: axisLbl, fontSize: "11px" } },
            title: { style: { color: axisTitle, fontSize: "11px" } },
        },
        yAxis: {
            lineColor: gridLine,
            tickColor: gridLine,
            gridLineColor: gridLine,
            labels: { style: { color: axisLbl, fontSize: "11px" } },
            title: { style: { color: axisTitle, fontSize: "11px" } },
            stackLabels: { style: { color: dataLbl, fontWeight: "700", textOutline: "none" } },
        },
        legend: {
            itemStyle: { color: legendItm, fontSize: "11px" },
            itemHoverStyle: { color: dark ? "#f1f5fb" : "#0f172a" },
        },
        plotOptions: {
            series: {
                dataLabels: {
                    style: {
                        color: dataLbl,
                        fontWeight: "700",
                        textOutline: dark ? "2px rgba(0,0,0,0.6)" : "2px rgba(255,255,255,0.8)",
                    },
                },
            },
            pie: {
                borderColor: dark ? "#0d1d35" : "#ffffff",
                borderWidth: 2,
            },
        },
    });

    // Update all existing charts without reloading data
    if (Highcharts.charts) {
        Highcharts.charts.forEach(function (chart) {
            if (chart) {
                chart.update(
                    {
                        chart: { backgroundColor: "transparent" },
                        xAxis: {
                            lineColor: gridLine,
                            tickColor: gridLine,
                            gridLineColor: gridLine,
                            labels: { style: { color: axisLbl } },
                            title: { style: { color: axisTitle } },
                        },
                        yAxis: {
                            lineColor: gridLine,
                            tickColor: gridLine,
                            gridLineColor: gridLine,
                            labels: { style: { color: axisLbl } },
                            title: { style: { color: axisTitle } },
                        },
                        legend: {
                            itemStyle: { color: legendItm },
                            itemHoverStyle: { color: dark ? "#f1f5fb" : "#0f172a" },
                        },
                        tooltip: {
                            backgroundColor: dark ? "rgba(20, 42, 76, 0.96)" : "#ffffff",
                            borderColor: dark ? "#3b82f6" : "#e5e7eb",
                            style: { color: dark ? "#f1f5fb" : "#1f2937" },
                        },
                        plotOptions: {
                            series: {
                                dataLabels: {
                                    style: {
                                        color: dataLbl,
                                        textOutline: dark ? "2px rgba(0,0,0,0.6)" : "2px rgba(255,255,255,0.8)",
                                    },
                                },
                            },
                            pie: { borderColor: dark ? "#0d1d35" : "#ffffff" },
                        },
                    },
                    true,
                ); // redraw
            }
        });
    }
}

// ─── Feature 11: Populate customer filter dropdown ──────────────────────────
function populateCustomerFilter() {
    var sel = byId("customerFilterSelect");
    if (!sel) return;
    var prev = activeCustomerFilter;
    // Rebuild options
    var opts = '<option value=""' + (prev === "" ? " selected" : "") + ">Tất cả khách hàng</option>";
    var rows = buildCustomerDetailRows();
    for (var ci = 0; ci < rows.length; ci++) {
        var r = rows[ci];
        var v = r.MaKH || "";
        if (!v) continue; // bỏ qua khách không có mã để tránh trùng với "Tất cả"
        var lbl = escapeHtml((r.TenKH || r.MaKH || "").substring(0, 28));
        var sel2 = v && v === prev ? " selected" : "";
        opts += '<option value="' + escapeHtml(v) + '"' + sel2 + ">" + lbl + "</option>";
    }
    sel.innerHTML = opts;
}

function openCalendarOverviewModal() {
    var fromD, toD, titleSuffix;
    if (calRangeFrom && calRangeTo) {
        fromD = calRangeFrom;
        toD = calRangeTo;
        titleSuffix = "từ " + formatDateShort(fromD) + " → " + formatDateShort(toD);
    } else {
        // fallback: tháng đang hiển thị
        var base = calMonthDate || new Date(new Date().getFullYear(), new Date().getMonth(), 1);
        fromD = new Date(base.getFullYear(), base.getMonth(), 1);
        toD = new Date(base.getFullYear(), base.getMonth() + 1, 0);
        titleSuffix = "tháng " + (base.getMonth() + 1) + "/" + base.getFullYear();
    }

    var modalTitle = byId(ids.detailModalTitle);
    var modalMeta = byId(ids.detailModalMeta);
    var modalContent = byId(ids.detailModalContent);
    var modal = byId(ids.detailModal);
    if (!modal) return;

    if (modalTitle) modalTitle.textContent = "Tổng quát hoạt động kho " + titleSuffix;
    if (modalMeta) modalMeta.innerHTML = '<span style="color:#64748b;font-size:13px">Đang tổng hợp...</span>';

    var searchBar = document.querySelector(".dk-modal-search-bar");
    if (searchBar) searchBar.style.display = "";
    var searchInputEl = byId("detailSearchInput");
    if (searchInputEl) {
        searchInputEl.value = "";
        searchInputEl.placeholder = "Tìm trong tab đang hiện...";
    }
    var searchCountEl = byId("detailSearchCount");
    if (searchCountEl) searchCountEl.textContent = "";

    if (modalContent)
        modalContent.innerHTML = '<div class="dk-empty" style="padding:30px">Đang tải dữ liệu tổng quát...</div>';
    modal.classList.add("open");

    var url =
        "/api/DashboardKhoDesktop/GetActivityRangeDetail?tuNgay=" + asIsoDate(fromD) + "&denNgay=" + asIsoDate(toD);

    requestJson(url)
        .then(function (data) {
            var d = data || {};
            var nhapRows = normalizeArray(d.Nhap);
            var xuatRows = normalizeArray(d.Xuat);
            var kiemKeRows = normalizeArray(d.KiemKe);

            for (var i = 0; i < xuatRows.length; i++) {
                if (!xuatRows[i].NgayXuat)
                    xuatRows[i].NgayXuat = xuatRows[i].NgayXuatDen || xuatRows[i].NgayXuatTu || "";
            }
            for (var i = 0; i < kiemKeRows.length; i++) {
                if (!kiemKeRows[i].NgayKiemKe)
                    kiemKeRows[i].NgayKiemKe = kiemKeRows[i].NgayKKDen || kiemKeRows[i].NgayKKTu || "";
            }

            // NK dự kiến: lọc state.nkDuKien theo range
            var nkSrc = state.nkDuKien || [];
            var plannedRows = nkSrc.filter(function (r) {
                var dt = parseDate(r.NgayNKDuKien || r.ngayNKDuKien);
                if (!dt) return false;
                return dt >= fromD && dt <= toD;
            });

            // Tính tổng cho header meta
            var sumIn = 0,
                sumOut = 0,
                sumKK = 0;
            for (var i = 0; i < nhapRows.length; i++) sumIn += toNumber(nhapRows[i].SoLuong);
            for (var j = 0; j < xuatRows.length; j++) sumOut += toNumber(xuatRows[j].SoLuong);
            for (var k = 0; k < kiemKeRows.length; k++) sumKK += toNumber(kiemKeRows[k].SoLuong);

            if (modalMeta) {
                modalMeta.innerHTML =
                    '<span style="color:#3b82f6;font-weight:700">Nhập: ' +
                    formatNumber(sumIn, 2) +
                    "</span>" +
                    ' &nbsp;·&nbsp; <span style="color:#f97316;font-weight:700">Xuất: ' +
                    formatNumber(sumOut, 2) +
                    "</span>" +
                    ' &nbsp;·&nbsp; <span style="color:#10b981;font-weight:700">Kiểm kê: ' +
                    formatNumber(sumKK, 2) +
                    "</span>" +
                    ' &nbsp;·&nbsp; <span style="color:#8b5cf6;font-weight:700">NK dự kiến: ' +
                    plannedRows.length +
                    " lô</span>";
            }

            renderOverviewDetailTabs(modalContent, nhapRows, xuatRows, kiemKeRows, plannedRows);
        })
        .catch(function (err) {
            if (modalContent) {
                modalContent.innerHTML =
                    '<div class="dk-empty" style="padding:30px;color:#dc2626">' +
                    "Lỗi tải tổng quát: " +
                    escapeHtml(String((err && err.message) || err)) +
                    "</div>";
            }
        });
}

// v2.3.46 — Render kết quả global search Itemcode
function renderGlobalSearchResult(container, code, row) {
    if (!container) return;
    if (!row || !row.MaVTID) {
        container.innerHTML =
            '<div class="dk-empty" style="padding:14px">' +
            "Không tìm thấy Itemcode <b>" +
            escapeHtml(code) +
            "</b> trong bảng vật tư.</div>";
        return;
    }
    var sections = [
        {
            key: "ton",
            label: "Tồn kho",
            page: 1,
            rows: toNumber(row.TonRows),
            sl: toNumber(row.TonSL),
            color: "#0ea5e9",
            goto: "capacitySummary",
        },
        {
            key: "nhap",
            label: "Nhập kho (30N)",
            page: 2,
            rows: toNumber(row.NhapRows),
            sl: toNumber(row.NhapSL),
            color: "#3b82f6",
            goto: null,
        },
        {
            key: "xuat",
            label: "Xuất kho (30N)",
            page: 2,
            rows: toNumber(row.XuatRows),
            sl: toNumber(row.XuatSL),
            color: "#f97316",
            goto: "outboundRunning",
        },
        {
            key: "kk",
            label: "Kiểm kê (30N)",
            page: 3,
            rows: toNumber(row.KKRows),
            sl: toNumber(row.KKSL),
            color: "#10b981",
            goto: null,
        },
        {
            key: "dk",
            label: "NK dự kiến (60N)",
            page: 3,
            rows: toNumber(row.DKRows),
            sl: 0,
            color: "#8b5cf6",
            goto: "inboundReady",
        },
    ];

    var html =
        '<div class="dk-gs-header">' +
        '<span class="dk-gs-found">Tìm thấy: <b>' +
        escapeHtml(code) +
        "</b>" +
        (row.TenVT ? ' — <span style="color:#475569">' + escapeHtml(String(row.TenVT)) + "</span>" : "") +
        "</span></div>";
    html += '<div class="dk-gs-grid">';
    for (var i = 0; i < sections.length; i++) {
        var s = sections[i];
        var hasData = s.rows > 0;
        var clickable = hasData;
        html +=
            '<div class="dk-gs-card' +
            (hasData ? " has-data" : " no-data") +
            (clickable ? " clickable" : "") +
            '" ' +
            'data-page="' +
            s.page +
            '" data-goto="' +
            (s.goto || "") +
            '" ' +
            'style="--gs-color:' +
            s.color +
            '">' +
            '<div class="dk-gs-card-head"><span class="dk-gs-dot" style="background:' +
            s.color +
            '"></span>' +
            escapeHtml(s.label) +
            "</div>" +
            '<div class="dk-gs-card-rows">' +
            formatNumber(s.rows, 0) +
            " bản ghi</div>" +
            (s.sl > 0 ? '<div class="dk-gs-card-sl">SL: ' + formatNumber(s.sl, 2) + "</div>" : "") +
            (clickable ? '<div class="dk-gs-card-go">Bấm để xem →</div>' : "") +
            "</div>";
    }
    html += "</div>";
    container.innerHTML = html;

    // Bind click → switch page + open detail
    var cards = container.querySelectorAll(".dk-gs-card.clickable");
    for (var c = 0; c < cards.length; c++) {
        cards[c].addEventListener("click", function () {
            var page = this.getAttribute("data-page");
            var go = this.getAttribute("data-goto");
            // Switch page (sidebar nav)
            if (page) {
                var nav = document.querySelector('.dk-page-btn[data-page="' + page + '"]');
                if (nav) nav.click();
            }
            // Open detail modal if specified
            if (go) {
                setTimeout(function () {
                    var trigger = document.querySelector('.js-open-detail[data-detail="' + go + '"]');
                    if (trigger) trigger.click();
                }, 250);
            }
        });
    }
}

// v2.3.7 — modal chi tiết hoạt động ngày với 4 tab record-level data
function showCalDayDetail(dateKey, info, plannedCount) {
    var modalTitle = byId(ids.detailModalTitle);
    var modalMeta = byId(ids.detailModalMeta);
    var modalContent = byId(ids.detailModalContent);
    var modal = byId(ids.detailModal);
    if (!modal) return;

    // Format dateKey for display (yyyy-MM-dd → dd/MM/yyyy)
    var displayDate = dateKey;
    var parts = String(dateKey).split("-");
    if (parts.length === 3) displayDate = parts[2] + "/" + parts[1] + "/" + parts[0];

    if (modalTitle) modalTitle.textContent = "Hoạt động kho ngày " + displayDate;
    if (modalMeta) {
        modalMeta.innerHTML =
            '<span style="color:#3b82f6;font-weight:700">Nhập: ' +
            formatNumber(info.totalIn, 2) +
            "</span>" +
            ' &nbsp;·&nbsp; <span style="color:#f97316;font-weight:700">Xuất: ' +
            formatNumber(info.totalOut, 2) +
            "</span>" +
            ' &nbsp;·&nbsp; <span style="color:#10b981;font-weight:700">Kiểm kê: ' +
            formatNumber(info.totalKK, 2) +
            "</span>" +
            ' &nbsp;·&nbsp; <span style="color:#8b5cf6;font-weight:700">NK dự kiến: ' +
            plannedCount +
            " lô</span>";
    }

    // v2.3.9 — KEEP search bar visible so user có thể lọc record trong tab đang hiện
    var searchBar = document.querySelector(".dk-modal-search-bar");
    if (searchBar) searchBar.style.display = "";
    var searchInputEl = byId("detailSearchInput");
    if (searchInputEl) {
        searchInputEl.value = "";
        searchInputEl.placeholder = "Tìm khách hàng, mã NPL, số lô, barcode...";
    }
    var searchCountEl = byId("detailSearchCount");
    if (searchCountEl) searchCountEl.textContent = "";

    // Show loading state
    if (modalContent) modalContent.innerHTML = '<div class="dk-empty" style="padding:30px">Đang tải chi tiết...</div>';
    modal.classList.add("open");

    var url = "/api/DashboardKhoDesktop/GetActivityDayDetail?ngay=" + dateKey;
    var inRangeMode = false; // Luôn hiển thị 1 ngày duy nhất khi click vào ô ngày, dù đang bật filter range

    requestJson(url)
        .then(function (data) {

            var d = data || {};
            var nhapRows = normalizeArray(d.Nhap);
            var xuatRows = normalizeArray(d.Xuat);
            var kiemKeRows = normalizeArray(d.KiemKe);

            for (var i = 0; i < nhapRows.length; i++) {
                if (!nhapRows[i].NgayNhap && !nhapRows[i].NgayNhapKho)
                    nhapRows[i].NgayNhapKho = inRangeMode
                        ? nhapRows[i].NgayNhapDen || nhapRows[i].NgayNhapTu || ""
                        : dateKey;
            }
            for (var i = 0; i < xuatRows.length; i++) {
                if (!xuatRows[i].NgayXuat)
                    xuatRows[i].NgayXuat = inRangeMode
                        ? xuatRows[i].NgayXuatDen || xuatRows[i].NgayXuatTu || ""
                        : dateKey;
            }
            for (var i = 0; i < kiemKeRows.length; i++) {
                if (!kiemKeRows[i].NgayKiemKe)
                    kiemKeRows[i].NgayKiemKe = inRangeMode
                        ? kiemKeRows[i].NgayKKDen || kiemKeRows[i].NgayKKTu || ""
                        : dateKey;
            }

            // v2.3.43 — NK dự kiến: nếu range mode thì lọc theo range; nếu không thì 1 ngày
            var nkSrc = state.nkDuKien || [];
            var plannedRows = nkSrc.filter(function (r) {
                var dt = parseDate(r.NgayNKDuKien || r.ngayNKDuKien);
                if (!dt) return false;
                if (inRangeMode) {
                    return dt >= calRangeFrom && dt <= calRangeTo;
                }
                return asIsoDate(dt) === dateKey;
            });

            renderDayDetailTabs(modalContent, nhapRows, xuatRows, kiemKeRows, plannedRows);

            // v2.3.54 — Append Lịch Phân Công tab sau 4 tabs kho
            if (!inRangeMode) {
                requestJson("/api/DashboardKhoDesktop/LichPhanCong_GetDayDetail?ngay=" + dateKey)
                    .then(function (lpcpRes) {
                        if (!lpcpRes || !lpcpRes.success) return;
                        appendLpcpTab(modalContent, lpcpRes.data || {});
                    })
                    .catch(function () {
                        /* optional tab — silent fail */
                    });
            }
        })
        .catch(function (err) {
            console.error("[Dashboard Kho] Day detail API error:", err);
            // v2.3.8.2 — Silent fallback: dùng data sẵn có (không hiện banner cảnh báo)
            var plannedRows = (state.nkDuKien || []).filter(function (r) {
                var dt = parseDate(r.NgayNKDuKien || r.ngayNKDuKien);
                if (!dt) return false;
                return asIsoDate(dt) === dateKey;
            });
            var xuatFallback = (state.outboundRunning || [])
                .filter(function (r) {
                    var dt = parseDate(r.NgayXuatHang);
                    if (!dt) return false;
                    return asIsoDate(dt) === dateKey;
                })
                .map(function (r) {
                    return {
                        MaLenh: r.MaLenh || "",
                        TenHang: r.TenHang || "",
                        TenKH: r.TenKH || r.KhachHang || "",
                        SoLuong: toNumber(r.SLXuat || r.SoLuong),
                        SoBarCode: 0,
                    };
                });
            if (modalContent) {
                renderDayDetailTabs(modalContent, [], xuatFallback, [], plannedRows);
            }
        });
}

// v2.3.48 — Tabs cho Tổng quát khoảng ngày: nhập dùng grouped table (date+PINCC), còn lại giữ flat

window.dkToggleGroup = function (btn) {
    var groupKey = btn.getAttribute("data-group");
    var icon = btn.querySelector("i");
    var tbody = btn.closest("table").querySelector("tbody");
    var isExpanded = btn.getAttribute("data-expanded") === "true";

    btn.setAttribute("data-expanded", !isExpanded);
    if (isExpanded) {
        icon.style.transform = "rotate(-90deg)";
    } else {
        icon.style.transform = "rotate(0deg)";
    }

    var rows = tbody.querySelectorAll("tr[data-group='" + groupKey + "']:not(.group-header)");
    for (var i = 0; i < rows.length; i++) {
        rows[i].style.display = isExpanded ? "none" : "";
    }
};

function renderGroupedDetailTable(cols, rows, dateField) {
    if (!rows || rows.length === 0) return "<div class='dk-empty' style='padding:30px'>Không có dữ liệu</div>";

    var groups = {};
    for (var i = 0; i < rows.length; i++) {
        var r = rows[i];
        var rawDate =
            r[dateField] ||
            r.NgayNhapKho ||
            r.NgayNhap ||
            r.NgayXuat ||
            r.NgayKiemKe ||
            r.NgayNKDuKien ||
            r.ngayNKDuKien ||
            "";
        var dateStr = "";
        if (rawDate) {
            var d = new Date(rawDate);
            if (!isNaN(d)) {
                var dd = String(d.getDate()).padStart(2, "0");
                var mm = String(d.getMonth() + 1).padStart(2, "0");
                var yyyy = d.getFullYear();
                dateStr = dd + "/" + mm + "/" + yyyy;
            } else {
                dateStr = rawDate.toString().substring(0, 10);
            }
        } else {
            dateStr = "Không có ngày";
        }

        if (!groups[dateStr]) groups[dateStr] = [];
        groups[dateStr].push(r);
    }

    // Sort groups by date descending
    var sortedDates = Object.keys(groups).sort(function (a, b) {
        if (a === "Không có ngày") return 1;
        if (b === "Không có ngày") return -1;
        var partA = a.split("/");
        var partB = b.split("/");
        var dA = new Date(partA[2], partA[1] - 1, partA[0]);
        var dB = new Date(partB[2], partB[1] - 1, partB[0]);
        return dB - dA;
    });

    var html = "<div class='dk-detail-table-wrap'><table class='dk-detail-table'>";
    html += "<thead><tr>";
    for (var c = 0; c < cols.length; c++) {
        var w = cols[c].width ? "width:" + cols[c].width + ";" : "";
        var align = cols[c].align ? "text-align:" + cols[c].align + ";" : "";
        html += "<th style='" + w + align + "'>" + escapeHtml(cols[c].label) + "</th>";
    }
    html += "</tr></thead><tbody>";

    for (var gi = 0; gi < sortedDates.length; gi++) {
        var gDate = sortedDates[gi];
        var groupRows = groups[gDate];
        var groupKey = "g_" + gi;

        // Tính tổng
        var hasSoLuong = false;
        var dayTotal = 0;
        for (var ck = 0; ck < cols.length; ck++)
            if (cols[ck].key === "SoLuong" || cols[ck].key === "SoLuongDuKien") hasSoLuong = true;
        if (hasSoLuong) {
            for (var ri = 0; ri < groupRows.length; ri++) {
                var q = groupRows[ri].SoLuong || groupRows[ri].SoLuongDuKien || 0;
                dayTotal += parseFloat(q) || 0;
            }
        }

        // Group header
        var dark = document.body.classList.contains("dark-theme");
        var headBg = dark ? "#1e3a8a" : "#dbeafe"; // Nền xanh dương đặc màu (không dùng rgba)
        var headBorder = dark ? "#1d4ed8" : "#bfdbfe"; // Viền trên/dưới
        var dateColor = dark ? "#93c5fd" : "#1e40af"; // Màu chữ ngày
        var totalColor = dark ? "#60a5fa" : "#0284c7"; // Màu chữ tổng
        var subTextColor = dark ? "#bfdbfe" : "#475569"; // Màu chữ phụ

        html +=
            "<tr class='group-header' style='background-color: " +
            headBg +
            "; cursor: pointer; border-top: 1px solid " +
            headBorder +
            "; border-bottom: 1px solid " +
            headBorder +
            ";' onclick='window.dkToggleGroup(this)' data-group='" +
            groupKey +
            "' data-expanded='true'>";

        var qtyColIdx = -1;
        for (var ck = 0; ck < cols.length; ck++)
            if (cols[ck].key === "SoLuong" || cols[ck].key === "SoLuongDuKien") qtyColIdx = ck;

        if (qtyColIdx > 0 && hasSoLuong) {
            html +=
                "<td colspan='" +
                qtyColIdx +
                "' style='font-weight: bold; padding: 6px 8px 6px 12px; position: relative; text-align: left; border-left: 3px solid " +
                totalColor +
                ";'>";
            html +=
                "<i class='fas fa-chevron-down dk-toggle-icon' style='margin-right: 6px; font-size: 11px; transition: transform 0.2s; display: inline-block; color: " +
                subTextColor +
                ";'></i>";
            html +=
                "<span style='color: " +
                dateColor +
                "; font-size: 13px;'>&#128197; Ngày " +
                escapeHtml(gDate) +
                "</span>";
            html +=
                "<span style='font-size: 11px; font-weight: 500; color: " +
                subTextColor +
                "; margin-left: 8px;'>(" +
                formatNumber(groupRows.length, 0) +
                " dòng)</span>";
            html += "</td>";

            var cellAlign = cols[qtyColIdx].center ? "center" : cols[qtyColIdx].left ? "left" : "right";
            html +=
                "<td style='font-weight: bold; font-size: 13px; padding: 6px 8px; text-align: " +
                cellAlign +
                "; color: " +
                totalColor +
                ";'>" +
                formatNumber(dayTotal, 2) +
                "</td>";

            var remainingCols = cols.length - 1 - qtyColIdx;
            if (remainingCols > 0) {
                html += "<td colspan='" + remainingCols + "'></td>";
            }
        } else {
            html +=
                "<td colspan='" +
                cols.length +
                "' style='font-weight: bold; padding: 6px 8px 6px 12px; position: relative; text-align: left; border-left: 3px solid " +
                totalColor +
                ";'>";
            html +=
                "<i class='fas fa-chevron-down dk-toggle-icon' style='margin-right: 6px; font-size: 11px; transition: transform 0.2s; display: inline-block; color: " +
                subTextColor +
                ";'></i>";
            html +=
                "<span style='color: " +
                dateColor +
                "; font-size: 13px;'>&#128197; Ngày " +
                escapeHtml(gDate) +
                "</span>";
            if (hasSoLuong) {
                html +=
                    "<span style='margin-left: 12px; color: " +
                    totalColor +
                    "; font-size: 13px;'>Tổng: " +
                    formatNumber(dayTotal, 2) +
                    "</span>";
            }
            html +=
                "<span style='float: right; font-size: 11px; font-weight: 500; color: " +
                subTextColor +
                "; margin-top: 1px;'>" +
                formatNumber(groupRows.length, 0) +
                " dòng</span>";
            html += "</td>";
        }
        html += "</tr>";

        // Group rows
        for (var ri = 0; ri < groupRows.length; ri++) {
            var row = groupRows[ri];
            html += "<tr data-group='" + groupKey + "'>";
            for (var c = 0; c < cols.length; c++) {
                var cv = cols[c];
                var val = row[cv.field || cv.key];
                if (val === null || val === undefined) val = "";
                if (cv.key === "STT") val = ri + 1;
                if (cv.type === "number" || cv.number !== undefined)
                    val = formatNumber(val, cv.number !== undefined ? cv.number : 0);
                if (cv.type === "date" || cv.date)
                    val =
                        typeof formatDateVn === "function"
                            ? formatDateVn(val)
                            : typeof formatDate === "function"
                                ? formatDate(val)
                                : val;

                var cellAlign = cv.center ? "center" : cv.number !== undefined ? "right" : cv.left ? "left" : "";
                var align = cellAlign ? " style='text-align:" + cellAlign + "'" : "";
                html += "<td" + align + ">" + escapeHtml(val.toString()) + "</td>";
            }
            html += "</tr>";
        }
    }

    html += "</tbody></table></div>";
    return html;
}


window.__dkDetailData = {};
window.dkGoDetailPg = function (tabKey, dir) {
    var d = window.__dkDetailData[tabKey];
    if (!d) return;
    d.page += dir;
    var pageSize = 500;
    var totalPages = Math.ceil(d.rows.length / pageSize);
    if (d.page < 1) d.page = 1;
    if (d.page > totalPages) d.page = totalPages;

    var pagedRows = d.rows.slice((d.page - 1) * pageSize, d.page * pageSize);
    var html = renderGroupedDetailTable(d.cols, pagedRows, d.dateField);

    if (totalPages > 1) {
        var btnPrev = d.page > 1 ? '<button class="dk-btn-page" style="margin-right:10px" onclick="window.dkGoDetailPg(\'' + tabKey + '\', -1)">&#8592; Trang trước</button>' : '';
        var btnNext = d.page < totalPages ? '<button class="dk-btn-page" style="margin-left:10px" onclick="window.dkGoDetailPg(\'' + tabKey + '\', 1)">Trang sau &#8594;</button>' : '';
        html += '<div class="dk-detail-pagination">' +
            btnPrev + '<span>Trang ' + d.page + ' / ' + totalPages + ' (' + formatNumber(d.rows.length, 0) + ' dòng)</span>' + btnNext +
            '</div>';
    }
    var panel = document.getElementById(d.panelId);
    if (panel) {
        panel.innerHTML = html;
        panel.scrollTop = 0;
    }

    setTimeout(function () {
        var container = document.getElementById('detailModalContent');
        if (!container) return;
        var tabs = container.querySelector(".dk-day-tabs");
        if (tabs) {
            var tabsHeight = tabs.offsetHeight;
            var ths = container.querySelectorAll(".dk-detail-table thead th");
            for (var i = 0; i < ths.length; i++) {
                ths[i].style.top = tabsHeight + "px";
            }
        }
    }, 10);

};

function renderOverviewDetailTabs(container, nhap, xuat, kiemke, planned) {
    if (!container) return;

    function addStt(arr) {
        return (arr || []).map(function (r, i) {
            return Object.assign({ STT: i + 1 }, r);
        });
    }
    var tabs = [
        {
            key: "nhap",
            label: "Nhập kho",
            color: "#3b82f6",
            rows: addStt(nhap),
            cols: colsNhap(),
            dateField: "NgayNhap",
        },
        {
            key: "xuat",
            label: "Xuất kho",
            color: "#f97316",
            rows: addStt(xuat),
            cols: colsXuat(),
            dateField: "NgayXuat",
        },
        {
            key: "kiemke",
            label: "Kiểm kê",
            color: "#10b981",
            rows: addStt(kiemke),
            cols: colsKiemKe(),
            dateField: "NgayKiemKe",
        },
        {
            key: "plan",
            label: "NK dự kiến",
            color: "#8b5cf6",
            rows: addStt(planned),
            cols: colsPlanned(),
            dateField: "NgayNKDuKien",
        },
    ];

    var activeIdx = 0;
    for (var ti = 0; ti < tabs.length; ti++) {
        if (tabs[ti].rows.length > 0) {
            activeIdx = ti;
            break;
        }
    }

    var tabBar = '<div class="dk-day-tabs">';
    for (var i = 0; i < tabs.length; i++) {
        var t = tabs[i];
        var isActive = i === activeIdx ? " active" : "";
        tabBar +=
            '<button type="button" class="dk-day-tab' +
            isActive +
            '" data-tabkey="' +
            t.key +
            '" style="--tab-color:' +
            t.color +
            '">' +
            '<span class="dk-day-tab-dot" style="background:' +
            t.color +
            '"></span>' +
            escapeHtml(t.label) +
            ' <span class="dk-day-tab-count">' +
            formatNumber(t.rows.length, 0) +
            "</span>" +
            "</button>";
    }
    tabBar += "</div>";

    var panels = "";
    for (var pi = 0; pi < tabs.length; pi++) {
        var tp = tabs[pi];
        var hidden = pi === activeIdx ? "" : ' style="display:none"';
        var panelId = 'dk_dp_' + Math.random().toString(36).substr(2, 9) + '_' + tp.key;
        panels += '<div class="dk-day-panel" id="' + panelId + '" data-tabkey="' + tp.key + '"' + hidden + ' style="position:relative;">';

        if (tp.rows.length === 0) {
            panels +=
                '<div class="dk-empty" style="padding:30px">Không có dữ liệu ' +
                escapeHtml(tp.label.toLowerCase()) +
                "</div>";
        } else {
            window.__dkDetailData = window.__dkDetailData || {};
            window.__dkDetailData[tp.key] = { cols: tp.cols, rows: tp.rows, dateField: tp.dateField, page: 1, panelId: panelId };
            var pagedRows = tp.rows.slice(0, 500);
            var html = renderGroupedDetailTable(tp.cols, pagedRows, tp.dateField);
            var totalPages = Math.ceil(tp.rows.length / 500);
            if (totalPages > 1) {
                var btnNext = '<button class="dk-btn-page" style="margin-left:10px" onclick="window.dkGoDetailPg(\'' + tp.key + '\', 1)">Trang sau &#8594;</button>';
                html += '<div class="dk-detail-pagination">' +
                    '<span>Trang 1 / ' + totalPages + ' (' + formatNumber(tp.rows.length, 0) + ' dòng)</span>' + btnNext +
                    '</div>';
            }
            panels += html;
        }
        panels += "</div>";
    }

    container.innerHTML = tabBar + panels;

    setTimeout(function () {
        var container = document.getElementById('detailModalContent');
        if (!container) return;
        var tabs = container.querySelector(".dk-day-tabs");
        if (tabs) {
            var tabsHeight = tabs.offsetHeight;
            var ths = container.querySelectorAll(".dk-detail-table thead th");
            for (var i = 0; i < ths.length; i++) {
                ths[i].style.top = tabsHeight + "px";
            }
        }
    }, 10);




    var tabBtns = container.querySelectorAll(".dk-day-tab");
    for (var bi = 0; bi < tabBtns.length; bi++) {
        tabBtns[bi].addEventListener("click", function () {
            var key = this.getAttribute("data-tabkey");
            var allBtns = container.querySelectorAll(".dk-day-tab");
            for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
            this.classList.add("active");
            var allPanels = container.querySelectorAll(".dk-day-panel");
            for (var p = 0; p < allPanels.length; p++) {
                allPanels[p].style.display = allPanels[p].getAttribute("data-tabkey") === key ? "" : "none";
            }
            var searchInput = document.getElementById("detailSearchInput");
            if (searchInput && searchInput.value) {
                if (typeof filterDetailTable === "function") filterDetailTable(searchInput.value.trim());
            }
        });
    }
}

function renderDayDetailTabs(container, nhap, xuat, kiemke, planned) {
    if (!container) return;

    function addStt(arr) {
        return (arr || []).map(function (r, i) {
            return Object.assign({ STT: i + 1 }, r);
        });
    }
    var tabs = [
        {
            key: "nhap",
            label: "Nhập kho",
            color: "#3b82f6",
            rows: addStt(nhap),
            cols: colsNhap(),
            dateField: "NgayNhap",
        },
        {
            key: "xuat",
            label: "Xuất kho",
            color: "#f97316",
            rows: addStt(xuat),
            cols: colsXuat(),
            dateField: "NgayXuat",
        },
        {
            key: "kiemke",
            label: "Kiểm kê",
            color: "#10b981",
            rows: addStt(kiemke),
            cols: colsKiemKe(),
            dateField: "NgayKiemKe",
        },
        {
            key: "plan",
            label: "NK dự kiến",
            color: "#8b5cf6",
            rows: addStt(planned),
            cols: colsPlanned(),
            dateField: "NgayNKDuKien",
        },
    ];

    var activeIdx = 0;
    for (var ti = 0; ti < tabs.length; ti++) {
        if (tabs[ti].rows.length > 0) {
            activeIdx = ti;
            break;
        }
    }

    var tabBar = '<div class="dk-day-tabs">';
    for (var i = 0; i < tabs.length; i++) {
        var t = tabs[i];
        var isActive = i === activeIdx ? " active" : "";
        tabBar +=
            '<button type="button" class="dk-day-tab' +
            isActive +
            '" data-tabkey="' +
            t.key +
            '" style="--tab-color:' +
            t.color +
            '">' +
            '<span class="dk-day-tab-dot" style="background:' +
            t.color +
            '"></span>' +
            escapeHtml(t.label) +
            ' <span class="dk-day-tab-count">' +
            formatNumber(t.rows.length, 0) +
            "</span>" +
            "</button>";
    }
    tabBar += "</div>";

    var panels = "";
    for (var pi = 0; pi < tabs.length; pi++) {
        var tp = tabs[pi];
        var hidden = pi === activeIdx ? "" : ' style="display:none"';
        var panelId = 'dk_dp_' + Math.random().toString(36).substr(2, 9) + '_' + tp.key;
        panels += '<div class="dk-day-panel" id="' + panelId + '" data-tabkey="' + tp.key + '"' + hidden + ' style="position:relative;">';

        if (tp.rows.length === 0) {
            panels +=
                '<div class="dk-empty" style="padding:30px">Không có dữ liệu ' +
                escapeHtml(tp.label.toLowerCase()) +
                " trong ngày này</div>";
        } else {
            window.__dkDetailData = window.__dkDetailData || {};
            window.__dkDetailData[tp.key] = { cols: tp.cols, rows: tp.rows, dateField: tp.dateField, page: 1, panelId: panelId };
            var pagedRows = tp.rows.slice(0, 500);
            var html = renderGroupedDetailTable(tp.cols, pagedRows, tp.dateField);
            var totalPages = Math.ceil(tp.rows.length / 500);
            if (totalPages > 1) {
                var btnNext = '<button class="dk-btn-page" style="margin-left:10px" onclick="window.dkGoDetailPg(\'' + tp.key + '\', 1)">Trang sau &#8594;</button>';
                html += '<div class="dk-detail-pagination">' +
                    '<span>Trang 1 / ' + totalPages + ' (' + formatNumber(tp.rows.length, 0) + ' dòng)</span>' + btnNext +
                    '</div>';
            }
            panels += html;
        }
        panels += "</div>";
    }

    container.innerHTML = tabBar + panels;

    setTimeout(function () {
        var container = document.getElementById('detailModalContent');
        if (!container) return;
        var tabs = container.querySelector(".dk-day-tabs");
        if (tabs) {
            var tabsHeight = tabs.offsetHeight;
            var ths = container.querySelectorAll(".dk-detail-table thead th");
            for (var i = 0; i < ths.length; i++) {
                ths[i].style.top = tabsHeight + "px";
            }
        }
    }, 10);




    var tabBtns = container.querySelectorAll(".dk-day-tab");
    for (var bi = 0; bi < tabBtns.length; bi++) {
        tabBtns[bi].addEventListener("click", function () {
            var key = this.getAttribute("data-tabkey");
            var allBtns = container.querySelectorAll(".dk-day-tab");
            for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
            this.classList.add("active");
            var allPanels = container.querySelectorAll(".dk-day-panel");
            for (var p = 0; p < allPanels.length; p++) {
                allPanels[p].style.display = allPanels[p].getAttribute("data-tabkey") === key ? "" : "none";
            }
            var searchInput = document.getElementById("detailSearchInput");
            if (searchInput && searchInput.value) {
                if (typeof filterDetailTable === "function") filterDetailTable(searchInput.value.trim());
            }
        });
    }
}

// // v2.3.60 — Tải stats LPCP cho đúng tháng đang hiển thị trên lịch (Đã sửa để gọi render biểu đồ mới)
function loadLpcpStatsForMonth(baseDate) {
    // Stats strip was removed, but we still trigger the bottom charts rendering here
    renderLpcpBottomCharts();
}

function renderLpcpStatsPage3() {
    // Disabled since stats strip is removed
}

function bindLpcpStatsClick() {
    // Disabled since stats strip is removed
}

function showLpcpMonthModal(type) {
    var baseDate = calMonthDate || new Date();
    var y = baseDate.getFullYear();
    var m = baseDate.getMonth() + 1;
    var from = y + "-" + String(m).padStart(2, "0") + "-01";
    var to = y + "-" + String(m).padStart(2, "0") + "-" + String(new Date(y, m, 0).getDate()).padStart(2, "0");

    var modalTitle = byId(ids.detailModalTitle);
    var modalMeta = byId(ids.detailModalMeta);
    var modalContent = byId(ids.detailModalContent);
    var modal = byId(ids.detailModal);
    if (!modal) return;

    var titleMap = {
        tong: "Tất cả Task Phụ Liệu trong tháng",
        hoan: "Task Phụ Liệu - Hoàn thành",
        dang: "Task Phụ Liệu - Đang thực hiện",
        cho: "Task Phụ Liệu - Chờ thực hiện",
        canhbao: "Task Phụ Liệu - Cảnh báo / Chưa HT",
    };

    if (modalTitle) modalTitle.textContent = titleMap[type] || "Chi tiết Task";
    if (modalMeta) modalMeta.innerHTML = '<span style="color:#64748b;font-size:13px">Đang tải dữ liệu...</span>';
    if (modalContent) modalContent.innerHTML = '<div class="dk-empty" style="padding:30px">Đang tải dữ liệu...</div>';
    modal.classList.add("open");

    // Hide search bar for simplicity in this view
    var searchBar = document.querySelector(".dk-modal-search-bar");
    if (searchBar) searchBar.style.display = "none";

    var url = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" + from + "&denNgay=" + to;
    requestJson(url)
        .then(function (res) {
            var rawData = res && res.data ? res.data : [];
            var tasks = [];
            var seen = {};

            for (var i = 0; i < rawData.length; i++) {
                var dayData = rawData[i];
                var dayTasks = dayData.Tasks || [];
                var dateStr = String(dayData.NgayLam || "").substring(0, 10);
                var formattedDate = dateStr.split("-").reverse().join("/");

                for (var j = 0; j < dayTasks.length; j++) {
                    var t = dayTasks[j];
                    var key = (t.MaLenhSX || "") + "_" + dateStr;
                    if (!seen[key]) {
                        seen[key] = true;
                        var tt = t.TrangThai || 0;

                        var match = false;
                        if (type === "tong") match = true;
                        else if (type === "hoan" && tt === 2) match = true;
                        else if (type === "dang" && tt === 1) match = true;
                        else if (type === "cho" && tt === 0) match = true;
                        else if (type === "canhbao" && tt === 3) match = true;

                        if (match) {
                            t._formattedDate = formattedDate;
                            tasks.push(t);
                        }
                    }
                }
            }

            if (modalMeta) {
                modalMeta.innerHTML =
                    '<span style="font-weight:600;color:#3b82f6">Tìm thấy ' + tasks.length + " task</span>";
            }

            if (tasks.length === 0) {
                if (modalContent)
                    modalContent.innerHTML =
                        '<div class="dk-empty" style="padding:30px">Không có task nào trong tháng này.</div>';
                return;
            }

            var html =
                '<div style="width: 100%; background: var(--dk-card); border-radius: 8px; overflow: hidden; border: 1px solid var(--dk-line); box-shadow: 0 4px 6px rgba(0,0,0,0.02);">';
            html += '<div style="overflow-x: auto;">';
            html += '<table style="width:100%; border-collapse:collapse; min-width: 700px; font-size: 13px;">';
            html +=
                '<thead><tr style="background: var(--dk-card-alt, rgba(0,0,0,0.02)); border-bottom: 1px solid var(--dk-line);">';
            html +=
                '<th style="padding:14px 16px;text-align:left;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:45%;">Mã lệnh / Mã hàng</th>';
            html +=
                '<th style="padding:14px 16px;text-align:left;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:15%;">Ngày giao</th>';
            html +=
                '<th style="padding:14px 16px;text-align:left;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:25%;">Người thực hiện</th>';
            html +=
                '<th style="padding:14px 16px;text-align:center;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:15%;">Trạng thái</th>';
            html += "</tr></thead><tbody>";

            var STATUS_LABELS = ["Chờ TH", "Đang TH", "Hoàn thành", "Chưa HT"];
            var STATUS_BG = [
                "rgba(249,115,22,0.15)",
                "rgba(59,130,246,0.15)",
                "rgba(34,197,94,0.15)",
                "rgba(239,68,68,0.15)",
            ];
            var STATUS_COLOR = ["#f97316", "#3b82f6", "#22c55e", "#ef4444"];
            for (var k = 0; k < tasks.length; k++) {
                var tk = tasks[k];
                var tt = tk.TrangThai || 0;
                var stLabel = STATUS_LABELS[tt] || "Không rõ";
                var stBg = STATUS_BG[tt] || STATUS_BG[0];
                var stColor = STATUS_COLOR[tt] || STATUS_COLOR[0];

                html +=
                    '<tr style="transition: background 0.2s; border-bottom:1px solid var(--dk-line);" onmouseover="this.style.background=\'rgba(148,163,184,0.1)\'" onmouseout="this.style.background=\'transparent\'">';
                html +=
                    '<td style="padding:14px 16px;text-align:left;"><b>' +
                    escapeHtml(tk.MaLenhSX || "N/A") +
                    "</b>" +
                    (tk.MaHang
                        ? ' <span style="color:var(--dk-muted);"> - ' + escapeHtml(tk.MaHang) + "</span>"
                        : "") +
                    "</td>";
                html +=
                    '<td style="padding:14px 16px;text-align:left;color:var(--dk-muted);">' +
                    tk._formattedDate +
                    "</td>";
                html +=
                    '<td style="padding:14px 16px;text-align:left;">' +
                    escapeHtml(tk.TenNV || "Chưa phân công") +
                    "</td>";
                html +=
                    '<td style="padding:14px 16px;text-align:center;"><span style="background:' +
                    stBg +
                    ";color:" +
                    stColor +
                    ';padding:4px 10px;border-radius:6px;font-size:12px;font-weight:600;">' +
                    stLabel +
                    "</span></td>";
                html += "</tr>";
            }
            html += "</tbody></table></div></div>";

            if (modalContent) modalContent.innerHTML = html;
        })
        .catch(function (err) {
            if (modalContent)
                modalContent.innerHTML =
                    '<div class="dk-empty" style="padding:30px;color:#dc2626">Lỗi tải dữ liệu.</div>';
        });
}

// v2.3.60 — Render LPCP detail inline (sidebar phải page 3) khi click vào ô ngày
function showLpcpInlineDetail(dateKey) {
    var dateEl = document.getElementById("lpcpDetailDate");
    var badgeEl = document.getElementById("lpcpDetailBadge");
    var assignEl = document.getElementById("lpcpDetailAssignments");
    var pickEl = document.getElementById("lpcpDetailPickOrders");
    var warnEl = document.getElementById("lpcpDetailWarnings");

    // Format date display — "Thứ Tư, 27/05/2026"
    var parts = String(dateKey).split("-");
    var DOW_NAMES = [
        "Ch\u1ee7 Nh\u1eadt",
        "Th\u1ee9 Hai",
        "Th\u1ee9 Ba",
        "Th\u1ee9 T\u01b0",
        "Th\u1ee9 N\u0103m",
        "Th\u1ee9 S\u00e1u",
        "Th\u1ee9 B\u1ea3y",
    ];
    var displayDate = dateKey;
    if (parts.length === 3) {
        var dow = DOW_NAMES[new Date(dateKey + "T00:00:00").getDay()] || "";
        displayDate = dow + ", " + parts[2] + "/" + parts[1] + "/" + parts[0];
    }

    // Highlight selected cell
    var cells = document.querySelectorAll(".dk-cal-monthly-cell");
    for (var ci = 0; ci < cells.length; ci++) {
        cells[ci].classList.remove("is-selected");
        if (cells[ci].getAttribute("data-date") === dateKey) {
            cells[ci].classList.add("is-selected");
        }
    }

    // Mở sidebar
    if (window.dkOpenSidebar) {
        window.dkOpenSidebar();

        // Match side panel height to calendar height so it doesn't stretch the page
        setTimeout(function () {
            var calPanel = document.querySelector(".dk-cal-monthly-panel");
            var sidePanel = document.getElementById("lpcpDetailPanel");
            if (calPanel && sidePanel) {
                sidePanel.style.maxHeight = calPanel.offsetHeight + "px";
            }
        }, 50);
    }

    // Update header
    if (dateEl) dateEl.textContent = displayDate;
    if (badgeEl) badgeEl.textContent = "\u0110ang t\u1ea3i...";

    // Spinners
    var SPIN =
        "<div class='dk-lpcp-placeholder'><svg width='14' height='14' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><circle cx='12' cy='12' r='10'/><polyline points='12 6 12 12 16 14'/></svg>\u0110ang t\u1ea3i...</div>";
    if (assignEl) assignEl.innerHTML = SPIN;
    if (pickEl) pickEl.innerHTML = SPIN;
    if (warnEl) warnEl.innerHTML = SPIN;

    // Reset badge counts
    ["lpcpWarnCount", "lpcpTaskCount", "lpcpPickCount"].forEach(function (id) {
        var el = document.getElementById(id);
        if (el) {
            el.style.display = "none";
            el.textContent = "";
        }
    });

    var TT_LABELS = ["Ch\u1edd TH", "\u0110ang TH", "Ho\u00e0n th\u00e0nh", "Ch\u01b0a HT"];
    var TT_COLORS = ["#d97706", "#2563eb", "#059669", "#ef4444"];
    var TT_BG = ["rgba(217,119,6,0.1)", "rgba(37,99,235,0.1)", "rgba(5,150,105,0.1)", "rgba(239,68,68,0.15)"];

    function ttBadge(tt) {
        var i = (tt !== undefined && tt !== null) ? parseInt(tt, 10) : 0;
        return (
            "<span style='display:inline-flex;align-items:center;gap:4px;" +
            "background:" +
            (TT_BG[i] || TT_BG[0]) +
            ";color:" +
            (TT_COLORS[i] || TT_COLORS[0]) +
            ";" +
            "padding:2px 8px;border-radius:4px;font-size:10px;font-weight:500;flex-shrink:0;'>" +
            "<span style='width:5px;height:5px;border-radius:50%;background:" +
            (TT_COLORS[i] || TT_COLORS[0]) +
            ";display:inline-block;'></span>" +
            escapeHtml(TT_LABELS[i] || "?") +
            "</span>"
        );
    }

    function setBadge(id, count) {
        var el = document.getElementById(id);
        if (!el) return;
        if (count > 0) {
            el.textContent = count;
            el.style.display = "inline-block";
        } else {
            el.style.display = "none";
        }
    }

    requestJson("/api/DashboardKhoDesktop/LichPhanCong_GetDayDetail?ngay=" + dateKey)
        .then(function (res) {
            var data = res && res.data ? res.data : {};
            var assignments = data.Assignments || [];
            var pickOrders = data.PickOrders || [];

            // Tự tổng hợp PickOrders nếu SP chưa trả ResultSet 2
            if (pickOrders.length === 0 && assignments.length > 0) {
                var lenhMap = {},
                    lenhOrder = [];
                assignments.forEach(function (a) {
                    if (!a.MaLenhSX) return;
                    if (!lenhMap[a.MaLenhSX]) {
                        lenhMap[a.MaLenhSX] = {
                            MaLenhSX: a.MaLenhSX,
                            TrangThai: a.TrangThai || 0,
                            TenNV: a.TenNV || a.MaNV || "",
                            MaNV: a.MaNV || "",
                            MaKhachHang: a.MaKhachHang || "",
                            MaHang: a.MaHang || "",
                            NgaySoan: a.NgayThucHien || "",
                            GioSoan: a.GioThucHien || "",
                        };
                        lenhOrder.push(a.MaLenhSX);
                    }
                });
                lenhOrder.forEach(function (ma) {
                    pickOrders.push(lenhMap[ma]);
                });
            }

            // ── Xây dựng cảnh báo từ dữ liệu ──────────────────────────────────
            var warnings = [];
            var thieuNPL = [], chuaHT = [], choTH = [], thieuPO = [], tongThieu = 0;
            var thieuNPLItems = [], chuaHTItems = [], choTHItems = [];
            assignments.forEach(function (a) {
                if (a.ThieuNPL) { thieuNPL.push(formatMaLenhSX(a.MaLenhSX) || "?"); thieuNPLItems.push(a); }
                if (a.TrangThai === 3) { chuaHT.push(formatMaLenhSX(a.MaLenhSX) || "?"); chuaHTItems.push(a); }
                if (a.TrangThai === 0) { choTH.push(formatMaLenhSX(a.MaLenhSX) || "?"); choTHItems.push(a); }
            });
            var thieuPOItems = [];
            pickOrders.forEach(function (po) {
                if ((po.SoPLThieu || 0) > 0) {
                    tongThieu += po.SoPLThieu;
                    thieuPO.push(formatMaLenhSX(po.MaLenhSX) || "?");
                    thieuPOItems.push(po);
                }
            });
            if (thieuNPL.length)
                warnings.push({
                    type: "task",
                    level: "critical",
                    name: "Thiếu NPL phân công",
                    cnt: thieuNPL.length + " lệnh",
                    desc: thieuNPL.join(", "),
                    items: thieuNPLItems
                });
            if (thieuPO.length)
                warnings.push({
                    type: "thieuPO",
                    level: "critical",
                    name: "PL thiếu trong lệnh soạn",
                    cnt: thieuPO.length + " lệnh",
                    desc: thieuPO.join(", "),
                    items: thieuPOItems,
                });
            if (chuaHT.length)
                warnings.push({
                    type: "task",
                    level: "caution",
                    name: "Chưa hoàn thành đúng hạn",
                    cnt: chuaHT.length + " task",
                    desc: chuaHT.join(", ") || "Cần theo dõi và xử lý ngay",
                    items: chuaHTItems
                });
            if (choTH.length)
                warnings.push({
                    type: "task",
                    level: "info",
                    name: "Chờ bắt đầu TH",
                    cnt: choTH.length + " task",
                    desc: choTH.join(", ") || "Chưa triển khai trong ngày",
                    items: choTHItems
                });

            // Badge counts
            var totalWarnItems = thieuNPL.length + thieuPO.length + chuaHT.length;
            setBadge("lpcpWarnCount", totalWarnItems);
            setBadge("lpcpTaskCount", assignments.length);
            setBadge("lpcpPickCount", pickOrders.length);
            if (badgeEl)
                badgeEl.textContent = assignments.length + " task \u00b7 " + pickOrders.length + " l\u1ec7nh so\u1ea1n";

            // ── BẢNG 1: CẢNH BÁO ──────────────────────────────────────────────
            var wHtml = "";
            if (warnings.length === 0) {
                wHtml =
                    "<div class='dk-lpcp-empty' style='text-align:center;padding:16px 0;'>" +
                    "<svg width='22' height='22' viewBox='0 0 24 24' fill='none' stroke='#22c55e' stroke-width='2' style='display:block;margin:0 auto 6px;'><path d='M22 11.08V12a10 10 0 1 1-5.93-9.14'/><polyline points='22 4 12 14.01 9 11.01'/></svg>" +
                    "Kh\u00f4ng c\u00f3 c\u1ea3nh b\u00e1o</div>";
            } else {
                var WC = { critical: "#ef4444", caution: "#f59e0b", info: "#3b82f6" };
                var WB = {
                    critical: "rgba(239,68,68,0.15)",
                    caution: "rgba(245,158,11,0.15)",
                    info: "rgba(59,130,246,0.15)",
                };
                warnings.forEach(function (w) {
                    wHtml +=
                        "<div style='display:flex;align-items:flex-start;gap:8px;padding:7px 9px;border-radius:7px;" +
                        "background:" +
                        (WB[w.level] || "rgba(148,163,184,0.1)") +
                        ";border-left:3px solid " +
                        (WC[w.level] || "#94a3b8") +
                        ";margin-bottom:6px;'>" +
                        "<div style='flex:1;min-width:0;'>" +
                        "<div style='display:flex;align-items:center;justify-content:space-between;gap:6px;margin-bottom:2px;'>" +
                        "<span style='font-size:11px;font-weight:600;color:var(--dk-title);white-space:nowrap;overflow:hidden;text-overflow:ellipsis;'>" +
                        escapeHtml(w.name) +
                        "</span>" +
                        "<span style='font-size:10px;font-weight:700;background:" +
                        (WC[w.level] || "#94a3b8") +
                        "22;color:" +
                        (WC[w.level] || "#94a3b8") +
                        ";padding:1px 6px;border-radius:4px;flex-shrink:0;'>" +
                        escapeHtml(w.cnt) +
                        "</span></div>" +
                        "<div style='font-size:10px;color:var(--dk-muted);line-height:1.4;'>" +
                        escapeHtml(w.desc) +
                        "</div>" +
                        "</div></div>";
                });
            }
            if (warnEl) warnEl.innerHTML = wHtml;

            // ── BẢNG 2: TASK PENDING (Assignments) ────────────────────────────
            var aHtml = "";
            if (assignments.length === 0) {
                aHtml = "<div class='dk-lpcp-empty'>Kh\u00f4ng c\u00f3 ph\u00e2n c\u00f4ng</div>";
            } else {
                assignments.forEach(function (a) {
                    var name = escapeHtml(a.TenNV || a.MaNV || "\u2014");
                    var maLenh = formatMaLenhSX(a.MaLenhSX);
                    var brand = a.MaHang ? " \u00b7 " + escapeHtml(a.MaHang) : "";
                    var timeHtml = a.GioThucHien
                        ? "<span style='font-size:10px;color:var(--dk-muted);margin-left:6px;'><svg width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='margin-right:4px;vertical-align:-1px'><circle cx='12' cy='12' r='10'></circle><polyline points='12 6 12 12 16 14'></polyline></svg> " +
                        escapeHtml(a.GioThucHien) +
                        "</span>"
                        : "";
                    var descHtml = a.MoTaCongViec
                        ? "<div style='font-size:10px;color:var(--dk-muted);margin-top:2px;'><svg width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='margin-right:4px;opacity:0.7;vertical-align:-1px'><path d='M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z'></path><polyline points='14 2 14 8 20 8'></polyline><line x1='16' y1='13' x2='8' y2='13'></line><line x1='16' y1='17' x2='8' y2='17'></line><polyline points='10 9 9 9 8 9'></polyline></svg>" +
                        escapeHtml(a.MoTaCongViec) +
                        "</div>"
                        : "";

                    aHtml +=
                        "<div style='display:flex;align-items:flex-start;gap:8px;padding:7px 0;border-bottom:1px solid var(--dk-line);'>" +
                        "<div style='flex:1;min-width:0;'>" +
                        "<div style='font-size:11px;font-weight:600;color:var(--dk-primary,#2563eb);'>" +
                        maLenh +
                        brand +
                        timeHtml +
                        "</div>" +
                        "<div style='font-size:10px;color:var(--dk-title);margin-top:2px;opacity:0.9;'><svg width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='margin-right:4px;opacity:0.7;vertical-align:-1px'><path d='M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2'></path><circle cx='12' cy='7' r='4'></circle></svg>" +
                        name +
                        "</div>" +
                        descHtml +
                        "</div>" +
                        ttBadge(a.TrangThai || 0) +
                        "</div>";
                });
            }
            if (assignEl) assignEl.innerHTML = aHtml;

            // ── BẢNG 3: PHỤ LIỆU — SOẠN HÀNG (Pick Orders) ───────────────────
            var pHtml = "";
            if (pickOrders.length === 0) {
                pHtml = "<div class='dk-lpcp-empty'>Kh\u00f4ng c\u00f3 l\u1ec7nh so\u1ea1n h\u00e0ng</div>";
            } else {
                pickOrders.forEach(function (po) {
                    var maLenh = formatMaLenhSX(po.MaLenhSX);
                    var brand = po.MaHang
                        ? escapeHtml(po.MaHang)
                        : po.MaKhachHang
                            ? escapeHtml(po.MaKhachHang)
                            : "";
                    var nv = escapeHtml(po.TenNV || po.MaNV || "");
                    var timeHtml = po.GioSoan
                        ? "<span style='font-size:10px;color:var(--dk-muted);margin-left:6px;'><svg width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='margin-right:4px;vertical-align:-1px'><circle cx='12' cy='12' r='10'></circle><polyline points='12 6 12 12 16 14'></polyline></svg> " +
                        escapeHtml(po.GioSoan) +
                        "</span>"
                        : "";

                    var metricsHtml = "<div style='display:flex; flex-wrap:wrap; gap:6px; margin-top:6px;'>";
                    if (po.TongSLCanSoan > 0 || po.SLSoan > 0) {
                        metricsHtml += "<span style='font-size:10px; font-weight:500; color:var(--dk-title); background:rgba(255,255,255,0.05); border:1px solid var(--dk-line); padding:2px 6px; border-radius:4px;'>Yêu cầu: <b>" + formatNumber(po.TongSLCanSoan || 0, 2) + "</b></span>";
                        metricsHtml += "<span style='font-size:10px; font-weight:600; color:var(--dk-primary); background:rgba(59,130,246,0.1); border:1px solid rgba(59,130,246,0.2); padding:2px 6px; border-radius:4px;'>Đã soạn: " + formatNumber(po.SLSoan || 0, 2) + "</span>";
                    }
                    if ((po.SoPLThieu || 0) > 0) {
                        metricsHtml += "<span style='font-size:10px; font-weight:600; color:#ef4444; background:rgba(239,68,68,0.15); border:1px solid rgba(239,68,68,0.3); padding:2px 6px; border-radius:4px;'>⚠ Thiếu: " + formatNumber(po.SoPLThieu || 0, 2) + "</span>";
                    }
                    metricsHtml += "</div>";

                    pHtml +=
                        "<div style='padding:7px 0;border-bottom:1px solid var(--dk-line);'>" +
                        "<div style='display:flex;align-items:flex-start;justify-content:space-between;gap:6px;'>" +
                        "<div><span style='font-size:11px;font-weight:600;color:var(--dk-primary,#2563eb);'>" +
                        maLenh +
                        "</span>" +
                        timeHtml +
                        "</div>" +
                        ttBadge(po.TrangThai || 0) +
                        "</div>" +
                        (brand
                            ? "<div style='font-size:10px;color:var(--dk-title);margin-top:2px;opacity:0.9;'>" +
                            brand +
                            "</div>"
                            : "") +
                        (nv
                            ? "<div style='font-size:10px;color:var(--dk-title);margin-top:2px;opacity:0.9;'><svg width='10' height='10' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' style='margin-right:4px;opacity:0.7;vertical-align:-1px'><path d='M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2'></path><circle cx='12' cy='7' r='4'></circle></svg>" +
                            nv +
                            "</div>"
                            : "") +
                        metricsHtml +
                        "</div>";
                });
            }
            if (pickEl) pickEl.innerHTML = pHtml;

            // ── Bind "Xem chi tiết" buttons ───────────────────────────────────
            (function () {
                var _warnings = warnings;
                var _assignments = assignments;
                var _pickOrders = pickOrders;
                var _displayDate = displayDate;

                function bindDetailBtn(btnId, section) {
                    var btn = document.getElementById(btnId);
                    if (!btn) return;
                    // Clone to remove any previous listener
                    var fresh = btn.cloneNode(true);
                    btn.parentNode.replaceChild(fresh, btn);
                    fresh.addEventListener("click", function () {
                        openLpcpSectionModal(section, _warnings, _assignments, _pickOrders, _displayDate);
                    });
                }
                bindDetailBtn("lpcpWarnDetailBtn", "warn");
                bindDetailBtn("lpcpTaskDetailBtn", "task");
                bindDetailBtn("lpcpPickDetailBtn", "pick");
            })();
        })
        .catch(function (err) {
            var errMsg = err && err.message ? err.message : "Lỗi kết nối";
            if (badgeEl) badgeEl.textContent = "Lỗi tải dữ liệu";
            if (assignEl)
                assignEl.innerHTML =
                    "<div class='dk-lpcp-empty' style='color:#ef4444'>" + escapeHtml(errMsg) + "</div>";
            if (pickEl)
                pickEl.innerHTML =
                    "<div class='dk-lpcp-empty' style='color:#ef4444'>Kiểm tra console để biết chi tiết</div>";
        });
}

// ── v2.3.61 — Chi tiết từng mục trong sidebar (Cảnh báo / Task / Pick) ──────
function openLpcpSectionModal(section, warnings, assignments, pickOrders, dateLabel) {
    var modal = document.getElementById("detailModal");
    var titleEl = document.getElementById("detailModalTitle");
    var metaEl = document.getElementById("detailModalMeta");
    var contentEl = document.getElementById("detailModalContent");
    var headEl = modal ? modal.querySelector(".dk-modal-head") : null;
    if (!modal || !contentEl) return;

    // Hide search bar (not used here)
    var searchBar = document.querySelector(".dk-modal-search-bar");
    if (searchBar) searchBar.style.display = "none";

    // Apply section-specific header class
    if (headEl) {
        headEl.classList.remove("section-warn", "section-task", "section-pick");
        headEl.classList.add("section-" + section);
    }

    var TT_LABELS = ["Chờ TH", "Đang TH", "Hoàn thành", "Chưa HT"];
    var TT_SLUG = ["cho", "dang", "done", "chua"];

    function badge(tt) {
        var i = tt >= 0 && tt <= 3 ? tt : 0;
        return (
            "<span class='dk-lpcp-badge dk-lpcp-badge--" +
            TT_SLUG[i] +
            "'>" +
            "<span class='dk-lpcp-badge-dot'></span>" +
            escapeHtml(TT_LABELS[i]) +
            "</span>"
        );
    }

    var COLS_TASK = "grid-template-columns:2fr 1.5fr 1.2fr 1.1fr;";
    var COLS_PICK = "grid-template-columns: 1fr 2fr 1.5fr 1fr 1fr 1fr 1fr;";

    var html = "";

    if (section === "warn") {
        if (titleEl) titleEl.textContent = "Cảnh Báo — " + dateLabel;
        if (metaEl)
            metaEl.innerHTML = "<span style='color:#dc2626;font-weight:600;'>" + warnings.length + " cảnh báo</span>";

        var WL = { critical: "Nghiêm trọng", caution: "Cần chú ý", info: "Thông tin" };

        if (warnings.length === 0) {
            html = "<div style='text-align:center;padding:40px;color:#64748b;'>Không có cảnh báo</div>";
        } else {
            warnings.forEach(function (w) {
                var lvl = w.level || "info";
                html +=
                    "<div class='dk-lpcp-warn-row level-" +
                    lvl +
                    "'>" +
                    "<div class='dk-lpcp-warn-body'>" +
                    "<div class='dk-lpcp-warn-title-row'>" +
                    "<span class='dk-lpcp-warn-name'>" +
                    escapeHtml(w.name) +
                    "</span>" +
                    "<span class='dk-lpcp-warn-cnt'>" +
                    escapeHtml(w.cnt) +
                    "</span>" +
                    "<span class='dk-lpcp-warn-severity'>" +
                    escapeHtml(WL[lvl] || lvl) +
                    "</span>" +
                    "</div>" +
                    "<div class='dk-lpcp-warn-desc'>";

                if (w.type === "thieuPO" && w.items && w.items.length > 0) {
                    var subHtml = "<div class='dk-lpcp-modal-thead' style='" + COLS_PICK + "; margin-top:12px; border-radius:4px 4px 0 0;'>" +
                        "<span>Mã Lệnh SX</span><span>Mã Hàng</span><span>NV Soạn</span><span style='text-align:right;padding-right:12px;'>SL Yêu Cầu</span><span style='text-align:right;padding-right:12px;'>SL Đã Soạn</span><span>PL Thiếu</span><span>Trạng Thái</span></div>";
                    var wTotalYeuCau = 0, wTotalSoan = 0, wTotalThieu = 0;
                    w.items.forEach(function (po) {
                        wTotalYeuCau += (po.TongSLCanSoan || 0);
                        wTotalSoan += (po.SLSoan || 0);
                        wTotalThieu += (po.SoPLThieu || 0);

                        var maLenh = formatMaLenhSX(po.MaLenhSX);
                        var brand = escapeHtml(po.MaHang || "—");
                        var nv = escapeHtml(po.TenNV || po.MaNV || "—");
                        var thieu = "<span class='dk-lpcp-thieu-yes'>⚠ " + formatNumber(po.SoPLThieu, 2) + "</span>";
                        var slYeuCauHtml = "<span style='font-size:12px;font-weight:600;color:var(--dk-title);text-align:right;padding-right:12px;'>" + formatNumber(po.TongSLCanSoan || 0, 2) + "</span>";
                        var slSoanHtml = "<span style='font-size:12px;font-weight:600;color:var(--dk-primary);text-align:right;padding-right:12px;'>" + formatNumber(po.SLSoan || 0, 2) + "</span>";

                        subHtml += "<div class='dk-lpcp-modal-row' style='" + COLS_PICK + "'>" +
                            "<span class='dk-lpcp-col-code'>" + maLenh + "</span>" +
                            "<span class='dk-lpcp-col-brand'>" + brand + "</span>" +
                            "<span class='dk-lpcp-col-nv'>" + nv + "</span>" +
                            slYeuCauHtml + slSoanHtml + thieu + badge(po.TrangThai || 0) + "</div>";
                    });

                    var wTotalThieuHtml = wTotalThieu > 0
                        ? "<span class='dk-lpcp-thieu-yes'>⚠ " + formatNumber(wTotalThieu, 2) + "</span>"
                        : "<span class='dk-lpcp-thieu-no'>—</span>";

                    subHtml +=
                        "<div class='dk-lpcp-modal-row' style='" + COLS_PICK + "; background:var(--dk-card-alt, #f8fafc); border-top:1px solid var(--dk-line); border-bottom:none; border-radius:0 0 4px 4px;'>" +
                        "<span style='grid-column: 1 / 4; text-align:center; font-weight:700; color:var(--dk-title); font-size:12px;'>Tổng Cộng:</span>" +
                        "<span style='text-align:right; padding-right:12px; font-weight:700; color:var(--dk-title); font-size:13px;'>" + formatNumber(wTotalYeuCau, 2) + "</span>" +
                        "<span style='text-align:right; padding-right:12px; font-weight:700; color:var(--dk-primary); font-size:13px;'>" + formatNumber(wTotalSoan, 2) + "</span>" +
                        wTotalThieuHtml +
                        "<span></span></div>";

                    html += subHtml;
                } else if (w.type === "task" && w.items && w.items.length > 0) {
                    var subHtmlTask = "<div class='dk-lpcp-modal-thead' style='" + COLS_TASK + "; margin-top:12px; border-radius:4px 4px 0 0;'>" +
                        "<span>Mã Lệnh SX</span><span>Nhân Viên</span><span>Mã Hàng</span><span>Trạng Thái</span></div>";
                    w.items.forEach(function (a) {
                        var maLenh = formatMaLenhSX(a.MaLenhSX);
                        var nv = escapeHtml(a.TenNV || a.MaNV || "—");
                        var brand = escapeHtml(a.MaHang || "—");
                        var ngay = a.NgayThucHien ? "<span class='dk-lpcp-col-date'> · " + escapeHtml(a.NgayThucHien) + "</span>" : "";
                        subHtmlTask += "<div class='dk-lpcp-modal-row' style='" + COLS_TASK + "'>" +
                            "<span class='dk-lpcp-col-code'>" + maLenh + ngay + "</span>" +
                            "<span class='dk-lpcp-col-nv'>" + nv + "</span>" +
                            "<span class='dk-lpcp-col-brand'>" + brand + "</span>" +
                            badge(a.TrangThai || 0) + "</div>";
                    });
                    html += subHtmlTask;
                } else {
                    html += escapeHtml(w.desc);
                }

                html += "</div>" +
                    "</div></div>";
            });
        }
    } else if (section === "task") {
        if (titleEl) titleEl.textContent = "Task Pending — " + dateLabel;
        if (metaEl)
            metaEl.innerHTML =
                "<span style='color:#1d4ed8;font-weight:600;'>" + assignments.length + " phân công</span>";

        if (assignments.length === 0) {
            html = "<div style='text-align:center;padding:40px;color:#64748b;'>Không có phân công</div>";
        } else {
            html +=
                "<div class='dk-lpcp-modal-thead' style='" +
                COLS_TASK +
                "'>" +
                "<span>Mã Lệnh SX</span><span>Nhân Viên</span><span>Mã Hàng</span><span>Trạng Thái</span></div>";
            assignments.forEach(function (a) {
                var maLenh = formatMaLenhSX(a.MaLenhSX);
                var nv = escapeHtml(a.TenNV || a.MaNV || "—");
                var brand = escapeHtml(a.MaHang || "—");
                var ngay = a.NgayThucHien
                    ? "<span class='dk-lpcp-col-date'> · " + escapeHtml(a.NgayThucHien) + "</span>"
                    : "";
                html +=
                    "<div class='dk-lpcp-modal-row' style='" +
                    COLS_TASK +
                    "'>" +
                    "<span class='dk-lpcp-col-code'>" +
                    maLenh +
                    ngay +
                    "</span>" +
                    "<span class='dk-lpcp-col-nv'>" +
                    nv +
                    "</span>" +
                    "<span class='dk-lpcp-col-brand'>" +
                    brand +
                    "</span>" +
                    badge(a.TrangThai || 0) +
                    "</div>";
            });
        }
    } else if (section === "pick") {
        if (titleEl) titleEl.textContent = "Phụ Liệu — Soạn Hàng — " + dateLabel;
        if (metaEl)
            metaEl.innerHTML =
                "<span style='color:#15803d;font-weight:600;'>" + pickOrders.length + " lệnh soạn</span>";

        if (pickOrders.length === 0) {
            html = "<div style='text-align:center;padding:40px;color:#64748b;'>Không có lệnh soạn hàng</div>";
        } else {
            html +=
                "<div class='dk-lpcp-modal-thead' style='" +
                COLS_PICK +
                "'>" +
                "<span>Mã Lệnh SX</span><span>Mã Hàng</span><span>NV Soạn</span><span style='text-align:right;padding-right:12px;'>SL Yêu Cầu</span><span style='text-align:right;padding-right:12px;'>SL Đã Soạn</span><span>PL Thiếu</span><span>Trạng Thái</span></div>";
            var totalYeuCau = 0, totalSoan = 0, totalThieu = 0;
            pickOrders.forEach(function (po) {
                totalYeuCau += (po.TongSLCanSoan || 0);
                totalSoan += (po.SLSoan || 0);
                totalThieu += (po.SoPLThieu || 0);

                var maLenh = formatMaLenhSX(po.MaLenhSX);
                var brand = escapeHtml(po.MaHang || "—");
                var nv = escapeHtml(po.TenNV || po.MaNV || "—");
                var thieu =
                    (po.SoPLThieu || 0) > 0
                        ? "<span class='dk-lpcp-thieu-yes'>⚠ " + formatNumber(po.SoPLThieu, 2) + "</span>"
                        : "<span class='dk-lpcp-thieu-no'>—</span>";

                var slYeuCauHtml = "<span style='font-size:12px;font-weight:600;color:var(--dk-title);text-align:right;padding-right:12px;'>" + formatNumber(po.TongSLCanSoan || 0, 2) + "</span>";
                var slSoanHtml = "<span style='font-size:12px;font-weight:600;color:var(--dk-primary);text-align:right;padding-right:12px;'>" + formatNumber(po.SLSoan || 0, 2) + "</span>";

                html +=
                    "<div class='dk-lpcp-modal-row' style='" +
                    COLS_PICK +
                    "'>" +
                    "<span class='dk-lpcp-col-code'>" +
                    maLenh +
                    "</span>" +
                    "<span class='dk-lpcp-col-brand'>" +
                    brand +
                    "</span>" +
                    "<span class='dk-lpcp-col-nv'>" +
                    nv +
                    "</span>" +
                    slYeuCauHtml +
                    slSoanHtml +
                    thieu +
                    badge(po.TrangThai || 0) +
                    "</div>";
            });

            var totalThieuHtml = totalThieu > 0
                ? "<span class='dk-lpcp-thieu-yes'>⚠ " + formatNumber(totalThieu, 2) + "</span>"
                : "<span class='dk-lpcp-thieu-no'>—</span>";

            html +=
                "<div class='dk-lpcp-modal-row' style='" + COLS_PICK + "; background:var(--dk-card-alt, #f8fafc); border-top:1px solid var(--dk-line);'>" +
                "<span style='grid-column: 1 / 4; text-align:center; font-weight:700; color:var(--dk-title); font-size:12px;'>Tổng Cộng:</span>" +
                "<span style='text-align:right; padding-right:12px; font-weight:700; color:var(--dk-title); font-size:13px;'>" + formatNumber(totalYeuCau, 2) + "</span>" +
                "<span style='text-align:right; padding-right:12px; font-weight:700; color:var(--dk-primary); font-size:13px;'>" + formatNumber(totalSoan, 2) + "</span>" +
                totalThieuHtml +
                "<span></span></div>";
        }
    }

    contentEl.innerHTML = html;
    modal.classList.add("open");
}

function getDayOfWeekVN(date) {
    var days = ["CN", "2", "3", "4", "5", "6", "7"];
    return days[date.getDay()] || "";
}

// v2.3.54 — Append "Phân công PL" tab vào modal sau 4 tabs kho
function appendLpcpTab(container, lpcpData) {
    if (!container) return;
    var assignments = lpcpData.Assignments || [];
    var pickOrders = lpcpData.PickOrders || [];
    var total = assignments.length + pickOrders.length;

    var isDark = document.body.classList.contains("dark-theme");
    var TT_LABELS = ["Chờ thực hiện", "Đang thực hiện", "Hoàn thành", "Chưa hoàn thành"];
    var TT_COLORS = ["#d97706", "#3b82f6", "#10b981", "#ef4444"]; // Adjusted colors slightly for better contrast
    var TT_BG = isDark
        ? ["rgba(217,119,6,0.15)", "rgba(59,130,246,0.15)", "rgba(16,185,129,0.15)", "rgba(239,68,68,0.15)"]
        : ["#fffbeb", "#eff6ff", "#f0fdf4", "#fef2f2"];

    var cardBg = isDark ? "rgba(255,255,255,0.05)" : "#f8fafc";
    var borderColor = isDark ? "rgba(255,255,255,0.1)" : "#e2e8f0";
    var textColor = isDark ? "#f8fafc" : "#1a2332";
    var subColor = isDark ? "#94a3b8" : "#64748b";

    function ttBadge(tt) {
        var i = (tt !== undefined && tt !== null) ? parseInt(tt, 10) : 0;
        return (
            '<span style="display:inline-flex;align-items:center;gap:4px;' +
            "background:" +
            (TT_BG[i] || TT_BG[0]) +
            ";color:" +
            (TT_COLORS[i] || TT_COLORS[0]) +
            ";" +
            'padding:2px 8px;border-radius:4px;font-size:10px;font-weight:500;">' +
            '<span style="width:5px;height:5px;border-radius:50%;background:' +
            (TT_COLORS[i] || TT_COLORS[0]) +
            ';display:inline-block;"></span>' +
            escapeHtml(TT_LABELS[i] || "") +
            "</span>"
        );
    }

    // Build tab button
    var tabBar = container.querySelector(".dk-day-tabs");
    if (!tabBar) return;
    var tabBtn = document.createElement("button");
    tabBtn.type = "button";
    tabBtn.className = "dk-day-tab";
    tabBtn.setAttribute("data-tabkey", "lpcp");
    tabBtn.style.setProperty("--tab-color", "#6366f1");
    tabBtn.innerHTML =
        '<span class="dk-day-tab-dot" style="background:#6366f1"></span>' +
        "Ph\u00e2n c\u00f4ng PL" +
        ' <span class="dk-day-tab-count">' +
        total +
        "</span>";
    tabBar.appendChild(tabBtn);

    // Build panel HTML
    var html = "";

    // Left: Phân công nhân viên
    html += '<div style="display:grid;grid-template-columns:1fr 1fr;gap:12px;padding:12px;">';

    html += "<div>";
    html +=
        '<div style="font-size:10px;font-weight:700;color:' +
        subColor +
        ";text-transform:uppercase;" +
        'letter-spacing:.05em;margin-bottom:8px;">Phân công công việc</div>';
    if (assignments.length === 0) {
        html +=
            '<div style="color:' +
            subColor +
            ';font-size:12px;text-align:center;padding:20px 0;">Không có phân công</div>';
    } else {
        assignments.forEach(function (a, i) {
            var name = escapeHtml(a.TenNV || a.MaNV || "");
            var initials = name
                .replace(/NV\.|LH\./g, "")
                .substring(0, 2)
                .toUpperCase();
            var bgColors = ["#3b82f6", "#a855f7", "#14b8a6", "#f97316", "#22c55e", "#ef4444"];
            var bg = bgColors[(name.charCodeAt(0) || 0) % bgColors.length];
            html +=
                '<div style="display:flex;align-items:center;gap:8px;padding:6px 10px;' +
                "background:" +
                cardBg +
                ";border-radius:8px;border:1px solid " +
                borderColor +
                ';margin-bottom:6px;">' +
                '<span style="width:28px;height:28px;border-radius:50%;background:' +
                bg +
                ";" +
                "display:inline-flex;align-items:center;justify-content:center;color:#fff;" +
                'font-size:11px;font-weight:700;flex-shrink:0;">' +
                initials +
                "</span>" +
                '<div style="flex:1;overflow:hidden;">' +
                '<div style="font-size:12px;font-weight:600;color:' +
                textColor +
                ';">' +
                name +
                "</div>" +
                '<div style="font-size:10px;color:' +
                subColor +
                ';">' +
                escapeHtml(a.MoTaCongViec || "") +
                "</div>" +
                "</div>" +
                ttBadge(a.TrangThai || 0) +
                "</div>";
        });
    }
    html += "</div>";

    // Right: Phụ liệu soạn hàng
    html += "<div>";
    html +=
        '<div style="font-size:10px;font-weight:700;color:' +
        subColor +
        ";text-transform:uppercase;" +
        'letter-spacing:.05em;margin-bottom:8px;">Phụ liệu — Soạn hàng</div>';
    if (pickOrders.length === 0) {
        html +=
            '<div style="color:' +
            subColor +
            ';font-size:12px;text-align:center;padding:20px 0;">Không có lệnh soạn hàng</div>';
    } else {
        pickOrders.forEach(function (po) {
            var gioNgay =
                (po.GioSoan ? po.GioSoan + " " : "") +
                (po.NgaySoan || "").replace(/(\d{4})-(\d{2})-(\d{2})/, "$3/$2/$1");
            var name = escapeHtml(po.TenNV || po.MaNV || "-");
            var initials2 = name
                .replace(/NV\.|LH\./g, "")
                .substring(0, 2)
                .toUpperCase();
            var bgColors = ["#3b82f6", "#a855f7", "#14b8a6", "#f97316", "#22c55e", "#ef4444"];
            var bg2 = bgColors[(name.charCodeAt(0) || 0) % bgColors.length];
            html +=
                '<div style="display:flex;align-items:center;gap:8px;padding:6px 10px;' +
                "background:" +
                cardBg +
                ";border-radius:8px;border:1px solid " +
                borderColor +
                ';margin-bottom:6px;">' +
                '<div style="flex:1;overflow:hidden;">' +
                '<div style="font-size:12px;">' +
                '<span style="font-weight:600;color:#3b82f6;">Lệnh ' +
                formatMaLenhSX(po.MaLenhSX) +
                "</span>" +
                '<span style="font-size:10px;color:' +
                subColor +
                ';margin-left:5px;">' +
                escapeHtml(po.MaHang || "") +
                "</span>" +
                "</div>" +
                '<div style="font-size:10px;color:' +
                subColor +
                ';display:flex;align-items:center;gap:5px;margin-top:2px;">' +
                '<span style="width:16px;height:16px;border-radius:50%;background:' +
                bg2 +
                ";" +
                'display:inline-flex;align-items:center;justify-content:center;color:#fff;font-size:8px;font-weight:700;">' +
                initials2 +
                "</span>" +
                "<span>" +
                name +
                "</span><span>·</span><span>" +
                escapeHtml(gioNgay) +
                "</span>" +
                "</div>" +
                "</div>" +
                ttBadge(po.TrangThai || 0) +
                "</div>";
        });
    }
    html += "</div></div>"; // grid end

    // Create panel div
    var panel = document.createElement("div");
    panel.className = "dk-day-panel";
    panel.setAttribute("data-tabkey", "lpcp");
    panel.style.display = "none";
    panel.innerHTML = html;
    container.appendChild(panel);

    // Bind click for new tab button (re-use existing tab logic)
    tabBtn.addEventListener("click", function () {
        var allBtns = container.querySelectorAll(".dk-day-tab");
        for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
        tabBtn.classList.add("active");
        var allPanels = container.querySelectorAll(".dk-day-panel");
        for (var p = 0; p < allPanels.length; p++) {
            allPanels[p].style.display = allPanels[p].getAttribute("data-tabkey") === "lpcp" ? "" : "none";
        }
    });
}

function renderAll() {
    renderMetricCards();
    renderMoMStrip();
    populateCustomerFilter();
    renderCharts();
    renderCustomersTable();
    renderRacksTable();
    // renderRacksHeatmap();

    // v2.4.0 — Các section bổ sung
    renderTodoList();
    renderTop5VTTable();
    renderTop5KHTable();
    renderHetHanTable();
    renderGiaTriTheoNhomChart();
    renderKiemKeBox();

    // Page 2 widgets
    renderAlertsList();
    renderHieuSuatGauges();

    // Lịch hoạt động kho
    if (state.activityCalendar && state.activityCalendar.length > 0) {
        renderActivityCalendar();
        renderActivityCalendarMonthly();
    }

    // Biểu đồ LPCP
    if (typeof renderLpcpBottomCharts === "function") {
        renderLpcpBottomCharts();
    }

    setTimeout(injectMaximizeButtons, 80);

    // v2.3.6 — Nếu chưa có data lịch (vd ở Page 1), fetch bổ sung
    if (!isDemoMode && currentPage !== 3 && (!state.activityCalendar || state.activityCalendar.length === 0)) {
        var now = new Date();
        var from = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        var to = new Date(now.getFullYear(), now.getMonth() + 2, 0);
        var url =
            "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
        requestJson(url)
            .then(function (data) {
                if (!state.activityCalendar || state.activityCalendar.length === 0) {
                    state.activityCalendar = normalizeArray(data);
                }
                renderActivityCalendar();
                renderActivityCalendarMonthly();
            })
            .catch(function () { });
    }
}

// ─── Pagination System ────────────────────────────────────────────────────
var currentPage = 1;

var autoRotateTimer = null;

var AUTO_ROTATE_INTERVAL = 15000; // 15 giây

function switchPage(pageNum) {
    currentPage = pageNum;

    var btns = document.querySelectorAll(".dk-page-btn, .dk-topbar-nav-btn");
    for (var j = 0; j < btns.length; j++) {
        var bn = parseInt(btns[j].getAttribute("data-page"), 10);
        if (bn === pageNum) btns[j].classList.add("active");
        else btns[j].classList.remove("active");
    }

    setTimeout(function () {
        try {
            loadPageData(pageNum);
        } catch (e) { }
        var pages = document.querySelectorAll(".dk-page");
        for (var i = 0; i < pages.length; i++) {
            var pn = parseInt(pages[i].getAttribute("data-page"), 10);
            if (pn === pageNum) {
                pages[i].classList.add("dk-page-active");
                pages[i].style.display = "flex";
            } else {
                pages[i].classList.remove("dk-page-active");
                pages[i].style.display = "none";
            }
        }

        var gsInput = document.getElementById("dkGlobalSearch");
        if (gsInput) {
            var gsWrap = gsInput.closest
                ? gsInput.closest(".dk-topbar-search, .input-group, .dk-search-wrap")
                : gsInput.parentElement;
            if (!gsWrap) gsWrap = gsInput.parentElement;
            if (gsWrap) gsWrap.style.display = pageNum === 3 ? "none" : "";
        }

        if (pageNum === 2) {
            setTimeout(function () {
                try {
                    renderAlertsList();
                } catch (e) { }
                try {
                    renderHieuSuatGauges();
                } catch (e) { }
                try {
                    renderTop5MaxChart();
                } catch (e) { }
                try {
                    renderAgeStockChart();
                } catch (e) { }
            }, 50);
        }

        if (typeof Highcharts !== "undefined") {
            setTimeout(function () {
                var charts = Highcharts.charts || [];
                for (var c = 0; c < charts.length; c++) {
                    if (charts[c]) charts[c].reflow();
                }
            }, 100);
        }
    }, 10);
    var topbar = document.querySelector(".dk-topbar");
    if (topbar) {
        if (pageNum === 3) {
            topbar.classList.add("filter-hidden");
        } else {
            topbar.classList.remove("filter-hidden");
        }
    }
}

function startAutoRotate() {
    stopAutoRotate();
    autoRotateTimer = setInterval(function () {
        var next = currentPage >= 3 ? 1 : currentPage + 1;
        switchPage(next);
    }, AUTO_ROTATE_INTERVAL);
}

function stopAutoRotate() {
    if (autoRotateTimer) {
        clearInterval(autoRotateTimer);
        autoRotateTimer = null;
    }
}

function bindPageNav() {
    var nav = byId("pageNav");
    if (!nav) return;
    nav.addEventListener("click", function (e) {
        // v2.6.0 — Match both old sidebar buttons and new topbar nav buttons
        var btn = e.target.closest(".dk-page-btn, .dk-topbar-nav-btn");
        if (!btn) return;
        var page = parseInt(btn.getAttribute("data-page"), 10);
        if (page) {
            switchPage(page);
        }
    });
}

function getDetailData(detail, index) {
    var overall = state.overall.length > 0 ? state.overall[0] : {};
    var totalCapRow = {
        TotalCapacity: toNumber(overall.TotalCapacity),
        CapacityNPL: toNumber(overall.CapacityNPL),
        CapacityPL: toNumber(overall.CapacityPL),
        UsedNPL: toNumber(overall.UsedNPL),
        UsedPL: toNumber(overall.UsedPL),
        UsedTotal: toNumber(overall.UsedNPL) + toNumber(overall.UsedPL),
        TotalFreeCapacity: toNumber(overall.TotalFreeCapacity),
        TotalVatTu: toNumber(overall.TotalVatTu),
        PercentNPL: toNumber(overall.PercentNPL),
        PercentPL: toNumber(overall.PercentPL),
        TotalPercent: toNumber(overall.TotalPercent),
        FreePercent: toNumber(overall.FreePercent),
    };

    // Chi tiết tổng sức chứa: hiện tất cả kệ, NL trước rồi PL
    var racksForCapDetail = state.racks
        .slice()
        .sort(function (a, b) {
            var mA = toNumber(a.Module),
                mB = toNumber(b.Module);
            if (mA !== mB) return mA - mB;
            return (a.TenDay || "").localeCompare(b.TenDay || "");
        })
        .map(function (r, idx) {
            var used = toNumber(r.TongCBMSuDungTrongKe);
            var cap = toNumber(r.TongCBMTrongKe);
            return {
                STT: idx + 1,
                LoaiKho: r.Module == 1 ? "NL" : r.Module == 2 ? "PL" : String(r.Module || ""),
                TenDay: r.TenDay || "",
                TenKe: r.TenKe || "",
                CBMSuDung: used,
                TongCBM: cap,
                PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                SLVatTu: toNumber(r.SLVatTu),
            };
        });

    var detailMap = {
        totalCapacity: {
            title: "Chi tiết tổng sức chứa — theo kệ",
            isWarehouseMap: true,
            rows: racksForCapDetail,
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "LoaiKho", label: "Loại kho", center: true, width: 85 },
                { key: "TenDay", label: "Dãy", center: true, width: 80 },
                { key: "TenKe", label: "Tên kệ", center: true, width: 95 },
                { key: "CBMSuDung", label: "CBM sử dụng", number: 2, width: 110 },
                { key: "TongCBM", label: "Tổng CBM", number: 2, width: 110 },
                { key: "PctSuDung", label: "% sử dụng", percent: true, width: 95 },
                { key: "SLVatTu", label: "Số vật tư", number: 0, center: true, width: 85 },
            ],
        },
        materialCount: (function () {
            var soMaVatTu =
                state.distinctMat && state.distinctMat.length > 0 ? toNumber(state.distinctMat[0].SoMaVatTu) : 0;
            var totalRacks = state.racks.length;
            var racksAbove85 = 0,
                racksOver100 = 0,
                racksUnder50 = 0;
            var totalUsedCBM = 0;
            for (var _ri = 0; _ri < state.racks.length; _ri++) {
                var _r = state.racks[_ri];
                var _used = toNumber(_r.TongCBMSuDungTrongKe);
                var _cap = toNumber(_r.TongCBMTrongKe);
                totalUsedCBM += _used;
                var _pct = _cap > 0 ? (_used / _cap) * 100 : 0;
                if (_pct > 100) racksOver100++;
                else if (_pct >= 85) racksAbove85++;
                else if (_pct < 50) racksUnder50++;
            }
            var rows = [
                { ChiSo: "Mã vật tư", GiaTri: soMaVatTu, drill: "matCountDrill_codes" },
                { ChiSo: "Số lượng tồn NL", GiaTri: toNumber(overall.TotalVatTuNPL) },
                { ChiSo: "Số lượng tồn PL", GiaTri: toNumber(overall.TotalVatTuPL) },
                { ChiSo: "Tổng số lượng tồn", GiaTri: toNumber(overall.TotalVatTu) },
                { ChiSo: "Tổng số kệ đang dùng", GiaTri: totalRacks, drill: "matCountDrill_allRacks" },
                { ChiSo: "Kệ < 50% lấp đầy", GiaTri: racksUnder50, drill: "matCountDrill_under50" },
                { ChiSo: "Kệ 85% - 100% lấp đầy", GiaTri: racksAbove85, drill: "matCountDrill_85to100" },
                { ChiSo: "Kệ > 100% (vượt tải)", GiaTri: racksOver100, drill: "matCountDrill_over100" },
                { ChiSo: "CBM đang sử dụng", GiaTri: totalUsedCBM, isDecimal: true },
            ];
            return {
                title: "Chi tiết tổng hợp vật tư trong kho",
                rows: rows.map(function (r, i) {
                    var formattedValue = r.isDecimal ? formatNumber(r.GiaTri, 2) : formatNumber(r.GiaTri, 0);
                    var canDrill = !!r.drill;
                    return {
                        STT: i + 1,
                        ChiSo: r.ChiSo,
                        GiaTri: canDrill
                            ? '<a href="#" class="dk-link js-open-detail" data-detail="' +
                            r.drill +
                            '">' +
                            formattedValue +
                            "</a>"
                            : formattedValue,
                        _raw: r,
                    };
                }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "ChiSo", label: "Chỉ số", width: 250 },
                    { key: "GiaTri", label: "Giá trị", raw: true, center: true, width: 130 },
                ],
            };
        })(),
        matCountDrill_codes: {
            title: "Danh sách tất cả mã vật tư đang tồn kho",
            rows: [],
            // v2.4.0 — Cột mới: STT, Loại, ItemCode, Chi tiết, Màu(mã+tên), Khổ vải(+đv), Tồn kho, Số kiện/roll
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "LoaiKho", label: "Loại", center: true, width: 70 },
                { key: "ItemCode", label: "ItemCode", width: 120 },
                { key: "ChiTiet", label: "Chi tiết" },
                { key: "MauDisplay", label: "Màu", raw: true, width: 160 },
                { key: "KhoVai", label: "Khổ vải", center: true, width: 100 },
                { key: "TonKho", label: "Tồn kho", number: 2, width: 100 },
                { key: "SoBarCode", label: "Số kiện/roll", number: 0, center: true, width: 100 },
            ],
            customAsync: "allMaterials",
        },
        // v2.4.4 — 6 modal "Xem chi tiết" mới: shell + customAsync dispatcher
        todoDetail: { title: "Công việc chờ xử lý", rows: [], columns: [], customAsync: "todoDetail" },
        top5VTAll: { title: "Toàn bộ vật tư theo dung tích", rows: [], columns: [], customAsync: "top5VTAll" },
        top5KHAll: { title: "Toàn bộ khách hàng theo giá trị tồn", rows: [], columns: [], customAsync: "top5KHAll" },
        hetHanAll: { title: "Vật tư sắp hết hạn", rows: [], columns: [], customAsync: "hetHanAll" },
        giaTriNhomAll: { title: "Chi tiết giá trị tồn theo nhóm", rows: [], columns: [], customAsync: "giaTriNhomAll" },
        kiemKeAll: { title: "Chi tiết kiểm kê", rows: [], columns: [], customAsync: "kiemKeAll" },
        // v2.4.15 — 5 modal KPI header
        tonDauKyDetail: { title: "Tồn đầu kỳ", rows: [], columns: [], customAsync: "tonDauKyDetail" },
        tongNhapDetail: { title: "Chi tiết nhập kho", rows: [], columns: [], customAsync: "tongNhapDetail" },
        tongXuatDetail: { title: "Chi tiết xuất kho", rows: [], columns: [], customAsync: "tongXuatDetail" },
        tonKhoDetail: { title: "Chi tiết tồn kho", rows: [], columns: [], customAsync: "tonKhoDetail" },
        poTreDetail: { title: "PO đang trễ", rows: [], columns: [], customAsync: "poTreDetail" },
        // v2.4.16 — chi tiết cảnh báo tồn kho (theo MaCB)
        alertDetail: { title: "Chi tiết cảnh báo", rows: [], columns: [], customAsync: "alertDetail" },
        matCountDrill_allRacks: (function () {
            var rows = state.racks.map(function (r, i) {
                var used = toNumber(r.TongCBMSuDungTrongKe);
                var cap = toNumber(r.TongCBMTrongKe);
                return {
                    STT: i + 1,
                    Module: r.Module == 1 ? "NL" : "PL",
                    TenDay: r.TenDay || "",
                    TenKe: r.TenKe || "",
                    CBMSuDung: used,
                    TongCBM: cap,
                    PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                    SLVatTu: toNumber(r.SLVatTu),
                };
            });
            return {
                title: "Danh sách kệ đang sử dụng",
                rows: rows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true },
                    { key: "Module", label: "Loại kho", center: true },
                    { key: "TenDay", label: "Dãy", center: true },
                    { key: "TenKe", label: "Tên kệ", center: true },
                    { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                    { key: "TongCBM", label: "Tổng CBM", number: 2 },
                    { key: "PctSuDung", label: "% sử dụng", percent: true },
                    { key: "SLVatTu", label: "Số vật tư", number: 0, center: true },
                ],
            };
        })(),
        matCountDrill_under50: (function () {
            var rows = state.racks
                .filter(function (r) {
                    var cap = toNumber(r.TongCBMTrongKe);
                    var pct = cap > 0 ? (toNumber(r.TongCBMSuDungTrongKe) / cap) * 100 : 0;
                    return pct < 50;
                })
                .map(function (r, i) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: i + 1,
                        Module: r.Module == 1 ? "NL" : "PL",
                        TenDay: r.TenDay || "",
                        TenKe: r.TenKe || "",
                        CBMSuDung: used,
                        TongCBM: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu),
                    };
                });
            return {
                title: "Danh sách kệ < 50% lấp đầy (còn trống nhiều)",
                rows: rows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true },
                    { key: "Module", label: "Loại kho", center: true },
                    { key: "TenDay", label: "Dãy", center: true },
                    { key: "TenKe", label: "Tên kệ", center: true },
                    { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                    { key: "TongCBM", label: "Tổng CBM", number: 2 },
                    { key: "PctSuDung", label: "% sử dụng", percent: true },
                    { key: "SLVatTu", label: "Số vật tư", number: 0, center: true },
                ],
            };
        })(),
        matCountDrill_85to100: (function () {
            var rows = state.racks
                .filter(function (r) {
                    var cap = toNumber(r.TongCBMTrongKe);
                    var pct = cap > 0 ? (toNumber(r.TongCBMSuDungTrongKe) / cap) * 100 : 0;
                    return pct >= 85 && pct <= 100;
                })
                .map(function (r, i) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: i + 1,
                        Module: r.Module == 1 ? "NL" : "PL",
                        TenDay: r.TenDay || "",
                        TenKe: r.TenKe || "",
                        CBMSuDung: used,
                        TongCBM: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu),
                    };
                });
            return {
                title: "Danh sách kệ 85% - 100% lấp đầy (gần đầy)",
                rows: rows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true },
                    { key: "Module", label: "Loại kho", center: true },
                    { key: "TenDay", label: "Dãy", center: true },
                    { key: "TenKe", label: "Tên kệ", center: true },
                    { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                    { key: "TongCBM", label: "Tổng CBM", number: 2 },
                    { key: "PctSuDung", label: "% sử dụng", percent: true },
                    { key: "SLVatTu", label: "Số vật tư", number: 0, center: true },
                ],
            };
        })(),
        matCountDrill_over100: (function () {
            var rows = state.racks
                .filter(function (r) {
                    var cap = toNumber(r.TongCBMTrongKe);
                    var pct = cap > 0 ? (toNumber(r.TongCBMSuDungTrongKe) / cap) * 100 : 0;
                    return pct > 100;
                })
                .map(function (r, i) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: i + 1,
                        Module: r.Module == 1 ? "NL" : "PL",
                        TenDay: r.TenDay || "",
                        TenKe: r.TenKe || "",
                        CBMSuDung: used,
                        TongCBM: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu),
                    };
                });
            return {
                title: "Danh sách kệ > 100% (vượt tải - CẢNH BÁO)",
                rows: rows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true },
                    { key: "Module", label: "Loại kho", center: true },
                    { key: "TenDay", label: "Dãy", center: true },
                    { key: "TenKe", label: "Tên kệ", center: true },
                    { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                    { key: "TongCBM", label: "Tổng CBM", number: 2 },
                    { key: "PctSuDung", label: "% sử dụng", percent: true },
                    { key: "SLVatTu", label: "Số vật tư", number: 0, center: true },
                ],
            };
        })(),
        capacitySummary: {
            title: "Tóm tắt sức chứa kho",
            rows: [
                (function () {
                    var soMaVatTu2 =
                        state.distinctMat && state.distinctMat.length > 0
                            ? toNumber(state.distinctMat[0].SoMaVatTu)
                            : toNumber(overall.TotalVatTu);
                    return {
                        TotalCapacity: toNumber(overall.TotalCapacity),
                        UsedTotal: toNumber(overall.UsedNPL) + toNumber(overall.UsedPL),
                        TotalFreeCapacity: toNumber(overall.TotalFreeCapacity),
                        TotalPercent: toNumber(overall.TotalPercent),
                        FreePercent: toNumber(overall.FreePercent),
                        CapacityNPL: toNumber(overall.CapacityNPL),
                        UsedNPL: toNumber(overall.UsedNPL),
                        PercentNPL: toNumber(overall.PercentNPL),
                        CapacityPL: toNumber(overall.CapacityPL),
                        UsedPL: toNumber(overall.UsedPL),
                        PercentPL: toNumber(overall.PercentPL),
                        SoMaVatTu: soMaVatTu2,
                    };
                })(),
            ],
            columns: [
                { key: "TotalCapacity", label: "Tổng sức chứa (CBM)", number: 2 },
                { key: "UsedTotal", label: "CBM đã sử dụng", number: 2 },
                { key: "TotalFreeCapacity", label: "CBM còn trống", number: 2 },
                { key: "TotalPercent", label: "% lấp đầy tổng", percent: true },
                { key: "FreePercent", label: "% còn trống", percent: true },
                { key: "CapacityNPL", label: "Sức chứa NL (CBM)", number: 2 },
                { key: "UsedNPL", label: "NL đã dùng (CBM)", number: 2 },
                { key: "PercentNPL", label: "% NL sử dụng", percent: true },
                { key: "CapacityPL", label: "Sức chứa PL (CBM)", number: 2 },
                { key: "UsedPL", label: "PL đã dùng (CBM)", number: 2 },
                { key: "PercentPL", label: "% PL sử dụng", percent: true },
                { key: "SoMaVatTu", label: "Số mã vật tư", number: 0 },
            ],
        },
        inboundReady: {
            title: "Danh sách lệnh chuẩn bị về",
            rows: state.inbound.map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            }),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "PO", label: "Số PO", center: true, width: 140 },
                { key: "SoLo", label: "Số lô", center: true, width: 120 },
                { key: "MaDH", label: "Mã đơn hàng", center: true, width: 120 },
                { key: "TenKH", label: "Khách hàng", width: 180 },
                { key: "MaHang", label: "Mã hàng", center: true, width: 150 },
                { key: "TenHang", label: "Tên hàng", width: 220 },
                { key: "SoLuongSP", label: "Số lượng SP", number: 0, center: true, width: 100 },
                { key: "NgayNKDuKien", label: "Ngày dự kiến về", date: true, center: true, width: 120 },
            ],
        },
        outboundReady: {
            title: "Danh sách lệnh chuẩn bị xuất",
            rows: state.outboundReady.map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            }),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                // v2.3.31 — Chỉ giữ "Mã lệnh SX" = MaLenh từ CanDoiDonViSanXuat, bỏ cột nội bộ
                { key: "MaLenh", label: "Mã lệnh SX", center: true, width: 120 },
                { key: "TenHang", label: "Mã hàng", width: 250 },
                { key: "SoLuongYeuCau", label: "SL chuẩn bị xuất", number: 2, width: 120 },
                { key: "KHCat", label: "KH cắt", date: true, center: true, width: 100 },
                { key: "DuKienCat", label: "Dự kiến cắt", date: true, center: true, width: 100 },
            ],
        },
        outboundRunning: {
            title: "Danh sách lệnh đang xuất",
            rows: state.outboundRunning.map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            }),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                // v2.3.46 — MaLenh = PhieuXuatHang.MaLenh (integer như 539), header giữ "Mã lệnh SX"
                { key: "MaLenh", label: "Mã lệnh SX", center: true, width: 110 },
                { key: "PhieuDK", label: "Phiếu ĐK", center: true, width: 110 },
                { key: "MaGop", label: "Mã gộp", center: true, width: 110 },
                { key: "TenHang", label: "Tên hàng", width: 220 },
                { key: "TenKH", label: "Khách hàng", width: 150 },
                { key: "NgayXuatHang", label: "Ngày xuất", date: true, center: true, width: 100 },
                { key: "SoLuongYeuCau", label: "SL yêu cầu", number: 2, width: 110 },
                { key: "SLXuat", label: "SL đã xuất", number: 2, width: 110 },
                { key: "PctDaXuat", label: "% đã xuất", percent: true, width: 90 },
            ],
        },
        customersAll: {
            title: "Chi tiết khách hàng theo dung tích sử dụng",
            rows: (function () {
                try {
                    return buildCustomerDetailRows().map(function (r, i) {
                        return Object.assign({ STT: i + 1 }, r);
                    });
                } catch (e) {
                    return [];
                }
            })(),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "MaKH", label: "Mã KH", center: true, width: 110 },
                { key: "TenKH", label: "Tên khách hàng", width: 250 },
                { key: "SLVatTu", label: "Số vật tư", number: 0, center: true, width: 100 },
                { key: "CBMSDTrongKho", label: "CBM sử dụng", number: 2, width: 120 },
                { key: "TyTrongCBM", label: "Tỷ trọng CBM (%)", percent: true, width: 120 },
            ],
        },
        top15MaxNL: (function () {
            var rows = (state.top15MaxNL || []).slice(0, 15).map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            });
            return {
                title: "Chi tiết Top 15 NL tồn kho nhiều nhất",
                rows: rows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "MaVT", label: "Mã vật tư", center: true, width: 140 },
                    { key: "TenVT", label: "Tên vật tư", width: 250 },
                    { key: "Mau", label: "Màu", center: true, width: 80 },
                    { key: "KhoVai", label: "Khổ vải", center: true, width: 80 },
                    { key: "ChiTiet", label: "Chi tiết", width: 180 },
                    { key: "TenDVVT", label: "Đơn vị", center: true, width: 60 },
                    { key: "TonKho", label: "Tồn kho", number: 2, width: 110 },
                ],
            };
        })(),
        top15MaxPL: (function () {
            var rows = (state.top15MaxPL || []).slice(0, 15).map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            });
            return {
                title: "Chi tiết Top 15 PL tồn kho nhiều nhất",
                rows: rows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "MaVT", label: "Mã vật tư", center: true, width: 140 },
                    { key: "TenVT", label: "Tên vật tư", width: 250 },
                    { key: "Mau", label: "Màu", center: true, width: 80 },
                    { key: "KhoVai", label: "Khổ vải", center: true, width: 80 },
                    { key: "ChiTiet", label: "Chi tiết", width: 180 },
                    { key: "TenDVVT", label: "Đơn vị", center: true, width: 60 },
                    { key: "TonKho", label: "Tồn kho", number: 2, width: 110 },
                ],
            };
        })(),
        racksAll: {
            title: "Chi tiết danh sách kệ kho (NL → PL)",
            rows: state.racks
                .slice()
                .sort(function (a, b) {
                    var mA = toNumber(a.Module),
                        mB = toNumber(b.Module);
                    if (mA !== mB) return mA - mB;
                    return (a.TenDay || "").localeCompare(b.TenDay || "");
                })
                .map(function (r, idx) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: idx + 1,
                        Module: r.Module == 1 ? "NL" : r.Module == 2 ? "PL" : String(r.Module || ""),
                        TenDay: r.TenDay || "",
                        TenKe: r.TenKe || "",
                        TongCBMSuDungTrongKe: used,
                        TongCBMTrongKe: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu),
                    };
                }),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "Module", label: "Loại kho", center: true, width: 85 },
                { key: "TenDay", label: "Tên dãy", center: true, width: 85 },
                { key: "TenKe", label: "Tên kệ", center: true, width: 95 },
                { key: "TongCBMSuDungTrongKe", label: "CBM sử dụng", number: 2, width: 110 },
                { key: "TongCBMTrongKe", label: "Tổng CBM", number: 2, width: 110 },
                { key: "PctSuDung", label: "% sử dụng", percent: true, width: 95 },
                { key: "SLVatTu", label: "Số vật tư", number: 0, center: true, width: 85 },
            ],
        },
        inboundAll: {
            title: "Chi tiết lệnh chuẩn bị về",
            rows: state.inbound.map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            }),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "PO", label: "POMUA", center: true, width: 140 },
                { key: "SoLo", label: "Số lô", center: true, width: 120 },
                { key: "MaDH", label: "Mã đơn hàng", center: true, width: 120 },
                { key: "MaHang", label: "Mã hàng", center: true, width: 150 },
                { key: "TenHang", label: "Tên hàng", width: 220 },
                { key: "SoLuongSP", label: "Số lượng SP", number: 0, center: true, width: 100 },
                { key: "NgayNKDuKien", label: "Ngày dự kiến", date: true, center: true, width: 120 },
            ],
        },
        outboundReadyAll: {
            title: "Chi tiết lệnh chuẩn bị xuất",
            rows: state.outboundReady.map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            }),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true },
                { key: "MaLenhSanXuat", label: "Mã lệnh SX", center: true },
                { key: "MaLenh", label: "Mã lệnh", center: true },
                { key: "MaDVSX", label: "Mã ĐVSX", center: true },
                { key: "TenHang", label: "Mã hàng" },
                { key: "KHCat", label: "KH cắt", date: true, center: true },
                { key: "DuKienCat", label: "Dự kiến cắt", date: true, center: true },
            ],
        },
        flowTrendDetail: (function () {
            var dailyData = state.flowTrendRangeRaw || [];
            if (dailyData.length > 0) {
                var rowsD = dailyData.map(function (r, i) {
                    var ngay = String(r.Ngay || "").substring(0, 10);
                    var parts = ngay.split("-");
                    var lbl = parts.length === 3 ? parts[2] + "/" + parts[1] + "/" + parts[0] : ngay;
                    var totalIn = toNumber(r.TotalIn);
                    var totalOut = toNumber(r.TotalOut);
                    var totalStk = toNumber(r.TotalStock);
                    return {
                        STT: i + 1,
                        KyBaoCao: lbl,
                        TotalIn: totalIn,
                        TotalOut: totalOut,
                        NetFlow: totalIn - totalOut,
                        TotalStock: totalStk,
                    };
                });
                return {
                    title: "Chi tiết biểu đồ xuất - nhập - tồn (theo ngày)",
                    rows: rowsD,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "KyBaoCao", label: "Ngày", center: true, width: 110 },
                        { key: "TotalIn", label: "Nhập", number: 0, width: 110 },
                        { key: "TotalOut", label: "Xuất", number: 0, width: 110 },
                        { key: "NetFlow", label: "Nhập - Xuất", number: 0, width: 115 },
                        { key: "TotalStock", label: "Tồn kho", number: 0, width: 130 },
                    ],
                };
            }
            var rows = (state.flowTrend12T || []).map(function (r, i) {
                var lbl = String(r.Thang || "").padStart(2, "0") + "/" + (r.Nam || "");
                var totalIn = toNumber(r.TotalIn);
                var totalOut = toNumber(r.TotalOut);
                var totalStk = toNumber(r.TotalStock);
                return {
                    STT: i + 1,
                    KyBaoCao: lbl,
                    TotalIn: totalIn,
                    TotalOut: totalOut,
                    NetFlow: totalIn - totalOut,
                    TotalStock: totalStk,
                };
            });
            return {
                title: "Chi tiết biểu đồ xuất - nhập - tồn (12 tháng gần nhất)",
                rows: rows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "KyBaoCao", label: "Kỳ báo cáo", center: true, width: 110 },
                    { key: "TotalIn", label: "Nhập", number: 0, width: 110 },
                    { key: "TotalOut", label: "Xuất", number: 0, width: 110 },
                    { key: "NetFlow", label: "Nhập - Xuất", number: 0, width: 115 },
                    { key: "TotalStock", label: "Tồn kho", number: 0, width: 130 },
                ],
            };
        })(),
        outboundRunningAll: {
            title: "Chi tiết lệnh đang xuất",
            rows: state.outboundRunning.map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            }),
            columns: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "MaLenhSX", label: "Mã lệnh SX", center: true, width: 130 },
                { key: "MaLenh", label: "Mã lệnh", center: true, width: 100 },
                { key: "MaGop", label: "Mã gộp", center: true, width: 100 },
                { key: "TenHang", label: "Tên hàng", width: 220 },
                { key: "TenKH", label: "Khách hàng", width: 180 },
                { key: "NgayXuatHang", label: "Ngày xuất", date: true, center: true, width: 100 },
                { key: "SLXuat", label: "SL xuất", number: 2, width: 110 },
            ],
        },
        ageStockDetail: (function () {
            var ageRows = state.ageStock.map(function (r, i) {
                var nplVal = r.NPL;
                var loaiKho = nplVal === true || nplVal === 1 || nplVal === "true" || nplVal === "1" ? "NL" : "PL";
                return Object.assign({ STT: i + 1 }, r, { LoaiKho: loaiKho });
            });
            return {
                title: "Chi tiết tuổi tồn kho theo nhóm vật tư",
                rows: ageRows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "TenNhom", label: "Nhóm vật tư", width: 240 },
                    { key: "Thang", label: "Tháng tuổi", number: 0, center: true, width: 100 },
                    { key: "TonKhoCT", label: "Tồn kho", number: 2, width: 120 },
                    { key: "LoaiKho", label: "Loại kho", center: true, width: 85 },
                ],
            };
        })(),
        turnoverDetail: (function () {
            var trend12 = state.flowTrend12T || [];
            var totalOut12 = 0,
                stockSum = 0,
                stockCount = 0;
            var monthRows = [];
            for (var _ti = 0; _ti < trend12.length; _ti++) {
                var _t = trend12[_ti];
                var _out = toNumber(_t.TotalOut);
                var _in = toNumber(_t.TotalIn);
                var _stk = toNumber(_t.TotalStock);
                totalOut12 += _out;
                if (_stk > 0) {
                    stockSum += _stk;
                    stockCount++;
                }
                var _lbl = String(_t.Thang).padStart(2, "0") + "/" + _t.Nam;
                var _to = _stk > 0 ? _out / _stk : 0;
                var _days = _to > 0 ? Math.round(365 / (_to * 12)) : 0;
                monthRows.push({
                    STT: _ti + 1,
                    KyBaoCao: _lbl,
                    TotalIn: _in,
                    TotalOut: _out,
                    TonKho: _stk,
                    VongQuayThang: _to > 0 ? formatNumber(_to, 2) : "--",
                    NgayTonTB: _days > 0 ? _days : "--",
                });
            }
            var avgStock = stockCount > 0 ? stockSum / stockCount : 0;
            var turnover = avgStock > 0 && totalOut12 > 0 ? totalOut12 / avgStock : 0;
            var avgDays = turnover > 0 ? Math.round(365 / turnover) : 0;
            return {
                title: "Chi tiết vòng quay hàng tồn kho (12 tháng)",
                meta:
                    "Vòng quay tổng: " +
                    (turnover > 0 ? formatNumber(turnover, 2) + " lần/năm" : "--") +
                    "  |  Ngày tồn kho TB: " +
                    (avgDays > 0 ? avgDays + " ngày" : "--"),
                rows: monthRows,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "KyBaoCao", label: "Kỳ báo cáo", center: true, width: 110 },
                    { key: "TotalIn", label: "Nhập", number: 0, width: 110 },
                    { key: "TotalOut", label: "Xuất", number: 0, width: 110 },
                    { key: "TonKho", label: "Tồn kho", number: 0, width: 120 },
                    { key: "VongQuayThang", label: "Vòng quay (lần/tháng)", center: true, width: 150 },
                    { key: "NgayTonTB", label: "Ngày tồn TB", center: true, width: 110 },
                ],
            };
        })(),
        // v2.3.36 — Thành giá hàng tồn kho
        thanhGiaDetail: (function () {
            var tg = state.thanhGia || {};
            var thanhGia = toNumber(tg.ThanhGia);
            var tongMa = toNumber(tg.TongMaVT);
            var soMaCoGia = toNumber(tg.SoMaCoGia);
            var soMaKhongGia = toNumber(tg.SoMaKhongGia);
            var tongSL = toNumber(tg.TongSoLuong);
            var avgGia = tongSL > 0 ? thanhGia / tongSL : 0;
            var pctCoGia = tongMa > 0 ? (soMaCoGia / tongMa) * 100 : 0;

            var rows = [
                { ChiSo: "Tổng thành giá hàng tồn (VND)", GiaTri: formatNumber(thanhGia, 0) },
                { ChiSo: "Tổng số mã vật tư trong kho", GiaTri: formatNumber(tongMa, 0) },
                {
                    ChiSo: "Số mã có giá",
                    GiaTri: formatNumber(soMaCoGia, 0) + "  (" + formatNumber(pctCoGia, 1) + "%)",
                },
                { ChiSo: "Số mã chưa có giá", GiaTri: formatNumber(soMaKhongGia, 0) },
                { ChiSo: "Tổng số lượng tồn", GiaTri: formatNumber(tongSL, 2) },
                { ChiSo: "Giá trị TB / 1 đơn vị (VND)", GiaTri: formatNumber(avgGia, 0) },
            ];
            return {
                title: "Chi tiết thành giá hàng tồn kho",
                meta: "Tổng giá trị: " + formatNumber(thanhGia, 0) + " VND",
                rows: rows.map(function (r, i) {
                    return Object.assign({ STT: i + 1 }, r);
                }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "ChiSo", label: "Chỉ số", width: 250 },
                    { key: "GiaTri", label: "Giá trị", center: true, width: 180 },
                ],
            };
        })(),
    };

    if (detail === "customerRow") {
        // v2.3.25 — Customer drill: hiện hàng của khách ở đâu (dãy/kệ/ô)
        var customerItem = buildCustomerDetailRows().filter(function (row) {
            return row.sourceIndex === index;
        })[0];
        return {
            title: "Vị trí lưu kho của khách hàng: " + (customerItem ? customerItem.TenKH || customerItem.MaKH : ""),
            rows: customerItem ? [customerItem] : [],
            columns: detailMap.customersAll.columns,
            customDrillCustomer: customerItem, // flag để openDetail tự fetch slot detail
        };
    }

    if (detail === "rackRow") {
        // v2.3.26 — Rack drill: hiện những vật tư trong kệ đó
        var rackItem = state.racks[index];
        return {
            title: "Chi tiết kệ: " + (rackItem ? rackItem.TenKe || "" : ""),
            rows: rackItem ? [rackItem] : [],
            columns: detailMap.racksAll.columns,
            customDrillRack: rackItem,
        };
    }

    if (detail === "inboundRow") {
        var inboundItem = state.inbound[index];
        return {
            title: "Chi tiết lệnh chuẩn bị về",
            rows: inboundItem ? [inboundItem] : [],
            columns: detailMap.inboundAll.columns,
        };
    }

    if (detail === "outboundReadyRow") {
        var readyItem = state.outboundReady[index];
        return {
            title: "Chi tiết lệnh chuẩn bị xuất",
            rows: readyItem ? [readyItem] : [],
            columns: detailMap.outboundReadyAll.columns,
        };
    }

    if (detail === "outboundRunningRow") {
        var runningItem = state.outboundRunning[index];
        return {
            title: "Chi tiết lệnh đang xuất",
            rows: runningItem ? [runningItem] : [],
            columns: detailMap.outboundRunningAll.columns,
        };
    }

    return (
        detailMap[detail] || {
            title: "Chi tiết",
            rows: [],
            columns: [],
        }
    );
}

function renderDetailTable(columns, rows) {
    if (!columns || columns.length === 0) {
        return '<p class="dk-empty">Không có cấu hình cột hiển thị.</p>';
    }

    if (!rows || rows.length === 0) {
        return '<p class="dk-empty">Không có dữ liệu chi tiết.</p>';
    }

    // v2.3.27 — Cột text dài cần wrap (left align). MaNPL/DanhSachKH/DanhSachMaNPL có thể rất dài
    var TEXT_LEFT_KEYS = {
        TenKH: 1,
        KhachHang: 1,
        TenKe: 1,
        TenDay: 1,
        TenHang: 1,
        MaVT: 1,
        ChiTiet: 1,
        MaGop: 1,
        MaDH: 1,
        MaHang: 1,
        MaLenh: 1,
        MaDVSX: 1,
        MaNPL: 1,
        DanhSachKH: 1,
        DanhSachMaNPL: 1,
        ChiSo: 1,
        ItemCode: 1,
        TenVT: 1,
        MaPhieu: 1,
    };
    // v2.4.0 — Cột code dùng monospace + tabular alignment
    var MONO_KEYS = { ItemCode: 1, MaVT: 1, MaNPL: 1, MaGop: 1, MaLenh: 1, MaPhieu: 1 };

    var headHtml = columns
        .map(function (column) {
            var styles = [];
            if (column.center) {
                styles.push("text-align:center");
            } else if (column.number !== undefined || column.percent) {
                styles.push("text-align:right");
            } else if (TEXT_LEFT_KEYS[column.key]) {
                styles.push("text-align:left");
            }
            // v2.3.39 - Width hint từng cột (compact tables)
            if (column.width) {
                var w = column.width + (typeof column.width === "number" ? "px" : "");
                styles.push("width:" + w + ";min-width:" + w);
            } else {
                styles.push("min-width:150px");
            }
            var thStyle = styles.length ? ' style="' + styles.join(";") + '"' : "";
            return "<th" + thStyle + ">" + escapeHtml(column.label) + "</th>";
        })
        .join("");

    var bodyHtml = rows
        .map(function (row, rowIdx) {
            var cols = columns
                .map(function (column) {
                    var value = row[column.key];
                    var cellStyle = "";
                    // v2.3.21 — raw:true → render HTML thẳng (cho link drill)
                    if (column.raw) {
                        var rawAlign = column.center ? "center" : "left";
                        return '<td style="text-align:' + rawAlign + '">' + (value || "") + "</td>";
                    }
                    if (column.number !== undefined) {
                        value = formatNumber(value, column.number);
                        cellStyle = column.center ? ' style="text-align:center"' : ' style="text-align:right"';
                    } else if (column.percent) {
                        value = formatPercent(value);
                        cellStyle = column.center ? ' style="text-align:center"' : ' style="text-align:right"';
                    } else if (column.date) {
                        value = formatDate(value);
                        if (column.center) cellStyle = ' style="text-align:center"';
                    } else if (column.center) {
                        var cellMonoStyle = MONO_KEYS[column.key]
                            ? ";font-family:Consolas,'Courier New',ui-monospace,monospace;font-weight:600"
                            : "";
                        cellStyle = ' style="text-align:center' + cellMonoStyle + '"';
                    } else if (
                        column.key === "TenKH" ||
                        column.key === "KhachHang" ||
                        column.key === "TenKe" ||
                        column.key === "TenDay" ||
                        TEXT_LEFT_KEYS[column.key]
                    ) {
                        if (column.key === "TenKH" || column.key === "KhachHang") value = normalizeCustomerName(value);
                        var leftStyle = "text-align:left";
                        if (MONO_KEYS[column.key]) {
                            leftStyle += ";font-family:Consolas,'Courier New',ui-monospace,monospace;font-weight:600";
                        }
                        cellStyle = ' style="' + leftStyle + '"';
                    } else if (column.key === "MaLenhSanXuat" || column.key === "MaLenhSX") {
                        value = shortMaLenh(value);
                        var mlAlign = column.center ? "center" : "left";
                        cellStyle = ' style="font-weight:700;text-align:' + mlAlign + '"';
                    } else if (column.key === "LoaiKho" || column.key === "Module") {
                        var loaiColor = value === "NL" ? "#2563eb" : value === "PL" ? "#d97706" : "#6b7280";
                        var loaiAlign = column.center ? "center" : "left";
                        return (
                            '<td style="text-align:' +
                            loaiAlign +
                            '"><span style="color:' +
                            loaiColor +
                            ';font-weight:800">' +
                            escapeHtml(value || "") +
                            "</span></td>"
                        );
                    }
                    return "<td" + cellStyle + ">" + escapeHtml(value) + "</td>";
                })
                .join("");

            return "<tr>" + cols + "</tr>";
        })
        .join("");

    return (
        '<div class="dk-detail-table-wrap"><table class="dk-detail-table"><thead><tr>' +
        headHtml +
        "</tr></thead><tbody>" +
        bodyHtml +
        "</tbody></table></div>"
    );
}

// v2.3.27 — Modal navigation stack: drill có nút "← Quay lại"
var _detailStack = [];

//#endregion


// ════════════════════════════════════════════════════════════════
// v2.4.6 — Page 2 widget mới: Cảnh báo tồn kho + Hiệu suất hoạt động
// ════════════════════════════════════════════════════════════════

function renderAlertsList() {
    var node = byId("alertsList");
    if (!node) return;
    var rows = state.alerts || [];
    if (rows.length === 0) {
        node.innerHTML = '<div class="dk-empty">Không có cảnh báo</div>';
        return;
    }
    var iconMap = {
        danger: "fa-triangle-exclamation",
        warn: "fa-circle-exclamation",
        info: "fa-circle-info",
    };
    node.innerHTML = rows
        .map(function (a) {
            var mucDo = (a.MucDo || "info").toLowerCase();
            var icon = iconMap[mucDo] || iconMap.info;
            // v2.4.16 — click 1 alert-item → mở modal alertDetail với code tương ứng
            return (
                "" +
                '<div class="dk-alert-item dk-alert-' +
                mucDo +
                ' js-open-detail" data-detail="alertDetail" data-alert-code="' +
                escapeHtml(a.MaCB || "") +
                '" data-alert-name="' +
                escapeHtml(a.TenCB || "") +
                '" title="Click để xem chi tiết" role="button" tabindex="0">' +
                '<span class="dk-alert-icon"><i class="fa-solid ' +
                icon +
                '"></i></span>' +
                '<div class="dk-alert-info">' +
                '<div class="dk-alert-title">' +
                escapeHtml(a.TenCB || "") +
                "</div>" +
                '<div class="dk-alert-desc">' +
                escapeHtml(a.MoTa || "") +
                "</div>" +
                "</div>" +
                '<div class="dk-alert-badge">' +
                '<span class="dk-alert-num">' +
                formatNumber(toNumber(a.SoLuong), 2) +
                "</span> " +
                '<span class="dk-alert-unit">' +
                escapeHtml(a.DonVi || "") +
                "</span>" +
                "</div>" +
                "</div>"
            );
        })
        .join("");
}

function renderHieuSuatGauges() {
    if (typeof Highcharts === "undefined") return;
    var rows = state.hieuSuat || [];
    if (rows.length === 0) return;
    applyHighchartsTheme();
    var dark = dkIsDark();
    // v2.4.7 — Mapping accent + icon FA cho từng chỉ số
    var meta = {
        hoan_thanh_nhap: {
            accent: "#3b82f6",
            accent2: "#60a5fa",
            icon: "fa-cloud-arrow-down",
            short: "Hoàn thành nhập",
        },
        hoan_thanh_xuat: { accent: "#f97316", accent2: "#fb923c", icon: "fa-truck-fast", short: "Hoàn thành xuất" },
        kiem_ke_dung_han: {
            accent: "#22c55e",
            accent2: "#4ade80",
            icon: "fa-clipboard-check",
            short: "Kiểm kê đúng hạn",
        },
        don_hang_dung_han: {
            accent: "#06b6d4",
            accent2: "#22d3ee",
            icon: "fa-circle-check",
            short: "Đơn hàng đúng hạn",
        },
    };
    rows.forEach(function (r) {
        var cell = document.querySelector('.dk-perf-cell[data-perf="' + r.MaChiSo + '"]');
        if (!cell) return;
        var pct = toNumber(r.Value);
        var delta = toNumber(r.Delta);
        var m = meta[r.MaChiSo] || {
            accent: "#3b82f6",
            accent2: "#60a5fa",
            icon: "fa-circle-check",
            short: r.TenChiSo,
        };
        cell.className = "dk-perf-cell dk-perf-accent-" + r.MaChiSo;
        cell.style.setProperty("--perf-accent", m.accent);
        cell.style.setProperty("--perf-accent2", m.accent2);
        cell.innerHTML =
            "" +
            '<div class="dk-perf-head">' +
            '<span class="dk-perf-head-icon"><i class="fa-solid ' +
            m.icon +
            '"></i></span>' +
            '<span class="dk-perf-head-label">' +
            escapeHtml(m.short) +
            "</span>" +
            "</div>" +
            '<div class="dk-perf-gauge"></div>' +
            '<div class="dk-perf-delta-row">' +
            formatDelta(delta) +
            "</div>";
        var gaugeEl = cell.querySelector(".dk-perf-gauge");
        var trackCol = dark ? "rgba(255,255,255,0.06)" : "rgba(0,0,0,0.05)";
        // Gradient stroke: light → accent đậm
        var gradId = "perfGrad_" + r.MaChiSo;
        Highcharts.chart(gaugeEl, {
            chart: { type: "solidgauge", backgroundColor: "transparent", spacing: [4, 4, 4, 4], margin: [0, 0, 0, 0] },
            title: { text: null },
            credits: { enabled: false },
            tooltip: { enabled: false },
            pane: {
                center: ["50%", "55%"],
                size: "100%",
                startAngle: -135,
                endAngle: 135,
                background: [
                    {
                        outerRadius: "100%",
                        innerRadius: "72%",
                        backgroundColor: trackCol,
                        borderWidth: 0,
                        shape: "arc",
                    },
                ],
            },
            yAxis: { min: 0, max: 100, lineWidth: 0, tickPositions: [] },
            plotOptions: {
                solidgauge: {
                    dataLabels: {
                        enabled: true,
                        y: -14,
                        borderWidth: 0,
                        useHTML: true,
                        format:
                            '<div class="dk-perf-center"><div class="dk-perf-center-pct" style="color:' +
                            m.accent +
                            '">{y}%</div><div class="dk-perf-center-sub">Đạt</div></div>',
                    },
                    rounded: true,
                    linecap: "round",
                },
            },
            series: [
                {
                    name: "pct",
                    data: [
                        {
                            y: pct,
                            color: {
                                linearGradient: { x1: 0, x2: 1, y1: 0, y2: 1 },
                                stops: [
                                    [0, m.accent2],
                                    [1, m.accent],
                                ],
                            },
                            radius: "100%",
                            innerRadius: "72%",
                        },
                    ],
                },
            ],
        });
    });
}

// ════════════════════════════════════════════════════════════════
// v2.4.0 — Render 6 section mới ở Page 1 (Tổng quan)
// ════════════════════════════════════════════════════════════════
function renderTodoList() {
    var node = byId("todoList");
    if (!node) return;
    var items = state.todoList || [];
    if (items.length === 0) {
        node.innerHTML = '<div class="dk-empty">Không có công việc</div>';
        return;
    }
    var iconMap = {
        "clipboard-check": "fa-clipboard-check",
        "clipboard-list": "fa-clipboard-list",
        "file-signature": "fa-file-signature",
        truck: "fa-truck",
    };
    var colorMap = [
        { bg: "rgba(59,130,246,0.18)", color: "#60a5fa" },
        { bg: "rgba(168,85,247,0.18)", color: "#a78bfa" },
        { bg: "rgba(34,197,94,0.18)", color: "#34d399" },
        { bg: "rgba(245,158,11,0.18)", color: "#fbbf24" },
    ];
    node.innerHTML = items
        .map(function (it, idx) {
            var iconCls = iconMap[it.Icon] || "fa-circle-exclamation";
            var col = colorMap[idx % colorMap.length];
            // v2.4.7 — Click 1 todo-item → mở modal todoDetail với tab type tương ứng
            return (
                "" +
                '<div class="dk-todo-item js-open-detail" data-detail="todoDetail" data-todo-type="' +
                escapeHtml(it.MaCV || "") +
                '" role="button" tabindex="0">' +
                '<span class="dk-todo-icon" style="background:' +
                col.bg +
                ";color:" +
                col.color +
                '">' +
                '<i class="fa-solid ' +
                iconCls +
                '"></i>' +
                "</span>" +
                '<div class="dk-todo-info">' +
                '<div class="dk-todo-title">' +
                escapeHtml(it.TenCV || "") +
                "</div>" +
                '<div class="dk-todo-desc">' +
                escapeHtml(it.MoTa || "") +
                "</div>" +
                "</div>" +
                '<div class="dk-todo-count">' +
                '<div class="dk-todo-num">' +
                formatNumber(toNumber(it.SoLuong), 2).padStart(2, "0") +
                "</div>" +
                '<div class="dk-todo-unit">' +
                escapeHtml(it.DonVi || "") +
                "</div>" +
                "</div>" +
                "</div>"
            );
        })
        .join("");
}

function renderKiemKeBox() {
    var node = byId("kiemKeBox");
    if (!node) return;
    var d = state.kiemKe;
    if (!d) {
        node.innerHTML = '<div class="dk-empty">Chưa có dữ liệu</div>';
        return;
    }
    var pct = toNumber(d.PctDaKiem);
    node.innerHTML =
        "" +
        '<div class="dk-kk-dashboard-layout">' +
        '<div class="dk-kk-chart-side">' +
        '<div class="dk-kk-circular-wrap">' +
        '<svg viewBox="0 0 36 36" class="dk-kk-circular-chart">' +
        "<defs>" +
        '<linearGradient id="kkGrad" x1="0%" y1="0%" x2="100%" y2="100%">' +
        '<stop offset="0%" stop-color="#10b981" />' +
        '<stop offset="100%" stop-color="#34d399" />' +
        "</linearGradient>" +
        "</defs>" +
        '<path class="dk-kk-circle-bg" fill="none" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />' +
        '<path class="dk-kk-circle" fill="none" stroke-dasharray="' +
        pct +
        ', 100" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />' +
        "</svg>" +
        '<div class="dk-kk-circular-val">' +
        '<span class="pct-num">' +
        formatNumber(pct, 1) +
        "%</span>" +
        '<span class="pct-lbl">Đã kiểm</span>' +
        "</div>" +
        "</div>" +
        "</div>" +
        '<div class="dk-kk-info-side">' +
        '<div class="dk-kk-info-card dk-kk-info-good js-open-detail" data-detail="kiemKeAll" role="button" tabindex="0">' +
        '<span class="dk-kk-info-icon"><i class="fa-solid fa-circle-check"></i></span>' +
        '<div class="dk-kk-info-meta">' +
        '<span class="dk-kk-info-lbl">Đã kiểm</span>' +
        '<span class="dk-kk-info-val dk-count-up" data-count-to="' +
        toNumber(d.DaKiem) +
        '">0</span>' +
        "</div>" +
        "</div>" +
        '<div class="dk-kk-info-card dk-kk-info-bad js-open-detail" data-detail="kiemKeAll" role="button" tabindex="0">' +
        '<span class="dk-kk-info-icon"><i class="fa-solid fa-circle-exclamation"></i></span>' +
        '<div class="dk-kk-info-meta">' +
        '<span class="dk-kk-info-lbl">Chưa kiểm</span>' +
        '<span class="dk-kk-info-val dk-count-up" data-count-to="' +
        toNumber(d.ChuaKiem) +
        '">0</span>' +
        "</div>" +
        "</div>" +
        '<div class="dk-kk-info-total">' +
        "<span>Tổng số itemcode:</span>" +
        "<strong>" +
        formatNumber(toNumber(d.Tong), 2) +
        "</strong>" +
        "</div>" +
        "</div>" +
        "</div>";
    // Trigger counter-up
    animateCountUp(node);
}

function renderSummaryStrip(items) {
    var html = '<div class="dk-modal-summary-strip">';
    for (var i = 0; i < items.length; i++) {
        var it = items[i];
        var kindCls = it.kind ? " dk-sum-" + it.kind : "";
        html +=
            '<div class="dk-sum-item' +
            kindCls +
            '">' +
            '<div class="dk-sum-label">' +
            escapeHtml(it.label) +
            "</div>" +
            '<div class="dk-sum-value">' +
            (it.value || "") +
            "</div>" +
            (it.sub ? '<div class="dk-sum-sub">' + escapeHtml(it.sub) + "</div>" : "") +
            "</div>";
    }
    return html + "</div>";
}

// ════════════════════════════════════════════════════════════════
// v2.4.15 — 5 modal chi tiết KPI header
// ════════════════════════════════════════════════════════════════

// Helper chung: render summary strip
function renderKpiSummaryStrip(items, extraClass) {
    // v2.7.1 — Auto-fit column class to prevent wrapping
    var colClass =
        items.length >= 4 ? " dk-modal-summary-strip--4col" : items.length === 3 ? " dk-modal-summary-strip--3col" : "";
    var cls = "dk-modal-summary-strip" + colClass + (extraClass ? " " + extraClass : "");
    var html = '<div class="' + cls + '">';
    for (var i = 0; i < items.length; i++) {
        var v = items[i];
        html +=
            '<div class="dk-sum-item dk-sum-' +
            (v.cls || "neutral") +
            '">' +
            '<div class="dk-sum-label">' +
            escapeHtml(v.label) +
            "</div>" +
            '<div class="dk-sum-value">' +
            v.value +
            "</div>" +
            '<div class="dk-sum-sub">' +
            escapeHtml(v.sub || "") +
            "</div>" +
            "</div>";
    }
    return html + "</div>";
}

// Helper chung: render sub-tab pills
function renderKpiSubtabs(tabs, activeKey) {
    var html = '<div class="dk-kk-subtabs">';
    for (var i = 0; i < tabs.length; i++) {
        var t = tabs[i];
        var active = t.key === activeKey ? " active" : "";
        html +=
            '<button type="button" class="dk-kk-subtab' +
            active +
            '" data-tab-key="' +
            escapeHtml(t.key) +
            '"><i class="fa-solid ' +
            t.icon +
            '"></i> ' +
            escapeHtml(t.label) +
            "</button>";
    }
    return html + "</div>";
}

// Helper chung: render filter bar
function renderKpiFilterBar(placeholder, opts) {
    opts = opts || {};
    var html =
        '<div class="dk-modal-filter-bar">' +
        '<i class="fa-solid fa-magnifying-glass"></i>' +
        '<input type="text" id="kpiSearch" class="dk-kk-search" placeholder="' +
        escapeHtml(placeholder) +
        '" />' +
        '<span id="kpiCount" class="dk-text-muted" style="display: none;"></span>' +
        "</div>";
    return html;
}

// Helper chung: bind search live
function bindKpiSearch(scope, refilterFn) {
    var input = byId("kpiSearch");
    if (!input) return;
    setTimeout(function () {
        input.focus();
    }, 120);
    input.addEventListener("input", function () {
        refilterFn();
    });
}

function renderMetricCards() {
    // v2.4.6 — 7 KPI mới: Tồn đầu kỳ + Tổng nhập + Tổng xuất + Tồn kho + PO chuẩn bị về + PO đang trễ + Giá trị tồn kho
    function setText(id, val) {
        var el = byId(id);
        if (el) el.textContent = val;
    }
    function setHtml(id, val) {
        var el = byId(id);
        if (el) el.innerHTML = val;
    }

    // 1) Tồn đầu kỳ
    if (state.kpiTonDauKy) setText("metricTonDauKy", formatNumber(toNumber(state.kpiTonDauKy.Value), 0));
    else setText("metricTonDauKy", "0");

    // 2) Tổng nhập
    if (state.kpiTongNhap) {
        setText("metricTongNhap", formatNumber(toNumber(state.kpiTongNhap.Value), 0));
        setHtml("metricTongNhapDelta", formatDelta(state.kpiTongNhap.Delta));
    } else {
        setText("metricTongNhap", "0");
        setHtml("metricTongNhapDelta", "");
    }

    // 3) Tổng xuất
    if (state.kpiTongXuat) {
        setText("metricTongXuat", formatNumber(toNumber(state.kpiTongXuat.Value), 0));
        setHtml("metricTongXuatDelta", formatDelta(state.kpiTongXuat.Delta));
    } else {
        setText("metricTongXuat", "0");
        setHtml("metricTongXuatDelta", "");
    }

    // 4) Tồn kho
    if (state.kpiTonKho) {
        setText("metricTonKho", formatNumber(toNumber(state.kpiTonKho.Value), 0));
        setHtml("metricTonKhoDelta", formatDelta(state.kpiTonKho.Delta));
    } else {
        setText("metricTonKho", "0");
        setHtml("metricTonKhoDelta", "");
    }
    // 5) PO chuẩn bị về
    setText("metricInboundReady", formatNumber(state.inbound.length, 0));
    setHtml(
        "metricInboundReadyHint",
        state.inbound.length > 0 ? '<i class="fa-solid fa-fire dk-text-warn"></i> ' + state.inbound.length + " PO" : "",
    );
    // 6) PO đang trễ
    if (state.kpiPODangTre) {
        setText("metricPODangTre", formatNumber(toNumber(state.kpiPODangTre.SoPO), 0));
        setHtml(
            "metricPODangTreHint",
            '<i class="fa-solid fa-fire dk-text-danger"></i> ' +
            toNumber(state.kpiPODangTre.SoPOChuaKiem) +
            " PO chưa kiểm",
        );
    } else {
        setText("metricPODangTre", "0");
        setHtml("metricPODangTreHint", "");
    }

    // 7) Giá trị tồn kho
    var thanhGiaEl = byId("metricThanhGia");
    if (thanhGiaEl) {
        var tg = state.kpiGiaTriTon
            ? toNumber(state.kpiGiaTriTon.Value)
            : state.thanhGia
                ? toNumber(state.thanhGia.ThanhGia || state.thanhGia.TongTien)
                : 0;
        if (tg > 0) {
            var formatted;
            if (tg >= 1e9) formatted = (tg / 1e9).toFixed(3).replace(/\.?0+$/, "") + " tỷ";
            else if (tg >= 1e6) formatted = (tg / 1e6).toFixed(1).replace(/\.0$/, "") + " tr";
            else formatted = formatNumber(tg, 0);
            thanhGiaEl.textContent = formatted;
        } else {
            thanhGiaEl.textContent = "0";
        }
    }
    if (state.kpiGiaTriTon) {
        setHtml("metricGiaTriTonDelta", formatDelta(state.kpiGiaTriTon.Delta));
    } else {
        setHtml("metricGiaTriTonDelta", "");
    }
}

function renderCustomersTable() {
    var body = byId(ids.customersBody);
    if (!body) return;
    if (state.customers.length === 0) {
        body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có dữ liệu khách hàng</td></tr>';
        return;
    }

    var data = state.customers
        .map(function (item, index) {
            return { sourceIndex: index, item: item };
        })
        .sort(function (a, b) {
            return toNumber(b.item.CBMSDTrongKho) - toNumber(a.item.CBMSDTrongKho);
        })
        .slice(0, 12);

    var html = data
        .map(function (entry, stt) {
            var item = entry.item;
            var isFiltered = activeCustomerFilter && item.MaKH !== activeCustomerFilter;
            var rowStyle = isFiltered ? ' style="opacity:0.35"' : "";
            return (
                "<tr " +
                rowDataAttr("customerRow", entry.sourceIndex) +
                rowStyle +
                ">" +
                "<td><b>" +
                (stt + 1) +
                "</b></td>" +
                "<td>" +
                escapeHtml(item.MaKH || "") +
                "</td>" +
                '<td style="text-align:left">' +
                escapeHtml(normalizeCustomerName(item.TenKH)) +
                "</td>" +
                '<td class="text-end">' +
                formatNumber(item.SLVatTu, 2) +
                "</td>" +
                '<td class="text-end">' +
                formatNumber(item.CBMSDTrongKho, 2) +
                "</td>" +
                "</tr>"
            );
        })
        .join("");

    body.innerHTML = html;
}