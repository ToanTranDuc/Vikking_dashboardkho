let data = [];
let ArrDoanhThuMaLenh = []
var fromDateValue = "", toDateValue = "";

let ArrLine = []
let arrbody = []

function chart(data) {
    // Dữ liệu cho các mã hàng
    // Chuẩn bị dữ liệu cho biểu đồ
    var categories = data.map(item => item.maHang);
    var seriesData = [
        {
            name: 'Số lượng kiểm',
            data: data.map(item => item.TongKiem)
        },
        {
            name: 'Số lượng đạt',
            data: data.map(item => item.SPDAT)
        },
    ];

    // Vẽ biểu đồ cột
    Highcharts.chart('container', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Biểu đồ số lượng'
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

function chartTyLe(data) {
    // Dữ liệu cho các mã hàng
    // Chuẩn bị dữ liệu cho biểu đồ
    var categories = data.map(item => item.maHang);
    var seriesData = [
        {
            name: 'Tỉ lệ đạt',
            data: data.map(item => item.TyLeDat),
            color: '#4CAF50'
        },
        {
            name: 'Tỉ lệ lỗi',
            data: data.map(item => item.TiLeLoi),
            color: '#F44336' // Màu đỏ
        }

    ];

    // Vẽ biểu đồ cột
    Highcharts.chart('container1', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Biểu đồ tỉ lệ'
        },
        xAxis: {
            categories: categories
        },
        yAxis: {
            title: {
                text: 'Giá trị'
            },
            min: 0,
            max: 100
        },
        series: seriesData,
        categories: ['xaxis']
    });
}
function functionGroup(id) {
    var listtr = document.getElementsByClassName('trTable')
    var count = 0;


    stt = id.split('-')[1];
    if (document.getElementById('trTable-' + stt).style.display == 'none') {
        document.getElementById('img-' + stt).setAttribute('src', '../Content/Images/chevron-down-solid.svg');
        document.getElementById('trTable-' + stt).removeAttribute('style')
        for (var i = parseInt(stt) + 1; i < listtr.length; i++) {
            if (document.getElementById('grouptr-' + i) == null) {
                document.getElementById('trTable-' + i).removeAttribute('style')
                document.getElementById('trTable-' + i).setAttribute('style', 'transition: all 4s ease-out')

            }
            else {
                return;
            }
        }
    } else {
        document.getElementById('trTable-' + stt).setAttribute('style', 'display:none')
        document.getElementById('img-' + stt).setAttribute('src', '../Content/Images/chevron-right-solid.svg');
        for (var i = parseInt(stt) + 1; i < listtr.length; i++) {
            if (document.getElementById('grouptr-' + i) == null) {
                document.getElementById('trTable-' + i).setAttribute('style', 'display:none; transition: width 2s, height 4s;')
            }
            else {
                return;
            }
        }
    }
}

