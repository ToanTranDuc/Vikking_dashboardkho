
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
var $currentRow;
var flagCheckBox = true
var trCheckBox;
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
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
    $(".select_2").select2();
    GetXHPhieu()
    const today = new Date();
    const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
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
        },
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

    $("#PhieuYeuCau_Selected").select2()
    $("#CayVai_Selected").select2()
    $("#tblDataBody").on("click", ".ipcheckbox", function () {
        CheckRow($(this))
    })

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

        } else {
            ChangeLoc()
        }

    })
    $("#btnCheckSave").on("click", function myfunction() {
        htmltbodyCurrent = $("#tblDataBody").html()
        $("#myModalS").modal("hide")
        if (checkChangeTable == 2) {
            var loc = $(".select_loc").val();
            var $select = $("#MaLenhSX");
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

    $("#tblDataBody").on("click", '.fa-rectangle-xmark', function () {
        let $this = $(this).closest("tr")
        $thisTrDelete = $this
        $("#myModalD").modal("show")
    })

    $(".fa-table").on("click", function () {
        renderDetailSLCap()
    })
    $("#CayVai_Selected").on("change", function () {
        GetXuatHang()
    })
    // tab 2 -----------------
    /*  getDate()*/
    $(".fa-calendar-days").on("click", function () {
        $(this).closest("div").find("input.datepicker").focus()
    })

    $(".icon_reload").on("click", function () {
        //GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
        GetPhieuNhapKho()
    })

    //$("#tbodyA").on("click", "tr", function () {
    //    $("#tbodyA").find("tr").removeClass("activeT")
    //    $(this).addClass("activeT")
    //    trMaPhieu = $(this).data("phieu")
    //    trDH = $(this).data("madh")
    //    trMaLenh = $(this).data("malenh")
    //    trloc = $(".select_loc").val()
    //    GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
    //})
    GetPhieuNhapKho();

    $(".checkAllXH").on("click", function () {
        flagCheckBox = true
        checkNhap = true
        checkAllXH()
        getTfoot()
    })
    $('#myModal4').on('shown.bs.modal', function () {
        $('.SuaTNInput').focus(); // Focus khi modal hiển thị xong
    });
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
    $(document).on('click', '.fa-pen-to-square', function () {
        checkNhap = false
        $currentRow = $(this).closest('tr');
        trCheckBox = $currentRow
        var mavtid = trCheckBox.data("mavtid")
        itemvai = arrDataVuot.find(x => x.MaVTID === mavtid)

        let value = $currentRow.find('.inputSLX').val()
        let ghichu = $currentRow.find('td.ghichu').text().trim()
        $('.SuaTNInput').val(value);
        $('.ghichuinput').val(ghichu);
        let sokien = $currentRow.data("sokien")
        let khosize = $currentRow.find(".khosize").text().trim()
        let donvi = $currentRow.find(".donvi").text().trim()
        barcodegoc = $currentRow.data("barcodegoc")
        var slNhapPress = $currentRow.data("slnhappress")
        let theoCT = $currentRow.find(".thucnhapT").text().trim()

        let solot = $currentRow.find('.solot').text().trim();
        let solo = $currentRow.find('.solo').text().trim();
        let vattu = $currentRow.find('.chitiet').text().trim();
        var dongia = $("#cayvai option:selected").data("dongia")
        $('.dongiaMD').val(parseFloat(dongia))
        $('.dongiaMDText').text(parseFloat(dongia))
        $('.khosizeMD').text(khosize)
        $('.donviMD').text(donvi)
        $('.soloMD').text(solo)
        $('.vattuMD').text(vattu)
        $('.sokienS4').text(sokien)
        $('.solotS4').text(solot)
        $('.theoCTS4').text(parseFloat(theoCT) + parseFloat(slNhapPress))

        $('#myModal4').modal('show');
    });
    $(".SuaTNInput ").on("input", function () {
        var $this = $(this);
        var currentVal = $this.val();
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
        var ghichu = $('.ghichuinput').val();
        if ($currentRow) {
            $currentRow.find('.inputSLX').val(_thucnhap);
            $currentRow.find('.ghichu').text(ghichu);
        }
        itemvai.MetDaCap = 0
        flagCheckBox = false
        if (_thucnhap == "")
            CheckVuot(trCheckBox, 2)
        else CheckVuot(trCheckBox)
        getTfoot()
        $('#myModal4').modal('hide');

    });
    $("#tblDataBody").on("change", ".checkXH", async function () {
        let $tr = $(this).closest("tr")
        if (!$(this).is(":checked")) {
            $tr.find("input.inputSLX").val("")
        }
        else {
            flagCheckBox = false
            checkNhap = true
            CheckVuot($tr)

        }
        getTfoot()
    })
    $("#tblDataBody").on("click", ".changeCLCT", function () {
        flagCheckBox = false
        checkNhap = true
        let $tr = $(this).closest("tr")
        let slth = $tr.find("span.slth").text().trim()

        CheckVuot($tr)
        renderBackgroundTN();
        getTfoot()
    })
    $("#btnComfirm").on("click", function () {
        var SLX = trCheckBox.find("input.inputSLX").val()
        let SLCT = trCheckBox.find(".thucnhapT").text().trim()
        itemvai.MetDaCap += parseFloat(SLX == "" ? SLCT : SLX)
        if (SLX == "")
            trCheckBox.find("input.inputSLX").val(SLCT.toLocaleString())
        trCheckBox.find("input.inputSLX").addClass("text-danger")
        trCheckBox.find(".checkXH").prop("checked", true)
        if (!flagCheckBox)
            itemvai.CheckVuot = true;
        else {
            arrDataVuot = arrDataVuot.map(item => ({
                ...item,
                CheckVuot: true  // Gán luôn false cho tất cả
            }));
            checkAllXH()
        }

        $('#myModalV').modal('hide');
        getTfoot()
    })
    $(".btnhuy").on("click", function () {
        if (!checkNhap) {
            var SLX = trCheckBox.find("input.inputSLX").val()
            itemvai.MetDaCap -= parseFloat(SLX)
            trCheckBox.find("input.inputSLX").val("")
        }
    })
    $(".changeall").on("click", function () {
        if ($(this).hasClass("fa-arrow-right")) {
            $(this).removeClass("fa-arrow-right").addClass("fa-xmark");
            setAllTH("fa-arrow-right", "fa-xmark")
        } else {
            $(this).removeClass("fa-xmark").addClass("fa-arrow-right");
            setAllTH("fa-xmark", "fa-arrow-right")
        }
        renderBackgroundTN();
    })
})

function checkAllXH() {
    $("#tblDataBody tr").each(function () {
        if ($(".checkAllXH").is(":checked")) {
            var checkXH = $(this).find(".checkXH ").is(":checked")
            if (checkXH) {
                return
            }
            CheckVuot($(this))
        }
        else {
            var SLX = $(this).find("input.inputSLX").val()
            $(this).find("input.checkXH ").prop("checked", false)
            $(this).find("input.inputSLX").val("")
            var mavtid = $(this).data("mavtid")
            var itemnew = arrDataVuot.find(x => x.MaVTID === mavtid)
            itemnew.MetDaCap -= parseFloat(SLX)
        }
    })
}
function CheckVuot($tr, value2) {
    var mavtid = $tr.data("mavtid")
    var khovaiid = $tr.data("khovai") ?? ""
    var mauvtid = $tr.data("mauvtid") ?? ""
    trCheckBox = $tr

    var itemvainew = arrDataVuot.filter(x => x.MaVTID == mavtid && x.KhoVaiID === khovaiid && x.MauVTID === mauvtid)[0]
    console.log(itemvainew)
    var SLX = $tr.find("input.inputSLX").val()
    let SLCT = $tr.find(".thucnhapT").text().trim()
    var soluongCL = (parseFloat(itemvainew.SoMet) - parseFloat(itemvainew.MetDaCap)) - parseFloat(SLX == "" ? SLCT : SLX)
    if (soluongCL < 0 && !itemvainew.CheckVuot) {
        $('#myModalV').modal('show');
        $tr.find(".checkXH").prop("checked", false)
        let html = `Thực xuất vật tư <span style="color:Red" class="">${flagCheckBox ? "trong bảng" : itemvainew.ChiTiet}</span> cấp vượt số lượng cấp trong phiếu yêu cầu.Bạn có muốn cho phép vượt không! `
        $(".textVuot").html(html)
    } else if (soluongCL < 0 && itemvainew.CheckVuot) {
        $tr.find("input.inputSLX").addClass("text-danger")
        $tr.find(".checkXH").prop("checked", true)
        if (SLX == "")
            $tr.find("input.inputSLX").val(SLCT.toLocaleString())
    }
    else if (value2 == 2) {
        $tr.find("input.inputSLX").val("")
        $tr.find("input.inputSLX").removeClass("text-danger")
        $tr.find(".checkXH").prop("checked", false)
    }
    else {
        $tr.find(".checkXH").prop("checked", true)
        $tr.find("input.inputSLX").removeClass("text-danger")
        if (SLX == "")
            $tr.find("input.inputSLX").val(SLCT.toLocaleString())
    }
}
function renderBackgroundTN() {
    $("#tblDataBody tr").each(function () {
        var SLTN = $(this).find(".thucnhapT").text()
        var SLN = $(this).find("input.inputSLX").val()
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
var valueSelect = "";
function getRowValues(tr) {
    const values = [];
    $(tr).children("td").each(function () {
        const $td = $(this);
        const $input = $td.find("input");

        if ($input.length == 1) {
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





function getDate() {
    const fp = $(".datepicker").flatpickr({
        enableTime: false,
        dateFormat: "d-m-Y",
        time_24hr: false,
        allowInput: true,
        onChange: function (selectedDates, dateStr, instance) {
            GetPhieuNhapKho();
        }
    });

    // Gán giá trị mặc định nhưng KHÔNG trigger sự kiện
    $(".datepicker").val(flatpickr.formatDate(new Date(), "d-m-Y"));
}
function GetDataBarCode() {

}

async function GetXuatHangItemCode() {
    renderTableXuatHang([])
    renderTableItemCode([])
    dataXH = []
    const PhieuCap = $("#soPhieuDK").val()
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetDanhSachitemCodeXH&para1=${para1PhieuCap}&para2=${para2PhieuCap}&para3=${para3PhieuCap}&para4=${PhieuCap}`;
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
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetPhieuCapVT&para1=${para2}&para2=0`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        let html = ``
        data.map(x => {
            html += `<option value="${x.PhieuDK}">PDK: ${x.Dot}</option>`
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
async function GetXuatHangItemCode() {
    renderTableXuatHang([])
    dataXH = []
    const PhieuCap = $("#soPhieuDK").val()
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetDanhSachitemCodeXHPL&para1=${para1PhieuCap}&para2=${para2PhieuCap}&para3=${para3PhieuCap}&para4=${PhieuCap}`;
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
function returnThucNhap() {
    $("#tblDataBody tr").each(function () {
        let $trRow = $(this)
        if ($trRow.find(".ipcheckbox").is(":checked")) {
            $trRow.find("td.thucnhap").text(0)
        }
        $trRow.find(".btnXoaKien").addClass("hide-important");
    })

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
async function Save(img) {
    var ArrSave = []
    var ArrSaveKT = []
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
        var thucNhap = parseFloat($tr.find("input.inputSLX").val()) || 0;
        var donVi = $tr.find(".donvi").text().trim();
        var soLot = $tr.find(".solot").text().trim();
        var ghichu = $tr.find(".ghichu").text().trim();
        var checkVuot = $tr.find(".inputSLX ").hasClass("text-danger") ? 1 : 0
        //if (!isCheckXH) return;

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
            SoLot: 'SLPL',
            KhoVai: khoSize,
            KhoVaiID: dataAttrs.khovai,
            DonVi: donVi,
            MaDonVi: dataAttrs.madvvt,
            SoKien: 'PK_1',
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
            Dot: dataAttrs.dot
        };
        if (thucNhap > 0) {
            const phieuXuatHangKTPL = {
                PhieuXHPL: "PX001",
                PhieuYC: ghichu,
                MaLenh: malenh,
                MaGop: magop,
                MaLenhSX: malenhsx,
                MaNPL: dataAttrs.npl,
                SLNhap: thucNhap,
                Module: 2,
                Sort: "",
                Sign: img,
                NgayXuatHang: "",
                Dot: dataAttrs.dot,
                SoLoID: dataAttrs.soloid
            };
            ArrSaveKT.push(phieuXuatHangKTPL)
        }

        ArrSave.push(soLoObject)
    })
    if (ArrSave.length > 0) {
        await ApiSave(ArrSave)
    }
    if (ArrSaveKT.length > 0) {
        await ApiSavePhieuXHPL(ArrSaveKT)
    }

}
function getTfoot() {
    var loc = $(".select_loc").val();
    var $select = $("#MaLenhSX");
    var tongcap = $select.find("option:selected").data("tongmet")


    let kienXuatCount = $('#tblDataBody .checkXH:checked').length;
    let chuaXuatCount = $('#tblDataBody .checkXH:not(:checked)').length;

    let sumSLN = $('#tblDataBody .thucnhapT').map(function () {
        return parseFloat($(this).text().trim().replace(/,/g, '')) || 0;
    }).get().reduce((a, b) => a + b, 0);

    let sumSLX = $('#tblDataBody input.inputSLX').map(function () {
        return parseFloat($(this).val()) || 0;
    }).get().reduce((a, b) => a + b, 0);
    $(".dacap").val(parseFloat(sumSLX.toFixed(2)))
    var chuacap = tongcap.toFixed(2) - sumSLX.toFixed(2)
    $(".chuacap").val(parseFloat(chuacap.toFixed(2)))
    $(".tongcap").val(parseFloat(tongcap.toFixed(2)))
    let htmlTfoot = `
              <tr >
                 <td class="text-left" colspan="2">
                     Vật tư xuất : <span>${kienXuatCount}</span>
                 </td>
                 <td class="text-left" colspan="">
                      Chưa xuất : <span>${chuaXuatCount}</span>
                 </td>
                 <td colspan="4"></td>
                 <td>${parseFloat(sumSLN.toFixed(2)).toLocaleString()}</td>
                <td colspan="2"></td>
              </tr>

        `
    $("#tfoot").html(htmlTfoot)
    renderBackgroundTN()
}

async function ApiSavePhieuXHPL(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser = userName

    const request = new Request(`/api/PhieuXuatHangNPL/PostPhieuXHPL?action=PostXHPLPhieu&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        await GetXHPhieu()
        var loc = $(".select_loc").val();
        if ($(".tab3").is(":visible")) {
            await GetViewXH()

        } else {
            //await GetPYCPLLS(loc)
            //await loadXH()
            //GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
        }
    }
}
async function ApiSave(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser = userName

    const request = new Request(`/api/PhieuXuatHangNPL/Post?action=PostPhuLieu&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {

        showToast("success", `Lưu thành công`);

        /* await loadXH()*/
        GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
        await GetXuatHangItemCode()
        dataXH = []
    }
    else {
        showToast("warning", `Lưu thất bại`);

        await PlayAudioError()
        return
    }
}
async function loadXH() {
    var loc = $(".select_loc").val()
    var $select = loc == 1 ? $("#PhieuYeuCau_Selected") : $("#MaLenhSX");
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


// tab2 -------------------
function ChangeCapPhat(value) {
    var loc = $(".select_loc").val()
    if (loc == 1) {
        $("#PhieuYeuCau_Selected").val(value).trigger("change")
    }
    else {
        $("#MaLenhSX").val(value).trigger("change")
    }
    $(".tab1").hide()
    $(".tab2").show()
    $("#QRScan").focus()
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
    valueSelect = loc == 1 ? $("#PhieuYeuCau_Selected").val() : $("#MaLenhSX").val()

    var $select = loc == 1 ? $("#PhieuYeuCau_Selected") : $("#MaLenhSX");
    var selected = $select.find("option:selected");

    var dacap = selected.data("dacap") || "";
    var chuacap = selected.data("chuacap") || "";
    var khachhang = selected.data("khachhang") || "";
    var mahang = selected.data("mahang") || "";
    var ngaytao = selected.data("ngaytao") || "";
    var malenh = selected.data("malenh") || "";

    if (loc == 1) {
        var maphieu = $select.val();
        GetXuatHang(maphieu, '', loc);
    } else {
        var magop = selected.data("magop") || "";
        var malenhsx = selected.data("malenhsx") || "";
        GetXuatHang(magop, malenhsx, loc);
    }

    $(".khachhangip").val(khachhang)
    $(".malenhip").val(malenh)
    $(".mahangip").val(mahang)
    $(".ngaytao").val(ngaytao)

    var loc = $(".select_loc").val();
    var $select = $("#MaLenhSX");
    let $tr = $(`#tbodyA tr[data-value="${$select.val()}"]`);
    $tr.trigger("click");
}
async function GetPhieuNhapKho() {

    var todate = moment($("#dateInput").val(), "DD-MM-YYYY").format("YYYY-MM-DD")
    var fromdate = moment($("#dateInput2").val(), "DD-MM-YYYY").format("YYYY-MM-DD")
    var loc = $(".select_loc").val()
    var selectView = $(".selectView").val()
    if (loc == 0) {
        $(".tblngay").hide()
        $(".tabGiaC").show()
        $(".tabToC").hide()
        $(".optionCat").val("Sản xuất")
    }
    else {
        $(".tblngay").hide()
        $(".tabGiaC").show()
        $(".tabToC").hide()
        $(".optionCat").val("Gia công")
    }
    $(".select_locls").val(loc).trigger("change")
    const url = `/api/PhieuXuatHangNPL/Get?action=GetPhieuYeuCauPL&para1=${todate}&para2=${fromdate}&para3=${loc}&para4=${selectView}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        if (data.length == 0) {
            $("#phieuxuatkho").empty()
            $(".thead-listA").empty()
            /* $("#tbodyA").empty()*/
            $("#tfoot").empty()

            GetDataPhieuXuatKho(1, 1, 1, 1)
            $(".khachhangip").val("")
            $(".malenhip").val("")
            $(".mahangip").val("")
            $(".dacap").val("")
            $(".chuacap").val("")
            trMaPhieu = "";
            trDH = ""
            $("#tblDataBody").empty()
            renderBody([])
            return;
        }

        renderBody(data)
        let htmlOption = "";




        $(".optionCat").val(loc == 0 ? "Sản xuất" : "Gia công")
        data.map(x => {
            htmlOption += `
                    <option data-maphieu="A" data-tongmet="${x.SoMet}" data-malenh="${x.MaLenh}" data-ngaytao="" data-mahang="${x.MaHang}" data-khachhang="${x.KhachHang}" data-chuacap="${x.ChuaCap}" data-dacap="${x.MetDaCap}" data-magop="${x.MaGop}" data-malenhsx="${x.MaLenhSanXuat}" value="${x.Display}">${x.Display}</option>
                
                `
        })
        $("#MaLenhSX").html(htmlOption)

        ChangeLoc()
    } catch (error) {
        console.error(error.message);
    }
}
function renderBody(data) {
    var loc = $(".select_loc").val()
    $("#tbodyA").dxDataGrid({
        dataSource: data,
        keyExpr: "Display", // Khóa
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
                //else if (e.data.CheckSH === 0) {
                //    $(e.rowElement).css("background-color", "#d7ffd7");
                //}
            }
        },
        columns: [
            {
                caption: "Cấp phát",
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
                dataField: "CheckSH",
                caption: "S.hàng",
                cssClass: "col-header",
                width: 100,
                alignment: "center",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var status =
                        options.data.XNXong == 1
                            ? '<span class="badge bg-success p-2">Đã soạn xong</span>'
                            : options.value == 0
                                ? '<span class="badge bg-warning p-2">Cần soạn hàng</span>'
                                : '';
                    $(container).html(status);
                }
            },
            {
                caption: "Đơn hàng",
                dataField: "MaDH",
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
                caption: "Ngày Cắt",
                dataField: "NgayCat",
                minWidth: 90
            },
            {
                caption: "BT.Phẩm",
                dataField: "SoMet",
                dataType: "number",
                format: { type: "fixedPoint", precision: 2 }
            }, {
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
            let rowData = e.data;

            trMaPhieu = rowData.MaDH;
            trDH = rowData.MaGop;
            trMaLenh = rowData.MaLenhSanXuat;
            trloc = $(".select_loc").val();
            madhViewLS = rowData.MaDH
            CheckSelect()
            trMaLenhDisplay = rowData.MaLenh
            trMaGop = rowData.MaGop
            $("#txtMaLenh").val(`LSX_${trMaLenhDisplay} - MH: ${rowData.MaHang}`);

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
                    $("#txtMaLenh").val(`LSX_${trMaLenhDisplay} - MH: ${firstRowData.MaHang}`);
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
    $("#Layer_1").click()
}
async function GetPYCPLLS(para) {
    var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetPYCPL&para1=${para}`;
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
        data.map(x => {
            html += `
                <option data-malenh="${x.MaLenh}" data-value2="${x.Value2}" value="${x.Value}">${x.Display}</option>
            `

        })
        $("#phieuycls").html(html)
        CheckSelect()
    } catch (error) {
        console.error(error.message);
    }
}
var madhViewLS;
function CheckSelect() {
    var found = $('#phieuycls option').filter(function () {
        return $(this).val() == madhViewLS
    }).length > 0;
    if (found) {
        $('#phieuycls').val(madhViewLS).trigger("change")
    }
    else GetXH()
}
var dataCheckVuot = []
async function GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, loc) {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetDataPhieuNhapKhoPL&para1=${trMaPhieu}&para2=${trDH}&para3=${trMaLenh}&para4=${loc}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        GetData(data)
        GetDataTBVuot(data)
        flagCheckApi = false
        dataCheckVuot = data

    } catch (error) {
        console.error(error.message);
    }
}
var arrDataVuot = [];
var itemvai;
function GetDataTBVuot(data) {
    arrDataVuot = [];
    let groupedMap = new Map();

    data.forEach(item => {
        const key = `${item.MaVTID}-${item.ChiTiet}-${item.MauVTID}-${item.KhoVaiID}`;
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
                MauVTID: item.MauVTID,
                KhoVaiID: item.KhoVaiID,
                CheckVuot: false
            });
        }
    });

    arrDataVuot = Array.from(groupedMap.values());
}
function GetData(data) {
    arrBaoCao = data;
    const columns = [
        {
            dataField: "ChiTiet",
            caption: "Vật tư",
            groupIndex: 0,

            width: 50,
            cssClass: 'col-khu d-none t mavttd',
        },
        {
            dataField: "TenNhom",
            caption: "Loại vật tư",
            cssClass: 'col-dep text-center mw-100',
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

                    if (field == "SoMetA") {
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
                    else if (field == "MetDaCapA") {
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
        if (options.name === "SoMet_T_SUMA") {
            if (options.summaryProcess === "start") {
                options.totalValue = 0;
            }
            if (options.summaryProcess === "calculate") {
                options.totalValue += options.value.SoMet || 0;
            }
        }
        if (options.name === "MetDaCapP_T_SUMA") {
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
            valueFormat: { type: "fixedPoint", precision: 0 },
            displayFormat: "{0}"
        }
    ] : [];
    const totalDaCat = groupCount >= 2 ? [
        {
            column: "MetDaCap",
            summaryType: "sum",
            valueFormat: { type: "fixedPoint", precision: 0 },
            displayFormat: "{0}"
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
        }
        //summary: {
        //    totalItems: summaries,
        //    groupItems: generateGroupSummary(columns),
        //    calculateCustomSummary: customSummaryHandler,
        //},
    });

    $("#Layer_1").click()
    GetViewXH()
}
function mergeColumns(classNames) {
    let $rows = $('table tbody tr.dx-column-lines');
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

        // Chỉ cộng dồn giá trị của ô đầu tiên trong nhóm gộp
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

    $(".dx-datagrid-summary-item[aria-label*='Số mét đã cắt']").text(colSumDaCat);
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
    const worksheet = workbook.addWorksheet('Xuất Hàng Phụ Liệu');

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
        const fileName = `XuatHangPL_${timestamp}.xlsx`;

        // Tải file xuống
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        link.click();

    }).catch(function (error) {
        console.error("Lỗi khi xuất Excel:", error);
    });

}
//end

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
    //    const myTimeout = setTimeout(CalCanavas, 500);
}


async function GetXHPhieu() {
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetXHPhieu`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        $(".phieuxh").val(`PXHPL-${data[0].Phieu}`)
    } catch (error) {
        console.error(error.message);
        return 0;
    }
}
var pxhData;
$(function () {
    CheckPQ()
    var loc = $(".select_locls").val()
    GetPYCPLLS(loc)
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
    $(".select_locls").on("change", function () {
        var loc = $(".select_locls").val()
        $(".txtphieuycls").text(loc == 1 ? "Phiếu yêu cầu" : "Mã lệnh")
        GetPYCPLLS(loc)
    })
    $("#phieuycls").on("change", function () {
        /*  if ($(this).val() != madhViewLS)*/
        GetXH()
    })
    $("#tbodyA").on("click", "tr", function () {
        //madhViewLS = $(this).data("madh")
        //CheckSelect()
    })
    $(document).on("click", ".fa-signature", function () {
        pxhData = $(this).closest("tr").data("pxh")
        CallModal(2)
    })
    $("#phieuxh").on("change", function () {
        GetViewXH()
    })
    $(".save-imgA").on("click", function () {
        SavePhuLieuV2("")
        $('#signatureModal').modal('hide');
    })
    $(".icon_reloadPhieu").on("click", function () {
        GetViewXH()
    })
    $(document).on("click", '.fa-rectangle-xmark', function () {
        let $this = $(this).closest("tr")
        deleteRow = $this
        var cayvai = deleteRow.data("cayvai")
        $(".cayvaidelete").text(cayvai)
        $("#myModalD").modal("show")
    })
    $(".btn-tab3").on("click", function () {
        GetViewXH()
    })
})
var deleteRow;
//async function DeleteKien() {
//    var phieuxh = deleteRow.data("pxh")
//    var manpl = deleteRow.data("manpl")
//    var SLNhap = deleteRow.data("slnhap")
//    var dot = deleteRow.data("dot")
//    var barcode = deleteRow.data("phieuyc")
//    var soloId = deleteRow.data("soloid")
//    var ArrSaveKT = []
//    const phieuXuatHangKTPL = {
//        PhieuXHPL: phieuxh,
//        PhieuYC: barcode,
//        MaLenh: "",
//        MaGop: "",
//        MaLenhSX: "",
//        MaNPL: manpl,
//        SLNhap: SLNhap,
//        Module: 1,
//        Sort: 1,
//        Sign: "",
//        NgayXuatHang: "",
//        Dot: dot,
//        SoLoID: soloId
//    };
//    ArrSaveKT.push(phieuXuatHangKTPL)
//    ApiDeletePhieuXHPL(ArrSaveKT)
//}
async function ApiDeletePhieuXHPL(arrSave) {

    const request = new Request(`/api/PhieuXuatHangNPL/DeletePhieu`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        await GetXHPhieu()
        await loadXH()
        await GetViewXH()
        GetDataPhieuXuatKho(trMaPhieu, trDH, trMaLenh, trloc)
        $("#myModalD").modal("hide")
    }

}
async function GetXH() {
    var para1 = $("#phieuycls").val()

    var para2 = $("#phieuycls option:selected").data("value2")
    var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetPXHPL&para1=${para1}&para2=${para2}`;
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
                <option  value="${x.PhieuXHPL}">${x.Display}</option>
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

    var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetViewXHPL&para1=${para1}&para2=${para2}&para3=${para3}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) {
            //$(".phieuxh").empty()
        }
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
                                .attr("src", `/Images/XuatHangPhuLieu/KyTen/${options.data.KyTen}?${new Date().getTime()}`)
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
            width: 150,
        },
        {
            dataField: "HuyCat",
            caption: "Hủy cấp",
            cssClass: "col-header",
            width: 100,
            cellTemplate: function (container, options) {
                const rowData = options.data;
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
                            .addClass("fa-solid fa-rectangle-xmark")
                            .toggleClass("d-none", rowData.CheckCat == "1" || checkPQ != 1)
                            .on("click", function () {
                                showConfirmModalDeleteXemPhieu(async function () {
                                    var arrDelete = []
                                    const phieuXuatHangKTPL = {
                                        PhieuXHPL: rowData.PhieuXH,
                                        PhieuYC: rowData.BarCode,
                                        MaLenh: "",
                                        MaGop: "",
                                        MaLenhSX: "",
                                        MaNPL: "",
                                        SLNhap: 1,
                                        Module: 1,
                                        Sort: 1,
                                        Sign: "",
                                        NgayXuatHang: "",
                                        Dot: rowData.Dot,
                                        SoLoID: ""
                                    };
                                    arrDelete.push(phieuXuatHangKTPL)
                                    ApiDeletePhieuXHPL(arrDelete)
                                });
                            })
                    )
                    .appendTo(container);
            }
        },
        {
            dataField: "GhiChu",
            caption: "Ghi chú",
            cssClass: "col-header",
            width: 100,
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
var checkPQ;
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
    dataItemCode = data
    // SORT theo Sort trước khi render
    data = data.sort((a, b) => Number(a.Sort) - Number(b.Sort));

    $("#tbodyXHItemCode").dxDataGrid({
        dataSource: data,

        columns: [

            { dataField: "MaVT", caption: "ItemCode", cssClass: "col-header", minWidth: 100 },
            { dataField: "CapPhat", caption: "Cấp phát", cssClass: "col-header", width: 80 },
            {
                dataField: "SLDK", caption: "SL đăng ký", cssClass: "col-header", width: 80,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.SLDK || 0).toFixed(2));
                }
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
            { dataField: "TenDVVT", caption: "Đơn vị", cssClass: "col-header", minWidth: 100 },
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
        const slGoc = parseFloat($(this).data("slxuat") || 0);  // SLXuất gốc từ data attr
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
            { dataField: "SLNhap", caption: "SL Xuất", cssClass: "thucnhap", minWidth: 80, },
            { dataField: "GhiChu", caption: "Ghi chú", cssClass: "ghichu", minWidth: 70 },
            { dataField: "BarCode", caption: "Barcode", cssClass: "barcode", visible: false },
            {
                caption: "In BarCode",
                width: 100,
                visible: false,
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
                                .on("click", function () {
                                    showConfirmModalTachKien(item, async function () {
                                        if ($("#modalSoLuong").val() == "") {
                                            showToast("warning", "Vui lòng nhập thực xuất kiện chia!!!");
                                            return
                                        }

                                        var itemXH = dataXH.find(x => x.BarCode == item.BarCode);
                                        var itemNew = { ...item };
                                        itemNew.GhiChu = $("#modalGhiChu").val()
                                        var barCodeCheck = await GetCheckBarCodePhuLieu(item.BarCode);
                                        var soLuongChia = parseFloat($("#modalSoLuong").val());

                                        itemNew.SLNhap = soLuongChia;
                                        itemXH.SLNhap = itemXH.SLNhap - soLuongChia;


                                        var dataBarCodeChia = dataXH.filter(x =>
                                            x.BarCodeGoc == item.BarCode && x.BarCode.includes(".")
                                        );

                                        if (dataBarCodeChia.length == 0 && barCodeCheck == "") {
                                            // Lần đầu tiên chia
                                            itemNew.BarCode = `${item.BarCode}.1`;
                                            itemNew.SoKienHienThi = `${item.SoKienHienThi}.1`;
                                            itemNew.TachKien = 1
                                        } else {
                                            // Đã có barcode chia trước đó
                                            // Lấy tất cả số sau dấu "." từ danh sách barcode đã chia
                                            var arrPasr = dataBarCodeChia
                                                .map(x => {
                                                    if (!x.BarCode || !x.BarCode.includes(".")) return null;
                                                    return parseInt(x.BarCode.split(".").pop(), 10);
                                                })
                                                .filter(x => !isNaN(x));
                                            // Max số hiện có trong DB / list
                                            var maxNumber = arrPasr.length > 0 ? Math.max(...arrPasr) : 0;
                                            var arrPasrData = barCodeCheck
                                                .map(x => {
                                                    if (!x.BarCode || !x.BarCode.includes(".")) return null;
                                                    return parseInt(x.BarCode.split(".").pop(), 10);
                                                })
                                                .filter(x => !isNaN(x));
                                            var maxNumberSql = arrPasrData.length > 0 ? Math.max(...arrPasrData) : 0;


                                            // Lấy số tiếp theo (đảm bảo lớn nhất)
                                            var nextNumber = Math.max(maxNumber, maxNumberSql) + 1;

                                            // Gán lại barcode & số kiện
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
                                console.log(2)
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
                $row.attr("data-sokien", d.SoKien || 0);
                $row.attr("data-slnhappress", d.SLN || 0);
                $row.attr("data-manpl", d.MaNPL || 0);
                $row.attr("data-mavtid", d.MaVTID || 0);
                $row.attr("data-mauvtid", d.MauVTID || 0);
                $row.attr("data-khovaiid", d.KhoVaiID || 0);
                $row.attr("data-madvcd", d.MaDVCD || 0);

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
    const PhieuCap = $("#soPhieuDK").val()
    if (PhieuCap == "") {
        showToast("warning", `Vui lòng chọn phiếu cấp!`);
        PlayAudioError();
        return;
    }
    const para1 = $("#MaLenhSX option:selected").data("magop")
    const para2 = $("#MaLenhSX option:selected").data("malenhsx")
    const para3 = barcode
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetXuatHangPLV2&para1=${para1}&para2=${para2}&para4=${para3}`;
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
        }
        var d = data[0];
        if (d.CheckKiemKe == 1) {
            showToast("warning", `Vật tư đang trong quá trình kiểm kê không được xuất hàng!`);
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
            console.log(5)
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
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetBarCodeCheckPL&para1=${para1}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        if (data.length == 0) return ""
        else return data[0].BarCode
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
        SavePhuLieuV2(signatureImage)
    }
}
async function UpdateKT(signatureImage) {

    var ArrSaveKT = []
    const phieuXuatHangKTPL = {
        PhieuXHPL: pxhData,
        PhieuYC: "",
        MaLenh: "",
        MaGop: "",
        MaLenhSX: "",
        MaNPL: "",
        SLNhap: 1,
        Module: 1,
        Sort: 1,
        Sign: signatureImage,
        NgayXuatHang: "",
        Dot: 1
    };
    ArrSaveKT.push(phieuXuatHangKTPL)
    await ApiSavePhieuXHPL(ArrSaveKT)
}

async function SavePhuLieuV2(signatureImage) {
    var ArrSave = []
    var ArrSaveKT = []
    var $select = $("#MaLenhSX");

    var selectedOption = $select.find("option:selected");
    var loc = $(".select_loc").val();
    var tenkh = selectedOption.data("khachhang") || null
    var tenhang = selectedOption.data("mahang") || null
    var malenh = selectedOption.data("malenh") || null
    var malenhsx = selectedOption.data("malenhsx") || null
    var magop = selectedOption.data("magop") || null
    const PhieuCap = $("#soPhieuDK").val()
    dataXH.forEach(x => {
        const soLoObject = {
            SoLoID: x.SoLoID,
            SoLo: x.SoLo,
            PhieuYC: PhieuCap,
            MaLenh: malenh,
            MaLenhSX: malenhsx,
            MaGop: magop,
            MaKH: x.null,
            TenKH: tenkh,
            MaHang: x.null,
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


        const phieuXuatHangKTPL = {
            PhieuXHPL: "PX001",
            PhieuYC: x.BarCode,
            MaLenh: malenh,
            MaGop: magop,
            MaLenhSX: malenhsx,
            MaNPL: x.MaNPL,
            SLNhap: x.SLNhap,
            Module: loc,
            Sort: "",
            Sign: signatureImage,
            NgayXuatHang: "",
            Dot: x.Dot,
            SoLoID: x.SoLoID,
        };
        ArrSaveKT.push(phieuXuatHangKTPL)
        ArrSave.push(soLoObject)
    })


    if (ArrSave.length > 0) {
        await ApiSave(ArrSave)
    }
    if (ArrSaveKT.length > 0) {
        await ApiSavePhieuXHPL(ArrSaveKT)
    }
}
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

/// Kiệt
let dxDataGridChiTietLenh;
/// API CALL
async function GetChiTietLenh(maLenhSanXuat) {
    const url = `/api/PhieuXuatHangNPL/Get?action=GetChiTietLenh&para1=${maLenhSanXuat}`;

    try {
        const response = await fetch(url)
        const data = await response.json()
        console.log("data Chi Tieyes Lệnh", data)
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
    const isNPL = 0;
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

function openModalSoanHang() {
    //renderTableSoanHang(dataSoanHangSample);
    GetViewSoanHang()
    $("#modalSoanHang").modal("show")
}
async function GetViewSoanHang() {
    var dataUser = !window.CefSharp ? userNameSave : userName
    var url = `/api/PhieuXuatHangNPL/Get?Action=GetDanhSachVatTuSoanHang&para1=${trMaLenh}&para2=${dataUser}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        renderTableSoanHang(data)

    } catch (error) {
        console.error(error.message);
        return 0;
    }
}
function renderTableSoanHang(data) {
    if (data.length > 0) {
        $("#btnXoaDongChon").show()
        $("#btnXacNhanDongChon").show()
        const checkNguoiTao = data[0].CheckEdit
        if (checkNguoiTao == 0) {
            $("#btnXoaDongChon").hide()
        } else {
            $("#btnXoaDongChon").show()
        }
    } else {
        $("#btnXoaDongChon").hide()
        $("#btnXacNhanDongChon").hide()
    }

    var hasVisibleButton = data.some(row => row.Status != 1 && row.CheckEdit != 1);
    $("#tbodySoanHang").dxDataGrid({
        dataSource: data,
        columns: [
            {
                caption: "",
                cssClass: "col-header position-relative",
                width: 50,
                alignment: "center",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var isDisabled = options.data.Status == 1;
                    $('<input>')
                        .addClass('form-check-input row-checkbox')
                        .attr('type', 'checkbox')
                        .attr('disabled', isDisabled)
                        .attr('data-phieush', options.data.PhieuSH)
                        .attr('data-manpl', options.data.MaNPL)
                        .css({
                            'width': '20px',
                            'height': '20px',
                            'cursor': isDisabled ? 'not-allowed' : 'pointer',
                            'position': 'absolute',
                            'top': '8px'
                        })
                        .on('change', function () {
                            if (!isDisabled) {

                            }
                        })
                        .appendTo(container);
                }
            },
            {
                dataField: "PhieuSH",
                caption: "Số phiếu",
                cssClass: "col-header",
                minWidth: 100,
                allowEditing: false,
                groupIndex: 0
            },
            {
                dataField: "MaVT",
                caption: "ItemCode",
                cssClass: "col-header",
                minWidth: 100,
                allowEditing: false
            },
            {
                dataField: "MauVT",
                caption: "Màu VT",
                cssClass: "col-header",
                minWidth: 100,
                allowEditing: false
            },
            {
                dataField: "KhoSize",
                caption: "Khổ Size",
                cssClass: "col-header",
                minWidth: 100,
                allowEditing: false
            },
            {
                dataField: "DonVi",
                caption: "Đơn Vị",
                cssClass: "col-header",
                minWidth: 80,
                allowEditing: false
            },
            {
                dataField: "SLSoanHang",
                caption: "SL Soạn Hàng",
                cssClass: "col-header",
                width: 130,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    // Nếu Status = 1 thì khóa hết
                    // Nếu Status = 0 và CheckEdit = 1 thì cho chỉnh sửa
                    // Nếu Status = 0 và CheckEdit = 0 thì không cho chỉnh sửa
                    var isDisabled = options.data.Status == 1 || options.data.CheckEdit == 0;
                    $('<input>')
                        .addClass('form-control form-control-sm')
                        .attr('type', 'number')
                        .attr('step', '0.01')
                        .attr('disabled', isDisabled)
                        .val(options.value || 0)
                        .on('change', function () {
                            if (!isDisabled) {
                                options.data.SLSoanHang = parseFloat($(this).val()) || 0;
                                const slSoanHang = options.data.SLSoanHang
                                const phieuSH = options.data.PhieuSH
                                const maNPL = options.data.MaNPL
                                const ghiChu = options.data.GhiChu
                                const nguoiSH = options.data.NguoiSoanHang

                                ArrSoanHang(phieuSH, maNPL, slSoanHang, nguoiSH, ghiChu, 0, "")
                            }
                        })
                        .appendTo(container);
                }
            },
            {
                dataField: "NguoiSoanHang",
                caption: "Người Soạn Hàng",
                cssClass: "col-header",
                width: 150,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    // Nếu Status = 1 thì khóa hết
                    // Nếu Status = 0 và CheckEdit = 1 thì cho chỉnh sửa
                    // Nếu Status = 0 và CheckEdit = 0 thì không cho chỉnh sửa
                    var isDisabled = options.data.Status == 1 || options.data.CheckEdit == 0;
                    $('<input>')
                        .addClass('form-control form-control-sm')
                        .attr('type', 'text')
                        .attr('disabled', isDisabled)
                        .val(options.value || '')
                        .on('change', function () {
                            if (!isDisabled) {
                                options.data.NguoiSoanHang = $(this).val();
                                const slSoanHang = options.data.SLSoanHang
                                const phieuSH = options.data.PhieuSH
                                const maNPL = options.data.MaNPL
                                const ghiChu = options.data.GhiChu
                                const nguoiSH = options.data.NguoiSoanHang
                                ArrSoanHang(phieuSH, maNPL, slSoanHang, nguoiSH, ghiChu, 0, "")
                            }
                        })
                        .appendTo(container);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                cssClass: "col-header",
                width: 150,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    // Nếu Status = 1 thì khóa hết
                    // Nếu Status = 0 và CheckEdit = 1 thì cho chỉnh sửa
                    // Nếu Status = 0 và CheckEdit = 0 thì không cho chỉnh sửa
                    var isDisabled = options.data.Status == 1 || options.data.CheckEdit == 0;
                    $('<input>')
                        .addClass('form-control form-control-sm')
                        .attr('type', 'text')
                        .attr('disabled', isDisabled)
                        .val(options.value || '')
                        .on('change', function () {
                            if (!isDisabled) {
                                options.data.GhiChu = $(this).val();
                                const slSoanHang = options.data.SLSoanHang
                                const phieuSH = options.data.PhieuSH
                                const maNPL = options.data.MaNPL
                                const ghiChu = options.data.GhiChu
                                const nguoiSH = options.data.NguoiSoanHang
                                ArrSoanHang(phieuSH, maNPL, slSoanHang, nguoiSH, ghiChu, 0, "")
                            }
                        })
                        .appendTo(container);
                }
            },

            {
                dataField: "NgaySoanHang",
                caption: "Ngày Soạn Hàng",
                cssClass: "col-header",
                width: 120,
                dataType: "date",
                format: "dd/MM/yyyy",
                allowEditing: false,
                allowFiltering: false
            },
            {
                dataField: "CheckEdit",
                caption: "Check Edit",
                cssClass: "col-header",
                width: 100,
                alignment: "center",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var status = options.value == 1 ?
                        '<span class="badge bg-info">Cho sửa</span>' :
                        '<span class="badge bg-secondary">Không sửa</span>';
                    $(container).html(status);
                },
                visible: false
            },
            {
                dataField: "Status",
                caption: "Trạng thái",
                cssClass: "col-header",
                width: 100,
                alignment: "center",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    var status = options.value == 1 ?
                        '<span class="badge bg-success p-2">Đã xác nhận</span>' :
                        '<span class="badge bg-warning p-2">Chưa xác nhận</span>';
                    $(container).html(status);
                }
            },
            {
                caption: "Thao tác",
                cssClass: "col-header",
                width: 130,
                alignment: "center",
                allowEditing: false,
                visible: true,
                cellTemplate: function (container, options) {
                    // Nếu Status = 1 thì khóa nút
                    // Nếu Status = 0 và CheckEdit = 1 thì khóa nút (vì đang cho sửa)
                    // Nếu Status = 0 và CheckEdit = 0 thì cho bấm xác nhận
                    var isDisabled = options.data.Status == 1;
                    $('<button>')
                        .addClass('btn btn-success btn-sm')
                        .html('<i class="bi bi-check-circle"></i> Xác nhận')
                        .attr('disabled', isDisabled)
                        .on('click', function () {
                            if (!isDisabled) {
                                showConfirmModalSign(async function () {
                                    $(".classSignSave").show()
                                    $(".classSignSaveSH").hide()
                                    options.data.Status = 1;
                                    const slSoanHang = options.data.SLSoanHang
                                    const phieuSH = options.data.PhieuSH
                                    const maNPL = options.data.MaNPL
                                    const ghiChu = options.data.GhiChu
                                    const nguoiSH = options.data.NguoiSoanHang
                                    const signatureImage = canvas.toDataURL('image/png');
                                    ArrSoanHang(phieuSH, maNPL, slSoanHang, nguoiSH, ghiChu, 1, signatureImage)
                                    // Refresh lại grid để hiển thị trạng thái mới
                                    $("#tbodySoanHang").dxDataGrid("instance").refresh();
                                })
                            }
                        })
                        .appendTo(container);
                }
            }
        ],
        editing: {
            mode: "cell",
            allowUpdating: false
        },
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: true,
        wordWrapEnabled: true,
        showBorders: true,
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
        }
    });
}
function ArrSoanHang(phieuSH, maNPL, slSoanHang, nguoiSH, ghiChu, isXN, imageKT) {
    var dataUser = !window.CefSharp ? userNameSave : userName
    var arrSoanHang = []
    arrSoanHang.push({
        PhieuSH: phieuSH,
        Dot: '',
        MaLenh: '',
        MaLenhSX: '',
        MaNPL: maNPL,
        MaVT: '',
        MauVT: '',
        KhoSize: '',
        DonVi: '',
        IsNPL: '',
        SLCapPhat: '',
        SLSoanHang: slSoanHang,
        NguoiSoanHang: nguoiSH,
        NgayDangKy: '',
        NgaySoanHang: '',
        IsXN: isXN,
        KyTen: imageKT,
        NgayXacNhan: '',
        GhiChu: ghiChu,
        NguoiTao: '',
        NgayTao: ''
    });
    if (isXN == 0)
        UpdateTTSoanHang(arrSoanHang, "PostSoanHang")
    else if (isXN == 1) {
        UpdateTTSoanHang(arrSoanHang, "UpdateXNSoanHang")
    }
}

function showConfirmModalSign(onConfirm) {
    $('.classSignSaveSH').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#signatureModal").modal("hide");
    });
    $(".classSignSave").hide()
    $(".classSignSaveSH").show()
    $("#signatureModal").modal("show");
}
function showConfirmModalDeleteSH(onConfirm) {
    $('#btnXacNhanXoaModal').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        $("#modalXacNhanXoa").modal("hide");
    });
    $("#modalXacNhanXoa").modal("show");
}

