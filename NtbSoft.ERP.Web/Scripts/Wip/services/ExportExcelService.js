(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    function ExportExcelService(deps) {
        this.deps = deps || {};
    }

    ExportExcelService.prototype.notify = function (message, type, time) {
        if (typeof this.deps.notify === "function") {
            this.deps.notify(message, type, time);
            return;
        }
        DevExpress.ui.notify(message, type, time);
    };

    ExportExcelService.prototype.saveExcelBuffer = function (buffer, fileName) {
        var blob = new Blob(
            [buffer],
            { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
        );

        if (window.navigator && typeof window.navigator.msSaveOrOpenBlob === "function") {
            window.navigator.msSaveOrOpenBlob(blob, fileName);
            return;
        }

        var url = URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        setTimeout(function () { URL.revokeObjectURL(url); }, 0);
    };

    ExportExcelService.prototype.buildWipExportFileName = function () {
        var d = new Date();
        var yyyy = d.getFullYear();
        var mm = String(d.getMonth() + 1).padStart(2, "0");
        var dd = String(d.getDate()).padStart(2, "0");
        var hh = String(d.getHours()).padStart(2, "0");
        var mi = String(d.getMinutes()).padStart(2, "0");
        var ss = String(d.getSeconds()).padStart(2, "0");
        var selectedLineX = typeof this.deps.getSelectedLineX === "function" ? this.deps.getSelectedLineX() : null;
        var lineItems = typeof this.deps.getLineItems === "function" ? (this.deps.getLineItems() || []) : [];
        var lineText = "ALL";

        if (selectedLineX != null) {
            var selectedItem = lineItems.find(function (it) {
                return it && it.value === selectedLineX;
            });

            lineText = selectedItem && selectedItem.text
                ? selectedItem.text
                : String(selectedLineX);
        }

        lineText = String(lineText)
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .replace(/\u0111/g, "d")
            .replace(/\u0110/g, "D")
            .replace(/[^\w-]/g, "_");

        return "WIP_" + lineText + "_" + yyyy + mm + dd + "_" + hh + mi + ss + ".xlsx";
    };

    ExportExcelService.prototype.normalizeHexColor = function (value) {
        if (typeof this.deps.normalizeHexColor === "function") {
            return this.deps.normalizeHexColor(value);
        }

        var raw = String(value || "").trim().toUpperCase();
        if (!raw) return "";
        if (raw.charAt(0) !== "#") raw = "#" + raw;
        if (/^#[0-9A-F]{3}$/.test(raw)) {
            return "#" + raw.charAt(1) + raw.charAt(1) + raw.charAt(2) + raw.charAt(2) + raw.charAt(3) + raw.charAt(3);
        }
        if (/^#[0-9A-F]{6}$/.test(raw)) return raw;
        return "";
    };

    ExportExcelService.prototype.colorToExcelArgb = function (value) {
        var raw = String(value || "").trim();
        if (!raw) return "";

        var hex = this.normalizeHexColor(raw);
        if (hex) return "FF" + hex.slice(1).toUpperCase();

        var rgbaMatch = raw.match(/^rgba?\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)(?:\s*,\s*(\d*\.?\d+))?\s*\)$/i);
        if (!rgbaMatch) return "";

        var r = Math.max(0, Math.min(255, Number(rgbaMatch[1] || 0)));
        var g = Math.max(0, Math.min(255, Number(rgbaMatch[2] || 0)));
        var b = Math.max(0, Math.min(255, Number(rgbaMatch[3] || 0)));
        var aRaw = rgbaMatch[4];
        var a = 255;

        if (aRaw != null && aRaw !== "") {
            var alpha01 = Math.max(0, Math.min(1, Number(aRaw)));
            a = Math.round(alpha01 * 255);
        }

        var toHex2 = function (n) { return Number(n).toString(16).padStart(2, "0").toUpperCase(); };
        return toHex2(a) + toHex2(r) + toHex2(g) + toHex2(b);
    };

    ExportExcelService.prototype.splitClassNames = function (classListText) {
        if (typeof this.deps.splitClassNames === "function") {
            return this.deps.splitClassNames(classListText);
        }
        return String(classListText || "").split(/\s+/).filter(Boolean);
    };

    ExportExcelService.prototype.hasClassName = function (classListText, className) {
        return this.splitClassNames(classListText).indexOf(className) >= 0;
    };

    ExportExcelService.prototype.getHeaderColorCssVarMap = function () {
        return {
            "grid-header-default": "--grid-header-default-bg",
            "hdr-donhang": "--hdr-donhang-bg",
            "hdr-kh-ngay": "--hdr-kh-ngay-bg",
            "hdr-qc": "--hdr-qc-bg",
            "hdr-merchandiser": "--hdr-merchandiser-bg",
            "hdr-nangsuat": "--hdr-nangsuat-bg",
            "hdr-tiendo-thucte": "--hdr-tiendo-thucte-bg",
            "hdr-kythuat": "--hdr-kythuat-bg",
            "hdr-ie": "--hdr-ie-bg",
            "hdr-codien": "--hdr-codien-bg",
            "hdr-intheu": "--hdr-intheu-bg",
            "hdr-dongbo-intheu": "--hdr-dongbo-intheu-bg",
            "hdr-thoigian": "--hdr-thoigian-bg",
            "hdr-tinhtoan-nangsuat": "--hdr-tinhtoan-nangsuat-bg"
        };
    };

    ExportExcelService.prototype.getHeaderTextColorCssVarMap = function () {
        return {
            "grid-header-default": "--grid-header-default-text",
            "hdr-donhang": "--hdr-donhang-text",
            "hdr-kh-ngay": "--hdr-kh-ngay-text",
            "hdr-qc": "--hdr-qc-text",
            "hdr-merchandiser": "--hdr-merchandiser-text",
            "hdr-nangsuat": "--hdr-nangsuat-text",
            "hdr-tiendo-thucte": "--hdr-tiendo-thucte-text",
            "hdr-kythuat": "--hdr-kythuat-text",
            "hdr-ie": "--hdr-ie-text",
            "hdr-codien": "--hdr-codien-text",
            "hdr-intheu": "--hdr-intheu-text",
            "hdr-dongbo-intheu": "--hdr-dongbo-intheu-text",
            "hdr-thoigian": "--hdr-thoigian-text",
            "hdr-tinhtoan-nangsuat": "--hdr-tinhtoan-nangsuat-text"
        };
    };

    ExportExcelService.prototype.normalizeHeaderColorClassName = function (name) {
        if (typeof this.deps.normalizeHeaderColorClassName === "function") {
            return this.deps.normalizeHeaderColorClassName(name);
        }
        return String(name || "").trim();
    };

    ExportExcelService.prototype.buildHeaderColorMap = function (items) {
        if (typeof this.deps.buildHeaderColorMap === "function") {
            return this.deps.buildHeaderColorMap(items);
        }
        return {};
    };

    ExportExcelService.prototype.normalizeHeaderColorConfig = function (config) {
        if (typeof this.deps.normalizeHeaderColorConfig === "function") {
            return this.deps.normalizeHeaderColorConfig(config);
        }
        return config && typeof config === "object" ? config : {};
    };

    ExportExcelService.prototype.resolveHeaderColorClassForExport = function (gridCell) {
        var col = gridCell && gridCell.column ? gridCell.column : null;
        var classNames = this.splitClassNames(col && col.headerCssClass);
        var classSet = typeof this.deps.getHeaderColorClassSet === "function" ? (this.deps.getHeaderColorClassSet() || {}) : {};
        var i;

        for (i = 0; i < classNames.length; i++) {
            var normalized = this.normalizeHeaderColorClassName(classNames[i]);
            if (classSet[normalized]) return normalized;
        }

        var byField = typeof this.deps.getPrefixedHeaderClass === "function"
            ? this.deps.getPrefixedHeaderClass(col && col.dataField)
            : "";

        if (byField) return this.normalizeHeaderColorClassName(byField);
        return "";
    };

    ExportExcelService.prototype.resolveHeaderArgbForExport = function (headerClassKey) {
        var normalizedKey = this.normalizeHeaderColorClassName(headerClassKey);
        var colorByKey = $.extend(
            {},
            this.normalizeHeaderColorConfig(window.WIP_HEADER_COLOR_CONFIG),
            this.buildHeaderColorMap(typeof this.deps.getHeaderColorItems === "function" ? this.deps.getHeaderColorItems() : [])
        );

        var configuredColor = colorByKey[normalizedKey] || colorByKey[headerClassKey];
        var argbFromConfig = this.colorToExcelArgb(configuredColor);
        if (argbFromConfig) return argbFromConfig;

        var cssVarMap = this.getHeaderColorCssVarMap();
        var cssVarName = cssVarMap[normalizedKey] || cssVarMap["grid-header-default"];
        var cssValue = window.getComputedStyle(document.documentElement).getPropertyValue(cssVarName);
        var argbFromCss = this.colorToExcelArgb(cssValue);
        if (argbFromCss) return argbFromCss;

        return "FF6EC2F7";
    };

    ExportExcelService.prototype.resolveHeaderTextArgbForExport = function (headerClassKey) {
        var normalizedKey = this.normalizeHeaderColorClassName(headerClassKey || "grid-header-default");
        var textKey = normalizedKey + "-text";
        var colorByKey = $.extend(
            {},
            typeof this.deps.getDefaultHeaderTextColorConfig === "function" ? this.deps.getDefaultHeaderTextColorConfig() : {},
            this.normalizeHeaderColorConfig(window.WIP_HEADER_TEXT_COLOR_CONFIG),
            this.normalizeHeaderColorConfig(window.WIP_HEADER_COLOR_CONFIG),
            this.buildHeaderColorMap(typeof this.deps.getHeaderColorItems === "function" ? this.deps.getHeaderColorItems() : [])
        );

        var configuredColor = colorByKey[textKey] || colorByKey[normalizedKey] || colorByKey["grid-header-default-text"];
        var argbFromConfig = this.colorToExcelArgb(configuredColor);
        if (argbFromConfig) return argbFromConfig;

        var cssVarMap = this.getHeaderTextColorCssVarMap();
        var cssVarName = cssVarMap[normalizedKey] || cssVarMap["grid-header-default"];
        var cssValue = window.getComputedStyle(document.documentElement).getPropertyValue(cssVarName);
        var argbFromCss = this.colorToExcelArgb(cssValue);
        if (argbFromCss) return argbFromCss;

        return "FF111827";
    };

    ExportExcelService.prototype.resolveManualCellArgbForExport = function () {
        var manualConfig = typeof this.deps.getManualCellColorConfig === "function" ? this.deps.getManualCellColorConfig() : {};
        var argbFromConfig = this.colorToExcelArgb(manualConfig && manualConfig.Background);
        if (argbFromConfig) return argbFromConfig;

        var cssValue = window.getComputedStyle(document.documentElement).getPropertyValue("--manual-cell-bg");
        var argbFromCss = this.colorToExcelArgb(cssValue);
        if (argbFromCss) return argbFromCss;

        return "FFFFE8C2";
    };

    ExportExcelService.prototype.getSelectedLineText = function () {
        var tabs = typeof this.deps.getTabs === "function" ? this.deps.getTabs() : null;
        var text = "Tất cả";

        if (tabs) {
            var selectedItem = tabs.option("selectedItem");
            if (selectedItem) text = selectedItem.text;
        }

        return text;
    };

    ExportExcelService.prototype.exportWipVisibleGridToExcel = async function () {
        var grid = typeof this.deps.getGrid === "function" ? this.deps.getGrid() : null;
        if (!grid) return;

        if (!window.ExcelJS || !DevExpress || !DevExpress.excelExporter) {
            this.notify("Thiếu thư viện export Excel (ExcelJS/DevExpress exporter).", "error", 2500);
            return;
        }

        var self = this;
        if (typeof this.deps.showBusy === "function") this.deps.showBusy();

        var originalLoadPanelEnabled = grid.option("loadPanel.enabled");
        grid.option("loadPanel.enabled", false);

        try {
            var workbook = new ExcelJS.Workbook();
            var worksheet = workbook.addWorksheet("WIP");

            await DevExpress.excelExporter.exportDataGrid({
                component: grid,
                worksheet: worksheet,
                topLeftCell: { row: 7, column: 1 },
                keepColumnWidths: true,
                autoFilterEnabled: true,
                selectedRowsOnly: false,
                customizeCell: function (options) {
                    var gridCell = options.gridCell;
                    var excelCell = options.excelCell;
                    if (!gridCell || !excelCell) return;

                    excelCell.alignment = {
                        vertical: "middle",
                        horizontal: (gridCell.column && gridCell.column.alignment) || "left",
                        wrapText: true
                    };
                    excelCell.border = {
                        top: { style: "thin", color: { argb: "FFD1D5DB" } },
                        left: { style: "thin", color: { argb: "FFD1D5DB" } },
                        bottom: { style: "thin", color: { argb: "FFD1D5DB" } },
                        right: { style: "thin", color: { argb: "FFD1D5DB" } }
                    };

                    if (gridCell.rowType === "header") {
                        var headerClassKey = self.resolveHeaderColorClassForExport(gridCell);
                        var headerArgb = self.resolveHeaderArgbForExport(headerClassKey || "grid-header-default");
                        var headerTextArgb = self.resolveHeaderTextArgbForExport(headerClassKey || "grid-header-default");
                        excelCell.font = { bold: true, color: { argb: headerTextArgb } };
                        excelCell.fill = { type: "pattern", pattern: "solid", fgColor: { argb: headerArgb } };
                        excelCell.border.bottom = { style: "thin", color: { argb: "FF000000" } };
                        if (self.hasClassName(gridCell.column && gridCell.column.headerCssClass, "group-end")) {
                            excelCell.border.right = { style: "medium", color: { argb: "FF000000" } };
                        }
                    }

                    if (gridCell.rowType === "data") {
                        if (gridCell.column && gridCell.column.dataType === "date") {
                            excelCell.numFmt = "dd/mm/yy";
                        }
                        if (gridCell.column && gridCell.column.dataType === "boolean") {
                            excelCell.value = gridCell.value ? "x" : "";
                            excelCell.alignment = { horizontal: "center", vertical: "middle" };
                        }

                        var dataField = gridCell.column && gridCell.column.dataField;
                        var isManualCell = typeof self.deps.isManualCell === "function" && self.deps.isManualCell(gridCell.data, dataField);
                        if (dataField && isManualCell) {
                            excelCell.fill = {
                                type: "pattern",
                                pattern: "solid",
                                fgColor: { argb: self.resolveManualCellArgbForExport() }
                            };
                        }
                        if (self.hasClassName(gridCell.column && gridCell.column.cssClass, "txt-red-bold")) {
                            excelCell.font = { bold: true, color: { argb: "FFFF0000" } };
                        }
                        if (self.hasClassName(gridCell.column && gridCell.column.cssClass, "group-end")) {
                            excelCell.border.right = { style: "medium", color: { argb: "FF000000" } };
                        }
                    }
                }
            });

            worksheet.getColumn(1).width = 8;
            worksheet.getColumn(2).width = 25;
            worksheet.mergeCells("A1:B4");

            var logoContainer = worksheet.getCell("A1");
            logoContainer.border = {
                top: { style: "thin" },
                left: { style: "thin" },
                bottom: { style: "thin" },
                right: { style: "thin" }
            };
            logoContainer.value = "VIKING VIETNAM CO.LTD";
            logoContainer.font = { name: "Times New Roman", bold: true, size: 12 };
            logoContainer.alignment = { horizontal: "center", vertical: "top", wrapText: true };

            var logoBase64 = typeof this.deps.getImageByField === "function" ? await this.deps.getImageByField() : "";
            if (logoBase64) {
                var extension = "png";
                if (logoBase64.indexOf("image/jpeg") > -1) extension = "jpeg";
                else if (logoBase64.indexOf("image/jpg") > -1) extension = "jpeg";

                var logoId = workbook.addImage({
                    base64: logoBase64,
                    extension: extension
                });

                worksheet.addImage(logoId, {
                    tl: { col: 0.70, row: 1.2 },
                    ext: { width: 80, height: 45 },
                    editAs: "oneCell"
                });
            }

            var tenChuyen = this.getSelectedLineText();
            var thoiGianXuat = new Date().toLocaleString("vi-VN");

            worksheet.getCell("C2").value = "CHUYỀN: " + String(tenChuyen || "").toUpperCase();
            worksheet.getCell("C2").font = { bold: true, size: 11, color: { argb: "FF0000FF" } };
            worksheet.getCell("C3").value = "NGÀY: " + thoiGianXuat;
            worksheet.getCell("C3").font = { bold: true, size: 10 };

            var visibleColumns = grid.getVisibleColumns();
            var fixedLeftCount = visibleColumns.filter(function (c) {
                return c && c.fixed === true && (!c.fixedPosition || c.fixedPosition === "left");
            }).length;

            worksheet.views = [{
                state: "frozen",
                ySplit: 7,
                xSplit: fixedLeftCount
            }];

            var buffer = await workbook.xlsx.writeBuffer();
            var fileName = this.buildWipExportFileName();
            this.saveExcelBuffer(buffer, fileName);
            this.notify("Đã xuất Excel!", "success", 1500);
        } catch (err) {
            var msg = (err && (err.message || err.toString())) || "Unknown error";
            this.notify("Xuất Excel lỗi: " + msg, "error", 3000);
            console.error(err);
        } finally {
            if (typeof this.deps.hideBusy === "function") this.deps.hideBusy();

            if (originalLoadPanelEnabled !== undefined) {
                grid.option("loadPanel.enabled", originalLoadPanelEnabled);
            } else {
                grid.option("loadPanel.enabled", true);
            }
        }
    };

    WIP.Services.ExportExcelService = ExportExcelService;
})(window, jQuery);
