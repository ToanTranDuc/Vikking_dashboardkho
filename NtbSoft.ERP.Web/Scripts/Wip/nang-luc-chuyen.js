$(function () {
    initHistoryFeature();
    initTabPanel();
    $("#Layer_1").click();
});
var tglvLineCache = null;
var tglvLinePromise = null;
var tglvFilterLineId = null;
var tglvFilterFromDate = null;
var tglvFilterToDate = null;
var tglvApplyLineId = null;
var historyPopup = null;
var historyTabPanel = null;
var historyLoadXhr = null;
var historyTabs = [
    { id: 1, title: "Năng lực chuyền" },
    { id: 2, title: "Lịch làm việc" },
    { id: 3, title: "Hệ số lợi nhuận" },
    { id: 4, title: "SMV" }
];
var historyLibCodeByTabId = {
    1: "NANG_LUC_CHUYEN",
    2: "LICH_LAM_VIEC",
    3: "HE_SO_LOI_NHUAN",
    4: "SMV"
};
var historyFieldCaptionByLibCode = {
    NANG_LUC_CHUYEN: {
        MaChuyen: "Chuyền",
        NgayApDung: "Ngày áp dụng",
        SLCN: "Số công nhân",
        TrangThai: "Trạng thái"
    },
    LICH_LAM_VIEC: {
        MaChuyen: "Chuyền",
        NgayApDung: "Thời gian",
        GioBatDau: "Giờ bắt đầu",
        GioKetThuc: "Giờ kết thúc",
        GioNghi: "Giờ nghỉ",
        SoGioTangCa: "Số giờ tăng ca",
        SoGio: "Số giờ làm việc(ngày)"
    },
    HE_SO_LOI_NHUAN: {
        HeSo: "Hệ số",
        NgayApDung: "Ngày áp dụng"
    },
    SMV: {
        MaHang: "Tên hàng",
        TenCL: "Tên chủng loại",
        NgayApDung: "Ngày áp dụng",
        SMV: "SMV (phút)",
        SMV_May: "SMV May",
        SMV_Cat: "SMV Cắt",
        SMV_Laptrinh: "SMV Lập trình",
        SMV_HoanThanh: "SMV Hoàn thành",
        In_ep: "In thêu",
        InTheuCT: "In thêu CT",
        HutAm: "Hút ẩm",
        DoKim: "Dò kim",
        CM: "CM",
        HasSMV: "Đã khai báo"
    }
};
var historyFieldFriendlyMap = {
    SLCN: "Số công nhân",
    NgayApDung: "Ngày áp dụng",
    TrangThai: "Trạng thái"
};

function initHistoryFeature() {
    if ($("#btnViewHistory").length) {
        $("#btnViewHistory").dxButton({
            text: "Xem lịch sử",
            type: "default",
            stylingMode: "contained",
            onClick: function () {
                if (historyPopup) historyPopup.show();
            }
        });
    }

    if ($("#historyPopup").length) {
        historyPopup = $("#historyPopup").dxPopup({
            visible: false,
            title: "Lịch sử thay đổi",
            width: "92vw",
            height: "90vh",
            minWidth: 980,
            minHeight: 620,
            maxWidth: 1760,
            maxHeight: 940,
            wrapperAttr: { class: "wip-history-popup" },
            closeOnOutsideClick: true,
            showCloseButton: true,
            contentTemplate: function (contentElement) {
                $("<div>").attr("id", "historyTabPanel").appendTo(contentElement);
            },
            onShown: function () {
                if (!historyTabPanel) {
                    initHistoryTabPanel();
                    return;
                }
                var currentTab = getCurrentHistoryTab();
                if (currentTab) {
                    loadHistoryByTab(currentTab);
                }
                setTimeout(function () {
                    try {
                        var tabItems = historyTabs || [];
                        tabItems.forEach(function (x) {
                            var g = $("#history_grid_" + x.id).dxDataGrid("instance");
                            if (g) g.updateDimensions();
                        });
                    } catch (e) { }
                }, 0);
            }
        }).dxPopup("instance");
    }
}

function initHistoryTabPanel() {
    if (historyTabPanel) return;

    historyTabPanel = $("#historyTabPanel").dxTabPanel({
        dataSource: historyTabs,
        selectedIndex: 0,
        height: "100%",
        deferRendering: false,
        onSelectionChanged: onTabSelectionChanged,
        itemTitleTemplate: function (itemData) {
            return $("<span>").text(itemData.title);
        },
        itemTemplate: function (itemData, itemIndex, itemElement) {
            var gridId = "history_grid_" + itemData.id;
            $("<div>").attr("id", gridId).addClass("history-grid").appendTo(itemElement);
            initHistoryGrid("#" + gridId);
        }
    }).dxTabPanel("instance");

    onTabSelectionChanged({ addedItems: [historyTabs[0]] });
}

function getCurrentHistoryTab() {
    if (!historyTabPanel) return null;
    var dataSource = historyTabPanel.option("dataSource") || historyTabs;
    var idx = Number(historyTabPanel.option("selectedIndex") || 0);
    return dataSource[idx] || null;
}

function onTabSelectionChanged(e) {
    var selected = e && e.addedItems && e.addedItems.length ? e.addedItems[0] : null;
    if (!selected) return;
    loadHistoryByTab(selected);
}

function resolveHistoryFieldCaption(libCode, fieldName) {
    var field = String(fieldName || "").trim();
    if (!field) return field;

    if (historyFieldFriendlyMap[field]) return historyFieldFriendlyMap[field];
    var friendlyKeys = Object.keys(historyFieldFriendlyMap);
    var fieldLower = field.toLowerCase();
    for (var j = 0; j < friendlyKeys.length; j++) {
        if (friendlyKeys[j].toLowerCase() === fieldLower) {
            return historyFieldFriendlyMap[friendlyKeys[j]];
        }
    }

    var dict = historyFieldCaptionByLibCode[libCode] || {};
    if (dict[field]) return dict[field];

    var lowerField = fieldLower;
    var keys = Object.keys(dict);
    for (var i = 0; i < keys.length; i++) {
        if (String(keys[i]).toLowerCase() === lowerField) {
            return dict[keys[i]];
        }
    }

    return field;
}

function formatAuditDateTime(value) {
    if (!value) return "";
    var d = new Date(value);
    if (isNaN(d.getTime())) return String(value);

    var dd = String(d.getDate()).padStart(2, "0");
    var mm = String(d.getMonth() + 1).padStart(2, "0");
    var yyyy = d.getFullYear();
    var hh = String(d.getHours()).padStart(2, "0");
    var mi = String(d.getMinutes()).padStart(2, "0");

    return dd + "/" + mm + "/" + yyyy + " " + hh + ":" + mi;
}

function formatAuditDateOnly(value) {
    if (!value) return "";
    var d = new Date(value);
    if (isNaN(d.getTime())) return String(value);

    var dd = String(d.getDate()).padStart(2, "0");
    var mm = String(d.getMonth() + 1).padStart(2, "0");
    var yyyy = d.getFullYear();

    return dd + "/" + mm + "/" + yyyy;
}

function formatAuditValueByField(fieldName, value) {
    var field = String(fieldName || "").trim().toLowerCase();
    if (!field) return value;

    if (field === "ngayapdung") {
        return formatAuditDateOnly(value);
    }

    return value;
}

function mapHistoryItemByCaption(item, libCode) {
    var source = item || {};
    var fieldName = source.FieldName != null ? source.FieldName : source.fieldName;
    var tenChuyen = source.TenChuyen != null ? source.TenChuyen : source.tenChuyen;
    var maHang = source.MaHang != null ? source.MaHang : source.maHang;
    var ngayApDung = source.NgayApDung;
    var oldValue = source.OldValue != null ? source.OldValue : source.oldValue;
    var newValue = source.NewValue != null ? source.NewValue : source.newValue;

    return $.extend({}, source, {
        ObjectName: tenChuyen || maHang || "",
        NgayApDung: formatAuditDateOnly(ngayApDung),
        FieldNameRaw: fieldName,
        FieldName: resolveHistoryFieldCaption(libCode, fieldName),
        OldValue: formatAuditValueByField(fieldName, oldValue),
        NewValue: formatAuditValueByField(fieldName, newValue)
    });
}

function loadHistoryByTab(selectedTab) {
    if (!selectedTab) return;
    var gridSelector = "#history_grid_" + selectedTab.id;
    var grid = $(gridSelector).dxDataGrid("instance");
    var libCode = historyLibCodeByTabId[selectedTab.id] || null;

    if (!grid || !libCode) return;
    try { historyLoadXhr?.abort(); } catch (e) { }

    grid.beginCustomLoading("Đang tải lịch sử...");
    historyLoadXhr = $.ajax({
        url: "/api/ThuVienWip/get-history",
        method: "GET",
        dataType: "json",
        data: { libCode: libCode },
        success: function (res) {
            var items = Array.isArray(res) ? res : [];
            var mapped = items.map(function (x) {
                return mapHistoryItemByCaption(x, libCode);
            });
            grid.option("dataSource", mapped);
        },
        error: function (xhr, status) {
            if (status === "abort") return;
            grid.option("dataSource", []);
            DevExpress.ui.notify("Không tải được lịch sử thay đổi", "error", 2200);
        },
        complete: function () {
            grid.endCustomLoading();
            historyLoadXhr = null;
        }
    });
}

function initHistoryGrid(gridSelector) {
    $(gridSelector).dxDataGrid({
        dataSource: [],
        keyExpr: "ID",
        height: "100%",
        showBorders: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        sorting: { mode: "multiple" },
        paging: { pageSize: 40 },
        columns: [
            { dataField: "ID", caption: "ID", width: 70, visible: false },
            {
                caption: "Chuyền / Mã hàng",
                dataField: "ObjectName",
                calculateCellValue: function (row) { return row.TenChuyen || row.MaHang || row.ObjectName || ""; },
                groupIndex: 0,
                sortOrder: "asc",
                groupCellTemplate: function (container, options) {
                    $("<span>").css({ "font-weight": 600 }).text(options.value || "Hệ số").appendTo(container);
                }
            },
            { dataField: "NgayApDung", caption: "Ngày áp dụng" },
            { dataField: "FieldName", caption: "Trường thay đổi" },
            {
                dataField: "OldValue",
                caption: "Giá trị cũ",
                cellTemplate: function (container, options) {
                    $("<span>").css({ color: "#b91c1c" }).text(options.value == null || options.value === "" ? "-" : options.value).appendTo(container);
                }
            },
            {
                dataField: "NewValue",
                caption: "Giá trị mới",
                cellTemplate: function (container, options) {
                    var value = options.value == null || options.value === "" ? "-" : options.value;
                    $("<span>").css({ color: "#166534", "font-weight": 600 }).text(value).appendTo(container);
                }
            },
            { dataField: "ModifiedByName", caption: "Người sửa" },
            {
                dataField: "ModifiedAt",
                caption: "Thời gian",
                dataType: "datetime",
                format: "dd/MM/yyyy HH:mm:ss",
                sortOrder: "desc"
            },
        ]
    });
}

