(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class HeaderColorPopupController {
        constructor(deps) {
            this.deps = deps || {};
            this.popup = null;
            this.items = [];
            this.groupSelect = null;
            this.headerBgColorEditor = null;
            this.headerTextColorEditor = null;
            this.manualBgColorEditor = null;
            this.manualBorderColorEditor = null;
            this.manualTextColorEditor = null;
            this.ttBomStatus1ColorEditor = null;
            this.ttBomStatus2ColorEditor = null;
            this.ttBomStatus3ColorEditor = null;
        }

        notify(message, type, time) {
            if (typeof this.deps.notify === "function") {
                this.deps.notify(message, type, time);
                return;
            }
            DevExpress.ui.notify(message, type, time);
        }

        canOpen() {
            return typeof this.deps.canOpen === "function" ? this.deps.canOpen() : true;
        }

        getPopupOptions() {
            return typeof this.deps.popupOptions === "function" ? this.deps.popupOptions() : {};
        }

        getGroupOptions() {
            return typeof this.deps.getGroupOptions === "function" ? this.deps.getGroupOptions() : (this.deps.groupOptions || []);
        }

        getSeed() {
            return typeof this.deps.getSeed === "function" ? (this.deps.getSeed() || {}) : (this.deps.seed || {});
        }

        getManualDefaults() {
            return typeof this.deps.getManualDefaults === "function" ? this.deps.getManualDefaults() : (this.deps.manualDefaults || {});
        }

        getTtBomDefaults() {
            return typeof this.deps.getTtBomDefaults === "function" ? this.deps.getTtBomDefaults() : (this.deps.ttBomDefaults || {});
        }

        normalizeHexColor(value) {
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
        }

        setItems(items) {
            this.items = Array.isArray(items) ? items : [];
            if (typeof this.deps.setItems === "function") this.deps.setItems(this.items);
        }

        getItems() {
            return this.items;
        }

        buildColorMap(items) {
            var self = this;
            var map = {};
            (items || []).forEach(function (item) {
                var key = String(item && item.CssClass || "").trim();
                var value = self.normalizeHexColor(item && item.ColorCode);
                if (key && value) map[key] = value;
            });
            return map;
        }

        getMapSnapshot() {
            return $.extend({}, this.getSeed(), this.buildColorMap(this.items));
        }

        getColorByCssClass(cssClass, fallback) {
            var map = this.getMapSnapshot();
            var color = this.normalizeHexColor(map[cssClass]);
            return color || this.normalizeHexColor(fallback) || "";
        }

        setEditorColor(editor, color) {
            if (!editor) return;
            var normalized = this.normalizeHexColor(color);
            editor.option("value", normalized || "");
        }

        upsertItem(cssClass, colorCode) {
            if (!cssClass || !colorCode) return;

            var found = false;
            for (var i = 0; i < this.items.length; i++) {
                if (String(this.items[i].CssClass || "").trim() === cssClass) {
                    this.items[i].ColorCode = colorCode;
                    found = true;
                    break;
                }
            }

            if (!found) {
                this.items.push({
                    CssClass: cssClass,
                    DisplayName: cssClass,
                    ColorCode: colorCode
                });
            }

            if (typeof this.deps.setItems === "function") this.deps.setItems(this.items);
        }

        syncGroupEditors(groupCssClass) {
            var key = String(groupCssClass || "").trim();
            if (!key) return;

            this.setEditorColor(this.headerBgColorEditor, this.getColorByCssClass(key, "#FFFFFF"));
            this.setEditorColor(this.headerTextColorEditor, this.getColorByCssClass(key + "-text", "#111827"));
        }

        syncManualEditors() {
            var defaults = this.getManualDefaults();
            this.setEditorColor(this.manualBgColorEditor, this.getColorByCssClass("manual-cell-bg", defaults.Background));
            this.setEditorColor(this.manualBorderColorEditor, this.getColorByCssClass("manual-cell-border", defaults.Border));
            this.setEditorColor(this.manualTextColorEditor, this.getColorByCssClass("manual-cell-text", defaults.Text));
        }

        syncTtBomEditors() {
            var defaults = this.getTtBomDefaults();
            this.setEditorColor(this.ttBomStatus1ColorEditor, this.getColorByCssClass("tt-bom-status-1-bg", defaults[1]));
            this.setEditorColor(this.ttBomStatus2ColorEditor, this.getColorByCssClass("tt-bom-status-2-bg", defaults[2]));
            this.setEditorColor(this.ttBomStatus3ColorEditor, this.getColorByCssClass("tt-bom-status-3-bg", defaults[3]));
        }

        refreshEditors() {
            var groups = this.getGroupOptions();
            var group = this.groupSelect ? this.groupSelect.option("value") : "";

            if (!group && this.groupSelect && groups.length) {
                group = groups[0].key;
                this.groupSelect.option("value", group);
            }

            this.syncGroupEditors(group);
            this.syncManualEditors();
            this.syncTtBomEditors();
        }

        load() {
            var self = this;
            return this.deps.loadService.loadHeaderColors().done(function (res) {
                self.setItems(Array.isArray(res) ? res : []);
                if (typeof self.deps.applyHeaderColors === "function") {
                    self.deps.applyHeaderColors(self.buildColorMap(self.items));
                }
                self.syncManualEditors();
                self.syncTtBomEditors();
                if (self.groupSelect) self.syncGroupEditors(self.groupSelect.option("value"));
            }).fail(function (xhr) {
                self.notify("Không tải được danh sách màu header: " + (xhr.responseText || xhr.statusText), "error", 2500);
            });
        }

        save() {
            var self = this;

            if (!this.canOpen()) {
                this.notify("Bạn không có quyền tùy chỉnh màu header", "warning", 1800);
                return;
            }

            var groupCssClass = this.groupSelect ? String(this.groupSelect.option("value") || "").trim() : "";
            if (!groupCssClass) {
                this.notify("Vui lòng chọn nhóm header.", "warning", 1800);
                return;
            }

            var updates = [
                { CssClass: groupCssClass, ColorCode: this.normalizeHexColor(this.headerBgColorEditor && this.headerBgColorEditor.option("value")) },
                { CssClass: groupCssClass + "-text", ColorCode: this.normalizeHexColor(this.headerTextColorEditor && this.headerTextColorEditor.option("value")) },
                { CssClass: "manual-cell-bg", ColorCode: this.normalizeHexColor(this.manualBgColorEditor && this.manualBgColorEditor.option("value")) },
                { CssClass: "manual-cell-border", ColorCode: this.normalizeHexColor(this.manualBorderColorEditor && this.manualBorderColorEditor.option("value")) },
                { CssClass: "manual-cell-text", ColorCode: this.normalizeHexColor(this.manualTextColorEditor && this.manualTextColorEditor.option("value")) },
                { CssClass: "tt-bom-status-1-bg", ColorCode: this.normalizeHexColor(this.ttBomStatus1ColorEditor && this.ttBomStatus1ColorEditor.option("value")) },
                { CssClass: "tt-bom-status-2-bg", ColorCode: this.normalizeHexColor(this.ttBomStatus2ColorEditor && this.ttBomStatus2ColorEditor.option("value")) },
                { CssClass: "tt-bom-status-3-bg", ColorCode: this.normalizeHexColor(this.ttBomStatus3ColorEditor && this.ttBomStatus3ColorEditor.option("value")) }
            ].filter(function (x) { return x.CssClass && x.ColorCode; });

            var currentMap = this.getMapSnapshot();
            updates = updates.filter(function (x) {
                var oldColor = self.normalizeHexColor(currentMap[x.CssClass]);
                return oldColor !== x.ColorCode;
            });

            if (!updates.length) {
                this.notify("Không có thay đổi màu.", "warning", 1600);
                return;
            }

            var reqs = updates.map(function (x) {
                return self.deps.saveService.updateHeaderColors($.extend({}, x, {
                    UserID: typeof self.deps.getUserName === "function" ? self.deps.getUserName() : ""
                }));
            });

            $.when.apply($, reqs).done(function () {
                updates.forEach(function (x) {
                    self.upsertItem(x.CssClass, x.ColorCode);
                });

                var map = self.getMapSnapshot();
                if (typeof self.deps.applyHeaderColors === "function") self.deps.applyHeaderColors(map);
                if (typeof self.deps.applyManualCellColors === "function") {
                    self.deps.applyManualCellColors({
                        Background: map["manual-cell-bg"],
                        Border: map["manual-cell-border"],
                        Text: map["manual-cell-text"]
                    });
                }
                if (typeof self.deps.applyTtBomStatusColors === "function") {
                    self.deps.applyTtBomStatusColors({
                        1: map["tt-bom-status-1-bg"],
                        2: map["tt-bom-status-2-bg"],
                        3: map["tt-bom-status-3-bg"]
                    });
                }

                self.notify("Cập nhật màu thành công.", "success", 1800);
            }).fail(function (xhr) {
                self.notify("Lưu màu thất bại: " + (xhr.responseText || xhr.statusText), "error", 2500);
            });
        }

        initPopup() {
            var self = this;
            var groups = this.getGroupOptions();

            this.popup = $("#popupHeaderColor").dxPopup($.extend(true, {
                title: "Tùy chỉnh màu",
                width: 980,
                height: "auto",
                minWidth: 680,
                showTitle: true,
                visible: false,
                dragEnabled: true,
                hideOnOutsideClick: false,
                contentTemplate: function (content) {
                    var $root = $("<div class='wip-color-popup'></div>").appendTo(content);
                    var $headerSection = $("<div class='wip-color-popup__section'></div>").appendTo($root);
                    $("<div class='wip-color-popup__title'>1) Header Group</div>").appendTo($headerSection);
                    $("<div id='hdrColorGroupSelect' class='wip-color-popup__full'></div>").appendTo($headerSection);
                    var $headerEditors = $("<div class='wip-color-popup__grid'></div>").appendTo($headerSection);
                    $("<div id='hdrColorPickerBg' class='wip-color-popup__editor'></div>").appendTo($headerEditors);
                    $("<div id='hdrColorPickerText' class='wip-color-popup__editor'></div>").appendTo($headerEditors);

                    var $manualSection = $("<div class='wip-color-popup__section'></div>").appendTo($root);
                    $("<div class='wip-color-popup__title'>2) Manual Highlight</div>").appendTo($manualSection);
                    var $manualEditors = $("<div class='wip-color-popup__grid wip-color-popup__grid--3'></div>").appendTo($manualSection);
                    $("<div id='manualCellBgPicker' class='wip-color-popup__editor'></div>").appendTo($manualEditors);
                    $("<div id='manualCellBorderPicker' class='wip-color-popup__editor'></div>").appendTo($manualEditors);
                    $("<div id='manualCellTextPicker' class='wip-color-popup__editor'></div>").appendTo($manualEditors);

                    var $ttBomSection = $("<div class='wip-color-popup__section'></div>").appendTo($root);
                    $("<div class='wip-color-popup__title'>3) TT_* Status</div>").appendTo($ttBomSection);
                    var $ttBomEditors = $("<div class='wip-color-popup__grid wip-color-popup__grid--3'></div>").appendTo($ttBomSection);
                    $("<div id='ttBomStatus1Picker' class='wip-color-popup__editor'></div>").appendTo($ttBomEditors);
                    $("<div id='ttBomStatus2Picker' class='wip-color-popup__editor'></div>").appendTo($ttBomEditors);
                    $("<div id='ttBomStatus3Picker' class='wip-color-popup__editor'></div>").appendTo($ttBomEditors);

                    self.groupSelect = $("#hdrColorGroupSelect").dxSelectBox({
                        dataSource: groups,
                        displayExpr: "name",
                        valueExpr: "key",
                        searchEnabled: true,
                        value: groups.length ? groups[0].key : "",
                        onValueChanged: function (e) {
                            self.syncGroupEditors(e.value);
                        }
                    }).dxSelectBox("instance");

                    self.headerBgColorEditor = $("#hdrColorPickerBg").dxColorBox({
                        label: "Màu Header",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.headerTextColorEditor = $("#hdrColorPickerText").dxColorBox({
                        label: "Màu Chữ",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.manualBgColorEditor = $("#manualCellBgPicker").dxColorBox({
                        label: "Nền khung nhập tay",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.manualBorderColorEditor = $("#manualCellBorderPicker").dxColorBox({
                        label: "Viền khung nhập tay",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.manualTextColorEditor = $("#manualCellTextPicker").dxColorBox({
                        label: "Chữ khung nhập tay",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.ttBomStatus1ColorEditor = $("#ttBomStatus1Picker").dxColorBox({
                        label: "Sớm so với KH_*",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.ttBomStatus2ColorEditor = $("#ttBomStatus2Picker").dxColorBox({
                        label: "Trễ so với KH_*",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.ttBomStatus3ColorEditor = $("#ttBomStatus3Picker").dxColorBox({
                        label: "Thao tác trễ so với KH_*",
                        labelMode: "floating",
                        editAlphaChannel: false,
                        applyValueMode: "instantly",
                        value: ""
                    }).dxColorBox("instance");

                    self.refreshEditors();
                },
                toolbarItems: [
                    {
                        toolbar: "bottom",
                        location: "after",
                        widget: "dxButton",
                        options: {
                            text: "Lưu",
                            type: "success",
                            stylingMode: "contained",
                            onClick: function () {
                                self.save();
                            }
                        }
                    },
                    {
                        toolbar: "bottom",
                        location: "after",
                        widget: "dxButton",
                        options: {
                            text: "Đóng",
                            type: "normal",
                            stylingMode: "outlined",
                            onClick: function () {
                                if (self.popup) self.popup.hide();
                            }
                        }
                    }
                ]
            }, this.getPopupOptions())).dxPopup("instance");

            return this.popup;
        }

        getPopup() {
            return this.popup;
        }

        open() {
            if (!this.canOpen()) {
                this.notify("Bạn không có quyền tùy chỉnh màu header", "warning", 1800);
                return;
            }

            if (!this.popup) this.initPopup();
            if (this.popup) this.popup.show();
            this.refreshEditors();
        }
    }

    WIP.UI.HeaderColorPopupController = HeaderColorPopupController;
})(window, jQuery);
