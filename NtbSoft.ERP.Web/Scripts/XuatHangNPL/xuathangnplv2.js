
var userNameSave = localStorage.getItem("username1")
var MaNPLBarCode = ""
var SoLoBarCode = ""
var $thisTrDelete;
var dataXH = []
var trMaPhieu = ""
var trDH = ""
var trMaLenh = ""
var trloc = ""
var htmltbodyCurrent;
var $thisKienChia = ""
var dataItemXH;
var checkChangeTable = 1;
let timeoutId2;
var flagCheckApi = false
var SLNVuot = 0
var checkPQ;
var checkNhap = true
var checkChangeTableDev = false
var dataItemCode = []
var historyScanBarcode = [];
var checkaddBarCodeHistory;
var barcodeLocal = ""
var para1PhieuCap;
var para2PhieuCap;
var para3PhieuCap;
var trMaLenhDisplay;
var trMaGop
$(function () {
    $('#dateInput,#dateInput2').on('change', function () {
        GetPhieuNhapKho()
    });
    $(".select_2").select2();
    CheckPQ()
    $("#PhieuYeuCau_Selected").select2()
    $("#CayVai_Selected").select2()
    $("#tblDataBody").on("click", ".ipcheckbox", function () {
        CheckRow($(this))
    })
    const firstDayOfMonth = moment().startOf('month').toDate();
    const today = moment().toDate();

    const picker1 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        }
    });

    const picker2 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        }
    });

    const picker3 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker3'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        }
    });

    const picker4 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker4'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        }
    });

    // set giá trị
    picker1.dates.setValue(new tempusDominus.DateTime(firstDayOfMonth));
    picker2.dates.setValue(new tempusDominus.DateTime(today));
    picker3.dates.setValue(new tempusDominus.DateTime(firstDayOfMonth));
    picker4.dates.setValue(new tempusDominus.DateTime(today));

    $(".dateInput").trigger("click");
    picker1.hide();
    picker2.hide();
    picker3.hide();
    picker4.hide();

    $(".btn-tab1").on("click", function () {
        checkChangeTable = 1
        if (!checkChangeTableDev) {
            $(".tab1").show()
            $(".tab2").hide()
            $(".tab3").hide()
        }
        else {
            $("#myModalS").modal("show")
        }

    })
    $("#btnCheckHuySave").on("click", function () {
        $("#myModalS").modal("hide")
        if (checkChangeTable == 1) {
            $(".tab1").show()
            $(".tab2").hide()
            $(".tab3").hide()

        }
        else if (checkChangeTable == 3) {
            $(".tab2").hide()
            $(".tab1").hide()
            $(".tab3").show()
        }
        else {
            ChangeLoc()
        }
        loadXH()
    })

    $(".checkboxcapp").on("click", function () {

        if ($(this).is(":checked")) {
            $(".checkboxcapcp").prop("checked", false)
        } else {
            if ($(".checkboxcCap").hasClass("d-none")) {
                $(this).prop("checked", true)
                $(".checkboxcapcp").prop("checked", false)
            }
            else {
                $(".checkboxcapcp").prop("checked", true)
            }
        }
    })

    $(".checkboxcapcp").on("click", function () {
        if ($(this).is(":checked")) {
            $(".checkboxcapp").prop("checked", false)
        } else {
            $(".checkboxcapp").prop("checked", true)
        }
    })
    $("#btnCheckSave").on("click", function myfunction() {
        //htmltbodyCurrent = $("#tblDataBody").html()
        $("#myModalS").modal("hide")
        if (checkChangeTable == 2) {
            var loc = $(".select_loc").val();
            var $select = 2 == 1 ? $("#PhieuYeuCau_Selected") : $("#MaLenhSX");
            var $selectedOption = $select.find('option[value="' + valueSelect + '"]');

            $selectedOption.prependTo($select); // Đưa option lên đầu
            $select.val(valueSelect);           // Gán lại giá trị (không gây change)
        }


    })
    $(".btn-tab2").on("click", function () {
        $(".tab2").show()
        $(".tab1").hide()
        $(".tab3").hide()
    })
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    $(".checkAllBC").on("click", function () {
        $("#tblDataBody tr").each(function () {
            if ($(".checkAllBC").is(":checked")) {
                $(this).find("input.checkbc").prop("checked", true)
            }
            else $(this).find("input.checkbc").prop("checked", false)
        })
    })

    $(".text_phieu").click(function () {
        $(this).toggleClass("active")
        if (!$(".text_phieu").hasClass("active")) {
            $("#PhieuYeuCau_Selected").select2('close');
        } else {
            $("#PhieuYeuCau_Selected").select2("open");
        }
    })
    $(document).on("blur", function (event) {
        if (!$(event.target).closest(".text_phieu").length) {
            $(".text_phieu").removeClass("active");
        }
    });
    $('#PhieuYeuCau_Selected').on('select2:select', function (e) {
        $(".text_phieu").removeClass("active");
    })

    $(document).on("click", '.huycatphieu', function () {
        let $this = $(this).closest("tr")

        $thisTrDelete = $this
        var isTachKien = $this.data("istachkien")
        $(".checkboxcapp").prop("checked", true)
        $(".checkboxcapcp").prop("checked", false)
        //if (isTachKien == 0)
        $(".checkboxcCap").addClass("d-none")
        $("#myModalD").modal("show")
    })
    $(".closebtn").on("click", function () {
        $('.barcodein').hide();
    })
    $(".fa-table").on("click", function () {
        renderDetailSLCap()
    })
    $("#CayVai_Selected").on("change", function () {
        GetXuatHang()
    })
    // tab 2 -----------------

    $(".fa-calendar-days").on("click", function () {
        $(this).closest("div").find("input.datepickerT").focus()
    })

    $(".icon_reload").on("click", function () {
        //GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
        GetPhieuNhapKho()
    })
    //$("#tbodyA .dx-datagrid-table").on("click", "tr", function () {
    //    $("#tbodyA").find("tr").removeClass("activeT")
    //    $(this).addClass("activeT")
    //    trMaPhieu = $(this).data("phieu")
    //    trDH = $(this).data("madh")
    //    trMaLenh = $(this).data("malenh")
    //    trloc = $(".select_loc").val()
    //    GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
    //})
    GetPhieuNhapKho();
    $("#tblDataBody").on("click", '.tachkien', function () {
        var $tr = $(this).closest("tr")
        $thisKienChia = $tr
        var solo = $tr.find("td.solo").text().trim()
        var chitiet = $tr.find("td.chitiet").text().trim()
        var mau = $tr.find("td.mau").text().trim()
        var sokien = $tr.find("td.sokien").text().trim()
        var khosize = $tr.find("td.khosize").text().trim()
        var thucnhap = $tr.find("td.thucnhap").text().trim()
        var donvi = $tr.find("td.donvi").text().trim()
        //var kiengoc = $tr.data("barcodegoc")

        $("#modalSoLo").text(solo)
        $("#modalVatTu").text(chitiet)
        $("#modalThucNhap").text(parseFloat(parseFloat(thucnhap).toFixed(2)))
        $("#modalMau").text(mau)
        $("#modalKhoSize").text(khosize)
        $("#modalDonVi").text(donvi)

        $("#modalKienGoc").val(sokien)

        $('#myModal').modal('show');

        $("#modalSoLuong").val("")
        $("#modalSoKienTach").val(1)
    })
    $("#modalSoLuong").on("input", function () {
        var $this = $(this);
        var currentVal = $this.val();

        var SLTN = parseFloat($("#modalThucNhap").text()) || 0; 9
        var SLTach = parseFloat($("#modalSoKienTach").val()) || 0;
        var SLN = parseFloat(currentVal) * SLTach;

        if (SLN > SLTN) {
            iziToast.warning({
                title: 'Warning',
                message: "Tổng thực xuất kiện chia không được lớn hơn thực xuất kiện gốc!!!",
                position: 'topRight'
            });

            $this.val(currentVal.slice(0, -1));
        }

    })

    $("#modalSoKienTach").on("input", function () {
        let value = $(this).val();
        value = value.replace(/[^0-9]/g, ''); // Chỉ giữ lại các số từ 0-9
        $(this).val(value); // Gán lại giá trị đã lọc vào input
        $("#modalSoLuong").val(""); // Reset ô số lượng
    })
    $("#btnSaveTachKien").on("click", function () {
        AddRowShareKien()
    })
    $("#btnComfirm").on("click", function () {
        checkAgeVuot = true
        AddTbody(dataItemXH, 2)
        itemvai.CheckVuot = true;
        itemvai.MetDaCap += parseFloat($(".thucnhap").val())

        $('#myModalV').modal('hide');
    })

    $(".item_inputsearch").on("keyup", function (e) {
        clearTimeout(timeoutId2);
        timeoutId2 = setTimeout(() => {
            let searchValue = removeDiacritics($(".item_inputsearch").val().toUpperCase())
            $("#tblDataBody tr").each(function () {
                let sokien = removeDiacritics($(this).find(".sokien").text().toString().toUpperCase());
                let solot = removeDiacritics($(this).find(".solot").text().toString().toUpperCase());
                let barcode = removeDiacritics($(this).find(".barcode").text().toString().toUpperCase());
                if (sokien.includes(searchValue) || solot.includes(searchValue) || barcode.includes(searchValue)) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
                if (searchValue == "") {
                    $(this).show();
                }
            });
        }, 200);
    })
    $(".item_inputsearchTab3").on("keyup", function (e) {
        clearTimeout(timeoutId2);
        timeoutId2 = setTimeout(() => {
            let searchValue = removeDiacritics($(".item_inputsearchTab3").val().toUpperCase())
            $("#tblDataBodyLS tr").each(function () {
                let sokien = removeDiacritics($(this).find(".sokien").text().toString().toUpperCase());
                let solot = removeDiacritics($(this).find(".solot").text().toString().toUpperCase());
                let barcode = removeDiacritics($(this).find(".barcode").text().toString().toUpperCase());
                if (sokien.includes(searchValue) || solot.includes(searchValue) || barcode.includes(searchValue)) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
                if (searchValue == "") {
                    $(this).show();
                }
            });
        }, 200);
    })
    //$(".enterbarcode").on("click", function () {
    //    if ($('#QRScan').val().trim() != "") {
    //        GetCheckBarCode($('#QRScan').val().trim())
    //        $("#QRScan").val("")
    //    }
    //    else {
    //        iziToast.warning({
    //            title: 'Warning',
    //            message: 'Vui lòng nhập barcode vào ô QR scan.',
    //            position: 'topRight'

    //        });
    //    }
    //})
    // tab3
    $("#btnHuyKien").on("click", function () {
        if ($(".tab3").is(":visible")) {
            DeleteKien()
        }

        //    
    })

})

function removeDiacritics(str) {
    return str.toString().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}
var valueSelect = "";
function getRowValues(tr) {
    const values = [];
    $(tr).children("td").each(function () {
        const $td = $(this);
        const $input = $td.find("input");

        if ($input.length == 6) {
            if ($input.attr("type") === "checkbox") {
                values.push($input.prop("checked") ? "checked" : "unchecked");
            } else {
                values.push($input.val());
            }
        } else {
            values.push($td.text().trim());
        }
    });
    return values.join("|");  // nối chuỗi để dễ so sánh
}

function compareRowsWithInput(tableSelector, htmlString) {
    const temp = $('<tbody>' + htmlString + '</tbody>');

    const currentRows = $(tableSelector).children("tr").toArray().map(tr => getRowValues(tr));
    const oldRows = temp.children("tr").toArray().map(tr => getRowValues(tr));

    if (currentRows.length !== oldRows.length) return false;

    for (let i = 0; i < currentRows.length; i++) {
        if (currentRows[i] !== oldRows[i]) return false;
    }
    return true;
}
async function GetMaxKien(para, maxkien) {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetMaxKien&para1=${para}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) {
            return ""
        }
        else {
            return data[0].SoKien

        }

    } catch (error) {
        console.error(error.message);
        return 0;
    }
}
var arrKienMax = []
function tangSoKien(soKien) {

    soKien = soKien.trim();
    var parts = soKien.split(".");

    if (parts.length === 1) {
        // Không có dấu chấm → thêm ".1"
        return soKien + ".1";
    } else if (parts.length === 2) {
        let soSauCham = parseInt(parts[1]);

        if (!isNaN(soSauCham)) {
            return parts[0] + "." + (soSauCham + 1);
        } else {
            // Nếu phần sau chấm không phải số → giữ nguyên
            return soKien;
        }
    }

    // Trường hợp đặc biệt: có nhiều dấu chấm → không xử lý
    return soKien;
}

async function AddRowShareKien() {
    var slchia = $("#modalSoLuong").val()
    if (slchia == "" || slchia == 0) {
        iziToast.warning({
            title: 'Warning',
            message: "Vui lòng nhập thực xuất chia!!!",
            position: 'topRight'
        });
        return
    }

    var $thisRow = $thisKienChia

    var soLo = $thisRow.find(".solo").text().trim();
    var chiTiet = $thisRow.find(".chitiet").text().trim();
    var mauVT = $thisRow.find(".mau").text().trim();
    var soKien = $thisRow.find(".sokien").text().trim();
    var khoSize = $thisRow.find(".khosize").text().trim();
    var thucNhap = parseFloat($thisRow.find(".thucnhap").text().trim()) || 0;
    var donVi = $thisRow.find(".donvi").text().trim();
    var soLot = $thisRow.find(".solot").text().trim();
    var poMua = $thisRow.find(".pomua").text().trim();
    var soLotbatch = $thisRow.find(".solotbatch").text().trim();
    var ghichu = $("#modalGhiChu").val()
    let barCodeGoc = $thisRow.data("barcodegoc")
    var checkVuot = $thisRow.find(".thucnhap").hasClass("text-danger")
    var maxSoKien = await GetMaxKien(barCodeGoc)
    var mavt = $thisRow.find(".mavt").text().trim();
    var vto = $thisRow.find(".vto").text().trim();
    var slyc = $thisRow.find(".slyc").text().trim();
    arrKienMax = []
    $("#tblDataBody tr").each(function () {
        let barCode = $(this).data("barcodegoc")
        let kienchia = $(this).find("td.sokien").text().trim()
        if (barCodeGoc == barCode) arrKienMax.push(kienchia)
    })
    if (arrKienMax != "")
        arrKienMax.push(maxSoKien)
    var maxKienTable = arrKienMax.sort(function (a, b) {
        const getPrefixAndNumber = (str) => {
            let parts = str.split('.');
            let prefix = parts[0];
            let num = parts.length > 1 ? parseInt(parts[1]) : 0;
            return [prefix, num];
        };

        let [prefixA, numA] = getPrefixAndNumber(a);
        let [prefixB, numB] = getPrefixAndNumber(b);

        if (prefixA !== prefixB) {
            return prefixA.localeCompare(prefixB); // sort theo chữ
        }

        return numA - numB;
    });
    $("#tblDataBody").find(".backgroundtd").removeClass("active")
    var dataAttrs = $thisRow.data();
    var maxKien = tangSoKien(maxKienTable[maxKienTable.length - 1])

    var SLTach = $("#modalSoKienTach").val()
    for (var i = 0; i < SLTach; i++) {
        var barcodechia = `${dataAttrs.barcode}${maxKien}`
        let html = `
     
           <tr data-dvtinh="${dataAttrs.dvtinh}" data-iskiemke="${dataAttrs.iskiemke}" data-ngaykiemke="${dataAttrs.ngaykiemke}" data-delete="0" data-dot="${dataAttrs.dot}" data-save="0" data-sokiensort='${dataAttrs.sokiensort}' data-soloid="${dataAttrs.soloid}" data-madvvt="${dataAttrs.madvvt}" data-npl="${dataAttrs.npl}"   data-mavt="${dataAttrs.mavt}"
                data-barcode="${barcodechia}"  data-kiengoc="${dataAttrs.kiengoc}" data-sokien="${maxKien}" data-khovai="${dataAttrs.khovai}" data-mavtid="${dataAttrs.mavtid}"
                  data-ngaynk="${dataAttrs.ngaynk}"  data-barcodegoc="${dataAttrs.barcodegoc}" data-phieuyc="${dataAttrs.phieuyc}" data-slgoc="${dataAttrs.slgoc}" data-mauvtid="${dataAttrs.mauvtid}">
                        <td  class="backgroundtd" style="max-width: 60px;min-width: 50px;">
                          <div style="display: flex;justify-content: center; align-items: center;">
                            <input type = "checkbox" class="checkXH savecheck" style = "width: 18px; height: 18px;pointer-events: none;"  checked />
                          </div>
                         </td>
                         <td class="pomua">${poMua}</td>
                        <td class="solo">${soLo}</td>
                        <td class="solo">${mavt}</td>
                         <td style="min-width:300px;max-width: 300px;" class="chitiet">${chiTiet}</td>
                         <td class="mau">${mauVT}</td>
                        <td class="solot d-none">${soLot}</td>
                        <td class="solotbatch">${soLotbatch}</td>
                         <td class="khosize">${khoSize}</td>
                              <td class="donvi">${donVi}</td>
                         <td class="sokien">${maxKien}</td>
                        <td class="vto">${vto}</td>
                        <td class="slyc">${slyc}</td>
                         <td class="thucnhap ${checkVuot ? "text-danger" : ""}">${slchia}</td>
                         <td class="ghichu">${ghichu}</td>
                         <td class="barcode d-none">${barcodechia}</td>
                        <td style="max-width: 60px;min-width: 50px;"  class="">
                            <div style="display: flex;justify-content: center; align-items: center;">
                           <input type= "checkbox" class="checkbc" style = "width: 18px; height: 18px;"  />
                          </div>
                        </td>
                        <td class="" style="background-color: #cbe732 !important;">
                           <div>
                           <i class="fas fa-share-alt-square tachkien d-none"></i>
                            </div>
                        </td>
                         <td class="">
                           <div>
                            <i class="fa-solid fa-rectangle-xmark "></i>
                            </div>
                        </td>
                </tr>

    `;
        /* SaveKienChia(dataAttrs.barcodegoc, barcodechia, maxKien, slchia, ghichu)*/
        $("#tblDataBody").prepend(html);
        maxKien = tangSoKien(maxKien)
    }
    var SLN = parseFloat($("#modalSoLuong").val()) || 0;
    var SLTach = parseFloat($("#modalSoKienTach").val()) || 0;
    var SLN = SLN * SLTach;
    var thucxuatkiengoc = thucNhap - SLN
    $thisRow.find(".thucnhap").text(parseFloat(thucxuatkiengoc.toFixed(2)))
    sortTableByMultipleClasses(['solo', 'chitiet', 'sokien']);
    $('#myModal').modal('hide');

    var item = dataXH.find(x => x.BarCode == barCodeGoc)
    item.SLN = thucxuatkiengoc
}
async function SaveKienChia(para1, para2, para3, para4, para5, para6) {

    var url = `/api/PhieuXuatHangNPL/Delete?Action=SaveChietNK&para1=${para1}&para2=${para2}&para3=${para3}&para4=${para4}&para5=${para5}&para6=${para6}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        await loadXH()
        GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)

    } catch (error) {
        console.error(error.message);
    }
}
function showToast(type, message, delay = 2000) {
    let toastId, messageId;

    switch (type) {
        case 'success':
            toastId = 'successToast';
            messageId = 'successMessage';
            break;
        case 'error':
            toastId = 'errorToast';
            messageId = 'errorMessage';
            break;
        case 'warning':
            toastId = 'warningToast';
            messageId = 'warningMessage';
            break;
        default:
            console.error('Unknown toast type:', type);
            return;
    }

    $('#' + messageId).text(message);

    const toastElement = $('#' + toastId)[0];
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: delay
    });

    toast.show();
}
async function renderDetailSLCap() {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetDetailPhieu&para1=${$("#CayVai_Selected").val()}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if ($("#CayVai_Selected").val() == null) {
            iziToast.warning({
                message: `Vui lòng quét cây vải`,
                position: 'topRight',
                timeout: 2500
            });
            return
        }
        if (data.length == 0) return
        let html = ``;
        data.map(item => {
            html += `
                <tr >
                          <td>${item.PhieuYC}</td>
                          <td>${$(".malenhip").val()}</td>
                           <td>${$(".mahangip").val()}</td>
                           <td>${$(".khachhangip").val()}</td>
                         <td>${item.SoLo}</td>
                         <td style="min-width: 250px;" class="chitiet">${item.CayVai}</td>
                         <td class="">${parseFloat(parseFloat(item.SLN).toFixed(2))}</td>
                      
                        </td>
                </tr>

            `
        })
        $("#tblDataDetail").html(html)
        const totalTN = data.reduce((acc, cur) => acc + parseFloat(cur.SLN), 0);
        let htmlTfoot = `
              <tr >
                 <td colspan="3">
                     Tổng
                 </td>
            <td colspan="3">
                   
                 </td>
                    <td>  <span style="font-size: 14px;">${parseFloat(totalTN.toFixed(2))}</span>
                </td>
              </tr>

        `
        $("#tfootDetail").html(htmlTfoot)
        $("#myModalDeital").modal("show")
        $("#myModalLabeDetail").text(`Chi tiết thực nhập`)
    } catch (error) {
        console.error(error.message);
        return 0;
    }
}


async function DeleteKien() {
    var barcodegoc = $thisTrDelete.data("barcodegoc")
    var barcode = $thisTrDelete.data("barcode")
    var pxh = $thisTrDelete.find("td.pxh").text().trim()
    $("#myModalD").modal("hide")
    const npl = $thisTrDelete.data("npl")
    const sltru = parseFloat($thisTrDelete.data("slxuat")) || 0
    truSLMaNPL(npl, sltru)
    if ($(".tab3").is(":visible")) {
        var checkDelete = $(".checkboxcapp").is(":checked") ? 1 : 0
        var pxh = $thisTrDelete.data("pxh")

        await Delete(barcode, checkDelete, pxh)
        await GetViewXH()
        await GetMaxPhieu()
        await loadXH()

    }
}

async function Delete(para1, para2, para3) {
    var url = `/api/PhieuXuatHangNPL/Delete?Action=Delete&para1=${para1}&para2=${para2}&para3=${para3}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        if (data == "True") {
            iziToast.success({
                message: `Lưu thành công`,
                position: 'topRight',
                timeout: 2000
            });

        }
        /* await loadXH()*/

    } catch (error) {
        console.error(error.message);
    }
}
function ClearPYC() {
    $("#CayVai_Selected").empty()
    $(".loaivaiip").val("")
    $(".khachhangip").val("")
    $(".malenhip").val("")
    $(".mahangip").val("")

    $(".dacap").val("")
    $(".chuacap").val("")
    $(".ngaytao").val("")
    $("#tblDataBody").empty()
    $(".kienquet").val("")
    $(".thucnhap").val("")
    $("#tfoot").empty()
}


