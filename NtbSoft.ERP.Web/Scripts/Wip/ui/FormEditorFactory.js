(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    function FormEditorFactory(options) {
        this.options = options || {};
        this.$ = this.options.$ || window.jQuery;
        this.DevExpress = this.options.DevExpress || window.DevExpress;
    }

    FormEditorFactory.prototype.optFor = function (field, extraOpt, ctx) {
        if (field && typeof field === "object" && !Array.isArray(field)) {
            extraOpt = field;
            field = null;
        }
        extraOpt = extraOpt || {};

        var opt = this.options;
        var data = ctx && ctx.data
            ? ctx.data
            : (typeof opt.getCurrentEditRowData === "function" ? opt.getCurrentEditRowData() : {});
        var isEdit = ctx && ctx.formSelector === "#editCellForm"
            ? true
            : (typeof opt.getPopupMode === "function" && opt.getPopupMode() === "edit");
        var isLocked = typeof opt.isFixedField === "function" ? opt.isFixedField(field) : false;
        var canEdit = typeof opt.canEditFieldByPermission === "function"
            ? opt.canEditFieldByPermission(field, data && data.LineX)
            : true;

        var cls = "";
        if (isLocked) cls = "wip-editor-locked";
        else if (typeof opt.isCalcField === "function" && opt.isCalcField(field)) cls = "wip-editor-calc";
        else if (typeof opt.isInputHintField === "function" && opt.isInputHintField(field)) cls = "wip-editor-input";

        var base = {
            elementAttr: { class: cls }
        };

        if (isEdit && (!canEdit || isLocked)) {
            base.readOnly = true;
        }

        return this.$.extend(true, {}, base, extraOpt);
    };

    FormEditorFactory.prototype.safeFormUpdate = function (ctx, fn) {
        if (typeof this.options.safeFormUpdate === "function") {
            return this.options.safeFormUpdate(ctx, fn);
        }
        return fn();
    };

    FormEditorFactory.prototype.markCtxFieldChanged = function (ctx, field) {
        if (typeof this.options.markCtxFieldChanged === "function") {
            return this.options.markCtxFieldChanged(ctx, field);
        }
        if (ctx && typeof ctx.markFieldChanged === "function") {
            ctx.markFieldChanged(field);
        }
    };

    FormEditorFactory.prototype.wipLibraryPickByStyle = function (ctx, dataField, caption, apiUrl, editorOpt, extra) {
        var self = this;
        var $ = this.$;
        var DevExpress = this.DevExpress;
        var opt = this.options;
        var libraryService = opt.libraryService;
        extra = extra || {};
        var autoPick = extra.autoPick !== false;
        var compareEmpty = extra.compareEmpty !== false;
        var valueField = extra.valueField || "SMV";
        var tempIdField = extra.tempIdField || ("__libId_" + dataField);

        function getCtxData() {
            return (ctx && ctx.data) ? ctx.data : {};
        }
        function getFormInstance() {
            var sel = (ctx && ctx.formSelector) ? ctx.formSelector : "#wipForm";
            return $(sel).dxForm("instance");
        }
        function getStyleId() {
            var d = getCtxData();
            return d.StyleId || d.MaHang || null;
        }
        function getMaKH() {
            var d = getCtxData();
            return d.MaKH || null;
        }

        var store = new DevExpress.data.CustomStore({
            loadMode: "raw",
            key: "ID",
            load: function () {
                var d = getCtxData();

                return libraryService.loadPickByStyleData(
                    apiUrl,
                    getStyleId(),
                    getMaKH(),
                    valueField,
                    Number((d && d[dataField]) || 0),
                    { compareDate: true }
                ).then(function (result) {
                    var list = result.list || [];
                    var resolvedId = result.resolvedId;
                    var picked = result.pickedItem;

                    if (resolvedId != null) {
                        d[tempIdField] = resolvedId;
                        setTimeout(function () {
                            var form = getFormInstance();
                            if (form) self.safeFormUpdate(ctx, function () {
                                form.updateData(tempIdField, resolvedId);
                            });
                        }, 0);
                    }

                    if (autoPick && picked) {
                        var cur = Number((d && d[dataField]) || 0);
                        var isEmpty = !(cur > 0);

                        if (!compareEmpty || isEmpty) {
                            var v = Number(picked[valueField] || 0);

                            if (v > 0) {
                                d[dataField] = v;
                                d[tempIdField] = picked.ID;
                                self.markCtxFieldChanged(ctx, dataField);

                                setTimeout(function () {
                                    var form = getFormInstance();
                                    if (form) self.safeFormUpdate(ctx, function () {
                                        form.updateData(dataField, v);
                                        form.updateData(tempIdField, picked.ID);
                                    });
                                }, 0);
                            }
                        }
                    }

                    return list;
                });
            },
            byKey: function (key) {
                return libraryService.fetchByStyle(apiUrl, getStyleId(), getMaKH()).then(function (list) {
                    return libraryService.findById(list, key);
                });
            }
        });

        return {
            dataField: tempIdField,
            permissionField: dataField,
            label: { text: caption },
            editorType: "dxSelectBox",
            editorOptions: $.extend(true, {
                dataSource: store,
                valueExpr: "ID",
                displayExpr: function (item) {
                    if (!item) return "";
                    var value = item[valueField] != null ? item[valueField] : "";
                    var d = item.NgayApDung ? new Date(item.NgayApDung) : null;
                    var s = d ? d.toLocaleDateString("vi-VN") : "";
                    return s ? value + " (" + s + ")" : String(value);
                },
                placeholder: "Tu lay theo ma hang...",
                showClearButton: true,
                onInitialized: function (e) {
                    setTimeout(function () {
                        var ds = e.component.getDataSource();
                        if (ds) ds.reload();
                    }, 0);
                },
                onValueChanged: function (e) {
                    var d = getCtxData();
                    var item = e.component.option("selectedItem");
                    var form = getFormInstance();

                    if (!item) {
                        d[tempIdField] = null;
                        d[dataField] = null;
                        if (form) {
                            form.updateData(tempIdField, null);
                            form.updateData(dataField, null);
                        }
                        return;
                    }

                    var v = Number(item[valueField] || 0);
                    if (v > 0) {
                        d[tempIdField] = item.ID;
                        d[dataField] = v;
                        if (form) {
                            form.updateData(tempIdField, item.ID);
                            form.updateData(dataField, v);
                        }
                    }
                },
                onOpened: function (e) {
                    e.component.getDataSource().reload();
                }
            }, editorOpt || {})
        };
    };

    FormEditorFactory.prototype.wipLibrarySelect = function (ctx, dataField, caption, apiUrl, valueField, editorOpt, extra) {
        var self = this;
        var $ = this.$;
        var DevExpress = this.DevExpress;
        var opt = this.options;
        var libraryService = opt.libraryService;
        extra = extra || {};
        var sendMaChuyen = extra.sendMaChuyen !== false;
        var autoPickLatest = extra.autoPickLatest !== false;
        var compareDate = extra.compareDate !== false;
        var tempIdField = extra.tempIdField || ("__libId_" + dataField);

        function getCtxData() {
            return (ctx && ctx.data) ? ctx.data : {};
        }
        function getFormInstance() {
            var sel = (ctx && ctx.formSelector) ? ctx.formSelector : "#wipForm";
            return $(sel).dxForm("instance");
        }
        function getLineX() {
            if (ctx && typeof ctx.getLineX === "function") return ctx.getLineX();
            var d = getCtxData();
            if (d.LineX) return d.LineX;
            return typeof opt.getFallbackLineX === "function" ? opt.getFallbackLineX() : null;
        }

        var store = new DevExpress.data.CustomStore({
            loadMode: "raw",
            key: "ID",
            load: function () {
                var dataObj = getCtxData();
                var q = {};
                if (sendMaChuyen) q.MaChuyen = getLineX();
                if (dataField === "SLCN") q.action = "GetAll";

                return libraryService.loadSelectData(
                    apiUrl,
                    q,
                    valueField,
                    Number((dataObj && dataObj[dataField]) || 0),
                    { compareDate: compareDate }
                ).then(function (result) {
                    var data = result.list || [];
                    var resolvedId = result.resolvedId;
                    var pick = result.pickedItem;

                    if (resolvedId != null) {
                        dataObj[tempIdField] = resolvedId;
                        setTimeout(function () {
                            var form = getFormInstance();
                            if (form) self.safeFormUpdate(ctx, function () {
                                form.updateData(tempIdField, resolvedId);
                            });
                        }, 0);
                    }

                    if (autoPickLatest) {
                        var currentVal = Number((dataObj && dataObj[dataField]) || 0);
                        var isEmptyOrInvalid = !(currentVal > 0);

                        if (isEmptyOrInvalid && pick) {
                            var autoValue = Number(pick[valueField] || 0);
                            if (autoValue > 0) {
                                dataObj[dataField] = autoValue;
                                dataObj[tempIdField] = pick.ID;
                                self.markCtxFieldChanged(ctx, dataField);

                                setTimeout(function () {
                                    var form = getFormInstance();
                                    if (form) self.safeFormUpdate(ctx, function () {
                                        form.updateData(dataField, autoValue);
                                        form.updateData(tempIdField, pick.ID);
                                    });
                                }, 0);
                            }
                        }
                    }

                    return data;
                });
            },
            byKey: function (key) {
                var q = {};
                if (sendMaChuyen) q.MaChuyen = getLineX();
                if (dataField === "SLCN") q.action = "GetAll";

                return libraryService.fetchLibrary(apiUrl, q).then(function (data) {
                    return libraryService.findById(data, key);
                });
            }
        });

        return {
            dataField: tempIdField,
            permissionField: dataField,
            label: { text: caption },
            editorType: "dxSelectBox",
            editorOptions: $.extend(true, {
                dataSource: store,
                valueExpr: "ID",
                displayExpr: function (item) {
                    if (!item) return "";
                    var val = item[valueField];
                    var d = item.NgayApDung ? new Date(item.NgayApDung) : null;
                    var s = d ? d.toLocaleDateString("vi-VN") : "";
                    return val + " (" + s + ")";
                },
                showClearButton: true,
                placeholder: "Chon tu thu vien...",
                onValueChanged: function (e) {
                    var item = e.component.option("selectedItem");
                    var dataObj = getCtxData();
                    var form = getFormInstance();

                    if (!item) {
                        dataObj[tempIdField] = null;
                        dataObj[dataField] = null;
                        if (form) {
                            form.updateData(tempIdField, null);
                            form.updateData(dataField, null);
                        }
                        return;
                    }

                    var v = Number(item[valueField] || 0);
                    dataObj[tempIdField] = item.ID;
                    dataObj[dataField] = v;

                    if (form) {
                        form.updateData(tempIdField, item.ID);
                        form.updateData(dataField, v);
                    }
                },
                onInitialized: function (e) {
                    setTimeout(function () {
                        var ds = e.component.getDataSource();
                        if (ds) ds.reload();
                    }, 0);
                },
                onOpened: function (e) {
                    e.component.getDataSource().reload();
                }
            }, editorOpt || {})
        };
    };

    FormEditorFactory.prototype.buildEditorForCell = function (ctx, field, caption) {
        var opt = this.options;
        var editorOpt = this.optFor(field, null, ctx);

        if (field === "SLCN") {
            return this.wipLibrarySelect(ctx, "SLCN", opt.getFieldCaption("SLCN"), "/api/ThuVienWip/nang-luc-chuyen", "SLCN", editorOpt, { sendMaChuyen: true });
        }
        if (field === "TGLV") {
            return this.wipLibrarySelect(ctx, "TGLV", opt.getFieldCaption("TGLV"), "/api/ThuVienWip/thoi-gian-lam-viec", "SoGio", editorOpt, { sendMaChuyen: true });
        }
        if (field === "Profit") {
            return this.wipLibrarySelect(ctx, "Profit", opt.getFieldCaption("Profit"), "/api/ThuVienWip/he-so-loi-nhuan", "HeSo", editorOpt, { sendMaChuyen: false });
        }
        if (field === "SMV_WIP") {
            return this.wipLibraryPickByStyle(ctx, "SMV_WIP", opt.getFieldCaption("SMV_WIP"), "/api/ThuVienWip/smv", editorOpt, { autoPick: true, compareEmpty: true, valueField: "SMV" });
        }
        if (field === "CM") {
            return this.wipLibraryPickByStyle(ctx, "CM", opt.getFieldCaption("CM"), "/api/ThuVienWip/get-cm", editorOpt, { autoPick: true, compareEmpty: true, valueField: "CM", tempIdField: "__libId_CM" });
        }
        if (field === "Mer") {
            return opt.merSelect("Mer", opt.getFieldCaption("Mer"), editorOpt);
        }

        var row = ctx && ctx.data ? ctx.data : null;
        var editorType = opt.getFieldEditorType(field, row);
        if (editorType === "date") {
            if (row && opt.isEmptyDateValue(row[field])) {
                row[field] = opt.getTodayDateOnly();
            }
            return opt.date(field, caption, editorOpt);
        }
        if (editorType === "bool") {
            return opt.bool(field, caption, editorOpt);
        }
        if (editorType === "number") {
            return opt.num(field, caption, editorOpt);
        }
        return opt.text(field, caption, editorOpt);
    };

    WIP.UI.FormEditorFactory = FormEditorFactory;
})(window);
