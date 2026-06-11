//coding by Dat
//start

const input = $(".toggle_input input");
const img_chuaDongThung = `<svg viewBox="0 0 640 512"><path fill="#db1d19" d="M0 488V171.3c0-26.2 15.9-49.7 40.2-59.4L308.1 4.8c7.6-3.1 16.1-3.1 23.8 0L599.8 111.9c24.3 9.7 40.2 33.3 40.2 59.4V488c0 13.3-10.7 24-24 24H568c-13.3 0-24-10.7-24-24V224c0-17.7-14.3-32-32-32H128c-17.7 0-32 14.3-32 32V488c0 13.3-10.7 24-24 24H24c-13.3 0-24-10.7-24-24zm488 24l-336 0c-13.3 0-24-10.7-24-24V432H512l0 56c0 13.3-10.7 24-24 24zM128 400V336H512v64H128zm0-96V224H512l0 80H128z"/></svg>`
const img_daDongThung = `<svg viewBox="0 0 640 512"><path fill="#3b973c" d="M0 488V171.3c0-26.2 15.9-49.7 40.2-59.4L308.1 4.8c7.6-3.1 16.1-3.1 23.8 0L599.8 111.9c24.3 9.7 40.2 33.3 40.2 59.4V488c0 13.3-10.7 24-24 24H568c-13.3 0-24-10.7-24-24V224c0-17.7-14.3-32-32-32H128c-17.7 0-32 14.3-32 32V488c0 13.3-10.7 24-24 24H24c-13.3 0-24-10.7-24-24zm488 24l-336 0c-13.3 0-24-10.7-24-24V432H512l0 56c0 13.3-10.7 24-24 24zM128 400V336H512v64H128zm0-96V224H512l0 80H128z"/></svg>`
const tobdy_tbl_DS_KHDT = $("#tbl_DS_KHDT tbody")
const slc_lenhsx = $("#slc_lenhsx");
const customSelect = $("#customSelect");
const select_selected = $(".select-selected")
const option = $(".option")
const handleByQuantity = $("#handleByQuantity")
const handleNhapKho = $("#handleNhapKho")
const handleFast = $("#handleFast")
const thung_item = $(".thung-item")
const btn_refresh = $("#btn-refresh")
const btn_hide = $('#btn-hide')
const search = $('#search')
const btn_luuchuyen = $('#btn-luuchuyen')
const slc_kho = $('#slc_kho')
const handleByVitri = $("#handleByVitri")

let currentDate = new Date()
$("#ngayTheoViTri").val(new Date().toISOString().split('T')[0])
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
let username = localStorage.getItem('username1')
let MaLenh = "", PO = "", ColorID = "", DauSize = "", TuThung = "", DenThung = "", MaHang = "", MaPKL = "", MaDVSX = "", MaDVSXHoanThanh, Dot = "", MaDH = "", POID = "", Size = "",
    TenMau, Min = "", Max = "", statuschuyen = 0, isDecat = "", DotSX = "", Makho = "", MaKhoDen = "", MaPKLChuyen = "", isTonKho = "";
let keyup_search_po = "", keyup_search_mahang = ""
let statusHidePKL = 0;
let objSave = {
    SttThung: "",
    MaDH: "",
    MaPKL: "",
    MaDVSX: "",
    POID: "",
    NgayNhapKho: "",
    NgayNhapKho_TC: formattedDateTime,
    IsNhapKho: "",
    StatusChuyen: 0,
    Makho: "",
    //MaKhoDen: ""
}

