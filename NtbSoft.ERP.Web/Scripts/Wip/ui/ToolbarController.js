(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class ToolbarController {
        constructor(options) {
            this.options = options || {};
            this.moreActionSheet = null;
        }

        getButtonInstance(selector) {
            if (WIP.Utils && WIP.Utils.Grid && typeof WIP.Utils.Grid.getButtonInstance === "function") {
                return WIP.Utils.Grid.getButtonInstance(selector);
            }

            try {
                var $el = $(selector);
                if (!$el.length) return null;
                return $el.dxButton("instance");
            } catch (e) {
                return null;
            }
        }

        initButton(selector, options) {
            return $(selector).dxButton(options || {}).dxButton("instance");
        }

        ensureButtonHost(options) {
            var opt = options || {};
            if ($(opt.selector).length) return;

            var id = String(opt.selector || "").replace(/^#/, "");
            var $host = $("<div>").attr("id", id);
            var $after = $(opt.insertAfterSelector);
            if ($after.length) $host.insertAfter($after);
            else $(opt.appendToSelector || "body").first().append($host);
        }

        initMoreActionButton(options) {
            var opt = options || {};
            this.ensureButtonHost({
                selector: opt.buttonSelector,
                insertAfterSelector: opt.insertAfterSelector,
                appendToSelector: opt.appendToSelector
            });

            this.initButton(opt.buttonSelector, {
                text: opt.text || "More",
                icon: opt.icon || "menu",
                type: opt.type || "normal",
                stylingMode: opt.stylingMode || "contained",
                visible: false,
                onClick: () => {
                    var items = this.buildActionItems(opt.definitions);
                    if (!items.length) return;

                    var sheet = this.ensureMoreActionSheet(opt.sheetSelector || "#wipMoreActionSheet", opt.sheetTitle || "More action");
                    sheet.option("items", items);
                    this.enableActionSheetOutsideClose(sheet);
                    sheet.show();
                }
            });

            this.syncMoreActionButton({
                buttonSelector: opt.buttonSelector,
                definitions: opt.definitions,
                isCompact: opt.isCompact,
                text: opt.text || "More"
            });
        }

        invokeButtonClick(selector) {
            var btn = this.getButtonInstance(selector);
            if (!btn || btn.option("disabled") === true) return;

            var onClick = btn.option("onClick");
            if (typeof onClick === "function") {
                onClick.call(btn, {
                    component: btn,
                    element: $(selector).get(0),
                    event: null
                });
            }
        }

        buildActionItems(definitions) {
            return (definitions || []).reduce((items, def) => {
                var btn = this.getButtonInstance(def.selector);
                if (!btn || btn.option("visible") === false) return items;

                items.push({
                    text: def.text,
                    icon: def.icon,
                    disabled: btn.option("disabled") === true,
                    _selector: def.selector,
                    _action: () => {
                        this.invokeButtonClick(def.selector);
                    }
                });
                return items;
            }, []);
        }

        ensureMoreActionSheet(selector, title) {
            if (this.moreActionSheet) return this.moreActionSheet;

            var $sheet = $(selector);
            if (!$sheet.length) {
                $sheet = $("<div id='" + String(selector || "").replace(/^#/, "") + "'></div>").appendTo("body");
            }

            this.moreActionSheet = $sheet.dxActionSheet({
                usePopover: false,
                showTitle: true,
                title: title || "More action",
                showCancelButton: false,
                onItemClick: (e) => {
                    var action = e && e.itemData ? e.itemData._action : null;
                    try { this.moreActionSheet && this.moreActionSheet.hide(); } catch (ex) { }
                    if (typeof action === "function") action();
                }
            }).dxActionSheet("instance");

            return this.moreActionSheet;
        }

        enableActionSheetOutsideClose(actionSheet) {
            try {
                if (actionSheet && actionSheet._popup) {
                    actionSheet._popup.option("hideOnOutsideClick", true);
                }
            } catch (e) { }
        }

        syncMoreActionButton(options) {
            var opt = options || {};
            var btn = this.getButtonInstance(opt.buttonSelector);
            if (!btn) return;

            var items = this.buildActionItems(opt.definitions);
            btn.option({
                text: opt.isCompact ? "" : (opt.text || "More"),
                visible: !!opt.isCompact && items.length > 0,
                disabled: !items.length
            });

            if (this.moreActionSheet) {
                this.moreActionSheet.option("items", items);
            }
        }
    }

    WIP.UI.ToolbarController = ToolbarController;
})(window, jQuery);
