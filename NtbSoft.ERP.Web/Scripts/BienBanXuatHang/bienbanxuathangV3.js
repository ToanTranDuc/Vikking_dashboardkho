
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
let hotenValue = "";
let bophanValue = ""
let countKiten = 0;
let linkImageNoiBo = "";
let maPhieu = "";
let tenBienBan = "";
let moduleWin = ""
let user = localStorage.getItem("username1")
let user1 = data.userName
let checkSave = 1;

$("#home").on("click", function () {
    window.location.href = '/Home/Dashboard'
})
let dataPhieuBB = data.phieuBB
var today = new Date();
$("#from").datepicker({
    defaultDate: "+1w",
    changeMonth: true,
    changeYear: true, // Cho phép chọn năm
    numberOfMonths: 1,
    dateFormat: "yy-mm-dd",
    onClose: function (selectedDate) {
        $("#to").datepicker("option", "minDate", selectedDate);
    }
});
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
$("#to").datepicker({
    defaultDate: "+1w",
    changeMonth: true,
    changeYear: true, // Cho phép chọn năm
    numberOfMonths: 1,
    dateFormat: "yy-mm-dd",
    onClose: function (selectedDate) {
        $("#from").datepicker("option", "maxDate", selectedDate);
    }
});
let isContT = [];
let isRomocT = [];
let isXeTaiT = [];
function setValue1() {
    for (let i = 1; i <= 19; i++) {
        isContT[i] = 1;
        isXeTaiT[i] = 0;
    }
}


for (let i = 1; i <= 19; i++) {
    $(`.isContT${i}`).on("change", function () {
        $(`.isContKT${i}`).prop("checked", false);
        isContT[i] = this.checked ? 1 : 0;
    });
    $(`.isXeTaiT${i}`).on("change", function () {
        $(`.isXeTaiKT${i}`).prop("checked", false);
        isXeTaiT[i] = this.checked ? 1 : 0;
    });
}

for (let i = 1; i <= 19; i++) {
    $(`.isContKT${i}`).on("change", function () {
        $(`.isContT${i}`).prop("checked", false);
        isContT[i] = this.checked ? 2 : 0;
    });


    $(`.isXeTaiKT${i}`).on("change", function () {
        $(`.isXeTaiT${i}`).prop("checked", false);
        isXeTaiT[i] = this.checked ? 2 : 0;
    });
}
var dd = String(today.getDate()).padStart(2, '0');
var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
var yyyy = today.getFullYear();
today = yyyy + '-' + mm + '-' + dd;
$(".lookup-results").hide();
$(".lookup-results1").hide();
$(".ultitle").hide()
$("#from").val(today)
$("#to").val(today)

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




///
$(".lookup-input2").on("click", function () {
    $(".hideLi").show()
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
    //setTimeout(function () {
    //    if (!$(document.activeElement).hasClass('lookup-input2')) {
    //        $(".lookup-results1").hide();
    //        $(".lookup-input1").hide();
    //    }
    //}, 200);

});
$(".lookup-input").on("blur", function () {
    setTimeout(function () {
        $(".lookup-results").hide();
        $(".ultitle").hide();

    }, 200);

});
function retrun() {
    deleteRow = 0;
    boCheckClass();
    isCheckNoiDung()
    renderValRong()
    hideImage()
    AddCheckClass()
    renderCongTy()
    renderMaPKL()
}
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
$(".lookup-results").on("click", "li", function () {
    var selectedValue = $(this).data("name")
    maPhieu = $(this).data("value")
    valCont = $(this).data("cont")
    valMaContModule2 = $(this).data("masocont")
    valMaContModule2V2 = $(this).data("masocont")
    tenBienBan = selectedValue;
    valNgayNhapKho = $(this).data("ngaynhapkho")
    $(".lookup-input").val(selectedValue)
    $(".lookup-results").hide();
    $(".ultitle").hide()
    renValueNoiDung()
    $(".camera_car_back").show()
    $(".camera_car_back").text("Chụp hình lại")
    $(".image_Cont").hide();
    $("#bodyKyTen").empty();
    hideDate()

});

$(document).on("click", ".list-img", function () {
    $(".popup").show()
})
$("#to").on("change", function () {
    renderMaPhieu()
})
$("#from").on("change", function () {
    renderMaPhieu()
})
let ketLuan = 1
let xeVC = 0;
let xeRoMoc = 0;
$(".isKetLuanT").on("change", function () {
    $(`.isKetLuanKT`).prop("checked", false);
    ketLuan = 1;
})
$(".isKetLuanKT").on("change", function () {
    $(`.isKetLuanT`).prop("checked", false);
    ketLuan = 2;
})

$(".vcCont").on("change", function () {
    $(`.vcXetai`).prop("checked", false);
    $(`.vcroMoc`).prop("checked", true);
    xeRoMoc = this.checked ? 1 : 0;;
    xeVC = 1;
    setValue1();
    boCheckClass()
    AddCheckClass()
    $(".inputseal").show()
    $(".selectseal").hide()
    $(".inputcontainer").show()
    $(".inputromoc").show()
})
$(".vcroMoc").on("change", function () {
    $(`.vcXetai`).prop("checked", false);
    xeRoMoc = this.checked ? 1 : 0;;
})
$(".vcXetai").on("change", function () {
    $(`.vcCont`).prop("checked", false);
    $(`.vcroMoc`).prop("checked", false);
    xeVC = 2;
    setValueXeTai();
    checkClassXeTai();
    $(".inputseal").hide()
    $(".selectseal").show()
    $(".inputcontainer").hide()
    $(".inputromoc").hide()
    xeRoMoc = 0;
})

function setValueXeTai() {
    for (let i = 1; i <= 19; i++) {
        isContT[i] = 0;
        isXeTaiT[i] = 1;
    }
}

function xoaDauVaKhoangCach(str) {
    str = str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    str = str.replace(/\s+/g, "_");
    return str;
}
let dataPhieu = [];
function duLieuPhieu() {
    dataPhieu = []
    if (module == 2) {
        getDSPhieu()
    }
    else {
        renderDataPost(arrMaPKL_Cont)
    }
}


function getDSPhieu() {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetPhieu&para1=${maPhieu}&para2=a&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            renderDataPost(data)
        }
    })
}
function DeleteSealAndDsXe() {
    if (module == 2) {
        getDSPhieu()
    }
    else {
        renderDataPost(arrMaPKL_Cont)
    }
}
function renderMaPhieu() {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetPhieu&para1=${maPhieu}&para2=a&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            renderDataPost(data)
        }
    })
}
function getMaxPhieu(arrDataPost) {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetMaPhieu&para1=${maPhieu}&para2=a&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            let maphieubb = 0;
            if (data === null || data.length == 0) {
                maphieubb = 1
            } else {
                maphieubb = Number(data[0].MaPhieuBB) + 1
            }
            var selectedValues = $('.maSealSelect').val();
            var selectedString = selectedValues.join(',');

            if (sophieuGetApi == maphieubb) {
                $("#soPhieu").text(sophieuGetApi + 1)
                maphieubb = maphieubb + 1
            }
            if (module == 2) maphieubb = maPhieu
            arrDataPost.map(item => {
                let object = {
                    MaPhieuBB: maphieubb,
                    NgayLap: today,
                    VanChuyen: xeVC,
                    LoaiFeets: $(".loaiFeest").val(),
                    LoaiTan: $(".loaiTan").val(),
                    SoXe: $(".banSo").val().toUpperCase(),
                    SoCont: $(".container").val(),
                    SoRoMoc: $(".romoc").val(),
                    TenTaiXe: capitalizeFirstLetter($(".nameTX").val()),
                    CMND: $(".CMND").val(),
                    CongTy: $(".congty").val(),
                    NgayXeDen: $(".xeDen").val(),
                    NgayGioCheckXeRoMoc: $(".checkXe").val(),
                    NgayGioDongHang: $(".donghang").val() == "" ? '1990-1-1' : $(".donghang").val(),
                    NgayGioKTDongHang: $(".finishDongHang").val() == "" ? '1990-1-1' : $(".finishDongHang").val(),
                    MaSeal: xeVC == 2 ? selectedString : $(".maSeal").val(),
                    NgayGioXeDi: $(".xeDI").val() == "" ? "1990-1-1" : $(".xeDI").val(),
                    XeRomoc: xeRoMoc,
                    SoContCu: item.MaCont,
                    MaPKL_XH: item.MaPKL,
                    MaCongTy: $(".congty option:selected").data("congty"),
                    KetLuan: ketLuan == 1 ? 2 : 1,
                    NhanVienKiem: moduleWin == 5 ? user1 : user,
                    TenBienBan: tenBienBan,
                    CangDen: $(".cangden").val().toUpperCase(),
                    ChungTu: $(".chungtu").val().toUpperCase(),
                    TGBVCheckBD: $(".bvcheckbd").val(),
                    TGBVCheckKT: $(".bvcheckkt").val(),
                    TGNVCheckBD: $(".nvcheckbd").val(),
                    TGNVCheckKT: $(".nvcheckkt").val(),
                }
                dataPhieu.push(object)
            })
            postAPIDLPhieu(dataPhieu, maphieubb, arrDataPost)
        }
    })
}
async function renderDataPost(arrDataPost) {
    getMaxPhieu(arrDataPost)

}

