var currentDate = new Date();
var day = currentDate.getDate();
var month = currentDate.getMonth() + 1;
var year = currentDate.getFullYear();
var formattedDate = year + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
let userName = localStorage.getItem('username');// sessionStorage.getItem("UserNameLogging");

$(document).ready(function () {
    sessionStorage.setItem("BranchId", "");
    sessionStorage.setItem("BranchName", "");
    setSoLieu(formattedDate, formattedDate, userName);
    $('#fromDate').val(formattedDate);
    $('#toDate').val(formattedDate);
    $('#btnXemBaoCaoCT').on('click', function () {
        window.location = '../../bao-cao-tong-hop/bao-cao-tong-quan-san-xuat-tgi';
    });
    $('.date-ip').on('change', function () {
        let fromDate = $('#fromDate').val();
        let toDate = $('#toDate').val();

        if (!isValidDate(fromDate) || !isValidDate(toDate)) {
            alert('Please enter valid dates in the format YYYY-MM-DD.');
            return;
        }
        fromDate = formatDate(fromDate);
        toDate = formatDate(toDate);
        setSoLieu(fromDate, toDate, userName);
    });
});

$('#info-tgi-th1').on('click', function () {
    let isTruyCap = getUserBranch(userName, 'DVSX_1');
    if (isTruyCap != true) {
        Swal.fire({
            title: 'Cảnh báo',
            text: 'Tài khoản của bạn không có quyền truy cập đến đơn vị này!',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
        return;
    }

    sessionStorage.setItem("BranchId", "DVSX_1");
    sessionStorage.setItem("BranchName", "Tân Hương 1");
    location.href = '../../../danh-sach-bao-cao';
});
$('#info-tgi-th2').on('click', function () {
    let isTruyCap = getUserBranch(userName, 'DVSX_5');
    if (isTruyCap != true) {
        Swal.fire({
            title: 'Cảnh báo',
            text: 'Tài khoản của bạn không có quyền truy cập đến đơn vị này!',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
        return;
    }
    sessionStorage.setItem("BranchId", "DVSX_5");
    sessionStorage.setItem("BranchName", "Tân Hương 2");
    location.href = '../../../danh-sach-bao-cao';
});
$('#info-tgi-cg').on('click', function () {
    let isTruyCap = getUserBranch(userName, 'DVSX_2');
    if (isTruyCap != true) {
        Swal.fire({
            title: 'Cảnh báo',
            text: 'Tài khoản của bạn không có quyền truy cập đến đơn vị này!',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
        return;
    }
    sessionStorage.setItem("BranchId", "DVSX_2");
    sessionStorage.setItem("BranchName", "Chợ Gạo");
    location.href = '../../../danh-sach-bao-cao';
});
$('#info-tgi-cl').on('click', function () {
    let isTruyCap = getUserBranch(userName, 'DVSX_3');
    if (isTruyCap != true) {
        Swal.fire({
            title: 'Cảnh báo',
            text: 'Tài khoản của bạn không có quyền truy cập đến đơn vị này!',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
        return;
    }
    sessionStorage.setItem("BranchId", "DVSX_3");
    sessionStorage.setItem("BranchName", "Cai Lậy");
    location.href = '../../../danh-sach-bao-cao';
});
function getUserBranch(username, branch) {
    let result = getRoleDVSX(branch);
    return result;
}
function setSoLieu(fromdate, todate, userName) {
    let result = getData(fromdate, todate, userName);
    if (result == null || result.length == 0) {
        Swal.fire({
            title: 'Cảnh báo',
            text: `Dữ liệu từ ngày trống hoặc kết nối lỗi!`,
            icon: 'warning',
            confirmButtonText: 'OK'
        });
        return;
    }
    $('#TH1-DT').text(result[0].DoanhThu);
    $('#TH2-DT').text(result[1].DoanhThu);
    $('#CG-DT').text(result[2].DoanhThu);
    $('#CL-DT').text(result[3].DoanhThu);
    $('#TGI-DT').text(result[4].DoanhThu);

    $('#TH1-NS').text(result[0].NangSuat);
    $('#TH2-NS').text(result[1].NangSuat);
    $('#CG-NS').text(result[2].NangSuat);
    $('#CL-NS').text(result[3].NangSuat);
    $('#TGI-NS').text(result[4].NangSuat);

    $('#TH1-SLDg').text(result[0].SoLaoDong);
    $('#TH2-SLDg').text(result[1].SoLaoDong);
    $('#CG-SLDg').text(result[2].SoLaoDong);
    $('#CL-SLDg').text(result[3].SoLaoDong);
    $('#TGI-SLDg').text(result[4].SoLaoDong);

    $('#TH1-SLL').text(result[0].SLLoi);
    $('#TH2-SLL').text(result[1].SLLoi);
    $('#CG-SLL').text(result[2].SLLoi);
    $('#CL-SLL').text(result[3].SLLoi);
    $('#TGI-SLL').text(result[4].SLLoi);

    $('#TH1-SLD').text(result[0].SuaDat);
    $('#TH2-SLD').text(result[1].SuaDat);
    $('#CG-SLD').text(result[2].SuaDat);
    $('#CL-SLD').text(result[3].SuaDat);
    $('#TGI-SLD').text(result[4].SuaDat);

    $('#TH1-TLL').text(result[0].TLLoi);
    $('#TH2-TLL').text(result[1].TLLoi);
    $('#CG-TLL').text(result[2].TLLoi);
    $('#CL-TLL').text(result[3].TLLoi);
    let tll = (result[4].SLLoi / (result[4].SuaDat + result[4].SLLoi)).toFixed(4);
    $('#TGI-TLL').text(isNaN(tll) ? 0 : tll);

    chartSet();
}

function getData(fromdate, todate, userName) {
    let sqlParam = `@Action = 'Get', @para1 = '${fromdate}', @para2 = '${todate}', @para3 = '${userName}'`
    let DataJS = {
        server: '',
        stored: '',
        paramStr: sqlParam
    };
    let result = '';
    $.ajax({
        url: '/api/BaoCaoTongQuan/GetData',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(DataJS),
        async: false,
        success: function (data) {
            result = data;
        },
        error: function () {
            console.error('Lỗi khi lấy dữ liệu');
        }
    });
    return result;
}
function isValidDate(dateString) {
    // Regular expression to validate YYYY-MM-DD format
    const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
    return dateRegex.test(dateString);
}

function formatDate(dateString) {
    const dateParts = dateString.split('-');
    const year = dateParts[0];
    const month = padNumber(parseInt(dateParts[1]));
    const day = padNumber(parseInt(dateParts[2]));
    return `${year}-${month}-${day}`;
}

function padNumber(number) {
    return (number < 10 ? '0' : '') + number;
}
function checkPQ(userName, branchID) {
    let dataResult = [];
    let DataJS = {
        server: userName,
        stored: branchID,
        paramStr: ''
    };

    $.ajax({
        url: '/api/user/RunStored',
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

function chartSet() {
    let DT = [parseInt($('#TH1-DT').text()), parseInt($('#TH2-DT').text()), parseInt($('#CG-DT').text()), parseInt($('#CL-DT').text())];
    let NS = [parseInt($('#TH1-NS').text()), parseInt($('#TH2-NS').text()), parseInt($('#CG-NS').text()), parseInt($('#CL-NS').text())];
    let SLD = [parseInt($('#TH1-SLDg').text()), parseInt($('#TH2-SLDg').text()), parseInt($('#CG-SLDg').text()), parseInt($('#CL-SLDg').text())];
    let SLL = [parseInt($('#TH1-SLL').text()), parseInt($('#TH2-SLL').text()), parseInt($('#CG-SLL').text()), parseInt($('#CL-SLL').text())];
    let SD = [parseInt($('#TH1-SLD').text()), parseInt($('#TH2-SLD').text()), parseInt($('#CG-SLD').text()), parseInt($('#CL-SLD').text())];
    // Biểu đồ đầu tiên
    Highcharts.chart('container', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Tổng Quan'
        },
        xAxis: {
            categories: ['Tân Hương 1', 'Tân Hương 2', 'Chợ Gạo', 'Cai Lậy']
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Số lượng'
            }
        },
        series: [{
            name: 'Doanh Thu',
            data: DT
        }, {
            name: 'Năng suất',
            data: NS
        }, {
            name: 'Số lao động',
            data: SLD
        }, {
            name: 'Số lượng lỗi',
            data: SLL
        }, {
            name: 'Sửa đạt',
            data: SD
        }]
    });
}
$('#btn-XuatBC-TH').on('click', function () {
    let ndung = sessionStorage.getItem('UserNameLogging');

    let url = `/api/XuatExcelBaoCaoTienDoTongHop/ExportExcel?userName=${ndung}`;
    fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        // body: dataToSend,
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
            a.download = 'BaoCaoTienDoTongHopCacKhu.xlsx';
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        });
});