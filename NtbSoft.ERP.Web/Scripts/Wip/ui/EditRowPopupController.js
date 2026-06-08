(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    function defaultEmptyRow() {
        return {
            WIPId: null,
            id: 0,
            ThuTuChuyen: null,
            LineX: null,
            LineName: null,
            LenhSX: null,
            MaKH: null,
            Season: null,
            StyleId: null,
            Mer: null,
            MaHang: null,
            KhachHang: null,
            PO: null,
            SLKH: null,
            InTheu: false,
            DoKim: false,
            HutAm: false,
            InTheuTrong_PKH: false,
            NSGio: null,
            SoGioSX: null,
            SoNgay: null,
            SLCN: null,
            //TGLV: null,
            //OT: null,
            //AllocatedRegularHours: null,
            //AllocatedOTHours: null,
            HieuSuat: null,
            KHCat: null,
            KHLapTrinh: null,
            KHMay: null,
            ThoatChuyen: null,
            TT_Cat: null,
            RaChuyen: null,
            BTP_DK: null,
            SMV_WIP: null,
            NSCost: null,
            NS_SMV: null,
            CostPerM: null,
            Profit: null,
            CM: null,
            HeSoCM: null,
            DoanhThuCM: null,
            GhiChuKho: null
        };
    }

    function EditRowPopupController(options) {
        this.options = options || {};
        this.$ = this.options.$ || window.jQuery;
        this.mode = "add";
        this.changedFields = {};
        this.formData = this.createEmptyRow();
        this.popup = null;
    }

    EditRowPopupController.getEmptyRow = defaultEmptyRow;

    EditRowPopupController.safeFormUpdate = function (ctx, fn) {
        if (!ctx) return fn();
        ctx.suppressFieldChanged = true;
        try { fn(); }
        finally {
            setTimeout(function () { ctx.suppressFieldChanged = false; }, 0);
        }
    };

    EditRowPopupController.prototype.createEmptyRow = function () {
        if (typeof this.options.getEmptyRow === "function") {
            return this.options.getEmptyRow();
        }
        return defaultEmptyRow();
    };

    EditRowPopupController.prototype.getMode = function () {
        return this.mode;
    };

    EditRowPopupController.prototype.getFormData = function () {
        return this.formData;
    };

    EditRowPopupController.prototype.getChangedFields = function () {
        return this.changedFields;
    };

    EditRowPopupController.prototype.markFieldChanged = function (field) {
        var f = String(field || "").replace("__libId_", "");
        if (!f) return;
        this.changedFields[f] = true;
    };

    EditRowPopupController.prototype.markCtxFieldChanged = function (ctx, field) {
        var f = String(field || "").replace("__libId_", "");
        if (!f) return;

        if (ctx && typeof ctx.markFieldChanged === "function") {
            ctx.markFieldChanged(f);
            return;
        }

        this.markFieldChanged(f);
    };

    EditRowPopupController.prototype.prepareFormItems = function (items, groupCaption) {
        var self = this;
        var opt = this.options;
        var $ = this.$;

        return (items || []).map(function (item) {
            if (!item || typeof item !== "object") return item;

            var current = $.extend(true, {}, item);
            var hasChildren = Array.isArray(current.items) && current.items.length > 0;
            var currentGroupCaption = current.itemType === "group"
                ? String(current.caption || "").trim()
                : groupCaption;

            if (hasChildren) {
                current.items = self.prepareFormItems(current.items, currentGroupCaption);
                if (!current.items.some(function (child) { return child && child.visible !== false; })) {
                    current.visible = false;
                }
                return current;
            }

            var permissionField = current.permissionField || current.dataField || "";
            if (typeof opt.normalizePermissionField === "function") {
                permissionField = opt.normalizePermissionField(permissionField);
            }
            if (String(permissionField).indexOf("__libId_") === 0) {
                permissionField = String(permissionField).replace("__libId_", "");
            }

            var shouldBypassViewPermission = currentGroupCaption === opt.orderInfoGroupCaption;
            if (
                permissionField &&
                !shouldBypassViewPermission &&
                typeof opt.canViewFieldByPermission === "function" &&
                !opt.canViewFieldByPermission(permissionField, this.formData && this.formData.LineX)
            ) {
                current.visible = false;
            }

            return current;
        }, this);
    };

    EditRowPopupController.prototype.createFormContext = function () {
        var self = this;
        return {
            data: this.formData,
            formSelector: this.options.formSelector || "#wipForm",
            getLineX: function () { return self.formData.LineX || null; },
            suppressFieldChanged: false,
            markFieldChanged: function (field) {
                self.markFieldChanged(field);
            }
        };
    };

    EditRowPopupController.prototype.renderForm = function () {
        var $ = this.$;
        var opt = this.options;
        var formSelector = opt.formSelector || "#wipForm";
        var old = $(formSelector).data("dxForm");
        if (old) old.dispose();

        $(formSelector).removeClass("wip-form-add wip-form-edit");
        $(formSelector).addClass(this.mode === "add" ? "wip-form-add" : "wip-form-edit");

        var ctx = this.createFormContext();
        var isEdit = this.mode === "edit";
        var rawItems = typeof opt.buildFormItems === "function"
            ? opt.buildFormItems(ctx, isEdit)
            : [];

        $(formSelector).dxForm({
            formData: this.formData,
            onFieldDataChanged: function (e) {
                if (!e || !e.dataField) return;
                if (ctx && ctx.suppressFieldChanged) return;
                ctx.markFieldChanged(e.dataField);
            },
            labelLocation: "top",
            showColonAfterLabel: false,
            colCount: opt.colCount || 4,
            items: this.prepareFormItems(rawItems)
        });

        return $(formSelector).dxForm("instance");
    };

    EditRowPopupController.prototype.focusFormField = function (field) {
        if (!field) return;
        var form = this.$(this.options.formSelector || "#wipForm").dxForm("instance");
        if (!form) return;

        var editor = form.getEditor(field);
        if (editor && editor.focus) editor.focus();
    };

    EditRowPopupController.prototype.save = function () {
        var $ = this.$;
        var opt = this.options;
        var popup = this.popup;
        var form = $(opt.formSelector || "#wipForm").dxForm("instance");

        if (!form) {
            if (opt.notify) opt.notify(opt.messages && opt.messages.formNotReady, "warning", 1500);
            return;
        }

        var result = form.validate();
        if (!result.isValid) return;

        var row = $.extend(true, {}, this.formData);
        Object.keys(row).forEach(function (key) {
            if (key && key.indexOf("__libId_") === 0) delete row[key];
        });

        var oldRow = typeof opt.findOldRow === "function" ? (opt.findOldRow(row) || {}) : {};
        var actualChangedFields = typeof opt.getActuallyChangedFields === "function"
            ? opt.getActuallyChangedFields(oldRow, row, this.changedFields)
            : this.changedFields;

        if (!Object.keys(actualChangedFields || {}).length) {
            if (popup) popup.hide();
            return;
        }

        var saveRequest = opt.buildSaveRequest(row, oldRow, actualChangedFields);
        if (opt.showBusy) opt.showBusy(opt.messages && opt.messages.saving);
        opt.saveByRequest(saveRequest, {
            success: function (res) {
                var items = opt.normalizeResponse(res);
                opt.ensureStepStatusForRows(items).always(function () {
                    opt.upsertRows(items);
                    opt.updateGrid(items, opt.messages && opt.messages.saved
                        ? opt.messages.saved(items.length)
                        : null,
                        "success",
                        1200);
                    if (popup) popup.hide();
                });
            },
            error: function (xhr) {
                if (opt.notify) {
                    opt.notify((opt.messages && opt.messages.saveErrorPrefix || "") + ((xhr && (xhr.responseText || xhr.statusText)) || ""), "error", 3000);
                }
            },
            complete: function () {
                if (opt.hideBusy) opt.hideBusy();
            }
        });
    };

    EditRowPopupController.prototype.init = function (selector) {
        var self = this;
        var $ = this.$;
        var opt = this.options;

        this.popup = $(selector).dxPopup($.extend(true, {
            title: opt.titleAdd,
            width: 820,
            maxHeight: "100vh",
            height: "auto",
            showTitle: true,
            visible: false,
            dragEnabled: true,
            hideOnOutsideClick: true,
            contentTemplate: function (contentElement) {
                $("<div>")
                    .attr("id", "popupScroll")
                    .css({
                        "max-height": "calc(90vh - 120px)",
                        "overflow-y": "auto",
                        "padding": "0 8px"
                    })
                    .append("<div id='wipForm'></div>")
                    .appendTo(contentElement);
            },
            onShowing: function () {
                $(opt.formSelector || "#wipForm").empty();
                self.changedFields = {};
                self.renderForm();
            },
            onShown: function () {
                setTimeout(function () {
                    var field = typeof opt.getSelectedDataField === "function" ? opt.getSelectedDataField() : null;
                    self.focusFormField(field);
                }, 0);
            },
            onHidden: function () {
                self.mode = "add";
                self.formData = self.createEmptyRow();
                if (typeof opt.setSelectedDataField === "function") opt.setSelectedDataField(null);
                if (self.popup) self.popup.option("title", opt.titleAdd);
            },
            toolbarItems: [
                {
                    widget: "dxButton",
                    toolbar: "bottom",
                    location: "after",
                    options: {
                        text: opt.saveText,
                        type: "success",
                        onClick: function () { self.save(); }
                    }
                },
                {
                    widget: "dxButton",
                    toolbar: "bottom",
                    location: "after",
                    options: {
                        text: opt.closeText,
                        type: "normal",
                        onClick: function () { if (self.popup) self.popup.hide(); }
                    }
                }
            ]
        }, opt.popupOptions || {})).dxPopup("instance");

        return this.popup;
    };

    EditRowPopupController.prototype.openForAdd = function (fieldToFocus) {
        this.mode = "add";
        this.formData = this.createEmptyRow();
        if (typeof this.options.setSelectedDataField === "function") {
            this.options.setSelectedDataField(fieldToFocus || "LineX");
        }
        if (this.popup) {
            this.popup.option("title", this.options.titleAdd);
            this.popup.show();
        }
    };

    EditRowPopupController.prototype.openForEdit = function (rowData) {
        var opt = this.options;
        if (typeof opt.hasAnyEditablePermissionForLine === "function" && !opt.hasAnyEditablePermissionForLine(rowData && rowData.LineX)) {
            if (opt.notify) opt.notify(opt.messages && opt.messages.noEditPermission, "warning", 1500);
            return;
        }

        this.mode = "edit";
        this.formData = this.$.extend(true, {}, rowData || {});
        if (this.formData.WIPId && !this.formData.id) this.formData.id = this.formData.WIPId;

        if (this.popup) {
            this.popup.option("title", opt.titleEdit);
            this.popup.show();
        }
    };

    WIP.UI.EditRowPopupController = EditRowPopupController;
})(window);
