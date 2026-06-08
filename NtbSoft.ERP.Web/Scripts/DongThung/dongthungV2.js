//coding by Dat
//start

const input = $(".toggle_input input");
const img_chuaDongThung = `<svg width="20px" height="20px" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path fill="#b82a2aa8" d="M58.9 42.1c3-6.1 9.6-9.6 16.3-8.7L320 64 564.8 33.4c6.7-.8 13.3 2.7 16.3 8.7l41.7 83.4c9 17.9-.6 39.6-19.8 45.1L439.6 217.3c-13.9 4-28.8-1.9-36.2-14.3L320 64 236.6 203c-7.4 12.4-22.3 18.3-36.2 14.3L37.1 170.6c-19.3-5.5-28.8-27.2-19.8-45.1L58.9 42.1zM321.1 128l54.9 91.4c14.9 24.8 44.6 36.6 72.5 28.6L576 211.6v167c0 22-15 41.2-36.4 46.6l-204.1 51c-10.2 2.6-20.9 2.6-31 0l-204.1-51C79 419.7 64 400.5 64 378.5v-167L191.6 248c27.8 8 57.6-3.8 72.5-28.6L318.9 128h2.2z"/></svg>`
const img_daDongThung = `<svg width="20px" height="20px" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path fill="#aabd78" d="M50.7 58.5L0 160H208V32H93.7C75.5 32 58.9 42.3 50.7 58.5zM240 160H448L397.3 58.5C389.1 42.3 372.5 32 354.3 32H240V160zm208 32H0V416c0 35.3 28.7 64 64 64H384c35.3 0 64-28.7 64-64V192z"/></svg>`
const img_daNK = `<svg width="20px" height="20px" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path fill="#2247cd" d="M50.7 58.5L0 160H208V32H93.7C75.5 32 58.9 42.3 50.7 58.5zM240 160H448L397.3 58.5C389.1 42.3 372.5 32 354.3 32H240V160zm208 32H0V416c0 35.3 28.7 64 64 64H384c35.3 0 64-28.7 64-64V192z"/></svg>`


const tobdy_tbl_DS_KHDT = $("#tbl_DS_KHDT tbody")
const slc_lenhsx = $("#slc_lenhsx");
const customSelect = $("#customSelect");
const select_selected = $(".select-selected")
const option = $(".option")
const handleByQuantity = $("#handleByQuantity")
const handleDongThung = $("#handleDongThung")
const handleFast = $("#handleFast")
const thung_item = $(".thung-item")
const btn_refresh = $("#btn-refresh")
const btn_hide = $('#btn-hide')
const search = $('#search')
const btn_luuchuyen = $('#btn-luuchuyen')
const handleByVitri = $("#handleByVitri")


let currentDate = new Date()
// Định dạng lại ngày và giờ thành chuỗi "YYYY-MM-DD HH:mm:ss.SSS"
let formattedDateTime = currentDate.toISOString().replace(/T/, ' ').replace(/\..+/, '');
setInterval(() => {
    formattedDateTime = currentDate.toISOString().replace(/T/, ' ').replace(/\..+/, '');
}, 600000)
let ArrDsDongThung = [], ArrDS_KHDT = [], arrLsxFilter = []
let arrLenhSX = []
let refresh = false;
let rowIndex = 0;
let typeAction = 2;
let isHide = false;
let isLuuChuyen = false;
let isSave = false;
let timeoutId;
let username = localStorage.getItem('username1') != null ? localStorage.getItem('username1') : ""

var DHBarCode = ""
var POBarCode = ""
var PKLBarCode = ""
function GetDS_KHDT() {
    let valueDH = $("#DH_PKL").val()
    let valuePO = $("#PO_PKL").val()
    let maPKL = $("#DVSX_PKL").val()
    let isDecat = $("#DVSX_PKL option:selected").data("isdecat")
    $.ajax({
        async: false,
        url: `/api/DongThung/GetDS_KHDT?maplk=${maPKL}&mahang=${valueDH}&poid=${valuePO}&statuschuyen=0&madvsx=0&isDecat=${isDecat}&dotsx=0`,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                if (refresh == false) {
                    GetDSThung()
                }
                renderTbl_DS_KHDT(data)
            }
            else {
                renderTbl_DS_KHDT(data)
                renderDsThung(data)
            }
        }
    });
}
$(document).on('click', '.select-options .option', function () {
    // bỏ active cũ
    $('.select-options .option').removeClass('active');

    // gắn active cho cái vừa chọn
    $(this).addClass('active');

    // lấy data-action
    const action = $(this).data('action');
    if (action == "0") {
        select_selected.html("Chưa đóng")
        handleDongThung.html("Đóng thùng")
        handleFast.html("Đóng tất cả")
    }
    else if (action == "1") {
        select_selected.html("Đã đóng")
        handleDongThung.html("Hủy đóng")
        handleFast.html("Hủy đóng tất cả")
    }
    else {
        select_selected.html("Tất cả")
        handleDongThung.html("Đóng thùng")
        handleFast.html("Đóng tất cả")
    }
    GetDSThung()
});

