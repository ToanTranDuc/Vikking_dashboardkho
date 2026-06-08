var userName = "";
var fromdate = '', todate = '';
var year = 2025;
$(document).ready(function () {
    userName = localStorage.getItem('username');
    GetYear();
    $("#btnExport").click(Export)
});

function GetYear() {
    com.ajax({
        type: 'GET',
        data: {},
        url: '/api/PhuLieu/GetPhuLieu?action=GetYear',
        success: function (d) {
            var data = d.dt1;
            $.each(data, function (key, item) {
                $('#Year').append($('<option></option>').val(item.Year).html(item.Year))
            });
            //$('#Year').append($('<option></option>').val('2024').html('2024'));
            year = $('#Year').val();
            GetDate();
        }
    })
}
function YearChange() {
    year = $('#Year').val();
    BindingWeek();
}
function GetDate() {
    $.ajax({
        type: "Get",
        url: "/api/PhuLieu/GetDate",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (d) {
            $("#Ngay").attr('value', d);
            BindingWeek();
            LocChange();
        }
    });
}
function LocChange() {
    var loc = $('#Loc').val();
    switch (loc) {
        case "Ngay":
            $('.ngay').css('display', '');
            $('.list_tuan').css('display', 'none');
            NgayChange();

            break;
        case "Tuan":
            $('.ngay').css('display', 'none');
            $('.list_tuan').css('display', '');
            WeekChange();
            break;
        case "MH":
            fromdate = todate = '2000-01-01';
            GetMaHang();
            break;
    }
}
function NgayChange() {
    fromdate = todate = $("#Ngay").val().split("-").reverse().join("-");
    GetMaHang();
}
function GetMaHang() {
    $('#MaHang').html('');
    var post = {
        action: 'GetMHBC',
        para1: fromdate,
        para2: todate,
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            var data = d.dt1;
            if (data.length == 0) {
                $('#MaHang').html('');
                $('#BodyListChecked').html('');
                $("#SignManager").attr('src', `/Images/DefaultSign.png`);
                $("#SignNVKiem").attr('src', `/Images/DefaultSign.png`);
                return;
            }
            $.each(data, function (key, items) {
                $('#MaHang').append($(`<option data-mahang=${items.MaHang} data-dot=${items.DotNhap} data-dotdisplay=${items.DotDisplay} data-ngaynhap=${items.NgayNhap} ></option>`).val(items.MaDH).html(items.MaHangDisplay));
            });
            MaHangChange();
        }
    });
}
function MaHangChange() {
    GetData();
}
let arrBody = []
function GetData() {
    var post = {
        action: 'GetDataBC',
        para1: $('#MaHang').val(),
        para2: $("#MaHang option:selected").data("mahang"),
        para3: $("#MaHang option:selected").data("dot")
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            var data = d.dt1;
            var html = '';
            arrBody = data
            $.each(data, function (key, item) {
                html += `<tr>
                        <td>${key + 1}</td>
                        <td>${item.TenVT}</td>
                        <td>${item.QuiCach}</td>
                        <td>${item.DVTinh}</td>
                        <td>${item.SoLuong}</td>
                        <td>${item.SoLuongThuc}</td>
                        <td>${item.Balance ?? ""}</td>
                        <td>${item.SoLuongKiem}</td>
                        <td>${item.SoLuongLoi}</td>
                        <td>${item.TLLoi}</td>
                        <td>${item.TenLoi}</td>
                        <td>${item.KetQua}</td>
                        </tr>`
            });
            $('#BodyListChecked').html(html);
            var drDataT = data[0];
            $("#KhachHang").val(drDataT.TenKH);
            document.getElementById('GhiChu').innerHTML = drDataT.GhiChu;
            if (drDataT.SignManager != null) {
                $("#SignManager").attr('src', `/Images/SignNguyenPhuLieu/${drDataT.SignManager}`);
            }
            else {
                $("#SignManager").attr('src', `/Images/DefaultSign.png`);
            }
            if (drDataT.SignNVKiem != null) {
                $("#SignNVKiem").attr('src', `/Images/SignNguyenPhuLieu/${drDataT.SignNVKiem}`);
            }
            else {
                $("#SignNVKiem").attr('src', `/Images/DefaultSign.png`);
            }
        }
    });
}

//Sign
var imageSign, idNguoiKy, scrollPosition;
function CallModal(id) {
    idNguoiKy = id;
    scrollPosition = window.pageYOffset || document.documentElement.scrollTop;
    $('#ModalSign').modal('show');
    const myTimeout = setTimeout(CalCanavas, 500);
}
function CalCanavas() {
    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
    setCanvasBackground();
}
saveImg.addEventListener("click", () => {
    const link = document.createElement("a"); // creating <a> element
    link.download = `${Date.now()}.jpg`; // passing current date as link download value
    imageSign = canvas.toDataURL();
    $("#" + idNguoiKy).attr('src', imageSign);
    SaveSign(idNguoiKy);
    $('#ModalSign').modal('hide');
    const myTimeout = setTimeout(Scroll, 500);
})
function Scroll() {
    window.scrollTo(0, scrollPosition);
}
function SaveSign() {
    var url = '/api/PhuLieu/SaveSign';
    var post = {
        IdNguoiKy: idNguoiKy,
        MaDH: $('#MaHang').val(),
        MaHang: $("#MaHang option:selected").data("mahang"),
        Dot: $("#MaHang option:selected").data("dot"),
        Image: imageSign,
        NVKiem: userName,
    };
    com.ajax({
        type: 'POST',
        url: url,
        data: JSON.stringify(post),
        success: function (d) {

        }
    });

}

