(function () {


    let rows = [];
    let gridInstance = null;
    let detailPopupInstance = null;
    let loadPanelInstance = null;
    const detailCache = Object.create(null);
    let postInProgress = false;
    let fromDateValue = null;
    let toDateValue = null;

    function notify(message, type) {
        if (globalThis.DevExpress && DevExpress.ui && typeof DevExpress.ui.notify === "function") {
            DevExpress.ui.notify({
                message: message,
                type: type || "info",
                displayTime: 1600,
                width: "auto",
                minWidth: 180,
                maxWidth: 320,
                position: {
                    my: "right top",
                    at: "right top",
                    of: globalThis,
                    offset: "-16 16"
                }
            });
            return;
        }
        alert(message);
    }

    function isApprovedMode() {
        return $("#nk-chk-da-duyet").is(":checked");
    }

    function normalizeRow(item, index) {
        const isDuyet = Number(item.IsDuyet) === 1 || item.IsDuyet === true;
        const baseKey = item.PheDuyetID || [item.SoLoID, item.MaNPL, item.IsNPL].join("|");
        const clientKey = baseKey + "|" + String(index || 0);
        return {
            ClientKey: clientKey,
            PheDuyetID: item.PheDuyetID,
            SoLoID: item.SoLoID,
            MaNPL: item.MaNPL,
            IsNPL: item.IsNPL,
            POMua: item.POMua || "",
            ChungLoaiVatTu: item.ChungLoaiVatTu || "",
            MaVT: item.MaVT || "",
            ChiTiet: item.ChiTiet || "",
            MauVT: item.MauVT || "",
            KhoVai: item.KhoVai || "",
            TenDVVT: item.TenDVVT || "",
            SLNhap: Number(item.SLNhap || 0),
            SLKiem: Number(item.SLKiem || 0),
            TrangThaiQC: item.TrangThaiQC || "",
            IsDuyet: isDuyet,
            sDuyet: isDuyet ? 1 : 0,
            ActionChecked: false
        };
    }

    function normalizeDateOnly(value) {
        if (!(value instanceof Date) || Number.isNaN(value.getTime())) {
            return null;
        }
        return new Date(value.getFullYear(), value.getMonth(), value.getDate());
    }

    function getDefaultFromDate() {
        const now = new Date();
        return new Date(now.getFullYear(), now.getMonth(), 1);
    }

    function getDefaultToDate() {
        const now = new Date();
        return new Date(now.getFullYear(), now.getMonth(), now.getDate());
    }

    function formatDateForApi(value) {
        const date = normalizeDateOnly(value);
        if (!date) {
            return "";
        }
        const yyyy = date.getFullYear();
        const mm = String(date.getMonth() + 1).padStart(2, "0");
        const dd = String(date.getDate()).padStart(2, "0");
        return yyyy + "-" + mm + "-" + dd;
    }

    function ensureLoadPanel() {
        if (loadPanelInstance) {
            return loadPanelInstance;
        }

        let panelEl = $("#qc-loading-panel");
        if (!panelEl.length) {
            panelEl = $('<div id="qc-loading-panel"></div>').appendTo("body");
        }

        panelEl.dxLoadPanel({
            shadingColor: "rgba(0, 0, 0, 0.18)",
            visible: false,
            showIndicator: true,
            showPane: true,
            shading: true,
            hideOnOutsideClick: false,
            message: "Đang lọc dữ liệu..."
        });

        loadPanelInstance = panelEl.dxLoadPanel("instance");
        return loadPanelInstance;
    }

    function showLoading(message) {
        const panel = ensureLoadPanel();
        panel.option("message", message || "Đang lọc dữ liệu...");
        panel.show();
    }

    function hideLoading() {
        if (loadPanelInstance) {
            loadPanelInstance.hide();
        }
    }

    function setToolbarBusy(isBusy) {
        $("#nk-btn-duyet, #nk-btn-huy-duyet, #nk-btn-export-excel, #nk-btn-filter").prop("disabled", !!isBusy);
    }

    function hasQuantityMismatch(item) {
        const slNhap = Number(item?.SLNhap || 0);
        const slKiem = Number(item?.SLKiem || 0);
        return Math.abs(slNhap - slKiem) > 0.00001;
    }

    function resolveQcStatusCode(value, item) {
        const raw = String(value == null ? "" : value).trim().toUpperCase();
        if (!raw) {
            return hasQuantityMismatch(item) ? "FAIL" : "PASS";
        }

        if (raw.indexOf("FAIL") >= 0 || raw === "0" || raw === "FALSE" || raw === "N") {
            return "FAIL";
        }

        if (raw.indexOf("PASS") >= 0 || raw === "1" || raw === "TRUE" || raw === "Y") {
            return "PASS";
        }

        return hasQuantityMismatch(item) ? "FAIL" : "PASS";
    }

    function toBit(value) {
        if (value === true) {
            return 1;
        }
        if (value === false || value == null) {
            return 0;
        }

        const text = String(value).trim().toLowerCase();
        if (text === "1" || text === "true" || text === "yes") {
            return 1;
        }
        if (text === "0" || text === "false" || text === "no") {
            return 0;
        }

        const numeric = Number(value);
        if (!Number.isNaN(numeric)) {
            return numeric === 0 ? 0 : 1;
        }
        return 0;
    }

    function getVisibleRows() {
        const approvedMode = isApprovedMode();
        return rows.filter(function (item) {
            return item.IsDuyet === approvedMode;
        });
    }

    function updateCheckAllCheckbox() {
        const chk = $(".qc-check-all-grid");
        if (!chk.length) {
            return;
        }

        const visibleRows = getVisibleRows();
        const total = visibleRows.length;
        let checkedCount = 0;

        for (const row of visibleRows) {
            if (row.ActionChecked) {
                checkedCount += 1;
            }
        }

        const allChecked = total > 0 && checkedCount === total;
        const partiallyChecked = checkedCount > 0 && checkedCount < total;

        chk.prop("disabled", total === 0);
        chk.prop("indeterminate", partiallyChecked);
        chk.prop("checked", allChecked);
    }

    function setCheckAllVisible(shouldCheck) {
        const visibleRows = getVisibleRows();
        if (!visibleRows.length) {
            updateCheckAllCheckbox();
            return;
        }

        for (let i = 0; i < visibleRows.length; i++) {
            visibleRows[i].ActionChecked = shouldCheck;
        }

        $(".qc-row-check").prop("checked", shouldCheck);
        updateCheckAllCheckbox();
    }

    function initDateFilters() {
        fromDateValue = getDefaultFromDate();
        toDateValue = getDefaultToDate();

        $("#nk-from-date").dxDateBox({
            type: "date",
            displayFormat: "dd/MM/yyyy",
            value: fromDateValue,
            width: 190,
            useMaskBehavior: true,
            onValueChanged: function (e) {
                fromDateValue = normalizeDateOnly(e.value);
                if (fromDateValue && toDateValue && fromDateValue > toDateValue) {
                    toDateValue = fromDateValue;
                    const toInstance = $("#nk-to-date").dxDateBox("instance");
                    if (toInstance) {
                        toInstance.option("value", toDateValue);
                    }
                }
            }
        });

        $("#nk-to-date").dxDateBox({
            type: "date",
            displayFormat: "dd/MM/yyyy",
            value: toDateValue,
            width: 190,
            useMaskBehavior: true,
            onValueChanged: function (e) {
                toDateValue = normalizeDateOnly(e.value);
                if (toDateValue && fromDateValue && toDateValue < fromDateValue) {
                    fromDateValue = toDateValue;
                    const fromInstance = $("#nk-from-date").dxDateBox("instance");
                    if (fromInstance) {
                        fromInstance.option("value", fromDateValue);
                    }
                }
            }
        });
    }

    function getApprovalCounts() {
        let approved = 0;
        let unapproved = 0;
        for (let i = 0; i < rows.length; i++) {
            if (rows[i].IsDuyet) {
                approved += 1;
            } else {
                unapproved += 1;
            }
        }
        return { approved: approved, unapproved: unapproved };
    }

    function autoSwitchToApprovedWhenNeeded() {
        const counts = getApprovalCounts();
        if (!isApprovedMode() && counts.unapproved === 0 && counts.approved > 0) {
            $("#nk-chk-da-duyet").prop("checked", true);
            notify("Hiện không còn dòng chưa duyệt, đã chuyển sang chế độ Đã duyệt", "info");
        }
    }

    function getDisplaySttByClientKey(clientKey) {
        if (!gridInstance || !clientKey) {
            return "";
        }

        const visible = gridInstance.getVisibleRows() || [];
        let stt = 0;
        for (let i = 0; i < visible.length; i++) {
            const row = visible[i];
            if (!row || row.rowType !== "data") {
                continue;
            }

            stt += 1;
            const key = row.key != null
                ? String(row.key)
                : (row.data && row.data.ClientKey ? String(row.data.ClientKey) : "");
            if (key === String(clientKey)) {
                return stt;
            }
        }

        return "";
    }

    function getGridInstance() {
        const el = document.getElementById("nk-gridContainer");
        if (!el || !globalThis.DevExpress || !DevExpress.ui || !DevExpress.ui.dxDataGrid) {
            return null;
        }
        return DevExpress.ui.dxDataGrid.getInstance(el) || null;
    }

    function createGrid() {
        $("#nk-gridContainer").dxDataGrid({
            dataSource: [],
            keyExpr: "ClientKey",
            height: "100%",
            showBorders: true,
            showRowLines: true,
            showColumnLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            selection: { mode: "single" },

            columnAutoWidth: true,
            allowColumnResizing: true,
            columnResizingMode: "widget",
            wordWrapEnabled: true,

            paging: { enabled: false },
            sorting: { mode: "multiple" },
            grouping: { autoExpandAll: true },
            summary: {
                groupItems: [
                    {
                        column: "SLNhap",
                        summaryType: "sum",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        displayFormat: "{0}",
                        valueFormat: "#,##0.##"
                    },
                    {
                        column: "SLKiem",
                        summaryType: "sum",
                        showInGroupFooter: true,
                        alignByColumn: true,
                        displayFormat: "{0}",
                        valueFormat: "#,##0.##"

                    }
                ]
            },
            headerFilter: { visible: true },
            scrolling: {
                mode: "standard",
                showScrollbar: "always",
                useNative: false
            },
            noDataText: "Không có dữ liệu",
            onCellPrepared: function (e) {
                if (e.rowType !== "data") {
                    return;
                }

                if ((e.column.dataField === "SLNhap" || e.column.dataField === "SLKiem") && hasQuantityMismatch(e.data)) {
                    $(e.cellElement).addClass("qc-qty-mismatch");
                }
            },
            columns: [
                {
                    dataField: "STT",
                    caption: "STT",
                    width: 55,
                    alignment: "center",
                    allowSorting: false,
                    cellTemplate: function (container, options) {
                        const clientKey = options && options.data ? options.data.ClientKey : "";
                        const displayStt = getDisplaySttByClientKey(clientKey);
                        container.text(displayStt);
                    }
                },
                {
                    dataField: "POMua",
                    caption: "PO mua",
                    groupIndex: 0,
                    groupCellTemplate: function (container, options) {
                        $("<span>").addClass("po-group-title").text("PO mua: " + (options.value || "-")).appendTo(container);
                    }
                },
                { dataField: "ChungLoaiVatTu", caption: "Chủng loại", width: 110, alignment: "center" },
                { dataField: "MaVT", caption: "ItemCode", width: 120, alignment: "center" },
                {
                    dataField: "ChiTiet",
                    caption: "Mô tả",
                    minWidth: 140,
                    alignment: "left",
                    cssClass: "mota-wrap"
                },
                { dataField: "MauVT", caption: "Màu vật tư", width: 120, alignment: "center" },
                { dataField: "KhoVai", caption: "Width/Size", width: 110, alignment: "center" },
                { dataField: "TenDVVT", caption: "Đơn vị", width: 100, alignment: "center" },
                {
                    dataField: "SLNhap",
                    caption: "SL nhập",
                    width: 100,
                    alignment: "center",
                    dataType: "number",
                    format: "#,##0.##"
                },
                {
                    dataField: "SLKiem",
                    caption: "SL kiểm",
                    width: 100,
                    alignment: "center",
                    dataType: "number",
                    format: "#,##0.##"
                },
                {
                    dataField: "TrangThaiQC",
                    caption: "Trạng thái QC",
                    width: 110,
                    alignment: "center",
                    cellTemplate: function (container, options) {
                        const statusCode = resolveQcStatusCode(options.value, options.data);
                        const isPass = statusCode === "PASS";
                        const statusClass = isPass ? "qc-pass" : "qc-fail";
                        const statusText = isPass ? "Pass" : "Fail";
                        $("<span>")
                            .addClass("qc-status " + statusClass)
                            .text(statusText)
                            .appendTo(container);
                    }
                },
                {
                    caption: "",
                    width: 60,
                    alignment: "center",
                    allowSorting: false,
                    allowFiltering: false,
                    cellTemplate: function (container, options) {
                        $("<button>")
                            .addClass("qc-detail-btn")
                            .attr("type", "button")
                            .attr("title", "Xem chi tiết")
                            .html('<i class="fas fa-eye"></i>')
                            .on("click", function (evt) {
                                evt.preventDefault();
                                evt.stopPropagation();
                                openDetailPopup(options.data);
                            })
                            .appendTo(container);
                    }
                },
                {
                    dataField: "ActionChecked",
                    caption: "Duyệt",
                    width: 100,
                    alignment: "center",
                    allowSorting: false,
                    allowFiltering: false,
                    allowHeaderFiltering: false,
                    headerCellTemplate: function (container) {
                        const header = $('<div class="qc-approve-header"></div>');
                        $('<span>Duyệt</span>').appendTo(header);
                        $('<input class="qc-check-all-grid" type="checkbox" title="Chọn tất cả" />').appendTo(header);
                        header.appendTo(container);
                    },
                    cellTemplate: function (container, options) {
                        $("<input>")
                            .attr("type", "checkbox")
                            .addClass("qc-row-check")
                            .attr("data-client-key", options.data.ClientKey || "")
                            .prop("checked", !!options.data.ActionChecked)
                            .on("mousedown click", function (e) {
                                e.stopPropagation();
                            })
                            .on("change", function (e) {
                                e.stopPropagation();
                                setActionChecked(options.data, $(this).is(":checked"));
                                updateCheckAllCheckbox();
                            })
                            .appendTo(container);
                    }
                }
            ]
        });

        gridInstance = getGridInstance();
    }




    function renderGridData() {
        if (!gridInstance) {
            gridInstance = getGridInstance();
        }
        if (!gridInstance) {
            return;
        }

        gridInstance.option("dataSource", getVisibleRows());
        gridInstance.clearSelection();
        gridInstance.searchByText($("#nk-txt-search").val() || "");
        updateCheckAllCheckbox();

    }




    function loadData(callback, options) {
        const fromDate = options && options.fromDate ? normalizeDateOnly(options.fromDate) : null;
        const toDate = options && options.toDate ? normalizeDateOnly(options.toDate) : null;
        const queryParts = [];
        const fromDateText = formatDateForApi(fromDate);
        const toDateText = formatDateForApi(toDate);

        if (fromDateText) {
            queryParts.push("fromDate=" + encodeURIComponent(fromDateText));
        }
        if (toDateText) {
            queryParts.push("toDate=" + encodeURIComponent(toDateText));
        }

        const url = "/api/PheDuyetNK/Get" + (queryParts.length ? ("?" + queryParts.join("&")) : "");
        setToolbarBusy(true);
        showLoading("Đang tải dữ liệu...");

        $.ajax({
            url: url,
            method: "GET",
            dataType: "json",
            success: function (data) {
                rows = (data || []).map(normalizeRow);
                Object.keys(detailCache).forEach(function (k) { delete detailCache[k]; });
                autoSwitchToApprovedWhenNeeded();
                renderGridData();
                if (rows.length === 0) {
                    notify("Không có dữ liệu", "warning");
                }
                if (typeof callback === "function") {
                    callback(true);
                }
            },
            error: function (xhr) {
                const message = xhr?.responseText || "Không thể tải dữ liệu";
                notify(message, "error");
                if (typeof callback === "function") {
                    callback(false);
                }
            },
            complete: function () {
                setToolbarBusy(false);
                hideLoading();
            }
        });
    }

    function applyDateFilter() {
        const fromInstance = $("#nk-from-date").dxDateBox("instance");
        const toInstance = $("#nk-to-date").dxDateBox("instance");
        fromDateValue = normalizeDateOnly(fromInstance ? fromInstance.option("value") : fromDateValue);
        toDateValue = normalizeDateOnly(toInstance ? toInstance.option("value") : toDateValue);

        if (fromDateValue && toDateValue && fromDateValue > toDateValue) {
            toDateValue = fromDateValue;
            if (toInstance) {
                toInstance.option("value", toDateValue);
            }
        }

        clearActionChecks();
        loadData(null, {
            fromDate: fromDateValue,
            toDate: toDateValue
        });
    }

    function getSelectedItem() {
        if (!gridInstance) {
            return null;
        }

        const selected = gridInstance.getSelectedRowsData();
        if (!selected || selected.length === 0) {
            return null;
        }

        const key = selected[0].ClientKey;
        for (const row of rows) {
            if (row.ClientKey === key) {
                return row;
            }
        }

        return null;
    }

    function setActionChecked(item, checked) {
        if (!item || !item.ClientKey) {
            return;
        }

        const key = String(item.ClientKey);
        for (let i = 0; i < rows.length; i++) {
            if (String(rows[i].ClientKey) === key) {
                rows[i].ActionChecked = !!checked;
                break;
            }
        }
    }

    function clearActionChecks() {
        for (let i = 0; i < rows.length; i++) {
            rows[i].ActionChecked = false;
        }
    }

    function applyApprovalResultLocally(items, isApprove) {
        if (!Array.isArray(items) || !items.length) {
            return;
        }

        const mark = !!isApprove;
        for (let i = 0; i < rows.length; i++) {
            const current = rows[i];
            for (let j = 0; j < items.length; j++) {
                const target = items[j];
                if (String(current.SoLoID || "") === String(target.SoLoID || "")
                    && String(current.MaNPL || "") === String(target.MaNPL || "")
                    && toBit(current.IsNPL) === toBit(target.IsNPL)) {
                    current.IsDuyet = mark;
                    current.sDuyet = mark ? 1 : 0;
                    current.ActionChecked = false;
                    break;
                }
            }
        }
    }

    function getActionItems() {
        const approvedMode = isApprovedMode();
        const checked = rows.filter(function (item) {
            return item.ActionChecked === true && item.IsDuyet === approvedMode;
        });
        if (checked.length > 0) {
            return checked;
        }

        const selected = getSelectedItem();
        return selected ? [selected] : [];
    }

    function postApprove(isApprove) {
        if (postInProgress) {
            return;
        }

        const actionItems = getActionItems();
        if (!actionItems.length) {
            const counts = getApprovalCounts();
            if (!isApprove && !isApprovedMode() && counts.approved > 0 && counts.unapproved === 0) {
                notify("Không có dòng chưa duyệt. Bật chế độ Đã duyệt để Hủy.", "warning");
                return;
            }
            notify("Vui lòng chọn dòng cần thao tác", "warning");
            return;
        }

        const eligibleItems = actionItems.filter(function (item) {
            return isApprove ? !item.IsDuyet : item.IsDuyet;
        });
        if (!eligibleItems.length) {
            notify(isApprove ? "Không có dòng nào cần duyệt" : "Không có dòng nào cần hủy duyệt", "warning");
            return;
        }

        const payload = eligibleItems.map(function (item) {
            return {
                SoLoID: item.SoLoID,
                MaNPL: item.MaNPL,
                IsNPL: toBit(item.IsNPL)
            };
        });

        $.ajax({
            url: isApprove ? "/api/PheDuyetNK/Approve" : "/api/PheDuyetNK/Cancel",
            method: "POST",
            data: JSON.stringify(payload),
            contentType: "application/json; charset=utf-8",
            beforeSend: function () {
                postInProgress = true;
                setToolbarBusy(true);
                showLoading(isApprove ? "Đang duyệt dữ liệu..." : "Đang hủy duyệt...");
            },
            success: function (result) {
                if (typeof result === "string" && result !== "True") {
                    notify(result, "error");
                    return;
                }
                applyApprovalResultLocally(eligibleItems, isApprove);
                autoSwitchToApprovedWhenNeeded();
                renderGridData();
                notify(isApprove ? "Đã duyệt thành công" : "Đã hủy duyệt", "success");
            },
            error: function (xhr) {
                const message = xhr?.responseText || "Thao tác thất bại";
                notify(message, "error");
            },
            complete: function () {
                postInProgress = false;
                setToolbarBusy(false);
                hideLoading();
            }
        });
    }

    function formatNumber(value) {
        const num = Number(value || 0);
        if (!isFinite(num)) {
            return "0";
        }
        return num.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }


    function escapeHtml(value) {
        return String(value == null ? "" : value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function mapDetailRows(rawRows) {
        if (!Array.isArray(rawRows)) {
            return [];
        }

        const mapped = rawRows.map(function (row) {
            const soRoll = row.SoRoll
                || row.SoRollHienThi
                || row.SoKienHienThi
                || row.SoKien
                || row.RollNo
                || "";
            const slTheoCt = row.SLNhap != null ? row.SLNhap : (row.SoGhiDauCay != null ? row.SoGhiDauCay : row.SLTheoCT);
            const slThucTe = row.SLKiem != null ? row.SLKiem : (row.SoLuongThucTe != null ? row.SoLuongThucTe : row.SLThucTe);
            const rawBarcode = String(row.BarCode || "").trim();
            let barcodeDisplay = rawBarcode;

            if (rawBarcode.indexOf("|") >= 0) {
                const suffix = rawBarcode.substring(rawBarcode.lastIndexOf("|") + 1).trim();
                if (!suffix) {
                    barcodeDisplay = "";
                }
            }

            return {
                stt: 0,
                soRoll: soRoll,
                lot: row.SoLoT || row.SoLot || row.SoLo || row.Lot || "",
                batch: row.Batch || row.SoBatch || row.BatchNo || "",
                donVi: row.TenDVVT || row.DonVi || row.Unit || "",
                slCt: Number(slTheoCt || 0),
                slThucTe: Number(slThucTe || 0),
                trangThai: row.TrangThai || row.Status || "",
                palet: row.Pallet || "",
                ghiChu: row.GhiChu || "",
                barcode: barcodeDisplay,
                raw: row
            };
        });

        for (let i = 0; i < mapped.length; i++) {
            mapped[i].stt = i + 1;
            if (!mapped[i].soRoll) {
                mapped[i].soRoll = "-";
            }
        }
        return mapped;
    }


    function buildDetailPopupContent(item, detailRows) {
        const infoHtml = [
            '<div class="qc-detail-info">',
            '<div><span>PO mua:</span><strong>' + escapeHtml(item?.POMua || "-") + "</strong></div>",
            '<div><span>Chủng loại:</span><strong>' + escapeHtml(item?.ChungLoaiVatTu || "-") + "</strong></div>",
            '<div><span>ItemCode:</span><strong>' + escapeHtml(item?.MaVT || "-") + "</strong></div>",
            '<div><span>Mô tả:</span><strong>' + escapeHtml(item?.ChiTiet || "-") + "</strong></div>",
            '<div><span>Màu vật tư:</span><strong>' + escapeHtml(item?.MauVT || "-") + "</strong></div>",
            '<div><span>Width/Size:</span><strong>' + escapeHtml(item?.KhoVai || "-") + "</strong></div>",
            '<div><span>Đơn vị:</span><strong>' + escapeHtml(item?.TenDVVT || "-") + "</strong></div>",
            "</div>"
        ].join("");

        const rowsHtml = (detailRows || []).map(function (row) {
            return [
                "<tr>",
                "<td>", row.stt, "</td>",
                "<td>", escapeHtml(row.soRoll), "</td>",
                "<td>", escapeHtml(row.lot), "</td>",
                "<td>", escapeHtml(row.batch), "</td>",
                "<td>", escapeHtml(row.donVi), "</td>",
                "<td class=\"num\">", formatNumber(row.slCt), "</td>",
                "<td class=\"num\">", formatNumber(row.slThucTe), "</td>",
                "<td>", escapeHtml(row.palet), "</td>",
                "<td>", escapeHtml(row.ghiChu || ""), "</td>",
                "<td>", escapeHtml(row.barcode), "</td>",
                "</tr>"
            ].join("");
        }).join("");

        const noDataRow = rowsHtml
            ? ""
            : '<tr><td colspan="10" class="qc-detail-empty">Không có dữ liệu chi tiết</td></tr>';

        return [
            '<div class="qc-detail-wrap">',
            infoHtml,
            '<div class="qc-detail-table-wrap">',
            '<table class="qc-detail-table">',
            "<thead><tr>",
            "<th>STT</th><th>Số Roll</th><th>LOT</th><th>Batch</th><th>Đơn vị</th><th>SL theo CT</th><th>SL thực tế</th><th>Palet</th><th>Ghi chú</th><th>BarCode</th>",
            "</tr></thead>",
            "<tbody>", rowsHtml, noDataRow, "</tbody>",
            "</table>",
            "</div>",
            "</div>"
        ].join("");
    }

    function fetchDetailData(item) {
        if (!item) {
            return Promise.resolve([]);
        }
        const cacheKey = [item.SoLoID || "", toBit(item.IsNPL), item.MaNPL || ""].join("|");
        if (detailCache[cacheKey]) {
            return Promise.resolve(detailCache[cacheKey]);
        }

        function parseDetailPayload(data) {
            if (data == null) {
                return [];
            }

            if (Array.isArray(data)) {
                return data;
            }

            if (typeof data === "string") {
                const raw = data.trim();
                if (!raw) {
                    return [];
                }

                try {
                    return parseDetailPayload(JSON.parse(raw));
                } catch (e) {
                    return [];
                }
            }

            if (data && typeof data === "object") {
                if (Array.isArray(data.Rows)) {
                    return data.Rows;
                }

                if (Array.isArray(data.rows)) {
                    return data.rows;
                }

                if (Array.isArray(data.Data)) {
                    return data.Data;
                }

                if (Array.isArray(data.data)) {
                    return data.data;
                }

                if (Array.isArray(data.Table)) {
                    return data.Table;
                }

                if (Array.isArray(data.Table1)) {
                    return data.Table1;
                }

                const keys = Object.keys(data);
                for (let i = 0; i < keys.length; i++) {
                    const val = data[keys[i]];
                    if (Array.isArray(val)) {
                        return val;
                    }

                    if (typeof val === "string") {
                        const text = val.trim();
                        if (text.startsWith("[") || text.startsWith("{")) {
                            const parsed = parseDetailPayload(text);
                            if (parsed.length > 0) {
                                return parsed;
                            }
                        }
                    }
                }
            }

            return [];
        }

        function callDetail(maNpl) {
            const soLoId = String(item.SoLoID || "").trim();
            const isNpl = String(toBit(item.IsNPL));
            const maNplValue = String(maNpl || "").trim();
            const query =
                "?parameter=" + encodeURIComponent(soLoId) +
                "&parameter2=" + encodeURIComponent(isNpl) +
                "&parameter3=" + encodeURIComponent(maNplValue);

            return $.ajax({
                url: "/api/PheDuyetNK/Detail" + query,
                method: "GET",
                dataType: "json"
            }).then(parseDetailPayload);
        }

        return callDetail(item.MaNPL || "").then(function (rows) {
            const result = rows || [];
            detailCache[cacheKey] = result;
            return result;
        });
    }

    function ensureDetailPopup() {
        if (detailPopupInstance) {
            return detailPopupInstance;
        }

        let popupEl = $("#qc-detail-popup");
        if (!popupEl.length) {
            popupEl = $('<div id="qc-detail-popup"></div>').appendTo("body");
        }

        popupEl.dxPopup({
            title: "Chi tiết vật tư",
            width: "99vw",
            height: "96vh",
            maxWidth: 1920,
            showCloseButton: true,
            dragEnabled: false,
            hideOnOutsideClick: true
        });

        detailPopupInstance = popupEl.dxPopup("instance");
        return detailPopupInstance;
    }

    function openDetailPopup(item) {
        showLoading("Đang tải dữ liệu chi tiết...");
        const popup = ensureDetailPopup();
        popup.option("contentTemplate", function (contentElement) {
            $(contentElement).html('<div class="qc-detail-loading">Đang tải dữ liệu chi tiết...</div>');
        });
        popup.show();

        fetchDetailData(item)
            .then(function (rawRows) {
                const mappedRows = mapDetailRows(rawRows);
                popup.option("contentTemplate", function (contentElement) {
                    $(contentElement).html(buildDetailPopupContent(item, mappedRows));
                });
            })
            .catch(function () {
                popup.option("contentTemplate", function (contentElement) {
                    $(contentElement).html(buildDetailPopupContent(item, []));
                });
                notify("Không tải được dữ liệu chi tiết", "error");
            })
            .then(function () {
                hideLoading();
            });
    }




    function getCheckedRowsForExport() {
        const checked = rows.filter(function (item) {
            return item.ActionChecked === true;
        });

        return checked.map(function (item, index) {
            const clone = $.extend({}, item);
            clone.STT = index + 1;
            return clone;
        });
    }






    function exportExcel() {
        const data = getCheckedRowsForExport();
        if (!data.length) {
            notify("Vui lòng chọn checkbox các dòng cần xuất", "warning");
            return;
        }

        setToolbarBusy(true);
        showLoading("Đang xuất Excel...");
        fetch("/api/PheDuyetNK/ExportExcel", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Rows: data })
        })
            .then(function (res) {
                if (!res.ok) {
                    throw new Error("Export failed");
                }
                return res.blob();
            })
            .then(function (blob) {
                const blobUrl = window.URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = blobUrl;
                a.download = "PXNK-da-duyet.xlsx";
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                window.URL.revokeObjectURL(blobUrl);
                notify("Đã xuất file excel", "success");
            })
            .catch(function (error) {
                console.error(error);
                notify("Không thể xuất excel", "error");
            })
            .then(function () {
                setToolbarBusy(false);
                hideLoading();
            });
    }
    function wireEvents() {
        $("#nk-chk-da-duyet").off("change").on("change", function () {
            clearActionChecks();
            renderGridData();
        });

        $(document).off("click", ".qc-check-all-grid").on("click", ".qc-check-all-grid", function (e) {
            e.stopPropagation();
        });

        $(document).off("change", ".qc-check-all-grid").on("change", ".qc-check-all-grid", function (e) {
            e.stopPropagation();
            setCheckAllVisible($(this).is(":checked"));
        });

        $("#nk-txt-search").off("input").on("input", function () {
            if (gridInstance) {
                gridInstance.searchByText(this.value || "");
            }
        });

        $("#nk-btn-duyet").off("click").on("click", function () { postApprove(true); });
        $("#nk-btn-huy-duyet").off("click").on("click", function () { postApprove(false); });
        $("#nk-btn-export-excel").off("click").on("click", exportExcel);
        $("#nk-btn-filter").off("click").on("click", applyDateFilter);
    }

    function waitForDevExtreme(ready, attempt) {
        const tries = attempt || 0;
        if ($.fn && typeof $.fn.dxDataGrid === "function" && typeof $.fn.dxDateBox === "function") {
            ready();
            return;
        }
        if (tries > 50) {
            console.error("DevExtreme is not available, cannot init PheDuyetNK.");
            return;
        }
        setTimeout(function () { waitForDevExtreme(ready, tries + 1); }, 100);
    }

    function initPageContent() {
        waitForDevExtreme(function () {
            $("#nk-chk-da-duyet").prop("checked", false);
            setToolbarBusy(false);
            hideLoading();

            const existingGrid = getGridInstance();
            if (existingGrid) {
                existingGrid.dispose();
                $("#nk-gridContainer").empty();
            }

            initDateFilters();
            createGrid();
            wireEvents();
            applyDateFilter();
        });
    }

    $(function () {
        initPageContent();
    });

    window.initPageContent = initPageContent;
})();


