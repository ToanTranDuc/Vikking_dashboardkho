

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
$(document).ready(function () {
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
});
$("#home").on("click", function () {
    window.location.href = '/Home/Dashboard'
})
$(function () {
    $(".donhang").on("change", function () {
        renderPO()
        renderMaKho()
    })
    $(".po").on("change", function () {
        renderMaPKL()
    })
    $(".maPKL").on("change", function () {
        renderDataDongThung()
        $trMapl = "";
    })
    $("#theadTotal").on("click", ".checkAllDT", function () {
        if ($(this).prop("checked")) {
            $("#tbodyTotal").find(".checkAllDT").prop("checked", true)
        }
        else {
            $("#tbodyTotal tr").each(function () {
                let $tr = $(this);
                $tr.find(".checkAllDT").prop("checked", false)
                //if ($tr.find("td.rowDaDong").length === 0) {
                //    $tr.find("td.slNhapDT").text("");

                //}
            });
        }
    })
    $("#tbodyTotal ").on("click", ".checkAllDT", function () {
        //if ($(this).prop("checked")) {
        //    let $tr = $(this).closest("tr");
        //    let slthung = $tr.data("slthung")
        //    $tr.find("td.slNhapDT").text(slthung)
        //} else {
        //    let $tr = $(this).closest("tr");
        //    $tr.find("td.slNhapDT").text("")
        //}

    })
    $("#tbodyTotal").on("click", "tr", function () {
        tableClick($(this))
    })
    $(".saveInRowCheck").on("click", function () {
        checkTr(true, 1)

    })
    $(".clearInRowCheck").on("click", function () {
        Swal.fire({
            title: 'Bạn muốn hủy hủy tất cả số kiện?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy bỏ',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33'
        }).then((result) => {
            if (result.isConfirmed) {
                checkTr(true, 0)
            }
        });
    })
    $(".saveSL").on("click", function () {
        saveInViTri(1)
    })
    $(".clearSL").on("click", function () {
        saveInViTri(0)
    })
    $("#hoanthanh").on("click", function () {
        xacNhanHTDot(1)
    })
    $(".cancel_list").on("click", function () {
        $(".popup").hide()
    })

    $(".saveViTri").on("click", function () {
        savetheovitri = 1
        popupTheoViTri()
        $(".tieude").text("Nhận số kiện theo vị trí")
        $(".checkTotal").prop("checked", true)
    })
    $("#tbodyTotal").on("click", ".cancelRow", function () {
        let $this = $(this).closest("tr")
        tableClick($this)
        savetheovitri = 0
        $(".tieude").text("Hủy nhận số kiện theo vị trí")
        $(".checkTotal").prop("checked", true)
        popupTheoViTri()
    })
    $(".item_tuthung").on("change", function () {
        let inputvalue = $(this).val()
        checkValueMinMax(inputvalue, $(this))
    })
    $(".item_denthung").on("change", function () {
        let inputvalue = $(this).val()
        checkValueMinMax(inputvalue, $(this))
    })
    $(".item_denthung").on("input", function () {
        let inputvalue = $(this).val()
        let result = 0;
        let inputTuThung = $(".item_tuthung").val()
        if (inputTuThung.length <= 0 || inputTuThung > inputvalue) {
            result = 0
        } else {
            result = inputvalue - inputTuThung + 1

        }
        $(".display").find(".SLThung").text(result)
    })
    $(".item_tuthung").on("input", function () {
        let inputvalue = $(this).val()
        let result = 0;
        let inputTuThung = $(".item_denthung").val()
        if (inputTuThung.length <= 0 || inputTuThung < inputvalue) {
            result = 0
        } else {
            result = inputTuThung - inputvalue + 1

        }
        $(".display").find(".SLThung").text(result)
    })
    $(".save_list").on("click", function () {
        saveTheoViTri(savetheovitri)
    })

    $(" #tbodyTotal").on("click", ".slNhapDT", function () {
        let $this = $(this).closest("tr")
        tableClick($this)
        popupTheoViTri1()
        $(".popupTextarea").show()
        $(".nhapLieu").focus()
        idDThung = $(this).attr("id")
    })
    $(".cancel").on("click", function () {
        Swal.fire({
            title: 'Bạn muốn hủy nhận số kiện ?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy bỏ',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33'
        }).then((result) => {
            if (result.isConfirmed) {
                let value = $(".nhapLieu").val()
                $(`#${idDThung}`).text(value)
                $(".popupTextarea").hide()
                saveInViTri(0)
                $(".nhapLieu").val("")
                $(`#${idDThung}`).text("")
            }
        });
    })
    $("#closePopupBtn").on("click", function () {
        $(".popupTextarea").hide()
        $(".nhapLieu").val("")

    })
    $("#result").on("click", function () {
        let value = $(".nhapLieu").val()
        $(`#${idDThung}`).text(value)
        $(".popupTextarea").hide()
        saveInViTri(1)
        $(".nhapLieu").val("")
        $(`#${idDThung}`).text("")
    })
    $(".nhapLieu").on("input", function () {
        let $value = $(this).val();
        let $valuenew = "";
        if ($value > $trSLThung) {
            toastr.error(`Không được nhập vượt số lượng thùng ${$trSLThung}`)
            $valuenew = $value.slice(0, -1)
            $(this).val($valuenew)
        }

    })
})

$(".display1").on("click", ".ico-btn", function () {
    $(this).toggleClass("is-active")
    if (!$(this).hasClass("is-active")) {
        $(".display1").find(".hideIn").hide()
    }
    else {
        $(".display1").find(".hideIn").css("display", "flex")
    }
})
$(".display").on("click", ".ico-btn", function () {
    $(this).toggleClass("is-active")
    if (!$(this).hasClass("is-active")) {
        $(".display").find(".hideIn").hide()
    }
    else {
        $(".display").find(".hideIn").css("display", "flex")
    }
})
let idDThung = ""
function checkTr(flagCheck, isDongThung) {
    $(".checkAllDT").prop("checked", flagCheck)
    saveTheoRow(isDongThung)
}
function checkTatCa() {
    $("#tbodyTotal tr").each(function () {
        let $tr = $(this)
        if ($tr.find("td.rowDaDong").length > 0) {
            return;
        }
        let slthung = $tr.data("slthung")
        $tr.find("td.slNhapDT").text(slthung)
    })
}

