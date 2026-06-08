(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Grid = WIP.Grid || {};

    function createRowPreparedHandler() {
        return function (e) {
            if (!e || e.rowType !== "data" || !e.rowElement) return;
            var key = Number((e.data && (e.data.WIPId || e.data.id)) || 0);
            $(e.rowElement).attr("data-wip-id", Number.isFinite(key) ? key : 0);
        };
    }

    function createCellPreparedHandler(deps) {
        deps = deps || {};

        return function (e) {
            var preparedField = e.column && e.column.dataField ? e.column.dataField : "";
            if ((e.rowType === "header" || e.rowType === "filter") && preparedField) {
                $(e.cellElement).attr("data-field", preparedField);
            }
            if ((e.rowType === "header" || e.rowType === "filter") && e.column.headerCssClass) {
                $(e.cellElement).addClass(e.column.headerCssClass);
            }
            if (e.rowType !== "data") return;

            var field = preparedField;
            var $cell = $(e.cellElement);
            $cell.attr("data-field", field || "");
            $cell.removeClass("tt-bom-status-1 tt-bom-status-2 tt-bom-status-3").removeAttr("title");

            var statusApplied = false;
            if (field && deps.isStepStatusComparableField(field)) {
                statusApplied = deps.applyStepStatusCellStyle($cell, e.data, field);
            }

            if (field && !statusApplied && deps.isManualCell(e.data, field)) {
                $(e.cellElement).addClass("cell-manual");
            } else {
                $(e.cellElement).removeClass("cell-manual");
            }
        };
    }

    function createCellClickHandler(deps) {
        deps = deps || {};

        return function (e) {
            if (e.rowType === "data") {
                deps.setSelectedDataField(e.column && e.column.dataField ? e.column.dataField : null);
            } else {
                deps.setSelectedDataField(null);
            }
        };
    }

    function createMobileRowClickHandler(deps) {
        deps = deps || {};

        return function (e) {
            if (!deps.isMobileUiMode() || !e || e.rowType !== "data" || !e.data) return;
            if (deps.getMobileHoldTriggered()) {
                deps.setMobileHoldTriggered(false);
                return;
            }

            var field = deps.getSelectedDataField()
                || (e.column && e.column.dataField ? e.column.dataField : null)
                || "MaHang";
            if (deps.isFixedField(field)) return;
            deps.openMobileRowMenu(e.data, field);
        };
    }

    function createContextMenuPreparingHandler(deps) {
        deps = deps || {};

        return function (e) {
            if (deps.isMobileUiMode()) {
                e.items = [];
                return;
            }
            e.items = e.items || [];

            if (e.target === "header") {
                var headerField = e.column && e.column.dataField ? e.column.dataField : null;
                e.items = e.items.concat(deps.buildDateColumnMenuItems(headerField, e.column));
                return;
            }

            if (e.target !== "content" || !e.row || e.row.rowType !== "data") return;

            var field = e.column && e.column.dataField ? e.column.dataField : null;
            e.items = e.items.concat(deps.buildGridRowMenuItems(e.row.data, field));
        };
    }

    function createEditorPreparingHandler(deps) {
        deps = deps || {};

        return function (e) {
            if (e.parentType === "filterRow") {
                e.editorOptions.updateValueTimeout = 600;
                if ((e.parentType === "filterRow" || e.parentType === "dataRow") && e.dataField === "Mer") {
                    e.editorOptions.dropDownOptions = $.extend(true, {}, e.editorOptions.dropDownOptions, {
                        width: 80
                    });
                }
            }
            if (e.parentType !== "dataRow") return;

            var field = e.dataField;
            var rowLineX = e.row && e.row.data ? e.row.data.LineX : null;
            if (!deps.canEditFieldByPermission(field, rowLineX) || deps.isFixedField(field)) {
                e.editorOptions.readOnly = true;
                e.editorOptions.disabled = true;
            }
            if (field === "ThuTuChuyen") {
                e.editorOptions.readOnly = true;
                e.editorOptions.disabled = true;
            }

            var fieldMaxLength = deps.getFieldMaxLength(field);
            if (fieldMaxLength) {
                e.editorOptions.maxLength = fieldMaxLength;
            }

            if (e.editorName === "dxDateBox" || e.dataType === "date") {
                e.editorOptions = deps.buildStrictDateEditorOptions(e.editorOptions);

                var oldOnInitialized = e.editorOptions.onInitialized;
                e.editorOptions.onInitialized = function (args) {
                    if (typeof oldOnInitialized === "function") oldOnInitialized(args);

                    var value = args.component.option("value");
                    if (deps.isEmptyDateValue(value)) {
                        args.component.option("value", deps.getTodayDateOnly());
                    }
                };
            }

            if (deps.isNonNegField(field)) {
                e.editorOptions.min = 0;

                e.editorOptions.onKeyDown = function (args) {
                    var ev = args.event;
                    if (!ev) return;

                    if (ev.key === "-" || ev.key === "Subtract") {
                        ev.preventDefault();
                    }
                };

                e.editorOptions.onInput = function (args) {
                    var value = args.component.option("value");
                    if (typeof value === "number" && value < 0) {
                        args.component.option("value", 0);
                    }
                };

                var oldChanged = e.editorOptions.onValueChanged;
                e.editorOptions.onValueChanged = function (args) {
                    if (typeof args.value === "number" && args.value < 0) {
                        args.component.option("value", 0);
                        return;
                    }
                    if (typeof oldChanged === "function") {
                        oldChanged(args);
                    }
                };
            }
        };
    }

    function createUnassignedSelectionChangedHandler(deps) {
        deps = deps || {};

        return function (e) {
            var selected = e.selectedRowsData || [];
            var btnInstance = deps.getCrossButtonInstance();
            if (!selected.length) {
                if (btnInstance) btnInstance.option("visible", false);
                deps.setBaseline(null);
                deps.setLastGoodKeys([]);
                return;
            }

            var baseline = deps.getBaseline();
            if (!baseline || !selected.some(function (row) { return deps.matchBaseline(row, baseline); })) {
                baseline = deps.pickBaseline(selected[0]);
                deps.setBaseline(baseline);
            }

            var bad = selected.filter(function (row) {
                return !deps.isRowCompatibleWithBaseline(row, baseline);
            });

            if (bad.length) {
                var toRemove = (e.currentSelectedRowKeys || []).filter(function (key) {
                    var row = selected.find(function (item) {
                        return String(item.id) === String(key);
                    });
                    return row && !deps.isRowCompatibleWithBaseline(row, baseline);
                });

                if (!toRemove.length) {
                    toRemove = bad.map(function (row) { return row.id; });
                }

                if (window.DevExpress && DevExpress.ui) {
                    DevExpress.ui.notify("Chỉ được chọn các dòng cùng Mã hàng.", "warning", 2200);
                }

                var keepKeys = selected
                    .filter(function (row) { return toRemove.indexOf(row.id) < 0; })
                    .map(function (row) { return row.id; });

                e.component.selectRows(keepKeys, false);
                return;
            }

            if (btnInstance) btnInstance.option("visible", true);
            deps.setLastGoodKeys(selected.map(function (row) { return row.id; }));
        };
    }

    function createSavingHandler(deps) {
        deps = deps || {};

        return function (e) {
            if (deps.getIsGridSaving()) {
                e.cancel = true;
                return;
            }
            if (!e.changes || !e.changes.length) return;

            var change = e.changes[0];
            if (change.type !== "update") return;

            var rows = deps.getRows();
            var oldRow = rows.find(function (row) {
                return row && row.id === change.key;
            }) || {};
            var newRow = $.extend(true, {}, oldRow, change.data);

            e.cancel = true;
            var deferred = $.Deferred();
            e.promise = deferred.promise();

            deps.setIsGridSaving(true);
            var changedFieldsObj = {};
            Object.keys(change.data || {}).forEach(function (key) {
                if (!key || key.indexOf("__libId_") === 0) return;
                changedFieldsObj[key] = true;
            });

            var saveRequest = deps.buildSaveRequest(newRow, oldRow, changedFieldsObj);
            deps.showBusy("Đang lưu...");
            deps.saveByRequest(saveRequest, {
                success: function (savedRow) {
                    var items = deps.normalizeResponse(savedRow);
                    deps.ensureStepStatusForRows(items).always(function () {
                        deps.upsertRows(rows, items);
                        e.component.cancelEditData();
                        deps.updateGrid(e.component, rows, "Đã lưu", "success", 800);
                        deps.setIsGridSaving(false);
                        deferred.resolve();
                    });
                },
                error: function (xhr) {
                    if (window.DevExpress && DevExpress.ui) {
                        DevExpress.ui.notify("Lưu lỗi: " + (xhr.responseText || xhr.statusText), "error", 3000);
                    }

                    e.component.cancelEditData();
                    deps.setIsGridSaving(false);
                    deferred.reject();
                },
                complete: function () {
                    deps.hideBusy();
                }
            });
        };
    }

    function createMainRowDraggingOptions(deps) {
        deps = deps || {};

        return {
            group: deps.group,
            allowReordering: false,
            showDragIcons: false,
            dragDirection: "vertical",
            onDragStart: function (e) {
                if (!deps.isSortMode()) {
                    e.cancel = true;
                }
            },
            onDragChange: function (e) {
                if (!deps.isSortMode()) {
                    e.cancel = true;
                    return;
                }

                if (!deps.getSelectedLineX()) {
                    e.cancel = true;
                    return;
                }

                var toRow = e.component.getVisibleRows()[e.toIndex];
                if (toRow && toRow.data && String(toRow.data.LineX || "") !== String(e.itemData.LineX || "")) {
                    e.cancel = true;
                    if (window.DevExpress && DevExpress.ui) {
                        DevExpress.ui.notify("Chỉ được kéo thả trong cùng 1 chuyền và đã chia kế hoạch", "warning", 1500);
                    }
                }
            },
            onAdd: function (e) {
                if (!deps.isSortMode()) {
                    e.cancel = true;
                }
            },
            onReorder: deps.onReorder
        };
    }

    WIP.Grid.WipGridEvents = {
        createRowPreparedHandler: createRowPreparedHandler,
        createCellPreparedHandler: createCellPreparedHandler,
        createCellClickHandler: createCellClickHandler,
        createMobileRowClickHandler: createMobileRowClickHandler,
        createContextMenuPreparingHandler: createContextMenuPreparingHandler,
        createEditorPreparingHandler: createEditorPreparingHandler,
        createUnassignedSelectionChangedHandler: createUnassignedSelectionChangedHandler,
        createSavingHandler: createSavingHandler,
        createMainRowDraggingOptions: createMainRowDraggingOptions
    };
})(window, jQuery);
