

toastr.options = {
    "closeButton": true,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
let user = localStorage.getItem("username1")
$("#home").on("click", function () {
    window.location.href = '/Home/Dashboard'
})
let isCheckDN = localStorage.getItem('isCheckDN');
//if (isCheckDN == 'false') {
//    window.location.href = '/Login/Index'
//}
let remember = localStorage.getItem('remember');
if (remember == 'false') {
    localStorage.setItem('isCheckDN', 'false');
}
$(document).on('input', '.CMND', function () {
    let value = $(this).val();
    value = value.replace(/\D/g, '');
    value = value.slice(0, 12);
    $(this).val(value);
});
$("#tbodyLists").on('input', '.CMNDListInput', function () {
    let value = $(this).val();
    value = value.replace(/\D/g, '');
    value = value.slice(0, 12);
    $(this).val(value);
});
var today = new Date();
var dd = String(today.getDate()).padStart(2, '0');
var mm = String(today.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0 (Tháng 1 là 0)
var yyyy = today.getFullYear();

// Định dạng thành chuỗi "yyyy-MM-dd"
today = yyyy + '-' + mm + '-' + dd;

$(".lookup-results").hide();
$(".lookup-results1").hide();// Ẩn danh sách kết quả lúc ban đầu
$(".ultitle").hide()
function convertDate(datetime) {
    var today = new Date(datetime);
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    return yyyy + '-' + mm + '-' + dd ;
}

$(".lookup-input").on("input", function () {
    var filter = $(this).val().toUpperCase();

    $(".lookup-results li").each(function () {
        var textValue = $(this).text().toUpperCase();
        if (textValue.indexOf(filter) > -1) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
});
$(".lookup-input1").on("input", function () {
    var filter = $(this).val().toUpperCase();

    $(".lookup-results1 li").each(function () {
        var textValue = $(this).text().toUpperCase();
        if (textValue.indexOf(filter) > -1) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
});

$(".lookup-input").on("focus", function () {
    $(".lookup-results").show();
    $(".ultitle").show()
});
$(".lookup-input2").on("focus", function () {
    $(".lookup-results1").show();
    $(".lookup-input1").show();
});
$(".lookup-input2").on("blur", function () {
    setTimeout(function () {
        if (!$(document.activeElement).hasClass('lookup-input1')) {
            $(".lookup-results1").hide();
            $(".lookup-input1").hide();
        }
    }, 200);

});
$(".lookup-input1").on("blur", function () {
    setTimeout(function () {
        if (!$(document.activeElement).hasClass('lookup-input2')) {
            $(".lookup-results1").hide();
            $(".lookup-input1").hide();
        }
    }, 200);

});
$(".lookup-input").on("blur", function () {
    setTimeout(function () {
        $(".lookup-results").hide();
        $(".ultitle").hide();

    }, 200);

});

var canvas = document.getElementById('signatureCanvas');
var signaturePad = new SignaturePad(canvas, {
    backgroundColor: 'rgb(255, 255, 255)' // Màu nền của canvas
});
$('#size-slider').on('input', function () {
    var selectedSize = parseInt($(this).val());
    signaturePad.minWidth = selectedSize; // Cài đặt kích thước nét tối thiểu
    signaturePad.maxWidth = selectedSize; // Cài đặt kích thước nét tối đa
    signaturePad.dotSize = selectedSize; // Cài đặt kích thước nét
});
let maPhieu = "";
$('#colorPicker').on('change', function () {
    var selectedColor = $(this).val();
    signaturePad.penColor = selectedColor;
});
$('#clearButton').on('click', function () {
    signaturePad.clear();
});
$(".hide_popup").on("click", function () {
    $(".popup").hide();
    signaturePad.clear();
})

let xeVC = 0;
let xeRoMoc = 0;
let xeTai = 0

let valCont = ""; let valPKL = ""; let valMaSeal = ""; let valDispaly = ""; var filterData = ""; let valTenCont = ""

$(".vcCont").on("change", function () {
    $(`.vcXetai`).prop("checked", false);
    xeVC = this.checked ? 1 : 0;;
    xeTai = 0

    $(".inputseal").show()
    $(".selectseal").hide()
    $(".inputcontainer").show()
    $(".inputromoc").show()
})
$(".vcroMoc").on("change", function () {
    $(`.vcXetai`).prop("checked", false);
    xeTai = 0
    xeRoMoc = this.checked ? 1 : 0;;
})
$(".vcXetai").on("change", function () {
    $(`.vcCont`).prop("checked", false);
    $(`.vcroMoc`).prop("checked", false);
    xeTai = this.checked ? 1 : 0;;

    $(".inputseal").hide()
    $(".selectseal").show()
    $(".inputcontainer").hide()
    $(".inputromoc").hide()
    xeRoMoc = 0;
    xeVC = 0
})

function duLieuPhieu() {
    let object = {
        MaPKL_XH: valPKL,
        VCCont: xeVC,
        VCXeTai: xeRoMoc,
        Cont: valCont,
        Romoc: $(".container").val(),
        TenTaiXe: $(".nameTX").val(),
        CMND: $(".CMND").val(),
        MaSeal: $(".maSeal").val(),
        PackDate: $(".xeDi").val(),
        FinishDate: $(".xeDen").val(),
        NVienXuat: user,
    }
    $.ajax({
        type: "POST",
        url: "/api/TheoDoiDonHang/UpdateChuyenKho",
        data: JSON.stringify(object),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            renderMaPhieu()
           
        }
    });
}

function renderMaSeal() {
    $.ajax({
        async: false,
        url: "/api/XuatHang/Get?Action=GetSeal&para1=A&para2=A",
        type: 'Get',
        success: function (data) {
            let html = ``
            let arrSeal = [];
            data.map(item => {
                if (item.Status != 0) {
                    arrSeal.push(item.MaSeal)
                }
                html += `
                        <option style="color:#fff" data-sudung='${item.Status}' value="${item.MaSeal}">${item.Display}</option>
                    `
            })
            $(".maSealSelect").html(html)
            $(".maSealSelect").select2()
            $(".maSealSelect").on('select2:open', function () {
                setTimeout(function () {
                    arrSeal.map(item => {
                        $(`li[id*='${item}']`).addClass("hightline_seal");
                    })
                }, 0);
            });
        }
    })
}


$(".maSealSelect").on("focus", function () {
    alert(1)
    previousValue = $(this).val();
});
let module = 1;
$("#Update").on("click", function () {
    if (module == 2) {
        if ($(".container").val().trim() == "") {
            toastr.error("Không có phiếu")
        } else {
            getTenBienBan()
        }
    }
    else {
        if ($(".container").val().trim() != "" &&
            $(".nameTX").val().trim() != "" &&
            $(".CMND").val().trim() != "" &&
            $(".xeDI").val() != "" &&
            $(".xeDen").val() != "") {
            getTenBienBan()
        } else {
            ThongBao()
        }
    }
})


function ThongBao() {
    var errorMessage = '';
    if ($(".container").val().trim() == "") {
        errorMessage += 'Số xe bị rỗng ';
        $(".container").focus(); // 
    } else if ($(".nameTX").val().trim() == "") {
        errorMessage += 'Tên TX bị rỗng ';
        $(".nameTX").focus();
    }
    else if ($(".CMND").val().trim() == "") {
        errorMessage += 'CMND bị rỗng ';
        $(".CMND").focus();
    }
    else if ($(".xeDen").val().trim() == "") {
        errorMessage += 'Giờ xe đến bị rỗng ';
        $(".xeDen").focus();
    }
    else if ($(".xeDI").val().trim() == "") {
        errorMessage += 'Kiểm tra xe bị rỗng'
        $(".checkXe").focus();
    }
    else if ($(".banSo").val().trim() == "") {
        errorMessage += 'Kiểm tra bản số xe bị rỗng ';
        $(".banSo").focus();
    }
    toastr.error('Vui lòng nhập đầy đủ thông tin cho các ô sau: ' + errorMessage);
}

let arrNoiDung = [];

function renderValRong() {
    $(".container").val("")
    $(".banSo").val("")
    $(".romoc").val("")
    $(".nameTX").val("")
    $(".CMND").val("")
    $(".xeDen").val("")
    $(".xeDi").val("")
    $(".maSeal").val("")
    $(".tukho").val("")
    $(".denkho").val("")
    $("#datetime_xeden").text("dd/mm/yyyy --:--")
    $("#datetime_xedi").text("dd/mm/yyyy --:--")
    $("#bodyMahang").empty();
}
function isCheckNoiDung() {
    $(`.vcCont`).prop("checked", true);
    $(`.vcroMoc`).prop("checked", true);
    $(`.vcXetai`).prop("checked", false);
    xeVC = 1;
    xeRoMoc = 1;
    xeTai = 0
    $(".inputseal").show()
    $(".selectseal").hide()
    $(".inputcontainer").show()
    $(".inputromoc").show()
}
function isCheckNoiDungRong() {
    $(`.vcCont`).prop("checked", false);
    $(`.vcroMoc`).prop("checked", false);
    $(`.vcXetai`).prop("checked", false);
    xeVC = 0;
    xeRoMoc = 0;
    xeTai = 0
}
function formatDateTime(datetime) {
    const date = new Date(datetime);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}/${month}/${year} ${hours}:${minutes}`;
}
function formatDateTimeAddNgay(datetime) {
    const date = new Date(datetime);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}/${month}/${year} `;
}
function handleDateChange(selector, outputSelector) {
    $(selector).on("change", function () {
        const datetime = $(this).val();
        $(outputSelector).text(formatDateTime(datetime));
    });
}
function handleDateChangeAddNgay(selector, outputSelector) {
    $(selector).on("change", function () {
        const datetime = $(this).val();
        $(outputSelector).text(formatDateTimeAddNgay(datetime));
    });
}
handleDateChange(".xeDI", "#datetime_xedi");
handleDateChange(".xeDen", "#datetime_xeden");
handleDateChangeAddNgay(".ngaylapphieu", "#datetime_ngaylapphieu");

let tenBienBan = ""
function saveBienBanChuyenKho() {
    var selectedValues = $('.maSealSelect').val();
    var selectedString = selectedValues.join(',');
    let object = {
        MaPKL_XH: valPKL,
        TenTaiXe: $(".nameTX").val(),
        TuKho: $(".tukho").val(),
        DenKho: $(".denkho").val(),
        CMND: $(".CMND").val(),
        SoXe: $(".banSo").val(),
        SoRoMoc: $(".romoc").val(),
        NgayXeDI: $(".xeDI").val(),
        NgayXeDen: $(".xeDen").val(),
        MaSeal: xeTai == 1 ? selectedString : $(".maSeal").val(),
        SoSeal: $(".ngaylapphieu").val(),
        Cont: valCont,
        TenCont: $(".container").val(),
        TenBienBan: tenBienBan,
        VCCont: xeVC,
        VCRoMoc: xeRoMoc,
        VCXetai: xeTai,
    }
    $.ajax({
        type: "POST",
        url: "/api/TheoDoiDonHang/PostBienBanChuyenKho",
        data: JSON.stringify(object),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            saveListSeal(1, 1)
            saveListTaiXe()
            renderMaSeal()
            duLieuPhieu()
            saveGhiChu()
            saveImageOther()
            saveNoiBo(selectedString, 1)
            saveKyTen()
            toastr.success("Lưu thành công");
            $("html, body").animate({ scrollTop: 0 }, 500);
        }
    });
}
function getTenBienBan() {
    $.ajax({
        url: "/api/TheoDoiDonHang/Get?Action=GetTenBienBan&Para1=A&Para2=A&Para3=a",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                tenBienBan = "";
            } else {
                tenBienBan = data[0].TenBienBan

            }
            getTenBienBanADD()
        }
    })
}
function addWithPadding(a, b) {
    let result = a + b;
    return result.toString().padStart(2, '0');
}
function getTenBienBanADD() {
    let month = today.split("-")[1]
    let day = today.split("-")[2]
    let bienbanMonth = tenBienBan == "" ? "" : tenBienBan.split("|")[2]
    if (tenBienBan == ""  || bienbanMonth != month) {
        tenBienBan = `XNB|${addWithPadding(1, 0)}|${month}`
    }
    else {
        let bienban = tenBienBan.split("|")[1]
        tenBienBan = `XNB|${addWithPadding(Number(bienban), 1)}|${month}`
    }
    saveBienBanChuyenKho()
}
$("#creatPhieu").on("click", function () {
    module = 1;
    $(".search").fadeOut(500);;
    $(".maPhieu").show()
    $("#Update").html(`<i class="fa fa-sharp fa-solid fa-floppy-disk"></i> Lưu `);
    $("#xuatEX").hide()
    $("#delete").hide()
    $("html, body").animate({ scrollTop: 0 }, 500);
    renderMaPhieu()
    $(".ma_PKL").show();
    $(".item_addImage").empty()
    $(".camera_imageOther").show()
    $("#bodyKyTen").empty()
    $(".camera_imageOther_View").hide()
    $(".camera_imageOther_back").show()
    count = 0;
    $("#delete").hide()
   
})
$("#viewPhieu").on("click", function () {
    $(".item_addImage").empty()
    module = 2;
    $(".search").fadeIn(500);;
    renderMaPhieu()
    $(".maPhieu").hide()
    $("#xuatEX").show()
    $("#Update").html(`<i class="fa-solid fa-file-pen"></i> Update phiếu`)
    $("html, body").animate({ scrollTop: 0 }, 500);
    $(".ma_PKL").hide();
    $("#bodyKyTen").empty()
    AddKyTen()
    $(".camera_imageOther_View").show()
    $(".camera_imageOther_back").hide()
    $(".item_addImage").hide()
    ischeckLength = false;
    divLength = 1;
    count = 0
    $("#delete").show()

})
function renderMaPhieu() {
    $.ajax({
        url: "/api/TheoDoiDonHang/Get?Action=GetMaPKL_CK&para1=&para2=A&para3=A",
        type: 'Get',
        success: function (data) {
            renderMaSeal()
            isCheckNoiDung()
            $("#bodyKyTen").empty()
            $(".item_addImage").empty()
            renderValRong()
            $("#bodyMahang").empty()
            if (module == 2) {
                renderPhieuDaKiem()
            } else {
                renderMaPKL(data)
            }
            GetName()
            GetTableChuKy()
        }
    })
}

renderMaPhieu()
function renderPhieuDaKiem() {
    $.ajax({
        url: "/api/TheoDoiDonHang/Get?Action=PhieuCKDaKiem&para1=&para2=A&para3=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                renderValRong();
                $(".lookup-results").empty()
                $(".lookup-input").val()

                return;
            }
            isCheckValue()
            arrPhieuDaKiem = data
            let html = ``
            valCont = data[0].Cont
            valPKL = data[0].MaPKL_CK
            valMaSeal = data[0].Seal
            valDispaly = data[0].TenBienBan
            valTenCont = data[0].TenCont
            $(".lookup-input").val(valDispaly)
            data.map(item => {
                html += `
               <li class="result-row" data-tencont=${item.TenCont} data-seal="${item.Seal}" data-cont="${item.Cont}" data-display= "${item.TenBienBan}" data-value="${item.MaPKL_CK}" data-name="${item.TenBienBan}">
                 <div class="result-column"> ${item.MaPKL_CK} </div>
                 <div class="result-column">${item.SoXe}</div>
                 <div class="result-column">${item.TenCont}</div>
                 <div class="result-column">${item.TenTaiXe}</div>
             </li>
                `
            })
            $(".lookup-results").html(html)
            renderValuePhieuDaKiem()

        }
    })
}
let arrPhieuDaKiem = []
function renderValuePhieuDaKiem() {
    let filterData = arrPhieuDaKiem.filter(item => item.MaPKL_CK === valPKL && item.Cont === valCont)
    let d = filterData[0];
    if (d.VCCont == 1) {
        $(`.vcCont`).prop("checked", true);
        xeVC = 1
        $(".inputcontainer").show()
        $(".inputromoc").show()
        $(".inputseal").show()
        $(".selectseal").hide()
    }
    else if (d.VCXetai == 1) {
        $(`.vcXetai`).prop("checked", true);
        xeTai = 1
        $(".inputcontainer").hide()
        $(".inputromoc").hide()
        $(".inputseal").hide()
        $(".selectseal").show()
    }
    if (d.VCRoMoc == 1) {
        $(`.vcroMoc`).prop("checked", true);
        xeRoMoc = 1
    }
    $(".banSo").val(d.SoXe)
    $(".romoc").val(d.SoRoMoc)
    $(".nameTX").val(d.TenTaiXe)
    $(".CMND").val(d.CMND)
    $(".maSeal").val(d.MaSeal)
    $(".ngaylapphieu").val(convertDate(d.Ngaylap))
    var defaultValues = d.MaSeal.split(',');
    $(".maSealSelect ").val(defaultValues).trigger('change');
    $(".xeDI").val(d.NgayXeDI)
    $("#datetime_xedi").text(formatDateTime(d.NgayXeDI))
    $(".xeDen").val(d.NgayXeDen)
    $("#datetime_xeden").text(formatDateTime(d.NgayXeDen))
    $("#datetime_ngaylapphieu").text(formatDateTimeAddNgay(d.Ngaylap))
    renderMaHang()
    renderImageOther()
    renderImageKiTen()
}
$('.maSealSelect').select2({
    placeholder: 'Select options',
    allowClear: true

});

