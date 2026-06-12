let data = [];
var fromDateValue = "", toDateValue = "";

function Chart(data, containerId, title) {
    Highcharts.chart(containerId, {
        chart: {
            type: 'bar'
        },
        title: {
            text: title
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
            pointFormat: 'Tỉ lệ lỗi: <b>{point.y:.1f}</b>'
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

    if (data && data.length > 0) {
        const count_category = data.length;
        const height = count_category * 30;
        //alert(height)
        $(".kt-portlet #container .highcharts-container").css({ "height": height + "px" });
    }
}

function setYear() {
    $("#byYear").empty();
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
function renderTable(fromdate, todate, action) {

    let BranchID = sessionStorage.getItem("BranchId");
    let response = RunStored_GetData(BranchID,
        "sp_ReportThongKeTLLoi",
        `
        @Action = '${action}',
        @TuNgay = '${fromdate}',
        @DenNgay = '${todate}',
        @Chuyen = '', 
        @UserID = '', 
        @IsQC = '1', 
        @StatusKiemMau = ''`);

    console.log(response, "DanhSachMaHang");

    let html = "";

    if (response == null) {
        $("#bodyContent").html('');
        let data = [];
        Chart(data, 'container', 'Dữ liệu rỗng!!!',);
        Swal.fire({
            title: 'Thông báo',
            text: 'Không có dữ liệu ở thời gian được chọn!',
            icon: 'info',
            confirmButtonText: 'OK'
        });
        return;
    }


    response.map(item => {
        html += `
            <tr> 
                <td>${item.Name}</td>
                <td>${item.MaHang}</td>
                <td>${item.MaMau}</td>
                <td>${item.TongKiem}</td>
                <td>${item.SPDAT}</td>
                <td>${item.SPLOI}</td>
                <td>${item.TiLeLoi}</td>
            </tr>
        `;
    });

    Chart(response.map(item => [item.Name + '_' + item.MaHang + '_' + item.MaMau, item.TiLeLoi]), 'container', '');

    $("#bodyContent").html(html);
}

$(document).ready(function () {
    localStorage.removeItem("ajax_data-1-meta");
    if (sessionStorage.getItem("BranchId") == null) { }
    else {
        if (sessionStorage.getItem("IsCefShap") != null)
            $('#BarMenu').hide();
        let todate = $('#todate').val();
        setYear();
        // Khởi tạo hiển thị ban đầu
        renderTable(formatStrYMD(todate), formatStrYMD(todate), 'Get_TLL_MaHang_Chuyen');

        // Khi người dùng bấm vào nút "Tìm"
        $("#btnFind").on("click", function () {
            let fromDate;
            let toDate;
            if ($("#date").prop("checked")) {
                let toDateValue = $("#todate").val();
                fromDate = formatStrYMD(toDateValue);
                toDate = formatStrYMD(toDateValue);
                renderTable(fromDate, toDate, 'Get_TLL_MaHang_Chuyen');
            }
            else {
                fromDate = getFirstDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
                toDate = getLastDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
                renderTable(fromDate, toDate, 'Get_TLL_MaHang_Chuyen');
            }
        });
    }
});

$('#btnXuatExcel').on('click', function () {
    let data_lst = [];
    let fromDate;
    let toDate;
    if ($("#date").prop("checked")) {
        let toDateValue = $("#todate").val();
        fromDate = formatStrYMD(toDateValue);
        toDate = formatStrYMD(toDateValue);
    }
    else {
        fromDate = getFirstDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
        toDate = getLastDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
    }
    $('#bodyContent tr').each(function () {
        let Chuyen = $(this).find('td').eq(0).text();
        let MaHang = $(this).find('td').eq(1).text();
        let Mau = $(this).find('td').eq(2).text();
        let SoLuongKiem = $(this).find('td').eq(3).text();
        let SoLuongDat = $(this).find('td').eq(4).text();
        let SoLuongLoi = $(this).find('td').eq(5).text();
        let TiLeLoi = $(this).find('td').eq(6).text();

        let data_item = {
            Chuyen: Chuyen,
            MaHang: MaHang,
            Mau: Mau,
            SoLuongKiem: SoLuongKiem,
            SoLuongDat: SoLuongDat,
            SoLuongLoi: SoLuongLoi,
            TiLeLoi: TiLeLoi
        };
        data_lst.push(data_item);
    });

    if (data_lst.length == 0) {
        Swal.fire({
            title: 'Thông báo',
            text: 'Không có dữ liệu ở thời gian được chọn để xuất!',
            icon: 'info',
            confirmButtonText: 'OK'
        });

        return;
    }

    let url = `/api/XuatExcelBaoCaoTiLeMaHangLoiTrenChuyen/ExportExcel`;

    let dataToSend = JSON.stringify({
        TuNgay: fromDate,
        DenNgay: toDate,
        DataBC: data_lst,
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
                    Swal.fire({
                        title: 'Lỗi',
                        text: 'Xuất file báo cáo thất bại!',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                    return;
                }
                return response.blob();
            })
            .then(blob => {
                var a = document.createElement("a");
                var url = window.URL.createObjectURL(blob);
                a.href = url;
                a.download = 'BaoCaoTongHopTiLeMaHangHoiTrenChuyen.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            });
    } else {
        console.log(`ExportExcel@${dataToSend}`);
    }
});


