(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class EditCellPopupController {
        constructor(deps) {
            this.deps = deps || {};
        }

        save() {
            var context = this.deps.getContext();
            var row = context.row;
            var field = context.field;
            if (!row || !field) return;

            var form = $("#editCellForm").dxForm("instance");
            if (!form) {
                DevExpress.ui.notify("Form chưa sẵn sàng!", "warning", 1500);
                return;
            }

            var validationResult = form.validate();
            if (!validationResult.isValid) return;

            var formData = form.option("formData") || {};
            var tempIdField = "__libId_" + field;
            var isLibraryEditor = (tempIdField in formData) || (tempIdField in row);

            if (!isLibraryEditor) {
                row[field] = formData[field];
            }

            Object.keys(row).forEach(function (key) {
                if (key && key.indexOf("__libId_") === 0) delete row[key];
            });

            var rows = this.deps.getRows();
            var oldRow = rows.find(function (item) {
                return item && (item.WIPId === row.WIPId || item.id === row.id);
            }) || {};

            if (!this.deps.isFieldValueChanged(oldRow, row, field)) {
                this.deps.getPopup().hide();
                return;
            }

            var changedObj = {};
            changedObj[field] = true;
            var saveRequest = this.deps.buildSaveRequest(row, oldRow, changedObj);

            this.deps.showBusy("Đang lưu...");
            this.deps.saveByRequest(saveRequest, {
                success: (savedRow) => {
                    var items = this.deps.normalizeResponse(savedRow);
                    this.deps.ensureStepStatusForRows(items).always(() => {
                        this.deps.upsertRows(rows, items);
                        this.deps.updateGrid(rows, "Đã cập nhật (" + items.length + " dòng)", "success", 1000);
                        this.deps.getPopup().hide();
                    });
                },
                error: function (xhr) {
                    DevExpress.ui.notify("Lưu lỗi: " + (xhr.responseText || xhr.statusText), "error", 3000);
                },
                complete: () => {
                    this.deps.hideBusy();
                }
            });
        }
    }

    WIP.UI.EditCellPopupController = EditCellPopupController;
})(window, jQuery);