function saveNoiBo(selectedString, ValueXoaOrDelete) {
    $.ajax({
        url: "/api/ThongKeDongThung/GetNK?Action=SaveSealNoiBo&para1=" + selectedString + "&para2=" + ValueXoaOrDelete + "&para3=A&para4=A&para5=A",
        type: 'Get',
        success: function (data) {

        }
    })
}
function isCheckValue() {
    $(`.vcCont`).prop("checked", false);
    $(`.vcroMoc`).prop("checked", false);
    $(`.vcXetai`).prop("checked", false);
    xeRoMoc = 0;
    xeTai = 0;
    xeVC = 0
}
function AutoWidth() {
    $('.lookup-input2').each(function () {
        $(this).width($(this).val().length * 7);
        $(".lookup-input1").width($(this).val().length * 7);
    });
    $('.lookup-input').each(function () {
        $(this).width($(this).val().length * 7);

    });
}
function renderMaPKL(data) {
    if (data.length == 0) {
        toastr.error("Chưa có mã PKL xuất hàng")
        hideDisable()
        return;
    }
    filterData = data.filter(item => item.Status == 0 && item.IsHT == 1)

    valCont = data[0].Cont
    valPKL = data[0].MaPKL_XH
    valMaSeal = data[0].Seal
    valDispaly = data[0].Display
    let html = ``;
    data.map(item => {
        html += `
            <li class="result-row" data-isexport="${item.Status}" data-seal="${item.Seal}" data-cont="${item.Cont}" data-display= "${item.Display}" data-value="${item.MaPKL_XH}"
                data-isht="${item.IsHT}" data-isdongthung="${item.IsDongThung}" data-isnhapkho="${item.IsNhapKho}" data-isxacnhan="${item.IsXacNhan}"
            ">
                        <div class="result-column">${item.Display} </div>
             </li>
        `
    })
    $(".lookup-results1").html(html)
    $(".ngaylapphieu").val(today)
    $("#datetime_ngaylapphieu").text(formatDateTimeAddNgay(today))
    renderTableMaHang();

    $(".lookup-results1").each(function () {
        $(this).find('li').each(function () {
            var isExportValue = $(this).data('isexport');
            let isHoanThanh = $(this).data('isht')
            if (isExportValue != "0") {
                $(this).addClass('highlight');
            } else {
                if (isHoanThanh == 0) {
                    $(this).addClass('highlight2');
                }
            }
        });
    })
    AddKyTen()
}

