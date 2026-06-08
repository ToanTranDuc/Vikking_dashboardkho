let data = [];
let ArrDoanhThuMaLenh = []
var fromDateValue = "", toDateValue = "";
var currentDate = new Date();
var year = currentDate.getFullYear();
var month = String(currentDate.getMonth() + 1).padStart(2, '0');
var day = String(currentDate.getDate()).padStart(2, '0');
var formattedDate = year + '-' + month + '-' + day;
var datatable = []
var oneWeekAgo = new Date(currentDate);
var oneDayAgo = new Date(currentDate);
var oneMonthAgo = new Date(currentDate);
let ChatLuong = 1;
let isQC = 1; // Giá trị mặc định của isQC
oneDayAgo.setDate(oneWeekAgo.getDate() - 1);
oneWeekAgo.setDate(oneWeekAgo.getDate() - 7);
oneMonthAgo.setDate(oneMonthAgo.getDate() - 30);

function formatYMD(currentDate) {
    let year = currentDate.getFullYear();
    let month = String(currentDate.getMonth() + 1).padStart(2, '0'); // January is 0, so we add 1 to get the correct month.
    let day = String(currentDate.getDate()).padStart(2, '0');

    // Create the final result string in the format 'YYYY-MM-DD'
    let result = `${year}-${month}-${day}`;
    return result;
}

function formattedNUmber(value) {
    return value.toLocaleString('en-US');
}

function setYear() {
    let BranchID = sessionStorage.getItem("BranchId");
    let response = RunStored_GetData(BranchID, "sp_ReportChatLuongThang", `@Action = 'getnam' , @FROMDATE = '',@TODATE ='' ,@UserID = '' ,@IsQC = '' `);
    let html = ``
    response.map(item => {
        if (html.includes(item.NamLamViec) == false) {
            html += `
             <option value="${item.NamLamViec}">${item.NamLamViec}</option>
        `
        }
    })
    $("#byYear").html(html)
}

