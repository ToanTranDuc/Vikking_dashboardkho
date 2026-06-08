
$(document).ready(function () {
    const today = new Date();

    $("#fromDate").dxDateBox({
        type: "date",
        displayFormat: "dd/MM/yyyy", 
        useMaskBehavior: true,       
        value: today,
        onValueChanged: function (e) {
            onDateRangeChange();
        }
    });
    $("#toDate").dxDateBox({
        type: "date",
        displayFormat: "dd/MM/yyyy",
        useMaskBehavior: true,
        value: today,
        onValueChanged: function (e) {
            onDateRangeChange();
        }
    });

    loadBaoCaoKiemPL(moment(today).format('YYYY-MM-DD'), moment(today).format('YYYY-MM-DD'), $("#select-filer").val());

    $("#btn-refresh").on("click", function () {
        onDateRangeChange();
    });
});
function onDateRangeChange() {
    const fromDateVal = $("#fromDate").dxDateBox("instance").option("value");
    const toDateVal = $("#toDate").dxDateBox("instance").option("value");

    if (fromDateVal && toDateVal) {
        if (fromDateVal > toDateVal) {
            console.warn("Từ ngày không được lớn hơn Đến ngày.");
            return;
        }

        const formatFromDate = moment(fromDateVal).format('YYYY-MM-DD');
        const formatToDate = moment(toDateVal).format('YYYY-MM-DD');

        loadBaoCaoKiemPL(formatFromDate, formatToDate, $("#select-filer").val());
    }
}
function loadBaoCaoKiemPL(fromDate, toDate, result) {
    const loaderWrapper = document.getElementById('customLoaderWrapper');
    const progressBar = document.getElementById('customProgressBar');

    if (!loaderWrapper || !progressBar) {
        console.error("Thiếu phần tử customLoaderWrapper hoặc customProgressBar.");
        return;
    }

    // Hiện loader
    loaderWrapper.classList.add('active');
    progressBar.style.width = '0%';
    progressBar.setAttribute('data-percentage', '0%');

    let progress = 0;
    const interval = setInterval(() => {
        if (progress < 90) {
            progress++;
            progressBar.style.width = progress + '%';
            progressBar.setAttribute('data-percentage', progress + '%');
        }
    }, 30);

    $.ajax({
        url: '/api/QtyKiemPL/GET',
        type: 'GET',
        dataType: 'json',
        data: {
            action: 'GetBaoCaoNgay',
            para1: fromDate, 
            para2: toDate,
            para3: result
        },
        success: function (res) {
            
            initGrid(res);
        },
        error: function (err) {
            console.error("Lỗi lấy dữ liệu: ", err);
        },
        complete: function () {
            clearInterval(interval);
            progressBar.style.width = '100%';
            progressBar.setAttribute('data-percentage', '100%');

            setTimeout(() => {
                loaderWrapper.classList.remove('active');
                progressBar.style.width = '0%';
                progressBar.setAttribute('data-percentage', '100%');
            }, 800); // hiệu ứng fade-out
        }
    });
}