function hideDisable() {
    $(".tukho,.denkho").prop("readonly", true);
}
function showDisable() {
    $(".container,  .nameTX, .CMND,  .xeDen, .xeDi").prop("readonly", false);
}
$("#clearButtonDetail").on("click", function () {
    $(".popup2").hide()
})
function getMaPKLNo(isAll, contDetail, mapklDetail) {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetPopChiTiet&para1=" + isAll + "&para2=" + mapklDetail + "&para3=" + contDetail,
        type: 'Get',
        success: function (data) {
            $(".popup2").show()
            let alertPopup = ""
            if (isAll == 1) alertPopup = 'đóng thùng';
            else if (isAll == 2) alertPopup = 'nhập kho'
            else if (isAll == 3) alertPopup = 'xác nhận'
            let html = ``
            data.map(item => {
                html += `
                 <div class="text_warring">
                    ${item.GopDH} - MH: ${item.MaHang} -  ${item.MaPKL} - PO: ${item.Po}
                </div>
                `
            })
            html += `
                  <div class="text_warring">
                   chưa hoàn thành ${alertPopup}
                </div>
            `
            $(".item_text").html(html)
        }
    })
}
$(".lookup-results1").on("click", "li", function () {
    if ($(this).hasClass("highlight")) {
        toastr.error("Phiếu này đã kiểm hàng")
        return;
    }
    if ($(this).hasClass("highlight2")) {
        let isDongThung = $(this).data("isdongthung")
        let contDetail = $(this).data("macont")
        let mapklDetail = $(this).data("value")
        let IsNhapKho = $(this).data("isnhapkho")
        let IsXacNhan = $(this).data("isxacnhan")
        if (isDongThung == 0) {
            getMaPKLNo(1, contDetail, mapklDetail)
        }
        else if (IsNhapKho == 0) {
            getMaPKLNo(2, contDetail, mapklDetail)
        }
        else if (IsXacNhan == 0) {
            getMaPKLNo(3, contDetail, mapklDetail)
        }

        return;
    }
    valDispaly = $(this).data("display")
    valMaSeal = $(this).data("seal")
    valPKL = $(this).data("value")
    valCont = $(this).data("cont")
    $(".lookup-results1").hide();
    $(".lookup-input1").hide();
    $(".lookup-input1").val("")
    renderTableMaHang()
});
$(".lookup-results").on("click", "li", function () {
    isCheckNoiDungRong()
    valDispaly = $(this).data("display")
    $("#bodyKyTen").empty()
    AddKyTen()
    valMaSeal = $(this).data("seal")
    valPKL = $(this).data("value")
    valCont = $(this).data("cont")
    $(".container").val(valTenCont)
    $(".lookup-results").hide();
    $(".ultitle").hide()
    $(".lookup-input").val(valDispaly)
    renderValuePhieuDaKiem()
});

