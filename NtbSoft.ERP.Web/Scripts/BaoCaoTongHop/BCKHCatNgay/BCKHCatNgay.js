let data = [];
let ArrDoanhThuMaLenh = []
var fromDateValue = "", toDateValue = "";

let arrBody = []
function Chart(data, containerId, title, value) {
    Highcharts.chart(containerId, {

        chart: {
            type: 'column'
        },
        title: {
            text: title
        },
        subtitle: {
            text: `Tổng doanh thu: ${value}$`
        },
        xAxis: {
            type: 'category',
            labels: {
                autoRotation: [-45, -90],
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Giá trị'
            }
        },
        legend: {
            enabled: false
        },
        tooltip: {
            pointFormat: 'Doanh thu: <b>{point.y:.1f} $</b>'
        },
        series: [{
            name: 'Population',
            colors: [
                '#9b20d9', '#9215ac', '#861ec9', '#7a17e6', '#7010f9', '#691af3',
                '#6225ed', '#5b30e7', '#533be1', '#4c46db', '#4551d5', '#3e5ccf',
                '#3667c9', '#2f72c3', '#277dbd', '#1f88b7', '#1693b1', '#0a9eaa',
                '#03c69b', '#00f194'
            ],
            colorByPoint: true,
            groupPadding: 0,
            data: data,
            dataLabels: {
                enabled: true,
                rotation: -90,
                color: '#FFFFFF',
                align: 'right',
                format: '{point.y:.1f}', // one decimal
                y: 10, // 10 pixels down from the top
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }
        }]
    });
}

function chart1(data) {
    // Dữ liệu cho các mã hàng
    // Chuẩn bị dữ liệu cho biểu đồ
    var categories = data.map(item => item.maHang);
    var seriesData = [
        {
            name: 'Lũy kế',
            data: data.map(item => item.luyKe)
        },
        {
            name: 'SLKH',
            data: data.map(item => item.SLKH)
        }
    ];

    // Vẽ biểu đồ cột
    Highcharts.chart('container1', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Biểu đồ Lũy kế và SLKH'
        },
        xAxis: {
            categories: categories
        },
        yAxis: {
            title: {
                text: 'Giá trị'
            }
        },
        series: seriesData,
        categories: ['xaxis']
    });
}

function DanhSachMaHang(fromdate, todate) {

    return new Promise(async (resolve, reject) => {
        try {

            const param = `@Action='GET', @fromDate='${fromdate}', @toDate='${todate}',@UserName='Admin'`;


            const response = await ExecProc("SP_BCThucHienCatNgayTheoTungMaHang", param);

            console.log(response, "DanhSachMaHang");
            let dataNull = [];

            //Chart(dataNull);
            //chart1(dataNull);
            if (response == null) {
                $('#bodyContent').html('');
                Swal.fire({
                    title: 'Thông báo',
                    text: 'Không có dữ liệu ở thời gian được chọn!',
                    icon: 'info',
                    confirmButtonText: 'OK'
                });
                return;
            }
            let html = ""
            let tfoot = ""
            let slCut = 0
            let slLK = 0
            let slKH = 0
            response.map((item, index) => {
                html += `
                <tr>
                    <td>${index + 1}</td>
                    <td>${item.MaHang}</td>
                    <td>${item.SLCut_TH}</td>
                    <td>${item.SLCut_LK}</td>
                    <td>${item.SLKH}</td>
                </tr>
                `
                slCut += item.SLCut_TH
                slLK += item.SLCut_LK
                slKH += item.SLKH
            })
            tfoot = `<tr>
                <td colspan="2" style="text-align:center">Tổng</td>
                <td>${slCut}</td>
                 <td>${slLK}</td>
                 <td>${slKH}</td>
            </tr>`
            $("#bodyContent").html(html)
            $("#footContent").html(tfoot)

            resolve(response || response.length > 0 ? response : []);

            let dataBDTRon = [];
            let dataBDCot = [];
            response.map(function (item, index) {
                let items = [];
                items.push(item.MaHang);
                items.push(item.SLCut_TH);
                dataBDCot.push({ maHang: item.MaHang, luyKe: item.SLCut_LK, SLKH: item.SLKH });
                dataBDTRon.push(items);
            })


            renderPie(dataBDTRon, 'pieDTKhachHang', "Biểu đồ cắt ngày theo mã hàng", fromdate, todate, '');
            chart1(dataBDCot);
            arrBody = response

        } catch (error) {
            reject(error);
        }
    });



}

