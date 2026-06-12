var userNameSave = localStorage.getItem("username1")
let timeoutId2;
var $currentRow;
var barcodegoc = ""
var index = 0
$(function () {
    $(".select_2").select2()
    getDate()
    GetMaHangNL()
    //GetSoLo()
    $(".closebtn").on("click", function () {
        $('.barcodein').hide();
    })
    $(".btn-add ").on("click", function () {
        $('#myModal').modal('show');
        getDataRow()
    })
    $(".saveKien").on("click", async function () {
        if ($('.sokienMD').val() == "") {
            handeleNoti("Vui lòng nhập sô kiện!", "warning")
            return
        }
        const checkDub = await CheckDub($('.sokienMD').val());
        if (checkDub) {
            handeleNoti("Số kiện đã có trong bảng ", "warning")
            return
        }
        if ($('.thucnhapMD').val() == "") {
            handeleNoti("Vui lòng nhập số lượng thực nhập!", "warning")
            return
        }
        if ($('.tenlotMD').val() == "") {
            handeleNoti("Vui lòng nhập tên lót!", "warning")
            return
        }

        await addRowBody()
        $('.sokienMD').val("")
        $('.thucnhapMD').val("")
        $('.tenlotMD').val("")
    })
    $("#tbody").on("click", '.changeCLCT', function () {
        let $tr = $(this).closest("tr")
        let inputSLN = $tr.find("input.inputSLN").val()
        let SLCT = $tr.find("span.SLCT").text().trim()
        $tr.find(".checkNK").prop("checked", true)
        if (inputSLN == "") {
            $tr.find("input.inputSLN").val(formatNumberUS(SLCT))
            calThanhTien()
            getTfoot()
        } else {
            calThanhTien()
            getTfoot()
        }

    })
    $("#tbody").on("change", ".checkNK", async function () {
        let $tr = $(this).closest("tr")
        if (!$(this).is(":checked")) {
            var checkXH = $tr.data("isxh")
            if (checkXH == 1) {
                iziToast.warning({
                    message: `Kiện này đã đc cấp xuất hàng không được check hủy`,
                    position: 'topRight',
                    timeout: 2500
                });
                $(this).prop("checked", true)
            }
            $tr.find("input.inputSLN").val("")
            $tr.find("td.thanhtien").text(0)
            getTfoot()

            return
        }
        else {
            let inputSLN = $tr.find("input.inputSLN").val()
            let SLCT = $tr.find("span.SLCT").text().trim()

            if (inputSLN == "") {
                $tr.find("input.inputSLN").val(formatNumberUS(SLCT))
                calThanhTien()
                getTfoot()
            } else {
                calThanhTien()
                getTfoot()
            }
        }

    })
    $(".btn-save").on("click", function () {
        $('#myModalN').modal('show');

    })
    $(".saveTable").on("click", function () {
        SaveTable()
    })
    $(".checkAllNK").on("click", function () {
        $("#tbody tr").each(function () {
            var checkXH = $(this).data("isxh")
            if (checkXH == 1) {
                return
            }
            if ($(".checkAllNK").is(":checked")) {
                $(this).find("input.checkNK").prop("checked", true)
                let slct = $(this).find("span.SLCT").text().trim()
                var dongia = $(this).find("td.dongia").text().trim()
                $(this).find("input.inputSLN").val(formatNumberUS(slct))
                $(this).find("td.thanhtien").text(formatNumberUS(parseFloat((parseFloat(slct) * parseFloat(dongia)).toFixed(2))))
            }
            else {
                $(this).find("input.checkNK").prop("checked", false)
                $(this).find("input.inputSLN").val("")
                $(this).find("td.thanhtien").text("")
            }
        })
        getTfoot()
    })
    $("#tbody").on("click", "tr", function () {
        $("#tbody").find("tr").removeClass("active")
        $(this).addClass("active")
    })
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })

    $(".checkAllBC").on("click", function () {
        $("#tbody tr").each(function () {
            if ($(".checkAllBC").is(":checked")) {
                $(this).find("input.checkbc").prop("checked", true)
            }
            else $(this).find("input.checkbc").prop("checked", false)
        })
    })
    $(".icon_reload").on("click", function () {
        GetPhieuNK()
    })
    $(".item_inputsearch").on("keyup", function (e) {
        clearTimeout(timeoutId2);
        timeoutId2 = setTimeout(() => {
            let searchValue = removeDiacritics($(".item_inputsearch").val().toUpperCase())
            $("#tbody tr").each(function () {
                let sokien = removeDiacritics($(this).find(".sokien").text().toString().toUpperCase());
                let solot = removeDiacritics($(this).find(".solot").text().toString().toUpperCase());

                if (sokien.includes(searchValue) || solot.includes(searchValue)) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
                if (searchValue == "") {
                    $(this).show();
                }
            });
            getTfoot()
        }, 200);

    })
    $(".item_inputsearchA").on("keyup", function (e) {
        clearTimeout(timeoutId2);
        timeoutId2 = setTimeout(() => {
            let searchValue = removeDiacritics($(".item_inputsearchA").val().toUpperCase())
            $("#tbody tr").each(function () {
                let sokien = removeDiacritics($(this).find(".sokien").text().toString().toUpperCase());

                if (sokien == searchValue) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
                if (searchValue == "") {
                    $(this).show();
                }

            });
            getTfoot()
        }, 200);

    })
    $('#myModal4').on('shown.bs.modal', function () {
        $('.SuaTNInput').focus(); // Focus khi modal hiển thị xong
    });
    $(document).on('click', '.fa-pen-to-square', function () {

        $currentRow = $(this).closest('tr');

        var checkXH = $currentRow.data("isxh")
        if (checkXH == 1) {
            iziToast.warning({
                message: `Kiện này đã đc cấp xuất hàng không được cập nhật số lượng`,
                position: 'topRight',
                timeout: 2500
            });
            retrun
        }
        let value = $currentRow.find('.inputSLN').val()
        $('.SuaTNInput').val(value);
        let sokien = $currentRow.data("sokien")
        let khosize = $currentRow.data("khosize")
        let donvi = $currentRow.data("donvi")
        barcodegoc = $currentRow.data("barcodegoc")
        let theoCT = $currentRow.find(".SLCT").text().trim()

        let solot = $currentRow.find('.solot').text().trim();
        let solo = $("#soloid option:selected").text().trim()
        let vattu = $("#cayvai option:selected").data("chitiet")
        var dongia = $("#cayvai option:selected").data("dongia")
        $('.dongiaMD').val(parseFloat(dongia))
        $('.dongiaMDText').text(parseFloat(dongia))
        $('.khosizeMD').text(khosize)
        $('.donviMD').text(donvi)
        $('.soloMD').text(solo)
        $('.vattuMD').text(vattu)
        $('.sokienS4').text(sokien)
        $('.solotS4').text(solot)
        $('.theoCTS4').text(theoCT)

        $('#myModal4').modal('show');
    });
    $(".SuaTNInput ").on("input", function () {

        var chungtu = $('.theoCTS4').text()
        if (parseFloat($(this).val()) > parseFloat(chungtu)) {
            iziToast.warning({
                message: `Vui lòng nhập không vượt SL chứng từ`,
                position: 'topRight',
                timeout: 2500
            });
            $(this).val($(this).val().slice(0, -1))
            return
        }
    })
    $(document).on('click', '.btnXacNhanSuaTN', function () {
        let _thucnhap = $('.SuaTNInput').val();
        if ($currentRow) {
            $currentRow.find('.inputSLN').val(_thucnhap);
        }
        calThanhTien()
        getTfoot()
        $('#myModal4').modal('hide');
    });
    $("#thead").on("click", '.fa-minus-square', function () {
        $('#myModalD').modal('show');
    })
    $(".btn-huy").on("click", function () {
        $('#myModalH').modal('show');
    })
})
function calThanhTien() {
    $("#tbody tr").each(function () {
        let slct = $(this).find("input.inputSLN").val() || 0
        var dongia = $("#cayvai option:selected").data("dongia")
        $(this).find("td.thanhtien").text(formatNumberUS(parseFloat((parseFloat(slct) * parseFloat(dongia)).toFixed(2))))
    })
}
async function CheckKienXH(sokien) {
    var npl = $("#cayvai option:selected").data("npl")
    let SoLo = $("#soloid").val()

    const url = `/api/PhieuXuatHangNPL/Get?action=GetCheckKienXuatHang&para1=${sokien}&para2=${npl}&para3=${SoLo}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) return false
        return true

    } catch (error) {
        console.error(error.message);
    }
}
async function ResetThucNhap() {
    $("#tbody tr").each(async function () {

        var checkXH = $(this).data("isxh")
        if (checkXH == 1) {
            retrun
        }
        $(this).find("input.inputSLN").val("")
    })
    $('#myModalD').modal('hide');

    await calThanhTien()
    await getTfoot()
}
function renderBackgroundTN() {
    $("#tbody tr").each(function () {
        var SLTN = $(this).find(".SLCT").text()
        var SLN = $(this).find("input.inputSLN").val()
        $(this).find(".itemThucNhap").removeClass("tdThucNhapB")
        $(this).find(".itemThucNhap").removeClass("tdThucNhapL")
        if (parseFloat(SLTN) == parseFloat(SLN)) {
            $(this).find(".itemThucNhap").addClass("tdThucNhapB")
        }
        else $(this).find(".itemThucNhap").addClass("tdThucNhapL")
    })
}
function removeDiacritics(str) {
    return str.toString().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}
function inbarcode() {
    GetDataBarCode()
    $("#myModalIn").modal("show")
    $('.barcodein').show();
}
function GetDataBarCode() {
    var mavt = $("#cayvai option:selected").data("tenmavt");
    $('.lablebarcode').empty(); // Đúng class
    $('.lablebarcodeB').empty();
    $("#tbody tr").each(function (index) {
        var $trRow = $(this);
        var checkbarCode = $trRow.find("input.checkbc").is(":checked");
        if (!checkbarCode) return;

        var chitiet = $trRow.find("td.chiTietNK").text().trim();
        var mau = $("#cayvai option:selected").data("maumau");
        var sokien = $trRow.find("td.sokien").text().trim();
        var thucnhap = $trRow.find("input.inputSLN").val();
        var solot = $trRow.find("td.solot").text().trim();
        var donvi = $trRow.find("td.donvi").text().trim();
        var ghichu = ""; // Nếu có thì thêm giá trị
        var barcodechia = $trRow.find("td.barcode").text().trim();
        var ngayNhap = moment().format("DD/MM/YYYY");


        const labelHTML = `
          <div class="box-item">
            <div class="label-container">
              <div class="label-header">DONAGAMEX</div>
              <div style="display: flex; justify-content: space-between;padding:4px">
                <div class="left-col" style="width: 65%;">
                  <p class="line-item cayvaibc mavtbc mb-2"><span>${mavt}</span></p>
                  <p class="line-item cayvaibc chitietbc mb-2"><span>${chitiet}</span></p>
                  <p class="line-item cayvaibc mb-2">${mau}</p>
                  <p class="line-item item-gapprint mb-2">
                    <span class="spantitle">Kiện:</span> <span class="spanitem">${sokien}</span>
                    <span class="spantitle">Lót:</span>  <span class="spanitem"> ${solot}</span>
                  
                   
                  </p>
                  <p class="line-item item-gapprint mb-2">
                    <span class="spantitle">SGĐC: </span> <span class="spanitem">${thucnhap}</span>
                    <span class="spanitemdv ms-1">${donvi}</span>
                     <span class="spantitle ms-2">QrCode:</span>
                      <span class="spanitem" style="margin-left: 0rem;">${barcodechia}</span>
                  </p>
                      <p class="line-item ">
                        <span class="spantitle">Ghi chú: </span> <span class="ghichubc">${ghichu}</span>
                     </p>
                </div>
                <div class="right-col " style="width: 35%; text-align: center;">
                 <div class="itemngay w-100">
                     <p class="ngaynhap">Ngày nhập:</p>
                  <p>${ngayNhap}</p>
                    <div>
                  <div id="qr-${index}" class="qr-code"></div>
                </div>
              </div>
            </div>
          </div>
        `;
        var classGap = index % 2 == 0 ? "pe-`" : "ps-1"
        const labelHTMLB = `
         <div class="col-6 col-md-6 col-lg-6 d-flex justify-content-center ${classGap} py-2">

           <div class="label-container w-100">
              <div class="label-header">DONAGAMEX</div>
              <div style="display: flex; justify-content: space-between;padding:4px">
                <div class="left-col" style="width: 65%;">
                  <p class="line-item cayvaibc mavtbc mb-2"><span>${mavt}</span></p>
                  <p class="line-item cayvaibc chitietbc mb-2"><span>${chitiet}</span></p>
                  <p class="line-item cayvaibc mb-2">${mau}</p>
                  <p class="line-item item-gapprint mb-2">
                    <span class="spantitle">Kiện:</span> <span class="spanitem">${sokien}</span>
                    <span class="spantitle">Lót:</span>  <span class="spanitem"> ${solot}</span>
                  
                  </p>
                 <p class="line-item item-gapprint mb-2">
                    <span class="spantitle">SGĐC: </span> <span class="spanitem">${thucnhap}</span>
                    <span class="spanitemdv ms-1">${donvi}</span>
                      <span class="spantitle ms-2">QrCode:</span>
                      <span class="spanitem" style="margin-left: 0rem;">${barcodechia}</span>
                  </p>
                   <p class="line-item ">
                        <span class="spantitle">Ghi chú: </span> <span class="ghichubc">${ghichu}</span>
                     </p>
                </div>
                <div class="right-col " style="width: 35%; text-align: center;">
                 <div class="itemngay w-100">
                     <p class="ngaynhap">Ngày nhập:</p>
                  <p>${ngayNhap}</p>
                <div style="display: flex; justify-content: end;">
                  <div id="qrB-${index}" class="qr-code"></div>
                </div>
              </div>
            </div>
        </div>
        `;
        $('.lablebarcodeB').append(labelHTMLB);
        $('.lablebarcode').append(labelHTML);

        new QRCode(document.getElementById(`qr-${index}`), {
            text: barcodechia,
            width: 100,
            height: 100
        });
        new QRCode(document.getElementById(`qrB-${index}`), {
            text: barcodechia,
            width: 100,
            height: 100
        });
    });
}