function saveGhiChu() {
    let arrGhiChu = []
    $("#bodyMahang tr").slice(0, -1).each(function () {
        let row = $(this).find('td');
        let poid = $(this).data("poid")
        let mahang = $(this).data('mahang')
        let object = {
            MaPKL_XH: valPKL,
            Cont: valCont,
            MaTuKho: mahang,
            MaDenKho: poid,
            TuKho: row.eq(5).text()
        };
        arrGhiChu.push(object);
    });
    if (arrGhiChu.length != 0) {
        $.ajax({
            url: '/api/TheoDoiDonHang/PostDL',
            type: "Post",
            data: JSON.stringify(arrGhiChu),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {

            }
        })
    }
}
function renderTableMaHang() {
    if (filterData.length == 0) {
        toastr.error("Các phiếu đã được kiểm hàng.")
        hideDisable()
        $(".camera_imageOther").hide()
        $(".camera_imageOther_back").hide()
        $(".lookup-input2").val("")
        renderValRong()
        return
    } else {
        showDisable()
    }
    $(".lookup-input2").val(valDispaly)
    renderMaHang()
}
let totalSoKienTable = ""
let totalSoLuongTable = ""
function renderMaHang() {
    $(".camera_imageOther").show()
    AutoWidth()
    $.ajax({
        url: "/api/TheoDoiDonHang/Get?Action=GetBienBanChuyenKho&para1=" + valPKL + "&para2=" + valCont + '&para3=a',
        type: 'Get',
        success: function (data) {
            d = data[0]
            $(".tukho").val(d.TuKho)
            $(".denkho").val(d.DenKho)
            $(".container").val(d.TenCont)
            let totalSoLuong = data.reduce((acc, curr) => acc + parseInt(curr.SoLuong), 0);
            let totalSoKien = data.reduce((acc, curr) => acc + parseInt(curr.Carton), 0);
            totalSoKienTable = totalSoKien
            totalSoLuongTable = totalSoLuong
            let htmlFoter = `
                          <tr style="height:25px">
                             <td colspan="2"></td>
                            <td colspan="">Tổng cộng</td>
                            <td colspan="">${totalSoLuong}</td>
                             <td colspan="">${totalSoKien}</td>
                            <td colspan=""></td>
                          
                        </tr>
            `
            let html = ``
            data.map(item => {
                html += `
                    <tr data-poid="${item.POID}" data-mahang=${item.MaHang} style="height:30px">
                            <td>${item.TenKH}</td>
                            <td>${item.MaHang}</td>
                            <td>${item.PO}</td>
                            <td>${item.SoLuong}</td>
                            <td>${item.Carton}</td>
                             <td style = "outline: none; text-align: center;  padding: 0px 0;" contenteditable = 'true' > ${item.GhiChu1}</td >
                        </tr>
                      `
            })
         
            html += htmlFoter
            $("#bodyMahang").html(html)

        }
    })
}
$("#xuatEX").on("click", function () {
    Export()

})
function Export() {
    var url = "/api/TheoDoiDonHang/BCEXNoiBo?Action=GetBCEx&para1=" + valPKL + "&para2=" + valCont + '&para3=' + $(".ngaylapphieu").val();
    var link = document.createElement('a');
    var filename = `PhieuXuatKho.xlsx`;
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
// Camera
function InitCamera() {
    $(function () {
        Webcam.set('constraints', {
            facingMode: "environment"
        });
        Webcam.set({
            width: 480,
            height: 680,
            image_format: 'jpeg',
            jpeg_quality: 90
        });
        $("#btnCloseModal").click(function () {
            Webcam.reset();
        });
    });
}

InitCamera()
function StartCamera() {
    Webcam.attach('#webcam');
    $('#webcam').css({ "width": "100%", "height": "100%" });
    $('video').css({ "width": "100%", "height": "100%" });

}
function CloseCamera() {
    Webcam.reset();
}

function CaptureDefect(id, _SPLoi) {
    lstImageSPLoi = id
    CodeSPLoi = _SPLoi;
    Webcam.attach('#webcamA');
    $('#webcamA').css({ "width": "100%", "height": "100%" });
    $('video').css({ "width": "100%", "height": "100%" });
    Webcam.reset();
}
let imageData = "";

function Capture() {
    Webcam.snap(function (data_uri) {
        Webcam.reset();
        UrlImage = data_uri;
        $('#PreViewImage')[0].src = data_uri;
        imageData = data_uri;
    });

}
function CaptureAgain() {
    Webcam.snap(function (data_uri) {
        Webcam.reset();
        $("#" + lstImageSPLoi).prepend(`<img data-toggle="modal" data-target="#ModalViewImage" class="ErrorImage" style ="height:20px;width:20px;margin-left:2px" src="${data_uri}" />`)

    });

}
let arrImage = []
function SavePreImage() {
    $(`#${linkImage}`).empty();
    let imageelement = $(`<img data-name="${imageData}">`).attr("src", imageData);
    $(`#${linkImage}`).append(imageelement);

}
$(document).on("click", function (event) {
    if (!$(event.target).closest(' .inputName,.input_image').length) {
        $(".list_kyten").removeClass("active");
    }
});

$(document).on("click", ".maSeal, .inputName,.input_image", function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $(".list_kyten").addClass("active")
        $("html, body").animate({ scrollTop: $(document).height() }, 800);
    }
});
$(document).on("click", ".input_image", function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $(".list_kyten").addClass("active")
        $("html, body").animate({ scrollTop: $(document).height() }, 800);
    }
});


let hotenR = "";
let bophanValue = ""
let countKiten = 0;
let linkImageNoiBo = "";
function renderImageKiTen() {
    $.ajax({
        url: "/api/TheoDoiDonHang/Get?Action=GetKyTenNoiBo&para1=" + valPKL + "&para2=" + valCont + "&para3=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
              
                return;
            }
            $("#bodyKyTen").empty()
            $.each(data, function (index, item) {
                countKiten = item.Position
                let html = ``
                if (item.ChuKy != null && item.ChuKy != "") {
                    html = `
                    <img  data-name="" src="/Images/KyTenNoiBo/${item.ChuKy}">
                `;
                }
                $("#bodyKyTen").append(`
                 <tr data-position="${countKiten}" data-mahoten="${item.MaHoTen}" style="position: relative;">
                     <td style="width:30%"><input style="text-transform: capitalize;" data-column="HoTen" id="hoten" class="hoten${countKiten} inputName" type="text" name="name" value="${item.HoTen}" />
                    <i class="fa-sharp fa-solid fa-trash icondelete_kiten"></i></td>
                     <td style="width:25%"><input style="text-transform: capitalize;" data-column="BoPhan" id="bophan" class="bophan${countKiten} inputName" type="text" name="name" value="${item.BoPhan}" /></td>
                     <td class="image_kyten" style="width:45%">
                         <div id="imageKyTen" class="list-img${countKiten}" data-name="" style=" width: 100%; height: 109px; display: flex; justify-content: center; margin-bottom: 5px; ">
                              ${html}
                          </div>
                     </td>
                 </tr>
                `)
            })
        }
    })
}
let indexKiTen = 0;
$("#bodyKyTen").on('click', '.image_kyten', function () {
    let trElement = $(this).closest('tr');
    indexKiTen = trElement.data("position")
    hotenValue = trElement.find(`.hoten${indexKiTen}`).val();
    bophanValue = trElement.find(`.bophan${indexKiTen}`).val();
    linkImageNoiBo = trElement.find(`.list-img${indexKiTen}`).attr("class")
    if (hotenValue.trim() != "" && bophanValue.trim() != "") {
        $(".popup").show()
    } else {
        toastr.error("Vui lòng nhập họ tên và bộ phận")
    }
});
$("#fileInput").change(function () {
    showFileImage(this);
});
function showFileImage(input) {
    var file = $('#fileInput')[0].files[0];
    var reader = new FileReader();
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            if (moduleAnh == 3) {
                $(`#${linkImage}`).empty();
                let imageelement = $(`<img data-name="${e.target.result}">`).attr("src", e.target.result);
                $(`#${linkImage}`).append(imageelement);
            } else {
                //imageData = e.target.result
                //saveImage()
                addImageInDiv(e.target.result)
                getImageFolder(e.target.result)
            }
        }
        reader.readAsDataURL(file);
    }

    $("#fileInput").val("")
}
function showConfirmationAdd(id_img) {
    Swal.fire({
        title: 'Thực hiện hành vi?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Chụp hình',
        cancelButtonText: 'Chọn ảnh',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button id="zoomImage2" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;">Zoom Image</button>'
    }).then((result) => {
        if (result.isConfirmed) {
            StartCamera();
            $('#ModalImage').modal('show');
            moduleAnh = 1
        }

    });
    $(".swal2-cancel").on("click", function () {
        moduleAnh = 3
        $("#fileInput").trigger('click');

    })
    $("#zoomImage2").on("click", function () {
        const imgSrc = $(`#${id_img} img`).attr('src');
        Swal.close();
        if (imgSrc == null || imgSrc == undefined) {
            toastr.error("Vui lòng chụp hình hoặc chọn hình trước khi phóng to !")
            return;
        }

        $('#zoomedImage').attr('src', imgSrc);
        $('#myModal2').modal('show');
    })
}


let arrKiTen = [];
$("#saveButton").on("click", function () {
    $(`.${linkImageNoiBo}`).empty()
    let signatureData = signaturePad.toDataURL();
    let elementImageNoiBo = $(`<img data-name='${signatureData}' class='linkhinh'>`).attr("src", signatureData)
    $(`.${linkImageNoiBo}`).append(elementImageNoiBo)
    var last_row = $('#bodyKyTen tr:last');
    if (last_row.find("#imageKyTen").children().length != 0) {
        AddKyTen()
        $("html, body").animate({ scrollTop: $(document).height() - $(window).height() }, 500);
    }
    signaturePad.clear();
    $(".popup").hide()

})
function foreachTable() {
    $("#bodyKyTen tr").each(function () {
        var hoten = $(this).find("#hoten").val(); // Lấy giá trị của input có class là hoten trong hàng hiện tại
        var bophan = $(this).find("#bophan").val(); // Lấy giá trị của input có class là bophan trong hàng hiện tại
        var linkHinh = $(this).find(".image_kyten .linkhinh").attr("data-name");
        var mahoten = $(this).data("mahoten")
        if (linkHinh == undefined) {
            if (hoten == "" || mahoten != "") return true;
        }
        var indexKiTen = $(this).data("position")
        let object = {
            MaPKL: valPKL,
            MaHoTen: "",
            HoTen: capitalizeFirstLetter(hoten),
            BoPhan: capitalizeFirstLetter(bophan),
            ChuKy: linkHinh,
            Position: indexKiTen,
            Cont: valCont
        }
        arrKiTen.push(object)
    });
}
function saveKyTen() {
    foreachTable()
    if (arrKiTen.length == 0) return true;
    $.ajax({
        type: "POST",
        url: "/api/TheoDoiDonHang/PostKyTenNoiBo",
        data: JSON.stringify(arrKiTen),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            arrKiTen = [];
            if (module == 2) {
                renderImageOther()
                renderImageKiTen()
            }
            GetName()
            GetTableChuKy()
        }
    });
}

