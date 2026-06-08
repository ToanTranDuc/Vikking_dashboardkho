var currentDate = new Date();
var day = currentDate.getDate();
var month = currentDate.getMonth() + 1;
var year = currentDate.getFullYear();
/*var formattedDate = year + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;*/

let mahang = "";
let ngay = "";
let mau = "";

let arrNhapKho = [];

// set ui date = ngày hiện tại
/*$('#date, #date1').val(formattedDate);*/
// khi thay đổi giá trị ngà thì load lại mã hàng
$('#date, #date1').on("change", function () {
    viewmahang();
});

/*fortmat datetime*/
function formatShotDate(dateTime) {
    const date = new Date(dateTime);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    // Trả về định dạng YYYY-MM-DD
    return `${year}-${month}-${day}`;
}


function getDateTimeNow() {
    var currentDate = new Date();
    var day = currentDate.getDate();
    var month = currentDate.getMonth() + 1;
    var year = currentDate.getFullYear();
    var hours = currentDate.getHours();
    var minutes = currentDate.getMinutes();
    var seconds = currentDate.getSeconds();


    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;


    var formattedDateTime = day + '-' + month + '-' + year + '-' + hours + minutes + seconds;
    return formattedDateTime;
}


// load danh sách mã hàng
function viewmahang() {
    arrNhapKho = new Array();
    let fromDate = $("#date").val();
    let toDate = $("#date1").val();
    let BranchID = sessionStorage.getItem("BranchId");
    let response = RunStored_GetData_DH(
        BranchID,
        "SP_DB_BaoCaoNhapKhoTP",
        `@Action = 'Get', @Para1 = '${formatStrYMD(fromDate)}', @Para2= '${formatStrYMD(toDate)}', @Para3 = '${BranchID}', @Para4 = '', @Para5 = ''`
    );

    if (!response) {

        Swal.fire({
            title: 'Cảnh báo',
            text: 'Chưa có dữ liệu',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
        $("#tbody").empty();
        $("#thear1").empty();
        clearChart();
        fechChar(null);
        return;
    }
    arrNhapKho = [...response]
    FetchSelected_MH(response)
    renderTable(response);
    Mergecell();
}

function handleDate(_date) {
    var date = new Date(_date);
    return ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '-' +
        ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' +
        date.getFullYear();
}

// tạo ui danh sách mã hàng 
function renderTable(data) {

    if (data.length > 0) {
        let html = "";
        mahang = data[0].MaHang;
        ngay = data[0].NgayNhapKho;
        data.forEach((item, i) => {
            let ngaythang = handleDate(item.NgayNhapKho);

            html += `
                    <tr class="${i === 0 ? "active4" : ""}" 
                        data-mahang="${item.MaHang}" 
                        data-ngay="${item.NgayNhapKho}"
                         dot="${item.Dot}"              >
                        <td style="background:#fff">${item.TenHang}</td>
                        <td>${item.Dot}</td>
                        <td>${ngaythang}</td>
                        <td>${item.SLKH}</td>                
                        <td>${item.SLSPNK}</td>
                        <td>${item.SoThung}</td>
                  
                    </tr>`;
        });
        $("#tbody").html(html);
        $("#tbody").find('tr').eq(0).click();
        // table();
    } else {
        $("#tbody").empty();
        $("#thear1").empty();
        Swal.fire({
            title: 'Cảnh báo',
            text: 'Chưa có dữ liệu',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
    }
}
// 
function Mergecell() {
    const previousRow = {};
    const colsChanged = {};
    const colsToMerge = [0];
    let leftMerged = false;
    let dark = false;

    $('#tbody tr').each(function (rowIdx, tr) {
        $(tr).children().each(function (colIdx, td) {
            const tdText = $(td).text();
            if (rowIdx > 0 && (colIdx === 0 || leftMerged) && previousRow[colIdx].text === tdText && colsToMerge.includes(colIdx)) {
                previousRow[colIdx].elem.attr('rowspan', ++previousRow[colIdx].span);
                colsChanged[colIdx] = false;
                $(td).remove();
                if (colIdx === 0) {
                    leftMerged = true;
                }
            } else {
                previousRow[colIdx] = { span: 1, text: tdText, elem: $(td), dark };
                colsChanged[colIdx] = true;
            }
        });

        const rowChanged = Object.values(colsChanged).every(Boolean);
        dark = rowChanged && rowIdx > 0 ? !dark : dark;
        if (dark) {
            $(tr).addClass('dark');
        }
        leftMerged = false;
    });
}
//click bảng để cập nhật mã hàng
$("#tbody").on('click', 'tr', function () {
    $("#tbody").find("tr").removeClass("active4");
    $(this).addClass("active4");
    mahang = $(this).data('mahang');
    ngay = $(this).data('ngay');
    const dot = $(this).attr('dot');
    table(ngay, mahang, dot);  // Cập nhật biểu đồ sau khi click vào hàng
});
//load chart 
function table(ngay, mahang, dot) {
    let BranchID = sessionStorage.getItem("BranchId");
    let response = RunStored_GetData_DH(
        BranchID,
        "SP_DB_BaoCaoNhapKhoTP",
        `@Action = 'GetCT', @Para1 = '${formatShotDate(ngay)}', @Para2= '${formatShotDate(ngay)}', @Para3 = '${mahang}', @Para4 = '${dot}', @Para5 = '${BranchID}'`
    );
    console.log(response);

    if (response == null || response.length === 0) {
        clearChart();
        fechChar(null, ngay);
        return;
    }

    fechChar(response, ngay);
}
//xoa chart 
function clearChart() {
    if (Highcharts.charts.length > 0 && Highcharts.charts[0]) {
        Highcharts.charts[0].destroy();  // Xóa biểu đồ
    }
}


function fechChar(d, ngay) {
    const arrCategories = [];
    const arrAmountOfProduct = [];
    const arrAmountPlanProduct = [];
    const arrCartonAmount = [];

    if (d) {
        for (const x of d) {
            arrCategories.push('Size ' + x.Size);
            arrCartonAmount.push(+ x.SLThung);

            if (!x.Size.includes(',')) {
                arrAmountPlanProduct.push(+x.SLKH);
                arrAmountOfProduct.push(+x.SoLuongSP);
            }

        }
    }


    /*draw chart*/

    ///chart 4 Ammount
    Highcharts.chart('Amount-Chart-Container', {
        chart: {
            type: 'column',
            backgroundColor: '#f0f0f0',
            margin: [60, 40, 60, 80],
            spacingTop: 10,
            spacingBottom: 10
        },
        title: {
            text: `Biểu đồ Nhập kho - Mã hàng: ${mahang} - Ngày: ${!ngay ? '' : handleDate(ngay)}`,
            align: 'center',
            style: {
                fontSize: '16px',
                fontWeight: 'bold'
            }
        },

        xAxis: {
            categories: arrCategories.filter(x => !x.includes(',')),
            crosshair: true,

            labels: {
                style: {
                    fontSize: '11px',
                    color: '#333333'
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Số Lượng Sản Phẩm',
                style: {
                    color: '#FF0000',
                    fontSize: '13px',
                    fontWeight: 'bold'
                }
            },
            labels: {
                style: {
                    fontSize: '11px',
                    color: '#333333'

                },


            },
            gridLineWidth: 0.5,
            gridLineColor: '#d0d0d0'
        },
        tooltip: {
            shared: true,
            useHTML: true,
            formatter: function () {
                return `<b>${this.x}</b><br/>` +
                    `Số Lượng Kế Hoạch: ${this.points[1].y} <br/>` +
                    `Số Lượng Nhập Kho: ${this.points[0].y}`;
            }
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    style: {
                        fontSize: '11px',
                        color: '#000000'
                    }
                }
            },
            series: {
                borderRadius: 5
            }
        },
        legend: {
            align: 'right',
            verticalAlign: 'top',
            layout: 'horizontal',
            y: -15,
            margin: 10

        },
        series: [
            {
                name: 'Số Lượng Sản Phẩm',
                type: 'column',
                data: arrAmountOfProduct,
                color: '#0071A7'
            },
            {
                name: 'Số Lượng Kế Hoạch',
                type: 'line',
                data: arrAmountPlanProduct,
                color: '#FF0000',
                marker: {
                    enabled: true,
                    radius: 3,
                    symbol: 'circle'
                },
                lineWidth: 2
            }
        ],
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        enabled: false
                    },
                    yAxis: {
                        labels: {
                            enabled: false
                        }
                    },
                    xAxis: {
                        labels: {
                            rotation: -45,
                            align: 'right'
                        }
                    }
                }
            }]
        }
    });

    // ///chart 4 Carton
    Highcharts.chart('Carton-Chart-Container', {
        chart: {
            type: 'column',
            backgroundColor: '#f0f0f0',
            margin: [60, 40, 60, 80],
            spacingTop: 10,
            spacingBottom: 10
        },
        title: {
            text: `Biểu đồ Nhập kho - Mã hàng: ${mahang} - Ngày: ${!ngay ? '' : handleDate(ngay)} Theo Số Kiện`,
            align: 'center',
            style: {
                fontSize: '16px',
                fontWeight: 'bold'
            }
        },

        xAxis: {
            categories: arrCategories,
            crosshair: true,
            accessibility: {
                description: 'Size'
            },
            labels: {
                style: {
                    fontSize: '11px',
                    color: '#333333'

                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Số Lượng Kiện',
                style: {
                    color: '#FF0000',
                    fontSize: '13px',
                    fontWeight: 'bold'
                }
            },
            labels: {
                style: {
                    fontSize: '11px',
                    color: '#333333'
                }
            },
            gridLineWidth: 0.5,
            gridLineColor: '#d0d0d0'
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    style: {
                        fontSize: '11px',
                        color: '#000000'
                    }
                }
            },
            series: {
                borderRadius: 5
            }
        },
        tooltip: {
            shared: true,
            useHTML: true,
            headerFormat: '<b>{point.key}</b><br>',
            pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b><br/>'
        },
        legend: {
            align: 'right',
            verticalAlign: 'top',
            layout: 'horizontal',
            y: -15,
            margin: 10

        },
        series: [
            {
                name: 'Số Kiện',
                data: arrCartonAmount,
                color: '#0071A7'  // Màu sắc nổi bật cho cột
            }
        ],
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        enabled: false
                    },
                    yAxis: {
                        labels: {
                            enabled: false
                        }
                    },
                    xAxis: {
                        labels: {
                            rotation: -45,
                            align: 'right'
                        }
                    }
                }
            }]
        }
    });
}

