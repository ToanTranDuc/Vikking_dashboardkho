var lstDataMaHang = [];
var fromdate = '', todate = '';
var year = 2025;
var uerName = ""
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
            //$('#Year').append($('<option></option>').val('2025').html('2025'));
            $.each(data, function (key, item) {
                $('#Year').append($('<option></option>').val(item.Year).html(item.Year))
            });
            year = $('#Year').val();
            //$('#Year').append($('<option></option>').val('2024').html('2024'));
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
    var post = {
        action: 'GetMHBC',
        para1: fromdate,
        para2: todate,
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/KiemVai/Get',
        success: function (d) {
            $('#MaHang').html('');
            var data = d.dt1;
            lstDataMaHang = data;
            if (data.length == 0) {
                $('#BodyListChecked').html('');
                $("#SignManager").attr('src', `/Images/DefaultSign.png`);
                $("#SignNVKiem").attr('src', `/Images/DefaultSign.png`);
                return;
            }
            var listMaHang = [
                ...new Map(data.map((item) =>
                    [`${item.MaHang}-${item.DotNhap}-${item.MaDH}-${item.MaHangDisplay}-${item.TenKH}`,
                    { MaHang: item.MaHang, DotNhap: item.DotNhap, MaDH: item.MaDH, MaHangDisplay: item.MaHangDisplay, TenKH: item.TenKH }]
                )).values()
            ];
            $.each(listMaHang, function (key, items) {
                $('#MaHang').append($(`<option data-mahang=${items.MaHang} data-dot=${items.DotNhap} data-tenkh=${items.TenKH} ></option>`).val(items.MaDH).html(items.MaHangDisplay));
            });
            MaHangChange();
        }
    });
}
function MaHangChange() {
    $('#LoaiVai').html('<option value="">Tất cả</option>');
    var dtLoaiVai = lstDataMaHang.filter(x => x.MaDH == $('#MaHang').val());
    $.each(dtLoaiVai, function (key, items) {
        $('#LoaiVai').append($(`<option></option>`).val(items.MaVai).html(items.TenVT));
    });
    LoaiVaiChange();
    $('#KhachHang').val($("#MaHang option:selected").data("tenkh"))
}
function LoaiVaiChange() {
    GetData();
}
let arrKqKT = []
function GetData() {
    var post = {
        action: 'GetDataBC',
        para1: $('#MaHang').val(),
        para2: $("#MaHang option:selected").data("mahang"),
        para3: $("#MaHang option:selected").data("dot"),
        para4: $('#LoaiVai').val()
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/KiemVai/Get',
        success: function (d) {
            var data = d.dt1;
            dataLoi = d.dt2;
            var dtLoi = d.dt1;
            var html = '';
            $('#BodyListChecked').html('');
            if (data.length != 0) {
                var dtTemp = data.filter(x => x.MaVTMau == $('#MauVai').val());
                if (dtTemp.length > 0) {
                    dataBinding = dtTemp[0];
                    $('#SoCay').val(dataBinding.SoCay ?? "0");
                    $('#SoLuong').val(dataBinding.SoLuong ?? "0");
                    $('#Kho').val(dataBinding.Kho ?? "0");
                }
                else {
                    $('#SoCay').val("0");
                    $('#SoLuong').val("0");
                    $('#Kho').val("0");

                }
            }
            else {
                $('#SoCay').val("0");
                $('#SoLuong').val("0");
                $('#Kho').val("0");
            }
            var lstFieldName = Object.keys(data[0]);
            var lengthColspan = 0;
            let indexCheck = 0;
            $.each(data, function (index, item) {
                var ToTalDiem = 0;
                var lengthColspan_Temp = 0;
                var htmlVitri = '', htmlLoaiLoi = '', htmlDiemLoi = '';
                lstFieldName.map((x, index) => {
                    //if (x.includes('@')) {
                    //    htmlVitri += ` <td></td>`
                    //    htmlLoaiLoi += item[x] != "0" ? ` <td>${x.split('@')[0]}</td>` : `<td></td>`;
                    //    htmlDiemLoi += ` <td>${item[x] == "0" ? "" : item[x]}</td>`
                    //    ToTalDiem += parseInt(item[x]);
                    //    lengthColspan_Temp++;
                    //}
                    if (x.includes('@')) {
                        var diemLoi = item[x].split('@')[0];
                        var vitriLoi = item[x].split('@')[1];
                        htmlVitri += vitriLoi != "" ? ` <td>${vitriLoi}</td>` : `<td></td>`;
                        htmlLoaiLoi += vitriLoi != "0" ? `<td>${x.split('@')[0]}</td>` : `<td></td>`;
                        htmlDiemLoi += ` <td>${diemLoi == "0" ? "" : diemLoi}</td>`
                        ToTalDiem += parseInt(diemLoi);
                        lengthColspan_Temp++;
                        //htmlVitri += ` <td></td>`
                        //htmlLoaiLoi += item[x] != "0" ? ` <td>${x.split('@')[1]}</td>` : `<td></td>`;
                        //htmlDiemLoi += ` <td>${item[x] == "0" ? "" : item[x]}</td>`
                        //ToTalDiem += parseInt(item[x]);
                        //lengthColspan_Temp++;
                    }

                })
                let value = item.ChiTiet
                let htmltd = ``
                if (indexCheck == 0) {
                    indexCheck = checkRowSpan(data, index, value)
                    htmltd = `<td style="max-width:150px" rowspan="${indexCheck * 3}">${item.ChiTiet}</td>`
                }

                var result = HandleResult(ToTalDiem, item.DVTinh, item.SoLuongTT, item.Kho)
                item.KetQua = result;
                html += `<tr>
                        ${htmltd}
                        <td  rowspan="3">${item.TenMau}</td>
                        <td  rowspan="3">${item.SoCay}</td>
                        <td  rowspan="3">${item.SoLuong}</td>
                         <td  rowspan="3">${item.SoLuongTT}</td>
                        <td  rowspan="3">${item.Kho}</td>
                        <td  rowspan="3">${item.KhoTT}</td>
                         <td >Vị trí lỗi</td>
                         ${htmlVitri == "" ? '<td></td>' : htmlVitri}
                         <td rowspan="3">${ToTalDiem}</td>
                         <td rowspan="3">${result}</td>
                        <td  rowspan="3">${item.GhiChu}</td>
                       
                        </tr>                       
                        <tr>
                            <td>Loại lỗi</td>
                            ${htmlLoaiLoi == "" ? '<td></td>' : htmlLoaiLoi}
                        </tr>
                        <tr>
                            <td>Điểm lỗi</td>
                              ${htmlDiemLoi == "" ? '<td></td>' : htmlDiemLoi}
                        </tr>
                        `
                if (lengthColspan_Temp > lengthColspan) lengthColspan = lengthColspan_Temp;
                indexCheck--
            });
            arrKqKT = data
            $('#CTLoi').attr("colspan", lengthColspan);
            $('#BodyListChecked').html(html);
            var drDataT = data[0];
            document.getElementById('GhiChu').innerHTML = drDataT.GhiChu;
            if (drDataT.SignManager != null) {
                $("#SignManager").attr('src', `/Images/SignKiemVai/${drDataT.SignManager}`);
            }
            else {
                $("#SignManager").attr('src', `/Images/DefaultSign.png`);
            }
            if (drDataT.SignNVKiem != null) {
                $("#SignNVKiem").attr('src', `/Images/SignKiemVai/${drDataT.SignNVKiem}`);
            }
            else {
                $("#SignNVKiem").attr('src', `/Images/DefaultSign.png`);
            }
            setTimeout(function () {
                mergeFirstColumn("#BodyListChecked");
            }, 110);
        }
    });
}
function checkRowSpan(data, valueI, object) {
    let index = 1;
    for (var i = valueI + 1; i < data.length; i++) {
        let checkValue = `${data[i].ChiTiet}`
        if (checkValue == object) index++;
        else break;
    }
    return index
}
function HandleResult(totalDiem, type, SoLuong, Kho) {
    var KquaKiem = "";
    var ChieuDaiVai = ConvertToYard(SoLuong, type);
    var SoDiem = (totalDiem / ChieuDaiVai) * 100;
    if (SoDiem > 15) KquaKiem = "K.Đạt";
    else KquaKiem = "Đạt";
    return KquaKiem;
}
function ConvertToYard(value, type) {
    var valConvert = 0;
    switch (type.toUpperCase()) {
        case "MET":
            valConvert = value * 1.0936133;
            break;
        case "MÉT":
            valConvert = value * 1.0936133;
            break;
        case "CM":
            valConvert = value * 0.010936133;
            break;
        case "INCH":
            valConvert = value * 0.0277777778;
            break;
        case "YARD":
            valConvert = value;
            break;
        default:
            valConvert = value;
            break;
    }
    return valConvert;
}
function ConvertToInch(value, type) {
    var valConvert = 0;
    switch (type) {
        case "Mét":
            valConvert = value * 39.3700787;
            break;
            break;
    }
    return valConvert;
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
    var url = '/api/KiemVai/SaveSign';
    var post = {
        IdNguoiKy: idNguoiKy,
        MaDH: $('#MaHang').val(),
        MaHang: $("#MaHang option:selected").data("mahang"),
        Dot: $("#MaHang option:selected").data("dot"),
        MaVai: $('#LoaiVai').val(),
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
            para3: $("#MaHang option:selected").data("dot"),
            para4: $('#LoaiVai').val()
        };
        $.ajax({
            type: "GET",
            url: '/api/KiemVai/Get',
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
        $("#LoaiVai option:selected").text(),
        $("#GhiChu").text(),
        dataImage[0]?.SignManager,
        dataImage[0]?.SignNVKiem,
    ]
    let dataToSend = JSON.stringify({
        ArrBody: arrKqKT,
        ArrThongSo: arrThongSo
    });

    var url = `/api/PhuLieu/EXBCKTVai`;
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
        a.download = 'BaoCaoKTCLVai.xlsx';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
    })

}