//add imageOther
let count = 0;
let divLength = 1;
let countRow = 0;
let ischeckLength = false
function renderImageOther() {
    $.ajax({
        url: '/api/TheoDoiDonHang/Get?Action=GetImageNoiBo&para1=' + valPKL + '&para2=' + valCont + '&para3=a',
        type: "Get",
        success: function (data) {
            $(".item_addImage").empty()
            countRow = 1
            if (data.length == 0) {
                count = 1;
                return;
            }
            $.each(data, function (index, item) {
                count = item.Position
                $(".item_addImage").append(`
                 <div data-position=" ${count}" class="image_other ${count}">
                <div style="display: flex;position:relative">
                    <label class="label_image">${countRow}. </label>
                    <input data-positon="${count}" data-change="2" id="input_image${count}" class="input_image" type="text" name="name" value="${item.NoiDung}" />
                     <i data-delete="2" data-name="${count}" class="fa-solid fa-xmark fa-lg iconDelete" style="color: #a00d14; position: absolute; margin-top: 13px; right: 3px;font-size: 30px;cursor: pointer;"></i>
                  </div>
                 <div data-name="${count}" id="lst_AddImage${count}" class="lst_AddImage">
                    <img  src="/Images/ImageNoiBo/${item.Image}">
                </div>
             </div>

                `)
                countRow++;
            })
            countRow--;
            renderImageKiTen()
        }
    })
}
$(".camera_imageOther").on("click", function () {

    if ($(".item_addImage").children().length == 0) {
        count = 1
        countRow = 1;
    } else {
        count++;
        countRow++;
    }

    $(".list_kyten").addClass("active")
    $(".item_addImage").show()
    if (ischeckLength)
        divLength = $(`#lst_AddImage${count - 1}`).children().length;
    if (divLength == 0) {
        toastr.error("Vui lòng chụp hình ")
        count--;
        countRow--;
        return;
    }
    let html = `
          <div data-position=" ${count}" class="image_other ${count}">  
                <div style="display: flex;position:relative">
                    <label class="label_image">${countRow}. </label>
                    <input data-positon="${count}" id="input_image${count}" class="input_image" type="text" name="name" value="" />
                    <i  data-name="${count}" class="fa-solid fa-xmark fa-lg iconDelete" style="color: #a00d14; position: absolute; margin-top: 13px; right: 3px;font-size: 30px;cursor: pointer;"></i>
                </div>
                <div data-name="${count}" id="lst_AddImage${count}" class="lst_AddImage">
                 
                </div>
            </div>`
    $(".item_addImage").append(html)
    $(".image_other").find(`#input_image${count}`).focus()
    $(".list_kyten").addClass("active")
    $("html, body").animate({ scrollTop: $(document).height() }, 800);
    ischeckLength = true
})
$(".camera_imageOther_back").on("click", function () {
    divLength = $(`#lst_AddImage${count}`).children().length;
    if (divLength == 0) {
        $(".item_addImage").children().last().remove();
        count--;
        countRow--;
    }
    ischeckLength = false;
    divLength = 1;
    $(".item_addImage").hide()
    if (module == 2) {
        $(".camera_imageOther_back").hide()
        $(".camera_imageOther_View").show()
    }

})
let linkImage = "";
let countImgae = "";
let inputImage = "";
$(".item_addImage").on("click", ".lst_AddImage", function () {
    moduleImage = 2;
    linkImage = $(this).attr("id")
    countImgae = $(this).data("name")
    inputImage = $(".image_other").find(`#input_image${countImgae}`).val()
    if (inputImage == "") {
        toastr.error("Vui lòng nhập tiêu đề")
        return;
    }
    showConfirmationAdd(linkImage)

})
$(".camera_imageOther_View").on("click", function () {
    if (module == 2) {
        $(".camera_imageOther_back").show()
        $(".camera_imageOther_View").hide()
    }


    $(".item_addImage").show()
})
function saveImageOther() {
    foreachImageNoiBo();
    if (arrImage.length == 0) return true;
    $.ajax({
        url: "/api/TheoDoiDonHang/PostImageNoiBo",
        type: "Post",
        data: JSON.stringify(arrImage),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            arrImage = [];
            ischeckLength = false;
            divLength = 1;
        }
    })
}
$("#delete").on("click", function () {
    let xoaphieu = confirm("Bạn muốn xóa phiếu này")
    var selectedValues = $('.maSealSelect').val();
    var selectedString = selectedValues.join(',');
    if (xoaphieu) {
        let post = {
            MaPKL: valPKL,
            Cont: valCont,
        }
        $.ajax({
            url: "/api/TheoDoiDonHang/DeleteNoiBo",
            type: "Post",
            data: JSON.stringify(post),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $(".lookup-input").val("")
                renderMaPhieu()
                saveNoiBo(selectedString, 0)
                deleteDSXe()
                 saveListSeal(1, 0)
            }
        })
    }
})
function foreachImageNoiBo() {
    $(".image_other").each(function () {
        let $this = $(this)
        let imageNoiBo = $this.find("img").data("name")
        if (imageNoiBo == "" || imageNoiBo == null) return true;
        let inputValue = $this.find(".input_image").val();
        let position = $this.data("position")
        let object = {
            MaPKL: valPKL,
            NoiDung: inputValue,
            Image: imageNoiBo,
            Position: position,
            Cont: valCont
        }
        arrImage.push(object)
    })
}
let arrInput = [];
$(".item_addImage").on("change", ".input_image", function () {
    if ($(this).data("change") == 2) {
        let position = $(this).data("positon")
        let value = $(this).val()
        let object = {
            MaPKL: valPKL,
            NoiDung: value,
            Position: position,
            Cont: valCont
        }
        arrInput.push(object)
        saveInput(arrInput)
    }
});
function saveInput(arrInput) {
    $.ajax({
        url: "/api/TheoDoiDonHang/UpdateInputImage",
        type: "Post",
        data: JSON.stringify(arrInput),
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (data) {

        }
    })
}

$(".item_addImage").on("click", ".iconDelete ", function () {
    ischeckLength = false;
    divLength = 1;
    let count_image = $(".item_addImage").children().length;
    let countDelete = $(this).data("name");
    let $items = $(".item_addImage").find(`.image_other.${countDelete}`);
    var index = $items.index() + 1;
    $items.remove()
    if ($(this).data("delete") == 2) {
        deleteImageOther(countDelete)
    }
    if (count_image > 1) {
        if (count_image == index) {
            count--;
            countRow--;
        }
    }
    count_image = $(".item_addImage").children().length;
    if (count_image == 0) {
        count = 10;
        countRow = 10;
    }
})
function deleteImageOther(countDelete) {
    let post = {
        Action: "DeleteImageOther",
        MaPKL: valPKL,
        Position: countDelete,
        Cont: valCont
    }
    $.ajax({
        url: "/api/TheoDoiDonHang/DeleteImageOtherNoiBo",
        type: "Post",
        data: JSON.stringify(post),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

        }
    })
}
//input kí tên change 
$("#bodyKyTen").on("change", ".inputName", function () {
    let $this = $(this)
    let columName = $this.data("column")
    let $tr = $this.closest("tr")
    let maHoTen = $tr.data("mahoten")
    if (maHoTen == "") return;
    let value = $this.val()
    let objectChangKiTen = {
        Action: "UpdateInputKyTenNoiBo",
        MaHoTen: maHoTen,
        Column: columName,
        Value: value
    }
    saveChangeInputKiTen(objectChangKiTen)
})
function saveChangeInputKiTen(objectChangKiTen) {
    $.ajax({
        url: '/api/TheoDoiDonHang/UpdateInputKyTen',
        type: "Post",
        data: JSON.stringify(objectChangKiTen),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

        }
    })
}
function standardizeName(name) {
    return name.trim().toLowerCase().replace(/\b\w/g, function (char) {
        return char.toUpperCase();
    });
}

