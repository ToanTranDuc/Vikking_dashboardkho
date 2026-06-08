(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Core = WIP.Core || {};

    class LineState {
        constructor(options) {
            this.options = options || {};
            this.items = [];
            this.selectedLineX = null;
            this.selectedIsGiaCong = false;
        }

        normalizeLines(response) {
            var arr = Array.isArray(response) ? response : [];

            if (arr.length && typeof arr[0] === "string") {
                return arr
                    .map(function (value) { return value == null ? "" : String(value).trim(); })
                    .filter(Boolean)
                    .map(function (name) { return { id: null, name: name }; });
            }

            return arr.map(function (item) {
                if (!item || typeof item !== "object") return null;

                var rawId = item.Id != null ? item.Id
                    : item.ID != null ? item.ID
                        : item.id != null ? item.id
                            : item.LineXId != null ? item.LineXId
                                : item.LineXID != null ? item.LineXID
                                    : item.lineXId != null ? item.lineXId
                                        : item.value;

                var id = null;
                if (rawId !== null && rawId !== undefined && rawId !== "") {
                    var n = Number(rawId);
                    id = Number.isFinite(n) ? n : null;
                }

                var rawName = item.Name != null ? item.Name
                    : item.name != null ? item.name
                        : item.LineName != null ? item.LineName
                            : item.lineName != null ? item.lineName
                                : item.text != null ? item.text
                                    : item.LineX;

                var name = rawName == null ? "" : String(rawName).trim();
                if (!name) return null;

                return { id: id, name: name };
            }).filter(Boolean);
        }

        buildTabItems(lines) {
            return [{ text: "Tất cả", value: null }]
                .concat((lines || []).map(function (line) {
                    return { text: line.name, value: line.id };
                }))
                .concat([{ text: "Gia công", value: 900, isGiaCongTab: true }]);
        }

        setItems(items) {
            this.items = items || [];
            return this.items;
        }

        getItems() {
            return this.items || [];
        }

        selectItem(item) {
            var selected = item || null;
            this.selectedLineX = selected ? selected.value : null;
            this.selectedIsGiaCong = selected && selected.isGiaCongTab === true ? true : false;
            return this.getSelection();
        }

        ensureDefault(defaultLineX) {
            if (this.selectedLineX == null && this.selectedIsGiaCong === false) {
                this.selectedLineX = defaultLineX;
            }
            return this.getSelection();
        }

        getSelectedIndex() {
            var items = this.getItems();
            var idx = items.findIndex((item) => {
                return this.selectedIsGiaCong
                    ? item.isGiaCongTab === true
                    : item.value === this.selectedLineX;
            });
            return idx < 0 ? 0 : idx;
        }

        selectIndex(index) {
            var items = this.getItems();
            return this.selectItem(items[index] || items[0] || null);
        }

        getSelection() {
            return {
                lineX: this.selectedLineX,
                isGiaCong: this.selectedIsGiaCong
            };
        }
    }

    WIP.Core.LineState = LineState;
})(window);
