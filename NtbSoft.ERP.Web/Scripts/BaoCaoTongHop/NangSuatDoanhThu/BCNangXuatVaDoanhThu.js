$(document).ready(function () {
    localStorage.removeItem("ajax_data-1-meta");
    if (sessionStorage.getItem("BranchId") == null) { }
    else {
        if (sessionStorage.getItem("IsCefShap") != null)
            $('#BarMenu').hide();
        loadDataStarView();
    }
})

function DanhSachNSVaDT(fromdate, todate, action) {
    isQC = 1;
    let BranchID = sessionStorage.getItem("BranchId");
    let response = RunStored_GetData(BranchID, "SP_BCNangSuatVaDoanhThu", `@Action = 'GET' , @FromDate = '${fromdate}', @ToDate = '${todate}', @UserName = '${action}'`);

    console.log(response, "DanhSachMaHang");

    if (response == null || response.length == 0) {
        $("#bodyContent").html('');
        $("#footContent").html('');
        let datanull = [];
        Chart(datanull.map(x => [x.Name, x.TiLeLoi]), "container", "Tỉ lệ lỗi", "Tỉ lệ lỗi: <b>{point.y:.1f} %</b>")
        Chart(datanull.map(x => [x.Name, x.DT_LKHT]), "container1", "Doanh thu lũy kế", "Doanh thu: <b>{point.y:.1f} $</b>")
        Swal.fire({
            title: 'Thông báo',
            text: 'Không có dữ liệu ở thời gian được chọn!',
            icon: 'info',
            confirmButtonText: 'OK'
        });
        return;
    }

    let ArrTiLeLoi = []

    let html = ""
    let tfoot = ""
    let SLDat = 0
    let SLLoi = 0
    let TiLeLoi = 0
    let UI_TH = 0
    let UI_LK = 0
    let Ton_UI = 0
    let GX_TH = 0
    let GX_LK = 0
    let Ton_TPHT = 0
    let DT_THHT = 0
    let DT_LKHT = 0
    response.map((item, index) => {
        ArrTiLeLoi.push(item.Name, item.TiLeLoi)

        html += `
                <tr>
                    <td>${item.Name}</td>
                    <td>${item.SLDat}</td>
                    <td>${item.SLLoi}</td>
                    <td>${item.TiLeLoi.toFixed(2)}</td>
                    <td>${item.UI_TH}</td>
                    <td>${item.UI_LK}</td>
                    <td>${item.Ton_UI}</td>
                    <td>${item.GX_TH}</td>
                    <td>${item.GX_LK}</td>
                    <td>${item.Ton_TPHT}</td>
                    <td>${item.DT_THHT}</td>
                    <td>${item.DT_LKHT.toFixed(2)}</td>
                </tr>
                `
        SLDat += item.SLDat
        SLLoi += item.SLLoi
        TiLeLoi += item.TiLeLoi
        UI_TH += item.UI_TH
        UI_LK += item.UI_LK
        Ton_UI += item.Ton_UI
        GX_TH += item.GX_TH
        GX_LK += item.GX_LK
        Ton_TPHT += item.Ton_TPHT
        DT_THHT += item.DT_THHT
        DT_LKHT += item.DT_LKHT


    })
    tfoot = `<tr>
                <td style="text-align:center">Tổng</td>
                <td>${SLDat}</td>
                <td>${SLLoi}</td>
                <td>${TiLeLoi.toFixed(2)}</td>
                <td>${UI_TH}</td>
                <td>${UI_LK}</td>
                <td>${Ton_UI}</td>
                <td>${GX_TH}</td>
                <td>${GX_LK}</td>
                <td>${Ton_TPHT}</td>
                <td>${DT_THHT}</td>
                <td>${DT_LKHT.toFixed(2)}</td>
            </tr>`
    $("#bodyContent").html(html)
    $("#footContent").html(tfoot)
    Chart(response.map(x => [x.Name, x.TiLeLoi]), "container", "Tỉ lệ lỗi", "Tỉ lệ lỗi: <b>{point.y:.1f} %</b>")
    Chart(response.map(x => [x.Name, x.DT_LKHT]), "container1", "Doanh thu lũy kế", "Doanh thu: <b>{point.y:.1f} $</b>")

}
// vẽ biểu đồ
function Chart(data, containerId, title, pointFormat) {

    let minValue = Math.min(...data.map(item => item[1]));
    let maxValue = Math.max(...data.map(item => item[1]));
    Highcharts.chart(containerId, {
        chart: {
            type: 'column'
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
                    fontFamily: 'Arial, sans-serif'
                }
            }
        },
        yAxis: {
            min: minValue,
            max: maxValue,
            title: {
                text: 'Giá trị'
            }
        },
        legend: {
            enabled: false
        },
        tooltip: {
            pointFormat: pointFormat//'Doanh thu: <b>{point.y:.1f} $</b>'
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
                align: 'center',
                format: '{point.y:.1f}', // one decimal
                style: {
                    fontSize: '13px',
                    fontFamily: 'Arial, sans-serif'
                }
            }
        }]
    });
}