function postAPIDLPhieu(dataPhieu, maphieubb, arrDataPost) {
    saveListSeal(maphieubb, 1, arrDataPost)
    $.ajax({
        type: "POST",
        url: "/api/XuatHang/PostBBXuatHang",
        data: JSON.stringify(dataPhieu),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#Update").prop("disabled", false);
            saveListTaiXe(arrDataPost)
            eachDivImage(maphieubb)
            renderMaPhieuEndUpdate()
            getTenBienBanEndSave()
            luuNoiDung(maphieubb);
            saveImageOther(maphieubb)
            saveKyTen(maphieubb)
            saveGhiChu();
            if (module == 2)
                return;
            else $('.item_addImage').empty();
            renderMaSeal()
            $("html, body").animate({ scrollTop: 0 }, 500);
            setTimeout(function () {
                $(`.vcCont`).prop("checked", true);
                $(`.vcroMoc`).prop("checked", true);
                xeVC = 1
                xeRoMoc = 1
            }, 500)
            retrun()

            $(".item_input2").empty()
        },
        error: function (errormessage) {
            $("#Update").prop("disabled", false);
        }

    });
}
function getTenBienBanEndSave() {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetTenBienBan&Para1=A&Para2=A&Para3=a",
        type: 'Get',
        success: function (data) {
            let tenBienBanEnd = ""
            if (data.length != 0)
                tenBienBanEnd = data[0].TenBienBan
            getTenBienBanAddEndSave(tenBienBanEnd)
        }
    })
}
function getTenBienBanAddEndSave(tenBienBanEnd) {
    let month = today.split("-")[1]
    let day = today.split("-")[2]
    let bienbanMonth = tenBienBanEnd == "" || tenBienBanEnd == null ? "" : tenBienBanEnd.split("|")[2]
    if (tenBienBanEnd == "" || tenBienBanEnd == null || bienbanMonth != month) {
        tenBienBanEnd = `XTP|${addWithPadding(1, 0)}|${month}`
    }
    else {
        let bienban = tenBienBanEnd.split("|")[1]
        tenBienBanEnd = `XTP|${addWithPadding(Number(bienban), 1)}|${month}`
    }
    $("#namePhieu").text(`(${tenBienBanEnd})`)
}
function saveAll() {
    duLieuPhieu();
    let phieu = $("#soPhieu").text()
    $("#soPhieu").text(Number(phieu) + 1)
    toastr.success('Lưu thành công')

}

$("#Update").on("click", function () {
    $(this).prop("disabled", true);
    if (module == 2) {
        if ($(".container").val().trim() == "") {
            toastr.error("Không có phiếu")
        } else {
            saveAll()
        }

    } else {
        if (ketLuan == 2) {
            if (($(".banSo").val() != "" || xeVC == 2) &&
                $(".container").val() != "" &&
                //($(".romoc").val() != "" || xeVC == 2) &&
                $(".nameTX").val() != "" &&
                $(".CMND").val() != "" &&
                $(".xeDen").val() != "" &&
                $(".checkXe").val() != "" &&
                $(".bvcheckbd").val() != "" &&
                $(".bvcheckkt").val() != "") {
                setTenBienBan()

            } else {
                ThongBao()
                $(this).prop("disabled", false);
            }
        } else {
            if (($(".banSo").val() != "" || xeVC == 2) &&
                $(".container").val() != "" &&
                //($(".romoc").val() != "" || xeVC == 2) &&
                $(".nameTX").val() != "" &&
                $(".CMND").val() != "" &&
                $(".xeDen").val() != "" &&
                $(".checkXe").val() != "" &&
                $(".donghang").val() != "" &&
                $(".finishDongHang").val() != "" &&
                $(".xeDI").val() != "") {
                setTenBienBan()

            } else {
                ThongBao()
                $(this).prop("disabled", false);
            }
        }
    }
})
function ThongBao() {
    var errorMessage = '';
    if ($(".banSo").val().trim() == "") {
        errorMessage += 'Bản số xe bị rỗng ';
        $(".banSo").focus();
    } else if ($(".nameTX").val().trim() == "") {
        errorMessage += 'Tên TX bị rỗng ';
        $(".nameTX").focus();
    }
    else if ($(".CMND").val().trim() == "") {
        errorMessage += 'CCCD bị rỗng ';
        $(".CMND").focus();
    }
    else if ($(".xeDen").val().trim() == "") {
        errorMessage += 'Giờ xe đến bị rỗng ';
        $(".xeDen").focus();
    }
    else if ($(".checkXe").val().trim() == "") {
        errorMessage += 'Ngày ETD bị rỗng ';
        $(".checkXe").focus();
    }
    else if ($(".donghang").val().trim() == "") {
        errorMessage += 'TG bắt đầu đóng cont ';
        $(".donghang").focus();
    }
    else if ($(".finishDongHang").val().trim() == "") {
        errorMessage += 'TG kết thúc đóng cont ';
        $(".finishDongHang").focus();
    }
    else if ($(".xeDI").val().trim() == "") {
        errorMessage += 'Ngày giờ xe đi  bị rỗng ';
        $(".xeDI").focus();
    }
    else if ($(".bvcheckbd").val().trim() == "") {
        errorMessage += 'Thời gian BV kiểm tra từ bị rỗng';
        $(".bvcheckbd").focus();
    }
    else if ($(".bvcheckkt").val().trim() == "") {
        errorMessage += 'Thời gian BV kiểm tra đến bị rỗng';
        $(".bvcheckkt").focus();
    }
    else if ($(".nvcheckbd").val().trim() == "") {
        errorMessage += 'Thời gian NV kho kiểm tra từ bị rỗng';
        $(".nvcheckbd").focus();
    }
    else if ($(".nvcheckkt").val().trim() == "") {
        errorMessage += 'Thời gian NV kho kiểm tra đến bị rỗng';
        $(".nvcheckkt").focus();
    }
    toastr.error('Vui lòng nhập đầy đủ thông tin cho các ô sau: ' + errorMessage);
    $("#Update").prop("disabled", false);

}
let arrNoiDung = [];

function luuNoiDung(maphieu) {
    let maphieubb = maphieu;
    if (module == 2) maphieubb = maPhieu
    let index = 1;
    arrNoiDung = []
    $('#tbody tr').slice(0, -1).each(function (i) {
        if (i == 0 || i == 18) {
            return true;
        }
        var secondColumnText = $(this).find('td').eq(1).text();
        let object = {
            MaBBPhieu: maphieubb,
            MaNoiDung: index,
            NoiDung: secondColumnText,
            StatusCont: isContT[index],
            StatusRoMoc: 0,
            StatusXeTai: isXeTaiT[index],
            StatusKL: ketLuan,
        }
        index++;
        arrNoiDung.push(object)
    });
    $.ajax({
        type: "POST",
        url: "/api/XuatHang/PostNDKT",
        data: JSON.stringify(arrNoiDung),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {


        }
    });
}
function addWithPadding(a, b) {
    let result = a + b;
    return result.toString().padStart(2, '0');
}
let bienBanFoder = ""
let sophieuGetApi = "";
function getTenBienBan() {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetTenBienBan&Para1=A&Para2=A&Para3=a",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                tenBienBan == "";
            } else {
                tenBienBan = data[0].TenBienBan
                sophieuGetApi = data[0].MaPhieu

            }
            getTenBienBanADD()
        }
    })
}
function setTenBienBan() {
    if (module == 1) {
        getTenBienBan()
    }
    else {
        getTenBienBanADD()
    }
}
function getTenBienBanADD() {
    let month = today.split("-")[1]
    let day = today.split("-")[2]
    let bienbanMonth = tenBienBan == "" || tenBienBan == null ? "" : tenBienBan.split("|")[2]
    if (tenBienBan == "" || tenBienBan == null || bienbanMonth != month) {
        tenBienBan = `XTP|${addWithPadding(1, 0)}|${month}`
    }
    else {
        let bienban = tenBienBan.split("|")[1]
        tenBienBan = `XTP|${addWithPadding(Number(bienban), 1)}|${month}`
    }
    saveAll()
}
function renderValRong() {
    $(".loaiFeest").val("")
    $(".loaiTan").val("")
    $(".chungtu").val("")
    $(".banSo").val("")
    $(".container").val("")
    $(".romoc").val("")
    $(".nameTX").val("")
    $(".CMND").val("")
    $(".xeDen").val("")
    $(".xeDI").val("")
    $(".checkXe").val("")
    $(".donghang").val("")
    $(".finishDongHang").val("")
    $(".maSeal").val("")
    $("#datetime_xeden").text("dd/mm/yyyy --:--")
    $("#datetime_xecheck").text("dd/mm/yyyy")
    $("#datetime_giodong").text("dd/mm/yyyy --:--")
    $("#datetime_ktgiodong").text("dd/mm/yyyy --:--")
    $("#datetime_xedi").text("dd/mm/yyyy --:--")
    $("#datetime_bvcheckbd").text("dd/mm/yyyy --:--")
    $("#datetime_bvcheckkt").text("dd/mm/yyyy --:--")
    $("#datetime_nvcheckbd").text("dd/mm/yyyy --:--")
    $("#datetime_nvcheckkt").text("dd/mm/yyyy --:--")
    $(".cangden").val("")
}

function handleDateChange(selector, outputSelector) {
    $(selector).on("change", function () {
        const datetime = $(this).val();
        if (selector == ".bvcheckbd") {
            $(".bvcheckbd").val(datetime)
            $(".bvcheckkt").val(datetime)
            $("#datetime_bvcheckbd").text(formatDateTime(datetime))
            $("#datetime_bvcheckkt").text(formatDateTime(datetime))
        }
       else if (selector == ".nvcheckbd") {
            $(".nvcheckbd").val(datetime)
            $(".nvcheckkt").val(datetime)
            $("#datetime_nvcheckbd").text(formatDateTime(datetime))
            $("#datetime_nvcheckkt").text(formatDateTime(datetime))
        }
       else if (selector == ".checkXe") {
            $(outputSelector).text(formatDateTimeAdd(datetime));
        } else {
            $(outputSelector).text(formatDateTime(datetime));
        }
    });
}
handleDateChange(".xeDen", "#datetime_xeden");
handleDateChange(".checkXe", "#datetime_xecheck");
handleDateChange(".donghang", "#datetime_giodong");
//handleDateChange(".finishDongHang", "#datetime_ktgiodong");
handleDateChange(".xeDI", "#datetime_xedi");
handleDateChange(".bvcheckbd", "#datetime_bvcheckbd");
handleDateChange(".bvcheckkt", "#datetime_bvcheckkt");
handleDateChange(".nvcheckbd", "#datetime_nvcheckbd");
handleDateChange(".nvcheckkt", "#datetime_nvcheckkt");