function getDate() {
    //const fp = $(".datepickerT").flatpickr({
    //    enableTime: false,
    //    dateFormat: "d-m-Y",
    //    time_24hr: false,
    //    allowInput: true,
    //    onChange: function (selectedDates, dateStr, instance) {
    //        GetPhieuNhapKho();
    //    }
    //});

    //// Gán giá trị mặc định nhưng KHÔNG trigger sự kiện
}
async function GetXuatHangItemCode() {
    renderTableXuatHang([])
    renderTableItemCode([])
    dataXH = []
    let PhieuCap = $("#soPhieuDK").val()
    var loc = $(".select_loc").val();
    var action = ""
    if (loc == 2) {
        action = "GetDanhSachitemCodeXHNLPYC"
        PhieuCap = loc
        para1PhieuCap = $("#MaLenhSX").val()
        para2PhieuCap = 1
    } else {
        action = "GetDanhSachitemCodeXH"
    }
    var url = `/api/PhieuXuatHangNPL/Get?Action=${action}&para1=${para1PhieuCap}&para2=${para2PhieuCap}&para3=${para3PhieuCap}&para4=${PhieuCap}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        checkChangeTableDev = false
        renderTableItemCode(data)
        createViewDxThongTinItemCode(data)
    } catch (error) {
        console.error(error.message);
    }
}
async function GetPhieuCap(para1, para2, para3) {
    para1PhieuCap = para1
    para2PhieuCap = para2
    para3PhieuCap = para3
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetPhieuCapVT&para1=${para2}&para2=1`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        let html = ``
        data.map(x => {
            html += `<option value="${x.PhieuDK}">${x.Display}</option>`
        })
        $("#soPhieuDK").html(html)
        GetXuatHangItemCode()
        //checkChangeTableDev = false
        //renderTableItemCode(data)
        //createViewDxThongTinItemCode(data)
    } catch (error) {
        console.error(error.message);
    }
}
async function GetXuatHang(para1, para2, para3) {
    GetPhieuCap(para1, para2, para3)
    renderTableXuatHang([])
    dataXH = []
}
async function GetTongCap(para1, para2, para3) {
    var loc = $(".select_loc").val();
    var $select = $("#MaLenhSX option:selected").data("malenhsx");

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetTongCap&para1=${para1}&para2=${para2}&para3=${para3}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();

        return data[0].SLN ?? 0
    } catch (error) {
        console.error(error.message);
    }
}
function calDaCapChCap(dacapvalue) {
    var tongmet = 0
    var loc = $(".select_loc").val()

    tongmet = $("#MaLenhSX option:selected").data("tongmet")

    var dacap = dacapvalue
    var chuacap = parseFloat(tongmet) - dacap
    $(".dacap").val(parseFloat(dacap.toFixed(2)))
    $(".chuacap").val(parseFloat(chuacap.toFixed(2)))
    $(".tongcap").val(parseFloat(parseFloat(tongmet).toFixed(2)))
}

function returnThucNhap() {
    $("#tblDataBody tr").each(function () {
        let $trRow = $(this)
        if ($trRow.find(".ipcheckbox").is(":checked")) {
            $trRow.find("td.thucnhap").text(0)
        }
        $trRow.find(".btnXoaKien").addClass("hide-important");
    })

}


let html5QrCode;
let lastScanned = null;
let scanCooldown = false;
let isProcessing = false;
function PlayAudio() {
    const beepSound = document.getElementById("beepSound");
    beepSound.currentTime = 0;
    beepSound.play();
}
function PlayAudioError() {
    const beepSoundE = document.getElementById("beepSoundError");
    beepSoundE.currentTime = 0;
    beepSoundE.play();
}

// Bắt đầu quét
let lastScanTime = 0;

function startScan() {
    $("#reader").addClass("active")
    document.getElementById('reader').style.display = 'block';
    document.getElementById('shaded-region').style.display = 'block';

    html5QrCode = new Html5Qrcode("reader");

    html5QrCode.start(
        { facingMode: "environment" },
        {
            fps: 30,
            qrbox: { width: 300, height: 300 }
        },
        (decodedText, decodedResult) => {
            const now = Date.now();
            const timeSinceLastScan = now - lastScanTime;

            if (timeSinceLastScan < 2000) {
                return;
            }

            lastScanTime = now;

            GetCheckBarCode(decodedText)
            $("#QRScan").val("")
        },
        (errorMessage) => {
        }
    ).catch((err) => {
        console.error("Lỗi khi bật camera:", err);
        alert("Không thể mở camera: " + err);
    });
}


// Dừng quét
function stopScan() {
    if (html5QrCode) {
        html5QrCode.stop().then(() => {
            html5QrCode.clear();
            $("#reader").removeClass("active")
            document.getElementById('reader').style.display = 'none';
            document.getElementById('shaded-region').style.display = 'none';  // Ẩn vùng tối

        }).catch(err => {
            console.error("Lỗi khi dừng camera:", err);
        });
    }
}
var checkXuat = ""
async function GetBarCodeNull(para1, para2) {
    var loc = $(".select_loc").val() == 1 ? "2" : "1"
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetCheckBarCodeNull&para1=${para1}&para2=${loc}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        if (data.length) {

            let joined = [...new Set(data.map(obj => obj.PhieuXH))].join(", ");
            checkXuat = `gia công: ${joined}`
        }
        return data.length > 0 ? true : false

    } catch (error) {
        console.error(error.message);
    }
}

async function CheckExit(item, mabarCode) {
    if (!item) {
        var checkXH = await GetBarCodeNull(mabarCode)
        if (checkXH) {

            showToast("warning", `Vật tư này  đã được cấp phát bên ${checkXuat}`);
            PlayAudioError();
            $("#QRScan").val("");
        }
        else {
            showToast("warning", `Barcode không khớp trong danh sách khai báo .`);
            PlayAudioError();
            $("#QRScan").val("");
        }
        return false
    }
    else {
        if (item.isCheck == 1) {
            showToast("warning", `Vật tư ${item.SoKien} đã được cấp phát bên phiếu ${item.PXH}`);
            PlayAudioError();
            $("#QRScan").val("");
            $(".kienquet").val(item.SoKien)
            $(".thucnhap").val(item.SLNhap)
            return false
        }
        else return true
    }
}
async function warningNoti(title) {
    iziToast.warning({
        title: 'Warning',
        message: title,
        position: 'topRight'
    });
    PlayAudioError();
    $("#QRScan").val("");
}

async function AddTbody(item, value) {

    if (checkaddBarCodeHistory == 1) {
        historyScanBarcode.push(item.BarCode)
    }
    $("#tblDataBody").find(".backgroundtd").removeClass("active")
    let html = `
          <tr  data-delete="1" data-dot="${item.Dot}" data-isvuot="2" data-istachkien="${item.IsTachKien}" data-save="0" data-sokiensort='${item.SoKienSort}' data-soloid="${item.SoLoID}" data-madvvt="${item.MaDVVT}" data-npl="${item.MaNPL}"   data-mavt="${item.MaVT}"
                data-barcode="${item.BarCode}"  data-kiengoc="${item.KienGoc}" data-sokien="${item.SoKien}" data-khovai="${item.KhoVaiID}" data-mavtid="${item.MaVTID}"
             data-dvtinh="${item.TenDVVT}" data-iskiemke="${item.IsKiemKe}" data-ngaykiemke="${item.NgayKiemKe}"
            data-ngaynk="${item.NgayNK}"  data-barcodegoc="${item.BarCodeGoc}" data-phieuyc="${item.PYC}" data-slgoc="${item.SLGoc}" data-mauvtid="${item.MauVTID}">
                        <td  class="backgroundtd active" style="max-width: 60px;min-width: 50px;">
                          <div style="display: flex;justify-content: center; align-items: center;">
                            <input type = "checkbox" class="checkXH savecheck" style = "width: 18px; height: 18px;"  checked />
                          </div>
                         </td>
                          <td class="pomua">${item.POMua}</td>
                         <td class="solo">${item.SoLo}</td>
                         <td class="mavt">${item.MaVT}</td>
                         <td style="min-width:300px;max-width: 300px;" class="chitiet">${item.ChiTiet}</td>
                         <td class="mau">${item.MauVT}</td>
                        
                         <td class="solot d-none">${item.SoLoT}</td>
                       <td class="solotbatch">${item.SoLoT}/${item.Batch}</td>
                         <td class="khosize">${item.KhoVai}</td>
                         <td class="donvi">${item.DonVi}</td>
                         <td class="sokien">${item.SoKien}</td>
                        <td class="vto">${item.MaONPL}</td>
                        <td class="slyc">${item.CapPhat}</td>
                         <td class="thucnhap ${value == 2 ? "text-danger" : ""}">${parseFloat(parseFloat(item.SLN).toFixed(2))}</td>
                       
                         <td class="ghichu">${item.GhiChu}</td>
                         <td class="barcode d-none">${item.BarCode}</td>
                        <td style="max-width: 60px;min-width: 50px;" class="">
                            <div style="display: flex;justify-content: center; align-items: center;">
                           <input type= "checkbox" class="checkbc " style = "width: 18px; height: 18px;"  />
                          </div>
                        </td>
                        <td style="background:${item.IsTachKien == 1 ? "#cbe732" : ""}" >
                           <div>
                           <i  style="display:${item.IsTachKien == 1 ? "none" : ""}" class="fas fa-share-alt-square tachkien"></i>
                            </div>
                        </td>
                         <td class="">
                           <div class="${item.CheckCat == "1" ? "d-none" : ""}">
                            <i class="fa-solid fa-rectangle-xmark "></i>
                            </div>
                        </td>
                </tr>

    `;

    $("#tblDataBody").prepend(html);
    var soluongCap = parseFloat($(".dacap").val()) + parseFloat(item.SLN)
    $(".dacap").val(parseFloat(soluongCap.toFixed(2)));
    var metconlai = $(".chuacap").val();

    var soluongCL = parseFloat(metconlai) - parseFloat(item.SLN)
    $(".chuacap").val(parseFloat(soluongCL.toFixed(2)));

    await getTfoot()
    sumSLMaNPL()
    PlayAudio()
    item.isCheck = true
    if (checkaddBarCodeHistory == 1) {
        const result = dataXH.filter(item => historyScanBarcode.includes(item.BarCode));
        getBarCodeScan(result, item.BarCode)
    }

}

async function CheckVuotBarCode() {
    return true
}
var checkAgeVuot = false
var checkVuot = true;
var arrDataVuot = [];
var itemvai;
function GetDataTBVuot(data) {
    arrDataVuot = [];
    let groupedMap = new Map();

    data.forEach(item => {
        const key = `${item.MaVTID}-${item.ChiTiet}`;
        const existing = groupedMap.get(key);

        if (existing) {
            existing.SoMet += item.SoMet || 0;
            existing.MetDaCap += item.MetDaCap || 0;
        } else {
            groupedMap.set(key, {
                MaVTID: item.MaVTID,
                SoMet: item.SoMet || 0,
                MetDaCap: item.MetDaCap || 0,
                ChiTiet: item.ChiTiet,
                CheckVuot: false
            });
        }
    });

    arrDataVuot = Array.from(groupedMap.values());

}
//async function GetCheckBarCode(mabarCode, checkbarcodehis) {
//    let exists = $('#tblDataBody td.barcode').filter(function () {
//        return $(this).text().trim() === mabarCode;
//    }).length > 0;
//    if (exists) {
//        showToast("warning", `Barcode đã tồn tại trong bảng`);
//        PlayAudioError();
//        $("#QRScan").val("");
//        return
//    }

//    var dataXHNew = dataXH.find(x => x.BarCode == mabarCode);
//    var checkExitGoc = await CheckExit(dataXHNew, mabarCode)
//    itemvai = arrDataVuot.find(x => x.MaVTID === dataXHNew.MaVTID)

//    checkaddBarCodeHistory = checkbarcodehis
//    if (!checkExitGoc) return
//    $(".kienquet").val(dataXHNew.SoKien)
//    $(".thucnhap").val(dataXHNew.SLN)

//    var soluongCL = (parseFloat(itemvai.SoMet) - parseFloat(itemvai.MetDaCap)) - parseFloat(dataXHNew.SLN)
//    if (soluongCL < 0 && !itemvai.CheckVuot) {
//        dataItemXH = dataXHNew
//        $('#myModalV').modal('show');
//        let html = `Thực xuất vật tư <span style="color:Red" class="">${itemvai.ChiTiet}</span> cấp vượt số lượng cấp trong phiếu yêu cầu.Bạn có muốn cho phép vượt không! `
//        $(".textVuot").html(html)

