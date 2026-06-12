!function (t, e) { "object" == typeof module && module.exports ? (e.default = e, module.exports = t && t.document ? e(t) : e) : "function" == typeof define && define.amd ? define("highcharts/highcharts", function () { return e(t) }) : (t.Highcharts && t.Highcharts.error(16, !0), t.Highcharts = e(t)) }("undefined" != typeof window ? window : this, function (r) {
    "use strict"; var t = {}; function e(t, e, i, s) { t.hasOwnProperty(e) || (t[e] = s.apply(null, i), r && "function" == typeof CustomEvent && r.dispatchEvent(new CustomEvent("HighchartsModuleLoaded", { detail: { path: e, module: t[e] } }))) } return e(t, "Core/Globals.js", [], function () { var t, i = t = { SVG_NS: "http://www.w3.org/2000/svg", product: "Highcharts", version: "11.4.8" }; return i.win = void 0 !== r ? r : {}, i.doc = i.win.document, i.svg = i.doc && i.doc.createElementNS && !!i.doc.createElementNS(i.SVG_NS, "svg").createSVGRect, i.userAgent = i.win.navigator && i.win.navigator.userAgent || "", i.isChrome = i.win.chrome, i.isFirefox = -1 !== i.userAgent.indexOf("Firefox"), i.isMS = /(edge|msie|trident)/i.test(i.userAgent) && !i.win.opera, i.isSafari = !i.isChrome && -1 !== i.userAgent.indexOf("Safari"), i.isTouchDevice = /(Mobile|Android|Windows Phone)/.test(i.userAgent), i.isWebKit = -1 !== i.userAgent.indexOf("AppleWebKit"), i.deg2rad = 2 * Math.PI / 360, i.hasBidiBug = i.isFirefox && parseInt(i.userAgent.split("Firefox/")[1], 10) < 4, i.marginNames = ["plotTop", "marginRight", "marginBottom", "plotLeft"], i.noop = function () { }, i.supportsPassiveEvents = function () { let t = !1; var e; return i.isMS || (e = Object.defineProperty({}, "passive", { get: function () { t = !0 } }), i.win.addEventListener && i.win.removeEventListener && (i.win.addEventListener("testPassive", i.noop, e), i.win.removeEventListener("testPassive", i.noop, e))), t }(), i.charts = [], i.composed = [], i.dateFormats = {}, i.seriesTypes = {}, i.symbolSizes = {}, i.chartCount = 0, t }), e(t, "Core/Utilities.js", [t["Core/Globals.js"]], function (n) {
        let i, { charts: e, doc: a, win: h } = n; function l(t, e, i, s) {
            var r = e ? "Highcharts error" : "Highcharts warning"; let o = f(t = 32 === t ? r + ": Deprecated member" : t), a = o ? r + ` #${t}: www.highcharts.com/errors/${t}/` : t.toString(); if (void 0 !== s) {
                let i = ""; o && (a += "?"), C(s, function (t, e) {
                    i += `
 - ${e}: ` + t, o && (a += encodeURI(e) + "=" + encodeURI(t))
                }), a += i
            } w(n, "displayError", { chart: i, code: t, message: a, params: s }, function () { if (e) throw Error(a); h.console && -1 === l.messages.indexOf(a) && console.warn(a) }), l.messages.push(a)
        } function d(t, e) { return parseInt(t, e || 10) } function c(t) { return "string" == typeof t } function p(t) { t = Object.prototype.toString.call(t); return "[object Array]" === t || "[object Array Iterator]" === t } function u(t, e) { return !(!t || "object" != typeof t || e && p(t)) } function o(t) { return u(t) && "number" == typeof t.nodeType } function g(t) { var e = t && t.constructor; return !(!u(t, !0) || o(t) || !e || !e.name || "Object" === e.name) } function f(t) { return "number" == typeof t && !isNaN(t) && t < 1 / 0 && -1 / 0 < t } function m(t) { return null != t } function s(i, t, e) { let s, r = c(t) && !m(e), o = (t, e) => { m(t) ? i.setAttribute(e, t) : r ? (s = i.getAttribute(e)) || "class" !== e || (s = i.getAttribute(e + "Name")) : i.removeAttribute(e) }; return c(t) ? o(e, t) : C(t, o), s } function x(t) { return p(t) ? t : [t] } function y(t, e) { let i; for (i in t = t || {}, e) t[i] = e[i]; return t } function b() { var e = arguments, i = e.length; for (let t = 0; t < i; t++) { var s = e[t]; if (null != s) return s } } function v(t, e) { y(t.style, e) } function S(t) { return Math.pow(10, Math.floor(Math.log(t) / Math.LN10)) } function k(t, e) { return 1e14 < t ? t : parseFloat(t.toPrecision(e || 14)) } (l = l || {}).messages = [], Math.easeInOutSine = function (t) { return -.5 * (Math.cos(Math.PI * t) - 1) }; var t = Array.prototype.find ? function (t, e) { return t.find(e) } : function (t, e) { let i; var s = t.length; for (i = 0; i < s; i++)if (e(t[i], i)) return t[i] }; function C(t, e, i) { for (var s in t) Object.hasOwnProperty.call(t, s) && e.call(i || t[s], t[s], s, t) } function M(r, o, i) { function a(t, e) { let i = r.removeEventListener; i && i.call(r, t, e, !1) } function s(i) { let t, s; r.nodeName && (o ? (t = {})[o] = !0 : t = i, C(t, function (t, e) { if (i[e]) for (s = i[e].length; s--;)a(e, i[e][s].fn) })) } let t = "function" == typeof r && r.prototype || r; if (Object.hasOwnProperty.call(t, "hcEvents")) { let e = t.hcEvents; if (o) { let t = e[o] || []; i ? (e[o] = t.filter(function (t) { return i !== t.fn }), a(o, i)) : (s(e), e[o] = []) } else s(e), delete t.hcEvents } } function w(s, r, o, t) { if (o = o || {}, a.createEvent && (s.dispatchEvent || s.fireEvent && s !== n)) { let t = a.createEvent("Events"); t.initEvent(r, !0, !0), o = y(t, o), s.dispatchEvent ? s.dispatchEvent(o) : s.fireEvent(r, o) } else if (s.hcEvents) { o.target || y(o, { preventDefault: function () { o.defaultPrevented = !0 }, target: s, type: r }); let t = [], e = s, i = !1; for (; e.hcEvents;)Object.hasOwnProperty.call(e, "hcEvents") && e.hcEvents[r] && (t.length && (i = !0), t.unshift.apply(t, e.hcEvents[r])), e = Object.getPrototypeOf(e); i && t.sort((t, e) => t.order - e.order), t.forEach(t => { !1 === t.fn.call(s, o) && o.preventDefault() }) } t && !o.defaultPrevented && t.call(s, o) } C({ map: "map", each: "forEach", grep: "filter", reduce: "reduce", some: "some" }, function (e, i) { n[i] = function (t) { return l(32, !1, void 0, { ["Highcharts." + i]: "use Array." + e }), Array.prototype[e].apply(t, [].slice.call(arguments, 1)) } }); var r = function () { let t = Math.random().toString(36).substring(2, 9) + "-", e = 0; return function () { return "highcharts-" + (i ? "" : t) + e++ } }(); return h.jQuery && (h.jQuery.fn.highcharts = function () { let t = [].slice.call(arguments); if (this[0]) return t[0] ? (new n[c(t[0]) ? t.shift() : "Chart"](this[0], t[0], t[1]), this) : e[s(this[0], "data-highcharts-chart")] }), { addEvent: function (t, e, i, s = {}) { let r = "function" == typeof t && t.prototype || t, o = (Object.hasOwnProperty.call(r, "hcEvents") || (r.hcEvents = {}), r.hcEvents), a = (n.Point && t instanceof n.Point && t.series && t.series.chart && (t.series.chart.runTrackerClick = !0), t.addEventListener); a && a.call(t, e, i, !!n.supportsPassiveEvents && { passive: void 0 === s.passive ? -1 !== e.indexOf("touch") : s.passive, capture: !1 }), o[e] || (o[e] = []); s = { fn: i, order: "number" == typeof s.order ? s.order : 1 / 0 }; return o[e].push(s), o[e].sort((t, e) => t.order - e.order), function () { M(t, e, i) } }, arrayMax: function (t) { let e = t.length, i = t[0]; for (; e--;)t[e] > i && (i = t[e]); return i }, arrayMin: function (t) { let e = t.length, i = t[0]; for (; e--;)t[e] < i && (i = t[e]); return i }, attr: s, clamp: function (t, e, i) { return e < t ? t < i ? t : i : e }, clearTimeout: function (t) { m(t) && clearTimeout(t) }, correctFloat: k, createElement: function (t, e, i, s, r) { t = a.createElement(t); return e && y(t, e), r && v(t, { padding: "0", border: "none", margin: "0" }), i && v(t, i), s && s.appendChild(t), t }, crisp: (t, e = 0, i) => { e = e % 2 / 2, i = i ? -1 : 1; return (Math.round(t * i - e) + e) * i }, css: v, defined: m, destroyObjectProperties: function (i, s, r) { C(i, function (t, e) { t !== s && t?.destroy && t.destroy(), !t?.destroy && r || delete i[e] }) }, diffObjects: function (t, e, i, h) { var s = {}; return function s(t, r, o, a) { let n = i ? r : t; C(t, function (e, i) { if (!a && h && -1 < h.indexOf(i) && r[i]) { e = x(e), o[i] = []; for (let t = 0; t < Math.max(e.length, r[i].length); t++)r[i][t] && (void 0 === e[t] ? o[i][t] = r[i][t] : (o[i][t] = {}, s(e[t], r[i][t], o[i][t], a + 1))) } else u(e, !0) && !e.nodeType ? (o[i] = p(e) ? [] : {}, s(e, r[i] || {}, o[i], a + 1), 0 !== Object.keys(o[i]).length || "colorAxis" === i && 0 === a || delete o[i]) : (t[i] !== r[i] || i in t && !(i in r)) && "__proto__" !== i && "constructor" !== i && (o[i] = n[i]) }) }(t, e, s, 0), s }, discardElement: function (t) { t && t.parentElement && t.parentElement.removeChild(t) }, erase: function (t, e) { let i = t.length; for (; i--;)if (t[i] === e) { t.splice(i, 1); break } }, error: l, extend: y, extendClass: function (t, e) { function i() { } return y(i.prototype = new t, e), i }, find: t, fireEvent: w, getClosestDistance: function (t, e) { let i, s, r, o, a = !e; return t.forEach(t => { if (1 < t.length) for (o = s = t.length - 1; 0 < o; o--)(r = t[o] - t[o - 1]) < 0 && !a ? (e?.(), e = void 0) : r && (void 0 === i || r < i) && (i = r) }), i }, getMagnitude: S, getNestedProperty: function (t, e) { let i = t.split("."); for (; i.length && m(e);) { var s = i.shift(); if (void 0 === s || "__proto__" === s) return; if ("this" === s) { let t; return (t = u(e) ? e["@this"] : t) ?? e } s = e[s]; if (!m(s) || "function" == typeof s || "number" == typeof s.nodeType || s === h) return; e = s } return e }, getStyle: function i(s, t, e) { let r; if ("width" === t) { let t = Math.min(s.offsetWidth, s.scrollWidth), e = s.getBoundingClientRect && s.getBoundingClientRect().width; return e < t && e >= t - 1 && (t = Math.floor(e)), Math.max(0, t - (i(s, "padding-left", !0) || 0) - (i(s, "padding-right", !0) || 0)) } if ("height" === t) return Math.max(0, Math.min(s.offsetHeight, s.scrollHeight) - (i(s, "padding-top", !0) || 0) - (i(s, "padding-bottom", !0) || 0)); let o = h.getComputedStyle(s, void 0); return o && (r = o.getPropertyValue(t), b(e, "opacity" !== t) && (r = d(r))), r }, inArray: function (t, e, i) { return l(32, !1, void 0, { "Highcharts.inArray": "use Array.indexOf" }), e.indexOf(t, i) }, insertItem: function (t, e) { let i; var s = t.options.index, r = e.length; for (i = t.options.isInternal ? r : 0; i < r + 1; i++)if (!e[i] || f(s) && s < b(e[i].options.index, e[i]._i) || e[i].options.isInternal) { e.splice(i, 0, t); break } return i }, isArray: p, isClass: g, isDOMElement: o, isFunction: function (t) { return "function" == typeof t }, isNumber: f, isObject: u, isString: c, keys: function (t) { return l(32, !1, void 0, { "Highcharts.keys": "use Object.keys" }), Object.keys(t) }, merge: function () { function r(i, s) { return "object" != typeof i && (i = {}), C(s, function (t, e) { "__proto__" !== e && "constructor" !== e && (!u(t, !0) || g(t) || o(t) ? i[e] = s[e] : i[e] = r(i[e] || {}, t)) }), i } let t, e = arguments, i = {}; !0 === e[0] && (i = e[1], e = Array.prototype.slice.call(e, 2)); var s = e.length; for (t = 0; t < s; t++)i = r(i, e[t]); return i }, normalizeTickInterval: function (t, e, i, s, r) { let o, a = t; var n = t / (i = b(i, S(t))); for (e || (e = r ? [1, 1.2, 1.5, 2, 2.5, 3, 4, 5, 6, 8, 10] : [1, 2, 2.5, 5, 10], !1 === s && (1 === i ? e = e.filter(function (t) { return t % 1 == 0 }) : i <= .1 && (e = [1 / i]))), o = 0; o < e.length && (a = e[o], (!r || !(a * i >= t)) && (r || !(n <= (e[o] + (e[o + 1] || e[o])) / 2))); o++); return k(a * i, -Math.round(Math.log(.001) / Math.LN10)) }, objectEach: C, offset: function (t) { var e = a.documentElement, t = t.parentElement || t.parentNode ? t.getBoundingClientRect() : { top: 0, left: 0, width: 0, height: 0 }; return { top: t.top + (h.pageYOffset || e.scrollTop) - (e.clientTop || 0), left: t.left + (h.pageXOffset || e.scrollLeft) - (e.clientLeft || 0), width: t.width, height: t.height } }, pad: function (t, e, i) { return Array((e || 2) + 1 - String(t).replace("-", "").length).join(i || "0") + t }, pick: b, pInt: d, pushUnique: function (t, e) { return t.indexOf(e) < 0 && !!t.push(e) }, relativeLength: function (t, e, i) { return /%$/.test(t) ? e * parseFloat(t) / 100 + (i || 0) : parseFloat(t) }, removeEvent: M, replaceNested: function (t, ...e) { let i, s; do { for (s of (i = t, e)) t = t.replace(s[0], s[1]) } while (t !== i); return t }, splat: x, stableSort: function (t, i) { let s, e; var r = t.length; for (e = 0; e < r; e++)t[e].safeI = e; for (t.sort(function (t, e) { return 0 === (s = i(t, e)) ? t.safeI - e.safeI : s }), e = 0; e < r; e++)delete t[e].safeI }, syncTimeout: function (t, e, i) { return 0 < e ? setTimeout(t, e, i) : (t.call(0, i), -1) }, timeUnits: { millisecond: 1, second: 1e3, minute: 6e4, hour: 36e5, day: 864e5, week: 6048e5, month: 24192e5, year: 314496e5 }, uniqueKey: r, useSerialIds: function (t) { return i = b(t, i) }, wrap: function (t, e, i) { let s = t[e]; t[e] = function () { let t = arguments, e = this; return i.apply(this, [function () { return s.apply(e, arguments.length ? arguments : t) }].concat([].slice.call(arguments))) } } }
    }), e(t, "Core/Chart/ChartDefaults.js", [], function () { return { alignThresholds: !1, panning: { enabled: !1, type: "x" }, styledMode: !1, borderRadius: 0, colorCount: 10, allowMutatingData: !0, ignoreHiddenSeries: !0, spacing: [10, 10, 15, 10], resetZoomButton: { theme: {}, position: {} }, reflow: !0, type: "line", zooming: { singleTouch: !1, resetButton: { theme: { zIndex: 6 }, position: { align: "right", x: -10, y: 10 } } }, width: null, height: null, borderColor: "#334eff", backgroundColor: "#ffffff", plotBorderColor: "#cccccc" } }), e(t, "Core/Color/Palettes.js", [], function () { return { colors: ["#2caffe", "#544fc5", "#00e272", "#fe6a35", "#6b8abc", "#d568fb", "#2ee0ca", "#fa4b42", "#feb56a", "#91e8e1"] } }), e(t, "Core/Time.js", [t["Core/Globals.js"], t["Core/Utilities.js"]], function (u, t) { let i = u["win"], { defined: y, error: a, extend: b, isNumber: n, isObject: e, merge: s, objectEach: g, pad: f, pick: v, splat: r, timeUnits: S } = t, o = u.isSafari && i.Intl && i.Intl.DateTimeFormat.prototype.formatRange, l = u.isSafari && i.Intl && !i.Intl.DateTimeFormat.prototype.formatRange; class h { constructor(t) { this.options = {}, this.useUTC = !1, this.variableTimezone = !1, this.Date = i.Date, this.getTimezoneOffset = this.timezoneOffsetFunction(), this.update(t) } get(t, e) { var i, s; return this.variableTimezone || this.timezoneOffset ? (s = (i = e.getTime()) - this.getTimezoneOffset(e), e.setTime(s), s = e["getUTC" + t](), e.setTime(i), s) : this.useUTC ? e["getUTC" + t]() : e["get" + t]() } set(t, e, i) { if (this.variableTimezone || this.timezoneOffset) { if ("Milliseconds" === t || "Seconds" === t || "Minutes" === t && this.getTimezoneOffset(e) % 36e5 == 0) return e["setUTC" + t](i); var s = this.getTimezoneOffset(e), s = e.getTime() - s, r = (e.setTime(s), e["setUTC" + t](i), this.getTimezoneOffset(e)), s = e.getTime() + r; return e.setTime(s) } return this.useUTC || o && "FullYear" === t ? e["setUTC" + t](i) : e["set" + t](i) } update(t = {}) { var e = v(t.useUTC, !0); this.options = t = s(!0, this.options, t), this.Date = t.Date || i.Date || Date, this.useUTC = e, this.timezoneOffset = e && t.timezoneOffset || void 0, this.getTimezoneOffset = this.timezoneOffsetFunction(), this.variableTimezone = e && !(!t.getTimezoneOffset && !t.timezone) } makeTime(t, e, i, s, r, o) { let a, n, h; return this.useUTC ? (a = this.Date.UTC.apply(0, arguments), n = this.getTimezoneOffset(a), a += n, n !== (h = this.getTimezoneOffset(a)) ? a += h - n : n - 36e5 !== this.getTimezoneOffset(a - 36e5) || l || (a -= 36e5)) : a = new this.Date(t, e, v(i, 1), v(s, 0), v(r, 0), v(o, 0)).getTime(), a } timezoneOffsetFunction() { let t = this, o = this.options, e = o.getTimezoneOffset; return this.useUTC ? o.timezone ? t => { try { var e = "shortOffset," + (o.timezone || ""), [, , i, , s = 0] = (h.formatCache[e] = h.formatCache[e] || Intl.DateTimeFormat("en", { timeZone: o.timezone, timeZoneName: "shortOffset" })).format(t).split(/(GMT|:)/).map(Number), r = -36e5 * (i + s / 60); if (n(r)) return r } catch (t) { a(34) } return 0 } : this.useUTC && e ? t => 6e4 * e(t.valueOf()) : () => 6e4 * (t.timezoneOffset || 0) : t => 6e4 * new Date(t.toString()).getTimezoneOffset() } dateFormat(i, s, t) { if (!y(s) || isNaN(s)) return u.defaultOptions.lang && u.defaultOptions.lang.invalidDate || ""; i = v(i, "%Y-%m-%d %H:%M:%S"); let r = this, e = new this.Date(s), o = this.get("Hours", e), a = this.get("Day", e), n = this.get("Date", e), h = this.get("Month", e), l = this.get("FullYear", e), d = u.defaultOptions.lang, c = d && d.weekdays, p = d && d.shortWeekdays; return g(b({ a: p ? p[a] : c[a].substr(0, 3), A: c[a], d: f(n), e: f(n, 2, " "), w: a, b: d.shortMonths[h], B: d.months[h], m: f(h + 1), o: h + 1, y: l.toString().substr(2, 2), Y: l, H: f(o), k: o, I: f(o % 12 || 12), l: o % 12 || 12, M: f(this.get("Minutes", e)), p: o < 12 ? "AM" : "PM", P: o < 12 ? "am" : "pm", S: f(this.get("Seconds", e)), L: f(Math.floor(s % 1e3), 3) }, u.dateFormats), function (t, e) { for (; -1 !== i.indexOf("%" + e);)i = i.replace("%" + e, "function" == typeof t ? t.call(r, s) : t) }), t ? i.substr(0, 1).toUpperCase() + i.substr(1) : i } resolveDTLFormat(t) { return e(t, !0) ? t : { main: (t = r(t))[0], from: t[1], to: t[2] } } getTimeTicks(t, e, i, s) { let r, o, a, n, h = this, l = h.Date, d = [], c = {}, p = new l(e), u = t.unitRange, g = t.count || 1; if (s = v(s, 1), y(e)) { h.set("Milliseconds", p, u >= S.second ? 0 : g * Math.floor(h.get("Milliseconds", p) / g)), u >= S.second && h.set("Seconds", p, u >= S.minute ? 0 : g * Math.floor(h.get("Seconds", p) / g)), u >= S.minute && h.set("Minutes", p, u >= S.hour ? 0 : g * Math.floor(h.get("Minutes", p) / g)), u >= S.hour && h.set("Hours", p, u >= S.day ? 0 : g * Math.floor(h.get("Hours", p) / g)), u >= S.day && h.set("Date", p, u >= S.month ? 1 : Math.max(1, g * Math.floor(h.get("Date", p) / g))), u >= S.month && (h.set("Month", p, u >= S.year ? 0 : g * Math.floor(h.get("Month", p) / g)), o = h.get("FullYear", p)), u >= S.year && (o -= o % g, h.set("FullYear", p, o)), u === S.week && (n = h.get("Day", p), h.set("Date", p, h.get("Date", p) - n + s + (n < s ? -7 : 0))), o = h.get("FullYear", p); var f = h.get("Month", p), m = h.get("Date", p), x = h.get("Hours", p); e = p.getTime(), !h.variableTimezone && h.useUTC || !y(i) || (a = i - e > 4 * S.month || h.getTimezoneOffset(e) !== h.getTimezoneOffset(i)); let t = p.getTime(); for (r = 1; t < i;)d.push(t), u === S.year ? t = h.makeTime(o + r * g, 0) : u === S.month ? t = h.makeTime(o, f + r * g) : !a || u !== S.day && u !== S.week ? a && u === S.hour && 1 < g ? t = h.makeTime(o, f, m, x + r * g) : t += u * g : t = h.makeTime(o, f, m + r * g * (u === S.day ? 1 : 7)), r++; d.push(t), u <= S.hour && d.length < 1e4 && d.forEach(function (t) { t % 18e5 == 0 && "000000000" === h.dateFormat("%H%M%S%L", t) && (c[t] = "day") }) } return d.info = b(t, { higherRanks: c, totalRange: u * g }), d } getDateFormat(t, e, i, s) { let r = this.dateFormat("%m-%d %H:%M:%S.%L", e), o = "01-01 00:00:00.000", a = { millisecond: 15, second: 12, minute: 9, hour: 6, day: 3 }, n = "millisecond", h = n; for (n in S) { if (t === S.week && +this.dateFormat("%w", e) === i && r.substr(6) === o.substr(6)) { n = "week"; break } if (S[n] > t) { n = h; break } if (a[n] && r.substr(a[n]) !== o.substr(a[n])) break; "week" !== n && (h = n) } return this.resolveDTLFormat(s[n]).main } } return h.formatCache = {}, h }), e(t, "Core/Defaults.js", [t["Core/Chart/ChartDefaults.js"], t["Core/Globals.js"], t["Core/Color/Palettes.js"], t["Core/Time.js"], t["Core/Utilities.js"]], function (t, e, i, s, r) { let o = e["isTouchDevice"], { fireEvent: a, merge: n } = r, h = { colors: i.colors, symbols: ["circle", "diamond", "square", "triangle", "triangle-down"], lang: { loading: "Loading...", months: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], shortMonths: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], weekdays: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], decimalPoint: ".", numericSymbols: ["k", "M", "G", "T", "P", "E"], resetZoom: "Reset zoom", resetZoomTitle: "Reset zoom level 1:1", thousandsSep: " " }, global: { buttonTheme: { fill: "#f7f7f7", padding: 8, r: 2, stroke: "#cccccc", "stroke-width": 1, style: { color: "#333333", cursor: "pointer", fontSize: "0.8em", fontWeight: "normal" }, states: { hover: { fill: "#e6e6e6" }, select: { fill: "#e6e9ff", style: { color: "#000000", fontWeight: "bold" } }, disabled: { style: { color: "#cccccc" } } } } }, time: { Date: void 0, getTimezoneOffset: void 0, timezone: void 0, timezoneOffset: 0, useUTC: !0 }, chart: t, title: { style: { color: "#333333", fontWeight: "bold" }, text: "Chart title", align: "center", margin: 15, widthAdjust: -44 }, subtitle: { style: { color: "#666666", fontSize: "0.8em" }, text: "", align: "center", widthAdjust: -44 }, caption: { margin: 15, style: { color: "#666666", fontSize: "0.8em" }, text: "", align: "left", verticalAlign: "bottom" }, plotOptions: {}, legend: { enabled: !0, align: "center", alignColumns: !0, className: "highcharts-no-tooltip", events: {}, layout: "horizontal", itemMarginBottom: 2, itemMarginTop: 2, labelFormatter: function () { return this.name }, borderColor: "#999999", borderRadius: 0, navigation: { style: { fontSize: "0.8em" }, activeColor: "#0022ff", inactiveColor: "#cccccc" }, itemStyle: { color: "#333333", cursor: "pointer", fontSize: "0.8em", textDecoration: "none", textOverflow: "ellipsis" }, itemHoverStyle: { color: "#000000" }, itemHiddenStyle: { color: "#666666", textDecoration: "line-through" }, shadow: !1, itemCheckboxStyle: { position: "absolute", width: "13px", height: "13px" }, squareSymbol: !0, symbolPadding: 5, verticalAlign: "bottom", x: 0, y: 0, title: { style: { fontSize: "0.8em", fontWeight: "bold" } } }, loading: { labelStyle: { fontWeight: "bold", position: "relative", top: "45%" }, style: { position: "absolute", backgroundColor: "#ffffff", opacity: .5, textAlign: "center" } }, tooltip: { enabled: !0, animation: { duration: 300, easing: t => Math.sqrt(1 - Math.pow(t - 1, 2)) }, borderRadius: 3, dateTimeLabelFormats: { millisecond: "%A, %e %b, %H:%M:%S.%L", second: "%A, %e %b, %H:%M:%S", minute: "%A, %e %b, %H:%M", hour: "%A, %e %b, %H:%M", day: "%A, %e %b %Y", week: "Week from %A, %e %b %Y", month: "%B %Y", year: "%Y" }, footerFormat: "", headerShape: "callout", hideDelay: 500, padding: 8, shape: "callout", shared: !1, snap: o ? 25 : 10, headerFormat: '<span style="font-size: 0.8em">{point.key}</span><br/>', pointFormat: '<span style="color:{point.color}">●</span> {series.name}: <b>{point.y}</b><br/>', backgroundColor: "#ffffff", borderWidth: void 0, shadow: !0, stickOnContact: !1, style: { color: "#333333", cursor: "default", fontSize: "0.8em" }, useHTML: !1 }, credits: { enabled: !0, href: "https://www.highcharts.com?credits", position: { align: "right", x: -10, verticalAlign: "bottom", y: -5 }, style: { cursor: "pointer", color: "#999999", fontSize: "0.6em" }, text: "Highcharts.com" } }, l = (h.chart.styledMode = !1, new s(h.time)); return { defaultOptions: h, defaultTime: l, getOptions: function () { return h }, setOptions: function (t) { return a(e, "setOptions", { options: t }), n(!0, h, t), (t.time || t.global) && (e.time ? e.time.update(n(h.global, h.time, t.global, t.time)) : e.time = l), h } } }), e(t, "Core/Color/Color.js", [t["Core/Globals.js"], t["Core/Utilities.js"]], function (h, t) { let { isNumber: r, merge: o, pInt: s } = t; class l { static parse(t) { return t ? new l(t) : l.None } constructor(t) { let e, i, s, r, o = (this.rgba = [NaN, NaN, NaN, NaN], this.input = t, h.Color); if (o && o !== l) return new o(t); if ("object" == typeof t && void 0 !== t.stops) this.stops = t.stops.map(t => new l(t[1])); else if ("string" == typeof t) { var a, n; if (this.input = t = l.names[t.toLowerCase()] || t, "#" === t.charAt(0) && (a = t.length, n = parseInt(t.substr(1), 16), 7 === a ? i = [(16711680 & n) >> 16, (65280 & n) >> 8, 255 & n, 1] : 4 === a && (i = [(3840 & n) >> 4 | (3840 & n) >> 8, (240 & n) >> 4 | 240 & n, (15 & n) << 4 | 15 & n, 1])), !i) for (s = l.parsers.length; s-- && !i;)(e = (r = l.parsers[s]).regex.exec(t)) && (i = r.parse(e)) } i && (this.rgba = i) } get(s) { let t = this.input, e = this.rgba; if ("object" != typeof t || void 0 === this.stops) return e && r(e[0]) ? "rgb" === s || !s && 1 === e[3] ? "rgb(" + e[0] + "," + e[1] + "," + e[2] + ")" : "a" === s ? "" + e[3] : "rgba(" + e.join(",") + ")" : t; { let i = o(t); return i.stops = [].slice.call(i.stops), this.stops.forEach((t, e) => { i.stops[e] = [i.stops[e][0], t.get(s)] }), i } } brighten(e) { let i = this.rgba; if (this.stops) this.stops.forEach(function (t) { t.brighten(e) }); else if (r(e) && 0 !== e) for (let t = 0; t < 3; t++)i[t] += s(255 * e), i[t] < 0 && (i[t] = 0), 255 < i[t] && (i[t] = 255); return this } setOpacity(t) { return this.rgba[3] = t, this } tweenTo(t, e) { var i = this.rgba, s = t.rgba; if (!r(i[0]) || !r(s[0])) return t.input || "none"; t = 1 !== s[3] || 1 !== i[3]; return (t ? "rgba(" : "rgb(") + Math.round(s[0] + (i[0] - s[0]) * (1 - e)) + "," + Math.round(s[1] + (i[1] - s[1]) * (1 - e)) + "," + Math.round(s[2] + (i[2] - s[2]) * (1 - e)) + (t ? "," + (s[3] + (i[3] - s[3]) * (1 - e)) : "") + ")" } } return l.names = { white: "#ffffff", black: "#000000" }, l.parsers = [{ regex: /rgba\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d?(?:\.\d+)?)\s*\)/, parse: function (t) { return [s(t[1]), s(t[2]), s(t[3]), parseFloat(t[4], 10)] } }, { regex: /rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)/, parse: function (t) { return [s(t[1]), s(t[2]), s(t[3]), 1] } }], l.None = new l(""), l }), e(t, "Core/Animation/Fx.js", [t["Core/Color/Color.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, e, i) { let s = t["parse"], h = e["win"], { isNumber: m, objectEach: l } = i; class d { constructor(t, e, i) { this.pos = NaN, this.options = e, this.elem = t, this.prop = i } dSetter() { let t = this.paths, r = t && t[0], o = t && t[1], a = this.now || 0, n = []; if (1 !== a && r && o) if (r.length === o.length && a < 1) for (let t = 0; t < o.length; t++) { let e = r[t], i = o[t], s = []; for (let t = 0; t < i.length; t++) { var h = e[t], l = i[t]; m(h) && m(l) && ("A" !== i[0] || 4 !== t && 5 !== t) ? s[t] = h + a * (l - h) : s[t] = l } n.push(s) } else n = o; else n = this.toD || []; this.elem.attr("d", n, void 0, !0) } update() { let t = this.elem, e = this.prop, i = this.now, s = this.options.step; this[e + "Setter"] ? this[e + "Setter"]() : t.attr ? t.element && t.attr(e, i, null, !0) : t.style[e] = i + this.unit, s && s.call(t, i, this) } run(t, e, i) { let s = this, r = s.options, o = function (t) { return !o.stopped && s.step(t) }, a = h.requestAnimationFrame || function (t) { setTimeout(t, 13) }, n = function () { for (let t = 0; t < d.timers.length; t++)d.timers[t]() || d.timers.splice(t--, 1); d.timers.length && a(n) }; t !== e || this.elem["forceAnimate:" + this.prop] ? (this.startTime = +new Date, this.start = t, this.end = e, this.unit = i, this.now = this.start, this.pos = 0, o.elem = this.elem, o.prop = this.prop, o() && 1 === d.timers.push(o) && a(n)) : (delete r.curAnim[this.prop], r.complete && 0 === Object.keys(r.curAnim).length && r.complete.call(this.elem)) } step(t) { let e, i, s = +new Date, r = this.options, o = this.elem, a = r.complete, n = r.duration, h = r.curAnim; return e = !(o.attr && !o.element) && (t || s >= n + this.startTime ? (this.now = this.end, this.pos = 1, this.update(), h[this.prop] = !0, i = !0, l(h, function (t) { !0 !== t && (i = !1) }), i && a && a.call(o), !1) : (this.pos = r.easing((s - this.startTime) / n), this.now = this.start + (this.end - this.start) * this.pos, this.update(), !0)) } initPath(t, e, i) { let s = t.startX, r = t.endX, o = i.slice(), a = t.isArea, n = a ? 2 : 1, h = e && i.length > e.length && i.hasStackedCliffs, l, d, c, p, u = e && e.slice(); if (!u || h) return [o, o]; function g(t, e) { for (; t.length < d;) { var i = t[0], s = e[d - t.length]; s && "M" === i[0] && ("C" === s[0] ? t[0] = ["C", i[1], i[2], i[1], i[2], i[1], i[2]] : t[0] = ["L", i[1], i[2]]), t.unshift(i), a && (s = t.pop(), t.push(t[t.length - 1], s)) } } function f(e) { for (; e.length < d;) { let t = e[Math.floor(e.length / n) - 1].slice(); var i; "C" === t[0] && (t[1] = t[5], t[2] = t[6]), a ? (i = e[Math.floor(e.length / n)].slice(), e.splice(e.length / 2, 0, t, i)) : e.push(t) } } if (s && r && r.length) { for (c = 0; c < s.length; c++) { if (s[c] === r[0]) { l = c; break } if (s[0] === r[r.length - s.length + c]) { l = c, p = !0; break } if (s[s.length - 1] === r[r.length - s.length + c]) { l = s.length - c; break } } void 0 === l && (u = []) } return u.length && m(l) && (d = o.length + l * n, p ? (g(u, o), f(o)) : (g(o, u), f(u))), [u, o] } fillSetter() { d.prototype.strokeSetter.apply(this, arguments) } strokeSetter() { this.elem.attr(this.prop, s(this.start).tweenTo(s(this.end), this.pos), void 0, !0) } } return d.timers = [], d }), e(t, "Core/Animation/AnimationUtilities.js", [t["Core/Animation/Fx.js"], t["Core/Utilities.js"]], function (l, t) { let { defined: n, getStyle: d, isArray: c, isNumber: e, isObject: p, merge: u, objectEach: g, pick: i } = t; function h(t) { return p(t) ? u({ duration: 500, defer: 0 }, t) : { duration: t ? 500 : 0, defer: 0 } } function f(t, e) { let i = l.timers.length; for (; i--;)l.timers[i].elem !== t || e && e !== l.timers[i].prop || (l.timers[i].stopped = !0) } return { animate: function (i, s, r) { let o, a = "", n, h, t; p(r) || (r = { duration: (t = arguments)[2], easing: t[3], complete: t[4] }), e(r.duration) || (r.duration = 400), r.easing = "function" == typeof r.easing ? r.easing : Math[r.easing] || Math.easeInOutSine, r.curAnim = u(s), g(s, function (t, e) { f(i, e), h = new l(i, r, e), n = void 0, "d" === e && c(s.d) ? (h.paths = h.initPath(i, i.pathArray, s.d), h.toD = s.d, o = 0, n = 1) : i.attr ? o = i.attr(e) : (o = parseFloat(d(i, e)) || 0, "opacity" !== e && (a = "px")), "string" == typeof (n = n || t) && n.match("px") && (n = n.replace(/px/g, "")), h.run(o, n, a) }) }, animObject: h, getDeferredAnimation: function (t, e, i) { let s = h(e), r = i ? [i] : t.series, o = 0, a = 0; return r.forEach(t => { t = h(t.options.animation); o = p(e) && n(e.defer) ? s.defer : Math.max(o, t.duration + t.defer), a = Math.min(s.duration, t.duration) }), t.renderer.forExport && (o = 0), { defer: Math.max(0, o - a), duration: Math.min(o, a) } }, setAnimation: function (t, e) { e.renderer.globalAnimation = i(t, e.options.chart.animation, !0) }, stop: f } }), e(t, "Core/Renderer/HTML/AST.js", [t["Core/Globals.js"], t["Core/Utilities.js"]], function (d, t) { let { SVG_NS: c, win: e } = d, { attr: p, createElement: r, css: u, error: g, isFunction: i, isString: o, objectEach: f, splat: s } = t, a = e["trustedTypes"], n = a && i(a.createPolicy) && a.createPolicy("highcharts", { createHTML: t => t }), h = n ? n.createHTML("") : "", l = function () { try { return !!(new DOMParser).parseFromString(h, "text/html") } catch (t) { return !1 } }(); class m { static filterUserAttributes(s) { return f(s, (e, t) => { let i = !0; -1 === m.allowedAttributes.indexOf(t) && (i = !1), (i = -1 !== ["background", "dynsrc", "href", "lowsrc", "src"].indexOf(t) ? o(e) && m.allowedReferences.some(t => 0 === e.indexOf(t)) : i) || (g(33, !1, void 0, { "Invalid attribute in config": "" + t }), delete s[t]), o(e) && s[t] && (s[t] = e.replace(/</g, "&lt;")) }), s } static parseStyle(t) { return t.split(";").reduce((t, e) => { let i = e.split(":").map(t => t.trim()), s = i.shift(); return s && i.length && (t[s.replace(/-([a-z])/g, t => t[1].toUpperCase())] = i.join(":")), t }, {}) } static setElementHTML(t, e) { t.innerHTML = m.emptyHTML, e && new m(e).addToDOM(t) } constructor(t) { this.nodes = "string" == typeof t ? this.parseMarkup(t) : t } addToDOM(t) { return function h(t, l) { let e; return s(t).forEach(function (s) { let r; var o = s.tagName, a = s.textContent ? d.doc.createTextNode(s.textContent) : void 0, n = m.bypassHTMLFiltering; if (o) if ("#text" === o) r = a; else if (-1 !== m.allowedTags.indexOf(o) || n) { let t = "svg" !== o && l.namespaceURI || c, e = d.doc.createElementNS(t, o), i = s.attributes || {}; f(s, function (t, e) { "tagName" !== e && "attributes" !== e && "children" !== e && "style" !== e && "textContent" !== e && (i[e] = t) }), p(e, n ? i : m.filterUserAttributes(i)), s.style && u(e, s.style), a && e.appendChild(a), h(s.children || [], e), r = e } else g(33, !1, void 0, { "Invalid tagName in config": o }); r && l.appendChild(r), e = r }), e }(this.nodes, t) } parseMarkup(e) { let i, s = []; if (e = e.trim().replace(/ style=(["'])/g, " data-style=$1"), l) i = (new DOMParser).parseFromString(n ? n.createHTML(e) : e, "text/html"); else { let t = r("div"); t.innerHTML = e, i = { body: t } } let o = (t, e) => { let i = t.nodeName.toLowerCase(), s = { tagName: i }; "#text" === i && (s.textContent = t.textContent || ""); var r = t.attributes; if (r) { let e = {};[].forEach.call(r, t => { "data-style" === t.name ? s.style = m.parseStyle(t.value) : e[t.name] = t.value }), s.attributes = e } if (t.childNodes.length) { let e = [];[].forEach.call(t.childNodes, t => { o(t, e) }), e.length && (s.children = e) } e.push(s) }; return [].forEach.call(i.body.childNodes, t => o(t, s)), s } } return m.allowedAttributes = ["alt", "aria-controls", "aria-describedby", "aria-expanded", "aria-haspopup", "aria-hidden", "aria-label", "aria-labelledby", "aria-live", "aria-pressed", "aria-readonly", "aria-roledescription", "aria-selected", "class", "clip-path", "color", "colspan", "cx", "cy", "d", "dx", "dy", "disabled", "fill", "filterUnits", "flood-color", "flood-opacity", "height", "href", "id", "in", "in2", "markerHeight", "markerWidth", "offset", "opacity", "operator", "orient", "padding", "paddingLeft", "paddingRight", "patternUnits", "r", "radius", "refX", "refY", "role", "scope", "slope", "src", "startOffset", "stdDeviation", "stroke", "stroke-linecap", "stroke-width", "style", "tableValues", "result", "rowspan", "summary", "target", "tabindex", "text-align", "text-anchor", "textAnchor", "textLength", "title", "type", "valign", "width", "x", "x1", "x2", "xlink:href", "y", "y1", "y2", "zIndex"], m.allowedReferences = ["https://", "http://", "mailto:", "/", "../", "./", "#"], m.allowedTags = ["a", "abbr", "b", "br", "button", "caption", "circle", "clipPath", "code", "dd", "defs", "div", "dl", "dt", "em", "feComponentTransfer", "feComposite", "feDropShadow", "feFlood", "feFuncA", "feFuncB", "feFuncG", "feFuncR", "feGaussianBlur", "feMorphology", "feOffset", "feMerge", "feMergeNode", "filter", "h1", "h2", "h3", "h4", "h5", "h6", "hr", "i", "img", "li", "linearGradient", "marker", "ol", "p", "path", "pattern", "pre", "rect", "small", "span", "stop", "strong", "style", "sub", "sup", "svg", "table", "text", "textPath", "thead", "title", "tbody", "tspan", "td", "th", "tr", "u", "ul", "#text"], m.emptyHTML = h, m.bypassHTMLFiltering = !1, m }), e(t, "Core/Templating.js", [t["Core/Defaults.js"], t["Core/Utilities.js"]], function (t, e) { let { defaultOptions: b, defaultTime: v } = t, { extend: r, getNestedProperty: S, isArray: o, isNumber: u, isObject: a, pick: k, pInt: g } = e, C = { add: (t, e) => t + e, divide: (t, e) => 0 !== e ? t / e : "", eq: (t, e) => t == e, each: function (i) { let s = arguments[arguments.length - 1]; return !!o(i) && i.map((t, e) => M(s.body, r(a(t) ? t : { "@this": t }, { "@index": e, "@first": 0 === e, "@last": e === i.length - 1 }))).join("") }, ge: (t, e) => e <= t, gt: (t, e) => e < t, if: t => !!t, le: (t, e) => t <= e, lt: (t, e) => t < e, multiply: (t, e) => t * e, ne: (t, e) => t != e, subtract: (t, e) => t - e, unless: t => !t }; function M(t = "", d, c) { let e = /\{([\w\:\.\,;\-\/<>%@"'’= #\(\)]+)\}/g, i = /\(([\w\:\.\,;\-\/<>%@"'= ]+)\)/g, s = [], p = /f$/, u = /\.(\d)/, g = b.lang, f = c && c.time || v, m = c && c.numberFormatter || w, x = (t = "") => { let e; return "true" === t || "false" !== t && ((e = Number(t)).toString() === t ? e : S(t, d)) }, r, o, a = 0, n; for (; null !== (r = e.exec(t));) { var h, l = i.exec(r[1]), y = (l && (r = l, n = !0), o && o.isBlock || (o = { ctx: d, expression: r[1], find: r[0], isBlock: "#" === r[1].charAt(0), start: r.index, startInner: r.index + r[0].length, length: r[0].length }), r[1].split(" ")[0].replace("#", "")), y = (C[y] && (o.isBlock && y === o.fn && a++, o.fn || (o.fn = y)), "else" === r[1]); if (o.isBlock && o.fn && (r[1] === "/" + o.fn || y) ? a ? y || a-- : (h = o.startInner, h = t.substr(h, r.index - h), void 0 === o.body ? (o.body = h, o.startInner = r.index + r[0].length) : o.elseBody = h, o.find += h + r[0], y || (s.push(o), o = void 0)) : o.isBlock || s.push(o), l && !o?.isBlock) break } return s.forEach(i => { let s, r, { body: o, elseBody: a, expression: n, fn: h } = i; if (h) { let t = [i], e = n.split(" "); for (r = C[h].length; r--;)t.unshift(x(e[r + 1])); s = C[h].apply(d, t), i.isBlock && "boolean" == typeof s && (s = M(s ? o : a, d, c)) } else { let e = n.split(":"); if (s = x(e.shift() || ""), e.length && "number" == typeof s) { let t = e.join(":"); var l; p.test(t) ? (l = parseInt((t.match(u) || ["", "-1"])[1], 10), null !== s && (s = m(s, l, g.decimalPoint, -1 < t.indexOf(",") ? g.thousandsSep : ""))) : s = f.dateFormat(t, s) } } t = t.replace(i.find, k(s, "")) }), n ? M(t, d, c) : t } function w(t, e, i, s) { let r, o, a = (t = +t || 0, e = +e, b.lang), n = (t.toString().split(".")[1] || "").split("e")[0].length, h = t.toString().split("e"), l = e, d = (-1 === e ? e = Math.min(n, 20) : u(e) ? e && h[1] && h[1] < 0 && (0 <= (o = e + +h[1]) ? (h[0] = (+h[0]).toExponential(o).split("e")[0], e = o) : (h[0] = h[0].split(".")[0] || 0, t = e < 20 ? (h[0] * Math.pow(10, h[1])).toFixed(e) : 0, h[1] = 0)) : e = 2, (Math.abs(h[1] ? h[0] : t) + Math.pow(10, -Math.max(e, n) - 1)).toFixed(e)), c = String(g(d)), p = 3 < c.length ? c.length % 3 : 0; return i = k(i, a.decimalPoint), s = k(s, a.thousandsSep), r = (t < 0 ? "-" : "") + (p ? c.substr(0, p) + s : ""), +h[1] < 0 && !l ? r = "0" : r += c.substr(p).replace(/(\d{3})(?=\d)/g, "$1" + s), e ? r += i + d.slice(-e) : 0 == +r && (r = "0"), h[1] && 0 != +r && (r += "e" + h[1]), r } return { dateFormat: function (t, e, i) { return v.dateFormat(t, e, i) }, format: M, helpers: C, numberFormat: w } }), e(t, "Core/Renderer/RendererRegistry.js", [t["Core/Globals.js"]], function (s) { var t, r; let o; return r = t = { rendererTypes: {}, getRendererType: function (t = o) { return r.rendererTypes[t] || r.rendererTypes[o] }, registerRendererType: function (t, e, i) { r.rendererTypes[t] = e, o && !i || (o = t, s.Renderer = e) } }, t }), e(t, "Core/Renderer/RendererUtilities.js", [t["Core/Utilities.js"]], function (t) { let { clamp: b, pick: v, pushUnique: S, stableSort: k } = t; return { distribute: function i(e, s, r) { let o = e, t = o.reducedLen || s, a = (t, e) => t.target - e.target, n = [], h = e.length, l = [], d = n.push, c, p, u, g = !0, f, m, x = 0, y; for (c = h; c--;)x += e[c].size; if (x > t) { for (k(e, (t, e) => (e.rank || 0) - (t.rank || 0)), u = (y = e[0].rank === e[e.length - 1].rank) ? h / 2 : -1, p = y ? u : h - 1; u && x > t;)f = e[c = Math.floor(p)], S(l, c) && (x -= f.size), p += u, y && p >= e.length && (u /= 2, p = u); l.sort((t, e) => e - t).forEach(t => d.apply(n, e.splice(t, 1))) } for (k(e, a), e = e.map(t => ({ size: t.size, targets: [t.target], align: v(t.align, .5) })); g;) { for (c = e.length; c--;)f = e[c], m = (Math.min.apply(0, f.targets) + Math.max.apply(0, f.targets)) / 2, f.pos = b(m - f.size * f.align, 0, s - f.size); for (c = e.length, g = !1; c--;)0 < c && e[c - 1].pos + e[c - 1].size > e[c].pos && (e[c - 1].size += e[c].size, e[c - 1].targets = e[c - 1].targets.concat(e[c].targets), e[c - 1].align = .5, e[c - 1].pos + e[c - 1].size > s && (e[c - 1].pos = s - e[c - 1].size), e.splice(c, 1), g = !0) } return d.apply(o, n), c = 0, e.some(t => { let e = 0; return (t.targets || []).some(() => (o[c].pos = t.pos + e, void 0 !== r && Math.abs(o[c].pos - o[c].target) > r ? (o.slice(0, c + 1).forEach(t => delete t.pos), o.reducedLen = (o.reducedLen || s) - .1 * s, o.reducedLen > .1 * s && i(o, s, r), !0) : (e += o[c].size, c++, !1))) }), k(o, a), o } } }), e(t, "Core/Renderer/SVG/SVGElement.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Color/Color.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, m, i, e) { let { animate: o, animObject: a, stop: d } = t, { deg2rad: R, doc: n, svg: h, SVG_NS: l, win: s } = i, { addEvent: r, attr: c, createElement: p, crisp: u, css: x, defined: y, erase: g, extend: b, fireEvent: v, isArray: S, isFunction: k, isObject: f, isString: C, merge: M, objectEach: w, pick: A, pInt: T, pushUnique: P, replaceNested: L, syncTimeout: O, uniqueKey: E } = e; class D { _defaultGetter(t) { let e = A(this[t + "Value"], this[t], this.element ? this.element.getAttribute(t) : null, 0); return e = /^-?[\d\.]+$/.test(e) ? parseFloat(e) : e } _defaultSetter(t, e, i) { i.setAttribute(e, t) } add(t) { let e, i = this.renderer, s = this.element; return t && (this.parentGroup = t), void 0 !== this.textStr && "text" === this.element.nodeName && i.buildText(this), this.added = !0, (e = t && !t.handleZ && !this.zIndex ? e : this.zIndexSetter()) || (t ? t.element : i.box).appendChild(s), this.onAdd && this.onAdd(), this } addClass(t, e) { let i = !e && this.attr("class") || ""; return (t = (t || "").split(/ /g).reduce(function (t, e) { return -1 === i.indexOf(e) && t.push(e), t }, i ? [i] : []).join(" ")) !== i && this.attr("class", t), this } afterSetters() { this.doTransform && (this.updateTransform(), this.doTransform = !1) } align(t, e, i, s = !0) { let r, o, a, n, h = {}, l = this.renderer, d = l.alignedObjects, c = !!t; t ? (this.alignOptions = t, this.alignByTranslate = e, this.alignTo = i) : (t = this.alignOptions || {}, e = this.alignByTranslate, i = this.alignTo); var p = !i || C(i) ? i || "renderer" : void 0, i = (p && (c && P(d, this), i = void 0), A(i, l[p], l)), p = t.align, u = t.verticalAlign; return r = (i.x || 0) + (t.x || 0), o = (i.y || 0) + (t.y || 0), "right" === p ? a = 1 : "center" === p && (a = 2), a && (r += ((i.width || 0) - (t.width || 0)) / a), h[e ? "translateX" : "x"] = Math.round(r), "bottom" === u ? n = 1 : "middle" === u && (n = 2), n && (o += ((i.height || 0) - (t.height || 0)) / n), h[e ? "translateY" : "y"] = Math.round(o), s && (this[this.placed ? "animate" : "attr"](h), this.placed = !0), this.alignAttr = h, this } alignSetter(t) { var e = { left: "start", center: "middle", right: "end" }; e[t] && (this.alignValue = t, this.element.setAttribute("text-anchor", e[t])) } animate(t, e, i) { let s = a(A(e, this.renderer.globalAnimation, !0)), r = s.defer; return n.hidden && (s.duration = 0), 0 !== s.duration ? (i && (s.complete = i), O(() => { this.element && o(this, t, s) }, r)) : (this.attr(t, void 0, i || s.complete), w(t, function (t, e) { s.step && s.step.call(this, t, { prop: e, pos: 1, elem: this }) }, this)), this } applyTextOutline(t) { let r = this.element, e = (t = -1 !== t.indexOf("contrast") ? t.replace(/contrast/g, this.renderer.getContrast(r.style.fill)) : t).split(" "), o = e[e.length - 1], a = e[0]; if (a && "none" !== a && i.svg) { this.fakeTS = !0, a = a.replace(/(^[\d\.]+)(.*?)$/g, function (t, e, i) { return 2 * Number(e) + i }), this.removeTextOutline(); let i = n.createElementNS(l, "tspan"), t = (c(i, { class: "highcharts-text-outline", fill: o, stroke: o, "stroke-width": a, "stroke-linejoin": "round" }), r.querySelector("textPath") || r), e = ([].forEach.call(t.childNodes, t => { let e = t.cloneNode(!0); e.removeAttribute && ["fill", "stroke", "stroke-width", "stroke"].forEach(t => e.removeAttribute(t)), i.appendChild(e) }), 0), s = ([].forEach.call(t.querySelectorAll("text tspan"), t => { e += Number(t.getAttribute("dy")) }), n.createElementNS(l, "tspan")); s.textContent = "​", c(s, { x: Number(r.getAttribute("x")), dy: -e }), i.appendChild(s), t.insertBefore(i, t.firstChild) } } attr(i, t, e, s) { let r = this["element"], o = D.symbolCustomAttribs, a, n, h = this, l; return "string" == typeof i && void 0 !== t && (a = i, (i = {})[a] = t), "string" == typeof i ? h = (this[i + "Getter"] || this._defaultGetter).call(this, i, r) : (w(i, function (t, e) { l = !1, s || d(this, e), this.symbolName && -1 !== o.indexOf(e) && (n || (this.symbolAttr(i), n = !0), l = !0), !this.rotation || "x" !== e && "y" !== e || (this.doTransform = !0), l || (this[e + "Setter"] || this._defaultSetter).call(this, t, e, r) }, this), this.afterSetters()), e && e.call(this), h } clip(t) { var e, i; return t && !t.clipPath && (e = E() + "-", i = this.renderer.createElement("clipPath").attr({ id: e }).add(this.renderer.defs), b(t, { clipPath: i, id: e, count: 0 }), t.add(i)), this.attr("clip-path", t ? `url(${this.renderer.url}#${t.id})` : "none") } crisp(t, e) { e = Math.round(e || t.strokeWidth || 0); var i = t.x || this.x || 0, s = t.y || this.y || 0, r = (t.width || this.width || 0) + i, o = (t.height || this.height || 0) + s, i = u(i, e), s = u(s, e); return b(t, { x: i, y: s, width: u(r, e) - i, height: u(o, e) - s }), y(t.strokeWidth) && (t.strokeWidth = e), t } complexColor(t, e, i) { let s = this.renderer, r, o, a, n, h, l, d, c, p, u, g = [], f; v(this.renderer, "complexColor", { args: arguments }, function () { if (t.radialGradient ? o = "radialGradient" : t.linearGradient && (o = "linearGradient"), o) { if (a = t[o], h = s.gradients, l = t.stops, p = i.radialReference, S(a) && (t[o] = a = { x1: a[0], y1: a[1], x2: a[2], y2: a[3], gradientUnits: "userSpaceOnUse" }), "radialGradient" === o && p && !y(a.gradientUnits) && (n = a, a = M(a, s.getRadialAttr(p, n), { gradientUnits: "userSpaceOnUse" })), w(a, function (t, e) { "id" !== e && g.push(e, t) }), w(l, function (t) { g.push(t) }), h[g = g.join(",")]) u = h[g].attr("id"); else { a.id = u = E(); let e = h[g] = s.createElement(o).attr(a).add(s.defs); e.radAttr = n, e.stops = [], l.forEach(function (t) { c = 0 === t[1].indexOf("rgba") ? (d = (r = m.parse(t[1])).get("rgb"), r.get("a")) : (d = t[1], 1); t = s.createElement("stop").attr({ offset: t[0], "stop-color": d, "stop-opacity": c }).add(e); e.stops.push(t) }) } f = "url(" + s.url + "#" + u + ")", i.setAttribute(e, f), i.gradient = g, t.toString = function () { return f } } }) } css(t) { let i = this.styles, s = {}, r = this.element, o, a = !i; if (i && w(t, function (t, e) { i && i[e] !== t && (s[e] = t, a = !0) }), a) { null === (t = i ? b(i, s) : t).width || "auto" === t.width ? delete this.textWidth : "text" === r.nodeName.toLowerCase() && t.width && (o = this.textWidth = T(t.width)), b(this.styles, t), o && !h && this.renderer.forExport && delete t.width; let e = M(t); r.namespaceURI === this.SVG_NS && (["textOutline", "textOverflow", "width"].forEach(t => e && delete e[t]), e.color && (e.fill = e.color)), x(r, e) } return this.added && ("text" === this.element.nodeName && this.renderer.buildText(this), t.textOutline && this.applyTextOutline(t.textOutline)), this } dashstyleSetter(e) { let i, s = this["stroke-width"]; if ("inherit" === s && (s = 1), e = e && e.toLowerCase()) { let t = e.replace("shortdashdotdot", "3,1,1,1,1,1,").replace("shortdashdot", "3,1,1,1").replace("shortdot", "1,1,").replace("shortdash", "3,1,").replace("longdash", "8,3,").replace(/dot/g, "1,3,").replace("dash", "4,3,").replace(/,$/, "").split(","); for (i = t.length; i--;)t[i] = "" + T(t[i]) * A(s, NaN); e = t.join(",").replace(/NaN/g, "none"), this.element.setAttribute("stroke-dasharray", e) } } destroy() { let i = this, t = i.element || {}, e = i.renderer, s = t.ownerSVGElement, r = "SPAN" === t.nodeName && i.parentGroup || void 0, o, a; if (t.onclick = t.onmouseout = t.onmouseover = t.onmousemove = t.point = null, d(i), i.clipPath && s) { let e = i.clipPath;[].forEach.call(s.querySelectorAll("[clip-path],[CLIP-PATH]"), function (t) { -1 < t.getAttribute("clip-path").indexOf(e.element.id) && t.removeAttribute("clip-path") }), i.clipPath = e.destroy() } if (i.connector = i.connector?.destroy(), i.stops) { for (a = 0; a < i.stops.length; a++)i.stops[a].destroy(); i.stops.length = 0, i.stops = void 0 } for (i.safeRemoveChild(t); r && r.div && 0 === r.div.childNodes.length;)o = r.parentGroup, i.safeRemoveChild(r.div), delete r.div, r = o; i.alignOptions && g(e.alignedObjects, i), w(i, function (t, e) { i[e] && i[e].parentGroup === i && i[e].destroy && i[e].destroy(), delete i[e] }) } dSetter(t, e, i) { S(t) && ("string" == typeof t[0] && (t = this.renderer.pathToSegments(t)), t = (this.pathArray = t).reduce((t, e, i) => e && e.join ? (i ? t + " " : "") + e.join(" ") : (e || "").toString(), "")), /(NaN| {2}|^$)/.test(t) && (t = "M 0 0"), this[e] !== t && (i.setAttribute(e, t), this[e] = t) } fillSetter(t, e, i) { "string" == typeof t ? i.setAttribute(e, t) : t && this.complexColor(t, e, i) } hrefSetter(t, e, i) { i.setAttributeNS("http://www.w3.org/1999/xlink", e, t) } getBBox(t, e) { let i, s, r, o, { alignValue: a, element: n, renderer: h, styles: l, textStr: d } = this, { cache: c, cacheKeys: p } = h, u = n.namespaceURI === this.SVG_NS, g = A(e, this.rotation, 0), f = h.styledMode ? n && D.prototype.getStyle.call(n, "font-size") : l.fontSize; if (y(d) && (-1 === (o = d.toString()).indexOf("<") && (o = o.replace(/\d/g, "0")), o += ["", h.rootFontSize, f, g, this.textWidth, a, l.textOverflow, l.fontWeight].join(",")), !(i = o && !t ? c[o] : i) || i.polygon) { if (u || h.forExport) { try { r = this.fakeTS && function (t) { var e = n.querySelector(".highcharts-text-outline"); e && x(e, { display: t }) }, k(r) && r("none"), i = n.getBBox ? b({}, n.getBBox()) : { width: n.offsetWidth, height: n.offsetHeight, x: 0, y: 0 }, k(r) && r("") } catch (t) { } (!i || i.width < 0) && (i = { x: 0, y: 0, width: 0, height: 0 }) } else i = this.htmlGetBBox(); s = i.height, u && (i.height = s = { "11px,17": 14, "13px,20": 16 }[`${f || ""},` + Math.round(s)] || s); e = { bBox: i = g ? this.getRotatedBox(i, g) : i }; v(this, "afterGetBBox", e), i = e.bBox } if (o && ("" === d || 0 < i.height)) { for (; 250 < p.length;)delete c[p.shift()]; c[o] || p.push(o), c[o] = i } return i } getRotatedBox(t, e) { let { x: i, y: s, width: r, height: o } = t, { alignValue: a, translateY: n, rotationOriginX: h = 0, rotationOriginY: l = 0 } = this, d = { right: 1, center: .5 }[a || 0] || 0, c = Number(this.element.getAttribute("y") || 0) - (n ? 0 : s), p = e * R, u = (e - 90) * R, g = Math.cos(p), f = Math.sin(p), m = r * g, x = r * f, y = Math.cos(u), b = Math.sin(u), [[v, S], [k, C]] = [h, l].map(t => [t - t * g, t * f]), M = i + d * (r - m) + v + C + c * y, w = M + m, A = w - o * y, T = A - m, P = s + c - d * x - S + k + c * b, L = P + x, O = L - o * b, E = O - x, D = Math.min(M, w, A, T), j = Math.min(P, L, O, E), I = Math.max(M, w, A, T) - D, B = Math.max(P, L, O, E) - j; return { x: D, y: j, width: I, height: B, polygon: [[M, P], [w, L], [A, O], [T, E]] } } getStyle(t) { return s.getComputedStyle(this.element || this, "").getPropertyValue(t) } hasClass(t) { return -1 !== ("" + this.attr("class")).split(" ").indexOf(t) } hide() { return this.attr({ visibility: "hidden" }) } htmlGetBBox() { return { height: 0, width: 0, x: 0, y: 0 } } constructor(t, e) { this.onEvents = {}, this.opacity = 1, this.SVG_NS = l, this.element = "span" === e || "body" === e ? p(e) : n.createElementNS(this.SVG_NS, e), this.renderer = t, this.styles = {}, v(this, "afterInit") } on(t, e) { let i = this["onEvents"]; return i[t] && i[t](), i[t] = r(this.element, t, e), this } opacitySetter(t, e, i) { t = Number(Number(t).toFixed(3)); this.opacity = t, i.setAttribute(e, t) } reAlign() { this.alignOptions?.width && "left" !== this.alignOptions.align && (this.alignOptions.width = this.getBBox().width, this.placed = !1, this.align()) } removeClass(t) { return this.attr("class", ("" + this.attr("class")).replace(C(t) ? RegExp(`(^| )${t}( |$)`) : t, " ").replace(/ +/g, " ").trim()) } removeTextOutline() { var t = this.element.querySelector("tspan.highcharts-text-outline"); t && this.safeRemoveChild(t) } safeRemoveChild(t) { let e = t.parentNode; e && e.removeChild(t) } setRadialReference(t) { let e = this.element.gradient && this.renderer.gradients[this.element.gradient]; return this.element.radialReference = t, e && e.radAttr && e.animate(this.renderer.getRadialAttr(t, e.radAttr)), this } shadow(t) { let e = this["renderer"], i = M(90 === this.parentGroup?.rotation ? { offsetX: -1, offsetY: -1 } : {}, f(t) ? t : {}), s = e.shadowDefinition(i); return this.attr({ filter: t ? `url(${e.url}#${s})` : "none" }) } show(t = !0) { return this.attr({ visibility: t ? "inherit" : "visible" }) } "stroke-widthSetter"(t, e, i) { this[e] = t, i.setAttribute(e, t) } strokeWidth() { if (!this.renderer.styledMode) return this["stroke-width"] || 0; let t = this.getStyle("stroke-width"), e = 0, i; return /px$/.test(t) ? e = T(t) : "" !== t && (c(i = n.createElementNS(l, "rect"), { width: t, "stroke-width": 0 }), this.element.parentNode.appendChild(i), e = i.getBBox().width, i.parentNode.removeChild(i)), e } symbolAttr(e) { let i = this; D.symbolCustomAttribs.forEach(function (t) { i[t] = A(e[t], i[t]) }), i.attr({ d: i.renderer.symbols[i.symbolName](i.x, i.y, i.width, i.height, i) }) } textSetter(t) { t !== this.textStr && (delete this.textPxLength, this.textStr = t, this.added && this.renderer.buildText(this), this.reAlign()) } titleSetter(t) { let e = this.element, i = e.getElementsByTagName("title")[0] || n.createElementNS(this.SVG_NS, "title"); e.insertBefore ? e.insertBefore(i, e.firstChild) : e.appendChild(i), i.textContent = L(A(t, ""), [/<[^>]*>/g, ""]).replace(/&lt;/g, "<").replace(/&gt;/g, ">") } toFront() { let t = this.element; return t.parentNode.appendChild(t), this } translate(t, e) { return this.attr({ translateX: t, translateY: e }) } updateTransform(t = "transform") { let { element: e, matrix: i, rotation: s = 0, rotationOriginX: r, rotationOriginY: o, scaleX: a, scaleY: n, translateX: h = 0, translateY: l = 0 } = this, d = ["translate(" + h + "," + l + ")"]; y(i) && d.push("matrix(" + i.join(",") + ")"), s && (d.push("rotate(" + s + " " + A(r, e.getAttribute("x"), 0) + " " + A(o, e.getAttribute("y") || 0) + ")"), "SPAN" === this.text?.element.tagName && this.text.attr({ rotation: s, rotationOriginX: (r || 0) - this.padding, rotationOriginY: (o || 0) - this.padding })), (y(a) || y(n)) && d.push("scale(" + A(a, 1) + " " + A(n, 1) + ")"), d.length && !(this.text || this).textPath && e.setAttribute(t, d.join(" ")) } visibilitySetter(t, e, i) { "inherit" === t ? i.removeAttribute(e) : this[e] !== t && i.setAttribute(e, t), this[e] = t } xGetter(t) { return "circle" === this.element.nodeName && ("x" === t ? t = "cx" : "y" === t && (t = "cy")), this._defaultGetter(t) } zIndexSetter(t, e) { let i = this.renderer, s = this.parentGroup, r = (s || i).element || i.box, o = this.element, a = r === i.box, n, h, l, d = !1, c, p = this.added, u; if (y(t) ? (o.setAttribute("data-z-index", t), this[e] === (t = +t) && (p = !1)) : y(this[e]) && o.removeAttribute("data-z-index"), this[e] = t, p) { for ((t = this.zIndex) && s && (s.handleZ = !0), u = (n = r.childNodes).length - 1; 0 <= u && !d; u--)c = !y(l = (h = n[u]).getAttribute("data-z-index")), h !== o && (t < 0 && c && !a && !u ? (r.insertBefore(o, n[u]), d = !0) : (T(l) <= t || c && (!y(t) || 0 <= t)) && (r.insertBefore(o, n[u + 1]), d = !0)); d || (r.insertBefore(o, n[a ? 3 : 0]), d = !0) } return d } } return D.symbolCustomAttribs = ["anchorX", "anchorY", "clockwise", "end", "height", "innerR", "r", "start", "width", "x", "y"], D.prototype.strokeSetter = D.prototype.fillSetter, D.prototype.yGetter = D.prototype.xGetter, D.prototype.matrixSetter = D.prototype.rotationOriginXSetter = D.prototype.rotationOriginYSetter = D.prototype.rotationSetter = D.prototype.scaleXSetter = D.prototype.scaleYSetter = D.prototype.translateXSetter = D.prototype.translateYSetter = D.prototype.verticalAlignSetter = function (t, e) { this[e] = t, this.doTransform = !0 }, D }), e(t, "Core/Renderer/SVG/SVGLabel.js", [t["Core/Renderer/SVG/SVGElement.js"], t["Core/Utilities.js"]], function (t, e) { let { defined: o, extend: a, isNumber: n, merge: s, pick: d, removeEvent: i } = e; class c extends t { constructor(t, e, i, s, r, o, a, n, h, l) { let d; super(t, "g"), this.paddingLeftSetter = this.paddingSetter, this.paddingRightSetter = this.paddingSetter, this.doUpdate = !1, this.textStr = e, this.x = i, this.y = s, this.anchorX = o, this.anchorY = a, this.baseline = h, this.className = l, this.addClass("button" === l ? "highcharts-no-tooltip" : "highcharts-label"), l && this.addClass("highcharts-" + l), this.text = t.text(void 0, 0, 0, n).attr({ zIndex: 1 }), "string" == typeof r && ((d = /^url\((.*?)\)$/.test(r)) || this.renderer.symbols[r]) && (this.symbolKey = r), this.bBox = c.emptyBBox, this.padding = 3, this.baselineOffset = 0, this.needsBox = t.styledMode || d, this.deferredAttr = {}, this.alignFactor = 0 } alignSetter(t) { t = { left: 0, center: .5, right: 1 }[t]; t !== this.alignFactor && (this.alignFactor = t, this.bBox && n(this.xSetting) && this.attr({ x: this.xSetting })) } anchorXSetter(t, e) { this.anchorX = t, this.boxAttr(e, Math.round(t) - this.getCrispAdjust() - this.xSetting) } anchorYSetter(t, e) { this.anchorY = t, this.boxAttr(e, t - this.ySetting) } boxAttr(t, e) { this.box ? this.box.attr(t, e) : this.deferredAttr[t] = e } css(i) { if (i) { let e = {}; i = s(i), c.textProps.forEach(t => { void 0 !== i[t] && (e[t] = i[t], delete i[t]) }), this.text.css(e), "fontSize" in e || "fontWeight" in e ? this.updateTextPadding() : ("width" in e || "textOverflow" in e) && this.updateBoxSize() } return t.prototype.css.call(this, i) } destroy() { i(this.element, "mouseenter"), i(this.element, "mouseleave"), this.text && this.text.destroy(), this.box && (this.box = this.box.destroy()), t.prototype.destroy.call(this) } fillSetter(t, e) { t && (this.needsBox = !0), this.fill = t, this.boxAttr(e, t) } getBBox(t, e) { this.textStr && 0 === this.bBox.width && 0 === this.bBox.height && this.updateBoxSize(); let { padding: i, height: s = 0, translateX: r = 0, translateY: o = 0, width: a = 0 } = this, n = d(this.paddingLeft, i), h = e ?? (this.rotation || 0), l = { width: a, height: s, x: r + this.bBox.x - n, y: o + this.bBox.y - i + this.baselineOffset }; return l = h ? this.getRotatedBox(l, h) : l } getCrispAdjust() { return (this.renderer.styledMode && this.box ? this.box.strokeWidth() : this["stroke-width"] ? parseInt(this["stroke-width"], 10) : 0) % 2 / 2 } heightSetter(t) { this.heightSetting = t, this.doUpdate = !0 } afterSetters() { super.afterSetters(), this.doUpdate && (this.updateBoxSize(), this.doUpdate = !1) } onAdd() { this.text.add(this), this.attr({ text: d(this.textStr, ""), x: this.x || 0, y: this.y || 0 }), this.box && o(this.anchorX) && this.attr({ anchorX: this.anchorX, anchorY: this.anchorY }) } paddingSetter(t, e) { n(t) ? t !== this[e] && (this[e] = t, this.updateTextPadding()) : this[e] = void 0 } rSetter(t, e) { this.boxAttr(e, t) } strokeSetter(t, e) { this.stroke = t, this.boxAttr(e, t) } "stroke-widthSetter"(t, e) { t && (this.needsBox = !0), this["stroke-width"] = t, this.boxAttr(e, t) } "text-alignSetter"(t) { this.textAlign = t } textSetter(t) { void 0 !== t && this.text.attr({ text: t }), this.updateTextPadding(), this.reAlign() } updateBoxSize() { let t = this.text, e = {}, i = this.padding, s = this.bBox = n(this.widthSetting) && n(this.heightSetting) && !this.textAlign || !o(t.textStr) ? c.emptyBBox : t.getBBox(void 0, 0); this.width = this.getPaddedWidth(), this.height = (this.heightSetting || s.height || 0) + 2 * i; var r = this.renderer.fontMetrics(t); if (this.baselineOffset = i + Math.min((this.text.firstLineMetrics || r).b, s.height || 1 / 0), this.heightSetting && (this.baselineOffset += (this.heightSetting - r.h) / 2), this.needsBox && !t.textPath) { if (!this.box) { let t = this.box = this.symbolKey ? this.renderer.symbol(this.symbolKey) : this.renderer.rect(); t.addClass(("button" === this.className ? "" : "highcharts-label-box") + (this.className ? " highcharts-" + this.className + "-box" : "")), t.add(this) } r = this.getCrispAdjust(), e.x = r, e.y = (this.baseline ? -this.baselineOffset : 0) + r, e.width = Math.round(this.width), e.height = Math.round(this.height), this.box.attr(a(e, this.deferredAttr)), this.deferredAttr = {} } } updateTextPadding() { let i = this.text; if (!i.textPath) { this.updateBoxSize(); let t = this.baseline ? 0 : this.baselineOffset, e = d(this.paddingLeft, this.padding); o(this.widthSetting) && this.bBox && ("center" === this.textAlign || "right" === this.textAlign) && (e += { center: .5, right: 1 }[this.textAlign] * (this.widthSetting - this.bBox.width)), e === i.x && t === i.y || (i.attr("x", e), i.hasBoxWidthChanged && (this.bBox = i.getBBox(!0)), void 0 !== t && i.attr("y", t)), i.x = e, i.y = t } } widthSetter(t) { this.widthSetting = n(t) ? t : void 0, this.doUpdate = !0 } getPaddedWidth() { var t = this.padding, e = d(this.paddingLeft, t), t = d(this.paddingRight, t); return (this.widthSetting || this.bBox.width || 0) + e + t } xSetter(t) { this.x = t, this.alignFactor && (t -= this.alignFactor * this.getPaddedWidth(), this["forceAnimate:x"] = !0), this.xSetting = Math.round(t), this.attr("translateX", this.xSetting) } ySetter(t) { this.ySetting = this.y = Math.round(t), this.attr("translateY", this.ySetting) } } return c.emptyBBox = { width: 0, height: 0, x: 0, y: 0 }, c.textProps = ["color", "direction", "fontFamily", "fontSize", "fontStyle", "fontWeight", "lineHeight", "textAlign", "textDecoration", "textOutline", "textOverflow", "whiteSpace", "width"], c }), e(t, "Core/Renderer/SVG/Symbols.js", [t["Core/Utilities.js"]], function (t) { let { defined: v, isNumber: d, pick: S } = t; function r(g, f, m, x, y) { let b = []; if (y) { let t = y.start || 0, e = S(y.r, m), i = S(y.r, x || m), s = 2e-4 / (y.borderRadius ? 1 : Math.max(e, 1)), r = Math.abs((y.end || 0) - t - 2 * Math.PI) < s, o = (y.end || 0) - (r ? s : 0), a = y.innerR, n = S(y.open, r), h = Math.cos(t), l = Math.sin(t), d = Math.cos(o), c = Math.sin(o), p = S(y.longArc, o - t - Math.PI < s ? 0 : 1), u = ["A", e, i, 0, p, S(y.clockwise, 1), g + e * d, f + i * c]; u.params = { start: t, end: o, cx: g, cy: f }, b.push(["M", g + e * h, f + i * l], u), v(a) && ((u = ["A", a, a, 0, p, v(y.clockwise) ? 1 - y.clockwise : 0, g + a * h, f + a * l]).params = { start: o, end: t, cx: g, cy: f }, b.push(n ? ["M", g + a * d, f + a * c] : ["L", g + a * d, f + a * c], u)), n || b.push(["Z"]) } return b } function e(t, e, i, s, r) { return r && r.r ? c(t, e, i, s, r) : [["M", t, e], ["L", t + i, e], ["L", t + i, e + s], ["L", t, e + s], ["Z"]] } function c(t, e, i, s, r) { r = r?.r || 0; return [["M", t + r, e], ["L", t + i - r, e], ["A", r, r, 0, 0, 1, t + i, e + r], ["L", t + i, e + s - r], ["A", r, r, 0, 0, 1, t + i - r, e + s], ["L", t + r, e + s], ["A", r, r, 0, 0, 1, t, e + s - r], ["L", t, e + r], ["A", r, r, 0, 0, 1, t + r, e], ["Z"]] } return { arc: r, callout: function (t, e, i, s, r) { let o = Math.min(r && r.r || 0, i, s), a = o + 6, n = r && r.anchorX, h = r && r.anchorY || 0, l = c(t, e, i, s, { r: o }); return !d(n) || n < i && 0 < n && h < s && 0 < h || (i - a < t + n ? e + a < h && h < e + s - a ? l.splice(3, 1, ["L", t + i, h - 6], ["L", t + i + 6, h], ["L", t + i, h + 6], ["L", t + i, e + s - o]) : n < i ? (r = h < e + a, l.splice(r ? 2 : 5, 0, ["L", n, h], ["L", t + i - o, r ? e : e + s])) : l.splice(3, 1, ["L", t + i, s / 2], ["L", n, h], ["L", t + i, s / 2], ["L", t + i, e + s - o]) : t + n < a ? e + a < h && h < e + s - a ? l.splice(7, 1, ["L", t, h + 6], ["L", t - 6, h], ["L", t, h - 6], ["L", t, e + o]) : 0 < n ? (r = h < e + a, l.splice(r ? 1 : 6, 0, ["L", n, h], ["L", t + o, r ? e : e + s])) : l.splice(7, 1, ["L", t, s / 2], ["L", n, h], ["L", t, s / 2], ["L", t, e + o]) : s < h && n < i - a ? l.splice(5, 1, ["L", n + 6, e + s], ["L", n, e + s + 6], ["L", n - 6, e + s], ["L", t + o, e + s]) : h < 0 && a < n && l.splice(1, 1, ["L", n - 6, e], ["L", n, e - 6], ["L", n + 6, e], ["L", i - o, e])), l }, circle: function (t, e, i, s) { return r(t + i / 2, e + s / 2, i / 2, s / 2, { start: .5 * Math.PI, end: 2.5 * Math.PI, open: !1 }) }, diamond: function (t, e, i, s) { return [["M", t + i / 2, e], ["L", t + i, e + s / 2], ["L", t + i / 2, e + s], ["L", t, e + s / 2], ["Z"]] }, rect: e, roundedRect: c, square: e, triangle: function (t, e, i, s) { return [["M", t + i / 2, e], ["L", t + i, e + s], ["L", t, e + s], ["Z"]] }, "triangle-down": function (t, e, i, s) { return [["M", t, e], ["L", t + i, e], ["L", t + i / 2, e + s], ["Z"]] } } }), e(t, "Core/Renderer/SVG/TextBuilder.js", [t["Core/Renderer/HTML/AST.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (h, t, e) { let { doc: c, SVG_NS: p, win: r } = t, { attr: u, extend: l, fireEvent: i, isString: d, objectEach: o, pick: g } = e; return class { constructor(t) { var e = t.styles; this.renderer = t.renderer, this.svgElement = t, this.width = t.textWidth, this.textLineHeight = e && e.lineHeight, this.textOutline = e && e.textOutline, this.ellipsis = !(!e || "ellipsis" !== e.textOverflow), this.noWrap = !(!e || "nowrap" !== e.whiteSpace) } buildSVG() { let e = this.svgElement, i = e.element, t = e.renderer, s = g(e.textStr, "").toString(), r = -1 !== s.indexOf("<"), o = i.childNodes, a = !e.added && t.box, n = [s, this.ellipsis, this.noWrap, this.textLineHeight, this.textOutline, e.getStyle("font-size"), this.width].join(","); if (n !== e.textCache) { e.textCache = n, delete e.actualWidth; for (let t = o.length; t--;)i.removeChild(o[t]); if (r || this.ellipsis || this.width || e.textPath || -1 !== s.indexOf(" ") && (!this.noWrap || /<br.*?>/g.test(s))) { if ("" !== s) { a && a.appendChild(i); let t = new h(s); this.modifyTree(t.nodes), t.addToDOM(i), this.modifyDOM(), this.ellipsis && -1 !== (i.textContent || "").indexOf("…") && e.attr("title", this.unescapeEntities(e.textStr || "", ["&lt;", "&gt;"])), a && a.removeChild(i) } } else i.appendChild(c.createTextNode(this.unescapeEntities(s))); d(this.textOutline) && e.applyTextOutline && e.applyTextOutline(this.textOutline) } } modifyDOM() { var t; let h = this.svgElement, l = u(h.element, "x"); for (h.firstLineMetrics = void 0; (t = h.element.firstChild) && /^[\s\u200B]*$/.test(t.textContent || " ");)h.element.removeChild(t);[].forEach.call(h.element.querySelectorAll("tspan.highcharts-br"), (t, e) => { t.nextSibling && t.previousSibling && (0 === e && 1 === t.previousSibling.nodeType && (h.firstLineMetrics = h.renderer.fontMetrics(t.previousSibling)), u(t, { dy: this.getLineHeight(t.nextSibling), x: l })) }); let d = this.width || 0; if (d) { let i = (i, s) => { let t = i.textContent || "", r = t.replace(/([^\^])-/g, "$1- ").split(" "), e = !this.noWrap && (1 < r.length || 1 < h.element.childNodes.length), o = this.getLineHeight(s), a = 0, n = h.actualWidth; if (this.ellipsis) t && this.truncate(i, t, void 0, 0, Math.max(0, d - .8 * o), (t, e) => t.substring(0, e) + "…"); else if (e) { let t = [], e = []; for (; s.firstChild && s.firstChild !== i;)e.push(s.firstChild), s.removeChild(s.firstChild); for (; r.length;)r.length && !this.noWrap && 0 < a && (t.push(i.textContent || ""), i.textContent = r.join(" ").replace(/- /g, "-")), this.truncate(i, void 0, r, 0 === a && n || 0, d, (t, e) => r.slice(0, e).join(" ").replace(/- /g, "-")), n = h.actualWidth, a++; e.forEach(t => { s.insertBefore(t, i) }), t.forEach(t => { s.insertBefore(c.createTextNode(t), i); let e = c.createElementNS(p, "tspan"); e.textContent = "​", u(e, { dy: o, x: l }), s.insertBefore(e, i) }) } }, s = e => { [].slice.call(e.childNodes).forEach(t => { t.nodeType === r.Node.TEXT_NODE ? i(t, e) : (-1 !== t.className.baseVal.indexOf("highcharts-br") && (h.actualWidth = 0), s(t)) }) }; s(h.element) } } getLineHeight(t) { t = t.nodeType === r.Node.TEXT_NODE ? t.parentElement : t; return this.textLineHeight ? parseInt(this.textLineHeight.toString(), 10) : this.renderer.fontMetrics(t || this.svgElement.element).h } modifyTree(n) { let h = (e, i) => { let { attributes: s = {}, children: t, style: r = {}, tagName: o } = e, a = this.renderer.styledMode; if ("b" === o || "strong" === o ? a ? s.class = "highcharts-strong" : r.fontWeight = "bold" : "i" !== o && "em" !== o || (a ? s.class = "highcharts-emphasized" : r.fontStyle = "italic"), r && r.color && (r.fill = r.color), "br" === o) { s.class = "highcharts-br", e.textContent = "​"; let t = n[i + 1]; t && t.textContent && (t.textContent = t.textContent.replace(/^ +/gm, "")) } else "a" === o && t && t.some(t => "#text" === t.tagName) && (e.children = [{ children: t, tagName: "tspan" }]); "#text" !== o && "a" !== o && (e.tagName = "tspan"), l(e, { attributes: s, style: r }), t && t.filter(t => "#text" !== t.tagName).forEach(h) }; n.forEach(h), i(this.svgElement, "afterModifyTree", { nodes: n }) } truncate(r, t, o, a, e, i) { function s(t, e) { let i = e || t, s = r.parentNode; if (s && void 0 === c[i] && s.getSubStringLength) try { c[i] = a + s.getSubStringLength(0, o ? i + 1 : i) } catch (t) { } return c[i] } let n, h, l = this.svgElement, d = l["rotation"], c = [], p = o ? 1 : 0, u = (t || o || "").length, g = u; if (l.rotation = 0, a + (h = s(r.textContent.length)) > e) { for (; p <= u;)g = Math.ceil((p + u) / 2), o && (n = i(o, g)), h = s(g, n && n.length - 1), p === u ? p = u + 1 : h > e ? u = g - 1 : p = g; 0 === u ? r.textContent = "" : t && u === t.length - 1 || (r.textContent = n || i(t || o, g)) } o && o.splice(0, g), l.actualWidth = h, l.rotation = d } unescapeEntities(i, s) { return o(this.renderer.escapes, function (t, e) { s && -1 !== s.indexOf(t) || (i = i.toString().replace(RegExp(t, "g"), e)) }), i } } }), e(t, "Core/Renderer/SVG/SVGRenderer.js", [t["Core/Renderer/HTML/AST.js"], t["Core/Defaults.js"], t["Core/Color/Color.js"], t["Core/Globals.js"], t["Core/Renderer/RendererRegistry.js"], t["Core/Renderer/SVG/SVGElement.js"], t["Core/Renderer/SVG/SVGLabel.js"], t["Core/Renderer/SVG/Symbols.js"], t["Core/Renderer/SVG/TextBuilder.js"], t["Core/Utilities.js"]], function (y, t, e, i, s, r, l, o, a, n) { let h, b = t["defaultOptions"], { charts: f, deg2rad: d, doc: m, isFirefox: c, isMS: v, isWebKit: p, noop: u, SVG_NS: g, symbolSizes: x, win: S } = i, { addEvent: k, attr: C, createElement: M, crisp: w, css: A, defined: T, destroyObjectProperties: P, extend: L, isArray: O, isNumber: E, isObject: D, isString: R, merge: j, pick: I, pInt: z, replaceNested: N, uniqueKey: W } = n; class B { constructor(t, e, i, s, r, o, a) { let n, h, l = this.createElement("svg").attr({ version: "1.1", class: "highcharts-root" }), d = l.element; a || l.css(this.getStyle(s || {})), t.appendChild(d), C(t, "dir", "ltr"), -1 === t.innerHTML.indexOf("xmlns") && C(d, "xmlns", this.SVG_NS), this.box = d, this.boxWrapper = l, this.alignedObjects = [], this.url = this.getReferenceURL(), this.createElement("desc").add().element.appendChild(m.createTextNode("Created with Highcharts 11.4.8")), this.defs = this.createElement("defs").add(), this.allowHTML = o, this.forExport = r, this.styledMode = a, this.gradients = {}, this.cache = {}, this.cacheKeys = [], this.imgCount = 0, this.rootFontSize = l.getStyle("font-size"), this.setSize(e, i, !1), c && t.getBoundingClientRect && ((n = function () { A(t, { left: 0, top: 0 }), h = t.getBoundingClientRect(), A(t, { left: Math.ceil(h.left) - h.left + "px", top: Math.ceil(h.top) - h.top + "px" }) })(), this.unSubPixelFix = k(S, "resize", n)) } definition(t) { return new y([t]).addToDOM(this.defs.element) } getReferenceURL() { var t, e; if ((c || p) && m.getElementsByTagName("base").length && (T(h) || (t = W(), t = new y([{ tagName: "svg", attributes: { width: 8, height: 8 }, children: [{ tagName: "defs", children: [{ tagName: "clipPath", attributes: { id: t }, children: [{ tagName: "rect", attributes: { width: 4, height: 4 } }] }] }, { tagName: "rect", attributes: { id: "hitme", width: 8, height: 8, "clip-path": `url(#${t})`, fill: "rgba(0,0,0,0.001)" } }] }]).addToDOM(m.body), A(t, { position: "fixed", top: 0, left: 0, zIndex: 9e5 }), e = m.elementFromPoint(6, 6), h = "hitme" === (e && e.id), m.body.removeChild(t)), h)) return N(S.location.href.split("#")[0], [/<[^>]*>/g, ""], [/([\('\)])/g, "\\$1"], [/ /g, "%20"]); return "" } getStyle(t) { return this.style = L({ fontFamily: "Helvetica, Arial, sans-serif", fontSize: "1rem" }, t), this.style } setStyle(t) { this.boxWrapper.css(this.getStyle(t)) } isHidden() { return !this.boxWrapper.getBBox().width } destroy() { let t = this.defs; return this.box = null, this.boxWrapper = this.boxWrapper.destroy(), P(this.gradients || {}), this.gradients = null, this.defs = t.destroy(), this.unSubPixelFix && this.unSubPixelFix(), this.alignedObjects = null } createElement(t) { return new this.Element(this, t) } getRadialAttr(t, e) { return { cx: t[0] - t[2] / 2 + (e.cx || 0) * t[2], cy: t[1] - t[2] / 2 + (e.cy || 0) * t[2], r: (e.r || 0) * t[2] } } shadowDefinition(e) { var t = ["highcharts-drop-shadow-" + this.chartIndex, ...Object.keys(e).map(t => t + "-" + e[t])].join("-").toLowerCase().replace(/[^a-z\d\-]/g, ""), i = j({ color: "#000000", offsetX: 1, offsetY: 1, opacity: .15, width: 5 }, e); return this.defs.element.querySelector("#" + t) || this.definition({ tagName: "filter", attributes: { id: t, filterUnits: i.filterUnits }, children: this.getShadowFilterContent(i) }), t } getShadowFilterContent(t) { return [{ tagName: "feDropShadow", attributes: { dx: t.offsetX, dy: t.offsetY, "flood-color": t.color, "flood-opacity": Math.min(5 * t.opacity, 1), stdDeviation: t.width / 2 } }] } buildText(t) { new a(t).buildSVG() } getContrast(t) { t = e.parse(t).rgba.map(t => { t /= 255; return t <= .03928 ? t / 12.92 : Math.pow((.055 + t) / 1.055, 2.4) }), t = .2126 * t[0] + .7152 * t[1] + .0722 * t[2]; return (.05 + t) / .05 < 1.05 / (.05 + t) ? "#FFFFFF" : "#000000" } button(t, e, i, s, r = {}, o, a, n, h, l) { let d = this.label(t, e, i, h, void 0, void 0, l, void 0, "button"), c = this.styledMode, p = arguments, u = 0, g = (r = j(b.global.buttonTheme, r), c && (delete r.fill, delete r.stroke, delete r["stroke-width"]), r.states || {}), f = r.style || {}, m = (delete r.states, delete r.style, [y.filterUserAttributes(r)]), x = [f]; return c || ["hover", "select", "disabled"].forEach((t, e) => { m.push(j(m[0], y.filterUserAttributes(p[e + 5] || g[t] || {}))), x.push(m[e + 1].style), delete m[e + 1].style }), k(d.element, v ? "mouseover" : "mouseenter", function () { 3 !== u && d.setState(1) }), k(d.element, v ? "mouseout" : "mouseleave", function () { 3 !== u && d.setState(u) }), d.setState = (t = 0) => { 1 !== t && (d.state = u = t), d.removeClass(/highcharts-button-(normal|hover|pressed|disabled)/).addClass("highcharts-button-" + ["normal", "hover", "pressed", "disabled"][t]), c || (d.attr(m[t]), t = x[t], D(t) && d.css(t)) }, d.attr(m[0]), c || (d.css(L({ cursor: "default" }, f)), l && d.text.css({ pointerEvents: "none" })), d.on("touchstart", t => t.stopPropagation()).on("click", function (t) { 3 !== u && s.call(d, t) }) } crispLine(t, e) { let [i, s] = t; return T(i[1]) && i[1] === s[1] && (i[1] = s[1] = w(i[1], e)), T(i[2]) && i[2] === s[2] && (i[2] = s[2] = w(i[2], e)), t } path(t) { let e = this.styledMode ? {} : { fill: "none" }; return O(t) ? e.d = t : D(t) && L(e, t), this.createElement("path").attr(e) } circle(t, e, i) { let s = D(t) ? t : void 0 === t ? {} : { x: t, y: e, r: i }, r = this.createElement("circle"); return r.xSetter = r.ySetter = function (t, e, i) { i.setAttribute("c" + e, t) }, r.attr(s) } arc(t, e, i, s, r, o) { let a, n = (D(t) ? (e = (a = t).y, i = a.r, s = a.innerR, r = a.start, o = a.end, t = a.x) : a = { innerR: s, start: r, end: o }, this.symbol("arc", t, e, i, i, a)); return n.r = i, n } rect(t, e, i, s, r, o) { let a = D(t) ? t : void 0 === t ? {} : { x: t, y: e, r: r, width: Math.max(i || 0, 0), height: Math.max(s || 0, 0) }, n = this.createElement("rect"); return this.styledMode || (void 0 !== o && (a["stroke-width"] = o, L(a, n.crisp(a))), a.fill = "none"), n.rSetter = function (t, e, i) { n.r = t, C(i, { rx: t, ry: t }) }, n.rGetter = function () { return n.r || 0 }, n.attr(a) } roundedRect(t) { return this.symbol("roundedRect").attr(t) } setSize(t, e, i) { this.width = t, this.height = e, this.boxWrapper.animate({ width: t, height: e }, { step: function () { this.attr({ viewBox: "0 0 " + this.attr("width") + " " + this.attr("height") }) }, duration: I(i, !0) ? void 0 : 0 }), this.alignElements() } g(t) { let e = this.createElement("g"); return t ? e.attr({ class: "highcharts-" + t }) : e } image(e, t, i, s, r, o) { function a(t) { h.attr({ href: e }), o.call(h, t) } let n = { preserveAspectRatio: "none" }, h = (E(t) && (n.x = t), E(i) && (n.y = i), E(s) && (n.width = s), E(r) && (n.height = r), this.createElement("image").attr(n)); if (o) { h.attr({ href: "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==" }); let t = new S.Image; k(t, "load", a), t.src = e, t.complete && a({}) } else h.attr({ href: e }); return h } symbol(t, i, s, e, r, d) { let o, a, n, h, l = this, c = /^url\((.*?)\)$/, p = c.test(t), u = !p && (this.symbols[t] ? t : "circle"), g = u && this.symbols[u]; if (g) "number" == typeof i && (a = g.call(this.symbols, i || 0, s || 0, e || 0, r || 0, d)), o = this.path(a), l.styledMode || o.attr("fill", "none"), L(o, { symbolName: u || void 0, x: i, y: s, width: e, height: r }), d && L(o, d); else if (p) { n = t.match(c)[1]; let e = o = this.image(n); e.imgwidth = I(d && d.width, x[n] && x[n].width), e.imgheight = I(d && d.height, x[n] && x[n].height), h = t => t.attr({ width: t.width, height: t.height }), ["width", "height"].forEach(t => { e[t + "Setter"] = function (t, e) { this[e] = t; let { alignByTranslate: i, element: s, width: r, height: o, imgwidth: a, imgheight: n } = this, h = "width" === e ? a : n, l = 1; d && "within" === d.backgroundSize && r && o && a && n ? (l = Math.min(r / a, o / n), C(s, { width: Math.round(a * l), height: Math.round(n * l) })) : s && h && s.setAttribute(e, h), !i && a && n && this.translate(((r || 0) - a * l) / 2, ((o || 0) - n * l) / 2) } }), T(i) && e.attr({ x: i, y: s }), e.isImg = !0, e.symbolUrl = t, T(e.imgwidth) && T(e.imgheight) ? h(e) : (e.attr({ width: 0, height: 0 }), M("img", { onload: function () { let t = f[l.chartIndex]; 0 === this.width && (A(this, { position: "absolute", top: "-999em" }), m.body.appendChild(this)), x[n] = { width: this.width, height: this.height }, e.imgwidth = this.width, e.imgheight = this.height, e.element && h(e), this.parentNode && this.parentNode.removeChild(this), l.imgCount--, l.imgCount || !t || t.hasLoaded || t.onload() }, src: n }), this.imgCount++) } return o } clipRect(t, e, i, s) { return this.rect(t, e, i, s, 0) } text(t, e, i, s) { let r = {}; if (s && (this.allowHTML || !this.forExport)) return this.html(t, e, i); r.x = Math.round(e || 0), i && (r.y = Math.round(i)), T(t) && (r.text = t); let o = this.createElement("text").attr(r); return s && (!this.forExport || this.allowHTML) || (o.xSetter = function (i, s, t) { let r = t.getElementsByTagName("tspan"), o = t.getAttribute(s); for (let t = 0, e; t < r.length; t++)(e = r[t]).getAttribute(s) === o && e.setAttribute(s, i); t.setAttribute(s, i) }), o } fontMetrics(t) { var t = z(r.prototype.getStyle.call(t, "font-size") || 0), e = t < 24 ? t + 3 : Math.round(1.2 * t); return { h: e, b: Math.round(.8 * e), f: t } } rotCorr(t, e, i) { let s = t; return e && i && (s = Math.max(s * Math.cos(e * d), 4)), { x: -t / 3 * Math.sin(e * d), y: s } } pathToSegments(e) { let i = [], s = [], r = { A: 8, C: 7, H: 2, L: 3, M: 3, Q: 5, S: 5, T: 3, V: 2 }; for (let t = 0; t < e.length; t++)R(s[0]) && E(e[t]) && s.length === r[s[0].toUpperCase()] && e.splice(t, 0, s[0].replace("M", "L").replace("m", "l")), "string" == typeof e[t] && (s.length && i.push(s.slice(0)), s.length = 0), s.push(e[t]); return i.push(s.slice(0)), i } label(t, e, i, s, r, o, a, n, h) { return new l(this, t, e, i, s, r, o, a, n, h) } alignElements() { this.alignedObjects.forEach(t => t.align()) } } return L(B.prototype, { Element: r, SVG_NS: g, escapes: { "&": "&amp;", "<": "&lt;", ">": "&gt;", "'": "&#39;", '"': "&quot;" }, symbols: o, draw: u }), s.registerRendererType("svg", B, !0), B }), e(t, "Core/Renderer/HTML/HTMLElement.js", [t["Core/Renderer/HTML/AST.js"], t["Core/Globals.js"], t["Core/Renderer/SVG/SVGElement.js"], t["Core/Utilities.js"]], function (e, t, a, i) { let s = t["composed"], { attr: n, css: b, createElement: h, defined: v, extend: o, pInt: l, pushUnique: r } = i; function d(t, e, i) { let s = this.div?.style || i.style; a.prototype[e + "Setter"].call(this, t, e, i), s && (s[e] = t) } class c extends a { static compose(t) { r(s, this.compose) && (t.prototype.html = function (t, e, i) { return new c(this, "span").attr({ text: t, x: Math.round(e), y: Math.round(i) }) }) } constructor(t, e) { super(t, e), this.css({ position: "absolute", ...t.styledMode ? {} : { fontFamily: t.style.fontFamily, fontSize: t.style.fontSize } }), this.element.style.whiteSpace = "nowrap" } getSpanCorrection(t, e, i) { this.xCorr = -t * i, this.yCorr = -e } css(t) { let e; var i = this["element"], s = "SPAN" === i.tagName && t && "width" in t, r = s && t.width; return s && (delete t.width, this.textWidth = l(r) || void 0, e = !0), "ellipsis" === t?.textOverflow && (t.whiteSpace = "nowrap", t.overflow = "hidden"), o(this.styles, t), b(i, t), e && this.updateTransform(), this } htmlGetBBox() { var t = this["element"]; return { x: t.offsetLeft, y: t.offsetTop, width: t.offsetWidth, height: t.offsetHeight } } updateTransform() { if (!this.added) return void (this.alignOnAdd = !0); let { element: r, renderer: o, rotation: a, rotationOriginX: n, rotationOriginY: h, styles: t, textAlign: l = "left", textWidth: d, translateX: e = 0, translateY: i = 0, x: c = 0, y: p = 0 } = this, u = { left: 0, center: .5, right: 1 }[l], g = t.whiteSpace; if (b(r, { marginLeft: e + "px", marginTop: i + "px" }), "SPAN" === r.tagName) { let t = [a, l, r.innerHTML, d, this.textAlign].join(","), e = -+this.parentGroup?.padding || 0, i, s = !1; d !== this.oldTextWidth && (m = this.textPxLength || (b(r, { width: "", whiteSpace: g || "nowrap" }), r.offsetWidth), ((f = d || 0) > this.oldTextWidth || f < m) && (/[ \-]/.test(r.textContent || r.innerText) || "ellipsis" === r.style.textOverflow) && (b(r, { width: f < m || a ? d + "px" : "auto", display: "block", whiteSpace: g || "normal" }), this.oldTextWidth = d, s = !0)), this.hasBoxWidthChanged = s, t !== this.cTT && (i = o.fontMetrics(r).b, !v(a) || a === (this.oldRotation || 0) && l === this.oldAlign || this.setSpanRotation(a, e, e), this.getSpanCorrection(!v(a) && this.textPxLength || r.offsetWidth, i, u)); var { xCorr: f = 0, yCorr: m = 0 } = this, x = (n ?? c) - f - c - e, y = (h ?? p) - m - p - e; b(r, { left: c + f + "px", top: p + m + "px", transformOrigin: x + `px ${y}px` }), this.cTT = t, this.oldRotation = a, this.oldAlign = l } } setSpanRotation(t, e, i) { b(this.element, { transform: `rotate(${t}deg)`, transformOrigin: e + `% ${i}px` }) } add(e) { let i, s = this.renderer.box.parentNode, r = []; if ((this.parentGroup = e) && !(i = e.div)) { let t = e; for (; t;)r.push(t), t = t.parentGroup; for (var o of r.reverse()) i = ((i, r) => { if (!i.div) { let t = n(i.element, "class"), e = i.css, s = h("div", t ? { className: t } : void 0, { position: "absolute", left: `${i.translateX || 0}px`, top: `${i.translateY || 0}px`, ...i.styles, display: i.display, opacity: i.opacity, visibility: i.visibility }, i.parentGroup?.div || r); i.classSetter = (t, e, i) => { i.setAttribute("class", t), s.className = t }, i.translateXSetter = i.translateYSetter = (t, e) => { i[e] = t, s.style["translateX" === e ? "left" : "top"] = t + "px", i.doTransform = !0 }, i.opacitySetter = i.visibilitySetter = d, i.css = t => (e.call(i, t), t.cursor && (s.style.cursor = t.cursor), t.pointerEvents && (s.style.pointerEvents = t.pointerEvents), i), i.on = function () { return a.prototype.on.apply({ element: s, onEvents: i.onEvents }, arguments), i }, i.div = s } return i.div })(o, s) } return (i || s).appendChild(this.element), this.added = !0, this.alignOnAdd && this.updateTransform(), this } textSetter(t) { t !== this.textStr && (delete this.bBox, delete this.oldTextWidth, e.setElementHTML(this.element, t ?? ""), this.textStr = t, this.doTransform = !0) } alignSetter(t) { this.alignValue = this.textAlign = t, this.doTransform = !0 } xSetter(t, e) { this[e] = t, this.doTransform = !0 } } let p = c.prototype; return p.visibilitySetter = p.opacitySetter = d, p.ySetter = p.rotationSetter = p.rotationOriginXSetter = p.rotationOriginYSetter = p.xSetter, c }), e(t, "Core/Axis/AxisDefaults.js", [], function () { var t; t = { xAxis: { alignTicks: !0, allowDecimals: void 0, panningEnabled: !0, zIndex: 2, zoomEnabled: !0, dateTimeLabelFormats: { millisecond: { main: "%H:%M:%S.%L", range: !1 }, second: { main: "%H:%M:%S", range: !1 }, minute: { main: "%H:%M", range: !1 }, hour: { main: "%H:%M", range: !1 }, day: { main: "%e %b" }, week: { main: "%e %b" }, month: { main: "%b '%y" }, year: { main: "%Y" } }, endOnTick: !1, gridLineDashStyle: "Solid", gridZIndex: 1, labels: { autoRotationLimit: 80, distance: 15, enabled: !0, indentation: 10, overflow: "justify", reserveSpace: void 0, rotation: void 0, staggerLines: 0, step: 0, useHTML: !1, zIndex: 7, style: { color: "#333333", cursor: "default", fontSize: "0.8em" } }, maxPadding: .01, minorGridLineDashStyle: "Solid", minorTickLength: 2, minorTickPosition: "outside", minorTicksPerMajor: 5, minPadding: .01, offset: void 0, reversed: void 0, reversedStacks: !1, showEmpty: !0, showFirstLabel: !0, showLastLabel: !0, startOfWeek: 1, startOnTick: !1, tickLength: 10, tickPixelInterval: 100, tickmarkPlacement: "between", tickPosition: "outside", title: { align: "middle", useHTML: !1, x: 0, y: 0, style: { color: "#666666", fontSize: "0.8em" } }, visible: !0, minorGridLineColor: "#f2f2f2", minorGridLineWidth: 1, minorTickColor: "#999999", lineColor: "#333333", lineWidth: 1, gridLineColor: "#e6e6e6", gridLineWidth: void 0, tickColor: "#333333" }, yAxis: { reversedStacks: !0, endOnTick: !0, maxPadding: .05, minPadding: .05, tickPixelInterval: 72, showLastLabel: !0, labels: { x: void 0 }, startOnTick: !0, title: { text: "Values" }, stackLabels: { animation: {}, allowOverlap: !1, enabled: !1, crop: !0, overflow: "justify", formatter: function () { let t = this.axis.chart["numberFormatter"]; return t(this.total || 0, -1) }, style: { color: "#000000", fontSize: "0.7em", fontWeight: "bold", textOutline: "1px contrast" } }, gridLineWidth: 1, lineWidth: 0 } }; return t }), e(t, "Core/Foundation.js", [t["Core/Utilities.js"]], function (t) { let { addEvent: s, isFunction: r, objectEach: e, removeEvent: o } = t; return { registerEventOptions: function (i, t) { i.eventOptions = i.eventOptions || {}, e(t.events, function (t, e) { i.eventOptions[e] !== t && (i.eventOptions[e] && (o(i, e, i.eventOptions[e]), delete i.eventOptions[e]), r(t) && (i.eventOptions[e] = t, s(i, e, t, { order: 0 }))) }) } } }), e(t, "Core/Axis/Tick.js", [t["Core/Templating.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (C, t, e) { let y = t["deg2rad"], { clamp: h, correctFloat: M, defined: w, destroyObjectProperties: i, extend: A, fireEvent: T, isNumber: P, merge: a, objectEach: n, pick: L } = e; return class { constructor(t, e, i, s, r) { this.isNew = !0, this.isNewLabel = !0, this.axis = t, this.pos = e, this.type = i || "", this.parameters = r || {}, this.tickmarkOffset = this.parameters.tickmarkOffset, this.options = this.parameters.options, T(this, "init"), i || s || this.addLabel() } addLabel() { let t = this, e = t.axis, i = e.options, s = e.chart, r = e.categories, o = e.logarithmic, a = e.names, n = t.pos, h = L(t.options && t.options.labels, i.labels), l = e.tickPositions, d = n === l[0], c = n === l[l.length - 1], p = (!h.step || 1 === h.step) && 1 === e.tickInterval, u = l.info, g = t.label, f, m, x, y = this.parameters.category || (r ? L(r[n], a[n], n) : n), b = (o && P(y) && (y = M(o.lin2log(y))), e.dateTime && (u ? f = (m = s.time.resolveDTLFormat(i.dateTimeLabelFormats[!i.grid && u.higherRanks[n] || u.unitName])).main : P(y) && (f = e.dateTime.getXDateFormat(y, i.dateTimeLabelFormats || {}))), t.isFirst = d, t.isLast = c, { axis: e, chart: s, dateTimeLabelFormat: f, isFirst: d, isLast: c, pos: n, tick: t, tickPositionInfo: u, value: y }), v = (T(this, "labelFormat", b), t => h.formatter ? h.formatter.call(t, t) : h.format ? (t.text = e.defaultLabelFormatter.call(t), C.format(h.format, t, s)) : e.defaultLabelFormatter.call(t)), S = v.call(b, b), k = m && m.list; k ? t.shortenLabel = function () { for (x = 0; x < k.length; x++)if (A(b, { dateTimeLabelFormat: k[x] }), g.attr({ text: v.call(b, b) }), g.getBBox().width < e.getSlotWidth(t) - 2 * (h.padding || 0)) return; g.attr({ text: "" }) } : t.shortenLabel = void 0, p && e._addedPlotLB && t.moveLabel(S, h), w(g) || t.movedLabel ? g && g.textStr !== S && !p && (!g.textWidth || h.style.width || g.styles.width || g.css({ width: null }), g.attr({ text: S }), g.textPxLength = g.getBBox().width) : (t.label = g = t.createLabel(S, h), t.rotation = 0) } createLabel(t, e, i) { let s = this.axis, r = s.chart, o = w(t) && e.enabled ? r.renderer.text(t, i?.x, i?.y, e.useHTML).add(s.labelGroup) : void 0; return o && (r.styledMode || o.css(a(e.style)), o.textPxLength = o.getBBox().width), o } destroy() { i(this, this.axis) } getPosition(t, e, i, s) { let r = this.axis, o = r.chart, a = s && o.oldChartHeight || o.chartHeight, n = { x: t ? M(r.translate(e + i, void 0, void 0, s) + r.transB) : r.left + r.offset + (r.opposite ? (s && o.oldChartWidth || o.chartWidth) - r.right - r.left : 0), y: t ? a - r.bottom + r.offset - (r.opposite ? r.height : 0) : M(a - r.translate(e + i, void 0, void 0, s) - r.transB) }; return n.y = h(n.y, -1e9, 1e9), T(this, "afterGetPosition", { pos: n }), n } getLabelPosition(t, e, i, s, r, o, a, n) { let h, l, d = this.axis, c = d.transA, p = (d.isLinked && d.linkedParent ? d.linkedParent : d).reversed, u = d.staggerLines, g = d.tickRotCorr || { x: 0, y: 0 }, f = s || d.reserveSpaceDefault ? 0 : -d.labelOffset * ("center" === d.labelAlign ? .5 : 1), m = r.distance, x = {}; return h = 0 === d.side ? i.rotation ? -m : -i.getBBox().height : 2 === d.side ? g.y + m : Math.cos(i.rotation * y) * (g.y - i.getBBox(!1, 0).height / 2), w(r.y) && (h = 0 === d.side && d.horiz ? r.y + h : r.y), t = t + L(r.x, [0, 1, 0, -1][d.side] * m) + f + g.x - (o && s ? o * c * (p ? -1 : 1) : 0), e = e + h - (o && !s ? o * c * (p ? 1 : -1) : 0), u && (l = a / (n || 1) % u, e += (l = d.opposite ? u - l - 1 : l) * (d.labelOffset / u)), x.x = t, x.y = Math.round(e), T(this, "afterGetLabelPosition", { pos: x, tickmarkOffset: o, index: a }), x } getLabelSize() { return this.label ? this.label.getBBox()[this.axis.horiz ? "height" : "width"] : 0 } getMarkPath(t, e, i, s, r = !1, o) { return o.crispLine([["M", t, e], ["L", t + (r ? 0 : -i), e + (r ? i : 0)]], s) } handleOverflow(t) { let e = this.axis, i = e.options.labels, s = t.x, r = e.chart.chartWidth, o = e.chart.spacing, a = L(e.labelLeft, Math.min(e.pos, o[3])), n = L(e.labelRight, Math.max(e.isRadial ? 0 : e.pos + e.len, r - o[1])), h = this.label, l = this.rotation, d = { left: 0, center: .5, right: 1 }[e.labelAlign || h.attr("align")], c = h.getBBox().width, p = e.getSlotWidth(this), u = {}, g = p, f = 1, m; l || "justify" !== i.overflow ? l < 0 && s - d * c < a ? m = Math.round(s / Math.cos(l * y) - a) : 0 < l && n < s + d * c && (m = Math.round((r - s) / Math.cos(l * y))) : (s - d * c < a ? g = t.x + g * (1 - d) - a : n < s + (1 - d) * c && (g = n - t.x + g * d, f = -1), (g = Math.min(p, g)) < p && "center" === e.labelAlign && (t.x += f * (p - g - d * (p - Math.min(c, g)))), (c > g || e.autoRotation && (h.styles || {}).width) && (m = g)), m && (this.shortenLabel ? this.shortenLabel() : (u.width = Math.floor(m) + "px", (i.style || {}).textOverflow || (u.textOverflow = "ellipsis"), h.css(u))) } moveLabel(e, t) { let i = this, s = i.label, r = i.axis, o = !1, a; s && s.textStr === e ? (i.movedLabel = s, o = !0, delete i.label) : n(r.ticks, function (t) { o || t.isNew || t === i || !t.label || t.label.textStr !== e || (i.movedLabel = t.label, o = !0, t.labelPos = i.movedLabel.xy, delete t.label) }), o || !i.labelPos && !s || (a = i.labelPos || s.xy, i.movedLabel = i.createLabel(e, t, a), i.movedLabel && i.movedLabel.attr({ opacity: 0 })) } render(t, e, i) { var s = this.axis, r = s.horiz, o = this.pos, a = L(this.tickmarkOffset, s.tickmarkOffset), o = this.getPosition(r, o, a, e), a = o.x, n = o.y, h = s.pos, l = h + s.len, r = r ? a : n, a = (!s.chart.polar && this.isNew && (M(r) < h || l < r) && (i = 0), L(i, this.label && this.label.newOpacity, 1)); i = L(i, 1), this.isActive = !0, this.renderGridLine(e, i), this.renderMark(o, i), this.renderLabel(o, e, a, t), this.isNew = !1, T(this, "afterRender") } renderGridLine(t, e) { let i = this.axis, s = i.options, r = {}, o = this.pos, a = this.type, n = L(this.tickmarkOffset, i.tickmarkOffset), h = i.chart.renderer, l = this.gridLine, d, c = s.gridLineWidth, p = s.gridLineColor, u = s.gridLineDashStyle; "minor" === this.type && (c = s.minorGridLineWidth, p = s.minorGridLineColor, u = s.minorGridLineDashStyle), l || (i.chart.styledMode || (r.stroke = p, r["stroke-width"] = c || 0, r.dashstyle = u), a || (r.zIndex = 1), t && (e = 0), this.gridLine = l = h.path().attr(r).addClass("highcharts-" + (a ? a + "-" : "") + "grid-line").add(i.gridGroup)), l && (d = i.getPlotLinePath({ value: o + n, lineWidth: l.strokeWidth(), force: "pass", old: t, acrossPanes: !1 })) && l[t || this.isNew ? "attr" : "animate"]({ d: d, opacity: e }) } renderMark(t, e) { let i = this.axis, s = i.options, r = i.chart.renderer, o = this.type, a = i.tickSize(o ? o + "Tick" : "tick"), n = t.x, h = t.y, l = L(s["minor" !== o ? "tickWidth" : "minorTickWidth"], !o && i.isXAxis ? 1 : 0), d = s["minor" !== o ? "tickColor" : "minorTickColor"], c = this.mark, p = !c; a && (i.opposite && (a[0] = -a[0]), c || (this.mark = c = r.path().addClass("highcharts-" + (o ? o + "-" : "") + "tick").add(i.axisGroup), i.chart.styledMode || c.attr({ stroke: d, "stroke-width": l })), c[p ? "attr" : "animate"]({ d: this.getMarkPath(n, h, a[0], c.strokeWidth(), i.horiz, r), opacity: e })) } renderLabel(t, e, i, s) { let r = this.axis, o = r.horiz, a = r.options, n = this.label, h = a.labels, l = h.step, d = L(this.tickmarkOffset, r.tickmarkOffset), c = t.x, p = t.y, u = !0; n && P(c) && (n.xy = t = this.getLabelPosition(c, p, n, o, h, d, s, l), this.isFirst && !this.isLast && !a.showFirstLabel || this.isLast && !this.isFirst && !a.showLastLabel ? u = !1 : !o || h.step || h.rotation || e || 0 === i || this.handleOverflow(t), (u = l && s % l ? !1 : u) && P(t.y) ? (t.opacity = i, n[this.isNewLabel ? "attr" : "animate"](t).show(!0), this.isNewLabel = !1) : (n.hide(), this.isNewLabel = !0)) } replaceMovedLabel() { let t = this.label, e = this.axis; t && !this.isNew && (t.animate({ opacity: 0 }, void 0, t.destroy), delete this.label), e.isDirty = !0, this.label = this.movedLabel, delete this.movedLabel } } }), e(t, "Core/Axis/Axis.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Axis/AxisDefaults.js"], t["Core/Color/Color.js"], t["Core/Defaults.js"], t["Core/Foundation.js"], t["Core/Globals.js"], t["Core/Axis/Tick.js"], t["Core/Utilities.js"]], function (t, e, d, i, s, v, S, r) { let k = t["animObject"], { xAxis: o, yAxis: a } = e, u = i["defaultOptions"], n = s["registerEventOptions"], g = v["deg2rad"], { arrayMax: l, arrayMin: c, clamp: b, correctFloat: L, defined: O, destroyObjectProperties: h, erase: p, error: E, extend: f, fireEvent: D, getClosestDistance: m, insertItem: x, isArray: y, isNumber: j, isString: C, merge: M, normalizeTickInterval: w, objectEach: A, pick: I, relativeLength: T, removeEvent: R, splat: z, syncTimeout: N } = r, B = (t, e) => w(e, void 0, void 0, I(t.options.allowDecimals, e < .5 || void 0 !== t.tickAmount), !!t.tickAmount); f(u, { xAxis: o, yAxis: M(o, a) }); class P { constructor(t, e, i) { this.init(t, e, i) } init(t, e, i = this.coll) { var s = "xAxis" === i, r = this.isZAxis || (t.inverted ? !s : s), i = (this.chart = t, this.horiz = r, this.isXAxis = s, this.coll = i, D(this, "init", { userOptions: e }), this.opposite = I(e.opposite, this.opposite), this.side = I(e.side, this.side, r ? this.opposite ? 0 : 2 : this.opposite ? 1 : 3), this.setOptions(e), this.options), r = i.labels, e = (this.type ?? (this.type = i.type || "linear"), this.uniqueNames ?? (this.uniqueNames = i.uniqueNames ?? !0), D(this, "afterSetType"), this.userOptions = e, this.minPixelPadding = 0, this.reversed = I(i.reversed, this.reversed), this.visible = i.visible, this.zoomEnabled = i.zoomEnabled, this.hasNames = "category" === this.type || !0 === i.categories, this.categories = y(i.categories) && i.categories || (this.hasNames ? [] : void 0), this.names || (this.names = [], this.names.keys = {}), this.plotLinesAndBandsGroups = {}, this.positiveValuesOnly = !!this.logarithmic, this.isLinked = O(i.linkedTo), this.ticks = {}, this.labelEdge = [], this.minorTicks = {}, this.plotLinesAndBands = [], this.alternateBands = {}, this.len ?? (this.len = 0), this.minRange = this.userMinRange = i.minRange || i.maxZoom, this.range = i.range, this.offset = i.offset || 0, this.max = void 0, this.min = void 0, I(i.crosshair, z(t.options.tooltip.crosshairs)[s ? 0 : 1])); this.crosshair = !0 === e ? {} : e, -1 === t.axes.indexOf(this) && (s ? t.axes.splice(t.xAxis.length, 0, this) : t.axes.push(this), x(this, t[this.coll])), t.orderItems(this.coll), this.series = this.series || [], t.inverted && !this.isZAxis && s && !O(this.reversed) && (this.reversed = !0), this.labelRotation = j(r.rotation) ? r.rotation : void 0, n(this, i), D(this, "afterInit") } setOptions(t) { var e = this.horiz ? { labels: { autoRotation: [-45], padding: 4 }, margin: 15 } : { labels: { padding: 1 }, title: { rotation: 90 * this.side } }; this.options = M(e, u[this.coll], t), D(this, "afterSetOptions", { userOptions: t }) } defaultLabelFormatter() { let t = this.axis, e = this.chart["numberFormatter"], i = j(this.value) ? this.value : NaN, s = t.chart.time, r = t.categories, o = this.dateTimeLabelFormat, a = u.lang, n = a.numericSymbols, h = a.numericSymbolMagnitude || 1e3, l = t.logarithmic ? Math.abs(i) : t.tickInterval, d = n && n.length, c, p; if (r) p = "" + this.value; else if (o) p = s.dateFormat(o, i); else if (d && n && 1e3 <= l) for (; d-- && void 0 === p;)l >= (c = Math.pow(h, d + 1)) && 10 * i % c == 0 && null !== n[d] && 0 !== i && (p = e(i / c, -1) + n[d]); return p = void 0 === p ? 1e4 <= Math.abs(i) ? e(i, -1) : e(i, -1, void 0, "") : p } getSeriesExtremes() { let a, n = this; D(this, "getSeriesExtremes", null, function () { n.hasVisibleSeries = !1, n.dataMin = n.dataMax = n.threshold = void 0, n.softThreshold = !n.isXAxis, n.series.forEach(o => { if (o.reserveSpace()) { let t = o.options, e, i = t.threshold, s, r; n.hasVisibleSeries = !0, n.positiveValuesOnly && (i || 0) <= 0 && (i = void 0), n.isXAxis ? (e = o.xData) && e.length && (e = n.logarithmic ? e.filter(t => 0 < t) : e, s = (a = o.getXExtremes(e)).min, r = a.max, j(s) || s instanceof Date || (e = e.filter(j), s = (a = o.getXExtremes(e)).min, r = a.max), e.length && (n.dataMin = Math.min(I(n.dataMin, s), s), n.dataMax = Math.max(I(n.dataMax, r), r))) : (o = o.applyExtremes(), j(o.dataMin) && (s = o.dataMin, n.dataMin = Math.min(I(n.dataMin, s), s)), j(o.dataMax) && (r = o.dataMax, n.dataMax = Math.max(I(n.dataMax, r), r)), O(i) && (n.threshold = i), t.softThreshold && !n.positiveValuesOnly || (n.softThreshold = !1)) } }) }), D(this, "afterGetSeriesExtremes") } translate(t, e, i, s, r, o) { let a = this.linkedParent || this, n = (s && a.old ? a.old : a).min; if (!j(n)) return NaN; let h = a.minPixelPadding, l = (a.isOrdinal || a.brokenAxis?.hasBreaks || a.logarithmic && r) && a.lin2val, d = 1, c = 0, p = (s && a.old ? a.old : a).transA, u = 0; return p = p || a.transA, i && (d *= -1, c = a.len), a.reversed && (d *= -1, c -= d * (a.sector || a.len)), e ? (u = (t = t * d + c - h) / p + n, l && (u = a.lin2val(u))) : (l && (t = a.val2lin(t)), u = d * (t - n) * p + c + d * h + (j(o) ? p * o : 0), a.isRadial || (u = L(u))), u } toPixels(t, e) { return this.translate(t, !1, !this.horiz, void 0, !0) + (e ? 0 : this.pos) } toValue(t, e) { return this.translate(t - (e ? 0 : this.pos), !0, !this.horiz, void 0, !0) } getPlotLinePath(t) { let e = this, i = e.chart, s = e.left, r = e.top, o = t.old, a = t.value, n = t.lineWidth, h = o && i.oldChartHeight || i.chartHeight, l = o && i.oldChartWidth || i.chartWidth, d = e.transB, c = t.translatedValue, p = t.force, u, g, f, m, x; function y(t, e, i) { return "pass" !== p && (t < e || i < t) && (p ? t = b(t, e, i) : x = !0), t } t = { value: a, lineWidth: n, old: o, force: p, acrossPanes: t.acrossPanes, translatedValue: c }; return D(this, "getPlotLinePath", t, function (t) { u = f = (c = b(c = I(c, e.translate(a, void 0, void 0, o)), -1e9, 1e9)) + d, g = m = h - c - d, j(c) ? e.horiz ? (g = r, m = h - e.bottom + (!e.options.isInternal && i.scrollablePixelsY || 0), u = f = y(u, s, s + e.width)) : (u = s, f = l - e.right + (i.scrollablePixelsX || 0), g = m = y(g, r, r + e.height)) : (x = !0, p = !1), t.path = x && !p ? void 0 : i.renderer.crispLine([["M", u, g], ["L", f, m]], n || 1) }), t.path } getLinearTickPositions(t, e, i) { let s, r, o, a = L(Math.floor(e / t) * t), n = L(Math.ceil(i / t) * t), h = []; if (L(a + t) === a && (o = 20), this.single) return [e]; for (s = a; s <= n && (h.push(s), (s = L(s + t, o)) !== r);)r = s; return h } getMinorTickInterval() { var { minorTicks: t, minorTickInterval: e } = this.options; return !0 === t ? I(e, "auto") : !1 !== t ? e : void 0 } getMinorTickPositions() { let t = this.options, e = this.tickPositions, r = this.minorTickInterval, i = this.pointRangePadding || 0, o = (this.min || 0) - i, a = (this.max || 0) + i, s = a - o, n = [], h; if (s && s / r < this.len / 3) { let s = this.logarithmic; if (s) this.paddedTicks.forEach(function (t, e, i) { e && n.push.apply(n, s.getLogTickPositions(r, i[e - 1], i[e], !0)) }); else if (this.dateTime && "auto" === this.getMinorTickInterval()) n = n.concat(this.getTimeTicks(this.dateTime.normalizeTimeTickInterval(r), o, a, t.startOfWeek)); else for (h = o + (e[0] - o) % r; h <= a && h !== n[0]; h += r)n.push(h) } return 0 !== n.length && this.trimTicks(n), n } adjustForMinRange() { let t = this.options, e = this.logarithmic, { max: i, min: s, minRange: r } = this, o, a, n, h; this.isXAxis && void 0 === r && !e && (r = O(t.min) || O(t.max) || O(t.floor) || O(t.ceiling) ? null : Math.min(5 * (m(this.series.map(t => (t.xIncrement ? t.xData?.slice(0, 2) : t.xData) || [])) || 0), this.dataMax - this.dataMin)), j(i) && j(s) && j(r) && i - s < r && (a = this.dataMax - this.dataMin >= r, o = (r - i + s) / 2, n = [s - o, I(t.min, s - o)], a && (n[2] = e ? e.log2lin(this.dataMin) : this.dataMin), h = [(s = l(n)) + r, I(t.max, s + r)], a && (h[2] = e ? e.log2lin(this.dataMax) : this.dataMax), (i = c(h)) - s < r && (n[0] = i - r, n[1] = I(t.min, i - r), s = l(n))), this.minRange = r, this.min = s, this.max = i } getClosest() { let t, s; if (this.categories) s = 1; else { let i = []; this.series.forEach(function (t) { var e = t.closestPointRange; 1 === t.xData?.length ? i.push(t.xData[0]) : !t.noSharedTooltip && O(e) && t.reserveSpace() && (s = O(s) ? Math.min(s, e) : e) }), i.length && (i.sort((t, e) => t - e), t = m([i])) } return t && s ? Math.min(t, s) : t || s } nameToX(t) { let e = y(this.options.categories), i = e ? this.categories : this.names, s = t.options.x, r; return t.series.requireSorting = !1, -1 === (s = O(s) ? s : this.uniqueNames && i ? e ? i.indexOf(t.name) : I(i.keys[t.name], -1) : t.series.autoIncrement()) ? !e && i && (r = i.length) : r = s, void 0 !== r ? (this.names[r] = t.name, this.names.keys[t.name] = r) : t.x && (r = t.x), r } updateNames() { let r = this, e = this.names; 0 < e.length && (Object.keys(e.keys).forEach(function (t) { delete e.keys[t] }), e.length = 0, this.minRange = this.userMinRange, (this.series || []).forEach(s => { s.xIncrement = null, s.points && !s.isDirtyData || (r.max = Math.max(r.max, s.xData.length - 1), s.processData(), s.generatePoints()), s.data.forEach(function (t, e) { let i; t?.options && void 0 !== t.name && void 0 !== (i = r.nameToX(t)) && i !== t.x && (t.x = i, s.xData[e] = i) }) })) } setAxisTranslation() { let s = this, t = s.max - s.min, e = s.linkedParent, r = !!s.categories, o = s.isXAxis, a = s.axisPointRange || 0, n, h = 0, l = 0, i, d = void s.transA; (o || r || a) && (n = s.getClosest(), e ? (h = e.minPointOffset, l = e.pointRangePadding) : s.series.forEach(function (t) { var e = r ? 1 : o ? I(t.options.pointRange, n, 0) : s.axisPointRange || 0, i = t.options.pointPlacement; a = Math.max(a, e), s.single && !r || (t = t.is("xrange") ? !o : o, h = Math.max(h, t && C(i) ? 0 : e / 2), l = Math.max(l, t && "on" === i ? 0 : e)) }), i = s.ordinal && s.ordinal.slope && n ? s.ordinal.slope / n : 1, s.minPointOffset = h *= i, s.pointRangePadding = l *= i, s.pointRange = Math.min(a, s.single && r ? 1 : t), o && n && (s.closestPointRange = n)), s.translationSlope = s.transA = d = s.staticScale || s.len / (t + l || 1), s.transB = s.horiz ? s.left : s.bottom, s.minPixelPadding = d * h, D(this, "afterSetAxisTranslation") } minFromRange() { var { max: t, min: e } = this; return j(t) && j(e) && t - e || void 0 } setTickInterval(t) { let { categories: e, chart: i, dataMax: s, dataMin: r, dateTime: o, isXAxis: a, logarithmic: n, options: h, softThreshold: l } = this, d = j(this.threshold) ? this.threshold : void 0, c = this.minRange || 0, { ceiling: p, floor: u, linkedTo: g, softMax: f, softMin: m } = h, x = j(g) && i[this.coll]?.[g], y = h.tickPixelInterval, b = h.maxPadding, v = h.minPadding, S = 0, k, C = j(h.tickInterval) && 0 <= h.tickInterval ? h.tickInterval : void 0, M, w, A, T; if (o || e || x || this.getTickAmount(), A = I(this.userMin, h.min), T = I(this.userMax, h.max), x ? (k = (this.linkedParent = x).getExtremes(), this.min = I(k.min, k.dataMin), this.max = I(k.max, k.dataMax), this.type !== x.type && E(11, !0, i)) : (l && O(d) && j(s) && j(r) && (d <= r ? (M = d, v = 0) : s <= d && (w = d, b = 0)), this.min = I(A, M, r), this.max = I(T, w, s)), j(this.max) && j(this.min) && (n && (this.positiveValuesOnly && !t && Math.min(this.min, I(r, this.min)) <= 0 && E(10, !0, i), this.min = L(n.log2lin(this.min), 16), this.max = L(n.log2lin(this.max), 16)), this.range && j(r) && (this.userMin = this.min = A = Math.max(r, this.minFromRange() || 0), this.userMax = T = this.max, this.range = void 0)), D(this, "foundExtremes"), this.adjustForMinRange(), j(this.min) && j(this.max) && (!j(this.userMin) && j(m) && m < this.min && (this.min = A = m), !j(this.userMax) && j(f) && f > this.max && (this.max = T = f), e || this.axisPointRange || this.stacking?.usePercentage || x || !(S = this.max - this.min) || (!O(A) && v && (this.min -= S * v), !O(T) && b && (this.max += S * b)), !j(this.userMin) && j(u) && (this.min = Math.max(this.min, u)), !j(this.userMax) && j(p) && (this.max = Math.min(this.max, p)), l && j(r) && j(s) && (P = d || 0, !O(A) && this.min < P && P <= r ? this.min = h.minRange ? Math.min(P, this.max - c) : P : !O(T) && this.max > P && s <= P && (this.max = h.minRange ? Math.max(P, this.min + c) : P)), !i.polar && this.min > this.max && (O(h.min) ? this.max = this.min : O(h.max) && (this.min = this.max)), S = this.max - this.min), this.min !== this.max && j(this.min) && j(this.max) ? x && !C && y === x.options.tickPixelInterval ? this.tickInterval = C = x.tickInterval : this.tickInterval = I(C, this.tickAmount ? S / Math.max(this.tickAmount - 1, 1) : void 0, e ? 1 : S * y / Math.max(this.len, y)) : this.tickInterval = 1, a && !t) { let e = this.min !== this.old?.min || this.max !== this.old?.max; this.series.forEach(function (t) { t.forceCrop = t.forceCropping?.(), t.processData(e) }), D(this, "postProcessData", { hasExtremesChanged: e }) } this.setAxisTranslation(), D(this, "initialAxisTranslation"), this.pointRange && !C && (this.tickInterval = Math.max(this.pointRange, this.tickInterval)); var P = I(h.minTickInterval, o && !this.series.some(t => t.noSharedTooltip) ? this.closestPointRange : 0); !C && this.tickInterval < P && (this.tickInterval = P), o || n || C || (this.tickInterval = B(this, this.tickInterval)), this.tickAmount || (this.tickInterval = this.unsquish()), this.setTickPositions() } setTickPositions() { let t = this.options, e = t.tickPositions, i = t.tickPositioner, s = this.getMinorTickInterval(), r = !this.isPanning, o = r && t.startOnTick, a = r && t.endOnTick, n = [], h; if (this.tickmarkOffset = this.categories && "between" === t.tickmarkPlacement && 1 === this.tickInterval ? .5 : 0, this.single = this.min === this.max && O(this.min) && !this.tickAmount && (this.min % 1 == 0 || !1 !== t.allowDecimals), e) n = e.slice(); else if (j(this.min) && j(this.max)) { if (!this.ordinal?.positions && (this.max - this.min) / this.tickInterval > Math.max(2 * this.len, 200)) n = [this.min, this.max], E(19, !1, this.chart); else if (this.dateTime) n = this.getTimeTicks(this.dateTime.normalizeTimeTickInterval(this.tickInterval, t.units), this.min, this.max, t.startOfWeek, this.ordinal?.positions, this.closestPointRange, !0); else if (this.logarithmic) n = this.logarithmic.getLogTickPositions(this.tickInterval, this.min, this.max); else { let t = this.tickInterval, e = t; for (; e <= 2 * t && (n = this.getLinearTickPositions(this.tickInterval, this.min, this.max), this.tickAmount && n.length > this.tickAmount);)this.tickInterval = B(this, e *= 1.1) } n.length > this.len && (n = [n[0], n[n.length - 1]])[0] === n[1] && (n.length = 1), i && (this.tickPositions = n, (h = i.apply(this, [this.min, this.max])) && (n = h)) } this.tickPositions = n, this.minorTickInterval = "auto" === s && this.tickInterval ? this.tickInterval / t.minorTicksPerMajor : s, this.paddedTicks = n.slice(0), this.trimTicks(n, o, a), !this.isLinked && j(this.min) && j(this.max) && (this.single && n.length < 2 && !this.categories && !this.series.some(t => t.is("heatmap") && "between" === t.options.pointPlacement) && (this.min -= .5, this.max += .5), e || h || this.adjustTickAmount()), D(this, "afterSetTickPositions") } trimTicks(t, e, i) { var s = t[0], r = t[t.length - 1], o = !this.isOrdinal && this.minPointOffset || 0; if (D(this, "trimTicks"), !this.isLinked) { if (e && s !== -1 / 0) this.min = s; else for (; this.min - o > t[0];)t.shift(); if (i) this.max = r; else for (; this.max + o < t[t.length - 1];)t.pop(); 0 === t.length && O(s) && !this.options.tickPositions && t.push((r + s) / 2) } } alignToOthers() { let r, o = this, t = o.chart, a = [this], e = o.options, i = t.options.chart, s = "yAxis" === this.coll && i.alignThresholds, n = []; if (o.thresholdAlignment = void 0, (!1 !== i.alignTicks && e.alignTicks || s) && !1 !== e.startOnTick && !1 !== e.endOnTick && !o.logarithmic) { let i = t => { let { horiz: e, options: i } = t; return [e ? i.left : i.top, i.width, i.height, i.pane].join(",") }, s = i(this); t[this.coll].forEach(function (t) { let e = t["series"]; e.length && e.some(t => t.visible) && t !== o && i(t) === s && (r = !0, a.push(t)) }) } if (r && s) { a.forEach(t => { t = t.getThresholdAlignment(o); j(t) && n.push(t) }); let e = 1 < n.length ? n.reduce((t, e) => t += e, 0) / n.length : void 0; a.forEach(t => { t.thresholdAlignment = e }) } return r } getThresholdAlignment(t) { if ((!j(this.dataMin) || this !== t && this.series.some(t => t.isDirty || t.isDirtyData)) && this.getSeriesExtremes(), j(this.threshold)) { let t = b((this.threshold - (this.dataMin || 0)) / ((this.dataMax || 0) - (this.dataMin || 0)), 0, 1); return t = this.options.reversed ? 1 - t : t } } getTickAmount() { let t = this.options, e = t.tickPixelInterval, i = t.tickAmount; (i = !(i = !O(t.tickInterval) && !i && this.len < e && !this.isRadial && !this.logarithmic && t.startOnTick && t.endOnTick ? 2 : i) && this.alignToOthers() ? Math.ceil(this.len / e) + 1 : i) < 4 && (this.finalTickAmt = i, i = 5), this.tickAmount = i } adjustTickAmount() { let t = this, { finalTickAmt: e, max: i, min: s, options: r, tickPositions: o, tickAmount: a, thresholdAlignment: n } = t, h = o?.length, l = I(t.threshold, t.softThreshold ? 0 : null), d, c, p = t.tickInterval, u, g = () => o.push(L(o[o.length - 1] + p)), f = () => o.unshift(L(o[0] - p)); if (j(n) && (u = n < .5 ? Math.ceil(n * (a - 1)) : Math.floor(n * (a - 1)), r.reversed && (u = a - 1 - u)), t.hasData() && j(s) && j(i)) { var m = () => { t.transA *= (h - 1) / (a - 1), t.min = r.startOnTick ? o[0] : Math.min(s, o[0]), t.max = r.endOnTick ? o[o.length - 1] : Math.max(i, o[o.length - 1]) }; if (j(u) && j(t.threshold)) { for (; o[u] !== l || o.length !== a || o[0] > s || o[o.length - 1] < i;) { for (o.length = 0, o.push(t.threshold); o.length < a;)(void 0 === o[u] || o[u] > t.threshold ? f : g)(); if (p > 8 * t.tickInterval) break; p *= 2 } m() } else if (h < a) { for (; o.length < a;)(o.length % 2 || s === l ? g : f)(); m() } if (O(e)) { for (c = d = o.length; c--;)(3 === e && c % 2 == 1 || e <= 2 && 0 < c && c < d - 1) && o.splice(c, 1); t.finalTickAmt = void 0 } } } setScale() { let { coll: t, stacking: e } = this, i = !1, s = !1; this.series.forEach(t => { i = i || t.isDirtyData || t.isDirty, s = s || t.xAxis && t.xAxis.isDirty || !1 }), this.setAxisSize(); var r = this.len !== (this.old && this.old.len); r || i || s || this.isLinked || this.forceRedraw || this.userMin !== (this.old && this.old.userMin) || this.userMax !== (this.old && this.old.userMax) || this.alignToOthers() ? (e && "yAxis" === t && e.buildStacks(), this.forceRedraw = !1, this.userMinRange || (this.minRange = void 0), this.getSeriesExtremes(), this.setTickInterval(), e && "xAxis" === t && e.buildStacks(), this.isDirty || (this.isDirty = r || this.min !== this.old?.min || this.max !== this.old?.max)) : e && e.cleanStacks(), i && delete this.allExtremes, D(this, "afterSetScale") } setExtremes(t, e, i = !0, s, r) { this.series.forEach(t => { delete t.kdTree }), D(this, "setExtremes", r = f(r, { min: t, max: e }), t => { this.userMin = t.min, this.userMax = t.max, this.eventArgs = t, i && this.chart.redraw(s) }) } setAxisSize() { var t = this.chart, e = this.options, i = e.offsets || [0, 0, 0, 0], s = this.horiz, r = this.width = Math.round(T(I(e.width, t.plotWidth - i[3] + i[1]), t.plotWidth)), o = this.height = Math.round(T(I(e.height, t.plotHeight - i[0] + i[2]), t.plotHeight)), a = this.top = Math.round(T(I(e.top, t.plotTop + i[0]), t.plotHeight, t.plotTop)), e = this.left = Math.round(T(I(e.left, t.plotLeft + i[3]), t.plotWidth, t.plotLeft)); this.bottom = t.chartHeight - o - a, this.right = t.chartWidth - r - e, this.len = Math.max(s ? r : o, 0), this.pos = s ? e : a } getExtremes() { let t = this.logarithmic; return { min: t ? L(t.lin2log(this.min)) : this.min, max: t ? L(t.lin2log(this.max)) : this.max, dataMin: this.dataMin, dataMax: this.dataMax, userMin: this.userMin, userMax: this.userMax } } getThreshold(t) { let e = this.logarithmic, i = e ? e.lin2log(this.min) : this.min, s = e ? e.lin2log(this.max) : this.max; return null === t || t === -1 / 0 ? t = i : t === 1 / 0 ? t = s : t < i ? t = i : s < t && (t = s), this.translate(t, 0, 1, 0, 1) } autoLabelAlign(t) { let e = (I(t, 0) - 90 * this.side + 720) % 360, i = { align: "center" }; return D(this, "autoLabelAlign", i, function (t) { 15 < e && e < 165 ? t.align = "right" : 195 < e && e < 345 && (t.align = "left") }), i.align } tickSize(t) { let e = this.options, i = I(e["tick" === t ? "tickWidth" : "minorTickWidth"], "tick" === t && this.isXAxis && !this.categories ? 1 : 0), s = e["tick" === t ? "tickLength" : "minorTickLength"], r; i && s && ("inside" === e[t + "Position"] && (s = -s), r = [s, i]); t = { tickSize: r }; return D(this, "afterTickSize", t), t.tickSize } labelMetrics() { var t = this.chart.renderer, e = this.ticks, e = e[Object.keys(e)[0]] || {}; return this.chart.renderer.fontMetrics(e.label || e.movedLabel || t.box) } unsquish() { function i(t) { let e = (t + 2 * s) / (o || 1); return (e = 1 < e ? Math.ceil(e) : 1) * r > h && t !== 1 / 0 && o !== 1 / 0 && h && (e = Math.ceil(h / r)), L(e * r) } let t = this.options.labels, s = t.padding || 0, e = this.horiz, r = this.tickInterval, o = this.len / (((this.categories ? 1 : 0) + this.max - this.min) / r), a = t.rotation, n = L(.8 * this.labelMetrics().h), h = Math.max(this.max - this.min, 0), l = r, d, c = Number.MAX_VALUE, p; if (e) { if (t.staggerLines || (j(a) ? p = [a] : o < t.autoRotationLimit && (p = t.autoRotation)), p) { let t, e; for (var u of p) (u === a || u && -90 <= u && u <= 90) && (e = (t = i(Math.abs(n / Math.sin(g * u)))) + Math.abs(u / 360)) < c && (c = e, d = u, l = t) } } else l = i(.75 * n); return this.autoRotation = p, this.labelRotation = I(d, j(a) ? a : 0), t.step ? r : l } getSlotWidth(t) { var e = this.chart, i = this.horiz, s = this.options.labels, r = Math.max(this.tickPositions.length - (this.categories ? 0 : 1), 1), o = e.margin[3]; if (t && j(t.slotWidth)) return t.slotWidth; if (i && s.step < 2) return s.rotation ? 0 : (this.staggerLines || 1) * this.len / r; if (!i) { t = s.style.width; if (void 0 !== t) return parseInt(String(t), 10); if (o) return o - e.spacing[3] } return .33 * e.chartWidth } renderUnsquish() { let t = this.chart, e = t.renderer, i = this.tickPositions, o = this.ticks, s = this.options.labels, a = s.style, r = this.horiz, n = this.getSlotWidth(), h = Math.max(1, Math.round(n - (r ? 2 * (s.padding || 0) : s.distance || 0))), l = {}, d = this.labelMetrics(), c = a.textOverflow, p, u, g = 0, f, m; if (C(s.rotation) || (l.rotation = s.rotation || 0), i.forEach(function (t) { let e = o[t]; e.movedLabel && e.replaceMovedLabel(), e && e.label && e.label.textPxLength > g && (g = e.label.textPxLength) }), this.maxLabelLength = g, this.autoRotation) g > h && g > d.h ? l.rotation = this.labelRotation : this.labelRotation = 0; else if (n && (p = h, !c)) for (u = "clip", m = i.length; !r && m--;)(f = o[i[m]].label) && ("ellipsis" === f.styles.textOverflow ? f.css({ textOverflow: "clip" }) : f.textPxLength > n && f.css({ width: n + "px" }), f.getBBox().height > this.len / i.length - (d.h - d.f) && (f.specificTextOverflow = "ellipsis")); l.rotation && (p = g > .5 * t.chartHeight ? .33 * t.chartHeight : g, c || (u = "ellipsis")), this.labelAlign = s.align || this.autoLabelAlign(this.labelRotation), this.labelAlign && (l.align = this.labelAlign), i.forEach(function (t) { let e = o[t], i = e && e.label, s = a.width, r = {}; i && (i.attr(l), e.shortenLabel ? e.shortenLabel() : p && !s && "nowrap" !== a.whiteSpace && (p < i.textPxLength || "SPAN" === i.element.tagName) ? (r.width = p + "px", c || (r.textOverflow = i.specificTextOverflow || u), i.css(r)) : !i.styles.width || r.width || s || i.css({ width: null }), delete i.specificTextOverflow, e.rotation = l.rotation) }, this), this.tickRotCorr = e.rotCorr(d.b, this.labelRotation || 0, 0 !== this.side) } hasData() { return this.series.some(function (t) { return t.hasData() }) || this.options.showEmpty && O(this.min) && O(this.max) } addTitle(t) { let e, i = this.chart.renderer, s = this.horiz, r = this.opposite, o = this.options.title, a = this.chart.styledMode; this.axisTitle || ((e = o.textAlign) || (e = (s ? { low: "left", middle: "center", high: "right" } : { low: r ? "right" : "left", middle: "center", high: r ? "left" : "right" })[o.align]), this.axisTitle = i.text(o.text || "", 0, 0, o.useHTML).attr({ zIndex: 7, rotation: o.rotation || 0, align: e }).addClass("highcharts-axis-title"), a || this.axisTitle.css(M(o.style)), this.axisTitle.add(this.axisGroup), this.axisTitle.isNew = !0), a || o.style.width || this.isRadial || this.axisTitle.css({ width: this.len + "px" }), this.axisTitle[t ? "show" : "hide"](t) } generateTick(t) { let e = this.ticks; e[t] ? e[t].addLabel() : e[t] = new S(this, t) } createGroups() { let { axisParent: s, chart: t, coll: r, options: o } = this, a = t.renderer, e = (t, e, i) => a.g(t).attr({ zIndex: i }).addClass(`highcharts-${r.toLowerCase()}${e} ` + (this.isRadial ? `highcharts-radial-axis${e} ` : "") + (o.className || "")).add(s); this.axisGroup || (this.gridGroup = e("grid", "-grid", o.gridZIndex), this.axisGroup = e("axis", "", o.zIndex), this.labelGroup = e("axis-labels", "-labels", o.labels.zIndex)) } getOffset() { let e = this, { chart: t, horiz: i, options: s, side: r, ticks: o, tickPositions: a, coll: n } = e, h = t.inverted && !e.isZAxis ? [1, 0, 3, 2][r] : r, l = e.hasData(), d = s.title, c = s.labels, p = j(s.crossing), u = t.axisOffset, g = t.clipOffset, f = [-1, 1, 1, -1][r], m, x = 0, y, b = 0, v = 0, S, k; var C; e.showAxis = m = l || s.showEmpty, e.staggerLines = e.horiz && c.staggerLines || void 0, e.createGroups(), l || e.isLinked ? (a.forEach(function (t) { e.generateTick(t) }), e.renderUnsquish(), e.reserveSpaceDefault = 0 === r || 2 === r || { 1: "left", 3: "right" }[r] === e.labelAlign, I(c.reserveSpace, !p && null, "center" === e.labelAlign || null, e.reserveSpaceDefault) && a.forEach(function (t) { v = Math.max(o[t].getLabelSize(), v) }), e.staggerLines && (v *= e.staggerLines), e.labelOffset = v * (e.opposite ? -1 : 1)) : A(o, function (t, e) { t.destroy(), delete o[e] }), d?.text && !1 !== d.enabled && (e.addTitle(m), m && !p && !1 !== d.reserveSpace && (e.titleOffset = x = e.axisTitle.getBBox()[i ? "height" : "width"], b = O(y = d.offset) ? 0 : I(d.margin, i ? 5 : 10))), e.renderLine(), e.offset = f * I(s.offset, u[r] ? u[r] + (s.margin || 0) : 0), e.tickRotCorr = e.tickRotCorr || { x: 0, y: 0 }, k = 0 === r ? -e.labelMetrics().h : 2 === r ? e.tickRotCorr.y : 0, S = Math.abs(v) + b, v && (S = (S -= k) + f * (i ? I(c.y, e.tickRotCorr.y + f * c.distance) : I(c.x, f * c.distance))), e.axisTitleMargin = I(y, S), e.getMaxLabelDimensions && (e.maxLabelDimensions = e.getMaxLabelDimensions(o, a)), "colorAxis" !== n && g && (C = this.tickSize("tick"), u[r] = Math.max(u[r], (e.axisTitleMargin || 0) + x + f * e.offset, S, a && a.length && C ? C[0] + f * e.offset : 0), C = !e.axisLine || s.offset ? 0 : e.axisLine.strokeWidth() / 2, g[h] = Math.max(g[h], C)), D(this, "afterGetOffset") } getLinePath(t) { let e = this.chart, i = this.opposite, s = this.offset, r = this.horiz, o = this.left + (i ? this.width : 0) + s, a = e.chartHeight - this.bottom - (i ? this.height : 0) + s; return i && (t *= -1), e.renderer.crispLine([["M", r ? this.left : o, r ? a : this.top], ["L", r ? e.chartWidth - this.right : o, r ? a : e.chartHeight - this.bottom]], t) } renderLine() { this.axisLine || (this.axisLine = this.chart.renderer.path().addClass("highcharts-axis-line").add(this.axisGroup), this.chart.styledMode || this.axisLine.attr({ stroke: this.options.lineColor, "stroke-width": this.options.lineWidth, zIndex: 7 })) } getTitlePosition(t) { var e = this.horiz, i = this.left, s = this.top, r = this.len, o = this.options.title, a = e ? i : s, n = this.opposite, h = this.offset, l = o.x, d = o.y, c = this.chart.renderer.fontMetrics(t), t = t ? Math.max(t.getBBox(!1, 0).height - c.h - 1, 0) : 0, a = { low: a + (e ? 0 : r), middle: a + r / 2, high: a + (e ? r : 0) }[o.align], r = (e ? s + this.height : i) + (e ? 1 : -1) * (n ? -1 : 1) * (this.axisTitleMargin || 0) + [-t, t, c.f, -t][this.side], o = { x: e ? a + l : r + (n ? this.width : 0) + h + l, y: e ? r + d - (n ? this.height : 0) + h : a + d }; return D(this, "afterGetTitlePosition", { titlePosition: o }), o } renderMinorTick(t, e) { let i = this.minorTicks; i[t] || (i[t] = new S(this, t, "minor")), e && i[t].isNew && i[t].render(null, !0), i[t].render(null, !1, 1) } renderTick(t, e, i) { let s = this.isLinked, r = this.ticks; (!s || t >= this.min && t <= this.max || this.grid && this.grid.isColumn) && (r[t] || (r[t] = new S(this, t)), i && r[t].isNew && r[t].render(e, !0, -1), r[t].render(e)) } render() { let s, r, o = this, a = o.chart, n = o.logarithmic, t = a.renderer, e = o.options, i = o.isLinked, h = o.tickPositions, l = o.axisTitle, d = o.ticks, c = o.minorTicks, p = o.alternateBands, u = e.stackLabels, g = e.alternateGridColor, f = e.crossing, m = o.tickmarkOffset, x = o.axisLine, y = o.showAxis, b = k(t.globalAnimation); if (o.labelEdge.length = 0, o.overlap = !1, [d, c, p].forEach(function (t) { A(t, function (t) { t.isActive = !1 }) }), j(f)) { let e = (this.isXAxis ? a.yAxis : a.xAxis)[0], i = [1, -1, -1, 1][this.side]; if (e) { let t = e.toPixels(f, !0); o.horiz && (t = e.len - t), o.offset = i * t } } if (o.hasData() || i) { let i = o.chart.hasRendered && o.old && j(o.old.min); o.minorTickInterval && !o.categories && o.getMinorTickPositions().forEach(function (t) { o.renderMinorTick(t, i) }), h.length && (h.forEach(function (t, e) { o.renderTick(t, e, i) }), m && (0 === o.min || o.single) && (d[-1] || (d[-1] = new S(o, -1, null, !0)), d[-1].render(-1))), g && h.forEach(function (t, e) { r = void 0 !== h[e + 1] ? h[e + 1] + m : o.max - m, e % 2 == 0 && t < o.max && r <= o.max + (a.polar ? -m : m) && (p[t] || (p[t] = new v.PlotLineOrBand(o, {})), s = t + m, p[t].options = { from: n ? n.lin2log(s) : s, to: n ? n.lin2log(r) : r, color: g, className: "highcharts-alternate-grid" }, p[t].render(), p[t].isActive = !0) }), o._addedPlotLB || (o._addedPlotLB = !0, (e.plotLines || []).concat(e.plotBands || []).forEach(function (t) { o.addPlotBandOrLine(t) })) } [d, c, p].forEach(function (e) { let i = [], t = b.duration; A(e, function (t, e) { t.isActive || (t.render(e, !1, 0), t.isActive = !1, i.push(e)) }), N(function () { let t = i.length; for (; t--;)e[i[t]] && !e[i[t]].isActive && (e[i[t]].destroy(), delete e[i[t]]) }, e !== p && a.hasRendered && t ? t : 0) }), x && (x[x.isPlaced ? "animate" : "attr"]({ d: this.getLinePath(x.strokeWidth()) }), x.isPlaced = !0, x[y ? "show" : "hide"](y)), l && y && (l[l.isNew ? "attr" : "animate"](o.getTitlePosition(l)), l.isNew = !1), u && u.enabled && o.stacking && o.stacking.renderStackTotals(), o.old = { len: o.len, max: o.max, min: o.min, transA: o.transA, userMax: o.userMax, userMin: o.userMin }, o.isDirty = !1, D(this, "afterRender") } redraw() { this.visible && (this.render(), this.plotLinesAndBands.forEach(function (t) { t.render() })), this.series.forEach(function (t) { t.isDirty = !0 }) } getKeepProps() { return this.keepProps || P.keepProps } destroy(t) { let i = this, e = i.plotLinesAndBands, s = this.eventOptions; if (D(this, "destroy", { keepEvents: t }), t || R(i), [i.ticks, i.minorTicks, i.alternateBands].forEach(function (t) { h(t) }), e) { let t = e.length; for (; t--;)e[t].destroy() } for (var r in ["axisLine", "axisTitle", "axisGroup", "gridGroup", "labelGroup", "cross", "scrollbar"].forEach(function (t) { i[t] && (i[t] = i[t].destroy()) }), i.plotLinesAndBandsGroups) i.plotLinesAndBandsGroups[r] = i.plotLinesAndBandsGroups[r].destroy(); A(i, function (t, e) { -1 === i.getKeepProps().indexOf(e) && delete i[e] }), this.eventOptions = s } drawCrosshair(t, e) { let i = this.crosshair, s = I(i && i.snap, !0), r = this.chart, o, a, n, h = this.cross, l; if (D(this, "drawCrosshair", { e: t, point: e }), t = t || this.cross && this.cross.e, i && !1 !== (O(e) || !s)) { if (s ? O(e) && (a = I("colorAxis" !== this.coll ? e.crosshairPos : null, this.isXAxis ? e.plotX : this.len - e.plotY)) : a = t && (this.horiz ? t.chartX - this.pos : this.len - t.chartY + this.pos), O(a) && (l = { value: e && (this.isXAxis ? e.x : I(e.stackY, e.y)), translatedValue: a }, r.polar && f(l, { isCrosshair: !0, chartX: t && t.chartX, chartY: t && t.chartY, point: e }), o = this.getPlotLinePath(l) || null), !O(o)) return void this.hideCrosshair(); n = this.categories && !this.isRadial, h || (this.cross = h = r.renderer.path().addClass("highcharts-crosshair highcharts-crosshair-" + (n ? "category " : "thin ") + (i.className || "")).attr({ zIndex: I(i.zIndex, 2) }).add(), r.styledMode || (h.attr({ stroke: i.color || (n ? d.parse("#ccd3ff").setOpacity(.25).get() : "#cccccc"), "stroke-width": I(i.width, 1) }).css({ "pointer-events": "none" }), i.dashStyle && h.attr({ dashstyle: i.dashStyle }))), h.show().attr({ d: o }), n && !i.width && h.attr({ "stroke-width": this.transA }), this.cross.e = t } else this.hideCrosshair(); D(this, "afterDrawCrosshair", { e: t, point: e }) } hideCrosshair() { this.cross && this.cross.hide(), D(this, "afterHideCrosshair") } update(t, e) { let i = this.chart; t = M(this.userOptions, t), this.destroy(!0), this.init(i, t), i.isDirtyBox = !0, I(e, !0) && i.redraw() } remove(t) { let e = this.chart, i = this.coll, s = this.series, r = s.length; for (; r--;)s[r] && s[r].remove(!1); p(e.axes, this), p(e[i] || [], this), e.orderItems(i), this.destroy(), e.isDirtyBox = !0, I(t, !0) && e.redraw() } setTitle(t, e) { this.update({ title: t }, e) } setCategories(t, e) { this.update({ categories: t }, e) } } return P.keepProps = ["coll", "extKey", "hcEvents", "len", "names", "series", "userMax", "userMin"], P }), e(t, "Core/Axis/DateTimeAxis.js", [t["Core/Utilities.js"]], function (t) { var e; let { addEvent: i, getMagnitude: n, normalizeTickInterval: h, timeUnits: l } = t; { function s() { return this.chart.time.getTimeTicks.apply(this.chart.time, arguments) } function r() { "datetime" !== this.type ? this.dateTime = void 0 : this.dateTime || (this.dateTime = new o(this)) } (t = e = e || {}).compose = function (t) { return t.keepProps.includes("dateTime") || (t.keepProps.push("dateTime"), t.prototype.getTimeTicks = s, i(t, "afterSetType", r)), t }; class o { constructor(t) { this.axis = t } normalizeTimeTickInterval(t, e) { let i = e || [["millisecond", [1, 2, 5, 10, 20, 25, 50, 100, 200, 500]], ["second", [1, 2, 5, 10, 15, 30]], ["minute", [1, 2, 5, 10, 15, 30]], ["hour", [1, 2, 3, 4, 6, 8, 12]], ["day", [1, 2]], ["week", [1, 2]], ["month", [1, 2, 3, 4, 6]], ["year", null]], s = i[i.length - 1], r = l[s[0]], o = s[1], a; for (a = 0; a < i.length && (r = l[(s = i[a])[0]], o = s[1], !(i[a + 1] && t <= (r * o[o.length - 1] + l[i[a + 1][0]]) / 2)); a++); r === l.year && t < 5 * r && (o = [1, 2, 5]); e = h(t / r, o, "year" === s[0] ? Math.max(n(t / r), 1) : 1); return { unitRange: r, count: e, unitName: s[0] } } getXDateFormat(t, e) { let i = this["axis"], s = i.chart.time; return i.closestPointRange ? s.getDateFormat(i.closestPointRange, t, i.options.startOfWeek, e) || s.resolveDTLFormat(e.year).main : s.resolveDTLFormat(e.day).main } } t.Additions = o } return e }), e(t, "Core/Axis/LogarithmicAxis.js", [t["Core/Utilities.js"]], function (t) { var e; let { addEvent: i, normalizeTickInterval: u, pick: g } = t; { function s() { "logarithmic" !== this.type ? this.logarithmic = void 0 : this.logarithmic ?? (this.logarithmic = new o(this)) } function r() { let e = this.logarithmic; e && (this.lin2val = function (t) { return e.lin2log(t) }, this.val2lin = function (t) { return e.log2lin(t) }) } (t = e = e || {}).compose = function (t) { return t.keepProps.includes("logarithmic") || (t.keepProps.push("logarithmic"), i(t, "afterSetType", s), i(t, "afterInit", r)), t }; class o { constructor(t) { this.axis = t } getLogTickPositions(n, h, l, d) { let t = this.axis, e = t.len, i = t.options, c = []; if (d || (this.minorAutoInterval = void 0), .5 <= n) n = Math.round(n), c = t.getLinearTickPositions(n, h, l); else if (.08 <= n) { let t, e, i, s, r, o, a; var p = Math.floor(h); for (t = .3 < n ? [1, 2, 4] : .15 < n ? [1, 2, 4, 6, 8] : [1, 2, 3, 4, 5, 6, 7, 8, 9], e = p; e < l + 1 && !a; e++)for (i = 0, s = t.length; i < s && !a; i++)(r = this.log2lin(this.lin2log(e) * t[i])) > h && (!d || o <= l) && void 0 !== o && c.push(o), o > l && (a = !0), o = r } else { var p = this.lin2log(h), s = this.lin2log(l), r = d ? t.getMinorTickInterval() : i.tickInterval, o = i.tickPixelInterval / (d ? 5 : 1), a = d ? e / t.tickPositions.length : e; n = u(n = g("auto" === r ? null : r, this.minorAutoInterval, (s - p) * o / (a || 1))), c = t.getLinearTickPositions(n, p, s).map(this.log2lin), d || (this.minorAutoInterval = n / 5) } return d || (t.tickInterval = n), c } lin2log(t) { return Math.pow(10, t) } log2lin(t) { return Math.log(t) / Math.LN10 } } t.Additions = o } return e }), e(t, "Core/Axis/PlotLineOrBand/PlotLineOrBandAxis.js", [t["Core/Utilities.js"]], function (t) { var e; let { erase: o, extend: s, isNumber: c } = t; { t = e = e || {}; let r; function a(t) { return this.addPlotBandOrLine(t, "plotBands") } function n(e, i) { let s = this.userOptions, t = new r(this, e); if (t = this.visible ? t.render() : t) { if (this._addedPlotLB || (this._addedPlotLB = !0, (s.plotLines || []).concat(s.plotBands || []).forEach(t => { this.addPlotBandOrLine(t) })), i) { let t = s[i] || []; t.push(e), s[i] = t } this.plotLinesAndBands.push(t) } return t } function h(t) { return this.addPlotBandOrLine(t, "plotLines") } function l(t, e, i) { i = i || this.options; let r = this.getPlotLinePath({ value: e, force: !0, acrossPanes: i.acrossPanes }), o = [], a = this.horiz, s = !c(this.min) || !c(this.max) || t < this.min && e < this.min || t > this.max && e > this.max, n = this.getPlotLinePath({ value: t, force: !0, acrossPanes: i.acrossPanes }), h, l = 1, d; if (n && r) for (s && (d = n.toString() === r.toString(), l = 0), h = 0; h < n.length; h += 2) { let t = n[h], e = n[h + 1], i = r[h], s = r[h + 1]; "M" !== t[0] && "L" !== t[0] || "M" !== e[0] && "L" !== e[0] || "M" !== i[0] && "L" !== i[0] || "M" !== s[0] && "L" !== s[0] || (a && i[1] === t[1] ? (i[1] += l, s[1] += l) : a || i[2] !== t[2] || (i[2] += l, s[2] += l), o.push(["M", t[1], t[2]], ["L", e[1], e[2]], ["L", s[1], s[2]], ["L", i[1], i[2]], ["Z"])), o.isFlat = d } return o } function d(t) { this.removePlotBandOrLine(t) } function p(i) { let t = this.plotLinesAndBands, s = this.options, r = this.userOptions; if (t) { let e = t.length; for (; e--;)t[e].id === i && t[e].destroy();[s.plotLines || [], r.plotLines || [], s.plotBands || [], r.plotBands || []].forEach(function (t) { for (e = t.length; e--;)(t[e] || {}).id === i && o(t, t[e]) }) } } function u(t) { this.removePlotBandOrLine(t) } t.compose = function (t, e) { var i = e.prototype; return i.addPlotBand || (r = t, s(i, { addPlotBand: a, addPlotLine: h, addPlotBandOrLine: n, getPlotBandPath: l, removePlotBand: d, removePlotLine: u, removePlotBandOrLine: p })), e } } return e }), e(t, "Core/Axis/PlotLineOrBand/PlotLineOrBand.js", [t["Core/Axis/PlotLineOrBand/PlotLineOrBandAxis.js"], t["Core/Utilities.js"]], function (i, t) { let { addEvent: s, arrayMax: d, arrayMin: c, defined: C, destroyObjectProperties: e, erase: r, fireEvent: M, merge: w, objectEach: A, pick: T } = t; return class o { static compose(t, e) { return s(t, "afterInit", function () { this.labelCollectors.push(() => { let t = []; for (var e of this.axes) for (var { label: i, options: s } of e.plotLinesAndBands) i && !s?.label?.allowOverlap && t.push(i); return t }) }), i.compose(o, e) } constructor(t, e) { this.axis = t, this.options = e, this.id = e.id } render() { M(this, "render"); let { axis: t, options: e } = this, { horiz: i, logarithmic: s } = t, { color: r, events: o, zIndex: a = 0 } = e, n = {}, h = t.chart.renderer, l = e.to, d = e.from, c = e.value, p = e.borderWidth, u = e.label, { label: g, svgElem: f } = this, m = [], x, y = C(d) && C(l), b = C(c), v = !f, S = { class: "highcharts-plot-" + (y ? "band " : "line ") + (e.className || "") }, k = y ? "bands" : "lines"; if (t.chart.styledMode || (b ? (S.stroke = r || "#999999", S["stroke-width"] = T(e.width, 1), e.dashStyle && (S.dashstyle = e.dashStyle)) : y && (S.fill = r || "#e6e9ff", p && (S.stroke = e.borderColor, S["stroke-width"] = p))), k += "-" + (n.zIndex = a), (x = t.plotLinesAndBandsGroups[k]) || (t.plotLinesAndBandsGroups[k] = x = h.g("plot-" + k).attr(n).add()), f || (this.svgElem = f = h.path().attr(S).add(x)), C(c)) m = t.getPlotLinePath({ value: s?.log2lin(c) ?? c, lineWidth: f.strokeWidth(), acrossPanes: e.acrossPanes }); else { if (!C(d) || !C(l)) return; m = t.getPlotBandPath(s?.log2lin(d) ?? d, s?.log2lin(l) ?? l, e) } return !this.eventsAdded && o && (A(o, (t, e) => { f?.on(e, t => { o[e].apply(this, [t]) }) }), this.eventsAdded = !0), !v && f.d || !m?.length ? f && (m ? (f.show(), f.animate({ d: m })) : f.d && (f.hide(), g && (this.label = g = g.destroy()))) : f.attr({ d: m }), u && (C(u.text) || C(u.formatter)) && m?.length && 0 < t.width && 0 < t.height && !m.isFlat ? (u = w({ align: i && y ? "center" : void 0, x: i ? !y && 4 : 10, verticalAlign: !i && y ? "middle" : void 0, y: i ? y ? 16 : 10 : y ? 6 : -4, rotation: i && !y ? 90 : 0, ...y ? { inside: !0 } : {} }, u), this.renderLabel(u, m, y, a)) : g && g.hide(), this } renderLabel(t, e, i, s) { let r = this.axis, o = r.chart.renderer, a = t.inside, n = this.label; n || (this.label = n = o.text(this.getLabelText(t), 0, 0, t.useHTML).attr({ align: t.textAlign || t.align, rotation: t.rotation, class: "highcharts-plot-" + (i ? "band" : "line") + "-label " + (t.className || ""), zIndex: s }), r.chart.styledMode || n.css(w({ fontSize: "0.8em", textOverflow: i && !a ? "" : "ellipsis" }, t.style)), n.add()); var s = e.xBounds || [e[0][1], e[1][1], (i ? e[2] : e[0])[1]], e = e.yBounds || [e[0][2], e[1][2], (i ? e[2] : e[0])[2]], h = c(s), l = c(e), s = d(s) - h; n.align(t, !1, { x: h, y: l, width: s, height: d(e) - l }), n.alignValue && "left" !== n.alignValue && !C(a) || n.css({ width: (t.style?.width || (i && a ? s : 90 === n.rotation ? r.height - (n.alignAttr.y - r.top) : (t.clip ? r.width : r.chart.chartWidth) - (n.alignAttr.x - r.left))) + "px" }), n.show(!0) } getLabelText(t) { return C(t.formatter) ? t.formatter.call(this) : t.text } destroy() { r(this.axis.plotLinesAndBands, this), delete this.axis, e(this) } } }), e(t, "Core/Tooltip.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Templating.js"], t["Core/Globals.js"], t["Core/Renderer/RendererUtilities.js"], t["Core/Renderer/RendererRegistry.js"], t["Core/Utilities.js"]], function (t, e, l, i, d, y) { var s; let h = t["animObject"], b = e["format"], { composed: r, doc: R, isSafari: z } = l, N = i["distribute"], { addEvent: o, clamp: W, css: x, discardElement: a, extend: G, fireEvent: v, isArray: S, isNumber: c, isString: H, merge: n, pick: X, pushUnique: p, splat: k, syncTimeout: u } = y; class g { constructor(t, e, i) { this.allowShared = !0, this.crosshairs = [], this.distance = 0, this.isHidden = !0, this.isSticky = !1, this.options = {}, this.outside = !1, this.chart = t, this.init(t, e), this.pointer = i } bodyFormatter(t) { return t.map(function (t) { let e = t.series.tooltipOptions; return (e[(t.point.formatPrefix || "point") + "Formatter"] || t.point.tooltipFormatter).call(t.point, e[(t.point.formatPrefix || "point") + "Format"] || "") }) } cleanSplit(i) { this.chart.series.forEach(function (t) { let e = t && t.tt; e && (!e.isActive || i ? t.tt = e.destroy() : e.isActive = !1) }) } defaultFormatter(t) { let e; var i = this.points || k(this); return (e = (e = [t.tooltipFooterHeaderFormatter(i[0])]).concat(t.bodyFormatter(i))).push(t.tooltipFooterHeaderFormatter(i[0], !0)), e } destroy() { this.label && (this.label = this.label.destroy()), this.split && (this.cleanSplit(!0), this.tt && (this.tt = this.tt.destroy())), this.renderer && (this.renderer = this.renderer.destroy(), a(this.container)), y.clearTimeout(this.hideTimer) } getAnchor(t, s) { let r, { chart: e, pointer: i } = this, o = e.inverted, a = e.plotTop, n = e.plotLeft; if ((t = k(t))[0].series && t[0].series.yAxis && !t[0].series.yAxis.options.reversedStacks && (t = t.slice().reverse()), this.followPointer && s) void 0 === s.chartX && (s = i.normalize(s)), r = [s.chartX - n, s.chartY - a]; else if (t[0].tooltipPos) r = t[0].tooltipPos; else { let e = 0, i = 0; t.forEach(function (t) { t = t.pos(!0); t && (e += t[0], i += t[1]) }), e /= t.length, i /= t.length, this.shared && 1 < t.length && s && (o ? e = s.chartX : i = s.chartY), r = [e - n, i - a] } return r.map(Math.round) } getClassName(t, e, i) { let s = this.options, r = t.series, o = r.options; return [s.className, "highcharts-label", i && "highcharts-tooltip-header", e ? "highcharts-tooltip-box" : "highcharts-tooltip", !i && "highcharts-color-" + X(t.colorIndex, r.colorIndex), o && o.className].filter(H).join(" ") } getLabel({ anchorX: t, anchorY: e } = { anchorX: 0, anchorY: 0 }) { let r = this, i = this.chart.styledMode, s = this.options, o = this.split && this.allowShared, a = this.container, n = this.chart.renderer; var h; if (this.label && (h = !this.label.hasClass("highcharts-label"), (!o && h || o && !h) && this.destroy()), !this.label) { if (this.outside) { let t = this.chart.options.chart.style, e = d.getRendererType(); this.container = a = l.doc.createElement("div"), a.className = "highcharts-tooltip-container", x(a, { position: "absolute", top: "1px", pointerEvents: "none", zIndex: Math.max(this.options.style.zIndex || 0, (t && t.zIndex || 0) + 3) }), this.renderer = n = new e(a, 0, 0, t, void 0, void 0, n.styledMode) } if (o ? this.label = n.g("tooltip") : (this.label = n.label("", t, e, s.shape, void 0, void 0, s.useHTML, void 0, "tooltip").attr({ padding: s.padding, r: s.borderRadius }), i || this.label.attr({ fill: s.backgroundColor, "stroke-width": s.borderWidth || 0 }).css(s.style).css({ pointerEvents: s.style.pointerEvents || (this.shouldStickOnContact() ? "auto" : "none") })), r.outside) { let s = this.label;[s.xSetter, s.ySetter].forEach((e, i) => { s[i ? "ySetter" : "xSetter"] = t => { e.call(s, r.distance), s[i ? "y" : "x"] = t, a && (a.style[i ? "top" : "left"] = t + "px") } }) } this.label.attr({ zIndex: 8 }).shadow(s.shadow).add() } return a && !a.parentElement && l.doc.body.appendChild(a), this.label } getPlayingField() { var { body: t, documentElement: e } = R, { chart: i, distance: s, outside: r } = this; return { width: r ? Math.max(t.scrollWidth, e.scrollWidth, t.offsetWidth, e.offsetWidth, e.clientWidth) - 2 * s : i.chartWidth, height: r ? Math.max(t.scrollHeight, e.scrollHeight, t.offsetHeight, e.offsetHeight, e.clientHeight) : i.chartHeight } } getPosition(i, s, t) { let { distance: p, chart: r, outside: u, pointer: e } = this, { inverted: o, plotLeft: a, plotTop: n, polar: h } = r, { plotX: l = 0, plotY: d = 0 } = t, g = {}, f = o && t.h || 0, { height: c, width: m } = this.getPlayingField(), x = e.getChartPosition(), y = t => t * x.scaleX, b = t => t * x.scaleY, v = t => { var e = "x" === t; return [t, e ? m : c, e ? i : s].concat(u ? [e ? y(i) : b(s), e ? x.left - p + y(l + a) : x.top - p + b(d + n), 0, e ? m : c] : [e ? i : s, e ? l + a : d + n, e ? a : n, e ? a + r.plotWidth : n + r.plotHeight]) }, S = v("y"), k = v("x"), C, M = !!t.negative, w = (!h && r.hoverSeries?.yAxis?.reversed && (M = !M), !this.followPointer && X(t.ttBelow, !h && !o === M)), A = function (t, e, i, s, r, o, a) { var n = u ? ("y" === t ? b : y)(p) : p, h = (i - s) / 2, l = s < r - p, d = r + p + s < e, c = r - n - i + h, r = r + n - h; if (w && d) g[t] = r; else if (!w && l) g[t] = c; else if (l) g[t] = Math.min(a - s, c - f < 0 ? c : c - f); else { if (!d) return !1; g[t] = Math.max(o, r + f + i > e ? r : r + f) } }, T = function (t, e, i, s, r) { if (r < p || r > e - p) return !1; g[t] = r < i / 2 ? 1 : e - s / 2 < r ? e - s - 2 : r - i / 2 }, P = function (t) { [S, k] = [k, S], C = t }, L = () => { !1 !== A.apply(0, S) ? !1 !== T.apply(0, k) || C || (P(!0), L()) : C ? g.x = g.y = 0 : (P(!0), L()) }; return (o && !h || 1 < this.len) && P(), L(), g } hide(e) { let i = this; y.clearTimeout(this.hideTimer), e = X(e, this.options.hideDelay), this.isHidden || (this.hideTimer = u(function () { let t = i.getLabel(); i.getLabel().animate({ opacity: 0 }, { duration: e && 150, complete: () => { t.hide(), i.container && i.container.remove() } }), i.isHidden = !0 }, e)) } init(t, e) { this.chart = t, this.options = e, this.crosshairs = [], this.isHidden = !0, this.split = e.split && !t.inverted && !t.polar, this.shared = e.shared || this.split, this.outside = X(e.outside, !(!t.scrollablePixelsX && !t.scrollablePixelsY)) } shouldStickOnContact(t) { return !(this.followPointer || !this.options.stickOnContact || t && !this.pointer.inClass(t.target, "highcharts-tooltip")) } move(t, e, i, s) { let r = this, o = h(!r.isHidden && r.options.animation), a = r.followPointer || 1 < (r.len || 0), n = { x: t, y: e }; a || (n.anchorX = i, n.anchorY = s), o.step = () => r.drawTracker(), r.getLabel().animate(n, o) } refresh(o, t) { let { chart: a, options: n, pointer: h, shared: e } = this, l = k(o), d = l[0], i = [], c = n.format, p = n.formatter || this.defaultFormatter, u = a.styledMode, g = {}, f = this.allowShared; if (n.enabled && d.series) { y.clearTimeout(this.hideTimer), this.allowShared = !(!S(o) && o.series && o.series.noSharedTooltip), f = f && !this.allowShared, this.followPointer = !this.split && d.series.tooltipOptions.followPointer; var o = this.getAnchor(o, t), m = o[0], x = o[1]; e && this.allowShared ? (h.applyInactiveState(l), l.forEach(function (t) { t.setState("hover"), i.push(t.getLabelConfig()) }), (g = d.getLabelConfig()).points = i) : g = d.getLabelConfig(), this.len = i.length; let s = H(c) ? b(c, g, a) : p.call(g, this), r = d.series; if (this.distance = X(r.tooltipOptions.distance, 16), !1 === s) this.hide(); else { if (this.split && this.allowShared) this.renderSplit(s, l); else { let e = m, i = x; if (t && h.isDirectTouch && (e = t.chartX - a.plotLeft, i = t.chartY - a.plotTop), !a.polar && !1 !== r.options.clip && !l.some(t => h.isDirectTouch || t.series.shouldShowTooltip(e, i))) return void this.hide(); { let t = this.getLabel(f && this.tt || {}); n.style.width && !u || t.css({ width: (this.outside ? this.getPlayingField() : a.spacingBox).width + "px" }), t.attr({ class: this.getClassName(d), text: s && s.join ? s.join("") : s }), this.outside && t.attr({ x: W(t.x || 0, 0, this.getPlayingField().width - (t.width || 0)) }), u || t.attr({ stroke: n.borderColor || d.color || r.color || "#666666" }), this.updatePosition({ plotX: m, plotY: x, negative: d.negative, ttBelow: d.ttBelow, h: o[2] || 0 }) } } this.isHidden && this.label && this.label.attr({ opacity: 1 }).show(), this.isHidden = !1 } v(this, "refresh") } } renderSplit(t, c) { let p = this, { chart: e, chart: { chartWidth: i, chartHeight: s, plotHeight: u, plotLeft: g, plotTop: f, scrollablePixelsY: r = 0, scrollablePixelsX: o, styledMode: m }, distance: x, options: y, options: { positioner: b }, pointer: a } = p, { scrollLeft: n = 0, scrollTop: h = 0 } = e.scrollablePlotArea?.scrollingContainer || {}, v = p.outside && "number" != typeof o ? R.documentElement.getBoundingClientRect() : { left: n, right: n + i, top: h, bottom: h + s }, S = p.getLabel(), k = this.renderer || e.renderer, C = !(!e.xAxis[0] || !e.xAxis[0].opposite), { left: l, top: d } = a.getChartPosition(), M = f + h, w, A = u - r; function T(t, e, i, s, r = !0) { let o, a; return { x: a = i ? (o = C ? 0 : A, W(t - s / 2, v.left, v.right - s - (p.outside ? l : 0))) : (o = e - M, W(a = r ? t - s - x : t + x, r ? a : v.left, v.right)), y: o } } let P = (t = H(t) ? [!1, t] : t).slice(0, c.length + 1).reduce(function (o, a, n) { if (!1 !== a && "" !== a) { let h = c[n - 1] || { isHeader: !0, plotX: c[0].plotX, plotY: u, series: {} }, t = h.isHeader, e = t ? p : h.series, i = e.tt = function (t, e, i) { let s = t, { isHeader: r, series: o } = e; if (!s) { let t = { padding: y.padding, r: y.borderRadius }; m || (t.fill = y.backgroundColor, t["stroke-width"] = y.borderWidth ?? 1), s = k.label("", 0, 0, y[r ? "headerShape" : "shape"], void 0, void 0, y.useHTML).addClass(p.getClassName(e, !0, r)).attr(t).add(S) } return s.isActive = !0, s.attr({ text: i }), m || s.css(y.style).attr({ stroke: y.borderColor || e.color || o.color || "#333333" }), s }(e.tt, h, a.toString()), s = i.getBBox(), r = s.width + i.strokeWidth(); t && (w = s.height, A += w, C && (M -= w)); var l, d, { anchorX: n, anchorY: a } = function () { let t, e, { isHeader: i, plotX: s = 0, plotY: r = 0, series: o } = h; var a, n; return i ? (t = Math.max(g + s, g), e = f + u / 2) : ({ xAxis: a, yAxis: n } = o, t = a.pos + W(s, -x, a.len + x), o.shouldShowTooltip(0, n.pos - f + r, { ignoreX: !0 }) && (e = n.pos + r)), { anchorX: t = W(t, v.left - x, v.right + x), anchorY: e } }(); "number" == typeof a ? (l = s.height + 1, d = b ? b.call(p, r, l, h) : T(n, a, t, r), o.push({ align: b ? 0 : void 0, anchorX: n, anchorY: a, boxWidth: r, point: h, rank: X(d.rank, t ? 1 : 0), size: l, target: d.y, tt: i, x: d.x })) : i.isActive = !1 } return o }, []), L = (!b && P.some(t => { var e = p["outside"], e = (e ? l : 0) + t.anchorX; return e < v.left && e + t.boxWidth < v.right || e < l - v.left + t.boxWidth && v.right - e > e }) && (P = P.map(t => { var { x: e, y: i } = T(t.anchorX, t.anchorY, t.point.isHeader, t.boxWidth, !1); return G(t, { target: i, x: e }) })), p.cleanSplit(), N(P, A), { left: l, right: l }), { container: O, outside: E, renderer: D } = (P.forEach(function (t) { var { x: t, boxWidth: e, isHeader: i } = t; i || (p.outside && l + t < L.left && (L.left = l + t), !i && p.outside && L.left + e > L.right && (L.right = l + t)) }), P.forEach(function (t) { let { x: e, anchorX: i, anchorY: s, pos: r, point: { isHeader: o } } = t, a = { visibility: void 0 === r ? "hidden" : "inherit", x: e, y: (r || 0) + M, anchorX: i, anchorY: s }; var n; p.outside && e < i && (0 < (n = l - L.left) && (o || (a.x = e + n, a.anchorX = i + n), o && (a.x = (L.right - L.left) / 2, a.anchorX = i + n))), t.tt.attr(a) }), p); var j, I, B; E && O && D && ({ width: t, height: j, x: I, y: B } = S.getBBox(), D.setSize(t + I, j + B, !1), O.style.left = L.left + "px", O.style.top = d + "px"), z && S.attr({ opacity: 1 === S.opacity ? .999 : 1 }) } drawTracker() { if (this.shouldStickOnContact()) { let s = this.chart, r = this.label, o = this.shared ? s.hoverPoints : s.hoverPoint; if (r && o) { let t = { x: 0, y: 0, width: 0, height: 0 }, e = this.getAnchor(o), i = r.getBBox(); e[0] += s.plotLeft - (r.translateX || 0), e[1] += s.plotTop - (r.translateY || 0), t.x = Math.min(0, e[0]), t.y = Math.min(0, e[1]), t.width = e[0] < 0 ? Math.max(Math.abs(e[0]), i.width - e[0]) : Math.max(Math.abs(e[0]), i.width), t.height = e[1] < 0 ? Math.max(Math.abs(e[1]), i.height - Math.abs(e[1])) : Math.max(Math.abs(e[1]), i.height), this.tracker ? this.tracker.attr(t) : (this.tracker = r.renderer.rect(t).addClass("highcharts-tracker").add(r), s.styledMode || this.tracker.attr({ fill: "rgba(0,0,0,0)" })) } } else this.tracker && (this.tracker = this.tracker.destroy()) } styledModeFormat(t) { return t.replace('style="font-size: 0.8em"', 'class="highcharts-header"').replace(/style="color:{(point|series)\.color}"/g, 'class="highcharts-color-{$1.colorIndex} {series.options.className} {point.options.className}"') } tooltipFooterHeaderFormatter(e, t) { let i = e.series, s = i.tooltipOptions, r = i.xAxis, o = r && r.dateTime, a = { isFooter: t, labelConfig: e }, n = s.xDateFormat, h = s[t ? "footerFormat" : "headerFormat"]; return v(this, "headerFormatter", a, function (t) { o && !n && c(e.key) && (n = o.getXDateFormat(e.key, s.dateTimeLabelFormats)), o && n && (e.point && e.point.tooltipDateKeys || ["key"]).forEach(function (t) { h = h.replace("{point." + t + "}", "{point." + t + ":" + n + "}") }), i.chart.styledMode && (h = this.styledModeFormat(h)), t.text = b(h, { point: e, series: i }, this.chart) }), a.text } update(t) { this.destroy(), this.init(this.chart, n(!0, this.options, t)) } updatePosition(t) { let { chart: e, container: i, distance: s, options: r, pointer: o, renderer: a } = this, { height: n = 0, width: h = 0 } = this.getLabel(), { left: l, top: d, scaleX: c, scaleY: p } = o.getChartPosition(), u = (r.positioner || this.getPosition).call(this, h, n, t), g = (t.plotX || 0) + e.plotLeft, f = (t.plotY || 0) + e.plotTop, m; a && i && (r.positioner && (u.x += l - s, u.y += d - s), m = (r.borderWidth || 0) + 2 * s + 2, a.setSize(h + m, n + m, !1), 1 === c && 1 === p || (x(i, { transform: `scale(${c}, ${p})` }), g *= c, f *= p), g += l - u.x, f += d - u.y), this.move(Math.round(u.x), Math.round(u.y || 0), g, f) } } return (s = g = g || {}).compose = function (t) { p(r, "Core.Tooltip") && o(t, "afterInit", function () { let t = this.chart; t.options.tooltip && (t.tooltip = new s(t, t.options.tooltip, this)) }) }, g }), e(t, "Core/Series/Point.js", [t["Core/Renderer/HTML/AST.js"], t["Core/Animation/AnimationUtilities.js"], t["Core/Defaults.js"], t["Core/Templating.js"], t["Core/Utilities.js"]], function (S, t, e, i, s) { let n = t["animObject"], k = e["defaultOptions"], a = i["format"], { addEvent: r, crisp: o, erase: h, extend: C, fireEvent: M, getNestedProperty: l, isArray: d, isFunction: c, isNumber: w, isObject: p, merge: u, pick: A, syncTimeout: g, removeEvent: f, uniqueKey: m } = s; class x { animateBeforeDestroy() { let e = this, i = { x: e.startXPos, opacity: 0 }, t = e.getGraphicalProps(); t.singular.forEach(function (t) { e[t] = e[t].animate("dataLabel" === t ? { x: e[t].startXPos, y: e[t].startYPos, opacity: 0 } : i) }), t.plural.forEach(function (t) { e[t].forEach(function (t) { t.element && t.animate(C({ x: e.startXPos }, t.startYPos ? { x: t.startXPos, y: t.startYPos } : {})) }) }) } applyOptions(t, e) { let i = this.series, s = i.options.pointValKey || i.pointValKey; return C(this, t = x.prototype.optionsToObject.call(this, t)), this.options = this.options ? C(this.options, t) : t, t.group && delete this.group, t.dataLabels && delete this.dataLabels, s && (this.y = x.prototype.getNestedProperty.call(this, s)), this.selected && (this.state = "select"), "name" in this && void 0 === e && i.xAxis && i.xAxis.hasNames && (this.x = i.xAxis.nameToX(this)), void 0 === this.x && i ? this.x = e ?? i.autoIncrement() : w(t.x) && i.options.relativeXValue && (this.x = i.autoIncrement(t.x)), this.isNull = this.isValid && !this.isValid(), this.formatPrefix = this.isNull ? "null" : "point", this } destroy() { if (!this.destroyed) { let e = this, t = e.series, i = t.chart, s = t.options.dataSorting, r = i.hoverPoints, o = n(e.series.chart.renderer.globalAnimation), a = () => { for (var t in (e.graphic || e.graphics || e.dataLabel || e.dataLabels) && (f(e), e.destroyElements()), e) delete e[t] }; e.legendItem && i.legend.destroyItem(e), r && (e.setState(), h(r, e), r.length || (i.hoverPoints = null)), e === i.hoverPoint && e.onMouseOut(), s && s.enabled ? (this.animateBeforeDestroy(), g(a, o.duration)) : a(), i.pointCount-- } this.destroyed = !0 } destroyElements(t) { let e = this, i = e.getGraphicalProps(t); i.singular.forEach(function (t) { e[t] = e[t].destroy() }), i.plural.forEach(function (t) { e[t].forEach(function (t) { t && t.element && t.destroy() }), delete e[t] }) } firePointEvent(t, e, i) { let s = this, r = this.series.options; s.manageEvent(t), "click" === t && r.allowPointSelect && (i = function (t) { !s.destroyed && s.select && s.select(null, t.ctrlKey || t.metaKey || t.shiftKey) }), M(s, t, e, i) } getClassName() { return "highcharts-point" + (this.selected ? " highcharts-point-select" : "") + (this.negative ? " highcharts-negative" : "") + (this.isNull ? " highcharts-null-point" : "") + (void 0 !== this.colorIndex ? " highcharts-color-" + this.colorIndex : "") + (this.options.className ? " " + this.options.className : "") + (this.zone && this.zone.className ? " " + this.zone.className.replace("highcharts-negative", "") : "") } getGraphicalProps(i) { let t, e, s = this, r = [], o = { singular: [], plural: [] }; for ((i = i || { graphic: 1, dataLabel: 1 }).graphic && r.push("graphic", "connector"), i.dataLabel && r.push("dataLabel", "dataLabelPath", "dataLabelUpper"), e = r.length; e--;)s[t = r[e]] && o.singular.push(t); return ["graphic", "dataLabel"].forEach(function (t) { var e = t + "s"; i[t] && s[e] && o.plural.push(e) }), o } getLabelConfig() { return { x: this.category, y: this.y, color: this.color, colorIndex: this.colorIndex, key: this.name || this.category, series: this.series, point: this, percentage: this.percentage, total: this.total || this.stackTotal } } getNestedProperty(t) { return t ? 0 === t.indexOf("custom.") ? l(t, this.options) : this[t] : void 0 } getZone() { let t = this.series, e = t.zones, i = t.zoneAxis || "y", s, r = 0; for (s = e[0]; this[i] >= s.value;)s = e[++r]; return this.nonZonedColor || (this.nonZonedColor = this.color), s && s.color && !this.options.color ? this.color = s.color : this.color = this.nonZonedColor, s } hasNewShapeType() { return (this.graphic && (this.graphic.symbolName || this.graphic.element.nodeName)) !== this.shapeType } constructor(t, e, i) { this.formatPrefix = "point", this.visible = !0, this.series = t, this.applyOptions(e, i), this.id ?? (this.id = m()), this.resolveColor(), t.chart.pointCount++, M(this, "afterInit") } isValid() { return (w(this.x) || this.x instanceof Date) && w(this.y) } optionsToObject(t) { let e = this.series, i = e.options.keys, s = i || e.pointArrayMap || ["y"], r = s.length, o = {}, a, n = 0, h = 0; if (w(t) || null === t) o[s[0]] = t; else if (d(t)) for (!i && t.length > r && ("string" == (a = typeof t[0]) ? o.name = t[0] : "number" == a && (o.x = t[0]), n++); h < r;)i && void 0 === t[n] || (0 < s[h].indexOf(".") ? x.prototype.setNestedProperty(o, t[n], s[h]) : o[s[h]] = t[n]), n++, h++; else "object" == typeof t && ((o = t).dataLabels && (e.hasDataLabels = () => !0), t.marker && (e._hasPointMarkers = !0)); return o } pos(n, h = this.plotY) { if (!this.destroyed) { let { plotX: t, series: e } = this, { chart: i, xAxis: s, yAxis: r } = e, o = 0, a = 0; if (w(t) && w(h)) return n && (o = s ? s.pos : i.plotLeft, a = r ? r.pos : i.plotTop), i.inverted && s && r ? [r.len - h + a, s.len - t + o] : [t + o, h + a] } } resolveColor() { let t = this.series, e = t.chart.options.chart, i = t.chart.styledMode, s, r, o = e.colorCount, a; delete this.nonZonedColor, t.options.colorByPoint ? (i || (s = (r = t.options.colors || t.chart.options.colors)[t.colorCounter], o = r.length), a = t.colorCounter, t.colorCounter++, t.colorCounter === o && (t.colorCounter = 0)) : (i || (s = t.color), a = t.colorIndex), this.colorIndex = A(this.options.colorIndex, a), this.color = A(this.options.color, s) } setNestedProperty(t, r, e) { return e.split(".").reduce(function (t, e, i, s) { s = s.length - 1 === i; return t[e] = s ? r : p(t[e], !0) ? t[e] : {}, t[e] }, t), t } shouldDraw() { return !this.isNull } tooltipFormatter(e) { let t = this.series, i = t.tooltipOptions, s = A(i.valueDecimals, ""), r = i.valuePrefix || "", o = i.valueSuffix || ""; return t.chart.styledMode && (e = t.chart.tooltip.styledModeFormat(e)), (t.pointArrayMap || ["y"]).forEach(function (t) { t = "{point." + t, e = (e = r || o ? e.replace(RegExp(t + "}", "g"), r + t + "}" + o) : e).replace(RegExp(t + "}", "g"), t + ":,." + s + "f}") }), a(e, { point: this, series: this.series }, t.chart) } update(e, i, s, t) { let r, o = this, a = o.series, n = o.graphic, h = a.chart, l = a.options; function d() { o.applyOptions(e); var t = n && o.hasMockGraphic, t = null === o.y ? !t : t; n && t && (o.graphic = n.destroy(), delete o.hasMockGraphic), p(e, !0) && (n && n.element && e && e.marker && void 0 !== e.marker.symbol && (o.graphic = n.destroy()), e?.dataLabels && o.dataLabel && (o.dataLabel = o.dataLabel.destroy())), r = o.index, a.updateParallelArrays(o, r), l.data[r] = p(l.data[r], !0) || p(e, !0) ? o.options : A(e, l.data[r]), a.isDirty = a.isDirtyData = !0, !a.fixedBox && a.hasCartesianSeries && (h.isDirtyBox = !0), "point" === l.legendType && (h.isDirtyLegend = !0), i && h.redraw(s) } i = A(i, !0), !1 === t ? d() : o.firePointEvent("update", { options: e }, d) } remove(t, e) { this.series.removePoint(this.series.data.indexOf(this), t, e) } select(t, e) { let i = this, s = i.series, r = s.chart; t = A(t, !i.selected), this.selectedStaging = t, i.firePointEvent(t ? "select" : "unselect", { accumulate: e }, function () { i.selected = i.options.selected = t, s.options.data[s.data.indexOf(i)] = i.options, i.setState(t && "select"), e || r.getSelectedPoints().forEach(function (t) { let e = t.series; t.selected && t !== i && (t.selected = t.options.selected = !1, e.options.data[e.data.indexOf(t)] = t.options, t.setState(r.hoverPoints && e.options.inactiveOtherPoints ? "inactive" : ""), t.firePointEvent("unselect")) }) }), delete this.selectedStaging } onMouseOver(t) { let { inverted: e, pointer: i } = this.series.chart; i && (t = t ? i.normalize(t) : i.getChartCoordinatesFromPoint(this, e), i.runPointActions(t, this)) } onMouseOut() { let t = this.series.chart; this.firePointEvent("mouseOut"), this.series.options.inactiveOtherPoints || (t.hoverPoints || []).forEach(function (t) { t.setState() }), t.hoverPoints = t.hoverPoint = null } manageEvent(t) { var e = u(this.series.options.point, this.options).events?.[t]; !c(e) || this.hcEvents?.[t] && -1 !== this.hcEvents?.[t]?.map(t => t.fn).indexOf(e) ? this.importedUserEvent && !e && this.hcEvents?.[t] && (f(this, t), delete this.hcEvents[t], Object.keys(this.hcEvents) || delete this.importedUserEvent) : (this.importedUserEvent?.(), this.importedUserEvent = r(this, t, e)) } setState(t, e) { let i = this.series, s = this.state, r = i.options.states[t || "normal"] || {}, o = k.plotOptions[i.type].marker && i.options.marker, a = o && !1 === o.enabled, n = o && o.states && o.states[t || "normal"] || {}, h = !1 === n.enabled, l = this.marker || {}, d = i.chart, c = o && i.markerAttribs, p = i.halo, u, g, f, m = i.stateMarkerGraphic, x; if (!((t = t || "") === this.state && !e || this.selected && "select" !== t || !1 === r.enabled || t && (h || a && !1 === n.enabled) || t && l.states && l.states[t] && !1 === l.states[t].enabled)) { if (this.state = t, c && (u = i.markerAttribs(this, t)), this.graphic && !this.hasMockGraphic) { if (s && this.graphic.removeClass("highcharts-point-" + s), t && this.graphic.addClass("highcharts-point-" + t), !d.styledMode) { g = i.pointAttribs(this, t), f = A(d.options.chart.animation, r.animation); let e = g.opacity; i.options.inactiveOtherPoints && w(e) && (this.dataLabels || []).forEach(function (t) { t && !t.hasClass("highcharts-data-label-hidden") && (t.animate({ opacity: e }, f), t.connector && t.connector.animate({ opacity: e }, f)) }), this.graphic.animate(g, f) } u && this.graphic.animate(u, A(d.options.chart.animation, n.animation, o.animation)), m && m.hide() } else t && n && (x = l.symbol || i.symbol, m && m.currentSymbol !== x && (m = m.destroy()), u && (m ? m[e ? "animate" : "attr"]({ x: u.x, y: u.y }) : x && (i.stateMarkerGraphic = m = d.renderer.symbol(x, u.x, u.y, u.width, u.height).add(i.markerGroup), m.currentSymbol = x)), !d.styledMode && m && "inactive" !== this.state && m.attr(i.pointAttribs(this, t))), m && (m[t && this.isInside ? "show" : "hide"](), m.element.point = this, m.addClass(this.getClassName(), !0)); var y = r.halo, b = this.graphic || m, v = b && b.visibility || "inherit"; y && y.size && b && "hidden" !== v && !this.isCluster ? (p || (i.halo = p = d.renderer.path().add(b.parentGroup)), p.show()[e ? "animate" : "attr"]({ d: this.haloPath(y.size) }), p.attr({ class: "highcharts-halo highcharts-color-" + A(this.colorIndex, i.colorIndex) + (this.className ? " " + this.className : ""), visibility: v, zIndex: -1 }), p.point = this, d.styledMode || p.attr(C({ fill: this.color || i.color, "fill-opacity": y.opacity }, S.filterUserAttributes(y.attributes || {})))) : p?.point?.haloPath && !p.point.destroyed && p.animate({ d: p.point.haloPath(0) }, null, p.hide), M(this, "afterSetState", { state: t }) } } haloPath(t) { var e = this.pos(); return e ? this.series.chart.renderer.symbols.circle(o(e[0], 1) - t, e[1] - t, 2 * t, 2 * t) : [] } } return x }), e(t, "Core/Pointer.js", [t["Core/Color/Color.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, r, e) { var i; let b = t["parse"], { charts: g, composed: s, isTouchDevice: o } = r, { addEvent: f, attr: a, css: n, extend: h, find: m, fireEvent: p, isNumber: l, isObject: v, objectEach: d, offset: c, pick: x, pushUnique: u, splat: y } = e; class S { applyInactiveState(t) { let e = [], i; (t || []).forEach(function (t) { i = t.series, e.push(i), i.linkedParent && e.push(i.linkedParent), i.linkedSeries && (e = e.concat(i.linkedSeries)), i.navigatorSeries && e.push(i.navigatorSeries) }), this.chart.series.forEach(function (t) { -1 === e.indexOf(t) ? t.setState("inactive", !0) : t.options.inactiveOtherPoints && t.setAllPointsToState("inactive") }) } destroy() { let i = this; this.eventsToUnbind.forEach(t => t()), this.eventsToUnbind = [], r.chartCount || (S.unbindDocumentMouseUp && S.unbindDocumentMouseUp.forEach(t => t()), S.unbindDocumentTouchEnd && (S.unbindDocumentTouchEnd = S.unbindDocumentTouchEnd())), clearInterval(i.tooltipTimeout), d(i, function (t, e) { i[e] = void 0 }) } getSelectionMarkerAttrs(h, l) { var t = { args: { chartX: h, chartY: l }, attrs: {}, shapeType: "rect" }; return p(this, "getSelectionMarkerAttrs", t, t => { let e, { chart: i, zoomHor: s, zoomVert: r } = this, { mouseDownX: o = 0, mouseDownY: a = 0 } = i, n = t.attrs; n.x = i.plotLeft, n.y = i.plotTop, n.width = s ? 1 : i.plotWidth, n.height = r ? 1 : i.plotHeight, s && (e = h - o, n.width = Math.max(1, Math.abs(e)), n.x = (0 < e ? 0 : e) + o), r && (e = l - a, n.height = Math.max(1, Math.abs(e)), n.y = (0 < e ? 0 : e) + a) }), t } drag(t) { let e = this["chart"], { mouseDownX: i = 0, mouseDownY: s = 0 } = e, { panning: r, panKey: o, selectionMarkerFill: a } = e.options.chart, n = e.plotLeft, h = e.plotTop, l = e.plotWidth, d = e.plotHeight, c = v(r) ? r.enabled : r, p = o && t[o + "Key"], u = t.chartX, g = t.chartY, f, m = this.selectionMarker; var x, y; m && m.touch || (u < n ? u = n : u > n + l && (u = n + l), g < h ? g = h : g > h + d && (g = h + d), this.hasDragged = Math.sqrt(Math.pow(i - u, 2) + Math.pow(s - g, 2)), !(10 < this.hasDragged)) || (f = e.isInsidePlot(i - n, s - h, { visiblePlotOnly: !0 }), { shapeType: x, attrs: y } = this.getSelectionMarkerAttrs(u, g), (e.hasCartesianSeries || e.mapView) && this.hasZoom && f && !p && !m && (this.selectionMarker = m = e.renderer[x](), m.attr({ class: "highcharts-selection-marker", zIndex: 7 }).add(), e.styledMode || m.attr({ fill: a || b("#334eff").setOpacity(.25).get() })), m && m.attr(y), f && !m && c && e.pan(t, r)) } dragStart(t) { let e = this.chart; e.mouseIsDown = t.type, e.cancelClick = !1, e.mouseDownX = t.chartX, e.mouseDownY = t.chartY } getSelectionBox(t) { t = { args: { marker: t }, result: t.getBBox() }; return p(this, "getSelectionBox", t), t.result } drop(t) { let e, { chart: i, selectionMarker: s } = this; for (var r of i.axes) r.isPanning && (r.isPanning = !1, (r.options.startOnTick || r.options.endOnTick || r.series.some(t => t.boosted)) && (r.forceRedraw = !0, r.setExtremes(r.userMin, r.userMax, !1), e = !0)); var o; e && i.redraw(), s && t && (this.hasDragged && (o = this.getSelectionBox(s), i.transform({ axes: i.axes.filter(t => t.zoomEnabled && ("xAxis" === t.coll && this.zoomX || "yAxis" === t.coll && this.zoomY)), selection: { originalEvent: t, xAxis: [], yAxis: [], ...o }, from: o })), l(i.index) && (this.selectionMarker = s.destroy())), i && l(i.index) && (n(i.container, { cursor: i._cursor }), i.cancelClick = 10 < this.hasDragged, i.mouseIsDown = !1, this.hasDragged = 0, this.pinchDown = []) } findNearestKDPoint(t, o, a) { let n; return t.forEach(function (t) { var e, i, s, r = !(t.noSharedTooltip && o) && t.options.findNearestPointBy.indexOf("y") < 0, t = t.searchPoint(a, r); v(t, !0) && t.series && (!v(n, !0) || (r = n, e = r.distX - t.distX, i = r.dist - t.dist, s = t.series.group?.zIndex - r.series.group?.zIndex, 0 < (0 != e && o ? e : 0 != i ? i : 0 != s ? s : r.series.index > t.series.index ? -1 : 1))) && (n = t) }), n } getChartCoordinatesFromPoint(i, s) { var { xAxis: r, yAxis: o } = i.series, a = i.shapeArgs; if (r && o) { let t = i.clientX ?? i.plotX ?? 0, e = i.plotY || 0; return i.isNode && a && l(a.x) && l(a.y) && (t = a.x, e = a.y), s ? { chartX: o.len + o.pos - e, chartY: r.len + r.pos - t } : { chartX: t + r.pos, chartY: e + o.pos } } if (a && a.x && a.y) return { chartX: a.x, chartY: a.y } } getChartPosition() { if (this.chartPosition) return this.chartPosition; var t = this.chart["container"], e = c(t), { offsetHeight: t, offsetWidth: i } = (this.chartPosition = { left: e.left, top: e.top, scaleX: 1, scaleY: 1 }, t); return 2 < i && 2 < t && (this.chartPosition.scaleX = e.width / i, this.chartPosition.scaleY = e.height / t), this.chartPosition } getCoordinates(t) { let e = { xAxis: [], yAxis: [] }; for (var i of this.chart.axes) e[i.isXAxis ? "xAxis" : "yAxis"].push({ axis: i, value: i.toValue(t[i.horiz ? "chartX" : "chartY"]) }); return e } getHoverData(t, e, i, s, r, o) { let a = [], n = function (t) { return t.visible && !(!r && t.directTouch) && x(t.options.enableMouseTracking, !0) }, h = e, l, d = { chartX: o ? o.chartX : void 0, chartY: o ? o.chartY : void 0, shared: r }, c = (p(this, "beforeGetHoverData", d), l = h && !h.stickyTracking ? [h] : i.filter(t => t.stickyTracking && (d.filter || n)(t)), s && t || !o ? t : this.findNearestKDPoint(l, r, o)); return h = c && c.series, c && (r && !h.noSharedTooltip ? (l = i.filter(function (t) { return d.filter ? d.filter(t) : n(t) && !t.noSharedTooltip })).forEach(function (t) { let e = m(t.points, function (t) { return t.x === c.x && !t.isNull }); v(e) && (t.boosted && t.boost && (e = t.boost.getPoint(e)), a.push(e)) }) : a.push(c)), p(this, "afterGetHoverData", d = { hoverPoint: c }), { hoverPoint: d.hoverPoint, hoverSeries: h, hoverPoints: a } } getPointFromEvent(t) { let e = t.target, i; for (; e && !i;)i = e.point, e = e.parentNode; return i } onTrackerMouseOut(t) { let e = this.chart, i = t.relatedTarget, s = e.hoverSeries; this.isDirectTouch = !1, !s || !i || s.stickyTracking || this.inClass(i, "highcharts-tooltip") || this.inClass(i, "highcharts-series-" + s.index) && this.inClass(i, "highcharts-tracker") || s.onMouseOut() } inClass(t, e) { let i = t, s; for (; i;) { if (s = a(i, "class")) { if (-1 !== s.indexOf(e)) return !0; if (-1 !== s.indexOf("highcharts-container")) return !1 } i = i.parentElement } } constructor(t, e) { this.hasDragged = 0, this.pointerCaptureEventsToUnbind = [], this.eventsToUnbind = [], this.options = e, this.chart = t, this.runChartClick = !!e.chart.events?.click, this.pinchDown = [], this.setDOMEvents(), p(this, "afterInit") } normalize(t, e) { let i = t.touches, s = i ? i.length ? i.item(0) : x(i.changedTouches, t.changedTouches)[0] : t; e = e || this.getChartPosition(); var r = s.pageX - e.left, o = s.pageY - e.top; return h(t, { chartX: Math.round(r /= e.scaleX), chartY: Math.round(o /= e.scaleY) }) } onContainerClick(t) { let e = this.chart, i = e.hoverPoint, s = this.normalize(t), r = e.plotLeft, o = e.plotTop; e.cancelClick || (i && this.inClass(s.target, "highcharts-tracker") ? (p(i.series, "click", h(s, { point: i })), e.hoverPoint && i.firePointEvent("click", s)) : (h(s, this.getCoordinates(s)), e.isInsidePlot(s.chartX - r, s.chartY - o, { visiblePlotOnly: !0 }) && p(e, "click", s))) } onContainerMouseDown(t) { var e = 1 == (1 & (t.buttons || t.button)); t = this.normalize(t), r.isFirefox && 0 !== t.button && this.onContainerMouseMove(t), void 0 !== t.button && !e || (this.zoomOption(t), e && t.preventDefault?.(), this.dragStart(t)) } onContainerMouseLeave(t) { let e = (g[x(S.hoverChartIndex, -1)] || {})["pointer"]; t = this.normalize(t), this.onContainerMouseMove(t), e && !this.inClass(t.relatedTarget, "highcharts-tooltip") && (e.reset(), e.chartPosition = void 0) } onContainerMouseEnter() { delete this.chartPosition } onContainerMouseMove(t) { let e = this.chart, i = e.tooltip, s = this.normalize(t); this.setHoverChartIndex(t), "mousedown" !== e.mouseIsDown && !this.touchSelect(s) || this.drag(s), e.openMenu || !this.inClass(s.target, "highcharts-tracker") && !e.isInsidePlot(s.chartX - e.plotLeft, s.chartY - e.plotTop, { visiblePlotOnly: !0 }) || i && i.shouldStickOnContact(s) || (this.inClass(s.target, "highcharts-no-tooltip") ? this.reset(!1, 0) : this.runPointActions(s)) } onDocumentTouchEnd(t) { this.onDocumentMouseUp(t) } onContainerTouchMove(t) { this.touchSelect(t) ? this.onContainerMouseMove(t) : this.touch(t) } onContainerTouchStart(t) { this.touchSelect(t) ? this.onContainerMouseDown(t) : (this.zoomOption(t), this.touch(t, !0)) } onDocumentMouseMove(t) { let e = this.chart, i = e.tooltip, s = this.chartPosition, r = this.normalize(t, s); !s || e.isInsidePlot(r.chartX - e.plotLeft, r.chartY - e.plotTop, { visiblePlotOnly: !0 }) || i && i.shouldStickOnContact(r) || r.target !== e.container.ownerDocument && this.inClass(r.target, "highcharts-tracker") || this.reset() } onDocumentMouseUp(t) { g[x(S.hoverChartIndex, -1)]?.pointer?.drop(t) } pinch(e) { let i = this, { chart: s, hasZoom: t, lastTouches: r } = i, o = [].map.call(e.touches || [], t => i.normalize(t)), a = o.length, n = 1 === a && (i.inClass(e.target, "highcharts-tracker") && s.runTrackerClick || i.runChartClick), h = s.tooltip, l = 1 === a && x(h?.options.followTouchMove, !0); 1 < a ? i.initiated = !0 : l && (i.initiated = !1), t && i.initiated && !n && !1 !== e.cancelable && e.preventDefault(), "touchstart" === e.type ? (i.pinchDown = o, i.res = !0, s.mouseDownX = e.chartX) : l ? this.runPointActions(i.normalize(e)) : r && (p(s, "touchpan", { originalEvent: e, touches: o }, () => { var t = t => { var e = t[0], t = t[1] || e; return { x: e.chartX, y: e.chartY, width: t.chartX - e.chartX, height: t.chartY - e.chartY } }; s.transform({ axes: s.axes.filter(t => t.zoomEnabled && (this.zoomHor && t.horiz || this.zoomVert && !t.horiz)), to: t(o), from: t(r), trigger: e.type }) }), i.res && (i.res = !1, this.reset(!1, 0))), i.lastTouches = o } reset(e, t) { let i = this.chart, s = i.hoverSeries, r = i.hoverPoint, o = i.hoverPoints, a = i.tooltip, n = a && a.shared ? o : r; e && n && y(n).forEach(function (t) { t.series.isCartesian && void 0 === t.plotX && (e = !1) }), e ? a && n && y(n).length && (a.refresh(n), a.shared && o ? o.forEach(function (t) { t.setState(t.state, !0), t.series.isCartesian && (t.series.xAxis.crosshair && t.series.xAxis.drawCrosshair(null, t), t.series.yAxis.crosshair && t.series.yAxis.drawCrosshair(null, t)) }) : r && (r.setState(r.state, !0), i.axes.forEach(function (t) { t.crosshair && r.series[t.coll] === t && t.drawCrosshair(null, r) }))) : (r && r.onMouseOut(), o && o.forEach(function (t) { t.setState() }), s && s.onMouseOut(), a && a.hide(t), this.unDocMouseMove && (this.unDocMouseMove = this.unDocMouseMove()), i.axes.forEach(function (t) { t.hideCrosshair() }), i.hoverPoints = i.hoverPoint = void 0) } runPointActions(s, t, e) { let r = this.chart, i = r.series, o = r.tooltip && r.tooltip.options.enabled ? r.tooltip : void 0, a = !!o && o.shared, n = t || r.hoverPoint, h = n && n.series || r.hoverSeries, l = (!s || "touchmove" !== s.type) && (!!t || h && h.directTouch && this.isDirectTouch), d = this.getHoverData(n, h, i, l, a, s), c = (n = d.hoverPoint, h = d.hoverSeries, d.hoverPoints), p = h && h.tooltipOptions.followPointer && !h.tooltipOptions.split, u = a && h && !h.noSharedTooltip; if (n && (e || n !== r.hoverPoint || o && o.isHidden)) { if ((r.hoverPoints || []).forEach(function (t) { -1 === c.indexOf(t) && t.setState() }), r.hoverSeries !== h && h.onMouseOver(), this.applyInactiveState(c), (c || []).forEach(function (t) { t.setState("hover") }), r.hoverPoint && r.hoverPoint.firePointEvent("mouseOut"), !n.series) return; r.hoverPoints = c, (r.hoverPoint = n).firePointEvent("mouseOver", void 0, () => { o && n && o.refresh(u ? c : n, s) }) } else p && o && !o.isHidden && (t = o.getAnchor([{}], s), r.isInsidePlot(t[0], t[1], { visiblePlotOnly: !0 }) && o.updatePosition({ plotX: t[0], plotY: t[1] })); this.unDocMouseMove || (this.unDocMouseMove = f(r.container.ownerDocument, "mousemove", t => g[S.hoverChartIndex ?? -1]?.pointer?.onDocumentMouseMove(t)), this.eventsToUnbind.push(this.unDocMouseMove)), r.axes.forEach(function (e) { let t; var i = x((e.crosshair || {}).snap, !0); (t = !i || (t = r.hoverPoint) && t.series[e.coll] === e ? t : m(c, t => t.series && t.series[e.coll] === e)) || !i ? e.drawCrosshair(s, t) : e.hideCrosshair() }) } setDOMEvents() { let t = this.chart.container, e = t.ownerDocument, i = (t.onmousedown = this.onContainerMouseDown.bind(this), t.onmousemove = this.onContainerMouseMove.bind(this), t.onclick = this.onContainerClick.bind(this), this.eventsToUnbind.push(f(t, "mouseenter", this.onContainerMouseEnter.bind(this)), f(t, "mouseleave", this.onContainerMouseLeave.bind(this))), S.unbindDocumentMouseUp || (S.unbindDocumentMouseUp = []), S.unbindDocumentMouseUp.push(f(e, "mouseup", this.onDocumentMouseUp.bind(this))), this.chart.renderTo.parentElement); for (; i && "BODY" !== i.tagName;)this.eventsToUnbind.push(f(i, "scroll", () => { delete this.chartPosition })), i = i.parentElement; this.eventsToUnbind.push(f(t, "touchstart", this.onContainerTouchStart.bind(this), { passive: !1 }), f(t, "touchmove", this.onContainerTouchMove.bind(this), { passive: !1 })), S.unbindDocumentTouchEnd || (S.unbindDocumentTouchEnd = f(e, "touchend", this.onDocumentTouchEnd.bind(this), { passive: !1 })), this.setPointerCapture(), f(this.chart, "redraw", this.setPointerCapture.bind(this)) } setPointerCapture() { if (o) { let t = this.pointerCaptureEventsToUnbind, e = this.chart, i = e.container, s = x(e.options.tooltip?.followTouchMove, !0) && e.series.some(t => -1 < t.options.findNearestPointBy.indexOf("y")); !this.hasPointerCapture && s ? (t.push(f(i, "pointerdown", t => { t.target?.hasPointerCapture(t.pointerId) && t.target?.releasePointerCapture(t.pointerId) }), f(i, "pointermove", t => { e.pointer?.getPointFromEvent(t)?.onMouseOver(t) })), e.styledMode || n(i, { "touch-action": "none" }), i.className += " highcharts-no-touch-action", this.hasPointerCapture = !0) : this.hasPointerCapture && !s && (t.forEach(t => t()), t.length = 0, e.styledMode || n(i, { "touch-action": x(e.options.chart.style?.["touch-action"], "manipulation") }), i.className = i.className.replace(" highcharts-no-touch-action", ""), this.hasPointerCapture = !1) } } setHoverChartIndex(t) { let e = this.chart, i = r.charts[x(S.hoverChartIndex, -1)]; var s; i && i !== e && (s = { relatedTarget: e.container }, t && !t?.relatedTarget && (t = { ...s, ...t }), i.pointer?.onContainerMouseLeave(t || s)), i && i.mouseIsDown || (S.hoverChartIndex = e.index) } touch(t, e) { let i, { chart: s, pinchDown: r = [] } = this; this.setHoverChartIndex(), 1 === (t = this.normalize(t)).touches.length ? s.isInsidePlot(t.chartX - s.plotLeft, t.chartY - s.plotTop, { visiblePlotOnly: !0 }) && !s.openMenu ? (e && this.runPointActions(t), "touchmove" === t.type && (i = !!r[0] && 16 <= Math.pow(r[0].chartX - t.chartX, 2) + Math.pow(r[0].chartY - t.chartY, 2)), x(i, !0) && this.pinch(t)) : e && this.reset() : 2 === t.touches.length && this.pinch(t) } touchSelect(t) { return !(!this.chart.zooming.singleTouch || !t.touches || 1 !== t.touches.length) } zoomOption(t) { let e = this.chart, i = e.inverted, s = e.zooming.type || "", r, o; /touch/.test(t.type) && (s = x(e.zooming.pinchType, s)), this.zoomX = r = /x/.test(s), this.zoomY = o = /y/.test(s), this.zoomHor = r && !i || o && i, this.zoomVert = o && !i || r && i, this.hasZoom = r || o } } return (i = S = S || {}).compose = function (t) { u(s, "Core.Pointer") && f(t, "beforeRender", function () { this.pointer = new i(this, this.options) }) }, S }), e(t, "Core/Legend/LegendSymbol.js", [t["Core/Utilities.js"]], function (t) { var e; let { extend: y, merge: b, pick: v } = t; return (t = e = e || {}).areaMarker = function (t, e) { i.call(this, t, e, !0) }, t.lineMarker = i, t.rectangle = function (t, e) { let i = e.legendItem || {}, s = t.options, r = t.symbolHeight, o = s.squareSymbol, a = o ? r : t.symbolWidth; i.symbol = this.chart.renderer.rect(o ? (t.symbolWidth - r) / 2 : 0, t.baseline - r + 1, a, r, v(t.options.symbolRadius, r / 2)).addClass("highcharts-point").attr({ zIndex: 3 }).add(i.group) }, e; function i(t, e, i) { let s = this.legendItem = this.legendItem || {}, { chart: r, options: o } = this, { baseline: a = 0, symbolWidth: n, symbolHeight: h } = t, l = this.symbol || "circle", d = h / 2, c = r.renderer, p = s.group, u = a - Math.round((t.fontMetrics?.b || h) * (i ? .4 : .3)), g = {}, f, m = o.marker, x = 0; if (r.styledMode || (g["stroke-width"] = Math.min(o.lineWidth || 0, 24), o.dashStyle ? g.dashstyle = o.dashStyle : "square" !== o.linecap && (g["stroke-linecap"] = "round")), s.line = c.path().addClass("highcharts-graph").attr(g).add(p), i && (s.area = c.path().addClass("highcharts-area").add(p)), g["stroke-linecap"] && (x = Math.min(s.line.strokeWidth(), n) / 2), n && (t = [["M", x, u], ["L", n - x, u]], s.line.attr({ d: t }), s.area?.attr({ d: [...t, ["L", n - x, a], ["L", x, a]] })), m && !1 !== m.enabled && n) { let t = Math.min(v(m.radius, d), d); 0 === l.indexOf("url") && (m = b(m, { width: h, height: h }), t = 0), s.symbol = f = c.symbol(l, n / 2 - t, u - t, 2 * t, 2 * t, y({ context: "legend" }, m)).addClass("highcharts-point").add(p), f.isMarker = !0 } } }), e(t, "Core/Series/SeriesDefaults.js", [], function () { return { lineWidth: 2, allowPointSelect: !1, crisp: !0, showCheckbox: !1, animation: { duration: 1e3 }, enableMouseTracking: !0, events: {}, marker: { enabledThreshold: 2, lineColor: "#ffffff", lineWidth: 0, radius: 4, states: { normal: { animation: !0 }, hover: { animation: { duration: 150 }, enabled: !0, radiusPlus: 2, lineWidthPlus: 1 }, select: { fillColor: "#cccccc", lineColor: "#000000", lineWidth: 2 } } }, point: { events: {} }, dataLabels: { animation: {}, align: "center", borderWidth: 0, defer: !0, formatter: function () { let t = this.series.chart["numberFormatter"]; return "number" != typeof this.y ? "" : t(this.y, -1) }, padding: 5, style: { fontSize: "0.7em", fontWeight: "bold", color: "contrast", textOutline: "1px contrast" }, verticalAlign: "bottom", x: 0, y: 0 }, cropThreshold: 300, opacity: 1, pointRange: 0, softThreshold: !0, states: { normal: { animation: !0 }, hover: { animation: { duration: 150 }, lineWidthPlus: 1, marker: {}, halo: { size: 10, opacity: .25 } }, select: { animation: { duration: 0 } }, inactive: { animation: { duration: 150 }, opacity: .2 } }, stickyTracking: !0, turboThreshold: 1e3, findNearestPointBy: "x" } }), e(t, "Core/Series/SeriesRegistry.js", [t["Core/Globals.js"], t["Core/Defaults.js"], t["Core/Series/Point.js"], t["Core/Utilities.js"]], function (t, e, a, i) { var s, n; let h = e["defaultOptions"], { extend: l, extendClass: d, merge: c } = i; return (n = s = s || {}).seriesTypes = t.seriesTypes, n.registerSeriesType = p, n.seriesType = function (t, e, i, s, r) { let o = h.plotOptions || {}; if (e = e || "", o[t] = c(o[e], i), delete n.seriesTypes[t], p(t, d(n.seriesTypes[e] || function () { }, s)), n.seriesTypes[t].prototype.type = t, r) { class p extends a { } l(p.prototype, r), n.seriesTypes[t].prototype.pointClass = p } return n.seriesTypes[t] }, s; function p(t, e) { let i = h.plotOptions || {}, s = e.defaultOptions, r = e.prototype; return r.type = t, r.pointClass || (r.pointClass = a), !n.seriesTypes[t] && (s && (i[t] = s), n.seriesTypes[t] = e, !0) } }), e(t, "Core/Series/Series.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Defaults.js"], t["Core/Foundation.js"], t["Core/Globals.js"], t["Core/Legend/LegendSymbol.js"], t["Core/Series/Point.js"], t["Core/Series/SeriesDefaults.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Renderer/SVG/SVGElement.js"], t["Core/Utilities.js"]], function (t, e, i, s, r, l, o, a, d, c) { let { animObject: p, setAnimation: u } = t, g = e["defaultOptions"], n = i["registerEventOptions"], { svg: f, win: m } = s, y = a["seriesTypes"], { arrayMax: C, arrayMin: M, clamp: w, correctFloat: A, crisp: x, defined: T, destroyObjectProperties: R, diffObjects: z, erase: b, error: P, extend: v, find: N, fireEvent: L, getClosestDistance: W, getNestedProperty: h, insertItem: S, isArray: O, isNumber: E, isString: G, merge: D, objectEach: k, pick: j, removeEvent: H, splat: X, syncTimeout: I } = c; class B { constructor() { this.zoneAxis = "y" } init(t, e) { let i, s = (L(this, "init", { options: e }), this), r = t.series; this.eventsToUnbind = [], s.chart = t, s.options = s.setOptions(e); var e = s.options, o = !1 !== e.visible, o = (s.linkedSeries = [], s.bindAxes(), v(s, { name: e.name, state: "", visible: o, selected: !0 === e.selected }), n(this, e), e.events); (o && o.click || e.point && e.point.events && e.point.events.click || e.allowPointSelect) && (t.runTrackerClick = !0), s.getColor(), s.getSymbol(), s.parallelArrays.forEach(function (t) { s[t + "Data"] || (s[t + "Data"] = []) }), s.isCartesian && (t.hasCartesianSeries = !0), r.length && (i = r[r.length - 1]), s._i = j(i && i._i, -1) + 1, s.opacity = s.options.opacity, t.orderItems("series", S(this, r)), e.dataSorting && e.dataSorting.enabled ? s.setDataSortingOptions() : s.points || s.data || s.setData(e.data, !1), L(this, "afterInit") } is(t) { return y[t] && this instanceof y[t] } bindAxes() { let i, s = this, r = s.options, t = s.chart; L(this, "bindAxes", null, function () { (s.axisTypes || []).forEach(function (e) { (t[e] || []).forEach(function (t) { i = t.options, (j(r[e], 0) === t.index || void 0 !== r[e] && r[e] === i.id) && (S(s, t.series), (s[e] = t).isDirty = !0) }), s[e] || s.optionalAxis === e || P(18, !0, t) }) }), L(this, "afterBindAxes") } updateParallelArrays(i, s, e) { let r = i.series, t = E(s) ? function (t) { var e = "y" === t && r.toYData ? r.toYData(i) : i[t]; r[t + "Data"][s] = e } : function (t) { Array.prototype[s].apply(r[t + "Data"], e) }; r.parallelArrays.forEach(t) } hasData() { return this.visible && void 0 !== this.dataMax && void 0 !== this.dataMin || this.visible && this.yData && 0 < this.yData.length } hasMarkerChanged(t, e) { t = t.marker, e = e.marker || {}; return t && (e.enabled && !t.enabled || e.symbol !== t.symbol || e.height !== t.height || e.width !== t.width) } autoIncrement(t) { let e = this.options, i = e.pointIntervalUnit, s = e.relativeXValue, r = this.chart.time, o = this.xIncrement, a, n; return o = j(o, e.pointStart, 0), this.pointInterval = n = j(this.pointInterval, e.pointInterval, 1), s && E(t) && (n *= t), i && (a = new r.Date(o), "day" === i ? r.set("Date", a, r.get("Date", a) + n) : "month" === i ? r.set("Month", a, r.get("Month", a) + n) : "year" === i && r.set("FullYear", a, r.get("FullYear", a) + n), n = a.getTime() - o), s && E(t) ? o + n : (this.xIncrement = o + n, o) } setDataSortingOptions() { let t = this.options; v(this, { requireSorting: !1, sorted: !1, enabledDataSorting: !0, allowDG: !1 }), T(t.pointRange) || (t.pointRange = 1) } setOptions(t) { let e; var i = this.chart, s = i.options.plotOptions, r = i.userOptions || {}, t = D(t), o = i.styledMode, a = { plotOptions: s, userOptions: t }, n = (L(this, "setOptions", a), a.plotOptions[this.type]), r = r.plotOptions || {}, h = r.series || {}, l = g.plotOptions[this.type] || {}, d = r[this.type] || {}; this.userOptions = a.userOptions; let c = D(n, s.series, d, t), p = (this.tooltipOptions = D(g.tooltip, g.plotOptions.series?.tooltip, l?.tooltip, i.userOptions.tooltip, r.series?.tooltip, d.tooltip, t.tooltip), this.stickyTracking = j(t.stickyTracking, d.stickyTracking, h.stickyTracking, !!this.tooltipOptions.shared && !this.noSharedTooltip || c.stickyTracking), null === n.marker && delete c.marker, this.zoneAxis = c.zoneAxis || "y", this.zones = (c.zones || []).map(t => ({ ...t }))); return !c.negativeColor && !c.negativeFillColor || c.zones || (e = { value: c[this.zoneAxis + "Threshold"] || c.threshold || 0, className: "highcharts-negative" }, o || (e.color = c.negativeColor, e.fillColor = c.negativeFillColor), p.push(e)), p.length && T(p[p.length - 1].value) && p.push(o ? {} : { color: this.color, fillColor: this.fillColor }), L(this, "afterSetOptions", { options: c }), c } getName() { return j(this.options.name, "Series " + (this.index + 1)) } getCyclic(t, e, i) { let s, r, o = this.chart, a = t + "Index", n = t + "Counter", h = i?.length || o.options.chart.colorCount; e || (T(r = j("color" === t ? this.options.colorIndex : void 0, this[a])) ? s = r : (o.series.length || (o[n] = 0), s = o[n] % h, o[n] += 1), i && (e = i[s])), void 0 !== s && (this[a] = s), this[t] = e } getColor() { this.chart.styledMode ? this.getCyclic("color") : this.options.colorByPoint ? this.color = "#cccccc" : this.getCyclic("color", this.options.color || g.plotOptions[this.type].color, this.chart.options.colors) } getPointsCollection() { return (this.hasGroupedData ? this.points : this.data) || [] } getSymbol() { var t = this.options.marker; this.getCyclic("symbol", t.symbol, this.chart.options.symbols) } findPointIndex(e, t) { let i, s, r; var o = e.id, a = e.x, n = this.points, h = this.options.dataSorting; if (o) { o = this.chart.get(o); o instanceof l && (i = o) } else if (this.linkedParent || this.enabledDataSorting || this.options.relativeXValue) { let t = t => !t.touched && t.index === e.index; if (h && h.matchByName ? t = t => !t.touched && t.name === e.name : this.options.relativeXValue && (t = t => !t.touched && t.options.x === e.x), !(i = N(n, t))) return } return i && void 0 !== (r = i && i.index) && (s = !0), -1 !== (r = void 0 === r && E(a) ? this.xData.indexOf(a, t) : r) && void 0 !== r && this.cropped && (r = r >= this.cropStart ? r - this.cropStart : r), r = !s && E(r) && n[r] && n[r].touched ? void 0 : r } updateData(t, e) { let r = this.options, o = r.dataSorting, a = this.points, n = [], h = this.requireSorting, l = t.length === a.length, d, i, s, c, p = !0; if (this.xIncrement = null, t.forEach(function (t, e) { var i = T(t) && this.pointClass.prototype.optionsToObject.call({ series: this }, t) || {}, s = i.x; i.id || E(s) ? (-1 === (s = this.findPointIndex(i, c)) || void 0 === s ? n.push(t) : a[s] && t !== r.data[s] ? (a[s].update(t, !1, null, !1), a[s].touched = !0, h && (c = s + 1)) : a[s] && (a[s].touched = !0), (!l || e !== s || o && o.enabled || this.hasDerivedData) && (d = !0)) : n.push(t) }, this), d) for (i = a.length; i--;)(s = a[i]) && !s.touched && s.remove && s.remove(!1, e); else !l || o && o.enabled ? p = !1 : (t.forEach(function (t, e) { t === a[e].y || a[e].destroyed || a[e].update(t, !1, null, !1) }), n.length = 0); return a.forEach(function (t) { t && (t.touched = !1) }), !!p && (n.forEach(function (t) { this.addPoint(t, !1, null, null, !1) }, this), null === this.xIncrement && this.xData && this.xData.length && (this.xIncrement = C(this.xData), this.autoIncrement()), !0) } setData(e, t = !0, i, s) { let r = this, o = r.points, a = o && o.length || 0, n = r.options, h = r.chart, l = n.dataSorting, d = r.xAxis, c = n.turboThreshold, p = this.xData, u = this.yData, g = r.pointArrayMap, f = g && g.length, m = n.keys, x, y, b, v = 0, S = 1, k; h.options.chart.allowMutatingData || (n.data && delete r.options.data, r.userOptions.data && delete r.userOptions.data, k = D(!0, e)); var C = (e = k || e || []).length; if (l && l.enabled && (e = this.sortData(e)), !(b = h.options.chart.allowMutatingData && !1 !== s && C && a && !r.cropped && !r.hasGroupedData && r.visible && !r.boosted ? this.updateData(e, i) : b)) { r.xIncrement = null, r.colorCounter = 0, this.parallelArrays.forEach(function (t) { r[t + "Data"].length = 0 }); let t = c && c < C; if (t) { var s = r.getFirstValidPoint(e), M = r.getFirstValidPoint(e, C - 1, -1), w = t => !(!O(t) || !m && !E(t[0])); if (E(s) && E(M)) for (x = 0; x < C; x++)p[x] = this.autoIncrement(), u[x] = e[x]; else if (w(s) && w(M)) if (f) if (s.length === f) for (x = 0; x < C; x++)p[x] = this.autoIncrement(), u[x] = e[x]; else for (x = 0; x < C; x++)y = e[x], p[x] = y[0], u[x] = y.slice(1, f + 1); else if (m && (v = m.indexOf("x"), S = m.indexOf("y"), v = 0 <= v ? v : 0, S = 0 <= S ? S : 1), 1 === s.length && (S = 0), v === S) for (x = 0; x < C; x++)p[x] = this.autoIncrement(), u[x] = e[x][S]; else for (x = 0; x < C; x++)y = e[x], p[x] = y[v], u[x] = y[S]; else t = !1 } if (!t) for (x = 0; x < C; x++)y = { series: r }, r.pointClass.prototype.applyOptions.apply(y, [e[x]]), r.updateParallelArrays(y, x); for (u && G(u[0]) && P(14, !0, h), r.data = [], r.options.data = r.userOptions.data = e, x = a; x--;)o[x]?.destroy(); d && (d.minRange = d.userMinRange), r.isDirty = h.isDirtyBox = !0, r.isDirtyData = !!o, i = !1 } "point" === n.legendType && (this.processData(), this.generatePoints()), t && h.redraw(i) } sortData(r) { function o(t, e) { return T(e) && t.pointClass.prototype.optionsToObject.call({ series: t }, e) || {} } let i = this, s = i.options.dataSorting.sortKey || "y"; return r.forEach(function (t, e) { r[e] = o(i, t), r[e].index = e }, this), r.concat().sort((t, e) => { t = h(s, t), e = h(s, e); return e < t ? -1 : t < e ? 1 : 0 }).forEach(function (t, e) { t.x = e }, this), i.linkedSeries && i.linkedSeries.forEach(function (i) { let t = i.options, s = t.data; t.dataSorting && t.dataSorting.enabled || !s || (s.forEach(function (t, e) { s[e] = o(i, t), r[e] && (s[e].x = r[e].x, s[e].index = e) }), i.setData(s, !1)) }), r } getProcessedData(t) { let e = this, i = e.xAxis, s = e.options.cropThreshold, r = i?.logarithmic, o = e.isCartesian, a, n, h = 0, l, d, c, p = e.xData, u = e.yData, g = !1, f = p.length; i && (d = (l = i.getExtremes()).min, c = l.max, g = !(!i.categories || i.names.length)), o && e.sorted && !t && (!s || s < f || e.forceCrop) && (p[f - 1] < d || p[0] > c ? (p = [], u = []) : e.yData && (p[0] < d || p[f - 1] > c) && (p = (a = this.cropData(e.xData, e.yData, d, c)).xData, u = a.yData, h = a.start, n = !0)); t = W([r ? p.map(r.log2lin) : p], () => e.requireSorting && !g && P(15, !1, e.chart)); return { xData: p, yData: u, cropped: n, cropStart: h, closestPointRange: t } } processData(t) { var e = this.xAxis; if (this.isCartesian && !this.isDirty && !e.isDirty && !this.yAxis.isDirty && !t) return !1; e = this.getProcessedData(); this.cropped = e.cropped, this.cropStart = e.cropStart, this.processedXData = e.xData, this.processedYData = e.yData, this.closestPointRange = this.basePointRange = e.closestPointRange, L(this, "afterProcessData") } cropData(t, e, i, s) { let r = t.length, o, a, n = 0, h = r; for (o = 0; o < r; o++)if (t[o] >= i) { n = Math.max(0, o - 1); break } for (a = o; a < r; a++)if (t[a] > s) { h = a + 1; break } return { xData: t.slice(n, h), yData: e.slice(n, h), start: n, end: h } } generatePoints() { let t = this.options, e = this.processedData || t.data, i = this.processedXData, s = this.processedYData, r = this.pointClass, o = i.length, a = this.cropStart || 0, n = this.hasGroupedData, h = t.keys, l = [], d = t.dataGrouping && t.dataGrouping.groupAll ? a : 0, c, p, u, g, f = this.data; if (!f && !n) { let t = []; t.length = e.length, f = this.data = t } for (h && n && (this.options.keys = !1), g = 0; g < o; g++)p = a + g, n ? ((u = new r(this, [i[g]].concat(X(s[g])))).dataGroup = this.groupMap[d + g], u.dataGroup.options && (u.options = u.dataGroup.options, v(u, u.dataGroup.options), delete u.dataLabels)) : (u = f[p]) || void 0 === e[p] || (f[p] = u = new r(this, e[p], i[g])), u && (u.index = n ? d + g : p, l[g] = u); if (this.options.keys = h, f && (o !== (c = f.length) || n)) for (g = 0; g < c; g++)g !== a || n || (g += o), f[g] && (f[g].destroyElements(), f[g].plotX = void 0); this.data = f, this.points = l, L(this, "afterGeneratePoints") } getXExtremes(t) { return { min: M(t), max: C(t) } } getExtremes(t, e) { let i = this.xAxis, s = this.yAxis, r = [], o = this.requireSorting && !this.is("column") ? 1 : 0, a = !!s && s.positiveValuesOnly, n = e || this.getExtremesFromAll || this.options.getExtremesFromAll, { processedXData: h, processedYData: l } = this, d, c, p, u, g, f, m, x = 0, y = 0, b = 0; this.cropped && n && (k = this.getProcessedData(!0), h = k.xData, l = k.yData); var v = (t = t || this.stackedYData || l || []).length, S = h || this.xData; for (i && (x = (d = i.getExtremes()).min, y = d.max), f = 0; f < v; f++)if (u = S[f], c = (E(g = t[f]) || O(g)) && ((E(g) ? 0 < g : g.length) || !a), p = e || this.getExtremesFromAll || this.options.getExtremesFromAll || this.cropped || !i || (S[f + o] || u) >= x && (S[f - o] || u) <= y, c && p) if (m = g.length) for (; m--;)E(g[m]) && (r[b++] = g[m]); else r[b++] = g; var k = { activeYData: r, dataMin: M(r), dataMax: C(r) }; return L(this, "afterGetExtremes", { dataExtremes: k }), k } applyExtremes() { var t = this.getExtremes(); return this.dataMin = t.dataMin, this.dataMax = t.dataMax, t } getFirstValidPoint(t, e = 0, i = 1) { let s = t.length, r = e; for (; 0 <= r && r < s;) { if (T(t[r])) return t[r]; r += i } } translate() { this.processedXData || this.processData(), this.generatePoints(); let t = this.options, h = t.stacking, l = this.xAxis, d = l.categories, c = this.enabledDataSorting, p = this.yAxis, u = this.points, e = u.length, g = this.pointPlacementToXValue(), f = !!g, m = t.threshold, x = t.startFromThreshold ? m : 0, y, b, v, S, k = Number.MAX_VALUE; function C(t) { return w(t, -1e9, 1e9) } for (y = 0; y < e; y++) { let t, e = u[y], i = e.x, s, r, o = e.y, a = e.low, n = h && p.stacking?.stacks[(this.negStacks && o < (x ? 0 : m) ? "-" : "") + this.stackKey]; b = l.translate(i, !1, !1, !1, !0, g), e.plotX = E(b) ? A(C(b)) : void 0, h && this.visible && n && n[i] && (S = this.getStackIndicator(S, i, this.index), !e.isNull && S.key && (r = (s = n[i]).points[S.key]), s && O(r) && (a = r[0], o = r[1], a === x && S.key === n[i].base && (a = j(E(m) ? m : p.min)), p.positiveValuesOnly && T(a) && a <= 0 && (a = void 0), e.total = e.stackTotal = j(s.total), e.percentage = T(e.y) && s.total ? e.y / s.total * 100 : void 0, e.stackY = o, this.irregularWidths || s.setOffset(this.pointXOffset || 0, this.barW || 0, void 0, void 0, void 0, this.xAxis))), e.yBottom = T(a) ? C(p.translate(a, !1, !0, !1, !0)) : void 0, this.dataModify && (o = this.dataModify.modifyValue(o, y)), E(o) && void 0 !== e.plotX && (t = E(t = p.translate(o, !1, !0, !1, !0)) ? C(t) : void 0), e.plotY = t, e.isInside = this.isPointInside(e), e.clientX = f ? A(l.translate(i, !1, !1, !1, !0, g)) : b, e.negative = (e.y || 0) < (m || 0), e.category = j(d && d[e.x], e.x), e.isNull || !1 === e.visible || (void 0 !== v && (k = Math.min(k, Math.abs(b - v))), v = b), e.zone = this.zones.length ? e.getZone() : void 0, !e.graphic && this.group && c && (e.isNew = !0) } this.closestPointRangePx = k, L(this, "afterTranslate") } getValidPoints(t, s, r) { let o = this.chart; return (t || this.points || []).filter(function (t) { var { plotX: e, plotY: i } = t; return !(!(r || !t.isNull && E(i)) || s && !o.isInsidePlot(e, i, { inverted: o.inverted })) && !1 !== t.visible }) } getClipBox() { let { chart: t, xAxis: e, yAxis: i } = this, { x: s, y: r, width: o, height: a } = D(t.clipBox); return e && e.len !== t.plotSizeX && (o = e.len), i && i.len !== t.plotSizeY && (a = i.len), t.inverted && !this.invertible && ([o, a] = [a, o]), { x: s, y: r, width: o, height: a } } getSharedClipKey() { return this.sharedClipKey = (this.options.xAxis || 0) + "," + (this.options.yAxis || 0), this.sharedClipKey } setClip() { let { chart: t, group: e, markerGroup: i } = this, s = t.sharedClips, r = t.renderer, o = this.getClipBox(), a = this.getSharedClipKey(), n = s[a]; n ? n.animate(o) : s[a] = n = r.clipRect(o), e && e.clip(!1 === this.options.clip ? void 0 : n), i && i.clip() } animate(e) { let { chart: s, group: i, markerGroup: r } = this, o = s.inverted, a = p(this.options.animation), n = [this.getSharedClipKey(), a.duration, a.easing, a.defer].join(","), h = s.sharedClips[n], l = s.sharedClips[n + "m"]; if (e && i) { let t = this.getClipBox(); h ? h.attr("height", t.height) : (t.width = 0, o && (t.x = s.plotHeight), h = s.renderer.clipRect(t), s.sharedClips[n] = h, e = { x: -99, y: -99, width: o ? s.plotWidth + 199 : 99, height: o ? 99 : s.plotHeight + 199 }, l = s.renderer.clipRect(e), s.sharedClips[n + "m"] = l), i.clip(h), r?.clip(l) } else if (h && !h.hasClass("highcharts-animating")) { let t = this.getClipBox(), i = a.step; (r?.element.childNodes.length || 1 < s.series.length) && (a.step = function (t, e) { i && i.apply(e, arguments), "width" === e.prop && l?.element && l.attr(o ? "height" : "width", t + 99) }), h.addClass("highcharts-animating").animate(t, a) } } afterAnimate() { this.setClip(), k(this.chart.sharedClips, (t, e, i) => { t && !this.chart.container.querySelector(`[clip-path="url(#${t.id})"]`) && (t.destroy(), delete i[e]) }), this.finishedAnimating = !0, L(this, "afterAnimate") } drawPoints(t = this.points) { let e, i, s, r, o, a, n, h = this.chart, l = h.styledMode, { colorAxis: d, options: c } = this, p = c.marker, u = this[this.specialGroup || "markerGroup"], g = this.xAxis, f = j(p.enabled, !g || !!g.isRadial || null, this.closestPointRangePx >= p.enabledThreshold * p.radius); if (!1 !== p.enabled || this._hasPointMarkers) for (e = 0; e < t.length; e++) { var m, x; r = (s = (i = t[e]).graphic) ? "animate" : "attr", o = i.marker || {}, a = !!i.marker, (f && void 0 === o.enabled || o.enabled) && !i.isNull && !1 !== i.visible ? (x = j(o.symbol, this.symbol, "rect"), n = this.markerAttribs(i, i.selected && "select"), this.enabledDataSorting && (i.startXPos = g.reversed ? -(n.width || 0) : g.width), m = !1 !== i.isInside, !s && m && (0 < (n.width || 0) || i.hasImage) && (i.graphic = s = h.renderer.symbol(x, n.x, n.y, n.width, n.height, a ? o : p).add(u), this.enabledDataSorting && h.hasRendered && (s.attr({ x: i.startXPos }), r = "animate")), s && "animate" === r && s[m ? "show" : "hide"](m).animate(n), s && (x = this.pointAttribs(i, l || !i.selected ? void 0 : "select"), l ? d && s.css({ fill: x.fill }) : s[r](x)), s && s.addClass(i.getClassName(), !0)) : s && (i.graphic = s.destroy()) } } markerAttribs(t, e) { let i = this.options, s = i.marker, r = t.marker || {}, o = r.symbol || s.symbol, a = {}, n, h, l = j(r.radius, s && s.radius), d = (e && (n = s.states[e], l = j((h = r.states && r.states[e]) && h.radius, n && n.radius, l && l + (n && n.radiusPlus || 0))), t.hasImage = o && 0 === o.indexOf("url"), t.hasImage && (l = 0), t.pos()); return E(l) && d && (i.crisp && (d[0] = x(d[0], t.hasImage ? 0 : "rect" === o ? s?.lineWidth || 0 : 1)), a.x = d[0] - l, a.y = d[1] - l), l && (a.width = a.height = 2 * l), a } pointAttribs(t, e) { let i = this.options.marker, s = t && t.options, r = s && s.marker || {}, o = s && s.color, a = t && t.color, n = t && t.zone && t.zone.color, h, l, d = this.color, c, p, u = j(r.lineWidth, i.lineWidth); return d = o || n || a || d, c = r.fillColor || i.fillColor || d, p = r.lineColor || i.lineColor || d, h = i.states[e = e || "normal"] || {}, u = j((l = r.states && r.states[e] || {}).lineWidth, h.lineWidth, u + j(l.lineWidthPlus, h.lineWidthPlus, 0)), c = l.fillColor || h.fillColor || c, { stroke: p = l.lineColor || h.lineColor || p, "stroke-width": u, fill: c, opacity: j(l.opacity, h.opacity, 1) } } destroy(i) { let t, e, s, r = this, o = r.chart, a = /AppleWebKit\/533/.test(m.navigator.userAgent), n = r.data || []; for (L(r, "destroy", { keepEventsForUpdate: i }), this.removeEvents(i), (r.axisTypes || []).forEach(function (t) { (s = r[t]) && s.series && (b(s.series, r), s.isDirty = s.forceRedraw = !0) }), r.legendItem && r.chart.legend.destroyItem(r), t = n.length; t--;)(e = n[t]) && e.destroy && e.destroy(); for (var h of r.zones) R(h, void 0, !0); c.clearTimeout(r.animationTimeout), k(r, function (t, e) { t instanceof d && !t.survive && t[a && "group" === e ? "hide" : "destroy"]() }), o.hoverSeries === r && (o.hoverSeries = void 0), b(o.series, r), o.orderItems("series"), k(r, function (t, e) { i && "hcEvents" === e || delete r[e] }) } applyZones() { let { area: g, chart: t, graph: f, zones: i, points: s, xAxis: m, yAxis: x, zoneAxis: r } = this, { inverted: y, renderer: b } = t, o = this[r + "Axis"], { isXAxis: v, len: a = 0 } = o || {}, n = (f?.strokeWidth() || 0) / 2 + 1, h = (t, e = 0, i = 0) => { let { translated: s = 0, lineClip: r } = t, o = (i = y ? a - i : i) - s; r?.push(["L", e, Math.abs(o) < n ? i - n * (o <= 0 ? -1 : 1) : s]) }; if (i.length && (f || g) && o && E(o.min)) { let e = o.getExtremes().max, c = i => { i.forEach((t, e) => { "M" !== t[0] && "L" !== t[0] || (i[e] = [t[0], v ? a - t[1] : t[1], v ? t[2] : a - t[2]]) }) }; if (i.forEach(t => { t.lineClip = [], t.translated = w(o.toPixels(j(t.value, e), !0) || 0, 0, a) }), f && !this.showLine && f.hide(), g && g.hide(), "y" === r && s.length < m.len) for (var l of s) { var { plotX: l, plotY: d, zone: S } = l, k = S && i[i.indexOf(S) - 1]; S && h(S, l, d), k && h(k, l, d) } let p = [], u = o.toPixels(o.getExtremes().min, !0); i.forEach(t => { let e = t.lineClip || [], i = Math.round(t.translated || 0), { clip: s, simpleClip: r } = (m.reversed && e.reverse(), t), o = 0, a = 0, n = m.len, h = x.len; v ? (o = i, n = u) : (a = i, h = u); var l = [["M", o, a], ["L", n, a], ["L", n, h], ["L", o, h], ["Z"]], d = [l[0], ...e, l[1], l[2], ...p, l[3], l[4]]; p = e.reverse(), u = i, y && (c(d), g && c(l)), s ? (s.animate({ d: d }), r?.animate({ d: l })) : (s = t.clip = b.path(d), g && (r = t.simpleClip = b.path(l))), f && t.graph?.clip(s), g && t.area?.clip(r) }) } else this.visible && (f && f.show(), g && g.show()) } plotGroup(t, e, i, s, r) { let o = this[t], a = !o, n = { visibility: i, zIndex: s || .1 }; return T(this.opacity) && !this.chart.styledMode && "inactive" !== this.state && (n.opacity = this.opacity), o || (this[t] = o = this.chart.renderer.g().add(r)), o.addClass("highcharts-" + e + " highcharts-series-" + this.index + " highcharts-" + this.type + "-series " + (T(this.colorIndex) ? "highcharts-color-" + this.colorIndex + " " : "") + (this.options.className || "") + (o.hasClass("highcharts-tracker") ? " highcharts-tracker" : ""), !0), o.attr(n)[a ? "attr" : "animate"](this.getPlotBox(e)), o } getPlotBox(t) { let e = this.xAxis, i = this.yAxis, s = this.chart, r = s.inverted && !s.polar && e && this.invertible && "series" === t; return s.inverted && (e = i, i = this.xAxis), { translateX: e ? e.left : s.plotLeft, translateY: i ? i.top : s.plotTop, rotation: r ? 90 : 0, rotationOriginX: r ? (e.len - i.len) / 2 : 0, rotationOriginY: r ? (e.len + i.len) / 2 : 0, scaleX: r ? -1 : 1, scaleY: 1 } } removeEvents(t) { let e = this["eventsToUnbind"]; t || H(this), e.length && (e.forEach(t => { t() }), e.length = 0) } render() { let t = this, { chart: e, options: i, hasRendered: s } = t, r = p(i.animation), o = t.visible ? "inherit" : "hidden", a = i.zIndex, n = e.seriesGroup, h = t.finishedAnimating ? 0 : r.duration; L(this, "render"), t.plotGroup("group", "series", o, a, n), t.markerGroup = t.plotGroup("markerGroup", "markers", o, a, n), !1 !== i.clip && t.setClip(), h && t.animate?.(!0), t.drawGraph && (t.drawGraph(), t.applyZones()), t.visible && t.drawPoints(), t.drawDataLabels?.(), t.redrawPoints?.(), i.enableMouseTracking && t.drawTracker?.(), h && t.animate?.(), s || (h && r.defer && (h += r.defer), t.animationTimeout = I(() => { t.afterAnimate() }, h || 0)), t.isDirty = !1, t.hasRendered = !0, L(t, "afterRender") } redraw() { var t = this.isDirty || this.isDirtyData; this.translate(), this.render(), t && delete this.kdTree } reserveSpace() { return this.visible || !this.chart.options.chart.ignoreHiddenSeries } searchPoint(t, e) { var { xAxis: i, yAxis: s } = this, r = this.chart.inverted; return this.searchKDTree({ clientX: r ? i.len - t.chartY + i.pos : t.chartX - i.pos, plotY: r ? s.len - t.chartX + s.pos : t.chartY - s.pos }, e, t) } buildKDTree(t) { this.buildingKdTree = !0; let n = this, e = -1 < n.options.findNearestPointBy.indexOf("y") ? 2 : 1; delete n.kdTree, I(function () { n.kdTree = function t(e, i, s) { let r, o; var a = e?.length; if (a) return r = n.kdAxisArray[i % s], e.sort((t, e) => (t[r] || 0) - (e[r] || 0)), { point: e[o = Math.floor(a / 2)], left: t(e.slice(0, o), i + 1, s), right: t(e.slice(o + 1), i + 1, s) } }(n.getValidPoints(void 0, !n.directTouch), e, e), n.buildingKdTree = !1 }, n.options.kdNow || "touchstart" === t?.type ? 0 : 1) } searchKDTree(t, e, i) { let g = this, [f, m] = this.kdAxisArray, x = e ? "distX" : "dist", s = -1 < (g.options.findNearestPointBy || "").indexOf("y") ? 2 : 1, y = !!g.isBubble; if (this.kdTree || this.buildingKdTree || this.buildKDTree(i), this.kdTree) return function t(e, i, s, r) { let o = i.point, a = g.kdAxisArray[s % r], n, h, l = o; p = o, u = (d = e)[f], c = p[f], u = T(u) && T(c) ? u - c : null, c = d[m], d = p[m], c = T(c) && T(d) ? c - d : 0, d = y && p.marker?.radius || 0, p.dist = Math.sqrt((u && u * u || 0) + c * c) - d, p.distX = T(u) ? Math.abs(u) - d : Number.MAX_VALUE; var d, c = (e[a] || 0) - (o[a] || 0) + (y && o.marker?.radius || 0), p = c < 0 ? "left" : "right", u = c < 0 ? "right" : "left"; return i[p] && (l = (n = t(e, i[p], s + 1, r))[x] < l[x] ? n : o), l = i[u] && Math.sqrt(c * c) < l[x] ? (h = t(e, i[u], s + 1, r))[x] < l[x] ? h : l : l }(t, this.kdTree, s, s) } pointPlacementToXValue() { let { options: t, xAxis: e } = this, i = t.pointPlacement; return "between" === i && (i = e.reversed ? -.5 : .5), E(i) ? i * (t.pointRange || e.pointRange) : 0 } isPointInside(t) { var { chart: e, xAxis: i, yAxis: s } = this, { plotX: t = -1, plotY: r = -1 } = t; return 0 <= r && r <= (s ? s.len : e.plotHeight) && 0 <= t && t <= (i ? i.len : e.plotWidth) } drawTracker() { let t = this, e = t.options, i = e.trackByArea, s = [].concat((i ? t.areaPath : t.graphPath) || []), r = t.chart, o = r.pointer, a = r.renderer, n = r.options.tooltip?.snap || 0, h = () => { e.enableMouseTracking && r.hoverSeries !== t && t.onMouseOver() }, l = "rgba(192,192,192," + (f ? 1e-4 : .002) + ")", d = t.tracker; d ? d.attr({ d: s }) : t.graph && (t.tracker = d = a.path(s).attr({ visibility: t.visible ? "inherit" : "hidden", zIndex: 2 }).addClass(i ? "highcharts-tracker-area" : "highcharts-tracker-line").add(t.group), r.styledMode || d.attr({ "stroke-linecap": "round", "stroke-linejoin": "round", stroke: l, fill: i ? l : "none", "stroke-width": t.graph.strokeWidth() + (i ? 0 : 2 * n) }), [t.tracker, t.markerGroup, t.dataLabelsGroup].forEach(t => { t && (t.addClass("highcharts-tracker").on("mouseover", h).on("mouseout", t => { o?.onTrackerMouseOut(t) }), e.cursor && !r.styledMode && t.css({ cursor: e.cursor }), t.on("touchstart", h)) })), L(this, "afterDrawTracker") } addPoint(t, e, i, s, r) { let o, a, n = this.options, h = this.data, l = this.chart, d = this.xAxis, c = d && d.hasNames && d.names, p = n.data, u = this.xData; e = j(e, !0); var g = { series: this }, f = (this.pointClass.prototype.applyOptions.apply(g, [t]), g.x); if (a = u.length, this.requireSorting && f < u[a - 1]) for (o = !0; a && u[a - 1] > f;)a--; this.updateParallelArrays(g, "splice", [a, 0, 0]), this.updateParallelArrays(g, a), c && g.name && (c[f] = g.name), p.splice(a, 0, t), (o || this.processedData) && (this.data.splice(a, 0, null), this.processData()), "point" === n.legendType && this.generatePoints(), i && (h[0] && h[0].remove ? h[0].remove(!1) : (h.shift(), this.updateParallelArrays(g, "shift"), p.shift())), !1 !== r && L(this, "addPoint", { point: g }), this.isDirty = !0, this.isDirtyData = !0, e && l.redraw(s) } removePoint(t, e, i) { function s() { n && n.length === o.length && n.splice(t, 1), o.splice(t, 1), r.options.data.splice(t, 1), r.updateParallelArrays(a || { series: r }, "splice", [t, 1]), a && a.destroy(), r.isDirty = !0, r.isDirtyData = !0, e && h.redraw() } let r = this, o = r.data, a = o[t], n = r.points, h = r.chart; u(i, h), e = j(e, !0), a ? a.firePointEvent("remove", null, s) : s() } remove(t, e, i, s) { let r = this, o = r.chart; function a() { r.destroy(s), o.isDirtyLegend = o.isDirtyBox = !0, o.linkSeries(s), j(t, !0) && o.redraw(e) } !1 !== i ? L(r, "remove", null, a) : a() } update(t, e) { L(this, "update", { options: t = z(t, this.userOptions) }); let i = this, s = i.chart, r = i.userOptions, o = i.initialType || i.type, a = s.options.plotOptions, n = y[o].prototype, h = i.finishedAnimating && { animation: !1 }, l = {}, d, c, p = ["colorIndex", "eventOptions", "navigatorSeries", "symbolIndex", "baseSeries"], u = t.type || r.type || s.options.chart.type, g = !(this.hasDerivedData || u && u !== this.type || void 0 !== t.pointStart || void 0 !== t.pointInterval || void 0 !== t.relativeXValue || t.joinBy || t.mapData || ["dataGrouping", "pointStart", "pointInterval", "pointIntervalUnit", "keys"].some(t => i.hasOptionChanged(t))), f = (u = u || o, g && (p.push("data", "isDirtyData", "isDirtyCanvas", "points", "processedData", "processedXData", "processedYData", "xIncrement", "cropped", "_hasPointMarkers", "hasDataLabels", "nodes", "layout", "level", "mapMap", "mapData", "minY", "maxY", "minX", "maxX", "transformGroups"), !1 !== t.visible && p.push("area", "graph"), i.parallelArrays.forEach(function (t) { p.push(t + "Data") }), t.data && (t.dataSorting && v(i.options.dataSorting, t.dataSorting), this.setData(t.data, !1))), t = D(r, { index: (void 0 === r.index ? i : r).index, pointStart: a?.series?.pointStart ?? r.pointStart ?? i.xData?.[0] }, !g && { data: i.options.data }, t, h), g && t.data && (t.data = i.options.data), (p = ["group", "markerGroup", "dataLabelsGroup", "transformGroup"].concat(p)).forEach(function (t) { p[t] = i[t], delete i[t] }), !1); if (y[u]) { if (f = u !== i.type, i.remove(!1, !1, !1, !0), f) if (s.propFromSeries(), Object.setPrototypeOf) Object.setPrototypeOf(i, y[u].prototype); else { var m = Object.hasOwnProperty.call(i, "hcEvents") && i.hcEvents; for (c in n) i[c] = void 0; v(i, y[u].prototype), m ? i.hcEvents = m : delete i.hcEvents } } else P(17, !0, s, { missingModuleFor: u }); if (p.forEach(function (t) { i[t] = p[t] }), i.init(s, t), g && this.points) for (var x of (!1 === (d = i.options).visible ? (l.graphic = 1, l.dataLabel = 1) : (this.hasMarkerChanged(d, r) && (l.graphic = 1), i.hasDataLabels?.() || (l.dataLabel = 1)), this.points)) x && x.series && (x.resolveColor(), Object.keys(l).length && x.destroyElements(l), !1 === d.showInLegend && x.legendItem && s.legend.destroyItem(x)); i.initialType = o, s.linkSeries(), s.setSortedData(), f && i.linkedSeries.length && (i.isDirtyData = !0), L(this, "afterUpdate"), j(e, !0) && s.redraw(!!g && void 0) } setName(t) { this.name = this.options.name = this.userOptions.name = t, this.chart.isDirtyLegend = !0 } hasOptionChanged(t) { var e = this.chart, i = this.options[t], e = e.options.plotOptions, s = this.userOptions[t], e = j(e?.[this.type]?.[t], e?.series?.[t]); return s && !T(e) ? i !== s : i !== j(e, i) } onMouseOver() { let t = this.chart, e = t.hoverSeries, i = t.pointer; i?.setHoverChartIndex(), e && e !== this && e.onMouseOut(), this.options.events.mouseOver && L(this, "mouseOver"), this.setState("hover"), t.hoverSeries = this } onMouseOut() { let t = this.options, e = this.chart, i = e.tooltip, s = e.hoverPoint; e.hoverSeries = null, s && s.onMouseOut(), this && t.events.mouseOut && L(this, "mouseOut"), !i || this.stickyTracking || i.shared && !this.noSharedTooltip || i.hide(), e.series.forEach(function (t) { t.setState("", !0) }) } setState(e, t) { let i = this, s = i.options, r = i.graph, o = s.inactiveOtherPoints, a = s.states, n = j(a[e || "normal"] && a[e || "normal"].animation, i.chart.options.chart.animation), h = s.lineWidth, l = s.opacity; if (e = e || "", i.state !== e && ([i.group, i.markerGroup, i.dataLabelsGroup].forEach(function (t) { t && (i.state && t.removeClass("highcharts-series-" + i.state), e && t.addClass("highcharts-series-" + e)) }), i.state = e, !i.chart.styledMode)) { if (a[e] && !1 === a[e].enabled) return; if (e && (h = a[e].lineWidth || h + (a[e].lineWidthPlus || 0), l = j(a[e].opacity, l)), r && !r.dashstyle && E(h)) for (var d of [r, ...this.zones.map(t => t.graph)]) d?.animate({ "stroke-width": h }, n); o || [i.group, i.markerGroup, i.dataLabelsGroup, i.labelBySeries].forEach(function (t) { t && t.animate({ opacity: l }, n) }) } t && o && i.points && i.setAllPointsToState(e || void 0) } setAllPointsToState(e) { this.points.forEach(function (t) { t.setState && t.setState(e) }) } setVisible(e, t) { let i = this, s = i.chart, r = s.options.chart.ignoreHiddenSeries, o = i.visible, a = (i.visible = e = i.options.visible = i.userOptions.visible = void 0 === e ? !o : e, e ? "show" : "hide");["group", "dataLabelsGroup", "markerGroup", "tracker", "tt"].forEach(t => { i[t]?.[a]() }), s.hoverSeries !== i && s.hoverPoint?.series !== i || i.onMouseOut(), i.legendItem && s.legend.colorizeItem(i, e), i.isDirty = !0, i.options.stacking && s.series.forEach(t => { t.options.stacking && t.visible && (t.isDirty = !0) }), i.linkedSeries.forEach(t => { t.setVisible(e, !1) }), r && (s.isDirtyBox = !0), L(i, a), !1 !== t && s.redraw() } show() { this.setVisible(!0) } hide() { this.setVisible(!1) } select(t) { this.selected = t = this.options.selected = void 0 === t ? !this.selected : t, this.checkbox && (this.checkbox.checked = t), L(this, t ? "select" : "unselect") } shouldShowTooltip(t, e, i = {}) { return i.series = this, i.visiblePlotOnly = !0, this.chart.isInsidePlot(t, e, i) } drawLegendSymbol(t, e) { r[this.options.legendSymbol || "rectangle"]?.call(this, t, e) } } return B.defaultOptions = o, B.types = a.seriesTypes, B.registerType = a.registerSeriesType, v(B.prototype, { axisTypes: ["xAxis", "yAxis"], coll: "series", colorCounter: 0, directTouch: !1, invertible: !0, isCartesian: !0, kdAxisArray: ["clientX", "plotY"], parallelArrays: ["x", "y"], pointClass: l, requireSorting: !0, sorted: !0 }), a.series = B }), e(t, "Core/Legend/Legend.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Foundation.js"], t["Core/Globals.js"], t["Core/Series/Series.js"], t["Core/Series/Point.js"], t["Core/Renderer/RendererUtilities.js"], t["Core/Templating.js"], t["Core/Utilities.js"]], function (t, e, i, u, g, s, r, o) { var a; let { animObject: d, setAnimation: c } = t, n = e["registerEventOptions"], { composed: h, marginNames: l } = i, p = s["distribute"], f = r["format"], { addEvent: m, createElement: x, css: y, defined: b, discardElement: v, find: S, fireEvent: k, isNumber: C, merge: M, pick: w, pushUnique: A, relativeLength: T, stableSort: P, syncTimeout: L } = o; class O { constructor(t, e) { this.allItems = [], this.initialItemY = 0, this.itemHeight = 0, this.itemMarginBottom = 0, this.itemMarginTop = 0, this.itemX = 0, this.itemY = 0, this.lastItemY = 0, this.lastLineHeight = 0, this.legendHeight = 0, this.legendWidth = 0, this.maxItemWidth = 0, this.maxLegendWidth = 0, this.offsetWidth = 0, this.padding = 0, this.pages = [], this.symbolHeight = 0, this.symbolWidth = 0, this.titleHeight = 0, this.totalItemWidth = 0, this.widthOption = 0, this.chart = t, this.setOptions(e), e.enabled && (this.render(), n(this, e), m(this.chart, "endResize", function () { this.legend.positionCheckboxes() })), m(this.chart, "render", () => { this.options.enabled && this.proximate && (this.proximatePositions(), this.positionItems()) }) } setOptions(t) { var e = w(t.padding, 8); this.options = t, this.chart.styledMode || (this.itemStyle = t.itemStyle, this.itemHiddenStyle = M(this.itemStyle, t.itemHiddenStyle)), this.itemMarginTop = t.itemMarginTop, this.itemMarginBottom = t.itemMarginBottom, this.padding = e, this.initialItemY = e - 5, this.symbolWidth = w(t.symbolWidth, 16), this.pages = [], this.proximate = "proximate" === t.layout && !this.chart.inverted, this.baseline = void 0 } update(t, e) { let i = this.chart; this.setOptions(M(!0, this.options, t)), "events" in this.options && n(this, this.options), this.destroy(), i.isDirtyLegend = i.isDirtyBox = !0, w(e, !0) && i.redraw(), k(this, "afterUpdate", { redraw: e }) } colorizeItem(n, h) { let { area: l, group: t, label: d, line: c, symbol: p } = n.legendItem || {}; if (t?.[h ? "removeClass" : "addClass"]("highcharts-legend-item-hidden"), !this.chart.styledMode) { let { itemHiddenStyle: t = {} } = this, e = t.color, { fillColor: i, fillOpacity: s, lineColor: r, marker: o } = n.options, a = t => (h || (t.fill && (t.fill = e), t.stroke && (t.stroke = e)), t); d?.css(M(h ? this.itemStyle : t)), c?.attr(a({ stroke: r || n.color })), p && p.attr(a(o && p.isMarker ? n.pointAttribs() : { fill: n.color })), l?.attr(a({ fill: i || n.color, "fill-opacity": i ? 1 : s ?? .75 })) } k(this, "afterColorizeItem", { item: n, visible: h }) } positionItems() { this.allItems.forEach(this.positionItem, this), this.chart.isResizing || this.positionCheckboxes() } positionItem(t) { let { group: e, x: i = 0, y: s = 0 } = t.legendItem || {}, r = this.options, o = r.symbolPadding, a = !r.rtl, n = t.checkbox; var h; e && e.element && (h = { translateX: a ? i : this.legendWidth - i - 2 * o - 4, translateY: s }, e[b(e.translateY) ? "animate" : "attr"](h, void 0, () => { k(this, "afterPositionItem", { item: t }) })), n && (n.x = i, n.y = s) } destroyItem(t) { let e = t.checkbox, i = t.legendItem || {}; for (var s of ["group", "label", "line", "symbol"]) i[s] && (i[s] = i[s].destroy()); e && v(e), t.legendItem = void 0 } destroy() { for (var t of this.getAllItems()) this.destroyItem(t); for (var e of ["clipRect", "up", "down", "pager", "nav", "box", "title", "group"]) this[e] && (this[e] = this[e].destroy()); this.display = null } positionCheckboxes() { let s, r = this.group && this.group.alignAttr, o = this.clipHeight || this.legendHeight, a = this.titleHeight; r && (s = r.translateY, this.allItems.forEach(function (t) { var e, i = t.checkbox; i && (e = s + a + i.y + (this.scrollOffset || 0) + 3, y(i, { left: r.translateX + t.checkboxOffset + i.x - 20 + "px", top: e + "px", display: this.proximate || e > s - 6 && e < s + o - 6 ? "" : "none" })) }, this)) } renderTitle() { let t = this.options, e = this.padding, i = t.title, s, r = 0; i.text && (this.title || (this.title = this.chart.renderer.label(i.text, e - 3, e - 4, void 0, void 0, void 0, t.useHTML, void 0, "legend-title").attr({ zIndex: 1 }), this.chart.styledMode || this.title.css(i.style), this.title.add(this.group)), i.width || this.title.css({ width: this.maxLegendWidth + "px" }), r = (s = this.title.getBBox()).height, this.offsetWidth = s.width, this.contentGroup.attr({ translateY: r })), this.titleHeight = r } setText(t) { let e = this.options; t.legendItem.label.attr({ text: e.labelFormat ? f(e.labelFormat, t, this.chart) : e.labelFormatter.call(t) }) } renderItem(t) { let e = t.legendItem = t.legendItem || {}, i = this.chart, s = i.renderer, r = this.options, o = "horizontal" === r.layout, a = this.symbolWidth, n = r.symbolPadding || 0, h = this.itemStyle, l = this.itemHiddenStyle, d = o ? w(r.itemDistance, 20) : 0, c = !r.rtl, p = !t.series, u = !p && t.series.drawLegendSymbol ? t.series : t, g = u.options, f = !!this.createCheckboxForItem && g && g.showCheckbox, m = r.useHTML, x = t.options.className, y = e.label, b = a + n + d + (f ? 20 : 0); y || (e.group = s.g("legend-item").addClass("highcharts-" + u.type + "-series highcharts-color-" + t.colorIndex + (x ? " " + x : "") + (p ? " highcharts-series-" + t.index : "")).attr({ zIndex: 1 }).add(this.scrollGroup), e.label = y = s.text("", c ? a + n : -n, this.baseline || 0, m), i.styledMode || y.css(M(t.visible ? h : l)), y.attr({ align: c ? "left" : "right", zIndex: 2 }).add(e.group), this.baseline || (this.fontMetrics = s.fontMetrics(y), this.baseline = this.fontMetrics.f + 3 + this.itemMarginTop, y.attr("y", this.baseline), this.symbolHeight = w(r.symbolHeight, this.fontMetrics.f), r.squareSymbol && (this.symbolWidth = w(r.symbolWidth, Math.max(this.symbolHeight, 16)), b = this.symbolWidth + n + d + (f ? 20 : 0), c && y.attr("x", this.symbolWidth + n))), u.drawLegendSymbol(this, t), this.setItemEvents && this.setItemEvents(t, y, m)), f && !t.checkbox && this.createCheckboxForItem && this.createCheckboxForItem(t), this.colorizeItem(t, t.visible), !i.styledMode && h.width || y.css({ width: (r.itemWidth || this.widthOption || i.spacingBox.width) - b + "px" }), this.setText(t); var v = y.getBBox(), S = this.fontMetrics && this.fontMetrics.h || 0; t.itemWidth = t.checkboxOffset = r.itemWidth || e.labelWidth || v.width + b, this.maxItemWidth = Math.max(this.maxItemWidth, t.itemWidth), this.totalItemWidth += t.itemWidth, this.itemHeight = t.itemHeight = Math.round(e.labelHeight || (v.height > 1.5 * S ? v.height : S)) } layoutItem(t) { let e = this.options, i = this.padding, s = "horizontal" === e.layout, r = t.itemHeight, o = this.itemMarginBottom, a = this.itemMarginTop, n = s ? w(e.itemDistance, 20) : 0, h = this.maxLegendWidth, l = e.alignColumns && this.totalItemWidth > h ? this.maxItemWidth : t.itemWidth, d = t.legendItem || {}; s && this.itemX - i + l > h && (this.itemX = i, this.lastLineHeight && (this.itemY += a + this.lastLineHeight + o), this.lastLineHeight = 0), this.lastItemY = a + this.itemY + o, this.lastLineHeight = Math.max(r, this.lastLineHeight), d.x = this.itemX, d.y = this.itemY, s ? this.itemX += l : (this.itemY += a + r + o, this.lastLineHeight = r), this.offsetWidth = this.widthOption || Math.max((s ? this.itemX - i - (t.checkbox ? 0 : n) : l) + i, this.offsetWidth) } getAllItems() { let i = []; return this.chart.series.forEach(function (t) { var e = t && t.options; t && w(e.showInLegend, !b(e.linkedTo) && void 0, !0) && (i = i.concat((t.legendItem || {}).labels || ("point" === e.legendType ? t.data : t))) }), k(this, "afterGetAllItems", { allItems: i }), i } getAlignment() { let t = this.options; return this.proximate ? t.align.charAt(0) + "tv" : t.floating ? "" : t.align.charAt(0) + t.verticalAlign.charAt(0) + t.layout.charAt(0) } adjustMargins(i, s) { let r = this.chart, o = this.options, a = this.getAlignment(); a && [/(lth|ct|rth)/, /(rtv|rm|rbv)/, /(rbh|cb|lbh)/, /(lbv|lm|ltv)/].forEach(function (t, e) { t.test(a) && !b(i[e]) && (r[l[e]] = Math.max(r[l[e]], r.legend[(e + 1) % 2 ? "legendHeight" : "legendWidth"] + [1, -1, -1, 1][e] * o[e % 2 ? "x" : "y"] + w(o.margin, 12) + s[e] + (r.titleOffset[e] || 0))) }) } proximatePositions() { let t, a = this.chart, n = [], h = "left" === this.options.align; for (var e of (this.allItems.forEach(function (t) { let e, i, s = h, r, o; t.yAxis && (t.xAxis.options.reversed && (s = !s), t.points && (e = S(s ? t.points : t.points.slice(0).reverse(), function (t) { return C(t.plotY) })), i = this.itemMarginTop + t.legendItem.label.getBBox().height + this.itemMarginBottom, o = t.yAxis.top - a.plotTop, r = t.visible ? (e ? e.plotY : t.yAxis.height) + (o - .3 * i) : o + t.yAxis.height, n.push({ target: r, size: i, item: t })) }, this), p(n, a.plotHeight))) t = e.item.legendItem || {}, C(e.pos) && (t.y = a.plotTop - a.spacing[0] + e.pos) } render() { let t = this.chart, e = t.renderer, i = this.options, s = this.padding, r = this.getAllItems(), o, a, n, h = this.group, l, d = this.box; this.itemX = s, this.itemY = this.initialItemY, this.offsetWidth = 0, this.lastItemY = 0, this.widthOption = T(i.width, t.spacingBox.width - s), l = t.spacingBox.width - 2 * s - i.x, -1 < ["rm", "lm"].indexOf(this.getAlignment().substring(0, 2)) && (l /= 2), this.maxLegendWidth = this.widthOption || l, h || (this.group = h = e.g("legend").addClass(i.className || "").attr({ zIndex: 7 }).add(), this.contentGroup = e.g().attr({ zIndex: 1 }).add(h), this.scrollGroup = e.g().add(this.contentGroup)), this.renderTitle(), P(r, (t, e) => (t.options && t.options.legendIndex || 0) - (e.options && e.options.legendIndex || 0)), i.reversed && r.reverse(), this.allItems = r, this.display = o = !!r.length, this.lastLineHeight = 0, this.maxItemWidth = 0, this.totalItemWidth = 0, this.itemHeight = 0, r.forEach(this.renderItem, this), r.forEach(this.layoutItem, this), a = (this.widthOption || this.offsetWidth) + s, n = this.lastItemY + this.lastLineHeight + this.titleHeight, n = this.handleOverflow(n) + s, d || (this.box = d = e.rect().addClass("highcharts-legend-box").attr({ r: i.borderRadius }).add(h)), t.styledMode || d.attr({ stroke: i.borderColor, "stroke-width": i.borderWidth || 0, fill: i.backgroundColor || "none" }).shadow(i.shadow), 0 < a && 0 < n && d[d.placed ? "animate" : "attr"](d.crisp.call({}, { x: 0, y: 0, width: a, height: n }, d.strokeWidth())), h[o ? "show" : "hide"](), t.styledMode && "none" === h.getStyle("display") && (a = n = 0), this.legendWidth = a, this.legendHeight = n, o && this.align(), this.proximate || this.positionItems(), k(this, "afterRender") } align(t = this.chart.spacingBox) { let e = this.chart, i = this.options, s = t.y; /(lth|ct|rth)/.test(this.getAlignment()) && 0 < e.titleOffset[0] ? s += e.titleOffset[0] : /(lbh|cb|rbh)/.test(this.getAlignment()) && 0 < e.titleOffset[2] && (s -= e.titleOffset[2]), s !== t.y && (t = M(t, { y: s })), e.hasRendered || (this.group.placed = !1), this.group.align(M(i, { width: this.legendWidth, height: this.legendHeight, verticalAlign: this.proximate ? "top" : i.verticalAlign }), !0, t) } handleOverflow(t) { function e(t) { "number" == typeof t ? S.attr({ height: t }) : S && (s.clipRect = S.destroy(), s.contentGroup.clip()), s.contentGroup.div && (s.contentGroup.div.style.clip = t ? "rect(" + l + "px,9999px," + (l + t) + "px,0)" : "auto") } function i(t) { return s[t] = o.circle(0, 0, 1.3 * u).translate(u / 2, u / 2).add(v), r.styledMode || s[t].attr("fill", "rgba(0,0,0,0.0001)"), s[t] } let s = this, r = this.chart, o = r.renderer, a = this.options, n = a.y, h = "top" === a.verticalAlign, l = this.padding, d = a.maxHeight, c = a.navigation, p = w(c.animation, !0), u = c.arrowSize || 12, g = this.pages, f = this.allItems, m, x, y, b = r.spacingBox.height + (h ? -n : n) - l, v = this.nav, S = this.clipRect; return "horizontal" !== a.layout || "middle" === a.verticalAlign || a.floating || (b /= 2), d && (b = Math.min(b, d)), g.length = 0, t && 0 < b && t > b && !1 !== c.enabled ? (this.clipHeight = m = Math.max(b - 20 - this.titleHeight - l, 0), this.currentPage = w(this.currentPage, 1), this.fullHeight = t, f.forEach((t, e) => { let i = (y = t.legendItem || {}).y || 0, s = Math.round(y.label.getBBox().height), r = g.length; (!r || i - g[r - 1] > m && (x || i) !== g[r - 1]) && (g.push(x || i), r++), y.pageIx = r - 1, x && ((f[e - 1].legendItem || {}).pageIx = r - 1), e === f.length - 1 && i + s - g[r - 1] > m && i > g[r - 1] && (g.push(i), y.pageIx = r), i !== x && (x = i) }), S || (S = s.clipRect = o.clipRect(0, l - 2, 9999, 0), s.contentGroup.clip(S)), e(m), v || (this.nav = v = o.g().attr({ zIndex: 1 }).add(this.group), this.up = o.symbol("triangle", 0, 0, u, u).add(v), i("upTracker").on("click", function () { s.scroll(-1, p) }), this.pager = o.text("", 15, 10).addClass("highcharts-legend-navigation"), !r.styledMode && c.style && this.pager.css(c.style), this.pager.add(v), this.down = o.symbol("triangle-down", 0, 0, u, u).add(v), i("downTracker").on("click", function () { s.scroll(1, p) })), s.scroll(0), t = b) : v && (e(), this.nav = v.destroy(), this.scrollGroup.attr({ translateY: 1 }), this.clipHeight = 0), t } scroll(t, e) { let i = this.chart, s = this.pages, r = s.length, o = this.clipHeight, a = this.options.navigation, n = this.pager, h = this.padding, l = this.currentPage + t; 0 < (l = l > r ? r : l) && (void 0 !== e && c(e, i), this.nav.attr({ translateX: h, translateY: o + this.padding + 7 + this.titleHeight, visibility: "inherit" }), [this.up, this.upTracker].forEach(function (t) { t.attr({ class: 1 === l ? "highcharts-legend-nav-inactive" : "highcharts-legend-nav-active" }) }), n.attr({ text: l + "/" + r }), [this.down, this.downTracker].forEach(function (t) { t.attr({ x: 18 + this.pager.getBBox().width, class: l === r ? "highcharts-legend-nav-inactive" : "highcharts-legend-nav-active" }) }, this), i.styledMode || (this.up.attr({ fill: 1 === l ? a.inactiveColor : a.activeColor }), this.upTracker.css({ cursor: 1 === l ? "default" : "pointer" }), this.down.attr({ fill: l === r ? a.inactiveColor : a.activeColor }), this.downTracker.css({ cursor: l === r ? "default" : "pointer" })), this.scrollOffset = -s[l - 1] + this.initialItemY, this.scrollGroup.animate({ translateY: this.scrollOffset }), this.currentPage = l, this.positionCheckboxes(), L(() => { k(this, "afterScroll", { currentPage: l }) }, d(w(e, i.renderer.globalAnimation, !0)).duration)) } setItemEvents(i, t, e) { let s = this, r = i.legendItem || {}, o = s.chart.renderer.boxWrapper, a = i instanceof g, n = i instanceof u, h = "highcharts-legend-" + (a ? "point" : "series") + "-active", l = s.chart.styledMode, d = e ? [t, r.symbol] : [r.group], c = e => { s.allItems.forEach(t => { i !== t && [t].concat(t.linkedSeries || []).forEach(t => { t.setState(e, !a) }) }) }; for (var p of d) p && p.on("mouseover", function () { i.visible && c("inactive"), i.setState("hover"), i.visible && o.addClass(h), l || t.css(s.options.itemHoverStyle) }).on("mouseout", function () { s.chart.styledMode || t.css(M(i.visible ? s.itemStyle : s.itemHiddenStyle)), c(""), o.removeClass(h), i.setState() }).on("click", function (t) { o.removeClass(h), k(s, "itemClick", { browserEvent: t, legendItem: i }, function () { i.setVisible && i.setVisible(), c(i.visible ? "inactive" : "") }), a ? i.firePointEvent("legendItemClick", { browserEvent: t }) : n && k(i, "legendItemClick", { browserEvent: t }) }) } createCheckboxForItem(e) { e.checkbox = x("input", { type: "checkbox", className: "highcharts-legend-checkbox", checked: e.selected, defaultChecked: e.selected }, this.options.itemCheckboxStyle, this.chart.container), m(e.checkbox, "click", function (t) { t = t.target; k(e.series || e, "checkboxClick", { checked: t.checked, item: e }, function () { e.select() }) }) } } return (a = O = O || {}).compose = function (t) { A(h, "Core.Legend") && m(t, "beforeMargins", function () { this.legend = new a(this, this.options.legend) }) }, O }), e(t, "Core/Chart/Chart.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Axis/Axis.js"], t["Core/Defaults.js"], t["Core/Templating.js"], t["Core/Foundation.js"], t["Core/Globals.js"], t["Core/Renderer/RendererRegistry.js"], t["Core/Series/Series.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Renderer/SVG/SVGRenderer.js"], t["Core/Time.js"], t["Core/Utilities.js"], t["Core/Renderer/HTML/AST.js"], t["Core/Axis/Tick.js"]], function (t, s, e, i, z, a, N, W, G, H, u, r, c, X) { let { animate: n, animObject: F, setAnimation: f } = t, { defaultOptions: h, defaultTime: Y } = e, U = i["numberFormat"], g = z["registerEventOptions"], { charts: p, doc: m, marginNames: l, svg: V, win: o } = a, d = G["seriesTypes"], { addEvent: x, attr: y, createElement: b, css: v, defined: M, diffObjects: S, discardElement: q, erase: _, error: k, extend: w, find: C, fireEvent: A, getStyle: T, isArray: Z, isNumber: P, isObject: K, isString: L, merge: O, objectEach: E, pick: D, pInt: $, relativeLength: j, removeEvent: I, splat: B, syncTimeout: J, uniqueKey: Q } = r; class R { static chart(t, e, i) { return new R(t, e, i) } constructor(t, e, i) { this.sharedClips = {}; let s = [...arguments]; (L(t) || t.nodeName) && (this.renderTo = s.shift()), this.init(s[0], s[1]) } setZoomOptions() { var t = this.options.chart, e = t.zooming; this.zooming = { ...e, type: D(t.zoomType, e.type), key: D(t.zoomKey, e.key), pinchType: D(t.pinchType, e.pinchType), singleTouch: D(t.zoomBySingleTouch, e.singleTouch, !1), resetButton: O(e.resetButton, t.resetZoomButton) } } init(i, s) { A(this, "init", { args: arguments }, function () { var t = O(h, i), e = t.chart; this.userOptions = w({}, i), this.margin = [], this.spacing = [], this.labelCollectors = [], this.callback = s, this.isResizing = 0, this.options = t, this.axes = [], this.series = [], this.time = i.time && Object.keys(i.time).length ? new u(i.time) : a.time, this.numberFormatter = e.numberFormatter || U, this.styledMode = e.styledMode, this.hasCartesianSeries = e.showAxes, this.index = p.length, p.push(this), a.chartCount++, g(this, e), this.xAxis = [], this.yAxis = [], this.pointCount = this.colorCounter = this.symbolCounter = 0, this.setZoomOptions(), A(this, "afterInit"), this.firstRender() }) } initSeries(t) { let e = this.options.chart, i = t.type || e.type, s = d[i], r = (s || k(17, !0, this, { missingModuleFor: i }), new s); return "function" == typeof r.init && r.init(this, t), r } setSortedData() { this.getSeriesOrderByLinks().forEach(function (t) { t.points || t.data || !t.enabledDataSorting || t.setData(t.options.data, !1) }) } getSeriesOrderByLinks() { return this.series.concat().sort(function (t, e) { return t.linkedSeries.length || e.linkedSeries.length ? e.linkedSeries.length - t.linkedSeries.length : 0 }) } orderItems(t, i = 0) { let s = this[t], r = this.options[t] = B(this.options[t]).slice(), o = this.userOptions[t] = this.userOptions[t] ? B(this.userOptions[t]).slice() : []; if (this.hasRendered && (r.splice(i), o.splice(i)), s) for (let e = i, t = s.length; e < t; ++e) { let t = s[e]; t && (t.index = e, t instanceof W && (t.name = t.getName()), t.options.isInternal || (r[e] = t.options, o[e] = t.userOptions)) } } isInsidePlot(t, e, i = {}) { let { inverted: s, plotBox: r, plotLeft: o, plotTop: a, scrollablePlotBox: n } = this, { scrollLeft: h = 0, scrollTop: l = 0 } = i.visiblePlotOnly && this.scrollablePlotArea?.scrollingContainer || {}, d = i.series, c = i.visiblePlotOnly && n || r, p = i.inverted ? e : t, u = i.inverted ? t : e, g = { x: p, y: u, isInsidePlot: !0, options: i }; return i.ignoreX || (t = d && (s && !this.polar ? d.yAxis : d.xAxis) || { pos: o, len: 1 / 0 }, (e = i.paneCoordinates ? t.pos + p : o + p) >= Math.max(h + o, t.pos) && e <= Math.min(h + o + c.width, t.pos + t.len) || (g.isInsidePlot = !1)), !i.ignoreY && g.isInsidePlot && (e = !s && i.axis && !i.axis.isXAxis && i.axis || d && (s ? d.xAxis : d.yAxis) || { pos: a, len: 1 / 0 }, (t = i.paneCoordinates ? e.pos + u : a + u) >= Math.max(l + a, e.pos) && t <= Math.min(l + a + c.height, e.pos + e.len) || (g.isInsidePlot = !1)), A(this, "afterIsInsidePlot", g), g.isInsidePlot } redraw(t) { A(this, "beforeRedraw"); let e = this.hasCartesianSeries ? this.axes : this.colorAxis || [], i = this.series, s = this.pointer, r = this.legend, o = this.userOptions.legend, a = this.renderer, n = a.isHidden(), h = [], l, d, c, p = this.isDirtyBox, u = this.isDirtyLegend, g; for (a.rootFontSize = a.boxWrapper.getStyle("font-size"), this.setResponsive && this.setResponsive(!1), f(!!this.hasRendered && t, this), n && this.temporaryDisplay(), this.layOutTitles(!1), c = i.length; c--;)if (((g = i[c]).options.stacking || g.options.centerInCategory) && (d = !0, g.isDirty)) { l = !0; break } if (l) for (c = i.length; c--;)(g = i[c]).options.stacking && (g.isDirty = !0); i.forEach(function (t) { t.isDirty && ("point" === t.options.legendType ? ("function" == typeof t.updateTotals && t.updateTotals(), u = !0) : o && (o.labelFormatter || o.labelFormat) && (u = !0)), t.isDirtyData && A(t, "updatedData") }), u && r && r.options.enabled && (r.render(), this.isDirtyLegend = !1), d && this.getStacks(), e.forEach(function (t) { t.updateNames(), t.setScale() }), this.getMargins(), e.forEach(function (t) { t.isDirty && (p = !0) }), e.forEach(function (t) { var e = t.min + "," + t.max; t.extKey !== e && (t.extKey = e, h.push(function () { A(t, "afterSetExtremes", w(t.eventArgs, t.getExtremes())), delete t.eventArgs })), (p || d) && t.redraw() }), p && this.drawChartBox(), A(this, "predraw"), i.forEach(function (t) { (p || t.isDirty) && t.visible && t.redraw(), t.isDirtyData = !1 }), s && s.reset(!0), a.draw(), A(this, "redraw"), A(this, "render"), n && this.temporaryDisplay(!0), h.forEach(function (t) { t.call() }) } get(e) { var i = this.series; function s(t) { return t.id === e || t.options && t.options.id === e } let r = C(this.axes, s) || C(this.series, s); for (let t = 0; !r && t < i.length; t++)r = C(i[t].points || [], s); return r } getAxes() { let t = this.userOptions; for (var e of (A(this, "getAxes"), ["xAxis", "yAxis"])) for (var i of t[e] = B(t[e] || {})) new s(this, i, e); A(this, "afterGetAxes") } getSelectedPoints() { return this.series.reduce((e, t) => (t.getPointsCollection().forEach(t => { D(t.selectedStaging, t.selected) && e.push(t) }), e), []) } getSelectedSeries() { return this.series.filter(function (t) { return t.selected }) } setTitle(t, e, i) { this.applyDescription("title", t), this.applyDescription("subtitle", e), this.applyDescription("caption", void 0), this.layOutTitles(i) } applyDescription(i, t) { let s = this, e = this.options[i] = O(this.options[i], t), r = this[i]; r && t && (this[i] = r = r.destroy()), e && !r && ((r = this.renderer.text(e.text, 0, 0, e.useHTML).attr({ align: e.align, class: "highcharts-" + i, zIndex: e.zIndex || 4 }).add()).update = function (t, e) { s.applyDescription(i, t), s.layOutTitles(e) }, this.styledMode || r.css(w("title" === i ? { fontSize: this.options.isStock ? "1em" : "1.2em" } : {}, e.style)), this[i] = r) } layOutTitles(t = !0) { let a = [0, 0, 0], n = this.renderer, h = this.spacingBox;["title", "subtitle", "caption"].forEach(function (t) { let e = this[t], i = this.options[t], s = i.verticalAlign || "top", r = "title" === t ? "top" === s ? -3 : 0 : "top" === s ? a[0] + 2 : 0; var o; e && (e.css({ width: (i.width || h.width + (i.widthAdjust || 0)) + "px" }), t = n.fontMetrics(e).b, o = Math.round(e.getBBox(i.useHTML).height), e.align(w({ y: "bottom" === s ? t : r + t, height: o }, i), !1, "spacingBox"), i.floating || ("top" === s ? a[0] = Math.ceil(a[0] + o) : "bottom" === s && (a[2] = Math.ceil(a[2] + o)))) }, this), a[0] && "top" === (this.options.title.verticalAlign || "top") && (a[0] += this.options.title.margin), a[2] && "bottom" === this.options.caption.verticalAlign && (a[2] += this.options.caption.margin); var e = !this.titleOffset || this.titleOffset.join(",") !== a.join(","); this.titleOffset = a, A(this, "afterLayOutTitles"), !this.isDirtyBox && e && (this.isDirtyBox = this.isDirtyLegend = e, this.hasRendered && t && this.isDirtyBox && this.redraw()) } getContainerBox() { let t = [].map.call(this.renderTo.children, t => { var e; if (t !== this.container) return e = t.style.display, t.style.display = "none", [t, e] }), e = { width: T(this.renderTo, "width", !0) || 0, height: T(this.renderTo, "height", !0) || 0 }; return t.filter(Boolean).forEach(([t, e]) => { t.style.display = e }), e } getChartSize() { var t = this.options.chart, e = t.width, t = t.height, i = this.getContainerBox(), s = 1 < i.height && !(!this.renderTo.parentElement?.style.height && "100%" === this.renderTo.style.height); this.chartWidth = Math.max(0, e || i.width || 600), this.chartHeight = Math.max(0, j(t, this.chartWidth) || (s ? i.height : 400)), this.containerBox = i } temporaryDisplay(t) { let e = this.renderTo, i; if (t) for (; e && e.style;)e.hcOrigStyle && (v(e, e.hcOrigStyle), delete e.hcOrigStyle), e.hcOrigDetached && (m.body.removeChild(e), e.hcOrigDetached = !1), e = e.parentNode; else for (; e && e.style && (m.body.contains(e) || e.parentNode || (e.hcOrigDetached = !0, m.body.appendChild(e)), "none" !== T(e, "display", !1) && !e.hcOricDetached || (e.hcOrigStyle = { display: e.style.display, height: e.style.height, overflow: e.style.overflow }, i = { display: "block", overflow: "hidden" }, e !== this.renderTo && (i.height = 0), v(e, i), e.offsetWidth || e.style.setProperty("display", "block", "important")), (e = e.parentNode) !== m.body);); } setClassName(t) { this.container.className = "highcharts-container " + (t || "") } getContainer() { let t = this.options, e = t.chart, i = "data-highcharts-chart", s = Q(), r, o = this.renderTo; o || (this.renderTo = o = e.renderTo), L(o) && (this.renderTo = o = m.getElementById(o)), o || k(13, !0, this); var a = $(y(o, i)); P(a) && p[a] && p[a].hasRendered && p[a].destroy(), y(o, i, this.index), o.innerHTML = c.emptyHTML, e.skipClone || o.offsetWidth || this.temporaryDisplay(), this.getChartSize(); let n = this.chartHeight, h = this.chartWidth; v(o, { overflow: "hidden" }), this.styledMode || (r = w({ position: "relative", overflow: "hidden", width: h + "px", height: n + "px", textAlign: "left", lineHeight: "normal", zIndex: 0, "-webkit-tap-highlight-color": "rgba(0,0,0,0)", userSelect: "none", "touch-action": "manipulation", outline: "none", padding: "0px" }, e.style || {})); a = b("div", { id: s }, r, o); this.container = a, this.getChartSize(), h !== this.chartWidth && (h = this.chartWidth, this.styledMode || v(a, { width: D(e.style?.width, h + "px") })), this.containerBox = this.getContainerBox(), this._cursor = a.style.cursor; let l = e.renderer || !V ? N.getRendererType(e.renderer) : H; if (this.renderer = new l(a, h, n, void 0, e.forExport, t.exporting && t.exporting.allowHTML, this.styledMode), f(void 0, this), this.setClassName(e.className), this.styledMode) for (var d in t.defs) this.renderer.definition(t.defs[d]); else this.renderer.setStyle(e.style); this.renderer.chartIndex = this.index, A(this, "afterGetContainer") } getMargins(t) { var { spacing: e, margin: i, titleOffset: s } = this; this.resetMargins(), s[0] && !M(i[0]) && (this.plotTop = Math.max(this.plotTop, s[0] + e[0])), s[2] && !M(i[2]) && (this.marginBottom = Math.max(this.marginBottom, s[2] + e[2])), this.legend && this.legend.display && this.legend.adjustMargins(i, e), A(this, "getMargins"), t || this.getAxisMargins() } getAxisMargins() { function t(t) { t.forEach(function (t) { t.visible && t.getOffset() }) } let i = this, s = i.axisOffset = [0, 0, 0, 0], e = i.colorAxis, r = i.margin; i.hasCartesianSeries ? t(i.axes) : e && e.length && t(e), l.forEach(function (t, e) { M(r[e]) || (i[t] += s[e]) }), i.setChartSize() } getOptions() { return S(this.userOptions, h) } reflow(t) { let e = this, i = e.containerBox, s = e.getContainerBox(); delete e.pointer?.chartPosition, !e.isPrinting && !e.isResizing && i && s.width && (s.width === i.width && s.height === i.height || (r.clearTimeout(e.reflowTimeout), e.reflowTimeout = J(function () { e.container && e.setSize(void 0, void 0, !1) }, t ? 100 : 0)), e.containerBox = s) } setReflow() { let e = this, t = t => { e.options?.chart.reflow && e.hasLoaded && e.reflow(t) }; var i; "function" == typeof ResizeObserver ? new ResizeObserver(t).observe(e.renderTo) : (i = x(o, "resize", t), x(this, "destroy", i)) } setSize(t, e, i) { let s = this, r = s.renderer; s.isResizing += 1, f(i, s); var i = r.globalAnimation, { chartWidth: t, chartHeight: e, scrollablePixelsX: o = 0, scrollablePixelsY: a = 0 } = (s.oldChartHeight = s.chartHeight, s.oldChartWidth = s.chartWidth, void 0 !== t && (s.options.chart.width = t), void 0 !== e && (s.options.chart.height = e), s.getChartSize(), s); !s.isDirtyBox && t === s.oldChartWidth && e === s.oldChartHeight || (s.styledMode || (i ? n : v)(s.container, { width: t + o + "px", height: e + a + "px" }, i), s.setChartSize(!0), r.setSize(t, e, i), s.axes.forEach(function (t) { t.isDirty = !0, t.setScale() }), s.isDirtyLegend = !0, s.isDirtyBox = !0, s.layOutTitles(), s.getMargins(), s.redraw(i), s.oldChartHeight = void 0, A(s, "resize"), setTimeout(() => { s && A(s, "endResize") }, F(i).duration)), --s.isResizing } setChartSize(t) { var e, i, s, r; let { chartHeight: o, chartWidth: a, inverted: n, spacing: h, renderer: l } = this, d = this.clipOffset, c = Math[n ? "floor" : "round"]; this.plotLeft = e = Math.round(this.plotLeft), this.plotTop = i = Math.round(this.plotTop), this.plotWidth = s = Math.max(0, Math.round(a - e - this.marginRight)), this.plotHeight = r = Math.max(0, Math.round(o - i - this.marginBottom)), this.plotSizeX = n ? r : s, this.plotSizeY = n ? s : r, this.spacingBox = l.spacingBox = { x: h[3], y: h[0], width: a - h[3] - h[1], height: o - h[0] - h[2] }, this.plotBox = l.plotBox = { x: e, y: i, width: s, height: r }, d && (this.clipBox = { x: c(d[3]), y: c(d[0]), width: c(this.plotSizeX - d[1] - d[3]), height: c(this.plotSizeY - d[0] - d[2]) }), t || (this.axes.forEach(function (t) { t.setAxisSize(), t.setAxisTranslation() }), l.alignElements()), A(this, "afterSetChartSize", { skipAxes: t }) } resetMargins() { A(this, "resetMargins"); let r = this, o = r.options.chart, t = o.plotBorderWidth || 0, e = t / 2;["margin", "spacing"].forEach(function (i) { let t = o[i], s = K(t) ? t : [t, t, t, t];["Top", "Right", "Bottom", "Left"].forEach(function (t, e) { r[i][e] = D(o[i + t], s[e]) }) }), l.forEach(function (t, e) { r[t] = D(r.margin[e], r.spacing[e]) }), r.axisOffset = [0, 0, 0, 0], r.clipOffset = [e, e, e, e], r.plotBorderWidth = t } drawChartBox() { let t = this.options.chart, e = this.renderer, i = this.chartWidth, s = this.chartHeight, r = this.styledMode, o = this.plotBGImage, a = t.backgroundColor, n = t.plotBackgroundColor, h = t.plotBackgroundImage, l = this.plotLeft, d = this.plotTop, c = this.plotWidth, p = this.plotHeight, u = this.plotBox, g = this.clipRect, f = this.clipBox, m = this.chartBackground, x = this.plotBackground, y = this.plotBorder, b, v, S, k = "animate"; m || (this.chartBackground = m = e.rect().addClass("highcharts-background").add(), k = "attr"), r ? b = v = m.strokeWidth() : (v = (b = t.borderWidth || 0) + (t.shadow ? 8 : 0), S = { fill: a || "none" }, (b || m["stroke-width"]) && (S.stroke = t.borderColor, S["stroke-width"] = b), m.attr(S).shadow(t.shadow)), m[k]({ x: v / 2, y: v / 2, width: i - v - b % 2, height: s - v - b % 2, r: t.borderRadius }), k = "animate", x || (k = "attr", this.plotBackground = x = e.rect().addClass("highcharts-plot-background").add()), x[k](u), r || (x.attr({ fill: n || "none" }).shadow(t.plotShadow), h && (o ? (h !== o.attr("href") && o.attr("href", h), o.animate(u)) : this.plotBGImage = e.image(h, l, d, c, p).add())), g ? g.animate({ width: f.width, height: f.height }) : this.clipRect = e.clipRect(f), k = "animate", y || (k = "attr", this.plotBorder = y = e.rect().addClass("highcharts-plot-border").attr({ zIndex: 1 }).add()), r || y.attr({ stroke: t.plotBorderColor, "stroke-width": t.plotBorderWidth || 0, fill: "none" }), y[k](y.crisp({ x: l, y: d, width: c, height: p }, -y.strokeWidth())), this.isDirtyBox = !1, A(this, "afterDrawChartBox") } propFromSeries() { let e, i, s, r = this, o = r.options.chart, a = r.options.series;["inverted", "angular", "polar"].forEach(function (t) { for (i = d[o.type], s = o[t] || i && i.prototype[t], e = a && a.length; !s && e--;)(i = d[a[e].type]) && i.prototype[t] && (s = !0); r[t] = s }) } linkSeries(t) { let s = this, e = s.series; e.forEach(function (t) { t.linkedSeries.length = 0 }), e.forEach(function (e) { var i = e.options["linkedTo"]; if (L(i)) { let t; (t = ":previous" === i ? s.series[e.index - 1] : s.get(i)) && t.linkedParent !== e && (t.linkedSeries.push(e), (e.linkedParent = t).enabledDataSorting && e.setDataSortingOptions(), e.visible = D(e.options.visible, t.options.visible, e.visible)) } }), A(this, "afterLinkSeries", { isUpdating: t }) } renderSeries() { this.series.forEach(function (t) { t.translate(), t.render() }) } render() { let t = this.axes, e = this.colorAxis, i = this.renderer, s = this.options.chart.axisLayoutRuns || 2, r = t => { t.forEach(t => { t.visible && t.render() }) }, o = 0, a = !0, n, h = 0; for (var l of (this.setTitle(), A(this, "beforeMargins"), this.getStacks?.(), this.getMargins(!0), this.setChartSize(), t)) { var d = l["options"], c = d["labels"]; if (this.hasCartesianSeries && l.horiz && l.visible && c.enabled && l.series.length && "colorAxis" !== l.coll && !this.polar) { o = d.tickLength, l.createGroups(); let t = new X(l, 0, "", !0), e = t.createLabel("x", c); if (t.destroy(), o = e && D(c.reserveSpace, !P(d.crossing)) ? e.getBBox().height + c.distance + Math.max(d.offset || 0, 0) : o) { e?.destroy(); break } } } for (this.plotHeight = Math.max(this.plotHeight - o, 0); (a || n || 1 < s) && h < s;) { var p, u = this.plotWidth, g = this.plotHeight; for (p of t) 0 === h ? p.setScale() : (p.horiz && a || !p.horiz && n) && p.setTickInterval(!0); 0 === h ? this.getAxisMargins() : this.getMargins(), a = u / this.plotWidth > (h ? 1 : 1.1), n = g / this.plotHeight > (h ? 1 : 1.05), h++ } this.drawChartBox(), this.hasCartesianSeries ? r(t) : e && e.length && r(e), this.seriesGroup || (this.seriesGroup = i.g("series-group").attr({ zIndex: 3 }).shadow(this.options.chart.seriesGroupShadow).add()), this.renderSeries(), this.addCredits(), this.setResponsive && this.setResponsive(), this.hasRendered = !0 } addCredits(t) { let e = this, i = O(!0, this.options.credits, t); i.enabled && !this.credits && (this.credits = this.renderer.text(i.text + (this.mapCredits || ""), 0, 0).addClass("highcharts-credits").on("click", function () { i.href && (o.location.href = i.href) }).attr({ align: i.position.align, zIndex: 8 }), e.styledMode || this.credits.css(i.style), this.credits.add().align(i.position), this.credits.update = function (t) { e.credits = e.credits.destroy(), e.addCredits(t) }) } destroy() { let t, i = this, e = i.axes, s = i.series, r = i.container, o = r && r.parentNode; for (A(i, "destroy"), i.renderer.forExport ? _(p, i) : p[i.index] = void 0, a.chartCount--, i.renderTo.removeAttribute("data-highcharts-chart"), I(i), t = e.length; t--;)e[t] = e[t].destroy(); for (this.scroller && this.scroller.destroy && this.scroller.destroy(), t = s.length; t--;)s[t] = s[t].destroy();["title", "subtitle", "chartBackground", "plotBackground", "plotBGImage", "plotBorder", "seriesGroup", "clipRect", "credits", "pointer", "rangeSelector", "legend", "resetZoomButton", "tooltip", "renderer"].forEach(function (t) { let e = i[t]; e && e.destroy && (i[t] = e.destroy()) }), r && (r.innerHTML = c.emptyHTML, I(r), o && q(r)), E(i, function (t, e) { delete i[e] }) } firstRender() { let e = this, t = e.options, i = (e.getContainer(), e.resetMargins(), e.setChartSize(), e.propFromSeries(), e.getAxes(), Z(t.series) ? t.series : []); t.series = [], i.forEach(function (t) { e.initSeries(t) }), e.linkSeries(), e.setSortedData(), A(e, "beforeRender"), e.render(), e.pointer?.getChartPosition(), e.renderer.imgCount || e.hasLoaded || e.onload(), e.temporaryDisplay(!0) } onload() { this.callbacks.concat([this.callback]).forEach(function (t) { t && void 0 !== this.index && t.apply(this, [this]) }, this), A(this, "load"), A(this, "render"), M(this.index) && this.setReflow(), this.warnIfA11yModuleNotLoaded(), this.hasLoaded = !0 } warnIfA11yModuleNotLoaded() { let { options: t, title: e } = this; t && !this.accessibility && (this.renderer.boxWrapper.attr({ role: "img", "aria-label": (e && e.element.textContent || "").replace(/</g, "&lt;") }), t.accessibility && !1 === t.accessibility.enabled || k('Highcharts warning: Consider including the "accessibility.js" module to make your chart more usable for people with disabilities. Set the "accessibility.enabled" option to false to remove this warning. See https://www.highcharts.com/docs/accessibility/accessibility-module.', !1, this)) } addSeries(t, e, i) { let s, r = this; return t && (e = D(e, !0), A(r, "addSeries", { options: t }, function () { s = r.initSeries(t), r.isDirtyLegend = !0, r.linkSeries(), s.enabledDataSorting && s.setData(t.data, !1), A(r, "afterAddSeries", { series: s }), e && r.redraw(i) })), s } addAxis(t, e, i, s) { return this.createAxis(e ? "xAxis" : "yAxis", { axis: t, redraw: i, animation: s }) } addColorAxis(t, e, i) { return this.createAxis("colorAxis", { axis: t, redraw: e, animation: i }) } createAxis(t, e) { t = new s(this, e.axis, t); return D(e.redraw, !0) && this.redraw(e.animation), t } showLoading(t) { function e() { o && v(o, { left: i.plotLeft + "px", top: i.plotTop + "px", width: i.plotWidth + "px", height: i.plotHeight + "px" }) } let i = this, s = i.options, r = s.loading, o = i.loadingDiv, a = i.loadingSpan; o || (i.loadingDiv = o = b("div", { className: "highcharts-loading highcharts-loading-hidden" }, null, i.container)), a || (i.loadingSpan = a = b("span", { className: "highcharts-loading-inner" }, null, o), x(i, "redraw", e)), o.className = "highcharts-loading", c.setElementHTML(a, D(t, s.lang.loading, "")), i.styledMode || (v(o, w(r.style, { zIndex: 10 })), v(a, r.labelStyle), i.loadingShown || (v(o, { opacity: 0, display: "" }), n(o, { opacity: r.style.opacity || .5 }, { duration: r.showDuration || 0 }))), i.loadingShown = !0, e() } hideLoading() { let t = this.options, e = this.loadingDiv; e && (e.className = "highcharts-loading highcharts-loading-hidden", this.styledMode || n(e, { opacity: 0 }, { duration: t.loading.hideDuration || 100, complete: function () { v(e, { display: "none" }) } })), this.loadingShown = !1 } update(i, t, o, e) { let s, r, a, n = this, h = { credits: "addCredits", title: "setTitle", subtitle: "setSubtitle", caption: "setCaption" }, l = i.isResponsiveOptions, d = []; A(n, "update", { options: i }), l || n.setResponsive(!1, !0), i = S(i, n.options), n.userOptions = O(n.userOptions, i); var c = i.chart, p = (c && (O(!0, n.options.chart, c), this.setZoomOptions(), "className" in c && n.setClassName(c.className), ("inverted" in c || "polar" in c || "type" in c) && (n.propFromSeries(), s = !0), "alignTicks" in c && (s = !0), "events" in c && g(this, c), E(c, function (t, e) { -1 !== n.propsRequireUpdateSeries.indexOf("chart." + e) && (r = !0), -1 !== n.propsRequireDirtyBox.indexOf(e) && (n.isDirtyBox = !0), -1 !== n.propsRequireReflow.indexOf(e) && (n.isDirtyBox = !0, l || (a = !0)) }), !n.styledMode && c.style && n.renderer.setStyle(n.options.chart.style || {})), !n.styledMode && i.colors && (this.options.colors = i.colors), i.time && (this.time === Y && (this.time = new u(i.time)), O(!0, n.options.time, i.time)), E(i, function (t, e) { n[e] && "function" == typeof n[e].update ? n[e].update(t, !1) : "function" == typeof n[h[e]] ? n[h[e]](t) : "colors" !== e && -1 === n.collectionsWithUpdate.indexOf(e) && O(!0, n.options[e], i[e]), "chart" !== e && -1 !== n.propsRequireUpdateSeries.indexOf(e) && (r = !0) }), this.collectionsWithUpdate.forEach(function (r) { i[r] && (B(i[r]).forEach(function (t, e) { let i; var s = M(t.id); (i = !(i = s ? n.get(t.id) : i) && n[r] && (i = n[r][D(t.index, e)]) && (s && M(i.options.id) || i.options.isInternal) ? void 0 : i) && i.coll === r && (i.update(t, !1), o && (i.touched = !0)), !i && o && n.collectionsWithInit[r] && (n.collectionsWithInit[r][0].apply(n, [t].concat(n.collectionsWithInit[r][1] || []).concat([!1])).touched = !0) }), o && n[r].forEach(function (t) { t.touched || t.options.isInternal ? delete t.touched : d.push(t) })) }), d.forEach(function (t) { t.chart && t.remove && t.remove(!1) }), s && n.axes.forEach(function (t) { t.update({}, !1) }), r && n.getSeriesOrderByLinks().forEach(function (t) { t.chart && t.update({}, !1) }, this), c && c.width), c = c && (L(c.height) ? j(c.height, p || n.chartWidth) : c.height); a || P(p) && p !== n.chartWidth || P(c) && c !== n.chartHeight ? n.setSize(p, c, e) : D(t, !0) && n.redraw(e), A(n, "afterUpdate", { options: i, redraw: t, animation: e }) } setSubtitle(t, e) { this.applyDescription("subtitle", t), this.layOutTitles(e) } setCaption(t, e) { this.applyDescription("caption", t), this.layOutTitles(e) } showResetZoom() { let t = this, e = h.lang, i = t.zooming.resetButton, s = i.theme, r = "chart" === i.relativeTo || "spacingBox" === i.relativeTo ? null : "plotBox"; function o() { t.zoomOut() } A(this, "beforeShowResetZoom", null, function () { t.resetZoomButton = t.renderer.button(e.resetZoom, null, null, o, s).attr({ align: i.position.align, title: e.resetZoomTitle }).addClass("highcharts-reset-zoom").add().align(i.position, !1, r) }), A(this, "afterShowResetZoom") } zoomOut() { A(this, "selection", { resetSelection: !0 }, () => this.transform({ reset: !0, trigger: "zoom" })) } pan(t, e) { let i = this, s = "object" == typeof e ? e : { enabled: e, type: "x" }, r = s.type, o = r && i[{ x: "xAxis", xy: "axes", y: "yAxis" }[r]].filter(t => t.options.panningEnabled && !t.options.isInternal), a = i.options.chart; a?.panning && (a.panning = s), A(this, "pan", { originalEvent: t }, () => { i.transform({ axes: o, event: t, to: { x: t.chartX - (i.mouseDownX || 0), y: t.chartY - (i.mouseDownY || 0) }, trigger: "pan" }), v(i.container, { cursor: "move" }) }) } transform(t) { let { axes: e = this.axes, event: s, from: i = {}, reset: r, selection: o, to: a = {}, trigger: n } = t, h = this["inverted"], l = !1, d, c; for (var p of (this.hoverPoints?.forEach(t => t.setState()), e)) { var { horiz: u, len: g, minPointOffset: f = 0, options: m, reversed: x } = p, y = u ? "width" : "height", b = u ? "x" : "y", v = D(a[y], p.len), y = D(i[y], p.len), S = Math.abs(v) < 10 ? 1 : v / y, y = (i[b] || 0) + y / 2 - p.pos, b = y - ((a[b] ?? p.pos) + v / 2 - p.pos) / S, v = x && !h || !x && h ? -1 : 1; if (r || !(y < 0 || y > p.len)) { let t = p.toValue(b, !0) + (o || p.isOrdinal ? 0 : f * v), e = p.toValue(b + g / S, !0) - (!o && !p.isOrdinal && f * v || 0), i = p.allExtremes; if (t > e && ([t, e] = [e, t]), 1 == S && !r && "yAxis" === p.coll && !i) { for (var k of p.series) { k = k.getExtremes(k.getProcessedData(!0).yData, !0); i = i ?? { dataMin: Number.MAX_VALUE, dataMax: -Number.MAX_VALUE }, P(k.dataMin) && P(k.dataMax) && (i.dataMin = Math.min(k.dataMin, i.dataMin), i.dataMax = Math.max(k.dataMax, i.dataMax)) } p.allExtremes = i } var { dataMin: x, dataMax: y, min: g, max: f } = w(p.getExtremes(), i || {}), v = x ?? m.min, x = y ?? m.max, y = e - t, C = p.categories ? 0 : Math.min(y, x - v), v = v - C * (M(m.min) ? 0 : m.minPadding), x = x + C * (M(m.max) ? 0 : m.maxPadding), C = p.allowZoomOutside || 1 == S || "zoom" !== n && 1 < S, v = Math.min(m.min ?? v, v, C ? g : v), m = Math.max(m.max ?? x, x, C ? f : x); p.isOrdinal && !p.options.overscroll && 1 == S && !r || (t < v && (t = v, 1 <= S && (e = t + y)), e > m && (e = m, 1 <= S && (t = e - y)), (r || p.series.length && (t !== g || e !== f) && t >= v && e <= m) && (o ? o[p.coll].push({ axis: p, min: t, max: e }) : (p.isPanning = "zoom" !== n, p.isPanning && (c = !0), p.setExtremes(r ? void 0 : t, r ? void 0 : e, !1, !1, { move: b, trigger: n, scale: S }), !r && (t > v || e < m) && "mousewheel" !== n && (d = !0)), l = !0), s && (this[u ? "mouseDownX" : "mouseDownY"] = s[u ? "chartX" : "chartY"])) } } return l && (o ? A(this, "selection", o, () => { delete t.selection, t.trigger = "zoom", this.transform(t) }) : (!d || c || this.resetZoomButton ? !d && this.resetZoomButton && (this.resetZoomButton = this.resetZoomButton.destroy()) : this.showResetZoom(), this.redraw("zoom" === n && (this.options.chart.animation ?? this.pointCount < 100)))), l } } return w(R.prototype, { callbacks: [], collectionsWithInit: { xAxis: [R.prototype.addAxis, [!0]], yAxis: [R.prototype.addAxis, [!1]], series: [R.prototype.addSeries] }, collectionsWithUpdate: ["xAxis", "yAxis", "series"], propsRequireDirtyBox: ["backgroundColor", "borderColor", "borderWidth", "borderRadius", "plotBackgroundColor", "plotBackgroundImage", "plotBorderColor", "plotBorderWidth", "plotShadow", "shadow"], propsRequireReflow: ["margin", "marginTop", "marginRight", "marginBottom", "marginLeft", "spacing", "spacingTop", "spacingRight", "spacingBottom", "spacingLeft"], propsRequireUpdateSeries: ["chart.inverted", "chart.polar", "chart.ignoreHiddenSeries", "chart.type", "colors", "plotOptions", "time", "tooltip"] }), R }), e(t, "Extensions/ScrollablePlotArea.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Globals.js"], t["Core/Renderer/RendererRegistry.js"], t["Core/Utilities.js"]], function (t, e, p, i) { let w = t["stop"], s = e["composed"], { addEvent: u, createElement: g, css: A, defined: T, merge: f, pushUnique: r } = i; function o() { let t = this.scrollablePlotArea; !this.scrollablePixelsX && !this.scrollablePixelsY || t || (this.scrollablePlotArea = t = new h(this)), t?.applyFixed() } function a() { this.chart.scrollablePlotArea && (this.chart.scrollablePlotArea.isDirty = !0) } class h { static compose(t, e, i) { r(s, this.compose) && (u(t, "afterInit", a), u(e, "afterSetChartSize", t => this.afterSetSize(t.target, t)), u(e, "render", o), u(i, "show", a)) } static afterSetSize(t, e) { let i, s, r, { minWidth: o, minHeight: a } = t.options.chart.scrollablePlotArea || {}, { clipBox: n, plotBox: h, inverted: l, renderer: d } = t; if (!d.forExport && (o ? (t.scrollablePixelsX = i = Math.max(0, o - t.chartWidth), i && (t.scrollablePlotBox = f(t.plotBox), h.width = t.plotWidth += i, n[l ? "height" : "width"] += i, r = !0)) : a && (t.scrollablePixelsY = s = Math.max(0, a - t.chartHeight), T(s) && (t.scrollablePlotBox = f(t.plotBox), h.height = t.plotHeight += s, n[l ? "width" : "height"] += s, r = !1)), T(r) && !e.skipAxes)) for (var c of t.axes) c.horiz === r && (c.setAxisSize(), c.setAxisTranslation()) } constructor(i) { let s, t = i.options.chart, e = p.getRendererType(), r = t.scrollablePlotArea || {}, o = this.moveFixedElements.bind(this), a = { WebkitOverflowScrolling: "touch", overflowX: "hidden", overflowY: "hidden" }, n = (i.scrollablePixelsX && (a.overflowX = "auto"), i.scrollablePixelsY && (a.overflowY = "auto"), this.chart = i, this.parentDiv = g("div", { className: "highcharts-scrolling-parent" }, { position: "relative" }, i.renderTo)), h = this.scrollingContainer = g("div", { className: "highcharts-scrolling" }, a, n), l = this.innerContainer = g("div", { className: "highcharts-inner-container" }, void 0, h), d = this.fixedDiv = g("div", { className: "highcharts-fixed" }, { position: "absolute", overflow: "hidden", pointerEvents: "none", zIndex: (t.style?.zIndex || 0) + 2, top: 0 }, void 0, !0), c = this.fixedRenderer = new e(d, i.chartWidth, i.chartHeight, t.style); this.mask = c.path().attr({ fill: t.backgroundColor || "#fff", "fill-opacity": r.opacity ?? .85, zIndex: -1 }).addClass("highcharts-scrollable-mask").add(), h.parentNode.insertBefore(d, h), A(i.renderTo, { overflow: "visible" }), u(i, "afterShowResetZoom", o), u(i, "afterApplyDrilldown", o), u(i, "afterLayOutTitles", o), u(h, "scroll", () => { let { pointer: t, hoverPoint: e } = i; t && (delete t.chartPosition, e && (s = e), t.runPointActions(void 0, s, !0)) }), l.appendChild(i.container) } applyFixed() { let { chart: t, fixedRenderer: e, isDirty: i, scrollingContainer: s } = this, { axisOffset: r, chartWidth: o, chartHeight: a, container: n, plotHeight: h, plotLeft: l, plotTop: d, plotWidth: c, scrollablePixelsX: p = 0, scrollablePixelsY: u = 0 } = t, { scrollPositionX: g = 0, scrollPositionY: f = 0 } = t.options.chart.scrollablePlotArea || {}, m = o + p, x = a + u, y = (e.setSize(o, a), (i ?? !0) && (this.isDirty = !1, this.moveFixedElements()), w(t.container), A(n, { width: m + "px", height: x + "px" }), t.renderer.boxWrapper.attr({ width: m, height: x, viewBox: [0, 0, m, x].join(" ") }), t.chartBackground?.attr({ width: m, height: x }), A(s, { width: o + "px", height: a + "px" }), T(i) || (s.scrollLeft = p * g, s.scrollTop = u * f), d - r[0] - 1), b = l - r[3] - 1, v = d + h + r[2] + 1, S = l + c + r[1] + 1, k = l + c - p, C = d + h - u, M = [["M", 0, 0]]; p ? M = [["M", 0, y], ["L", l - 1, y], ["L", l - 1, v], ["L", 0, v], ["Z"], ["M", k, y], ["L", o, y], ["L", o, v], ["L", k, v], ["Z"]] : u && (M = [["M", b, 0], ["L", b, d - 1], ["L", S, d - 1], ["L", S, 0], ["Z"], ["M", b, C], ["L", b, a], ["L", S, a], ["L", S, C], ["Z"]]), "adjustHeight" !== t.redrawTrigger && this.mask.attr({ d: M }) } moveFixedElements() { let t, { container: e, inverted: i, scrollablePixelsX: s, scrollablePixelsY: r } = this.chart, o = this.fixedRenderer, a = h.fixedSelectors; for (var n of (s && !i ? t = ".highcharts-yaxis" : s && i || r && !i ? t = ".highcharts-xaxis" : r && i && (t = ".highcharts-yaxis"), t && a.push(t + ":not(.highcharts-radial-axis)", t + "-labels:not(.highcharts-radial-axis-labels)"), a)) [].forEach.call(e.querySelectorAll(n), t => { (t.namespaceURI === o.SVG_NS ? o.box : o.box.parentNode).appendChild(t), t.style.pointerEvents = "auto" }) } } return h.fixedSelectors = [".highcharts-breadcrumbs-group", ".highcharts-contextbutton", ".highcharts-caption", ".highcharts-credits", ".highcharts-drillup-button", ".highcharts-legend", ".highcharts-legend-checkbox", ".highcharts-navigator-series", ".highcharts-navigator-xaxis", ".highcharts-navigator-yaxis", ".highcharts-navigator", ".highcharts-range-selector-group", ".highcharts-reset-zoom", ".highcharts-scrollbar", ".highcharts-subtitle", ".highcharts-title"], h }), e(t, "Core/Axis/Stacking/StackItem.js", [t["Core/Templating.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let o = t["format"], g = e["series"], { destroyObjectProperties: s, fireEvent: f, isNumber: m, pick: x } = i; return class { constructor(t, e, i, s, r) { var o = t.chart.inverted, a = t.reversed, t = (this.axis = t, this.isNegative = !!i != !!a); this.options = e = e || {}, this.x = s, this.total = null, this.cumulative = null, this.points = {}, this.hasValidPoints = !1, this.stack = r, this.leftCliff = 0, this.rightCliff = 0, this.alignOptions = { align: e.align || (o ? t ? "left" : "right" : "center"), verticalAlign: e.verticalAlign || (o ? "middle" : t ? "bottom" : "top"), y: e.y, x: e.x }, this.textAlign = e.textAlign || (o ? t ? "right" : "left" : "center") } destroy() { s(this, this.axis) } render(e) { let i = this.axis.chart, s = this.options, t = s.format, r = t ? o(t, this, i) : s.formatter.call(this); if (this.label) this.label.attr({ text: r, visibility: "hidden" }); else { this.label = i.renderer.label(r, null, void 0, s.shape, void 0, void 0, s.useHTML, !1, "stack-labels"); let t = { r: s.borderRadius || 0, text: r, padding: x(s.padding, 5), visibility: "hidden" }; i.styledMode || (t.fill = s.backgroundColor, t.stroke = s.borderColor, t["stroke-width"] = s.borderWidth, this.label.css(s.style || {})), this.label.attr(t), this.label.added || this.label.add(e) } this.label.labelrank = i.plotSizeY, f(this, "afterRender") } setOffset(t, e, r, o, i, s) { let { alignOptions: a, axis: n, label: h, options: l, textAlign: d } = this, c = n.chart, p = this.getStackBox({ xOffset: t, width: e, boxBottom: r, boxTop: o, defaultX: i, xAxis: s }), u = a["verticalAlign"]; if (h && p) { let t = h.getBBox(void 0, 0), e = h.padding, i = "justify" === x(l.overflow, "justify"), s; a.x = l.x || 0, a.y = l.y || 0; var { x: r, y: o } = this.adjustStackPosition({ labelBox: t, verticalAlign: u, textAlign: d }); p.x -= r, p.y -= o, h.align(a, !1, p), (i = (s = c.isInsidePlot(h.alignAttr.x + a.x + r, h.alignAttr.y + a.y + o)) ? i : !1) && g.prototype.justifyDataLabel.call(n, h, a, h.alignAttr, t, p), h.attr({ x: h.alignAttr.x, y: h.alignAttr.y, rotation: l.rotation, rotationOriginX: t.width * { left: 0, center: .5, right: 1 }[l.textAlign || "center"], rotationOriginY: t.height / 2 }), x(!i && l.crop, !0) && (s = m(h.x) && m(h.y) && c.isInsidePlot(h.x - e + (h.width || 0), h.y) && c.isInsidePlot(h.x + e, h.y)), h[s ? "show" : "hide"]() } f(this, "afterSetOffset", { xOffset: t, width: e }) } adjustStackPosition({ labelBox: t, verticalAlign: e, textAlign: i }) { var s = { bottom: 0, middle: 1, top: 2, right: 1, center: 0, left: -1 }; return { x: t.width / 2 + t.width / 2 * s[i], y: t.height / 2 * s[e] } } getStackBox(t) { let e = this.axis, i = e.chart, { boxTop: s, defaultX: r, xOffset: o, width: a, boxBottom: n } = t, h = e.stacking.usePercentage ? 100 : x(s, this.total, 0), l = e.toPixels(h), d = t.xAxis || i.xAxis[0], c = x(r, d.translate(this.x)) + o, p = Math.abs(l - e.toPixels(n || m(e.min) && e.logarithmic && e.logarithmic.lin2log(e.min) || 0)), u = i.inverted, g = this.isNegative; return u ? { x: (g ? l : l - p) - i.plotLeft, y: d.height - c - a + d.top - i.plotTop, width: p, height: a } : { x: c + d.transB - i.plotLeft, y: (g ? l - p : l) - i.plotTop, width: a, height: p } } } }), e(t, "Core/Axis/Stacking/StackingAxis.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Axis/Axis.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Axis/Stacking/StackItem.js"], t["Core/Utilities.js"]], function (t, e, i, A, s) { let a = t["getDeferredAnimation"], { prototype: r } = i["series"], { addEvent: o, correctFloat: T, defined: P, destroyObjectProperties: n, fireEvent: h, isArray: L, isNumber: l, objectEach: d, pick: O } = s; function c() { let i = this.inverted; this.axes.forEach(t => { t.stacking && t.stacking.stacks && t.hasVisibleSeries && (t.stacking.oldStacks = t.stacking.stacks) }), this.series.forEach(t => { let e = t.xAxis && t.xAxis.options || {}; t.options.stacking && t.reserveSpace() && (t.stackKey = [t.type, O(t.options.stack, ""), i ? e.top : e.left, i ? e.height : e.width].join(",")) }) } function p() { let t = this.stacking; if (t) { let i = t.stacks; d(i, (t, e) => { n(t), delete i[e] }), t.stackTotalGroup?.destroy() } } function u() { this.stacking || (this.stacking = new b(this)) } function g(t, e, i, s) { return !P(t) || t.x !== e || s && t.stackKey !== s ? t = { x: e, index: 0, key: s, stackKey: s } : t.index++, t.key = [i, e, t.index].join(","), t } function f() { let o, a = this, t = a.yAxis, e = a.stackKey || "", n = t.stacking.stacks, h = a.processedXData, i = a.options.stacking, l = a[i + "Stacker"]; l && [e, "-" + e].forEach(t => { let e = h.length, i, s, r; for (; e--;)i = h[e], o = a.getStackIndicator(o, i, a.index, t), (r = (s = n[t]?.[i])?.points[o.key || ""]) && l.call(a, r, s, e) }) } function m(t, e, i) { e = e.total ? 100 / e.total : 0; t[0] = T(t[0] * e), t[1] = T(t[1] * e), this.stackedYData[i] = t[1] } function x(t) { (this.is("column") || this.is("columnrange")) && (this.options.centerInCategory && !this.options.stacking && 1 < this.chart.series.length ? r.setStackedPoints.call(this, t, "group") : t.stacking.resetStacks()) } function y(g, f) { let m, x, y, b, v, S, k, C, M; var w = f || this.options.stacking; if (w && this.reserveSpace() && ({ group: "xAxis" }[w] || "yAxis") === g.coll) { let e = this.processedXData, i = this.processedYData, s = [], t = i.length, r = this.options, o = r.threshold || 0, a = r.startFromThreshold ? o : 0, n = r.stack, h = f ? this.type + "," + w : this.stackKey || "", l = "-" + h, d = this.negStacks, c = g.stacking, p = c.stacks, u = c.oldStacks; for (c.stacksTouched += 1, k = 0; k < t; k++) { C = e[k], M = i[k], S = (m = this.getStackIndicator(m, C, this.index)).key || "", p[v = (x = d && M < (a ? 0 : o)) ? l : h] || (p[v] = {}), p[v][C] || (u[v]?.[C] ? (p[v][C] = u[v][C], p[v][C].total = null) : p[v][C] = new A(g, g.options.stackLabels, !!x, C, n)), y = p[v][C], null !== M ? (y.points[S] = y.points[this.index] = [O(y.cumulative, a)], P(y.cumulative) || (y.base = S), y.touched = c.stacksTouched, 0 < m.index && !1 === this.singleStacks && (y.points[S][0] = y.points[this.index + "," + C + ",0"][0])) : (delete y.points[S], delete y.points[this.index]); let t = y.total || 0; "percent" === w ? (b = x ? h : l, t = d && p[b]?.[C] ? (b = p[b][C]).total = Math.max(b.total || 0, t) + Math.abs(M) || 0 : T(t + (Math.abs(M) || 0))) : "group" === w ? null !== (M = L(M) ? M[0] : M) && t++ : t = T(t + (M || 0)), y.cumulative = "group" === w ? (t || 1) - 1 : T(O(y.cumulative, a) + (M || 0)), y.total = t, null !== M && (y.points[S].push(y.cumulative), s[k] = y.cumulative, y.hasValidPoints = !0) } "percent" === w && (c.usePercentage = !0), "group" !== w && (this.stackedYData = s), c.oldStacks = {} } } class b { constructor(t) { this.oldStacks = {}, this.stacks = {}, this.stacksTouched = 0, this.axis = t } buildStacks() { let t, e, i = this.axis, s = i.series, r = "xAxis" === i.coll, o = i.options.reversedStacks, a = s.length; for (this.resetStacks(), this.usePercentage = !1, e = a; e--;)t = s[o ? e : a - e - 1], r && t.setGroupedPoints(i), t.setStackedPoints(i); if (!r) for (e = 0; e < a; e++)s[e].modifyStacks(); h(i, "afterBuildStacks") } cleanStacks() { this.oldStacks && (this.stacks = this.oldStacks, d(this.stacks, t => { d(t, t => { t.cumulative = t.total }) })) } resetStacks() { d(this.stacks, i => { d(i, (t, e) => { l(t.touched) && t.touched < this.stacksTouched ? (t.destroy(), delete i[e]) : (t.total = null, t.cumulative = null) }) }) } renderStackTotals() { let t = this.axis, e = t.chart, i = e.renderer, s = this.stacks, r = a(e, t.options.stackLabels?.animation || !1), o = this.stackTotalGroup = this.stackTotalGroup || i.g("stack-labels").attr({ zIndex: 6, opacity: 0 }).add(); o.translate(e.plotLeft, e.plotTop), d(s, t => { d(t, t => { t.render(o) }) }), o.animate({ opacity: 1 }, r) } } return { compose: function (t, e, i) { let s = e.prototype, r = i.prototype; s.getStacks || (o(t, "init", u), o(t, "destroy", p), s.getStacks = c, r.getStackIndicator = g, r.modifyStacks = f, r.percentStacker = m, r.setGroupedPoints = x, r.setStackedPoints = y) } } }), e(t, "Series/Line/LineSeries.js", [t["Core/Series/Series.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let { defined: m, merge: l, isObject: d } = i; class s extends t { drawGraph() { let a = this.options, n = (this.gappedPath || this.getGraphPath).call(this), h = this.chart.styledMode;[this, ...this.zones].forEach((t, e) => { let i, s = t.graph, r = s ? "animate" : "attr", o = t.dashStyle || a.dashStyle; s ? (s.endX = this.preventGraphAnimation ? null : n.xMap, s.animate({ d: n })) : n.length && (t.graph = s = this.chart.renderer.path(n).addClass("highcharts-graph" + (e ? ` highcharts-zone-graph-${e - 1} ` : " ") + (e && t.className || "")).attr({ zIndex: 1 }).add(this.group)), s && !h && (i = { stroke: !e && a.lineColor || t.color || this.color || "#cccccc", "stroke-width": a.lineWidth || 0, fill: this.fillGraph && this.color || "none" }, o ? i.dashstyle = o : "square" !== a.linecap && (i["stroke-linecap"] = i["stroke-linejoin"] = "round"), s[r](i).shadow(e < 2 && a.shadow && l({ filterUnits: "userSpaceOnUse" }, d(a.shadow) ? a.shadow : {}))), s && (s.startX = n.xMap, s.isArea = n.isArea) }) } getGraphPath(n, h, l) { let d = this, c = d.options, p = [], u = [], g, f = c.step, t = (n = n || d.points).reversed; return t && n.reverse(), (f = { right: 1, center: 2 }[f] || f && 3) && t && (f = 4 - f), (n = this.getValidPoints(n, !1, !(c.connectNulls && !h && !l))).forEach(function (t, e) { let i, s = t.plotX, r = t.plotY, o = n[e - 1], a = t.isNull || "number" != typeof r; (t.leftCliff || o && o.rightCliff) && !l && (g = !0), g = a && !m(h) && 0 < e ? !c.connectNulls : !(!a || h) || (0 === e || g ? i = [["M", t.plotX, t.plotY]] : d.getPointSpline ? i = [d.getPointSpline(n, t, e)] : f ? (i = 1 === f ? [["L", o.plotX, r]] : 2 === f ? [["L", (o.plotX + s) / 2, o.plotY], ["L", (o.plotX + s) / 2, r]] : [["L", s, o.plotY]]).push(["L", s, r]) : i = [["L", s, r]], u.push(t.x), f && (u.push(t.x), 2 === f && u.push(t.x)), p.push.apply(p, i), !1) }), p.xMap = u, d.graphPath = p } } return s.defaultOptions = l(t.defaultOptions, { legendSymbol: "lineMarker" }), e.registerSeriesType("line", s), s }), e(t, "Series/Area/AreaSeriesDefaults.js", [], function () { return { threshold: 0, legendSymbol: "areaMarker" } }), e(t, "Series/Area/AreaSeries.js", [t["Series/Area/AreaSeriesDefaults.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let { line: S } = e["seriesTypes"], { extend: s, merge: r, objectEach: o, pick: k } = i; class a extends S { drawGraph() { this.areaPath = [], super.drawGraph.apply(this); let { areaPath: a, options: n } = this;[this, ...this.zones].forEach((t, e) => { let i = {}, s = t.fillColor || n.fillColor, r = t.area, o = r ? "animate" : "attr"; r ? (r.endX = this.preventGraphAnimation ? null : a.xMap, r.animate({ d: a })) : (i.zIndex = 0, (r = t.area = this.chart.renderer.path(a).addClass("highcharts-area" + (e ? ` highcharts-zone-area-${e - 1} ` : " ") + (e && t.className || "")).add(this.group)).isArea = !0), this.chart.styledMode || (i.fill = s || t.color || this.color, i["fill-opacity"] = s ? 1 : n.fillOpacity ?? .75, r.css({ pointerEvents: this.stickyTracking ? "none" : "auto" })), r[o](i), r.startX = a.xMap, r.shiftUnit = n.step ? 2 : 1 }) } getGraphPath(d) { function i(t, e, i) { let s = d[t], r = p && x[s.x].points[m], o = s[i + "Null"] || 0, a = s[i + "Cliff"] || 0, n, h, l = !0; a || o ? (n = (o ? r[0] : r[1]) + a, h = r[0] + a, l = !!o) : !p && d[e] && d[e].isNull && (n = h = y), void 0 !== n && (f.push({ plotX: c, plotY: null === n ? b : u.getThreshold(n), isNull: l, isCliff: !0 }), g.push({ plotX: c, plotY: null === h ? b : u.getThreshold(h), doCurve: !1 })) } let c, s, r, t = S.prototype.getGraphPath, e = this.options, p = e.stacking, u = this.yAxis, g = [], f = [], m = this.index, x = u.stacking.stacks[this.stackKey], y = e.threshold, b = Math.round(u.getThreshold(e.threshold)), o = k(e.connectNulls, "percent" === p); d = d || this.points; for (let t = 0, e = (d = p ? this.getStackPoints(d) : d).length; t < e; ++t)p || (d[t].leftCliff = d[t].rightCliff = d[t].leftNull = d[t].rightNull = void 0), s = d[t].isNull, c = k(d[t].rectPlotX, d[t].plotX), r = p ? k(d[t].yBottom, b) : b, s && !o || (o || i(t, t - 1, "left"), s && !p && o || (f.push(d[t]), g.push({ x: t, plotX: c, plotY: r })), o || i(t, t + 1, "right")); let a = t.call(this, f, !0, !0), n = (g.reversed = !0, t.call(this, g, !0, !0)), h = n[0], l = (h && "M" === h[0] && (n[0] = ["L", h[1], h[2]]), a.concat(n)), v = (l.length && l.push(["Z"]), t.call(this, f, !1, o)); return 1 < this.chart.series.length && p && f.some(t => t.isCliff) && (l.hasStackedCliffs = v.hasStackedCliffs = !0), l.xMap = a.xMap, this.areaPath = l, v } getStackPoints(e) { let d = this, s = [], c = [], r = this.xAxis, p = this.yAxis, u = p.stacking.stacks[this.stackKey], g = {}, f = p.series, m = f.length, x = p.options.reversedStacks ? 1 : -1, y = f.indexOf(d); if (e = e || this.points, this.options.stacking) { for (let t = 0; t < e.length; t++)e[t].leftNull = e[t].rightNull = void 0, g[e[t].x] = e[t]; o(u, function (t, e) { null !== t.total && c.push(e) }), c.sort(function (t, e) { return t - e }); let l = f.map(t => t.visible); c.forEach(function (o, a) { let e = 0, n, h; if (g[o] && !g[o].isNull) s.push(g[o]), [-1, 1].forEach(function (t) { let e = 1 === t ? "rightNull" : "leftNull", i = u[c[a + t]], s = 0; if (i) { let t = y; for (; 0 <= t && t < m;) { var r = f[t].index; (n = i.points[r]) || (r === d.index ? g[o][e] = !0 : l[t] && (h = u[o].points[r]) && (s -= h[1] - h[0])), t += x } } g[o][1 === t ? "rightCliff" : "leftCliff"] = s }); else { let t = y; for (; 0 <= t && t < m;) { var i = f[t].index; if (n = u[o].points[i]) { e = n[1]; break } t += x } e = k(e, 0), e = p.translate(e, 0, 1, 0, 1), s.push({ isNull: !0, plotX: r.translate(o, 0, 0, 0, 1), x: o, plotY: e, yBottom: e }) } }) } return s } } return a.defaultOptions = r(S.defaultOptions, t), s(a.prototype, { singleStacks: !1 }), e.registerSeriesType("area", a), a }), e(t, "Series/Spline/SplineSeries.js", [t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e) { let i = t.seriesTypes["line"], { merge: s, pick: r } = e; class o extends i { getPointSpline(t, o, e) { let a, n, h, l, d = o.plotX || 0, c = o.plotY || 0, p = t[e - 1], u = t[e + 1]; function i(t) { return t && !t.isNull && !1 !== t.doCurve && !o.isCliff } if (i(p) && i(u)) { let t = p.plotX || 0, e = p.plotY || 0, i = u.plotX || 0, s = u.plotY || 0, r = 0; a = (1.5 * d + t) / 2.5, n = (1.5 * c + e) / 2.5, h = (1.5 * d + i) / 2.5, l = (1.5 * c + s) / 2.5, h !== a && (r = (l - n) * (h - d) / (h - a) + c - l), n += r, l += r, n > e && n > c ? (n = Math.max(e, c), l = 2 * c - n) : n < e && n < c && (n = Math.min(e, c), l = 2 * c - n), l > s && l > c ? (l = Math.max(s, c), n = 2 * c - l) : l < s && l < c && (l = Math.min(s, c), n = 2 * c - l), o.rightContX = h, o.rightContY = l, o.controlPoints = { low: [a, n], high: [h, l] } } t = ["C", r(p.rightContX, p.plotX, 0), r(p.rightContY, p.plotY, 0), r(a, d, 0), r(n, c, 0), d, c]; return p.rightContX = p.rightContY = void 0, t } } return o.defaultOptions = s(i.defaultOptions), t.registerSeriesType("spline", o), o }), e(t, "Series/AreaSpline/AreaSplineSeries.js", [t["Series/Spline/SplineSeries.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let { area: s, area: { prototype: r } } = e.seriesTypes, { extend: o, merge: a } = i; class n extends t { } return n.defaultOptions = a(t.defaultOptions, s.defaultOptions), o(n.prototype, { getGraphPath: r.getGraphPath, getStackPoints: r.getStackPoints, drawGraph: r.drawGraph }), e.registerSeriesType("areaspline", n), n }), e(t, "Series/Column/ColumnSeriesDefaults.js", [], function () { return { borderRadius: 3, centerInCategory: !1, groupPadding: .2, marker: null, pointPadding: .1, minPointLength: 0, cropThreshold: 50, pointRange: null, states: { hover: { halo: !1, brightness: .1 }, select: { color: "#cccccc", borderColor: "#000000" } }, dataLabels: { align: void 0, verticalAlign: void 0, y: void 0 }, startFromThreshold: !0, stickyTracking: !1, tooltip: { distance: 6 }, threshold: 0, borderColor: "#ffffff" } }), e(t, "Series/Column/ColumnSeries.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Color/Color.js"], t["Series/Column/ColumnSeriesDefaults.js"], t["Core/Globals.js"], t["Core/Series/Series.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s, r, o, a) { let p = t["animObject"], f = e["parse"], n = s["noop"], { clamp: w, crisp: h, defined: A, extend: u, fireEvent: l, isArray: d, isNumber: T, merge: m, pick: P, objectEach: c } = a; class g extends r { animate(t) { let i, e, s = this, r = this.yAxis, o = r.pos, a = r.reversed, n = s.options, { clipOffset: h, inverted: l } = this.chart, d = {}, c = l ? "translateX" : "translateY"; t && h ? (d.scaleY = .001, e = w(r.toPixels(n.threshold), o, o + r.len), l ? (e += a ? -Math.floor(h[0]) : Math.ceil(h[2]), d.translateX = e - r.len) : (e += a ? Math.ceil(h[0]) : -Math.floor(h[2]), d.translateY = e), s.clipBox && s.setClip(), s.group.attr(d)) : (i = Number(s.group.attr(c)), s.group.animate({ scaleY: 1 }, u(p(s.options.animation), { step: function (t, e) { s.group && (d[c] = i + e.pos * (o - i), s.group.attr(d)) } }))) } init(t, e) { super.init.apply(this, arguments); let i = this; (t = i.chart).hasRendered && t.series.forEach(function (t) { t.type === i.type && (t.isDirty = !0) }) } getColumnMetrics() { let r = this, t = r.options, e = r.xAxis, o = r.yAxis, i = e.options.reversedStacks, s = e.reversed && !i || !e.reversed && i, a = {}, n, h = 0; !1 === t.grouping ? h = 1 : r.chart.series.forEach(function (t) { let e; var i = t.yAxis, s = t.options; t.type === r.type && t.reserveSpace() && o.len === i.len && o.pos === i.pos && (s.stacking && "group" !== s.stacking ? (void 0 === a[n = t.stackKey] && (a[n] = h++), e = a[n]) : !1 !== s.grouping && (e = h++), t.columnIndex = e) }); var l = Math.min(Math.abs(e.transA) * (!e.brokenAxis?.hasBreaks && e.ordinal?.slope || t.pointRange || e.closestPointRange || e.tickInterval || 1), e.len), d = l * t.groupPadding, c = (l - 2 * d) / (h || 1), p = Math.min(t.maxPointWidth || e.len, P(t.pointWidth, c * (1 - 2 * t.pointPadding))), u = (r.columnIndex || 0) + (s ? 1 : 0); return r.columnMetrics = { width: p, offset: (c - p) / 2 + (d + u * c - l / 2) * (s ? -1 : 1), paddedWidth: c, columnCount: h }, r.columnMetrics } crispCol(t, e, i, s) { var r = this.borderWidth, o = this.chart.inverted; return s = h(e + s, r, o) - (e = h(e, r, o)), this.options.crisp && (i = h(t + i, r) - (t = h(t, r))), { x: t, y: e, width: i, height: s } } adjustForMissingColumns(t, e, o, a) { if (!o.isNull && 1 < a.columnCount) { let i = this.xAxis.series.filter(t => t.visible).map(t => t.index), s = 0, r = 0; c(this.xAxis.stacking?.stacks, t => { if ("number" == typeof o.x) { let e = t[o.x.toString()]; if (e && d(e.points[this.index])) { let t = Object.keys(e.points).filter(t => !t.match(",") && e.points[t] && 1 < e.points[t].length).map(parseFloat).filter(t => -1 !== i.indexOf(t)).sort((t, e) => e - t); s = t.indexOf(this.index), r = t.length } } }), s = this.xAxis.reversed ? r - 1 - s : s; var n = (r - 1) * a.paddedWidth + e; t = (o.plotX || 0) + n / 2 - e - s * a.paddedWidth } return t } translate() { let c = this, p = c.chart, u = c.options, t = c.dense = c.closestPointRange * c.xAxis.transA < 2, e = c.borderWidth = P(u.borderWidth, t ? 0 : 1), g = c.xAxis, f = c.yAxis, m = u.threshold, x = P(u.minPointLength, 5), y = c.getColumnMetrics(), b = y.width, v = c.pointXOffset = y.offset, S = c.dataMin, k = c.dataMax, C = c.translatedThreshold = f.getThreshold(m), M = c.barW = Math.max(b, 1 + 2 * e); u.pointPadding && (M = Math.ceil(M)), r.prototype.translate.apply(c), c.points.forEach(function (t) { let e = P(t.yBottom, C), i = 999 + Math.abs(e), s = t.plotX || 0, r = w(t.plotY, -i, f.len + i), o, a = Math.min(r, e), n = Math.max(r, e) - a, h = b, l = s + v, d = M; x && Math.abs(n) < x && (n = x, o = !f.reversed && !t.negative || f.reversed && t.negative, T(m) && T(k) && t.y === m && k <= m && (f.min || 0) < m && (S !== k || (f.max || 0) <= m) && (o = !o, t.negative = !t.negative), a = Math.abs(a - C) > x ? e - x : C - (o ? x : 0)), A(t.options.pointWidth) && (l -= Math.round(((h = d = Math.ceil(t.options.pointWidth)) - b) / 2)), u.centerInCategory && !u.stacking && (l = c.adjustForMissingColumns(l, h, t, y)), t.barX = l, t.pointWidth = h, t.tooltipPos = p.inverted ? [w(f.len + f.pos - p.plotLeft - r, f.pos - p.plotLeft, f.len + f.pos - p.plotLeft), g.len + g.pos - p.plotTop - l - d / 2, n] : [g.left - p.plotLeft + l + d / 2, w(r + f.pos - p.plotTop, f.pos - p.plotTop, f.len + f.pos - p.plotTop), n], t.shapeType = c.pointClass.prototype.shapeType || "roundedRect", t.shapeArgs = c.crispCol(l, t.isNull ? C : a, d, t.isNull ? 0 : n) }), l(this, "afterColumnTranslate") } drawGraph() { this.group[this.dense ? "addClass" : "removeClass"]("highcharts-dense-data") } pointAttribs(t, e) { let i = this.options, s = this.pointAttrToOptions || {}, r = s.stroke || "borderColor", o = s["stroke-width"] || "borderWidth", a, n, h, l = t && t.color || this.color, d = t && t[r] || i[r] || l, c = t && t.options.dashStyle || i.dashStyle, p = t && t[o] || i[o] || this[o] || 0, u = P(t && t.opacity, i.opacity, 1), g = (t && this.zones.length && (n = t.getZone(), l = t.options.color || n && (n.color || t.nonZonedColor) || this.color, n && (d = n.borderColor || d, c = n.dashStyle || c, p = n.borderWidth || p)), e && t && (h = (a = m(i.states[e], t.options.states && t.options.states[e] || {})).brightness, l = a.color || void 0 !== h && f(l).brighten(a.brightness).get() || l, d = a[r] || d, p = a[o] || p, c = a.dashStyle || c, u = P(a.opacity, u)), { fill: l, stroke: d, "stroke-width": p, opacity: u }); return c && (g.dashstyle = c), g } drawPoints(t = this.points) { let o, a = this, n = this.chart, h = a.options, l = n.renderer, d = h.animationLimit || 250; t.forEach(function (t) { let e = t.plotY, i = t.graphic, s = !!i, r = i && n.pointCount < d ? "animate" : "attr"; T(e) && null !== t.y ? (o = t.shapeArgs, i && t.hasNewShapeType() && (i = i.destroy()), a.enabledDataSorting && (t.startXPos = a.xAxis.reversed ? -(o && o.width || 0) : a.xAxis.width), i || (t.graphic = i = l[t.shapeType](o).add(t.group || a.group), i && a.enabledDataSorting && n.hasRendered && n.pointCount < d && (i.attr({ x: t.startXPos }), s = !0, r = "animate")), i && s && i[r](m(o)), n.styledMode || i[r](a.pointAttribs(t, t.selected && "select")).shadow(!1 !== t.allowShadow && h.shadow), i && (i.addClass(t.getClassName(), !0), i.attr({ visibility: t.visible ? "inherit" : "hidden" }))) : i && (t.graphic = i.destroy()) }) } drawTracker(t = this.points) { function e(t) { let e = o?.getPointFromEvent(t); o && e && s.options.enableMouseTracking && (o.isDirectTouch = !0, e.onMouseOver(t)) } let i, s = this, r = s.chart, o = r.pointer; t.forEach(function (e) { i = d(e.dataLabels) ? e.dataLabels : e.dataLabel ? [e.dataLabel] : [], e.graphic && (e.graphic.element.point = e), i.forEach(function (t) { (t.div || t.element).point = e }) }), s._hasTracking || (s.trackerGroups.forEach(function (t) { s[t] && (s[t].addClass("highcharts-tracker").on("mouseover", e).on("mouseout", function (t) { o?.onTrackerMouseOut(t) }).on("touchstart", e), !r.styledMode && s.options.cursor && s[t].css({ cursor: s.options.cursor })) }), s._hasTracking = !0), l(this, "afterDrawTracker") } remove() { let e = this, t = e.chart; t.hasRendered && t.series.forEach(function (t) { t.type === e.type && (t.isDirty = !0) }), r.prototype.remove.apply(e, arguments) } } return g.defaultOptions = m(r.defaultOptions, i), u(g.prototype, { directTouch: !0, getSymbol: n, negStacks: !0, trackerGroups: ["group", "dataLabelsGroup"] }), o.registerSeriesType("column", g), g }), e(t, "Core/Series/DataLabel.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Templating.js"], t["Core/Utilities.js"]], function (t, e, i) { var s; let n = t["getDeferredAnimation"], A = e["format"], { defined: T, extend: v, fireEvent: P, isArray: r, isString: L, merge: S, objectEach: O, pick: E, pInt: D, splat: h } = i; return (s || (s = {})).compose = function (t) { let e = t.prototype; e.initDataLabels || (e.initDataLabels = d, e.initDataLabelsGroup = l, e.alignDataLabel = a, e.drawDataLabels = c, e.justifyDataLabel = p, e.setDataLabelStartPos = f, e.hasDataLabels = o) }, s; function o() { return g(this).some(t => t?.enabled) } function a(t, e, i, s, r) { let { chart: o, enabledDataSorting: a } = this, n = this.isCartesian && o.inverted, h = t.plotX, l = t.plotY, d = i.rotation || 0, c = T(h) && T(l) && o.isInsidePlot(h, Math.round(l), { inverted: n, paneCoordinates: !0, series: this }), p = 0 === d && "justify" === E(i.overflow, a ? "none" : "justify"), u = this.visible && !1 !== t.visible && T(h) && (t.series.forceDL || a && !p || c || E(i.inside, !!this.options.stacking) && s && o.isInsidePlot(h, n ? s.x + 1 : s.y + s.height - 1, { inverted: n, paneCoordinates: !0, series: this })), g = t.pos(); var f, m, x, y, b; u && g && (f = e.getBBox(), m = e.getBBox(void 0, 0), x = { right: 1, center: .5 }[i.align || 0] || 0, y = { bottom: 1, middle: .5 }[i.verticalAlign || 0] || 0, s = v({ x: g[0], y: Math.round(g[1]), width: 0, height: 0 }, s || {}), "plotEdges" === i.alignTo && this.isCartesian && (s[n ? "x" : "y"] = 0, s[n ? "width" : "height"] = this.yAxis?.len || 0), v(i, { width: f.width, height: f.height }), b = s, a && this.xAxis && !p && this.setDataLabelStartPos(t, e, r, c, b), e.align(S(i, { width: m.width, height: m.height }), !1, s, !1), e.alignAttr.x += x * (m.width - f.width), e.alignAttr.y += y * (m.height - f.height), e[e.placed ? "animate" : "attr"]({ x: e.alignAttr.x + (f.width - m.width) / 2, y: e.alignAttr.y + (f.height - m.height) / 2, rotationOriginX: (e.width || 0) / 2, rotationOriginY: (e.height || 0) / 2 }), p && 0 <= s.height ? this.justifyDataLabel(e, i, e.alignAttr, f, s, r) : E(i.crop, !0) && ({ x: t, y: b } = e.alignAttr, u = o.isInsidePlot(t, b, { paneCoordinates: !0, series: this }) && o.isInsidePlot(t + f.width - 1, b + f.height - 1, { paneCoordinates: !0, series: this })), i.shape && !d && e[r ? "attr" : "animate"]({ anchorX: g[0], anchorY: g[1] })), r && a && (e.placed = !1), u || a && !p ? (e.show(), e.placed = !0) : (e.hide(), e.placed = !1) } function l() { return this.plotGroup("dataLabelsGroup", "data-labels", this.hasRendered ? "inherit" : "hidden", this.options.dataLabels.zIndex || 6) } function d(t) { let e = this.hasRendered || 0, i = this.initDataLabelsGroup().attr({ opacity: +e }); return !e && i && (this.visible && i.show(), this.options.animation ? i.animate({ opacity: 1 }, t) : i.attr({ opacity: 1 })), i } function c(t) { let v, S = (t = t || this.points, this), k = S.chart, C = S.options, M = k.renderer, { backgroundColor: e, plotBackgroundColor: i } = k.options.chart, w = M.getContrast(L(i) && i || L(e) && e || "#000000"), s = g(S), { animation: r, defer: o } = s[0], a = o ? n(k, r, S) : { defer: 0, duration: 0 }; P(this, "drawDataLabels"), S.hasDataLabels?.() && (v = this.initDataLabels(a), t.forEach(y => { let b = y.dataLabels || [], t = (h(u(s, y.dlOptions || y.options?.dataLabels)).forEach((t, e) => { let i = t.enabled && (y.visible || y.dataLabelOnHidden) && (!y.isNull || y.dataLabelOnNull) && (!(x = t.filter) || (f = x.operator, m = y[x.property], x = x.value, ">" === f && x < m || "<" === f && m < x || ">=" === f && x <= m || "<=" === f && m <= x || "==" === f && m == x || "===" === f && m === x || "!=" === f && m != x || "!==" === f && m !== x)), { backgroundColor: s, borderColor: r, distance: o, style: a = {} } = t, n, h, l, d, c = {}, p = b[e], u = !p, g; var f, m, x; i && (h = E(t[y.formatPrefix + "Format"], t.format), n = y.getLabelConfig(), l = T(h) ? A(h, n, k) : (t[y.formatPrefix + "Formatter"] || t.formatter).call(n, t), d = t.rotation, k.styledMode || (a.color = E(t.color, a.color, L(S.color) ? S.color : void 0, "#000000"), "contrast" === a.color ? ("none" !== s && (g = s), y.contrastColor = M.getContrast("auto" !== g && g || y.color || S.color), a.color = g || !T(o) && t.inside || D(o || 0) < 0 || C.stacking ? y.contrastColor : w) : delete y.contrastColor, C.cursor && (a.cursor = C.cursor)), c = { r: t.borderRadius || 0, rotation: d, padding: t.padding, zIndex: 1 }, k.styledMode || (c.fill = "auto" === s ? y.color : s, c.stroke = "auto" === r ? y.color : r, c["stroke-width"] = t.borderWidth), O(c, (t, e) => { void 0 === t && delete c[e] })), !p || i && T(l) && !!p.div == !!t.useHTML && (p.rotation && t.rotation || p.rotation === t.rotation) || (p = void 0, u = !0), i && T(l) && (p ? c.text = l : (p = M.label(l, 0, 0, t.shape, void 0, void 0, t.useHTML, void 0, "data-label")).addClass(" highcharts-data-label-color-" + y.colorIndex + " " + (t.className || "") + (t.useHTML ? " highcharts-tracker" : "")), p && (p.options = t, p.attr(c), k.styledMode ? a.width && p.css({ width: a.width, textOverflow: a.textOverflow }) : p.css(a).shadow(t.shadow), P(p, "beforeAddingDataLabel", { labelOptions: t, point: y }), p.added || p.add(v), S.alignDataLabel(y, p, t, void 0, u), p.isActive = !0, b[e] && b[e] !== p && b[e].destroy(), b[e] = p)) }), b.length); for (; t--;)b[t] && b[t].isActive ? b[t].isActive = !1 : (b[t]?.destroy(), b.splice(t, 1)); y.dataLabel = b[0], y.dataLabels = b })), P(this, "afterDrawDataLabels") } function p(t, e, i, s, r, o) { let a = this.chart, n = e.align, h = e.verticalAlign, l = !t.box && t.padding || 0, d = a.inverted ? this.yAxis : this.xAxis, c = d ? d.left - a.plotLeft : 0, p = a.inverted ? this.xAxis : this.yAxis, u = p ? p.top - a.plotTop : 0, { x: g = 0, y: f = 0 } = e, m, x; return (m = (i.x || 0) + l + c) < 0 && ("right" === n && 0 <= g ? (e.align = "left", e.inside = !0) : g -= m, x = !0), (m = (i.x || 0) + s.width - l + c) > a.plotWidth && ("left" === n && g <= 0 ? (e.align = "right", e.inside = !0) : g += a.plotWidth - m, x = !0), (m = i.y + l + u) < 0 && ("bottom" === h && 0 <= f ? (e.verticalAlign = "top", e.inside = !0) : f -= m, x = !0), (m = (i.y || 0) + s.height - l + u) > a.plotHeight && ("top" === h && f <= 0 ? (e.verticalAlign = "bottom", e.inside = !0) : f += a.plotHeight - m, x = !0), x && (e.x = g, e.y = f, t.placed = !o, t.align(e, void 0, r)), x } function u(e, i) { let t = [], s; if (r(e) && !r(i)) t = e.map(function (t) { return S(t, i) }); else if (r(i) && !r(e)) t = i.map(function (t) { return S(e, t) }); else if (r(e) || r(i)) { if (r(e) && r(i)) for (s = Math.max(e.length, i.length); s--;)t[s] = S(e[s], i[s]) } else t = S(e, i); return t } function g(t) { var e = t.chart.options.plotOptions; return h(u(u(e?.series?.dataLabels, e?.[t.type]?.dataLabels), t.options.dataLabels)) } function f(t, e, i, s, r) { var o = this.chart, a = o.inverted, n = this.xAxis, h = n.reversed, l = ((a ? e.height : e.width) || 0) / 2, t = t.pointWidth, t = t ? t / 2 : 0; e.startXPos = a ? r.x : h ? -l - t : n.width - l + t, e.startYPos = a ? h ? this.yAxis.height - l + t : -l - t : r.y, s ? "hidden" === e.visibility && (e.show(), e.attr({ opacity: 0 }).animate({ opacity: 1 })) : e.attr({ opacity: 1 }).animate({ opacity: 0 }, void 0, e.hide), o.hasRendered && (i && e.attr({ x: e.startXPos, y: e.startYPos }), e.placed = !0) } }), e(t, "Series/Column/ColumnDataLabel.js", [t["Core/Series/DataLabel.js"], t["Core/Globals.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (e, t, i, s) { var r; let o = t["composed"], c = i["series"], { merge: p, pick: u, pushUnique: a } = s; return (r || (r = {})).compose = function (t) { e.compose(c), a(o, "ColumnDataLabel") && (t.prototype.alignDataLabel = n) }, r; function n(t, e, i, s, r) { var o = this.chart.inverted, a = t.series, n = (a.xAxis ? a.xAxis.len : this.chart.plotSizeX) || 0, a = (a.yAxis ? a.yAxis.len : this.chart.plotSizeY) || 0, h = t.dlBox || t.shapeArgs, l = u(t.below, t.plotY > u(this.translatedThreshold, a)), d = u(i.inside, !!this.options.stacking); h && (s = p(h), "allow" === i.overflow && !1 === i.crop || (s.y < 0 && (s.height += s.y, s.y = 0), 0 < (h = s.y + s.height - a) && h < s.height - 1 && (s.height -= h)), o && (s = { x: a - s.y - s.height, y: n - s.x - s.width, width: s.height, height: s.width }), d || (o ? (s.x += l ? 0 : s.width, s.width = 0) : (s.y += l ? s.height : 0, s.height = 0))), i.align = u(i.align, !o || d ? "center" : l ? "right" : "left"), i.verticalAlign = u(i.verticalAlign, o || d ? "middle" : l ? "top" : "bottom"), c.prototype.alignDataLabel.call(this, t, e, i, s, r), i.inside && t.contrastColor && e.css({ color: t.contrastColor }) } }), e(t, "Series/Bar/BarSeries.js", [t["Series/Column/ColumnSeries.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let { extend: s, merge: r } = i; class o extends t { } return o.defaultOptions = r(t.defaultOptions, {}), s(o.prototype, { inverted: !0 }), e.registerSeriesType("bar", o), o }), e(t, "Series/Scatter/ScatterSeriesDefaults.js", [], function () { return { lineWidth: 0, findNearestPointBy: "xy", jitter: { x: 0, y: 0 }, marker: { enabled: !0 }, tooltip: { headerFormat: '<span style="color:{point.color}">●</span> <span style="font-size: 0.8em"> {series.name}</span><br/>', pointFormat: "x: <b>{point.x}</b><br/>y: <b>{point.y}</b><br/>" } } }), e(t, "Series/Scatter/ScatterSeries.js", [t["Series/Scatter/ScatterSeriesDefaults.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let { column: s, line: r } = e.seriesTypes, { addEvent: o, extend: a, merge: n } = i; class h extends r { applyJitter() { let h = this, l = this.options.jitter, d = this.points.length; l && this.points.forEach(function (a, n) { ["x", "y"].forEach(function (t, e) { var i, s, r, o; l[t] && !a.isNull && (i = "plot" + t.toUpperCase(), o = h[t + "Axis"], s = l[t] * o.transA, o && !o.logarithmic && (r = Math.max(0, (a[i] || 0) - s), o = Math.min(o.len, (a[i] || 0) + s), a[i] = r + (o - r) * function (t) { t = 1e4 * Math.sin(t); return t - Math.floor(t) }(n + e * d), "x" === t && (a.clientX = a.plotX))) }) }) } drawGraph() { this.options.lineWidth ? super.drawGraph() : this.graph && (this.graph = this.graph.destroy()) } } return h.defaultOptions = n(r.defaultOptions, t), a(h.prototype, { drawTracker: s.prototype.drawTracker, sorted: !1, requireSorting: !1, noSharedTooltip: !0, trackerGroups: ["group", "markerGroup", "dataLabelsGroup"] }), o(h, "afterTranslate", function () { this.applyJitter() }), e.registerSeriesType("scatter", h), h }), e(t, "Series/CenteredUtilities.js", [t["Core/Globals.js"], t["Core/Series/Series.js"], t["Core/Utilities.js"]], function (t, g, e) { let i = t["deg2rad"], { fireEvent: f, isNumber: m, pick: x, relativeLength: y } = e; return t = { getCenter: function () { let t = this.options, e = this.chart, i = 2 * (t.slicedOffset || 0), s = e.plotWidth - 2 * i, r = e.plotHeight - 2 * i, o = t.center, a = Math.min(s, r), n = t.thickness, h, l = t.size, d = t.innerSize || 0, c, p, u = ("string" == typeof l && (l = parseFloat(l)), "string" == typeof d && (d = parseFloat(d)), [x(o[0], "50%"), x(o[1], "50%"), x(l && l < 0 ? void 0 : t.size, "100%"), x(d && d < 0 ? void 0 : t.innerSize || 0, "0%")]); for (!e.angular || this instanceof g || (u[3] = 0), c = 0; c < 4; ++c)p = u[c], h = c < 2 || 2 === c && /%$/.test(p), u[c] = y(p, [s, r, a, u[2]][c]) + (h ? i : 0); return u[3] > u[2] && (u[3] = u[2]), m(n) && 2 * n < u[2] && 0 < n && (u[3] = u[2] - 2 * n), f(this, "afterGetCenter", { positions: u }), u }, getStartAndEndRadians: function (t, e) { t = m(t) ? t : 0, e = m(e) && t < e && e - t < 360 ? e : t + 360; return { start: i * (t + -90), end: i * (e + -90) } } }, t }), e(t, "Series/Pie/PiePoint.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Series/Point.js"], t["Core/Utilities.js"]], function (t, e, i) { let r = t["setAnimation"], { addEvent: s, defined: o, extend: a, isNumber: n, pick: h, relativeLength: x } = i; class l extends e { getConnectorPath(t) { let e = t.dataLabelPosition, i = t.options || {}, s = i.connectorShape, r = this.connectorShapes[s] || s; return e && r.call(this, { ...e.computed, alignment: e.alignment }, e.connectorPosition, i) || [] } getTranslate() { return this.sliced && this.slicedTranslation || { translateX: 0, translateY: 0 } } haloPath(t) { var e = this.shapeArgs; return this.sliced || !this.visible ? [] : this.series.chart.renderer.symbols.arc(e.x, e.y, e.r + t, e.r + t, { innerR: e.r - 1, start: e.start, end: e.end, borderRadius: e.borderRadius }) } constructor(t, e, i) { super(t, e, i), this.half = 0, this.name ?? (this.name = "Slice"); t = t => { this.slice("select" === t.type) }; s(this, "select", t), s(this, "unselect", t) } isValid() { return n(this.y) && 0 <= this.y } setVisible(t, e = !0) { t !== this.visible && this.update({ visible: t ?? !this.visible }, e, void 0, !1) } slice(t, e, i) { let s = this.series; r(i, s.chart), e = h(e, !0), this.sliced = this.options.sliced = t = o(t) ? t : !this.sliced, s.options.data[s.data.indexOf(this)] = this.options, this.graphic && this.graphic.animate(this.getTranslate()) } } return a(l.prototype, { connectorShapes: { fixedOffset: function (t, e, i) { var s = e.breakAt, e = e.touchingSliceAt, i = i.softConnector ? ["C", t.x + ("left" === t.alignment ? -5 : 5), t.y, 2 * s.x - e.x, 2 * s.y - e.y, s.x, s.y] : ["L", s.x, s.y]; return [["M", t.x, t.y], i, ["L", e.x, e.y]] }, straight: function (t, e) { e = e.touchingSliceAt; return [["M", t.x, t.y], ["L", e.x, e.y]] }, crookedLine: function (t, e, i) { let { breakAt: s, touchingSliceAt: r } = e, o = this["series"], [a, n, h] = o.center, l = h / 2, { plotLeft: d, plotWidth: c } = o.chart, p = "left" === t.alignment, { x: u, y: g } = t, f = s.x; f = i.crookDistance ? (e = x(i.crookDistance, 1), p ? a + l + (c + d - a - l) * (1 - e) : d + (a - l) * e) : a + (n - g) * Math.tan((this.angle || 0) - Math.PI / 2); let m = [["M", u, g]]; return (p ? f <= u && f >= s.x : f >= u && f <= s.x) && m.push(["L", f, g]), m.push(["L", s.x, s.y], ["L", r.x, r.y]), m } } }), l }), e(t, "Series/Pie/PieSeriesDefaults.js", [], function () { return { borderRadius: 3, center: [null, null], clip: !1, colorByPoint: !0, dataLabels: { connectorPadding: 5, connectorShape: "crookedLine", crookDistance: void 0, distance: 30, enabled: !0, formatter: function () { return this.point.isNull ? void 0 : this.point.name }, softConnector: !0, x: 0 }, fillColor: void 0, ignoreHiddenPoint: !0, inactiveOtherPoints: !0, legendType: "point", marker: null, size: null, showInLegend: !1, slicedOffset: 10, stickyTracking: !1, tooltip: { followPointer: !0 }, borderColor: "#ffffff", borderWidth: 1, lineWidth: void 0, states: { hover: { brightness: .1 } } } }), e(t, "Series/Pie/PieSeries.js", [t["Series/CenteredUtilities.js"], t["Series/Column/ColumnSeries.js"], t["Core/Globals.js"], t["Series/Pie/PiePoint.js"], t["Series/Pie/PieSeriesDefaults.js"], t["Core/Series/Series.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Renderer/SVG/Symbols.js"], t["Core/Utilities.js"]], function (t, e, i, s, r, o, a, n, h) { let y = t["getStartAndEndRadians"], l = i["noop"], { clamp: d, extend: c, fireEvent: b, merge: p, pick: u } = h; class g extends o { animate(t) { let s = this, e = s.points, r = s.startAngleRad; t || e.forEach(function (t) { let e = t.graphic, i = t.shapeArgs; e && i && (e.attr({ r: u(t.startR, s.center && s.center[3] / 2), start: r, end: r }), e.animate({ r: i.r, start: i.start, end: i.end }, s.options.animation)) }) } drawEmpty() { var t, e, i = this.startAngleRad, s = this.endAngleRad, r = this.options; 0 === this.total && this.center ? (t = this.center[0], e = this.center[1], this.graph || (this.graph = this.chart.renderer.arc(t, e, this.center[1] / 2, 0, i, s).addClass("highcharts-empty-series").add(this.group)), this.graph.attr({ d: n.arc(t, e, this.center[2] / 2, 0, { start: i, end: s, innerR: this.center[3] / 2 }) }), this.chart.styledMode || this.graph.attr({ "stroke-width": r.borderWidth, fill: r.fillColor || "none", stroke: r.color || "#cccccc" })) : this.graph && (this.graph = this.graph.destroy()) } drawPoints() { let e = this.chart.renderer; this.points.forEach(function (t) { t.graphic && t.hasNewShapeType() && (t.graphic = t.graphic.destroy()), t.graphic || (t.graphic = e[t.shapeType](t.shapeArgs).add(t.series.group), t.delayedRendering = !0) }) } generatePoints() { super.generatePoints(), this.updateTotals() } getX(t, e, i, s) { var r = this.center, i = this.radii ? this.radii[i.index] || 0 : r[2] / 2, o = s.dataLabelPosition?.distance || 0, t = Math.asin(d((t - r[1]) / (i + o), -1, 1)); return r[0] + Math.cos(t) * (i + o) * (e ? -1 : 1) + (0 < o ? (e ? -1 : 1) * (s.padding || 0) : 0) } hasData() { return !!this.processedXData.length } redrawPoints() { let i, s, r, o, a = this, n = a.chart; this.drawEmpty(), a.group && !n.styledMode && a.group.shadow(a.options.shadow), a.points.forEach(function (t) { var e = {}; s = t.graphic, !t.isNull && s ? (o = t.shapeArgs, i = t.getTranslate(), n.styledMode || (r = a.pointAttribs(t, t.selected && "select")), t.delayedRendering ? (s.setRadialReference(a.center).attr(o).attr(i), n.styledMode || s.attr(r).attr({ "stroke-linejoin": "round" }), t.delayedRendering = !1) : (s.setRadialReference(a.center), n.styledMode || p(!0, e, r), p(!0, e, o, i), s.animate(e)), s.attr({ visibility: t.visible ? "inherit" : "hidden" }), s.addClass(t.getClassName(), !0)) : s && (t.graphic = s.destroy()) }) } sortByAngle(t, i) { t.sort(function (t, e) { return void 0 !== t.angle && (e.angle - t.angle) * i }) } translate(t) { b(this, "translate"), this.generatePoints(); let e = this.options, i = e.slicedOffset, s = y(e.startAngle, e.endAngle), r = this.startAngleRad = s.start, o = (this.endAngleRad = s.end) - r, a = this.points, n = e.ignoreHiddenPoint, h = a.length, l, d, c, p, u, g, f, m = 0; for (t || (this.center = t = this.getCenter()), g = 0; g < h; g++) { f = a[g], l = r + m * o, !f.isValid() || n && !f.visible || (m += f.percentage / 100), d = r + m * o; var x = { x: t[0], y: t[1], r: t[2] / 2, innerR: t[3] / 2, start: Math.round(1e3 * l) / 1e3, end: Math.round(1e3 * d) / 1e3 }; f.shapeType = "arc", f.shapeArgs = x, (c = (d + l) / 2) > 1.5 * Math.PI ? c -= 2 * Math.PI : c < -Math.PI / 2 && (c += 2 * Math.PI), f.slicedTranslation = { translateX: Math.round(Math.cos(c) * i), translateY: Math.round(Math.sin(c) * i) }, p = Math.cos(c) * t[2] / 2, u = Math.sin(c) * t[2] / 2, f.tooltipPos = [t[0] + .7 * p, t[1] + .7 * u], f.half = c < -Math.PI / 2 || c > Math.PI / 2 ? 1 : 0, f.angle = c } b(this, "afterTranslate") } updateTotals() { let t = this.points, e = t.length, i = this.options.ignoreHiddenPoint, s, r, o = 0; for (s = 0; s < e; s++)!(r = t[s]).isValid() || i && !r.visible || (o += r.y); for (s = 0, this.total = o; s < e; s++)(r = t[s]).percentage = 0 < o && (r.visible || !i) ? r.y / o * 100 : 0, r.total = o } } return g.defaultOptions = p(o.defaultOptions, r), c(g.prototype, { axisTypes: [], directTouch: !0, drawGraph: void 0, drawTracker: e.prototype.drawTracker, getCenter: t.getCenter, getSymbol: l, invertible: !1, isCartesian: !1, noSharedTooltip: !0, pointAttribs: e.prototype.pointAttribs, pointClass: s, requireSorting: !1, searchPoint: l, trackerGroups: ["group", "dataLabelsGroup"] }), a.registerSeriesType("pie", g), g }), e(t, "Series/Pie/PieDataLabel.js", [t["Core/Series/DataLabel.js"], t["Core/Globals.js"], t["Core/Renderer/RendererUtilities.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s, r) { var o; let { composed: a, noop: n } = e, c = i["distribute"], p = s["series"], { arrayMax: u, clamp: h, defined: w, pick: A, pushUnique: l, relativeLength: g } = r; { e = o = o || {}; let i = { radialDistributionY: function (t, e) { return (e.dataLabelPosition?.top || 0) + t.distributeBox.pos }, radialDistributionX: function (t, e, i, s, r) { var o = r.dataLabelPosition; return t.getX(i < (o?.top || 0) + 2 || i > (o?.bottom || 0) - 2 ? s : i, e.half, e, r) }, justify: function (t, e, i, s) { return s[0] + (t.half ? -1 : 1) * (i + (e.dataLabelPosition?.distance || 0)) }, alignToPlotEdges: function (t, e, i, s) { t = t.getBBox().width; return e ? t + s : i - t - s }, alignToConnectors: function (t, e, i, s) { let r = 0, o; return t.forEach(function (t) { (o = t.dataLabel.getBBox().width) > r && (r = o) }), e ? r + s : i - r - s } }; function d(t, e) { var { center: i, options: s } = this, r = i[2] / 2, o = t.angle || 0, a = Math.cos(o), o = Math.sin(o), n = i[0] + a * r, i = i[1] + o * r, r = Math.min((s.slicedOffset || 0) + (s.borderWidth || 0), e / 5); return { natural: { x: n + a * e, y: i + o * e }, computed: {}, alignment: e < 0 ? "center" : t.half ? "right" : "left", connectorPosition: { breakAt: { x: n + a * r, y: i + o * r }, touchingSliceAt: { x: n, y: i } }, distance: e } } function f() { let f = this, t = f.points, a = f.chart, m = a.plotWidth, x = a.plotHeight, y = a.plotLeft, o = Math.round(a.chartWidth / 3), b = f.center, v = b[2] / 2, n = b[1], h = [[], []], S = [0, 0, 0, 0], k = f.dataLabelPositioners, l, C, M, d = 0; f.visible && f.hasDataLabels?.() && (t.forEach(t => { (t.dataLabels || []).forEach(t => { t.shortened && (t.attr({ width: "auto" }).css({ width: "auto", textOverflow: "clip" }), t.shortened = !1) }) }), p.prototype.drawDataLabels.apply(f), t.forEach(r => { (r.dataLabels || []).forEach((t, e) => { var i = b[2] / 2, s = t.options, i = g(s?.distance || 0, i); 0 === e && h[r.half].push(r), !w(s?.style?.width) && t.getBBox().width > o && (t.css({ width: Math.round(.7 * o) + "px" }), t.shortened = !0), t.dataLabelPosition = this.getDataLabelPosition(r, i), d = Math.max(d, i) }) }), h.forEach((p, u) => { let t = p.length, g = [], e, i, s = 0, r; t && (f.sortByAngle(p, u - .5), 0 < d && (e = Math.max(0, n - v - d), i = Math.min(n + v + d, a.plotHeight), p.forEach(i => { (i.dataLabels || []).forEach(t => { let e = t.dataLabelPosition; e && 0 < e.distance && (e.top = Math.max(0, n - v - e.distance), e.bottom = Math.min(n + v + e.distance, a.plotHeight), s = t.getBBox().height || 21, t.lineHeight = a.renderer.fontMetrics(t.text || t).h + 2 * t.padding, i.distributeBox = { target: (t.dataLabelPosition?.natural.y || 0) - e.top + t.lineHeight / 2, size: s, rank: i.y }, g.push(i.distributeBox)) }) }), c(g, r = i + s - e, r / 5)), p.forEach(c => { (c.dataLabels || []).forEach(e => { let t = e.options || {}, i = c.distributeBox, s = e.dataLabelPosition, r = s?.natural.y || 0, o = t.connectorPadding || 0, a = e.lineHeight || 21, n = (a - e.getBBox().height) / 2, h = 0, l = r, d = "inherit"; if (s) { if (g && w(i) && 0 < s.distance && (void 0 === i.pos ? d = "hidden" : (M = i.size, l = k.radialDistributionY(c, e))), t.justify) h = k.justify(c, e, v, b); else switch (t.alignTo) { case "connectors": h = k.alignToConnectors(p, u, m, y); break; case "plotEdges": h = k.alignToPlotEdges(e, u, m, y); break; default: h = k.radialDistributionX(f, c, l - n, r, e) }if (s.attribs = { visibility: d, align: s.alignment }, s.posAttribs = { x: h + (t.x || 0) + ({ left: o, right: -o }[s.alignment] || 0), y: l + (t.y || 0) - a / 2 }, s.computed.x = h, s.computed.y = l - n, A(t.crop, !0)) { let t; h - (C = e.getBBox().width) < o && 1 === u ? (t = Math.round(C - h + o), S[3] = Math.max(t, S[3])) : h + C > m - o && 0 === u && (t = Math.round(h + C - m + o), S[1] = Math.max(t, S[1])), l - M / 2 < 0 ? S[0] = Math.max(Math.round(-l + M / 2), S[0]) : l + M / 2 > x && (S[2] = Math.max(Math.round(l + M / 2 - x), S[2])), s.sideOverflow = t } } }) })) }), 0 !== u(S) && !this.verifyDataLabelOverflow(S) || (this.placeDataLabels(), this.points.forEach(o => { (o.dataLabels || []).forEach(t => { var e, { connectorColor: i, connectorWidth: s = 1 } = t.options || {}, r = t.dataLabelPosition; s && (l = t.connector, r && 0 < r.distance ? (e = !l, l || (t.connector = l = a.renderer.path().addClass("highcharts-data-label-connector  highcharts-color-" + o.colorIndex + (o.className ? " " + o.className : "")).add(f.dataLabelsGroup)), a.styledMode || l.attr({ "stroke-width": s, stroke: i || o.color || "#666666" }), l[e ? "attr" : "animate"]({ d: o.getConnectorPath(t) }), l.attr({ visibility: r.attribs?.visibility })) : l && (t.connector = l.destroy())) }) }))) } function m() { this.points.forEach(t => { (t.dataLabels || []).forEach(t => { var e = t.dataLabelPosition; e ? (e.sideOverflow && (t.css({ width: Math.max(t.getBBox().width - e.sideOverflow, 0) + "px", textOverflow: (t.options?.style || {}).textOverflow || "ellipsis" }), t.shortened = !0), t.attr(e.attribs), t[t.moved ? "animate" : "attr"](e.posAttribs), t.moved = !0) : t && t.attr({ y: -9999 }) }), delete t.distributeBox }, this) } function x(t) { let e = this.center, i = this.options, s = i.center, r = i.minSize || 80, o = r, a = null !== i.size; return a || (null !== s[0] ? o = Math.max(e[2] - Math.max(t[1], t[3]), r) : (o = Math.max(e[2] - t[1] - t[3], r), e[0] += (t[3] - t[1]) / 2), null !== s[1] ? o = h(o, r, e[2] - Math.max(t[0], t[2])) : (o = h(o, r, e[2] - t[0] - t[2]), e[1] += (t[0] - t[2]) / 2), o < e[2] ? (e[2] = o, e[3] = Math.min(i.thickness ? Math.max(0, o - 2 * i.thickness) : Math.max(0, g(i.innerSize || 0, o)), o), this.translate(e), this.drawDataLabels && this.drawDataLabels()) : a = !0), a } e.compose = function (e) { if (t.compose(p), l(a, "PieDataLabel")) { let t = e.prototype; t.dataLabelPositioners = i, t.alignDataLabel = n, t.drawDataLabels = f, t.getDataLabelPosition = d, t.placeDataLabels = m, t.verifyDataLabelOverflow = x } } } return o }), e(t, "Core/Geometry/GeometryUtilities.js", [], function () { var t; t = { getCenterOfPoints: function (t) { var e = t.reduce((t, e) => (t.x += e.x, t.y += e.y, t), { x: 0, y: 0 }); return { x: e.x / t.length, y: e.y / t.length } }, getDistanceBetweenPoints: function (t, e) { return Math.sqrt(Math.pow(e.x - t.x, 2) + Math.pow(e.y - t.y, 2)) }, getAngleBetweenPoints: function (t, e) { return Math.atan2(e.x - t.x, e.y - t.y) }, pointInPolygon: function ({ x: t, y: e }, i) { let s = i.length, r, o, a = !1; for (r = 0, o = s - 1; r < s; o = r++) { var [n, h] = i[r], [l, d] = i[o]; e < h != e < d && t < (l - n) * (e - h) / (d - h) + n && (a = !a) } return a } }; return t }), e(t, "Extensions/OverlappingDataLabels.js", [t["Core/Geometry/GeometryUtilities.js"], t["Core/Utilities.js"]], function (t, e) { let p = t["pointInPolygon"], { addEvent: i, fireEvent: u, objectEach: a, pick: n } = e; function s(i) { let s = i.length, e, r, o, a, n, t = !1; for (let t = 0; t < s; t++)(e = i[t]) && (e.oldOpacity = e.opacity, e.newOpacity = 1, e.absoluteBox = function (t) { var e, i, s; if (t && (!t.alignAttr || t.placed)) return e = !t.box && t.padding || 0, i = t.alignAttr || { x: t.attr("x"), y: t.attr("y") }, s = t.getBBox(), t.width = s.width, t.height = s.height, { x: i.x + (t.parentGroup?.translateX || 0) + e, y: i.y + (t.parentGroup?.translateY || 0) + e, width: (t.width || 0) - 2 * e, height: (t.height || 0) - 2 * e, polygon: s?.polygon } }(e)); i.sort((t, e) => (e.labelrank || 0) - (t.labelrank || 0)); for (let t = 0; t < s; ++t) { var h = (a = (r = i[t]) && r.absoluteBox)?.polygon; for (let e = t + 1; e < s; ++e) { n = (o = i[e]) && o.absoluteBox; let t = !1; if (a && n && r !== o && 0 !== r.newOpacity && 0 !== o.newOpacity && "hidden" !== r.visibility && "hidden" !== o.visibility) { var l = n.polygon; if (h && l && h !== l ? ((t, e) => { for (var i of t) if (p({ x: i[0], y: i[1] }, e)) return !0; return !1 })(h, l) && (t = !0) : (l = a, (d = n).x >= l.x + l.width || d.x + d.width <= l.x || d.y >= l.y + l.height || d.y + d.height <= l.y || (t = !0)), t) { let t = r.labelrank < o.labelrank ? r : o, e = t.text; t.newOpacity = 0, e?.element.querySelector("textPath") && e.hide() } } } } var d, c; for (c of i) g(c, this) && (t = !0); t && u(this, "afterHideAllOverlappingLabels") } function g(t, e) { let i, s = !1; return t && (i = t.newOpacity, t.oldOpacity !== i && (t.hasClass("highcharts-data-label") ? (t[i ? "removeClass" : "addClass"]("highcharts-data-label-hidden"), s = !0, t[t.isOld ? "animate" : "attr"]({ opacity: i }, void 0, function () { e.styledMode || t.css({ pointerEvents: i ? "auto" : "none" }) }), u(e, "afterHideOverlappingLabel")) : t.attr({ opacity: i })), t.isOld = !0), s } function r() { let s = this, r = []; for (var t of s.labelCollectors || []) r = r.concat(t()); for (var e of s.yAxis || []) e.stacking && e.options.stackLabels && !e.options.stackLabels.allowOverlap && a(e.stacking.stacks, t => { a(t, t => { t.label && r.push(t.label) }) }); for (var i of s.series || []) { var o; i.visible && i.hasDataLabels?.() && ((o = t => { for (let i of t) i.visible && (i.dataLabels || []).forEach(t => { var e = t.options || {}; t.labelrank = n(e.labelrank, i.labelrank, i.shapeArgs?.height), e.allowOverlap ?? 0 < Number(e.distance) ? (t.oldOpacity = t.opacity, t.newOpacity = 1, g(t, s)) : r.push(t) }) })(i.nodes || []), o(i.points)) } this.hideOverlappingLabels(r) } return { compose: function (t) { let e = t.prototype; e.hideOverlappingLabels || (e.hideOverlappingLabels = s, i(t, "render", r)) } } }), e(t, "Extensions/BorderRadius.js", [t["Core/Defaults.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, e, i) { let s = t["defaultOptions"], r = e["noop"], { addEvent: o, extend: m, isObject: a, merge: n, relativeLength: x } = i, h = { radius: 0, scope: "stack", where: void 0 }, c = r, M = r; function l(t, e, i, s, r = {}) { var o = c(t, e, i, s, r), { innerR: t = 0, r: e = i, start: s = 0, end: i = 0 } = r; if (r.open || !r.borderRadius) return o; let a = i - s, n = Math.sin(a / 2), h = Math.max(Math.min(x(r.borderRadius || 0, e - t), (e - t) / 2, e * n / (1 + n)), 0), l = Math.min(h, a / Math.PI * 2 * t), d = o.length - 1; for (; d--;) { var g = o; var f = d; var m = 1 < d ? l : h; let c, p, u, t = g[f], e = g[f + 1]; if ("Z" === e[0] && (e = g[0]), "M" !== t[0] && "L" !== t[0] || "A" !== e[0] ? "A" !== t[0] || "M" !== e[0] && "L" !== e[0] || (c = e, p = t) : (c = t, p = e, u = !0), c && p && p.params) { let t = p[1], e = p[5], i = p.params, { start: s, end: r, cx: o, cy: a } = i, n = e ? t - m : t + m, h = n ? Math.asin(m / n) : 0, l = e ? h : -h, d = Math.cos(h) * n; u ? (i.start = s + l, c[1] = o + d * Math.cos(s), c[2] = a + d * Math.sin(s), g.splice(f + 1, 0, ["A", m, m, 0, 0, 1, o + t * Math.cos(i.start), a + t * Math.sin(i.start)])) : (i.end = r - l, p[6] = o + t * Math.cos(i.end), p[7] = a + t * Math.sin(i.end), g.splice(f + 1, 0, ["A", m, m, 0, 0, 1, o + d * Math.cos(r), a + d * Math.sin(r)])), p[4] = Math.abs(i.end - i.start) < Math.PI ? 0 : 1 } } return o } function d() { if (this.options.borderRadius && (!this.chart.is3d || !this.chart.is3d())) { let { options: n, yAxis: h } = this, l = "percent" === n.stacking, t = s.plotOptions?.[this.type]?.borderRadius, d = y(n.borderRadius, a(t) ? t : {}), c = h.options.reversed; for (var p of this.points) { var u, g = p["shapeArgs"]; if ("roundedRect" === p.shapeType && g) { let { width: t = 0, height: e = 0, y: i = 0 } = g, s = i, r = e; "stack" === d.scope && p.stackTotal && (u = h.translate(l ? 100 : p.stackTotal, !1, !0, !1, !0), f = h.translate(n.threshold || 0, !1, !0, !1, !0), u = this.crispCol(0, Math.min(u, f), 0, Math.abs(u - f)), s = u.y, r = u.height); let o = (p.negative ? -1 : 1) * (c ? -1 : 1) == -1, a = d.where; a = (a = !a && this.is("waterfall") && Math.abs((p.yBottom || 0) - (this.translatedThreshold || 0)) > this.borderWidth ? "all" : a) || "end"; var f = Math.min(x(d.radius, t), t / 2, "all" === a ? e / 2 : 1 / 0) || 0; "end" === a && (o && (s -= f), r += f), m(g, { brBoxHeight: r, brBoxY: s, r: f }) } } } } function y(t, e) { return a(t) || (t = { radius: t || 0 }), n(h, e, t) } function p() { var e, i = y(this.options.borderRadius); for (e of this.points) { let t = e.shapeArgs; t && (t.borderRadius = x(i.radius, (t.r || 0) - (t.innerR || 0))) } } function u(t, e, i, s, r = {}) { let o = M(t, e, i, s, r), { r: a = 0, brBoxHeight: n = s, brBoxY: h = e } = r, l = e - h, d = h + n - (e + s), c = -.1 < l - a ? 0 : a, p = -.1 < d - a ? 0 : a, u = Math.max(c && l, 0), g = Math.max(p && d, 0), f = [t + c, e], m = [t + i - c, e], x = [t + i, e + c], y = [t + i, e + s - p], b = [t + i - p, e + s], v = [t + p, e + s], S = [t, e + s - p], k = [t, e + c], C = (t, e) => Math.sqrt(Math.pow(t, 2) - Math.pow(e, 2)); return u && (r = C(c, c - u), f[0] -= r, m[0] += r, x[1] = k[1] = e + c - u), s < c - u && (r = C(c, c - u - s), x[0] = y[0] = t + i - c + r, b[0] = Math.min(x[0], b[0]), v[0] = Math.max(y[0], v[0]), S[0] = k[0] = t + c - r, x[1] = k[1] = e + s), g && (r = C(p, p - g), b[0] += r, v[0] -= r, y[1] = S[1] = e + s - p + g), s < p - g && (r = C(p, p - g - s), x[0] = y[0] = t + i - p + r, m[0] = Math.min(x[0], m[0]), f[0] = Math.max(y[0], f[0]), S[0] = k[0] = t + p - r, y[1] = S[1] = e), o.length = 0, o.push(["M", ...f], ["L", ...m], ["A", c, c, 0, 0, 1, ...x], ["L", ...y], ["A", p, p, 0, 0, 1, ...b], ["L", ...v], ["A", p, p, 0, 0, 1, ...S], ["L", ...k], ["A", c, c, 0, 0, 1, ...f], ["Z"]), o } return { compose: function (e, i, s) { var r = e.types.pie; if (!i.symbolCustomAttribs.includes("borderRadius")) { let t = s.prototype.symbols; o(e, "afterColumnTranslate", d, { order: 9 }), o(r, "afterTranslate", p), i.symbolCustomAttribs.push("borderRadius", "brBoxHeight", "brBoxY"), c = t.arc, M = t.roundedRect, t.arc = l, t.roundedRect = u } }, optionsToObject: y } }), e(t, "Core/Responsive.js", [t["Core/Utilities.js"]], function (t) { var e; let { diffObjects: n, extend: i, find: h, merge: l, pick: s, uniqueKey: d } = t; return (e || (e = {})).compose = function (t) { var e = t.prototype; return e.matchResponsiveRule || i(e, { matchResponsiveRule: r, setResponsive: o }), t }, e; function r(t, e) { let i = t.condition; (i.callback || function () { return this.chartWidth <= s(i.maxWidth, Number.MAX_VALUE) && this.chartHeight <= s(i.maxHeight, Number.MAX_VALUE) && this.chartWidth >= s(i.minWidth, 0) && this.chartHeight >= s(i.minHeight, 0) }).call(this) && e.push(t._id) } function o(t, e) { let i = this.options.responsive, s = this.currentResponsive, r = [], o, a = (!e && i && i.rules && i.rules.forEach(t => { void 0 === t._id && (t._id = d()), this.matchResponsiveRule(t, r) }, this), l(...r.map(e => h((i || {}).rules || [], t => t._id === e)).map(t => t && t.chartOptions))); a.isResponsiveOptions = !0, r = r.toString() || void 0; e = s && s.ruleIds; r !== e && (s && (this.currentResponsive = void 0, this.updatingResponsive = !0, this.update(s.undoOptions, t, !0), this.updatingResponsive = !1), r ? ((o = n(a, this.options, !0, this.collectionsWithUpdate)).isResponsiveOptions = !0, this.currentResponsive = { ruleIds: r, mergedOptions: a, undoOptions: o }, this.updatingResponsive || this.update(a, t, !0)) : this.currentResponsive = void 0) } }), e(t, "masters/highcharts.src.js", [t["Core/Globals.js"], t["Core/Utilities.js"], t["Core/Defaults.js"], t["Core/Animation/Fx.js"], t["Core/Animation/AnimationUtilities.js"], t["Core/Renderer/HTML/AST.js"], t["Core/Templating.js"], t["Core/Renderer/RendererRegistry.js"], t["Core/Renderer/RendererUtilities.js"], t["Core/Renderer/SVG/SVGElement.js"], t["Core/Renderer/SVG/SVGRenderer.js"], t["Core/Renderer/HTML/HTMLElement.js"], t["Core/Axis/Axis.js"], t["Core/Axis/DateTimeAxis.js"], t["Core/Axis/LogarithmicAxis.js"], t["Core/Axis/PlotLineOrBand/PlotLineOrBand.js"], t["Core/Axis/Tick.js"], t["Core/Tooltip.js"], t["Core/Series/Point.js"], t["Core/Pointer.js"], t["Core/Legend/Legend.js"], t["Core/Legend/LegendSymbol.js"], t["Core/Chart/Chart.js"], t["Extensions/ScrollablePlotArea.js"], t["Core/Axis/Stacking/StackingAxis.js"], t["Core/Axis/Stacking/StackItem.js"], t["Core/Series/Series.js"], t["Core/Series/SeriesRegistry.js"], t["Series/Column/ColumnDataLabel.js"], t["Series/Pie/PieDataLabel.js"], t["Core/Series/DataLabel.js"], t["Extensions/OverlappingDataLabels.js"], t["Extensions/BorderRadius.js"], t["Core/Responsive.js"], t["Core/Color/Color.js"], t["Core/Time.js"]], function (t, e, i, s, r, o, a, n, h, l, d, c, p, u, g, f, m, x, y, b, v, S, k, C, M, w, A, T, P, L, O, E, D, j, I, B) { return t.AST = o, t.Axis = p, t.Chart = k, t.Color = I, t.DataLabel = O, t.Fx = s, t.HTMLElement = c, t.Legend = v, t.LegendSymbol = S, t.OverlappingDataLabels = t.OverlappingDataLabels || E, t.PlotLineOrBand = f, t.Point = y, t.Pointer = b, t.RendererRegistry = n, t.Series = A, t.SeriesRegistry = T, t.StackItem = w, t.SVGElement = l, t.SVGRenderer = d, t.Templating = a, t.Tick = m, t.Time = B, t.Tooltip = x, t.animate = r.animate, t.animObject = r.animObject, t.chart = k.chart, t.color = I.parse, t.dateFormat = a.dateFormat, t.defaultOptions = i.defaultOptions, t.distribute = h.distribute, t.format = a.format, t.getDeferredAnimation = r.getDeferredAnimation, t.getOptions = i.getOptions, t.numberFormat = a.numberFormat, t.seriesType = T.seriesType, t.setAnimation = r.setAnimation, t.setOptions = i.setOptions, t.stop = r.stop, t.time = i.defaultTime, t.timers = s.timers, D.compose(t.Series, t.SVGElement, t.SVGRenderer), P.compose(t.Series.types.column), O.compose(t.Series), u.compose(t.Axis), c.compose(t.SVGRenderer), v.compose(t.Chart), g.compose(t.Axis), E.compose(t.Chart), L.compose(t.Series.types.pie), f.compose(t.Chart, t.Axis), b.compose(t.Chart), j.compose(t.Chart), C.compose(t.Axis, t.Chart, t.Series), M.compose(t.Axis, t.Chart, t.Series), x.compose(t.Pointer), e.extend(t, e), t }), (t["masters/highcharts.src.js"]._modules = t)["masters/highcharts.src.js"]
});;
!function (e) { "object" == typeof module && module.exports ? module.exports = e.default = e : "function" == typeof define && define.amd ? define("highcharts/highcharts-more", ["highcharts"], function (t) { return e(t), e.Highcharts = t, e }) : e("undefined" != typeof Highcharts ? Highcharts : void 0) }(function (a) { "use strict"; var t = a ? a._modules : {}; function e(t, e, i, s) { t.hasOwnProperty(e) || (t[e] = s.apply(null, i), "function" == typeof CustomEvent && a.win.dispatchEvent(new CustomEvent("HighchartsModuleLoaded", { detail: { path: e, module: t[e] } }))) } e(t, "Extensions/Pane/PaneComposition.js", [t["Core/Utilities.js"]], function (t) { let { addEvent: s, correctFloat: h, defined: p, pick: a } = t; function o(e) { let i, s = this; return e && s.pane.forEach(t => { r(e.chartX - s.plotLeft, e.chartY - s.plotTop, t.center) && (i = t) }), i } function r(t, e, i, s, a) { let o = !0, r = i[0], n = i[1], l = Math.sqrt(Math.pow(t - r, 2) + Math.pow(e - n, 2)); return p(s) && p(a) && (e = Math.atan2(h(e - n, 8), h(t - r, 8)), a !== s && (o = a < s ? s <= e && e <= Math.PI || e <= a && e >= -Math.PI : s <= e && e <= h(a, 8))), l <= Math.ceil(i[2] / 2) && o } function n(e) { this.polar && (e.options.inverted && ([e.x, e.y] = [e.y, e.x]), e.isInsidePlot = this.pane.some(t => r(e.x, e.y, t.center, t.axis && t.axis.normalizedStartAngleRad, t.axis && t.axis.normalizedEndAngleRad))) } function l(t) { var e = this.chart; t.hoverPoint && t.hoverPoint.plotX && t.hoverPoint.plotY && e.hoverPane && !r(t.hoverPoint.plotX, t.hoverPoint.plotY, e.hoverPane.center) && (t.hoverPoint = void 0) } function d(e) { let i = this.chart; i.polar ? (i.hoverPane = i.getHoverPane(e), e.filter = function (t) { return t.visible && !(!e.shared && t.directTouch) && a(t.options.enableMouseTracking, !0) && (!i.hoverPane || t.xAxis.pane === i.hoverPane) }) : i.hoverPane = void 0 } return { compose: function (t, e) { let i = t.prototype; i.getHoverPane || (i.collectionsWithUpdate.push("pane"), i.getHoverPane = o, s(t, "afterIsInsidePlot", n), s(e, "afterGetHoverData", l), s(e, "beforeGetHoverData", d)) } } }), e(t, "Extensions/Pane/PaneDefaults.js", [], function () { return { pane: { center: ["50%", "50%"], size: "85%", innerSize: "0%", startAngle: 0 }, background: { shape: "circle", borderRadius: 0, borderWidth: 1, borderColor: "#cccccc", backgroundColor: { linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 }, stops: [[0, "#ffffff"], [1, "#e6e6e6"]] }, from: -Number.MAX_VALUE, innerRadius: 0, to: Number.MAX_VALUE, outerRadius: "105%" } } }), e(t, "Extensions/Pane/Pane.js", [t["Series/CenteredUtilities.js"], t["Extensions/Pane/PaneComposition.js"], t["Extensions/Pane/PaneDefaults.js"], t["Core/Utilities.js"]], function (e, t, a, i) { let { extend: o, merge: r, splat: n } = i; class s { constructor(t, e) { this.coll = "pane", this.init(t, e) } init(t, e) { this.chart = e, this.background = [], e.pane.push(this), this.setOptions(t) } setOptions(t) { this.options = t = r(a.pane, this.chart.angular ? { background: {} } : void 0, t) } render() { let t = this.options, e = this.chart.renderer; this.group || (this.group = e.g("pane-group").attr({ zIndex: t.zIndex || 0 }).add()), this.updateCenter(); var i = this.options.background; if (i) { var s = Math.max((i = n(i)).length, this.background.length || 0); for (let t = 0; t < s; t++)i[t] && this.axis ? this.renderBackground(r(a.background, i[t]), t) : this.background[t] && (this.background[t] = this.background[t].destroy(), this.background.splice(t, 1)) } } renderBackground(t, e) { let i = { class: "highcharts-pane " + (t.className || "") }, s = "animate"; this.chart.styledMode || o(i, { fill: t.backgroundColor, stroke: t.borderColor, "stroke-width": t.borderWidth }), this.background[e] || (this.background[e] = this.chart.renderer.path().add(this.group), s = "attr"), this.background[e][s]({ d: this.axis.getPlotBandPath(t.from, t.to, t) }).attr(i) } updateCenter(t) { this.center = (t || this.axis || {}).center = e.getCenter.call(this) } update(t, e) { r(!0, this.options, t), this.setOptions(this.options), this.render(), this.chart.axes.forEach(function (t) { t.pane === this && (t.pane = null, t.update({}, e)) }, this) } } return s.compose = t.compose, s }), e(t, "Series/AreaRange/AreaRangePoint.js", [t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e) { let { prototype: { pointClass: i, pointClass: { prototype: a } } } = t.seriesTypes["area"], { defined: o, isNumber: r } = e; return class extends i { setState() { let t = this.state, e = this.series, i = e.chart.polar; o(this.plotHigh) || (this.plotHigh = e.yAxis.toPixels(this.high, !0)), o(this.plotLow) || (this.plotLow = this.plotY = e.yAxis.toPixels(this.low, !0)), e.lowerStateMarkerGraphic = e.stateMarkerGraphic, e.stateMarkerGraphic = e.upperStateMarkerGraphic, this.graphic = this.graphics && this.graphics[1], this.plotY = this.plotHigh, i && r(this.plotHighX) && (this.plotX = this.plotHighX), a.setState.apply(this, arguments), this.state = t, this.plotY = this.plotLow, this.graphic = this.graphics && this.graphics[0], i && r(this.plotLowX) && (this.plotX = this.plotLowX), e.upperStateMarkerGraphic = e.stateMarkerGraphic, e.stateMarkerGraphic = e.lowerStateMarkerGraphic, e.lowerStateMarkerGraphic = void 0; var s = e.modifyMarkerSettings(); a.setState.apply(this, arguments), e.restoreMarkerSettings(s) } haloPath() { let t = this.series.chart.polar, e = []; return this.plotY = this.plotLow, t && r(this.plotLowX) && (this.plotX = this.plotLowX), this.isInside && (e = a.haloPath.apply(this, arguments)), this.plotY = this.plotHigh, t && r(this.plotHighX) && (this.plotX = this.plotHighX), e = this.isTopInside ? e.concat(a.haloPath.apply(this, arguments)) : e } isValid() { return r(this.low) && r(this.high) } } }), e(t, "Series/AreaRange/AreaRangeSeries.js", [t["Series/AreaRange/AreaRangePoint.js"], t["Core/Globals.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s) { let a = e["noop"], { area: o, area: { prototype: m }, column: { prototype: r } } = i.seriesTypes, { addEvent: n, defined: l, extend: g, isArray: b, isNumber: h, pick: y, merge: p } = s; class d extends o { toYData(t) { return [t.low, t.high] } highToXY(t) { var e = this.chart, i = this.xAxis.postTranslate(t.rectPlotX || 0, this.yAxis.len - (t.plotHigh || 0)); t.plotHighX = i.x - e.plotLeft, t.plotHigh = i.y - e.plotTop, t.plotLowX = t.plotX } getGraphPath(t) { let e = [], i = [], s = m.getGraphPath, a = this.options, o = this.chart.polar, r = o && !1 !== a.connectEnds, n = a.connectNulls, l, h, p, d = a.step; for (l = (t = t || this.points).length; l--;) { h = t[l]; var c = o ? { plotX: h.rectPlotX, plotY: h.yBottom, doCurve: !1 } : { plotX: h.plotX, plotY: h.plotY, doCurve: !1 }; h.isNull || r || n || t[l + 1] && !t[l + 1].isNull || i.push(c), p = { polarPlotY: h.polarPlotY, rectPlotX: h.rectPlotX, yBottom: h.yBottom, plotX: y(h.plotHighX, h.plotX), plotY: h.plotHigh, isNull: h.isNull }, i.push(p), e.push(p), h.isNull || r || n || t[l - 1] && !t[l - 1].isNull || i.push(c) } let u = s.call(this, t), g = (d && (!0 === d && (d = "left"), a.step = { left: "right", center: "center", right: "left" }[d]), s.call(this, e)), b = s.call(this, i), f = (a.step = d, [].concat(u, g)); return !this.chart.polar && b[0] && "M" === b[0][0] && (b[0] = ["L", b[0][1], b[0][2]]), this.graphPath = f, this.areaPath = u.concat(b), f.isArea = !0, f.xMap = u.xMap, this.areaPath.xMap = u.xMap, f } drawDataLabels() { let t, e, i, s, a, o = this.points, r = o.length, n = [], l = this.options.dataLabels, h = this.chart.inverted; if (l) { if (b(l) ? (s = l[0] || { enabled: !1 }, a = l[1] || { enabled: !1 }) : ((s = g({}, l)).x = l.xHigh, s.y = l.yHigh, (a = g({}, l)).x = l.xLow, a.y = l.yLow), s.enabled || this.hasDataLabels?.()) { for (t = r; t--;) { var p, d; (e = o[t]) && ({ plotHigh: p = 0, plotLow: d = 0 } = e, i = s.inside ? p < d : d < p, e.y = e.high, e._plotY = e.plotY, e.plotY = p, n[t] = e.dataLabel, e.dataLabel = e.dataLabelUpper, e.below = i, h ? s.align || (s.align = i ? "right" : "left") : s.verticalAlign || (s.verticalAlign = i ? "top" : "bottom")) } for (this.options.dataLabels = s, m.drawDataLabels && m.drawDataLabels.apply(this, arguments), t = r; t--;)(e = o[t]) && (e.dataLabelUpper = e.dataLabel, e.dataLabel = n[t], delete e.dataLabels, e.y = e.low, e.plotY = e._plotY) } if (a.enabled || this.hasDataLabels?.()) { for (t = r; t--;) { var c, u; (e = o[t]) && ({ plotHigh: c = 0, plotLow: u = 0 } = e, i = a.inside ? c < u : u < c, e.below = !i, h ? a.align || (a.align = i ? "left" : "right") : a.verticalAlign || (a.verticalAlign = i ? "bottom" : "top")) } this.options.dataLabels = a, m.drawDataLabels && m.drawDataLabels.apply(this, arguments) } if (s.enabled) for (t = r; t--;)(e = o[t]) && (e.dataLabels = [e.dataLabelUpper, e.dataLabel].filter(function (t) { return !!t })); this.options.dataLabels = l } } alignDataLabel() { r.alignDataLabel.apply(this, arguments) } modifyMarkerSettings() { var t, e, i = { marker: this.options.marker, symbol: this.symbol }; return this.options.lowMarker && ({ marker: t, lowMarker: e } = this["options"], this.options.marker = p(t, e), e.symbol && (this.symbol = e.symbol)), i } restoreMarkerSettings(t) { this.options.marker = t.marker, this.symbol = t.symbol } drawPoints() { let t, e; var i = this.points.length, s = this.modifyMarkerSettings(); for (m.drawPoints.apply(this, arguments), this.restoreMarkerSettings(s), t = 0; t < i;)(e = this.points[t]).graphics = e.graphics || [], e.origProps = { plotY: e.plotY, plotX: e.plotX, isInside: e.isInside, negative: e.negative, zone: e.zone, y: e.y }, (e.graphic || e.graphics[0]) && (e.graphics[0] = e.graphic), e.graphic = e.graphics[1], e.plotY = e.plotHigh, l(e.plotHighX) && (e.plotX = e.plotHighX), e.y = y(e.high, e.origProps.y), e.negative = e.y < (this.options.threshold || 0), this.zones.length && (e.zone = e.getZone()), this.chart.polar || (e.isInside = e.isTopInside = void 0 !== e.plotY && 0 <= e.plotY && e.plotY <= this.yAxis.len && 0 <= e.plotX && e.plotX <= this.xAxis.len), t++; for (m.drawPoints.apply(this, arguments), t = 0; t < i;)(e = this.points[t]).graphics = e.graphics || [], (e.graphic || e.graphics[1]) && (e.graphics[1] = e.graphic), e.graphic = e.graphics[0], e.origProps && (g(e, e.origProps), delete e.origProps), t++ } hasMarkerChanged(t, e) { var i = t.lowMarker, s = e.lowMarker || {}; return i && (!1 === i.enabled || s.symbol !== i.symbol || s.height !== i.height || s.width !== i.width) || super.hasMarkerChanged(t, e) } } return d.defaultOptions = p(o.defaultOptions, { lineWidth: 1, threshold: null, tooltip: { pointFormat: '<span style="color:{series.color}">●</span> {series.name}: <b>{point.low}</b> - <b>{point.high}</b><br/>' }, trackByArea: !0, dataLabels: { align: void 0, verticalAlign: void 0, xLow: 0, xHigh: 0, yLow: 0, yHigh: 0 } }), n(d, "afterTranslate", function () { "low,high" === this.pointArrayMap.join(",") && this.points.forEach(t => { var e = t.high, i = t.plotY; t.isNull ? t.plotY = void 0 : (t.plotLow = i, t.plotHigh = h(e) ? this.yAxis.translate(this.dataModify ? this.dataModify.modifyValue(e) : e, !1, !0, void 0, !0) : void 0, this.dataModify && (t.yBottom = t.plotHigh)) }) }, { order: 0 }), n(d, "afterTranslate", function () { this.points.forEach(i => { if (this.chart.polar) this.highToXY(i), i.plotLow = i.plotY, i.tooltipPos = [((i.plotHighX || 0) + (i.plotLowX || 0)) / 2, ((i.plotHigh || 0) + (i.plotLow || 0)) / 2]; else { let t = i.pos(!1, i.plotLow), e = i.pos(!1, i.plotHigh); t && e && (t[0] = (t[0] + e[0]) / 2, t[1] = (t[1] + e[1]) / 2), i.tooltipPos = t } }) }, { order: 3 }), g(d.prototype, { deferTranslatePolar: !0, pointArrayMap: ["low", "high"], pointClass: t, pointValKey: "low", setStackedPoints: a }), i.registerSeriesType("arearange", d), d }), e(t, "Series/AreaSplineRange/AreaSplineRangeSeries.js", [t["Series/AreaRange/AreaRangeSeries.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let { prototype: s } = e.seriesTypes["spline"], { merge: a, extend: o } = i; class r extends t { } return r.defaultOptions = a(t.defaultOptions), o(r.prototype, { getPointSpline: s.getPointSpline }), e.registerSeriesType("areasplinerange", r), r }), e(t, "Series/BoxPlot/BoxPlotSeriesDefaults.js", [], function () { return { threshold: null, tooltip: { pointFormat: '<span style="color:{point.color}">●</span> <b>{series.name}</b><br/>Maximum: {point.high}<br/>Upper quartile: {point.q3}<br/>Median: {point.median}<br/>Lower quartile: {point.q1}<br/>Minimum: {point.low}<br/>' }, whiskerLength: "50%", fillColor: "#ffffff", lineWidth: 1, medianWidth: 2, whiskerWidth: 2 } }), e(t, "Series/BoxPlot/BoxPlotSeries.js", [t["Series/BoxPlot/BoxPlotSeriesDefaults.js"], t["Series/Column/ColumnSeries.js"], t["Core/Globals.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s, a) { let o = i["noop"], { crisp: A, extend: r, merge: n, pick: w } = a; class l extends e { pointAttribs() { return {} } translate() { let i = this.yAxis, t = this.pointArrayMap; super.translate.apply(this), this.points.forEach(function (e) { t.forEach(function (t) { null !== e[t] && (e[t + "Plot"] = i.translate(e[t], 0, 1, 0, 1)) }), e.plotHigh = e.highPlot }) } drawPoints() { let l, h, p, d, c, u, g, b, f, m, y, t = this.points, x = this.options, P = this.chart, v = P.renderer, S = !1 !== this.doQuartiles, M = this.options.whiskerLength; for (var L of t) { let e = (u = L.graphic) ? "animate" : "attr", i = L.shapeArgs, s = {}, a = {}, o = {}, r = {}, n = L.color || this.color; if (void 0 !== L.plotY) { let t; g = i.width, f = (b = i.x) + g, m = g / 2, l = S ? L.q1Plot : L.lowPlot, h = S ? L.q3Plot : L.lowPlot, p = L.highPlot, d = L.lowPlot, u || (L.graphic = u = v.g("point").add(this.group), L.stem = v.path().addClass("highcharts-boxplot-stem").add(u), M && (L.whiskers = v.path().addClass("highcharts-boxplot-whisker").add(u)), S && (L.box = v.path(void 0).addClass("highcharts-boxplot-box").add(u)), L.medianShape = v.path(void 0).addClass("highcharts-boxplot-median").add(u)), P.styledMode || (a.stroke = L.stemColor || x.stemColor || n, a["stroke-width"] = w(L.stemWidth, x.stemWidth, x.lineWidth), a.dashstyle = L.stemDashStyle || x.stemDashStyle || x.dashStyle, L.stem.attr(a), M && (o.stroke = L.whiskerColor || x.whiskerColor || n, o["stroke-width"] = w(L.whiskerWidth, x.whiskerWidth, x.lineWidth), o.dashstyle = L.whiskerDashStyle || x.whiskerDashStyle || x.dashStyle, L.whiskers.attr(o)), S && (s.fill = L.fillColor || x.fillColor || n, s.stroke = x.lineColor || n, s["stroke-width"] = x.lineWidth || 0, s.dashstyle = L.boxDashStyle || x.boxDashStyle || x.dashStyle, L.box.attr(s)), r.stroke = L.medianColor || x.medianColor || n, r["stroke-width"] = w(L.medianWidth, x.medianWidth, x.lineWidth), r.dashstyle = L.medianDashStyle || x.medianDashStyle || x.dashStyle, L.medianShape.attr(r)); var k, C = A((L.plotX || 0) + (this.pointXOffset || 0) + (this.barW || 0) / 2, L.stem.strokeWidth()); t = [["M", C, h], ["L", C, p], ["M", C, l], ["L", C, d]], L.stem[e]({ d: t }), S && (k = L.box.strokeWidth(), l = A(l, k), h = A(h, k), t = [["M", b = A(b, k), h], ["L", b, l], ["L", f = A(f, k), l], ["L", f, h], ["L", b, h], ["Z"]], L.box[e]({ d: t })), M && (k = L.whiskers.strokeWidth(), p = A(L.highPlot, k), d = A(L.lowPlot, k), t = [["M", A(C - (y = "string" == typeof M && /%$/.test(M) ? m * parseFloat(M) / 100 : Number(M) / 2)), p], ["L", A(C + y), p], ["M", A(C - y), d], ["L", A(C + y), d]], L.whiskers[e]({ d: t })), t = [["M", b, c = A(L.medianPlot, L.medianShape.strokeWidth())], ["L", f, c]], L.medianShape[e]({ d: t }) } } } toYData(t) { return [t.low, t.q1, t.median, t.q3, t.high] } } return l.defaultOptions = n(e.defaultOptions, t), r(l.prototype, { pointArrayMap: ["low", "q1", "median", "q3", "high"], pointValKey: "high", drawDataLabels: o, setStackedPoints: o }), s.registerSeriesType("boxplot", l), l }), e(t, "Series/Bubble/BubbleLegendDefaults.js", [], function () { return { borderColor: void 0, borderWidth: 2, className: void 0, color: void 0, connectorClassName: void 0, connectorColor: void 0, connectorDistance: 60, connectorWidth: 1, enabled: !1, labels: { className: void 0, allowOverlap: !1, format: "", formatter: void 0, align: "right", style: { fontSize: "0.9em", color: "#000000" }, x: 0, y: 0 }, maxSize: 60, minSize: 10, legendIndex: 0, ranges: { value: void 0, borderColor: void 0, color: void 0, connectorColor: void 0 }, sizeBy: "area", sizeByAbsoluteValue: !1, zIndex: 1, zThreshold: 0 } }), e(t, "Series/Bubble/BubbleLegendItem.js", [t["Core/Color/Color.js"], t["Core/Templating.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, o, e, i) { let d = t["parse"], s = e["noop"], { arrayMax: n, arrayMin: l, isNumber: p, merge: c, pick: u, stableSort: g } = i; return class { constructor(t, e) { this.setState = s, this.init(t, e) } init(t, e) { this.options = t, this.visible = !0, this.chart = e.chart, this.legend = e } addToLegend(t) { t.splice(this.options.legendIndex, 0, this) } drawLegendSymbol(t) { let e, i = u(t.options.itemDistance, 20), s = this.legendItem || {}, a = this.options, o = a.ranges, r = a.connectorDistance; var n, l, h; o && o.length && p(o[0].value) ? (g(o, function (t, e) { return e.value - t.value }), this.ranges = o, this.setOptions(), this.render(), n = this.getMaxLabelSize(), h = 2 * (l = this.ranges[0].radius), e = 0 < (e = r - l + n.width) ? e : 0, this.maxLabel = n, this.movementX = "left" === a.labels.align ? e : 0, s.labelWidth = h + e + i, s.labelHeight = h + n.height / 2) : t.options.bubbleLegend.autoRanges = !0 } setOptions() { let i = this.ranges, s = this.options, a = this.chart.series[s.seriesIndex], o = this.legend.baseline, r = { zIndex: s.zIndex, "stroke-width": s.borderWidth }, n = { zIndex: s.zIndex, "stroke-width": s.connectorWidth }, l = { align: this.legend.options.rtl || "left" === s.labels.align ? "right" : "left", zIndex: s.zIndex }, h = a.options.marker.fillOpacity, p = this.chart.styledMode; i.forEach(function (t, e) { p || (r.stroke = u(t.borderColor, s.borderColor, a.color), r.fill = u(t.color, s.color, 1 !== h ? d(a.color).setOpacity(h).get("rgba") : a.color), n.stroke = u(t.connectorColor, s.connectorColor, a.color)), i[e].radius = this.getRangeRadius(t.value), i[e] = c(i[e], { center: i[0].radius - i[e].radius + o }), p || c(!0, i[e], { bubbleAttribs: c(r), connectorAttribs: c(n), labelAttribs: l }) }, this) } getRangeRadius(t) { let e = this.options, i = this.options.seriesIndex, s = this.chart.series[i], a = e.ranges[0].value, o = e.ranges[e.ranges.length - 1].value, r = e.minSize, n = e.maxSize; return s.getRadius.call(this, o, a, r, n, t) } render() { let t = this.legendItem || {}, e = this.chart.renderer, i = this.options.zThreshold; for (var s of (this.symbols || (this.symbols = { connectors: [], bubbleItems: [], labels: [] }), t.symbol = e.g("bubble-legend"), t.label = e.g("bubble-legend-item").css(this.legend.itemStyle || {}), t.symbol.translateX = 0, t.symbol.translateY = 0, t.symbol.add(t.label), t.label.add(t.group), this.ranges)) s.value >= i && this.renderRange(s); this.hideOverlappingLabels() } renderRange(t) { let e = this.ranges[0], i = this.legend, s = this.options, a = s.labels, o = this.chart, r = o.series[s.seriesIndex], n = o.renderer, l = this.symbols, h = l.labels, p = t.center, d = Math.abs(t.radius), c = s.connectorDistance || 0, u = a.align, g = i.options.rtl, b = s.borderWidth, f = s.connectorWidth, m = e.radius || 0, y = p - d - b / 2 + f / 2, x = (y % 1 ? 1 : .5) - (f % 2 ? 0 : .5), P = n.styledMode, v = g || "left" === u ? -c : c, S = ("center" === u && (v = 0, s.connectorDistance = 0, t.labelAttribs.align = "center"), l.bubbleItems.push(n.circle(m, p + x, d).attr(P ? {} : t.bubbleAttribs).addClass((P ? "highcharts-color-" + r.colorIndex + " " : "") + "highcharts-bubble-legend-symbol " + (s.className || "")).add(this.legendItem.symbol)), l.connectors.push(n.path(n.crispLine([["M", m, y], ["L", m + v, y]], s.connectorWidth)).attr(P ? {} : t.connectorAttribs).addClass((P ? "highcharts-color-" + this.options.seriesIndex + " " : "") + "highcharts-bubble-legend-connectors " + (s.connectorClassName || "")).add(this.legendItem.symbol)), n.text(this.formatLabel(t)).attr(P ? {} : t.labelAttribs).css(P ? {} : a.style).addClass("highcharts-bubble-legend-labels " + (s.labels.className || "")).add(this.legendItem.symbol)), M = { x: m + v + s.labels.x, y: y + s.labels.y + .4 * S.getBBox().height }; S.attr(M), h.push(S), S.placed = !0, S.alignAttr = M } getMaxLabelSize() { let e, i; return this.symbols.labels.forEach(function (t) { i = t.getBBox(!0), e = !e || i.width > e.width ? i : e }), e || {} } formatLabel(t) { let e = this.options, i = e.labels.formatter, s = e.labels.format, a = this.chart["numberFormatter"]; return s ? o.format(s, t) : i ? i.call(t) : a(t.value, 1) } hideOverlappingLabels() { let t = this.chart, e = this.options.labels.allowOverlap, i = this.symbols; !e && i && (t.hideOverlappingLabels(i.labels), i.labels.forEach(function (t, e) { t.newOpacity ? t.newOpacity !== t.oldOpacity && i.connectors[e].show() : i.connectors[e].hide() })) } getRanges() { let t = this.legend.bubbleLegend, e = t.chart.series, i = t.options.ranges, s, a, o = Number.MAX_VALUE, r = -Number.MAX_VALUE; return e.forEach(function (t) { t.isBubble && !t.ignoreSeries && (a = t.zData.filter(p)).length && (o = u(t.options.zMin, Math.min(o, Math.max(l(a), !1 === t.options.displayNegative ? t.options.zThreshold : -Number.MAX_VALUE))), r = u(t.options.zMax, Math.max(r, n(a)))) }), s = o === r ? [{ value: r }] : [{ value: o }, { value: (o + r) / 2 }, { value: r, autoRanges: !0 }], i.length && i[0].radius && s.reverse(), s.forEach(function (t, e) { i && i[e] && (s[e] = c(i[e], t)) }), s } predictBubbleSizes() { let t = this.chart, e = t.legend.options, i = e.floating, s = "horizontal" === e.layout, a = s ? t.legend.lastLineHeight : 0, o = t.plotSizeX, r = t.plotSizeY, n = t.series[this.options.seriesIndex], l = n.getPxExtremes(), h = Math.ceil(l.minPxSize), p = Math.ceil(l.maxPxSize), d = Math.min(r, o), c, u = n.options.maxSize; return i || !/%$/.test(u) ? c = p : (c = (d + a) * (u = parseFloat(u)) / 100 / (u / 100 + 1), (s && r - c >= o || !s && o - c >= r) && (c = p)), [h, Math.ceil(c)] } updateRanges(t, e) { let i = this.legend.options.bubbleLegend; i.minSize = t, i.maxSize = e, i.ranges = this.getRanges() } correctSizes() { let t = this.legend, e = this.chart.series[this.options.seriesIndex].getPxExtremes(); 1 < Math.abs(Math.ceil(e.maxPxSize) - this.options.maxSize) && (this.updateRanges(this.options.minSize, e.maxPxSize), t.render()) } } }), e(t, "Series/Bubble/BubbleLegendComposition.js", [t["Series/Bubble/BubbleLegendDefaults.js"], t["Series/Bubble/BubbleLegendItem.js"], t["Core/Defaults.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (i, o, t, e, s) { let a = t["setOptions"], r = e["composed"], { addEvent: n, objectEach: l, pushUnique: h, wrap: p } = s; function d(t, e, i) { let s, a, o, r = this.legend, n = 0 <= c(this); r && r.options.enabled && r.bubbleLegend && r.options.bubbleLegend.autoRanges && n ? (s = r.bubbleLegend.options, a = r.bubbleLegend.predictBubbleSizes(), r.bubbleLegend.updateRanges(a[0], a[1]), s.placed || (r.group.placed = !1, r.allItems.forEach(t => { (o = t.legendItem || {}).group && (o.group.translateY = void 0) })), r.render(), s.placed || (this.getMargins(), this.axes.forEach(function (t) { t.visible && t.render(), s.placed || (t.setScale(), t.updateNames(), l(t.ticks, function (t) { t.isNew = !0, t.isNewLabel = !0 })) }), this.getMargins()), s.placed = !0, t.call(this, e, i), r.bubbleLegend.correctSizes(), f(r, u(r))) : (t.call(this, e, i), r && r.options.enabled && r.bubbleLegend && (r.render(), f(r, u(r)))) } function c(t) { let e = t.series, i = 0; for (; i < e.length;) { if (e[i] && e[i].isBubble && e[i].visible && e[i].zData.length) return i; i++ } return -1 } function u(t) { let e = t.allItems, i = [], s = e.length, a, o, r, n = 0, l = 0; for (n = 0; n < s; n++)if (o = e[n].legendItem || {}, r = (e[n + 1] || {}).legendItem || {}, o.labelHeight && (e[n].itemHeight = o.labelHeight), e[n] === e[s - 1] || o.y !== r.y) { for (i.push({ height: 0 }), a = i[i.length - 1]; l <= n; l++)e[l].itemHeight > a.height && (a.height = e[l].itemHeight); a.step = n } return i } function g(t) { let e = this.bubbleLegend, i = this.options, s = i.bubbleLegend, a = c(this.chart); e && e.ranges && e.ranges.length && (s.ranges.length && (s.autoRanges = !!s.ranges[0].autoRanges), this.destroyItem(e)), 0 <= a && i.enabled && s.enabled && (s.seriesIndex = a, this.bubbleLegend = new o(s, this), this.bubbleLegend.addToLegend(t.allItems)) } function b(t) { if (t.defaultPrevented) return !1; let e = t.legendItem, i = this.chart, s = e.visible; this && this.bubbleLegend && (e.visible = !s, e.ignoreSeries = s, t = 0 <= c(i), this.bubbleLegend.visible !== t && (this.update({ bubbleLegend: { enabled: t } }), this.bubbleLegend.visible = t), e.visible = s) } function f(t, i) { let e = t.allItems, s = t.options.rtl, a, o, r, n, l = 0; e.forEach((t, e) => { (n = t.legendItem || {}).group && (a = n.group.translateX || 0, o = n.y || 0, ((r = t.movementX) || s && t.ranges) && (r = s ? a - t.options.maxSize / 2 : a + r, n.group.attr({ translateX: r })), e > i[l].step && l++, n.group.attr({ translateY: Math.round(o + i[l].height / 2) }), n.y = o + i[l].height / 2) }) } return { compose: function (t, e) { h(r, "Series.BubbleLegend") && (a({ legend: { bubbleLegend: i } }), p(t.prototype, "drawChartBox", d), n(e, "afterGetAllItems", g), n(e, "itemClick", b)) } } }), e(t, "Series/Bubble/BubblePoint.js", [t["Core/Series/Point.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let { scatter: { prototype: { pointClass: s } } } = e["seriesTypes"], a = i["extend"]; class o extends s { haloPath(a) { a = (a && this.marker && this.marker.radius || 0) + a; if (this.series.chart.inverted) { let t = this.pos() || [0, 0], { xAxis: e, yAxis: i, chart: s } = this.series; return s.renderer.symbols.circle(e.len - t[1] - a, i.len - t[0] - a, 2 * a, 2 * a) } return t.prototype.haloPath.call(this, a) } } return a(o.prototype, { ttBelow: !1 }), o }), e(t, "Series/Bubble/BubbleSeries.js", [t["Series/Bubble/BubbleLegendComposition.js"], t["Series/Bubble/BubblePoint.js"], t["Core/Color/Color.js"], t["Core/Globals.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (s, t, e, i, a, o) { let r = e["parse"], { composed: n, noop: l } = i, { series: h, seriesTypes: { column: { prototype: p }, scatter: d } } = a, { addEvent: c, arrayMax: u, arrayMin: g, clamp: b, extend: f, isNumber: m, merge: y, pick: x, pushUnique: P } = o; function v() { let t = this.len, { coll: e, isXAxis: a, min: o } = this, r = a ? "xData" : "yData", n = (this.max || 0) - (o || 0), l = 0, h = t, p = t / n, d; "xAxis" !== e && "yAxis" !== e || (this.series.forEach(e => { if (e.bubblePadding && e.reserveSpace()) { this.allowZoomOutside = !0, d = !0; var i, s = e[r]; if (a && ((e.onPoint || e).getRadii(0, 0, e), e.onPoint && (e.radii = e.onPoint.radii)), 0 < n) { let t = s.length; for (; t--;)m(s[t]) && this.dataMin <= s[t] && s[t] <= this.max && (i = e.radii && e.radii[t] || 0, l = Math.min((s[t] - o) * p - i, l), h = Math.max((s[t] - o) * p + i, h)) } } }), d && 0 < n && !this.logarithmic && (h -= t, p *= (t + Math.max(0, l) - Math.min(h, t)) / t, [["min", "userMin", l], ["max", "userMax", h]].forEach(t => { void 0 === x(this.options[t[0]], this[t[1]]) && (this[t[0]] += t[2] / p) }))) } class S extends d { static compose(t, e, i) { s.compose(e, i), P(n, "Series.Bubble") && c(t, "foundExtremes", v) } animate(t) { !t && this.points.length < this.options.animationLimit && this.points.forEach(function (t) { let { graphic: e, plotX: i = 0, plotY: s = 0 } = t; e && e.width && (this.hasRendered || e.attr({ x: i, y: s, width: 1, height: 1 }), e.animate(this.markerAttribs(t), this.options.animation)) }, this) } getRadii() { let t = this.zData, e = this.yData, i = [], s, a, o, r = this.chart.bubbleZExtremes, { minPxSize: n, maxPxSize: l } = this.getPxExtremes(); if (!r) { let e, i = Number.MAX_VALUE, s = -Number.MAX_VALUE; this.chart.series.forEach(t => { t.bubblePadding && t.reserveSpace() && ((t = (t.onPoint || t).getZExtremes()) && (i = Math.min(x(i, t.zMin), t.zMin), s = Math.max(x(s, t.zMax), t.zMax), e = !0)) }), e ? (r = { zMin: i, zMax: s }, this.chart.bubbleZExtremes = r) : r = { zMin: 0, zMax: 0 } } for (a = 0, s = t.length; a < s; a++)o = t[a], i.push(this.getRadius(r.zMin, r.zMax, n, l, o, e && e[a])); this.radii = i } getRadius(t, e, i, s, a, o) { let r = this.options, n = "width" !== r.sizeBy, l = r.zThreshold, h = e - t, p = .5; if (null === o || null === a) return null; if (m(a)) { if (r.sizeByAbsoluteValue && (a = Math.abs(a - l), e = h = Math.max(e - l, Math.abs(t - l)), t = 0), a < t) return i / 2 - 1; 0 < h && (p = (a - t) / h) } return n && 0 <= p && (p = Math.sqrt(p)), Math.ceil(i + p * (s - i)) / 2 } hasData() { return !!this.processedXData.length } markerAttribs(t, e) { var e = super.markerAttribs(t, e), { height: i = 0, width: s = 0 } = e; return this.chart.inverted ? f(e, { x: (t.plotX || 0) - s / 2, y: (t.plotY || 0) - i / 2 }) : e } pointAttribs(t, e) { let i = this.options.marker.fillOpacity, s = h.prototype.pointAttribs.call(this, t, e); return 1 !== i && (s.fill = r(s.fill).setOpacity(i).get("rgba")), s } translate() { super.translate.call(this), this.getRadii(), this.translateBubble() } translateBubble() { let { data: i, options: s, radii: a } = this, o = this.getPxExtremes()["minPxSize"], r = i.length; for (; r--;) { let t = i[r], e = a ? a[r] : 0; "z" === this.zoneAxis && (t.negative = (t.z || 0) < (s.zThreshold || 0)), m(e) && o / 2 <= e ? (t.marker = f(t.marker, { radius: e, width: 2 * e, height: 2 * e }), t.dlBox = { x: t.plotX - e, y: t.plotY - e, width: 2 * e, height: 2 * e }) : (t.shapeArgs = t.plotY = t.dlBox = void 0, t.isInside = !1) } } getPxExtremes() { let i = Math.min(this.chart.plotWidth, this.chart.plotHeight), t = t => { let e; return "string" == typeof t && (e = /%$/.test(t), t = parseInt(t, 10)), e ? i * t / 100 : t }, e = t(x(this.options.minSize, 8)), s = Math.max(t(x(this.options.maxSize, "20%")), e); return { minPxSize: e, maxPxSize: s } } getZExtremes() { var t = this.options, e = (this.zData || []).filter(m); if (e.length) { var i = x(t.zMin, b(g(e), !1 === t.displayNegative ? t.zThreshold || 0 : -Number.MAX_VALUE, Number.MAX_VALUE)), t = x(t.zMax, u(e)); if (m(i) && m(t)) return { zMin: i, zMax: t } } } } return S.defaultOptions = y(d.defaultOptions, { dataLabels: { formatter: function () { let t = this.series.chart["numberFormatter"], e = this.point["z"]; return m(e) ? t(e, -1) : "" }, inside: !0, verticalAlign: "middle" }, animationLimit: 250, marker: { lineColor: null, lineWidth: 1, fillOpacity: .5, radius: null, states: { hover: { radiusPlus: 0 } }, symbol: "circle" }, minSize: 8, maxSize: "20%", softThreshold: !1, states: { hover: { halo: { size: 5 } } }, tooltip: { pointFormat: "({point.x}, {point.y}), Size: {point.z}" }, turboThreshold: 0, zThreshold: 0, zoneAxis: "z" }), f(S.prototype, { alignDataLabel: p.alignDataLabel, applyZones: l, bubblePadding: !0, isBubble: !0, pointArrayMap: ["y", "z"], pointClass: t, parallelArrays: ["x", "y", "z"], trackerGroups: ["group", "dataLabelsGroup"], specialGroup: "group", zoneAxis: "z" }), c(S, "updatedData", t => { delete t.target.chart.bubbleZExtremes }), c(S, "remove", t => { delete t.target.chart.bubbleZExtremes }), a.registerSeriesType("bubble", S), S }), e(t, "Series/ColumnRange/ColumnRangePoint.js", [t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e) { let { column: { prototype: { pointClass: { prototype: i } } }, arearange: { prototype: { pointClass: s } } } = t["seriesTypes"], { extend: a, isNumber: o } = e; class r extends s { isValid() { return o(this.low) } } return a(r.prototype, { setState: i.setState }), r }), e(t, "Series/ColumnRange/ColumnRangeSeries.js", [t["Series/ColumnRange/ColumnRangePoint.js"], t["Core/Globals.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s) { let a = e["noop"], { arearange: o, column: r, column: { prototype: n } } = i["seriesTypes"], { addEvent: l, clamp: m, extend: h, isNumber: y, merge: x, pick: P } = s; class p extends o { setOptions() { return x(!0, arguments[0], { stacking: void 0 }), o.prototype.setOptions.apply(this, arguments) } translate() { return n.translate.apply(this) } pointAttribs() { return n.pointAttribs.apply(this, arguments) } translate3dPoints() { return n.translate3dPoints.apply(this, arguments) } translate3dShapes() { return n.translate3dShapes.apply(this, arguments) } afterColumnTranslate() { let n, l, h, p, d = this.yAxis, c = this.xAxis, u = c.startAngleRad, g = this.chart, b = this.xAxis.isRadial, f = Math.max(g.chartWidth, g.chartHeight) + 999; this.points.forEach(t => { let e = t.shapeArgs || {}, i = this.options.minPointLength, s = t.plotY, a = d.translate(t.high, 0, 1, 0, 1); var o, r; y(a) && y(s) && (t.plotHigh = m(a, -f, f), t.plotLow = m(s, -f, f), p = t.plotHigh, Math.abs(n = P(t.rectPlotY, t.plotY) - t.plotHigh) < i ? (l = i - n, n += l, p -= l / 2) : n < 0 && (n *= -1, p -= n), b && this.polar ? (h = t.barX + u, t.shapeType = "arc", t.shapeArgs = this.polar.arc(p + n, p, h, h + t.pointWidth)) : (e.height = n, e.y = p, { x: o = 0, width: r = 0 } = e, t.shapeArgs = x(t.shapeArgs, this.crispCol(o, p, r, n)), t.tooltipPos = g.inverted ? [d.len + d.pos - g.plotLeft - p - n / 2, c.len + c.pos - g.plotTop - o - r / 2, n] : [c.left - g.plotLeft + o + r / 2, d.pos - g.plotTop + p + n / 2, n])) }) } } return p.defaultOptions = x(r.defaultOptions, o.defaultOptions, { borderRadius: { where: "all" }, pointRange: null, legendSymbol: "rectangle", marker: null, states: { hover: { halo: !1 } } }), l(p, "afterColumnTranslate", function () { p.prototype.afterColumnTranslate.apply(this) }, { order: 5 }), h(p.prototype, { directTouch: !0, pointClass: t, trackerGroups: ["group", "dataLabelsGroup"], adjustForMissingColumns: n.adjustForMissingColumns, animate: n.animate, crispCol: n.crispCol, drawGraph: a, drawPoints: n.drawPoints, getSymbol: a, drawTracker: n.drawTracker, getColumnMetrics: n.getColumnMetrics }), i.registerSeriesType("columnrange", p), p }), e(t, "Series/ColumnPyramid/ColumnPyramidSeriesDefaults.js", [], function () { return {} }), e(t, "Series/ColumnPyramid/ColumnPyramidSeries.js", [t["Series/ColumnPyramid/ColumnPyramidSeriesDefaults.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i) { let s = e.seriesTypes["column"], { clamp: T, merge: a, pick: X } = i; class o extends s { translate() { let y = this.chart, x = this.options, t = this.dense = this.closestPointRange * this.xAxis.transA < 2, e = this.borderWidth = X(x.borderWidth, t ? 0 : 1), P = this.yAxis, v = x.threshold, S = X(x.minPointLength, 5), M = this.getColumnMetrics(), L = M.width, k = this.pointXOffset = M.offset, C = this.translatedThreshold = P.getThreshold(v), A = this.barW = Math.max(L, 1 + 2 * e); for (var w of (y.inverted && (C -= .5), x.pointPadding && (A = Math.ceil(A)), super.translate(), this.points)) { let t = X(w.yBottom, C), e = 999 + Math.abs(t), i = T(w.plotY, -e, P.len + e), s = A / 2, a = Math.min(i, t), o = Math.max(i, t) - a, r = w.plotX + k, n, l, h, p, d, c, u, g, b, f, m; x.centerInCategory && (r = this.adjustForMissingColumns(r, L, w, M)), w.barX = r, w.pointWidth = L, w.tooltipPos = y.inverted ? [P.len + P.pos - y.plotLeft - i, this.xAxis.len - r - s, o] : [r + s, i + P.pos - y.plotTop, o], n = v + (w.total || w.y), "percent" === x.stacking && (n = v + (w.y < 0) ? -100 : 100); var N = P.toPixels(n, !0); h = (l = y.plotHeight - N - (y.plotHeight - C)) ? s * (a - N) / l : 0, p = l ? s * (a + o - N) / l : 0, c = r - h + s, u = r + h + s, g = r + p + s, b = r - p + s, f = a - S, m = a + o, w.y < 0 && (f = a, m = a + o + S), y.inverted && (h = s * (N - (d = P.width - a)) / (l = N - (P.width - C)), p = s * (N - (d - o)) / l, u = (c = r + s + h) - 2 * h, g = r - p + s, b = r + p + s, f = a, m = a + o - S, w.y < 0 && (m = a + o + S)), w.shapeType = "path", w.shapeArgs = { x: c, y: f, width: u - c, height: o, d: [["M", c, f], ["L", u, f], ["L", g, m], ["L", b, m], ["Z"]] } } } } return o.defaultOptions = a(s.defaultOptions, t), e.registerSeriesType("columnpyramid", o), o }), e(t, "Series/ErrorBar/ErrorBarSeriesDefaults.js", [], function () { return { color: "#000000", grouping: !1, linkedTo: ":previous", tooltip: { pointFormat: '<span style="color:{point.color}">●</span> {series.name}: <b>{point.low}</b> - <b>{point.high}</b><br/>' }, whiskerWidth: null } }), e(t, "Series/ErrorBar/ErrorBarSeries.js", [t["Series/BoxPlot/BoxPlotSeries.js"], t["Series/Column/ColumnSeries.js"], t["Series/ErrorBar/ErrorBarSeriesDefaults.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s, a) { let o = s.seriesTypes["arearange"], { addEvent: r, merge: n, extend: l } = a; class h extends t { getColumnMetrics() { return this.linkedParent && this.linkedParent.columnMetrics || e.prototype.getColumnMetrics.call(this) } drawDataLabels() { var t = this.pointValKey; if (o) for (var e of (o.prototype.drawDataLabels.call(this), this.points)) e.y = e[t] } toYData(t) { return [t.low, t.high] } } return h.defaultOptions = n(t.defaultOptions, i), r(h, "afterTranslate", function () { for (var t of this.points) t.plotLow = t.plotY }, { order: 0 }), l(h.prototype, { pointArrayMap: ["low", "high"], pointValKey: "high", doQuartiles: !1 }), s.registerSeriesType("errorbar", h), h }), e(t, "Series/Gauge/GaugePoint.js", [t["Core/Series/SeriesRegistry.js"]], function (t) { var { pointClass: t } = t["series"]["prototype"]; return class extends t { setState(t) { this.state = t } } }), e(t, "Series/Gauge/GaugeSeries.js", [t["Series/Gauge/GaugePoint.js"], t["Core/Globals.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s) { let a = e["noop"], { series: o, seriesTypes: { column: r } } = i, { clamp: u, isNumber: g, extend: n, merge: b, pick: l, pInt: f, defined: m } = s; class h extends o { translate() { let p = this.yAxis, d = this.options, c = p.center; this.generatePoints(), this.points.forEach(t => { let e = b(d.dial, t.dial), i = f(e.radius) * c[2] / 200, s = f(e.baseLength) * i / 100, a = f(e.rearLength) * i / 100, o = e.baseWidth, r = e.topWidth, n = d.overshoot, l = p.startAngleRad + p.translate(t.y, void 0, void 0, void 0, !0); !g(n) && !1 !== d.wrap || (n = g(n) ? n / 180 * Math.PI : 0, l = u(l, p.startAngleRad - n, p.endAngleRad + n)), l = 180 * l / Math.PI, t.shapeType = "path"; var h = e.path || [["M", -a, -o / 2], ["L", s, -o / 2], ["L", i, -r / 2], ["L", i, r / 2], ["L", s, o / 2], ["L", -a, o / 2], ["Z"]]; t.shapeArgs = { d: h, translateX: c[0], translateY: c[1], rotation: l }, t.plotX = c[0], t.plotY = c[1], m(t.y) && p.max - p.min && (t.percentage = (t.y - p.min) / (p.max - p.min) * 100) }) } drawPoints() { let o = this, r = o.chart, t = o.yAxis.center, e = o.pivot, n = o.options, i = n.pivot, l = r.renderer; o.points.forEach(t => { let e = t.graphic, i = t.shapeArgs, s = i.d, a = b(n.dial, t.dial); e ? (e.animate(i), i.d = s) : t.graphic = l[t.shapeType](i).addClass("highcharts-dial").add(o.group), r.styledMode || t.graphic[e ? "animate" : "attr"]({ stroke: a.borderColor, "stroke-width": a.borderWidth, fill: a.backgroundColor }) }), e ? e.animate({ translateX: t[0], translateY: t[1] }) : i && (o.pivot = l.circle(0, 0, i.radius).attr({ zIndex: 2 }).addClass("highcharts-pivot").translate(t[0], t[1]).add(o.group), r.styledMode || o.pivot.attr({ fill: i.backgroundColor, stroke: i.borderColor, "stroke-width": i.borderWidth })) } animate(t) { let i = this; t || i.points.forEach(t => { let e = t.graphic; e && (e.attr({ rotation: 180 * i.yAxis.startAngleRad / Math.PI }), e.animate({ rotation: t.shapeArgs.rotation }, i.options.animation)) }) } render() { this.group = this.plotGroup("group", "series", this.visible ? "inherit" : "hidden", this.options.zIndex, this.chart.seriesGroup), o.prototype.render.call(this), this.group.clip(this.chart.clipRect) } setData(t, e) { o.prototype.setData.call(this, t, !1), this.processData(), this.generatePoints(), l(e, !0) && this.chart.redraw() } hasData() { return !!this.points.length } } return h.defaultOptions = b(o.defaultOptions, { dataLabels: { borderColor: "#cccccc", borderRadius: 3, borderWidth: 1, crop: !1, defer: !1, enabled: !0, verticalAlign: "top", y: 15, zIndex: 2 }, dial: { backgroundColor: "#000000", baseLength: "70%", baseWidth: 3, borderColor: "#cccccc", borderWidth: 0, radius: "80%", rearLength: "10%", topWidth: 1 }, pivot: { radius: 5, borderWidth: 0, borderColor: "#cccccc", backgroundColor: "#000000" }, tooltip: { headerFormat: "" }, showInLegend: !1 }), n(h.prototype, { angular: !0, directTouch: !0, drawGraph: a, drawTracker: r.prototype.drawTracker, fixedBox: !0, forceDL: !0, noSharedTooltip: !0, pointClass: t, trackerGroups: ["group", "dataLabelsGroup"] }), i.registerSeriesType("gauge", h), h }), e(t, "Series/DragNodesComposition.js", [t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, e) { let i = t["composed"], { addEvent: o, pushUnique: s } = e; function a() { let t, i, s, a = this; a.container && (t = o(a.container, "mousedown", t => { let e = a.hoverPoint; e && e.series && e.series.hasDraggableNodes && e.series.options.draggable && (e.series.onMouseDown(e, t), i = o(a.container, "mousemove", t => e && e.series && e.series.onMouseMove(e, t)), s = o(a.container.ownerDocument, "mouseup", t => (i(), s(), e && e.series && e.series.onMouseUp(e, t)))) })), o(a, "destroy", function () { t() }) } return { compose: function (t) { s(i, "DragNodes") && o(t, "load", a) }, onMouseDown: function (t, e) { e = this.chart.pointer?.normalize(e) || e; t.fixedPosition = { chartX: e.chartX, chartY: e.chartY, plotX: t.plotX, plotY: t.plotY }, t.inDragMode = !0 }, onMouseMove: function (o, r) { if (o.fixedPosition && o.inDragMode) { var n; let t = this.chart, e = t.pointer?.normalize(r) || r, i = o.fixedPosition.chartX - e.chartX, s = o.fixedPosition.chartY - e.chartY, a = t.graphLayoutsLookup; (5 < Math.abs(i) || 5 < Math.abs(s)) && (r = o.fixedPosition.plotX - i, n = o.fixedPosition.plotY - s, t.isInsidePlot(r, n) && (o.plotX = r, o.plotY = n, o.hasDragged = !0, this.redrawHalo(o), a.forEach(t => { t.restartSimulation() }))) } }, onMouseUp: function (t) { t.fixedPosition && (t.hasDragged && (this.layout.enableSimulation ? this.layout.start() : this.chart.redraw()), t.inDragMode = t.hasDragged = !1, this.options.fixedDraggable || delete t.fixedPosition) }, redrawHalo: function (t) { t && this.halo && this.halo.attr({ d: t.haloPath(this.options.states.hover.halo.size) }) } } }), e(t, "Series/GraphLayoutComposition.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, e, i) { let s = t["setAnimation"], a = e["composed"], { addEvent: o, pushUnique: r } = i; function n() { this.graphLayoutsLookup && (this.graphLayoutsLookup.forEach(t => { t.updateSimulation() }), this.redraw()) } function l() { this.graphLayoutsLookup && (this.graphLayoutsLookup.forEach(t => { t.updateSimulation(!1) }), this.redraw()) } function h() { this.graphLayoutsLookup && this.graphLayoutsLookup.forEach(t => { t.stop() }) } function p() { let e, i = !1, t = t => { t.maxIterations-- && isFinite(t.temperature) && !t.isStable() && !t.enableSimulation && (t.beforeStep && t.beforeStep(), t.step(), e = !1, i = !0) }; if (this.graphLayoutsLookup) { for (s(!1, this), this.graphLayoutsLookup.forEach(t => t.start()); !e;)e = !0, this.graphLayoutsLookup.forEach(t); i && this.series.forEach(t => { t && t.layout && t.render() }) } } return { compose: function (t) { r(a, "GraphLayout") && (o(t, "afterPrint", n), o(t, "beforePrint", l), o(t, "predraw", h), o(t, "render", p)) }, integrations: {}, layouts: {} } }), e(t, "Series/PackedBubble/PackedBubblePoint.js", [t["Core/Chart/Chart.js"], t["Core/Series/Point.js"], t["Core/Series/SeriesRegistry.js"]], function (e, i, t) { var { prototype: { pointClass: t } } = t["seriesTypes"]["bubble"]; return class extends t { destroy() { return this.series?.layout && this.series.layout.removeElementFromCollection(this, this.series.layout.nodes), i.prototype.destroy.apply(this, arguments) } firePointEvent() { let t = this.series.options; var e; this.isParentNode && t.parentNode ? (e = t.allowPointSelect, t.allowPointSelect = t.parentNode.allowPointSelect, i.prototype.firePointEvent.apply(this, arguments), t.allowPointSelect = e) : i.prototype.firePointEvent.apply(this, arguments) } select() { let t = this.series.chart; this.isParentNode ? (t.getSelectedPoints = t.getSelectedParentNodes, i.prototype.select.apply(this, arguments), t.getSelectedPoints = e.prototype.getSelectedPoints) : i.prototype.select.apply(this, arguments) } } }), e(t, "Series/PackedBubble/PackedBubbleSeriesDefaults.js", [t["Core/Utilities.js"]], function (t) { let i = t["isNumber"]; return { minSize: "10%", maxSize: "50%", sizeBy: "area", zoneAxis: "y", crisp: !1, tooltip: { pointFormat: "Value: {point.value}" }, draggable: !0, useSimulation: !0, parentNode: { allowPointSelect: !1 }, dataLabels: { formatter: function () { let t = this.series.chart["numberFormatter"], e = this.point["value"]; return i(e) ? t(e, -1) : "" }, parentNodeFormatter: function () { return this.name }, parentNodeTextPath: { enabled: !0 }, padding: 0, style: { transition: "opacity 2000ms" } }, layoutAlgorithm: { initialPositions: "circle", initialPositionRadius: 20, bubblePadding: 5, parentNodeLimit: !1, seriesInteraction: !0, dragBetweenSeries: !1, parentNodeOptions: { maxIterations: 400, gravitationalConstant: .03, maxSpeed: 50, initialPositionRadius: 100, seriesInteraction: !0, marker: { fillColor: null, fillOpacity: 1, lineWidth: null, lineColor: null, symbol: "circle" } }, enableSimulation: !0, type: "packedbubble", integration: "packedbubble", maxIterations: 1e3, splitSeries: !1, maxSpeed: 5, gravitationalConstant: .01, friction: -.981 } } }), e(t, "Series/Networkgraph/VerletIntegration.js", [], function () { return { attractive: function (t, e, i) { var s = t.getMass(), a = -i.x * e * this.diffTemperature, i = -i.y * e * this.diffTemperature; t.fromNode.fixedPosition || (t.fromNode.plotX -= a * s.fromNode / t.fromNode.degree, t.fromNode.plotY -= i * s.fromNode / t.fromNode.degree), t.toNode.fixedPosition || (t.toNode.plotX += a * s.toNode / t.toNode.degree, t.toNode.plotY += i * s.toNode / t.toNode.degree) }, attractiveForceFunction: function (t, e) { return (e - t) / t }, barycenter: function () { let t = this.options.gravitationalConstant || 0, e = (this.barycenter.xFactor - (this.box.left + this.box.width) / 2) * t, i = (this.barycenter.yFactor - (this.box.top + this.box.height) / 2) * t; this.nodes.forEach(function (t) { t.fixedPosition || (t.plotX -= e / t.mass / t.degree, t.plotY -= i / t.mass / t.degree) }) }, getK: function (t) { return Math.pow(t.box.width * t.box.height / t.nodes.length, .5) }, integrate: function (t, e) { let i = -t.options.friction, s = t.options.maxSpeed, a = e.prevX, o = e.prevY, r = (e.plotX + e.dispX - a) * i, n = (e.plotY + e.dispY - o) * i, l = Math.abs, h = l(r) / (r || 1), p = l(n) / (n || 1), d = h * Math.min(s, Math.abs(r)), c = p * Math.min(s, Math.abs(n)); e.prevX = e.plotX + e.dispX, e.prevY = e.plotY + e.dispY, e.plotX += d, e.plotY += c, e.temperature = t.vectorLength({ x: d, y: c }) }, repulsive: function (t, e, i) { e = e * this.diffTemperature / t.mass / t.degree; t.fixedPosition || (t.plotX += i.x * e, t.plotY += i.y * e) }, repulsiveForceFunction: function (t, e) { return (e - t) / t * (t < e ? 1 : 0) } } }), e(t, "Series/PackedBubble/PackedBubbleIntegration.js", [t["Core/Globals.js"], t["Series/Networkgraph/VerletIntegration.js"]], function (t, e) { t = t.noop; return { barycenter: function () { let t, e; var i, s = this.options.gravitationalConstant, a = this.box, o = this.nodes; for (i of o) e = this.options.splitSeries && !i.isParentNode ? (t = i.series.parentNode.plotX, i.series.parentNode.plotY) : (t = a.width / 2, a.height / 2), i.fixedPosition || (i.plotX -= (i.plotX - t) * s / (i.mass * Math.sqrt(o.length)), i.plotY -= (i.plotY - e) * s / (i.mass * Math.sqrt(o.length))) }, getK: t, integrate: e.integrate, repulsive: function (t, e, i, s) { var e = e * this.diffTemperature / t.mass / t.degree, a = i.x * e, i = i.y * e; t.fixedPosition || (t.plotX += a, t.plotY += i), s.fixedPosition || (s.plotX -= a, s.plotY -= i) }, repulsiveForceFunction: function (t, e, i, s) { return Math.min(t, (i.marker.radius + s.marker.radius) / 2) } } }), e(t, "Series/Networkgraph/EulerIntegration.js", [], function () { return { attractive: function (t, e, i, s) { var a = t.getMass(), o = i.x / s * e, i = i.y / s * e; t.fromNode.fixedPosition || (t.fromNode.dispX -= o * a.fromNode / t.fromNode.degree, t.fromNode.dispY -= i * a.fromNode / t.fromNode.degree), t.toNode.fixedPosition || (t.toNode.dispX += o * a.toNode / t.toNode.degree, t.toNode.dispY += i * a.toNode / t.toNode.degree) }, attractiveForceFunction: function (t, e) { return t * t / e }, barycenter: function () { let i = this.options.gravitationalConstant, s = this.barycenter.xFactor, a = this.barycenter.yFactor; this.nodes.forEach(function (t) { var e; t.fixedPosition || (e = (e = t.getDegree()) * (1 + e / 2), t.dispX += (s - t.plotX) * i * e / t.degree, t.dispY += (a - t.plotY) * i * e / t.degree) }) }, getK: function (t) { return Math.pow(t.box.width * t.box.height / t.nodes.length, .3) }, integrate: function (t, e) { e.dispX += e.dispX * t.options.friction, e.dispY += e.dispY * t.options.friction; var i = e.temperature = t.vectorLength({ x: e.dispX, y: e.dispY }); 0 !== i && (e.plotX += e.dispX / i * Math.min(Math.abs(e.dispX), t.temperature), e.plotY += e.dispY / i * Math.min(Math.abs(e.dispY), t.temperature)) }, repulsive: function (t, e, i, s) { t.dispX += i.x / s * e / t.degree, t.dispY += i.y / s * e / t.degree }, repulsiveForceFunction: function (t, e) { return e * e / t } } }), e(t, "Series/Networkgraph/QuadTreeNode.js", [], function () { return class s { constructor(t) { this.body = !1, this.isEmpty = !1, this.isInternal = !1, this.nodes = [], this.box = t, this.boxSize = Math.min(t.width, t.height) } divideBox() { var t = this.box.width / 2, e = this.box.height / 2; this.nodes[0] = new s({ left: this.box.left, top: this.box.top, width: t, height: e }), this.nodes[1] = new s({ left: this.box.left + t, top: this.box.top, width: t, height: e }), this.nodes[2] = new s({ left: this.box.left + t, top: this.box.top + e, width: t, height: e }), this.nodes[3] = new s({ left: this.box.left, top: this.box.top + e, width: t, height: e }) } getBoxPosition(t) { var e = t.plotX < this.box.left + this.box.width / 2, t = t.plotY < this.box.top + this.box.height / 2; return e ? t ? 0 : 3 : t ? 1 : 2 } insert(t, e) { let i; this.isInternal ? this.nodes[this.getBoxPosition(t)].insert(t, e - 1) : (this.isEmpty = !1, this.body ? e ? (this.isInternal = !0, this.divideBox(), !0 !== this.body && (this.nodes[this.getBoxPosition(this.body)].insert(this.body, e - 1), this.body = !0), this.nodes[this.getBoxPosition(t)].insert(t, e - 1)) : ((i = new s({ top: t.plotX || NaN, left: t.plotY || NaN, width: .1, height: .1 })).body = t, i.isInternal = !1, this.nodes.push(i)) : (this.isInternal = !1, this.body = t)) } updateMassAndCenter() { let t = 0, e = 0, i = 0; if (this.isInternal) { for (var s of this.nodes) s.isEmpty || (t += s.mass, e += s.plotX * s.mass, i += s.plotY * s.mass); e /= t, i /= t } else this.body && (t = this.body.mass, e = this.body.plotX, i = this.body.plotY); this.mass = t, this.plotX = e, this.plotY = i } } }), e(t, "Series/Networkgraph/QuadTree.js", [t["Series/Networkgraph/QuadTreeNode.js"]], function (a) { return class { constructor(t, e, i, s) { this.box = { left: t, top: e, width: i, height: s }, this.maxDepth = 25, this.root = new a(this.box), this.root.isInternal = !0, this.root.isRoot = !0, this.root.divideBox() } calculateMassAndCenter() { this.visitNodeRecursive(null, null, function (t) { t.updateMassAndCenter() }) } insertNodes(t) { for (var e of t) this.root.insert(e, this.maxDepth) } visitNodeRecursive(t, e, i) { let s; if (t = t || this.root, !1 !== (s = t === this.root && e ? e(t) : s)) { for (var a of t.nodes) { if (a.isInternal) { if (!1 === (s = e ? e(a) : s)) continue; this.visitNodeRecursive(a, e, i) } else a.body && e && e(a.body); i && i(a) } t === this.root && i && i(t) } } } }), e(t, "Series/Networkgraph/ReingoldFruchtermanLayout.js", [t["Series/Networkgraph/EulerIntegration.js"], t["Core/Globals.js"], t["Series/GraphLayoutComposition.js"], t["Series/Networkgraph/QuadTree.js"], t["Core/Utilities.js"], t["Series/Networkgraph/VerletIntegration.js"]], function (e, t, i, s, a, o) { let r = t["win"], { clamp: n, defined: l, isFunction: h, fireEvent: p, pick: d } = a; return class c { constructor() { this.box = {}, this.currentStep = 0, this.initialRendering = !0, this.links = [], this.nodes = [], this.series = [], this.simulation = !1 } static compose(t) { i.compose(t), i.integrations.euler = e, i.integrations.verlet = o, i.layouts["reingold-fruchterman"] = c } init(t) { this.options = t, this.nodes = [], this.links = [], this.series = [], this.box = { x: 0, y: 0, width: 0, height: 0 }, this.setInitialRendering(!0), this.integration = i.integrations[t.integration], this.enableSimulation = t.enableSimulation, this.attractiveForce = d(t.attractiveForce, this.integration.attractiveForceFunction), this.repulsiveForce = d(t.repulsiveForce, this.integration.repulsiveForceFunction), this.approximation = t.approximation } updateSimulation(t) { this.enableSimulation = d(t, this.options.enableSimulation) } start() { let t = this.series, e = this.options; this.currentStep = 0, this.forces = t[0] && t[0].forces || [], this.chart = t[0] && t[0].chart, this.initialRendering && (this.initPositions(), t.forEach(function (t) { t.finishedAnimating = !0, t.render() })), this.setK(), this.resetSimulation(e), this.enableSimulation && this.step() } step() { var t, e = this.series; for (t of (this.currentStep++, "barnes-hut" === this.approximation && (this.createQuadTree(), this.quadTree.calculateMassAndCenter()), this.forces || [])) this[t + "Forces"](this.temperature); if (this.applyLimits(), this.temperature = this.coolDown(this.startTemperature, this.diffTemperature, this.currentStep), this.prevSystemTemperature = this.systemTemperature, this.systemTemperature = this.getSystemTemperature(), this.enableSimulation) { for (var i of e) i.chart && i.render(); this.maxIterations-- && isFinite(this.temperature) && !this.isStable() ? (this.simulation && r.cancelAnimationFrame(this.simulation), this.simulation = r.requestAnimationFrame(() => this.step())) : (this.simulation = !1, this.series.forEach(t => { p(t, "afterSimulation") })) } } stop() { this.simulation && r.cancelAnimationFrame(this.simulation) } setArea(t, e, i, s) { this.box = { left: t, top: e, width: i, height: s } } setK() { this.k = this.options.linkLength || this.integration.getK(this) } addElementsToCollection(t, e) { for (var i of t) -1 === e.indexOf(i) && e.push(i) } removeElementFromCollection(t, e) { -1 !== (t = e.indexOf(t)) && e.splice(t, 1) } clear() { this.nodes.length = 0, this.links.length = 0, this.series.length = 0, this.resetSimulation() } resetSimulation() { this.forcedStop = !1, this.systemTemperature = 0, this.setMaxIterations(), this.setTemperature(), this.setDiffTemperature() } restartSimulation() { this.simulation ? this.resetSimulation() : (this.setInitialRendering(!1), this.enableSimulation ? this.start() : this.setMaxIterations(1), this.chart && this.chart.redraw(), this.setInitialRendering(!0)) } setMaxIterations(t) { this.maxIterations = d(t, this.options.maxIterations) } setTemperature() { this.temperature = this.startTemperature = Math.sqrt(this.nodes.length) } setDiffTemperature() { this.diffTemperature = this.startTemperature / (this.options.maxIterations + 1) } setInitialRendering(t) { this.initialRendering = t } createQuadTree() { this.quadTree = new s(this.box.left, this.box.top, this.box.width, this.box.height), this.quadTree.insertNodes(this.nodes) } initPositions() { let t = this.options.initialPositions; if (h(t)) for (var e of (t.call(this), this.nodes)) l(e.prevX) || (e.prevX = e.plotX), l(e.prevY) || (e.prevY = e.plotY), e.dispX = 0, e.dispY = 0; else "circle" === t ? this.setCircularPositions() : this.setRandomPositions() } setCircularPositions() { let i, s = this.box, t = this.nodes, a = 2 * Math.PI / (t.length + 1), e = t.filter(function (t) { return 0 === t.linksTo.length }), o = {}, r = this.options.initialPositionRadius, n = t => { for (var e of t.linksFrom || []) o[e.toNode.id] || (o[e.toNode.id] = !0, l.push(e.toNode), n(e.toNode)) }, l = []; for (var h of e) l.push(h), n(h); if (l.length) for (var p of t) -1 === l.indexOf(p) && l.push(p); else l = t; for (let t = 0, e = l.length; t < e; ++t)(i = l[t]).plotX = i.prevX = d(i.plotX, s.width / 2 + r * Math.cos(t * a)), i.plotY = i.prevY = d(i.plotY, s.height / 2 + r * Math.sin(t * a)), i.dispX = 0, i.dispY = 0 } setRandomPositions() { let i, s = this.box, a = this.nodes, o = a.length + 1, r = t => (t = t * t / Math.PI) - Math.floor(t); for (let t = 0, e = a.length; t < e; ++t)(i = a[t]).plotX = i.prevX = d(i.plotX, s.width * r(t)), i.plotY = i.prevY = d(i.plotY, s.height * r(o + t)), i.dispX = 0, i.dispY = 0 } force(t, ...e) { this.integration[t].apply(this, e) } barycenterForces() { this.getBarycenter(), this.force("barycenter") } getBarycenter() { let t = 0, e = 0, i = 0; for (var s of this.nodes) e += s.plotX * s.mass, i += s.plotY * s.mass, t += s.mass; return this.barycenter = { x: e, y: i, xFactor: e / t, yFactor: i / t }, this.barycenter } barnesHutApproximation(t, e) { let i, s; var a = this.getDistXY(t, e), o = this.vectorLength(a); return t !== e && 0 !== o && (e.isInternal ? i = !(e.boxSize / o < this.options.theta && 0 !== o && (s = this.repulsiveForce(o, this.k), this.force("repulsive", t, s * e.mass, a, o), 1)) : (s = this.repulsiveForce(o, this.k), this.force("repulsive", t, s * e.mass, a, o))), i } repulsiveForces() { var t, e, i, s; if ("barnes-hut" === this.approximation) for (let e of this.nodes) this.quadTree.visitNodeRecursive(null, t => this.barnesHutApproximation(e, t)); else for (s of this.nodes) for (var a of this.nodes) s === a || s.fixedPosition || (i = this.getDistXY(s, a), 0 !== (e = this.vectorLength(i)) && (t = this.repulsiveForce(e, this.k), this.force("repulsive", s, t * a.mass, i, e))) } attractiveForces() { var t, e, i, s; for (s of this.links) s.fromNode && s.toNode && (t = this.getDistXY(s.fromNode, s.toNode), 0 !== (e = this.vectorLength(t)) && (i = this.attractiveForce(e, this.k), this.force("attractive", s, i, t, e))) } applyLimits() { for (var t of this.nodes) t.fixedPosition || (this.integration.integrate(this, t), this.applyLimitBox(t, this.box), t.dispX = 0, t.dispY = 0) } applyLimitBox(t, e) { var i = t.radius; t.plotX = n(t.plotX, e.left + i, e.width - i), t.plotY = n(t.plotY, e.top + i, e.height - i) } coolDown(t, e, i) { return t - e * i } isStable() { return Math.abs(this.systemTemperature - this.prevSystemTemperature) < 1e-5 || this.temperature <= 0 } getSystemTemperature() { let t = 0; for (var e of this.nodes) t += e.temperature; return t } vectorLength(t) { return Math.sqrt(t.x * t.x + t.y * t.y) } getDistR(t, e) { return t = this.getDistXY(t, e), this.vectorLength(t) } getDistXY(t, e) { var i = t.plotX - e.plotX, t = t.plotY - e.plotY; return { x: i, y: t, absX: Math.abs(i), absY: Math.abs(t) } } } }), e(t, "Series/PackedBubble/PackedBubbleLayout.js", [t["Series/GraphLayoutComposition.js"], t["Series/PackedBubble/PackedBubbleIntegration.js"], t["Series/Networkgraph/ReingoldFruchtermanLayout.js"], t["Core/Utilities.js"]], function (i, s, a, t) { let { addEvent: o, pick: l } = t; function r() { let t = this.series, e = []; return t.forEach(t => { t.parentNode && t.parentNode.selected && e.push(t.parentNode) }), e } function n() { this.allDataPoints && delete this.allDataPoints } class h extends a { constructor() { super(...arguments), this.index = NaN, this.nodes = [], this.series = [] } static compose(t) { a.compose(t), i.integrations.packedbubble = s, i.layouts.packedbubble = h; let e = t.prototype; e.getSelectedParentNodes || (o(t, "beforeRedraw", n), e.getSelectedParentNodes = r) } beforeStep() { this.options.marker && this.series.forEach(t => { t && t.calculateParentRadius() }) } isStable() { var t = Math.abs(this.prevSystemTemperature - this.systemTemperature); return Math.abs(10 * this.systemTemperature / Math.sqrt(this.nodes.length)) < 1 && t < 1e-5 || this.temperature <= 0 } setCircularPositions() { let t = this.box, e = this.nodes, i = 2 * Math.PI / (e.length + 1), s = this.options.initialPositionRadius, a, o, r = 0; for (var n of e) o = this.options.splitSeries && !n.isParentNode ? (a = n.series.parentNode.plotX, n.series.parentNode.plotY) : (a = t.width / 2, t.height / 2), n.plotX = n.prevX = l(n.plotX, a + s * Math.cos(n.index || r * i)), n.plotY = n.prevY = l(n.plotY, o + s * Math.sin(n.index || r * i)), n.dispX = 0, n.dispY = 0, r++ } repulsiveForces() { let i, s, a, o = this, r = o.options.bubblePadding, t = o.nodes; t.forEach(e => { e.degree = e.mass, e.neighbours = 0, t.forEach(t => { i = 0, e === t || e.fixedPosition || !o.options.seriesInteraction && e.series !== t.series || (a = o.getDistXY(e, t), (s = o.vectorLength(a) - (e.marker.radius + t.marker.radius + r)) < 0 && (e.degree += .01, e.neighbours++, i = o.repulsiveForce(-s / Math.sqrt(e.neighbours), o.k, e, t)), o.force("repulsive", e, i * t.mass, a, t, s)) }) }) } applyLimitBox(t, e) { var i, s; this.options.splitSeries && !t.isParentNode && this.options.parentNodeLimit && (i = this.getDistXY(t, t.series.parentNode), (s = t.series.parentNodeRadius - t.marker.radius - this.vectorLength(i)) < 0 && s > -2 * t.marker.radius && (t.plotX -= .01 * i.x, t.plotY -= .01 * i.y)), super.applyLimitBox(t, e) } } return i.layouts.packedbubble = h }), e(t, "Series/SimulationSeriesUtilities.js", [t["Core/Utilities.js"], t["Core/Animation/AnimationUtilities.js"]], function (t, e) { let { merge: i, syncTimeout: s } = t, a = e["animObject"]; return { initDataLabels: function () { var e = this.options.dataLabels; if (this.dataLabelsGroup) return this.dataLabelsGroup.attr(i({ opacity: 1 }, this.getPlotBox("data-labels"))), this.dataLabelsGroup; { let t = this.initDataLabelsGroup(); return !this.chart.styledMode && e?.style && t.css(e.style), t.attr({ opacity: 0 }), this.visible && t.show(), t } }, initDataLabelsDefer: function () { var t = this.options.dataLabels; t?.defer && this.options.layoutAlgorithm?.enableSimulation ? s(() => { this.deferDataLabels = !1 }, t ? a(t.animation).defer : 0) : this.deferDataLabels = !1 } } }), e(t, "Extensions/TextPath.js", [t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, e) { let x = t["deg2rad"], { addEvent: i, merge: s, uniqueKey: h, defined: p, extend: d } = e; function a(a, t) { t = s(!0, { enabled: !0, attributes: { dy: -5, startOffset: "50%", textAnchor: "middle" } }, t); let o = this.renderer.url, r = this.text || this, e = r.textPath, { attributes: n, enabled: l } = t; return a = a || e && e.path, e && e.undo(), a && l ? (t = i(r, "afterModifyTree", i => { if (a && l) { let t = a.attr("id"), e = (t || a.attr("id", t = h()), { x: 0, y: 0 }); p(n.dx) && (e.dx = n.dx, delete n.dx), p(n.dy) && (e.dy = n.dy, delete n.dy), r.attr(e), this.attr({ transform: "" }), this.box && (this.box = this.box.destroy()); var s = i.nodes.slice(0); i.nodes.length = 0, i.nodes[0] = { tagName: "textPath", attributes: d(n, { "text-anchor": n.textAnchor, href: o + "#" + t }), children: s } } }), r.textPath = { path: a, undo: t }) : (r.attr({ dx: 0, dy: 0 }), delete r.textPath), this.added && (r.textCache = "", this.renderer.buildText(r)), this } function o(h) { let i = h.bBox, p = this.element?.querySelector("textPath"); if (p) { let s = [], { b: a, h: t } = this.renderer.fontMetrics(this.element), o = t - a, e = RegExp('(<tspan>|<tspan(?!\\sclass="highcharts-br")[^>]*>|<\\/tspan>)', "g"), r = p.innerHTML.replace(e, "").split(/<tspan class="highcharts-br"[^>]*>/), n = r.length, l = (t, e) => { var { x: e, y: i } = e, t = (p.getRotationOfChar(t) - 90) * x, s = Math.cos(t), t = Math.sin(t); return [[e - o * s, i - o * t], [e + a * s, i + a * t]] }; for (let e = 0, i = 0; i < n; i++) { var d = r[i].length; for (let t = 0; t < d; t += 5)try { var c = e + t + i, [u, g] = l(c, p.getStartPositionOfChar(c)); 0 === t ? (s.push(g), s.push(u)) : (0 === i && s.unshift(g), i === n - 1 && s.push(u)) } catch (h) { break } e += d - 1; try { var b = e + i, f = p.getEndPositionOfChar(b), [m, y] = l(b, f); s.unshift(y), s.unshift(m) } catch (h) { break } } s.length && s.push(s[0].slice()), i.polygon = s } return i } function r(t) { let e = t.labelOptions, i = t.point, s = e[i.formatPrefix + "TextPath"] || e.textPath; s && !e.useHTML && (this.setTextPath(i.getDataLabelPath?.(this) || i.graphic, s), i.dataLabelPath && !s.enabled && (i.dataLabelPath = i.dataLabelPath.destroy())) } return { compose: function (t) { i(t, "afterGetBBox", o), i(t, "beforeAddingDataLabel", r); let e = t.prototype; e.setTextPath || (e.setTextPath = a) } } }), e(t, "Series/PackedBubble/PackedBubbleSeries.js", [t["Core/Color/Color.js"], t["Series/DragNodesComposition.js"], t["Series/GraphLayoutComposition.js"], t["Core/Globals.js"], t["Series/PackedBubble/PackedBubblePoint.js"], t["Series/PackedBubble/PackedBubbleSeriesDefaults.js"], t["Series/PackedBubble/PackedBubbleLayout.js"], t["Core/Series/SeriesRegistry.js"], t["Series/SimulationSeriesUtilities.js"], t["Core/Utilities.js"], t["Core/Renderer/SVG/SVGElement.js"], t["Extensions/TextPath.js"]], function (t, a, r, e, i, s, o, n, l, h, p, d) { let c = t["parse"], u = e["noop"], { series: { prototype: g }, seriesTypes: { bubble: b } } = n, { initDataLabels: f, initDataLabelsDefer: m } = l, { addEvent: y, clamp: x, defined: P, extend: v, fireEvent: S, isArray: M, isNumber: L, merge: k, pick: C } = h; d.compose(p); class A extends b { constructor() { super(...arguments), this.parentNodeMass = 0, this.deferDataLabels = !0 } static compose(t, e, i) { b.compose(t, e, i), a.compose(e), o.compose(e) } accumulateAllPoints() { var e, i; let t = this.chart, s = []; for (i of t.series) if (i.is("packedbubble") && i.reserveSpace()) { e = i.yData || []; for (let t = 0; t < e.length; t++)s.push([null, null, e[t], i.index, t, { id: t, marker: { radius: 0 } }]) } return s } addLayout() { let t = this.options.layoutAlgorithm = this.options.layoutAlgorithm || {}, e = t.type || "packedbubble", i = this.chart.options.chart, s = this.chart.graphLayoutsStorage, a = this.chart.graphLayoutsLookup, o; s || (this.chart.graphLayoutsStorage = s = {}, this.chart.graphLayoutsLookup = a = []), (o = s[e]) || (t.enableSimulation = P(i.forExport) ? !i.forExport : t.enableSimulation, s[e] = o = new r.layouts[e], o.init(t), a.splice(o.index, 0, o)), this.layout = o, this.points.forEach(t => { t.mass = 2, t.degree = 1, t.collisionNmb = 1 }), o.setArea(0, 0, this.chart.plotWidth, this.chart.plotHeight), o.addElementsToCollection([this], o.series), o.addElementsToCollection(this.points, o.nodes) } addSeriesLayout() { let t = this.options.layoutAlgorithm = this.options.layoutAlgorithm || {}, e = t.type || "packedbubble", i = this.chart.graphLayoutsStorage, s = this.chart.graphLayoutsLookup, a = k(t, t.parentNodeOptions, { enableSimulation: this.layout.options.enableSimulation }), o = i[e + "-series"]; o || (i[e + "-series"] = o = new r.layouts[e], o.init(a), s.splice(o.index, 0, o)), this.parentNodeLayout = o, this.createParentNodes() } calculateParentRadius() { var t = this.seriesBox(); this.parentNodeRadius = x(Math.sqrt(2 * this.parentNodeMass / Math.PI) + 20, 20, t ? Math.max(Math.sqrt(Math.pow(t.width, 2) + Math.pow(t.height, 2)) / 2 + 20, 20) : Math.sqrt(2 * this.parentNodeMass / Math.PI) + 20), this.parentNode && (this.parentNode.marker.radius = this.parentNode.radius = this.parentNodeRadius) } calculateZExtremes() { let t = this.chart.series, e = this.options.zMin, i = this.options.zMax, s = 1 / 0, a = -1 / 0; return e && i ? [e, i] : (t.forEach(t => { t.yData.forEach(t => { P(t) && (t > a && (a = t), t < s && (s = t)) }) }), [e = C(e, s), i = C(i, a)]) } checkOverlap(t, e) { var i = t[0] - e[0], s = t[1] - e[1]; return Math.sqrt(i * i + s * s) - Math.abs(t[2] + e[2]) < -.001 } createParentNodes() { let t = this.pointClass, e = this.chart, i = this.parentNodeLayout, s = this.layout.options, a, o = this.parentNode, r = { radius: this.parentNodeRadius, lineColor: this.color, fillColor: c(this.color).brighten(.4).get() }; s.parentNodeOptions && (r = k(s.parentNodeOptions.marker || {}, r)), this.parentNodeMass = 0, this.points.forEach(t => { this.parentNodeMass += Math.PI * Math.pow(t.marker.radius, 2) }), this.calculateParentRadius(), i.nodes.forEach(t => { t.seriesIndex === this.index && (a = !0) }), i.setArea(0, 0, e.plotWidth, e.plotHeight), a || (o = o || new t(this, { mass: this.parentNodeRadius / 2, marker: r, dataLabels: { inside: !1 }, states: { normal: { marker: r }, hover: { marker: r } }, dataLabelOnNull: !0, degree: this.parentNodeRadius, isParentNode: !0, seriesIndex: this.index }), this.parentNode && (o.plotX = this.parentNode.plotX, o.plotY = this.parentNode.plotY), this.parentNode = o, i.addElementsToCollection([this], i.series), i.addElementsToCollection([o], i.nodes)) } deferLayout() { var t = this.options.layoutAlgorithm; this.visible && (this.addLayout(), t.splitSeries && this.addSeriesLayout()) } destroy() { this.chart.graphLayoutsLookup && this.chart.graphLayoutsLookup.forEach(t => { t.removeElementFromCollection(this, t.series) }, this), this.parentNode && this.parentNodeLayout && (this.parentNodeLayout.removeElementFromCollection(this.parentNode, this.parentNodeLayout.nodes), this.parentNode.dataLabel && (this.parentNode.dataLabel = this.parentNode.dataLabel.destroy())), g.destroy.apply(this, arguments) } drawDataLabels() { this.deferDataLabels || (g.drawDataLabels.call(this, this.points), this.parentNode && (this.parentNode.formatPrefix = "parentNode", g.drawDataLabels.call(this, [this.parentNode]))) } drawGraph() { if (this.layout && this.layout.options.splitSeries) { let t = this.chart, e = this.layout.options.parentNodeOptions.marker, i = { fill: e.fillColor || c(this.color).brighten(.4).get(), opacity: e.fillOpacity, stroke: e.lineColor || this.color, "stroke-width": C(e.lineWidth, this.options.lineWidth) }, s; this.parentNodesGroup = this.plotGroup("parentNodesGroup", "parentNode", this.visible ? "inherit" : "hidden", .1, t.seriesGroup), this.group?.attr({ zIndex: 2 }), this.calculateParentRadius(), this.parentNode && P(this.parentNode.plotX) && P(this.parentNode.plotY) && P(this.parentNodeRadius) && (s = k({ x: this.parentNode.plotX - this.parentNodeRadius, y: this.parentNode.plotY - this.parentNodeRadius, width: 2 * this.parentNodeRadius, height: 2 * this.parentNodeRadius }, i), this.parentNode.graphic || (this.graph = this.parentNode.graphic = t.renderer.symbol(i.symbol).add(this.parentNodesGroup)), this.parentNode.graphic.attr(s)) } } drawTracker() { let t, e = this.parentNode; super.drawTracker(), e && (t = M(e.dataLabels) ? e.dataLabels : e.dataLabel ? [e.dataLabel] : [], e.graphic && (e.graphic.element.point = e), t.forEach(t => { (t.div || t.element).point = e })) } getPointRadius() { let i, s, a, o, t = this.chart, e = t.plotWidth, r = t.plotHeight, n = this.options, l = n.useSimulation, h = Math.min(e, r), p = {}, d = [], c = t.allDataPoints || [], u = c.length, g = (["minSize", "maxSize"].forEach(t => { var e = parseInt(n[t], 10), i = /%$/.test(n[t]); p[t] = i ? h * e / 100 : e * Math.sqrt(u) }), t.minRadius = i = p.minSize / Math.sqrt(u), t.maxRadius = s = p.maxSize / Math.sqrt(u), l ? this.calculateZExtremes() : [i, s]); c.forEach((t, e) => { a = l ? x(t[2], g[0], g[1]) : t[2], 0 === (o = this.getRadius(g[0], g[1], i, s, a)) && (o = null), c[e][2] = o, d.push(o) }), this.radii = d } init() { return g.init.apply(this, arguments), m.call(this), this.eventsToUnbind.push(y(this, "updatedData", function () { this.chart.series.forEach(t => { t.type === this.type && (t.isDirty = !0) }, this) })), this } onMouseUp(s) { if (s.fixedPosition && !s.removed) { let e, i = this.layout, t = this.parentNodeLayout; t && i.options.dragBetweenSeries && t.nodes.forEach(t => { s && s.marker && t !== s.series.parentNode && (e = i.getDistXY(s, t), i.vectorLength(e) - t.marker.radius - s.marker.radius < 0 && (t.series.addPoint(k(s.options, { plotX: s.plotX, plotY: s.plotY }), !1), i.removeElementFromCollection(s, i.nodes), s.remove())) }), a.onMouseUp.apply(this, arguments) } } placeBubbles(t) { let e = this.checkOverlap, i = this.positionBubble, s = [], a = 1, o = 0, r = 0, n, l = [], h, p = t.sort((t, e) => e[2] - t[2]); if (p.length) { if (s.push([[0, 0, p[0][2], p[0][3], p[0][4]]]), 1 < p.length) for (s.push([[0, 0 - p[1][2] - p[0][2], p[1][2], p[1][3], p[1][4]]]), h = 2; h < p.length; h++)p[h][2] = p[h][2] || 1, e(n = i(s[a][o], s[a - 1][r], p[h]), s[a][0]) ? (s.push([]), r = 0, s[a + 1].push(i(s[a][o], s[a][0], p[h])), a++, o = 0) : 1 < a && s[a - 1][r + 1] && e(n, s[a - 1][r + 1]) ? (r++, s[a].push(i(s[a][o], s[a - 1][r], p[h])), o++) : (o++, s[a].push(n)); this.chart.stages = s, this.chart.rawPositions = [].concat.apply([], s), this.resizeRadius(), l = this.chart.rawPositions } return l } pointAttribs(t, e) { let i = this.options, s = t && t.isParentNode, a = i.marker, o = (a = s && i.layoutAlgorithm && i.layoutAlgorithm.parentNodeOptions ? i.layoutAlgorithm.parentNodeOptions.marker : a).fillOpacity, r = g.pointAttribs.call(this, t, e); return 1 !== o && (r["fill-opacity"] = o), r } positionBubble(t, e, i) { let s = Math.asin, a = Math.acos, o = Math.pow, r = Math.abs, n = (0, Math.sqrt)(o(t[0] - e[0], 2) + o(t[1] - e[1], 2)), l = a((o(n, 2) + o(i[2] + e[2], 2) - o(i[2] + t[2], 2)) / (2 * (i[2] + e[2]) * n)), h = s(r(t[0] - e[0]) / n), p = (t[1] - e[1] < 0 ? 0 : Math.PI) + l + h * ((t[0] - e[0]) * (t[1] - e[1]) < 0 ? 1 : -1), d = Math.cos(p), c = Math.sin(p); return [e[0] + (e[2] + i[2]) * c, e[1] - (e[2] + i[2]) * d, i[2], i[3], i[4]] } render() { let e = []; g.render.apply(this, arguments), this.options.dataLabels.allowOverlap || (this.data.forEach(t => { M(t.dataLabels) && t.dataLabels.forEach(t => { e.push(t) }) }), this.options.useSimulation && this.chart.hideOverlappingLabels(e)) } resizeRadius() { let t, e, i, s, a, o = this.chart, r = o.rawPositions, n = Math.min, l = Math.max, h = o.plotLeft, p = o.plotTop, d = o.plotHeight, c = o.plotWidth; for (var u of (t = i = Number.POSITIVE_INFINITY, e = s = Number.NEGATIVE_INFINITY, r)) a = u[2], t = n(t, u[0] - a), e = l(e, u[0] + a), i = n(i, u[1] - a), s = l(s, u[1] + a); var g = [e - t, s - i], b = n.apply([], [(c - h) / g[0], (d - p) / g[1]]); if (1e-10 < Math.abs(b - 1)) { for (var f of r) f[2] *= b; this.placeBubbles(r) } else o.diffY = d / 2 + p - i - (s - i) / 2, o.diffX = c / 2 + h - t - (e - t) / 2 } seriesBox() { let e, t = this.chart, i = this.data, s = Math.max, a = Math.min, o = [t.plotLeft, t.plotLeft + t.plotWidth, t.plotTop, t.plotTop + t.plotHeight]; return i.forEach(t => { P(t.plotX) && P(t.plotY) && t.marker.radius && (e = t.marker.radius, o[0] = a(o[0], t.plotX - e), o[1] = s(o[1], t.plotX + e), o[2] = a(o[2], t.plotY - e), o[3] = s(o[3], t.plotY + e)) }), L(o.width / o.height) ? o : null } setVisible() { let e = this; g.setVisible.apply(e, arguments), e.parentNodeLayout && e.graph ? e.visible ? (e.graph.show(), e.parentNode.dataLabel && e.parentNode.dataLabel.show()) : (e.graph.hide(), e.parentNodeLayout.removeElementFromCollection(e.parentNode, e.parentNodeLayout.nodes), e.parentNode.dataLabel && e.parentNode.dataLabel.hide()) : e.layout && (e.visible ? e.layout.addElementsToCollection(e.points, e.layout.nodes) : e.points.forEach(t => { e.layout.removeElementFromCollection(t, e.layout.nodes) })) } translate() { let t, e, i, s = this.chart, a = this.data, o = this.index, r = this.options.useSimulation; for (var n of (this.processedXData = this.xData, this.generatePoints(), P(s.allDataPoints) || (s.allDataPoints = this.accumulateAllPoints(), this.getPointRadius()), r ? i = s.allDataPoints : (i = this.placeBubbles(s.allDataPoints), this.options.draggable = !1), i)) n[3] === o && (t = a[n[4]], e = C(n[2], void 0), r || (t.plotX = n[0] - s.plotLeft + s.diffX, t.plotY = n[1] - s.plotTop + s.diffY), L(e) && (t.marker = v(t.marker, { radius: e, width: 2 * e, height: 2 * e }), t.radius = e)); r && this.deferLayout(), S(this, "afterTranslate") } } return A.defaultOptions = k(b.defaultOptions, s), v(A.prototype, { pointClass: i, axisTypes: [], directTouch: !0, forces: ["barycenter", "repulsive"], hasDraggableNodes: !0, invertible: !1, isCartesian: !1, noSharedTooltip: !0, pointArrayMap: ["value"], pointValKey: "value", requireSorting: !1, trackerGroups: ["group", "dataLabelsGroup", "parentNodesGroup"], initDataLabels: f, alignDataLabel: g.alignDataLabel, indexateNodes: u, onMouseDown: a.onMouseDown, onMouseMove: a.onMouseMove, redrawHalo: a.redrawHalo, searchPoint: u }), n.registerSeriesType("packedbubble", A), A }), e(t, "Series/Polygon/PolygonSeriesDefaults.js", [], function () { return { marker: { enabled: !1, states: { hover: { enabled: !1 } } }, stickyTracking: !1, tooltip: { followPointer: !0, pointFormat: "" }, trackByArea: !0, legendSymbol: "rectangle" } }), e(t, "Series/Polygon/PolygonSeries.js", [t["Core/Globals.js"], t["Series/Polygon/PolygonSeriesDefaults.js"], t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"]], function (t, e, i, s) { let a = t["noop"], { area: o, line: r, scatter: n } = i.seriesTypes, { extend: l, merge: h } = s; class p extends n { getGraphPath() { let t = r.prototype.getGraphPath.call(this), e = t.length + 1; for (; e--;)(e === t.length || "M" === t[e][0]) && 0 < e && t.splice(e, 0, ["Z"]); return this.areaPath = t } drawGraph() { this.options.fillColor = this.color, o.prototype.drawGraph.call(this) } } return p.defaultOptions = h(n.defaultOptions, e), l(p.prototype, { type: "polygon", drawTracker: r.prototype.drawTracker, setStackedPoints: a }), i.registerSeriesType("polygon", p), p }), e(t, "Core/Axis/RadialAxisDefaults.js", [], function () { return { circular: { gridLineWidth: 1, labels: { align: void 0, x: 0, y: void 0, style: { textOverflow: "none" } }, maxPadding: 0, minPadding: 0, showLastLabel: !1, tickLength: 0 }, radial: { gridLineInterpolation: "circle", gridLineWidth: 1, labels: { align: "right", padding: 5, x: -3, y: -2 }, showLastLabel: !1, title: { x: 4, text: null, rotation: 90 } }, radialGauge: { endOnTick: !1, gridLineWidth: 0, labels: { align: "center", distance: -25, x: 0, y: void 0 }, lineWidth: 1, minorGridLineWidth: 0, minorTickInterval: "auto", minorTickLength: 10, minorTickPosition: "inside", minorTickWidth: 1, startOnTick: !1, tickLength: 10, tickPixelInterval: 100, tickPosition: "inside", tickWidth: 2, title: { rotation: 0, text: "" }, zIndex: 2 } } }), e(t, "Core/Axis/RadialAxis.js", [t["Core/Axis/RadialAxisDefaults.js"], t["Core/Defaults.js"], t["Core/Globals.js"], t["Core/Utilities.js"]], function (t, e, i, s) { var a, n; let l = e["defaultOptions"], { composed: o, noop: h } = i, { addEvent: r, correctFloat: p, defined: m, extend: d, fireEvent: c, isObject: u, merge: g, pick: S, pushUnique: b, relativeLength: C, wrap: f } = s; return (n = a = a || {}).radialDefaultOptions = g(t), n.compose = function (t, e) { return b(o, "Axis.Radial") && (r(t, "afterInit", X), r(t, "autoLabelAlign", R), r(t, "destroy", Y), r(t, "init", j), r(t, "initialAxisTranslation", D), r(e, "afterGetLabelPosition", E), r(e, "afterGetPosition", B), r(i, "setOptions", z), f(e.prototype, "getMarkPath", H)), t }, a; function y() { this.autoConnect = this.isCircular && void 0 === S(this.userMax, this.options.max) && p(this.endAngleRad - this.startAngleRad) === p(2 * Math.PI), !this.isCircular && this.chart.inverted && this.max++, this.autoConnect && (this.max += (this.categories ? 1 : this.pointRange) || this.closestPointRange || 0) } function x() { return () => { if (this.isRadial && this.tickPositions && this.options.labels && !0 !== this.options.labels.allowOverlap) return this.tickPositions.map(t => this.ticks[t] && this.ticks[t].label).filter(t => !!t) } } function P() { return h } function v(t, e, i) { let s = this.pane.center, a = t.value, o, r, n; return this.isCircular ? (m(a) ? t.point && (t.point.shapeArgs || {}).start && (a = this.chart.inverted ? this.translate(t.point.rectPlotY, !0) : t.point.x) : (r = t.chartX || 0, n = t.chartY || 0, a = this.translate(Math.atan2(n - i, r - e) - this.startAngleRad, !0)), r = (o = this.getPosition(a)).x, n = o.y) : (m(a) || (r = t.chartX, n = t.chartY), m(r) && m(n) && (i = s[1] + this.chart.plotTop, a = this.translate(Math.min(Math.sqrt(Math.pow(r - e, 2) + Math.pow(n - i, 2)), s[2] / 2) - s[3] / 2, !0))), [a, r || 0, n || 0] } function M(t, e, i) { let s = this.pane.center, a = this.chart, o = this.left || 0, r = this.top || 0, n, l = S(e, s[2] / 2 - this.offset), h; return (i = void 0 === i ? this.horiz ? 0 : this.center && -this.center[3] / 2 : i) && (l += i), this.isCircular || void 0 !== e ? ((h = this.chart.renderer.symbols.arc(o + s[0], r + s[1], l, l, { start: this.startAngleRad, end: this.endAngleRad, open: !0, innerR: 0 })).xBounds = [o + s[0]], h.yBounds = [r + s[1] - l]) : (n = this.postTranslate(this.angleRad, l), h = [["M", this.center[0] + a.plotLeft, this.center[1] + a.plotTop], ["L", n.x, n.y]]), h } function L() { this.constructor.prototype.getOffset.call(this), this.chart.axisOffset[this.side] = 0 } function k(t, e, i) { let s = this.chart, a = e => { if ("string" != typeof e) return e; { let t = parseInt(e, 10); return t = d.test(e) ? t * n / 100 : t } }, o = this.center, r = this.startAngleRad, n = o[2] / 2, l = Math.min(this.offset, 0), h = this.left || 0, p = this.top || 0, d = /%$/, c = this.isCircular, u, g, b, f, m, y, x = S(a(i.outerRadius), n), P = a(i.innerRadius), v = S(a(i.thickness), 10); return "polygon" === this.options.gridLineInterpolation ? y = this.getPlotLinePath({ value: t }).concat(this.getPlotLinePath({ value: e, reverse: !0 })) : (t = Math.max(t, this.min), e = Math.min(e, this.max), t = this.translate(t), e = this.translate(e), c || (x = t || 0, P = e || 0), "circle" !== i.shape && c ? (u = r + (t || 0), g = r + (e || 0)) : (u = -Math.PI / 2, g = 1.5 * Math.PI, m = !0), x -= l, v -= l, y = s.renderer.symbols.arc(h + o[0], p + o[1], x, x, { start: Math.min(u, g), end: Math.max(u, g), innerR: S(P, x - v), open: m, borderRadius: i.borderRadius }), c && (b = (g + u) / 2, f = h + o[0] + o[2] / 2 * Math.cos(b), y.xBounds = b > -Math.PI / 2 && b < Math.PI / 2 ? [f, s.plotWidth] : [0, f], y.yBounds = [p + o[1] + o[2] / 2 * Math.sin(b)], y.yBounds[0] += b > -Math.PI && b < 0 || b > Math.PI ? -10 : 10)), y } function A(t) { let e = this.pane.center, i = this.chart, s = i.inverted, a = t.reverse, o = this.pane.options.background ? this.pane.options.background[0] || this.pane.options.background : {}, r = o.innerRadius || "0%", n = o.outerRadius || "100%", l = e[0] + i.plotLeft, h = e[1] + i.plotTop, p = this.height, d = t.isCrosshair, c = e[3] / 2, u = t.value, g, b, f, m, y, x, P, v, S, M = this.getPosition(u), L = M.x, k = M.y; if (d && (u = (v = this.getCrosshairPosition(t, l, h))[0], L = v[1], k = v[2]), this.isCircular) b = Math.sqrt(Math.pow(L - l, 2) + Math.pow(k - h, 2)), f = "string" == typeof r ? C(r, 1) : r / b, m = "string" == typeof n ? C(n, 1) : n / b, e && c && (f < (g = c / b) && (f = g), m < g && (m = g)), S = [["M", l + f * (L - l), h - f * (h - k)], ["L", L - (1 - m) * (L - l), k + (1 - m) * (h - k)]]; else if ((u = this.translate(u)) && (u < 0 || u > p) && (u = 0), "circle" === this.options.gridLineInterpolation) S = this.getLinePath(0, u, c); else if (S = [], i[s ? "yAxis" : "xAxis"].forEach(t => { t.pane === this.pane && (y = t) }), y) { P = y.tickPositions, y.autoConnect && (P = P.concat([P[0]])), a && (P = P.slice().reverse()), u && (u += c); for (let t = 0; t < P.length; t++)x = y.getPosition(P[t], u), S.push(t ? ["L", x.x, x.y] : ["M", x.x, x.y]) } return S } function w(t, e) { t = this.translate(t); return this.postTranslate(this.isCircular ? t : this.angleRad, S(this.isCircular ? e : t < 0 ? 0 : t, this.center[2] / 2) - this.offset) } function N() { var t = this.center, e = this.chart, i = this.options.title; return { x: e.plotLeft + t[0] + (i.x || 0), y: e.plotTop + t[1] - { high: .5, middle: .25, low: 0 }[i.align] * t[2] + (i.y || 0) } } function T(t) { t.beforeSetTickPositions = y, t.createLabelCollector = x, t.getCrosshairPosition = v, t.getLinePath = M, t.getOffset = L, t.getPlotBandPath = k, t.getPlotLinePath = A, t.getPosition = w, t.getTitlePosition = N, t.postTranslate = O, t.setAxisSize = G, t.setAxisTranslation = W, t.setOptions = F } function X() { var i = this.chart, s = this.options, a = i.angular && this.isXAxis, o = this.pane, r = o && o.options; if (!a && o && (i.angular || i.polar)) { a = 2 * Math.PI, o = (S(r.startAngle, 0) - 90) * Math.PI / 180, i = (S(r.endAngle, S(r.startAngle, 0) + 360) - 90) * Math.PI / 180; this.angleRad = (s.angle || 0) * Math.PI / 180, this.startAngleRad = o, this.endAngleRad = i, this.offset = s.offset || 0; let t = (o % a + a) % a, e = (i % a + a) % a; t > Math.PI && (t -= a), e > Math.PI && (e -= a), this.normalizedStartAngleRad = t, this.normalizedEndAngleRad = e } } function R(t) { this.isRadial && (t.align = void 0, t.preventDefault()) } function Y() { var t; this.chart && this.chart.labelCollectors && (0 <= (t = this.labelCollector ? this.chart.labelCollectors.indexOf(this.labelCollector) : -1) && this.chart.labelCollectors.splice(t, 1)) } function j(t) { let e, i = this.chart, s = i.angular, a = i.polar, o = this.isXAxis, r = this.coll, n = t.userOptions.pane || 0, l = this.pane = i.pane && i.pane[n]; "colorAxis" === r ? this.isRadial = !1 : (s ? (o ? (this.isHidden = !0, this.createLabelCollector = P, this.getOffset = h, this.redraw = I, this.render = I, this.setScale = h, this.setCategories = h, this.setTitle = h) : T(this), e = !o) : a && (T(this), e = this.horiz), s || a ? (this.isRadial = !0, this.labelCollector || (this.labelCollector = this.createLabelCollector()), this.labelCollector && i.labelCollectors.push(this.labelCollector)) : this.isRadial = !1, l && e && (l.axis = this), this.isCircular = e) } function D() { this.isRadial && this.beforeSetTickPositions() } function E(b) { let f = this.label; if (f) { let t = this.axis, e = f.getBBox(), i = t.options.labels, s = (t.translate(this.pos) + t.startAngleRad + Math.PI / 2) / Math.PI * 180 % 360, a = Math.round(s), o = m(i.y) ? 0 : -.3 * e.height, r = i.y, n, l = 20, h = i.align, p = "end", d = a < 0 ? a + 360 : a, c = d, u = 0, g = 0; t.isRadial && (n = t.getPosition(this.pos, t.center[2] / 2 + C(S(i.distance, -25), t.center[2] / 2, -t.center[2] / 2)), "auto" === i.rotation ? f.attr({ rotation: s }) : m(r) || (r = t.chart.renderer.fontMetrics(f).b - e.height / 2), m(h) || (h = t.isCircular ? s > (l = e.width > t.len * t.tickInterval / (t.max - t.min) ? 0 : l) && s < 180 - l ? "left" : s > 180 + l && s < 360 - l ? "right" : "center" : "center", f.attr({ align: h })), "auto" === h && 2 === t.tickPositions.length && t.isCircular && (90 < d && d < 180 ? d = 180 - d : 270 < d && d <= 360 && (d = 540 - d), 180 < c && c <= 360 && (c = 360 - c), t.pane.options.startAngle !== a && t.pane.options.startAngle !== a + 360 && t.pane.options.startAngle !== a - 360 || (p = "start"), h = -90 <= a && a <= 90 || -360 <= a && a <= -270 || 270 <= a && a <= 360 ? "start" === p ? "right" : "left" : "start" === p ? "left" : "right", 70 < c && c < 110 && (h = "center"), d < 15 || 180 <= d && d < 195 ? u = .3 * e.height : 15 <= d && d <= 35 ? u = "start" === p ? 0 : .75 * e.height : 195 <= d && d <= 215 ? u = "start" === p ? .75 * e.height : 0 : 35 < d && d <= 90 ? u = "start" === p ? -.25 * e.height : e.height : 215 < d && d <= 270 && (u = "start" === p ? e.height : -.25 * e.height), c < 15 ? g = "start" === p ? -.15 * e.height : .15 * e.height : 165 < c && c <= 180 && (g = "start" === p ? .15 * e.height : -.15 * e.height), f.attr({ align: h }), f.translate(g, u + o)), b.pos.x = n.x + (i.x || 0), b.pos.y = n.y + (r || 0)) } } function B(t) { this.axis.getPosition && d(t.pos, this.axis.getPosition(this.pos)) } function z({ options: t }) { t.xAxis && g(!0, n.radialDefaultOptions.circular, t.xAxis), t.yAxis && g(!0, n.radialDefaultOptions.radialGauge, t.yAxis) } function O(t, e) { var i = this.chart, s = this.center; return t = this.startAngleRad + t, { x: i.plotLeft + s[0] + Math.cos(t) * e, y: i.plotTop + s[1] + Math.sin(t) * e } } function I() { this.isDirty = !1 } function G() { let t, e; this.constructor.prototype.setAxisSize.call(this), this.isRadial && (this.pane.updateCenter(this), t = this.center = this.pane.center.slice(), this.isCircular ? this.sector = this.endAngleRad - this.startAngleRad : (e = this.postTranslate(this.angleRad, t[3] / 2), t[0] = e.x - this.chart.plotLeft, t[1] = e.y - this.chart.plotTop), this.len = this.width = this.height = (t[2] - t[3]) * S(this.sector, 1) / 2) } function W() { this.constructor.prototype.setAxisTranslation.call(this), this.center && (this.isCircular ? this.transA = (this.endAngleRad - this.startAngleRad) / (this.max - this.min || 1) : this.transA = (this.center[2] - this.center[3]) / 2 / (this.max - this.min || 1), this.isXAxis ? this.minPixelPadding = this.transA * this.minPointOffset : this.minPixelPadding = 0) } function F(t) { let e = this["coll"], { angular: i, inverted: s, polar: a } = this.chart, o = {}, r = (i ? this.isXAxis || (o = g(l.yAxis, n.radialDefaultOptions.radialGauge)) : a && (o = this.horiz ? g(l.xAxis, n.radialDefaultOptions.circular) : g("xAxis" === e ? l.xAxis : l.yAxis, n.radialDefaultOptions.radial)), s && "yAxis" === e && (o.stackLabels = u(l.yAxis, !0) ? l.yAxis.stackLabels : {}, o.reversedStacks = !0), this.options = g(o, t)); r.plotBands || (r.plotBands = []), c(this, "afterSetOptions") } function H(t, e, i, s, a, o, r) { var n; let l = this.axis; return l.isRadial ? ["M", e, i, "L", (n = l.getPosition(this.pos, l.center[2] / 2 + s)).x, n.y] : t.call(this, e, i, s, a, o, r) } }), e(t, "Series/PolarComposition.js", [t["Core/Animation/AnimationUtilities.js"], t["Core/Globals.js"], t["Core/Series/Series.js"], t["Extensions/Pane/Pane.js"], t["Core/Axis/RadialAxis.js"], t["Core/Utilities.js"]], function (t, f, p, d, c, A) { let m = t["animObject"], u = f["composed"], { addEvent: g, defined: w, find: l, isNumber: N, merge: b, pick: y, pushUnique: x, relativeLength: T, splat: e, uniqueKey: h, wrap: P } = A; function v() { (this.pane || []).forEach(t => { t.render() }) } function S(t) { let e = t.args[0].xAxis, i = t.args[0].yAxis, s = t.args[0].chart; e && i && ("polygon" === i.gridLineInterpolation ? (e.startOnTick = !0, e.endOnTick = !0) : "polygon" === e.gridLineInterpolation && s.inverted && (i.startOnTick = !0, i.endOnTick = !0)) } function M() { this.pane || (this.pane = []), this.options.pane = e(this.options.pane), this.options.pane.forEach(t => { new d(t, this) }, this) } function L(t) { let e = t.args.marker, i = this.chart.xAxis[0], s = this.chart.yAxis[0], a = this.chart.inverted, o = a ? s : i, r = a ? i : s; var n, l, h, p; this.chart.polar && (t.preventDefault(), n = (e.attr ? e.attr("start") : e.start) - o.startAngleRad, l = e.attr ? e.attr("r") : e.r, h = (e.attr ? e.attr("end") : e.end) - o.startAngleRad, p = e.attr ? e.attr("innerR") : e.innerR, t.result.x = n + o.pos, t.result.width = h - n, t.result.y = r.len + r.pos - l, t.result.height = l - p) } function k(t) { var g = this.chart; if (g.polar && g.hoverPane && g.hoverPane.axis) { t.preventDefault(); let o = g.hoverPane.center, s = g.mouseDownX || 0, a = g.mouseDownY || 0, r = t.args.chartY, n = t.args.chartX, l = 2 * Math.PI, h = g.hoverPane.axis.startAngleRad, p = g.hoverPane.axis.endAngleRad, d = (g.inverted ? g.xAxis : g.yAxis)[0], c = {}, u = "arc"; if (c.x = o[0] + g.plotLeft, c.y = o[1] + g.plotTop, this.zoomHor) { let t = 0 < h ? p - h : Math.abs(h) + Math.abs(p), e = Math.atan2(a - g.plotTop - o[1], s - g.plotLeft - o[0]) - h, i = Math.atan2(r - g.plotTop - o[1], n - g.plotLeft - o[0]) - h; c.r = o[2] / 2, c.innerR = o[3] / 2, e <= 0 && (e += l), i <= 0 && (i += l), i < e && (i = [e, e = i][0]), t < l && h + i > p + (l - t) / 2 && (i = e, e = h <= 0 ? h : 0); var b = c.start = Math.max(e + h, h), f = c.end = Math.min(i + h, p); if ("polygon" === d.options.gridLineInterpolation) { let t = g.hoverPane.axis, e = b - t.startAngleRad + t.pos, i = d.getPlotLinePath({ value: d.max }), s = t.toValue(e), a = t.toValue(e + (f - b)); s < t.getExtremes().min && ({ min: f, max: b } = t.getExtremes(), s = b - (f - s)), a < t.getExtremes().min && ({ min: b, max: f } = t.getExtremes(), a = f - (b - a)), a < s && (a = [s, s = a][0]), (i = R(i, s, a, t)).push(["L", o[0] + g.plotLeft, g.plotTop + o[1]]), c.d = i, u = "path" } } if (this.zoomVert) { let t = (g.inverted ? g.xAxis : g.yAxis)[0], e = Math.sqrt(Math.pow(s - g.plotLeft - o[0], 2) + Math.pow(a - g.plotTop - o[1], 2)), i = Math.sqrt(Math.pow(n - g.plotLeft - o[0], 2) + Math.pow(r - g.plotTop - o[1], 2)); i < e && (e = [i, i = e][0]), i > o[2] / 2 && (i = o[2] / 2), e < o[3] / 2 && (e = o[3] / 2), this.zoomHor || (c.start = h, c.end = p), c.r = i, c.innerR = e, "polygon" === t.options.gridLineInterpolation && (f = t.toValue(t.len + t.pos - e), b = t.toValue(t.len + t.pos - i), b = t.getPlotLinePath({ value: b }).concat(t.getPlotLinePath({ value: f, reverse: !0 })), c.d = b, u = "path") } if (this.zoomHor && this.zoomVert && "polygon" === d.options.gridLineInterpolation) { let t = g.hoverPane.axis, e = c.start || 0, i = c.end || 0, s = e - t.startAngleRad + t.pos, a = t.toValue(s), o = t.toValue(s + (i - e)); if (c.d instanceof Array) { let t = c.d.slice(0, c.d.length / 2), e = c.d.slice(c.d.length / 2, c.d.length); e = [...e].reverse(); f = g.hoverPane.axis; t = R(t, a, o, f), (e = R(e, a, o, f)) && (e[0][0] = "L"), e = [...e].reverse(), c.d = t.concat(e), u = "path" } } t.attrs = c, t.shapeType = u } } function C() { var t = this.chart; t.polar && (this.polar = new i(this), t.inverted && (this.isRadialSeries = !0, this.is("column") && (this.isRadialBar = !0))) } function X() { if (this.chart.polar && this.xAxis) { let { xAxis: t, yAxis: e } = this, i = this.chart, s = (this.kdByAngle = i.tooltip && i.tooltip.shared, this.kdByAngle || i.inverted ? this.searchPoint = o : this.options.findNearestPointBy = "xy", this.points), a = s.length; for (; a--;)this.is("column") || this.is("columnrange") || this.polar.toXY(s[a]), i.hasParallelCoordinates || this.yAxis.reversed || (y(s[a].y, Number.MIN_VALUE) < e.min || s[a].x < t.min || s[a].x > t.max ? (s[a].isNull = !0, s[a].plotY = NaN) : s[a].isNull = s[a].isValid && !s[a].isValid()); this.hasClipCircleSetter || (this.hasClipCircleSetter = !!this.eventsToUnbind.push(g(this, "afterRender", function () { var t; i.polar && !1 !== this.options.clip && (t = this.yAxis.pane.center, this.clipCircle ? this.clipCircle.animate({ x: t[0], y: t[1], r: t[2] / 2, innerR: t[3] / 2 }) : this.clipCircle = function (t, e, i, s, a) { let o = h(), r = t.createElement("clipPath").attr({ id: o }).add(t.defs), n = (a ? t.arc(e, i, s, a, 0, 2 * Math.PI) : t.circle(e, i, s)).add(r); return n.id = o, n.clipPath = r, n }(i.renderer, t[0], t[1], t[2] / 2, t[3] / 2), this.group.clip(this.clipCircle), this.setClip = f.noop) }))) } } function o(t) { var e = this.chart, i = this.xAxis, s = this.yAxis, a = i.pane && i.pane.center, o = t.chartX - (a && a[0] || 0) - e.plotLeft, a = t.chartY - (a && a[1] || 0) - e.plotTop, e = e.inverted ? { clientX: t.chartX - s.pos, plotY: t.chartY - i.pos } : { clientX: 180 + -180 / Math.PI * Math.atan2(o, a) }; return this.searchKDTree(e) } function R(t, e, i, s) { let a = s.tickInterval, o = s.tickPositions, r = l(o, t => i <= t), n = l([...o].reverse(), t => t <= e); return w(r) || (r = o[o.length - 1]), w(n) || (n = o[0], r += a, t[0][0] = "L", t.unshift(t[t.length - 3])), (t = t.slice(o.indexOf(n), o.indexOf(r) + 1))[0][0] = "M", t } function Y(t, e) { return l(this.pane || [], t => t.options.id === e) || t.call(this, e) } function j(t, e, i, s, a, o) { var r, n; let l = this.chart, h = y(s.inside, !!this.options.stacking); l.polar ? (n = e.rectPlotX / Math.PI * 180, l.inverted ? (this.forceDL = l.isInsidePlot(e.plotX, e.plotY), h && e.shapeArgs ? (r = e.shapeArgs, a = b(a, { x: (r = this.yAxis.postTranslate(((r.start || 0) + (r.end || 0)) / 2 - this.xAxis.startAngleRad, e.barX + e.pointWidth / 2)).x - l.plotLeft, y: r.y - l.plotTop })) : e.tooltipPos && (a = b(a, { x: e.tooltipPos[0], y: e.tooltipPos[1] })), s.align = y(s.align, "center"), s.verticalAlign = y(s.verticalAlign, "middle")) : (null === (r = s).align && (r.align = 20 < n && n < 160 ? "left" : 200 < n && n < 340 ? "right" : "center"), null === r.verticalAlign && (r.verticalAlign = n < 45 || 315 < n ? "bottom" : 135 < n && n < 225 ? "top" : "middle"), s = r), p.prototype.alignDataLabel.call(this, e, i, s, a, o), this.isRadialBar && e.shapeArgs && e.shapeArgs.start === e.shapeArgs.end ? i.hide() : i.show()) : t.call(this, e, i, s, a, o) } function I() { let t = this.options, e = t.stacking, i = this.chart, s = this.xAxis, a = this.yAxis, o = a.reversed, r = a.center, n = s.startAngleRad, l = s.endAngleRad - n, h = void t.threshold, p = 0, d, c, u, g, b, f = 0, m = 0, y, x, P, v, S, M, L, k; if (s.isRadial) for (u = (d = this.points).length, g = a.translate(a.min), b = a.translate(a.max), h = t.threshold || 0, i.inverted && N(h) && w(p = a.translate(h)) && (p < 0 ? p = 0 : p > l && (p = l), this.translatedThreshold = p + n); u--;) { var C; M = (c = d[u]).barX, x = c.x, P = c.y, c.shapeType = "arc", i.inverted ? (c.plotY = a.translate(P), e && a.stacking ? (S = a.stacking.stacks[(P < 0 ? "-" : "") + this.stackKey], this.visible && S && S[x] && !c.isNull && (v = S[x].points[this.getStackIndicator(void 0, x, this.index).key], f = a.translate(v[0]), m = a.translate(v[1]), w(f) && (f = A.clamp(f, 0, l)))) : (f = p, m = c.plotY), f > m && (m = [f, f = m][0]), o ? m > g ? m = g : f < b ? f = b : (f > g || m < b) && (f = m = l) : f < g ? f = g : m > b ? m = b : (m < g || f > b) && (f = m = 0), a.min > a.max && (f = m = o ? l : 0), f += n, m += n, r && (c.barX = M += r[3] / 2), L = Math.max(M, 0), k = Math.max(M + c.pointWidth, 0), C = t.borderRadius, C = T(("object" == typeof C ? C.radius : C) || 0, k - L), c.shapeArgs = { x: r[0], y: r[1], r: k, innerR: L, start: f, end: m, borderRadius: C }, c.opacity = f === m ? 0 : void 0, c.plotY = (w(this.translatedThreshold) && (f < this.translatedThreshold ? f : m)) - n) : (f = M + n, c.shapeArgs = this.polar.arc(c.yBottom, c.plotY, f, f + c.pointWidth), c.shapeArgs.borderRadius = 0), this.polar.toXY(c), i.inverted ? (y = a.postTranslate(c.rectPlotY, M + c.pointWidth / 2), c.tooltipPos = [y.x - i.plotLeft, y.y - i.plotTop]) : c.tooltipPos = [c.plotX, c.plotY], r && (c.ttBelow = c.plotY > r[1]) } } function D(t, e) { let i, s, a = this; if (this.chart.polar) { e = e || this.points; for (let t = 0; t < e.length; t++)if (!e[t].isNull) { i = t; break } !1 !== this.options.connectEnds && void 0 !== i && (this.connectEnds = !0, e.splice(e.length, 0, e[i]), s = !0), e.forEach(t => { void 0 === t.polarPlotY && a.polar.toXY(t) }) } t = t.apply(this, [].slice.call(arguments, 1)); return s && e.pop(), t } function E(t, a) { let o = this.chart, r = { xAxis: [], yAxis: [] }; return o.polar ? o.axes.forEach(t => { var e, i, s; "colorAxis" !== t.coll && (e = t.isXAxis, s = t.center, i = a.chartX - s[0] - o.plotLeft, s = a.chartY - s[1] - o.plotTop, r[e ? "xAxis" : "yAxis"].push({ axis: t, value: t.translate(e ? Math.PI - Math.atan2(i, s) : Math.sqrt(Math.pow(i, 2) + Math.pow(s, 2)), !0) })) }) : r = t.call(this, a), r } function B(t, e) { this.chart.polar || t.call(this, e) } function z(t, e) { let i = this, s = this.chart, a = this.group, o = this.markerGroup, r = this.xAxis && this.xAxis.center, n = s.plotLeft, l = s.plotTop, h = this.options.animation, p, d, c, u, g, b; s.polar ? i.isRadialBar ? e || (i.startAngleRad = y(i.translatedThreshold, i.xAxis.startAngleRad), f.seriesTypes.pie.prototype.animate.call(i, e)) : (h = m(h), i.is("column") ? e || (d = r[3] / 2, i.points.forEach(t => { c = t.graphic, g = (u = t.shapeArgs) && u.r, b = u && u.innerR, c && u && (c.attr({ r: d, innerR: d }), c.animate({ r: g, innerR: b }, i.options.animation)) })) : e ? (p = { translateX: r[0] + n, translateY: r[1] + l, scaleX: .001, scaleY: .001 }, a.attr(p), o && o.attr(p)) : (p = { translateX: n, translateY: l, scaleX: 1, scaleY: 1 }, a.animate(p, h), o && o.animate(p, h))) : t.call(this, e) } function O(t, e, i, s) { let a, o; var r, n; return a = this.chart.polar ? s ? (r = (o = function t(e, i, s, a) { let o, r, n, l, h, p; var d = a ? 1 : 0, i = (o = 0 <= i && i <= e.length - 1 ? i : i < 0 ? e.length - 1 + i : 0) - 1 < 0 ? e.length - (1 + d) : o - 1, d = o + 1 > e.length - 1 ? d : o + 1, c = e[i], d = e[d], u = c.plotX, c = c.plotY, g = d.plotX, d = d.plotY, b = e[o].plotX, f = e[o].plotY, u = (r = (1.5 * b + u) / 2.5, n = (1.5 * f + c) / 2.5, l = (1.5 * b + g) / 2.5, h = (1.5 * f + d) / 2.5, Math.sqrt(Math.pow(r - b, 2) + Math.pow(n - f, 2))), c = Math.sqrt(Math.pow(l - b, 2) + Math.pow(h - f, 2)), g = Math.atan2(n - f, r - b); p = Math.PI / 2 + (g + Math.atan2(h - f, l - b)) / 2, Math.abs(g - p) > Math.PI / 2 && (p -= Math.PI), r = b + Math.cos(p) * u, n = f + Math.sin(p) * u; let m = { rightContX: b + Math.cos(Math.PI + p) * c, rightContY: f + Math.sin(Math.PI + p) * c, leftContX: r, leftContY: n, plotX: b, plotY: f }; return s && (m.prevPointCont = t(e, i, !1, a)), m }(e, s, !0, this.connectEnds)).prevPointCont && o.prevPointCont.rightContX, n = o.prevPointCont && o.prevPointCont.rightContY, ["C", N(r) ? r : o.plotX, N(n) ? n : o.plotY, N(o.leftContX) ? o.leftContX : o.plotX, N(o.leftContY) ? o.leftContY : o.plotY, o.plotX, o.plotY]) : ["M", i.plotX, i.plotY] : t.call(this, e, i, s) } function G(t, e, i = this.plotY) { var s, a; if (!this.destroyed) return { plotX: s, series: a } = this, a = a["chart"], a.polar && N(s) && N(i) ? [s + (e ? a.plotLeft : 0), i + (e ? a.plotTop : 0)] : t.call(this, e, i) } class i { static compose(t, e, i, s, a, o, r, n, l, h) { var p; d.compose(e, i), c.compose(t, a), x(u, "Polar") && (t = e.prototype, a = o.prototype, o = i.prototype, p = s.prototype, g(e, "afterDrawChartBox", v), g(e, "getAxes", M), g(e, "init", S), P(t, "get", Y), P(o, "getCoordinates", E), P(o, "pinch", B), g(i, "getSelectionMarkerAttrs", k), g(i, "getSelectionBox", L), g(s, "afterInit", C), g(s, "afterTranslate", X, { order: 2 }), g(s, "afterColumnTranslate", I, { order: 4 }), P(p, "animate", z), P(a, "pos", G), n && (e = n.prototype, P(e, "alignDataLabel", j), P(e, "animate", z)), l && P(l.prototype, "getGraphPath", D), h && (t = h.prototype, P(t, "getPointSpline", O), r && (r.prototype.getPointSpline = t.getPointSpline))) } constructor(t) { this.series = t } arc(t, e, i, s) { let a = this.series, o = a.xAxis.center, r = a.yAxis.len, n = o[3] / 2, l = r - e + n, h = r - y(t, r) + n; return a.yAxis.reversed && (l < 0 && (l = n), h < 0 && (h = n)), { x: o[0], y: o[1], r: l, innerR: h, start: i, end: s } } toXY(t) { let e = this.series, i = e.chart, s = e.xAxis, a = e.yAxis, o = t.plotX, r = i.inverted, n = t.y, l = t.plotY, h = r ? o : a.len - l, p; var d; r && e && !e.isRadialBar && (t.plotY = l = N(n) ? a.translate(n) : 0), t.rectPlotX = o, t.rectPlotY = l, a.center && (h += a.center[3] / 2), N(l) && (d = r ? a.postTranslate(l, h) : s.postTranslate(o, h), t.plotX = t.polarPlotX = d.x - i.plotLeft, t.plotY = t.polarPlotY = d.y - i.plotTop), e.kdByAngle ? ((p = (o / Math.PI * 180 + s.pane.options.startAngle) % 360) < 0 && (p += 360), t.clientX = p) : t.clientX = t.plotX } } return class { static compose(t, e, i, s, a, o, r, n, l, h) { var p; d.compose(e, i), c.compose(t, a), x(u, "Polar") && (t = e.prototype, a = o.prototype, o = i.prototype, p = s.prototype, g(e, "afterDrawChartBox", v), g(e, "getAxes", M), g(e, "init", S), P(t, "get", Y), P(o, "getCoordinates", E), P(o, "pinch", B), g(i, "getSelectionMarkerAttrs", k), g(i, "getSelectionBox", L), g(s, "afterInit", C), g(s, "afterTranslate", X, { order: 2 }), g(s, "afterColumnTranslate", I, { order: 4 }), P(p, "animate", z), P(a, "pos", G), n && (e = n.prototype, P(e, "alignDataLabel", j), P(e, "animate", z)), l && P(l.prototype, "getGraphPath", D), h && (t = h.prototype, P(t, "getPointSpline", O), r && (r.prototype.getPointSpline = t.getPointSpline))) } constructor(t) { this.series = t } arc(t, e, i, s) { let a = this.series, o = a.xAxis.center, r = a.yAxis.len, n = o[3] / 2, l = r - e + n, h = r - y(t, r) + n; return a.yAxis.reversed && (l < 0 && (l = n), h < 0 && (h = n)), { x: o[0], y: o[1], r: l, innerR: h, start: i, end: s } } toXY(t) { let e = this.series, i = e.chart, s = e.xAxis, a = e.yAxis, o = t.plotX, r = i.inverted, n = t.y, l = t.plotY, h = r ? o : a.len - l, p; var d; r && e && !e.isRadialBar && (t.plotY = l = N(n) ? a.translate(n) : 0), t.rectPlotX = o, t.rectPlotY = l, a.center && (h += a.center[3] / 2), N(l) && (d = r ? a.postTranslate(l, h) : s.postTranslate(o, h), t.plotX = t.polarPlotX = d.x - i.plotLeft, t.plotY = t.polarPlotY = d.y - i.plotTop), e.kdByAngle ? ((p = (o / Math.PI * 180 + s.pane.options.startAngle) % 360) < 0 && (p += 360), t.clientX = p) : t.clientX = t.plotX } } }), e(t, "Core/Axis/WaterfallAxis.js", [t["Core/Globals.js"], t["Core/Axis/Stacking/StackItem.js"], t["Core/Utilities.js"]], function (t, a, e) { var i; let s = t["composed"], { addEvent: o, objectEach: r, pushUnique: n } = e; { function l() { let t = this.waterfall.stacks; t && (t.changed = !1, delete t.alreadyChanged) } function h() { var t = this.options.stackLabels; t && t.enabled && this.waterfall.stacks && this.waterfall.renderStackTotals() } function p() { this.waterfall || (this.waterfall = new c(this)) } function d() { var t, e = this.axes; for (t of this.series) if (t.options.stacking) { for (var i of e) i.isXAxis || (i.waterfall.stacks.changed = !0); break } } (t = i = i || {}).compose = function (t, e) { n(s, "Axis.Waterfall") && (o(t, "init", p), o(t, "afterBuildStacks", l), o(t, "afterRender", h), o(e, "beforeRedraw", d)) }; class c { constructor(t) { this.axis = t, this.stacks = { changed: !1 } } renderStackTotals() { let t = this.axis, e = t.waterfall.stacks, i = t.stacking && t.stacking.stackTotalGroup, s = new a(t, t.options.stackLabels || {}, !1, 0, void 0); this.dummyStackItem = s, i && r(e, t => { r(t, (t, e) => { s.total = t.stackTotal, s.x = +e, t.label && (s.label = t.label), a.prototype.render.call(s, i), t.label = s.label, delete s.label }) }), s.total = null } } t.Composition = c } return i }), e(t, "Series/Waterfall/WaterfallPoint.js", [t["Series/Column/ColumnSeries.js"], t["Core/Series/Point.js"], t["Core/Utilities.js"]], function (t, e, i) { let s = i["isNumber"]; class a extends t.prototype.pointClass { getClassName() { let t = e.prototype.getClassName.call(this); return this.isSum ? t += " highcharts-sum" : this.isIntermediateSum && (t += " highcharts-intermediate-sum"), t } isValid() { return s(this.y) || this.isSum || !!this.isIntermediateSum } } return a }), e(t, "Series/Waterfall/WaterfallSeriesDefaults.js", [], function () { return { dataLabels: { inside: !0 }, lineWidth: 1, lineColor: "#333333", dashStyle: "Dot", borderColor: "#333333", states: { hover: { lineWidthPlus: 0 } } } }), e(t, "Series/Waterfall/WaterfallSeries.js", [t["Core/Series/SeriesRegistry.js"], t["Core/Utilities.js"], t["Core/Axis/WaterfallAxis.js"], t["Series/Waterfall/WaterfallPoint.js"], t["Series/Waterfall/WaterfallSeriesDefaults.js"]], function (t, e, i, s, a) { let { column: o, line: r } = t.seriesTypes, { addEvent: n, arrayMax: l, arrayMin: h, correctFloat: c, crisp: x, extend: P, isNumber: v, merge: S, objectEach: p, pick: M } = e; function L(t, e) { return Object.hasOwnProperty.call(t, e) } class d extends o { generatePoints() { o.prototype.generatePoints.apply(this); for (let i = 0, t = this.points.length; i < t; i++) { let t = this.points[i], e = this.processedYData[i]; v(e) && (t.isIntermediateSum || t.isSum) && (t.y = c(e)) } } processData(t) { let e, i, s, a, o, r, n = this.options, l = this.yData, h = n.data, p = l.length, d = n.threshold || 0; for (let t = s = i = a = o = 0; t < p; t++)r = l[t], e = h && h[t] ? h[t] : {}, "sum" === r || e.isSum ? l[t] = c(s) : "intermediateSum" === r || e.isIntermediateSum ? (l[t] = c(i), i = 0) : (s += r, i += r), a = Math.min(s, a), o = Math.max(s, o); super.processData.call(this, t), n.stacking || (this.dataMin = a + d, this.dataMax = o) } toYData(t) { return t.isSum ? "sum" : t.isIntermediateSum ? "intermediateSum" : t.y } updateParallelArrays(t, e) { super.updateParallelArrays.call(this, t, e), "sum" !== this.yData[0] && "intermediateSum" !== this.yData[0] || (this.yData[0] = null) } pointAttribs(t, e) { var i = this.options.upColor; i && !t.options.color && v(t.y) && (t.color = 0 < t.y ? i : void 0); let s = o.prototype.pointAttribs.call(this, t, e); return delete s.dashstyle, s } getGraphPath() { return [["M", 0, 0]] } getCrispPath() { let t = this.data.filter(t => v(t.y)), i = this.yAxis, s = t.length, a = this.graph?.strokeWidth() || 0, o = this.xAxis.reversed, r = this.yAxis.reversed, n = this.options.stacking, l = []; for (let e = 1; e < s; e++)if (this.options.connectNulls || v(this.data[t[e].index - 1].y)) { var h = t[e].box, p = t[e - 1], d = p.y || 0, c = t[e - 1].box; if (h && c) { var u = i.waterfall.stacks[this.stackKey], g = 0 < d ? -c.height : 0; if (u && c && h) { let t; var u = u[e - 1]; t = n ? (u = u.connectorThreshold, x(i.translate(u, !1, !0, !1, !0) + (r ? g : 0), a)) : x(c.y + (p.minPointLengthOffset || 0), a), l.push(["M", (c.x || 0) + (!o && c.width || 0), t], ["L", (h.x || 0) + (o && h.width || 0), t]) } if (c && l.length && (!n && d < 0 && !r || 0 < d && r)) { let t = l[l.length - 2], e = (t && "number" == typeof t[2] && (t[2] += c.height || 0), l[l.length - 1]); e && "number" == typeof e[2] && (e[2] += c.height || 0) } } } return l } drawGraph() { r.prototype.drawGraph.call(this), this.graph && this.graph.attr({ d: this.getCrispPath() }) } setStackedPoints(t) { let i = this.options, s = t.waterfall?.stacks, a = i.threshold || 0, o = this.stackKey, r = this.xData, n = r.length, l = a, h = l, p, d = 0, c = 0, u = 0, g, b, f, m, y, x, P, v, S = (t, e, i, s) => { if (p) { if (g) for (; i < g; i++)p.stackState[i] += s; else p.stackState[0] = t, g = p.stackState.length; p.stackState.push(p.stackState[g - 1] + e) } }; if (t.stacking && s && this.reserveSpace()) { v = s.changed, (P = s.alreadyChanged) && P.indexOf(o) < 0 && (v = !0), s[o] || (s[o] = {}); let e = s[o]; if (e) for (let t = 0; t < n; t++)e[x = r[t]] && !v || (e[x] = { negTotal: 0, posTotal: 0, stackTotal: 0, threshold: 0, stateIndex: 0, stackState: [], label: v && e[x] ? e[x].label : void 0 }), p = e[x], 0 <= (y = this.yData[t]) ? p.posTotal += y : p.negTotal += y, m = i.data[t], b = p.absolutePos = p.posTotal, f = p.absoluteNeg = p.negTotal, p.stackTotal = b + f, g = p.stackState.length, m && m.isIntermediateSum ? (S(u, c, 0, u), u = c, c = a, l ^= h, h ^= l, l ^= h) : m && m.isSum ? (S(a, d, g, 0), l = a) : (S(l, y, 0, d), m && (d += y, c += y)), p.stateIndex++, p.threshold = l, l += p.stackTotal; s.changed = !1, s.alreadyChanged || (s.alreadyChanged = []), s.alreadyChanged.push(o) } } getExtremes() { let t, e, i; var s = this.options.stacking; return s ? (t = this.yAxis.waterfall.stacks, e = this.stackedYNeg = [], i = this.stackedYPos = [], "overlap" === s ? p(t[this.stackKey], function (t) { e.push(h(t.stackState)), i.push(l(t.stackState)) }) : p(t[this.stackKey], function (t) { e.push(t.negTotal + t.threshold), i.push(t.posTotal + t.threshold) }), { dataMin: h(e), dataMax: l(i) }) : { dataMin: this.dataMin, dataMax: this.dataMax } } } return d.defaultOptions = S(o.defaultOptions, a), d.compose = i.compose, P(d.prototype, { pointValKey: "y", showLine: !0, pointClass: s }), n(d, "afterColumnTranslate", function () { let { options: t, points: e, yAxis: o } = this, r = M(t.minPointLength, 5), n = r / 2, l = t.threshold || 0, h = t.stacking, p = o.waterfall.stacks[this.stackKey], d = l, c = l, u, g, b, f; for (let a = 0; a < e.length; a++) { let i = e[a], t = this.processedYData[a], s = P({ x: 0, y: 0, width: 0, height: 0 }, i.shapeArgs || {}); i.box = s; var m = [0, t], y = i.y || 0; if (h) { if (p) { let t = p[a], e = ("overlap" === h ? (g = t.stackState[t.stateIndex--], u = 0 <= y ? g : g - y, L(t, "absolutePos") && delete t.absolutePos, L(t, "absoluteNeg") && delete t.absoluteNeg) : (u = 0 <= y ? (g = t.threshold + t.posTotal, t.posTotal -= y, g) : (g = t.threshold + t.negTotal, t.negTotal -= y, g - y), !t.posTotal && v(t.absolutePos) && L(t, "absolutePos") && (t.posTotal = t.absolutePos, delete t.absolutePos), !t.negTotal && v(t.absoluteNeg) && L(t, "absoluteNeg") && (t.negTotal = t.absoluteNeg, delete t.absoluteNeg)), i.isSum || (t.connectorThreshold = t.threshold + t.stackTotal), f = o.reversed ? (b = 0 <= y ? u - y : u + y, u) : (b = u) - y, i.below = b <= l, s.y = o.translate(b, !1, !0, !1, !0), s.height = Math.abs(s.y - o.translate(f, !1, !0, !1, !0)), o.waterfall.dummyStackItem); e && (e.x = a, e.label = p[a].label, e.setOffset(this.pointXOffset || 0, this.barW || 0, this.stackedYNeg[a], this.stackedYPos[a], void 0, this.xAxis)) } } else u = Math.max(c, c + y) + m[0], s.y = o.translate(u, !1, !0, !1, !0), i.isSum ? (s.y = o.translate(m[1], !1, !0, !1, !0), s.height = Math.min(o.translate(m[0], !1, !0, !1, !0), o.len) - s.y, i.below = m[1] <= l) : i.isIntermediateSum ? (f = 0 <= y ? (b = m[1] + d, d) : (b = d, m[1] + d), o.reversed && (b ^= f, f ^= b, b ^= f), s.y = o.translate(b, !1, !0, !1, !0), s.height = Math.abs(s.y - Math.min(o.translate(f, !1, !0, !1, !0), o.len)), d += m[1], i.below = b <= l) : (s.height = 0 < t ? o.translate(c, !1, !0, !1, !0) - s.y : o.translate(c, !1, !0, !1, !0) - o.translate(c - t, !1, !0, !1, !0), c += t, i.below = c < l), s.height < 0 && (s.y += s.height, s.height *= -1); i.plotY = s.y, i.yBottom = s.y + s.height, s.height <= r && !i.isNull ? (s.height = r, s.y -= n, i.yBottom = s.y + s.height, i.plotY = s.y, i.minPointLengthOffset = y < 0 ? -n : n) : (i.isNull && (s.width = 0), i.minPointLengthOffset = 0); m = i.plotY + (i.negative ? s.height : 0), y = (i.below && (i.plotY += s.height), i.tooltipPos && (this.chart.inverted ? i.tooltipPos[0] = o.len - m : i.tooltipPos[1] = m), i.isInside = this.isPointInside(i), x(i.yBottom, this.borderWidth)); s.y = x(s.y, this.borderWidth), s.height = y - s.y, S(!0, i.shapeArgs, s) } }, { order: 2 }), t.registerSeriesType("waterfall", d), d }), e(t, "masters/highcharts-more.src.js", [t["Core/Globals.js"], t["Core/Series/SeriesRegistry.js"], t["Extensions/Pane/Pane.js"], t["Series/Bubble/BubbleSeries.js"], t["Series/PackedBubble/PackedBubbleSeries.js"], t["Series/PolarComposition.js"], t["Core/Axis/RadialAxis.js"], t["Series/Waterfall/WaterfallSeries.js"]], function (t, e, i, s, a, o, r, n) { return t.RadialAxis = r, s.compose(t.Axis, t.Chart, t.Legend), a.compose(t.Axis, t.Chart, t.Legend), i.compose(t.Chart, t.Pointer), o.compose(t.Axis, t.Chart, t.Pointer, t.Series, t.Tick, t.Point, e.seriesTypes.areasplinerange, e.seriesTypes.column, e.seriesTypes.line, e.seriesTypes.spline), n.compose(t.Axis, t.Chart), t }) });;
!function (t) { "object" == typeof module && module.exports ? module.exports = t.default = t : "function" == typeof define && define.amd ? define("highcharts/modules/solid-gauge", ["highcharts", "highcharts/highcharts-more"], function (e) { return t(e), t.Highcharts = e, t }) : t("undefined" != typeof Highcharts ? Highcharts : void 0) }(function (i) { "use strict"; var e = i ? i._modules : {}; function t(e, t, o, s) { e.hasOwnProperty(t) || (e[t] = s.apply(null, o), "function" == typeof CustomEvent && i.win.dispatchEvent(new CustomEvent("HighchartsModuleLoaded", { detail: { path: t, module: e[t] } }))) } t(e, "Core/Axis/Color/ColorAxisLike.js", [e["Core/Color/Color.js"], e["Core/Utilities.js"]], function (e, t) { let h = e["parse"], u = t["merge"]; return e = { initDataClasses: function (e) { let o = this.chart, t = this.legendItem = this.legendItem || {}, s = this.options, i = e.dataClasses || [], r, a, l = o.options.chart.colorCount, n = 0, d; this.dataClasses = a = [], t.labels = []; for (let e = 0, t = i.length; e < t; ++e)r = u(r = i[e]), a.push(r), !o.styledMode && r.color || ("category" === s.dataClassColor ? (o.styledMode || (l = (d = o.options.colors || []).length, r.color = d[n]), r.colorIndex = n, ++n === l && (n = 0)) : r.color = h(s.minColor).tweenTo(h(s.maxColor), t < 2 ? .5 : e / (t - 1))) }, initStops: function () { let e = this.options, o = this.stops = e.stops || [[0, e.minColor || ""], [1, e.maxColor || ""]]; for (let e = 0, t = o.length; e < t; ++e)o[e].color = h(o[e][1]) }, normalizedValue: function (e) { var t = this.max || 0, o = this.min || 0; return 1 - (t - (e = this.logarithmic ? this.logarithmic.log2lin(e) : e)) / (t - o || 1) }, toColor: function (e, t) { let o, s, i, r, a, l; var n = this.dataClasses, d = this.stops; if (n) { for (l = n.length; l--;)if (s = (a = n[l]).from, i = a.to, (void 0 === s || e >= s) && (void 0 === i || e <= i)) { r = a.color, t && (t.dataClass = l, t.colorIndex = a.colorIndex); break } } else { for (o = this.normalizedValue(e), l = d.length; l-- && !(o > d[l][0]);); s = d[l] || d[l + 1], o = 1 - ((i = d[l + 1] || s)[0] - o) / (i[0] - s[0] || 1), r = s.color.tweenTo(i.color, o) } return r } }, e }), t(e, "Core/Axis/SolidGaugeAxis.js", [e["Core/Axis/Color/ColorAxisLike.js"], e["Core/Utilities.js"]], function (t, e) { let o = e["extend"]; return { init: function (e) { o(e, t) } } }), t(e, "Series/SolidGauge/SolidGaugeSeriesDefaults.js", [], function () { return { colorByPoint: !0, dataLabels: { y: 0 } } }), t(e, "Series/SolidGauge/SolidGaugeSeries.js", [e["Extensions/BorderRadius.js"], e["Core/Series/SeriesRegistry.js"], e["Core/Axis/SolidGaugeAxis.js"], e["Series/SolidGauge/SolidGaugeSeriesDefaults.js"], e["Core/Utilities.js"]], function (y, e, t, o, s) { let { gauge: i, pie: r } = e.seriesTypes, { clamp: R, extend: S, isNumber: a, merge: l, pick: j, pInt: v } = s; class n extends i { translate() { let e = this.yAxis; t.init(e), !e.dataClasses && e.options.dataClasses && e.initDataClasses(e.options), e.initStops(), i.prototype.translate.call(this) } drawPoints() { let e, g = this.yAxis, p = g.center, f = this.options, C = this.chart.renderer, t = f.overshoot, m = f.rounded && void 0 === f.borderRadius, x = a(t) ? t / 180 * Math.PI : 0; for (var A of (a(f.threshold) && (e = g.startAngleRad + g.translate(f.threshold, void 0, void 0, void 0, !0)), this.thresholdAngleRad = j(e, g.startAngleRad), this.points)) if (!A.isNull) { let e = v(j(A.options.radius, f.radius, 100)) * p[2] / 200, t = v(j(A.options.innerRadius, f.innerRadius, 60)) * p[2] / 200, o = Math.min(g.startAngleRad, g.endAngleRad), s = Math.max(g.startAngleRad, g.endAngleRad), i = A.graphic, r = g.startAngleRad + g.translate(A.y, void 0, void 0, void 0, !0), a, l, n = g.toColor(A.y, A), d = ("none" !== (n = "none" === n ? A.color || this.color || "none" : n) && (A.color = n), r = R(r, o - x, s + x), !1 === f.wrap && (r = R(r, o, s)), m ? (e - t) / 2 / e : 0), h = Math.min(r, this.thresholdAngleRad) - d, u = Math.max(r, this.thresholdAngleRad) + d, c = (u - h > 2 * Math.PI && (u = h + 2 * Math.PI), m ? "50%" : 0); f.borderRadius && (c = y.optionsToObject(f.borderRadius).radius), A.shapeArgs = a = { x: p[0], y: p[1], r: e, innerR: t, start: h, end: u, borderRadius: c }, A.startR = e, i ? (l = a.d, i.animate(S({ fill: n }, a)), l && (a.d = l)) : A.graphic = i = C.arc(a).attr({ fill: n, "sweep-flag": 0 }).add(this.group), this.chart.styledMode || ("square" !== f.linecap && i.attr({ "stroke-linecap": "round", "stroke-linejoin": "round" }), i.attr({ stroke: f.borderColor || "none", "stroke-width": f.borderWidth || 0 })), i && i.addClass(A.getClassName(), !0) } } animate(e) { e || (this.startAngleRad = this.thresholdAngleRad, r.prototype.animate.call(this, e)) } } return n.defaultOptions = l(i.defaultOptions, o), e.registerSeriesType("solidgauge", n), n }), t(e, "masters/modules/solid-gauge.src.js", [e["Core/Globals.js"]], function (e) { return e }) });;
/* --- FLATPICKR MIN JS --- */
/* flatpickr v4.6.13,, @license MIT */
!function (e, n) { "object" == typeof exports && "undefined" != typeof module ? module.exports = n() : "function" == typeof define && define.amd ? define(n) : (e = "undefined" != typeof globalThis ? globalThis : e || self).flatpickr = n() }(this, (function () { "use strict"; var e = function () { return (e = Object.assign || function (e) { for (var n, t = 1, a = arguments.length; t < a; t++)for (var i in n = arguments[t]) Object.prototype.hasOwnProperty.call(n, i) && (e[i] = n[i]); return e }).apply(this, arguments) }; function n() { for (var e = 0, n = 0, t = arguments.length; n < t; n++)e += arguments[n].length; var a = Array(e), i = 0; for (n = 0; n < t; n++)for (var o = arguments[n], r = 0, l = o.length; r < l; r++, i++)a[i] = o[r]; return a } var t = ["onChange", "onClose", "onDayCreate", "onDestroy", "onKeyDown", "onMonthChange", "onOpen", "onParseConfig", "onReady", "onValueUpdate", "onYearChange", "onPreCalendarPosition"], a = { _disable: [], allowInput: !1, allowInvalidPreload: !1, altFormat: "F j, Y", altInput: !1, altInputClass: "form-control input", animate: "object" == typeof window && -1 === window.navigator.userAgent.indexOf("MSIE"), ariaDateFormat: "F j, Y", autoFillDefaultTime: !0, clickOpens: !0, closeOnSelect: !0, conjunction: ", ", dateFormat: "Y-m-d", defaultHour: 12, defaultMinute: 0, defaultSeconds: 0, disable: [], disableMobile: !1, enableSeconds: !1, enableTime: !1, errorHandler: function (e) { return "undefined" != typeof console && console.warn(e) }, getWeek: function (e) { var n = new Date(e.getTime()); n.setHours(0, 0, 0, 0), n.setDate(n.getDate() + 3 - (n.getDay() + 6) % 7); var t = new Date(n.getFullYear(), 0, 4); return 1 + Math.round(((n.getTime() - t.getTime()) / 864e5 - 3 + (t.getDay() + 6) % 7) / 7) }, hourIncrement: 1, ignoredFocusElements: [], inline: !1, locale: "default", minuteIncrement: 5, mode: "single", monthSelectorType: "dropdown", nextArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M13.207 8.472l-7.854 7.854-0.707-0.707 7.146-7.146-7.146-7.148 0.707-0.707 7.854 7.854z' /></svg>", noCalendar: !1, now: new Date, onChange: [], onClose: [], onDayCreate: [], onDestroy: [], onKeyDown: [], onMonthChange: [], onOpen: [], onParseConfig: [], onReady: [], onValueUpdate: [], onYearChange: [], onPreCalendarPosition: [], plugins: [], position: "auto", positionElement: void 0, prevArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M5.207 8.471l7.146 7.147-0.707 0.707-7.853-7.854 7.854-7.853 0.707 0.707-7.147 7.146z' /></svg>", shorthandCurrentMonth: !1, showMonths: 1, static: !1, time_24hr: !1, weekNumbers: !1, wrap: !1 }, i = { weekdays: { shorthand: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], longhand: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"] }, months: { shorthand: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], longhand: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"] }, daysInMonth: [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31], firstDayOfWeek: 0, ordinal: function (e) { var n = e % 100; if (n > 3 && n < 21) return "th"; switch (n % 10) { case 1: return "st"; case 2: return "nd"; case 3: return "rd"; default: return "th" } }, rangeSeparator: " to ", weekAbbreviation: "Wk", scrollTitle: "Scroll to increment", toggleTitle: "Click to toggle", amPM: ["AM", "PM"], yearAriaLabel: "Year", monthAriaLabel: "Month", hourAriaLabel: "Hour", minuteAriaLabel: "Minute", time_24hr: !1 }, o = function (e, n) { return void 0 === n && (n = 2), ("000" + e).slice(-1 * n) }, r = function (e) { return !0 === e ? 1 : 0 }; function l(e, n) { var t; return function () { var a = this, i = arguments; clearTimeout(t), t = setTimeout((function () { return e.apply(a, i) }), n) } } var c = function (e) { return e instanceof Array ? e : [e] }; function s(e, n, t) { if (!0 === t) return e.classList.add(n); e.classList.remove(n) } function d(e, n, t) { var a = window.document.createElement(e); return n = n || "", t = t || "", a.className = n, void 0 !== t && (a.textContent = t), a } function u(e) { for (; e.firstChild;)e.removeChild(e.firstChild) } function f(e, n) { return n(e) ? e : e.parentNode ? f(e.parentNode, n) : void 0 } function m(e, n) { var t = d("div", "numInputWrapper"), a = d("input", "numInput " + e), i = d("span", "arrowUp"), o = d("span", "arrowDown"); if (-1 === navigator.userAgent.indexOf("MSIE 9.0") ? a.type = "number" : (a.type = "text", a.pattern = "\\d*"), void 0 !== n) for (var r in n) a.setAttribute(r, n[r]); return t.appendChild(a), t.appendChild(i), t.appendChild(o), t } function g(e) { try { return "function" == typeof e.composedPath ? e.composedPath()[0] : e.target } catch (n) { return e.target } } var p = function () { }, h = function (e, n, t) { return t.months[n ? "shorthand" : "longhand"][e] }, v = { D: p, F: function (e, n, t) { e.setMonth(t.months.longhand.indexOf(n)) }, G: function (e, n) { e.setHours((e.getHours() >= 12 ? 12 : 0) + parseFloat(n)) }, H: function (e, n) { e.setHours(parseFloat(n)) }, J: function (e, n) { e.setDate(parseFloat(n)) }, K: function (e, n, t) { e.setHours(e.getHours() % 12 + 12 * r(new RegExp(t.amPM[1], "i").test(n))) }, M: function (e, n, t) { e.setMonth(t.months.shorthand.indexOf(n)) }, S: function (e, n) { e.setSeconds(parseFloat(n)) }, U: function (e, n) { return new Date(1e3 * parseFloat(n)) }, W: function (e, n, t) { var a = parseInt(n), i = new Date(e.getFullYear(), 0, 2 + 7 * (a - 1), 0, 0, 0, 0); return i.setDate(i.getDate() - i.getDay() + t.firstDayOfWeek), i }, Y: function (e, n) { e.setFullYear(parseFloat(n)) }, Z: function (e, n) { return new Date(n) }, d: function (e, n) { e.setDate(parseFloat(n)) }, h: function (e, n) { e.setHours((e.getHours() >= 12 ? 12 : 0) + parseFloat(n)) }, i: function (e, n) { e.setMinutes(parseFloat(n)) }, j: function (e, n) { e.setDate(parseFloat(n)) }, l: p, m: function (e, n) { e.setMonth(parseFloat(n) - 1) }, n: function (e, n) { e.setMonth(parseFloat(n) - 1) }, s: function (e, n) { e.setSeconds(parseFloat(n)) }, u: function (e, n) { return new Date(parseFloat(n)) }, w: p, y: function (e, n) { e.setFullYear(2e3 + parseFloat(n)) } }, D = { D: "", F: "", G: "(\\d\\d|\\d)", H: "(\\d\\d|\\d)", J: "(\\d\\d|\\d)\\w+", K: "", M: "", S: "(\\d\\d|\\d)", U: "(.+)", W: "(\\d\\d|\\d)", Y: "(\\d{4})", Z: "(.+)", d: "(\\d\\d|\\d)", h: "(\\d\\d|\\d)", i: "(\\d\\d|\\d)", j: "(\\d\\d|\\d)", l: "", m: "(\\d\\d|\\d)", n: "(\\d\\d|\\d)", s: "(\\d\\d|\\d)", u: "(.+)", w: "(\\d\\d|\\d)", y: "(\\d{2})" }, w = { Z: function (e) { return e.toISOString() }, D: function (e, n, t) { return n.weekdays.shorthand[w.w(e, n, t)] }, F: function (e, n, t) { return h(w.n(e, n, t) - 1, !1, n) }, G: function (e, n, t) { return o(w.h(e, n, t)) }, H: function (e) { return o(e.getHours()) }, J: function (e, n) { return void 0 !== n.ordinal ? e.getDate() + n.ordinal(e.getDate()) : e.getDate() }, K: function (e, n) { return n.amPM[r(e.getHours() > 11)] }, M: function (e, n) { return h(e.getMonth(), !0, n) }, S: function (e) { return o(e.getSeconds()) }, U: function (e) { return e.getTime() / 1e3 }, W: function (e, n, t) { return t.getWeek(e) }, Y: function (e) { return o(e.getFullYear(), 4) }, d: function (e) { return o(e.getDate()) }, h: function (e) { return e.getHours() % 12 ? e.getHours() % 12 : 12 }, i: function (e) { return o(e.getMinutes()) }, j: function (e) { return e.getDate() }, l: function (e, n) { return n.weekdays.longhand[e.getDay()] }, m: function (e) { return o(e.getMonth() + 1) }, n: function (e) { return e.getMonth() + 1 }, s: function (e) { return e.getSeconds() }, u: function (e) { return e.getTime() }, w: function (e) { return e.getDay() }, y: function (e) { return String(e.getFullYear()).substring(2) } }, b = function (e) { var n = e.config, t = void 0 === n ? a : n, o = e.l10n, r = void 0 === o ? i : o, l = e.isMobile, c = void 0 !== l && l; return function (e, n, a) { var i = a || r; return void 0 === t.formatDate || c ? n.split("").map((function (n, a, o) { return w[n] && "\\" !== o[a - 1] ? w[n](e, i, t) : "\\" !== n ? n : "" })).join("") : t.formatDate(e, n, i) } }, C = function (e) { var n = e.config, t = void 0 === n ? a : n, o = e.l10n, r = void 0 === o ? i : o; return function (e, n, i, o) { if (0 === e || e) { var l, c = o || r, s = e; if (e instanceof Date) l = new Date(e.getTime()); else if ("string" != typeof e && void 0 !== e.toFixed) l = new Date(e); else if ("string" == typeof e) { var d = n || (t || a).dateFormat, u = String(e).trim(); if ("today" === u) l = new Date, i = !0; else if (t && t.parseDate) l = t.parseDate(e, d); else if (/Z$/.test(u) || /GMT$/.test(u)) l = new Date(e); else { for (var f = void 0, m = [], g = 0, p = 0, h = ""; g < d.length; g++) { var w = d[g], b = "\\" === w, C = "\\" === d[g - 1] || b; if (D[w] && !C) { h += D[w]; var M = new RegExp(h).exec(e); M && (f = !0) && m["Y" !== w ? "push" : "unshift"]({ fn: v[w], val: M[++p] }) } else b || (h += ".") } l = t && t.noCalendar ? new Date((new Date).setHours(0, 0, 0, 0)) : new Date((new Date).getFullYear(), 0, 1, 0, 0, 0, 0), m.forEach((function (e) { var n = e.fn, t = e.val; return l = n(l, t, c) || l })), l = f ? l : void 0 } } if (l instanceof Date && !isNaN(l.getTime())) return !0 === i && l.setHours(0, 0, 0, 0), l; t.errorHandler(new Error("Invalid date provided: " + s)) } } }; function M(e, n, t) { return void 0 === t && (t = !0), !1 !== t ? new Date(e.getTime()).setHours(0, 0, 0, 0) - new Date(n.getTime()).setHours(0, 0, 0, 0) : e.getTime() - n.getTime() } var y = function (e, n, t) { return 3600 * e + 60 * n + t }, x = 864e5; function E(e) { var n = e.defaultHour, t = e.defaultMinute, a = e.defaultSeconds; if (void 0 !== e.minDate) { var i = e.minDate.getHours(), o = e.minDate.getMinutes(), r = e.minDate.getSeconds(); n < i && (n = i), n === i && t < o && (t = o), n === i && t === o && a < r && (a = e.minDate.getSeconds()) } if (void 0 !== e.maxDate) { var l = e.maxDate.getHours(), c = e.maxDate.getMinutes(); (n = Math.min(n, l)) === l && (t = Math.min(c, t)), n === l && t === c && (a = e.maxDate.getSeconds()) } return { hours: n, minutes: t, seconds: a } } "function" != typeof Object.assign && (Object.assign = function (e) { for (var n = [], t = 1; t < arguments.length; t++)n[t - 1] = arguments[t]; if (!e) throw TypeError("Cannot convert undefined or null to object"); for (var a = function (n) { n && Object.keys(n).forEach((function (t) { return e[t] = n[t] })) }, i = 0, o = n; i < o.length; i++) { var r = o[i]; a(r) } return e }); function k(p, v) { var w = { config: e(e({}, a), I.defaultConfig), l10n: i }; function k() { var e; return (null === (e = w.calendarContainer) || void 0 === e ? void 0 : e.getRootNode()).activeElement || document.activeElement } function T(e) { return e.bind(w) } function S() { var e = w.config; !1 === e.weekNumbers && 1 === e.showMonths || !0 !== e.noCalendar && window.requestAnimationFrame((function () { if (void 0 !== w.calendarContainer && (w.calendarContainer.style.visibility = "hidden", w.calendarContainer.style.display = "block"), void 0 !== w.daysContainer) { var n = (w.days.offsetWidth + 1) * e.showMonths; w.daysContainer.style.width = n + "px", w.calendarContainer.style.width = n + (void 0 !== w.weekWrapper ? w.weekWrapper.offsetWidth : 0) + "px", w.calendarContainer.style.removeProperty("visibility"), w.calendarContainer.style.removeProperty("display") } })) } function _(e) { if (0 === w.selectedDates.length) { var n = void 0 === w.config.minDate || M(new Date, w.config.minDate) >= 0 ? new Date : new Date(w.config.minDate.getTime()), t = E(w.config); n.setHours(t.hours, t.minutes, t.seconds, n.getMilliseconds()), w.selectedDates = [n], w.latestSelectedDateObj = n } void 0 !== e && "blur" !== e.type && function (e) { e.preventDefault(); var n = "keydown" === e.type, t = g(e), a = t; void 0 !== w.amPM && t === w.amPM && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]); var i = parseFloat(a.getAttribute("min")), l = parseFloat(a.getAttribute("max")), c = parseFloat(a.getAttribute("step")), s = parseInt(a.value, 10), d = e.delta || (n ? 38 === e.which ? 1 : -1 : 0), u = s + c * d; if (void 0 !== a.value && 2 === a.value.length) { var f = a === w.hourElement, m = a === w.minuteElement; u < i ? (u = l + u + r(!f) + (r(f) && r(!w.amPM)), m && L(void 0, -1, w.hourElement)) : u > l && (u = a === w.hourElement ? u - l - r(!w.amPM) : i, m && L(void 0, 1, w.hourElement)), w.amPM && f && (1 === c ? u + s === 23 : Math.abs(u - s) > c) && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]), a.value = o(u) } }(e); var a = w._input.value; O(), ye(), w._input.value !== a && w._debouncedChange() } function O() { if (void 0 !== w.hourElement && void 0 !== w.minuteElement) { var e, n, t = (parseInt(w.hourElement.value.slice(-2), 10) || 0) % 24, a = (parseInt(w.minuteElement.value, 10) || 0) % 60, i = void 0 !== w.secondElement ? (parseInt(w.secondElement.value, 10) || 0) % 60 : 0; void 0 !== w.amPM && (e = t, n = w.amPM.textContent, t = e % 12 + 12 * r(n === w.l10n.amPM[1])); var o = void 0 !== w.config.minTime || w.config.minDate && w.minDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.minDate, !0), l = void 0 !== w.config.maxTime || w.config.maxDate && w.maxDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.maxDate, !0); if (void 0 !== w.config.maxTime && void 0 !== w.config.minTime && w.config.minTime > w.config.maxTime) { var c = y(w.config.minTime.getHours(), w.config.minTime.getMinutes(), w.config.minTime.getSeconds()), s = y(w.config.maxTime.getHours(), w.config.maxTime.getMinutes(), w.config.maxTime.getSeconds()), d = y(t, a, i); if (d > s && d < c) { var u = function (e) { var n = Math.floor(e / 3600), t = (e - 3600 * n) / 60; return [n, t, e - 3600 * n - 60 * t] }(c); t = u[0], a = u[1], i = u[2] } } else { if (l) { var f = void 0 !== w.config.maxTime ? w.config.maxTime : w.config.maxDate; (t = Math.min(t, f.getHours())) === f.getHours() && (a = Math.min(a, f.getMinutes())), a === f.getMinutes() && (i = Math.min(i, f.getSeconds())) } if (o) { var m = void 0 !== w.config.minTime ? w.config.minTime : w.config.minDate; (t = Math.max(t, m.getHours())) === m.getHours() && a < m.getMinutes() && (a = m.getMinutes()), a === m.getMinutes() && (i = Math.max(i, m.getSeconds())) } } A(t, a, i) } } function F(e) { var n = e || w.latestSelectedDateObj; n && n instanceof Date && A(n.getHours(), n.getMinutes(), n.getSeconds()) } function A(e, n, t) { void 0 !== w.latestSelectedDateObj && w.latestSelectedDateObj.setHours(e % 24, n, t || 0, 0), w.hourElement && w.minuteElement && !w.isMobile && (w.hourElement.value = o(w.config.time_24hr ? e : (12 + e) % 12 + 12 * r(e % 12 == 0)), w.minuteElement.value = o(n), void 0 !== w.amPM && (w.amPM.textContent = w.l10n.amPM[r(e >= 12)]), void 0 !== w.secondElement && (w.secondElement.value = o(t))) } function N(e) { var n = g(e), t = parseInt(n.value) + (e.delta || 0); (t / 1e3 > 1 || "Enter" === e.key && !/[^\d]/.test(t.toString())) && ee(t) } function P(e, n, t, a) { return n instanceof Array ? n.forEach((function (n) { return P(e, n, t, a) })) : e instanceof Array ? e.forEach((function (e) { return P(e, n, t, a) })) : (e.addEventListener(n, t, a), void w._handlers.push({ remove: function () { return e.removeEventListener(n, t, a) } })) } function Y() { De("onChange") } function j(e, n) { var t = void 0 !== e ? w.parseDate(e) : w.latestSelectedDateObj || (w.config.minDate && w.config.minDate > w.now ? w.config.minDate : w.config.maxDate && w.config.maxDate < w.now ? w.config.maxDate : w.now), a = w.currentYear, i = w.currentMonth; try { void 0 !== t && (w.currentYear = t.getFullYear(), w.currentMonth = t.getMonth()) } catch (e) { e.message = "Invalid date supplied: " + t, w.config.errorHandler(e) } n && w.currentYear !== a && (De("onYearChange"), q()), !n || w.currentYear === a && w.currentMonth === i || De("onMonthChange"), w.redraw() } function H(e) { var n = g(e); ~n.className.indexOf("arrow") && L(e, n.classList.contains("arrowUp") ? 1 : -1) } function L(e, n, t) { var a = e && g(e), i = t || a && a.parentNode && a.parentNode.firstChild, o = we("increment"); o.delta = n, i && i.dispatchEvent(o) } function R(e, n, t, a) { var i = ne(n, !0), o = d("span", e, n.getDate().toString()); return o.dateObj = n, o.$i = a, o.setAttribute("aria-label", w.formatDate(n, w.config.ariaDateFormat)), -1 === e.indexOf("hidden") && 0 === M(n, w.now) && (w.todayDateElem = o, o.classList.add("today"), o.setAttribute("aria-current", "date")), i ? (o.tabIndex = -1, be(n) && (o.classList.add("selected"), w.selectedDateElem = o, "range" === w.config.mode && (s(o, "startRange", w.selectedDates[0] && 0 === M(n, w.selectedDates[0], !0)), s(o, "endRange", w.selectedDates[1] && 0 === M(n, w.selectedDates[1], !0)), "nextMonthDay" === e && o.classList.add("inRange")))) : o.classList.add("flatpickr-disabled"), "range" === w.config.mode && function (e) { return !("range" !== w.config.mode || w.selectedDates.length < 2) && (M(e, w.selectedDates[0]) >= 0 && M(e, w.selectedDates[1]) <= 0) }(n) && !be(n) && o.classList.add("inRange"), w.weekNumbers && 1 === w.config.showMonths && "prevMonthDay" !== e && a % 7 == 6 && w.weekNumbers.insertAdjacentHTML("beforeend", "<span class='flatpickr-day'>" + w.config.getWeek(n) + "</span>"), De("onDayCreate", o), o } function W(e) { e.focus(), "range" === w.config.mode && oe(e) } function B(e) { for (var n = e > 0 ? 0 : w.config.showMonths - 1, t = e > 0 ? w.config.showMonths : -1, a = n; a != t; a += e)for (var i = w.daysContainer.children[a], o = e > 0 ? 0 : i.children.length - 1, r = e > 0 ? i.children.length : -1, l = o; l != r; l += e) { var c = i.children[l]; if (-1 === c.className.indexOf("hidden") && ne(c.dateObj)) return c } } function J(e, n) { var t = k(), a = te(t || document.body), i = void 0 !== e ? e : a ? t : void 0 !== w.selectedDateElem && te(w.selectedDateElem) ? w.selectedDateElem : void 0 !== w.todayDateElem && te(w.todayDateElem) ? w.todayDateElem : B(n > 0 ? 1 : -1); void 0 === i ? w._input.focus() : a ? function (e, n) { for (var t = -1 === e.className.indexOf("Month") ? e.dateObj.getMonth() : w.currentMonth, a = n > 0 ? w.config.showMonths : -1, i = n > 0 ? 1 : -1, o = t - w.currentMonth; o != a; o += i)for (var r = w.daysContainer.children[o], l = t - w.currentMonth === o ? e.$i + n : n < 0 ? r.children.length - 1 : 0, c = r.children.length, s = l; s >= 0 && s < c && s != (n > 0 ? c : -1); s += i) { var d = r.children[s]; if (-1 === d.className.indexOf("hidden") && ne(d.dateObj) && Math.abs(e.$i - s) >= Math.abs(n)) return W(d) } w.changeMonth(i), J(B(i), 0) }(i, n) : W(i) } function K(e, n) { for (var t = (new Date(e, n, 1).getDay() - w.l10n.firstDayOfWeek + 7) % 7, a = w.utils.getDaysInMonth((n - 1 + 12) % 12, e), i = w.utils.getDaysInMonth(n, e), o = window.document.createDocumentFragment(), r = w.config.showMonths > 1, l = r ? "prevMonthDay hidden" : "prevMonthDay", c = r ? "nextMonthDay hidden" : "nextMonthDay", s = a + 1 - t, u = 0; s <= a; s++, u++)o.appendChild(R("flatpickr-day " + l, new Date(e, n - 1, s), 0, u)); for (s = 1; s <= i; s++, u++)o.appendChild(R("flatpickr-day", new Date(e, n, s), 0, u)); for (var f = i + 1; f <= 42 - t && (1 === w.config.showMonths || u % 7 != 0); f++, u++)o.appendChild(R("flatpickr-day " + c, new Date(e, n + 1, f % i), 0, u)); var m = d("div", "dayContainer"); return m.appendChild(o), m } function U() { if (void 0 !== w.daysContainer) { u(w.daysContainer), w.weekNumbers && u(w.weekNumbers); for (var e = document.createDocumentFragment(), n = 0; n < w.config.showMonths; n++) { var t = new Date(w.currentYear, w.currentMonth, 1); t.setMonth(w.currentMonth + n), e.appendChild(K(t.getFullYear(), t.getMonth())) } w.daysContainer.appendChild(e), w.days = w.daysContainer.firstChild, "range" === w.config.mode && 1 === w.selectedDates.length && oe() } } function q() { if (!(w.config.showMonths > 1 || "dropdown" !== w.config.monthSelectorType)) { var e = function (e) { return !(void 0 !== w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && e < w.config.minDate.getMonth()) && !(void 0 !== w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() && e > w.config.maxDate.getMonth()) }; w.monthsDropdownContainer.tabIndex = -1, w.monthsDropdownContainer.innerHTML = ""; for (var n = 0; n < 12; n++)if (e(n)) { var t = d("option", "flatpickr-monthDropdown-month"); t.value = new Date(w.currentYear, n).getMonth().toString(), t.textContent = h(n, w.config.shorthandCurrentMonth, w.l10n), t.tabIndex = -1, w.currentMonth === n && (t.selected = !0), w.monthsDropdownContainer.appendChild(t) } } } function $() { var e, n = d("div", "flatpickr-month"), t = window.document.createDocumentFragment(); w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? e = d("span", "cur-month") : (w.monthsDropdownContainer = d("select", "flatpickr-monthDropdown-months"), w.monthsDropdownContainer.setAttribute("aria-label", w.l10n.monthAriaLabel), P(w.monthsDropdownContainer, "change", (function (e) { var n = g(e), t = parseInt(n.value, 10); w.changeMonth(t - w.currentMonth), De("onMonthChange") })), q(), e = w.monthsDropdownContainer); var a = m("cur-year", { tabindex: "-1" }), i = a.getElementsByTagName("input")[0]; i.setAttribute("aria-label", w.l10n.yearAriaLabel), w.config.minDate && i.setAttribute("min", w.config.minDate.getFullYear().toString()), w.config.maxDate && (i.setAttribute("max", w.config.maxDate.getFullYear().toString()), i.disabled = !!w.config.minDate && w.config.minDate.getFullYear() === w.config.maxDate.getFullYear()); var o = d("div", "flatpickr-current-month"); return o.appendChild(e), o.appendChild(a), t.appendChild(o), n.appendChild(t), { container: n, yearElement: i, monthElement: e } } function V() { u(w.monthNav), w.monthNav.appendChild(w.prevMonthNav), w.config.showMonths && (w.yearElements = [], w.monthElements = []); for (var e = w.config.showMonths; e--;) { var n = $(); w.yearElements.push(n.yearElement), w.monthElements.push(n.monthElement), w.monthNav.appendChild(n.container) } w.monthNav.appendChild(w.nextMonthNav) } function z() { w.weekdayContainer ? u(w.weekdayContainer) : w.weekdayContainer = d("div", "flatpickr-weekdays"); for (var e = w.config.showMonths; e--;) { var n = d("div", "flatpickr-weekdaycontainer"); w.weekdayContainer.appendChild(n) } return G(), w.weekdayContainer } function G() { if (w.weekdayContainer) { var e = w.l10n.firstDayOfWeek, t = n(w.l10n.weekdays.shorthand); e > 0 && e < t.length && (t = n(t.splice(e, t.length), t.splice(0, e))); for (var a = w.config.showMonths; a--;)w.weekdayContainer.children[a].innerHTML = "\n      <span class='flatpickr-weekday'>\n        " + t.join("</span><span class='flatpickr-weekday'>") + "\n      </span>\n      " } } function Z(e, n) { void 0 === n && (n = !0); var t = n ? e : e - w.currentMonth; t < 0 && !0 === w._hidePrevMonthArrow || t > 0 && !0 === w._hideNextMonthArrow || (w.currentMonth += t, (w.currentMonth < 0 || w.currentMonth > 11) && (w.currentYear += w.currentMonth > 11 ? 1 : -1, w.currentMonth = (w.currentMonth + 12) % 12, De("onYearChange"), q()), U(), De("onMonthChange"), Ce()) } function Q(e) { return w.calendarContainer.contains(e) } function X(e) { if (w.isOpen && !w.config.inline) { var n = g(e), t = Q(n), a = !(n === w.input || n === w.altInput || w.element.contains(n) || e.path && e.path.indexOf && (~e.path.indexOf(w.input) || ~e.path.indexOf(w.altInput))) && !t && !Q(e.relatedTarget), i = !w.config.ignoredFocusElements.some((function (e) { return e.contains(n) })); a && i && (w.config.allowInput && w.setDate(w._input.value, !1, w.config.altInput ? w.config.altFormat : w.config.dateFormat), void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement && "" !== w.input.value && void 0 !== w.input.value && _(), w.close(), w.config && "range" === w.config.mode && 1 === w.selectedDates.length && w.clear(!1)) } } function ee(e) { if (!(!e || w.config.minDate && e < w.config.minDate.getFullYear() || w.config.maxDate && e > w.config.maxDate.getFullYear())) { var n = e, t = w.currentYear !== n; w.currentYear = n || w.currentYear, w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth = Math.min(w.config.maxDate.getMonth(), w.currentMonth) : w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && (w.currentMonth = Math.max(w.config.minDate.getMonth(), w.currentMonth)), t && (w.redraw(), De("onYearChange"), q()) } } function ne(e, n) { var t; void 0 === n && (n = !0); var a = w.parseDate(e, void 0, n); if (w.config.minDate && a && M(a, w.config.minDate, void 0 !== n ? n : !w.minDateHasTime) < 0 || w.config.maxDate && a && M(a, w.config.maxDate, void 0 !== n ? n : !w.maxDateHasTime) > 0) return !1; if (!w.config.enable && 0 === w.config.disable.length) return !0; if (void 0 === a) return !1; for (var i = !!w.config.enable, o = null !== (t = w.config.enable) && void 0 !== t ? t : w.config.disable, r = 0, l = void 0; r < o.length; r++) { if ("function" == typeof (l = o[r]) && l(a)) return i; if (l instanceof Date && void 0 !== a && l.getTime() === a.getTime()) return i; if ("string" == typeof l) { var c = w.parseDate(l, void 0, !0); return c && c.getTime() === a.getTime() ? i : !i } if ("object" == typeof l && void 0 !== a && l.from && l.to && a.getTime() >= l.from.getTime() && a.getTime() <= l.to.getTime()) return i } return !i } function te(e) { return void 0 !== w.daysContainer && (-1 === e.className.indexOf("hidden") && -1 === e.className.indexOf("flatpickr-disabled") && w.daysContainer.contains(e)) } function ae(e) { var n = e.target === w._input, t = w._input.value.trimEnd() !== Me(); !n || !t || e.relatedTarget && Q(e.relatedTarget) || w.setDate(w._input.value, !0, e.target === w.altInput ? w.config.altFormat : w.config.dateFormat) } function ie(e) { var n = g(e), t = w.config.wrap ? p.contains(n) : n === w._input, a = w.config.allowInput, i = w.isOpen && (!a || !t), o = w.config.inline && t && !a; if (13 === e.keyCode && t) { if (a) return w.setDate(w._input.value, !0, n === w.altInput ? w.config.altFormat : w.config.dateFormat), w.close(), n.blur(); w.open() } else if (Q(n) || i || o) { var r = !!w.timeContainer && w.timeContainer.contains(n); switch (e.keyCode) { case 13: r ? (e.preventDefault(), _(), fe()) : me(e); break; case 27: e.preventDefault(), fe(); break; case 8: case 46: t && !w.config.allowInput && (e.preventDefault(), w.clear()); break; case 37: case 39: if (r || t) w.hourElement && w.hourElement.focus(); else { e.preventDefault(); var l = k(); if (void 0 !== w.daysContainer && (!1 === a || l && te(l))) { var c = 39 === e.keyCode ? 1 : -1; e.ctrlKey ? (e.stopPropagation(), Z(c), J(B(1), 0)) : J(void 0, c) } } break; case 38: case 40: e.preventDefault(); var s = 40 === e.keyCode ? 1 : -1; w.daysContainer && void 0 !== n.$i || n === w.input || n === w.altInput ? e.ctrlKey ? (e.stopPropagation(), ee(w.currentYear - s), J(B(1), 0)) : r || J(void 0, 7 * s) : n === w.currentYearElement ? ee(w.currentYear - s) : w.config.enableTime && (!r && w.hourElement && w.hourElement.focus(), _(e), w._debouncedChange()); break; case 9: if (r) { var d = [w.hourElement, w.minuteElement, w.secondElement, w.amPM].concat(w.pluginElements).filter((function (e) { return e })), u = d.indexOf(n); if (-1 !== u) { var f = d[u + (e.shiftKey ? -1 : 1)]; e.preventDefault(), (f || w._input).focus() } } else !w.config.noCalendar && w.daysContainer && w.daysContainer.contains(n) && e.shiftKey && (e.preventDefault(), w._input.focus()) } } if (void 0 !== w.amPM && n === w.amPM) switch (e.key) { case w.l10n.amPM[0].charAt(0): case w.l10n.amPM[0].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[0], O(), ye(); break; case w.l10n.amPM[1].charAt(0): case w.l10n.amPM[1].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[1], O(), ye() }(t || Q(n)) && De("onKeyDown", e) } function oe(e, n) { if (void 0 === n && (n = "flatpickr-day"), 1 === w.selectedDates.length && (!e || e.classList.contains(n) && !e.classList.contains("flatpickr-disabled"))) { for (var t = e ? e.dateObj.getTime() : w.days.firstElementChild.dateObj.getTime(), a = w.parseDate(w.selectedDates[0], void 0, !0).getTime(), i = Math.min(t, w.selectedDates[0].getTime()), o = Math.max(t, w.selectedDates[0].getTime()), r = !1, l = 0, c = 0, s = i; s < o; s += x)ne(new Date(s), !0) || (r = r || s > i && s < o, s < a && (!l || s > l) ? l = s : s > a && (!c || s < c) && (c = s)); Array.from(w.rContainer.querySelectorAll("*:nth-child(-n+" + w.config.showMonths + ") > ." + n)).forEach((function (n) { var i, o, s, d = n.dateObj.getTime(), u = l > 0 && d < l || c > 0 && d > c; if (u) return n.classList.add("notAllowed"), void ["inRange", "startRange", "endRange"].forEach((function (e) { n.classList.remove(e) })); r && !u || (["startRange", "inRange", "endRange", "notAllowed"].forEach((function (e) { n.classList.remove(e) })), void 0 !== e && (e.classList.add(t <= w.selectedDates[0].getTime() ? "startRange" : "endRange"), a < t && d === a ? n.classList.add("startRange") : a > t && d === a && n.classList.add("endRange"), d >= l && (0 === c || d <= c) && (o = a, s = t, (i = d) > Math.min(o, s) && i < Math.max(o, s)) && n.classList.add("inRange"))) })) } } function re() { !w.isOpen || w.config.static || w.config.inline || de() } function le(e) { return function (n) { var t = w.config["_" + e + "Date"] = w.parseDate(n, w.config.dateFormat), a = w.config["_" + ("min" === e ? "max" : "min") + "Date"]; void 0 !== t && (w["min" === e ? "minDateHasTime" : "maxDateHasTime"] = t.getHours() > 0 || t.getMinutes() > 0 || t.getSeconds() > 0), w.selectedDates && (w.selectedDates = w.selectedDates.filter((function (e) { return ne(e) })), w.selectedDates.length || "min" !== e || F(t), ye()), w.daysContainer && (ue(), void 0 !== t ? w.currentYearElement[e] = t.getFullYear().toString() : w.currentYearElement.removeAttribute(e), w.currentYearElement.disabled = !!a && void 0 !== t && a.getFullYear() === t.getFullYear()) } } function ce() { return w.config.wrap ? p.querySelector("[data-input]") : p } function se() { "object" != typeof w.config.locale && void 0 === I.l10ns[w.config.locale] && w.config.errorHandler(new Error("flatpickr: invalid locale " + w.config.locale)), w.l10n = e(e({}, I.l10ns.default), "object" == typeof w.config.locale ? w.config.locale : "default" !== w.config.locale ? I.l10ns[w.config.locale] : void 0), D.D = "(" + w.l10n.weekdays.shorthand.join("|") + ")", D.l = "(" + w.l10n.weekdays.longhand.join("|") + ")", D.M = "(" + w.l10n.months.shorthand.join("|") + ")", D.F = "(" + w.l10n.months.longhand.join("|") + ")", D.K = "(" + w.l10n.amPM[0] + "|" + w.l10n.amPM[1] + "|" + w.l10n.amPM[0].toLowerCase() + "|" + w.l10n.amPM[1].toLowerCase() + ")", void 0 === e(e({}, v), JSON.parse(JSON.stringify(p.dataset || {}))).time_24hr && void 0 === I.defaultConfig.time_24hr && (w.config.time_24hr = w.l10n.time_24hr), w.formatDate = b(w), w.parseDate = C({ config: w.config, l10n: w.l10n }) } function de(e) { if ("function" != typeof w.config.position) { if (void 0 !== w.calendarContainer) { De("onPreCalendarPosition"); var n = e || w._positionElement, t = Array.prototype.reduce.call(w.calendarContainer.children, (function (e, n) { return e + n.offsetHeight }), 0), a = w.calendarContainer.offsetWidth, i = w.config.position.split(" "), o = i[0], r = i.length > 1 ? i[1] : null, l = n.getBoundingClientRect(), c = window.innerHeight - l.bottom, d = "above" === o || "below" !== o && c < t && l.top > t, u = window.pageYOffset + l.top + (d ? -t - 2 : n.offsetHeight + 2); if (s(w.calendarContainer, "arrowTop", !d), s(w.calendarContainer, "arrowBottom", d), !w.config.inline) { var f = window.pageXOffset + l.left, m = !1, g = !1; "center" === r ? (f -= (a - l.width) / 2, m = !0) : "right" === r && (f -= a - l.width, g = !0), s(w.calendarContainer, "arrowLeft", !m && !g), s(w.calendarContainer, "arrowCenter", m), s(w.calendarContainer, "arrowRight", g); var p = window.document.body.offsetWidth - (window.pageXOffset + l.right), h = f + a > window.document.body.offsetWidth, v = p + a > window.document.body.offsetWidth; if (s(w.calendarContainer, "rightMost", h), !w.config.static) if (w.calendarContainer.style.top = u + "px", h) if (v) { var D = function () { for (var e = null, n = 0; n < document.styleSheets.length; n++) { var t = document.styleSheets[n]; if (t.cssRules) { try { t.cssRules } catch (e) { continue } e = t; break } } return null != e ? e : (a = document.createElement("style"), document.head.appendChild(a), a.sheet); var a }(); if (void 0 === D) return; var b = window.document.body.offsetWidth, C = Math.max(0, b / 2 - a / 2), M = D.cssRules.length, y = "{left:" + l.left + "px;right:auto;}"; s(w.calendarContainer, "rightMost", !1), s(w.calendarContainer, "centerMost", !0), D.insertRule(".flatpickr-calendar.centerMost:before,.flatpickr-calendar.centerMost:after" + y, M), w.calendarContainer.style.left = C + "px", w.calendarContainer.style.right = "auto" } else w.calendarContainer.style.left = "auto", w.calendarContainer.style.right = p + "px"; else w.calendarContainer.style.left = f + "px", w.calendarContainer.style.right = "auto" } } } else w.config.position(w, e) } function ue() { w.config.noCalendar || w.isMobile || (q(), Ce(), U()) } function fe() { w._input.focus(), -1 !== window.navigator.userAgent.indexOf("MSIE") || void 0 !== navigator.msMaxTouchPoints ? setTimeout(w.close, 0) : w.close() } function me(e) { e.preventDefault(), e.stopPropagation(); var n = f(g(e), (function (e) { return e.classList && e.classList.contains("flatpickr-day") && !e.classList.contains("flatpickr-disabled") && !e.classList.contains("notAllowed") })); if (void 0 !== n) { var t = n, a = w.latestSelectedDateObj = new Date(t.dateObj.getTime()), i = (a.getMonth() < w.currentMonth || a.getMonth() > w.currentMonth + w.config.showMonths - 1) && "range" !== w.config.mode; if (w.selectedDateElem = t, "single" === w.config.mode) w.selectedDates = [a]; else if ("multiple" === w.config.mode) { var o = be(a); o ? w.selectedDates.splice(parseInt(o), 1) : w.selectedDates.push(a) } else "range" === w.config.mode && (2 === w.selectedDates.length && w.clear(!1, !1), w.latestSelectedDateObj = a, w.selectedDates.push(a), 0 !== M(a, w.selectedDates[0], !0) && w.selectedDates.sort((function (e, n) { return e.getTime() - n.getTime() }))); if (O(), i) { var r = w.currentYear !== a.getFullYear(); w.currentYear = a.getFullYear(), w.currentMonth = a.getMonth(), r && (De("onYearChange"), q()), De("onMonthChange") } if (Ce(), U(), ye(), i || "range" === w.config.mode || 1 !== w.config.showMonths ? void 0 !== w.selectedDateElem && void 0 === w.hourElement && w.selectedDateElem && w.selectedDateElem.focus() : W(t), void 0 !== w.hourElement && void 0 !== w.hourElement && w.hourElement.focus(), w.config.closeOnSelect) { var l = "single" === w.config.mode && !w.config.enableTime, c = "range" === w.config.mode && 2 === w.selectedDates.length && !w.config.enableTime; (l || c) && fe() } Y() } } w.parseDate = C({ config: w.config, l10n: w.l10n }), w._handlers = [], w.pluginElements = [], w.loadedPlugins = [], w._bind = P, w._setHoursFromDate = F, w._positionCalendar = de, w.changeMonth = Z, w.changeYear = ee, w.clear = function (e, n) { void 0 === e && (e = !0); void 0 === n && (n = !0); w.input.value = "", void 0 !== w.altInput && (w.altInput.value = ""); void 0 !== w.mobileInput && (w.mobileInput.value = ""); w.selectedDates = [], w.latestSelectedDateObj = void 0, !0 === n && (w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth()); if (!0 === w.config.enableTime) { var t = E(w.config), a = t.hours, i = t.minutes, o = t.seconds; A(a, i, o) } w.redraw(), e && De("onChange") }, w.close = function () { w.isOpen = !1, w.isMobile || (void 0 !== w.calendarContainer && w.calendarContainer.classList.remove("open"), void 0 !== w._input && w._input.classList.remove("active")); De("onClose") }, w.onMouseOver = oe, w._createElement = d, w.createDay = R, w.destroy = function () { void 0 !== w.config && De("onDestroy"); for (var e = w._handlers.length; e--;)w._handlers[e].remove(); if (w._handlers = [], w.mobileInput) w.mobileInput.parentNode && w.mobileInput.parentNode.removeChild(w.mobileInput), w.mobileInput = void 0; else if (w.calendarContainer && w.calendarContainer.parentNode) if (w.config.static && w.calendarContainer.parentNode) { var n = w.calendarContainer.parentNode; if (n.lastChild && n.removeChild(n.lastChild), n.parentNode) { for (; n.firstChild;)n.parentNode.insertBefore(n.firstChild, n); n.parentNode.removeChild(n) } } else w.calendarContainer.parentNode.removeChild(w.calendarContainer); w.altInput && (w.input.type = "text", w.altInput.parentNode && w.altInput.parentNode.removeChild(w.altInput), delete w.altInput); w.input && (w.input.type = w.input._type, w.input.classList.remove("flatpickr-input"), w.input.removeAttribute("readonly"));["_showTimeInput", "latestSelectedDateObj", "_hideNextMonthArrow", "_hidePrevMonthArrow", "__hideNextMonthArrow", "__hidePrevMonthArrow", "isMobile", "isOpen", "selectedDateElem", "minDateHasTime", "maxDateHasTime", "days", "daysContainer", "_input", "_positionElement", "innerContainer", "rContainer", "monthNav", "todayDateElem", "calendarContainer", "weekdayContainer", "prevMonthNav", "nextMonthNav", "monthsDropdownContainer", "currentMonthElement", "currentYearElement", "navigationCurrentMonth", "selectedDateElem", "config"].forEach((function (e) { try { delete w[e] } catch (e) { } })) }, w.isEnabled = ne, w.jumpToDate = j, w.updateValue = ye, w.open = function (e, n) { void 0 === n && (n = w._positionElement); if (!0 === w.isMobile) { if (e) { e.preventDefault(); var t = g(e); t && t.blur() } return void 0 !== w.mobileInput && (w.mobileInput.focus(), w.mobileInput.click()), void De("onOpen") } if (w._input.disabled || w.config.inline) return; var a = w.isOpen; w.isOpen = !0, a || (w.calendarContainer.classList.add("open"), w._input.classList.add("active"), De("onOpen"), de(n)); !0 === w.config.enableTime && !0 === w.config.noCalendar && (!1 !== w.config.allowInput || void 0 !== e && w.timeContainer.contains(e.relatedTarget) || setTimeout((function () { return w.hourElement.select() }), 50)) }, w.redraw = ue, w.set = function (e, n) { if (null !== e && "object" == typeof e) for (var a in Object.assign(w.config, e), e) void 0 !== ge[a] && ge[a].forEach((function (e) { return e() })); else w.config[e] = n, void 0 !== ge[e] ? ge[e].forEach((function (e) { return e() })) : t.indexOf(e) > -1 && (w.config[e] = c(n)); w.redraw(), ye(!0) }, w.setDate = function (e, n, t) { void 0 === n && (n = !1); void 0 === t && (t = w.config.dateFormat); if (0 !== e && !e || e instanceof Array && 0 === e.length) return w.clear(n); pe(e, t), w.latestSelectedDateObj = w.selectedDates[w.selectedDates.length - 1], w.redraw(), j(void 0, n), F(), 0 === w.selectedDates.length && w.clear(!1); ye(n), n && De("onChange") }, w.toggle = function (e) { if (!0 === w.isOpen) return w.close(); w.open(e) }; var ge = { locale: [se, G], showMonths: [V, S, z], minDate: [j], maxDate: [j], positionElement: [ve], clickOpens: [function () { !0 === w.config.clickOpens ? (P(w._input, "focus", w.open), P(w._input, "click", w.open)) : (w._input.removeEventListener("focus", w.open), w._input.removeEventListener("click", w.open)) }] }; function pe(e, n) { var t = []; if (e instanceof Array) t = e.map((function (e) { return w.parseDate(e, n) })); else if (e instanceof Date || "number" == typeof e) t = [w.parseDate(e, n)]; else if ("string" == typeof e) switch (w.config.mode) { case "single": case "time": t = [w.parseDate(e, n)]; break; case "multiple": t = e.split(w.config.conjunction).map((function (e) { return w.parseDate(e, n) })); break; case "range": t = e.split(w.l10n.rangeSeparator).map((function (e) { return w.parseDate(e, n) })) } else w.config.errorHandler(new Error("Invalid date supplied: " + JSON.stringify(e))); w.selectedDates = w.config.allowInvalidPreload ? t : t.filter((function (e) { return e instanceof Date && ne(e, !1) })), "range" === w.config.mode && w.selectedDates.sort((function (e, n) { return e.getTime() - n.getTime() })) } function he(e) { return e.slice().map((function (e) { return "string" == typeof e || "number" == typeof e || e instanceof Date ? w.parseDate(e, void 0, !0) : e && "object" == typeof e && e.from && e.to ? { from: w.parseDate(e.from, void 0), to: w.parseDate(e.to, void 0) } : e })).filter((function (e) { return e })) } function ve() { w._positionElement = w.config.positionElement || w._input } function De(e, n) { if (void 0 !== w.config) { var t = w.config[e]; if (void 0 !== t && t.length > 0) for (var a = 0; t[a] && a < t.length; a++)t[a](w.selectedDates, w.input.value, w, n); "onChange" === e && (w.input.dispatchEvent(we("change")), w.input.dispatchEvent(we("input"))) } } function we(e) { var n = document.createEvent("Event"); return n.initEvent(e, !0, !0), n } function be(e) { for (var n = 0; n < w.selectedDates.length; n++) { var t = w.selectedDates[n]; if (t instanceof Date && 0 === M(t, e)) return "" + n } return !1 } function Ce() { w.config.noCalendar || w.isMobile || !w.monthNav || (w.yearElements.forEach((function (e, n) { var t = new Date(w.currentYear, w.currentMonth, 1); t.setMonth(w.currentMonth + n), w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? w.monthElements[n].textContent = h(t.getMonth(), w.config.shorthandCurrentMonth, w.l10n) + " " : w.monthsDropdownContainer.value = t.getMonth().toString(), e.value = t.getFullYear().toString() })), w._hidePrevMonthArrow = void 0 !== w.config.minDate && (w.currentYear === w.config.minDate.getFullYear() ? w.currentMonth <= w.config.minDate.getMonth() : w.currentYear < w.config.minDate.getFullYear()), w._hideNextMonthArrow = void 0 !== w.config.maxDate && (w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth + 1 > w.config.maxDate.getMonth() : w.currentYear > w.config.maxDate.getFullYear())) } function Me(e) { var n = e || (w.config.altInput ? w.config.altFormat : w.config.dateFormat); return w.selectedDates.map((function (e) { return w.formatDate(e, n) })).filter((function (e, n, t) { return "range" !== w.config.mode || w.config.enableTime || t.indexOf(e) === n })).join("range" !== w.config.mode ? w.config.conjunction : w.l10n.rangeSeparator) } function ye(e) { void 0 === e && (e = !0), void 0 !== w.mobileInput && w.mobileFormatStr && (w.mobileInput.value = void 0 !== w.latestSelectedDateObj ? w.formatDate(w.latestSelectedDateObj, w.mobileFormatStr) : ""), w.input.value = Me(w.config.dateFormat), void 0 !== w.altInput && (w.altInput.value = Me(w.config.altFormat)), !1 !== e && De("onValueUpdate") } function xe(e) { var n = g(e), t = w.prevMonthNav.contains(n), a = w.nextMonthNav.contains(n); t || a ? Z(t ? -1 : 1) : w.yearElements.indexOf(n) >= 0 ? n.select() : n.classList.contains("arrowUp") ? w.changeYear(w.currentYear + 1) : n.classList.contains("arrowDown") && w.changeYear(w.currentYear - 1) } return function () { w.element = w.input = p, w.isOpen = !1, function () { var n = ["wrap", "weekNumbers", "allowInput", "allowInvalidPreload", "clickOpens", "time_24hr", "enableTime", "noCalendar", "altInput", "shorthandCurrentMonth", "inline", "static", "enableSeconds", "disableMobile"], i = e(e({}, JSON.parse(JSON.stringify(p.dataset || {}))), v), o = {}; w.config.parseDate = i.parseDate, w.config.formatDate = i.formatDate, Object.defineProperty(w.config, "enable", { get: function () { return w.config._enable }, set: function (e) { w.config._enable = he(e) } }), Object.defineProperty(w.config, "disable", { get: function () { return w.config._disable }, set: function (e) { w.config._disable = he(e) } }); var r = "time" === i.mode; if (!i.dateFormat && (i.enableTime || r)) { var l = I.defaultConfig.dateFormat || a.dateFormat; o.dateFormat = i.noCalendar || r ? "H:i" + (i.enableSeconds ? ":S" : "") : l + " H:i" + (i.enableSeconds ? ":S" : "") } if (i.altInput && (i.enableTime || r) && !i.altFormat) { var s = I.defaultConfig.altFormat || a.altFormat; o.altFormat = i.noCalendar || r ? "h:i" + (i.enableSeconds ? ":S K" : " K") : s + " h:i" + (i.enableSeconds ? ":S" : "") + " K" } Object.defineProperty(w.config, "minDate", { get: function () { return w.config._minDate }, set: le("min") }), Object.defineProperty(w.config, "maxDate", { get: function () { return w.config._maxDate }, set: le("max") }); var d = function (e) { return function (n) { w.config["min" === e ? "_minTime" : "_maxTime"] = w.parseDate(n, "H:i:S") } }; Object.defineProperty(w.config, "minTime", { get: function () { return w.config._minTime }, set: d("min") }), Object.defineProperty(w.config, "maxTime", { get: function () { return w.config._maxTime }, set: d("max") }), "time" === i.mode && (w.config.noCalendar = !0, w.config.enableTime = !0); Object.assign(w.config, o, i); for (var u = 0; u < n.length; u++)w.config[n[u]] = !0 === w.config[n[u]] || "true" === w.config[n[u]]; t.filter((function (e) { return void 0 !== w.config[e] })).forEach((function (e) { w.config[e] = c(w.config[e] || []).map(T) })), w.isMobile = !w.config.disableMobile && !w.config.inline && "single" === w.config.mode && !w.config.disable.length && !w.config.enable && !w.config.weekNumbers && /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent); for (u = 0; u < w.config.plugins.length; u++) { var f = w.config.plugins[u](w) || {}; for (var m in f) t.indexOf(m) > -1 ? w.config[m] = c(f[m]).map(T).concat(w.config[m]) : void 0 === i[m] && (w.config[m] = f[m]) } i.altInputClass || (w.config.altInputClass = ce().className + " " + w.config.altInputClass); De("onParseConfig") }(), se(), function () { if (w.input = ce(), !w.input) return void w.config.errorHandler(new Error("Invalid input element specified")); w.input._type = w.input.type, w.input.type = "text", w.input.classList.add("flatpickr-input"), w._input = w.input, w.config.altInput && (w.altInput = d(w.input.nodeName, w.config.altInputClass), w._input = w.altInput, w.altInput.placeholder = w.input.placeholder, w.altInput.disabled = w.input.disabled, w.altInput.required = w.input.required, w.altInput.tabIndex = w.input.tabIndex, w.altInput.type = "text", w.input.setAttribute("type", "hidden"), !w.config.static && w.input.parentNode && w.input.parentNode.insertBefore(w.altInput, w.input.nextSibling)); w.config.allowInput || w._input.setAttribute("readonly", "readonly"); ve() }(), function () { w.selectedDates = [], w.now = w.parseDate(w.config.now) || new Date; var e = w.config.defaultDate || ("INPUT" !== w.input.nodeName && "TEXTAREA" !== w.input.nodeName || !w.input.placeholder || w.input.value !== w.input.placeholder ? w.input.value : null); e && pe(e, w.config.dateFormat); w._initialDate = w.selectedDates.length > 0 ? w.selectedDates[0] : w.config.minDate && w.config.minDate.getTime() > w.now.getTime() ? w.config.minDate : w.config.maxDate && w.config.maxDate.getTime() < w.now.getTime() ? w.config.maxDate : w.now, w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth(), w.selectedDates.length > 0 && (w.latestSelectedDateObj = w.selectedDates[0]); void 0 !== w.config.minTime && (w.config.minTime = w.parseDate(w.config.minTime, "H:i")); void 0 !== w.config.maxTime && (w.config.maxTime = w.parseDate(w.config.maxTime, "H:i")); w.minDateHasTime = !!w.config.minDate && (w.config.minDate.getHours() > 0 || w.config.minDate.getMinutes() > 0 || w.config.minDate.getSeconds() > 0), w.maxDateHasTime = !!w.config.maxDate && (w.config.maxDate.getHours() > 0 || w.config.maxDate.getMinutes() > 0 || w.config.maxDate.getSeconds() > 0) }(), w.utils = { getDaysInMonth: function (e, n) { return void 0 === e && (e = w.currentMonth), void 0 === n && (n = w.currentYear), 1 === e && (n % 4 == 0 && n % 100 != 0 || n % 400 == 0) ? 29 : w.l10n.daysInMonth[e] } }, w.isMobile || function () { var e = window.document.createDocumentFragment(); if (w.calendarContainer = d("div", "flatpickr-calendar"), w.calendarContainer.tabIndex = -1, !w.config.noCalendar) { if (e.appendChild((w.monthNav = d("div", "flatpickr-months"), w.yearElements = [], w.monthElements = [], w.prevMonthNav = d("span", "flatpickr-prev-month"), w.prevMonthNav.innerHTML = w.config.prevArrow, w.nextMonthNav = d("span", "flatpickr-next-month"), w.nextMonthNav.innerHTML = w.config.nextArrow, V(), Object.defineProperty(w, "_hidePrevMonthArrow", { get: function () { return w.__hidePrevMonthArrow }, set: function (e) { w.__hidePrevMonthArrow !== e && (s(w.prevMonthNav, "flatpickr-disabled", e), w.__hidePrevMonthArrow = e) } }), Object.defineProperty(w, "_hideNextMonthArrow", { get: function () { return w.__hideNextMonthArrow }, set: function (e) { w.__hideNextMonthArrow !== e && (s(w.nextMonthNav, "flatpickr-disabled", e), w.__hideNextMonthArrow = e) } }), w.currentYearElement = w.yearElements[0], Ce(), w.monthNav)), w.innerContainer = d("div", "flatpickr-innerContainer"), w.config.weekNumbers) { var n = function () { w.calendarContainer.classList.add("hasWeeks"); var e = d("div", "flatpickr-weekwrapper"); e.appendChild(d("span", "flatpickr-weekday", w.l10n.weekAbbreviation)); var n = d("div", "flatpickr-weeks"); return e.appendChild(n), { weekWrapper: e, weekNumbers: n } }(), t = n.weekWrapper, a = n.weekNumbers; w.innerContainer.appendChild(t), w.weekNumbers = a, w.weekWrapper = t } w.rContainer = d("div", "flatpickr-rContainer"), w.rContainer.appendChild(z()), w.daysContainer || (w.daysContainer = d("div", "flatpickr-days"), w.daysContainer.tabIndex = -1), U(), w.rContainer.appendChild(w.daysContainer), w.innerContainer.appendChild(w.rContainer), e.appendChild(w.innerContainer) } w.config.enableTime && e.appendChild(function () { w.calendarContainer.classList.add("hasTime"), w.config.noCalendar && w.calendarContainer.classList.add("noCalendar"); var e = E(w.config); w.timeContainer = d("div", "flatpickr-time"), w.timeContainer.tabIndex = -1; var n = d("span", "flatpickr-time-separator", ":"), t = m("flatpickr-hour", { "aria-label": w.l10n.hourAriaLabel }); w.hourElement = t.getElementsByTagName("input")[0]; var a = m("flatpickr-minute", { "aria-label": w.l10n.minuteAriaLabel }); w.minuteElement = a.getElementsByTagName("input")[0], w.hourElement.tabIndex = w.minuteElement.tabIndex = -1, w.hourElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getHours() : w.config.time_24hr ? e.hours : function (e) { switch (e % 24) { case 0: case 12: return 12; default: return e % 12 } }(e.hours)), w.minuteElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getMinutes() : e.minutes), w.hourElement.setAttribute("step", w.config.hourIncrement.toString()), w.minuteElement.setAttribute("step", w.config.minuteIncrement.toString()), w.hourElement.setAttribute("min", w.config.time_24hr ? "0" : "1"), w.hourElement.setAttribute("max", w.config.time_24hr ? "23" : "12"), w.hourElement.setAttribute("maxlength", "2"), w.minuteElement.setAttribute("min", "0"), w.minuteElement.setAttribute("max", "59"), w.minuteElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(t), w.timeContainer.appendChild(n), w.timeContainer.appendChild(a), w.config.time_24hr && w.timeContainer.classList.add("time24hr"); if (w.config.enableSeconds) { w.timeContainer.classList.add("hasSeconds"); var i = m("flatpickr-second"); w.secondElement = i.getElementsByTagName("input")[0], w.secondElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getSeconds() : e.seconds), w.secondElement.setAttribute("step", w.minuteElement.getAttribute("step")), w.secondElement.setAttribute("min", "0"), w.secondElement.setAttribute("max", "59"), w.secondElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(d("span", "flatpickr-time-separator", ":")), w.timeContainer.appendChild(i) } w.config.time_24hr || (w.amPM = d("span", "flatpickr-am-pm", w.l10n.amPM[r((w.latestSelectedDateObj ? w.hourElement.value : w.config.defaultHour) > 11)]), w.amPM.title = w.l10n.toggleTitle, w.amPM.tabIndex = -1, w.timeContainer.appendChild(w.amPM)); return w.timeContainer }()); s(w.calendarContainer, "rangeMode", "range" === w.config.mode), s(w.calendarContainer, "animate", !0 === w.config.animate), s(w.calendarContainer, "multiMonth", w.config.showMonths > 1), w.calendarContainer.appendChild(e); var i = void 0 !== w.config.appendTo && void 0 !== w.config.appendTo.nodeType; if ((w.config.inline || w.config.static) && (w.calendarContainer.classList.add(w.config.inline ? "inline" : "static"), w.config.inline && (!i && w.element.parentNode ? w.element.parentNode.insertBefore(w.calendarContainer, w._input.nextSibling) : void 0 !== w.config.appendTo && w.config.appendTo.appendChild(w.calendarContainer)), w.config.static)) { var l = d("div", "flatpickr-wrapper"); w.element.parentNode && w.element.parentNode.insertBefore(l, w.element), l.appendChild(w.element), w.altInput && l.appendChild(w.altInput), l.appendChild(w.calendarContainer) } w.config.static || w.config.inline || (void 0 !== w.config.appendTo ? w.config.appendTo : window.document.body).appendChild(w.calendarContainer) }(), function () { w.config.wrap && ["open", "close", "toggle", "clear"].forEach((function (e) { Array.prototype.forEach.call(w.element.querySelectorAll("[data-" + e + "]"), (function (n) { return P(n, "click", w[e]) })) })); if (w.isMobile) return void function () { var e = w.config.enableTime ? w.config.noCalendar ? "time" : "datetime-local" : "date"; w.mobileInput = d("input", w.input.className + " flatpickr-mobile"), w.mobileInput.tabIndex = 1, w.mobileInput.type = e, w.mobileInput.disabled = w.input.disabled, w.mobileInput.required = w.input.required, w.mobileInput.placeholder = w.input.placeholder, w.mobileFormatStr = "datetime-local" === e ? "Y-m-d\\TH:i:S" : "date" === e ? "Y-m-d" : "H:i:S", w.selectedDates.length > 0 && (w.mobileInput.defaultValue = w.mobileInput.value = w.formatDate(w.selectedDates[0], w.mobileFormatStr)); w.config.minDate && (w.mobileInput.min = w.formatDate(w.config.minDate, "Y-m-d")); w.config.maxDate && (w.mobileInput.max = w.formatDate(w.config.maxDate, "Y-m-d")); w.input.getAttribute("step") && (w.mobileInput.step = String(w.input.getAttribute("step"))); w.input.type = "hidden", void 0 !== w.altInput && (w.altInput.type = "hidden"); try { w.input.parentNode && w.input.parentNode.insertBefore(w.mobileInput, w.input.nextSibling) } catch (e) { } P(w.mobileInput, "change", (function (e) { w.setDate(g(e).value, !1, w.mobileFormatStr), De("onChange"), De("onClose") })) }(); var e = l(re, 50); w._debouncedChange = l(Y, 300), w.daysContainer && !/iPhone|iPad|iPod/i.test(navigator.userAgent) && P(w.daysContainer, "mouseover", (function (e) { "range" === w.config.mode && oe(g(e)) })); P(w._input, "keydown", ie), void 0 !== w.calendarContainer && P(w.calendarContainer, "keydown", ie); w.config.inline || w.config.static || P(window, "resize", e); void 0 !== window.ontouchstart ? P(window.document, "touchstart", X) : P(window.document, "mousedown", X); P(window.document, "focus", X, { capture: !0 }), !0 === w.config.clickOpens && (P(w._input, "focus", w.open), P(w._input, "click", w.open)); void 0 !== w.daysContainer && (P(w.monthNav, "click", xe), P(w.monthNav, ["keyup", "increment"], N), P(w.daysContainer, "click", me)); if (void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement) { var n = function (e) { return g(e).select() }; P(w.timeContainer, ["increment"], _), P(w.timeContainer, "blur", _, { capture: !0 }), P(w.timeContainer, "click", H), P([w.hourElement, w.minuteElement], ["focus", "click"], n), void 0 !== w.secondElement && P(w.secondElement, "focus", (function () { return w.secondElement && w.secondElement.select() })), void 0 !== w.amPM && P(w.amPM, "click", (function (e) { _(e) })) } w.config.allowInput && P(w._input, "blur", ae) }(), (w.selectedDates.length || w.config.noCalendar) && (w.config.enableTime && F(w.config.noCalendar ? w.latestSelectedDateObj : void 0), ye(!1)), S(); var n = /^((?!chrome|android).)*safari/i.test(navigator.userAgent); !w.isMobile && n && de(), De("onReady") }(), w } function T(e, n) { for (var t = Array.prototype.slice.call(e).filter((function (e) { return e instanceof HTMLElement })), a = [], i = 0; i < t.length; i++) { var o = t[i]; try { if (null !== o.getAttribute("data-fp-omit")) continue; void 0 !== o._flatpickr && (o._flatpickr.destroy(), o._flatpickr = void 0), o._flatpickr = k(o, n || {}), a.push(o._flatpickr) } catch (e) { console.error(e) } } return 1 === a.length ? a[0] : a } "undefined" != typeof HTMLElement && "undefined" != typeof HTMLCollection && "undefined" != typeof NodeList && (HTMLCollection.prototype.flatpickr = NodeList.prototype.flatpickr = function (e) { return T(this, e) }, HTMLElement.prototype.flatpickr = function (e) { return T([this], e) }); var I = function (e, n) { return "string" == typeof e ? T(window.document.querySelectorAll(e), n) : e instanceof Node ? T([e], n) : T(e, n) }; return I.defaultConfig = {}, I.l10ns = { en: e({}, i), default: e({}, i) }, I.localize = function (n) { I.l10ns.default = e(e({}, I.l10ns.default), n) }, I.setDefaults = function (n) { I.defaultConfig = e(e({}, I.defaultConfig), n) }, I.parseDate = C({}), I.formatDate = b({}), I.compareDates = M, "undefined" != typeof jQuery && void 0 !== jQuery.fn && (jQuery.fn.flatpickr = function (e) { return T(this, e) }), Date.prototype.fp_incr = function (e) { return new Date(this.getFullYear(), this.getMonth(), this.getDate() + ("string" == typeof e ? parseInt(e, 10) : e)) }, "undefined" != typeof window && (window.flatpickr = I), I }));

/* --- FLATPICKR VIETNAMESE LOCALIZATION --- */
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
        typeof define === 'function' && define.amd ? define(['exports'], factory) :
            (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.vn = {}));
}(this, (function (exports) {
    'use strict';

    var fp = typeof window !== "undefined" && window.flatpickr !== undefined
        ? window.flatpickr
        : {
            l10ns: {},
        };
    var Vietnamese = {
        weekdays: {
            shorthand: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
            longhand: [
                "Chủ nhật",
                "Thứ hai",
                "Thứ ba",
                "Thứ tư",
                "Thứ năm",
                "Thứ sáu",
                "Thứ bảy",
            ],
        },
        months: {
            shorthand: [
                "Th1",
                "Th2",
                "Th3",
                "Th4",
                "Th5",
                "Th6",
                "Th7",
                "Th8",
                "Th9",
                "Th10",
                "Th11",
                "Th12",
            ],
            longhand: [
                "Tháng một",
                "Tháng hai",
                "Tháng ba",
                "Tháng tư",
                "Tháng năm",
                "Tháng sáu",
                "Tháng bảy",
                "Tháng tám",
                "Tháng chín",
                "Tháng mười",
                "Tháng mười một",
                "Tháng mười hai",
            ],
        },
        firstDayOfWeek: 1,
        rangeSeparator: " đến ",
    };
    fp.l10ns.vn = Vietnamese;
    var vn = fp.l10ns;

    exports.Vietnamese = Vietnamese;
    exports.default = vn;

    Object.defineProperty(exports, '__esModule', { value: true });

})));


(function () {
    "use strict";

    var DK_VERSION = "2.6.16";
    try {
        console.log("%c[Dashboard Kho Desktop] v" + DK_VERSION + " loaded",
            "background:#2563eb;color:#fff;padding:4px 10px;border-radius:4px;font-weight:700;font-size:13px");
    } catch (e) { }
    window.__DK_VERSION__ = DK_VERSION;

    var state = {
        overall: [],
        customers: [],
        racks: [],
        inbound: [],
        outboundReady: [],
        outboundRunning: [],
        top15MaxNL: [],
        top15MaxPL: [],
        nkDuKien: [],
        calActFilter: { in: true, out: true, kk: true, plan: true },  // v2.3.33
        thanhGia: null,
        ageStock: [],
        distinctMat: [],
        activityCalendar: [],
        momComparison: [],
        flowTrend12T: [],
        flowTrendRangeRaw: [],
        lastUpdated: null,
        lpcpCalendar: {},
        lpcpStats: null,
        loading: false
    };
    var activeCustomerFilter = "";
    // v2.4.6 — Filter ngày global cho dashboard
    state.dateFilter = (function () {
        var to = new Date();
        var from = new Date(); from.setDate(to.getDate() - 30);
        return {
            from: from.toISOString().slice(0, 10),
            to: to.toISOString().slice(0, 10)
        };
    })();
    state.kpiTongNhap = null;
    state.kpiTongXuat = null;
    state.kpiTonKho = null;
    state.kpiTonDauKy = null;
    state.kpiPODangTre = null;
    state.kpiGiaTriTon = null;
    state.alerts = [];
    state.hieuSuat = [];
    var activityWeeksCount = 13;
    var isDemoMode = getQueryParam("demo") === "1";
    var calMonthDate = null;
    var calRangeFrom = null;
    var calRangeTo = null;

    var ids = {
        currentDate: "currentDate",
        currentTime: "currentTime",
        chartCapacityRing: "chartCapacityRing",
        chartCapacityLegend: "chartCapacityLegend",
        chartCustomerPie: "chartCustomerPie",
        chartFlowTrend: "chartFlowTrend",
        chartTop5MaxNL: "chartTop5MaxNL",
        chartTop5MaxPL: "chartTop5MaxPL",
        chartAgeStock: "chartAgeStock",
        customersBody: "customersBody",
        racksBody: "racksBody",
        detailModal: "detailModal",
        detailModalTitle: "detailModalTitle",
        detailModalMeta: "detailModalMeta",
        detailModalContent: "detailModalContent"
    };

    function byId(id) {
        return document.getElementById(id);
    }

    function toNumber(value) {
        if (value === null || value === undefined || value === "") return 0;
        var number = Number(value);
        return isNaN(number) ? 0 : number;
    }

    function formatNumber(value, fractionDigits) {
        var digits = typeof fractionDigits === "number" ? fractionDigits : 0;
        return toNumber(value).toLocaleString("vi-VN", {
            minimumFractionDigits: digits,
            maximumFractionDigits: digits
        });
    }

    function formatPercent(value) {
        return formatNumber(value, 2) + "%";
    }

    function parseDate(raw) {
        if (!raw) return null;
        if (Object.prototype.toString.call(raw) === "[object Date]") return raw;

        if (typeof raw === "string") {
            var match = /\/Date\((\d+)\)\//.exec(raw);
            if (match && match[1]) {
                return new Date(parseInt(match[1], 10));
            }

            var parsed = new Date(raw);
            if (!isNaN(parsed.getTime())) return parsed;
        }

        return null;
    }

    function formatDateShort(d) {
        if (!d) return "";
        if (!(d instanceof Date)) d = parseDate(d);
        if (!d) return "";
        return String(d.getDate()).padStart(2, "0") + "/" +
            String(d.getMonth() + 1).padStart(2, "0") + "/" +
            d.getFullYear();
    }

    function formatDate(raw) {
        var date = parseDate(raw);
        if (!date) return "";
        return date.toLocaleDateString("vi-VN");
    }

    function formatDateTime(date) {
        if (!date) return "--:--:--";
        return date.toLocaleTimeString("vi-VN", { hour12: false });
    }

    function renderClockNow() {
        var now = new Date();
        var timeNode = byId(ids.currentTime);
        var dateNode = byId(ids.currentDate);
        if (timeNode) timeNode.textContent = now.toLocaleTimeString("vi-VN", { hour12: false });
        if (dateNode) dateNode.textContent = now.toLocaleDateString("vi-VN", { weekday: "long", year: "numeric", month: "2-digit", day: "2-digit" });
        // v2.5.0 — Also update topbar clock
        var tbTime = byId("topbarTime");
        var tbDate = byId("topbarDate");
        if (tbTime) tbTime.textContent = now.toLocaleTimeString("vi-VN", { hour12: false });
        if (tbDate) tbDate.textContent = now.toLocaleDateString("vi-VN", { weekday: "short", day: "2-digit", month: "2-digit", year: "numeric" });
    }

    function formatCompactDate(raw) {
        var date = parseDate(raw);
        if (!date) return "";
        var day = String(date.getDate()).padStart(2, "0");
        var month = String(date.getMonth() + 1).padStart(2, "0");
        return day + "/" + month;
    }

    function escapeHtml(value) {
        var text = String(value === null || value === undefined ? "" : value);
        return text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function shortMaLenh(value) {
        if (!value) return "";
        var str = String(value).trim();
        var idx = str.indexOf("|");
        return idx >= 0 ? str.substring(0, idx).trim() : str;
    }

    function normalizeCustomerName(value) {
        var name = value === null || value === undefined ? "" : String(value).trim();
        return name ? name : "Khách trống";
    }

    function normalizeArray(data) {
        if (!data) return [];
        if (Array.isArray(data)) return data;
        if (data.d && Array.isArray(data.d)) return data.d;
        if (data.Data && Array.isArray(data.Data)) return data.Data;
        return [];
    }

    function requestJson(url) {
        // Luôn luôn băm cache (cache bust) để ngăn chặn trình duyệt cache API GET
        var cacheBustUrl = url + (url.indexOf("?") !== -1 ? "&" : "?") + "_t=" + new Date().getTime();

        if (window.fetch) {
            return window.fetch(cacheBustUrl, {
                method: "GET",
                cache: "no-store",
                headers: { "Accept": "application/json" }
            }).then(function (response) {
                if (!response.ok) {
                    return response.text().then(function (body) {
                        var errMsg = "HTTP " + response.status + " - " + response.statusText;
                        if (body) {
                            try {
                                var j = JSON.parse(body);
                                if (j && j.Message) errMsg += "\n" + j.Message;
                                else errMsg += "\n" + body.substring(0, 500);
                            } catch (e) {
                                errMsg += "\n" + body.substring(0, 500);
                            }
                        }
                        throw new Error(errMsg);
                    });
                }
                return response.json();
            });
        }

        return new Promise(function (resolve, reject) {
            var xhr = new XMLHttpRequest();
            xhr.open("GET", cacheBustUrl, true);
            xhr.setRequestHeader("Accept", "application/json");
            xhr.onreadystatechange = function () {
                if (xhr.readyState !== 4) return;
                if (xhr.status < 200 || xhr.status >= 300) {
                    var em = "HTTP " + xhr.status;
                    if (xhr.responseText) {
                        try {
                            var jj = JSON.parse(xhr.responseText);
                            if (jj && jj.Message) em += "\n" + jj.Message;
                        } catch (e) { em += "\n" + xhr.responseText.substring(0, 500); }
                    }
                    reject(new Error(em));
                    return;
                }
                try {
                    resolve(JSON.parse(xhr.responseText));
                } catch (error) {
                    reject(error);
                }
            };
            xhr.send();
        });
    }

    function buildDateRange() {
        var now = new Date();
        // Lấy 12 tháng cho biểu đồ xuất nhập tồn
        var from = new Date(now.getFullYear(), now.getMonth() - 11, 1);
        var to = new Date(now.getFullYear(), now.getMonth() + 1, 0);
        return {
            from: from.toISOString().slice(0, 10),
            to: to.toISOString().slice(0, 10)
        };
    }

    function getQueryParam(name) {
        if (!name) return "";
        if (window.URLSearchParams) {
            var params = new URLSearchParams(window.location.search || "");
            return params.get(name) || "";
        }
        var search = window.location.search || "";
        var match = new RegExp("[?&]" + name + "=([^&]*)", "i").exec(search);
        return match ? decodeURIComponent(match[1]) : "";
    }

    function setConnectionState(isOk, message) {
        var node = byId(ids.connectionStatus);
        if (!node) return;
        node.className = "dk-connection " + (isOk ? "ok" : "error");
        node.textContent = message;
    }

    function setLoading(loading) {
        state.loading = !!loading;

        var skeletonNodes = ["chartCapacityRing", "chartCustomerPie", "chartFlowTrend",
            "chartCapacityBar", "chartTop5MaxNL", "chartTop5MaxPL",
            "chartAgeStock", "chartActivityCalendarMonthly",
            "chartVolumePie", "chartTrendLine", "chartLoadBar",
            "lpcpDetailAssignments", "lpcpDetailPickOrders", "lpcpDetailWarnings"];

        var textMetrics = [
            "metricTonDauKy", "metricTongNhap", "metricTongXuat", "metricTonKho",
            "metricPOChuanBiVe", "metricPODangTre", "metricGiaTriTonKho",
            "metricTongNhapDelta", "metricTongXuatDelta", "metricTonKhoDelta", "metricGiaTriTonKhoDelta"
        ];
        var tableBodies = ["customersBody", "racksBody"];

        if (loading) {
            for (var i = 0; i < skeletonNodes.length; i++) {
                var n = byId(skeletonNodes[i]);
                if (n) {
                    n.innerHTML = "<div class=\"dk-skeleton\"><div class=\"dk-skeleton-shimmer\"></div></div>";
                }
            }
            for (var j = 0; j < textMetrics.length; j++) {
                var tm = byId(textMetrics[j]);
                if (tm) tm.innerHTML = "<div class=\"dk-skeleton\" style=\"width:60%; height:20px; display:inline-block;\"><div class=\"dk-skeleton-shimmer\"></div></div>";
            }
            for (var k = 0; k < tableBodies.length; k++) {
                var tb = byId(tableBodies[k]);
                if (tb) tb.innerHTML = "<tr><td colspan=\"10\"><div class=\"dk-skeleton\" style=\"height:30px;\"><div class=\"dk-skeleton-shimmer\"></div></div></td></tr>";
            }
        }
        var button = byId(ids.refreshButton);
        if (!button) return;
        button.disabled = state.loading;
        button.textContent = state.loading ? "Đang tải..." : "Làm mới";
    }

    // v2.4.6 — Render delta chip "↑ 18.6% so với kỳ trước"
    function formatDelta(delta) {
        if (delta === null || delta === undefined || isNaN(toNumber(delta))) return "";
        var d = toNumber(delta);
        var arrow = d > 0 ? "↑" : (d < 0 ? "↓" : "→");
        var cls = d > 0 ? "dk-delta-up" : (d < 0 ? "dk-delta-down" : "dk-delta-flat");
        return '<span class="' + cls + '">' + arrow + " " + formatNumber(Math.abs(d), 1) + "%</span> so với kỳ trước";
    }

    function renderMetricCards() {
        // v2.4.6 — 7 KPI mới: Tồn đầu kỳ + Tổng nhập + Tổng xuất + Tồn kho + PO chuẩn bị về + PO đang trễ + Giá trị tồn kho
        function setText(id, val) { var el = byId(id); if (el) el.textContent = val; }
        function setHtml(id, val) { var el = byId(id); if (el) el.innerHTML = val; }

        // 1) Tồn đầu kỳ
        if (state.kpiTonDauKy) setText("metricTonDauKy", formatNumber(toNumber(state.kpiTonDauKy.Value), 0));
        else setText("metricTonDauKy", "0");

        // 2) Tổng nhập
        if (state.kpiTongNhap) {
            setText("metricTongNhap", formatNumber(toNumber(state.kpiTongNhap.Value), 0));
            setHtml("metricTongNhapDelta", formatDelta(state.kpiTongNhap.Delta));
        } else {
            setText("metricTongNhap", "0");
            setHtml("metricTongNhapDelta", "");
        }

        // 3) Tổng xuất
        if (state.kpiTongXuat) {
            setText("metricTongXuat", formatNumber(toNumber(state.kpiTongXuat.Value), 0));
            setHtml("metricTongXuatDelta", formatDelta(state.kpiTongXuat.Delta));
        } else {
            setText("metricTongXuat", "0");
            setHtml("metricTongXuatDelta", "");
        }

        // 4) Tồn kho
        if (state.kpiTonKho) {
            setText("metricTonKho", formatNumber(toNumber(state.kpiTonKho.Value), 0));
            setHtml("metricTonKhoDelta", formatDelta(state.kpiTonKho.Delta));
        } else {
            setText("metricTonKho", "0");
            setHtml("metricTonKhoDelta", "");
        }
        // 5) PO chuẩn bị về
        setText("metricInboundReady", formatNumber(state.inbound.length, 0));
        setHtml("metricInboundReadyHint", state.inbound.length > 0 ? '<i class="fa-solid fa-fire dk-text-warn"></i> ' + state.inbound.length + " PO" : "");
        // 6) PO đang trễ
        if (state.kpiPODangTre) {
            setText("metricPODangTre", formatNumber(toNumber(state.kpiPODangTre.SoPO), 0));
            setHtml("metricPODangTreHint", '<i class="fa-solid fa-fire dk-text-danger"></i> ' + toNumber(state.kpiPODangTre.SoPOChuaKiem) + " PO chưa kiểm");
        } else {
            setText("metricPODangTre", "0");
            setHtml("metricPODangTreHint", "");
        }

        // 7) Giá trị tồn kho
        var thanhGiaEl = byId("metricThanhGia");
        if (thanhGiaEl) {
            var tg = state.kpiGiaTriTon ? toNumber(state.kpiGiaTriTon.Value) : (state.thanhGia ? toNumber(state.thanhGia.ThanhGia || state.thanhGia.TongTien) : 0);
            if (tg > 0) {
                var formatted;
                if (tg >= 1e9) formatted = (tg / 1e9).toFixed(3).replace(/\.?0+$/, "") + " tỷ";
                else if (tg >= 1e6) formatted = (tg / 1e6).toFixed(1).replace(/\.0$/, "") + " tr";
                else formatted = formatNumber(tg, 0);
                thanhGiaEl.textContent = formatted;
            } else {
                thanhGiaEl.textContent = "0";
            }
        }
        if (state.kpiGiaTriTon) {
            setHtml("metricGiaTriTonDelta", formatDelta(state.kpiGiaTriTon.Delta));
        } else {
            setHtml("metricGiaTriTonDelta", "");
        }
    }

    function rowDataAttr(type, index) {
        return 'class="clickable js-open-detail" data-detail="' + type + '" data-index="' + index + '"';
    }

    function renderCustomersTable() {
        var body = byId(ids.customersBody);
        if (!body) return;
        if (state.customers.length === 0) {
            body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có dữ liệu khách hàng</td></tr>';
            return;
        }

        var data = state.customers
            .map(function (item, index) {
                return { sourceIndex: index, item: item };
            })
            .sort(function (a, b) { return toNumber(b.item.CBMSDTrongKho) - toNumber(a.item.CBMSDTrongKho); })
            .slice(0, 12);

        var html = data.map(function (entry, stt) {
            var item = entry.item;
            var isFiltered = activeCustomerFilter && (item.MaKH !== activeCustomerFilter);
            var rowStyle = isFiltered ? " style=\"opacity:0.35\"" : "";
            return "<tr " + rowDataAttr("customerRow", entry.sourceIndex) + rowStyle + ">" +
                "<td><b>" + (stt + 1) + "</b></td>" +
                "<td>" + escapeHtml(item.MaKH || "") + "</td>" +
                "<td style=\"text-align:left\">" + escapeHtml(normalizeCustomerName(item.TenKH)) + "</td>" +
                "<td class=\"text-end\">" + formatNumber(item.SLVatTu, 0) + "</td>" +
                "<td class=\"text-end\">" + formatNumber(item.CBMSDTrongKho, 2) + "</td>" +
                "</tr>";
        }).join("");

        body.innerHTML = html;
    }

    function renderRacksTable() {
        var body = byId(ids.racksBody);
        if (!body) return;
        if (state.racks.length === 0) {
            body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có dữ liệu kệ kho</td></tr>';
            return;
        }

        var ranked = state.racks.map(function (item, index) {
            var used = toNumber(item.TongCBMSuDungTrongKe);
            var cap = toNumber(item.TongCBMTrongKe);
            return {
                sourceIndex: index,
                Module: item.Module == 1 ? "NL" : (item.Module == 2 ? "PL" : String(item.Module || "")),
                TenKe: item.TenKe,
                used: used,
                rate: cap > 0 ? (used / cap) * 100 : 0
            };
        }).sort(function (a, b) { return b.rate - a.rate; }).slice(0, 12);

        // v2.4.9 — pct bar gradient + module chip + top3 highlight
        var html = ranked.map(function (item, stt) {
            var pct = item.rate;
            var lvl = pct >= 100 ? "extreme" : (pct >= 85 ? "high" : (pct >= 50 ? "med" : "low"));
            var modCls = item.Module === "NL" ? "dk-rack-mod-nl" : "dk-rack-mod-pl";
            var topCls = stt < 3 ? " dk-rack-row-top" : "";
            var pctBarHtml =
                '<div class="dk-rack-pct">' +
                '<div class="dk-rack-pct-bar dk-rack-pct-' + lvl + '" style="width:' + Math.min(100, pct).toFixed(1) + '%"></div>' +
                '<span class="dk-rack-pct-num">' +
                (pct >= 100 ? '<i class="fa-solid fa-triangle-exclamation"></i> ' : '') +
                formatNumber(pct, 1) + '%' +
                '</span>' +
                '</div>';
            return "<tr class=\"dk-rack-row" + topCls + "\" " + rowDataAttr("rackRow", item.sourceIndex) + ">" +
                "<td class=\"text-center\"><b>" + (stt + 1) + "</b></td>" +
                "<td class=\"text-center\"><span class=\"dk-rack-module-chip " + modCls + "\">" + escapeHtml(item.Module) + "</span></td>" +
                "<td>" + escapeHtml(item.TenKe || "") + "</td>" +
                "<td class=\"text-end dk-cell-num\">" + formatNumber(item.used, 2) + "</td>" +
                "<td>" + pctBarHtml + "</td>" +
                "</tr>";
        }).join("");

        body.innerHTML = html;
    }

    function renderInboundTable() {
        var body = byId(ids.inboundBody);
        if (!body) return;
        if (state.inbound.length === 0) {
            body.innerHTML = '<tr><td colspan="4" class="dk-empty">Không có lệnh chuẩn bị về</td></tr>';
            return;
        }

        var data = state.inbound
            .map(function (item, index) {
                return { sourceIndex: index, item: item };
            })
            .sort(function (a, b) {
                return toNumber(parseDate(a.item.NgayNKDuKien)) - toNumber(parseDate(b.item.NgayNKDuKien));
            })
            .slice(0, 12);

        var html = data.map(function (entry) {
            var item = entry.item;
            return "<tr " + rowDataAttr("inboundRow", entry.sourceIndex) + ">" +
                "<td>" + escapeHtml(item.PO || item.POMua || "") + "</td>" +
                "<td>" + escapeHtml(item.MaHang || "") + "</td>" +
                "<td class=\"text-end\">" + formatNumber(item.SoLuongSP, 0) + "</td>" +
                "<td>" + escapeHtml(formatDate(item.NgayNKDuKien)) + "</td>" +
                "</tr>";
        }).join("");

        body.innerHTML = html;
    }

    function renderOutboundReadyTable() {
        var body = byId(ids.outboundReadyBody);
        if (!body) return;
        if (state.outboundReady.length === 0) {
            body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có lệnh chuẩn bị xuất</td></tr>';
            return;
        }

        var html = state.outboundReady.slice(0, 12).map(function (item, stt) {
            return "<tr " + rowDataAttr("outboundReadyRow", stt) + ">" +
                "<td><b>" + (stt + 1) + "</b></td>" +
                "<td style=\"text-align:left;font-weight:700\">" + escapeHtml(shortMaLenh(item.MaLenhSanXuat || "")) + "</td>" +
                "<td style=\"text-align:left\">" + escapeHtml(normalizeCustomerName(item.KhachHang)) + "</td>" +
                "<td>" + escapeHtml(formatDate(item.KHCat)) + "</td>" +
                "<td>" + escapeHtml(formatDate(item.DuKienCat)) + "</td>" +
                "</tr>";
        }).join("");

        body.innerHTML = html;
    }

    function renderOutboundRunningTable() {
        var body = byId(ids.outboundRunningBody);
        if (!body) return;
        if (state.outboundRunning.length === 0) {
            body.innerHTML = '<tr><td colspan="5" class="dk-empty">Không có lệnh đang xuất</td></tr>';
            return;
        }

        var html = state.outboundRunning.slice(0, 12).map(function (item, stt) {
            return "<tr " + rowDataAttr("outboundRunningRow", stt) + ">" +
                "<td><b>" + (stt + 1) + "</b></td>" +
                "<td style=\"text-align:left;font-weight:700\">" + escapeHtml(shortMaLenh(item.MaLenhSX || "")) + "</td>" +
                "<td style=\"text-align:left\">" + escapeHtml(item.TenHang || "") + "</td>" +
                "<td style=\"text-align:left\">" + escapeHtml(normalizeCustomerName(item.TenKH)) + "</td>" +
                "<td>" + escapeHtml(formatDate(item.NgayXuatHang)) + "</td>" +
                "<td class=\"text-end\">" + formatNumber(item.SLXuat, 2) + "</td>" +
                "</tr>";
        }).join("");

        body.innerHTML = html;
    }

    function renderQuickStatus() {
        var statusNode = byId(ids.quickStatus);
        if (!statusNode) return;

        var racksCritical = state.racks.filter(function (item) {
            var used = toNumber(item.TongCBMSuDungTrongKe);
            var cap = toNumber(item.TongCBMTrongKe);
            if (cap <= 0) return false;
            return ((used / cap) * 100) >= 90;
        }).length;

        var inboundOverdue = state.inbound.filter(function (item) {
            var date = parseDate(item.NgayNKDuKien);
            if (!date) return false;
            var today = new Date();
            var midnight = new Date(today.getFullYear(), today.getMonth(), today.getDate());
            return date.getTime() < midnight.getTime();
        }).length;

        var outboundHighVolume = state.outboundRunning.filter(function (item) {
            return toNumber(item.SLXuat) > 1000;
        }).length;

        var blocks = [];
        blocks.push('<div class="dk-status-item ' + (racksCritical > 0 ? "warn" : "good") + '"><strong>Kệ vượt 90%:</strong> ' + formatNumber(racksCritical, 0) + ' kệ</div>');
        blocks.push('<div class="dk-status-item ' + (inboundOverdue > 0 ? "bad" : "good") + '"><strong>Lệnh nhập quá hạn:</strong> ' + formatNumber(inboundOverdue, 0) + ' lệnh</div>');
        blocks.push('<div class="dk-status-item ' + (outboundHighVolume > 0 ? "warn" : "good") + '"><strong>Lệnh xuất SL lớn (&gt;1000):</strong> ' + formatNumber(outboundHighVolume, 0) + ' lệnh</div>');
        blocks.push('<div class="dk-status-item"><strong>Kho đang theo dõi:</strong> ' + formatNumber(state.customers.length, 0) + ' khách hàng | ' + formatNumber(state.racks.length, 0) + ' kệ</div>');

        statusNode.innerHTML = blocks.join("");
    }

    function renderCapacityChart() {
        var ringNode = byId(ids.chartCapacityRing);
        var legendNode = byId(ids.chartCapacityLegend);
        if (!ringNode || !legendNode) return;

        var overall = state.overall.length > 0 ? state.overall[0] : {};

        // v2.6.0 — Use pre-calculated per-warehouse percentages from API
        // PercentNPL = UsedNPL/CapacityNPL*100, PercentPL = UsedPL/CapacityPL*100
        var usedNpl = Math.max(0, toNumber(overall.UsedNPL));
        var usedPl = Math.max(0, toNumber(overall.UsedPL));
        var totalCapacity = Math.max(0, toNumber(overall.TotalCapacity));
        var capNpl = Math.max(0, toNumber(overall.CapacityNPL));
        var capPl = Math.max(0, toNumber(overall.CapacityPL));
        var totalUsed = usedNpl + usedPl;
        // Ring segments: proportion of TOTAL capacity (for donut visual)
        var segNpl, segPl, segFree, pctTotal;
        // Per-kho fill % (for legend labels, matching gauge)
        var fillNpl, fillPl;
        if (totalCapacity > 0) {
            segNpl = (usedNpl / totalCapacity) * 100;
            segPl = (usedPl / totalCapacity) * 100;
            pctTotal = segNpl + segPl;
            segFree = Math.max(0, 100 - pctTotal);
            fillNpl = capNpl > 0 ? (usedNpl / capNpl) * 100 : 0;
            fillPl = capPl > 0 ? (usedPl / capPl) * 100 : 0;
        } else {
            segNpl = 0; segPl = 0; segFree = 0; pctTotal = 0;
            fillNpl = 0; fillPl = 0;
        }

        if (segNpl <= 0 && segPl <= 0 && segFree <= 0) {
            ringNode.innerHTML = "<div class=\"dk-empty\">Không có dữ liệu</div>";
            return;
        }

        // v2.4.3 — Segment màu sáng hơn ở dark (NL/PL/Còn trống)
        var _darkSeg = dkIsDark();
        var colNL = _darkSeg ? "#60a5fa" : "#0b5bf0";
        var colPL = _darkSeg ? "#fbbf24" : "#f5a623";
        var colFree = _darkSeg ? "#94a3b8" : "#9fb3c6";
        // Dùng % làm giá trị vẽ vòng tròn (tổng = 100)
        var segments = [
            { name: "Đã dùng NL", value: segNpl, color: colNL, label: formatNumber(segNpl, 2) + "%" },
            { name: "Đã dùng PL", value: segPl, color: colPL, label: formatNumber(segPl, 2) + "%" },
            { name: "Còn trống", value: segFree, color: colFree, label: formatNumber(segFree, 2) + "%" }
        ];
        var totalPct = segNpl + segPl + segFree;
        // Nếu tổng chưa đủ 100 (do làm tròn), điều chỉnh phần trống
        if (totalPct < 99.5) {
            segments[2].value = Math.max(0, 100 - segNpl - segPl);
            segments[2].label = formatNumber(segments[2].value, 2) + "%";
        }

        var svgWidth = 900;
        var svgHeight = 440;
        var cx = 450;
        var cy = 220;
        var r = 145;
        var strokeWidth = 42;
        var c = 2 * Math.PI * r;
        var dashOffset = 0;
        var circles = [];
        var calloutLines = [];
        var calloutBoxes = [];
        var startAngle = -Math.PI / 2;

        var rightLastY = -999;
        var leftLastY = -999;
        var MIN_GAP = 68;
        var boxW = 196;
        var boxH = 57;
        var MARGIN = 6;

        var defs = "<defs>";

        for (var i = 0; i < segments.length; i++) {
            var seg = segments[i];

            defs += "<marker id=\"arrow_" + i + "\" markerWidth=\"8\" markerHeight=\"8\" refX=\"7\" refY=\"4\" orient=\"auto\"><polygon points=\"0 0, 8 4, 0 8\" fill=\"" + seg.color + "\"></polygon></marker>";

            if (seg.value <= 0) continue;

            var segLength = (seg.value / 100) * c;
            circles.push(
                "<circle cx=\"" + cx + "\" cy=\"" + cy + "\" r=\"" + r + "\" fill=\"none\" stroke=\"" + seg.color + "\" stroke-width=\"" + strokeWidth + "\" " +
                "stroke-dasharray=\"" + segLength + " " + (c - segLength) + "\" stroke-dashoffset=\"" + (-dashOffset) + "\" transform=\"rotate(-90 " + cx + " " + cy + ")\" " +
                "data-cap-seg=\"" + i + "\" style=\"transition:opacity 0.2s;\" />"
            );
            dashOffset += segLength;

            var segAngle = (seg.value / 100) * Math.PI * 2;
            var midAngle = startAngle + (segAngle / 2);
            var direction = Math.cos(midAngle) >= 0 ? 1 : -1;

            var lineStartX = cx + Math.cos(midAngle) * (r + strokeWidth / 2);
            var lineStartY = cy + Math.sin(midAngle) * (r + strokeWidth / 2);
            var bendX = cx + Math.cos(midAngle) * (r + strokeWidth / 2 + 28);
            var bendY = cy + Math.sin(midAngle) * (r + strokeWidth / 2 + 28);

            if (direction > 0) {
                if (bendY - rightLastY < MIN_GAP) bendY = rightLastY + MIN_GAP;
                rightLastY = bendY;
            } else {
                if (bendY - leftLastY < MIN_GAP) bendY = leftLastY + MIN_GAP;
                leftLastY = bendY;
            }

            var boxX, boxCX, endX;
            if (direction > 0) {
                boxX = svgWidth - boxW - MARGIN;
                endX = boxX;
            } else {
                boxX = MARGIN;
                endX = boxX + boxW;
            }
            boxCX = boxX + boxW / 2;
            var boxY = bendY - boxH / 2;

            calloutLines.push(
                "<polyline points=\"" + lineStartX + "," + lineStartY + " " + bendX + "," + bendY + " " + endX + "," + bendY + "\" fill=\"none\" stroke=\"" + seg.color + "\" stroke-width=\"2\" marker-end=\"url(#arrow_" + i + ")\"/>"
            );
            calloutBoxes.push(
                "<g class=\"cap-lbl\" data-seg=\"" + i + "\" style=\"cursor:pointer;\">" +
                "<rect class=\"dk-callout-bg\" x=\"" + boxX + "\" y=\"" + boxY + "\" width=\"" + boxW + "\" height=\"" + boxH + "\" fill=\"#ffffff\" stroke=\"" + seg.color + "\" stroke-width=\"2\" rx=\"6\" />" +
                "<text x=\"" + boxCX + "\" y=\"" + bendY + "\" text-anchor=\"middle\" class=\"dk-capacity-callout-label\" style=\"dominant-baseline:middle; fill:" + seg.color + "; font-size:14.9px;\">" +
                "<tspan x=\"" + boxCX + "\" dy=\"-0.6em\" font-weight=\"bold\">" + escapeHtml(seg.name) + "</tspan>" +
                "<tspan x=\"" + boxCX + "\" dy=\"1.3em\">" + escapeHtml(seg.label) + "</tspan>" +
                "</text>" +
                "</g>"
            );
            startAngle += segAngle;
        }
        defs += "</defs>";

        var innerR = r - strokeWidth / 2 - 2;
        var centerPct = pctTotal > 0 ? pctTotal : (segNpl + segPl);
        // v2.4.2 — Ring track + inner hole theo theme (light=trắng, dark=navy đậm)
        var _dark = dkIsDark();
        var trackStroke = _dark ? "#1e3a6b" : "#e4edf6";
        var innerFill = _dark ? "#0d1d35" : "#ffffff";
        ringNode.innerHTML = "<svg viewBox=\"0 0 " + svgWidth + " " + svgHeight + "\" style=\"width:100%; height:100%;\">" +
            defs +
            calloutLines.join("") +
            "<circle cx=\"" + cx + "\" cy=\"" + cy + "\" r=\"" + r + "\" fill=\"none\" stroke=\"" + trackStroke + "\" stroke-width=\"" + strokeWidth + "\" />" +
            circles.join("") +
            "<circle cx=\"" + cx + "\" cy=\"" + cy + "\" r=\"" + innerR + "\" fill=\"" + innerFill + "\" />" +
            "<text x=\"" + cx + "\" y=\"" + (cy - 6) + "\" class=\"dk-capacity-center\" style=\"font-size:40px;\">" + formatNumber(centerPct, 1) + "%</text>" +
            "<text x=\"" + cx + "\" y=\"" + (cy + 28) + "\" class=\"dk-flow-label\" style=\"font-size:15px; text-anchor:middle;\">Lấp đầy</text>" +
            calloutBoxes.join("") +
            "</svg>";
        legendNode.innerHTML = "";

        // Hover: highlight hovered segment, dim others
        var capSvg = ringNode.querySelector("svg");
        if (capSvg) {
            capSvg.addEventListener("mouseover", function (e) {
                var g = e.target;
                while (g && g !== capSvg) {
                    if (g.classList && g.classList.contains("cap-lbl")) break;
                    g = g.parentNode;
                }
                if (!g || g === capSvg) return;
                var idx = g.getAttribute("data-seg");
                var allSegs = capSvg.querySelectorAll("[data-cap-seg]");
                for (var s = 0; s < allSegs.length; s++) {
                    allSegs[s].style.opacity = (allSegs[s].getAttribute("data-cap-seg") === idx) ? "1" : "0.2";
                }
            });
            capSvg.addEventListener("mouseout", function (e) {
                var g = e.target;
                while (g && g !== capSvg) {
                    if (g.classList && g.classList.contains("cap-lbl")) break;
                    g = g.parentNode;
                }
                if (!g || g === capSvg) return;
                var related = e.relatedTarget;
                if (related && g.contains && g.contains(related)) return;
                var allSegs = capSvg.querySelectorAll("[data-cap-seg]");
                for (var s = 0; s < allSegs.length; s++) {
                    allSegs[s].style.opacity = "1";
                }
            });
        }
    }

    function buildCustomerDetailRows() {
        var totalCbm = state.customers.reduce(function (sum, item) {
            return sum + Math.max(0, toNumber(item.CBMSDTrongKho));
        }, 0);
        var sorted = state.customers.map(function (item, index) {
            var cbm = Math.max(0, toNumber(item.CBMSDTrongKho));
            return {
                sourceIndex: index,
                MaKH: item.MaKH || "",
                TenKH: normalizeCustomerName(item.TenKH),
                SLVatTu: toNumber(item.SLVatTu),
                CBMSDTrongKho: cbm,
                TyTrongCBM: totalCbm > 0 ? (cbm / totalCbm) * 100 : 0
            };
        }).sort(function (a, b) {
            return b.CBMSDTrongKho - a.CBMSDTrongKho;
        });
        for (var i = 0; i < sorted.length; i++) {
            sorted[i].ThuHang = i + 1;
        }
        return sorted;
    }

    function renderCustomerPieChart() {
        var node = byId(ids.chartCustomerPie);
        if (!node) return;
        if (state.customers.length === 0) {
            node.innerHTML = "<div class=\"dk-empty\">Không có dữ liệu khách hàng</div>";
            return;
        }

        // Feature 11: customer filter flag (declared at top to avoid hoisting issues)
        var filterActive = activeCustomerFilter !== "";

        // Hiện TẤT CẢ khách hàng (không giới hạn top 8)
        var data = buildCustomerDetailRows();
        var total = data.reduce(function (sum, row) { return sum + row.CBMSDTrongKho; }, 0);
        if (total <= 0) {
            node.innerHTML = "<div class=\"dk-empty\">Không đủ dữ liệu CBM để vẽ biểu đồ</div>";
            return;
        }

        var n = data.length;
        var halfN = Math.ceil(n / 2);
        var MIN_GAP_DYN = Math.min(60, Math.max(38, Math.floor(460 / halfN)));
        var height = Math.max(550, Math.round((halfN * MIN_GAP_DYN + 120) * 1.1));
        var width = 860;
        var cx = width / 2;
        var cy = height / 2;
        var r = 138;
        var strokeWidth = 61;
        var c = 2 * Math.PI * r;
        // v2.4.3 — Palette sáng hơn ở dark (bỏ #1a3a6b dark-navy, dùng full bright)
        var colors = dkIsDark()
            ? ["#60a5fa", "#34d399", "#fbbf24", "#a78bfa", "#22d3ee", "#f87171", "#f472b6", "#5eead4", "#fb923c", "#c4b5fd", "#6ee7b7"]
            : ["#1a3a6b", "#2f7fd9", "#f5a623", "#8d6be6", "#00a6c7", "#e34f70", "#718096", "#5ebf5e", "#d97706", "#7c3aed", "#059669"];
        var offset = 0;
        var startAngle = -Math.PI / 2;

        var segments = [];
        var labels = [];
        var defs = [];
        var anchors = [];

        // Add shadow filter
        defs.push("<filter id=\"pieShadow\" x=\"-20%\" y=\"-20%\" width=\"140%\" height=\"140%\"><feDropShadow dx=\"0\" dy=\"6\" stdDeviation=\"8\" flood-color=\"#10283f\" flood-opacity=\"0.15\" /></filter>");

        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            var ratio = row.CBMSDTrongKho / total;
            var segLength = ratio * c;
            var color = colors[i % colors.length];

            var segOpacity = (!filterActive || row.MaKH === activeCustomerFilter) ? "1" : "0.15";
            segments.push(
                "<circle cx=\"" + cx + "\" cy=\"" + cy + "\" r=\"" + r + "\" fill=\"none\" stroke=\"" + color + "\" stroke-width=\"" + strokeWidth + "\" " +
                "stroke-dasharray=\"" + segLength + " " + Math.max(0, c - segLength) + "\" stroke-dashoffset=\"" + (-offset) + "\" transform=\"rotate(-90 " + cx + " " + cy + ")\" " +
                "class=\"js-open-detail\" data-cust-seg=\"" + i + "\" style=\"cursor:pointer; filter:url(#pieShadow); transition:opacity 0.2s; opacity:" + segOpacity + ";\" data-detail=\"customerRow\" data-index=\"" + row.sourceIndex + "\"><title>" + escapeHtml(row.TenKH) + "</title></circle>"
            );
            offset += segLength;

            // Chia đều: nửa đầu sang trái, nửa sau sang phải để tự cân bằng
            var half = Math.ceil(data.length / 2);
            var forcedSide = i < half ? -1 : 1;

            anchors.push({
                i: i,
                row: row,
                color: color,
                side: forcedSide,
                angle: startAngle + ratio * Math.PI,
                preferY: cy + Math.sin(startAngle + ratio * Math.PI) * (r + 40)
            });
            startAngle += ratio * Math.PI * 2;
        }

        function distributeBySide(items) {
            if (items.length === 0) return;
            items.sort(function (a, b) { return a.preferY - b.preferY; });
            var minY = 32;
            var maxY = height - 32;
            // Reduce gap if needed to fit all items
            var minGap = Math.min(MIN_GAP_DYN || 60, Math.floor((maxY - minY) / Math.max(items.length, 1)));
            minGap = Math.max(minGap, 22); // never less than 22px

            for (var j = 0; j < items.length; j++) {
                var prevY = j > 0 ? items[j - 1].labelY : minY - minGap;
                items[j].labelY = Math.max(items[j].preferY, prevY + minGap);
            }
            // If last item exceeds maxY, shift everything up proportionally
            var last = items[items.length - 1];
            if (last.labelY > maxY) {
                var overflow = last.labelY - maxY;
                for (var k = items.length - 1; k >= 0; k--) {
                    items[k].labelY -= overflow;
                    overflow = 0;
                    if (k > 0 && items[k].labelY - items[k - 1].labelY < minGap) {
                        items[k - 1].labelY = items[k].labelY - minGap;
                        overflow = Math.max(0, minY - items[0].labelY);
                    }
                }
                // Final clamp
                for (var m = 0; m < items.length; m++) {
                    items[m].labelY = Math.max(minY + m * minGap, Math.min(maxY - (items.length - 1 - m) * minGap, items[m].labelY));
                }
            }
        }

        var rightSide = anchors.filter(function (a) { return a.side > 0; });
        var leftSide = anchors.filter(function (a) { return a.side < 0; });
        distributeBySide(rightSide);
        distributeBySide(leftSide);

        var boxW = 182;
        var boxH = 54;
        var labelLines = [];  // drawn BEFORE ring
        var labelBoxes = [];  // drawn AFTER ring + inner fill

        for (var a = 0; a < anchors.length; a++) {
            var item = anchors[a];

            var edgeX = cx + Math.cos(item.angle) * (r + strokeWidth / 2 + 5);
            var edgeY = cy + Math.sin(item.angle) * (r + strokeWidth / 2 + 5);
            var radialX = cx + Math.cos(item.angle) * (r + strokeWidth / 2 + 35);
            var radialY = cy + Math.sin(item.angle) * (r + strokeWidth / 2 + 35);
            var elbowX = cx + item.side * (r + strokeWidth / 2 + 50);
            // Box anchored to SVG edge; arrow tip at box edge
            var boxX, boxCX, endX;
            if (item.side > 0) {
                boxX = width - boxW - 6;
                endX = boxX;
            } else {
                boxX = 6;
                endX = boxX + boxW;
            }
            boxCX = boxX + boxW / 2;
            var boxY = item.labelY - boxH / 2;

            // Smooth bezier line + dot at box end (no triangle arrow)
            var lineStartX = cx + Math.cos(item.angle) * (r + strokeWidth / 2);
            var lineStartY = cy + Math.sin(item.angle) * (r + strokeWidth / 2);
            // Control point for smooth curve
            var ctrlX = (radialX + elbowX) / 2;
            var ctrlY = (radialY + item.labelY) / 2;
            labelLines.push(
                "<path d=\"M " + lineStartX + " " + lineStartY + " C " + radialX + " " + radialY + " " + ctrlX + " " + ctrlY + " " + endX + " " + item.labelY + "\" fill=\"none\" stroke=\"" + item.color + "\" stroke-width=\"2\" stroke-opacity=\"0.5\" stroke-linecap=\"round\" />" +
                "<circle cx=\"" + endX + "\" cy=\"" + item.labelY + "\" r=\"4\" fill=\"" + item.color + "\" fill-opacity=\"0.7\" />"
            );
            var lbOpacity = (!filterActive || item.row.MaKH === activeCustomerFilter) ? "1" : "0.15";
            labelBoxes.push(
                "<g class=\"cust-lbl js-open-detail\" data-seg=\"" + item.i + "\" data-detail=\"customerRow\" data-index=\"" + item.row.sourceIndex + "\" style=\"cursor:pointer;opacity:" + lbOpacity + ";\">" +
                "<rect class=\"dk-callout-bg\" x=\"" + boxX + "\" y=\"" + boxY + "\" width=\"" + boxW + "\" height=\"" + boxH + "\" fill=\"#ffffff\" fill-opacity=\"0.95\" stroke=\"" + item.color + "\" stroke-width=\"1.5\" rx=\"10\" filter=\"url(#pieShadow)\" />" +
                "<text x=\"" + boxCX + "\" y=\"" + item.labelY + "\" text-anchor=\"middle\" class=\"dk-customer-callout\" style=\"dominant-baseline: middle; fill: " + item.color + "; font-size: 14px;\">" +
                "<tspan x=\"" + boxCX + "\" dy=\"-0.7em\" font-weight=\"800\">" + escapeHtml(item.row.TenKH || item.row.MaKH) + "</tspan>" +
                "<tspan x=\"" + boxCX + "\" dy=\"1.4em\">" + formatNumber(item.row.TyTrongCBM, 1) + "% (" + formatNumber(item.row.CBMSDTrongKho, 1) + " CBM)</tspan>" +
                "</text>" +
                "</g>"
            );
        }

        var innerRCust = r - strokeWidth / 2 - 2;
        // v2.4.2 — track + inner hole theo theme
        var _darkC = dkIsDark();
        var trackStrokeC = _darkC ? "#1e3a6b" : "#e6edf5";
        var innerFillC = _darkC ? "#0d1d35" : "#ffffff";
        node.innerHTML = "<svg viewBox=\"0 0 " + width + " " + height + "\" style=\"width:100%; height:100%; display:block;\" role=\"img\" aria-label=\"Tỷ trọng khách hàng theo CBM\">" +
            "<defs>" + defs.join("") + "</defs>" +
            labelLines.join("") +
            "<circle cx=\"" + cx + "\" cy=\"" + cy + "\" r=\"" + r + "\" fill=\"none\" stroke=\"" + trackStrokeC + "\" stroke-width=\"" + strokeWidth + "\"/>" +
            segments.join("") +
            "<circle cx=\"" + cx + "\" cy=\"" + cy + "\" r=\"" + innerRCust + "\" fill=\"" + innerFillC + "\" />" +
            "<text x=\"" + cx + "\" y=\"" + (cy - 6) + "\" text-anchor=\"middle\" class=\"dk-capacity-center\" style=\"font-size:42px;\">" + state.customers.length + "</text>" +
            "<text x=\"" + cx + "\" y=\"" + (cy + 26) + "\" text-anchor=\"middle\" class=\"dk-flow-label\" style=\"font-size:16px;\">Khách hàng</text>" +
            labelBoxes.join("") +
            "</svg>";

        // Hover: highlight hovered segment, dim others
        var svgEl = node.querySelector("svg");
        if (svgEl) {
            svgEl.addEventListener("mouseover", function (e) {
                var g = e.target;
                while (g && g !== svgEl) {
                    if (g.classList && g.classList.contains("cust-lbl")) break;
                    g = g.parentNode;
                }
                if (!g || g === svgEl) return;
                var idx = g.getAttribute("data-seg");
                var allSegs = svgEl.querySelectorAll("[data-cust-seg]");
                for (var s = 0; s < allSegs.length; s++) {
                    allSegs[s].style.opacity = (allSegs[s].getAttribute("data-cust-seg") === idx) ? "1" : "0.2";
                }
            });
            svgEl.addEventListener("mouseout", function (e) {
                var g = e.target;
                while (g && g !== svgEl) {
                    if (g.classList && g.classList.contains("cust-lbl")) break;
                    g = g.parentNode;
                }
                if (!g || g === svgEl) return;
                var related = e.relatedTarget;
                if (related && g.contains && g.contains(related)) return;
                var allSegs = svgEl.querySelectorAll("[data-cust-seg]");
                for (var s = 0; s < allSegs.length; s++) {
                    allSegs[s].style.opacity = "1";
                }
            });
        }
    }

    function toDateKey(raw) {
        var date = parseDate(raw);
        if (!date) return "";
        return date.getFullYear() + "-" + String(date.getMonth() + 1).padStart(2, "0") + "-" + String(date.getDate()).padStart(2, "0");
    }

    function buildFlowSeries() {
        var today = new Date();
        var months = [];
        var thisMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        var startMonth = addMonths(thisMonth, -11);
        for (var i = 0; i < 12; i++) {
            months.push(addMonths(startMonth, i));
        }

        var labels = [];
        var inboundValues = [];
        var outboundValues = [];
        var stockValues = [];

        if (state.flowTrend12T && state.flowTrend12T.length > 0) {
            var trend = state.flowTrend12T;
            var trendMap = {};
            for (var t = 0; t < trend.length; t++) {
                var tk = trend[t].Nam + "-" + String(trend[t].Thang).padStart(2, "0");
                trendMap[tk] = {
                    inn: toNumber(trend[t].TotalIn),
                    out: toNumber(trend[t].TotalOut),
                    stock: toNumber(trend[t].TotalStock)
                };
            }
            for (var d = 0; d < months.length; d++) {
                var md = months[d];
                var mk = md.getFullYear() + "-" + String(md.getMonth() + 1).padStart(2, "0");
                var ml = String(md.getMonth() + 1).padStart(2, "0") + "/" + String(md.getFullYear()).slice(-2);
                labels.push(ml);
                var row = trendMap[mk] || { inn: 0, out: 0, stock: 0 };
                inboundValues.push(row.inn);
                outboundValues.push(row.out);
                stockValues.push(row.stock);
            }
        } else {
            for (var d2 = 0; d2 < months.length; d2++) {
                var md2 = months[d2];
                labels.push(String(md2.getMonth() + 1).padStart(2, "0") + "/" + String(md2.getFullYear()).slice(-2));
                inboundValues.push(0);
                outboundValues.push(0);
                stockValues.push(0);
            }
        }

        return {
            labels: labels,
            inbound: inboundValues,
            outbound: outboundValues,
            stock: stockValues
        };
    }

    function renderFlowTrendChart() {
        var node = byId(ids.chartFlowTrend);
        if (!node) return;
        var series = buildFlowSeries();

        function getNiceMax(value) {
            if (!isFinite(value) || value <= 0) return 10;
            var exp = Math.pow(10, Math.floor(Math.log(value) / Math.LN10));
            var base = value / exp;
            var niceBase = base <= 1 ? 1 : (base <= 2 ? 2 : (base <= 5 ? 5 : 10));
            return niceBase * exp;
        }

        // Max cho cả 3 series: Nhập, Xuất, Tồn (cùng trục)
        var maxVal = 1;
        var allValues = series.inbound.concat(series.outbound).concat(series.stock);
        for (var i = 0; i < allValues.length; i++) { if (allValues[i] > maxVal) maxVal = allValues[i]; }
        maxVal = getNiceMax(maxVal * 1.15);

        var W = 820, H = 320;
        var L = 60, R = 20, T = 30, B = 46;
        var plotW = W - L - R, plotH = H - T - B;
        var axisY = T + plotH;
        var N = series.labels.length;

        function groupX(idx) {
            if (N <= 1) return L + plotW / 2;
            var margin = 28;
            return (L + margin) + (idx / (N - 1)) * (plotW - margin * 2);
        }
        function yScale(v) { return T + (1 - v / maxVal) * plotH; }

        var groupStep = N > 1 ? (plotW / (N - 1)) : plotW;
        var barW = Math.max(12, Math.min(30, groupStep * 0.32));
        var gap = 3;

        var colorIn = "#5bb8f5";
        var colorOut = "#a78bfa";
        var colorLine = "#f97316";

        var defs = "<defs>" +
            "<linearGradient id=\"gradIn\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#74c7f8\"/><stop offset=\"100%\" stop-color=\"#3d9de8\"/></linearGradient>" +
            "<linearGradient id=\"gradOut\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#c4b5fd\"/><stop offset=\"100%\" stop-color=\"#8b5cf6\"/></linearGradient>" +
            // v2.4.10 — Gradient area dưới line tồn kho
            "<linearGradient id=\"gradStockArea\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#f97316\" stop-opacity=\"0.25\"/><stop offset=\"100%\" stop-color=\"#f97316\" stop-opacity=\"0\"/></linearGradient>" +
            "<filter id=\"bsf\" x=\"-20%\" y=\"-20%\" width=\"140%\" height=\"140%\"><feDropShadow dx=\"0\" dy=\"2\" stdDeviation=\"2\" flood-color=\"#0f172a\" flood-opacity=\"0.12\"/></filter>" +
            "</defs>";

        // Lưới — 1 trục Y duy nhất (số lượng)
        var grid = "";
        for (var g = 0; g <= 4; g++) {
            var gv = (maxVal / 4) * g;
            var gy = yScale(gv);
            grid += "<line x1=\"" + L + "\" y1=\"" + gy + "\" x2=\"" + (W - R) + "\" y2=\"" + gy + "\" stroke=\"#e2eaf4\" stroke-dasharray=\"4 3\" stroke-width=\"1\"/>";
            grid += "<text x=\"" + (L - 4) + "\" y=\"" + (gy + 4) + "\" text-anchor=\"end\" class=\"dk-flow-label\">" + formatNumber(gv, 0) + "</text>";
        }
        grid += "<line x1=\"" + L + "\" y1=\"" + axisY + "\" x2=\"" + (W - R) + "\" y2=\"" + axisY + "\" stroke=\"#b0c4d8\" stroke-width=\"1.5\"/>";

        var barsIn = "", barsOut = "";
        for (var bi = 0; bi < N; bi++) {
            var gx = groupX(bi);
            var yin = yScale(series.inbound[bi]);
            var hin = Math.max(2, axisY - yin);
            var yout = yScale(series.outbound[bi]);
            var hout = Math.max(2, axisY - yout);
            // v2.4.10 — class dk-flow-bar để animate scaleY grow + stagger delay
            barsIn += "<rect class=\"dk-flow-bar dk-flow-bar-in\"  style=\"transform-origin:" + (gx - barW - gap / 2 + barW / 2) + "px " + axisY + "px;animation-delay:" + (bi * 25) + "ms\" x=\"" + (gx - barW - gap / 2) + "\" y=\"" + yin + "\" width=\"" + barW + "\" height=\"" + hin + "\" rx=\"3\" fill=\"url(#gradIn)\"  filter=\"url(#bsf)\"/>";
            barsOut += "<rect class=\"dk-flow-bar dk-flow-bar-out\" style=\"transform-origin:" + (gx + gap / 2 + barW / 2) + "px " + axisY + "px;animation-delay:" + (bi * 25 + 80) + "ms\" x=\"" + (gx + gap / 2) + "\" y=\"" + yout + "\" width=\"" + barW + "\" height=\"" + hout + "\" rx=\"3\" fill=\"url(#gradOut)\" filter=\"url(#bsf)\"/>";
        }

        // v2.4.10 — Smooth spline (Catmull-Rom-ish via cubic bezier) cho line tồn kho + area gradient dưới
        var pts = [];
        for (var pti = 0; pti < N; pti++) pts.push({ x: groupX(pti), y: yScale(series.stock[pti]) });
        var smoothD = "";
        if (pts.length > 0) {
            smoothD = "M " + pts[0].x + " " + pts[0].y;
            for (var si = 0; si < pts.length - 1; si++) {
                var p0 = pts[si === 0 ? 0 : si - 1];
                var p1 = pts[si];
                var p2 = pts[si + 1];
                var p3 = pts[si + 2 < pts.length ? si + 2 : pts.length - 1];
                var t = 0.18;
                var cp1x = p1.x + (p2.x - p0.x) * t;
                var cp1y = p1.y + (p2.y - p0.y) * t;
                var cp2x = p2.x - (p3.x - p1.x) * t;
                var cp2y = p2.y - (p3.y - p1.y) * t;
                smoothD += " C " + cp1x.toFixed(1) + " " + cp1y.toFixed(1) + " " + cp2x.toFixed(1) + " " + cp2y.toFixed(1) + " " + p2.x + " " + p2.y;
            }
        }
        // Area path: theo line rồi đóng xuống axis
        var areaD = smoothD;
        if (pts.length > 0) {
            areaD += " L " + pts[pts.length - 1].x + " " + axisY + " L " + pts[0].x + " " + axisY + " Z";
        }
        var dots = "", stockLabels = "";
        for (var pj = 0; pj < N; pj++) {
            var px = pts[pj].x, py = pts[pj].y;
            dots += "<circle class=\"dk-flow-dot\" style=\"animation-delay:" + (pj * 40 + 400) + "ms\" cx=\"" + px + "\" cy=\"" + py + "\" r=\"5\" fill=\"#fff\" stroke=\"" + colorLine + "\" stroke-width=\"2.5\" filter=\"url(#bsf)\"/>";
            stockLabels += "<text class=\"dk-flow-dot-label\" x=\"" + px + "\" y=\"" + (py - 10) + "\" text-anchor=\"middle\" style=\"font-size:10px;font-weight:700;fill:" + colorLine + ";animation-delay:" + (pj * 40 + 600) + "ms\">" + formatNumber(series.stock[pj], 0) + "</text>";
        }
        var stockArea = (pts.length > 1) ? "<path class=\"dk-flow-area\" d=\"" + areaD + "\" fill=\"url(#gradStockArea)\"/>" : "";
        var stockLine = "<path class=\"dk-flow-line\" d=\"" + smoothD + "\" fill=\"none\" stroke=\"" + colorLine + "\" stroke-width=\"3\" stroke-linecap=\"round\" stroke-linejoin=\"round\" style=\"filter:drop-shadow(0 2px 4px rgba(249,115,22,0.35))\"/>";

        var xLabels = series.labels.map(function (lbl, idx) {
            var lx = groupX(idx);
            var ly = axisY + 14;
            return "<text x=\"" + lx + "\" y=\"" + ly + "\" text-anchor=\"middle\" class=\"dk-flow-x-label\" style=\"font-size:10.5px; fill:#5f758b; font-weight:600;\">" + escapeHtml(lbl) + "</text>";
        }).join("");

        var axisTitles = "<text x=\"" + (L - 4) + "\" y=\"" + (T - 10) + "\" text-anchor=\"end\" class=\"dk-flow-axis-title\">Số lượng</text>";

        node.innerHTML =
            "<div class=\"dk-flow-legend\">" +
            "<span class=\"dk-flow-legend-item\"><span class=\"dk-flow-legend-dot\" style=\"background:#5bb8f5\"></span>Nhập</span>" +
            "<span class=\"dk-flow-legend-item\"><span class=\"dk-flow-legend-dot\" style=\"background:#a78bfa\"></span>Xuất</span>" +
            "<span class=\"dk-flow-legend-item\"><span class=\"dk-flow-legend-dot\" style=\"background:" + colorLine + "\"></span>Tồn Kho</span>" +
            "</div>" +
            "<svg viewBox=\"0 0 " + W + " " + H + "\" style=\"width:100%; height:auto; display:block; overflow:visible;\">" +
            defs + axisTitles + grid + barsIn + barsOut + stockArea + stockLine + dots + stockLabels + xLabels +
            "</svg>";
    }

    function renderCharts() {
        renderCapacityChart();
        renderCapacityBarChart();
        renderCustomerPieChart();
        // Flow trend: default 30-day range (Issue 1, v2.3.5). Fall back to
        // old 12-month chart if the date-range picker isn't on the page yet
        // (e.g. demo/test pages).
        var fromEl = byId("flowFromDate"), toEl = byId("flowToDate");
        if (fromEl && toEl) {
            var today = new Date();
            var defStart = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);
            if (!fromEl.value) fromEl.value = asIsoDate(defStart);
            if (!toEl.value) toEl.value = asIsoDate(today);
            var from = new Date(fromEl.value + "T00:00:00");
            var to = new Date(toEl.value + "T00:00:00");
            loadAndRenderFlowByRange(from, to);
        } else {
            renderFlowTrendChart();
        }
        renderTop5MaxChart();
        renderTop5MinChart();
        renderAgeStockChart();
    }



    // ─── Top 5 grouped bar chart (NL + PL) ──────────────────────────────────

    function dkIsDark() {
        return document.body.classList.contains("dark-theme");
    }

    function applyHighchartsTheme() {
        if (typeof Highcharts === "undefined") return;
        var dark = dkIsDark();
        var axisLbl = dark ? "#9fb3d1" : "#475569";
        var axisTitle = dark ? "#cbd5e1" : "#475569";
        var gridLine = dark ? "rgba(96,165,250,0.12)" : "#e5e7eb";
        var dataLbl = dark ? "#e2eaf5" : "#1f2937";
        var legendItm = dark ? "#cbd5e1" : "#1f2937";
        Highcharts.setOptions({
            accessibility: { enabled: false },
            colors: dark
                ? ["#60a5fa", "#34d399", "#fbbf24", "#f87171", "#a78bfa", "#22d3ee", "#f472b6", "#fb923c"]
                : ["#2563eb", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6", "#0ea5e9", "#ec4899", "#f97316"],
            chart: { style: { fontFamily: "'Segoe UI', Tahoma, Arial, sans-serif" }, backgroundColor: "transparent" },
            tooltip: {
                backgroundColor: dark ? "rgba(20, 42, 76, 0.96)" : "#ffffff",
                borderColor: dark ? "#3b82f6" : "#e5e7eb",
                borderRadius: 8,
                borderWidth: 1,
                shadow: { color: dark ? "rgba(0,0,0,0.6)" : "rgba(0,0,0,0.15)", offsetX: 0, offsetY: 2, opacity: 0.4, width: 6 },
                style: { color: dark ? "#f1f5fb" : "#1f2937", fontSize: "12px" }
            },
            xAxis: {
                lineColor: gridLine,
                tickColor: gridLine,
                gridLineColor: gridLine,
                labels: { style: { color: axisLbl, fontSize: "11px" } },
                title: { style: { color: axisTitle, fontSize: "11px" } }
            },
            yAxis: {
                lineColor: gridLine,
                tickColor: gridLine,
                gridLineColor: gridLine,
                labels: { style: { color: axisLbl, fontSize: "11px" } },
                title: { style: { color: axisTitle, fontSize: "11px" } },
                stackLabels: { style: { color: dataLbl, fontWeight: "700", textOutline: "none" } }
            },
            legend: { itemStyle: { color: legendItm, fontSize: "11px" }, itemHoverStyle: { color: dark ? "#f1f5fb" : "#0f172a" } },
            plotOptions: {
                series: {
                    dataLabels: {
                        style: {
                            color: dataLbl,
                            fontWeight: "700",
                            textOutline: dark ? "2px rgba(0,0,0,0.6)" : "2px rgba(255,255,255,0.8)"
                        }
                    }
                },
                pie: {
                    borderColor: dark ? "#0d1d35" : "#ffffff",
                    borderWidth: 2
                }
            }
        });

        // Update all existing charts without reloading data
        if (Highcharts.charts) {
            Highcharts.charts.forEach(function (chart) {
                if (chart) {
                    chart.update({
                        chart: { backgroundColor: "transparent" },
                        xAxis: {
                            lineColor: gridLine,
                            tickColor: gridLine,
                            gridLineColor: gridLine,
                            labels: { style: { color: axisLbl } },
                            title: { style: { color: axisTitle } }
                        },
                        yAxis: {
                            lineColor: gridLine,
                            tickColor: gridLine,
                            gridLineColor: gridLine,
                            labels: { style: { color: axisLbl } },
                            title: { style: { color: axisTitle } }
                        },
                        legend: { itemStyle: { color: legendItm }, itemHoverStyle: { color: dark ? "#f1f5fb" : "#0f172a" } },
                        tooltip: {
                            backgroundColor: dark ? "rgba(20, 42, 76, 0.96)" : "#ffffff",
                            borderColor: dark ? "#3b82f6" : "#e5e7eb",
                            style: { color: dark ? "#f1f5fb" : "#1f2937" }
                        },
                        plotOptions: {
                            series: { dataLabels: { style: { color: dataLbl, textOutline: dark ? "2px rgba(0,0,0,0.6)" : "2px rgba(255,255,255,0.8)" } } },
                            pie: { borderColor: dark ? "#0d1d35" : "#ffffff" }
                        }
                    }, true); // redraw
                }
            });
        }
    }

    function renderTop5Single(nodeId, loaiNPL) {
        var node = byId(nodeId);
        if (!node) return;
        if (typeof Highcharts === "undefined") return;

        function getLabel(d) {
            var code = null;
            if (d.MaVT && String(d.MaVT).trim()) code = String(d.MaVT).trim();
            else if (d.MaNPL) {
                var parts = String(d.MaNPL).split("@");
                for (var p = 0; p < parts.length; p++) {
                    if (parts[p].indexOf("MAVT_") === 0) { code = parts[p]; break; }
                }
                if (!code) code = parts[0] || null;
            }
            var desc = d.TenVT || d.ChiTiet || '';
            if (code) return desc ? (code + ' — ' + desc) : code;
            return desc || "N/A";
        }

        function processRows(rows) {
            var raw = normalizeArray(rows);
            var arr = raw.filter(function (d) { return toNumber(d.TonKho) > 0; });
            arr.sort(function (a, b) { return toNumber(b.TonKho) - toNumber(a.TonKho); });
            var top5 = arr.slice(0, 5);

            // Lưu raw để modal chi tiết hiện 15 dòng
            if (loaiNPL === 1) state.top15MaxNL = raw;
            else state.top15MaxPL = raw;

            if (top5.length === 0) {
                node.innerHTML = "<div class=\"dk-empty\">Không có dữ liệu</div>";
                return;
            }

            var cats = top5.map(getLabel);
            var data = top5.map(function (d) { return toNumber(d.TonKho); });
            var seriesName = loaiNPL === 1 ? "Nguyên liệu" : "Phụ liệu";
            var color = loaiNPL === 1 ? "#2563eb" : "#f97316";

            node.innerHTML = "";
            var dark = dkIsDark();
            var lblColor = dark ? "#cbd5e1" : "#1e3a5f";
            var axisColor = dark ? "#9fb3d1" : "#6b7280";
            var gridColor = dark ? "rgba(96, 165, 250, 0.15)" : "#e5e7eb";
            var dlColor = dark ? "#e2eaf5" : "#1f2937";
            Highcharts.chart(node, {
                chart: {
                    type: "bar", backgroundColor: "transparent",
                    style: { fontFamily: "'Segoe UI', Tahoma, Arial, sans-serif" }
                },
                title: { text: null },
                xAxis: {
                    categories: cats,
                    lineColor: gridColor,
                    tickColor: gridColor,
                    labels: { style: { fontSize: "11px", fontWeight: "600", color: lblColor } }
                },
                yAxis: {
                    min: 0,
                    title: { text: "Số lượng tồn", style: { color: axisColor } },
                    labels: { style: { color: axisColor } },
                    gridLineDashStyle: "ShortDash",
                    gridLineColor: gridColor
                },
                legend: { enabled: false },
                tooltip: { pointFormat: seriesName + ": <b>{point.y:,.0f}</b><br/>" },
                plotOptions: {
                    bar: {
                        borderRadius: 3,
                        dataLabels: {
                            enabled: true,
                            style: { fontWeight: "700", fontSize: "11px", color: dlColor, textOutline: dark ? "1px rgba(0,0,0,0.6)" : "1px rgba(255,255,255,0.6)" },
                            formatter: function () { return Highcharts.numberFormat(this.y, 0); }
                        }
                    }
                },
                series: [{ name: seriesName, data: data, color: color }],
                credits: { enabled: false }
            });
        }

        var cached = loaiNPL === 1 ? state.top15MaxNL : state.top15MaxPL;
        if (cached && cached.length > 0) {
            processRows(cached);
        } else {
            node.innerHTML = "<div class=\"dk-empty\">Đang tải...</div>";
            requestJson("/api/DashboardKhoDesktop/GetTop5?isNhieuNhat=1&loaiNPL=" + loaiNPL).then(function (rows) {
                processRows(rows);
            }).catch(function () {
                node.innerHTML = "<div class=\"dk-empty\">Lỗi tải dữ liệu</div>";
            });
        }
    }

    function renderTop5MaxNLChart() { renderTop5Single("chartTop5MaxNL", 1); }
    function renderTop5MaxPLChart() { renderTop5Single("chartTop5MaxPL", 2); }

    function renderTop5MaxChart() { renderTop5MaxNLChart(); renderTop5MaxPLChart(); }
    function renderTop5MinChart() { /* removed in v2.3.18 */ }

    function renderAgeStockChart() {
        var node = byId(ids.chartAgeStock);
        if (!node) return;
        if (typeof Highcharts === "undefined") return;
        if (!state.ageStock || state.ageStock.length === 0) {
            node.innerHTML = "<div class=\"dk-empty\">Không có dữ liệu tuổi tồn kho</div>";
            return;
        }

        var buckets = [
            { label: "< 3 tháng", color: "#22c55e", min: 0, max: 2 },
            { label: "3-6 tháng", color: "#eab308", min: 3, max: 5 },
            { label: "6-12 tháng", color: "#f97316", min: 6, max: 11 },
            { label: "> 12 tháng", color: "#ef4444", min: 12, max: 9999 }
        ];

        var groups = {};
        var groupOrder = [];
        for (var ai = 0; ai < state.ageStock.length; ai++) {
            var ag = state.ageStock[ai];
            var nhom = ag.TenNhom || "Khác";
            var thang = toNumber(ag.Thang);
            var qty = toNumber(ag.TonKhoCT);
            if (!groups[nhom]) { groups[nhom] = {}; groupOrder.push(nhom); }
            var bLabel = null;
            for (var bi = 0; bi < buckets.length; bi++) {
                if (thang >= buckets[bi].min && thang <= buckets[bi].max) { bLabel = buckets[bi].label; break; }
            }
            if (!bLabel) bLabel = "> 12 tháng";
            groups[nhom][bLabel] = (groups[nhom][bLabel] || 0) + qty;
        }

        var uniqueGroups = [];
        var seen = {};
        for (var gi = 0; gi < groupOrder.length; gi++) {
            if (!seen[groupOrder[gi]]) { uniqueGroups.push(groupOrder[gi]); seen[groupOrder[gi]] = true; }
        }

        var series = buckets.map(function (b) {
            return {
                name: b.label,
                color: b.color,
                data: uniqueGroups.map(function (g) { return groups[g][b.label] || 0; })
            };
        });

        node.innerHTML = "";
        applyHighchartsTheme();
        Highcharts.chart(node, {
            chart: { type: "column", backgroundColor: "transparent" },
            title: { text: null },
            xAxis: { categories: uniqueGroups },
            yAxis: { min: 0, title: { text: "Số lượng" }, stackLabels: { enabled: true }, gridLineDashStyle: "ShortDash" },
            tooltip: {
                useHTML: true,
                backgroundColor: "#fff",
                borderRadius: 10,
                shadow: true,
                formatter: function () {
                    var nhom = this.x;
                    var bLabel = this.series.name;
                    // Find individual months in this bucket for this nhom
                    var monthDetails = [];
                    for (var _ai = 0; _ai < state.ageStock.length; _ai++) {
                        var _r = state.ageStock[_ai];
                        if ((_r.TenNhom || "Khác") !== nhom) continue;
                        var _t = toNumber(_r.Thang);
                        var _b = null;
                        for (var _bi = 0; _bi < buckets.length; _bi++) {
                            if (_t >= buckets[_bi].min && _t <= buckets[_bi].max) { _b = buckets[_bi].label; break; }
                        }
                        if (!_b) _b = "> 12 tháng";
                        if (_b !== bLabel) continue;
                        monthDetails.push({ thang: _t, qty: toNumber(_r.TonKhoCT) });
                    }
                    monthDetails.sort(function (a, b) { return a.thang - b.thang; });
                    var rows = monthDetails.map(function (m) {
                        return "<tr><td style='padding:1px 6px;color:#64748b;font-size:10px;'>Tháng " + m.thang + "</td><td style='padding:1px 6px;font-weight:700;text-align:right;'>" + formatNumber(m.qty, 0) + "</td></tr>";
                    }).join("");
                    return "<div style='font-size:11px;min-width:160px;'>" +
                        "<div style='font-weight:800;color:#1e3a5f;margin-bottom:4px;'>" + escapeHtml(nhom) + "</div>" +
                        "<div style='color:" + this.series.color + ";font-weight:700;margin-bottom:4px;'>" + bLabel + ": <b>" + formatNumber(this.y, 0) + "</b></div>" +
                        (rows ? "<table style='border-collapse:collapse;width:100%;'><thead><tr><th style='font-size:9px;color:#94a3b8;text-align:left;padding:0 6px;'>Tháng</th><th style='font-size:9px;color:#94a3b8;text-align:right;padding:0 6px;'>SL</th></tr></thead><tbody>" + rows + "</tbody></table>" : "") +
                        "</div>";
                }
            },
            plotOptions: { column: { stacking: "normal", borderRadius: 3, borderWidth: 0 } },
            legend: { enabled: true, itemStyle: { fontSize: "11px", fontWeight: "600" } },
            series: series,
            credits: { enabled: false }
        });
    }

    // ─── Feature 7: MoM Comparison Strip ────────────────────────────────────────
    function renderMoMStrip() {
        var strip = byId("momStrip");
        if (!strip) return;
        var mom = state.momComparison || [];
        var thisRow = null, lastRow = null;
        for (var mi = 0; mi < mom.length; mi++) {
            var k = String(mom[mi].KieuKy || mom[mi].kieuKy || "").trim().toLowerCase();
            if (k === "thismonth") thisRow = mom[mi];
            else if (k === "lastmonth") lastRow = mom[mi];
        }
        if (!thisRow && !lastRow) {
            var now = new Date();
            var lastM = new Date(now.getFullYear(), now.getMonth() - 1, 1);
            thisRow = { Nam: now.getFullYear(), Thang: now.getMonth() + 1, TotalIn: 0, TotalOut: 0 };
            lastRow = { Nam: lastM.getFullYear(), Thang: lastM.getMonth() + 1, TotalIn: 0, TotalOut: 0 };
        }
        var curIn = thisRow ? toNumber(thisRow.TotalIn) : 0;
        var curOut = thisRow ? toNumber(thisRow.TotalOut) : 0;
        var prvIn = lastRow ? toNumber(lastRow.TotalIn) : 0;
        var prvOut = lastRow ? toNumber(lastRow.TotalOut) : 0;
        var curMonth = thisRow ? (thisRow.Thang + "/" + thisRow.Nam) : "";
        var prvMonth = lastRow ? (lastRow.Thang + "/" + lastRow.Nam) : "";

        function deltaHtml(cur, prv) {
            if (prv <= 0) return "";
            var pct = ((cur - prv) / prv) * 100;
            var cls = pct > 2 ? "up" : (pct < -2 ? "down" : "flat");
            var arrow = pct > 2 ? "▲" : (pct < -2 ? "▼" : "→");
            return "<span class=\"dk-mom-delta " + cls + "\">" + arrow + " " + formatNumber(Math.abs(pct), 1) + "%</span>";
        }

        strip.innerHTML =
            "<div class=\"dk-mom-block dk-mom-in\">" +
            "  <span class=\"dk-mom-icon\">📥</span>" +
            "  <div><div class=\"dk-mom-label\">Nhập kho — " + escapeHtml(curMonth) + "</div>" +
            "    <div class=\"dk-mom-values\">" +
            "      <span class=\"dk-mom-cur\">" + formatNumber(curIn, 0) + "</span>" +
            "      <span class=\"dk-mom-prev\">Tháng trước: " + formatNumber(prvIn, 0) + "</span>" +
            "      " + deltaHtml(curIn, prvIn) +
            "    </div></div></div>" +
            "<div class=\"dk-mom-block dk-mom-out\">" +
            "  <span class=\"dk-mom-icon\">📤</span>" +
            "  <div><div class=\"dk-mom-label\">Xuất kho — " + escapeHtml(curMonth) + "</div>" +
            "    <div class=\"dk-mom-values\">" +
            "      <span class=\"dk-mom-cur\">" + formatNumber(curOut, 0) + "</span>" +
            "      <span class=\"dk-mom-prev\">Tháng trước: " + formatNumber(prvOut, 0) + "</span>" +
            "      " + deltaHtml(curOut, prvOut) +
            "    </div></div></div>";
    }

    // ─── Feature 11: Populate customer filter dropdown ──────────────────────────
    function populateCustomerFilter() {
        var sel = byId("customerFilterSelect");
        if (!sel) return;
        var prev = activeCustomerFilter;
        // Rebuild options
        var opts = "<option value=\"\"" + (prev === "" ? " selected" : "") + ">Tất cả khách hàng</option>";
        var rows = buildCustomerDetailRows();
        for (var ci = 0; ci < rows.length; ci++) {
            var r = rows[ci];
            var v = r.MaKH || "";
            if (!v) continue; // bỏ qua khách không có mã để tránh trùng với "Tất cả"
            var lbl = escapeHtml((r.TenKH || r.MaKH || "").substring(0, 28));
            var sel2 = (v && v === prev) ? " selected" : "";
            opts += "<option value=\"" + escapeHtml(v) + "\"" + sel2 + ">" + lbl + "</option>";
        }
        sel.innerHTML = opts;
    }

    function renderActivityCalendar() {
        var node = byId("chartActivityCalendar");
        if (!node) return;
        var data = state.activityCalendar || [];
        var nWeeks = activityWeeksCount || 13;

        if (data.length === 0) {
            node.innerHTML = "<div class=\"dk-empty\">Không có dữ liệu hoạt động</div>";
            return;
        }

        var dayMap = {};
        var maxActivity = 0;
        for (var di = 0; di < data.length; di++) {
            var d = data[di];
            var key = String(d.NgayHoatDong || "").substring(0, 10);
            var val = toNumber(d.TotalActivity);
            dayMap[key] = { totalIn: toNumber(d.TotalIn), totalOut: toNumber(d.TotalOut), total: val };
            if (val > maxActivity) maxActivity = val;
        }

        function actColor(val) {
            if (val <= 0) return "#ebedf0";
            var r = val / maxActivity;
            if (r < 0.2) return "#9be9a8";
            if (r < 0.45) return "#40c463";
            if (r < 0.7) return "#30a14e";
            return "#216e39";
        }

        function isoDateKey(d) {
            return d.getFullYear() + "-" +
                String(d.getMonth() + 1).padStart(2, "0") + "-" +
                String(d.getDate()).padStart(2, "0");
        }

        var today = new Date();
        today.setHours(0, 0, 0, 0);

        var endDay = new Date(today);
        var todayDow = (endDay.getDay() + 6) % 7;
        endDay.setDate(endDay.getDate() + (6 - todayDow));

        var startDay = new Date(endDay);
        startDay.setDate(startDay.getDate() - (nWeeks * 7 - 1));
        var startDow = (startDay.getDay() + 6) % 7;
        startDay.setDate(startDay.getDate() - startDow);

        var weeks = [];
        var cur = new Date(startDay);
        var maxWeeks = nWeeks + 2; // safety cap
        while (cur <= endDay && weeks.length < maxWeeks) {
            var week = [];
            for (var wd = 0; wd < 7; wd++) {
                var dateKey = isoDateKey(cur);
                var info = dayMap[dateKey] || { totalIn: 0, totalOut: 0, total: 0 };
                var isFuture = cur > today;
                week.push({ key: dateKey, info: info, isFuture: isFuture });
                cur.setDate(cur.getDate() + 1);
            }
            weeks.push(week);
        }

        var DAY_LABELS = ["T2", "", "T4", "", "T6", "", "CN"];

        var monthRowHtml = "<div class=\"dk-cal-month-row\">";
        var lastMonth = -1;
        for (var wi = 0; wi < weeks.length; wi++) {
            var firstDay = new Date(weeks[wi][0].key);
            var m = firstDay.getMonth();
            var lbl = m !== lastMonth ? ((m + 1) + "/" + String(firstDay.getFullYear()).slice(-2)) : "";
            if (m !== lastMonth) lastMonth = m;
            monthRowHtml += "<div class=\"dk-cal-month-slot\" style=\"" + (lbl ? "font-weight:800;color:#334155;" : "") + "\">" + lbl + "</div>";
        }
        monthRowHtml += "</div>";

        // Day-label column
        var dayLabelHtml = "<div class=\"dk-cal-day-labels\">";
        for (var dl = 0; dl < DAY_LABELS.length; dl++) {
            dayLabelHtml += "<div class=\"dk-cal-day-label\">" + DAY_LABELS[dl] + "</div>";
        }
        dayLabelHtml += "</div>";

        // Week columns
        var weeksHtml = "";
        for (var wi2 = 0; wi2 < weeks.length; wi2++) {
            weeksHtml += "<div class=\"dk-cal-week\">";
            for (var wd2 = 0; wd2 < weeks[wi2].length; wd2++) {
                var cell = weeks[wi2][wd2];
                var bg = cell.isFuture ? "transparent" : actColor(cell.info.total);
                var border = cell.isFuture ? "1px dashed #e2e8f0" : "none";
                var ttip = cell.isFuture ? "" : (cell.key + "  Nhập: " + formatNumber(cell.info.totalIn, 0) + "  Xuất: " + formatNumber(cell.info.totalOut, 0) + "  Tổng: " + formatNumber(cell.info.total, 0));
                weeksHtml += "<div class=\"dk-cal-cell\" style=\"background:" + bg + ";border:" + border + ";\" title=\"" + escapeHtml(ttip) + "\"></div>";
            }
            weeksHtml += "</div>";
        }

        node.innerHTML =
            monthRowHtml +
            "<div class=\"dk-cal-outer\">" +
            dayLabelHtml +
            "<div class=\"dk-cal-wrap\">" + weeksHtml + "</div>" +
            "</div>";
    }

    // v2.3.23 — Fetch NK dự kiến từ ERP_NhapKhoNPL (full range, không bị giới hạn 14 ngày như GetChuanBiVe)
    function loadNKDuKienForCalendar(callback) {
        if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
        var from = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() - 1, 1);
        var to = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 2, 0);
        var url = "/api/DashboardKhoDesktop/GetNKDuKienByRange?tuNgay=" +
            asIsoDate(from) + "&denNgay=" + asIsoDate(to);
        requestJson(url).then(function (data) {
            state.nkDuKien = normalizeArray(data);
            if (callback) callback();
        }).catch(function () {
            state.nkDuKien = [];
            if (callback) callback();
        });
    }

    function renderActivityCalendarMonthly() {

        console.log("Dữ liệu LPCP hiện tại:", state.lpcpCalendar);
        var node = byId("chartActivityCalendarMonthly");
        if (!node) return;
        var data = state.activityCalendar || [];

        // Init calMonthDate to current month if not set
        if (!calMonthDate) {
            var now = new Date();
            calMonthDate = new Date(now.getFullYear(), now.getMonth(), 1);
        }

        // Update month title
        var titleEl = byId("calMonthTitle");
        if (titleEl) {
            var monthNames = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
            if (calRangeFrom && calRangeTo) {
                titleEl.textContent = "Từ: " + formatDateShort(calRangeFrom) + " - " + formatDateShort(calRangeTo);
            } else {
                titleEl.textContent = monthNames[calMonthDate.getMonth()] + " " + calMonthDate.getFullYear();
            }
        }

        // Build dayMap from state.activityCalendar
        var dayMap = {};
        for (var di = 0; di < data.length; di++) {
            var d = data[di];
            var key = String(d.NgayHoatDong || d.ngayHoatDong || "").substring(0, 10);
            dayMap[key] = {
                totalIn: toNumber(d.TotalIn || d.totalIn),
                totalOut: toNumber(d.TotalOut || d.totalOut),
                totalKK: toNumber(d.TotalKiemKe || d.totalKiemKe),
                total: toNumber(d.TotalActivity || d.totalActivity)
            };
        }

        // Build planned inbound dates
        var plannedDates = {};
        var plannedSource = (state.nkDuKien && state.nkDuKien.length > 0) ? state.nkDuKien : state.inbound;
        for (var pi = 0; pi < plannedSource.length; pi++) {
            var pDate = parseDate(plannedSource[pi].NgayNKDuKien || plannedSource[pi].ngayNKDuKien);
            if (pDate) {
                var pKey = pDate.getFullYear() + "-" +
                    String(pDate.getMonth() + 1).padStart(2, "0") + "-" +
                    String(pDate.getDate()).padStart(2, "0");
                plannedDates[pKey] = (plannedDates[pKey] || 0) + 1;
            }
        }

        // Determine display range
        var dispFrom, dispTo;
        if (calRangeFrom && calRangeTo) {
            dispFrom = new Date(calRangeFrom);
            dispTo = new Date(calRangeTo);
        } else {
            dispFrom = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth(), 1);
            dispTo = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 1, 0);
        }

        function isoKey(dt) {
            return dt.getFullYear() + "-" +
                String(dt.getMonth() + 1).padStart(2, "0") + "-" +
                String(dt.getDate()).padStart(2, "0");
        }

        // Build calendar grid for the display range
        var gridStart = new Date(dispFrom);
        var dow = (gridStart.getDay() + 6) % 7; // 0=Mon
        gridStart.setDate(gridStart.getDate() - dow);

        var gridEnd = new Date(dispTo);
        var dow2 = (gridEnd.getDay() + 6) % 7;
        gridEnd.setDate(gridEnd.getDate() + (6 - dow2));

        var today = new Date(); today.setHours(0, 0, 0, 0);
        var DAY_NAMES = ["Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy", "Chủ Nhật"];

        var html = "";

        // Day-of-week header
        html += "<div class=\"dk-cal-monthly-header\">";
        for (var dh = 0; dh < 7; dh++) {
            html += "<div class=\"dk-cal-monthly-dow\">" + DAY_NAMES[dh] + "</div>";
        }
        html += "</div>";

        // Weeks
        html += "<div class=\"dk-cal-monthly-grid\">";
        var cur = new Date(gridStart);
        while (cur <= gridEnd) {
            for (var wd = 0; wd < 7; wd++) {
                var dk = isoKey(cur);
                var isCurrentMonth = (cur.getMonth() === calMonthDate.getMonth() && cur.getFullYear() === calMonthDate.getFullYear());
                var isFuture = cur > today;
                var isToday = isoKey(cur) === isoKey(today);
                var info = dayMap[dk] || { totalIn: 0, totalOut: 0, totalKK: 0, total: 0 };
                var planned = plannedDates[dk] || 0;

                var cellClass = "dk-cal-monthly-cell";
                var inlineStyle = "";
                var isHidden = false;

                if (calRangeFrom && calRangeTo) {
                    var curTime = cur.getTime();
                    if (curTime < calRangeFrom.getTime() || curTime > calRangeTo.getTime()) {
                        cellClass += " dk-cal-dimmed dk-cal-out-month";
                        inlineStyle += "visibility: hidden;";
                    } else if (calRangeFrom.getMonth() !== calRangeTo.getMonth() || calRangeFrom.getFullYear() !== calRangeTo.getFullYear()) {
                        cellClass += " dk-cal-month-" + cur.getMonth();
                    }
                } else {
                    if (!isCurrentMonth) cellClass += " dk-cal-out-month";
                }

                if (isToday) cellClass += " dk-cal-today";
                if (isFuture && !planned) cellClass += " dk-cal-future";

                var actFilter = state.calActFilter || { in: true, out: true, kk: true, plan: true };

                function formatShort(n) {
                    n = toNumber(n);
                    if (n === 0) return "0";
                    if (n >= 1000000) return (n / 1000000).toFixed(1).replace(/\.0$/, "") + "M";
                    if (n >= 1000) return (n / 1000).toFixed(1).replace(/\.0$/, "") + "K";
                    return formatNumber(n, 0);
                }

                var inVal = toNumber(info.totalIn);
                var outVal = toNumber(info.totalOut);
                var kkVal = toNumber(info.totalKK);
                var planVal = toNumber(planned);

                // ==========================================
                // XỬ LÝ GIAO DIỆN LPCP
                // ==========================================
                var lpcpDay = state.lpcpCalendar ? state.lpcpCalendar[dk] : null;
                var warnCount = 0, taskCount = 0, pickCount = 0;
                if (lpcpDay) {
                    var lTasks = lpcpDay.Tasks || lpcpDay.tasks || [];
                    taskCount = lTasks.length;
                    lTasks.forEach(function (t) { if (t.ThieuNPL || t.thieuNPL || t.TrangThai === 3) warnCount++; });
                    if (lpcpDay.ThieuNPL || lpcpDay.thieuNPL) warnCount = Math.max(warnCount, 1);
                    var uniqueLenh = {};
                    lTasks.forEach(function (t) { if (t.MaLenhSX || t.maLenhSX) uniqueLenh[t.MaLenhSX || t.maLenhSX] = 1; });
                    pickCount = Object.keys(uniqueLenh).length || taskCount;
                }

                var iconIn = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>';
                var iconOut = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="17 8 12 3 7 8"/><line x1="12" y1="3" x2="12" y2="15"/></svg>';
                var iconKK = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="9" y="3" width="6" height="4" rx="1"/><path d="M9 5H6a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-3M9 14l2 2 4-4"/></svg>';
                var iconPlan = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/><rect x="10.5" y="14.5" width="3" height="3"/></svg>';
                var iconWarn = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>';
                var iconTask = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="8" r="4" /><path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" /></svg>';
                var iconPick = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z" /></svg>';

                function buildSlot(key, icon, val, label, visible) {
                    if (!visible || !val || val === 0 || val === "0") return "";
                    var title = label + ": " + (key === "plan" ? val + " l\u00f4" : formatNumber(val, 0));
                    return '<div class="dk-cal-act dk-cal-act-' + key + '" title="' + escapeHtml(title) + '" data-act="' + key + '">' +
                        '<span class="dk-cal-act-icon">' + icon + '</span>' +
                        '<span class="dk-cal-act-val">' + formatShort(val) + '</span>' +
                        '</div>';
                }

                var visibleCount = 0;
                if (actFilter.in) visibleCount++;
                if (actFilter.out) visibleCount++;
                if (actFilter.kk) visibleCount++;
                if (actFilter.plan) visibleCount++;

                cellClass += " dk-cal-slots-" + visibleCount;

                var slots = "";
                var actListLeft = "";
                if (actFilter.in) actListLeft += buildSlot("in", iconIn, inVal, "Nh\u1eadp kho", true);
                if (actFilter.out) actListLeft += buildSlot("out", iconOut, outVal, "Xu\u1ea5t kho", true);
                if (actFilter.kk) actListLeft += buildSlot("kk", iconKK, kkVal, "Ki\u1ec3m k\u00ea", true);
                if (actFilter.plan) actListLeft += buildSlot("plan", iconPlan, planVal, "NK d\u1ef1 ki\u1ebfn", true);

                var actListRight = "";
                actListRight += buildSlot("warn", iconWarn, warnCount, "C\u1ea3nh b\u00e1o", true);
                actListRight += buildSlot("task", iconTask, taskCount, "Task ph\u00e2n c\u00f4ng", true);
                actListRight += buildSlot("pick", iconPick, pickCount, "L\u1ec7nh so\u1ea1n h\u00e0ng", true);

                if (actListLeft || actListRight) {
                    slots = '<div class="dk-cal-acts-container" style="display: flex; justify-content: center; gap: 12px; padding: 4px 2px 0;">' +
                        '<div class="dk-cal-acts-left" style="display: flex; flex-direction: column; gap: 4px;">' + actListLeft + '</div>' +
                        (actListRight ? '<div class="dk-cal-acts-right" style="display: flex; flex-direction: column; gap: 4px;">' + actListRight + '</div>' : '') +
                        '</div>';
                } else {
                    slots = '<div class="dk-cal-acts dk-cal-acts-empty"></div>';
                }

                var tooltip = dk + " Nh\u1eadp: " + formatNumber(inVal, 0) +
                    " Xu\u1ea5t: " + formatNumber(outVal, 0) +
                    (kkVal ? " KK: " + formatNumber(kkVal, 0) : "") +
                    (planVal ? " NK DK: " + planVal + " l\u00f4" : "");

                // Gộp chung 1 cell top
                var cellTop = "<div class=\"dk-cell-top\" style=\"width: 100%; display: flex; flex-direction: column; align-items: center;\">" +
                    "<div class=\"dk-cal-day-num\" style=\"font-size: 22px; text-align: center; width: 100%; margin: 0 auto;\">" + cur.getDate() + "</div>" +
                    slots +
                    "</div>";

                html += "<div class=\"" + cellClass + "\" style=\"" + inlineStyle + "; justify-content: center;\" data-date=\"" + dk + "\" title=\"" + escapeHtml(tooltip) + "\">" +
                    (isHidden ? "" : cellTop) +
                    "</div>";

                cur.setDate(cur.getDate() + 1);
            }
        }
        html += "</div>";

        node.innerHTML = html;

        // Bind day click: phần trên (bars) → modal, phần dưới (tasks) → sidebar
        var cells = node.querySelectorAll(".dk-cal-monthly-cell[data-date]");
        for (var ci = 0; ci < cells.length; ci++) {
            (function (cell) {
                var dt = cell.getAttribute("data-date");
                if (!dt) return;
                var cellInfo = dayMap[dt] || { totalIn: 0, totalOut: 0, totalKK: 0, total: 0 };
                var plCount = plannedDates[dt] || 0;

                // Chỉ cần bind 1 sự kiện click duy nhất cho toàn bộ ô lịch (gộp lại)
                cell.style.cursor = "pointer";
                cell.addEventListener("click", function () {
                    // Luôn cho phép click mở Modal để người dùng không tưởng là bị lỗi (unclickable)
                    showCalDayDetail(dt, cellInfo, plCount);

                    // Luôn cập nhật Sidebar (phần dưới)
                    if (currentPage === 3) {
                        showLpcpInlineDetail(dt);
                    }
                });
            })(cells[ci]);
        }
    }

    function openCalendarOverviewModal() {
        var fromD, toD, titleSuffix;
        if (calRangeFrom && calRangeTo) {
            fromD = calRangeFrom; toD = calRangeTo;
            titleSuffix = "từ " + formatDateShort(fromD) + " → " + formatDateShort(toD);
        } else {
            // fallback: tháng đang hiển thị
            var base = calMonthDate || new Date(new Date().getFullYear(), new Date().getMonth(), 1);
            fromD = new Date(base.getFullYear(), base.getMonth(), 1);
            toD = new Date(base.getFullYear(), base.getMonth() + 1, 0);
            titleSuffix = "tháng " + (base.getMonth() + 1) + "/" + base.getFullYear();
        }

        var modalTitle = byId(ids.detailModalTitle);
        var modalMeta = byId(ids.detailModalMeta);
        var modalContent = byId(ids.detailModalContent);
        var modal = byId(ids.detailModal);
        if (!modal) return;

        if (modalTitle) modalTitle.textContent = "Tổng quát hoạt động kho " + titleSuffix;
        if (modalMeta) modalMeta.innerHTML = "<span style=\"color:#64748b;font-size:13px\">Đang tổng hợp...</span>";

        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "";
        var searchInputEl = byId("detailSearchInput");
        if (searchInputEl) { searchInputEl.value = ""; searchInputEl.placeholder = "Tìm trong tab đang hiện..."; }
        var searchCountEl = byId("detailSearchCount");
        if (searchCountEl) searchCountEl.textContent = "";

        if (modalContent) modalContent.innerHTML =
            "<div class=\"dk-empty\" style=\"padding:30px\">Đang tải dữ liệu tổng quát...</div>";
        modal.classList.add("open");

        var url = "/api/DashboardKhoDesktop/GetActivityRangeDetail?tuNgay=" +
            asIsoDate(fromD) + "&denNgay=" + asIsoDate(toD);

        requestJson(url).then(function (data) {
            var d = data || {};
            var nhapRows = normalizeArray(d.Nhap);
            var xuatRows = normalizeArray(d.Xuat);
            var kiemKeRows = normalizeArray(d.KiemKe);

            for (var i = 0; i < xuatRows.length; i++) {
                if (!xuatRows[i].NgayXuat) xuatRows[i].NgayXuat = xuatRows[i].NgayXuatDen || xuatRows[i].NgayXuatTu || "";
            }
            for (var i = 0; i < kiemKeRows.length; i++) {
                if (!kiemKeRows[i].NgayKiemKe) kiemKeRows[i].NgayKiemKe = kiemKeRows[i].NgayKKDen || kiemKeRows[i].NgayKKTu || "";
            }

            // NK dự kiến: lọc state.nkDuKien theo range
            var nkSrc = (state.nkDuKien && state.nkDuKien.length > 0) ? state.nkDuKien : (state.inbound || []);
            var plannedRows = nkSrc.filter(function (r) {
                var dt = parseDate(r.NgayNKDuKien);
                if (!dt) return false;
                return dt >= fromD && dt <= toD;
            });

            // Tính tổng cho header meta
            var sumIn = 0, sumOut = 0, sumKK = 0;
            for (var i = 0; i < nhapRows.length; i++) sumIn += toNumber(nhapRows[i].SoLuong);
            for (var j = 0; j < xuatRows.length; j++) sumOut += toNumber(xuatRows[j].SoLuong);
            for (var k = 0; k < kiemKeRows.length; k++) sumKK += toNumber(kiemKeRows[k].SoLuong);

            if (modalMeta) {
                modalMeta.innerHTML =
                    "<span style=\"color:#3b82f6;font-weight:700\">Nhập: " + formatNumber(sumIn, 0) + "</span>" +
                    " &nbsp;·&nbsp; <span style=\"color:#f97316;font-weight:700\">Xuất: " + formatNumber(sumOut, 0) + "</span>" +
                    " &nbsp;·&nbsp; <span style=\"color:#10b981;font-weight:700\">Kiểm kê: " + formatNumber(sumKK, 0) + "</span>" +
                    " &nbsp;·&nbsp; <span style=\"color:#8b5cf6;font-weight:700\">NK dự kiến: " + plannedRows.length + " lô</span>";
            }

            renderOverviewDetailTabs(modalContent, nhapRows, xuatRows, kiemKeRows, plannedRows);
        }).catch(function (err) {
            if (modalContent) {
                modalContent.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px;color:#dc2626\">" +
                    "Lỗi tải tổng quát: " + escapeHtml(String(err && err.message || err)) + "</div>";
            }
        });
    }


    // v2.3.46 — Render kết quả global search Itemcode
    function renderGlobalSearchResult(container, code, row) {
        if (!container) return;
        if (!row || !row.MaVTID) {
            container.innerHTML = "<div class=\"dk-empty\" style=\"padding:14px\">" +
                "Không tìm thấy Itemcode <b>" + escapeHtml(code) + "</b> trong bảng vật tư.</div>";
            return;
        }
        var sections = [
            { key: "ton", label: "Tồn kho", page: 1, rows: toNumber(row.TonRows), sl: toNumber(row.TonSL), color: "#0ea5e9", goto: "capacitySummary" },
            { key: "nhap", label: "Nhập kho (30N)", page: 2, rows: toNumber(row.NhapRows), sl: toNumber(row.NhapSL), color: "#3b82f6", goto: null },
            { key: "xuat", label: "Xuất kho (30N)", page: 2, rows: toNumber(row.XuatRows), sl: toNumber(row.XuatSL), color: "#f97316", goto: "outboundRunning" },
            { key: "kk", label: "Kiểm kê (30N)", page: 3, rows: toNumber(row.KKRows), sl: toNumber(row.KKSL), color: "#10b981", goto: null },
            { key: "dk", label: "NK dự kiến (60N)", page: 3, rows: toNumber(row.DKRows), sl: 0, color: "#8b5cf6", goto: "inboundReady" }
        ];

        var html = "<div class=\"dk-gs-header\">" +
            "<span class=\"dk-gs-found\">Tìm thấy: <b>" + escapeHtml(code) + "</b>" +
            (row.TenVT ? " — <span style=\"color:#475569\">" + escapeHtml(String(row.TenVT)) + "</span>" : "") +
            "</span></div>";
        html += "<div class=\"dk-gs-grid\">";
        for (var i = 0; i < sections.length; i++) {
            var s = sections[i];
            var hasData = s.rows > 0;
            var clickable = hasData;
            html += "<div class=\"dk-gs-card" + (hasData ? " has-data" : " no-data") + (clickable ? " clickable" : "") + "\" " +
                "data-page=\"" + s.page + "\" data-goto=\"" + (s.goto || "") + "\" " +
                "style=\"--gs-color:" + s.color + "\">" +
                "<div class=\"dk-gs-card-head\"><span class=\"dk-gs-dot\" style=\"background:" + s.color + "\"></span>" +
                escapeHtml(s.label) + "</div>" +
                "<div class=\"dk-gs-card-rows\">" + formatNumber(s.rows, 0) + " bản ghi</div>" +
                (s.sl > 0 ? "<div class=\"dk-gs-card-sl\">SL: " + formatNumber(s.sl, 2) + "</div>" : "") +
                (clickable ? "<div class=\"dk-gs-card-go\">Bấm để xem →</div>" : "") +
                "</div>";
        }
        html += "</div>";
        container.innerHTML = html;

        // Bind click → switch page + open detail
        var cards = container.querySelectorAll(".dk-gs-card.clickable");
        for (var c = 0; c < cards.length; c++) {
            cards[c].addEventListener("click", function () {
                var page = this.getAttribute("data-page");
                var go = this.getAttribute("data-goto");
                // Switch page (sidebar nav)
                if (page) {
                    var nav = document.querySelector(".dk-page-btn[data-page=\"" + page + "\"]");
                    if (nav) nav.click();
                }
                // Open detail modal if specified
                if (go) {
                    setTimeout(function () {
                        var trigger = document.querySelector(".js-open-detail[data-detail=\"" + go + "\"]");
                        if (trigger) trigger.click();
                    }, 250);
                }
            });
        }
    }

    // v2.3.7 — modal chi tiết hoạt động ngày với 4 tab record-level data
    function showCalDayDetail(dateKey, info, plannedCount) {
        var modalTitle = byId(ids.detailModalTitle);
        var modalMeta = byId(ids.detailModalMeta);
        var modalContent = byId(ids.detailModalContent);
        var modal = byId(ids.detailModal);
        if (!modal) return;

        // Format dateKey for display (yyyy-MM-dd → dd/MM/yyyy)
        var displayDate = dateKey;
        var parts = String(dateKey).split("-");
        if (parts.length === 3) displayDate = parts[2] + "/" + parts[1] + "/" + parts[0];

        if (modalTitle) modalTitle.textContent = "Hoạt động kho ngày " + displayDate;
        if (modalMeta) {
            modalMeta.innerHTML =
                "<span style=\"color:#3b82f6;font-weight:700\">Nhập: " + formatNumber(info.totalIn, 0) + "</span>" +
                " &nbsp;·&nbsp; <span style=\"color:#f97316;font-weight:700\">Xuất: " + formatNumber(info.totalOut, 0) + "</span>" +
                " &nbsp;·&nbsp; <span style=\"color:#10b981;font-weight:700\">Kiểm kê: " + formatNumber(info.totalKK, 0) + "</span>" +
                " &nbsp;·&nbsp; <span style=\"color:#8b5cf6;font-weight:700\">NK dự kiến: " + plannedCount + " lô</span>";
        }

        // v2.3.9 — KEEP search bar visible so user có thể lọc record trong tab đang hiện
        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "";
        var searchInputEl = byId("detailSearchInput");
        if (searchInputEl) {
            searchInputEl.value = "";
            searchInputEl.placeholder = "Tìm khách hàng, mã NPL, số lô, barcode...";
        }
        var searchCountEl = byId("detailSearchCount");
        if (searchCountEl) searchCountEl.textContent = "";

        // Show loading state
        if (modalContent) modalContent.innerHTML =
            "<div class=\"dk-empty\" style=\"padding:30px\">Đang tải chi tiết...</div>";
        modal.classList.add("open");

        var url = "/api/DashboardKhoDesktop/GetActivityDayDetail?ngay=" + dateKey;
        var inRangeMode = false; // Luôn hiển thị 1 ngày duy nhất khi click vào ô ngày, dù đang bật filter range
        console.log("[Dashboard Kho] Fetching:", url);
        requestJson(url).then(function (data) {
            console.log("[Dashboard Kho] Day detail response:", data);
            var d = data || {};
            var nhapRows = normalizeArray(d.Nhap);
            var xuatRows = normalizeArray(d.Xuat);
            var kiemKeRows = normalizeArray(d.KiemKe);

            for (var i = 0; i < nhapRows.length; i++) {
                if (!nhapRows[i].NgayNhap && !nhapRows[i].NgayNhapKho) nhapRows[i].NgayNhapKho = inRangeMode ? (nhapRows[i].NgayNhapDen || nhapRows[i].NgayNhapTu || "") : dateKey;
            }
            for (var i = 0; i < xuatRows.length; i++) {
                if (!xuatRows[i].NgayXuat) xuatRows[i].NgayXuat = inRangeMode ? (xuatRows[i].NgayXuatDen || xuatRows[i].NgayXuatTu || "") : dateKey;
            }
            for (var i = 0; i < kiemKeRows.length; i++) {
                if (!kiemKeRows[i].NgayKiemKe) kiemKeRows[i].NgayKiemKe = inRangeMode ? (kiemKeRows[i].NgayKKDen || kiemKeRows[i].NgayKKTu || "") : dateKey;
            }

            // v2.3.43 — NK dự kiến: nếu range mode thì lọc theo range; nếu không thì 1 ngày
            var nkSrc = (state.nkDuKien && state.nkDuKien.length > 0) ? state.nkDuKien : (state.inbound || []);
            var plannedRows = nkSrc.filter(function (r) {
                var dt = parseDate(r.NgayNKDuKien);
                if (!dt) return false;
                if (inRangeMode) {
                    return dt >= calRangeFrom && dt <= calRangeTo;
                }
                return asIsoDate(dt) === dateKey;
            });

            renderDayDetailTabs(modalContent, nhapRows, xuatRows, kiemKeRows, plannedRows);

            // v2.3.54 — Append Lịch Phân Công tab sau 4 tabs kho
            if (!inRangeMode) {
                requestJson("/api/DashboardKhoDesktop/LichPhanCong_GetDayDetail?ngay=" + dateKey)
                    .then(function (lpcpRes) {
                        if (!lpcpRes || !lpcpRes.success) return;
                        appendLpcpTab(modalContent, lpcpRes.data || {});
                    })
                    .catch(function () { /* optional tab — silent fail */ });
            }
        }).catch(function (err) {
            console.error("[Dashboard Kho] Day detail API error:", err);
            // v2.3.8.2 — Silent fallback: dùng data sẵn có (không hiện banner cảnh báo)
            var plannedRows = (state.inbound || []).filter(function (r) {
                var dt = parseDate(r.NgayNKDuKien);
                if (!dt) return false;
                return asIsoDate(dt) === dateKey;
            });
            var xuatFallback = (state.outboundRunning || []).filter(function (r) {
                var dt = parseDate(r.NgayXuatHang);
                if (!dt) return false;
                return asIsoDate(dt) === dateKey;
            }).map(function (r) {
                return {
                    MaLenh: r.MaLenh || "",
                    TenHang: r.TenHang || "",
                    TenKH: r.TenKH || r.KhachHang || "",
                    SoLuong: toNumber(r.SLXuat || r.SoLuong),
                    SoBarCode: 0
                };
            });
            if (modalContent) {
                renderDayDetailTabs(modalContent, [], xuatFallback, [], plannedRows);
            }
        });
    }

    // v2.3.48 — Tabs cho Tổng quát khoảng ngày: nhập dùng grouped table (date+PINCC), còn lại giữ flat

    window.dkToggleGroup = function (btn) {
        var groupKey = btn.getAttribute("data-group");
        var icon = btn.querySelector("i");
        var tbody = btn.closest("table").querySelector("tbody");
        var isExpanded = btn.getAttribute("data-expanded") === "true";

        btn.setAttribute("data-expanded", !isExpanded);
        if (isExpanded) {
            icon.style.transform = "rotate(-90deg)";
        } else {
            icon.style.transform = "rotate(0deg)";
        }

        var rows = tbody.querySelectorAll("tr[data-group='" + groupKey + "']:not(.group-header)");
        for (var i = 0; i < rows.length; i++) {
            rows[i].style.display = isExpanded ? "none" : "";
        }
    };

    function renderGroupedDetailTable(cols, rows, dateField) {
        if (!rows || rows.length === 0) return "<div class='dk-empty' style='padding:30px'>Không có dữ liệu</div>";

        var groups = {};
        for (var i = 0; i < rows.length; i++) {
            var r = rows[i];
            var rawDate = r[dateField] || r.NgayNhapKho || r.NgayNhap || r.NgayXuat || r.NgayKiemKe || r.NgayNKDuKien || "";
            var dateStr = "";
            if (rawDate) {
                var d = new Date(rawDate);
                if (!isNaN(d)) {
                    var dd = String(d.getDate()).padStart(2, '0');
                    var mm = String(d.getMonth() + 1).padStart(2, '0');
                    var yyyy = d.getFullYear();
                    dateStr = dd + "/" + mm + "/" + yyyy;
                } else {
                    dateStr = rawDate.toString().substring(0, 10);
                }
            } else {
                dateStr = "Không có ngày";
            }

            if (!groups[dateStr]) groups[dateStr] = [];
            groups[dateStr].push(r);
        }

        // Sort groups by date descending
        var sortedDates = Object.keys(groups).sort(function (a, b) {
            if (a === "Không có ngày") return 1;
            if (b === "Không có ngày") return -1;
            var partA = a.split("/");
            var partB = b.split("/");
            var dA = new Date(partA[2], partA[1] - 1, partA[0]);
            var dB = new Date(partB[2], partB[1] - 1, partB[0]);
            return dB - dA;
        });

        var html = "<div class='dk-detail-table-wrap'><table class='dk-detail-table'>";
        html += "<thead><tr>";
        for (var c = 0; c < cols.length; c++) {
            var w = cols[c].width ? "width:" + cols[c].width + ";" : "";
            var align = cols[c].align ? "text-align:" + cols[c].align + ";" : "";
            html += "<th style='" + w + align + "'>" + escapeHtml(cols[c].label) + "</th>";
        }
        html += "</tr></thead><tbody>";

        for (var gi = 0; gi < sortedDates.length; gi++) {
            var gDate = sortedDates[gi];
            var groupRows = groups[gDate];
            var groupKey = "g_" + gi;

            // Tính tổng
            var hasSoLuong = false;
            var dayTotal = 0;
            for (var ck = 0; ck < cols.length; ck++) if (cols[ck].key === "SoLuong" || cols[ck].key === "SoLuongDuKien") hasSoLuong = true;
            if (hasSoLuong) {
                for (var ri = 0; ri < groupRows.length; ri++) {
                    var q = groupRows[ri].SoLuong || groupRows[ri].SoLuongDuKien || 0;
                    dayTotal += parseFloat(q) || 0;
                }
            }

            // Group header
            var dark = document.body.classList.contains("dark-theme");
            var headBg = dark ? "#1e3a8a" : "#dbeafe"; // Nền xanh dương đặc màu (không dùng rgba)
            var headBorder = dark ? "#1d4ed8" : "#bfdbfe"; // Viền trên/dưới
            var dateColor = dark ? "#93c5fd" : "#1e40af"; // Màu chữ ngày
            var totalColor = dark ? "#60a5fa" : "#0284c7"; // Màu chữ tổng
            var subTextColor = dark ? "#bfdbfe" : "#475569"; // Màu chữ phụ

            html += "<tr class='group-header' style='background-color: " + headBg + "; cursor: pointer; border-top: 1px solid " + headBorder + "; border-bottom: 1px solid " + headBorder + ";' onclick='window.dkToggleGroup(this)' data-group='" + groupKey + "' data-expanded='true'>";

            var qtyColIdx = -1;
            for (var ck = 0; ck < cols.length; ck++) if (cols[ck].key === "SoLuong" || cols[ck].key === "SoLuongDuKien") qtyColIdx = ck;

            if (qtyColIdx > 0 && hasSoLuong) {
                html += "<td colspan='" + qtyColIdx + "' style='font-weight: bold; padding: 6px 8px 6px 12px; position: relative; text-align: left; border-left: 3px solid " + totalColor + ";'>";
                html += "<i class='fas fa-chevron-down dk-toggle-icon' style='margin-right: 6px; font-size: 11px; transition: transform 0.2s; display: inline-block; color: " + subTextColor + ";'></i>";
                html += "<span style='color: " + dateColor + "; font-size: 13px;'>&#128197; Ngày " + escapeHtml(gDate) + "</span>";
                html += "<span style='font-size: 11px; font-weight: 500; color: " + subTextColor + "; margin-left: 8px;'>(" + formatNumber(groupRows.length, 0) + " dòng)</span>";
                html += "</td>";

                var cellAlign = cols[qtyColIdx].center ? "center" : (cols[qtyColIdx].left ? "left" : "right");
                html += "<td style='font-weight: bold; font-size: 13px; padding: 6px 8px; text-align: " + cellAlign + "; color: " + totalColor + ";'>" + formatNumber(dayTotal, 2) + "</td>";

                var remainingCols = cols.length - 1 - qtyColIdx;
                if (remainingCols > 0) {
                    html += "<td colspan='" + remainingCols + "'></td>";
                }
            } else {
                html += "<td colspan='" + cols.length + "' style='font-weight: bold; padding: 6px 8px 6px 12px; position: relative; text-align: left; border-left: 3px solid " + totalColor + ";'>";
                html += "<i class='fas fa-chevron-down dk-toggle-icon' style='margin-right: 6px; font-size: 11px; transition: transform 0.2s; display: inline-block; color: " + subTextColor + ";'></i>";
                html += "<span style='color: " + dateColor + "; font-size: 13px;'>&#128197; Ngày " + escapeHtml(gDate) + "</span>";
                if (hasSoLuong) {
                    html += "<span style='margin-left: 12px; color: " + totalColor + "; font-size: 13px;'>Tổng: " + formatNumber(dayTotal, 2) + "</span>";
                }
                html += "<span style='float: right; font-size: 11px; font-weight: 500; color: " + subTextColor + "; margin-top: 1px;'>" + formatNumber(groupRows.length, 0) + " dòng</span>";
                html += "</td>";
            }
            html += "</tr>";

            // Group rows
            for (var ri = 0; ri < groupRows.length; ri++) {
                var row = groupRows[ri];
                html += "<tr data-group='" + groupKey + "'>";
                for (var c = 0; c < cols.length; c++) {
                    var cv = cols[c];
                    var val = row[cv.field || cv.key];
                    if (val === null || val === undefined) val = "";
                    if (cv.key === "STT") val = ri + 1;
                    if (cv.type === "number" || cv.number !== undefined) val = formatNumber(val, cv.number !== undefined ? cv.number : 0);
                    if (cv.type === "date" || cv.date) val = typeof formatDateVn === 'function' ? formatDateVn(val) : (typeof formatDate === 'function' ? formatDate(val) : val);

                    var cellAlign = cv.center ? "center" : (cv.number !== undefined ? "right" : (cv.left ? "left" : ""));
                    var align = cellAlign ? " style='text-align:" + cellAlign + "'" : "";
                    html += "<td" + align + ">" + escapeHtml(val.toString()) + "</td>";
                }
                html += "</tr>";
            }
        }

        html += "</tbody></table></div>";
        return html;
    }

    function renderOverviewDetailTabs(container, nhap, xuat, kiemke, planned) {
        if (!container) return;

        function addStt(arr) {
            return (arr || []).map(function (r, i) { return Object.assign({ STT: i + 1 }, r); });
        }
        var tabs = [
            { key: "nhap", label: "Nhập kho", color: "#3b82f6", rows: addStt(nhap), cols: colsNhap(), dateField: "NgayNhap" },
            { key: "xuat", label: "Xuất kho", color: "#f97316", rows: addStt(xuat), cols: colsXuat(), dateField: "NgayXuat" },
            { key: "kiemke", label: "Kiểm kê", color: "#10b981", rows: addStt(kiemke), cols: colsKiemKe(), dateField: "NgayKiemKe" },
            { key: "plan", label: "NK dự kiến", color: "#8b5cf6", rows: addStt(planned), cols: colsPlanned(), dateField: "NgayNKDuKien" }
        ];

        var activeIdx = 0;
        for (var ti = 0; ti < tabs.length; ti++) {
            if (tabs[ti].rows.length > 0) { activeIdx = ti; break; }
        }

        var tabBar = "<div class=\"dk-day-tabs\">";
        for (var i = 0; i < tabs.length; i++) {
            var t = tabs[i];
            var isActive = i === activeIdx ? " active" : "";
            tabBar += "<button type=\"button\" class=\"dk-day-tab" + isActive +
                "\" data-tabkey=\"" + t.key + "\" style=\"--tab-color:" + t.color + "\">" +
                "<span class=\"dk-day-tab-dot\" style=\"background:" + t.color + "\"></span>" +
                escapeHtml(t.label) +
                " <span class=\"dk-day-tab-count\">" + formatNumber(t.rows.length, 0) + "</span>" +
                "</button>";
        }
        tabBar += "</div>";

        var panels = "";
        for (var pi = 0; pi < tabs.length; pi++) {
            var tp = tabs[pi];
            var hidden = pi === activeIdx ? "" : " style=\"display:none\"";
            panels += "<div class=\"dk-day-panel\" data-tabkey=\"" + tp.key + "\"" + hidden + ">";
            if (tp.rows.length === 0) {
                panels += "<div class=\"dk-empty\" style=\"padding:30px\">Không có dữ liệu " + escapeHtml(tp.label.toLowerCase()) + "</div>";
            } else {
                panels += renderGroupedDetailTable(tp.cols, tp.rows, tp.dateField);
            }
            panels += "</div>";
        }

        container.innerHTML = tabBar + panels;

        setTimeout(function () {
            var tabs = container.querySelector(".dk-day-tabs");
            if (tabs) {
                var tabsHeight = tabs.offsetHeight;
                var ths = container.querySelectorAll(".dk-detail-table thead th");
                for (var i = 0; i < ths.length; i++) {
                    ths[i].style.top = tabsHeight + "px";
                }
            }
        }, 10);

        var tabBtns = container.querySelectorAll(".dk-day-tab");
        for (var bi = 0; bi < tabBtns.length; bi++) {
            tabBtns[bi].addEventListener("click", function () {
                var key = this.getAttribute("data-tabkey");
                var allBtns = container.querySelectorAll(".dk-day-tab");
                for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
                this.classList.add("active");
                var allPanels = container.querySelectorAll(".dk-day-panel");
                for (var p = 0; p < allPanels.length; p++) {
                    allPanels[p].style.display = allPanels[p].getAttribute("data-tabkey") === key ? "" : "none";
                }
                var searchInput = document.getElementById("detailSearchInput");
                if (searchInput && searchInput.value) {
                    if (typeof filterDetailTable === 'function') filterDetailTable(searchInput.value.trim());
                }
            });
        }
    }

    function renderDayDetailTabs(container, nhap, xuat, kiemke, planned) {
        if (!container) return;

        function addStt(arr) {
            return (arr || []).map(function (r, i) { return Object.assign({ STT: i + 1 }, r); });
        }
        var tabs = [
            { key: "nhap", label: "Nhập kho", color: "#3b82f6", rows: addStt(nhap), cols: colsNhap(), dateField: "NgayNhap" },
            { key: "xuat", label: "Xuất kho", color: "#f97316", rows: addStt(xuat), cols: colsXuat(), dateField: "NgayXuat" },
            { key: "kiemke", label: "Kiểm kê", color: "#10b981", rows: addStt(kiemke), cols: colsKiemKe(), dateField: "NgayKiemKe" },
            { key: "plan", label: "NK dự kiến", color: "#8b5cf6", rows: addStt(planned), cols: colsPlanned(), dateField: "NgayNKDuKien" }
        ];

        var activeIdx = 0;
        for (var ti = 0; ti < tabs.length; ti++) {
            if (tabs[ti].rows.length > 0) { activeIdx = ti; break; }
        }

        var tabBar = "<div class=\"dk-day-tabs\">";
        for (var i = 0; i < tabs.length; i++) {
            var t = tabs[i];
            var isActive = i === activeIdx ? " active" : "";
            tabBar += "<button type=\"button\" class=\"dk-day-tab" + isActive +
                "\" data-tabkey=\"" + t.key + "\" style=\"--tab-color:" + t.color + "\">" +
                "<span class=\"dk-day-tab-dot\" style=\"background:" + t.color + "\"></span>" +
                escapeHtml(t.label) +
                " <span class=\"dk-day-tab-count\">" + formatNumber(t.rows.length, 0) + "</span>" +
                "</button>";
        }
        tabBar += "</div>";

        var panels = "";
        for (var pi = 0; pi < tabs.length; pi++) {
            var tp = tabs[pi];
            var hidden = pi === activeIdx ? "" : " style=\"display:none\"";
            panels += "<div class=\"dk-day-panel\" data-tabkey=\"" + tp.key + "\"" + hidden + ">";
            if (tp.rows.length === 0) {
                panels += "<div class=\"dk-empty\" style=\"padding:30px\">Không có dữ liệu " + escapeHtml(tp.label.toLowerCase()) + " trong ngày này</div>";
            } else {
                panels += renderGroupedDetailTable(tp.cols, tp.rows, tp.dateField);
            }
            panels += "</div>";
        }

        container.innerHTML = tabBar + panels;

        setTimeout(function () {
            var tabs = container.querySelector(".dk-day-tabs");
            if (tabs) {
                var tabsHeight = tabs.offsetHeight;
                var ths = container.querySelectorAll(".dk-detail-table thead th");
                for (var i = 0; i < ths.length; i++) {
                    ths[i].style.top = tabsHeight + "px";
                }
            }
        }, 10);

        var tabBtns = container.querySelectorAll(".dk-day-tab");
        for (var bi = 0; bi < tabBtns.length; bi++) {
            tabBtns[bi].addEventListener("click", function () {
                var key = this.getAttribute("data-tabkey");
                var allBtns = container.querySelectorAll(".dk-day-tab");
                for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
                this.classList.add("active");
                var allPanels = container.querySelectorAll(".dk-day-panel");
                for (var p = 0; p < allPanels.length; p++) {
                    allPanels[p].style.display = allPanels[p].getAttribute("data-tabkey") === key ? "" : "none";
                }
                var searchInput = document.getElementById("detailSearchInput");
                if (searchInput && searchInput.value) {
                    if (typeof filterDetailTable === 'function') filterDetailTable(searchInput.value.trim());
                }
            });
        }
    }

    // // v2.3.60 — Tải stats LPCP cho đúng tháng đang hiển thị trên lịch (Đã sửa để gọi render biểu đồ mới)
    function loadLpcpStatsForMonth(baseDate) {
        // Stats strip was removed, but we still trigger the bottom charts rendering here
        renderLpcpBottomCharts();
    }

    function renderLpcpStatsPage3() {
        // Disabled since stats strip is removed
    }

    function bindLpcpStatsClick() {
        // Disabled since stats strip is removed
    }

    // Biểu đồ mới phần Lịch phân công
    // Biểu đồ mới phần Lịch phân công
    function renderLpcpBottomCharts() {
        var isDark = document.body.classList.contains("dark-theme");
        var textColor = isDark ? "#f8fafc" : "#1e293b"; // Đậm hơn ở light mode, sáng hơn ở dark mode
        var gridColor = isDark ? "rgba(255,255,255,0.05)" : "#e2e8f0";
        var bgColor = isDark ? "#1e293b" : "#ffffff";

        // Hàm tiện ích
        function _asIso(d) { return d.getFullYear() + "-" + String(d.getMonth() + 1).padStart(2, "0") + "-" + String(d.getDate()).padStart(2, "0"); }

        // --- Data cho Biểu đồ 1: Khối lượng theo loại hoạt động (Dữ liệu đang xem) ---
        var sumIn = 0, sumOut = 0, sumKiemKe = 0;
        var curMonth = calMonthDate || new Date();
        var targetYear = curMonth.getFullYear();
        var targetMonth = curMonth.getMonth();

        if (state.activityCalendar) {
            state.activityCalendar.forEach(function (d) {
                var dDate = parseDate(d.NgayHoatDong || d.ngayHoatDong);
                if (dDate && dDate.getFullYear() === targetYear && dDate.getMonth() === targetMonth) {
                    sumIn += toNumber(d.TotalIn || d.totalIn);
                    sumOut += toNumber(d.TotalOut || d.totalOut);
                    sumKiemKe += toNumber(d.TotalKiemKe || d.totalKiemKe);
                }
            });
        }
        var sumTotal = sumIn + sumOut + sumKiemKe;

        var pieData = [
            { name: 'Nhập kho', y: sumIn, color: '#3b82f6' },
            { name: 'Xuất kho', y: sumOut, color: '#f97316' },
            { name: 'Kiểm kê', y: sumKiemKe, color: '#8b5cf6' }
        ];

        var emptySeries = [];

        if (document.getElementById("chartVolumePie")) {
            Highcharts.chart("chartVolumePie", {
                chart: {
                    type: "pie",
                    backgroundColor: bgColor,
                    events: {
                        render: function () {
                            var chart = this;
                            var series = chart.series[0];
                            if (series && series.center) {
                                var isDk = document.body.classList.contains("dark-theme");
                                var txtCol = isDk ? "#f8fafc" : "#1e293b";
                                var cx = chart.plotLeft + series.center[0];
                                var cy = chart.plotTop + series.center[1];
                                if (chart.centerLabel) {
                                    chart.centerLabel.destroy();
                                    chart.centerLabel = null;
                                }
                                chart.centerLabel = chart.renderer.text(
                                    '<div style="text-align:center;font-size:14px;line-height:1.2;color:' + txtCol + ';">Tổng<br><span style="font-size:20px;font-weight:700;">' + Highcharts.numberFormat(sumTotal, 0, '.', '.') + '</span></div>',
                                    cx, cy, true
                                ).attr({ align: 'center', zIndex: 10 }).add();
                                chart.centerLabel.attr({ x: cx, y: cy - 16 });
                            }
                        }
                    }
                },
                title: { text: null },
                credits: { enabled: false },
                plotOptions: {
                    pie: {
                        innerSize: '75%', size: '85%', borderWidth: 0,
                        dataLabels: { enabled: false }, showInLegend: true, center: ['35%', '50%']
                    }
                },
                legend: {
                    layout: 'vertical', align: 'right', verticalAlign: 'middle',
                    itemStyle: { color: textColor, fontWeight: '600', fontSize: '14px' },
                    useHTML: true,
                    labelFormatter: function () {
                        var isDk = document.body.classList.contains("dark-theme");
                        var txtCol = isDk ? "#f8fafc" : "#1e293b";
                        var pct = sumTotal > 0 ? (this.y / sumTotal * 100).toFixed(1) : 0;
                        return '<span style="color:' + txtCol + ';">' + this.name + ': <b>' + pct + '%</b></span>';
                    }
                },
                series: emptySeries.concat([{
                    name: 'Khối lượng',
                    data: pieData,
                    showInLegend: true
                }]),
                tooltip: {
                    pointFormat: '<b>{point.y:,.0f}</b> ({point.percentage:.1f}%)'
                }
            });
        }

        // --- Biểu đồ 2 & 3: Lấy trực tiếp từ SQL API theo range được chọn ---
        var now = new Date();
        var trendDays = document.getElementById("trendFilterSelect") ? parseInt(document.getElementById("trendFilterSelect").value) : 30;
        var loadDays = document.getElementById("loadFilterSelect") ? parseInt(document.getElementById("loadFilterSelect").value) : 30;

        // Range: bỏ hôm nay, lùi N ngày kể từ hôm qua
        function buildRange(nDays) {
            var to = new Date(now); to.setDate(to.getDate() - 1);
            var from = new Date(now); from.setDate(from.getDate() - nDays);
            return { from: _asIso(from), to: _asIso(to) };
        }
        function doFetch(url) {
            return typeof requestJson === 'function' ? requestJson(url) : fetch(url).then(function (r) { return r.json(); });
        }

        // ── Biểu đồ 2: Xu hướng Nhập-Xuất ─────────────────────────────────────────
        if (document.getElementById("chartTrendLine")) {
            var rTrend = buildRange(trendDays);
            var urlTrend = "/api/DashboardKhoDesktop/GetFlowTrendByRange?tuNgay=" + rTrend.from + "&denNgay=" + rTrend.to;
            document.getElementById("chartTrendLine").innerHTML =
                '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:' + textColor + ';font-size:12px;">Đang tải...</div>';

            doFetch(urlTrend).then(function (data) {
                var arr = Array.isArray(data) ? data : (data && data.value ? data.value : []);
                var cats = [], inArr = [], outArr = [];
                for (var j = 0; j < arr.length; j++) {
                    var ngay = String(arr[j].Ngay || arr[j].ngay || "").substring(0, 10);
                    var pts = ngay.split("-");
                    cats.push(pts.length === 3 ? (pts[2] + "/" + pts[1]) : ngay);
                    inArr.push(toNumber(arr[j].TotalIn || arr[j].totalIn));
                    outArr.push(toNumber(arr[j].TotalOut || arr[j].totalOut));
                }
                if (!document.getElementById("chartTrendLine")) return;
                Highcharts.chart("chartTrendLine", {
                    chart: { type: "spline", backgroundColor: bgColor, spacingTop: 20, spacingBottom: 15 },
                    title: { text: '' },
                    xAxis: { categories: cats, labels: { style: { color: textColor, fontSize: '14px' } }, tickLength: 0, lineColor: isDark ? '#334155' : '#e2e8f0' },
                    yAxis: {
                        title: { text: 'Khối lượng (cuộn/m)', style: { fontSize: '13px' } },
                        labels: { style: { color: textColor, fontSize: '14px' }, formatter: function () { var v = this.value; return v >= 1000000 ? (v / 1000000).toFixed(1) + 'M' : v >= 1000 ? (v / 1000).toFixed(0) + 'K' : v; } },
                        gridLineColor: isDark ? '#334155' : '#e2e8f0', min: 0
                    },
                    legend: { layout: 'horizontal', align: 'center', verticalAlign: 'bottom', y: 0, itemStyle: { color: textColor, fontWeight: '600', fontSize: '14px' } },
                    tooltip: { shared: true, formatter: function () { var s = '<b>' + this.points[0].key + '</b><br/>'; this.points.forEach(function (p) { s += '<span style="color:' + p.color + '">\u25CF</span> ' + p.series.name + ': <b>' + Highcharts.numberFormat(p.y, 0, '.', ',') + '</b><br/>'; }); return s; } },
                    plotOptions: { spline: { marker: { radius: 3, symbol: 'circle' }, lineWidth: 2 } },
                    series: [
                        { name: 'Nhập kho', data: inArr, color: '#3b82f6' },
                        { name: 'Xuất kho', data: outArr, color: '#f97316' }
                    ],
                    credits: { enabled: false }
                });
            }).catch(function () {
                var el = document.getElementById("chartTrendLine");
                if (el) el.innerHTML = '<div style="padding:20px;color:' + textColor + ';">Không thể tải dữ liệu</div>';
            });
        }

        // ── Biểu đồ 3: Tồn kho theo ngày (GetFlowTrendByRange → TotalStock) ────────
        if (document.getElementById("chartLoadBar")) {
            var rLoad = buildRange(loadDays);
            var urlLoad = "/api/DashboardKhoDesktop/GetFlowTrendByRange?tuNgay=" + rLoad.from + "&denNgay=" + rLoad.to;
            document.getElementById("chartLoadBar").innerHTML =
                '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:' + textColor + ';font-size:12px;">Đang tải...</div>';

            doFetch(urlLoad).then(function (data) {
                var arr = Array.isArray(data) ? data : (data && data.value ? data.value : []);
                var cats = [], vals = [], colors = [];
                var maxStock = 0;
                for (var j = 0; j < arr.length; j++) {
                    var s = toNumber(arr[j].TotalStock || arr[j].totalStock);
                    if (s > maxStock) maxStock = s;
                }
                for (var j = 0; j < arr.length; j++) {
                    var ngay = String(arr[j].Ngay || arr[j].ngay || "").substring(0, 10);
                    var pts = ngay.split("-");
                    cats.push(pts.length === 3 ? (pts[2] + "/" + pts[1]) : ngay);
                    var stock = toNumber(arr[j].TotalStock || arr[j].totalStock);
                    vals.push(stock);
                    var pct = maxStock > 0 ? (stock / maxStock) * 100 : 0;
                    colors.push(pct >= 95 ? '#ef4444' : pct >= 80 ? '#eab308' : '#22c55e');
                }
                if (!document.getElementById("chartLoadBar")) return;
                var pw = loadDays <= 7 ? 28 : (loadDays <= 14 ? 16 : (loadDays <= 30 ? 8 : 4));
                Highcharts.chart("chartLoadBar", {
                    chart: { type: "column", backgroundColor: bgColor, spacingTop: 20, spacingBottom: 15 },
                    title: { text: '' },
                    xAxis: { categories: cats, labels: { style: { color: textColor, fontSize: '14px' } }, tickLength: 0, lineColor: isDark ? '#334155' : '#e2e8f0' },
                    yAxis: {
                        title: { text: 'Tồn kho (cuộn/m)', style: { fontSize: '13px' } },
                        labels: { style: { color: textColor, fontSize: '14px' }, formatter: function () { var v = this.value; return v >= 1000000 ? (v / 1000000).toFixed(1) + 'M' : v >= 1000 ? (v / 1000).toFixed(0) + 'K' : v; } },
                        gridLineColor: isDark ? '#334155' : '#e2e8f0', min: 0
                    },
                    legend: { enabled: false },
                    tooltip: { formatter: function () { return '<b>' + this.key + '</b><br/>Tồn kho: <b>' + Highcharts.numberFormat(this.y, 0, '.', ',') + '</b>'; } },
                    plotOptions: { column: { borderRadius: 4, borderWidth: 0, maxPointWidth: 28, pointWidth: pw, colorByPoint: true, colors: colors } },
                    series: [{
                        name: 'Tồn kho',
                        data: vals,
                        dataLabels: {
                            enabled: loadDays <= 14,
                            formatter: function () { var v = this.y; return v >= 1000000 ? (v / 1000000).toFixed(1) + 'M' : v >= 1000 ? (v / 1000).toFixed(0) + 'K' : String(v); },
                            style: { color: textColor, fontSize: '14px', fontWeight: 'normal', textOutline: 'none' }
                        }
                    }],
                    credits: { enabled: false }
                });
            }).catch(function () {
                var el = document.getElementById("chartLoadBar");
                if (el) el.innerHTML = '<div style="padding:20px;color:' + textColor + ';">Không thể tải dữ liệu</div>';
            });
        }
    }
    function showLpcpMonthModal(type) {
        var baseDate = calMonthDate || new Date();
        var y = baseDate.getFullYear();
        var m = baseDate.getMonth() + 1;
        var from = y + '-' + String(m).padStart(2, '0') + '-01';
        var to = y + '-' + String(m).padStart(2, '0') + '-' + String(new Date(y, m, 0).getDate()).padStart(2, '0');

        var modalTitle = byId(ids.detailModalTitle);
        var modalMeta = byId(ids.detailModalMeta);
        var modalContent = byId(ids.detailModalContent);
        var modal = byId(ids.detailModal);
        if (!modal) return;

        var titleMap = {
            'tong': 'Tất cả Task Phụ Liệu trong tháng',
            'hoan': 'Task Phụ Liệu - Hoàn thành',
            'dang': 'Task Phụ Liệu - Đang thực hiện',
            'cho': 'Task Phụ Liệu - Chờ thực hiện',
            'canhbao': 'Task Phụ Liệu - Cảnh báo / Chưa HT'
        };

        if (modalTitle) modalTitle.textContent = titleMap[type] || "Chi tiết Task";
        if (modalMeta) modalMeta.innerHTML = "<span style=\"color:#64748b;font-size:13px\">Đang tải dữ liệu...</span>";
        if (modalContent) modalContent.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px\">Đang tải dữ liệu...</div>";
        modal.classList.add("open");

        // Hide search bar for simplicity in this view
        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "none";

        var url = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" + from + "&denNgay=" + to;
        requestJson(url).then(function (res) {
            var rawData = (res && res.data) ? res.data : [];
            var tasks = [];
            var seen = {};

            for (var i = 0; i < rawData.length; i++) {
                var dayData = rawData[i];
                var dayTasks = dayData.Tasks || [];
                var dateStr = String(dayData.NgayLam || '').substring(0, 10);
                var formattedDate = dateStr.split('-').reverse().join('/');

                for (var j = 0; j < dayTasks.length; j++) {
                    var t = dayTasks[j];
                    var key = (t.MaLenhSX || '') + '_' + dateStr;
                    if (!seen[key]) {
                        seen[key] = true;
                        var tt = t.TrangThai || 0;

                        var match = false;
                        if (type === 'tong') match = true;
                        else if (type === 'hoan' && tt === 2) match = true;
                        else if (type === 'dang' && tt === 1) match = true;
                        else if (type === 'cho' && tt === 0) match = true;
                        else if (type === 'canhbao' && tt === 3) match = true;

                        if (match) {
                            t._formattedDate = formattedDate;
                            tasks.push(t);
                        }
                    }
                }
            }

            if (modalMeta) {
                modalMeta.innerHTML = "<span style=\"font-weight:600;color:#3b82f6\">Tìm thấy " + tasks.length + " task</span>";
            }

            if (tasks.length === 0) {
                if (modalContent) modalContent.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px\">Không có task nào trong tháng này.</div>";
                return;
            }

            var html = '<div style="width: 100%; background: var(--dk-card); border-radius: 8px; overflow: hidden; border: 1px solid var(--dk-line); box-shadow: 0 4px 6px rgba(0,0,0,0.02);">';
            html += '<div style="overflow-x: auto;">';
            html += '<table style="width:100%; border-collapse:collapse; min-width: 700px; font-size: 13px;">';
            html += '<thead><tr style="background: var(--dk-card-alt, rgba(0,0,0,0.02)); border-bottom: 1px solid var(--dk-line);">';
            html += '<th style="padding:14px 16px;text-align:left;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:45%;">Mã lệnh / Brand</th>';
            html += '<th style="padding:14px 16px;text-align:left;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:15%;">Ngày giao</th>';
            html += '<th style="padding:14px 16px;text-align:left;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:25%;">Người thực hiện</th>';
            html += '<th style="padding:14px 16px;text-align:center;border-bottom:1px solid var(--dk-line);font-weight:600;color:var(--dk-title);width:15%;">Trạng thái</th>';
            html += '</tr></thead><tbody>';

            var STATUS_LABELS = ['Chờ TH', 'Đang TH', 'Hoàn thành', 'Chưa HT'];
            var STATUS_BG = ["rgba(249,115,22,0.15)", "rgba(59,130,246,0.15)", "rgba(34,197,94,0.15)", "rgba(239,68,68,0.15)"];
            var STATUS_COLOR = ["#f97316", "#3b82f6", "#22c55e", "#ef4444"];
            for (var k = 0; k < tasks.length; k++) {
                var tk = tasks[k];
                var tt = tk.TrangThai || 0;
                var stLabel = STATUS_LABELS[tt] || 'Không rõ';
                var stBg = STATUS_BG[tt] || STATUS_BG[0];
                var stColor = STATUS_COLOR[tt] || STATUS_COLOR[0];

                html += '<tr style="transition: background 0.2s; border-bottom:1px solid var(--dk-line);" onmouseover="this.style.background=\'rgba(148,163,184,0.1)\'" onmouseout="this.style.background=\'transparent\'">';
                html += '<td style="padding:14px 16px;text-align:left;"><b>' + escapeHtml(tk.MaLenhSX || 'N/A') + '</b>' + (tk.TenBrand ? ' <span style="color:var(--dk-muted);"> - ' + escapeHtml(tk.TenBrand) + '</span>' : '') + '</td>';
                html += '<td style="padding:14px 16px;text-align:left;color:var(--dk-muted);">' + tk._formattedDate + '</td>';
                html += '<td style="padding:14px 16px;text-align:left;">' + escapeHtml(tk.TenNV || 'Chưa phân công') + '</td>';
                html += '<td style="padding:14px 16px;text-align:center;"><span style="background:' + stBg + ';color:' + stColor + ';padding:4px 10px;border-radius:6px;font-size:12px;font-weight:600;">' + stLabel + '</span></td>';
                html += '</tr>';
            }
            html += '</tbody></table></div></div>';

            if (modalContent) modalContent.innerHTML = html;

        }).catch(function (err) {
            if (modalContent) modalContent.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px;color:#dc2626\">Lỗi tải dữ liệu.</div>";
        });
    }

    // v2.3.60 — Render LPCP detail inline (sidebar phải page 3) khi click vào ô ngày
    function showLpcpInlineDetail(dateKey) {
        var dateEl = document.getElementById("lpcpDetailDate");
        var badgeEl = document.getElementById("lpcpDetailBadge");
        var assignEl = document.getElementById("lpcpDetailAssignments");
        var pickEl = document.getElementById("lpcpDetailPickOrders");
        var warnEl = document.getElementById("lpcpDetailWarnings");

        // Format date display — "Thứ Tư, 27/05/2026"
        var parts = String(dateKey).split("-");
        var DOW_NAMES = ["Ch\u1ee7 Nh\u1eadt", "Th\u1ee9 Hai", "Th\u1ee9 Ba", "Th\u1ee9 T\u01b0", "Th\u1ee9 N\u0103m", "Th\u1ee9 S\u00e1u", "Th\u1ee9 B\u1ea3y"];
        var displayDate = dateKey;
        if (parts.length === 3) {
            var dow = DOW_NAMES[new Date(dateKey + "T00:00:00").getDay()] || "";
            displayDate = dow + ", " + parts[2] + "/" + parts[1] + "/" + parts[0];
        }

        // Highlight selected cell
        var cells = document.querySelectorAll(".dk-cal-monthly-cell");
        for (var ci = 0; ci < cells.length; ci++) {
            cells[ci].classList.remove("is-selected");
            if (cells[ci].getAttribute("data-date") === dateKey) {
                cells[ci].classList.add("is-selected");
            }
        }

        // Mở sidebar
        if (window.dkOpenSidebar) {
            window.dkOpenSidebar();

            // Match side panel height to calendar height so it doesn't stretch the page
            setTimeout(function () {
                var calPanel = document.querySelector('.dk-cal-monthly-panel');
                var sidePanel = document.getElementById('lpcpDetailPanel');
                if (calPanel && sidePanel) {
                    sidePanel.style.maxHeight = calPanel.offsetHeight + 'px';
                }
            }, 50);
        }

        // Update header
        if (dateEl) dateEl.textContent = displayDate;
        if (badgeEl) badgeEl.textContent = "\u0110ang t\u1ea3i...";

        // Spinners
        var SPIN = "<div class='dk-lpcp-placeholder'><svg width='14' height='14' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><circle cx='12' cy='12' r='10'/><polyline points='12 6 12 12 16 14'/></svg>\u0110ang t\u1ea3i...</div>";
        if (assignEl) assignEl.innerHTML = SPIN;
        if (pickEl) pickEl.innerHTML = SPIN;
        if (warnEl) warnEl.innerHTML = SPIN;

        // Reset badge counts
        ["lpcpWarnCount", "lpcpTaskCount", "lpcpPickCount"].forEach(function (id) {
            var el = document.getElementById(id);
            if (el) { el.style.display = "none"; el.textContent = ""; }
        });

        var TT_LABELS = ["Ch\u1edd TH", "\u0110ang TH", "Ho\u00e0n th\u00e0nh", "Ch\u01b0a HT"];
        var TT_COLORS = ["#d97706", "#2563eb", "#059669", "#ef4444"];
        var TT_BG = ["rgba(217,119,6,0.1)", "rgba(37,99,235,0.1)", "rgba(5,150,105,0.1)", "rgba(239,68,68,0.15)"];

        function ttBadge(tt) {
            var i = tt || 0;
            return "<span style='display:inline-flex;align-items:center;gap:4px;" +
                "background:" + (TT_BG[i] || TT_BG[0]) + ";color:" + (TT_COLORS[i] || TT_COLORS[0]) + ";" +
                "padding:2px 8px;border-radius:4px;font-size:10px;font-weight:500;flex-shrink:0;'>" +
                "<span style='width:5px;height:5px;border-radius:50%;background:" + (TT_COLORS[i] || TT_COLORS[0]) + ";display:inline-block;'></span>" +
                escapeHtml(TT_LABELS[i] || "?") + "</span>";
        }

        function setBadge(id, count) {
            var el = document.getElementById(id);
            if (!el) return;
            if (count > 0) { el.textContent = count; el.style.display = "inline-block"; }
            else { el.style.display = "none"; }
        }

        requestJson("/api/DashboardKhoDesktop/LichPhanCong_GetDayDetail?ngay=" + dateKey)
            .then(function (res) {
                var data = (res && res.data) ? res.data : {};
                var assignments = data.Assignments || [];
                var pickOrders = data.PickOrders || [];

                // Tự tổng hợp PickOrders nếu SP chưa trả ResultSet 2
                if (pickOrders.length === 0 && assignments.length > 0) {
                    var lenhMap = {}, lenhOrder = [];
                    assignments.forEach(function (a) {
                        if (!a.MaLenhSX) return;
                        if (!lenhMap[a.MaLenhSX]) {
                            lenhMap[a.MaLenhSX] = {
                                MaLenhSX: a.MaLenhSX, TrangThai: a.TrangThai || 0,
                                TenNV: a.TenNV || a.MaNV || "", MaNV: a.MaNV || "",
                                MaKhachHang: a.MaKhachHang || "", TenBrand: a.TenBrand || "",
                                NgaySoan: a.NgayThucHien || "", GioSoan: a.GioThucHien || ""
                            };
                            lenhOrder.push(a.MaLenhSX);
                        }
                    });
                    lenhOrder.forEach(function (ma) { pickOrders.push(lenhMap[ma]); });
                }

                // ── Xây dựng cảnh báo từ dữ liệu ──────────────────────────────────
                var warnings = [];
                var thieuNPL = [], chuaHT = [], choTH = [], thieuPO = [], tongThieu = 0;
                assignments.forEach(function (a) {
                    if (a.ThieuNPL) thieuNPL.push(a.MaLenhSX || "?");
                    if (a.TrangThai === 3) chuaHT.push(a.MaLenhSX || "?");
                    if (a.TrangThai === 0) choTH.push(a.MaLenhSX || "?");
                });
                pickOrders.forEach(function (po) {
                    if ((po.SoPLThieu || 0) > 0) { tongThieu += po.SoPLThieu; thieuPO.push(po.MaLenhSX || "?"); }
                });
                if (thieuNPL.length) warnings.push({ level: "critical", name: "Thi\u1ebfu NPL ph\u00e2n c\u00f4ng", cnt: thieuNPL.length + " l\u1ec7nh", desc: thieuNPL.slice(0, 3).join(", ") + (thieuNPL.length > 3 ? "..." : "") });
                if (thieuPO.length) warnings.push({ level: "critical", name: "PL thi\u1ebfu trong l\u1ec7nh so\u1ea1n", cnt: tongThieu + " lo\u1ea1i", desc: thieuPO.slice(0, 3).join(", ") + (thieuPO.length > 3 ? "..." : "") });
                if (chuaHT.length) warnings.push({ level: "caution", name: "Ch\u01b0a ho\u00e0n th\u00e0nh \u0111\u00fang h\u1ea1n", cnt: chuaHT.length + " task", desc: "C\u1ea7n theo d\u00f5i v\u00e0 x\u1eed l\u00fd ngay" });
                if (choTH.length) warnings.push({ level: "info", name: "Ch\u1edd b\u1eaft \u0111\u1ea7u TH", cnt: choTH.length + " task", desc: "Ch\u01b0a tri\u1ec3n khai trong ng\u00e0y" });

                // Badge counts
                setBadge("lpcpWarnCount", warnings.length);
                setBadge("lpcpTaskCount", assignments.length);
                setBadge("lpcpPickCount", pickOrders.length);
                if (badgeEl) badgeEl.textContent = assignments.length + " task \u00b7 " + pickOrders.length + " l\u1ec7nh so\u1ea1n";

                // ── BẢNG 1: CẢNH BÁO ──────────────────────────────────────────────
                var wHtml = "";
                if (warnings.length === 0) {
                    wHtml = "<div class='dk-lpcp-empty' style='text-align:center;padding:16px 0;'>" +
                        "<svg width='22' height='22' viewBox='0 0 24 24' fill='none' stroke='#22c55e' stroke-width='2' style='display:block;margin:0 auto 6px;'><path d='M22 11.08V12a10 10 0 1 1-5.93-9.14'/><polyline points='22 4 12 14.01 9 11.01'/></svg>" +
                        "Kh\u00f4ng c\u00f3 c\u1ea3nh b\u00e1o</div>";
                } else {
                    var WC = { critical: "#ef4444", caution: "#f59e0b", info: "#3b82f6" };
                    var WB = { critical: "rgba(239,68,68,0.15)", caution: "rgba(245,158,11,0.15)", info: "rgba(59,130,246,0.15)" };
                    warnings.forEach(function (w) {
                        wHtml += "<div style='display:flex;align-items:flex-start;gap:8px;padding:7px 9px;border-radius:7px;" +
                            "background:" + (WB[w.level] || "rgba(148,163,184,0.1)") + ";border-left:3px solid " + (WC[w.level] || "#94a3b8") + ";margin-bottom:6px;'>" +
                            "<div style='flex:1;min-width:0;'>" +
                            "<div style='display:flex;align-items:center;justify-content:space-between;gap:6px;margin-bottom:2px;'>" +
                            "<span style='font-size:11px;font-weight:600;color:var(--dk-title);white-space:nowrap;overflow:hidden;text-overflow:ellipsis;'>" +
                            escapeHtml(w.name) + "</span>" +
                            "<span style='font-size:10px;font-weight:700;background:" + (WC[w.level] || "#94a3b8") + "22;color:" + (WC[w.level] || "#94a3b8") + ";padding:1px 6px;border-radius:4px;flex-shrink:0;'>" +
                            escapeHtml(w.cnt) + "</span></div>" +
                            "<div style='font-size:10px;color:var(--dk-muted);line-height:1.4;'>" + escapeHtml(w.desc) + "</div>" +
                            "</div></div>";
                    });
                }
                if (warnEl) warnEl.innerHTML = wHtml;

                // ── BẢNG 2: TASK PENDING (Assignments) ────────────────────────────
                var aHtml = "";
                if (assignments.length === 0) {
                    aHtml = "<div class='dk-lpcp-empty'>Kh\u00f4ng c\u00f3 ph\u00e2n c\u00f4ng</div>";
                } else {
                    assignments.forEach(function (a) {
                        var name = escapeHtml(a.TenNV || a.MaNV || "\u2014");
                        var maLenh = escapeHtml(a.MaLenhSX || "");
                        var brand = a.TenBrand ? " \u00b7 " + escapeHtml(a.TenBrand) : (a.MaKhachHang ? " \u00b7 " + escapeHtml(a.MaKhachHang) : "");
                        var timeHtml = a.GioThucHien ? "<span style='font-size:10px;color:var(--dk-muted);margin-left:6px;'><i class='far fa-clock'></i> " + escapeHtml(a.GioThucHien) + "</span>" : "";
                        var descHtml = a.MoTaCongViec ? "<div style='font-size:10px;color:var(--dk-muted);margin-top:2px;'><i class='fas fa-tasks' style='margin-right:4px;opacity:0.7'></i>" + escapeHtml(a.MoTaCongViec) + "</div>" : "";

                        aHtml += "<div style='display:flex;align-items:flex-start;gap:8px;padding:7px 0;border-bottom:1px solid var(--dk-line);'>" +
                            "<div style='flex:1;min-width:0;'>" +
                            "<div style='font-size:11px;font-weight:600;color:var(--dk-primary,#2563eb);'>" + maLenh + brand + timeHtml + "</div>" +
                            "<div style='font-size:10px;color:var(--dk-title);margin-top:2px;opacity:0.9;'><i class='far fa-user' style='margin-right:4px;opacity:0.7'></i>" + name + "</div>" +
                            descHtml +
                            "</div>" + ttBadge(a.TrangThai || 0) + "</div>";
                    });
                }
                if (assignEl) assignEl.innerHTML = aHtml;

                // ── BẢNG 3: PHỤ LIỆU — SOẠN HÀNG (Pick Orders) ───────────────────
                var pHtml = "";
                if (pickOrders.length === 0) {
                    pHtml = "<div class='dk-lpcp-empty'>Kh\u00f4ng c\u00f3 l\u1ec7nh so\u1ea1n h\u00e0ng</div>";
                } else {
                    pickOrders.forEach(function (po) {
                        var maLenh = escapeHtml(po.MaLenhSX || "\u2014");
                        var brand = po.TenBrand ? escapeHtml(po.TenBrand) : (po.MaKhachHang ? escapeHtml(po.MaKhachHang) : "");
                        var nv = escapeHtml(po.TenNV || po.MaNV || "");
                        var thieu = (po.SoPLThieu || 0) > 0
                            ? "<span style='font-size:9px;font-weight:700;color:#dc2626;background:rgba(220,38,38,0.15);padding:1px 6px;border-radius:3px;margin-left:4px;'>\u26a0 Thi\u1ebfu " + po.SoPLThieu + "</span>"
                            : "";
                        var timeHtml = po.GioSoan ? "<span style='font-size:10px;color:var(--dk-muted);margin-left:6px;'><i class='far fa-clock'></i> " + escapeHtml(po.GioSoan) + "</span>" : "";
                        var slHtml = "";
                        if (po.SoLoaiPL > 0 || po.TongSLCanSoan > 0) {
                            slHtml = "<div style='font-size:10px;color:var(--dk-muted);margin-top:2px;'><i class='fas fa-box-open' style='margin-right:4px;opacity:0.7'></i>Soạn: <b>" + (po.SoLoaiPL || 0) + "</b> loại / <b>" + formatNumber(po.TongSLCanSoan || 0, 2) + "</b> SL</div>";
                        }

                        pHtml += "<div style='padding:7px 0;border-bottom:1px solid var(--dk-line);'>" +
                            "<div style='display:flex;align-items:flex-start;justify-content:space-between;gap:6px;'>" +
                            "<div><span style='font-size:11px;font-weight:600;color:var(--dk-primary,#2563eb);'>" + maLenh + "</span>" + timeHtml + "</div>" +
                            ttBadge(po.TrangThai || 0) + "</div>" +
                            (brand ? "<div style='font-size:10px;color:var(--dk-title);margin-top:2px;opacity:0.9;'>" + brand + "</div>" : "") +
                            (nv ? "<div style='font-size:10px;color:var(--dk-title);margin-top:2px;opacity:0.9;'><i class='far fa-user' style='margin-right:4px;opacity:0.7'></i>" + nv + "</div>" : "") +
                            slHtml +
                            (thieu ? "<div style='margin-top:4px'>" + thieu + "</div>" : "") + "</div>";
                    });
                }
                if (pickEl) pickEl.innerHTML = pHtml;

                // ── Bind "Xem chi tiết" buttons ───────────────────────────────────
                (function () {
                    var _warnings = warnings;
                    var _assignments = assignments;
                    var _pickOrders = pickOrders;
                    var _displayDate = displayDate;

                    function bindDetailBtn(btnId, section) {
                        var btn = document.getElementById(btnId);
                        if (!btn) return;
                        // Clone to remove any previous listener
                        var fresh = btn.cloneNode(true);
                        btn.parentNode.replaceChild(fresh, btn);
                        fresh.addEventListener("click", function () {
                            openLpcpSectionModal(section, _warnings, _assignments, _pickOrders, _displayDate);
                        });
                    }
                    bindDetailBtn("lpcpWarnDetailBtn", "warn");
                    bindDetailBtn("lpcpTaskDetailBtn", "task");
                    bindDetailBtn("lpcpPickDetailBtn", "pick");
                })();
            })
            .catch(function (err) {
                var errMsg = (err && err.message) ? err.message : "Lỗi kết nối";
                if (badgeEl) badgeEl.textContent = "Lỗi tải dữ liệu";
                if (assignEl) assignEl.innerHTML = "<div class='dk-lpcp-empty' style='color:#ef4444'>" + escapeHtml(errMsg) + "</div>";
                if (pickEl) pickEl.innerHTML = "<div class='dk-lpcp-empty' style='color:#ef4444'>Kiểm tra console để biết chi tiết</div>";
            });
    }

    // ── v2.3.61 — Chi tiết từng mục trong sidebar (Cảnh báo / Task / Pick) ──────
    function openLpcpSectionModal(section, warnings, assignments, pickOrders, dateLabel) {
        var modal = document.getElementById("detailModal");
        var titleEl = document.getElementById("detailModalTitle");
        var metaEl = document.getElementById("detailModalMeta");
        var contentEl = document.getElementById("detailModalContent");
        var headEl = modal ? modal.querySelector(".dk-modal-head") : null;
        if (!modal || !contentEl) return;

        // Hide search bar (not used here)
        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "none";

        // Apply section-specific header class
        if (headEl) {
            headEl.classList.remove("section-warn", "section-task", "section-pick");
            headEl.classList.add("section-" + section);
        }

        var TT_LABELS = ["Chờ TH", "Đang TH", "Hoàn thành", "Chưa HT"];
        var TT_SLUG = ["cho", "dang", "done", "chua"];

        function badge(tt) {
            var i = (tt >= 0 && tt <= 3) ? tt : 0;
            return "<span class='dk-lpcp-badge dk-lpcp-badge--" + TT_SLUG[i] + "'>" +
                "<span class='dk-lpcp-badge-dot'></span>" +
                escapeHtml(TT_LABELS[i]) + "</span>";
        }

        var COLS_TASK = "grid-template-columns:2fr 1.5fr 1.2fr 1.1fr;";
        var COLS_PICK = "grid-template-columns:2fr 1.2fr 1.5fr 80px 1.1fr;";

        var html = "";

        if (section === "warn") {
            if (titleEl) titleEl.textContent = "Cảnh Báo — " + dateLabel;
            if (metaEl) metaEl.innerHTML =
                "<span style='color:#dc2626;font-weight:600;'>" + warnings.length + " cảnh báo</span>";

            var WL = { critical: "Nghiêm trọng", caution: "Cần chú ý", info: "Thông tin" };

            if (warnings.length === 0) {
                html = "<div style='text-align:center;padding:40px;color:#64748b;'>Không có cảnh báo</div>";
            } else {
                warnings.forEach(function (w) {
                    var lvl = w.level || "info";
                    html += "<div class='dk-lpcp-warn-row level-" + lvl + "'>" +
                        "<div class='dk-lpcp-warn-body'>" +
                        "<div class='dk-lpcp-warn-title-row'>" +
                        "<span class='dk-lpcp-warn-name'>" + escapeHtml(w.name) + "</span>" +
                        "<span class='dk-lpcp-warn-cnt'>" + escapeHtml(w.cnt) + "</span>" +
                        "<span class='dk-lpcp-warn-severity'>" + escapeHtml(WL[lvl] || lvl) + "</span>" +
                        "</div>" +
                        "<div class='dk-lpcp-warn-desc'>" + escapeHtml(w.desc) + "</div>" +
                        "</div></div>";
                });
            }
        }
        else if (section === "task") {
            if (titleEl) titleEl.textContent = "Task Pending — " + dateLabel;
            if (metaEl) metaEl.innerHTML =
                "<span style='color:#1d4ed8;font-weight:600;'>" + assignments.length + " phân công</span>";

            if (assignments.length === 0) {
                html = "<div style='text-align:center;padding:40px;color:#64748b;'>Không có phân công</div>";
            } else {
                html += "<div class='dk-lpcp-modal-thead' style='" + COLS_TASK + "'>" +
                    "<span>Mã Lệnh SX</span><span>Nhân Viên</span><span>Brand / KH</span><span>Trạng Thái</span></div>";
                assignments.forEach(function (a) {
                    var maLenh = escapeHtml(a.MaLenhSX || "—");
                    var nv = escapeHtml(a.TenNV || a.MaNV || "—");
                    var brand = escapeHtml(a.TenBrand || a.MaKhachHang || "—");
                    var ngay = a.NgayThucHien
                        ? "<span class='dk-lpcp-col-date'> · " + escapeHtml(a.NgayThucHien) + "</span>"
                        : "";
                    html += "<div class='dk-lpcp-modal-row' style='" + COLS_TASK + "'>" +
                        "<span class='dk-lpcp-col-code'>" + maLenh + ngay + "</span>" +
                        "<span class='dk-lpcp-col-nv'>" + nv + "</span>" +
                        "<span class='dk-lpcp-col-brand'>" + brand + "</span>" +
                        badge(a.TrangThai || 0) + "</div>";
                });
            }
        }
        else if (section === "pick") {
            if (titleEl) titleEl.textContent = "Phụ Liệu — Soạn Hàng — " + dateLabel;
            if (metaEl) metaEl.innerHTML =
                "<span style='color:#15803d;font-weight:600;'>" + pickOrders.length + " lệnh soạn</span>";

            if (pickOrders.length === 0) {
                html = "<div style='text-align:center;padding:40px;color:#64748b;'>Không có lệnh soạn hàng</div>";
            } else {
                html += "<div class='dk-lpcp-modal-thead' style='" + COLS_PICK + "'>" +
                    "<span>Mã Lệnh SX</span><span>Brand</span><span>NV Soạn</span><span>PL Thiếu</span><span>Trạng Thái</span></div>";
                pickOrders.forEach(function (po) {
                    var maLenh = escapeHtml(po.MaLenhSX || "—");
                    var brand = escapeHtml(po.TenBrand || po.MaKhachHang || "—");
                    var nv = escapeHtml(po.TenNV || po.MaNV || "—");
                    var thieu = (po.SoPLThieu || 0) > 0
                        ? "<span class='dk-lpcp-thieu-yes'>⚠ " + po.SoPLThieu + "</span>"
                        : "<span class='dk-lpcp-thieu-no'>—</span>";
                    html += "<div class='dk-lpcp-modal-row' style='" + COLS_PICK + "'>" +
                        "<span class='dk-lpcp-col-code'>" + maLenh + "</span>" +
                        "<span class='dk-lpcp-col-nv'>" + brand + "</span>" +
                        "<span class='dk-lpcp-col-brand'>" + nv + "</span>" +
                        thieu +
                        badge(po.TrangThai || 0) + "</div>";
                });
            }
        }

        contentEl.innerHTML = html;
        modal.classList.add("open");
    }

    function getDayOfWeekVN(date) {
        var days = ["CN", "2", "3", "4", "5", "6", "7"];
        return days[date.getDay()] || "";
    }

    // v2.3.54 — Append "Phân công PL" tab vào modal sau 4 tabs kho
    function appendLpcpTab(container, lpcpData) {
        if (!container) return;
        var assignments = lpcpData.Assignments || [];
        var pickOrders = lpcpData.PickOrders || [];
        var total = assignments.length + pickOrders.length;

        var isDark = document.body.classList.contains("dark-theme");
        var TT_LABELS = ["Chờ thực hiện", "Đang thực hiện", "Hoàn thành", "Chưa hoàn thành"];
        var TT_COLORS = ["#d97706", "#3b82f6", "#10b981", "#ef4444"]; // Adjusted colors slightly for better contrast
        var TT_BG = isDark
            ? ["rgba(217,119,6,0.15)", "rgba(59,130,246,0.15)", "rgba(16,185,129,0.15)", "rgba(239,68,68,0.15)"]
            : ["#fffbeb", "#eff6ff", "#f0fdf4", "#fef2f2"];

        var cardBg = isDark ? "rgba(255,255,255,0.05)" : "#f8fafc";
        var borderColor = isDark ? "rgba(255,255,255,0.1)" : "#e2e8f0";
        var textColor = isDark ? "#f8fafc" : "#1a2332";
        var subColor = isDark ? "#94a3b8" : "#64748b";

        function ttBadge(tt) {
            return "<span style=\"display:inline-flex;align-items:center;gap:4px;" +
                "background:" + (TT_BG[tt] || TT_BG[0]) + ";color:" + (TT_COLORS[tt] || TT_COLORS[0]) + ";" +
                "padding:2px 8px;border-radius:4px;font-size:10px;font-weight:500;\">" +
                "<span style=\"width:5px;height:5px;border-radius:50%;background:" + (TT_COLORS[tt] || TT_COLORS[0]) + ";display:inline-block;\"></span>" +
                escapeHtml(TT_LABELS[tt] || "") + "</span>";
        }

        // Build tab button
        var tabBar = container.querySelector(".dk-day-tabs");
        if (!tabBar) return;
        var tabBtn = document.createElement("button");
        tabBtn.type = "button";
        tabBtn.className = "dk-day-tab";
        tabBtn.setAttribute("data-tabkey", "lpcp");
        tabBtn.style.setProperty("--tab-color", "#6366f1");
        tabBtn.innerHTML =
            "<span class=\"dk-day-tab-dot\" style=\"background:#6366f1\"></span>" +
            "Ph\u00e2n c\u00f4ng PL" +
            " <span class=\"dk-day-tab-count\">" + total + "</span>";
        tabBar.appendChild(tabBtn);

        // Build panel HTML
        var html = "";

        // Left: Phân công nhân viên
        html += "<div style=\"display:grid;grid-template-columns:1fr 1fr;gap:12px;padding:12px;\">";

        html += "<div>";
        html += "<div style=\"font-size:10px;font-weight:700;color:" + subColor + ";text-transform:uppercase;" +
            "letter-spacing:.05em;margin-bottom:8px;\">Phân công công việc</div>";
        if (assignments.length === 0) {
            html += "<div style=\"color:" + subColor + ";font-size:12px;text-align:center;padding:20px 0;\">Không có phân công</div>";
        } else {
            assignments.forEach(function (a, i) {
                var name = escapeHtml(a.TenNV || a.MaNV || "");
                var initials = name.replace(/NV\.|LH\./g, "").substring(0, 2).toUpperCase();
                var bgColors = ["#3b82f6", "#a855f7", "#14b8a6", "#f97316", "#22c55e", "#ef4444"];
                var bg = bgColors[(name.charCodeAt(0) || 0) % bgColors.length];
                html += "<div style=\"display:flex;align-items:center;gap:8px;padding:6px 10px;" +
                    "background:" + cardBg + ";border-radius:8px;border:1px solid " + borderColor + ";margin-bottom:6px;\">" +
                    "<span style=\"width:28px;height:28px;border-radius:50%;background:" + bg + ";" +
                    "display:inline-flex;align-items:center;justify-content:center;color:#fff;" +
                    "font-size:11px;font-weight:700;flex-shrink:0;\">" + initials + "</span>" +
                    "<div style=\"flex:1;overflow:hidden;\">" +
                    "<div style=\"font-size:12px;font-weight:600;color:" + textColor + ";\">" + name + "</div>" +
                    "<div style=\"font-size:10px;color:" + subColor + ";\">" + escapeHtml(a.MoTaCongViec || "") + "</div>" +
                    "</div>" +
                    ttBadge(a.TrangThai || 0) +
                    "</div>";
            });
        }
        html += "</div>";

        // Right: Phụ liệu soạn hàng
        html += "<div>";
        html += "<div style=\"font-size:10px;font-weight:700;color:" + subColor + ";text-transform:uppercase;" +
            "letter-spacing:.05em;margin-bottom:8px;\">Phụ liệu — Soạn hàng</div>";
        if (pickOrders.length === 0) {
            html += "<div style=\"color:" + subColor + ";font-size:12px;text-align:center;padding:20px 0;\">Không có lệnh soạn hàng</div>";
        } else {
            pickOrders.forEach(function (po) {
                var gioNgay = (po.GioSoan ? po.GioSoan + " " : "") +
                    (po.NgaySoan || "").replace(/(\d{4})-(\d{2})-(\d{2})/, "$3/$2/$1");
                var name = escapeHtml(po.TenNV || po.MaNV || "-");
                var initials2 = name.replace(/NV\.|LH\./g, "").substring(0, 2).toUpperCase();
                var bgColors = ["#3b82f6", "#a855f7", "#14b8a6", "#f97316", "#22c55e", "#ef4444"];
                var bg2 = bgColors[(name.charCodeAt(0) || 0) % bgColors.length];
                html += "<div style=\"display:flex;align-items:center;gap:8px;padding:6px 10px;" +
                    "background:" + cardBg + ";border-radius:8px;border:1px solid " + borderColor + ";margin-bottom:6px;\">" +
                    "<div style=\"flex:1;overflow:hidden;\">" +
                    "<div style=\"font-size:12px;\">" +
                    "<span style=\"font-weight:600;color:#3b82f6;\">Lệnh " + escapeHtml(po.MaLenhSX || "") + "</span>" +
                    "<span style=\"font-size:10px;color:" + subColor + ";margin-left:5px;\">" + escapeHtml(po.TenBrand || po.MaKhachHang || "") + "</span>" +
                    "</div>" +
                    "<div style=\"font-size:10px;color:" + subColor + ";display:flex;align-items:center;gap:5px;margin-top:2px;\">" +
                    "<span style=\"width:16px;height:16px;border-radius:50%;background:" + bg2 + ";" +
                    "display:inline-flex;align-items:center;justify-content:center;color:#fff;font-size:8px;font-weight:700;\">" + initials2 + "</span>" +
                    "<span>" + name + "</span><span>·</span><span>" + escapeHtml(gioNgay) + "</span>" +
                    "</div>" +
                    "</div>" +
                    ttBadge(po.TrangThai || 0) +
                    "</div>";
            });
        }
        html += "</div></div>"; // grid end

        // Create panel div
        var panel = document.createElement("div");
        panel.className = "dk-day-panel";
        panel.setAttribute("data-tabkey", "lpcp");
        panel.style.display = "none";
        panel.innerHTML = html;
        container.appendChild(panel);

        // Bind click for new tab button (re-use existing tab logic)
        tabBtn.addEventListener("click", function () {
            var allBtns = container.querySelectorAll(".dk-day-tab");
            for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
            tabBtn.classList.add("active");
            var allPanels = container.querySelectorAll(".dk-day-panel");
            for (var p = 0; p < allPanels.length; p++) {
                allPanels[p].style.display =
                    allPanels[p].getAttribute("data-tabkey") === "lpcp" ? "" : "none";
            }
        });
    }

    function colsNhap() {
        return [
            { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
            { key: "PINCC", label: "PI NCC", center: true, width: "10%" },
            { key: "PO", label: "POMUA", center: true, width: "9%" },
            { key: "ItemCode", label: "Itemcode", center: true, width: "10%" },
            { key: "MaMauVT", label: "Mã màu VT", center: true, width: "7%" },
            { key: "MauVT", label: "Màu", center: true, width: "8%" },
            { key: "WidthSize", label: "Width/Size", center: true, width: "8%" },
            { key: "DonViVT", label: "Đơn vị vật tư", center: true, width: "7%" },
            { key: "TenKH", label: "Khách hàng", center: true, width: "20%" },
            { key: "SoLuong", label: "Số lượng", number: 2, center: true, width: "8%" },
            { key: "SoBarCode", label: "Số BarCode", number: 0, center: true, width: "8%" }
        ];
    }
    function colsXuat() {
        return [
            { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
            { key: "MaLenh", label: "Mã lệnh", center: true, width: "15%" },
            { key: "TenHang", label: "Tên hàng", center: true, width: "30%" },
            { key: "TenKH", label: "Khách hàng", center: true, width: "25%" },
            { key: "SoLuong", label: "Số lượng", number: 2, center: true, width: "12%" },
            { key: "SoBarCode", label: "Số BarCode", number: 0, center: true, width: "13%" }
        ];
    }
    function colsKiemKe() {
        return [
            { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
            { key: "PhieuKiemKe", label: "Phiếu KK", center: true, width: "15%" },
            { key: "SoLo", label: "Số lô", center: true, width: "15%" },
            { key: "SoBarCode", label: "Số BarCode", number: 0, center: true, width: "15%" },
            { key: "SoLuong", label: "SL kiểm kê", number: 2, center: true, width: "15%" },
            { key: "UserKK", label: "Người KK", center: true, width: "15%" },
            { key: "GhiChu", label: "Ghi chú", center: true, width: "20%" }
        ];
    }
    function colsPlanned() {
        return [
            { key: "STT", label: "STT", number: 0, center: true, width: "5%" },
            { key: "SoLo", label: "Số lô", center: true, width: "15%" },
            { key: "PO", label: "POMUA", center: true, width: "12%" },
            { key: "MaKH", label: "Mã KH", center: true, width: "10%" },
            { key: "TenKH", label: "Khách hàng", center: true, width: "25%" },
            { key: "SoLuongDuKien", label: "Số lượng dự kiến", number: 0, center: true, width: "18%" },
            { key: "NgayNKDuKien", label: "Ngày dự kiến", date: true, center: true, width: "15%" }
        ];
    }

    function renderAll() {
        renderMetricCards();
        renderMoMStrip();
        populateCustomerFilter();
        renderCharts();
        renderCustomersTable();
        renderRacksTable();
        // renderRacksHeatmap();
        renderActivityCalendar();
        renderActivityCalendarMonthly();
        setTimeout(injectMaximizeButtons, 80);

        // v2.3.6 — Sau khi render lần đầu (có thể state.activityCalendar
        // rỗng do API lỗi hoặc khoảng ngày mặc định không cover được tháng
        // hiện tại), thử gọi lại API với khoảng ngày tường minh cho ±1 tháng.
        if (!isDemoMode && (!state.activityCalendar || state.activityCalendar.length === 0)) {
            var now = new Date();
            var from = new Date(now.getFullYear(), now.getMonth() - 1, 1);
            var to = new Date(now.getFullYear(), now.getMonth() + 2, 0);
            var url = "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" +
                asIsoDate(from) + "&denNgay=" + asIsoDate(to);
            requestJson(url).then(function (data) {
                state.activityCalendar = normalizeArray(data);
                renderActivityCalendarMonthly();
            }).catch(function () { });
        }
    }

    function renderRacksHeatmap() {
        var container = byId("warehouseMapContainer");
        if (!container) return;
        if (!state.racks || state.racks.length === 0) {
            container.innerHTML = "<div class=\"dk-empty\">Không có dữ liệu kệ kho</div>";
            return;
        }

        var modules = {};
        var moduleOrder = [];
        // NL (1) trước, PL (2) sau
        for (var i = 0; i < state.racks.length; i++) {
            var r = state.racks[i];
            var mod = toNumber(r.Module);
            var mKey = mod === 1 ? 1 : (mod === 2 ? 2 : 3);
            var mName = (mod === 1) ? "Kho Nguyên Liệu (NL)" : (mod === 2 ? "Kho Phụ Liệu (PL)" : "Module Khác");
            if (!modules[mKey]) { modules[mKey] = { name: mName, racks: [] }; moduleOrder.push(mKey); }
            modules[mKey].racks.push({ item: r, index: i });
        }
        moduleOrder.sort(function (a, b) { return a - b; });

        var html = "<div class=\"dk-wh-grid-wrap\">";

        for (var mi = 0; mi < moduleOrder.length; mi++) {
            var modKey = moduleOrder[mi];
            var modObj = modules[modKey];
            var racks = modObj.racks;

            racks.sort(function (a, b) {
                // Sắp xếp theo dãy trước, rồi theo % sử dụng
                var dayA = (a.item.TenDay || "").toLowerCase();
                var dayB = (b.item.TenDay || "").toLowerCase();
                if (dayA !== dayB) return dayA < dayB ? -1 : 1;
                var capA = toNumber(a.item.TongCBMTrongKe);
                var capB = toNumber(b.item.TongCBMTrongKe);
                var pA = capA > 0 ? (toNumber(a.item.TongCBMSuDungTrongKe) / capA) * 100 : 0;
                var pB = capB > 0 ? (toNumber(b.item.TongCBMSuDungTrongKe) / capB) * 100 : 0;
                return pB - pA;
            });

            // Thống kê module
            var totalRacks = racks.length;
            var under50 = 0, between50and85 = 0, above85 = 0, over100 = 0;
            var totalUsed = 0, totalCap = 0;
            for (var k = 0; k < racks.length; k++) {
                var ri = racks[k].item;
                var usd = toNumber(ri.TongCBMSuDungTrongKe);
                var cp = toNumber(ri.TongCBMTrongKe);
                var pt = cp > 0 ? (usd / cp) * 100 : 0;
                totalUsed += usd;
                totalCap += cp;
                if (pt > 100) over100++;
                else if (pt >= 85) above85++;
                else if (pt >= 50) between50and85++;
                else under50++;
            }
            var modPct = totalCap > 0 ? (totalUsed / totalCap) * 100 : 0;

            html += "<div class=\"dk-wh-module-section\">";
            html += "<div class=\"dk-wh-module-title\">" + escapeHtml(modObj.name) +
                "<span class=\"dk-wh-module-stats\">" +
                formatNumber(modPct, 1) + "% lấp đầy &nbsp;|&nbsp; " +
                under50 + " kệ &lt;50% &nbsp;|&nbsp; " +
                above85 + " kệ &gt;85% &nbsp;|&nbsp; " +
                over100 + " kệ &gt;100%" +
                "</span></div>";

            // Nhóm theo dãy
            var dayGroups = {};
            var dayOrder = [];
            for (var j2 = 0; j2 < racks.length; j2++) {
                var rk = racks[j2];
                var dayName = rk.item.TenDay || "Không xác định";
                if (!dayGroups[dayName]) { dayGroups[dayName] = []; dayOrder.push(dayName); }
                dayGroups[dayName].push(rk);
            }
            // Remove duplicates in dayOrder
            var seenDays = {};
            dayOrder = dayOrder.filter(function (d) { return seenDays[d] ? false : (seenDays[d] = true); });

            html += "<div class=\"dk-wh-days-grid\">";
            for (var di = 0; di < dayOrder.length; di++) {
                var dayName2 = dayOrder[di];
                var dayRacks = dayGroups[dayName2];
                html += "<div class=\"dk-wh-day-section\">";
                html += "<div class=\"dk-bg-soft dk-text-body\" style=\"font-size:11px;font-weight:700;margin-bottom:4px\">" + escapeHtml(dayName2) + "</div>";
                html += "<div class=\"dk-wh-tile-grid\">";

                for (var j = 0; j < dayRacks.length; j++) {
                    var rackData = dayRacks[j];
                    var rItem = rackData.item;
                    var rIndex = rackData.index;
                    var used = toNumber(rItem.TongCBMSuDungTrongKe);
                    var cap = toNumber(rItem.TongCBMTrongKe);
                    var pct = cap > 0 ? (used / cap) * 100 : 0;
                    var slVatTu = toNumber(rItem.SLVatTu);

                    var tileClass = "dk-wh-safe";
                    if (pct > 100) tileClass = "dk-wh-over";
                    else if (pct >= 85) tileClass = "dk-wh-full";
                    else if (pct >= 50) tileClass = "dk-wh-warn";

                    var tooltipText = [
                        "Kệ: " + (rItem.TenKe || ""),
                        "Dãy: " + (rItem.TenDay || ""),
                        "Module: " + (rItem.Module == 1 ? "NL" : (rItem.Module == 2 ? "PL" : rItem.Module)),
                        "Sử dụng: " + formatNumber(used, 1) + " / " + formatNumber(cap, 1) + " CBM",
                        "Tỷ lệ: " + formatNumber(pct, 1) + "%",
                        "Vật tư: " + formatNumber(slVatTu, 0) + " mã"
                    ].join("\n");

                    // v2.4.16 — Tile với fill animation từ dưới lên
                    var fillH = Math.min(100, Math.max(0, pct));
                    var staggerDelay = (j * 40);
                    html += "<div class=\"dk-wh-tile " + tileClass + " js-open-detail\" data-detail=\"rackRow\" data-index=\"" + rIndex + "\" data-pct=\"" + fillH.toFixed(1) + "\" title=\"" + escapeHtml(tooltipText) + "\" style=\"animation-delay:" + staggerDelay + "ms\">";
                    html += "<span class=\"dk-wh-tile-fill\" style=\"height:" + fillH.toFixed(1) + "%\"></span>";
                    if (pct >= 100) html += "<i class=\"fa-solid fa-triangle-exclamation dk-wh-tile-warn-icon\"></i>";
                    html += "<span class=\"dk-wh-tile-name\">" + escapeHtml(rItem.TenKe || "Kệ") + "</span>";
                    html += "<span class=\"dk-wh-tile-pct\">" + formatNumber(pct, 0) + "%</span>";
                    html += "<span class=\"dk-wh-tile-sl\">" + formatNumber(slVatTu, 0) + " mã</span>";
                    html += "</div>";
                }
                html += "</div></div>";
            }
            html += "</div>"; // close dk-wh-days-grid

            html += "</div>";
        }

        html += "</div>";
        container.innerHTML = html;
    }

    // ─── Pagination System ────────────────────────────────────────────────────
    var currentPage = 1;
    var autoRotateTimer = null;
    var AUTO_ROTATE_INTERVAL = 15000; // 15 giây

    function switchPage(pageNum) {
        currentPage = pageNum;
        // v2.3.8 — lazy load data for page being shown (Page 2/3 chỉ fetch khi vào)
        try { loadPageData(pageNum); } catch (e) { }
        var pages = document.querySelectorAll(".dk-page");
        // v2.6.0 — Support both old .dk-page-btn and new .dk-topbar-nav-btn
        var btns = document.querySelectorAll(".dk-page-btn, .dk-topbar-nav-btn");
        for (var i = 0; i < pages.length; i++) {
            var pn = parseInt(pages[i].getAttribute("data-page"), 10);
            if (pn === pageNum) {
                pages[i].classList.add("dk-page-active");
                pages[i].style.display = "flex"; // [FIX] Ép hiển thị trang hiện tại dưới dạng flexbox
            } else {
                pages[i].classList.remove("dk-page-active");
                pages[i].style.display = "none";  // [FIX] Ép ẩn hoàn toàn trang khác
            }
        }

        // Hide global search bar on Page 3 (Bảng thống kê)
        var gsInput = document.getElementById("dkGlobalSearch");
        if (gsInput) {
            var gsWrap = gsInput.closest ? gsInput.closest(".dk-topbar-search, .input-group, .dk-search-wrap") : gsInput.parentElement;
            if (!gsWrap) gsWrap = gsInput.parentElement;
            if (gsWrap) gsWrap.style.display = (pageNum === 3) ? "none" : "";
        }
        for (var j = 0; j < btns.length; j++) {
            var bn = parseInt(btns[j].getAttribute("data-page"), 10);
            if (bn === pageNum) {
                btns[j].classList.add("active");
            } else {
                btns[j].classList.remove("active");
            }
        }

        if (pageNum === 2) {
            setTimeout(function () {
                try { renderAlertsList(); } catch (e) { }
                try { renderHieuSuatGauges(); } catch (e) { }
                try { renderTop5MaxChart(); } catch (e) { }
                try { renderAgeStockChart(); } catch (e) { }
            }, 50);
        }

        if (typeof Highcharts !== "undefined") {
            setTimeout(function () {
                var charts = Highcharts.charts || [];
                for (var c = 0; c < charts.length; c++) {
                    if (charts[c]) charts[c].reflow();
                }
            }, 100);
        }

        var topbar = document.querySelector(".dk-topbar");
        if (topbar) {
            if (pageNum === 3) {
                topbar.classList.add("filter-hidden");
            } else {
                topbar.classList.remove("filter-hidden");
            }
        }
    }

    function startAutoRotate() {
        stopAutoRotate();
        autoRotateTimer = setInterval(function () {
            var next = currentPage >= 3 ? 1 : currentPage + 1;
            switchPage(next);
        }, AUTO_ROTATE_INTERVAL);
    }

    function stopAutoRotate() {
        if (autoRotateTimer) {
            clearInterval(autoRotateTimer);
            autoRotateTimer = null;
        }
    }

    function bindPageNav() {
        var nav = byId("pageNav");
        if (!nav) return;
        nav.addEventListener("click", function (e) {
            // v2.6.0 — Match both old sidebar buttons and new topbar nav buttons
            var btn = e.target.closest(".dk-page-btn, .dk-topbar-nav-btn");
            if (!btn) return;
            var page = parseInt(btn.getAttribute("data-page"), 10);
            if (page) {
                switchPage(page);
            }
        });
    }

    function getDetailData(detail, index) {
        var overall = state.overall.length > 0 ? state.overall[0] : {};
        var totalCapRow = {
            TotalCapacity: toNumber(overall.TotalCapacity),
            CapacityNPL: toNumber(overall.CapacityNPL),
            CapacityPL: toNumber(overall.CapacityPL),
            UsedNPL: toNumber(overall.UsedNPL),
            UsedPL: toNumber(overall.UsedPL),
            UsedTotal: toNumber(overall.UsedNPL) + toNumber(overall.UsedPL),
            TotalFreeCapacity: toNumber(overall.TotalFreeCapacity),
            TotalVatTu: toNumber(overall.TotalVatTu),
            PercentNPL: toNumber(overall.PercentNPL),
            PercentPL: toNumber(overall.PercentPL),
            TotalPercent: toNumber(overall.TotalPercent),
            FreePercent: toNumber(overall.FreePercent)
        };

        // Chi tiết tổng sức chứa: hiện tất cả kệ, NL trước rồi PL
        var racksForCapDetail = state.racks.slice().sort(function (a, b) {
            var mA = toNumber(a.Module), mB = toNumber(b.Module);
            if (mA !== mB) return mA - mB;
            return (a.TenDay || "").localeCompare(b.TenDay || "");
        }).map(function (r, idx) {
            var used = toNumber(r.TongCBMSuDungTrongKe);
            var cap = toNumber(r.TongCBMTrongKe);
            return {
                STT: idx + 1,
                LoaiKho: r.Module == 1 ? "NL" : (r.Module == 2 ? "PL" : String(r.Module || "")),
                TenDay: r.TenDay || "",
                TenKe: r.TenKe || "",
                CBMSuDung: used,
                TongCBM: cap,
                PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                SLVatTu: toNumber(r.SLVatTu)
            };
        });

        var detailMap = {
            totalCapacity: {
                title: "Chi tiết tổng sức chứa — theo kệ",
                isWarehouseMap: true,
                rows: racksForCapDetail,
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "LoaiKho", label: "Loại kho", center: true, width: 85 },
                    { key: "TenDay", label: "Dãy", center: true, width: 80 },
                    { key: "TenKe", label: "Tên kệ", center: true, width: 95 },
                    { key: "CBMSuDung", label: "CBM sử dụng", number: 2, width: 110 },
                    { key: "TongCBM", label: "Tổng CBM", number: 2, width: 110 },
                    { key: "PctSuDung", label: "% sử dụng", percent: true, width: 95 },
                    { key: "SLVatTu", label: "Số vật tư", number: 0, center: true, width: 85 }
                ]
            },
            materialCount: (function () {
                var soMaVatTu = state.distinctMat && state.distinctMat.length > 0 ? toNumber(state.distinctMat[0].SoMaVatTu) : 0;
                var totalRacks = state.racks.length;
                var racksAbove85 = 0, racksOver100 = 0, racksUnder50 = 0;
                var totalUsedCBM = 0;
                for (var _ri = 0; _ri < state.racks.length; _ri++) {
                    var _r = state.racks[_ri];
                    var _used = toNumber(_r.TongCBMSuDungTrongKe);
                    var _cap = toNumber(_r.TongCBMTrongKe);
                    totalUsedCBM += _used;
                    var _pct = _cap > 0 ? (_used / _cap) * 100 : 0;
                    if (_pct > 100) racksOver100++;
                    else if (_pct >= 85) racksAbove85++;
                    else if (_pct < 50) racksUnder50++;
                }
                var rows = [
                    { ChiSo: "Mã vật tư", GiaTri: soMaVatTu, drill: "matCountDrill_codes" },
                    { ChiSo: "Số lượng tồn NL", GiaTri: toNumber(overall.TotalVatTuNPL) },
                    { ChiSo: "Số lượng tồn PL", GiaTri: toNumber(overall.TotalVatTuPL) },
                    { ChiSo: "Tổng số lượng tồn", GiaTri: toNumber(overall.TotalVatTu) },
                    { ChiSo: "Tổng số kệ đang dùng", GiaTri: totalRacks, drill: "matCountDrill_allRacks" },
                    { ChiSo: "Kệ < 50% lấp đầy", GiaTri: racksUnder50, drill: "matCountDrill_under50" },
                    { ChiSo: "Kệ 85% - 100% lấp đầy", GiaTri: racksAbove85, drill: "matCountDrill_85to100" },
                    { ChiSo: "Kệ > 100% (vượt tải)", GiaTri: racksOver100, drill: "matCountDrill_over100" },
                    { ChiSo: "CBM đang sử dụng", GiaTri: totalUsedCBM, isDecimal: true }
                ];
                return {
                    title: "Chi tiết tổng hợp vật tư trong kho",
                    rows: rows.map(function (r, i) {
                        var formattedValue = r.isDecimal ? formatNumber(r.GiaTri, 2) : formatNumber(r.GiaTri, 0);
                        var canDrill = !!r.drill;
                        return {
                            STT: i + 1,
                            ChiSo: r.ChiSo,
                            GiaTri: canDrill
                                ? '<a href="#" class="dk-link js-open-detail" data-detail="' + r.drill + '">' + formattedValue + '</a>'
                                : formattedValue,
                            _raw: r
                        };
                    }),
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "ChiSo", label: "Chỉ số", width: 250 },
                        { key: "GiaTri", label: "Giá trị", raw: true, center: true, width: 130 }
                    ]
                };
            })(),
            matCountDrill_codes: {
                title: "Danh sách tất cả mã vật tư đang tồn kho",
                rows: [],
                // v2.4.0 — Cột mới: STT, Loại, ItemCode, Chi tiết, Màu(mã+tên), Khổ vải(+đv), Tồn kho, Số kiện/roll
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "LoaiKho", label: "Loại", center: true, width: 70 },
                    { key: "ItemCode", label: "ItemCode", width: 120 },
                    { key: "ChiTiet", label: "Chi tiết" },
                    { key: "MauDisplay", label: "Màu", raw: true, width: 160 },
                    { key: "KhoVai", label: "Khổ vải", center: true, width: 100 },
                    { key: "TonKho", label: "Tồn kho", number: 2, width: 100 },
                    { key: "SoBarCode", label: "Số kiện/roll", number: 0, center: true, width: 100 }
                ],
                customAsync: "allMaterials"
            },
            // v2.4.4 — 6 modal "Xem chi tiết" mới: shell + customAsync dispatcher
            todoDetail: { title: "Công việc chờ xử lý", rows: [], columns: [], customAsync: "todoDetail" },
            top5VTAll: { title: "Toàn bộ vật tư theo dung tích", rows: [], columns: [], customAsync: "top5VTAll" },
            top5KHAll: { title: "Toàn bộ khách hàng theo giá trị tồn", rows: [], columns: [], customAsync: "top5KHAll" },
            hetHanAll: { title: "Vật tư sắp hết hạn", rows: [], columns: [], customAsync: "hetHanAll" },
            giaTriNhomAll: { title: "Chi tiết giá trị tồn theo nhóm", rows: [], columns: [], customAsync: "giaTriNhomAll" },
            kiemKeAll: { title: "Chi tiết kiểm kê", rows: [], columns: [], customAsync: "kiemKeAll" },
            // v2.4.15 — 5 modal KPI header
            tonDauKyDetail: { title: "Tồn đầu kỳ", rows: [], columns: [], customAsync: "tonDauKyDetail" },
            tongNhapDetail: { title: "Chi tiết nhập kho", rows: [], columns: [], customAsync: "tongNhapDetail" },
            tongXuatDetail: { title: "Chi tiết xuất kho", rows: [], columns: [], customAsync: "tongXuatDetail" },
            tonKhoDetail: { title: "Chi tiết tồn kho", rows: [], columns: [], customAsync: "tonKhoDetail" },
            poTreDetail: { title: "PO đang trễ", rows: [], columns: [], customAsync: "poTreDetail" },
            // v2.4.16 — chi tiết cảnh báo tồn kho (theo MaCB)
            alertDetail: { title: "Chi tiết cảnh báo", rows: [], columns: [], customAsync: "alertDetail" },
            matCountDrill_allRacks: (function () {
                var rows = state.racks.map(function (r, i) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: i + 1,
                        Module: r.Module == 1 ? "NL" : "PL",
                        TenDay: r.TenDay || "",
                        TenKe: r.TenKe || "",
                        CBMSuDung: used, TongCBM: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu)
                    };
                });
                return {
                    title: "Danh sách kệ đang sử dụng",
                    rows: rows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true },
                        { key: "Module", label: "Loại kho", center: true },
                        { key: "TenDay", label: "Dãy", center: true },
                        { key: "TenKe", label: "Tên kệ", center: true },
                        { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                        { key: "TongCBM", label: "Tổng CBM", number: 2 },
                        { key: "PctSuDung", label: "% sử dụng", percent: true },
                        { key: "SLVatTu", label: "Số vật tư", number: 0, center: true }
                    ]
                };
            })(),
            matCountDrill_under50: (function () {
                var rows = state.racks.filter(function (r) {
                    var cap = toNumber(r.TongCBMTrongKe);
                    var pct = cap > 0 ? (toNumber(r.TongCBMSuDungTrongKe) / cap) * 100 : 0;
                    return pct < 50;
                }).map(function (r, i) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: i + 1, Module: r.Module == 1 ? "NL" : "PL",
                        TenDay: r.TenDay || "", TenKe: r.TenKe || "",
                        CBMSuDung: used, TongCBM: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu)
                    };
                });
                return {
                    title: "Danh sách kệ < 50% lấp đầy (còn trống nhiều)",
                    rows: rows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true },
                        { key: "Module", label: "Loại kho", center: true },
                        { key: "TenDay", label: "Dãy", center: true },
                        { key: "TenKe", label: "Tên kệ", center: true },
                        { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                        { key: "TongCBM", label: "Tổng CBM", number: 2 },
                        { key: "PctSuDung", label: "% sử dụng", percent: true },
                        { key: "SLVatTu", label: "Số vật tư", number: 0, center: true }
                    ]
                };
            })(),
            matCountDrill_85to100: (function () {
                var rows = state.racks.filter(function (r) {
                    var cap = toNumber(r.TongCBMTrongKe);
                    var pct = cap > 0 ? (toNumber(r.TongCBMSuDungTrongKe) / cap) * 100 : 0;
                    return pct >= 85 && pct <= 100;
                }).map(function (r, i) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: i + 1, Module: r.Module == 1 ? "NL" : "PL",
                        TenDay: r.TenDay || "", TenKe: r.TenKe || "",
                        CBMSuDung: used, TongCBM: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu)
                    };
                });
                return {
                    title: "Danh sách kệ 85% - 100% lấp đầy (gần đầy)",
                    rows: rows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true },
                        { key: "Module", label: "Loại kho", center: true },
                        { key: "TenDay", label: "Dãy", center: true },
                        { key: "TenKe", label: "Tên kệ", center: true },
                        { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                        { key: "TongCBM", label: "Tổng CBM", number: 2 },
                        { key: "PctSuDung", label: "% sử dụng", percent: true },
                        { key: "SLVatTu", label: "Số vật tư", number: 0, center: true }
                    ]
                };
            })(),
            matCountDrill_over100: (function () {
                var rows = state.racks.filter(function (r) {
                    var cap = toNumber(r.TongCBMTrongKe);
                    var pct = cap > 0 ? (toNumber(r.TongCBMSuDungTrongKe) / cap) * 100 : 0;
                    return pct > 100;
                }).map(function (r, i) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: i + 1, Module: r.Module == 1 ? "NL" : "PL",
                        TenDay: r.TenDay || "", TenKe: r.TenKe || "",
                        CBMSuDung: used, TongCBM: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu)
                    };
                });
                return {
                    title: "Danh sách kệ > 100% (vượt tải - CẢNH BÁO)",
                    rows: rows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true },
                        { key: "Module", label: "Loại kho", center: true },
                        { key: "TenDay", label: "Dãy", center: true },
                        { key: "TenKe", label: "Tên kệ", center: true },
                        { key: "CBMSuDung", label: "CBM sử dụng", number: 2 },
                        { key: "TongCBM", label: "Tổng CBM", number: 2 },
                        { key: "PctSuDung", label: "% sử dụng", percent: true },
                        { key: "SLVatTu", label: "Số vật tư", number: 0, center: true }
                    ]
                };
            })(),
            capacitySummary: {
                title: "Tóm tắt sức chứa kho",
                rows: [(function () {
                    var soMaVatTu2 = state.distinctMat && state.distinctMat.length > 0 ? toNumber(state.distinctMat[0].SoMaVatTu) : toNumber(overall.TotalVatTu);
                    return {
                        TotalCapacity: toNumber(overall.TotalCapacity),
                        UsedTotal: toNumber(overall.UsedNPL) + toNumber(overall.UsedPL),
                        TotalFreeCapacity: toNumber(overall.TotalFreeCapacity),
                        TotalPercent: toNumber(overall.TotalPercent),
                        FreePercent: toNumber(overall.FreePercent),
                        CapacityNPL: toNumber(overall.CapacityNPL),
                        UsedNPL: toNumber(overall.UsedNPL),
                        PercentNPL: toNumber(overall.PercentNPL),
                        CapacityPL: toNumber(overall.CapacityPL),
                        UsedPL: toNumber(overall.UsedPL),
                        PercentPL: toNumber(overall.PercentPL),
                        SoMaVatTu: soMaVatTu2
                    };
                })()],
                columns: [
                    { key: "TotalCapacity", label: "Tổng sức chứa (CBM)", number: 2 },
                    { key: "UsedTotal", label: "CBM đã sử dụng", number: 2 },
                    { key: "TotalFreeCapacity", label: "CBM còn trống", number: 2 },
                    { key: "TotalPercent", label: "% lấp đầy tổng", percent: true },
                    { key: "FreePercent", label: "% còn trống", percent: true },
                    { key: "CapacityNPL", label: "Sức chứa NL (CBM)", number: 2 },
                    { key: "UsedNPL", label: "NL đã dùng (CBM)", number: 2 },
                    { key: "PercentNPL", label: "% NL sử dụng", percent: true },
                    { key: "CapacityPL", label: "Sức chứa PL (CBM)", number: 2 },
                    { key: "UsedPL", label: "PL đã dùng (CBM)", number: 2 },
                    { key: "PercentPL", label: "% PL sử dụng", percent: true },
                    { key: "SoMaVatTu", label: "Số mã vật tư", number: 0 }
                ]
            },
            inboundReady: {
                title: "Danh sách lệnh chuẩn bị về",
                rows: state.inbound.map(function (r, i) { return Object.assign({ STT: i + 1 }, r); }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "PO", label: "Số PO", center: true, width: 140 },
                    { key: "SoLo", label: "Số lô", center: true, width: 120 },
                    { key: "MaDH", label: "Mã đơn hàng", center: true, width: 120 },
                    { key: "TenKH", label: "Khách hàng", width: 180 },
                    { key: "MaHang", label: "Mã hàng", center: true, width: 150 },
                    { key: "TenHang", label: "Tên hàng", width: 220 },
                    { key: "SoLuongSP", label: "Số lượng SP", number: 0, center: true, width: 100 },
                    { key: "NgayNKDuKien", label: "Ngày dự kiến về", date: true, center: true, width: 120 }
                ]
            },
            outboundReady: {
                title: "Danh sách lệnh chuẩn bị xuất",
                rows: state.outboundReady.map(function (r, i) { return Object.assign({ STT: i + 1 }, r); }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    // v2.3.31 — Chỉ giữ "Mã lệnh SX" = MaLenh từ CanDoiDonViSanXuat, bỏ cột nội bộ
                    { key: "MaLenh", label: "Mã lệnh SX", center: true, width: 120 },
                    { key: "KhachHang", label: "Khách hàng", width: 180 },
                    { key: "TenHang", label: "Tên hàng", width: 250 },
                    { key: "SoLuongYeuCau", label: "SL chuẩn bị xuất", number: 2, width: 120 },
                    { key: "KHCat", label: "KH cắt", date: true, center: true, width: 100 },
                    { key: "DuKienCat", label: "Dự kiến cắt", date: true, center: true, width: 100 }
                ]
            },
            outboundRunning: {
                title: "Danh sách lệnh đang xuất",
                rows: state.outboundRunning.map(function (r, i) { return Object.assign({ STT: i + 1 }, r); }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    // v2.3.46 — MaLenh = PhieuXuatHang.MaLenh (integer như 539), header giữ "Mã lệnh SX"
                    { key: "MaLenh", label: "Mã lệnh SX", center: true, width: 110 },
                    { key: "PhieuDK", label: "Phiếu ĐK", center: true, width: 110 },
                    { key: "MaGop", label: "Mã gộp", center: true, width: 110 },
                    { key: "TenHang", label: "Tên hàng", width: 220 },
                    { key: "TenKH", label: "Khách hàng", width: 150 },
                    { key: "NgayXuatHang", label: "Ngày xuất", date: true, center: true, width: 100 },
                    { key: "SoLuongYeuCau", label: "SL yêu cầu", number: 2, width: 110 },
                    { key: "SLXuat", label: "SL đã xuất", number: 2, width: 110 },
                    { key: "PctDaXuat", label: "% đã xuất", percent: true, width: 90 }
                ]
            },
            customersAll: {
                title: "Chi tiết khách hàng theo dung tích sử dụng",
                rows: (function () {
                    try {
                        return buildCustomerDetailRows().map(function (r, i) { return Object.assign({ STT: i + 1 }, r); });
                    } catch (e) { return []; }
                })(),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "MaKH", label: "Mã KH", center: true, width: 110 },
                    { key: "TenKH", label: "Tên khách hàng", width: 250 },
                    { key: "SLVatTu", label: "Số vật tư", number: 0, center: true, width: 100 },
                    { key: "CBMSDTrongKho", label: "CBM sử dụng", number: 2, width: 120 },
                    { key: "TyTrongCBM", label: "Tỷ trọng CBM (%)", percent: true, width: 120 }
                ]
            },
            top15MaxNL: (function () {
                var rows = (state.top15MaxNL || []).slice(0, 15).map(function (r, i) {
                    return Object.assign({ STT: i + 1 }, r);
                });
                return {
                    title: "Chi tiết Top 15 NL tồn kho nhiều nhất",
                    rows: rows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "MaVT", label: "Mã vật tư", center: true, width: 140 },
                        { key: "TenVT", label: "Tên vật tư", width: 250 },
                        { key: "Mau", label: "Màu", center: true, width: 80 },
                        { key: "KhoVai", label: "Khổ vải", center: true, width: 80 },
                        { key: "ChiTiet", label: "Chi tiết", width: 180 },
                        { key: "TenDVVT", label: "Đơn vị", center: true, width: 60 },
                        { key: "TonKho", label: "Tồn kho", number: 2, width: 110 }
                    ]
                };
            })(),
            top15MaxPL: (function () {
                var rows = (state.top15MaxPL || []).slice(0, 15).map(function (r, i) {
                    return Object.assign({ STT: i + 1 }, r);
                });
                return {
                    title: "Chi tiết Top 15 PL tồn kho nhiều nhất",
                    rows: rows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "MaVT", label: "Mã vật tư", center: true, width: 140 },
                        { key: "TenVT", label: "Tên vật tư", width: 250 },
                        { key: "Mau", label: "Màu", center: true, width: 80 },
                        { key: "KhoVai", label: "Khổ vải", center: true, width: 80 },
                        { key: "ChiTiet", label: "Chi tiết", width: 180 },
                        { key: "TenDVVT", label: "Đơn vị", center: true, width: 60 },
                        { key: "TonKho", label: "Tồn kho", number: 2, width: 110 }
                    ]
                };
            })(),
            racksAll: {
                title: "Chi tiết danh sách kệ kho (NL → PL)",
                rows: state.racks.slice().sort(function (a, b) {
                    var mA = toNumber(a.Module), mB = toNumber(b.Module);
                    if (mA !== mB) return mA - mB;
                    return (a.TenDay || "").localeCompare(b.TenDay || "");
                }).map(function (r, idx) {
                    var used = toNumber(r.TongCBMSuDungTrongKe);
                    var cap = toNumber(r.TongCBMTrongKe);
                    return {
                        STT: idx + 1,
                        Module: r.Module == 1 ? "NL" : (r.Module == 2 ? "PL" : String(r.Module || "")),
                        TenDay: r.TenDay || "",
                        TenKe: r.TenKe || "",
                        TongCBMSuDungTrongKe: used,
                        TongCBMTrongKe: cap,
                        PctSuDung: cap > 0 ? (used / cap) * 100 : 0,
                        SLVatTu: toNumber(r.SLVatTu)
                    };
                }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "Module", label: "Loại kho", center: true, width: 85 },
                    { key: "TenDay", label: "Tên dãy", center: true, width: 85 },
                    { key: "TenKe", label: "Tên kệ", center: true, width: 95 },
                    { key: "TongCBMSuDungTrongKe", label: "CBM sử dụng", number: 2, width: 110 },
                    { key: "TongCBMTrongKe", label: "Tổng CBM", number: 2, width: 110 },
                    { key: "PctSuDung", label: "% sử dụng", percent: true, width: 95 },
                    { key: "SLVatTu", label: "Số vật tư", number: 0, center: true, width: 85 }
                ]
            },
            inboundAll: {
                title: "Chi tiết lệnh chuẩn bị về",
                rows: state.inbound.map(function (r, i) { return Object.assign({ STT: i + 1 }, r); }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "PO", label: "POMUA", center: true, width: 140 },
                    { key: "SoLo", label: "Số lô", center: true, width: 120 },
                    { key: "MaDH", label: "Mã đơn hàng", center: true, width: 120 },
                    { key: "MaHang", label: "Mã hàng", center: true, width: 150 },
                    { key: "TenHang", label: "Tên hàng", width: 220 },
                    { key: "SoLuongSP", label: "Số lượng SP", number: 0, center: true, width: 100 },
                    { key: "NgayNKDuKien", label: "Ngày dự kiến", date: true, center: true, width: 120 }
                ]
            },
            outboundReadyAll: {
                title: "Chi tiết lệnh chuẩn bị xuất",
                rows: state.outboundReady.map(function (r, i) { return Object.assign({ STT: i + 1 }, r); }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true },
                    { key: "MaLenhSanXuat", label: "Mã lệnh SX", center: true },
                    { key: "MaLenh", label: "Mã lệnh", center: true },
                    { key: "MaDVSX", label: "Mã ĐVSX", center: true },
                    { key: "TenHang", label: "Tên hàng" },
                    { key: "KhachHang", label: "Khách hàng" },
                    { key: "KHCat", label: "KH cắt", date: true, center: true },
                    { key: "DuKienCat", label: "Dự kiến cắt", date: true, center: true }
                ]
            },
            flowTrendDetail: (function () {
                var dailyData = state.flowTrendRangeRaw || [];
                if (dailyData.length > 0) {
                    var rowsD = dailyData.map(function (r, i) {
                        var ngay = String(r.Ngay || "").substring(0, 10);
                        var parts = ngay.split("-");
                        var lbl = parts.length === 3 ? (parts[2] + "/" + parts[1] + "/" + parts[0]) : ngay;
                        var totalIn = toNumber(r.TotalIn);
                        var totalOut = toNumber(r.TotalOut);
                        var totalStk = toNumber(r.TotalStock);
                        return {
                            STT: i + 1,
                            KyBaoCao: lbl,
                            TotalIn: totalIn,
                            TotalOut: totalOut,
                            NetFlow: totalIn - totalOut,
                            TotalStock: totalStk
                        };
                    });
                    return {
                        title: "Chi tiết biểu đồ xuất - nhập - tồn (theo ngày)",
                        rows: rowsD,
                        columns: [
                            { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                            { key: "KyBaoCao", label: "Ngày", center: true, width: 110 },
                            { key: "TotalIn", label: "Nhập", number: 0, width: 110 },
                            { key: "TotalOut", label: "Xuất", number: 0, width: 110 },
                            { key: "NetFlow", label: "Nhập - Xuất", number: 0, width: 115 },
                            { key: "TotalStock", label: "Tồn kho", number: 0, width: 130 }
                        ]
                    };
                }
                var rows = (state.flowTrend12T || []).map(function (r, i) {
                    var lbl = String(r.Thang || "").padStart(2, "0") + "/" + (r.Nam || "");
                    var totalIn = toNumber(r.TotalIn);
                    var totalOut = toNumber(r.TotalOut);
                    var totalStk = toNumber(r.TotalStock);
                    return {
                        STT: i + 1,
                        KyBaoCao: lbl,
                        TotalIn: totalIn,
                        TotalOut: totalOut,
                        NetFlow: totalIn - totalOut,
                        TotalStock: totalStk
                    };
                });
                return {
                    title: "Chi tiết biểu đồ xuất - nhập - tồn (12 tháng gần nhất)",
                    rows: rows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "KyBaoCao", label: "Kỳ báo cáo", center: true, width: 110 },
                        { key: "TotalIn", label: "Nhập", number: 0, width: 110 },
                        { key: "TotalOut", label: "Xuất", number: 0, width: 110 },
                        { key: "NetFlow", label: "Nhập - Xuất", number: 0, width: 115 },
                        { key: "TotalStock", label: "Tồn kho", number: 0, width: 130 }
                    ]
                };
            })(),
            outboundRunningAll: {
                title: "Chi tiết lệnh đang xuất",
                rows: state.outboundRunning.map(function (r, i) { return Object.assign({ STT: i + 1 }, r); }),
                columns: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "MaLenhSX", label: "Mã lệnh SX", center: true, width: 130 },
                    { key: "MaLenh", label: "Mã lệnh", center: true, width: 100 },
                    { key: "MaGop", label: "Mã gộp", center: true, width: 100 },
                    { key: "TenHang", label: "Tên hàng", width: 220 },
                    { key: "TenKH", label: "Khách hàng", width: 180 },
                    { key: "NgayXuatHang", label: "Ngày xuất", date: true, center: true, width: 100 },
                    { key: "SLXuat", label: "SL xuất", number: 2, width: 110 }
                ]
            },
            ageStockDetail: (function () {
                var ageRows = state.ageStock.map(function (r, i) {
                    var nplVal = r.NPL;
                    var loaiKho = (nplVal === true || nplVal === 1 || nplVal === "true" || nplVal === "1") ? "NL" : "PL";
                    return Object.assign({ STT: i + 1 }, r, { LoaiKho: loaiKho });
                });
                return {
                    title: "Chi tiết tuổi tồn kho theo nhóm vật tư",
                    rows: ageRows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "TenNhom", label: "Nhóm vật tư", width: 240 },
                        { key: "Thang", label: "Tháng tuổi", number: 0, center: true, width: 100 },
                        { key: "TonKhoCT", label: "Tồn kho", number: 2, width: 120 },
                        { key: "LoaiKho", label: "Loại kho", center: true, width: 85 }
                    ]
                };
            })(),
            turnoverDetail: (function () {
                var trend12 = state.flowTrend12T || [];
                var totalOut12 = 0, stockSum = 0, stockCount = 0;
                var monthRows = [];
                for (var _ti = 0; _ti < trend12.length; _ti++) {
                    var _t = trend12[_ti];
                    var _out = toNumber(_t.TotalOut);
                    var _in = toNumber(_t.TotalIn);
                    var _stk = toNumber(_t.TotalStock);
                    totalOut12 += _out;
                    if (_stk > 0) { stockSum += _stk; stockCount++; }
                    var _lbl = String(_t.Thang).padStart(2, "0") + "/" + _t.Nam;
                    var _to = _stk > 0 ? (_out / _stk) : 0;
                    var _days = _to > 0 ? Math.round(365 / (_to * 12)) : 0;
                    monthRows.push({
                        STT: _ti + 1, KyBaoCao: _lbl,
                        TotalIn: _in, TotalOut: _out, TonKho: _stk,
                        VongQuayThang: _to > 0 ? formatNumber(_to, 2) : "--",
                        NgayTonTB: _days > 0 ? _days : "--"
                    });
                }
                var avgStock = stockCount > 0 ? stockSum / stockCount : 0;
                var turnover = avgStock > 0 && totalOut12 > 0 ? totalOut12 / avgStock : 0;
                var avgDays = turnover > 0 ? Math.round(365 / turnover) : 0;
                return {
                    title: "Chi tiết vòng quay hàng tồn kho (12 tháng)",
                    meta: "Vòng quay tổng: " + (turnover > 0 ? formatNumber(turnover, 2) + " lần/năm" : "--") +
                        "  |  Ngày tồn kho TB: " + (avgDays > 0 ? avgDays + " ngày" : "--"),
                    rows: monthRows,
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "KyBaoCao", label: "Kỳ báo cáo", center: true, width: 110 },
                        { key: "TotalIn", label: "Nhập", number: 0, width: 110 },
                        { key: "TotalOut", label: "Xuất", number: 0, width: 110 },
                        { key: "TonKho", label: "Tồn kho", number: 0, width: 120 },
                        { key: "VongQuayThang", label: "Vòng quay (lần/tháng)", center: true, width: 150 },
                        { key: "NgayTonTB", label: "Ngày tồn TB", center: true, width: 110 }
                    ]
                };
            })(),
            // v2.3.36 — Thành giá hàng tồn kho
            thanhGiaDetail: (function () {
                var tg = state.thanhGia || {};
                var thanhGia = toNumber(tg.ThanhGia);
                var tongMa = toNumber(tg.TongMaVT);
                var soMaCoGia = toNumber(tg.SoMaCoGia);
                var soMaKhongGia = toNumber(tg.SoMaKhongGia);
                var tongSL = toNumber(tg.TongSoLuong);
                var avgGia = tongSL > 0 ? thanhGia / tongSL : 0;
                var pctCoGia = tongMa > 0 ? (soMaCoGia / tongMa) * 100 : 0;

                var rows = [
                    { ChiSo: "Tổng thành giá hàng tồn (VND)", GiaTri: formatNumber(thanhGia, 0) },
                    { ChiSo: "Tổng số mã vật tư trong kho", GiaTri: formatNumber(tongMa, 0) },
                    { ChiSo: "Số mã có giá", GiaTri: formatNumber(soMaCoGia, 0) + "  (" + formatNumber(pctCoGia, 1) + "%)" },
                    { ChiSo: "Số mã chưa có giá", GiaTri: formatNumber(soMaKhongGia, 0) },
                    { ChiSo: "Tổng số lượng tồn", GiaTri: formatNumber(tongSL, 2) },
                    { ChiSo: "Giá trị TB / 1 đơn vị (VND)", GiaTri: formatNumber(avgGia, 0) }
                ];
                return {
                    title: "Chi tiết thành giá hàng tồn kho",
                    meta: "Tổng giá trị: " + formatNumber(thanhGia, 0) + " VND",
                    rows: rows.map(function (r, i) { return Object.assign({ STT: i + 1 }, r); }),
                    columns: [
                        { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                        { key: "ChiSo", label: "Chỉ số", width: 250 },
                        { key: "GiaTri", label: "Giá trị", center: true, width: 180 }
                    ]
                };
            })()
        };

        if (detail === "customerRow") {
            // v2.3.25 — Customer drill: hiện hàng của khách ở đâu (dãy/kệ/ô)
            var customerItem = buildCustomerDetailRows().filter(function (row) {
                return row.sourceIndex === index;
            })[0];
            return {
                title: "Vị trí lưu kho của khách hàng: " + (customerItem ? (customerItem.TenKH || customerItem.MaKH) : ""),
                rows: customerItem ? [customerItem] : [],
                columns: detailMap.customersAll.columns,
                customDrillCustomer: customerItem  // flag để openDetail tự fetch slot detail
            };
        }

        if (detail === "rackRow") {
            // v2.3.26 — Rack drill: hiện những vật tư trong kệ đó
            var rackItem = state.racks[index];
            return {
                title: "Chi tiết kệ: " + (rackItem ? (rackItem.TenKe || "") : ""),
                rows: rackItem ? [rackItem] : [],
                columns: detailMap.racksAll.columns,
                customDrillRack: rackItem
            };
        }

        if (detail === "inboundRow") {
            var inboundItem = state.inbound[index];
            return {
                title: "Chi tiết lệnh chuẩn bị về",
                rows: inboundItem ? [inboundItem] : [],
                columns: detailMap.inboundAll.columns
            };
        }

        if (detail === "outboundReadyRow") {
            var readyItem = state.outboundReady[index];
            return {
                title: "Chi tiết lệnh chuẩn bị xuất",
                rows: readyItem ? [readyItem] : [],
                columns: detailMap.outboundReadyAll.columns
            };
        }

        if (detail === "outboundRunningRow") {
            var runningItem = state.outboundRunning[index];
            return {
                title: "Chi tiết lệnh đang xuất",
                rows: runningItem ? [runningItem] : [],
                columns: detailMap.outboundRunningAll.columns
            };
        }

        return detailMap[detail] || {
            title: "Chi tiết",
            rows: [],
            columns: []
        };
    }

    function renderDetailTable(columns, rows) {
        if (!columns || columns.length === 0) {
            return "<p class=\"dk-empty\">Không có cấu hình cột hiển thị.</p>";
        }

        if (!rows || rows.length === 0) {
            return "<p class=\"dk-empty\">Không có dữ liệu chi tiết.</p>";
        }

        // v2.3.27 — Cột text dài cần wrap (left align). MaNPL/DanhSachKH/DanhSachMaNPL có thể rất dài
        var TEXT_LEFT_KEYS = { TenKH: 1, KhachHang: 1, TenKe: 1, TenDay: 1, TenHang: 1, MaVT: 1, ChiTiet: 1, MaGop: 1, MaDH: 1, MaHang: 1, MaLenh: 1, MaDVSX: 1, MaNPL: 1, DanhSachKH: 1, DanhSachMaNPL: 1, ChiSo: 1, ItemCode: 1, TenVT: 1, MaPhieu: 1 };
        // v2.4.0 — Cột code dùng monospace + tabular alignment
        var MONO_KEYS = { ItemCode: 1, MaVT: 1, MaNPL: 1, MaGop: 1, MaLenh: 1, MaPhieu: 1 };

        var headHtml = columns.map(function (column) {
            var styles = [];
            if (column.center) {
                styles.push("text-align:center");
            } else if (column.number !== undefined || column.percent) {
                styles.push("text-align:right");
            } else if (TEXT_LEFT_KEYS[column.key]) {
                styles.push("text-align:left");
            }
            // v2.3.39 - Width hint từng cột (compact tables)
            if (column.width) {
                var w = column.width + (typeof column.width === "number" ? "px" : "");
                styles.push("width:" + w + ";min-width:" + w);
            } else {
                styles.push("min-width:150px");
            }
            var thStyle = styles.length ? ' style="' + styles.join(";") + '"' : "";
            return "<th" + thStyle + ">" + escapeHtml(column.label) + "</th>";
        }).join("");

        var bodyHtml = rows.map(function (row, rowIdx) {
            var cols = columns.map(function (column) {
                var value = row[column.key];
                var cellStyle = "";
                // v2.3.21 — raw:true → render HTML thẳng (cho link drill)
                if (column.raw) {
                    var rawAlign = column.center ? "center" : "left";
                    return "<td style=\"text-align:" + rawAlign + "\">" + (value || "") + "</td>";
                }
                if (column.number !== undefined) {
                    value = formatNumber(value, column.number);
                    cellStyle = column.center ? " style=\"text-align:center\"" : " style=\"text-align:right\"";
                } else if (column.percent) {
                    value = formatPercent(value);
                    cellStyle = column.center ? " style=\"text-align:center\"" : " style=\"text-align:right\"";
                } else if (column.date) {
                    value = formatDate(value);
                    if (column.center) cellStyle = " style=\"text-align:center\"";
                } else if (column.center) {
                    var cellMonoStyle = MONO_KEYS[column.key] ? ";font-family:Consolas,'Courier New',ui-monospace,monospace;font-weight:600" : "";
                    cellStyle = " style=\"text-align:center" + cellMonoStyle + "\"";
                } else if (column.key === "TenKH" || column.key === "KhachHang" || column.key === "TenKe" || column.key === "TenDay" || TEXT_LEFT_KEYS[column.key]) {
                    if (column.key === "TenKH" || column.key === "KhachHang") value = normalizeCustomerName(value);
                    var leftStyle = "text-align:left";
                    if (MONO_KEYS[column.key]) {
                        leftStyle += ";font-family:Consolas,'Courier New',ui-monospace,monospace;font-weight:600";
                    }
                    cellStyle = " style=\"" + leftStyle + "\"";
                } else if (column.key === "MaLenhSanXuat" || column.key === "MaLenhSX") {
                    value = shortMaLenh(value);
                    var mlAlign = column.center ? "center" : "left";
                    cellStyle = " style=\"font-weight:700;text-align:" + mlAlign + "\"";
                } else if (column.key === "LoaiKho" || column.key === "Module") {
                    var loaiColor = value === "NL" ? "#2563eb" : (value === "PL" ? "#d97706" : "#6b7280");
                    var loaiAlign = column.center ? "center" : "left";
                    return "<td style=\"text-align:" + loaiAlign + "\"><span style=\"color:" + loaiColor + ";font-weight:800\">" + escapeHtml(value || "") + "</span></td>";
                }
                return "<td" + cellStyle + ">" + escapeHtml(value) + "</td>";
            }).join("");

            return "<tr>" + cols + "</tr>";
        }).join("");

        return "<div class=\"dk-detail-table-wrap\"><table class=\"dk-detail-table\"><thead><tr>" + headHtml + "</tr></thead><tbody>" + bodyHtml + "</tbody></table></div>";
    }

    // v2.3.27 — Modal navigation stack: drill có nút "← Quay lại"
    var _detailStack = [];
    function openDetail(detail, index) {
        // Push parent state lên stack nếu modal đang mở (= drilling)
        var modal = byId(ids.detailModal);
        if (modal && modal.classList.contains("open") && _detailStack.length === 0) {
            // First-time push: lưu lại detail HIỆN TẠI (parent)
            // (chỉ push 1 lần, không nested deeper)
        }
        if (modal && modal.classList.contains("open") && _currentDetail) {
            _detailStack.push({ detail: _currentDetail, index: _currentDetailIndex });
        }
        _currentDetail = detail;
        _currentDetailIndex = index;
        renderDetailModal(detail, index);
    }
    var _currentDetail = null;
    var _currentDetailIndex = -1;

    function renderDetailModal(detail, index) {
        var model;
        try { model = getDetailData(detail, index); } catch (e) { model = { title: detail, rows: [], columns: [], isWarehouseMap: false }; }
        // Update title with back button if stack non-empty
        var titleEl = byId(ids.detailModalTitle);
        if (_detailStack.length > 0) {
            titleEl.innerHTML = "<button type=\"button\" class=\"dk-back-btn js-modal-back\" title=\"Quay lại\">← Quay lại</button> " + escapeHtml(model.title);
        } else {
            titleEl.textContent = model.title;
        }
        // v2.3.20 — Meta header chỉ hiện model.meta (nếu có), tổng dòng dời xuống footer
        byId(ids.detailModalMeta).textContent = model.meta || "";
        if (!model.meta) byId(ids.detailModalMeta).style.display = "none";
        else byId(ids.detailModalMeta).style.display = "";
        // Update footer row count
        var rowCountEl = byId("detailModalRowCount");
        if (rowCountEl) {
            rowCountEl.textContent = "Tổng số dòng: " + formatNumber((model.rows || []).length, 0);
        }

        // Reset search
        var searchInput = byId("detailSearchInput");
        if (searchInput) { searchInput.value = ""; }
        var searchCount = byId("detailSearchCount");
        if (searchCount) { searchCount.textContent = ""; }

        var rowCount = (model.rows || []).length;
        // v2.3.17 — detail có ít dòng → ẩn search bar
        var noSearchDetails = { capacitySummary: 1, materialCount: 1 };
        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) {
            searchBar.style.display = (rowCount <= 5 || noSearchDetails[detail]) ? "none" : "";
        }

        if (model.isWarehouseMap) {
            var overall = state.overall.length > 0 ? state.overall[0] : {};
            var wrapHtml = "<div class=\"dk-modal-wh-wrap\">";

            wrapHtml += "<div class=\"dk-modal-wh-summary\">";
            wrapHtml += "<div class=\"dk-modal-wh-stat\"><div class=\"dk-modal-wh-stat-label\">Tổng sức chứa (CBM)</div><div class=\"dk-modal-wh-stat-value\">" + formatNumber(toNumber(overall.TotalCapacity), 2) + "</div></div>";
            wrapHtml += "<div class=\"dk-modal-wh-stat\"><div class=\"dk-modal-wh-stat-label\">Đã sử dụng</div><div class=\"dk-modal-wh-stat-value dk-text-danger\">" + formatNumber(toNumber(overall.UsedNPL) + toNumber(overall.UsedPL), 2) + " CBM — " + formatNumber(toNumber(overall.TotalPercent), 1) + "%</div></div>";
            wrapHtml += "<div class=\"dk-modal-wh-stat\"><div class=\"dk-modal-wh-stat-label\">Kho NL</div><div class=\"dk-modal-wh-stat-value dk-text-primary\">" + formatNumber(toNumber(overall.UsedNPL), 2) + " / " + formatNumber(toNumber(overall.CapacityNPL), 2) + " CBM</div><div class=\"dk-modal-wh-stat-sub\">" + formatNumber(toNumber(overall.PercentNPL), 1) + "% lấp đầy</div></div>";
            wrapHtml += "<div class=\"dk-modal-wh-stat\"><div class=\"dk-modal-wh-stat-label\">Kho PL</div><div class=\"dk-modal-wh-stat-value dk-text-warn\">" + formatNumber(toNumber(overall.UsedPL), 2) + " / " + formatNumber(toNumber(overall.CapacityPL), 2) + " CBM</div><div class=\"dk-modal-wh-stat-sub\">" + formatNumber(toNumber(overall.PercentPL), 1) + "% lấp đầy</div></div>";
            wrapHtml += "</div>";

            // Sơ đồ tile map
            wrapHtml += "<div style=\"margin-bottom:14px\">";
            wrapHtml += "<b style=\"font-size:13px\">Sơ đồ lấp đầy kệ</b>";
            wrapHtml += "<div style=\"display:flex;gap:10px;font-size:11px;font-weight:700;margin:6px 0;\">";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#dcfce7;border:1px solid #86efac;border-radius:2px;vertical-align:middle\"></i> &lt;50%</span>";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#fef9c3;border:1px solid #fde047;border-radius:2px;vertical-align:middle\"></i> 50-85%</span>";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#fecdd3;border:1px solid #fda4af;border-radius:2px;vertical-align:middle\"></i> 85-100%</span>";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#e11d48;border:1px solid #be123c;border-radius:2px;vertical-align:middle\"></i> &gt;100%</span>";
            wrapHtml += "</div>";

            // Build inline heatmap
            var racksSorted = state.racks.slice().sort(function (a, b) {
                var mA = toNumber(a.Module), mB = toNumber(b.Module);
                if (mA !== mB) return mA - mB;
                return (a.TenDay || "").localeCompare(b.TenDay || "");
            });
            var modGroups = {};
            for (var rr = 0; rr < racksSorted.length; rr++) {
                var rm = toNumber(racksSorted[rr].Module);
                var rmName = rm === 1 ? "NL" : (rm === 2 ? "PL" : "Khác");
                if (!modGroups[rmName]) modGroups[rmName] = [];
                modGroups[rmName].push(racksSorted[rr]);
            }
            var groupKeys = ["NL", "PL", "Khác"];
            for (var gk = 0; gk < groupKeys.length; gk++) {
                var gName = groupKeys[gk];
                var gRacks = modGroups[gName];
                if (!gRacks || !gRacks.length) continue;
                wrapHtml += "<div style=\"margin-bottom:10px\"><b style=\"font-size:12px;color:" + (gName === "NL" ? "#2563eb" : "#d97706") + "\">" + gName + "</b><div style=\"display:flex;flex-wrap:wrap;gap:5px;margin-top:4px\">";
                for (var gr = 0; gr < gRacks.length; gr++) {
                    var grItem = gRacks[gr];
                    var grUsed = toNumber(grItem.TongCBMSuDungTrongKe);
                    var grCap = toNumber(grItem.TongCBMTrongKe);
                    var grPct = grCap > 0 ? (grUsed / grCap) * 100 : 0;
                    var grClass = grPct > 100 ? "dk-wh-over" : (grPct >= 85 ? "dk-wh-full" : (grPct >= 50 ? "dk-wh-warn" : "dk-wh-safe"));
                    // v2.4.16 — Tile fill animation
                    var grFillH = Math.min(100, Math.max(0, grPct));
                    wrapHtml += "<div class=\"dk-wh-tile " + grClass + "\" data-pct=\"" + grFillH.toFixed(1) + "\" title=\"" + escapeHtml((grItem.TenKe || "") + " | " + (grItem.TenDay || "") + " | " + formatNumber(grPct, 1) + "%") + "\" style=\"cursor:default;animation-delay:" + (gr * 40) + "ms\">";
                    wrapHtml += "<span class=\"dk-wh-tile-fill\" style=\"height:" + grFillH.toFixed(1) + "%\"></span>";
                    if (grPct >= 100) wrapHtml += "<i class=\"fa-solid fa-triangle-exclamation dk-wh-tile-warn-icon\"></i>";
                    wrapHtml += "<span class=\"dk-wh-tile-name\">" + escapeHtml(grItem.TenKe || "Kệ") + "</span>";
                    wrapHtml += "<span class=\"dk-wh-tile-pct\">" + formatNumber(grPct, 0) + "%</span>";
                    wrapHtml += "</div>";
                }
                wrapHtml += "</div></div>";
            }
            wrapHtml += "</div>";

            // Table bên dưới (v2.7.1 — bỏ label "NL trước → PL")
            wrapHtml += renderDetailTable(model.columns, model.rows);
            wrapHtml += "</div>";

            byId(ids.detailModalContent).innerHTML = wrapHtml;
        } else {
            byId(ids.detailModalContent).innerHTML = renderDetailTable(model.columns, model.rows);
        }

        byId(ids.detailModal).classList.add("open");

        // v2.3.9 — Sau khi mở modal totalCapacity → fetch chi tiết theo Ô + append
        if (model.isWarehouseMap) {
            loadRackSlotDetailIntoModal();
        }

        // v2.3.30 — Async load cho drill "Mã vật tư" (toàn bộ ~2348 mã)
        if (model.customAsync === "allMaterials") {
            var contentEl = byId(ids.detailModalContent);
            if (contentEl) contentEl.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px\">Đang tải tất cả mã vật tư trong kho (có thể mất 5-15 giây)...</div>";
            requestJson("/api/DashboardKhoDesktop/GetAllMaterialsInStock").then(function (data) {
                var rows = normalizeArray(data).map(function (r, i) {
                    // v2.4.0 — Compose Màu = "Mã màu — Tên màu" + swatch; Khổ vải kèm đơn vị; alias MaVT → ItemCode
                    var maMau = r.MaMauVT ? String(r.MaMauVT).trim() : "";
                    var tenMau = r.Mau ? String(r.Mau).trim() : "";
                    var mauText = "";
                    if (maMau && tenMau) mauText = escapeHtml(maMau) + " — " + escapeHtml(tenMau);
                    else if (maMau) mauText = escapeHtml(maMau);
                    else if (tenMau) mauText = escapeHtml(tenMau);
                    var mauDisplay = mauText
                        ? '<span class="dk-mau-swatch" data-mau="' + escapeHtml(maMau || tenMau) + '"></span>' +
                        '<span class="dk-mau-text">' + mauText + "</span>"
                        : "";
                    var khoVai = r.KhoVai ? String(r.KhoVai).trim() : "";
                    var dvvt = r.TenDVVT ? String(r.TenDVVT).trim() : "";
                    var khoVaiDisplay = khoVai ? (khoVai + (dvvt ? " " + dvvt : "")) : "";

                    return Object.assign({}, r, {
                        STT: i + 1,
                        ItemCode: r.MaVT || "",
                        MauDisplay: mauDisplay,
                        KhoVai: khoVaiDisplay
                    });
                });
                if (rows.length === 0) {
                    contentEl.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px\">Không có mã vật tư nào</div>";
                } else {
                    contentEl.innerHTML = renderDetailTable(model.columns, rows);
                }
                // Update footer row count
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            }).catch(function (err) {
                // v2.3.35 — Hiện rõ lỗi để debug, không chỉ "Lỗi tải" chung chung
                console.error("[Dashboard Kho] AllMaterials error:", err);
                var msg = (err && err.message) ? err.message : "Lỗi không rõ";
                contentEl.innerHTML =
                    "<div style=\"padding:30px\">" +
                    "<div class=\"dk-text-danger\" style=\"margin-bottom:10px;font-size:14px\">⚠ Lỗi tải danh sách mã vật tư</div>" +
                    "<div class=\"dk-error-box\">" + escapeHtml(msg) + "</div>" +
                    "<div class=\"dk-text-muted\" style=\"margin-top:14px;font-size:12.5px;line-height:1.6\">" +
                    "<b>Có thể do:</b><br>" +
                    "• Backend chưa được Rebuild (SP <code>GetAllMaterialsInStock</code> chưa tồn tại) → Stop debug → Rebuild Solution → F5<br>" +
                    "• Query SQL bị lỗi → mở F12 Network → tab Response của request <code>GetAllMaterialsInStock</code> để xem chi tiết<br>" +
                    "• Connection timeout → query quá nặng, thử lại sau" +
                    "</div>" +
                    "</div>";
            });
        }

        // v2.4.4 — Dispatcher cho 6 modal "Xem chi tiết" mới
        if (model.customAsync === "todoDetail") { renderTodoDetailModal(); }
        if (model.customAsync === "top5VTAll") { renderTop5VTAllModal(); }
        if (model.customAsync === "top5KHAll") { renderTop5KHAllModal(); }
        if (model.customAsync === "hetHanAll") { renderHetHanAllModal(); }
        if (model.customAsync === "giaTriNhomAll") { renderGiaTriNhomAllModal(); }
        if (model.customAsync === "kiemKeAll") { renderKiemKeAllModal(); }
        // v2.4.15 — Dispatcher cho 5 modal KPI header
        if (model.customAsync === "tonDauKyDetail") { renderTonDauKyDetailModal(); }
        if (model.customAsync === "tongNhapDetail") { renderTongNhapDetailModal(); }
        if (model.customAsync === "tongXuatDetail") { renderTongXuatDetailModal(); }
        if (model.customAsync === "tonKhoDetail") { renderTonKhoDetailModal(); }
        if (model.customAsync === "poTreDetail") { renderPOTreDetailModal(); }
        if (model.customAsync === "alertDetail") { renderAlertDetailModal(); }

        if (model.customDrillCustomer) {
            var custTenKH = String(model.customDrillCustomer.TenKH || "").trim();
            var custMaKH = String(model.customDrillCustomer.MaKH || "").trim();
            loadSlotDrillIntoModal({
                filterFn: function (r) {
                    var ds = String(r.DanhSachKH || "");
                    if (custTenKH && ds.toLowerCase().indexOf(custTenKH.toLowerCase()) >= 0) return true;
                    if (custMaKH && ds.toLowerCase().indexOf(custMaKH.toLowerCase()) >= 0) return true;
                    return false;
                },
                emptyText: "Khách hàng \"" + (custTenKH || custMaKH) + "\" chưa có vật tư nào trong kho",
                sectionTitle: "Vị trí hàng của khách trong kho — theo dãy / kệ / ô"
            });
        }

        // v2.3.26 — Rack drill: fetch GetRackSlotDetail và filter theo TenKe
        if (model.customDrillRack) {
            loadSlotDrillIntoModal({
                filterFn: function (r) {
                    return String(r.TenKe || "").trim() === String(model.customDrillRack.TenKe || "").trim();
                },
                emptyText: "Kệ này chưa có vật tư nào",
                sectionTitle: "Danh sách ô và vật tư trong kệ này"
            });
        }
    }

    // v2.3.25 + v2.3.26 — Fetch GetRackSlotDetail rồi filter + append vào modal
    function loadSlotDrillIntoModal(opts) {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var section = document.createElement("div");
        section.style.cssText = "margin-top:18px;padding-top:14px;border-top:2px solid #e2e8f0";
        section.innerHTML =
            "<div style=\"display:flex;align-items:center;gap:10px;margin-bottom:10px\">" +
            "<b class=\"dk-text-title\" style=\"font-size:14px\">" + opts.sectionTitle + "</b>" +
            "</div>" +
            "<div class=\"dk-slot-drill-body\"><div class=\"dk-empty\" style=\"padding:20px\">Đang tải...</div></div>";
        content.appendChild(section);
        var body = section.querySelector(".dk-slot-drill-body");

        requestJson("/api/DashboardKhoDesktop/GetRackSlotDetail").then(function (data) {
            var allRows = normalizeArray(data);
            var rows = allRows.filter(opts.filterFn).map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            });
            if (rows.length === 0) {
                body.innerHTML = "<div class=\"dk-empty\" style=\"padding:20px\">" + opts.emptyText + "</div>";
                return;
            }
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true },
                { key: "LoaiKho", label: "Loại", center: true },
                { key: "TenDay", label: "Dãy", center: true },
                { key: "TenKe", label: "Kệ", center: true },
                { key: "TenO", label: "Ô", center: true },
                { key: "DanhSachKH", label: "Khách hàng" },
                { key: "DanhSachMaNPL", label: "Mã NPL" },
                { key: "SoBarCode", label: "Số barcode", number: 0, center: true },
                { key: "TongCBM", label: "Tổng CBM", number: 4, center: true }
            ];
            body.innerHTML = renderDetailTable(cols, rows);
        }).catch(function () {
            body.innerHTML = "<div class=\"dk-empty dk-text-danger\" style=\"padding:20px\">Lỗi tải dữ liệu — cần rebuild backend</div>";
        });
    }

    // v2.3.9 — Tải chi tiết cấp Ô và chèn vào cuối modal Tổng sức chứa
    function loadRackSlotDetailIntoModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;

        // Placeholder section
        var section = document.createElement("div");
        section.style.cssText = "margin-top:18px;padding-top:14px;border-top:2px solid #e2e8f0";
        section.innerHTML =
            "<div style=\"display:flex;align-items:center;gap:10px;margin-bottom:10px\">" +
            "<b class=\"dk-text-title\" style=\"font-size:14px\">Chi tiết theo ô (slot)</b>" +
            "<span class=\"dk-text-muted\" style=\"font-size:12px\">— Group theo loại kho NL / PL</span>" +
            "</div>" +
            "<div id=\"dkRackSlotBody\" style=\"min-height:80px\">" +
            "<div class=\"dk-empty\" style=\"padding:20px\">Đang tải chi tiết theo ô...</div>" +
            "</div>";
        content.appendChild(section);

        requestJson("/api/DashboardKhoDesktop/GetRackSlotDetail").then(function (data) {
            var rows = normalizeArray(data);
            var body = byId("dkRackSlotBody");
            if (!body) return;
            if (rows.length === 0) {
                body.innerHTML = "<div class=\"dk-empty\" style=\"padding:20px\">Không có dữ liệu chi tiết theo ô</div>";
                return;
            }

            // v2.7.1 — Cols: bỏ cột Dãy (TenDay) và Mã màu (MaMauVT)
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "TenKe", label: "Kệ", center: true, width: 90 },
                { key: "TenO", label: "Ô", center: true, width: 80 },
                { key: "DanhSachKH", label: "Khách hàng" },
                { key: "MauVT", label: "Màu", center: true, width: 90 },
                { key: "WidthSize", label: "Width/Size", center: true, width: 110 },
                { key: "SoBarCode", label: "Số BC", number: 0, center: true, width: 60 },
                { key: "TongCBM", label: "CBM", number: 4, center: true, width: 90 }
            ];

            // v2.3.41 — Tách NL và PL
            var nlRows = rows.filter(function (r) { return toNumber(r.Module) === 1; })
                .map(function (r, i) { return Object.assign({ STT: i + 1 }, r); });
            var plRows = rows.filter(function (r) { return toNumber(r.Module) === 2; })
                .map(function (r, i) { return Object.assign({ STT: i + 1 }, r); });

            var totalBC = 0;
            rows.forEach(function (r) { totalBC += toNumber(r.SoBarCode); });

            var html = "<div style=\"display:flex;gap:14px;flex-wrap:wrap;margin-bottom:12px;font-size:12.5px;font-weight:700\">" +
                "<span class=\"dk-text-title\">Tổng ô đang dùng: " + formatNumber(rows.length, 0) + "</span>" +
                "<span class=\"dk-text-primary\">Ô NL: " + formatNumber(nlRows.length, 0) + "</span>" +
                "<span class=\"dk-text-warn\">Ô PL: " + formatNumber(plRows.length, 0) + "</span>" +
                "<span class=\"dk-text-muted\">Tổng barcode: " + formatNumber(totalBC, 0) + "</span>" +
                "</div>";

            // Section NL
            if (nlRows.length > 0) {
                html += "<div class=\"dk-slot-section\">" +
                    "<div class=\"dk-slot-section-head dk-slot-nl\">" +
                    "<span class=\"dk-slot-section-badge\">NL</span>" +
                    "<span class=\"dk-slot-section-title\">Kho Nguyên liệu</span>" +
                    "<span class=\"dk-slot-section-count\">" + nlRows.length + " ô</span>" +
                    "</div>" +
                    renderDetailTable(cols, nlRows) +
                    "</div>";
            }
            // Section PL
            if (plRows.length > 0) {
                html += "<div class=\"dk-slot-section\" style=\"margin-top:16px\">" +
                    "<div class=\"dk-slot-section-head dk-slot-pl\">" +
                    "<span class=\"dk-slot-section-badge\">PL</span>" +
                    "<span class=\"dk-slot-section-title\">Kho Phụ liệu</span>" +
                    "<span class=\"dk-slot-section-count\">" + plRows.length + " ô</span>" +
                    "</div>" +
                    renderDetailTable(cols, plRows) +
                    "</div>";
            }
            body.innerHTML = html;
        }).catch(function (err) {
            console.error("[Dashboard Kho] Rack slot detail error:", err);
            var body = byId("dkRackSlotBody");
            if (body) body.innerHTML = "<div class=\"dk-empty dk-text-danger\" style=\"padding:20px\">Lỗi tải chi tiết theo ô. (Cần rebuild backend nếu chưa)</div>";
        });
    }

    function closeDetailModal() {
        var modal = byId(ids.detailModal);
        if (!modal) return;
        modal.classList.remove("open");
        modal.style.display = "";  // clear any inline style from older code paths

        // v2.3.27 — Reset drill stack khi đóng modal hoàn toàn
        _detailStack = [];
        _currentDetail = null;
        _currentDetailIndex = -1;

        // Restore search bar visibility (calendar-day modal hides it)
        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "";
    }


    // ─── Feature 10: Full-screen Panel Mode ─────────────────────────────────────
    var fsBackdrop = null;

    function injectMaximizeButtons() {
        var heads = document.querySelectorAll(".dk-panel-head");
        for (var hi = 0; hi < heads.length; hi++) {
            if (heads[hi].querySelector(".dk-maximize-btn")) continue; // already added
            var btn = document.createElement("button");
            btn.type = "button";
            btn.className = "dk-maximize-btn";
            btn.title = "Mở rộng";
            btn.setAttribute("aria-label", "Mở rộng");
            btn.innerHTML = '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="15 3 21 3 21 9"/><polyline points="9 21 3 21 3 15"/><line x1="21" y1="3" x2="14" y2="10"/><line x1="3" y1="21" x2="10" y2="14"/></svg>';
            heads[hi].appendChild(btn);
        }
    }

    function togglePanelFullscreen(panel) {
        var isFs = panel.classList.contains("dk-panel-fullscreen");
        var maxBtn = panel.querySelector(".dk-maximize-btn");
        if (isFs) {
            panel.classList.remove("dk-panel-fullscreen");
            if (fsBackdrop && fsBackdrop.parentNode) fsBackdrop.parentNode.removeChild(fsBackdrop);
            fsBackdrop = null;
            if (maxBtn) {
                maxBtn.title = "Mở rộng";
                maxBtn.setAttribute("aria-label", "Mở rộng");
                maxBtn.innerHTML = '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="15 3 21 3 21 9"/><polyline points="9 21 3 21 3 15"/><line x1="21" y1="3" x2="14" y2="10"/><line x1="3" y1="21" x2="10" y2="14"/></svg>';
            }
        } else {
            // Remove any existing fullscreen
            var existing = document.querySelector(".dk-panel-fullscreen");
            if (existing) {
                existing.classList.remove("dk-panel-fullscreen");
                var extBtn = existing.querySelector(".dk-maximize-btn");
                if (extBtn) {
                    extBtn.title = "Mở rộng";
                    extBtn.setAttribute("aria-label", "Mở rộng");
                    extBtn.innerHTML = '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="15 3 21 3 21 9"/><polyline points="9 21 3 21 3 15"/><line x1="21" y1="3" x2="14" y2="10"/><line x1="3" y1="21" x2="10" y2="14"/></svg>';
                }
            }

            panel.classList.add("dk-panel-fullscreen");
            fsBackdrop = document.createElement("div");
            fsBackdrop.className = "dk-fs-backdrop";
            fsBackdrop.addEventListener("click", function () { togglePanelFullscreen(panel); });
            document.body.appendChild(fsBackdrop);
            if (maxBtn) {
                maxBtn.title = "Thu nhỏ";
                maxBtn.setAttribute("aria-label", "Thu nhỏ");
                maxBtn.innerHTML = '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="4 14 10 14 10 20"/><polyline points="20 10 14 10 14 4"/><line x1="14" y1="10" x2="21" y2="3"/><line x1="10" y1="14" x2="3" y2="21"/></svg>';
            }
        }
        function reflowAllHighcharts() {
            if (typeof Highcharts === "undefined") return;
            var charts = Highcharts.charts || [];
            for (var c = 0; c < charts.length; c++) {
                if (!charts[c]) continue;
                try { charts[c].reflow(); } catch (e) { }
            }
        }
        setTimeout(reflowAllHighcharts, 60);
        setTimeout(reflowAllHighcharts, 260);
        setTimeout(reflowAllHighcharts, 600);
        setTimeout(function () {
            try { window.dispatchEvent(new Event("resize")); } catch (e) { }
        }, 80);
    }

    function bindEvents() {
        // Refresh button (sidebar)
        function refreshWithClearCache() {
            sessionStorage.setItem("dk_restore_tab", currentPage);
            requestJson("/api/DashboardKhoDesktop/ClearCache")
                .then(function () {
                    window.location.reload();
                })
                .catch(function () {
                    window.location.reload();
                });
        }
        var refreshButton = byId("btnRefresh");
        if (refreshButton) {
            refreshButton.onclick = function () { refreshWithClearCache(); };
        }
        var legacyRefresh = byId(ids.refreshButton);
        if (legacyRefresh) {
            legacyRefresh.onclick = function () { refreshWithClearCache(); };
        }

        document.addEventListener("click", function (event) {
            var target = event.target;
            if (!target) return;

            // Feature 10: maximize button
            var maxBtn = target;
            while (maxBtn && maxBtn !== document) {
                if (maxBtn.classList && maxBtn.classList.contains("dk-maximize-btn")) break;
                maxBtn = maxBtn.parentNode;
            }
            if (maxBtn && maxBtn !== document && maxBtn.classList.contains("dk-maximize-btn")) {
                var panel = maxBtn.closest ? maxBtn.closest(".dk-panel") : null;
                if (!panel) {
                    var p = maxBtn;
                    while (p && p !== document) {
                        if (p.classList && p.classList.contains("dk-panel")) { panel = p; break; }
                        p = p.parentNode;
                    }
                }
                if (panel) { togglePanelFullscreen(panel); return; }
            }

            var detailNode = findDetailNode(target);
            if (detailNode) {
                // v2.3.21 — Prevent default anchor jump
                if (event.preventDefault) event.preventDefault();
                var detail = detailNode.getAttribute("data-detail");
                var index = parseInt(detailNode.getAttribute("data-index"), 10);
                // v2.4.7 — Lưu data-todo-type để renderTodoDetailModal mở đúng tab
                var todoType = detailNode.getAttribute("data-todo-type");
                window.__dkPendingTodoType = todoType || null;
                // v2.4.16 — Lưu data-alert-code + name để renderAlertDetailModal đọc
                var alertCode = detailNode.getAttribute("data-alert-code");
                var alertName = detailNode.getAttribute("data-alert-name");
                window.__dkPendingAlertCode = alertCode || null;
                window.__dkPendingAlertName = alertName || null;
                openDetail(detail, isNaN(index) ? -1 : index);
                return;
            }

            // v2.3.27 — Back button trong modal drill: pop stack thay vì close
            var backNode = target.closest ? target.closest(".js-modal-back") : null;
            if (!backNode) {
                var t2 = target;
                while (t2 && t2 !== document) {
                    if (t2.classList && t2.classList.contains("js-modal-back")) { backNode = t2; break; }
                    t2 = t2.parentNode;
                }
            }
            if (backNode) {
                if (event.preventDefault) event.preventDefault();
                if (_detailStack.length > 0) {
                    var parent = _detailStack.pop();
                    _currentDetail = parent.detail;
                    _currentDetailIndex = parent.index;
                    renderDetailModal(parent.detail, parent.index);
                } else {
                    closeDetailModal();
                }
                return;
            }

            var closeNode = findCloseNode(target);
            if (closeNode) {
                // v2.3.27 — Nếu có stack thì X cũng quay lại parent (giống back)
                if (_detailStack.length > 0) {
                    var p = _detailStack.pop();
                    _currentDetail = p.detail;
                    _currentDetailIndex = p.index;
                    renderDetailModal(p.detail, p.index);
                } else {
                    closeDetailModal();
                }
            }
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                // Close fullscreen first, then modal
                var fsPanel = document.querySelector(".dk-panel-fullscreen");
                if (fsPanel) { togglePanelFullscreen(fsPanel); return; }
                closeDetailModal();
            }
        });

        // Feature 11: Customer filter dropdown
        var custFilter = byId("customerFilterSelect");
        if (custFilter) {
            custFilter.addEventListener("change", function () {
                activeCustomerFilter = this.value;
                renderCustomerPieChart();
                renderCustomersTable();
            });
        }

        // Feature 6: Activity calendar week-count selector
        var actWeekSel = byId("activityWeeksSelect");
        if (actWeekSel) {
            actWeekSel.addEventListener("change", function () {
                activityWeeksCount = parseInt(this.value, 10) || 13;
                renderActivityCalendar();
            });
        }

        // Event listeners for trend and load filter selects
        var trendFilter = byId("trendFilterSelect");
        if (trendFilter) {
            trendFilter.addEventListener("change", function () {
                renderLpcpBottomCharts();
            });
        }

        var loadFilter = byId("loadFilterSelect");
        if (loadFilter) {
            loadFilter.addEventListener("change", function () {
                renderLpcpBottomCharts();
            });
        }

        // v2.3.60 — Tải stats LPCP cho đúng tháng đang hiển thị trên lịch
        function loadLpcpStatsForMonth(baseDate) {
            renderLpcpBottomCharts();
        }

        // Monthly calendar navigation — fetch data when month changes (Issue 2)
        function reloadCalendarForMonth() {
            var calNode = byId("chartActivityCalendarMonthly");
            if (calNode) calNode.innerHTML = "<div class=\"dk-skeleton\"><div class=\"dk-skeleton-shimmer\"></div></div>";

            if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
            // Fetch a 3-month window centered on displayed month
            var from = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() - 1, 1);
            var to = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 2, 0);
            var url = "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" +
                asIsoDate(from) + "&denNgay=" + asIsoDate(to);

            // v2.3.60 — Reload LPCP calendar + stats cho tháng mới
            var urlLPCP = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" +
                asIsoDate(from) + "&denNgay=" + asIsoDate(to);

            state.lpcpCalendar = {};
            loadLpcpStatsForMonth(calMonthDate);

            Promise.all([
                requestJson(url).catch(function () { return []; }),
                requestJson(urlLPCP).catch(function () { return []; })
            ]).then(function (results) {
                state.activityCalendar = normalizeArray(results[0]);
                var lpcpArr = normalizeArray((results[1] && results[1].data) ? results[1].data : results[1]);
                lpcpArr.forEach(function (d) {
                    var k = String(d.NgayLam || d.ngayLam || "").substring(0, 10);
                    if (k) state.lpcpCalendar[k] = d;
                });
                renderActivityCalendarMonthly();
            });
        }

        var calPrev = byId("calPrevMonth");
        if (calPrev) {
            calPrev.addEventListener("click", function () {
                if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
                calRangeFrom = null; calRangeTo = null;
                calMonthDate = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() - 1, 1);
                reloadCalendarForMonth();
            });
        }
        var calNext = byId("calNextMonth");
        if (calNext) {
            calNext.addEventListener("click", function () {
                if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
                calRangeFrom = null; calRangeTo = null;
                calMonthDate = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 1, 1);
                reloadCalendarForMonth();
            });
        }
        var calApply = byId("calApplyRange");
        if (calApply) {
            calApply.addEventListener("click", function () {
                var fromEl = byId("calFromDate");
                var toEl = byId("calToDate");
                if (fromEl && fromEl.value && toEl && toEl.value) {
                    var calNode = byId("chartActivityCalendarMonthly");
                    if (calNode) calNode.innerHTML = "<div class=\"dk-skeleton\"><div class=\"dk-skeleton-shimmer\"></div></div>";

                    calRangeFrom = new Date(fromEl.value + "T00:00:00");
                    calRangeTo = new Date(toEl.value + "T00:00:00");
                    calMonthDate = new Date(calRangeFrom.getFullYear(), calRangeFrom.getMonth(), 1);

                    var urlLichGoc = "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" + asIsoDate(calRangeFrom) + "&denNgay=" + asIsoDate(calRangeTo);
                    var urlNKDK = "/api/DashboardKhoDesktop/GetNKDuKienByRange?tuNgay=" + asIsoDate(calRangeFrom) + "&denNgay=" + asIsoDate(calRangeTo);
                    var urlLPCP = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" + asIsoDate(calRangeFrom) + "&denNgay=" + asIsoDate(calRangeTo);

                    Promise.all([
                        requestJson(urlNKDK).catch(function () { return []; }),
                        requestJson(urlLichGoc).catch(function () { return []; }),
                        requestJson(urlLPCP).catch(function () { return []; })
                    ]).then(function (results) {
                        state.nkDuKien = normalizeArray(results[0]);

                        var rLPCP = results[2] || {};
                        var lpcpArr = normalizeArray(rLPCP.Tasks || rLPCP.data || rLPCP);
                        var invArr = normalizeArray(rLPCP.Inventory || []);

                        state.lpcpCalendar = {};
                        lpcpArr.forEach(function (d) {
                            var k = String(d.NgayLam || d.ngayLam || "").substring(0, 10);
                            if (k) state.lpcpCalendar[k] = d;
                        });

                        if (invArr.length > 0) {
                            state.activityCalendar = invArr.map(function (item) {
                                var inQty = toNumber(item.SoLuongNhapKho || item.SoLuongNhap || item.TotalIn || item.totalIn || item.SLNhap || 0);
                                var outQty = toNumber(item.SoLuongXuatHang || item.SoLuongXuat || item.TotalOut || item.totalOut || item.SLXuat || 0);
                                var kkQty = toNumber(item.SoLuongKiemKe || item.SoLuongKK || item.TotalKiemKe || item.totalKiemKe || item.SLKiemKe || 0);
                                return {
                                    NgayHoatDong: item.Ngay || item.ngay || item.NgayHoatDong,
                                    TotalIn: inQty,
                                    TotalOut: outQty,
                                    TotalKiemKe: kkQty,
                                    TotalActivity: inQty + outQty + kkQty
                                };
                            });
                        } else {
                            state.activityCalendar = normalizeArray(results[1]);
                        }

                        renderActivityCalendarMonthly();
                    }).catch(function () {
                        renderActivityCalendarMonthly();
                    });
                }
            });
        }

        // v2.3.46 — "Tổng quát" button: mở modal aggregate cho toàn bộ range
        var calOverview = byId("calOpenOverview");
        if (calOverview) {
            calOverview.addEventListener("click", function () {
                openCalendarOverviewModal();
            });
        }

        // v2.3.46 — Global Itemcode search
        var gsInput = byId("globalKhoSearch");
        var gsBtn = byId("globalKhoSearchBtn");
        var gsClear = byId("globalKhoSearchClear");
        var gsResult = byId("globalKhoSearchResult");
        var gsDebounceTimer = null;
        function runGlobalSearch(immediate) {
            if (!gsInput || !gsResult) return;
            var code = (gsInput.value || "").trim();
            if (!code) {
                gsResult.style.display = "none";
                gsResult.innerHTML = "";
                return;
            }
            // debounce
            if (!immediate) {
                if (gsDebounceTimer) clearTimeout(gsDebounceTimer);
                gsDebounceTimer = setTimeout(function () { runGlobalSearch(true); }, 300);
                return;
            }
            gsResult.style.display = "block";
            gsResult.innerHTML = "<div class=\"dk-empty\" style=\"padding:14px\">Đang tìm \"" + escapeHtml(code) + "\" ...</div>";
            if (gsBtn) gsBtn.disabled = true;
            requestJson("/api/DashboardKhoDesktop/GlobalSearch?itemcode=" + encodeURIComponent(code))
                .then(function (data) {
                    var arr = normalizeArray(data);
                    renderGlobalSearchResult(gsResult, code, arr[0] || null);
                })
                .catch(function (err) {
                    gsResult.innerHTML = "<div class=\"dk-empty dk-text-danger\" style=\"padding:14px\">Lỗi: " + escapeHtml(String(err && err.message || err)) + "</div>";
                })
                .finally(function () { if (gsBtn) gsBtn.disabled = false; });
        }
        if (gsBtn) gsBtn.addEventListener("click", function () { runGlobalSearch(true); });
        if (gsInput) {
            gsInput.addEventListener("input", function () { runGlobalSearch(false); });
            gsInput.addEventListener("keydown", function (e) {
                if (e.key === "Enter") { e.preventDefault(); runGlobalSearch(true); }
            });
        }
        if (gsClear) {
            gsClear.addEventListener("click", function () {
                if (gsInput) { gsInput.value = ""; gsInput.focus(); }
                if (gsResult) { gsResult.style.display = "none"; gsResult.innerHTML = ""; }
            });
        }

        // v2.3.33 — Legend toggle filter (click để bật/tắt hiển thị từng loại)
        var calLegend = byId("calActLegend");
        if (calLegend) {
            calLegend.addEventListener("click", function (e) {
                var btn = e.target.closest ? e.target.closest(".dk-cal-leg-toggle") : null;
                if (!btn) {
                    var t = e.target;
                    while (t && t !== calLegend) {
                        if (t.classList && t.classList.contains("dk-cal-leg-toggle")) { btn = t; break; }
                        t = t.parentNode;
                    }
                }
                if (!btn) return;
                var act = btn.getAttribute("data-act");
                if (!act) return;
                // Toggle filter state
                state.calActFilter[act] = !state.calActFilter[act];
                btn.classList.toggle("is-active", state.calActFilter[act]);
                // Re-render calendar
                renderActivityCalendarMonthly();
            });
        }

        // Modal search filter
        var searchInput = byId("detailSearchInput");
        if (searchInput) {
            searchInput.addEventListener("input", function () {
                filterDetailTable(this.value.trim());
            });
        }
    }

    function filterDetailTable(query) {
        var content = byId(ids.detailModalContent);
        if (!content) return;

        // v2.3.9 — Nếu có tabbed UI (modal ngày calendar), chỉ filter tab đang hiện;
        // ngược lại filter tất cả tbody trong modal.
        var visiblePanels = content.querySelectorAll(".dk-day-panel");
        var tbodies;
        if (visiblePanels.length > 0) {
            // Chỉ lấy tbody của panel đang visible
            tbodies = [];
            for (var p = 0; p < visiblePanels.length; p++) {
                if (visiblePanels[p].style.display !== "none") {
                    var tb = visiblePanels[p].querySelectorAll("tbody");
                    for (var tt = 0; tt < tb.length; tt++) tbodies.push(tb[tt]);
                }
            }
        } else {
            tbodies = content.querySelectorAll("tbody");
        }

        var lowerQuery = query.toLowerCase();
        var visibleTotal = 0, totalRows = 0;
        for (var b = 0; b < tbodies.length; b++) {
            var rows = tbodies[b].querySelectorAll("tr");
            for (var i = 0; i < rows.length; i++) {
                totalRows++;
                var match = !lowerQuery || rows[i].textContent.toLowerCase().indexOf(lowerQuery) >= 0;
                rows[i].style.display = match ? "" : "none";
                if (match) visibleTotal++;
            }
        }
        var countEl = byId("detailSearchCount");
        if (countEl) {
            countEl.textContent = query ? (visibleTotal + " / " + totalRows + " dòng") : "";
        }
    }

    function hasClass(node, className) {
        if (!node || !node.classList) return false;
        return node.classList.contains(className);
    }

    function findDetailNode(node) {
        var current = node;
        while (current && current !== document) {
            if (hasClass(current, "js-open-detail")) return current;
            current = current.parentNode;
        }
        return null;
    }

    function findCloseNode(node) {
        var current = node;
        while (current && current !== document) {
            if (hasClass(current, "js-close-modal")) return current;
            current = current.parentNode;
        }
        return null;
    }

    function addDays(baseDate, diffDays) {
        var date = new Date(baseDate.getFullYear(), baseDate.getMonth(), baseDate.getDate());
        date.setDate(date.getDate() + diffDays);
        return date;
    }

    function addMonths(baseDate, diffMonths) {
        return new Date(baseDate.getFullYear(), baseDate.getMonth() + diffMonths, 1);
    }

    function asIsoDate(date) {
        var year = date.getFullYear();
        var month = String(date.getMonth() + 1).padStart(2, "0");
        var day = String(date.getDate()).padStart(2, "0");
        return year + "-" + month + "-" + day;
    }

    function buildDemoData() {
        var now = new Date();
        return {
            overall: [
                {
                    TotalCapacity: 18420.5,
                    CapacityNPL: 11620.25,
                    CapacityPL: 6800.25,
                    UsedNPL: 8515.6,
                    UsedPL: 4272.4,
                    TotalVatTuNPL: 8120,
                    TotalVatTuPL: 3250,
                    TotalVatTu: 11370,
                    TotalFreeCapacity: 5632.5,
                    PercentNPL: 73.28,
                    PercentPL: 62.83,
                    TotalPercent: 69.42,
                    FreePercent: 30.58
                }
            ],
            customers: [
                { MaKH: "KH001", TenKH: "Viking Apparel", SLVatTu: 2140, CBMSDTrongKho: 1650.2 },
                { MaKH: "KH002", TenKH: "Northwind Garment", SLVatTu: 1830, CBMSDTrongKho: 1422.8 },
                { MaKH: "KH003", TenKH: "EverWin Textile", SLVatTu: 1484, CBMSDTrongKho: 1218.5 },
                { MaKH: "KH004", TenKH: "Sunrise Uniform", SLVatTu: 1202, CBMSDTrongKho: 980.3 },
                { MaKH: "KH005", TenKH: "Ocean Knit", SLVatTu: 1090, CBMSDTrongKho: 912.1 },
                { MaKH: "KH006", TenKH: "BlueSky Sport", SLVatTu: 960, CBMSDTrongKho: 840.4 },
                { MaKH: "KH007", TenKH: "Lotus Fashion", SLVatTu: 846, CBMSDTrongKho: 755.7 },
                { MaKH: "KH008", TenKH: "Apex Wear", SLVatTu: 732, CBMSDTrongKho: 640.2 }
            ],
            racks: [
                { Module: 1, TenDay: "Dãy A", TenKe: "A-01", TongCBMSuDungTrongKe: 315.5, TongCBMTrongKe: 342.0, SLVatTu: 268 },
                { Module: 1, TenDay: "Dãy A", TenKe: "A-02", TongCBMSuDungTrongKe: 302.1, TongCBMTrongKe: 330.0, SLVatTu: 254 },
                { Module: 1, TenDay: "Dãy B", TenKe: "B-05", TongCBMSuDungTrongKe: 288.7, TongCBMTrongKe: 328.0, SLVatTu: 247 },
                { Module: 1, TenDay: "Dãy C", TenKe: "C-03", TongCBMSuDungTrongKe: 275.2, TongCBMTrongKe: 320.0, SLVatTu: 231 },
                { Module: 2, TenDay: "Dãy P1", TenKe: "P1-01", TongCBMSuDungTrongKe: 192.6, TongCBMTrongKe: 212.0, SLVatTu: 198 },
                { Module: 2, TenDay: "Dãy P1", TenKe: "P1-02", TongCBMSuDungTrongKe: 180.8, TongCBMTrongKe: 208.0, SLVatTu: 176 },
                { Module: 2, TenDay: "Dãy P2", TenKe: "P2-03", TongCBMSuDungTrongKe: 171.4, TongCBMTrongKe: 205.0, SLVatTu: 168 },
                { Module: 2, TenDay: "Dãy P2", TenKe: "P2-05", TongCBMSuDungTrongKe: 162.9, TongCBMTrongKe: 198.0, SLVatTu: 161 }
            ],
            inbound: [
                { PO: "PO-240501", SoLo: "SL-001", MaDH: "DH-1001", MaHang: "MH-JACKET-01", SoLuongSP: 1280, NgayNKDuKien: asIsoDate(addDays(now, 1)) },
                { PO: "PO-240502", SoLo: "SL-002", MaDH: "DH-1002", MaHang: "MH-SHIRT-06", SoLuongSP: 960, NgayNKDuKien: asIsoDate(addDays(now, 2)) },
                { PO: "PO-240503", SoLo: "SL-003", MaDH: "DH-1003", MaHang: "MH-PANTS-03", SoLuongSP: 760, NgayNKDuKien: asIsoDate(addDays(now, 3)) },
                { PO: "PO-240504", SoLo: "SL-004", MaDH: "DH-1004", MaHang: "MH-POLO-02", SoLuongSP: 1140, NgayNKDuKien: asIsoDate(addDays(now, 4)) },
                { PO: "PO-240505", SoLo: "SL-005", MaDH: "DH-1005", MaHang: "MH-HOODIE-08", SoLuongSP: 520, NgayNKDuKien: asIsoDate(addDays(now, 5)) }
            ],
            outboundReady: [
                { MaLenhSanXuat: "LSX-240601", MaLenh: "601", MaDVSX: "DVSX_1", TenHang: "Jacket Running", KhachHang: "Viking Apparel", KHCat: asIsoDate(addDays(now, 2)), DuKienCat: asIsoDate(addDays(now, 9)) },
                { MaLenhSanXuat: "LSX-240602", MaLenh: "602", MaDVSX: "DVSX_2", TenHang: "Shirt Office", KhachHang: "Northwind Garment", KHCat: asIsoDate(addDays(now, 1)), DuKienCat: asIsoDate(addDays(now, 8)) },
                { MaLenhSanXuat: "LSX-240603", MaLenh: "603", MaDVSX: "DVSX_3", TenHang: "Polo Classic", KhachHang: "EverWin Textile", KHCat: asIsoDate(addDays(now, 3)), DuKienCat: asIsoDate(addDays(now, 10)) },
                { MaLenhSanXuat: "LSX-240604", MaLenh: "604", MaDVSX: "DVSX_5", TenHang: "Sport Pant", KhachHang: "Sunrise Uniform", KHCat: asIsoDate(addDays(now, 4)), DuKienCat: asIsoDate(addDays(now, 11)) }
            ],
            outboundRunning: [
                { MaLenhSX: "LSX-240511", MaLenh: "511", MaGop: "GOP-301", TenHang: "Jacket Running", TenKH: "Viking Apparel", NgayXuatHang: asIsoDate(addDays(now, -1)), SLXuat: 1350.5 },
                { MaLenhSX: "LSX-240512", MaLenh: "512", MaGop: "GOP-302", TenHang: "Shirt Office", TenKH: "Northwind Garment", NgayXuatHang: asIsoDate(now), SLXuat: 980.0 },
                { MaLenhSX: "LSX-240513", MaLenh: "513", MaGop: "GOP-303", TenHang: "Polo Classic", TenKH: "Lotus Fashion", NgayXuatHang: asIsoDate(addDays(now, -2)), SLXuat: 740.25 },
                { MaLenhSX: "LSX-240514", MaLenh: "514", MaGop: "GOP-304", TenHang: "Sport Pant", TenKH: "BlueSky Sport", NgayXuatHang: asIsoDate(now), SLXuat: 1125.0 }
            ]
        };
    }

    function loadDemoData() {
        var demo = buildDemoData();
        state.overall = demo.overall;
        state.customers = demo.customers;
        state.racks = demo.racks;
        state.inbound = demo.inbound;
        state.outboundReady = demo.outboundReady;
        state.outboundRunning = demo.outboundRunning;
        state.lastUpdated = new Date();
        renderAll();
    }

    var loadedPages = { 1: false, 2: false, 3: false };

    function loadData() {
        if (state.loading) return;
        if (isDemoMode) {
            loadDemoData();
            return;
        }

        var now = new Date();
        var inFromStr = now.toISOString().slice(0, 10);
        var inTo = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 14);
        var inToStr = inTo.toISOString().slice(0, 10);

        setLoading(true);

        function safeJson(url) {
            return requestJson(url).then(function (r) { return r; }).catch(function (e) {
                console.warn("API lỗi [" + url + "]:", e.message);
                return [];
            });
        }

        var BASE = "/api/DashboardKhoDesktop/";
        // v2.4.6 — Query string filter ngày global
        var dfFrom = state.dateFilter && state.dateFilter.from ? state.dateFilter.from : "";
        var dfTo = state.dateFilter && state.dateFilter.to ? state.dateFilter.to : "";
        var dfQS = (dfFrom && dfTo) ? "?tuNgay=" + encodeURIComponent(dfFrom) + "&denNgay=" + encodeURIComponent(dfTo) : "";

        var lpcpFrom = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        var lpcpTo = new Date(now.getFullYear(), now.getMonth() + 2, 0);
        function _isoDate(d) { return d.getFullYear() + "-" + String(d.getMonth() + 1).padStart(2, "0") + "-" + String(d.getDate()).padStart(2, "0"); }
        var LPCP_URL = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" + encodeURIComponent(_isoDate(lpcpFrom)) + "&denNgay=" + encodeURIComponent(_isoDate(lpcpTo));

        // === PAGE 1 priority — load + render NGAY (user thấy ngay khi vào) ===
        loadedPages[1] = true;
        var p1a = safeJson(BASE + "GetOverallCapacity").then(function (r) {
            state.overall = normalizeArray(r);
            renderMetricCards();
            renderCapacityChart();
            renderCapacityBarChart();
        });
        var p1b = safeJson(BASE + "GetCustomers").then(function (r) {
            state.customers = normalizeArray(r);
            populateCustomerFilter();
            renderCustomerPieChart();
            renderCustomersTable();
        });
        var p1c = safeJson(BASE + "GetDistinctMaterialCount").then(function (r) {
            state.distinctMat = normalizeArray(r);
            renderMetricCards();
        });
        var inboundTo = new Date(now);
        inboundTo.setDate(inboundTo.getDate() + 365);
        var inboundQS = "?tuNgay=" + encodeURIComponent(now.toISOString().slice(0, 10)) + "&denNgay=" + encodeURIComponent(inboundTo.toISOString().slice(0, 10));

        var p1d = safeJson(BASE + "GetChuanBiVe" + inboundQS).then(function (r) {
            state.inbound = normalizeArray(r);
            renderMetricCards();
        });
        var p1e = safeJson(BASE + "GetChuanBiXuat" + dfQS).then(function (r) {
            state.outboundReady = normalizeArray(r);
            renderMetricCards();
        });
        var p1f = safeJson(BASE + "GetDangXuat" + dfQS).then(function (r) {
            state.outboundRunning = normalizeArray(r);
            renderMetricCards();
        });
        var p1g = safeJson(BASE + "GetMoMComparison").then(function (r) {
            state.momComparison = normalizeArray(r);
            renderMoMStrip();
        });
        var p1h = safeJson(BASE + "GetRacks").then(function (r) {
            state.racks = normalizeArray(r);
        });
        var p1i = safeJson(BASE + "GetTop5?isNhieuNhat=1&loaiNPL=1").then(function (r) {
            state.top15MaxNL = normalizeArray(r);
        });
        var p1j = safeJson(BASE + "GetTop5?isNhieuNhat=1&loaiNPL=2").then(function (r) {
            state.top15MaxPL = normalizeArray(r);
        });
        var p1k = safeJson(BASE + "GetThanhGiaHangTon").then(function (r) {
            var arr = normalizeArray(r);
            state.thanhGia = arr.length > 0 ? arr[0] : null;
            renderMetricCards();
        });

        // v2.4.0 — 6 section mới của Tổng quan (stub API)
        var p1l = safeJson(BASE + "GetCongViecChoXuLy").then(function (r) {
            state.todoList = normalizeArray(r);
            renderTodoList();
        });
        var p1m = safeJson(BASE + "GetTop5VatTuDungTich").then(function (r) {
            state.top5VT = normalizeArray(r);
            renderTop5VTTable();
        });
        var p1n = safeJson(BASE + "GetTop5KhachHangTonKho").then(function (r) {
            state.top5KH = normalizeArray(r);
            renderTop5KHTable();
        });
        var p1o = safeJson(BASE + "GetVatTuSapHetHan").then(function (r) {
            state.vtHetHan = normalizeArray(r);
            renderHetHanTable();
        });
        var p1p = safeJson(BASE + "GetGiaTriTonKhoTheoNhom").then(function (r) {
            state.giaTriNhom = normalizeArray(r);
            renderGiaTriTheoNhomChart();
        });
        var p1q = safeJson(BASE + "GetTinhHinhKiemKe").then(function (r) {
            var arr = normalizeArray(r);
            state.kiemKe = arr.length > 0 ? arr[0] : null;
            renderKiemKeBox();
        });

        // v2.4.6 — 6 KPI mới (filter ngày) + alerts + hieusuat
        var p1r = safeJson(BASE + "GetTongNhap" + dfQS).then(function (r) {
            var arr = normalizeArray(r); state.kpiTongNhap = arr[0] || null; renderMetricCards();
        });
        var p1s = safeJson(BASE + "GetTongXuat" + dfQS).then(function (r) {
            var arr = normalizeArray(r); state.kpiTongXuat = arr[0] || null; renderMetricCards();
        });
        var p1t = safeJson(BASE + "GetTonKho" + (dfTo ? "?denNgay=" + encodeURIComponent(dfTo) : "")).then(function (r) {
            var arr = normalizeArray(r); state.kpiTonKho = arr[0] || null; renderMetricCards();
        });
        var p1u = safeJson(BASE + "GetTonDauKy" + (dfFrom ? "?tuNgay=" + encodeURIComponent(dfFrom) : "")).then(function (r) {
            var arr = normalizeArray(r); state.kpiTonDauKy = arr[0] || null; renderMetricCards();
        });
        var p1v = safeJson(BASE + "GetPODangTre").then(function (r) {
            var arr = normalizeArray(r); state.kpiPODangTre = arr[0] || null; renderMetricCards();
        });
        var p1w = safeJson(BASE + "GetGiaTriTon" + (dfTo ? "?denNgay=" + encodeURIComponent(dfTo) : "")).then(function (r) {
            var arr = normalizeArray(r); state.kpiGiaTriTon = arr[0] || null; renderMetricCards();
        });
        // Page 2 widget mới — load lazy nhưng đặt vào Promise.all để re-fetch khi Áp dụng
        var p1x = safeJson(BASE + "GetCanhBaoTonKho").then(function (r) {
            state.alerts = normalizeArray(r); renderAlertsList();
        });
        var p1y = safeJson(BASE + "GetHieuSuatHoatDong" + dfQS).then(function (r) {
            state.hieuSuat = normalizeArray(r); renderHieuSuatGauges();
        });
        var pLPCP = requestJson(LPCP_URL).then(function (res) {
            // Đọc Tasks và Inventory từ cấu trúc API mới
            var lpcpArr = normalizeArray(res.Tasks || res.data || res);
            var invArr = normalizeArray(res.Inventory || []);

            lpcpArr.forEach(function (d) {
                var k = String(d.NgayLam || "").substring(0, 10);
                if (k) state.lpcpCalendar[k] = d;
            });

            // Ánh xạ dữ liệu Inventory mới gán đè vào activityCalendar
            if (invArr.length > 0) {
                state.activityCalendar = invArr.map(function (item) {
                    var inQty = toNumber(item.SoLuongNhapKho);
                    var outQty = toNumber(item.SoLuongXuatHang);
                    var kkQty = toNumber(item.SoLuongKiemKe);
                    return {
                        NgayHoatDong: item.Ngay,
                        TotalIn: inQty,
                        TotalOut: outQty,
                        TotalKiemKe: kkQty,
                        TotalActivity: inQty + outQty + kkQty
                    };
                });
            }
        }).catch(function (e) {
            console.warn("Lỗi tải lịch phân công:", e);
            return [];
        });

        // Bỏ gọi API GetActivityCalendar cũ để không bị ghi đè dữ liệu
        var pActivity = Promise.resolve([]);

        Promise.all([p1a, p1b, p1c, p1d, p1e, p1f, p1g, p1h, p1i, p1j, p1k, p1l, p1m, p1n, p1o, p1p, p1q, p1r, p1s, p1t, p1u, p1v, p1w, p1x, p1y, pLPCP, pActivity]).then(function () {
            state.lastUpdated = new Date();
            if (!skipLoadingState) {
                setLoading(false);
            } else {
                var button = byId(ids.refreshButton);
                if (button) { button.disabled = false; button.textContent = "Làm mới"; }
                state.loading = false;
            }

            // Render lại lịch kho (Hàm này giờ sẽ có state.lpcpCalendar để vẽ chấm)
            renderActivityCalendarMonthly();

            // Tự động load nội dung của ngày hôm nay
            var today = new Date();
            if (typeof showLpcpInlineDetail === 'function') {
                showLpcpInlineDetail(_isoDate(today));
            }

            // Render 3 biểu đồ mới ở cuối trang với dữ liệu thực tế
            if (typeof renderLpcpBottomCharts === 'function') {
                renderLpcpBottomCharts();
            }

            if ((currentPage === 2 || currentPage === 3) && !skipReloadCurrent) {
                loadedPages[currentPage] = false;
                loadPageData(currentPage);
            }
            setTimeout(injectMaximizeButtons, 60);
        }).catch(function (e) {
            console.error("loadData Promise.all error:", e);
            if (!skipLoadingState) {
                setLoading(false);
            } else {
                var button = byId(ids.refreshButton);
                if (button) { button.disabled = false; button.textContent = "Làm mới"; }
                state.loading = false;
            }
        });
    }
    function loadPageData(pageNum) {
        if (loadedPages[pageNum]) return Promise.resolve();
        loadedPages[pageNum] = true;

        function safeJson(url) {
            return requestJson(url).then(function (r) { return r; }).catch(function (e) {
                console.warn("API lỗi [" + url + "]:", e.message);
                return [];
            });
        }
        var BASE = "/api/DashboardKhoDesktop/";

        if (pageNum === 2) {
            var promises = [];
            promises.push(safeJson(BASE + "GetRacks").then(function (r) {
                state.racks = normalizeArray(r);
                renderRacksTable();
                // renderRacksHeatmap();
            }));
            promises.push(safeJson(BASE + "GetFlowTrend12T").then(function (r) {
                state.flowTrend12T = normalizeArray(r);
                var fromEl = byId("flowFromDate"), toEl = byId("flowToDate");
                if (fromEl && toEl) {
                    var today = new Date();
                    var defStart = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);
                    if (!fromEl.value) fromEl.value = asIsoDate(defStart);
                    if (!toEl.value) toEl.value = asIsoDate(today);
                    loadAndRenderFlowByRange(new Date(fromEl.value + "T00:00:00"), new Date(toEl.value + "T00:00:00"));
                } else {
                    renderFlowTrendChart();
                }
            }));
            promises.push(safeJson(BASE + "GetAgeStock").then(function (r) {
                state.ageStock = normalizeArray(r);
                renderAgeStockChart();
            }));
            // Top 5 NL/PL charts — fire and render independently
            renderTop5MaxChart();
            renderTop5MinChart();
            // v2.7.2 FIX: Re-fetch alerts nếu chưa có data (khi navigate sang page 2 trước khi loadData hoàn thành)
            if (!state.alerts || state.alerts.length === 0) {
                promises.push(safeJson(BASE + "GetCanhBaoTonKho").then(function (r) {
                    state.alerts = normalizeArray(r);
                    renderAlertsList();
                }));
            } else {
                renderAlertsList();
            }
            // v2.7.1 FIX: Fetch hieuSuat nếu chưa có data (khi navigate sang page 2 trước khi loadData hoàn thành)
            if (!state.hieuSuat || state.hieuSuat.length === 0) {
                var dfQS2 = "?tuNgay=" + encodeURIComponent(state.dateFilter.from) + "&denNgay=" + encodeURIComponent(state.dateFilter.to);
                promises.push(safeJson(BASE + "GetHieuSuatHoatDong" + dfQS2).then(function (r) {
                    state.hieuSuat = normalizeArray(r);
                    renderHieuSuatGauges();
                }));
            } else {
                renderHieuSuatGauges();
            }
            setTimeout(injectMaximizeButtons, 60);
            return Promise.all(promises);
        }

        if (pageNum === 3) {
            // Sử dụng calMonthDate nếu có (để giữ nguyên tháng đang xem), nếu không thì dùng now
            var baseMonth = (typeof calMonthDate !== "undefined" && calMonthDate) ? calMonthDate : new Date();
            var from = new Date(baseMonth.getFullYear(), baseMonth.getMonth() - 1, 1);
            var to = new Date(baseMonth.getFullYear(), baseMonth.getMonth() + 2, 0);

            var urlLichGoc = BASE + "GetActivityCalendar?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
            var urlNKDK = BASE + "GetNKDuKienByRange?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
            var urlLPCP = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);

            // Khởi tạo state rỗng
            state.nkDuKien = [];
            state.activityCalendar = [];
            state.lpcpCalendar = {};

            // GỌI 3 API SONG SONG VÀ ĐỢI TẤT CẢ HOÀN TẤT
            return Promise.all([
                safeJson(urlNKDK),
                safeJson(urlLichGoc),
                requestJson(urlLPCP).catch(function (e) {
                    console.warn("API LPCP lỗi, bỏ qua hiển thị:", e);
                    return [];
                })
            ]).then(function (results) {
                state.nkDuKien = normalizeArray(results[0]);

                // Xử lý dữ liệu LPCP và Inventory mới từ kết quả API
                var rLPCP = results[2] || {};
                var lpcpArr = normalizeArray(rLPCP.Tasks || rLPCP.data || rLPCP);
                var invArr = normalizeArray(rLPCP.Inventory || []);

                lpcpArr.forEach(function (d) {
                    var k = String(d.NgayLam || d.ngayLam || "").substring(0, 10);
                    if (k) state.lpcpCalendar[k] = d;
                });

                // [FIX] Cập nhật Inventory cho Lịch — ưu tiên từ API LPCP, fallback API cũ
                if (invArr.length > 0) {
                    state.activityCalendar = invArr.map(function (item) {
                        var inQty = toNumber(item.SoLuongNhapKho || item.SoLuongNhap || item.TotalIn || item.totalIn || item.SLNhap || 0);
                        var outQty = toNumber(item.SoLuongXuatHang || item.SoLuongXuat || item.TotalOut || item.totalOut || item.SLXuat || 0);
                        var kkQty = toNumber(item.SoLuongKiemKe || item.SoLuongKK || item.TotalKiemKe || item.totalKiemKe || item.SLKiemKe || 0);
                        return {
                            NgayHoatDong: item.Ngay || item.ngay || item.NgayHoatDong,
                            TotalIn: inQty,
                            TotalOut: outQty,
                            TotalKiemKe: kkQty,
                            TotalActivity: inQty + outQty + kkQty
                        };
                    });
                } else {
                    // Fallback: dùng dữ liệu từ GetActivityCalendar (API cũ)
                    state.activityCalendar = normalizeArray(results[1]);
                }

                // RENDER LỊCH 1 LẦN DUY NHẤT SAU KHI ĐÃ CÓ FULL DỮ LIỆU
                renderActivityCalendarMonthly();
                setTimeout(injectMaximizeButtons, 60);

                // Load LPCP stats after data is ready
                loadLpcpStatsForMonth(calMonthDate || now);

                // Tự động load chi tiết ngày hiện tại cho sidebar
                if (typeof showLpcpInlineDetail === 'function') {
                    showLpcpInlineDetail(asIsoDate(new Date()));
                }
            }).catch(function (e) {
                console.warn("loadPageData page 3 error:", e);
            });
        }

        return Promise.resolve();
    }

    function triggerSequentialReload() {
        if (currentPage === 2 || currentPage === 3) {
            setLoading(true); // Hiển thị skeleton cho toàn bộ các tab
            var p = loadPageData(currentPage);
            if (p && p.then) {
                p.then(function () { loadData(true, true); }).catch(function () { loadData(true, true); });
            } else {
                loadData();
            }
        } else {
            loadData();
        }
    }

    // ─── Sidebar Toggle ─────────────────────────────────────────────────────────
    function bindSidebarToggle() {
        var sidebar = byId("dkSidebar");
        if (!sidebar) return;
        function doToggle() {
            sidebar.classList.toggle("collapsed");
            try { localStorage.setItem("dkSidebarCollapsed", sidebar.classList.contains("collapsed") ? "1" : "0"); } catch (e) { }
        }
        var toggle = byId("sidebarToggle");
        if (toggle) toggle.addEventListener("click", doToggle);
        var topToggle = byId("sidebarToggleTop");
        if (topToggle) topToggle.addEventListener("click", doToggle);

        var searchBox = document.querySelector(".dk-sb-search-box");
        if (searchBox) {
            searchBox.addEventListener("click", function () {
                if (sidebar.classList.contains("collapsed")) {
                    sidebar.classList.remove("collapsed");
                    try { localStorage.setItem("dkSidebarCollapsed", "0"); } catch (e) { }
                    var inp = byId("globalKhoSearch");
                    if (inp) setTimeout(function () { inp.focus(); }, 280);
                }
            });
        }
        try {
            if (localStorage.getItem("dkSidebarCollapsed") === "1") {
                sidebar.classList.add("collapsed");
            }
        } catch (e) { }
    }

    var flowRangeFrom = null;
    var flowRangeTo = null;

    function bindFlowRangePicker() {
        var fromEl = byId("flowFromDate");
        var toEl = byId("flowToDate");
        var applyBtn = byId("flowApplyRange");
        if (!fromEl || !toEl || !applyBtn) return;

        // Default range: 30 days back → today
        var today = new Date();
        var startDefault = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);
        fromEl.value = asIsoDate(startDefault);
        toEl.value = asIsoDate(today);

        applyBtn.addEventListener("click", function () {
            if (!fromEl.value || !toEl.value) return;
            var from = new Date(fromEl.value + "T00:00:00");
            var to = new Date(toEl.value + "T00:00:00");
            if (isNaN(from.getTime()) || isNaN(to.getTime())) return;
            if (from > to) { var tmp = from; from = to; to = tmp; }
            flowRangeFrom = from;
            flowRangeTo = to;
            loadAndRenderFlowByRange(from, to);
        });
    }

    function loadAndRenderFlowByRange(fromDate, toDate) {
        var node = byId(ids.chartFlowTrend);
        if (node) node.innerHTML = "<div class=\"dk-empty\" style=\"padding:20px\">Đang tải dữ liệu...</div>";
        var url = "/api/DashboardKhoDesktop/GetFlowTrendByRange?tuNgay=" +
            asIsoDate(fromDate) + "&denNgay=" + asIsoDate(toDate);
        requestJson(url)
            .then(function (data) {
                var arr = normalizeArray(data);
                if (!arr || arr.length === 0) {
                    if (node) node.innerHTML = "<div class=\"dk-empty\">Không có dữ liệu trong khoảng đã chọn</div>";
                    return;
                }
                var labels = [], inbound = [], outbound = [], stock = [];
                for (var i = 0; i < arr.length; i++) {
                    var ngay = String(arr[i].Ngay || "").substring(0, 10);
                    var parts = ngay.split("-");
                    var shortLbl = parts.length === 3 ? (parts[2] + "/" + parts[1]) : ngay;
                    labels.push(shortLbl);
                    inbound.push(toNumber(arr[i].TotalIn));
                    outbound.push(toNumber(arr[i].TotalOut));
                    stock.push(toNumber(arr[i].TotalStock));
                }
                // Lưu lại vào state để "Xem chi tiết" có thể đọc
                state.flowTrendRangeRaw = arr;
                renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
            })
            .catch(function () {
                if (node) node.innerHTML = "<div class=\"dk-empty\">Lỗi tải dữ liệu</div>";
            });
    }

    function bindPeriodSelector() {
        bindFlowRangePicker();
    }

    function renderFlowTrendQuarter() {
        if (!state.flowTrend12T || state.flowTrend12T.length === 0) {
            renderFlowTrendChart(); return;
        }
        var quarterMap = {};
        var quarterOrder = [];
        for (var i = 0; i < state.flowTrend12T.length; i++) {
            var t = state.flowTrend12T[i];
            var q = Math.ceil(toNumber(t.Thang) / 3);
            var key = "Q" + q + "/" + String(t.Nam).slice(-2);
            if (!quarterMap[key]) {
                quarterMap[key] = { lbl: key, inn: 0, out: 0, stock: 0 };
                quarterOrder.push(key);
            }
            quarterMap[key].inn += toNumber(t.TotalIn);
            quarterMap[key].out += toNumber(t.TotalOut);
            quarterMap[key].stock = toNumber(t.TotalStock); // lấy tháng cuối quý
        }
        var labels = [], inbound = [], outbound = [], stock = [];
        for (var qi = 0; qi < quarterOrder.length; qi++) {
            var qk = quarterOrder[qi];
            labels.push(quarterMap[qk].lbl);
            inbound.push(quarterMap[qk].inn);
            outbound.push(quarterMap[qk].out);
            stock.push(quarterMap[qk].stock);
        }
        renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
    }

    function renderFlowTrendYear() {
        // Gộp 12 tháng thành từng năm
        if (!state.flowTrend12T || state.flowTrend12T.length === 0) {
            renderFlowTrendChart(); return;
        }
        var yearMap = {};
        var yearOrder = [];
        for (var i = 0; i < state.flowTrend12T.length; i++) {
            var t = state.flowTrend12T[i];
            var yr = String(t.Nam);
            if (!yearMap[yr]) {
                yearMap[yr] = { inn: 0, out: 0, stock: 0 };
                yearOrder.push(yr);
            }
            yearMap[yr].inn += toNumber(t.TotalIn);
            yearMap[yr].out += toNumber(t.TotalOut);
            yearMap[yr].stock = toNumber(t.TotalStock);
        }
        var labels = [], inbound = [], outbound = [], stock = [];
        for (var yi = 0; yi < yearOrder.length; yi++) {
            labels.push(yearOrder[yi]);
            inbound.push(yearMap[yearOrder[yi]].inn);
            outbound.push(yearMap[yearOrder[yi]].out);
            stock.push(yearMap[yearOrder[yi]].stock);
        }
        renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
    }

    function loadAndRenderFlowWeekly() {
        var node = byId(ids.chartFlowTrend);
        if (node) node.innerHTML = "<div class=\"dk-empty\" style=\"padding:20px\">Đang tải dữ liệu tuần...</div>";
        requestJson("/api/DashboardKhoDesktop/GetFlowTrendWeekly")
            .then(function (data) {
                var arr = normalizeArray(data);
                if (!arr || arr.length === 0) { renderFlowTrendChart(); return; }
                var labels = [], inbound = [], outbound = [], stock = [];
                for (var i = 0; i < arr.length; i++) {
                    labels.push("T" + arr[i].Tuan + "/" + String(arr[i].Nam).slice(-2));
                    inbound.push(toNumber(arr[i].TotalIn));
                    outbound.push(toNumber(arr[i].TotalOut));
                    stock.push(toNumber(arr[i].TotalStock));
                }
                renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
            })
            .catch(function () { renderFlowTrendChart(); });
    }

    // Hàm vẽ biểu đồ flow với dữ liệu tùy chỉnh 
    function renderFlowTrendCustom(series) {
        var node = byId(ids.chartFlowTrend);
        if (!node) return;

        function getNiceMax(value) {
            if (!isFinite(value) || value <= 0) return 10;
            var exp = Math.pow(10, Math.floor(Math.log(value) / Math.LN10));
            var base = value / exp;
            var niceBase = base <= 1 ? 1 : (base <= 2 ? 2 : (base <= 5 ? 5 : 10));
            return niceBase * exp;
        }

        var N = series.labels.length;

        var maxBars = 1;
        for (var ii = 0; ii < N; ii++) {
            if (series.inbound[ii] > maxBars) maxBars = series.inbound[ii];
            if (series.outbound[ii] > maxBars) maxBars = series.outbound[ii];
        }
        var maxStock = 1;
        for (var si = 0; si < N; si++) {
            if (series.stock[si] > maxStock) maxStock = series.stock[si];
        }
        maxBars = getNiceMax(maxBars * 1.15);
        maxStock = getNiceMax(maxStock * 1.15);

        var W = 820, H = 320;
        var L = 64, R = 64, T = 30, B = 46;
        var plotW = W - L - R, plotH = H - T - B;
        var axisY = T + plotH;

        function groupX(idx) {
            if (N <= 1) return L + plotW / 2;
            var margin = 22;
            return (L + margin) + (idx / (N - 1)) * (plotW - margin * 2);
        }
        function yScaleBars(v) { return T + (1 - v / maxBars) * plotH; }
        function yScaleStock(v) { return T + (1 - v / maxStock) * plotH; }

        // Bar width auto-shrinks with density; ≥60 points → use line for bars too.
        var groupStep = N > 1 ? (plotW / (N - 1)) : plotW;
        var useBars = N <= 45;
        var barW = useBars ? Math.max(2, Math.min(20, groupStep * 0.32)) : 0;
        var gap = useBars ? 2 : 0;

        var defs = "<defs>" +
            "<linearGradient id=\"gradIn2\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#74c7f8\"/><stop offset=\"100%\" stop-color=\"#3d9de8\"/></linearGradient>" +
            "<linearGradient id=\"gradOut2\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#c4b5fd\"/><stop offset=\"100%\" stop-color=\"#8b5cf6\"/></linearGradient>" +
            "<filter id=\"bsf2\" x=\"-20%\" y=\"-20%\" width=\"140%\" height=\"140%\"><feDropShadow dx=\"0\" dy=\"2\" stdDeviation=\"2\" flood-color=\"#0f172a\" flood-opacity=\"0.12\"/></filter>" +
            "</defs>";

        // Grid + LEFT axis labels (bars)
        var grid = "";
        for (var g = 0; g <= 4; g++) {
            var gv = (maxBars / 4) * g;
            var gy = yScaleBars(gv);
            grid += "<line x1=\"" + L + "\" y1=\"" + gy + "\" x2=\"" + (W - R) + "\" y2=\"" + gy + "\" stroke=\"#e2eaf4\" stroke-dasharray=\"4 3\" stroke-width=\"1\"/>";
            grid += "<text x=\"" + (L - 6) + "\" y=\"" + (gy + 4) + "\" text-anchor=\"end\" class=\"dk-flow-label\" style=\"font-size:10px;fill:#3d9de8;font-weight:700\">" + formatNumber(gv, 0) + "</text>";
        }
        // RIGHT axis labels (stock)
        for (var gs = 0; gs <= 4; gs++) {
            var gvS = (maxStock / 4) * gs;
            var gyS = yScaleStock(gvS);
            grid += "<text x=\"" + (W - R + 6) + "\" y=\"" + (gyS + 4) + "\" text-anchor=\"start\" class=\"dk-flow-label\" style=\"font-size:10px;fill:#f97316;font-weight:700\">" + formatNumber(gvS, 0) + "</text>";
        }
        grid += "<line x1=\"" + L + "\" y1=\"" + axisY + "\" x2=\"" + (W - R) + "\" y2=\"" + axisY + "\" stroke=\"#b0c4d8\" stroke-width=\"1.5\"/>";
        // axis titles
        var axisTitles = "<text x=\"" + (L - 6) + "\" y=\"" + (T - 8) + "\" text-anchor=\"end\"   class=\"dk-flow-axis-title\" style=\"font-size:10px;fill:#3d9de8;font-weight:800\">Nhập/Xuất</text>" +
            "<text x=\"" + (W - R + 6) + "\" y=\"" + (T - 8) + "\" text-anchor=\"start\" class=\"dk-flow-axis-title\" style=\"font-size:10px;fill:#f97316;font-weight:800\">Tồn kho</text>";

        var barsIn = "", barsOut = "";
        if (useBars) {
            for (var bi = 0; bi < N; bi++) {
                var gx = groupX(bi);
                var yin = yScaleBars(series.inbound[bi]);
                var hin = Math.max(0, axisY - yin);
                var yout = yScaleBars(series.outbound[bi]);
                var hout = Math.max(0, axisY - yout);
                if (series.inbound[bi] > 0) barsIn += "<rect x=\"" + (gx - barW - gap / 2) + "\" y=\"" + yin + "\" width=\"" + barW + "\" height=\"" + Math.max(2, hin) + "\" rx=\"2\" fill=\"url(#gradIn2)\"  filter=\"url(#bsf2)\"/>";
                if (series.outbound[bi] > 0) barsOut += "<rect x=\"" + (gx + gap / 2) + "\" y=\"" + yout + "\" width=\"" + barW + "\" height=\"" + Math.max(2, hout) + "\" rx=\"2\" fill=\"url(#gradOut2)\" filter=\"url(#bsf2)\"/>";
            }
        } else {
            var pIn = "", pOut = "";
            for (var bi2 = 0; bi2 < N; bi2++) {
                var gx2 = groupX(bi2);
                var yin2 = yScaleBars(series.inbound[bi2]);
                var yout2 = yScaleBars(series.outbound[bi2]);
                pIn += (bi2 === 0 ? "M " : " L ") + gx2 + " " + yin2;
                pOut += (bi2 === 0 ? "M " : " L ") + gx2 + " " + yout2;
            }
            barsIn = "<path d=\"" + pIn + "\" fill=\"none\" stroke=\"#3d9de8\" stroke-width=\"2.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/>";
            barsOut = "<path d=\"" + pOut + "\" fill=\"none\" stroke=\"#8b5cf6\" stroke-width=\"2.2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/>";
        }

        var colorLine = "#f97316";
        var lineD = "", dots = "", stockLabels = "";
        var showStockLabels = N <= 15;        // chỉ vẽ label số khi ít điểm
        var dotRadius = N > 60 ? 2 : (N > 30 ? 3 : 5);
        for (var pi = 0; pi < N; pi++) {
            var px = groupX(pi);
            var py = yScaleStock(series.stock[pi]);
            if (pi === 0) lineD = "M " + px + " " + py;
            else lineD += " L " + px + " " + py;
            dots += "<circle cx=\"" + px + "\" cy=\"" + py + "\" r=\"" + dotRadius + "\" fill=\"#fff\" stroke=\"" + colorLine + "\" stroke-width=\"1.8\"/>";
            if (showStockLabels) {
                stockLabels += "<text x=\"" + px + "\" y=\"" + (py - 10) + "\" text-anchor=\"middle\" style=\"font-size:10px;font-weight:700;fill:" + colorLine + ";\">" + formatNumber(series.stock[pi], 0) + "</text>";
            }
        }
        var stockLine = "<path d=\"" + lineD + "\" fill=\"none\" stroke=\"" + colorLine + "\" stroke-width=\"2.6\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/>";

        var maxLabelsShown = 12;
        var labelStep = Math.max(1, Math.ceil(N / maxLabelsShown));
        var xLabels = "";
        for (var li = 0; li < N; li++) {
            if (li !== 0 && li !== N - 1 && (li % labelStep) !== 0) continue;
            var lblText = String(series.labels[li] || "");
            var xPos = groupX(li);
            var rotateAttr = N > 30 ? " transform=\"rotate(-30," + xPos + "," + (axisY + 14) + ")\"" : "";
            xLabels += "<text x=\"" + xPos + "\" y=\"" + (axisY + 14) + "\" text-anchor=\"" + (N > 30 ? "end" : "middle") + "\"" + rotateAttr + " class=\"dk-flow-x-label\" style=\"font-size:10px;fill:#5f758b;font-weight:600;\">" + escapeHtml(lblText) + "</text>";
        }

        node.innerHTML =
            "<div class=\"dk-flow-legend\">" +
            "<span class=\"dk-flow-legend-item\"><span class=\"dk-flow-legend-dot\" style=\"background:#5bb8f5\"></span>Nhập</span>" +
            "<span class=\"dk-flow-legend-item\"><span class=\"dk-flow-legend-dot\" style=\"background:#a78bfa\"></span>Xuất</span>" +
            "<span class=\"dk-flow-legend-item\"><span class=\"dk-flow-legend-dot\" style=\"background:" + colorLine + "\"></span>Tồn Kho</span>" +
            "<span class=\"dk-text-muted\" style=\"margin-left:auto;font-size:11px\">" + N + " ngày" + (useBars ? "" : " (xem dạng đường)") + "</span>" +
            "</div>" +
            "<svg viewBox=\"0 0 " + W + " " + H + "\" style=\"width:100%;height:auto;display:block;overflow:visible;\">" +
            defs + axisTitles + grid + barsIn + barsOut + stockLine + dots + stockLabels + xLabels +
            "</svg>";
    }

    // ─── Biểu đồ đồng hồ đo lấp đầy NL/PL (Feature 4) ─────────────────────────
    function renderCapacityBarChart() {
        var node = byId("chartCapacityBar");
        if (!node) return;

        var overall = state.overall.length > 0 ? state.overall[0] : {};
        var usedNpl = Math.max(0, toNumber(overall.UsedNPL));
        var capNpl = Math.max(0, toNumber(overall.CapacityNPL));
        var usedPl = Math.max(0, toNumber(overall.UsedPL));
        var capPl = Math.max(0, toNumber(overall.CapacityPL));

        var pctNl = (capNpl > 0 && isFinite(capNpl)) ? Math.max(0, Math.min((usedNpl / capNpl) * 100, 100)) : 0;
        var pctPl = (capPl > 0 && isFinite(capPl)) ? Math.max(0, Math.min((usedPl / capPl) * 100, 100)) : 0;
        if (!isFinite(pctNl)) pctNl = 0;
        if (!isFinite(pctPl)) pctPl = 0;

        function gaugeColor(pct) {
            if (pct >= 100) return "#ef4444";
            if (pct >= 80) return "#f97316";
            if (pct >= 60) return "#eab308";
            return "#22c55e";
        }

        function semiArcPath(cx, cy, R, pct, stroke) {
            var bg = "M " + (cx - R) + " " + cy + " A " + R + " " + R + " 0 0 1 " + (cx + R) + " " + cy;
            // Guard: NaN, negative, or zero
            if (!isFinite(pct) || pct <= 0) return { bg: bg, fill: null };
            var ratio = Math.max(0, Math.min(pct / 100, 0.9999));   // tránh ratio = 1 → arc endpoint trùng start
            var theta = Math.PI * ratio;
            var ex = cx - R * Math.cos(theta);
            var ey = cy - R * Math.sin(theta);
            // Safety: nếu ex/ey không phải number → bỏ fill
            if (!isFinite(ex) || !isFinite(ey)) return { bg: bg, fill: null };
            var large = ratio > 0.5 ? 1 : 0;
            var fill = "M " + (cx - R) + " " + cy + " A " + R + " " + R + " 0 " + large + " 1 " + ex.toFixed(3) + " " + ey.toFixed(3);
            return { bg: bg, fill: fill };
        }

        function buildGaugeSvg(cx, cy, R, label, pct, used, total, gradId) {
            var visPct = pct;
            if (pct > 0 && pct < 22) visPct = 22;
            var arc = semiArcPath(cx, cy, R, visPct);
            var col = gaugeColor(pct);
            var col2 = pct >= 100 ? "#dc2626" : pct >= 80 ? "#ea580c" : pct >= 60 ? "#ca8a04" : "#16a34a";
            var strokeW = Math.round(R * 0.18);
            // v2.4.3 — theme-aware track + subtitle text + tag opacity
            var _darkG = dkIsDark();
            var trackColor = _darkG ? "#1e3a6b" : "#e8eef7";
            var subTxt = _darkG ? "#cbd5e1" : "#64748b";
            var tagOpac = _darkG ? "0.28" : "0.14";
            var html = "";
            // Gradient defs
            html += "<defs>" +
                "<linearGradient id=\"" + gradId + "\" x1=\"0\" y1=\"0\" x2=\"1\" y2=\"0\">" +
                "<stop offset=\"0%\"  stop-color=\"" + col + "\"/>" +
                "<stop offset=\"100%\" stop-color=\"" + col2 + "\"/>" +
                "</linearGradient>" +
                "<filter id=\"" + gradId + "_glow\" x=\"-30%\" y=\"-30%\" width=\"160%\" height=\"160%\">" +
                "<feGaussianBlur stdDeviation=\"4\" result=\"b\"/>" +
                "<feMerge><feMergeNode in=\"b\"/><feMergeNode in=\"SourceGraphic\"/></feMerge>" +
                "</filter>" +
                "</defs>";
            // background arc with subtle shadow
            html += "<path d=\"" + arc.bg + "\" fill=\"none\" stroke=\"" + trackColor + "\" stroke-width=\"" + strokeW + "\" stroke-linecap=\"round\"/>";
            // fill arc — gradient + glow, animated draw-in
            if (arc.fill) {
                // pathLength: 1000 normalize → CSS animate stroke-dashoffset 1000 → 0
                html += "<path class=\"dk-gauge-fill\" d=\"" + arc.fill + "\" fill=\"none\" " +
                    "stroke=\"url(#" + gradId + ")\" stroke-width=\"" + strokeW + "\" stroke-linecap=\"round\" " +
                    "pathLength=\"1000\" stroke-dasharray=\"1000\" filter=\"url(#" + gradId + "_glow)\"/>";
            }
            // 85% warning tick (dashed red)
            var warnAngle = Math.PI * 0.85;
            var nxIn = cx - (R - strokeW / 2 - 2) * Math.cos(warnAngle);
            var nyIn = cy - (R - strokeW / 2 - 2) * Math.sin(warnAngle);
            var nxOut = cx - (R + strokeW / 2 + 3) * Math.cos(warnAngle);
            var nyOut = cy - (R + strokeW / 2 + 3) * Math.sin(warnAngle);
            html += "<line x1=\"" + nxIn + "\" y1=\"" + nyIn + "\" x2=\"" + nxOut + "\" y2=\"" + nyOut + "\" stroke=\"#ef4444\" stroke-width=\"2.5\" stroke-dasharray=\"3 2\"/>";
            html += "<text x=\"" + cx + "\" y=\"" + (cy - 10) + "\" text-anchor=\"middle\" class=\"dk-gauge-pct\" style=\"font-size:32px;font-weight:900;fill:" + col + ";font-variant-numeric:tabular-nums;\">" + formatNumber(pct, 1) + "%</text>";
            html += "<text x=\"" + cx + "\" y=\"" + (cy + 14) + "\" text-anchor=\"middle\" style=\"font-size:12px;font-weight:700;fill:" + subTxt + ";\">" + formatNumber(used, 1) + " / " + formatNumber(total, 1) + " CBM</text>";
            var tagCol = label === "Kho NL" ? (_darkG ? "#60a5fa" : "#2563eb") : (_darkG ? "#fbbf24" : "#d97706");
            html += "<rect x=\"" + (cx - 26) + "\" y=\"" + (cy + 22) + "\" width=\"52\" height=\"22\" rx=\"11\" fill=\"" + tagCol + "\" opacity=\"" + tagOpac + "\"/>";
            html += "<text x=\"" + cx + "\" y=\"" + (cy + 37) + "\" text-anchor=\"middle\" style=\"font-size:13px;font-weight:900;fill:" + tagCol + ";\">" + escapeHtml(label) + "</text>";
            return html;
        }

        var W = 280, H = 210, cx = W / 2, cy = 155, R = 108;

        function makeSvg(label, pct, used, total, gradId) {
            var inner = buildGaugeSvg(cx, cy, R, label, pct, used, total, gradId);
            inner += "<text x=\"" + cx + "\" y=\"" + (H - 2) + "\" text-anchor=\"middle\" style=\"font-size:9px;fill:#ef4444;font-weight:700;\">── 85% cảnh báo</text>";
            return "<svg class=\"dk-gauge-svg\" viewBox=\"0 0 " + W + " " + H + "\">" + inner + "</svg>";
        }

        node.innerHTML =
            "<div class=\"dk-dual-gauge-wrap\">" +
            "<div class=\"dk-gauge-half\">" + makeSvg("Kho NL", pctNl, usedNpl, capNpl, "gradNL") + "</div>" +
            "<div class=\"dk-gauge-divider\"></div>" +
            "<div class=\"dk-gauge-half\">" + makeSvg("Kho PL", pctPl, usedPl, capPl, "gradPL") + "</div>" +
            "</div>";
    }

    // ════════════════════════════════════════════════════════════════
    // v2.4.6 — Render bảng Todo group-by-PO (header row + sub-rows)
    // ════════════════════════════════════════════════════════════════
    function renderTodoGroupedTable(cols, rows, isTraHang) {
        if (!rows || rows.length === 0) {
            return "<div class=\"dk-empty\" style=\"padding:20px\">Không có dữ liệu</div>";
        }
        // Group by POMua
        var groups = {};
        var poOrder = [];
        for (var i = 0; i < rows.length; i++) {
            var po = rows[i].POMua || "(Không có PO)";
            if (!groups[po]) { groups[po] = []; poOrder.push(po); }
            groups[po].push(rows[i]);
        }
        // Header
        var headHtml = "";
        for (var c = 0; c < cols.length; c++) {
            var col = cols[c];
            var styles = [];
            if (col.center) styles.push("text-align:center");
            else if (col.number !== undefined || col.percent) styles.push("text-align:right");
            if (col.width) styles.push("width:" + col.width + "px");
            headHtml += "<th" + (styles.length ? ' style="' + styles.join(";") + '"' : "") + ">" + escapeHtml(col.label) + "</th>";
        }
        // Body
        var bodyHtml = "";
        for (var pi = 0; pi < poOrder.length; pi++) {
            var po = poOrder[pi];
            var items = groups[po];
            var ncc = isTraHang && items[0] ? (items[0].NCC || "") : "";
            var nccText = ncc ? ' · <span class="dk-text-muted">NCC: ' + escapeHtml(ncc) + '</span>' : "";
            bodyHtml += '<tr class="dk-todo-po-row" data-po="' + escapeHtml(po) + '">' +
                '<td colspan="' + cols.length + '">' +
                '<span class="dk-todo-po-toggle" data-po-toggle="' + escapeHtml(po) + '">' +
                '<i class="fa-solid fa-chevron-down"></i>' +
                '</span>' +
                '<a href="#" class="dk-todo-po-link" data-po-link="' + escapeHtml(po) + '">' +
                '<i class="fa-solid fa-file-invoice"></i> PO: <b>' + escapeHtml(po) + '</b>' +
                '</a>' +
                ' — <span class="dk-text-muted">' + items.length + ' itemcode</span>' +
                nccText +
                '</td>' +
                '</tr>';
            for (var k = 0; k < items.length; k++) {
                var row = items[k];
                var rowHtml = "";
                for (var cc = 0; cc < cols.length; cc++) {
                    var colC = cols[cc];
                    var v = row[colC.key];
                    var cellStyle = "";
                    if (colC.raw) {
                        var alignR = colC.center ? "center" : "left";
                        rowHtml += '<td style="text-align:' + alignR + '">' + (v || "") + '</td>';
                        continue;
                    }
                    if (colC.number !== undefined) {
                        v = formatNumber(v, colC.number);
                        cellStyle = ' style="text-align:right;font-variant-numeric:tabular-nums"';
                    } else if (colC.center) {
                        cellStyle = ' style="text-align:center"';
                    } else if (colC.date) {
                        v = formatDate(v);
                        cellStyle = ' style="text-align:center"';
                    }
                    if (colC.key === "ItemCode") cellStyle = ' style="text-align:left;font-family:Consolas,monospace;font-weight:600"';
                    rowHtml += "<td" + cellStyle + ">" + escapeHtml(v) + "</td>";
                }
                bodyHtml += '<tr class="dk-todo-sub-row" data-po-sub="' + escapeHtml(po) + '">' + rowHtml + "</tr>";
            }
        }
        return '<table class="dk-detail-table dk-todo-table"><thead><tr>' + headHtml + '</tr></thead><tbody>' + bodyHtml + '</tbody></table>';
    }

    // ════════════════════════════════════════════════════════════════
    // v2.4.6 — Page 2 widget mới: Cảnh báo tồn kho + Hiệu suất hoạt động
    // ════════════════════════════════════════════════════════════════

    function renderAlertsList() {
        var node = byId("alertsList");
        if (!node) return;
        var rows = state.alerts || [];
        if (rows.length === 0) {
            node.innerHTML = "<div class=\"dk-empty\">Không có cảnh báo</div>";
            return;
        }
        var iconMap = {
            danger: "fa-triangle-exclamation",
            warn: "fa-circle-exclamation",
            info: "fa-circle-info"
        };
        node.innerHTML = rows.map(function (a) {
            var mucDo = (a.MucDo || "info").toLowerCase();
            var icon = iconMap[mucDo] || iconMap.info;
            // v2.4.16 — click 1 alert-item → mở modal alertDetail với code tương ứng
            return "" +
                "<div class=\"dk-alert-item dk-alert-" + mucDo + " js-open-detail\" data-detail=\"alertDetail\" data-alert-code=\"" + escapeHtml(a.MaCB || "") + "\" data-alert-name=\"" + escapeHtml(a.TenCB || "") + "\" title=\"Click để xem chi tiết\" role=\"button\" tabindex=\"0\">" +
                "<span class=\"dk-alert-icon\"><i class=\"fa-solid " + icon + "\"></i></span>" +
                "<div class=\"dk-alert-info\">" +
                "<div class=\"dk-alert-title\">" + escapeHtml(a.TenCB || "") + "</div>" +
                "<div class=\"dk-alert-desc\">" + escapeHtml(a.MoTa || "") + "</div>" +
                "</div>" +
                "<div class=\"dk-alert-badge\">" +
                "<span class=\"dk-alert-num\">" + formatNumber(toNumber(a.SoLuong), 0) + "</span> " +
                "<span class=\"dk-alert-unit\">" + escapeHtml(a.DonVi || "") + "</span>" +
                "</div>" +
                "</div>";
        }).join("");
    }

    function renderHieuSuatGauges() {
        if (typeof Highcharts === "undefined") return;
        var rows = state.hieuSuat || [];
        if (rows.length === 0) return;
        applyHighchartsTheme();
        var dark = dkIsDark();
        // v2.4.7 — Mapping accent + icon FA cho từng chỉ số
        var meta = {
            "hoan_thanh_nhap": { accent: "#3b82f6", accent2: "#60a5fa", icon: "fa-cloud-arrow-down", short: "Hoàn thành nhập" },
            "hoan_thanh_xuat": { accent: "#f97316", accent2: "#fb923c", icon: "fa-truck-fast", short: "Hoàn thành xuất" },
            "kiem_ke_dung_han": { accent: "#22c55e", accent2: "#4ade80", icon: "fa-clipboard-check", short: "Kiểm kê đúng hạn" },
            "don_hang_dung_han": { accent: "#06b6d4", accent2: "#22d3ee", icon: "fa-circle-check", short: "Đơn hàng đúng hạn" }
        };
        rows.forEach(function (r) {
            var cell = document.querySelector('.dk-perf-cell[data-perf="' + r.MaChiSo + '"]');
            if (!cell) return;
            var pct = toNumber(r.Value);
            var delta = toNumber(r.Delta);
            var m = meta[r.MaChiSo] || { accent: "#3b82f6", accent2: "#60a5fa", icon: "fa-circle-check", short: r.TenChiSo };
            cell.className = "dk-perf-cell dk-perf-accent-" + r.MaChiSo;
            cell.style.setProperty("--perf-accent", m.accent);
            cell.style.setProperty("--perf-accent2", m.accent2);
            cell.innerHTML = '' +
                '<div class="dk-perf-head">' +
                '<span class="dk-perf-head-icon"><i class="fa-solid ' + m.icon + '"></i></span>' +
                '<span class="dk-perf-head-label">' + escapeHtml(m.short) + '</span>' +
                '</div>' +
                '<div class="dk-perf-gauge"></div>' +
                '<div class="dk-perf-delta-row">' + formatDelta(delta) + '</div>';
            var gaugeEl = cell.querySelector(".dk-perf-gauge");
            var trackCol = dark ? "rgba(255,255,255,0.06)" : "rgba(0,0,0,0.05)";
            // Gradient stroke: light → accent đậm
            var gradId = "perfGrad_" + r.MaChiSo;
            Highcharts.chart(gaugeEl, {
                chart: { type: "solidgauge", backgroundColor: "transparent", spacing: [4, 4, 4, 4], margin: [0, 0, 0, 0] },
                title: { text: null },
                credits: { enabled: false },
                tooltip: { enabled: false },
                pane: {
                    center: ["50%", "55%"],
                    size: "100%",
                    startAngle: -135,
                    endAngle: 135,
                    background: [{
                        outerRadius: "100%",
                        innerRadius: "72%",
                        backgroundColor: trackCol,
                        borderWidth: 0,
                        shape: "arc"
                    }]
                },
                yAxis: { min: 0, max: 100, lineWidth: 0, tickPositions: [] },
                plotOptions: {
                    solidgauge: {
                        dataLabels: {
                            enabled: true,
                            y: -14,
                            borderWidth: 0,
                            useHTML: true,
                            format: '<div class="dk-perf-center"><div class="dk-perf-center-pct" style="color:' + m.accent + '">{y}%</div><div class="dk-perf-center-sub">Đạt</div></div>'
                        },
                        rounded: true,
                        linecap: "round"
                    }
                },
                series: [{
                    name: "pct",
                    data: [{
                        y: pct,
                        color: {
                            linearGradient: { x1: 0, x2: 1, y1: 0, y2: 1 },
                            stops: [[0, m.accent2], [1, m.accent]]
                        },
                        radius: "100%",
                        innerRadius: "72%"
                    }]
                }]
            });
        });
    }

    // ─── Theme Toggle (light/dark) ─────────────────────────────────────────────
    // v2.6.0 — Theme Toggle: sidebar removed, only topbar button
    function bindThemeToggle() {
        var btn = byId("btnThemeToggle");
        var btnTop = byId("btnThemeToggleTop");

        function doThemeToggle(e) {
            if (e) e.preventDefault();
            var mainContainer = document.getElementById("dkMain");
            var scrollY = mainContainer ? mainContainer.scrollTop : (window.scrollY || document.documentElement.scrollTop);

            var isDark = document.body.classList.toggle("dark-theme");
            try { localStorage.setItem("dkTheme", isDark ? "dark" : "light"); } catch (err) { }
            try { applyHighchartsTheme(); } catch (err) { }

            if (mainContainer) {
                mainContainer.scrollTop = scrollY;
            } else {
                window.scrollTo(0, scrollY);
            }

            setTimeout(function () {
                if (mainContainer) {
                    mainContainer.scrollTop = scrollY;
                } else {
                    window.scrollTo(0, scrollY);
                }
            }, 50);
        }

        if (btn) btn.addEventListener("click", doThemeToggle);
        if (btnTop) btnTop.addEventListener("click", doThemeToggle);
    }

    // ════════════════════════════════════════════════════════════════
    // v2.4.0 — Render 6 section mới ở Page 1 (Tổng quan)
    // ════════════════════════════════════════════════════════════════
    function renderTodoList() {
        var node = byId("todoList");
        if (!node) return;
        var items = state.todoList || [];
        if (items.length === 0) {
            node.innerHTML = "<div class=\"dk-empty\">Không có công việc</div>";
            return;
        }
        var iconMap = {
            "clipboard-check": "fa-clipboard-check",
            "clipboard-list": "fa-clipboard-list",
            "file-signature": "fa-file-signature",
            "truck": "fa-truck"
        };
        var colorMap = [
            { bg: "rgba(59,130,246,0.18)", color: "#60a5fa" },
            { bg: "rgba(168,85,247,0.18)", color: "#a78bfa" },
            { bg: "rgba(34,197,94,0.18)", color: "#34d399" },
            { bg: "rgba(245,158,11,0.18)", color: "#fbbf24" }
        ];
        node.innerHTML = items.map(function (it, idx) {
            var iconCls = iconMap[it.Icon] || "fa-circle-exclamation";
            var col = colorMap[idx % colorMap.length];
            // v2.4.7 — Click 1 todo-item → mở modal todoDetail với tab type tương ứng
            return '' +
                '<div class="dk-todo-item js-open-detail" data-detail="todoDetail" data-todo-type="' + escapeHtml(it.MaCV || "") + '" role="button" tabindex="0">' +
                '<span class="dk-todo-icon" style="background:' + col.bg + ';color:' + col.color + '">' +
                '<i class="fa-solid ' + iconCls + '"></i>' +
                '</span>' +
                '<div class="dk-todo-info">' +
                '<div class="dk-todo-title">' + escapeHtml(it.TenCV || "") + '</div>' +
                '<div class="dk-todo-desc">' + escapeHtml(it.MoTa || "") + '</div>' +
                '</div>' +
                '<div class="dk-todo-count">' +
                '<div class="dk-todo-num">' + formatNumber(toNumber(it.SoLuong), 0).padStart(2, "0") + '</div>' +
                '<div class="dk-todo-unit">' + escapeHtml(it.DonVi || "") + '</div>' +
                '</div>' +
                '</div>';
        }).join("");
    }

    function renderTop5VTTable() {
        var body = byId("top5VTBody");
        if (!body) return;
        var rows = state.top5VT || [];
        if (rows.length === 0) {
            body.innerHTML = "<tr><td colspan=\"6\" class=\"dk-empty\">Chưa có dữ liệu</td></tr>";
            return;
        }
        body.innerHTML = rows.map(function (r) {
            var loaiColor = r.LoaiKho === "NL" ? "#2563eb" : (r.LoaiKho === "PL" ? "#d97706" : "#6b7280");
            return "<tr>" +
                "<td class=\"text-center\">" + escapeHtml(r.STT) + "</td>" +
                "<td class=\"dk-cell-mono\"><a href=\"#\" class=\"dk-link-inline\">" + escapeHtml(r.MaVT || "") + "</a></td>" +
                "<td>" + escapeHtml(r.TenVT || "") + "</td>" +
                "<td class=\"text-center\"><span style=\"color:" + loaiColor + ";font-weight:700\">" + escapeHtml(r.LoaiKho || "") + "</span></td>" +
                "<td class=\"text-end dk-cell-num\">" + formatNumber(toNumber(r.CBM), 1) + "</td>" +
                "<td class=\"text-end dk-cell-num\">" + formatNumber(toNumber(r.TyTrong), 1) + "%</td>" +
                "</tr>";
        }).join("");
    }

    function renderTop5KHTable() {
        var body = byId("top5KHBody");
        if (!body) return;
        var rows = state.top5KH || [];
        if (rows.length === 0) {
            body.innerHTML = "<tr><td colspan=\"4\" class=\"dk-empty\">Chưa có dữ liệu</td></tr>";
            return;
        }
        body.innerHTML = rows.map(function (r) {
            return "<tr>" +
                "<td class=\"text-center\">" + escapeHtml(r.STT) + "</td>" +
                "<td>" + escapeHtml(r.KhachHang || "") + "</td>" +
                "<td class=\"text-end dk-cell-num\">" + formatNumber(toNumber(r.GiaTri), 0) + "</td>" +
                "<td class=\"text-end dk-cell-num\">" + formatNumber(toNumber(r.TyTrong), 1) + "%</td>" +
                "</tr>";
        }).join("");
    }

    function renderHetHanTable() {
        var body = byId("hetHanBody");
        if (!body) return;
        var rows = state.vtHetHan || [];
        if (rows.length === 0) {
            body.innerHTML = "<tr><td colspan=\"4\" class=\"dk-empty\">Không có vật tư sắp hết hạn</td></tr>";
            return;
        }
        var today = new Date(); today.setHours(0, 0, 0, 0);
        body.innerHTML = rows.map(function (r) {
            var dt = r.NgayHetHan ? new Date(r.NgayHetHan) : null;
            var daysLeft = dt ? Math.round((dt - today) / 86400000) : 999;
            var dateCls = daysLeft <= 7 ? "dk-date-urgent" : (daysLeft <= 14 ? "dk-date-warn" : "");
            var dateStr = dt ? formatDate(dt) : "—";
            var donVi = r.DonVi ? " " + escapeHtml(r.DonVi) : "";
            return "<tr>" +
                "<td class=\"dk-cell-mono\">" + escapeHtml(r.MaVT || "") + "</td>" +
                "<td>" + escapeHtml(r.TenVT || "") + "</td>" +
                "<td class=\"text-center " + dateCls + "\">" + dateStr + "</td>" +
                "<td class=\"text-end dk-cell-num\">" + formatNumber(toNumber(r.TonKho), 1) + donVi + "</td>" +
                "</tr>";
        }).join("");
    }

    function renderGiaTriTheoNhomChart() {
        var node = byId("chartGiaTriTheoNhom");
        if (!node) return;
        if (typeof Highcharts === "undefined") return;
        var rows = state.giaTriNhom || [];
        if (rows.length === 0) {
            node.innerHTML = "<div class=\"dk-empty\">Chưa có dữ liệu</div>";
            return;
        }
        applyHighchartsTheme();
        var dark = dkIsDark();
        var data = rows.filter(function (r) { return r.IsGroup === undefined || toNumber(r.IsGroup) === 1; }).map(function (r) {
            return { name: r.Nhom, y: toNumber(r.GiaTri), pct: toNumber(r.TyTrong) };
        });
        var tongGiaTri = data.reduce(function (s, d) { return s + d.y; }, 0);

        var formattedTotal;
        if (tongGiaTri >= 1e9) formattedTotal = formatNumber(tongGiaTri / 1e9, 2) + " tỷ";
        else if (tongGiaTri >= 1e6) formattedTotal = formatNumber(tongGiaTri / 1e6, 1) + " tr";
        else formattedTotal = formatNumber(tongGiaTri, 0);

        node.innerHTML = "";
        node.classList.add("dk-donut-shadow");
        // v2.4.16 — Append icon SAU chart (DOM order = stacking order); align qua events
        var centerEl = document.createElement("div");
        centerEl.className = "dk-nhom-center-icon";
        centerEl.innerHTML = '<i class="fa-solid fa-sack-dollar"></i>';
        function alignCenterIcon(chart) {
            try {
                var series = chart && chart.series && chart.series[0];
                if (!series || !series.center) return;
                var cx = series.center[0];
                var cy = series.center[1];
                var plotLeft = chart.plotLeft || 0;
                var plotTop = chart.plotTop || 0;
                centerEl.style.left = (plotLeft + cx) + "px";
                centerEl.style.top = (plotTop + cy) + "px";
                centerEl.style.transform = "translate(-50%, -50%)";
            } catch (e) { }
        }
        Highcharts.chart(node, {
            chart: {
                type: "pie",
                backgroundColor: "transparent",
                spacing: [10, 6, 22, 6],
                animation: { duration: 1000, easing: "easeOutCubic" },
                events: {
                    load: function () { alignCenterIcon(this); },
                    redraw: function () { alignCenterIcon(this); }
                }
            },
            title: { text: null },
            credits: { enabled: false },
            tooltip: {
                pointFormat: '<span style="color:{point.color}">●</span> <b>{point.name}</b><br/>Giá trị: <b>{point.y:,.0f} VND</b><br/>Tỷ trọng: <b>{point.pct:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    size: "55%",
                    innerSize: "60%",
                    borderWidth: 3,
                    borderColor: dark ? "#0d1d35" : "#ffffff",
                    borderRadius: 4,
                    showInLegend: false,
                    states: { hover: { brightness: 0.15, halo: { size: 8, opacity: 0.3 } } },
                    dataLabels: {
                        enabled: true,
                        distance: 18,
                        softConnector: false,
                        connectorWidth: 1.2,
                        allowOverlap: true,
                        format: "{point.name}<br/>{point.pct:.1f}%",
                        style: { color: dark ? "#e2eaf5" : "#1f2937", fontSize: "11px", fontWeight: "700", textOutline: "none" }
                    }
                }
            },
            legend: { enabled: false },
            series: [{
                name: "Giá trị tồn",
                colorByPoint: true,
                data: data,
                slicedOffset: 8
            }]
        });
        // v2.4.16 — Append icon SAU khi chart render xong → stacking order trên SVG
        node.appendChild(centerEl);
        var totalEl = document.createElement("div");
        totalEl.className = "dk-nhom-total";
        totalEl.innerHTML = "Tổng: <b>" + formattedTotal + "</b>";
        node.appendChild(totalEl);
    }

    function renderKiemKeBox() {
        var node = byId("kiemKeBox");
        if (!node) return;
        var d = state.kiemKe;
        if (!d) {
            node.innerHTML = "<div class=\"dk-empty\">Chưa có dữ liệu</div>";
            return;
        }
        var pct = toNumber(d.PctDaKiem);
        node.innerHTML = '' +
            '<div class="dk-kk-dashboard-layout">' +
            '<div class="dk-kk-chart-side">' +
            '<div class="dk-kk-circular-wrap">' +
            '<svg viewBox="0 0 36 36" class="dk-kk-circular-chart">' +
            '<defs>' +
            '<linearGradient id="kkGrad" x1="0%" y1="0%" x2="100%" y2="100%">' +
            '<stop offset="0%" stop-color="#10b981" />' +
            '<stop offset="100%" stop-color="#34d399" />' +
            '</linearGradient>' +
            '</defs>' +
            '<path class="dk-kk-circle-bg" fill="none" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />' +
            '<path class="dk-kk-circle" fill="none" stroke-dasharray="' + pct + ', 100" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />' +
            '</svg>' +
            '<div class="dk-kk-circular-val">' +
            '<span class="pct-num">' + formatNumber(pct, 1) + '%</span>' +
            '<span class="pct-lbl">Đã kiểm</span>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '<div class="dk-kk-info-side">' +
            '<div class="dk-kk-info-card dk-kk-info-good js-open-detail" data-detail="kiemKeAll" role="button" tabindex="0">' +
            '<span class="dk-kk-info-icon"><i class="fa-solid fa-circle-check"></i></span>' +
            '<div class="dk-kk-info-meta">' +
            '<span class="dk-kk-info-lbl">Đã kiểm</span>' +
            '<span class="dk-kk-info-val dk-count-up" data-count-to="' + toNumber(d.DaKiem) + '">0</span>' +
            '</div>' +
            '</div>' +
            '<div class="dk-kk-info-card dk-kk-info-bad js-open-detail" data-detail="kiemKeAll" role="button" tabindex="0">' +
            '<span class="dk-kk-info-icon"><i class="fa-solid fa-circle-exclamation"></i></span>' +
            '<div class="dk-kk-info-meta">' +
            '<span class="dk-kk-info-lbl">Chưa kiểm</span>' +
            '<span class="dk-kk-info-val dk-count-up" data-count-to="' + toNumber(d.ChuaKiem) + '">0</span>' +
            '</div>' +
            '</div>' +
            '<div class="dk-kk-info-total">' +
            '<span>Tổng số itemcode:</span>' +
            '<strong>' + formatNumber(toNumber(d.Tong), 0) + '</strong>' +
            '</div>' +
            '</div>' +
            '</div>';
        // Trigger counter-up
        animateCountUp(node);
    }

    // v2.4.10 — Counter-up helper (chạy 1 lần per element)
    function animateCountUp(scope) {
        var els = (scope || document).querySelectorAll(".dk-count-up:not([data-counted])");
        for (var i = 0; i < els.length; i++) {
            (function (el) {
                el.setAttribute("data-counted", "1");
                var to = toNumber(el.getAttribute("data-count-to"));
                var dur = 800;
                var start = performance.now();
                function tick(now) {
                    var t = Math.min(1, (now - start) / dur);
                    var ease = 1 - Math.pow(1 - t, 3); // easeOutCubic
                    el.textContent = formatNumber(to * ease, 0);
                    if (t < 1) requestAnimationFrame(tick);
                }
                requestAnimationFrame(tick);
            })(els[i]);
        }
    }

    // ════════════════════════════════════════════════════════════════
    // v2.4.4 — TOAST helper
    // ════════════════════════════════════════════════════════════════
    function showToast(msg, kind) {
        var cont = byId("dkToastContainer");
        if (!cont) return;
        var el = document.createElement("div");
        el.className = "dk-toast dk-toast-" + (kind || "info");
        el.innerHTML = '<i class="fa-solid ' +
            (kind === "success" ? "fa-circle-check" : (kind === "error" ? "fa-circle-exclamation" : "fa-circle-info")) +
            '"></i><span>' + escapeHtml(msg) + '</span>';
        cont.appendChild(el);
        setTimeout(function () { el.classList.add("show"); }, 10);
        setTimeout(function () {
            el.classList.remove("show");
            setTimeout(function () { if (el.parentNode) el.parentNode.removeChild(el); }, 250);
        }, 2500);
    }
    window.dkShowToast = showToast;
    function renderSummaryStrip(items) {
        var html = '<div class="dk-modal-summary-strip">';
        for (var i = 0; i < items.length; i++) {
            var it = items[i];
            var kindCls = it.kind ? (" dk-sum-" + it.kind) : "";
            html += '<div class="dk-sum-item' + kindCls + '">' +
                '<div class="dk-sum-label">' + escapeHtml(it.label) + '</div>' +
                '<div class="dk-sum-value">' + (it.value || "") + '</div>' +
                (it.sub ? '<div class="dk-sum-sub">' + escapeHtml(it.sub) + '</div>' : '') +
                '</div>';
        }
        return html + '</div>';
    }

    function modalLoading(text) {
        return '<div class="dk-empty" style="padding:40px;text-align:center"><i class="fa-solid fa-spinner fa-spin" style="font-size:24px;margin-bottom:10px;display:block"></i>' + escapeHtml(text || "Đang tải...") + '</div>';
    }

    function modalErrorBox(msg) {
        return '<div style="padding:30px">' +
            '<div class="dk-text-danger" style="margin-bottom:10px;font-size:14px">⚠ Lỗi tải dữ liệu</div>' +
            '<div class="dk-error-box">' + escapeHtml(msg || "Không rõ") + '</div>' +
            '</div>';
    }

    function renderTodoDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var BASE = "/api/DashboardKhoDesktop/";
        var tabs = [
            { key: "itemcode_cho_nk", label: "ItemCode chờ nhập kho", icon: "fa-clipboard-check", color: "blue" },
            { key: "kk_cho_duyet", label: "Kiểm kê chờ duyệt", icon: "fa-clipboard-list", color: "violet" }
        ];
        var tabHtml = '<div class="dk-todo-tab-strip">';
        for (var i = 0; i < tabs.length; i++) {
            tabHtml += '<button type="button" class="dk-todo-tab dk-todo-tab-' + tabs[i].color +
                (i === 0 ? ' active' : '') + '" data-todo-key="' + tabs[i].key + '">' +
                '<i class="fa-solid ' + tabs[i].icon + '"></i>' +
                '<span class="dk-todo-tab-label">' + escapeHtml(tabs[i].label) + '</span>' +
                '<span class="dk-todo-tab-badge" data-todo-badge="' + tabs[i].key + '">…</span>' +
                '</button>';
        }
        tabHtml += '</div>';
        tabHtml += '<div id="todoTabBody">' + modalLoading() + '</div>';
        content.innerHTML = tabHtml;

        var cache = {};
        function loadTab(key) {
            var body = byId("todoTabBody");
            if (!body) return;
            body.classList.remove("show");
            body.classList.add("fade-out");
            setTimeout(function () {
                if (cache[key]) { paint(key, cache[key]); return; }
                body.innerHTML = modalLoading();
                requestJson(BASE + "GetTodoDetail?type=" + encodeURIComponent(key)).then(function (data) {
                    var rows = normalizeArray(data);
                    cache[key] = rows;
                    paint(key, rows);
                    var badge = document.querySelector('[data-todo-badge="' + key + '"]');
                    if (badge) badge.textContent = String(rows.length).padStart(2, "0");
                }).catch(function (err) {
                    body.innerHTML = modalErrorBox(err && err.message);
                });
            }, 120);
        }
        function paint(key, rows) {
            var body = byId("todoTabBody");
            if (!body) return;
            if (rows.length === 0) {
                body.innerHTML = '<div class="dk-todo-empty"><i class="fa-solid fa-folder-open"></i><div>Không có item nào chờ xử lý</div></div>';
            } else {
                var searchBar = '<div class="dk-todo-searchbar">' +
                    '<i class="fa-solid fa-magnifying-glass"></i>' +
                    '<input type="text" id="todoSearch" placeholder="Tìm ItemCode, tên VT, PO mua, màu..." />' +
                    '<button type="button" id="todoSearchClear" class="dk-todo-search-clear">&times;</button>' +
                    '<span id="todoSearchCount" class="dk-todo-search-count">' + rows.length + ' kết quả</span>' +
                    '</div>';
                var commonHead = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 44 },
                    { key: "ItemCode", label: "ItemCode", width: 110 },
                    { key: "_TenVT", label: "Tên vật tư", raw: true, width: 180 },
                    { key: "MaMauVT", label: "Mã màu", center: true, width: 80 },
                    { key: "_MauVT", label: "Màu VT", raw: true, width: 130 }
                ];
                var commonTail = [
                    { key: "NgayTao", label: "Ngày tạo", date: true, center: true, width: 100 }
                ];
                var cols;
                if (key === "itemcode_cho_nk") {
                    cols = commonHead.concat([
                        { key: "WidthSize", label: "Width/Size", center: true, width: 100 },
                        { key: "_SLMua", label: "SL mua", raw: true, width: 90 },
                        { key: "_SLVe", label: "SL về", raw: true, width: 100 },
                        { key: "NCC", label: "NCC", width: 130 }
                    ], commonTail);
                } else if (key === "kk_cho_duyet") {
                    cols = commonHead.concat([
                        { key: "WidthSize", label: "Width/Size", center: true, width: 100 },
                        { key: "DonVi", label: "ĐV", center: true, width: 50 },
                        { key: "_TonKho", label: "Tồn kho", raw: true, width: 90 },
                        { key: "_SLKiemKe", label: "SL kiểm kê", raw: true, width: 100 },
                        { key: "_ChenhLech", label: "Chênh lệch (m)", raw: true, center: true, width: 110 }
                    ], commonTail);
                } else {
                    cols = commonHead.concat([
                        { key: "DonVi", label: "ĐV", center: true, width: 50 }
                    ], commonTail);
                }
                function fmtSL(val, unit, extraCls) {
                    if (val == null || val === "") return "";
                    var cls = "dk-cell-num" + (extraCls ? " " + extraCls : "");
                    return '<span class="' + cls + '">' + formatNumber(toNumber(val), 1) + (unit ? " " + escapeHtml(unit) : "") + '</span>';
                }
                var rowsView = rows.map(function (r) {
                    var tenMau = r.MauVT ? String(r.MauVT).trim() : "";
                    var mauVTHtml = tenMau
                        ? '<span class="dk-mau-swatch"></span><span class="dk-mau-text">' + escapeHtml(tenMau) + '</span>'
                        : "";
                    var tenVT = r.TenVT ? String(r.TenVT) : "";
                    var tenVTHtml = '<span class="dk-cell-tenvt" title="' + escapeHtml(tenVT) + '">' + escapeHtml(tenVT) + '</span>';

                    var slMuaHtml = fmtSL(r.SLMua);
                    var slVeHtml = "";
                    if (r.SLVe != null && r.SLVe !== "") {
                        var mua = toNumber(r.SLMua), ve = toNumber(r.SLVe);
                        if (mua > 0 && ve < mua) {
                            slVeHtml = fmtSL(r.SLVe, null, "dk-cell-warn");
                        } else if (mua > 0 && ve === mua) {
                            slVeHtml = '<i class="fa-solid fa-circle-check dk-cell-good" style="margin-right:4px"></i>' + fmtSL(r.SLVe, null, "dk-cell-good");
                        } else {
                            slVeHtml = fmtSL(r.SLVe);
                        }
                    }

                    var chenhHtml = "";
                    if (r.ChenhLech != null && r.ChenhLech !== "") {
                        var d = toNumber(r.ChenhLech);
                        var sign = d > 0 ? "+" : (d < 0 ? "−" : "");
                        var cls = d > 0 ? "dk-cell-good" : (d < 0 ? "dk-cell-danger" : "dk-cell-muted");
                        chenhHtml = '<span class="dk-cell-num ' + cls + '">' + sign + formatNumber(Math.abs(d), 1) + ' m</span>';
                    }

                    return Object.assign({}, r, {
                        _TenVT: tenVTHtml,
                        _MauVT: mauVTHtml,
                        _SLMua: slMuaHtml,
                        _SLVe: slVeHtml,
                        _TonKho: fmtSL(r.TonKho, r.DonVi),
                        _SLKiemKe: fmtSL(r.SLKiemKe, r.DonVi),
                        _ChenhLech: chenhHtml
                    });
                });
                // v2.4.6 — Group rows by POMua → render với group header row có thể collapse
                body.innerHTML = searchBar + '<div id="todoTableWrap">' + renderTodoGroupedTable(cols, rowsView, false) + '</div>';
                // v2.7.1 — Cập nhật tổng số dòng ở footer modal
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
                // Bind search
                var searchInput = byId("todoSearch");
                var searchCount = byId("todoSearchCount");
                if (searchInput) {
                    setTimeout(function () { searchInput.focus(); }, 150);
                    searchInput.addEventListener("input", function () {
                        var q = (this.value || "").trim().toLowerCase();
                        var trs = body.querySelectorAll("#todoTableWrap tbody tr");
                        var visible = 0;
                        for (var ti = 0; ti < trs.length; ti++) {
                            var match = q === "" || trs[ti].textContent.toLowerCase().indexOf(q) >= 0;
                            trs[ti].style.display = match ? "" : "none";
                            if (match) visible++;
                        }
                        searchCount.textContent = visible + " kết quả";
                    });
                }
                var clearBtn = byId("todoSearchClear");
                if (clearBtn) clearBtn.onclick = function () {
                    searchInput.value = "";
                    var ev = new Event("input"); searchInput.dispatchEvent(ev);
                    searchInput.focus();
                };


            }
            body.classList.remove("fade-out");
            setTimeout(function () { body.classList.add("show"); }, 10);
        }
        var tabBtns = content.querySelectorAll(".dk-todo-tab");
        for (var ti = 0; ti < tabBtns.length; ti++) {
            tabBtns[ti].addEventListener("click", function () {
                var allBtns = content.querySelectorAll(".dk-todo-tab");
                for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
                this.classList.add("active");
                loadTab(this.getAttribute("data-todo-key"));
            });
        }

        var initialKey = tabs[0].key;
        if (window.__dkPendingTodoType) {
            for (var pi = 0; pi < tabs.length; pi++) {
                if (tabs[pi].key === window.__dkPendingTodoType) { initialKey = tabs[pi].key; break; }
            }
            var allBtns = content.querySelectorAll(".dk-todo-tab");
            for (var ki = 0; ki < allBtns.length; ki++) {
                allBtns[ki].classList.toggle("active", allBtns[ki].getAttribute("data-todo-key") === initialKey);
            }
            window.__dkPendingTodoType = null;
        }
        loadTab(initialKey);
        var sb = document.querySelector(".dk-modal-search-bar");
        if (sb) sb.style.display = "none";
    }

    function renderTop5VTAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading("Đang tải danh sách vật tư theo dung tích...");
        requestJson("/api/DashboardKhoDesktop/GetVatTuTheoDungTich").then(function (data) {
            var rows = normalizeArray(data);
            var tongCBM = 0, slNL = 0, slPL = 0;
            for (var i = 0; i < rows.length; i++) {
                tongCBM += toNumber(rows[i].CBM);
                if (rows[i].LoaiKho === "NL") slNL++; else if (rows[i].LoaiKho === "PL") slPL++;
            }
            var avg = rows.length > 0 ? tongCBM / rows.length : 0;
            var sumHtml = renderSummaryStrip([
                { label: "Tổng CBM", value: formatNumber(tongCBM, 1), sub: rows.length + " mã VT" },
                { label: "Vật tư NL", value: formatNumber(slNL, 0), sub: "mã", kind: "primary" },
                { label: "Vật tư PL", value: formatNumber(slPL, 0), sub: "mã", kind: "warn" },
                { label: "Avg/mã", value: formatNumber(avg, 2), sub: "CBM" }
            ]);
            var cols = [
                { key: "STT", label: "#", number: 0, center: true, width: 40 },
                { key: "MaVT", label: "Mã VT", drillTo: "matCountDrill_codes", width: 130 },
                { key: "TenVT", label: "Tên vật tư" },
                { key: "LoaiKho", label: "Loại", center: true, width: 70 },
                { key: "CBM", label: "CBM", number: 2, sortable: true, width: 100 },
                { key: "TyTrong", label: "Tỷ trọng", percent: true, sortable: true, width: 90 },
                { key: "ViTriKe", label: "Vị trí kệ", center: true, width: 100 }
            ];
            content.innerHTML = sumHtml + '<div id="top5VTBodyTbl">' + renderSortableTable(cols, rows, { highlightTopN: 5 }) + '</div>';
            wireSortableTable(byId("top5VTBodyTbl"), cols, rows, { highlightTopN: 5 });
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            // Click cell MaVT → drill về popup ItemCode 
            content.querySelectorAll("[data-mavt-drill]").forEach(function (el) {
                el.addEventListener("click", function (e) {
                    e.preventDefault();
                    openDetail("matCountDrill_codes", -1);
                });
            });
        }).catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
    }

    // ════════════════════════════════════════════════════════════════
    //  — Modal: Top 5 KH (full 15 với pct bar + drill vị trí kệ)
    // ════════════════════════════════════════════════════════════════
    function renderTop5KHAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading("Đang tải danh sách khách hàng theo giá trị tồn...");
        requestJson("/api/DashboardKhoDesktop/GetKhachHangTonKhoChiTiet").then(function (data) {
            var rows = normalizeArray(data);
            var tongGT = 0, kt = 0;
            for (var i = 0; i < rows.length; i++) {
                tongGT += toNumber(rows[i].GiaTri);
                if (rows[i].KhachHang === "Khách trống") kt = toNumber(rows[i].GiaTri);
            }
            var avg = rows.length > 0 ? tongGT / rows.length : 0;
            var sumHtml = renderSummaryStrip([
                { label: "Tổng giá trị (VND)", value: formatNumber(tongGT, 0) },
                { label: "Số khách", value: formatNumber(rows.length, 0), sub: "khách hàng", kind: "primary" },
                { label: "Khách trống", value: formatNumber(kt, 0), sub: "VND", kind: "warn" },
                { label: "Avg/khách", value: formatNumber(avg, 0), sub: "VND" }
            ]);
            // Tìm max ty trong for pct bar
            var maxPct = 0;
            for (var j = 0; j < rows.length; j++) { if (toNumber(rows[j].TyTrong) > maxPct) maxPct = toNumber(rows[j].TyTrong); }
            var tableHtml = '<table class="dk-detail-table dk-top5kh-table"><thead><tr>' +
                '<th style="width:40px;text-align:center">#</th>' +
                '<th>Khách hàng</th>' +
                '<th style="width:100px">Mã KH</th>' +
                '<th style="width:80px;text-align:right">Số mã VT</th>' +
                '<th style="width:100px;text-align:right">Tổng CBM</th>' +
                '<th style="width:140px;text-align:right">Giá trị tồn (VND)</th>' +
                '<th style="width:180px">Tỷ trọng</th>' +
                '</tr></thead><tbody>';
            for (var k = 0; k < rows.length; k++) {
                var r = rows[k];
                var p = toNumber(r.TyTrong);
                var pBarW = maxPct > 0 ? (p / maxPct * 100) : 0;
                tableHtml += '<tr class="dk-row-kh js-open-detail clickable" data-detail="customerRow" data-makh="' + escapeHtml(r.MaKH || "") + '" data-tenkh="' + escapeHtml(r.KhachHang || "") + '">' +
                    '<td style="text-align:center">' + escapeHtml(r.STT) + '</td>' +
                    '<td>' + escapeHtml(r.KhachHang || "") + '</td>' +
                    '<td class="dk-cell-mono">' + escapeHtml(r.MaKH || "") + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.SoMaVT), 0) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.TongCBM), 4) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.GiaTri), 0) + '</td>' +
                    '<td><div class="dk-pct-bar-wrap"><div class="dk-pct-bar-fill" style="width:' + pBarW.toFixed(1) + '%"></div><span class="dk-pct-bar-label">' + formatNumber(p, 1) + '%</span></div></td>' +
                    '</tr>';
            }
            tableHtml += '</tbody></table>';
            content.innerHTML = sumHtml + tableHtml;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            content.querySelectorAll(".dk-row-kh").forEach(function (tr) {
                tr.addEventListener("click", function () {
                    var customerItem = { MaKH: this.getAttribute("data-makh"), TenKH: this.getAttribute("data-tenkh") };
                    _detailStack.push({ detail: _currentDetail, index: _currentDetailIndex });
                    _currentDetail = "_customerRow_drill";
                    _currentDetailIndex = -1;
                    renderDetailModalDirect({
                        title: "Vị trí kệ — " + (customerItem.TenKH || customerItem.MaKH),
                        rows: [], columns: [],
                        customDrillCustomer: customerItem
                    });
                });
            });
        }).catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
    }

    function loadCustomerMaterialDetail(customerItem) {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading("Đang tải chi tiết vật tư của khách hàng...");

        var maOrTen = customerItem.MaKH || customerItem.TenKH;
        requestJson("/api/DashboardKhoDesktop/GetVatTuTheoKhachHang?maKH=" + encodeURIComponent(maOrTen)).then(function (data) {
            var rows = normalizeArray(data);
            if (rows.length === 0) {
                content.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px\">Khách hàng \"" + (customerItem.TenKH || customerItem.MaKH) + "\" chưa có vật tư tồn kho.</div>";
                return;
            }

            var sumQty = 0, sumCBM = 0;
            for (var i = 0; i < rows.length; i++) {
                sumQty += toNumber(rows[i].SoLuong);
                sumCBM += toNumber(rows[i].TongCBM);
            }

            var sumHtml = renderSummaryStrip([
                { label: "Tổng số lượng", value: formatNumber(sumQty, 0) },
                { label: "Tổng CBM", value: formatNumber(sumCBM, 4), kind: "primary" },
                { label: "Số mặt hàng", value: formatNumber(rows.length, 0), kind: "success" }
            ]);

            var tableHtml = '<div class="dk-detail-table-wrap"><table class="dk-detail-table"><thead><tr>' +
                '<th style="width:40px;text-align:center">#</th>' +
                '<th style="width:140px">Mã vật tư</th>' +
                '<th style="width:250px">Tên vật tư</th>' +
                '<th style="width:80px;text-align:center">Màu</th>' +
                '<th style="width:80px;text-align:center">Khổ</th>' +
                '<th style="width:110px;text-align:right">Số lượng</th>' +
                '<th style="width:110px;text-align:right">Tổng CBM</th>' +
                '<th style="width:120px;text-align:center">Vị trí kệ</th>' +
                '</tr></thead><tbody>';

            for (var k = 0; k < rows.length; k++) {
                var r = rows[k];
                tableHtml += '<tr>' +
                    '<td style="text-align:center">' + (k + 1) + '</td>' +
                    '<td class="dk-cell-mono">' + escapeHtml(r.ItemCode || "") + '</td>' +
                    '<td style="text-align:left">' + escapeHtml(r.TenVT || "") + '</td>' +
                    '<td class="text-center">' + escapeHtml(r.Mau || "") + '</td>' +
                    '<td class="text-center">' + escapeHtml(r.KhoVai || "") + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.SoLuong), 0) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.TongCBM), 4) + '</td>' +
                    '<td class="text-center"><span class="dk-status-chip dk-status-chip-good">' + escapeHtml(r.ViTriKe || "") + '</span></td>' +
                    '</tr>';
            }
            tableHtml += '</tbody></table></div>';
            content.innerHTML = sumHtml + tableHtml;

            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
        }).catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
    }

    function renderDetailModalDirect(model) {
        var titleEl = byId(ids.detailModalTitle);
        if (_detailStack.length > 0) {
            titleEl.innerHTML = "<button type=\"button\" class=\"dk-back-btn js-modal-back\" title=\"Quay lại\">← Quay lại</button> " + escapeHtml(model.title);
        } else {
            titleEl.textContent = model.title;
        }
        byId(ids.detailModalMeta).textContent = model.meta || "";
        byId(ids.detailModalMeta).style.display = model.meta ? "" : "none";
        var sb = document.querySelector(".dk-modal-search-bar");
        if (sb) sb.style.display = "none";
        if (model.customDrillCustomer) {
            loadCustomerMaterialDetail(model.customDrillCustomer);
        }
    }

    // ════════════════════════════════════════════════════════════════
    // Modal: Vật tư sắp hết hạn
    // ════════════════════════════════════════════════════════════════
    function renderHetHanAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading();
        requestJson("/api/DashboardKhoDesktop/GetVatTuSapHetHanChiTiet").then(function (data) {
            var rows = normalizeArray(data);
            var today = new Date(); today.setHours(0, 0, 0, 0);
            rows.forEach(function (r) {
                r._ConLai = r.NgayHetHan ? Math.round((new Date(r.NgayHetHan) - today) / 86400000) : 0;
            });
            rows.sort(function (a, b) { return a._ConLai - b._ConLai; });
            var quaHan = 0, w7 = 0, w30 = 0;
            for (var i = 0; i < rows.length; i++) {
                var cl = rows[i]._ConLai;
                if (cl <= 0) quaHan++;
                else if (cl <= 7) w7++;
                else if (cl <= 30) w30++;
            }
            var sumHtml = renderSummaryStrip([
                { label: "Tổng vật tư", value: formatNumber(rows.length, 0) },
                { label: "Quá hạn", value: formatNumber(quaHan, 0), kind: "danger" },
                { label: "≤ 7 ngày", value: formatNumber(w7, 0), kind: "danger" },
                { label: "≤ 30 ngày", value: formatNumber(w30, 0), kind: "warn" }
            ]);
            var filterHtml = '<div class="dk-modal-filter-bar">' +
                '<label class="dk-text-muted">Lọc:</label>' +
                '<select id="hetHanFilter" class="dk-filter-select-inline">' +
                '<option value="all">Tất cả</option>' +
                '<option value="qh">Quá hạn</option>' +
                '<option value="7">≤ 7 ngày</option>' +
                '<option value="30">≤ 30 ngày</option>' +
                '<option value="90">≤ 90 ngày</option>' +
                '</select>' +
                '</div>';
            var tableHtml = '<table class="dk-detail-table"><thead><tr>' +
                '<th style="width:110px">Mã VT</th><th>Tên vật tư</th>' +
                '<th style="width:50px;text-align:center">Loại</th>' +
                '<th style="width:80px">Lô</th>' +
                '<th style="width:90px;text-align:center">Ngày SX</th>' +
                '<th style="width:100px;text-align:center">Hết hạn</th>' +
                '<th style="width:90px;text-align:center">Còn lại</th>' +
                '<th style="width:80px;text-align:right">Tồn kho</th>' +
                '<th style="width:50px;text-align:center">ĐV</th>' +
                '<th style="width:80px">Vị trí</th></tr></thead><tbody id="hetHanTbody">';
            tableHtml += hetHanRowsHtml(rows);
            tableHtml += '</tbody></table>';
            content.innerHTML = sumHtml + filterHtml + tableHtml;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            byId("hetHanFilter").addEventListener("change", function () {
                var v = this.value;
                var filtered = rows.filter(function (r) {
                    var cl = r._ConLai;
                    if (v === "qh") return cl <= 0;
                    if (v === "7") return cl > 0 && cl <= 7;
                    if (v === "30") return cl > 0 && cl <= 30;
                    if (v === "90") return cl > 0 && cl <= 90;
                    return true;
                });
                byId("hetHanTbody").innerHTML = hetHanRowsHtml(filtered);
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
            });
        }).catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
    }
    function hetHanRowsHtml(rows) {
        if (rows.length === 0) return '<tr><td colspan="10" class="dk-empty">Không có vật tư phù hợp bộ lọc</td></tr>';
        return rows.map(function (r) {
            var cl = r._ConLai;
            var cls = cl <= 0 ? "dk-conlai-overdue" : cl <= 7 ? "dk-conlai-urgent" : cl <= 30 ? "dk-conlai-warn" : "dk-conlai-soft";
            var icon = cl <= 0 ? '<i class="fa-solid fa-triangle-exclamation"></i> ' : "";
            return '<tr>' +
                '<td class="dk-cell-mono">' + escapeHtml(r.MaVT || "") + '</td>' +
                '<td>' + escapeHtml(r.TenVT || "") + '</td>' +
                '<td class="text-center">' + escapeHtml(r.LoaiKho || "") + '</td>' +
                '<td>' + escapeHtml(r.Lo || "") + '</td>' +
                '<td class="text-center">' + formatDate(r.NgaySX) + '</td>' +
                '<td class="text-center">' + formatDate(r.NgayHetHan) + '</td>' +
                '<td class="text-center ' + cls + '">' + icon + (cl <= 0 ? "Quá " + Math.abs(cl) : cl) + ' ngày</td>' +
                '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.TonKho), 1) + '</td>' +
                '<td class="text-center">' + escapeHtml(r.DonVi || "") + '</td>' +
                '<td>' + escapeHtml(r.ViTri || "") + '</td>' +
                '</tr>';
        }).join("");
    }

    // ════════════════════════════════════════════════════════════════
    // Modal: Giá trị nhóm 
    // ════════════════════════════════════════════════════════════════
    function renderGiaTriNhomAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading();
        requestJson("/api/DashboardKhoDesktop/GetGiaTriNhomChiTiet").then(function (data) {
            var rows = normalizeArray(data);
            var groups = rows.filter(function (r) { return toNumber(r.IsGroup) === 1; });
            var subs = rows.filter(function (r) { return toNumber(r.IsGroup) === 0; });
            var byParent = {};
            subs.forEach(function (s) {
                var p = s.ParentNhom;
                if (!byParent[p]) byParent[p] = [];
                byParent[p].push(s);
            });
            var html = '<div id="giaTriNhomDonut" class="dk-nhom-donut-modal" style="min-height:500px; height:500px; margin-bottom:30px; overflow:visible;"></div>';
            html += '<table class="dk-detail-table dk-tree-table"><thead><tr>' +
                '<th style="width:36px"></th>' +
                '<th style="width:40px">STT</th>' +
                '<th>Nhóm</th>' +
                '<th style="width:90px;text-align:right">Số mã VT</th>' +
                '<th style="width:100px;text-align:right">Tổng CBM</th>' +
                '<th style="width:140px;text-align:right">Giá trị</th>' +
                '<th style="width:100px;text-align:right">Tỷ trọng</th>' +
                '</tr></thead><tbody>';
            for (var i = 0; i < groups.length; i++) {
                var g = groups[i];
                var subList = byParent[g.Nhom] || [];
                html += '<tr class="dk-tree-row dk-tree-group" data-nhom="' + escapeHtml(g.Nhom) + '">' +
                    '<td class="text-center"><button type="button" class="dk-tree-toggle" data-nhom-toggle="' + escapeHtml(g.Nhom) + '"><i class="fa-solid fa-chevron-right"></i></button></td>' +
                    '<td class="text-center">' + g.STT + '</td>' +
                    '<td><b>' + escapeHtml(g.Nhom) + '</b></td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(g.SoMaVT), 0) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(g.TongCBM), 4) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(g.GiaTri), 0) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(g.TyTrong), 1) + '%</td>' +
                    '</tr>';
                if (subList.length > 0) {
                    html += '<tr class="dk-tree-sub-wrap" data-nhom-sub="' + escapeHtml(g.Nhom) + '" style="display:none"><td colspan="7">' +
                        '<table class="dk-tree-sub-table"><thead><tr>' +
                        '<th style="width:130px">Mã VT</th><th>Tên vật tư</th>' +
                        '<th style="width:90px;text-align:right">CBM</th>' +
                        '<th style="width:140px;text-align:right">Giá trị</th>' +
                        '</tr></thead><tbody>';
                    for (var j = 0; j < subList.length; j++) {
                        var s = subList[j];
                        html += '<tr><td class="dk-cell-mono">' + escapeHtml(s.MaVT || "") + '</td>' +
                            '<td>' + escapeHtml(s.Nhom || "") + '</td>' +
                            '<td class="text-end dk-cell-num">' + formatNumber(toNumber(s.TongCBM), 4) + '</td>' +
                            '<td class="text-end dk-cell-num">' + formatNumber(toNumber(s.GiaTri), 0) + '</td></tr>';
                    }
                    html += '</tbody></table></td></tr>';
                }
            }
            html += '</tbody></table>';
            content.innerHTML = html;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(groups.length, 0);
            // Render donut Highcharts
            if (typeof Highcharts !== "undefined" && groups.length > 0) {
                applyHighchartsTheme();
                Highcharts.chart("giaTriNhomDonut", {
                    chart: { type: "pie", backgroundColor: "transparent", height: 460 },
                    title: { text: null }, credits: { enabled: false },
                    tooltip: { useHTML: true, pointFormat: '<b>{point.name}</b><br/>Giá trị: <b>{point.y:,.0f} VND</b><br/>Tỷ trọng: <b>{point.pct:.1f}%</b>' },
                    plotOptions: { pie: { innerSize: "55%", borderWidth: 2, showInLegend: true, dataLabels: { enabled: true, distance: 15, allowOverlap: true, format: "{point.name}<br/>{point.pct:.1f}%", style: { fontSize: "10px", textOutline: "none" } } } },
                    legend: { enabled: true, layout: 'horizontal', align: 'center', verticalAlign: 'bottom', itemStyle: { fontSize: '11px', fontWeight: '500' } },
                    series: [{ name: "Giá trị", colorByPoint: true, data: groups.map(function (g) { return { name: g.Nhom, y: toNumber(g.GiaTri), pct: toNumber(g.TyTrong) }; }) }]
                });
            }
            content.querySelectorAll(".dk-tree-toggle").forEach(function (btn) {
                btn.addEventListener("click", function (e) {
                    e.stopPropagation();
                    var nhom = this.getAttribute("data-nhom-toggle");
                    var sub = content.querySelector('[data-nhom-sub="' + nhom + '"]');
                    if (!sub) return;
                    var icon = this.querySelector("i");
                    if (sub.style.display === "none") {
                        sub.style.display = "";
                        if (icon) icon.classList.replace("fa-chevron-right", "fa-chevron-down");
                    } else {
                        sub.style.display = "none";
                        if (icon) icon.classList.replace("fa-chevron-down", "fa-chevron-right");
                    }
                });
            });
        }).catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
    }

    // ════════════════════════════════════════════════════════════════
    //  Modal: Kiểm kê 
    // ════════════════════════════════════════════════════════════════
    function renderKiemKeAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading();
        requestJson("/api/DashboardKhoDesktop/GetKiemKeChiTiet").then(function (data) {
            var rows = normalizeArray(data);
            var totalCheckedItems = 0;
            var totalUncheckedItems = 0;
            var cSheetsCompleted = 0;
            var cSheetsUnchecked = 0;
            var kvs = {};
            for (var i = 0; i < rows.length; i++) {
                var r = rows[i];
                var s = toNumber(r.Status);
                var da = toNumber(r.DaKiem);
                var t = toNumber(r.Tong);

                totalCheckedItems += da;
                totalUncheckedItems += (t - da);
                kvs[r.KhuVuc] = 1;

                if (s === 1 || s === 2) {
                    cSheetsCompleted++;
                } else {
                    cSheetsUnchecked++;
                }
            }
            var sumHtml = renderKpiSummaryStrip([
                { label: "Đã kiểm", value: formatNumber(totalCheckedItems, 0), sub: "Trong " + cSheetsCompleted + " phiếu", cls: "good" },
                { label: "Chưa kiểm", value: formatNumber(totalUncheckedItems, 0), sub: "Trong " + cSheetsUnchecked + " phiếu", cls: "danger" }
            ], "dk-kiemke-summary");
            var tabs = [
                { key: "0", label: "Tất cả (" + rows.length + ")", icon: "fa-list" },
                { key: "1", label: "Đã kiểm (" + cSheetsCompleted + ")", icon: "fa-circle-check" },
                { key: "3", label: "Chưa kiểm (" + cSheetsUnchecked + ")", icon: "fa-circle-xmark" }
            ];
            var activeKey = "0";
            var tabHtml = renderKpiSubtabs(tabs, activeKey);
            var kvOpts = '<option value="">Tất cả khu vực</option>';
            Object.keys(kvs).sort().forEach(function (k) { kvOpts += '<option value="' + escapeHtml(k) + '">' + escapeHtml(k) + '</option>'; });
            var filterHtml = '<div class="dk-modal-filter-bar">' +
                '<i class="fa-solid fa-magnifying-glass"></i>' +
                '<input type="text" id="kkSearch" class="dk-kk-search" placeholder="Tìm mã phiếu, người phụ trách..." />' +
                '<select id="kkKhuVuc" class="dk-filter-select-inline">' + kvOpts + '</select>' +
                '<span id="kkCount" class="dk-text-muted" style="display: none;"></span>' +
                '</div>';
            var tableWrapHtml = '<div id="kpiTbody"></div>';
            content.innerHTML = sumHtml + tabHtml + filterHtml + tableWrapHtml;
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "MaPhieu", label: "Mã phiếu", center: true, width: 120 },
                { key: "NgayBatDau", label: "Ngày BĐ", date: true, center: true, width: 110 },
                { key: "KhuVuc", label: "Khu vực" },
                { key: "SoMa", label: "Số mã", number: 0, center: true, width: 80 },
                { key: "_DaTrenTong", label: "Đã/Tổng", raw: true, center: true, width: 120 },
                { key: "Pct", label: "%", percent: true, width: 80 },
                { key: "NguoiPT", label: "Người phụ trách" },
                { key: "_TrangThai", label: "Trạng thái", raw: true, center: true, width: 120 }
            ];
            function refilter() {
                var q = (byId("kkSearch").value || "").trim().toLowerCase();
                var kv = byId("kkKhuVuc").value;
                var filtered = rows.filter(function (r) {
                    var s = toNumber(r.Status);
                    if (activeKey === "1") {
                        if (s !== 1 && s !== 2) return false;
                    } else if (activeKey === "3") {
                        if (s !== 3) return false;
                    }
                    if (kv && r.KhuVuc !== kv) return false;
                    if (q) {
                        var hay = ((r.MaPhieu || "") + " " + (r.NguoiPT || "") + " " + (r.KhuVuc || "")).toLowerCase();
                        if (hay.indexOf(q) < 0) return false;
                    }
                    return true;
                });
                var mappedRows = filtered.map(function (r, idx) {
                    var s = toNumber(r.Status);
                    var chipCls = s === 1 ? "good" : (s === 2 ? "warn" : "danger");
                    return Object.assign({}, r, {
                        STT: idx + 1,
                        _DaTrenTong: formatNumber(toNumber(r.DaKiem), 0) + ' / ' + formatNumber(toNumber(r.Tong), 0),
                        _TrangThai: '<span class="dk-status-chip dk-status-chip-' + chipCls + '">' + escapeHtml(r.TrangThai || "") + '</span>'
                    });
                });
                byId("kpiTbody").innerHTML = renderDetailTable(cols, mappedRows);
                byId("kkCount").textContent = filtered.length + " phiếu";
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
            }
            content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
                b.addEventListener("click", function () {
                    content.querySelectorAll(".dk-kk-subtab").forEach(function (x) { x.classList.remove("active"); });
                    this.classList.add("active");
                    activeKey = this.getAttribute("data-tab-key");
                    refilter();
                });
            });
            byId("kkSearch").addEventListener("input", refilter);
            byId("kkKhuVuc").addEventListener("change", refilter);
            refilter();
        }).catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
    }

    // ════════════════════════════════════════════════════════════════
    // v2.4.15 — 5 modal chi tiết KPI header
    // ════════════════════════════════════════════════════════════════

    // Helper chung: render summary strip
    function renderKpiSummaryStrip(items, extraClass) {
        // v2.7.1 — Auto-fit column class to prevent wrapping
        var colClass = items.length >= 4 ? " dk-modal-summary-strip--4col" :
            (items.length === 3 ? " dk-modal-summary-strip--3col" : "");
        var cls = "dk-modal-summary-strip" + colClass + (extraClass ? " " + extraClass : "");
        var html = '<div class="' + cls + '">';
        for (var i = 0; i < items.length; i++) {
            var v = items[i];
            html += '<div class="dk-sum-item dk-sum-' + (v.cls || 'neutral') + '">' +
                '<div class="dk-sum-label">' + escapeHtml(v.label) + '</div>' +
                '<div class="dk-sum-value">' + v.value + '</div>' +
                '<div class="dk-sum-sub">' + escapeHtml(v.sub || '') + '</div>' +
                '</div>';
        }
        return html + '</div>';
    }

    // Helper chung: render sub-tab pills
    function renderKpiSubtabs(tabs, activeKey) {
        var html = '<div class="dk-kk-subtabs">';
        for (var i = 0; i < tabs.length; i++) {
            var t = tabs[i];
            var active = (t.key === activeKey) ? ' active' : '';
            html += '<button type="button" class="dk-kk-subtab' + active + '" data-tab-key="' + escapeHtml(t.key) + '"><i class="fa-solid ' + t.icon + '"></i> ' + escapeHtml(t.label) + '</button>';
        }
        return html + '</div>';
    }

    // Helper chung: render filter bar
    function renderKpiFilterBar(placeholder, opts) {
        opts = opts || {};
        var html = '<div class="dk-modal-filter-bar">' +
            '<i class="fa-solid fa-magnifying-glass"></i>' +
            '<input type="text" id="kpiSearch" class="dk-kk-search" placeholder="' + escapeHtml(placeholder) + '" />' +
            '<span id="kpiCount" class="dk-text-muted" style="display: none;"></span>' +
            '</div>';
        return html;
    }

    // Helper chung: bind search live
    function bindKpiSearch(scope, refilterFn) {
        var input = byId("kpiSearch");
        if (!input) return;
        setTimeout(function () { input.focus(); }, 120);
        input.addEventListener("input", function () { refilterFn(); });
    }

    // Helper chung: format VND tỷ
    function formatVNDShort(v) {
        var n = toNumber(v);
        if (n >= 1e9) return (n / 1e9).toFixed(2).replace(/\.?0+$/, "") + " tỷ";
        if (n >= 1e6) return (n / 1e6).toFixed(1).replace(/\.0$/, "") + " tr";
        return formatNumber(n, 0);
    }

    // ─── #0 Tồn đầu kỳ ────────────────────────────────────────────────
    function renderTonDauKyDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var titleEl = byId(ids.detailModalTitle);
        if (titleEl) titleEl.textContent = "Tồn đầu kỳ — tại " + (state.dateFilter ? state.dateFilter.from : "");
        var tabs = [
            { key: "all", label: "Tất cả", icon: "fa-list" },
            { key: "nl", label: "Nguyên liệu", icon: "fa-leaf" },
            { key: "pl", label: "Phụ liệu", icon: "fa-boxes-stacked" }
        ];
        var activeKey = "all";
        var allRows = [];
        function paint() {
            var sumSL = 0, sumGT = 0, soVT = new Set ? new Set() : {};
            allRows.forEach(function (r) {
                sumSL += toNumber(r.SLTonDau);
                sumGT += toNumber(r.ThanhTien);
                if (typeof soVT.add === "function") soVT.add(r.ItemCode);
                else soVT[r.ItemCode] = 1;
            });
            var numVT = (typeof soVT.size === "number") ? soVT.size : Object.keys(soVT).length;
            content.innerHTML =
                renderKpiSummaryStrip([
                    { label: "Tổng SL tồn đầu", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                    { label: "Số mã VT", value: formatNumber(numVT, 0), sub: "ItemCode", cls: "warn" },
                    { label: "Tổng giá trị", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" }
                ]) +
                renderKpiSubtabs(tabs, activeKey) +
                renderKpiFilterBar("Tìm ItemCode, tên VT...") +
                '<div id="kpiTbody">' + modalLoading() + '</div>';
            bindKpiSubtabs();
            bindKpiSearch(content, doFilter);
            doFilter();
        }
        function doFilter() {
            var q = (byId("kpiSearch").value || "").trim().toLowerCase();
            var filtered = allRows.filter(function (r) {
                if (!q) return true;
                var hay = ((r.MaNPL || "") + " " + (r.ItemCode || "") + " " + (r.TenVT || "") + " " + (r.Mau || "") + " " + (r.KhoVai || "")).toLowerCase();
                return hay.indexOf(q) >= 0;
            });
            renderTable(filtered);
            var cnt = byId("kpiCount"); if (cnt) cnt.textContent = filtered.length + " dòng";
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
        }
        function renderTable(rows) {
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                // v2.7.1 — Ẩn cột Mã NPL theo yêu cầu (chuỗi dài khó đọc)
                { key: "ItemCode", label: "ItemCode", width: 100 },
                { key: "TenVT", label: "Tên VT", width: 220 },
                { key: "Mau", label: "Màu", center: true, width: 80 },
                { key: "KhoVai", label: "Khổ", center: true, width: 80 },
                { key: "LoaiKho", label: "Loại", center: true, width: 60 },
                { key: "DonVi", label: "ĐV", center: true, width: 50 },
                { key: "SLTonDau", label: "SL tồn đầu", number: 1, width: 110 },
                { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
                { key: "ThanhTien", label: "Thành tiền", number: 0, width: 130 }
            ];
            byId("kpiTbody").innerHTML = renderDetailTable(cols, rows);
        }
        function bindKpiSubtabs() {
            content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
                b.addEventListener("click", function () {
                    content.querySelectorAll(".dk-kk-subtab").forEach(function (x) { x.classList.remove("active"); });
                    this.classList.add("active");
                    activeKey = this.getAttribute("data-tab-key");
                    fetchData();
                });
            });
        }
        function fetchData() {
            content.innerHTML = modalLoading();
            var from = state.dateFilter ? state.dateFilter.from : "";
            requestJson("/api/DashboardKhoDesktop/GetTonDauKyChiTiet?tuNgay=" + encodeURIComponent(from) + "&loai=" + encodeURIComponent(activeKey))
                .then(function (d) {
                    allRows = normalizeArray(d);
                    paint();
                })
                .catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
        }
        fetchData();
        var sb = document.querySelector(".dk-modal-search-bar"); if (sb) sb.style.display = "none";
    }

    // ─── #1 Tổng nhập ────────────────────────────────────────────────
    function renderTongNhapDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var titleEl = byId(ids.detailModalTitle);
        if (titleEl) titleEl.textContent = "Chi tiết nhập kho — " + (state.dateFilter ? state.dateFilter.from + " → " + state.dateFilter.to : "");
        var tabs = [
            { key: "all", label: "Chi tiết", icon: "fa-list" },
            { key: "date", label: "Theo ngày", icon: "fa-calendar-days" },
            { key: "po", label: "Theo PO mua", icon: "fa-file-invoice" },
            { key: "ncc", label: "Theo nhà cung cấp", icon: "fa-building" },
            { key: "vt", label: "Theo vật tư", icon: "fa-cube" }
        ];
        var activeKey = "all";
        var allRows = [];
        function paint() {
            var sumSL = 0, sumGT = 0;
            allRows.forEach(function (r) {
                sumSL += toNumber(r.SLNhap || r.SL);
                sumGT += toNumber(r.GiaTri);
            });
            content.innerHTML =
                renderKpiSummaryStrip([
                    { label: "Tổng SL nhập", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                    { label: "Số dòng", value: formatNumber(allRows.length, 0), sub: "trong " + activeKey, cls: "warn" },
                    { label: "Tổng giá trị", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" }
                ]) +
                renderKpiSubtabs(tabs, activeKey) +
                renderKpiFilterBar("Tìm nhanh trong bảng...") +
                '<div id="kpiTbody"></div>';
            bindKpiSubtabs();
            bindKpiSearch(content, doFilter);
            doFilter();
        }
        function doFilter() {
            var q = (byId("kpiSearch").value || "").trim().toLowerCase();
            var filtered = q ? allRows.filter(function (r) {
                return JSON.stringify(r).toLowerCase().indexOf(q) >= 0;
            }) : allRows;
            renderTable(filtered);
            var cnt = byId("kpiCount"); if (cnt) cnt.textContent = filtered.length + " dòng";
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
        }
        function renderTable(rows) {
            var cols;
            if (activeKey === "all") {
                cols = colsNhap();
            } else if (activeKey === "date") {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "Ngay", label: "Ngày", date: true, center: true, width: 100 },
                    { key: "SoPhieu", label: "Số phiếu", number: 0, center: true, width: 90 },
                    { key: "SoVT", label: "Số VT", number: 0, center: true, width: 80 },
                    { key: "SLNhap", label: "SL nhập", number: 1, width: 120 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 130 }
                ];
            } else if (activeKey === "po") {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "POMua", label: "PO mua", center: true, width: 130 },
                    { key: "NCC", label: "NCC", width: 180 },
                    { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
                    { key: "SL", label: "SL", number: 1, width: 110 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
                    { key: "NgayDuKien", label: "Dự kiến", date: true, center: true, width: 100 }
                ];
            } else if (activeKey === "ncc") {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "NCC", label: "NCC", width: 200 },
                    { key: "SoPO", label: "Số PO", number: 0, center: true, width: 80 },
                    { key: "SoVT", label: "Số VT", number: 0, center: true, width: 80 },
                    { key: "SL", label: "SL", number: 1, width: 120 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 140 }
                ];
            } else {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "ItemCode", label: "ItemCode", width: 110 },
                    { key: "TenVT", label: "Tên VT", width: 240 },
                    { key: "LoaiKho", label: "Loại", center: true, width: 60 },
                    { key: "DonVi", label: "ĐV", center: true, width: 50 },
                    { key: "SLNhap", label: "SL nhập", number: 1, width: 110 },
                    { key: "SoPO", label: "Số PO", number: 0, center: true, width: 70 },
                    { key: "LanNhapCuoi", label: "Lần cuối", date: true, center: true, width: 100 },
                    { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 130 }
                ];
            }
            byId("kpiTbody").innerHTML = renderDetailTable(cols, rows);
        }
        function bindKpiSubtabs() {
            content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
                b.addEventListener("click", function () {
                    content.querySelectorAll(".dk-kk-subtab").forEach(function (x) { x.classList.remove("active"); });
                    this.classList.add("active");
                    activeKey = this.getAttribute("data-tab-key");
                    fetchData();
                });
            });
        }
        function fetchData() {
            content.innerHTML = modalLoading();
            var qs = "?tuNgay=" + encodeURIComponent(state.dateFilter.from) + "&denNgay=" + encodeURIComponent(state.dateFilter.to) + "&groupBy=" + encodeURIComponent(activeKey);
            requestJson("/api/DashboardKhoDesktop/GetTongNhapChiTiet" + qs)
                .then(function (d) { allRows = normalizeArray(d); paint(); })
                .catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
        }
        fetchData();
        var sb = document.querySelector(".dk-modal-search-bar"); if (sb) sb.style.display = "none";
    }

    // ─── #2 Tổng xuất (similar to Tổng nhập) ──────────────────────────
    function renderTongXuatDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var titleEl = byId(ids.detailModalTitle);
        if (titleEl) titleEl.textContent = "Chi tiết xuất kho — " + (state.dateFilter ? state.dateFilter.from + " → " + state.dateFilter.to : "");
        var tabs = [
            { key: "all", label: "Chi tiết", icon: "fa-list" },
            { key: "date", label: "Theo ngày", icon: "fa-calendar-days" },
            { key: "dh", label: "Theo đơn hàng", icon: "fa-file-lines" },
            { key: "kh", label: "Theo khách hàng", icon: "fa-users" },
            { key: "vt", label: "Theo vật tư", icon: "fa-cube" }
        ];
        var activeKey = "all";
        var allRows = [];
        function paint() {
            var sumSL = 0, sumGT = 0;
            allRows.forEach(function (r) { sumSL += toNumber(r.SLXuat || r.SL); sumGT += toNumber(r.GiaTri); });
            content.innerHTML =
                renderKpiSummaryStrip([
                    { label: "Tổng SL xuất", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                    { label: "Số dòng", value: formatNumber(allRows.length, 0), sub: "trong " + activeKey, cls: "warn" },
                    { label: "Tổng giá trị", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" }
                ]) +
                renderKpiSubtabs(tabs, activeKey) +
                renderKpiFilterBar("Tìm nhanh trong bảng...") +
                '<div id="kpiTbody"></div>';
            bindKpiSubtabs();
            bindKpiSearch(content, doFilter);
            doFilter();
        }
        function doFilter() {
            var q = (byId("kpiSearch").value || "").trim().toLowerCase();
            var filtered = q ? allRows.filter(function (r) { return JSON.stringify(r).toLowerCase().indexOf(q) >= 0; }) : allRows;
            renderTable(filtered);
            var cnt = byId("kpiCount"); if (cnt) cnt.textContent = filtered.length + " dòng";
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
        }
        function renderTable(rows) {
            var cols;
            if (activeKey === "all") {
                cols = colsXuat();
            } else if (activeKey === "date") {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "Ngay", label: "Ngày", date: true, center: true, width: 100 },
                    { key: "SoPhieu", label: "Số phiếu", number: 0, center: true, width: 90 },
                    { key: "SoDH", label: "Số ĐH", number: 0, center: true, width: 80 },
                    { key: "SLXuat", label: "SL xuất", number: 1, width: 120 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 130 }
                ];
            } else if (activeKey === "dh") {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "MaDH", label: "Mã ĐH", center: true, width: 130 },
                    { key: "KhachHang", label: "Khách hàng", width: 180 },
                    { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
                    { key: "SL", label: "SL", number: 1, width: 110 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
                    { key: "NgayXuat", label: "Ngày", date: true, center: true, width: 100 }
                ];
            } else if (activeKey === "kh") {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "KhachHang", label: "Khách hàng", width: 220 },
                    { key: "MaKH", label: "Mã KH", center: true, width: 90 },
                    { key: "SoDH", label: "Số ĐH", number: 0, center: true, width: 70 },
                    { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
                    { key: "SL", label: "SL", number: 1, width: 110 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 130 }
                ];
            } else {
                cols = [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "ItemCode", label: "ItemCode", width: 110 },
                    { key: "TenVT", label: "Tên VT", width: 240 },
                    { key: "LoaiKho", label: "Loại", center: true, width: 60 },
                    { key: "DonVi", label: "ĐV", center: true, width: 50 },
                    { key: "SLXuat", label: "SL xuất", number: 1, width: 110 },
                    { key: "SoDH", label: "Số ĐH", number: 0, center: true, width: 70 },
                    { key: "LanXuatCuoi", label: "Lần cuối", date: true, center: true, width: 100 },
                    { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
                    { key: "GiaTri", label: "Giá trị", number: 0, width: 130 }
                ];
            }
            byId("kpiTbody").innerHTML = renderDetailTable(cols, rows);
        }
        function bindKpiSubtabs() {
            content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
                b.addEventListener("click", function () {
                    content.querySelectorAll(".dk-kk-subtab").forEach(function (x) { x.classList.remove("active"); });
                    this.classList.add("active");
                    activeKey = this.getAttribute("data-tab-key");
                    fetchData();
                });
            });
        }
        function fetchData() {
            content.innerHTML = modalLoading();
            var qs = "?tuNgay=" + encodeURIComponent(state.dateFilter.from) + "&denNgay=" + encodeURIComponent(state.dateFilter.to) + "&groupBy=" + encodeURIComponent(activeKey);
            requestJson("/api/DashboardKhoDesktop/GetTongXuatChiTiet" + qs)
                .then(function (d) { allRows = normalizeArray(d); paint(); })
                .catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
        }
        fetchData();
        var sb = document.querySelector(".dk-modal-search-bar"); if (sb) sb.style.display = "none";
    }

    // ─── #3 Tồn kho ──────────────────────────────────────────────────
    function renderTonKhoDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var titleEl = byId(ids.detailModalTitle);
        if (titleEl) titleEl.textContent = "Chi tiết tồn kho — tại " + (state.dateFilter ? state.dateFilter.to : "");
        var tabs = [
            { key: "all", label: "Tất cả", icon: "fa-list" },
            { key: "nl", label: "Nguyên liệu", icon: "fa-leaf" },
            { key: "pl", label: "Phụ liệu", icon: "fa-boxes-stacked" },
            { key: "expired", label: "Sắp hết hạn", icon: "fa-clock" }
        ];
        var activeKey = "all";
        var allRows = [];
        function paint() {
            var sumSL = 0, sumGT = 0, soVT = {}, soKe = {};
            allRows.forEach(function (r) {
                sumSL += toNumber(r.SLTon);
                sumGT += toNumber(r.SLTon) * toNumber(r.DonGia);
                soVT[r.ItemCode] = 1; soKe[r.ViTriKe] = 1;
            });
            content.innerHTML =
                renderKpiSummaryStrip([
                    { label: "Tổng SL tồn", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                    { label: "Số mã VT", value: formatNumber(Object.keys(soVT).length, 0), sub: "ItemCode", cls: "warn" },
                    { label: "Số kệ", value: formatNumber(Object.keys(soKe).length, 0), sub: "vị trí", cls: "neutral" },
                    { label: "Giá trị tồn", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" }
                ]) +
                renderKpiSubtabs(tabs, activeKey) +
                renderKpiFilterBar("Tìm ItemCode, tên VT, vị trí kệ...") +
                '<div id="kpiTbody"></div>';
            bindKpiSubtabs();
            bindKpiSearch(content, doFilter);
            doFilter();
        }
        function doFilter() {
            var q = (byId("kpiSearch").value || "").trim().toLowerCase();
            var filtered = q ? allRows.filter(function (r) {
                var hay = ((r.ItemCode || "") + " " + (r.TenVT || "") + " " + (r.ViTriKe || "") + " " + (r.POMua || "") + " " + (r.SoLo || "") + " " + (r.TenKH || "")).toLowerCase();
                return hay.indexOf(q) >= 0;
            }) : allRows;
            renderTable(filtered);
            var cnt = byId("kpiCount"); if (cnt) cnt.textContent = filtered.length + " dòng";
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
        }
        function renderTable(rows) {
            var today = new Date(); today.setHours(0, 0, 0, 0);
            var rowsView = rows.map(function (r) {
                var hd = r.HanDung ? new Date(r.HanDung) : null;
                var dleft = hd ? Math.round((hd - today) / 86400000) : 999;
                var chipCls = dleft < 7 ? "danger" : (dleft < 30 ? "warn" : "good");
                var chipText = dleft < 0 ? "Quá hạn " + Math.abs(dleft) + "d" : (dleft + " ngày");
                return Object.assign({}, r, {
                    _HanDung: '<span class="dk-status-chip dk-status-chip-' + chipCls + '">' + chipText + '</span>'
                });
            });
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "ItemCode", label: "ItemCode", width: 110 },
                { key: "TenVT", label: "Tên VT", width: 220 },
                { key: "LoaiKho", label: "Loại", center: true, width: 60 },
                { key: "DonVi", label: "ĐV", center: true, width: 50 },
                { key: "SLTon", label: "SL tồn", number: 1, width: 110 },
                { key: "ViTriKe", label: "Vị trí", center: true, width: 90 },
                { key: "_HanDung", label: "Hạn dùng", raw: true, center: true, width: 110 },
                { key: "POMua", label: "Số PO gần nhất", center: true, width: 120 },
                { key: "SoLo", label: "Số lô gần nhất", center: true, width: 120 },
                { key: "TenKH", label: "Khách hàng", width: 160 },
                { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
                { key: "ThanhTien", label: "Thành tiền", number: 0, width: 130 }
            ];
            byId("kpiTbody").innerHTML = renderDetailTable(cols, rowsView);
        }
        function bindKpiSubtabs() {
            content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
                b.addEventListener("click", function () {
                    content.querySelectorAll(".dk-kk-subtab").forEach(function (x) { x.classList.remove("active"); });
                    this.classList.add("active");
                    activeKey = this.getAttribute("data-tab-key");
                    fetchData();
                });
            });
        }
        function fetchData() {
            content.innerHTML = modalLoading();
            requestJson("/api/DashboardKhoDesktop/GetTonKhoChiTiet?denNgay=" + encodeURIComponent(state.dateFilter.to) + "&loai=" + encodeURIComponent(activeKey))
                .then(function (d) { allRows = normalizeArray(d); paint(); })
                .catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
        }
        fetchData();
        var sb = document.querySelector(".dk-modal-search-bar"); if (sb) sb.style.display = "none";
    }

    // ─── #5 PO đang trễ ──────────────────────────────────────────────
    function renderPOTreDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var titleEl = byId(ids.detailModalTitle);
        if (titleEl) titleEl.textContent = "PO đã về kho — đang chờ kiểm";
        var tabs = [
            { key: "all", label: "Tất cả", icon: "fa-list" },
            { key: "chua_qc", label: "Chưa QC", icon: "fa-clipboard-question" },
            { key: "dang_qc", label: "Đang QC", icon: "fa-spinner" },
            { key: "da_qc_chua_nk", label: "Đã QC chưa nhập", icon: "fa-clipboard-check" }
        ];
        var activeKey = "all";
        var allRows = [];
        function paint() {
            var soNCC = {}, treSmall = 0, treBig = 0;
            allRows.forEach(function (r) {
                soNCC[r.NCC] = 1;
                var h = toNumber(r.SoGioTre);
                if (h > 168) treBig++;
                else if (h <= 72) treSmall++;
            });
            content.innerHTML =
                renderKpiSummaryStrip([
                    { label: "Tổng PO trễ", value: formatNumber(allRows.length, 0), sub: "PO", cls: "danger" },
                    { label: "Số NCC", value: formatNumber(Object.keys(soNCC).length, 0), sub: "nhà cung cấp", cls: "warn" },
                    { label: "Trễ ≤ 3 ngày", value: formatNumber(treSmall, 0), sub: "PO", cls: "good" },
                    { label: "Trễ > 7 ngày", value: formatNumber(treBig, 0), sub: "PO", cls: "danger" }
                ]) +
                renderKpiSubtabs(tabs, activeKey) +
                renderKpiFilterBar("Tìm số PO, NCC...") +
                '<div id="kpiTbody"></div>';
            bindKpiSubtabs();
            bindKpiSearch(content, doFilter);
            doFilter();
        }
        function doFilter() {
            var q = (byId("kpiSearch").value || "").trim().toLowerCase();
            var filtered = q ? allRows.filter(function (r) {
                var hay = ((r.POMua || "") + " " + (r.NCC || "")).toLowerCase();
                return hay.indexOf(q) >= 0;
            }) : allRows;
            renderTable(filtered);
            var cnt = byId("kpiCount"); if (cnt) cnt.textContent = filtered.length + " dòng";
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
        }
        function renderTable(rows) {
            var rowsView = rows.map(function (r) {
                var h = toNumber(r.SoGioTre);
                var cellCls = h > 168 ? "dk-cell-late-extreme" : (h >= 48 ? "dk-cell-late-warn" : "");
                var ttCls = r.MaTT === "chua_qc" ? "danger" : (r.MaTT === "dang_qc" ? "warn" : "good");
                return Object.assign({}, r, {
                    _SoGio: '<span class="dk-cell-num ' + cellCls + '">' + formatNumber(h, 0) + ' h</span>',
                    _TrangThai: '<span class="dk-status-chip dk-status-chip-' + ttCls + '">' + escapeHtml(r.TrangThai || "") + '</span>'
                });
            });
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "POMua", label: "Số PO", center: true, width: 130 },
                { key: "NCC", label: "NCC", width: 140 },
                { key: "NgayDuKien", label: "Dự kiến", date: true, center: true, width: 100 },
                { key: "NgayVeThucTe", label: "Về thực tế", date: true, center: true, width: 100 },
                { key: "_SoGio", label: "Giờ trễ", raw: true, width: 90 },
                { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
                { key: "SL", label: "SL", number: 1, width: 100 },
                { key: "_TrangThai", label: "Trạng thái", raw: true, center: true, width: 140 }
            ];
            byId("kpiTbody").innerHTML = renderDetailTable(cols, rowsView);
        }
        function bindKpiSubtabs() {
            content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
                b.addEventListener("click", function () {
                    content.querySelectorAll(".dk-kk-subtab").forEach(function (x) { x.classList.remove("active"); });
                    this.classList.add("active");
                    activeKey = this.getAttribute("data-tab-key");
                    fetchData();
                });
            });
        }
        function fetchData() {
            content.innerHTML = modalLoading();
            requestJson("/api/DashboardKhoDesktop/GetPODangTreChiTiet?groupBy=" + encodeURIComponent(activeKey))
                .then(function (d) { allRows = normalizeArray(d); paint(); })
                .catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
        }
        fetchData();
        var sb = document.querySelector(".dk-modal-search-bar"); if (sb) sb.style.display = "none";
    }

    // ════════════════════════════════════════════════════════════════
    // v2.4.16 — Alert detail modal (dispatcher theo MaCB)
    // ════════════════════════════════════════════════════════════════
    function renderAlertDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var code = window.__dkPendingAlertCode || "po_tre";
        var name = window.__dkPendingAlertName || "Cảnh báo";
        window.__dkPendingAlertCode = null;
        window.__dkPendingAlertName = null;

        var titleEl = byId(ids.detailModalTitle);
        if (titleEl) titleEl.textContent = "Chi tiết cảnh báo — " + name;

        // Map MaCB → endpoint + columns + cls
        var alertMap = {
            "po_tre": {
                api: "/api/DashboardKhoDesktop/GetPODangTreChiTiet?groupBy=all",
                cls: "danger",
                cols: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "POMua", label: "Số PO", center: true, width: 130 },
                    { key: "NCC", label: "NCC" },
                    { key: "MaHang", label: "Mã hàng", center: true, width: 120 },
                    { key: "SoVT", label: "Số VT", number: 0, center: true, width: 80 },
                    { key: "SL", label: "Tổng SL", number: 2, width: 100 },
                    { key: "NgayNKDuKien", label: "Dự kiến", date: true, center: true, width: 100 },
                    { key: "SoGioTre", label: "Trễ (giờ)", number: 0, width: 90 },
                    { key: "TrangThai", label: "Trạng thái", center: true, width: 110 }
                ]
            },
            "npl_thieu": {
                api: "/api/DashboardKhoDesktop/GetNPLThieuChiTiet",
                cls: "danger",
                cols: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "ItemCode", label: "ItemCode", width: 120 },
                    { key: "TenVT", label: "Tên VT" },
                    { key: "LenhSX", label: "Lệnh SX", center: true, width: 130 },
                    { key: "DonVi", label: "ĐV", center: true, width: 50 },
                    { key: "SLCan", label: "SL cần", number: 1, width: 100 },
                    { key: "SLCo", label: "SL có", number: 1, width: 100 },
                    { key: "SLThieu", label: "Thiếu", number: 1, width: 100 }
                ]
            },
            "kk_lech": {
                api: "/api/DashboardKhoDesktop/GetKiemKeLechChiTiet",
                cls: "warn",
                cols: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "MaPhieu", label: "Mã phiếu", center: true, width: 110 },
                    { key: "KhuVuc", label: "Khu vực" },
                    { key: "SLHeThong", label: "SL hệ thống", number: 1, width: 120 },
                    { key: "SLThucKiem", label: "SL thực kiểm", number: 1, width: 120 },
                    { key: "LechPct", label: "Lệch (%)", number: 2, width: 100 },
                    { key: "NguoiPT", label: "Người phụ trách" }
                ]
            },
            "ton_vuot_dm": {
                api: "/api/DashboardKhoDesktop/GetTonVuotDinhMucChiTiet",
                cls: "danger",
                cols: [
                    { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                    { key: "ItemCode", label: "ItemCode", width: 120 },
                    { key: "TenVT", label: "Tên VT" },
                    { key: "DonVi", label: "ĐV", center: true, width: 50 },
                    { key: "SLTon", label: "SL tồn", number: 1, width: 100 },
                    { key: "DinhMuc", label: "Định mức", number: 1, width: 100 },
                    { key: "VuotPct", label: "Vượt (%)", number: 2, width: 100 },
                    { key: "ViTriKe", label: "Vị trí", center: true, width: 90 }
                ]
            }
        };
        var cfg = alertMap[code] || alertMap.po_tre;
        var allRows = [];
        function paint() {
            content.innerHTML =
                renderKpiSummaryStrip([
                    { label: "Tổng cảnh báo", value: formatNumber(allRows.length, 0), sub: "dòng", cls: cfg.cls },
                    { label: "Mức độ", value: cfg.cls === "danger" ? "Nghiêm trọng" : (cfg.cls === "warn" ? "Cảnh báo" : "Thông tin"), sub: "", cls: cfg.cls },
                    { label: "Cập nhật", value: new Date().toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" }), sub: "hôm nay", cls: "neutral" }
                ]) +
                renderKpiFilterBar("Tìm nhanh trong bảng...") +
                '<div id="kpiTbody"></div>';
            bindKpiSearch(content, doFilter);
            doFilter();
        }
        function doFilter() {
            var q = (byId("kpiSearch").value || "").trim().toLowerCase();
            var filtered = q ? allRows.filter(function (r) { return JSON.stringify(r).toLowerCase().indexOf(q) >= 0; }) : allRows;
            byId("kpiTbody").innerHTML = renderDetailTable(cfg.cols, filtered);
            var cnt = byId("kpiCount"); if (cnt) cnt.textContent = filtered.length + " dòng";
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
        }
        content.innerHTML = modalLoading();
        requestJson(cfg.api)
            .then(function (d) { allRows = normalizeArray(d); paint(); })
            .catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
        var sb = document.querySelector(".dk-modal-search-bar"); if (sb) sb.style.display = "none";
    }

    // ════════════════════════════════════════════════════════════════
    // v2.4.4 — Sortable table helper (cho Top5 VT)
    // ════════════════════════════════════════════════════════════════
    function renderSortableTable(cols, rows, opts) {
        opts = opts || {};
        var head = "<thead><tr>";
        for (var i = 0; i < cols.length; i++) {
            var c = cols[i];
            var thCls = c.sortable ? ' class="dk-sortable-th" data-sort-key="' + c.key + '"' : "";
            var thStyles = [];
            if (c.width) thStyles.push("width:" + c.width + "px");
            if (c.center) {
                thStyles.push("text-align:center");
            } else if (c.number !== undefined || c.percent) {
                thStyles.push("text-align:right");
            }
            var thStyleStr = thStyles.length ? ' style="' + thStyles.join(";") + '"' : "";
            head += '<th' + thCls + thStyleStr + '>' + escapeHtml(c.label) +
                (c.sortable ? ' <i class="fa-solid fa-sort dk-sort-icon"></i>' : "") +
                '</th>';
        }
        head += "</tr></thead>";
        return '<div class="dk-detail-table-wrap"><table class="dk-detail-table dk-sortable-table">' + head + '<tbody>' + sortableTbody(cols, rows, opts) + '</tbody></table></div>';
    }
    function sortableTbody(cols, rows, opts) {
        return rows.map(function (r, idx) {
            var trCls = (opts.highlightTopN && idx < opts.highlightTopN) ? ' class="dk-row-highlight-top"' : "";
            return '<tr' + trCls + '>' + cols.map(function (c) {
                var v = r[c.key];
                if (c.drillTo && c.key === "MaVT") {
                    return '<td class="dk-cell-mono"><a href="#" class="dk-link-inline" data-mavt-drill="1">' + escapeHtml(v || "") + '</a></td>';
                }
                if (c.number !== undefined) return '<td class="text-end dk-cell-num">' + formatNumber(v, c.number) + '</td>';
                if (c.percent) return '<td class="text-end dk-cell-num">' + formatNumber(v, 2) + '%</td>';
                if (c.center) return '<td class="text-center">' + escapeHtml(v) + '</td>';
                return '<td>' + escapeHtml(v) + '</td>';
            }).join("") + '</tr>';
        }).join("");
    }
    function wireSortableTable(wrapEl, cols, rows, opts) {
        if (!wrapEl) return;
        var ths = wrapEl.querySelectorAll(".dk-sortable-th");
        var sortKey = null, sortDir = 1;
        for (var i = 0; i < ths.length; i++) {
            ths[i].addEventListener("click", function () {
                var key = this.getAttribute("data-sort-key");
                if (sortKey === key) sortDir = -sortDir; else { sortKey = key; sortDir = -1; }
                rows.sort(function (a, b) { return (toNumber(a[sortKey]) - toNumber(b[sortKey])) * sortDir; });
                wrapEl.querySelectorAll(".dk-sort-icon").forEach(function (ic) { ic.className = "fa-solid fa-sort dk-sort-icon"; });
                this.querySelector(".dk-sort-icon").className = "fa-solid " + (sortDir > 0 ? "fa-sort-up" : "fa-sort-down") + " dk-sort-icon active";
                wrapEl.querySelector("tbody").innerHTML = sortableTbody(cols, rows, opts);
            });
        }
    }

    function bindDateFilter() {
        var fromEl = byId("dashFromDate");
        var toEl = byId("dashToDate");
        var btnEl = byId("dashApplyFilter");
        if (fromEl) fromEl.value = state.dateFilter.from;
        if (toEl) toEl.value = state.dateFilter.to;
        if (btnEl) {
            btnEl.addEventListener("click", function () {
                var f = fromEl ? fromEl.value : "";
                var t = toEl ? toEl.value : "";
                if (!f || !t) { showToast("Vui lòng nhập đủ Từ ngày và Đến ngày", "warn"); return; }
                if (f > t) { showToast("Từ ngày phải <= Đến ngày", "warn"); return; }
                state.dateFilter.from = f;
                state.dateFilter.to = t;
                state.loading = false;
                loadedPages = { 1: false, 2: false, 3: false };
                showToast("Đang áp dụng filter " + f + " → " + t + "...", "info");
                triggerSequentialReload();
            });
        }
        // v2.5.0 — Quick range chip buttons
        var chips = document.querySelectorAll(".dk-topbar-chip");
        for (var ci = 0; ci < chips.length; ci++) {
            (function (chip) {
                chip.addEventListener("click", function () {
                    // Clear active on all chips
                    for (var x = 0; x < chips.length; x++) chips[x].classList.remove("active");
                    chip.classList.add("active");
                    var range = chip.getAttribute("data-range");
                    var now = new Date();
                    var toDate = now.toISOString().slice(0, 10);
                    var fromDate;
                    if (range === "mtd") {
                        fromDate = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().slice(0, 10);
                    } else {
                        var days = parseInt(range, 10);
                        var d = new Date(now);
                        d.setDate(d.getDate() - days + 1);
                        fromDate = d.toISOString().slice(0, 10);
                    }
                    if (fromEl) fromEl.value = fromDate;
                    if (toEl) toEl.value = toDate;
                    state.dateFilter.from = fromDate;
                    state.dateFilter.to = toDate;
                    state.loading = false;
                    loadedPages = { 1: false, 2: false, 3: false };
                    showToast("Đang tải dữ liệu " + fromDate + " → " + toDate + "...", "info");
                    triggerSequentialReload();
                });
            })(chips[ci]);
        }
        // v2.5.0 — Topbar refresh button
        var btnRefreshTop = byId("btnRefreshTop");
        if (btnRefreshTop) {
            btnRefreshTop.addEventListener("click", function () {
                sessionStorage.setItem("dk_restore_tab", currentPage);
                requestJson("/api/DashboardKhoDesktop/ClearCache")
                    .then(function () {
                        window.location.reload();
                    })
                    .catch(function () {
                        window.location.reload();
                    });
            });
        }
    }

    function bindTodoPOHandlers() {
        document.addEventListener("click", function (e) {
            var tog = e.target.closest ? e.target.closest("[data-po-toggle]") : null;
            if (tog) {
                e.preventDefault();
                var po = tog.getAttribute("data-po-toggle");
                var rows = document.querySelectorAll('[data-po-sub="' + po.replace(/"/g, '\\"') + '"]');
                var isHidden = tog.classList.toggle("dk-collapsed");
                var icon = tog.querySelector("i");
                if (icon) icon.className = isHidden ? "fa-solid fa-chevron-right" : "fa-solid fa-chevron-down";
                for (var i = 0; i < rows.length; i++) rows[i].style.display = isHidden ? "none" : "";
                return;
            }
            var lnk = e.target.closest ? e.target.closest("[data-po-link]") : null;
            if (lnk) {
                e.preventDefault();
                showToast("Chi tiết PO sẽ triển khai sau (" + lnk.getAttribute("data-po-link") + ")", "info");
                return;
            }
        });
    }

    // v2.6.11 - Global Search All functionality
    function bindGlobalSearchAll() {
        var searchInput = document.getElementById("dkGlobalSearch");
        var searchDropdown = document.getElementById("dkSearchDropdown");
        var searchResults = document.getElementById("dkSearchResults");

        if (!searchInput || !searchDropdown || !searchResults) return;

        var dashboardSections = [
            { label: "Tồn đầu kỳ", icon: "fa-warehouse", elementId: "metricTonDauKy", page: 1 },
            { label: "Tổng nhập", icon: "fa-cloud-arrow-down", elementId: "metricTongNhap", page: 1 },
            { label: "Tổng xuất", icon: "fa-truck-fast", elementId: "metricTongXuat", page: 1 },
            { label: "Tồn kho", icon: "fa-cube", elementId: "metricTonKho", page: 1 },
            { label: "PO chuẩn bị về", icon: "fa-clipboard-list", elementId: "metricInboundReady", page: 1 },
            { label: "PO đã về kho", icon: "fa-truck-ramp-box", elementId: "metricPODangTre", page: 1 },
            { label: "Giá trị tồn kho", icon: "fa-sack-dollar", elementId: "metricThanhGia", page: 1 },
            { label: "Biểu đồ lấp đầy kho", icon: "fa-chart-pie", elementId: "chartCapacityRing", page: 1 },
            { label: "Tỷ trọng khách hàng theo CBM", icon: "fa-users", elementId: "chartCustomerPie", page: 1 },
            { label: "Công việc chờ xử lý", icon: "fa-clipboard-check", elementId: "todoList", page: 1 },
            { label: "Lấp đầy theo loại kho", icon: "fa-chart-bar", elementId: "chartCapacityBar", page: 1 },
            { label: "Tình hình kiểm kê", icon: "fa-check-double", elementId: "kiemKeBox", page: 1 },
            { label: "Giá trị tồn kho theo nhóm", icon: "fa-chart-pie", elementId: "chartGiaTriTheoNhom", page: 1 },
            { label: "Top 5 vật tư chiếm dung tích", icon: "fa-ranking-star", elementId: "chartCapacityBar", page: 1 },
            { label: "Top 5 khách hàng giá trị tồn", icon: "fa-building", elementId: "chartCustomerPie", page: 1 },
            { label: "Biểu đồ xuất nhập tồn", icon: "fa-chart-line", elementId: "chartFlowTrend", page: 2 },
            { label: "Top kệ sử dụng cao", icon: "fa-layer-group", elementId: "chartFlowTrend", page: 2 },
            { label: "Top 5 NL tồn kho nhiều nhất", icon: "fa-boxes-stacked", elementId: "chartTop5MaxNL", page: 2 },
            { label: "Top 5 PL tồn kho nhiều nhất", icon: "fa-boxes-stacked", elementId: "chartTop5MaxPL", page: 2 },
            { label: "Cảnh báo tồn kho", icon: "fa-triangle-exclamation", elementId: "alertsList", page: 2 },
            { label: "Tuổi tồn kho theo nhóm vật tư", icon: "fa-hourglass-half", elementId: "chartAgeStock", page: 2 },
            { label: "Hiệu suất hoạt động", icon: "fa-gauge-high", elementId: "chartVolumePie", page: 2 },
            { label: "Vật tư sắp hết hạn", icon: "fa-clock", elementId: "chartAgeStock", page: 2 },
            { label: "Lịch hoạt động kho", icon: "fa-calendar-days", elementId: "chartActivityCalendarMonthly", page: 3 }
        ];

        function highlightElement(el) {
            // Scroll to element
            el.scrollIntoView({ behavior: "smooth", block: "center" });

            // Add highlight class with strong glow
            el.classList.add("dk-nav-highlight");

            // Remove after animation completes
            setTimeout(function () {
                el.classList.remove("dk-nav-highlight");
            }, 3500);
        }

        function navigateToSection(section) {
            var targetPage = section.page || 1;
            // Use switchPage directly
            if (typeof switchPage === "function") {
                switchPage(targetPage);
            }
            setTimeout(function () {
                var panel = document.getElementById(section.elementId);
                if (panel) {
                    var target = panel.closest ? (panel.closest(".dk-panel") || panel.closest(".dk-metric-card") || panel) : panel;
                    highlightElement(target);
                }
            }, 500);
        }

        function filterSections(query) {
            var q = query.toLowerCase();
            var words = q.split(/\s+/);
            return dashboardSections.filter(function (s) {
                var lbl = s.label.toLowerCase();
                return words.every(function (w) { return lbl.indexOf(w) >= 0; });
            });
        }

        var timer;
        searchInput.addEventListener("input", function () {
            clearTimeout(timer);
            var query = this.value.trim();
            if (!query || query.length < 1) {
                searchDropdown.style.display = "none";
                return;
            }

            var sectionMatches = filterSections(query);
            var html = "";

            if (sectionMatches.length > 0) {
                html += '<div class="dk-search-group-title"><i class="fa-solid fa-compass" style="margin-right:5px"></i>Mục Dashboard</div>';
                html += '<ul class="dk-search-list">';
                sectionMatches.forEach(function (s) {
                    html += '<li class="dk-search-item dk-search-section" data-section-id="' + s.elementId + '" data-section-page="' + s.page + '">';
                    html += '  <div class="dk-search-item-title"><i class="fa-solid ' + s.icon + '" style="margin-right:6px;opacity:0.7;color:#60a5fa"></i>' + escapeHtml(s.label) + '</div>';
                    html += '  <div class="dk-search-item-desc"><i class="fa-solid fa-location-dot" style="margin-right:4px;font-size:10px"></i>Trang ' + s.page + ' — Nhấn để đi tới mục này</div>';
                    html += '</li>';
                });
                html += '</ul>';
            }

            searchResults.innerHTML = html;
            if (html) searchDropdown.style.display = "block";

            if (query.length >= 2) {
                timer = setTimeout(function () {
                    requestJson("/api/DashboardKhoDesktop/GlobalSearchAll?keyword=" + encodeURIComponent(query))
                        .then(function (res) {
                            var arr = normalizeArray(res);
                            var apiHtml = "";
                            if (arr && arr.length > 0) {
                                var grouped = {};
                                arr.forEach(function (item) {
                                    var cat = item.Category || "OTHER";
                                    if (!grouped[cat]) grouped[cat] = [];
                                    grouped[cat].push(item);
                                });
                                var catLabels = { PO: "PO & Đơn hàng", ITEM_RACK: "Vị trí hàng" };
                                var catTypes = { PO: "po", ITEM_RACK: "rack" };
                                Object.keys(grouped).forEach(function (cat) {
                                    apiHtml += '<div class="dk-search-group-title"><i class="fa-solid ' + (cat === "PO" ? "fa-file-invoice" : "fa-map-pin") + '" style="margin-right:5px"></i>' + escapeHtml(catLabels[cat] || cat) + '</div>';
                                    apiHtml += '<ul class="dk-search-list">';
                                    grouped[cat].forEach(function (item) {
                                        var type = catTypes[cat] || "other";
                                        apiHtml += '<li class="dk-search-item" data-type="' + type + '" data-id="' + escapeHtml(item.TargetID || "") + '" data-title="' + escapeHtml(item.Title || "") + '">';
                                        apiHtml += '  <div class="dk-search-item-title">' + escapeHtml(item.Title || "") + '</div>';
                                        apiHtml += '  <div class="dk-search-item-desc">' + escapeHtml(item.Subtitle || "") + '</div>';
                                        apiHtml += '</li>';
                                    });
                                    apiHtml += '</ul>';
                                });
                            }
                            var sectionBlock = searchResults.querySelector(".dk-search-group-title");
                            var sectionList = searchResults.querySelector(".dk-search-list");
                            var keepHtml = "";
                            if (sectionBlock) keepHtml += sectionBlock.outerHTML;
                            if (sectionList) keepHtml += sectionList.outerHTML;
                            searchResults.innerHTML = keepHtml + apiHtml;
                            searchDropdown.style.display = "block";
                        })
                        .catch(function () { });
                }, 300);
            }
        });

        document.addEventListener("click", function (e) {
            if (!searchInput.contains(e.target) && !searchDropdown.contains(e.target)) {
                searchDropdown.style.display = "none";
            }
        });

        searchResults.addEventListener("click", function (e) {
            var itemEl = e.target.closest ? e.target.closest(".dk-search-item") : null;
            if (!itemEl) return;

            if (itemEl.classList.contains("dk-search-section")) {
                var sectionId = itemEl.getAttribute("data-section-id");
                var sectionPage = parseInt(itemEl.getAttribute("data-section-page") || "1");
                searchDropdown.style.display = "none";
                searchInput.value = "";
                var matchedSection = dashboardSections.filter(function (s) { return s.elementId === sectionId && s.page === sectionPage; })[0];
                if (matchedSection) navigateToSection(matchedSection);
                return;
            }

            var type = itemEl.getAttribute("data-type");
            var id = itemEl.getAttribute("data-id");
            var title = itemEl.getAttribute("data-title");

            searchDropdown.style.display = "none";
            searchInput.value = title;

            if (type === "po") {
                requestJson("/api/DashboardKhoDesktop/GetChuanBiVe?tuNgay=2000-01-01&denNgay=2099-12-31&keyword=" + encodeURIComponent(id))
                    .then(function (res) {
                        var newItems = normalizeArray(res);
                        if (newItems && newItems.length > 0) {
                            newItems.forEach(function (ni) {
                                var exists = state.inbound.some(function (x) { return x.SoLo === ni.SoLo; });
                                if (!exists) state.inbound.unshift(ni);
                            });
                        }
                        if (typeof openDetail === "function") {
                            openDetail("inboundReady", 0);
                            setTimeout(function () {
                                var dInput = document.getElementById("detailSearchInput");
                                if (dInput && typeof filterDetailTable === "function") {
                                    dInput.value = title;
                                    filterDetailTable(title);
                                }
                            }, 400);
                        }
                    });
            } else if (type === "rack") {
                if (typeof openDetail === "function") {
                    openDetail("top5VTAll", 0);
                    setTimeout(function () {
                        var dInput = document.getElementById("detailSearchInput");
                        if (dInput && typeof filterDetailTable === "function") {
                            dInput.value = title;
                            filterDetailTable(title);
                        }
                    }, 800);
                }
            }
        });
    }

    function initFlatpickr() {
        if (typeof flatpickr !== 'undefined') {
            function bindPair(fromSelector, toSelector) {
                var fromEl = document.querySelector(fromSelector);
                var toEl = document.querySelector(toSelector);
                if (!fromEl || !toEl) return;

                var fpTo = flatpickr(toEl, {
                    dateFormat: "Y-m-d",
                    altInput: true,
                    altFormat: "d-m-Y",
                    altInputClass: toEl.className,
                    locale: "vn",
                    defaultDate: toEl.value,
                    onChange: function (selectedDates, dateStr) {
                        if (fpFrom) fpFrom.set("maxDate", dateStr);
                    }
                });
                var fpFrom = flatpickr(fromEl, {
                    dateFormat: "Y-m-d",
                    altInput: true,
                    altFormat: "d-m-Y",
                    altInputClass: fromEl.className,
                    locale: "vn",
                    defaultDate: fromEl.value,
                    onChange: function (selectedDates, dateStr) {
                        if (fpTo) fpTo.set("minDate", dateStr);
                    }
                });

                if (fromEl.value) fpTo.set("minDate", fromEl.value);
                if (toEl.value) fpFrom.set("maxDate", toEl.value);
            }

            bindPair("#dashFromDate", "#dashToDate");
            bindPair("#flowFromDate", "#flowToDate");
            bindPair("#calFromDate", "#calToDate");
        }
    }

    function init() {
        var restoreTab = sessionStorage.getItem("dk_restore_tab");
        if (restoreTab) {
            sessionStorage.removeItem("dk_restore_tab");

            // Defeat browser form auto-fill
            var inputsToClear = ["flowFromDate", "flowToDate", "calFromDate", "calToDate", "searchAll"];
            for (var i = 0; i < inputsToClear.length; i++) {
                var el = document.getElementById(inputsToClear[i]);
                if (el) el.value = "";
            }
            var selectsToClear = ["customerFilterSelect", "trendFilterSelect", "loadFilterSelect"];
            for (var j = 0; j < selectsToClear.length; j++) {
                var selEl = document.getElementById(selectsToClear[j]);
                if (selEl && selEl.options && selEl.options.length > 0) {
                    selEl.value = selEl.options[0].value;
                }
            }

            var chksToClear = ["chkActIn", "chkActOut", "chkActKK", "chkActPlan"];
            for (var k = 0; k < chksToClear.length; k++) {
                var chkEl = document.getElementById(chksToClear[k]);
                if (chkEl) chkEl.checked = true;
            }

            var tabId = parseInt(restoreTab, 10);
            if (!isNaN(tabId) && tabId >= 1 && tabId <= 3) {
                currentPage = tabId;
            }
        }

        bindEvents();
        bindPageNav();
        bindSidebarToggle();
        bindThemeToggle();
        bindPeriodSelector();
        bindDateFilter();
        bindTodoPOHandlers();
        bindGlobalSearchAll();
        renderClockNow();
        applyHighchartsTheme();
        initFlatpickr();
        setTimeout(injectMaximizeButtons, 50);

        switchPage(currentPage || 1);
        loadData();
        window.setInterval(renderClockNow, 1000);

        window.setInterval(function () {
            loadData();
        }, 300000);
    }

    init();
})();