/*export excel*/
$("#btn-excel").on("click", function () {
    let fromDate = formatStrYMD($("#date").val());
    let toDate = formatStrYMD($("#date1").val());
    let BranchID = sessionStorage.getItem("BranchId");
    const objPara = {
        BranchID: BranchID,
        FromDate: fromDate,
        ToDate: toDate
    };


    var url = `/api/BCNhapKhoTP/ExportExcel`;
    var fileName = `Báo Cáo Nhập Kho TP ${getDateTimeNow()}`
    if (sessionStorage.getItem("IsCefShap") == null) {
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(objPara),
        })
            .then(response => {
                if (!response.ok) {
                    //Toast.fire({
                    //    icon: 'error',
                    //    title: 'Lỗi Không thể xuất excel . Vui lòng thử lại!'
                    //})
                    return;

                }

                return response.blob();
            })
            .then(blob => {
                var a = document.createElement("a");
                var url = window.URL.createObjectURL(blob);
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            })
            .catch(error => {
                console.error('Error fetching data from the server.', error);
            });
    } else {
        console.log(`ExportExcel@${JSON.stringify(objPara)}`);
    }

})

/*fetch selected 2 MH*/
function FetchSelected_MH(data) {
    $('#selected-mh').empty();
    var html = '<option value="all" data-ss="all">Tất cả</option>'
    if (data) {
        data.forEach(item => {

            html += `<option value='${item.MaHang}' data-ss='${item.Dot}'>${item.TenHang}-SS:${item.Dot}</option>`
        })
        $('#selected-mh').html(html);
    }
}


$('#selected-mh').on("change", function () {
    var arrFilter = [...arrNhapKho]
    const MH = $(this).val();
    const Dot = $('#selected-mh option:selected').data('ss');
    if (MH != 'all' && Dot != 'all') {
        arrFilter = arrFilter.filter(x => x.MaHang == MH && x.Dot == Dot);
    }
    renderTable(arrFilter);
    Mergecell();

})

$(document).ready(function () {
    localStorage.removeItem("ajax_data-1-meta");
    if (sessionStorage.getItem("BranchId") == null) { }
    else {
        if (sessionStorage.getItem("IsCefShap") != null)
            $('#BarMenu').hide();
        $('#selected-mh').select2({
            placeholder: 'Chọn Mã Hàng',
            allowClear: false
        });

        viewmahang()
    }

});