//    } else {
//        itemvai.MetDaCap += parseFloat(dataXHNew.SLN)
//        sortTableByMultipleClasses(['solo', 'chitiet', 'sokien']);
//        if (soluongCL < 0) await AddTbody(dataXHNew, 2)
//        else await AddTbody(dataXHNew, 1)
//    }
//}
async function Save(image) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser = userName
    var ArrSave = []
    var ArrPhieuXHKT = []
    var loc = $(".select_loc").val();
    var $select = $("#MaLenhSX");

    var selectedOption = $select.find("option:selected");

    var tenkh = selectedOption.data("khachhang") || null
    var tenhang = selectedOption.data("mahang") || null
    var malenh = selectedOption.data("malenh") || null
    var malenhsx = selectedOption.data("malenhsx") || null
    var maphieu = selectedOption.data("maphieu") || null
    var magop = selectedOption.data("magop") || null
    $("#tblDataBody tr").each(function () {
        let $tr = $(this)
        var isCheckXH = $tr.find("input.checkXH").is(":checked");

        var dataAttrs = $tr.data();
        var soLo = $tr.find(".solo").text().trim();
        var chiTiet = $tr.find(".chitiet").text().trim();
        var mauVT = $tr.find(".mau").text().trim();
        var soKien = $tr.find(".sokien").text().trim();
        var khoSize = $tr.find(".khosize").text().trim();
        var thucNhap = parseFloat($tr.find(".thucnhap").text().trim()) || 0;
        var donVi = $tr.find(".donvi").text().trim();
        var soLot = $tr.find(".solot").text().trim();
        var ghichu = $("#modalGhiChu").val()
        var sokiensort = $tr.data("sokiensort")
        var checkVuot = $tr.find(".thucnhap").hasClass("text-danger") ? 1 : 0
        if (dataAttrs.save == 1 || !isCheckXH)
            return

        const soLoObject = {
            SoLoID: dataAttrs.soloid,
            SoLo: soLo,
            PhieuYC: maphieu,
            MaLenh: malenh,
            MaLenhSX: malenhsx,
            MaGop: magop,
            MaKH: null,
            TenKH: tenkh,
            MaHang: null,
            TenHang: tenhang,
            MaNPL: dataAttrs.npl,
            MaVTID: dataAttrs.mavtid,
            MauVTID: dataAttrs.mauvtid,
            CayVai: chiTiet,
            SoLot: soLot,
            KhoVai: khoSize,
            KhoVaiID: dataAttrs.khovai,
            DonVi: donVi,
            MaDonVi: dataAttrs.madvvt,
            SoKien: soKien,
            KienGoc: dataAttrs.kiengoc,
            SLGoc: dataAttrs.slgoc,
            SLNhap: thucNhap,
            isCheck: 1,
            GhiChu: ghichu,
            BarCodeGoc: dataAttrs.barcodegoc,
            BarCode: dataAttrs.barcode,
            NgayXuatHang: null,
            NguoiXuatHang: checkVuot,
            Moudule: loc,
            SoKienSort: sokiensort,
            Dot: dataAttrs.dot
        };
        const phieuXuatHang = {
            PhieuXH: "",
            SortXH: "",
            BarCode: dataAttrs.barcode,
            Dot: dataAttrs.dot,
            IsNPL: 1,
            KyTen: image,
            PhieuYC: maphieu,
            NgayXH: "",
            UserXH: dataUser
        };
        ArrSave.push(soLoObject)
        ArrPhieuXHKT.push(phieuXuatHang)
    })
    if (ArrSave.length > 0) {
        await ApiSave(ArrSave)
        await ApiSaveKiTen(ArrPhieuXHKT, "PostPXHKT")
    }

}

function getTfoot() {
    let sumSLN = $('#tblDataBody td.thucnhap').map(function () {
        return parseFloat($(this).text().replace(/,/g, '')) || 0;
    }).get().reduce((a, b) => a + b, 0);


    let sumSLNT = dataXH.filter(x => x.isCheck === 1).reduce((acc, cur) => acc + parseFloat(cur.SLN), 0);


    let iconmauCP = `<i style="font-size: 12px;padding: 0 2px;color: #576cb7;" class="fas fa-text-size"></i>`;
    let iconmauKC = `<i style="font-size: 12px;padding: 0 2px;color: #CBE732;" class="fa-solid fa-circle"></i>`;
    let iconmauMQ = `<i style="font-size: 12px;padding: 0 2px;color: #DAB1EF;" class="fa-solid fa-circle"></i>`;
    let iconmauCCP = `<i style="font-size: 12px;padding: 0 2px;color: ;" class="fal fa-circle"></i>`;

    let htmlTfoot = `
              <tr style="height:35px">
                 <td style="text-align: left; padding-left: 10px;" colspan="11">
                    Ghi chú: ${iconmauCP} đã cấp phát -- ${iconmauCCP} chưa cấp phát-- ${iconmauKC} kiện chia -- ${iconmauMQ} đang quét
                 </td>
                 <td colspan="1">
                     Tổng
                 </td>
                <td> <span class="tfootDaCap" style="font-size: 14px;">${parseFloat(sumSLN.toFixed(2).toLocaleString())}</span </td>
                <td> <span class="tfootDaCapt d-none" style="font-size: 14px;">${parseFloat(sumSLNT.toFixed(2).toLocaleString())}</span </td>
                <td colspan="5"> </td>
              </tr>

        `
    $("#tfoot").html(htmlTfoot)
}
function sortTableByMultipleClasses(classArray) {
    const $tbody = $("#tblDataBody");
    const $rows = $tbody.find("tr").get();

    $rows.sort(function (a, b) {
        for (let className of classArray) {
            let valA = $(a).find('.' + className).text().trim();
            let valB = $(b).find('.' + className).text().trim();

            // Nếu là số dạng float
            if (!isNaN(parseFloat(valA)) && !isNaN(parseFloat(valB))) {
                valA = parseFloat(valA);
                valB = parseFloat(valB);

                if (valA !== valB) return valA - valB;
            } else {
                // Nếu dạng chữ có định dạng như "abc.1", "abc.2"
                const parseCustom = str => {
                    let parts = str.split('.');
                    return {
                        prefix: parts[0],
                        num: parts.length > 1 ? parseInt(parts[1]) : 0
                    };
                };

                let parsedA = parseCustom(valA);
                let parsedB = parseCustom(valB);

                if (parsedA.prefix !== parsedB.prefix) {
                    return parsedA.prefix.localeCompare(parsedB.prefix);
                }

                if (parsedA.num !== parsedB.num) {
                    return parsedA.num - parsedB.num;
                }
            }
        }

        return 0; // Các giá trị đều bằng nhau
    });

    // Gắn lại các hàng đã sắp xếp
    $.each($rows, function (i, row) {
        $tbody.append(row);
    });
}

async function ApiSave(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser = userName

    const request = new Request(`/api/PhieuXuatHangNPL/Post?action=Post&para1=${dataUser}`, {
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

        await GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
        await GetXuatHangItemCode()
    }
    else {
        iziToast.warning({
            message: `Lưu thất bại`,
            position: 'topRight',
            timeout: 2500,

        });
        await PlayAudioError()
        return
    }
}
async function loadXH() {
    var loc = $(".select_loc").val()
    var $select = $("#MaLenhSX");
    var selected = $select.find("option:selected");
    if (loc == 1) {
        var maphieu = $select.val();
        GetXuatHang(maphieu, '', loc);
    } else {
        var magop = selected.data("magop") || "";
        var malenhsx = selected.data("malenhsx") || "";
        GetXuatHang(magop, malenhsx, loc);
    }
}


//$(document).on('keydown', async function (e) {

//    if (e.key === 'Enter') {
//        if ($('#QRScan:focus').length > 0) {
//            var barcodeNow = e.target.value.trim();

//            GetCheckBarCode(e.target.value.trim())
//            $("#QRScan").val("")
//        }
//        else {
//            iziToast.warning({
//                title: 'Warning',
//                message: 'Vui lòng chọn vào ô QR scan.',
//                position: 'topRight'

//            });
//        }

//    }
//});

function inbarcode() {

}



// tab2 -------------------
function ChangeCapPhat(value) {
    var loc = $(".select_loc").val()
    $("#MaLenhSX").val(value).trigger("change")
    $(".tab1").hide()
    $(".tab2").show()
    $(".tab3").hide()
    $("#capphat-tab").trigger("click")
    $("#QRScan").focus()
}
async function GetMaxPhieu() {
    var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetMaxPXH`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        $(".phieuxh").val(`PXH_${data[0].PXH}`)

    } catch (error) {
        console.error(error.message);
    }
}

function renderSLMetLoc() {
    checkChangeTable = 2
    if (checkChangeTableDev) {
        $("#myModalS").modal("show")
        return;
    } else {
        ChangeLoc()

    }

}
function ChangeLoc() {
    var loc = $(".select_loc").val()
    GetDataTBVuot(dataCheckVuot)
    valueSelect = $("#MaLenhSX").val()

    var $select = $("#MaLenhSX");
    var selected = $select.find("option:selected");

    var dacap = selected.data("dacap") || "";
    var chuacap = selected.data("chuacap") || "";
    var khachhang = selected.data("khachhang") || "";
    var mahang = selected.data("mahang") || "";
    var ngaytao = selected.data("ngaytao") || "";
    var malenh = selected.data("malenh") || "";
    var tongmet = selected.data("tongmet") || "";

    var magop = selected.data("magop") || "";
    var malenhsx = selected.data("malenhsx") || "";
    GetXuatHang(magop, malenhsx, loc);


    $(".khachhangip").val(khachhang)
    $(".malenhip").val(malenh)
    $(".mahangip").val(mahang)
    $(".ngaytao").val(ngaytao)
    $(".tongcap").val(parseFloat(parseFloat(tongmet).toFixed(2)))
    var loc = $(".select_loc").val();
    var $select = $("#MaLenhSX");
    let $tr = $(`#tbodyA tr[data-value="${$select.val()}"]`);
    $tr.trigger("click");
    GetMaxPhieu()
}
async function GetPhieuNhapKho() {

    var todate = moment($("#dateInput").val(), "DD-MM-YYYY").format("YYYY-MM-DD")
    var fromdate = moment($("#dateInput2").val(), "DD-MM-YYYY").format("YYYY-MM-DD")
    var loc = $(".select_loc").val()
    var selectView = $(".selectView").val()
    let actionPYC = ""
    if (loc == 0) {

        $(".tabGiaC").show()
        $(".tabToC").hide()
        $(".optionCat").val("Sản xuất")
        actionPYC = "GetPhieuYeuCau"
        $(".txtMaLenh").text("Mã lệnh")
        $(".phieudangkycap").show()
        $(".item_dh").removeClass("col-lg-6")
        $(".item_dh").addClass("col-lg-7")
        $(".item_detailcapphat").removeClass("col-lg-6")
        $(".item_detailcapphat").addClass("col-lg-5")
        $(".tblngay").show()
        $(".colPhieuCap").removeClass("col-md-4 col-lg-5 col-xl-7")
        $(".colPhieuCap").addClass("col-md-2 col-lg-2 col-xl-4")
        $(".btnEX").addClass("d-flex")
        $(".btnEX").removeClass("d-none")
    }
    else if (loc == 2) {
        $(".tblngay").hide()
        actionPYC = "GetPhieuYeuCauMayMau"
        $(".txtMaLenh").text("Phiếu yêu cầu")
        $(".phieudangkycap").hide()
        $(".item_dh").removeClass("col-lg-7")
        $(".item_dh").addClass("col-lg-6")
        $(".item_detailcapphat").removeClass("col-lg-5")
        $(".item_detailcapphat").addClass("col-lg-6")
        $(".colPhieuCap").removeClass("col-md-2 col-lg-2 col-xl-4")
        $(".colPhieuCap").addClass("col-md-4 col-lg-5 col-xl-7")
        fromdate = 1
        $(".btnEX").removeClass("d-flex")
        $(".btnEX").addClass("d-none")
    }
    else {
        $(".tblngay").hide();
        actionPYC = "GetPhieuYeuCau"
        $(".optionCat").val("Gia công")
        $(".phieudangkycap").show()
        $(".item_dh").removeClass("col-lg-6")
        $(".item_dh").addClass("col-lg-7")
        $(".item_detailcapphat").removeClass("col-lg-6")
        $(".item_detailcapphat").addClass("col-lg-5")
        $(".colPhieuCap").removeClass("col-md-2 col-lg-2 col-xl-4")
        $(".colPhieuCap").addClass("col-md-4 col-lg-5 col-xl-7")
        $(".btnEX").addClass("d-flex")
        $(".btnEX").removeClass("d-none")
    }
    await GetItemCode();

    $(".select_locls").val(loc).trigger("change")
    const url = `/api/PhieuXuatHangNPL/Get?action=${actionPYC}&para1=${todate}&para2=${fromdate}&para3=${loc}&para4=${selectView}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        if (data.length == 0) {
            $("#phieuxuatkho").empty()
            $(".thead-listA").empty()
            $("#tfoot").empty()

            GetDataPhieuXuatKho(1, 1, 1, 1)
            $(".khachhangip").val("")
            $(".malenhip").val("")
            $(".mahangip").val("")
            $(".dacap").val("")
            $(".chuacap").val("")
            $(".kienquet").val("")
            $(".thucnhap").val("")
            $(".tongcap").val("")
            trMaPhieu = "";
            trDH = ""
            $("#tblDataBody").empty()
            renderBody([])
            return;
        }

        renderBody(data)
        let htmlOption = "";

        GetMaxPhieu()
        $(".optionCat").val(loc == 0 ? "Sản xuất" : loc == 2 ? "Phiếu YC NPL" : 'Gia công')
        if (loc == 0 || loc == 1) {
            data.map(x => {
                htmlOption += `
                    <option data-maphieu="A" data-tongmet="${x.SoMet}" data-malenh="${x.MaLenh}" data-ngaytao="" data-mahang="${x.MaHang}" data-khachhang="${x.KhachHang}" data-chuacap="${x.ChuaCap}" data-dacap="${x.MetDaCap}" data-magop="${x.MaGop}" 
                    data-malenhsx="${x.MaLenhSanXuat}" value="${x.Display}">${x.MaLenh}</option>
                
                `
            })
        } else if (loc == 2) {
            data.map(x => {
                htmlOption += `
                    <option  data-mahang="${x.TenHang}" data-khachhang="${x.TenKH}" value="${x.MaPhieu}">${x.SoPhieu}</option>
                
                `
            })
        }

        $("#MaLenhSX").html(htmlOption)

        ChangeLoc()
    } catch (error) {
        console.error(error.message);
    }
}
function renderBody(data) {
    var loc = $(".select_loc").val()
    if (loc == 0 || loc == 1) {
        $("#tbodyA").dxDataGrid({
            dataSource: data,
            keyExpr: 2 == 1 ? "MaPhieu" : "Display", // Khóa
            noDataText: "Chưa có dữ liệu",
            columnAutoWidth: true,
            wordWrapEnabled: true,
            showBorders: true,
            noDataText: "",
            scrolling: { mode: 'standard' },
            filterRow: { visible: true },
            headerFilter: { visible: false },
            paging: {
                enabled: false
            },
            renderAsync: false,
            grouping: { autoExpandAll: true },
            groupPanel: { visible: false },
            loadPanel: {
                enabled: true,
                text: "Đang tải dữ liệu...",
                showIndicator: true,
                showPane: true
            },
            onCellPrepared: function (e) {
                if (e.rowType === "header") {
                    $(e.cellElement).addClass("col-header");
                }
                if (e.rowType === "data") {
                    if (e.column.dataField === "MaLenh") {
                        let checkNgayCap = e.data.CheckNCap;

                        if (checkNgayCap === 0) {
                            $(e.cellElement).addClass("nhapnhay-text");
                        }
                    }
                    $(e.cellElement).addClass("text-center"); // tbody
                }
            },
            onRowPrepared: function (e) {
                if (e.rowType === "data") {
                    let checkNgayCat = e.data.CheckNgayCat;

                    if (checkNgayCat === 0) {
                        $(e.rowElement).addClass("nenxanh");
                    } else if (checkNgayCat === 1) {
                        $(e.rowElement).addClass("nendo");
                    }
                }
            },
            columns: [
                {
                    caption: "Cấp phát",
                    minWidth: 50,
                    cellTemplate: function (container, options) {
                        // Gán các data-* attribute vào tr
                        let $row = $(container).closest("tr");
                        $row.attr("data-value", options.data.Display);
                        $row.attr("data-phieu", options.data.MaDH);
                        $row.attr("data-madh", options.data.MaGop);
                        $row.attr("data-malenh", options.data.MaLenhSanXuat);

                        // Thêm nút
                        $("<div>")
                            .css({
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                height: "25px"
                            })
                            .append(
                                $("<i>")
                                    .addClass("fa-solid fa-square-sliders")
                                    .css({ fontSize: "22px", color: "#4a39c9", cursor: "pointer" })
                                    .on("click", function () {
                                        ChangeCapPhat(options.data.Display);
                                    })
                            )
                            .appendTo(container);
                    }
                },
                {
                    caption: 2 == 1 ? "Mã phiếu" : "Đơn hàng",
                    dataField: 2 == 1 ? "MaPhieu" : "MaDH",
                },
                {
                    caption: "Mã Lệnh",
                    dataField: "MaLenh",
                },
                {
                    caption: "Mã Hàng",
                    dataField: "MaHang",
                },
                {
                    caption: "Khách Hàng",
                    dataField: "KhachHang",
                },
                {
                    caption: "Ngày Vào Chuyền",
                    dataField: "NgayDKVC",
                },
                {
                    caption: "KH cắt",
                    dataField: "NgayCat",
                    minWidth: 90
                },
                {
                    caption: "KH lập trình",
                    dataField: "KHLapTrinh",
                    minWidth: 90
                },
                {
                    caption: "KH may",
                    dataField: "KHMay",
                    minWidth: 90
                },
                {
                    caption: "KH thoát chuyền",
                    dataField: "ThoatChuyen",
                    minWidth: 90
                },
                {
                    caption: "Số Lượng",
                    dataField: "SoLuong",
                    dataType: "number",
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    caption: "Ghi chú",
                    dataField: "GhiChu",
                },
                {
                    caption: "Chi Tiết",
                    minWidth: 50,
                    cellTemplate: function (container, options) {
                        // Thêm nút
                        $("<div>")
                            .css({
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                height: "25px"
                            })
                            .append(
                                $("<i>")
                                    .addClass("fa-solid fa-rectangle-list")
                                    .css({ fontSize: "20px", color: "rgb(0 181 247)", cursor: "pointer" })
                                    .on("click", function (e) {
                                        /*e.stopPropagation()*/
                                        console.log("data: ", options.data)
                                        GetChiTietLenh(options.data.MaLenhSanXuat)
                                    })
                            )
                            .appendTo(container);
                    }
                },

            ],

            onRowClick: function (e) {
                // lấy data của row vừa click
                let rowData = e.data;

                trMaPhieu = rowData.MaDH;
                trDH = rowData.MaGop;
                trMaLenh = rowData.MaLenhSanXuat;

                trloc = $(".select_loc").val();
                madhViewLS = rowData.MaDH
                trMaLenhDisplay = rowData.MaLenh
                trMaGop = rowData.MaGop
                CheckSelect()
                // gọi hàm xử lý
                GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc);

                // highlight row
                $(e.rowElement).closest(".dx-datagrid-rowsview")
                    .find(".dx-row")
                    .removeClass("activeT");
                $(e.rowElement).addClass("activeT");
            },
            onContentReady: function (e) {
                if (data && data.length > 0) {
                    const firstRowData = data[0];
                    if (firstRowData) {
                        // Gọi logic trực tiếp KHÔNG dùng selectRowsByIndexes
                        trMaPhieu = firstRowData.MaDH;
                        trDH = firstRowData.MaGop;
                        trMaLenh = firstRowData.MaLenhSanXuat;
                        trloc = $(".select_loc").val();
                        madhViewLS = firstRowData.MaDH;
                        trMaLenhDisplay = firstRowData.MaLenh
                        trMaGop = firstRowData.MaGop
                        CheckSelect();
                        GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc);

                        // Highlight row đầu tiên KHÔNG dùng selection API
                        setTimeout(() => {
                            const firstRowElement = e.component.getRowElement(0);
                            if (firstRowElement) {
                                $(firstRowElement).closest(".dx-datagrid-rowsview")
                                    .find(".dx-row")
                                    .removeClass("activeT dx-selection");
                                $(firstRowElement)
                                    .removeClass("dx-selection")
                                    .addClass("activeT");
                            }
                        }, 50);
                    }
                }
            }
        });
    }
    else if (loc == 2) {
        $("#tbodyA").dxDataGrid({
            dataSource: data,
            keyExpr: "MaPhieu", // Khóa
            noDataText: "Chưa có dữ liệu",
            columnAutoWidth: true,
            wordWrapEnabled: true,
            showBorders: true,
            noDataText: "",
            scrolling: { mode: 'standard' },
            filterRow: { visible: true },
            headerFilter: { visible: false },
            paging: {
                enabled: false
            },
            renderAsync: false,
            grouping: { autoExpandAll: true },
            groupPanel: { visible: false },
            loadPanel: {
                enabled: true,
                text: "Đang tải dữ liệu...",
                showIndicator: true,
                showPane: true
            },

            columns: [
                {
                    caption: "Cấp phát",
                    minWidth: 50,
                    cellTemplate: function (container, options) {
                        // Gán các data-* attribute vào tr
                        let $row = $(container).closest("tr");
                        $row.attr("data-phieu", options.data.MaPhieu);

                        // Thêm nút
                        $("<div>")
                            .css({
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                height: "25px"
                            })
                            .append(
                                $("<i>")
                                    .addClass("fa-solid fa-square-sliders")
                                    .css({ fontSize: "22px", color: "#4a39c9", cursor: "pointer" })
                                    .on("click", function () {
                                        ChangeCapPhat(options.data.MaPhieu);
                                    })
                            )
                            .appendTo(container);
                    }
                },

                {
                    caption: "Mã Hàng",
                    dataField: "TenHang",
                },
                {
                    caption: "Khách Hàng",
                    dataField: "TenKH",
                },

                {
                    caption: "Số Lượng",
                    dataField: "SoLuong",
                    dataType: "number",
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    caption: "Ghi chú",
                    dataField: "GhiChuTong",
                },


            ],

            onRowClick: function (e) {
                // lấy data của row vừa click
                let rowData = e.data;

                trMaPhieu = rowData.MaPhieu;
                //trDH = rowData.MaGop;
                //trMaLenh = rowData.MaLenhSanXuat;

                trloc = $(".select_loc").val();
                madhViewLS = rowData.MaPhieu
                //trMaLenhDisplay = rowData.MaLenh
                //trMaGop = rowData.MaGop
                CheckSelect()
                // gọi hàm xử lý
                GetDataPhieuXuatKho(trMaPhieu, "", "", trloc);

                // highlight row
                $(e.rowElement).closest(".dx-datagrid-rowsview")
                    .find(".dx-row")
                    .removeClass("activeT");
                $(e.rowElement).addClass("activeT");
            },
            onContentReady: function (e) {
                if (data && data.length > 0) {
                    const firstRowData = data[0];
                    if (firstRowData) {
                        // Gọi logic trực tiếp KHÔNG dùng selectRowsByIndexes
                        trMaPhieu = firstRowData.MaPhieu;

                        trloc = $(".select_loc").val();
                        madhViewLS = firstRowData.MaPhieu;
                        //trMaLenhDisplay = firstRowData.MaLenh
                        //trMaGop = firstRowData.MaGop
                        CheckSelect();
                        GetDataPhieuXuatKho(trMaPhieu, "", "", trloc);

                        // Highlight row đầu tiên KHÔNG dùng selection API
                        setTimeout(() => {
                            const firstRowElement = e.component.getRowElement(0);
                            if (firstRowElement) {
                                $(firstRowElement).closest(".dx-datagrid-rowsview")
                                    .find(".dx-row")
                                    .removeClass("activeT dx-selection");
                                $(firstRowElement)
                                    .removeClass("dx-selection")
                                    .addClass("activeT");
                            }
                        }, 50);
                    }
                }
            }
        });
    }
    $("#Layer_1").click()
}
var dataCheckVuot = []
async function GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, loc) {
    let actionPYC = ""
    if (loc == 2) {
        actionPYC = "GetDataPhieuNhapKhoPhieuYCNPL"
        trDH = 1
    } else {
        actionPYC = "GetDataPhieuNhapKho"
    }


    const url = `/api/PhieuXuatHangNPL/Get?action=${actionPYC}&para1=${trMaPhieu}&para2=${trDH}&para3=${trMaLenh}&para4=${loc}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        GetData(data)
        GetDataTBVuot(data)
        dataCheckVuot = data
        flagCheckApi = false


    } catch (error) {
        console.error(error.message);
    }
}