//(string maLenh, string po, string colorid, string dausize, string tuthung, string denthung)
function GetDSThung() {
    let valueDH = $("#DH_PKL").val()
    let valuePO = $("#PO_PKL").val()
    let maPKL = $("#DVSX_PKL").val()
    let isDecat = $("#DVSX_PKL option:selected").data("isdecat")
    let $tr = $("#tbody tr.active");
    if (!$tr.length) {
        $tr = $("#tbody tr:not(.active)").first();
    }
    const textSelect = $(".select-selected").text()
    const action = textSelect == "Chưa đóng" ? 0 : textSelect == "Đã đóng" ? 1 : 2
    const min = $tr.data("min");
    const max = $tr.data("max");
    if (!min || !max) return
    var actionSelect = action ?? 2
    $.ajax({
        async: false,
        url: `/api/DongThung/GetDSThung?maplk=${maPKL}&poid=${valuePO}&mahang=${valueDH}&tuthung=${min}&denthung=${max}&madvsx=${actionSelect}&isDeCat=${isDecat}&dotsx=a`,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ArrDsDongThung = data
            renderDsThung(data)
        }
    });
}


//render data
function renderTbl_DS_KHDT(data) {
    //malenh, po, colorid, dausize, tuthung, denthung
    let html = ''
    if (data.length > 0) {

        data.map((x, index) => {
            let max = x.Max
            let min = x.Min
            const formatted = moment(x.NgayLapKH, "YYYY-MM-DD").format("DD/MM/YYYY");
            html += `<tr class="" data-madh=${x.MaDH} data-po=${x.PO} data-poid=${x.POID}
                data-dausize="${x.DauSize}" data-tuthungdenthung="${x.TuThungDenThung}" data-size="${x.Size}" data-tenmau=${x.TenMau}
                data-mapkl=${x.MaPKL} data-madvsx=${x.MaDVSX} data-dot=${x.Dot} data-rowindex = ${index} data-min=${min} data-max=${max} data-id="thung${min}${max}"
            >`
            html += `
                <td class="d-none">${x.MaDH}</td>
                <td>${x.MaHang}</td>
                <td>${x.PO}</td>
                <td>${x.TenDVSX}</td>
                <td>${x.TenMau}</td>
               
                <td>${x.DauSize}</td>
                 <td>${x.Size}</td>
                <td>${x.TuThungDenThung}</td>
                <td class="totalThung"><span class="slthtt">${x.SLTH}</span>/${x.TongThung}</td>
                <td>${formatted}</td>
                
            `
            html += '</tr>'
        })

        tobdy_tbl_DS_KHDT.html(html);
        tobdy_tbl_DS_KHDT.find("tr:first").trigger("click");
    }
    else {
        /* createViewDxDataGridTable1(data);*/
        tobdy_tbl_DS_KHDT.html(html);
    }
}



