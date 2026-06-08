var lstDataVai = [];
var arrayError = {};
var arrayXY = Array();
var DiemLoi;
var gViTriLoi = "";
var userName = "";
var ojPhuLieu = {
    MaVT: "",
    TenVT: "",
    SoCay: "",
    SLThucTe: "",
    KhoThucTe: ""
}
var lstStyleID_Total = [];
var lstMaHang_ToTal = [];
var gMaGopMH = "";
const modalMH = new bootstrap.Modal(document.getElementById('ModalMH'));
const modalConfirmSoCay = new bootstrap.Modal(document.getElementById('ModalConfirmSoCay'));
$(document).ready(function () {
    LoadKH();
    LoadSoLo();
    InitNhomLoi();
    userName = localStorage.getItem('username');
    $('#KhachHang').on('change', function () {
        KhachHangChange();
    });
    $('#MaHang').on('change', function () {
        MaHangChange();
    });
    $('#SoLo').on('change', function () {
        SoLoChange();
    });
    $('#SoCay').on('change', function () {
        SoCayChange();
    });
});
$("body").on("click", "#DSDiemLoi .DiemLoi", function () {
    $('.DiemLoi').prop('checked', false); // bỏ chọn tất cả
    $(this).prop('checked', true);
    DiemLoi = $(this).data("value");
})
function LoadKH() {
    var post = {
        action: 'GetKH',
        para1: "A",
        para2: "A",
    }
    com.ajax({
        async: false,
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/KiemVai/Get',
        success: function (d) {
            $.each(d.dt1, function (key, items) {
                $('#KhachHang').append($(`<option></option>`).val(items.MaKH).html(items.TenKH));
            });
            $('#KhachHang').val('');
        }
    })
}
function KhachHangChange() {
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
        url: '/api/KiemVai/Get',
        success: function (d) {
            var data = d.dt1;
            if (data != null) {
                $.each(data, function (key, items) {
                    $('#SoLo').append($(`<option data-magop="${items.MaGop}" data-makh="${items.MaKH}" data-mahang="${items.MaHang}" data-tenhang="${items.TenHang}"  data-mahangtong="${items.MaHangTong}" ></option>`).val(items.SoLoID).html(items.SoLo));
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

            var MaHang = lstMaHang_ToTal.join(', ');
            SaveMHGop();
            $('#MaHang').val(MaHang);
        }
        else {
            if (maHangTemp != '')
                $('#MaHang').val(maHangTemp);
            else
                $('#MaHang').val(tenhang);

        }
    }
    else {
        $('#KhachHang').val('').trigger('change');
        $('#MaHang').val('');
    }
    LoadVai();
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
        url: '/api/KiemVai/Get',
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
        url: '/api/KiemVai/Get',
        success: function (d) {
            var data = d.dt1;
            if (data != null) {
                $.each(data, function (key, items) {
                    $('#MaHang').append($(`<option data-mahang=${items.MaHang}></option>`).val(items.MaDH).html(items.MaHangDisplay));
                });
            }
            MaHangChange();
        }
    });
}
function MaHangChange() {
    LoadSoLo();
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
        url: "/api/KiemVai/SaveGopMH",
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

function LoadVai() {
    $('#PhuLieuA').html("");
    var post = {
        action: 'GetVai',
        para1: gMaGopMH,
        para2: gMaGopMH,
        para3: $('#SoLo').val(),
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/KiemVai/Get',
        success: function (d) {
            var data = d.dt1;
            lstDataVai = data;
            html = '';
            if (data != null && data.length > 0) {
                var listLoaiVai = [
                    ...new Map(data.map((item) =>
                        [`${item.MaVatTu}-${item.TenVatTu}`, { MaVatTu: item.MaVatTu, TenVatTu: item.TenVatTu }]
                    )).values()
                ];
                $.each(listLoaiVai, function (key, items) {
                    //html += ` <div style="display:flex">  <input class="class-PhuLieu" type="checkbox" data-value="${items.MaVatTu}" data-name="${items.TenVatTu}" data-soluong="${items.SoLuong}" readonly="">
                    //                        <label style="font-weight: 400;font-size:14px;margin-left:5px">${items.TenVatTu} </label> </div>`
                    $('#PhuLieuA').append($(`<option></option>`).val(items.MaVatTu).html(items.TenVatTu));
                });
                ojPhuLieu.MaVT = data[0].MaVatTu;
                ojPhuLieu.TenVT = data[0].TenVatTu;
                ojPhuLieu.SoCay = data[0].SoCay;
                ojPhuLieu.SLThucTe = data[0].SLThucTe;
                ojPhuLieu.KhoThucTe = data[0].KhoThucTe;
            }
            PhuLieuChange();
        }
    });
}
function PhuLieuChange() {
    $('#MauVai').html('');
    ojPhuLieu.MaVT = $('#PhuLieuA').val();
    ojPhuLieu.TenVT = $("#PhuLieuA option:selected").text();
    var dtFilter = lstDataVai.filter(x => x.MaVatTu == $('#PhuLieuA').val());
    $.each(dtFilter, function (key, items) {
        $('#MauVai').append($(`<option data-socay=${items.SoCay} data-sltt=${items.SLThucTe} data-khott=${items.KhoThucTe}></option>`).val(items.MaVTMau).html(items.TenMau));
    });
    MauVaiChange();
}
function MauVaiChange() {
    LoadSoCay();
    LoadListChecked();
}
function LoadSoCay() {
    $('#SoCay').html('')
    var post = {
        action: 'GetSoCay',
        para1: $('#SoLo').val(),
        para2: $('#PhuLieuA').val(),
        para3: $('#MauVai').val(),
    }
    com.ajax({
        type: 'GET',
        data: ko.toJS(post),
        url: '/api/KiemVai/Get',
        success: function (d) {
            var data = d.dt1;
            if (data != null) {
                $('#SoCay').append($(`<option></option>`).val('').html(''));
                $.each(data, function (key, items) {
                    $('#SoCay').append($(`<option data-sltt=${items.SLThucTe} data-khott=${items.KhoThucTe}></option>`).val(items.BarCodeGoc).html(items.SoKienHienThi));
                });
            }
            $('#SoCay').val('');
        }
    });
}
function SoCayChange() {
    if ($('#SoCay').val() == "") return;
    var soCay = $("#SoCay option:selected").text();
    document.getElementById("TitleConfirmMH").innerHTML = "Xác nhận kiểm cây: " + soCay;
    modalConfirmSoCay.show();

}
function ConfirmSoCay(status) {
    if (status) {
        $('#SoLuong').val($("#SoCay option:selected").data("sltt"));
        $('#Kho').val($("#SoCay option:selected").data("khott"));
        SaveSLKiem("SaveSLKiem");
    }
    else {
        $('#SoCay').val('').trigger('change');
    }
    modalConfirmSoCay.hide();
}
function LoadListChecked() {
    var post = {
        action: 'GetListChecked',
        para1: gMaGopMH,
        para2: gMaGopMH,
        para3: $('#SoLo').val(),
        para4: $('#PhuLieuA').val()
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
            if (data == null || data.length == 0) {
                $('#BodyListChecked').html('');
                return;
            }

            if (data.length != 0) {
                var dtTemp = data.filter(x => x.MaVTMau == $('#MauVai').val());
                if (dtTemp.length > 0) {
                    dataBinding = dtTemp[0];
                    //$('#SoCay').val(dataBinding.SoCay ?? "0");
                    //$('#SoLuong').val(dataBinding.SoLuong ?? "0");
                    //$('#Kho').val(dataBinding.Kho ?? "0");                    
                }
                else {
                    //$('#SoCay').val("0");
                    //$('#SoLuong').val("0");
                    //$('#Kho').val("0");                 
                }
            }
            //else {
            //    $('#SoCay').val("0");
            //    $('#SoLuong').val("0");
            //    $('#Kho').val("0");
            //}
            var lstFieldName = Object.keys(data[0]);
            var lengthColspan = 0;

            $.each(data, function (key, item) {
                var ToTalDiem = 0;
                var lengthColspan_Temp = 0;
                var htmlVitri = '', htmlLoaiLoi = '', htmlDiemLoi = '';
                lstFieldName.map((x, index) => {
                    if (x.includes('@')) {
                        var diemLoi = item[x].split('@')[0];
                        var vitriLoi = item[x].split('@')[1];
                        htmlVitri += vitriLoi != "" ? ` <td>${vitriLoi}</td>` : `<td></td>`;
                        htmlLoaiLoi += vitriLoi != "0" ? ` <td>${x.split('@')[0]}</td>` : `<td></td>`;
                        htmlDiemLoi += ` <td>${diemLoi == "0" ? "" : diemLoi}</td>`
                        ToTalDiem += parseInt(diemLoi);
                        lengthColspan_Temp++;
                    }
                    //if (item[x] != "0" && x.includes('@')) {
                    //    htmlVitri += ` <td></td>`
                    //    htmlLoaiLoi += ` <td>${x.split('@')[0]}</td>`
                    //    htmlDiemLoi += ` <td>${item[x]}</td>`
                    //    ToTalDiem += parseInt(item[x]);
                    //    lengthColspan_Temp++;
                    //}                   
                })
                var result = HandleResult(ToTalDiem, item.DVTinh, item.SoLuongTT, item.Kho)
                item.KetQua = result;
                html += `<tr>
                        <td rowspan="3">${item.TenMau}</td>
                        <td rowspan="3">${item.SoCay}</td>
                        <td rowspan="3">${item.SoLuong}</td>
                        <td rowspan="3" id="SLTT_${item.ID}" 
                               onclick="OpenModalCheckList('SLTT_${item.ID}', '${item.SoCayID}', '${item.SoLuongTT}', 'SL thực tế','1')">${item.SoLuongTT}</td>
                        <td rowspan="3">${item.Kho}</td>
                        <td rowspan="3" id="KhoTT_${item.ID}" 
                                onclick="OpenModalCheckList('KhoTT_${item.ID}', '${item.SoCayID}', '${item.KhoTT}', 'khổ thực tế','0')">${item.KhoTT}</td>
                         <td >Vị trí lỗi</td>
                         ${htmlVitri == "" ? '<td></td>' : htmlVitri}
                         <td rowspan="3">${ToTalDiem}</td>
                         <td rowspan="3">${item.KetQua}</td>
                        <td rowspan="3" id="KhoTT_${item.ID}"
                                onclick="OpenModalCheckList('KhoTT_${item.ID}', '${item.SoCayID}', '${item.GhiChu}', 'ghi chú','2')">${item.GhiChu}</td>
                              <td style="max-width:100px" rowspan="3"><div onclick="DeleteCayCai('${item.SoCayID}')" class="d-flex justify-content-center align-items-center">  <button type="button" class="btn btn-danger d-flex justify-content-center align-items-center" style="height:30px;gap:5px">
                                             <i class="fas fa-trash-alt"></i> <span>Delete</span>
                                             </button></div></td>

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
            });
            $('#CTLoi').attr("colspan", lengthColspan);
            $('#BodyListChecked').html(html);
        }
    });
}
var SoCayTT = ""
var checkSave = "";
function OpenModalCheckList(id, socay, soluong, nameDisplay, valueSave) {
    SoCayTT = socay;
    checkSave = valueSave
    $("#modalNhapSoLuongLabel").text(`Nhập ${nameDisplay}`)
    if (valueSave != 2) {
        $(".itemLable").text("Số lượng");
        $("#soLuongInput").attr("type", "number");
    } else {
        $(".itemLable").text("Ghi chú");
        $("#soLuongInput").attr("type", "text");
    }
    $("#soLuongInput").val(soluong == 0 ? "" : soluong)
    $("#modalNhapSoLuong").modal("show")

}
$("#modalNhapSoLuong").on('shown.bs.modal', function () {
    $("#soLuongInput").focus();
});
function DeleteCayCai(soCay) {
    SoCayTT = soCay
    $("#modalDelete").modal("show")
}
function ConfirmDelete() {
    var data = {
        Action: "DeleteCayVai",
        MaVai: $('#PhuLieuA').val(),
        MaVTMau: $('#MauVai').val(),
        SoCay: SoCayTT,
        Dot: $('#SoLo').val(),
        SoLuongTT: 0,
        KhoTT: 0,
        GhiChu: 0,
        DiemLoi: 0
    }
    $.ajax({
        type: "POST",
        url: "/api/KiemVai/SaveSLTT",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        success: function (d) {
            if (d.Status != "OK") {
                toast({
                    title: "Lỗi",
                    message: "Xóa thất bại",
                    type: "error",
                    duration: 1000
                });
                return;
            }

            toast({
                title: "Thành Công",
                message: "Xóa thành công",
                type: "success",
                duration: 1000
            });
            LoadListChecked();
            $("#modalDelete").modal("hide")
        }
    });
}
function UpdateSLTT() {
    var data = {
        Action: "SaveSLTT",
        MaVai: $('#PhuLieuA').val(),
        MaVTMau: $('#MauVai').val(),
        SoCay: SoCayTT,
        Dot: $('#SoLo').val(),
        SoLuongTT: checkSave == "1" ? $("#soLuongInput").val() : 0,
        KhoTT: checkSave == "0" ? $("#soLuongInput").val() : 0,
        GhiChu: checkSave == "2" ? $("#soLuongInput").val() : "",
        DiemLoi: checkSave
    }
    console.log(data)
    $.ajax({
        type: "POST",
        url: "/api/KiemVai/SaveSLTT",
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
                message: "Lưu thành công",
                type: "success",
                duration: 1000
            });
            LoadListChecked();
            $("#modalNhapSoLuong").modal("hide")
        }
    });
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
    }
    return valConvert;
}

function SaveSLKiem(action) {
    if (!CheckCondition()) return;
    var data = {
        Action: action,
        MaDH: gMaGopMH,
        MaHang: gMaGopMH,
        MaVai: $('#PhuLieuA').val(),
        MaVTMau: $('#MauVai').val(),
        SoCay: $('#SoCay').val(),
        SoLuong: $('#SoLuong').val(),
        Kho: $('#Kho').val(),
        NVKiem: userName,
        Dot: $('#SoLo').val(),
        NgayKiem: $('#Ngay').val().split("-").reverse().join("-")
    }
    $.ajax({
        type: "POST",
        url: "/api/KiemVai/SaveVai",
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
                message: "Lưu thành công",
                type: "success",
                duration: 1000
            });
            LoadListChecked();
        }
    });
}
function SaveError() {
    if (!CheckCondition()) return;

    if (arrayXY.length == 0) {
        toast({
            title: "Cảnh báo",
            message: "Vui lòng chọn điểm lỗi!",
            type: "warning",
            duration: 2000
        });
        return;
    }
    var data = {
        MaDH: gMaGopMH,
        MaHang: gMaGopMH,
        MaVai: $('#PhuLieuA').val(),
        MaVTMau: $('#MauVai').val(),
        SoCay: $('#SoCay').val(),
        ArrayXY: arrayXY,
        DiemLoi: 1,
        NVKiem: userName,
        NgayKiem: $('#Ngay').val().split("-").reverse().join("-"),
        Dot: $('#SoLo').val(),
    }
    $.ajax({
        type: "POST",
        url: "/api/KiemVai/SaveError",
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
            arrayXY = Array();
            loadGridErrorDetail()
            LoadListChecked()
        }
    });
}
function SaveGhiChu() {
    var data = {
        Action: "SaveGhiChu",
        MaDH: gMaGopMH,
        MaHang: gMaGopMH,
        MaVai: ojPhuLieu.MaVT,
        Dot: $('#SoLo').val(),
        GhiChu: $('#GhiChu').val(),
        NVKiem: userName,
    }
    $.ajax({
        type: "POST",
        url: "/api/KiemVai/SaveVai",
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
    else if ($('#PhuLieuA').val() == null || $('#PhuLieuA').val() == '') {
        toast({
            title: "Thông báo",
            message: "Vui lòng chọn vải",
            type: "warning",
            duration: 2000
        });
        return false;
    }
    else if ($('#SoCay').val() == null || $('#SoCay').val() == "") {
        toast({
            title: "Cảnh báo",
            message: "Vui lòng chọn số cây!",
            type: "warning",
            duration: 2000
        });
        return;
    }
    return true;
}
function InitNhomLoi() {
    var html = "";
    var htmlMota = "";
    $.ajax({
        type: "Get",
        url: "/api/KiemVai/Get?action=GetNhomLoiChiTiet",
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
                                            <h5 style="column-span:all;font-weight:bold"> - ${nhomloi}</h5>`
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
function ClickDiemLoi() {
    $('#ModalTinhDiem').modal('hide');
    arrayXY.push({
        'PageX': 0, 'PageY': 0, 'Code': arrayError.code, 'ViTri': $('#ViTriLoi').val(),
        'Name': arrayError.name,
        'NgayKiem': utils.formatDate(new Date(), 'dd-MM-yyyy'),
        'Diem': DiemLoi

    });
    loadGridErrorDetail();
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

    document.getElementById("NhomLoi").innerHTML = nhomLoi[0].Name;;
    document.getElementById("Loi").innerHTML = loi[0].MaLoi + '. ' + loi[0].TenLoi;

    $('#ModalTinhDiem').modal('show');
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
                '<td style="width:25%">' + value.Code + '</td>' +
                '<td style="width:60%">' + value.Name + '</td>' +
                '<td style="width:15%">' + value.Diem + '</td>' +
                '</tr>';
        });
        $('#listErrorDetail').html($(row));
    }
}
$(document).ready(function () {
    handle_height_Image();
    hande_width();
    handle_height();
    handle_height_NL();

})
function hande_width() {
    let get_screen_ele = $(".js-width").width();
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
    let get_height_action = $(".set-js-width-chitietloi").height();

    get_js_get_height = $(".js-get-height").height() + get_height_action + 180;

    $(".response-table").css({ 'height': 'calc(' + height_screen_now + 'px - ' + 60 + 'px - ' + get_js_get_height + 'px)' })
    $(".js-width-chitietloi").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px + ' + 55 + 'px)' })
    $(".canhbaoloi").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px - ' + 30 + 'px)' })
    //$(".set-height").css("background", 'red');
}
function handle_height_NL() {
    let get_js_get_height = 0;
    let height_screen_now = $(window).height();
    get_js_get_height = $(".js-get-height").height() + 48;
    $(".nhomloi").css({ 'height': 'calc(' + height_screen_now + 'px - ' + get_js_get_height + 'px)' })
}
function handle_height_Image() {
    let get_js_get_height = 0;
    let height_screen_now = $(window).height();
    let get_height_action = $(".set-js-width-chitietloi").height();

    get_js_get_height = $(".js-get-height").height() + 94;

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