function GetData(data) {
    arrBaoCao = data;
    var locPYC = $(".select_loc").val();
    const columns = [
        {
            dataField: "ChiTiet",
            caption: "Vật tư",
            groupIndex: 0,

            width: 50,
            cssClass: 'col-khu d-none  mavttd',
        },
        {
            dataField: "TenNhom",
            caption: "Loại vật tư",
            cssClass: 'col-dep text-center tennhom mw-100',
        },
        {
            dataField: "MauVT",
            caption: "Màu",
            cssClass: 'col-th text-center mavttd mw-100',
        },

        {
            dataField: "KhoVai",
            caption: "Widthổ/size",
            cssClass: 'col-th text-center mw-80',
        },
        {
            dataField: "TenDVVT",
            caption: "Đơn vị",
            cssClass: 'col-th text-center mw-80',
            visible: locPYC == 0 || locPYC == 1 ? true : false
        },
        {
            dataField: "SLTon",
            caption: "Tồn kho",
            dataType: "number",
            format: { type: "fixedPoint", precision: 2 },
            cssClass: 'col-th text-center mw-80'
        },

        {
            dataField: "SoMet",
            caption: "Yêu cầu",
            dataType: "number",
            cssClass: 'col-th text-center mw-80',
            customizeText: function (cellInfo) {
                var cellInfo = cellInfo.value == null ? 0 : cellInfo.value.toFixed(2)
                return cellInfo;
            }
        },
        {
            dataField: "MetDaCap",
            caption: "Thực xuất",
            dataType: "number",
            cssClass: 'col-th text-center mw-80 dacattd',
            customizeText: function (cellInfo) {
                var cellInfo = cellInfo.value == null ? 0 : cellInfo.value.toFixed(2)
                return cellInfo;
            }
        },
    ];
    function generateGroupSummary(cols) {
        const groupItems = [];
        function scan(columns) {
            columns.forEach(col => {
                if (col.columns) {
                    scan(col.columns);
                }
                else if (col.dataType === "number" && col.dataField) {
                    const field = col.dataField;

                    if (field == "SoMet") {
                        groupItems.push({
                            name: "SoMet_T_SUM",
                            summaryType: "custom",
                            showInColumn: "SoMet",
                            showInGroupFooter: true,
                            valueFormat: { type: "fixedPoint", precision: 0 },
                            customizeText: e =>
                                e.value != null
                                    ? e.value.toLocaleString("en-EN")
                                    : "0"
                        });
                    }
                    else if (field == "MetDaCap") {
                        groupItems.push({
                            name: "MetDaCapP_T_SUM",
                            summaryType: "custom",
                            showInColumn: "MetDaCap",
                            showInGroupFooter: true,
                            valueFormat: { type: "fixedPoint", precision: 0 },
                            customizeText: e =>
                                e.value != null
                                    ? e.value.toLocaleString("en-EN")
                                    : "0"
                        });
                    }

                }
            });
        }

        scan(cols);
        return groupItems;
    }
    function customSummaryHandler(options) {
        if (options.name === "SoMet_T_SUM") {
            if (options.summaryProcess === "start") {
                options.totalValue = 0;
            }
            if (options.summaryProcess === "calculate") {
                options.totalValue += options.value.SoMet || 0;
            }
        }
        if (options.name === "MetDaCapP_T_SUM") {
            if (options.summaryProcess === "start") {
                options.totalValue = 0;
            }
            if (options.summaryProcess === "calculate") {
                options.totalValue += options.value.MetDaCap || 0;
            }
        }

    }
    const groupCount = arrBaoCao.filter(col => col.ChiTiet).length;
    const totalItems = groupCount >= 2 ? [
        {
            column: "SoMet",
            summaryType: "sum",
            customizeText: function (e) {
                return (e.value || 0).toFixed(2);
            }
        }
    ] : [];

    const totalDaCat = groupCount >= 2 ? [
        {
            column: "MetDaCap",
            summaryType: "sum",
            customizeText: function (e) {
                return (e.value || 0).toFixed(2);
            }
        }
    ] : [];
    const summaries = [...totalItems, ...totalDaCat];
    $("#tblBaoCao").dxDataGrid({
        width: '100%',
        dataSource: arrBaoCao,
        columns: columns,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: true,
        wordWrapEnabled: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
            }
        },

        summary: {
            totalItems: summaries,
            groupItems: generateGroupSummary(columns),
            calculateCustomSummary: customSummaryHandler,
        },


    });

    $("#Layer_1").click()
}
function mergeColumns(classNames) {
    let $rows = $('table tbody tr.dx-column-lines');
    let colSumDaCat = 0;

    classNames.forEach(className => {
        let previousCell = null;
        let rowspan = 1;

        $rows.each(function () {
            let $currentCell = $(this).find('td.' + className);
            let currentText = $currentCell.text().trim();

            if (previousCell === null) {
                previousCell = $currentCell;
                rowspan = 1;
            } else if (currentText === previousCell.text().trim()) {
                rowspan++;
                $currentCell.addClass('d-none');
                previousCell.attr('rowspan', rowspan);
                previousCell.addClass("textMidle");
            } else {
                previousCell = $currentCell;
                rowspan = 1;
            }
        });
    });
    $('table tbody tr.dx-column-lines td.dacattd').not('.d-none').each(function () {
        let val = parseFloat($(this).text().trim().replace(',', ''));
        if (!isNaN(val)) {
            colSumDaCat += val;
        }
    });
    console.log(colSumDaCat)
    $(".dx-datagrid-summary-item[aria-label*='Thực xuất']").text(colSumDaCat);
}
function merge2Class(className1, className2) {
    let $rows = $('table tbody tr.dx-column-lines');
    let previousText = null;
    let previousCell = null;
    let rowspan = 1;
    let colSumDaCat = 0;

    $rows.each(function () {
        let $currentCell1 = $(this).find('td.' + className1);
        let $currentCell2 = $(this).find('td.' + className2);
        let currentText = $currentCell1.text().trim() + '|' + $currentCell2.text().trim();
        let cellValue = parseFloat($currentCell1.text().trim()) || 0;

        if (previousText === null || currentText !== previousText) {
            if ($(this).hasClass('dx-datagrid-group-footer')) return;
            colSumDaCat += cellValue;
        }

        if (previousText === null) {
            previousText = currentText;
            previousCell = $currentCell1;
            rowspan = 1;
        } else if (currentText === previousText) {
            rowspan++;
            $currentCell1.addClass('d-none');
            previousCell.attr('rowspan', rowspan);
            previousCell.addClass("textMidle");
        } else {
            previousText = currentText;
            previousCell = $currentCell1;
            rowspan = 1;
        }
    });

    $(".dx-datagrid-summary-item[aria-label*='Thực xuất']").text(colSumDaCat);
}

var imageSign, idNguoiKy, scrollPosition;
function CallModal(id) {
    if (id == 2) {
        $(".save-imgA").hide()
    }
    else {
        if (dataXH.length == 0) {
            showToast("warning", "Vui lòng quét kiện để xuất!!");
            return
        }
        $(".save-imgA").show()
    }
    $('#signatureModal').modal('show');
}
//function CalCanavas() {
//    canvas.width = canvas.offsetWidth;
//    canvas.height = canvas.offsetHeight;
//    setCanvasBackground();
//}
//saveImg.addEventListener("click", () => {

//    const link = document.createElement("a"); // creating <a> element
//    link.download = `${Date.now()}.jpg`; // passing current date as link download value
//    imageSign = canvas.toDataURL();



//    $('#ModalSign').modal('hide');
//    const myTimeout = setTimeout(Scroll, 500);
//    if ($(".tab3").is(":visible")) {
//        UpdateKT(imageSign)
//    }
//    else {
//        Save(imageSign)
//    }