$(".finishDongHang").on("change", function () {
    let ngaythang = $(this).val()
    if (module == 1) {
        if (arrMaPKL_Cont.length == 0) {
            toastr.error("Vui lòng chọn pkl")
            return;
        }
        arrMaPKL_Cont.map(item => {
            let ngaythangngayNK = item.NgayNhapKho

            var parts1 = formatDateTimeAdd(ngaythang).split('/');
            var parts2 = formatDateTimeAdd(ngaythangngayNK).split('/');

            var d1 = new Date(parts1[2], parts1[1] - 1, parts1[0]);
            var d2 = new Date(parts2[2], parts2[1] - 1, parts2[0]);
            if (d1 < d2) {
                Swal.fire({
                    title: `Ngày kt đóng hàng ${formatDateTimeAdd(ngaythang)} không được nhỏ hơn ngày nhập kho ${formatDateTimeAdd(ngaythangngayNK)}`,
                    icon: 'warning',
                    confirmButtonText: 'OK',
                    confirmButtonColor: '#3085d6',
                    customClass: {
                        confirmButton: 'custom-confirm-button'
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        $(this).val("");
                        $("#datetime_ktgiodong").text("dd/mm/yyyy --:--")
                        return;
                    }
                });
            }

        })
        $("#datetime_ktgiodong").text(formatDateTime(ngaythang))
    }
    else {
        $.ajax({
            url: `/api/BienBanLuuSeal/Get?Action=GetNgayNhapKho&para1=${maPhieu}&para2=a&para3=a&para4=a&para5=a`,
            type: "Get",
            success: function (data) {
                data.map(item => {
                    let ngaythangngayNK = item.NgayNhapKho

                    var parts1 = formatDateTimeAdd(ngaythang).split('/');
                    var parts2 = formatDateTimeAdd(ngaythangngayNK).split('/');

                    var d1 = new Date(parts1[2], parts1[1] - 1, parts1[0]);
                    var d2 = new Date(parts2[2], parts2[1] - 1, parts2[0]);
                    if (d1 < d2) {
                        Swal.fire({
                            title: `Ngày kt đóng hàng ${formatDateTimeAdd(ngaythang)} không được nhỏ hơn ngày nhập kho ${formatDateTimeAdd(ngaythangngayNK)}`,
                            icon: 'warning',
                            confirmButtonText: 'OK',
                            confirmButtonColor: '#3085d6',
                            customClass: {
                                confirmButton: 'custom-confirm-button'
                            }
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $(this).val("");
                                $("#datetime_ktgiodong").text("dd/mm/yyyy --:--")
                                return;
                            }
                        });
                    }

                })
                $("#datetime_ktgiodong").text(formatDateTime(ngaythang))
            }
        })
    }


})
function isCheckNoiDung() {
    $(`.vcCont`).prop("checked", false);
    $(`.vcroMoc`).prop("checked", false);
    $(`.vcXetai`).prop("checked", false);
    $(`.isKetLuanT`).prop("checked", false);
    $(`.isKetLuanKT`).prop("checked", false);
}
function ischeckNoiDungVcCont() {
    $(`.vcCont`).prop("checked", true);
    $(`.vcroMoc`).prop("checked", true);
}
let module = 1;
$("#creatPhieu").on("click", function () {
    $(".item_input2").empty()
    previousValue = null;
    renderMaSeal()
    count = 9
    module = 1;
    setValue1()
    $("#tbodyLists").empty()
    $("#bodyMahang").empty();
    $(".mapkl_cont").show()
    $(".inputseal").show()
    $(".selectseal").hide()
    $(".inputcontainer").show()
    $(".inputromoc").show()
    $(".search").fadeOut(500);;
    $(".maPhieu").show()
    $("#Update").html(`<i class="fa fa-sharp fa-solid fa-floppy-disk"></i> Lưu `);
    $("#xuatEX").hide()
    $("#delete").hide()
    $("html, body").animate({ scrollTop: 0 }, 500);
    selectMaPhieu()
    $(".camera_car_back").hide()
    $(".camera_car").show()
    $(".camera_car").text("Chụp hình xe Cont")
    $(".ma_PKL").show();
    $("#bodyKyTen").empty()
    AddKyTen()
    $(".camera_imageOther_View").hide()
    $(".camera_imageOther_back").show()
    renderMaSeal()

})
$("#viewPhieu").on("click", function () {
    previousValue = null;
    module = 2;
    showDisable()
    $(".search").fadeIn(500);;
    $(".mapkl_cont").hide()
    renderMaPhieu()
    $(".maPhieu").hide()
    $("#xuatEX").show()
    $("#Update").html(`<i class="fa-solid fa-file-pen"></i> Update phiếu`)
    $("html, body").animate({ scrollTop: 0 }, 500);
    $(".camera_car").text("Chụp hình xe Cont")
    $(".camera_car").hide()
    $(".image_Cont").hide();
    $(".ma_PKL").hide();
    $(".camera_car_back").show()
    $(".camera_car_back").text("Chụp hình lại")
    $("#delete").show()
    $(".item_addImage").empty()
    $(".item_addImage").hide()
    $("#bodyKyTen").empty()
    $(".camera_imageOther_View").show()
    $(".camera_imageOther_back").hide()
    ischeckLength = false;
    divLength = 1;
    count = 9
    showDisable()
    hideDate()
})

function hideDate() {
    setTimeout(function () {
        if (module == 2) {
            if (ketLuan == 2) {
                $(".donghang").val("")
                $(".finishDongHang").val("")
                $(".maSeal").val("")
                $(".xeDI").val("")
                $("#bodyMahang").empty();
                $(".camera_car_back").hide();
            }
        }
    }, 200)

}
let valMaContModule2 = "";
let valMaContModule2V2 = "";
let isCheckDN = localStorage.getItem('isCheckDN');
//if (isCheckDN == 'false') {
//    window.location.href = '/Login/Index'
//}
let remember = localStorage.getItem('remember');
if (remember == 'false') {
    localStorage.setItem('isCheckDN', 'false');
}
function renderMaPhieu() {
    $(".lookup-results").empty()
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetMaPhieu&para1=&para2=A",
        type: 'Get',
        success: function (data) {
            isCheckNoiDung()
            renderValRong()
            boCheckClass()

            if (data.length == 0) {
                $(".lookup-input").val("")
                renderImage()
                return;
            }
            let tenBB = data[0].TenBienBan
            valMaContModule2 = data[0].Cont
            valMaContModule2V2 = data[0].Cont
            if (!window.CefSharp)
                maPhieu = data[0].MaPhieuBB;
            valCont = data[0].SoCont;
            valNgayNhapKho = data[0].NgayNhapKho
            $(".lookup-input").val(tenBB)
            $.each(data, function (index, item) {

                $(".lookup-results").append(`
                    <li class="result-row" data-cont="${item.SoCont}" data-ngaynhapkho="${item.NgayNhapKho}"  data-masocont="${item.Cont}" data-value=${item.MaPhieuBB} data-name="${item.TenBienBan}">
                        <div class="result-column">${item.TenBienBan} </div>
                        <div class="result-column">${item.SoXe} </div>
                        <div class="result-column">${item.SoCont}</div>
                        <div class="result-column">${item.SoRoMoc}</div>
                        <div class="result-column">${item.SoBooking}</div>
                    </li>
                 `)
            })

            renValueNoiDung()
            rendeImage()
            renderImageOther()
        }
    })
}
function renderMaPhieuEndUpdate() {
    $(".lookup-results").empty()
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetMaPhieu&para1=&para2=A",
        type: 'Get',
        success: function (data) {
            let html = ``
            data.map(item => {
                html += `
            <li class="result-row" data-cont="${item.SoCont}" data-ngaynhapkho="${item.NgayNhapKho}"  data-masocont="${item.Cont}" data-value=${item.MaPhieuBB} data-name="${item.TenBienBan}">
                  <div class="result-column">${item.TenBienBan} </div>
                  <div class="result-column">${item.SoXe} </div>
                  <div class="result-column">${item.SoCont}</div>
                  <div class="result-column">${item.SoRoMoc}</div>
                  <div class="result-column">${item.SoBooking}</div>
              </li>
                `
            })
            $(".lookup-results").html(html)
        }
    })
}