function loadDataStarView() {
    showLoading();
    try {
        let toDateValue = $("#todate").val();
        let fromDate = formatStrYMD(toDateValue);
        let todate = formatStrYMD(toDateValue);
        DanhSachNSVaDT(fromDate, todate, 'GetData');
        hideLoading();
    } catch (error) {
        hideLoading();
        showPopupError();
    }

}
// sự kiện nút tìm 
$("#btnFind").on("click", function () {
    showLoading();
    try {
        if ($("#date").prop("checked")) {
            let toDateValue = $("#todate").val();
            let fromDate = formatStrYMD(toDateValue);
            let toDate = formatStrYMD(toDateValue);
            DanhSachNSVaDT(fromDate, toDate, 'GetData');
            hideLoading();
        }
        else {
            let fromDate = getFirstDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
            let toDate = getLastDayOfMonth(($("#byYear").val()), parseInt($("#byMonth").val()));
            DanhSachNSVaDT(fromDate, toDate, 'GetData');
            hideLoading();
        }
    } catch (error) {
        hideLoading();
        showPopupError();
    }
});
// sự kiện nút xuất excel
$("#btnXuatExcel").on('click', function () {
    let data_lst = [];
    $('#bodyContent tr').each(function () {
        let Chuyen = $(this).find('td').eq(0).text();
        let SLDat = $(this).find('td').eq(1).text();
        let SLLoi = $(this).find('td').eq(2).text();
        let TiLeLoi = $(this).find('td').eq(3).text();
        let Ui_TH = $(this).find('td').eq(4).text();
        let Ui_LK = $(this).find('td').eq(5).text();
        let Ui_UUi = $(this).find('td').eq(6).text();
        let DG_TH = $(this).find('td').eq(7).text();
        let DG_LK = $(this).find('td').eq(8).text();
        let DG_TonTPHT = $(this).find('td').eq(9).text();
        let DT_TH = $(this).find('td').eq(10).text();
        let DT_LK = $(this).find('td').eq(11).text();

        let data_item = { Chuyen: Chuyen, SLDat: SLDat, SLLoi: SLLoi, TiLeLoi: TiLeLoi, Ui_TH: Ui_TH, Ui_LK: Ui_LK, Ui_UUi: Ui_UUi, DG_TH: DG_TH, DG_LK: DG_LK, DG_TonTPHT: DG_TonTPHT, DT_TH: DT_TH, DT_LK: DT_LK };
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
    let url = `/api/XuatExcelBaoCaoNangXuatDoanhThu/ExportExcel`;
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
                a.download = 'BaoCaoNangSuatDoanhThu.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            })
    } else {
        console.log(`ExportExcel@${dataToSend}`);
    }
});