//});
//function Scroll() {
//    window.scrollTo(0, scrollPosition);
//}
//$(window).on("resize", function () {
//    $('#ModalSign').modal('hide');
//});
async function ApiSaveKiTen(arrSaveKT, action) {
    const request = new Request(`/api/PhieuXuatHangNPL/PostXHKT?action=${action}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSaveKT),
    });
    let response = await fetch(request)
    let data = await response.json()

    GetMaxPhieu()

    var loc = $(".select_locls").val()
    $(".txtphieuycls").text("Mã lệnh")

    if ($("#phieuxh").val() != madhViewLS) {
        var loc = $(".select_locls").val()
        $(".txtphieuycls").text("Mã lệnh")
        await GetPYCLS(loc)
        await GetXH()
    } else
        GetXH()
}

// tab 3
var madhViewLS;
var pxhData;
$(function () {
    var loc = $(".select_locls").val()
    GetPYCLS(loc)
    $(".txtphieuycls").text("Mã lệnh")
    // tab3
    $(".btn-tab3").on("click", function () {
        checkChangeTable = 3
        if (compareRowsWithInput("#tblDataBody", htmltbodyCurrent)) {
            $(".tab2").hide()
            $(".tab1").hide()
            $(".tab3").show()
        }
        else {
            $("#myModalS").modal("show")
        }

    })
    $(".save-imgA").on("click", function () {
        Save("")
        $('#signatureModal').modal('hide');
    })
    $(".select_locls").on("change", function () {
        var loc = $(".select_locls").val()
        $(".txtphieuycls").text("Mã lệnh")
        GetPYCLS(loc)
    })
    $("#phieuycls").on("change", function () {
        /*  if ($(this).val() != madhViewLS)*/
        GetXH()
    })
    $("#phieuxh").on("change", function () {
        GetViewXH()
    })
    //$("#tbodyA .dx-datagrid-rowsview").on("click", "tr", function () {

    //})
    $(document).on("click", ".fa-signature", function () {
        pxhData = $(this).closest("tr").data("pxh")
        CallModal(2)
    })
    $(".btn-tab3").on("click", function () {
        GetViewXH()
    })
})


async function GetPYCLS(para) {
    var actionPYC = ""
    var loc = $(".select_locls").val()
    if (loc == 2) {
        actionPYC = "GetPYCNLMayMau"
        para = 1
    } else {
        actionPYC = "GetPYC"
    }
    var url = `/api/PhieuXuatHangNPL/GetTH?Action=${actionPYC}&para1=${para}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) {
            $("#phieuycls").empty()
        }
        let html = ``;
        if (loc == 2) {
            data.map(x => {
                html += `
                <option  value="${x.MaPhieu}">${x.SoPhieu}</option>
                 `
            })
        } else {
            data.map(x => {
                html += `
                <option data-malenh="${x.MaLenh}" data-value2="${x.Value2}" value="${x.Value}">${x.Display}</option>
            `

            })

        }

        $("#phieuycls").html(html)
        CheckSelect()
    } catch (error) {
        console.error(error.message);
    }
}
async function GetXH() {
    var para1 = $("#phieuycls").val()
    var para2 = $("#phieuycls option:selected").data("value2")
    var loc = $(".select_locls").val()
    var actionPYC = ""
    if (loc == 2) {
        actionPYC = "GetPXHNLMM"
        para2 = 1
    } else {
        actionPYC = "GetPXH"
    }

    var url = `/api/PhieuXuatHangNPL/GetTH?Action=${actionPYC}&para1=${para1}&para2=${para2}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) {
            $("#phieuxh").empty()
        }
        let html = `<option  value="all">Tất cả</option>`;
        data.map(x => {
            html += `
                <option  value="${x.PhieuXH}">${x.Display}</option>
            `
        })
        $("#phieuxh").html(html)
        GetViewXH()
    } catch (error) {
        console.error(error.message);
    }
}
var dataEX = []
async function GetViewXH() {
    var para1 = $("#phieuxh").val()
    var para2 = $("#phieuycls").val()
    var para3 = $("#phieuycls option:selected").data("value2")

    var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetViewXH&para1=${para1}&para2=${para2}&para3=${para3}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        dataEX = data
        if (data.length == 0) {
            //$(".phieuxh").empty()
        }
        let html = ``
        let indexCheck = 0;
        dataEX = data

        renderTable(data)
    } catch (error) {
        console.error(error.message);
    }
}
function renderTable(data) {
    var checkIndex = 0;

    var columns = [
        {
            dataField: "PhieuXH",
            caption: "Phiếu XH",
            cssClass: "col-header",
            minWidth: 100,
            groupIndex: 0
        },
        {
            dataField: "POMua",
            caption: "PO Mua",
            cssClass: "col-header",
            minWidth: 100,
        },
        {
            dataField: "MaVT",
            caption: "ItemCode",
            cssClass: "col-header",
            minWidth: 100,
        },
        {
            dataField: "SoLo",
            caption: "Số lô",
            cssClass: "col-header",
            minWidth: 100,
        },
        {
            dataField: "SoLoT",
            caption: "LOT/Batch",
            cssClass: "col-header",
            minWidth: 100,
            width: 100,
        },
        {
            dataField: "MauVT",
            caption: "Màu",
            cssClass: "col-header",
            minWidth: 100,
        },
        {
            dataField: "ChiTiet",
            caption: "Mô tả",
            cssClass: "col-header",
            width: 300,
            minWidth: 200
        },
        {
            dataField: "KhoVai",
            caption: "Width vải",
            cssClass: "col-header",
            minWidth: 100,
        },
        {
            dataField: "DonVi",
            caption: "Đơn vị",
            cssClass: "col-header",
            minWidth: 100,
            width: 100,
        },
        {
            dataField: "SoKien",
            caption: "Vật tư",
            cssClass: "col-header",
            minWidth: 100,
        },

        {
            dataField: "SLNhap",
            caption: "Thực xuất",
            cssClass: "col-header",
            minWidth: 100,
            cellTemplate: function (container, options) {
                const value = parseFloat(parseFloat(options.value || 0).toFixed(2));
                $("<div>")
                    .text(value)
                    .addClass("thucnhap")
                    .toggleClass("text-danger", options.data.isCheckVuot === true)
                    .appendTo(container);
            }
        },
        {
            dataField: "KyTen",
            caption: "Ký tên",
            cssClass: "col-header",
            minWidth: 80,
            cellTemplate: function (container, options) {
                if (!options.data) {
                    return;
                }
                const valueCheck = `${options.data.PhieuXH}`;
                const rowIndexInData = data.findIndex(d => d.PhieuXH === options.data.PhieuXH);
                let rowspan = 0;

                if (checkIndex == 0) {
                    for (let i = rowIndexInData; i < data.length; i++) {
                        var x = data[i];
                        var valueData = `${x.PhieuXH}`;
                        if (valueData == valueCheck) {
                            rowspan++;
                        } else {
                            break; // Sửa lỗi logic: phải break khi khác nhau
                        }
                    }
                }

                const isFirst = (checkIndex == 0);

                if (isFirst) {
                    const containerDiv = $("<div>")
                        .css({
                            width: "100%",
                            height: "40px",
                            position: "relative",
                            display: "flex",
                            justifyContent: "center"
                        });

                    // Thêm icon signature
                    containerDiv.append(
                        $("<i>")
                            .addClass("fa-solid fa-signature")
                            .css({
                                position: "absolute",
                                top: "0",
                                right: "5px",
                                cursor: "pointer"
                            })
                    );

                    // Thêm ảnh ký tên nếu có
                    if (options.data.KyTen && options.data.KyTen !== "") {
                        containerDiv.append(
                            $("<img>")
                                .attr("src", `/Images/XuatHangNPL/KyTen/${options.data.KyTen}?${new Date().getTime()}`)
                                .css({ width: "70px", height: "35px", display: "block" })
                        );
                    }

                    containerDiv.appendTo(container);
                    container.attr("rowspan", rowspan);
                    checkIndex = rowspan;
                } else {
                    container.addClass("d-none");
                }
                checkIndex--;
            }
        },

        {
            dataField: "BarCode",
            caption: "BarCode",
            cssClass: "col-header",
            minWidth: 100,
        },
        {
            dataField: "HuyCat",
            caption: "Hủy cấp",
            cssClass: "col-header",
            width: 100,
            cellTemplate: function (container, options) {
                $("<div>")
                    .css({
                        width: "100%",
                        height: "40px",
                        position: "relative",
                        display: "flex",
                        justifyContent: "center"
                    })
                    .addClass("itemHD")
                    .append(
                        $("<i>")
                            .attr("title", "Hủy xuất")
                            .addClass("fa-solid fa-rectangle-xmark huycatphieu")
                            .toggleClass("d-none", options.data.CheckCat == "1" || checkPQ != 1)
                    )
                    .appendTo(container);
            }
        },
        {
            dataField: "GhiChu",
            caption: "Ghi chú",
            cssClass: "col-header",
            minWidth: 100,
        },
    ];

    // Xóa instance cũ nếu có

    $("#tblDataBodyLS").dxDataGrid({
        dataSource: data,
        columns: columns,
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-pxh", e.data.PhieuXH);
                e.rowElement.attr("data-manpl", e.data.MaNPL);
                e.rowElement.attr("data-cayvai", e.data.ChiTiet);
                e.rowElement.attr("data-dot", e.data.Dot);
                e.rowElement.attr("data-phieuyc", e.data.BarCodeGoc);
                e.rowElement.attr("data-slnhap", e.data.SLNhap);
                e.rowElement.attr("data-barcode", e.data.BarCode);
            }
        },
        columnAutoWidth: true,
        wordWrapEnabled: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        //scrolling: { mode: 'virtual' },
        renderAsync: false,
        summary: {
            totalItems: [
                summaryItem("SoLuongSP"),

            ],
            groupItems: [

                {
                    column: "SoLuongSP", summaryType: "sum", showInGroupFooter: true, alignByColumn: true,
                    customizeText: function (e) {
                        return e.value;
                    }
                },

            ]
        },
        columnAutoWidth: true,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        // Thêm event để đảm bảo sort đúng
        onContentReady: function (e) {
            // Force sort lại nếu cần
            e.component.columnOption("SortXH", "sortOrder", "asc");
        }
    }).dxDataGrid('instance');

    $("#Layer_1").click();
}
function summaryItem(column) {
    return {
        column: column,
        summaryType: "sum",
        valueFormat: "#,##0.####",
        customizeText: function (e) {
            return Number.isInteger(e.value)
                ? DevExpress.localization.formatNumber(e.value, "#,##0")
                : DevExpress.localization.formatNumber(e.value, "#,##0.0000");
        }
    }
}
function checkRowSpan(data, valueI, object) {
    let index = 1;
    for (var i = valueI + 1; i < data.length; i++) {
        let checkValue = `${data[i].PhieuXH}`
        if (checkValue == object) index++;
        else break;
    }
    return index
}


function CheckSelect() {
    var found = $('#phieuycls option').filter(function () {
        return $(this).val() == madhViewLS
    }).length > 0;

    if (found) {
        $('#phieuycls').val(madhViewLS).trigger("change")
    }
    else GetXH()
}

/*function Export() {
    var loc = $(".select_locls").val()
    var malenh = $("#phieuycls option:selected").data("malenh");
    const url = `/api/PhieuXuatHangNPL/GetEX?action=GetEX&para1=${malenh}&para2=${loc}`;
    var link = document.createElement('a');
    var filename = `PhieuXuatHangNL.xlsx`;
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}*/
$(function () {
    $(".enterbarcode").on("click", function () {
        barcodeLocal = $('#QRScan').val().trim()
        barcodeInput();
    })
    $(document).on('keydown', async function (e) {
        if (e.key === 'Enter') {
            if ($('#QRScan:focus').length > 0) {
                barcodeLocal = e.target.value.trim()
                barcodeInput();

            }
            else {
                showToast("warning", "Vui lòng chọn vào ô scan !")
                PlayAudioError()
            }

        }
    });
})
async function barcodeInput() {
    if (barcodeLocal.trim() != "") {
        await GetVTBarCode(barcodeLocal)
        $("#QRScan").val("")
    }
    else {
        showToast("warning", "Vui lòng nhập barcode vào ô QR scan. !")
        PlayAudioError()
    }
}
function renderTableXuatHang(data) {
    data.sort((a, b) => (b.sort || 0) - (a.sort || 0));

    const $grid = $("#gridXuatHang");
    $grid.dxDataGrid({
        dataSource: data,
        keyExpr: "SoLoID",
        sortByGroupSummaryInfo: [],
        columns: [
            {
                caption: "Chọn",
                width: 60,
                allowSorting: false,
                allowFiltering: false,
                cssClass: "backgroundtd",
                cellTemplate: function (container, options) {
                    const item = options.data;
                    const bg = item.BarCode == item.BarCodeGoc ? "" : "#cbe732";
                    $(container).css({
                        "background-color": `${bg}`,
                        "max-width": "60px",
                        "min-width": "50px"
                    });

                    // Thêm class active nếu sort = 2
                    if (item.sort === 2 && item.TachKien != 1) {
                        $(container).addClass("active");
                    }

                    const wrap = $("<div>")
                        .css({ display: "flex", "justify-content": "center", "align-items": "center" })
                        .appendTo(container);
                    $("<input>", {
                        type: "checkbox",
                        class: "checkXH savecheck uncheckTD",
                        style: "width:18px;height:18px;"
                    })
                        .prop("checked", true)
                        .appendTo(wrap);
                }
            },
            { dataField: "POMua", caption: "PO Mua", cssClass: "pomua" },
            { dataField: "SoLo", caption: "Lô", cssClass: "solo" },
            { dataField: "MaVT", caption: "ItemCode", cssClass: "mavt" },
            { dataField: "CayVai", caption: "Mô tả", cssClass: "chitiet", minWidth: 250 },
            { dataField: "MauVT", caption: "Màu", cssClass: "mau" },
            {
                dataField: "LotBatch",
                caption: "LOT/BATCH",
                cssClass: "solotbatch",
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const d = options.data;
                    $(container).text((d.SoLoT || "") + "/" + (d.Batch || ""));
                }
            },
            { dataField: "KhoVai", caption: "Width/size", cssClass: "khosize" },
            { dataField: "TenDVCD", caption: "Đơn vị", cssClass: "donvi" },
            { dataField: "SoKienHienThi", caption: "Vật tư", cssClass: "sokien" },
            { dataField: "MaONPL", caption: "Vị trí Ô", cssClass: "vto" },
            { dataField: "SLYeuCau", caption: "SLYC", cssClass: "slyc" },
            {
                dataField: "SLNhap", caption: "SL Xuất", cssClass: "thucnhap", minWidth: 80,
                cellTemplate: function (container, options) {
                    let SLNhap = parseFloat(options.data.SLNhap.toFixed(4));
                    $("<div>")
                        .text(SLNhap)
                        .appendTo(container);
                }

            },
            { dataField: "GhiChu", caption: "Ghi chú", cssClass: "ghichu", minWidth: 70 },
            { dataField: "BarCode", caption: "Barcode", cssClass: "barcode", visible: false },
            {
                caption: "In BarCode",
                width: 100,
                cellTemplate: function (container, options) {
                    const wrap = $("<div>")
                        .css({ display: "flex", "justify-content": "center", "align-items": "center" })
                        .appendTo(container);
                    $("<input>", {
                        type: "checkbox",
                        class: "checkbc",
                        style: "width:18px;height:18px;"
                    }).appendTo(wrap);
                }
            },
            {
                caption: "Tách vật tư",
                width: 60,
                cellTemplate: function (container, options) {
                    const item = options.data;
                    const color = item.IsTachKien == 1 ? "#cbe732" : "";
                    $(container).css("background", color);
                    $("<div>")
                        .append(
                            $("<i>")
                                .addClass("tachkien fas fa-share-alt-square iconTachKien")
                                .css("display", `${item.TachKien == 0 ? "" : "none"}`)
                                .on("click", async function () {
                                    var barCodeCheck = await GetCheckBarCodePhuLieu(item.BarCode);

                                    showConfirmModalTachKien(item, async function () {
                                        if ($("#modalSoLuong").val() == "") {
                                            showToast("warning", "Vui lòng nhập thực xuất kiện chia!!!");
                                            return
                                        }

                                        var itemXH = dataXH.find(x => x.BarCode == item.BarCode);
                                        var itemNew = { ...item };
                                        itemNew.GhiChu = $("#modalGhiChu").val()
                                        var soLuongChia = parseFloat($("#modalSoLuong").val());

                                        itemNew.SLNhap = soLuongChia;
                                        itemXH.SLNhap = itemXH.SLNhap - soLuongChia;


                                        var dataBarCodeChia = dataXH.filter(x =>
                                            x.BarCodeGoc == item.BarCode && x.BarCode.includes(".")
                                        );

                                        if (dataBarCodeChia.length == 0 && barCodeCheck.length == 0) {
                                            // Lần đầu tiên chia
                                            itemNew.BarCode = `${item.BarCode}.1`;
                                            itemNew.SoKienHienThi = `${item.SoKienHienThi}.1`;
                                            itemNew.TachKien = 1
                                        } else {


                                            // Lấy tất cả số sau dấu "." từ danh sách barcode đã chia
                                            var arrPasr = [];

                                            if (Array.isArray(dataBarCodeChia)) {
                                                arrPasr = dataBarCodeChia
                                                    .map(x => {
                                                        if (!x.BarCode || !x.BarCode.includes(".")) return null;
                                                        return parseInt(x.BarCode.split(".").pop(), 10);
                                                    })
                                                    .filter(x => !isNaN(x));
                                            }

                                            // ===== Lấy số từ barcode đang check =====
                                            var arrPasrData = [];

                                            if (barCodeCheck) {
                                                const maxNumber = Math.max(
                                                    0,
                                                    ...barCodeCheck
                                                        .map(x => {
                                                            if (!x.BarCode || !x.BarCode.includes(".")) return null;
                                                            return parseInt(x.BarCode.split(".").pop(), 10);
                                                        })
                                                        .filter(n => !isNaN(n))
                                                );

                                                const result = maxNumber
                                                if (!isNaN(result)) {
                                                    arrPasrData.push(result);
                                                }
                                            }

                                            // ===== Tìm max =====
                                            var maxNumber = arrPasr.length > 0 ? Math.max(...arrPasr) : 0;
                                            var maxNumberSql = arrPasrData.length > 0 ? Math.max(...arrPasrData) : 0;

                                            // ===== Số tiếp theo =====
                                            var nextNumber = Math.max(maxNumber, maxNumberSql) + 1;

                                            // ===== Gán barcode mới =====
                                            itemNew.BarCode = `${item.BarCode}.${nextNumber}`;
                                            itemNew.SoKienHienThi = `${item.SoKienHienThi}.${nextNumber}`;
                                            itemNew.TachKien = 1;

                                        }
                                        dataXH.forEach(item => item.sort = 1);
                                        dataXH.push(itemNew)
                                        renderTableXuatHang(dataXH)

                                    })
                                })
                        )
                        .appendTo(container);
                }
            },
            {
                caption: "Hủy cấp phát",
                width: 80,
                cellTemplate: function (container, options) {
                    const item = options.data;
                    const wrap = $("<div>").addClass("itemHD").appendTo(container);
                    $("<i>")
                        .attr("title", "Hủy xuất")
                        .addClass("fa-solid fa-rectangle-xmark" + (item.CheckCat == "1" ? " d-none" : ""))
                        .on("click", function () {
                            showConfirmModalDelete(item, async function () {
                                if (item.BarCodeGoc == item.BarCode) {
                                    dataXH = dataXH.filter(x => x.BarCode != item.BarCode)
                                    renderTableXuatHang(dataXH)

                                } else {
                                    const itemSLNhap = item.SLNhap
                                    const itemXH = dataXH.find(x => x.BarCode == item.BarCodeGoc)
                                    if (itemXH == null) {
                                        dataXH = dataXH.filter(x => x.BarCode != item.BarCode)
                                        renderTableXuatHang(dataXH)
                                    } else {
                                        itemXH.SLNhap = itemXH.SLNhap + itemSLNhap
                                        dataXH = dataXH.filter(x => x.BarCode != item.BarCode)
                                        renderTableXuatHang(dataXH)
                                    }
                                }


                            })
                        })
                        .appendTo(wrap);
                }
            }
        ],
        summary: {
            totalItems: [
                summaryItem("SLNhap"),
            ],
            groupItems: [
                {
                    column: "SLNhap", summaryType: "sum", showInGroupFooter: true, alignByColumn: true,
                    customizeText: function (e) {
                        return e.value;
                    }
                },
            ]
        },
        columnAutoWidth: true,
        wordWrapEnabled: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: { enabled: false },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
            }
            if (e.rowType === "data" && e.column.dataField === "SLNhap") {
                if (e.data.isCheckVuot === 1) {
                    e.cellElement.css("color", "red");    // tô chữ đỏ
                }
            }
        },
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                const d = e.data;
                const $row = e.rowElement;

                // Tất cả các data-* attributes từ addRowToGrid
                $row.attr("data-delete", 1);
                $row.attr("data-soloid", d.SoLoID);
                $row.attr("data-save", 1);
                $row.attr("data-mavt", d.MaVT);
                $row.attr("data-barcode", d.BarCode);
                $row.attr("data-madvvt", d.MaDVVT || "");
                $row.attr("data-kiengoc", d.KienGoc || "");
                $row.attr("data-sokien", d.SoKienHienThi || 0);
                $row.attr("data-sokiengoc", d.KienGoc || 0);
                $row.attr("data-slnhappress", d.SLN || 0);
                $row.attr("data-manpl", d.MaNPL || 0);
                $row.attr("data-mavtid", d.MaVTID || 0);
                $row.attr("data-mauvtid", d.MauVTID || 0);
                $row.attr("data-khovaiid", d.KhoVaiID || 0);
                $row.attr("data-madvcd", d.MaDVCD || 0);
                $row.attr("data-dvtinh", d.TenDVVT || 0);

                $row.attr("data-ngaynhapkho", d.NgayNhapKho);


                $row.attr("data-ngaykiemke", d.NgayKiemKe || "");
                $row.attr("data-iskiemke", d.IsKiemKe || 0);
                $row.attr("data-tenkh", d.TenKH || "");



                $row.attr("data-isCheckVuot", d.isCheckVuot || 0);
                // Các attr bổ sung từ code cũ (nếu có)
                if (d.NgayNK) $row.attr("data-ngaynk", d.NgayNK);
                if (d.BarCodeGoc) $row.attr("data-barcodegoc", d.BarCodeGoc);
                if (d.SLGoc) $row.attr("data-slgoc", d.SLGoc);
                if (d.Dot) $row.attr("data-dot", d.Dot);

                // Class "thucxuat" nếu SLN != 0
                const sln = Number(parseFloat(d.SLN || 0).toFixed(2));
                if (sln !== 0) {
                    $row.addClass("thucxuat");
                }
            }
        },
        onContentReady: function () {
            sumSLMaNPL()
        }
    });
}
async function GetVTBarCode(barcode, value) {
    const PhieuCap = $("#soPhieuDK").val() ?? ""
    var loc = $(".select_loc").val();
    if (PhieuCap == "" && loc != 2) {
        showToast("warning", `Vui lòng chọn phiếu cấp!`);
        PlayAudioError();
        return;
    }
    var para1 = $("#MaLenhSX option:selected").data("magop")
    var para2 = $("#MaLenhSX option:selected").data("malenhsx")
    var para3 = barcode

    var action = ""
    if (loc == 2) {
        action = "GetXuatHangNLV2PYC"
        para1 = $("#MaLenhSX").val();
    } else {
        action = "GetXHNLV2"
    }

    var url = `/api/PhieuXuatHangNPL/Get?Action=${action}&para1=${para1}&para2=${para2}&para4=${encodeURIComponent(para3)}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        if (data.length == 0) {
            showToast("warning", "BarCode không tồn tại trong danh sách!");
            PlayAudioError();
            return
        } else if (data[0].PhieuXHPL != "") {
            showToast("warning", `Barcode đã được xuất hàng tại ${data[0].PhieuXHPL}!`);
            PlayAudioError();
            return
        } else if (data[0].IsNK == 0) {
            showToast("warning", `Số kiện/roll này chưa được kiểm số lượng!`);
            PlayAudioError();
            return
        }
        var d = data[0];
        if (d.CheckKiemKe == 1) {
            showToast("warning", `Vật tư đang trong quá trình kiểm kê không được xuất hàng!`);
            PlayAudioError();
            return
        }
        if (d.MaONPL == "") {
            showToast("warning", `Số kiện/Roll chưa được đưa lên kệ.Vui lòng nhập vào hệ thống!`);
            PlayAudioError();
            return
        }
        const existingItem = dataXH.find(item => item.BarCode == d.BarCode);

        if (existingItem) {
            showToast("warning", "BarCode đã tồn tại trong danh sách xuất hàng!");
            PlayAudioError();
            return;
        }

        let $tr = $(`#tbodyXHItemCode tr[data-manpl="${d.MaNPL}"]`);

        const total = parseFloat($tr.find("td.slxuat").text().trim() || 0)

        const itemVuotBarCode = dataItemCode.find(x => x.MaNPL == d.MaNPL)
        if (!itemVuotBarCode) {
            showToast("warning", "BarCode không nằm trong danh sách vật tư phiếu cấp!");
            PlayAudioError();
            return
        }


        const totalKienChia = dataXH
            .filter(p => p.BarCodeGoc == d.BarCode)
            .reduce((sum, p) => sum + p.SLNhap, 0);
        d.SLNhap = d.SLNhap - totalKienChia
        let html = `Thực xuất vật tư <span style="color:Red" class="">${itemVuotBarCode.MaVT} / ${d.CayVai}</span> cấp vượt số lượng cấp trong phiếu yêu cầu.Bạn có muốn cho phép vượt không! `

        if (value == 1) {
            historyScanBarcode.push(barcode)
        }
        if (total + d.SLNhap > itemVuotBarCode.CapPhat && itemVuotBarCode.isCheckVuot == 0) {
            showConfirmModal(async function () {
                checkChangeTableDev = true;
                dataXH.forEach(item => item.sort = 1);

                var d = data[0];
                d.sort = 2;
                d.isCheckVuot = 1

                dataXH.push(d);
                PlayAudio();
                $(".kienquet").val(d.SoKienHienThi);
                $(".thucnhap").val(d.SLNhap);

                renderTableXuatHang(dataXH);
                itemVuotBarCode.isCheckVuot = 1
                if (value == 1) {
                    const result = dataXH.filter(item => historyScanBarcode.includes(item.BarCode));
                    getBarCodeScan(result, barcode)
                }
            });
        } else {
            checkChangeTableDev = true;
            dataXH.forEach(item => item.sort = 1);

            var d = data[0];
            d.sort = 2;
            d.isCheckVuot = itemVuotBarCode.isCheckVuot

            dataXH.push(d);
            PlayAudio();
            $(".kienquet").val(d.SoKienHienThi);
            $(".thucnhap").val(d.SLNhap);

            renderTableXuatHang(dataXH);
            if (value == 1) {
                const result = dataXH.filter(item => historyScanBarcode.includes(item.BarCode));
                getBarCodeScan(result, barcode)
            }
        }
        $(".textVuot").html(html)

    } catch (error) {
        console.error(error.message);
    }
}
async function CheckPQ() {
    var para1 = !window.CefSharp ? userNameSave : dataUser = userName


    var url = `/api/PhieuXuatHangNPL/GetTH?Action=CheckPQ&para1=${para1}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        checkPQ = data[0].CheckSua
    } catch (error) {
        console.error(error.message);
    }
}
function renderTableItemCode(data) {
    var loc = $(".select_locls").val()
    dataItemCode = data
    // SORT theo Sort trước khi render
    data = data.sort((a, b) => Number(a.Sort) - Number(b.Sort));

    $("#tbodyXHItemCode").dxDataGrid({
        dataSource: data,

        columns: [

            { dataField: "MaVT", caption: "ItemCode", cssClass: "col-header", minWidth: 100 },
            { dataField: "CapPhat", caption: `${loc == 2 ? "SL yêu cầu" : "Cấp phát"}`, cssClass: "col-header", width: 80 },
            {
                dataField: "SLDK", caption: "SL đăng ký", cssClass: "col-header", width: 80,
                visible: loc == 2 ? false : true,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.SLDK || 0).toFixed(2));
                },
            },
            {
                dataField: "SLXuat",
                caption: "SL xuất",
                cssClass: "col-header slxuat",
                width: 80,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.SLXuat || 0).toFixed(2));
                }
            },
            { dataField: "KhoVai", caption: "Width/size", cssClass: "col-header", minWidth: 100 },
            {
                dataField: "TenDVVT", caption: "Đơn vị", cssClass: "col-header", minWidth: 100,
                visible: loc == 2 ? false : true
            },
            { dataField: "MaONPL", caption: "Vị trí ô", cssClass: "col-header", minWidth: 100 },

        ],

        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: true,
        wordWrapEnabled: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-manpl", e.data.MaNPL);
                e.rowElement.attr("data-slxuat", e.data.SLXuat);
            }
        }

    });
}
function sumSLMaNPL() {
    let sumByNPL = {};

    // Tính tổng theo NPL
    dataXH.forEach(x => {
        let npl = x.MaNPL;
        let val = x.SLNhap;
        if (!sumByNPL[npl]) sumByNPL[npl] = 0;
        sumByNPL[npl] += val;
    })
    // Cập nhật lại SL xuất trong dxDataGrid
    $("#tbodyXHItemCode tr").each(function () {
        const trNPL = $(this).data("manpl");
        const slGoc = parseFloat($(this).data("slxuat") || 0);  // SLXuất gốc từ data attr
        if (trNPL == null) return
        if (sumByNPL[trNPL] !== undefined) {
            const tongMoi = slGoc + sumByNPL[trNPL];
            $(this).find("td.slxuat").text(tongMoi);
        }
    });
    $("#dxThongTinItemCode tr").each(function () {
        const trNPL = $(this).data("manpl");
        const slGoc = parseFloat($(this).data("slxuat") || 0); // SLXuất gốc từ data attr
        if (trNPL == null) return
        if (sumByNPL[trNPL] !== undefined) {
            const tongMoi = slGoc + sumByNPL[trNPL];
            $(this).find("td.slxuat").text(tongMoi);
        }
    });
}
function truSLMaNPL(manpl, sltru) {
    $("#tbodyXHItemCode tr").each(function () {
        const trNPL = $(this).data("manpl");

        if (trNPL == manpl) {
            const slGoc = parseFloat($(this).find("td.slxuat").text()) || 0;  // SLXuất gốc từ data attr
            console.log(slGoc, parseFloat(sltru))
            const tongMoi = slGoc - parseFloat(sltru);
            $(this).find("td.slxuat").text(tongMoi);
        }


    });
    $("#dxThongTinItemCode tr").each(function () {
        const trNPL = $(this).data("manpl");
        if (trNPL == manpl) {
            const slGoc = parseFloat($(this).find("td.slxuat").text()) || 0;
            const tongMoi = slGoc - parseFloat(sltru);
            $(this).find("td.slxuat").text(tongMoi);
        }
    });
}
function showConfirmModal(onConfirm) {

    $('#btnComfirm').off('click');
    // Khi nhấn Đồng ý
    $('#btnComfirm').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        const modalEl = document.getElementById('myModalV');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
    });

    // Hiện modal
    const modal = new bootstrap.Modal(document.getElementById('myModalV'));
    modal.show();
}
function showConfirmModalDelete(item, onConfirm) {

    $('#btnHuyKien').off('click');
    // Khi nhấn Đồng ý
    $('#btnHuyKien').on('click', function () {
        if (typeof onConfirm === 'function') {
            truSLMaNPL(item.MaNPL, item.SLNhap)
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        const modalEl = document.getElementById('myModalD');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
    });

    // Hiện modal
    const modal = new bootstrap.Modal(document.getElementById('myModalD'));
    modal.show();
}
function showConfirmModalDeleteXemPhieu(onConfirm) {

    $('#btnHuyKien').off('click');
    // Khi nhấn Đồng ý
    $('#btnHuyKien').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        const modalEl = document.getElementById('myModalD');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
    });

    // Hiện modal
    const modal = new bootstrap.Modal(document.getElementById('myModalD'));
    modal.show();
}
function showConfirmModalTachKien(item, onConfirm) {
    $("#modalSoLo").text(item.SoLo)
    $("#modalVatTu").text(item.MaVT)
    $("#modalThucNhap").text(item.SLNhap)
    $("#modalMau").text(item.MauVT)

    $("#modalKhoSize").text(item.KhoVai)
    $("#modalDonVi").text(item.TenDVCD)
    $("#modalKienGoc").val(item.SoKienHienThi)
    $("#modalSoLuong").val("")
    $('#btnSaveTachKien').off('click');
    // Khi nhấn Đồng ý
    $('#btnSaveTachKien').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        const modalEl = document.getElementById('myModal');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
    });

    // Hiện modal
    const modal = new bootstrap.Modal(document.getElementById('myModal'));
    modal.show();
}
$("#modalSoLuong").on("input", function () {
    var $this = $(this);
    var currentVal = $this.val();

    var SLTN = parseFloat($("#modalThucNhap").text()) || 0; 9
    var SLTach = parseFloat($("#modalSoKienTach").val()) || 0;
    var SLN = parseFloat(currentVal) * SLTach;

    if (SLN >= SLTN) {
        showToast("warning", "Tổng thực xuất kiện chia phải nhỏ hơn thực xuất kiện gốc!!!");
        $this.val(currentVal.slice(0, -1));
    }

})
$('#myModal').on('shown.bs.modal', function () {
    $('#modalSoLuong').focus(); // Focus khi modal hiển thị xong
});
async function GetCheckBarCodePhuLieu(para1) {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetBarCodeCheckPL&para1=${encodeURIComponent(para1)}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        if (data.length == 0) return ""
        else return data
    } catch (error) {
        console.error(error.message);
    }
}
let canvas, ctx;
let isDrawing = false;
let penColor = '#000000';
let penSize = 2;
let signatureHistory = [];
let currentStroke = [];

$(document).ready(function () {
    canvas = document.getElementById('signatureCanvas');
    ctx = canvas.getContext('2d');

    // Khởi tạo kích thước canvas
    initCanvas();

    // Cấu hình canvas
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';

    // Sự kiện chuột
    $(canvas).on('mousedown', startDrawing);
    $(canvas).on('mousemove', draw);
    $(canvas).on('mouseup', stopDrawing);
    $(canvas).on('mouseleave', stopDrawing);

    // Sự kiện chạm (mobile)
    canvas.addEventListener('touchstart', handleTouchStart, { passive: false });
    canvas.addEventListener('touchmove', handleTouchMove, { passive: false });
    canvas.addEventListener('touchend', stopDrawing, { passive: false });

    // Cập nhật màu từ color picker
    $('#penColor').on('change', function () {
        penColor = $(this).val();
    });

    // Reset canvas khi mở modal
    $('#signatureModal').on('shown.bs.modal', function () {
        initCanvas();
        clearSignature();
    });

    // Resize canvas khi thay đổi kích thước màn hình
    $(window).on('resize', function () {
        if ($('#signatureModal').hasClass('show')) {
            initCanvas();
        }
    });
});

function initCanvas() {
    const container = canvas.parentElement;
    const containerWidth = container.offsetWidth;

    // Xác định kích thước canvas dựa trên màn hình
    if (window.innerWidth < 768) {
        canvas.width = Math.min(containerWidth - 40, 500);
        canvas.height = 250;
    } else {
        canvas.width = Math.min(containerWidth - 40, 700);
        canvas.height = 300;
    }

    // Cấu hình lại context sau khi thay đổi kích thước
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
}

function getMousePos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.clientX - rect.left) * scaleX,
        y: (e.clientY - rect.top) * scaleY
    };
}

function getTouchPos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.touches[0].clientX - rect.left) * scaleX,
        y: (e.touches[0].clientY - rect.top) * scaleY
    };
}

function startDrawing(e) {
    isDrawing = true;
    const pos = getMousePos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function draw(e) {
    if (!isDrawing) return;

    const pos = getMousePos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function stopDrawing() {
    if (isDrawing && currentStroke.length > 0) {
        signatureHistory.push([...currentStroke]);
        currentStroke = [];
    }
    isDrawing = false;
}

function handleTouchStart(e) {
    e.preventDefault();
    isDrawing = true;
    const pos = getTouchPos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function handleTouchMove(e) {
    e.preventDefault();
    if (!isDrawing) return;

    const pos = getTouchPos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function clearSignature() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    signatureHistory = [];
    currentStroke = [];
}

function undoSignature() {
    if (signatureHistory.length === 0) return;

    signatureHistory.pop();
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Vẽ lại tất cả nét còn lại
    signatureHistory.forEach(stroke => {
        if (stroke.length === 0) return;

        ctx.beginPath();
        ctx.moveTo(stroke[0].x, stroke[0].y);

        stroke.forEach((point, index) => {
            if (index === 0) return;
            ctx.strokeStyle = point.color;
            ctx.lineWidth = point.size;
            ctx.lineTo(point.x, point.y);
            ctx.stroke();
        });
    });
}

function setPenColor(color) {
    penColor = color;
    $('#penColor').val(color);
}

function setPenSize(size) {
    penSize = size;
    $('.pen-size-btn').removeClass('active');
    $(`.pen-size-btn[data-size="${size}"]`).addClass('active');
}
function saveSignature() {
    if (signatureHistory.length === 0) {
        alert('Vui lòng ký tên trước khi lưu!');
        return;
    }
    $('#signatureModal').modal('hide');
    // Chuyển canvas thành hình ảnh
    const signatureImage = canvas.toDataURL('image/png');
    if ($(".tab3").is(":visible")) {
        UpdateKT(signatureImage)
    }
    else {
        SaveNLV2(signatureImage)
    }
}

async function UpdateKT(image) {
    var dataUser = !window.CefSharp ? userNameSave : userName
    var ArrPhieuXHKT = []
    const phieuXuatHang = {
        PhieuXH: pxhData,
        SortXH: 1,
        BarCode: "",
        Dot: 1,
        IsNPL: 1,
        KyTen: image,
        PhieuYC: "",
        NgayXH: "",
        UserXH: dataUser
    };
    ArrPhieuXHKT.push(phieuXuatHang)

    await ApiSaveKiTen(ArrPhieuXHKT, "PostUPKT")
}
async function SaveNLV2(signatureImage) {
    var ArrSave = []
    var ArrPhieuXHKT = []
    var $select = $("#MaLenhSX");
    var dataUser = !window.CefSharp ? userNameSave : userName
    const PhieuCap = $("#soPhieuDK").val() ?? ""
    var selectedOption = $select.find("option:selected");
    var loc = $(".select_loc").val();
    var tenkh = selectedOption.data("khachhang") || null
    var tenhang = selectedOption.data("mahang") || null
    var malenh = selectedOption.data("malenh") || null
    var malenhsx = selectedOption.data("malenhsx") || null
    var magop = selectedOption.data("magop") || null

    dataXH.forEach(x => {
        const soLoObject = {
            SoLoID: x.SoLoID,
            SoLo: x.SoLo,
            PhieuYC: loc == 2 ? "" : PhieuCap,
            MaLenh: loc == 2 ? "" : malenh,
            MaLenhSX: loc == 2 ? "" : malenhsx,
            MaGop: loc == 2 ? "" : magop,
            MaKH: loc == 2 ? $select.val() : "",
            TenKH: tenkh,
            MaHang: "",
            TenHang: tenhang,
            MaNPL: x.MaNPL,
            MaVTID: x.MaVTID,
            MauVTID: x.MauVTID,
            CayVai: x.CayVai,
            SoLot: x.SoLoT,
            KhoVai: x.KhoVai,
            KhoVaiID: x.KhoVaiID,
            DonVi: x.TenDVCD,
            MaDonVi: x.MaDVCD,
            SoKien: x.SoKienHienThi,
            KienGoc: x.SoKienHienThi,
            SLGoc: x.SLGoc,
            SLNhap: x.SLNhap,
            isCheck: 1,
            GhiChu: x.GhiChu,
            BarCodeGoc: x.BarCodeGoc,
            BarCode: x.BarCode,
            NgayXuatHang: null,
            NguoiXuatHang: x.isCheckVuot,
            Moudule: loc,
            Dot: x.Dot
        };


        const phieuXuatHang = {
            PhieuXH: "",
            SortXH: "",
            BarCode: x.BarCode,
            Dot: x.Dot,
            IsNPL: 1,
            KyTen: signatureImage,
            PhieuYC: "a",
            NgayXH: "",
            UserXH: dataUser
        };
        ArrSave.push(soLoObject)
        ArrPhieuXHKT.push(phieuXuatHang)
    })

    if (ArrSave.length > 0) {
        await ApiSaveKiTen(ArrPhieuXHKT, "PostPXHKT")
        await ApiSave(ArrSave)

    }
}
//end

//in barcode
function GetDataBarCode() {
    var mavt = $("#cayvai option:selected").data("tenmavt");
    $('.lablebarcode').empty(); // Đúng class
    $('.lablebarcodeB').empty();
    $("#gridXuatHang tr").each(function (index) {
        var $trRow = $(this);
        var checkbarCode = $trRow.find("input.checkbc").is(":checked");
        if (!checkbarCode) return;


        var mavt = $trRow.find(".mavt").text().trim();
        var chitiet = $trRow.find("td.chitiet").text().trim();
        var mau = $trRow.find(".mau").text().trim();
        var sokien = $trRow.data("sokiengoc");
        var thucnhap = $trRow.find(".thucnhap ").text().trim();
        var solot = $trRow.find("td.solotbatch").text().trim();
        var donvi = $trRow.find("td.donvi").text().trim();
        var ghichu = ""; // Nếu có thì thêm giá trị
        var barcodechia = $trRow.data("barcode")
        var mahang = $trRow.data("mahang")
        var ngayNhap = $trRow.data("ngaynhapkho");
        var solo = $trRow.data("solo")
        var pomua = $trRow.find(".pomua").text().trim();
        var khovai = $trRow.find(".khosize").text().trim();
        var gw = $trRow.data("gw")
        var batch = $trRow.data("batch")
        var iskiemke = $trRow.data("iskiemke")
        var ngaykiemke = $trRow.data("ngaykiemke")
        var tenKH = $trRow.data("tenkh")
        var donvivt = $trRow.data("dvtinh")
        var classWinform = ""
        const tenKHLable = tenKH == "" ? "" : ` - KH: ${tenKH}`
        const lableHeader = iskiemke ? `VIKING VIET NAM - Kiểm Kê: ${ngaykiemke}${tenKHLable} ` : `VIKING VIET NAM${tenKHLable}`
        console.log(tenKHLable)
        console.log(lableHeader)
        if (!window.CefSharp) {
            classWinform = ""
        } else {
            classWinform = "winforms";
        }
        const labelHTML = `
  <div class="box-item ${classWinform}">
   <div class="label-container" style="width:378px;height:195px">
     <div class="label-header">${lableHeader}</div>
        <div style="height:28px !important;padding:2px 4px 6px">
             <p style="font-size: 11px;
                width: 100%;
                white-space: nowrap;
                overflow: hidden;
                text-overflow: ellipsis;" class="line-item  mavtbc item_lineNew">
            <span  style="font-weight: bold;margin-right:5px" >PO: </span>
            <span style="font-weight: bold;margin-right:10px" class="me-5 itemsolo">${pomua}</span>
          </p>
        </div>
      <div style="display: flex; justify-content: space-between;padding:2px 4px">
        <div class="left-col" style="width: 75%;">
           <p  class="line-item  mavtbc mb-2 " style="white-space: nowrap;overflow: hidden; text-overflow: ellipsis;">
            <span class="caycaitext">${mavt}</span>
            </p>
          <p style="" class=" viewhchitiet line-item cayvaibc chitietbc"><span>${chitiet}</span></p>

             <p class="" style="height: 20px;white-space: nowrap;overflow: hidden; text-overflow: ellipsis;"
                    class="line-item chitietbc mb-2  spantitle w-100">
                <span style="font-weight: bold;margin-right:5px;font-size:18px">Màu: </span>
                <span class="caycaitext" style="">${mau}</span>
            </p>

          <div class=" line-item item-gapprint item_lineNew gapsokien" style="display:flex;gap:10px !important;height: 18px">
           <p class="text-nowrap">
                 <span class="spantitle text-nowrap">Số lượng: </span>
                 <span class="spanitem text-nowrap">${thucnhap}</span>
                 <span class="spanitemdv text-nowrap">${donvi}</span>
            <p/>
           <p style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; height: 25px; width: 200px;">
             <span style="font-weight: bold;margin-right:1px" class="spantitle">LOT/BATCH:</span>
             <span class="spanitem"> ${solot}</span>
           </p>
          </div>
       
               ${classWinform == "" ? "" : ` <div <p style="white-space: nowrap;overflow: hidden; text-overflow: ellipsis;height: 25px;width:510px">
                <span style="font-weight: bold;margin-right:5px" class="spantitle">QrCode:</span>
                <span class="spanitem">${barcodechia}</span>
            </p></div>`}
        </div>
        <div class="right-col " style="width: 25%; text-align: center;">
         <div class="itemngay w-100">
             <p class="ngaynhap">Ngày nhập:</p>
          <p>${ngayNhap}</p>
        <div style="display: flex; justify-content: center;">
          <div id="qr-${index}" class="qr-code"></div>
        </div>  
        </div>
      </div>
              
   
  </div>
  <div  style="display:flex;gap:5px !important;height: 25px; padding-left: 4px" class="gapslth line-item item_lineNew item-gapprint">
            <p class="text-nowrap" style="white-space: nowrap; height: 25px;">
               <span class="spantitle">Width/Size: </span>
                 <span class="spanitem">${khovai}</span>
                  <span class="spanitemdv">${donvivt}</span>
            </p>
              <p class="text-nowrap" style="overflow: hidden; text-overflow: ellipsis;height: 25px">
                <span style="font-weight: bold;margin-right:5px" class="spantitle">Kiện/Roll:</span>
                <span class="spanitem">${sokien}</span>
            </p>
          </div>
    ${classWinform == "" ? ` <div class="line-item item-gapprint mb-0" style="padding:2px 4px 6px">
            <p style="white-space: nowrap;overflow: hidden; text-overflow: ellipsis;height: 25px;width: 98%">
                <span style="font-weight: bold;margin-right:5px" class="spantitle">QrCode:</span>
                <span class="spanitem">${barcodechia}</span>
            </p>
          </div>` : ""}
  </div>
`;
        var classGap = index % 2 == 0 ? "pe-`" : "ps-1"
        const labelHTMLB = `
         <div class="col-12 col-md-12 col-lg-6 col-xl-6 d-flex justify-content-center ${classGap} py-2">
           <div class="label-container" style="width:378px;height:220px !important">
              <div class="label-header">${lableHeader}</div>
                <div class="w-100" style="padding:0px 4px 4px">
                        <p style="font-size: 11px;
                        height: 15px;
                        width: 100%;
                        white-space: nowrap;
                        overflow: hidden;
                        text-overflow: ellipsis;" class="line-item  mavtbc mb-0 ">
                    <span  style="font-weight: bold;margin-right:5px" >PO: </span>
                    <span style="font-weight: bold;margin-right:10px" class="me-5 itemsolo">${pomua}</span>
                  </p>
                 </div>
              <div style="display: flex; justify-content: space-between;padding:0px 4px">
                 
                <div class="left-col" style="width: 70%;">
                    <p  class="line-item  mavtbc mb-1 " style="white-space: nowrap;overflow: hidden; text-overflow: ellipsis;">
                    <span class="cayvaibc">${mavt}</span>
                    </p>
                  <p style="height: 52px;
                    display: -webkit-box;
                    -webkit-line-clamp: 3;
                    -webkit-box-orient: vertical;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    font-size: 11px;
                    l etter-spacing: 1px;
                    margin-bottom: 2px !important;" class="line-item cayvaibc chitietbc mb-2"><span>${chitiet}</span></p>
                 <p  style="height: 18px" class="line-item chitietbc mb-1">
                    <span class="spantitle" style="font-weight: bold;margin-right:5px">Màu: </span>
                    <span class="cayvaibc" style="">${mau}</span>
                </p>
                  <div class="line-item item-gapprint mb-2" style="display:flex;gap:10px;height: 18px">
                     <p class="text-nowrap">
                         <span class="spantitle text-nowrap">Số lượng: </span>
                         <span class="spanitem text-nowrap">${thucnhap}</span>
                         <span class="spanitemdv text-nowrap">${donvi}</span>
                    <p/>
                   <p style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; height: 15px; width: 138px;">
                     <span style="font-weight: bold;margin-right:5px" class="spantitle">LOT/BATCH:</span>
                     <span class="spanitem"> ${solot}</span>
                   </p>
                  </div>
                </div>
                <div class="right-col " style="width: 30%; text-align: center;">
                 <div class="itemngay w-100">
                     <p class="ngaynhap">Ngày nhập:</p>
                  <p>${ngayNhap}</p>
                <div style="display: flex; justify-content: center;">
                  <div id="qrB-${index}" class="qr-code"></div>
                </div>
              </div>
            </div>
        </div>
        <div  style="display:flex;gap:2px;height: 12px; padding-left: 4px" class="line-item item-gapprint mb-2">
                    <p class="text-nowrap" style="white-space: nowrap; height: 18px;">
                       <span class="spantitle">Width/Size: </span>
                         <span class="spanitem">${khovai}</span>
                         <span class="spanitemdv">${donvivt}</span>
                    </p>
                      <p class="text-nowrap w-30" style="overflow: hidden; text-overflow: ellipsis;height: 15px;padding-left:6px">
                        <span style="font-weight: bold;margin-right:5px" class="spantitle">Kiện/Roll:</span>
                        <span class="spanitem">${sokien}</span>
                    </p>
                  </div>
                <div class="line-item item-gapprint mb-2" style="display:flex;gap:50px;height: 18px;padding:2px 4px 6px">
 
                    <p style="white-space: nowrap;overflow: hidden; text-overflow: ellipsis;height: 20px;width: 98%;">
                        <span style="font-weight: bold;margin-right:5px" class="spantitle">QrCode:</span>
                        <span class="spanitem">${barcodechia}</span>
                    </p>

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

function inbarcode() {
    GetDataBarCode()
    $("#myModalIn").modal("show")
    $('.barcodein').show();
}
/// Kiệt 

function createViewDxThongTinItemCode(data) {
    data = data.sort((a, b) => Number(a.Sort) - Number(b.Sort));

    $("#dxThongTinItemCode").dxDataGrid({
        dataSource: data,

        columns: [
            { dataField: "MaVT", caption: "ItemCode", cssClass: "col-header", minWidth: 100 },
            {
                dataField: "CapPhat", caption: "Cấp phát", cssClass: "col-header", minWidth: 100,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.CapPhat || 0).toFixed(2));
                }
            },
            {
                dataField: "SLXuat",
                caption: "SL xuất",
                cssClass: "col-header slxuat",
                minWidth: 100,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.SLXuat || 0).toFixed(2));
                }
            },
            { dataField: "MaONPL", caption: "Vị trí ô", cssClass: "col-header", minWidth: 100 },

            { dataField: "Sort", caption: "Sort", visible: false }
        ],

        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: true,
        wordWrapEnabled: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-manpl", e.data.MaNPL);
                e.rowElement.attr("data-slxuat", e.data.SLXuat);
            }
        }

    });
}

function createViewDxGridDetailsContainer(data) {

    $("#dxLichSuQuet").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                dataField: "POMua",
                caption: "PO Mua",
                width: 100,
            },
            {
                caption: "LOT/Batch",
                dataField: "SoLotBatch",
                width: 120,
                calculateCellValue: function (row) {
                    return `${row.SoLot ?? ""}/${row.Batch ?? ""}`;
                },
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                width: 90,
            },
            {
                dataField: "SoKienHienThi",
                caption: "Vật tư",
                minWidth: 80,
                width: 140,
            },
            {
                dataField: "SLYeuCau",
                caption: "Số mét",
                width: 80,
            },
            {
                dataField: "SLNhap",
                caption: "SL xuất",
                width: 80,
            }
        ],
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        },
    });

}
function getBarCodeScan(result, itembarcode) {
    const itemScan = result.find(x => x.BarCode == itembarcode)
    setTextAndTitle("#txtPoMua", itemScan.POMua);
    setTextAndTitle("#txtLotBatch", `${itemScan.SoLoT}/${itemScan.Batch}`);
    setTextAndTitle("#txtItemCode", itemScan.MaVT);
    setTextAndTitle("#txtRoll", itemScan.SoKienHienThi);
    setTextAndTitle("#txtSoMet", itemScan.SLYeuCau);
    setTextAndTitle("#txtSoLuongXuat", itemScan.SLNhap);

    createViewDxGridDetailsContainer(result)
}

function setTextAndTitle(selector, value) {
    $(selector).text(value ?? "").attr("title", value ?? "");
}
async function GetEXMaHang() {
    var $select = $("#phieuycls");
    const magop = $select.val()
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetDataEX&para1=${magop}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        return data
    } catch (error) {
        console.error(error.message);
    }
}
async function Export() {
    var dataSize = await GetEXMaHang()
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Xuất Hàng Nguyên Liệu');

    // Định nghĩa các cột
    worksheet.columns = [
        { header: 'STT', key: 'stt', width: 8 },
        { header: 'PO', key: 'po', width: 15 },
        { header: 'Style', key: 'style', width: 25 },
        { header: 'Size Group', key: 'sizeGroup', width: 15 },
        { header: 'Quantity', key: 'quantity', width: 12 },
        { header: 'Material', key: 'material', width: 15 },
        { header: 'Roll', key: 'roll', width: 20 },
        { header: 'QTY', key: 'qty', width: 12 },
        { header: 'USED', key: 'used', width: 12 },
        { header: 'BALANCE', key: 'balance', width: 12 },
        { header: 'Đầu khúc', key: 'dauKhuc', width: 12 },
        { header: 'Lỗi vải (m)', key: 'loiVai', width: 12 }
    ];

    // Style cho header row
    const headerRow = worksheet.getRow(1);
    for (let i = 1; i <= 12; i++) {
        headerRow.getCell(i).font = { bold: true };
        headerRow.getCell(i).alignment = { vertical: 'middle', horizontal: 'center' };
        headerRow.getCell(i).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FFFFFF00' }
        };
    }

    // Thêm dữ liệu vào worksheet
    const maxLen = Math.max(dataEX.length, dataSize.length);

    for (let i = 0; i < maxLen; i++) {
        const item = dataEX[i] || {};
        const sizeData = dataSize[i] || {};

        worksheet.addRow({
            stt: i < dataEX.length ? i + 1 : "",
            po: sizeData.PO || "",
            style: item.TenHang || "",
            sizeGroup: sizeData.TenSize || "",
            quantity: sizeData.SoLuong ?? "",
            material: item.MaVT || "",
            roll: item.SoKien || "",
            qty: item.SLNhap || "",
            used: "",
            balance: "",
            dauKhuc: "",
            loiVai: ""
        });
    }

    // Thêm border cho tất cả các cell có dữ liệu
    worksheet.eachRow((row, rowNumber) => {
        row.eachCell((cell) => {
            cell.border = {
                top: { style: 'thin' },
                left: { style: 'thin' },
                bottom: { style: 'thin' },
                right: { style: 'thin' }
            };
        });

        // Alignment cho data rows
        if (rowNumber > 1) {
            row.alignment = { vertical: 'middle', horizontal: 'center' };
        }
    });

    // Xuất file Excel
    workbook.xlsx.writeBuffer().then(function (buffer) {
        const blob = new Blob([buffer], {
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });

        // Tạo tên file với timestamp
        const timestamp = new Date().toISOString().slice(0, 19).replace(/[:]/g, '-');
        const fileName = `XuatHangNL_${timestamp}.xlsx`;

        // Tải file xuống
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        link.click();

    }).catch(function (error) {
        console.error("Lỗi khi xuất Excel:", error);
    });
}
function updateNotification(maPhieu) {
    const alert = $("#notificationAlert");
    if (maPhieu) {
        alert.className = 'alert alert-primary border-start border-primary border-1';
        alert.innerHTML = '<strong>Thông báo:</strong> Bạn đang chọn phiếu đăng ký vật tư <strong>' + maPhieu + '</strong>';
    } else {
        alert.className = 'alert alert-warning border-start border-warning border-1';
        alert.innerHTML = '<strong>Lưu ý:</strong> Vui lòng chọn phiếu đăng ký vật tư';
    }
}
/// Kiệt
let dxDataGridChiTietLenh;
/// API CALL
async function GetChiTietLenh(maLenhSanXuat) {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetChiTietLenh&para1=${maLenhSanXuat}`;

    try {
        const response = await fetch(url)
        const data = await response.json()
        /// Set Data vào grid
        dxDataGridChiTietLenh.beginUpdate()
        dxDataGridChiTietLenh.option({ dataSource: data })
        dxDataGridChiTietLenh.endUpdate()

        // Hiển thị modal
        $("#modalChiTietLenh").modal("show")
    } catch (err) {
        console.error(err)
    }
}
/// EVENT
$(document).ready(function () {
    CreateViewDxDataGridChiTietLenh();
})