function chooseBetterName(currentName, newName) {
    function standardizeName(name) {
        return name.trim().toLowerCase().replace(/\b\w/g, function (char) {
            return char.toUpperCase();
        });
    }

    var standardizedCurrent = standardizeName(currentName);
    var standardizedNew = standardizeName(newName);

    if (standardizedNew > standardizedCurrent) {
        return true;
    } else {
        return false;
    }
}
let arrTenTaiXe = [];
let ChuKy = [];
let bodyChuKy = [];
function GetName() {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetNameCK&Para1=A&Para2=A&Para3=a",
        type: 'Get',
        success: function (data) {
            var bestNames = {};
            $.each(data, function (index, item) {
                var cmnd = item.CMND;
                var tenTaiXe = item.TenTaiXe;
                if (!bestNames[cmnd] || chooseBetterName(bestNames[cmnd].TenTaiXe, tenTaiXe)) {
                    bestNames[cmnd] = item;
                }
            });
            var uniqueNames = Object.values(bestNames);
            arrTenTaiXe = uniqueNames
        }
    })
}
function GetTableChuKy() {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetAddKyTenCK&Para1=A&Para2=A&Para3=a",
        type: 'Get',
        success: function (data) {
            var bestNames = {};
            $.each(data, function (index, item) {
                var tenTaiXe = item.TenTaiXe;
                var cmnd = item.CMND;
                if (!bestNames[tenTaiXe] || chooseBetterName(bestNames[tenTaiXe].TenTaiXe, cmnd)) {
                    bestNames[tenTaiXe] = item;
                }
            });
            var uniqueNames = Object.values(bestNames);

            ChuKy = uniqueNames
        }
    })
}

let moduleBodyTableList = 0;
function renderTenTaiXe() {
    let htmlThead = `
                <tr>
                      <td style="position: sticky; top: 0;" >Tên tài xế</td>
                       <td style="position: sticky; top: 0;white-space: nowrap;" >CMND</td>
                </tr>
               `
    $("#theadList").html(htmlThead)
    $("#theadList1").html(htmlThead)
    rederBodyList(arrTenTaiXe)
    moduleBodyTableList = 1
}
function renderChuKy() {

    let htmlThead = `
                <tr >
                      <td style="position: sticky; top: 0;" >Họ tên </td>
                       <td style="position: sticky; top: 0;white-space: nowrap;" >Bộ phận</td>
                </tr>
               `
    $("#theadList").html(htmlThead)
   
    rederBodyList(ChuKy)
    moduleBodyTableList = 2
}
function rederBodyList(arrTenTaiXe) {
    let html = ``
    let arrName = arrTenTaiXe
    arrName.map(item => {
        html += `
             <tr data-maphieu="${item.MaPhieuBB}" data-tentaixe="${item.TenTaiXe}" data-cmnd="${item.CMND}" data-value="${item.Display}">
                  <td style="white-space: nowrap;">${item.TenTaiXe}</td>
                 <td style="white-space: nowrap;">${item.CMND}</td>
                </tr>
        `
    })
    $("#tbodyList").html(html)
    $("#tbodyList1").html(html)
}
$(".addRow").on("click", function () {
    let index = 1;
    $("#bodyKyTen tr").each(function () {
        var hoten = $(this).find("#hoten").val();
        if (hoten == "") {
            index = 0
            return true;
        }

    });
    if (index == 1) {
        AddKyTen()
        $("html, body").animate({ scrollTop: $(document).height() - $(window).height() }, 500);
    }
})
let moduleTen = "";
$(".icon_tablesignature").on("click", function () {
    $(".popup1 ").show()
    $(".search_table").val("")
    renderChuKy()
})
$(".listTenTX").on("click", function () {
    $(".popup1 ").show()
    $(".search_table").val("")
    renderTenTaiXe()

})
$(".listCMND").on("click", function () {
    $(".popup1 ").show()
    $(".search_table").val("")
    renderTenTaiXe()
})
$(".hide_popup").on("click", function () {
    $(".popup1 ").hide()
})
$("#tbodyList ").on("click", "tr", function () {
    let $this = $(this)
    $("#tbodyList tr").removeClass("activeList")
    $this.addClass("activeList")
    let taixe = $this.data("tentaixe")
    let cmnd = $this.data("cmnd")
    if (moduleBodyTableList == 1) {

        $(".nameTX").val(taixe)
        $(".CMND").val(cmnd)

    }
    else {
        rerderBodyKyTen(taixe, cmnd)
    }
    $(".popup1").hide()
})
$("#bodyKyTen ").on("click", ".icondelete_kiten", function () {
    let $rowtr = $(this).closest("tr")
    $rowtr.remove();
    let mahoten = $rowtr.data("mahoten")
    if (mahoten == "") return;
    deleteKiTenRow(mahoten)
})
function deleteKiTenRow(mahoten) {
    $.ajax({
        url: "/api/XuatHang/Get?Action=DeleteKyTenNBRow&Para1=" + mahoten + "&Para2=A&Para3=a",
        type: 'Get',
        success: function (data) {

        }
    })
}
function AddKyTen() {
    countKiten++;
    let html = `
     <tr data-position="${countKiten}" data-mahoten="" style="position: relative;">
         <td style="width:30%"><input style="text-transform: capitalize;" data-column="HoTen" id="hoten" class="hoten${countKiten} inputName" type="text" name="name" value="" />
          <i class="fa-sharp fa-solid fa-trash icondelete_kiten"></i>
        </td>
         <td style="width:25%"><input style="text-transform: capitalize;" data-column="BoPhan" id="bophan" class="bophan${countKiten} inputName" type="text" name="name" value="" />
            </td>
           
         <td class="image_kyten" style="width:45%">            
             <div id="imageKyTen" class="list-img${countKiten}" data-name="" style=" width: 100%; height: 109px; display: flex; justify-content: center; margin-bottom: 5px; ">
                                       
              </div>
         </td>
     </tr>
    `
    $("#bodyKyTen").append(html)
}
function AddKyTenNew(hotennew, bophannew) {
    countKiten++;
    $("#bodyKyTen tr").each(function () {
        let hotenValue = $(this).find('input#hoten').val().trim();
        if (!hotenValue) {
            $(this).remove();
        }
    })
    let html = `
     <tr data-position="${countKiten}" data-mahoten="" style="position: relative;">
         <td style="width:30%"><input style="text-transform: capitalize;" data-column="HoTen" id="hoten" class="hoten${countKiten} inputName" type="text" name="name" value="${hotennew}" />
        <i class="fa-sharp fa-solid fa-trash icondelete_kiten"></i>
        </td>
         <td style="width:25%"><input style="text-transform: capitalize;" data-column="BoPhan" id="bophan" class="bophan${countKiten} inputName" type="text" name="name" value="${bophannew}" /></td>
         <td class="image_kyten" style="width:45%">
             <div id="imageKyTen" class="list-img${countKiten}" data-name="" style=" width: 100%; height: 109px; display: flex; justify-content: center; margin-bottom: 5px; ">
                                       
              </div>
         </td>
     </tr>
    `
    $("#bodyKyTen").append(html)
    
}
function rerderBodyKyTen(taixe, cmnd) {
    AddKyTenNew(taixe, cmnd)
    $("html, body").animate({ scrollTop: $(document).height() - $(window).height() }, 500);
}
let timeoutId;
function removeDiacritics(str) {
    return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}
