(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class DateFilterController {
        constructor(deps) {
            this.deps = deps || {};
        }

        getPopoverTarget() {
            return this.deps.isCompactActionMode() && $("#btnWipMoreActions").length
                ? "#btnWipMoreActions"
                : "#btnDateFilter";
        }

        preparePopover() {
            this.deps.updateButtonState();
            this.deps.updatePopoverTitle();
            this.deps.syncPopoverContent();

            var pop = $("#popDateFilter").dxPopover("instance");
            if (!pop) return null;

            var target = this.getPopoverTarget();
            pop.option({
                target: target,
                position: { my: "top", at: "bottom", of: target }
            });

            return pop;
        }

        togglePopover() {
            if (!this.deps.hasActiveField()) {
                DevExpress.ui.notify("Chuột phải vào header cột ngày và chọn lọc theo cột này.", "warning", 2200);
                return;
            }

            var pop = this.preparePopover();
            if (!pop) return;

            if (pop.option("visible")) pop.hide();
            else pop.show();
        }

        showPopover() {
            var pop = this.preparePopover();
            if (pop) pop.show();
        }

        openForColumn(field, caption) {
            if (!field) return;

            this.deps.setActiveField(field, caption);
            this.showPopover();
        }
    }

    WIP.UI.DateFilterController = DateFilterController;
})(window, jQuery);