let arrSave = []
let dtBarCode = [];
let dtDSKHDT = [];

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
        url: `/api/NhapKho/GetDS_KHDT?maplk=${maPKL}&mahang=${valueDH}&poid=${valuePO}&statuschuyen=0&madvsx=0&isDecat=${isDecat}&dotsx=0`,
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
    const action = textSelect == "Chưa nhập" ? 0 : textSelect == "Đã nhập" ? 1 : 2
    const min = $tr.data("min");
    const max = $tr.data("max");
    if (!min || !max) return
    var actionSelect = action ?? 2
    $.ajax({
        async: false,
        url: `/api/NhapKho/GetDSThungV2?maplk=${maPKL}&poid=${valuePO}&mahang=${valueDH}&tuthung=${min}&isDongThung=${actionSelect}&denthung=${max}&isDeCat=${isDecat}`,
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
    let html = ''
    if (data.length > 0) {
        data.map((x, index) => {
            let max = x.Max
            let min = x.Min
            const formatted = moment(x.NgayLapKH, "YYYY-MM-DD").format("DD/MM/YYYY");

            html += `<tr  data-madh=${x.MaDH} data-po=${x.PO} data-poid=${x.POID}
              data-dausize="${x.DauSize}" data-dausizeid="${x.DauSizeID}" data-tuthungdenthung="${x.TuThungDenThung}" data-size="${x.Size}" data-sizeid="${x.SizeID}" data-tenmau=${x.TenMau} data-colorid=${x.ColorID}
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
        setTimeout(function () {
            tobdy_tbl_DS_KHDT.find("tr:first").trigger("click");
        }, 50)
    }
    tobdy_tbl_DS_KHDT.html(html);
}





function renderDsThung(data) {
    let html = "";
    const textSelect = $(".select-selected").text()
    const action = textSelect == "Chưa nhập" ? 0 : textSelect == "Đã nhập" ? 1 : 2
    if (data.length > 0) {
        if (isSave == true) {
            $("#tbl_DS_KHDT tbody tr.active .totalThung span").html(CountNhapKho(data))
        }
        let check = true;
        if (data[0].SttThung_decat != 0) {
            check = false;
        }
        data.forEach(x => {
            let stt = check == true ? x.SttThung_temp : x.SttThung_decat

            html += `
           <div style="padding: 0px 3px;height: 52px;" class="thung-item ${x.IsXacNhan == true ? "nhapkho" : ""}" data-sttThung=${x.SttThung} id="thung${x.SttThung}">
                <p>${stt}</p>
                <hr />
                ${x.IsNhapKho == false ? img_chuaDongThung : img_daDongThung}
                ${action == 1 ? `${x.IsXacNhan == false ? `<input class="checkItemDSThung" type="checkbox" />` : `<input class="checkItemDSThung" type="checkbox" checked disabled />`}` : `${(x.IsXacNhan == false && x.IsNhapKho == false) ? `<input class="checkItemDSThung" type="checkbox" />` : `<input class="checkItemDSThung" type="checkbox" checked disabled />`}`}
            </div>
            `
        })

        $("#dsThung").html(html);
        // Sau khi tất cả các phần tử đã được thêm vào DOM, gọi hàm tippy()
        data.forEach(x => {
            //0: size, 1: đầu size, 2: size: 3: số lượng
            let stt = check == true ? x.SttThung : x.SttThung_decat
            if (x.thung.includes('@')) {
                let _thung = x.thung.split('@');
                let txtXacNhan = ''
                let tenKho = ''
                let dsSize = []
                let dataNew = []
                let objThung = {
                    TenMau: "",
                    DauSize: "",
                    Size: "",
                    SL: 0,
                    Check: "",
                    TenKho: tenKho
                }
                _thung.map(item => {
                    let _thungItem = item.split('|');
                    let _tenmau = _thungItem['0'];
                    let _dausize = _thungItem['1'];
                    let _size = _thungItem['2'];
                    let _soluong = _thungItem['3'];
                    txtXacNhan = x.IsNhapKho == true ? "Đã xác nhận" : "";
                    tenKho = isLuuChuyen ? x.TenKhoDen == null ? "" : x.TenKhoDen : x.TenKho == null ? "" : x.TenKho
                    dsSize.push(_size)

                    let newObj = { ...objThung }
                    newObj.TenMau = _tenmau
                    newObj.DauSize = _dausize
                    newObj.Size = _size
                    newObj.SL = _soluong
                    newObj.Check = txtXacNhan
                    newObj.TenKho = tenKho
                    dataNew.push(newObj)
                })
                let groupedData = dataNew.reduce((acc, item) => {
                    let key = `${item.TenMau}|${item.DauSize}|${item.Check}|${item.TenKho}`;
                    if (!acc[key]) {
                        acc[key] = { TenMau: item.TenMau, DauSize: item.DauSize, Check: item.Check, TenKho: item.TenKho };
                    }
                    acc[key][item.Size] = item.SL;
                    return acc;
                }, {});

                // Bước 2: Chuyển đổi cấu trúc dữ liệu sang mảng
                let pivotData = Object.values(groupedData);

                // Hiển thị dữ liệu trên console
                console.log(pivotData);
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
                            <th rowspan="2">Kho</th>
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
                    <td>${row.TenKho}</td>
                    ${renderSizeBody(row, uniqueSizes)}
                </tr>`;
                });
                table += '</tbody></table>';
                table += `<p style="margin-top: 8px;">${pivotData[0].Check}</p>`

                tippy(`body #thung${x.SttThung}`, {
                    allowHTML: true,
                    content: table
                });
            }
            else {
                let _thung = x.thung.split('|');
                let _size = _thung['2'];
                let _soluong = _thung['3'];
                let txtXuatHang = ''
                txtXuatHang = x.IsXacNhan == true ? "Đã xác nhận" : "";

                tippy(`body #thung${x.SttThung}`, {
                    content: `Size: ${_size}<br>SL:${_soluong}<br>${txtXuatHang}${x.IsNhapKho == 1 ? `<br>${isLuuChuyen ? x.TenKhoDen == null ? "" : x.TenKhoDen : x.TenKho == null ? "" : x.TenKho}` : ''}`,
                    allowHTML: true
                });
            }
        });
    }
    else {
        if (typeAction == 1) {
            $("#dsThung").html("<h3>Thùng chưa được nhập</h3>");
        }
        else {
            if (typeAction == 2) {
                $("#dsThung").html("<h3>Thùng chưa được nhập</h3>");
            }
            else {
                $("#dsThung").html('')
            }
        }
    }
}
//end

