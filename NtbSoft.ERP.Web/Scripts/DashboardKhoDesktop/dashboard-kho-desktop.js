/* flatpickr v4.6.13,, @license MIT */
!function (e, n) { "object" == typeof exports && "undefined" != typeof module ? module.exports = n() : "function" == typeof define && define.amd ? define(n) : (e = "undefined" != typeof globalThis ? globalThis : e || self).flatpickr = n() }(this, (function () { "use strict"; var e = function () { return (e = Object.assign || function (e) { for (var n, t = 1, a = arguments.length; t < a; t++)for (var i in n = arguments[t]) Object.prototype.hasOwnProperty.call(n, i) && (e[i] = n[i]); return e }).apply(this, arguments) }; function n() { for (var e = 0, n = 0, t = arguments.length; n < t; n++)e += arguments[n].length; var a = Array(e), i = 0; for (n = 0; n < t; n++)for (var o = arguments[n], r = 0, l = o.length; r < l; r++, i++)a[i] = o[r]; return a } var t = ["onChange", "onClose", "onDayCreate", "onDestroy", "onKeyDown", "onMonthChange", "onOpen", "onParseConfig", "onReady", "onValueUpdate", "onYearChange", "onPreCalendarPosition"], a = { _disable: [], allowInput: !1, allowInvalidPreload: !1, altFormat: "F j, Y", altInput: !1, altInputClass: "form-control input", animate: "object" == typeof window && -1 === window.navigator.userAgent.indexOf("MSIE"), ariaDateFormat: "F j, Y", autoFillDefaultTime: !0, clickOpens: !0, closeOnSelect: !0, conjunction: ", ", dateFormat: "Y-m-d", defaultHour: 12, defaultMinute: 0, defaultSeconds: 0, disable: [], disableMobile: !1, enableSeconds: !1, enableTime: !1, errorHandler: function (e) { return "undefined" != typeof console && console.warn(e) }, getWeek: function (e) { var n = new Date(e.getTime()); n.setHours(0, 0, 0, 0), n.setDate(n.getDate() + 3 - (n.getDay() + 6) % 7); var t = new Date(n.getFullYear(), 0, 4); return 1 + Math.round(((n.getTime() - t.getTime()) / 864e5 - 3 + (t.getDay() + 6) % 7) / 7) }, hourIncrement: 1, ignoredFocusElements: [], inline: !1, locale: "default", minuteIncrement: 5, mode: "single", monthSelectorType: "dropdown", nextArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M13.207 8.472l-7.854 7.854-0.707-0.707 7.146-7.146-7.146-7.148 0.707-0.707 7.854 7.854z' /></svg>", noCalendar: !1, now: new Date, onChange: [], onClose: [], onDayCreate: [], onDestroy: [], onKeyDown: [], onMonthChange: [], onOpen: [], onParseConfig: [], onReady: [], onValueUpdate: [], onYearChange: [], onPreCalendarPosition: [], plugins: [], position: "auto", positionElement: void 0, prevArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M5.207 8.471l7.146 7.147-0.707 0.707-7.853-7.854 7.854-7.853 0.707 0.707-7.147 7.146z' /></svg>", shorthandCurrentMonth: !1, showMonths: 1, static: !1, time_24hr: !1, weekNumbers: !1, wrap: !1 }, i = { weekdays: { shorthand: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], longhand: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"] }, months: { shorthand: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], longhand: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"] }, daysInMonth: [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31], firstDayOfWeek: 0, ordinal: function (e) { var n = e % 100; if (n > 3 && n < 21) return "th"; switch (n % 10) { case 1: return "st"; case 2: return "nd"; case 3: return "rd"; default: return "th" } }, rangeSeparator: " to ", weekAbbreviation: "Wk", scrollTitle: "Scroll to increment", toggleTitle: "Click to toggle", amPM: ["AM", "PM"], yearAriaLabel: "Year", monthAriaLabel: "Month", hourAriaLabel: "Hour", minuteAriaLabel: "Minute", time_24hr: !1 }, o = function (e, n) { return void 0 === n && (n = 2), ("000" + e).slice(-1 * n) }, r = function (e) { return !0 === e ? 1 : 0 }; function l(e, n) { var t; return function () { var a = this, i = arguments; clearTimeout(t), t = setTimeout((function () { return e.apply(a, i) }), n) } } var c = function (e) { return e instanceof Array ? e : [e] }; function s(e, n, t) { if (!0 === t) return e.classList.add(n); e.classList.remove(n) } function d(e, n, t) { var a = window.document.createElement(e); return n = n || "", t = t || "", a.className = n, void 0 !== t && (a.textContent = t), a } function u(e) { for (; e.firstChild;)e.removeChild(e.firstChild) } function f(e, n) { return n(e) ? e : e.parentNode ? f(e.parentNode, n) : void 0 } function m(e, n) { var t = d("div", "numInputWrapper"), a = d("input", "numInput " + e), i = d("span", "arrowUp"), o = d("span", "arrowDown"); if (-1 === navigator.userAgent.indexOf("MSIE 9.0") ? a.type = "number" : (a.type = "text", a.pattern = "\\d*"), void 0 !== n) for (var r in n) a.setAttribute(r, n[r]); return t.appendChild(a), t.appendChild(i), t.appendChild(o), t } function g(e) { try { return "function" == typeof e.composedPath ? e.composedPath()[0] : e.target } catch (n) { return e.target } } var p = function () { }, h = function (e, n, t) { return t.months[n ? "shorthand" : "longhand"][e] }, v = { D: p, F: function (e, n, t) { e.setMonth(t.months.longhand.indexOf(n)) }, G: function (e, n) { e.setHours((e.getHours() >= 12 ? 12 : 0) + parseFloat(n)) }, H: function (e, n) { e.setHours(parseFloat(n)) }, J: function (e, n) { e.setDate(parseFloat(n)) }, K: function (e, n, t) { e.setHours(e.getHours() % 12 + 12 * r(new RegExp(t.amPM[1], "i").test(n))) }, M: function (e, n, t) { e.setMonth(t.months.shorthand.indexOf(n)) }, S: function (e, n) { e.setSeconds(parseFloat(n)) }, U: function (e, n) { return new Date(1e3 * parseFloat(n)) }, W: function (e, n, t) { var a = parseInt(n), i = new Date(e.getFullYear(), 0, 2 + 7 * (a - 1), 0, 0, 0, 0); return i.setDate(i.getDate() - i.getDay() + t.firstDayOfWeek), i }, Y: function (e, n) { e.setFullYear(parseFloat(n)) }, Z: function (e, n) { return new Date(n) }, d: function (e, n) { e.setDate(parseFloat(n)) }, h: function (e, n) { e.setHours((e.getHours() >= 12 ? 12 : 0) + parseFloat(n)) }, i: function (e, n) { e.setMinutes(parseFloat(n)) }, j: function (e, n) { e.setDate(parseFloat(n)) }, l: p, m: function (e, n) { e.setMonth(parseFloat(n) - 1) }, n: function (e, n) { e.setMonth(parseFloat(n) - 1) }, s: function (e, n) { e.setSeconds(parseFloat(n)) }, u: function (e, n) { return new Date(parseFloat(n)) }, w: p, y: function (e, n) { e.setFullYear(2e3 + parseFloat(n)) } }, D = { D: "", F: "", G: "(\\d\\d|\\d)", H: "(\\d\\d|\\d)", J: "(\\d\\d|\\d)\\w+", K: "", M: "", S: "(\\d\\d|\\d)", U: "(.+)", W: "(\\d\\d|\\d)", Y: "(\\d{4})", Z: "(.+)", d: "(\\d\\d|\\d)", h: "(\\d\\d|\\d)", i: "(\\d\\d|\\d)", j: "(\\d\\d|\\d)", l: "", m: "(\\d\\d|\\d)", n: "(\\d\\d|\\d)", s: "(\\d\\d|\\d)", u: "(.+)", w: "(\\d\\d|\\d)", y: "(\\d{2})" }, w = { Z: function (e) { return e.toISOString() }, D: function (e, n, t) { return n.weekdays.shorthand[w.w(e, n, t)] }, F: function (e, n, t) { return h(w.n(e, n, t) - 1, !1, n) }, G: function (e, n, t) { return o(w.h(e, n, t)) }, H: function (e) { return o(e.getHours()) }, J: function (e, n) { return void 0 !== n.ordinal ? e.getDate() + n.ordinal(e.getDate()) : e.getDate() }, K: function (e, n) { return n.amPM[r(e.getHours() > 11)] }, M: function (e, n) { return h(e.getMonth(), !0, n) }, S: function (e) { return o(e.getSeconds()) }, U: function (e) { return e.getTime() / 1e3 }, W: function (e, n, t) { return t.getWeek(e) }, Y: function (e) { return o(e.getFullYear(), 4) }, d: function (e) { return o(e.getDate()) }, h: function (e) { return e.getHours() % 12 ? e.getHours() % 12 : 12 }, i: function (e) { return o(e.getMinutes()) }, j: function (e) { return e.getDate() }, l: function (e, n) { return n.weekdays.longhand[e.getDay()] }, m: function (e) { return o(e.getMonth() + 1) }, n: function (e) { return e.getMonth() + 1 }, s: function (e) { return e.getSeconds() }, u: function (e) { return e.getTime() }, w: function (e) { return e.getDay() }, y: function (e) { return String(e.getFullYear()).substring(2) } }, b = function (e) { var n = e.config, t = void 0 === n ? a : n, o = e.l10n, r = void 0 === o ? i : o, l = e.isMobile, c = void 0 !== l && l; return function (e, n, a) { var i = a || r; return void 0 === t.formatDate || c ? n.split("").map((function (n, a, o) { return w[n] && "\\" !== o[a - 1] ? w[n](e, i, t) : "\\" !== n ? n : "" })).join("") : t.formatDate(e, n, i) } }, C = function (e) { var n = e.config, t = void 0 === n ? a : n, o = e.l10n, r = void 0 === o ? i : o; return function (e, n, i, o) { if (0 === e || e) { var l, c = o || r, s = e; if (e instanceof Date) l = new Date(e.getTime()); else if ("string" != typeof e && void 0 !== e.toFixed) l = new Date(e); else if ("string" == typeof e) { var d = n || (t || a).dateFormat, u = String(e).trim(); if ("today" === u) l = new Date, i = !0; else if (t && t.parseDate) l = t.parseDate(e, d); else if (/Z$/.test(u) || /GMT$/.test(u)) l = new Date(e); else { for (var f = void 0, m = [], g = 0, p = 0, h = ""; g < d.length; g++) { var w = d[g], b = "\\" === w, C = "\\" === d[g - 1] || b; if (D[w] && !C) { h += D[w]; var M = new RegExp(h).exec(e); M && (f = !0) && m["Y" !== w ? "push" : "unshift"]({ fn: v[w], val: M[++p] }) } else b || (h += ".") } l = t && t.noCalendar ? new Date((new Date).setHours(0, 0, 0, 0)) : new Date((new Date).getFullYear(), 0, 1, 0, 0, 0, 0), m.forEach((function (e) { var n = e.fn, t = e.val; return l = n(l, t, c) || l })), l = f ? l : void 0 } } if (l instanceof Date && !isNaN(l.getTime())) return !0 === i && l.setHours(0, 0, 0, 0), l; t.errorHandler(new Error("Invalid date provided: " + s)) } } }; function M(e, n, t) { return void 0 === t && (t = !0), !1 !== t ? new Date(e.getTime()).setHours(0, 0, 0, 0) - new Date(n.getTime()).setHours(0, 0, 0, 0) : e.getTime() - n.getTime() } var y = function (e, n, t) { return 3600 * e + 60 * n + t }, x = 864e5; function E(e) { var n = e.defaultHour, t = e.defaultMinute, a = e.defaultSeconds; if (void 0 !== e.minDate) { var i = e.minDate.getHours(), o = e.minDate.getMinutes(), r = e.minDate.getSeconds(); n < i && (n = i), n === i && t < o && (t = o), n === i && t === o && a < r && (a = e.minDate.getSeconds()) } if (void 0 !== e.maxDate) { var l = e.maxDate.getHours(), c = e.maxDate.getMinutes(); (n = Math.min(n, l)) === l && (t = Math.min(c, t)), n === l && t === c && (a = e.maxDate.getSeconds()) } return { hours: n, minutes: t, seconds: a } } "function" != typeof Object.assign && (Object.assign = function (e) { for (var n = [], t = 1; t < arguments.length; t++)n[t - 1] = arguments[t]; if (!e) throw TypeError("Cannot convert undefined or null to object"); for (var a = function (n) { n && Object.keys(n).forEach((function (t) { return e[t] = n[t] })) }, i = 0, o = n; i < o.length; i++) { var r = o[i]; a(r) } return e }); function k(p, v) { var w = { config: e(e({}, a), I.defaultConfig), l10n: i }; function k() { var e; return (null === (e = w.calendarContainer) || void 0 === e ? void 0 : e.getRootNode()).activeElement || document.activeElement } function T(e) { return e.bind(w) } function S() { var e = w.config; !1 === e.weekNumbers && 1 === e.showMonths || !0 !== e.noCalendar && window.requestAnimationFrame((function () { if (void 0 !== w.calendarContainer && (w.calendarContainer.style.visibility = "hidden", w.calendarContainer.style.display = "block"), void 0 !== w.daysContainer) { var n = (w.days.offsetWidth + 1) * e.showMonths; w.daysContainer.style.width = n + "px", w.calendarContainer.style.width = n + (void 0 !== w.weekWrapper ? w.weekWrapper.offsetWidth : 0) + "px", w.calendarContainer.style.removeProperty("visibility"), w.calendarContainer.style.removeProperty("display") } })) } function _(e) { if (0 === w.selectedDates.length) { var n = void 0 === w.config.minDate || M(new Date, w.config.minDate) >= 0 ? new Date : new Date(w.config.minDate.getTime()), t = E(w.config); n.setHours(t.hours, t.minutes, t.seconds, n.getMilliseconds()), w.selectedDates = [n], w.latestSelectedDateObj = n } void 0 !== e && "blur" !== e.type && function (e) { e.preventDefault(); var n = "keydown" === e.type, t = g(e), a = t; void 0 !== w.amPM && t === w.amPM && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]); var i = parseFloat(a.getAttribute("min")), l = parseFloat(a.getAttribute("max")), c = parseFloat(a.getAttribute("step")), s = parseInt(a.value, 10), d = e.delta || (n ? 38 === e.which ? 1 : -1 : 0), u = s + c * d; if (void 0 !== a.value && 2 === a.value.length) { var f = a === w.hourElement, m = a === w.minuteElement; u < i ? (u = l + u + r(!f) + (r(f) && r(!w.amPM)), m && L(void 0, -1, w.hourElement)) : u > l && (u = a === w.hourElement ? u - l - r(!w.amPM) : i, m && L(void 0, 1, w.hourElement)), w.amPM && f && (1 === c ? u + s === 23 : Math.abs(u - s) > c) && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]), a.value = o(u) } }(e); var a = w._input.value; O(), ye(), w._input.value !== a && w._debouncedChange() } function O() { if (void 0 !== w.hourElement && void 0 !== w.minuteElement) { var e, n, t = (parseInt(w.hourElement.value.slice(-2), 10) || 0) % 24, a = (parseInt(w.minuteElement.value, 10) || 0) % 60, i = void 0 !== w.secondElement ? (parseInt(w.secondElement.value, 10) || 0) % 60 : 0; void 0 !== w.amPM && (e = t, n = w.amPM.textContent, t = e % 12 + 12 * r(n === w.l10n.amPM[1])); var o = void 0 !== w.config.minTime || w.config.minDate && w.minDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.minDate, !0), l = void 0 !== w.config.maxTime || w.config.maxDate && w.maxDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.maxDate, !0); if (void 0 !== w.config.maxTime && void 0 !== w.config.minTime && w.config.minTime > w.config.maxTime) { var c = y(w.config.minTime.getHours(), w.config.minTime.getMinutes(), w.config.minTime.getSeconds()), s = y(w.config.maxTime.getHours(), w.config.maxTime.getMinutes(), w.config.maxTime.getSeconds()), d = y(t, a, i); if (d > s && d < c) { var u = function (e) { var n = Math.floor(e / 3600), t = (e - 3600 * n) / 60; return [n, t, e - 3600 * n - 60 * t] }(c); t = u[0], a = u[1], i = u[2] } } else { if (l) { var f = void 0 !== w.config.maxTime ? w.config.maxTime : w.config.maxDate; (t = Math.min(t, f.getHours())) === f.getHours() && (a = Math.min(a, f.getMinutes())), a === f.getMinutes() && (i = Math.min(i, f.getSeconds())) } if (o) { var m = void 0 !== w.config.minTime ? w.config.minTime : w.config.minDate; (t = Math.max(t, m.getHours())) === m.getHours() && a < m.getMinutes() && (a = m.getMinutes()), a === m.getMinutes() && (i = Math.max(i, m.getSeconds())) } } A(t, a, i) } } function F(e) { var n = e || w.latestSelectedDateObj; n && n instanceof Date && A(n.getHours(), n.getMinutes(), n.getSeconds()) } function A(e, n, t) { void 0 !== w.latestSelectedDateObj && w.latestSelectedDateObj.setHours(e % 24, n, t || 0, 0), w.hourElement && w.minuteElement && !w.isMobile && (w.hourElement.value = o(w.config.time_24hr ? e : (12 + e) % 12 + 12 * r(e % 12 == 0)), w.minuteElement.value = o(n), void 0 !== w.amPM && (w.amPM.textContent = w.l10n.amPM[r(e >= 12)]), void 0 !== w.secondElement && (w.secondElement.value = o(t))) } function N(e) { var n = g(e), t = parseInt(n.value) + (e.delta || 0); (t / 1e3 > 1 || "Enter" === e.key && !/[^\d]/.test(t.toString())) && ee(t) } function P(e, n, t, a) { return n instanceof Array ? n.forEach((function (n) { return P(e, n, t, a) })) : e instanceof Array ? e.forEach((function (e) { return P(e, n, t, a) })) : (e.addEventListener(n, t, a), void w._handlers.push({ remove: function () { return e.removeEventListener(n, t, a) } })) } function Y() { De("onChange") } function j(e, n) { var t = void 0 !== e ? w.parseDate(e) : w.latestSelectedDateObj || (w.config.minDate && w.config.minDate > w.now ? w.config.minDate : w.config.maxDate && w.config.maxDate < w.now ? w.config.maxDate : w.now), a = w.currentYear, i = w.currentMonth; try { void 0 !== t && (w.currentYear = t.getFullYear(), w.currentMonth = t.getMonth()) } catch (e) { e.message = "Invalid date supplied: " + t, w.config.errorHandler(e) } n && w.currentYear !== a && (De("onYearChange"), q()), !n || w.currentYear === a && w.currentMonth === i || De("onMonthChange"), w.redraw() } function H(e) { var n = g(e); ~n.className.indexOf("arrow") && L(e, n.classList.contains("arrowUp") ? 1 : -1) } function L(e, n, t) { var a = e && g(e), i = t || a && a.parentNode && a.parentNode.firstChild, o = we("increment"); o.delta = n, i && i.dispatchEvent(o) } function R(e, n, t, a) { var i = ne(n, !0), o = d("span", e, n.getDate().toString()); return o.dateObj = n, o.$i = a, o.setAttribute("aria-label", w.formatDate(n, w.config.ariaDateFormat)), -1 === e.indexOf("hidden") && 0 === M(n, w.now) && (w.todayDateElem = o, o.classList.add("today"), o.setAttribute("aria-current", "date")), i ? (o.tabIndex = -1, be(n) && (o.classList.add("selected"), w.selectedDateElem = o, "range" === w.config.mode && (s(o, "startRange", w.selectedDates[0] && 0 === M(n, w.selectedDates[0], !0)), s(o, "endRange", w.selectedDates[1] && 0 === M(n, w.selectedDates[1], !0)), "nextMonthDay" === e && o.classList.add("inRange")))) : o.classList.add("flatpickr-disabled"), "range" === w.config.mode && function (e) { return !("range" !== w.config.mode || w.selectedDates.length < 2) && (M(e, w.selectedDates[0]) >= 0 && M(e, w.selectedDates[1]) <= 0) }(n) && !be(n) && o.classList.add("inRange"), w.weekNumbers && 1 === w.config.showMonths && "prevMonthDay" !== e && a % 7 == 6 && w.weekNumbers.insertAdjacentHTML("beforeend", "<span class='flatpickr-day'>" + w.config.getWeek(n) + "</span>"), De("onDayCreate", o), o } function W(e) { e.focus(), "range" === w.config.mode && oe(e) } function B(e) { for (var n = e > 0 ? 0 : w.config.showMonths - 1, t = e > 0 ? w.config.showMonths : -1, a = n; a != t; a += e)for (var i = w.daysContainer.children[a], o = e > 0 ? 0 : i.children.length - 1, r = e > 0 ? i.children.length : -1, l = o; l != r; l += e) { var c = i.children[l]; if (-1 === c.className.indexOf("hidden") && ne(c.dateObj)) return c } } function J(e, n) { var t = k(), a = te(t || document.body), i = void 0 !== e ? e : a ? t : void 0 !== w.selectedDateElem && te(w.selectedDateElem) ? w.selectedDateElem : void 0 !== w.todayDateElem && te(w.todayDateElem) ? w.todayDateElem : B(n > 0 ? 1 : -1); void 0 === i ? w._input.focus() : a ? function (e, n) { for (var t = -1 === e.className.indexOf("Month") ? e.dateObj.getMonth() : w.currentMonth, a = n > 0 ? w.config.showMonths : -1, i = n > 0 ? 1 : -1, o = t - w.currentMonth; o != a; o += i)for (var r = w.daysContainer.children[o], l = t - w.currentMonth === o ? e.$i + n : n < 0 ? r.children.length - 1 : 0, c = r.children.length, s = l; s >= 0 && s < c && s != (n > 0 ? c : -1); s += i) { var d = r.children[s]; if (-1 === d.className.indexOf("hidden") && ne(d.dateObj) && Math.abs(e.$i - s) >= Math.abs(n)) return W(d) } w.changeMonth(i), J(B(i), 0) }(i, n) : W(i) } function K(e, n) { for (var t = (new Date(e, n, 1).getDay() - w.l10n.firstDayOfWeek + 7) % 7, a = w.utils.getDaysInMonth((n - 1 + 12) % 12, e), i = w.utils.getDaysInMonth(n, e), o = window.document.createDocumentFragment(), r = w.config.showMonths > 1, l = r ? "prevMonthDay hidden" : "prevMonthDay", c = r ? "nextMonthDay hidden" : "nextMonthDay", s = a + 1 - t, u = 0; s <= a; s++, u++)o.appendChild(R("flatpickr-day " + l, new Date(e, n - 1, s), 0, u)); for (s = 1; s <= i; s++, u++)o.appendChild(R("flatpickr-day", new Date(e, n, s), 0, u)); for (var f = i + 1; f <= 42 - t && (1 === w.config.showMonths || u % 7 != 0); f++, u++)o.appendChild(R("flatpickr-day " + c, new Date(e, n + 1, f % i), 0, u)); var m = d("div", "dayContainer"); return m.appendChild(o), m } function U() { if (void 0 !== w.daysContainer) { u(w.daysContainer), w.weekNumbers && u(w.weekNumbers); for (var e = document.createDocumentFragment(), n = 0; n < w.config.showMonths; n++) { var t = new Date(w.currentYear, w.currentMonth, 1); t.setMonth(w.currentMonth + n), e.appendChild(K(t.getFullYear(), t.getMonth())) } w.daysContainer.appendChild(e), w.days = w.daysContainer.firstChild, "range" === w.config.mode && 1 === w.selectedDates.length && oe() } } function q() { if (!(w.config.showMonths > 1 || "dropdown" !== w.config.monthSelectorType)) { var e = function (e) { return !(void 0 !== w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && e < w.config.minDate.getMonth()) && !(void 0 !== w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() && e > w.config.maxDate.getMonth()) }; w.monthsDropdownContainer.tabIndex = -1, w.monthsDropdownContainer.innerHTML = ""; for (var n = 0; n < 12; n++)if (e(n)) { var t = d("option", "flatpickr-monthDropdown-month"); t.value = new Date(w.currentYear, n).getMonth().toString(), t.textContent = h(n, w.config.shorthandCurrentMonth, w.l10n), t.tabIndex = -1, w.currentMonth === n && (t.selected = !0), w.monthsDropdownContainer.appendChild(t) } } } function $() { var e, n = d("div", "flatpickr-month"), t = window.document.createDocumentFragment(); w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? e = d("span", "cur-month") : (w.monthsDropdownContainer = d("select", "flatpickr-monthDropdown-months"), w.monthsDropdownContainer.setAttribute("aria-label", w.l10n.monthAriaLabel), P(w.monthsDropdownContainer, "change", (function (e) { var n = g(e), t = parseInt(n.value, 10); w.changeMonth(t - w.currentMonth), De("onMonthChange") })), q(), e = w.monthsDropdownContainer); var a = m("cur-year", { tabindex: "-1" }), i = a.getElementsByTagName("input")[0]; i.setAttribute("aria-label", w.l10n.yearAriaLabel), w.config.minDate && i.setAttribute("min", w.config.minDate.getFullYear().toString()), w.config.maxDate && (i.setAttribute("max", w.config.maxDate.getFullYear().toString()), i.disabled = !!w.config.minDate && w.config.minDate.getFullYear() === w.config.maxDate.getFullYear()); var o = d("div", "flatpickr-current-month"); return o.appendChild(e), o.appendChild(a), t.appendChild(o), n.appendChild(t), { container: n, yearElement: i, monthElement: e } } function V() { u(w.monthNav), w.monthNav.appendChild(w.prevMonthNav), w.config.showMonths && (w.yearElements = [], w.monthElements = []); for (var e = w.config.showMonths; e--;) { var n = $(); w.yearElements.push(n.yearElement), w.monthElements.push(n.monthElement), w.monthNav.appendChild(n.container) } w.monthNav.appendChild(w.nextMonthNav) } function z() { w.weekdayContainer ? u(w.weekdayContainer) : w.weekdayContainer = d("div", "flatpickr-weekdays"); for (var e = w.config.showMonths; e--;) { var n = d("div", "flatpickr-weekdaycontainer"); w.weekdayContainer.appendChild(n) } return G(), w.weekdayContainer } function G() { if (w.weekdayContainer) { var e = w.l10n.firstDayOfWeek, t = n(w.l10n.weekdays.shorthand); e > 0 && e < t.length && (t = n(t.splice(e, t.length), t.splice(0, e))); for (var a = w.config.showMonths; a--;)w.weekdayContainer.children[a].innerHTML = "\n      <span class='flatpickr-weekday'>\n        " + t.join("</span><span class='flatpickr-weekday'>") + "\n      </span>\n      " } } function Z(e, n) { void 0 === n && (n = !0); var t = n ? e : e - w.currentMonth; t < 0 && !0 === w._hidePrevMonthArrow || t > 0 && !0 === w._hideNextMonthArrow || (w.currentMonth += t, (w.currentMonth < 0 || w.currentMonth > 11) && (w.currentYear += w.currentMonth > 11 ? 1 : -1, w.currentMonth = (w.currentMonth + 12) % 12, De("onYearChange"), q()), U(), De("onMonthChange"), Ce()) } function Q(e) { return w.calendarContainer.contains(e) } function X(e) { if (w.isOpen && !w.config.inline) { var n = g(e), t = Q(n), a = !(n === w.input || n === w.altInput || w.element.contains(n) || e.path && e.path.indexOf && (~e.path.indexOf(w.input) || ~e.path.indexOf(w.altInput))) && !t && !Q(e.relatedTarget), i = !w.config.ignoredFocusElements.some((function (e) { return e.contains(n) })); a && i && (w.config.allowInput && w.setDate(w._input.value, !1, w.config.altInput ? w.config.altFormat : w.config.dateFormat), void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement && "" !== w.input.value && void 0 !== w.input.value && _(), w.close(), w.config && "range" === w.config.mode && 1 === w.selectedDates.length && w.clear(!1)) } } function ee(e) { if (!(!e || w.config.minDate && e < w.config.minDate.getFullYear() || w.config.maxDate && e > w.config.maxDate.getFullYear())) { var n = e, t = w.currentYear !== n; w.currentYear = n || w.currentYear, w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth = Math.min(w.config.maxDate.getMonth(), w.currentMonth) : w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && (w.currentMonth = Math.max(w.config.minDate.getMonth(), w.currentMonth)), t && (w.redraw(), De("onYearChange"), q()) } } function ne(e, n) { var t; void 0 === n && (n = !0); var a = w.parseDate(e, void 0, n); if (w.config.minDate && a && M(a, w.config.minDate, void 0 !== n ? n : !w.minDateHasTime) < 0 || w.config.maxDate && a && M(a, w.config.maxDate, void 0 !== n ? n : !w.maxDateHasTime) > 0) return !1; if (!w.config.enable && 0 === w.config.disable.length) return !0; if (void 0 === a) return !1; for (var i = !!w.config.enable, o = null !== (t = w.config.enable) && void 0 !== t ? t : w.config.disable, r = 0, l = void 0; r < o.length; r++) { if ("function" == typeof (l = o[r]) && l(a)) return i; if (l instanceof Date && void 0 !== a && l.getTime() === a.getTime()) return i; if ("string" == typeof l) { var c = w.parseDate(l, void 0, !0); return c && c.getTime() === a.getTime() ? i : !i } if ("object" == typeof l && void 0 !== a && l.from && l.to && a.getTime() >= l.from.getTime() && a.getTime() <= l.to.getTime()) return i } return !i } function te(e) { return void 0 !== w.daysContainer && (-1 === e.className.indexOf("hidden") && -1 === e.className.indexOf("flatpickr-disabled") && w.daysContainer.contains(e)) } function ae(e) { var n = e.target === w._input, t = w._input.value.trimEnd() !== Me(); !n || !t || e.relatedTarget && Q(e.relatedTarget) || w.setDate(w._input.value, !0, e.target === w.altInput ? w.config.altFormat : w.config.dateFormat) } function ie(e) { var n = g(e), t = w.config.wrap ? p.contains(n) : n === w._input, a = w.config.allowInput, i = w.isOpen && (!a || !t), o = w.config.inline && t && !a; if (13 === e.keyCode && t) { if (a) return w.setDate(w._input.value, !0, n === w.altInput ? w.config.altFormat : w.config.dateFormat), w.close(), n.blur(); w.open() } else if (Q(n) || i || o) { var r = !!w.timeContainer && w.timeContainer.contains(n); switch (e.keyCode) { case 13: r ? (e.preventDefault(), _(), fe()) : me(e); break; case 27: e.preventDefault(), fe(); break; case 8: case 46: t && !w.config.allowInput && (e.preventDefault(), w.clear()); break; case 37: case 39: if (r || t) w.hourElement && w.hourElement.focus(); else { e.preventDefault(); var l = k(); if (void 0 !== w.daysContainer && (!1 === a || l && te(l))) { var c = 39 === e.keyCode ? 1 : -1; e.ctrlKey ? (e.stopPropagation(), Z(c), J(B(1), 0)) : J(void 0, c) } } break; case 38: case 40: e.preventDefault(); var s = 40 === e.keyCode ? 1 : -1; w.daysContainer && void 0 !== n.$i || n === w.input || n === w.altInput ? e.ctrlKey ? (e.stopPropagation(), ee(w.currentYear - s), J(B(1), 0)) : r || J(void 0, 7 * s) : n === w.currentYearElement ? ee(w.currentYear - s) : w.config.enableTime && (!r && w.hourElement && w.hourElement.focus(), _(e), w._debouncedChange()); break; case 9: if (r) { var d = [w.hourElement, w.minuteElement, w.secondElement, w.amPM].concat(w.pluginElements).filter((function (e) { return e })), u = d.indexOf(n); if (-1 !== u) { var f = d[u + (e.shiftKey ? -1 : 1)]; e.preventDefault(), (f || w._input).focus() } } else !w.config.noCalendar && w.daysContainer && w.daysContainer.contains(n) && e.shiftKey && (e.preventDefault(), w._input.focus()) } } if (void 0 !== w.amPM && n === w.amPM) switch (e.key) { case w.l10n.amPM[0].charAt(0): case w.l10n.amPM[0].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[0], O(), ye(); break; case w.l10n.amPM[1].charAt(0): case w.l10n.amPM[1].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[1], O(), ye() }(t || Q(n)) && De("onKeyDown", e) } function oe(e, n) { if (void 0 === n && (n = "flatpickr-day"), 1 === w.selectedDates.length && (!e || e.classList.contains(n) && !e.classList.contains("flatpickr-disabled"))) { for (var t = e ? e.dateObj.getTime() : w.days.firstElementChild.dateObj.getTime(), a = w.parseDate(w.selectedDates[0], void 0, !0).getTime(), i = Math.min(t, w.selectedDates[0].getTime()), o = Math.max(t, w.selectedDates[0].getTime()), r = !1, l = 0, c = 0, s = i; s < o; s += x)ne(new Date(s), !0) || (r = r || s > i && s < o, s < a && (!l || s > l) ? l = s : s > a && (!c || s < c) && (c = s)); Array.from(w.rContainer.querySelectorAll("*:nth-child(-n+" + w.config.showMonths + ") > ." + n)).forEach((function (n) { var i, o, s, d = n.dateObj.getTime(), u = l > 0 && d < l || c > 0 && d > c; if (u) return n.classList.add("notAllowed"), void ["inRange", "startRange", "endRange"].forEach((function (e) { n.classList.remove(e) })); r && !u || (["startRange", "inRange", "endRange", "notAllowed"].forEach((function (e) { n.classList.remove(e) })), void 0 !== e && (e.classList.add(t <= w.selectedDates[0].getTime() ? "startRange" : "endRange"), a < t && d === a ? n.classList.add("startRange") : a > t && d === a && n.classList.add("endRange"), d >= l && (0 === c || d <= c) && (o = a, s = t, (i = d) > Math.min(o, s) && i < Math.max(o, s)) && n.classList.add("inRange"))) })) } } function re() { !w.isOpen || w.config.static || w.config.inline || de() } function le(e) { return function (n) { var t = w.config["_" + e + "Date"] = w.parseDate(n, w.config.dateFormat), a = w.config["_" + ("min" === e ? "max" : "min") + "Date"]; void 0 !== t && (w["min" === e ? "minDateHasTime" : "maxDateHasTime"] = t.getHours() > 0 || t.getMinutes() > 0 || t.getSeconds() > 0), w.selectedDates && (w.selectedDates = w.selectedDates.filter((function (e) { return ne(e) })), w.selectedDates.length || "min" !== e || F(t), ye()), w.daysContainer && (ue(), void 0 !== t ? w.currentYearElement[e] = t.getFullYear().toString() : w.currentYearElement.removeAttribute(e), w.currentYearElement.disabled = !!a && void 0 !== t && a.getFullYear() === t.getFullYear()) } } function ce() { return w.config.wrap ? p.querySelector("[data-input]") : p } function se() { "object" != typeof w.config.locale && void 0 === I.l10ns[w.config.locale] && w.config.errorHandler(new Error("flatpickr: invalid locale " + w.config.locale)), w.l10n = e(e({}, I.l10ns.default), "object" == typeof w.config.locale ? w.config.locale : "default" !== w.config.locale ? I.l10ns[w.config.locale] : void 0), D.D = "(" + w.l10n.weekdays.shorthand.join("|") + ")", D.l = "(" + w.l10n.weekdays.longhand.join("|") + ")", D.M = "(" + w.l10n.months.shorthand.join("|") + ")", D.F = "(" + w.l10n.months.longhand.join("|") + ")", D.K = "(" + w.l10n.amPM[0] + "|" + w.l10n.amPM[1] + "|" + w.l10n.amPM[0].toLowerCase() + "|" + w.l10n.amPM[1].toLowerCase() + ")", void 0 === e(e({}, v), JSON.parse(JSON.stringify(p.dataset || {}))).time_24hr && void 0 === I.defaultConfig.time_24hr && (w.config.time_24hr = w.l10n.time_24hr), w.formatDate = b(w), w.parseDate = C({ config: w.config, l10n: w.l10n }) } function de(e) { if ("function" != typeof w.config.position) { if (void 0 !== w.calendarContainer) { De("onPreCalendarPosition"); var n = e || w._positionElement, t = Array.prototype.reduce.call(w.calendarContainer.children, (function (e, n) { return e + n.offsetHeight }), 0), a = w.calendarContainer.offsetWidth, i = w.config.position.split(" "), o = i[0], r = i.length > 1 ? i[1] : null, l = n.getBoundingClientRect(), c = window.innerHeight - l.bottom, d = "above" === o || "below" !== o && c < t && l.top > t, u = window.pageYOffset + l.top + (d ? -t - 2 : n.offsetHeight + 2); if (s(w.calendarContainer, "arrowTop", !d), s(w.calendarContainer, "arrowBottom", d), !w.config.inline) { var f = window.pageXOffset + l.left, m = !1, g = !1; "center" === r ? (f -= (a - l.width) / 2, m = !0) : "right" === r && (f -= a - l.width, g = !0), s(w.calendarContainer, "arrowLeft", !m && !g), s(w.calendarContainer, "arrowCenter", m), s(w.calendarContainer, "arrowRight", g); var p = window.document.body.offsetWidth - (window.pageXOffset + l.right), h = f + a > window.document.body.offsetWidth, v = p + a > window.document.body.offsetWidth; if (s(w.calendarContainer, "rightMost", h), !w.config.static) if (w.calendarContainer.style.top = u + "px", h) if (v) { var D = function () { for (var e = null, n = 0; n < document.styleSheets.length; n++) { var t = document.styleSheets[n]; if (t.cssRules) { try { t.cssRules } catch (e) { continue } e = t; break } } return null != e ? e : (a = document.createElement("style"), document.head.appendChild(a), a.sheet); var a }(); if (void 0 === D) return; var b = window.document.body.offsetWidth, C = Math.max(0, b / 2 - a / 2), M = D.cssRules.length, y = "{left:" + l.left + "px;right:auto;}"; s(w.calendarContainer, "rightMost", !1), s(w.calendarContainer, "centerMost", !0), D.insertRule(".flatpickr-calendar.centerMost:before,.flatpickr-calendar.centerMost:after" + y, M), w.calendarContainer.style.left = C + "px", w.calendarContainer.style.right = "auto" } else w.calendarContainer.style.left = "auto", w.calendarContainer.style.right = p + "px"; else w.calendarContainer.style.left = f + "px", w.calendarContainer.style.right = "auto" } } } else w.config.position(w, e) } function ue() { w.config.noCalendar || w.isMobile || (q(), Ce(), U()) } function fe() { w._input.focus(), -1 !== window.navigator.userAgent.indexOf("MSIE") || void 0 !== navigator.msMaxTouchPoints ? setTimeout(w.close, 0) : w.close() } function me(e) { e.preventDefault(), e.stopPropagation(); var n = f(g(e), (function (e) { return e.classList && e.classList.contains("flatpickr-day") && !e.classList.contains("flatpickr-disabled") && !e.classList.contains("notAllowed") })); if (void 0 !== n) { var t = n, a = w.latestSelectedDateObj = new Date(t.dateObj.getTime()), i = (a.getMonth() < w.currentMonth || a.getMonth() > w.currentMonth + w.config.showMonths - 1) && "range" !== w.config.mode; if (w.selectedDateElem = t, "single" === w.config.mode) w.selectedDates = [a]; else if ("multiple" === w.config.mode) { var o = be(a); o ? w.selectedDates.splice(parseInt(o), 1) : w.selectedDates.push(a) } else "range" === w.config.mode && (2 === w.selectedDates.length && w.clear(!1, !1), w.latestSelectedDateObj = a, w.selectedDates.push(a), 0 !== M(a, w.selectedDates[0], !0) && w.selectedDates.sort((function (e, n) { return e.getTime() - n.getTime() }))); if (O(), i) { var r = w.currentYear !== a.getFullYear(); w.currentYear = a.getFullYear(), w.currentMonth = a.getMonth(), r && (De("onYearChange"), q()), De("onMonthChange") } if (Ce(), U(), ye(), i || "range" === w.config.mode || 1 !== w.config.showMonths ? void 0 !== w.selectedDateElem && void 0 === w.hourElement && w.selectedDateElem && w.selectedDateElem.focus() : W(t), void 0 !== w.hourElement && void 0 !== w.hourElement && w.hourElement.focus(), w.config.closeOnSelect) { var l = "single" === w.config.mode && !w.config.enableTime, c = "range" === w.config.mode && 2 === w.selectedDates.length && !w.config.enableTime; (l || c) && fe() } Y() } } w.parseDate = C({ config: w.config, l10n: w.l10n }), w._handlers = [], w.pluginElements = [], w.loadedPlugins = [], w._bind = P, w._setHoursFromDate = F, w._positionCalendar = de, w.changeMonth = Z, w.changeYear = ee, w.clear = function (e, n) { void 0 === e && (e = !0); void 0 === n && (n = !0); w.input.value = "", void 0 !== w.altInput && (w.altInput.value = ""); void 0 !== w.mobileInput && (w.mobileInput.value = ""); w.selectedDates = [], w.latestSelectedDateObj = void 0, !0 === n && (w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth()); if (!0 === w.config.enableTime) { var t = E(w.config), a = t.hours, i = t.minutes, o = t.seconds; A(a, i, o) } w.redraw(), e && De("onChange") }, w.close = function () { w.isOpen = !1, w.isMobile || (void 0 !== w.calendarContainer && w.calendarContainer.classList.remove("open"), void 0 !== w._input && w._input.classList.remove("active")); De("onClose") }, w.onMouseOver = oe, w._createElement = d, w.createDay = R, w.destroy = function () { void 0 !== w.config && De("onDestroy"); for (var e = w._handlers.length; e--;)w._handlers[e].remove(); if (w._handlers = [], w.mobileInput) w.mobileInput.parentNode && w.mobileInput.parentNode.removeChild(w.mobileInput), w.mobileInput = void 0; else if (w.calendarContainer && w.calendarContainer.parentNode) if (w.config.static && w.calendarContainer.parentNode) { var n = w.calendarContainer.parentNode; if (n.lastChild && n.removeChild(n.lastChild), n.parentNode) { for (; n.firstChild;)n.parentNode.insertBefore(n.firstChild, n); n.parentNode.removeChild(n) } } else w.calendarContainer.parentNode.removeChild(w.calendarContainer); w.altInput && (w.input.type = "text", w.altInput.parentNode && w.altInput.parentNode.removeChild(w.altInput), delete w.altInput); w.input && (w.input.type = w.input._type, w.input.classList.remove("flatpickr-input"), w.input.removeAttribute("readonly"));["_showTimeInput", "latestSelectedDateObj", "_hideNextMonthArrow", "_hidePrevMonthArrow", "__hideNextMonthArrow", "__hidePrevMonthArrow", "isMobile", "isOpen", "selectedDateElem", "minDateHasTime", "maxDateHasTime", "days", "daysContainer", "_input", "_positionElement", "innerContainer", "rContainer", "monthNav", "todayDateElem", "calendarContainer", "weekdayContainer", "prevMonthNav", "nextMonthNav", "monthsDropdownContainer", "currentMonthElement", "currentYearElement", "navigationCurrentMonth", "selectedDateElem", "config"].forEach((function (e) { try { delete w[e] } catch (e) { } })) }, w.isEnabled = ne, w.jumpToDate = j, w.updateValue = ye, w.open = function (e, n) { void 0 === n && (n = w._positionElement); if (!0 === w.isMobile) { if (e) { e.preventDefault(); var t = g(e); t && t.blur() } return void 0 !== w.mobileInput && (w.mobileInput.focus(), w.mobileInput.click()), void De("onOpen") } if (w._input.disabled || w.config.inline) return; var a = w.isOpen; w.isOpen = !0, a || (w.calendarContainer.classList.add("open"), w._input.classList.add("active"), De("onOpen"), de(n)); !0 === w.config.enableTime && !0 === w.config.noCalendar && (!1 !== w.config.allowInput || void 0 !== e && w.timeContainer.contains(e.relatedTarget) || setTimeout((function () { return w.hourElement.select() }), 50)) }, w.redraw = ue, w.set = function (e, n) { if (null !== e && "object" == typeof e) for (var a in Object.assign(w.config, e), e) void 0 !== ge[a] && ge[a].forEach((function (e) { return e() })); else w.config[e] = n, void 0 !== ge[e] ? ge[e].forEach((function (e) { return e() })) : t.indexOf(e) > -1 && (w.config[e] = c(n)); w.redraw(), ye(!0) }, w.setDate = function (e, n, t) { void 0 === n && (n = !1); void 0 === t && (t = w.config.dateFormat); if (0 !== e && !e || e instanceof Array && 0 === e.length) return w.clear(n); pe(e, t), w.latestSelectedDateObj = w.selectedDates[w.selectedDates.length - 1], w.redraw(), j(void 0, n), F(), 0 === w.selectedDates.length && w.clear(!1); ye(n), n && De("onChange") }, w.toggle = function (e) { if (!0 === w.isOpen) return w.close(); w.open(e) }; var ge = { locale: [se, G], showMonths: [V, S, z], minDate: [j], maxDate: [j], positionElement: [ve], clickOpens: [function () { !0 === w.config.clickOpens ? (P(w._input, "focus", w.open), P(w._input, "click", w.open)) : (w._input.removeEventListener("focus", w.open), w._input.removeEventListener("click", w.open)) }] }; function pe(e, n) { var t = []; if (e instanceof Array) t = e.map((function (e) { return w.parseDate(e, n) })); else if (e instanceof Date || "number" == typeof e) t = [w.parseDate(e, n)]; else if ("string" == typeof e) switch (w.config.mode) { case "single": case "time": t = [w.parseDate(e, n)]; break; case "multiple": t = e.split(w.config.conjunction).map((function (e) { return w.parseDate(e, n) })); break; case "range": t = e.split(w.l10n.rangeSeparator).map((function (e) { return w.parseDate(e, n) })) } else w.config.errorHandler(new Error("Invalid date supplied: " + JSON.stringify(e))); w.selectedDates = w.config.allowInvalidPreload ? t : t.filter((function (e) { return e instanceof Date && ne(e, !1) })), "range" === w.config.mode && w.selectedDates.sort((function (e, n) { return e.getTime() - n.getTime() })) } function he(e) { return e.slice().map((function (e) { return "string" == typeof e || "number" == typeof e || e instanceof Date ? w.parseDate(e, void 0, !0) : e && "object" == typeof e && e.from && e.to ? { from: w.parseDate(e.from, void 0), to: w.parseDate(e.to, void 0) } : e })).filter((function (e) { return e })) } function ve() { w._positionElement = w.config.positionElement || w._input } function De(e, n) { if (void 0 !== w.config) { var t = w.config[e]; if (void 0 !== t && t.length > 0) for (var a = 0; t[a] && a < t.length; a++)t[a](w.selectedDates, w.input.value, w, n); "onChange" === e && (w.input.dispatchEvent(we("change")), w.input.dispatchEvent(we("input"))) } } function we(e) { var n = document.createEvent("Event"); return n.initEvent(e, !0, !0), n } function be(e) { for (var n = 0; n < w.selectedDates.length; n++) { var t = w.selectedDates[n]; if (t instanceof Date && 0 === M(t, e)) return "" + n } return !1 } function Ce() { w.config.noCalendar || w.isMobile || !w.monthNav || (w.yearElements.forEach((function (e, n) { var t = new Date(w.currentYear, w.currentMonth, 1); t.setMonth(w.currentMonth + n), w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? w.monthElements[n].textContent = h(t.getMonth(), w.config.shorthandCurrentMonth, w.l10n) + " " : w.monthsDropdownContainer.value = t.getMonth().toString(), e.value = t.getFullYear().toString() })), w._hidePrevMonthArrow = void 0 !== w.config.minDate && (w.currentYear === w.config.minDate.getFullYear() ? w.currentMonth <= w.config.minDate.getMonth() : w.currentYear < w.config.minDate.getFullYear()), w._hideNextMonthArrow = void 0 !== w.config.maxDate && (w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth + 1 > w.config.maxDate.getMonth() : w.currentYear > w.config.maxDate.getFullYear())) } function Me(e) { var n = e || (w.config.altInput ? w.config.altFormat : w.config.dateFormat); return w.selectedDates.map((function (e) { return w.formatDate(e, n) })).filter((function (e, n, t) { return "range" !== w.config.mode || w.config.enableTime || t.indexOf(e) === n })).join("range" !== w.config.mode ? w.config.conjunction : w.l10n.rangeSeparator) } function ye(e) { void 0 === e && (e = !0), void 0 !== w.mobileInput && w.mobileFormatStr && (w.mobileInput.value = void 0 !== w.latestSelectedDateObj ? w.formatDate(w.latestSelectedDateObj, w.mobileFormatStr) : ""), w.input.value = Me(w.config.dateFormat), void 0 !== w.altInput && (w.altInput.value = Me(w.config.altFormat)), !1 !== e && De("onValueUpdate") } function xe(e) { var n = g(e), t = w.prevMonthNav.contains(n), a = w.nextMonthNav.contains(n); t || a ? Z(t ? -1 : 1) : w.yearElements.indexOf(n) >= 0 ? n.select() : n.classList.contains("arrowUp") ? w.changeYear(w.currentYear + 1) : n.classList.contains("arrowDown") && w.changeYear(w.currentYear - 1) } return function () { w.element = w.input = p, w.isOpen = !1, function () { var n = ["wrap", "weekNumbers", "allowInput", "allowInvalidPreload", "clickOpens", "time_24hr", "enableTime", "noCalendar", "altInput", "shorthandCurrentMonth", "inline", "static", "enableSeconds", "disableMobile"], i = e(e({}, JSON.parse(JSON.stringify(p.dataset || {}))), v), o = {}; w.config.parseDate = i.parseDate, w.config.formatDate = i.formatDate, Object.defineProperty(w.config, "enable", { get: function () { return w.config._enable }, set: function (e) { w.config._enable = he(e) } }), Object.defineProperty(w.config, "disable", { get: function () { return w.config._disable }, set: function (e) { w.config._disable = he(e) } }); var r = "time" === i.mode; if (!i.dateFormat && (i.enableTime || r)) { var l = I.defaultConfig.dateFormat || a.dateFormat; o.dateFormat = i.noCalendar || r ? "H:i" + (i.enableSeconds ? ":S" : "") : l + " H:i" + (i.enableSeconds ? ":S" : "") } if (i.altInput && (i.enableTime || r) && !i.altFormat) { var s = I.defaultConfig.altFormat || a.altFormat; o.altFormat = i.noCalendar || r ? "h:i" + (i.enableSeconds ? ":S K" : " K") : s + " h:i" + (i.enableSeconds ? ":S" : "") + " K" } Object.defineProperty(w.config, "minDate", { get: function () { return w.config._minDate }, set: le("min") }), Object.defineProperty(w.config, "maxDate", { get: function () { return w.config._maxDate }, set: le("max") }); var d = function (e) { return function (n) { w.config["min" === e ? "_minTime" : "_maxTime"] = w.parseDate(n, "H:i:S") } }; Object.defineProperty(w.config, "minTime", { get: function () { return w.config._minTime }, set: d("min") }), Object.defineProperty(w.config, "maxTime", { get: function () { return w.config._maxTime }, set: d("max") }), "time" === i.mode && (w.config.noCalendar = !0, w.config.enableTime = !0); Object.assign(w.config, o, i); for (var u = 0; u < n.length; u++)w.config[n[u]] = !0 === w.config[n[u]] || "true" === w.config[n[u]]; t.filter((function (e) { return void 0 !== w.config[e] })).forEach((function (e) { w.config[e] = c(w.config[e] || []).map(T) })), w.isMobile = !w.config.disableMobile && !w.config.inline && "single" === w.config.mode && !w.config.disable.length && !w.config.enable && !w.config.weekNumbers && /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent); for (u = 0; u < w.config.plugins.length; u++) { var f = w.config.plugins[u](w) || {}; for (var m in f) t.indexOf(m) > -1 ? w.config[m] = c(f[m]).map(T).concat(w.config[m]) : void 0 === i[m] && (w.config[m] = f[m]) } i.altInputClass || (w.config.altInputClass = ce().className + " " + w.config.altInputClass); De("onParseConfig") }(), se(), function () { if (w.input = ce(), !w.input) return void w.config.errorHandler(new Error("Invalid input element specified")); w.input._type = w.input.type, w.input.type = "text", w.input.classList.add("flatpickr-input"), w._input = w.input, w.config.altInput && (w.altInput = d(w.input.nodeName, w.config.altInputClass), w._input = w.altInput, w.altInput.placeholder = w.input.placeholder, w.altInput.disabled = w.input.disabled, w.altInput.required = w.input.required, w.altInput.tabIndex = w.input.tabIndex, w.altInput.type = "text", w.input.setAttribute("type", "hidden"), !w.config.static && w.input.parentNode && w.input.parentNode.insertBefore(w.altInput, w.input.nextSibling)); w.config.allowInput || w._input.setAttribute("readonly", "readonly"); ve() }(), function () { w.selectedDates = [], w.now = w.parseDate(w.config.now) || new Date; var e = w.config.defaultDate || ("INPUT" !== w.input.nodeName && "TEXTAREA" !== w.input.nodeName || !w.input.placeholder || w.input.value !== w.input.placeholder ? w.input.value : null); e && pe(e, w.config.dateFormat); w._initialDate = w.selectedDates.length > 0 ? w.selectedDates[0] : w.config.minDate && w.config.minDate.getTime() > w.now.getTime() ? w.config.minDate : w.config.maxDate && w.config.maxDate.getTime() < w.now.getTime() ? w.config.maxDate : w.now, w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth(), w.selectedDates.length > 0 && (w.latestSelectedDateObj = w.selectedDates[0]); void 0 !== w.config.minTime && (w.config.minTime = w.parseDate(w.config.minTime, "H:i")); void 0 !== w.config.maxTime && (w.config.maxTime = w.parseDate(w.config.maxTime, "H:i")); w.minDateHasTime = !!w.config.minDate && (w.config.minDate.getHours() > 0 || w.config.minDate.getMinutes() > 0 || w.config.minDate.getSeconds() > 0), w.maxDateHasTime = !!w.config.maxDate && (w.config.maxDate.getHours() > 0 || w.config.maxDate.getMinutes() > 0 || w.config.maxDate.getSeconds() > 0) }(), w.utils = { getDaysInMonth: function (e, n) { return void 0 === e && (e = w.currentMonth), void 0 === n && (n = w.currentYear), 1 === e && (n % 4 == 0 && n % 100 != 0 || n % 400 == 0) ? 29 : w.l10n.daysInMonth[e] } }, w.isMobile || function () { var e = window.document.createDocumentFragment(); if (w.calendarContainer = d("div", "flatpickr-calendar"), w.calendarContainer.tabIndex = -1, !w.config.noCalendar) { if (e.appendChild((w.monthNav = d("div", "flatpickr-months"), w.yearElements = [], w.monthElements = [], w.prevMonthNav = d("span", "flatpickr-prev-month"), w.prevMonthNav.innerHTML = w.config.prevArrow, w.nextMonthNav = d("span", "flatpickr-next-month"), w.nextMonthNav.innerHTML = w.config.nextArrow, V(), Object.defineProperty(w, "_hidePrevMonthArrow", { get: function () { return w.__hidePrevMonthArrow }, set: function (e) { w.__hidePrevMonthArrow !== e && (s(w.prevMonthNav, "flatpickr-disabled", e), w.__hidePrevMonthArrow = e) } }), Object.defineProperty(w, "_hideNextMonthArrow", { get: function () { return w.__hideNextMonthArrow }, set: function (e) { w.__hideNextMonthArrow !== e && (s(w.nextMonthNav, "flatpickr-disabled", e), w.__hideNextMonthArrow = e) } }), w.currentYearElement = w.yearElements[0], Ce(), w.monthNav)), w.innerContainer = d("div", "flatpickr-innerContainer"), w.config.weekNumbers) { var n = function () { w.calendarContainer.classList.add("hasWeeks"); var e = d("div", "flatpickr-weekwrapper"); e.appendChild(d("span", "flatpickr-weekday", w.l10n.weekAbbreviation)); var n = d("div", "flatpickr-weeks"); return e.appendChild(n), { weekWrapper: e, weekNumbers: n } }(), t = n.weekWrapper, a = n.weekNumbers; w.innerContainer.appendChild(t), w.weekNumbers = a, w.weekWrapper = t } w.rContainer = d("div", "flatpickr-rContainer"), w.rContainer.appendChild(z()), w.daysContainer || (w.daysContainer = d("div", "flatpickr-days"), w.daysContainer.tabIndex = -1), U(), w.rContainer.appendChild(w.daysContainer), w.innerContainer.appendChild(w.rContainer), e.appendChild(w.innerContainer) } w.config.enableTime && e.appendChild(function () { w.calendarContainer.classList.add("hasTime"), w.config.noCalendar && w.calendarContainer.classList.add("noCalendar"); var e = E(w.config); w.timeContainer = d("div", "flatpickr-time"), w.timeContainer.tabIndex = -1; var n = d("span", "flatpickr-time-separator", ":"), t = m("flatpickr-hour", { "aria-label": w.l10n.hourAriaLabel }); w.hourElement = t.getElementsByTagName("input")[0]; var a = m("flatpickr-minute", { "aria-label": w.l10n.minuteAriaLabel }); w.minuteElement = a.getElementsByTagName("input")[0], w.hourElement.tabIndex = w.minuteElement.tabIndex = -1, w.hourElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getHours() : w.config.time_24hr ? e.hours : function (e) { switch (e % 24) { case 0: case 12: return 12; default: return e % 12 } }(e.hours)), w.minuteElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getMinutes() : e.minutes), w.hourElement.setAttribute("step", w.config.hourIncrement.toString()), w.minuteElement.setAttribute("step", w.config.minuteIncrement.toString()), w.hourElement.setAttribute("min", w.config.time_24hr ? "0" : "1"), w.hourElement.setAttribute("max", w.config.time_24hr ? "23" : "12"), w.hourElement.setAttribute("maxlength", "2"), w.minuteElement.setAttribute("min", "0"), w.minuteElement.setAttribute("max", "59"), w.minuteElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(t), w.timeContainer.appendChild(n), w.timeContainer.appendChild(a), w.config.time_24hr && w.timeContainer.classList.add("time24hr"); if (w.config.enableSeconds) { w.timeContainer.classList.add("hasSeconds"); var i = m("flatpickr-second"); w.secondElement = i.getElementsByTagName("input")[0], w.secondElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getSeconds() : e.seconds), w.secondElement.setAttribute("step", w.minuteElement.getAttribute("step")), w.secondElement.setAttribute("min", "0"), w.secondElement.setAttribute("max", "59"), w.secondElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(d("span", "flatpickr-time-separator", ":")), w.timeContainer.appendChild(i) } w.config.time_24hr || (w.amPM = d("span", "flatpickr-am-pm", w.l10n.amPM[r((w.latestSelectedDateObj ? w.hourElement.value : w.config.defaultHour) > 11)]), w.amPM.title = w.l10n.toggleTitle, w.amPM.tabIndex = -1, w.timeContainer.appendChild(w.amPM)); return w.timeContainer }()); s(w.calendarContainer, "rangeMode", "range" === w.config.mode), s(w.calendarContainer, "animate", !0 === w.config.animate), s(w.calendarContainer, "multiMonth", w.config.showMonths > 1), w.calendarContainer.appendChild(e); var i = void 0 !== w.config.appendTo && void 0 !== w.config.appendTo.nodeType; if ((w.config.inline || w.config.static) && (w.calendarContainer.classList.add(w.config.inline ? "inline" : "static"), w.config.inline && (!i && w.element.parentNode ? w.element.parentNode.insertBefore(w.calendarContainer, w._input.nextSibling) : void 0 !== w.config.appendTo && w.config.appendTo.appendChild(w.calendarContainer)), w.config.static)) { var l = d("div", "flatpickr-wrapper"); w.element.parentNode && w.element.parentNode.insertBefore(l, w.element), l.appendChild(w.element), w.altInput && l.appendChild(w.altInput), l.appendChild(w.calendarContainer) } w.config.static || w.config.inline || (void 0 !== w.config.appendTo ? w.config.appendTo : window.document.body).appendChild(w.calendarContainer) }(), function () { w.config.wrap && ["open", "close", "toggle", "clear"].forEach((function (e) { Array.prototype.forEach.call(w.element.querySelectorAll("[data-" + e + "]"), (function (n) { return P(n, "click", w[e]) })) })); if (w.isMobile) return void function () { var e = w.config.enableTime ? w.config.noCalendar ? "time" : "datetime-local" : "date"; w.mobileInput = d("input", w.input.className + " flatpickr-mobile"), w.mobileInput.tabIndex = 1, w.mobileInput.type = e, w.mobileInput.disabled = w.input.disabled, w.mobileInput.required = w.input.required, w.mobileInput.placeholder = w.input.placeholder, w.mobileFormatStr = "datetime-local" === e ? "Y-m-d\\TH:i:S" : "date" === e ? "Y-m-d" : "H:i:S", w.selectedDates.length > 0 && (w.mobileInput.defaultValue = w.mobileInput.value = w.formatDate(w.selectedDates[0], w.mobileFormatStr)); w.config.minDate && (w.mobileInput.min = w.formatDate(w.config.minDate, "Y-m-d")); w.config.maxDate && (w.mobileInput.max = w.formatDate(w.config.maxDate, "Y-m-d")); w.input.getAttribute("step") && (w.mobileInput.step = String(w.input.getAttribute("step"))); w.input.type = "hidden", void 0 !== w.altInput && (w.altInput.type = "hidden"); try { w.input.parentNode && w.input.parentNode.insertBefore(w.mobileInput, w.input.nextSibling) } catch (e) { } P(w.mobileInput, "change", (function (e) { w.setDate(g(e).value, !1, w.mobileFormatStr), De("onChange"), De("onClose") })) }(); var e = l(re, 50); w._debouncedChange = l(Y, 300), w.daysContainer && !/iPhone|iPad|iPod/i.test(navigator.userAgent) && P(w.daysContainer, "mouseover", (function (e) { "range" === w.config.mode && oe(g(e)) })); P(w._input, "keydown", ie), void 0 !== w.calendarContainer && P(w.calendarContainer, "keydown", ie); w.config.inline || w.config.static || P(window, "resize", e); void 0 !== window.ontouchstart ? P(window.document, "touchstart", X) : P(window.document, "mousedown", X); P(window.document, "focus", X, { capture: !0 }), !0 === w.config.clickOpens && (P(w._input, "focus", w.open), P(w._input, "click", w.open)); void 0 !== w.daysContainer && (P(w.monthNav, "click", xe), P(w.monthNav, ["keyup", "increment"], N), P(w.daysContainer, "click", me)); if (void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement) { var n = function (e) { return g(e).select() }; P(w.timeContainer, ["increment"], _), P(w.timeContainer, "blur", _, { capture: !0 }), P(w.timeContainer, "click", H), P([w.hourElement, w.minuteElement], ["focus", "click"], n), void 0 !== w.secondElement && P(w.secondElement, "focus", (function () { return w.secondElement && w.secondElement.select() })), void 0 !== w.amPM && P(w.amPM, "click", (function (e) { _(e) })) } w.config.allowInput && P(w._input, "blur", ae) }(), (w.selectedDates.length || w.config.noCalendar) && (w.config.enableTime && F(w.config.noCalendar ? w.latestSelectedDateObj : void 0), ye(!1)), S(); var n = /^((?!chrome|android).)*safari/i.test(navigator.userAgent); !w.isMobile && n && de(), De("onReady") }(), w } function T(e, n) { for (var t = Array.prototype.slice.call(e).filter((function (e) { return e instanceof HTMLElement })), a = [], i = 0; i < t.length; i++) { var o = t[i]; try { if (null !== o.getAttribute("data-fp-omit")) continue; void 0 !== o._flatpickr && (o._flatpickr.destroy(), o._flatpickr = void 0), o._flatpickr = k(o, n || {}), a.push(o._flatpickr) } catch (e) { console.error(e) } } return 1 === a.length ? a[0] : a } "undefined" != typeof HTMLElement && "undefined" != typeof HTMLCollection && "undefined" != typeof NodeList && (HTMLCollection.prototype.flatpickr = NodeList.prototype.flatpickr = function (e) { return T(this, e) }, HTMLElement.prototype.flatpickr = function (e) { return T([this], e) }); var I = function (e, n) { return "string" == typeof e ? T(window.document.querySelectorAll(e), n) : e instanceof Node ? T([e], n) : T(e, n) }; return I.defaultConfig = {}, I.l10ns = { en: e({}, i), default: e({}, i) }, I.localize = function (n) { I.l10ns.default = e(e({}, I.l10ns.default), n) }, I.setDefaults = function (n) { I.defaultConfig = e(e({}, I.defaultConfig), n) }, I.parseDate = C({}), I.formatDate = b({}), I.compareDates = M, "undefined" != typeof jQuery && void 0 !== jQuery.fn && (jQuery.fn.flatpickr = function (e) { return T(this, e) }), Date.prototype.fp_incr = function (e) { return new Date(this.getFullYear(), this.getMonth(), this.getDate() + ("string" == typeof e ? parseInt(e, 10) : e)) }, "undefined" != typeof window && (window.flatpickr = I), I }));

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

    function formatDelta(delta) {
        if (delta === null || delta === undefined || isNaN(toNumber(delta))) return "";
        var d = toNumber(delta);
        var arrow = d > 0 ? "↑" : (d < 0 ? "↓" : "→");
        var cls = d > 0 ? "dk-delta-up" : (d < 0 ? "dk-delta-down" : "dk-delta-flat");
        return '<span class="' + cls + '">' + arrow + " " + formatNumber(Math.abs(d), 1) + "%</span> so với kỳ trước";
    }

    function renderMetricCards() {
        function setText(id, val) { var el = byId(id); if (el) el.textContent = val; }
        function setHtml(id, val) { var el = byId(id); if (el) el.innerHTML = val; }

        if (state.kpiTonDauKy) setText("metricTonDauKy", formatNumber(toNumber(state.kpiTonDauKy.Value), 0));
        else setText("metricTonDauKy", "0");

        if (state.kpiTongNhap) {
            setText("metricTongNhap", formatNumber(toNumber(state.kpiTongNhap.Value), 0));
            setHtml("metricTongNhapDelta", formatDelta(state.kpiTongNhap.Delta));
        } else {
            setText("metricTongNhap", "0");
            setHtml("metricTongNhapDelta", "");
        }

        if (state.kpiTongXuat) {
            setText("metricTongXuat", formatNumber(toNumber(state.kpiTongXuat.Value), 0));
            setHtml("metricTongXuatDelta", formatDelta(state.kpiTongXuat.Delta));
        } else {
            setText("metricTongXuat", "0");
            setHtml("metricTongXuatDelta", "");
        }

        if (state.kpiTonKho) {
            setText("metricTonKho", formatNumber(toNumber(state.kpiTonKho.Value), 0));
            setHtml("metricTonKhoDelta", formatDelta(state.kpiTonKho.Delta));
        } else {
            setText("metricTonKho", "0");
            setHtml("metricTonKhoDelta", "");
        }
        setText("metricInboundReady", formatNumber(state.inbound.length, 0));
        setHtml("metricInboundReadyHint", state.inbound.length > 0 ? '<i class="fa-solid fa-fire dk-text-warn"></i> ' + state.inbound.length + " PO" : "");
        if (state.kpiPODangTre) {
            setText("metricPODangTre", formatNumber(toNumber(state.kpiPODangTre.SoPO), 0));
            setHtml("metricPODangTreHint", '<i class="fa-solid fa-fire dk-text-danger"></i> ' + toNumber(state.kpiPODangTre.SoPOChuaKiem) + " PO chưa kiểm");
        } else {
            setText("metricPODangTre", "0");
            setHtml("metricPODangTreHint", "");
        }

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

        var usedNpl = Math.max(0, toNumber(overall.UsedNPL));
        var usedPl = Math.max(0, toNumber(overall.UsedPL));
        var totalCapacity = Math.max(0, toNumber(overall.TotalCapacity));
        var capNpl = Math.max(0, toNumber(overall.CapacityNPL));
        var capPl = Math.max(0, toNumber(overall.CapacityPL));
        var totalUsed = usedNpl + usedPl;
        var segNpl, segPl, segFree, pctTotal;
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

        var _darkSeg = dkIsDark();
        var colNL = _darkSeg ? "#60a5fa" : "#0b5bf0";
        var colPL = _darkSeg ? "#fbbf24" : "#f5a623";
        var colFree = _darkSeg ? "#94a3b8" : "#9fb3c6";
        var segments = [
            { name: "Đã dùng NL", value: segNpl, color: colNL, label: formatNumber(segNpl, 2) + "%" },
            { name: "Đã dùng PL", value: segPl, color: colPL, label: formatNumber(segPl, 2) + "%" },
            { name: "Còn trống", value: segFree, color: colFree, label: formatNumber(segFree, 2) + "%" }
        ];
        var totalPct = segNpl + segPl + segFree;
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

        var filterActive = activeCustomerFilter !== "";

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
        var colors = dkIsDark()
            ? ["#60a5fa", "#34d399", "#fbbf24", "#a78bfa", "#22d3ee", "#f87171", "#f472b6", "#5eead4", "#fb923c", "#c4b5fd", "#6ee7b7"]
            : ["#1a3a6b", "#2f7fd9", "#f5a623", "#8d6be6", "#00a6c7", "#e34f70", "#718096", "#5ebf5e", "#d97706", "#7c3aed", "#059669"];
        var offset = 0;
        var startAngle = -Math.PI / 2;

        var segments = [];
        var labels = [];
        var defs = [];
        var anchors = [];

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
            var minGap = Math.min(MIN_GAP_DYN || 60, Math.floor((maxY - minY) / Math.max(items.length, 1)));
            minGap = Math.max(minGap, 22);

            for (var j = 0; j < items.length; j++) {
                var prevY = j > 0 ? items[j - 1].labelY : minY - minGap;
                items[j].labelY = Math.max(items[j].preferY, prevY + minGap);
            }
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
        var labelLines = [];
        var labelBoxes = [];

        for (var a = 0; a < anchors.length; a++) {
            var item = anchors[a];

            var edgeX = cx + Math.cos(item.angle) * (r + strokeWidth / 2 + 5);
            var edgeY = cy + Math.sin(item.angle) * (r + strokeWidth / 2 + 5);
            var radialX = cx + Math.cos(item.angle) * (r + strokeWidth / 2 + 35);
            var radialY = cy + Math.sin(item.angle) * (r + strokeWidth / 2 + 35);
            var elbowX = cx + item.side * (r + strokeWidth / 2 + 50);
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

            var lineStartX = cx + Math.cos(item.angle) * (r + strokeWidth / 2);
            var lineStartY = cy + Math.sin(item.angle) * (r + strokeWidth / 2);
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
            "<linearGradient id=\"gradStockArea\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#f97316\" stop-opacity=\"0.25\"/><stop offset=\"100%\" stop-color=\"#f97316\" stop-opacity=\"0\"/></linearGradient>" +
            "<filter id=\"bsf\" x=\"-20%\" y=\"-20%\" width=\"140%\" height=\"140%\"><feDropShadow dx=\"0\" dy=\"2\" stdDeviation=\"2\" flood-color=\"#0f172a\" flood-opacity=\"0.12\"/></filter>" +
            "</defs>";

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
            barsIn += "<rect class=\"dk-flow-bar dk-flow-bar-in\"  style=\"transform-origin:" + (gx - barW - gap / 2 + barW / 2) + "px " + axisY + "px;animation-delay:" + (bi * 25) + "ms\" x=\"" + (gx - barW - gap / 2) + "\" y=\"" + yin + "\" width=\"" + barW + "\" height=\"" + hin + "\" rx=\"3\" fill=\"url(#gradIn)\"  filter=\"url(#bsf)\"/>";
            barsOut += "<rect class=\"dk-flow-bar dk-flow-bar-out\" style=\"transform-origin:" + (gx + gap / 2 + barW / 2) + "px " + axisY + "px;animation-delay:" + (bi * 25 + 80) + "ms\" x=\"" + (gx + gap / 2) + "\" y=\"" + yout + "\" width=\"" + barW + "\" height=\"" + hout + "\" rx=\"3\" fill=\"url(#gradOut)\" filter=\"url(#bsf)\"/>";
        }

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
    function renderTop5MinChart() { }

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

    function populateCustomerFilter() {
        var sel = byId("customerFilterSelect");
        if (!sel) return;
        var prev = activeCustomerFilter;
        var opts = "<option value=\"\"" + (prev === "" ? " selected" : "") + ">Tất cả khách hàng</option>";
        var rows = buildCustomerDetailRows();
        for (var ci = 0; ci < rows.length; ci++) {
            var r = rows[ci];
            var v = r.MaKH || "";
            if (!v) continue;
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

        var dayLabelHtml = "<div class=\"dk-cal-day-labels\">";
        for (var dl = 0; dl < DAY_LABELS.length; dl++) {
            dayLabelHtml += "<div class=\"dk-cal-day-label\">" + DAY_LABELS[dl] + "</div>";
        }
        dayLabelHtml += "</div>";

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

        if (!calMonthDate) {
            var now = new Date();
            calMonthDate = new Date(now.getFullYear(), now.getMonth(), 1);
        }

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

        var gridStart = new Date(dispFrom);
        var dow = (gridStart.getDay() + 6) % 7; // 0=Mon
        gridStart.setDate(gridStart.getDate() - dow);

        var gridEnd = new Date(dispTo);
        var dow2 = (gridEnd.getDay() + 6) % 7;
        gridEnd.setDate(gridEnd.getDate() + (6 - dow2));

        var today = new Date(); today.setHours(0, 0, 0, 0);
        var DAY_NAMES = ["Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy", "Chủ Nhật"];

        var html = "";

        html += "<div class=\"dk-cal-monthly-header\">";
        for (var dh = 0; dh < 7; dh++) {
            html += "<div class=\"dk-cal-monthly-dow\">" + DAY_NAMES[dh] + "</div>";
        }
        html += "</div>";

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

        var cells = node.querySelectorAll(".dk-cal-monthly-cell[data-date]");
        for (var ci = 0; ci < cells.length; ci++) {
            (function (cell) {
                var dt = cell.getAttribute("data-date");
                if (!dt) return;
                var cellInfo = dayMap[dt] || { totalIn: 0, totalOut: 0, totalKK: 0, total: 0 };
                var plCount = plannedDates[dt] || 0;

                cell.style.cursor = "pointer";
                cell.addEventListener("click", function () {
                    showCalDayDetail(dt, cellInfo, plCount);

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

            var nkSrc = (state.nkDuKien && state.nkDuKien.length > 0) ? state.nkDuKien : (state.inbound || []);
            var plannedRows = nkSrc.filter(function (r) {
                var dt = parseDate(r.NgayNKDuKien);
                if (!dt) return false;
                return dt >= fromD && dt <= toD;
            });

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

        var cards = container.querySelectorAll(".dk-gs-card.clickable");
        for (var c = 0; c < cards.length; c++) {
            cards[c].addEventListener("click", function () {
                var page = this.getAttribute("data-page");
                var go = this.getAttribute("data-goto");
                if (page) {
                    var nav = document.querySelector(".dk-page-btn[data-page=\"" + page + "\"]");
                    if (nav) nav.click();
                }
                if (go) {
                    setTimeout(function () {
                        var trigger = document.querySelector(".js-open-detail[data-detail=\"" + go + "\"]");
                        if (trigger) trigger.click();
                    }, 250);
                }
            });
        }
    }

    function showCalDayDetail(dateKey, info, plannedCount) {
        var modalTitle = byId(ids.detailModalTitle);
        var modalMeta = byId(ids.detailModalMeta);
        var modalContent = byId(ids.detailModalContent);
        var modal = byId(ids.detailModal);
        if (!modal) return;

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

        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "";
        var searchInputEl = byId("detailSearchInput");
        if (searchInputEl) {
            searchInputEl.value = "";
            searchInputEl.placeholder = "Tìm khách hàng, mã NPL, số lô, barcode...";
        }
        var searchCountEl = byId("detailSearchCount");
        if (searchCountEl) searchCountEl.textContent = "";

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

            var hasSoLuong = false;
            var dayTotal = 0;
            for (var ck = 0; ck < cols.length; ck++) if (cols[ck].key === "SoLuong" || cols[ck].key === "SoLuongDuKien") hasSoLuong = true;
            if (hasSoLuong) {
                for (var ri = 0; ri < groupRows.length; ri++) {
                    var q = groupRows[ri].SoLuong || groupRows[ri].SoLuongDuKien || 0;
                    dayTotal += parseFloat(q) || 0;
                }
            }

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

    function loadLpcpStatsForMonth(baseDate) {
        renderLpcpBottomCharts();
    }

    function renderLpcpStatsPage3() {
    }

    function bindLpcpStatsClick() {
    }

    function renderLpcpBottomCharts() {
        var isDark = document.body.classList.contains("dark-theme");
        var textColor = isDark ? "#f8fafc" : "#1e293b"; // Đậm hơn ở light mode, sáng hơn ở dark mode
        var gridColor = isDark ? "rgba(255,255,255,0.05)" : "#e2e8f0";
        var bgColor = isDark ? "#1e293b" : "#ffffff";

        function _asIso(d) { return d.getFullYear() + "-" + String(d.getMonth() + 1).padStart(2, "0") + "-" + String(d.getDate()).padStart(2, "0"); }

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

        var now = new Date();
        var trendDays = document.getElementById("trendFilterSelect") ? parseInt(document.getElementById("trendFilterSelect").value) : 30;
        var loadDays = document.getElementById("loadFilterSelect") ? parseInt(document.getElementById("loadFilterSelect").value) : 30;

        function buildRange(nDays) {
            var to = new Date(now); to.setDate(to.getDate() - 1);
            var from = new Date(now); from.setDate(from.getDate() - nDays);
            return { from: _asIso(from), to: _asIso(to) };
        }
        function doFetch(url) {
            return typeof requestJson === 'function' ? requestJson(url) : fetch(url).then(function (r) { return r.json(); });
        }

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

    function showLpcpInlineDetail(dateKey) {
        var dateEl = document.getElementById("lpcpDetailDate");
        var badgeEl = document.getElementById("lpcpDetailBadge");
        var assignEl = document.getElementById("lpcpDetailAssignments");
        var pickEl = document.getElementById("lpcpDetailPickOrders");
        var warnEl = document.getElementById("lpcpDetailWarnings");

        var parts = String(dateKey).split("-");
        var DOW_NAMES = ["Ch\u1ee7 Nh\u1eadt", "Th\u1ee9 Hai", "Th\u1ee9 Ba", "Th\u1ee9 T\u01b0", "Th\u1ee9 N\u0103m", "Th\u1ee9 S\u00e1u", "Th\u1ee9 B\u1ea3y"];
        var displayDate = dateKey;
        if (parts.length === 3) {
            var dow = DOW_NAMES[new Date(dateKey + "T00:00:00").getDay()] || "";
            displayDate = dow + ", " + parts[2] + "/" + parts[1] + "/" + parts[0];
        }

        var cells = document.querySelectorAll(".dk-cal-monthly-cell");
        for (var ci = 0; ci < cells.length; ci++) {
            cells[ci].classList.remove("is-selected");
            if (cells[ci].getAttribute("data-date") === dateKey) {
                cells[ci].classList.add("is-selected");
            }
        }

        if (window.dkOpenSidebar) {
            window.dkOpenSidebar();

            setTimeout(function () {
                var calPanel = document.querySelector('.dk-cal-monthly-panel');
                var sidePanel = document.getElementById('lpcpDetailPanel');
                if (calPanel && sidePanel) {
                    sidePanel.style.maxHeight = calPanel.offsetHeight + 'px';
                }
            }, 50);
        }

        if (dateEl) dateEl.textContent = displayDate;
        if (badgeEl) badgeEl.textContent = "\u0110ang t\u1ea3i...";

        var SPIN = "<div class='dk-lpcp-placeholder'><svg width='14' height='14' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'><circle cx='12' cy='12' r='10'/><polyline points='12 6 12 12 16 14'/></svg>\u0110ang t\u1ea3i...</div>";
        if (assignEl) assignEl.innerHTML = SPIN;
        if (pickEl) pickEl.innerHTML = SPIN;
        if (warnEl) warnEl.innerHTML = SPIN;

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

                setBadge("lpcpWarnCount", warnings.length);
                setBadge("lpcpTaskCount", assignments.length);
                setBadge("lpcpPickCount", pickOrders.length);
                if (badgeEl) badgeEl.textContent = assignments.length + " task \u00b7 " + pickOrders.length + " l\u1ec7nh so\u1ea1n";

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

                (function () {
                    var _warnings = warnings;
                    var _assignments = assignments;
                    var _pickOrders = pickOrders;
                    var _displayDate = displayDate;

                    function bindDetailBtn(btnId, section) {
                        var btn = document.getElementById(btnId);
                        if (!btn) return;
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

    function openLpcpSectionModal(section, warnings, assignments, pickOrders, dateLabel) {
        var modal = document.getElementById("detailModal");
        var titleEl = document.getElementById("detailModalTitle");
        var metaEl = document.getElementById("detailModalMeta");
        var contentEl = document.getElementById("detailModalContent");
        var headEl = modal ? modal.querySelector(".dk-modal-head") : null;
        if (!modal || !contentEl) return;

        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "none";

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

        var html = "";

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

        var panel = document.createElement("div");
        panel.className = "dk-day-panel";
        panel.setAttribute("data-tabkey", "lpcp");
        panel.style.display = "none";
        panel.innerHTML = html;
        container.appendChild(panel);

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
            { key: "STT", label: "STT", number: 0, center: true, width: "4%" },
            { key: "MaLenh", label: "Mã lệnh", center: true, width: "13%" },
            { key: "TenHang", label: "Tên hàng", width: "28%" },
            { key: "TenKH", label: "Khách hàng", width: "22%" },
            { key: "SoLuong", label: "Số lượng", number: 2, center: true, width: "10%" },
            { key: "NgayXuat", label: "Ngày xuất", date: true, center: true, width: "10%" },
            { key: "LoaiKho", label: "Loại kho", center: true, width: "8%" },
            { key: "GiaTri", label: "Giá trị", number: 0, width: "10%" }
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
        renderActivityCalendar();
        renderActivityCalendarMonthly();
        setTimeout(injectMaximizeButtons, 80);

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
                var dayA = (a.item.TenDay || "").toLowerCase();
                var dayB = (b.item.TenDay || "").toLowerCase();
                if (dayA !== dayB) return dayA < dayB ? -1 : 1;
                var capA = toNumber(a.item.TongCBMTrongKe);
                var capB = toNumber(b.item.TongCBMTrongKe);
                var pA = capA > 0 ? (toNumber(a.item.TongCBMSuDungTrongKe) / capA) * 100 : 0;
                var pB = capB > 0 ? (toNumber(b.item.TongCBMSuDungTrongKe) / capB) * 100 : 0;
                return pB - pA;
            });

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

            var dayGroups = {};
            var dayOrder = [];
            for (var j2 = 0; j2 < racks.length; j2++) {
                var rk = racks[j2];
                var dayName = rk.item.TenDay || "Không xác định";
                if (!dayGroups[dayName]) { dayGroups[dayName] = []; dayOrder.push(dayName); }
                dayGroups[dayName].push(rk);
            }
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

    var currentPage = 1;
    var autoRotateTimer = null;
    var AUTO_ROTATE_INTERVAL = 15000; // 15 giây

    function switchPage(pageNum) {
        currentPage = pageNum;
        try { loadPageData(pageNum); } catch (e) { }
        var pages = document.querySelectorAll(".dk-page");
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
            hieuSuatHoatDongAll: { title: "Hiệu suất hoạt động", rows: [], columns: [], customAsync: "hieuSuatHoatDongAll" },
            todoDetail: { title: "Công việc chờ xử lý", rows: [], columns: [], customAsync: "todoDetail" },
            top5VTAll: { title: "Toàn bộ vật tư theo dung tích", rows: [], columns: [], customAsync: "top5VTAll" },
            top5KHAll: { title: "Toàn bộ khách hàng theo giá trị tồn", rows: [], columns: [], customAsync: "top5KHAll" },
            hetHanAll: { title: "Vật tư sắp hết hạn", rows: [], columns: [], customAsync: "hetHanAll" },
            giaTriNhomAll: { title: "Chi tiết giá trị tồn theo nhóm", rows: [], columns: [], customAsync: "giaTriNhomAll" },
            kiemKeAll: { title: "Chi tiết kiểm kê", rows: [], columns: [], customAsync: "kiemKeAll" },
            tonDauKyDetail: { title: "Tồn đầu kỳ", rows: [], columns: [], customAsync: "tonDauKyDetail" },
            tongNhapDetail: { title: "Chi tiết nhập kho", rows: [], columns: [], customAsync: "tongNhapDetail" },
            tongXuatDetail: { title: "Chi tiết xuất kho", rows: [], columns: [], customAsync: "tongXuatDetail" },
            tonKhoDetail: { title: "Chi tiết tồn kho", rows: [], columns: [], customAsync: "tonKhoDetail" },
            poTreDetail: { title: "PO đang trễ", rows: [], columns: [], customAsync: "poTreDetail" },
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

        var TEXT_LEFT_KEYS = { TenKH: 1, KhachHang: 1, TenKe: 1, TenDay: 1, TenHang: 1, MaVT: 1, ChiTiet: 1, MaGop: 1, MaDH: 1, MaHang: 1, MaLenh: 1, MaDVSX: 1, MaNPL: 1, DanhSachKH: 1, DanhSachMaNPL: 1, ChiSo: 1, ItemCode: 1, TenVT: 1, MaPhieu: 1 };
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

    var _detailStack = [];
    function openDetail(detail, index) {
        var modal = byId(ids.detailModal);
        if (modal && modal.classList.contains("open") && _detailStack.length === 0) {
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
        try { model = getDetailData(detail, index); } catch (e) { model = { title: detail, rows: [], columns: [], isWarehouseMap: false, customAsync: detail }; }
        var titleEl = byId(ids.detailModalTitle);
        if (_detailStack.length > 0) {
            titleEl.innerHTML = "<button type=\"button\" class=\"dk-back-btn js-modal-back\" title=\"Quay lại\">← Quay lại</button> " + escapeHtml(model.title);
        } else {
            titleEl.textContent = model.title;
        }
        byId(ids.detailModalMeta).textContent = model.meta || "";
        if (!model.meta) byId(ids.detailModalMeta).style.display = "none";
        else byId(ids.detailModalMeta).style.display = "";
        var rowCountEl = byId("detailModalRowCount");
        if (rowCountEl) {
            rowCountEl.textContent = "Tổng số dòng: " + formatNumber((model.rows || []).length, 0);
        }

        var searchInput = byId("detailSearchInput");
        if (searchInput) { searchInput.value = ""; }
        var searchCount = byId("detailSearchCount");
        if (searchCount) { searchCount.textContent = ""; }

        var rowCount = (model.rows || []).length;
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

            wrapHtml += "<div style=\"margin-bottom:14px\">";
            wrapHtml += "<b style=\"font-size:13px\">Sơ đồ lấp đầy kệ</b>";
            wrapHtml += "<div style=\"display:flex;gap:10px;font-size:11px;font-weight:700;margin:6px 0;\">";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#dcfce7;border:1px solid #86efac;border-radius:2px;vertical-align:middle\"></i> &lt;50%</span>";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#fef9c3;border:1px solid #fde047;border-radius:2px;vertical-align:middle\"></i> 50-85%</span>";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#fecdd3;border:1px solid #fda4af;border-radius:2px;vertical-align:middle\"></i> 85-100%</span>";
            wrapHtml += "<span><i style=\"display:inline-block;width:12px;height:12px;background:#e11d48;border:1px solid #be123c;border-radius:2px;vertical-align:middle\"></i> &gt;100%</span>";
            wrapHtml += "</div>";

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

            wrapHtml += renderDetailTable(model.columns, model.rows);
            wrapHtml += "</div>";

            byId(ids.detailModalContent).innerHTML = wrapHtml;
        } else {
            byId(ids.detailModalContent).innerHTML = renderDetailTable(model.columns, model.rows);
        }

        byId(ids.detailModal).classList.add("open");

        if (model.isWarehouseMap) {
            loadRackSlotDetailIntoModal();
        }

        if (model.customAsync === "allMaterials") {
            var contentEl = byId(ids.detailModalContent);
            if (contentEl) contentEl.innerHTML = "<div class=\"dk-empty\" style=\"padding:30px\">Đang tải tất cả mã vật tư trong kho (có thể mất 5-15 giây)...</div>";
            requestJson("/api/DashboardKhoDesktop/GetAllMaterialsInStock").then(function (data) {
                var rows = normalizeArray(data).map(function (r, i) {
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
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            }).catch(function (err) {
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

        if (model.customAsync === "hieuSuatHoatDongAll") { renderHieuSuatHoatDongAllModal(); }
        if (model.customAsync === "todoDetail") { renderTodoDetailModal(); }
        if (model.customAsync === "top5VTAll") { renderTop5VTAllModal(); }
        if (model.customAsync === "top5KHAll") { renderTop5KHAllModal(); }
        if (model.customAsync === "hetHanAll") { renderHetHanAllModal(); }
        if (model.customAsync === "giaTriNhomAll") { renderGiaTriNhomAllModal(); }
        if (model.customAsync === "kiemKeAll") { renderKiemKeAllModal(); }
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
                { key: "LoaiKho", label: "Loại", center: true, width: 46 },
                { key: "TenKe", label: "Kệ", center: true, width: 80 },
                { key: "TenO", label: "Ô", center: true, width: 70 },
                { key: "SoKhachHang", label: "Số KH", number: 0, center: true, width: 65 },
                { key: "DanhSachKH", label: "Khách hàng", width: 160 },
                { key: "DanhSachMaNPL", label: "Mã NPL" },
                { key: "SoBarCode", label: "Số barcode", number: 0, center: true, width: 80 },
                { key: "TongCBM", label: "Tổng CBM", number: 4, center: true, width: 90 }
            ];
            body.innerHTML = renderDetailTable(cols, rows);
        }).catch(function () {
            body.innerHTML = "<div class=\"dk-empty dk-text-danger\" style=\"padding:20px\">Lỗi tải dữ liệu — cần rebuild backend</div>";
        });
    }

    function loadRackSlotDetailIntoModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;

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

            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 46 },
                { key: "TenKe", label: "Kệ", center: true, width: 80 },
                { key: "TenO", label: "Ô", center: true, width: 70 },
                { key: "SoKhachHang", label: "Số KH", number: 0, center: true, width: 60 },
                { key: "DanhSachKH", label: "Khách hàng", width: 160 },
                { key: "MauVT", label: "Màu", center: true, width: 90 },
                { key: "WidthSize", label: "Width/Size", center: true, width: 110 },
                { key: "SoBarCode", label: "Số BC", number: 0, center: true, width: 60 },
                { key: "TongCBM", label: "CBM", number: 4, center: true, width: 80 }
            ];

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

        _detailStack = [];
        _currentDetail = null;
        _currentDetailIndex = -1;

        var searchBar = document.querySelector(".dk-modal-search-bar");
        if (searchBar) searchBar.style.display = "";
    }

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
                if (event.preventDefault) event.preventDefault();
                var detail = detailNode.getAttribute("data-detail");
                var index = parseInt(detailNode.getAttribute("data-index"), 10);
                var todoType = detailNode.getAttribute("data-todo-type");
                window.__dkPendingTodoType = todoType || null;
                var alertCode = detailNode.getAttribute("data-alert-code");
                var alertName = detailNode.getAttribute("data-alert-name");
                window.__dkPendingAlertCode = alertCode || null;
                window.__dkPendingAlertName = alertName || null;
                openDetail(detail, isNaN(index) ? -1 : index);
                return;
            }

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
                    if (parent.savedHtml != null) {
                        var titleEl2 = byId(ids.detailModalTitle);
                        if (titleEl2) {
                            if (_detailStack.length > 0) {
                                titleEl2.innerHTML = "<button type=\"button\" class=\"dk-back-btn js-modal-back\" title=\"Quay lại\">← Quay lại</button> " + escapeHtml(parent.savedTitle || parent.detail);
                            } else {
                                titleEl2.textContent = parent.savedTitle || parent.detail;
                            }
                        }
                        var contentEl2 = byId(ids.detailModalContent);
                        if (contentEl2) contentEl2.innerHTML = parent.savedHtml;
                        var rcEl2 = byId("detailModalRowCount");
                        if (rcEl2 && parent.savedRowCount != null) rcEl2.textContent = parent.savedRowCount;
                        var metaEl2 = byId(ids.detailModalMeta);
                        if (metaEl2) {
                            metaEl2.textContent = parent.savedMeta || "";
                            metaEl2.style.display = parent.savedMeta ? "" : "none";
                        }
                        var sb2 = document.querySelector(".dk-modal-search-bar");
                        if (sb2) sb2.style.display = parent.savedSearchBar ? "" : "none";
                        if (_currentDetail === "top5KHAll") {
                            rewireTop5KHRowClicks();
                        }
                    } else {
                        renderDetailModal(parent.detail, parent.index);
                    }
                } else {
                    closeDetailModal();
                }
                return;
            }

            var closeNode = findCloseNode(target);
            if (closeNode) {
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
                var fsPanel = document.querySelector(".dk-panel-fullscreen");
                if (fsPanel) { togglePanelFullscreen(fsPanel); return; }
                closeDetailModal();
            }
        });

        var custFilter = byId("customerFilterSelect");
        if (custFilter) {
            custFilter.addEventListener("change", function () {
                activeCustomerFilter = this.value;
                renderCustomerPieChart();
                renderCustomersTable();
            });
        }

        var actWeekSel = byId("activityWeeksSelect");
        if (actWeekSel) {
            actWeekSel.addEventListener("change", function () {
                activityWeeksCount = parseInt(this.value, 10) || 13;
                renderActivityCalendar();
            });
        }

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

        function loadLpcpStatsForMonth(baseDate) {
            renderLpcpBottomCharts();
        }

        function reloadCalendarForMonth() {
            var calNode = byId("chartActivityCalendarMonthly");
            if (calNode) calNode.innerHTML = "<div class=\"dk-skeleton\"><div class=\"dk-skeleton-shimmer\"></div></div>";

            if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
            var from = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() - 1, 1);
            var to = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 2, 0);
            var url = "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" +
                asIsoDate(from) + "&denNgay=" + asIsoDate(to);

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

        var calOverview = byId("calOpenOverview");
        if (calOverview) {
            calOverview.addEventListener("click", function () {
                openCalendarOverviewModal();
            });
        }

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
                state.calActFilter[act] = !state.calActFilter[act];
                btn.classList.toggle("is-active", state.calActFilter[act]);
                renderActivityCalendarMonthly();
            });
        }

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

        var visiblePanels = content.querySelectorAll(".dk-day-panel");
        var tbodies;
        if (visiblePanels.length > 0) {
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

    function loadData(skipLoadingState, skipReloadCurrent) {
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
        var dfFrom = state.dateFilter && state.dateFilter.from ? state.dateFilter.from : "";
        var dfTo = state.dateFilter && state.dateFilter.to ? state.dateFilter.to : "";
        var dfQS = (dfFrom && dfTo) ? "?tuNgay=" + encodeURIComponent(dfFrom) + "&denNgay=" + encodeURIComponent(dfTo) : "";

        var lpcpFrom = new Date(now.getFullYear(), now.getMonth() - 1, 1);
        var lpcpTo = new Date(now.getFullYear(), now.getMonth() + 2, 0);
        function _isoDate(d) { return d.getFullYear() + "-" + String(d.getMonth() + 1).padStart(2, "0") + "-" + String(d.getDate()).padStart(2, "0"); }
        var LPCP_URL = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" + encodeURIComponent(_isoDate(lpcpFrom)) + "&denNgay=" + encodeURIComponent(_isoDate(lpcpTo));

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
        var p1x = safeJson(BASE + "GetCanhBaoTonKho").then(function (r) {
            state.alerts = normalizeArray(r); renderAlertsList();
        });
        var p1y = safeJson(BASE + "GetHieuSuatHoatDong" + dfQS).then(function (r) {
            state.hieuSuat = normalizeArray(r); renderHieuSuatGauges();
        });
        var pLPCP = requestJson(LPCP_URL).then(function (res) {
            var lpcpArr = normalizeArray(res.Tasks || res.data || res);
            var invArr = normalizeArray(res.Inventory || []);

            lpcpArr.forEach(function (d) {
                var k = String(d.NgayLam || "").substring(0, 10);
                if (k) state.lpcpCalendar[k] = d;
            });

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

            renderActivityCalendarMonthly();

            var today = new Date();
            if (typeof showLpcpInlineDetail === 'function') {
                showLpcpInlineDetail(_isoDate(today));
            }

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
            renderTop5MaxChart();
            renderTop5MinChart();
            if (!state.alerts || state.alerts.length === 0) {
                promises.push(safeJson(BASE + "GetCanhBaoTonKho").then(function (r) {
                    state.alerts = normalizeArray(r);
                    renderAlertsList();
                }));
            } else {
                renderAlertsList();
            }
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
            var baseMonth = (typeof calMonthDate !== "undefined" && calMonthDate) ? calMonthDate : new Date();
            var from = new Date(baseMonth.getFullYear(), baseMonth.getMonth() - 1, 1);
            var to = new Date(baseMonth.getFullYear(), baseMonth.getMonth() + 2, 0);

            var urlLichGoc = BASE + "GetActivityCalendar?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
            var urlNKDK = BASE + "GetNKDuKienByRange?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
            var urlLPCP = "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);

            state.nkDuKien = [];
            state.activityCalendar = [];
            state.lpcpCalendar = {};

            return Promise.all([
                safeJson(urlNKDK),
                safeJson(urlLichGoc),
                requestJson(urlLPCP).catch(function (e) {
                    console.warn("API LPCP lỗi, bỏ qua hiển thị:", e);
                    return [];
                })
            ]).then(function (results) {
                state.nkDuKien = normalizeArray(results[0]);

                var rLPCP = results[2] || {};
                var lpcpArr = normalizeArray(rLPCP.Tasks || rLPCP.data || rLPCP);
                var invArr = normalizeArray(rLPCP.Inventory || []);

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
                setTimeout(injectMaximizeButtons, 60);

                loadLpcpStatsForMonth(calMonthDate || now);

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

        var groupStep = N > 1 ? (plotW / (N - 1)) : plotW;
        var useBars = N <= 45;
        var barW = useBars ? Math.max(2, Math.min(20, groupStep * 0.32)) : 0;
        var gap = useBars ? 2 : 0;

        var defs = "<defs>" +
            "<linearGradient id=\"gradIn2\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#74c7f8\"/><stop offset=\"100%\" stop-color=\"#3d9de8\"/></linearGradient>" +
            "<linearGradient id=\"gradOut2\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\"><stop offset=\"0%\" stop-color=\"#c4b5fd\"/><stop offset=\"100%\" stop-color=\"#8b5cf6\"/></linearGradient>" +
            "<filter id=\"bsf2\" x=\"-20%\" y=\"-20%\" width=\"140%\" height=\"140%\"><feDropShadow dx=\"0\" dy=\"2\" stdDeviation=\"2\" flood-color=\"#0f172a\" flood-opacity=\"0.12\"/></filter>" +
            "</defs>";

        var grid = "";
        for (var g = 0; g <= 4; g++) {
            var gv = (maxBars / 4) * g;
            var gy = yScaleBars(gv);
            grid += "<line x1=\"" + L + "\" y1=\"" + gy + "\" x2=\"" + (W - R) + "\" y2=\"" + gy + "\" stroke=\"#e2eaf4\" stroke-dasharray=\"4 3\" stroke-width=\"1\"/>";
            grid += "<text x=\"" + (L - 6) + "\" y=\"" + (gy + 4) + "\" text-anchor=\"end\" class=\"dk-flow-label\" style=\"font-size:10px;fill:#3d9de8;font-weight:700\">" + formatNumber(gv, 0) + "</text>";
        }
        for (var gs = 0; gs <= 4; gs++) {
            var gvS = (maxStock / 4) * gs;
            var gyS = yScaleStock(gvS);
            grid += "<text x=\"" + (W - R + 6) + "\" y=\"" + (gyS + 4) + "\" text-anchor=\"start\" class=\"dk-flow-label\" style=\"font-size:10px;fill:#f97316;font-weight:700\">" + formatNumber(gvS, 0) + "</text>";
        }
        grid += "<line x1=\"" + L + "\" y1=\"" + axisY + "\" x2=\"" + (W - R) + "\" y2=\"" + axisY + "\" stroke=\"#b0c4d8\" stroke-width=\"1.5\"/>";
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
            if (!isFinite(pct) || pct <= 0) return { bg: bg, fill: null };
            var ratio = Math.max(0, Math.min(pct / 100, 0.9999));   // tránh ratio = 1 → arc endpoint trùng start
            var theta = Math.PI * ratio;
            var ex = cx - R * Math.cos(theta);
            var ey = cy - R * Math.sin(theta);
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
            var _darkG = dkIsDark();
            var trackColor = _darkG ? "#1e3a6b" : "#e8eef7";
            var subTxt = _darkG ? "#cbd5e1" : "#64748b";
            var tagOpac = _darkG ? "0.28" : "0.14";
            var html = "";
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
            html += "<path d=\"" + arc.bg + "\" fill=\"none\" stroke=\"" + trackColor + "\" stroke-width=\"" + strokeW + "\" stroke-linecap=\"round\"/>";
            if (arc.fill) {
                html += "<path class=\"dk-gauge-fill\" d=\"" + arc.fill + "\" fill=\"none\" " +
                    "stroke=\"url(#" + gradId + ")\" stroke-width=\"" + strokeW + "\" stroke-linecap=\"round\" " +
                    "pathLength=\"1000\" stroke-dasharray=\"1000\" filter=\"url(#" + gradId + "_glow)\"/>";
            }
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

    function renderTodoGroupedTable(cols, rows, isTraHang) {
        if (!rows || rows.length === 0) {
            return "<div class=\"dk-empty\" style=\"padding:20px\">Không có dữ liệu</div>";
        }
        var groups = {};
        var poOrder = [];
        for (var i = 0; i < rows.length; i++) {
            var po = rows[i].POMua || "(Không có PO)";
            if (!groups[po]) { groups[po] = []; poOrder.push(po); }
            groups[po].push(rows[i]);
        }
        var headHtml = "";
        for (var c = 0; c < cols.length; c++) {
            var col = cols[c];
            var styles = [];
            if (col.center) styles.push("text-align:center");
            else if (col.number !== undefined || col.percent) styles.push("text-align:right");
            if (col.width) styles.push("width:" + col.width + "px");
            headHtml += "<th" + (styles.length ? ' style="' + styles.join(";") + '"' : "") + ">" + escapeHtml(col.label) + "</th>";
        }
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
                "<td class=\"dk-table-truncate\" title=\"" + escapeHtml(r.TenVT || "") + "\">" + escapeHtml(r.TenVT || "") + "</td>" +
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
                "<td class=\"dk-table-truncate\" title=\"" + escapeHtml(r.KhachHang || "") + "\">" + escapeHtml(r.KhachHang || "") + "</td>" +
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
            var maVT = r.MaVT || "";
            var tenVT = r.TenVT || "";
            return "<tr>" +
                "<td class=\"dk-cell-mono\">" + (maVT ? escapeHtml(maVT) : "<span style='color:#aaa'>—</span>") + "</td>" +
                "<td class=\"dk-table-truncate\" title=\"" + escapeHtml(tenVT) + "\">" + (tenVT ? escapeHtml(tenVT) : "<span style='color:#aaa;font-style:italic'>Chưa có tên</span>") + "</td>" +
                "<td class=\"text-center " + dateCls + "\">" + dateStr + "<br><small class='dk-text-muted'>" + (dt ? daysLeft + " ngày" : "") + "</small></td>" +
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
        animateCountUp(node);
    }

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
        var colClass = items.length >= 4 ? " dk-modal-summary-strip--4col" :
            (items.length === 3 ? " dk-modal-summary-strip--3col" : "");
        var html = '<div class="dk-modal-summary-strip' + colClass + '">';
        for (var i = 0; i < items.length; i++) {
            var it = items[i];
            var kindCls = " dk-sum-" + (it.kind || "neutral");
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
                body.innerHTML = searchBar + '<div id="todoTableWrap">' + renderTodoGroupedTable(cols, rowsView, false) + '</div>';
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
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
                { key: "STT", label: "#", number: 0, center: true, width: 50 },
                { key: "MaVT", label: "Mã VT", drillTo: "matCountDrill_codes", width: 150 },
                { key: "TenVT", label: "Tên vật tư", width: 280 },
                { key: "LoaiKho", label: "Loại", center: true, width: 80 },
                { key: "MaMauVT", label: "Mã màu VT", width: 100 },
                { key: "MauVT", label: "Màu VT", width: 120 },
                { key: "WidthSize", label: "Width/Size", width: 100 },
                { key: "DonVi", label: "ĐV", center: true, width: 60 },
                { key: "SoRoll", label: "Số roll", number: 0, sortable: true, width: 80 },
                { key: "SLTon", label: "SL tồn", number: 1, sortable: true, width: 100 },
                { key: "ThanhTien", label: "Thành tiền", number: 0, sortable: true, width: 120 },
                { key: "CBM", label: "CBM", number: 4, sortable: true, width: 100 },
                { key: "TyTrong", label: "Tỷ trọng", percent: true, sortable: true, width: 90 },
                { key: "ViTriKe", label: "Vị trí kệ", center: true, width: 110 }
            ];
            content.innerHTML = sumHtml + '<div id="top5VTBodyTbl">' + renderSortableTable(cols, rows, { highlightTopN: 5 }) + '</div>';
            wireSortableTable(byId("top5VTBodyTbl"), cols, rows, { highlightTopN: 5 });
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            content.querySelectorAll("[data-mavt-drill]").forEach(function (el) {
                el.addEventListener("click", function (e) {
                    e.preventDefault();
                    openDetail("matCountDrill_codes", -1);
                });
            });
        }).catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
    }

    function renderTop5KHAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading("Đang tải danh sách khách hàng theo giá trị tồn...");
        requestJson("/api/DashboardKhoDesktop/GetKhachHangTonKhoChiTiet").then(function (data) {
            var rows = normalizeArray(data);
            var tongSL = 0, tongThanhTien = 0, ktSL = 0, ktThanhTien = 0;
            for (var i = 0; i < rows.length; i++) {
                tongSL += toNumber(rows[i].GiaTri);
                tongThanhTien += toNumber(rows[i].ThanhTien);
                if (rows[i].KhachHang === "Khách trống" || rows[i].KhachHang === "") {
                    ktSL += toNumber(rows[i].GiaTri);
                    ktThanhTien += toNumber(rows[i].ThanhTien);
                }
            }
            var avgSL = rows.length > 0 ? tongSL / rows.length : 0;
            var sumHtml = renderSummaryStrip([
                { label: "Tổng giá trị (VND)", value: formatNumber(tongThanhTien, 0), sub: "Tổng thành tiền" },
                { label: "Số khách", value: formatNumber(rows.length, 0), sub: "khách hàng", kind: "primary" },
                { label: "Tổng SL tồn", value: formatNumber(tongSL, 0), sub: "Đơn vị tính", kind: "success" },
                { label: "Khách trống (VND)", value: formatNumber(ktThanhTien, 0), sub: "Thành tiền", kind: "warn" }
            ]);
            var maxPct = 0;
            for (var j = 0; j < rows.length; j++) { if (toNumber(rows[j].TyTrong) > maxPct) maxPct = toNumber(rows[j].TyTrong); }
            var tableHtml = '<div class="dk-detail-table-wrap"><table class="dk-detail-table dk-top5kh-table"><thead><tr>' +
                '<th style="width:40px;text-align:center">#</th>' +
                '<th>Khách hàng</th>' +
                '<th style="width:100px;text-align:center">Mã KH</th>' +
                '<th style="width:80px;text-align:right">Số mã VT</th>' +
                '<th style="width:80px;text-align:right">Số roll</th>' +
                '<th style="width:100px;text-align:right">Tổng CBM</th>' +
                '<th style="width:120px;text-align:right">SL tồn</th>' +
                '<th style="width:150px;text-align:right">Thành tiền (VND)</th>' +
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
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.SoRoll), 0) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.TongCBM), 4) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.GiaTri), 0) + '</td>' +
                    '<td class="text-end dk-cell-num">' + formatNumber(toNumber(r.ThanhTien), 0) + '</td>' +
                    '<td><div class="dk-pct-bar-wrap"><div class="dk-pct-bar-fill" style="width:' + pBarW.toFixed(1) + '%"></div><span class="dk-pct-bar-label">' + formatNumber(p, 1) + '%</span></div></td>' +
                    '</tr>';
            }
            tableHtml += '</tbody></table></div>';
            content.innerHTML = sumHtml + tableHtml;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            content.querySelectorAll(".dk-row-kh").forEach(function (tr) {
                tr.addEventListener("click", function () {
                    var customerItem = { MaKH: this.getAttribute("data-makh"), TenKH: this.getAttribute("data-tenkh") };
                    var contentEl = byId(ids.detailModalContent);
                    var titleEl = byId(ids.detailModalTitle);
                    var rcEl = byId("detailModalRowCount");
                    var metaEl = byId(ids.detailModalMeta);
                    var sb = document.querySelector(".dk-modal-search-bar");
                    _detailStack.push({
                        detail: _currentDetail,
                        index: _currentDetailIndex,
                        savedHtml: contentEl ? contentEl.innerHTML : null,
                        savedTitle: titleEl ? (titleEl.textContent || titleEl.innerText || "Toàn bộ khách hàng theo giá trị tồn") : "Toàn bộ khách hàng theo giá trị tồn",
                        savedRowCount: rcEl ? rcEl.textContent : null,
                        savedMeta: metaEl ? metaEl.textContent : null,
                        savedSearchBar: sb ? sb.style.display !== "none" : true
                    });
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

    function rewireTop5KHRowClicks() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.querySelectorAll(".dk-row-kh").forEach(function (tr) {
            var newTr = tr.cloneNode(true);
            tr.parentNode.replaceChild(newTr, tr);
            newTr.addEventListener("click", function () {
                var customerItem = { MaKH: this.getAttribute("data-makh"), TenKH: this.getAttribute("data-tenkh") };
                var contentEl = byId(ids.detailModalContent);
                var titleEl = byId(ids.detailModalTitle);
                var rcEl = byId("detailModalRowCount");
                var metaEl = byId(ids.detailModalMeta);
                var sb = document.querySelector(".dk-modal-search-bar");
                _detailStack.push({
                    detail: _currentDetail,
                    index: _currentDetailIndex,
                    savedHtml: contentEl ? contentEl.innerHTML : null,
                    savedTitle: titleEl ? (titleEl.textContent || titleEl.innerText || "Toàn bộ khách hàng theo giá trị tồn") : "Toàn bộ khách hàng theo giá trị tồn",
                    savedRowCount: rcEl ? rcEl.textContent : null,
                    savedMeta: metaEl ? metaEl.textContent : null,
                    savedSearchBar: sb ? sb.style.display !== "none" : true
                });
                _currentDetail = "_customerRow_drill";
                _currentDetailIndex = -1;
                renderDetailModalDirect({
                    title: "Vị trí kệ — " + (customerItem.TenKH || customerItem.MaKH),
                    rows: [], columns: [],
                    customDrillCustomer: customerItem
                });
            });
        });
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
                '<th style="width:50px;text-align:center">STT</th>' +
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
        if (rows.length === 0) return '<tr><td colspan="11" class="dk-empty">Không có vật tư phù hợp bộ lọc</td></tr>';
        return rows.map(function (r, idx) {
            var cl = r._ConLai;
            var cls = cl <= 0 ? "dk-conlai-overdue" : cl <= 7 ? "dk-conlai-urgent" : cl <= 30 ? "dk-conlai-warn" : "dk-conlai-soft";
            var icon = cl <= 0 ? '<i class="fa-solid fa-triangle-exclamation"></i> ' : "";
            return '<tr>' +
                '<td class="text-center">' + (idx + 1) + '</td>' +
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

    function renderGiaTriNhomAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading();
        requestJson("/api/DashboardKhoDesktop/GetGiaTriNhomChiTiet").then(function (data) {
            var rows = normalizeArray(data);
            var groups = rows.filter(function (r) { return toNumber(r.IsGroup) === 1; });
            var tongGiaTri = 0, tongCBM = 0, tongMaVT = 0;
            for (var i = 0; i < groups.length; i++) {
                tongGiaTri += toNumber(groups[i].GiaTri);
                tongCBM += toNumber(groups[i].TongCBM);
                tongMaVT += toNumber(groups[i].SoMaVT);
            }
            var sumHtml = renderSummaryStrip([
                { label: "Tổng giá trị (VND)", value: formatNumber(tongGiaTri, 0), sub: groups.length + " nhóm" },
                { label: "Tổng CBM", value: formatNumber(tongCBM, 2), sub: "m³", kind: "primary" },
                { label: "Số mã vật tư", value: formatNumber(tongMaVT, 0), sub: "mã", kind: "success" }
            ]);
            var html = sumHtml + '<div id="giaTriNhomDonut" class="dk-nhom-donut-modal" style="min-height:520px; height:520px; overflow:visible;"></div>';
            content.innerHTML = html;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Số nhóm: " + formatNumber(groups.length, 0);
            if (typeof Highcharts !== "undefined" && groups.length > 0) {
                applyHighchartsTheme();
                Highcharts.chart("giaTriNhomDonut", {
                    chart: { type: "pie", backgroundColor: "transparent", height: 500 },
                    title: { text: null },
                    credits: { enabled: false },
                    tooltip: {
                        useHTML: true,
                        pointFormat: '<b>{point.name}</b><br/>Giá trị: <b>{point.y:,.0f} VND</b><br/>Tỷ trọng: <b>{point.pct:.1f}%</b>'
                    },
                    plotOptions: {
                        pie: {
                            innerSize: "50%",
                            borderWidth: 2,
                            showInLegend: true,
                            dataLabels: {
                                enabled: true,
                                distance: 20,
                                allowOverlap: false,
                                format: "<b>{point.name}</b><br/>{point.pct:.1f}%",
                                style: { fontSize: "11px", textOutline: "none", fontWeight: "600" }
                            }
                        }
                    },
                    legend: {
                        enabled: true,
                        layout: "horizontal",
                        align: "center",
                        verticalAlign: "bottom",
                        itemStyle: { fontSize: "12px", fontWeight: "600" },
                        itemMarginTop: 4,
                        itemMarginBottom: 4,
                        labelFormatter: function () {
                            return this.name + " <span style=\"font-weight:400;color:#666\">(" + formatNumber(this.y, 0) + " VND — " + this.pct.toFixed(1) + "%)</span>";
                        }
                    },
                    series: [{
                        name: "Giá trị",
                        colorByPoint: true,
                        data: groups.map(function (g) {
                            return { name: g.Nhom, y: toNumber(g.GiaTri), pct: toNumber(g.TyTrong) };
                        })
                    }]
                });
            }
        }).catch(function (err) { content.innerHTML = modalErrorBox(err && err.message); });
    }

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

    function renderKpiSummaryStrip(items, extraClass) {
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

    function renderKpiSubtabs(tabs, activeKey) {
        var html = '<div class="dk-kk-subtabs">';
        for (var i = 0; i < tabs.length; i++) {
            var t = tabs[i];
            var active = (t.key === activeKey) ? ' active' : '';
            html += '<button type="button" class="dk-kk-subtab' + active + '" data-tab-key="' + escapeHtml(t.key) + '"><i class="fa-solid ' + t.icon + '"></i> ' + escapeHtml(t.label) + '</button>';
        }
        return html + '</div>';
    }

    function renderKpiFilterBar(placeholder, opts) {
        opts = opts || {};
        var html = '<div class="dk-modal-filter-bar">' +
            '<i class="fa-solid fa-magnifying-glass"></i>' +
            '<input type="text" id="kpiSearch" class="dk-kk-search" placeholder="' + escapeHtml(placeholder) + '" />' +
            '<span id="kpiCount" class="dk-text-muted" style="display: none;"></span>' +
            '</div>';
        return html;
    }

    function bindKpiSearch(scope, refilterFn) {
        var input = byId("kpiSearch");
        if (!input) return;
        setTimeout(function () { input.focus(); }, 120);
        input.addEventListener("input", function () { refilterFn(); });
    }

    function formatVNDShort(v) {
        var n = toNumber(v);
        if (n >= 1e9) return (n / 1e9).toFixed(2).replace(/\.?0+$/, "") + " tỷ";
        if (n >= 1e6) return (n / 1e6).toFixed(1).replace(/\.0$/, "") + " tr";
        return formatNumber(n, 0);
    }

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
        var selectedIdx = -1;

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
                '<div class="dk-tdk-master-detail" id="tdkMasterDetail">' +
                '<div class="dk-tdk-left" id="kpiTbody"></div>' +
                '<div class="dk-tdk-resizer" id="tdkResizer"></div>' +
                '<div class="dk-tdk-right" id="tdkDetailPanel"></div>' +
                '</div>';
            bindKpiSubtabs();
            bindKpiSearch(content, doFilter);
            selectedIdx = 0;
            doFilter();
            bindResizer();
        }

        function doFilter() {
            var q = (byId("kpiSearch").value || "").trim().toLowerCase();
            var filtered = allRows.filter(function (r) {
                if (!q) return true;
                var hay = ((r.MaNPL || "") + " " + (r.ItemCode || "") + " " + (r.TenVT || "") + " " + (r.MauVT || "") + " " + (r.KhoVai || "")).toLowerCase();
                return hay.indexOf(q) >= 0;
            });
            renderMasterTable(filtered);
            var cnt = byId("kpiCount"); if (cnt) cnt.textContent = filtered.length + " dòng";
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
        }

        function renderMasterTable(rows) {
            var tbody = byId("kpiTbody");
            if (!tbody) return;
            var html = '<table class="dk-detail-table dk-tdk-table"><thead><tr>' +
                '<th class="dk-tdk-col-stt">STT</th>' +
                '<th class="dk-tdk-col-loai">Loại</th>' +
                '<th class="dk-tdk-col-ic">ItemCode</th>' +
                '<th class="dk-tdk-col-mota">Mô tả</th>' +
                '<th class="dk-tdk-col-mau">Màu VT</th>' +
                '<th class="dk-tdk-col-kho">Width/Size</th>' +
                '<th class="dk-tdk-col-dv">ĐV</th>' +
                '<th class="dk-tdk-col-sl">SL tồn đầu kỳ</th>' +
                '</tr></thead><tbody>';
            for (var i = 0; i < rows.length; i++) {
                var r = rows[i];
                var cls = (i === selectedIdx) ? ' dk-tdk-row-active' : '';
                html += '<tr data-tdk-idx="' + i + '" class="dk-tdk-row' + cls + '">' +
                    '<td class="dk-tdk-col-stt">' + (i + 1) + '</td>' +
                    '<td class="dk-tdk-col-loai">' + escapeHtml(r.LoaiKho || "") + '</td>' +
                    '<td class="dk-tdk-col-ic dk-tdk-ellip" title="' + escapeHtml(r.ItemCode || "") + '">' + escapeHtml(r.ItemCode || "") + '</td>' +
                    '<td class="dk-tdk-col-mota dk-tdk-ellip" title="' + escapeHtml(r.TenVT || "") + '">' + escapeHtml(r.TenVT || "") + '</td>' +
                    '<td class="dk-tdk-col-mau dk-tdk-ellip" title="' + escapeHtml(r.MauVT || "") + '">' + escapeHtml(r.MauVT || "") + '</td>' +
                    '<td class="dk-tdk-col-kho">' + escapeHtml(r.KhoVai || "") + '</td>' +
                    '<td class="dk-tdk-col-dv">' + escapeHtml(r.DonVi || "") + '</td>' +
                    '<td class="dk-tdk-col-sl">' + formatNumber(toNumber(r.SLTonDau), 1) + '</td>' +
                    '</tr>';
            }
            html += '</tbody></table>';
            tbody.innerHTML = html;
            var trs = tbody.querySelectorAll("tr[data-tdk-idx]");
            for (var j = 0; j < trs.length; j++) {
                (function (tr) {
                    tr.addEventListener("click", function () {
                        var idx = parseInt(tr.getAttribute("data-tdk-idx"), 10);
                        selectedIdx = idx;
                        var allTr = tbody.querySelectorAll("tr[data-tdk-idx]");
                        for (var k = 0; k < allTr.length; k++) allTr[k].classList.remove("dk-tdk-row-active");
                        tr.classList.add("dk-tdk-row-active");
                        renderDetailPanel(rows[idx]);
                    });
                })(trs[j]);
            }
            if (rows.length > 0) {
                var autoIdx = (selectedIdx >= 0 && selectedIdx < rows.length) ? selectedIdx : 0;
                selectedIdx = autoIdx;
                var activeTr = tbody.querySelector('tr[data-tdk-idx="' + autoIdx + '"]');
                if (activeTr) activeTr.classList.add("dk-tdk-row-active");
                renderDetailPanel(rows[autoIdx]);
            }
        }

        function renderDetailPanel(row) {
            var panel = byId("tdkDetailPanel");
            if (!panel || !row) return;
            panel.innerHTML =
                '<div class="dk-tdk-detail-header">' +
                '<div class="dk-tdk-detail-title">' + escapeHtml(row.ItemCode || "") + '</div>' +
                '<div class="dk-tdk-detail-sub">' + escapeHtml(row.TenVT || "") + '</div>' +
                '</div>' +
                '<div class="dk-tdk-roll-table-wrap" id="tdkRollTableWrap">' +
                '<div class="dk-loading-inline" style="padding:15px;color:var(--dk-text-muted,#64748b)"><i class="fa fa-spinner fa-spin"></i> \u0110ang t\u1ea3i chi ti\u1ebft...</div>' +
                '</div>' +
                '<div class="dk-tdk-detail-meta">' +
                '<div><span class="dk-tdk-meta-label">Lo\u1ea1i kho:</span> ' + escapeHtml(row.LoaiKho || "") + '</div>' +
                '<div><span class="dk-tdk-meta-label">M\u00e0u VT:</span> ' + escapeHtml(row.MauVT || "") + '</div>' +
                '<div><span class="dk-tdk-meta-label">Width/Size:</span> ' + escapeHtml(row.KhoVai || "") + '</div>' +
                '<div><span class="dk-tdk-meta-label">\u0110\u01a1n v\u1ecb:</span> ' + escapeHtml(row.DonVi || "") + '</div>' +
                '</div>';

            var from = state.dateFilter ? state.dateFilter.from : "";
            requestJson("/api/DashboardKhoDesktop/GetTonDauKyRollDetail?tuNgay=" + encodeURIComponent(from) + "&loai=" + encodeURIComponent(row.MaNPL || ""))
                .then(function (data) {
                    var list = normalizeArray(data);

                    var tbodyHtml = "";
                    list.forEach(function (r) {
                        var ttVal = toNumber(r.DonGia) * toNumber(r.SoLuongThucTe);
                        var ttStr = formatNumber(ttVal, 0) + " " + escapeHtml(r.TienTe || "USD");

                        tbodyHtml += '<tr>' +
                            '<td style="text-align:right;font-weight:700;color:var(--dk-primary,#3b82f6)">' + formatNumber(toNumber(r.SLTonDau), 1) + '</td>' +
                            '<td style="text-align:left;font-weight:700;color:var(--dk-warn,#f59e0b)">' + escapeHtml(r.SoKienHienThi || "") + '</td>' +
                            '<td style="text-align:right">' + formatNumber(toNumber(r.DonGia), 0) + '</td>' +
                            '<td style="text-align:right;font-weight:700;color:var(--dk-danger,#ef4444)">' + ttStr + '</td>' +
                            '</tr>';
                    });

                    if (list.length === 0) {
                        tbodyHtml = '<tr><td colspan="4" style="text-align:center;color:var(--dk-text-muted,#64748b)">Kh\u00f4ng c\u00f3 d\u1eef li\u1ec7u chi ti\u1ebft</td></tr>';
                    }

                    var wrap = byId("tdkRollTableWrap");
                    if (wrap) {
                        wrap.innerHTML =
                            '<table class="dk-detail-table dk-tdk-roll-tbl"><thead><tr>' +
                            '<th style="text-align:right">SL t\u1ed3n \u0111\u1ea7u k\u1ef3</th>' +
                            '<th style="text-align:left">S\u1ed1 roll/ki\u1ec7n</th>' +
                            '<th style="text-align:right">\u0110\u01a1n gi\u00e1</th>' +
                            '<th style="text-align:right">Th\u00e0nh ti\u1ec1n</th>' +
                            '</tr></thead><tbody>' +
                            tbodyHtml +
                            '</tbody></table>';
                    }
                })
                .catch(function (err) {
                    var wrap = byId("tdkRollTableWrap");
                    if (wrap) {
                        wrap.innerHTML = '<div style="color:var(--dk-danger,#ef4444);padding:15px;">L\u1ed7i t\u1ea3i chi ti\u1ebft: ' + escapeHtml(err && err.message) + '</div>';
                    }
                });
        }

        function bindResizer() {
            var resizer = byId("tdkResizer");
            var container = byId("tdkMasterDetail");
            if (!resizer || !container) return;
            var leftPanel = byId("kpiTbody");
            var rightPanel = byId("tdkDetailPanel");
            var dragging = false, rafId = 0;
            resizer.addEventListener("mousedown", function (e) {
                e.preventDefault();
                dragging = true;
                document.body.style.cursor = "col-resize";
                document.body.style.userSelect = "none";
                container.classList.add("dk-tdk-resizing");
            });
            document.addEventListener("mousemove", function (e) {
                if (!dragging) return;
                var cx = e.clientX;
                if (rafId) cancelAnimationFrame(rafId);
                rafId = requestAnimationFrame(function () {
                    var rect = container.getBoundingClientRect();
                    var leftW = Math.max(200, Math.min(cx - rect.left, rect.width - 206));
                    leftPanel.style.flex = "0 0 " + leftW + "px";
                    rightPanel.style.flex = "1 1 0";
                });
            });
            document.addEventListener("mouseup", function () {
                if (dragging) {
                    dragging = false; rafId = 0;
                    document.body.style.cursor = "";
                    document.body.style.userSelect = "";
                    container.classList.remove("dk-tdk-resizing");
                }
            });
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
                sumSL += toNumber(r.SoLuong || r.SLNhap || r.SL);
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
            allRows.forEach(function (r) { sumSL += toNumber(r.SoLuong || r.SLXuat || r.SL); sumGT += toNumber(r.GiaTri); });
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

    function renderAlertDetailModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        var code = window.__dkPendingAlertCode || "po_tre";
        var name = window.__dkPendingAlertName || "Cảnh báo";
        window.__dkPendingAlertCode = null;
        window.__dkPendingAlertName = null;

        var titleEl = byId(ids.detailModalTitle);
        if (titleEl) titleEl.textContent = "Chi tiết cảnh báo — " + name;

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

    function renderSortableTable(cols, rows, opts) {
        opts = opts || {};
        var head = "<thead><tr>";
        for (var i = 0; i < cols.length; i++) {
            var c = cols[i];
            var thCls = c.sortable ? ' class="dk-sortable-th" data-sort-key="' + c.key + '"' : "";
            var thStyles = [];
            if (c.width) thStyles.push("width:" + c.width + "px");
            if (c.maxWidth) thStyles.push("max-width:" + c.maxWidth + "px");
            if (c.center) {
                thStyles.push("text-align:center");
            } else if (c.number !== undefined || c.percent) {
                thStyles.push("text-align:right");
            }
            var thStyleStr = thStyles.length ? ' style="' + thStyles.join(";") + '"' : "";

            var labelHtml = escapeHtml(c.label);
            if (c.wrapText && c.maxWidth) {
                labelHtml = '<div style="max-width:' + c.maxWidth + 'px; white-space:normal; text-align:left;">' + labelHtml + '</div>';
            }

            head += '<th' + thCls + thStyleStr + '>' + labelHtml +
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
                var tdStyles = [];
                if (c.width) tdStyles.push("width:" + c.width + "px");
                if (c.maxWidth) tdStyles.push("max-width:" + c.maxWidth + "px");
                var styleStr = tdStyles.length ? ' style="' + tdStyles.join(";") + '"' : "";

                var contentHtml = escapeHtml(v || "");
                if (c.wrapText && c.maxWidth) {
                    contentHtml = '<div style="max-width:' + c.maxWidth + 'px; white-space:normal; word-break:break-word; text-align:left;">' + contentHtml + '</div>';
                }

                if (c.drillTo && c.key === "MaVT") {
                    return '<td class="dk-cell-mono"' + styleStr + '><a href="#" class="dk-link-inline" data-mavt-drill="1">' + contentHtml + '</a></td>';
                }
                if (c.key === "ViTriKe") {
                    var isPlaced = v && v !== "Chưa xếp kệ" && v !== "Chưa xếp";
                    var chipCls = isPlaced ? "dk-status-chip-good" : "dk-status-chip-neutral";
                    return '<td class="text-center"' + styleStr + '><span class="dk-status-chip ' + chipCls + '">' + escapeHtml(v || "Chưa xếp") + '</span></td>';
                }
                if (c.key === "LoaiKho") {
                    var chipCls = v === "NL" ? "dk-status-chip-good" : (v === "PL" ? "dk-status-chip-warn" : "dk-status-chip-neutral");
                    return '<td class="text-center"' + styleStr + '><span class="dk-status-chip ' + chipCls + '">' + escapeHtml(v || "") + '</span></td>';
                }
                if (c.number !== undefined) return '<td class="text-end dk-cell-num"' + styleStr + '>' + formatNumber(v, c.number) + '</td>';
                if (c.percent) return '<td class="text-end dk-cell-num"' + styleStr + '>' + formatNumber(v, 2) + '%</td>';
                if (c.center) return '<td class="text-center"' + styleStr + '>' + contentHtml + '</td>';
                return '<td' + styleStr + '>' + contentHtml + '</td>';
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
        var chips = document.querySelectorAll(".dk-topbar-chip");
        for (var ci = 0; ci < chips.length; ci++) {
            (function (chip) {
                chip.addEventListener("click", function () {
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
        var btnRefreshTop = byId("btnRefreshTop");
        if (btnRefreshTop) {
            btnRefreshTop.addEventListener("click", function () {
                if (currentPage === 1) {
                    showToast("Đang làm mới dữ liệu của trang tổng quan...", "info");
                } else if (currentPage === 2) {
                    showToast("Đang làm mới dữ liệu của trang xuất - nhập - tồn...", "info");
                } else if (currentPage === 3) {
                    showToast("Đang tải dữ liệu của trang Bảng thống kê...", "info");
                }

                requestJson("/api/DashboardKhoDesktop/ClearCache")
                    .then(function () {
                        if (currentPage === 1) {
                            state.loading = false;
                            loadedPages = { 1: false, 2: false, 3: false };
                            loadData();
                        } else if (currentPage === 2) {
                            loadedPages[2] = false;
                            loadPageData(2);
                        } else if (currentPage === 3) {
                            loadedPages[3] = false;
                            loadPageData(3);
                        }
                    })
                    .catch(function () {
                        if (currentPage === 1) {
                            state.loading = false;
                            loadedPages = { 1: false, 2: false, 3: false };
                            loadData();
                        } else if (currentPage === 2) {
                            loadedPages[2] = false;
                            loadPageData(2);
                        } else if (currentPage === 3) {
                            loadedPages[3] = false;
                            loadPageData(3);
                        }
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
            el.scrollIntoView({ behavior: "smooth", block: "center" });

            el.classList.add("dk-nav-highlight");

            setTimeout(function () {
                el.classList.remove("dk-nav-highlight");
            }, 3500);
        }

        function navigateToSection(section) {
            var targetPage = section.page || 1;
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

    function renderHieuSuatHoatDongAllModal() {
        var content = byId(ids.detailModalContent);
        if (!content) return;
        content.innerHTML = modalLoading("Đang tải chi tiết hiệu suất hoạt động...");

        var from = state.dateFilter ? state.dateFilter.from : "";
        var to = state.dateFilter ? state.dateFilter.to : "";
        var url = "/api/DashboardKhoDesktop/GetHieuSuatHoatDongChiTiet?tuNgay=" + encodeURIComponent(from) + "&denNgay=" + encodeURIComponent(to);

        requestJson(url).then(function (data) {
            var rows = normalizeArray(data);

            var sumHtml = '<div class="dk-modal-summary-strip dk-modal-summary-strip--4col">';
            for (var i = 0; i < rows.length; i++) {
                var r = rows[i];
                var val = toNumber(r.Value);
                var delta = toNumber(r.Delta);
                var cls = val >= 90 ? "good" : (val >= 80 ? "warn" : "neutral");

                sumHtml += '<div class="dk-sum-item dk-sum-' + cls + '">' +
                    '<div class="dk-sum-label">' + escapeHtml(r.TenChiSo || "") + '</div>' +
                    '<div class="dk-sum-value">' + formatNumber(val, 1) + '%</div>' +
                    '<div class="dk-sum-sub">' + formatDelta(delta) + '</div>' +
                    '</div>';
            }
            sumHtml += '</div>';

            var tableHtml = '<table class="dk-detail-table"><thead><tr>' +
                '<th style="width:50px;text-align:center">STT</th>' +
                '<th>Chỉ số hiệu suất</th>' +
                '<th style="width:120px;text-align:right">Tỷ lệ đạt</th>' +
                '<th style="width:200px">Tiến trình</th>' +
                '<th style="width:160px;text-align:center">Xu hướng</th>' +
                '</tr></thead><tbody>';

            for (var j = 0; j < rows.length; j++) {
                var row = rows[j];
                var v = toNumber(row.Value);
                var d = toNumber(row.Delta);
                var deltaStr = formatDelta(d);

                tableHtml += '<tr>' +
                    '<td style="text-align:center">' + (j + 1) + '</td>' +
                    '<td style="font-weight:600">' + escapeHtml(row.TenChiSo || "") + '</td>' +
                    '<td class="text-end dk-cell-num" style="font-weight:700">' + formatNumber(v, 1) + '%</td>' +
                    '<td>' +
                    '<div class="dk-pct-bar-wrap" style="height:10px; background:var(--dk-border); border-radius:4px; overflow:hidden;">' +
                    '<div class="dk-pct-bar-fill" style="width:' + Math.min(100, Math.max(0, v)) + '%; height:100%; background:linear-gradient(90deg, var(--dk-primary-light, #60a5fa), var(--dk-primary, #3b82f6)); border-radius:4px;"></div>' +
                    '</div>' +
                    '</td>' +
                    '<td style="text-align:center">' + deltaStr + '</td>' +
                    '</tr>';
            }
            tableHtml += '</tbody></table>';

            content.innerHTML = sumHtml + tableHtml;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
        }).catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
    }

    function init() {
        var restoreTab = sessionStorage.getItem("dk_restore_tab");
        if (restoreTab) {
            sessionStorage.removeItem("dk_restore_tab");

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