function renderPie(data, idTag, title, fromdate, todate, khachhang) {

    Highcharts.chart(idTag, {
        chart: {
            type: 'pie'
        },
        title: {
            text: title,
            align: 'left'
        },
        tooltip: {
        },
        plotOptions: {
            pie: {
                dataLabels: {
                    enabled: true,
                    distance: 20
                },
                tooltip: {
                    pointFormat: ' SL <b>{point.y}</b>'
                }
            },
            series: {
                point: {
                    events: {
                        click: function () {
                            var color = this.color;
                            console.log("Màu fill của điểm:", color);
                            maKho = this.customAttr;
                            renderChartLine(color);
                        }
                    }
                },
                dataLabels: [{
                    enabled: true,
                    distance: 10
                }, {
                    enabled: true,
                    distance: -25,
                    formatter: function () {
                        return this.point.percentage > 5 ? Highcharts.numberFormat(this.percentage, 1) + '%' : null;
                    },
                    style: {
                        fontSize: '13px',
                        textOutline: 'none',
                        opacity: 0.7
                    }, tooltip: {
                        pointFormat: ' SL <b>{point.y}</b>'
                    }
                }]
            }
        },
        series: [{
            name: 'SL',
            colorByPoint: true,
            data: data
        }]
    });

    function renderChartLine(color) {
        // Your implementation for renderChartLine function
        console.log("Render chart line with color:", color);
    }
}

async function ExecProc(spName, Para) {
    try {
        let BranchId = sessionStorage.getItem("BranchId");
        let response = await RunStored_GetData(BranchId, spName, Para);
        return response;
    } catch (error) {
        console.error("Error executing stored procedure:", error);
        throw error;
    }
}
/*event func*/

$("#btnFind").on("click", function () {

    let toDateValue = formatStrYMD($("#todate").val());
    Promise.all([
        DanhSachMaHang(toDateValue, toDateValue)])
});


$(document).ready(async function () {
    localStorage.removeItem("ajax_data-1-meta");
    if (sessionStorage.getItem("BranchId") == null) { }
    else {
        if (sessionStorage.getItem("IsCefShap") != null)
            $('#BarMenu').hide();
        localStorage.removeItem("ajax_data-1-meta");
        let toDateValue = formatStrYMD($("#todate").val());
        await Promise.all([
            DanhSachMaHang(toDateValue, toDateValue),
        ]);
    }
});



$("#btnExportExcel").on("click", xuatexcel)

function xuatexcel() {
    let toDateValue = $("#todate").val();

    let arrDate = [{
        TuNgay: toDateValue,
        DenNgay: toDateValue
    }]
    let dataToSend = JSON.stringify({
        arrBody: arrBody,
        copyarrHeader: arrDate,
    });
    console.log(dataToSend)
    var url = '/api/BaoCaoCatNgay/ExPoster?ngay=' + toDateValue;
    if (sessionStorage.getItem("IsCefShap") == null) {
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: dataToSend,
        })
            .then(response => {
                if (!response.ok) {
                    Toast.fire({
                        icon: 'error',
                        title: 'Lỗi Không thể xuất excel . Vui lòng thử lại!'
                    })
                    return;
                }
                return response.blob();
            })
            .then(blob => {
                var a = document.createElement("a");
                var url = window.URL.createObjectURL(blob);
                a.href = url;
                a.download = 'BaoCaoKeHoachCatNgay.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            })
    } else {
        console.log(`ExportExcel@${dataToSend}`);
    }
}