(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.UI = WIP.UI || {};

    class ColumnFilterPopupController {
        constructor(deps) {
            this.deps = deps || {};
            this.popup = null;
            this.tree = null;
            this.treeItems = [];
            this.leafFields = [];
            this.saving = false;
            this.rootId = 0;
        }

        notify(message, type, time) {
            if (this.deps.notify) {
                this.deps.notify(message, type, time);
                return;
            }
            DevExpress.ui.notify(message, type, time);
        }

        getStorageKey() {
            if (typeof this.deps.getStorageKey === "function") {
                return this.deps.getStorageKey();
            }

            var prefix = this.deps.storageKeyPrefix || "wip-donhang-hidden-columns";
            var userName = "";

            if (typeof this.deps.getUserName === "function") {
                userName = this.deps.getUserName();
            } else if (this.deps.userName !== undefined && this.deps.userName !== null) {
                userName = this.deps.userName;
            }

            userName = String(userName || "").trim() || "anonymous";
            return prefix + "-" + userName;
        }

        normalizeHiddenFields(fields) {
            var map = {};
            var result = [];

            (Array.isArray(fields) ? fields : []).forEach(function (field) {
                field = String(field || "").trim();
                if (!field || map[field]) return;

                map[field] = true;
                result.push(field);
            });

            return result;
        }

        loadHiddenFields() {
            try {
                return this.normalizeHiddenFields(JSON.parse(localStorage.getItem(this.getStorageKey()) || "[]"));
            } catch (e) {
                return [];
            }
        }

        saveHiddenFields(fields) {
            localStorage.setItem(this.getStorageKey(), JSON.stringify(this.normalizeHiddenFields(fields)));
        }

        buildHiddenFieldMap() {
            var map = {};
            this.loadHiddenFields().forEach(function (field) {
                map[field] = true;
            });
            return map;
        }

        isUserColumnHidden(field) {
            field = String(field || "").trim();
            return !!(field && this.buildHiddenFieldMap()[field]);
        }

        normalizeCaption(value) {
            return String(value || "").replace(/^\s*(?:\(\d+\)|\d+)\s+/, "").trim();
        }

        buildTreeData(columns) {
            var hiddenMap = this.buildHiddenFieldMap();
            var items = [];
            var leafFields = [];
            var groupMap = {};
            var fieldMap = {};
            var self = this;

            function ensureGroup(path) {
                var parentId = self.rootId;
                var currentPath = [];

                (path || []).forEach(function (caption) {
                    caption = self.normalizeCaption(caption);
                    if (!caption) return;

                    currentPath.push(caption);
                    var id = "group:" + currentPath.join(" / ");

                    if (!groupMap[id]) {
                        groupMap[id] = true;
                        items.push({
                            id: id,
                            parentId: parentId,
                            text: caption,
                            isLeaf: false,
                            expanded: false
                        });
                    }

                    parentId = id;
                });

                return parentId;
            }

            function walk(cols, path) {
                (cols || []).forEach(function (column) {
                    if (!column) return;

                    var hasChildren = Array.isArray(column.columns) && column.columns.length > 0;
                    var caption = self.normalizeCaption(column.caption) || String(column.dataField || "");
                    var nextPath = caption ? path.concat(caption) : path;

                    if (hasChildren) {
                        if (column.visible === false) return;
                        walk(column.columns, nextPath);
                        return;
                    }

                    if (!column.dataField || column.visible === false || fieldMap[column.dataField]) return;

                    fieldMap[column.dataField] = true;
                    leafFields.push(column.dataField);

                    items.push({
                        id: "field:" + column.dataField,
                        parentId: ensureGroup(path),
                        text: caption || column.dataField,
                        field: column.dataField,
                        isLeaf: true,
                        selected: hiddenMap[column.dataField] !== true
                    });
                });
            }

            walk(columns, []);

            return {
                items: items,
                leafFields: leafFields
            };
        }

        applyUserColumnVisibility(columns) {
            var hiddenMap = this.buildHiddenFieldMap();

            function walk(items) {
                (items || []).forEach(function (column) {
                    if (!column) return;

                    var hasChildren = Array.isArray(column.columns) && column.columns.length > 0;
                    if (hasChildren) {
                        walk(column.columns);

                        if (!column.columns.some(function (child) {
                            return child && child.visible !== false;
                        })) {
                            column.visible = false;
                        }
                        return;
                    }

                    if (!column.dataField || column.visible === false) return;

                    if (hiddenMap[column.dataField]) {
                        column.visible = false;
                    }
                });
            }

            walk(columns);
            return columns;
        }

        ensureButtonElement() {
            if ($("#btnColumnFilter").length) return;

            var $buttonHost = $('<div id="btnColumnFilter"></div>');
            var $dateButton = $("#btnDateFilter");
            var $headerColorButton = $("#btnHeaderColorConfig");
            var $topbarRight = $(".topbar-right").first();

            if ($headerColorButton.length) {
                $buttonHost.insertAfter($headerColorButton);
            } else if ($dateButton.length) {
                $buttonHost.insertBefore($dateButton);
            } else if ($topbarRight.length) {
                $topbarRight.append($buttonHost);
            } else {
                $("body").append($buttonHost);
            }
        }

        ensurePopup() {
            var self = this;
            if (this.popup) return;

            if (!$("#popupColumnFilter").length) {
                $("body").append(
                    '<div id="popupColumnFilter">' +
                    '  <div class="wip-column-filter-popup" style="height:100%;">' +
                    '    <div id="treeColumnFilter" style="height:100%;"></div>' +
                    '  </div>' +
                    '</div>'
                );
            }

            this.tree = $("#treeColumnFilter").dxTreeView({
                dataSource: [],
                height: "100%",
                dataStructure: "plain",
                rootValue: this.rootId,
                keyExpr: "id",
                parentIdExpr: "parentId",
                displayExpr: "text",
                selectedExpr: "selected",
                selectionMode: "multiple",
                showCheckBoxesMode: "normal",
                selectNodesRecursive: true,
                selectByClick: true,
                searchEnabled: true,
                searchExpr: ["text", "field"],
                noDataText: "Không có cột để chọn"
            }).dxTreeView("instance");

            this.popup = $("#popupColumnFilter").dxPopup($.extend(true, {
                title: "Chọn cột hiển thị",
                width: 520,
                height: 650,
                maxHeight: "90vh",
                showCloseButton: true,
                dragEnabled: true,
                deferRendering: false,
                onShown: function () {
                    try { if (self.tree) self.tree.repaint(); } catch (e) { }
                },
                toolbarItems: [
                    {
                        widget: "dxButton",
                        toolbar: "bottom",
                        location: "before",
                        options: {
                            text: "Chọn tất cả",
                            onClick: function () {
                                self.setSelection(true);
                            }
                        }
                    },
                    {
                        widget: "dxButton",
                        toolbar: "bottom",
                        location: "before",
                        options: {
                            text: "Bỏ chọn tất cả",
                            onClick: function () {
                                self.setSelection(false);
                            }
                        }
                    },
                    {
                        widget: "dxButton",
                        toolbar: "bottom",
                        location: "after",
                        options: {
                            text: "Lưu",
                            type: "default",
                            onClick: function () {
                                self.save();
                            }
                        }
                    }
                ]
            }, this.deps.popupOptions ? this.deps.popupOptions() : {})).dxPopup("instance");
        }

        setSelection(isSelected) {
            if (!this.tree) return;

            (this.leafFields || []).forEach(function (field) {
                try {
                    if (isSelected) {
                        this.tree.selectItem("field:" + field);
                    } else {
                        this.tree.unselectItem("field:" + field);
                    }
                } catch (e) { }
            }, this);
        }

        getSelectedFieldMap() {
            var selectedMap = {};
            if (!this.tree) return selectedMap;

            function walk(nodes) {
                (nodes || []).forEach(function (node) {
                    var item = node && node.itemData ? node.itemData : null;
                    if (item && item.isLeaf && item.field && node.selected) {
                        selectedMap[item.field] = true;
                    }
                    if (node && node.children && node.children.length) {
                        walk(node.children);
                    }
                });
            }

            walk(this.tree.getNodes());
            return selectedMap;
        }

        setSavingState(isSaving) {
            this.saving = !!isSaving;

            try {
                if (this.tree) {
                    this.tree.option("disabled", this.saving);
                }
            } catch (e) { }

            try {
                if (this.popup) {
                    var self = this;
                    var items = this.popup.option("toolbarItems") || [];
                    this.popup.option("toolbarItems", items.map(function (item) {
                        if (!item || !item.options) return item;

                        var next = $.extend(true, {}, item);
                        next.options.disabled = self.saving;
                        if (next.options.type === "default") {
                            next.options.text = self.saving ? "Đang lưu..." : "Lưu";
                        }
                        return next;
                    }));
                }
            } catch (e) { }
        }

        save() {
            if (this.saving) return;

            var selectedMap = this.getSelectedFieldMap();
            var currentMap = {};
            var selectedCount = 0;
            var self = this;

            Object.keys(selectedMap).forEach(function (field) {
                if (selectedMap[field]) selectedCount += 1;
            });

            if (this.leafFields.length && selectedCount === 0) {
                this.notify("Cần giữ ít nhất một cột hiển thị.", "warning", 1800);
                return;
            }

            this.setSavingState(true);
            if (this.deps.showBusy) this.deps.showBusy("Đang lưu cấu hình cột...");

            setTimeout(function () {
                try {
                    self.leafFields.forEach(function (field) {
                        currentMap[field] = true;
                    });

                    var nextHidden = self.loadHiddenFields().filter(function (field) {
                        return !currentMap[field];
                    });

                    self.leafFields.forEach(function (field) {
                        if (!selectedMap[field]) {
                            nextHidden.push(field);
                        }
                    });

                    self.saveHiddenFields(nextHidden);
                    self.deps.rebuildGridColumns(self.deps.getSelectedLineX());

                    if (self.popup) self.popup.hide();
                    self.notify("Đã lưu cấu hình cột hiển thị.", "success", 1200);
                } catch (e) {
                    console.error(e);
                    self.notify("Lưu cấu hình cột thất bại.", "error", 2200);
                } finally {
                    if (self.deps.hideBusy) self.deps.hideBusy();
                    self.setSavingState(false);
                }
            }, 0);
        }

        open() {
            if (!this.deps.hasBaseColumns()) {
                this.notify("Danh sách cột chưa sẵn sàng.", "warning", 1600);
                return;
            }

            this.ensurePopup();
            this.setSavingState(false);

            var permissionLineX = this.deps.getPermissionLineX();
            var columns = this.deps.cloneBaseColumns();
            columns = this.deps.prepareGridColumns(columns, permissionLineX);
            columns = this.deps.applyLineModeColumnVisibility(columns, false);

            var treeData = this.buildTreeData(columns);
            this.treeItems = treeData.items;
            this.leafFields = treeData.leafFields;

            this.tree.option("dataSource", treeData.items);
            try { this.tree.repaint(); } catch (e) { }

            if (!treeData.leafFields.length) {
                this.notify("Không có cột nào được quyền hiển thị để cấu hình.", "warning", 1800);
            }

            this.popup.show();
        }
    }

    WIP.UI.ColumnFilterPopupController = ColumnFilterPopupController;
})(window, jQuery);