async function UpdateTTSoanHang(arrSave, action) {
    const request = new Request(`/api/PhieuXuatHangNPL/PostSoanHang?action=${action}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (action == "UpdateXNSoanHang") {
        if (data == "True") {
            showToast("success", `Lưu thành công`);

        }
        else {
            showToast("warning", `Lưu thất bại`);
        }
    }
    if (action == 'DeleteSoanHang') {
        if (data == "True") {
            showToast("success", `Xóa thành công`);
            GetViewSoanHang()
        }
        else {
            showToast("warning", `Xóa thất bại`);
        }
    }

}
$(function () {
    $("#btnXoaDongChon").on("click", function () {
        // Lấy tất cả các checkbox đã được check và không bị disabled
        var checkedBoxes = $(".row-checkbox:checked:not(:disabled)");

        if (checkedBoxes.length === 0) {
            toastr.warning("Vui lòng chọn ít nhất một dòng để xóa!");
            return;
        }

        // Lấy instance của grid
        var gridInstance = $("#tbodySoanHang").dxDataGrid("instance");
        var dataSource = gridInstance.option("dataSource");

        // Lấy danh sách các row cần xóa
        var rowsToDelete = [];
        var listHTML = "";

        checkedBoxes.each(function () {
            var phieuSH = $(this).attr('data-phieush');
            var maNPL = $(this).attr('data-manpl');

            // Tìm row tương ứng trong dataSource
            var row = dataSource.find(function (item) {
                return item.PhieuSH === phieuSH && item.MaNPL === maNPL;
            });

            if (row) {
                rowsToDelete.push({
                    PhieuSH: phieuSH,
                    MaNPL: maNPL,
                    data: row
                });

                // Tạo HTML cho danh sách
                listHTML += `<li>Phiếu: <strong> ${phieuSH} </strong> - ItemCode: ${row.MaVT} </li>`;
            }
        });

        // Cập nhật nội dung modal
        $("#soLuongDongXoa").text(rowsToDelete.length + " dòng");
        $("#listDongXoa").html(listHTML);
        var dataUser = !window.CefSharp ? userNameSave : userName
        showConfirmModalDeleteSH(async function () {
            var arrSoanHang = []
            rowsToDelete.forEach(x => {
                arrSoanHang.push({
                    PhieuSH: x.PhieuSH,
                    Dot: '',
                    MaLenh: '',
                    MaLenhSX: '',
                    MaNPL: x.MaNPL,
                    MaVT: '',
                    MauVT: '',
                    KhoSize: '',
                    DonVi: '',
                    IsNPL: '',
                    SLCapPhat: '',
                    SLSoanHang: 0,
                    NguoiSoanHang: 0,
                    NgayDangKy: '',
                    NgaySoanHang: '',
                    IsXN: 0,
                    KyTen: 0,
                    NgayXacNhan: "",
                    GhiChu: 0,
                    NguoiTao: '',
                    NgayTao: ''
                });

            })
            UpdateTTSoanHang(arrSoanHang, "DeleteSoanHang")
        })
    });
    $("#btnXacNhanDongChon").on("click", function () {
        // Lấy tất cả các checkbox đã được check và không bị disabled
        var checkedBoxes = $(".row-checkbox:checked:not(:disabled)");

        if (checkedBoxes.length === 0) {
            showToast("warning", "Vui lòng chọn dòng để xác nhận")
            return;
        }
        var gridInstance = $("#tbodySoanHang").dxDataGrid("instance");
        var dataSource = gridInstance.option("dataSource");

        // Lấy danh sách các row được chọn
        var selectedRows = [];
        checkedBoxes.each(function () {
            var phieuSH = $(this).attr('data-phieush');
            var maNPL = $(this).attr('data-manpl');

            // Tìm row tương ứng trong dataSource
            var row = dataSource.find(function (item) {
                return item.PhieuSH === phieuSH && item.MaNPL === maNPL;
            });

            if (row) {
                selectedRows.push(row);
            }
        });


        showConfirmModalSign(async function () {
            var arrSoanHang = []
            var dataUser = !window.CefSharp ? userNameSave : userName

            const signatureImage = canvas.toDataURL('image/png');
            selectedRows.forEach(function (row) {
                var rowIndex = dataSource.findIndex(item =>
                    item.PhieuSH === row.PhieuSH &&
                    item.MaNPL === row.MaNPL
                );
                dataSource[rowIndex].Status = 1;
                arrSoanHang.push({
                    PhieuSH: row.PhieuSH,
                    Dot: '',
                    MaLenh: '',
                    MaLenhSX: '',
                    MaNPL: row.MaNPL,
                    MaVT: '',
                    MauVT: '',
                    KhoSize: '',
                    DonVi: '',
                    IsNPL: '',
                    SLCapPhat: '',
                    SLSoanHang: 0,
                    NguoiSoanHang: dataUser,
                    NgayDangKy: '',
                    NgaySoanHang: '',
                    IsXN: 1,
                    KyTen: signatureImage,
                    NgayXacNhan: '',
                    GhiChu: 0,
                    NguoiTao: '',
                    NgayTao: ''
                });

            });

            checkedBoxes.prop('checked', false);

            // Refresh grid
            gridInstance.option("dataSource", dataSource);
            gridInstance.refresh();

            $(".classSignSave").show()
            $(".classSignSaveSH").hide()
            UpdateTTSoanHang(arrSoanHang, "UpdateXNSoanHang")
        })
    });
})

/// Kiệt Soạn Hàng
/// VARIABLE
let dotSH;
let dxDataGridDangKySH;
let batchDataSH;
let selectedItems = [];
let rowDelete;
let maNPLJoin;
/// EVENT 
$(document).ready(function () {
    GetPhieuSH();
    createViewDxDataGridDangKySH([]);
})

$(function () {
    $(".select_2").select2()

    const picker1 = new tempusDominus.TempusDominus(document.getElementById('datetimepickerDangKy'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    const picker2 = new tempusDominus.TempusDominus(document.getElementById('datetimepickerSoanHang'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    $("#ngayDangKy").trigger("click");
    $("#ngaySoanHang").trigger("click");
    picker1.hide();
    picker2.hide();

    $('#searchInput').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        const filtered = batchDataSH.filter(item => item.Display.toLowerCase().includes(searchTerm));
        renderList(filtered);
    });

    $('#itemCodeInput').click(function (e) {
        e.stopPropagation();
        $('#dropdownList').toggleClass('show');
        $('#searchInput').val('').focus();
        renderList(batchDataSH);
    });

    $('#checkAll').change(async function () {
        const isChecked = $(this).is(':checked');

        dxDataGridDangKySH.beginCustomLoading("Đang tải dữ liệu...");

        try {
            if (isChecked) {
                selectedItems = [...batchDataSH];

                maNPLJoin = selectedItems.map(item => item.MaNPL).join(';');

                if (maNPLJoin) {
                    await GetChiTietVatTu(maNPLJoin);
                }
            } else {
                selectedItems = [];
                maNPLJoin = '';

                createViewDxDataGridDangKySH([]);
            }

            $('#itemList input[type="checkbox"]').prop('checked', isChecked);

            updateInput();

        } catch (error) {
            console.error('Error:', error);
        } finally {
            dxDataGridDangKySH.endCustomLoading();
        }
    });

    $(document).on('change', '#itemList input[type="checkbox"]', async function (e) {
        e.stopPropagation();

        const itemId = $(this).attr('id');
        const isChecked = $(this).is(':checked');

        if (isChecked) {
            const item = batchDataSH.find(x => x.MaNPL == itemId);
            if (item && !selectedItems.some(x => x.MaNPL == itemId)) {
                selectedItems.push(item);
            }
        } else {
            selectedItems = selectedItems.filter(x => x.MaNPL != itemId);
        }

        updateCheckAll();
        updateInput();

        maNPLJoin = selectedItems.map(item => item.MaNPL).join(';');

        if (maNPLJoin) {
            await GetChiTietVatTu(maNPLJoin);
        }
    });

    $(document).on('click', function (e) {
        if (e.isTrigger) return;

        if (!$(e.target).closest('.select-container,.dropdown-item').length) {
            $('#dropdownList').removeClass('show');
        }
    });

    $("#btnShowModal").on("click", function () {
        $("#modalAddSoanHang").modal("show")
        GetChiTietLenhVatTu()
    })

    // Khi đóng modal - reset tất cả checkbox và state
    $('#modalAddSoanHang').on('hidden.bs.modal', function () {
        // Reset checkbox
        $('#itemList input[type="checkbox"]').prop('checked', false);
        $('#checkAll').prop('checked', false);
        $('#itemCodeInput').val('');
        //$('#dropdownList').removeClass('show');

        // Reset state
        selectedItems = [];
        $("#txtSLVTDelete").empty();
        // Clear grid
        createViewDxDataGridDangKySH([])
    });

    // Khi mở modal - khởi tạo lại
    $('#modalAddSoanHang').on('shown.bs.modal', function () {
        GetPhieuSH();
        /*GetMaLenh();*/
    });

    $('#btnSavePhieuSH').on("click", async function () {
        const dataSource = dxDataGridDangKySH.option("dataSource");

        if (dataSource.length === 0) {
            showToast("warning", "Chưa có thông tin đăng ký");
            PlayAudioError();
            return;
        }

        const ngayDK = moment($("#ngayDangKy").val(), "DD/MM/YYYY").format("YYYY-MM-DD");
        const ngaySH = moment($("#ngaySoanHang").val(), "DD/MM/YYYY").format("YYYY-MM-DD");
        var dataUser = !window.CefSharp ? userNameSave : dataUser = userName

        const arrSave = dataSource.map(item => ({
            PhieuSH: '',
            Dot: null,
            MaLenh: item.MaLenh,
            MaLenhSX: item.MaLenhSanXuat,
            MaNPL: item.MaNPL,
            MaVT: item.MaVT,
            MauVT: item.MauVT,
            KhoSize: item.KhoVai,
            DonVi: item.TenDVVT,
            IsNPL: 0,
            SLCapPhat: item.CapPhat,
            SLSoanHang: item.SLDK,
            NguoiSoanHang: item.NguoiSoanHang,
            NgayDangKy: ngayDK,
            NgaySoanHang: ngaySH,
            IsXN: 0,
            KyTen: null,
            NgayXacNhan: null,
            GhiChu: item.GhiChu,
            NguoiTao: dataUser,
            NgayTao: null
        }));

        for (const item of arrSave) {
            if (!item.SLSoanHang || item.SLSoanHang <= 0) {
                focusErrorRow({
                    gridId: 'dxDataGridDangKySH',
                    dataSource,
                    findItem: item,
                    inputClass: '.inputSLDK'
                });

                showToast(
                    "warning",
                    `Item Code: ${item.MaVT} chưa nhập số lượng đăng ký`
                );
                PlayAudioError();
                return;
            }

            if (!item.NguoiSoanHang || !item.NguoiSoanHang.trim()) {
                focusErrorRow({
                    gridId: 'dxDataGridDangKySH',
                    dataSource,
                    findItem: item,
                    inputClass: '.inputSoanHang'
                });

                showToast(
                    "warning",
                    `Item Code: ${item.MaVT} chưa nhập người soạn hàng`
                );
                PlayAudioError();
                return;
            }
        }

        await SavePhieuSH(arrSave);
    });

    $("#btnConfirmtDeletePhieu").on("click", function () {
        let dataSource = dxDataGridDangKySH.option("dataSource");
        const index = dataSource.findIndex(item =>
            item.MaNPL === rowDelete.MaNPL
        );

        if (index !== -1) {
            // Xóa khỏi dataSource
            dataSource.splice(index, 1);

            // Xóa khỏi selectedItems
            selectedItems = selectedItems.filter(x => x.MaNPL !== rowDelete.MaNPL);

            // Cập nhật maNPLJoin
            maNPLJoin = selectedItems.map(item => item.MaNPL).join(';');

            // Cập nhật input hiển thị
            updateInput();

            // Bỏ check checkbox trong dropdown - DÙNG ATTRIBUTE SELECTOR
            $(`#itemList input[type="checkbox"][id="${rowDelete.MaNPL}"]`).prop('checked', false);

            // Cập nhật checkAll
            updateCheckAll();

            // Refresh grid
            createViewDxDataGridDangKySH(dataSource);
            showToast("success", "Xóa thành công");
        }

        $("#modalComfimrtDeleteVT").modal("hide");
    });

})