function addBackInRow() {
    $("#tbody tr").each(function () {
        if ($(this).find("input.checkNK").is(":checked")) {
            $(this).css("background", "#b5fff8")
            $(this).attr("data-nhapkho", 1)
            $(this).data("nhapkho", 1)

        }
        else $(this).css("background", "")
    })
}
function addBackHInRow() {
    $("#tbody tr").each(function () {
        if ($(this).find("input.checkNK").is(":checked")) {
            $(this).css("background", "#b5fff8")


        }
        else {
            $(this).css("background", "")
            $(this).attr("data-nhapkho", 0)
            $(this).data("nhapkho", 0)
        }
    })
}
async function CheckDub(sokien) {
    var npl = $("#cayvai option:selected").data("npl")
    let SoLo = $("#soloid").val()

    const url = `/api/PhieuXuatHangNPL/Get?action=GetCheckDub&para1=${SoLo}&para2=${npl}&para3=${sokien}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) return false
        return true

    } catch (error) {
        console.error(error.message);
    }
}
function getDate() {
    $(".datepicker").flatpickr({
        enableTime: false,
        dateFormat: "d-m-Y ",
        time_24hr: false,
        allowInput: true,
        defaultDate: new Date()
    });
}
async function addRowBody() {
    let x = {
        SoKien: $('.sokienMD').val(),
        KhoVai: $('.khosizeMDA').text().trim(),
        TenDVVT: $('.donviMDA').text().trim(),
        SoLoT: $('.tenlotMD').val(),
        SLNhap: $('.thucnhapMD').val(),
        DonGia: $('.dongiaMDA').val(),
        ChiTiet: $('.vattuMDA').text().trim(),
    }
    let html = `
                <tr style="background:#b4ff8c" data-isxh="0"  data-id="0" data-sokien="${x.SoKien}" data-khosize="${x.KhoVai}" data-donvi="${x.TenDVVT}">
                    <td style="max-width: 60px;min-width: 50px;">
                          <div>
                           <input type="checkbox" class="checkNK" style = "width: 18px; height: 18px;" checked />
                          </div>
                    </td>
                    <td class="" style="">${index + 1}</td>
                   
                    <td class="solot" style="">${x.SoLoT}</td>
                    <td class="chiTietNK" style="">${x.ChiTiet}</td>
                    <td style="">${x.KhoVai}</td>
                    <td class="donvi" style=" ">${x.TenDVVT}</td>
                     <td class="sokien" style="">${x.SoKien}</td>
                  <td  style=" "> <div class="itemct"><span class="SLCT" >${x.SLNhap}</span> <i class="fa-solid fa-arrow-right"></i></div> </td>
                    <td class="itemThucNhap" style="width: 100px;
    min-width: 100px;">
                       <div class="" style="width: 100%; position: relative;padding-right: 15px;">
                            <input readonly style="border: none; outline: none;text-align:center;width: 100%;background: transparent;color:;font-weight:700;width: 100%;
                            " type="number" class="inputSLN" value="${x.SLNhap}"/>
                               <i class="fa-solid fa-pen-to-square"></i>
                         </div>
                    </td>
                   
                    <td class="dongia" style="">${formatNumberUS(x.DonGia) ?? ""}</td>
                    <td class="thanhtien" style="">${formatNumberUS(parseFloat((x.DonGia * x.SLNhap).toFixed(2)))}</td>
                    <td class="barcode" style=""></td>
                    <td style="max-width: 60px;min-width: 50px;">
                            <div style="display: flex;justify-content: center; align-items: center;">
                           <input type = "checkbox" class="checkbc" style = "width: 18px; height: 18px;"  checked />
                          </div>
                    </td>
               </tr>`
    $("#tbody").append(html)
    $('#myModal').modal('hide');
    index += 1
    getTfoot()
    let arrSave = []
    let lastRow = $('#tbody tr:last');
    let object = await handleList(lastRow, 0)
    arrSave.push(object)
    await Save(arrSave)
    await GetMaxID(lastRow)

}
async function SaveTable() {
    if ($(".paletMD").val() == "") {
        iziToast.warning({
            message: `Vui lòng nhập palet`,
            position: 'topRight',
            timeout: 2000
        });
        return
    }
    let arrSave = [];
    let rows = $("#tbody tr").toArray();

    for (const row of rows) {
        const $row = $(row);
        const isChecked = $row.find('.checkNK').is(':checked');

        if (!isChecked) continue; // Bỏ qua nếu checkbox không được chọn

        let object = await handleList($(row), 1);
        arrSave.push(object);
    }
    await Save(arrSave);
    await $('#myModalN').modal('hide');

    await addBackInRow()
}

async function HuyNhan() {

    let arrSave = [];
    let rows = $("#tbody tr").toArray();

    for (const row of rows) {
        const $row = $(row);
        const isChecked = $row.find('.checkNK').is(':checked');
        const dataHuy = $row.data('nhapkho')

        if (isChecked || dataHuy != 1) continue; // Bỏ qua nếu checkbox không được chọn

        let object = await handleList($(row), 1);
        arrSave.push(object);
    }
    await Save(arrSave);
    await $('#myModalH').modal('hide');

    await addBackHInRow()
}

function handeleNoti(mess, title) {
    iziToast.warning({
        title: `${title}`,
        message: `${mess}`,
        position: 'topRight'
    });
}
async function handleList($this, checkSave) {
    let SoKien = $this.data("sokien");
    let SoLoT = $this.find(".solot").text().trim();
    let SLNhap = $this.find(".inputSLN").val() || "0";
    let DonGia = $("#cayvai option:selected").data("dongia")

    let SoLo = $("#soloid").val()
    var khovaiID = $("#cayvai option:selected").data("khovai")
    var maumau = $("#cayvai option:selected").data("maumau")
    var haiquan = $("#cayvai option:selected").data("haiquan")
    var npl = $("#cayvai option:selected").data("npl")
    var maketoan = $("#cayvai option:selected").data("maketoan")
    var mauvt = $("#cayvai option:selected").data("mauvt")
    var madvvt = $("#cayvai option:selected").data("dvvt")
    var isnpl = $("#cayvai option:selected").data("isnpl")
    var mavt = $("#cayvai").val()
    let barCode = `${SoLo}@${npl}@${SoKien}`
    let slct = $this.find(".SLCT").text().trim() || "0";
    let object =
    {
        ID: $this.data("id"),
        SoLoID: SoLo,
        MaNPL: npl,
        MaVTID: mavt,
        MaMauVT: maumau,
        SoKien: SoKien,
        SoLoT: SoLoT,
        MaHaiQuan: haiquan,
        MaKeToan: maketoan,
        SoGhiDauCay: slct,
        NW: isnpl,
        GW: 0,
        BarCode: barCode,
        GhiChu: "",
        IsNPL: checkSave == 0 ? 0 : $this.find('.checkNK').is(':checked') ? 1 : 0,
        KhoVaiID: khovaiID,
        SoKienParent: checkSave == 0 ? null : $(".paletMD").val(),
        SoLuongThucTe: SLNhap ?? 0,
        DonGia: DonGia,
        ThanhTien: parseFloat(SLNhap) * parseFloat(DonGia),
        Pallet: checkSave == 0 ? null : moment($(".datepicker").val(), "DD-MM-YYYY").format("YYYY-MM-DD"),
        MaDVVT: madvvt,
        MauVTID: mauvt,
    }
    return object
}
function getDataRow() {
    let lastRow = $('#tbody tr:last');
    let sokien = lastRow.data("sokien")
    let khosize = lastRow.data("khosize")
    let donvi = lastRow.data("donvi")

    let solo = $("#soloid option:selected").text().trim()
    let vattu = $("#cayvai option:selected").data("chitiet")
    var dongia = $("#cayvai option:selected").data("dongia")
    $('.dongiaMDA').val(parseFloat(dongia))
    $('.dongiaMDTextA').text(parseFloat(dongia))
    $('.khosizeMDA').text(khosize)
    $('.donviMDA').text(donvi)
    $('.soloMDA').text(solo)
    $('.vattuMDA').text(vattu)

}
async function GetMaxID(lastRow) {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetMaxIDKien`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        lastRow.attr('data-id', data[0].ID);
        lastRow.data("id", data[0].ID)
        lastRow.attr('data-barcodegoc', data[0].BarCode);
        lastRow.data("barcodegoc", data[0].BarCode)
        lastRow.find(".barcode").text(data[0].BarCode)
    } catch (error) {
        console.error(error.message);
    }
}
async function Save(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser = userName
    const request = new Request(`/api/PhieuXuatHangNPL/PostWeb?action=PostVT&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        iziToast.success({
            message: `Lưu thành công`,
            position: 'topRight',
            timeout: 2000
        });
    }
    else {
        iziToast.warning({
            message: `Lưu thất bại`,
            position: 'topRight',
            timeout: 2500
        });
        return
    }
}
async function GetMaHangNL() {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetMaHangMK&para1=0`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        let html = ``;
        data.map(x => {
            html += `
                <option value="${x.MaHang}">${x.MaHang}</option>
            `

        })
        $("#mahangid").html(html)
        //GetPhieuNK()
        GetVatTu()
    } catch (error) {
        console.error(error.message);
    }
}
async function GetSoLo() {
    var maVT = $("#cayvai").val()
    var mauVT = $("#cayvai option:selected").data("mauvt")
    var khovai = $("#cayvai option:selected").data("khovai")
    var soloid = $("#cayvai option:selected").data("soloid")
    const url = `/api/PhieuXuatHangNPL/Get?action=GetSoLoMK&para1=${maVT}&para2=${mauVT}&para3=${khovai}&para4=${soloid}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        let html = ``;
        data.map(x => {
            html += `
                <option value="${x.SoLoID}">${x.SoLo}</option>
            `

        })
        $("#soloid").html(html)
        GetPhieuNK()
        //GetVatTu()
    } catch (error) {
        console.error(error.message);
    }
}
var dataVT = []
async function GetVatTu() {
    dataVT = []
    const url = `/api/PhieuXuatHangNPL/Get?action=GetVTMK&para1=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        dataVT = data
        returnVatTu()
    } catch (error) {
        console.error(error.message);
    }
}
function returnVatTu() {
    var isChecked = $(".checkNhapThung").prop("checked") ? 1 : 0;
    var data = dataVT.filter(x => x.isNK == isChecked);
    if (data.length == 0) {
        $("#tbody").empty()
        $("#tfoot").empty()
        $("#cayvai").empty()
        return
    }
    let html = ``;
    data.map(x => {
        html += `
                <option data-soloid="${x.SoLoID}" data-isnpl="${x.IsNPL}" data-tenmavt="${x.MaVT}" data-dongia="${x.DonGia}" data-chitiet="${x.ChiTiet}" data-dvvt="${x.MaDVVT}" data-maumau=${x.MaMauVT}  data-haiquan="${x.MaHaiQuan}" data-npl="${x.MaNPL}" data-maketoan="${x.MaKeToan}" data-khovai="${x.KhoVaiID}" data-mauvt=${x.MauVTID} value="${x.MaVTID}">${x.Display}</option>
            `

    })

    $("#cayvai").html(html)
    GetSoLo()
}
async function GetPhieuNK() {
    $(".checkAllBC").prop("checked", false)
    var maVT = $("#cayvai").val()
    var mauVT = $("#cayvai option:selected").data("mauvt")
    var khovai = $("#cayvai option:selected").data("khovai")
    var isNPL = $("#cayvai option:selected").data("isnpl")
    const url = `/api/PhieuXuatHangNPL/Get?action=GetBienBan&para1=${$("#soloid").val()}&para2=${maVT}&para3=${mauVT}&para4=${khovai}&para5=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        if (data.length == 0) {
            $("#tbody").empty()
            $("#tfoot").empty()
            return;
        }
        let html = ``;
        const checkLenghth = data.reduce((acc, cur) => acc + Number(cur.IsNK), 0);
        if (checkLenghth == data.length && data.length > 0)
            $("#thead").find("input.checkAllNK").prop("checked", true)

        else $("#thead").find("input.checkAllNK").prop("checked", false)
        data.map((x, i) => {
            let back = x.IsNK == 1 ? "#b5fff8" : ""
            html += `
                <tr style="background:${back}" data-nhapkho="${x.IsNK == 1 ? '1' : '0'}" data-isxh="${x.isXH}" data-id="${x.ID}" data-barcodegoc="${x.BarCodeGoc}" data-sokien="${x.SoKien}" data-khosize="${x.KhoVai}" data-donvi="${x.TenDVVT}">
                    <td style="max-width: 60px;min-width: 50px;">
                          <div style="display: flex;justify-content: center; align-items: center;">
                            <input type = "checkbox" class="checkNK" style = "width: 18px; height: 18px;" ${x.IsNK == 1 ? 'checked' : ''} />
                          </div>
                    </td>
                    <td class="" style="">${i + 1}</td>
                   
                    <td class="solot" style="">${x.SoLoT}</td>
                    <td class="chiTietNK" style="">${x.ChiTiet}</td>
                    <td style="">${x.KhoVai}</td>
                    <td class="donvi" style=" ">${x.TenDVVT}</td>
                    <td class="sokien" style="">${x.SoKien ?? ""}</td>
                    <td   style=" "> <div class="itemct"><span class="SLCT" >${x.SLChungTu}</span> <i class="fa-solid fa-arrow-right changeCLCT"></i></div> </td>
                    <td class="itemThucNhap" style="width: 100px;
    min-width: 100px;">
                        <div class="" style="width: 100%; position: relative;padding-right: 15px;">
                            <input readonly style="border: none; outline: none; text-align: center;  background: transparent; color: ; font-weight: 700;width: 100%;
                   " type="number" class="inputSLN"  value="${x.SLNhap || ''}"/>
                               <i class="fa-solid fa-pen-to-square"></i>
                     
                         </div>
                    </td>
                    <td class="dongia" style="">${formatNumberUS(x.DonGia) ?? ""}</td>
                   <td class="thanhtien" style="">${formatNumberUS(parseFloat((x.DonGia * x.SLNhap).toFixed(2)))}</td>
                    <td class="barcode" style="">${x.BarCode}</td>
                    <td style="max-width: 60px;min-width: 50px;">
                            <div style="display: flex;justify-content: center; align-items: center;">
                           <input type = "checkbox" class="checkbc" style = "width: 18px; height: 18px;"  />
                          </div>
                    </td>
               </tr>`
            index = i + 1
        })
        $("#tbody").html(html)
        getTfoot()

    } catch (error) {
        console.error(error.message);
    }
}
function getTfoot() {
    // Đếm các checkbox đang được chọn và hiển thị
    let kienXuatCount = $('#tbody .checkNK:checked:visible').length;
    let chuaXuatCount = $('#tbody .checkNK:not(:checked):visible').length;

    // Tổng số lượng nhập từ các input hiển thị
    let sumSLN = $('#tbody input.inputSLN:visible').map(function () {
        return parseFloat($(this).val().replace(/,/g, '')) || 0;
    }).get().reduce((a, b) => a + b, 0);

    // Tổng số lượng còn tồn từ các span hiển thị
    let sumSLCT = $('#tbody span.SLCT:visible').map(function () {
        return parseFloat($(this).text().trim().replace(/,/g, '')) || 0;
    }).get().reduce((a, b) => a + b, 0);

    // Tổng thành tiền từ các ô td hiển thị
    let sumThanhTien = $('#tbody td.thanhtien:visible').map(function () {
        return parseFloat($(this).text().trim().replace(/,/g, '')) || 0;
    }).get().reduce((a, b) => a + b, 0);

    let htmlTfoot = `
        <tr>
            <td colspan="2">
                Kiện nhập : <span>${kienXuatCount}</span>
            </td>
            <td>
                Chưa nhập : <span>${chuaXuatCount}</span>
            </td>
            <td colspan="4"></td>
            <td>${formatNumberUS(parseFloat(sumSLCT.toFixed(2)))}</td>
            <td>${formatNumberUS(parseFloat(sumSLN.toFixed(2)))}</td>
            <td></td>
            <td>${formatNumberUS(parseFloat(sumThanhTien.toFixed(2)))}</td>
            <td colspan="2"></td>
        </tr>
    `;

    $("#tfoot").html(htmlTfoot);
    renderBackgroundTN();
}
function formatNumberUS(num) {
    const [integer, decimal] = num.toString().split(".");
    const withCommas = integer.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return decimal ? `${withCommas}.${decimal}` : withCommas;
}

