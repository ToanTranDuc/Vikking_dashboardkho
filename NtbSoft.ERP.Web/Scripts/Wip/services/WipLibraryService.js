(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    function WipLibraryService(options) {
        options = options || {};
        this.loadService = options.loadService;
    }

    WipLibraryService.prototype.normalizeList = function (list) {
        return Array.isArray(list) ? list : [];
    };

    WipLibraryService.prototype.getTodayStart = function () {
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        return today;
    };

    WipLibraryService.prototype.isEffectiveRecord = function (item, compareDate) {
        if (!compareDate) return true;
        if (!item || !item.NgayApDung) return false;

        var dt = new Date(item.NgayApDung);
        dt.setHours(0, 0, 0, 0);
        return dt <= this.getTodayStart();
    };

    WipLibraryService.prototype.sortByNgayApDungDesc = function (list, idField) {
        idField = idField || "ID";
        return (list || []).slice().sort(function (a, b) {
            var da = a && a.NgayApDung ? new Date(a.NgayApDung).getTime() : 0;
            var db = b && b.NgayApDung ? new Date(b.NgayApDung).getTime() : 0;
            if (db !== da) return db - da;
            return Number((b && b[idField]) || 0) - Number((a && a[idField]) || 0);
        });
    };

    WipLibraryService.prototype.fetchByStyle = function (apiUrl, styleId, maKH) {
        if (!styleId) return window.jQuery.Deferred().resolve([]).promise();

        var q = { maHang: styleId, action: "GetAll" };
        if (maKH) q.maKH = maKH;

        return this.loadService.fetchLibrary(apiUrl, q).then(this.normalizeList.bind(this));
    };

    WipLibraryService.prototype.fetchLibrary = function (apiUrl, query) {
        return this.loadService.fetchLibrary(apiUrl, query || {}).then(this.normalizeList.bind(this));
    };

    WipLibraryService.prototype.resolveDisplayId = function (list, valueField, currentVal, compareDate) {
        currentVal = Number(currentVal || 0);
        if (!(currentVal > 0)) return null;

        var self = this;
        var candidates = (list || []).filter(function (x) {
            return Number((x && x[valueField]) || 0) === currentVal;
        });

        if (compareDate) {
            candidates = candidates.filter(function (x) {
                return self.isEffectiveRecord(x, true);
            });
        }

        candidates = this.sortByNgayApDungDesc(candidates, "ID");
        return candidates.length ? candidates[0].ID : null;
    };

    WipLibraryService.prototype.pickLatestRecord = function (list, valueField, compareDate) {
        var self = this;
        var candidates = (list || []).filter(function (x) {
            return Number((x && x[valueField]) || 0) > 0;
        });

        if (compareDate) {
            candidates = candidates.filter(function (x) {
                return self.isEffectiveRecord(x, true);
            });
        }

        candidates = this.sortByNgayApDungDesc(candidates, "ID");
        return candidates.length ? candidates[0] : null;
    };

    WipLibraryService.prototype.findById = function (list, key) {
        var k = String(key);
        return (list || []).find(function (x) {
            return String(x && x.ID) === k;
        }) || null;
    };

    WipLibraryService.prototype.loadPickByStyleData = function (apiUrl, styleId, maKH, valueField, currentVal, options) {
        var self = this;
        if (typeof valueField !== "string") {
            options = currentVal;
            currentVal = valueField;
            valueField = "SMV";
        }
        options = options || {};
        var compareDate = options.compareDate !== false;

        return this.fetchByStyle(apiUrl, styleId, maKH).then(function (list) {
            var available = self.normalizeList(list).filter(function (x) {
                return Number((x && x[valueField]) || 0) > 0;
            });

            return {
                list: list,
                resolvedId: self.resolveDisplayId(available, valueField, currentVal, compareDate),
                pickedItem: self.pickLatestRecord(available, valueField, compareDate)
            };
        });
    };

    WipLibraryService.prototype.loadSelectData = function (apiUrl, query, valueField, currentVal, options) {
        var self = this;
        options = options || {};
        var compareDate = options.compareDate !== false;

        return this.fetchLibrary(apiUrl, query).then(function (list) {
            return {
                list: list,
                resolvedId: self.resolveDisplayId(list, valueField, currentVal, compareDate),
                pickedItem: self.pickLatestRecord(list, valueField, compareDate)
            };
        });
    };

    WIP.Services.WipLibraryService = WipLibraryService;
})(window);