function renderDsThung(data) {
    let html = "";
    const textSelect = $(".select-selected").text()
    const action = textSelect == "Chưa đóng" ? 0 : textSelect == "Đã đóng" ? 1 : 2
    if (data.length > 0) {
        if (isSave == true) {
            $("#tbl_DS_KHDT tbody tr.active .totalThung span").html(CountDongThung(data))
        }
        let check = true;
        if (data[0].SttThung_decat != 0) {
            check = false;
        }
        data.forEach(x => {
            let stt = check == true ? x.SttThung_temp : x.SttThung_decat


            html += `
            <div style="padding: 0px 3px;" data-sl="${x.thung}" class="thung-item ${x.IsNhapKho == true ? "nhapkho" : ""}" data-sttThung=${x.SttThung} data-thunghienthi=${stt} id="thung${x.SttThung}">
                <p class="mb-0">${stt}</p>
                    <hr />
                <div class= 'd-flex justify-contents-center'>
                    ${x.IsNhapKho == true ? img_daNK : x.IsDongThung == false ? img_chuaDongThung : img_daDongThung}
                    ${action == 1 ? `${x.IsNhapKho == false ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}` : `${(x.IsDongThung == false) ? `<input type="checkbox" />` : `<input type="checkbox" checked disabled />`}`}
                </div>
                
            </div>
            `
        })
        $("#dsThung").html(html);
        // Sau khi tất cả các phần tử đã được thêm vào DOM, gọi hàm tippy()
        data.forEach((x, index) => {

            //0: size, 1: đầu size, 2: size: 3: số lượng
            let stt = check == true ? x.SttThung : x.SttThung_decat
            if (x.thung.includes('@')) {
                let _thung = x.thung.split('@');

                let html = ''
                let html1 = ''
                let txtNhapKho = ''
                let dsSize = []
                let dataNew = []
                let objThung = {
                    TenMau: "",
                    DauSize: "",
                    Size: "",
                    SL: 0,
                    Check: ""
                }

                _thung.map((item, index) => {
                    let checkLast = index == _thung.length - 1 ? true : false
                    let _thungItem = item.split('|');
                    let _tenmau = _thungItem['0'];
                    let _dausize = _thungItem['1'];
                    let _size = _thungItem['2'];
                    let _soluong = _thungItem['3'];
                    txtNhapKho = x.IsNhapKho == true ? "Đã nhập kho" : "";
                    dsSize.push(_size)

                    let newObj = { ...objThung }
                    newObj.TenMau = _tenmau
                    newObj.DauSize = _dausize
                    newObj.Size = _size
                    newObj.SL = _soluong
                    newObj.Check = txtNhapKho
                    dataNew.push(newObj)
                })

                // Bước 1: Nhóm các đối tượng theo TenMau và DauSize
                let groupedData = dataNew.reduce((acc, item) => {
                    let key = `${item.TenMau}|${item.DauSize}|${item.Check}`;
                    if (!acc[key]) {
                        acc[key] = { TenMau: item.TenMau, DauSize: item.DauSize, Check: item.Check };
                    }
                    acc[key][item.Size] = item.SL;
                    return acc;
                }, {});

                // Bước 2: Chuyển đổi cấu trúc dữ liệu sang mảng
                let pivotData = Object.values(groupedData);

                let uniqueSizes = Array.from(new Set(dsSize));
                let renderSize = ""
                uniqueSizes.map(item => {
                    renderSize += `
                        <th class="colSize">${item}</th>
                    `
                })

                // In ra HTML
                let table = `<table id="tbl_tippy">
                    <thead>
                        <tr>
                            <th rowspan="2">Tên Màu</th>
                            <th rowspan="2">Đầu Size</th>
                            <th colspan=${uniqueSizes.length}>Size</th>
                            <th rowspan="2">Tổng</th>
                        </tr>
                        <tr>
                            ${renderSize}
                        </tr>
                    </thead>
                <tbody>`;
                pivotData.forEach(row => {
                    table += `<tr>
                    <td>${row.TenMau}</td>
                    <td>${row.DauSize}</td>
                    ${renderSizeBody(row, uniqueSizes)}
                </tr>`;
                });
                table += '</tbody></table>';
                table += `<p style="margin-top: 8px;">${pivotData[0].Check}</p>`

                tippy(`body #thung${x.SttThung}`, {
                    //content: `Size: ${x.Size}<br>SL: ${x.SoLuongSP}<br>Màu: ${x.TenMau}<br>Đầu Size: ${x.DauSize}`,
                    allowHTML: true,
                    content: table,
                    interactive: true,
                });
            }
            else {
                let _thung = x.thung.split('|');
                let _tenmau = _thung['0'];
                let _dausize = _thung['1'];
                let _size = _thung['2'];
                let _soluong = _thung['3'];
                let txtNhapKho = ''
                txtNhapKho = x.IsNhapKho == true ? "Đã nhập kho" : "";
                tippy(`body #thung${x.SttThung}`, {
                    content: `Size: ${_size}<br>SL:${_soluong}<br>${txtNhapKho}`,
                    allowHTML: true,
                    interactive: true,
                });
            }
        });


    }
    else {
        if (action == 1) {
            $("#dsThung").html("<h3>Thùng chưa được đóng</h3>");
        }
        else {
            $("#dsThung").html("<h3>Thùng đã được đóng hết</h3>");
        }
    }
}
function renderSizeBody(row, uniqueSizes) {
    let html = ""
    let sum = 0
    uniqueSizes.map(size => {
        html += `
            <td>${row[size] || ''}</td>
        `
        sum += Number(row[size]) || 0;
    })
    html += `<td>${sum}</td>`
    return html;
}