function chart(data) {
    var categories = data == null ? [] : data.map(item => item.maHang);
    var truocui = data == null ? [] : [data.map(item => item.luyKe)]
    var sauui = data == null ? [] : [data.map(item => item.SLKH)]
    Highcharts.chart('container', {
        chart: {
            zoomType: 'xy',
            type: 'column'
        },
        title: {
            text: 'Biểu đồ phần trăm tỉ lệ lỗi trước ủi và sau ủi',
            align: 'center'
        },
        subtitle: {
            align: 'left'
        },
        xAxis: [{
            categories: categories,
            crosshair: true
        }],
        yAxis: [{ // Primary yAxis
            labels: {
                format: '{value} %',
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            },
            title: {
                text: 'Trước ủi',
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            }
        }, { // Secondary yAxis
            title: {
                text: 'Sau ủi',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            labels: {
                format: '{value} %',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            opposite: true
        }],
        tooltip: {
            shared: true
        },
        legend: {
            align: 'left',
            x: 0,
            verticalAlign: 'top',
            y: 0,
            floating: true,
            backgroundColor:
                Highcharts.defaultOptions.legend.backgroundColor || // theme
                'rgba(255,255,255,0.25)'
        },
        series: [{
            name: 'Sau ủi',
            type: 'column',
            yAxis: 1,
            data: sauui[0],
            align: 'center',
            tooltip: {
                valueSuffix: '%'
            }

        }, {
            name: 'Trước ủi',
            type: 'line',
            data: truocui[0],
            align: 'center',
            tooltip: {
                valueSuffix: '%'
            }
        }]
    });

    if (categories && categories.length > 0) {
        const count_category = categories.length;
        const height = count_category * 40;
        //alert(height)
        $(".kt-portlet #container .highcharts-container ").css({ "height": height + "px" });
    }
}

function handleDate(datetimeStr) {
    const date = new Date(datetimeStr);
    return date.toISOString().split('T')[0];
}

function renderTable(fromdate, todate, action, isQC) {
    /*    isQC = 1;*/
    let BranchID = sessionStorage.getItem("BranchId");
    let response = GetData_BCTongHop(BranchID, "sp_ReportChatLuongThang", `@Action = 'GetReport' , @FROMDATE = '${fromdate}',@TODATE ='${todate}' ,@UserID = '' ,@IsQC = '${isQC}' `);
    console.log(response, "DanhSachMaHang");
    if (response == null || response.length == 0) {
        hideLoading();
        $('#bodyContent').html('');
        chart(null);
        Swal.fire({
            title: 'Thông báo',
            text: 'Không có dữ liệu ở thời gian được chọn!',
            icon: 'info',
            confirmButtonText: 'OK'
        });

        return;
    }
    let html = ""
    let data = []
    if (response != null) {
        response.map(item => {
            let formattedLoiHuNoiCom = item['LoiHuNoiCom'].replace(/\n/g, "<br>");
            let mahang = item['Mã hàng'].split("-")[1]
            let malenh = item['Mã hàng'].split("-")[0]
            html += `
                    <tr>
                    <td>${item['Chuyền']}</td>
                    <td>${malenh}</td>
                    <td>${mahang}</td>
                    <td>${item['TL lỗi']}</td>
                    <td>${item['TL lỗi sau ủi']}</td>
                    <td style="min-width: 270px;">${formattedLoiHuNoiCom}</td>
                    <td>${item.StatusKiem}</td>
                    </tr>
                    `
            data.push({ maHang: item['Mã hàng'] + '_' + item['Chuyền'], luyKe: item['TL lỗi'], SLKH: item['TL lỗi sau ủi'] })
        })
    }
    chart(data)
    $("#bodyContent").html(html)

}

function getLastDayOfMonth(year, month) {
    const nextMonth = month < 12 ? month : 1;
    const nextMonthFirstDay = new Date(year, nextMonth, 1);
    const lastDay = new Date(nextMonthFirstDay.getTime() - 1);
    return data = {
        fromdate: `${year}-${month}-01`,
        todate: `${year}-${month}-${lastDay.getDate()}`
    }
}

$(document).ready(function () {
    showLoading();
    try {
        localStorage.removeItem("ajax_data-1-meta");
        if (sessionStorage.getItem("BranchId") == null) { }
        else {
            setYear();
            if (sessionStorage.getItem("IsCefShap") != null)
                $('#BarMenu').hide();

            let getdate = $('#todate').val();
            renderTable(formatStrYMD(getdate), formatStrYMD(getdate), 'GetReport', ChatLuong);
            $("#byLine").on("change", function () {
                isQC = $(this).val(); // Cập nhật giá trị isQC dựa trên lựa chọn của người dùng
            });
            hideLoading();
        }

    } catch (error) {
        hideLoading();
    }
})

$("#btnFind").on("click", function () {
    if ($("#loctheo").val() == "ngay") {
        toDateValue = $("#todate").val();
        renderTable(formatStrYMD(toDateValue), formatStrYMD(toDateValue), 'GetReport', isQC);
    }
    else {
        fromDateValue = ""
        const date = getLastDayOfMonth(parseInt($("#byYear").val()), parseInt($("#byMonth").val()))
        renderTable(date.fromdate, date.todate, 'GetReport', isQC)
    }
});
$("#loctheo").on("change", function () {
    if ($(this).val() == "ngay") {
        $(".item_ngay").removeClass("active")
        $(".item_listThang").addClass("active")
    }
    else {
        $(".item_ngay").addClass("active")
        $(".item_listThang").removeClass("active")
    }

})
function GetData_BCTongHop(server, stored, paramStr) {
    let dataResult = [];
    let DataJS = {
        server: server,
        stored: stored,
        paramStr: paramStr
    };

    $.ajax({
        url: '/api/sqlQueryBaoCao/BaoCaoChatLuong',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(DataJS),
        async: false, // Thiết lập xử lý đồng bộ
        success: function (data) {
            dataResult = data; // Gán trực tiếp kết quả vào dataResult
        },
        error: function () {
            // Xử lý lỗi ở đây, ví dụ: hiển thị thông báo lỗi
            console.error('Lỗi khi lấy dữ liệu');
        }
    });

    return dataResult;
}
function FormatStrDate(date) {
    let year = date.getFullYear();
    let month = (date.getMonth() + 1).toString().padStart(2, '0'); // Tháng bắt đầu từ 0
    let day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
}
function FormatStrDate2(date) {
    return date.split('/')[2] + '-' + date.split('/')[1] + '-' + date.split('/')[0];
}
$('#btn-XuatExcel').on('click', function () {
    let fromDate;
    let toDate;
    let locTheo = $('#loctheo').val();
    if (locTheo == "ngay") {
        let toDateValue = $("#todate").val();
        fromDate = formatStrYMD(toDateValue);
        toDate = formatStrYMD(toDateValue);
    }
    else {
        fromDate = getFirstDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
        toDate = getLastDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
    }

    let data_lst = [];
    $('#bodyContent tr').each(function () {
        let To = $(this).find('td').eq(0).text();
        let MaLenh = $(this).find('td').eq(1).text();
        let MaHang = $(this).find('td').eq(2).text();
        let TruocUi = $(this).find('td').eq(3).text();
        let SauUi = $(this).find('td').eq(4).text();
        let LoiHuNoiCom = $(this).find('td').eq(5).text();
        let KetQua = $(this).find('td').eq(6).text();
        let data_item = {
            To: To,
            MaLenh: MaLenh,
            MaHang: MaHang,
            TruocUi: TruocUi,
            SauUi: SauUi,
            LoiHuNoiCom: LoiHuNoiCom,
            KetQua: KetQua
        };
        data_lst.push(data_item);
    });

    if (data_lst.length == 0) {
        Swal.fire({
            title: 'Cảnh báo',
            text: 'KHông có dữ liệu ở thời gian được chọn!',
            icon: 'info',
            confirmButtonText: 'OK'
        });
    }
    let loctheo;
    let locval = $('#locIsQC').val();
    if (locval == 0) {
        loctheo = 'QA'
    } else if (locval == 1) {
        loctheo = 'QC'
    } else {
        loctheo = 'TQC'
    }
    let url = `/api/XuatExcelBaoCaoChatLuongMaHangTungChuyen/ExportExcel`;
    let dataToSend = JSON.stringify({
        ChatLuong: loctheo,
        TuNgay: fromDate,
        DenNgay: toDate,
        BaoCaoBC: data_lst,
    });
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
                a.download = 'BAoCaoChatLuongMaHangTrenTungChuyen.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            })
    } else {
        console.log(`ExportExcel@${dataToSend}`);
    }
});