function GetChuyenSauUi(fromdate, todate, action, chuyen) {

    isQC = 1;
    let BranchID = sessionStorage.getItem("BranchId");


    let response = RunStored_GetData(BranchID, "sp_ReportLoiCuoiChuyen", `@Action = 'GetChuyenP' ,
                                            @FromDate = '${fromdate}', @ToDate = '${todate}', @UserID = '',
                                            @IsQC = '${isQC}', @Chuyen = '${chuyen}', @MaHang = ''`);
    let html = "";

    if (response == null || response.length == 0) {
        $("#slc_line").html('');
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
            <option value=${item.LineX}>${item.Name}</option>
            `
    })

    $("#slc_line").html(html)
    renderTable(fromdate, todate, 'GetAfterPress', response.length > 0 ? response[0].LineX : "")
}

function renderTable(fromdate, todate, action, chuyen) {
    isQC = 1;
    let BranchID = sessionStorage.getItem("BranchId");
    let response = RunStored_GetData(BranchID, "sp_ReportLoiCuoiChuyen", `@Action = 'GetAfterPress' , @FromDate = '${fromdate}', @ToDate = '${todate}', @UserID = '', @IsQC = '${isQC}', @Chuyen = '${chuyen}', @MaHang = ''`);

    if (response == null) {
        $("#bodyContent").html('');
        let data = [];
        chart(data);
        chartTyLe(data);
        tableData = []; // Đặt lại dữ liệu khi không có phản hồi
        Swal.fire({
            title: 'Thông báo',
            text: 'Không có dữ liệu ở thời gian được chọn!',
            icon: 'info',
            confirmButtonText: 'OK'
        });
        return;
    }

    console.log(response, "DanhSachMaHang")
    let html = "";
    let data = [];
    response.map((item, index) => {
        html += `
            <tr>
                <td>${item['MaHang']}</td>
                <td>${item['NgayKiemDisplay']}</td>
                <td>${item.TongKiem}</td>
                <td>${item.SPDAT}</td>
                <td>${checkZeroAndNull(item['1'])}</td>
                <td>${checkZeroAndNull(item['2'])}</td>
                <td>${checkZeroAndNull(item['3'])}</td>
                <td>${checkZeroAndNull(item['4'])}</td>
                <td>${checkZeroAndNull(item['5'])}</td>
                <td>${checkZeroAndNull(item['6'])}</td>
                <td>${checkZeroAndNull(item['7'])}</td>
                <td>${checkZeroAndNull(item['8'])}</td>
                <td>${checkZeroAndNull(item['9'])}</td>
                <td>${checkZeroAndNull(item['10'])}</td>
                <td>${checkZeroAndNull(item['11'])}</td>
                <td>${checkZeroAndNull(item['12'])}</td>
                <td>${item['TyLeDat']}</td>
                <td>${(100 - item['TyLeDat']).toFixed(2)}</td>
                <td>${item['NguoiKiemTra']}</td>
                <td>${item['GhiNhanLoi']}</td>
            </tr>
        `;
        data.push({
            maHang: item['MaHang'] + '_' + item['Name'],
            TongKiem: item['TongKiem'],
            SPDAT: item['SPDAT'],
            TyLeDat: item['TyLeDat'],
            TiLeLoi: item['TiLeLoi']
        });
    });

    chart(data);
    chartTyLe(data);
    $("#bodyContent").html(html);
    Mergecell();
    arrBody = response;
    // Lưu dữ liệu vào biến toàn cục
    tableData = response; // Lưu dữ liệu vào biến toàn cục
}


function checkZeroAndNull(value) {
    if (value == null || value == 0) {
        return "--"
    }
    return value;
}


function Mergecell() {
    const previousRow = {};
    const colsChanged = {};
    const colsToMerge = [0, 18];
    let leftMerged = false;
    let dark = false;
    //(colIdx === 0 || leftMerged)
    Array.from(document.querySelectorAll('#bodyContent tr')).forEach((tr, rowIdx) => {
        Array.from(tr.children).forEach((td, colIdx) => {
            if (rowIdx > 0 && (colIdx === 0 || leftMerged) && previousRow[colIdx].text === td.innerText && colsToMerge.includes(colIdx)) {
                previousRow[colIdx].elem.setAttribute('rowspan', ++previousRow[colIdx].span);
                colsChanged[colIdx] = false;
                td.remove();
                if (colIdx === 0) {
                    leftMerged = true;
                }
            } else {
                previousRow[colIdx] = { span: 1, text: td.innerText, elem: td, dark };
                colsChanged[colIdx] = true;
            }
        });
        const rowChanged = Object.values(colsChanged).every(Boolean);
        dark = rowChanged && rowIdx > 0 ? !dark : dark;
        if (dark) {
            tr.classList.add('dark');
        }
        leftMerged = false;
    });
}


$(document).ready(function () {
    showLoading();
    try {
        localStorage.removeItem("ajax_data-1-meta")

        if (sessionStorage.getItem("BranchId") == null) { }
        else {
            if (sessionStorage.getItem("IsCefShap") != null)
                $('#BarMenu').hide();

            GetChuyenSauUi(formatStrYMD($('.fromdate').eq(0).val()), formatStrYMD($('.todate').eq(0).val()), 'GetChuyenP', "")


            $("#btnFind").on("click", function () {
                showLoading();
                try {
                    fromDateValue = formatStrYMD($('.fromdate').eq(0).val());
                    toDateValue = formatStrYMD($('.todate').eq(0).val());
                    renderTable(fromDateValue, toDateValue, 'Get_TLL_MaHang_Chuyen', $("#slc_line").val());
                    hideLoading();
                } catch (error) {
                    hideLoading();
                }
            });
            hideLoading();
        }
    } catch (error) {
        hideLoading();
    }
})


$("#btnExport").on("click", xuatexcel)
function xuatexcel() {

    if (arrBody.length == 0) {
        alert('không có data');
        return;
    }

    let data_reponse_lst = [];

    $('#bodyContent tr').each(function () {
        let tdCount = $(this).find('td').length;

        let MaHang = "";
        let NgayKiemDisplay = "";
        let TongKiem = "";
        let SPDAT = "";
        let Loi1 = "";
        let Loi2 = "";
        let Loi3 = "";
        let Loi4 = "";
        let Loi5 = "";
        let Loi6 = "";
        let Loi7 = "";
        let Loi8 = "";
        let Loi9 = "";
        let Loi10 = "";
        let Loi11 = "";
        let Loi12 = "";
        let TyLeDat = "";
        let TiLeLoi = "";
        let NguoiKiemTra = "";
        let GhiNhanLoi = "";

        // Điều kiện nếu có đủ 20 td
        if (tdCount >= 20) {
            MaHang = $(this).find('td').eq(0).text();
            NgayKiemDisplay = $(this).find('td').eq(1).text();
            TongKiem = $(this).find('td').eq(2).text();
            SPDAT = $(this).find('td').eq(3).text();

            Loi1 = $(this).find('td').eq(4).text();
            Loi2 = $(this).find('td').eq(5).text();
            Loi3 = $(this).find('td').eq(6).text();
            Loi4 = $(this).find('td').eq(7).text();
            Loi5 = $(this).find('td').eq(8).text();
            Loi6 = $(this).find('td').eq(9).text();
            Loi7 = $(this).find('td').eq(10).text();
            Loi8 = $(this).find('td').eq(11).text();
            Loi9 = $(this).find('td').eq(12).text();
            Loi10 = $(this).find('td').eq(13).text();
            Loi11 = $(this).find('td').eq(14).text();
            Loi12 = $(this).find('td').eq(15).text();
            TyLeDat = $(this).find('td').eq(16).text();
            TiLeLoi = $(this).find('td').eq(17).text();
            NguoiKiemTra = $(this).find('td').eq(18).text();
            GhiNhanLoi = $(this).find('td').eq(19).text();
        }
        // Điều kiện nếu có đúng 18 td
        else if (tdCount == 18) {
            NgayKiemDisplay = $(this).find('td').eq(0).text();
            TongKiem = $(this).find('td').eq(1).text();
            SPDAT = $(this).find('td').eq(2).text();

            Loi1 = $(this).find('td').eq(3).text();
            Loi2 = $(this).find('td').eq(4).text();
            Loi3 = $(this).find('td').eq(5).text();
            Loi4 = $(this).find('td').eq(6).text();
            Loi5 = $(this).find('td').eq(7).text();
            Loi6 = $(this).find('td').eq(8).text();
            Loi7 = $(this).find('td').eq(9).text();
            Loi8 = $(this).find('td').eq(10).text();
            Loi9 = $(this).find('td').eq(11).text();
            Loi10 = $(this).find('td').eq(12).text();
            Loi11 = $(this).find('td').eq(13).text();
            Loi12 = $(this).find('td').eq(14).text();
            TyLeDat = $(this).find('td').eq(15).text();
            TiLeLoi = $(this).find('td').eq(16).text();
            GhiNhanLoi = $(this).find('td').eq(17).text();
        }

        let item = {
            MaHang: MaHang,
            NgayKiemDisplay: NgayKiemDisplay,
            TongKiem: TongKiem,
            SPDAT: SPDAT,
            NguoiKiemTra: NguoiKiemTra,
            GhiNhanLoi: GhiNhanLoi,
            TyLeDat: TyLeDat,
            TiLeLoi: TiLeLoi,
            Loi1: Loi1,
            Loi2: Loi2,
            Loi3: Loi3,
            Loi4: Loi4,
            Loi5: Loi5,
            Loi6: Loi6,
            Loi7: Loi7,
            Loi8: Loi8,
            Loi9: Loi9,
            Loi10: Loi10,
            Loi11: Loi11,
            Loi12: Loi12
        };

        data_reponse_lst.push(item);
    });


    let fromDateValue = formatStrYMD($(".fromdate").eq(0).val());
    let toDateValue = formatStrYMD($(".todate").eq(0).val());
    let chuyen = $("#slc_line").val();

    let dataToSend = JSON.stringify({
        FromDate: fromDateValue,
        ToDate: toDateValue,
        Chuyen: chuyen,
        DataBC: data_reponse_lst,
    });

    console.log('data', dataToSend);
    var url = '/api/BaoCaoSauUi/ExPost';
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
                        title: 'Lỗi Không thể xuất excel. Vui lòng thử lại!'
                    });
                    return;
                }
                return response.blob();
            })
            .then(blob => {
                var a = document.createElement("a");
                var url = window.URL.createObjectURL(blob);
                a.href = url;
                a.download = 'BaoCaoChatLuongMaHangSauUi.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            });
    } else {
        console.log(`ExportExcel@${dataToSend}`);
    }
}
