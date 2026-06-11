var arrayError = {};
var arrayXY = Array();
var lstNhomLoi, lstNhomLoiCT;
var StatusKiemLai = "0";
var LineKL = "", LenhSXKL = "";
var dataGioiHan = [], dataLoi = [];
var KetQuaT = 0;
const modal = new bootstrap.Modal(document.getElementById('ModalPhuLieu'));
const modalMH = new bootstrap.Modal(document.getElementById('ModalMH'));
var ojPhuLieuT = {
    MaVT: "",
    TenVT: "",
    SLCT: "",
    SLTT: ""
}
var ojPhuLieu = {
    MaVT: "",
    TenVT: "",
    SLCT: "",
    SLTT: ""
}
var glPhuLieu = "";
var lstStyleID_Total = [];
var lstMaHang_ToTal = [];
var gMaGopMH = "";
var userName = "";
$(document).ready(function () {
    userName = localStorage.getItem('username');
    LoadKH();
    LoadSoLo();
    InitNhomLoi();
    $('#KhachHang').on('change', function () {
        KhachHangChange();
    });
    $('#MaHang').on('change', function () {
        MaHangChange();
    });
    $('#SoLo').on('change', function () {
        SoLoChange();
    });
});
function NgayKiemChange() {
    LoadMaHang();
    //LoadListChecked();
}
$("body").on("click", "#DsPhuLieu .class-PhuLieu", function () {
    $('.class-PhuLieu').prop('checked', false); // bỏ chọn tất cả
    $(this).prop('checked', true);              // chọn cái vừa click
    ojPhuLieuT.MaVT = $(this).data("value")
    ojPhuLieuT.TenVT = $(this).data("name")
    ojPhuLieuT.SLCT = $(this).data("slct")
    ojPhuLieuT.SLTT = $(this).data("sltt")
})
function LoadKH() {
    var post = {
        action: 'GetKH',
        para1: "",
        para2: "",
    }
    com.ajax({
        async: false,
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            $.each(d.dt1, function (key, items) {
                $('#KhachHang').append($(`<option></option>`).val(items.MaKH).html(items.TenKH));
            });
            $('#KhachHang').val('');
        }
    })
}
function KhachHangChange() {
    if ($('#KhachHang').val() == '') return;
    LoadMaHang();
}
function LoadSoLo() {
    $('#SoLo').html('')
    var post = {
        action: 'GetLo',
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            var data = d.dt1;
            if (data != null) {
                $.each(data, function (key, items) {
                    $('#SoLo').append($(`<option data-magop="${items.MaGop}" data-makh="${items.MaKH}" data-mahang="${items.MaHang}" data-tenhang="${items.TenHang}" data-mahangtong="${items.MaHangTong}" ></option>`).val(items.SoLoID).html(items.SoLo));
                });
            }
            $('#Ngay').val(d.datetime);
            SoLoChange();
        }
    });
}
function SoLoChange() {
    gMaGopMH = $("#SoLo option:selected").data("magop");
    var maKHTemp = $("#SoLo option:selected").data("makh");
    var mahang = $("#SoLo option:selected").data("mahang");
    var tenhang = $("#SoLo option:selected").data("tenhang");
    if (maKHTemp != "") {
        var maHangTemp = $("#SoLo option:selected").data("mahangtong");
        $('#KhachHang').val(maKHTemp).trigger('change');
        if (gMaGopMH == "" && mahang != '') {
            lstStyleID_Total = [];
            lstMaHang_ToTal = [];
            lstStyleID_Total.push(mahang);
            lstMaHang_ToTal.push(tenhang);
            var StyleID = lstStyleID_Total.join('@');
            var MaHang = lstMaHang_ToTal.join(', ');
            SaveMHGop();
            $('#MaHang').val(MaHang);
            $('#MaHang').attr('data-rvalue', StyleID);
        }
        else {
            if (maHangTemp != '')
                $('#MaHang').val(maHangTemp);
            else
                $('#MaHang').val(tenhang);
        }
        $('#PhuLieu').val('');
        $('#BodyListChecked').html('');
    }
    else {
        $('#KhachHang').val('').trigger('change');
        $('#MaHang').val('');
        $('#PhuLieu').val('');
        $('#PhuLieu').val('');
    }
    LoadPhuLieu();
    LoadListChecked();
}
function OpenModalMH() {
    modalMH.show();
}
function ChonMaHang() {
    lstStyleID_Total = [...document.querySelectorAll('.class-MaHang:checked')]
        .map(checkbox => checkbox.dataset.value);
    lstMaHang_ToTal = [...document.querySelectorAll('.class-MaHang:checked')]
        .map(checkbox => checkbox.dataset.name);
    var StyleID = lstStyleID_Total.join('@');
    var MaHang = lstMaHang_ToTal.join(', ');
    SaveMHGop();
    $('#MaHang').val(MaHang);
    $('#MaHang').attr('data-rvalue', StyleID);
    modalMH.hide();

}
function SaveMHGop() {
    var lstSave = [];
    lstStyleID_Total.map((item, index) => {
        var object = {
            ID: 0,
            SoLo: $('#SoLo').val(),
            MaGop: gMaGopMH,
            StyleID: item,
            MaHang: "",
            MaHangTong: lstMaHang_ToTal.join(', ')
        }
        lstSave.push(object);
    });
    if (lstSave.length == 0) return;
    $.ajax({
        type: "POST",
        url: "/api/PhuLieu/SaveGopMH",
        data: JSON.stringify(lstSave),
        dataType: "json",
        contentType: "application/json",
        success: function (d) {
            gMaGopMH = d;
            $('#SoLo option').each(function () {
                var $opt = $(this);
                var soLoID = $opt.val();
                // tìm object tương ứng trong newData
                var solocheck = $('#SoLo').val();
                if (soLoID == $('#SoLo').val()) {
                    $opt.data("magop", gMaGopMH);
                }
            });
        }
    });
}
function OpenModalPhuLieu() {
    modal.show();
    //$('#ModalPhuLieu').modal("show");
}
function ChonPhuLieu() {
    modal.hide();

    ojPhuLieu = ojPhuLieuT;
    $('#SLKH').val(ojPhuLieu.SLCT);
    $('#SLThuc').val(ojPhuLieu.SLTT);
    $('#PhuLieu').val(ojPhuLieu.TenVT);
    var slKiem = Math.round(parseInt($('#SLKH').val()) / 10);
    $('#SLKiem').val(slKiem)
    SaveSLKiem('SaveSLKiem');
    //LoadListChecked();
}
function LoadMaHang() {
    $('#MaHang').html("");
    var post = {
        action: 'GetMH',
        para1: $('#KhachHang').val(),
        para2: $('#SoLo').val(),
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            var html = '';
            var data = d.dt1;
            if (data != null) {
                $.each(data, function (key, items) {
                    html += ` <div style="display:flex;margin-bottom:5px">
                                      <input style="height:20px;width:20px" class="class-MaHang" ${items.Checked ? "Checked" : ""} type="checkbox" data-value="${items.MaHang}" data-name="${items.TenHang}" readonly>
                                      <label style="font-weight: 400;font-size:14px;margin-left:5px;margin-bottom:auto">${items.TenHang} </label>
                               </div>`
                });
            }
            $('#DsMaHang').html(html);
        }
    });
}
function LoadMaHang1() {
    $('#MaHang').html("");
    var post = {
        action: 'GetDH',
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            var data = d.dt1;
            if (data != null) {
                $.each(data, function (key, items) {
                    $('#MaHang').append($(`<option data-mahang=${items.MaHang} ></option>`).val(items.MaDH).html(items.MaHangDisplay));
                });
            }
            MaHangChange();
        }
    });
}
function MaHangChange() {
    LoadSoLo();
}
function LoadPhuLieu() {
    $('#PhuLieu').html("");
    var post = {
        action: 'GetPhuLieu',
        para1: "",
        para2: "",
        para3: $('#SoLo').val()
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            var data = d.dt1;
            html = '';
            if (data != null && data.length > 0) {
                $.each(data, function (key, items) {
                    html += ` <div style="display:flex;margin-bottom:5px">
                                      <input style="height:20px;width:20px" class="class-PhuLieu" type="checkbox" data-value="${items.MaVatTu}" data-name="${items.TenVatTu}" data-slct="${items.SLCT}" data-sltt="${items.SLTT}" readonly="">
                                      <label style="font-weight: 400;font-size:14px;margin-left:5px;margin-bottom:auto">${items.TenVatTu} </label>
                             </div>`
                    //$('#PhuLieu').append($(`<option data-slct=${items.SoLuong}></option>`).val(items.MaVatTu).html(items.TenVatTu));
                });
                //ojPhuLieu.MaVT = data[0].MaVatTu;
                //ojPhuLieu.TenVT = data[0].TenVatTu;
                //ojPhuLieu.SoLuong = data[0].SoLuong;
            }
            $('#DsPhuLieu').html(html)
            //PhuLieuChange();
        }
    });
}
function PhuLieuChange() {
    $('#SLKH').val(ojPhuLieu.SLCT);
    $('#SLThuc').val(ojPhuLieu.SLTT);
    $('#PhuLieu').val(ojPhuLieu.TenVT);
    var slKiem = Math.round(parseInt($('#SLThuc').val()) / 10);
    $('#SLKiem').val(slKiem)
    SaveSLKiem('SaveSLKiem');
    //LoadListChecked();
}
function LoadListChecked() {
    var post = {
        action: 'GetListChecked',
        para1: $('#MaHang').val(),
        para2: $("#MaHang option:selected").data("mahang"),
        para3: $('#SoLo').val() //$("#MaHang option:selected").data("dot")
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/PhuLieu/GetPhuLieu',
        success: function (d) {
            if (d == null) return;
            var data = d.dt1;
            dataLoi = d.dt2;
            var dtLoi = d.dt1;
            var html = '';
            $('#BodyListChecked').html('');
            if (data.length != 0) {
                var dtTemp = data.filter(x => x.MaVatTu == ojPhuLieu.MaVT);
                if (dtTemp.length > 0) {
                    dataBinding = dtTemp[0];
                    $('#QuiCach').val(dataBinding.QuiCach ?? "");
                    $('#Balance').val(dataBinding.Balance ?? "0");
                    $('#HangLoi').val(dataBinding.SoLuongLoi ?? "0");
                    $('#TLLoi').val(dataBinding.TLLoi ?? "0");
                }
                else {
                    $('#QuiCach').val("");
                    $('#Balance').val("0");
                    $('#HangLoi').val("0");
                    $('#TLLoi').val("0");
                }
            }
            else {
                $('#QuiCach').val("");
                $('#Balance').val("0");
                $('#HangLoi').val("0");
                $('#TLLoi').val("0");
            }
            if (dataLoi == null || dataLoi.length == 0) return;
            $.each(dataLoi, function (key, item) {
                html += `<tr class="${item.MaVatTu == ojPhuLieu.MaVT ? "classFocusVT" : ""}">
                        <td>${key + 1}</td>
                        <td>${item.TenVT}</td>
                        <td>${item.QuiCach ?? ""}</td>
                        <td>${item.DVTinh ?? ""}</td>
                        <td>${item.SoLuong}</td>
                        <td>${item.SoLuongThuc}</td>
                        <td>${(item.Balance == "0" || item.Balance == null) ? "" : item.Balance}</td>
                        <td>${item.SoLuongKiem}</td>                      
                        <td>${item.SoLuongLoi}</td>
                        <td>${item.TLLoi}</td>
                        <td>${item.TenLoi}</td>
                        <td>${item.KetQua}</td>
                        </tr>`
            });
            $('#BodyListChecked').html(html);
        }
    });
}

