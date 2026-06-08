(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Utils = WIP.Utils || {};

    function getDxInstance(selector, widgetName) {
        try {
            var $element = $(selector);
            if (!$element.length || typeof $element[widgetName] !== "function") return null;
            return $element[widgetName]("instance");
        } catch (e) {
            return null;
        }
    }

    function getButtonInstance(selector) {
        return getDxInstance(selector, "dxButton");
    }

    function getGridInstance(selector) {
        return getDxInstance(selector, "dxDataGrid");
    }

    function updateGridData(gridInstance, dataSource) {
        if (!gridInstance) return;
        gridInstance.option("dataSource", Array.isArray(dataSource) ? dataSource : []);
        if (typeof gridInstance.refresh === "function") {
            gridInstance.refresh();
        }
    }

    WIP.Utils.Grid = {
        getDxInstance: getDxInstance,
        getButtonInstance: getButtonInstance,
        getGridInstance: getGridInstance,
        updateGridData: updateGridData
    };
})(window, jQuery);
