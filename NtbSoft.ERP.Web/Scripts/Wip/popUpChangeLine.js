(function () {
    window.WIP = window.WIP || {};

    var popup, form, ctx = {};

    // ===== LOADING STATE =====
    var lp = null;
    var _busyCount = 0;
    function isMobileUiMode() {
        var ua = String(navigator.userAgent || "").toLowerCase();
        var uaMobile = /android|iphone|ipad|ipod|windows phone|opera mini|mobile/i.test(ua);
        var viewportMobile = window.innerWidth < 992;
        var coarsePointer = false;
        try {
            coarsePointer = !!(window.matchMedia && (window.matchMedia("(pointer: coarse)").matches || window.matchMedia("(hover: none)").matches));
        } catch (e) { }
        return !!(uaMobile || coarsePointer || viewportMobile);
    }
    function getPopupResponsiveOptions() {
        if (!isMobileUiMode()) return {};
        return {
            fullScreen: true,
            width: "100vw",
            height: "100vh",
            maxWidth: "100vw",
            maxHeight: "100vh",
            dragEnabled: false
        };
    }

    function setBusy(isBusy, msg) {
        msg = msg || "Đang xử lý...";
        if (!lp) return;

        if (isBusy) _busyCount++;
        else _busyCount = Math.max(0, _busyCount - 1);

        var busy = _busyCount > 0;

        lp.option("message", msg);
        lp.option("visible", busy);

        // disable nút Lưu / Đóng để tránh double click hoặc đóng popup khi đang chạy
        try {
            var items = popup.option("toolbarItems") || [];
            items = items.map(function (it) {
                if (it && it.widget === "dxButton" && it.options) {
                    if (it.options.text === "Lưu" || it.options.text === "Đóng") {
                        it = $.extend(true, {}, it);
                        it.options.disabled = busy;
                    }
                }
                return it;
            });
            popup.option("toolbarItems", items);
        } catch (e) { }
    }

    function ensure() {
        if (popup) return;

        if (!$("#popupChangeLine").length) {
            $("body").append("<div id='popupChangeLine'></div>");
        }

        popup = $("#popupChangeLine").dxPopup($.extend(true, {
            title: "Đổi chuyền",
            width: 520,
            height: "auto",
            showTitle: true,
            dragEnabled: true,
            hideOnOutsideClick: true,
            deferRendering: false,
            contentTemplate: function (content) {
                $("<div id='formChangeLine'></div>").appendTo(content);

                // ===== LOAD PANEL (overlay trên popup) =====
                // bọc 1 wrapper để loadpanel đè đúng vùng popup
                var $wrap = $("<div class='change-line-wrap' style='position:relative;'></div>");
                $(content).children().appendTo($wrap);
                $(content).append($wrap);

                $("<div id='lpChangeLine'></div>").appendTo($wrap);
                lp = $("#lpChangeLine").dxLoadPanel({
                    shading: true,
                    showIndicator: true,
                    showPane: true,
                    visible: false,
                    closeOnOutsideClick: false,
                    message: "Đang xử lý..."
                }).dxLoadPanel("instance");

                var mobileMode = isMobileUiMode();
                form = $("#formChangeLine").dxForm({
                    labelLocation: "top",
                    colCount: mobileMode ? 1 : 2,
                    showColonAfterLabel: false,
                    formData: {},
                    items: [
                        { dataField: "MaDH_Display", label: { text: "Mã ĐH" }, editorType: "dxTextBox", editorOptions: { readOnly: true } },
                        { dataField: "MaDH", label: { text: "Mã ĐH" }, editorType: "dxTextBox", editorOptions: { readOnly: true }, visible: false },
                        { dataField: "Season", label: { text: "Đợt SX" }, editorType: "dxTextBox", editorOptions: { readOnly: true } },

                        { dataField: "LenhSX", label: { text: "Lệnh SX" }, editorType: "dxTextBox", editorOptions: { readOnly: true } },
                        { dataField: "MaHang", label: { text: "Mã hàng" }, editorType: "dxTextBox", editorOptions: { readOnly: true } },

                        {
                            dataField: "LineX",
                            label: { text: "Chuyền" },
                            editorType: "dxSelectBox",
                            editorOptions: {
                                dataSource: [],
                                valueExpr: "id",
                                displayExpr: "name",
                                searchEnabled: false,
                                acceptCustomValue: false,
                                openOnFieldClick: true,
                                readOnly: true
                            },
                            validationRules: [{ type: "required", message: "Thiếu chuyền hiện tại" }]
                        },
                        {
                            dataField: "LineChange",
                            label: { text: "Chuyền đổi" },
                            editorType: "dxSelectBox",
                            editorOptions: {
                                dataSource: [],
                                valueExpr: "id",
                                displayExpr: "name",
                                searchEnabled: false,
                                acceptCustomValue: false,
                                openOnFieldClick: true,
                                placeholder: "Chọn chuyền"
                            },
                            validationRules: [{ type: "required", message: "Vui lòng chọn chuyền đổi" }]
                        }
                    ]
                }).dxForm("instance");
            },
            toolbarItems: [
                {
                    widget: "dxButton",
                    toolbar: "bottom",
                    location: "after",
                    options: { text: "Lưu", type: "success", onClick: submit }
                },
                {
                    widget: "dxButton",
                    toolbar: "bottom",
                    location: "after",
                    options: { text: "Đóng", onClick: function () { popup.hide(); } }
                }
            ]
        }, getPopupResponsiveOptions())).dxPopup("instance");
    }

    async function loadChangeLines(maDH, maLenhSanXuat) {
        const base = (ctx && ctx.apiBase) ? ctx.apiBase.replace(/\/?$/, "/") : "/api/";
        const url =
            base + "ERPDonHangTong/Get"
            + "?action=GetLineChia"
            + "&para=" + encodeURIComponent(String(maDH || "").trim())
            + "&para5=" + encodeURIComponent(String(maLenhSanXuat || "").trim());

        const txt = await fetch(url, { method: "GET", credentials: "include" }).then(r => r.text());
        let data = null;
        try { data = JSON.parse(txt); } catch { data = null; }

        if (!Array.isArray(data)) return [];

        return data
            .map(function (x) {
                if (!x || typeof x !== "object") return null;

                const rawId = x.Line;
                const rawName = x.Name;
                const id = rawId == null || rawId === "" ? null : Number(rawId);
                const name = rawName == null ? "" : String(rawName).trim();

                if (!Number.isFinite(id) || !name) return null;
                return { id: id, name: name };
            })
            .filter(Boolean);
    }

    async function open(payload) {
        // payload: { row, apiBase, onDone }
        ensure();

        ctx = payload || {};
        var row = ctx.row || {};

        var lines = (ctx.lines || []).map(function (x) {
            return { id: Number(x.id), name: String(x.name || "") };
        });
        const display = String(ctx.displayMaDH || "").trim() || String(row.MaDH || "").trim();
        // bind data
        form.option("formData", {
            MaDH_Display: display,
            MaDH: row.MaDH || "",
            Season: row.Season || "",
            LenhSX: row.LenhSX || "",
            MaHang: row.StyleId || "",
            LineX: row.LineX != null ? Number(row.LineX) : null,
            LineChange: null,
            MaLenhSanXuat: row.MaLenhSanXuat || "",
            ThuTuChuyen: row.ThuTuChuyen || null
        });

        popup.show();
        form.getEditor("LineX").option("dataSource", lines);
        setBusy(true, "Đang tải danh sách chuyền...");
        try {
            const lineChanges = await loadChangeLines(row.MaDH, row.MaLenhSanXuat);
            form.getEditor("LineChange").option("dataSource", lineChanges);
        } catch (e) {
            form.getEditor("LineX").option("dataSource", []);
            form.getEditor("LineChange").option("dataSource", []);
            DevExpress.ui.notify("Không tải được danh sách chuyền đổi.", "error", 2500);
        } finally {
            setBusy(false);
        }
    }

    async function resolveMaGopFromCheck(maDH, lineXGocId, maLenhSanXuat) {
        const apiBase = (ctx.apiBase || "/api/").replace(/\/?$/, "/");

        const url =
            apiBase + "wip-donhang/checkLineExists"
            + "?maDH=" + encodeURIComponent(String(maDH || "").trim())
            + "&maLenhSanXuat=" + encodeURIComponent(String(maLenhSanXuat || "").trim())
            + "&lineX=" + encodeURIComponent(String(Number(lineXGocId || 0)));

        const res = await fetch(url, { method: "GET", credentials: "include" }).then(r => r.json());
        return String(res?.maGop || "").trim();
    }

    async function checkChuyenIsActive(maLenhSX, lineId) {
        const base = (ctx && ctx.apiBase) ? ctx.apiBase.replace(/\/?$/, "/") : "/api/";
        const url =
            base + "ERPDonHangTong/Get"
            + "?action=CheckChuyen"
            + "&para=" + encodeURIComponent(String(maLenhSX || "").trim())
            + "&para2=" + encodeURIComponent(String(lineId || "").trim());

        const txt = await fetch(url, { method: "GET", credentials: "include" }).then(r => r.text());
        let data = null;
        try { data = JSON.parse(txt); } catch { data = null; }

        const row = Array.isArray(data) && data.length ? data[0] : null;
        return {
            isActive: Number(row?.IsActive ?? 0) === 1,
            nameLine: String(row?.Name ?? "").trim(),
            raw: data
        };
    }
    function GetCurrentUser() {
        return localStorage.getItem("username") || localStorage.getItem("username1") || "";
    }
    async function submit() {
        var vr = form.validate();
        if (!vr.isValid) return;

        var fd = form.option("formData");
        var oldLine = fd.LineX;
        var newLine = fd.LineChange;

        var maLenhSanXuat = fd.MaLenhSanXuat != null ? fd.MaLenhSanXuat : "";
        var base = (ctx && ctx.apiBase) ? ctx.apiBase.replace(/\/$/, "") : "/api/";
        var userName = GetCurrentUser();
        // ===== SHOW LOADING (cover all API calls) =====
        setBusy(true, "Đang đổi chuyền...");

        try {
            const chk = await checkChuyenIsActive(maLenhSanXuat, oldLine);
            if (chk.isActive) {
                DevExpress.ui.notify(`Chuyền ${chk.nameLine || oldLine} đã được lên chuyền, không được phép đổi.`, "warning", 3000);
                return;
            }

            const maGop = await resolveMaGopFromCheck(fd.MaDH, oldLine, maLenhSanXuat);
            if (!maGop) {
                DevExpress.ui.notify("Không lấy được MaGop để đổi chuyền.", "error", 2500);
                return;
            }

            var url =
                base +
                "ERPDonHangTong/GetChuyen" +
                "?action=ChangeChuyen" +
                "&para=" + encodeURIComponent(maGop) +
                "&para2=" + encodeURIComponent(oldLine) +
                "&para3=" + encodeURIComponent(newLine) +
                "&para4=0" +
                "&para5=" + encodeURIComponent(maLenhSanXuat) +
                "&para6=" + encodeURIComponent(userName);

            var txt = await fetch(url, { method: "GET", credentials: "include" }).then(r => r.text());

            if (String(txt ?? "").trim().replace(/^['"]+|['"]+$/g, "").toUpperCase() === "TRUE") {

                await fetch("/api/wip-donhang/changeLine", {
                    method: "POST",
                    credentials: "include",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        MaGop: maGop,
                        MaLenhSanXuat: maLenhSanXuat,
                        OldLine: oldLine,
                        NewLine: newLine,
                        FromThuTu: fd.ThuTuChuyen || null
                    })
                }).then(r => r.json());

                DevExpress.ui.notify("Đổi chuyền thành công.", "success", 1200);
                popup.hide();

                // nếu 2 hàm này lâu cũng đã nằm trong loading
                await WIP.loadUnassignedData();
                await WIP.loadWipData({ oldLine });

            } else {
                DevExpress.ui.notify("Đổi chuyền thất bại: " + (txt || ""), "error", 3000);
            }
        } catch (e) {
            DevExpress.ui.notify("Lỗi gọi API: " + (e?.message || e), "error", 3000);
        } finally {
            setBusy(false);
        }
    }

    window.WIP.ChangeLinePopup = { open: open };
})();