function selectMaPhieu() {
    $.ajax({
        async: false,
        url: "/api/XuatHang/Get?Action=GetBBXuatHang&para1=A&para2=A",
        type: 'Get',
        success: function (data) {
            retrun();
            AddKyTen()
            setValue1()
            GetName()
            GetTableChuKy()
            ischeckNoiDungVcCont()
            xeVC = 1
            xeRoMoc = 1
            renderMaSeal()
            getTenBienBanEndSave()
            $(".item_addImage").empty()
            if (data.length == 0) {
                $("#soPhieu").text(1)
                console.log($("#soPhieu").text())
                tenBienBan = "";
                return;
            }
            if (module == 2) return
            else {
                let indexPhieu = data[0].MaPhieuBB
                tenBienBan = data[0].TenBienBan
                if (data[0].NgayLap != null)
                    indexPhieu++;
                $("#soPhieu").text(indexPhieu)
                /*deletePhieu()*/
                hideImage()
                $(".camera_car").text("Chụp hình xe Cont")
                $(".image_Cont").hide();
            }

        }
    })
}
function renderMaSeal() {
    $.ajax({
        async: false,
        url: `/api/XuatHang/Get?Action=GetSeal&para1=A&para2=A`,
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
            $(".maSealSelect").html(html)
            $(".maSealSelect").select2()
            if (module == 2) {
                $(".maSealSelect").on('select2:open', function () {
                    setTimeout(function () {
                        arrSeal.map(item => {
                            $(`li[id*='${item}']`).addClass("hightline_seal");
                        })
                    }, 0);
                });
            }
        }
    })
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
function formatDateTimeAdd(datetime) {
    const date = new Date(datetime);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}/${month}/${year}`;
}
function formatDateTimeAddA(datetime) {
    const date = new Date(datetime);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}
function renValueNoiDung() {
    renderMaSeal()
    GetName()
    GetTableChuKy()
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetBBTheoPhieu&para1=" + maPhieu + "&para2=A",
        type: 'Get',
        success: function (data) {
            previousValue = null
            boCheckClass()
            d = data[0]
            isCheckNoiDung()
            xeRoMoc = 0
            if (d.VanChuyen != 0) {
                if (d.VanChuyen == 1) {
                    $(`.vcCont`).prop("checked", true);
                    xeVC = 1
                    $(".inputcontainer").show()
                    $(".inputromoc").show()
                    $(".inputseal").show()
                    $(".selectseal").hide()
                }
                else if (d.VanChuyen == 2) {
                    $(`.vcXetai`).prop("checked", true);
                    xeVC = 2
                    $(".inputcontainer").hide()
                    $(".inputromoc").hide()
                    $(".inputseal").hide()
                    $(".selectseal").show()
                    setValueXeTai()
                }
            }
            if (d.XeRomoc != 0) {
                $(`.vcroMoc`).prop("checked", true);
                xeRoMoc = 1
            }
            $(".loaiFeest").val(d.LoaiFeets)
            $(".loaiTan").val(d.LoaiTan)
            $(".banSo").val(d.SoXe)
            $(".cangden").val(d.CangDen)
            $(".chungtu").val(d.ChungTu)
            $(".container").val(d.SoCont)
            $(".romoc").val(d.SoRoMoc)
            $(".nameTX").val(d.TenTaiXe)
            $(".CMND").val(d.CMND)
            $(".congty").val(d.CongTy)
            $(".xeDen").val(d.NgayXeDen)
            $("#datetime_xeden").text(formatDateTime(d.NgayXeDen))
            $(".checkXe").val(formatDateTimeAddA(d.NgayGioCheckXeRoMoc))
            $("#datetime_xecheck").text(formatDateTimeAdd(d.NgayGioCheckXeRoMoc))

            $(".bvcheckbd").val(d.TGBVCheckBD)
            $("#datetime_bvcheckbd").text(formatDateTime(d.TGBVCheckBD))
            $(".bvcheckkt").val(d.TGBVCheckKT)
            $("#datetime_bvcheckkt").text(formatDateTime(d.TGBVCheckKT))

            $(".nvcheckbd").val(d.TGNVCheckBD)
            $("#datetime_nvcheckbd").text(formatDateTime(d.TGNVCheckBD))
            $(".nvcheckkt").val(d.TGNVCheckKT)
            $("#datetime_nvcheckkt").text(formatDateTime(d.TGNVCheckKT))


            $(".donghang").val(d.NgayGioDongHang)
            $("#datetime_giodong").text(formatDateTime(d.NgayGioDongHang))
            $(".finishDongHang").val(d.NgayGioKTDongHang)
            $("#datetime_ktgiodong").text(formatDateTime(d.NgayGioKTDongHang))
            $(".maSeal").val(d.MaSeal)
            var defaultValues = d.MaSeal.split(',');
            $(".maSealSelect ").val(defaultValues).trigger('change');
            $(".xeDI").val(d.NgayGioXeDi)
            $("#datetime_xedi").text(formatDateTime(d.NgayGioXeDi))
            /* valPKL = d.Ma_PKL*/
            renderCheckKT()
            if (module == 2) {
                renderTableTenHang()
            }

            renderImage()
            rendeImage()
            renderImageOther()
        }

    })
}
function AddCheckClass() {
    for (let i = 1; i <= 19; i++) {
        $(`.isContT${i}`).prop("checked", true);
    }
    $(`.isKetLuanT`).prop("checked", true);
}
function boCheckClass() {
    for (let i = 1; i <= 19; i++) {
        $(`.isContKT${i}`).prop("checked", false);
        $(`.isXeTaiKT${i}`).prop("checked", false);
        $(`.isContT${i}`).prop("checked", false);
        $(`.isXeTaiT${i}`).prop("checked", false);
    }
}
function checkClassXeTai() {
    for (let i = 1; i <= 19; i++) {
        $(`.isContKT${i}`).prop("checked", false);
        $(`.isXeTaiKT${i}`).prop("checked", false);
        $(`.isContT${i}`).prop("checked", false);
        $(`.isRomocT${i}`).prop("checked", false);
        $(`.isXeTaiT${i}`).prop("checked", true);

        $(`.isXeTaiKT${i}`).prop("checked", false);
        $(`.isXeTaiT${i}`).prop("checked", true);


    }
}
function renderCheckKT() {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetNoiDungXuatHang&para1=" + maPhieu + "&para2=A",
        type: 'Get',
        success: function (data) {
            let index = 1;
            if (data[0].StatusKL != 0) {
                data[0].StatusKL == 1 ? $(`.isKetLuanT`).prop("checked", true) : $(`.isKetLuanKT`).prop("checked", true);
                ketLuan = data[0].StatusKL;
            }
            for (var i = 0; i < data.length; i++) {
                let d = data[i];
                if (d.StatusCont != 0) {
                    d.StatusCont == 1 ? $(`.isContT${index}`).prop("checked", true) : $(`.isContKT${index}`).prop("checked", true)
                    isContT[index] = d.StatusCont;
                }
                if (d.StatusXeTai != 0) {
                    d.StatusXeTai == 1 ? $(`.isXeTaiT${index}`).prop("checked", true) : $(`.isXeTaiKT${index}`).prop("checked", true)
                    isXeTaiT[index] = d.StatusXeTai
                }
                index++;
            }
        }
    })
}

$(".isKetLuanT").change(function () {
    if (module == 1) {
        if ($(this).prop("checked")) {
            renderTableMaHang();
            $(".camera_car").show()
            //$(".ma_PKL").show();
        } else {
            //$(".ma_PKL").hide();
            $("#bodyMahang").empty();
            $(".camera_car").hide();
            $(".camera_car").text("Chụp hình xe Cont")
            $(".image_Cont").hide();
        }
    }
});
$(".isKetLuanKT").change(function () {
    /*$(".ma_PKL").hide();*/
    $("#bodyMahang").empty();
    $(".camera_car").hide();
    $(".camera_car").text("Chụp hình xe Cont")
    $(".image_Cont").hide();
})

function renderCongTy() {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetCty&para1=a&para2=a",
        type: 'Get',
        success: function (data) {
            $(".congty").empty()
            $.each(data, function (index, item) {
                $(".congty").append(`
                     <option data-congty="${item.MaCongTy}" value="${item.TenCongTy}">${item.TenCongTy}</option>
                `)
            })
        }
    })
}
function checkFirstOption() {
    var firstOptionValue = $('.select_MaPKL option:selected').data("isexport");

    if (firstOptionValue == "0") {
        var validOptions = $('.select_MaPKL option').filter(function () {
            return $(this).val() !== "0";
        });
        if (validOptions.length > 0) {
            $('.select_MaPKL').val(validOptions.first().val());
        }
    }
}

let valCont = ""; let valPKL = ""; let valMaSeal = ""; let valDispaly = ""; var filterData = ""; var valMaCont = "";
let valNgayNhapKho = "";
function AutoWidth() {
    $('.lookup-input2').each(function () {
        $(this).width($(this).val().length * 7);
        $(".lookup-input1").width($(this).val().length * 7);
    });
}
function renderMaPKL() {
    $(".select_MaPKL").empty()
    $.ajax({
        async: false,
        url: "/api/XuatHang/Get?Action=GetKH&para1=A&para2=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                toastr.error("Chưa có mã PKL xuất hàng")
                hideDisable()
                return;
            }
            filterData = data.filter(item => item.IsExport == 0 && item.IsHT == 1)
            //valMaCont = data[0].MaCont
            //valCont = data[0].Cont
            //valPKL = data[0].MaPKL_XH
            //valMaSeal = data[0].MaSeal
            //valDispaly = data[0].Display
            //valNgayNhapKho = data[0].NgayNhapKho
            $(".lookup-results1").empty();
            $.each(data, function (index, item) {
                let htmlInputCheck = ""
                if (item.IsExport == "0" && item.IsHT != 0) {
                    htmlInputCheck = `
                        <input class="checkLI" style="margin: 0 20px;" type="checkbox">
                `
                }

                $(".lookup-results1").append(`
                    <li class="result-row" data-sobooking="${item.SoBooking}" data-ngaynhapkho="${item.NgayNhapKho}" data-isexport="${item.IsExport}" data-seal="${item.MaSeal}" data-macont="${item.MaCont}" data-cont="${item.Cont}"
                        data-display= "${item.Display}" data-value="${item.MaPKL_XH}"  data-isht="${item.IsHT}" data-isdongthung="${item.IsDongThung}" data-isnhapkho="${item.IsNhapKho}" data-isxacnhan="${item.IsXacNhan}">
                        ${htmlInputCheck}  
                        <div class="result-column">${item.Display} </div>
                      
                    </li>
                 `)
            })
            /*renderTableMaHang();*/
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

        }
    })
}
function hideDisable() {
    $(".banSo, .container, .romoc, .nameTX, .CMND,  .xeDen, .checkXe, .donghang, .finishDongHang, .xeDI").prop("readonly", true);
}
function showDisable() {
    $(".banSo ,.container, .romoc, .nameTX, .CMND, .congty, .xeDen, .checkXe, .donghang, .finishDongHang, .xeDI").prop("readonly", false);
}
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
$("#clearButtonDetail").on("click", function () {
    $(".popup2").hide()
})

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

});
function GetCheckMakHo(mapkl, macont, $this) {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetCheckMaKho&para1=${mapkl}&para2=${macont}&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            if (data.length == 1) {
                eachCheckBoxLi()
                if ($(".container").val() == "")
                    $(".container").val('Cont')
            }
            else {
                toastr.error("Hiện tại PKL này đang có 2 mã kho! Vui lòng luân chuyển về chung kho để được xuất hàng ")
                $this.prop("checked", false)
            }
        }
    })
}
$(".lookup-results1").on("click", ".checkLI", function () {
    if ($(this).prop("checked")) {
        let macont = $(this).closest("li").data("macont")
        let mapkl = $(this).closest("li").data("value")
        GetCheckMakHo(mapkl, macont, $(this))
    } else {
        eachCheckBoxLi()
        if ($(".container").val() == "")
            $(".container").val('Cont')
    }

});
let $liMaPKL = ""
let $liCont = ""
let arrMaPKL_Cont = []
function eachCheckBoxLi() {
    $liMaPKL = "";
    $liCont = ""
    arrMaPKL_Cont = []
    let html = ``
    $(".lookup-results1 li").each(function () {
        if ($(this).find('input[type="checkbox"]:checked').length > 0) {
            let textDiv = $(this).find(".result-column").text()
            $liMaPKL += `${$(this).data("macont")};`;
            $liCont += `${$(this).data("value")};`;
            let object = {
                MaCont: $(this).data("macont"),
                MaPKL: $(this).data("value"),
                SoBooking: $(this).data("sobooking"),
                Cont: $(this).data("cont"),
                NgayNhapKho: $(this).data("ngaynhapkho")
            }
            arrMaPKL_Cont.push(object)
            html += `
                <div>${textDiv}</div>
            `
        }
    })
    $(".item_input2").html(html)
    renderTableMaHang($liMaPKL, $liCont)
}
$(".hideLi").on("click", function () {
    $(".lookup-results1").hide();
    $(".lookup-input1").hide();
    $(".hideLi").hide()
})
let mahangImage = "";
function renderTableMaHang() {
    if (filterData.length == 0) {
        toastr.error("Các phiếu đã được kiểm hàng.")
        hideDisable()
        $(".lookup-input2").val("")
        $("#bodyMahang").empty();
        return
    } else {
        showDisable()
    }
    maPKLOnly = ""
    maContOnly = ""
    //$(".lookup-input2").val(valDispaly)
    //$(".container").val(valCont)
    //$(".maSeal").val(valMaSeal)

    //AutoWidth()
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetTableTenHang&para1=" + $liMaPKL + "&para2=" + $liCont,
        type: 'Get',
        success: function (data) {
            $("#bodyMahang").empty();
            let totalSoLuong = data.reduce((acc, curr) => acc + parseInt(curr.SoLuong), 0);
            let totalSoKien = data.reduce((acc, curr) => acc + parseInt(curr.SLThung), 0);

            let htmlFoter = `
                          <tr style="height:25px">
                             <td colspan="2"></td>
                            <td colspan="">Tổng cộng</td>
                            <td class="TongSL" colspan="">${totalSoLuong}</td>
                             <td class="TongSK" colspan="">${totalSoKien}</td>
                            <td colspan="2"></td>
                          
                        </tr>
            `
            const maHValues = data.map(item => item.MaH);

            mahangImage = maHValues.join(' + ');
            $.each(data, function (index, item) {
                $("#bodyMahang").append(`
                <tr data-poid="${item.POID}" data-mahang=${item.MaH} data-mapkl= "${item.Ma_PKL}" data-macont="${item.MaSoCont}" style="height:30px">
                            <td>${item.TenHang}</td>
                            <td>${item.MaHang}</td>
                            <td>${item.PO}</td>
                            <td>${item.SoLuong}</td>
                            <td>${item.SLThung}</td>
                             <td class='SoBooking' style="outline: none; text-align: center;" contenteditable='true'>${item.SoBooking}</td>
                            <td class='SoBooking' style="outline: none; text-align: center;" contenteditable='true' >${item.GhiChu}</td>
                        </tr>

                 `)
            })
            $("#bodyMahang").append(htmlFoter)
            totalSoKienTable = totalSoKien
            totalSoLuongTable = totalSoLuong
            $(".theadSL").html(`Số lượng <p>(${totalSoLuongTable})</p>`)
            $(".theadSoKien").html(`Số kiện <p>(${totalSoKienTable})</p>`)
        }
    })
}
let totalSoKienTable = ""
let totalSoLuongTable = ""
let maPKLOnly = ""
let maContOnly = ""
function renderTableTenHang() {
    $("#bodyMahang").empty();
    maPKLOnly = ""
    maContOnly = ""
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetTenHang&para1=" + maPhieu + "&para2=" + valMaContModule2V2,
        type: 'Get',
        success: function (data) {
            let totalSoLuong = data.reduce((acc, curr) => acc + parseInt(curr.SoLuong), 0);
            let totalSoKien = data.reduce((acc, curr) => acc + parseInt(curr.SLThung), 0);

            let htmlFoter = `
                          <tr style="height:25px">
                             <td colspan="2"></td>
                            <td colspan="">Tổng cộng</td>
                            <td class="TongSL" colspan="">${totalSoLuong}</td>
                             <td class="TongSK" colspan="">${totalSoKien}</td>
                            <td colspan="2"></td>
                          
                        </tr>
            `
            const maHValues = data.map(item => item.MaH);

            mahangImage = maHValues.join(' + ');
            $.each(data, function (index, item) {
                if ($(".container").val() == "") return;
                if (index == 0) {
                    maPKLOnly = data[0].Ma_PKL
                    maContOnly = data[0].MaSoCont
                }
                $("#bodyMahang").append(`
                <tr data-poid="${item.POID}" data-mahang=${item.MaH} data-mapkl= "${item.Ma_PKL}" data-macont="${item.MaSoCont}" style="height:30px">
                            <td>${item.TenHang}</td>
                            <td>${item.MaHang}</td>
                            <td>${item.PO}</td>
                            <td>${item.SoLuong}</td>
                            <td>${item.SLThung}</td>
                            <td class='SoBooking' style="outline: none; text-align: center;" contenteditable='true'>${item.SoBooking}</td>
                            <td class='SoBooking' style="outline: none; text-align: center;" contenteditable='true' >${item.GhiChu}</td>
                        </tr>

              `)
            })

            $("#bodyMahang").append(htmlFoter)
            getDsXe()
            totalSoKienTable = totalSoKien
            totalSoLuongTable = totalSoLuong
            $(".theadSL").html(`Số lượng <p>(${totalSoLuongTable})</p>`)
            $(".theadSoKien").html(`Số kiện <p>(${totalSoKienTable})</p>`)
        }
    })

}
$(".camera_car").on("click", function () {
    if ($(this).text() == "Chụp hình xe Cont") {
        $(this).text("Ẩn hình Cont");
        $(".image_Cont").show();
    } else {
        $(this).text("Chụp hình xe Cont");
        $(".image_Cont").hide();
    }
});
$(".camera_car_back").on("click", function () {
    if ($(this).text() == "Chụp hình lại") {
        $(this).text("Ẩn hình Cont");
        $(".image_Cont").show();

    } else {
        $(this).text("Chụp hình lại");
        $(".image_Cont").hide();
    }
});
let deleteRow = 0;
function getDSPhieuAdd() {
    let maphieubb = Number($("#soPhieu").text());
    if (module == 2) maphieubb = maPhieu
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetPhieu&para1=${maPhieu}&para2=a&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            saveListSeal(maphieubb, 0, data)
            deletePhieu()
        }
    })
}
function DeleteSealAndDsXe() {
    getDSPhieuAdd()
}
function deletePhieu() {
    let maphieubb = Number($("#soPhieu").text());
    if (module == 2) maphieubb = maPhieu

    let obejct = {
        MaPhieu: maphieubb,
        Delete: deleteRow,
        SoCont: $(".container").val()
    }
    $.ajax({
        type: "POST",
        url: '/api/XuatHang/DeletePhieu',
        data: JSON.stringify(obejct),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function () {


        },

    });
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
$("#delete").on("click", function () {
    let xoaphieu = confirm("Bạn muốn xóa phiếu này")
    var selectedValues = $('.maSealSelect').val();
    var selectedString = selectedValues.join(',');
    deleteRow = 1;
    if (xoaphieu) {
        DeleteSealAndDsXe()

        saveNoiBo(selectedString, 0)
        deleteDSXe()
        setTimeout(function () {
            renderMaPhieu()
            renderTableTenHang()
        }, 200)

    }
    $("#bodyKyTen").empty()
    AddKyTen()

})
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
function addImageInDiv(srcString) {
    $(`#${id_img}`).empty()
    let imageElementAdd = $(`<img data-valueimage="${srcString}" data-name="10" class="${id_img}">`).attr("src", srcString);
    $(`#${id_img}`).append(imageElementAdd)
}
let moduleImage = 1;
$(".list_image").on("click", ".image", function () {
    if (window.CefSharp) {
        $(".swal2-confirm").hide()
    }
    moduleImage = 1
    let $this = $(this)
    id_img = $this.attr("id")
    if (id_img == 'image0') index = 1
    else if (id_img == 'image25') index = 2
    else if (id_img == 'image50') index = 3
    else if (id_img == 'image75') index = 4
    else if (id_img == 'image100') index = 5
    else if (id_img == 'imageDoAm') index = 6
    else if (id_img == 'imageDongCua') index = 7
    else if (id_img == 'imageTruocChot') index = 8
    else if (id_img == 'imageChotAT') index = 9
    showConfirmation(id_img)
})
let moduleAnh = "";
function showConfirmation(id_img) {
    Swal.fire({
        title: 'Thực hiện hành vi?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Chụp hình',
        cancelButtonText: 'Chọn ảnh',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button id="zoomImage" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;">Zoom Image</button>'
    }).then((result) => {
        if (result.isConfirmed) {
            StartCamera();
            $('#ModalImage').modal('show');
            moduleAnh = 1
        }

    });
    $(".swal2-cancel").on("click", function () {
        moduleAnh = 2
        $("#fileInput").trigger('click');

    })
    $("#zoomImage").on("click", function () {
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


selectMaPhieu()
$("#xuatEX").on("click", function () {
    Export()

})
function Export() {
    var url = "/api/XuatHang/BCExcelBB?Action=GetBCEXXuathang&para1=" + maPhieu + "&para2=A" + "&para3=" + valMaContModule2V2;
    var link = document.createElement('a');
    var filename = `BCLoiQA.xlsx`;
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function rendeImage() {
    let maphieubb = Number($("#soPhieu").text());
    if (module == 2) maphieubb = maPhieu
    hideImage()
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetHoSoHX&para1=" + maphieubb + "&para2=A",
        type: 'Get',
        success: function (data) {
            $.each(data, function (index, item) {
                if (item.Image0 != "" && item.Image0 != null) $("#image0").html(`<img class="image0" src="/Images/All/${item.Image0}">`)
                if (item.Image25 != "" && item.Image25 != null) $("#image25").html(`<img class="image25" src="/Images/All/${item.Image25}">`)
                if (item.Image50 != "" && item.Image50 != null) $("#image50").html(`<img class="image50" src="/Images/All/${item.Image50}">`)
                if (item.Image75 != "" && item.Image75 != null) $("#image75").html(`<img class="image75" src="/Images/All/${item.Image75}">`)
                if (item.Image100 != "" && item.Image100 != null) $("#image100").html(`<img class="image100" src="/Images/All/${item.Image100}">`)
                if (item.ImageDongCua != "" && item.ImageDongCua != null) $("#imageDongCua").html(`<img class="imageDongCua" src="/Images/All/${item.ImageDongCua}">`)
                if (item.ImageTruocChot != "" && item.ImageTruocChot != null) $("#imageTruocChot").html(`<img class="imageTruocChot" src="/Images/All/${item.ImageTruocChot}">`)
                if (item.ImageChotAT != "" && item.ImageChotAT != null) $("#imageChotAT").html(`<img class="imageChotAT" src="/Images/All/${item.ImageChotAT}">`)
                if (item.ImageDoAm != "" && item.ImageDoAm != null) $("#imageDoAm").html(`<img class="doam" src="/Images/All/${item.ImageDoAm}">`)
            })
        }
    })
}
function hideImage() {
    $("#image0").html("")
    $("#image25").html("")
    $("#image50").html("")
    $("#image75").html("")
    $("#image100").html("")
    $("#imageDongCua").html("")
    $("#imageTruocChot").html("")
    $("#imageChotAT").html("")
    $("#imageDoAm").html("")
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
function setTenBienBanFoder() {
    let month = today.split("-")[1]
    let day = today.split("-")[2]
    let bienbanMonth = tenBienBan == "" || tenBienBan == null ? "" : tenBienBan.split("|")[2]
    if (tenBienBan == "" || day == "01" || tenBienBan == null || bienbanMonth != month) {
        bienBanFoder = `XTP|${addWithPadding(1, 0)}|${month}`
    }
    else {
        let bienban = tenBienBan.split("|")[1]
        bienBanFoder = `XTP|${addWithPadding(Number(bienban), module == 2 ? 0 : 1)}|${month}`
    }
}

function eachDivImage(maphieubb) {
    let arrImage9 = [];
    setTenBienBanFoder()
    let checkAnh = moduleAnh == 1 ? "anhchup" : "chonanh"
    let arrDivImage = ["image0", "image25", "image50", "image75", "image100", "imageDoAm", "imageDongCua", "imageTruocChot", "imageChotAT"]
    let index = 1
    arrDivImage.map(item => {
        if ($(`#${item}`).find('img[data-name="10"]').length > 0) {
            let imageNoiBo = $(`#${item}`).find("img").data("valueimage")
            let object = {
                MaBienBan: maphieubb,
                [item]: imageNoiBo,
                SoXe: index,
                NameImage: item,
                ListAnh: checkAnh,
                TenBienBan: bienBanFoder,
                TenMaHang: mahangImage,
            }
            arrImage9.push(object)
        }
        index++;
    })
    if (arrImage9.length > 0)
        saveImage(arrImage9)

}
function saveImage(arrImage9) {
    $.ajax({
        type: "POST",
        url: "/api/XuatHang/PostImage",
        data: JSON.stringify(arrImage9),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            rendeImage()
        }
    });
}
$("#DenNgay").val("dd/mm/yyyy");
$("#DenNgay").datepicker({
    dateFormat: "dd/mm/yy", // Định dạng ngày tháng
    timeFormat: 'HH:mm',
    dayNamesMin: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"], // Tên ngày viết tắt
    monthNamesShort: ["Thg1", "Thg2", "Thg3", "Thg4", "Thg5", "Thg6", "Thg7", "Thg8", "Thg9", "Thg10", "Thg11", "Thg12"], // Tên tháng viết tắt
    prevText: "Tháng trước",
    nextText: "Tháng sau",
    currentText: "Hôm nay",
    changeMonth: true,
    changeYear: true
});
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
function getImageFolder(imageData) {
    let checkAnh = moduleAnh == 1 ? "anhchup" : "chonanh"
    setTenBienBanFoder()
    let maphieubb = Number($("#soPhieu").text());

    if (module == 2) maphieubb = maPhieu
    let arrImage10 = {
        MaBienBan: maphieubb,
        ListAnh: checkAnh,
        TenBienBan: bienBanFoder,
        TenMaHang: mahangImage,
        ImageHinh: imageData
    }
    $.ajax({
        type: "POST",
        url: "/api/XuatHang/PostImageInFolder",
        data: JSON.stringify(arrImage10),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

        }
    });
}
let arrImage = []
function SavePreImage() {
    if (moduleImage == 1) {
        addImageInDiv(imageData)
        getImageFolder(imageData)
    } else {
        $(`#${linkImage}`).empty();
        let imageelement = $(`<img  data-name="${imageData}">`).attr("src", imageData);
        $(`#${linkImage}`).append(imageelement);
    }
}
$(document).on("click", function (event) {
    if (!$(event.target).closest('.maSeal, .inputName,.input_image,.SoBooking,.selectseal').length) {
        $(".list_kyten").removeClass("active");
    }
});

$(document).on("click", ".maSeal, .inputName,.input_image,.SoBooking", function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $(".list_kyten").addClass("active")
        $("html, body").animate({ scrollTop: $(document).height() }, 800);
    }
});
$(document).on("click", ".input_image,.SoBooking", function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $(".list_kyten").addClass("active")
        $("html, body").animate({ scrollTop: $(document).height() }, 800);
    }
});
$('.selectseal').on('select2:open', function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))
        $(".list_kyten").addClass("active")
    $("html, body").animate({ scrollTop: $(document).height() }, 800);
});