function SaveSLKiem(action) {
    if (!CheckCondition()) return;
    //if (parseInt($('#SLThuc').val()) > parseInt($('#SLKH').val())) {
    //    toast({
    //        title: "Cảnh bảo",
    //        message: "SL kiểm > SL chứng từ",
    //        type: "error",
    //        duration: 1000
    //    });
    //    return;
    //}

    var data = {
        Action: action,
        MaDH: gMaGopMH,
        MaHang: gMaGopMH,
        MaVatTu: ojPhuLieu.MaVT,
        QuiCach: $('#QuiCach').val(),
        Balance: $('#Balance').val(),
        SoLuong: $('#SLKH').val(),
        SoLuongThuc: $('#SLThuc').val(),
        SoLuongKiem: $('#SLKiem').val(),
        NVKiem: userName,
        Dot: $('#SoLo').val(),
        NgayKiem: $('#Ngay').val().split("-").reverse().join("-")
    }
    $.ajax({
        type: "POST",
        url: "/api/PhuLieu/SavePhuLieu",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        success: function (d) {
            //if (d.Status != "OK") {
            //    toast({
            //        title: "Lỗi",
            //        message: "Lưu thất bại",
            //        type: "error",
            //        duration: 1000
            //    });
            //    return;
            //}

            //toast({
            //    title: "Thành Công",
            //    message: "Lưu thành công",
            //    type: "success",
            //    duration: 1000
            //});
            LoadListChecked();
        }
    });
}
function SaveError() {
    if (!CheckCondition()) return;
    //if ($('#SLThuc').val() == 0) {
    //    toast({
    //        title: "Cảnh báo",
    //        message: "Vui lòng nhập SL kiểm!",
    //        type: "error",
    //        duration: 1000
    //    });
    //    return;
    //}
    var loi = parseInt($('#HangLoi').val()) + 1
    if (loi > parseInt($('#SLKiem').val())) {
        toast({
            title: "Cảnh báo",
            message: "Số lượng lỗi vượt số lượng kiểm!",
            type: "error",
            duration: 1000
        });
        return;
    }
    if (arrayXY.length == 0) {
        toast({
            title: "Cảnh báo",
            message: "Vui lòng chọn điểm lỗi!",
            type: "warning",
            duration: 1000
        });
        return;
    }
    var data = {
        MaDH: gMaGopMH,
        MaHang: gMaGopMH,
        MaVatTu: ojPhuLieu.MaVT,
        ArrayXY: arrayXY,
        NVKiem: userName,
        NoiDung: $('#NoiDung').val(),
        NgayKiem: $('#Ngay').val().split("-").reverse().join("-"),
        Dot: $('#SoLo').val(),
    }
    $.ajax({
        type: "POST",
        url: "/api/PhuLieu/SaveError",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        success: function (d) {
            if (d.Status != "OK") {
                toast({
                    title: "Lỗi",
                    message: "Lưu thất bại",
                    type: "error",
                    duration: 1000
                });
                return;
            }
            toast({
                title: "Thành Công",
                message: "Đã lưu lỗi thành công",
                type: "success",
                duration: 1000
            });
            //$('#NoiDung').val('');
            //document.getElementById('lstErrorImages').innerHTML = '';
            var loi = parseInt($('#HangLoi').val()) + 1;
            $('#HangLoi').val(loi);
            arrayXY = Array();
            loadGridErrorDetail()
            LoadListChecked()
        }
    });
}
$('#SLKCS').on('change', function () {

    SaveSLKiem("SaveSLKiem");
});
function SaveGhiChu() {
    if (!CheckCondition()) return;
    var data = {
        Action: "SaveGhiChu",
        MaDH: gMaGopMH,
        MaHang: gMaGopMH,
        MaVatTu: ojPhuLieu.MaVT,
        Dot: $('#SoLo').val(),
        GhiChu: $('#GhiChu').val(),
        NVKiem: userName,
    }
    $.ajax({
        type: "POST",
        url: "/api/PhuLieu/SavePhuLieu",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        success: function (d) {
            if (d.Status != "OK") {
                toast({
                    title: "Lỗi",
                    message: "Lưu thất bại",
                    type: "error",
                    duration: 1000
                });
                return;
            }
            else {
                toast({
                    title: "Thành công",
                    message: "Lưu thành công",
                    type: "success",
                    duration: 1000
                });
            }

        }
    });
}
function CheckCondition() {
    if ($('#MaHang').val() == '') {
        toast({
            title: "Thông báo",
            message: "Vui lòng chọn mã hàng",
            type: "warning",
            duration: 2000
        });
        return false;
    }
    else if ($('#PhuLieu').val() == '') {
        toast({
            title: "Thông báo",
            message: "Vui lòng chọn phụ liệu",
            type: "warning",
            duration: 2000
        });
        return false;
    }
    return true;
}
function clearCavasAll() {
    arrayXY = Array();
    //var canvas = document.getElementById("imageSample");
    //var ctx = canvas.getContext("2d");
    //ctx.clearRect(0, 0, canvas.width, canvas.height);
    //ReDrawLoiKiem();
    loadGridErrorDetail();
}
function clearCavas() {
    if (arrayXY.length == 0) return;
    //e.stopImmediatePropagation();
    //e.preventDefault();

    //var canvas = document.getElementById("imageSample");
    //var ctx = canvas.getContext("2d");
    //var x = arrayXY[arrayXY.length - 1].PageX,
    //    y = arrayXY[arrayXY.length - 1].PageY,
    //    radius = 16;

    //ctx.beginPath();
    //ctx.clearRect(x - radius - 1, y - radius - 1, radius * 2 + 2, radius * 2 + 2);
    //ctx.closePath();

    arrayXY.splice(arrayXY.length - 1, 1);
    loadGridErrorDetail();
}
function loadGridErrorDetail() {
    if (arrayXY.length == 0) {
        var row = '';
        $('#listErrorDetail').html($(row));
    }
    else {
        var row = '';
        $.each(arrayXY, function (key, value) {
            row = row + '<tr>' +
                '<td style="width:30%">' + value.Code + '</td>' +
                '<td style="width:70%">' + value.Name + '</td>' +
                '</tr>';
        });
        $('#listErrorDetail').html($(row));
    }
}
function InitNhomLoi() {
    var html = "";
    var htmlMota = "";
    $.ajax({
        type: "Get",
        url: "/api/PhuLieu/GetPhuLieu?action=GetNhomLoiWeb",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data == null) return;
            lstNhomLoi = data.dt1;
            lstNhomLoiCT = data.dt2;
            $.each(lstNhomLoi, function () {
                var idNhom = this.ID, nhomloi = this.Name, backColor = this.BackColor, forColor = this.ForColor;
                html += `<div style="display:flex;justify-content:flex-start;flex-wrap:wrap">`
                htmlMota += `<div style=" column-count: 2; column-gap: 10px; margin-bottom: 5px;width:100%">
                                            <h6 style="column-span:all;font-weight:bold"> - ${nhomloi}</h6>`
                var result = $.grep(lstNhomLoiCT, function (e) { return e.NhomLoi == idNhom; });
                $.each(result, function () {
                    html += `<div class="info-box item-MaLoi" onclick=ClickLoi("${this.NhomLoi}","${this.MaLoi}") style="background-color:${backColor}; color:${forColor}" data-dismiss="modal" data-toggle="tooltip" data-placement="bottom" title="Tooltip on bottom">${this.MaLoi}</div>`
                    htmlMota += `<p class="p-motaloi"><b>${this.MaLoi}</b> = ${this.TenLoi} </p>`
                });
                html += `</div>`
                htmlMota += `</div>`
            });
            $('#ChiTietMaLoi').html(html);
            $('#MotaLoi').html(htmlMota);
        }
    });

}
function ClickLoi(_nhomLoi, _maLoi) {
    arrayError.code = _maLoi;
    arrayError.backColor = '#FFF';
    arrayError.foreColor = 'red';
    //Lấy lối được chọn
    var loi = $.grep(lstNhomLoiCT, function (e1) { return $.trim(e1.MaLoi) == $.trim(arrayError.code); });
    arrayError.name = loi[0].TenLoi;
    arrayError.loaiLoi = loi[0].LoaiLoi;
    var nhomLoi = $.grep(lstNhomLoi, function (e1) { return e1.ID == loi[0].NhomLoi; });
    if (nhomLoi == null && nhomLoi.length == 0) return;
    if (nhomLoi[0].BackColor != null)
        arrayError.backColor = nhomLoi[0].BackColor;
    if (nhomLoi[0].ForColor != null)
        arrayError.foreColor = nhomLoi[0].ForColor;
    arrayError.maCongDoan = "";
    document.getElementById("NhomLoi").innerHTML = nhomLoi[0].Name;;
    document.getElementById("Loi").innerHTML = loi[0].MaLoi + '. ' + loi[0].TenLoi;
    arrayXY.push({
        'PageX': 0, 'PageY': 0, 'Code': arrayError.code, 'MaCongDoan': arrayError.maCongDoan,
        'Name': arrayError.name,
        'NgayKiem': utils.formatDate(new Date(), 'dd-MM-yyyy'),

    });
    loadGridErrorDetail();
    //$('#myModal').modal('hide');
}
$(document).ready(function () {
    handle_height_Image();
    hande_width();
    handle_height();
    handle_height_NL();

})
function hande_width() {
    let get_screen_ele = $(".js-width").width() + 12;
    let get_screen_ele_chitietloi = $(".js-width-chitietloi").width();

    console.log(get_screen_ele, "get_screen_ele");

    if (window.innerHeight > window.innerWidth) {
        if (get_screen_ele > 800) {
            $(".set-js-width").css("width", get_screen_ele);
        }
        else {
            $(".set-js-width").css("width", get_screen_ele);
        }
    }
    else {
        if (get_screen_ele > 800) {
            $(".set-js-width").css("width", get_screen_ele + get_screen_ele_chitietloi + 15);
        }
        else {
            $(".set-js-width").css("width", get_screen_ele + get_screen_ele_chitietloi + 15);
        }
    }
    get_screen_ele = $(".js-width").width() + 12;
    $(".set-js-width").css("width", get_screen_ele);

    let right = 24 + get_screen_ele;
    //$(".set-js-width-chitietloi").css("right", right);
    $(".set-js-width-chitietloi").css("width", get_screen_ele_chitietloi);
}
function handle_height() {
    let get_js_get_height = 0;
    let height_screen_now = $(window).height();
    let get_height_action = $(".set-js-width-chitietloi").height() + 170;

    get_js_get_height = $(".js-get-height").height() + get_height_action;

    $(".response-table").css({ 'height': 'calc(' + height_screen_now + 'px - ' + 60 + 'px - ' + get_js_get_height + 'px)' })
    $(".js-width-chitietloi").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px + ' + 55 + 'px)' })
    $(".canhbaoloi").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px - ' + 30 + 'px)' })
    //$(".set-height").css("background", 'red');
}
function handle_height_NL() {
    let get_js_get_height = 0;
    let height_screen_now = $(window).height();
    get_js_get_height = $(".js-get-height").height() + 40;
    $(".nhomloi").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px)' })
}
function handle_height_Image() {
    let get_js_get_height = 0;
    let height_screen_now = $(window).height();
    let get_height_action = $(".set-js-width-chitietloi").height();

    get_js_get_height = $(".js-get-height").height() + 84;

    $(".js-width").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px)' })

    $(".box-error").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px - ' + 40 + 'px)' })
}
$(window).on("resize", function (event) {
    $('.sLenhSX').each(function () {
        if ($(this).data('select2')) {
            $(this).select2('close');
        }
    });
    hande_width();
    handle_height();
    handle_height_NL();
    handle_height_Image();

    let get_screen_ele = $(".js-width").width() + 12;
    let get_screen_ele_chitietloi = $(".js-width-chitietloi").width();

    //if (window.innerHeight > window.innerWidth) {
    //    if (get_screen_ele > 800) {
    //        $(".set-js-width").css("width", get_screen_ele);
    //    }
    //    else {
    //        $(".set-js-width").css("width", get_screen_ele);
    //    }
    //}
    //else {
    //    if (get_screen_ele > 800) {
    //        $(".set-js-width").css("width", get_screen_ele + get_screen_ele_chitietloi + 15);
    //    }
    //    else {
    //        $(".set-js-width").css("width", get_screen_ele + get_screen_ele_chitietloi + 15);
    //    }
    //}

    //let right = 24 + get_screen_ele;
    //$(".set-js-width-chitietloi").css("width", get_screen_ele_chitietloi);
    //$(".set-js-width-chitietloi").css("right", right);
});