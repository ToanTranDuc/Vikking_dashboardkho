(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class ResponsiveController {
        constructor(options) {
            this.options = $.extend({
                compactQuery: "(max-width: 991.98px)",
                compactWidth: 992
            }, options || {});
        }

        isCompactActionMode() {
            try {
                return !!(window.matchMedia && window.matchMedia(this.options.compactQuery).matches);
            } catch (e) {
                return window.innerWidth < this.options.compactWidth;
            }
        }

        getUnassignedButtonText(isExpanded) {
            if (this.isCompactActionMode()) return isExpanded ? "Ẩn DS" : "Hiện DS";
            return isExpanded ? "Ẩn danh sách" : "Danh sách chưa chia kế hoạch";
        }

        getEndedButtonText(showEnded) {
            if (this.isCompactActionMode()) return showEnded ? "Ẩn ĐKT" : "Đơn KT";
            return showEnded ? "Ẩn đơn đã kết thúc" : "Đơn đã kết thúc";
        }

        getLoadButtonText() {
            return this.isCompactActionMode() ? "Load lại" : "Tải dữ liệu";
        }

        applyBodyClass(className) {
            $("body").toggleClass(className || "wip-compact-actions", this.isCompactActionMode());
        }

        applyHeaderAndActions(options) {
            var opt = options || {};
            var compact = this.isCompactActionMode();
            this.applyBodyClass(opt.bodyClass || "wip-compact-actions");

            var titleState = opt.titleState || {};
            var $layoutTitle = $(opt.titleSelector);
            if ($layoutTitle.length) {
                if (titleState.titleText === null || titleState.titleText === undefined) {
                    titleState.titleText = $.trim($layoutTitle.text()) || "PRODUCTION MANAGEMENT SYSTEM";
                }
                $layoutTitle.text(compact ? "" : titleState.titleText);
            }

            var $layoutSubTitle = $(opt.subTitleSelector);
            if ($layoutSubTitle.length) {
                if (titleState.subTitleText === null || titleState.subTitleText === undefined) {
                    titleState.subTitleText = $.trim($layoutSubTitle.text());
                }
                $layoutSubTitle.text(compact ? "" : titleState.subTitleText);
            }

            if (typeof opt.updateButtons === "function") {
                opt.updateButtons(compact);
            }

            return titleState;
        }

        schedule(timerId, callback, delayMs) {
            if (timerId) clearTimeout(timerId);
            return setTimeout(function () {
                callback();
            }, delayMs || 80);
        }
    }

    WIP.UI.ResponsiveController = ResponsiveController;
})(window, jQuery);