$(".search_table").on("keyup", function (e) {
    clearTimeout(timeoutId);
    timeoutId = setTimeout(() => {
        let searchValue = removeDiacritics($(".search_table").val()).toUpperCase()
        $("#tbodyList tr").each(function () {
            let tentaixe = removeDiacritics($(this).data("value")).toString().toUpperCase();
            if (!tentaixe.includes(searchValue)) {
                $(this).hide();
            } else {
                $(this).show();
            }
            if (searchValue == "") {
                $(this).show();
            }
        });
    }, 500);
})
//DS Xe
function saveListSeal(maPhieu, status) {
    let arrMaSeal = [];
    $(".maSealSelect option:selected").each(function () {
        let item = $(this).val();
        let text = $(this).text();
        let objcet = {
            MaPhieu: maPhieu,
            MaCont: valCont,
            MaSeal: item,
            TenSeal: text,
            MaPKL: valPKL,
            SoXe: $(".banSo").val().toUpperCase(),
            Status: status
                
        };
        arrMaSeal.push(objcet);
    });
    if (arrMaSeal.length > 0)
        postListSeal(arrMaSeal)
}
function postListSeal(arrMaSeal) {
    $.ajax({
        url: '/api/BienBanLuuSeal/PostSeal',
        type: "Post",
        data: JSON.stringify(arrMaSeal),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

        }
    })
}
let gopArr = ""
$('.maSealSelect').on('select2:unselect', function (e) {
    var defaultValues = $(".maSealSelect").val()
    defaultValues = defaultValues.join(",")
    let valueClear = e.params.data.id;
    var array1 = defaultValues.split(',');
    var array2 = valueClear.split(',');
    var combinedArray = array1.concat(array2);
    gopArr= combinedArray.join(',');
    $(".maSealSelect ").val(defaultValues).trigger('change');
    $(`.maSealSelect option[value='${valueClear}']`).data("sudung", 0);
    showConfirmationSeal(valueClear, defaultValues)
});
function showConfirmationSeal(valueClear, defaultValues) {
    Swal.fire({
        title: 'Thực hiện hành vi?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Xóa seal lỗi',
        cancelButtonText: 'Đổi seal',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button id="clear2" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;">Hủy</button>'

    }).then((result) => {
        if (result.isConfirmed) {
            updateSavefalse(valueClear, defaultValues, 1)
        }
    });
    $(".swal2-cancel").on("click", function () {
        let arr = defaultValues.split(',').map(item => item.trim());
        $(".maSealSelect ").val(arr).trigger('change');
    })
    $("#clear2").on("click", function () {
        var arr  = gopArr.split(',').map(item => item.trim());
        $(".maSealSelect").val(arr).trigger('change');
        Swal.close();
    })
}
function updateSavefalse(valueClear, defaultValues, valueIdex) {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=UpdateSeal&para1=${valueClear}&para2=a&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            if (valueIdex == 1) {
                renderMaSeal()
                let arr = defaultValues.split(',').map(item => item.trim());
                $(".maSealSelect ").val(arr).trigger('change');
            }
        }
    })
}
let goparrTable = []
$("#tbodyLists").on('select2:unselect', '.maSealSelectList', function (e) {
    var defaultValues = $(this).val()
    defaultValues = defaultValues.join(",")
    let valueClear = e.params.data.id;
    var array1 = defaultValues.split(',');
    var array2 = valueClear.split(',');
    var combinedArray = array1.concat(array2);
    goparrTable = combinedArray.join(',');
    $(this).val($(this).val()).trigger('change');
    $(this).find(`option[value='${valueClear}']`).data("sudung", 0);
    showConfirmationSeal1(valueClear, defaultValues, $(this))
});
function showConfirmationSeal1(valueClear, defaultValues, $this) {
    Swal.fire({
        title: 'Thực hiện hành vi?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Xóa seal lỗi',
        cancelButtonText: 'Đổi seal',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button id="Clear1" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;">Hủy</button>'

    }).then((result) => {
        if (result.isConfirmed) {
            updateSavefalse(valueClear, defaultValues, 2)
            console.log(valueClear)
            $this.trigger('change.select2');
            $this.find(`option[value="${valueClear}"]`).remove();
        }
    });
    $(".swal2-cancel").on("click", function () {
        let arr = defaultValues.split(',').map(item => item.trim());
        $this.trigger('change.select2');
    })
    $("#clear1").on("click", function () {
        var arr = goparrTable.split(',').map(item => item.trim());
        $this.val(arr).trigger('change');
        Swal.close();
    })
}
//addListXe
$(".listXe").on("click", function () {
    if (xeTai != 1) {
        toastr.error("Vui lòng chọn xe tải")
        return;
    }
    //$("#tbodyLists").empty()
    $(".input_popup").val("")
    $(".popup3").show()
    $(".theadSL").html(`Số lượng <p>(${totalSoLuongTable})</p>`)
    $(".theadSoKien").html(`Số kiện <p>(${totalSoKienTable})</p>`)
    getDsXe()
})
let vitriclass = 0;
$(".addRow_popup").on("click", function () {
    addListXe()
    renderMaSealList()
})
$(".clearlistTaxe").on("click", function () {
    $("#tbodyLists").empty()
    $("#tfootLists").empty()
})
$('.CMND').on('focus', function () {
    $(this).attr('inputmode', 'numeric'); // Tạo giao diện kiểu số cho bàn phím trên di động
}).on('blur', function () {
    $(this).removeAttr('inputmode'); // Khôi phục trạng thái ban đầu
});
function addListXe() {
    let html = ""
    let giatri = $(".input_popup").val()
    for (var i = 0; i < giatri; i++) {
        html += `
             <tr data-vitri=" ${vitriclass}">
                <td class="tenTaiXeList">
                <input style="text-transform: capitalize;" class="tenTaiXeListInput" type="text" name="name"  value="" autocomplete="off"/>
                </td>
                <td  class="CMNDList">
                  <input class="CMNDListInput" type="number" name="name"  value="" maxlength="12" autocomplete="off"/>
                </td>
                <td>
                    <input style="text-transform: uppercase;" class="soxeListInput" type="text" name="name" value="" autocomplete="off"/>
                </td>
                <td><select multiple="multiple" class="maSealSelectList">
                            </select></td>
                 <td >
                 <input class="SLSPistInput" type="number" name="name" value="" autocomplete="off"/>

                </td>
                <td >
                 <input class="SLThungListInput" type="number" name="name" value="" autocomplete="off"/>

                </td>
                 
            </tr>`
        vitriclass++;
    }
    let htmlTfoat = `

                <tr id="thead" style="position:relative">
                                        <td colspan="4"></td>
                                        <td id="tfootSL">0</td>
                                        <td id="tfootSoKien">0</td>
                                    </tr>`
    $("#tbodyLists").append(html)
    $("#tfootLists").html(htmlTfoat)
}
let $tridListXe = ""
$("#tbodyList1 ").on("click", "tr", function () {
    let $trTenTaiXe = $(this).data("tentaixe")
    let $trCMND = $(this).data("cmnd")
    eachTable($trTenTaiXe, $trCMND)
    $(".popup4 ").hide()
})
$("#tbodyLists ").on("click", "tr", function () {
    $("#tbodyLists tr").removeClass("active1")
    $(this).addClass("active1")
    $tridListXe = $(this).data("vitri")
})
function eachTable($trTenTaiXe, $trCMND) {
    $("#tbodyLists tr").each(function () {
        if ($(this).data("vitri") === $tridListXe) {
            $(this).find(".tenTaiXeListInput").val($trTenTaiXe)
            $(this).find(".CMNDListInput").val($trCMND)
        }
    });
}
$("#tbodyLists").on("input", ".SLThungListInput", function () {
    let value = $(this).val();
    let totalSL = $(".SLThungListInput")
        .map(function () {
            return parseFloat($(this).val()) || 0;
        }).get()
        .reduce((sum, value) => sum + value, 0);
    if (totalSL > totalSoLuongTable) {
        toastr.error("Vui lòng không được nhập vượt tổng SLSP ")
        $(this).val(value.slice(0, -1));

        return;
    }
    $("#tfootSL").text(totalSL)
})
$("#tbodyLists").on("input", ".SLSPistInput", function () {
    let value = $(this).val();
    let totalSThung = $(".SLSPistInput")
        .map(function () {
            return parseFloat($(this).val()) || 0;
        }).get()
        .reduce((sum, value) => sum + value, 0);
    if (totalSThung > totalSoKienTable) {
        toastr.error("Vui lòng không được nhập vượt tổng số lương thùng ")
        $(this).val(value.slice(0, -1));

        return;
    }
    $("#tfootSoKien").text(totalSThung)
})

$(".popup3").on("click", ".hide_popuplist", function () {
    $(".popup3").hide()
})
$("#tbodyLists").on("change", ".maSealSelectList", function () {
    let $this = $(this);
    let valueSeaNew = $this.val();
    let lastValue = ""
    if (valueSeaNew && valueSeaNew.length > 0) {
        lastValue = valueSeaNew[valueSeaNew.length - 1];
    }
    $("#tbodyLists tr").each(function () {
        let $tr = $(this);
        let $select = $tr.find(".maSealSelectList");

        if ($select[0] !== $this[0]) {
            $select.find("option").each(function () {
                if ($(this).val() == lastValue) {
                    $(this).remove();
                }
            });
        }
    });
});
$('.maSealSelectList').select2({
    placeholder: 'Select options',
    allowClear: true,

});
$("#theadLists ").on("click", ".listTenTXAll", function () {
    if ($tridListXe === "") {
        toastr.error("Vui lòng chọn dòng để thêm tên hoặc cmnd")
        return;
    }
    $(".popup4 ").show()
    $(".search_table1").val("")
    //$("#tfootSL").text("")
    //$("#tfootSoKien").text("")
    renderTenTaiXe()
})