function CheckHideIconDelete() {
    if (user.toLowerCase().trim() == 'admin') {
        $(".iconDelete").show()
    } else {
        $(".iconDelete").css("display", "none")
    }
}
//add imageOther
let count = 9;
let countRow = 9;
let divLength = 1;
let ischeckLength = false
function renderImageOther() {
    $.ajax({
        url: '/api/TheoDoiDonHang/Get?Action=GetImageOther&para1=' + maPhieu + '&para2=a&para3=a',
        type: "Get",
        success: function (data) {
            $(".item_addImage").empty()
            countRow = 10
            if (data.length == 0) {
                count = 10;
                return;
            }
            $.each(data, function (index, item) {
                count = item.Position
                $(".item_addImage").append(`
                 <div data-position="${count}"  class="image_other ${count}">
                     <div style="display: flex;position:relative">
                    <label class="label_image">${countRow}. </label>
                    <input data-positon="${count}" data-change="2" id="input_image${count}" class="input_image" type="text" name="name" value="${item.NoiDung}" />
                    <i data-delete="2" data-name="${count}" class="fa-solid fa-xmark fa-lg iconDelete" style="color: #a00d14;
                       position: absolute;  right: 3px; font-size: 15px; cursor: pointer;  top: 12px;"></i>
                </div>
                  <div data-name="${count}" id="lst_AddImage${count}" class="lst_AddImage">
                    <img  src="/Images/Other/${item.Image}">
                </div>
             </div>

                `)
                countRow++;
            })
            countRow--;
            CheckHideIconDelete()
        }
    })
}