$(document).on("keydown", function (event) {
    if (event.key === "Enter" || event.keyCode === 13) {
        event.preventDefault();
        if ($(".popupTextarea").is(":visible")) {
            let value = $(".nhapLieu").val()
            $(`#${idDThung}`).text(value)
            $(".popupTextarea").hide()
            saveInViTri(1)
            $(".nhapLieu").val("")
            $(`#${idDThung}`).text("")
        }
        if ($(".popup").is(":visible")) {
            saveTheoViTri(savetheovitri)
        }
    }
});
function renderDVSX() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetDVSXDT&para1=" + user + "&para2=A&para3=a&para4=A&para5=a&para6=A",
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
        url: `/api/NHGiaCong/Get?Action=GetDHDT&para1=${user}&para2=${_dvsx}&para3=a&para4=A&para5=a&para6=A`,
        type: 'Get',
        success: function (data) {
            $(".donhang").empty();
            $(".po").empty();
            $(".maPKL").empty();
            $(".DotSX").empty();
            $("#theadTotal").empty()
            $("#tbodyTotal").empty()
            $("#tfootTotal").empty()
            if (data.length == 0) return;
            $.each(data, function (index, item) {
                $(".donhang").append(`
                   <option data-magop="${item.MaGop}" data-madvsxht="${item.MaDVSXHoanThanh}" value="${item.MaDH}">${item.TenLenh}</option>
            `)
            })
            $(".donhang").select2()

            renderPO()
          
        }
    })
}
function renderMaKho() {
    let malenhsx = $(".dotSX ").val()
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=GetMaKho&para1=${_dvsx}&para2=${malenhsx}&para3=a&para4=A&para5=a&para6=A`,
        type: 'Get',
        success: function (data) {
            let html = ``
            data.map(item => {
                html += `
                    <option value="${item.MaKho}">${item.TenKho}</option>
                    `

            })
            $(".makho").html(html)
            $(".makho").select2()
        }
    })
}
function renderPO() {
    $.ajax({
        url: "/api/NHGiaCong/Get?Action=GetPO&para1=" + $(".dvsx").val() + "&para2=" + $(".donhang option:selected").data("magop") + "&para3=a&para4=A&para5=a&para6=A",
        type: 'Get',
        success: function (data) {
            if (data.length == 0) return;
            let html = ``
            data.map(item => {
                html += `
                       <option data-poid=${item.POID_T} value="${item.POID}">${item.PO}</option>
                `
            })
            $(".po").html(html)
            $(".po").select2()
            renderDotSX();

        }
    })
}
function renderDotSX() {
    let madh = $(".donhang option:selected").data("magop")
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=GetSS&para1=${_dvsx}&para2=${madh}&para3=a&para4=A&para5=a&para6=A`,
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                $("#theadTotal").empty()
                $("#tbodyTotal").empty()
                $("#tfootTotal").empty()
                $("#theadHistory").empty()
                $("#tbodyHistory").empty()
                $("#tfootHistory").empty()
                $(".madvsxht ").empty()
                return;
            }
            let html = ``
            data.map(item => {
                html += `
                 <option value="${item.MaLenh}">${item.DotSX}</option>
            `
            })
            $(".dotSX ").html(html)
            $(".dotSX").select2()
            renderMaPKL()
            renderDVSXHT()
            renderMaKho()
        }
    })
}
$(".dotSX").on("change", function () {
    renderMaPKL()
})
function renderMaPKL() {
    let poid = $(".po option:selected").data("poid")
    let madh = $(".donhang option:selected").data("magop")
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=GetMaPKLDT&para1=${madh}&para2=${_dvsx}&para3=a&para4=${poid}&para5=${$(".dotSX ").val()}&para6=A`,
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                $("#theadTotal").empty()
                $("#tbodyTotal").empty()
                $("#tfootTotal").empty()
                return;
            }
            let html = ``
            data.map(item => {
                html += `
                 <option value="${item.MaPKL}">${item.Display}</option>
            `
            })
            $(".maPKL ").html(html)
            $(".maPKL ").select2()
            renderDataDongThung()
        }
    })
}
function renderDataDongThung() {
    let mapkl = $(".maPKL").val()
    let poid = $(".po option:selected").data("poid")
    let madh = $(".donhang option:selected").data("magop")
    let malenhsx = $(".dotSX ").val()
    $.ajax({
        url: `/api/DongThung/GetThongTinPOCTB?mapkl=${mapkl}&madvsx=${_dvsx}&malenh=a&poid=${poid}&madh=${madh}&malenhsanxuat=${malenhsx}`,
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                $("#theadTotal").empty()
                $("#tbodyTotal").empty()
                $("#tfootTotal").empty()

                return;
            }
            let keys = Object.keys(data[0]);
            let filteredKeys = keys.filter(key => key.includes('SIZE_'));

            let $trSize = "";
            $.each(filteredKeys, (index, item) => {
                item = item.split("@")[2]
                $trSize += `
                            <td> ${item} </td>
                            `
            })
            let htmlHeader = `
                                <tr  style="position:relative">
                                    <td style="position: sticky; top: 0;display:none" rowspan="2" >SttThung</td>
                                    <td style="position: sticky; top: 0;width: 80px; display:none"rowspan="2" colspan=""> Chọn tất cả
                                    <input style="display: flex; justify-content: center; align-items: center; width: 100%;" class="checkAllDT" type="checkbox" /></td>
                                    <td style="position: sticky; top: 0;"rowspan="2" colspan="2">Carton Number</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">PO</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Đơn vị sản xuất</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Đợt SX</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">InSeam</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Color</td>
                                    <td style="position: sticky; top: 0;"colspan="${filteredKeys.length}">Size</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Qty </td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Carton </td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Total Piece</td>
                                    <td style="position: sticky; top: 0;width: 100px;"rowspan="2">Kiện đã nhập</td>
                                     <td style="position: sticky; top: 0;width: 100px;"rowspan="2">Ngày nhập kiện</td>
                                    <td  style="position: sticky; top: 0;width: 100px;"rowspan="2">Nhập số kiện</td>
                                    <td  style="position: sticky; top: 0;width: 100px"rowspan="2">Đợt nhận</td>
                                   <td  style="position: sticky; top: 0;width: 100px"rowspan="2">Hủy</td>
                                </tr>
                                 <tr >
                                   ${$trSize}
                                </tr>  `;

            $("#theadTotal").html(htmlHeader)
            let htmltbodyTotal = ``;
            let totalSLThung = 0;
            let totalTotalPiece = 0;
            let totalSoLuong = 0
            let checkSttThung = ""
            $.each(data, (index, item) => {
                let htmlTrBody = ``;
                $.each(filteredKeys, (index, lstItem) => {
                    let value = item[lstItem];
                    value === 0 ? htmlTrBody += `<td></td>` : htmlTrBody += `<td class="sizeItem" data-valuesize="${lstItem}">${value}</td>`
                })
                let slthungnew = item.SLNhap.split("/")[1]
                let sttThungnew = `${item.SttThung}${item.MinSttThung}`
                if (checkSttThung !== sttThungnew) {
                    totalSLThung += Number(slthungnew)
                    totalTotalPiece += item.SoLuong * Number(slthungnew)
                    totalSoLuong += item.SoLuong
                }
                checkSttThung = sttThungnew
                htmltbodyTotal += `
                             <tr data-slthung="${item.SLThung}" data-sttthung="${item.SttThung}" data-minstt="${item.MinSttThung}" data-maxstt="${item.MaxSttThung}" data-dotdt="${Number(item.DotDT)}"
                                   data-tuthung="${item.TuThung}" data-denthung="${item.DenThung}" data-mapkl="${item.MaPKL}" data-po="${item.PO}" data-dausize="${item.DauSize}"
                                   data-color="${item.TenMau}"
                                >
                                <td class="rowMerge mergeRow" style="display:none">${item.SttThung}</td>
                                <td style="display:none" class="checkdongthung" data-sttthung="${item.SttThung}"><input  class="checkAllDT" type="checkbox" /></td>
                                <td class="mergeRow"> ${item.TuThung}</td>
                                <td class="mergeRow">${item.DenThung}</td>
                                <td style="white-space: nowrap;">${item.PO}</td>
                                <td  style="white-space: nowrap;">${item.TenDVSX}</td>  
                                <td>${item.DotSX}</td>  
                                <td style="white-space: nowrap;">${item.DauSize}</td>
                                <td style="white-space: nowrap;">${item.TenMau}</td>
                                ${htmlTrBody}
                                <td class="rowMerge1 mergeRow" >${item.SoLuong}</td>
                                <td  class="mergeRow">${slthungnew}</td>
                                <td  class="rowMerge2 mergeRow">${item.SoLuong * Number(slthungnew)}</td>
                                 <td class="mergeRow dadongall">${item.SLNhap}</td>
                                    <td class="ngaythangnam">${item.NgayNhapKho}</td>
                                <td id="dthung${index}" data-sttthung="${item.SttThung}"  class="mergeRow slNhapDT "></td>
                                <td class="mergeRow" >${Number(item.DotDT)} </td>
                                <td class="mergeRow" ><i class="fa-sharp fa-solid fa-xmark cancelRow"></i>
                                 </td>
                             
                        </tr>
                            `
            })
            console.log(totalSLThung)
            let iconmau = `<i style="font-size: 12px;padding: 0 2px;color: #eb9e9e;" class="fa-solid fa-circle"></i>`;
            let htmltfootr = `
                       <tr>
                        <td colspan="${filteredKeys.length + 7}">Chú ý (${iconmau}) đã hoàn thành nhập</td>
                        <td>${totalSoLuong}</td>
                        <td>${totalSLThung}</td>
                        <td>${totalTotalPiece}</td>
                        <td class="slkiendanhap"></td>
                        <td colspan="3" class=""></td>
                    </tr>`

            $("#tbodyTotal").html(htmltbodyTotal)
            $("#tfootTotal").html(htmltfootr)
            MergeRowCong()
            unCheckBox()
            getHistory()
            $("#tbodyTotal tr:first").trigger("click")
            renderSLKienDaNhap();
        }
    })
}

function unCheckBox() {
    let tableLength = $("#tbodyTotal tr")
    $(tableLength).each(function () {
        let giatri = $(this).find("td.dadongall").text()
        let valueDaDong = giatri.split("/")[0]
        let valueThung = giatri.split("/")[1]
        if (valueDaDong == valueThung) {
            $(this).find(".checkAllDT").addClass("dadong");
            $(this).find(".checkdongthung ").addClass("rowDaDong")
            $(this).find(".slNhapDT ").addClass("rowDaDong")
        }
        if (valueDaDong == 0) {
            $(this).find(".ngaythangnam").text("")
        }
    })
}
//check value
function checkValueMinMax(input, $this) {
    let valueDenthung = $(".item_denthung").val()
    let valueTuThung = $(".item_tuthung").val()
    if (input > $trDenthung) {
        toastr.error(`Vui lòng nhập giá trị từ thùng nhỏ hơn ${$trDenthung}`)
        $this.val("")
        return;
    }
    else if (input < $trTuThung) {
        toastr.error(`Vui lòng nhập giá trị từ thùng nhỏ hơn ${$trTuThung}`)
        $this.val("")
        return
    }
    if ($this.is(".item_tuthung") && Number(input) > Number(valueDenthung) && valueDenthung != "") {
        toastr.error("Vui lòng nhập giá trị từ thùng nhỏ hơn giá trị đến thùng")
        $this.val("")
        return;
    }
    if ($this.is(".item_denthung") && Number(input) < Number(valueTuThung) && valueTuThung != "") {
        toastr.error("Vui lòng nhập giá trị đến thùng lớn hơn giá trị từ thùng")
        $this.val("")
        return;
    }
}
// DT theo vị trí
let $trMapl = "";
let $trPo = "";
let $trDauSize = "";
let $trColor = "";
let $trkhoanViTri = "";
let $trTuThung = ""
let $trDenthung = ""
let $trSttMin = ""
let $trSttMax = ""
let savetheovitri = "0"
let $trSLThung = ""
let $trSizeNew = ""
let $trColorNew = ""
let $trDauSizeNew = ""
let $trEq = -1
let $SLDaDong = 0
let $SLChuaDong = 0;
function tableClick($this) {
    let $tr = $("#tbodyTotal tr");
    $tr.removeClass("active1");
    $this.addClass("active1");
    $trEq = $this.index()
    $trMapl = $this.data("mapkl");
    $trPo = $this.data("po");
    $trDauSize = $this.data("dausize");
    $trColor = $this.data("color");
    $trTuThung = $this.data("tuthung");
    $trDenthung = $this.data("denthung");
    $trSttMin = $this.data("minstt");
    $trSttMax = $this.data("maxstt");
    $trkhoanViTri = `${$trTuThung} -> ${$trDenthung}`;
    $trSLThung = $this.data("slthung");
    stthungThis = $this.data("sttthung");

    let objectSize = [];
    let objectDauSize = [];
    let objectColor = [];

    $tr.each(function () {
        let trThisSttThung = $(this).data("sttthung");
        if (trThisSttThung != stthungThis) return;

        let dauSizeValue = $(this).data("dausize");
        let colorValue = $(this).data("color");

        if (dauSizeValue) {
            objectDauSize.push(dauSizeValue);
        }

        if (colorValue) {
            objectColor.push(colorValue);
        }

        $(this).find(".sizeItem").each(function () {
            let sizeValue = $(this).data("valuesize");
            if (sizeValue) {
                sizeValue = sizeValue.split("@")[2]
                objectSize.push(sizeValue);
            }
        });
        let giatri = $(this).find("td.dadongall").text()
        let valueDaDong = giatri.split("/")[0]
        let valueThung = giatri.split("/")[1]
        $SLDaDong = valueDaDong
        $SLChuaDong = valueThung - valueDaDong

    });
    objectSize = [... new Set(objectSize)]
    $trSizeNew = objectSize.join(",");
    objectColor = [... new Set(objectColor)]
    $trColorNew = objectColor.join(",");
    objectDauSize = [... new Set(objectDauSize)]
    $trDauSizeNew = objectDauSize.join(",");
}

$(".arrow_right").on("click", function () {
    moveRowTable(1)
    popupTheoViTri()

})
$(".arrow_left").on("click", function () {
    moveRowTable(0)
    popupTheoViTri()
})
$(".arrow_down").on("click", function () {
    moveRowTable(1)
    popupTheoViTri1()

})
$(".arrow_up").on("click", function () {
    moveRowTable(0)
    popupTheoViTri1()
})
function eachtrEq() {
    let indexStt = 0;
    const stthungeq = $("#tbodyTotal tr").eq($trEq).data("sttthung");

    $("#tbodyTotal tr").each(function () {
        let stthung = $(this).data("sttthung");
        if (stthung == stthungeq) {
            indexStt++;
        }
    });
    return indexStt;
}

function moveRowTable(value) {
    $("#tbodyTotal tr").removeClass("active1");
    let eq;
    if (value == 1) {
        eq = $trEq + eachtrEq();
    } else {
        if ($trEq != 0) {
            eq = $trEq - eachtrEq();
        }
    }

    if (eq >= 0 && eq < $("#tbodyTotal tr").length) {
        $("#tbodyTotal tr").eq(eq).addClass("active1");
        $trEq = eq;
    }
    tableClick($("#tbodyTotal tr").eq($trEq))
    $(".checkTotal").prop("checked", true)
}
$(".checkTotal").on("change", function () {
    if ($(this).prop("checked")) {
        $(".item_tuthung").val($trTuThung)
        $(".item_denthung").val($trDenthung)
        let soluongthung = Number($trDenthung - $trTuThung + 1)
        $(".display").find(".SLThung").text(soluongthung)
    }
    else {
        $(".item_tuthung").val("")
        $(".item_denthung").val("")
        $(".display").find(".SLThung").text(0)
    }
})
let $trRangeTrue = ""
let $trRangeFalse = ""
async function popupTheoViTri() {
    await setThungChuaDaNhap();
    if ($trMapl == "") {
        toastr.error("Vui lòng chọn dòng bạn muốn nhận")
        return
    }
    $(".popup").show()
    let html = `
                 <div class="list_SizeColor">
                     <div class="item_size">
                            <p st>Mã PKL: <span>${$trMapl}</span></p>
                        </div>
                        <div class="item_size">
                            <p>PO: <span> ${$trPo} </span></p>
                        </div>
                    <div class="item_size">
                            <p>Size: <span> ${$trSizeNew} </span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor">

                        <div class="item_size">
                            <p>InSeam: <span> ${$trDauSizeNew} </span></p>
                        </div>
                        <div class="item_size">
                            <p>Color: <span> ${$trColorNew} </span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor">
                        <div class="item_size">
                            <p>Khoản vị trí: <span> ${$trkhoanViTri} </span></p>
                        </div>
                         <div class="item_size">
                            <p>SLThung: <span class="SLThung"></span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor">
                        <div class="ico-btn">
                          <span class="ico-btn__plus"></span>
                        </div>
                    </div>
                     <div class="list_SizeColor hideIn">
                        <div class="item_size">
                            <p> Các thùng đã nhập: <span> ${$trRangeTrue} </span></p>
                        </div>
                        <div class="item_size">
                            <p> SLN: <span> ${$SLDaDong} </span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor hideIn">
                         <div class="item_size">
                            <p>Các thùng chưa nhập: <span >${$trRangeFalse}</span></p>
                        </div>
                            <div class="item_size">
                            <p> SLCN: <span> ${$SLChuaDong} </span></p>
                        </div>
                    </div>
    `
    $(".display").html(html)
    $(".item_tuthung").val($trTuThung)
    $(".item_denthung").val($trDenthung)
    let soluongthung = Number($trDenthung - $trTuThung + 1)
    $(".display").find(".SLThung").text(soluongthung)
}
async function setThungChuaDaNhap() {
    let poid = $(".po option:selected").data("poid");
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX").val();
    let madvsx = _dvsx;

    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/api/NHGiaCong/Get?Action=GetSLDongThung&para1=${$trMapl}&para2=${madvsx}&para3=${madh}&para4=${poid}&para5=${$trSttMin}&para6=${$trSttMax}&para7=${malenhsx}`,
            type: 'GET',
            success: function (data) {
                let grouped = groupByIsDongThung(data);
                let trueRanges = extractRanges(grouped, true);
                let falseRanges = extractRanges(grouped, false);
                $trRangeTrue = trueRanges.join(", ");
                $trRangeFalse = falseRanges.join(", ");
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}
function extractRanges(groupedArray, isDongThungValue) {
    let ranges = [];

    groupedArray.forEach(group => {
        if (group.isDongThung === isDongThungValue) {
            ranges.push(`${group.start} -> ${group.end}`);
        }
    });
    return ranges;
}
function groupByIsDongThung(arr) {
    let result = [];
    let currentGroup = { isDongThung: arr[0].IsDongThung, start: arr[0].SttThung, end: arr[0].SttThung };

    for (let i = 1; i < arr.length; i++) {
        if (arr[i].IsDongThung === currentGroup.isDongThung) {
            currentGroup.end = arr[i].SttThung;
        } else {
            result.push(currentGroup);
            currentGroup = { isDongThung: arr[i].IsDongThung, start: arr[i].SttThung, end: arr[i].SttThung };
        }
    }
    result.push(currentGroup);

    return result;
}
async function popupTheoViTri1() {
    await setThungChuaDaNhap();
    let html = `
                 <div class="list_SizeColor">
                     <div class="item_size">
                            <p st>Mã PKL: <span>${$trMapl}</span></p>
                        </div>
                        <div class="item_size">
                            <p>PO: <span> ${$trPo} </span></p>
                        </div>
                    <div class="item_size">
                            <p>Size: <span> ${$trSizeNew} </span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor">

                        <div class="item_size">
                            <p>InSeam: <span> ${$trDauSizeNew} </span></p>
                        </div>
                        <div class="item_size">
                            <p>Color: <span> ${$trColorNew} </span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor">
                        <div class="item_size">
                            <p>Khoản vị trí: <span> ${$trkhoanViTri} </span></p>
                        </div>
                       <div class="item_size">
                            <p>SLThung: <span class="SLThung1">${$trSLThung}</span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor">
                        <div class="ico-btn">
                          <span class="ico-btn__plus"></span>
                        </div>
                    </div>
                        
                      <div class="list_SizeColor hideIn ">
                        <div class="item_size">
                            <p> Các thùng đã nhập: <span> ${$trRangeTrue} </span></p>
                        </div>
                        <div class="item_size">
                            <p> SLN: <span> ${$SLDaDong} </span></p>
                        </div>
                    </div>
                    <div class="list_SizeColor hideIn">
                         <div class="item_size">
                            <p>Các thùng chưa nhập: <span >${$trRangeFalse}</span></p>
                        </div>
                            <div class="item_size">
                            <p> SLCN: <span> ${$SLChuaDong} </span></p>
                        </div>
                    </div>
    `
    $(".display1").html(html)

}
function saveTheoViTri(isDongThung) {
    let arrSaveTheoViTri = []
    let tuthung = $trTuThung
    let denthung = $trDenthung
    let minthung = $trSttMin
    let maxthung = $trSttMax

    let tuthungvalue = $(".item_tuthung").val()
    let denthungvalue = $(".item_denthung").val()
    if (tuthungvalue == "" || denthungvalue == "") return
    let tinhSttThungMin = minthung + (tuthungvalue - tuthung)
    let tinhStthungMax = maxthung - (denthung - denthungvalue)
    let mapkl = $(".maPKL").val();
    let poid = $(".po option:selected").data("poid");
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX ").val();
    let madvsx = _dvsx;
    let makho = $(".makho").val()
    let object = {
        MaDH: madh,
        MaLenh: malenhsx,
        MaDVSX: madvsx,
        POID: poid,
        MaPKL: mapkl,
        MinSttThung: tinhSttThungMin,
        MaxSttThung: tinhStthungMax,
        TuThung: 10000000,
        DenThung: isDongThung,
        MaKho: makho
    }
    arrSaveTheoViTri.push(object)
    postApiSave(arrSaveTheoViTri, isDongThung)
}

// Đóng thùng theo dòng chọn
function saveTheoRow(isDongThung) {
    let mapkl = $(".maPKL").val()
    let poid = $(".po option:selected").data("poid")
    let madh = $(".donhang option:selected").data("magop")
    let malenhsx = $(".dotSX ").val()
    let madvsx = _dvsx
    let arrSaveInRow = []
    let tableLength = $("#tbodyTotal tr")
    let sttthungCheck = "";
    let makho =  $(".makho").val() 
    $(tableLength).each(function () {
        let $tr = $(this)
        let ifDieuKien = $tr.find(".checkAllDT").prop("checked")
        let stthung = $tr.data("sttthung")
        if (ifDieuKien && sttthungCheck != stthung) {
            let stthungmin = $tr.data("minstt")
            let stthungmax = $tr.data("maxstt")
            let object = {
                MaDH: madh,
                MaLenh: malenhsx,
                MaDVSX: madvsx,
                POID: poid,
                MaPKL: mapkl,
                MinSttThung: stthungmin,
                MaxSttThung: stthungmax,
                TuThung: 10000000,
                DenThung: isDongThung,
                MaKho: makho
            }
            arrSaveInRow.push(object)
        }
        sttthungCheck = stthung
    })
    if (arrSaveInRow.length > 0)
        postApiSave(arrSaveInRow, isDongThung)
}

let arrDataInVitri = [];

//DT Theo soluong
async function saveInViTri(isDongThung) {
    let tableLength = $("#tbodyTotal tr");
    let sttthungCheck = "";
    arrDataInVitri = [];

    for (let i = 0; i < tableLength.length; i++) {
        let $tr = $(tableLength[i]);
        let slNhap = $tr.find(".slNhapDT").text();
        let stthung = $tr.data("sttthung");

        if (sttthungCheck !== stthung && slNhap !== "" && !isNaN(slNhap) && parseInt(slNhap) > 0) {
            let stthungmin = $tr.data("minstt");
            let stthungmax = $tr.data("maxstt");
            await addArrSaveViTri(stthungmin, stthungmax, slNhap, isDongThung);
        }
        sttthungCheck = stthung;
    }
    console.log(isDongThung)
    if (arrDataInVitri.length > 0)
        postApiSave(arrDataInVitri, isDongThung)
}
async function addArrSaveViTri(stthungmin, stthungmax, slnhap, isDongThung) {
    let mapkl = $(".maPKL").val();
    let poid = $(".po option:selected").data("poid");
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX ").val();
    let madvsx = _dvsx;

    try {
        let response = await $.ajax({
            url: `/api/NHGiaCong/Get?Action=GetSLDongThung&para1=${mapkl}&para2=${madvsx}&para3=${madh}&para4=${poid}&para5=${stthungmin}&para6=${stthungmax}&para7=${malenhsx}`,
            type: 'GET'
        });
        let checkDT = isDongThung == 1 ? false : true;
        let makho = $(".makho").val() 
        let index = 0; // Initialize index if needed
        if (isDongThung != 1) {
            for (let i = response.length - 1; i >= 0; i--) {
                let item = response[i];
                if (item.IsDongThung === checkDT && slnhap > index && item.DotDT == "0") {
                    let object = {
                        MaDH: madh,
                        MaLenh: malenhsx,
                        MaDVSX: madvsx,
                        POID: poid,
                        MaPKL: mapkl,
                        MinSttThung: item.SttThung,
                        MaxSttThung: item.SttThung,
                        TuThung: 10000000,
                        DenThung: isDongThung,
                        MaKho: makho
                    };
                    index++;
                    arrDataInVitri.push(object);
                }
            }
            if (arrDataInVitri.length == 0) {
                Swal.fire({
                    title: 'Đã hoàn thành nhập không được hủy!',
                    icon: 'error',
                    showConfirmButton: false,
                    timer: 2000, // Thời gian hiển thị thông báo (1500 ms = 1.5 giây)
                    timerProgressBar: true, // Hiển thị thanh tiến trình của timer
                    allowOutsideClick: false,
                    backdrop: true,
                    didOpen: () => {
                        Swal.showLoading(); // Hiển thị thanh tiến trình khi thông báo đang được hiển thị
                    }
                });
            }
        } else {
            for (let i = 0; i < response.length; i++) {
                let item = response[i];
                if (item.IsDongThung === checkDT && slnhap > index) {
                    let object = {
                        MaDH: madh,
                        MaLenh: malenhsx,
                        MaDVSX: madvsx,
                        POID: poid,
                        MaPKL: mapkl,
                        MinSttThung: item.SttThung,
                        MaxSttThung: item.SttThung,
                        TuThung: 10000000,
                        DenThung: isDongThung,
                        MaKho: makho
                    };
                    index++;
                    arrDataInVitri.push(object);
                }
            }
        }
    } catch (error) {

    }
}
// Hoàn thành đợt
let arrDataIsHoanThanhDot = [];
async function xacNhanHTDot() {
    let tableLength = $("#tbodyTotal tr");
    let sttthungCheck = "";
    let dotCheck = ""
    arrDataIsHoanThanhDot = [];

    for (let i = 0; i < tableLength.length; i++) {
        let $tr = $(tableLength[i]);
        let stthung = $tr.data("sttthung");
        let dot = $tr.data("dotdt");
        if (`${sttthungCheck}${dotCheck}` !== `${stthung}${dot}` ) {
            let stthungmin = $tr.data("minstt");
            let stthungmax = $tr.data("maxstt");
            let dotdt = $tr.data("dotdt");
            console.log(dotdt)
            await addArrSaveIsHTDot(stthungmin, stthungmax, dotdt);
        }
        sttthungCheck = stthung;
        dotCheck = dot
    }
    if (arrDataIsHoanThanhDot.length > 0) {
        Swal.fire({
            title: 'Bạn muốn xác nhận hoàn thành đợt này không?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy bỏ',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33'
        }).then((result) => {
            if (result.isConfirmed) {
                postApiSave(arrDataIsHoanThanhDot)
            }
        });
    }

}
async function addArrSaveIsHTDot(stthungmin, stthungmax, Dot) {
    let mapkl = $(".maPKL").val();
    let poid = $(".po option:selected").data("poid");
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX ").val();
    let madvsx = _dvsx;

    try {
        let response = await $.ajax({
            url: `/api/NHGiaCong/Get?Action=GetSLDongThung&para1=${mapkl}&para2=${madvsx}&para3=${madh}&para4=${poid}&para5=${stthungmin}&para6=${stthungmax}&para7=${malenhsx}`,
            type: 'GET'
        });
        response.map(item => {
            if (item.IsDongThung == true && item.DotDT == 0) {
                let object = {
                    MaDH: madh,
                    MaLenh: malenhsx,   
                    MaDVSX: madvsx,
                    POID: poid,
                    MaPKL: mapkl,
                    MinSttThung: item.SttThung,
                    MaxSttThung: item.SttThung,
                    TuThung: Dot,
                    DenThung: 1,
                    MaKho: $(".makho").val()
                };
                arrDataIsHoanThanhDot.push(object);
            }
        });
    } catch (error) {
    }
}

//Post All
$(".dateNhapKho").on("click", function () {
    let updatedData = dataXN.map(obj => {
        return { ...obj, NgayNhapKho: fomatDateTime($(".datepicker3").val()) };
    });
    saveNhanHangKien(updatedData)
})
function fomatDateTime(datetime) {
    const date = datetime.split("-")
    return `${date[2].trim()}-${date[1].trim()}-${date[0].trim()}`
}
let dataXN = []
function postApiSave(arrSaveInRow, value) {
    if (value == 1) {
        $(".popup4").show()
       getDate()
        dataXN = arrSaveInRow
    }
    else {
        let updatedData = arrSaveInRow.map(obj => {
            return { ...obj, NgayNhapKho: fomatDateTime($(".datepicker3").val()) };
        });
        saveNhanHangKien(updatedData)
    }
}
function saveNhanHangKien(arrSaveInRow) {
    $.ajax({
        type: "POST",
        url: "/api/NHGiaCong/PostDT",
        data: JSON.stringify(arrSaveInRow),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            toastr.success("Lưu thành công")
            renderDataDongThung()
            $(".popup").hide()
            $(".popup4").hide()
        }
    })
}
$(".list_tabIndex").on("click", "button", function () {
    $(".list_tabIndex").find("button").removeClass("active")
    $(this).addClass("active")
    var tab_id = $(this).data('tab');
    $('.tab-content').removeClass('active');
    $(`#${tab_id}`).addClass('active');
    if (tab_id == "tab-2") {
        $(".btn_list").addClass("active")
        $(".nav_dongthung").addClass("active")
    }
    else {
        $(".btn_list").removeClass("active")
        $(".nav_dongthung").removeClass("active")
    }
})
function getHistory() {
    let mapkl = $(".maPKL").val();
    let poid = $(".po option:selected").data("poid");
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX ").val();
    let madvsx = _dvsx;
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=GetHistory&para1=${mapkl}&para2=${madh}&para3=${madvsx}&para4=${malenhsx}&para5=${poid}&para6=a&para7=a`,
        type: 'Get',
        success: function (data) {
            data = data.filter(item => item.DotDT > 0 && item.DotDT != null)
            if (data.length == 0) {
                $("#tbodyTong").empty()
                $("#tfootTong").empty()
                $("#theadHistory").empty()
                $("#tbodyHistory").empty()
                $("#tfootHistory").empty()
                return
            }
            let html = ``
            let totalSLThung = data.reduce((acc, curr) => acc + parseInt(curr.carton), 0);
            let totalNhap = data.reduce((acc, curr) => acc + parseInt(curr.SLN), 0);
            data.map(item => {
                html += `
                    <tr data-dotdt="${item.DotDT}">
                      <td>${item.NgayXNDotDT}</td>
                      <td>${item.TenHang}</td>
                      <td>${item.TenKH}</td>
                      <td>${item.PO}</td>
                      <td>${item.TenMau}</td>
                      <td>${item.DauSize}</td>
                      <td>${item.carton}</td>
                      <td>${item.SLN} </td>
                      <td>${item.DotDT} </td>
                    <tr/>
                `

            })
            $("#tbodyTong").html(html)
            let htmltf = `
                    <tr>
                     <td colspan="6"></td>
                      <td>${totalSLThung}</td>
                      <td>${totalNhap} </td>
                    <tr/>
                `
            $("#tfootTong").html(htmltf)
            $("#tbodyTong tr:first").trigger("click")
        }
    })

}
let $trDotDt = ""
$("#tbodyTong").on("click", "tr", function () {
    $("#tbodyTong tr").removeClass("active7")
    $(this).addClass("active7")
    $trDotDt = $(this).data("dotdt")
    renderDataHistoryDetal()
})
function renderDataHistoryDetal() {
    let mapkl = $(".maPKL").val()
    let poid = $(".po option:selected").data("poid")
    let madh = $(".donhang option:selected").data("magop")
    let malenhsx = $(".dotSX ").val()
    let madvsx = _dvsx;
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=GetHistoryDetail&para1=${mapkl}&para2=${madh}&para3=${madvsx}&para4=${malenhsx}&para5=${poid}&para6=${$trDotDt}&para7=a`,
        type: 'Get',
        success: function (data) {
            if (data.length == 0) {
                $("#theadHistory").empty()
                $("#tbodyHistory").empty()
                $("#tfootHistory").empty()
                return;
            }
            let keys = Object.keys(data[0]);
            let filteredKeys = keys.filter(key => key.includes('SIZE_'));

            let uniqueValues = data.map(x => ({
                MaPKL: x.MaPKLDisplay,
                SttThung: x.SttThung,
                SLThung: x.SLThung,
                SoLuong: x.SoLuong,
                TotalPiece: x.TotalPiece,
            }))
            uniqueValues = uniqueValues.filter((value, index, self) =>
                index === self.findIndex(obj =>
                    obj.MaPKL === value.MaPKL &&
                    obj.SttThung === value.SttThung
                )
            );
            let totalSLThung = uniqueValues.reduce((acc, curr) => acc + parseInt(curr.SLThung), 0);
            let totalTotalPiece = data.reduce((acc, curr) => acc + parseInt(curr.TotalPiece), 0);
            let totalSoLuong = data.reduce((acc, curr) => acc + parseInt(curr.SoLuong), 0);
            let $trSize = "";
            $.each(filteredKeys, (index, item) => {
                item = item.split("@")[2]
                $trSize += `
                            <td> ${item} </td>
                            `
            })
            let htmlHeader = `
                                <tr  style="position:relative">
                                    <td style="position: sticky; top: 0;display:none" rowspan="2" >SttThung</td>
                                    <td style="position: sticky; top: 0;width: 80px; display:none"rowspan="2" colspan=""> Chọn tất cả
                                    <input style="display: flex; justify-content: center; align-items: center; width: 100%;" class="checkAllDT" type="checkbox" /></td>
                                    <td style="position: sticky; top: 0;"rowspan="2" colspan="2">Carton Number</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">PO</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Đơn vị sản xuất</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Đợt SX</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">InSeam</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Color</td>
                                    <td style="position: sticky; top: 0;"colspan="${filteredKeys.length}">Size</td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Qty </td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Carton </td>
                                    <td style="position: sticky; top: 0;"rowspan="2">Total Piece</td>
                                    <td style="position: sticky; top: 0;width: 100px;"rowspan="2">Kiện đã nhập</td>
                                    <td  style="position: sticky; top: 0;width: 100px"rowspan="2">Đợt nhận</td>
                                </tr>
                                 <tr >
                                   ${$trSize}
                                </tr>  `;

            $("#theadHistory").html(htmlHeader)
            let htmltbodyTotal = ``;
            $.each(data, (index, item) => {
                let htmlTrBody = ``;
                $.each(filteredKeys, (index, lstItem) => {
                    let value = item[lstItem];
                    value === 0 ? htmlTrBody += `<td></td>` : htmlTrBody += `<td class="sizeItem" data-valuesize="${lstItem}">${value}</td>`
                })
                let eye = `<div> <i style="font-size: 11px; cursor: pointer; border: 1px solid; padding: 0 2px;
                    background: #fff; border-radius: 5px; outline: none; margin-top:0px;" class="fa-solid fa-eye-slash eye-hideSL"></i>
                    </div>`
                htmltbodyTotal += `
                             <tr data-slthung="${item.SLThung}" data-sttthung="${item.SttThung}" data-minstt="${item.MinSttThung}" data-maxstt="${item.MaxSttThung}" data-dotdt="${Number(item.DotDT) + 1}"
                                   data-tuthung="${item.TuThung}" data-denthung="${item.DenThung}" data-mapkl="${item.MaPKL}" data-po="${item.PO}" data-dausize="${item.DauSize}"
                                   data-color="${item.TenMau}"
                                >
                                <td class="rowMerge mergeRow" style="display:none">${item.SttThung}</td>
                                <td style="display:none" class="checkdongthung" data-sttthung="${item.SttThung}"><input  class="checkAllDT" type="checkbox" /></td>
                                <td class="mergeRow"> ${item.TuThung}</td>
                                <td class="mergeRow">${item.DenThung}</td>
                                <td style="white-space: nowrap;">${item.PO}</td>
                                <td  style="white-space: nowrap;">${item.TenDVSX}</td>  
                                <td>${item.DotSX}</td>
                                <td style="white-space: nowrap;">${item.DauSize}</td>
                                <td style="white-space: nowrap;">${item.TenMau}</td>
                                ${htmlTrBody}
                                <td class="rowMerge1 mergeRow" >${item.SoLuong}</td>
                                <td  class="mergeRow">${item.SLThung}</td>
                                <td  class="rowMerge2 mergeRow">${item.TotalPiece}</td>
                                 <td class="mergeRow dadongall">
                                    <div style="display: flex;justify-content: space-between;align-items: center;margin: 0 4px;column-gap: 2px;">
                                    ${item.SLNhap}
                                    ${eye}
                                    </td>
                                <td class="mergeRow" >${Number(item.DotDT)} </td>
                             
                        </tr>
                            `

            })
            let iconmau = `<i style="font-size: 12px;padding: 0 2px;color: #eb9e9e;" class="fa-solid fa-circle"></i>`;
            let htmltfootr = `
                       <tr>
                        <td colspan="${filteredKeys.length + 7}">Chú ý (${iconmau}) đã hoàn thành nhập</td>
                        <td>${totalSoLuong}</td>
                        <td>${totalSLThung}</td>
                        <td>${totalTotalPiece}</td>
                        <td class="slkiendanhaphistory"></td>
                        <td colspan="2" class=""></td>
                    </tr>`

            $("#tbodyHistory").html(htmltbodyTotal)
            $("#tfootHistory").html(htmltfootr)
            MergeRowCong()
            renderSLKienDaNhapHistory()
        }
    })
}
function renderSLKienDaNhapHistory() {
    let giatriHistory = 0
    $("#tbodyHistory tr").each(function () {
        let giatri = $(this).find("td.dadongall").text()
        let valueDaDong = giatri.split("/")[0]
        giatriHistory += Number(valueDaDong)
    })
    $("#tfootHistory").find(".slkiendanhaphistory").text(giatriHistory)
}
function renderSLKienDaNhap() {
    let giatriDaDong = 0
    $("#tbodyTotal tr").each(function () {
        if ($(this).find(".mergeRow").is(":visible")) {
            let giatri = $(this).find("td.dadongall").text()
            let valueDaDong = giatri.split("/")[0]
            giatriDaDong += Number(valueDaDong)
        }

    })
    $("#tfootTotal").find(".slkiendanhap").text(giatriDaDong)
}
$("#tbodyHistory").on("click", ".eye-hideSL", function () {
    $(this).removeClass('fa-eye-slash');
    $(this).addClass("fa-eye")
    let $this = $(this).closest("tr")

    let $trMapl = $this.data("mapkl");
    let $trPo = $this.data("po");
    let $trDauSize = $this.data("dausize");
    let $trColor = $this.data("color");
    let $trTuThung = $this.data("tuthung");
    let $trDenthung = $this.data("denthung");
    let $trSttMin = $this.data("minstt");
    let $trSttMax = $this.data("maxstt");
    let $trSLThung = $this.data("slthung");
    let stthungThis = $this.data("sttthung");

    let poid = $(".po option:selected").data("poid");
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX").val();
    let madvsx = _dvsx;
    let $trRangeTrueNew = ""
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=GetSLDongThung&para1=${$trMapl}&para2=${madvsx}&para3=${madh}&para4=${poid}&para5=${$trSttMin}&para6=${$trSttMax}&para7=${malenhsx}`,
        type: 'Get',
        success: function (data) {
            let grouped = groupByIsDongThung(data);
            let trueRanges = extractRanges(grouped, true);
            $trRangeTrueNew = trueRanges.join(", ")
            $("#myModal").modal('show');
            let html = `
                <p class="">${$trRangeTrueNew}</p>
                `
            $(".item_sizeDN").html(html)
        }
    })


})
$("#tbodyHistory").on("click", "tr", function () {
    let $this = $(this)
    $("#tbodyHistory tr").removeClass("active1");
    $this.addClass("active1");

})
$('#myModal').on('hidden.bs.modal', function () {
    $("#tbodyHistory").find(".eye-hideSL").removeClass('fa-eye');
    $("#tbodyHistory").find(".eye-hideSL").addClass("fa-eye-slash")
});

//Merge table
function MergeRow() {
    let tableLength = $("#tbodyTotal tr").length;

    for (let i = 0; i < tableLength; i++) {
        let merge = 1;
        let valueMerge = $("#tbodyTotal tr").eq(i).find(".mergeRow").text();
        for (let j = i + 1; j < tableLength; j++) {
            let valueMergeNext = $("#tbodyTotal tr").eq(j).find(".mergeRow").text();
            if (valueMerge === valueMergeNext) {
                merge++;
            } else {
                break;
            }
        }
        $("#tbodyTotal tr").eq(i).find(".mergeRow, td:eq(1)").attr("rowspan", merge);
        for (let k = 1; k < merge; k++) {
            $("#tbodyTotal tr").eq(i + k).find(".mergeRow, td:eq(1)").hide();
        }
        i += merge - 1;
    }
}
function MergeRowCong() {
    let tableLength = $("#tbodyTotal tr").length;

    for (let i = 0; i < tableLength; i++) {
        let merge = 1;
        let minsthung = $("#tbodyTotal tr").eq(i).data("minstt")
        let valueMerge = $("#tbodyTotal tr").eq(i).find(".rowMerge").text();
        let totalValue = parseInt($("#tbodyTotal tr").eq(i).find(".rowMerge1").text(), 10);
        let totalValuePrice = parseInt($("#tbodyTotal tr").eq(i).find(".rowMerge2").text(), 10);
        for (let j = i + 1; j < tableLength; j++) {
            let valueMergeNext = $("#tbodyTotal tr").eq(j).find(".rowMerge").text();
            let minsthungNext = $("#tbodyTotal tr").eq(j).data("minstt")
            let valueCong = parseInt($("#tbodyTotal tr").eq(j).find(".rowMerge1").text(), 10);
            let valueCongPrice = parseInt($("#tbodyTotal tr").eq(j).find(".rowMerge2").text(), 10);
            if (valueMerge === valueMergeNext && minsthungNext == minsthung) {
                merge++;
                totalValue += valueCong;
                totalValuePrice += valueCongPrice;
                $("#tbodyTotal tr").eq(i).find(".rowMerge1").text(totalValue);
                $("#tbodyTotal tr").eq(j).find(".rowMerge1").text(totalValue);
                $("#tbodyTotal tr").eq(i).find(".rowMerge2").text(totalValuePrice);
                $("#tbodyTotal tr").eq(j).find(".rowMerge2").text(totalValuePrice);
            } else {
                break;
            }
        }
        i += merge - 1;

    }
    MergeRow()
}
//Ke
function renderTenKhuVuc() {
    let dvsx = $madvsx
    let tenkho = $("#slc_kho").val()
    let MaKH = $("#DH_PKL").val().split('@PKL@')[3];
    $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetDayInKH&Para1=${dvsx}&Para2=${tenkho}&Para3=${MaKH}&Para4=a&Para5=a&Para6=a`,
        type: 'Get',
        success: function (data) {
            let html = ``
            data.map((item, index) => {
                let cbm = Math.round(item.CBM * 1000) / 1000;
                html += `
                      <li class="result-row ${item.CheckStatus == 1 ? 'active8' : ''}" data-value=${item.MaDay} data-name="${item.NameDay}">
                        <div class="result-column">${item.NameDay} </div>
                        <div class="result-column">${cbm}</div>
                        <div style="width: 250px;" class="result-column">${item.TenKH}</div>
                    </li>
                `
            })
            $(".lookup-results").html(html)
            $(".lookup-results li:first").trigger("click")
        }
    })
}
$(".hide_popup1").on("click", function () {
    $(".popup4 ").hide()
})
//renderTenKhuVuc()
async function renderKeNew() {
    try {

        let htmlTable = "";
        let dvsx = $madvsx;
        let tenkho = $("#slc_kho").val();

        const data = await $.ajax({
            url: `/api/ViTriKhoNew/Get?Action=GetKe&Para1=${dvsx}&Para2=${tenkho}&Para3=${$trMaDay}&Para4=a&Para5=a&Para6=a`,
            type: 'GET'
        });

        if (data.length === 0) {
            $(".rendertable_list").empty();
            removeLoading()
            alertError("Dãy này chưa được khai báo")

            return;
        }

        let htmlTotal = "";

        for (const item of data) {
            let make = item.MaKe;
            let tenke = item.NameKe;
            let htmlKe = "";


            const dataTang = await renderTang1($trMaDay, make);
            if (dataTang.length === 0) continue;

            let htmlTang = "";

            for (const tang of dataTang) {
                let matang = tang.MaTang;
                let tenTang = tang.NameTang;
                const dataO = await renderO1($trMaDay, make, matang);

                let htmlO = "";
                for (const o of dataO) {

                    let phantram = ((o.CBM - o.CBMTotal) / o.CBM) * 100;
                    let iconmau = "";
                    let cbmO = Math.round(o.CBMTotal * 1000) / 1000
                    let valueo = `<p style="margin-top:2px;white-space: nowrap;margin-left: 5px;">${o.NameO}</p>`
                    let chuyenIndex = `<i class="fa-solid fa-arrow-right arrowIndex"></i>`
                    if (100 - phantram <= 0) {
                        iconmau = `
                           ${chuyenIndex} <div class="list_p"> <i style="  font-size: 12px; padding: 0 2px; color:" class="fa-regular fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p></div>`;
                    } else if (100 - phantram > 80) {
                        iconmau = `
                           ${chuyenIndex} <div class="list_p"> <i style=" font-size: 12px; padding: 0 2px; color:#00FF00" class="fa-solid fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p></div>`;
                    } else if (100 - phantram >= 50 && 100 - phantram <= 80) {
                        iconmau = ` 
                                   ${chuyenIndex} <div class="list_p"><i style=" font-size: 12px; padding: 0 2px;color:#F6B26B" class="fa-solid fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p> </div>`;
                    } else if (100 - phantram < 50) {
                        iconmau = ` 
                                   ${chuyenIndex} <div class="list_p"><i style=" font-size: 12px; padding: 0 2px;color: #FF0000" class="fa-solid fa-circle"></i><p class="CBMTD">(${cbmO}/${o.CBM})</p></div> `;
                    }

                    htmlO += `<td data-cbmcl="${cbmO}" data-make="${make}" data-o="${o.MaO}" data-khoiluong="${o.CBM}" data-tang="${matang}" data-teno="${o.NameO}">
                        <div class="list_item">
                           <div class="item_Icon">
                              ${iconmau}
                           </div>
                       <div style="display: flex;justify-content: center;align-items: center;height: 100%;">
                            <div style="padding: 0 5px;" class="">
                              ${valueo}
                            </div>
                            </div>
                            </div>
                        </div>
                          
                         </td>`;
                }

                htmlKe = `<td class="rowMerge" style="white-space: ; max-width: 16px; min-width: 30px;background: #a8f6ff;position: sticky; left: -3px;">
                    ${tenke}</td>`;
                htmlTang += `
                    <tr>
                    ${htmlKe}
                    ${htmlO}</tr>`;
            }

            let htmlTangFomat = fomatRenderTang(htmlTang);
            htmlTotal += `
                    <table class="tbody1">
                        ${htmlTangFomat}
                       </table>
                `
                ;
            let maxTdCount = findMaxTdCount();
            let htmlFolter = `
               <table  >
                <tr >
                    <td class="trTfoot" colspan=${maxTdCount} style="font-size: 25px;position: sticky;  bottom: 0; background: #fff; color: #016b69; font-family: 'Roboto';text-align: center;" class="myTableCell">Chú thích : Sức chứa còn lại --- (<i style="font-size: 22px;" class="fa-regular fa-circle"></i>)
                      = 0% --- (<i style="font-size: 22px; color:#00FF00" class="fa-solid fa-circle"></i>) > 80% 
                        --- (<i style="font-size: 22px;  color:#FF0000 " class="fa-solid fa-circle"></i>) < 50%
                --- (<i style="font-size: 22px;color:#F6B26B" class="fa-solid fa-circle"></i>) 50% - 80% </td>
                </tr>
                 </table>

            `;
            $(".rendertable_list").html(htmlTotal);
            $(".rendertable_list").append(htmlFolter);
            MergeRow1();
        }
        removeLoading()


    } catch (error) {
        console.error(error);
    }
}

