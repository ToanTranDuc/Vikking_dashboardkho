(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class LineTabsController {
        constructor(options) {
            this.options = options || {};
            this.tabs = null;
            this.initLocked = true;
        }

        init(selector) {
            this.tabs = $(selector).dxTabs({
                items: [],
                selectedIndex: 0,
                scrollByContent: true,
                showNavButtons: true,
                selectionMode: "single",
                onSelectionChanged: async (e) => {
                    if (this.initLocked) return;

                    var item = e.addedItems && e.addedItems[0] ? e.addedItems[0] : null;
                    var selection = this.options.lineState.selectItem(item);
                    this.options.onSelectionChanged(selection, item);
                }
            }).dxTabs("instance");

            return this.tabs;
        }

        setItems(items) {
            if (this.tabs) this.tabs.option("items", items || []);
        }

        setSelectedIndex(index) {
            if (this.tabs) this.tabs.option("selectedIndex", index);
        }

        async load(isFirstLoad) {
            return this.options.lineService.loadLineXList({}, {
                success: async (response) => {
                    var lines = this.options.lineState.normalizeLines(response);
                    var items = this.options.lineState.setItems(this.options.lineState.buildTabItems(lines));

                    this.initLocked = true;
                    this.setItems(items);

                    if (isFirstLoad) {
                        this.options.lineState.ensureDefault(this.options.defaultLineX);
                    }

                    var idx = this.options.lineState.getSelectedIndex();
                    this.setSelectedIndex(idx);

                    var selection = this.options.lineState.selectIndex(idx);
                    this.initLocked = false;

                    await this.options.afterLoad(selection, lines, isFirstLoad);
                },
                error: function (xhr) {
                    DevExpress.ui.notify(
                        "Load chuyền lỗi: " + (xhr.responseText || xhr.statusText),
                        "error",
                        2500
                    );
                }
            });
        }
    }

    WIP.UI.LineTabsController = LineTabsController;
})(window, jQuery);