/// DxDataGird
function CreateViewDxDataGridChiTietLenh() {
    dxDataGridChiTietLenh = $("#dxDataGridChiTietLenh").dxDataGrid({
        dataSource: [],
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: true,
        wordWrapEnabled: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center"); // tbody
            }
        },
        columns: [
            {
                caption: "PO",
                dataField: "PO"
            },
            {
                caption: "Đầu Size",
                dataField: "DauSize"
            },
            {
                caption: "Màu",
                dataField: "TenMau"
            },
            {
                caption: "Size",
                dataField: "Size"
            },
            {
                caption: "Số Lượng",
                dataField: "SL"
            }
        ],
        summary: {
            totalItems: [{
                column: "SL",
                summaryType: "sum",
                customizeText(e) {
                    return e.value;
                }
            }]
        }
    }).dxDataGrid("instance")
    $("#Layer_1").click()
}

/// Xuát Excel
async function GetDataChiTietLenh() {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetChiTietLenh&para1=${trMaLenh}`;

    try {
        const response = await fetch(url)
        const data = await response.json()
        lstDataChiTietLenhEX = data
    } catch (err) {
        console.error(err)
    }
}

async function GetThongTinDH() {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetThongTinDonHang&para1=${trMaLenh}`;

    try {
        const response = await fetch(url)
        const data = await response.json()
        thongTinDonHang = data[0]
    } catch (err) {
        console.error(err)
    }
}

