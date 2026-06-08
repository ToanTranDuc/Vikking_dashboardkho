(function () {
    "use strict";

    var api = window.WIP_HIGHCHARTS_GANTT_API || {};
    var DEFAULT_LINE_X = 101;
    var GANTT_ROW_HEIGHT = 46;
    var GANTT_RESERVED_HEIGHT = 56;
    var GANTT_FALLBACK_HEIGHT = 320;
    var chart = null;
    var lineTabs = null;
    var fromDateBox = null;
    var toDateBox = null;
    var resizeTimer = null;
    var lineTabsInitLock = true;
    var dateBoxInitLock = true;
    var lineItems = [];
    var labelLineColorMap = {};
    var state = {
        lineX: DEFAULT_LINE_X,
        keyword: "",
        fromDate: "",
        toDate: "",
        pageIndex: 1,
        pageSize: 50,
        totalRows: 0
    };

    $(function () {
        setDefaultDateRange();
        initDateBoxes();
        bindEvents();
        initLineTabs();
        loadLineXList();
    });

    function bindEvents() {
        var keywordTimer = null;

        $("#wipHcKeyword").on("keyup", function (e) {
            state.keyword = this.value || "";
            if (e.keyCode === 13) {
                state.pageIndex = 1;
                loadData();
                return;
            }

            clearTimeout(keywordTimer);
            keywordTimer = setTimeout(function () {
                state.pageIndex = 1;
                loadData();
            }, 500);
        });

        $("#wipHcPageSize").on("change", function () {
            state.pageSize = Number(this.value || 50);
            state.pageIndex = 1;
            loadData();
        });

        $("#wipHcReload").on("click", function () {
            loadData();
        });

        $("#wipHcPrev").on("click", function () {
            if (state.pageIndex <= 1) return;
            state.pageIndex -= 1;
            loadData();
        });

        $("#wipHcNext").on("click", function () {
            var maxPage = getMaxPage();
            if (state.pageIndex >= maxPage) return;
            state.pageIndex += 1;
            loadData();
        });

        $(window).on("resize.wipHcGantt orientationchange.wipHcGantt", scheduleChartReflow);

        if (window.visualViewport && window.visualViewport.addEventListener) {
            window.visualViewport.addEventListener("resize", scheduleChartReflow);
        }
    }

    function initDateBoxes() {
        if (!$.fn.dxDateBox) {
            return;
        }

        var commonOptions = {
            type: "date",
            pickerType: "calendar",
            displayFormat: "dd-MM-yyyy",
            useMaskBehavior: true,
            showClearButton: false,
            height: 28,
            width: "100%"
        };

        dateBoxInitLock = true;

        fromDateBox = $("#wipHcFromDate").dxDateBox($.extend({}, commonOptions, {
            value: parseDate(state.fromDate),
            onValueChanged: function (e) {
                if (dateBoxInitLock) return;
                state.fromDate = formatInputDate(e.value) || "";
                state.pageIndex = 1;
                loadData();
            }
        })).dxDateBox("instance");

        toDateBox = $("#wipHcToDate").dxDateBox($.extend({}, commonOptions, {
            value: parseDate(state.toDate),
            onValueChanged: function (e) {
                if (dateBoxInitLock) return;
                state.toDate = formatInputDate(e.value) || "";
                state.pageIndex = 1;
                loadData();
            }
        })).dxDateBox("instance");

        dateBoxInitLock = false;
    }

    function loadData() {
        if (!api.GET) return;

        setLoading(true);
        $.ajax({
            url: api.GET,
            method: "GET",
            dataType: "json",
            data: {
                keyword: state.keyword || null,
                fromDate: state.fromDate || null,
                toDate: state.toDate || null,
                lineX: state.lineX || null,
                pageIndex: state.pageIndex,
                pageSize: state.pageSize,
                includeEmptySteps: false,
                isKetThuc: 0
            }
        }).done(function (response) {
            response = normalizeResponse(response);
            state.totalRows = response.totalRows || 0;
            renderTotal();
            renderChart(mapToChartData(response));
        }).fail(function (xhr) {
            var message = "Không tải được dữ liệu Gantt.";
            if (xhr && xhr.responseJSON && xhr.responseJSON.Message) {
                message = xhr.responseJSON.Message;
            }
            showError(message);
        }).always(function () {
            setLoading(false);
        });
    }

    function initLineTabs() {
        if (!$("#lineTabs").length || !$.fn.dxTabs) {
            return;
        }

        lineTabs = $("#lineTabs").dxTabs({
            items: [],
            selectedIndex: 0,
            scrollByContent: true,
            showNavButtons: true,
            selectionMode: "single",
            onSelectionChanged: function (e) {
                if (lineTabsInitLock) return;

                var item = e.addedItems && e.addedItems[0] ? e.addedItems[0] : null;
                state.lineX = item ? item.value : null;
                state.pageIndex = 1;
                loadData();
            }
        }).dxTabs("instance");
        $("#Layer_1").click();
    }

    function loadLineXList() {
        if (!api.GET_LINEX || !lineTabs) {
            loadData();
            return;
        }

        $.ajax({
            url: api.GET_LINEX,
            method: "GET",
            dataType: "json"
        }).done(function (response) {
            var lines = normalizeLines(response);
            lineItems = [{ text: "Tất cả", value: null }]
                .concat(lines.map(function (line) {
                    return { text: line.name, value: line.id };
                }));

            lineTabsInitLock = true;
            lineTabs.option("items", lineItems);

            var defaultIndex = findLineIndex(DEFAULT_LINE_X);
            if (defaultIndex < 0 && lineItems.length > 1) defaultIndex = 1;
            if (defaultIndex < 0) defaultIndex = 0;

            lineTabs.option("selectedIndex", defaultIndex);
            state.lineX = lineItems[defaultIndex] ? lineItems[defaultIndex].value : null;
            lineTabsInitLock = false;
        }).fail(function () {
            state.lineX = DEFAULT_LINE_X;
            showError("Không tải được danh sách chuyền, đang dùng mặc định line " + DEFAULT_LINE_X + ".");
        }).always(function () {
            state.pageIndex = 1;
            loadData();
        });
    }

    function findLineIndex(lineX) {
        for (var i = 0; i < lineItems.length; i++) {
            if (Number(lineItems[i].value) === Number(lineX)) return i;
        }
        return -1;
    }

    function normalizeLines(response) {
        var items = Array.isArray(response) ? response : [];

        if (items.length && typeof items[0] === "string") {
            return items.map(function (name) {
                name = String(name || "").trim();
                return name ? { id: null, name: name } : null;
            }).filter(Boolean);
        }

        return items.map(function (item) {
            if (!item || typeof item !== "object") return null;

            var rawId = firstValue(item.Id, item.ID, item.id, item.LineXId, item.LineXID, item.lineXId, item.value);
            var id = null;
            if (rawId !== null && rawId !== undefined && rawId !== "") {
                var numericId = Number(rawId);
                id = isFinite(numericId) ? numericId : null;
            }

            var rawName = firstValue(item.Name, item.name, item.LineName, item.lineName, item.text, item.LineX);
            var name = rawName === null || rawName === undefined ? "" : String(rawName).trim();
            if (!name) return null;

            return { id: id, name: name };
        }).filter(Boolean);
    }

    function normalizeResponse(response) {
        response = response || {};
        return {
            masters: response.masters || response.Masters || [],
            totalRows: response.totalRows || response.TotalRows || 0
        };
    }

    function mapToChartData(response) {
        var data = [];
        var totalMasters = response.masters ? response.masters.length : 0;
        var missingDateCount = 0;
        labelLineColorMap = {};

        each(response.masters, function (master) {
            var wipId = get(master, "WIPId");
            if (!wipId) return;

            var range = getMasterRange(master);
            if (!range) {
                missingDateCount++;
                return;
            }

            var masterId = "wip-" + wipId;
            var masterName = buildMasterName(master);
            var lineColorIndex = getLineColorIndex(master);
            labelLineColorMap[masterName] = lineColorIndex;

            data.push({
                id: masterId,
                name: masterName,
                start: toUtc(range.start),
                end: toUtc(range.endForChart),
                colorIndex: lineColorIndex,
                className: "wip-hc-point-master wip-hc-line-" + lineColorIndex,
                custom: {
                    type: "master",
                    lineName: get(master, "LineName"),
                    lineColorIndex: lineColorIndex,
                    lenhSX: get(master, "LenhSX"),
                    maDH: get(master, "MaDH"),
                    maHang: get(master, "MaHang"),
                    po: get(master, "PO"),
                    slkh: get(master, "SLKH"),
                    timelineStart: range.start,
                    timelineEnd: range.end,
                    statusText: "Master"
                }
            });
        });

        return {
            data: data,
            totalMasters: totalMasters,
            missingDateCount: missingDateCount
        };
    }

    function renderChart(mapped) {
        if (!window.Highcharts || !Highcharts.ganttChart) {
            showError("Chưa load được highcharts-gantt.js.");
            return;
        }

        if (!mapped.data.length) {
            renderEmptyChart(mapped);
            return;
        }

        var $frame = $(".wip-hc-chart-frame");
        var frameHeight = $frame.height() || GANTT_FALLBACK_HEIGHT;
        var rowHeight = GANTT_ROW_HEIGHT;
        var chartReserveHeight = GANTT_RESERVED_HEIGHT;
        var chartHeight = frameHeight;
        var day = 24 * 3600 * 1000;
        var axisRange = getChartAxisRange(mapped, day);
        var viewRange = getInitialViewRange(axisRange, day);
        var visibleRows = Math.max(1, Math.floor((frameHeight - chartReserveHeight) / rowHeight));

        clearEmptyChart();
        $("#wipDonHangHighchartGantt").css({
            width: "100%",
            minWidth: "100%",
            height: chartHeight + "px"
        });

        chart = Highcharts.ganttChart("wipDonHangHighchartGantt", {
            chart: {
                styledMode: true,
                spacingLeft: 8,
                spacingRight: 8,
                spacingBottom: 8,
                height: chartHeight,
                events: {
                    render: function () {
                        renderHeaderMonthBoundaries(this);
                    }
                }
            },
            title: {
                text: "WIP - Gantt đơn hàng"
            },
            credits: {
                enabled: false
            },
            navigator: {
                enabled: true,
                liveRedraw: true,
                height: 30,
                margin: 0
            },
            scrollbar: {
                enabled: true,
                height: 9,
                margin: 0
            },
            rangeSelector: {
                enabled: false
            },
            xAxis: {
                min: viewRange.min,
                max: viewRange.max,
                floor: axisRange.min,
                ceiling: axisRange.max,
                plotBands: buildMonthPlotBands(axisRange.min, axisRange.max),
                plotLines: buildTodayPlotLine(),
                startOnTick: false,
                endOnTick: false,
                showFirstLabel: true,
                showLastLabel: true,
                tickInterval: day,
                minTickInterval: day,
                tickPixelInterval: 38,
                ordinal: false,
                units: [
                    ["day", [1]],
                    ["month", [1]],
                    ["year", [1]]
                ],
                tickPositioner: function () {
                    return buildDateTickPositions(this.min, this.max, day);
                },
                labels: {
                    format: "{value:%d}",
                    autoRotation: false,
                    crop: false,
                    overflow: "justify",
                    style: {
                        fontSize: "10px",
                        textOverflow: "none"
                    }
                },
                dateTimeLabelFormats: {
                    millisecond: "%d",
                    second: "%d",
                    minute: "%d",
                    hour: "%d",
                    day: "%d",
                    week: "%B %Y",
                    month: "%B %Y",
                    year: "%Y"
                },
                grid: {
                    enabled: true,
                    dateTimeLabelFormats: {
                        millisecond: "%B %Y",
                        second: "%B %Y",
                        minute: "%B %Y",
                        hour: "%B %Y",
                        day: "%B %Y",
                        week: "%B %Y",
                        month: "%B %Y",
                        year: "%Y"
                    }
                }
            },
            yAxis: {
                type: "treegrid",
                uniqueNames: false,
                staticScale: GANTT_ROW_HEIGHT,
                min: 0,
                max: Math.min(mapped.data.length - 1, visibleRows - 1),
                scrollbar: {
                    enabled: mapped.data.length > visibleRows
                },
                grid: {
                    enabled: true,
                    borderColor: "#e5e7eb"
                },
                labels: {
                    useHTML: true,
                    formatter: function () {
                        return formatTaskLabel(this.value, "wip-hc-y-label");
                    },
                    style: {
                        fontSize: "12px",
                        width: "clamp(132px, 12vw, 210px)"
                    }
                }
            },
            tooltip: {
                useHTML: true,
                outside: true,
                hideDelay: 0,
                style: {
                    fontSize: "12px",
                    lineHeight: "1.42"
                },
                pointFormatter: function () {
                    return tooltipHtml(this);
                }
            },
            plotOptions: {
                series: {
                    animation: false,
                    connectors: {
                        enabled: false
                    },
                    dataLabels: {
                        enabled: false
                    }
                }
            },
            series: [{
                name: "Tiến độ",
                data: mapped.data,
                dataSorting: {
                    enabled: false
                }
            }],
            lang: {
                noData: "Không có dữ liệu"
            },
            noData: {
                style: {
                    fontWeight: "600",
                    fontSize: "14px",
                    color: "#64748b"
                }
            }
        });

        bindYAxisWheelScroll(chart, visibleRows);
        scheduleChartReflow();
    }

    function scheduleChartReflow() {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(resizeChartToFrame, 120);
    }

    function resizeChartToFrame() {
        if (!chart) return;

        var $frame = $(".wip-hc-chart-frame");
        var frameHeight = $frame.height() || GANTT_FALLBACK_HEIGHT;

        closeExportMenu(chart);

        $("#wipDonHangHighchartGantt").css({
            width: "100%",
            minWidth: "100%",
            height: frameHeight + "px"
        });

        chart.setSize(null, frameHeight, false);
        chart.reflow();
        chart.redraw(false);
    }

    function closeExportMenu(chartInstance) {
        var exporting = chartInstance && chartInstance.exporting;
        var menu = exporting && exporting.contextMenuEl;

        if (menu && menu.hideMenu) {
            menu.hideMenu();
        }
    }

    function renderEmptyChart(mapped) {
        if (chart) {
            chart.destroy();
            chart = null;
        }

        $("#wipDonHangHighchartGantt").css({
            width: "100%",
            minWidth: "100%",
            height: "100%"
        });

        var message = "Không có dữ liệu để hiển thị.";
        if (mapped.totalMasters > 0 && mapped.missingDateCount > 0) {
            message = "Có " + mapped.totalMasters + " dòng nhưng chưa có ngày bắt đầu/kết thúc để vẽ Gantt.";
        }

        $("#wipDonHangHighchartGantt").html(
            '<div class="wip-hc-empty">' +
            '<div class="wip-hc-empty__title">' + escapeHtml(message) + '</div>' +
            '<div class="wip-hc-empty__desc">Vui lòng chọn line khác hoặc kiểm tra dữ liệu TimelineStart/TimelineEnd.</div>' +
            '</div>'
        );
    }

    function clearEmptyChart() {
        $("#wipDonHangHighchartGantt").empty();
    }

    function getChartAxisRange(mapped, day) {
        var min = null;
        var max = null;
        var labelMax = null;

        each(mapped.data, function (point) {
            var start = Number(point.start);
            var end = Number(point.end);
            var customEnd = point.custom ? toUtc(point.custom.timelineEnd) : null;
            var pointLabelMax = customEnd || (isFinite(end) ? end - day : null);

            if (isFinite(start)) {
                min = min === null ? start : Math.min(min, start);
            }

            if (isFinite(end)) {
                max = max === null ? end : Math.max(max, end);
            }

            if (pointLabelMax !== null && isFinite(pointLabelMax)) {
                labelMax = labelMax === null ? pointLabelMax : Math.max(labelMax, pointLabelMax);
            }
        });

        var filterFrom = toUtc(state.fromDate);
        var filterTo = toUtc(state.toDate);

        if (filterFrom !== null && isFinite(filterFrom)) {
            min = min === null ? filterFrom : Math.min(min, filterFrom);
        }

        if (filterTo !== null && isFinite(filterTo)) {
            max = max === null ? filterTo + day : Math.max(max, filterTo + day);
            labelMax = labelMax === null ? filterTo : Math.max(labelMax, filterTo);
        }

        min = min === null ? toUtc(new Date()) : min;
        max = max === null ? min + day : max;
        labelMax = labelMax === null ? max : labelMax;
        labelMax = addUtcDays(labelMax, 1);
        max = Math.max(max, labelMax);

        return {
            min: min,
            max: max,
            labelMax: labelMax
        };
    }

    function getInitialViewRange(axisRange, day) {
        var todayUtc = toUtc(startOfDay(new Date()));
        var viewMin = todayUtc - (45 * day);
        var viewMax = todayUtc + (45 * day);
        var rangeLength = viewMax - viewMin;

        if (viewMin < axisRange.min) {
            viewMin = axisRange.min;
            viewMax = Math.min(axisRange.max, viewMin + rangeLength);
        }

        if (viewMax > axisRange.max) {
            viewMax = axisRange.max;
            viewMin = Math.max(axisRange.min, viewMax - rangeLength);
        }

        if (viewMin >= viewMax) {
            viewMax = viewMin + day;
        }

        return {
            min: viewMin,
            max: viewMax
        };
    }

    function buildDateTickPositions(min, max, day) {
        if (!isFinite(min) || !isFinite(max) || max < min) {
            return [];
        }

        var ticks = [];
        var nextTick = Math.floor(min / day) * day;

        while (nextTick <= max) {
            ticks.push(nextTick);
            nextTick += day;
        }

        return ticks;
    }

    function buildMonthPlotBands(min, max) {
        var months = buildMonthRanges(min, max);

        return months.map(function (range, index) {
            return {
                from: range.from,
                to: range.to,
                className: index % 2 ? "wip-hc-month-band-alt" : "wip-hc-month-band"
            };
        });
    }

    function buildTodayPlotLine() {
        var today = toUtc(startOfDay(new Date()));

        return [{
            value: today,
            width: 2,
            zIndex: 7,
            className: "wip-hc-today-line",
            label: {
                text: "Hôm nay " + formatBarDate(today),
                rotation: 0,
                align: "left",
                x: 4,
                y: 13,
                style: {
                    fontSize: "10px",
                    fontWeight: "700"
                }
            }
        }];
    }

    function buildMonthRanges(min, max) {
        if (!isFinite(min) || !isFinite(max) || max < min) {
            return [];
        }

        var ranges = [];
        var cursor = new Date(min);
        cursor = new Date(Date.UTC(cursor.getUTCFullYear(), cursor.getUTCMonth(), 1));

        while (cursor.getTime() < max) {
            var next = new Date(Date.UTC(cursor.getUTCFullYear(), cursor.getUTCMonth() + 1, 1));

            ranges.push({
                from: Math.max(cursor.getTime(), min),
                to: Math.min(next.getTime(), max)
            });

            cursor = next;
        }

        return ranges;
    }

    function renderHeaderMonthBoundaries(chartInstance) {
        if (chartInstance.wipMonthHeaderGroup) {
            chartInstance.wipMonthHeaderGroup.destroy();
        }

        var xAxis = chartInstance.xAxis && chartInstance.xAxis[0];
        if (!xAxis) return;

        var extremes = xAxis.getExtremes();
        var months = buildMonthRanges(extremes.min, extremes.max);
        var monthBoundaries = months.slice(1);
        var headerBottom = chartInstance.plotTop;
        var headerTop = Math.max(0, headerBottom - 108);
        var plotLeft = chartInstance.plotLeft;
        var plotRight = chartInstance.plotLeft + chartInstance.plotWidth;
        var group = chartInstance.renderer.g("wip-month-header-boundaries")
            .attr({ zIndex: 8 })
            .add();

        months.forEach(function (range) {
            var fromX = clamp(Math.round(xAxis.toPixels(range.from)), plotLeft, plotRight);
            var toX = clamp(Math.round(xAxis.toPixels(range.to)), plotLeft, plotRight);
            var labelWidth = toX - fromX;

            if (labelWidth < 54) return;

            chartInstance.renderer
                .text(formatMonthHeader(range.from), Math.round(fromX + (labelWidth / 2)), headerTop + 14)
                .attr({ align: "center" })
                .addClass("wip-hc-month-header-label")
                .add(group);
        });

        monthBoundaries.forEach(function (range) {
            var x = Math.round(xAxis.toPixels(range.from));
            if (x <= plotLeft || x >= plotRight) return;

            chartInstance.renderer
                .path(["M", x, headerTop, "L", x, headerBottom])
                .addClass("wip-hc-month-header-boundary")
                .add(group);
        });

        chartInstance.wipMonthHeaderGroup = group;
    }

    function formatMonthHeader(value) {
        var date = new Date(value);
        if (isNaN(date.getTime())) return "";
        return "Tháng " + pad2(date.getUTCMonth() + 1) + "/" + date.getUTCFullYear();
    }

    function bindYAxisWheelScroll(chartInstance, visibleRows) {
        if (!chartInstance || !chartInstance.container || chartInstance.wipWheelBound) return;

        chartInstance.wipWheelBound = true;
        chartInstance.container.addEventListener("wheel", function (event) {
            var yAxis = chartInstance.yAxis && chartInstance.yAxis[0];
            if (!yAxis || !yAxis.scrollbar || visibleRows <= 0) return;

            var extremes = yAxis.getExtremes();
            var dataMin = isFinite(extremes.dataMin) ? extremes.dataMin : 0;
            var dataMax = isFinite(extremes.dataMax) ? extremes.dataMax : 0;
            var currentMin = isFinite(extremes.min) ? extremes.min : dataMin;
            var currentMax = isFinite(extremes.max) ? extremes.max : Math.min(dataMax, currentMin + visibleRows - 1);
            var span = currentMax - currentMin;
            var maxMin = Math.max(dataMin, dataMax - span);

            if (dataMax - dataMin + 1 <= visibleRows || maxMin <= dataMin) return;

            var delta = event.deltaY || event.wheelDelta || 0;
            var step = Math.max(1, Math.round(Math.abs(delta) / 120));
            var direction = delta > 0 ? 1 : -1;
            var nextMin = clamp(currentMin + (direction * step), dataMin, maxMin);
            var nextMax = nextMin + span;

            if (nextMin === currentMin) return;

            event.preventDefault();
            yAxis.setExtremes(nextMin, nextMax, true, false, { trigger: "mouseWheel" });
        }, { passive: false });
    }

    function addUtcDays(value, days) {
        return value + (days * 24 * 3600 * 1000);
    }

    function setDefaultDateRange() {
        var today = startOfDay(new Date());
        var fromDate = addDays(today, -45);
        var toDate = addDays(today, 45);

        state.fromDate = formatInputDate(fromDate);
        state.toDate = formatInputDate(toDate);
    }

    function tooltipHtml(point) {
        var custom = point.custom || {};
        var html = '<div style="min-width:270px;font-size:12px;line-height:1.42;padding:2px;background:#fff;position:relative;z-index:99999">';
        html += '<div style="font-size:14px;font-weight:700;margin-bottom:2px">Tiến độ</div>';
        html += "<b>Vào chuyền:</b> " + formatDate(custom.timelineStart || point.start);
        html += "<br/><b>Thoát chuyền:</b> " + formatDate(custom.timelineEnd || point.end);
        html += "<br/><b>Chuyền:</b> " + escapeHtml(custom.lineName || "");
        html += "<br/><b>Lệnh SX:</b> " + escapeHtml(custom.lenhSX || "");
        html += "<br/><b>Đơn hàng:</b> " + escapeHtml(custom.maDH || "");
        html += "<br/><b>Mã hàng:</b> " + escapeHtml(custom.maHang || "");
        html += "<br/><b>PO:</b> " + escapeHtml(custom.po || "");
        html += "<br/><b>SLKH:</b> " + escapeHtml(custom.slkh || "");

        html += "</div>";
        return html;
    }

    function getMasterRange(master) {
        var start = parseDate(get(master, "TimelineStart")) ||
            parseDate(get(master, "StartDate")) ||
            parseDate(get(master, "Start")) ||
            parseDate(get(master, "NgayBatDau"));

        var end = parseDate(get(master, "TimelineEnd")) ||
            parseDate(get(master, "EndDate")) ||
            parseDate(get(master, "End")) ||
            parseDate(get(master, "NgayKetThuc"));

        if (!start) return null;

        start = startOfDay(start);
        end = end ? startOfDay(end) : addDays(start, 1);

        if (end.getTime() < start.getTime()) {
            end = addDays(start, 1);
        }

        return {
            start: start,
            end: end,
            endForChart: addDays(end, 1)
        };
    }

    function buildMasterName(master) {
        var topParts = [];
        var maHang = get(master, "MaHang");

        if (get(master, "LineName")) topParts.push(get(master, "LineName"));
        if (get(master, "LenhSX")) topParts.push("LSX " + get(master, "LenhSX"));
        if (get(master, "MaDH")) topParts.push(get(master, "MaDH"));

        return topParts.join(" - ") + "||" + (maHang || "");
    }

    function formatTaskLabel(value, className) {
        var parts = splitTaskLabel(value);
        var title = parts.sub ? parts.top + " - " + parts.sub : parts.top;
        var lineColorIndex = labelLineColorMap[value];
        var lineClass = isFinite(lineColorIndex) ? " " + className + "--line-" + lineColorIndex : "";

        return '<div class="' + className + lineClass + '" title="' + escapeHtml(title) + '">' +
            '<div class="' + className + '__top">' + escapeHtml(parts.top) + '</div>' +
            (parts.sub ? '<div class="' + className + '__sub">' + escapeHtml(parts.sub) + '</div>' : '') +
            '</div>';
    }

    function splitTaskLabel(value) {
        var text = String(value || "");
        var parts = text.split("||");
        return {
            top: parts[0] || "",
            sub: parts.slice(1).join("||") || ""
        };
    }

    function getTaskDisplayName(value) {
        var parts = splitTaskLabel(value);
        return parts.sub ? parts.top + " - " + parts.sub : parts.top;
    }

    function getLineColorIndex(master) {
        var lineKey = get(master, "LineX") || get(master, "LineName") || get(master, "ThuTuChuyen") || "";
        return hashText(lineKey) % 10;
    }

    function hashText(value) {
        value = String(value || "");
        var hash = 0;
        for (var i = 0; i < value.length; i++) {
            hash = ((hash << 5) - hash) + value.charCodeAt(i);
            hash = hash & hash;
        }
        return Math.abs(hash);
    }

    function renderTotal() {
        $("#wipHcRecordTotal").text(state.totalRows || 0);
        $("#wipHcTotal").text("Trang " + state.pageIndex + "/" + getMaxPage());
        $("#wipHcPrev").prop("disabled", state.pageIndex <= 1);
        $("#wipHcNext").prop("disabled", state.pageIndex >= getMaxPage());
    }

    function getMaxPage() {
        return Math.max(1, Math.ceil((state.totalRows || 0) / state.pageSize));
    }

    function setLoading(value) {
        $("#wipHcLoading").toggle(!!value);
    }

    function showError(message) {
        $("#wipHcLoading").show().text(message);
    }

    function get(obj, name) {
        if (!obj) return null;
        if (obj[name] !== undefined) return obj[name];
        var camel = name.charAt(0).toLowerCase() + name.slice(1);
        return obj[camel] !== undefined ? obj[camel] : null;
    }

    function firstValue() {
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i] !== null && arguments[i] !== undefined && arguments[i] !== "") {
                return arguments[i];
            }
        }
        return null;
    }

    function parseDate(value) {
        if (!value) return null;
        if (value instanceof Date) return isNaN(value.getTime()) ? null : value;
        var inputDate = typeof value === "string" ? /^(\d{2})-(\d{2})-(\d{4})$/.exec(value) : null;
        if (inputDate) {
            var parsedDate = new Date(Number(inputDate[3]), Number(inputDate[2]) - 1, Number(inputDate[1]));
            return isNaN(parsedDate.getTime()) ? null : parsedDate;
        }
        var netDate = typeof value === "string" ? /\/Date\((\d+)(?:[+-]\d+)?\)\//.exec(value) : null;
        var date = netDate ? new Date(Number(netDate[1])) : new Date(value);
        return isNaN(date.getTime()) ? null : date;
    }

    function toUtc(date) {
        date = parseDate(date);
        if (!date) return null;
        return Date.UTC(date.getFullYear(), date.getMonth(), date.getDate());
    }

    function formatDate(value) {
        var date = parseDate(value);
        if (!date) return "";
        return pad2(date.getDate()) + "-" + pad2(date.getMonth() + 1) + "-" + String(date.getFullYear()).slice(-2);
    }

    function formatBarDate(value) {
        var date = parseDate(value);
        if (!date) return "";
        return pad2(date.getDate()) + "-" + pad2(date.getMonth() + 1);
    }

    function formatInputDate(value) {
        var date = parseDate(value);
        if (!date) return "";
        return date.getFullYear() + "-" + pad2(date.getMonth() + 1) + "-" + pad2(date.getDate());
    }

    function startOfDay(date) {
        return new Date(date.getFullYear(), date.getMonth(), date.getDate());
    }

    function addDays(date, days) {
        var next = new Date(date.getTime());
        next.setDate(next.getDate() + days);
        return next;
    }

    function clamp(value, min, max) {
        return Math.min(Math.max(value, min), max);
    }

    function pad2(value) {
        value = String(value);
        return value.length < 2 ? "0" + value : value;
    }

    function each(items, callback) {
        if (!items) return;
        for (var i = 0; i < items.length; i++) {
            callback(items[i], i);
        }
    }

    function escapeHtml(value) {
        return String(value === null || value === undefined ? "" : value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
})();
