(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class AuditPopupController {
        constructor(deps) {
            this.deps = deps || {};
            this.popup = null;
            this.grid = null;
            this.loadXhr = null;
            this.filter = {
                wipId: 0,
                fromDate: null,
                toDate: null
            };
        }

        getIds() {
            if (typeof this.deps.getIds === "function") return this.deps.getIds();
            return this.deps.ids || {};
        }

        notify(message, type, time) {
            if (typeof this.deps.notify === "function") {
                this.deps.notify(message, type, time);
                return;
            }
            DevExpress.ui.notify(message, type, time);
        }

        getPopupOptions() {
            return typeof this.deps.popupOptions === "function" ? this.deps.popupOptions() : {};
        }

        isGiaCongMode() {
            return typeof this.deps.isGiaCongMode === "function" && this.deps.isGiaCongMode();
        }

        resetFilter(wipId) {
            this.filter.wipId = wipId || 0;
            this.filter.fromDate = null;
            this.filter.toDate = null;
        }

        formatFieldName(fieldName) {
            var fieldKey = String(fieldName || "").trim();
            var captionMap = typeof this.deps.getFieldCaptionMap === "function"
                ? (this.deps.getFieldCaptionMap() || {})
                : (this.deps.fieldCaptionMap || {});

            if (captionMap[fieldKey]) return captionMap[fieldKey];
            if (typeof this.deps.getFieldCaption === "function") {
                return this.deps.getFieldCaption(fieldKey, fieldKey);
            }
            return fieldKey;
        }

        parseBooleanLike(value) {
            if (value == null) return null;
            if (typeof value === "boolean") return value;

            var normalized = String(value).trim().toLowerCase();
            if (!normalized) return null;
            if (normalized === "true") return true;
            if (normalized === "false") return false;
            return null;
        }

        formatDefaultValue(value) {
            if (value == null || value === "") return "-";

            var boolValue = this.parseBooleanLike(value);
            if (boolValue === true) return "Có";
            if (boolValue === false) return "Không";

            return String(value);
        }

        formatBooleanValue(value) {
            return this.formatDefaultValue(value);
        }

        formatValueByField(fieldName, value) {
            var fieldKey = String(fieldName || "").trim();
            var booleanFields = {
                InTheu: true,
                DoKim: true,
                HutAm: true,
                InTheuTrong_PKH: true
            };

            if (booleanFields[fieldKey]) return this.formatBooleanValue(value);
            return this.formatDefaultValue(value);
        }

        mapApiItem(item) {
            var source = item || {};
            var fieldNameRaw = source.FieldName || source.fieldName || "";
            var name = source.ModifiedByName || source.modifiedByName;
            var account = source.ModifiedBy || source.modifiedBy;

            return {
                AuditId: source.AuditId != null ? source.AuditId : source.auditId,
                WipId: source.WipId != null ? source.WipId : source.wipId,
                fieldName: this.formatFieldName(fieldNameRaw),
                OldValue: this.formatValueByField(fieldNameRaw, source.OldValue != null ? source.OldValue : source.oldValue),
                NewValue: this.formatValueByField(fieldNameRaw, source.NewValue != null ? source.NewValue : source.newValue),
                ModifiedBy: name ? name : account,
                ModifiedAt: source.ModifiedAt != null ? source.ModifiedAt : source.modifiedAt
            };
        }

        renderHeader(rowData) {
            var ids = this.getIds();
            var isGiaCong = this.isGiaCongMode();
            var label = isGiaCong ? "Đơn vị gia công" : "Chuyền";
            var dataField = isGiaCong ? rowData.TenDVSX : rowData.LineName;
            var headerItems = [
                { label: label, value: dataField },
                { label: "Mã lệnh sản xuất", value: rowData && rowData.LenhSX ? rowData.LenhSX : "" },
                { label: "Mã ĐH", value: rowData && rowData.MaDH ? rowData.MaDH : "" },
                { label: "Mã hàng", value: rowData && rowData.MaHang ? rowData.MaHang : "" }
            ];
            var $header = $("#" + ids.header);

            $header.empty();
            $.each(headerItems, function (_, item) {
                $("<div>", { class: "wip-audit-header__item" })
                    .append(
                        $("<div>", { class: "wip-audit-header__body" })
                            .append($("<span>", { class: "wip-audit-header__label", text: item.label }))
                            .append($("<span>", { class: "wip-audit-header__value", text: item.value || "-" }))
                    )
                    .appendTo($header);
            });
        }

        createValueCell(className) {
            return function (container, options) {
                $("<span>", {
                    class: className,
                    text: options.value == null || options.value === "" ? "-" : options.value
                }).appendTo(container);
            };
        }

        ensureFilterUi() {
            var ids = this.getIds();
            var self = this;
            var $popup = $("#" + ids.popup);
            var $content = $popup.find(".wip-audit-popup__content");
            if (!$content.length) return;

            var $header = $("#" + ids.header);
            var $grid = $("#" + ids.grid);

            if (!document.getElementById(ids.filter)) {
                var $filter = $("<div>", {
                    id: ids.filter,
                    class: "wip-audit-filter"
                });

                var $from = $("<div>", { id: ids.from, class: "wip-audit-filter__date" }).appendTo($filter);
                var $to = $("<div>", { id: ids.to, class: "wip-audit-filter__date" }).appendTo($filter);
                var $apply = $("<div>", { id: ids.apply }).appendTo($filter);
                var $clear = $("<div>", { id: ids.clear }).appendTo($filter);

                if ($header.length && $grid.length) $filter.insertAfter($header);
                else $filter.appendTo($content);

                $from.dxDateBox({
                    type: "date",
                    displayFormat: "dd/MM/yyyy",
                    dateSerializationFormat: "yyyy-MM-dd",
                    placeholder: "Từ ngày",
                    height: 30,
                    value: this.filter.fromDate,
                    onValueChanged: function (e) {
                        self.filter.fromDate = e.value || null;
                    }
                });

                $to.dxDateBox({
                    type: "date",
                    displayFormat: "dd/MM/yyyy",
                    dateSerializationFormat: "yyyy-MM-dd",
                    placeholder: "Đến ngày",
                    value: this.filter.toDate,
                    height: 30,
                    onValueChanged: function (e) {
                        self.filter.toDate = e.value || null;
                    }
                });

                $apply.dxButton({
                    text: "Áp dụng",
                    height: 30,
                    type: "default",
                    onClick: function () {
                        if (self.filter.fromDate && self.filter.toDate && self.filter.fromDate > self.filter.toDate) {
                            self.notify("'Từ ngày' không được lớn hơn 'Đến ngày'", "warning", 2000);
                            return;
                        }
                        self.loadData(self.filter.wipId, self.filter.fromDate, self.filter.toDate);
                    }
                });

                $clear.dxButton({
                    text: "Xóa",
                    height: 30,
                    stylingMode: "outlined",
                    onClick: function () {
                        self.filter.fromDate = null;
                        self.filter.toDate = null;
                        var fromInst = $("#" + ids.from).dxDateBox("instance");
                        var toInst = $("#" + ids.to).dxDateBox("instance");
                        if (fromInst) fromInst.option("value", null);
                        if (toInst) toInst.option("value", null);
                        self.loadData(self.filter.wipId, null, null);
                    }
                });
            } else {
                var from = $("#" + ids.from).dxDateBox("instance");
                var to = $("#" + ids.to).dxDateBox("instance");
                if (from) from.option("value", this.filter.fromDate);
                if (to) to.option("value", this.filter.toDate);
            }
        }

        ensurePopup() {
            var ids = this.getIds();
            var self = this;

            if (!document.getElementById(ids.popup) ||
                !document.getElementById(ids.header) ||
                !document.getElementById(ids.grid)) {
                this.notify("Thiếu HTML popup Audit trong trang WIP.", "warning", 2500);
                return false;
            }

            this.ensureFilterUi();

            if (!this.grid) {
                this.grid = $("#" + ids.grid).dxDataGrid({
                    dataSource: [],
                    elementAttr: { class: "wip-audit-grid__table" },
                    showBorders: true,
                    hoverStateEnabled: true,
                    rowAlternationEnabled: true,
                    columnAutoWidth: false,
                    columnResizingMode: "widget",
                    wordWrapEnabled: true,
                    paging: { enabled: false },
                    scrolling: { mode: "standard", showScrollbar: "always", useNative: true },
                    editing: { allowUpdating: false, allowAdding: false, allowDeleting: false },
                    columns: [
                        { dataField: "fieldName", caption: "Trường", minWidth: 220 },
                        { dataField: "OldValue", caption: "Giá trị cũ", minWidth: 140, cellTemplate: this.createValueCell("wip-audit-old-value") },
                        { dataField: "NewValue", caption: "Giá trị mới", minWidth: 140, cellTemplate: this.createValueCell("wip-audit-new-value") },
                        { dataField: "ModifiedBy", caption: "Người sửa", minWidth: 120 },
                        { dataField: "ModifiedAt", caption: "Thời gian sửa", dataType: "datetime", format: "dd/MM/yyyy HH:mm:ss", minWidth: 170 }
                    ]
                }).dxDataGrid("instance");
            }

            if (!this.popup) {
                this.popup = $("#" + ids.popup).dxPopup($.extend(true, {
                    title: "Lịch sử chỉnh sửa WIP",
                    maxWidth: 1300,
                    minWidth: 420,
                    minHeight: 360,
                    showTitle: true,
                    showCloseButton: true,
                    dragEnabled: true,
                    hideOnOutsideClick: true,
                    resizeEnabled: true,
                    deferRendering: false,
                    toolbarItems: [
                        {
                            widget: "dxButton",
                            toolbar: "bottom",
                            location: "after",
                            options: {
                                text: "Đóng",
                                onClick: function () {
                                    if (self.popup) self.popup.hide();
                                }
                            }
                        }
                    ],
                    onShown: function () {
                        if (self.grid) self.grid.updateDimensions();
                    }
                }, this.getPopupOptions())).dxPopup("instance");
            }

            return true;
        }

        clearGrid() {
            if (this.grid) this.grid.option("dataSource", []);
        }

        showPopup() {
            if (this.popup) this.popup.show();
        }

        getLoadService() {
            if (typeof this.deps.getLoadService === "function") {
                return this.deps.getLoadService();
            }
            return this.deps.loadService || null;
        }

        loadData(wipId, fromDate, toDate) {
            var self = this;
            var loadService = this.getLoadService();
            if (!this.grid) return;
            if (!loadService || typeof loadService.loadAudit !== "function") {
                this.notify("Audit service chưa sẵn sàng.", "error", 2500);
                return;
            }

            try {
                if (this.loadXhr && this.loadXhr.abort) this.loadXhr.abort();
            } catch (e) { }

            this.grid.option("dataSource", []);
            this.loadXhr = loadService.loadAudit({
                wipId: wipId,
                fromDate: fromDate,
                toDate: toDate
            }, {
                success: function (res) {
                    var items = Array.isArray(res) ? res : [];
                    self.grid.option("dataSource", items.map(function (item) {
                        return self.mapApiItem(item);
                    }));
                },
                error: function (xhr, status) {
                    if (status === "abort") return;
                    self.grid.option("dataSource", []);
                    self.notify("Có lỗi xảy ra khi tải dữ liệu lịch sử", "error", 2500);
                },
                complete: function () {
                    self.loadXhr = null;
                }
            });
        }

        open(rowData) {
            if (!this.ensurePopup()) return;

            var wipId = rowData && rowData.id != null
                ? Number(rowData.id)
                : Number(rowData && rowData.WIPId != null ? rowData.WIPId : 0);

            this.resetFilter(wipId);
            this.ensureFilterUi();
            this.renderHeader(rowData || {});
            this.clearGrid();
            this.showPopup();
            this.loadData(this.filter.wipId, this.filter.fromDate, this.filter.toDate);
        }
    }

    WIP.UI.AuditPopupController = AuditPopupController;
})(window, jQuery);