let lstDataChiTietLenhEX = [];
let lstDanhSachVatTuEX = [];
let thongTinDonHang = {};

async function Export2() {
    const now = moment().format("DDMMYYYY");
    const fileDisplay = `LSX_${trMaLenhDisplay}_${now}`
    const isNPL = 1;
    const url = `/api/PhieuXuatHangNPL/GetEXXH?action=GetEXXH&para1=${trMaGop}&para2=${trMaLenh}&para3=${isNPL}`;
    try {
        await fetch(url)
            .then(response => response.blob())
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = fileDisplay;
                document.body.appendChild(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            });
    } catch (err) {
        console.log(err)
    }


}


/// kiet-13032026
let dxPhieuCap1;
let dxPhieuCap2;
let dxPhieuCap3;
let dxPhieuCap4;
var batchDataMalenh = [];
var selectedItemCodeItems = [];
var itemCodeJoin = [];

var selectedRowMaLenh;

/// Event
$(document).ready(function () {
    $(".select_loc").select2({
        minimumResultsForSearch: Infinity
    });
    $(".selectView").select2({
        minimumResultsForSearch: Infinity
    });
    createViewDxDataGridPhieuCap1([]);
    createViewDxDataGridPhieuCap2([]);
    createViewDxDataGridPhieuCap3([]);
    createViewDxDataGridPhieuCap4([]);
})
$(function () {

    $("#locPhieuCap").select2({
        dropdownParent: "#modalPhieuCap",
        minimumResultsForSearch: Infinity
    })

    $("#locPhieuCap").on("change", async function () {
        const valueLoc = $(this).val();
        $("#itemCodeInput").val('');
        batchDataMalenh = [];
        itemCodeJoin = [];
        $("#itemList").empty();
        createViewDxDataGridPhieuCap1([])
        createViewDxDataGridPhieuCap2([])

        if (valueLoc == 1) {
            $(".containerGrid1").removeClass("d-none")
            $(".containerGrid2").addClass("d-none")

            $(".colTuNgayPC").addClass("d-none")
            $(".colDenNgayPC").addClass("d-none")

            $(".colCBItemCode").addClass("d-none")
            GetMaLenhChuaCap();
        } else {
            $(".containerGrid1").addClass("d-none")
            $(".containerGrid2").removeClass("d-none")

            $(".colTuNgayPC").removeClass("d-none")
            $(".colDenNgayPC").removeClass("d-none")

            $(".colCBItemCode").removeClass("d-none")
            isFirstChange = true;
            await GetItemCode();
            GetDaCapXuatHang();

        }
    })

    $("#modalPhieuCap").on("show.bs.modal", function () {
        GetMaLenhChuaCap();
    })

    $("#modalPhieuCap").on("hide.bs.modal", function () {
        isFirstChange = true
    })

    $("#tuNgayPhieuYC").on("change", async function () {
        $("#itemCodeInput").val('');
        createViewDxDataGridPhieuCap1([])
        createViewDxDataGridPhieuCap2([])
        isFirstChange = true
        await GetItemCode();
        GetDaCapXuatHang();
    })

    $("#denNgayPhieuYC").on("change", async function () {
        $("#itemCodeInput").val('');
        createViewDxDataGridPhieuCap1([])
        createViewDxDataGridPhieuCap2([])
        isFirstChange = true
        await GetItemCode();
        GetDaCapXuatHang();
    })

    $('#btnShowPhieuYC').on("click", function () {
        showPhieuYC();
    })
    $('#searchitemCodeInput').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        const filtered = batchDataMalenh.filter(item => item.Display.toLowerCase().includes(searchTerm));
        renderListItemCode(filtered);
    });

    $('#checkAll').change(async function () {
        const isChecked = $(this).is(':checked');

        try {
            if (isChecked) {
                selectedItemCodeItems = [...batchDataMalenh];
                itemCodeJoin = selectedItemCodeItems.map(item => item.MaNPL).join(";")
            } else {
                selectedItemCodeItems = [];
                itemCodeJoin = [];
                $("#itemCodeInput").val('')
                createViewDxDataGridPhieuCap1([])
                createViewDxDataGridPhieuCap2([])
            }

            $('#itemList input[type="checkbox"]').prop('checked', isChecked);
            updateInput();

            const loc = $("#locPhieuCap").val();
            if (itemCodeJoin.length > 0) {
                if (loc == 2) {
                    GetDaCapXuatHang();
                }
            }
        } catch (err) {
            console.error(err)
        }

    });

    $(document).on('change', '#itemList input[type="checkbox"]', async function () {
        const itemId = $(this).attr('id');
        const isChecked = $(this).is(':checked');

        if (isChecked) {
            const item = batchDataMalenh.find(x => x.MaNPL == itemId);
            if (item && !selectedItemCodeItems.some(x => x.MaNPL == itemId)) selectedItemCodeItems.push(item);
        } else {
            selectedItemCodeItems = selectedItemCodeItems.filter(x => x.MaNPL != itemId);
        }

        updateCheckAll();
        updateInput();

        const loc = $("#locPhieuCap").val();
        if (selectedItemCodeItems.length > 0) {
            if (loc == 2) {
                GetDaCapXuatHang();
            }
        } else {
            createViewDxDataGridPhieuCap1([]);
            createViewDxDataGridPhieuCap2([]);
        }
    });
    // malenh vs isNPL

    $('#itemCodeInput').click(function (e) {
        e.stopPropagation();
        $('#dropdownList').toggleClass('show');
        $('#searchInput').val('').focus();
        renderListItemCode(batchDataMalenh);
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('.select-container').length) {
            $('#dropdownList').removeClass('show');
        }
    });

})


