

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
let userWin = data.userName
let user = userWin == null ? localStorage.getItem("username1") : userWin

let _dvsx = "";
$("#home").on("click", function () {
    window.location.href = '/Home/Dashboard'
})
let module = 1;
$("#creatDot").on("click", function () {
    module = 1
    emptySelectViewNhap()
    renderDH()
    $("#delete").hide()
    $("#hoanthanh").show()
    //$("#Update").show()
    $(".solieunhap").show()
    $(".thongke").hide()
    $(".thongkeChiTiet").hide()
})
$("#viewDot").on("click", function () {
    module = 2
    emptySelectViewNhap()
    renderGCDH()
    /*$("#delete").show()*/
    $("#hoanthanh").hide()
    $("#Update").hide()
    $(".solieunhap").hide()

})

function renderDVSX() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetDVSX&para1=" + user + "&para2=A&para3=a&para4=A&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                toastr.error("Tài khoản không có quyền gia công")
                return;
            }
            else {
                let html = ``;
                data.map(item => {
                    html += ` <option value="${item.MaDVSX}">${item.TenDVSX}</option> `
                })
                $(".dvsx").html(html)
                $(".dvsx").select2();
                renderDH()
            }
        }
    })
}
renderDVSX()
//view nhập số lượng
function emptySelectViewNhap() {
    $(".donhang").empty();
    $(".po").empty();
    $(".dausize").empty();
    $("#tbody").empty();
}
$(".dvsx").on("change", function () {
    renderDH();
})
function renderDH() {
    _dvsx = $(".dvsx").val()
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=GetDH&para1=${user}&para2=${_dvsx}&para3=a&para4=A&para5=a&para6=A`,
        type: 'Get',
        success: function (data) {
            $(".donhang").empty();
            $(".po").empty();
            $(".dausize").empty();
            $("#tbody").empty();
            if (data.length == 0) return;
            $.each(data, function (index, item) {
                $(".donhang").append(`
                   <option data-magop="${item.MaGop}" value="${item.MaDH}">${item.TenLenh}</option>
            `)
            })
            $(".donhang").select2()

            renderPO()

        }
    })
}
function renderPO() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetPOSP&para1=" + _dvsx + "&para2=" + $(".donhang option:selected").data("magop") + "&para3=a&para4=A&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            $(".po").empty();
            if (data.length == 0) return;
            $.each(data, function (index, item) {
                $(".po").append(`
                   <option  value="${item.POID}">${item.PO}</option>
            `)
            })
            $(".po").select2()
            renderDauSize()
        }
    })
}
function renderDauSize() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetDauSize&para1=" + _dvsx + "&para2=" + $(".donhang option:selected").data("magop") + "&para3=" + $(".po").val() + "&para4=a&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            $(".dausize").empty();
            if (data.length == 0) return;
            $.each(data, function (index, item) {
                $(".dausize").append(`
                   <option value="${item.DauSizeID}">${item.DauSize}</option>
            `)
            })
            $(".dausize").select2()
            renderTableNhap()
        }
    })
}
function renderMaPKL() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetMaPKL&para1=" + $(".donhang option:selected").data("magop") + "&para2=" + _dvsx + "&para3=" + dotSX + "&para4=a&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                Swal.fire({
                    title: `Vui lòng lập PKL Đợt ${dotSX} `,
                    icon: 'error',
                    confirmButtonText: 'OK',
                    confirmButtonColor: '#3085d6',
                    customClass: {
                        confirmButton: 'custom-confirm-button'
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        $(".popup").hide()
                    }
                });
            }
            let html = "";
            data.map(item => {
                html += `
                    <option data-slsp="${item.SLN}" data-sldd="${item.SLSPDDT}" data-poid="${item.POID}" value="${item.MaPKL}">${item.Display}</option>
                `
            })

            $(".maPKL").html(html)
            $(".maPKL").select2()
            getHtmSLDD()
        }
    })
}
$(".save_list").on("click", function () {
    renderMaxSLThung()
})
$(".maPKL").on("change", function () {
    getHtmSLDD()
})
function renderMaxSLThung() {
    let poMaPKL = $(".maPKL option:selected").data("poid")
    let tuthung = $(".item_tuthung").val();
    let denthung = $(".item_denthung").val();
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetMaxSLThung&para1=" + $(".maPKL").val() + "&para2=" + _dvsx + "&para3=" + sizeid + "&para4=" + poMaPKL + "&para5=" + mamau + "&para6=" + tuthung + "&para7=" + denthung + "&para8=" + dotSX,
        type: 'Get',
        success: function (data) {
            denThungMax = data[0].SttThung
            renderIsDongThung()
        }
    })
}
let denThungMax = "";
function renderIsDongThung() {
    let poMaPKL = $(".maPKL option:selected").data("poid")
    console.log(poMaPKL)
    let tuthung = $(".item_tuthung").val();
    let denthung = $(".item_denthung").val();
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetSLDongThung&para1=" + $(".maPKL").val() + "&para2=" + _dvsx + "&para3=" + sizeid + "&para4=" + poMaPKL + "&para5=" + mamau + "&para6=" + tuthung + "&para7=" + denthung + "&para8=" + dotSX,
        type: 'Get',
        success: function (data) {
            let denThungNew = denthung > denThungMax ? denThungMax : denthung
            let transformedData = splitDataByRange(data);
            let joinedRangesChuDT = joinRanges(transformedData);
            if (joinedRangesChuDT == "") {
                sokienXacNhan = denThungNew - tuthung + 1
                updateErpDongThung()
                return;
            }
            sokienXacNhan = Number(denThungNew) - (data.length + 1) + Number(soKienOld) + 1
            let missingRange = findMissingRange(transformedData, tuthung, denThungNew);
            let joinedRangsDt = joinRanges(missingRange)

            if (joinedRangsDt.length > 0) {
                showConfirmation(joinedRangesChuDT, joinedRangsDt, denThungNew, data.length + 1)
            }
            else {
                Swal.fire({
                    title: `Thùng ${tuthung} - ${denthung} đã được đóng `,
                    icon: 'error',
                    timer: 2000,
                    showConfirmButton: false,
                });
                $(".popup").hide()
                return;
            }
        }
    })
}
function getHtmSLDD() {
    let slthung = $(".maPKL option:selected").data("slsp")
    let slddthung = $(".maPKL option:selected").data("sldd")
    let html = `
             <div class="item_size">
                            <p>SLSP: <span >${slthung}</span></p>
                        </div>
                        <div class="item_color">
                            <p>SLT Đã đóng: <span> ${slddthung} </span></p>
                        </div>
                       
            `
    $(".NumBer").html(html)
}
function updateErpDongThung() {
    let poMaPKL = $(".maPKL option:selected").data("poid")
    let tuthung = $(".item_tuthung").val();
    let denthung = $(".item_denthung").val();
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=UpdateErpDongThung&para1=" + $(".maPKL").val() + "&para2=" + _dvsx + "&para3=" + sizeid + "&para4=" + poMaPKL + "&para5=" + mamau + "&para6=" + tuthung + "&para7=" + denthung + "&para8=" + dotSX,
        type: 'Get',
        success: function (data) {
            $(".popup").hide()
            obectSave();

        }
    })
}
function showConfirmation(joinedRangesChuDT, joinedRangsDt, denThungNew, countdata) {
    Swal.fire({
        title: `Thùng ${joinedRangesChuDT} đã đóng,bạn có muốn đóng thùng ${joinedRangsDt} không? Số lượng : ${Number(denThungNew) - countdata + 1}`,
        icon: 'question',
        showCancelButton: true,
        cancelButtonText: 'Hủy',
        confirmButtonText: 'Đồng ý',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,

    }).then((result) => {
        if (result.isConfirmed) {
            updateErpDongThung()
        }
    });
}
function joinRanges(ranges) {
    return ranges.map(item => `${item.BatDau}-${item.KetThuc}`).join(", ");
}
function findMissingRange(ranges, start, end) {
    let missingRanges = [];
    start = parseInt(start, 10);
    end = parseInt(end, 10);

    ranges.sort((a, b) => a.BatDau - b.BatDau);

    // Kiểm tra trước phần tử
    if (ranges.length > 0 && ranges[0].BatDau > start) {
        missingRanges.push({
            BatDau: start,
            KetThuc: ranges[0].BatDau - 1
        });
    }

    // Kiểm tra giữa phần từ
    for (let i = 0; i < ranges.length - 1; i++) {
        if (ranges[i].KetThuc + 1 < ranges[i + 1].BatDau) {
            missingRanges.push({
                BatDau: ranges[i].KetThuc + 1,
                KetThuc: ranges[i + 1].BatDau - 1
            });
        }
    }

    // kiểm tra sau phần tử
    if (ranges.length > 0 && ranges[ranges.length - 1].KetThuc < end) {
        missingRanges.push({
            BatDau: ranges[ranges.length - 1].KetThuc + 1,
            KetThuc: end
        });
    }

    if (ranges.length === 0) {
        missingRanges.push({
            BatDau: start,
            KetThuc: end
        });
    }

    return missingRanges;
}
function splitDataByRange(data) {
    let arrMangCheck = [];
    let index = 0;
    // Kiểm tra lấy các phần tử chưa đóng thùng
    for (let i = 0; i < data.length; i++) {
        let nextSttThung = (i + 1 < data.length) ? data[i + 1].SttThung : 123123;
        let currentSttThung = data[i].SttThung;

        if (currentSttThung + 1 != nextSttThung) {
            let object = {
                BatDau: data[i - index].SttThung,
                KetThuc: currentSttThung
            };
            arrMangCheck.push(object);
            index = -1;
        }

        index++;
    }

    return arrMangCheck;
}

let dotNhap = 0;
let moduleSLKH = 1;
function renderTableNhap() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetSoLuong&para1=" + _dvsx + "&para2=" + $(".donhang").val() + "&para3=" + $(".po").val() + "&para4=" + $(".dausize").val() + "&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            $("#tbody").empty();
            let totalSLKH = data.reduce((acc, curr) => acc + parseInt(curr.SlKH), 0);
            let totalSLNhap = 0;
            let totalLuyKe = data.reduce((acc, curr) => acc + parseInt(curr.LuyKe), 0);
            if (data.length == 0) return;
            if (data[0].IsHoanThanh == 2 || data[0].IsHoanThanh == 0) {
                dotNhap = data[0].Dot + 1;
            }
            else {
                dotNhap = data[0].Dot
            }
            let html = "";
            if (moduleSLKH == 1) {
                data = data.filter(item => item.SlKH > 0)
            }
            $.each(data, function (index, item) {
                var soLuong;
                var soKien
                if (item.IsHoanThanh == 0 || item.IsHoanThanh == 2) {
                    soLuong = 0;
                    soKien = 0
                } else {
                    soLuong = item.SoLuongNhap;
                    soKien = item.SoKien;
                }
                var luyke = item.LuyKe;
                $("#tbody").append(`
                    <tr data-mamau="${item.MaMau}" data-slkh="${item.SlKH}" data-malenh="${item.MaLenhSanXuat}" data-tenmau="${item.TenMau}" data-index="${index}"  data-sizeid="${item.SizeID}" data-dausize= "${item.DauSize}" data-po="${item.DotSX}" data-size= "${item.Size}"  data-color="${item.MaMau}">
                       <td class="tenLenh">${item.TenLenh}</td>
                       <td class="tenmau">${item.TenMau}</td>
                       <td >${item.DotSX1}</td>
                       <td class="size">${item.Size}</td>
                       <td class = "slkh">${item.SlKH}</td>
                          <td id="quantity" class="quantity${item.Size}${item.MaMau}${item.DotSX}${index}" >${soLuong}</td>
                        <td class="luyke${item.Size}${item.MaMau}${item.DotSX}${index}">${luyke}</td>
                    
                       
                       <td >${dotNhap}</td>
                    </tr>
                `);
                totalSLNhap += soLuong
            });
            let htmltfoot = `
                       <tr>
                       <td colspan="4"></td>
                       
                       <td>${totalSLKH}</td>
                          <td >${totalSLNhap}</td>
                        <td>${totalLuyKe}</td>
                    
                       <td ></td>
                    </tr>
            `
            $("#tfootTotal").html(htmltfoot)


        }
    })
}
let po = ""; dausize = ""; size = ""; sizeid = ""; tenlenh = ""; slkh = ""; sllk = ""; soluongnhap = ""; tenmau = ""; mamau = "";
let malenh = ""
//Chang view nhâp liệu
$(".donhang").on("change", function () {
    if (module == 1) {
        renderPO()
    } else {
        renderGCPO()
    }

})
$(".po").on("change", function () {
    if (module == 1) {
        renderDauSize()
    } else {
        renderGCDauSize()
    }

})
$(".dausize").on("change", function () {
    if (module == 1) {
        renderTableNhap()
    } else {
        renderTableTong()
    }
})
let $sizeID = "";
let $colorID = "";
let soluongOld = "";
let soKienOld = 0;
let dotSX = "";
$("#tbody").on("click", ".quantitySoKienOne", function () {

    let $tr = $(this).closest("tr")
    renderTr($tr)
    renderMaPKL()
})
function renderTr($tr) {
    tdSize = $tr.data("size")
    size = $tr.find(".size").text()
    mamau = $tr.data("mamau")
    po = $tr.data("po")
    dotSX = $tr.data("po")
    tenlenh = $tr.find(".tenLenh").text()
    malenh = $tr.data("malenh")
    tenmau = $tr.find(".tenmau").text()
    dausize = $tr.data("dausize")
    sizeid = $tr.data("sizeid")
    slkh = $tr.data("slkh")
    soluongOld = $tr.find(`.quantity${tdSize}${mamau}${po}`).text()
    let SLThungOld = $("#tbody").find(`.soKien${tdSize}${mamau}${po}`).text()
    soKienOld = $("#tbody").find(`.soKien${tdSize}${mamau}${po}`).text()
    $(".popup").show()
    let html = `
                   <div class="item_size">
                            <p>Size: <span >${tdSize}</span></p>
                        </div>
                        <div class="item_color">
                            <p>Màu:<span> ${tenmau} </span></p>
                        </div>
                        <div class="item_skold">
                            <p>SL nhập:<span> ${SLThungOld} </span></p>
                        </div>
     `
    $(".textSizeColor").html(html)
}
$(".btn_soKien").on("click", function () {
    window.location.href = '/NhanHangGiaCong/SoKien';
})
$(".cancel_list").on("click", function () {
    $(".popup").hide()
})
$("#tbody, #tbodyTong,#tbodyDot").on("click", "tr", function () {
    $(this).closest("tbody").find("tr").removeClass("active1");
    $(this).addClass("active1");
});
$("#closePopupBtn").on("click", function () {
    $(".popupTextarea").hide()
    $(".nhapLieu").val("")
    $("#tbody").find(`.luyke${tdSize}${mamau}${po}${index}`).text(sllk)
    $("#tbody").find(`.${clickedClass}`).text(soluongnhap)
})
$(".cancel").on("click", function () {
    $(".nhapLieu").val("")
    $(".nhapLieu").focus()
    $("#tbody").find(`.luyke${tdSize}${mamau}${po}${index}`).text(sllk)
    $("#tbody").find(`.${clickedClass}`).text(soluongnhap)
})
let clickedClass = "";
let tdSize = "";
let index = "";
$("#tbody").on("click", "#quantity", function () {
    clickedClass = $(this).attr("class");

    $(".popupTextarea").show()
    let $tr = $(this).closest("tr")
    tdSize = $tr.data("size")
    size = $tr.find(".size").text()
    index = $tr.data("index")
    slkh = $tr.data("slkh")
    mamau = $tr.data("mamau")
    po = $tr.data("po")
    sllk = $tr.find(`.luyke${tdSize}${mamau}${po}${index}`).text()
    tenlenh = $tr.find(".tenLenh").text()
    malenh = $tr.data("malenh")
    tenmau = $tr.find(".tenmau").text()
    soluongnhap = $((`.${clickedClass}`)).text();
    soKienOld = $("#tbody").find(`.soKien${tdSize}${mamau}${po}`).text()
    dausize = $tr.data("dausize")
    sizeid = $tr.data("sizeid")
    let html = `
                 <div class="item_size  solieu">Size: ${size}</div>
                <div class="item_SLKH solieu"> SLKH: ${slkh}</div>
                <div class="item_luyke solieu"> Luy kế : ${sllk}</div>
                <div class="item_soluong solieu">Số lượng cũ : ${soluongnhap}</div>
    `
    $(".text_chitiet").html(html)
    $(".nhapLieu").focus()
})
$("body").on("keypress", ".nhapLieu", function (event) {
    var charCode = event.keyCode || event.which;
    if ((charCode >= 48 && charCode <= 57) || charCode == 8 || charCode == 13) {
        return true;
    }
    else if (charCode == 43 || charCode == 45 || charCode == 42 || charCode == 47) {
        event.preventDefault();
        return false;
    }
    else {
        event.preventDefault();
        return false;
    }
});
let arr = [];
$(".nhapLieu").on("input", function () {
    let slnhap = $(this).val()
    let sumSLNhapSLLK = Number(slnhap) + Number(sllk) - Number(soluongnhap)
    if (sumSLNhapSLLK > slkh) {
        toastr.error("Số lượng vượt quá số lượng KH")
        let newGiatri = slnhap.slice(0, -1);
        sumSLNhapSLLK = Number(newGiatri) + Number(sllk)
        $(this).val(newGiatri)
        $("#tbody").find(`.luyke${tdSize}${mamau}${po}${index}`).text(sumSLNhapSLLK)
        $("#tbody").find(`.${clickedClass}`).text(newGiatri)
        $(".item_luyke ").text("Luy kế :" + sumSLNhapSLLK)
    }
    else {
        $("#tbody").find(`.luyke${tdSize}${mamau}${po}${index}`).text(sumSLNhapSLLK)
        $("#tbody").find(`.${clickedClass}`).text(slnhap)
        $(".item_luyke ").text("Luy kế :" + sumSLNhapSLLK)
    }
})
$("#result").on("click", function () {
    let index = arr.findIndex(item => item.SizeID === sizeid && item.MaMau === mamau && item.PO === po);
    if (index !== -1)
        arr[index].SoLuong = soluongnhap;
    else {
        obectSave()
    }
    $(".nhapLieu").val("")
    $(".popupTextarea").hide()
})
let sokienXacNhan = 0;
function obectSave() {
    let valueInput = $(".nhapLieu").val()
    if (valueInput != "")
        $("#tbody").find(`.${clickedClass}`).text(valueInput);
    else valueInput = 0;
    let soKien = sokienXacNhan;
    let object = {
        MaLenh: malenh,
        MaDH: $(".donhang").val(),
        MaDVSX: _dvsx,
        POID: $(".po").val(),
        PO: po,
        MaMau: mamau,
        TenMau: tenmau,
        DauSizeID: $(".dausize").val(),
        DauSize: dausize,
        SizeID: sizeid,
        Size: size,
        SoLuong: valueInput,
        LuyKe: valueInput,
        SoKien: sokienXacNhan,
        SoLuongKH: slkh,
        Dot: dotNhap,
        NgayKiem: "",
        NhanVien: user,
        Status: 0,
        IsHoanThanh: 1
    }
    arr.push(object)
    arrnew = arr
    $(".item_tuthung").val("");
    $(".item_denthung").val("");
    save()
    sokienXacNhan = 0;

}
let arrnew = [];
function locTbody() {
    $("#tbody").find("tr").each(function () {
        let $tr = $(this)
        let quantityValue = $tr.find("#quantity").text();
        let soKien = $tr.find(".quantitySoKienOne").text();
        tdSize = $tr.data("size")
        if (quantityValue.trim() != "0") {
            let object = {
                MaLenh: $tr.data("malenh"),
                MaDH: $(".donhang").val(),
                MaDVSX: _dvsx,
                POID: $(".po").val(),
                PO: $tr.data("po"),
                MaMau: $tr.data("mamau"),
                TenMau: $tr.find(".tenmau").text(),
                DauSizeID: $(".dausize").val(),
                DauSize: $tr.data("dausize"),
                SizeID: $tr.data("sizeid"),
                Size: $tr.find(".size").text(),
                SoLuong: quantityValue,
                LuyKe: quantityValue,
                SoLuongKH: $tr.find(".slkh").text(),
                SoKien: Number(soKien),
                Dot: dotNhap,
                NgayKiem: "",
                NhanVien: user,
                Status: 0,
                IsHoanThanh: 0
            }
            arr.push(object)

        }


    });
}
$("#hoanthanh").on("click", function () {
    if (arr.length == 0) {
        locTbody()
    }
    if (arr.length == 0) {
        toastr.error("Vui lòng nhập số lượng")
        return
    }
    arrnew = arr.map(item => ({
        ...item,
        IsHoanThanh: 2
    }))

    save()
})
$("#Update").on("click", function () {


})
function save() {
    $.ajax({
        type: "POST",
        url: "/api/NHGiaCong/Post",
        data: JSON.stringify(arrnew),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            arr = [];
            arrnew = []
            toastr.success("Lưu thành công")
            renderTableNhap()
        }
    });
}
//view thống kê tổng

function renderGCDH() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetDHGC&para1=" + _dvsx + "&para2=A&para3=a&para4=A&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            $(".donhang").empty();
            if (data.length == 0) return;
            $.each(data, function (index, item) {
                $(".donhang").append(`
                   <option value="${item.MaDH}">${item.MaLenh}</option>
            `)
            })
            $(".donhang").select2()
            renderGCPO()
        }
    })
}
function renderGCPO() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetPOGC&para1=" + _dvsx + "&para2=" + $(".donhang").val() + "&para3=a&para4=A&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            $(".po").empty();
            if (data.length == 0) return;
            $.each(data, function (index, item) {
                $(".po").append(`
                   <option value="${item.POID}">${item.PO}</option>
            `)
            })
            $(".po").select2()
            renderGCDauSize()
        }
    })
}
function renderGCDauSize() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetDauSizeGC&para1=" + _dvsx + "&para2=" + $(".donhang").val() + "&para3=" + $(".po").val() + "&para4=a&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            $(".dausize").empty();
            if (data.length == 0) {
                $(".thongkeChiTiet").hide()
                $(".thongke").hide()
                toastr.error("Chưa có hàng gia công")
                return;
            }
            $(".thongkeChiTiet").show()
            $(".thongke").show()
            $.each(data, function (index, item) {
                $(".dausize").append(`
                   <option value="${item.DauSizeID}">${item.DauSize}</option>
            `)
            })
            $(".dausize").select2()
            renderTableTong()
        }
    })
}
let dotGiaCong = "";
function renderTableTong() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetTableDot&para1=" + _dvsx + "&para2=" + $(".donhang").val() + "&para3=" + $(".po").val() + "&para4=" + $(".dausize").val() + "&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) return;
            $("#tbodyTong").empty();
            dotGiaCong = data[0].Dot;
            $.each(data, function (index, item) {
                $("#tbodyTong").append(`
                    <tr>
                       <td >${item.MaLenh}</td>
                       <td >${item.TenMau}</td>
                       <td >${item.SoLuongKH}</td>
                        <td >${item.LuyKeThung}</td>
                       <td >${item.SLThung}</td>
                    <td >${item.SoLuong}</td>
                       <td >${item.LuyKe}</td>
                     
                       <td class="dotGiaCong" >${item.Dot}</td>
                    </tr>
                `);
            });
            renderTableChiTiet()
        }
    })
}

function renderTableChiTiet() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetTableChiTietDot&para1=" + _dvsx + "&para2=" + $(".donhang").val() + "&para3=" + $(".po").val() + "&para4=" + $(".dausize").val() + "&para5=" + dotGiaCong + "&para6=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) return;
            $("#tbodyDot1").empty();

            $.each(data, function (index, item) {
                $("#tbodyDot1").append(`
                    <tr>
                       <td >${item.MaLenh}</td>
                       <td >${item.TenMau}</td>
                       <td >${item.Size}</td>
                       <td >${item.SoLuongKH}</td>
                       <td >${item.LuyKe}</td>
                       <td >${item.SoLuong}</td>
                       <td >${item.Dot}</td>
                    </tr>
                `);
            });
        }
    })
}
$("#tbodyTong ").on("click", "tr", function () {
    dotGiaCong = $(this).find(".dotGiaCong").text()
    renderTableChiTiet()
})
$("#eye-hideSL").on("click", function () {
    $(this).toggleClass('fa-eye-slash fa-eye');
    if ($(this).hasClass("fa-eye-slash")) {
        moduleSLKH = 1
        renderTableNhap()
    } else {
        moduleSLKH = 2
        renderTableNhap()
    }
})
$(document).ready(function () {
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
});