//func
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

function CountNhapKho(data) {
    const count = data.reduce((acc, curr) => {
        if (curr.IsNhapKho == 1) {
            return acc + 1;
        } else {
            return acc;
        }
    }, 0);
    return count;
}

function CountNhapKhoLuuChuyen(data) {
    const count = data.reduce((acc, curr) => {
        if (curr.StatusChuyen == 2) {
            return acc + 1;
        } else {
            return acc;
        }
    }, 0);
    return count;
}

function FindMax(lstNumber) {
    // Chuyển chuỗi thành mảng các số
    let numbersArray = lstNumber.split(',').map(function (item) {
        return parseInt(item.trim(), 10);
    });
    let maxValue = Math.max.apply(null, numbersArray);
    return maxValue;
}

function FindMin(lstNumber) {
    // Chuyển chuỗi thành mảng các số
    let numbersArray = lstNumber.split(',').map(function (item) {
        return parseInt(item.trim(), 10);
    });
    let maxValue = Math.min.apply(null, numbersArray);
    return maxValue;
}



$(function () {
    $("#handleFast").on("click", function () {
        const textDT = $("#handleNhapKho").text().trim() == "Nhập Kho"
        var arr = []
        let valueDH = $("#DH_PKL").val()
        let valuePO = $("#PO_PKL").val()
        let maPKL = $("#DVSX_PKL").val()


        if (!textDT) {
            let xacnhanLength = ArrDsDongThung.filter(x => x.IsXacNhan == 0).length;
            if (xacnhanLength == 0) {
                showToast("warning", "Thùng đã được xác nhận không được hủy nhập kho")
                return
            }

        } else {
            let xacnhanLength = ArrDsDongThung.filter(x => x.IsNhapKho == 0).length;
            if (xacnhanLength == 0) {
                showToast("warning", "Tất cả các thùng đã được nhập kho")
                return
            }
        }
        ArrDsDongThung.forEach(x => {
            if (x.IsNhapKho == !textDT && x.IsXacNhan == 0) {
                arr.push({
                    SttThung: x.SttThung,
                    MaDH: valueDH,
                    MaPKL: maPKL,
                    MaDVSX: null,
                    POID: valuePO,
                    NgayNhapKho: null,
                    NgayNhapKho_TC: null,
                    IsNhapKho: textDT,
                    StatusChuyen: 0,
                    Makho: "KH002",


                });
            }
        });
        if (textDT) {
            showConfirmModaConfirmDT(async function () {
                const rawDate = $("#ngaynhap").val().trim();
                const selectedDate = moment(rawDate, 'DD/MM/YYYY').format("YYYY-MM-DD")
                if (arr.length > 0) {
                    arr.forEach(x => {
                        x.NgayNhapKho_TC = selectedDate
                    })
                    SaveNhapKho(arr, textDT)
                }

            })
        } else {
            showConfirmModaCancelConfirmDT(async function () {
                const rawDate = $("#ngaynhap").val().trim();
                const selectedDate = moment(rawDate, 'DD/MM/YYYY').format("YYYY-MM-DD")
                if (arr.length > 0) {
                    arr.forEach(x => {
                        x.NgayNhapKho_TC = selectedDate
                    })
                    SaveNhapKho(arr, textDT)
                }

            })
        }
    })
    $("#handleNhapKho").on("click", function () {
        ArrThungLap()
    })
    $("#tbody").on("click", "tr", function () {
        $("#tbody").find("tr").removeClass("active")
        $(this).addClass("active")
        GetDSThung()
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
    const textDT = $("#handleNhapKho").text().trim() == "Nhập Kho"




    if (sttThungChecked.length == 0) {
        showToast("warning", `Vui lòng chọn thùng để ${textDT ? "đóng" : "hủy"}`)
    } else if (textDT) {
        ArrDsDongThung.forEach(x => {
            const found = sttThungChecked.some(s => s.sttThung === x.SttThung && x.IsNhapKho == false);
            if (found) {
                arr.push({
                    SttThung: x.SttThung,
                    MaDH: valueDH,
                    MaPKL: maPKL,
                    MaDVSX: null,
                    POID: valuePO,
                    NgayNhapKho: null,
                    NgayNhapKho_TC: null,
                    IsNhapKho: textDT,
                    StatusChuyen: 0,
                    Makho: "KH002",
                });
            }
        });
        if (arr.length > 0) {
            showConfirmModaConfirmDT(async function () {
                const rawDate = $("#ngaynhap").val()
                const selectedDate = moment(rawDate, 'DD/MM/YYYY').format("YYYY-MM-DD")
                arr.forEach(x => {
                    x.NgayNhapKho_TC = selectedDate
                })
                SaveNhapKho(arr, textDT)
            })
        }
    } else {
        let xacnhanLength = ArrDsDongThung.filter(x => x.IsXacNhan == 0).length;
        if (xacnhanLength == 0) {
            showToast("warning", "Thùng đã được xác nhận không được hủy nhập kho")
            return
        }
        const rawDate = $("#ngaynhap").val()
        const selectedDate = moment(rawDate, 'DD/MM/YYYY').format("YYYY-MM-DD")
        ArrDsDongThung.forEach(x => {
            const found = sttThungChecked.some(s => s.sttThung === x.SttThung && x.IsNhapKho == true && x.IsXacNhan == 0);
            if (found) {
                arr.push({
                    SttThung: x.SttThung,
                    MaDH: valueDH,
                    MaPKL: maPKL,
                    MaDVSX: null,
                    POID: valuePO,
                    NgayNhapKho: null,
                    NgayNhapKho_TC: selectedDate,
                    IsNhapKho: textDT,
                    StatusChuyen: 0,
                    Makho: "KH002",
                });
            }
        });
        if (arr.length > 0)
            SaveNhapKho(arr, textDT)
    }
}
function showConfirmModaConfirmDT(onConfirm) {
    $('#confirmNhapKhoBtn').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();
            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }
        // ✅ Chỉ đóng khi hợp lệ
        $("#confirmNhapKhoModal").modal("hide");
    });

    $("#confirmNhapKhoModal").modal("show");
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

let actionOption = "";
$(document).ready(function () {


    customSelect.on("click", function () {
        if (customSelect.hasClass('open')) {
            customSelect.removeClass('open')
        }
        else {
            customSelect.addClass('open')
        }
    })

    $(document).on('click', '.select-options .option', function () {
        // bỏ active cũ
        $('.select-options .option').removeClass('active');

        // gắn active cho cái vừa chọn
        $(this).addClass('active');

        // lấy data-action
        const action = $(this).data('action');
        if (action == "0") {
            select_selected.html("Chưa nhập")
            handleNhapKho.html("Nhập Kho")
            handleFast.html("Nhập tất cả")
        }
        else if (action == "1") {
            select_selected.html("Đã nhập")
            handleNhapKho.html("Hủy nhập")
            handleFast.html("Hủy nhập tất cả")
        }
        else {
            select_selected.html("Tất cả")
            handleNhapKho.html("Nhập Kho")
            handleFast.html("Nhập tất cả")
        }
        GetDSThung()
    });

})

var dsDonhang = []
function GetMaHangDH() {

    $.ajax({
        url: `/api/KeHoachDongThung/GetOption?Action=Get_DH_PKL&Para1=All&Para2=All&Para3=0&Para4=1&UserName=${username}&status=1&IsNhapKho=1&IsLuanChuyen=0`,
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

        url: `/api/KeHoachDongThung/GetOption?Action=Get_PO_PKLNK&Para1=${valueDH}&Para4=1`,
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

        url: `/api/KeHoachDongThung/GetOption?Action=Get_DVSX_PKLNK&Para1=${valueDH}&Para2=${valuePO}`,
        type: "GET",
        data: {},
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length == 0) {
                EmtyTable();
            } else {
                let html = ``;
                data.forEach(item => {
                    html += `<option data-isdecat="${item.IsDecat}" value="${item.MaPKL}">${item.MaPKL}</option>`;
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

var DHBarCode = ""
var POBarCode = ""
var PKLBarCode = ""


$(function () {
    GetMaHangDH();
    $("#DH_PKL").select2({
        placeholder: "",
    });
    $("#PO_PKL").select2({
        placeholder: "Chọn PO",
    });
    $("#DVSX_PKL").select2({
        placeholder: "Chọn DVSX - SeaSon",

    });
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

function EmtyTable() {
    var tbody = document.querySelector("#tbl_DS_KHDT tbody");
    var rows =
        `
    <tr><td colspan="11" rowspan="3">Chưa có Thùng đã đóng</td> </tr> <tr></tr> <tr></tr>`;
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
                IsNhapKho: data.Table[0].IsNhapKho
            }

            DHBarCode = data.Table[0].MaGop
            POBarCode = data.Table[0].POID
            PKLBarCode = data.Table[0].MaPKL


            let madhCurrent = $("#DH_PKL").val()
            let poCurrent = $("#PO_PKL").val()
            let mapklCurrent = $("#DVSX_PKL").val()


            if (!data.Table[0].MaGop) {
                showToast("warning", "Mã hàng không tồn tại")
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
                    SaveNhapKhoBarCode(data.Table[0], dataInfor)
                    PlayAudio()
                } else {
                    $tr.prependTo($tbody);

                    $tr.trigger('click');

                    $tbody.closest('div').animate({
                        scrollTop: 0
                    }, 300);
                    SaveNhapKhoBarCode(data.Table[0], dataInfor)
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
async function GetDate() {

    const url = `/api/NhapKho/GetDate`;
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

async function SaveNhapKhoBarCode(data, dataInfor) {
    var arr = []

    arr.push({
        SttThung: data.SttThung,
        MaDH: data.MaGop,
        MaPKL: data.MaPKL,
        MaDVSX: null,
        POID: data.POID,
        NgayNhapKho: null,
        NgayNhapKho_TC: await GetDate(),
        IsNhapKho: true,
        StatusChuyen: 0,
        Makho: "KH002",

    })
    if (arr.length > 0)
        SaveNhapKho(arr, true, dataInfor)
}

async function SaveNhapKho(arrSaveKT, textDT, dataInfor) {
    if (dataInfor && dataInfor.IsNhapKho) {
        scanInformation(dataInfor)
        showToast('warning', `Thùng ${dataInfor.SttThung} đã được nhập kho`)
        return;
    }
    const request = new Request(`/api/NhapKho/UpdateDongThungNhapKho?action=UpdateNK`, {
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
            dxLichSuQuetNhapKho.option({ dataSource: arrLichSuQuet })
        }
        showToast("success", `${textDT ? "Nhập" : "Hủy nhập"} thùng thành công`)
        let $tr = $("#tbody tr.active");
        const sltt = parseFloat($tr.find(".slthtt").text()) || 0;

        const newValue = textDT
            ? sltt + arrSaveKT.length
            : sltt - arrSaveKT.length;

        $tr.find(".slthtt").text(newValue);
        GetDSThung()
    } else {
        showToast("error", `${textDT ? "Nhập" : "Hủy nhập"} thùng thất bại`)
    }

}

var arrQRScan = []


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

async function barcodeInput() {
    if (barcodeLocal.trim() != "") {
        GetDHBarCode(barcodeLocal);
    } else {
        showToast("warning", "Vui lòng nhập barcode vào ô QR scan.");
        PlayAudioError();
    }
}


/// Kiệt
let dxLichSuQuetNhapKho;
let lstLichSuQuetNhapKho = [];
/// EVENT
$(document).ready(function () {
    /*createViewDxDataGridTable1();*/
    createViewDxLichSuQuetNhapKho();
})

$(function () {

    $("#btnSearch").on("click", function () {
        const value = $("#QRScan").val().trim();
        GetDHBarCode(value)
        $("#QRScan").val("")
    })

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

    $("#ngaynhap").trigger("click");
    picker1.hide();


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

function createViewDxLichSuQuetNhapKho(data) {

    dxLichSuQuetNhapKho = $("#dxLichSuQuetNhapKho").dxDataGrid({
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
                caption: "SL Sản Phẩm",
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
    console.log("ShowSwith value:  ", value)

    showConfirmSwitchVT(async function () {
        if (value == 1) {
            $("#DH_PKL").val(data.Table[0].MaGop).trigger("change", true)
        }
        if (value == 2) {
            $("#PO_PKL").val(data.Table[0].POID).trigger("change", true)
        }
        if (value == 3) {
            $("#DVSX_PKL").val(data.Table[0].MaPKL).trigger("change", true)
        }
    })
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