function normalizeLineItems(data) {
    return (data || []).map(function (x) {
        return {
            Id: Number(x.Id ?? x.ID ?? x.id ?? 0),
            Name: x.Name ?? x.name ?? ""
        };
    }).filter(function (x) { return x.Id > 0; });
}
function withGiaCongLine(lines) {
    var items = (lines || []).slice();
    var exists = items.some(function (x) { return Number(x.Id) === 900 && Number(x.Id) === 901; });
    if (!exists) {
        items.push({ Id: 900, Name: "GIA CÔNG L AND Y" }, { Id: 901, Name: "GIA CÔNG SAIGON SAO"});
    }
    return items;
}
function loadTGLVLineList() {
    if (tglvLineCache) {
        return $.Deferred().resolve(tglvLineCache).promise();
    }
    if (tglvLinePromise) return tglvLinePromise;

    tglvLinePromise = $.ajax({
        url: "/api/wip-donhang/get-list-linex",
        method: "GET",
        dataType: "json"
    }).then(function (res) {
        tglvLineCache = normalizeLineItems(res);
        return tglvLineCache;
    });
    return tglvLinePromise;
}

function toTGLVLocalDateOnly(value) {
    if (!value) return null;
    var d = new Date(value);
    if (isNaN(d.getTime())) return null;
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

function formatTGLVDateForApi(value) {
    var d = toTGLVLocalDateOnly(value);
    if (!d) return null;
    return d.getFullYear() + "-" +
        String(d.getMonth() + 1).padStart(2, "0") + "-" +
        String(d.getDate()).padStart(2, "0") + "T00:00:00";
}

function hasTGLVDateFilter() {
    return !!(tglvFilterFromDate || tglvFilterToDate);
}

function getTGLVDisplayFilterKey() {
    return [
        tglvFilterLineId || "",
        formatTGLVDateForApi(tglvFilterFromDate) || "",
        formatTGLVDateForApi(tglvFilterToDate) || ""
    ].join("|");
}

function applyTGLVLineFilter(gridId) {
    var grid = $(gridId).dxDataGrid("instance");
    if (!grid) return;
    var fromDate = toTGLVLocalDateOnly(tglvFilterFromDate);
    var toDate = toTGLVLocalDateOnly(tglvFilterToDate);
    if (fromDate && toDate && toDate < fromDate) {
        DevExpress.ui.notify("Đến ngày phải lớn hơn hoặc bằng Từ ngày", "warning", 2000);
        return;
    }
    var expectedFilterKey = getTGLVDisplayFilterKey();
    if (grid.__tglvLoadedFilterKey === expectedFilterKey) return;
    grid.__tglvLoadedFilterKey = expectedFilterKey;
    grid.pageIndex(0);
    grid.refresh();
}

function fitPanelToGrid(panelSelector, gridSelector) {
    setTimeout(function () {
        var $panel = $(panelSelector);
        var $gridHost = $(gridSelector);
        if (!$panel.length || !$gridHost.length) return;

        var grid = $gridHost.dxDataGrid("instance");
        if (grid) {
            try { grid.updateDimensions(); } catch (e) { }
        }

        var $gridRoot = $gridHost.find(".dx-datagrid").first();
        if (!$gridRoot.length) return;

        var gridWidth = Math.ceil($gridRoot.outerWidth(true) || 0);
        if (gridWidth <= 0) return;

        var parentWidth = Math.floor($panel.parent().width() || 0);
        var finalWidth = parentWidth > 0 ? Math.min(gridWidth + 2, parentWidth) : (gridWidth + 2);

        $panel.css("width", finalWidth + "px");
    }, 0);
}

function initTabPanel() {
    $("#tabPanel").dxTabPanel({
        dataSource: [
            { id: 0, title: "Năng lực chuyền", icon: "chart" },
            { id: 1, title: "Lịch làm việc", icon: "event" },
            { id: 2, title: "Hệ số lợi nhuận", icon: "money" },
            { id: 3, title: "Mã hàng", icon: "preferences" },
            { id: 4, title: "Ngày nghỉ", icon: "folder" }
        ],
        selectedIndex: 0,
        loop: false,
        animationEnabled: true,
        swipeEnabled: false,
        itemTitleTemplate: function (itemData) {
            return $("<span>")
                .addClass("dx-icon-" + itemData.icon)
                .append(" " + itemData.title);
        },
        onSelectionChanged: function (e) {
            var selectedTab = e.addedItems[0];
            loadTabContent(selectedTab.id);
        },
        itemTemplate: function (itemData, itemIndex, itemElement) {
            itemElement.append("<div class='grid-container' id='grid_" + itemData.id + "'></div>");
        }
    });

    // Load tab đầu tiên
    loadTabContent(0);
}
function loadTabContent(tabId) {
    switch (tabId) {
        case 0: loadNangLucChuyen(); break;
        case 1: loadTGLVFullTab(); break; // Gọi hàm load TGLV riêng
        case 2: loadHSLNFullTab(); break; // Gọi hàm load HSLN riêng
        case 3: loadSMV(); break;
        case 4: loadThuVienNgayNghiTab(); break;
    }
}
function loadNangLucChuyen() {
    var gridId = "#grid_0";
    var getGridData = function () {
        return $(gridId).dxDataGrid("instance").getDataSource().items();
    };
    if ($(gridId).data("dxDataGrid")) {
        $(gridId).dxDataGrid("instance").dispose();
        $(gridId).empty();
    }
    $(gridId).addClass("grid--tglv-blue");

    const store = new DevExpress.data.CustomStore({
        key: "ID",

        load: function () {
            return $.ajax({
                url: "/api/ThuVienWip/nang-luc-chuyen",
                method: "GET",
                dataType: "json"
            });
        },

        insert: function (values) {
            values = values || {};
            return $.ajax({
                url: "/api/ThuVienWip/nang-luc-chuyen",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    Action: "Insert",
                    MaChuyen: values.MaChuyen,
                    SLCN: values.SLCN,
                    NgayApDung: values.NgayApDung,
                    NguoiTao: values.NguoiTao || getCurrentUser()
                }),
                processData: false
            });
        },

        //update: function (key, values) {
        //    return store.byKey(key).then(function (oldData) {
        //        var mergedData = $.extend({}, oldData, values);

        //        return $.ajax({
        //            url: "/api/ThuVienWip/nang-luc-chuyen",
        //            method: "POST",
        //            contentType: "application/json; charset=utf-8",
        //            data: JSON.stringify({
        //                Action: "Update",
        //                ID: key,
        //                MaChuyen: mergedData.MaChuyen,
        //                SLCN: mergedData.SLCN,
        //                NgayApDung: mergedData.NgayApDung,
        //                NguoiTao: getCurrentUser()
        //            })
        //        });
        //    });
        //},
        update: function (key, values) {
            var currentItems = getGridData();
            var oldData = currentItems.find(function (item) { return item.ID === key; });

            if (!oldData) oldData = {};
            var mergedData = $.extend({}, oldData, values);

            return $.ajax({
                url: "/api/ThuVienWip/nang-luc-chuyen",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    Action: "Update",
                    ID: key,
                    MaChuyen: mergedData.MaChuyen,
                    SLCN: mergedData.SLCN,
                    NgayApDung: mergedData.NgayApDung,
                    ModifiedBy: getCurrentUser()
                })
            });
        },
        remove: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/nang-luc-chuyen",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    Action: "Delete",
                    ID: key,
                    ModifiedBy: getCurrentUser()
                }),
                processData: false
            });
        },
        byKey: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/nang-luc-chuyen",
                method: "GET",
                dataType: "json"
            }).then(function (data) {
                return data.find(function (item) {
                    return item.ID === key;
                });
            });
        }
    });
    $(gridId).dxDataGrid({
        dataSource: store,
        remoteOperations: false,
        columns: [
            {
                dataField: "ID",
                caption: "ID",
                allowEditing: false,
                visible: false
            },
            {
                dataField: "MaChuyen",
                caption: "Chuyền",
                lookup: {
                    dataSource: {
                        store: new DevExpress.data.CustomStore({
                            key: "Id",
                            load: function () {
                                return $.ajax({
                                    url: "/api/wip-donhang/get-list-linex",
                                    method: "GET",
                                    dataType: "json"
                                }).then(function (res) {
                                    var lines = normalizeLineItems(res);
                                    return withGiaCongLine(lines);
                                });
                            },
                            byKey: function (key) {
                                key = Number(key);
                                if (key === 900) {
                                    return $.Deferred().resolve({ Id: 900, Name: "Gia công" }).promise();
                                }

                                return $.ajax({
                                    url: "/api/wip-donhang/get-list-linex",
                                    method: "GET",
                                    dataType: "json"
                                }).then(function (data) {
                                    var items = withGiaCongLine(normalizeLineItems(data));
                                    return items.find(function (item) {
                                        return Number(item.Id) === key;
                                    }) || null;
                                });
                            },
                        })
                    },
                    valueExpr: "Id",
                    displayExpr: function (item) {
                        return item ? item.Name : "";
                    }
                },
                validationRules: [{
                    type: "required",
                    message: "Vui lòng chọn chuyền"
                }]
            },
            {
                dataField: "NgayApDung",
                caption: "Ngày áp dụng",
                dataType: "date",
                format: "dd/MM/yyyy",
                editorOptions: {
                    type: "date",
                    displayFormat: "dd/MM/yyyy"
                },
                validationRules: [{
                    type: "required",
                    message: "Vui lòng chọn ngày áp dụng"
                }]
            },
            {
                dataField: "SLCN",
                caption: "Số lượng công nhân",
                dataType: "number",
                alignment: "right",
                format: "#,##0",
                editorOptions: {
                    min: 0,
                    showSpinButtons: true
                },
                validationRules: [
                    { type: "range", min: 1, message: "Số lượng công nhân phải lớn hơn 0" }
                ]
            },
            //{
            //    dataField: "TenNguoiTao",
            //    caption: "Người tạo",
            //    allowEditing: false
            //},
            //{
            //    dataField: "NgayTao",
            //    caption: "Ngày tạo",
            //    dataType: "datetime",
            //    format: "dd/MM/yyyy HH:mm",
            //    allowEditing: false
            //}
        ],
        //editing: {
        //    mode: "row",
        //    allowAdding: true,
        //    allowUpdating: true,
        //    allowDeleting: true,
        //    useIcons: true
        //},
        editing: {
            mode: "batch",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },
        toolbar: {
            items: [
                "addRowButton",
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Lưu",
                        icon: "save",
                        hint: "Lưu thay đổi",
                        width: 96,
                        stylingMode: "contained",
                        type: "success",
                        onClick: function () {
                            var grid = $(gridId).dxDataGrid("instance");
                            if (grid) {
                                grid.saveEditData();
                            }
                        }
                    }
                },
                {
                    name: "revertButton",
                    location: "after"
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Làm mới",
                        icon: "refresh",
                        stylingMode: "contained",
                        type: "default",
                        onClick: function () {
                            $(gridId).dxDataGrid("instance").refresh();
                        }
                    }
                },
            ]
        },
        headerFilter: { visible: true },
        filterRow: { visible: true },
        paging: { pageSize: 20 },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [10, 20, 50]
        },
        showBorders: true,
        rowAlternationEnabled: true,
        onInitNewRow: function (e) {
            e.data.NgayApDung = new Date();
            e.data.NguoiTao = getCurrentUser();
        },
        onSaved: function () {
            DevExpress.ui.notify("Đã lưu thay đổi!", "success", 2000);
        },
        onDataErrorOccurred: function (e) {
            DevExpress.ui.notify(
                e.error?.message || "Có lỗi khi lưu dữ liệu",
                "error",
                3000
            );
        }
    });
}
function loadTGLVFullTab() {
    var containerId = "#grid_1";
    if ($(containerId).length === 0) return;

    $(containerId).empty().append(`
        <div class="ns-panel ns-panel--tglv" style="margin-top: 5px; width: 100%;">
            <div class="ns-panel__header ns-panel__header--with-lines">
                <div class="ns-panel__title">THIẾT LẬP LỊCH LÀM VIỆC (SỐ GIỜ/NGÀY)</div>
                <div class="tglv-line-topbar">
                    <div id="tglv_line_tabs"></div>
                </div>
            </div>
            <div class="ns-panel__body">
                <div id="grid_tglv_main"></div>
            </div>
        </div>
    `);
    loadTGLVLineList().then(function (lines) {
        var lineItems = lines.concat({ Id: 900, Name: "Gia Công"}) || [];
        var tabItems = [{ Id: null, Name: "Tất cả" }].concat(lineItems);

        if (lineItems.length > 0) {
            var currentLineStillExists = lineItems.some(function (x) { return x.Id === tglvFilterLineId; });
            if (!currentLineStillExists) {
                tglvFilterLineId = lineItems[0].Id;
            }
            var applyLineStillExists = lineItems.some(function (x) { return x.Id === tglvApplyLineId; });
            if (!applyLineStillExists) {
                tglvApplyLineId = lineItems[0].Id;
            }
        } else {
            tglvFilterLineId = null;
            tglvApplyLineId = null;
        }

        $("#tglv_line_tabs").dxTabs({
            dataSource: tabItems,
            keyExpr: "Id",
            selectedItem: tabItems.find(function (x) { return x.Id === tglvFilterLineId; }) || tabItems[0] || null,
            scrollingEnabled: true,
            showNavButtons: true,
            itemTemplate: function (itemData) {
                return $("<span>").text(itemData ? itemData.Name : "");
            },
            onSelectionChanged: function (e) {
                var selected = e.addedItems && e.addedItems.length ? e.addedItems[0] : null;
                tglvFilterLineId = selected ? selected.Id : null;
                applyTGLVLineFilter("#grid_tglv_main");
            }
        });
        loadTGLVGrid("#grid_tglv_main", lineItems);

        // FIX: Buộc Grid phải tính toán lại kích thước sau khi Tab render
        setTimeout(function () {
            var grid = $("#grid_tglv_main").dxDataGrid("instance");
            if (grid) grid.updateDimensions();
        }, 150);
    }).fail(function () {
        DevExpress.ui.notify("Không tải được danh sách chuyền", "error", 2000);
        loadTGLVGrid("#grid_tglv_main", []);
    });
}

