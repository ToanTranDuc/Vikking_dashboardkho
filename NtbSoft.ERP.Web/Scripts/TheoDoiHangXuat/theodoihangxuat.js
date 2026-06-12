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

$("#home").on("click", function () {
    window.location.href = '/Home/Dashboard'
})
let user = localStorage.getItem('username1');
let isCheckDN = localStorage.getItem('isCheckDN');
if (isCheckDN == 'false') {
    window.location.href = '/Login/Index'
}
let remember = localStorage.getItem('remember');
if (remember == 'false') {
    localStorage.setItem('isCheckDN', 'false');
}
var today = new Date();
//$("#from").datepicker({
//    defaultDate: "+1w",
//    changeMonth: true,
//    changeYear: true, // Cho phép chọn năm
//    numberOfMonths: 1,
//    dateFormat: "yy-mm-dd",
//    onClose: function (selectedDate) {
//        $("#to").datepicker("option", "minDate", selectedDate);
//    }
//});
//$("#to").datepicker({
//    defaultDate: "+1w",
//    changeMonth: true,
//    changeYear: true, // Cho phép chọn năm
//    numberOfMonths: 1,
//    dateFormat: "yy-mm-dd",
//    onClose: function (selectedDate) {
//        $("#from").datepicker("option", "maxDate", selectedDate);
//    }
//});
var dd = String(today.getDate()).padStart(2, '0');
var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
var yyyy = today.getFullYear();
today = yyyy + '-' + mm + '-' + dd;
$(".lookup-results").hide(); // Ẩn danh sách kết quả lúc ban đầu
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

$(".lookup-input").on("focus", function () {
    $(".lookup-results").show();
    $(".ultitle").show()
});

$(".lookup-input").on("blur", function () {
    setTimeout(function () {
        $(".lookup-results").hide();
        $(".ultitle").hide()
    }, 200)

});
//let xeVanChuyen = 0;
//$(".xeCont").on("click", function () {
//    $(".xeTai").prop("checked", false)
//    xeVanChuyen = this.checked ? 1 : 0
//})
//$(".xeTai").on("click", function () {
//    $(".xeCont").prop("checked", false)
//    xeVanChuyen = this.checked ? 2: 0 
//})


$(".lookup-results").on("click", "li", function () {
    var selectedValue = $(this).data("name")
    maPhieuPKL = $(this).data("value")
    $(".lookup-input").val(selectedValue)
    $(".lookup-results").hide();
    $(".ultitle").hide()
    renderVlue()
});
$("#to").on("change", function () {
    getPhieuDonhang()
})
$("#from").on("change", function () {
    getPhieuDonhang()
})


//$(".hide_popup").on("click", function () {
//    $(".popup").hide()
//})
let index = 0;
//$(".list_image").on("click", ".image", function () {

//    if ($(".lookup-input").val() != "") {
//        let $this = $(this)
//        StartCamera();
//        $('#ModalImage').modal('show');
//        id_img = $this.attr("id")
//        if (id_img == 'image0') index = 1
//        else if (id_img == 'image25') index = 2
//        else if (id_img == 'image50') index = 3
//        else if (id_img == 'image75') index = 4
//        else if (id_img == 'image100') index = 5
//        else if (id_img == 'imageDoAm') index = 6
//        else if (id_img == 'imageDongCua') index = 7
//        else if (id_img == 'imageTruocChot') index = 8
//        else if (id_img == 'imageChotAT') index = 9
//    }

//})