$(".hide_popup1").on("click", function () {
    $(".popup4 ").hide()
})
$(".savelistTaxe").on("click", function () {
    let firstColumnValuesName = $("#tbodyLists tr")
        .find(".tenTaiXeListInput")
        .map(function () {
            return $(this).val();
        }).get().join(',');
    let firstColumnValuesCMND = $("#tbodyLists tr")
        .find(".CMNDListInput")
        .map(function () {
            return $(this).val();
        }).get().join(',');
    let firstColumnValuesSoXe = $("#tbodyLists tr")
        .find(".soxeListInput")
        .map(function () {
            return $(this).val();
        }).get().join(',');
    let sealValues = $("#tbodyLists tr")
        .find(".maSealSelectList")
        .map(function () {
            return $(this).val();
        }).get().flat().join(',');


    $(".CMND").val(firstColumnValuesCMND)
    $(".nameTX").val(firstColumnValuesName)
    $(".banSo").val(firstColumnValuesSoXe)
    let defaultValues = sealValues.split(",").map(item => item.trim())
    $(".maSealSelect ").val(defaultValues).trigger('change');
    if ($("#tfootSL").text() == 0) {
        toastr.error("Vui lòng nhâp số lượng sản phẩm ")
        return;
    }
    if ($("#tfootSoKien").text() == 0) {
        toastr.error("Vui lòng nhâp số lượng thùng ")
        return;
    }
    $(".popup3").hide()

});
function capitalizeFirstLetter(str) {
    return str
        .toLowerCase()
        .split(' ')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');
}

function saveListTaiXe() {
    let arrListTaiXe = [];
    if ($("#tbodyLists tr").length > 0) {
        $("#tbodyLists tr").map(function () {
            if ($(this).find(".tenTaiXeListInput").val() != "") {
                let $this = $(this)
                let valueSelected = $this.find(".maSealSelectList").val().join(",")
                let textSelected = $this.find(".maSealSelectList option:selected").map(function () {
                    return $(this).text()
                }).get().join(",")
                let object = {
                    MaCont: valCont,
                    MaSeal: valueSelected,
                    TenSeal: textSelected,
                    MaPKL: valPKL,
                    TenTaiXe: capitalizeFirstLetter($this.find(".tenTaiXeListInput").val()),
                    CMND: $this.find(".CMNDListInput").val(),
                    SoXe: $this.find(".soxeListInput").val().toUpperCase(),
                    SLSP: $this.find(".SLThungListInput").val(),
                    SLThung: $this.find(".SLSPistInput").val(),
                    Module: 2

                };
                arrListTaiXe.push(object);
            }
        })
    }
    else {
        var selectedValues = $('.maSealSelect').val();
        var selectedString = selectedValues.join(',');
        var selectedText = $('.maSealSelect option:selected ').map(function () {
            return $(this).text()
        }).get().join(",");
        console.log(selectedText, selectedString)
        let object = {
            MaCont: valCont,
            MaSeal: selectedString,
            TenSeal: selectedText,
            MaPKL: valPKL,
            TenTaiXe: capitalizeFirstLetter($(".nameTX").val()),
            CMND: $(".CMND").val(),
            SoXe: $(".banSo").val().toUpperCase(),
            SLSP: totalSoKienTable,
            SLThung: totalSoLuongTable,
            Module: 2

        };
        arrListTaiXe.push(object);
    }
    if (arrListTaiXe.length > 0)
        postListTaiXe(arrListTaiXe)
}
function postListTaiXe(arrListTaiXe) {
    $.ajax({
        url: '/api/BienBanLuuSeal/PostDSXe',
        type: "Post",
        data: JSON.stringify(arrListTaiXe),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

        }
    })
}


function renderMaSealList() {
    $.ajax({
        async: false,
        url: "/api/XuatHang/Get?Action=GetSeal&para1=A&para2=A",
        type: 'Get',
        success: function (data) {
            if (module == 1) {
                data = data.filter(item => item.Status == 0)
            }
            let html = ``
            let arrSeal = [];
            data.map(item => {
                if (item.Status != 0) {
                    arrSeal.push(item.MaSeal)
                }
                html += `
                        <option style="color:#fff" data-sudung='${item.Status}' value="${item.MaSeal}">${item.Display}</option>
                    `
            })
            $("#tbodyLists tr").each(function () {
                let $tr = $(this)
                let $select = $tr.find(".maSealSelectList");
                if ($select.children('option').length === 0) {
                    $select.html(html);
                }
            })

            $(".maSealSelectList").select2()
            $(".maSealSelectList").on('select2:open', function () {
                setTimeout(function () {
                    arrSeal.map(item => {
                        $(`li[id*='${item}']`).addClass("hightline_seal");
                    })
                }, 0);
            });
        }
    })
}

function getDsXe() {
    let arrDsSeal = []
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetDSXe&para1=${valCont}&para2=${valPKL}&para3=a&para4=a&para5=a`,
        type: "GET",
        success: function (data) {
            let html = '';
            let totaltfootSLSP = data.reduce((acc, curr) => acc + parseInt(curr.SLSP), 0);
            let totaltfootSLThung = data.reduce((acc, curr) => acc + parseInt(curr.SLThung), 0);
            data.map((item, index) => {
                html += `
                <tr data-vitri="${vitriclass}">
                    <td class="tenTaiXeList">
                        <input style="text-transform: capitalize;" class="tenTaiXeListInput" type="text" value="${item.TenTaiXe}" autocomplete="off"/>
                    </td>
                    <td class="CMNDList">
                        <input class="CMNDListInput" type="number" value="${item.CMND}" autocomplete="off"/>
                    </td>
                    <td>
                        <input style="text-transform: uppercase;" class="soxeListInput" type="text" value="${item.SoXe}" autocomplete="off"/>
                    </td>
                    <td><select multiple="multiple" class="maSealSelectList">
                            </select></td>

                    <td>
                        <input class="SLSPistInput" type="number" value="${item.SLThung}" autocomplete="off"/>
                    </td>
                        <td>
                        <input class="SLThungListInput" type="number" value="${item.SLSP}" autocomplete="off"/>
                    </td>
                   
                </tr>`;
                vitriclass++;
                arrDsSeal.push(item.MaSeal)
            });
            $("#tbodyLists").html(html);
            if ($("#tfootSL").text() == 0 && $("#tfootSoKien").text() == 0) {
                let htmlTfoat = `

                <tr id="thead" style="position:relative">
                                        <td colspan="4"></td>
                                        <td id="tfootSoKien">0</td>
                                        <td id="tfootSL">0</td>
                                       
                                    </tr>`
                $("#tfootLists").html(htmlTfoat)
            }
            renderMaSealList();
            renderListSeal(arrDsSeal)
        }
    });
}
function renderListSeal(arrDsSeal) {
    $("#tbodyLists tr").each(function (index) {
        let valuesSplit = arrDsSeal[index].split(',').map(item => item.trim());
        $(this).find("td .maSealSelectList").val(valuesSplit).trigger('change');
    });
}
let previousValue = null;
$(".maSealSelect").on("select2:select", function (e) {
    if (module == 2) {
        let selectedValue = e.params.data.id; // Giá trị vừa được chọn
        let valseal = $(this).find("option[value='" + selectedValue + "']").data("sudung");

        if (valseal == 1 && previousValue != null) {
            toastr.error("Seal này đã được sử dụng");
            $(this).val(previousValue).trigger('change'); // Đặt lại giá trị trước đó
        }
    }
});
$(".selectseal").on("click", function (e) {
    previousValue = $(".maSealSelect").val()
});
let previousValueSeal = null
$("#tbodyLists ").on("click", "tr", function () {
    if (module == 2) {
        previousValueSeal = $(this).find(".maSealSelectList").val()
    }
})
$("#tbodyLists ").on("select2:select", ".maSealSelectList", function (e) {
    if (module == 2) {
        let selectedValue = e.params.data.id;
        let valseal = $(this).find("option[value='" + selectedValue + "']").data("sudung");
        if (valseal == 1) {
            toastr.error("Seal này đã được sử dụng");
            $(this).val(previousValueSeal).trigger('change');
        }
    }
})

function deleteDSXe() {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=DeleteDSXe&para1=${valCont}&para2=${valPKL}&para3=a&para4=a&para5=a`,
        type: "GET",
        success: function (data) {

        }
    });
}