function initGrid(data) {
    $("#headerInfo").empty();
    let countPass = data.filter(x => x.Result == 1).length;
    let countFail = data.filter(x => x.Result == 0).length;


    $("#headerInfo").html(`
  <div class="stats-header">
    <div class="stat-card total">
      <span class="stat-label">Tổng kiểm</span>
      <span class="stat-value">${data.length || 0}</span>
      <div class="stat-bar"></div>
    </div>
    <div class="stat-card pass">
      <span class="stat-label">Pass</span>
      <span class="stat-value">${countPass || 0}</span>
      <div class="stat-bar"></div>
    </div>
    <div class="stat-card fail">
      <span class="stat-label">Fail</span>
      <span class="stat-value">${countFail || 0}</span>
      <div class="stat-bar"></div>
    </div>
    <div class="stat-card percent">
      <span class="stat-label">% Lỗi</span>
      <span class="stat-value">${data.length == 0 ? '0.00' : ((countFail / data.length) * 100).toFixed(2) || '0.00'}%</span>
      <div class="stat-bar"></div>
    </div>
  </div>
`);

    $("#grvPhuLieuKiemNgay").dxDataGrid({
        dataSource: data,
        showBorders: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        hoverStateEnabled: true,
        height: "calc(115vh - 230px)",

        scrolling: {
            mode: "standard", 
            showScrollbar: "always",
            useNative: true 
        },
        paging: {
            enabled: false
        },
        headerFilter: { visible: false },
        stateStoring: {
            enabled: false,
            type: "localStorage",
            storageKey: "storageBaoCaoKiemPL"
        },
        filterRow: { visible: true },
        searchPanel: {
            visible: true,
            width: 250,
            placeholder: "Tìm kiếm tổng hợp..."
        },


        columns: [          
            {
                caption: "STT",
                alignment: "center",
                width: 50,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {
                    container.text(options.rowIndex + 1 + (options.component.pageIndex() * options.component.pageSize()));
                },
            },
            {
                dataField: "NgayKiem",
                caption: "Ngày Kiểm",
                dataType: "date",
                format: "dd/MM/yyyy",
                alignment: "center",
                width: 120
            },
            {
                dataField: "POMua",
                caption: "PO Mua",
                width: 120
            },
            {
                dataField: "SoLo",
                caption: "Số Lô",
                width: 120
            },
            {
                dataField: "TenKH",
                caption: "Khách Hàng",
                minWidth: 100
            },
            {
                dataField: "TenCL",
                caption: "Chủng Loại",
                width: 120
            },
            {
                dataField: "ItemCode",
                caption: "ItemCode",
                width: 120
            },
            
            {
                dataField: "MauVT",
                caption: "Màu",
                alignment: "center",
                width: 100
            },
            {
                dataField: "MoTa",
                caption: "Mô Tả",
                minWidth: 200
            },
            {
                dataField: "KhoVai",
                caption: "Khổ/Size",
                alignment: "center",
                width: 100
            },
            {
                dataField: "TenDVVT",
                caption: "Đơn Vị",
                alignment: "center",
                width: 90
            },
            { dataField: "Dot", caption: "Đợt", minWidth: 70, width: 70 },
            {
                dataField: "Result",
                caption: "Kết Quả",
                alignment: "center",
                width: 100,
                cellTemplate: function (container, options) {
                    var val = options.value;
                    if (val === 1 || val === true || val === "Pass") {
                        $("<span class='text-success fw-bold'><i class='fas fa-check-circle'></i> Pass</span>").appendTo(container);
                    } else if (val === 0 || val === false || val === "Fail") {
                        $("<span class='text-danger fw-bold'><i class='fas fa-times-circle'></i> Fail</span>").appendTo(container);
                    } else {
                        $("<span></span>").appendTo(container);
                    }
                }
            },
            {
                dataField: "LyDo",
                caption: "Lý Do Lỗi",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    if (options.value) {
                        $("<span class='text-danger'></span>").text(options.value).appendTo(container);
                    }
                }
            },
             {
                caption: "Xem Chi TIết",
                alignment: "center",
                width: 110,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    const rowData = options.data;

                    if (rowData && rowData.SoLoID && rowData.MaNPL) {
                        const targetUrl = `/QtyKiemPL/PhieuKiem?SoLoID=${encodeURIComponent(rowData.SoLoID)}&MaNPL=${encodeURIComponent(rowData.MaNPL)}&isView=true`;
                        $("<button>")
                            .addClass("btn btn-sm btn-outline-primary")
                            .attr("title", "Xem chi tiết")
                            .html("<i class='fas fa-eye'></i>")
                            .on("click", function () {
                                window.open(targetUrl, '_blank');
                            })
                            .appendTo(container);
                    }
                }
            }
        ],

        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                var isFail = e.data.Result === 0 || e.data.Result === false || (e.data.Result && e.data.Result.toString().toLowerCase() === "fail");
                if (isFail) {
                    e.rowElement.css("background-color", "rgba(255, 0, 0, 0.05)");
                }
            }
        }
    });
}

$("#select-filer").on("change", function () {
    onDateRangeChange();

})