function loadHSLNFullTab() {
    var containerId = "#grid_2";
    if ($(containerId).length === 0) return;

    $(containerId).empty().append(`
        <div class="ns-panel ns-panel--tglv" style="margin-top: 5px; width: fit-content !important; max-width: 100%;">
            <div class="ns-panel__header">QUẢN LÝ HỆ SỐ LỢI NHUẬN</div>
            <div class="ns-panel__body">
                <div id="grid_hsln_main"></div>
            </div>
        </div>
    `);

    loadHeSoLoiNhuanGrid("#grid_hsln_main");
    fitPanelToGrid(containerId + " .ns-panel", "#grid_hsln_main");

    setTimeout(function () {
        var grid = $("#grid_hsln_main").dxDataGrid("instance");
        if (grid) grid.updateDimensions();
        fitPanelToGrid(containerId + " .ns-panel", "#grid_hsln_main");
    }, 150);
}
function loadSMV() {
    var containerId = "#grid_3";
    if ($(containerId).length === 0) return;

    // layout panel giống tab năng suất (cho đẹp)
    $(containerId).empty().append(`
        <div class="ns-wrap">
            <div class="ns-panel ns-panel--smv">
                <div class="ns-panel__body">
                    <div id="grid_smv"></div>
                </div>
            </div>
        </div>
    `);

    loadSMVGrid("#grid_smv");
}

function getNgayNghiThuVien(year, month) {
    return $.ajax({
        url: "/api/ThuVienWip/ngay-nghi",
        method: "GET",
        dataType: "json",
        data: {
            year: year,
            month: month || null
        }
    });
}

function postNgayNghiAction(payload) {
    return $.ajax({
        url: "/api/ThuVienWip/ngay-nghi",
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(payload || {}),
        processData: false
    }).then(function (res) {
        return res;
    }, function (xhr) {
        return $.Deferred().reject(new Error(getAjaxErrorMessage(xhr, "Có lỗi khi xử lý dữ liệu ngày nghỉ")));
    });
}

function getAjaxErrorMessage(xhr, fallbackMessage) {
    var fallback = fallbackMessage || "Có lỗi xảy ra";
    if (!xhr) return fallback;

    var respJson = xhr.responseJSON || null;
    if (respJson) {
        if (respJson.Message) return respJson.Message;
        if (respJson.message) return respJson.message;
        if (respJson.ExceptionMessage) return respJson.ExceptionMessage;
        if (respJson.error_description) return respJson.error_description;
    }

    var raw = xhr.responseText;
    if (raw) {
        try {
            var parsed = JSON.parse(raw);
            if (parsed) {
                if (parsed.Message) return parsed.Message;
                if (parsed.message) return parsed.message;
                if (parsed.ExceptionMessage) return parsed.ExceptionMessage;
            }
        } catch (e) { }
        if (typeof raw === "string" && raw.trim().length > 0 && raw.trim().charAt(0) !== "<") {
            return raw.trim();
        }
    }

    if (xhr.statusText) return xhr.statusText;
    return fallback;
}

function getNgayNghiFilterYear() {
    var yearEditor = $("#ngaynghi_year").dxSelectBox("instance");
    var yearValue = yearEditor ? Number(yearEditor.option("value")) : new Date().getFullYear();
    return yearValue > 0 ? yearValue : new Date().getFullYear();
}

function getNgayNghiFilterMonth() {
    var monthEditor = $("#ngaynghi_month").dxSelectBox("instance");
    var monthValue = monthEditor ? monthEditor.option("value") : null;
    if (monthValue == null || monthValue === "") return null;
    var parsed = Number(monthValue);
    return (parsed >= 1 && parsed <= 12) ? parsed : null;
}

function refreshNgayNghiGrid() {
    var grid = $("#grid_ngaynghi").dxDataGrid("instance");
    if (grid) grid.refresh();
}

function loadThuVienNgayNghiTab() {
    var containerId = "#grid_4";
    if ($(containerId).length === 0) return;

    var now = new Date();
    var currentYear = now.getFullYear();
    var yearData = [];
    for (var y = currentYear - 3; y <= currentYear + 3; y++) {
        yearData.push(y);
    }
    var monthData = [{ Value: null, Text: "Tất cả" }];
    for (var m = 1; m <= 12; m++) {
        monthData.push({ Value: m, Text: "Tháng " + m });
    }

    $(containerId).empty().append(`
        <div class="ns-panel ns-panel--tglv" style="margin-top: 5px; width: fit-content !important; max-width: 100%;">
            <div class="ns-panel__header">THƯ VIỆN NGÀY NGHỈ</div>
            <div class="ns-panel__body">
                <div class="day-off-container" style="justify-content: flex-start;">
                    <div class="day-off"><label>NĂM</label><div id="ngaynghi_year"></div></div>
                    <div class="day-off"><label>THÁNG</label><div id="ngaynghi_month"></div></div>
                </div>
                <div id="grid_ngaynghi"></div>
            </div>
        </div>
    `);

    $("#ngaynghi_year").dxSelectBox({
        dataSource: yearData,
        value: currentYear,
        searchEnabled: false,
        width: 140,
        onValueChanged: function () {
            refreshNgayNghiGrid();
            fitPanelToGrid(containerId + " .ns-panel", "#grid_ngaynghi");
        }
    });

    $("#ngaynghi_month").dxSelectBox({
        dataSource: monthData,
        valueExpr: "Value",
        displayExpr: "Text",
        value: null,
        searchEnabled: false,
        width: 160,
        onValueChanged: function () {
            refreshNgayNghiGrid();
            fitPanelToGrid(containerId + " .ns-panel", "#grid_ngaynghi");
        }
    });
    loadNgayNghiGrid("#grid_ngaynghi");
    fitPanelToGrid(containerId + " .ns-panel", "#grid_ngaynghi");
}

function loadNgayNghiGrid(gridId) {
    if ($(gridId).data("dxDataGrid")) {
        $(gridId).dxDataGrid("instance").dispose();
        $(gridId).empty();
    }

    var getGridData = function () {
        var grid = $(gridId).dxDataGrid("instance");
        return grid ? grid.getDataSource().items() : [];
    };

    var store = new DevExpress.data.CustomStore({
        key: "ID",
        load: function () {
            var year = getNgayNghiFilterYear();
            var month = getNgayNghiFilterMonth();
            return getNgayNghiThuVien(year, month);
        },
        insert: function (values) {
            values = values || {};
            return postNgayNghiAction({
                Action: "CREATE",
                ID_NgayNghi: values.ID_NgayNghi,
                NgayNghi: values.NgayNghi,
                GhiChu: values.GhiChu,
                Year: getNgayNghiFilterYear(),
                Month: getNgayNghiFilterMonth()
            });
        },
        update: function (key, values) {
            var currentItems = getGridData();
            var oldData = currentItems.find(function (item) { return item.ID === key; }) || {};
            var mergedData = $.extend({}, oldData, values);

            return postNgayNghiAction({
                Action: "UPDATE",
                ID: key,
                ID_NgayNghi: mergedData.ID_NgayNghi,
                NgayNghi: mergedData.NgayNghi,
                GhiChu: mergedData.GhiChu,
                Year: getNgayNghiFilterYear(),
                Month: getNgayNghiFilterMonth()
            });
        },
        remove: function (key) {
            return postNgayNghiAction({
                Action: "DELETE",
                ID: key,
                Year: getNgayNghiFilterYear(),
                Month: getNgayNghiFilterMonth()
            });
        },
        byKey: function (key) {
            var year = getNgayNghiFilterYear();
            var month = getNgayNghiFilterMonth();
            return getNgayNghiThuVien(year, month).then(function (data) {
                return (data || []).find(function (item) { return item.ID === key; });
            });
        }
    });

    $(gridId).dxDataGrid({
        dataSource: store,
        keyExpr: "ID",
        remoteOperations: false,
        showBorders: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,
        columnMinWidth: 110,
        columns: [
            { dataField: "ID", visible: false, allowEditing: false },
            {
                dataField: "STT",
                caption: "STT",
                dataType: "number",
                width: 100,
                alignment: "right",
                allowEditing: false,
            },
            {
                dataField: "NgayNghi",
                caption: "Ngày nghỉ",
                dataType: "date",
                width: 200,
                format: "dd/MM/yyyy",
                editorOptions: { type: "date", displayFormat: "dd/MM/yyyy" },
                validationRules: [{ type: "required", message: "Chọn Ngày nghỉ" }]
            },
            {
                dataField: "StrDayOfWeek",
                caption: "Thứ",
                allowEditing: false,
                width: 250
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                width: 250
            },
            {
                type: "buttons",
                width: 60,
                buttons: ["delete"]
            }
        ],
        editing: {
            mode: "batch",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },
        startEditAction: "click",
        toolbar: {
            items: [
                "addRowButton",
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Lưu",
                        icon: "save",
                        hint: "Lưu thay đổi",
                        width: 96,
                        stylingMode: "contained",
                        type: "success",
                        onClick: function () {
                            var grid = $(gridId).dxDataGrid("instance");
                            if (grid) {
                                grid.saveEditData();
                            }
                        }
                    }
                },
                {
                    name: "revertButton",
                    location: "after"
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Làm mới",
                        icon: "refresh",
                        stylingMode: "contained",
                        type: "default",
                        onClick: function () {
                            $(gridId).dxDataGrid("instance").refresh();
                        }
                    }
                }
            ]
        },
        paging: { pageSize: 20 },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [10, 20, 50]
        },
        onInitNewRow: function (e) {
            e.data.NgayNghi = new Date();
        },
        onSaved: function () {
            DevExpress.ui.notify("Đã lưu dữ liệu ngày nghỉ", "success", 1800);
        },
        onContentReady: function () {
            fitPanelToGrid("#grid_4 .ns-panel", gridId);
        },
        onDataErrorOccurred: function (e) {
            DevExpress.ui.notify(e.error?.message || "Có lỗi khi xử lý dữ liệu ngày nghỉ", "error", 2800);
        }
    });
}

