(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Grid = WIP.Grid || {};

    function poolColumns() {
        return [
            {
                dataField: "MaDH",
                caption: "Đơn hàng",
                cssClass: "col-info",
                fixed: true,
                fixedPosition: "left"
            },
            {
                dataField: "MaHang",
                caption: "Mã hàng",
                cssClass: "col-info"
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                visible: false,
                cssClass: "col-info"
            },
            {
                dataField: "KhachHang",
                caption: "Khách hàng",
                cssClass: "col-info"
            },
            {
                dataField: "PO",
                caption: "PO",
                cssClass: "col-info",
                width: 1000,
                minWidth: 800
            },
            {
                dataField: "SLKH",
                caption: "SLKH",
                dataType: "number",
                format: "#,##0",
                allowEditing: false,
                cssClass: "col-info group-end",
                headerCssClass: "group-end"
            }
        ];
    }

    function cloneColumns(columns, $) {
        return $.extend(true, [], columns || []);
    }

    function buildMainColumns(baseColumns, options) {
        var opt = options || {};
        var columns = cloneColumns(baseColumns, opt.$ || window.jQuery);

        if (typeof opt.prepareGridColumns === "function") {
            columns = opt.prepareGridColumns(columns, opt.permissionLineX);
        }

        if (typeof opt.applyDynamicGroupEnd === "function") {
            columns = opt.applyDynamicGroupEnd(columns);
        }

        if (typeof opt.numberGridLeafCaptions === "function") {
            columns = opt.numberGridLeafCaptions(columns);
        }

        return columns;
    }

    function colDate(field, width, cssClass, opt, deps) {
        opt = opt || {};
        deps = deps || {};
        var fmt = width < 80 ? "dd/MM/yy" : "dd/MM/yyyy";
        var resolvedCssClass = deps.getDataCellCssClass
            ? deps.getDataCellCssClass(cssClass)
            : cssClass;

        return {
            dataField: field,
            caption: deps.getFieldCaption ? deps.getFieldCaption(field) : field,
            width: width,
            dataType: "date",
            format: fmt,
            dateSerializationFormat: "yyyy-MM-dd",
            cssClass: resolvedCssClass,
            allowFiltering: !!opt.allowFiltering
        };
    }

    function text(field, caption, editorOpt, deps) {
        deps = deps || {};
        var $ = deps.$ || window.jQuery;
        var fieldMaxLength = deps.getFieldMaxLength ? deps.getFieldMaxLength(field) : null;
        var options = $.extend(true, {}, editorOpt || {});
        if (fieldMaxLength) options.maxLength = fieldMaxLength;

        return {
            dataField: field,
            permissionField: field,
            label: { text: caption },
            editorType: "dxTextBox",
            editorOptions: options
        };
    }

    function reqText(field, caption, editorOpt, deps) {
        var item = text(field, caption, editorOpt, deps);
        item.validationRules = [
            { type: "required", message: caption + " bắt buộc", trim: true }
        ];
        return item;
    }

    function merSelect(field, caption, editorOpt, deps) {
        deps = deps || {};
        var $ = deps.$ || window.jQuery;
        return {
            dataField: field,
            permissionField: field,
            label: { text: caption },
            editorType: "dxSelectBox",
            editorOptions: $.extend(true, {
                dataSource: deps.mdStore,
                valueExpr: "UserID",
                displayExpr: "Ten",
                searchEnabled: true,
                showClearButton: true,
                placeholder: "Chọn Mer"
            }, editorOpt || {})
        };
    }

    function applyNonNegativeNumberRules(baseOpt, caption) {
        baseOpt.min = 0;

        var oldKeyDown = baseOpt.onKeyDown;
        baseOpt.onKeyDown = function (args) {
            var ev = args && args.event;
            if (ev && (ev.key === "-" || ev.key === "Subtract")) {
                ev.preventDefault();
                return;
            }
            if (typeof oldKeyDown === "function") oldKeyDown(args);
        };

        var oldInput = baseOpt.onInput;
        baseOpt.onInput = function (args) {
            var comp = args && args.component;
            if (comp) {
                var value = comp.option("value");
                if (typeof value === "number" && value < 0) comp.option("value", 0);

                var textValue = comp.option("text");
                if (typeof textValue === "string" && textValue.trim().startsWith("-")) {
                    comp.option("value", 0);
                }
            }
            if (typeof oldInput === "function") oldInput(args);
        };

        var oldChanged = baseOpt.onValueChanged;
        baseOpt.onValueChanged = function (args) {
            if (typeof args.value === "number" && args.value < 0) {
                args.component.option("value", 0);
                return;
            }
            if (typeof oldChanged === "function") oldChanged(args);
        };

        return [{ type: "range", min: 0, message: caption + " không được < 0" }];
    }

    function num(field, caption, editorOpt, deps) {
        deps = deps || {};
        var $ = deps.$ || window.jQuery;
        var baseOpt = $.extend({ showSpinButtons: true }, editorOpt || {});
        var isNonNegative = deps.isNonNegField ? deps.isNonNegField(field) : false;
        var validationRules = isNonNegative ? applyNonNegativeNumberRules(baseOpt, caption) : [];

        return {
            dataField: field,
            permissionField: field,
            label: { text: caption },
            editorType: "dxNumberBox",
            editorOptions: baseOpt,
            validationRules: validationRules
        };
    }

    function reqNum(field, caption, editorOpt, deps) {
        deps = deps || {};
        var $ = deps.$ || window.jQuery;
        return {
            dataField: field,
            permissionField: field,
            label: { text: caption },
            editorType: "dxNumberBox",
            editorOptions: $.extend({ showSpinButtons: true }, editorOpt || {}),
            validationRules: [
                { type: "required", message: caption + " bắt buộc" }
            ]
        };
    }

    function date(field, caption, editorOpt, deps) {
        deps = deps || {};
        return {
            dataField: field,
            permissionField: field,
            label: { text: caption },
            editorType: "dxDateBox",
            editorOptions: deps.buildStrictDateEditorOptions
                ? deps.buildStrictDateEditorOptions(editorOpt)
                : (editorOpt || {})
        };
    }

    function bool(field, caption, editorOpt) {
        return {
            dataField: field,
            permissionField: field,
            label: { text: caption },
            editorType: "dxCheckBox",
            editorOptions: editorOpt || {}
        };
    }

    function buildRawMainColumns(deps) {
        deps = deps || {};
        var selectedIsGiaCong = deps.getSelectedIsGiaCong ? deps.getSelectedIsGiaCong() : !!deps.selectedIsGiaCong;
        var mdStore = deps.mdStore;

        function getFieldCaption(field, fallbackCaption) {
            if (typeof deps.getFieldCaption === "function") {
                return deps.getFieldCaption(field, fallbackCaption);
            }
            return fallbackCaption || field;
        }

        function colDate(field, width, cssClass, opt) {
            return WIP.Grid.WipGridColumns.colDate(field, width, cssClass, opt, {
                $: deps.$ || window.jQuery,
                getDataCellCssClass: deps.getDataCellCssClass,
                getFieldCaption: getFieldCaption
            });
        }

        return [
                    // + DonHang
                    {
                        caption: "Đơn hàng",
                        alignment: "center",
                        fixed: true,
                        fixedPosition: "left",
                        cssClass: "group-end",
                        columns: [
                            {
                                dataField: "Mer",
                                caption: getFieldCaption("Mer"),
                                width: 80,
                                cssClass: "col-info",
                                fixed: true,
                                fixedPosition: "left",
                                allowEditing: true,

                                calculateDisplayValue: function (rowData) {
                                    return rowData.NameMer || "";
                                },

                                lookup: {
                                    dataSource: mdStore,
                                    valueExpr: "UserID",
                                    displayExpr: "Ten"
                                }
                            },
                            {
                                dataField: "ThuTuChuyen", caption: getFieldCaption("ThuTuChuyen"), width: 55, dataType: "number",
                                cssClass: "col-info",
                                allowEditing: false, fixed: true, fixedPosition: "left", allowFiltering: false
                            },

                            {
                                dataField: "LineX", caption: getFieldCaption("LineX"), width: 60,
                                cssClass: "col-info",
                                allowSorting: false, fixed: true, fixedPosition: "left", visible: false, allowEditing: false
                            },

                            {
                                dataField: "KhachHang", caption: getFieldCaption("KhachHang"), width: 90,
                                cssClass: "col-info",
                                fixed: true, fixedPosition: "left", allowEditing: false
                            },
                            {
                                dataField: "LineName", caption: "Chuyền", width: 60,
                                cssClass: "col-info",
                                allowSorting: false, fixed: true, fixedPosition: "left", allowFiltering: false, allowEditing: false, visible: !selectedIsGiaCong
                            },
                            {
                                dataField: "TenDVSX",
                                caption: "(4) Đơn vị gia công",
                                visible: selectedIsGiaCong,
                                width: 115,
                                cssClass: "col-info",
                                allowSorting: false,
                                fixed: true,
                                fixedPosition: "left",
                                allowFiltering: false,
                                allowEditing: false
                            },
                            {
                                dataField: "MaLenhSanXuat", caption: "Mã lệnh sản xuất", width: 180,
                                cssClass: "col-info",
                                allowSorting: false, fixed: true, fixedPosition: "left", visible: false, allowEditing: false
                            },
                            {
                                dataField: "LenhSX", caption: "Mã lệnh sản xuất", width: 60, dataType: "number",
                                cssClass: "col-info",
                                fixed: true, fixedPosition: "left", visible: true, allowEditing: false
                            },
                            {
                                dataField: "MaDH", caption: getFieldCaption("MaDH"), width: 90,
                                cssClass: "col-info",
                                allowSorting: true, fixed: true, fixedPosition: "left", visible: true, allowEditing: false
                            },

                            {
                                dataField: "MaKH", caption: getFieldCaption("MaKH"), width: 75,
                                cssClass: "col-info",
                                allowSorting: false, fixed: true, fixedPosition: "left", visible: true, allowEditing: false
                            },
                            {
                                dataField: "MaHang", caption: getFieldCaption("StyleId"), width: 130,
                                cssClass: "col-info",
                                fixed: true, fixedPosition: "left", visible: true, allowEditing: false
                            },

                            {
                                dataField: "TenCL", visible: true, caption: "Des", width: 60,
                                cssClass: "col-info",
                                fixed: true, fixedPosition: "left", visible: true, allowEditing: false
                            },
                            {
                                dataField: "PO", visible: true, caption: getFieldCaption("PO"), width: 120,
                                cssClass: "col-info",
                                fixed: true, fixedPosition: "left", visible: true, allowEditing: false
                            },

                            {
                                dataField: "Season", caption: getFieldCaption("Season"), width: 60,
                                cssClass: "col-info",
                                allowSorting: false, fixed: true, fixedPosition: "left", visible: false
                            },

                            {
                                dataField: "StyleId", visible: false, caption: getFieldCaption("StyleId"), width: 170,
                                cssClass: "col-info",
                                fixed: true, fixedPosition: "left", allowEditing: false
                            },

                            {
                                dataField: "GhiChu", caption: "Ghi chú", width: 130,
                                cssClass: "col-info",
                                fixed: true, fixedPosition: "left", visible: true, allowEditing: false, allowFiltering: false
                            },

                            {
                                dataField: "SLKH", caption: getFieldCaption("SLKH"), width: 60, dataType: "number",
                                cssClass: "col-info group-end", headerCssClass: "group-end",
                                fixed: true, fixedPosition: "left", allowEditing: false
                            },
                        ]
                    },
                    {
                        caption: "IN-ÉP-THÊU-LASER",
                        alignment: "center",
                        fixed: false,
                        fixedPosition: "left",
                        cssClass: "group-end",
                        columns: [
                            { dataField: "InTheu", caption: "Bên ngoài", width: 60, dataType: "boolean", allowFiltering: false },
                            { dataField: "InTheuTrong_PKH", caption: "Công ty", width: 60, dataType: "boolean", allowFiltering: false },
                            { dataField: "HutAm", caption: "Hút ẩm", width: 60, dataType: "boolean", allowFiltering: false },
                            { dataField: "DoKim", caption: "Dò kim", width: 60, dataType: "boolean", cssClass: "group-end", allowFiltering: false },
                        //    { dataField: "InTheuNgoai_PKH", caption: "Bên ngoài", width: 60, dataType: "boolean", cssClass: "group-end", allowFiltering: false },
                        ]
                    },
                
                    {
                        caption: "Đồng bộ in/thêu",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            {
                                caption: "Đưa BTP",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_GuiInTheu", 80, ""),
                                    colDate("TT_GuiInTheu", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Nhận BTP",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_NhanInTheu", 80, ""),
                                    colDate("TT_NhanInTheu", 80, "group-end"),
                                ]
                            },
                        ],
                    },
               
                
                    // + Nang Suat
                    {
                        caption: "Năng suất",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            { dataField: "NSGio", caption: getFieldCaption("NSGio"), width: 60, dataType: "number", allowFiltering: false },
                            { dataField: "SoGioSX", caption: getFieldCaption("SoGioSX"), width: 60, dataType: "number", allowFiltering: false },
                            { dataField: "SoNgay", caption: getFieldCaption("SoNgay"), width: 60, dataType: "number", cssClass: "group-end", allowFiltering: false }
                        ]
                    },

                    // + Thoi Gian
                    {
                        caption: "Thời gian",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            colDate("KHCat", 70, ""),
                            colDate("TTCat", 70, ""),
                            colDate("KHLapTrinh", 70, ""),
                            colDate("TTLapTrinh", 70, ""),
                            colDate("KHMay", 70, ""),
                            colDate("TTMay", 70, ""),
                            // Cột cuối nhóm thêm class group-end vào tham số thứ 3
                            colDate("ThoatChuyen", 70, ""),
                            colDate("TTThoatChuyen", 70, "group-end"),
                        ]
                    },

                    // + Tien Do Ra Hang
                    {
                        caption: "Tiến độ",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            { dataField: "TT_Cat", caption: getFieldCaption("TT_Cat"), width: 55, dataType: "number", allowFiltering: false, allowEditing: false },
                            { dataField: "BTP_DK", caption: getFieldCaption("BTP_DK"), width: 60, dataType: "number", allowFiltering: false, allowEditing: false },
                            { dataField: "RaChuyen", caption: getFieldCaption("RaChuyen"), width: 55, dataType: "number", allowFiltering: false, allowEditing: false },
                            { dataField: "TP_KiemDat", caption: getFieldCaption("TP_KiemDat"), width: 55, dataType: "number", allowFiltering: false, allowEditing: false },
                            { dataField: "TP_Nhan", caption: getFieldCaption("TP_Nhan"), width: 55, dataType: "number", allowFiltering: false, allowEditing: false },
                            { dataField: "Packing", caption: "Packing", width: 65, cssClass: "group-end", allowFiltering: false, allowEditing: false },
                        ]
                    },
                    // + Merchandiser
                    {
                        caption: "QC",
                        alignment: "center",
                        cssClass: "group-end",
                        headerCssClass: "hdr-qc",
                        columns: [
                            {
                                caption: "Mẫu TOP",
                                cssClass: "group-end",
                                alignment: "center",
                                columns: [
                                    colDate("KH_Top", 70, ""),
                                    { dataField: "SLSizeMau_Top", caption: getFieldCaption("SLSizeMau_Top"), width: 90, dataType: "number", allowFiltering: false },
                                    colDate("TT_MauTop", 75, "group-end"),
                                ]
                            },
                            {
                                caption: "Shipment sample",
                                alignment: "center",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_Ship", 70, ""),
                                    { dataField: "SLSizeMau_Ship", caption: getFieldCaption("SLSizeMau_Ship"), width: 90, dataType: "number", allowFiltering: false },
                                    colDate("TT_Ship", 70, "group-end"),
                                ]
                            },
                            {
                                caption: "Mẫu đầu chuyền",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_MauDauChuyen", 80, ""),
                                    colDate("TT_MauDauChuyen", 80, "group-end"),
                                ]
                            },
                        ]
                    },
                    {
                        caption: "Merchandiser",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            {
                                caption: "Vải",
                                alignment: "center",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_Vai", 80, ""),
                                    { dataField: "TT_Vai", caption: getFieldCaption("TT_Vai"), width: 80, cssClass: "group-end", allowFiltering: false }
                                ]
                            },
                            {
                                caption: "PL In ép-LT",
                                alignment: "center",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_PLInEp", 80, ""),
                                    { dataField: "TT_PLInEp", caption: getFieldCaption("TT_PLInEp"), width: 80, cssClass: "group-end", allowFiltering: false }
                                ]
                            },
                            {
                                caption: "PL may",
                                alignment: "center",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_PLMay", 80, ""),
                                    { dataField: "TT_PLMay", caption: getFieldCaption("TT_PLMay"), width: 80, cssClass: "group-end", allowFiltering: false }
                                ]
                            },
                            {
                                caption: "Packing list",
                                alignment: "center",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_PackingList", 80, ""),
                                    colDate("TT_PackingList", 80, "group-end")
                                ]
                            },
                            {
                                caption: "PL đóng gói",
                                alignment: "center",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_PLDongGoi", 80, ""),
                                    { dataField: "TT_PLDongGoi", caption: getFieldCaption("TT_PLDongGoi"), width: 80, cssClass: "group-end", allowFiltering: false }
                                ]
                            },
                            {
                                caption: "Lệnh SX",
                                alignment: "center",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_LenhSX", 80, ""),
                                    colDate("TT_LenhSX", 80, "group-end"),
                                    //{ dataField: "TT_LenhSX", caption: getFieldCaption("TT_LenhSX"), width: 80, cssClass: "group-end", allowFiltering: false }

                                ]
                            }
                        ]
                    },
                    // + Tinh Toan Nang Suat
                    {
                        caption: "Tính toán năng suất",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            { dataField: "SLCN", caption: getFieldCaption("SLCN"), width: 85, dataType: "number", allowFiltering: false },
                            { dataField: "TGLV", caption: getFieldCaption("TGLV"), width: 70, dataType: "number", allowFiltering: false, visible: false },
                            { dataField: "OT", caption: getFieldCaption("OT"), width: 65, dataType: "number", allowFiltering: false, visible: false },
                            { dataField: "AllocatedRegularHours", caption: getFieldCaption("TGLV"), width: 85, dataType: "number", allowEditing: false, allowFiltering: false },
                            { dataField: "AllocatedOTHours", caption: getFieldCaption("OT"), width: 75, dataType: "number", allowEditing: false, allowFiltering: false },
                            { dataField: "HieuSuat", caption: getFieldCaption("HieuSuat"), width: 65, dataType: "number", allowFiltering: false },

                            { dataField: "CostPerM", caption: getFieldCaption("CostPerM"), width: 75, dataType: "number", allowFiltering: false },
                            { dataField: "Profit", caption: getFieldCaption("Profit"), width: 60, dataType: "number", allowFiltering: false },

                            { dataField: "SMV_WIP", caption: getFieldCaption("SMV_WIP"), width: 90, dataType: "number", allowFiltering: false },
                            { dataField: "NSCost", caption: getFieldCaption("NSCost"), width: 80, dataType: "number", allowFiltering: false },
                            { dataField: "NS_SMV", caption: getFieldCaption("NS_SMV"), width: 80, dataType: "number", cssClass: "group-end", allowFiltering: false }
                        ]
                    },

                    // + Theo doi doanh thu
                    {
                        caption: "Theo dõi doanh thu",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            { dataField: "CM", caption: getFieldCaption("CM"), width: 75, dataType: "number", cssClass: "col-cyan", allowFiltering: false },
                            { dataField: "HeSoCM", caption: getFieldCaption("HeSoCM"), width: 60, dataType: "number", cssClass: "col-cyan", allowFiltering: false },
                            { dataField: "DoanhThuCM", caption: getFieldCaption("DoanhThuCM"), width: 90, dataType: "number", cssClass: "col-cyan group-end", allowFiltering: false }
                        ]
                    },
                    {
                        caption: "Phòng kỹ thuật",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            {
                                caption: "BOM",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("NgayNhanTT_BOM", 80, ""),
                                    colDate("KH_BOM", 80, ""),
                                    colDate("TT_BOM", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "NPL BM+KTX may mẫu",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_NPL_BM_KTX_MayMau", 80, ""),
                                    colDate("TT_NPL_BM_KTX_MayMau", 80, ""),
                                    {
                                        dataField: "TT_NPL_BM_KTX_MayMau_NoData",
                                        caption: "Không phát sinh",
                                        width: 80,
                                        alignment: "center",
                                        dataType: "boolean",
                                        allowFiltering: false,
                                        cssClass: "group-end"
                                    },
                                ]
                            },
                            {
                                caption: "Test keo logo -tape laser",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_TestKeoLogo_TapeLaser", 80, ""),
                                    colDate("TT_TestKeoLogo_TapeLaser", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Rập",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_Rap", 80, ""),
                                    colDate("TT_Rap", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Tác nghiệp cắt",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_TacNghiepCat", 80, ""),
                                    colDate("TT_TacNghiepCat", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Sơ đồ",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_SoDo", 80, ""),
                                    colDate("TT_SoDo", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "BẢNG MÀU VẢI + IN ÉP",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_BangMauVai_InEp", 80, ""),
                                    colDate("TT_BangMauVai_InEp", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "BẢNG MÀU PL + FULL",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_BangMauPL_Full", 80, ""),
                                    colDate("TT_BangMauPL_Full", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Bảng đánh số",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_BangDanhSo", 80, ""),
                                    colDate("TT_BangDanhSo", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "TLiệu in ép",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_TLieuInEp", 80, ""),
                                    colDate("TT_TLieuInEp", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "TLiệu LT",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_TLieuLT", 80, ""),
                                    colDate("TT_TLieuLT", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "TS cắt PL",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_TSCatPL", 80, ""),
                                    colDate("TT_TSCatPL", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Cối nút",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_CoiNut", 80, ""),
                                    colDate("TT_CoiNut", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "LAYOUT NHÃN",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_Layout", 80, ""),
                                    colDate("TT_Layout", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "NHÃN CARE",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_NhanCare", 80, ""),
                                    colDate("TT_NhanCare", 80, ""),
                                    colDate("DateTrenNhanCare", 80, "group-end txt-red-bold"),
                                ]
                            },
                            {
                                caption: "Chuẩn bị sản xuất cho cơ điện",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_CbiSXChoCoDien", 80, ""),
                                    colDate("TT_CbiSXChoCoDien", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Tài liệu hoàn chỉnh",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_TaiLieuHoanChinh", 80, ""),
                                    colDate("TT_TaiLieuHoanChinh", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Kích thùng",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_KichThuoc", 80, ""),
                                    colDate("TT_KichThuoc", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Quy cách đóng gói",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_QuyCachDongGoi", 80, ""),
                                    colDate("TT_QuyCachDongGoi", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Họp TKSX",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_HopTKSX", 80, ""),
                                    colDate("TT_HopTKSX", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "NHÃN BAO THÙNG",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_NhanBaoThung", 80, ""),
                                    colDate("TT_NhanBaoThung", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Duyệt mẫu đầu chuyền",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_DuyetMauDauChuyen", 80, ""),
                                    colDate("TT_DuyetMauDauChuyen", 80, "group-end"),
                                ]
                            },
                        ],
                    },
                    {
                        caption: "IE",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            {
                                caption: "QT May",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_QTMay", 80, ""),
                                    colDate("TT_QTMay", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Nhu cầu máy móc",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_NhuCauMayMoc", 80, ""),
                                    colDate("TT_NhuCauMayMoc", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "LAYOUT",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_Layout_IE", 80, ""),
                                    colDate("TT_Layout_IE", 80, "group-end"),
                                ]
                            },
                            
                            {
                                caption: "Đơn giá công nhân",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_DonGiaCongNhan", 80, ""),
                                    colDate("TT_DonGiaCongNhan", 80, "group-end"),
                                ]
                            },
                        ],
                    },
                    {
                        caption: "Cơ điện",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            {
                                caption: "Duyệt mockup công đoạn máy",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_ChuanBiMayMoc_KTX", 80, ""),
                                    colDate("TT_ChuanBiMayMoc_KTX", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Chuẩn bị, bàn giao máy móc cho chuyền",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_ChuanBiMayMoc_Chuyen", 80, ""),
                                    colDate("TT_ChuanBiMayMoc_Chuyen", 80, "group-end"),
                                ]
                            },
                        ],
                    },
                    {
                        caption: "Kho",
                        alignment: "center",
                        cssClass: "group-end",
                        columns: [
                            {
                                caption: "Cấp phát vải",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_KhoVai", 80, ""),
                                    colDate("TT_KhoVai", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Cấp phát phụ liệu may",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_KhoPhuLieuMay", 80, ""),
                                    colDate("TT_KhoPhuLieuMay", 80, "group-end"),
                                ]
                            },
                            {
                                caption: "Cấp phát thùng",
                                alignment: "center",
                                fixed: false,
                                fixedPosition: "left",
                                cssClass: "group-end",
                                columns: [
                                    colDate("KH_Thung", 80, ""),
                                    colDate("TT_Thung", 80, "group-end"),
                                ]
                            },
                            {
                                dataField: "GhiChuKho",
                                caption: "Ghi chú của kho",
                                alignment: "center",
                                fixed: false,
                                width: 160,
                                cssClass: "group-end",
                                allowFiltering: false,                                
                            },
                        ],
                    }

        ];
    }

    WIP.Grid.WipGridColumns = {
        poolColumns: poolColumns,
        cloneColumns: cloneColumns,
        buildMainColumns: buildMainColumns,
        buildRawMainColumns: buildRawMainColumns,
        colDate: colDate,
        text: text,
        reqText: reqText,
        merSelect: merSelect,
        num: num,
        reqNum: reqNum,
        date: date,
        bool: bool
    };
})(window);
