(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Grid = WIP.Grid || {};

    class WipGridController {
        constructor(options) {
            this.options = options || {};
        }

        create(selector, dataSource, columns, extraOptions) {
            var options = $.extend(true, {
                dataSource: Array.isArray(dataSource) ? dataSource : [],
                keyExpr: "id",
                showBorders: true,
                allowColumnResizing: true,
                columnAutoWidth: false,
                wordWrapEnabled: true,
                rowAlternationEnabled: true,
                hoverStateEnabled: true,
                paging: { enabled: false },
                scrolling: { mode: "standard" },
                columnResizingMode: "widget",
                focusedRowIndex: -1,
                focusedRowEnabled: false,
                focusStateEnabled: false,
                sorting: { mode: "none" },
                editing: {
                    mode: "cell",
                    allowUpdating: true,
                    selectTextOnEditStart: true,
                    startEditAction: "dblClick"
                },
                filterRow: {
                    visible: true,
                    applyFilter: "auto"
                },
                columns: columns || []
            }, extraOptions || {});

            var gridInstance = $(selector).dxDataGrid(options).dxDataGrid("instance");
            this.setMainGrid(gridInstance);
            return gridInstance;
        }

        updateGrid(gridInstance, dataSource, notifyMsg, notifyType, notifyTime) {
            if (!gridInstance) return;

            var resolvedDataSource = Array.isArray(dataSource) ? dataSource : [];
            if (gridInstance === this.options.mainGrid && typeof this.options.filterData === "function") {
                resolvedDataSource = this.options.filterData(resolvedDataSource);
            }

            gridInstance.option("dataSource", resolvedDataSource);
            if (typeof gridInstance.refresh === "function") {
                gridInstance.refresh();
            }

            if (notifyMsg && window.DevExpress && DevExpress.ui) {
                DevExpress.ui.notify(notifyMsg, notifyType || "success", notifyTime || 1000);
            }
        }

        updateDimensions(gridInstance) {
            try {
                if (gridInstance && typeof gridInstance.updateDimensions === "function") {
                    gridInstance.updateDimensions();
                }
            } catch (e) { }
        }

        setMainGrid(gridInstance) {
            this.options.mainGrid = gridInstance || null;
        }
    }

    WIP.Grid.WipGridController = WipGridController;
})(window, jQuery);