function renderTang1(khuvuc, make) {
    let dvsx = $madvsx;
    let tenkho = $("#slc_kho").val();
    return $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetTang&Para1=${dvsx}&Para2=${tenkho}&Para3=${khuvuc}&Para4=${make}&Para5=a&Para6=a`,
        type: 'GET'
    });
}

function renderO1(khuvuc, make, matang) {
    let dvsx = $madvsx;  // Ensure $madvsx is defined
    let tenkho = $("#slc_kho").val();  // Make sure jQuery is properly loaded
    return $.ajax({
        url: `/api/ViTriKhoNew/Get?Action=GetOCBM&Para1=${dvsx}&Para2=${tenkho}&Para3=${khuvuc}&Para4=${make}&Para5=${matang}&Para6=a`,
        type: 'GET'
    });
}

function MergeRow1() {
    let tableLength = $(".tbody1 tr").length;

    for (let i = 0; i < tableLength; i++) {
        let merge = 1;
        let valueMerge = $(".tbody1 tr").eq(i).find(".rowMerge").text();
        for (let j = i + 1; j < tableLength; j++) {
            let valueMergeNext = $(".tbody1 tr").eq(j).find(".rowMerge").text();
            if (valueMerge === valueMergeNext) {
                merge++;
            } else {
                break;
            }
        }
        $(".tbody1 tr").eq(i).find(".rowMerge, td:eq(0)").attr("rowspan", merge);
        for (let k = 1; k < merge; k++) {
            $(".tbody1 tr").eq(i + k).find(".rowMerge, td:eq(0)").hide();
        }
        i += merge - 1;
    }
}
function mergeTable(tableId, columnIndex) {
    var firstCell = null;
    var rowspan = 1;

    $('.' + tableId + ' tr').each(function () {
        var currentCell = $(this).find('td').eq(columnIndex);

        if (firstCell == null) {
            firstCell = currentCell; // Lưu ô đầu tiên để so sánh
        } else if (currentCell.text() == firstCell.text()) {
            currentCell.remove(); // Xóa ô hiện tại
            rowspan++; // Tăng rowspan của ô đầu tiên
            firstCell.attr('rowspan', rowspan); // Cập nhật rowspan
        } else {
            firstCell = currentCell; // Gán lại ô đầu tiên
            rowspan = 1; // Đặt lại rowspan
        }
    });
}
function fomatRenderTang(htmlTangFomat) {
    const wrappedHtml = `<table>${htmlTangFomat}</table>`;
    const rows = $(wrappedHtml).find('tr');

    let maxCols = 0;

    // Find the maximum number of columns
    rows.each(function () {
        const cols = $(this).find('td').length;
        if (cols > maxCols) {
            maxCols = cols;
        }
    });

    // Adjust each row to have the same number of columns
    let htmltr = '';
    rows.each(function () {
        const cols = $(this).find('td').length;
        if (cols < maxCols) {
            const lastTd = $(this).find('td').last();
            const colspan = maxCols - cols + 1;
            lastTd.attr('colspan', colspan);
        }
        htmltr += `<tr>${$(this).html()}</tr>\n`;
    });

    return htmltr
}
function findMaxTdCount() {
    let maxTdCount = 0;

    $(".tbody").find("tr").each(function () {
        let tdCount = $(this).find("td").length;

        if (tdCount > maxTdCount) {
            maxTdCount = tdCount;
        }
    });

    return maxTdCount;
}
$(document).on("click", ".popup4", function (event) {
    if (!$(event.target).closest(".lookup-results li, .lookup-input, .lookup-input, .text_day,.ultitle").length && $(".lookup-results").is(":visible")) {
        $(".lookup-results").hide();
        $(".lookup-input").hide();
        $(".ultitle").hide()
        $(".text_day").removeClass("hideshow")
    }
});
$(".text_day").on("click", function () {
    $(".text_day").toggleClass("hideshow")
    if ($(this).hasClass("hideshow")) {
        $(".lookup-results").show();
        $(".lookup-input").show();
        $(".ultitle").show()
    }
    else {
        $(".lookup-results").hide();
        $(".lookup-input").hide();
        $(".ultitle").hide()
    }
})
let $trMaDayCheck = ""
$(".lookup-results").on("click", "li", function () {
    var selectedValue = $(this).data("name")
    $trMaDay = $(this).data("value")
    $(".text_day").text(selectedValue)
    $(".lookup-results").hide();
    $(".lookup-input").hide();
    $(".ultitle").hide()
    $(".text_day").removeClass("hideshow")
    if ($trMaDayCheck != $trMaDay) {
        renderKeNew()
        addLoading()
    }
    $trMaDayCheck = $trMaDay

});
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
function getDate() {
    $(".datepicker3").flatpickr({
        enableTime: false,
        dateFormat: "d-m-Y ",
        time_24hr: false,
        defaultDate: new Date()
    });
}
getDate()
function removeLoading() {
    if ($(".popup4").is(":visible"))
        $(".list_loading").addClass("active")
}
function addLoading() {
    if ($(".popup4").is(":visible"))
        $(".list_loading").removeClass("active")
}
async function renderDVSXHT() {
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX").val();
    const [response1] = await Promise.all([
        fetch(`/api/NHGiaCong/Get?Action=GetDVSXHT&para1=${madh}&para2=${malenhsx}&para3=A&para4=a&para5=a&para6=a&para7=a`),
    ]);
    let data = await response1.json()
    let html = ``
    data.map(item => {
        html += `
                 <option value="${item.MaDVSX}">${item.TenDVSX}</option>
            `
    })
    $(".madvsxht ").html(html)
    $(".madvsxht ").select2()
    previousValue = $('.madvsxht').val();
}
let previousValue = ""
let isChangeProgrammatically = false; // Cờ để kiểm soát sự kiện change


$('.madvsxht').on('change', function () {
    if (isChangeProgrammatically) {
        isChangeProgrammatically = false;
        return;
    }
    let newValue = $(this).val();
    Swal.fire({
        title: 'Bạn có chắc muốn thay đổi lựa chọn?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý đổi',
        cancelButtonText: 'Hủy',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        showCloseButton: true,// Thêm nút đóng
    }).then((result) => {
        if (result.isConfirmed) {
            previousValue = newValue;
            saveChangeDVSXHT(newValue, user)
        }
    });
    $(".swal2-cancel").on("click", function () {
        isChangeProgrammatically = true;
        $('.madvsxht').val(previousValue).trigger('change');
    })


});
function saveChangeDVSXHT(newValue, user) {
    let madh = $(".donhang option:selected").data("magop");
    let malenhsx = $(".dotSX").val();
    $.ajax({
        url: `/api/NHGiaCong/Get?Action=UpdateMaDVSXHT&para1=${newValue}&para2=${user}&para3=${madh}&para4=${malenhsx}&para5=a&para6=a&para7=a`,
        type: 'Get',
        success: function () {
            toastr.success("Thay đổi thành công")
        }
    })

}