/// API
async function GetPhieuSH() {
    const url = `/api/PhieuXuatHangNPL/Get?Action=GetPhieuSH`;
    try {
        const response = await fetch(url)
        const data = await response.json();
        dotSH = data[0].Dot
        $("#txtPhieuSH").val(`PSH_${dotSH}`)
    } catch (err) {
        console.error(err)

    }
}

async function GetChiTietLenhVatTu() {
    const url = `/api/PhieuXuatHangNPL/Get?Action=ChiTietLenhVatTu&para1=0&para2=${trMaLenh}`;

    try {
        const response = await fetch(url)
        const data = await response.json();

        batchDataSH = data;
        renderList(data)

    } catch (err) {
        console.error(err)
    }
}

async function GetChiTietVatTu(maNPLJoin) {
    const url = `/api/PhieuXuatHangNPL/Get?Action=GetChiTietVatTu&para1=0&para2=${trMaLenh}&para3=${maNPLJoin}`;

    try {
        const response = await fetch(url)
        const data = await response.json();
        createViewDxDataGridDangKySH(data);

    } catch (err) {
        console.error(err)
    }
}

async function SavePhieuSH(arrSave) {
    const url = `/api/PhieuXuatHangNPL/PostSoanHang?action=PostSoanHang`;

    try {
        await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(arrSave),
        })


        showToast("success", "Thêm thành công !")
        GetPhieuSH();
        handleBack();
    } catch (err) {
        console.error(err)
    }
}