///modify filter DT
var dsDonhang = []
function GetMaHangDH() {
    $.ajax({

        url: `/api/KeHoachDongThung/GetOption?Action=Get_DH_PKL&Para1=All&Para2=All&Para3=0&Para4=0&UserName=${username}&status=1&IsNhapKho=0&IsLuanChuyen=0`,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length == 0) {
                $('#DVSX_PKL').empty();
                $('#PO_PKL').empty();
                EmtyTable();
            }
            else {

                let html = ``;
                data.forEach(item => {
                    html += `<option data-madh="${item.GopDH}" value="${item.MaDH}" >${item.Text}</option>`;
                });

                $("#DH_PKL").html(html)
                GetPO()
            }


        }
    });
}

function GetPO(para1) {
    let valueDH = $("#DH_PKL").val()
    $.ajax({
        url: `/api/KeHoachDongThung/GetOption?Action=Get_PO_PKL&Para1=${valueDH}`,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length == 0) {
                $('#PO_PKL').empty();
                EmtyTable();
            }
            else {
                let html = ``;
                data.forEach(item => {
                    html += `<option data-madh="" value="${item.POID}" >${item.PO}</option>`;
                });

                $("#PO_PKL").html(html)
                if (para1 == 1) {
                    $("#PO_PKL").val(POBarCode).trigger("change", true)
                } else
                    GetDVSX_SS()
            }
        }
    });
}

function GetDVSX_SS(para1) {
    let valueDH = $("#DH_PKL").val()
    let valuePO = $("#PO_PKL").val()
    $.ajax({

        url: `/api/KeHoachDongThung/GetOption?Action=Get_DVSX_PKL&Para1=${valueDH}&Para2=${valuePO}`,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length == 0) {
                EmtyTable();
            } else {
                let html = ``;
                data.forEach(item => {
                    html += `<option data-isdecat="${item.IsDecat}" value="${item.MaPKL}" >${item.MaPKL}</option>`;
                });

                $("#DVSX_PKL").html(html)
            }
            if (para1 == 1) {
                $("#DVSX_PKL").val(PKLBarCode).trigger("change", true)
            }
            else {
                GetDS_KHDT()
                LoadDTDescription()
            }

        }
    });
}



$(document).ready(function () {
    GetMaHangDH();
    $("#DH_PKL").select2({
        placeholder: "",
    });
    $("#PO_PKL").select2({
        placeholder: "",
    });
    $("#DVSX_PKL").select2({
        placeholder: "",
    });
})



function EmtyTable() {
    var tbody = document.querySelector("#tbl_DS_KHDT tbody");
    var rows =
        `  <tr><td colspan="11" rowspan="3">Chưa có Thùng đã đóng</td> </tr>
            <tr></tr>
            <tr></tr>`;
    tbody.innerHTML = rows;
    $('#dsThung').empty();
}


$(document).on('keydown', e => {
    if (e.key === 'Enter') {
        if ($('#QRScan:focus').length > 0) {
            GetDHBarCode(e.target.value.trim())
            $("#QRScan").val("")
        }
        else {
            iziToast.warning({
                title: 'Warning',
                message: 'Vui lòng chọn vào ô QR scan.',
                position: 'topRight'

            });
        }
    }
});
async function barcodeInput() {
    if (barcodeLocal.trim() != "") {
        GetDHBarCode(barcodeLocal);

    } else {
        showToast("warning", "Vui lòng nhập barcode vào ô QR scan.");
        PlayAudioError();
    }
}