$(".camera_imageOther").on("click", function () {
    $(".camera_imageOther_back").show()
    $(".camera_imageOther_View").hide()
    if ($(".item_addImage").children().length == 0) {
        count = 10
        countRow = 10;
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
                    <i  data-name="${count}" class="fa-solid fa-xmark fa-lg iconDelete" style="color: #a00d14;
    position: absolute;  right: 3px; font-size: 15px; cursor: pointer;  top: 12px;"></i>
                </div>
                <div data-name="${count}" id="lst_AddImage${count}" class="lst_AddImage">
                 
                </div>
            </div>`
    $(".item_addImage").append(html)
    $(".image_other").find(`#input_image${count}`).focus()
    $(".list_kyten").addClass("active")
    $("html, body").animate({ scrollTop: $(document).height() }, 800);
    ischeckLength = true
    CheckHideIconDelete()
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
let arrInput = [];
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
$(".camera_imageOther_View").on("click", function () {
    $(".camera_imageOther_back").show()
    $(".camera_imageOther_View").hide()

    $(".item_addImage").show()
})

function foreachImageNoiBo(maphieu) {
    let maphieubb = maphieu;
    if (module == 2) maphieubb = maPhieu
    $(".image_other").each(function () {
        let $this = $(this)
        let imageNoiBo = $this.find("img").data("name")
        if (imageNoiBo == "" || imageNoiBo == null) return true;
        let inputValue = $this.find(".input_image").val();
        let position = $this.data("position")
        let object = {
            MaBienBan: maphieubb,
            NoiDung: inputValue,
            Image: imageNoiBo,
            Position: position,
        }
        arrImage.push(object)
    })
    if (arrImage.length == 0) return true;
    $.ajax({
        url: "/api/TheoDoiDonHang/PostImage",
        type: "Post",
        data: JSON.stringify(arrImage),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            arrImage = [];
            ischeckLength = false;
            divLength = 1;
            renderImageOther()
        }
    })
}

function saveImageOther(maphieu) {
    foreachImageNoiBo(maphieu)
}
$(".item_addImage").on("change", ".input_image", function () {

    let maphieubb = Number($("#soPhieu").text());
    if (module == 2) maphieubb = maPhieu
    let position = $(this).data("positon")
    let value = $(this).val()
    let object = {
        MaBienBan: maphieubb,
        NoiDung: value,
        Position: position,
    }
    arrInput.push(object)
    saveInput()

});
function saveInput() {
    $.ajax({
        url: "/api/TheoDoiDonHang/UpdateInputImageOther",
        type: "Post",
        data: JSON.stringify(arrInput),
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (data) {
            arrInput = [];
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
        MaBienBan: maPhieu,
        Position: countDelete
    }
    $.ajax({
        url: "/api/TheoDoiDonHang/DeleteImage",
        type: "Post",
        data: JSON.stringify(post),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

        }
    })
}
//Ky ten
function saveGhiChu() {
    let arrGhiChu = []
    $("#bodyMahang tr").not(':last').each(function () {
        let row = $(this).find('td');
        let poid = $(this).data("poid")
        let mahang = $(this).data('mahang')
        let macont = $(this).data('macont')
        let mapkl = $(this).data('mapkl')
        if (row.eq(5).text().trim() == "" && row.eq(6).text().trim()) return true;
        let object = {
            MaPKL_XH: mapkl,
            VCCont: macont,
            MaHang: mahang,
            PO: poid,
            SoBooking: row.eq(5).text(),
            GhiChu: row.eq(6).text()
        };
        arrGhiChu.push(object);
    });
    if (arrGhiChu.length != 0) {
        $.ajax({
            url: '/api/XuatHang/PostDL',
            type: "Post",
            data: JSON.stringify(arrGhiChu),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {

            }
        })
    }
}

function renderImage() {
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetKyTenXuatHang&para1=" + maPhieu + "&para2=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                countKiten = 0
                AddKyTen()
                return
            };
            rederKyTenXH(data)

        }
    })
}
function rederKyTenXH(data) {
    $("#bodyKyTen").empty()
    countKiten = 1
    $.each(data, function (index, item) {
        let htmlImage = item.KyTen == "" || item.KyTen == null ? "" : ` <img  data-name="" src="/Images/KytenXuatHang/${item.KyTen}">`

        $("#bodyKyTen").append(`
                 <tr data-position="${countKiten}" data-mahoten="${item.MaHoTen}" style="position: relative;">
                     <td style="width:30%"><input style="text-transform: capitalize;" data-column="HoTen" id="hoten" class="hoten${countKiten} inputName" type="text" name="name" value="${item.HoTen}" />
                        <i class="fa-sharp fa-solid fa-trash icondelete_kiten"></i>
                    </td>
                     <td style="width:25%"><input style="text-transform: capitalize;" data-column="BoPhan" id="bophan" class="bophan${countKiten} inputName" type="text" name="name" value="${item.BoPhan}" /></td>
                     <td class="image_kyten" style="width:45%">
                         <div id="imageKyTen" class="list-img${countKiten}" data-name="" style=" width: 100%; height: 109px; display: flex; justify-content: center; margin-bottom: 5px; ">
                          ${htmlImage}
                          </div>
                     </td>
                 </tr>
                `)
        countKiten++;
    })
}
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
    countKiten++;
}
let arrKiTen = [];
$("#bodyKyTen ").on("click", ".icondelete_kiten", function () {
    let $rowtr = $(this).closest("tr")
    $rowtr.remove();
    let mahoten = $rowtr.data("mahoten")
    if (mahoten == "") return;
    deleteKiTenRow(mahoten)
})
function deleteKiTenRow(mahoten) {
    $.ajax({
        url: "/api/XuatHang/Get?Action=DeleteKyTenRow&Para1=" + mahoten + "&Para2=A&Para3=a",
        type: 'Get',
        success: function (data) {

        }
    })
}
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
function foreachTable(maphieu) {
    let maphieubb = maphieu
    if (module == 2) maphieubb = maPhieu
    $("#bodyKyTen tr").each(function () {
        var mahoten = $(this).data("mahoten")
        var hoten = $(this).find("#hoten").val(); // Lấy giá trị của input có class là hoten trong hàng hiện tại
        var bophan = $(this).find("#bophan").val(); // Lấy giá trị của input có class là bophan trong hàng hiện tại
        var linkHinh = $(this).find(".image_kyten .linkhinh").attr("data-name");
        if (hoten == "") return true;
        let object = {
            MaBBXuatHang: maphieubb,
            MaHoTen: mahoten,
            HoTen: capitalizeFirstLetter(hoten),
            BoPhan: capitalizeFirstLetter(bophan),
            KyTen: linkHinh,
        }
        arrKiTen.push(object)
    });
    if (arrKiTen.length == 0) return true;
    $.ajax({
        type: "POST",
        url: "/api/TheoDoiDonHang/PostImageKyTen",
        data: JSON.stringify(arrKiTen),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            arrKiTen = [];
            $("#bodyKyTen").empty()
            AddKyTen();
            if (module == 2) {
                renderImage()
            }
        }
    })
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
function saveKyTen(maphieu) {
    foreachTable(maphieu)

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
        Action: "UpdateInputKyTen",
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
$(document).ready(function () {
    if (window.CefSharp) {
        $("#home").css("display", "none");
        maPhieu = data.phieuBB
        moduleWin = 5
        if (maPhieu == "") return
        module = 2
        $("#creatPhieu").css("display", "none");
        $("#viewPhieu").text("Xem phiếu");
        renderMaPhieu()
        $(".list_item ma_PKL ").css("display", "none");
        $(".btn_search").css("display", "");
        $(".search").css("display", "none");
        $(".maPhieu").css("display", "none");
        $(".ma_PKL ").css("display", "none");
    }
});
$("#xuatHangDetail").on("click", async function () {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetMaPKLPhieu&para1=${maPhieu}&para2=A&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            if (module == 2) {
                arrMaPKL_Cont = []
                arrMaPKL_Cont = data
                console.log(arrMaPKL_Cont)
            }
            let html = ``
            arrMaPKL_Cont.map(item => {
                html += `
             <option data-mapkl="${item.MaPKL}" value="${item.MaCont}">${item.MaPKL} -- Cont: ${item.Cont} -- Số booking: ${item.SoBooking}</option>
            `

            })
            $(".mapkl_select2").html(html)
            $(".mapkl_select2").select2()
            ChiTietXuatHang()
        }
    })
})
$(".mapkl_select2").on("change", function () {
    ChiTietXuatHang()
})