let maPhieuPKL = ""; let Khachhang = ""; let MaCont = "";
let arr = [];
function getPhieuDonhang() {
    $(".lookup-results").empty();
    $.ajax({
        url: "/api/XuatHang/Get?Action=GetMaPhieuBienBan&para1=A&para2=A",
        type: 'Get',
        success: function (data) {
            arr = data;
            console.log(data)
            if (data.length == 0) return;
            maPhieuPKL = data[0].MaPhieuBB;
            Khachhang = data[0].TenBienBan;
            MaCont = data[0].MaSoCont;
            $(".lookup-input").val(Khachhang)
            $.each(data, function (index, item) {
                let soBooking = item.SoBooking == null ? "" : item.SoBooking
                $(".lookup-results").append(`
                    <li class="result-row" data-macont="${item.MaSoCont}" data-value=${item.MaPhieuBB} data-name="${item.TenBienBan}">
                        <div class="result-column">${item.KhachHang}</div>
                        <div class="result-column">${item.TenBienBan} </div>
                        <div class="result-column">${item.Soxe} </div>
                        <div class="result-column">${item.SoCont}</div>
                    <div class="result-column">${soBooking}</div>
                    </li>
                 `)
            })
            renderVlue()
        }
    })
}
function convertDateTime(datetime) {
    const date = new Date(datetime);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}/${month}/${year} ${hours}:${minutes}`;
}
function isCheckNoiDung() {
    $(`.xeCont`).prop("checked", false);
    $(`.xeTai`).prop("checked", false);
    $(`.xeRoMoc`).prop("checked", false);
   
}
function renderVlue() {
    let data = arr.filter(item => item.MaPhieuBB == maPhieuPKL)
    hideImage()
    $.each(data, function (index, item) {
        isCheckNoiDung()
        let kienAndSP = `${item.SLThung} kiện = ${item.SoLuong} sp`
        $(".khachhang").val(item.KhachHang)
        $(".mahang").val(item.MaHang)
        $(".Po").val(item.PO)
        $(".Soluong").val(kienAndSP)
        $(".creatDate").text(convertDateTime(item.NgayGioDongA))
        $(".finishDate").text(convertDateTime(item.NgayGioKTA))
        $(".banSo").val(item.Soxe)
        $(".container").val(item.SoCont)
        $(".romoc").val(item.SoRoMoc)
        $(".nameTX").val(item.TenTaiXe)
        $(".CMND").val(item.CMND)
        $(".congty").val(item.CongTy)
        $(".seal").text(item.TenSeal1)
        $(".cangden").val(item.CangDen)
        if (item.VanChuyen != 0) {
            if (item.VanChuyen == 1) {
                $(`.xeCont`).prop("checked", true);
                xeVC = 1
                $(".inputcontainer").show()
                $(".inputromoc").show()
              
            }
            else if (item.VanChuyen == 2) {
                $(`.xeTai`).prop("checked", true);
                xeVC = 2
                $(".inputcontainer").hide()
                $(".inputromoc").hide()
            }
        }
        if (item.XeRomoc != 0) {
            $(`.xeRoMoc`).prop("checked", true);
            xeVC = 1
        }
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
    renderImageOther()
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


getPhieuDonhang()
let isStatusHT = 0
$("#Update").on("click", function () {
    isStatusHT = 1
    saveHT()
})
$("#UpdateHT").on("click", function () {
    isStatusHT = 2
    saveHT()
})
function saveHT() {
    let object = {
        MaPKL_XH: maPhieuPKL,
        CreateDate: $(".creatDate").val(),
        FinishDate: $(".finishDate").val(),
        VCCont: xeVanChuyen,
        IsExport: isStatusHT,
        VCXeTai: $(".ghiChu").val(),
        Soxe: $(".banSo").val(),
        Cont: $(".container").val(),
        Romoc: $(".romoc").val(),
        TenTaiXe: $(".nameTX").val(),
        CMND: $(".CMND").val(),
        CongTy: $(".congty").val(),
        Seal: $(".seal").val(),
        NhanVienKiem: user,
    }
    window.location.href = "/BienBanHangXuat/Index"
    $.ajax({
        type: "POST",
        url: "/api/XuatHang/PostDL",
        data: JSON.stringify(object),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            toastr.success('Lưu thành công');
            renderVlue()
        }
    });
}
$("#xuatEX").on("click", function () {
    Export()

})
function Export() {
    var url = "/api/XuatHang/BCExcelBB?Action=GetBCEXXuathang&para1=" + maPhieuPKL + "&para2=A" + "&para3=" + MaCont;
    var link = document.createElement('a');
    var filename = `BCLoiQA.xlsx`;
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
function saveImage() {
    let object = {
        MaPKL_XH: maPhieuPKL,
        Image0: 1 == index ? imageData : "",
        Image25: 2 == index ? imageData : "",
        Image50: 3 == index ? imageData : "",
        Image75: 4 == index ? imageData : "",
        Image100: 5 == index ? imageData : "",
        ImageDoAm: 6 == index ? imageData : "",
        ImageDongCua: 7 == index ? imageData : "",
        ImageTruocChot: 8 == index ? imageData : "",
        ImageChotAT: 9 == index ? imageData : "",
    }
    console.log(object)
    $.ajax({
        type: "POST",
        url: "/api/XuatHang/PostImage",
        data: JSON.stringify(object),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            toastr.success('Lưu thành công');
            
        }
    });
}
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
function SavePreImage() {
    saveImage()
}
$(document).on("blur", ".seal", function (event) {
    $(".tablet").removeClass("active");
});
$(document).on("click", ".seal", function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $(".tablet").addClass("active")
        $("html, body").animate({ scrollTop: $(document).height() }, 800);
    }
});
function renderImageOther() {
    $.ajax({
        url: '/api/TheoDoiDonHang/Get?Action=GetImageOther&para1=' + maPhieuPKL + '&para2=a&para3=a',
        type: "Get",
        success: function (data) {
            $(".item_addImage").empty()
            count = 10;
            $.each(data, function (index, item) {
                $(".item_addImage").append(`
                 <div class="image_other ${count}">
                <div style="display: flex;">
                    <label class="label_image">${count}. </label>
                    <input id="input_image${count}" class="input_image" type="text" name="name" value="${item.NoiDung}" dis/>
                  </div>
                  <div data-name="${count}" id="lst_AddImage${count}" class="lst_AddImage">
                    <img  src="/Images/Other/${item.Image}">
                </div>
             </div>

                `)
                count++;
            })
        }
    })
}
$(document).ready(function () {
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
});