function BindingWeek() {
    var lstWeek = getAllWeekNumbersInYear(year);
    lstWeek.map((x, index) => {
        $('#Week').append($('<option></option>').val(x).html(x))
    });
    var weekCurrent = getWeekNumber(parseDateFromDDMMYYYY($("#Ngay").val()));
    $('#Week').val(weekCurrent);
}
function WeekChange() {
    const { startOfWeek, endOfWeek } = getStartAndEndOfWeek(year, parseInt($('#Week').val()));
    fromdate = formatDateToYYYYMMDD(startOfWeek);
    todate = formatDateToYYYYMMDD(endOfWeek);
    console.log("Ngày bắt đầu của tuần:", fromdate);
    console.log("Ngày cuối của tuần:", todate);
    GetMaHang();
}
function getAllWeekNumbersInYear(year) {
    const weekNumbers = [];
    const firstDayOfYear = new Date(year, 0, 1); // Lấy ngày đầu tiên của năm
    const lastDayOfYear = new Date(year, 11, 31); // Lấy ngày cuối cùng của năm

    let currentDate = new Date(firstDayOfYear);
    currentDate.setDate(firstDayOfYear.getDate() + (7 - firstDayOfYear.getDay())); // Di chuyển đến ngày bắt đầu của tuần thứ 1

    while (currentDate <= lastDayOfYear) {
        const weekNumber = getWeekNumber(currentDate);
        weekNumbers.push(weekNumber);

        currentDate.setDate(currentDate.getDate() + 7); // Di chuyển đến ngày bắt đầu của tuần tiếp theo
    }

    return weekNumbers;
}
function parseDateFromDDMMYYYY(dateString) {
    const [day, month, year] = dateString.split('-').map(Number);
    return new Date(year, month - 1, day); // Lưu ý: Tháng trong Date bắt đầu từ 0
}
function getWeekNumber(date) {
    // Hàm tính số tuần dựa trên ngày
    const firstDayOfYear = new Date(date.getFullYear(), 0, 1);
    const dayOfYear = Math.floor((date - firstDayOfYear) / 86400000); // 86400000 milliseconds trong một ngày
    return Math.ceil((dayOfYear + firstDayOfYear.getDay() + 1) / 7);
}
function getStartAndEndOfWeek(year, weekNumber) {
    const januaryFirst = new Date(year, 0, 1);
    const daysToAdd = (weekNumber - 1) * 7;
    const startDate = new Date(januaryFirst);
    startDate.setDate(januaryFirst.getDate() + daysToAdd - januaryFirst.getDay() + 1); // Lấy ngày bắt đầu tuần
    const endDate = new Date(startDate);
    endDate.setDate(startDate.getDate() + 5); // Lấy ngày cuối tuần

    return {
        startOfWeek: startDate,
        endOfWeek: endDate,
    };
}
function formatDateToYYYYMMDD(date) {
    const year = date.getFullYear();

    const month = String(date.getMonth() + 1).padStart(2, '0'); // Thêm số 0 phía trước nếu tháng < 10
    const day = String(date.getDate()).padStart(2, '0'); // Thêm số 0 phía trước nếu ngày < 10
    return `${year}-${month}-${day}`;
}
function renderImage() {
    return new Promise((resolve, reject) => {
        var post = {
            action: 'GetDataBC',
            para1: $('#MaHang').val(),
            para2: $("#MaHang option:selected").data("mahang"),
            para3: $("#MaHang option:selected").data("dot")
        }
        $.ajax({
            type: "GET",
            url: '/api/PhuLieu/GetPhuLieu',
            data: ko.toJS(post),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (d) {
                resolve(d.dt1); // Resolve dữ liệu khi thành công
            },
            error: function (err) {
                reject(err); // Reject khi có lỗi
            }
        });
    });
}
async function Export() {
    let dataImage = await renderImage()

    let arrThongSo = await [
        $("#KhachHang").val(),
        $("#MaHang option:selected").data("mahang"),
        $("#GhiChu").text(),
        dataImage[0]?.SignManager,
        dataImage[0]?.SignNVKiem,
        $("#MaHang option:selected").data("dotdisplay"),
        $("#MaHang option:selected").data("ngaynhap"),
    ]
    let dataToSend = JSON.stringify({
        ArrBody: arrBody,
        ArrThongSo: arrThongSo
    });

    var url = `/api/PhuLieu/EXBCNPhuLieu`;
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: dataToSend,
    }).then(response => {

        return response.blob();
    }).then(blob => {
        var a = document.createElement("a");
        var url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = 'BaoCaoNPhuLieu.xlsx';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
    })

}