/// FUNCTION
function focusErrorRow({
    gridId,
    dataSource,
    findItem,
    inputClass,
    removeOnInput = true
}) {
    const rowIndex = dataSource.findIndex(item => item.MaNPL === findItem.MaNPL);
    if (rowIndex === -1) return;

    dxDataGridDangKySH.navigateToRow(dataSource[rowIndex]);

    setTimeout(() => {
        const $row = $(`#${gridId} .dx-data-row`).eq(rowIndex);
        $row.addClass('highlight-row');

        const $input = $row.find(inputClass);
        if ($input.length) {
            $input.focus().select();

            if (removeOnInput) {
                $input.one('input', () => {
                    $row.removeClass('highlight-row');
                });
            }
        }
    }, 200);
}


function updateSummary(grid) {
    const dataSource = grid.option("dataSource");
    const total = dataSource.reduce(
        (sum, item) => sum + (Number(item.SLDK) || 0),
        0
    );

    // CẮT 4 số thập phân – KHÔNG LÀM TRÒN
    const truncated = Math.trunc(total * 10000) / 10000;

    // Tìm cell summary của cột SLDK dựa vào column index
    const columns = grid.option("columns");
    const visibleColumns = columns.filter(col => col.visible !== false);
    const sldkColumnIndex = visibleColumns.findIndex(col => col.dataField === "SLDK");

    if (sldkColumnIndex !== -1) {
        // Tìm summary cell theo index của cột
        const $summaryRow = grid.element().find('.dx-datagrid-total-footer .dx-row');
        const $summaryCell = $summaryRow.find('td').eq(sldkColumnIndex);
        const $summaryItem = $summaryCell.find('.dx-datagrid-summary-item');

        if ($summaryItem.length) {
            $summaryItem.text(truncated.toString());
        }
    }
}
function handleBack() {
    $("#modalAddSoanHang").modal("hide")
    // Reset checkbox
    $('#itemList input[type="checkbox"]').prop('checked', false);
    $('#checkAll').prop('checked', false);
    $('#itemCodeInput').val('');
    /*   $('#dropdownList').removeClass('show');*/

    // Reset state
    selectedItems = [];
    maNPLJoin = '';
    $("#txtSLVTDelete").empty();
    // Clear grid
    createViewDxDataGridDangKySH([])


}
function renderList(data) {
    const itemList = $('#itemList');
    itemList.empty();

    // Kiểm tra data có tồn tại và là array không
    if (!data || !Array.isArray(data)) {
        return;
    }

    const html = data.map(item => {
        const isChecked = selectedItems.some(selected => selected.MaNPL == item.MaNPL);
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
    const displayText = selectedItems.map(item => item.Display).join(', ');
    $('#itemCodeInput').val(displayText);

    maNPLJoin = selectedItems.map(item => item.MaNPL).join(';');
}


/// DX DataGrid

function createViewDxDataGridDangKySH(data) {
    dxDataGridDangKySH = $("#dxDataGridDangKySH").dxDataGrid({
        dataSource: data,
        width: '100%',
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
            /*
                        { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 120 },*/
            { dataField: "MaLenh", caption: "Mã Lệnh", alignment: "center", minWidth: 100 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MauVT", caption: "Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "KhoVai", caption: "Khổ/Size", alignment: "center", minWidth: 120 },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "CapPhat",
                caption: "SL Cấp Phát",
                alignment: "center",
                minWidth: 120,
                headerCellTemplate: function (header, info) {
                    const $container = $("<div></div>").css({
                        width: "100%",
                        display: "flex",
                        alignItems: "center",
                        gap: "14px",
                        padding: "0 8px"
                    });
                    const $icon = $("<i></i>")
                        .addClass("fa-light fa-circle-arrow-right")
                        .css({
                            cursor: "pointer",
                            fontSize: "16px",
                            flexShrink: "0",
                        })
                        .on("click", function (e) {
                            e.stopPropagation();
                            var refresh = false;
                            const dataSource = dxDataGridDangKySH.option("dataSource");
                            dataSource.forEach(item => {
                                item.SLDK = item.CapPhat;
                                refresh = true;
                            });
                            if (refresh) {
                                dxDataGridDangKySH.refresh();
                            }
                        });
                    const $text = $("<span></span>")
                        .text(info.column.caption)
                        .css({
                            flexShrink: "0"
                        });
                    $container.append($text, $icon);
                    header.append($container);
                },
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    // Chuyển sang string để CẮT, không làm tròn
                    let str = value.toString();

                    if (str.includes(".")) {
                        const [intPart, decPart] = str.split(".");
                        str = intPart + "." + decPart.substring(0, 2);
                    }

                    container.text(str);
                }
            },
            {
                dataField: "SLDK",
                caption: "SL Soạn Hàng",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="number"
                                class="form-control inputSLDK"
                                style= "font-size:14px"
                                value="${options.data.SLDK ?? ''}" />
                    `);

                    $input.on("input", function () {
                        let value = this.value;

                        // Không cho số âm
                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng đăng ký không được âm");
                            this.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridDangKySH);
                            return;
                        }

                        // Chỉ cho tối đa 4 số thập phân 
                        if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            this.value = intPart + "." + decPart.substring(0, 4);
                            value = this.value;
                        }

                        if (Number(value) > options.data.CapPhat) {
                            showToast("warning", "Số lượng soạn hàng không được lớn hơn số lượng cấp phát");

                            const truncated = Math.trunc(options.data.CapPhat * 10000) / 10000;

                            this.value = truncated;
                            options.data.SLDK = truncated;
                            updateSummary(dxDataGridDangKySH);
                            return;
                        }

                        options.data.SLDK = value === "" ? null : Number(value);
                        updateSummary(dxDataGridDangKySH);
                    });

                    container.append($input);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control"
                               value="${options.data.GhiChu || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.GhiChu = this.value;
                    });

                    container.append($input);
                }
            },
            {
                dataField: "",
                caption: "Người Soạn Hàng",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control inputSoanHang"
                               value="${options.data.NguoiSoanHang || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.NguoiSoanHang = this.value;
                    });

                    container.append($input);
                }
            },

            {
                caption: "Xóa",
                minWidth: 50,
                alignment: "center",
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
                                .addClass("fa-solid fa-trash-can")
                                .css({ fontSize: "15px", color: "red", cursor: "pointer" })
                                .on("click", function () {
                                    rowDelete = options.data;
                                    isDeleteDK = true;
                                    $("#modalComfimrtDeleteVT").modal("show");
                                })
                        )
                        .appendTo(container);
                }
            }
        ],
        summary: {
            totalItems: [{
                column: "SLDK",
                summaryType: "sum",
                customizeText(e) {
                    if (e.value == null) return "";

                    const value = Number(e.value);

                    // CẮT 4 số thập phân – KHÔNG LÀM TRÒN
                    const truncated = Math.trunc(value * 10000) / 10000;

                    // Bỏ số 0 dư
                    return truncated.toString();
                }
            },
            {
                column: "CapPhat",
                summaryType: "sum",
                customizeText(e) {
                    if (e.value == null) return "";

                    const value = Number(e.value);

                    // CẮT 4 số thập phân – KHÔNG LÀM TRÒN
                    const truncated = Math.trunc(value * 10000) / 10000;

                    // Bỏ số 0 dư
                    return truncated.toString();
                }
            }]
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
    }).dxDataGrid("instance");

    /*$("#Layer_1").click();*/
}