function loadTGLVGrid(gridId, lineItems) {
    lineItems = lineItems || [];
    if ($(gridId).data("dxDataGrid")) {
        $(gridId).dxDataGrid("instance").dispose();
        $(gridId).empty();
    }
    $(gridId).addClass("grid--tglv-blue");

    const container = $(gridId).parent();
    container.find(".bulk-container").remove();
    // 1. Render bộ khai báo nhanh
    const $bulkHtml = $(`
        <div class="bulk-container">
            <div class="bulk-item"><label>CHUYỀN ÁP DỤNG</label><div id="tglv_line"></div></div>
            <div class="bulk-item"><label>TỪ NGÀY</label><div id="tglv_from"></div></div>
            <div class="bulk-item"><label>ĐẾN NGÀY</label><div id="tglv_to"></div></div>
            <div class="bulk-item"><label>GIỜ BẮT ĐẦU</label><div id="tglv_start"></div></div>
            <div class="bulk-item"><label>GIỜ KẾT THÚC</label><div id="tglv_end"></div></div>
            <div class="bulk-item"><label>GIỜ NGHỈ</label><div id="tglv_break"></div></div>
            <div class="bulk-item"><label>SỐ GIỜ TĂNG CA</label><div id="tglv_ot"></div></div>
            <div class="bulk-item"><label>TỔNG GIỜ LÀM VIỆC</label><div id="tglv_total"></div></div>
            <div id="tglv_btn_apply" style="flex: 0 0 auto;"></div>
        </div>
    `);
    container.prepend($bulkHtml);

    const defaultStart = new Date();
    defaultStart.setHours(8, 0, 0, 0);
    const defaultEnd = new Date();
    defaultEnd.setHours(18, 0, 0, 0);
    const defaultOff = 1;
    $("#tglv_line").dxSelectBox({
        dataSource: lineItems,
        valueExpr: "Id",
        displayExpr: "Name",
        value: tglvApplyLineId,
        searchEnabled: true,
        showClearButton: false,
        placeholder:"Chọn chuyền...",
        width: "100%",
        onValueChanged: function (e) {
            tglvApplyLineId = e.value || null;
        }
    });
    $("#tglv_from").dxDateBox({ value: new Date(), displayFormat: "dd/MM/yyyy", width: "100%" });
    $("#tglv_to").dxDateBox({ value: new Date(), displayFormat: "dd/MM/yyyy", width: "100%" });
    $("#tglv_start").dxDateBox({
        type: "time",
        value: defaultStart,
        displayFormat: "HH:mm",
        useMaskBehavior: true,
        width: "100%"
    });
    $("#tglv_end").dxDateBox({
        type: "time",
        value: defaultEnd,
        displayFormat: "HH:mm",
        useMaskBehavior: true,
        width: "100%"
    });
    $("#tglv_break").dxNumberBox({
        value: defaultOff,
        min: 0,
        step: 0.5,
        format: "#,##0.##",
        showSpinButtons: true,
        width: "100%"
    });
    $("#tglv_ot").dxNumberBox({
        value: 0,
        min: 0,
        step: 0.5,
        format: "#,##0.##",
        showSpinButtons: true,
        width: "100%"
    });
    $("#tglv_total").dxNumberBox({
        value: 0,
        readOnly: true,
        format: "#,##0.##",
        width: "100%"
    });

    const btnApply = $("#tglv_btn_apply").dxButton({
        text: "Khai báo",
        type: "default",
        icon: "event",
        stylingMode: "contained",
        onClick: function () {
            const maChuyen = $("#tglv_line").dxSelectBox("instance").option("value");
            if (!maChuyen) {
                DevExpress.ui.notify("Vui lòng chọn chuyền áp dụng", "warning", 1800);
                return;
            }
            tglvApplyLineId = maChuyen;
            const totalHours = $("#tglv_total").dxNumberBox("instance").option("value");
            const gioBatDau = formatTimeHHmm($("#tglv_start").dxDateBox("instance").option("value"));
            const gioKetThuc = formatTimeHHmm($("#tglv_end").dxDateBox("instance").option("value"));
            const gioNghi = Number($("#tglv_break").dxNumberBox("instance").option("value"));
            const soGioTangCa = Number($("#tglv_ot").dxNumberBox("instance").option("value"));
            const tuNgay = $("#tglv_from").dxDateBox("instance").option("value");
            const denNgay = $("#tglv_to").dxDateBox("instance").option("value");

            if (!tuNgay || !denNgay) {
                DevExpress.ui.notify("Vui lòng chọn từ ngày và đến ngày", "warning", 1800);
                return;
            }

            if (new Date(denNgay) < new Date(tuNgay)) {
                DevExpress.ui.notify("Đến ngày phải lớn hơn hoặc bằng Từ ngày", "warning", 2000);
                return;
            }

            const data = {
                Action: "InsertRange",
                MaChuyen: maChuyen,
                TuNgay: tuNgay,
                DenNgay: denNgay,
                SoGio: totalHours,
                GioBatDau: gioBatDau,
                GioKetThuc: gioKetThuc,
                GioNghi: isNaN(gioNghi) ? null : gioNghi,
                SoGioTangCa: isNaN(soGioTangCa) ? 0 : soGioTangCa,
                NguoiTao: getCurrentUser()
            };
            $.ajax({
                url: "/api/ThuVienWip/thoi-gian-lam-viec",
                method: "POST",
                data: JSON.stringify(data),
                contentType: "application/json",
                success: function () {
                    DevExpress.ui.notify("Đã cập thành công", "success", 2000);
                    $(gridId).dxDataGrid("instance").refresh();
                }
            });
        }
    }).dxButton("instance");

    function toMinutes(timeValue) {
        if (!timeValue) return null;

        if (timeValue instanceof Date && !isNaN(timeValue.getTime())) {
            return (timeValue.getHours() * 60) + timeValue.getMinutes();
        }

        if (typeof timeValue === "string") {
            const match = timeValue.match(/^([01]\d|2[0-3]):([0-5]\d)$/);
            if (!match) return null;
            return (parseInt(match[1], 10) * 60) + parseInt(match[2], 10);
        }

        return null;
    }

    function formatTimeHHmm(timeValue) {
        if (timeValue instanceof Date && !isNaN(timeValue.getTime())) {
            const hh = timeValue.getHours().toString().padStart(2, "0");
            const mm = timeValue.getMinutes().toString().padStart(2, "0");
            return hh + ":" + mm;
        }

        if (typeof timeValue === "string") {
            const match = timeValue.match(/^([01]\d|2[0-3]):([0-5]\d)$/);
            return match ? timeValue : null;
        }

        return null;
    }

    function calculateSoGioByTime(gioBatDau, gioKetThuc, gioNghi, soGioTangCa) {
        const startMinutes = toMinutes(gioBatDau);
        const endMinutesRaw = toMinutes(gioKetThuc);
        const breakHours = Number(gioNghi);
        const overtimeHours = Number(soGioTangCa);
        const validBreak = !isNaN(breakHours) && breakHours >= 0;
        const validOvertime = !isNaN(overtimeHours) && overtimeHours >= 0;

        if (startMinutes === null || endMinutesRaw === null || !validBreak || !validOvertime) return null;

        let endMinutes = endMinutesRaw;
        if (endMinutes < startMinutes) endMinutes += 24 * 60;

        const totalWorkingMinutes = (endMinutes - startMinutes) - (breakHours * 60);
        if (totalWorkingMinutes <= 0) return null;

        return Math.round((((totalWorkingMinutes / 60) + overtimeHours) * 100)) / 100;
    }

    function recalculateWorkingHours() {
        const startVal = $("#tglv_start").dxDateBox("instance").option("value");
        const endVal = $("#tglv_end").dxDateBox("instance").option("value");
        const breakHours = Number($("#tglv_break").dxNumberBox("instance").option("value"));
        const overtimeHours = Number($("#tglv_ot").dxNumberBox("instance").option("value"));
        const totalEditor = $("#tglv_total").dxNumberBox("instance");
        const totalHours = calculateSoGioByTime(startVal, endVal, breakHours, overtimeHours);
        const isInvalid = totalHours === null;

        totalEditor.option({
            value: isInvalid ? null : totalHours,
            isValid: !isInvalid
        });
        btnApply.option("disabled", isInvalid);
    }

    function recalcRowSoGio(newData, currentRowData) {
        const merged = $.extend({}, currentRowData || {}, newData || {});
        const calculated = calculateSoGioByTime(merged.GioBatDau, merged.GioKetThuc, merged.GioNghi, merged.SoGioTangCa);
        newData.SoGio = calculated;
    }

    function resolveLineName(lineId) {
        const found = (lineItems || []).find(function (x) { return Number(x.Id) === Number(lineId); });
        return found ? found.Name : (lineId || "");
    }

    function formatNgay(value) {
        if (!value) return "";
        const d = new Date(value);
        if (isNaN(d.getTime())) return "";
        return d.getDate().toString().padStart(2, "0") + "/" +
            (d.getMonth() + 1).toString().padStart(2, "0") + "/" +
            d.getFullYear();
    }

    function toLocalDateOnly(value) {
        if (!value) return null;
        const d = new Date(value);
        if (isNaN(d.getTime())) return null;
        return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    }

    function formatDateForApi(value) {
        const d = toLocalDateOnly(value);
        if (!d) return null;
        return d.getFullYear() + "-" +
            (d.getMonth() + 1).toString().padStart(2, "0") + "-" +
            d.getDate().toString().padStart(2, "0") + "T00:00:00";
    }

    function postTGLVWeekday(payload) {
        return $.ajax({
            url: "/api/ThuVienWip/thoi-gian-lam-viec/theo-thu",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload)
        });
    }

    function getAjaxErrorMessage(xhr, fallback) {
        return xhr && xhr.responseJSON && (xhr.responseJSON.ExceptionMessage || xhr.responseJSON.Message)
            ? (xhr.responseJSON.ExceptionMessage || xhr.responseJSON.Message)
            : fallback;
    }

    function getTGLVFilterButtonHint() {
        if (!hasTGLVDateFilter()) return "Lọc theo khoảng ngày";
        var fromText = tglvFilterFromDate ? formatNgay(tglvFilterFromDate) : "...";
        var toText = tglvFilterToDate ? formatNgay(tglvFilterToDate) : "...";
        return "Đang lọc: " + fromText + " - " + toText;
    }

    function updateTGLVFilterButtonState() {
        var grid = $(gridId).dxDataGrid("instance");
        if (!grid) return;
        var $button = $("#tglv_display_filter_btn");
        if (!$button.length) return;
        var button = $button.dxButton("instance");
        if (!button) return;
        $button.toggleClass("is-filter-active", hasTGLVDateFilter());
        button.option({
            type: hasTGLVDateFilter() ? "default" : "normal",
            hint: getTGLVFilterButtonHint()
        });
    }

    function openTGLVDisplayFilterPopover() {
        const popoverHostId = "tglv_display_filter_popover";
        let $host = $("#" + popoverHostId);
        if ($host.length) {
            const oldPopover = $host.dxPopover("instance");
            if (oldPopover) oldPopover.dispose();
            $host.remove();
        }

        let pendingFromDate = tglvFilterFromDate;
        let pendingToDate = tglvFilterToDate;
        $host = $("<div>").attr("id", popoverHostId).appendTo("body");

        function applyDisplayFilter() {
            var fromDate = toTGLVLocalDateOnly(pendingFromDate);
            var toDate = toTGLVLocalDateOnly(pendingToDate);
            if (fromDate && toDate && toDate < fromDate) {
                DevExpress.ui.notify("Đến ngày phải lớn hơn hoặc bằng Từ ngày", "warning", 2000);
                return;
            }

            tglvFilterFromDate = fromDate;
            tglvFilterToDate = toDate;
            updateTGLVFilterButtonState();
            applyTGLVLineFilter(gridId);

            var popover = $host.dxPopover("instance");
            if (popover) popover.hide();
        }

        function clearDisplayFilter() {
            pendingFromDate = null;
            pendingToDate = null;
            tglvFilterFromDate = null;
            tglvFilterToDate = null;

            var fromEditor = $("#tglv_display_filter_from").dxDateBox("instance");
            var toEditor = $("#tglv_display_filter_to").dxDateBox("instance");
            if (fromEditor) fromEditor.option("value", null);
            if (toEditor) toEditor.option("value", null);

            updateTGLVFilterButtonState();
            applyTGLVLineFilter(gridId);

            var popover = $host.dxPopover("instance");
            if (popover) popover.hide();
        }

        $host.dxPopover({
            target: "#tglv_display_filter_btn",
            showTitle: false,
            width: 360,
            hideOnOutsideClick: true,
            position: { my: "top", at: "bottom", of: "#tglv_display_filter_btn" },
            wrapperAttr: { class: "tglv-date-filter-popover" },
            contentTemplate: function (contentElement) {
                const $wrap = $("<div class='tglv-date-filter-content'></div>").appendTo(contentElement);
                $("<div class='tglv-date-filter-title'>Lọc theo khoảng ngày</div>").appendTo($wrap);
                const $row = $("<div class='tglv-date-filter-row'></div>").appendTo($wrap);
                const $from = $("<div id='tglv_display_filter_from'></div>").appendTo($row);
                $("<div class='tglv-date-filter-sep'>|</div>").appendTo($row);
                const $to = $("<div id='tglv_display_filter_to'></div>").appendTo($row);

                $from.dxDateBox({
                    value: pendingFromDate,
                    type: "date",
                    displayFormat: "dd/MM/yyyy",
                    placeholder: "Từ ngày",
                    showClearButton: true,
                    onValueChanged: function (e) {
                        pendingFromDate = e.value || null;
                    }
                });
                $to.dxDateBox({
                    value: pendingToDate,
                    type: "date",
                    displayFormat: "dd/MM/yyyy",
                    placeholder: "Đến ngày",
                    showClearButton: true,
                    onValueChanged: function (e) {
                        pendingToDate = e.value || null;
                    }
                });

                const $actions = $("<div class='tglv-date-filter-actions'></div>").appendTo($wrap);
                $("<div>").dxButton({
                    text: "Xóa",
                    type: "normal",
                    stylingMode: "outlined",
                    onClick: clearDisplayFilter
                }).appendTo($actions);
                $("<div>").dxButton({
                    text: "Lọc",
                    type: "default",
                    stylingMode: "contained",
                    onClick: applyDisplayFilter
                }).appendTo($actions);
            }
        }).dxPopover("instance").show();
    }

    function openWeekdaySettingPopup() {
        const popupHostId = "tglv_weekday_setting_popup";
        let $host = $("#" + popupHostId);
        if ($host.length) {
            const oldPopup = $host.dxPopup("instance");
            if (oldPopup) oldPopup.dispose();
            $host.remove();
        }
        $host = $("<div>").attr("id", popupHostId).appendTo("body");

        const allLineValue = "__ALL__";
        const weekdayItems = [
            { Id: 2, Name: "Thứ 2" },
            { Id: 3, Name: "Thứ 3" },
            { Id: 4, Name: "Thứ 4" },
            { Id: 5, Name: "Thứ 5" },
            { Id: 6, Name: "Thứ 6" },
            { Id: 7, Name: "Thứ 7" },
            { Id: 8, Name: "Chủ nhật" }
        ];
        const lineOptions = [{ Id: allLineValue, Name: "Tất cả chuyền" }].concat(lineItems || []);
        const popupStart = new Date();
        popupStart.setHours(8, 0, 0, 0);
        const popupEnd = new Date();
        popupEnd.setHours(18, 0, 0, 0);
        let popupApplyButton = null;

        function selectedLinesFromValue(value) {
            const raw = value || [];
            if (raw.indexOf(allLineValue) >= 0) {
                return null;
            }
            return raw.filter(function (x) { return Number(x) > 0; });
        }

        function recalculatePopupTotal() {
            const totalEditor = $("#tglv_weekday_total").dxNumberBox("instance");
            if (!totalEditor) return;

            const total = calculateSoGioByTime(
                $("#tglv_weekday_start").dxDateBox("instance").option("value"),
                $("#tglv_weekday_end").dxDateBox("instance").option("value"),
                $("#tglv_weekday_break").dxNumberBox("instance").option("value"),
                $("#tglv_weekday_ot").dxNumberBox("instance").option("value")
            );
            const isInvalid = total === null;
            totalEditor.option({
                value: isInvalid ? null : total,
                isValid: !isInvalid
            });
            if (popupApplyButton) popupApplyButton.option("disabled", isInvalid);
        }

        function applyWeekdaySetting() {
            const tagBox = $("#tglv_weekday_lines").dxTagBox("instance");
            const selectedLineIds = selectedLinesFromValue(tagBox.option("value"));
            const weekday = $("#tglv_weekday_day").dxSelectBox("instance").option("value");
            const tuNgay = $("#tglv_weekday_from").dxDateBox("instance").option("value");
            const denNgay = $("#tglv_weekday_to").dxDateBox("instance").option("value");
            const gioBatDau = formatTimeHHmm($("#tglv_weekday_start").dxDateBox("instance").option("value"));
            const gioKetThuc = formatTimeHHmm($("#tglv_weekday_end").dxDateBox("instance").option("value"));
            const gioNghi = Number($("#tglv_weekday_break").dxNumberBox("instance").option("value"));
            const soGioTangCa = Number($("#tglv_weekday_ot").dxNumberBox("instance").option("value"));
            const isAllLines = selectedLineIds === null;
            const lineCount = isAllLines ? (lineItems || []).length : selectedLineIds.length;

            if (!isAllLines && !selectedLineIds.length) {
                DevExpress.ui.notify("Vui lòng chọn chuyền áp dụng", "warning", 1800);
                return;
            }
            if (!tuNgay || !denNgay) {
                DevExpress.ui.notify("Vui lòng chọn từ ngày và đến ngày", "warning", 1800);
                return;
            }
            if (toLocalDateOnly(denNgay) < toLocalDateOnly(tuNgay)) {
                DevExpress.ui.notify("Đến ngày phải lớn hơn hoặc bằng Từ ngày", "warning", 2000);
                return;
            }

            const weekdayName = (weekdayItems.find(function (x) { return x.Id === weekday; }) || {}).Name || "";
            const confirmMsg = "Cập nhật " + weekdayName + " từ " + formatNgay(tuNgay) + " đến " + formatNgay(denNgay) +
                " cho " + (isAllLines ? "tất cả chuyền" : (lineCount + " chuyền")) + "?";

            DevExpress.ui.dialog.confirm(confirmMsg, "Xác nhận cài đặt lịch").done(function (ok) {
                if (!ok) return;

                const popup = $host.dxPopup("instance");
                popupApplyButton.option("disabled", true);
                if (popup) popup.option("title", "Đang cập nhật lịch làm việc...");

                postTGLVWeekday({
                    Thu: weekday,
                    TuNgay: formatDateForApi(tuNgay),
                    DenNgay: formatDateForApi(denNgay),
                    GioBatDau: gioBatDau,
                    GioKetThuc: gioKetThuc,
                    GioNghi: isNaN(gioNghi) ? null : gioNghi,
                    SoGioTangCa: isNaN(soGioTangCa) ? 0 : soGioTangCa,
                    MaChuyenIds: isAllLines ? null : selectedLineIds,
                    ModifiedBy: getCurrentUser()
                }).done(function (res) {
                    const updatedRows = res && Number.isFinite(Number(res.updatedRows)) ? Number(res.updatedRows) : 0;
                    DevExpress.ui.notify("Đã cài đặt lịch làm việc (" + updatedRows + " dòng)", "success", 2200);
                    if (popup) popup.hide();
                    const grid = $(gridId).dxDataGrid("instance");
                    if (grid) grid.refresh();
                }).fail(function (xhr) {
                    DevExpress.ui.notify(getAjaxErrorMessage(xhr, "Có lỗi khi cài đặt lịch làm việc"), "error", 3500);
                    if (popup) popup.option("title", "Cài đặt lịch làm việc theo thứ");
                    popupApplyButton.option("disabled", false);
                });
            });
        }

        $host.dxPopup({
            title: "Cài đặt lịch làm việc theo thứ",
            width: 760,
            maxWidth: "96vw",
            height: "auto",
            maxHeight: "92vh",
            visible: true,
            showCloseButton: true,
            closeOnOutsideClick: true,
            wrapperAttr: { class: "tglv-weekday-setting-popup" },
            contentTemplate: function (contentElement) {
                const $wrap = $("<div>").css({
                    display: "grid",
                    gridTemplateColumns: "1fr 1fr",
                    gap: "12px 14px",
                    padding: "4px"
                }).appendTo(contentElement);

                function addEditor(label, id, wide) {
                    const $item = $("<div>").css("minWidth", "0").appendTo($wrap);
                    if (wide) $item.css("gridColumn", "1 / -1");
                    $("<label>").text(label).css({
                        display: "block",
                        fontWeight: 600,
                        marginBottom: "5px"
                    }).appendTo($item);
                    return $("<div>").attr("id", id).appendTo($item);
                }

                addEditor("Chuyền áp dụng", "tglv_weekday_lines", true);
                addEditor("Thứ", "tglv_weekday_day", false);
                addEditor("Tổng giờ làm việc", "tglv_weekday_total", false);
                addEditor("Từ ngày", "tglv_weekday_from", false);
                addEditor("Đến ngày", "tglv_weekday_to", false);
                addEditor("Giờ bắt đầu", "tglv_weekday_start", false);
                addEditor("Giờ kết thúc", "tglv_weekday_end", false);
                addEditor("Giờ nghỉ", "tglv_weekday_break", false);
                addEditor("Số giờ tăng ca", "tglv_weekday_ot", false);

                $("#tglv_weekday_lines").dxTagBox({
                    dataSource: lineOptions,
                    valueExpr: "Id",
                    displayExpr: "Name",
                    value: [allLineValue],
                    searchEnabled: true,
                    showSelectionControls: true,
                    applyValueMode: "useButtons",
                    multiline: false,
                    placeholder: "Chọn chuyền...",
                    onValueChanged: function (e) {
                        const value = e.value || [];
                        const previous = e.previousValue || [];
                        const hasAll = value.indexOf(allLineValue) >= 0;
                        const addedAll = hasAll && previous.indexOf(allLineValue) < 0;

                        if (addedAll && value.length > 1) {
                            e.component.option("value", [allLineValue]);
                        } else if (hasAll && value.length > 1) {
                            e.component.option("value", value.filter(function (x) { return x !== allLineValue; }));
                        }
                    }
                });
                $("#tglv_weekday_day").dxSelectBox({
                    dataSource: weekdayItems,
                    valueExpr: "Id",
                    displayExpr: "Name",
                    value: 7,
                    searchEnabled: false
                });
                $("#tglv_weekday_from").dxDateBox({ value: new Date(), displayFormat: "dd/MM/yyyy", width: "100%" });
                $("#tglv_weekday_to").dxDateBox({ value: new Date(), displayFormat: "dd/MM/yyyy", width: "100%" });
                $("#tglv_weekday_start").dxDateBox({
                    type: "time",
                    value: popupStart,
                    displayFormat: "HH:mm",
                    useMaskBehavior: true,
                    width: "100%"
                });
                $("#tglv_weekday_end").dxDateBox({
                    type: "time",
                    value: popupEnd,
                    displayFormat: "HH:mm",
                    useMaskBehavior: true,
                    width: "100%"
                });
                $("#tglv_weekday_break").dxNumberBox({
                    value: 1,
                    min: 0,
                    step: 0.5,
                    format: "#,##0.##",
                    showSpinButtons: true,
                    width: "100%"
                });
                $("#tglv_weekday_ot").dxNumberBox({
                    value: 0,
                    min: 0,
                    step: 0.5,
                    format: "#,##0.##",
                    showSpinButtons: true,
                    width: "100%"
                });
                $("#tglv_weekday_total").dxNumberBox({
                    value: 0,
                    readOnly: true,
                    format: "#,##0.##",
                    width: "100%"
                });

                ["#tglv_weekday_start", "#tglv_weekday_end"].forEach(function (selector) {
                    $(selector).dxDateBox("instance").on("valueChanged", recalculatePopupTotal);
                });
                ["#tglv_weekday_break", "#tglv_weekday_ot"].forEach(function (selector) {
                    $(selector).dxNumberBox("instance").on("valueChanged", recalculatePopupTotal);
                });
            },
            toolbarItems: [
                {
                    widget: "dxButton",
                    toolbar: "bottom",
                    location: "after",
                    options: {
                        text: "Lưu",
                        icon: "check",
                        type: "success",
                        stylingMode: "contained",
                        onInitialized: function (e) {
                            popupApplyButton = e.component;
                        },
                        onClick: applyWeekdaySetting
                    }
                }
            ],
            onShown: function () {
                recalculatePopupTotal();
            },
            onHidden: function (e) {
                e.component.option("title", "Cài đặt lịch làm việc theo thứ");
            }
        }).dxPopup("instance").show();
    }

    $("#tglv_start").dxDateBox("instance").on("valueChanged", recalculateWorkingHours);
    $("#tglv_end").dxDateBox("instance").on("valueChanged", recalculateWorkingHours);
    $("#tglv_break").dxNumberBox("instance").on("valueChanged", recalculateWorkingHours);
    $("#tglv_ot").dxNumberBox("instance").on("valueChanged", recalculateWorkingHours);
    recalculateWorkingHours();
    var tglvPageIndex = 0;
    var tglvPageSize = 50;
    // 2. CustomStore tối ưu cho logic mới
    const store = new DevExpress.data.CustomStore({
        key: "ID",
        load: function (loadOptions) {
            var skip = (loadOptions && Number.isFinite(loadOptions.skip)) ? loadOptions.skip : 0;
            var take = (loadOptions && Number.isFinite(loadOptions.take)) ? loadOptions.take : 40;
            var maChuyen = tglvFilterLineId || null;
            return $.ajax({
                url: "/api/ThuVienWip/thoi-gian-lam-viec",
                method: "GET",
                dataType: "json",
                data: {
                    maChuyen: maChuyen,
                    tuNgay: formatTGLVDateForApi(tglvFilterFromDate),
                    denNgay: formatTGLVDateForApi(tglvFilterToDate),
                    skip: skip,
                    take: take,
                    pageSize: take
                }
            }).then(function (res) {
                if (res && Array.isArray(res.data)) {
                    return {
                        data: res.data,
                        totalCount: Number(res.totalCount) || 0
                    };
                }
                var items = Array.isArray(res) ? res : [];
                return { data: items, totalCount: items.length };
            });
        },
        // Update lẻ từng ô trên grid
        update: function (key, values) {
            // Lấy dữ liệu cũ của dòng để có NgayApDung
            return store.byKey(key).then(oldData => {
                if (!oldData) throw new Error("Không tìm thấy dữ liệu dòng.");

                const mergedData = $.extend({}, oldData, values);
                const recalculatedSoGio = calculateSoGioByTime(
                    mergedData.GioBatDau,
                    mergedData.GioKetThuc,
                    mergedData.GioNghi,
                    mergedData.SoGioTangCa
                );

                if (recalculatedSoGio === null) {
                    return $.Deferred().reject(new Error("Giờ làm việc không hợp lệ. Vui lòng kiểm tra giờ bắt đầu/kết thúc/giờ nghỉ."));
                }

                return $.ajax({
                    url: "/api/ThuVienWip/thoi-gian-lam-viec",
                    method: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        Action: "Update",
                        ID: key,
                        MaChuyen: mergedData.MaChuyen,
                        SoGio: recalculatedSoGio,
                        GioBatDau: mergedData.GioBatDau,
                        GioKetThuc: mergedData.GioKetThuc,
                        GioNghi: mergedData.GioNghi,
                        SoGioTangCa: mergedData.SoGioTangCa,
                        NgayApDung: mergedData.NgayApDung,
                        ModifiedBy: getCurrentUser()
                    })
                });
            });
        },
        remove: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/thoi-gian-lam-viec",
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify({ Action: "Delete", ID: key, ModifiedBy: getCurrentUser() })
            });
        },
        byKey: function (key) {
            return $.getJSON("/api/ThuVienWip/thoi-gian-lam-viec").then(data => {
                return (data || []).find(x => x.ID === key);
            });
        },
    });

    // 3. Cấu hình Grid
    $(gridId).dxDataGrid({
        dataSource: store,
        remoteOperations: true,
        showBorders: true,
        width: "100%",
        rowAlternationEnabled: true,
        editing: {
            mode: "batch",
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },
        paging: { pageSize: 150 },      // ← pageSize mặc định
        pager: {
            visible: true,
            showPageSizeSelector: true,
            showInfo: true,
            allowedPageSizes: [50, 100, 150, 300, 500],  // ← phải có 10 khớp pageSize mặc định
            showNavigationButtons: true
        },
        onOptionChanged: function (e) {
            if (e.fullName === "paging.pageIndex") {
                tglvPageIndex = e.value;
                console.log("pageIndex changed:", tglvPageIndex);
            }
            if (e.fullName === "paging.pageSize") {
                tglvPageSize = e.value;
                tglvPageIndex = 0; // reset về trang 1
                console.log("pageSize changed:", tglvPageSize);
            }
        },
        grouping: {
            autoExpandAll: true, contextMenuEnabled: true,
            expandMode: "rowClick"
        },

        groupPanel: { visible: false },
        columns: [
            {
                dataField: "MaChuyen",
                caption: "Chuyền",
                alignment: "center",
                width: 180,
                //groupIndex: 0,
                lookup: {
                    dataSource: lineItems,
                    valueExpr: "Id",
                    displayExpr: "Name"
                },
                groupCellTemplate: function (container, options) {
                    var lineName = resolveLineName(options.value);
                    $("<span>").text("Chuyền: " + lineName).appendTo(container);
                },
                validationRules: [{
                    type: "required",
                    message: "Vui lòng chọn chuyền"
                }]
            },
            //{
            //    dataField: "NgayApDung",
            //    caption: "Ngày",
            //    dataType: "date",
            //    format: "dd/MM/yyyy",
            //    allowEditing: false, // Khóa ngày
            //    sortOrder: "desc",
            //    width: 130
            //},
            //{
            //    dataField: "NgayApDung",
            //    caption: "Tháng",
            //    groupIndex: 0, // Tự động nhóm theo tháng
            //    calculateGroupValue: function (data) {
            //        const d = new Date(data.NgayApDung);
            //        return "Tháng " + (d.getMonth() + 1) + "/" + d.getFullYear();
            //    }
            //},
            {
                caption: "Thời gian", // Đổi tên cho bao quát
                allowEditing: false,
                sortOrder: "asc",
                dataField: "NgayApDung", // Gán field để sorting vẫn chạy đúng
                calculateCellValue: function (rowData) {
                    return rowData.NgayApDung;
                },
                cellTemplate: function (container, options) {
                    if (!options.value) return;

                    const d = new Date(options.value);
                    const days = ['Chủ Nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
                    const dayName = days[d.getDay()];

                    // Format ngày: dd/MM/yyyy
                    const datePart = d.getDate().toString().padStart(2, '0') + '/' +
                        (d.getMonth() + 1).toString().padStart(2, '0') + '/' +
                        d.getFullYear();

                    const displayValue = dayName + " - " + datePart;
                    const $span = $("<span>").text(displayValue);

                    // Màu sắc nhận diện
                    if (d.getDay() === 6) { // Thứ 7 màu xanh
                        $span.css({ "color": "#2563eb", "font-weight": "bold" });
                    } else if (d.getDay() === 0) { // Chủ nhật màu đỏ
                        $span.css({ "color": "#dc2626", "font-weight": "bold" });
                    }

                    $span.appendTo(container);
                }
            },
            {
                dataField: "GioBatDau",
                caption: "Giờ bắt đầu",
                alignment: "center",
                editorOptions: {
                    mask: "00:00",
                    useMaskedValue: true
                },
                validationRules: [
                    {
                        type: "pattern",
                        pattern: /^([01]\d|2[0-3]):([0-5]\d)$/,
                        message: "Định dạng giờ bắt đầu phải là HH:mm"
                    }
                ],
                setCellValue: function (newData, value, currentRowData) {
                    newData.GioBatDau = value;
                    recalcRowSoGio(newData, currentRowData);
                }
            },
            {
                dataField: "GioKetThuc",
                caption: "Giờ kết thúc",
                alignment: "center",
                editorOptions: {
                    mask: "00:00",
                    useMaskedValue: true
                },
                validationRules: [
                    {
                        type: "pattern",
                        pattern: /^([01]\d|2[0-3]):([0-5]\d)$/,
                        message: "Định dạng giờ kết thúc phải là HH:mm"
                    }
                ],
                setCellValue: function (newData, value, currentRowData) {
                    newData.GioKetThuc = value;
                    recalcRowSoGio(newData, currentRowData);
                }
            },
            {
                dataField: "GioNghi",
                caption: "Giờ nghỉ",
                dataType: "number",
                alignment: "center",
                format: "#,##0.##",
                editorOptions: {
                    min: 0,
                    showSpinButtons: true,
                    step: 0.5
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.GioNghi = value;
                    recalcRowSoGio(newData, currentRowData);
                }
            },
            {
                dataField: "SoGioTangCa",
                caption: "Số giờ tăng ca",
                dataType: "number",
                alignment: "center",
                format: "#,##0.##",
                editorOptions: {
                    min: 0,
                    showSpinButtons: true,
                    step: 0.5
                },
                setCellValue: function (newData, value, currentRowData) {
                    newData.SoGioTangCa = value;
                    recalcRowSoGio(newData, currentRowData);
                }
            },
            {
                dataField: "SoGio",
                caption: "Số giờ làm việc(ngày)",
                dataType: "number",
                alignment: "center",
                cssClass: "font-weight-bold text-primary",
                allowEditing: false,
                editorOptions: {
                    min: 0, max: 24, showSpinButtons: true
                }
            },
            {
                type: "buttons",
                width: 92,
                buttons: [
                    {
                        hint: "Đồng bộ",
                        icon: "repeat",
                        onClick: function (e) {
                            const row = e.row && e.row.data ? e.row.data : null;
                            if (!row) return;
                            if (!row.ID) {
                                DevExpress.ui.notify("Vui lòng lưu dòng nguồn trước khi đồng bộ", "warning", 1800);
                                return;
                            }

                            const lineName = resolveLineName(row.MaChuyen);
                            const ngay = formatNgay(row.NgayApDung);
                            const message = "Đồng bộ lịch ngày " + ngay +
                                " từ chuyền " + lineName +
                                " sang các chuyền khác?";

                            DevExpress.ui.dialog.confirm(message, "Xác nhận đồng bộ").done(function (ok) {
                                if (!ok) return;
                                $.ajax({
                                    url: "/api/ThuVienWip/thoi-gian-lam-viec",
                                    method: "POST",
                                    contentType: "application/json",
                                    data: JSON.stringify({
                                        Action: "SyncFromRow",
                                        ID: row.ID,
                                        MaChuyen: row.MaChuyen,
                                        NgayApDung: row.NgayApDung,
                                        SoGio: row.SoGio,
                                        GioBatDau: row.GioBatDau,
                                        GioKetThuc: row.GioKetThuc,
                                        GioNghi: row.GioNghi,
                                        SoGioTangCa: row.SoGioTangCa,
                                        NguoiTao: getCurrentUser()
                                    }),
                                    success: function () {
                                        DevExpress.ui.notify("Đồng bộ thành công", "success", 2000);
                                        $(gridId).dxDataGrid("instance").refresh();
                                    },
                                    error: function (xhr) {
                                        const msg = xhr && xhr.responseJSON && (xhr.responseJSON.ExceptionMessage || xhr.responseJSON.Message)
                                            ? (xhr.responseJSON.ExceptionMessage || xhr.responseJSON.Message)
                                            : "Có lỗi khi đồng bộ dữ liệu";
                                        DevExpress.ui.notify(msg, "error", 3000);
                                    }
                                });
                            });
                        }
                    },
                    "delete"
                ]
            }
        ],
        // Loại bỏ nút addRow mặc định vì đã có Bulk khai báo
        toolbar: {
            items: [
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "filter",
                        hint: getTGLVFilterButtonHint(),
                        stylingMode: "contained",
                        type: hasTGLVDateFilter() ? "default" : "normal",
                        width: 38,
                        elementAttr: {
                            id: "tglv_display_filter_btn",
                            class: "tglv-display-filter-button"
                        },
                        onClick: openTGLVDisplayFilterPopover
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "preferences",
                        hint: "Cài đặt lịch làm việc theo thứ",
                        stylingMode: "contained",
                        type: "default",
                        onClick: openWeekdaySettingPopup
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Lưu",
                        icon: "save",
                        hint: "Lưu thay đổi",
                        width: 96,
                        stylingMode: "contained",
                        type: "success",
                        onClick: function () {
                            var grid = $(gridId).dxDataGrid("instance");
                            if (grid) {
                                grid.saveEditData();
                            }
                        }
                    }
                },
                {
                    name: "revertButton",
                    location: "after"
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Làm mới",
                        icon: "refresh",
                        onClick: function () { $(gridId).dxDataGrid("instance").refresh(); }
                    }
                }
            ]
        },
        onRowValidating: function (e) {
            const mergedData = $.extend({}, e.oldData || {}, e.newData || {});
            const recalculatedSoGio = calculateSoGioByTime(
                mergedData.GioBatDau,
                mergedData.GioKetThuc,
                mergedData.GioNghi,
                mergedData.SoGioTangCa
            );

            if (recalculatedSoGio === null) {
                e.isValid = false;
                e.errorText = "Giờ làm việc không hợp lệ. Kiểm tra giờ bắt đầu/kết thúc/giờ nghỉ.";
            }
        },
        onSaved: function (e) {
            DevExpress.ui.notify("Đã lưu thay đổi!", "success", 2000);
        },
        onContentReady: function () {
            updateTGLVFilterButtonState();
            applyTGLVLineFilter(gridId);
        },
        onDataErrorOccurred: function (e) {
            DevExpress.ui.notify(
                e.error?.message || "Có lỗi khi lưu dữ liệu",
                "error",
                3000
            );
        }
    });
}
function loadHeSoLoiNhuanGrid(gridId) {
    var getGridData = function () {
        var grid = $(gridId).dxDataGrid("instance");
        return grid ? grid.getDataSource().items() : [];
    };
    if ($(gridId).data("dxDataGrid")) {
        $(gridId).dxDataGrid("instance").dispose();
        $(gridId).empty();
    }
    $(gridId).addClass("grid--tglv-blue");
    const store = new DevExpress.data.CustomStore({
        key: "ID",
        load: function () {
            return $.ajax({
                url: "/api/ThuVienWip/he-so-loi-nhuan",
                method: "GET",
                dataType: "json"
            });
        },
        insert: function (values) {
            values = values || {};
            return $.ajax({
                url: "/api/ThuVienWip/he-so-loi-nhuan",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    Action: "Insert",
                    HeSo: values.HeSo,
                    NgayApDung: values.NgayApDung,
                    NguoiTao: values.NguoiTao || getCurrentUser()
                }),
                processData: false
            });
        },
        update: function (key, values) {
            var currentItems = getGridData();
            var oldData = currentItems.find(function (item) { return item.ID === key; });
            if (!oldData) oldData = {};
            var mergedData = $.extend({}, oldData, values);
            return $.ajax({
                url: "/api/ThuVienWip/he-so-loi-nhuan",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    Action: "Update",
                    ID: key,
                    HeSo: mergedData.HeSo,
                    NgayApDung: mergedData.NgayApDung,
                    ModifiedBy: getCurrentUser()
                }),
                processData: false
            });
        },
        remove: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/he-so-loi-nhuan",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    Action: "Delete",
                    ID: key,
                    ModifiedBy: getCurrentUser()
                }),
                processData: false
            });
        },
        byKey: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/he-so-loi-nhuan",
                method: "GET",
                dataType: "json"
            }).then(function (data) {
                return data.find(i => i.ID === key);
            });
        }
    });

    $(gridId).dxDataGrid({
        dataSource: store,
        remoteOperations: false,
        columnAutoWidth: true,
        columnMinWidth: 110,
        columns: [
            { dataField: "ID", caption: "ID", visible: false, allowEditing: false },

            {
                dataField: "HeSo",
                caption: "Hệ số",
                dataType: "number",
                width: 200,
                alignment: "right",
                format: "#,##0.####",
                editorOptions: { min: 0, showSpinButtons: true, step: 0.01 },
                validationRules: [
                    { type: "required", message: "Vui lòng nhập hệ số" },
                    { type: "range", min: 0.0001, message: "Hệ số phải > 0" }
                ]
            },
            {
                dataField: "NgayApDung",
                caption: "Ngày áp dụng",
                dataType: "date",
                width: 250,
                format: "dd/MM/yyyy",
                editorOptions: { type: "date", displayFormat: "dd/MM/yyyy" },
                validationRules: [{ type: "required", message: "Vui lòng chọn ngày áp dụng" }]
            },
            {
                type: "buttons",
                width: 70,
                buttons: ["delete"]
            }
            //{ dataField: "TenNguoiTao", caption: "Người tạo", allowEditing: false },
            //{ dataField: "NgayTao", caption: "Ngày tạo", dataType: "datetime", format: "dd/MM/yyyy HH:mm", allowEditing: false }
        ],
        editing: {
            mode: "batch",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },
        toolbar: {
            items: [
                "addRowButton",
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Lưu",
                        icon: "save",
                        hint: "Lưu thay đổi",
                        width: 96,
                        stylingMode: "contained",
                        type: "success",
                        onClick: function () {
                            var grid = $(gridId).dxDataGrid("instance");
                            if (grid) {
                                grid.saveEditData();
                            }
                        }
                    }
                },
                {
                    name: "revertButton",
                    location: "after"
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Làm mới",
                        type: "default",
                        stylingMode: "contained",
                        icon: "refresh",
                        onClick: function () { $(gridId).dxDataGrid("instance").refresh(); }
                    }
                }
            ]
        },
        headerFilter: { visible: true },
        filterRow: { visible: true },
        paging: { pageSize: 20 },
        pager: { showPageSizeSelector: true, allowedPageSizes: [10, 20, 50] },
        showBorders: true,
        rowAlternationEnabled: true,
        onInitNewRow: function (e) {
            e.data.NgayApDung = new Date();
            e.data.NguoiTao = getCurrentUser();
        },
        onSaved: function () {
            DevExpress.ui.notify("Đã lưu thay đổi hệ số!", "success", 2000);
        },
        onContentReady: function () {
            fitPanelToGrid("#grid_2 .ns-panel", gridId);
        },
        onDataErrorOccurred: function (e) {
            DevExpress.ui.notify(
                e.error?.message || "Có lỗi khi lưu dữ liệu hệ số",
                "error",
                3000
            );
        }
    });
}
function loadSMVGrid(gridId) {
    // 1. Helper lấy dữ liệu client-side để tối ưu update
    var getGridData = function () {
        var grid = $(gridId).dxDataGrid("instance");
        return grid ? grid.getDataSource().items() : [];
    };

    if ($(gridId).data("dxDataGrid")) {
        $(gridId).dxDataGrid("instance").dispose();
        $(gridId).empty();
    }
    $(gridId).addClass("grid--tglv-blue");

    // ========== Lookup store (Remote Search - Giữ nguyên vì dữ liệu lớn) ==========
    const hangHoaStore = new DevExpress.data.CustomStore({
        key: "MaHang",
        load: function (loadOptions) {
            const keyword = loadOptions.searchValue ?? "";
            return $.ajax({
                url: "/api/ThuVienWip/smv-hanghoa",
                method: "GET",
                dataType: "json",
                data: { keyword }
            });
        },
        byKey: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/smv-hanghoa",
                method: "GET",
                dataType: "json",
                data: { keyword: key }
            }).then(arr => (arr || []).find(x => x.MaHang === key) || null);
        }
    });

    // ========== Main store ==========
    const store = new DevExpress.data.CustomStore({
        key: "ID",
        load: function () {
            return $.ajax({
                url: "/api/ThuVienWip/smv",
                method: "GET",
                dataType: "json",
                data: { user: getCurrentUser(), doSync: true }
            }).then(res => (res || []).map(x => ({
                ...x,
                ID: x.ID != null ? Number(x.ID) : null
            })));
        },

        insert: function (values) {
            values = values || {};
            return $.ajax({
                url: "/api/ThuVienWip/smv",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({
                    Action: "Insert",
                    MaHang: values.MaHang,
                    NgayApDung: values.NgayApDung,
                    SMV: values.SMV,
                    SMV_May: values.SMV_May,
                    SMV_Cat: values.SMV_Cat,
                    SMV_Laptrinh: values.SMV_Laptrinh,
                    SMV_HoanThanh: values.SMV_HoanThanh,
                    In_ep: values.In_ep,
                    CM: values.CM,
                    InTheuCT: values.InTheuCT,
                    HutAm: values.HutAm,
                    DoKim: values.DoKim,
                    NguoiTao: getCurrentUser(),
                    UpdatedBy: getCurrentUser()
                }),
                processData: false
            });
        },

        // --- TỐI ƯU HÀM UPDATE CHO CELL EDITING ---
        update: function (key, values) {
            // Lấy data cũ từ lưới (không gọi API)
            var currentItems = getGridData();
            var oldData = currentItems.find(function (item) { return item.ID === key; }) || {};

            // Gộp data mới vào data cũ
            var mergedData = $.extend({}, oldData, values);

            // Tạo payload đầy đủ (hoặc tùy biến theo API)
            // Việc gửi full mergedData an toàn hơn khi sửa từng ô
            const payload = {
                Action: "Update",
                ID: key,
                UpdatedBy: getCurrentUser(),
                // Luôn gửi các trường giá trị, lấy từ mergedData
                MaHang: mergedData.MaHang,
                NgayApDung: mergedData.NgayApDung,
                SMV: mergedData.SMV,
                SMV_May: mergedData.SMV_May,
                SMV_Cat: mergedData.SMV_Cat,
                SMV_Laptrinh: mergedData.SMV_Laptrinh,
                SMV_HoanThanh: mergedData.SMV_HoanThanh,
                In_ep: mergedData.In_ep,
                CM: mergedData.CM,
                InTheuCT: mergedData.InTheuCT,
                HutAm: mergedData.HutAm,
                DoKim: mergedData.DoKim
            };

            return $.ajax({
                url: "/api/ThuVienWip/smv",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(payload),
                processData: false
            });
        },

        remove: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/smv",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({
                    Action: "Delete",
                    ID: key,
                    UpdatedBy: getCurrentUser()
                }),
                processData: false
            });
        },
        // byKey giữ nguyên để Grid dùng nội bộ nếu cần
        byKey: function (key) {
            return $.ajax({
                url: "/api/ThuVienWip/smv",
                method: "GET",
                dataType: "json",
                data: { user: getCurrentUser(), doSync: false } // Sync false cho nhẹ
            }).then(res => res.find(x => x.ID == key));
        }
    });

    // ========== Grid ==========
    $(gridId).dxDataGrid({
        dataSource: store,
        remoteOperations: false,
        showBorders: true,
        rowAlternationEnabled: true,
        keyExpr: "ID",
        height: "80vh",
        scrolling: {
            mode: "standard",
        },
        columns: [
            { dataField: "ID", visible: false, allowEditing: false },
            {
                dataField: "MaHang",
                caption: "Tên hàng",
                validationRules: [{ type: "required", message: "Chọn Tên hàng" }],
                lookup: {
                    dataSource: hangHoaStore,
                    valueExpr: "MaHang",
                    displayExpr: "TenHang"
                },
                editorOptions: {
                    searchEnabled: true,
                    searchMode: "contains",
                    minSearchLength: 0,
                    showDataBeforeSearch: true,
                    showClearButton: true
                },
                setCellValue: function (rowData, value) {
                    rowData.MaHang = value;

                    // ✅ set TenHang ngay nếu có selectedItem
                    // (lookup editor là dxSelectBox, nhưng setCellValue không có selectedItem,
                    // nên vẫn có thể fallback byKey để chắc chắn)
                    if (!value) {
                        rowData.TenHang = null;
                        rowData.In_ep = null;
                        rowData.InTheuCT = null;
                        rowData.HutAm = null;
                        rowData.DoKim = null;
                        return;
                    }

                    hangHoaStore.byKey(value)
                        .done(function (item) {
                            rowData.TenHang = item ? item.TenHang : null;
                            rowData.In_ep = item ? item.InTheu : null;
                            rowData.InTheuCT = item ? item.InTheuCT : null;
                            rowData.HutAm = item ? item.HutAm : null;
                            rowData.DoKim = item ? item.DoKim : null;
                        })
                        .fail(function () {
                            rowData.TenHang = null;
                            rowData.In_ep = null;
                            rowData.InTheuCT = null;
                            rowData.HutAm = null;
                            rowData.DoKim = null;
                        });
                },
                calculateDisplayValue: function (row) {
                    return row?.TenHang || "";
                }
            },
            { dataField: "TenHang", visible: false },
            { dataField: "TenCL", caption: "Tên chủng loại", allowEditing: false },
            //{
            //    dataField: "MaHang",
            //    caption: "Mã hàng",
            //    validationRules: [{ type: "required", message: "Chọn Mã hàng" }],
            //    calculateDisplayValue: function (row) {
            //        if (!row) return "";
            //        return row.MaHang ? `${row.MaHang}` : "";
            //    },
            //    lookup: {
            //        dataSource: hangHoaStore,
            //        valueExpr: "MaHang",
            //        displayExpr: "MaHang" // Hiển thị mã hàng trong dropdown
            //    },
            //    editorOptions: {
            //        searchEnabled: true,
            //        searchMode: "contains",
            //        minSearchLength: 0,
            //        showDataBeforeSearch: true,
            //        showClearButton: true
            //    },
            //    // Fill Tên hàng khi chọn Mã hàng
            //    setCellValue: function (rowData, value) {
            //        rowData.MaHang = value;
            //        if (!value) { rowData.TenHang = null; return; }
            //        // Đoạn này vẫn cần request để lấy Tên Hàng hiển thị ngay
            //        hangHoaStore.byKey(value)
            //            .done(item => rowData.TenHang = item ? item.TenHang : null)
            //            .fail(() => rowData.TenHang = null);
            //    }
            //},

            //{ dataField: "TenHang", caption: "Tên hàng", allowEditing: false },
            {
                dataField: "NgayApDung",
                caption: "Ngày áp dụng",
                dataType: "date",
                format: "dd/MM/yyyy",
                editorOptions: { type: "date", displayFormat: "dd/MM/yyyy" },
                validationRules: [{ type: "required", message: "Chọn ngày áp dụng" }]
            },
            {
                caption: "SMV",
                alignment: "center",
                columns: [
                    {
                        dataField: "SMV",
                        caption: "SMV (phút)",
                        dataType: "number",
                        alignment: "right",
                        format: "#,##0.####",
                        editorOptions: { min: 0, showSpinButtons: true, step: 0.1 },
                        validationRules: [{ type: "required", message: "Nhập SMV" }]
                    },
                    {
                        dataField: "SMV_May",
                        caption: "SMV may",
                        dataType: "number",
                        alignment: "right",
                        format: "#,##0.####",
                        editorOptions: { min: 0, showSpinButtons: true, step: 0.1 }
                    },
                    {
                        dataField: "SMV_Cat",
                        caption: "SMV Cắt",
                        dataType: "number",
                        alignment: "right",
                        format: "#,##0.####",
                        editorOptions: { min: 0, showSpinButtons: true, step: 0.1 }
                    },
                    {
                        dataField: "SMV_Laptrinh",
                        caption: "SMV Lập trình",
                        dataType: "number",
                        alignment: "right",
                        format: "#,##0.####",
                        editorOptions: { min: 0, showSpinButtons: true, step: 0.1 }
                    },
                    {
                        dataField: "SMV_HoanThanh",
                        caption: "SMV Hoàn thành",
                        dataType: "number",
                        alignment: "right",
                        format: "#,##0.####",
                        editorOptions: { min: 0, showSpinButtons: true, step: 0.1 }
                    },
                    {
                        dataField: "In_ep",
                        caption: "In thêu",
                        dataType: "boolean"
                    },
                    {
                        dataField: "InTheuCT",
                        caption: "In thêu CT",
                        dataType: "boolean"
                    },
                    {
                        dataField: "HutAm",
                        caption: "Hút ẩm",
                        dataType: "boolean"
                    },
                    {
                        dataField: "DoKim",
                        caption: "Dò kim",
                        dataType: "boolean"
                    },
                    {
                        dataField: "CM",
                        caption: "CM",
                        dataType: "number",
                        alignment: "right",
                        format: "#,##0.####",
                        editorOptions: { min: 0, showSpinButtons: true, step: 0.1 }
                    }
                ]
            },
            {
                dataField: "HasSMV",
                caption: "Đã khai báo",
                dataType: "boolean",
                allowEditing: false,
                calculateCellValue: function (r) { return r.SMV != null; }
            },
            // Nút xóa vẫn giữ lại
            { type: "buttons", width: 60, buttons: ["delete"] }
        ],

        // --- CẤU HÌNH EDIT BATCH: chỉ lưu khi bấm nút Lưu ---
        editing: {
            mode: "batch",
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },

        onInitNewRow: function (e) {
            e.data.NgayApDung = new Date();
        },

        // Vẫn giữ logic khóa Mã hàng khi sửa dòng cũ
        onEditorPreparing: function (e) {
            if (e.parentType !== "dataRow") return;
            // Nếu không phải dòng mới (isNewRow = false/undefined) thì khóa MaHang
            if (e.dataField === "MaHang" && !e.row?.isNewRow) {
                e.editorOptions.disabled = true;
            }
        },

        toolbar: {
            items: [
                "addRowButton",
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Lưu",
                        icon: "save",
                        hint: "Lưu thay đổi",
                        width: 96,
                        stylingMode: "contained",
                        type: "success",
                        onClick: function () {
                            var grid = $(gridId).dxDataGrid("instance");
                            if (grid) {
                                grid.saveEditData();
                            }
                        }
                    }
                },
                "revertButton",
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Làm mới",
                        icon: "refresh",
                        stylingMode: "contained",
                        type: "default",
                        onClick: function () {
                            // Chỉ refresh khi người dùng bấm nút
                            $(gridId).dxDataGrid("instance").refresh();
                        }
                    }
                },
            ]
        },

        headerFilter: { visible: true },
        filterRow: { visible: true },
        searchPanel: { visible: true, width: 300, placeholder: "Tìm mã hàng..." },

        paging: { pageSize: 100 },
        pager: { showPageSizeSelector: true, allowedPageSizes: [50, 100, 150, 500], visible: true, displayMode: "full", },

        onSaved: function (e) {
            if (e.changes && e.changes.length > 0) {
                DevExpress.ui.notify("Đã lưu thay đổi thành công!", "success", 1200);
            }
        },
        onDataErrorOccurred: function (e) {
            DevExpress.ui.notify(e.error?.message || "Có lỗi khi xử lý SMV", "error", 2500);
        }
    });
}

function getCurrentUser() {
    return localStorage.getItem("username1") || localStorage.getItem("username") || "Unknown User";
}
