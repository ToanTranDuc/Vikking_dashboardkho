(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Grid = WIP.Grid || {};

    class UnassignedGridController {
        constructor(options) {
            this.options = options || {};
        }

        create(selector, dataSource, extraOptions) {
            var columnsProvider = this.options.columnsProvider;
            var columns = typeof columnsProvider === "function"
                ? columnsProvider()
                : [];

            var options = $.extend(true, {
                dataSource: Array.isArray(dataSource) ? dataSource : [],
                keyExpr: "id",
                showBorders: true,
                allowColumnResizing: true,
                columnAutoWidth: true,
                filterRow: {
                    visible: true,
                    applyFilter: "auto"
                },
                wordWrapEnabled: true,
                rowAlternationEnabled: true,
                hoverStateEnabled: true,
                paging: { enabled: false },
                scrolling: { mode: "standard" },
                columnResizingMode: "widget",
                focusedRowIndex: -1,
                focusedRowEnabled: false,
                focusStateEnabled: false,
                editing: { mode: "cell", allowUpdating: false },
                columns: columns,
                selection: {
                    mode: "multiple",
                    showCheckBoxesMode: "always",
                    deferred: false
                }
            }, extraOptions || {});

            return $(selector).dxDataGrid(options).dxDataGrid("instance");
        }
    }

    WIP.Grid.UnassignedGridController = UnassignedGridController;
})(window, jQuery);