let arrLichSuQuet = [];
async function GetDHBarCode(value) {
    var url = `/api/ScanBarcodeDT_NK/Get?Action=GetDH_Barcode&para1=${value.trim()}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.Table.length > 0) {

            const dataInfor = {
                DauSize: data.Table[0].DauSize,
                Size: data.Table[0].Size,
                SttThung: data.Table[0].SttThung,
                SLSP: data.Table[0].SoLuongSP,
                PO: data.Table[0].PO,
                TenMau: data.Table[0].TenMau,
                IsDongThung: data.Table[0].IsDongThung
            }


            DHBarCode = data.Table[0].MaGop
            POBarCode = data.Table[0].POID
            PKLBarCode = data.Table[0].MaPKL

            let madhCurrent = $("#DH_PKL").val()
            let poCurrent = $("#PO_PKL").val()
            let mapklCurrent = $("#DVSX_PKL").val()


            if (!data.Table[0].MaGop) {
                showToast("warning", "Barcode không tồn tại trong danh sách")
                PlayAudioError()
                return
            }

            const $matchedDH = $("#DH_PKL option").filter(function () {
                return $(this).val() === DHBarCode;
            });

            if ($matchedDH.length === 0) {
                showToast("warning", "Mã hàng chưa được duyệt")
                PlayAudioError()
                return
            }


            // khách đơn hàng
            if (madhCurrent.trim() != DHBarCode) {
                ShowSwith(data, 1)
                PlayAudio()
            } else if (POBarCode != poCurrent) {
                ShowSwith(data, 2)
                PlayAudio()
            } else if (mapklCurrent != PKLBarCode) {
                ShowSwith(data, 3)
                PlayAudio()
            }
            else {

                const sttthung = Number(data.Table[0].SttThung);

                let $tbody = $('#tbody');

                let $activeRow = $('#tbody').find('tr.active').first();

                let $tr = $tbody.find('tr').filter(function () {
                    const min = Number($(this).data('min'));
                    const max = Number($(this).data('max'));
                    return sttthung >= min && sttthung <= max;
                }).first();


                const minActive = $activeRow.data('min')
                const maxActive = $activeRow.data('max');

                if (sttthung >= minActive && sttthung <= maxActive) {
                    SaveDongThungBarCode(data.Table[0], dataInfor)
                    PlayAudio()
                } else {
                    $tr.prependTo($tbody);

                    $tr.trigger('click');

                    $tbody.closest('div').animate({
                        scrollTop: 0
                    }, 300);
                    SaveDongThungBarCode(data.Table[0], dataInfor)
                    PlayAudio()
                }
            }
        } else {
            showToast("warning", "BarCode không tồn tại")
            PlayAudioError()
        }
    } catch (error) {
        console.error(error.message);
    }
}

async function SaveDongThungBarCode(data, dataInfor) {
    var arr = []
    arr.push({
        SttThung: data.SttThung,
        MaDH: data.MaGop,
        MaPKL: data.MaPKL,
        MaDVSX: null,
        POID: data.POID,
        NgayDongThung: null,
        IsDongThung: true

    })
    if (arr.length > 0)
        SaveDongThung(arr, true, dataInfor)
}

$(function () {
    $("#DH_PKL").on('change', function (e, isTriggered) {
        if (isTriggered) {
            GetPO(1);
        } else {
            GetPO();

        }
    });
    $("#PO_PKL").on('change', function (e, isTriggered) {
        if (isTriggered) {
            GetDVSX_SS(1)
        }
        else {
            GetDVSX_SS()
        }

    });
    $("#DVSX_PKL").on('change', function (e, isTriggered) {
        if (isTriggered) {
            GetDS_KHDT()
            LoadDTDescription()
        } else {
            GetDS_KHDT()
            LoadDTDescription()
        }
    });
})
function ShowSwith(data, value) {
    let madhTT = $("#DH_PKL option:selected").data("madh")
    let potext = $("#PO_PKL option:selected").text()
    let mapkltext = $("#DVSX_PKL option:selected").text()

    $("#newVTDH").text(data.Table[0].MaDH)
    $("#newVTPO").text(data.Table[0].PO)
    $("#newVTPKL").text(data.Table[0].MaPKL)

    $("#currentVTDH").text(madhTT)
    $("#currentVTPO").text(potext)
    $("#currentVTPKL").text(mapkltext)

    showConfirmSwitchVT(async function () {
        if (value == 1) {
            $("#DH_PKL").val(data.Table[0].MaGop).trigger("change", true)
            barcodeLocal = ""
        }
        if (value == 2) {
            $("#PO_PKL").val(data.Table[0].POID).trigger("change", true)
            barcodeLocal = ""
        }
        if (value == 3) {
            $("#DVSX_PKL").val(data.Table[0].MaPKL).trigger("change", true)
            barcodeLocal = ""
        }
    })
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
    $(".openScannerBtn").modal("show")
})
/// Kiệt
/*let dxTable1;*/
let dxLichSuQuetDongThung;
let lstLichSuQuetDongThung = [];
/// EVENT
$(document).ready(function () {
    createViewDxLichSuQuetDongThung();
})

$(function () {
    $("#btnSearch").on("click", function () {
        const value = $("#QRScan").val().trim();
        GetDHBarCode(value)
        $("#QRScan").val("")
    })
})
//GetThongTinDT
function scanInformation(d) {
    $("#txtDauSize").text(d.DauSize);
    $("#txtSize").text(d.Size);
    $("#txtSTT").text(d.SttThung);
    $("#txtSLSP").text(d.SLSP);
    $("#txtPO").text(d.PO);
    $("#txtMauVT").text(d.TenMau);
}

// dxDataGrid
function createViewDxLichSuQuetDongThung(data) {

    dxLichSuQuetDongThung = $("#dxLichSuQuetDongThung").dxDataGrid({
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
        filterRow: { visible: false },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "PO",
                caption: "PO",
                alignment: "center",
            },
            {
                dataField: "SttThung",
                caption: "STT Thùng",
                alignment: "center",
            },
            {
                dataField: "TenMau",
                caption: "Màu",
                alignment: "center",
            },
            {
                dataField: "DauSize",
                caption: "Đầu Size",
                alignment: "center",
            },
            {
                dataField: "Size",
                caption: "Size",
                alignment: "center",
            },
            {
                dataField: "SLSP",
                caption: "SLSP",
                alignment: "center",
            },
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
        }
    }).dxDataGrid("instance");

    $("#Layer_1").click()
}

var dataImage = ''
var dataImageList = []
async function LoadDTDescription() {
    let madh = $("#DH_PKL").val()
    let poid = $("#PO_PKL").val()
    let mapkl = $("#DVSX_PKL").val()
    var url = `/api/ScanBarcodeDT_NK/Get?Action=GetDescriptionPKL&para1=${madh}&para2=${poid}&para3=${mapkl}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.Table.length > 0) {
            dataImage = data.Table[0].UrlHinhAnh
            GetListImg(data.Table[0].UrlPdf)
        } else {
            dataImage = []
            dataImageList = []
        }

    } catch (error) {
        console.error(error.message);
    }

}
async function GetListImg(value) {
    var url = `/api/ScanBarcodeDT_NK/GetImgPDF?para1=${value}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (dataImageList)
            dataImageList = data
        else
            dataImageList = []
        openImageModal(dataImage, dataImageList);
    } catch (error) {
        console.error(error.message);
    }
}

// Variables
var currentSlideIndex = 0;
var slideImages = [];
var autoSlideInterval = null;
var isTransitioning = false;

// Open Modal Button
$('#openModalBtn').on('click', function () {
    if ($("#DVSX_PKL").val() == null) {
        showToast("warning", "Vui lòng chọn mã PKL")
        return
    }
    $("#imageModal").modal("show")
});

// Main Function
function openImageModal(dataImage, dataImageList) {
    // Reset sections
    $('#singleImageSection').hide();
    $('#imageListSection').hide();
    $('#noImageSection').hide();

    // Stop auto-slide
    stopAutoSlide();

    var hasSingleImage = false;
    var hasImageList = false;

    // Kiểm tra hình ảnh đơn
    if (dataImage !== null && dataImage !== undefined && dataImage !== '') {
        $('#singleImage').attr('src', `/Content/UrlPDFDongThung/112043_DOC_VN/${dataImage}`);
        $('#singleImageSection').show();
        hasSingleImage = true;
    }

    // Kiểm tra danh sách hình ảnh
    if ($.isArray(dataImageList) && dataImageList.length > 0) {
        setupCarousel(dataImageList);
        $('#imageListSection').show();
        hasImageList = true;
    }

    // Nếu không có hình nào
    if (!hasSingleImage && !hasImageList) {
        $('#noImageSection').show();
    }

    // Hiển thị modal

}

// Setup Carousel
function setupCarousel(images) {
    slideImages = images;
    currentSlideIndex = 0;
    isTransitioning = false;

    var $track = $('#carouselTrack');
    var $indicators = $('#customIndicators');

    // Clear old content
    $track.empty();
    $indicators.empty();

    // Add slides
    $.each(images, function (index, imgSrc) {
        var $slide = $('<div class="carousel-slide-item"></div>');
        var $img = $('<img>').attr('src', imgSrc).attr('alt', 'Hình ' + (index + 1));
        $slide.append($img);
        $track.append($slide);

        // Add indicator
        var $btn = $('<button></button>');
        if (index === 0) {
            $btn.addClass('active');
        }
        $btn.on('click', function () {
            goToSlide(index);
        });
        $indicators.append($btn);
    });

    // Update counter
    updateCounter();

    // Setup navigation
    $('#prevBtn').off('click').on('click', prevSlide);
    $('#nextBtn').off('click').on('click', nextSlide);

    // Start auto-slide
//    startAutoSlide();
}

// Show Slide with smooth transition
function showSlide(index) {
    if (slideImages.length === 0 || isTransitioning) return;

    isTransitioning = true;
    currentSlideIndex = index;

    // Calculate transform
    var offset = -100 * index;
    $('#carouselTrack').css('transform', 'translateX(' + offset + '%)');

    // Update indicators with smooth animation
    $('#customIndicators button').removeClass('active');
    $('#customIndicators button').eq(index).addClass('active');

    // Update counter
    updateCounter();

    // Reset transition lock
    setTimeout(function () {
        isTransitioning = false;
    }, 700);
}

// Navigation Functions
function nextSlide() {
    var nextIndex = (currentSlideIndex + 1) % slideImages.length;
    showSlide(nextIndex);
    resetAutoSlide();
}

function prevSlide() {
    var prevIndex = (currentSlideIndex - 1 + slideImages.length) % slideImages.length;
    showSlide(prevIndex);
    resetAutoSlide();
}

function goToSlide(index) {
    if (index !== currentSlideIndex) {
        showSlide(index);
        resetAutoSlide();
    } ``
}

// Update Counter
function updateCounter() {
    $('#slideCounter').text((currentSlideIndex + 1) + ' / ' + slideImages.length);
}

function stopAutoSlide() {
    if (autoSlideInterval) {
        clearInterval(autoSlideInterval);
        autoSlideInterval = null;
    }
}

function resetAutoSlide() {
//    startAutoSlide();
}

// Cleanup when modal closes
$('#imageModal').on('hidden.bs.modal', function () {
    stopAutoSlide();
    slideImages = [];
    currentSlideIndex = 0;
    isTransitioning = false;
});

// Keyboard Navigation
$(document).on('keydown', function (e) {
    if ($('#imageModal').hasClass('show') && $('#imageListSection').is(':visible')) {
        if (e.key === 'ArrowLeft') {
            prevSlide();
        } else if (e.key === 'ArrowRight') {
            nextSlide();
        }
    }
});

// Touch/Swipe Support
var touchStartX = 0;
var touchEndX = 0;

$('#carouselTrack').on('touchstart', function (e) {
    touchStartX = e.originalEvent.touches[0].clientX;
});

$('#carouselTrack').on('touchend', function (e) {
    touchEndX = e.originalEvent.changedTouches[0].clientX;
    handleSwipe();
});

function handleSwipe() {
    var swipeThreshold = 50;
    if (touchEndX < touchStartX - swipeThreshold) {
        // Swipe left - next
        nextSlide();
    }
    if (touchEndX > touchStartX + swipeThreshold) {
        // Swipe right - prev
        prevSlide();
    }
}

function showConfirmSwitchVT(onConfirm) {
    $('#btnConfirmSwitchVT').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#myModalSwitchVT").modal("hide");
    });

    $("#myModalSwitchVT").modal("show");
}

$(function () {
    $("#handleDongThung").on("click", function () {
        ArrThungLap()
    })
    $("#tbody").on("click", "tr", function () {
        $("#tbody").find("tr").removeClass("active")
        $(this).addClass("active")
        GetDSThung()
    })
    $("#handleFast").on("click", function () {
        const textDT = $("#handleDongThung").text() == "Đóng thùng"
        var arr = []
        let valueDH = $("#DH_PKL").val()
        let valuePO = $("#PO_PKL").val()
        let maPKL = $("#DVSX_PKL").val()
        if (!textDT) {
            let xacnhanLength = ArrDsDongThung.filter(x => x.IsNhapKho == 0).length;
            if (xacnhanLength == 0) {
                showToast("warning", "Tất cả các thùng đã được nhập kho không được hủy đóng thùng")
                return
            }

        } else {
            console.log(ArrDsDongThung)
            let xacnhanLength = ArrDsDongThung.filter(x => x.IsDongThung == 0).length;
            if (xacnhanLength == 0) {
                showToast("warning", "Tất cả các thùng đã được đóng")
                return
            }
        }
        ArrDsDongThung.forEach(x => {
            if (x.IsDongThung == !textDT && x.IsNhapKho == false) {
                arr.push({
                    SttThung: x.SttThung,
                    MaDH: valueDH,
                    MaPKL: maPKL,
                    MaDVSX: null,
                    POID: valuePO,
                    NgayDongThung: null,
                    IsDongThung: textDT
                });
            }
        });
        if (textDT) {
            showConfirmModaConfirmDT(async function () {
                if (arr.length > 0)
                    SaveDongThung(arr, textDT)
            })
        } else {
            showConfirmModaCancelConfirmDT(async function () {
                if (arr.length > 0)
                    SaveDongThung(arr, textDT)
            })
        }
    })
    customSelect.on("click", function () {
        if (customSelect.hasClass('open')) {
            customSelect.removeClass('open')
        }
        else {
            customSelect.addClass('open')
        }
    })
})
function ArrThungLap() {
    const sttThungChecked = [];
    $('#dsThung .thung-item').each(function () {
        const $item = $(this);
        const isChecked = $item.find('input[type="checkbox"]').is(':checked');

        if (isChecked) {
            sttThungChecked.push({
                sttThung: $item.data('sttthung'),
            });
        }
    });
    var arr = []
    let valueDH = $("#DH_PKL").val()
    let valuePO = $("#PO_PKL").val()
    let maPKL = $("#DVSX_PKL").val()
    let isDecat = $("#DVSX_PKL option:selected").data("isdecat")
    const textDT = $("#handleDongThung").text() == "Đóng thùng"
    if (sttThungChecked.length == 0) {
        showToast("warning", `Vui lòng chọn thùng để ${textDT ? "đóng" : "hủy"}`)
    } else if (textDT) {

        ArrDsDongThung.forEach(x => {
            const found = sttThungChecked.some(s => s.sttThung === x.SttThung && x.IsDongThung == false && x.IsNhapKho == false);
            if (found) {
                arr.push({
                    SttThung: x.SttThung,
                    MaDH: valueDH,
                    MaPKL: maPKL,
                    MaDVSX: null,
                    POID: valuePO,
                    NgayDongThung: null,
                    IsDongThung: textDT
                });
            }
        });
        if (arr.length > 0)
            SaveDongThung(arr, textDT)
    } else {
        let xacnhanLength = ArrDsDongThung.filter(x => x.IsNhapKho == 0).length;
        if (xacnhanLength == 0) {
            showToast("warning", "Tất cả các thùng đã được nhập kho không được hủy đóng thùng")
            return
        }
        ArrDsDongThung.forEach(x => {
            const found = sttThungChecked.some(s => s.sttThung === x.SttThung && x.IsDongThung == true && x.IsNhapKho == false);
            if (found) {
                arr.push({
                    SttThung: x.SttThung,
                    MaDH: valueDH,
                    MaPKL: maPKL,
                    MaDVSX: null,
                    POID: valuePO,
                    NgayDongThung: null,
                    IsDongThung: textDT
                });
            }
        });
        if (arr.length > 0)
            SaveDongThung(arr, textDT)
    }
}

async function SaveDongThung(arrSaveKT, textDT, dataInfor) {

    if (dataInfor && dataInfor.IsDongThung) {
        scanInformation(dataInfor)
        showToast('warning', `Thùng ${dataInfor.SttThung} đã được đóng thùng`)
        return;
    }


    const request = new Request(`/api/DongThung/UpdateDongThungNhapKho?action=UpdateDT`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSaveKT),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        if (dataInfor) {
            arrLichSuQuet.push(dataInfor)
            scanInformation(dataInfor)
            dxLichSuQuetDongThung.option({ dataSource: arrLichSuQuet })
        }
        showToast("success", `${textDT ? "Đóng" : "Hủy đóng"} thùng thành công`)
        let $tr = $("#tbody tr.active");
        const sltt = parseFloat($tr.find(".slthtt").text()) || 0;

        const newValue = textDT
            ? sltt + arrSaveKT.length
            : sltt - arrSaveKT.length;

        $tr.find(".slthtt").text(newValue);
        GetDSThung()
    } else {
        showToast("error", `${textDT ? "Đóng" : "Hủy đóng"} thùng thất bại`)
    }

}

function showConfirmModaConfirmDT(onConfirm) {
    $('#confirmDT').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();
            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }
        // ✅ Chỉ đóng khi hợp lệ
        $("#confirmCloseBoxModal").modal("hide");
    });

    $("#confirmCloseBoxModal").modal("show");
}
function showConfirmModaCancelConfirmDT(onConfirm) {
    $('#confirmHDT').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }
        // ✅ Chỉ đóng khi hợp lệ
        $("#confirmCancelBoxModal").modal("hide");
    });

    $("#confirmCancelBoxModal").modal("show");
}