function standardizeName(name) {
    return name.trim().toLowerCase().replace(/\b\w/g, function (char) {
        return char.toUpperCase();
    });
}
$(document).on("click", "body", function (event) {
    if (!$(event.target).closest(".lookup-results1 li, .lookup-input2, .lookup-input1").length && $(".lookup-results1").is(":visible")) {
        $(".lookup-results1").hide();
        $(".lookup-input1").hide();
        $(".hideLi").hide()
    }
});

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
        url: "/api/XuatHang/Get?Action=GetName&Para1=A&Para2=A&Para3=a",
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
        url: "/api/XuatHang/Get?Action=GetAddKyTen&Para1=A&Para2=A&Para3=a",
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
                  <td>${item.TenTaiXe}</td>
                 <td style="white-space: nowrap;">${item.CMND}</td>
                </tr>
        `
    })
    $("#tbodyList").html(html)
    $("#tbodyList1").html(html)
}
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
// chi tiết xuất hàng
function ChiTietXuatHang() {
    let mapkl = $(".mapkl_select2 option:selected").data("mapkl")
    let macont = $(".mapkl_select2").val();
    $.ajax({
        url: "/api/ThongKeDongThung/GetCTXH?&para1=" + mapkl + "&para2=A" + "&Para3=a",
        type: 'Get',
        success: function (data) {

            if (data.length == 0) return;
            data = data.filter(item => item.Cont === macont)
            let keys = Object.keys(data[0]);
            let filteredKeys = keys.filter(key => key.includes('@SIZE_'));

            let uniqueValues = data.map(x => ({
                MaPKL: x.MaPKLDisplay,
                SttThung: x.SttThung,
                SLThung: x.SLThung,
                SoLuong: x.SoLuong,
                TotalPiece: x.TotalPiece,
                TrongLuong: x.TrongLuong,
                KhoiLuong: x.KhoiLuong,

            }))
            uniqueValues = uniqueValues.filter((value, index, self) =>
                index === self.findIndex(obj =>
                    obj.MaPKL === value.MaPKL &&
                    obj.SttThung === value.SttThung
                )
            );
            let totalSLThung = uniqueValues.reduce((acc, curr) => acc + parseInt(curr.SLThung), 0);
            let totalTotalPiece = uniqueValues.reduce((acc, curr) => acc + parseInt(curr.TotalPiece), 0);
            let totalSoLuong = uniqueValues.reduce((acc, curr) => acc + parseInt(curr.SoLuong), 0);
            var totalTrongLuong = uniqueValues.reduce((acc, curr) => acc + parseFloat(curr.TrongLuong), 0);
            var roundedTotalTrongLuong = Math.round(totalTrongLuong * 10) / 10;
            var totalKhoiLuong = uniqueValues.reduce((acc, curr) => acc + parseFloat(curr.KhoiLuong), 0);
            var roundedTotalKhoiLuong = Math.round(totalKhoiLuong * 10) / 10;
            let $trSize = "";
            $.each(filteredKeys, (index, item) => {
                item = item.split("@")[0]
                $trSize += `
                            <td> ${item} </td>
                            `
            })
            let htmlHeader = `
                                <tr  style="position:relative">
                                    <td style="position: sticky; top: 0;display:none" rowspan="2" >SttThung</td>
                                    <td style="position: sticky; top: 0;"rowspan="2" colspan="2">Carton Number</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">PO</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Đơn vị sản xuất</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Đầu Size</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Color</td>
                                    <td style="position: sticky; top: 0;"colspan="${filteredKeys.length}">Size</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Qty </td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Carton </td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Total Piece</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">NW</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">GW</td>
                                    <td style="position: sticky; top: 0;" rowspan="2">Carton</td>
                                </tr>
                                 <tr >
                                   ${$trSize}
                                </tr>  `;

            $("#theadTotal").html(htmlHeader)
            let htmltbodyTotal = ``;
            $.each(data, (index, item) => {
                let htmlTrBody = ``;
                $.each(filteredKeys, (index, lstItem) => {
                    let value = item[lstItem];
                    value === 0 ? htmlTrBody += `<td></td>` : htmlTrBody += `<td>${value}</td>`
                })
                htmltbodyTotal += `
                             <tr>
                                <td class="rowMerge" style="display:none">${item.SttThung}</td>
                                <td class="rowMerge"> ${item.TuThung}</td>
                                <td class="rowMerge">${item.DenThung}</td>
                                <td style="white-space: nowrap;">${item.PO}</td>
                                <td  style="white-space: nowrap;">${item.TenDVSX}</td>
                                <td style="white-space: nowrap;">${item.DauSize}</td>
                                <td style="white-space: nowrap;">${item.TenMau}</td>
                                ${htmlTrBody}
                                <td class="rowMerge" >${item.SoLuong}</td>
                                <td  class="rowMerge">${item.SLThung}</td>
                                <td  class="rowMerge">${item.TotalPiece}</td>
                                <td class="rowMerge">${item.KhoiLuong.toFixed(1)}</td>
                                <td class="rowMerge">${item.TrongLuong.toFixed(1)}</td>
                                <td>${item.KyHieu}</td>
                        </tr>
                            `
            })
            let htmltfootr = `
                       <tr>
                        <td colspan="${filteredKeys.length + 6}"></td>
                        <td>${totalSoLuong}</td>
                        <td>${totalSLThung}</td>
                        <td>${totalTotalPiece}</td>
                        <td>${roundedTotalKhoiLuong}</td>
                        <td>${roundedTotalTrongLuong}</td>
                        <td></td>
                    </tr>`

            $("#tbodyTotal").html(htmltbodyTotal)
            $("#tfootTotal").html(htmltfootr)
            MergeRow()
        }
    })
}
//Add seal

function saveListSeal(maPhieu, status, arrDataPost) {
    let arrMaSeal = [];
    arrDataPost.map(item2 => {
        $(".maSealSelect option:selected").each(function () {
            let item = $(this).val();
            let text = $(this).text();
            let objcet = {
                MaPhieu: maPhieu,
                MaCont: item2.MaCont,
                MaSeal: item,
                TenSeal: text,
                MaPKL: item2.MaPKL,
                SoXe: $(".banSo").val().toUpperCase(),
                Status: status

            };
            arrMaSeal.push(objcet);
        });
    })

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
    gopArr = combinedArray.join(',');
    $(`.maSealSelect option[value='${valueClear}']`).data("sudung", 0);
    /*$(".maSealSelect ").val(defaultValues).trigger('change');*/
    showConfirmationSeal(valueClear, defaultValues)
});

function showConfirmationSeal(valueClear, defaultValues) {
    Swal.fire({
        title: 'Lựa chọn thao tác?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Hủy',
        cancelButtonText: 'Đổi seal',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button  id="clear2" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;display:none">Hủy</button>'

    }).then((result) => {
        if (result.isConfirmed) {
            var arr = gopArr.split(',').map(item => item.trim());
            $(".maSealSelect").val(arr).trigger('change');
            Swal.close();
        }
    });
    $(".swal2-cancel").on("click", function () {
        let arr = defaultValues.split(',').map(item => item.trim());
        $(".maSealSelect ").val(arr).trigger('change');
    })
    $("#clear2").on("click", function () {
        var arr = gopArr.split(',').map(item => item.trim());
        $(".maSealSelect").val(arr).trigger('change');
        Swal.close();
    })
}
//  seal lỗi
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
        title: 'Lựa chọn thao tác?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Hủy',
        cancelButtonText: 'Đổi seal',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
        html: '<button id="clear1" class="swal2-confirm swal2-styled" style="background-color: #f6c23e;display:none">Hủy</button>'

    }).then((result) => {
        if (result.isConfirmed) {
            var arr = goparrTable.split(',').map(item => item.trim());
            $this.val(arr).trigger('change');
            Swal.close();
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
    if (xeVC != 2) {
        toastr.error("Vui lòng chọn xe tải")
        return;
    }
    //$("#tbodyLists").empty()
    $(".input_popup").val("")
    $(".popup3").show()

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
    $(".nameTX").val("")
    $(".CMND").val("")
    $(".banSo").val("")
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
                <input style="text-transform: capitalize;" class="tenTaiXeListInput" type="text" name="name" value="" autocomplete="off"/>
                </td>
                <td  class="CMNDList">
                  <input  class="CMNDListInput" type="number" maxlength="12" name="name" value="" autocomplete="off"/>
                </td>
                <td>
                    <input style="text-transform: uppercase;" style="text-transform: uppercase;"  class="soxeListInput" type="text" name="name" value="" autocomplete="off"/>
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

    $("#tbodyLists").append(html)
    if ($("#tfootSL").text() == 0 && $("#tfootSoKien").text() == 0) {
        let htmlTfoat = `

                <tr id="thead" style="position:relative">
                                        <td colspan="4"></td>
                                        <td id="tfootSoKien">0</td>
                                        <td id="tfootSL">0</td>
                                       
                                    </tr>`
        $("#tfootLists").html(htmlTfoat)
    }

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

//$(".aaa").on("click", function () {
//    saveListTaiXe()
//})
function saveListTaiXe(arrDataPost) {
    /* getDSPhieu()*/
    let arrListTaiXe = [];
    arrDataPost.map(item => {
        if ($("#tbodyLists tr").length > 0) {
            $("#tbodyLists tr").map(function () {
                if ($(this).find(".tenTaiXeListInput").val() != "") {
                    let $this = $(this)
                    let valueSelected = $this.find(".maSealSelectList").val().join(",")
                    let textSelected = $this.find(".maSealSelectList option:selected").map(function () {
                        return $(this).text()
                    }).get().join(",")
                    let object = {
                        MaCont: item.MaCont,
                        MaSeal: valueSelected,
                        TenSeal: textSelected,
                        MaPKL: item.MaPKL,
                        TenTaiXe: capitalizeFirstLetter($this.find(".tenTaiXeListInput").val()),
                        CMND: $this.find(".CMNDListInput").val(),
                        SoXe: $this.find(".soxeListInput").val().toUpperCase(),
                        SLSP: $this.find(".SLThungListInput").val(),
                        SLThung: $this.find(".SLSPistInput").val(),
                        Module: 1

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
            let object = {
                MaCont: item.MaCont,
                MaSeal: selectedString,
                TenSeal: selectedText,
                MaPKL: item.MaPKL,
                TenTaiXe: capitalizeFirstLetter($(".nameTX").val()),
                CMND: $(".CMND").val(),
                SoXe: $(".banSo").val().toUpperCase(),
                SLSP: $("#bodyMahang").find(".TongSL").text(),
                SLThung: $("#bodyMahang").find(".TongSK").text(),
                Module: 1

            };
            arrListTaiXe.push(object);
        }
    })

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
            $("#tbodyLists").empty()
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
        url: `/api/BienBanLuuSeal/Get?Action=GetDSXe&para1=${maContOnly}&para2=${maPKLOnly}&para3=a&para4=a&para5=a`,
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
            let htmlTfoat = `
                <tr id="thead" style="position:relative">
                                        <td colspan="4"></td>
                                         <td id="tfootSoKien">${totaltfootSLThung}</td>
                                        <td id="tfootSL">${totaltfootSLSP}</td>
                                     
                                    </tr>`
            $("#tfootLists").html(htmlTfoat)
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
function deleteDSXe() {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=DeleteDSXe&para1=${valMaContModule2V2}&para2=${valPKL}&para3=a&para4=a&para5=a`,
        type: "GET",
        success: function (data) {

        }
    });
}
let previousValue = null;
$(".maSealSelect").on("select2:select", function (e) {
    if (module == 2) {
        let selectedValue = e.params.data.id;
        let valseal = $(this).find("option[value='" + selectedValue + "']").data("sudung");

        if (valseal == 1 && previousValue != null) {
            toastr.error("Seal này đã được sử dụng");
            $(this).val(previousValue).trigger('change');
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
$(".listCanDen").on("click", function () {
    $(".popup5 ").show()
})
getCangDen()
function getCangDen() {
    $.ajax({
        url: `/api/BienBanLuuSeal/Get?Action=GetCangDen&para1=${maPhieu}&para2=a&para3=a&para4=a&para5=a`,
        type: "Get",
        success: function (data) {
            let htmlThead2 = `
                <tr>
                      <td style="position: sticky; top: 0;" >Cảng đến</td>
                </tr>
               `
            $("#theadList2").html(htmlThead2)
            let html = ``
            data.map(item => {
                html += `
             <tr data-value="${item.CangDen}">
                  <td style="text-transform: uppercase;">${item.CangDen}</td>
                </tr>
        `
            })
            $("#tbodyList2").html(html)
        }
    })
}

$(".hide_popup2").on("click", function () {
    $(".popup5 ").hide()
})
$(".search_table2").on("keyup", function (e) {
    clearTimeout(timeoutId);
    timeoutId = setTimeout(() => {
        let searchValue = removeDiacritics($(".search_table2").val()).toUpperCase()
        $("#tbodyList2 tr").each(function () {
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
$("#tbodyList2").on("click", "tr", function () {
    let value = $(this).data("value")
    $(".cangden").val(value)
    $(".popup5 ").hide()
})
function MergeRow() {
    let tableLength = $("#tbodyTotal tr").length;

    for (let i = 0; i < tableLength; i++) {
        let merge = 1;
        let valueMerge = $("#tbodyTotal tr").eq(i).find(".rowMerge").text();
        for (let j = i + 1; j < tableLength; j++) {
            let valueMergeNext = $("#tbodyTotal tr").eq(j).find(".rowMerge").text();
            if (valueMerge === valueMergeNext) {
                merge++;
            } else {
                break;
            }
        }
        $("#tbodyTotal tr").eq(i).find(".rowMerge").attr("rowspan", merge);
        for (let k = 1; k < merge; k++) {
            $("#tbodyTotal tr").eq(i + k).find(".rowMerge").hide();
        }
        i += merge - 1;
    }
}






