(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Core = WIP.Core || {};

    WIP.Core.bootstrapWipDonHang = async function () {
        if (WIP.Core.wipDonHangPage) return WIP.Core.wipDonHangPage;

        var state = new WIP.Core.WipState();
        var page = new WIP.Core.WipDonHangPage({ state: state });
        WIP.Core.wipDonHangPage = page;
        await page.init();
        return page;
    };

    $(function () {
        if (window.WIP_DISABLE_AUTO_BOOTSTRAP === true) return;

        WIP.Core.bootstrapWipDonHang().catch(function (err) {
            console.error(err);
            if (window.DevExpress && DevExpress.ui) {
                DevExpress.ui.notify("Init lỗi: " + ((err && err.message) || err), "error", 3000);
            }
        });
    });
})(window, jQuery);
