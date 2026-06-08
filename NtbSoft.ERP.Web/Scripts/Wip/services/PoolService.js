(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    function PoolService(deps) {
        this.deps = deps || {};
    }

    PoolService.prototype.getPoolData = function () {
        return typeof this.deps.getPoolData === "function" ? (this.deps.getPoolData() || []) : [];
    };

    PoolService.prototype.setPoolData = function (items) {
        if (typeof this.deps.setPoolData === "function") this.deps.setPoolData(items || []);
    };

    PoolService.prototype.getLineData = function () {
        return typeof this.deps.getLineData === "function" ? (this.deps.getLineData() || []) : [];
    };

    PoolService.prototype.setLineData = function (items) {
        if (typeof this.deps.setLineData === "function") this.deps.setLineData(items || []);
    };

    PoolService.prototype.updateGrid = function (grid, items) {
        if (this.deps.gridViewAdapter && typeof this.deps.gridViewAdapter.updateGrid === "function") {
            this.deps.gridViewAdapter.updateGrid(grid, items);
            return;
        }

        if (grid) {
            grid.option("dataSource", items || []);
            grid.refresh();
        }
    };

    PoolService.prototype.mapRowId = function (row) {
        return typeof this.deps.mapRowId === "function" ? this.deps.mapRowId(row) : row;
    };

    PoolService.prototype.wipIdStrFromRow = function (row) {
        return String(row && row.WIPId ? row.WIPId : "");
    };

    PoolService.prototype.removePoolByWipIds = function (wipIds) {
        var set = {};
        var next;

        (wipIds || []).forEach(function (id) {
            set[String(id)] = true;
        });

        next = this.getPoolData().filter(function (row) {
            return !set[String(row && row.WIPId)];
        });

        this.setPoolData(next);
        this.updateGrid(this.deps.getPoolGrid && this.deps.getPoolGrid(), next);
    };

    PoolService.prototype.addRowsToPool = function (rows) {
        var self = this;
        var existed = {};
        var current = this.getPoolData();

        current.forEach(function (row) {
            existed[self.wipIdStrFromRow(row)] = true;
        });

        var add = (rows || []).map(function (row) {
            if (!row) return null;

            var next = $.extend(true, {}, row);
            next.LineX = null;
            next.LenhSX = null;
            next.ThuTuChuyen = null;

            return self.mapRowId(next);
        }).filter(function (row) {
            var key = self.wipIdStrFromRow(row);
            return row && key && !existed[key];
        });

        var nextData = add.concat(current || []);
        this.setPoolData(nextData);
        this.updateGrid(this.deps.getPoolGrid && this.deps.getPoolGrid(), nextData);
    };

    PoolService.prototype.replaceLineData = function (lineX, rows) {
        var self = this;
        var selectedLineX = typeof this.deps.getSelectedLineX === "function" ? this.deps.getSelectedLineX() : null;
        if (!selectedLineX) return false;

        var lx = String(lineX || "");
        var nextData = this.getLineData().filter(function (row) {
            return String((row && row.LineX) || "") !== lx;
        });

        (rows || []).map(function (row) {
            return self.mapRowId(row);
        }).forEach(function (row) {
            nextData.push(row);
        });

        this.setLineData(nextData);
        this.updateGrid(this.deps.getMainGrid && this.deps.getMainGrid(), nextData);
        return true;
    };

    PoolService.prototype.patchPoolByMaDH = function (maDH, items) {
        maDH = String(maDH || "").trim();
        if (!maDH) return;

        var poolData = this.getPoolData();
        var grid = this.deps.getPoolGrid && this.deps.getPoolGrid();

        if (!items || items.length === 0) {
            var before = poolData.length;
            var filtered = poolData.filter(function (row) {
                return String(row && row.MaDH || "").trim() !== maDH;
            });

            if (before !== filtered.length) {
                this.setPoolData(filtered);
                this.updateGrid(grid, filtered);
                try { if (grid) grid.clearSelection(); } catch (e) { }
            }
            return;
        }

        var row = items[0];
        if (!row) return;

        row = this.mapRowId($.extend(true, {}, row));

        var idx = -1;
        if (row.WIPId && row.WIPId > 0) {
            idx = poolData.findIndex(function (item) {
                return item && item.WIPId === row.WIPId;
            });
        }

        if (idx < 0) {
            idx = poolData.findIndex(function (item) {
                return String(item && item.MaDH || "").trim() === maDH &&
                    (item.LineX == null) &&
                    (item.LenhSX == null) &&
                    (item.ThuTuChuyen == null);
            });
        }

        if (idx >= 0) {
            poolData[idx] = row;
        } else {
            poolData = [row].concat(poolData || []);
        }

        this.setPoolData(poolData);
        if (grid) {
            grid.option("dataSource", poolData);
            grid.refresh();
        }
    };

    WIP.Services.PoolService = PoolService;
})(window, jQuery);