/// API
async function GetItemCode() {
    try {
        var loc = $(".select_loc").val();

        const tuMoment = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY", true);
        const denMoment = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY", true);

        // ❌ Nếu 1 trong 2 ngày không hợp lệ → không gọi API
        if (!tuMoment.isValid() || !denMoment.isValid()) {
            return;
        }

        // (optional) check từ ngày <= đến ngày
        if (tuMoment.isAfter(denMoment)) {
            return;
        }

        // ✅ Sau khi valid mới format
        const tuNgay = tuMoment.format("YYYY-MM-DD");
        const denNgay = denMoment.format("YYYY-MM-DD");
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetItemDaCapXuatHang&para1=${tuNgay}&para2=${denNgay}&para3=${loc}`;
        const response = await fetch(url)
        const data = await response.json();
        createViewDxDataGridPhieuCap1([])
        createViewDxDataGridPhieuCap2([])
        $("#itemCodeInput").val('');
        batchDataMalenh = data
        renderListItemCode(data)

    } catch (err) {
        console.error(err)
    }
}

async function GetDaCapXuatHang() {
    try {
        var loc = $(".select_loc").val()
        const tuNgay = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const denNgay = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetDaCapXuatHang&para1=${tuNgay}&para2=${denNgay}&para4=${itemCodeJoin}&para5=${loc}`;
        const response = await fetch(url)
        const data = await response.json();

        createViewDxDataGridPhieuCap1(data);

    } catch (err) {
        console.error(err)
    }
}

async function GetMaLenhChuaCap() {
    try {
        var loc = $(".select_loc").val()
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetChuaCapXuatHang&para3=${loc}`;
        const response = await fetch(url)
        const data = await response.json();

        /*  batchDataMalenh = data
          renderListItemCode(data)*/

        createViewDxDataGridPhieuCap4(data)

    } catch (err) {
        console.error(err)
    }
}

async function GetMaLenhDaCap() {
    try {
        const tuNgay = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const denNgay = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")

        var url = `/api/PhieuXuatHangNPL/Get?Action=GetDaCapXuatHang&para1=${tuNgay}&para2=${denNgay}`;
        const response = await fetch(url)
        const data = await response.json();

        /*batchDataMalenh = data
        renderListItemCode(data)*/

        createViewDxDataGridPhieuCap1(data)

    } catch (err) {
        console.error(err)
    }
}

async function GetChiTietLenhChuaCap() {

    try {
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetChiTietLenhChuaCap&para1=${selectedRowMaLenh}&para2=1`;
        const response = await fetch(url);
        const data = await response.json();

        createViewDxDataGridPhieuCap3(data)
    } catch (err) {
        console.error(err)
    }
}
async function GetChiTietLenhDaCap() {
    try {
        const tuNgay = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const denNgay = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const loc = $(".select_loc").val();
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetChiTietLenhDaCap&para1=${selectedRowMaLenh}&para2=1&para3=${loc}&para4=${itemCodeJoin}&para5=${tuNgay}&para6=${denNgay}`;

        const response = await fetch(url);
        const data = await response.json();

        createViewDxDataGridPhieuCap2(data);

    } catch (err) {
        console.error(err)
    }
}

let isFirstChange = true
function renderListItemCode(data) {
    const itemList = $('#itemList');
    itemList.empty();

    // Kiểm tra data có tồn tại và là array không
    if (!data || !Array.isArray(data)) {
        return;
    }

    // Mặc định chọn hết tất cả
    //if (isFirstChange) {
    //    selectedItemCodeItems = [...data];
    //    itemCodeJoin = selectedItemCodeItems.map(item => item.MaNPL).join(";");
    //    updateInput();
    //    isFirstChange = false
    //}

    const html = data.map(item => {
        const isChecked = selectedItemCodeItems.some(selected => selected.MaNPL == item.MaNPL);
        return `
            <div class="dropdown-item">
                <input type="checkbox" 
                       id="${item.MaNPL}"
                       data-display="${item.Display}"
                       ${isChecked ? 'checked' : ''}>
                <label class="mb-0" for="${item.MaNPL}">${item.Display}</label>
            </div>
        `;
    }).join('');

    itemList.append(html);
    updateCheckAll();
}

function updateCheckAll() {
    const total = $('#itemList input[type="checkbox"]').length;
    const checked = $('#itemList input[type="checkbox"]:checked').length;
    $('#checkAll').prop('checked', total > 0 && total == checked);
}

function updateInput() {
    const displayText = selectedItemCodeItems.map(item => item.Display).join(', ');
    $('#itemCodeInput').val(displayText);

    itemCodeJoin = selectedItemCodeItems.map(item => item.MaNPL).join(";")
}

function showPhieuYC() {
    $("#modalPhieuCap").modal("show");
}

/// Helper Function
function formatNumber(value) {
    if (value === undefined || value === null || value === "") return "";

    let str = value.toString();
    let intPart = str;
    let decPart = "";

    if (str.includes(".")) {
        [intPart, decPart] = str.split(".");
        decPart = decPart.substring(0, 4);
    }

    let formattedInt = Number(intPart).toLocaleString();

    return decPart ? `${formattedInt}.${decPart}` : formattedInt;
}

/// DxDatagrid
function createViewDxDataGridPhieuCap1(data) {
    dxPhieuCap1 = $("#dxPhieuCap1").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        selection: {
            mode: 'single',
            showCheckBoxesmode: "onclick",
            allowSelectAll: false,
        },

        columns: [
            {
                dataField: "MaNPL",
                caption: "ItemCode",
                calculateCellValue: function (row) {
                    return row.MaVT;
                },
                groupIndex: 0,
                visible: false,
            },
            { dataField: "MaLenh", caption: "Mã Lệnh", width: 90, },
            { dataField: "TenHang", caption: "Tên Hàng", width: 230, },
            {
                dataField: "SLXuat",
                caption: "SL Xuất",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            {
                dataField: "NgayXuatHang",
                caption: "Ngày Xuất",
                cellTemplate: function (container, options) {
                    const dateFormat = moment(options.value).format("DD/MM/YYYY")
                    container.text(dateFormat)
                },
                minWidth: 120
            },

        ],
        summary: {
            groupItems: [
                {
                    column: "SLXuat", summaryType: "sum", showInGroupFooter: true, alignByColumn: true,
                    customizeText: function (e) {
                        if (!e.value) return '';
                        return formatNumber(e.value);

                    }
                },
            ]
        },
        onContentReady: function (e) {
            const dataSource = e.component.option("dataSource");
            if (dataSource && dataSource.length > 0) {
                const firstDataRow = e.component.getVisibleRows().find(r => r.rowType === 'data');
                if (firstDataRow) {
                    const rowIndex = firstDataRow.rowIndex;
                    selectedRowMaLenh = firstDataRow.data.MaLenhSX;
                    GetChiTietLenhDaCap();

                    // Highlight row đầu tiên
                    setTimeout(() => {
                        const $rowElement = $(e.component.getRowElement(rowIndex));
                        e.component.element().find(".dx-row").removeClass("activeT");
                        $rowElement.addClass("activeT");
                    }, 50);
                }
            }
        },
        onRowClick: function (e) {
            selectedRowMaLenh = e.data.MaLenhSX;
            GetChiTietLenhDaCap();

            // highlight row
            $(e.rowElement).closest(".dx-datagrid-rowsview")
                .find(".dx-row")
                .removeClass("activeT");
            $(e.rowElement).addClass("activeT");
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        },

    }).dxDataGrid('instance');
}
function createViewDxDataGridPhieuCap2(data) {
    dxPhieuCap2 = $("#dxPhieuCap2").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            { dataField: "PhieuXH", groupIndex: 0, caption: "Phiếu Xuất hàng", width: 90, },
            { dataField: "POMua", caption: "PO Mua", width: 100, },
            { dataField: "SoLo", caption: "Số Lô", minWidth: 100, },
            { dataField: "MaVT", caption: "Item Code", width: 90, },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
            { dataField: "SoKienHienThi", caption: "Số Roll", minWidth: 100, },
            { dataField: "SLNhap", caption: "Thực Xuất", minWidth: 100, },
            { dataField: "NgayXuatHang", caption: "Ngày Xuất", minWidth: 100, },
        ],
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }

        },
        summary: {
            groupItems: [
                {
                    column: "SLNhap", summaryType: "sum", showInGroupFooter: true, alignByColumn: true,
                    customizeText: function (e) {
                        if (!e.value) return '';
                        return formatNumber(e.value);

                    }
                },
            ]
        }
    }).dxDataGrid('instance');
}

function createViewDxDataGridPhieuCap3(data) {
    dxPhieuCap3 = $("#dxPhieuCap3").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            { dataField: "PhieuDK", caption: "Phiếu Đăng Ký", groupIndex: 0, visible: false },
            { dataField: "NgayCap", caption: "Ngày YC Cấp", },
            { dataField: "MaVT", caption: "Item Code", width: 90, },
            //{
            //    dataField: "CapPhat",
            //    caption: "Cấp Phát",
            //    width: 100,
            //    cellTemplate: function (container, options) {
            //        container.text(formatNumber(options.value))
            //    }
            //},
            {
                dataField: "SLDK",
                caption: "SL Đăng Ký",
                width: 100,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
            { dataField: "MaONPL", caption: "Vị Trí Ô", minWidth: 100, },
        ],
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        },
    }).dxDataGrid('instance');

}
function createViewDxDataGridPhieuCap4(data) {
    dxPhieuCap4 = $("#dxPhieuCap4").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        selection: {
            mode: 'single',
            showCheckBoxesmode: "onclick",
            allowSelectAll: false,
        },

        columns: [
            { dataField: "MaLenh", caption: "Mã Lệnh", width: 90, },
            { dataField: "TenHang", caption: "Tên Hàng", width: 230, },
        ],
        onContentReady: function (e) {
            const dataSource = e.component.option("dataSource");
            if (dataSource && dataSource.length > 0) {
                const firstDataRow = e.component.getVisibleRows().find(r => r.rowType === 'data');
                if (firstDataRow) {
                    const rowIndex = firstDataRow.rowIndex;
                    selectedRowMaLenh = firstDataRow.data.MaLenhSanXuat;
                    GetChiTietLenhChuaCap();
                    // Highlight row đầu tiên
                    setTimeout(() => {
                        const $rowElement = $(e.component.getRowElement(rowIndex));
                        e.component.element().find(".dx-row").removeClass("activeT");
                        $rowElement.addClass("activeT");
                    }, 50);
                }
            }
        },
        onRowClick: function (e) {
            selectedRowMaLenh = e.data.MaLenhSanXuat;
            GetChiTietLenhChuaCap();

            // highlight row
            $(e.rowElement).closest(".dx-datagrid-rowsview")
                .find(".dx-row")
                .removeClass("activeT");
            $(e.rowElement).addClass("activeT");
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        },
    }).dxDataGrid('instance');

}


$(function () {

    // Khi click tab "Phiếu Yêu Cầu"
    $('#phieuyc-tab').on('shown.bs.tab', function () {
        $(".tab1").show();
        $(".tab2").hide();
        $(".tab3").hide();

        // Buttons
        $(".tabl2A.tab1").css("display", "flex");
        $(".tabl2A.tab2").hide();
        $(".tabl2A.tab3").hide();
    });

    // Khi click tab "Cấp Phát"
    $('#capphat-tab').on('shown.bs.tab', function () {
        $(".tab1").hide();
        $(".tab2").show();
        $(".tab3").hide();

        // Buttons
        $(".tabl2A.tab1").hide();
        $(".tabl2A.tab2").css("display", "flex");
        $(".tabl2A.tab3").hide();
    });

    // Khi click tab "Xem Phiếu"
    $('#xemphieu-tab').on('shown.bs.tab', function () {
        $(".tab1").hide();
        $(".tab2").hide();
        $(".tab3").show();

        // Buttons
        $(".tabl2A.tab1").hide();
        $(".tabl2A.tab2").hide();
        $(".tabl2A.tab3").css("display", "flex");

        // Load data tab 3
        GetViewXH();
    });

    // Mặc định tab 1 active khi load
    $(".tab1").show();
    $(".tab2").hide();
    $(".tab3").hide();
    $(".tabl2A.tab1").css("display", "flex");
    $(".tabl2A.tab2").hide();
    $(".tabl2A.tab3").hide();
});