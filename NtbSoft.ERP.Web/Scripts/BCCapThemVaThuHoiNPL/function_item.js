$(function () {
    let _Mahang = '';
    let _MaLenh = '';
    let _MaKH = '';

    const now = new Date();
    const firstDayOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    const lastDayOfMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0);

    const filterMaHang = $("#filter_mahang").dxDropDownBox({
        valueExpr: "MaLenh",
        displayExpr: function (item) {
            return "Mã hàng: " + item.MaHang + " " + "Mã lệnh: " + item.MaLenh + " " + "Khách hàng: " + item.MaKH
        },
        placeholder: "Chọn mã hàng...",
        showClearButton: true,
        label: "Mã hàng",
        dataSource: new DevExpress.data.CustomStore({
            key: "MaLenh",
            loadMode: "raw",
            load: function () {
                return $.getJSON('/api/baocao/get', { action: 'filter_mahang' });
            }
        }),
        dropDownOptions: {
            width: 700,
        },
        contentTemplate: function (e) {
            const value = e.component.option("value");
            const $dataGrid = $("<div>").dxDataGrid({
                dataSource: e.component.getDataSource(),
                columns: [
                    {
                        caption: "STT",
                        alignment: "center",
                        width: 50,
                        cellTemplate: function (container, options) {
                            const index = options.component.pageIndex() * options.component.pageSize() + options.rowIndex + 1;
                            container.text(index);                        }
                    },
                    { dataField: "MaLenh", caption: "Mã Lệnh" },
                    { dataField: "MaHang", caption: "Mã Hàng" },
                    { dataField: "TenHang", caption: "Tên Hàng" },
                    { dataField: "MaKH", caption: "Khách hàng" }
                ],
                onCellPrepared: function (e) {
                    if (e.rowType === "header") {
                        e.cellElement.css({ "background-color": "#2288ff", "font-weight": "bold", "text-align": "center", "color": "white" });
                    } else if (e.rowType === "group") {
                        e.cellElement.css({
                            "background-color": "#eaf4ff",
                            "color": "#1565c0",
                            "font-weight": "600",
                            "font-size": "16px",
                            "padding-left": "0px"
                        });
                    }
                },
                showRowLines: true,
                showColumnLines: true,
                hoverStateEnabled: true,
                paging: { enabled: true, pageSize: 10 },
                filterRow: { visible: true },
                scrolling: { mode: "virtual" },
                selection: { mode: "single" },
                selectedRowKeys: value ? [value] : [],
                height: "100%",
                onContentReady: function (gridArgs) {
                    if (value) {
                        gridArgs.component.selectRows([value], false);
                    }
                },
                onSelectionChanged: function (selectedItems) {
                    const keys = selectedItems.selectedRowKeys;
                    const hasSelection = keys.length > 0;

                    e.component.option("value", hasSelection ? keys[0] : null);
                    if (hasSelection) {
                        const selectedData = selectedItems.selectedRowsData[0];

                        _Mahang = selectedData.MaHang;
                        _MaLenh = selectedData.MaLenh;
                        _MaKH = selectedData.MaKH;

                        localStorage.setItem('MaHang', _Mahang);
                        localStorage.setItem('MaLenh', _MaLenh);
                        localStorage.setItem('MaKH', _MaKH);

                        e.component.close();

                        const isTabThuHoiActive = $('#tab-thu-hoi').hasClass('active');

                        if (isTabThuHoiActive) {
                            loadDataThuHoi(_Mahang, _MaLenh, _MaKH);
                        } else {
                            return;
                        }
                    }
                },
                onRowClick: function (rowArgs) {
                    const selectedData = rowArgs.data;
                    e.component.option("value", selectedData.MaLenh);

                    _Mahang = selectedData.MaHang;
                    _MaLenh = selectedData.MaLenh;
                    _MaKH = selectedData.MaKH;

                    localStorage.setItem('MaHang', _Mahang);
                    localStorage.setItem('MaLenh', _MaLenh);
                    localStorage.setItem('MaKH', _MaKH);

                    e.component.close();
                    if ($('#tab-thu-hoi').hasClass('active')) {
                        loadDataThuHoi(_Mahang, _MaLenh, _MaKH);
                    } else {
                        loadDataCapThem(_Mahang, _MaLenh, _MaKH);
                    }
                },
            });

            return $dataGrid;
        }
    }).dxDropDownBox("instance");

    $("#select-startdate").dxDateBox({
        type: "date",
        value: firstDayOfMonth,
        displayFormat: "dd/MM/yyyy",
        label: "Ngày bắt đầu",
        labelMode: "floating",
        stylingMode: "outlined",
        onValueChanged: function (e) {
            $("#select-enddate").dxDateBox("instance").option("min", e.value);
            const maHang = localStorage.getItem("MaHang");
            const maLenh = localStorage.getItem("MaLenh");
            const maKH = localStorage.getItem("MaKH");
            if (maHang) loadDataThuHoi(maHang, maLenh, maKH);
            else loadDataCapThem();

            loadDataCapThemNgoaiDH()
        }
    });

    $("#select-enddate").dxDateBox({
        type: "date",
        value: lastDayOfMonth,
        displayFormat: "dd/MM/yyyy",
        label: "Ngày kết thúc",
        labelMode: "floating",
        stylingMode: "outlined",
        min: firstDayOfMonth,
        onValueChanged: function (e) {
            const maHang = localStorage.getItem("MaHang");
            const maLenh = localStorage.getItem("MaLenh");
            const maKH = localStorage.getItem("MaKH");
            if (maHang) loadDataThuHoi(maHang, maLenh, maKH);
            else loadDataCapThem();

            loadDataCapThemNgoaiDH();
        }
    });

    $("#btn-refresh").on("click", function () {
        const dropDownInstance = $("#filter_mahang").dxDropDownBox("instance");
        dropDownInstance.option("value", null);

        localStorage.removeItem('MaHang');
        localStorage.removeItem('MaLenh');
        localStorage.removeItem('MaKH');
        _Mahang = ''; _MaLenh = ''; _MaKH = '';

        if ($('#tab-thu-hoi').hasClass('active')) {
            loadDataThuHoi();
        }
        else if ($('#tab-cap-them').hasClass('active')) {
            loadDataCapThem();
        }
        else if ($('#tab-cap-them-ngoai-don-hang').hasClass('active')) {
            loadDataCapThemNgoaiDH();
        }
    });
});

const LoadingSpinner = {
    show: function (message = 'Đang tải dữ liệu...') {
        let overlay = document.getElementById('loading-overlay');

        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'loading-overlay';
            overlay.innerHTML = `
                <div class="spinner-container">
                    <div class="spinner"></div>
                    <p class="spinner-text">${message}</p>
                    <div class="spinner-progress"></div>
                </div>
            `;
            document.body.appendChild(overlay);

            if (!document.getElementById('loading-spinner-style')) {
                const style = document.createElement('style');
                style.id = 'loading-spinner-style';
                document.head.appendChild(style);
            }
        } else {
            overlay.style.display = 'flex';
            overlay.querySelector('.spinner-text').textContent = message;
        }
    },

    hide: function () {
        const overlay = document.getElementById('loading-overlay');
        if (overlay) {
            overlay.style.animation = 'fadeOut 0.3s ease';
            setTimeout(() => {
                overlay.style.display = 'none';
            }, 300);
        }
    }
};